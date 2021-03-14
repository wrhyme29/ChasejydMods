using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class SpinOutCardController : HeadlongCardController
    {
        public SpinOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Headlong} deals 1 Non-Hero Target 4 Melee Damage. Then he may deal a second Non-Hero Target 3 Projectile Damage.

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 4, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, storedResultsDamage: storedResults, cardSource: GetCardSource());
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
                Card damagedTarget = storedResults.First().Target;
                coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 3, DamageType.Projectile, 1, false, 0, additionalCriteria: (Card c) => c != damagedTarget && !c.IsHero && c.IsTarget, storedResultsDamage: storedResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }


    }
}