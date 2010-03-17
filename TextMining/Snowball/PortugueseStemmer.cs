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
    public class PortugueseStemmer : SnowballProgram, ISnowballStemmer
	{
		public PortugueseStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 3, "", this), new Among("\u00E3", 0, 1, "", this), new Among("\u00F5", 0, 2, "", this)};
			a1 = new Among[]{new Among("", - 1, 3, "", this), new Among("a~", 0, 1, "", this), new Among("o~", 0, 2, "", this)};
			a2 = new Among[]{new Among("ic", - 1, - 1, "", this), new Among("ad", - 1, - 1, "", this), new Among("os", - 1, - 1, "", this), new Among("iv", - 1, 1, "", this)};
			a3 = new Among[]{new Among("avel", - 1, 1, "", this), new Among("\u00EDvel", - 1, 1, "", this)};
			a4 = new Among[]{new Among("ic", - 1, 1, "", this), new Among("abil", - 1, 1, "", this), new Among("iv", - 1, 1, "", this)};
			a5 = new Among[]{new Among("ica", - 1, 1, "", this), new Among("\u00EAncia", - 1, 4, "", this), new Among("ira", - 1, 9, "", this), new Among("adora", - 1, 1, "", this), new Among("osa", - 1, 1, "", this), new Among("ista", - 1, 1, "", this), new Among("iva", - 1, 8, "", this), new Among("eza", - 1, 1, "", this), new Among("log\u00EDa", - 1, 2, "", this), new Among("idade", - 1, 7, "", this), new Among("mente", - 1, 6, "", this), new Among("amente", 10, 5, "", this), new Among("\u00E1vel", - 1, 1, "", this), new Among("\u00EDvel", - 1, 1, "", this), new Among("uci\u00F3n", - 1, 3, "", this), new Among("ico", - 1, 1, "", this), new Among("ismo", - 1, 1, "", this), new Among("oso", - 1, 1, "", this), new Among("amento", - 1, 1, "", this), new Among("imento", - 1, 1, "", this), new Among("ivo", - 1, 8, "", this), new Among("a\u00E7a~o", - 1, 1, "", this), new Among("ador", - 1, 1, "", this), new Among("icas", - 1, 1, "", this), new Among("\u00EAncias", - 1, 4, "", this), new Among("iras", - 1, 9, "", this), new Among("adoras", - 1, 1, "", this), new Among("osas", - 1, 1, "", this), new Among("istas", - 1, 1, "", this), new Among("ivas", - 1, 8, "", this), new Among("ezas", - 1, 1, "", this), new Among("log\u00EDas", - 1, 2, "", this), new Among("idades", - 1, 7, "", this), new Among("uciones", - 1, 3, "", this), new Among("adores", - 1, 1, "", this), new Among("a\u00E7o~es", - 1, 1, "", this), new Among("icos", - 1, 1, "", this), new Among("ismos", - 1, 1, "", this), new Among("osos", - 1, 1, "", this), new Among("amentos", - 1, 1, "", this), new Among("imentos", - 1, 1, "", this), new Among("ivos", - 1, 8, "", this)};
			a6 = new Among[]{new Among("ada", - 1, 1, "", this), new Among("ida", - 1, 1, "", this), new Among("ia", - 1, 1, "", this), new Among("aria", 2, 1, "", this), new Among("eria", 2, 1, "", this), new Among("iria", 2, 1, "", this), new Among("ara", - 1, 1, "", this), new Among("era", - 1, 1, "", this), new Among("ira", - 1, 1, "", this), new Among("ava", - 1, 1, "", this), new Among("asse", - 1, 1, "", this), new Among("esse", - 1, 1, "", this), new Among("isse", - 1, 1, "", this), new Among("aste", - 1, 1, "", this), new Among("este", - 1, 1, "", this), new Among("iste", - 1, 1, "", this), new Among("ei", - 1, 1, "", this), new Among("arei", 16, 1, "", this), new Among("erei", 16, 1, "", this), new Among("irei", 16, 1, "", this), new Among("am", - 1, 1, "", this), new Among("iam", 20, 1, "", this), new Among("ariam", 21, 1, "", this), new Among("eriam", 21, 1, "", this), new Among("iriam", 21, 1, "", this), new Among("aram", 20, 1, "", this), new Among("eram", 20, 1, "", this), new Among("iram", 20, 1, "", this), new Among("avam", 20, 1, "", this), new Among("em", - 1, 1, "", this), new Among("arem", 29, 1, "", this), new Among("erem", 29, 1, "", this), new Among("irem", 29, 1, "", this), new Among("assem", 29, 1, "", this), new Among("essem", 29, 1, "", this), new Among("issem", 29, 1, "", this), new Among("ado", - 1, 1, "", this), new Among("ido", - 1, 1, "", this), new Among("ando", - 1, 1, "", this), new Among("endo", - 1, 1, "", this), new Among("indo", - 1, 1, "", this), new Among("ara~o", - 1, 1, "", this), new Among("era~o", - 1, 1, "", this), new Among("ira~o", - 1, 1, "", this), new Among("ar", - 1, 1, "", this), new Among("er", - 1, 1, "", this), new Among("ir", - 1, 1, "", this), new Among("as", - 1, 1, "", this), new Among("adas", 47, 1, "", this), new Among("idas", 47, 1, "", this), new Among("ias", 47, 1, "", this), new Among("arias", 50, 1, "", this), new Among("erias", 50, 1, "", this), new Among("irias", 50, 1, "", this), new Among("aras", 47, 1, "", this), new Among("eras", 47, 
				1, "", this), new Among("iras", 47, 1, "", this), new Among("avas", 47, 1, "", this), new Among("es", - 1, 1, "", this), new Among("ardes", 58, 1, "", this), new Among("erdes", 58, 1, "", this), new Among("irdes", 58, 1, "", this), new Among("ares", 58, 1, "", this), new Among("eres", 58, 1, "", this), new Among("ires", 58, 1, "", this), new Among("asses", 58, 1, "", this), new Among("esses", 58, 1, "", this), new Among("isses", 58, 1, "", this), new Among("astes", 58, 1, "", this), new Among("estes", 58, 1, "", this), new Among("istes", 58, 1, "", this), new Among("is", - 1, 1, "", this), new Among("ais", 71, 1, "", this), new Among("eis", 71, 1, "", this), new Among("areis", 73, 1, "", this), new Among("ereis", 73, 1, "", this), new Among("ireis", 73, 1, "", this), new Among("\u00E1reis", 73, 1, "", this), new Among("\u00E9reis", 73, 1, "", this), new Among("\u00EDreis", 73, 1, "", this), new Among("\u00E1sseis", 73, 1, "", this), new Among("\u00E9sseis", 73, 1, "", this), new Among("\u00EDsseis", 73, 1, "", this), new Among("\u00E1veis", 73, 1, "", this), new Among("\u00EDeis", 73, 1, "", this), new Among("ar\u00EDeis", 84, 1, "", this), new Among("er\u00EDeis", 84, 1, "", this), new Among("ir\u00EDeis", 84, 1, "", this), new Among("ados", - 1, 1, "", this), new Among("idos", - 1, 1, "", this), new Among("amos", - 1, 1, "", this), new Among("\u00E1ramos", 90, 1, "", this), new Among("\u00E9ramos", 90, 1, "", this), new Among("\u00EDramos", 90, 1, "", this), new Among("\u00E1vamos", 90, 1, "", this), new Among("\u00EDamos", 90, 1, "", this), new Among("ar\u00EDamos", 95, 1, "", this), new Among("er\u00EDamos", 95, 1, "", this), new Among("ir\u00EDamos", 95, 1, "", this), new Among("emos", - 1, 1, "", this), new Among("aremos", 99, 1, "", this), new Among("eremos", 99, 1, "", this), new Among("iremos", 99, 1, "", this), new Among("\u00E1ssemos", 99, 1, "", this), new Among("\u00EAssemos", 99, 1, "", this), new Among("\u00EDssemos", 99, 1, "", this), new Among("imos", - 1, 1, "", this), new 
				Among("armos", - 1, 1, "", this), new Among("ermos", - 1, 1, "", this), new Among("irmos", - 1, 1, "", this), new Among("\u00E1mos", - 1, 1, "", this), new Among("ar\u00E1s", - 1, 1, "", this), new Among("er\u00E1s", - 1, 1, "", this), new Among("ir\u00E1s", - 1, 1, "", this), new Among("eu", - 1, 1, "", this), new Among("iu", - 1, 1, "", this), new Among("ou", - 1, 1, "", this), new Among("ar\u00E1", - 1, 1, "", this), new Among("er\u00E1", - 1, 1, "", this), new Among("ir\u00E1", - 1, 1, "", this)};
			a7 = new Among[]{new Among("a", - 1, 1, "", this), new Among("i", - 1, 1, "", this), new Among("o", - 1, 1, "", this), new Among("os", - 1, 1, "", this), new Among("\u00E1", - 1, 1, "", this), new Among("\u00ED", - 1, 1, "", this), new Among("\u00F3", - 1, 1, "", this)};
			a8 = new Among[]{new Among("e", - 1, 1, "", this), new Among("\u00E7", - 1, 2, "", this), new Among("\u00E9", - 1, 1, "", this), new Among("\u00EA", - 1, 1, "", this)};
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
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (3), (char) (19), (char) (12), (char) (2)};
		
		private int I_p2;
		private int I_p1;
		private int I_pV;
		
		protected internal virtual void  copyFrom(PortugueseStemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			I_pV = other.I_pV;
			base.copyFrom(other);
		}
		
		private bool rPrelude()
		{
			int amongVar;
			int v1;
			// repeat, line 36
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 36
					// [, line 37
					bra = cursor;
					// substring, line 37
					amongVar = findAmong(a0, 3);
					if (amongVar == 0)
					{
						goto lab1Brk;
					}
					// ], line 37
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab1Brk;
						
						case 1: 
							// (, line 38
							// <-, line 38
							sliceFrom("a~");
							break;
						
						case 2: 
							// (, line 39
							// <-, line 39
							sliceFrom("o~");
							break;
						
						case 3: 
							// (, line 40
							// next, line 40
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
				
				cursor = v1;
				goto replab0Brk;

replab0: ;
			}

