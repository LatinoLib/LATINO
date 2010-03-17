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
    public class SpanishStemmer : SnowballProgram, ISnowballStemmer
	{
		public SpanishStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 6, "", this), new Among("\u00E1", 0, 1, "", this), new Among("\u00E9", 0, 2, "", this), new Among("\u00ED", 0, 3, "", this), new Among("\u00F3", 0, 4, "", this), new Among("\u00FA", 0, 5, "", this)};
			a1 = new Among[]{new Among("la", - 1, - 1, "", this), new Among("sela", 0, - 1, "", this), new Among("le", - 1, - 1, "", this), new Among("me", - 1, - 1, "", this), new Among("se", - 1, - 1, "", this), new Among("lo", - 1, - 1, "", this), new Among("selo", 5, - 1, "", this), new Among("las", - 1, - 1, "", this), new Among("selas", 7, - 1, "", this), new Among("les", - 1, - 1, "", this), new Among("los", - 1, - 1, "", this), new Among("selos", 10, - 1, "", this), new Among("nos", - 1, - 1, "", this)};
			a2 = new Among[]{new Among("ando", - 1, 6, "", this), new Among("iendo", - 1, 6, "", this), new Among("yendo", - 1, 7, "", this), new Among("\u00E1ndo", - 1, 2, "", this), new Among("i\u00E9ndo", - 1, 1, "", this), new Among("ar", - 1, 6, "", this), new Among("er", - 1, 6, "", this), new Among("ir", - 1, 6, "", this), new Among("\u00E1r", - 1, 3, "", this), new Among("\u00E9r", - 1, 4, "", this), new Among("\u00EDr", - 1, 5, "", this)};
			a3 = new Among[]{new Among("ic", - 1, - 1, "", this), new Among("ad", - 1, - 1, "", this), new Among("os", - 1, - 1, "", this), new Among("iv", - 1, 1, "", this)};
			a4 = new Among[]{new Among("able", - 1, 1, "", this), new Among("ible", - 1, 1, "", this)};
			a5 = new Among[]{new Among("ic", - 1, 1, "", this), new Among("abil", - 1, 1, "", this), new Among("iv", - 1, 1, "", this)};
			a6 = new Among[]{new Among("ica", - 1, 1, "", this), new Among("encia", - 1, 5, "", this), new Among("adora", - 1, 2, "", this), new Among("osa", - 1, 1, "", this), new Among("ista", - 1, 1, "", this), new Among("iva", - 1, 9, "", this), new Among("anza", - 1, 1, "", this), new Among("log\u00EDa", - 1, 3, "", this), new Among("idad", - 1, 8, "", this), new Among("able", - 1, 1, "", this), new Among("ible", - 1, 1, "", this), new Among("mente", - 1, 7, "", this), new Among("amente", 11, 6, "", this), new Among("aci\u00F3n", - 1, 2, "", this), new Among("uci\u00F3n", - 1, 4, "", this), new Among("ico", - 1, 1, "", this), new Among("ismo", - 1, 1, "", this), new Among("oso", - 1, 1, "", this), new Among("amiento", - 1, 1, "", this), new Among("imiento", - 1, 1, "", this), new Among("ivo", - 1, 9, "", this), new Among("ador", - 1, 2, "", this), new Among("icas", - 1, 1, "", this), new Among("encias", - 1, 5, "", this), new Among("adoras", - 1, 2, "", this), new Among("osas", - 1, 1, "", this), new Among("istas", - 1, 1, "", this), new Among("ivas", - 1, 9, "", this), new Among("anzas", - 1, 1, "", this), new Among("log\u00EDas", - 1, 3, "", this), new Among("idades", - 1, 8, "", this), new Among("ables", - 1, 1, "", this), new Among("ibles", - 1, 1, "", this), new Among("aciones", - 1, 2, "", this), new Among("uciones", - 1, 4, "", this), new Among("adores", - 1, 2, "", this), new Among("icos", - 1, 1, "", this), new Among("ismos", - 1, 1, "", this), new Among("osos", - 1, 1, "", this), new Among("amientos", - 1, 1, "", this), new Among("imientos", - 1, 1, "", this), new Among("ivos", - 1, 9, "", this)};
			a7 = new Among[]{new Among("ya", - 1, 1, "", this), new Among("ye", - 1, 1, "", this), new Among("yan", - 1, 1, "", this), new Among("yen", - 1, 1, "", this), new Among("yeron", - 1, 1, "", this), new Among("yendo", - 1, 1, "", this), new Among("yo", - 1, 1, "", this), new Among("yas", - 1, 1, "", this), new Among("yes", - 1, 1, "", this), new Among("yais", - 1, 1, "", this), new Among("yamos", - 1, 1, "", this), new Among("y\u00F3", - 1, 1, "", this)};
			a8 = new Among[]{new Among("aba", - 1, 2, "", this), new Among("ada", - 1, 2, "", this), new Among("ida", - 1, 2, "", this), new Among("ara", - 1, 2, "", this), new Among("iera", - 1, 2, "", this), new Among("\u00EDa", - 1, 2, "", this), new Among("ar\u00EDa", 5, 2, "", this), new Among("er\u00EDa", 5, 2, "", this), new Among("ir\u00EDa", 5, 2, "", this), new Among("ad", - 1, 2, "", this), new Among("ed", - 1, 2, "", this), new Among("id", - 1, 2, "", this), new Among("ase", - 1, 2, "", this), new Among("iese", - 1, 2, "", this), new Among("aste", - 1, 2, "", this), new Among("iste", - 1, 2, "", this), new Among("an", - 1, 2, "", this), new Among("aban", 16, 2, "", this), new Among("aran", 16, 2, "", this), new Among("ieran", 16, 2, "", this), new Among("\u00EDan", 16, 2, "", this), new Among("ar\u00EDan", 20, 2, "", this), new Among("er\u00EDan", 20, 2, "", this), new Among("ir\u00EDan", 20, 2, "", this), new Among("en", - 1, 1, "", this), new Among("asen", 24, 2, "", this), new Among("iesen", 24, 2, "", this), new Among("aron", - 1, 2, "", this), new Among("ieron", - 1, 2, "", this), new Among("ar\u00E1n", - 1, 2, "", this), new Among("er\u00E1n", - 1, 2, "", this), new Among("ir\u00E1n", - 1, 2, "", this), new Among("ado", - 1, 2, "", this), new Among("ido", - 1, 2, "", this), new Among("ando", - 1, 2, "", this), new Among("iendo", - 1, 2, "", this), new Among("ar", - 1, 2, "", this), new Among("er", - 1, 2, "", this), new Among("ir", - 1, 2, "", this), new Among("as", - 1, 2, "", this), new Among("abas", 39, 2, "", this), new Among("adas", 39, 2, "", this), new Among("idas", 39, 2, "", this), new Among("aras", 39, 2, "", this), new Among("ieras", 39, 2, "", this), new Among("\u00EDas", 39, 2, "", this), new Among("ar\u00EDas", 45, 2, "", this), new Among("er\u00EDas", 45, 2, "", this), new Among("ir\u00EDas", 45, 2, "", this), new Among("es", - 1, 1, "", this), new Among("ases", 49, 2, "", this), new Among("ieses", 49, 2, "", this), new Among("abais", - 1, 2, "", this), new Among("arais", - 
				1, 2, "", this), new Among("ierais", - 1, 2, "", this), new Among("\u00EDais", - 1, 2, "", this), new Among("ar\u00EDais", 55, 2, "", this), new Among("er\u00EDais", 55, 2, "", this), new Among("ir\u00EDais", 55, 2, "", this), new Among("aseis", - 1, 2, "", this), new Among("ieseis", - 1, 2, "", this), new Among("asteis", - 1, 2, "", this), new Among("isteis", - 1, 2, "", this), new Among("\u00E1is", - 1, 2, "", this), new Among("\u00E9is", - 1, 1, "", this), new Among("ar\u00E9is", 64, 2, "", this), new Among("er\u00E9is", 64, 2, "", this), new Among("ir\u00E9is", 64, 2, "", this), new Among("ados", - 1, 2, "", this), new Among("idos", - 1, 2, "", this), new Among("amos", - 1, 2, "", this), new Among("\u00E1bamos", 70, 2, "", this), new Among("\u00E1ramos", 70, 2, "", this), new Among("i\u00E9ramos", 70, 2, "", this), new Among("\u00EDamos", 70, 2, "", this), new Among("ar\u00EDamos", 74, 2, "", this), new Among("er\u00EDamos", 74, 2, "", this), new Among("ir\u00EDamos", 74, 2, "", this), new Among("emos", - 1, 1, "", this), new Among("aremos", 78, 2, "", this), new Among("eremos", 78, 2, "", this), new Among("iremos", 78, 2, "", this), new Among("\u00E1semos", 78, 2, "", this), new Among("i\u00E9semos", 78, 2, "", this), new Among("imos", - 1, 2, "", this), new Among("ar\u00E1s", - 1, 2, "", this), new Among("er\u00E1s", - 1, 2, "", this), new Among("ir\u00E1s", - 1, 2, "", this), new Among("\u00EDs", - 1, 2, "", this), new Among("ar\u00E1", - 1, 2, "", this), new Among("er\u00E1", - 1, 2, "", this), new Among("ir\u00E1", - 1, 2, "", this), new Among("ar\u00E9", - 1, 2, "", this), new Among("er\u00E9", - 1, 2, "", this), new Among("ir\u00E9", - 1, 2, "", this), new Among("i\u00F3", - 1, 2, "", this)};
			a9 = new Among[]{new Among("a", - 1, 1, "", this), new Among("e", - 1, 2, "", this), new Among("o", - 1, 1, "", this), new Among("os", - 1, 1, "", this), new Among("\u00E1", - 1, 1, "", this), new Among("\u00E9", - 1, 2, "", this), new Among("\u00ED", - 1, 1, "", this), new Among("\u00F3", - 1, 1, "", this)};
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
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (1), (char) (17), (char) (4), (char) (10)};
		
		private int I_p2;
		private int I_p1;
		private int I_pV;
		
		protected internal virtual void  copyFrom(SpanishStemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			I_pV = other.I_pV;
			base.copyFrom(other);
		}
		
		private bool rMarkRegions()
		{
			int v1;
			int v2;
			int v3;
			int v6;
			int v8;
			// (, line 31
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			// do, line 37
			v1 = cursor;
			do 
			{
				// (, line 37
				// or, line 39
				do 
				{
					v2 = cursor;
					do 
					{
						// (, line 38
						if (!(inGrouping(gV, 97, 252)))
						{
							goto lab2Brk;
						}
						// or, line 38
						do 
						{
							v3 = cursor;
							do 
							{
								// (, line 38
								if (!(outGrouping(gV, 97, 252)))
								{
									goto lab4Brk;
								}
								// gopast, line 38
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
										goto lab4Brk;
									}
									cursor++;
								}

golab5Brk: ;
								
								goto lab3Brk;
							}
							while (false);

lab4Brk: ;
							
							cursor = v3;
							// (, line 38
							if (!(inGrouping(gV, 97, 252)))
							{
								goto lab2Brk;
							}
							// gopast, line 38
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
									goto lab2Brk;
								}
								cursor++;
							}

golab7Brk: ;
							
						}
						while (false);

