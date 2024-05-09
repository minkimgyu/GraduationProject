using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

public enum RecoilType
{
    Map,
    Range
}

public class SprayEditor : EditorWindow
{
    private EnumField _modeField;

    private TextField _nameField;
    private IntegerField _repeatIndexField;
    private FloatField _distanceInput;
    //private FloatField _recoveryInput;

    private TextField _pathField;
    private Toggle _saveToggle;

    private ObjectField _assetField;
    private Vector2Field _pointPosField;
    private IntegerField _indexField;

    private VisualElement _customeMapPreview;
    private VisualElement _drawPreview;

    private Button _saveButton;
    private Button _clearButton;
    private Button _loadButton;
    private Button _pointApplyButton;
    private Button _rangeApplyButton;

    private Vector2Field _rangePosField;

    private VisualTreeAsset _crosshairPrefab;

    float _pointHeight = 25;
    float _pointWidth = 25;

    private float _ratioBetweenDistanceAndTarget = 0.5f;
    private float _previewSize = 800;

    bool _dragging;
    VisualElement _selectedPoint;

    LineDrawer _lineDrawer;

    VisualElement _pointBox;
    VisualElement _rangeBox;

    JsonAssetGenerator _jsonAssetGenerator = new JsonAssetGenerator();

    private RecoilType _recoilType = RecoilType.Map;

    private float _rangeModeStartPoint = -100;

    Dictionary<RecoilType, Action<VisualElement>> OnSelectedPointChangeRequested;
    Dictionary<RecoilType, Action> OnSaveRequested;
    Dictionary<RecoilType, Action> OnLoadRequested;

    Dictionary<RecoilType, Action> OnModeChangeRequested;


    [MenuItem("Tools/SprayEditor")]
    public static void OpenEditorWindow()
    {
        SprayEditor wnd = GetWindow<SprayEditor>();
        wnd.titleContent = new GUIContent("SprayEditor");
        wnd.maxSize = new Vector2(1200, 800);
        wnd.minSize = wnd.maxSize;
    }

    //private void Awake()
    //{
    //    Debug.Log(typeof(RecoilType).AssemblyQualifiedName);
    //}

    //public void OnGUI()
    //{
    //    Handles.BeginGUI();
    //    Handles.color = Color.red;
    //    Handles.DrawLine(new Vector3(0, 0), new Vector3(500, 1300));
    //    Handles.EndGUI();
    //}

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Map/SprayEditorWindow.uxml");

