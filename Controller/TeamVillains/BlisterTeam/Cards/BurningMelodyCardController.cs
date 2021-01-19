using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq ;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class BurningMelodyCardController : BlisterTeamUtilityCardController
    {

        public BurningMelodyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //Deal the Hero Target with the highest HP 2 Fire Damage.

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(Card, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 2, DamageType.Fire, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Destroy an Ongoing Card in that Hero's play area.
            if (storedResults != null && storedResults.Any())
            {
                Card target = storedResults.FirstOrDefault().OriginalTarget;
                coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && c.Location == target.Owner.PlayArea), false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

    }
}