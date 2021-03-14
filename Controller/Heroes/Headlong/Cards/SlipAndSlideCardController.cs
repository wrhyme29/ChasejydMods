using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Headlong
{
    public class SlipAndSlideCardController : HeadlongCardController
    {

        public SlipAndSlideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal one Target 2 Projectile Damage. 

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 2, DamageType.Projectile, 1, false, 1, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If a Target takes Damage this way, that Target Deals a Non-Hero Target 3 Melee Damage.

            if (DidDealDamage(storedResults))
            {
                DealDamageAction dd = storedResults.First();
                if (!dd.Target.Owner.IsIncapacitatedOrOutOfGame && dd.Target.IsInPlayAndHasGameText)
                {
                    HeroTurnTakerController httc = dd.Target.IsHeroCharacterCard ? FindHeroTurnTakerController(dd.Target.Owner.ToHero()) : DecisionMaker;
                    coroutine = GameController.SelectTargetsAndDealDamage(httc, new DamageSource(GameController, dd.Target), 3, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource());
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
            yield break;
        }


    }
}