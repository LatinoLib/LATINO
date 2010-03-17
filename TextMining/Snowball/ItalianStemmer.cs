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
    public class ItalianStemmer : SnowballProgram, ISnowballStemmer
	{
		public ItalianStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("", - 1, 7, "", this), new Among("qu", 0, 6, "", this), new Among("\u00E1", 0, 1, "", this), new Among("\u00E9", 0, 2, "", this), new Among("\u00ED", 0, 3, "", this), new Among("\u00F3", 0, 4, "", this), new Among("\u00FA", 0, 5, "", this)};
			a1 = new Among[]{new Among("", - 1, 3, "", this), new Among("I", 0, 1, "", this), new Among("U", 0, 2, "", this)};
			a2 = new Among[]{new Among("la", - 1, - 1, "", this), new Among("cela", 0, - 1, "", this), new Among("gliela", 0, - 1, "", this), new Among("mela", 0, - 1, "", this), new Among("tela", 0, - 1, "", this), new Among("vela", 0, - 1, "", this), new Among("le", - 1, - 1, "", this), new Among("cele", 6, - 1, "", this), new Among("gliele", 6, - 1, "", this), new Among("mele", 6, - 1, "", this), new Among("tele", 6, - 1, "", this), new Among("vele", 6, - 1, "", this), new Among("ne", - 1, - 1, "", this), new Among("cene", 12, - 1, "", this), new Among("gliene", 12, - 1, "", this), new Among("mene", 12, - 1, "", this), new Among("sene", 12, - 1, "", this), new Among("tene", 12, - 1, "", this), new Among("vene", 12, - 1, "", this), new Among("ci", - 1, - 1, "", this), new Among("li", - 1, - 1, "", this), new Among("celi", 20, - 1, "", this), new Among("glieli", 20, - 1, "", this), new Among("meli", 20, - 1, "", this), new Among("teli", 20, - 1, "", this), new Among("veli", 20, - 1, "", this), new Among("gli", 20, - 1, "", this), new Among("mi", - 1, - 1, "", this), new Among("si", - 1, - 1, "", this), new Among("ti", - 1, - 1, "", this), new Among("vi", - 1, - 1, "", this), new Among("lo", - 1, - 1, "", this), new Among("celo", 31, - 1, "", this), new Among("glielo", 31, - 1, "", this), new Among("melo", 31, - 1, "", this), new Among("telo", 31, - 1, "", this), new Among("velo", 31, - 1, "", this)};
			a3 = new Among[]{new Among("ando", - 1, 1, "", this), new Among("endo", - 1, 1, "", this), new Among("ar", - 1, 2, "", this), new Among("er", - 1, 2, "", this), new Among("ir", - 1, 2, "", this)};
			a4 = new Among[]{new Among("ic", - 1, - 1, "", this), new Among("abil", - 1, - 1, "", this), new Among("os", - 1, - 1, "", this), new Among("iv", - 1, 1, "", this)};
			a5 = new Among[]{new Among("ic", - 1, 1, "", this), new Among("abil", - 1, 1, "", this), new Among("iv", - 1, 1, "", this)};
			a6 = new Among[]{new Among("ica", - 1, 1, "", this), new Among("logia", - 1, 3, "", this), new Among("osa", - 1, 1, "", this), new Among("ista", - 1, 1, "", this), new Among("iva", - 1, 9, "", this), new Among("anza", - 1, 1, "", this), new Among("enza", - 1, 5, "", this), new Among("ice", - 1, 1, "", this), new Among("atrice", 7, 1, "", this), new Among("iche", - 1, 1, "", this), new Among("logie", - 1, 3, "", this), new Among("abile", - 1, 1, "", this), new Among("ibile", - 1, 1, "", this), new Among("usione", - 1, 4, "", this), new Among("azione", - 1, 2, "", this), new Among("uzione", - 1, 4, "", this), new Among("atore", - 1, 2, "", this), new Among("ose", - 1, 1, "", this), new Among("mente", - 1, 1, "", this), new Among("amente", 18, 7, "", this), new Among("iste", - 1, 1, "", this), new Among("ive", - 1, 9, "", this), new Among("anze", - 1, 1, "", this), new Among("enze", - 1, 5, "", this), new Among("ici", - 1, 1, "", this), new Among("atrici", 24, 1, "", this), new Among("ichi", - 1, 1, "", this), new Among("abili", - 1, 1, "", this), new Among("ibili", - 1, 1, "", this), new Among("ismi", - 1, 1, "", this), new Among("usioni", - 1, 4, "", this), new Among("azioni", - 1, 2, "", this), new Among("uzioni", - 1, 4, "", this), new Among("atori", - 1, 2, "", this), new Among("osi", - 1, 1, "", this), new Among("amenti", - 1, 6, "", this), new Among("imenti", - 1, 6, "", this), new Among("isti", - 1, 1, "", this), new Among("ivi", - 1, 9, "", this), new Among("ico", - 1, 1, "", this), new Among("ismo", - 1, 1, "", this), new Among("oso", - 1, 1, "", this), new Among("amento", - 1, 6, "", this), new Among("imento", - 1, 6, "", this), new Among("ivo", - 1, 9, "", this), new Among("it\u00E0", - 1, 8, "", this), new Among("ist\u00E0", - 1, 1, "", this), new Among("ist\u00E8", - 1, 1, "", this), new Among("ist\u00EC", - 1, 1, "", this)};
			a7 = new Among[]{new Among("isca", - 1, 1, "", this), new Among("enda", - 1, 1, "", this), new Among("ata", - 1, 1, "", this), new Among("ita", - 1, 1, "", this), new Among("uta", - 1, 1, "", this), new Among("ava", - 1, 1, "", this), new Among("eva", - 1, 1, "", this), new Among("iva", - 1, 1, "", this), new Among("erebbe", - 1, 1, "", this), new Among("irebbe", - 1, 1, "", this), new Among("isce", - 1, 1, "", this), new Among("ende", - 1, 1, "", this), new Among("are", - 1, 1, "", this), new Among("ere", - 1, 1, "", this), new Among("ire", - 1, 1, "", this), new Among("asse", - 1, 1, "", this), new Among("ate", - 1, 1, "", this), new Among("avate", 16, 1, "", this), new Among("evate", 16, 1, "", this), new Among("ivate", 16, 1, "", this), new Among("ete", - 1, 1, "", this), new Among("erete", 20, 1, "", this), new Among("irete", 20, 1, "", this), new Among("ite", - 1, 1, "", this), new Among("ereste", - 1, 1, "", this), new Among("ireste", - 1, 1, "", this), new Among("ute", - 1, 1, "", this), new Among("erai", - 1, 1, "", this), new Among("irai", - 1, 1, "", this), new Among("isci", - 1, 1, "", this), new Among("endi", - 1, 1, "", this), new Among("erei", - 1, 1, "", this), new Among("irei", - 1, 1, "", this), new Among("assi", - 1, 1, "", this), new Among("ati", - 1, 1, "", this), new Among("iti", - 1, 1, "", this), new Among("eresti", - 1, 1, "", this), new Among("iresti", - 1, 1, "", this), new Among("uti", - 1, 1, "", this), new Among("avi", - 1, 1, "", this), new Among("evi", - 1, 1, "", this), new Among("ivi", - 1, 1, "", this), new Among("isco", - 1, 1, "", this), new Among("ando", - 1, 1, "", this), new Among("endo", - 1, 1, "", this), new Among("Yamo", - 1, 1, "", this), new Among("iamo", - 1, 1, "", this), new Among("avamo", - 1, 1, "", this), new Among("evamo", - 1, 1, "", this), new Among("ivamo", - 1, 1, "", this), new Among("eremo", - 1, 1, "", this), new Among("iremo", - 1, 1, "", this), new Among("assimo", - 1, 1, "", this), new Among("ammo", - 1, 1, "", this), new Among(
				"emmo", - 1, 1, "", this), new Among("eremmo", 54, 1, "", this), new Among("iremmo", 54, 1, "", this), new Among("immo", - 1, 1, "", this), new Among("ano", - 1, 1, "", this), new Among("iscano", 58, 1, "", this), new Among("avano", 58, 1, "", this), new Among("evano", 58, 1, "", this), new Among("ivano", 58, 1, "", this), new Among("eranno", - 1, 1, "", this), new Among("iranno", - 1, 1, "", this), new Among("ono", - 1, 1, "", this), new Among("iscono", 65, 1, "", this), new Among("arono", 65, 1, "", this), new Among("erono", 65, 1, "", this), new Among("irono", 65, 1, "", this), new Among("erebbero", - 1, 1, "", this), new Among("irebbero", - 1, 1, "", this), new Among("assero", - 1, 1, "", this), new Among("essero", - 1, 1, "", this), new Among("issero", - 1, 1, "", this), new Among("ato", - 1, 1, "", this), new Among("ito", - 1, 1, "", this), new Among("uto", - 1, 1, "", this), new Among("avo", - 1, 1, "", this), new Among("evo", - 1, 1, "", this), new Among("ivo", - 1, 1, "", this), new Among("ar", - 1, 1, "", this), new Among("ir", - 1, 1, "", this), new Among("er\u00E0", - 1, 1, "", this), new Among("ir\u00E0", - 1, 1, "", this), new Among("er\u00F2", - 1, 1, "", this), new Among("ir\u00F2", - 1, 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private Among[] a6;
		private Among[] a7;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128), (char) (128), (char) (8), (char) (2), (char) (1)};
		private static readonly char[] g_AEIO = new char[]{(char) (17), (char) (65), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128), (char) (128), (char) (8), (char) (2)};
		private static readonly char[] g_CG = new char[]{(char) (17)};
		
		private int I_p2;
		private int I_p1;
		private int I_pV;
		
		protected internal virtual void  copyFrom(ItalianStemmer other)
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
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 34
			// test, line 35
			v1 = cursor;
			// repeat, line 35
			while (true)
			{
				v2 = cursor;
				do 
				{
					// (, line 35
					// [, line 36
					bra = cursor;
					// substring, line 36
					amongVar = findAmong(a0, 7);
					if (amongVar == 0)
					{
						goto lab1Brk;
					}
					// ], line 36
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab1Brk;
						
						case 1: 
							// (, line 37
							// <-, line 37
							sliceFrom("\u00E0");
							break;
						
						case 2: 
							// (, line 38
							// <-, line 38
							sliceFrom("\u00E8");
							break;
						
						case 3: 
							// (, line 39
							// <-, line 39
							sliceFrom("\u00EC");
							break;
						
						case 4: 
							// (, line 40
							// <-, line 40
							sliceFrom("\u00F2");
							break;
						
						case 5: 
							// (, line 41
							// <-, line 41
							sliceFrom("\u00F9");
							break;
						
						case 6: 
							// (, line 42
							// <-, line 42
							sliceFrom("qU");
							break;
						
						case 7: 
							// (, line 43
							// next, line 43
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
			// repeat, line 46
			while (true)
			{
				v3 = cursor;
				do 
				{
					// goto, line 46
					while (true)
					{
						v4 = cursor;
						do 
						{
							// (, line 46
							if (!(inGrouping(gV, 97, 249)))
							{
								goto lab5Brk;
							}
							// [, line 47
							bra = cursor;
							// or, line 47
							do 
							{
								v5 = cursor;
								do 
								{
									// (, line 47
									// literal, line 47
									if (!(eqS(1, "u")))
									{
										goto lab7Brk;
									}
									// ], line 47
									ket = cursor;
									if (!(inGrouping(gV, 97, 249)))
									{
										goto lab7Brk;
									}
									// <-, line 47
									sliceFrom("U");
									goto lab6Brk;
								}
								while (false);

lab7Brk: ;
								
								cursor = v5;
								// (, line 48
								// literal, line 48
								if (!(eqS(1, "i")))
								{
									goto lab5Brk;
								}
								// ], line 48
								ket = cursor;
								if (!(inGrouping(gV, 97, 249)))
								{
									goto lab5Brk;
								}
								// <-, line 48
								sliceFrom("I");
							}
							while (false);

lab6Brk: ;
							
							cursor = v4;
							goto golab4Brk;
						}
						while (false);

lab5Brk: ;
						
						cursor = v4;
						if (cursor >= limit)
						{
							goto lab3Brk;
						}
						cursor++;
					}

golab4Brk: ;
					
					goto replab2;
				}
				while (false);

lab3Brk: ;
				
				cursor = v3;
				goto replab2Brk;

replab2: ;
			}

replab2Brk: ;
			
			return true;
		}
		
		private bool rMarkRegions()
		{
			int v1;
			int v2;
			int v3;
			int v6;
			int v8;
			// (, line 52
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			// do, line 58
			v1 = cursor;
			do 
			{
				// (, line 58
				// or, line 60

lab2: 
				do 
				{
					v2 = cursor;
					do 
					{
						// (, line 59
						if (!(inGrouping(gV, 97, 249)))
						{
							goto lab2Brk;
						}
						// or, line 59

lab4: 
						do 
						{
							v3 = cursor;
							do 
							{
								// (, line 59
								if (!(outGrouping(gV, 97, 249)))
								{
									goto lab4Brk;
								}
								// gopast, line 59
								while (true)
								{
									do 
									{
										if (!(inGrouping(gV, 97, 249)))
										{
											goto lab8Brk;
										}
										goto golab5Brk;
									}
									while (false);

lab8Brk: ;
									
									if (cursor >= limit)
									{
										goto lab4Brk;
									}
									cursor++;
								}

golab5Brk: ;
								
								goto lab4Brk;
							}
							while (false);

lab4Brk: ;
							
							cursor = v3;
							// (, line 59
							if (!(inGrouping(gV, 97, 249)))
							{
								goto lab2Brk;
							}
							// gopast, line 59
							while (true)
							{
								do 
								{
									if (!(outGrouping(gV, 97, 249)))
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
						goto lab2Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = v2;
					// (, line 61
					if (!(outGrouping(gV, 97, 249)))
					{
						goto lab0Brk;
					}
					// or, line 61
					do 
					{
						v6 = cursor;
						do 
						{
							// (, line 61
							if (!(outGrouping(gV, 97, 249)))
							{
								goto lab10Brk;
							}
							// gopast, line 61
							while (true)
							{
								do 
								{
									if (!(inGrouping(gV, 97, 249)))
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
						// (, line 61
						if (!(inGrouping(gV, 97, 249)))
						{
							goto lab0Brk;
						}
						// next, line 61
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
				// setmark pV, line 62
				I_pV = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 64
			v8 = cursor;
			do 
			{
				// (, line 64
				// gopast, line 65
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 249)))
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
				
				// gopast, line 65
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 249)))
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
				
				// setmark p1, line 65
				I_p1 = cursor;
				// gopast, line 66
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 249)))
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
				
				// gopast, line 66
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 249)))
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
				
				// setmark p2, line 66
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
			// repeat, line 70
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 70
					// [, line 72
					bra = cursor;
					// substring, line 72
					amongVar = findAmong(a1, 3);
					if (amongVar == 0)
					{
						goto lab11Brk;
					}
					// ], line 72
					ket = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab11Brk;
						
						case 1: 
							// (, line 73
							// <-, line 73
							sliceFrom("i");
							break;
						
						case 2: 
							// (, line 74
							// <-, line 74
							sliceFrom("u");
							break;
						
						case 3: 
							// (, line 75
							// next, line 75
							if (cursor >= limit)
							{
								goto lab11Brk;
							}
							cursor++;
							break;
						}
					goto replab1;
				}
				while (false);

