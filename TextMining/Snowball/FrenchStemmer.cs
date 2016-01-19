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
    public class FrenchStemmer : SnowballProgram, ISnowballStemmer
	{
		public FrenchStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new MyAmong[]{new MyAmong("", - 1, 4), new MyAmong("I", 0, 1), new MyAmong("U", 0, 2), new MyAmong("Y", 0, 3)};
			a1 = new MyAmong[]{new MyAmong("iqU", - 1, 3), new MyAmong("abl", - 1, 3), new MyAmong("I\u00E8r", - 1, 4), new MyAmong("i\u00E8r", - 1, 4), new MyAmong("eus", - 1, 2), new MyAmong("iv", - 1, 1)};
			a2 = new MyAmong[]{new MyAmong("ic", - 1, 2), new MyAmong("abil", - 1, 1), new MyAmong("iv", - 1, 3)};
			a3 = new MyAmong[]{new MyAmong("iqUe", - 1, 1), new MyAmong("atrice", - 1, 2), new MyAmong("ance", - 1, 1), new MyAmong("ence", - 1, 5), new MyAmong("logie", - 1, 3), new MyAmong("able", - 1, 1), new MyAmong("isme", - 1, 1), new MyAmong("euse", - 1, 11), new MyAmong("iste", - 1, 1), new MyAmong("ive", - 1, 8), new MyAmong("if", - 1, 8), new MyAmong("usion", - 1, 4), new MyAmong("ation", - 1, 2), new MyAmong("ution", - 1, 4), new MyAmong("ateur", - 1, 2), new MyAmong("iqUes", - 1, 1), new MyAmong("atrices", - 1, 2), new MyAmong("ances", - 1, 1), new MyAmong("ences", - 1, 5), new MyAmong("logies", - 1, 3), new MyAmong("ables", - 1, 1), new MyAmong("ismes", - 1, 1), new MyAmong("euses", - 1, 11), new MyAmong("istes", - 1, 1), new MyAmong("ives", - 1, 8), new MyAmong("ifs", - 1, 8), new MyAmong("usions", - 1, 4), new MyAmong("ations", - 1, 2), new MyAmong("utions", - 1, 4), new MyAmong("ateurs", - 1, 2), new MyAmong("ments", - 1, 15), new MyAmong("ements", 30, 6), new MyAmong("issements", 31, 12), new MyAmong("it\u00E9s", - 1, 7), new MyAmong("ment", - 1, 15), new MyAmong("ement", 34, 6), new MyAmong("issement", 35, 12), new MyAmong("amment", 34, 13), new MyAmong("emment", 34, 14), new MyAmong("aux", - 1, 10), new MyAmong("eaux", 39, 9), new MyAmong("eux", - 1, 1), new MyAmong("it\u00E9", - 1, 7)};
			a4 = new MyAmong[]{new MyAmong("ira", - 1, 1), new MyAmong("ie", - 1, 1), new MyAmong("isse", - 1, 1), new MyAmong("issante", - 1, 1), new MyAmong("i", - 1, 1), new MyAmong("irai", 4, 1), new MyAmong("ir", - 1, 1), new MyAmong("iras", - 1, 1), new MyAmong("ies", - 1, 1), new MyAmong("\u00EEmes", - 1, 1), new MyAmong("isses", - 1, 1), new MyAmong("issantes", - 1, 1), new MyAmong("\u00EEtes", - 1, 1), new MyAmong("is", - 1, 1), new MyAmong("irais", 13, 1), new MyAmong("issais", 13, 1), new MyAmong("irions", - 1, 1), new MyAmong("issions", - 1, 1), new MyAmong("irons", - 1, 1), new MyAmong("issons", - 1, 1), new MyAmong("issants", - 1, 1), new MyAmong("it", - 1, 1), new MyAmong("irait", 21, 1), new MyAmong("issait", 21, 1), new MyAmong("issant", - 1, 1), new MyAmong("iraIent", - 1, 1), new MyAmong("issaIent", - 1, 1), new MyAmong("irent", - 1, 1), new MyAmong("issent", - 1, 1), new MyAmong("iront", - 1, 1), new MyAmong("\u00EEt", - 1, 1), new MyAmong("iriez", - 1, 1), new MyAmong("issiez", - 1, 1), new MyAmong("irez", - 1, 1), new MyAmong("issez", - 1, 1)};
			a5 = new MyAmong[]{new MyAmong("a", - 1, 3), new MyAmong("era", 0, 2), new MyAmong("asse", - 1, 3), new MyAmong("ante", - 1, 3), new MyAmong("\u00E9e", - 1, 2), new MyAmong("ai", - 1, 3), new MyAmong("erai", 5, 2), new MyAmong("er", - 1, 2), new MyAmong("as", - 1, 3), new MyAmong("eras", 8, 2), new MyAmong("\u00E2mes", - 1, 3), new MyAmong("asses", - 1, 3), new MyAmong("antes", - 1, 3), new MyAmong("\u00E2tes", - 1, 3), new MyAmong("\u00E9es", - 1, 2), new MyAmong("ais", - 1, 3), new MyAmong("erais", 15, 2), new MyAmong("ions", - 1, 1), new MyAmong("erions", 17, 2), new MyAmong("assions", 17, 3), new MyAmong("erons", - 1, 2), new MyAmong("ants", - 1, 3), new MyAmong("\u00E9s", - 1, 2), new MyAmong("ait", - 1, 3), new MyAmong("erait", 23, 2), new MyAmong("ant", - 1, 3), new MyAmong("aIent", - 1, 3), new MyAmong("eraIent", 26, 2), new MyAmong("\u00E8rent", - 1, 2), new MyAmong("assent", - 1, 3), new MyAmong("eront", - 1, 2), new MyAmong("\u00E2t", - 1, 3), new MyAmong("ez", - 1, 2), new MyAmong("iez", 32, 2), new MyAmong("eriez", 33, 2), new MyAmong("assiez", 33, 3), new MyAmong("erez", 32, 2), new MyAmong("\u00E9", - 1, 2)};
			a6 = new MyAmong[]{new MyAmong("e", - 1, 3), new MyAmong("I\u00E8re", 0, 2), new MyAmong("i\u00E8re", 0, 2), new MyAmong("ion", - 1, 1), new MyAmong("Ier", - 1, 2), new MyAmong("ier", - 1, 2), new MyAmong("\u00EB", - 1, 4)};
			a7 = new MyAmong[]{new MyAmong("ell", - 1, - 1), new MyAmong("eill", - 1, - 1), new MyAmong("enn", - 1, - 1), new MyAmong("onn", - 1, - 1), new MyAmong("ett", - 1, - 1)};
		}
		
		private MyAmong[] a0;
		private MyAmong[] a1;
		private MyAmong[] a2;
		private MyAmong[] a3;
		private MyAmong[] a4;
		private MyAmong[] a5;
		private MyAmong[] a6;
		private MyAmong[] a7;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128), (char) (130), (char) (103), (char) (8), (char) (5)};
		private static readonly char[] gKeepWithS = new char[]{(char) (1), (char) (65), (char) (20), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (0), (char) (128)};
		
		private int I_p2;
		private int I_p1;
		private int I_pV;
		protected internal virtual void  copyFrom(FrenchStemmer other)
		{
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			I_pV = other.I_pV;
			base.copyFrom(other);
		}
		
		private bool rPrelude()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// repeat, line 38
			while (true)
			{
				v1 = cursor;
				do 
				{
					// goto, line 38
					while (true)
					{
						v2 = cursor;
						do 
						{
							// (, line 38
							// or, line 44
							do 
							{
								v3 = cursor;
								do 
								{
									// (, line 40
									if (!(inGrouping(gV, 97, 251)))
									{
										goto lab5Brk;
									}
									// [, line 40
									bra = cursor;
									// or, line 40
									do 
									{
										v4 = cursor;
										do 
										{
											// (, line 40
											// literal, line 40
											if (!(eqS(1, "u")))
											{
												goto lab7Brk;
											}
											// ], line 40
											ket = cursor;
											if (!(inGrouping(gV, 97, 251)))
											{
												goto lab7Brk;
											}
											// <-, line 40
											sliceFrom("U");
											goto lab6Brk;
										}
										while (false);

lab7Brk: ;
										
										cursor = v4;
										do 
										{
											// (, line 41
											// literal, line 41
											if (!(eqS(1, "i")))
											{
												goto lab8Brk;
											}
											// ], line 41
											ket = cursor;
											if (!(inGrouping(gV, 97, 251)))
											{
												goto lab8Brk;
											}
											// <-, line 41
											sliceFrom("I");
											goto lab6Brk;
										}
										while (false);

lab8Brk: ;
										
										cursor = v4;
										// (, line 42
										// literal, line 42
										if (!(eqS(1, "y")))
										{
											goto lab5Brk;
										}
										// ], line 42
										ket = cursor;
										// <-, line 42
										sliceFrom("Y");
									}
									while (false);

lab6Brk: ;
									
									goto lab4Brk;
								}
								while (false);

lab5Brk: ;
								
								cursor = v3;
								do 
								{
									// (, line 45
									// [, line 45
									bra = cursor;
									// literal, line 45
									if (!(eqS(1, "y")))
									{
										goto lab9Brk;
									}
									// ], line 45
									ket = cursor;
									if (!(inGrouping(gV, 97, 251)))
									{
										goto lab9Brk;
									}
									// <-, line 45
									sliceFrom("Y");
									goto lab4Brk;
								}
								while (false);

lab9Brk: ;
								
								cursor = v3;
								// (, line 47
								// literal, line 47
								if (!(eqS(1, "q")))
								{
									goto lab3Brk;
								}
								// [, line 47
								bra = cursor;
								// literal, line 47
								if (!(eqS(1, "u")))
								{
									goto lab3Brk;
								}
								// ], line 47
								ket = cursor;
								// <-, line 47
								sliceFrom("U");
							}
							while (false);

lab4Brk: ;
							
							cursor = v2;
							goto golab2Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = v2;
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
			int v4;
			// (, line 50
			I_pV = limit;
			I_p1 = limit;
			I_p2 = limit;
			// do, line 56
			v1 = cursor;
			do 
			{
				// (, line 56
				// or, line 57

				do 
				{
					v2 = cursor;
					do 
					{
						// (, line 57
						if (!(inGrouping(gV, 97, 251)))
						{
							goto lab2Brk;
						}
						if (!(inGrouping(gV, 97, 251)))
						{
							goto lab2Brk;
						}
						// next, line 57
						if (cursor >= limit)
						{
							goto lab2Brk;
						}
						cursor++;
						goto lab1Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = v2;
					// (, line 57
					// next, line 57
					if (cursor >= limit)
					{
						goto lab0Brk;
					}
					cursor++;
					// gopast, line 57
					while (true)
					{
						do 
						{
							if (!(inGrouping(gV, 97, 251)))
							{
								goto lab4Brk;
							}
							goto golab3Brk;
						}
						while (false);

lab4Brk: ;
						
						if (cursor >= limit)
						{
							goto lab0Brk;
						}
						cursor++;
					}

golab3Brk: ;
					
				}
				while (false);

lab1Brk: ;
				// setmark pV, line 58
				I_pV = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 60
			v4 = cursor;
			do 
			{
				// (, line 60
				// gopast, line 61
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 251)))
						{
							goto lab7Brk;
						}
						goto golab6Brk;
					}
					while (false);

lab7Brk: ;
					
					if (cursor >= limit)
					{
						goto lab5Brk;
					}
					cursor++;
				}

golab6Brk: ;
				
				// gopast, line 61
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 251)))
						{
							goto lab9Brk;
						}
						goto golab8Brk;
					}
					while (false);

