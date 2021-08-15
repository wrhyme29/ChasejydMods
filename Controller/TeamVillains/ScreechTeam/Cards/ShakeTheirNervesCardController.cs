using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class ShakeTheirNervesCardController : ScreechTeamCardController
    {

        public ShakeTheirNervesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string CannotDrawCardsStatusEffectKey = "CannotDrawCardsStatusEffect";


        /*
         * The actual triggers for the CannotDrawCards and the clearing of the fake status effect both live on the ScreechTeamCharacterCardController
         * This is because this card will move out of play and thus the triggers will no longer be active
         * Unfortunately there doesn't exist a CannotDrawCards status effect (best option)
         * Unfortunately there isn't a parameter for outOfPlayTrigger on CannotPerformAction function (second best)
         */
        public override IEnumerator Play()
        {
            //set property to empty list if null
            if (Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey) is null)
            {
                Game.Journal.RecordCardProperties(CharacterCard, CannotDrawCardsStatusEffectKey, new List<string>());
            }

            //Discard the top Card of each Deck.
            IEnumerator coroutine = GameController.DiscardTopCardsOfDecks(DecisionMaker, (Location l) => !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, 1, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Screech} deals each Hero Target 1 Sonic Damage. Heroes dealt Damage this way cannot Draw cards until the start of {Screech}'s next turn.
            coroutine = DealDamage(CharacterCard, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 1, DamageType.Sonic, addStatusEffect: AddCannotDrawCardsUntilTheStartOfYourNextTurnResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AddCannotDrawCardsUntilTheStartOfYourNextTurnResponse(DealDamageAction dd)
        {
            //Heroes dealt Damage this way cannot Draw cards until the start of {Screech}'s next turn.
            if(!dd.DidDealDamage)
            {
                yield break;
            }

            List<string> cannotDrawCardsIdentifiers = Game.Journal.GetCardPropertiesStringList(CharacterCard, CannotDrawCardsStatusEffectKey).ToList();
            cannotDrawCardsIdentifiers.Add(dd.Target.Owner.Identifier);
            Game.Journal.RecordCardProperties(CharacterCard, CannotDrawCardsStatusEffectKey, cannotDrawCardsIdentifiers);

            IEnumerator coroutine = GameController.SendMessageAction($"{dd.Target.Owner.Name} cannot draw cards until the start of {TurnTaker.Name}'s next turn.", Priority.High, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

    }
}