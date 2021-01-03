using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.Rockstar
{
    public class StagePresenceCardController : RockstarCardController
    {

        public StagePresenceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
			//When this card enters play, destroy all other stage presence cards.
			if (GetNumberOfStagePresenceInPlay() > 1)
			{
				IEnumerator coroutine = GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => IsStagePresence(c) && c != Card, "stage presence"), cardSource: GetCardSource());
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