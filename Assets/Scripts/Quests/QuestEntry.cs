using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestEntry : MonoBehaviour {

    public Quest quest;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI infosText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (quest != null)
        {
            descText.text = quest.defaultDescription;
            infosText.text = quest.SpecificInformation();
        }
    }
}
