using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class ImmortalFormCardController : DrudgeTeamCardController
    {

        public ImmortalFormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            AddWhenHPDropsToZeroOrBelowRestoreHPTriggers(() => CharacterCard, () => 5, destroyThisCardAfterwards: true);
        }

    }
}