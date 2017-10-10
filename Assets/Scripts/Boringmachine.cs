using UnityEngine;
using System.Collections;

public class Boringmachine : MonoBehaviour {

    Animator anim;
    //Level level;
    void Start()
    {
        anim = transform.parent.GetComponent<Animator>();
        //level = GameObject.Find("Level").GetComponent<Level>();
    }

    internal void playAnimation(float speed)
    {
        anim.speed = Constants.Time * speed;
        anim.SetInteger("State", 2);
        Utility.Instance.delayAction(3f / speed, () => gameObject.SetActive(false));
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("hitted");
    //    anim.speed = Constants.Time * 2;
    //    anim.SetInteger("State", 2);
    //    Utility.Instance.delayAction(3f / 2, () => gameObject.SetActive(false));
    //    level.Count--;
    //}    
}
