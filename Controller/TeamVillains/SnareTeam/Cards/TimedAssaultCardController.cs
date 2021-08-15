using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class TimedAssaultCardController : SnareTeamCardController
    {

        public TimedAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria(c => c.IsVillainTeamCharacter && c != CharacterCard, "other villain character"));

        }

        public override IEnumerator Play()
        {
            //Play the top card of the villain character with the highest HP, other than {Snare}.
            IEnumerator coroutine;
            IEnumerable<Card> highestVillains = GameController.FindAllTargetsWithHighestHitPoints(1, c => c.IsVillainTeamCharacter && c != CharacterCard && !c.IsIncapacitatedOrOutOfGame, GetCardSource());
            if (highestVillains.Count() == 0)
            {
                yield break;
            }
            Card highestVillain = highestVillains.First();
            if (highestVillains.Count() > 1)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.Custom, highestVillains, storedResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (!DidSelectCard(storedResults))
                {
                    yield break;
                }
                highestVillain = GetSelectedCard(storedResults);
            }

            coroutine = GameController.SendMessageAction(Card.Title + " plays the top card of " + highestVillain.Owner.Name + "'s deck...", Priority.High, GetCardSource(), null, showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, highestVillain.Owner.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then that villain character deals the hero target with the highest HP 2 sonic damage.
            coroutine = DealDamageToHighestHP(highestVillain, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 2, DamageType.Sonic);
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

            return new CustomDecisionText("Select a villain to be treated as the highest HP.", "Select a villain to be treated as the highest HP.", "Vote for a villain to be treated as the highest HP.", "highest HP villain");

        }
    }
}