lab3Brk: ;
						
						goto lab1Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = v2;
					// (, line 40
					if (!(outGrouping(gV, 97, 252)))
					{
						goto lab0Brk;
					}
					// or, line 40
					do 
					{
						v6 = cursor;
						do 
						{
							// (, line 40
							if (!(outGrouping(gV, 97, 252)))
							{
								goto lab10Brk;
							}
							// gopast, line 40
							while (true)
							{
								do 
								{
									if (!(inGrouping(gV, 97, 252)))
									{
										goto lab12Brk;
									}
									goto golab11Brk;
								}
								while (false);

lab12Brk: ;
								
								if (cursor >= limit)
								{
									goto lab10Brk;
								}
								cursor++;
							}

golab11Brk: ;
							
							goto lab9Brk;
						}
						while (false);

lab10Brk: ;
						
						cursor = v6;
						// (, line 40
						if (!(inGrouping(gV, 97, 252)))
						{
							goto lab0Brk;
						}
						// next, line 40
						if (cursor >= limit)
						{
							goto lab0Brk;
						}
						cursor++;
					}
					while (false);

lab9Brk: ;
					
				}
				while (false);

lab1Brk: ;
				
				// setmark pV, line 41
				I_pV = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 43
			v8 = cursor;
			do 
			{
				// (, line 43
				// gopast, line 44
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 252)))
						{
							goto lab15Brk;
						}
						goto golab14Brk;
					}
					while (false);

