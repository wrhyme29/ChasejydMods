using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class BlindsideCardController : HeadlongCardController
    {

        public BlindsideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            int X = GetNumberOfEnvironmentCardsInPlay() + 1;
            //Deal one Non-Hero Target X Melee Damage where X is equal to the number of Environment Cards in play +1. 
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), X, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Each other Player may Draw a Card.
            coroutine = EachPlayerDrawsACard(heroCriteria: htt => htt != HeroTurnTaker);
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

        private int GetNumberOfEnvironmentCardsInPlay()
        {
            return FindCardsWhere(c => c.IsEnvironment && c.IsInPlayAndHasGameText && c.IsRealCard).Count();
        }


    }
}