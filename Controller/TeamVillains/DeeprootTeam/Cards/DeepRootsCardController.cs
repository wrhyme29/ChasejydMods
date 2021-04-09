using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class DeepRootsCardController : DeeprootTeamCardController
    {

        public DeepRootsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce Damage dealt to Environment Targets by 1.
            AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);

            //{Deeproot} is Immune to Damage dealt by Environment Cards.
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard && dd.Target == CharacterCard);

            //At the end of {Deeproot}'s turn, shuffle the Environment Trash into its deck
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, pca => GameController.ShuffleTrashIntoDeck(FindEnvironment(), cardSource: GetCardSource()), TriggerType.ShuffleTrashIntoDeck);

        }


    }
}