lab15Brk: ;
					
					if (cursor >= limit)
					{
						goto lab13Brk;
					}
					cursor++;
				}

golab14Brk: ;
				
				// gopast, line 44
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 252)))
						{
							goto lab17Brk;
						}
						goto golab16Brk;
					}
					while (false);

lab17Brk: ;
					
					if (cursor >= limit)
					{
						goto lab13Brk;
					}
					cursor++;
				}

golab16Brk: ;
				
				// setmark p1, line 44
				I_p1 = cursor;
				// gopast, line 45
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 252)))
						{
							goto lab19Brk;
						}
						goto golab18Brk;
					}
					while (false);

lab19Brk: ;
					
					if (cursor >= limit)
					{
						goto lab13Brk;
					}
					cursor++;
				}

golab18Brk: ;
				
				// gopast, line 45
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 252)))
						{
							goto lab21Brk;
						}
						goto golab20Brk;
					}
					while (false);

lab21Brk: ;
					
					if (cursor >= limit)
					{
						goto lab13Brk;
					}
					cursor++;
				}

golab20Brk: ;
				
				// setmark p2, line 45
				I_p2 = cursor;
			}
			while (false);

lab13Brk: ;
			
			cursor = v8;
			return true;
		}
		
		private bool rPostlude()
		{
			int amongVar;
			int v1;
			// repeat, line 49
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 49
					// [, line 50
					bra = cursor;
					// substring, line 50
					amongVar = findAmong(a0, 6);
					if (amongVar == 0)
					{
						goto lab5Brk;
					}
					// ], line 50
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab5Brk;
						
						case 1: 
							// (, line 51
							// <-, line 51
							sliceFrom("a");
							break;
						
						case 2: 
							// (, line 52
							// <-, line 52
							sliceFrom("e");
							break;
						
						case 3: 
							// (, line 53
							// <-, line 53
							sliceFrom("i");
							break;
						
						case 4: 
							// (, line 54
							// <-, line 54
							sliceFrom("o");
							break;
						
						case 5: 
							// (, line 55
							// <-, line 55
							sliceFrom("u");
							break;
						
						case 6: 
							// (, line 57
							// next, line 57
							if (cursor >= limit)
							{
								goto lab5Brk;
							}
							cursor++;
							break;
						}
					goto replab0;
				}
				while (false);

