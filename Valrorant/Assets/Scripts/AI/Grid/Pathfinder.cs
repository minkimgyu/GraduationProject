using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Grid.Pathfinder
{

    public class Pathfinder : MonoBehaviour
    {
        Func<Vector3Int, List<Vector3Int>> ReturnNearNodeIndexes;
        Func<Vector3, Vector3Int> ReturnNodeIndex;

        Func<Vector3Int, Node> ReturnNode;

        const int maxSize = 1000;

        Heap<Node> _openList = new Heap<Node>(maxSize);
        HashSet<Node> _closedList = new HashSet<Node>();

        // Start is called before the first frame update
        void Awake()
        {
            GridManager gridManager = GetComponent<GridManager>();
            ReturnNearNodeIndexes = gridManager.ReturnNearNodeIndexes;
            ReturnNodeIndex = gridManager.ReturnNodeIndex;

            ReturnNode = gridManager.ReturnNode;
        }

        List<Vector3> ConvertNodeToV3(Stack<Node> stackNode)
        {
            List<Vector3> points = new List<Vector3>();
            while (stackNode.Count > 0)
            {
                Node node = stackNode.Peek();
                points.Add(node.SurfacePos);
                stackNode.Pop();
            }

            return points;
        }

        // 가장 먼저 반올림을 통해 가장 가까운 노드를 찾는다.
        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            //// 리스트 초기화
            _openList.Clear();
            _closedList.Clear();

            Vector3Int startIndex = ReturnNodeIndex(startPos);
            Vector3Int endIndex = ReturnNodeIndex(targetPos);

            Node startNode = ReturnNode(startIndex);
            Node endNode = ReturnNode(endIndex);
            _openList.Insert(startNode);

            while (_openList.Count > 0)
            {
                Node targetNode = _openList.ReturnMin();
                if (targetNode == endNode) // 목적지와 타겟이 같으면 끝
                {
                    Stack<Node> finalList = new Stack<Node>();

                    Node TargetCurNode = targetNode;
                    while (TargetCurNode != startNode)
                    {
                        finalList.Push(TargetCurNode);
                        TargetCurNode = TargetCurNode.ParentNode;
                    }
                    //finalList.Push(startNode);

                    return ConvertNodeToV3(finalList);
                }

                _openList.DeleteMin(); // 해당 그리드 지워줌
                _closedList.Add(targetNode); // 해당 그리드 추가해줌
                AddNearGridInList(targetNode, endNode.SurfacePos); // 주변 그리드를 찾아서 다시 넣어줌
            }

            // 이 경우는 경로를 찾지 못한 상황임
            return null;
        }

        void AddNearGridInList(Node targetGrid, Vector3 targetGridPos)
        {
            Vector3Int index = ReturnNodeIndex(targetGrid.SurfacePos);
            List<Vector3Int> nearGridIndexes = ReturnNearNodeIndexes(index);

            for (int i = 0; i < nearGridIndexes.Count; i++)
            {
                Node nearNode = ReturnNode(nearGridIndexes[i]);
                if (nearNode.CanPass == false || _closedList.Contains(nearNode)) continue; // 통과하지 못하거나 닫힌 리스트에 있는 경우 다음 그리드 탐색

                // 이 부분 중요! --> 거리를 측정해서 업데이트 하지 않고 계속 더해주는 방식으로 진행해야함
                float moveCost = Vector3.Distance(targetGrid.SurfacePos, nearNode.SurfacePos);
                moveCost += targetGrid.G;

                bool isOpenListContainNearGrid = _openList.Contain(nearNode);

                // 오픈 리스트에 있더라도 G 값이 변경된다면 다시 리셋해주기
                if (isOpenListContainNearGrid == false || moveCost < nearNode.G)
                {
                    // 여기서 grid 값 할당 필요
                    nearNode.G = moveCost;
                    nearNode.H = Vector3.Distance(nearNode.SurfacePos, targetGridPos);
                    nearNode.ParentNode = targetGrid;
                }

                if (isOpenListContainNearGrid == false) _openList.Insert(nearNode);
            }
        }
    }
}