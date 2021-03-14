using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class PerfectPlacementCardController : HeadlongCardController
    {

        public PerfectPlacementCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal a Hero Character 2 Projectile Damage. 
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 2, DamageType.Projectile, 1, false, 1, additionalCriteria: (Card c) => c.IsHeroCharacterCard, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If a Hero takes Damage this way, that Hero may Play a Card. 
            if(DidDealDamage(storedResults))
            {
                DealDamageAction dd = storedResults.First();
                if(dd.Target.IsHeroCharacterCard && !dd.Target.Owner.IsIncapacitatedOrOutOfGame)
                {
                    HeroTurnTakerController httc = FindHeroTurnTakerController(dd.Target.Owner.ToHero());
                    coroutine = GameController.SelectAndPlayCardFromHand(httc, optional: true, cardSource: GetCardSource());
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
            //Then {Headlong} may use a Power.
            coroutine = GameController.SelectAndUsePower(HeroTurnTakerController, cardSource: GetCardSource());
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