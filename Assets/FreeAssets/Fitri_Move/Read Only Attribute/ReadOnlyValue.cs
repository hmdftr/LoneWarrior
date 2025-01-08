using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadOnlyValue : MonoBehaviour
{
    [Header("Using ReadOnly Arttibute")]
    [ReadOnly] public int value = 4;

    [Header("Using ReadOnlyWhenPlaying Arttibute")]
    [ReadOnlyWhenPlaying] public int _value = 4;
}