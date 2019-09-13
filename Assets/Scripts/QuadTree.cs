using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.QuadTree
{
    [System.Serializable]
    public class QuadTree
    {
        //Represent rectangle of the Quad Tree
        Rectangle rootRect;
        //The max count of entities that one rectangle can hold before subdivide
        int capacity;

        List<Point> points;

        [SerializeField]
        public QuadTree northWest;
        [SerializeField]
        public QuadTree northEast;
        [SerializeField]
        public QuadTree southWest;
        [SerializeField]
        public QuadTree southEast;

        bool divided;

        public QuadTree(Rectangle rect, int _capacity)
        {
            rootRect = rect;
            capacity = _capacity;
            divided = false;
        }

        public bool Insert(Point p)
        {
            if (!rootRect.Contains(p))
            {
                return false;
            }

            if (points == null)
            {
                points = new List<Point>();
                points.Add(p);
                return true;
            }
            else
            {
                if (points.Count < capacity)
                {
                    points.Add(p);
                    return true;
                }
                else
                {
                    if (!divided) {
                        Subdivide();
                    }

                    if (northWest.Insert(p)) {
                        return true;
                    }

                    if (northEast.Insert(p))
                    {
                        return true;
                    }

                    if (southWest.Insert(p))
                    {
                        return true;
                    }

                    if (southEast.Insert(p))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public List<Rectangle> GetAllRect()
        {
            List<Rectangle> list = new List<Rectangle>();
            list.Add(rootRect);
            if (divided) {
                list.AddRange(northEast.GetAllRect());
                list.AddRange(northWest.GetAllRect());
                list.AddRange(southWest.GetAllRect());
                list.AddRange(southEast.GetAllRect());
            }
            return list;
        }

        void Subdivide()
        {
            Rectangle nw = new Rectangle(rootRect.x - rootRect.w / 4, rootRect.y + rootRect.h / 4, rootRect.w / 2, rootRect.h / 2);
            Rectangle ne = new Rectangle(rootRect.x + rootRect.w / 4, rootRect.y + rootRect.h / 4, rootRect.w / 2, rootRect.h / 2);
            Rectangle sw = new Rectangle(rootRect.x - rootRect.w / 4, rootRect.y - rootRect.h / 4, rootRect.w / 2, rootRect.h / 2);
            Rectangle se = new Rectangle(rootRect.x + rootRect.w / 4, rootRect.y - rootRect.h / 4, rootRect.w / 2, rootRect.h / 2);

            northWest = new QuadTree(nw, capacity);
            northEast = new QuadTree(ne, capacity);
            southWest = new QuadTree(sw, capacity);
            southEast = new QuadTree(se, capacity);

            divided = true;
        }

        public List<Point> Querry(Rectangle range) {

            List<Point> list = new List<Point>();

            if (!rootRect.Intersect(range)) {
                //Not intersect
                return list;
            }

            if (points == null) {
                return list;
            }

            for (int i = 0; i < points.Count; i++) {
                if (range.Contains(points[i])) {
                    list.Add(points[i]);
                }
            }

            if (!divided) {
                return list;
            }

            list.AddRange(northWest.Querry(range));
            list.AddRange(northEast.Querry(range));
            list.AddRange(southWest.Querry(range));
            list.AddRange(southEast.Querry(range));

            return list;
        }
    }
    //Base class for each entity that gonna use in Quad Tree
    [System.Serializable]
    public class Point
    {
        public float x;
        public float y;
        public Point(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

    }

    //Class represent the container of entities
    [System.Serializable]
    public class Rectangle
    {
        public float x, y, w, h;

        public Vector2 topLeft;
        public Vector2 bottomRight;

        public Rectangle(float _x, float _y, float _w, float _h)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            topLeft = new Vector2(x - w / 2, y + h / 2);
            bottomRight = new Vector2(x + w / 2, y - h / 2);
        }

        public bool Contains(Point p)
        {
            bool value =  p.x <= x + w / 2 && p.x >= x - w / 2 && p.y <= y + h / 2 && p.y >= y - h / 2;
            if (value == false) {
                return value;
            }
            return value;
        }

        public bool Intersect(Rectangle rect) {
            return !(topLeft.x > rect.bottomRight.x || topLeft.y < rect.bottomRight.y || bottomRight.x < rect.topLeft.x || bottomRight.y > rect.topLeft.y); 
        }
    }
}
