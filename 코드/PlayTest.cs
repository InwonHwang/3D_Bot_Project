using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleMaker
{
    public class PlayTest : MonoBehaviour
    {
        PlaySystem playSystem;

        void Awake()
        {
            Time.timeScale = 1.5f;
            
            PuzzleManager.Instance.LoadPuzzleFromFile("test");

            playSystem = PlaySystem.Instance;            

            GameObject.Find("Canvas/Play").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
            {
                playSystem.Play();
            });            
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                playSystem.PlayBackOneStep();
                //PuzzleManager.Instance.ActivePuzzle.WriteData();                
            }

            if(Input.GetKeyUp(KeyCode.Z))
            {
                Debug.Log("Play");
                playSystem.Play();
            }

            if (Input.GetKeyUp(KeyCode.X))
            {
                Debug.Log("Pause");
                playSystem.Pause();
            }

            if (Input.GetKeyUp(KeyCode.C))
            {
                Debug.Log("Revert");
                playSystem.PlayBackOneStep();
            }

        }

        private void OnApplicationQuit()
        {
            PuzzlePlayLogSystem.Instance.FileName = "executed.json";
            PuzzlePlayLogSystem.Instance.Clear();

            PuzzlePlayLogSystem.Instance.FileName = "reverted.json";
            PuzzlePlayLogSystem.Instance.Clear();
        }
    }
}