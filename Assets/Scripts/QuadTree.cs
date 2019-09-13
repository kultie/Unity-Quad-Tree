using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Kultie.QuadTree
{
    [System.Serializable]
    public class QuadTree
    {
        public QuadTree parrent;
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

        public bool divided { private set; get; }

        public bool isEmpty
        {
            get
            {
                if (points == null) return true;
                return points.Count == 0;
            }
        }

        public int size
        {
            get
            {
                if (points == null) return 0;
                return points.Count;
            }
        }

        public Rectangle GetRectangle()
        {
            return rootRect;
        }

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
                    if (!divided)
                    {
                        Subdivide();
                    }

                    if (northWest.Insert(p))
                    {
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
            if (divided)
            {
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
            northWest.parrent = this;

            northEast = new QuadTree(ne, capacity);
            northEast.parrent = this;

            southWest = new QuadTree(sw, capacity);
            southWest.parrent = this;

            southEast = new QuadTree(se, capacity);
            southEast.parrent = this;

            divided = true;
        }

        public List<Point> Querry(Circle range)
        {
            List<Point> list = new List<Point>();

            if (!range.Intersect(rootRect))
            {
                //Not intersect
                return list;
            }

            if (points == null)
            {
                return list;
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (range.Contains(points[i]))
                {
                    list.Add(points[i]);
                }
            }

            if (!divided)
            {
                return list;
            }

            list.AddRange(northWest.Querry(range));
            list.AddRange(northEast.Querry(range));
            list.AddRange(southWest.Querry(range));
            list.AddRange(southEast.Querry(range));

            return list;
        }

        public List<Point> Querry(Rectangle range)
        {

            List<Point> list = new List<Point>();

            if (!rootRect.Intersect(range))
            {
                //Not intersect
                return list;
            }

            if (points == null)
            {
                return list;
            }

            for (int i = 0; i < points.Count; i++)
            {
                if (range.Contains(points[i]))
                {
                    list.Add(points[i]);
                }
            }

            if (!divided)
            {
                return list;
            }

            list.AddRange(northWest.Querry(range));
            list.AddRange(northEast.Querry(range));
            list.AddRange(southWest.Querry(range));
            list.AddRange(southEast.Querry(range));

            return list;
        }

        public QuadTree FindContainer(Point p)
        {
            if (Contains(p))
            {
                return this;
            }
            else
            {
                QuadTree container = null;
                if (divided)
                {
                    container = northWest.FindContainer(p);
                    if (container != null) return container;
                    container = northEast.FindContainer(p);
                    if (container != null) return container;
                    container = southEast.FindContainer(p);
                    if (container != null) return container;
                    container = southWest.FindContainer(p);
                    if (container != null) return container;

                }
                return container;
            }
        }

        public bool Contains(Point p)
        {
            if (rootRect.Contains(p))
            {
                if (points != null)
                {
                    return points.Contains(p);
                }
            }
            return false;
        }

        public void Remove(Point p)
        {
            if (!rootRect.Contains(p)) return;

            if (size != 0)
            {
                if (points.Contains(p))
                {
                    RemovePoint(p);
                }
                if (divided)
                {
                    northEast.Remove(p);
                    northWest.Remove(p);
                    southWest.Remove(p);
                    southEast.Remove(p);
                }
            }

            if (divided)
            {
                if (size <= capacity)
                {
                    UnSubDivide();
                }
            }


        }

        bool RemovePoint(Point p)
        {
            if (points == null) return false;
            if (points.Count > 0)
            {
                points.Remove(p);
                return true;
            }
            return false;

        }

        public void UnSubDivide()
        {
            TakePointsFromChild(northEast);
            TakePointsFromChild(northWest);
            TakePointsFromChild(southEast);
            TakePointsFromChild(southWest);
            int elementInChild = northEast.size + northWest.size + southWest.size + southEast.size;
            if (elementInChild == 0)
            {
                divided = false;
            }
        }

        public void TakePointsFromChild(QuadTree child)
        {
            if (child.size > 0)
            {
                for (int i = 0; i < child.size; i++)
                {
                    if (points.Count < capacity)
                    {
                        points.Add(child.points[i]);
                        child.points.Remove(child.points[i]);
                    }
                }
            }
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
            bool value = p.x <= x + w / 2 && p.x >= x - w / 2 && p.y <= y + h / 2 && p.y >= y - h / 2;
            if (value == false)
            {
                return value;
            }
            return value;
        }

        public bool Intersect(Rectangle rect)
        {
            return !(topLeft.x > rect.bottomRight.x || topLeft.y < rect.bottomRight.y || bottomRight.x < rect.topLeft.x || bottomRight.y > rect.topLeft.y);
        }
    }

    public class Circle
    {
        public float x, y, r;
        public float rSquared
        {
            get
            {
                return r * r;
            }
        }

        public Circle(float _x, float _y, float _r)
        {
            x = _x;
            y = _y;
            r = _r;
        }

        public bool Contains(Point p)
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(p.x, p.y)) <= r;
        }

        public bool Intersect(Rectangle rect)
        {
            float xDist = Mathf.Abs(x - rect.x);
            float yDist = Mathf.Abs(y - rect.y);

            float edges = Mathf.Pow((xDist - rect.w), 2) + Mathf.Pow((yDist - rect.h), 2);

            if (xDist > (r + rect.w) || yDist > (r + rect.h))
            {
                return false;
            }

            if (xDist < rect.w || yDist < rect.h)
            {
                return true;
            }

            return edges <= rSquared;
        }
    }
}
