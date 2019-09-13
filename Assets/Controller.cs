using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kultie.QuadTree;
public class Controller : MonoBehaviour
{
    QuadTree qt;
    public float width, height, posX , posY;

    //Visualization Section
    public GameObject pGoPref;
    public int objectCount;
    public bool showArea;
    public bool showPoints;
    List<Point> points = new List<Point>();




    // Use this for initialization
    void Start()
    {
        Rectangle rect = new Rectangle(posX, posY, width, height);
        qt = new QuadTree(rect, 4);

        //for (int i = 0; i < objectCount; i++)
        //{
        //    var p = new Point(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
        //    points.Add(p);
        //    qt.Insert(p);
        //}


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var p = new Point(mousePos.x, mousePos.y);
            qt.Insert(p);

            points.Add(p);
        }
    }

    private void OnDrawGizmos()
    {

        if (qt != null) {
            if (showArea) {
                List<Rectangle> list = qt.GetAllRect();
                for (int i = 0; i < qt.GetAllRect().Count; i++)
                {
                    Gizmos.DrawWireCube(new Vector3(list[i].x, list[i].y), new Vector3(list[i].w, list[i].h));
                }
            }


            if (showPoints) {
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.DrawSphere(new Vector3(points[i].x, points[i].y), 0.01f);
                }
            }

        }
    }
}