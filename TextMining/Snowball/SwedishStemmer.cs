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
    public class SwedishStemmer : SnowballProgram, ISnowballStemmer
	{
		public SwedishStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("a", - 1, 1, "", this), new Among("arna", 0, 1, "", this), new Among("erna", 0, 1, "", this), new Among("heterna", 2, 1, "", this), new Among("orna", 0, 1, "", this), new Among("ad", - 1, 1, "", this), new Among("e", - 1, 1, "", this), new Among("ade", 6, 1, "", this), new Among("ande", 6, 1, "", this), new Among("arne", 6, 1, "", this), new Among("are", 6, 1, "", this), new Among("aste", 6, 1, "", this), new Among("en", - 1, 1, "", this), new Among("anden", 12, 1, "", this), new Among("aren", 12, 1, "", this), new Among("heten", 12, 1, "", this), new Among("ern", - 1, 1, "", this), new Among("ar", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("heter", 18, 1, "", this), new Among("or", - 1, 1, "", this), new Among("s", - 1, 2, "", this), new Among("as", 21, 1, "", this), new Among("arnas", 22, 1, "", this), new Among("ernas", 22, 1, "", this), new Among("ornas", 22, 1, "", this), new Among("es", 21, 1, "", this), new Among("ades", 26, 1, "", this), new Among("andes", 26, 1, "", this), new Among("ens", 21, 1, "", this), new Among("arens", 29, 1, "", this), new Among("hetens", 29, 1, "", this), new Among("erns", 21, 1, "", this), new Among("at", - 1, 1, "", this), new Among("andet", - 1, 1, "", this), new Among("het", - 1, 1, "", this), new Among("ast", - 1, 1, "", this)};
			a1 = new Among[]{new Among("dd", - 1, - 1, "", this), new Among("gd", - 1, - 1, "", this), new Among("nn", - 1, - 1, "", this), new Among("dt", - 1, - 1, "", this), new Among("gt", - 1, - 1, "", this), new Among("kt", - 1, - 1, "", this), new Among("tt", - 1, - 1, "", this)};
			a2 = new Among[]{new Among("ig", - 1, 1, "", this), new Among("lig", 0, 1, "", this), new Among("els", - 1, 1, "", this), new Among("fullt", - 1, 3, "", this), new Among("l\u00F6st", - 1, 2, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (24), (char) (0), (char) (32)};
		private static readonly char[] gSEnding = new char[]{(char) (119), (char) (127), (char) (149)};
		
		private int I_p1;
		
		protected internal virtual void  copyFrom(SwedishStemmer other)
		{
			I_p1 = other.I_p1;
			base.copyFrom(other);
		}
		
		private bool rMarkRegions()
		{
			int v1;
			// (, line 26
			I_p1 = limit;
			// goto, line 30
			while (true)
			{
				v1 = cursor;
				do 
				{
					if (!(inGrouping(gV, 97, 246)))
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
			
			// gopast, line 30
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 246)))
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
			
			// setmark p1, line 30
			I_p1 = cursor;
			// try, line 31
			do 
			{
				// (, line 31
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
			// (, line 36
			// setlimit, line 37
			v1 = limit - cursor;
			// tomark, line 37
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 37
			// [, line 37
			ket = cursor;
			// substring, line 37
			amongVar = findAmongB(a0, 37);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 37
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 44
					// delete, line 44
					sliceDel();
					break;
				
				case 2: 
					// (, line 46
					if (!(inGroupingB(gSEnding, 98, 121)))
					{
						return false;
					}
					// delete, line 46
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
			// setlimit, line 50
			v1 = limit - cursor;
			// tomark, line 50
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 50
			// and, line 52
			v3 = limit - cursor;
			// among, line 51
			if (findAmongB(a1, 7) == 0)
			{
				limitBackward = v2;
				return false;
			}
			cursor = limit - v3;
			// (, line 52
			// [, line 52
			ket = cursor;
			// next, line 52
			if (cursor <= limitBackward)
			{
				limitBackward = v2;
				return false;
			}
			cursor--;
			// ], line 52
			bra = cursor;
			// delete, line 52
			sliceDel();
			limitBackward = v2;
			return true;
		}
		
		private bool rOtherSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// setlimit, line 55
			v1 = limit - cursor;
			// tomark, line 55
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 55
			// [, line 56
			ket = cursor;
			// substring, line 56
			amongVar = findAmongB(a2, 5);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 56
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					limitBackward = v2;
					return false;
				
				case 1: 
					// (, line 57
					// delete, line 57
					sliceDel();
					break;
				
				case 2: 
					// (, line 58
					// <-, line 58
					sliceFrom("l\u00F6s");
					break;
				
				case 3: 
					// (, line 59
					// <-, line 59
					sliceFrom("full");
					break;
				}
			limitBackward = v2;
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 64
			// do, line 66
			v1 = cursor;
			do 
			{
				// call markRegions, line 66
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// backwards, line 67
			limitBackward = cursor; cursor = limit;
			// (, line 67
			// do, line 68
			v2 = limit - cursor;
			do 
			{
				// call mainSuffix, line 68
				if (!rMainSuffix())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 69
			v3 = limit - cursor;
			do 
			{
				// call consonantPair, line 69
				if (!rConsonantPair())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			// do, line 70
			v4 = limit - cursor;
			do 
			{
				// call otherSuffix, line 70
				if (!rOtherSuffix())
				{
					goto lab3Brk;
				}
			}
			while (false);

lab3Brk: ;
			
			cursor = limit - v4;
			cursor = limitBackward; return true;
		}
	}
}
