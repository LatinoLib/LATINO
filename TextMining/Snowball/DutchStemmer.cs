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
    public class DutchStemmer : SnowballProgram, ISnowballStemmer
	{
		public DutchStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 6, "", this), new Among("\u00E1", 0, 1, "", this), new Among("\u00E4", 0, 1, "", this), new Among("\u00E9", 0, 2, "", this), new Among("\u00EB", 0, 2, "", this), new Among("\u00ED", 0, 3, "", this), new Among("\u00EF", 0, 3, "", this), new Among("\u00F3", 0, 4, "", this), new Among("\u00F6", 0, 4, "", this), new Among("\u00FA", 0, 5, "", this), new Among("\u00FC", 0, 5, "", this)};
			a1 = new Among[]{new Among("", - 1, 3, "", this), new Among("I", 0, 2, "", this), new Among("Y", 0, 1, "", this)};
			a2 = new Among[]{new Among("dd", - 1, - 1, "", this), new Among("kk", - 1, - 1, "", this), new Among("tt", - 1, - 1, "", this)};
			a3 = new Among[]{new Among("ene", - 1, 2, "", this), new Among("se", - 1, 3, "", this), new Among("en", - 1, 2, "", this), new Among("heden", 2, 1, "", this), new Among("s", - 1, 3, "", this)};
			a4 = new Among[]{new Among("end", - 1, 1, "", this), new Among("ig", - 1, 2, "", this), new Among("ing", - 1, 1, "", this), new Among("lijk", - 1, 3, "", this), new Among("baar", - 1, 4, "", this), new Among("bar", - 1, 5, "", this)};
			a5 = new Among[]{new Among("aa", - 1, - 1, "", this), new Among("ee", - 1, - 1, "", this), new Among("oo", - 1, - 1, "", this), new Among("uu", - 1, - 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128)};
		private static readonly char[] g_v_I = new char[]{(char) (1), (char) (0), (char) (0), (char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128)};
		private static readonly char[] gVJ = new char[]{(char) (17), (char) (67), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128)};
		
		private int I_p2;
		private int I_p1;
		private bool B_e_found;
		
		protected internal virtual void  copyFrom(DutchStemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			B_e_found = other.B_e_found;
			base.copyFrom(other);
		}
		
		private bool rPrelude()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			// (, line 41
			// test, line 42
			v1 = cursor;
			// repeat, line 42
			while (true)
			{
				v2 = cursor;
				do 
				{
					// (, line 42
					// [, line 43
					bra = cursor;
					// substring, line 43
					amongVar = findAmong(a0, 11);
					if (amongVar == 0)
					{
						goto lab1Brk;
					}
					// ], line 43
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab1Brk;
						
						case 1: 
							// (, line 45
							// <-, line 45
							sliceFrom("a");
							break;
						
						case 2: 
							// (, line 47
							// <-, line 47
							sliceFrom("e");
							break;
						
						case 3: 
							// (, line 49
							// <-, line 49
							sliceFrom("i");
							break;
						
						case 4: 
							// (, line 51
							// <-, line 51
							sliceFrom("o");
							break;
						
						case 5: 
							// (, line 53
							// <-, line 53
							sliceFrom("u");
							break;
						
						case 6: 
							// (, line 54
							// next, line 54
							if (cursor >= limit)
							{
								goto lab1Brk;
							}
							cursor++;
							break;
						}
					goto replab0;
				}
				while (false);

lab1Brk: ;
				
				cursor = v2;
				goto replab0Brk;

replab0: ;
			}

replab0Brk: ;
			
			cursor = v1;
			// try, line 57
			v3 = cursor;
			do 
			{
				// (, line 57
				// [, line 57
				bra = cursor;
				// literal, line 57
				if (!(eqS(1, "y")))
				{
					cursor = v3;
					goto lab2Brk;
				}
				// ], line 57
				ket = cursor;
				// <-, line 57
				sliceFrom("Y");
			}
			while (false);

