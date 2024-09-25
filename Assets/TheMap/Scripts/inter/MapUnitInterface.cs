using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUnitInterface : MonoBehaviour
{

}

public interface IMapUnit
{
    void ReData(List<UnitData> goodDatas, UnitData badData);

    UnitData returnData();//返回格子上面存储的unitData信息

    UnitType returnTileType();//返回这个格子的类型信息,这个格子上存储的到底是我方类型还是敌方类型

    void ChangeUnit(unitControl _this);//改变格子上面存储的单位
    unitControl preUnit();//来记录当前上面的我方单位
    Vector3 thisPos();//表示这个单元格子的物理位置,用于在这个格子的物理位置生成单位
    Vector2 returnPos(); //返回六边形坐标
    void changePos(Vector2 pos); //改变坐标
    HexManager returnManager(); //返回hexmanager
    void changeManager(HexManager manager); //改变hexmanager
    GameObject returnGmo(); //返回自己的GameObject
    void Highlight(); //表示这个单元被暂时选中
    void DeHighlight(); //取消这个单元的暂时选中
    void SetSelected(); //表示这个单元已经被确定选择
    void SetInBound(); //表示这单元处于范围内
    void DownBound(); //取消处于范围内
    void DownSelection();//取消确定选择
    void OnMouseSelect(); //开启选择模式
    void DownMouseSelect(); // 取消选择模式
    SelectMode returnMode(); //返回自己当前的选择模式

    void AddBuff(UnitBuff buff);
    /*
     * 还需要这个
     * private void OnMouseEnter()
    {
        if (curMode == SelectMode.rangeMode)
        {
            manager.OnSelectShape(this);
        }

    }
     */

    bool IfHaveFriendCell(); //返回当前格子是否有友方细胞
    void setNoUnit(); //设置当前格子为没有友方细胞。但是不是真的没有！只是当做没有！
    bool canBeMove();//返回当前格子是否可以被进入。（有友方，敌方，

    void DestroyCurUnit(); //删除当前
    BuffMaintainer getMaintainer();


}

public enum SelectMode
{
    pathMode,
    rangeMode,
    zeroMode
}

