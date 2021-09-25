using Core;

public class MagnetAbility : BaseAbility
{
    public override bool Execute()
    {
        if (!base.Execute())
        {
            return false;
        }

        FindObjectOfType<PlayerController>().Attract(Duration);

        return true;
    }
}