lab2Brk: ;
			
			// repeat, line 58
			while (true)
			{
				v4 = cursor;
				do 
				{
					// goto, line 58
					while (true)
					{
						v5 = cursor;
						do 
						{
							// (, line 58
							if (!(inGrouping(gV, 97, 232)))
							{
								goto lab6Brk;
							}
							// [, line 59
							bra = cursor;
							// or, line 59
							do 
							{
								v6 = cursor;
								do 
								{
									// (, line 59
									// literal, line 59
									if (!(eqS(1, "i")))
									{
										goto lab8Brk;
									}
									// ], line 59
									ket = cursor;
									if (!(inGrouping(gV, 97, 232)))
									{
										goto lab8Brk;
									}
									// <-, line 59
									sliceFrom("I");
									goto lab7Brk;
								}
								while (false);

lab8Brk: ;
								
								cursor = v6;
								// (, line 60
								// literal, line 60
								if (!(eqS(1, "y")))
								{
									goto lab6Brk;
								}
								// ], line 60
								ket = cursor;
								// <-, line 60
								sliceFrom("Y");
							}
							while (false);

lab7Brk: ;
							
							cursor = v5;
							goto golab5Brk;
						}
						while (false);

lab6Brk: ;
						
						cursor = v5;
						if (cursor >= limit)
						{
							goto lab4Brk;
						}
						cursor++;
					}

golab5Brk: ;
					
					goto replab3;
				}
				while (false);

lab4Brk: ;
				
				cursor = v4;
				goto replab3Brk;

replab3: ;
			}

replab3Brk: ;
			
			return true;
		}
		
		private bool rMarkRegions()
		{
			// (, line 64
			I_p1 = limit;
			I_p2 = limit;
			// gopast, line 69
			while (true)
			{
				do 
				{
					if (!(inGrouping(gV, 97, 232)))
					{
						goto lab3Brk;
					}
					goto golab0Brk;
				}
				while (false);

lab3Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab0Brk: ;
			
			// gopast, line 69
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 232)))
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
			
			// setmark p1, line 69
			I_p1 = cursor;
			// try, line 70
			do 
			{
				// (, line 70
				if (!(I_p1 < 3))
				{
					goto lab5Brk;
				}
				I_p1 = 3;
			}
			while (false);

lab5Brk: ;
			
			// gopast, line 71
			while (true)
			{
				do 
				{
					if (!(inGrouping(gV, 97, 232)))
					{
						goto lab9Brk;
					}
					goto golab6Brk;
				}
				while (false);

lab9Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab6Brk: ;
			
			// gopast, line 71
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 232)))
					{
						goto lab9Brk;
					}
					goto golab7Brk;
				}
				while (false);

lab9Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab7Brk: ;
			
			// setmark p2, line 71
			I_p2 = cursor;
			return true;
		}
		
		private bool rPostlude()
		{
			int amongVar;
			int v1;
			// repeat, line 75
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 75
					// [, line 77
					bra = cursor;
					// substring, line 77
					amongVar = findAmong(a1, 3);
					if (amongVar == 0)
					{
						goto lab5Brk;
					}
					// ], line 77
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab5Brk;
						
						case 1: 
							// (, line 78
							// <-, line 78
							sliceFrom("y");
							break;
						
						case 2: 
							// (, line 79
							// <-, line 79
							sliceFrom("i");
							break;
						
						case 3: 
							// (, line 80
							// next, line 80
							if (cursor >= limit)
							{
								goto lab5Brk;
							}
							cursor++;
							break;
						}
					goto replab1;
				}
				while (false);

lab5Brk: ;
				
				cursor = v1;
				goto replab1Brk;

replab1: ;
			}

