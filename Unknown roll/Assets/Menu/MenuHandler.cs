using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public Settings settings;              
    public List<GameObject> MenuElements;          //lista che comprende tutte le schede del menu
    private Commands DoCommand = new Commands();
    public GameObject ConsoleOBJ;



    private void Start()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        DoCommand.settings = settings;

        if (settings == null)
        {
            settings.Error_Profiler("D003", 0, "MenuHandler => Start => Settings", 2, true);
            return;
        }
        else
            if (ConsoleOBJ == null)
            {
                settings.Error_Profiler("D003", 0, "MenuHandler => Start => Console", 2, true);
                return;
            }
    }


    private void Update()
    {
        //Controllo per aprire o meno la console

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (ConsoleOBJ == null)
            {
                settings.Error_Profiler("D001", 0, "Cannot find console", 4, true);
                return;
            }
            if (!ConsoleOBJ.activeSelf)
            {
                ConsoleOBJ.SetActive(true);
                ConsoleOBJ.transform.Find("ConsoleInput").GetComponent<InputField>().Select();
            }
            else
                ConsoleOBJ.SetActive(false);
        }

        if (ConsoleOBJ.transform.Find("ConsoleInput").GetComponent<InputField>().isFocused && ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text != "" && Input.GetKeyDown(KeyCode.Tab))
        {
            settings.Console_Write(ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text, false);

            DoCommand.Esegui_Comando(ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text);

            ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text = "";
        }



        
    }

    public void AddMenuItem (GameObject Item)
    {
        lock (MenuElements)
            if (MenuElements.Find(X => X == Item) == null)
                MenuElements.Add(Item);
    }

    /// <summary>
    /// Gestisce i movimenti all'interno di un menu annidiato
    /// </summary>
    /// <param name="Temp"></param>
    public void SwitchMenu (string Temp)
    {
        try
        {
            //Controllo che venga passato il giusto numero di argomenti
            if (Temp.Split(',').Length != 2)
            {
                settings.Error_Profiler("D002", 0, "SwitchMenu: " + Temp,3, true);
                return;
            }
            //ricerco e controllo esistano quei due menu
            GameObject Actual = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[0])).SingleOrDefault();
            GameObject NewOne = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[1])).SingleOrDefault();
            if (NewOne == null || Actual == null && !Temp.Split(',')[0].ToLower().Equals("all"))
            {
                settings.Error_Profiler("M001", 0, "(MenuHandler => Switch_menu) not found: " + Temp,3, true);
                return;
            }

            if (Temp.Split(',')[0].ToLower().Equals("all"))
            {
                foreach (GameObject T in MenuElements)
                {
                    if (!T.name.Equals("MainMenu"))
                        T.SetActive(false);
                    else
                        Actual = T;
                }
            } 

            //Se il menu attuale è il MainMenu allora va a cambiare il valore di OnScreen Nell'animator, altrimenti disattiva la scheda corrente
            if (Actual.name.Equals("MainMenu"))
            gameObject.GetComponent<Animator>().SetBool("OnScreen", false);
            Actual.SetActive(false);
            //Se il menu in cui si vuole andare è il MainMenu allora va a cambiare il valore di Onscreen nell'animator a true, per permmetterne la visione, altrimenti imposta attivo il menu richiesto
            if (NewOne.name.Equals("MainMenu"))
                gameObject.GetComponent<Animator>().SetBool("OnScreen", true);
            NewOne.SetActive(true);

        }
        catch (Exception e)
        {
            settings.Error_Profiler("D001", 0, Temp + e,2, true);
        }
    }               
    




    public void KillApllication()
    {
        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().KillThreads();
        Application.Quit();
    }
}