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
    public class German2Stemmer : SnowballProgram, ISnowballStemmer
	{
		public German2Stemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 6, "", this), new Among("ae", 0, 2, "", this), new Among("oe", 0, 3, "", this), new Among("qu", 0, 5, "", this), new Among("ue", 0, 4, "", this), new Among("\u00DF", 0, 1, "", this)};
			a1 = new Among[]{new Among("", - 1, 6, "", this), new Among("U", 0, 2, "", this), new Among("Y", 0, 1, "", this), new Among("\u00E4", 0, 3, "", this), new Among("\u00F6", 0, 4, "", this), new Among("\u00FC", 0, 5, "", this)};
			a2 = new Among[]{new Among("e", - 1, 1, "", this), new Among("em", - 1, 1, "", this), new Among("en", - 1, 1, "", this), new Among("ern", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("s", - 1, 2, "", this), new Among("es", 5, 1, "", this)};
			a3 = new Among[]{new Among("en", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("st", - 1, 2, "", this), new Among("est", 2, 1, "", this)};
			a4 = new Among[]{new Among("ig", - 1, 1, "", this), new Among("lich", - 1, 1, "", this)};
			a5 = new Among[]{new Among("end", - 1, 1, "", this), new Among("ig", - 1, 2, "", this), new Among("ung", - 1, 1, "", this), new Among("lich", - 1, 3, "", this), new Among("isch", - 1, 2, "", this), new Among("ik", - 1, 2, "", this), new Among("heit", - 1, 3, "", this), new Among("keit", - 1, 4, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8), (char) (0), (char) (32), (char) (8)};
		private static readonly char[] gSEnding = new char[]{(char) (117), (char) (30), (char) (5)};
		private static readonly char[] gStEnding = new char[]{(char) (117), (char) (30), (char) (4)};
		
		private int I_p2;
		private int I_p1;

        protected internal virtual void  copyFrom(German2Stemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
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
			// (, line 28
			// test, line 30
			v1 = cursor;
			// repeat, line 30
			while (true)
			{
				v2 = cursor;
				do 
				{
					// goto, line 30
					while (true)
					{
						v3 = cursor;
						do 
						{
							// (, line 30
							if (!(inGrouping(gV, 97, 252)))
							{
								goto lab3Brk;
							}
							// [, line 31
							bra = cursor;
							// or, line 31
							do 
							{
								v4 = cursor;
								do 
								{
									// (, line 31
									// literal, line 31
									if (!(eqS(1, "u")))
									{
										goto lab5Brk;
									}
									// ], line 31
									ket = cursor;
									if (!(inGrouping(gV, 97, 252)))
									{
										goto lab5Brk;
									}
									// <-, line 31
									sliceFrom("U");
									goto lab4Brk;
								}
								while (false);

lab5Brk: ;
								
								cursor = v4;
								// (, line 32
								// literal, line 32
								if (!(eqS(1, "y")))
								{
									goto lab3Brk;
								}
								// ], line 32
								ket = cursor;
								if (!(inGrouping(gV, 97, 252)))
								{
									goto lab3Brk;
								}
								// <-, line 32
								sliceFrom("Y");
							}
							while (false);

lab4Brk: ;
							
							cursor = v3;
							goto golab2Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = v3;
						if (cursor >= limit)
						{
							goto lab1Brk;
						}
						cursor++;
					}

golab2Brk: ;
					
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
			// repeat, line 35
			while (true)
			{
				v5 = cursor;
				do 
				{
					// (, line 35
					// [, line 36
					bra = cursor;
					// substring, line 36
					amongVar = findAmong(a0, 6);
					if (amongVar == 0)
					{
						goto lab7Brk;
					}
					// ], line 36
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab7Brk;
						
						case 1: 
							// (, line 37
							// <-, line 37
							sliceFrom("ss");
							break;
						
						case 2: 
							// (, line 38
							// <-, line 38
							sliceFrom("\u00E4");
							break;
						
						case 3: 
							// (, line 39
							// <-, line 39
							sliceFrom("\u00F6");
							break;
						
						case 4: 
							// (, line 40
							// <-, line 40
							sliceFrom("\u00FC");
							break;
						
						case 5: 
							// (, line 41
							// hop, line 41
							{
								int c = cursor + 2;
								if (0 > c || c > limit)
								{
									goto lab7Brk;
								}
								cursor = c;
							}
							break;
						
						case 6: 
							// (, line 42
							// next, line 42
							if (cursor >= limit)
							{
								goto lab7Brk;
							}
							cursor++;
							break;
						}
					goto replab6;
				}
				while (false);

lab7Brk: ;
				
				cursor = v5;
				goto replab6Brk;

replab6: ;
			}

replab6Brk: ;
			
			return true;
		}
		
		private bool rMarkRegions()
		{
			// (, line 48
			I_p1 = limit;
			I_p2 = limit;
			// gopast, line 53
			while (true)
			{
				do 
				{
					if (!(inGrouping(gV, 97, 252)))
					{
						goto lab1Brk;
					}
					goto golab0Brk;
				}
				while (false);

lab1Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab0Brk: ;
			
			// gopast, line 53
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 252)))
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
			
			// setmark p1, line 53
			I_p1 = cursor;
			// try, line 54
			do 
			{
				// (, line 54
				if (!(I_p1 < 3))
				{
					goto lab4Brk;
				}
				I_p1 = 3;
			}
			while (false);

lab4Brk: ;
			
			// gopast, line 55
			while (true)
			{
				do 
				{
					if (!(inGrouping(gV, 97, 252)))
					{
						goto lab6Brk;
					}
					goto golab5Brk;
				}
				while (false);

lab6Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab5Brk: ;
			
			// gopast, line 55
			while (true)
			{
				do 
				{
					if (!(outGrouping(gV, 97, 252)))
					{
						goto lab8Brk;
					}
					goto golab7Brk;
				}
				while (false);

lab8Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab7Brk: ;
			
			// setmark p2, line 55
			I_p2 = cursor;
			return true;
		}
		
		private bool rPostlude()
		{
			int amongVar;
			int v1;
			// repeat, line 59
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 59
					// [, line 61
					bra = cursor;
					// substring, line 61
					amongVar = findAmong(a1, 6);
					if (amongVar == 0)
					{
						goto lab2Brk;
					}
					// ], line 61
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab2Brk;
						
						case 1: 
							// (, line 62
							// <-, line 62
							sliceFrom("y");
							break;
						
						case 2: 
							// (, line 63
							// <-, line 63
							sliceFrom("u");
							break;
						
						case 3: 
							// (, line 64
							// <-, line 64
							sliceFrom("a");
							break;
						
						case 4: 
							// (, line 65
							// <-, line 65
							sliceFrom("o");
							break;
						
						case 5: 
							// (, line 66
							// <-, line 66
							sliceFrom("u");
							break;
						
						case 6: 
							// (, line 67
							// next, line 67
							if (cursor >= limit)
							{
								goto lab2Brk;
							}
							cursor++;
							break;
						}
					goto replab1;
				}
				while (false);

lab2Brk: ;
				
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
			// (, line 77
			// do, line 78
			v1 = limit - cursor;
			do 
			{
				// (, line 78
				// [, line 79
				ket = cursor;
				// substring, line 79
				amongVar = findAmongB(a2, 7);
				if (amongVar == 0)
				{
					goto lab0Brk;
				}
				// ], line 79
				bra = cursor;
				// call R1, line 79
				if (!r_R1())
				{
					goto lab0Brk;
				}
				switch (amongVar)
				{
					
					case 0: 
						goto lab0Brk;
					
					case 1: 
						// (, line 81
						// delete, line 81
						sliceDel();
						break;
					
					case 2: 
						// (, line 84
						if (!(inGroupingB(gSEnding, 98, 116)))
						{
							goto lab0Brk;
						}
						// delete, line 84
						sliceDel();
						break;
					}
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v1;
			// do, line 88
			v2 = limit - cursor;
			do 
			{
				// (, line 88
				// [, line 89
				ket = cursor;
				// substring, line 89
				amongVar = findAmongB(a3, 4);
				if (amongVar == 0)
				{
					goto lab1Brk;
				}
				// ], line 89
				bra = cursor;
				// call R1, line 89
				if (!r_R1())
				{
					goto lab1Brk;
				}
				switch (amongVar)
				{
					
					case 0: 
						goto lab1Brk;
					
					case 1: 
						// (, line 91
						// delete, line 91
						sliceDel();
						break;
					
					case 2: 
						// (, line 94
						if (!(inGroupingB(gStEnding, 98, 116)))
						{
							goto lab1Brk;
						}
						// hop, line 94
						{
							int c = cursor - 3;
							if (limitBackward > c || c > limit)
							{
								goto lab1Brk;
							}
							cursor = c;
						}
						// delete, line 94
						sliceDel();
						break;
					}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 98
			v3 = limit - cursor;
			do 
			{
				// (, line 98
				// [, line 99
				ket = cursor;
				// substring, line 99
				amongVar = findAmongB(a5, 8);
				if (amongVar == 0)
				{
					goto lab2Brk;
				}
				// ], line 99
				bra = cursor;
				// call R2, line 99
				if (!r_R2())
				{
					goto lab2Brk;
				}
				switch (amongVar)
				{
					
					case 0: 
						goto lab2Brk;
					
					case 1: 
						// (, line 101
						// delete, line 101
						sliceDel();
						// try, line 102
						v4 = limit - cursor;
						do 
						{
							// (, line 102
							// [, line 102
							ket = cursor;
							// literal, line 102
							if (!(eqSB(2, "ig")))
							{
								cursor = limit - v4;
								goto lab3Brk;
							}
							// ], line 102
							bra = cursor;
							// not, line 102
							{
								v5 = limit - cursor;
								do 
								{
									// literal, line 102
									if (!(eqSB(1, "e")))
									{
										goto lab4Brk;
									}
									cursor = limit - v4;
									goto lab3Brk;
								}
								while (false);

lab4Brk: ;
								
								cursor = limit - v5;
							}
							// call R2, line 102
							if (!r_R2())
							{
								cursor = limit - v4;
								goto lab3Brk;
							}
							// delete, line 102
							sliceDel();
						}
						while (false);

lab3Brk: ;
						
						break;
					
					case 2: 
						// (, line 105
						// not, line 105
						{
							v6 = limit - cursor;
							do 
							{
								// literal, line 105
								if (!(eqSB(1, "e")))
								{
									goto lab5Brk;
								}
								goto lab2Brk;
							}
							while (false);

lab5Brk: ;
							
							cursor = limit - v6;
						}
						// delete, line 105
						sliceDel();
						break;
					
					case 3: 
						// (, line 108
						// delete, line 108
						sliceDel();
						// try, line 109
						v7 = limit - cursor;
						do 
						{
							// (, line 109
							// [, line 110
							ket = cursor;
							// or, line 110
							do 
							{
								v8 = limit - cursor;
								do 
								{
									// literal, line 110
									if (!(eqSB(2, "er")))
									{
										goto lab8Brk;
									}
									goto lab7Brk;
								}
								while (false);

lab8Brk: ;
								
								cursor = limit - v8;
								// literal, line 110
								if (!(eqSB(2, "en")))
								{
									cursor = limit - v7;
									goto lab6Brk;
								}
							}
							while (false);

lab7Brk: ;
							
							// ], line 110
							bra = cursor;
							// call R1, line 110
							if (!r_R1())
							{
								cursor = limit - v7;
								goto lab6Brk;
							}
							// delete, line 110
							sliceDel();
						}
						while (false);

lab6Brk: ;
						
						break;
					
					case 4: 
						// (, line 114
						// delete, line 114
						sliceDel();
						// try, line 115
						v9 = limit - cursor;
						do 
						{
							// (, line 115
							// [, line 116
							ket = cursor;
							// substring, line 116
							amongVar = findAmongB(a4, 2);
							if (amongVar == 0)
							{
								cursor = limit - v9;
								goto lab9Brk;
							}
							// ], line 116
							bra = cursor;
							// call R2, line 116
							if (!r_R2())
							{
								cursor = limit - v9;
								goto lab9Brk;
							}
							switch (amongVar)
							{
								
								case 0: 
									cursor = limit - v9;
									goto lab9Brk;
								
								case 1: 
									// (, line 118
									// delete, line 118
									sliceDel();
									break;
								}
						}
						while (false);

lab9Brk: ;
						
						break;
					}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 128
			// do, line 129
			v1 = cursor;
			do 
			{
				// call prelude, line 129
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 130
			v2 = cursor;
			do 
			{
				// call markRegions, line 130
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 131
			limitBackward = cursor; cursor = limit;
			// do, line 132
			v3 = limit - cursor;
			do 
			{
				// call standardSuffix, line 132
				if (!rStandardSuffix())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			cursor = limitBackward; // do, line 133
			v4 = cursor;
			do 
			{
				// call postlude, line 133
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
