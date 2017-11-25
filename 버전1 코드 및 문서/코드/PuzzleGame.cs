using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Robot.Singleton;

public class PuzzleGame : MonoBehaviour
{
    struct BaseInfo
    {
        public Transform BaseOfPanel { get; set; }
        public int Index { get; set; }

        public BaseInfo(Transform baseOfPanel, int index)
        {
            BaseOfPanel = baseOfPanel;
            Index = index;
        }
    }

    enum NODETYPE { NOTEXIST, TOP, MID, BOTTOM };

    GraphNode curNode;
    GameObject boxParent;
    GameObject objectsParent;
    GameObject selectedRobot;
    GameObject indicator;
    GameObject orthographicCam;
    List<EntityType.BoxData> listOfEnemy = new List<EntityType.BoxData>();
    Level level;
    float speed = 1.5f;
    public static int stage = 1;
    public static int step = 1;

    void Awake()
    {

        selectedRobot = GameObjectAgent.Instance.findChild("Object", "SelectedRobot");
        boxParent = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Boxes");
        objectsParent = GameObjectAgent.Instance.findChild("Level", "ToBeActivated/Objects");
        level = GameObject.Find("Level").GetComponent<Level>();
        indicator = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Indicator");
        orthographicCam = GameObjectAgent.Instance.findChild("Camera", "Orthographic Camera");
    }

    internal void init()
    {
        //setSpeed();
        iTween.Stop(selectedRobot);
        MoveAgent.Instance.stop();
        StopCoroutine("start");
        listOfEnemy.Clear();        

        if (stage == 10) level.setLevel("0" + step.ToString() + stage.ToString());
        else level.setLevel("0" + step.ToString() + "0" + stage.ToString());

        if (step == 1 && stage == 3) level.Count = 1;

        curNode = level.StartNode;
                
        selectedRobot.transform.position = curNode.BoxData.Position + new Vector3(0, 0.23f, 0);
        selectedRobot.transform.GetChild(0).localScale = new Vector3(0.1f, 0.1f, 0.1f);
        selectedRobot.transform.GetChild(0).localPosition = new Vector3(0, 0, 0f);
        selectedRobot.transform.GetChild(0).localRotation = Quaternion.identity;
        selectedRobot.transform.rotation = Quaternion.identity;
        indicator.transform.position = new Vector3(0, -1500, 0);

        float y = selectedRobot.transform.rotation.y;
        while (findFrontNode(curNode.Neighbour) == null)
        {
            y -= 90;
            selectedRobot.transform.rotation = Quaternion.Euler(0, y, 0);
        }

        if (stage == 10)
        {
            GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top/Image/Number_Zero"].SetActive(true);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Top/Image/Number_Stage", ResourcesManager.Instance.sprites[1.ToString()]);
        }
        else
        {
            GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top/Image/Number_Zero"].SetActive(false);
            GUIAgent.Instance.setSprite("UI", "3_Ingame/Top/Image/Number_Stage", ResourcesManager.Instance.sprites[PuzzleGame.stage.ToString()]);
        }
        GUIAgent.Instance.setSprite("UI", "3_Ingame/Top/Image/Number_Step", ResourcesManager.Instance.sprites[PuzzleGame.step.ToString()]);

