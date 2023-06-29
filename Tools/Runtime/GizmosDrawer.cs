using NorskaLib.Extensions;
using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace NorskaLib.Tools
{
    public enum GizmosDrawerModes 
    { 
        OnSelected, 
        Always 
    }
    public enum GizmosDrawerShapes 
    { 
        Custom = -1,

        Cube, 
        Sphere, 
        Circle,
        Sector,
        Cross,
        Rectangle
    }
    public enum GizmosDrawerStyles 
    { 
        Solid,
        Wired 
    }

    public class GizmosDrawer : MonoBehaviour
    {
        public GizmosDrawerShapes shape;

        [FormerlySerializedAs("offset")]
        public Vector3 positionOffset;

        private bool ShowEulerOffsetEditor
            => shape == GizmosDrawerShapes.Custom;
        [ShowIf(nameof(ShowEulerOffsetEditor))]
        public Vector3 eulerOffset;

        private bool ShowRadiusEditor
            => shape == GizmosDrawerShapes.Sphere
            || shape == GizmosDrawerShapes.Circle
            || shape == GizmosDrawerShapes.Sector;
        [ShowIf(nameof(ShowRadiusEditor))]
        [MinValue(0)]
        public float radius = 1;

        // TO DO:
        private bool ShowRadiusInnerEditor
            => shape == GizmosDrawerShapes.Sector
            || shape == GizmosDrawerShapes.Circle;
        [ShowIf(nameof(ShowRadiusInnerEditor))]
        [MinValue(0)]
        public float radiusInner = 0.2f;

        private bool ShowSpanEditor
            => shape == GizmosDrawerShapes.Sector;
        [ShowIf(nameof(ShowSpanEditor))]
        public float span = 90;

        private bool ShowSizesEditor
            => shape == GizmosDrawerShapes.Cube
            || shape == GizmosDrawerShapes.Cross
            || shape == GizmosDrawerShapes.Custom
            || shape == GizmosDrawerShapes.Rectangle;
        [ShowIf(nameof(ShowSizesEditor)), LabelText("Size")]
        public Vector3 sizes = Vector3.one;

        private bool ShowMeshEditor 
            => shape == GizmosDrawerShapes.Custom;
        [ShowIf(nameof(ShowMeshEditor))]
        public Mesh[] meshes;

        [Space]

        public Color color = Color.blue.WithA(0.5f);

        // TO DO:
        // Add solid sector support
        private bool ShowStyleEditor
            => shape != GizmosDrawerShapes.Cross
            && shape != GizmosDrawerShapes.Sector;
        [ShowIf(nameof(ShowStyleEditor))]
        public GizmosDrawerStyles style;

        public GizmosDrawerModes mode;

        [Space]

        public bool drawForward;
        [EnableIf(nameof(drawForward))]
        public float forwardScale = 1;

        private Mesh bufferMesh;

        private void OnDrawGizmos()
        {
            if (mode != GizmosDrawerModes.Always)
                return;

            Draw();
        }

        private void OnDrawGizmosSelected()
        {
            if (mode != GizmosDrawerModes.OnSelected)
                return;

            Draw();
        }

        private void Draw()
        {
            Gizmos.color = color;

            if (bufferMesh == null)
                bufferMesh = new Mesh();

            if (drawForward)
                GizmosUtils.DrawStraitWireArrow(transform.position, transform.position + transform.forward * forwardScale);

            var position = transform.position + positionOffset;
            var rotation = transform.rotation * Quaternion.Euler(eulerOffset);
            var facing = MathUtils.AbsoluteSignedAngleXZ(this.transform);
            switch (shape)
            {
                case GizmosDrawerShapes.Custom:
                    switch (style)
                    {
                        case GizmosDrawerStyles.Wired:
                            if (meshes == null)
                                break;
                            foreach (var mesh in meshes)
                                Gizmos.DrawWireMesh(
                                    mesh, 
                                    position, 
                                    rotation,
                                    sizes);
                            break;
                        
                        default:
                        case GizmosDrawerStyles.Solid:
                            if (meshes == null)
                                break;
                            foreach (var mesh in meshes)
                                if (mesh != null)
                                    Gizmos.DrawMesh(
                                        mesh, 
                                        position, 
                                        rotation, 
                                        sizes);
                            break;
                    }
                    break;

                case GizmosDrawerShapes.Cube:
                    switch (style)
                    {
                        case GizmosDrawerStyles.Wired:
                            Gizmos.DrawWireCube(position, sizes);
                            break;
                        
                        default:
                        case GizmosDrawerStyles.Solid:
                            Gizmos.DrawCube(position, sizes);
                            break;
                    }
                    break;
                
                case GizmosDrawerShapes.Sphere:
                    switch (style)
                    {
                        default:
                        case GizmosDrawerStyles.Solid:
                            Gizmos.DrawSphere(position, radius);
                            break;
                        case GizmosDrawerStyles.Wired:
                            Gizmos.DrawWireSphere(position, radius);
                            break;
                    }
                    break;

                case GizmosDrawerShapes.Circle:
                    switch (style)
                    {
                        case GizmosDrawerStyles.Solid:
                            //if (radiusInner.ApproximatelyZero())
                                MeshUtils.BuildCircleMesh(bufferMesh, radius, 32);
                            //else
                            //    MeshUtils.BuildCircleMesh(bufferMesh, radius, radiusInner, 32);
                            Gizmos.DrawMesh(bufferMesh, position, rotation);
                            break;

                        default:
                        case GizmosDrawerStyles.Wired:
                            GizmosUtils.DrawWireCircle(position, radius, 32);
                            if (!radiusInner.ApproximatelyZero())
                                GizmosUtils.DrawWireCircle(position, radiusInner, 32);
                            break;
                    }
                    break;

                // TO DO:
                // Add solid sector support
                case GizmosDrawerShapes.Sector:
                    //switch (style)
                    //{
                    //    case GizmosDrawerStyles.Solid:
                    //        break;

                    //    case GizmosDrawerStyles.Wired:
                    //    default:
                            if (radiusInner.ApproximatelyZero())
                                GizmosUtils.DrawWireSector(position, facing, span, radius, 8);
                            else
                                GizmosUtils.DrawWireSector(position, facing, span, radiusInner, radius, 8);
                    //        break;
                    //}
                    break;

                case GizmosDrawerShapes.Cross:
                    GizmosUtils.DrawCrossPoint(position, sizes);
                    break;

                case GizmosDrawerShapes.Rectangle:
                    switch (style)
                    {
                        case GizmosDrawerStyles.Solid:
                            GizmosUtils.DrawSolidRectangle(position, sizes, transform.rotation, bufferMesh);
                            break;

                        default:
                        case GizmosDrawerStyles.Wired:
                            GizmosUtils.DrawWireRectangle(position, sizes.ToXZ(), transform.rotation);
                            break;
                    }
                    break;
            }
        }
    } 
}
