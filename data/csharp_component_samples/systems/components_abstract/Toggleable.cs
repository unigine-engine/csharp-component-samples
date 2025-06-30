using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "a7d5d2a62572dd16887cc1793473380ff98c0197")]
public abstract class Toggleable : Component
{
	[ShowInEditor]
	private bool isToggled = false;

	public bool Toggled
	{
		get => isToggled;
		set
		{
			if (value != isToggled)
			{
				bool ok = value ? On() : Off();
				isToggled = isToggled ^ ok;
			}
		}
	}

	public bool Toggle() => isToggled = isToggled ^ (isToggled ? Off() : On());

	protected abstract bool On();
	protected abstract bool Off();
}
