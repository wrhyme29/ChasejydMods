using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class FrictionTransferCardController : HeadlongCardController
    {

        public FrictionTransferCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Increase the next Damage dealt by {Headlong} by 2. 
            IncreaseDamageStatusEffect increaseEffect = new IncreaseDamageStatusEffect(2);
            increaseEffect.SourceCriteria.IsSpecificCard = CharacterCard;
            increaseEffect.NumberOfUses = 1;
            increaseEffect.CardFlippedExpiryCriteria.Card = CharacterCard;
            IEnumerator coroutine = AddStatusEffect(increaseEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Reduce the next Damage dealt to a Hero Target by 2.
            ReduceDamageStatusEffect reduceEffect = new ReduceDamageStatusEffect(2);
            reduceEffect.TargetCriteria.IsHero = true;
            reduceEffect.NumberOfUses = 1;
            coroutine = AddStatusEffect(reduceEffect);
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