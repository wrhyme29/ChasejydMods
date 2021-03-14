using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class HeadlongCardController : CardController
    {

        public HeadlongCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string MomentumKeyword = "momentum";

        protected bool IsMomentum(Card card)
        {
            return card.DoKeywordsContain(MomentumKeyword);
        }

        protected int GetNumberOfEnvironmentCardsInPlay()
        {
            return FindCardsWhere(c => c.IsEnvironment && c.IsInPlayAndHasGameText && c.IsRealCard).Count();
        }

    }
}