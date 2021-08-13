using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class DeepRootsCardController : DeeprootTeamCardController
    {

        public DeepRootsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce Damage dealt to Environment Targets by 1.
            AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);

            //Increase damage from Environment Cards to Hero Targets by 1.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsEnvironmentCard && dd.Target != null && dd.Target.IsHero && GameController.IsCardVisibleToCardSource(dd.DamageSource.Card, GetCardSource()), 1);

        }


    }
}