lab9Brk: ;
					
					if (cursor >= limit)
					{
						goto lab5Brk;
					}
					cursor++;
				}

golab8Brk: ;
				
				// setmark p1, line 61
				I_p1 = cursor;
				// gopast, line 62
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 251)))
						{
							goto lab11Brk;
						}
						goto golab10Brk;
					}
					while (false);

lab11Brk: ;
					
					if (cursor >= limit)
					{
						goto lab5Brk;
					}
					cursor++;
				}

golab10Brk: ;
				
				// gopast, line 62
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 251)))
						{
							goto lab13Brk;
						}
						goto golab12Brk;
					}
					while (false);

lab13Brk: ;
					
					if (cursor >= limit)
					{
						goto lab5Brk;
					}
					cursor++;
				}

golab12Brk: ;
				
				// setmark p2, line 62
				I_p2 = cursor;
			}
			while (false);

lab5Brk: ;
			
			cursor = v4;
			return true;
		}
		
		private bool rPostlude()
		{
			int MyAmongVar;
			int v1;
			// repeat, line 66
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 66
					// [, line 68
					bra = cursor;
					// substring, line 68
					MyAmongVar = findAmong(a0, 4);
					if (MyAmongVar == 0)
					{
						goto lab10Brk;
					}
					// ], line 68
					ket = cursor;
					switch (MyAmongVar)
					{
						
						case 0: 
							goto lab10Brk;
						
						case 1: 
							// (, line 69
							// <-, line 69
							sliceFrom("i");
							break;
						
						case 2: 
							// (, line 70
							// <-, line 70
							sliceFrom("u");
							break;
						
						case 3: 
							// (, line 71
							// <-, line 71
							sliceFrom("y");
							break;
						
						case 4: 
							// (, line 72
							// next, line 72
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
			int MyAmongVar;
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
			int v11;
			// (, line 82
			// [, line 83
			ket = cursor;
			// substring, line 83
			MyAmongVar = findAmongB(a3, 43);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 83
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 87
					// call R2, line 87
					if (!r_R2())
					{
						return false;
					}
					// delete, line 87
					sliceDel();
					break;
				
				case 2: 
					// (, line 90
					// call R2, line 90
					if (!r_R2())
					{
						return false;
					}
					// delete, line 90
					sliceDel();
					// try, line 91
					v1 = limit - cursor;
					do 
					{
						// (, line 91
						// [, line 91
						ket = cursor;
						// literal, line 91
						if (!(eqSB(2, "ic")))
						{
							cursor = limit - v1;
							goto lab0Brk;
						}
						// ], line 91
						bra = cursor;
						// or, line 91
						do 
						{
							v2 = limit - cursor;
							do 
							{
								// (, line 91
								// call R2, line 91
								if (!r_R2())
								{
									goto lab2Brk;
								}
								// delete, line 91
								sliceDel();
								goto lab1Brk;
							}
							while (false);

lab2Brk: ;
							
							cursor = limit - v2;
							// <-, line 91
							sliceFrom("iqU");
						}
						while (false);

lab1Brk: ;
					}
					while (false);

lab0Brk: ;
					
					break;
				
				case 3: 
					// (, line 95
					// call R2, line 95
					if (!r_R2())
					{
						return false;
					}
					// <-, line 95
					sliceFrom("log");
					break;
				
				case 4: 
					// (, line 98
					// call R2, line 98
					if (!r_R2())
					{
						return false;
					}
					// <-, line 98
					sliceFrom("u");
					break;
				
				case 5: 
					// (, line 101
					// call R2, line 101
					if (!r_R2())
					{
						return false;
					}
					// <-, line 101
					sliceFrom("ent");
					break;
				
				case 6: 
					// (, line 104
					// call RV, line 105
					if (!r_RV())
					{
						return false;
					}
					// delete, line 105
					sliceDel();
					// try, line 106
					v3 = limit - cursor;
					do 
					{
						// (, line 106
						// [, line 107
						ket = cursor;
						// substring, line 107
						MyAmongVar = findAmongB(a1, 6);
						if (MyAmongVar == 0)
						{
							cursor = limit - v3;
							goto lab3Brk;
						}
						// ], line 107
						bra = cursor;
						switch (MyAmongVar)
						{
							
							case 0: 
								cursor = limit - v3;
								goto lab3Brk;
							
							case 1: 
								// (, line 108
								// call R2, line 108
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab3Brk;
								}
								// delete, line 108
								sliceDel();
								// [, line 108
								ket = cursor;
								// literal, line 108
								if (!(eqSB(2, "at")))
								{
									cursor = limit - v3;
									goto lab3Brk;
								}
								// ], line 108
								bra = cursor;
								// call R2, line 108
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab3Brk;
								}
								// delete, line 108
								sliceDel();
								break;
							
							case 2: 
								// (, line 109
								// or, line 109
								do 
								{
									v4 = limit - cursor;
									do 
									{
										// (, line 109
										// call R2, line 109
										if (!r_R2())
										{
											goto lab5Brk;
										}
										// delete, line 109
										sliceDel();
										goto lab4Brk;
									}
									while (false);

lab5Brk: ;
									
									cursor = limit - v4;
									// (, line 109
									// call R1, line 109
									if (!r_R1())
									{
										cursor = limit - v3;
										goto lab3Brk;
									}
									// <-, line 109
									sliceFrom("eux");
								}
								while (false);

lab4Brk: ;
								
								break;
							
							case 3: 
								// (, line 111
								// call R2, line 111
								if (!r_R2())
								{
									cursor = limit - v3;
									goto lab3Brk;
								}
								// delete, line 111
								sliceDel();
								break;
							
							case 4: 
								// (, line 113
								// call RV, line 113
								if (!r_RV())
								{
									cursor = limit - v3;
									goto lab3Brk;
								}
								// <-, line 113
								sliceFrom("i");
								break;
							}
					}
					while (false);

