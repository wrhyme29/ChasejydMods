using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DeeprootTeam
{
    public class PhotosynthestrikeCardController : DeeprootTeamCardController
    {

        public PhotosynthestrikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsEnvironment, "environment"));
        }

        public override IEnumerator Play()
        {
            //{Deeproot} deals the Hero Target with the Highest HP X+1 toxic Damage, where X equals the number of Environment Cards in play.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => GetNumberOfEnvironmentCardsInPlay() + 1, DamageType.Toxic);
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