using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupHandler : MonoBehaviour
{
    public Settings settings;
    public List<GameObject> PopupList;
    public List<GameObject> ActivePopup;
    public List<string> PopupRequests;

    public void KillPopup ()
    {

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
        if (Name.IndexOf(',') >= 0)
            SubStrings = Name.Split(',');

        // Guardo se esiste il popUP richiesto altrimenti segnalo l'errore

        GameObject Popup = PopupList.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLower().Equals(SubStrings[0].ToLower()));
        if (Popup == null)
        {
            settings.Error_Profiler("G003", 0, Caller + ": PopUp non trovato: (popupHandler => CreatePopup) => Name: " + SubStrings[0] + " Ricevuto: " + Name, 4, true);
            return;
        }

        //eseguo un controllo sul nome per capire come comportarsi


        switch (SubStrings[0].ToLower())
        {
            case "error":      // nel suo caso bisogna semplicemente agggiungere il testo al pop up precedente perciò controlla se esiste e nel caso non fa nulla
                lock (ActivePopup)
                {
                    GameObject TempPopup = null;
                    if (ActivePopup.Count != 0)
                        TempPopup = ActivePopup.Find(T => T.GetComponent<GenericPopUp>().PopUpID.ToLower().Equals(SubStrings[0].ToLower()));
                    if (TempPopup == null)  //se non esiste lo istanzia altrimenti non fa nulla
                    {
                        Popup = Instantiate(Popup);
                        ActivePopup.Add(Popup);
                    }
                    else
                    {
                        TempPopup.
                    }

                }
                break;
            default:
                Popup = Instantiate(Popup);
                ActivePopup.Add(Popup);
                break;

        }

        //gli resetto la posizione

        Popup.transform.parent = GameObject.FindGameObjectWithTag("PopUp").transform;

        Popup.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);  
        Popup.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        Popup.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);  //width, height

        //Eseguo uno switch sul PopUp appena avviato per capire se ha delle funzioni da eseguire durante l'avvio o delle modifiche da apportare in base alla chiamata
        switch (Popup.GetComponent<GenericPopUp>().PopUpID)     //see also in GenericPopUP
        {
            case "PlayDirectHost":  //devo inserirli lo stato che si vuole avere attualmente sul popup
                Popup.GetComponent<GenericPopUp>().DirectConnectOrHostGame = Convert.ToByte(SubStrings[1]);// Int32.Parse(SubStrings[1]);
                break;
        }

        //GameObject PopUP = PopupList.Find(T => T.name.Equals(SubStrings[0]));
        //if (PopUP == null && SubStrings[0].ToLower().Equals("all"))
        //    lock (MenuElements)
        //    {
        //        foreach (GameObject T in MenuElements)
        //            if (!T.name.Equals("Sfondo"))
        //                T.SetActive(false);
        //    }
        //else
        //{
        //    settings.Error_Profiler("G003", 0, Caller + ": PopUp non trovato: (CallPopUpByName) => Name: " + SubStrings[0] + " Ricevuto: " + Name, 4, true);
        //    return;
        //}
        //PopUP.SetActive(true);

        //Eseguo uno switch sul PopUp appena avviato per capire se ha delle funzioni da eseguire durante l'avvio
        //switch (PopUP.GetComponent<GenericPopUp>().PopUpID)     //see also in GenericPopUP
        //{
        //    case "PlayDirectHost":
        //        PopUP.GetComponent<GenericPopUp>().DirectConnectOrHostGame = Convert.ToByte(SubStrings[1]);// Int32.Parse(SubStrings[1]);
        //        break;
        //}
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


//public class OldPopupHAndler
//{


//    /// <summary>
//    /// Serve per attivare un pop up dato il nome e nel caso delle identifidicazioni specifiche
//    /// </summary>
//    /// <param name="Name">nome del popup e relative informazioni divise da una virgola</param>
//    public void CallPopUPByName(string Caller, string Name, bool MainThread)
//    {
//        //nel caso in cui non venga eseguito dal main thread verrebbe generata un eccezzione dati i ristretti comandi di cui dispone, perciò la manda a una lista gestita dal mainthread
//        if (!MainThread)
//        {
//            PopUpRequest.Add(Caller + ";" + Name);
//            return;
//        }


//        //Instazio un vettore di 5 elementi che potrei utilizzare in seguito (è necessario inizializzarla al fine i integrità del codice)
//        string[] SubStrings = new string[5];

//        //Controllo se all'interno della stringa sono presenti delle virgole, se si li splitto e li assegno al vettore, mettendo il primo in Name, essendo il nome del popUP
//        if (Name.IndexOf(',') >= 0)
//        {
//            SubStrings = Name.Split(',');
//        }

//        // Guardo se esiste il popUP richiesto, se esiste lo attivo

//        GameObject PopUP = MenuElements.Find(T => T.name.Equals(SubStrings[0]));
//        if (PopUP == null && SubStrings[0].Equals("All"))
//            lock (MenuElements)
//            {
//                foreach (GameObject T in MenuElements)
//                    if (!T.name.Equals("Sfondo"))
//                        T.SetActive(false);
//            }
//        else
//        {
//            settings.Error_Profiler("G003", 0, Caller + ": PopUp non trovato: (CallPopUpByName) => Name: " + SubStrings[0] + " Ricevuto: " + Name, 4, true);
//            return;
//        }
//        PopUP.SetActive(true);

//        //Eseguo uno switch sul PopUp appena avviato per capire se ha delle funzioni da eseguire durante l'avvio
//        switch (PopUP.GetComponent<GenericPopUp>().PopUpID)     //see also in GenericPopUP
//        {
//            case "PlayDirectHost":
//                PopUP.GetComponent<GenericPopUp>().DirectConnectOrHostGame = Convert.ToByte(SubStrings[1]);// Int32.Parse(SubStrings[1]);
//                break;
//        }
//    }

//    public void KillPopUp(string Name)
//    {
//        try
//        {
//            GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text = "";
//            MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(false);
//        }
//        catch (Exception e)
//        {
//            settings.Error_Profiler("M004", 0, "Pop up " + Name + " non found: " + e, 2, true);
//        }

//    }
//}