lab3Brk: ;
					
					break;
				
				case 7: 
					// (, line 119
					// call R2, line 120
					if (!r_R2())
					{
						return false;
					}
					// delete, line 120
					sliceDel();
					// try, line 121
					v5 = limit - cursor;
					do 
					{
						// (, line 121
						// [, line 122
						ket = cursor;
						// substring, line 122
						MyAmongVar = findAmongB(a2, 3);
						if (MyAmongVar == 0)
						{
							cursor = limit - v5;
							goto lab6Brk;
						}
						// ], line 122
						bra = cursor;
						switch (MyAmongVar)
						{
							
							case 0: 
								cursor = limit - v5;
								goto lab6Brk;
							
							case 1: 
								// (, line 123
								// or, line 123

								do 
								{
									v6 = limit - cursor;
									do 
									{
										// (, line 123
										// call R2, line 123
										if (!r_R2())
										{
											goto lab8Brk;
										}
										// delete, line 123
										sliceDel();
										goto lab7Brk;
									}
									while (false);

lab8Brk: ;
									
									cursor = limit - v6;
									// <-, line 123
									sliceFrom("abl");
								}
								while (false);

lab7Brk: ;
								break;
							
							case 2: 
								// (, line 124
								// or, line 124
								do 
								{
									v7 = limit - cursor;
									do 
									{
										// (, line 124
										// call R2, line 124
										if (!r_R2())
										{
											goto lab10Brk;
										}
										// delete, line 124
										sliceDel();
										goto lab9Brk;
									}
									while (false);

lab10Brk: ;
									
									cursor = limit - v7;
									// <-, line 124
									sliceFrom("iqU");
								}
								while (false);

lab9Brk: ;

								break;
							
							case 3: 
								// (, line 125
								// call R2, line 125
								if (!r_R2())
								{
									cursor = limit - v5;
									goto lab6Brk;
								}
								// delete, line 125
								sliceDel();
								break;
							}
					}
					while (false);

