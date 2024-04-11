// Designed by KINEMATION, 2023

using Kinemation.FPSFramework.Runtime.Core.Components;
using Kinemation.FPSFramework.Runtime.Core.Types;
using UnityEngine;

namespace Kinemation.FPSFramework.Runtime.Layers
{
    public class ViewmodelLayer : AnimLayer
    {
        public override void UpdateLayer()
        {
            LocRot offset = GetGunAsset().viewmodelOffset.poseOffset;
            GetMasterIK().Offset(GetRootBone(), offset.position / 100f);
            GetMasterIK().Offset(GetRootBone(), offset.rotation, GetRigData().aimWeight);

            offset = GetGunAsset().viewmodelOffset.rightHandOffset;
            Vector3 curveAnimation = new Vector3()
            {
                x = GetCurveValue(CurveLib.Curve_IK_RightHand_X),
                y = GetCurveValue(CurveLib.Curve_IK_RightHand_Y),
                z = GetCurveValue(CurveLib.Curve_IK_RightHand_Z)
            };

            offset.position += curveAnimation;
            
            GetRightHandIK().Offset(GetMasterPivot(), offset.position / 100f);
            GetRightHandIK().Offset(GetMasterPivot(), offset.rotation);
            
            offset = GetGunAsset().viewmodelOffset.leftHandOffset;
            GetLeftHandIK().Offset(GetMasterPivot(), offset.position / 100f);
            GetLeftHandIK().Offset(GetMasterPivot(), offset.rotation);
        }
    }
}
