using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class FireballCardController : BlisterTeamUtilityCardController
    {

        public FireballCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: FindCardsWhere(c=>c.Location.IsTrash && c.Location.IsEnvironment).Count());
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("BlazingAxe");
        }

        public override IEnumerator Play()
        {   
            //"Blister deals the X Hero Targets with the highest HP 3 Fire Damage, where X is equal to the number of cards in the Environment Trash plus 1.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 3, DamageType.Fire, numberOfTargets: () => FindCardsWhere(c => c.Location.IsTrash && c.Location.IsEnvironment).Count() + 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Destroy 1 Hero Ongoing and 1 of {Blister}'s Ongoings. 
            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && c.IsHero), false, cardSource: GetCardSource());
            IEnumerator coroutine2 = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsOngoing && c.Owner == TurnTaker), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            //Then if Blazing Axe is in play, Blister deals it 2 Fire Damage.
            if(IsBlazingAxeInPlay())
            {
                Card axe = FindBlazingAxeInPlay();
                coroutine = DealDamage(CharacterCard, axe, 2, DamageType.Fire, cardSource: GetCardSource());
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