using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(CharacterRootMotion))]
public class CharacterRootMotionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Nueva lista
        var newAnimationsData = new List<AnimationCustomCurves>();

        // Script target
        var charRootMotion = target as CharacterRootMotion;

        // Animator y Controlador
        var animator = charRootMotion.gameObject.GetComponent<Animator>();
        var animController = animator.runtimeAnimatorController as UnityEditorInternal.AnimatorController;
        charRootMotion._animator = animator;

        // Por cada capa
        for (int layerIndex = 0; layerIndex < animController.layerCount; layerIndex++)
        {
            // Prefijo -> nombre de capa
            var prefix = animController.GetLayer(layerIndex).name + ".";

            // Extrae la maquina de estados
            var stateMachine = animController.GetLayer(layerIndex).stateMachine;

            // Agrega la data de la maquina
            newAnimationsData.AddRange(GetCurvesFromStateMachine(stateMachine, prefix));
        }

        // Reemplaza la lista
        charRootMotion._statesAnimationsCurves = newAnimationsData;
    }

    private List<AnimationCustomCurves> GetCurvesFromStateMachine(UnityEditorInternal.StateMachine stateMachine, string namePrefix)
    {
        var animCurvesList = new List<AnimationCustomCurves>();

        // Por cada estado de la maquina
        for (int stateIndex = 0; stateIndex < stateMachine.stateCount; stateIndex++)
        {
            //Extrae el estado y el animation clip
            var state = stateMachine.GetState(stateIndex);
            var clip = state.GetMotion() as AnimationClip;

            // Si no esta el clip ya en la lista lo agrega
            if (!animCurvesList.Exists(x => x.name == clip.name))
            {
                var deltaX = new AnimationCurve();
                var deltaY = new AnimationCurve();
                var deltaZ = new AnimationCurve();

                // Extrae las curvas
#pragma warning disable 612, 618
                foreach (var curveData in AnimationUtility.GetAllCurves(clip))
#pragma warning restore 612, 618
                {
                    if (curveData.propertyName == "deltaX")
                    {
                        deltaX = NormalizeCurveLength(curveData.curve, 1f);
                    }
                    else if (curveData.propertyName == "deltaY")
                    {
                        deltaY = NormalizeCurveLength(curveData.curve, 1f);
                    }
                    else if (curveData.propertyName == "deltaZ")
                    {
                        deltaZ = NormalizeCurveLength(curveData.curve, 1f);
                    }
                }

                // Agrega el nodo
                animCurvesList.Add(new AnimationCustomCurves(namePrefix + state.name, deltaX, deltaY, deltaZ));
            }
        }

        // Para cada sub maquina de estado
        for (int index = 0; index < stateMachine.stateMachineCount; index++)
        {
            // Extrae la maquina de estados
            var subStateMachine = stateMachine.GetStateMachine(index);

            // Agrega al prefijo el nombre de la sub maquina
            var subNamePrefix = subStateMachine.name + (".");

            // Extrae las curvas de la submaquina
            animCurvesList.AddRange(GetCurvesFromStateMachine(subStateMachine, subNamePrefix));
        }

        return animCurvesList;
    }

    private AnimationCurve NormalizeCurveLength(AnimationCurve curve, float length)
    {
        var newCurve = new AnimationCurve();
        if (curve.keys.Length > 0)
        {
            var actualLength = curve.keys[curve.keys.Length - 1].time;
            for (int i = 0; i < curve.keys.Length; i++)
            {
                var key = curve.keys[i];
                key.time = key.time / actualLength;
                newCurve.AddKey(key);
            }
        }
        return newCurve;
    }
}
