// Parameter class defines the parameters
// of a function or procedure. Was never really
// used because we didn't get to the A level code
// generation.

using System;

namespace Compilers
{
	public class Parameter
	{
		// Instance variables
		string type;
		string mode;

		// Constructor

		public Parameter (string in_mode, string in_type)
		{
			mode = in_mode;
			type = in_type;
		}

		// Getter method for type

		public string Type(){
			return type;
		}

		// Getter method for mode

		public string Mode(){
			return mode;
		}
	}
}

