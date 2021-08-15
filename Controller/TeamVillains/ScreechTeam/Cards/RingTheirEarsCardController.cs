using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.ScreechTeam
{
    public class RingTheirEarsCardController : ScreechTeamCardController
    {

        public RingTheirEarsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 2);
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(2, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults.Count() == 0)
            {
                yield break;
            }

            Card lowestHero = storedResults.First();

            //{Screech} deals the Hero Target with the second lowest HP 2 Sonic Damage.
            coroutine = DealDamage(CharacterCard, lowestHero, 2, DamageType.Sonic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //That Target then deals the Hero Target with the highest HP 3 Melee Damage.
            CardSource lowestHeroSource = FindCardController(lowestHero).GetCardSource();
            coroutine = DealDamageToHighestHP(lowestHero, 1, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, lowestHeroSource), c => 3, DamageType.Melee);
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