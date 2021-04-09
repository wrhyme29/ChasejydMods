using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class DeeprootTeamCardController : CardController
    {

        public DeeprootTeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string PlantGrowthKeyword = "plant growth";

        protected bool IsPlantGrowth(Card card)
        {
            return card.DoKeywordsContain(PlantGrowthKeyword);
        }

        protected int GetNumberOfEnvironmentCardsInPlay()
        {
            return FindCardsWhere(c => c.IsEnvironment && c.IsInPlayAndHasGameText && c.IsRealCard).Count();
        }

        protected int GetNumberOfPlantGrowthCardsInPlay()
        {
            return FindCardsWhere(c => IsPlantGrowth(c) && c.IsInPlayAndHasGameText).Count();
        }

    }
}