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
    public class ItalianStemmer : SnowballProgram, ISnowballStemmer
	{
		static ItalianStemmer()
		{
			InitBlock();
		}
		private static void InitBlock()
		{
			a0 = new MyAmong[]{new MyAmong("", - 1, 7), new MyAmong("qu", 0, 6), new MyAmong("\u00E1", 0, 1), new MyAmong("\u00E9", 0, 2), new MyAmong("\u00ED", 0, 3), new MyAmong("\u00F3", 0, 4), new MyAmong("\u00FA", 0, 5)};
			a1 = new MyAmong[]{new MyAmong("", - 1, 3), new MyAmong("I", 0, 1), new MyAmong("U", 0, 2)};
			a2 = new MyAmong[]{new MyAmong("la", - 1, - 1), new MyAmong("cela", 0, - 1), new MyAmong("gliela", 0, - 1), new MyAmong("mela", 0, - 1), new MyAmong("tela", 0, - 1), new MyAmong("vela", 0, - 1), new MyAmong("le", - 1, - 1), new MyAmong("cele", 6, - 1), new MyAmong("gliele", 6, - 1), new MyAmong("mele", 6, - 1), new MyAmong("tele", 6, - 1), new MyAmong("vele", 6, - 1), new MyAmong("ne", - 1, - 1), new MyAmong("cene", 12, - 1), new MyAmong("gliene", 12, - 1), new MyAmong("mene", 12, - 1), new MyAmong("sene", 12, - 1), new MyAmong("tene", 12, - 1), new MyAmong("vene", 12, - 1), new MyAmong("ci", - 1, - 1), new MyAmong("li", - 1, - 1), new MyAmong("celi", 20, - 1), new MyAmong("glieli", 20, - 1), new MyAmong("meli", 20, - 1), new MyAmong("teli", 20, - 1), new MyAmong("veli", 20, - 1), new MyAmong("gli", 20, - 1), new MyAmong("mi", - 1, - 1), new MyAmong("si", - 1, - 1), new MyAmong("ti", - 1, - 1), new MyAmong("vi", - 1, - 1), new MyAmong("lo", - 1, - 1), new MyAmong("celo", 31, - 1), new MyAmong("glielo", 31, - 1), new MyAmong("melo", 31, - 1), new MyAmong("telo", 31, - 1), new MyAmong("velo", 31, - 1)};
			a3 = new MyAmong[]{new MyAmong("ando", - 1, 1), new MyAmong("endo", - 1, 1), new MyAmong("ar", - 1, 2), new MyAmong("er", - 1, 2), new MyAmong("ir", - 1, 2)};
			a4 = new MyAmong[]{new MyAmong("ic", - 1, - 1), new MyAmong("abil", - 1, - 1), new MyAmong("os", - 1, - 1), new MyAmong("iv", - 1, 1)};
			a5 = new MyAmong[]{new MyAmong("ic", - 1, 1), new MyAmong("abil", - 1, 1), new MyAmong("iv", - 1, 1)};
			a6 = new MyAmong[]{new MyAmong("ica", - 1, 1), new MyAmong("logia", - 1, 3), new MyAmong("osa", - 1, 1), new MyAmong("ista", - 1, 1), new MyAmong("iva", - 1, 9), new MyAmong("anza", - 1, 1), new MyAmong("enza", - 1, 5), new MyAmong("ice", - 1, 1), new MyAmong("atrice", 7, 1), new MyAmong("iche", - 1, 1), new MyAmong("logie", - 1, 3), new MyAmong("abile", - 1, 1), new MyAmong("ibile", - 1, 1), new MyAmong("usione", - 1, 4), new MyAmong("azione", - 1, 2), new MyAmong("uzione", - 1, 4), new MyAmong("atore", - 1, 2), new MyAmong("ose", - 1, 1), new MyAmong("mente", - 1, 1), new MyAmong("amente", 18, 7), new MyAmong("iste", - 1, 1), new MyAmong("ive", - 1, 9), new MyAmong("anze", - 1, 1), new MyAmong("enze", - 1, 5), new MyAmong("ici", - 1, 1), new MyAmong("atrici", 24, 1), new MyAmong("ichi", - 1, 1), new MyAmong("abili", - 1, 1), new MyAmong("ibili", - 1, 1), new MyAmong("ismi", - 1, 1), new MyAmong("usioni", - 1, 4), new MyAmong("azioni", - 1, 2), new MyAmong("uzioni", - 1, 4), new MyAmong("atori", - 1, 2), new MyAmong("osi", - 1, 1), new MyAmong("amenti", - 1, 6), new MyAmong("imenti", - 1, 6), new MyAmong("isti", - 1, 1), new MyAmong("ivi", - 1, 9), new MyAmong("ico", - 1, 1), new MyAmong("ismo", - 1, 1), new MyAmong("oso", - 1, 1), new MyAmong("amento", - 1, 6), new MyAmong("imento", - 1, 6), new MyAmong("ivo", - 1, 9), new MyAmong("it\u00E0", - 1, 8), new MyAmong("ist\u00E0", - 1, 1), new MyAmong("ist\u00E8", - 1, 1), new MyAmong("ist\u00EC", - 1, 1)};
			a7 = new MyAmong[]{new MyAmong("isca", - 1, 1), new MyAmong("enda", - 1, 1), new MyAmong("ata", - 1, 1), new MyAmong("ita", - 1, 1), new MyAmong("uta", - 1, 1), new MyAmong("ava", - 1, 1), new MyAmong("eva", - 1, 1), new MyAmong("iva", - 1, 1), new MyAmong("erebbe", - 1, 1), new MyAmong("irebbe", - 1, 1), new MyAmong("isce", - 1, 1), new MyAmong("ende", - 1, 1), new MyAmong("are", - 1, 1), new MyAmong("ere", - 1, 1), new MyAmong("ire", - 1, 1), new MyAmong("asse", - 1, 1), new MyAmong("ate", - 1, 1), new MyAmong("avate", 16, 1), new MyAmong("evate", 16, 1), new MyAmong("ivate", 16, 1), new MyAmong("ete", - 1, 1), new MyAmong("erete", 20, 1), new MyAmong("irete", 20, 1), new MyAmong("ite", - 1, 1), new MyAmong("ereste", - 1, 1), new MyAmong("ireste", - 1, 1), new MyAmong("ute", - 1, 1), new MyAmong("erai", - 1, 1), new MyAmong("irai", - 1, 1), new MyAmong("isci", - 1, 1), new MyAmong("endi", - 1, 1), new MyAmong("erei", - 1, 1), new MyAmong("irei", - 1, 1), new MyAmong("assi", - 1, 1), new MyAmong("ati", - 1, 1), new MyAmong("iti", - 1, 1), new MyAmong("eresti", - 1, 1), new MyAmong("iresti", - 1, 1), new MyAmong("uti", - 1, 1), new MyAmong("avi", - 1, 1), new MyAmong("evi", - 1, 1), new MyAmong("ivi", - 1, 1), new MyAmong("isco", - 1, 1), new MyAmong("ando", - 1, 1), new MyAmong("endo", - 1, 1), new MyAmong("Yamo", - 1, 1), new MyAmong("iamo", - 1, 1), new MyAmong("avamo", - 1, 1), new MyAmong("evamo", - 1, 1), new MyAmong("ivamo", - 1, 1), new MyAmong("eremo", - 1, 1), new MyAmong("iremo", - 1, 1), new MyAmong("assimo", - 1, 1), new MyAmong("ammo", - 1, 1), new MyAmong(
				"emmo", - 1, 1), new MyAmong("eremmo", 54, 1), new MyAmong("iremmo", 54, 1), new MyAmong("immo", - 1, 1), new MyAmong("ano", - 1, 1), new MyAmong("iscano", 58, 1), new MyAmong("avano", 58, 1), new MyAmong("evano", 58, 1), new MyAmong("ivano", 58, 1), new MyAmong("eranno", - 1, 1), new MyAmong("iranno", - 1, 1), new MyAmong("ono", - 1, 1), new MyAmong("iscono", 65, 1), new MyAmong("arono", 65, 1), new MyAmong("erono", 65, 1), new MyAmong("irono", 65, 1), new MyAmong("erebbero", - 1, 1), new MyAmong("irebbero", - 1, 1), new MyAmong("assero", - 1, 1), new MyAmong("essero", - 1, 1), new MyAmong("issero", - 1, 1), new MyAmong("ato", - 1, 1), new MyAmong("ito", - 1, 1), new MyAmong("uto", - 1, 1), new MyAmong("avo", - 1, 1), new MyAmong("evo", - 1, 1), new MyAmong("ivo", - 1, 1), new MyAmong("ar", - 1, 1), new MyAmong("ir", - 1, 1), new MyAmong("er\u00E0", - 1, 1), new MyAmong("ir\u00E0", - 1, 1), new MyAmong("er\u00F2", - 1, 1), new MyAmong("ir\u00F2", - 1, 1)};
		}
		
		private static MyAmong[] a0;
		private static MyAmong[] a1;
		private static MyAmong[] a2;
		private static MyAmong[] a3;
		private static MyAmong[] a4;
		private static MyAmong[] a5;
		private static MyAmong[] a6;
		private static MyAmong[] a7;
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
			int MyAmongVar;
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
					MyAmongVar = findAmong(a0, 7);
					if (MyAmongVar == 0)
					{
						goto lab1Brk;
					}
					// ], line 36
					ket = cursor;
					switch (MyAmongVar)
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
			int MyAmongVar;
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
					MyAmongVar = findAmong(a1, 3);
					if (MyAmongVar == 0)
					{
						goto lab11Brk;
					}
					// ], line 72
					ket = cursor;
					switch (MyAmongVar)
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
			int MyAmongVar;
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
			// MyAmong, line 97
			MyAmongVar = findAmongB(a3, 5);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// (, line 97
			// call RV, line 97
			if (!r_RV())
			{
				return false;
			}
			switch (MyAmongVar)
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
			int MyAmongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 103
			// [, line 104
			ket = cursor;
			// substring, line 104
			MyAmongVar = findAmongB(a6, 49);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 104
			bra = cursor;
			switch (MyAmongVar)
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
						MyAmongVar = findAmongB(a4, 4);
						if (MyAmongVar == 0)
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
						switch (MyAmongVar)
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
						MyAmongVar = findAmongB(a5, 3);
						if (MyAmongVar == 0)
						{
							cursor = limit - v3;
							goto lab2Brk;
						}
						// ], line 135
						bra = cursor;
						switch (MyAmongVar)
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
			int MyAmongVar;
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
			MyAmongVar = findAmongB(a7, 87);
			if (MyAmongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 148
			bra = cursor;
			switch (MyAmongVar)
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
