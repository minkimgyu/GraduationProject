using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] float _nodeSize = 0.5f;
        [SerializeField] float _surfaceHeight = 0.1f;

        [SerializeField] LayerMask _layerMask;
        [SerializeField] Vector3Int _sizeOfGrid;

        [SerializeField] Color _nodeColor;
        [SerializeField] Color _canPassSurfaceColor;
        [SerializeField] Color _cantPassSurfaceColor;

        [SerializeField] bool _showNode;
        [SerializeField] bool _showSurface;
        [SerializeField] bool _showNavigationRect;

        List<Node>[,] _simpleNodes;

        [Range(0f, 90f)]
        [SerializeField] float _maxAngleBetweenNode = 45; // 허용가능한 Grid cell 간의 최대 각도 차이

        [SerializeField] Transform _bottomLeftTarget;

        // Start is called before the first frame update
        void Start()
        {
            CreateGrid();
        }

        bool IsCollidersContainObstacle(Collider[] colliders)
        {
            bool canPass = true;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].CompareTag("CanNotPass"))
                {
                    canPass = false;
                    break;
                }
            }

            return canPass;
        }

        void CreateGrid()
        {
            bool[,,] _checkIsFill = new bool[_sizeOfGrid.x, _sizeOfGrid.y, _sizeOfGrid.z];
            _simpleNodes = new List<Node>[_sizeOfGrid.x, _sizeOfGrid.z];
            Vector3 previousPos = Vector3.zero;
            bool previousCanPass = true;

            float halfSize = _nodeSize / 2;
            Vector3 boxSize = new Vector3(halfSize, halfSize, halfSize);
            Vector3 originPos = _bottomLeftTarget.position + new Vector3(halfSize, 0, halfSize);

            for (int x = 0; x < _sizeOfGrid.x; x++)
            {
                for (int z = 0; z < _sizeOfGrid.z; z++)
                {
                    _simpleNodes[x, z] = new List<Node>();
                    for (int y = 0; y < _sizeOfGrid.y; y++)
                    {
                        Vector3 pos = originPos + new Vector3(x, y, z) * _nodeSize;
                        Collider[] colliders = Physics.OverlapBox(pos, boxSize, Quaternion.identity, _layerMask);

                        bool canPass = IsCollidersContainObstacle(colliders);

                        bool isFill = false;
                        if (colliders.Length > 0) isFill = true;
                        _checkIsFill[x, y, z] = isFill;

                        if (y > 0 && _checkIsFill[x, y - 1, z] == true && isFill == false)
                        {
                            // 레이케스트를 아래로 쏴서 확인해보기
                            RaycastHit hit;
                            Physics.BoxCast(pos, boxSize, Vector3.down, out hit, Quaternion.identity, 30, _layerMask);
                            Vector3 surfacePos = new Vector3(pos.x, hit.point.y, pos.z);

                            Node grid = new Node(previousPos, surfacePos, previousCanPass);
                            _simpleNodes[x, z].Add(grid);
                        }

                        previousPos = pos;
                        previousCanPass = canPass;
                    }
                }
            }
        }
        

        public Node ReturnNode(Vector3Int index) { return _simpleNodes[index.x, index.z][index.y]; }
        public Node ReturnNode(int x, int z) { return _simpleNodes[x, z][0]; }

        public List<Vector3Int> ReturnNearNodeIndexes(Vector3Int index)
        {
            List<Vector3Int> closeGridIndex = new List<Vector3Int>();

            // 주변 그리드
            Vector3Int[] closeIndex = new Vector3Int[] {
                new Vector3Int(index.x - 1, 0, index.z + 1), new Vector3Int(index.x, 0, index.z + 1), new Vector3Int(index.x + 1, 0, index.z + 1),
                new Vector3Int(index.x - 1, 0, index.z), new Vector3Int(index.x + 1, 0, index.z),
                new Vector3Int(index.x - 1, 0, index.z - 1), new Vector3Int(index.x, 0, index.z - 1), new Vector3Int(index.x + 1, 0, index.z - 1)
            };

            for (int i = 0; i < closeIndex.Length; i++)
            {
                bool isOutOfRange = closeIndex[i].x < 0 || closeIndex[i].z < 0 || closeIndex[i].x > _sizeOfGrid.x - 1 || closeIndex[i].z > _sizeOfGrid.z - 1;
                if (isOutOfRange == true) continue;

                // 여기서 경사도를 판단해준다.
                Vector3 currentNodePos = _simpleNodes[index.x, index.z][index.y].SurfacePos;
                Vector3 nearNodePos; // 가까이 있는 노드 위치

                int yIndex;
                List<Node> nodes = _simpleNodes[closeIndex[i].x, closeIndex[i].z]; // 여기서 y 인덱스 이용해서 가능한 높이 찾아서 집어넣기

                if (nodes.Count == 1) yIndex = closeIndex[i].y; // 1인 경우
                else yIndex = ReturnCloestNodeIndexInYAxis(nodes, currentNodePos.y); // 1이 아닌 경우 가장 가까운 노드 값을 집어넣음

                nearNodePos = _simpleNodes[closeIndex[i].x, closeIndex[i].z][yIndex].SurfacePos;

                bool isIn = IsInMaxAngleBetweenNode(currentNodePos, nearNodePos);
                if (isIn == false) continue;

                closeGridIndex.Add(new Vector3Int(closeIndex[i].x, yIndex, closeIndex[i].z));
            }

            return closeGridIndex;
        }

        public Vector3 ReturnClampedRange(Vector3 pos)
        {
            Vector3 bottomLeftPos = ReturnNode(0, 0).Pos;
            Vector3 topRightPos = ReturnNode(_sizeOfGrid.x - 1, _sizeOfGrid.z - 1).Pos;

            // 반올림하고 범위 안에 맞춰줌
            // 이 부분은 GridSize 바뀌면 수정해야함
            float xPos = Mathf.Clamp(pos.x, bottomLeftPos.x, topRightPos.x);
            float zPos = Mathf.Clamp(pos.z, bottomLeftPos.z, topRightPos.z);

            return new Vector3(xPos, pos.y, zPos);
        }

        int ReturnCloestNodeIndexInYAxis(List<Node> nodes, float yPos)
        {
            float storedDistance = 0;
            int index = 0;

            for (int i = 0; i < nodes.Count; i++)
            {
                float distance = Mathf.Abs(yPos - nodes[i].SurfacePos.y);

                if (i == 0) storedDistance = distance;
                else
                {
                    if (storedDistance <= distance) continue;

                    storedDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        public Vector3 ReturnNodePos(Vector3 worldPos)
        {
            Vector3 clampedPos = ReturnClampedRange(worldPos);
            Vector3 bottomLeftPos = ReturnNode(0, 0).Pos;

            int xIndex = Mathf.RoundToInt((clampedPos.x - bottomLeftPos.x) / _nodeSize); // 인덱스이므로 1 빼준다.
            int zIndex = Mathf.RoundToInt((clampedPos.z - bottomLeftPos.z) / _nodeSize);

            List<Node> currentNode = _simpleNodes[xIndex, zIndex];
            
            if (currentNode.Count == 0) return Vector3Int.zero;
            else if (currentNode.Count == 1)
            {
                return _simpleNodes[xIndex, zIndex][0].SurfacePos;
            }
            else
            {
                int yIndex = ReturnCloestNodeIndexInYAxis(currentNode, worldPos.y);
                return _simpleNodes[xIndex, zIndex][yIndex].SurfacePos;
            }
        }

        public Vector3Int ReturnNodeIndex(Vector3 worldPos)
        {
            Vector3 clampedPos = ReturnClampedRange(worldPos);
            Vector3 bottomLeftPos = ReturnNode(0, 0).Pos;

            int xIndex = Mathf.RoundToInt((clampedPos.x - bottomLeftPos.x) / _nodeSize); // 인덱스이므로 1 빼준다.
            int zIndex = Mathf.RoundToInt((clampedPos.z - bottomLeftPos.z) / _nodeSize);

            List<Node> currentNode = _simpleNodes[xIndex, zIndex];

            if (currentNode.Count == 0) return Vector3Int.zero;
            else if (currentNode.Count == 1) return new Vector3Int(xIndex, 0, zIndex);
            else
            {
                int yIndex = ReturnCloestNodeIndexInYAxis(currentNode, worldPos.y);
                return new Vector3Int(xIndex, yIndex, zIndex);
            }
        }

        float ReturnAngleBetweenNode(Vector3 targetNodePos, Vector3 nearNodePos)
        {
            float yDistance = Mathf.Abs(nearNodePos.y - targetNodePos.y);
            float xzDistance = Vector3.Distance(new Vector3(targetNodePos.x, 0, targetNodePos.z), new Vector3(nearNodePos.x, 0, nearNodePos.z));

            return Mathf.Atan2(yDistance, xzDistance) * Mathf.Rad2Deg; // 반반해서 _gridCellSize가 width임
        }

        public bool IsInMaxAngleBetweenNode(Vector3 targetNodePos, Vector3 nearNodePos)
        {
            float yDiatance = targetNodePos.y - nearNodePos.y;
            if (yDiatance > 0) return true;

            float angle = ReturnAngleBetweenNode(targetNodePos, nearNodePos); // 반반해서 _gridCellSize가 width임
            return angle <= _maxAngleBetweenNode;
        }

        void DrawNavigationRect()
        {
            float x = _bottomLeftTarget.position.x + _sizeOfGrid.x / 2;
            float y = _bottomLeftTarget.position.y + _sizeOfGrid.y / 2;
            float z = _bottomLeftTarget.position.z + _sizeOfGrid.z / 2;

            DrawGizmoCube(new Vector3(x, y, z), _canPassSurfaceColor, _sizeOfGrid);
        }

        void DrawGizmoCube(Vector3 pos, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(pos, new Vector3(_nodeSize, _nodeSize, _nodeSize));
        }

        void DrawGizmoCube(Vector3 pos, Color color, Vector3 size)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(pos, size);
        }

        void DrawGizmoCube(Vector3 pos, Color color, float height)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(pos + new Vector3(0, _nodeSize * height / 2, 0), new Vector3(_nodeSize, _nodeSize * height, _nodeSize));
        }

        #region DrawGizmos

        private void OnDrawGizmos()
        {
            if(_showNavigationRect) DrawNavigationRect();

            if (Application.isPlaying == false || _simpleNodes.Length == 0) return;

            int xLength = _simpleNodes.GetLength(0);
            int zLength = _simpleNodes.GetLength(1);

            for (int x = 0; x < xLength; x++)
            {
                for (int z = 0; z < zLength; z++)
                {
                    List<Node> grids = _simpleNodes[x, z];
                    int count = grids.Count;
                    for (int y = 0; y < count; y++)
                    {
                        if (_showSurface)
                        {
                            bool canPass = grids[y].CanPass == true;
                            if(canPass) DrawGizmoCube(grids[y].SurfacePos, _canPassSurfaceColor, _surfaceHeight);
                            else DrawGizmoCube(grids[y].SurfacePos, _cantPassSurfaceColor, _surfaceHeight);
                        }

                        if(_showNode) DrawGizmoCube(grids[y].SurfacePos, _nodeColor);
                    }
                }
            }
        }

        #endregion
    }
}