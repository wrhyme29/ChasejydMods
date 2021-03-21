using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Chasejyd.Rockstar
{
    public class RockstarUtilityCharacterCardController : HeroCharacterCardController
    {

        public RockstarUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public static readonly string DivaIdentifier = "Diva";
        public static readonly string RoarIdentifier = "RockstarRoar";

        public override bool AskIfCardMayPreventAction<T>(TurnTakerController ttc, CardController preventer)
        {
            //Non-Hero cards cannot prevent {Rockstar} from using Powers if Diva is in play.
            if (typeof(T) == typeof(UsePowerAction) && ttc == TurnTakerController && !preventer.Card.IsHero && IsDivaInPlay())
            {
                return false;
            }

            //Non-Hero Cards cannot prevent {Rockstar} from playing cards..
            if (typeof(T) == typeof(PlayCardAction) && ttc == TurnTakerController && !preventer.Card.IsHero && IsRoarInPlay())
            {
                return false;
            }
            return true;
        }

        private IEnumerable<Card> FindDiva()
        {
            return base.FindCardsWhere(c => c.Identifier == DivaIdentifier);
        }

        protected bool IsDivaInPlay()
        {
            return FindDiva().Where(c => c.IsInPlayAndHasGameText).Any();
        }

        private IEnumerable<Card> FindRoar()
        {
            return base.FindCardsWhere(c => c.Identifier == RoarIdentifier);
        }

        protected bool IsRoarInPlay()
        {
            return FindRoar().Where(c => c.IsInPlayAndHasGameText).Any();
        }


        protected bool IsStagePresence(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "stage presence");
        }


        protected int GetNumberOfStagePresenceInPlay()
        {
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsStagePresence(c)).Count();
        }
    }
}