using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class ComponentCopy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject parentA;
    public GameObject parentB;
    
    void Awake()
    {
        CopyRecursive(parentA, parentB);

    }

    void CopyRecursive(GameObject a, GameObject b)
    {
        Debug.Log(a.name + " " + b.name);

        for (int i = 0; i < a.transform.childCount; i++)
        {
            var c = a.transform.GetChild(i);
            var d = b.transform.GetChild(i);
            Debug.Log("     " + c.name + " " + d.name);

            CopyComponents(c.gameObject, d.gameObject);
            if(c.childCount > 0)
                CopyRecursive(c.gameObject, d.gameObject);

        }
    }


    void CopyComponents(GameObject from, GameObject target)
    {
        var components = from.GetComponents<Component>();
        foreach (var item in components)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(item);

            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(target);
        }
    }
}
#endif