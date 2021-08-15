using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class ScreechTeamCardController : CardController
    {

        public ScreechTeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string DiscordKeyword = "discord";

        protected bool IsDiscord(Card card)
        {
            return card.DoKeywordsContain(DiscordKeyword);
        }

        protected int GetNumberOfDiscordCardsInPlay(bool excludeSelf = false)
        {
            return FindCardsWhere(c => IsDiscord(c) && c.IsInPlayAndHasGameText && (excludeSelf ? c != Card : true)).Count();
        }


    }
}