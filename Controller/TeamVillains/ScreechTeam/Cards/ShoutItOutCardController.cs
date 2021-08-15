using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class ShoutItOutCardController : ScreechTeamCardController
    {

        public ShoutItOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsDiscord(c), "discord"));
        }

        int X => GetNumberOfDiscordCardsInPlay();

        public override IEnumerator Play()
        {
            //{Screech} deals the Hero Target with the highest HP X Sonic Damage, where X is equal to the number of Discord cards in play.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), c => X, DamageType.Sonic, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults.Count == 0)
            {
                yield break;
            }
            Card heroTarget = storedResults.First().OriginalTarget;

            //Then {Screech} deals each other Hero Target 1 Projectile Damage.
            coroutine = DealDamage(CharacterCard, c => c != heroTarget && c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, DamageType.Projectile);
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