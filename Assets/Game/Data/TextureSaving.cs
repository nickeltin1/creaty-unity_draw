using System;
using System.IO;
using System.Linq;
using Game.Scripts;
using PaintCore;
using UnityEngine;

namespace Game.Saving
{
    /// <summary>
    /// Collection of functions to save <see cref="CwPaintableTexture"/>'s to disk.
    /// Utilizes <see cref="SaveIDComponent"/> for easier save paths adjustable from editor. 
    /// 
    /// Note: Overall its probably better to make all loading async, but its exceeds requirements
    /// and will require bunch of overall flow changes, so whatever.
    /// </summary>
    public static class TextureSaving
    {
        public static readonly string PATH = Application.persistentDataPath + "/SavedTextures/";
        public const string EXT = "png";
        
        private static void WriteTexture(string subPath, byte[] texture)
        {
            File.WriteAllBytes(EnsureSubPathAndGetFullPath(subPath), texture);
        }

        private static byte[] ReadTexture(string subPath)
        {
            return File.ReadAllBytes(EnsureSubPathAndGetFullPath(subPath));
        }
        

        private static string EnsureSubPathAndGetFullPath(string subPath)
        {
            var path = Path.Combine(PATH, subPath);
            var directory = Path.GetDirectoryName(path);
            Directory.CreateDirectory(directory!);
            return path;
        }

        public static void WriteTexture(string subPath, CwPaintableTexture texture)
        {
            WriteTexture(subPath, texture.GetPngData());
        }

        public static void ReadTexture(string subPath, CwPaintableTexture texture)
        {
            var data = ReadTexture(subPath);
            texture.LoadFromData(data);
        }


        public static void WriteTextures(CwPaintableTexture[] textures, string[] paths)
        {
            VerifyCollectionsLength(textures, paths);
            
            for (var i = 0; i < textures.Length; i++)
            {
                WriteTexture(paths[i], textures[i]);
            }
        }

        public static void ReadTextures(CwPaintableTexture[] textures, string[] paths)
        {
            VerifyCollectionsLength(textures, paths);
            
            for (var i = 0; i < textures.Length; i++)
            {
                ReadTexture(paths[i], textures[i]);
            }
        }
        
        private static void VerifyCollectionsLength(CwPaintableTexture[] textures, string[] paths)
        {
            if (textures.Length != paths.Length)
            {
                throw new Exception($"{nameof(textures)} ({textures.Length}) and {nameof(paths)} ({paths.Length}) should have same size");
            }
        }

        private static SaveIDComponent[] CollectCorrespondingIDComponents(CwPaintableTexture[] textures, bool throwExceptionIfNotFound = true)
        {
            var idComponents = new SaveIDComponent[textures.Length];
            for (var i = 0; i < textures.Length; i++)
            {
                var texture = textures[i];
                if (texture.TryGetComponent(out SaveIDComponent saveID))
                    idComponents[i] = saveID;
                else if (throwExceptionIfNotFound)
                    throw new Exception($"{texture} don't have {nameof(SaveIDComponent)}");
            }

            return idComponents;
        }
        
        private static string[] CollectCorrespondingPathsFromIDComponents(CwPaintableTexture[] textures, bool throwExceptionIfNotFound = true)
        {
            return CollectCorrespondingIDComponents(textures, throwExceptionIfNotFound)
                .Select(component => component.CalculateSavePath(EXT))
                .ToArray();
        }
        
        public static void WriteTexturesWithIDComponents(CwPaintableTexture[] textures)
        {
            WriteTextures(textures, CollectCorrespondingPathsFromIDComponents(textures));
        }
       

        public static void ReadTexturesWithIDComponents(CwPaintableTexture[] textures)
        {
            ReadTextures(textures, CollectCorrespondingPathsFromIDComponents(textures));
        }
    }
}