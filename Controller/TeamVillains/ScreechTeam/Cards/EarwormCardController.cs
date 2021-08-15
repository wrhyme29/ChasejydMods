using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class EarwormCardController : ScreechTeamCardController
    {

        public EarwormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of {Screech}'s turn, {Screech} deals each Hero Target 1 Sonic Damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => c.IsHero && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Sonic);
        }
    }
}