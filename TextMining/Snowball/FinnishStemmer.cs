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
    public class FinnishStemmer : SnowballProgram, ISnowballStemmer
	{
		public FinnishStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("pa", - 1, 1, "", this), new Among("sti", - 1, 2, "", this), new Among("kaan", - 1, 1, "", this), new Among("han", - 1, 1, "", this), new Among("kin", - 1, 1, "", this), new Among("h\u00E4n", - 1, 1, "", this), new Among("k\u00E4\u00E4n", - 1, 1, "", this), new Among("ko", - 1, 1, "", this), new Among("p\u00E4", - 1, 1, "", this), new Among("k\u00F6", - 1, 1, "", this)};
			a1 = new Among[]{new Among("lla", - 1, - 1, "", this), new Among("na", - 1, - 1, "", this), new Among("ssa", - 1, - 1, "", this), new Among("ta", - 1, - 1, "", this), new Among("lta", 3, - 1, "", this), new Among("sta", 3, - 1, "", this)};
			a2 = new Among[]{new Among("ll\u00E4", - 1, - 1, "", this), new Among("n\u00E4", - 1, - 1, "", this), new Among("ss\u00E4", - 1, - 1, "", this), new Among("t\u00E4", - 1, - 1, "", this), new Among("lt\u00E4", 3, - 1, "", this), new Among("st\u00E4", 3, - 1, "", this)};
			a3 = new Among[]{new Among("lle", - 1, - 1, "", this), new Among("ine", - 1, - 1, "", this)};
			a4 = new Among[]{new Among("nsa", - 1, 3, "", this), new Among("mme", - 1, 3, "", this), new Among("nne", - 1, 3, "", this), new Among("ni", - 1, 2, "", this), new Among("si", - 1, 1, "", this), new Among("an", - 1, 4, "", this), new Among("en", - 1, 6, "", this), new Among("\u00E4n", - 1, 5, "", this), new Among("ns\u00E4", - 1, 3, "", this)};
			a5 = new Among[]{new Among("aa", - 1, - 1, "", this), new Among("ee", - 1, - 1, "", this), new Among("ii", - 1, - 1, "", this), new Among("oo", - 1, - 1, "", this), new Among("uu", - 1, - 1, "", this), new Among("\u00E4\u00E4", - 1, - 1, "", this), new Among("\u00F6\u00F6", - 1, - 1, "", this)};
			a6 = new Among[]{new Among("a", - 1, 8, "", this), new Among("lla", 0, - 1, "", this), new Among("na", 0, - 1, "", this), new Among("ssa", 0, - 1, "", this), new Among("ta", 0, - 1, "", this), new Among("lta", 4, - 1, "", this), new Among("sta", 4, - 1, "", this), new Among("tta", 4, 9, "", this), new Among("lle", - 1, - 1, "", this), new Among("ine", - 1, - 1, "", this), new Among("ksi", - 1, - 1, "", this), new Among("n", - 1, 7, "", this), new Among("han", 11, 1, "", this), new Among("den", 11, - 1, "r_VI", this), new Among("seen", 11, - 1, "r_LONG", this), new Among("hen", 11, 2, "", this), new Among("tten", 11, - 1, "r_VI", this), new Among("hin", 11, 3, "", this), new Among("siin", 11, - 1, "r_VI", this), new Among("hon", 11, 4, "", this), new Among("h\u00E4n", 11, 5, "", this), new Among("h\u00F6n", 11, 6, "", this), new Among("\u00E4", - 1, 8, "", this), new Among("ll\u00E4", 22, - 1, "", this), new Among("n\u00E4", 22, - 1, "", this), new Among("ss\u00E4", 22, - 1, "", this), new Among("t\u00E4", 22, - 1, "", this), new Among("lt\u00E4", 26, - 1, "", this), new Among("st\u00E4", 26, - 1, "", this), new Among("tt\u00E4", 26, 9, "", this)};
			a7 = new Among[]{new Among("eja", - 1, - 1, "", this), new Among("mma", - 1, 1, "", this), new Among("imma", 1, - 1, "", this), new Among("mpa", - 1, 1, "", this), new Among("impa", 3, - 1, "", this), new Among("mmi", - 1, 1, "", this), new Among("immi", 5, - 1, "", this), new Among("mpi", - 1, 1, "", this), new Among("impi", 7, - 1, "", this), new Among("ej\u00E4", - 1, - 1, "", this), new Among("mm\u00E4", - 1, 1, "", this), new Among("imm\u00E4", 10, - 1, "", this), new Among("mp\u00E4", - 1, 1, "", this), new Among("imp\u00E4", 12, - 1, "", this)};
			a8 = new Among[]{new Among("i", - 1, - 1, "", this), new Among("j", - 1, - 1, "", this)};
			a9 = new Among[]{new Among("mma", - 1, 1, "", this), new Among("imma", 0, - 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private Among[] a6;
		private Among[] a7;
		private Among[] a8;
		private Among[] a9;

        private static readonly char[] g_AEI = new char[]{(char) (17), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8)};
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8), (char) (0), (char) (32)};
		private static readonly char[] g_V = new char[]{(char) (17), (char) (65), (char) (16), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8), (char) (0), (char) (32)};
		private static readonly char[] gParticleEnd = new char[]{(char) (17), (char) (97), (char) (24), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8), (char) (0), (char) (32)};
		
		private bool B_ending_removed;
		private System.Text.StringBuilder S_x = new System.Text.StringBuilder();
		private int I_p2;
		private int I_p1;
		
		protected internal virtual void  copyFrom(FinnishStemmer other)
		{
			B_ending_removed = other.B_ending_removed;
			S_x = other.S_x;
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			base.copyFrom(other);
		}
		
		private bool rMarkRegions()
		{
			int v1;
			int v3;
			// (, line 41
			I_p1 = limit;
			I_p2 = limit;
			// goto, line 46
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
			
			// gopast, line 46
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
			
			// setmark p1, line 46
			I_p1 = cursor;
			// goto, line 47
			while (true)
			{
				v3 = cursor;
				do 
				{
					if (!(inGrouping(gV, 97, 246)))
					{
						goto lab5Brk;
					}
					cursor = v3;
					goto golab4Brk;
				}
				while (false);

lab5Brk: ;
				
				cursor = v3;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab4Brk: ;
			
			// gopast, line 47
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 246)))
					{
						goto lab7Brk;
					}
					goto golab6Brk;
				}
				while (false);

