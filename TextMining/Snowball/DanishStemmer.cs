/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// This file was generated automatically by the Snowball to Java compiler
using System;
using Among = SF.Snowball.Among;
using SnowballProgram = SF.Snowball.SnowballProgram;
namespace SF.Snowball.Ext
{
	
	/// <summary> Generated class implementing code defined by a snowball script.</summary>
    public class DanishStemmer : SnowballProgram, ISnowballStemmer
	{
		public DanishStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("hed", - 1, 1, "", this), new Among("ethed", 0, 1, "", this), new Among("ered", - 1, 1, "", this), new Among("e", - 1, 1, "", this), new Among("erede", 3, 1, "", this), new Among("ende", 3, 1, "", this), new Among("erende", 5, 1, "", this), new Among("ene", 3, 1, "", this), new Among("erne", 3, 1, "", this), new Among("ere", 3, 1, "", this), new Among("en", - 1, 1, "", this), new Among("heden", 10, 1, "", this), new Among("eren", 10, 1, "", this), new Among("er", - 1, 1, "", this), new Among("heder", 13, 1, "", this), new Among("erer", 13, 1, "", this), new Among("s", - 1, 2, "", this), new Among("heds", 16, 1, "", this), new Among("es", 16, 1, "", this), new Among("endes", 18, 1, "", this), new Among("erendes", 19, 1, "", this), new Among("enes", 18, 1, "", this), new Among("ernes", 18, 1, "", this), new Among("eres", 18, 1, "", this), new Among("ens", 16, 1, "", this), new Among("hedens", 24, 1, "", this), new Among("erens", 24, 1, "", this), new Among("ers", 16, 1, "", this), new Among("ets", 16, 1, "", this), new Among("erets", 28, 1, "", this), new Among("et", - 1, 1, "", this), new Among("eret", 30, 1, "", this)};
			a1 = new Among[]{new Among("gd", - 1, - 1, "", this), new Among("dt", - 1, - 1, "", this), new Among("gt", - 1, - 1, "", this), new Among("kt", - 1, - 1, "", this)};
			a2 = new Among[]{new Among("ig", - 1, 1, "", this), new Among("lig", 0, 1, "", this), new Among("elig", 1, 1, "", this), new Among("els", - 1, 1, "", this), new Among("l\u00F8st", - 1, 2, "", this)};
		}
		
		private Among[] a0;
		
		private Among[] a1;
		private Among[] a2;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (48), (char) (0), (char) (128)};
		private static readonly char[] gSEnding = new char[]{(char) (239), (char) (254), (char) (42), (char) (3), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (16)};
		
		private int I_p1;
		private System.Text.StringBuilder S_ch = new System.Text.StringBuilder();
		
		protected internal virtual void  copyFrom(DanishStemmer other)
		{
			I_p1 = other.I_p1;
			S_ch = other.S_ch;
			base.copyFrom(other);
		}
		
		private bool rMarkRegions()
		{
			int v1;
			// (, line 29
			I_p1 = limit;
			// goto, line 33
			while (true)
			{
				v1 = cursor;
				do 
				{
					if (!(inGrouping(gV, 97, 248)))
					{
						goto lab1Brk;
					}
					cursor = v1;
					goto golab0Brk;
				}
				while (false);

lab1Brk: ;

				cursor = v1;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab0Brk: ;

			// gopast, line 33
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 248)))
					{
						goto lab3Brk;
					}
					goto golab2Brk;
				}
				while (false);

lab3Brk: ;

				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab2Brk: ;

			// setmark p1, line 33
			I_p1 = cursor;
			// try, line 34
			do 
			{
				// (, line 34
				if (!(I_p1 < 3))
				{
					goto lab4Brk;
				}
				I_p1 = 3;
			}
			while (false);

