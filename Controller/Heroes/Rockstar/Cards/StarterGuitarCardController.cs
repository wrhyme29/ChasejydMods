using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class StarterGuitarCardController : RockstarCardController
    {

        public StarterGuitarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Deal 1 Target 1 Sonic and 1 Melee Damage. 
            int powerNumeral = GetPowerNumeral(0, 1);
            int powerNumeral2 = GetPowerNumeral(1, 1);
            int powerNumeral3 = GetPowerNumeral(2, 1);
            List<DealDamageAction> list = new List<DealDamageAction>();
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, CharacterCard), null, powerNumeral2, DamageType.Sonic));
            list.Add(new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, CharacterCard), null, powerNumeral3, DamageType.Melee));
            IEnumerator coroutine = SelectTargetsAndDealMultipleInstancesOfDamage(list, null, null, powerNumeral, powerNumeral);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Until the start of your next turn treat this card as a 1 HP Hero Target.
            MakeTargetStatusEffect makeTargetStatusEffect = new MakeTargetStatusEffect(1);
            makeTargetStatusEffect.CardsToMakeTargets.IsSpecificCard = Card;
            makeTargetStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine2 = AddStatusEffect(makeTargetStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is Indestructible.
            return card == Card;
        }
    }
}