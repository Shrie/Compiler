using System;
using System.Collections;

namespace Compilers
{
	public class SymbolTable
	{
		string name;
		int depth;
		int size;
		string label;
		ArrayList records;

		public SymbolTable (string in_name, int in_depth, string in_label)
		{
			name = in_name;
			depth = in_depth;
			label = in_label;
			records = new ArrayList ();
		}

		public void AddRecord(TableRecord in_record){
			in_record.SetOffset (size);
			size = in_record.Size () + size;
			records.Add (in_record);
			in_record.printRecord ();
		}

		public int Size(){
			return size;
		}
	}
}

