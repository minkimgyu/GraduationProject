using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRecoilGenerator : RecoilStrategy
{
    public override void OnClickStart() { }

    public override void OnClickEnd() { }

    public override void OnEventRequested() { }

    public override void OnOtherActionEventRequested() { }

    protected override Vector2 ReturnNextRecoilPoint() { return default(Vector2); }

    public override void OnLink(GameObject player) { }

    public override void OnUnlink(GameObject player) { }

    public override void OnInintialize(GameObject player) { }

    public override void OnEventFinished()
    {
    }

    public override void RecoverRecoil()
    {
    }

    public override void ResetRecoil()
    {
    }
}
