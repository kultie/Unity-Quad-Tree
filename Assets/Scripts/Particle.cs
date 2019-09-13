using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kultie.QuadTree;
public class Particle: Point {
    public Particle(float _x, float _y) : base(_x, _y)
    {
        x = _x;
        y = _y;
    }

    public void Move() {
        x += Random.Range(-0.05f, 0.05f);
        y += Random.Range(-0.05f, 0.05f);
    }

    public bool Intersect(Particle p) {
        return Vector2.Distance(new Vector2(x, y), new Vector2(p.x, p.y)) <= 0.05f;
    }
}
