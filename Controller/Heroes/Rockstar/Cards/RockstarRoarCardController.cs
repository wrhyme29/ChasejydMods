using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class RockstarRoarCardController : RockstarCardController
    {

        public RockstarRoarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override bool AskIfCardMayPreventAction<T>(TurnTakerController ttc, CardController preventer)
        {
            //Non-Hero Cards cannot prevent {Rockstar} from playing cards..
            if (typeof(T) == typeof(PlayCardAction) && ttc == TurnTakerController && !preventer.Card.IsHero)
            {
                return false;
            }
            return true;
        }

    }
}