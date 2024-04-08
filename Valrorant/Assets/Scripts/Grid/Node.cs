using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class Node : IItem<Node>
    {
        public Node(Vector3 pos, Vector3 surfacePos, bool canPass)
        {
            _pos = pos;
            _surfacePos = surfacePos;
            _canPass = canPass;
            StoredIndex = -1;
        }

        Vector3 _pos; // 그리드 실제 위치
        public Vector3 Pos { get { return _pos; } }

        Vector3 _surfacePos; // 발을 딛을 수 있는 표면 위치
        public Vector3 SurfacePos { set { _surfacePos = value; } get { return _surfacePos; } }

        bool _canPass;
        public bool CanPass { get { return _canPass; } }

        // g는 시작 노드부터의 거리
        // h는 끝 노드부터의 거리
        // f는 이 둘을 합친 값
        float g, h = 0;
        public float G { get { return g; } set { g = value; } }
        public float H { get { return h; } set { h = value; } }
        public float F { get { return g + h; } }

        public Node ParentNode { get; set; }
        public int StoredIndex { get; set; }

        public int CompareTo(Node other)
        {
            int compareValue = F.CompareTo(other.F);
            if (compareValue == 0) compareValue = H.CompareTo(other.H);
            return compareValue;
        }

        public void Dispose()
        {
            StoredIndex = -1;
            ParentNode = null;
        }
    }
}