lab5Brk: ;
				
				cursor = v1;
				goto replab0Brk;

replab0: ;
			}

replab0Brk: ;
			
			return true;
		}
		
		private bool r_RV()
		{
			if (!(I_pV <= cursor))
			{
				return false;
			}
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
		
		private bool rAttachedPronoun()
		{
			int amongVar;
			// (, line 67
			// [, line 68
			ket = cursor;
			// substring, line 68
			if (findAmongB(a1, 13) == 0)
			{
				return false;
			}
			// ], line 68
			bra = cursor;
			// substring, line 72
			amongVar = findAmongB(a2, 11);
			if (amongVar == 0)
			{
				return false;
			}
			// call RV, line 72
			if (!r_RV())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 73
					// ], line 73
					bra = cursor;
					// <-, line 73
					sliceFrom("iendo");
					break;
				
				case 2: 
					// (, line 74
					// ], line 74
					bra = cursor;
					// <-, line 74
					sliceFrom("ando");
					break;
				
				case 3: 
					// (, line 75
					// ], line 75
					bra = cursor;
					// <-, line 75
					sliceFrom("ar");
					break;
				
				case 4: 
					// (, line 76
					// ], line 76
					bra = cursor;
					// <-, line 76
					sliceFrom("er");
					break;
				
				case 5: 
					// (, line 77
					// ], line 77
					bra = cursor;
					// <-, line 77
					sliceFrom("ir");
					break;
				
				case 6: 
					// (, line 81
					// delete, line 81
					sliceDel();
					break;
				
				case 7: 
					// (, line 82
					// literal, line 82
					if (!(eqSB(1, "u")))
					{
						return false;
					}
					// delete, line 82
					sliceDel();
					break;
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
			// (, line 86
			// [, line 87
			ket = cursor;
			// substring, line 87
			amongVar = findAmongB(a6, 42);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 87
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 98
					// call R2, line 99
					if (!r_R2())
					{
						return false;
					}
					// delete, line 99
					sliceDel();
					break;
				
				case 2: 
					// (, line 103
					// call R2, line 104
					if (!r_R2())
					{
						return false;
					}
					// delete, line 104
					sliceDel();
					// try, line 105
					v1 = limit - cursor;
					do 
					{
						// (, line 105
						// [, line 105
						ket = cursor;
						// literal, line 105
						if (!(eqSB(2, "ic")))
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// ], line 105
						bra = cursor;
						// call R2, line 105
						if (!r_R2())
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// delete, line 105
						sliceDel();
					}
					while (false);

lab0Brk: ;
					
					break;
				
				case 3: 
					// (, line 109
					// call R2, line 110
					if (!r_R2())
					{
						return false;
					}
					// <-, line 110
					sliceFrom("log");
					break;
				
				case 4: 
					// (, line 113
					// call R2, line 114
					if (!r_R2())
					{
						return false;
					}
					// <-, line 114
					sliceFrom("u");
					break;
				
				case 5: 
					// (, line 117
					// call R2, line 118
					if (!r_R2())
					{
						return false;
					}
					// <-, line 118
					sliceFrom("ente");
					break;
				
				case 6: 
					// (, line 121
					// call R1, line 122
					if (!r_R1())
					{
						return false;
					}
					// delete, line 122
					sliceDel();
					// try, line 123
					v2 = limit - cursor;
					do 
					{
						// (, line 123
						// [, line 124
						ket = cursor;
						// substring, line 124
						amongVar = findAmongB(a3, 4);
						if (amongVar == 0)
						{
							cursor = limit - v2;
							goto lab1Brk;
						}
						// ], line 124
						bra = cursor;
						// call R2, line 124
						if (!r_R2())
						{
							cursor = limit - v2;
							goto lab1Brk;
						}
						// delete, line 124
						sliceDel();
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v2;
								goto lab1Brk;
							
							case 1: 
								// (, line 125
								// [, line 125
								ket = cursor;
								// literal, line 125
								if (!(eqSB(2, "at")))
								{
									cursor = limit - v2;
									goto lab1Brk;
								}
								// ], line 125
								bra = cursor;
								// call R2, line 125
								if (!r_R2())
								{
									cursor = limit - v2;
									goto lab1Brk;
								}
								// delete, line 125
								sliceDel();
								break;
							}
					}
					while (false);

lab1Brk: ;
					
					break;
				
				case 7: 
					// (, line 133
					// call R2, line 134
					if (!r_R2())
					{
						return false;
					}
					// delete, line 134
					sliceDel();
					// try, line 135
					v3 = limit - cursor;
					do 
					{
						// (, line 135
						// [, line 136
						ket = cursor;
						// substring, line 136
						amongVar = findAmongB(a4, 2);
						if (amongVar == 0)
						{
							cursor = limit - v3;
							goto lab2Brk;
						}
						// ], line 136
						bra = cursor;
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v3;
								goto lab2Brk;
							
							case 1: 
								// (, line 138
								// call R2, line 138
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab2Brk;
								}
								// delete, line 138
								sliceDel();
								break;
							}
					}
					while (false);

