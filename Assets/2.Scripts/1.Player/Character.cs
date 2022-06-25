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
    public int killCount;
    public int deathCount;
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
    AudioListener audioListener;

    [Header("Cam")]
    public Transform camPos;

    [Header("Effect")]
    public GameObject stunEffect;
    public GameObject shieldEffect;

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
        audioListener = GetComponent<AudioListener>();

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
        if (photonView.IsMine)
        {
          //  audioListener.enabled = true;

            CamManager.Instance.FollowPlayerCam(this);
            CamManager.Instance.ActiveCam(CamType.Player);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_GEN, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            BattleManager.Instance.hpUI.SetUp(this);
        }
        Player ownerPlayer = photonView.Owner;
        Map map = MapManager.Instance.map;
        nickName = photonView.Owner.NickName;
        playerId = photonView.Owner.ActorNumber;

        nameOnPlayer.SetNickName(nickName);

        // 노드 위치 지정
        Point vec = map.startPos[ownerPlayer.GetPlayerNumber()];

        // 자신의 최초 노드를 지정
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        stat.playerMoveDistance = 1;
        stat.damage = 1;
        stat.hp = 5;

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
        if (Input.anyKeyDown && RhythmManager.Instance.BitCheck() &&
            state == PlayerState.Normal)
        {
            RhythmManager.Instance.rhythmBox.NoteHit();
            return true;
        }
        else
        {
            return false;
        }
    }


    public void Damaged(int damageInt)
    {
        stat.hp -= damageInt;

        anim.SetTrigger("Hit");
        audioSource.PlayOneShot(attackSound);

        if (stat.hp <= 0) Die();
    }

    private void Die()
    {
        anim.SetTrigger("Die");
        ++stat.deathCount;
        state = PlayerState.Dead;

        // 죽은 대상이 해당 클라이언트 플레이어가 아니라면 실행하지 않는다.
        if (!photonView.IsMine) return;

        GameLogManager.Instance.SendDeadLog(nickName);
        photonView.RPC("RequestDeleteMe", RpcTarget.All);

        if (isRegen && photonView.IsMine)
        {
            // 부활이 가능하고 해당 클라이언트의 플레이어라면
            // 해당 클라이언트에 부활 UI를 표기한다.
            BattleManager.Instance.regenUI.RegenStart(this);
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
        curNode.eOnTileObject = eTileOccupation.EMPTY;
        curNode = null;

        // 체력 채우고, 위치 초기화하고, 애니메이션 Idle 실행시키기
        if (photonView.IsMine)
        {
            CamManager.Instance.ActiveCam(CamType.Player);
        }
        state = PlayerState.Normal;
        stat.hp = 1;
        curNode = MapManager.Instance.map.GetTileNode(new Point(y, x));
        stat.curPos = curNode.tilePos;
        anim.Play("Idle");
        curNode.eOnTileObject = eTileOccupation.PLAYER;
        transform.position = curNode.transform.position + Vector3.up;

    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Dir);
        }
        else
        {
            Dir = (PlayerDir)stream.ReceiveNext();
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

