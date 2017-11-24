using UnityEngine;
using System.Collections;


using Robot.Singleton;

namespace Robot.GUI
{
    internal class Lobby : MonoBehaviour
    {

        void Start()
        {
            #region find object

            var robot = GameObjectAgent.Instance.findChild("Object", "Robot");
            var selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
            var parts = GameObjectAgent.Instance.findChild("Object", "Parts");
            //var activePart = parts.GetComponent<PartsManager>().ActiveParts;
            var table = GameObjectAgent.Instance.findChild("Object", "Table");
            var light = GameObjectAgent.Instance.findChild("Light", "Base");
            var lightOfIngame = GameObjectAgent.Instance.findChild("Light", "Ingame");
            var customizing = GameObjectAgent.Instance.getComponent<Customizing>("UI", "2_Customizing");
            var gameStart = GameObjectAgent.Instance.getComponent<GameStart>("UI", "7_GameStart");
            var storage = GameObjectAgent.Instance.getComponent<Storage>("UI", "6_Storage");

            #endregion find object

            #region set 1_Lobby
            {
                GUIAgent.Instance.addListener("UI", "1_Lobby/GameStart", () =>
                {
                    if (selectedRobot.transform.childCount == 0)
                    {
                        GUIAgent.Instance.GuiObjects["UI/1_Lobby/Notice"].SetActive(true);
                        return;
                    }                    
                    //GameManager.Instance.transition();
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    selectedRobot.transform.GetChild(0).gameObject.SetActive(true);
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(light, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(lightOfIngame, true, GameManager.Close + GameManager.Delay);
                    gameStart.init();
                    

                    GameObject.Find("Camera/Main Camera").GetComponent<HighlightingSystem.HighlightingRenderer>().enabled = false;
                });

                GUIAgent.Instance.addListener("UI", "1_Lobby/Notice/Cancel", () =>
                {
                    GUIAgent.Instance.GuiObjects["UI/1_Lobby/Notice"].SetActive(false);
                });

                GUIAgent.Instance.addListener("UI", "1_Lobby/Customizing", () =>
                {                    
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.setActive(parts, false);
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);                    
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], false, GameManager.Close + GameManager.Delay);

                    if (customizing.nameOfSelected != null)
                        storage.loadPart(customizing.nameOfSelected);
                    else
                    {
                        var partsManager = parts.GetComponent<PartsManager>();
                        partsManager.activate("bot_head_black");
                        partsManager.activate("bot_body_black");
                        partsManager.activate("bot_arm_left_black");
                        partsManager.activate("bot_arm_right_black");
                        partsManager.activate("bot_leg_left_black");
                        partsManager.activate("bot_leg_right_black");
                    }

                    customizing.init();                    
                });

                GUIAgent.Instance.addListener("UI", "1_Lobby/Setting", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option"], true);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Setting"], true);
                });

                
                GUIAgent.Instance.addListener("UI", "1_Lobby/Storage", () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/6_Storage/Title/Prev"]);
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/6_Storage/Title/Next"]);

                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.setActive(parts, false);
                    GameObjectAgent.Instance.delaySetActive(robot, true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, true, GameManager.Close + GameManager.Delay);

                    robot.transform.rotation = Quaternion.identity;

                    if (selectedRobot.transform.childCount > 0) //선택 -> 창고                    
                        selectedRobot.transform.GetChild(0).SetParent(robot.transform);                    

                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/6_Storage"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/6_Storage"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(table, true, GameManager.Close + GameManager.Delay);


                    robot.transform.position = new Vector3(-1.3f, 0, 0);
                    table.transform.position = new Vector3(-1.1f, -2, 0);
                    robot.transform.rotation = Quaternion.Euler(0, 330, 0);

                    var scrollableOfStorage = GameObjectAgent.Instance.addComponent<GUIScrollable>(GUIAgent.Instance.GuiObjects["UI/6_Storage/Frame/ScrollPanel/Bot"]);
                    scrollableOfStorage.init("save_");
                    scrollableOfStorage.action = () =>
                    {
                        if (selectedRobot.transform.childCount > 0) selectedRobot.transform.GetChild(0).SetParent(robot.transform);

                        for (int i = 0; i < robot.transform.childCount; i++)
                        {
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
                                child.SetActive(false);
                        }
                    };
                });

                GUIAgent.Instance.GuiObjects["UI/1_Lobby/Notice"].SetActive(false);
            }
            #endregion set 1_Lobby

            #region init

            GameObjectAgent.Instance.setActive(lightOfIngame, false);

            #endregion init
        }
       
    }
}