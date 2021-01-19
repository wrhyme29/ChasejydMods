using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class StageDiveCardController : BlisterTeamUtilityCardController
    {

        public StageDiveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //Destroy an Environment Card. 
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsEnvironment), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Deal the Hero with the Second Highest HP 2 Melee Damage and 2 Fire Damage.
            List<Card> storeHighest = new List<Card>();
            coroutine = GameController.FindTargetWithHighestHitPoints(2, (Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, storeHighest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storeHighest != null && storeHighest.Any())
            {
                Card target = storeHighest.FirstOrDefault();

                coroutine = DealDamage(CharacterCard, target, 2, DamageType.Melee, cardSource: GetCardSource());
                IEnumerator coroutine2 = DealDamage(CharacterCard, target, 2, DamageType.Fire, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            //Play the top Card of the Environment Deck.
            coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
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