using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class GigerMobilityChairCardController : SnareTeamCardController
    {

        public GigerMobilityChairCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to Giger Mobility Chair by 1.
            AddReduceDamageTrigger(c => c == Card, 1);

            //{Snare} and Giger Mobility Chair are immune to damage from environment cards.
            AddImmuneToDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsEnvironmentCard && dd.Target != null && (dd.Target == Card || dd.Target == CharacterCard));

            //At the end of {Snare}’s turn, {Snare} gains 1 HP.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(CharacterCard, 1, cardSource: GetCardSource()), TriggerType.GainHP);
        }

    }
}