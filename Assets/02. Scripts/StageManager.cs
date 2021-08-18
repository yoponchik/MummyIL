using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    // Start is called before the first frame update

    public enum HINT_COLOR { 
        BLACK, BLUE, RED, GREEN
    }

    public HINT_COLOR hintColor = HINT_COLOR.BLACK;  //GO Cube Hint's color

    public Material[] hintMt;
    public string[] hintTag;


    //GO cube Hint's rendererer
    private new Renderer renderer;

    //to save the previous tag
    private int prevIndex = -1;

    void Start()
    {
        renderer = transform.Find("Hint").GetComponent<Renderer>();
    }

    public void InitStage() {

        int index = 0;

        do
        {
            index = Random.Range(0, hintMt.Length);
        }
        while (index == prevIndex);

        prevIndex = index;
            
     

        //change Hint Material
        renderer.material = hintMt[index];

        //give Hint new tag
        renderer.gameObject.tag = hintTag[index];

        //the target color
        hintColor = (HINT_COLOR)index; //typecasting
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            InitStage();
        }
    }
}
