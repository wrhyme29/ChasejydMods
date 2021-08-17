using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class FiendishGuidanceCardController : DrudgeTeamCardController
    {

        public FiendishGuidanceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of {DrudgeTeam}'s turn, each Villain Character gains 1 HP and all Thralls in play gain 2 HP.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => c.IsVillainTeamCharacter, 1, cardSource: GetCardSource()), TriggerType.GainHP);
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => IsThrall(c), 2, cardSource: GetCardSource()), TriggerType.GainHP);

            //{DrudgeTeam} is Immune to damage from Villain cards.
            AddImmuneToDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsVillain && dd.Target != null && dd.Target == CharacterCard);
        }
    }
}