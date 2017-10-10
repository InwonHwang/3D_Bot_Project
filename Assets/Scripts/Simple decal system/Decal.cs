using UnityEngine;
using System.Collections.Generic;

//sticker관련 된것도 GUI에만 적용되기 때문에 internal로 바꾸기
//더 좋게 구현하는것을 생각해보기

namespace DecalSystem
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Decal : MonoBehaviour
    {
        private EntityType.StickerInfo info;
        private Mesh mesh;

        public LayerMask affectedLayers = -1;
        public Material material;
        public Sprite sprite;
        public float maxAngle = 90f;
        public float pushDistance = 0.1f;

        public EntityType.StickerInfo Info
        {
            get
            {
                if (info == null)
                {
                    mesh = GetComponent<MeshFilter>().mesh;
                    info = new EntityType.StickerInfo(gameObject, mesh, sprite);
                }
                else
                    info.set(gameObject, mesh, sprite);

                return info;
            }
            set
            {
                info = value;
            }
        }

        void Awake()
        {
            mesh = GetComponent<MeshFilter>().mesh;
        }

        public Bounds GetBounds()
        {
            Vector3 size = transform.lossyScale;
            Vector3 min = -size / 2f;
            Vector3 max = size / 2f;


            Vector3[] vts = new Vector3[] {
            new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z),
            new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, max.y, max.z), new Vector3(max.x, max.y, max.z),
        };

            for (int i = 0; i < 8; i++)
                vts[i] = transform.TransformDirection(vts[i]);

            min = max = vts[0];
            foreach (Vector3 v in vts)
            {
                min = Vector3.Min(min, v);
                max = Vector3.Max(max, v);
            }
            return new Bounds(transform.position, max - min);
        }

        public void createMesh()
        {
            for (int i = 0; i < this.info.BufNormals.Count; i++)
            {
                DecalBuilder.bufVertices.Add(this.info.BufVertices[i]);
                DecalBuilder.bufNormals.Add(this.info.BufNormals[i]);
            }

            for (int i = 0; i < this.info.BufIndices.Count; i++)
            {
                DecalBuilder.bufIndices.Add(this.info.BufIndices[i]);
            }

            DecalBuilder.GenerateTexCoords(0, this.sprite);

            MeshFilter filter = gameObject.GetComponent<MeshFilter>();
            gameObject.GetComponent<Renderer>().material = this.material;

            filter.mesh = DecalBuilder.CreateMesh();

            transform.position = this.info.Position;
            transform.rotation = this.info.Rotation;
        }
    }   // class Decal
}   //namespace DecalSystem