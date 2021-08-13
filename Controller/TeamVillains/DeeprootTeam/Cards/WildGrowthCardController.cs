using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class WildGrowthCardController : DeeprootTeamCardController
    {

        public WildGrowthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Shuffle {Deeproot}'s trash into his deck.
            IEnumerator coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			//Reveal the top {H} card of {Deeproot}'s deck. Put any Plant Growth Cards into Play. Discard all other cards. {Deeproot} gains 2 HP for each card discarded this way.
			List<Card> storedDiscards = new List<Card>();
			coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, Game.H, new LinqCardCriteria(c => IsPlantGrowth(c), "plant growth"), discardedCards: storedDiscards);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = GameController.GainHP(CharacterCard, 2 * storedDiscards.Count(), cardSource: GetCardSource());
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