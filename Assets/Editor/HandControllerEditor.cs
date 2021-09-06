using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandController))]
public class HandControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        HandController handController = (HandController)target;

        if (handController.UsePhysics)
        {
            if (GUILayout.Button("Build Physics Hand"))
            {

            }
        }


        if (GUILayout.Button("Build"))
        {

        }

        

    }
}
