using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class RockstarCardController : CardController
    {

        public RockstarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		protected bool IsStagePresence(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "stage presence");
		}

	
		protected int GetNumberOfStagePresenceInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsStagePresence(c)).Count();
		}

	}
}