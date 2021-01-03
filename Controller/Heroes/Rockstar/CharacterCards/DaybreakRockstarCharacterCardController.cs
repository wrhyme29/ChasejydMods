using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
	public class DaybreakRockstarCharacterCardController : HeroCharacterCardController
	{
		public DaybreakRockstarCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		private static readonly string InnatePowerUses = "InnatePower";


		public override IEnumerator UsePower(int index = 0)
		{
			//Reduce damage redirected to {Rockstar} by 1 until the start of your next turn. Gain 2 HP.
			int num = GetCardPropertyJournalEntryInteger(InnatePowerUses) ?? 0;
			SetCardProperty(InnatePowerUses, num + 1);
			IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
			AddReduceDamageTrigger((DealDamageAction dd) => GetCardPropertyJournalEntryInteger(InnatePowerUses) != null && GetCardPropertyJournalEntryInteger(InnatePowerUses).HasValue && GetCardPropertyJournalEntryInteger(InnatePowerUses).Value > 0 && dd.Target == CharacterCard && dd.OriginalTarget != dd.Target, (DealDamageAction dd) => GetCardPropertyJournalEntryInteger(InnatePowerUses).Value);
			AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, ResetNumberOfPowersResponse, TriggerType.Hidden);
        }

        private IEnumerator ResetNumberOfPowersResponse(PhaseChangeAction arg)
        {
			SetCardProperty(InnatePowerUses, 0);
			yield return null;
			yield break;

		}

        public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//Up to 3 Hero Targets may gain 1 HP.
						IEnumerator coroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, 1, optional: false, (Card c) => c.IsHero, 3, 0, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						// Each Player may shuffle a card from their trash into their deck.
						List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine = EachPlayerMovesCards(0, 1, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => true), (HeroTurnTaker htt) => htt.Trash, (HeroTurnTaker htt) => new List<MoveCardDestination>
						{
							new MoveCardDestination(htt.Deck)
						}, requiredNumberOfHeroes: 0, storedResults: storedResults);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						if(DidSelectCards(storedResults))
                        {
							foreach(SelectCardDecision cardDecision in storedResults)
                            {
								Location deck = cardDecision.SelectedCard.Owner.Deck;
								coroutine = ShuffleDeck(DecisionMaker, deck);
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
						break;
					}
				case 2:
					{
						//Treat this card as a 1 HP, Indestructible Hero Target until the start of your next turn.
						MakeTargetStatusEffect makeTargetStatusEffect = new MakeTargetStatusEffect(1, isIndestructible: true);
						makeTargetStatusEffect.CardsToMakeTargets.IsSpecificCard = base.Card;
						makeTargetStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
						IEnumerator coroutine3 = AddStatusEffect(makeTargetStatusEffect);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
			}
			yield break;
		}
	}
}
