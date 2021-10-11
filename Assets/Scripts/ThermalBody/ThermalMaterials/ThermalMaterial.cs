using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ThermalMaterial")]
public class ThermalMaterial : ScriptableObject
{
    public string materialName;
    public float thermalConductivity;
    public float materialSpecificHeat;
    public float emissivity;
}
