using Core;

public class SlowGravityAbility : BaseAbility
{
    public override bool Execute()
    {
        if (!base.Execute())
        {
            return false;
        }

        FindObjectOfType<PlayerController>().SlowGravity(Duration);

        return true;
    }
}
