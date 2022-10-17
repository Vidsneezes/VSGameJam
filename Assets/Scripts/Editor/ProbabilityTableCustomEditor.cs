using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(ProbabilityTable))]
public class ProbabilityTableCustomEditor : Editor
{

    ScrollView scroller;
    Label tableIsDirty;
    public string folderPath;
    public ProbabilityTable md;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement mainPanel = new VisualElement();
        mainPanel.style.flexGrow = 1;
        mainPanel.style.flexDirection = FlexDirection.Column;

        IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { OnInspectorGUI(); });
        
        md = target as ProbabilityTable;

        tableIsDirty = new Label("");

        scroller = new ScrollView(ScrollViewMode.Vertical);
        RefreshScroller();

        mainPanel.Add(tableIsDirty);
        mainPanel.Add(new Label("Table Percentages"));
       
        mainPanel.Add(scroller);
        mainPanel.Add(new Label("Edit Table"));

        Button addRow = new Button(() => {
            SetupBag();
            AddProbabilityObject();
            RefreshScroller();
        });
        addRow.text = "Add Row";
        Button refreshButton = new Button(() => {
            RefreshScroller();
            tableIsDirty.text = "";
        });
        refreshButton.text = "Refresh";

        Button clearButton = new Button(() => {
            
            if(md.probabilityBag != null)
            {
                for (int i = 0; i < md.probabilityBag.probabilityObjects.Count; i++)
                {
                    AssetDatabase.RemoveObjectFromAsset(md.probabilityBag.probabilityObjects[i]);
                    md.probabilityBag.probabilityObjects[i] = null;
                }

                AssetDatabase.RemoveObjectFromAsset(md.probabilityBag);
                md.probabilityBag = null;
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                EditorUtility.SetDirty(md);

                SetupBag();
            }

            RefreshScroller();

            tableIsDirty.text = "";
        });
        clearButton.text = "Clear";


        mainPanel.Add(refreshButton);
        mainPanel.Add(addRow);
        mainPanel.Add(clearButton);


        TextField folderPathField = new TextField("Fetch From Folder");
        folderPathField.bindingPath = "folderPath";
        folderPathField.RegisterValueChangedCallback<string>(callback => { folderPath = callback.newValue; });
        mainPanel.Add(folderPathField);
        

        Button fetchButton = new Button(() => {
            FetchFromFolder();
            RefreshScroller();
            tableIsDirty.text = "";
        });
        fetchButton.text = "Fetch";
        mainPanel.Add(fetchButton);

        return mainPanel;
    }

    public void SetupBag()
    {
        if (md.probabilityBag == null)
        {
            md.probabilityBag = ScriptableObject.CreateInstance<ProbabilityBag>();
            md.probabilityBag.probabilityObjects = new List<ProbabilityObject>();
            md.probabilityBag.name = "ProbabilityBag";
            AssetDatabase.AddObjectToAsset(md.probabilityBag, md);
            EditorUtility.SetDirty(md);
            EditorUtility.SetDirty(md.probabilityBag);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

    }

    public void AddProbabilityObject(ScriptableObject so = null)
    {
        ProbabilityObject m = ScriptableObject.CreateInstance<ProbabilityObject>();
        m.selectionObject = null;
        m.probability = 0;
        m.name = "Probability Element";
        if (so != null)
        {
            m.name = "Probability : " + so.name;
            m.selectionObject = so;
        }

        md.probabilityBag.probabilityObjects.Add(m);
        AssetDatabase.AddObjectToAsset(m, md);
        EditorUtility.SetDirty(md);
        EditorUtility.SetDirty(m);
        EditorUtility.SetDirty(md.probabilityBag);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public void FetchFromFolder()
    {
        SetupBag();

        string[] paths = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { folderPath });
        for (int i = 0; i < paths.Length; i++)
        {
            Debug.Log(AssetDatabase.GUIDToAssetPath(paths[i]));
            ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(paths[i]));
            if (so != null)
            {
                bool existsInTable = false;
                for (int j = 0; j < md.probabilityBag.probabilityObjects.Count; j++)
                {
                    if (md.probabilityBag.probabilityObjects[j].selectionObject == so)
                    {
                        existsInTable = true;
                        break;
                    }
                }

                if (!existsInTable)
                {
                    Debug.Log(so.name);
                    AddProbabilityObject(so);
                }
            }
        }


    }

    public void RefreshScroller()
    {
        Undo.RecordObject(md, "Save Changes");
        EditorUtility.SetDirty(md);
        AssetDatabase.SaveAssetIfDirty(md);

        if(md.probabilityBag == null)
        {
            return;
        }

        scroller.Clear();
        scroller.style.maxHeight = 300;
        scroller.style.flexDirection = FlexDirection.Column;
        List<float> percentages = md.probabilityBag.GetPercentages();
        for (int i = 0; i < percentages.Count; i++)
        {
            EditorUtility.SetDirty(md.probabilityBag.probabilityObjects[i]);
            SerializedObject so = new SerializedObject(md.probabilityBag.probabilityObjects[i]);

            VisualElement row = new VisualElement();
            row.style.maxHeight = 50;
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingBottom = 5;
            row.style.paddingLeft = 5;
            row.style.paddingTop = 5;
            row.style.paddingRight = 5;

            Label nameLabel = new Label("MISSING SO");
            if(md.probabilityBag.probabilityObjects[i].selectionObject != null)
            {
                nameLabel.text = $"{md.probabilityBag.probabilityObjects[i].selectionObject.name}";
            }
           
            nameLabel.style.marginLeft = 4;
            nameLabel.style.marginRight = 15;
            nameLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            nameLabel.style.width = 160;
            row.Add(nameLabel);

            Label percentageLabel = new Label($"{Mathf.Floor(percentages[i] * 100f)}");
            percentageLabel.style.width = 40;
            percentageLabel.style.marginLeft = 15;
            percentageLabel.style.marginRight = 15;
            percentageLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            row.Add(percentageLabel);

            FloatField probabilityLabel = new FloatField();
            probabilityLabel.value = md.probabilityBag.probabilityObjects[i].probability;
            probabilityLabel.style.width = 40;
            probabilityLabel.style.marginLeft = 15;
            probabilityLabel.style.marginRight = 15;
            probabilityLabel.style.unityTextAlign = TextAnchor.MiddleLeft;

            SerializedProperty property = so.FindProperty("probability");
            probabilityLabel.BindProperty(property);
            probabilityLabel.TrackPropertyValue(property, (Prop) => { if (tableIsDirty != null) { tableIsDirty.text = "<b><color=red>TABLE DIRTY</color></b>"; } });
            row.Add(probabilityLabel);

            ObjectField objectField = new ObjectField();
            objectField.objectType = typeof(ScriptableObject);
            objectField.style.width = 160;
            objectField.style.marginLeft = 15;
            objectField.style.marginRight = 15;
            objectField.style.unityTextAlign = TextAnchor.MiddleLeft;

            SerializedProperty selectionProp = so.FindProperty("selectionObject");
            objectField.BindProperty(selectionProp);
            objectField.TrackPropertyValue(selectionProp, (Prop) => { RefreshScroller(); });
            row.Add(objectField);

            RemovalButton deleteButton = new RemovalButton();
            deleteButton.removeIndex = i;
            deleteButton.pt = md;
            deleteButton.text = "-";
            deleteButton.clicked += RefreshScroller;
            row.Add(deleteButton);

            scroller.Add(row);
        }

        AssetDatabase.SaveAssets();

    }

    public VisualElement DrawDefault()
    {
        var container = new VisualElement();

        var iterator = serializedObject.GetIterator();
        if (iterator.NextVisible(true))
        {
            do
            {
                var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                    propertyField.SetEnabled(value: false);

                container.Add(propertyField);
            }
            while (iterator.NextVisible(false));
        }

        return container;
    }
}

public class RemovalButton : Button
{
    public int removeIndex;
    public ProbabilityTable pt;

    public RemovalButton() : base() 
    {
        clicked += Remove;
    }

    public void Remove()
    {
        pt.probabilityBag.probabilityObjects.RemoveAt(removeIndex);
    }
}

