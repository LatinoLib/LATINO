﻿/* 
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
    public class EnglishStemmer : SnowballProgram, ISnowballStemmer
    {

		static EnglishStemmer()
		{
			InitBlock();
		}
		private static void InitBlock()
		{
            // TODO: make these static?
			a0 = new Among[]{new Among("gener", - 1, - 1)};
			a1 = new Among[]{new Among("ied", - 1, 2), new Among("s", - 1, 3), new Among("ies", 1, 2), new Among("sses", 1, 1), new Among("ss", 1, - 1), new Among("us", 1, - 1)};
			a2 = new Among[]{new Among("", - 1, 3), new Among("bb", 0, 2), new Among("dd", 0, 2), new Among("ff", 0, 2), new Among("gg", 0, 2), new Among("bl", 0, 1), new Among("mm", 0, 2), new Among("nn", 0, 2), new Among("pp", 0, 2), new Among("rr", 0, 2), new Among("at", 0, 1), new Among("tt", 0, 2), new Among("iz", 0, 1)};
			a3 = new Among[]{new Among("ed", - 1, 2), new Among("eed", 0, 1), new Among("ing", - 1, 2), new Among("edly", - 1, 2), new Among("eedly", 3, 1), new Among("ingly", - 1, 2)};
			a4 = new Among[]{new Among("anci", - 1, 3), new Among("enci", - 1, 2), new Among("ogi", - 1, 13), new Among("li", - 1, 16), new Among("bli", 3, 12), new Among("abli", 4, 4), new Among("alli", 3, 8), new Among("fulli", 3, 14), new Among("lessli", 3, 15), new Among("ousli", 3, 10), new Among("entli", 3, 5), new Among("aliti", - 1, 8), new Among("biliti", - 1, 12), new Among("iviti", - 1, 11), new Among("tional", - 1, 1), new Among("ational", 14, 7), new Among("alism", - 1, 8), new Among("ation", - 1, 7), new Among("ization", 17, 6), new Among("izer", - 1, 6), new Among("ator", - 1, 7), new Among("iveness", - 1, 11), new Among("fulness", - 1, 9), new Among("ousness", - 1, 10)};
			a5 = new Among[]{new Among("icate", - 1, 4), new Among("ative", - 1, 6), new Among("alize", - 1, 3), new Among("iciti", - 1, 4), new Among("ical", - 1, 4), new Among("tional", - 1, 1), new Among("ational", 5, 2), new Among("ful", - 1, 5), new Among("ness", - 1, 5)};
			a6 = new Among[]{new Among("ic", - 1, 1), new Among("ance", - 1, 1), new Among("ence", - 1, 1), new Among("able", - 1, 1), new Among("ible", - 1, 1), new Among("ate", - 1, 1), new Among("ive", - 1, 1), new Among("ize", - 1, 1), new Among("iti", - 1, 1), new Among("al", - 1, 1), new Among("ism", - 1, 1), new Among("ion", - 1, 2), new Among("er", - 1, 1), new Among("ous", - 1, 1), new Among("ant", - 1, 1), new Among("ent", - 1, 1), new Among("ment", 15, 1), new Among("ement", 16, 1)};
			a7 = new Among[]{new Among("e", - 1, 1), new Among("l", - 1, 2)};
			a8 = new Among[]{new Among("succeed", - 1, - 1), new Among("proceed", - 1, - 1), new Among("exceed", - 1, - 1), new Among("canning", - 1, - 1), new Among("inning", - 1, - 1), new Among("earring", - 1, - 1), new Among("herring", - 1, - 1), new Among("outing", - 1, - 1)};
			a9 = new Among[]{new Among("andes", - 1, - 1), new Among("atlas", - 1, - 1), new Among("bias", - 1, - 1), new Among("cosmos", - 1, - 1), new Among("dying", - 1, 3), new Among("early", - 1, 9), new Among("gently", - 1, 7), new Among("howe", - 1, - 1), new Among("idly", - 1, 6), new Among("lying", - 1, 4), new Among("news", - 1, - 1), new Among("only", - 1, 10), new Among("singly", - 1, 11), new Among("skies", - 1, 2), new Among("skis", - 1, 1), new Among("sky", - 1, - 1), new Among("tying", - 1, 5), new Among("ugly", - 1, 8)};
		}
		
		private static Among[] a0;
		private static Among[] a1;
		private static Among[] a2;
		private static Among[] a3;
		private static Among[] a4;
		private static Among[] a5;
		private static Among[] a6;
		private static Among[] a7;
		private static Among[] a8;
		private static Among[] a9;

        private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1)};
		private static readonly char[] g_v_WXY = new char[]{(char) (1), (char) (17), (char) (65), (char) (208), (char) (1)};
		private static readonly char[] g_valid_LI = new char[]{(char) (55), (char) (141), (char) (2)};
		
		private bool B_Y_found;
		private int I_p2;
		private int I_p1;
		
		protected internal virtual void  copyFrom(EnglishStemmer other)
		{
			B_Y_found = other.B_Y_found;
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
			// (, line 23
			// unset Y_found, line 24
			B_Y_found = false;
			// do, line 25
			v1 = cursor;
			do 
			{
				// (, line 25
				// [, line 25
				bra = cursor;
				// literal, line 25
				if (!(eqS(1, "y")))
				{
					goto lab0Brk;
				}
				// ], line 25
				ket = cursor;
				if (!(inGrouping(gV, 97, 121)))
				{
					goto lab0Brk;
				}
				// <-, line 25
				sliceFrom("Y");
				// set Y_found, line 25
				B_Y_found = true;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 26
			v2 = cursor;

			do 
			{
				// repeat, line 26
				while (true)
				{
					v3 = cursor;
					do 
					{
						// (, line 26
						// goto, line 26
						while (true)
						{
							v4 = cursor;
							do 
							{
								// (, line 26
								if (!(inGrouping(gV, 97, 121)))
								{
									goto lab5Brk;
								}
								// [, line 26
								bra = cursor;
								// literal, line 26
								if (!(eqS(1, "y")))
								{
									goto lab5Brk;
								}
								// ], line 26
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

						// <-, line 26
						sliceFrom("Y");
						// set Y_found, line 26
						B_Y_found = true;
						goto replab2;
					}
					while (false);

lab3Brk: ;

					cursor = v3;
					goto replab2Brk;

replab2: ;
				}

replab2Brk: ;
				
			}
			while (false);

lab1Brk: ;

			cursor = v2;
			return true;
		}
		
		private bool rMarkRegions()
		{
			int v1;
			int v2;
			// (, line 29
			I_p1 = limit;
			I_p2 = limit;
			// do, line 32
			v1 = cursor;
			do 
			{
				// (, line 32
				// or, line 36
				do 
				{
					v2 = cursor;
					do 
					{
						// Among, line 33
						if (findAmong(a0, 1) == 0)
						{
							goto lab2Brk;
						}
						goto lab1Brk;
					}
					while (false);

lab2Brk: ;
					
					cursor = v2;
					// (, line 36
					// gopast, line 36
					while (true)
					{
						do 
						{
							if (!(inGrouping(gV, 97, 121)))
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
					
					// gopast, line 36
					while (true)
					{
						do 
						{
							if (!(outGrouping(gV, 97, 121)))
							{
								goto lab6Brk;
							}
							goto golab5Brk;
						}
						while (false);

lab6Brk: ;
						
						if (cursor >= limit)
						{
							goto lab0Brk;
						}
						cursor++;
					}

golab5Brk: ;
					
				}
				while (false);

lab1Brk: ;
				
				// setmark p1, line 37
				I_p1 = cursor;
				// gopast, line 38
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 121)))
						{
							goto lab8Brk;
						}
						goto golab7Brk;
					}
					while (false);

lab8Brk: ;
					
					if (cursor >= limit)
					{
						goto lab0Brk;
					}
					cursor++;
				}

golab7Brk: ;
				
				// gopast, line 38
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 121)))
						{
							goto lab10Brk;
						}
						goto golab9Brk;
					}
					while (false);

lab10Brk: ;
					
					if (cursor >= limit)
					{
						goto lab0Brk;
					}
					cursor++;
				}

golab9Brk: ;
				
				// setmark p2, line 38
				I_p2 = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			return true;
		}
		
		private bool rShortv()
		{
			int v1;
			// (, line 44
			// or, line 46

			do 
			{
				v1 = limit - cursor;
				do 
				{
					// (, line 45
					if (!(outGroupingB(g_v_WXY, 89, 121)))
					{
						goto lab1Brk;
					}
					if (!(inGroupingB(gV, 97, 121)))
					{
						goto lab1Brk;
					}
					if (!(outGroupingB(gV, 97, 121)))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v1;
				// (, line 47
				if (!(outGroupingB(gV, 97, 121)))
				{
					return false;
				}
				if (!(inGroupingB(gV, 97, 121)))
				{
					return false;
				}
				// atlimit, line 47
				if (cursor > limitBackward)
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;

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
		
		private bool r_Step1a()
		{
			int MyAmongVar;
			int v1;
			// (, line 53
			// [, line 54
			ket = cursor;
			// substring, line 54
			MyAmongVar = findAmongB(a1, 6);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 54
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 55
					// <-, line 55
					sliceFrom("ss");
					break;
				
				case 2: 
					// (, line 57
					// or, line 57

					do 
					{
						v1 = limit - cursor;
						do 
						{
							// (, line 57
							// next, line 57
							if (cursor <= limitBackward)
							{
								goto lab1Brk;
							}
							cursor--;
							// atlimit, line 57
							if (cursor > limitBackward)
							{
								goto lab1Brk;
							}
							// <-, line 57
							sliceFrom("ie");
							goto lab0Brk;
						}
						while (false);

lab1Brk: ;

						cursor = limit - v1;
						// <-, line 57
						sliceFrom("i");
					}
					while (false);

lab0Brk: ;

					break;
				
				case 3: 
					// (, line 58
					// next, line 58
					if (cursor <= limitBackward)
					{
						return false;
					}
					cursor--;
					// gopast, line 58
					while (true)
					{
						do 
						{
							if (!(inGroupingB(gV, 97, 121)))
							{
								goto lab3Brk;
							}
							goto golab2Brk;
						}
						while (false);

lab3Brk: ;

						if (cursor <= limitBackward)
						{
							return false;
						}
						cursor--;
					}

golab2Brk: ;

					// delete, line 58
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step1b()
		{
			int MyAmongVar;
			int v1;
			int v3;
			int v4;
			// (, line 63
			// [, line 64
			ket = cursor;
			// substring, line 64
			MyAmongVar = findAmongB(a3, 6);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 64
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 66
					// call R1, line 66
					if (!r_R1())
					{
						return false;
					}
					// <-, line 66
					sliceFrom("ee");
					break;
				
				case 2: 
					// (, line 68
					// test, line 69
					v1 = limit - cursor;
					// gopast, line 69
					while (true)
					{
						do 
						{
							if (!(inGroupingB(gV, 97, 121)))
							{
								goto lab1Brk;
							}
							goto golab0Brk;
						}
						while (false);

lab1Brk: ;

						if (cursor <= limitBackward)
						{
							return false;
						}
						cursor--;
					}

golab0Brk: ;
					
					cursor = limit - v1;
					// delete, line 69
					sliceDel();
					// test, line 70
					v3 = limit - cursor;
					// substring, line 70
					MyAmongVar = findAmongB(a2, 13);
					if (MyAmongVar == 0)
					{
						return false;
					}
					cursor = limit - v3;
					switch (MyAmongVar)
					{
						
						case 0: 
							return false;
						
						case 1: 
							// (, line 72
							// <+, line 72
							{
								int c = cursor;
								insert(cursor, cursor, "e");
								cursor = c;
							}
							break;
						
						case 2: 
							// (, line 75
							// [, line 75
							ket = cursor;
							// next, line 75
							if (cursor <= limitBackward)
							{
								return false;
							}
							cursor--;
							// ], line 75
							bra = cursor;
							// delete, line 75
							sliceDel();
							break;
						
						case 3: 
							// (, line 76
							// atmark, line 76
							if (cursor != I_p1)
							{
								return false;
							}
							// test, line 76
							v4 = limit - cursor;
							// call shortv, line 76
							if (!rShortv())
							{
								return false;
							}
							cursor = limit - v4;
							// <+, line 76
							{
								int c = cursor;
								insert(cursor, cursor, "e");
								cursor = c;
							}
							break;
						}
					break;
				}
			return true;
		}
		
		private bool r_Step1c()
		{
			int v1;
			int v2;
			// (, line 82
			// [, line 83
			ket = cursor;
			// or, line 83

			do 
			{
				v1 = limit - cursor;
				do 
				{
					// literal, line 83
					if (!(eqSB(1, "y")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v1;
				// literal, line 83
				if (!(eqSB(1, "Y")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;

			// ], line 83
			bra = cursor;
			if (!(outGroupingB(gV, 97, 121)))
			{
				return false;
			}
			// not, line 84
			{
				v2 = limit - cursor;
				do 
				{
					// atlimit, line 84
					if (cursor > limitBackward)
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;

				cursor = limit - v2;
			}
			// <-, line 85
			sliceFrom("i");
			return true;
		}
		
		private bool r_Step2()
		{
			int MyAmongVar;
			// (, line 88
			// [, line 89
			ket = cursor;
			// substring, line 89
			MyAmongVar = findAmongB(a4, 24);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 89
			bra = cursor;
			// call R1, line 89
			if (!r_R1())
			{
				return false;
			}
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 90
					// <-, line 90
					sliceFrom("tion");
					break;
				
				case 2: 
					// (, line 91
					// <-, line 91
					sliceFrom("ence");
					break;
				
				case 3: 
					// (, line 92
					// <-, line 92
					sliceFrom("ance");
					break;
				
				case 4: 
					// (, line 93
					// <-, line 93
					sliceFrom("able");
					break;
				
				case 5: 
					// (, line 94
					// <-, line 94
					sliceFrom("ent");
					break;
				
				case 6: 
					// (, line 96
					// <-, line 96
					sliceFrom("ize");
					break;
				
				case 7: 
					// (, line 98
					// <-, line 98
					sliceFrom("ate");
					break;
				
				case 8: 
					// (, line 100
					// <-, line 100
					sliceFrom("al");
					break;
				
				case 9: 
					// (, line 101
					// <-, line 101
					sliceFrom("ful");
					break;
				
				case 10: 
					// (, line 103
					// <-, line 103
					sliceFrom("ous");
					break;
				
				case 11: 
					// (, line 105
					// <-, line 105
					sliceFrom("ive");
					break;
				
				case 12: 
					// (, line 107
					// <-, line 107
					sliceFrom("ble");
					break;
				
				case 13: 
					// (, line 108
					// literal, line 108
					if (!(eqSB(1, "l")))
					{
						return false;
					}
					// <-, line 108
					sliceFrom("og");
					break;
				
				case 14: 
					// (, line 109
					// <-, line 109
					sliceFrom("ful");
					break;
				
				case 15: 
					// (, line 110
					// <-, line 110
					sliceFrom("less");
					break;
				
				case 16: 
					// (, line 111
					if (!(inGroupingB(g_valid_LI, 99, 116)))
					{
						return false;
					}
					// delete, line 111
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step3()
		{
			int MyAmongVar;
			// (, line 115
			// [, line 116
			ket = cursor;
			// substring, line 116
			MyAmongVar = findAmongB(a5, 9);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 116
			bra = cursor;
			// call R1, line 116
			if (!r_R1())
			{
				return false;
			}
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 117
					// <-, line 117
					sliceFrom("tion");
					break;
				
				case 2: 
					// (, line 118
					// <-, line 118
					sliceFrom("ate");
					break;
				
				case 3: 
					// (, line 119
					// <-, line 119
					sliceFrom("al");
					break;
				
				case 4: 
					// (, line 121
					// <-, line 121
					sliceFrom("ic");
					break;
				
				case 5: 
					// (, line 123
					// delete, line 123
					sliceDel();
					break;
				
				case 6: 
					// (, line 125
					// call R2, line 125
					if (!r_R2())
					{
						return false;
					}
					// delete, line 125
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step4()
		{
			int MyAmongVar;
			int v1;
			// (, line 129
			// [, line 130
			ket = cursor;
			// substring, line 130
			MyAmongVar = findAmongB(a6, 18);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 130
			bra = cursor;
			// call R2, line 130
			if (!r_R2())
			{
				return false;
			}
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 133
					// delete, line 133
					sliceDel();
					break;
				
				case 2: 
					// (, line 134
					// or, line 134

					do 
					{
						v1 = limit - cursor;
						do 
						{
							// literal, line 134
							if (!(eqSB(1, "s")))
							{
								goto lab1Brk;
							}
							goto lab0Brk;
						}
						while (false);

lab1Brk: ;

						cursor = limit - v1;
						// literal, line 134
						if (!(eqSB(1, "t")))
						{
							return false;
						}
					}
					while (false);

lab0Brk: ;

					// delete, line 134
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step5()
		{
			int MyAmongVar;
			int v1;
			int v2;
			// (, line 138
			// [, line 139
			ket = cursor;
			// substring, line 139
			MyAmongVar = findAmongB(a7, 2);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 139
			bra = cursor;
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 140
					// or, line 140

					do 
					{
						v1 = limit - cursor;
						do 
						{
							// call R2, line 140
							if (!r_R2())
							{
								goto lab1Brk;
							}
							goto lab0Brk;
						}
						while (false);

lab1Brk: ;
						
						cursor = limit - v1;
						// (, line 140
						// call R1, line 140
						if (!r_R1())
						{
							return false;
						}
						// not, line 140
						{
							v2 = limit - cursor;
							do 
							{
								// call shortv, line 140
								if (!rShortv())
								{
									goto lab2Brk;
								}
								return false;
							}
							while (false);

lab2Brk: ;
							
							cursor = limit - v2;
						}
					}
					while (false);
lab0Brk: ;
					// delete, line 140
					sliceDel();
					break;
				
				case 2: 
					// (, line 141
					// call R2, line 141
					if (!r_R2())
					{
						return false;
					}
					// literal, line 141
					if (!(eqSB(1, "l")))
					{
						return false;
					}
					// delete, line 141
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rException2()
		{
			// (, line 145
			// [, line 147
			ket = cursor;
			// substring, line 147
			if (findAmongB(a8, 8) == 0)
			{
				return false;
			}
			// ], line 147
			bra = cursor;
			// atlimit, line 147
			if (cursor > limitBackward)
			{
				return false;
			}
			return true;
		}
		
		private bool rException1()
		{
			int MyAmongVar;
			// (, line 157
			// [, line 159
			bra = cursor;
			// substring, line 159
			MyAmongVar = findAmong(a9, 18);
			if (MyAmongVar == 0)
			{
				return false;
			}
			// ], line 159
			ket = cursor;
			// atlimit, line 159
			if (cursor < limit)
			{
				return false;
			}
			switch (MyAmongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 163
					// <-, line 163
					sliceFrom("ski");
					break;
				
				case 2: 
					// (, line 164
					// <-, line 164
					sliceFrom("sky");
					break;
				
				case 3: 
					// (, line 165
					// <-, line 165
					sliceFrom("die");
					break;
				
				case 4: 
					// (, line 166
					// <-, line 166
					sliceFrom("lie");
					break;
				
				case 5: 
					// (, line 167
					// <-, line 167
					sliceFrom("tie");
					break;
				
				case 6: 
					// (, line 171
					// <-, line 171
					sliceFrom("idl");
					break;
				
				case 7: 
					// (, line 172
					// <-, line 172
					sliceFrom("gentl");
					break;
				
				case 8: 
					// (, line 173
					// <-, line 173
					sliceFrom("ugli");
					break;
				
				case 9: 
					// (, line 174
					// <-, line 174
					sliceFrom("earli");
					break;
				
				case 10: 
					// (, line 175
					// <-, line 175
					sliceFrom("onli");
					break;
				
				case 11: 
					// (, line 176
					// <-, line 176
					sliceFrom("singl");
					break;
				}
			return true;
		}
		
		private bool rPostlude()
		{
			int v1;
			int v2;
			// (, line 192
			// Boolean test Y_found, line 192
			if (!(B_Y_found))
			{
				return false;
			}
			// repeat, line 192
			while (true)
			{
				v1 = cursor;
				do 
				{
					// (, line 192
					// goto, line 192
					while (true)
					{
						v2 = cursor;
						do 
						{
							// (, line 192
							// [, line 192
							bra = cursor;
							// literal, line 192
							if (!(eqS(1, "Y")))
							{
								goto lab3Brk;
							}
							// ], line 192
							ket = cursor;
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
					
					// <-, line 192
					sliceFrom("y");
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
			// (, line 194
			// or, line 196

			do 
			{
				v1 = cursor;
				do 
				{
					// call exception1, line 196
					if (!rException1())
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;

				cursor = v1;
				// (, line 196
				// test, line 198
				v2 = cursor;
				// hop, line 198
				{
					int c = cursor + 3;
					if (0 > c || c > limit)
					{
						return false;
					}
					cursor = c;
				}
				cursor = v2;
				// do, line 199
				v3 = cursor;
				do 
				{
					// call prelude, line 199
					if (!rPrelude())
					{
						goto lab2Brk;
					}
				}
				while (false);

lab2Brk: ;
				
				cursor = v3;
				// do, line 200
				v4 = cursor;
				do 
				{
					// call markRegions, line 200
					if (!rMarkRegions())
					{
						goto lab3Brk;
					}
				}
				while (false);

lab3Brk: ;

				cursor = v4;
				// backwards, line 201
				limitBackward = cursor; cursor = limit;
				// (, line 201
				// do, line 203
				v5 = limit - cursor;
				do 
				{
					// call Step1a, line 203
					if (!r_Step1a())
					{
						goto lab4Brk;
					}
				}
				while (false);

lab4Brk: ;

				cursor = limit - v5;
				// or, line 205

				do 
				{
					v6 = limit - cursor;
					do 
					{
						// call exception2, line 205
						if (!rException2())
						{
							goto lab6Brk;
						}
						goto lab5Brk;
					}
					while (false);

lab6Brk: ;

					cursor = limit - v6;
					// (, line 205
					// do, line 207
					v7 = limit - cursor;
					do 
					{
						// call Step1b, line 207
						if (!r_Step1b())
						{
							goto lab7Brk;
						}
					}
					while (false);

lab7Brk: ;
					
					cursor = limit - v7;
					// do, line 208
					v8 = limit - cursor;
					do 
					{
						// call Step1c, line 208
						if (!r_Step1c())
						{
							goto lab8Brk;
						}
					}
					while (false);

lab8Brk: ;

					cursor = limit - v8;
					// do, line 210
					v9 = limit - cursor;
					do 
					{
						// call Step2, line 210
						if (!r_Step2())
						{
							goto lab9Brk;
						}
					}
					while (false);

lab9Brk: ;
					
					cursor = limit - v9;
					// do, line 211
					v10 = limit - cursor;
					do 
					{
						// call Step3, line 211
						if (!r_Step3())
						{
							goto lab10Brk;
						}
					}
					while (false);

lab10Brk: ;
					
					cursor = limit - v10;
					// do, line 212
					v11 = limit - cursor;
					do 
					{
						// call Step4, line 212
						if (!r_Step4())
						{
							goto lab11Brk;
						}
					}
					while (false);

lab11Brk: ;
					
					cursor = limit - v11;
					// do, line 214
					v12 = limit - cursor;
					do 
					{
						// call Step5, line 214
						if (!r_Step5())
						{
							goto lab12Brk;
						}
					}
					while (false);

lab12Brk: ;
					
					cursor = limit - v12;
				}
				while (false);

lab5Brk: ;

				cursor = limitBackward; // do, line 217
				v13 = cursor;
				do 
				{
					// call postlude, line 217
					if (!rPostlude())
					{
						goto lab13Brk;
					}
				}
				while (false);

lab13Brk: ;
				
				cursor = v13;
			}
			while (false);

lab0Brk: ;

			return true;
		}
	}
}
