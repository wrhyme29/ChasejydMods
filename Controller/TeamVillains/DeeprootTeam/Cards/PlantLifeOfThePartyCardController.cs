using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class PlantLifeOfThePartyCardController : DeeprootTeamCardController
    {

        public PlantLifeOfThePartyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsPlantGrowth(c), "plant growth"));
        }

        public override void AddTriggers()
        {
            //Reduce Non-Fire Damage dealt to {Deeproot} by 1
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageType != DamageType.Fire && dd.Target == CharacterCard, dd => 1);

            //Increase Fire Damage dealt to {Deeproot} by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Fire && dd.Target == CharacterCard, dd => 1);

            //At the end of {Deeproot}'s turn, he gains 1 HP for each Plant Growth Card in play."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, pca => GameController.GainHP(CharacterCard, GetNumberOfPlantGrowthCardsInPlay(), cardSource: GetCardSource()), TriggerType.GainHP);
        }
    }
}