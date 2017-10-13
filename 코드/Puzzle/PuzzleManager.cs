using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace PuzzleMaker
{
    public class PuzzleManager : Singleton<PuzzleManager>
    {
        //public Puzzle ActivePuzzle { get; private set; }
        public Puzzle ActivePuzzle { get; set; }

        private Dictionary<string, Puzzle> puzzles = new Dictionary<string, Puzzle>();

        private void Awake()
        {                        
        }

        public void SetActivePuzzle(string guid)
        {            
        }

        public string LoadPuzzleFromUrl(string url)
        {
            string guid = null;

            ObservableWWW.GetWWW(url)
                 .Select(www => www.text)
                 .Subscribe(data => {
                     guid = loadPuzzleFromData(data);
                 });

            return guid;
        }

        public void SavePuzzleToUrl(string url, string guid)
        {
            if (!puzzles.ContainsKey(guid)) return;

            var data = puzzles[guid].Serialize();

            ObservableWWW.PostWWW(url, System.Convert.FromBase64String(data));
        }

        public string LoadPuzzleFromFile(string path)
        {
            var new_puzzle = new Puzzle();

            var text_asset = Resources.Load<TextAsset>(path);
            new_puzzle.Deserialize(text_asset.text);

            puzzles.Add("", new_puzzle);

            ActivePuzzle = new_puzzle;

            return null;
        }

        private string loadPuzzleFromData(string data)
        {            
            string guid = "";

            var new_puzzle = new Puzzle();
            new_puzzle.Deserialize(data);

            puzzles.Add(guid, new_puzzle);

            return guid;
        }
    }
}