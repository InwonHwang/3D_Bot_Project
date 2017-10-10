using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CarBot : MonoBehaviour {

    private Scrollbar slider;
    public float MovementSpeed = 10f;

    void Start () {
        //임시
        slider = GameObject.Find ( "SpeedScrollbar" ).GetComponent<Scrollbar> ();
    }

    ////////////////////////////////////////////////////////////////////////////////////
    //Public Method 
    public void RunControlJoyStick () {
        StartCoroutine ( "ControlJoyStick", 1 );
    }

    public void StopControlJoyStick () {
        StopCoroutine ( "ControlJoyStick" );
    }

    public void OneShotPlayProccess ( Transform SubProccess ) {
        Debug.Log(SubProccess.name);
        StartCoroutine ( PlayProccessLoop ( SubProccess ) );
    }

    public void StopPlayProccessLoop () {

    }

    ////////////////////////////////////////////////////////////////////////////////////
    //private Method 
    private IEnumerator ControlJoyStick ( int loopTime ) {

        float R = 0;
        float L = 0;
        //Time , loopTime의 상관 관계 테스트 
        int Time = 100;
        Vector2 movementVector;

        while ( true ) {
            MovementSpeed = slider.value;
            movementVector = new Vector2 (  CnControls.CnInputManager.GetAxis ( "Horizontal" ), 
                                            CnControls.CnInputManager.GetAxis ( "Vertical" ) );

            if ( movementVector.sqrMagnitude < 0.01f ) {


            } else {

                float speed = speedAverger ( MovementSpeed * 255 );

                if ( movementVector.x > 0 && movementVector.y > 0 ) {
                    //1사분면
                    L = speed * movementVector.y;

                    R = speed;

                } else if ( movementVector.x < 0 && movementVector.y > 0 ) {
                    //2사분면
                    L = speed;

                    R = speed * movementVector.y;

                } else if ( movementVector.x < 0 && movementVector.y < 0 ) {
                    //3사분면
                    L = speed * -1;

                    R = speed * movementVector.y;

                } else if ( movementVector.x > 0 && movementVector.y < 0 ) {
                    //4사분면
                    L = speed * movementVector.y;

                    R = speed * -1;

                }
                //Debug.Log ( "x : " + R + "/ y : " + L  + "/ movementVector.y : " + movementVector.y );
                string charApi = "M:" + arangeCheck ( speedMaxCheck ( directionCheck ( R ) ) ) + ":" + arangeCheck ( speedMaxCheck ( directionCheck ( L ) ) ) + ":" + arangeCheck ( Time );
                Debug.Log ( charApi );
                BlueToothManager.Instance.Send ( charApi );

            }

            yield return new WaitForSeconds ( loopTime * 0.01f );

        }
    }

    string arangeCheck ( int data ) {

        int no = data;
        string temp = "";

        if ( no < 0 ) {
            temp = "000";
        } else if ( no < 10 ) {
            temp = "00" + no;
        } else if ( no < 100 ) {
            temp = "0" + no;
        } else if ( no > 999 ) {
            temp = "999";
        } else {
            temp = "" + no;
        }
        return temp;
    }


    float speedAverger ( float data ) {

        float temp = data;

        if ( temp < 50 ) {

            temp = 100f;
        } else if ( temp > 255 ) {
            temp = 255;
        }


        return temp;

    }

    int speedMaxCheck ( float data ) {

        int temp = ( int ) data;

        if ( temp < 0 ) {
            temp = 100;
        } else if ( temp > 510 ) {
            temp = 510;
        }


        return temp;

    }

    float directionCheck ( float data ) {

        float temp = data;

        if ( temp < 0 ) {

            temp = System.Math.Abs ( temp ) + 255;
        }

        return temp;
    }


    IEnumerator PlayProccessLoop (Transform SubProccess) {

        //Debug.Log("ddddd")
        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        //속도
        string no_0 = "";
        if ((no_0 = GameObject.Find("NO (0)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_0 = "250";
        }

        string no_1 = "";
        if ((no_1 = GameObject.Find("NO (1)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_1 = "320";
        }

        string no_2 = "";
        if ((no_2 = GameObject.Find("NO (2)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_2 = "300";
        }


        string no_3 = "";
        if ((no_3 = GameObject.Find("NO (3)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_3 = "128";
        }

        string no_4 = "";
        if ((no_4 = GameObject.Find("NO (4)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_4 = "128";
        }

        string no_5 = "";
        if ((no_5 = GameObject.Find("NO (5)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_5 = "500";
        }


        string no_6 = "";
        if ((no_6 = GameObject.Find("NO (6)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_6 = "001";
        }

        string no_7 = "";
        if ((no_7 = GameObject.Find("NO (7)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_7 = "500";
        }

        string no_8 = "";
        if ((no_8 = GameObject.Find("NO (8)").GetComponent<UnityEngine.UI.InputField>().text) == "")
        {
            no_8 = "100";
        }
        UnityEngine.UI.Text Log = GameObject.Find("Log").GetComponent<UnityEngine.UI.Text>();

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < SubProccess.childCount; i++)
        {

            string api = SubProccess.GetChild(i).name;

            string charApi = "";
            string waitTime = "";
            if (api == "Go")
            {
                charApi = "M:" + no_0 + ":" + no_0 + ":" + no_2;
                waitTime = no_2;

            }
            else if (api == "Left")
            {
                charApi = "M:" + no_0 + ":" + no_1 + ":" + no_2;
                waitTime = no_2;

            }
            else if (api == "Right")
            {
                charApi = "M:" + no_1 + ":" + no_0 + ":" + no_2;
                waitTime = no_2;

            }
            else if (api == "L")
            {
                charApi = "L:" + no_3 + ":" + no_4 + ":" + no_5;
                waitTime = no_5;

            }
            else if (api == "S")
            {
                charApi = "S:" + no_6 + ":" + no_7;
                waitTime = no_7;

            }
            else {
                charApi = "";
                waitTime = "";
            }

            Log.text = charApi;

            Debug.Log("charApi : " + charApi + " / " + "time : " + arangeCheck(System.Int32.Parse(waitTime)));

            if (charApi != "")
            {
                //BtConnector.sendChar(charApi);
                BlueToothManager.Instance.Send(charApi);
                //Debug.Log(""+temp);
                yield return new WaitForSeconds(System.Int32.Parse(waitTime) * 0.01f);

                //BtConnector.sendChar('S');

                //yield return new WaitForSeconds(time);
            }
            else {
                //Debug.Log("else");

                yield return new WaitForSeconds(System.Int32.Parse(waitTime) * 0.01f);
                // yield return new WaitForSeconds( System.Convert.ToSingle ( time ) );
            }
        }
        yield return null;
    }
}
