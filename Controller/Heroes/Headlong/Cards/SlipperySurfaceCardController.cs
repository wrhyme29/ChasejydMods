using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class SlipperySurfaceCardController : HeadlongCardController
    {

        public SlipperySurfaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to Hero Targets from Environment Cards by 1. 
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsEnvironment && dd.Target.IsHero, dd => 1);
            //Increase Damage dealt to Villain Targets by Environment Cards by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsEnvironment && IsVillainTarget(dd.Target), 1);
        }


    }
}