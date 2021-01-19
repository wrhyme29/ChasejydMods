using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.BlisterTeam
{
    public class BlazingAxeCardController : BlisterTeamUtilityCardController
    {

        public BlazingAxeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            
            return card == Card && CharacterCardController.IsGameAdvanced && !CharacterCard.IsFlipped;
        }

        public override void AddTriggers()
        {
            //Increase Damage Dealt by {Blister} by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard), 1);

            //Damage from {Blister} cannot be redirected.
            AddTrigger((RedirectDamageAction rd) => rd.DealDamageAction != null && rd.DealDamageAction.DamageSource != null && rd.DealDamageAction.DamageSource.IsSameCard(CharacterCard), (RedirectDamageAction rd) => CancelAction(rd), TriggerType.CancelAction, TriggerTiming.Before);
            AddTrigger((MakeDecisionAction md) => GetDecisionCriteria(md),
                (MakeDecisionAction md) => CancelAction(md), TriggerType.CancelAction, TriggerTiming.Before);
        }

        private bool GetDecisionCriteria(MakeDecisionAction md)
        {
            return md.Decision != null && md.Decision.IsRedirectDecision && md.Decision.GameAction != null && md.Decision.GameAction is DealDamageAction dd && dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard);
        }
    }
}