lab4Brk: ;
			
			return true;
		}
		
		private bool rMainSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 39
			// setlimit, line 40
			v1 = limit - cursor;
			// tomark, line 40
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 40
			// [, line 40
			ket = cursor;
			// substring, line 40
			amongVar = findAmongB(a0, 32);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 40
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 47
					// delete, line 47
					sliceDel();
					break;
				
				case 2: 
					// (, line 49
					if (!(inGroupingB(gSEnding, 97, 229)))
					{
						return false;
					}
					// delete, line 49
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rConsonantPair()
		{
			int v1;
			int v2;
			int v3;
			// (, line 53
			// test, line 54
			v1 = limit - cursor;
			// (, line 54
			// setlimit, line 55
			v2 = limit - cursor;
			// tomark, line 55
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v3 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v2;
			// (, line 55
			// [, line 55
			ket = cursor;
			// substring, line 55
			if (findAmongB(a1, 4) == 0)
			{
				limitBackward = v3;
				return false;
			}
			// ], line 55
			bra = cursor;
			limitBackward = v3;
			cursor = limit - v1;
			// next, line 61
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 61
			bra = cursor;
			// delete, line 61
			sliceDel();
			return true;
		}
		
		private bool rOtherSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 64
			// do, line 65
			v1 = limit - cursor;
			do 
			{
				// (, line 65
				// [, line 65
				ket = cursor;
				// literal, line 65
				if (!(eqSB(2, "st")))
				{
					goto lab0Brk;
				}
				// ], line 65
				bra = cursor;
				// literal, line 65
				if (!(eqSB(2, "ig")))
				{
					goto lab0Brk;
				}
				// delete, line 65
				sliceDel();
			}
			while (false);

lab0Brk: ;

			cursor = limit - v1;
			// setlimit, line 66
			v2 = limit - cursor;
			// tomark, line 66
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v3 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v2;
			// (, line 66
			// [, line 66
			ket = cursor;
			// substring, line 66
			amongVar = findAmongB(a2, 5);
			if (amongVar == 0)
			{
				limitBackward = v3;
				return false;
			}
			// ], line 66
			bra = cursor;
			limitBackward = v3;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 69
					// delete, line 69
					sliceDel();
					// do, line 69
					v4 = limit - cursor;
					do 
					{
						// call consonantPair, line 69
						if (!rConsonantPair())
						{
							goto lab1Brk;
						}
					}
					while (false);

lab1Brk: ;

					cursor = limit - v4;
					break;
				
				case 2: 
					// (, line 71
					// <-, line 71
					sliceFrom("l\u00F8s");
					break;
				}
			return true;
		}
		
		private bool rUndouble()
		{
			int v1;
			int v2;
			// (, line 74
			// setlimit, line 75
			v1 = limit - cursor;
			// tomark, line 75
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 75
			// [, line 75
			ket = cursor;
			if (!(outGroupingB(gV, 97, 248)))
			{
				limitBackward = v2;
				return false;
			}
			// ], line 75
			bra = cursor;
			// -> ch, line 75
			S_ch = sliceTo(S_ch);
			limitBackward = v2;
			// name ch, line 76
			if (!(eqVB(S_ch)))
			{
				return false;
			}
			// delete, line 77
			sliceDel();
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 81
			// do, line 83
			v1 = cursor;
			do 
			{
				// call markRegions, line 83
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;

			cursor = v1;
			// backwards, line 84
			limitBackward = cursor; cursor = limit;
			// (, line 84
			// do, line 85
			v2 = limit - cursor;
			do 
			{
				// call mainSuffix, line 85
				if (!rMainSuffix())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 86
			v3 = limit - cursor;
			do 
			{
				// call consonantPair, line 86
				if (!rConsonantPair())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;

			cursor = limit - v3;
			// do, line 87
			v4 = limit - cursor;
			do 
			{
				// call otherSuffix, line 87
				if (!rOtherSuffix())
				{
					goto lab3Brk;
				}
			}
			while (false);

lab3Brk: ;

			cursor = limit - v4;
			// do, line 88
			v5 = limit - cursor;
			do 
			{
				// call undouble, line 88
				if (!rUndouble())
				{
					goto lab4Brk;
				}
			}
			while (false);

lab4Brk: ;

			cursor = limit - v5;
			cursor = limitBackward; return true;
		}
	}
}
