using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.SnareTeam
{
    public class EncapsulateCardController : SnareTeamCardController
    {

        public EncapsulateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
		{
			//Play this card next to the hero character with the most cards in play.
			List<TurnTaker> storedTurnTaker = new List<TurnTaker>();
			IEnumerator coroutine = FindHeroWithMostCardsInPlay(storedTurnTaker, heroCriteria: new LinqTurnTakerCriteria(tt => GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(storedTurnTaker is null || storedTurnTaker.Count == 0)
            {
				yield break;
            }

			TurnTaker selectedTurnTaker = storedTurnTaker.First();
			coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.Owner == selectedTurnTaker && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "active hero"), storedResults, isPutIntoPlay, decisionSources);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

        public override void AddTriggers()
        {
			//The hero next to this card cannot play cards.
			CannotPlayCards(ttc => ttc != null && ttc.TurnTaker != null && GetCardThisCardIsNextTo() != null && ttc.TurnTaker == GetCardThisCardIsNextTo().Owner);

			//At the start of {Snare}’s turn, she deals the hero character next to this card 2 toxic damage.
			AddDealDamageAtStartOfTurnTrigger(TurnTaker, CharacterCard, c => c == GetCardThisCardIsNextTo(), TargetType.SelectTarget, 2, DamageType.Toxic);
		}

	}
}