replab1Brk: ;
			
			return true;
		}
		
		private bool r_R1()
		{
			if (!(I_p1 <= cursor))
			{
				return false;
			}
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
		
		private bool rUndouble()
		{
			int v1;
			// (, line 90
			// test, line 91
			v1 = limit - cursor;
			// among, line 91
			if (findAmongB(a2, 3) == 0)
			{
				return false;
			}
			cursor = limit - v1;
			// [, line 91
			ket = cursor;
			// next, line 91
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 91
			bra = cursor;
			// delete, line 91
			sliceDel();
			return true;
		}
		
		private bool rEEnding()
		{
			int v1;
			// (, line 94
			// unset eFound, line 95
			B_e_found = false;
			// [, line 96
			ket = cursor;
			// literal, line 96
			if (!(eqSB(1, "e")))
			{
				return false;
			}
			// ], line 96
			bra = cursor;
			// call R1, line 96
			if (!r_R1())
			{
				return false;
			}
			// test, line 96
			v1 = limit - cursor;
			if (!(outGroupingB(gV, 97, 232)))
			{
				return false;
			}
			cursor = limit - v1;
			// delete, line 96
			sliceDel();
			// set eFound, line 97
			B_e_found = true;
			// call undouble, line 98
			if (!rUndouble())
			{
				return false;
			}
			return true;
		}
		
		private bool rEnEnding()
		{
			int v1;
			int v2;
			// (, line 101
			// call R1, line 102
			if (!r_R1())
			{
				return false;
			}
			// and, line 102
			v1 = limit - cursor;
			if (!(outGroupingB(gV, 97, 232)))
			{
				return false;
			}
			cursor = limit - v1;
			// not, line 102
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 102
					if (!(eqSB(3, "gem")))
					{
						goto lab0Brk;
					}
					return false;
				}
				while (false);

lab0Brk: ;
				
				cursor = limit - v2;
			}
			// delete, line 102
			sliceDel();
			// call undouble, line 103
			if (!rUndouble())
			{
				return false;
			}
			return true;
		}
		
		private bool rStandardSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			int v7;
			int v8;
			int v9;
			int v10;
			// (, line 106
			// do, line 107
			v1 = limit - cursor;
			do 
			{
				// (, line 107
				// [, line 108
				ket = cursor;
				// substring, line 108
				amongVar = findAmongB(a3, 5);
				if (amongVar == 0)
				{
					goto lab0Brk;
				}
				// ], line 108
				bra = cursor;
				switch (amongVar)
				{
					
					case 0: 
						goto lab0Brk;
					
					case 1: 
						// (, line 110
						// call R1, line 110
						if (!r_R1())
						{
							goto lab0Brk;
						}
						// <-, line 110
						sliceFrom("heid");
						break;
					
					case 2: 
						// (, line 113
						// call enEnding, line 113
						if (!rEnEnding())
						{
							goto lab0Brk;
						}
						break;
					
					case 3: 
						// (, line 116
						// call R1, line 116
						if (!r_R1())
						{
							goto lab0Brk;
						}
						if (!(outGroupingB(gVJ, 97, 232)))
						{
							goto lab0Brk;
						}
						// delete, line 116
						sliceDel();
						break;
					}
			}
			while (false);

