using System;

namespace Compilers
{
	public class Parameter
	{
		string type;
		string mode;
		public Parameter (string in_mode, string in_type)
		{
			mode = in_mode;
			type = in_type;
		}

		public string Type(){
			return type;
		}

		public string Mode(){
			return mode;
		}
	}
}

