using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;

public class EditorHelper : MonoBehaviour
{
    [MenuItem("EditorHelper/SliceSprites")]
    static void SliceSprites()
    {
        // Change the below for the with and height dimensions of each sprite within the spritesheets
        int sliceWidth = 64;
        int sliceHeight = 64;

        // Change the below for the path to the folder containing the sprite sheets
        string folderPath = "ToSlice";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        Debug.Log("spriteSheets.Length: " + spriteSheets.Length);

        for (int z = 0; z < spriteSheets.Length; z++)
        {
            Debug.Log("z: " + z + " spriteSheets[z]: " + spriteSheets[z]);

            string path = AssetDatabase.GetAssetPath(spriteSheets[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
            dataProvider.InitSpriteEditorDataProvider();

            List<SpriteRect> newData = new List<SpriteRect>();
            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth)
            {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight)
                {
                    SpriteRect sr = new SpriteRect();
                    sr.pivot = new Vector2(0.5f, 0.5f);
                    sr.alignment = SpriteAlignment.Center;
                    sr.name = (spriteSheet.height - j) / sliceHeight + ", " + i / sliceWidth;
                    sr.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);
                    sr.spriteID = GUID.Generate();

                    newData.Add(sr);
                }
            }

            dataProvider.SetSpriteRects(newData.ToArray());
            dataProvider.Apply();

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }
}