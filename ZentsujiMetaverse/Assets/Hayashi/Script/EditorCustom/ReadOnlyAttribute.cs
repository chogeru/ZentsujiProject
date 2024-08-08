using UnityEditor;
using UnityEngine;

namespace AbubuResouse.Editor
{
    public class ReadOnlyAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
        public class ReadOnlyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                GUI.enabled = false; // �t�B�[���h��ǂݎ���p�ɂ��邽�߂�GUI�𖳌���
                EditorGUI.PropertyField(position, property, label);
                GUI.enabled = true; // �㑱�̃t�B�[���h������ɕ\�������悤��GUI��L����
            }
        }
#endif
    }
}
