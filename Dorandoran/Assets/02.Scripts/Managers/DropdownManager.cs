using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    enum DropType
    {
        FilterDropType,
        MapDropType,
        RoomInputDropType,
        DropType_End
    }   
    
    //enum FilterDropDownType
    //{
    //    Filter00,
    //    Filter01,
    //    Filter02,
    //    FilterType_End
    //}

    //enum MapDropDownType
    //{
    //    Map00,
    //    Map01,
    //    Map02,
    //    MapType_End
    //}

    //enum RoomInputDropDownType
    //{
    //    RoomInput00,
    //    RoomInput01,
    //    RoomInput02,
    //    RoomInputType_End
    //}

    public Dropdown[] mapDropDownList;

    private void Awake()
    {

    }

    private void Start()
    {
        
    }

    // 필터 데이터 드롭다운
    public int GetFilterDropDownSelectButton()
    {
        return mapDropDownList[(int)DropType.FilterDropType].value;
    }

    // 맵 데이터 드롭다운
    public int GetmapDropDownSelectButton()
    {
        return mapDropDownList[(int)DropType.MapDropType].value;
    }

    public int GetRoomInputDropDownSelectButton()
    {
        return mapDropDownList[(int)DropType.RoomInputDropType].value;
    }

}
