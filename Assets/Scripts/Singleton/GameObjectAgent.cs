using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Robot.Singleton
{
    public class GameObjectAgent : Singleton<GameObjectAgent>
    {
        #region public method

        public GameObject findChild(string parent, string path)
        {
            return internalFindChild(parent, path);
        }

        /////////////////////////////////////////////////////////////////////////////////

        public T addComponent<T>(string parent, string path = null) where T : Component
        {
            var guiObject = internalFindChild(parent, path);

            return internalAddComponent<T>(guiObject);
        }

        public T getComponent<T>(string parent, string path = null) where T : Component
        {
            var guiObject = internalFindChild(parent, path);

            return internalGetComponent<T>(guiObject);
        }

        public void setActive(string parent, string path, bool value)
        {
            var guiObject = internalFindChild(parent, path);

            internalSetActive(guiObject, value);
        }
        
        public void setEnabled(string parent, string path, bool value)
        {
            var guiObject = internalFindChild(parent, path);

            internalSetEnabled(guiObject, value);
        }

        //수정
        public void delaySetActive(string parent, string path, bool value, float time)
        {
            var guiObject = internalFindChild(parent, path);

            delaySetActive(guiObject, value, time);
        }

        //수정
        public void delaySetEnabled(string parent, string path, bool value, float time)
        {
            var guiObject = internalFindChild(parent, path);

            delaySetEnabled(guiObject, value, time);
        }

        ////////////////////////////////////////////////////////////////////////////////////////        

        public T addComponent<T>(GameObject guiObject) where T : Component
        {
            return internalAddComponent<T>(guiObject);
        }

        public T getComponent<T>(GameObject guiObject) where T : Component
        {
            return internalGetComponent<T>(guiObject);
        }

        public void setActive(GameObject guiObject, bool value)
        {
            internalSetActive(guiObject, value);
        }

        public void setEnabled(GameObject guiObject, bool value)
        {
            internalSetEnabled(guiObject, value);
        }

        public void delaySetActive(GameObject guiObject, bool value, float time = 0)
        {
            StartCoroutine(internalDelaySetActive(guiObject, value, time));
        }

        public void delaySetEnabled(GameObject guiObject, bool value, float time = 0)
        {
            StartCoroutine(internalDelaySetEnabled(guiObject, value, time));
        }

        #endregion public method

        #region private method

        GameObject internalFindChild(string parent, string path)
        {
            if (parent == null)
            {
                Debug.LogError("error, null reference");
                return null;
            }
            if (path == null)
                return GameObject.Find(parent);

            var retValue = GameObject.Find(parent);

            if (!retValue)
            {
                Debug.LogError("error, null reference: " + parent);
                return null;
            }
            retValue = retValue.transform.FindChild(path).gameObject;
            

            if (!retValue)
            {
                Debug.Log(parent);
                Debug.LogError("error, null reference: " + path);
                return null;
            }

            return retValue;
        }       

        ////////////////////////////////////////////////////////////////////////////////////////        

        T internalAddComponent<T>(GameObject guiObject) where T : Component
        {
            if (!guiObject) return default(T);

            if (guiObject.GetComponent<T>()) return guiObject.GetComponent<T>();

            return guiObject.gameObject.AddComponent<T>();
        }

        T internalGetComponent<T>(GameObject guiObject) where T : Component
        {
            if (!guiObject) return default(T);

            return guiObject.GetComponent<T>();
        }

        void internalSetActive(GameObject guiObject, bool value)
        {
            
            if (guiObject) guiObject.SetActive(value);
        }

        void internalSetEnabled(GameObject guiObject, bool value)
        {
            if (guiObject == null || guiObject.GetComponent<Renderer>() == null) return;

            guiObject.GetComponent<Renderer>().enabled = value;
        }

        IEnumerator internalDelaySetEnabled(GameObject guiObject, bool value, float time = 0)
        {
            yield return new WaitForSeconds(time);

            if (guiObject == null || guiObject.GetComponent<Renderer>() == null) yield break;

            guiObject.GetComponent<Renderer>().enabled = value;
        }

        IEnumerator internalDelaySetActive(GameObject guiObject, bool value, float time = 0)
        {
            yield return new WaitForSeconds(time);

            
            if (guiObject)
            {                
                guiObject.SetActive(value);
            }
        }

        #endregion

    }  // class Gameobject Agent

} // namespace GameObject
