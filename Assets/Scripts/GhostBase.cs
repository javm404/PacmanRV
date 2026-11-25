using UnityEngine;

public class GhostBase : MonoBehaviour
{
    public bool tagged = false;
    public enum ghostState { deciding, moving, attacking, tackling }
    public ghostState state;
}