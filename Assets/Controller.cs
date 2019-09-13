using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kultie.QuadTree;
using System.Diagnostics;
using UnityEditor;

public class Controller : MonoBehaviour
{
    QuadTree qt;
    public float width, height, posX, posY;

    //Visualization Section
    public GameObject pGoPref;
    public int objectCount;
    public bool showArea;
    public bool showPoints;
    public bool useQuadTreeForQuerry;
    public bool showQuerry;

    List<Point> points = new List<Point>();

    Rectangle mouseRect;


    // Use this for initialization
    void Start()
    {
        Rectangle rect = new Rectangle(posX, posY, width, height);
        qt = new QuadTree(rect, 4);

        for (int i = 0; i < objectCount; i++)
        {
            var p = new Point(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
            points.Add(p);
            qt.Insert(p);
        }


    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseRect = new Rectangle(mousePos.x, mousePos.y , 2, 2);

        //if (Input.GetMouseButton(0))
        //{
        //    //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    var p = new Point(mousePos.x, mousePos.y);
        //    qt.Insert(p);

        //    points.Add(p);
        //}

        //if (Input.GetMouseButtonDown(1))
        //{

        //    var p = new Point(0, 0);
        //    qt.Insert(p);

        //    points.Add(p);
        //}
    }

    List<Point> NormalQuerry(Rectangle rect) {
        List<Point> list = new List<Point>();
        for (int i = 0; i < points.Count; i++) {
            if (rect.Contains(points[i])) {
                list.Add(points[i]);
            }
        }       
        return list;
    }

    private void OnDrawGizmos()
    {

        if (qt != null)
        {
            if (showArea)
            {
                List<Rectangle> list = qt.GetAllRect();
                for (int i = 0; i < qt.GetAllRect().Count; i++)
                {
                    Gizmos.DrawWireCube(new Vector3(list[i].x, list[i].y), new Vector3(list[i].w, list[i].h));
                }
            }


            if (showPoints)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Gizmos.DrawSphere(new Vector3(points[i].x, points[i].y), 0.01f);
                }
            }

            if (showQuerry) {
                //showArea = true;
                //showPoints = true;
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector3(mouseRect.x, mouseRect.y), new Vector3(mouseRect.w, mouseRect.h));
               
                if (useQuadTreeForQuerry)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<Point> querry = qt.Querry(mouseRect);
                    sw.Stop();
                    UnityEngine.Debug.Log("Quad Tree Querry: " + sw.ElapsedMilliseconds + "ms");
                    for (int i = 0; i < querry.Count; i++)
                    {
                        Gizmos.DrawSphere(new Vector3(querry[i].x, querry[i].y), 0.01f);
                    }
                }
                else {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    List<Point> querry = NormalQuerry(mouseRect);
                    sw.Stop();
                    UnityEngine.Debug.Log("Normal Querry: " + sw.ElapsedMilliseconds + "ms");                    
                    for (int i = 0; i < querry.Count; i++)
                    {
                        Gizmos.DrawSphere(new Vector3(querry[i].x, querry[i].y), 0.01f);
                    }
                }
            }
        }
    }
}