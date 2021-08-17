using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class VampiricAuraCardController : DrudgeTeamCardController
    {

        public VampiricAuraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Hero Targets damaged by {DrudgeTeam} cannot gain HP until the start of {DrudgeTeam}'s next turn.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.Target != null && dd.Target.IsHero, CannotGainHPResponse, TriggerType.AddStatusEffectToDamage, TriggerTiming.After);
        }

        private IEnumerator CannotGainHPResponse(DealDamageAction dd)
        {
            if(!dd.DidDealDamage || dd.DidDestroyTarget)
            {
                yield break;
            }

            CannotGainHPStatusEffect statusEffect = new CannotGainHPStatusEffect();
            statusEffect.CardSource = Card;
            statusEffect.TargetCriteria.IsSpecificCard = dd.Target;
            statusEffect.UntilStartOfNextTurn(TurnTaker);
            IEnumerator coroutine = AddStatusEffect(statusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}