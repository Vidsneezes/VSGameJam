using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterDatabaseEditor : EditorWindow
{
    [MenuItem("Database/Monsters")]
    public static void ShowWindow()
    {
        MonsterDatabaseEditor monsterDatabaseEditor =  (MonsterDatabaseEditor)EditorWindow.GetWindow(typeof(MonsterDatabaseEditor));
        monsterDatabaseEditor.Init();
    }


    MonsterData currentMonsterData
    {
        get
        {
            return monstersData[currentMonster];
        }
    }
    public int currentMonster;
    public List<MonsterData> monstersData;

    public void Init()
    {
        monstersData = new List<MonsterData>();
        string[] monsterDataPath = AssetDatabase.FindAssets("t:MonsterData");
        for (int i = 0; i < monsterDataPath.Length; i++)
        {
            monstersData.Add(AssetDatabase.LoadAssetAtPath<MonsterData>(AssetDatabase.GUIDToAssetPath(monsterDataPath[i])));
        }
        if (monstersData.Count > 0)
        {
            currentMonster = 0;
        }
    }

    void OnGUI()
    {
        if(currentMonsterData != null)
        {
            //GUIStyle style = new GUIStyle("AssetLabel");
            //style.fontSize = 4;
            //style.alignment = TextAnchor.MiddleCenter;
            Rect rect = new Rect(0, 0, 150, 25);

            GUI.Box(rect, currentMonsterData.name);

            rect.x = 150;
            rect.width = 40;
            bool previousMonster = GUI.Button(rect,"<");

            rect.x = 190;
            bool nextMonster = GUI.Button(rect,">");

            rect.y = 25;
            rect.x = 0;
            rect.width = 150;
            rect.height = 150;
            GUI.Box(rect,"");
            GUI.DrawTexture(rect,currentMonsterData.displayImage.texture,ScaleMode.StretchToFill,true);


            rect.y += 160;
            rect.width = 150;
            rect.height = 25;
            currentMonsterData.HP = EditorGUI.FloatField(rect, "HP: ", currentMonsterData.HP);

           
            

            if (nextMonster)
            {
                currentMonster += 1;
            }
            else if(previousMonster)
            {
                currentMonster -= 1;
            }

            if(currentMonster < 0)
            {
                currentMonster = 0;
            }else if(currentMonster >= monstersData.Count)
            {
                currentMonster = monstersData.Count - 1;
            }
        }
     
    }

}
