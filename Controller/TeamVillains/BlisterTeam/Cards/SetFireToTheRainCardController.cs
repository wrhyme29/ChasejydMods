using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chasejyd.BlisterTeam
{
    public class SetFireToTheRainCardController : BlisterTeamUtilityCardController
    {

        public SetFireToTheRainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("BlazingAxe");
        }

        public override IEnumerator Play()
        {
            //Blister deals each Non-Villain Target 1 Fire Damage. 
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsVillainTarget(c), 1, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If Blazing Axe is in play, deal it 2 Fire Damage.
            if (IsBlazingAxeInPlay())
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
        }


    }
}