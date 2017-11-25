using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using Robot.Singleton;


namespace Robot.GUI
{
    internal sealed class Customizing : MonoBehaviour
    {        
        internal static readonly string[] nameOfParts = { "Head", "Body", "Arm_Left", "Arm_Right", "Leg_Left", "Leg_Right" };
        readonly string[] customizingPanel = { "Parts", "Color", "Scale", "Sticker" };        

        struct ScaleInfo {
            public int indexOfActive;
            public float angle;

            public ScaleInfo(int indexOfActive, float angle)
            {
                this.indexOfActive = indexOfActive;
                this.angle = angle;
            }
        }
        
        Dictionary<string, string> pairOfPartAndColor = new Dictionary<string, string>();
        Dictionary<string, ScaleInfo> pairOfPartAndScale = new Dictionary<string, ScaleInfo>();

        internal string nameOfSelected;
        Transform highlightedObject;

        GameObject parts;
        GameObject robot;
        GameObject selectedRobot;
        GameObject table;
        
        GameObject[] activePart;      

        //수정하기
        void Update()
        {
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hitObj;
                if (Physics.Raycast(ray, out hitObj, Mathf.Infinity))
                {
                    if (hitObj.transform.gameObject.layer != LayerMask.NameToLayer("Default")) return;

                    highlightedObject = hitObj.transform;
                    
                    HighlightAgent.Instance.constantOffImmediateAll();                    
                    if (GameObjectAgent.Instance.findChild("UI", "2_Customizing/Frame/").gameObject.activeSelf &&
                        !GameObjectAgent.Instance.findChild("UI", "2_Customizing/Frame/ScrollPanel/Sticker").gameObject.activeSelf)
                        HighlightAgent.Instance.constantOnImmediate(highlightedObject.parent.name, Color.cyan);

                    if (hitObj.transform.name.Contains("Sticker"))
                    {
                        GameObjectAgent.Instance.setActive("UI", "2_Customizing/Cancel", true);
                        GameObjectAgent.Instance.findChild("UI", "2_Customizing/Cancel").transform.position = hitObj.transform.position + new Vector3(0.1f, 0.1f, 0);
                        return;
                    }

                    matchPanelToObject(highlightedObject);

                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Head"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Body"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Left"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Right"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Left"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Right"], false);

