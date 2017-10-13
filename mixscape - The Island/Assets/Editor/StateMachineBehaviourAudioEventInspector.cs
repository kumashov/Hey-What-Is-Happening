using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StateMachineBehaviourAudioEvent))]
public class StateMachineBehaviourAudioEventInspector : AkBaseInspector
{
    SerializedProperty eventID;
    
    public void OnEnable()
    {
        if(target != null)
        {
            eventID				= serializedObject.FindProperty("eventID");
            m_guidProperty = new SerializedProperty[1];
            m_guidProperty[0] = serializedObject.FindProperty("valueGuid.Array");
        }

        //Needed by the base class to know which type of component its working with
        m_typeName		= "Event";
        m_objectType	= AkWwiseProjectData.WwiseObjectType.EVENT;
    }

    public override void OnChildInspectorGUI ()
    {
        if(target != null)
        {
            serializedObject.Update ();
		
            GUILayout.Space(2);

            serializedObject.ApplyModifiedProperties ();
        }
    }

    public override string UpdateIds (Guid[] in_guid)
    {
        for(int i = 0; i < AkWwiseProjectInfo.GetData().EventWwu.Count; i++)
        {
            AkWwiseProjectData.Event e = AkWwiseProjectInfo.GetData().EventWwu[i].List.Find(x => new Guid(x.Guid).Equals(in_guid[0]));
		
            if(e != null)
            {
                if(target != null)
                {
                    serializedObject.Update();
                    eventID.intValue = e.ID;
                    serializedObject.ApplyModifiedProperties();
                }

                return e.Name;
            }
        }

        return string.Empty;
    }
}