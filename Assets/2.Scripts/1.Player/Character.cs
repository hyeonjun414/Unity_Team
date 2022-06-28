using System.Text;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

[System.Serializable]
public class CharacterStatus
{
 public int playerMoveDistance = 1;
    public int damage;
    public int hp;
    public Point curPos;
    public int currentCombo;
    public int score;
    public int kill;
    public int killCount
    {
        get { return kill; }
        set
        {
            kill = value;
            score += 100;
        }
    }
    public int death;
    public int deathCount
    {
        get { return death; }
        set { 
            death = value;
            score -= 50;
        }
    }

}

public class Character : MonoBehaviourPun, IPunObservable
{
    [Header("Node")]
    public TileNode curNode;

    [Header("Custom")]
    public GameObject[] characterform;

    [Header("Player Info")]
    public string nickName;
    private NickNameOnPlayer nameOnPlayer;
    public int playerId;

    [Header("Player State")]
    public CharacterStatus stat = new CharacterStatus();
    public ePlayerInput eCurInput = ePlayerInput.NULL;
    public PlayerState state = PlayerState.Normal;

    [Header("Option")]
    public bool isRegen = false;
    
    [Header("Command")]
    public CharacterRote roteCommand;
    public CharacterInput inputCommand;
    public CharacterMove moveCommand;
    public CharacterAction actionCommand;

    [Header("Audio Effect")]
    public AudioClip attackSound;
    public AudioClip attackMissSound;
    public AudioClip shieldSound;
    public AudioClip getItemSound;
    public AudioSource audioSource;

    [Header("Cam")]
    public Transform camPos;

    [Header("Effect")]
    public GameObject stunEffect;
    public GameObject shieldEffect;

    [Header("UI")]
    public PlayerStatusUI statusUI;

    [HideInInspector]
    public Animator anim;
    public BoxCollider coll;
    public PlayerDir dir;
    public PlayerDir Dir
    {
        get { return dir; }
        set
        {
            dir = value;
            if (dir == PlayerDir.Start)
                dir = PlayerDir.Left;
            else if (dir == PlayerDir.End)
                dir = PlayerDir.Up;
        }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider>();
        nameOnPlayer = GetComponentInChildren<NickNameOnPlayer>();
        audioSource = GetComponent<AudioSource>();

        inputCommand = gameObject.AddComponent<CharacterInput>();
        inputCommand.SetUp(this);
        moveCommand = gameObject.AddComponent<CharacterMove>();
        moveCommand.SetUp(this);
        roteCommand = gameObject.AddComponent<CharacterRote>();
        roteCommand.SetUp(this);
        actionCommand = gameObject.AddComponent<CharacterAction>();
        actionCommand.SetUp(this);

        Dir = PlayerDir.Right;

        float angle = 0f;
        switch (Dir)
        {
            case PlayerDir.Up:
                angle = 0f;
                break;
            case PlayerDir.Right:
                angle = 90f;
                break;
            case PlayerDir.Down:
                angle = 180f;
                break;
            case PlayerDir.Left:
                angle = 270f;
                break;
        }
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
        photonView.RPC("SetUp", RpcTarget.AllBuffered);
        CamManager.Instance.miniMapCam.GetComponent<CameraLock>().EnableCamera(this);
    }

    [PunRPC]
    public void SetUp()
    {
        nickName = photonView.Owner.NickName;
        playerId = photonView.Owner.ActorNumber;
        nameOnPlayer.SetNickName(nickName);

        if (photonView.IsMine)
        {
            CamManager.Instance.FollowPlayerCam(this);
            CamManager.Instance.ActiveCam(CamType.Player);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_GEN, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            UIManager.Instance.statusUI.SetUp(this);
            nameOnPlayer.gameObject.SetActive(false);
        }
        Player ownerPlayer = photonView.Owner;
        Map map = MapManager.Instance.map;
        

        

        // 노드 위치 지정
        Point vec = map.startPos[ownerPlayer.GetPlayerNumber()];

        // 자신의 최초 노드를 지정
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        stat.playerMoveDistance = 1;
        stat.damage = 1;
        stat.hp = 5;
        stat.score = 0;

        // 플레이어 외형 지정
        object characterIndex;
        if (ownerPlayer.CustomProperties.TryGetValue(GameData.PLAYER_INDEX, out characterIndex))
        {
            int index = (int)characterIndex;
            for (int i = 0; i < characterform.Length; i++)
            {
                if (i == index)
                {
                    // 인덱스에 해당하는 캐릭터를 활성화하고 animator를 연결함.
                    characterform[i].SetActive(true);
                    anim = characterform[i].GetComponent<Animator>();
                }
                else
                {
                    characterform[i].SetActive(false);
                }
            }
        }


        // 현재 플레이어 위치 = 최초 노드 위치
        stat.curPos = curNode.tilePos;
        curNode.eOnTileObject = eTileOccupation.PLAYER;
        transform.position = tile.transform.position + Vector3.up;
        anim.speed = 2f;

        isRegen = true;

        UpdateStatus();

    }

