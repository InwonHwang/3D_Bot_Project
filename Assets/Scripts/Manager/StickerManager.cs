using UnityEngine;
using System.Collections;

using Robot.Singleton;

//스티커 생성을 위해 만든 스크립트 비효율적
//나중에 ObjectManager에서 처리하기
public class StickerManager : MonoBehaviour
{
    void Start()
	{
		createSticker ();        
    }
	
	private void createSticker()
	{      
        for (int i = 0; i < 21; i++) {
            var stickerName = string.Format("StickerAtlas_{0}", i + 1);            
            var parent = GameObjectAgent.Instance.findChild("Object", "Stickers/" + stickerName).transform;
            var prefab = ResourcesManager.Instance.prefabs[stickerName];
            var dummy = Instantiate(ResourcesManager.Instance.prefabs["Dummy"]);

            dummy.transform.position = new Vector3(2000, 2000, 0);
            dummy.name = stickerName;
            dummy.transform.SetParent(parent);
            dummy.GetComponent<SpriteRenderer>().sprite = ResourcesManager.Instance.sprites[stickerName];

            if (!prefab || !parent) continue;

            for (int  j = 0; j < 30; j++) {             
                var sticker = Instantiate(prefab) as GameObject;
				sticker.name = stickerName;				
				sticker.transform.SetParent(parent);                

                var decal = sticker.GetComponent<DecalSystem.Decal>();
                if (decal)
                {
                    sticker.transform.localScale = new Vector3(decal.sprite.textureRect.width / 300,
                                                               decal.sprite.textureRect.height / 300,
                                                               decal.sprite.textureRect.width / 80);                    
                    sticker.transform.position = new Vector3(2000, 2000, 0);
                }
            }
		}
	}
}