        _crosshairPrefab = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Point/PointWindow.uxml");
        //경로 설정

        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);

        _pointBox = root.Q<VisualElement>("point-box");
        _rangeBox = root.Q<VisualElement>("range-box");

        ////Assign Elements

        _modeField = root.Q<EnumField>("mode-field");
        _modeField.RegisterValueChangedCallback((ChangeEvent<Enum> evt) =>
        {
            _recoilType = (RecoilType)evt.newValue;
            OnModeChangeRequested[_recoilType]();
        });


        _nameField = root.Q<TextField>("name-field");
        _repeatIndexField = root.Q<IntegerField>("repeat-field");
        _distanceInput = root.Q<FloatField>("distance-field");

        //_recoveryInput = root.Q<FloatField>("recovery-duration-field");

        _pathField = root.Q<TextField>("path-field");
        _saveToggle = root.Q<Toggle>("save-toggle");
        _saveButton = root.Q<Button>("save-button");

        _assetField = root.Q<ObjectField>("asset-field");
        _loadButton = root.Q<Button>("load-button");

        _clearButton = root.Q<Button>("remove-button");

        _pointPosField = root.Q<Vector2Field>("point-pos-field");
        _indexField = root.Q<IntegerField>("index-field");

        _pointApplyButton = root.Q<Button>("point-apply-button");

        _customeMapPreview = root.Q<VisualElement>("point-preview");
        _drawPreview = root.Q<VisualElement>("draw-preview");


        _rangePosField = root.Q<Vector2Field>("range-pos-field");

        _rangeApplyButton = root.Q<Button>("range-apply-button");


        OnSelectedPointChangeRequested = new Dictionary<RecoilType, Action<VisualElement>>()
        {
            { RecoilType.Map, (VisualElement target) => {ResetPosFieldValue(_pointPosField, target); } },
            { RecoilType.Range, (VisualElement target) => {ResetPosFieldValue(_rangePosField, target); } }
        };

        OnSaveRequested = new Dictionary<RecoilType, Action>()
        {
            { RecoilType.Map, () => {SaveMap(); } },
            { RecoilType.Range, () => {SaveRange(); } }
        };

        OnLoadRequested = new Dictionary<RecoilType, Action>()
        {
            { RecoilType.Map, () => {LoadMap(); } },
            { RecoilType.Range, () => {LoadRange(); } }
        };

        OnModeChangeRequested = new Dictionary<RecoilType, Action>()
        {
            { RecoilType.Map, () => {InintializeMapMode(); } },
            { RecoilType.Range, () => {InitializeRangeMode(); } }
        };



        _saveButton.clicked += () => OnSaveRequested[_recoilType]();
        _loadButton.clicked += () => OnLoadRequested[_recoilType]();

        _clearButton.clicked += () => ClearMap();
        _pointApplyButton.clicked += () => ApplyPointData();

        _rangeApplyButton.clicked += () => ApplyRangeData();

        _customeMapPreview.RegisterCallback<ClickEvent>(OnCustomMapClick);

        _lineDrawer = new LineDrawer();
        _drawPreview.Add(_lineDrawer);
    }

    void InintializeMapMode()
    {
        _pointBox.style.display = DisplayStyle.Flex;
        _rangeBox.style.display = DisplayStyle.None;

        ClearPointAndLine();
    }

    void SpawnCenterAndPoint(Vector2 pointPosition)
    {
        SpawnPoint(new Vector2(_previewSize / 2, _previewSize / 2), false);
        VisualElement endPoint = SpawnPoint(pointPosition, true);
        ChangeSelectedPoint(endPoint);
    }

    void InitializeRangeMode()
    {
        _pointBox.style.display = DisplayStyle.None;
        _rangeBox.style.display = DisplayStyle.Flex;

        ClearPointAndLine();

        Vector2 pointPosition = new Vector2(_previewSize / 2, (_previewSize + _rangeModeStartPoint) / 2);
        SpawnCenterAndPoint(pointPosition);
        DrawPointLines();
    }

    #region Save And Load

    float ReturnRatioBetweenTargetAndDistanceInPixel()
    {
        return _distanceInput.value * _ratioBetweenDistanceAndTarget / _previewSize; // 1px당 실제 거리 비율
    }

    /// <summary>
    /// element를 받아서 중심 좌표를 리턴해준다.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    Vector2 ReturnCenterPositionOfPoint(VisualElement element)
    {
        return new Vector2(element.style.left.value.value - (_previewSize / 2) + (_pointWidth / 2), (element.style.top.value.value - (_previewSize / 2) + (_pointHeight / 2)));
    }


    /// <summary>
    /// position을 받아서 중심 좌표에 맞게 변환시켜준다.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Vector2 ReturnCenterPositionOfPoint(Vector2 pos)
    {
        return new Vector2(pos.x + (_previewSize / 2), (pos.y + (_previewSize / 2)));
    }

    private void SaveRange()
    {
        float ratio = ReturnRatioBetweenTargetAndDistanceInPixel();
        Vector2 centerPosition = ReturnCenterPositionOfPoint(_customeMapPreview[1]);

        RecoilRangeData tmpData = new RecoilRangeData(_nameField.value, _distanceInput.value, ratio, new SerializableVector2(centerPosition.x, centerPosition.y));
        _jsonAssetGenerator.CreateAndSaveJsonAsset(tmpData, _pathField.text, _nameField.text, _saveToggle.value);
    }

    T ReturnRecoilData<T>()
    {
        return _jsonAssetGenerator.JsonToObject<T>((TextAsset)_assetField.value);
    }

    private void LoadRange()
    {
        RecoilRangeData data = ReturnRecoilData<RecoilRangeData>();
        if (data == null) return;

        ClearMap(); // 모든 라인을 지워줌

        _nameField.value = data.Name;
        _distanceInput.value = data.DistanceFromTarget;

        //Vector2 pointPos = RevertAngleToPoint(data.Point);
        Vector2 pos = ReturnCenterPositionOfPoint(new Vector2(data.Point.x, data.Point.y));
        SpawnCenterAndPoint(pos);

        DrawPointLines(); // 선 생성시켜주기
    }

    // 아래 두 함수는 타입에 따라 달리 동작 해야한다. 
    private void SaveMap()
    {
        List<SerializableVector2> pointPosition = new List<SerializableVector2>();
        for (int i = 0; i < _customeMapPreview.childCount; i++)
        {
            Vector2 pos = ReturnCenterPositionOfPoint(_customeMapPreview[i]);
            SerializableVector2 position = new SerializableVector2(pos.x, pos.y);
            pointPosition.Add(position);
        }

        float ratio = ReturnRatioBetweenTargetAndDistanceInPixel();

        RecoilMapData tmpData = new RecoilMapData(_nameField.value, _distanceInput.value, ratio, _indexField.value, _repeatIndexField.value, pointPosition);
        _jsonAssetGenerator.CreateAndSaveJsonAsset(tmpData, _pathField.text, _nameField.text, _saveToggle.value);
    }

    private void LoadMap()
    {
        RecoilMapData data = ReturnRecoilData<RecoilMapData>();
        if (data == null) return;

        ClearMap(); // 모든 라인을 지워줌

        _nameField.value = data.Name;
        _repeatIndexField.value = data.RepeatIndex;
        _distanceInput.value = data.DistanceFromTarget;

        for (int i = 0; i < data.Points.Count; i++)
        {
            Vector2 pos = ReturnCenterPositionOfPoint(data.Points[i].V2);

            VisualElement point = SpawnPoint(pos);
            if (data.SelectedIndex == i) ChangeSelectedPoint(point);
        }

        DrawPointLines(); // 선 생성시켜주기
    }

    #endregion

    VisualElement SpawnPoint(Vector2 pos, bool canDrag = true)
    {
        VisualElement point = _crosshairPrefab.Instantiate();
        point.style.position = Position.Absolute;
        SetPointLableIndex(point, _customeMapPreview.childCount); // 인덱스 부여

        if (canDrag)
        {
            // --> 이렇게 3개를 넣어서 드래그 구현
            point.RegisterCallback<MouseDownEvent>(OnPointClick);
            point.RegisterCallback<MouseMoveEvent>(OnPointMove);
            point.RegisterCallback<MouseUpEvent>(OnPointUp);
        }

        SetPointPosition(point, pos);
        _customeMapPreview.Add(point); // 자식 오브젝트로 추가

        return point;
    }

    void ResetPointInfoField()
    {
        _pointPosField.value = Vector2.zero;
        _indexField.value = 0;
    }

    private void ClearPointAndLine()
    {
        _customeMapPreview.Clear();
        _lineDrawer.EraseAllLines();
    }

    private void ClearMap()
    {
        ClearPointAndLine();
        ResetPointInfoField();
    }

    #region Event

    private void OnCustomMapClick(ClickEvent evt)
    {
        if (_recoilType == RecoilType.Range) return; // Map 모드일 때만 적용시키기

        VisualElement point = SpawnPoint(evt.localPosition);

        ChangeSelectedPoint(point);
        DrawPointLines();
    }

    void DrawPointLines()
    {
        List<Vector2> pointPos = new List<Vector2>();
        for (int i = 0; i < _customeMapPreview.childCount; i++)
        {
            pointPos.Add(new Vector2(_customeMapPreview[i].style.left.value.value - (_previewSize / 2) + (_pointWidth / 2),
                _customeMapPreview[i].style.top.value.value - (_previewSize / 2) + (_pointHeight / 2)));
        }

        _lineDrawer.DrawUsingPoint(pointPos);
    }

    private void OnPointUp(MouseUpEvent evt)
    {
        _dragging = false;
    }

    private void OnPointMove(MouseMoveEvent evt)
    {
        if (!_dragging) return;
        if (_selectedPoint == null) return;

        Vector3 previewPos = _customeMapPreview.worldTransform.GetPosition();

        SetSelectedPointPosition(evt.mousePosition, previewPos);

        OnSelectedPointChangeRequested[_recoilType](_selectedPoint);

        DrawPointLines();
    }

    private void OnPointClick(MouseDownEvent evt)
    {
        VisualElement clickedTarget = (VisualElement)evt.currentTarget;

        if (evt.button == 2)
        {
            _dragging = true;

            ChangeSelectedPoint(clickedTarget);

            Vector3 previewPos = _customeMapPreview.worldTransform.GetPosition();
            SetSelectedPointPosition(evt.mousePosition, previewPos);

            DrawPointLines();
        }
        else if (evt.button == 1)
        {
            int clickedTargetIndex = _customeMapPreview.IndexOf(clickedTarget);

            _customeMapPreview.Remove(clickedTarget);
            ResetIndexOfPointTxtWhenDelete();

            DrawPointLines();

            if (_indexField.value == clickedTargetIndex)
            {
                ResetPointInfoField();

                if (_customeMapPreview.childCount >= 1)
                {
                    ChangeSelectedPoint(_customeMapPreview[_customeMapPreview.childCount - 1]);
                }
            }
            else
            {
                ChangeSelectedPoint(_selectedPoint);
            }
        }
    }

    #endregion

    #region EventFunction

    private void ApplyPointPosition(Vector2Field field)
    {
        float left = field.value.x + (_previewSize / 2);
        float top = field.value.y + (_previewSize / 2); // Y축은 반전시켜서 보여줘야함

        SetSelectedPointPosition(new Vector2(left, top));
    }

    private void ApplyRangeData()
    {
        ApplyPointPosition(_rangePosField);
        DrawPointLines();
    }

    private void ApplyPointData()
    {
        if (_selectedPoint == null) return;

        //float left = pointPosField.value.x + (previewSize / 2);
        //float top = -pointPosField.value.y + (previewSize / 2); // Y축은 반전시켜서 보여줘야함

        //SetSelectedPointPosition(new Vector2(left, top));

        ApplyPointPosition(_pointPosField);

        Label label = _selectedPoint.Q<Label>("index");
        if (label.text == _indexField.text)
        {
            DrawPointLines(); // x, y의 변경점을 지정해줌
            Debug.LogError("기존 인덱스와 같습니다.");
            return;
        }

        int index = int.Parse(_indexField.text);
        if (index > _customeMapPreview.childCount - 1)
        {
            Debug.LogError("인덱스가 너무 큽니다.");
            return;
        }

        _customeMapPreview.hierarchy.Remove(_selectedPoint);
        _customeMapPreview.hierarchy.Insert(index, _selectedPoint);

        ResetIndexOfPointTxtWhenDelete();
        DrawPointLines();
    }

    void ResetIndexOfPointTxtWhenDelete()
    {
        for (int i = 0; i < _customeMapPreview.childCount; i++)
        {
            Label lable = _customeMapPreview[i].Q<Label>("index");
            lable.text = i.ToString();
        }
    }

    void ChangeSelectedPoint(VisualElement target)
    {
        Label selectedLable = target.Q<Label>("index");

        for (int i = 0; i < _customeMapPreview.childCount; i++)
        {
            Label nowLable = _customeMapPreview[i].Q<Label>("index");

            if (nowLable == selectedLable) ActiveLableBorder(nowLable, true);
            else ActiveLableBorder(nowLable, false);
        }

        ResetSelectedPoint(target, selectedLable);
    }

    void ResetSelectedPoint(VisualElement target, Label selectedLable)
    {
        if (_selectedPoint != target) _selectedPoint = target;

        OnSelectedPointChangeRequested[_recoilType](_selectedPoint);
        _indexField.value = int.Parse(selectedLable.text); // 인덱스 필드도 초기화
    }

    void ResetPosFieldValue(Vector2Field field, VisualElement target)
    {
        float x = target.style.left.value.value + (_pointWidth / 2.0f) - (_previewSize / 2.0f); // x 값
        float y = target.style.top.value.value + (_pointHeight / 2.0f) - (_previewSize / 2.0f); // y 값

        field.value = new Vector2(x, y); // y축은 반전시켜서 보여줘야함
    }

    void SetPointLableIndex(VisualElement point, int index)
    {
        Label lable = point.Q<Label>("index");
        lable.text = index.ToString();
    }

    void SetPointPosition(VisualElement element, Vector2 pos)
    {
        element.style.top = pos.y - (_pointHeight / 2);
        element.style.left = pos.x - (_pointWidth / 2);
    }

    void SetSelectedPointPosition(Vector3 pos)
    {
        SetPointPosition(_selectedPoint, pos);
    }

    void SetSelectedPointPosition(Vector3 pos, Vector3 previewPos)
    {
        Vector3 newPos = new Vector3(pos.x - previewPos.x, pos.y - previewPos.y);
        SetPointPosition(_selectedPoint, newPos);
    }

    void ActiveLableBorder(Label lable, bool nowActive)
    {
        Color borderColor = Color.black;
        if (nowActive) borderColor = Color.cyan;

        lable.style.borderRightColor = borderColor;
        lable.style.borderLeftColor = borderColor;
        lable.style.borderTopColor = borderColor;
        lable.style.borderBottomColor = borderColor;
    }

    #endregion
}