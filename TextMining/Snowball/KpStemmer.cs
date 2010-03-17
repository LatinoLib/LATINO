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
    public class KpStemmer : SnowballProgram, ISnowballStemmer
	{
		public KpStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("nde", - 1, 7, "", this), new Among("en", - 1, 6, "", this), new Among("s", - 1, 2, "", this), new Among("'s", 2, 1, "", this), new Among("es", 2, 4, "", this), new Among("ies", 4, 3, "", this), new Among("aus", 2, 5, "", this)};
			a1 = new Among[]{new Among("de", - 1, 5, "", this), new Among("ge", - 1, 2, "", this), new Among("ische", - 1, 4, "", this), new Among("je", - 1, 1, "", this), new Among("lijke", - 1, 3, "", this), new Among("le", - 1, 9, "", this), new Among("ene", - 1, 10, "", this), new Among("re", - 1, 8, "", this), new Among("se", - 1, 7, "", this), new Among("te", - 1, 6, "", this), new Among("ieve", - 1, 11, "", this)};
			a2 = new Among[]{new Among("heid", - 1, 3, "", this), new Among("fie", - 1, 7, "", this), new Among("gie", - 1, 8, "", this), new Among("atie", - 1, 1, "", this), new Among("isme", - 1, 5, "", this), new Among("ing", - 1, 5, "", this), new Among("arij", - 1, 6, "", this), new Among("erij", - 1, 5, "", this), new Among("sel", - 1, 3, "", this), new Among("rder", - 1, 4, "", this), new Among("ster", - 1, 3, "", this), new Among("iteit", - 1, 2, "", this), new Among("dst", - 1, 10, "", this), new Among("tst", - 1, 9, "", this)};
			a3 = new Among[]{new Among("end", - 1, 10, "", this), new Among("atief", - 1, 2, "", this), new Among("erig", - 1, 10, "", this), new Among("achtig", - 1, 9, "", this), new Among("ioneel", - 1, 1, "", this), new Among("baar", - 1, 3, "", this), new Among("laar", - 1, 5, "", this), new Among("naar", - 1, 4, "", this), new Among("raar", - 1, 6, "", this), new Among("eriger", - 1, 10, "", this), new Among("achtiger", - 1, 9, "", this), new Among("lijker", - 1, 8, "", this), new Among("tant", - 1, 7, "", this), new Among("erigst", - 1, 10, "", this), new Among("achtigst", - 1, 9, "", this), new Among("lijkst", - 1, 8, "", this)};
			a4 = new Among[]{new Among("ig", - 1, 1, "", this), new Among("iger", - 1, 1, "", this), new Among("igst", - 1, 1, "", this)};
			a5 = new Among[]{new Among("ft", - 1, 2, "", this), new Among("kt", - 1, 1, "", this), new Among("pt", - 1, 3, "", this)};
			a6 = new Among[]{new Among("bb", - 1, 1, "", this), new Among("cc", - 1, 2, "", this), new Among("dd", - 1, 3, "", this), new Among("ff", - 1, 4, "", this), new Among("gg", - 1, 5, "", this), new Among("hh", - 1, 6, "", this), new Among("jj", - 1, 7, "", this), new Among("kk", - 1, 8, "", this), new Among("ll", - 1, 9, "", this), new Among("mm", - 1, 10, "", this), new Among("nn", - 1, 11, "", this), new Among("pp", - 1, 12, "", this), new Among("qq", - 1, 13, "", this), new Among("rr", - 1, 14, "", this), new Among("ss", - 1, 15, "", this), new Among("tt", - 1, 16, "", this), new Among("v", - 1, 21, "", this), new Among("vv", 16, 17, "", this), new Among("ww", - 1, 18, "", this), new Among("xx", - 1, 19, "", this), new Among("z", - 1, 22, "", this), new Among("zz", 20, 20, "", this)};
			a7 = new Among[]{new Among("d", - 1, 1, "", this), new Among("t", - 1, 2, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private Among[] a6;
		private Among[] a7;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1)};
		private static readonly char[] g_v_WX = new char[]{(char) (17), (char) (65), (char) (208), (char) (1)};
		private static readonly char[] g_AOU = new char[]{(char) (1), (char) (64), (char) (16)};
		private static readonly char[] g_AIOU = new char[]{(char) (1), (char) (65), (char) (16)};
		
		private bool B_GE_removed;
		private bool B_stemmed;
		private bool B_Y_found;
		private int I_p2;
		private int I_p1;
		private int I_x;
		private System.Text.StringBuilder S_ch = new System.Text.StringBuilder();
		
		protected internal virtual void  copyFrom(KpStemmer other)
		{
			B_GE_removed = other.B_GE_removed;
			B_stemmed = other.B_stemmed;
			B_Y_found = other.B_Y_found;
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			I_x = other.I_x;
			S_ch = other.S_ch;
			base.copyFrom(other);
		}
		
		private bool r_R1()
		{
			// (, line 32
			// setmark x, line 32
			I_x = cursor;
			if (!(I_x >= I_p1))
			{
				return false;
			}
			return true;
		}
		
		private bool r_R2()
		{
			// (, line 33
			// setmark x, line 33
			I_x = cursor;
			if (!(I_x >= I_p2))
			{
				return false;
			}
			return true;
		}
		
		private bool r_V()
		{
			int v1;
			int v2;
			// test, line 35
			v1 = limit - cursor;
			// (, line 35
			// or, line 35
			do 
			{
				v2 = limit - cursor;
				do 
				{
					if (!(inGroupingB(gV, 97, 121)))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				// literal, line 35
				if (!(eqSB(2, "ij")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v1;
			return true;
		}
		
		private bool r_VX()
		{
			int v1;
			int v2;
			// test, line 36
			v1 = limit - cursor;
			// (, line 36
			// next, line 36
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// or, line 36

lab2: 
			do 
			{
				v2 = limit - cursor;
				do 
				{
					if (!(inGroupingB(gV, 97, 121)))
					{
						goto lab2Brk;
					}
					goto lab2Brk;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
				// literal, line 36
				if (!(eqSB(2, "ij")))
				{
					return false;
				}
			}
			while (false);
			cursor = limit - v1;
			return true;
		}
		
		private bool r_C()
		{
			int v1;
			int v2;
			// test, line 37
			v1 = limit - cursor;
			// (, line 37
			// not, line 37
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 37
					if (!(eqSB(2, "ij")))
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
			}
			if (!(outGroupingB(gV, 97, 121)))
			{
				return false;
			}
			cursor = limit - v1;
			return true;
		}
		
		private bool r_lengthen_V()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v6;
			int v7;
			int v8;
			// do, line 39
			v1 = limit - cursor;
			do 
			{
				// (, line 39
				if (!(outGroupingB(g_v_WX, 97, 121)))
				{
					goto lab0Brk;
				}
				// [, line 40
				ket = cursor;
				// or, line 40
				do 
				{
					v2 = limit - cursor;
					do 
					{
						// (, line 40
						if (!(inGroupingB(g_AOU, 97, 117)))
						{
							goto lab2Brk;
						}
						// ], line 40
						bra = cursor;
						// test, line 40
						v3 = limit - cursor;
						// (, line 40
						// or, line 40
						do 
						{
							v4 = limit - cursor;
							do 
							{
								if (!(outGroupingB(gV, 97, 121)))
								{
									goto lab4Brk;
								}
								goto lab3Brk;
							}
							while (false);

lab4Brk: ;
							
							cursor = limit - v4;
							// atlimit, line 40
							if (cursor > limitBackward)
							{
								goto lab2Brk;
							}
						}
						while (false);

lab3Brk: ;
						
						cursor = limit - v3;
						goto lab1Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = limit - v2;
					// (, line 41
					// literal, line 41
					if (!(eqSB(1, "e")))
					{
						goto lab0Brk;
					}
					// ], line 41
					bra = cursor;
					// test, line 41
					v5 = limit - cursor;
					// (, line 41
					// or, line 41
					do 
					{
						v6 = limit - cursor;
						do 
						{
							if (!(outGroupingB(gV, 97, 121)))
							{
								goto lab6Brk;
							}
							goto lab5Brk;
						}
						while (false);

lab6Brk: ;
						
						cursor = limit - v6;
						// atlimit, line 41
						if (cursor > limitBackward)
						{
							goto lab0Brk;
						}
					}
					while (false);

lab5Brk: ;
					
					// not, line 42
					{
						v7 = limit - cursor;
						do 
						{
							if (!(inGroupingB(g_AIOU, 97, 117)))
							{
								goto lab7Brk;
							}
							goto lab0Brk;
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v7;
					}
					// not, line 43
					{
						v8 = limit - cursor;
						do 
						{
							// (, line 43
							// next, line 43
							if (cursor <= limitBackward)
							{
								goto lab8Brk;
							}
							cursor--;
							if (!(inGroupingB(g_AIOU, 97, 117)))
							{
								goto lab8Brk;
							}
							if (!(outGroupingB(gV, 97, 121)))
							{
								goto lab8Brk;
							}
							goto lab0Brk;
						}
						while (false);

lab8Brk: ;
						
						cursor = limit - v8;
					}
					cursor = limit - v5;
				}
				while (false);

lab1Brk: ;
				
				// -> ch, line 44
				S_ch = sliceTo(S_ch);
				// <+ ch, line 44
				{
					int c = cursor;
					insert(cursor, cursor, S_ch);
					cursor = c;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v1;
			return true;
		}
		
		private bool r_Step1()
		{
			int amongVar;
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 48
			// [, line 49
			ket = cursor;
			// among, line 49
			amongVar = findAmongB(a0, 7);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 49
			// ], line 49
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 51
					// delete, line 51
					sliceDel();
					break;
				
				case 2: 
					// (, line 52
					// call R1, line 52
					if (!r_R1())
					{
						return false;
					}
					// not, line 52
					{
						v1 = limit - cursor;
						do 
						{
							// (, line 52
							// literal, line 52
							if (!(eqSB(1, "t")))
							{
								goto lab0Brk;
							}
							// call R1, line 52
							if (!r_R1())
							{
								goto lab0Brk;
							}
							return false;
						}
						while (false);

lab0Brk: ;
						
						cursor = limit - v1;
					}
					// call C, line 52
					if (!r_C())
					{
						return false;
					}
					// delete, line 52
					sliceDel();
					break;
				
				case 3: 
					// (, line 53
					// call R1, line 53
					if (!r_R1())
					{
						return false;
					}
					// <-, line 53
					sliceFrom("ie");
					break;
				
				case 4: 
					// (, line 55
					// or, line 55
					do 
					{
						v2 = limit - cursor;
						do 
						{
							// (, line 55
							// literal, line 55
							if (!(eqSB(2, "ar")))
							{
								goto lab2Brk;
							}
							// call R1, line 55
							if (!r_R1())
							{
								goto lab2Brk;
							}
							// call C, line 55
							if (!r_C())
							{
								goto lab2Brk;
							}
							// ], line 55
							bra = cursor;
							// delete, line 55
							sliceDel();
							// call lengthen_V, line 55
							if (!r_lengthen_V())
							{
								goto lab2Brk;
							}
							goto lab1Brk;
						}
						while (false);

lab2Brk: ;
						
						cursor = limit - v2;
						do 
						{
							// (, line 56
							// literal, line 56
							if (!(eqSB(2, "er")))
							{
								goto lab3Brk;
							}
							// call R1, line 56
							if (!r_R1())
							{
								goto lab3Brk;
							}
							// call C, line 56
							if (!r_C())
							{
								goto lab3Brk;
							}
							// ], line 56
							bra = cursor;
							// delete, line 56
							sliceDel();
							goto lab1Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = limit - v2;
						// (, line 57
						// call R1, line 57
						if (!r_R1())
						{
							return false;
						}
						// call C, line 57
						if (!r_C())
						{
							return false;
						}
						// <-, line 57
						sliceFrom("e");
					}
					while (false);

lab1Brk: ;

					break;
				
				case 5: 
					// (, line 59
					// call R1, line 59
					if (!r_R1())
					{
						return false;
					}
					// call V, line 59
					if (!r_V())
					{
						return false;
					}
					// <-, line 59
					sliceFrom("au");
					break;
				
				case 6: 
					// (, line 60
					// or, line 60
					do 
					{
						v3 = limit - cursor;
						do 
						{
							// (, line 60
							// literal, line 60
							if (!(eqSB(3, "hed")))
							{
								goto lab5Brk;
							}
							// call R1, line 60
							if (!r_R1())
							{
								goto lab5Brk;
							}
							// ], line 60
							bra = cursor;
							// <-, line 60
							sliceFrom("heid");
							goto lab4Brk;
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v3;
						do 
						{
							// (, line 61
							// literal, line 61
							if (!(eqSB(2, "nd")))
							{
								goto lab6Brk;
							}
							// delete, line 61
							sliceDel();
							goto lab4Brk;
						}
						while (false);

lab6Brk: ;
						
						cursor = limit - v3;
						do 
						{
							// (, line 62
							// literal, line 62
							if (!(eqSB(1, "d")))
							{
								goto lab7Brk;
							}
							// call R1, line 62
							if (!r_R1())
							{
								goto lab7Brk;
							}
							// call C, line 62
							if (!r_C())
							{
								goto lab7Brk;
							}
							// ], line 62
							bra = cursor;
							// delete, line 62
							sliceDel();
							goto lab4Brk;
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v3;
						do 
						{
							// (, line 63
							// or, line 63
							do 
							{
								v4 = limit - cursor;
								do 
								{
									// literal, line 63
									if (!(eqSB(1, "i")))
									{
										goto lab10Brk;
									}
									goto lab9Brk;
								}
								while (false);

lab10Brk: ;
								
								cursor = limit - v4;
								// literal, line 63
								if (!(eqSB(1, "j")))
								{
									goto lab8Brk;
								}
							}
							while (false);

lab9Brk: ;
							
							// call V, line 63
							if (!r_V())
							{
								goto lab8Brk;
							}
							// delete, line 63
							sliceDel();
							goto lab4Brk;
						}
						while (false);

lab8Brk: ;
						
						cursor = limit - v3;
						// (, line 64
						// call R1, line 64
						if (!r_R1())
						{
							return false;
						}
						// call C, line 64
						if (!r_C())
						{
							return false;
						}
						// delete, line 64
						sliceDel();
						// call lengthen_V, line 64
						if (!r_lengthen_V())
						{
							return false;
						}
					}
					while (false);

lab4Brk: ;

					break;
				
				case 7: 
					// (, line 65
					// <-, line 65
					sliceFrom("nd");
					break;
				}
			return true;
		}
		
		private bool r_Step2()
		{
			int amongVar;
			int v1;
			// (, line 70
			// [, line 71
			ket = cursor;
			// among, line 71
			amongVar = findAmongB(a1, 11);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 71
			// ], line 71
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 72
					// or, line 72
					do 
					{
						v1 = limit - cursor;
						do 
						{
							// (, line 72
							// literal, line 72
							if (!(eqSB(2, "'t")))
							{
								goto lab1Brk;
							}
							// ], line 72
							bra = cursor;
							// delete, line 72
							sliceDel();
							goto lab0Brk;
						}
						while (false);

lab1Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 73
							// literal, line 73
							if (!(eqSB(2, "et")))
							{
								goto lab2Brk;
							}
							// ], line 73
							bra = cursor;
							// call R1, line 73
							if (!r_R1())
							{
								goto lab2Brk;
							}
							// call C, line 73
							if (!r_C())
							{
								goto lab2Brk;
							}
							// delete, line 73
							sliceDel();
							goto lab0Brk;
						}
						while (false);

lab2Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 74
							// literal, line 74
							if (!(eqSB(3, "rnt")))
							{
								goto lab3Brk;
							}
							// ], line 74
							bra = cursor;
							// <-, line 74
							sliceFrom("rn");
							goto lab0Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 75
							// literal, line 75
							if (!(eqSB(1, "t")))
							{
								goto lab4Brk;
							}
							// ], line 75
							bra = cursor;
							// call R1, line 75
							if (!r_R1())
							{
								goto lab4Brk;
							}
							// call VX, line 75
							if (!r_VX())
							{
								goto lab4Brk;
							}
							// delete, line 75
							sliceDel();
							goto lab0Brk;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 76
							// literal, line 76
							if (!(eqSB(3, "ink")))
							{
								goto lab5Brk;
							}
							// ], line 76
							bra = cursor;
							// <-, line 76
							sliceFrom("ing");
							goto lab0Brk;
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 77
							// literal, line 77
							if (!(eqSB(2, "mp")))
							{
								goto lab6Brk;
							}
							// ], line 77
							bra = cursor;
							// <-, line 77
							sliceFrom("m");
							goto lab0Brk;
						}
						while (false);

lab6Brk: ;
						
						cursor = limit - v1;
						do 
						{
							// (, line 78
							// literal, line 78
							if (!(eqSB(1, "'")))
							{
								goto lab7Brk;
							}
							// ], line 78
							bra = cursor;
							// call R1, line 78
							if (!r_R1())
							{
								goto lab7Brk;
							}
							// delete, line 78
							sliceDel();
							goto lab0Brk;
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v1;
						// (, line 79
						// ], line 79
						bra = cursor;
						// call R1, line 79
						if (!r_R1())
						{
							return false;
						}
						// call C, line 79
						if (!r_C())
						{
							return false;
						}
						// delete, line 79
						sliceDel();
					}
					while (false);

lab0Brk: ;

					break;
				
				case 2: 
					// (, line 80
					// call R1, line 80
					if (!r_R1())
					{
						return false;
					}
					// <-, line 80
					sliceFrom("g");
					break;
				
				case 3: 
					// (, line 81
					// call R1, line 81
					if (!r_R1())
					{
						return false;
					}
					// <-, line 81
					sliceFrom("lijk");
					break;
				
				case 4: 
					// (, line 82
					// call R1, line 82
					if (!r_R1())
					{
						return false;
					}
					// <-, line 82
					sliceFrom("isch");
					break;
				
				case 5: 
					// (, line 83
					// call R1, line 83
					if (!r_R1())
					{
						return false;
					}
					// call C, line 83
					if (!r_C())
					{
						return false;
					}
					// delete, line 83
					sliceDel();
					break;
				
				case 6: 
					// (, line 84
					// call R1, line 84
					if (!r_R1())
					{
						return false;
					}
					// <-, line 84
					sliceFrom("t");
					break;
				
				case 7: 
					// (, line 85
					// call R1, line 85
					if (!r_R1())
					{
						return false;
					}
					// <-, line 85
					sliceFrom("s");
					break;
				
				case 8: 
					// (, line 86
					// call R1, line 86
					if (!r_R1())
					{
						return false;
					}
					// <-, line 86
					sliceFrom("r");
					break;
				
				case 9: 
					// (, line 87
					// call R1, line 87
					if (!r_R1())
					{
						return false;
					}
					// delete, line 87
					sliceDel();
					// attach, line 87
					insert(cursor, cursor, "l");
					// call lengthen_V, line 87
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 10: 
					// (, line 88
					// call R1, line 88
					if (!r_R1())
					{
						return false;
					}
					// call C, line 88
					if (!r_C())
					{
						return false;
					}
					// delete, line 88
					sliceDel();
					// attach, line 88
					insert(cursor, cursor, "en");
					// call lengthen_V, line 88
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 11: 
					// (, line 89
					// call R1, line 89
					if (!r_R1())
					{
						return false;
					}
					// call C, line 89
					if (!r_C())
					{
						return false;
					}
					// <-, line 89
					sliceFrom("ief");
					break;
				}
			return true;
		}
		
		private bool r_Step3()
		{
			int amongVar;
			// (, line 94
			// [, line 95
			ket = cursor;
			// among, line 95
			amongVar = findAmongB(a2, 14);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 95
			// ], line 95
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 96
					// call R1, line 96
					if (!r_R1())
					{
						return false;
					}
					// <-, line 96
					sliceFrom("eer");
					break;
				
				case 2: 
					// (, line 97
					// call R1, line 97
					if (!r_R1())
					{
						return false;
					}
					// delete, line 97
					sliceDel();
					// call lengthen_V, line 97
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 3: 
					// (, line 100
					// call R1, line 100
					if (!r_R1())
					{
						return false;
					}
					// delete, line 100
					sliceDel();
					break;
				
				case 4: 
					// (, line 101
					// <-, line 101
					sliceFrom("r");
					break;
				
				case 5: 
					// (, line 104
					// call R1, line 104
					if (!r_R1())
					{
						return false;
					}
					// delete, line 104
					sliceDel();
					// call lengthen_V, line 104
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 6: 
					// (, line 105
					// call R1, line 105
					if (!r_R1())
					{
						return false;
					}
					// call C, line 105
					if (!r_C())
					{
						return false;
					}
					// <-, line 105
					sliceFrom("aar");
					break;
				
				case 7: 
					// (, line 106
					// call R2, line 106
					if (!r_R2())
					{
						return false;
					}
					// delete, line 106
					sliceDel();
					// attach, line 106
					insert(cursor, cursor, "f");
					// call lengthen_V, line 106
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 8: 
					// (, line 107
					// call R2, line 107
					if (!r_R2())
					{
						return false;
					}
					// delete, line 107
					sliceDel();
					// attach, line 107
					insert(cursor, cursor, "g");
					// call lengthen_V, line 107
					if (!r_lengthen_V())
					{
						return false;
					}
					break;
				
				case 9: 
					// (, line 108
					// call R1, line 108
					if (!r_R1())
					{
						return false;
					}
					// call C, line 108
					if (!r_C())
					{
						return false;
					}
					// <-, line 108
					sliceFrom("t");
					break;
				
				case 10: 
					// (, line 109
					// call R1, line 109
					if (!r_R1())
					{
						return false;
					}
					// call C, line 109
					if (!r_C())
					{
						return false;
					}
					// <-, line 109
					sliceFrom("d");
					break;
				}
			return true;
		}
		
		private bool r_Step4()
		{
			int amongVar;
			int v1;
			// (, line 114
			// or, line 134

lab11: 
			do 
			{
				v1 = limit - cursor;
				do 
				{
					// (, line 115
					// [, line 115
					ket = cursor;
					// among, line 115
					amongVar = findAmongB(a3, 16);
					if (amongVar == 0)
					{
						goto lab11Brk;
					}
					// (, line 115
					// ], line 115
					bra = cursor;
					switch (amongVar)
					{
						
						case 0: 
							goto lab11Brk;
						
						case 1: 
							// (, line 116
							// call R1, line 116
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// <-, line 116
							sliceFrom("ie");
							break;
						
						case 2: 
							// (, line 117
							// call R1, line 117
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// <-, line 117
							sliceFrom("eer");
							break;
						
						case 3: 
							// (, line 118
							// call R1, line 118
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// delete, line 118
							sliceDel();
							break;
						
						case 4: 
							// (, line 119
							// call R1, line 119
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// call V, line 119
							if (!r_V())
							{
								goto lab11Brk;
							}
							// <-, line 119
							sliceFrom("n");
							break;
						
						case 5: 
							// (, line 120
							// call R1, line 120
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// call V, line 120
							if (!r_V())
							{
								goto lab11Brk;
							}
							// <-, line 120
							sliceFrom("l");
							break;
						
						case 6: 
							// (, line 121
							// call R1, line 121
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// call V, line 121
							if (!r_V())
							{
								goto lab11Brk;
							}
							// <-, line 121
							sliceFrom("r");
							break;
						
						case 7: 
							// (, line 122
							// call R1, line 122
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// <-, line 122
							sliceFrom("teer");
							break;
						
						case 8: 
							// (, line 124
							// call R1, line 124
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// <-, line 124
							sliceFrom("lijk");
							break;
						
						case 9: 
							// (, line 127
							// call R1, line 127
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// delete, line 127
							sliceDel();
							break;
						
						case 10: 
							// (, line 131
							// call R1, line 131
							if (!r_R1())
							{
								goto lab11Brk;
							}
							// call C, line 131
							if (!r_C())
							{
								goto lab11Brk;
							}
							// delete, line 131
							sliceDel();
							// call lengthen_V, line 131
							if (!r_lengthen_V())
							{
								goto lab11Brk;
							}
							break;
						}
					goto lab11Brk;
				}
				while (false);

lab11Brk: ;
				
				cursor = limit - v1;
				// (, line 135
				// [, line 135
				ket = cursor;
				// among, line 135
				amongVar = findAmongB(a4, 3);
				if (amongVar == 0)
				{
					return false;
				}
				// (, line 135
				// ], line 135
				bra = cursor;
				switch (amongVar)
				{
					
					case 0: 
						return false;
					
					case 1: 
						// (, line 138
						// call R1, line 138
						if (!r_R1())
						{
							return false;
						}
						// call C, line 138
						if (!r_C())
						{
							return false;
						}
						// delete, line 138
						sliceDel();
						// call lengthen_V, line 138
						if (!r_lengthen_V())
						{
							return false;
						}
						break;
					}
			}
			while (false);
			return true;
		}
		
		private bool r_Step7()
		{
			int amongVar;
			// (, line 144
			// [, line 145
			ket = cursor;
			// among, line 145
			amongVar = findAmongB(a5, 3);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 145
			// ], line 145
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 146
					// <-, line 146
					sliceFrom("k");
					break;
				
				case 2: 
					// (, line 147
					// <-, line 147
					sliceFrom("f");
					break;
				
				case 3: 
					// (, line 148
					// <-, line 148
					sliceFrom("p");
					break;
				}
			return true;
		}
		
		private bool r_Step6()
		{
			int amongVar;
			// (, line 153
			// [, line 154
			ket = cursor;
			// among, line 154
			amongVar = findAmongB(a6, 22);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 154
			// ], line 154
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 155
					// <-, line 155
					sliceFrom("b");
					break;
				
				case 2: 
					// (, line 156
					// <-, line 156
					sliceFrom("c");
					break;
				
				case 3: 
					// (, line 157
					// <-, line 157
					sliceFrom("d");
					break;
				
				case 4: 
					// (, line 158
					// <-, line 158
					sliceFrom("f");
					break;
				
				case 5: 
					// (, line 159
					// <-, line 159
					sliceFrom("g");
					break;
				
				case 6: 
					// (, line 160
					// <-, line 160
					sliceFrom("h");
					break;
				
				case 7: 
					// (, line 161
					// <-, line 161
					sliceFrom("j");
					break;
				
				case 8: 
					// (, line 162
					// <-, line 162
					sliceFrom("k");
					break;
				
				case 9: 
					// (, line 163
					// <-, line 163
					sliceFrom("l");
					break;
				
				case 10: 
					// (, line 164
					// <-, line 164
					sliceFrom("m");
					break;
				
				case 11: 
					// (, line 165
					// <-, line 165
					sliceFrom("n");
					break;
				
				case 12: 
					// (, line 166
					// <-, line 166
					sliceFrom("p");
					break;
				
				case 13: 
					// (, line 167
					// <-, line 167
					sliceFrom("q");
					break;
				
				case 14: 
					// (, line 168
					// <-, line 168
					sliceFrom("r");
					break;
				
				case 15: 
					// (, line 169
					// <-, line 169
					sliceFrom("s");
					break;
				
				case 16: 
					// (, line 170
					// <-, line 170
					sliceFrom("t");
					break;
				
				case 17: 
					// (, line 171
					// <-, line 171
					sliceFrom("v");
					break;
				
				case 18: 
					// (, line 172
					// <-, line 172
					sliceFrom("w");
					break;
				
				case 19: 
					// (, line 173
					// <-, line 173
					sliceFrom("x");
					break;
				
				case 20: 
					// (, line 174
					// <-, line 174
					sliceFrom("z");
					break;
				
				case 21: 
					// (, line 175
					// <-, line 175
					sliceFrom("f");
					break;
				
				case 22: 
					// (, line 176
					// <-, line 176
					sliceFrom("s");
					break;
				}
			return true;
		}
		
		private bool r_Step1c()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 181
			// [, line 182
			ket = cursor;
			// among, line 182
			amongVar = findAmongB(a7, 2);
			if (amongVar == 0)
			{
				return false;
			}
			// (, line 182
			// ], line 182
			bra = cursor;
			// call R1, line 182
			if (!r_R1())
			{
				return false;
			}
			// call C, line 182
			if (!r_C())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 183
					// not, line 183
					{
						v1 = limit - cursor;
						do 
						{
							// (, line 183
							// literal, line 183
							if (!(eqSB(1, "n")))
							{
								goto lab11Brk;
							}
							// call R1, line 183
							if (!r_R1())
							{
								goto lab11Brk;
							}
							return false;
						}
						while (false);

lab11Brk: ;
						
						cursor = limit - v1;
					}
					// delete, line 183
					sliceDel();
					break;
				
				case 2: 
					// (, line 184
					// not, line 184
					{
						v2 = limit - cursor;
						do 
						{
							// (, line 184
							// literal, line 184
							if (!(eqSB(1, "h")))
							{
								goto lab11Brk;
							}
							// call R1, line 184
							if (!r_R1())
							{
								goto lab11Brk;
							}
							return false;
						}
						while (false);

lab11Brk: ;
						
						cursor = limit - v2;
					}
					// delete, line 184
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_LosePrefix()
		{
			int v1;
			int v2;
			int v3;
			// (, line 189
			// [, line 190
			bra = cursor;
			// literal, line 190
			if (!(eqS(2, "ge")))
			{
				return false;
			}
			// ], line 190
			ket = cursor;
			// test, line 190
			v1 = cursor;
			// hop, line 190
			{
				int c = cursor + 3;
				if (0 > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = v1;
			// (, line 190
			// goto, line 190
			while (true)
			{
				v2 = cursor;
				do 
				{
					if (!(inGrouping(gV, 97, 121)))
					{
						goto lab11Brk;
					}
					cursor = v2;
					goto golab0Brk;
				}
				while (false);

lab11Brk: ;
				
				cursor = v2;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab0Brk: ;
			
			// goto, line 190
			while (true)
			{
				v3 = cursor;
				do 
				{
					if (!(outGrouping(gV, 97, 121)))
					{
						goto lab11Brk;
					}
					cursor = v3;
					goto golab2Brk;
				}
				while (false);

lab11Brk: ;
				
				cursor = v3;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab2Brk: ;
			
			// set GE_removed, line 191
			B_GE_removed = true;
			// delete, line 192
			sliceDel();
			return true;
		}
		
		private bool r_LoseInfix()
		{
			int v2;
			int v3;
			int v4;
			// (, line 195
			// next, line 196
			if (cursor >= limit)
			{
				return false;
			}
			cursor++;
			// gopast, line 197
			while (true)
			{
				do 
				{
					// (, line 197
					// [, line 197
					bra = cursor;
					// literal, line 197
					if (!(eqS(2, "ge")))
					{
						goto lab11Brk;
					}
					// ], line 197
					ket = cursor;
					goto golab1Brk;
				}
				while (false);

lab11Brk: ;
				
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab1Brk: ;
			
			// test, line 197
			v2 = cursor;
			// hop, line 197
			{
				int c = cursor + 3;
				if (0 > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = v2;
			// (, line 197
			// goto, line 197
			while (true)
			{
				v3 = cursor;
				do 
				{
					if (!(inGrouping(gV, 97, 121)))
					{
						goto lab11Brk;
					}
					cursor = v3;
					goto golab3Brk;
				}
				while (false);

lab11Brk: ;
				
				cursor = v3;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab3Brk: ;
			
			// goto, line 197
			while (true)
			{
				v4 = cursor;
				do 
				{
					if (!(outGrouping(gV, 97, 121)))
					{
						goto lab11Brk;
					}
					cursor = v4;
					goto golab4Brk;
				}
				while (false);

lab11Brk: ;
				
				cursor = v4;
				if (cursor >= limit)
				{
					return false;
				}
				cursor++;
			}

golab4Brk: ;
			
			// set GE_removed, line 198
			B_GE_removed = true;
			// delete, line 199
			sliceDel();
			return true;
		}
		
		private bool rMeasure()
		{
			int v1;
			int v2;
			int v5;
			int v6;
			int v9;
			int v10;
			// (, line 202
			// do, line 203
			v1 = cursor;
			do 
			{
				// (, line 203
				// tolimit, line 204
				cursor = limit;
				// setmark p1, line 205
				I_p1 = cursor;
				// setmark p2, line 206
				I_p2 = cursor;
			}
			while (false);

lab0Brk: ;

			cursor = v1;
			// do, line 208
			v2 = cursor;
			do 
			{
				// (, line 208
				// repeat, line 209
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 121)))
						{
							goto lab3Brk;
						}
						goto replab2;
					}
					while (false);

lab3Brk: ;
					
					goto replab2Brk;

replab2: ;
				}

replab2Brk: ;
				
				// atleast, line 209
				{
					int v4 = 1;
					// atleast, line 209
					while (true)
					{
						v5 = cursor;
						do 
						{
							// (, line 209
							// or, line 209
							do 
							{
								v6 = cursor;
								do 
								{
									// literal, line 209
									if (!(eqS(2, "ij")))
									{
										goto lab7Brk;
									}
									goto lab6Brk;
								}
								while (false);

lab7Brk: ;
								
								cursor = v6;
								if (!(inGrouping(gV, 97, 121)))
								{
									goto lab5Brk;
								}
							}
							while (false);

lab6Brk: ;
							
							v4--;
							goto replab4;
						}
						while (false);

lab5Brk: ;

						cursor = v5;
						goto replab4Brk;

replab4: ;
					}

replab4Brk: ;
					
					if (v4 > 0)
					{
						goto lab1Brk;
					}
				}
				if (!(outGrouping(gV, 97, 121)))
				{
					goto lab1Brk;
				}
				// setmark p1, line 209
				I_p1 = cursor;
				// repeat, line 210
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 121)))
						{
							goto lab9Brk;
						}
						goto replab8;
					}
					while (false);

lab9Brk: ;
					
					goto replab8Brk;

replab8: ;
				}

replab8Brk: ;
				
				// atleast, line 210
				{
					int v8 = 1;
					// atleast, line 210
					while (true)
					{
						v9 = cursor;
						do 
						{
							// (, line 210
							// or, line 210
							do 
							{
								v10 = cursor;
								do 
								{
									// literal, line 210
									if (!(eqS(2, "ij")))
									{
										goto lab13Brk;
									}
									goto lab12Brk;
								}
								while (false);

lab13Brk: ;
								
								cursor = v10;
								if (!(inGrouping(gV, 97, 121)))
								{
									goto lab11Brk;
								}
							}
							while (false);

lab12Brk: ;
							
							v8--;
							goto replab10;
						}
						while (false);

lab11Brk: ;
						
						cursor = v9;
						goto replab10Brk;

replab10: ;
					}

replab10Brk: ;
					
					if (v8 > 0)
					{
						goto lab1Brk;
					}
				}
				if (!(outGrouping(gV, 97, 121)))
				{
					goto lab1Brk;
				}
				// setmark p2, line 210
				I_p2 = cursor;
			}
			while (false);

lab1Brk: ;
			
			cursor = v2;
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
			int v12;
			int v13;
			int v14;
			int v15;
			int v16;
			int v18;
			int v19;
			int v20;
			// (, line 214
			// unset Y_found, line 216
			B_Y_found = false;
			// unset stemmed, line 217
			B_stemmed = false;
			// do, line 218
			v1 = cursor;
			do 
			{
				// (, line 218
				// [, line 218
				bra = cursor;
				// literal, line 218
				if (!(eqS(1, "y")))
				{
					goto lab0Brk;
				}
				// ], line 218
				ket = cursor;
				// <-, line 218
				sliceFrom("Y");
				// set Y_found, line 218
				B_Y_found = true;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 219
			v2 = cursor;
			do 
			{
				// repeat, line 219
				while (true)
				{
					v3 = cursor;
					do 
					{
						// (, line 219
						// goto, line 219
						while (true)
						{
							v4 = cursor;
							do 
							{
								// (, line 219
								if (!(inGrouping(gV, 97, 121)))
								{
									goto lab5Brk;
								}
								// [, line 219
								bra = cursor;
								// literal, line 219
								if (!(eqS(1, "y")))
								{
									goto lab5Brk;
								}
								// ], line 219
								ket = cursor;
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
						
						// <-, line 219
						sliceFrom("Y");
						// set Y_found, line 219
						B_Y_found = true;
						goto replab3;
					}
					while (false);

lab3Brk: ;
					
					cursor = v3;
					goto replab3Brk;

replab3: ;
				}

replab3Brk: ;
				
			}
			while (false);

lab1Brk: ;

			cursor = v2;
			// call measure, line 221
			if (!rMeasure())
			{
				return false;
			}
			// backwards, line 223
			limitBackward = cursor; cursor = limit;
			// (, line 223
			// do, line 224
			v5 = limit - cursor;
			do 
			{
				// (, line 224
				// call Step1, line 224
				if (!r_Step1())
				{
					goto lab6Brk;
				}
				// set stemmed, line 224
				B_stemmed = true;
			}
			while (false);

lab6Brk: ;
			
			cursor = limit - v5;
			// do, line 225
			v6 = limit - cursor;
			do 
			{
				// (, line 225
				// call Step2, line 225
				if (!r_Step2())
				{
					goto lab7Brk;
				}
				// set stemmed, line 225
				B_stemmed = true;
			}
			while (false);

lab7Brk: ;
			
			cursor = limit - v6;
			// do, line 226
			v7 = limit - cursor;
			do 
			{
				// (, line 226
				// call Step3, line 226
				if (!r_Step3())
				{
					goto lab8Brk;
				}
				// set stemmed, line 226
				B_stemmed = true;
			}
			while (false);

lab8Brk: ;
			
			cursor = limit - v7;
			// do, line 227
			v8 = limit - cursor;
			do 
			{
				// (, line 227
				// call Step4, line 227
				if (!r_Step4())
				{
					goto lab9Brk;
				}
				// set stemmed, line 227
				B_stemmed = true;
			}
			while (false);

lab9Brk: ;
			
			cursor = limit - v8;
			cursor = limitBackward; // unset GE_removed, line 229
			B_GE_removed = false;
			// do, line 230
			v9 = cursor;
			do 
			{
				// (, line 230
				// and, line 230
				v10 = cursor;
				// call LosePrefix, line 230
				if (!r_LosePrefix())
				{
					goto lab10Brk;
				}
				cursor = v10;
				// call measure, line 230
				if (!rMeasure())
				{
					goto lab10Brk;
				}
			}
			while (false);

lab10Brk: ;
			
			cursor = v9;
			// backwards, line 231
			limitBackward = cursor; cursor = limit;
			// (, line 231
			// do, line 232
			v11 = limit - cursor;
			do 
			{
				// (, line 232
				// Boolean test GE_removed, line 232
				if (!(B_GE_removed))
				{
					goto lab11Brk;
				}
				// call Step1c, line 232
				if (!r_Step1c())
				{
					goto lab11Brk;
				}
			}
			while (false);

lab11Brk: ;
			
			cursor = limit - v11;
			cursor = limitBackward; // unset GE_removed, line 234
			B_GE_removed = false;
			// do, line 235
			v12 = cursor;
			do 
			{
				// (, line 235
				// and, line 235
				v13 = cursor;
				// call LoseInfix, line 235
				if (!r_LoseInfix())
				{
					goto lab12Brk;
				}
				cursor = v13;
				// call measure, line 235
				if (!rMeasure())
				{
					goto lab12Brk;
				}
			}
			while (false);

lab12Brk: ;
			
			cursor = v12;
			// backwards, line 236
			limitBackward = cursor; cursor = limit;
			// (, line 236
			// do, line 237
			v14 = limit - cursor;
			do 
			{
				// (, line 237
				// Boolean test GE_removed, line 237
				if (!(B_GE_removed))
				{
					goto lab13Brk;
				}
				// call Step1c, line 237
				if (!r_Step1c())
				{
					goto lab13Brk;
				}
			}
			while (false);

lab13Brk: ;
			
			cursor = limit - v14;
			cursor = limitBackward; // backwards, line 239
			limitBackward = cursor; cursor = limit;
			// (, line 239
			// do, line 240
			v15 = limit - cursor;
			do 
			{
				// (, line 240
				// call Step7, line 240
				if (!r_Step7())
				{
					goto lab14Brk;
				}
				// set stemmed, line 240
				B_stemmed = true;
			}
			while (false);

lab14Brk: ;
			
			cursor = limit - v15;
			// do, line 241
			v16 = limit - cursor;
			do 
			{
				// (, line 241
				// or, line 241
				do 
				{
					do 
					{
						// Boolean test stemmed, line 241
						if (!(B_stemmed))
						{
							goto lab17Brk;
						}
						goto lab16Brk;
					}
					while (false);

lab17Brk: ;
					
					// Boolean test GE_removed, line 241
					if (!(B_GE_removed))
					{
						goto lab15Brk;
					}
				}
				while (false);

lab16Brk: ;
				
				// call Step6, line 241
				if (!r_Step6())
				{
					goto lab15Brk;
				}
			}
			while (false);

lab15Brk: ;
			
			cursor = limit - v16;
			cursor = limitBackward; // do, line 243
			v18 = cursor;
			do 
			{
				// (, line 243
				// Boolean test Y_found, line 243
				if (!(B_Y_found))
				{
					goto lab18Brk;
				}
				// repeat, line 243
				while (true)
				{
					v19 = cursor;
					do 
					{
						// (, line 243
						// goto, line 243
						while (true)
						{
							v20 = cursor;
							do 
							{
								// (, line 243
								// [, line 243
								bra = cursor;
								// literal, line 243
								if (!(eqS(1, "Y")))
								{
									goto lab22Brk;
								}
								// ], line 243
								ket = cursor;
								cursor = v20;
								goto golab21Brk;
							}
							while (false);

lab22Brk: ;
							
							cursor = v20;
							if (cursor >= limit)
							{
								goto lab20Brk;
							}
							cursor++;
						}

golab21Brk: ;
						
						// <-, line 243
						sliceFrom("y");
						goto replab19;
					}
					while (false);

lab20Brk: ;
					
					cursor = v19;
					goto replab19Brk;

replab19: ;
				}

replab19Brk: ;
				
			}
			while (false);

lab18Brk: ;
			
			cursor = v18;
			return true;
		}
	}
}