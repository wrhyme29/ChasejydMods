using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class ScreamOfAngerCardController : ScreechTeamCardController
    {

        public ScreamOfAngerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //{Screech} deals the Non-Villain Target with the highest HP 3 Sonic Damage. If a Target takes damage this way, reduce Damage dealt by that Target by 1 until the start of {Screech}'s next turn.
            return DealDamageToHighestHP(CharacterCard, 1, c => !IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Sonic, addStatusEffect: dd => ReduceDamageDealtByThatTargetIfDamagedUntilTheStartOfYourNextTurnResponse(dd, 1));
        }

        protected IEnumerator ReduceDamageDealtByThatTargetIfDamagedUntilTheStartOfYourNextTurnResponse(DealDamageAction dd, int amount)
        {
            if (!dd.DidDealDamage)
            {
                yield break;
            }


            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
            reduceDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
            reduceDamageStatusEffect.UntilTargetLeavesPlay(dd.Target);
            
            IEnumerator coroutine = AddStatusEffect(reduceDamageStatusEffect);
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