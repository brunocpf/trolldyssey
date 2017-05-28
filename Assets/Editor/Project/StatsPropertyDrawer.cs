/*

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitData))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (target.GetType() == typeof(UnitData))
        {
            UnitData unitData = (UnitData) target;

            unitData.initialStats.level.baseValue = unitData.initialStats.mhp._baseValue;
            unitData.initialStats.exp.baseValue = unitData.initialStats.exp._baseValue;
            unitData.initialStats.mhp.baseValue = unitData.initialStats.mhp._baseValue;
            //unitData.baseValue = unit.publicVariable;
        }

        // Take out this if statement to set the value using setter when ever you change it in the inspector.
        // But then it gets called a couple of times when ever inspector updates
        // By having a button, you can control when the value goes through the setter and getter, your self.
        if (GUILayout.Button("Use setters/getters"))
        {
            if (target.GetType() == typeof(UseGetterSetter))
            {
                UseGetterSetter getterSetter = (UseGetterSetter)target;
                getterSetter.PublicVariableProperty = getterSetter.publicVariable;
                Debug.Log(getterSetter.PublicVariableProperty);
            }
        }
    }
}
*/