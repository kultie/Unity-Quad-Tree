using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kultie.NewQuadTree;

public class NewController : MonoBehaviour {

    QuadTree qt;
    public float width, height, posX, posY;

    public GameObject pGoPref;
    public int objectCount;


    void Start()
    {
        qt = new QuadTree((int)width, (int)height, 4, 4);
        for (int i = 0; i < objectCount; i++) {
            qt.insert(i, 1, 1, 1, 1);
        }
        IntList list = qt.query(1, 1, 1, 1);
        for (int i = 1; i <= list.size(); i++) {
            Debug.Log(list.get(i,1));
        }

    }

}
