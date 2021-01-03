using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class IAmTitaniumCardController : RockstarCardController
    {

        public IAmTitaniumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to {Rockstar} by 1.
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);

            //When {Rockstar} is dealt Melee or Projectile Damage, she may deal 1 damage of that type to one Non-Hero Target.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == CharacterCard && dd.DidDealDamage && (dd.DamageType == DamageType.Melee || dd.DamageType == DamageType.Projectile), DealCounterDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealCounterDamageResponse(DealDamageAction dd)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 1, dd.DamageType, 1, false, 0, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}