using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class SynchronicityCardController : SnareTeamCardController
    {

        public SynchronicityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //Play the top card of the villain character with the lowest HP, other than {Snare}.
            IEnumerable<Card> lowestVillains = FindAllTargetsWithLowestHitPoints(c => c.IsVillainTeamCharacter && c != CharacterCard && !c.IsIncapacitatedOrOutOfGame, 1);
            if(lowestVillains.Count() == 0)
            {
                yield break;
            }
            Card lowestVillain = lowestVillains.First();
            if(lowestVillains.Count() > 1)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.Custom, lowestVillains, storedResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(!DidSelectCard(storedResults))
                {
                    yield break;
                }
                lowestVillain = GetSelectedCard(storedResults);
            }

            coroutine = GameController.SendMessageAction(Card.Title + " plays the top card of " + lowestVillain.Owner.Name + "'s deck...", Priority.High, GetCardSource(), null, showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, lowestVillain.Owner.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then that villain character gains 2 HP.
            coroutine = GameController.GainHP(lowestVillain, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select a villain to be treated as the lowest HP.", "Select a villain to be treated as the lowest HP.", "Vote for a villain to be treated as the lowest HP.", "lowest HP villain");

        }
    }
}