lab0Brk: ;

			cursor = limit - v1;
			// do, line 120
			v2 = limit - cursor;
			do 
			{
				// call eEnding, line 120
				if (!rEEnding())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;

			cursor = limit - v2;
			// do, line 122
			v3 = limit - cursor;
			do 
			{
				// (, line 122
				// [, line 122
				ket = cursor;
				// literal, line 122
				if (!(eqSB(4, "heid")))
				{
					goto lab2Brk;
				}
				// ], line 122
				bra = cursor;
				// call R2, line 122
				if (!r_R2())
				{
					goto lab2Brk;
				}
				// not, line 122
				{
					v4 = limit - cursor;
					do 
					{
						// literal, line 122
						if (!(eqSB(1, "c")))
						{
							goto lab3Brk;
						}
						goto lab2Brk;
					}
					while (false);

lab3Brk: ;
					
					cursor = limit - v4;
				}
				// delete, line 122
				sliceDel();
				// [, line 123
				ket = cursor;
				// literal, line 123
				if (!(eqSB(2, "en")))
				{
					goto lab2Brk;
				}
				// ], line 123
				bra = cursor;
				// call enEnding, line 123
				if (!rEnEnding())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			// do, line 126
			v5 = limit - cursor;
			do 
			{
				// (, line 126
				// [, line 127
				ket = cursor;
				// substring, line 127
				amongVar = findAmongB(a4, 6);
				if (amongVar == 0)
				{
					goto lab4Brk;
				}
				// ], line 127
				bra = cursor;
				switch (amongVar)
				{
					
					case 0: 
						goto lab4Brk;
					
					case 1: 
						// (, line 129
						// call R2, line 129
						if (!r_R2())
						{
							goto lab4Brk;
						}
						// delete, line 129
						sliceDel();
						// or, line 130
						do 
						{
							v6 = limit - cursor;
							do 
							{
								// (, line 130
								// [, line 130
								ket = cursor;
								// literal, line 130
								if (!(eqSB(2, "ig")))
								{
									goto lab6Brk;
								}
								// ], line 130
								bra = cursor;
								// call R2, line 130
								if (!r_R2())
								{
									goto lab6Brk;
								}
								// not, line 130
								{
									v7 = limit - cursor;
									do 
									{
										// literal, line 130
										if (!(eqSB(1, "e")))
										{
											goto lab7Brk;
										}
										goto lab6Brk;
									}
									while (false);

lab7Brk: ;
									
									cursor = limit - v7;
								}
								// delete, line 130
								sliceDel();
								goto lab5Brk;
							}
							while (false);

lab6Brk: ;
							
							cursor = limit - v6;
							// call undouble, line 130
							if (!rUndouble())
							{
								goto lab4Brk;
							}
						}
						while (false);

lab5Brk: ;
						
						break;
					
					case 2: 
						// (, line 133
						// call R2, line 133
						if (!r_R2())
						{
							goto lab4Brk;
						}
						// not, line 133
						{
							v8 = limit - cursor;
							do 
							{
								// literal, line 133
								if (!(eqSB(1, "e")))
								{
									goto lab8Brk;
								}
								goto lab4Brk;
							}
							while (false);

lab8Brk: ;

							cursor = limit - v8;
						}
						// delete, line 133
						sliceDel();
						break;
					
					case 3: 
						// (, line 136
						// call R2, line 136
						if (!r_R2())
						{
							goto lab4Brk;
						}
						// delete, line 136
						sliceDel();
						// call eEnding, line 136
						if (!rEEnding())
						{
							goto lab4Brk;
						}
						break;
					
					case 4: 
						// (, line 139
						// call R2, line 139
						if (!r_R2())
						{
							goto lab4Brk;
						}
						// delete, line 139
						sliceDel();
						break;
					
					case 5: 
						// (, line 142
						// call R2, line 142
						if (!r_R2())
						{
							goto lab4Brk;
						}
						// Boolean test eFound, line 142
						if (!(B_e_found))
						{
							goto lab4Brk;
						}
						// delete, line 142
						sliceDel();
						break;
					}
			}
			while (false);

lab4Brk: ;
			
			cursor = limit - v5;
			// do, line 146
			v9 = limit - cursor;
			do 
			{
				// (, line 146
				if (!(outGroupingB(g_v_I, 73, 232)))
				{
					goto lab9Brk;
				}
				// test, line 148
				v10 = limit - cursor;
				// (, line 148
				// among, line 149
				if (findAmongB(a5, 4) == 0)
				{
					goto lab9Brk;
				}
				if (!(outGroupingB(gV, 97, 232)))
				{
					goto lab9Brk;
				}
				cursor = limit - v10;
				// [, line 152
				ket = cursor;
				// next, line 152
				if (cursor <= limitBackward)
				{
					goto lab9Brk;
				}
				cursor--;
				// ], line 152
				bra = cursor;
				// delete, line 152
				sliceDel();
			}
			while (false);

lab9Brk: ;
			
			cursor = limit - v9;
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 157
			// do, line 159
			v1 = cursor;
			do 
			{
				// call prelude, line 159
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;

			cursor = v1;
			// do, line 160
			v2 = cursor;
			do 
			{
				// call markRegions, line 160
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 161
			limitBackward = cursor; cursor = limit;
			// do, line 162
			v3 = limit - cursor;
			do 
			{
				// call standardSuffix, line 162
				if (!rStandardSuffix())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			cursor = limitBackward; // do, line 163
			v4 = cursor;
			do 
			{
				// call postlude, line 163
				if (!rPostlude())
				{
					goto lab3Brk;
				}
			}
			while (false);

lab3Brk: ;
			
			cursor = v4;
			return true;
		}
	}
}