lab2Brk: ;
					
					break;
				
				case 8: 
					// (, line 144
					// call R2, line 145
					if (!r_R2())
					{
						return false;
					}
					// delete, line 145
					sliceDel();
					// try, line 146
					v4 = limit - cursor;
					do 
					{
						// (, line 146
						// [, line 147
						ket = cursor;
						// substring, line 147
						amongVar = findAmongB(a5, 3);
						if (amongVar == 0)
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// ], line 147
						bra = cursor;
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v4;
								goto lab3Brk;
							
							case 1: 
								// (, line 150
								// call R2, line 150
								if (!r_R2())
								{
									cursor = limit - v4;
									goto lab3Brk;
								}
								// delete, line 150
								sliceDel();
								break;
							}
					}
					while (false);

lab3Brk: ;
					
					break;
				
				case 9: 
					// (, line 156
					// call R2, line 157
					if (!r_R2())
					{
						return false;
					}
					// delete, line 157
					sliceDel();
					// try, line 158
					v5 = limit - cursor;
					do 
					{
						// (, line 158
						// [, line 159
						ket = cursor;
						// literal, line 159
						if (!(eqSB(2, "at")))
						{
							cursor = limit - v5;
							goto lab4Brk;
						}
						// ], line 159
						bra = cursor;
						// call R2, line 159
						if (!r_R2())
						{
							cursor = limit - v5;
							goto lab4Brk;
						}
						// delete, line 159
						sliceDel();
					}
					while (false);

