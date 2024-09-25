using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    public class DialogNode
    {
        [Header("索引（对话索引选项需要连续）")]
        public int id;

        [Header("跳转索引（如果是最后一句，值为id+1）"), Space(1)]
        public int nextId;

        [Header("是否是对话选项"), Space(1)]
        public bool isOption;

        [Header("角色名称"), Space(1)]
        public string name;

        [Header("角色图片"), Space(1)]
        public Sprite sprite;

        [Header("对话内容"), TextArea, Space(1)]
        public string context;
    }
}

