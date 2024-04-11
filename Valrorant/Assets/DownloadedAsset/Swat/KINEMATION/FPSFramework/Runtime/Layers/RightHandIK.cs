// Designed by KINEMATION, 2023

using Kinemation.FPSFramework.Runtime.Core.Components;
using Kinemation.FPSFramework.Runtime.Core.Types;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class RightHandIK : AnimLayer
    {
        public override void UpdateLayer()
        {
            LocRot offset = GetGunAsset().viewmodelOffset.rightHandOffset;
            
            Vector3 curveAnimation = new Vector3()
            {
                x = GetCurveValue(CurveLib.Curve_IK_RightHand_X),
                y = GetCurveValue(CurveLib.Curve_IK_RightHand_Y),
                z = GetCurveValue(CurveLib.Curve_IK_RightHand_Z)
            };

            offset.position += curveAnimation;
            
            GetRightHandIK().Offset(GetMasterPivot(), offset.position, smoothLayerAlpha);
            GetRightHandIK().Offset(GetMasterPivot(), offset.rotation, smoothLayerAlpha);
        }
    }
}