lab7Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab6Brk: ;
			
			// setmark p2, line 47
			I_p2 = cursor;
			return true;
		}
		
		private bool r_R2()
		{
			if (!(I_p2 <= cursor))
			{
				return false;
			}
			return true;
		}
		
		private bool rParticleEtc()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 54
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
			// [, line 55
			ket = cursor;
			// substring, line 55
			amongVar = findAmongB(a0, 10);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 55
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 62
					if (!(inGroupingB(gParticleEnd, 97, 246)))
					{
						return false;
					}
					break;
				
				case 2: 
					// (, line 64
					// call R2, line 64
					if (!r_R2())
					{
						return false;
					}
					break;
				}
			// delete, line 66
			sliceDel();
			return true;
		}
		
		private bool rPossessive()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			// (, line 68
			// setlimit, line 69
			v1 = limit - cursor;
			// tomark, line 69
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 69
			// [, line 69
			ket = cursor;
			// substring, line 69
			amongVar = findAmongB(a4, 9);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 69
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 72
					// not, line 72
					{
						v3 = limit - cursor;
						do 
						{
							// literal, line 72
							if (!(eqSB(1, "k")))
							{
								goto lab0Brk;
							}
							return false;
						}
						while (false);

lab0Brk: ;
						
						cursor = limit - v3;
					}
					// delete, line 72
					sliceDel();
					break;
				
				case 2: 
					// (, line 74
					// delete, line 74
					sliceDel();
					// [, line 74
					ket = cursor;
					// literal, line 74
					if (!(eqSB(3, "kse")))
					{
						return false;
					}
					// ], line 74
					bra = cursor;
					// <-, line 74
					sliceFrom("ksi");
					break;
				
				case 3: 
					// (, line 78
					// delete, line 78
					sliceDel();
					break;
				
				case 4: 
					// (, line 81
					// among, line 81
					if (findAmongB(a1, 6) == 0)
					{
						return false;
					}
					// delete, line 81
					sliceDel();
					break;
				
				case 5: 
					// (, line 83
					// among, line 83
					if (findAmongB(a2, 6) == 0)
					{
						return false;
					}
					// delete, line 84
					sliceDel();
					break;
				
				case 6: 
					// (, line 86
					// among, line 86
					if (findAmongB(a3, 2) == 0)
					{
						return false;
					}
					// delete, line 86
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_LONG()
		{
			// among, line 91
			if (findAmongB(a5, 7) == 0)
			{
				return false;
			}
			return true;
		}
		
		private bool r_VI()
		{
			// (, line 93
			// literal, line 93
			if (!(eqSB(1, "i")))
			{
				return false;
			}
			if (!(inGroupingB(g_V, 97, 246)))
			{
				return false;
			}
			return true;
		}
		
		private bool rCase()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 95
			// setlimit, line 96
			v1 = limit - cursor;
			// tomark, line 96
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 96
			// [, line 96
			ket = cursor;
			// substring, line 96
			amongVar = findAmongB(a6, 30);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 96
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 98
					// literal, line 98
					if (!(eqSB(1, "a")))
					{
						return false;
					}
					break;
				
				case 2: 
					// (, line 99
					// literal, line 99
					if (!(eqSB(1, "e")))
					{
						return false;
					}
					break;
				
				case 3: 
					// (, line 100
					// literal, line 100
					if (!(eqSB(1, "i")))
					{
						return false;
					}
					break;
				
				case 4: 
					// (, line 101
					// literal, line 101
					if (!(eqSB(1, "o")))
					{
						return false;
					}
					break;
				
				case 5: 
					// (, line 102
					// literal, line 102
					if (!(eqSB(1, "\u00E4")))
					{
						return false;
					}
					break;
				
				case 6: 
					// (, line 103
					// literal, line 103
					if (!(eqSB(1, "\u00F6")))
					{
						return false;
					}
					break;
				
				case 7: 
					// (, line 111
					// try, line 111
					v3 = limit - cursor;
					do 
					{
						// (, line 111
						// and, line 113
						v4 = limit - cursor;
						// or, line 112
						do 
						{
							v5 = limit - cursor;
							do 
							{
								// call LONG, line 111
								if (!r_LONG())
								{
									goto lab2Brk;
								}
								goto lab1Brk;
							}
							while (false);

lab2Brk: ;
							
							cursor = limit - v5;
							// literal, line 112
							if (!(eqSB(2, "ie")))
							{
								cursor = limit - v3;
								goto lab0Brk;
							}
						}
						while (false);

lab1Brk: ;
						
						cursor = limit - v4;
						// next, line 113
						if (cursor <= limitBackward)
						{
							cursor = limit - v3;
							goto lab0Brk;
						}
						cursor--;
						// ], line 113
						bra = cursor;
					}
					while (false);

