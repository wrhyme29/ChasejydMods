using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class SnareTeamCardController : CardController
    {

        public SnareTeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string BarrierKeyword = "barrier";

        protected bool IsBarrier(Card card)
        {
            return card.DoKeywordsContain(BarrierKeyword);
        }

    }
}