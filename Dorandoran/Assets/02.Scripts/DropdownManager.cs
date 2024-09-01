using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour
{
    Dropdown mapDropDown;
    public int GetmapDropDownSelectButton()
    {
        return mapDropDown.value;
    }
}
