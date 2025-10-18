public class MannequinAI : GhostAI
{
    public override void light_collided()
    {
        looking_for_player = false;
    }
}