                    var name = highlightedObject.parent.name;
                    if (name.Contains("head")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Head"], true);                    
                    else if (name.Contains("body")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Body"], true);                    
                    else if (name.Contains("arm_left")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Left"], true);                    
                    else if (name.Contains("arm_right")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Right"], true);                    
                    else if (name.Contains("leg_left")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Left"], true);                    
                    else if (name.Contains("leg_right")) GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Right"], true);
                }
            }            
        }    
                
        void Start()
        {
            pairOfPartAndColor = new Dictionary<string, string>();
            pairOfPartAndScale = new Dictionary<string, ScaleInfo>();

            #region find object            

            selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
            parts = GameObjectAgent.Instance.findChild("Object", "Parts");            
            robot = GameObjectAgent.Instance.findChild("Object", "Robot");
            table = GameObjectAgent.Instance.findChild("Object", "Table");            
            var scriptOfBot = parts.GetComponent<PartsManager>();
            activePart = scriptOfBot.ActiveParts;

            for (int i = 0; i < parts.transform.childCount; i++) {
                var parentOfPart = parts.transform.GetChild(i);
                for(int j = 0; j < parentOfPart.childCount; j++)
                {
                    var part = parentOfPart.GetChild(j);

                    pairOfPartAndColor.Add(part.name, "color_base");
                    pairOfPartAndScale.Add(part.name, new ScaleInfo(2, 0));
                }                
            }     
            
            #endregion find object            

            var scrollableOfSticker = GameObjectAgent.Instance.addComponent<GUIScrollable>("UI", "2_Customizing/Frame/ScrollPanel/Sticker");
            var scrollableOfColor = GameObjectAgent.Instance.addComponent<GUIScrollable>("UI", "2_Customizing/Frame/ScrollPanel/Color");

            #region add event           
            {
                //Parts                
                for (int i = 0; i < nameOfParts.Length; i++)
                {
                    var scrollable = GameObjectAgent.Instance.addComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/" + nameOfParts[i]]);
                    scrollable.init(nameOfParts[i].ToLower());
                    scrollable.action = () =>
                    {
                        if (!highlightedObject) return;
                        if (highlightedObject.name.Contains("Sticker")) return;
                        if (highlightedObject.parent.parent.name.CompareTo(scrollable.MinButtonName) == 0) return;

                        var newPart = scriptOfBot.activate(scrollable.MinButtonName);

                        if (newPart == null) return;

                        scriptOfBot.matchJoint();
                        HighlightAgent.Instance.constantOnImmediate(newPart.name, Color.cyan);
                        highlightedObject = newPart.transform.GetChild(1);
                    };
                }

                //Color
                scrollableOfColor.init("color", "bt");
                scrollableOfColor.action = () =>
                {
                    if (!highlightedObject || highlightedObject.name.Contains("Sticker") ||
                        highlightedObject.parent.parent.name.CompareTo(scrollableOfColor.MinButtonName) == 0) return;
                    
                    var renderer = highlightedObject.GetComponent<SkinnedMeshRenderer>();

                    Color color = stringToColor(scrollableOfColor.MinButtonName);                    
                    renderer.material.SetColor("_Color", color);
                    pairOfPartAndColor[highlightedObject.parent.name] = scrollableOfColor.MinButtonName;                    
                };

                // Scale
                EventTriggerAgent.Instance.addEvent("UI", "2_Customizing/Frame/ScrollPanel/Scale/Center/Toggle", EventTriggerType.Drag, () =>
                {
                    if (!highlightedObject) return;

                    float x;
                    float y;

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float ang = 0;
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        x = transform.InverseTransformPoint(hit.point).x;                        

                        if (x < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Left"].transform.localPosition.x)
                            x = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Left"].transform.localPosition.x;
                        else if (x > GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Right"].transform.localPosition.x)
                            x = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Right"].transform.localPosition.x;

                        y = Mathf.Sqrt(332 * 332 - x * x);

                        ang = Mathf.Atan2(y, x) / Mathf.PI * 180 - 90;
                        GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation = Quaternion.Euler(0, 0, ang);
                        highlightedObject.parent.localScale = new Vector3(13 - ang / 90, 13 - ang / 90, 13 - ang / 90);

                        scriptOfBot.matchJoint();
                    }

                    float minangle = 360;
                    int closestButtonindex = 2;

                    for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.childCount; i++)
                    {
                        var child = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.GetChild(i);
                        float angle = Mathf.Abs(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation.z - child.rotation.z);

                        if (minangle > angle)
                        {
                            minangle = angle;
                            closestButtonindex = i;
                        }
                    }

                    for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.childCount; ++i)
                    {
                        if (closestButtonindex != i)
                            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_" + (i+1), ResourcesManager.Instance.sprites["number_" + (i + 1) + "_normal"]);
                        else
                            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_" + (i+1), ResourcesManager.Instance.sprites["number_" + (i + 1) + "_normal"]);
                    }

                    var scaleInfo = new ScaleInfo(closestButtonindex, ang);
                    pairOfPartAndScale[highlightedObject.parent.name] = scaleInfo;
                });

                // Sticker
                scrollableOfSticker.init("StickerAtlas");
                var stickerCamera = GameObjectAgent.Instance.findChild("Camera", "Sticker Camera").GetComponent<Camera>();
                var uiCamera = GameObjectAgent.Instance.findChild("Camera", "UI Camera").GetComponent<Camera>();
                for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Sticker/Panel"].transform.childCount; i++)
                {
                    var button = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Sticker/Panel"].transform.GetChild(i);
                    var image = button.FindChild("Image").gameObject;
                    var stickerParent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + button.name).transform;
                    var dummySticker = GameObjectAgent.Instance.findChild("UI", "2_Customizing/Frame/Sticker/" + button.name).transform;

                    EventTriggerAgent.Instance.addEvent(image, EventTriggerType.Drag, () =>
                    {
                        if (image.GetComponent<UnityEngine.UI.Image>().sprite.name.CompareTo(scrollableOfSticker.MinButtonName) != 0) return;
                        //if (!stickerParent || stickerParent.childCount == 0) return;

                        RaycastHit rh;
                        Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);

                        //Ray ray = stickerCamera.ScreenPointToRay(Input.mousePosition);
                        //RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, Mathf.Infinity))
                            dummySticker.transform.localPosition = transform.InverseTransformPoint(rh.point) + new Vector3(0, 600, 0);

                        //sticker.transform.position = new Vector3(rh.point.x, rh.point.y, 0);
                    });

                    EventTriggerAgent.Instance.addEvent(image, EventTriggerType.EndDrag, () =>
                    {
                        if (image.GetComponent<UnityEngine.UI.Image>().sprite.name.CompareTo(scrollableOfSticker.MinButtonName) != 0) return;

                        stickerParent.GetChild(0).transform.position = new Vector3(1000, 1000, 1000);

                        if (!stickerParent || stickerParent.childCount == 1) return;

                        var sticker = stickerParent.GetChild(1);

                        Ray ray = stickerCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit rh;

                        if (Physics.Raycast(ray, out rh, Mathf.Infinity) &&
                            rh.transform.gameObject.layer == LayerMask.NameToLayer("Default") &&
                            !rh.transform.name.Contains("Sticker"))
                        {

                            var decal = sticker.GetComponent<DecalSystem.Decal>();
                            if (decal)
                            {
                                sticker.transform.position = rh.point;
                                sticker.transform.SetParent(rh.transform);
                                DecalSystem.DecalBuilder.BuildDecal(decal);

                                if (rh.transform.parent.name.Contains("head"))
                                    sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.HEAD]));
                                else if (rh.transform.parent.name.Contains("arm_left"))
                                {
                                    if (sticker.position.y > 2.3f)
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.UPPPERARM_LEFT]));
                                    else
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.ARM_LEFT]));
                                }
                                else if (rh.transform.parent.name.Contains("arm_right"))
                                {
                                    if (sticker.position.y > 2.3f)
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.UPPERARM_RIGHT]));
                                    else
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.ARM_RIGHT]));
                                }
                                else if (rh.transform.parent.name.Contains("leg_left"))
                                    sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.LEG_LEFT]));
                                else if (rh.transform.parent.name.Contains("leg_right"))
                                    sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.LEG_RIGHT]));
                                else if (rh.transform.parent.name.Contains("body"))
                                {
                                    if (sticker.position.y > 1f)
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.UPPER_BODY]));
                                    else
                                        sticker.transform.SetParent(rh.transform.parent.FindChild(Constants.stickerParent[(int)Constants.PART.BODY]));
                                }
                            }
                        }

                        dummySticker.transform.localPosition = new Vector3(0, 3000, 0);
                    });
                }

                GUIAgent.Instance.addListener("UI", "2_Customizing/Title/Next", () =>
                {
                    if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"].gameObject.activeSelf) return;

                    for (int i = 0; i < activePart.Length; i++)
                    {
                        GameObjectAgent.Instance.setActive(activePart[i], false);
                        GameObjectAgent.Instance.setActive(activePart[i], true);
                    }

                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"], true);
                });

                GUIAgent.Instance.addListener("UI", "2_Customizing/Title/Prev", () =>
                {
                    if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"].gameObject.activeSelf) return;

                    if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"].gameObject.activeSelf == true)
                    {
                        GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame"], true);
                        GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"], false);
                        return;
                    }
                    else
                    {
                        //GameManager.Instance.transition();
                        GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                        GameObjectAgent.Instance.delaySetActive(parts, false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(table, false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing"], false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                    }                    
                });

                for (int i = 0; i < customizingPanel.Length; i++)
                {
                    var nameOfPanel = customizingPanel[i];

                    EventTriggerAgent.Instance.addEvent("UI", "2_Customizing/Frame/Image/" + nameOfPanel
                        , EventTriggerType.PointerClick, () =>
                        {
                            for(int j = 0; j < customizingPanel.Length; j++)
                            {
                                GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/" + customizingPanel[j], ResourcesManager.Instance.sprites["bt_" + customizingPanel[j].ToLower() + "_normal"]);
                                GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/" + customizingPanel[j]].SetActive(false);
                            }

                            GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/" + nameOfPanel].SetActive(true);
                            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/" + nameOfPanel, ResourcesManager.Instance.sprites["bt_" + nameOfPanel.ToLower() + "_select"]);
                            if (nameOfPanel.Contains("Scale"))
                                GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/FrameVignetting"], false);
                            else
                                GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/FrameVignetting"], true);
                        });
                }

                bool click = false;
                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Left"), EventTriggerType.PointerDown, () =>
                {
                    click = true;
                });

                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Left"), EventTriggerType.UpdateSelected, () =>
                {
                    if (!click) return;

                    iTween.RotateBy(parts.gameObject, iTween.Hash("y", -0.004f, "time", 0.005f));
                });

                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Left"), EventTriggerType.PointerUp, () =>
                {
                    iTween.Stop();
                    click = false;
                });

                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Right"), EventTriggerType.PointerDown, () =>
                {
                    click = true;
                });

                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Right"), EventTriggerType.UpdateSelected, () =>
                {
                    if (!click) return;

                    iTween.RotateBy(parts.gameObject, iTween.Hash("y", 0.004f, "time", 0.005f));
                    //iTween.RotateAdd(parts.gameObject, Vector3.down, 0.002f);
                });

                EventTriggerAgent.Instance.addEvent(GameObjectAgent.Instance.findChild("UI", "2_Customizing/Button/Right"), EventTriggerType.PointerUp, () =>
                {
                    iTween.Stop();
                    click = false;
                });

                GUIAgent.Instance.addListener("UI", "2_Customizing/Button/Storage", () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/6_Storage/Title/Prev"]);
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/6_Storage/Title/Next"]);

                    //GameManager.Instance.transition();
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    if (selectedRobot.transform.childCount > 0) //선택 -> 창고
                    {
                        nameOfSelected = selectedRobot.transform.GetChild(0).name;
                        selectedRobot.transform.GetChild(0).SetParent(robot.transform);
                    }
                    
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(parts, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(robot, true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/6_Storage"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive("Background", "6_Storage", true, GameManager.Close + GameManager.Delay);

                    Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                    {
                        //되돌리기
                        for (int i = 0; i < parts.transform.childCount; i++)
                        {
                            var part = parts.transform.GetChild(i).GetChild(0);
                            part.localScale = new Vector3(13f, 13f, 13f);
                            part.GetChild(1).GetComponent<Renderer>().material.color = new Color(0.784f, 0.784f, 0.784f);
                            for (int j = 0; j < Constants.stickerParent.Length; j++)
                            {
                                var stickerParent = part.FindChild(Constants.stickerParent[j]);

                                for (int k = 0; k < stickerParent.childCount; k++)
                                {
                                    var sticker = stickerParent.GetChild(k);

                                    if (!sticker.name.Contains("Sticker")) continue;

                                    var newParent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + sticker.name);
                                    sticker.SetParent(newParent.transform);
                                    sticker.localPosition = new Vector3(2000, 2000, 0);
                                }
                            }
                        }

                        robot.transform.position = new Vector3(-1.3f, 0, 0);
                        table.transform.position = new Vector3(-1.1f, -2, 0);
                        robot.transform.rotation = Quaternion.Euler(0, 330, 0);
                    });

                    var scrollableOfStorage = GameObjectAgent.Instance.addComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/6_Storage/Frame/ScrollPanel/Bot"]);
                    scrollableOfStorage.init("save_");
                    scrollableOfStorage.action = () =>
                    {                       
                        for (int i = 0; i < robot.transform.childCount; i++)
                        {
                            if (selectedRobot.transform.childCount > 0) selectedRobot.transform.GetChild(0).SetParent(robot.transform);

                            var child = robot.transform.GetChild(i).gameObject;
                            if (child.name.CompareTo(scrollableOfStorage.MinButtonName) == 0)
                            {
                                child.SetActive(true);
                                child.transform.localPosition = Vector3.zero;
                                child.transform.localRotation = Quaternion.identity;
                                string temp = child.name.Split('_')[1];
                                GUIAgent.Instance.setText("UI", "6_Storage/Panel/Name", temp);
                                for (int j = 0; j < child.transform.childCount; j++)
                                    child.transform.GetChild(j).gameObject.SetActive(true);
                            }
                            else
                            {
                                child.SetActive(false);                                
                            }
                        }
                    };


                    for (int i = 0; i < activePart.Length; i++)
                    {
                        if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"].gameObject.activeSelf)                        
                            AnimationAgent.Instance.setInteger(activePart[i].name, "State", 9);
                    }
                });

                var inputField = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"].transform.FindChild("InputField").FindChild("Text").GetComponent<UnityEngine.UI.Text>();
                GUIAgent.Instance.addListener("UI", "2_Customizing/Naming/Complete", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Left"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Right"], false);
                    parts.transform.rotation = Quaternion.identity;
                    robot.transform.rotation = Quaternion.identity;

                    GUIAgent.Instance.setText("UI", "2_Customizing/Complete/Panel/Text/Name", inputField.text);

                    string botName = "save_" + inputField.text;
                    nameOfSelected = botName;                    

                    // save
                    var obj = parts.GetComponent<PartsManager>().BotInfo;
                    var data = Converter.Serialize(obj);
                    PlayerPrefs.SetString(botName, data);
                    string saved = PlayerPrefs.GetString("name", "") + botName + "/";
                    PlayerPrefs.SetString("name", saved);
                    PlayerPrefs.Save();

                    capture(botName);
                    robot.gameObject.SetActive(true);
                    selectedRobot.gameObject.SetActive(true);
                    if(selectedRobot.transform.childCount > 0)                    
                        selectedRobot.transform.GetChild(0).SetParent(robot.transform);
                    
                    var newRobot = new GameObject(botName);                   

                    newRobot.transform.SetParent(selectedRobot.transform);

                    parts.gameObject.SetActive(false);
                    robot.gameObject.SetActive(false);

                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"], true);

                    for (int i = 0; i < activePart.Length; i++)
                    {
                        activePart[i].transform.SetParent(newRobot.transform);
                        AnimationAgent.Instance.setInteger(activePart[i].name, "State", 0);
                    }
                });

                GUIAgent.Instance.addListener("UI", "2_Customizing/Complete/Frame/Back", () =>
                {
                    //GameManager.Instance.transition();
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    for (int i = 0; i < activePart.Length; i++)
                    {
                        if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"].gameObject.activeSelf)
                        {
                            AnimationAgent.Instance.setInteger(activePart[i].name, "State", 9);
                        }
                        else activePart[i].SetActive(false);
                        activePart[i] = null;
                    }

                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(parts, false, GameManager.Close + GameManager.Delay);                    
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(table, false, GameManager.Close + GameManager.Delay);
                 
                });

                GUIAgent.Instance.addListener("UI", "2_Customizing/Cancel", () =>
                {
                    if (!highlightedObject) return;

                    var parent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + highlightedObject.name);

                    highlightedObject.SetParent(parent.transform);
                    highlightedObject.localPosition = new Vector3(1000, 1000, 1000);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Cancel"], false);
                });

                //GUIAgent.Instance.addListener("UI", "1_Lobby/Button", () =>
                //{
                //    if (selectedRobot.transform.childCount > 0)
                //        selectedRobot.transform.GetChild(0).SetParent(robot.transform);

                //    string saved = PlayerPrefs.GetString("name", "");
                //    for (int i = 0; i < robot.transform.childCount; i++)
                //    {
                //        if (robot.transform.GetChild(i).name.Contains("save_") && !saved.Contains(robot.transform.GetChild(i).name))
                //            saved += robot.transform.GetChild(i).name + "/";
                //    }

                //    if (selectedRobot.transform.childCount > 0)  saved += selectedRobot.transform.GetChild(0) + "/";

                //    PlayerPrefs.SetString("name", saved);
                //    //PlayerPrefs.SetString("nameOfSprite", nameOfSprite);
                    

                //    Application.Quit();
                //});

            }
            #endregion add event

            #region
            
            GameObjectAgent.Instance.setActive(gameObject, false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing"], false);

            #endregion
        }

        internal void init()
        {
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Left"], true);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Right"], true);
            GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Title/Prev"]);
            GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Title/Next"]);
            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/Parts", ResourcesManager.Instance.sprites["bt_parts_select"]);
            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/Color", ResourcesManager.Instance.sprites["bt_color_normal"]);
            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/Scale", ResourcesManager.Instance.sprites["bt_scale_normal"]);
            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/Image/Sticker", ResourcesManager.Instance.sprites["bt_sticker_normal"]);
            GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_3", ResourcesManager.Instance.sprites["number_" + 3 + "_select"]);

            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Head"], true);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Body"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Left"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Arm_Right"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Left"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/Leg_Right"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts"], true);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Color"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Sticker"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame"], true);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Naming"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Complete"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Cancel"], false);            
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing/Clear"], false);

            GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/2_Customizing"], true, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(robot, false, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(selectedRobot, false, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(gameObject, true, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(parts, true, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(table, true, GameManager.Close + GameManager.Delay);
            table.transform.position = new Vector3(0, -2f, 0);
            //iTween.MoveTo(table, iTween.Hash("x", 0f, "y", -2f, "delay", GameManager.Close, "time", 0));

            GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation = Quaternion.identity;
            parts.GetComponent<PartsManager>().matchJoint();

            for (int i = 0; i < nameOfParts.Length; i++)
            {
                var scrollable = GameObjectAgent.Instance.addComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Parts/" + nameOfParts[i]]);
                if (activePart[i].transform.parent.name != scrollable.MinButtonName)
                    scrollable.matchButtonToCenter(activePart[i].transform.parent.name);

                pairOfPartAndColor[activePart[i].name] = colorToString(activePart[i].transform.GetChild(1).GetComponent<Renderer>().material.color);
                pairOfPartAndScale[activePart[i].name] = getScaleInfoByScale(activePart[i].transform.localScale.x);

            }
            highlightedObject = activePart[0].transform.GetChild(1);
            HighlightAgent.Instance.constantOnImmediate(highlightedObject.parent.name, Color.cyan);            
            GameObjectAgent.Instance.addComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Color"]).matchButtonToCenter(pairOfPartAndColor[activePart[0].name]);            
        }

        string colorToString(Color color)
        {
            if (color.Equals(new Color(0.784f, 0.784f, 0.784f)))
                return "color_base";
            else if (color.Equals(Color.red))
                return "color_red";
            else if (color.Equals(Color.Lerp(Color.red, Color.white, 0.3f)))
                return "color_orange";
            else if (color.Equals(Color.yellow))
                return "color_yellow";
            else if (color.Equals(Color.green))
                return "color_green";            
            else if (color.Equals(Color.cyan))
                return "color_blue";

            return null;
        }

        Color stringToColor(string colorName)
        {
            if (colorName.CompareTo("color_base") == 0) return new Color(0.784f, 0.784f, 0.784f);
            else if (colorName.CompareTo("color_red") == 0) return Color.red;
            else if (colorName.CompareTo("color_orange") == 0) return Color.Lerp(Color.red, Color.white, 0.3f);
            else if (colorName.CompareTo("color_yellow") == 0) return Color.yellow;
            else if (colorName.CompareTo("color_green") == 0) return Color.green;
            else if (colorName.CompareTo("color_blue") == 0) return Color.cyan;

            return new Color(0.784f, 0.784f, 0.784f);
        }

        ScaleInfo getScaleInfoByScale(float scale)
        {
            float minangle = 360;
            int closestButtonindex = 2;
            for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.childCount; i++)
            {
                var child = GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.GetChild(i);
                float angle = Mathf.Abs(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation.z - child.rotation.z);

                if (minangle > angle)
                {
                    minangle = angle;
                    closestButtonindex = i;
                }
            }

            return new ScaleInfo( closestButtonindex, (13 - scale) * 90);
        }

        void matchPanelToObject(Transform obj)
        {
            var scrollable = GameObjectAgent.Instance.getComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Color"]);

            if (pairOfPartAndColor[obj.parent.name] != scrollable.MinButtonName)
                scrollable.matchButtonToCenter(pairOfPartAndColor[obj.parent.name]);

            if (GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation.z != pairOfPartAndScale[obj.parent.name].angle)
            {
                var scaleInfo = pairOfPartAndScale[obj.parent.name];
                GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Center"].transform.rotation = Quaternion.Euler(0, 0, scaleInfo.angle);               

                for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing / Frame / ScrollPanel / Scale / Number"].transform.childCount; ++i)
                    GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_" + (i + 1), ResourcesManager.Instance.sprites["number_" + (i + 1) + "_normal"]);
                
                GUIAgent.Instance.setSprite("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_" + scaleInfo.indexOfActive, ResourcesManager.Instance.sprites["number_" + (scaleInfo.indexOfActive + 1) + "_select"]);                
            }
        }
                       
        void capture(string name)
        {
            var camera = GameObjectAgent.Instance.findChild("Camera", "SnapShot Camera").GetComponent<Camera>();

            RenderTexture rt = new RenderTexture(460, 460, 24);
            Texture2D texture = new Texture2D(460, 460, TextureFormat.ARGB32, false);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            texture.ReadPixels(new Rect(0, 0, 460, 460), 0, 0, false);
            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            string temp = System.Convert.ToBase64String(bytes);
            string nameOfSprite = PlayerPrefs.GetString("nameOfSprite", "");
            PlayerPrefs.SetString(name + "+texture", temp);
            nameOfSprite += name + "+texture/";
            PlayerPrefs.SetString("nameOfSprite", nameOfSprite);
            PlayerPrefs.Save();

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprite.name = name;
            ResourcesManager.Instance.sprites.Add(name, sprite);

            camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors       
            Destroy(rt);
        }
    } // class Customizing

} // namespcae Robot.GUI