lab6Brk: ;
					
					break;
				
				case 8: 
					// (, line 131
					// call R2, line 132
					if (!r_R2())
					{
						return false;
					}
					// delete, line 132
					sliceDel();
					// try, line 133
					v8 = limit - cursor;
					do 
					{
						// (, line 133
						// [, line 133
						ket = cursor;
						// literal, line 133
						if (!(eqSB(2, "at")))
						{
							cursor = limit - v8;
							goto lab11Brk;
						}
						// ], line 133
						bra = cursor;
						// call R2, line 133
						if (!r_R2())
						{
							cursor = limit - v8;
							goto lab11Brk;
						}
						// delete, line 133
						sliceDel();
						// [, line 133
						ket = cursor;
						// literal, line 133
						if (!(eqSB(2, "ic")))
						{
							cursor = limit - v8;
							goto lab11Brk;
						}
						// ], line 133
						bra = cursor;
						// or, line 133
						do 
						{
							v9 = limit - cursor;
							do 
							{
								// (, line 133
								// call R2, line 133
								if (!r_R2())
								{
									goto lab13Brk;
								}
								// delete, line 133
								sliceDel();
								goto lab12Brk;
							}
							while (false);

lab13Brk: ;
							
							cursor = limit - v9;
							// <-, line 133
							sliceFrom("iqU");
						}
						while (false);

lab12Brk: ;
						
					}
					while (false);

