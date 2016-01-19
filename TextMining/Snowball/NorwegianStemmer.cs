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
using MyAmong = SF.Snowball.MyAmong;
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
			a0 = new MyAmong[]{new MyAmong("a", - 1, 1), new MyAmong("e", - 1, 1), new MyAmong("ede", 1, 1), new MyAmong("ande", 1, 1), new MyAmong("ende", 1, 1), new MyAmong("ane", 1, 1), new MyAmong("ene", 1, 1), new MyAmong("hetene", 6, 1), new MyAmong("erte", 1, 3), new MyAmong("en", - 1, 1), new MyAmong("heten", 9, 1), new MyAmong("ar", - 1, 1), new MyAmong("er", - 1, 1), new MyAmong("heter", 12, 1), new MyAmong("s", - 1, 2), new MyAmong("as", 14, 1), new MyAmong("es", 14, 1), new MyAmong("edes", 16, 1), new MyAmong("endes", 16, 1), new MyAmong("enes", 16, 1), new MyAmong("hetenes", 19, 1), new MyAmong("ens", 14, 1), new MyAmong("hetens", 21, 1), new MyAmong("ers", 14, 1), new MyAmong("ets", 14, 1), new MyAmong("et", - 1, 1), new MyAmong("het", 25, 1), new MyAmong("ert", - 1, 3), new MyAmong("ast", - 1, 1)};
			a1 = new MyAmong[]{new MyAmong("dt", - 1, - 1), new MyAmong("vt", - 1, - 1)};
			a2 = new MyAmong[]{new MyAmong("leg", - 1, 1), new MyAmong("eleg", 0, 1), new MyAmong("ig", - 1, 1), new MyAmong("eig", 2, 1), new MyAmong("lig", 2, 1), new MyAmong("elig", 4, 1), new MyAmong("els", - 1, 1), new MyAmong("lov", - 1, 1), new MyAmong("elov", 7, 1), new MyAmong("slov", 7, 1), new MyAmong("hetslov", 9, 1)};
		}
		
		private MyAmong[] a0;
		private MyAmong[] a1;
		private MyAmong[] a2;
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
			int MyAmongVar;
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
			MyAmongVar = findAmongB(a0, 29);
			if (MyAmongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 37
			bra = cursor;
			limitBackward = v2;
			switch (MyAmongVar)
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
			int MyAmongVar;
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
			MyAmongVar = findAmongB(a2, 11);
			if (MyAmongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 62
			bra = cursor;
			limitBackward = v2;
			switch (MyAmongVar)
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
