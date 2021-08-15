using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class LeadSingerCardController : ScreechTeamCardController
    {

        public LeadSingerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsVillainTeamCharacter, "villain character"));
        }

        int X => GameController.GetAllCards().Count(c => c.IsVillainTeamCharacter && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()));

        public override void AddTriggers()
        {
            //At the Start of {Screech}'s turn, he gains X HP, where X is equal to the number of Villain Character cards in play.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(CharacterCard, X, cardSource: GetCardSource()), TriggerType.GainHP);

            //If a Villain Character deals damage to {Screech}, Destroy this card.
            AddTrigger((DealDamageAction dd) => dd.Target != null && dd.Target == CharacterCard && dd.DamageSource != null && dd.DamageSource.IsCard && dd.DamageSource.Card.IsVillainTeamCharacter, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);

        }

    }
}