lab11Brk: ;
					
					break;
				
				case 9: 
					// (, line 135
					// <-, line 135
					sliceFrom("eau");
					break;
				
				case 10: 
					// (, line 136
					// call R1, line 136
					if (!r_R1())
					{
						return false;
					}
					// <-, line 136
					sliceFrom("al");
					break;
				
				case 11: 
					// (, line 138
					// or, line 138
					do 
					{
						v10 = limit - cursor;
						do 
						{
							// (, line 138
							// call R2, line 138
							if (!r_R2())
							{
								goto lab15Brk;
							}
							// delete, line 138
							sliceDel();
							goto lab14Brk;
						}
						while (false);

lab15Brk: ;
						
						cursor = limit - v10;
						// (, line 138
						// call R1, line 138
						if (!r_R1())
						{
							return false;
						}
						// <-, line 138
						sliceFrom("eux");
					}
					while (false);

lab14Brk: ;
					
					break;
				
				case 12: 
					// (, line 141
					// call R1, line 141
					if (!r_R1())
					{
						return false;
					}
					if (!(outGroupingB(gV, 97, 251)))
					{
						return false;
					}
					// delete, line 141
					sliceDel();
					break;
				
				case 13: 
					// (, line 146
					// call RV, line 146
					if (!r_RV())
					{
						return false;
					}
					// fail, line 146
					// (, line 146
					// <-, line 146
					sliceFrom("ant");
					return false;
				
				case 14: 
					// (, line 147
					// call RV, line 147
					if (!r_RV())
					{
						return false;
					}
					// fail, line 147
					// (, line 147
					// <-, line 147
					sliceFrom("ent");
					return false;
				
				case 15: 
					// (, line 149
					// test, line 149
					v11 = limit - cursor;
					// (, line 149
					if (!(inGroupingB(gV, 97, 251)))
					{
						return false;
					}
					// call RV, line 149
					if (!r_RV())
					{
						return false;
					}
					cursor = limit - v11;
					// fail, line 149
					// (, line 149
					// delete, line 149
					sliceDel();
					return false;
				}
			return true;
		}
		
		private bool rIVerbSuffix()
		{
			int MyAmongVar;
			int v1;
			int v2;
			// setlimit, line 154
			v1 = limit - cursor;
			// tomark, line 154
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 154
			// [, line 155
			ket = cursor;
			// substring, line 155
			MyAmongVar = findAmongB(a4, 35);
			if (MyAmongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 155
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					limitBackward = v2;
					return false;
				
				case 1: 
					// (, line 161
					if (!(outGroupingB(gV, 97, 251)))
					{
						limitBackward = v2;
						return false;
					}
					// delete, line 161
					sliceDel();
					break;
				}
			limitBackward = v2;
			return true;
		}
		
		private bool rVerbSuffix()
		{
			int MyAmongVar;
			int v1;
			int v2;
			int v3;
			// setlimit, line 165
			v1 = limit - cursor;
			// tomark, line 165
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v2 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v1;
			// (, line 165
			// [, line 166
			ket = cursor;
			// substring, line 166
			MyAmongVar = findAmongB(a5, 38);
			if (MyAmongVar == 0)
			{
				limitBackward = v2;
				return false;
			}
			// ], line 166
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					limitBackward = v2;
					return false;
				
				case 1: 
					// (, line 168
					// call R2, line 168
					if (!r_R2())
					{
						limitBackward = v2;
						return false;
					}
					// delete, line 168
					sliceDel();
					break;
				
				case 2: 
					// (, line 176
					// delete, line 176
					sliceDel();
					break;
				
				case 3: 
					// (, line 181
					// delete, line 181
					sliceDel();
					// try, line 182
					v3 = limit - cursor;
					do 
					{
						// (, line 182
						// [, line 182
						ket = cursor;
						// literal, line 182
						if (!(eqSB(1, "e")))
						{
							cursor = limit - v3;
							goto lab16Brk;
						}
						// ], line 182
						bra = cursor;
						// delete, line 182
						sliceDel();
					}
					while (false);

lab16Brk: ;
					
					break;
				}
			limitBackward = v2;
			return true;
		}
		
		private bool rResidualSuffix()
		{
			int MyAmongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 189
			// try, line 190
			v1 = limit - cursor;
			do 
			{
				// (, line 190
				// [, line 190
				ket = cursor;
				// literal, line 190
				if (!(eqSB(1, "s")))
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// ], line 190
				bra = cursor;
				// test, line 190
				v2 = limit - cursor;
				if (!(outGroupingB(gKeepWithS, 97, 232)))
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				cursor = limit - v2;
				// delete, line 190
				sliceDel();
			}
			while (false);

lab0Brk: ;
			
			// setlimit, line 191
			v3 = limit - cursor;
			// tomark, line 191
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v4 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v3;
			// (, line 191
			// [, line 192
			ket = cursor;
			// substring, line 192
			MyAmongVar = findAmongB(a6, 7);
			if (MyAmongVar == 0)
			{
				limitBackward = v4;
				return false;
			}
			// ], line 192
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					limitBackward = v4;
					return false;
				
				case 1: 
					// (, line 193
					// call R2, line 193
					if (!r_R2())
					{
						limitBackward = v4;
						return false;
					}
					// or, line 193
					do 
					{
						v5 = limit - cursor;
						do 
						{
							// literal, line 193
							if (!(eqSB(1, "s")))
							{
								goto lab2Brk;
							}
							goto lab1Brk;
						}
						while (false);

lab2Brk: ;
						
						cursor = limit - v5;
						// literal, line 193
						if (!(eqSB(1, "t")))
						{
							limitBackward = v4;
							return false;
						}
					}
					while (false);

lab1Brk: ;

					// delete, line 193
					sliceDel();
					break;
				
				case 2: 
					// (, line 195
					// <-, line 195
					sliceFrom("i");
					break;
				
				case 3: 
					// (, line 196
					// delete, line 196
					sliceDel();
					break;
				
				case 4: 
					// (, line 197
					// literal, line 197
					if (!(eqSB(2, "gu")))
					{
						limitBackward = v4;
						return false;
					}
					// delete, line 197
					sliceDel();
					break;
				}
			limitBackward = v4;
			return true;
		}
		
		private bool rUnDouble()
		{
			int v1;
			// (, line 202
			// test, line 203
			v1 = limit - cursor;
			// MyAmong, line 203
			if (findAmongB(a7, 5) == 0)
			{
				return false;
			}
			cursor = limit - v1;
			// [, line 203
			ket = cursor;
			// next, line 203
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 203
			bra = cursor;
			// delete, line 203
			sliceDel();
			return true;
		}
		
		private bool rUnAccent()
		{
			int v3;
			// (, line 206
			// atleast, line 207
			{
				int v1 = 1;
				// atleast, line 207
				while (true)
				{
					do 
					{
						if (!(outGroupingB(gV, 97, 251)))
						{
							goto lab16Brk;
						}
						v1--;
						goto replab1;
					}
					while (false);

lab16Brk: ;
					
					goto replab1Brk;

replab1: ;
				}

replab1Brk: ;
				
				if (v1 > 0)
				{
					return false;
				}
			}
			// [, line 208
			ket = cursor;
			// or, line 208

lab16: 
			do 
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 208
					if (!(eqSB(1, "\u00E9")))
					{
						goto lab16Brk;
					}
					goto lab16Brk;
				}
				while (false);

