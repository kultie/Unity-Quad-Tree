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
    Particle selectedPoint;
    List<Particle> points = new List<Particle>();

    Circle mouseRect;


    // Use this for initialization
    void Start()
    {
        Rectangle rect = new Rectangle(posX, posY, width, height);
        qt = new QuadTree(rect, 4);
        selectedPoint = new Particle(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
        for (int i = 0; i < objectCount; i++)
        {
            var p = new Particle(Random.Range(-width / 2, width / 2), Random.Range(-height / 2, height / 2));
            points.Add(p);
            qt.Insert(p);
        }
        qt.Insert(selectedPoint);
        points.Add(selectedPoint);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseRect = new Circle(mousePos.x, mousePos.y, 2);

        Rectangle rect = new Rectangle(posX, posY, width, height);
        qt = new QuadTree(rect, 4);

        for (int i = 0; i < points.Count / 2; i++) {
            //qt.Remove(points[i]);
            points[i].Move();
            //qt.Insert(points[i]);
        }

        for (int i = 0; i < points.Count; i++)
        {
            qt.Insert(points[i]);
        }
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

    List<Particle> NormalCollisionDetection() {
        List<Particle> list = new List<Particle>();
        for (int i = 0; i < points.Count; i++)
        {
            Particle par = points[i];
            for (int j = 0; j < points.Count; j++) {
                if (par != points[j]) {
                    if (par.Intersect(points[j])) {
                        list.Add(points[j]);
                    }
                }
            }
        }
        return list;
    }

    List<Particle> QuadTreeCollisioDetection()
    {
        List<Particle> list = new List<Particle>();
        for (int i = 0; i < points.Count; i++)
        {
            Particle par = points[i];
            Circle circle = new Circle(points[i].x, points[i].y, 0.025f);
            List<Point> querryList = qt.Querry(circle);
            for (int j = 0; j < querryList.Count; j++)
            {
                if (par != querryList[j])
                {
                    if (par.Intersect((Particle)querryList[j]))
                    {
                        list.Add((Particle)querryList[j]);
                    }
                }
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
                    Gizmos.DrawSphere(new Vector3(points[i].x, points[i].y), 0.05f);
                }
                //if (selectedPoint != null)
                //{
                //    Gizmos.color = Color.yellow;

                //    Gizmos.DrawSphere(new Vector3(selectedPoint.x, selectedPoint.y), 0.05f);
                //    QuadTree container = qt.FindContainer(selectedPoint);
                //    if (container != null)
                //    {
                //        Rectangle rect = container.GetRectangle();
                //        Gizmos.DrawWireCube(new Vector3(rect.x, rect.y), new Vector3(rect.w, rect.h));
                //    }
                //}
            }

           


            if (showQuerry)
            {
                //showArea = true;
                //showPoints = true;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(mouseRect.x, mouseRect.y), 2);

                if (useQuadTreeForQuerry)
                {
                    List<Point> querry = qt.Querry(mouseRect);                 
                    for (int i = 0; i < querry.Count; i++)
                    {
                        Gizmos.DrawSphere(new Vector3(querry[i].x, querry[i].y), 0.05f);
                    }
                }
                //else
                //{
                //    Stopwatch sw = new Stopwatch();
                //    sw.Start();
                //    List<Point> querry = NormalQuerry(mouseRect);
                //    sw.Stop();
                //    UnityEngine.Debug.Log("Normal Querry: " + sw.ElapsedMilliseconds + "ms");
                //    for (int i = 0; i < querry.Count; i++)
                //    {
                //        Gizmos.DrawSphere(new Vector3(querry[i].x, querry[i].y), 0.01f);
                //    }
                //}

                List<Particle> partList = useQuadTreeForQuerry ? QuadTreeCollisioDetection() : NormalCollisionDetection();
                Gizmos.color = Color.blue;
                for (int i = 0; i < partList.Count; i++)
                {
                    Gizmos.DrawSphere(new Vector3(partList[i].x, partList[i].y), 0.05f);
                }
            }
        }
    }
}