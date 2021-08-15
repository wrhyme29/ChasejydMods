using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class NoSelfControlCardController : ScreechTeamCardController
    {

        public NoSelfControlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: Game.H - 2);
        }

        public override IEnumerator Play()
        {
            //{Screech} deals himself 3 Irreducible Toxic Damage.
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 3, DamageType.Toxic, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then {Screech} deals the {H - 2} Hero Targets with the highest HP 3 Sonic Damage each. Any Hero Characters Dealt Damage this way must Discard a Card.
            coroutine = DealDamageToHighestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => 3, DamageType.Sonic, numberOfTargets: () => Game.H - 2, addStatusEffect: DiscardCardResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DiscardCardResponse(DealDamageAction dd)
        {
            if(!dd.Target.IsHeroCharacterCard || !dd.DidDealDamage || dd.Target.Owner.IsIncapacitatedOrOutOfGame)
            {
                yield break;
            }

            HeroTurnTakerController hero = FindHeroTurnTakerController(dd.Target.Owner.ToHero());
            IEnumerator coroutine =  GameController.SelectAndDiscardCard(hero, cardSource: GetCardSource());
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