    private void Update()
    {
        // 자기자신만 조작됨.
        if (!photonView.IsMine) return;

        if (RhythmHit())
        {
            // 입력, 이동, 회전은 트랜스폼뷰를 통한 동기화로 결과를 동기화 한다.
            inputCommand.Execute();
            roteCommand.Execute();
            moveCommand.Execute();

            // 액션은 각각의 기능을 RPC를 통해 수행하여 다른 클라이언트에게 같은 행위를 하도록 해준다.
            actionCommand.Execute();
        }
        eCurInput = ePlayerInput.NULL;
    }
    public bool RhythmHit()
    {
        if (GameData.InputGetCheck() && state == PlayerState.Normal)
        {
            if(RhythmManager.Instance.BitCheck())
            {
                RhythmManager.Instance.rhythmBox.NoteHit();
            }
            else
            {
                photonView.RPC("Stunned", RpcTarget.All, 0.5f);
                return false;
            }
            
            return true;
        }
        else
        {

            return false;
        }
    }


    public void Damaged(int damage, int playerId)
    {
        if (!photonView.IsMine) return;

        photonView.RPC("SendDamaged", RpcTarget.All, damage, playerId);
    }
    [PunRPC]
    public void SendDamaged(int damage, int playerId)
    {
        stat.hp -= damage;
        statusUI?.UpdateStatusUI();
        anim.SetTrigger("Hit");
        BattleManager.Instance.PlayerAddScore(playerId);
        audioSource.PlayOneShot(attackSound);
        UpdateStatus();

        if (stat.hp <= 0) Die(playerId);
    }

    private void Die(int playerId)
    {
        // 죽은 대상이 해당 클라이언트 플레이어가 아니라면 로그를 보내지 않는다.
        if (!photonView.IsMine) return;

        GameLogManager.Instance.SendDeadLog(nickName);
        
        
        
        if (isRegen)
        {
            // 부활이 가능하고 해당 클라이언트의 플레이어라면
            // 해당 클라이언트에 부활 UI를 표기한다.
            BattleManager.Instance.regenUI.RegenStart(this);
        }
        else
        {
            CamManager.Instance.ActiveCam(CamType.Dead);
        }

        photonView.RPC("SendDie", RpcTarget.All, playerId);



    }
    [PunRPC]
    public void SendDie(int playerId)
    {
        actionCommand.ActionStop();
        anim.SetTrigger("Die");
        ++stat.deathCount;
        BattleManager.Instance.PlayerAddKill(playerId);
        state = PlayerState.Dead;
        coll.enabled = false;
        StartCoroutine("DieRoutine");
        BattleManager.Instance.PlayerOut(this);
        UpdateStatus();
    }
    public void SendUpdateUI()
    {
        photonView.RPC("UpdateStatus", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateStatus()
    {
        statusUI?.UpdateStatusUI();
    }
    IEnumerator DieRoutine()
    {
        float curTime = 0;
        while(true)
        {
            if(curTime >= 2f)
            {
                break;
            }
            curTime += Time.deltaTime;
            transform.Translate(Vector3.down * Time.deltaTime);
            yield return null;
        }
    }


    [PunRPC]
    public void RequestDeleteMe()
    {
        BattleManager.Instance.PlayerOut(this);
    }

    [PunRPC]
    public void Revive(int y, int x)
    {
        coll.enabled = true;

        curNode.eOnTileObject = eTileOccupation.EMPTY;
        curNode = null;

        // 체력 채우고, 위치 초기화하고, 애니메이션 Idle 실행시키기
        if (photonView.IsMine)
        {
            CamManager.Instance.ActiveCam(CamType.Player);
        }
        state = PlayerState.Normal;
        stat.hp = 5;
        curNode = MapManager.Instance.map.GetTileNode(new Point(y, x));
        stat.curPos = curNode.tilePos;
        anim.Play("Idle");
        curNode.eOnTileObject = eTileOccupation.PLAYER;
        transform.position = curNode.transform.position + Vector3.up;
        statusUI?.UpdateStatusUI();

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            stream.SendNext(Dir);
            if (stat != null)
            {
                stream.SendNext(stat.hp);
                stream.SendNext(stat.damage);
                
            }

        }
        else
        {
            //transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            Dir = (PlayerDir)stream.ReceiveNext();
            if (stat != null)
            {
                stat.hp = (int)stream.ReceiveNext();
                stat.damage = (int)stream.ReceiveNext();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            moveCommand.CollidedPlayer();
        }
    }

    public void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Item"){
            audioSource.PlayOneShot(getItemSound);
        }
    }
}

