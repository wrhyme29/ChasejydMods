using TheDragonRune.MrNoble;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace TheDragonRuneTest
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("TheDragonRune", typeof(MrNobleCharacterCardController).Assembly);
        }
    }
}
