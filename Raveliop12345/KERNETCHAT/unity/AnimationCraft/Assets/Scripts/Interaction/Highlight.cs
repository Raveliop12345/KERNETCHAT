using UnityEngine;

namespace AnimationCraft.Interaction
{
    public class Highlight : MonoBehaviour
    {
        public Color color = Color.yellow;
        public Vector3Int target;
        public bool visible;

        void OnDrawGizmos()
        {
            if (!visible) return;
            Gizmos.color = color;
            var p = (Vector3)target + Vector3.one * 0.5f;
            Gizmos.DrawWireCube(p, Vector3.one);
        }
    }
}
