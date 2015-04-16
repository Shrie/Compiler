using System;

namespace Compilers
{
	public class LabelMaker
	{
		string label;
		int labelNum;
		public LabelMaker (string in_label)
		{
			label = in_label;
			labelNum = 0;
		}

		public string MakeLabel(){
			string outLabel = string.Concat (label, labelNum.ToString());
			labelNum++;
			return outLabel;
		}
	}
}

