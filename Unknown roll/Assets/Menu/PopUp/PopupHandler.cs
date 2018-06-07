using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    public bool D;
    public Settings settings;
    public GameObject InspectorPopupElement;
    public List<GameObject> PopupList;
    public List<GameObject> ActivePopup;        
    public List<string> PopupRequests;

    public void KillPopup (string ID)
    {
        GameObject TempPopup = null;
        lock (ActivePopup)
        {
            try
            {
                if (ActivePopup.Count != 0)
                    TempPopup = ActivePopup.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLowerInvariant().Equals(ID.ToLowerInvariant()));

                if (TempPopup != null)  //se non esiste segnala altrimenti esegue il comando
                {
                    lock(ActivePopup)
                        ActivePopup.Remove(TempPopup);
                    Destroy(TempPopup);
                }
                else
                    settings.Error_Profiler("G003", 0, "(PopupHandler => KillPopup) ID " + ID + " non è attualmente attivo o non esiste", 1, false);
            }
            catch (Exception e)
            {
                settings.Error_Profiler("G005", 0, "(PopupHandler => KillPopup) Tentativo di chiusura popup fallito: ID " + ID + " Errore: " + e , 5, false);
            }
        }
        

    }

    public void EditPopup()
    {

    }

    /// <summary>
    /// Viene utilizzata dai elementi del menu con la seguente formattazione Caller;Name
    /// </summary>
    /// <param name="Temp"></param>
    public void CreatePopup(string Temp)
    {
        CreatePopup(Temp.Split(';')[0], Temp.Split(';')[1], true);
    }


    /// <summary>
    /// Serve per attivare un pop up dato il nome e nel caso delle identifidicazioni specifiche
    /// </summary>
    /// <param name="Name">nome del popup e relative informazioni divise da una virgola</param>
    public void CreatePopup(string Caller, string Name, bool MainThread)
    {
        //nel caso in cui non venga eseguito dal main thread verrebbe generata un eccezzione dati i ristretti comandi di cui dispone, perciò la manda a una lista gestita dal mainthread
        if (!MainThread)
        {
            PopupRequests.Add(Caller + ";" + Name);
            return;
        }


        //Instazio un vettore di 5 elementi che potrei utilizzare in seguito (è necessario inizializzarla al fine i integrità del codice)
        string[] SubStrings = new string[5];

        //Controllo se all'interno della stringa sono presenti delle virgole, se si li splitto e li assegno al vettore, mettendo il primo in Name, essendo il nome del popUP
        if (Name.IndexOf(',') > 0)
            SubStrings = Name.Split(',');
        else
            SubStrings[0] = Name;

        // Guardo se esiste il popUP richiesto altrimenti segnalo l'errore

        GameObject Popup = PopupList.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLower().Equals(SubStrings[0].ToLower()));
        if (Popup == null)
        {
            settings.Error_Profiler("G003", 0, "(PopupHandler => CreatePopup)" + Caller + ": PopUp non trovato: (popupHandler => CreatePopup) => Name: " + SubStrings[0] + " Ricevuto: " + Name, 1, true);
            return;
        }

        //eseguo un controllo sul nome per capire come comportarsi


        switch (SubStrings[0].ToLower())
        {
            case "error":      // nel suo caso bisogna semplicemente agggiungere il testo al pop up precedente perciò controlla se esiste e nel caso non fa nulla
                lock (ActivePopup)
                {
                    try
                    {
                        GameObject TempPopup = null;
                        TextMeshProUGUI ErrorText;
                        if (ActivePopup.Count != 0)
                            TempPopup = ActivePopup.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLower().Equals(SubStrings[0].ToLower()));
                        if (TempPopup == null)  //se non esiste lo istanzia altrimenti non fa nulla
                        {
                            Popup = Instantiate(Popup, InspectorPopupElement.transform);
                            ActivePopup.Add(Popup);

                            //ricavo il testo
                            ErrorText = Popup.GetComponentInChildren<TextMeshProUGUI>();
                            ErrorText.text = "";
                        }
                        else
                            Popup = TempPopup;
                        //aggiungo il testo (sempre se ve ne è uno)
                        ErrorText = Popup.GetComponentInChildren<TextMeshProUGUI>();
                        if (Name.IndexOf(',') > 0)  //controllo il quantitativo di virgole in name
                        {
                            if (!ErrorText.text.Equals("")) //se l'errore non è vuoto vado a capo
                                ErrorText.text += "\n";
                            for (int I = 1; I < SubStrings.Length; I++)     //aggiungo il testo e la virgola tolta fuorchè per il primo
                            {
                                if (I > 1)
                                    ErrorText.text += "," + SubStrings[I];
                                else
                                    ErrorText.text += SubStrings[I];

                            }
                        }


                    }
                    catch(Exception e)
                    {
                        settings.Error_Profiler("G005", 0, "Errore durante la creazione di un popup: " + e, 2, MainThread);
                    }

                }
                break;
            case "directhost":
                lock (ActivePopup)
                {
                    //controllo se è già stato inizializzato
                    GameObject TempPopup = null;
                    if (ActivePopup.Count != 0)
                        TempPopup = ActivePopup.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLower().Equals(SubStrings[0].ToLowerInvariant()));
                    if (TempPopup == null)
                    {
                        Popup = Instantiate(Popup, InspectorPopupElement.transform);
                        ActivePopup.Add(Popup);
                    }
                    else
                        Popup = TempPopup;

                    switch(SubStrings[1])
                    {
                        case "0":
                            Popup.GetComponent<GenericPopUp>().Childrens[0].SetActive(true);
                            Popup.GetComponent<GenericPopUp>().Childrens[1].SetActive(false);
                            break;
                        case "1":
                            Popup.GetComponent<GenericPopUp>().Childrens[0].SetActive(false);
                            Popup.GetComponent<GenericPopUp>().Childrens[1].SetActive(true);
                            break;
                        default:
                            settings.Error_Profiler("M005",0,"",5,MainThread);
                            break;
                    }
                }
                break;
            default:
                Popup = Instantiate(Popup, InspectorPopupElement.transform);
                ActivePopup.Add(Popup);
                break;

        }
    }

    void Update()
    {
        //Controllo se vi siano PopUp da dover displayare
        if (PopupRequests.Count != 0)
        {
            lock (PopupRequests)
            {
                foreach (string PopupRichiesto in PopupRequests)
                {
                    CreatePopup(PopupRichiesto.Split(';')[0], PopupRichiesto.Split(';')[1], true);
                }
                PopupRequests.Clear();
            }
        }
    }

}


