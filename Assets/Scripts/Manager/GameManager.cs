using UnityEngine;
using System.Collections;
using Robot.Singleton;
using Robot.GUI;


public class GameManager : MonoBehaviour
{

    void Start()
    {
        var parts = GameObjectAgent.Instance.findChild("Object", "Parts");
        var selectedBot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
        var activePart = parts.GetComponent<PartsManager>().ActiveParts;
        var table = GameObjectAgent.Instance.findChild("Object", "Table");
        var robot = GameObjectAgent.Instance.findChild("Object", "Robot");

        GUIAgent.Instance.registerObject("UI");
        GUIAgent.Instance.registerObject("UI", "1_Lobby");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/Notice");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/Notice/Cancel");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/GameStart");
        //GUIAgent.Instance.registerObject("UI", "1_Lobby/Button");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/Customizing");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/Setting");
        GUIAgent.Instance.registerObject("UI", "1_Lobby/Storage");            
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Button/Left");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Button/Right");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Center");        
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/Image/Parts");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/Image/Color");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/Image/Scale");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/Image/Sticker");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Head");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Body");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Arm_Left");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Arm_Right");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Leg_Left");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Parts/Leg_Right");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Color");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Sticker");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Naming");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Complete");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Cancel");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number");
        for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/2_Customizing/Frame/ScrollPanel/Scale/Number"].transform.childCount; ++i)
            GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Number/Number_" + (i + 1));
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Sticker/Panel");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/FrameVignetting");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Complete/Panel/Text/Name");        
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Center/Toggle");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Right");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Frame/ScrollPanel/Scale/Left");        
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Title/Next");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Title/Prev");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Button/Storage");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Naming/Complete");
        GUIAgent.Instance.registerObject("UI", "2_Customizing/Complete/Frame/Back");
        GUIAgent.Instance.registerObject("UI", "3_Ingame");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/PopUp");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Base/Main");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Base/G1");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Base/G2");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause");        
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Top");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Panel");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Top/Option");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause/Button/Cancel");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause/Button/Continue");       
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause/Button/Replay");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Button/Play");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause/Button/Setting");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Pause/Button/Stop");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Clear");

        GUIAgent.Instance.registerObject("UI", "3_Ingame/Clear/Top/Title/Prev");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Clear/Top/Title/Next");

        GUIAgent.Instance.registerObject("UI", "3_Ingame/Clear/Bottom/Button/Next");  
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Clear/Bottom/Button/Back");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Title/Main");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Title/G1");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Title/G2");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Top/Image/Number_Stage");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Top/Image/Number_Zero");
        GUIAgent.Instance.registerObject("UI", "3_Ingame/Top/Image/Number_Step");
        GUIAgent.Instance.registerObject("UI", "4_Option/Setting/Bt_Cancel");
        GUIAgent.Instance.registerObject("UI", "4_Option/Connect/Bt_Cancel");
        GUIAgent.Instance.registerObject("UI", "4_Option/Account/Bt_Cancel");



        GUIAgent.Instance.registerObject("UI", "6_Storage/FrameSub/Back");
        GUIAgent.Instance.registerObject("UI", "6_Storage/bt");
        GUIAgent.Instance.registerObject("UI", "6_Storage/FrameSub/Customizing");
        GUIAgent.Instance.registerObject("UI", "6_Storage/FrameSub/Choice");
        for (int i = 0; i < Constants.imageNames.Length; i++)        
            GUIAgent.Instance.registerObject("UI", "3_Ingame/Bottom/Button/Layout/" + Constants.imageNames[i]);
        GUIAgent.Instance.registerObject("UI", "4_Option");
        GUIAgent.Instance.registerObject("UI", "4_Option/Setting");
        GUIAgent.Instance.registerObject("UI", "4_Option/Connect");
        GUIAgent.Instance.registerObject("UI", "4_Option/Account");
        GUIAgent.Instance.registerObject("UI", "4_Option/Account/Title_Facebook");
        GUIAgent.Instance.registerObject("UI", "4_Option/Account/Title_Twitter");
        GUIAgent.Instance.registerObject("UI", "6_Storage");
        GUIAgent.Instance.registerObject("UI", "6_Storage/Title/Prev");
        GUIAgent.Instance.registerObject("UI", "6_Storage/Title/Next");
        GUIAgent.Instance.registerObject("UI", "6_Storage/Frame/ScrollPanel/Bot");
        GUIAgent.Instance.registerObject("UI", "6_Storage/Panel/Name");
        GUIAgent.Instance.registerObject("UI", "7_GameStart");
        GUIAgent.Instance.registerObject("UI", "7_GameStart/Earth");
        GUIAgent.Instance.registerObject("UI", "7_GameStart/Stage");
        GUIAgent.Instance.registerObject("UI", "7_GameStart/Back");
        GUIAgent.Instance.registerObject("UI", "7_GameStart/Stage/ScrollPanel");
        for (int i = 0; i < GUIAgent.Instance.GuiObjects["UI/7_GameStart/Stage/ScrollPanel"].transform.childCount; i++)        
            GUIAgent.Instance.registerObject("UI", "7_GameStart/Stage/ScrollPanel/" + (i + 1));
        GUIAgent.Instance.registerObject("Background", "1_Lobby");
        GUIAgent.Instance.registerObject("Background", "2_Customizing");
        GUIAgent.Instance.registerObject("Background", "2_Customizing/Clear");
        GUIAgent.Instance.registerObject("Background", "3_Ingame/Clear");
        GUIAgent.Instance.registerObject("Background", "3_Ingame/Bg");
        GUIAgent.Instance.registerObject("Background", "3_Ingame");
        GUIAgent.Instance.registerObject("Background", "6_Storage");
        GUIAgent.Instance.registerObject("0_Transition", "Top");
        GUIAgent.Instance.registerObject("0_Transition", "Bottom");

        GUIAgent.Instance.registerObject("JoyStickGUI");
        GUIAgent.Instance.registerObject("JoyStickGUI", "BTPanel/Start");
        GUIAgent.Instance.registerObject("BlueToothGUI");
        GUIAgent.Instance.registerObject("MainGUI");

        AnimationAgent.Instance.registerAnimator(selectedBot.name, selectedBot.GetComponent<Animator>());
        for (int i = 0; i < activePart.Length; i++)
            AnimationAgent.Instance.setInteger(activePart[i].name, "State", 0);

        CarBot carBot = GameObjectAgent.Instance.addComponent<CarBot>("UI", "3_Ingame");
        GameObjectAgent.Instance.addComponent<Lobby>("UI", "1_Lobby");
        GameObjectAgent.Instance.addComponent<Customizing>("UI", "2_Customizing");
        GameObjectAgent.Instance.addComponent<Ingame>("UI", "3_Ingame");
        GameObjectAgent.Instance.addComponent<Option>("UI", "4_Option");
        GameObjectAgent.Instance.addComponent<Storage>("UI", "6_Storage");
        GameObjectAgent.Instance.addComponent<GameStart>("UI", "7_GameStart");

        string tmp = PlayerPrefs.GetString("name", "N/A");
        string[] nameOfBots = tmp.Split('/');
        if (!tmp.Contains("N/A") && nameOfBots != null && nameOfBots.Length > 0)
        {
            for (int i = 0; i < nameOfBots.Length - 1; i++)
            {
                GameObject newBot = new GameObject(nameOfBots[i]);
                newBot.SetActive(true);
                newBot.name = nameOfBots[i];
                newBot.transform.SetParent(robot.transform);
                GameObjectAgent.Instance.getComponent<Storage>(GUIAgent.Instance.GuiObjects["UI/6_Storage"]).loadPart2(nameOfBots[i]);
            }
        }

        string[] nameOfSprites = PlayerPrefs.GetString("nameOfSprite").Split('/');
        for (int i = 0; i < nameOfSprites.Length - 1; i++)
        {
            Texture2D text = new Texture2D(460, 460, TextureFormat.ARGB32, false);
            string temp = PlayerPrefs.GetString(nameOfSprites[i]);
            byte[] bytes = System.Convert.FromBase64String(temp);
            text.LoadImage(bytes);

            Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), new Vector2(.5f, .5f));
            sprite.name = nameOfSprites[i].Split('+')[0];

            ResourcesManager.Instance.sprites.Add(sprite.name, sprite);
        }

        GameObjectAgent.Instance.setActive(parts, false);
        GameObjectAgent.Instance.setActive(robot, false);
        GameObjectAgent.Instance.setActive(table, false);
        GameObjectAgent.Instance.setActive(selectedBot, false);
        GUIAgent.Instance.setEnabled("BlueToothGUI", false);
        GUIAgent.Instance.setEnabled("JoyStickGUI", false);
        GUIAgent.Instance.setEnabled("MainGUI", false);

        //블루투스 이부분 적용하면됨 
        #region BlueTooth 세팅
        //스캔 버튼등록
        GUIAgent.Instance.addListener("MainGUI", "TitlePanel/ScanButton", () =>
        {
            //신호가 잡힌 블루투스를 배열할 스크롤컨탠트 위치값 
            GameObject tagetScrollContent = GameObject.Find("ScrollContent");
            //프리팹 설정
            GameObject DeviceButton = Resources.Load<GameObject>("Prefab/Button/DeviceButton");

            BlueToothManager.Instance.Scan(tagetScrollContent,
                                            DeviceButton,
                                            () => GUIAgent.Instance.setEnabled("MainGUI", true));
        });


        GUIAgent.Instance.addListener("MainGUI", "TitlePanel/ScanButton", () =>
        {
            //신호가 잡힌 블루투스를 배열할 스크롤컨탠트 위치값 
            GameObject tagetScrollContent = GameObject.Find("ScrollContent");
            //프리팹 설정
            GameObject DeviceButton = ResourcesManager.Instance.prefabs["DeviceButton"];//Resources.Load<GameObject>("Prefab /Button/DeviceButton");

            BlueToothManager.Instance.Scan(tagetScrollContent,
                                            DeviceButton,
                                            () => GUIAgent.Instance.setEnabled("JoyStickGUI", true));
        });

        /* // 폐기
        //움직임 제어 테스트 등록
        GUIAgent.GetInst.AddListener ( "MainGUI", "Test", () => {
            //GUIAgent.GetInst.CanvasEnabled ( "JoyStickGUI", true );
            GUIAgent.GetInst.Reset ( "CabotTestGUI" );

            //GUIAgent.GetInst.SetInputFieldText ( "JoyStickGUI", "BTPanel", "Time", "500" );
        } );
        */
        //조이스틱 시작등록 
        GUIAgent.Instance.addListener("MainGUI", "Move", () =>
        {
            GUIAgent.Instance.setEnabled("JoyStickGUI", true);
            GUIAgent.Instance.setEnabled("BlueToothGUI", false);
            GUIAgent.Instance.setEnabled("MainGUI", false);
            GameObjectAgent.Instance.getComponent<UnityEngine.UI.InputField>("JoyStickGUI", "BTPanel/Time").text = "500";
        });

        //////백버튼 등록
        GUIAgent.Instance.addListener("JoyStickGUI", "BTPanel/Back", () =>
        {
            GUIAgent.Instance.setEnabled("JoyStickGUI", false);
            GUIAgent.Instance.setEnabled("BlueToothGUI", false);
            GUIAgent.Instance.setEnabled("MainGUI", true); ;
            carBot.StopControlJoyStick();
        });

        //////시작버튼 등록 
        GUIAgent.Instance.addListener("JoyStickGUI", "BTPanel/Start", () =>
        {
            string text = GameObjectAgent.Instance.getComponent<UnityEngine.UI.InputField>("JoyStickGUI", "BTPanel/Time").text;
            int loopTime = System.Int32.Parse(text);
            Debug.Log(loopTime);
            carBot.RunControlJoyStick();
        });

        #endregion

        


    }

    internal void transition()
    {
        StopCoroutine("internalTransition");
        StartCoroutine("internalTransition");
    }
    internal static float Close = 1;
    internal static float Delay = 0.5f;
    internal static float Open = 1;

    IEnumerator internalTransition()
    {
        var top = GUIAgent.Instance.GuiObjects["0_Transition/Top"];
        var bottom = GUIAgent.Instance.GuiObjects["0_Transition/Bottom"];
        iTween.Stop(top);
        iTween.Stop(bottom);
        var posYOfTop = top.GetComponent<RectTransform>().TransformVector(new Vector3(0, 440.5f, 0)).y + transform.GetComponent<RectTransform>().position.y;
        var posYOfBottom = bottom.GetComponent<RectTransform>().TransformVector(new Vector3(0, -452.0f, 0)).y + transform.GetComponent<RectTransform>().position.y;

        iTween.MoveTo(top, iTween.Hash("y", posYOfTop, "time", Close));
        iTween.MoveTo(bottom, iTween.Hash("y", posYOfBottom, "time", Close));

        yield return new WaitForSeconds(Close + Delay);

        posYOfTop = top.GetComponent<RectTransform>().TransformVector(new Vector3(0, 1480, 0)).y + transform.GetComponent<RectTransform>().position.y;
        posYOfBottom = bottom.GetComponent<RectTransform>().TransformVector(new Vector3(0, -1480, 0)).y + transform.GetComponent<RectTransform>().position.y;

        iTween.MoveTo(top, iTween.Hash("y", posYOfTop, "time", Open));
        iTween.MoveTo(bottom, iTween.Hash("y", posYOfBottom, "time", Open));
    }

    internal void changerSprite(GameObject obj)
    {
        StartCoroutine(internalChangeSprite(obj));
    }

    float time = 0.5f;    

    IEnumerator internalChangeSprite(GameObject obj)
    {
        string spriteName = obj.name.CompareTo("Prev") == 0 ? "bt_left" : "bt_right";

        bool selected = obj.GetComponent<UnityEngine.UI.Image>().sprite.name.Contains("select") ? true : false;

        while (obj.activeSelf) {
            if (selected) {
                obj.GetComponent<UnityEngine.UI.Image>().sprite = Robot.Singleton.ResourcesManager.Instance.sprites[spriteName + "_normal"];
                selected = false;
            }
            else {
                obj.GetComponent<UnityEngine.UI.Image>().sprite = Robot.Singleton.ResourcesManager.Instance.sprites[spriteName + "_select"];
                selected = true;
            }

            yield return new WaitForSeconds(time);
        }

        yield break;
    }



}   //class GUIManager


