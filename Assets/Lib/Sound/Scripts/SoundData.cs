using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Lib.Util;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lib.Sound
{
    /// <summary>
    /// AudioClipの纏まり
    /// </summary>
    [CreateAssetMenu(menuName = "Lib/Sound/SoundData")]
    public class SoundData : ScriptableObject
    {
        /// <summary>
        /// AudioClip情報
        /// </summary>
        [Serializable]
        public class AudioClipInfo
        {
            public string       key;
            public AudioClip    clip;

            public AudioClipInfo(string key, AudioClip clip)
            {
                key = string.IsNullOrEmpty(key) ? clip.name : key;
                this.key = key.ToUpperSnake();
                this.clip = clip;
            }
        }

        [SerializeField, HideInInspector] private string              _sourceFolder;
        [SerializeField, HideInInspector] private List<AudioClipInfo> _audioClipInfoList = new List<AudioClipInfo>();

        /// <summary>
        /// AudioClipを取得
        /// </summary>
        public AudioClip GetAudioClip(string key)
        {
            // とりあえず総当り
            foreach(var info in _audioClipInfoList)
            {
                if(info.key == key) { return info.clip; }
            }

            return null;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// AudioClipの格納先フォルダ以下を検索して、AudioClip情報を自動入力
        /// </summary>
        [CustomEditor(typeof(SoundData))]
        public class SoundDataEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                var data = target as SoundData;

                DrawDefaultInspector();

                EditorGUILayout.BeginVertical();

                // 元フォルダ
                EditorGUILayout.BeginHorizontal();
                data._sourceFolder = EditorGUILayout.TextField("Source Folder", data._sourceFolder);
                if(GUILayout.Button("Select"))
                {
                    var folderPath = EditorUtility.OpenFolderPanel("Source Folder", "Assets", "");
                    if(!string.IsNullOrEmpty(folderPath))
                    {
                        data._sourceFolder = folderPath.Remove(0, folderPath.LastIndexOf("Assets"));
                        EditorUtility.SetDirty(target);
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // リスト自動入力
                GUILayout.Label("キー名を変更したときもこのボタンを押してね");
                if(GUILayout.Button("Update"))
                {
                    data.UpdateSoundData();
                }

                // AudioClipのリスト
                var list = data._audioClipInfoList;
                GUI.enabled = false;
                int count = EditorGUILayout.DelayedIntField("Size", list.Count);
                GUI.enabled = true;
                foreach(var info in list)
                {
                    EditorGUILayout.BeginHorizontal();
                    info.key = EditorGUILayout.TextField(info.key).ToUpperSnake();
                    info.clip = EditorGUILayout.ObjectField(info.clip, typeof(AudioClip), true) as AudioClip;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// SoundDataを更新
        /// </summary>
        public void UpdateSoundData()
        {
            UpdateSoundDataList();
            UpdateSoundDataPath(GetInstanceID());
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// SoundDataリストを更新
        /// </summary>
        private void UpdateSoundDataList()
        {
            var pathList = Directory.GetFiles(GetAssetFullPath(_sourceFolder));
            var clipInfoList = new List<AudioClipInfo>();
            foreach(var path in pathList)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path.Remove(0, path.LastIndexOf("Assets")));
                if(clip == null) { continue; }

                var key = _audioClipInfoList.Where(x => x.clip == clip).FirstOrDefault()?.key;
                clipInfoList.Add(new AudioClipInfo(key, clip));
            }

            _audioClipInfoList = clipInfoList;
        }

        /// <summary>
        /// SoundDataのパスを列挙
        /// </summary>
        private void UpdateSoundDataPath(int instanceID)
        {
            string fname = $"SoundPath_{name}";
            string path = GetAssetFullPath(Path.GetDirectoryName(AssetDatabase.GetAssetPath(instanceID)));
            path = Path.Combine(path, $"{fname}.cs");

            using(var sw = File.CreateText(path))
            {
                sw.WriteLine($"public static class {fname}");
                sw.WriteLine("{");
                foreach(var info in _audioClipInfoList)
                {
                    sw.WriteIndentLine($"public const string _{info.key.Remove(0, info.key.IndexOf("/") + 1)} = \"{info.key}\";", 1);
                }
                sw.WriteLine("}");
            }
        }
        #endif

        /// <summary>
        /// Assets/から始まるパスをフルパスに変換
        /// </summary>
        private static string GetAssetFullPath(string path)
        {
            var strAssets = "Aassets/";
            path = path.Replace("\\", "/");
            path = path.Remove(0, path.LastIndexOf(strAssets) + strAssets.Count());
            path = Path.Combine(Application.dataPath, path);
            return path;
        }
    }
}