lab0Brk: ;
					
					break;
				
				case 8: 
					// (, line 119
					if (!(inGroupingB(gV, 97, 246)))
					{
						return false;
					}
					if (!(outGroupingB(gV, 97, 246)))
					{
						return false;
					}
					break;
				
				case 9: 
					// (, line 121
					// literal, line 121
					if (!(eqSB(1, "e")))
					{
						return false;
					}
					break;
				}
			// delete, line 138
			sliceDel();
			// set endingRemoved, line 139
			B_ending_removed = true;
			return true;
		}
		
		private bool rOtherEndings()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			// (, line 141
			// setlimit, line 142
			v1 = limit - cursor;
			// tomark, line 142
			if (cursor < I_p2)
			{
				return false;
			}
			cursor = I_p2;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 142
			// [, line 142
			ket = cursor;
			// substring, line 142
			amongVar = findAmongB(a7, 14);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 142
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 146
					// not, line 146
					{
						v3 = limit - cursor;
						do 
						{
							// literal, line 146
							if (!(eqSB(2, "po")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v3;
					}
					break;
				}
			// delete, line 151
			sliceDel();
			return true;
		}
		
		private bool rIPlural()
		{
			int v1;
			int v2;
			// (, line 153
			// setlimit, line 154
			v1 = limit - cursor;
			// tomark, line 154
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 154
			// [, line 154
			ket = cursor;
			// substring, line 154
			if (findAmongB(a8, 2) == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 154
			bra = cursor;
			limitBackward = v2;
			// delete, line 158
			sliceDel();
			return true;
		}
		
		private bool rTPlural()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			// (, line 160
			// setlimit, line 161
			v1 = limit - cursor;
			// tomark, line 161
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 161
			// [, line 162
			ket = cursor;
			// literal, line 162
			if (!(eqSB(1, "t")))
			{
				limitBackward = v2;
				return false;
			}
			// ], line 162
			bra = cursor;
			// test, line 162
			v3 = limit - cursor;
			if (!(inGroupingB(gV, 97, 246)))
			{
				limitBackward = v2;
				return false;
			}
			cursor = limit - v3;
			// delete, line 163
			sliceDel();
			limitBackward = v2;
			// setlimit, line 165
			v4 = limit - cursor;
			// tomark, line 165
			if (cursor < I_p2)
			{
				return false;
			}
			cursor = I_p2;
			v5 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v4;
			// (, line 165
			// [, line 165
			ket = cursor;
			// substring, line 165
			amongVar = findAmongB(a9, 2);
			if (amongVar == 0)
			{
				limitBackward = v5;
				return false;
			}
			// ], line 165
			bra = cursor;
			limitBackward = v5;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 167
					// not, line 167
					{
						v6 = limit - cursor;
						do 
						{
							// literal, line 167
							if (!(eqSB(2, "po")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v6;
					}
					break;
				}
			// delete, line 170
			sliceDel();
			return true;
		}
		
		private bool rTidy()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			int v7;
			int v8;
			int v9;
			// (, line 172
			// setlimit, line 173
			v1 = limit - cursor;
			// tomark, line 173
			if (cursor < I_p1)
			{
				return false;
			}
			cursor = I_p1;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 173
			// do, line 174
			v3 = limit - cursor;
			do 
			{
				// (, line 174
				// and, line 174
				v4 = limit - cursor;
				// call LONG, line 174
				if (!r_LONG())
				{
					goto lab0Brk;
				}
				cursor = limit - v4;
				// (, line 174
				// [, line 174
				ket = cursor;
				// next, line 174
				if (cursor <= limitBackward)
				{
					goto lab0Brk;
				}
				cursor--;
				// ], line 174
				bra = cursor;
				// delete, line 174
				sliceDel();
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v3;
			// do, line 175
			v5 = limit - cursor;
			do 
			{
				// (, line 175
				// [, line 175
				ket = cursor;
				if (!(inGroupingB(g_AEI, 97, 228)))
				{
					goto lab1Brk;
				}
				// ], line 175
				bra = cursor;
				if (!(outGroupingB(gV, 97, 246)))
				{
					goto lab1Brk;
				}
				// delete, line 175
				sliceDel();
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v5;
			// do, line 176
			v6 = limit - cursor;
			do 
			{
				// (, line 176
				// [, line 176
				ket = cursor;
				// literal, line 176
				if (!(eqSB(1, "j")))
				{
					goto lab2Brk;
				}
				// ], line 176
				bra = cursor;
				// or, line 176
				do 
				{
					v7 = limit - cursor;
					do 
					{
						// literal, line 176
						if (!(eqSB(1, "o")))
						{
							goto lab4Brk;
						}
						goto lab3Brk;
					}
					while (false);

lab4Brk: ;
					
					cursor = limit - v7;
					// literal, line 176
					if (!(eqSB(1, "u")))
					{
						goto lab2Brk;
					}
				}
				while (false);

lab3Brk: ;
				
				// delete, line 176
				sliceDel();
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v6;
			// do, line 177
			v8 = limit - cursor;
			do 
			{
				// (, line 177
				// [, line 177
				ket = cursor;
				// literal, line 177
				if (!(eqSB(1, "o")))
				{
					goto lab5Brk;
				}
				// ], line 177
				bra = cursor;
				// literal, line 177
				if (!(eqSB(1, "j")))
				{
					goto lab5Brk;
				}
				// delete, line 177
				sliceDel();
			}
			while (false);

lab5Brk: ;
			
			cursor = limit - v8;
			limitBackward = v2;
			// goto, line 179
			while (true)
			{
				v9 = limit - cursor;
				do 
				{
					if (!(outGroupingB(gV, 97, 246)))
					{
						goto lab7Brk;
					}
					cursor = limit - v9;
					goto golab6Brk;
				}
				while (false);

lab7Brk: ;
				
				cursor = limit - v9;
				if (cursor <= limitBackward)
				{
					return false;
				}
				cursor--;
			}

golab6Brk: ;
			
			// [, line 179
			ket = cursor;
			// next, line 179
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 179
			bra = cursor;
			// -> x, line 179
			S_x = sliceTo(S_x);
			// name x, line 179
			if (!(eqVB(S_x)))
			{
				return false;
			}
			// delete, line 179
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
			int v6;
			int v7;
			int v8;
			int v9;
			// (, line 183
			// do, line 185
			v1 = cursor;
			do 
			{
				// call markRegions, line 185
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// unset endingRemoved, line 186
			B_ending_removed = false;
			// backwards, line 187
			limitBackward = cursor; cursor = limit;
			// (, line 187
			// do, line 188
			v2 = limit - cursor;
			do 
			{
				// call particleEtc, line 188
				if (!rParticleEtc())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 189
			v3 = limit - cursor;
			do 
			{
				// call possessive, line 189
				if (!rPossessive())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			// do, line 190
			v4 = limit - cursor;
			do 
			{
				// call case, line 190
				if (!rCase())
				{
					goto lab3Brk;
				}
			}
			while (false);

lab3Brk: ;
			
			cursor = limit - v4;
			// do, line 191
			v5 = limit - cursor;
			do 
			{
				// call otherEndings, line 191
				if (!rOtherEndings())
				{
					goto lab4Brk;
				}
			}
			while (false);

lab4Brk: ;
			
			cursor = limit - v5;
			// or, line 192

			do 
			{
				v6 = limit - cursor;
				do 
				{
					// (, line 192
					// Boolean test endingRemoved, line 192
					if (!(B_ending_removed))
					{
						goto lab6Brk;
					}
					// do, line 192
					v7 = limit - cursor;
					do 
					{
						// call iPlural, line 192
						if (!rIPlural())
						{
							goto lab7Brk;
						}
					}
					while (false);

lab7Brk: ;
					
					cursor = limit - v7;
					goto lab6Brk;
				}
				while (false);

lab6Brk: ;
				
				cursor = limit - v6;
				// do, line 192
				v8 = limit - cursor;
				do 
				{
					// call tPlural, line 192
					if (!rTPlural())
					{
						goto lab8Brk;
					}
				}
				while (false);

lab8Brk: ;
				
				cursor = limit - v8;
			}
			while (false);

lab5Brk: ;

			// do, line 193
			v9 = limit - cursor;
			do 
			{
				// call tidy, line 193
				if (!rTidy())
				{
					goto lab9Brk;
				}
			}
			while (false);

lab9Brk: ;
			
			cursor = limit - v9;
			cursor = limitBackward; return true;
		}
	}
}
