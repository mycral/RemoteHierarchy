using System;
using UnityEditor;
using UnityEngine;

namespace RemoteHierarchy.EditorGUIHelper
{
    public struct GUIColor : IDisposable
    {
        private Color m_kColor;
        public GUIColor(Color color)
        {
            this.m_kColor = GUI.color;
            GUI.color = color;
        }
            
        public void Dispose()
        {
            GUI.color = m_kColor;
        }
    }
}