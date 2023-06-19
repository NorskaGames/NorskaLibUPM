using NorskaLib.Extensions;
using NorskaLib.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

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
        Cross
    }
    public enum GizmosDrawerStyles 
    { 
        Solid,
        Wired 
    }

    public class GizmosDrawer : MonoBehaviour
    {
        public GizmosDrawerShapes shape;

        private bool showRadius
            => shape == GizmosDrawerShapes.Sphere
            || shape == GizmosDrawerShapes.Circle
            || shape == GizmosDrawerShapes.Sector;
        [ShowIf(nameof(showRadius))]
        public float radius = 1;

        private bool showSpawn
            => shape == GizmosDrawerShapes.Sector;
        [ShowIf(nameof(showSpawn))]
        public float span = 90;

        private bool showSizes
            => shape == GizmosDrawerShapes.Cube
            || shape == GizmosDrawerShapes.Cross
            || shape == GizmosDrawerShapes.Custom;
        [ShowIf(nameof(showSizes)), LabelText("Size")]
        public Vector3 sizes = Vector3.one;

        private bool showMesh 
            => shape == GizmosDrawerShapes.Custom;
        [ShowIf(nameof(showMesh))]
        public Mesh[] meshes;

        [Space]

        public Color color = Color.blue.WithA(0.5f);

        private bool showStyle
            => shape == GizmosDrawerShapes.Custom
            || shape == GizmosDrawerShapes.Cube
            || shape == GizmosDrawerShapes.Sphere;
        [ShowIf(nameof(showStyle))]
        public GizmosDrawerStyles style;

        public GizmosDrawerModes mode;
        public Vector3 offset;

        [Space]

        public bool drawForward;

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

        void Draw()
        {
            Gizmos.color = color;

            if (drawForward)
            {
                Gizmos.DrawLine(transform.position, transform.position + transform.forward);
            }

            var position = transform.position + offset;
            var rotation = transform.rotation;
            var facing = MathUtils.AbsoluteSignedAngleXZ(this.transform);
            switch (shape)
            {
                case GizmosDrawerShapes.Custom:
                    switch (style)
                    {
                        case GizmosDrawerStyles.Wired:
                            foreach (var mesh in meshes)
                                Gizmos.DrawWireMesh(
                                    mesh, 
                                    position, 
                                    rotation,
                                    sizes);
                            break;
                        
                        default:
                        case GizmosDrawerStyles.Solid:
                            foreach (var mesh in meshes)
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
                    GizmosUtils.DrawCircle(position, radius, 16);
                    break;

                case GizmosDrawerShapes.Sector:
                    GizmosUtils.DrawSector(position, facing, span, radius, 8);
                    break;

                case GizmosDrawerShapes.Cross:
                    GizmosUtils.DrawCrossPoint(position, sizes);
                    break;
            }
        }
    } 
}
