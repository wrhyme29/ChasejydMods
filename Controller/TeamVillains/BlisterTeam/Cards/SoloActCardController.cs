using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class SoloActCardController : BlisterTeamUtilityCardController
    {

        public SoloActCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //{Blister} deals the Hero Target with the Highest HP 1 Sonic Damage, 1 Fire Damage, and 1 Toxic Damage.

            List<Card> storeHighest = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHero && c.IsTarget, storeHighest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(storeHighest != null && storeHighest.Any())
            {
                Card target = storeHighest.FirstOrDefault();

                coroutine = DealDamage(CharacterCard, target, 1, DamageType.Sonic, cardSource: GetCardSource());
                IEnumerator coroutine2 = DealDamage(CharacterCard, target, 1, DamageType.Fire, cardSource: GetCardSource());
                IEnumerator coroutine3 = DealDamage(CharacterCard, target, 1, DamageType.Toxic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
            }
        }


    }
}