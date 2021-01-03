using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class OwnTheStageCardController : StagePresenceCardController
    {

        public OwnTheStageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage to {Rockstar} from the environment by 2.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard, 2, null, targetCriteria: (Card c) => c == CharacterCard);

            //You may redirect damage from the environment to {Rockstar}.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard, () => CharacterCard, optional: true);
        }


    }
}