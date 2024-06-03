using System;
using UnityEditor;
using UnityEngine;

namespace RemoteHierarchy.EditorGUIHelper
{
    /// <summary>
    /// Represents a temporary change in GUI color.
    /// </summary>
    public struct GUIColor : IDisposable
    {
        private Color m_kColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GUIColor"/> struct and changes the GUI color.
        /// </summary>
        /// <param name="color">The new GUI color.</param>
        public GUIColor(Color color)
        {
            this.m_kColor = GUI.color;
            GUI.color = color;
        }
            
        /// <summary>
        /// Restores the previous GUI color.
        /// </summary>
        public void Dispose()
        {
            GUI.color = m_kColor;
        }
    }
}