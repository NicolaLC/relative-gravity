using Core;

public class LockWormholesAbility : BaseAbility
{
    public override bool Execute()
    {
        if (!base.Execute())
        {
            return false;
        }
        foreach (WormHoleEnemyController enemyController in FindObjectsOfType<WormHoleEnemyController>())
        {
            enemyController.Freeze(Duration);
        }

        return true;
    }
}
