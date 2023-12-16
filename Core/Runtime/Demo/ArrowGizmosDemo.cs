using UnityEngine;

namespace NorskaLib.Utilities.Demo
{
    public class ArrowGizmosDemo : MonoBehaviour
	{
		[SerializeField] Transform startPoint;
		[SerializeField] Transform endPoint;
		[SerializeField] Transform arcPoint;

        [SerializeField] Color color = Color.red;
        [SerializeField] Vector2 arrowHeadSize = new Vector2(0.2f, 0.3f);

        private void OnDrawGizmos()
        {
            Gizmos.color = color;

            if (startPoint == null || endPoint == null)
                return;

            if (arcPoint == null)
                GizmosUtils.DrawWireArrow(startPoint.position, endPoint.position, arrowHeadSize);
            else
                GizmosUtils.DrawWireArrow(startPoint.position, arcPoint.position, endPoint.position, arrowHeadSize, 8);
        }
    }
}