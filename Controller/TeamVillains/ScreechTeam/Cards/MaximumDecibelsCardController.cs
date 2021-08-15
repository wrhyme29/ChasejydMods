using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class MaximumDecibelsCardController : ScreechTeamCardController
    {

        public MaximumDecibelsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsDiscord(c) && c != Card, "other discord"));
        }

        public override void AddTriggers()
        {
            //Increase Sonic Damage dealt by Screech by 2.
            AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.DamageType == DamageType.Sonic, 2);

            //At the end of {Screech}'s turn, if there are no other Discord cards in play, destroy this card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf, additionalCriteria: pca => GetNumberOfDiscordCardsInPlay(excludeSelf: true) == 0);
        }

    }
}