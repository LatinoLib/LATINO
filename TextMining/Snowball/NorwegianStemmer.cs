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
    public class NorwegianStemmer : SnowballProgram, ISnowballStemmer
	{
		public NorwegianStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("a", - 1, 1, "", this), new Among("e", - 1, 1, "", this), new Among("ede", 1, 1, "", this), new Among("ande", 1, 1, "", this), new Among("ende", 1, 1, "", this), new Among("ane", 1, 1, "", this), new Among("ene", 1, 1, "", this), new Among("hetene", 6, 1, "", this), new Among("erte", 1, 3, "", this), new Among("en", - 1, 1, "", this), new Among("heten", 9, 1, "", this), new Among("ar", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("heter", 12, 1, "", this), new Among("s", - 1, 2, "", this), new Among("as", 14, 1, "", this), new Among("es", 14, 1, "", this), new Among("edes", 16, 1, "", this), new Among("endes", 16, 1, "", this), new Among("enes", 16, 1, "", this), new Among("hetenes", 19, 1, "", this), new Among("ens", 14, 1, "", this), new Among("hetens", 21, 1, "", this), new Among("ers", 14, 1, "", this), new Among("ets", 14, 1, "", this), new Among("et", - 1, 1, "", this), new Among("het", 25, 1, "", this), new Among("ert", - 1, 3, "", this), new Among("ast", - 1, 1, "", this)};
			a1 = new Among[]{new Among("dt", - 1, - 1, "", this), new Among("vt", - 1, - 1, "", this)};
			a2 = new Among[]{new Among("leg", - 1, 1, "", this), new Among("eleg", 0, 1, "", this), new Among("ig", - 1, 1, "", this), new Among("eig", 2, 1, "", this), new Among("lig", 2, 1, "", this), new Among("elig", 4, 1, "", this), new Among("els", - 1, 1, "", this), new Among("lov", - 1, 1, "", this), new Among("elov", 7, 1, "", this), new Among("slov", 7, 1, "", this), new Among("hetslov", 9, 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (48), (char) (0), (char) (128)};
		private static readonly char[] gSEnding = new char[]{(char) (119), (char) (127), (char) (149), (char) (1)};
		
		private int I_p1;
		
		protected internal virtual void  copyFrom(NorwegianStemmer other)
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
			
			// gopast, line 30
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
			amongVar = findAmongB(a0, 29);
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
					// (, line 43
					// delete, line 43
					sliceDel();
					break;
				
				case 2: 
					// (, line 45
					if (!(inGroupingB(gSEnding, 98, 122)))
					{
						return false;
					}
					// delete, line 45
					sliceDel();
					break;
				
				case 3: 
					// (, line 47
					// <-, line 47
					sliceFrom("er");
					break;
				}
			return true;
		}
		
		private bool rConsonantPair()
		{
			int v1;
			int v2;
			int v3;
			// (, line 51
			// test, line 52
			v1 = limit - cursor;
			// (, line 52
			// setlimit, line 53
			v2 = limit - cursor;
			// tomark, line 53
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v3 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v2;
			// (, line 53
			// [, line 53
			ket = cursor;
			// substring, line 53
			if (findAmongB(a1, 2) == 0)
			{
				limitBackward = v3;
				return false;
			}
			// ], line 53
			bra = cursor;
			limitBackward = v3;
			cursor = limit - v1;
			// next, line 58
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 58
			bra = cursor;
			// delete, line 58
			sliceDel();
			return true;
		}
		
		private bool rOtherSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 61
			// setlimit, line 62
			v1 = limit - cursor;
			// tomark, line 62
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 62
			// [, line 62
			ket = cursor;
			// substring, line 62
			amongVar = findAmongB(a2, 11);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 62
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 66
					// delete, line 66
					sliceDel();
					break;
				}
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 71
			// do, line 73
			v1 = cursor;
			do 
			{
				// call markRegions, line 73
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
		    	
			cursor = v1;
			// backwards, line 74
			limitBackward = cursor; cursor = limit;
			// (, line 74
			// do, line 75
			v2 = limit - cursor;
			do 
			{
				// call mainSuffix, line 75
				if (!rMainSuffix())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 76
			v3 = limit - cursor;
			do 
			{
				// call consonantPair, line 76
				if (!rConsonantPair())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			// do, line 77
			v4 = limit - cursor;
			do 
			{
				// call otherSuffix, line 77
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
