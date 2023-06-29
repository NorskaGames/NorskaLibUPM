using NorskaLib.Utilities;
using UnityEngine;

namespace NorskaLib.Tools.Demo
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
                GizmosUtils.DrawStraitWireArrow(startPoint.position, endPoint.position, arrowHeadSize);
            else
                GizmosUtils.DrawQuadWireArrow(startPoint.position, arcPoint.position, endPoint.position, arrowHeadSize, 8);
        }
    }
}