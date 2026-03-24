using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using static UnityEditor.AnimationUtility;

[Serializable]
public class SerializableCurve
{
    public List<SerializableKeyframe> keys;
    public int preWrapMode;
    public int postWrapMode;
}

[Serializable]
public class SerializableKeyframe
{
    public float time;
    public float value;
    public float inTangent;
    public float outTangent;
    public int leftTangentMode;
    public int rightTangentMode;
    public float inWeight;
    public float outWeight;
    public int weightedMode;
}
[Serializable]
public class BTTargetAnimaCurve
{
    [HideInInspector] public SerializableCurve animaCurveData;
    public AnimationCurve curve;

    public void SerializeSelf()
    {
        animaCurveData = new SerializableCurve();
        curve ??= new AnimationCurve();
        animaCurveData.preWrapMode = (int)curve.preWrapMode;
        animaCurveData.postWrapMode = (int)curve.postWrapMode;
        animaCurveData.keys = curve.keys.Select(k =>
        {
            var sk = new SerializableKeyframe
            {
                time = k.time,
                value = k.value,
                inTangent = k.inTangent,
                outTangent = k.outTangent,
                inWeight = k.inWeight,
                outWeight = k.outWeight,
                weightedMode = (int)k.weightedMode
            };
            int index = curve.keys.ToList().IndexOf(k);
            sk.leftTangentMode = (int)GetKeyLeftTangentMode(curve, index);
            sk.rightTangentMode = (int)GetKeyRightTangentMode(curve, index);

            return sk;
        }).ToList();
    }
    public void SetAnimationCurve()
    {
        animaCurveData ??= new SerializableCurve();
        curve = ConvertToCurve(animaCurveData);
    }
    private AnimationCurve ConvertToCurve(SerializableCurve data)
    {
        var curve = new AnimationCurve();
        curve.preWrapMode = (WrapMode)data.preWrapMode;
        curve.postWrapMode = (WrapMode)data.postWrapMode;
        data.keys ??= new List<SerializableKeyframe>();
        for (int i = 0; i < data.keys.Count; i++)
        {
            var k = data.keys[i];
            var key = new Keyframe(k.time, k.value)
            {
                inTangent = k.inTangent,
                outTangent = k.outTangent,
                inWeight = k.inWeight,
                outWeight = k.outWeight,
                weightedMode = (WeightedMode)k.weightedMode
            };
            curve.AddKey(key);

            // ��ԭ������ģʽ
            AnimationUtility.SetKeyLeftTangentMode(
                curve, i, (TangentMode)k.leftTangentMode
            );

            // ��ԭ������ģʽ
            AnimationUtility.SetKeyRightTangentMode(
                curve, i, (TangentMode)k.leftTangentMode
            );
        }
        return curve;
    }


}
