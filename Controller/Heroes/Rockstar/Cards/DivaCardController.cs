using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class DivaCardController : RockstarCardController
    {

        public DivaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override bool AskIfCardMayPreventAction<T>(TurnTakerController ttc, CardController preventer)
        {
            //Non-Hero cards cannot prevent {Rockstar} from using Powers.
            if (typeof(T) == typeof(UsePowerAction) && ttc == TurnTakerController && !preventer.Card.IsHero)
            {
                return false;
            }
            return true;
        }

    }
}