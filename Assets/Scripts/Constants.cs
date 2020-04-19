using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    public static int LayerCar = 11;
    public static float pixelToMeters = 1 / 64f;
    public static int SortLayerDead
    {
        get
        {
            return SortingLayer.GetLayerValueFromName("Character - back");
        }
    }
}
