using System;
using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;

namespace RemoteHierarchy
{
    public class SerilizeJsonUtility
    {
        public static JSONObject SerializeGameObject(GameObject gameObject)
        {
            List<JSONObject> array = new List<JSONObject>(4);
            var comps = gameObject.GetComponents<Component>();
            foreach (var comp in comps)
            {
                array.Add(SerializeComponent(comp));
            }
            return JSONObject.Create(array);
        }

        public static JSONObject SerializeComponent(Component component)
        {
            if (component == null)
                return JSONObject.nullObject;

            Type componentType = component.GetType();
            Type[] parameterTypes = new Type[] { componentType };

            // Find the appropriate serialization method based on the component type
            var serializeMethod = typeof(SerilizeJsonUtility).GetMethod("Serialize", parameterTypes);
            if (serializeMethod != null)
            {
                return (JSONObject)serializeMethod.Invoke(null, new object[] { component });
            }

            return JSONObject.StringObject(componentType.Name);
        }

        public static JSONObject Serialize(Transform component)
        {
            return new JSONObject(jsn =>
            {
                jsn.AddField("type", component.GetType().Name);
                jsn.AddField("position", component.position.ToJson());
                jsn.AddField("rotation", component.eulerAngles.ToJson());
                jsn.AddField("lossyScale", component.lossyScale.ToJson());
                jsn.AddField("localPosition", component.localPosition.ToJson());
                jsn.AddField("localRotation", component.localEulerAngles.ToJson());
                jsn.AddField("localScale", component.localScale.ToJson());
            });
        }

        public static JSONObject Serialize(MeshFilter component)
        {
            return new JSONObject(jsn =>
            {
                jsn.AddField("type", component.GetType().Name);
                jsn.AddField("mesh", component.sharedMesh.name);
            });
        }

        public static JSONObject Serialize(MeshRenderer component)
        {
            return new JSONObject(jsn =>
            {
                jsn.AddField("type", component.GetType().Name);
                int count = 0;
                foreach (var mat in component.sharedMaterials)
                {
                    jsn.AddField($"material[{count}]", component.sharedMaterial.name);
                    count++;
                }
            });
        }
    }
}
