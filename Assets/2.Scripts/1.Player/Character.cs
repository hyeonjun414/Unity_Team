using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Cinemachine;

public enum ePlayerInput
{
    NULL,
    MOVE_UP,
    MOVE_RIGHT,
    MOVE_DOWN,
    MOVE_LEFT,
    ROTATE_RIGHT,
    ROTATE_LEFT,
    ATTACK,
    BLOCK,
    USE_ITEM,
    CHANGE_ITEM_SLOT,
}

public enum PlayerDir
{
    Start,
    Up,
    Right,
    Down,
    Left,
    End
}

public enum PlayerState
{
    Normal,
    Move,
    Defend,
    Attack,
    Stun,
    Dead
}

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
    public CharacterStatus stat;
    public ePlayerInput eCurInput = ePlayerInput.NULL;
    public PlayerState state = PlayerState.Normal;
    private int killStreak;//연속킬

    [Header("Option")]
    public bool isRegen = false;
    public int KillStreak
    {
        get { return killStreak; }
        set
        {
            killStreak = value;

            var builder = new StringBuilder();
            if (killStreak == 3)
            {
                builder.Append(PhotonNetwork.LocalPlayer.NickName);
                builder.Append("을(를) 막을 수 없습니다");
                string deadString = builder.ToString();
                photonView.RPC("SendLogToPlayers", RpcTarget.All, deadString);
            }
            if (killStreak == 4)
            {
                builder.Append(PhotonNetwork.LocalPlayer.NickName);
                builder.Append("이(가) 게임을 지배하고 있습니다");
                string deadString = builder.ToString();
                photonView.RPC("SendLogToPlayers", RpcTarget.All, deadString);
            }
            if (killStreak == 5)
            {
                builder.Append(PhotonNetwork.LocalPlayer.NickName);
                builder.Append("이(가) 미쳐 날뛰고 있습니다");
                string deadString = builder.ToString();
                photonView.RPC("SendLogToPlayers", RpcTarget.All, deadString);
            }

        }
    }
    public int defenceCount;
    public int DC
    {
        get { return defenceCount; }
        set
        {
            defenceCount = value;
            if (defenceCount <= 0)
            {
                photonView.RPC("SetNormalState", RpcTarget.All, state);
            }
        }
    }
    public int stunCount;
    public int SC
    {
        get { return stunCount; }
        set
        {
            stunCount = value;
            if (stunCount <= 0)
            {
                photonView.RPC("SetNormalState", RpcTarget.All, state);
            }
        }
    }

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
    AudioSource audioSource;


    [Header("Cam")]
    public Transform camPos;

    [Header("Effect")]
    public GameObject stunEffect;
    public GameObject shieldEffect;
    //private UnityAction OnRhythmHit;

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
        this.audioSource = GetComponent<AudioSource>();


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
        RhythmManager.Instance.OnRhythmHit += RhythmOnChange;
        photonView.RPC("SetUp", RpcTarget.AllBuffered);
        CamManager.Instance.miniMapCam.GetComponent<CameraLock>().EnableCamera(this);
    }

    [PunRPC]
    public void SetUp()
    {
        if (photonView.IsMine)
        {
            CamManager.Instance.FollowPlayerCam(this);
            CamManager.Instance.ActiveCam(CamType.Player);
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable() { { GameData.PLAYER_GEN, true } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        Player ownerPlayer = photonView.Owner;
        Map map = MapManager.Instance.map;
        nickName = photonView.Owner.NickName;

        nameOnPlayer.SetNickName(nickName);

        // 노드 위치 지정
        Point vec = map.startPos[ownerPlayer.GetPlayerNumber()];

        // 자신의 최초 노드를 지정
        TileNode tile = map.GetTileNode(vec);
        curNode = tile;
        CharacterReset();

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
        if (!photonView.IsMine) return;

        if (RhythmHit())
        {
            inputCommand.Execute();
            roteCommand.Execute();
            moveCommand.Execute();
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

    public void CharacterReset()
    {
        stat = new CharacterStatus();
        stat.playerMoveDistance = 1;
        stat.damage = 1;
        stat.hp = 5;
        stat.curPos.y = 0;
        stat.curPos.x = 0;
        stat.currentCombo = 0;
        stat.killCount = 0;
        stat.deathCount = 0;
    }

    [PunRPC]
    public void Damaged(int damageInt, int actorNum)
    {
        stat.hp -= damageInt;

        anim.SetTrigger("Hit");
        audioSource.PlayOneShot(attackSound);

        if (stat.hp <= 0)
        {

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (actorNum == p.ActorNumber)
                {
                    p.AddScore(1);
                }
            }
            Die();

        }
    }
    [PunRPC]
    public void Attack()
    {
        anim.SetTrigger("Attack");
        audioSource.PlayOneShot(attackMissSound);


    }
    [PunRPC]
    public void Block()
    {
        PlaySound((int)eCurInput);
        shieldEffect.gameObject.SetActive(true);
        audioSource.PlayOneShot(shieldSound);

        anim.SetBool("Defend", true);
        DC = 5;
        state = PlayerState.Defend;
    }
    [PunRPC]
    public void Stunned()
    {
        stunEffect.gameObject.SetActive(true);
        anim.SetBool("Stunned", true);
        SC = 5;
        state = PlayerState.Stun;
    }
    public void RhythmOnChange()
    {
        print(photonView.Owner.NickName);
        if (state == PlayerState.Defend)
        {
            DC--;
        }
        if (state == PlayerState.Stun)
        {
            SC--;
        }
    }
    [PunRPC]
    public void SetNormalState(PlayerState ps)
    {
        switch (ps)
        {
            case PlayerState.Defend:
                anim.SetBool("Defend", false);
                shieldEffect.gameObject.SetActive(false);
                defenceCount = 0;
                break;
            case PlayerState.Stun:
                anim.SetBool("Stunned", false);
                stunEffect.gameObject.SetActive(false);
                stunCount = 0;
                break;
        }
        state = PlayerState.Normal;
    }

    private void Die()
    {
        ++(stat.deathCount);
        //if (!photonView.IsMine) return;

        state = PlayerState.Dead;
        if (photonView.IsMine)
        {
            CamManager.Instance.ActiveCam(CamType.Dead);

        }
        KillStreak = 0;
        anim.SetTrigger("Die");
        var builder = new StringBuilder();
        builder.Append(PhotonNetwork.LocalPlayer.NickName);
        builder.Append("이(가) 사망하였습니다");
        string deadString = builder.ToString();
        photonView.RPC("SendLogToPlayers", RpcTarget.All, deadString);
        photonView.RPC("SendLogToPlayersDead", RpcTarget.All);

        if (isRegen && photonView.IsMine)
        {
            // 부활이 가능하고 해당 클라이언트의 플레이어라면
            // 해당 클라이언트에 부활 UI를 표기한다.
            BattleManager.Instance.regenUI.RegenStart(this);
        }

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
        stat.hp = 5;
        curNode = MapManager.Instance.map.GetTileNode(new Point(y, x));
        stat.curPos = curNode.tilePos;
        anim.Play("Idle");
        curNode.eOnTileObject = eTileOccupation.PLAYER;
        transform.position = curNode.transform.position + Vector3.up;

    }

    [PunRPC]
    public void SendLogToPlayers(string msg)
    {

        GameLogManager.Instance.AddQueue(msg);
    }

    [PunRPC]
    public void SendLogToPlayersDead()
    {
        BattleManager.Instance.PlayerOut(this);

    }


    private void OnCollisionEnter(Collision other)
    {
        print("collide");
        if (other.collider.CompareTag("Player") || other.collider.CompareTag("Wall"))
        {
            moveCommand.CollidedPlayer();
        }
    }

    [PunRPC]
    public void SetCommand(ePlayerInput input, int curY, int curX)
    {
        eCurInput = input;
        stat.curPos = new Point(curY, curX);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(state);
            stream.SendNext(Dir);
        }
        else
        {
            state = (PlayerState)stream.ReceiveNext();
            Dir = (PlayerDir)stream.ReceiveNext();
        }
    }


    [PunRPC]
    void PlaySound(int inputType)
    {
        ePlayerInput eCurInput = (ePlayerInput)inputType;
        switch (eCurInput)
        {

            case ePlayerInput.ATTACK:
                audioSource.PlayOneShot(attackSound);
                break;

            case ePlayerInput.BLOCK:
                audioSource.PlayOneShot(shieldSound);
                break;
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item")
        {
            audioSource.PlayOneShot(getItemSound);
        }

    }







    //캐릭터 부활

    [PunRPC]
    public void Revive()
    {

        //캐릭터 목숨 리셋
        stat.hp = 5;
        StartCoroutine(Revival());

    }


    IEnumerator Revival()
    {
        yield return new WaitForSeconds(5f);
        //애니메이션 리셋 

    }







}

