/***************************************************************
 * Copyright 2016 By Zhang Minglin
 * Author: Zhang Minglin
 * Create: 2016/01/25
 * Note  : 资源加载器
***************************************************************/
using UnityEngine;
using System.Collections;
using System.IO;

namespace XFramework.AssetBundlePacker
{
    /// <summary>
    ///   资源加载器
    /// </summary>
    public static class ResourcesManager
    {
        /// <summary>
        ///   资源相对目录
        /// </summary>
        public static readonly string RESOURCES_LOCAL_DIRECTORY = "Assets/Resources/";

        /// <summary>
        ///   资源全局目录 
        /// </summary>
        public static readonly string RESOURCES_DIRECTORY = Application.dataPath + "/Resources/";

        /// <summary>
        /// 
        /// </summary>
        public static readonly string RESOURCES_PATH = Application.dataPath + "/Resources";

        /// <summary>
        /// 资源加载方式，默认采用DefaultLoadPattern
        /// </summary>
        public static ILoadPattern LoadPattern = new DefaultLoadPattern();


        /// <summary>
        ///   加载一个资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T Load<T>(string asset, bool UseResRoot = true)
                where T : Object
        {
            T result = null;
            if(UseResRoot) asset = string.Format("{0}{1}", AssetDefine.ResRoot, asset);
#if UNITY_EDITOR
            if (LoadPattern.ResourcesLoadPattern == emLoadPattern.EditorAsset
                || LoadPattern.ResourcesLoadPattern == emLoadPattern.All)
            {
                result = ResourcesManager.LoadAssetAtPath<T>(asset);
                if (result != null)
                    return result;
            }
#endif

            if (LoadPattern.ResourcesLoadPattern == emLoadPattern.AssetBundle
                || LoadPattern.ResourcesLoadPattern == emLoadPattern.All)
            {
                result = AssetBundleManager.Instance.LoadAsset<T>(asset);
                if (result != null)
                    return result;
            }
            if (LoadPattern.ResourcesLoadPattern == emLoadPattern.Original
                || LoadPattern.ResourcesLoadPattern == emLoadPattern.All)
            {
                result = ResourcesManager.LoadResources<T>(asset);
                if (result != null)
                    return result;
            }

            return result;
        }

        /// <summary>
        ///   加载一个Resources下资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T LoadResources<T>(string asset)
            where T : Object
        {
            //去除扩展名
            asset = XFramework.FileHelper.GetPathWithoutExtension(asset);
            //转至以Resources为根目录的相对路径
            asset = XFramework.FileHelper.AbsoluteToRelativePath(RESOURCES_LOCAL_DIRECTORY, asset);
            T a = Resources.Load<T>(asset);
            return a;
        }

        /// <summary>
        ///   文本文件加载
        /// <param name="file_name">全局路径</param>
        /// </summary>
        public static string LoadTextFile(string file_name)
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    if (File.Exists(file_name))
                        return File.ReadAllText(file_name);
                }

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return null;
        }

        /// <summary>
        ///   二进制文件加载
        /// <param name="file_name">全局路径</param>
        /// </summary>
        public static byte[] LoadByteFile(string file_name)
        {
            try
            {
                if (!string.IsNullOrEmpty(file_name))
                {
                    if (File.Exists(file_name))
                        return File.ReadAllBytes(file_name);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }

            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        ///   加载一个Resources下资源
        /// <param name="asset">资源局部路径（"Assets/..."）</param>
        /// </summary>
        public static T LoadAssetAtPath<T>(string asset)
            where T : Object
        {
            if (asset.Contains("Resources"))
            {
                return LoadResources<T>(asset);
            }
            else
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(asset);
            }
        }
#endif
    }
}