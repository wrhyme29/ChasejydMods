using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class RattleTheirBrainsCardController : ScreechTeamCardController
    {

        public RattleTheirBrainsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Each player must Discard 1 Card.
            IEnumerator coroutine = GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Screech} deals each hero target 1 Sonic Damage. Any targets dealt damage this way then deals themselves 1 Psychic Damage.
            coroutine = DealDamage(CharacterCard, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, DamageType.Sonic, addStatusEffect: DealDamageFollowupResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageFollowupResponse(DealDamageAction dd)
        {
            if(!dd.DidDealDamage)
            {
                yield break;
            }

            IEnumerator coroutine = DealDamage(dd.Target, dd.Target, 1, DamageType.Psychic, cardSource: GetCardSource());
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