lab11Brk: ;
				
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
		
		private bool rAttachedPronoun()
		{
			int amongVar;
			// (, line 86
			// [, line 87
			ket = cursor;
			// substring, line 87
			if (findAmongB(a2, 37) == 0)
			{
				return false;
			}
			// ], line 87
			bra = cursor;
			// among, line 97
			amongVar = findAmongB(a3, 5);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 97
			// call RV, line 97
			if (!r_RV())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 98
					// delete, line 98
					sliceDel();
					break;
				
				case 2: 
					// (, line 99
					// <-, line 99
					sliceFrom("e");
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
			// (, line 103
			// [, line 104
			ket = cursor;
			// substring, line 104
			amongVar = findAmongB(a6, 49);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 104
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 110
					// call R2, line 110
					if (!r_R2())
					{
						return false;
					}
					// delete, line 110
					sliceDel();
					break;
				
				case 2: 
					// (, line 112
					// call R2, line 112
					if (!r_R2())
					{
						return false;
					}
					// delete, line 112
					sliceDel();
					// try, line 113
					v1 = limit - cursor;
					do 
					{
						// (, line 113
						// [, line 113
						ket = cursor;
						// literal, line 113
						if (!(eqSB(2, "ic")))
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// ], line 113
						bra = cursor;
						// call R2, line 113
						if (!r_R2())
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// delete, line 113
						sliceDel();
					}
					while (false);

lab0Brk: ;
					
					break;
				
				case 3: 
					// (, line 116
					// call R2, line 116
					if (!r_R2())
					{
						return false;
					}
					// <-, line 116
					sliceFrom("log");
					break;
				
				case 4: 
					// (, line 118
					// call R2, line 118
					if (!r_R2())
					{
						return false;
					}
					// <-, line 118
					sliceFrom("u");
					break;
				
				case 5: 
					// (, line 120
					// call R2, line 120
					if (!r_R2())
					{
						return false;
					}
					// <-, line 120
					sliceFrom("ente");
					break;
				
				case 6: 
					// (, line 122
					// call RV, line 122
					if (!r_RV())
					{
						return false;
					}
					// delete, line 122
					sliceDel();
					break;
				
				case 7: 
					// (, line 123
					// call R1, line 124
					if (!r_R1())
					{
						return false;
					}
					// delete, line 124
					sliceDel();
					// try, line 125
					v2 = limit - cursor;
					do 
					{
						// (, line 125
						// [, line 126
						ket = cursor;
						// substring, line 126
						amongVar = findAmongB(a4, 4);
						if (amongVar == 0)
						{
							cursor = limit - v2;
							goto lab1Brk;
						}
						// ], line 126
						bra = cursor;
						// call R2, line 126
						if (!r_R2())
						{
							cursor = limit - v2;
							goto lab1Brk;
						}
						// delete, line 126
						sliceDel();
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v2;
								goto lab1Brk;
							
							case 1: 
								// (, line 127
								// [, line 127
								ket = cursor;
								// literal, line 127
								if (!(eqSB(2, "at")))
								{
									cursor = limit - v2;
									goto lab1Brk;
								}
								// ], line 127
								bra = cursor;
								// call R2, line 127
								if (!r_R2())
								{
									cursor = limit - v2;
									goto lab1Brk;
								}
								// delete, line 127
								sliceDel();
								break;
							}
					}
					while (false);

