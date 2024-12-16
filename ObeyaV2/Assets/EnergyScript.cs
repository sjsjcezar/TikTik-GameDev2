using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    public int energy = 3, tempEnergy = 3;
    public List <GameObject> energySystem;
    public GameObject energyUI;


    // Start is called before the first frame update
    void Start()
    {
        //energySystem.SetActive(true);
        UpdateEnergy();
    }

    // Update is called once per frame

    public void UpdateEnergy()
    {
        for (int i = 0; i < energy; i++)
        {
            energySystem[i].SetActive(true);
            energySystem[i].GetComponent<Image>().color = Color.white;
        }
        tempEnergy = energy;
    }

    public void ReduceEnergy()
    {
        int i = tempEnergy - 1;
        energySystem[i].GetComponent<Image>().color = Color.red;
        tempEnergy--;
    }

    public void IncreaseEnergy()
    {
        energy += 1;
    }

    public void EnableEnergy()
    {
        energyUI.SetActive(true);
    }

    public void DisableEnergy()
    {
        energyUI.SetActive(false);
    }
}
