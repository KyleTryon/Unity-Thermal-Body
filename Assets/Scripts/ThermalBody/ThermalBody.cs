using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalBody : MonoBehaviour
{

    //Component references
    public Rigidbody rb;  // Ensure to fail if these are not set
    public Collider col;  // Ensure to fail if these are not set

    //Attributes
    [Header("Physics Attributes")]
    public ThermalMaterial thermalMaterial;
    public float localAreaTemp;
    public float materialThickness;
    private float surfaceArea;
    private float mass;
    public float CurrentTemp
    {
        get;
        set;
    }

    //[RangeAttribute(0, 1)]
    void Awake()
    {
        try
        {
            surfaceArea = (col.bounds.size.x * col.bounds.size.y);
        }
        catch (System.Exception)
        {
            Debug.LogError("Unable to calculate surface area.");
            throw;
        }
        mass = rb.mass;
        CurrentTemp = localAreaTemp;
    }

    public void AddEnergy(float joules)
    {
        CurrentTemp += joules;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        GameObject collidedObject = collisionInfo.gameObject;
        ThermalBody foriegnThermal = collidedObject.GetComponent<ThermalBody>();
        if (foriegnThermal) // Check if the collision is with a ThermalBody
        {
            float tempDelta = this.CurrentTemp - foriegnThermal.CurrentTemp;
            if (tempDelta > 0.01 || tempDelta < -0.01)
            {
                float energyTransferRate = FourierCalc(thermalMaterial.thermalConductivity, surfaceArea, materialThickness, tempDelta) / (thermalMaterial.materialSpecificHeat * mass);
                AddEnergy(energyTransferRate * Time.deltaTime);
            }

            if (this.CurrentTemp - foriegnThermal.CurrentTemp > 0.001)
            {
                rb.sleepThreshold = 0f;
            }
            else
            {
                rb.sleepThreshold = 0.005f;
            }
        }
 
    }

    void FixedUpdate()
    {
        // Radiate heat
        if (CurrentTemp != localAreaTemp)
        {
            Radiate(Time.deltaTime);
        }
        
    }

    public float GetTemp(GameObject target)
    {
        ThermalBody targetThermalBody = target.GetComponent<ThermalBody>(); // What if they have no thermal body?
        return targetThermalBody.CurrentTemp;
    }

    public void Radiate(float timeModifier)
    {
        AddEnergy((StephanBoltzmannCalc(thermalMaterial.emissivity, surfaceArea, CurrentTemp, localAreaTemp) * timeModifier) * -1);
    }

    public float StephanBoltzmannCalc(float emissivity, float area, float temp, float areaTemp) // Thermal Emission
    {
        float SBConstant = (5.67f * Mathf.Pow(10, -8));
        return emissivity * SBConstant *  (Mathf.Pow(temp, 4) - Mathf.Pow(areaTemp, 4 )) * (area * 6);
    }
       

    public float FourierCalc(float thermalConductivity, float surfaceArea, float materialThickness, float tempDelta)  //Thermal transfer
    {
        return(-1 * thermalConductivity * surfaceArea) * (tempDelta / materialThickness);
    }


}
