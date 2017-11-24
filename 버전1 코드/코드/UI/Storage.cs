using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Robot.Singleton;

namespace Robot.GUI
{
    internal class Storage : MonoBehaviour
    {
        Customizing customizing;
        
        GameObject parts;
        GameObject robot;
        GameObject selectedRobot;
        GameObject nameOfPanel;
        GameObject[] activePart;

        void Awake()
        {
            #region find object

            parts = GameObjectAgent.Instance.findChild("Object", "Parts");
            selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
            nameOfPanel = GameObjectAgent.Instance.findChild("UI", "6_Storage/Panel/Name");
            activePart = parts.GetComponent<PartsManager>().ActiveParts;
            robot = GameObjectAgent.Instance.findChild("Object", "Robot");
            customizing = GameObjectAgent.Instance.getComponent<Customizing>("UI", "2_Customizing");
            var table = GameObjectAgent.Instance.findChild("Object", "Table");  

            #endregion find object

            #region add evnet
            {
                GUIAgent.Instance.addListener("UI", "6_Storage/FrameSub/Back", () =>
                {
                    if (selectedRobot.transform.childCount == 0 && customizing.nameOfSelected != null)
                    {
                        Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                        {
                            var selected = robot.transform.FindChild(customizing.nameOfSelected);
                            if (robot.transform.childCount > 0) selected.SetParent(selectedRobot.transform);
                        });                        
                    }

                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/6_Storage"], false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/1_Lobby"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Left"], true, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/2_Customizing/Button/Right"], true, GameManager.Close + GameManager.Delay);

                    GameObjectAgent.Instance.delaySetActive(robot, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(table, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, false, GameManager.Close + GameManager.Delay);
                });

                GUIAgent.Instance.addListener("UI", "6_Storage/bt", () =>
                {
                    PlayerPrefs.DeleteAll();
                });
                //수정하기

                GUIAgent.Instance.addListener("UI", "6_Storage/FrameSub/Customizing", () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();

                    if (selectedRobot.transform.childCount == 0 && customizing.nameOfSelected != null)
                    {
                        Debug.Log(customizing.nameOfSelected);
                        Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                        {
                            var selected = robot.transform.FindChild(customizing.nameOfSelected);
                            if (robot.transform.childCount > 0) selected.SetParent(selectedRobot.transform);
                        });
                    }

                    GameObjectAgent.Instance.setActive(parts, false);
                    GameObjectAgent.Instance.delaySetActive(selectedRobot, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);
                    GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/6_Storage"], false, GameManager.Close + GameManager.Delay);

                    if (customizing.nameOfSelected != null)
                        loadPart(customizing.nameOfSelected);
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

                //수정하기
                // scrollableOfSaved = GameObjectAgent.Instance.findChild("UI", "6_Storage/Frame/ScrollPanel/Bot");
                GUIAgent.Instance.addListener("UI", "6_Storage/FrameSub/Choice", () =>
                {
                    

                    //if(selectedRobot.transform.childCount >0 && robot.transform.childCount != 0)
                    //{
                    //    selectedRobot.transform.GetChild(0).gameObject.SetActive(false);
                    //    selectedRobot.transform.GetChild(0).SetParent(robot.transform);
                    //}

                    for(int i = 0; i < robot.transform.childCount; i++)
                    {
                        var toBeSelected = robot.transform.GetChild(i);
                        if (toBeSelected.gameObject.activeSelf)
                            customizing.nameOfSelected = toBeSelected.name;
                            //toBeSelected.SetParent(selectedRobot.transform);                        
                    }                                        

                });
            }
            #endregion add event

            #region init
            {
                GameObjectAgent.Instance.setActive("UI", "6_Storage", false);
                GameObjectAgent.Instance.setActive("Background", "6_Storage", false);
            }
            #endregion
        }

        internal void loadPart(string fileName)
        {
            GUIAgent.Instance.setText("UI", "6_Storage/Panel/Name", fileName);
            var data = PlayerPrefs.GetString(fileName, "N/A");
            var botInfo = Converter.Deserialize<EntityType.BotInfo>(data);
            
            for (int i = 0; i < botInfo.Parts.Length; i++)
            {
                string partName = botInfo.Parts[i].Name.Split('/')[0];
                var part = parts.transform.FindChild(partName).GetChild(0);                
                part.position = botInfo.Parts[i].Position;
                part.rotation = botInfo.Parts[i].Rotation;
                part.localScale = botInfo.Parts[i].Scale;
                part.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = botInfo.Parts[i].Color;
                part.gameObject.SetActive(true);                
                parts.GetComponent<PartsManager>().activate(partName);
            }

            for (int i = 0; i < botInfo.StickerMesh.Count; i++)
            {
                var parent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + botInfo.StickerMesh[i].Name);

                if (parent.transform.childCount == 1) continue;

                var sticker = parent.transform.GetChild(1);
                var stickerInfo = botInfo.StickerMesh[i];

                sticker.GetComponent<DecalSystem.Decal>().Info = stickerInfo;
                sticker.GetComponent<DecalSystem.Decal>().createMesh();

                var newParent = GameObjectAgent.Instance.findChild("Object/", "Parts/" + stickerInfo.GParentName).transform.GetChild(0).FindChild(stickerInfo.BoneName);
                sticker.SetParent(newParent);
                DecalSystem.DecalBuilder.BuildDecal(sticker.GetComponent<DecalSystem.Decal>());
                
            }
        }

        internal void loadPart2(string fileName)
        {            
            GUIAgent.Instance.setText("UI", "6_Storage/Panel/Name", fileName);            
            var data = PlayerPrefs.GetString(fileName, "N/A");
            var botInfo = Converter.Deserialize<EntityType.BotInfo>(data);

            if (parts == null) Debug.Log("??");

            for(int i = 0; i < botInfo.Parts.Length; i++)
            {
                var part = parts.transform.FindChild(botInfo.Parts[i].Name);
                part.position = botInfo.Parts[i].Position;
                part.rotation = botInfo.Parts[i].Rotation;
                part.localScale = botInfo.Parts[i].Scale;
                part.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.color = botInfo.Parts[i].Color;
                part.gameObject.SetActive(true);
                parts.GetComponent<PartsManager>().activate(part.parent.name);
                part.SetParent(robot.transform.FindChild(fileName));
            }           

            for (int i = 0; i < botInfo.StickerMesh.Count; i++)
            {
                var parent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + botInfo.StickerMesh[i].Name);

                if (parent.transform.childCount == 1) continue;

                var sticker = parent.transform.GetChild(1);
                var stickerInfo = botInfo.StickerMesh[i];

                sticker.GetComponent<DecalSystem.Decal>().Info = stickerInfo;
                sticker.GetComponent<DecalSystem.Decal>().createMesh();
                var newParent = GameObjectAgent.Instance.findChild("Object/", "Robot/" + fileName + "/" + stickerInfo.ParentName + "/" + stickerInfo.BoneName);
                sticker.SetParent(newParent.transform);
                DecalSystem.DecalBuilder.BuildDecal(sticker.GetComponent<DecalSystem.Decal>());
            }
        }
    }
}