replab0Brk: ;
			
			return true;
		}
		
		private bool rMarkRegions()
		{
			int v1;
			int v2;
			int v3;
			int v6;
			int v8;
			// (, line 44
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			// do, line 50
			v1 = cursor;
			do 
			{
				// (, line 50
				// or, line 52

lab2: 
				do 
				{
					v2 = cursor;
					do 
					{
						// (, line 51
						if (!(inGrouping(gV, 97, 250)))
						{
							goto lab2Brk;
						}
						// or, line 51
						do 
						{
							v3 = cursor;
							do 
							{
								// (, line 51
								if (!(outGrouping(gV, 97, 250)))
								{
									goto lab4Brk;
								}
								// gopast, line 51
								while (true)
								{
									do 
									{
										if (!(inGrouping(gV, 97, 250)))
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
							// (, line 51
							if (!(inGrouping(gV, 97, 250)))
							{
								goto lab2Brk;
							}
							// gopast, line 51
							while (true)
							{
								do 
								{
									if (!(outGrouping(gV, 97, 250)))
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
						
						goto lab2Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = v2;
					// (, line 53
					if (!(outGrouping(gV, 97, 250)))
					{
						goto lab0Brk;
					}
					// or, line 53
					do 
					{
						v6 = cursor;
						do 
						{
							// (, line 53
							if (!(outGrouping(gV, 97, 250)))
							{
								goto lab10Brk;
							}
							// gopast, line 53
							while (true)
							{
								do 
								{
									if (!(inGrouping(gV, 97, 250)))
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
						// (, line 53
						if (!(inGrouping(gV, 97, 250)))
						{
							goto lab0Brk;
						}
						// next, line 53
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
				// setmark pV, line 54
				I_pV = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 56
			v8 = cursor;
			do 
			{
				// (, line 56
				// gopast, line 57
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 250)))
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
				
				// gopast, line 57
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 250)))
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
				
				// setmark p1, line 57
				I_p1 = cursor;
				// gopast, line 58
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 250)))
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
				
				// gopast, line 58
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 250)))
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
				
				// setmark p2, line 58
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
			// repeat, line 62
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 62
					// [, line 63
					bra = cursor;
					// substring, line 63
					amongVar = findAmong(a1, 3);
					if (amongVar == 0)
					{
						goto lab5Brk;
					}
					// ], line 63
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab5Brk;
						
						case 1: 
							// (, line 64
							// <-, line 64
							sliceFrom("\u00E3");
							break;
						
						case 2: 
							// (, line 65
							// <-, line 65
							sliceFrom("\u00F5");
							break;
						
						case 3: 
							// (, line 66
							// next, line 66
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
		
		private bool rStandardSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 76
			// [, line 77
			ket = cursor;
			// substring, line 77
			amongVar = findAmongB(a5, 42);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 77
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 91
					// call R2, line 92
					if (!r_R2())
					{
						return false;
					}
					// delete, line 92
					sliceDel();
					break;
				
				case 2: 
					// (, line 96
					// call R2, line 97
					if (!r_R2())
					{
						return false;
					}
					// <-, line 97
					sliceFrom("log");
					break;
				
				case 3: 
					// (, line 100
					// call R2, line 101
					if (!r_R2())
					{
						return false;
					}
					// <-, line 101
					sliceFrom("u");
					break;
				
				case 4: 
					// (, line 104
					// call R2, line 105
					if (!r_R2())
					{
						return false;
					}
					// <-, line 105
					sliceFrom("ente");
					break;
				
				case 5: 
					// (, line 108
					// call R1, line 109
					if (!r_R1())
					{
						return false;
					}
					// delete, line 109
					sliceDel();
					// try, line 110
					v1 = limit - cursor;
					do 
					{
						// (, line 110
						// [, line 111
						ket = cursor;
						// substring, line 111
						amongVar = findAmongB(a2, 4);
						if (amongVar == 0)
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// ], line 111
						bra = cursor;
						// call R2, line 111
						if (!r_R2())
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// delete, line 111
						sliceDel();
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v1;
								goto lab0Brk;
							
							case 1: 
								// (, line 112
								// [, line 112
								ket = cursor;
								// literal, line 112
								if (!(eqSB(2, "at")))
								{
									cursor = limit - v1;
									goto lab0Brk;
								}
								// ], line 112
								bra = cursor;
								// call R2, line 112
								if (!r_R2())
								{
									cursor = limit - v1;
									goto lab0Brk;
								}
								// delete, line 112
								sliceDel();
								break;
							}
					}
					while (false);

lab0Brk: ;
					
					break;
				
				case 6: 
					// (, line 120
					// call R2, line 121
					if (!r_R2())
					{
						return false;
					}
					// delete, line 121
					sliceDel();
					// try, line 122
					v2 = limit - cursor;
					do 
					{
						// (, line 122
						// [, line 123
						ket = cursor;
						// substring, line 123
						amongVar = findAmongB(a3, 2);
						if (amongVar == 0)
						{
							cursor = limit - v2;
							goto lab1Brk;
						}
						// ], line 123
						bra = cursor;
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v2;
								goto lab1Brk;
							
							case 1: 
								// (, line 125
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
					// (, line 131
					// call R2, line 132
					if (!r_R2())
					{
						return false;
					}
					// delete, line 132
					sliceDel();
					// try, line 133
					v3 = limit - cursor;
					do 
					{
						// (, line 133
						// [, line 134
						ket = cursor;
						// substring, line 134
						amongVar = findAmongB(a4, 3);
						if (amongVar == 0)
						{
							cursor = limit - v3;
							goto lab2Brk;
						}
						// ], line 134
						bra = cursor;
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v3;
								goto lab2Brk;
							
							case 1: 
								// (, line 137
								// call R2, line 137
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab2Brk;
								}
								// delete, line 137
								sliceDel();
								break;
							}
					}
					while (false);

lab2Brk: ;
					
					break;
				
				case 8: 
					// (, line 143
					// call R2, line 144
					if (!r_R2())
					{
						return false;
					}
					// delete, line 144
					sliceDel();
					// try, line 145
					v4 = limit - cursor;
					do 
					{
						// (, line 145
						// [, line 146
						ket = cursor;
						// literal, line 146
						if (!(eqSB(2, "at")))
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// ], line 146
						bra = cursor;
						// call R2, line 146
						if (!r_R2())
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// delete, line 146
						sliceDel();
					}
					while (false);

lab3Brk: ;
					
					break;
				
				case 9: 
					// (, line 150
					// call RV, line 151
					if (!r_RV())
					{
						return false;
					}
					// literal, line 151
					if (!(eqSB(1, "e")))
					{
						return false;
					}
					// <-, line 152
					sliceFrom("ir");
					break;
				}
			return true;
		}
		
		private bool rVerbSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// setlimit, line 157
			v1 = limit - cursor;
			// tomark, line 157
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 157
			// [, line 158
			ket = cursor;
			// substring, line 158
			amongVar = findAmongB(a6, 120);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 158
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					limitBackward = v2;
					return false;
				
				case 1: 
					// (, line 177
					// delete, line 177
					sliceDel();
					break;
				}
			limitBackward = v2;
			return true;
		}
		
		private bool rResidualSuffix()
		{
			int amongVar;
			// (, line 181
			// [, line 182
			ket = cursor;
			// substring, line 182
			amongVar = findAmongB(a7, 7);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 182
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 185
					// call RV, line 185
					if (!r_RV())
					{
						return false;
					}
					// delete, line 185
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rResidualForm()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			// (, line 189
			// [, line 190
			ket = cursor;
			// substring, line 190
			amongVar = findAmongB(a8, 4);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 190
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 192
					// call RV, line 192
					if (!r_RV())
					{
						return false;
					}
					// delete, line 192
					sliceDel();
					// [, line 192
					ket = cursor;
					// or, line 192

lab5: 
					do 
					{
						v1 = limit - cursor;
						do 
						{
							// (, line 192
							// literal, line 192
							if (!(eqSB(1, "u")))
							{
								goto lab5Brk;
							}
							// ], line 192
							bra = cursor;
							// test, line 192
							v2 = limit - cursor;
							// literal, line 192
							if (!(eqSB(1, "g")))
							{
								goto lab5Brk;
							}
							cursor = limit - v2;
							goto lab5Brk;
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v1;
						// (, line 193
						// literal, line 193
						if (!(eqSB(1, "i")))
						{
							return false;
						}
						// ], line 193
						bra = cursor;
						// test, line 193
						v3 = limit - cursor;
						// literal, line 193
						if (!(eqSB(1, "c")))
						{
							return false;
						}
						cursor = limit - v3;
					}
					while (false);
					// call RV, line 193
					if (!r_RV())
					{
						return false;
					}
					// delete, line 193
					sliceDel();
					break;
				
				case 2: 
					// (, line 194
					// <-, line 194
					sliceFrom("c");
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
			int v7;
			int v8;
			int v9;
			int v10;
			// (, line 199
			// do, line 200
			v1 = cursor;
			do 
			{
				// call prelude, line 200
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 201
			v2 = cursor;
			do 
			{
				// call markRegions, line 201
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 202
			limitBackward = cursor; cursor = limit;
			// (, line 202
			// do, line 203
			v3 = limit - cursor;
			do 
			{
				// (, line 203
				// or, line 207
				do 
				{
					v4 = limit - cursor;
					do 
					{
						// (, line 204
						// and, line 205
						v5 = limit - cursor;
						// (, line 204
						// or, line 204
						do 
						{
							v6 = limit - cursor;
							do 
							{
								// call standardSuffix, line 204
								if (!rStandardSuffix())
								{
									goto lab6Brk;
								}
								goto lab5Brk;
							}
							while (false);

lab6Brk: ;
							
							cursor = limit - v6;
							// call verbSuffix, line 204
							if (!rVerbSuffix())
							{
								goto lab4Brk;
							}
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v5;
						// do, line 205
						v7 = limit - cursor;
						do 
						{
							// (, line 205
							// [, line 205
							ket = cursor;
							// literal, line 205
							if (!(eqSB(1, "i")))
							{
								goto lab7Brk;
							}
							// ], line 205
							bra = cursor;
							// test, line 205
							v8 = limit - cursor;
							// literal, line 205
							if (!(eqSB(1, "c")))
							{
								goto lab7Brk;
							}
							cursor = limit - v8;
							// call RV, line 205
							if (!r_RV())
							{
								goto lab7Brk;
							}
							// delete, line 205
							sliceDel();
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v7;
						goto lab5Brk;
					}
					while (false);

lab4Brk: ;
					
					cursor = limit - v4;
					// call residualSuffix, line 207
					if (!rResidualSuffix())
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
			// do, line 209
			v9 = limit - cursor;
			do 
			{
				// call residualForm, line 209
				if (!rResidualForm())
				{
					goto lab8Brk;
				}
			}
			while (false);

lab8Brk: ;
			
			cursor = limit - v9;
			cursor = limitBackward; // do, line 211
			v10 = cursor;
			do 
			{
				// call postlude, line 211
				if (!rPostlude())
				{
					goto lab9Brk;
				}
			}
			while (false);

lab9Brk: ;
			
			cursor = v10;
			return true;
		}
	}
}
