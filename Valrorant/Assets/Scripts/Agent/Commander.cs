using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class Commander : MonoBehaviour
{
    [SerializeField] Helper _helperPrefab;
    [SerializeField] int _helperCnt = 1;
    [SerializeField] float _startRange = 10f;
    List<ICommandListener> _listeners = new List<ICommandListener>();

    Vector3 ReturnPlayerPos() { return transform.position; }

    public void Initialize()
    {
        // HelperFactory 구현 필요함
        //HelperViewer viewer =  FindObjectOfType<HelperViewer>();
        //viewer.

        CharacterPlant plant = FindObjectOfType<CharacterPlant>();

        for (int i = 0; i < _helperCnt; i++)
        {
            Vector2 startPos = Random.insideUnitCircle * _startRange;

            plant.Create(CharacterPlant.Name.Oryx);

            Helper swat = Instantiate(_helperPrefab, new Vector3(transform.position.x + startPos.y, transform.position.y, transform.position.z + startPos.x), Quaternion.identity);
            //swat.Initialize(ReturnPlayerPos);

            ICommandListener listener = swat.GetComponent<ICommandListener>();
            _listeners.Add(listener);
        }

        ResetFormationData();
    }

    public void ResetFormationData()
    {
        for (int i = 0; i < _listeners.Count; i++)
        {
            FormationData data = new FormationData(i + 1, _listeners.Count, _listeners);
            _listeners[i].ResetFormationData(data);
        }
    }

    public void BuildFormation()
    {
        for (int i = 0; i < _listeners.Count; i++)
        {
            _listeners[i].GoToBuildFormationState();
        }
    }

    public void FreeRole()
    {
        for (int i = 0; i < _listeners.Count; i++)
        {
            _listeners[i].GoToFreeRoleState();
        }
    }

    public void PickUpWeapon()
    {

    }

    public void SetPriorityTarget()
    {

    }
}
