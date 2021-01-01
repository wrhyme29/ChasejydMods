using Chasejyd.Rockstar;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace ChasejydTests
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("Chasejyd", typeof(RockstarCharacterCardController).Assembly);
        }
    }
}
