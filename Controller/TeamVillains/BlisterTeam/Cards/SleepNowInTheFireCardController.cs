using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class SleepNowInTheFireCardController : BlisterTeamUtilityCardController
    {

        public SleepNowInTheFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDealingFire);
        }

        public static readonly string FirstTimeDealingFire = "FirstTimeDealingFire";

        public override void AddTriggers()
        {
            //The first time each turn that {Blister} deals Fire Damage, she also heals HP equal to the amount of Fire Damage dealt.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == CharacterCard && dd.DamageType == DamageType.Fire && dd.DidDealDamage && !HasBeenSetToTrueThisTurn(FirstTimeDealingFire), FirstTimeDealingFireResponse, TriggerType.GainHP, TriggerTiming.After);
        }

        private IEnumerator FirstTimeDealingFireResponse(DealDamageAction dd)
        {
            //she also heals HP equal to the amount of Fire Damage dealt.
            SetCardPropertyToTrueIfRealAction(FirstTimeDealingFire);
            IEnumerator coroutine = GameController.GainHP(CharacterCard, dd.Amount, cardSource: GetCardSource());
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