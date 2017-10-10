using UnityEngine;
using System.Collections;


using Robot.Singleton;

namespace Robot.GUI
{
    internal sealed class GameStart : MonoBehaviour
    {        
        GameObject robot;
        GameObject selectedRobot;
        GameObject parts;
        Camera orthographicCamera;
        
        void Start()
        {
            #region find object

            orthographicCamera = GameObjectAgent.Instance.findChild("Camera", "Orthographic Camera").GetComponent<Camera>();
            
            parts = GameObjectAgent.Instance.findChild("Object", "Parts");
            robot = GameObjectAgent.Instance.findChild("Object", "Robot");
            selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");

            //var activePart = parts.GetComponent<PartsManager>().ActiveParts;
            var manager = GameObject.Find("Manager");
            var level = GameObject.Find("Level");            
            var mainCamera = GameObjectAgent.Instance.getComponent<Camera>("Camera", "Main Camera");            
            
            
            #endregion find object

            #region add event

            GUIAgent.Instance.addListener("UI", "7_GameStart/Earth", () =>
            {
                GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], true, GameManager.Close + GameManager.Delay);
            });

            GUIAgent.Instance.addListener("UI", "7_GameStart/Back", () =>
            {                
                GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame"], false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame"], false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(selectedRobot, false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(level, false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive("Light", "Ingame", false, GameManager.Close + GameManager.Delay);
                
                GameObjectAgent.Instance.delaySetActive("Light", "Base", true, GameManager.Close + GameManager.Delay);
                Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                {
                    mainCamera.enabled = true;
                    orthographicCamera.enabled = false;                    
                    selectedRobot.transform.position = Vector3.zero;
                    selectedRobot.transform.GetChild(0).localScale = Vector3.one;                    
                });
            });


            var parent = GameObjectAgent.Instance.findChild("UI", "7_GameStart/Stage/ScrollPanel");
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var stage = parent.transform.GetChild(i).gameObject;
                int numOfStage = i + 1;
                GUIAgent.Instance.addListener("UI", "7_GameStart/Stage/ScrollPanel/" + (i+1).ToString(), () =>
                {                    

                    if (numOfStage % 10 == 0)
                    {
                        PuzzleGame.stage = 10;
                        PuzzleGame.step = numOfStage / 10;
                    }
                    else
                    {
                        PuzzleGame.stage = numOfStage % 10;
                        PuzzleGame.step = numOfStage / 10 + 1;
                    }

                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);

                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Bg"], true, GameManager.Close + GameManager.Delay);

                    Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                    {
                        GameObjectAgent.Instance.setActive(level, true);
                        GameObjectAgent.Instance.setActive(manager, true);

                        manager.GetComponent<PuzzleGame>().init();

                        mainCamera.enabled = false;
                        orthographicCamera.enabled = true;
                    });                    
                });
            }

            #endregion add event

            #region set active

            orthographicCamera.enabled = false;
            GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], false);
            GameObjectAgent.Instance.setActive(gameObject, false);

            #endregion set active
        }

        internal void init()
        {
            GameObjectAgent.Instance.delaySetActive(selectedRobot, true, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(parts, false, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(gameObject, true, GameManager.Close + GameManager.Delay);
            GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage"], false, GameManager.Close + GameManager.Delay);            
        }
    } // class GameStart

} // namespace Robot.GUI