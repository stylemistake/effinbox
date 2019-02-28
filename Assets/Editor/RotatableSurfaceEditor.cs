using UnityEditor;
using UnityEngine;

namespace Effinbox {

  [CustomEditor(typeof(RotatableSurface)), CanEditMultipleObjects]
  public class RotatableSurfaceEditor: Editor {

    protected virtual void OnSceneGUI() {
      RotatableSurface item = (RotatableSurface) target;
      var tfm = item.transform;
      EditorGUI.BeginChangeCheck();
      var pivotPoint1 = Handles.PositionHandle(
        tfm.TransformPoint(item.pivotPoint1),
        Quaternion.identity);
      var pivotPoint2 = Handles.PositionHandle(
        tfm.TransformPoint(item.pivotPoint2),
        Quaternion.identity);
      if (EditorGUI.EndChangeCheck()) {
        Undo.RecordObject(item, "Change Look At Target Position");
        item.pivotPoint1 = tfm.InverseTransformPoint(pivotPoint1);
        item.pivotPoint2 = tfm.InverseTransformPoint(pivotPoint2);
      }
    }

  }

}
