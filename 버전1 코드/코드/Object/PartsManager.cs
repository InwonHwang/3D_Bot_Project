using UnityEngine;
using System.Collections;

using Robot.Singleton;

//어떻게 바꿀지 생각하기
//public을 쓸바엔 internal로 바꾸자
//나중에 ObjectManager에서 처리하기
public class PartsManager : MonoBehaviour //ObjectResourceManager
{
    readonly string[] nameOfParts = { "head", "body", "arm_left", "arm_right", "leg_left", "leg_right" };
    enum State : int { IDLE1 = 0, IDLE2, WALK, JUMP1, JUMP2, TURNLEFT, TURNRIGHT, ATTACK, SPELL, BASE };

    private GameObject[] parts;
    private EntityType.BotInfo botInfo;

    public GameObject[] ActiveParts { get; private set; }

    public EntityType.BotInfo BotInfo
    {
        get
        {
            if (botInfo == null)
                BotInfo = new EntityType.BotInfo(gameObject);
            else
                botInfo.set(gameObject);

            return botInfo;
        }
        set
        {
            botInfo = value;
        }
    }

    void Awake()
    {        
        AnimationAgent.Instance.registerAnimator(gameObject.name, gameObject.GetComponent<Animator>());
        ActiveParts = new GameObject[6];
        loadParts();
        matchJoint();
    }

    #region manage parts

    private void loadParts()
    {       
        var prefabs = ResourcesManager.Instance.getValues<GameObject>(ResourcesManager.Instance.prefabs, "bot");

        parts = new GameObject[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++)
        {
            var parent = transform.FindChild(prefabs[i].name);
            for (int j = 0; j < 10; j++)
            {
                parts[i] = Instantiate(prefabs[i]) as GameObject;
                parts[i].transform.position = new Vector3(0f, 0.3f, 0f);
                parts[i].transform.SetParent(parent);
                parts[i].name = j.ToString() + "_" + prefabs[i].name;

                var rendered = parts[i].transform.GetChild(1).gameObject;
                AnimationAgent.Instance.registerAnimator(parts[i].name, parts[i].GetComponent<Animator>());
                HighlightAgent.Instance.registerHighlighter(parts[i].name, rendered.GetComponent<HighlightingSystem.Highlighter>());

                if (parts[i].name.Contains("black") && j == 0)                
                    activate(parent.name);                
                else
                    parts[i].SetActive(false);
            }
        }
    }

    public void matchJoint()
    {
        for(int i = 0; i < ActiveParts.Length; i++)
        {
            if (ActiveParts[i] == null) return;
        }

        Vector3 gap;
        var jointOfBody = ActiveParts[(int)Constants.PART.BODY].transform.FindChild("Joint_Head");
        gap = jointOfBody.transform.position - ActiveParts[(int)Constants.PART.HEAD].transform.FindChild("Joint_Head").position;
        ActiveParts[(int)Constants.PART.HEAD].transform.position += gap;

        jointOfBody = ActiveParts[(int)Constants.PART.BODY].transform.FindChild("Joint_LArm");
        gap = jointOfBody.transform.position - ActiveParts[(int)Constants.PART.ARM_LEFT].transform.FindChild("Joint_LArm").position;
        ActiveParts[(int)Constants.PART.ARM_LEFT].transform.position += gap;

        jointOfBody = ActiveParts[(int)Constants.PART.BODY].transform.FindChild("Joint_RArm");
        gap = jointOfBody.transform.position - ActiveParts[(int)Constants.PART.ARM_RIGHT].transform.FindChild("Joint_RArm").position;
        ActiveParts[(int)Constants.PART.ARM_RIGHT].transform.position += gap;

        jointOfBody = ActiveParts[(int)Constants.PART.BODY].transform.FindChild("Joint_LFoot");
        gap = jointOfBody.transform.position - ActiveParts[(int)Constants.PART.LEG_LEFT].transform.FindChild("Joint_LFoot").position;
        ActiveParts[(int)Constants.PART.LEG_LEFT].transform.position += gap;

        jointOfBody = ActiveParts[(int)Constants.PART.BODY].transform.FindChild("Joint_RFoot");
        gap = jointOfBody.transform.position - ActiveParts[(int)Constants.PART.LEG_RIGHT].transform.FindChild("Joint_RFoot").position;
        ActiveParts[(int)Constants.PART.LEG_RIGHT].transform.position += gap;
    }

    public GameObject activate(string partName)
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            var parentOfPart = transform.GetChild(i);

            if (!partName.Contains(parentOfPart.name)) continue;

            for (int j = 0; j < nameOfParts.Length; j++)
            {
                if (partName.Contains(nameOfParts[j]))
                {
                    if (ActiveParts[j]) ActiveParts[j].SetActive(false);

                    var newPart = parentOfPart.GetChild(0).gameObject;
                    ActiveParts[j] = newPart;
                    newPart.SetActive(true);
                    return newPart;
                }
            }
        }

        return null;
    }

         
    
    #endregion manage parts
}
