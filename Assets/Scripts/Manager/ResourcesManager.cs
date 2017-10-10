using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Robot.Singleton
{
    public class ResourcesManager : Singleton<ResourcesManager>
    {
        public Dictionary<string, Sprite> sprites { get; private set; }
        public Dictionary<string, GameObject> prefabs { get; private set; }
        public Dictionary<string, AnimationClip> animationClips { get; private set; }

        private ResourcesManager()
        {
            sprites = new Dictionary<string, Sprite>();
            prefabs = new Dictionary<string, GameObject>();
            animationClips = new Dictionary<string, AnimationClip>();
            setDictionary<Sprite>(sprites, "Images");
            setDictionary<GameObject>(prefabs, "Prefabs");
            setDictionary<AnimationClip>(animationClips, "Animations");
        }

        private void setDictionary<T>(Dictionary<string, T> dictionary,string path) where T : Object
        {
            var sprites = Resources.LoadAll<T>(path) as T[];
            if (sprites == null) return;

            for (int i = 0; i < sprites.Length; i++)
            {
                if (dictionary.ContainsKey(sprites[i].name)) continue;

                dictionary.Add(sprites[i].name, sprites[i]);
            }
        }

        public int getCount<T>(Dictionary<string, T> dictionary, string contains, string trim = null) where T : Object
        {
            int retValue = 0;

            foreach (var key in dictionary.Keys)
            {
                if ((key.Contains(contains) && trim == null) ||

                    (key.Contains(contains) && !key.Contains(trim)))
                    retValue++;
            }

            return retValue;
        }

        public T[] getValues<T>(Dictionary<string, T> dictionary, string contains, string trim = null) where T : Object
        {
            int count = getCount<T>(dictionary, contains, trim);
            var retValue = new T[count];
            int i = 0;

            foreach (var prefab in dictionary.Values)
            {
                if ((prefab.name.Contains(contains) && trim == null) ||
                   (prefab.name.Contains(contains) && !prefab.name.Contains(trim)))
                {
                    retValue[i] = prefab;
                    i++;
                }
            }

            return retValue;
        }        
    }

    //public class SpriteManager : Singleton<SpriteManager>
    //{
    //    public Dictionary<string, Sprite> Dictionary { get; private set; }
    //    public string Path { get; private set; }

    //    private SpriteManager()
    //    {
    //        Dictionary = new Dictionary<string, Sprite>();
    //        Path = "Images/";
    //        setDictionary();
    //    }

    //    public void setDictionary()
    //    {
    //        var sprites = Resources.LoadAll<Sprite>(Path) as Sprite[];
    //        if (sprites == null) return;

    //        for (int i = 0; i < sprites.Length; i++)
    //        {
    //            if (Dictionary.ContainsKey(sprites[i].name)) continue;

    //            Dictionary.Add(sprites[i].name, sprites[i]);
    //        }
    //    }



    //    public int getCount(string contains, string trim = null)
    //    {
    //        int retValue = 0;

    //        foreach (var key in Dictionary.Keys)
    //        {
    //            if ((key.Contains(contains) && trim == null) ||

    //                (key.Contains(contains) && !key.Contains(trim)))
    //                retValue++;
    //        }

    //        return retValue;
    //    }

    //    public Sprite getValue(string key)
    //    {
    //        Sprite retValue;

    //        if (Dictionary.TryGetValue(key, out retValue) == false) return null;

    //        return retValue;
    //    }

    //    public Sprite[] getValues(string contains, string trim = null)
    //    {
    //        int count = getCount(contains, trim);
    //        var retValue = new Sprite[count];
    //        int i = 0;

    //        foreach (var prefab in Dictionary.Values)
    //        {
    //            if ((prefab.name.Contains(contains) && trim == null) ||
    //               (prefab.name.Contains(contains) && !prefab.name.Contains(trim)))
    //            {
    //                retValue[i] = prefab;
    //                i++;
    //            }
    //        }

    //        return retValue;
    //    }
    //} // class SpriteManager

    //public class PrefabManager : Singleton<PrefabManager>
    //{
    //    public Dictionary<string, GameObject> Dictionary { get; private set; }
    //    public string Path { get; private set; }

    //    private PrefabManager()
    //    {
    //        Dictionary = new Dictionary<string, GameObject>();
    //        Path = "Prefabs/";
    //        setDictionary();
    //    }

    //    public void setDictionary()
    //    {
    //        var prefabs = Resources.LoadAll<GameObject>(Path) as GameObject[];
    //        if (prefabs == null) return;

    //        for (int i = 0; i < prefabs.Length; i++)
    //        {
    //            if (Dictionary.ContainsKey(prefabs[i].name)) continue;

    //            Dictionary.Add(prefabs[i].name, prefabs[i]);
    //        }
    //    }

    //    public int getCount(string contains, string trim = null)
    //    {
    //        int retValue = 0;

    //        foreach (var prefab in Dictionary.Values)
    //        {
    //            if ((prefab.name.Contains(contains) && trim == null) ||

    //                (prefab.name.Contains(contains) && !prefab.name.Contains(trim)))
    //                retValue++;
    //        }

    //        return retValue;
    //    }

    //    public GameObject getValue(string key)
    //    {
    //        GameObject retValue;

    //        if (Dictionary.TryGetValue(key, out retValue) == false) return null;

    //        return retValue;
    //    }

    //    public GameObject[] getValues(string contains, string trim = null)
    //    {
    //        int count = getCount(contains, trim);
    //        var retValue = new GameObject[count];
    //        int i = 0;

    //        foreach (var prefab in Dictionary.Values)
    //        {
    //            if ((prefab.name.Contains(contains) && trim == null) ||
    //               (prefab.name.Contains(contains) && !prefab.name.Contains(trim)))
    //            {
    //                retValue[i] = prefab;
    //                i++;
    //            }
    //        }

    //        return retValue;
    //    }
    //} // class PrefabManager


} // namespace Robot.Singleton