lab16Brk: ;
				
				cursor = limit - v3;
				// literal, line 208
				if (!(eqSB(1, "\u00E8")))
				{
					return false;
				}
			}
			while (false);
			// ], line 208
			bra = cursor;
			// <-, line 208
			sliceFrom("e");
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
			int v11;
			// (, line 212
			// do, line 214
			v1 = cursor;
			do 
			{
				// call prelude, line 214
				if (!rPrelude())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 215
			v2 = cursor;
			do 
			{
				// call markRegions, line 215
				if (!rMarkRegions())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
			// backwards, line 216
			limitBackward = cursor; cursor = limit;
			// (, line 216
			// do, line 218
			v3 = limit - cursor;

			do 
			{
				// (, line 218
				// or, line 228
				do 
				{
					v4 = limit - cursor;
					do 
					{
						// (, line 219
						// and, line 224
						v5 = limit - cursor;
						// (, line 220
						// or, line 220
						do 
						{
							v6 = limit - cursor;
							do 
							{
								// call standardSuffix, line 220
								if (!rStandardSuffix())
								{
									goto lab6Brk;
								}
								goto lab5Brk;
							}
							while (false);

lab6Brk: ;
							
							cursor = limit - v6;
							do 
							{
								// call iVerbSuffix, line 221
								if (!rIVerbSuffix())
								{
									goto lab7Brk;
								}
								goto lab5Brk;
							}
							while (false);

lab7Brk: ;
							
							cursor = limit - v6;
							// call verbSuffix, line 222
							if (!rVerbSuffix())
							{
								goto lab4Brk;
							}
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v5;
						// try, line 225
						v7 = limit - cursor;
						do 
						{
							// (, line 225
							// [, line 225
							ket = cursor;
							// or, line 225
							do 
							{
								v8 = limit - cursor;
								do 
								{
									// (, line 225
									// literal, line 225
									if (!(eqSB(1, "Y")))
									{
										goto lab10Brk;
									}
									// ], line 225
									bra = cursor;
									// <-, line 225
									sliceFrom("i");
									goto lab9Brk;
								}
								while (false);

lab10Brk: ;
								
								cursor = limit - v8;
								// (, line 226
								// literal, line 226
								if (!(eqSB(1, "\u00E7")))
								{
									cursor = limit - v7;
									goto lab8Brk;
								}
								// ], line 226
								bra = cursor;
								// <-, line 226
								sliceFrom("c");
							}
							while (false);

lab9Brk: ;
							
						}
						while (false);

lab8Brk: ;

						goto lab3Brk;
					}
					while (false);

lab4Brk: ;
					
					cursor = limit - v4;
					// call residualSuffix, line 229
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
			// do, line 234
			v9 = limit - cursor;
			do 
			{
				// call unDouble, line 234
				if (!rUnDouble())
				{
					goto lab11Brk;
				}
			}
			while (false);

lab11Brk: ;
			
			cursor = limit - v9;
			// do, line 235
			v10 = limit - cursor;
			do 
			{
				// call unAccent, line 235
				if (!rUnAccent())
				{
					goto lab12Brk;
				}
			}
			while (false);

lab12Brk: ;
			
			cursor = limit - v10;
			cursor = limitBackward; // do, line 237
			v11 = cursor;
			do 
			{
				// call postlude, line 237
				if (!rPostlude())
				{
					goto lab13Brk;
				}
			}
			while (false);

lab13Brk: ;
			
			cursor = v11;
			return true;
		}
	}
}
