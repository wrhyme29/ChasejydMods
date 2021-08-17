using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class ConsumeLifeforceCardController : DrudgeTeamCardController
    {

        public ConsumeLifeforceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDealInfernalKey, $"{CharacterCard.Title} has dealt infernal damage this turn", $"{CharacterCard.Title} has no dealt infernal damage this turn");
        }

        private static readonly string FirstTimeDealInfernalKey = "FirstTimeDealInfernal";

        public override void AddTriggers()
        {
            // The first time each turn that {DrudgeTeam} deals Infernal Damage, he gains 2 HP.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard) && dd.DamageType == DamageType.Infernal && !HasBeenSetToTrueThisTurn(FirstTimeDealInfernalKey), GainHPResponse, TriggerType.GainHP, TriggerTiming.After);

            //When {DrudgeTeam} destroys a Target, reveal cards from the top of his deck until Thrall is revealed. Put Thrall into play. Shuffle all other cards revealed back into his deck."
            AddTrigger((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card.IsTarget && dca.GetCardDestroyer() == CharacterCard, dca => RevealCardsFromDeckToPlayThrall(), TriggerType.RevealCard, TriggerTiming.After);
        }

        private IEnumerator GainHPResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDealInfernalKey);
            return GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
        }

    }
}