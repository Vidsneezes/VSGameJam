using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class MonsterMainEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/MonsterMainEditor")]
    public static void ShowExample()
    {
        MonsterMainEditor wnd = GetWindow<MonsterMainEditor>();
        wnd.titleContent = new GUIContent("MonsterMainEditor");
    }

    public const string MONSTER_PATH = "Assets/Resources/GameData/Monsters";
    public const string DEFAULT_SPRITE = "Sprites/monster/Mon_monstercard";

    MonsterData currentMonsterData
    {
        get
        {
            return monsters[currentMonster];
        }
    }

    public List<MonsterData> monsters;
    public int currentMonster;
    public VisualElement Main;
    public VisualElement DropsInfoPanel;
    public ScrollView monsterList;

    public Image MonsterDisplay;
    Color bkgColor;


    public void CreateGUI()
    {
        monsters = new List<MonsterData>();
        string[] monsterDataPath = AssetDatabase.FindAssets("t:MonsterData");
        for (int i = 0; i < monsterDataPath.Length; i++)
        {
            monsters.Add(AssetDatabase.LoadAssetAtPath<MonsterData>(AssetDatabase.GUIDToAssetPath(monsterDataPath[i])));
        }
        if (monsters.Count > 0)
        {
            currentMonster = 0;
        }

        if (monsters.Count > 0)
        {
            currentMonster = 0;
        }

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        ColorUtility.TryParseHtmlString("#292828", out bkgColor);

        if (monsters.Count > 0)
        {
            Main = new VisualElement();
            Main.style.flexGrow = 1;
            Main.style.flexDirection = FlexDirection.Row;
            Main.style.alignItems = Align.FlexStart;
            Main.style.justifyContent = Justify.FlexStart;

            VisualElement infoPanel = new VisualElement();
            infoPanel.style.minWidth = 200;
            infoPanel.style.maxWidth = 200;

            infoPanel.style.flexGrow = 1;
            infoPanel.style.flexDirection = FlexDirection.Column;
            infoPanel.style.backgroundColor = bkgColor;

            VisualElement attributeSlot = new VisualElement();
            attributeSlot.style.flexDirection = FlexDirection.Row;

            TextField textName = new TextField();
            textName.style.minWidth = 40;
            textName.RegisterValueChangedCallback(PopulateMonsterListCallback);
            textName.bindingPath = "internalName";
        
            //attributeSlot.Add(new Label("Name: "));
            attributeSlot.Add(textName);
            infoPanel.Add(attributeSlot);

            VisualElement imagePanel = new VisualElement();
            imagePanel.style.minWidth = 200;
            imagePanel.style.maxWidth = 200;
            imagePanel.style.minHeight = 200;
            imagePanel.style.maxHeight = 200;
            imagePanel.style.backgroundColor = bkgColor * 1.5f;

            MonsterDisplay = new Image();
            MonsterDisplay.image = currentMonsterData.displayImage.texture;
            MonsterDisplay.scaleMode = ScaleMode.StretchToFill;
            MonsterDisplay.style.width = 100;
            MonsterDisplay.style.height = 100;
            MonsterDisplay.style.position = Position.Absolute;
            MonsterDisplay.style.left = 50;
            MonsterDisplay.style.top = 50;

            imagePanel.Add(MonsterDisplay);

            infoPanel.Add(imagePanel);

            VisualElement attributesPanel = new VisualElement();
            attributesPanel.style.flexDirection = FlexDirection.Column;
            attributesPanel.style.flexGrow = 1;
            attributesPanel.style.backgroundColor = bkgColor;

            attributesPanel.Add(AddBindedFloatField("HP", "HP"));
            attributesPanel.Add(AddBindedFloatField("Damage", "damage"));
            attributesPanel.Add(AddBindedFloatField("Defense", "defense"));
            attributesPanel.Add(AddBindedFloatField("Speed", "speed"));
            attributesPanel.Add(AddBindedFloatField("Move Time", "moveTime"));
            attributesPanel.Add(AddBindedFloatField("Hold Time", "holdTime"));

            ObjectField objectF = new ObjectField();
            objectF.objectType = typeof(Sprite);
            objectF.bindingPath = "displayImage";
            objectF.RegisterValueChangedCallback(UpdateMonsterDisplay);
            attributesPanel.Add(objectF);

            infoPanel.Add(attributesPanel);

            Main.Add(CreateMonsterList());
            Main.Add(infoPanel);
            root.Add(Main);
            
            SerializedObject so = new SerializedObject(currentMonsterData);
            rootVisualElement.Bind(so);

        }
    }

    public void PopulateMonsterListCallback(ChangeEvent<string> evt)
    {
        Undo.RecordObject(currentMonsterData, "Save Name");
        PopulateMonsterList();
    }

    public void UpdateMonsterDisplay(ChangeEvent<UnityEngine.Object> evt)
    {
        MonsterDisplay.image = currentMonsterData.displayImage.texture;
    }

    public VisualElement AddBindedFloatField(string attribute, string bindingPath)
    {
        VisualElement attributeSlot = new VisualElement();
        attributeSlot.style.width = 190;
        attributeSlot.style.height = 20;

        FloatField floatField = new FloatField(attribute, 999999);
        floatField.bindingPath = bindingPath;
        floatField.style.backgroundColor = bkgColor * 1.3f;
        attributeSlot.Add(floatField);
        return attributeSlot;
    }

    public void UpdateMonsterData(int next)
    {
        PopulateMonsterList();
        rootVisualElement.Unbind();

        currentMonster = Mathf.Clamp(next, 0, monsters.Count-1);

        if(currentMonsterData.displayImage == null)
        {
            currentMonsterData.displayImage = Resources.Load<Sprite>(DEFAULT_SPRITE);
        }

        MonsterDisplay.image = currentMonsterData.displayImage.texture;
        if (DropsInfoPanel != null)
        {
            Main.Remove(DropsInfoPanel);
        }
        DropsInfoPanel = CreateDropsInfo();
        Main.Add(DropsInfoPanel);
        
        SerializedObject so = new SerializedObject(currentMonsterData);
        rootVisualElement.Bind(so);
    }

    public VisualElement CreateMonsterList()
    {
        VisualElement mainPanel = new VisualElement();
        mainPanel.name = "monsterscroll";
        mainPanel.style.maxWidth = 100;
        mainPanel.style.flexGrow = 1;
        mainPanel.style.flexDirection = FlexDirection.Column;

        Button addNew = new Button(CreateNewMonster);
        addNew.text = "New";

        mainPanel.Add(addNew);

        monsterList = new ScrollView(ScrollViewMode.Vertical);

        PopulateMonsterList();

        mainPanel.Add(monsterList);

        return mainPanel;
    }

    public void PopulateMonsterList()
    {
        monsterList.Clear();
        for (int i = 0; i < monsters.Count; i++)
        {
            monsterList.Add(MonsterButton.CreateMonsterButton(monsters[i].name, i, UpdateMonsterData));
        }
    }

    public void CreateNewMonster()
    {
        MonsterData monsterData = ScriptableObject.CreateInstance<MonsterData>();
        monsterData.HP = 20;
        monsterData.speed = 1;
        monsterData.damage = 20;
        monsterData.defense = 20;
        monsterData.holdTime = 1;
        monsterData.moveTime = 3;
        monsterData.displayImage = Resources.Load<Sprite>(DEFAULT_SPRITE);
        AssetDatabase.CreateAsset(monsterData, System.IO.Path.Combine(MONSTER_PATH, "NewMonster.asset"));
        monsters.Add(monsterData);

        PopulateMonsterList();
    }

    public VisualElement CreateDropsInfo()
    {
        VisualElement mainPanel = new VisualElement();
        mainPanel.style.maxWidth = 100;
        mainPanel.style.height = 300;
        mainPanel.style.paddingLeft = 2;
        mainPanel.style.flexDirection = FlexDirection.Column;

        mainPanel.style.paddingTop = 10;

        if (currentMonsterData.itemLootTable != null)
        {
            ItemLootTable lootTable = currentMonsterData.itemLootTable;

            List<float> percentages = lootTable.GetPercentages();

            for (int i = 0; i < lootTable.itemDrops.Count; i++)
            {
                string dropValue = $"{lootTable.itemDrops[i].item.name} : {percentages[i]}%";

                mainPanel.Add(new Label(dropValue));
            }
        }
        return mainPanel;
    }

    public struct MonsterButton
    {
        public int index;
        public Button button;

        public static Button CreateMonsterButton(string text, int _index, Action<int> callback)
        {
            MonsterButton mon;
            mon.index = _index;
            mon.button = new Button(() => { callback(mon.index); });
            mon.button.text = _index +" : "+ text;
            mon.button.style.minHeight = 25;
            mon.button.style.fontSize = 10;
            mon.button.style.unityTextAlign = TextAnchor.MiddleLeft;
            return mon.button;
        }
    }

}