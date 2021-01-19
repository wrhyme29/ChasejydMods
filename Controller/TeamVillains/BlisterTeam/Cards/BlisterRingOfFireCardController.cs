using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class BlisterRingOfFireCardController : BlisterTeamUtilityCardController
    {

        public BlisterRingOfFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenDealtDamageThisTurn(CharacterCard), () => $"{CharacterCard.Title} has been dealt damage this turn.", () => $"{CharacterCard.Title} has not been dealt damage this turn.");
        }

        public override void AddTriggers()
        {
            //The first time each turn that {Blister} would be dealt damage, she first deals the source of that damage 2 Fire Damage.
            AddTrigger((DealDamageAction dd) => dd.Target != null && dd.Target == CharacterCard && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card.IsTarget && !HasBeenDealtDamageThisTurn(CharacterCard), FirstDamagePerTurnResponse, TriggerType.DealDamage, TriggerTiming.Before);
        }

        private IEnumerator FirstDamagePerTurnResponse(DealDamageAction dd)
        {
            //she first deals the source of that damage 2 Fire Damage.
            Card source = dd.DamageSource.Card;
            IEnumerator coroutine = DealDamage(CharacterCard, source, 2, DamageType.Fire, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}