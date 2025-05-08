using System.Collections.Generic;
using UnityEngine;

public class ExpandableGrid<T> {
    public List<List<T>> grid;

    public ExpandableGrid(T initalValue) {
        grid = new List<List<T>>();

        grid.Add(new List<T>());
        grid[0].Add(initalValue);

    }

    /***********************************************************************************
        COLUMNS
    ***********************************************************************************/

    public void ExpandColumn(T genericFill) {
        foreach(List<T> row in grid) {
            row.Add(genericFill);
        }
    }

    public void InsertColumn(int insertColumn, T genericFill) {
        if(insertColumn == grid[0].Count) {
            ExpandRow(genericFill);
            return;
        }
        if(insertColumn < 0 || insertColumn > grid[0].Count) {
            Debug.LogError("Error[ExpandableGrid]: insertColumn is an invalid value");
            return;
        }

        foreach(List<T> row in grid) {
            row.Insert(insertColumn, genericFill);
        }
    }

    /***********************************************************************************
        ROWS
    ***********************************************************************************/
    public void ExpandRow(T genericFill) {
        List<T> newRow = new List<T>();
        for(int i = 0; i < grid[0].Count; i++) {
            newRow.Add(genericFill);
        }
        grid.Add(newRow);
    }

    public void InsertRow(int insertRow, T genericFill) {
        if(insertRow == grid.Count) {
            ExpandRow(genericFill);
            return;
        }
        if(insertRow < 0 || insertRow > grid.Count) {
            Debug.LogError("Error[ExpandableGrid]: insertRow is an invalid value");
            return;
        }

        List<T> newRow = new List<T>();
        for(int i = 0; i < grid[0].Count; i++) {
            newRow.Add(genericFill);
        }
        grid.Insert(insertRow, newRow);
    }


    /***********************************************************************************
        OTHER
    ***********************************************************************************/
    public Vector2Int Find(T itemToFind) {
        for(int row = 0; row < grid.Count; row++) {
            for(int col = 0; col < grid[row].Count; col++) {
                if(itemToFind.Equals(grid[row][col])) {
                    return new Vector2Int(row, col);
                }
            }
        }

        return new Vector2Int(-1 , -1);
    }








    public void PrintGrid() {
        string output = "";

        for(int row = 0; row < grid.Count; row++) {
            for(int col = 0; col < grid[row].Count; col++) {
                output += grid[row][col].ToString() + " ";
            }
            output += "\n";
        }

        Debug.Log(output);
    }
}
