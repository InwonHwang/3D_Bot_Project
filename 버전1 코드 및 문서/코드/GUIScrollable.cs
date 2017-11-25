using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

using Robot.Singleton;

// ArrayList 를 사용하기 편하게 만들자
//public void createButton() 추가하기
//public void addButton() 추가하기
//storage  들어갈때마다 버튼을 생성하는 거 변경하기

namespace Robot.GUI
{
    internal class GUIScrollable : MonoBehaviour
    {
        [SerializeField]
        float Sensitibity = 0.5f;
        [SerializeField]
        float Deceleration = 0.8f;

        Vector2 curPos;
        float startPos;
        float endPos;
        float startTime;
        float endTime;
        bool drag;

        RectTransform panel;
        ArrayList buttons = new ArrayList();

        float[] distance;
        float buttonWidth;        

        internal UnityEngine.Events.UnityAction action;
        internal string MinButtonName
        {
            get
            {
                var button = buttons[MinButtonNum] as GameObject;

                return button.name;
            }
            set
            {
                var name = value;
                for (int i = 0; i < buttons.Count; i++)
                {
                    var button = buttons[i] as GameObject;
                    if (button.name.CompareTo(name) == 0)
                        MinButtonNum = i;
                }
            }
        }

        internal int MinButtonNum { get; set; } 

        void Update()
        {
            if (buttons.Count == 0) return;

            actionOnCenter();

            if( panel.anchoredPosition.x > 0 || panel.anchoredPosition.x < -(buttonWidth * (buttons.Count - 1f)) * 1.15f)
            {
                MinButtonNum = getIndexOfClosestButton();

                var button = buttons[MinButtonNum] as GameObject;

                float position = -button.transform.localPosition.x;
                float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 3f);
                panel.anchoredPosition = new Vector2(newX, panel.anchoredPosition.y);
            }
        }

        internal void init(string contains, string trim = null)
        {
            //EventTriggerAgent.Instance.addEvent(gameObject, EventTriggerType.EndDrag, () => StartCoroutine(CLerpTo(MinButtonNum)) );
            //EventTriggerAgent.Instance.addEvent(gameObject, EventTriggerType.PointerDown, StopAllCoroutines);            

            panel = transform.FindChild("Panel").GetComponent<RectTransform>();

            buttonWidth = 238;
            createButtons(contains, trim);
            distance = new float[buttons.Count];            
        }