lab1Brk: ;
					
					break;
				
				case 8: 
					// (, line 132
					// call R2, line 133
					if (!r_R2())
					{
						return false;
					}
					// delete, line 133
					sliceDel();
					// try, line 134
					v3 = limit - cursor;
					do 
					{
						// (, line 134
						// [, line 135
						ket = cursor;
						// substring, line 135
						amongVar = findAmongB(a5, 3);
						if (amongVar == 0)
						{
							cursor = limit - v3;
							goto lab2Brk;
						}
						// ], line 135
						bra = cursor;
						switch (amongVar)
						{
							
							case 0: 
								cursor = limit - v3;
								goto lab2Brk;
							
							case 1: 
								// (, line 136
								// call R2, line 136
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab2Brk;
								}
								// delete, line 136
								sliceDel();
								break;
							}
					}
					while (false);

lab2Brk: ;
					
					break;
				
				case 9: 
					// (, line 140
					// call R2, line 141
					if (!r_R2())
					{
						return false;
					}
					// delete, line 141
					sliceDel();
					// try, line 142
					v4 = limit - cursor;
					do 
					{
						// (, line 142
						// [, line 142
						ket = cursor;
						// literal, line 142
						if (!(eqSB(2, "at")))
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// ], line 142
						bra = cursor;
						// call R2, line 142
						if (!r_R2())
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// delete, line 142
						sliceDel();
						// [, line 142
						ket = cursor;
						// literal, line 142
						if (!(eqSB(2, "ic")))
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// ], line 142
						bra = cursor;
						// call R2, line 142
						if (!r_R2())
						{
							cursor = limit - v4;
							goto lab3Brk;
						}
						// delete, line 142
						sliceDel();
					}
					while (false);

