using NUnit.Framework;

public class BossBrainTests
{
    [Test]
    public void Start_SetsInitialState()
    {
        var context = new BossContext();
        var brain = new BossBrain(context);

        context.Brain = brain;

        brain.Start();

        Assert.Pass();
    }

    [Test]
    public void Update_CallsRageUpdate()
    {
        var rage = new BossRage();

        var context = new BossContext
        {
            Rage = rage
        };

        var brain = new BossBrain(context);
        context.Brain = brain;

        brain.Update();

        Assert.Pass();
    }
}