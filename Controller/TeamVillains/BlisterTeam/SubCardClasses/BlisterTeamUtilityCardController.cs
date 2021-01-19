using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class BlisterTeamUtilityCardController : CardController
    {

        public BlisterTeamUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string BlazingAxeIdentifier = "BlazingAxe";

        private IEnumerable<Card> FindBlazingAxe()
        {
            return base.FindCardsWhere(c => c.Identifier == BlazingAxeIdentifier);
        }
        protected Card FindBlazingAxeInPlay()
        {
            return FindBlazingAxe().Where(c => c.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected bool IsBlazingAxeInPlay()
        {
            return FindBlazingAxe().Where(c => c.IsInPlayAndHasGameText).Any();
        }



    }
}