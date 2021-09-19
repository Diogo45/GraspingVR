using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Statistics : MonoBehaviour
{
    // Start is called before the first frame update

    private TMP_Text text;
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = string.Format("{0:3F}",Time.deltaTime.ToString());
    }
}
