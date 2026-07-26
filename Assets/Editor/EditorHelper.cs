using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Sprites;
using System.IO;
using System.Linq;
public class EditorHelper : MonoBehaviour
{
    [MenuItem("EditorHelper/SliceSprites")]
    static void SliceSprites()
    {
        // Change the below for the with and height dimensions of each sprite within the spritesheets
        int sliceWidth = 64;
        int sliceHeight = 64;

        // Change the below for the path to the folder containing the sprite sheets (warning: not tested on folders containing anything other than just spritesheets!)
        // Ensure the folder is within 'Assets/Resources/' (the below example folder's full path within the project is 'Assets/Resources/ToSlice')
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

            // 1. Khởi tạo Data Provider theo chuẩn API mới của Unity
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(ti);
            dataProvider.InitSpriteEditorDataProvider();

            // 2. Dùng List<SpriteRect> thay vì List<SpriteMetaData>
            List<SpriteRect> newData = new List<SpriteRect>();

            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth)
            {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight)
                {
                    SpriteRect rect = new SpriteRect();

                    // Bắt buộc phải tạo GUID (ID độc nhất) cho mỗi sprite theo chuẩn mới
                    rect.spriteID = GUID.Generate();

                    // Đặt tên theo chuẩn không chứa ký tự đặc biệt (đã fix ở lỗi trước)
                    rect.name = $"{spriteSheet.name}_{(spriteSheet.height - j) / sliceHeight}_{i / sliceWidth}";

                    rect.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

                    // Alignment = 9 trong API cũ tương đương với Custom
                    rect.alignment = SpriteAlignment.Custom;
                    rect.pivot = new Vector2(0.5f, 0.5f);

                    newData.Add(rect);
                }
            }

            // 3. Áp dụng dữ liệu mới thông qua provider thay vì dùng ti.spritesheet
            dataProvider.SetSpriteRects(newData.ToArray());
            dataProvider.Apply(); // Lưu thay đổi vào bộ nhớ

            // 4. Force Update lại asset
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }
}