        void createButtons(string subImageNameContains, string trim = null)
        {
            buttons.Clear();

            var prefab = ResourcesManager.Instance.prefabs["ScrollPanel"];
            var selectedImage = ResourcesManager.Instance.sprites["line_select"];
            var image = ResourcesManager.Instance.getValues<Sprite>(ResourcesManager.Instance.sprites, subImageNameContains, trim);

            if (prefab == null || selectedImage == null || image == null) return;

            for (int i = 0; i < image.Length; i++)
            {
                var button = Instantiate(prefab) as GameObject;
                button.transform.position = new Vector3(buttonWidth * i * 1.15f, 0, 0);
                if( i == 0 ) button.transform.localScale = Vector2.one;
                else button.transform.localScale = new Vector2(1.1f, 1.1f);
                button.transform.SetParent(transform.FindChild("Panel"), false);
                button.name = image[i].name;

                var selected = button.transform.FindChild("Selected").gameObject;
                var subImage = button.transform.FindChild("Image").gameObject;

                int index = i;

                EventTriggerAgent.Instance.addEvent(subImage, EventTriggerType.PointerDown, () =>
                {
                    iTween.Stop();
                    StopAllCoroutines();
                });

                EventTriggerAgent.Instance.addEvent(subImage, EventTriggerType.BeginDrag, () => 
                {                    
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        startPos = transform.InverseTransformPoint(hit.point).x;
                        startTime = Time.time;
                        curPos = panel.anchoredPosition;
                    }
                });
                EventTriggerAgent.Instance.addEvent(subImage, EventTriggerType.Drag, () =>
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity) && transform.InverseTransformPoint(hit.point).y < -130)
                    {
                        endPos = transform.InverseTransformPoint(hit.point).x;
                        endTime = Time.time;
                        float deltaX = (endPos - startPos) * Sensitibity;
                        panel.anchoredPosition = curPos + new Vector2(deltaX, 0);
                    }
                    else
                    {
                        drag = true;
                        StartCoroutine(CLerpTo(MinButtonNum));
                    }
                });
                EventTriggerAgent.Instance.addEvent(subImage, EventTriggerType.PointerUp, () =>
                {
                    if (endPos - startPos == 0 || drag)
                    {
                        drag = false;
                        MinButtonNum = index;
                        StartCoroutine(CLerpTo(MinButtonNum));
                        return;
                    }

                    float breakingTime = Mathf.Abs((endPos - startPos) * Sensitibity / ((endTime - startTime) * 2500));
                    float breakingDistance = (endPos - startPos) * Sensitibity / ((endTime - startTime) * 2500 * Deceleration);

                    iTween.MoveAdd(panel.gameObject, iTween.Hash("x", breakingDistance, "time", breakingTime));
                    startPos = 0;
                    endPos = 0;
                    StartCoroutine(CLerpToCenter());
                });

                EventTriggerAgent.Instance.addEvent(button, EventTriggerType.PointerDown, () =>
                {
                    iTween.Stop();
                    StopAllCoroutines();
                });

                EventTriggerAgent.Instance.addEvent(button, EventTriggerType.BeginDrag, () =>
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        startPos = transform.InverseTransformPoint(hit.point).x;
                        startTime = Time.time;
                        curPos = panel.anchoredPosition;
                    }
                });
                EventTriggerAgent.Instance.addEvent(button, EventTriggerType.Drag, () =>
                {

                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity) && transform.InverseTransformPoint(hit.point).y < -130)
                    {
                        endPos = transform.InverseTransformPoint(hit.point).x;
                        endTime = Time.time;
                        float deltaX = (endPos - startPos) * Sensitibity;
                        panel.anchoredPosition = curPos + new Vector2(deltaX, 0);
                    }
                    else
                    {                        
                        drag = true;
                        StartCoroutine(CLerpTo(MinButtonNum));
                    }
                });
                EventTriggerAgent.Instance.addEvent(button, EventTriggerType.PointerUp, () =>
                {
                    if (endPos - startPos == 0 || drag)
                    {
                        drag = false;
                        MinButtonNum = index;
                        StartCoroutine(CLerpTo(MinButtonNum));
                        return;
                    }

                    float breakingTime = Mathf.Abs((endPos - startPos) * Sensitibity / ((endTime - startTime) * 2500));
                    float breakingDistance = (endPos - startPos) * Sensitibity / ((endTime - startTime) * 2500 * Deceleration);

                    iTween.MoveAdd(panel.gameObject, iTween.Hash("x", breakingDistance, "time", breakingTime));
                    startPos = 0;
                    endPos = 0;
                    StartCoroutine(CLerpToCenter());
                });

                selected.GetComponent<Image>().sprite = selectedImage;
                subImage.GetComponent<Image>().sprite = image[i];
                selected.GetComponent<Image>().GetComponent<Image>().SetNativeSize();
                subImage.GetComponent<Image>().SetNativeSize();
                buttons.Add(button);
            }
        }

        void actionOnCenter()
        {
            int index = getIndexOfClosestButton();

            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i] as GameObject;
                if (index == i)
                {
                    button.transform.localScale = Vector2.one;
                    button.transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    button.transform.localScale = new Vector2(1.1f, 1.1f);
                    button.transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            if (action != null) action();
        }

        int getIndexOfClosestButton()
        {
            if (buttons == null || buttons.Count == 0) return 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i] as GameObject;
                distance[i] = Mathf.Abs(-button.GetComponent<RectTransform>().position.x);
            }

            float minDistance = Mathf.Min(distance);

            for (int i = 0; i < buttons.Count; i++)
            {
                if (minDistance == distance[i])
                    return i;
            }

            return MinButtonNum;
        }

        IEnumerator CLerpTo(int index)
        {
            if (buttons.Count == 0) yield break;

            MinButtonNum = index;

            while (true)
            {
                var button = buttons[MinButtonNum] as GameObject;                

                float position = -button.transform.localPosition.x;
                float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 5f);
                panel.anchoredPosition = new Vector2(newX, panel.anchoredPosition.y);

                yield return null;
            }
        }

        IEnumerator CLerpToCenter()
        {
            if (buttons.Count == 0) yield break;

            float prevPos = 0;
            float curPos = panel.anchoredPosition.x;      
            
            while( Mathf.Abs(curPos - prevPos) > 0.00001f )
            {
                prevPos = curPos;
                curPos = panel.anchoredPosition.x;             
                yield return new WaitForSeconds(1f);
            }

            iTween.Stop();

            while (true)
            {
                MinButtonNum = getIndexOfClosestButton();

                var button = buttons[MinButtonNum] as GameObject;               

                float position = -button.transform.localPosition.x;
                float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 3f);
                panel.anchoredPosition = new Vector2(newX, panel.anchoredPosition.y);

                yield return null;
            }
        }

        internal void matchButtonToCenter(int index)
        {
            if (index > buttons.Count || buttons.Count == 0) return;

            StopAllCoroutines();
            MinButtonNum = index;
            var button = buttons[MinButtonNum] as GameObject;            
            panel.anchoredPosition = new Vector2(-button.transform.localPosition.x, panel.anchoredPosition.y);
        }

        internal void matchButtonToCenter(string name)
        {            
            for (int i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i] as GameObject;
                if(button.name.CompareTo(name) == 0)
                {
                    StopAllCoroutines();
                    MinButtonNum = i;                    
                    panel.anchoredPosition = new Vector2(-button.transform.localPosition.x, panel.anchoredPosition.y);
                }
            }
        }
    }   //class GUIScrollable

}   //namespace GUI