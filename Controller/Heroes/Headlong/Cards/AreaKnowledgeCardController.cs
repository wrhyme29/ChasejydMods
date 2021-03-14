using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Headlong
{
    public class AreaKnowledgeCardController : HeadlongCardController
    {

        public AreaKnowledgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Look at the Top 3 Cards of the Environment Deck. Put one into Play, one at the top or bottom of the Environment Deck, and one into the Trash. 

            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.RevealCards(TurnTakerController, FindEnvironment().TurnTaker.Deck, 3, storedResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults.Any())
            {
                //Put one into Play
                List<PlayCardAction> playedCard = new List<PlayCardAction>();
                coroutine = GameController.SelectAndPlayCard(HeroTurnTakerController, storedResults, isPutIntoPlay: true, storedResults: playedCard, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidPlayCards(playedCard))
                {
                    Card toRemove = playedCard.First().CardToPlay;
                    storedResults.Remove(toRemove);
                }
            }

            if(storedResults.Any())
            {
                //put one at the top or bottom of the Environment Deck

                List<SelectCardDecision> moveCardResults = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.MoveCard, storedResults, moveCardResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectCard(moveCardResults))
                {
                    Card selectedCard = GetSelectedCard(moveCardResults);
                    List<MoveCardDestination> list = new List<MoveCardDestination>();
                    list.Add(new MoveCardDestination(FindEnvironment().TurnTaker.Deck));
                    list.Add(new MoveCardDestination(FindEnvironment().TurnTaker.Deck, toBottom: true));
                    coroutine = GameController.SelectLocationAndMoveCard(HeroTurnTakerController, selectedCard, list,cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    storedResults.Remove(selectedCard);

                }
            }

            if(storedResults.Any())
            {
                //put one into the Trash
                Card cardToMove = storedResults.First();
                coroutine = GameController.MoveCard(TurnTakerController, cardToMove, FindEnvironment().TurnTaker.Trash, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Until the Start of {Headlong}’s next Turn, Heroes are Immune to Damage from Environment Cards.
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.SourceCriteria.IsEnvironment = true;
            effect.TargetCriteria.IsHeroCharacterCard = true;
            effect.UntilStartOfNextTurn(TurnTaker);
            coroutine = AddStatusEffect(effect);
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