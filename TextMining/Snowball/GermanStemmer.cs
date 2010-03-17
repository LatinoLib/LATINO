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
    public class GermanStemmer : SnowballProgram, ISnowballStemmer
	{
		public GermanStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 6, "", this), new Among("U", 0, 2, "", this), new Among("Y", 0, 1, "", this), new Among("\u00E4", 0, 3, "", this), new Among("\u00F6", 0, 4, "", this), new Among("\u00FC", 0, 5, "", this)};
			a1 = new Among[]{new Among("e", - 1, 1, "", this), new Among("em", - 1, 1, "", this), new Among("en", - 1, 1, "", this), new Among("ern", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("s", - 1, 2, "", this), new Among("es", 5, 1, "", this)};
			a2 = new Among[]{new Among("en", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("st", - 1, 2, "", this), new Among("est", 2, 1, "", this)};
			a3 = new Among[]{new Among("ig", - 1, 1, "", this), new Among("lich", - 1, 1, "", this)};
			a4 = new Among[]{new Among("end", - 1, 1, "", this), new Among("ig", - 1, 2, "", this), new Among("ung", - 1, 1, "", this), new Among("lich", - 1, 3, "", this), new Among("isch", - 1, 2, "", this), new Among("ik", - 1, 2, "", this), new Among("heit", - 1, 3, "", this), new Among("keit", - 1, 4, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (8), (char) (0), (char) (32), (char) (8)};
		private static readonly char[] gSEnding = new char[]{(char) (117), (char) (30), (char) (5)};
		private static readonly char[] gStEnding = new char[]{(char) (117), (char) (30), (char) (4)};
		
		private int I_p2;
		private int I_p1;
		
		protected internal virtual void  copyFrom(GermanStemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			base.copyFrom(other);
		}
		
		private bool rPrelude()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			// (, line 28
			// test, line 30
			v1 = cursor;
			// repeat, line 30
			while (true)
			{
				v2 = cursor;
				do 
				{
					// (, line 30
					// or, line 33
					do 
					{
						v3 = cursor;
						do 
						{
							// (, line 31
							// [, line 32
							bra = cursor;
							// literal, line 32
							if (!(eqS(1, "\u00DF")))
							{
								goto lab3Brk;
							}
							// ], line 32
							ket = cursor;
							// <-, line 32
							sliceFrom("ss");
							goto lab2Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = v3;
						// next, line 33
						if (cursor >= limit)
						{
							goto lab1Brk;
						}
						cursor++;
					}
					while (false);

lab2Brk: ;
					
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
			// repeat, line 36
			while (true)
			{
				v4 = cursor;
				do 
				{
					// goto, line 36
					while (true)
					{
						v5 = cursor;
						do 
						{
							// (, line 36
							if (!(inGrouping(gV, 97, 252)))
							{
								goto lab7Brk;
							}
							// [, line 37
							bra = cursor;
							// or, line 37
							do 
							{
								v6 = cursor;
								do 
								{
									// (, line 37
									// literal, line 37
									if (!(eqS(1, "u")))
									{
										goto lab9Brk;
									}
									// ], line 37
									ket = cursor;
									if (!(inGrouping(gV, 97, 252)))
									{
										goto lab9Brk;
									}
									// <-, line 37
									sliceFrom("U");
									goto lab8Brk;
								}
								while (false);

lab9Brk: ;
								
								cursor = v6;
								// (, line 38
								// literal, line 38
								if (!(eqS(1, "y")))
								{
									goto lab7Brk;
								}
								// ], line 38
								ket = cursor;
								if (!(inGrouping(gV, 97, 252)))
								{
									goto lab7Brk;
								}
								// <-, line 38
								sliceFrom("Y");
							}
							while (false);

lab8Brk: ;
							
							cursor = v5;
							goto golab6Brk;
						}
						while (false);

lab7Brk: ;
						
						cursor = v5;
						if (cursor >= limit)
						{
							goto lab5Brk;
						}
						cursor++;
					}

golab6Brk: ;
					
					goto replab4;
				}
				while (false);

lab5Brk: ;
				
				cursor = v4;
				goto replab4Brk;

replab4: ;
			}

replab4Brk: ;
			
			return true;
		}
		
		private bool rMarkRegions()
		{
			// (, line 42
			I_p1 = limit;
			I_p2 = limit;
			// gopast, line 47
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
			
			// gopast, line 47
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
			
			// setmark p1, line 47
			I_p1 = cursor;
			// try, line 48
			do 
			{
				// (, line 48
				if (!(I_p1 < 3))
				{
					goto lab4Brk;
				}
				I_p1 = 3;
			}
			while (false);

lab4Brk: ;
			
			// gopast, line 49
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
			
			// gopast, line 49
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
			
			// setmark p2, line 49
			I_p2 = cursor;
			return true;
		}
		
		private bool rPostlude()
		{
			int amongVar;
			int v1;
			// repeat, line 53
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 53
					// [, line 55
					bra = cursor;
					// substring, line 55
					amongVar = findAmong(a0, 6);
					if (amongVar == 0)
					{
						goto lab10Brk;
					}
					// ], line 55
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab10Brk;
						
						case 1: 
							// (, line 56
							// <-, line 56
							sliceFrom("y");
							break;
						
						case 2: 
							// (, line 57
							// <-, line 57
							sliceFrom("u");
							break;
						
						case 3: 
							// (, line 58
							// <-, line 58
							sliceFrom("a");
							break;
						
						case 4: 
							// (, line 59
							// <-, line 59
							sliceFrom("o");
							break;
						
						case 5: 
							// (, line 60
							// <-, line 60
							sliceFrom("u");
							break;
						
						case 6: 
							// (, line 61
							// next, line 61
							if (cursor >= limit)
							{
								goto lab10Brk;
							}
							cursor++;
							break;
						}
					goto replab1;
				}
				while (false);

lab10Brk: ;
				
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
			// (, line 71
			// do, line 72
			v1 = limit - cursor;
			do 
			{
				// (, line 72
				// [, line 73
				ket = cursor;
				// substring, line 73
				amongVar = findAmongB(a1, 7);
				if (amongVar == 0)
				{
					goto lab0Brk;
				}
				// ], line 73
				bra = cursor;
				// call R1, line 73
				if (!r_R1())
				{
					goto lab0Brk;
				}
				switch (amongVar)
				{
					
					case 0: 
						goto lab0Brk;
					
					case 1: 
						// (, line 75
						// delete, line 75
						sliceDel();
						break;
					
					case 2: 
						// (, line 78
						if (!(inGroupingB(gSEnding, 98, 116)))
						{
							goto lab0Brk;
						}
						// delete, line 78
						sliceDel();
						break;
					}
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v1;
			// do, line 82
			v2 = limit - cursor;
			do 
			{
				// (, line 82
				// [, line 83
				ket = cursor;
				// substring, line 83
				amongVar = findAmongB(a2, 4);
				if (amongVar == 0)
				{
					goto lab1Brk;
				}
				// ], line 83
				bra = cursor;
				// call R1, line 83
				if (!r_R1())
				{
					goto lab1Brk;
				}
				switch (amongVar)
				{
					
					case 0: 
						goto lab1Brk;
					
					case 1: 
						// (, line 85
						// delete, line 85
						sliceDel();
						break;
					
					case 2: 
						// (, line 88
						if (!(inGroupingB(gStEnding, 98, 116)))
						{
							goto lab1Brk;
						}
						// hop, line 88
						{
							int c = cursor - 3;
							if (limitBackward > c || c > limit)
							{
								goto lab1Brk;
							}
							cursor = c;
						}
						// delete, line 88
						sliceDel();
						break;
					}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 92
			v3 = limit - cursor;
			do 
			{
				// (, line 92
				// [, line 93
				ket = cursor;
				// substring, line 93
				amongVar = findAmongB(a4, 8);
				if (amongVar == 0)
				{
					goto lab2Brk;
				}
				// ], line 93
				bra = cursor;
				// call R2, line 93
				if (!r_R2())
				{
					goto lab2Brk;
				}
				switch (amongVar)
				{
					
					case 0: 

                        goto lab2Brk;
					
					case 1: 
						// (, line 95
						// delete, line 95
						sliceDel();
						// try, line 96
						v4 = limit - cursor;
						do 
						{
							// (, line 96
							// [, line 96
							ket = cursor;
							// literal, line 96
							if (!(eqSB(2, "ig")))
							{
								cursor = limit - v4;
								goto lab3Brk;
							}
							// ], line 96
							bra = cursor;
							// not, line 96
							{
								v5 = limit - cursor;
								do 
								{
									// literal, line 96
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
							// call R2, line 96
							if (!r_R2())
							{
								cursor = limit - v4;
								goto lab3Brk;
							}
							// delete, line 96
							sliceDel();
						}
						while (false);

lab3Brk: ;
						
						break;
					
					case 2: 
						// (, line 99
						// not, line 99
						{
							v6 = limit - cursor;
							do 
							{
								// literal, line 99
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
						// delete, line 99
						sliceDel();
						break;
					
					case 3: 
						// (, line 102
						// delete, line 102
						sliceDel();
						// try, line 103
						v7 = limit - cursor;
						do 
						{
							// (, line 103
							// [, line 104
							ket = cursor;
							// or, line 104
							do 
							{
								v8 = limit - cursor;
								do 
								{
									// literal, line 104
									if (!(eqSB(2, "er")))
									{
										goto lab8Brk;
									}
									goto lab7Brk;
								}
								while (false);

lab8Brk: ;
								
								cursor = limit - v8;
								// literal, line 104
								if (!(eqSB(2, "en")))
								{
									cursor = limit - v7;
									goto lab6Brk;
								}
							}
							while (false);

lab7Brk: ;
							
							// ], line 104
							bra = cursor;
							// call R1, line 104
							if (!r_R1())
							{
								cursor = limit - v7;
								goto lab6Brk;
							}
							// delete, line 104
							sliceDel();
						}
						while (false);

lab6Brk: ;
						
						break;
					
					case 4: 
						// (, line 108
						// delete, line 108
						sliceDel();
						// try, line 109
						v9 = limit - cursor;
						do 
						{
							// (, line 109
							// [, line 110
							ket = cursor;
							// substring, line 110
							amongVar = findAmongB(a3, 2);
							if (amongVar == 0)
							{
								cursor = limit - v9;
								goto lab9Brk;
							}
							// ], line 110
							bra = cursor;
							// call R2, line 110
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
									// (, line 112
									// delete, line 112
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
			// (, line 122
			// do, line 123
			v1 = cursor;
			do 
			{
				// call prelude, line 123
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 124
			v2 = cursor;
			do 
			{
				// call markRegions, line 124
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 125
			limitBackward = cursor; cursor = limit;
			// do, line 126
			v3 = limit - cursor;
			do 
			{
				// call standardSuffix, line 126
				if (!rStandardSuffix())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			cursor = limitBackward; // do, line 127
			v4 = cursor;
			do 
			{
				// call postlude, line 127
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
