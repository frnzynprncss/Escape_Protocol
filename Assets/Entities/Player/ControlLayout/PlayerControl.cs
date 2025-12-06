using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControl", menuName = "PlayerControl", order = 0)]
public class PlayerControl : ScriptableObject
{
    public KeyCode move_up;
    public KeyCode move_down;
    public KeyCode move_left;
    public KeyCode move_right;
    public KeyCode shoot;
    public KeyCode interact;
}