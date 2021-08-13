using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.DeeprootTeam
{
    public class WildbondCardController : DeeprootTeamCardController
    {

        public WildbondCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Each time an Environment Card enters play, {Deeproot} gains 2 HP and deals the Hero Target with the highest HP 3 Melee Damage.
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.IsEnvironment, EnvironmentCardPlayedResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.DealDamage }, TriggerTiming.After);

            // Each time an Environment Card is Destroyed, {Deeproot} deals the Hero Target with the Lowest HP 2 Toxic Damage.
            AddTrigger((DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card.IsEnvironment && dca.WasCardDestroyed, EnvironmentCardDestroyedResponse, TriggerType.DealDamage, TriggerTiming.After);

        }

        private IEnumerator EnvironmentCardDestroyedResponse(DestroyCardAction dca)
        {
            //{Deeproot} deals the Hero Target with the Lowest HP 2 Toxic Damage.
            return DealDamageToLowestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 2, DamageType.Toxic);
        }

        private IEnumerator EnvironmentCardPlayedResponse(CardEntersPlayAction cep)
        {
            //{Deeproot} gains 2 HP
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // {Deeproot} deals the Hero Target with the highest HP 3 Melee Damage.
            coroutine = DealDamageToHighestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Melee);
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