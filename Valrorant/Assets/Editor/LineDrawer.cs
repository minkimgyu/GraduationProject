using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineDrawer : VisualElement
{
    List<Vector2> points = new List<Vector2>();

    // 0번 인덱스부터 끝 인덱스까지 연결하면서 선을 그린다.
    public void DrawUsingPoint(List<Vector2> points)
    {
        this.points = points;
        MarkDirtyRepaint();
    }

    public void EraseAllLines()
    {
        this.points.Clear();
        MarkDirtyRepaint();
    }

    public LineDrawer()
    {
        generateVisualContent += OnGenerateVisualContentRequested;
    }

    private void OnGenerateVisualContentRequested(MeshGenerationContext mgc)
    {
        //if (points.Count == 0) return;

        Painter2D _painter2d = mgc.painter2D;

        _painter2d.strokeColor = Color.red;
        _painter2d.lineJoin = LineJoin.Round;
        _painter2d.lineCap = LineCap.Round;

        _painter2d.lineWidth = 3.0f;

        _painter2d.BeginPath();

        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
            {
                _painter2d.MoveTo(points[i]);
                _painter2d.LineTo(points[i]);
            }
            else
            {
                _painter2d.LineTo(points[i]);
            }
        }

        _painter2d.Stroke();
    }
}

