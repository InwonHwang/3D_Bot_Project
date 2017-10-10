using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


using Robot.Singleton;

namespace Robot.GUI
{
    internal class Option : MonoBehaviour
    {

        void Start()
        {
            #region find object

            //var activePart = GameObjectAgent.Instance.getComponent<PartsManager>("Object", "Parts").ActiveParts;

            #endregion

            #region set 4_Option
            {
                GUIAgent.Instance.addListener("UI", "4_Option/Setting/Bt_Cancel", () =>
                {
                    GameObjectAgent.Instance.setActive(gameObject, false);
                });

                GUIAgent.Instance.addListener("UI", "4_Option/Connect/Bt_Cancel", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Connect"], false);
                    GUIAgent.Instance.setRaycastTarget("UI", "4_Option/Setting/Bt_Cancel", true);
                });

                GUIAgent.Instance.addListener("UI", "4_Option/Account/Bt_Cancel", () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account"], false);
                    GUIAgent.Instance.setRaycastTarget("UI", "4_Option/Connect/Bt_Cancel", true);
                });

                EventTriggerAgent.Instance.addEvent("UI", "4_Option/Setting/Account", EventTriggerType.PointerClick, () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Connect"], true);
                    GUIAgent.Instance.setRaycastTarget("UI", "4_Option/Setting/Bt_Cancel", false);
                });

                EventTriggerAgent.Instance.addEvent("UI", "4_Option/Connect/Facebook", EventTriggerType.PointerClick, () =>
                {                   
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account/Title_Facebook"], true);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account/Title_Twitter"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account"], true);
                    GUIAgent.Instance.setRaycastTarget("UI", "4_Option/Connect/Bt_Cancel", false);
                    
                });

                EventTriggerAgent.Instance.addEvent("UI", "4_Option/Connect/Twitter", EventTriggerType.PointerClick, () =>
                {
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account/Title_Facebook"], false);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account/Title_Twitter"], true);
                    GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account"], false);
                    GUIAgent.Instance.setRaycastTarget("UI", "4_Option/Connect/Bt_Cancel", true);
                });

                GUIAgent.Instance.addListener("UI", "4_Option/Button", () =>
                {
                    GUIAgent.Instance.setEnabled("MainGUI", true);
                    GUIAgent.Instance.setEnabled("UI", false);
                    GUIAgent.Instance.setEnabled("JoyStickGUI", false);
                    GUIAgent.Instance.setEnabled("BlueToothGUI", false);
                    GUIAgent.Instance.setEnabled("Background", false);
                });

                GUIAgent.Instance.addListener("MainGUI", "Back", () =>
                {
                    GUIAgent.Instance.setEnabled("MainGUI", false);
                    GUIAgent.Instance.setEnabled("UI", true);
                    GUIAgent.Instance.setEnabled("JoyStickGUI", false);
                    GUIAgent.Instance.setEnabled("BlueToothGUI", false);
                    GUIAgent.Instance.setEnabled("Background", true);
                });                
            }
            #endregion set 4_Option

            #region init
            {
                GameObjectAgent.Instance.setActive(gameObject, false);                
                GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Connect"], false);
                GameObjectAgent.Instance.setActive(GUIAgent.Instance.GuiObjects["UI/4_Option/Account"], false);
                GameObjectAgent.Instance.setActive("UI", "4_Option/Account/Login_2", false);
            }
            #endregion init
        }
    }
}