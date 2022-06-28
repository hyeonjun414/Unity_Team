using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BattleResultPanel : MonoBehaviour
{
    public ResultEntry entryPrefab;
    public Transform contents;
    public TMP_Text modeName;
    public TMP_Text mapName;
    public ResultEntry[] cachedEntries = null;

    private void Awake()
    {
        // 처음에 엔트리 4개를 생성
        cachedEntries = new ResultEntry[4];
        for (int i = 0; i < cachedEntries.Length; i++)
        {
            cachedEntries[i] = Instantiate(entryPrefab, contents);
            cachedEntries[i].gameObject.SetActive(false);
        }
    }
    private void Start()
    {

        // 모드와 맵 텍스트 지정
        object value;
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MODE, out value);
        ModeType modeType = (ModeType)value;
        modeName.text = GameData.GetMode(modeType);
        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameData.GAME_MAP, out value);
        MapType mapType = (MapType)value;
        mapName.text = GameData.GetMap(mapType);
    }
    private void OnEnable()
    {
        UpdateUI();
    }
    public void UpdateUI()
    {
        // 킬카운트를 기준으로 정렬한 플레이어의 목록을 가져옴.
        List<Character> sortedList =
            BattleManager.Instance.players.OrderByDescending(
                Character => Character.stat.score).ToList();

        for (int i = 0; i < cachedEntries.Length; i++)
        {
            if (i < sortedList.Count)
            {
                cachedEntries[i].UpdateEntry(sortedList[i]);
            }
            else
            {
                cachedEntries[i].ResetEntry();
            }
        }
    }

}
