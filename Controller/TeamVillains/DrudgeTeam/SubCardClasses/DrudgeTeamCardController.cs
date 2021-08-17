using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class DrudgeTeamCardController : CardController
    {

        public DrudgeTeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string VampiricKeyword = "vampiric";
        public static readonly string ThrallIdentifier = "Thrall";

        protected bool IsVampiric(Card card)
        {
            return card.DoKeywordsContain(VampiricKeyword);
        }

        protected bool IsThrall(Card card)
        {
            return card != null && card.Identifier == ThrallIdentifier;
        }

        protected IEnumerator RevealCardsFromDeckToPlayThrall()
        {
            return RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, true, true, false, new LinqCardCriteria(c => IsThrall(c), "", useCardsSuffix: false, singular: "thrall", plural: "thralls"), 1);
        }

    }
}