using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

/// <summary>
/// Facilitates the creation of a Texture Array asset within Unity.
/// </summary>
public class TextureArrayWizard : ScriptableWizard
{
    [MenuItem("Assets/Create/Texture Array")]
    static void CreateWizard()
    {
        DisplayWizard<TextureArrayWizard>("Create Texture Array", "Create");
    }

    public Texture2D[] textures;

    void OnWizardCreate()
    {
        if (textures.Length > 0) {
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Texture Array", "Texture Array", "asset", "Save Texture Array"
            );
            if (path != "") {
                CreateTextureArray(path);
            }
        }
    }

    private void CreateTextureArray(string path)
    {
        Texture2D t = textures[0];
        Texture2DArray textureArray = new Texture2DArray(
            t.width, t.height, textures.Length, t.format, t.mipmapCount > 1
        ) {
            anisoLevel = t.anisoLevel,
            filterMode = t.filterMode,
            wrapMode = t.wrapMode,
        };
        for (int i = 0; i < textures.Length; i++) {
            Texture2D texture = textures[i];
            for (int mipmap = 0; mipmap < texture.mipmapCount; mipmap++) {
                Graphics.CopyTexture(texture, 0, mipmap, textureArray, i, mipmap);
            }
        }
        AssetDatabase.CreateAsset(textureArray, path);
    }
}