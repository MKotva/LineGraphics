using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using LineCanvas;
using Utilities;

namespace _092lines
{
  public class Lines
  {
    /// <summary>
    /// Form data initialization.
    /// </summary>
    /// <param name="name">Your first-name and last-name.</param>
    /// <param name="wid">Initial image width in pixels.</param>
    /// <param name="hei">Initial image height in pixels.</param>
    /// <param name="param">Optional text to initialize the form's text-field.</param>
    /// <param name="tooltip">Optional tooltip = param help.</param>
    public static void InitParams (out string name, out int wid, out int hei, out string param, out string tooltip)
    {
      // {{

      // Put your name here.
      name = "Milan Kotva";

      // Image size in pixels.
      wid = 1600;
      hei = 1200;

      // Specific animation params.
      param = "width=1.0,anti=true,objects=100,prob=0.95,seed=12,depth=150,inPerc=0.99,rndColor=false,showTriangle=false";

      // Tooltip = help.
      tooltip = "width=<int>, anti[=<bool>], objects=<int>, hatches=<int>, prob=<float>, seed=<int>, depth=<int>, inPerc=<float>, rndColor=<bool>, showTriangle=<bool>";

      // }}
    }

    /// <summary>
    /// Draw the image into the initialized Canvas object.
    /// </summary>
    /// <param name="c">Canvas ready for your drawing.</param>
    /// <param name="param">Optional string parameter from the form.</param>
    public static void Draw (Canvas c, string param)
    {
      var rnd = new Random();

      // Input params.
      float penWidth = 1.0f;   // pen width
      bool antialias = false;  // use anti-aliasing?
      int objects    = 100;    // number of randomly generated objects (squares, stars, Brownian particles)
      int hatches    = 12;     // number of hatch-lines for the squares
      double prob    = 0.95;   // continue-probability for the Brownian motion simulator
      int seed       = 12;     // random generator seed
      int depth      = 145;    // Count of inner triangles
      float inPerc = (float)0.99; // Percentil of triangle site. Just for triangle!!
      bool rndColor = false;   //Should color change?
      bool mode = true;        //Decide if show triangle or hexagon.

      Dictionary<string, string> p = Util.ParseKeyValueList(param);
      if (p.Count > 0)
      {
        // with=<line-width>
        if (Util.TryParse(p, "width", ref penWidth))
        {
          if (penWidth < 0.0f)
            penWidth = 0.0f;
        }

        // anti[=<bool>]
        Util.TryParse(p, "anti", ref antialias);

        // squares=<number>
        if (Util.TryParse(p, "objects", ref objects) &&
            objects < 0)
          objects = 0;

        // hatches=<number>
        if (Util.TryParse(p, "hatches", ref hatches) &&
            hatches < 1)
          hatches = 1;

        // prob=<probability>
        if (Util.TryParse(p, "prob", ref prob) &&
            prob > 0.999)
          prob = 0.999;

        Util.TryParse(p, "depth", ref depth);

        Util.TryParse(p, "inPerc", ref inPerc);

        Util.TryParse(p, "rndColor", ref rndColor);

        Util.TryParse(p, "showTriangle", ref mode);

        // seed=<int>
        Util.TryParse(p, "seed", ref seed);
      }

      //Triangle
      if (mode)
      {
        ColorSupport colorSupport;
        if (rndColor)
          colorSupport = new ColorSupport();
        else
          colorSupport = new ColorSupport(Color.DarkMagenta, Color.AntiqueWhite);

        var triangle = new Triangle(new Point(0, c.Height - 4), new Point(c.Width, c.Height - 4), new Point(c.Width/2, 0));
        triangle.DrawTriangle(c, colorSupport.actualColor);

        for (int i = 0; i < depth; i++)
        {
          triangle = triangle.GetInnerTriangle(c, inPerc);
          colorSupport.SetTransitionColor();
          triangle.DrawTriangle(c, colorSupport.actualColor);
        }
      }
      //Hexagon
      else
      {
        ColorSupport colorSupport;
        ColorSupport colorSupport2 = null;
        if (rndColor)
        {
          colorSupport = new ColorSupport();
          if (rnd.Next(0, 2) > 0)
            colorSupport2 = new ColorSupport(colorSupport);
        }
        else
          colorSupport = new ColorSupport(Color.Black, Color.LightGray);

        var thirdOfWidth = c.Width / 3;
        var thirdOfHeight = c.Height / 3;
        var halfOfWidth = c.Width / 2;
        var halfOfHeigth = c.Height / 2;

        var triangleA = new Triangle(new Point(0, halfOfHeigth), new Point(thirdOfWidth, 2), new Point(halfOfWidth, halfOfHeigth));
        var triangleB = new Triangle(new Point(thirdOfWidth, 2), new Point(thirdOfWidth * 2, 2), new Point(halfOfWidth, halfOfHeigth));
        var triangleC = new Triangle(new Point(thirdOfWidth * 2, 2), new Point(c.Width, halfOfHeigth), new Point(halfOfWidth, halfOfHeigth));
        var triangleD = new Triangle(new Point(c.Width, halfOfHeigth), new Point(thirdOfWidth * 2, c.Height -2), new Point(halfOfWidth, halfOfHeigth));
        var triangleE = new Triangle(new Point(thirdOfWidth * 2, c.Height -2), new Point(thirdOfWidth, c.Height -2), new Point(halfOfWidth, halfOfHeigth));
        var triangleF = new Triangle(new Point(thirdOfWidth, c.Height -2), new Point(0, halfOfHeigth), new Point(halfOfWidth, halfOfHeigth));

        var percentil = (float)1.0 / depth;
        var tmpPer = (float)1.0;
        for (int i = 0; i < depth; i++)
        {
          c.SetColor(colorSupport.actualColor);
          c.SetPenWidth(1);
          Point a;
          Point b;
          triangleA.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleB.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleC.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleD.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleE.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleF.GetOppositePoints((float)(1.0 - tmpPer), out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);

          if(colorSupport2 != null)
            c.SetColor(colorSupport2.actualColor);

          triangleA.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleB, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleB.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleC, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleC.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleD, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleD.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleE, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleE.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleF, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);
          triangleF.ConnectToABOfTriangles((float)(1.0 - tmpPer), triangleA, out a, out b);
          c.Line(a.X, a.Y, b.X, b.Y);

          tmpPer -= percentil;
          colorSupport.SetTransitionColor();
          if (colorSupport2 != null)
            colorSupport2.SetTransitionColor();
        }

        triangleA.DrawTriangle(c, colorSupport.actualColor);
        triangleB.DrawTriangle(c, colorSupport.actualColor);
        triangleC.DrawTriangle(c, colorSupport.actualColor);
        triangleD.DrawTriangle(c, colorSupport.actualColor);
        triangleE.DrawTriangle(c, colorSupport.actualColor);
        triangleF.DrawTriangle(c, colorSupport.actualColor);
      }
    }
  }

  class Triangle
  {
    public Point A { get; set; }
    public Point B { get; set; }
    public Point C { get; set; }


    public Triangle (Point a, Point b, Point c)
    {
      A = a;
      B = b;
      C = c;
    }

    public Triangle GetInnerTriangle(Canvas c, float percentilSize)
    {
      var xDistance = A.X - B.X;
      var yDistance = A.Y - B.Y;

      var vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      var Anew = new Point((int)(B.X + vector.X), (int)(B.Y + vector.Y));

      xDistance = B.X - C.X;
      yDistance = B.Y - C.Y;

      vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      var Bnew = new Point((int)(C.X + vector.X), (int)(C.Y + vector.Y));

      xDistance = C.X - A.X;
      yDistance = C.Y - A.Y;

      vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      var Cnew = new Point((int)(A.X + vector.X), (int)(A.Y + vector.Y));

      return new Triangle(Anew, Bnew, Cnew); 
    }

    public void GetOppositePoints(float percentilSize, out Point a, out Point b)
    {
      var xDistance = B.X - C.X;
      var yDistance = B.Y - C.Y;

      var vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      b = new Point((int)(C.X + vector.X), (int)(C.Y + vector.Y));

      xDistance = C.X - A.X;
      yDistance = C.Y - A.Y;

      vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      a = new Point((int)(A.X + vector.X), (int)(A.Y + vector.Y));
    }

    public void ConnectToABOfTriangles(float percentilSize, Triangle second, out Point a, out Point b)
    {
      var xDistance = A.X - B.X;
      var yDistance = A.Y - B.Y;

      var vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      a = new Point((int)(B.X + vector.X), (int)(B.Y + vector.Y));

      xDistance = second.A.X - second.B.X;
      yDistance = second.A.Y - second.B.Y;

      vector = new Vector2(xDistance, yDistance);
      vector = Vector2.Multiply(vector, percentilSize);

      b = new Point((int)(second.B.X + vector.X), (int)(second.B.Y + vector.Y));
    }
    public void DrawTriangle(Canvas c, Color color)
    {
      c.SetColor(color);
      c.SetPenWidth(1);
      c.Line(A.X, A.Y, B.X, B.Y);
      c.Line(B.X, B.Y, C.X, C.Y);
      c.Line(C.X, C.Y, A.X, A.Y);
    }
  }

  public class ColorSupport
  {
    private Random _rnd = new Random ();
    private Color[] _colorList;
    private Color _finalColor;
    public Color actualColor;

    public ColorSupport(Color actual, Color final)
    {
      var properties = typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color));
      var colorList = new List<Color>();
      foreach (var prop in properties)
      {
        colorList.Add((Color)prop.GetValue(prop.Name));
      }
      _colorList = colorList.ToArray();

      _finalColor = final;
      actualColor = actual;
    }

    public ColorSupport (ColorSupport choosedColor)
    {
      var properties = typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color));
      var colorList = new List<Color>();
      foreach (var prop in properties)
      {
        colorList.Add((Color)prop.GetValue(prop.Name));
      }
      _colorList = colorList.ToArray();

      while (true)
      {
        _finalColor = _colorList[_rnd.Next(0, _colorList.Length)];
        actualColor = _colorList[_rnd.Next(0, _colorList.Length)];
        if (actualColor != choosedColor.actualColor)
          break;
      }
    }

    public ColorSupport()
    {
      var properties = typeof(Color).GetProperties().Where(x => x.PropertyType == typeof(Color));
      var colorList = new List<Color>();
      foreach (var prop in properties)
      {
        colorList.Add((Color)prop.GetValue(prop.Name));
      }
      _colorList = colorList.ToArray();

      _finalColor = _colorList[_rnd.Next(0, _colorList.Length)];
      actualColor = _colorList[_rnd.Next(0, _colorList.Length)];
    }

    public void SetTransitionColor ()
    {
      var newR = 0;
      var newG = 0;
      var newB = 0;
      var newA = 0;

      if (actualColor.ToArgb() == _finalColor.ToArgb())
      {
        SetRandomFinalColor();
      }

      //Red
      if (actualColor.R > _finalColor.R)
        newR = actualColor.R - 1;
      else if (actualColor.R < _finalColor.R)
        newR = actualColor.R + 1;
      else
        newR = actualColor.R;

      //Green
      if (actualColor.G > _finalColor.G)
        newG = actualColor.G - 1;
      else if (actualColor.G < _finalColor.G)
        newG = actualColor.G + 1;
      else
        newG = actualColor.G;

      //Blue
      if (actualColor.B > _finalColor.B)
        newB = actualColor.B - 1;
      else if (actualColor.B < _finalColor.B)
        newB = actualColor.B + 1;
      else
        newB = actualColor.B;

      //Alpha
      if (actualColor.A > _finalColor.A)
        newA = actualColor.A - 1;
      else if (actualColor.A < _finalColor.A)
        newA = actualColor.A + 1;
      else
        newA = actualColor.A;

      actualColor = Color.FromArgb(newA, newR, newG, newB);
    }

    /// <summary>
    /// Sets random final color
    /// </summary>
    private void SetRandomFinalColor ()
    {
      _finalColor = _colorList[_rnd.Next(0, _colorList.Length)];
    }
  }
}
