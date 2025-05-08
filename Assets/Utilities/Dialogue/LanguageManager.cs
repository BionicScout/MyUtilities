using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageManager : MonoBehaviour {
    string lang = "EN";
    List<LanguageText> textToUpdate = new List<LanguageText>();

    Dictionary<string, List<string>> languageDict;
    Dictionary<string, int> languageIndexes;

    [System.Serializable]
    private struct LanguageText {
        public string id; 
        public TMP_Text text;
    }

    //Temp
    public TextAsset file;

    private void Awake() {
        lang = CurrentLanguage.lang;
        CSVReader_Language.ReadLanguage(file, out languageDict, out languageIndexes);
        UpdateLanguage();
    }

    public void Register(string id, TMP_Text text) {
        textToUpdate.Add(new LanguageText {  id = id, text = text });
    }
    //public void Unregister(string id) {
        
    //}

    public void UI_SelectLang(string lang) {
        CurrentLanguage.lang = lang;
        this.lang = lang;
        UpdateLanguage();
    }

    public void UI_SelectLang(TMP_Text lang) {
        UI_SelectLang(lang.text);
    }

    private void UpdateLanguage() {
        //Debug.Log(languageIndexes.Keys.Count);
        //foreach(var temp in languageIndexes.Keys) {
        //    for(int i = 0; i < temp.Length; i++) {
        //        Debug.Log($"char[{i}] = '{temp[i]}' (ASCII: {(int)lang[i]})");
        //    }
        //    Debug.Log("");
        //}
        int langSel = languageIndexes[lang];

        foreach (var item in textToUpdate) {
            //Debug.Log(item.id);
            if(languageDict.ContainsKey(item.id)) {
                //We find the index related to that language;

                //Return the line corresponding to that key and language
                item.text.text = languageDict[item.id][langSel];
            }
            else {
                item.text.text = "ERROR: TEXT NOT FOUND";
            }
        }
    }

    public string GetLine(string key) {
        //If the languahe dictionary contains the key received
        if(languageDict.ContainsKey(key)) {
            //We find the index related to that language
            int langSel = languageIndexes[lang];

            //Return the line corresponding to that key and language
            return languageDict[key][langSel];
        }

        return "ERROR: Line not found";
    }
}

public static class CurrentLanguage {
    public static string lang = "EN";
}
