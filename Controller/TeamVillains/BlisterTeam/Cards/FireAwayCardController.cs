using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class FireAwayCardController : BlisterTeamUtilityCardController
    {

        public FireAwayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 3);
        }

        public override IEnumerator Play()
        {
            //Deal the 3 Non-Villain Targets with the Highest HP 2 Fire Damage each.
            IEnumerator coroutine = DealDamageToHighestHP(Card, 1, (Card c) => !IsVillainTarget(c), (Card c) => 2, DamageType.Fire, numberOfTargets: () => 3);
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