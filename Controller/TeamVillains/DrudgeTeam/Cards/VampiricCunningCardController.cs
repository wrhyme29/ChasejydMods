using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.DrudgeTeam
{
    public class VampiricCunningCardController : DrudgeTeamCardController
    {

        public VampiricCunningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsVampiric(c), "vampiric"));
        }

        private int NumberOfVampiricCardsInPlay => FindCardsWhere(c => IsVampiric(c) && c.IsInPlayAndHasGameText).Count();

        public override IEnumerator Play()
        {
            //Destroy a Hero Ongoing.
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsHero && c.IsOngoing, "hero ongoing"), false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If there are 3 or more Vampiric Cards in play, destroy a second Hero Ongoing.
            if(NumberOfVampiricCardsInPlay < 3)
            {
                yield break;
            }

            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsHero && c.IsOngoing, "hero ongoing"), false, cardSource: GetCardSource());
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