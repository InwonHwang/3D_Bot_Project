using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

using Robot.Singleton;

namespace Robot.GUI
{
    internal class MainGUI : MonoBehaviour
    {
        GameObject selected;
        GameObject baseMain;
        GameObject baseG1;
        GameObject baseG2;
        GameObject parent;
        GameObject titleOfMain;
        GameObject titleOfG1;
        GameObject titleOfG2;
        Ingame ingame;

        void Start()
        {
            #region find object
            CarBot carBot = gameObject.AddComponent<CarBot>();
            carBot.MovementSpeed = 10f;
            ingame = GameObjectAgent.Instance.getComponent<Ingame>("MainGUI", "3_Ingame");
            baseMain = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Base/Main");
            baseG1 = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Base/G1");
            baseG2 = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Base/G2");
            parent = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Panel");

            titleOfMain = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Title/Main");
            titleOfG1 = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Title/G1");
            titleOfG2 = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Title/G2");
            //var bottom = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom");




            #endregion find object           

            #region add event
            {
                EventTriggerAgent.Instance.addEvent(baseMain, EventTriggerType.PointerClick, () =>
                {
                    Debug.Log("??");
                    selected = baseMain;
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
                });

                EventTriggerAgent.Instance.addEvent(baseG1, EventTriggerType.PointerClick, () =>
                {
                    Debug.Log("??");
                    selected = baseG1;
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_normal"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_select"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
                });

                EventTriggerAgent.Instance.addEvent(baseG2, EventTriggerType.PointerClick, () =>
                {
                    Debug.Log("??");
                    selected = baseG2;
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_normal"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
                    GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_select"]);
                });


                GUIAgent.Instance.addListener("UI", "3_Ingame/Bottom/Button/Play", () =>
                {
                    //puzzleGame.play();
                });                

              
            }

            for (int i = 0; i < Constants.imageNames.Length; i++)
            {
                var name = Constants.imageNames[i];
                var parentOfPanel = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Panel/" + name);
                GUIAgent.Instance.addListener("MainGUI", "3_Ingame/Bottom/Button/Layout/" + name, () =>
                {
                    if (!selected) return;
                    int countOfContainer = selected.name.Contains("Main") ? 16 : 8;

                    if (selected.transform.childCount < countOfContainer && parentOfPanel.transform.childCount > 0)
                        parentOfPanel.transform.GetChild(0).SetParent(selected.transform);
                });
            }
            #endregion add event

            #region init

            createPanel(32);
            selected = baseMain;
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);

            #endregion init
        }

        internal void init()
        {
            selected = baseMain;
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/Main", ResourcesManager.Instance.sprites["title_MAIN_select"]);
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G1", ResourcesManager.Instance.sprites["title_G1_normal"]);
            GUIAgent.Instance.setSprite("MainGUI", "3_Ingame/Bottom/Title/G2", ResourcesManager.Instance.sprites["title_G2_normal"]);
            GameObjectAgent.Instance.setActive(gameObject, true);
        }

        void createPanel(int count)
        {
            var prefab = ResourcesManager.Instance.prefabs["panel"];

            var spriteOfIndicatorChange = ResourcesManager.Instance.sprites["indicator_change"];
            var spriteOfIndicatorBetween = ResourcesManager.Instance.sprites["indicator_between"];

            //var indictorChange = GUIAgent.Instance.createSub(prefab, new Vector2(0, -1200), spriteOfIndicatorChange, "IndicatorChange", "UI", "3_Ingame", true);
            //var indictorBetween = GUIAgent.Instance.createSub(prefab, new Vector2(0, -1200), spriteOfIndicatorBetween, "IndicatorBetween", "UI", "3_Ingame", true);

            for (int i = 0; i < Constants.imageNames.Length; i++)
            {
                var sprite = ResourcesManager.Instance.sprites["panel_" + Constants.imageNames[i].ToLower()];
                var parent = GameObjectAgent.Instance.findChild("MainGUI", "3_Ingame/Bottom/Panel/" + Constants.imageNames[i]).gameObject;

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