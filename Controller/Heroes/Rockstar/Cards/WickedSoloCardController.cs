using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class WickedSoloCardController : RockstarCardController
    {

        public WickedSoloCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Rockstar} deals one target 2 melee damage. 
            int powerNumeral = GetPowerNumeral(0, 2);
            int powerNumeral2 = GetPowerNumeral(1, 1);

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), powerNumeral, DamageType.Melee, 1, false, 1, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidDealDamage(storedResults))
            {
                Card target = storedResults.First().Target;
                //If a target takes damage this way, redirect all damage dealt by that target to { Rockstar} and reduce damage dealt by that target by 1 until the start of your next turn.
                RedirectDamageStatusEffect redirectEffect = new RedirectDamageStatusEffect();
                redirectEffect.SourceCriteria.IsSpecificCard = target;
                redirectEffect.UntilStartOfNextTurn(TurnTaker);
                redirectEffect.RedirectTarget = CharacterCard;
                redirectEffect.UntilCardLeavesPlay(target);
                redirectEffect.UntilTargetLeavesPlay(target);
                redirectEffect.TargetRemovedExpiryCriteria.Card = target;
                IEnumerator redirectCoroutine = AddStatusEffect(redirectEffect);

                ReduceDamageStatusEffect reduceEffect = new ReduceDamageStatusEffect(1);
                reduceEffect.SourceCriteria.IsSpecificCard = target;
                reduceEffect.UntilStartOfNextTurn(TurnTaker);
                reduceEffect.UntilCardLeavesPlay(target);
                reduceEffect.UntilTargetLeavesPlay(target);
                reduceEffect.TargetRemovedExpiryCriteria.Card = target;
                IEnumerator reduceCoroutine = AddStatusEffect(reduceEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(redirectCoroutine);
                    yield return base.GameController.StartCoroutine(reduceCoroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(redirectCoroutine);
                    base.GameController.ExhaustCoroutine(reduceCoroutine);
                }
            }
        }
    }
}