lab4Brk: ;
					
					break;
				}
			return true;
		}
		
		private bool rYVerbSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 165
			// setlimit, line 166
			v1 = limit - cursor;
			// tomark, line 166
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 166
			// [, line 166
			ket = cursor;
			// substring, line 166
			amongVar = findAmongB(a7, 12);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 166
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 169
					// literal, line 169
					if (!(eqSB(1, "u")))
					{
						return false;
					}
					// delete, line 169
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rVerbSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 173
			// setlimit, line 174
			v1 = limit - cursor;
			// tomark, line 174
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 174
			// [, line 174
			ket = cursor;
			// substring, line 174
			amongVar = findAmongB(a8, 96);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 174
			bra = cursor;
			limitBackward = v2;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 177
					// try, line 177
					v3 = limit - cursor;
					do 
					{
						// (, line 177
						// literal, line 177
						if (!(eqSB(1, "u")))
						{
							cursor = limit - v3;
							goto lab5Brk;
						}
						// test, line 177
						v4 = limit - cursor;
						// literal, line 177
						if (!(eqSB(1, "g")))
						{
							cursor = limit - v3;
							goto lab5Brk;
						}
						cursor = limit - v4;
					}
					while (false);

lab5Brk: ;
					
					// ], line 177
					bra = cursor;
					// delete, line 177
					sliceDel();
					break;
				
				case 2: 
					// (, line 198
					// delete, line 198
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rResidualSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 202
			// [, line 203
			ket = cursor;
			// substring, line 203
			amongVar = findAmongB(a9, 8);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 203
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 206
					// call RV, line 206
					if (!r_RV())
					{
						return false;
					}
					// delete, line 206
					sliceDel();
					break;
				
				case 2: 
					// (, line 208
					// call RV, line 208
					if (!r_RV())
					{
						return false;
					}
					// delete, line 208
					sliceDel();
					// try, line 208
					v1 = limit - cursor;
					do 
					{
						// (, line 208
						// [, line 208
						ket = cursor;
						// literal, line 208
						if (!(eqSB(1, "u")))
						{
							cursor = limit - v1;
						goto lab5Brk;
						}
						// ], line 208
						bra = cursor;
						// test, line 208
						v2 = limit - cursor;
						// literal, line 208
						if (!(eqSB(1, "g")))
						{
							cursor = limit - v1;
							goto lab5Brk;
						}
						cursor = limit - v2;
						// call RV, line 208
						if (!r_RV())
						{
							cursor = limit - v1;
							goto lab5Brk;
						}
						// delete, line 208
						sliceDel();
					}
					while (false);

lab5Brk: ;
					
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
			int v5;
			int v6;
			// (, line 213
			// do, line 214
			v1 = cursor;
			do 
			{
				// call markRegions, line 214
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// backwards, line 215
			limitBackward = cursor; cursor = limit;
			// (, line 215
			// do, line 216
			v2 = limit - cursor;
			do 
			{
				// call attachedPronoun, line 216
				if (!rAttachedPronoun())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 217
			v3 = limit - cursor;
			do 
			{
				// (, line 217
				// or, line 217
				do 
				{
					v4 = limit - cursor;
					do 
					{
						// call standardSuffix, line 217
						if (!rStandardSuffix())
						{
							goto lab4Brk;
						}
						goto lab3Brk;
					}
					while (false);

lab4Brk: ;
					
					cursor = limit - v4;
					do 
					{
						// call yVerbSuffix, line 218
						if (!rYVerbSuffix())
						{
							goto lab5Brk;
						}
						goto lab3Brk;
					}
					while (false);

lab5Brk: ;
					
					cursor = limit - v4;
					// call verbSuffix, line 219
					if (!rVerbSuffix())
					{
						goto lab2Brk;
					}
				}
				while (false);

lab3Brk: ;
				
			}
			while (false);

lab2Brk: ;

			cursor = limit - v3;
			// do, line 221
			v5 = limit - cursor;
			do 
			{
				// call residualSuffix, line 221
				if (!rResidualSuffix())
				{
					goto lab6Brk;
				}
			}
			while (false);

lab6Brk: ;
			
			cursor = limit - v5;
			cursor = limitBackward; // do, line 223
			v6 = cursor;
			do 
			{
				// call postlude, line 223
				if (!rPostlude())
				{
					goto lab7Brk;
				}
			}
			while (false);

lab7Brk: ;
			
			cursor = v6;
			return true;
		}
	}
}
