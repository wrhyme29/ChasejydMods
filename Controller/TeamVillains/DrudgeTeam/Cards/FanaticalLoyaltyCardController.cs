using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class FanaticalLoyaltyCardController : DrudgeTeamCardController
    {

        public FanaticalLoyaltyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Increase Damage dealt by Thralls by 1.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.Card != null && IsThrall(dd.DamageSource.Card), 1);

            // Reduce Damage dealt to Thralls by 1.
            AddReduceDamageTrigger(c => IsThrall(c), 1);
        }


    }
}