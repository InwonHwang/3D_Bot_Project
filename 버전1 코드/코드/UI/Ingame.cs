using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

using Robot.Singleton;

namespace Robot.GUI
{
    internal class Ingame : MonoBehaviour
    {
        GameObject selected;


        void Start()
        {
            #region find object
            CarBot carBot = gameObject.GetComponent<CarBot>();
            carBot.MovementSpeed = 10f;

            var selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
            var parts = GameObjectAgent.Instance.findChild("Object", "Parts");
            var robot = GameObjectAgent.Instance.findChild("Object", "Robot");
            //var activePart = parts.GetComponent<PartsManager>().ActiveParts;
            var table = GameObjectAgent.Instance.findChild("Object", "Table");
            var level = GameObject.Find("Level");
            var manager = GameObject.Find("Manager");
            var mainCamera = GameObjectAgent.Instance.findChild("Camera", "Main Camera").GetComponent<Camera>();
            var orthographicCamera = GameObjectAgent.Instance.findChild("Camera", "Orthographic Camera").GetComponent<Camera>();
            var light = GameObjectAgent.Instance.findChild("Light", "Base");
            var lightOfIngame = GameObjectAgent.Instance.findChild("Light", "Ingame");


            #endregion find object           

            #region add event
            {
                EventTriggerAgent.Instance.addEvent(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/Main"], EventTriggerType.PointerClick, () =>
                {
                    selected = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/Main"];
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
                });

                EventTriggerAgent.Instance.addEvent(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G1"], EventTriggerType.PointerClick, () =>
                {
                    selected = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G1"];
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_normal"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_select"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
                });

                EventTriggerAgent.Instance.addEvent(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G2"], EventTriggerType.PointerClick, () =>
                {
                    selected = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G2"];
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_normal"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_select"]);
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Top/Option", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Pause"], true);
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Pause/Button/Cancel", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Pause"], false);
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Pause/Button/Continue", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Pause"], false);
                });

                var puzzleGame = manager.GetComponent<PuzzleGame>();
                GUIAgent.Instance.addListener("UI", "3_Ingame/Pause/Button/Replay", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Pause"], false);
                    puzzleGame.init();
                    clearBase();
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Bottom/Button/Play", () =>
                {
                    var sprite = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Button/Play"].GetComponent<UnityEngine.UI.Image>().sprite;

                    if (sprite.Equals(ResourcesManager.Instance.sprites["bt_play"]))
                    {
                        GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Button/Play", ResourcesManager.Instance.sprites["bt_stop"]);
                        puzzleGame.play();
                    }
                    else
                    {
                        GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Button/Play", ResourcesManager.Instance.sprites["bt_play"]);
                        puzzleGame.init();
                    }
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Pause/Button/Setting", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option"], true);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Setting"], true);
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Pause/Button/Stop", () =>
                {//
                    clearBase();
                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Button/Play", ResourcesManager.Instance.sprites["bt_play"]);
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], true, GameManager.Close + GameManager.Delay);                    
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Pause"], false, GameManager.Close + GameManager.Delay);                    
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom"], true, GameManager.Close + GameManager.Delay);
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Clear/Bottom/Button/Next", () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    if (PuzzleGame.stage > 10)
                    {
                        PuzzleGame.step++;
                        PuzzleGame.stage = 1;
                    }
                    
                    if (PuzzleGame.step == 3 && PuzzleGame.stage == 1)
                    {
                        GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear"], false, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top"], true, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom"], true, GameManager.Close + GameManager.Delay);                        
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart"], true, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Earth"], true, GameManager.Close + GameManager.Delay);
                        GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], false, GameManager.Close + GameManager.Delay);
                        return;
                    }

                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Button/Play", ResourcesManager.Instance.sprites["bt_play"]);
                    GameObjectAgent.Instance.delaySetActive(light, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(lightOfIngame, true, GameManager.Close + GameManager.Delay);                                       

                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Clear"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Bg"], true, GameManager.Close + GameManager.Delay);

                    Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                    {
                        orthographicCamera.enabled = true;
                        mainCamera.enabled = false;

                        GameObjectAgent.Instance.setActive(level, true);
                        GameObjectAgent.Instance.setActive(manager, true);

                        manager.GetComponent<PuzzleGame>().init();
                        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 0);
                    });

                    clearBase();
                });

                GUIAgent.Instance.addListener("UI", "3_Ingame/Clear/Bottom/Button/Back", () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], true, GameManager.Close + GameManager.Delay);
                    
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, true, GameManager.Close + GameManager.Delay);
                    
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Clear"], false, GameManager.Close + GameManager.Delay);                    
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom"], true, GameManager.Close + GameManager.Delay);

                    GameObjectAgent.Instance.delaySetActive(light, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(lightOfIngame, true, GameManager.Close + GameManager.Delay);

                    GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Button/Play", ResourcesManager.Instance.sprites["bt_play"]);


                    //GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], false);
                    //GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/1_Lobby"], true);
                    //GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], true);                    

                    clearBase();
                });
            }

            for (int i = 0; i < Constants.imageNames.Length; i++)
            {
                var name = Constants.imageNames[i];
                var parentOfPanel = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Panel/" + name);
                GUIAgent.Instance.addListener("UI", "3_Ingame/Bottom/Button/Layout/" + name, () =>
                {
                    if (!selected) return;
                    int countOfContainer = 0;// = selected.name.Contains("Main") ? 16 : 8;
                    if (selected.name.Contains("Main")) countOfContainer = countOfBase;
                    if (selected.name.Contains("G1")) countOfContainer = countOfG1;
                    if (selected.name.Contains("G2")) countOfContainer = countOfG2;

                    if (selected.transform.childCount < countOfContainer && parentOfPanel.transform.childCount > 0)
                        parentOfPanel.transform.GetChild(0).SetParent(selected.transform);

                });
            }
            #endregion add event

            #region init

            createPanel(32);
            selected = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/Main"];
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
            GameObjectAgent.Instance.setActive(gameObject, false);
            GameObjectAgent.Instance.setActive("UI", "3_Ingame/Pause", false);
            GameObjectAgent.Instance.setActive("UI", "3_Ingame/Clear", false);
            GameObjectAgent.Instance.setActive("Background", "3_Ingame/Clear", false);
            GameObjectAgent.Instance.setActive("Background", "3_Ingame", false);

            #endregion init
        }
        internal int countOfBase = 16;
        internal int countOfG1 = 8;
        internal int countOfG2 = 8;        

        internal void init()
        {
            selected = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/Main"];
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
            GameObjectAgent.Instance.setActive(gameObject, true);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Clear"], false);
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame"], true);
        }

        void createPanel(int count)
        {            
            var prefab = ResourcesManager.Instance.prefabs["panel"];

            var spriteOfIndicatorChange = ResourcesManager.Instance.sprites["indicator_change"];
            var spriteOfIndicatorBetween = ResourcesManager.Instance.sprites["indicator_between"];

            //var indictorChange = GUIAgent.Instance.createSub(prefab, new Vector2(0, -1200), spriteOfIndicatorChange, "IndicatorChange", "UI", "3_Ingame", true);
            //var indictorBetween = GUIAgent.Instance.createSub(prefab, new Vector2(0, -1200), spriteOfIndicatorBetween, "IndicatorBetween", "UI", "3_Ingame", true);

            var p = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Panel/PanelX");
            var s = ResourcesManager.Instance.sprites["panel_limit"];
            for (int i = 0; i < count; i++)
            {
                var button = GUIAgent.Instance.createSub(prefab, Vector3.one, s, "X", p, true);
                button.GetComponent<UnityEngine.UI.Image>().raycastTarget = false;
                button.transform.position = new Vector2(0, -1200);
                button.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().raycastTarget = false;
            }
            

            for (int i = 0; i < Constants.imageNames.Length; i++)
            {                
                var sprite = ResourcesManager.Instance.sprites["panel_" + Constants.imageNames[i].ToLower()];
                var parent = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Panel/" + Constants.imageNames[i]);

                for (int j = 0; j < count; j++)
                {
                    var button = GUIAgent.Instance.createSub(prefab, Vector3.one, sprite, Constants.imageNames[i], parent, true);
                    EventTriggerAgent.Instance.addEvent(button, UnityEngine.EventSystems.EventTriggerType.Drag, () =>
                    {
                        button.transform.position = Input.mousePosition;                   
                        //indictorChange.transform.position = button.position;

                        var width = Mathf.Abs(selected.transform.GetChild(0).position.x - selected.transform.GetChild(0).position.x);
                        var height = Mathf.Abs(selected.transform.GetChild(0).position.y - selected.transform.GetChild(0).position.y);

                        for (int k = 0; k < selected.transform.childCount; k++)
                        {
                            if (k + 1 < selected.transform.childCount)
                            {
                                if (button.transform.position.x > selected.transform.GetChild(k).position.x &&
                                    button.transform.position.x < selected.transform.GetChild(k + 1).position.x &&
                                    button.transform.position.y < selected.transform.GetChild(k).position.y + height / 2 &&
                                    button.transform.position.y > selected.transform.GetChild(k).position.y - height / 2)
                                {
                                    //indictorBetween.position = (selected.GetChild(k).position + selected.GetChild(k + 1).position) / 2;
                                }
                            }
                        }

                        var last = selected.transform.GetChild(selected.transform.childCount - 1);

                        if (button.transform.position.y < last.position.y + height / 2)
                        {
                            //indictorBetween.position = last.position + new Vector3(20, 0, 0);
                        }

                        if (button.transform.position.x > last.position.x + width / 2 &&
                            button.transform.position.y < last.position.y + height / 2 &&
                            button.transform.position.y > last.position.y - height / 2)
                        {
                            //indictorBetween.position = last.position + new Vector3(20, 0, 0);
                        }

                    });

                    EventTriggerAgent.Instance.addEvent(button, UnityEngine.EventSystems.EventTriggerType.EndDrag, () =>
                    {
                        //indictorChange.transform.position = new Vector2(0, -1200);
                        //indictorBetween.transform.position = new Vector2(0, -1200);
                        if (!selected) return;
                        int countOfContainer = selected.name.Contains("Main") ? 16 : 8;

                        if (selected.transform.childCount < countOfContainer)
                            button.transform.SetParent(selected.transform);

                        for (int k = 0; k < selected.transform.childCount; k++)
                        {
                            if (k + 1 < selected.transform.childCount)
                            {
                                if (button.transform.position.x > selected.transform.GetChild(k).position.x &&
                                    button.transform.position.x < selected.transform.GetChild(k + 1).position.x &&
                                    button.transform.position.y < selected.transform.GetChild(k).position.y + 20 &&
                                    button.transform.position.y > selected.transform.GetChild(k).position.y - 20)
                                {
                                    button.transform.SetSiblingIndex(k);
                                }
                            }
                        }
                    });

                    EventTriggerAgent.Instance.addEvent(button, UnityEngine.EventSystems.EventTriggerType.PointerClick, () =>
                    {
                        button.transform.position = new Vector2(0, -1200);
                        button.transform.SetParent(parent.transform);
                    });
                }
            }
        }

        internal void clearBase()
        {
            var parent = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Panel"];
            var baseMain = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/Main"];
            var baseG1 = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G1"];
            var baseG2 = GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom/Base/G2"];

            for (int i = baseMain.transform.childCount - 1; i > -1; --i)
            {
                baseMain.transform.GetChild(i).position = new Vector2(0, -1200);
                baseMain.transform.GetChild(i).SetParent(parent.transform.FindChild(baseMain.transform.GetChild(i).name));
            }

            for (int i = baseG1.transform.childCount - 1; i > -1; --i)
            {
                baseG1.transform.GetChild(i).position = new Vector2(0, -1200);
                baseG1.transform.GetChild(i).SetParent(parent.transform.FindChild(baseG1.transform.GetChild(i).name));
            }

            for (int i = baseG2.transform.childCount - 1; i > -1; --i)
            {
                baseG2.transform.GetChild(i).position = new Vector2(0, -1200);
                baseG2.transform.GetChild(i).SetParent(parent.transform.FindChild(baseG2.transform.GetChild(i).name));
            }
        }
    } // class
}//namespcae