lab3Brk: ;
					
					break;
				}
			return true;
		}
		
		private bool rVerbSuffix()
		{
			int amongVar;
			int v1;
			int v2;
			// setlimit, line 147
			v1 = limit - cursor;
			// tomark, line 147
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 147
			// [, line 148
			ket = cursor;
			// substring, line 148
			amongVar = findAmongB(a7, 87);
			if (amongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 148
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					limitBackward = v2;
					return false;
				
				case 1: 
					// (, line 162
					// delete, line 162
					sliceDel();
					break;
				}
			limitBackward = v2;
			return true;
		}
		
		private bool rVowelSuffix()
		{
			int v1;
			int v2;
			// (, line 169
			// try, line 170
			v1 = limit - cursor;
			do 
			{
				// (, line 170
				// [, line 171
				ket = cursor;
				if (!(inGroupingB(g_AEIO, 97, 242)))
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// ], line 171
				bra = cursor;
				// call RV, line 171
				if (!r_RV())
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// delete, line 171
				sliceDel();
				// [, line 172
				ket = cursor;
				// literal, line 172
				if (!(eqSB(1, "i")))
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// ], line 172
				bra = cursor;
				// call RV, line 172
				if (!r_RV())
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// delete, line 172
				sliceDel();
			}
			while (false);

lab0Brk: ;
			
			// try, line 174
			v2 = limit - cursor;
			do 
			{
				// (, line 174
				// [, line 175
				ket = cursor;
				// literal, line 175
				if (!(eqSB(1, "h")))
				{
					cursor = limit - v2;
					goto lab1Brk;
				}
				// ], line 175
				bra = cursor;
				if (!(inGroupingB(g_CG, 99, 103)))
				{
					cursor = limit - v2;
					goto lab1Brk;
				}
				// call RV, line 175
				if (!r_RV())
				{
					cursor = limit - v2;
					goto lab1Brk;
				}
				// delete, line 175
				sliceDel();
			}
			while (false);

lab1Brk: ;
			
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
			// (, line 180
			// do, line 181
			v1 = cursor;
			do 
			{
				// call prelude, line 181
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 182
			v2 = cursor;
			do 
			{
				// call markRegions, line 182
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 183
			limitBackward = cursor; cursor = limit;
			// (, line 183
			// do, line 184
			v3 = limit - cursor;
			do 
			{
				// call attachedPronoun, line 184
				if (!rAttachedPronoun())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			// do, line 185
			v4 = limit - cursor;
			do 
			{
				// (, line 185
				// or, line 185
				do 
				{
					v5 = limit - cursor;
					do 
					{
						// call standardSuffix, line 185
						if (!rStandardSuffix())
						{
							goto lab5Brk;
						}
						goto lab4Brk;
					}
					while (false);

lab5Brk: ;
					
					cursor = limit - v5;
					// call verbSuffix, line 185
					if (!rVerbSuffix())
					{
						goto lab3Brk;
					}
				}
				while (false);

lab4Brk: ;
				
			}
			while (false);

lab3Brk: ;

			cursor = limit - v4;
			// do, line 186
			v6 = limit - cursor;
			do 
			{
				// call vowelSuffix, line 186
				if (!rVowelSuffix())
				{
					goto lab6Brk;
				}
			}
			while (false);

lab6Brk: ;
			
			cursor = limit - v6;
			cursor = limitBackward; // do, line 188
			v7 = cursor;
			do 
			{
				// call postlude, line 188
				if (!rPostlude())
				{
					goto lab7Brk;
				}
			}
			while (false);

lab7Brk: ;
			
			cursor = v7;
			return true;
		}
	}
}
