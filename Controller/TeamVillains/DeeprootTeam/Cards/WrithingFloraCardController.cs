using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class WrithingFloraCardController : DeeprootTeamCardController
    {

        public WrithingFloraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce Damage dealt by Hero Targets by 1.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsCard && dd.DamageSource.IsHero && dd.DamageSource.IsTarget && GameController.IsCardVisibleToCardSource(dd.DamageSource.Card, GetCardSource()), dd => 1);

            //When this card is destroyed shuffle {Deeproot}'s Trash into his deck.
            AddWhenDestroyedTrigger(dca => GameController.ShuffleTrashIntoDeck(TurnTakerController, cardSource: GetCardSource()), TriggerType.ShuffleTrashIntoDeck);
        }


    }
}