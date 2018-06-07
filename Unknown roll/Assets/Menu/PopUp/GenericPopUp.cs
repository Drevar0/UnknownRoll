using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPopUp : MonoBehaviour
{
    public string PopUpID = "";                         //Utilizzata per capire la posizione e lo script che si vuole eseguire

    public Settings settings;
    public List<GameObject> Childrens;                  //Utilizzato dalle funzioni che richiedono più children, la divisone dello scopo avviene tramite le variabili booleane

    public void KillMePls()
    {
        GameObject.FindGameObjectWithTag("Canvas").GetComponent<PopupHandler>().KillPopup(PopUpID);
    }

    public void CreateServerClient(string T)
    {
        switch (T)
        {
            case "0":
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().Create_Server("NoPort");
                KillMePls();
                break;
            case "1":
                GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().Create_Client("NoClient");
                KillMePls();
                break;
            default:
                settings.Error_Profiler("D002", 0, "Tentativo di creazione server/client da tasti fallito causa codice errato: " + T, 5, true);
                break;
        }
        
    }

}
