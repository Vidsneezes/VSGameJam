using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { OnInspectorGUI(); });

        MonsterData md = target as MonsterData;
        VisualElement mainPanel = new VisualElement();
        mainPanel.style.flexGrow = 1;
        mainPanel.style.maxWidth = 300;
        mainPanel.style.height = 250;
        mainPanel.style.flexDirection = FlexDirection.Column;

        mainPanel.Bind(new SerializedObject(target));
        Color bkgColor;
        ColorUtility.TryParseHtmlString("#292828", out bkgColor);


        VisualElement imagePanel = new VisualElement();
        imagePanel.style.minWidth = 200;
        imagePanel.style.maxWidth = 200;
        imagePanel.style.minHeight = 200;
        imagePanel.style.maxHeight = 200;
        imagePanel.style.backgroundColor = bkgColor * 1.5f;

        Image MonsterDisplay = new Image();
        MonsterDisplay.image = md.displayImage.texture;
        MonsterDisplay.scaleMode = ScaleMode.StretchToFill;
        MonsterDisplay.style.width = 100;
        MonsterDisplay.style.height = 100;
        MonsterDisplay.style.position = Position.Absolute;
        MonsterDisplay.style.left = 50;
        MonsterDisplay.style.top = 50;

        imagePanel.Add(MonsterDisplay);
        mainPanel.Add(imagePanel);

        if (md.itemLootTable != null)
        {
            ItemLootTable lootTable = md.itemLootTable;

            List<float> percentages = lootTable.GetPercentages();

            for (int i = 0; i < lootTable.itemDrops.Count; i++)
            {
                string dropValue = $"{lootTable.itemDrops[i].item.name} : {percentages[i]}%";

                mainPanel.Add(new Label(dropValue));
            }
        }

        ScrollView scroller = new ScrollView(ScrollViewMode.Vertical);
        scroller.Add(mainPanel);
        scroller.Add(inspectorIMGUI);
        return scroller;
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
