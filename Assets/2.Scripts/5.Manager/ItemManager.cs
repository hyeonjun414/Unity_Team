//아이템 매니저: 플레이어와 아이템이 충돌했을 때 아이템 데이터에 맞는 행동을 보내줌.

//아이템은 1번 자리에 먼저, 그 다음 아이템을 먹었을 때 1번 자리에 아이템이 있다면 2번 자리에 넣어줌
//플레이어가 아이템을 사용하면 1번 먼저 사용되고, 사용하는 순간 2번 자리에 있던 아이템이 1번 자리로 감.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eBlendMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent,
}
public class ItemManager : Singleton<ItemManager>
{
    eBlendMode blendMode;
    public List<ItemData> itemList = new List<ItemData>();
    private ItemSlotUI itemSlotUI;
    public int maxCount = 2;

    public Material wallMaterial;


    private void Start()
    {
        itemSlotUI = UIManager.Instance.itemSlotUI;

    }
    private void Awake() {
        if (_instance == null){
            _instance = this;
        }
    }

    public void UseItem(Character player, ItemData data)
    {

        switch (data.itemType)
        {
            case ItemType.HEAL:
                HealPotion(player);
                break;
            case ItemType.POWERUP:
                PowerUpPotion(player);
                break;
            case ItemType.DASH:
                DashItem(player);
                break;
            case ItemType.SEEINGTHORUGH:
                SeeingThrough(player);
                break;
        }
    }

    public void HealPotion(Character player)
    {
        //생명이 2개 늘어남
        player.stat.hp = 2 + player.stat.hp;

        //테스트용 디버그 로그
        Debug.Log(player.nickName + "의 체력이 2 증가합니다.");

        //늘어난 라이프가 5개 이상일 경우 5개로 고정해줌
        if (player.stat.hp > 5)
        {
            player.stat.hp = 5;

            //테스트용 디버그 로그
            Debug.Log(player.nickName + "의 체력이 이미 최대입니다!");
        }
        Debug.Log(player.stat.hp);

        player.statusUI?.UpdateStatusUI();
    }


    public void PowerUpPotion(Character player)
    {
        //공격력이 2배로 증가
        //후에 틱(노트)당으로 변경하기
        StartCoroutine(TwiceDamage(player,5f));
        Debug.Log(player.name + "의 공격력이 두 배로 증가합니다.");
    }


    public void DashItem(Character player)
    {
        //같은 이동을 한 턴에 연속 두번 처리
        //이동 종류: 오른쪽 회전, 왼쪽 회전, 앞, 뒤, 좌, 우
        //앞뒤좌우 이동 시 같은 방향으로 2칸 이동, Vector로는 2씩 이동
        StartCoroutine(TwiceMoveDIstance(player,5f));
        //현 아이템이 사용 되면
        Debug.Log(player.nickName + "의 이동이 두 배로 증가합니다.");
    }


    public void ChangeRender(eBlendMode mode)
    {
        blendMode = mode;
        int minRenderQueue = -1;
        int maxRenderQueue = 5000;
        int defaultRenderQueue = -1;
        switch (blendMode)
        {
            case eBlendMode.Opaque:
                //wallMaterial.SetOverrideTag("RenderType", "");
                wallMaterial.SetFloat("_Mode", (float)eBlendMode.Opaque);
                wallMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                wallMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                wallMaterial.SetInt("_ZWrite", 1);
                wallMaterial.DisableKeyword("_ALPHATEST_ON");
                wallMaterial.DisableKeyword("_ALPHABLEND_ON");
                wallMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                minRenderQueue = -1;
                maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest - 1;
                defaultRenderQueue = -1;

                wallMaterial.color = new Color(wallMaterial.color.r,wallMaterial.color.g,wallMaterial.color.b,1f);
                break;
            case eBlendMode.Cutout:
                wallMaterial.SetOverrideTag("RenderType", "TransparentCutout");
                //wallMaterial.SetFloat("_Mode", (float)eBlendMode.Cutout);
                wallMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                wallMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                wallMaterial.SetInt("_ZWrite", 1);
                wallMaterial.EnableKeyword("_ALPHATEST_ON");
                wallMaterial.DisableKeyword("_ALPHABLEND_ON");
                wallMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast;
                defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

                wallMaterial.color = new Color(wallMaterial.color.r,wallMaterial.color.g,wallMaterial.color.b,0.4f);
                break;
            case eBlendMode.Fade:
                wallMaterial.SetOverrideTag("RenderType", "Transparent");
                //wallMaterial.SetFloat("_Mode", (float)eBlendMode.Fade);
                wallMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                wallMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                wallMaterial.SetInt("_ZWrite", 0);
                wallMaterial.DisableKeyword("_ALPHATEST_ON");
                wallMaterial.EnableKeyword("_ALPHABLEND_ON");
                wallMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast + 1;
                maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Overlay - 1;
                defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                wallMaterial.color = new Color(wallMaterial.color.r,wallMaterial.color.g,wallMaterial.color.b,0.4f);
                break;
            case eBlendMode.Transparent:
                wallMaterial.SetOverrideTag("RenderType", "Transparent");
                wallMaterial.SetFloat("_Mode", (float)eBlendMode.Transparent);
                wallMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                wallMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                wallMaterial.SetInt("_ZWrite", 0);
                wallMaterial.DisableKeyword("_ALPHATEST_ON");
                wallMaterial.DisableKeyword("_ALPHABLEND_ON");
                wallMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                minRenderQueue = (int)UnityEngine.Rendering.RenderQueue.GeometryLast + 1;
                maxRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Overlay - 1;
                defaultRenderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                wallMaterial.color = new Color(wallMaterial.color.r,wallMaterial.color.g,wallMaterial.color.b,0.4f);
                break;
        }
    }
    public void SeeingThrough(Character player)
    {

        StartCoroutine(TransparentTroughWall(5f));


        //오브젝트 알파값
        Debug.Log(player.nickName + "이(가) 벽을 투시합니다.");
    }


    public bool AddNum(ItemData item){
        if (itemList.Count >= maxCount){
            return false;
        }
        else{
            itemList.Add(item);
            itemSlotUI.UpdateUI();
            return true;
        }
        
    }
    public void RemoveNum(ItemData item){

        itemList.Remove(item);
        itemSlotUI.UpdateUI();
    }

    public void SwitchItems(){
        itemList.Reverse();
        itemSlotUI.UpdateUI();
        Debug.Log("아이템 순서를 바꿉니다.");
      
    }
    IEnumerator TwiceMoveDIstance(Character player, float time)
    {
        player.stat.playerMoveDistance +=1;
        yield return new WaitForSeconds(time);
        player.stat.playerMoveDistance -=1;
    }
    IEnumerator TwiceDamage(Character player,float time)
    {
        player.stat.damage +=1;
        yield return new WaitForSeconds(time);
        player.stat.damage -=1;
    }
    IEnumerator TransparentTroughWall(float time)
    {
        ChangeRender(eBlendMode.Transparent);
        yield return new WaitForSeconds(time);
        ChangeRender(eBlendMode.Opaque);
    }

}
