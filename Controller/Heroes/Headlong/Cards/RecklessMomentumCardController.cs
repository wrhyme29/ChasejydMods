using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Headlong
{
    public class RecklessMomentumCardController : HeadlongCardController
    {

        public RecklessMomentumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Shuffle any number of Momentum Cards from your Trash into your Deck. 

            List<MoveCardAction> storedResults = new List<MoveCardAction>();
            int maxCount = FindCardsWhere(c => IsMomentum(c) && TurnTaker.Trash.HasCard(c)).Count();
            IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(HeroTurnTakerController, TurnTaker.Trash, 0, maxCount, new LinqCardCriteria(c => IsMomentum(c), "momentum"), (new MoveCardDestination(TurnTaker.Deck)).ToEnumerable(), selectionType: SelectionType.ShuffleCardFromTrashIntoDeck,  allowAutoDecide: false, storedResultsMove: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = ShuffleDeck(HeroTurnTakerController, TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(!DidMoveCard(storedResults))
            {
                yield break;
            }
            //For each Card Shuffled into your Deck this way, {Headlong} deals himself 1 Fire Damage, and then deals 1 Non-Hero Target 2 Melee Damage. 
            //He must choose a different Non-Hero Target each time.
            int cardsMoved = storedResults.Count();
            List<Card> selectedNonHeroTargets = new List<Card>();
            List<SelectCardDecision> storedChoice;
            while (cardsMoved > 0 && CharacterCard.IsInPlayAndHasGameText && !CharacterCard.IsIncapacitatedOrOutOfGame)
            {
                cardsMoved--;

                //Headlong deals himself 1 Fire Damage
                coroutine = DealDamage(CharacterCard, CharacterCard, 1, DamageType.Fire, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //then deals 1 Non - Hero Target 2 Melee Damage
                storedChoice = new List<SelectCardDecision>();

                coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget && !selectedNonHeroTargets.Contains(c), storedResultsDecisions: storedChoice, selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(DidSelectCard(storedChoice))
                {
                    selectedNonHeroTargets.Add(GetSelectedCard(storedChoice));
                }

            }

            yield break;
        }

    }
}