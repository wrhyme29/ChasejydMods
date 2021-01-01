using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chasejyd.Rockstar
{
	public class RockstarCharacterCardController : HeroCharacterCardController
	{
		public RockstarCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			
			yield break;
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						yield break;
					}
				case 1:
					{
						
						yield break;
					}
				case 2:
					{
						
						yield break;
					}
			}
			yield break;
		}
	}
}
