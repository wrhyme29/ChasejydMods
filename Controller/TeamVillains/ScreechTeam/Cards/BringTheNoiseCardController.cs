using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class BringTheNoiseCardController : ScreechTeamCardController
    {

        public BringTheNoiseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, cardCriteria: new LinqCardCriteria(c => IsDiscord(c), "discord"));
        }

        public override void AddTriggers()
        {
            //At the start of {Screech}'s turn, {Screech} deals himself 2 Irreducible Toxic Damage. Then reveal cards from his deck until a Discord card is revealed and put it into play. Shuffle the other revealed cards back into {Screech}'s deck.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.RevealCard });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //{Screech} deals himself 2 Irreducible Toxic Damage
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Toxic, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then reveal cards from his deck until a Discord card is revealed and put it into play. Shuffle the other revealed cards back into {Screech}'s deck.

            coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, true, true, false, new LinqCardCriteria(c => IsDiscord(c), "discord"), 1, shuffleReturnedCards: true);
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