        for (int j = 0; j < selectedRobot.transform.GetChild(0).childCount; j++)
            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(j).name, "State", 0);
        AnimationAgent.Instance.Animators[selectedRobot.name].Play("Dummy", 0, 0);

        GUIAgent.Instance.GuiObjects["UI/3_Ingame/PopUp"].GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);
        GUIAgent.Instance.GuiObjects["UI/3_Ingame/PopUp"].SetActive(false);        
    }

    internal void play()
    {
        init();        
        StartCoroutine("start");
    }

    void setSpeed()
    {
        var inputfield = GameObjectAgent.Instance.getComponent<UnityEngine.UI.InputField>("UI", "3_Ingame/InputField");

        if (inputfield.text.Length == 0) return;

        speed = float.Parse(inputfield.text);
    }

    float moveByPanel(string panelName)
    {
        GraphNode node = null;
        NODETYPE type = NODETYPE.NOTEXIST;

        if ((panelName.CompareTo("Go") == 0 || panelName.CompareTo("Jump") == 0 || panelName.CompareTo("Action") == 0))
        {
            node = findFrontNode(curNode.Neighbour);
            if (node != null) type = getNodeType(node);
        }
        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
            AnimationAgent.Instance.Animators[selectedRobot.transform.GetChild(0).GetChild(i).name].speed = Constants.Time * speed;

        float time = AnimationAgent.Instance.Animators[selectedRobot.transform.GetChild(0).GetChild(0).name].GetCurrentAnimatorClipInfo(0)[0].clip.length;

        selectedRobot.transform.position = curNode.BoxData.Position + new Vector3(0, 0.23f, 0);

        switch (panelName)
        {
            case "Go":
                for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                    AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 2);

                time = ResourcesManager.Instance.animationClips["robot_walk"].length - 0.1f;

                if (node != null && !node.Equals(curNode) && type == NODETYPE.MID && node.BoxData.Traversable == true)
                {
                    MoveAgent.Instance.MoveTo(selectedRobot.gameObject, node.BoxData.Position, time /speed);
                    curNode = node;
                }
                break;

            case "Jump":
                if (node != null && node.BoxData.Traversable == true)
                {
                    if (type == NODETYPE.TOP)
                    {
                        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 4);

                        AnimationAgent.Instance.Animators[selectedRobot.name].speed = Constants.Time * speed;
                        AnimationAgent.Instance.Animators[selectedRobot.name].Play("JumpUp", 0, 0);
                        

                        time = ResourcesManager.Instance.animationClips["JumpUp"].length;

                        curNode = node;
                    }
                    else if (type == NODETYPE.BOTTOM)
                    {
                        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 3);

                        AnimationAgent.Instance.Animators[selectedRobot.name].speed = Constants.Time * speed;
                        AnimationAgent.Instance.Animators[selectedRobot.name].Play("JumpDown", 0, 0);

                        time = ResourcesManager.Instance.animationClips["JumpDown"].length;

                        curNode = node;
                    }
                }

                break;
            case "Left":
                for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                    AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 5);

                time = ResourcesManager.Instance.animationClips["robot_turn_L"].length - 0.1f;
                StartCoroutine(MoveAgent.Instance.turnLeft(selectedRobot, time / speed));
                break;
            case "Right":
                for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                    AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 6);

                time = ResourcesManager.Instance.animationClips["robot_turn_R"].length - 0.1f;
                StartCoroutine(MoveAgent.Instance.turnRight(selectedRobot, time / speed));

                break;
            case "Action":
                for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                    AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 0);

                time = ResourcesManager.Instance.animationClips["robot_idle1"].length;

                if (type != NODETYPE.NOTEXIST && node != null && !curNode.BoxData.Name.Contains("warp"))
                {
                    if (!node.BoxData.Name.Contains("pollution"))
                    {
                        GameObject oilTank = null;
                        for (int i = 0; i < objectsParent.transform.childCount; i++)
                        {
                            var child = objectsParent.transform.GetChild(i);
                            if (!child.name.Contains("BoringMachine")) continue;

                            if (node.BoxData.Position + new Vector3(0, 0.55f, 0) == child.position)
                                oilTank = child.gameObject;
                        }

                        if (oilTank != null && oilTank.transform.GetChild(1).gameObject.activeSelf)
                        {
                            Utility.Instance.delayAction(0.5f, () => oilTank.transform.GetChild(1).GetComponent<Boringmachine>().playAnimation(speed));

                            for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                                AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 7);

                            //time = ResourcesManager.Instance.animationClips["robot_attack"].length - 0.1f;
                            time = 1.5f;
                            level.Count--;
                            //curNode = node;
                            node.BoxData.Traversable = true;
                        }
                    }
                    else if (node.BoxData.Name.Contains("pollution") && !listOfEnemy.Contains(node.BoxData))
                    {
                        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 8);

                        time = ResourcesManager.Instance.animationClips["robot_spell2"].length + ResourcesManager.Instance.animationClips["robot_spell2"].length - 0.1f;


                        node.BoxData.Traversable = true;
                        level.Count--;
                        for (int i = 0; i < boxParent.transform.childCount; i++)
                        {
                            var box = boxParent.transform.GetChild(i);
                            if (box.position == node.BoxData.Position)
                            {
                                Utility.Instance.delayAction(time / speed, () =>
                                {
                                    box.GetComponent<Renderer>().material.mainTexture = ResourcesManager.Instance.sprites["box_basic"].texture;
                                });
                            }
                        }
                        listOfEnemy.Add(node.BoxData);
                    }
                }
                else if (curNode.BoxData.Name.Contains("warp"))
                {
                    Debug.Log("warp");

                    for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                        AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 1);

                    time = ResourcesManager.Instance.animationClips["robot_idle1"].length - 0.1f;

                    if (step == 1 && stage == 3) level.Count--;
                    for (int j = 0; j < curNode.Neighbour.Count; j++)
                    {
                        var neighbour = curNode.Neighbour[j];
                        
                        if (curNode.BoxData.Position == neighbour.BoxData.Position || curNode.BoxData.Name.CompareTo(neighbour.BoxData.Name) != 0) continue;

                        Debug.Log(neighbour.BoxData.Position);
                        Debug.Log(neighbour.BoxData.Name);

                        curNode = neighbour;
                    }
                }
                else
                {
                    GameObject oilTank = null;
                    for (int i = 0; i < objectsParent.transform.childCount; i++)
                    {
                        var child = objectsParent.transform.GetChild(i);
                        if (!child.name.Contains("BoringMachine")) continue;

                        int rotationY = (int)Mathf.Round(selectedRobot.transform.localEulerAngles.y / 10) * 10;
                        switch (rotationY)
                        {
                            case 0:
                                if (child.position == selectedRobot.transform.position + new Vector3(0, 0.07f, -0.25f) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0, -0.18f, -0.25f) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0, 0.32f, -0.25f))
                                    oilTank = child.gameObject;
                                break;
                            case 90:
                                if (child.position == selectedRobot.transform.position + new Vector3(-0.25f, 0.07f, 0) ||
                                    child.position == selectedRobot.transform.position + new Vector3(-0.25f, -0.18f, 0) ||
                                    child.position == selectedRobot.transform.position + new Vector3(-0.25f, 0.32f, 0))
                                    oilTank = child.gameObject;
                                break;
                            case 180:
                                if (child.position == selectedRobot.transform.position + new Vector3(0, 0.07f, 0.25f) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0, -0.18f, 0.25f) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0, 0.32f, 0.25f))
                                    oilTank = child.gameObject;
                                break;
                            case 270:
                                if (child.position == selectedRobot.transform.position + new Vector3(0.25f, 0.07f, 0) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0.25f, -0.18f, 0) ||
                                    child.position == selectedRobot.transform.position + new Vector3(0.25f, 0.32f, 0))
                                    oilTank = child.gameObject;
                                break;
                        }

                    }

                    if (oilTank != null && oilTank.transform.GetChild(1).gameObject.activeSelf)
                    {
                        Debug.Log(oilTank.transform.position);
                        Utility.Instance.delayAction(0.5f, () => oilTank.transform.GetChild(1).GetComponent<Boringmachine>().playAnimation(speed));

                        for (int i = 0; i < selectedRobot.transform.GetChild(0).childCount; i++)
                            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(i).name, "State", 7);

                        time = 1.5f;
                        level.Count--;
                    }
                }
                break;
        }

        return time / speed;
    }

    IEnumerator popUp()
    {
        var popUp = GUIAgent.Instance.GuiObjects["UI/3_Ingame/PopUp"];
        popUp.SetActive(true);

        float size = 1;
        while(popUp.GetComponent<RectTransform>().sizeDelta.x < 1400)
        {
            size += 30;
            popUp.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);

            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator start()
    {
        var baseMain = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/Main").transform;
        var baseG1 = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/G1").transform;
        var baseG2 = GameObjectAgent.Instance.findChild("UI", "3_Ingame/Bottom/Base/G2").transform;

        yield return new WaitForSeconds(0.3f);

        Stack<BaseInfo> stackOfBase = new Stack<BaseInfo>();

        var curBase = baseMain;
        for (int i = 0; i < curBase.childCount; i++)
        {
            var panel = curBase.GetChild(i);

            if (panel.name.CompareTo("G1") == 0)
            {
                stackOfBase.Push(new BaseInfo(curBase, i));
                curBase = baseG1;
                i = -1;
                continue;
            }
            if (panel.name.CompareTo("G2") == 0)
            {
                stackOfBase.Push(new BaseInfo(curBase, i));
                curBase = baseG2;
                i = -1;
                continue;
            }

            float time = moveByPanel(panel.name);
            indicator.transform.position = panel.position;

            if (level.Count == 0) // 수정하기
            {
                yield return new WaitForSeconds(time);

                StartCoroutine(popUp());
                for (int j = 0; j < selectedRobot.transform.GetChild(0).childCount; j++)
                    AnimationAgent.Instance.Animators[selectedRobot.transform.GetChild(0).GetChild(j).name].speed = Constants.Time;

                for (int j = 0; j < selectedRobot.transform.GetChild(0).childCount; j++)
                    AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(j).name, "State", 0);

                indicator.transform.position = new Vector3(0, -1500, 0);

                yield return new WaitForSeconds(3.3f);                

                GameObjectAgent.Instance.getComponent<GameManager>("UI").transition();
                stackOfBase.Clear();

                GameObjectAgent.Instance.delaySetActive("Light", "Base", true, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive("Light", "Ingame", false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Top"], false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Bottom"], false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear"], true, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Clear"], true, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(GUIAgent.Instance.GuiObjects["Background/3_Ingame/Bg"], false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(level.gameObject, false, GameManager.Close + GameManager.Delay);
                GameObjectAgent.Instance.delaySetActive(gameObject, false, GameManager.Close + GameManager.Delay);

                Utility.Instance.delayAction(GameManager.Close + GameManager.Delay, () =>
                {
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear/Top/Title/Prev"]);
                    GameObjectAgent.Instance.getComponent<GameManager>("UI").changerSprite(GUIAgent.Instance.GuiObjects["UI/3_Ingame/Clear/Top/Title/Next"]);
                    selectedRobot.transform.GetChild(0).localScale = Vector3.one;
                    selectedRobot.transform.rotation = Quaternion.identity;
                    selectedRobot.transform.position = Vector3.zero;
                    GameObjectAgent.Instance.getComponent<Camera>(orthographicCam).enabled = false;
                    GameObjectAgent.Instance.getComponent<Camera>("Camera", "Main Camera").enabled = true;
                    GUIAgent.Instance.GuiObjects["UI/3_Ingame/PopUp"].SetActive(false);
                });

                stage++;

                yield break;
            }

            while (i == curBase.childCount - 1 && stackOfBase.Count > 0)
            {   
                var baseInfo = stackOfBase.Pop();
                curBase = baseInfo.BaseOfPanel;
                i = baseInfo.Index;                        
            }

            
            yield return new WaitForSeconds(time);

            if (curNode.BoxData.Name.Contains("warp") && panel.name.CompareTo("Action") == 0)
            {
                selectedRobot.transform.position = curNode.BoxData.Position + new Vector3(0, 0.23f, 0);
                //selectedRobot.transform.rotation = Quaternion.identity;
                float y = selectedRobot.transform.rotation.y;

                while (curNode.Neighbour.Count != 1 && findFrontNode(curNode.Neighbour) == null)
                {
                    y -= 90;
                    selectedRobot.transform.rotation = Quaternion.Euler(0, y, 0);
                }
            }

            AnimationAgent.Instance.Animators[selectedRobot.name].Play("Dummy", 0, 0);

            //if (i == curBase.childCount - 1 && stackOfBase.Count == 0)
            //{
            //    for (int j = 0; j < selectedRobot.transform.GetChild(0).childCount; j++)
            //        AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(j).name, "State", 0);
                                
            //}        
        }
        indicator.transform.position = new Vector3(0, -1500, 0);
        stackOfBase.Clear();
        
        for (int j = 0; j < selectedRobot.transform.GetChild(0).childCount; j++)
            AnimationAgent.Instance.setInteger(selectedRobot.transform.GetChild(0).GetChild(j).name, "State", 0);
    }
    
    NODETYPE getNodeType(GraphNode data)
    {
        if (data == null) return NODETYPE.NOTEXIST;

        float posY = curNode.BoxData.Position.y;

        if (data.BoxData.Position.y > posY)
            return NODETYPE.TOP;
        else if (data.BoxData.Position.y < posY)
            return NODETYPE.BOTTOM;

        return NODETYPE.MID;
    }

    //GraphNode findFrontNode(List<GraphNode> list)
    //{
    //    int rotationY = (int)Mathf.Round(selectedRobot.transform.localEulerAngles.y);
    //    selectedRobot.transform.rotation = Quaternion.Euler(0, rotationY, 0);
    //    var position = curNode.BoxData.Position;

    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        switch (rotationY)
    //        {
    //            case 0:
    //                if (position.z - 1 == list[i].BoxData.Position.z)
    //                    return list[i];                    
    //                break;
    //            case 90:
    //                if (position.x - 1 == list[i].BoxData.Position.x)
    //                    return list[i];                    
    //                break;
    //            case 180:
    //                if (position.z + 1 == list[i].BoxData.Position.z)
    //                    return list[i];                    
    //                break;
    //            case 270:
    //                if (position.x + 1 == list[i].BoxData.Position.x)                    
    //                    return list[i];                    
    //                break;
    //        }
    //    }

    //    return null;
    //}

    GraphNode findFrontNode(List<GraphNode> list)
    {
        int rotationY = (int)Mathf.Round(selectedRobot.transform.localEulerAngles.y / 10) * 10;
        var position = curNode.BoxData.Position;
        for (int i = 0; i < list.Count; i++)
        {
            switch (rotationY)
            {
                case 0:
                    if (position.z - 0.25f == list[i].BoxData.Position.z)
                        return list[i];
                    break;
                case 90:
                    if (position.x - 0.25f == list[i].BoxData.Position.x)
                        return list[i];
                    break;
                case 180:
                    if (position.z + 0.25f == list[i].BoxData.Position.z)
                        return list[i];
                    break;
                case 270:
                    if (position.x + 0.25f == list[i].BoxData.Position.x)
                        return list[i];
                    break;
            }
        }

        return null;
    }
}