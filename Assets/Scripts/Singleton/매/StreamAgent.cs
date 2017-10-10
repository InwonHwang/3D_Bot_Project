using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


// 수정하기
namespace Robot.Singleton
{
    public class StreamAgent : Singleton<StreamAgent>
    {

        private StreamAgent() { }

        public void writeFile(string data, string path, string fileName, string format)
        {
            var streamWriter = new StreamWriter(path + "/" + fileName + "." + format);
            streamWriter.Write(data);
            streamWriter.Close();
        }

        public string readFile(string path, string fileName)
        {
            TextAsset data = (TextAsset)Resources.Load(path + "/" + fileName);

            return data.text;
        }
    }
}