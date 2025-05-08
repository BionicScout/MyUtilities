using System.Collections.Generic;
using UnityEngine;

public static class CSVReader {
    public static string[][] ReadFile(TextAsset file) {
        //We load the text asset into a string
        string fileContents = file.text;

        //We split the string into the lines that make up the file
        string[] lines = fileContents.Split("\n");

        //We split the first line into its cells
        //We'll use this variable to repeat the process with the other lines
        string[][] cells = new string[lines.Length][];
        for(int i = 0; i < lines.Length; i++) {
            string line = lines[i];
            cells[i] = line.Split(",");
        }
        
        return cells;
    }

    public static void DebugCSV(string[][] cells) {
        string str = "";

        for(int i = 0; i < cells.Length; i++) {
            for(int j = 0;  j < cells[i].Length; j++) {
                str += cells[i][j] + ", ";
            }
            str += "\n";
        }

        Debug.Log(str);
    }
}

public static class CSVReader_Language {
    public static void ReadLanguage(
        TextAsset file, 
        out Dictionary<string, List<string>> languageDict, 
        out Dictionary<string, int> languageIndexes
    ) {

        //The dictionary where we'll load the dialogue
        languageDict = new Dictionary<string, List<string>>();

        //The index corresponding to each of the language initials
        languageIndexes = new Dictionary<string, int>();


        //We load the text asset into a string
        string fileContents = file.text;

        //We split the string into the lines that make up the file
        string[] lines = fileContents.Replace("\r\n", "\n").Replace("\r", "\n").Split("\n");

        //We split the first line into its cells
        //We'll use this variable to repeat the process with the other lines
        string[] cells = lines[0].Replace("\t", ",").Split(",");

        //for(int i = 0; i < lines[0].Length; i++) {
        //    Debug.Log($"char[{i}] = '{lines[0][i]}' (ASCII: {(int)lines[0][i]})");
        //}

        //Debug.Log("Lines Length: " + lines.Length);

        //for(int i = 0; i < cells.Length; i++) {
        //    Debug.Log(cells[i]);
        //}

        //Debug.Log("Cell Length: " + cells.Length);
        

        //We load each name into the languageIndexDictionary
        for(int n = 1; n < cells.Length; n++) {
            //MAKE SURE TO DO N-1, OTHERWISE IT'LL BE SHIFTED ONE TO THE RIGHT
            languageIndexes.Add(cells[n], n - 1);
        }

        //We are going line by line, skipping the first line
        for(int l = 1; l < lines.Length; l++) {
            //We split the line into the cells that make it
            cells = lines[l].Replace("\t", ",").Split(",");

            //We make a list to store it
            List<string> languages = new List<string>();

            //For each cell in the line, skippin the first one (the ID)
            for(int c = 1; c < cells.Length; c++) {
                //We add it ot the list
                languages.Add(cells[c]);
            }

            //We register the languages in the dictionary
            languageDict.Add(cells[0], languages);
        }
    }
}
