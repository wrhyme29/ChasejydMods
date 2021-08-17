using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class ThrallCardController : DrudgeTeamCardController
    {

        public ThrallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDrudgeDealtNonRadiantDamageKey, $"{CharacterCard.Title} has not been dealt non-radiant damage this turn", $"{CharacterCard.Title} has been dealt non-radiant damage this turn.").Condition = () => Card.IsInPlayAndHasGameText;
        }

        public static readonly string FirstTimeDrudgeDealtNonRadiantDamageKey = "FirstTimeDrudgeDealtNonRadiantDamage";

        public override void AddTriggers()
        {
            //At the end of {DrudgeTeam}'s turn, Thrall deals the Hero Target with the highest HP 2 Melee Damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHero, TargetType.HighestHP, 2, DamageType.Melee);

            //The first time each turn that {DrudgeTeam} would be dealt non-Radiant Damage, redirect that Damage to the Thrall with the lowest HP.
            AddFirstTimePerTurnRedirectTrigger(dd => dd.Target != null && dd.Target == CharacterCard && dd.DamageType != DamageType.Radiant, FirstTimeDrudgeDealtNonRadiantDamageKey, TargetType.LowestHP, c => IsThrall(c));
        }


    }
}