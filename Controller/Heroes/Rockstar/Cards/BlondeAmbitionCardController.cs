using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
    public class BlondeAmbitionCardController : RockstarCardController
    {

        public BlondeAmbitionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal one Target 2 Melee damage. Deal a second target 2 Projectile Damage. 

            List<DealDamageAction> targets = new List<DealDamageAction>();
            IEnumerator firstDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Melee, new int?(1), false, new int?(1), storedResultsDamage: targets, addStatusEffect: RemoveEndOfTurnResponse, cardSource: GetCardSource());
            IEnumerator secondDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Projectile, new int?(1), false, new int?(1), additionalCriteria: (Card c) => !(from d in targets
                                                                                                                                                                                                                                                                  select d.Target).Contains(c), addStatusEffect: RemoveEndOfTurnResponse, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(firstDamage);
                yield return base.GameController.StartCoroutine(secondDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(firstDamage);
                base.GameController.ExhaustCoroutine(secondDamage);
            }
           
            yield break;
        }

        private IEnumerator RemoveEndOfTurnResponse(DealDamageAction dd)
        {
            //Non-character-card targets dealt damage this way may not use any End of Turn abilities until the start of {Rockstar}'s next turn.

            if (dd != null && dd.DidDealDamage && dd.Target != null && !dd.Target.IsCharacter)
            {
                PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect();
                preventPhaseEffectStatusEffect.UntilStartOfNextTurn(TurnTaker);
                preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = dd.Target;
                IEnumerator coroutine = AddStatusEffect(preventPhaseEffectStatusEffect);
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
}