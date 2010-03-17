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
    public class PorterStemmer : SnowballProgram, ISnowballStemmer
	{
		public PorterStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("s", - 1, 3, "", this), new Among("ies", 0, 2, "", this), new Among("sses", 0, 1, "", this), new Among("ss", 0, - 1, "", this)};
			a1 = new Among[]{new Among("", - 1, 3, "", this), new Among("bb", 0, 2, "", this), new Among("dd", 0, 2, "", this), new Among("ff", 0, 2, "", this), new Among("gg", 0, 2, "", this), new Among("bl", 0, 1, "", this), new Among("mm", 0, 2, "", this), new Among("nn", 0, 2, "", this), new Among("pp", 0, 2, "", this), new Among("rr", 0, 2, "", this), new Among("at", 0, 1, "", this), new Among("tt", 0, 2, "", this), new Among("iz", 0, 1, "", this)};
			a2 = new Among[]{new Among("ed", - 1, 2, "", this), new Among("eed", 0, 1, "", this), new Among("ing", - 1, 2, "", this)};
			a3 = new Among[]{new Among("anci", - 1, 3, "", this), new Among("enci", - 1, 2, "", this), new Among("abli", - 1, 4, "", this), new Among("eli", - 1, 6, "", this), new Among("alli", - 1, 9, "", this), new Among("ousli", - 1, 12, "", this), new Among("entli", - 1, 5, "", this), new Among("aliti", - 1, 10, "", this), new Among("biliti", - 1, 14, "", this), new Among("iviti", - 1, 13, "", this), new Among("tional", - 1, 1, "", this), new Among("ational", 10, 8, "", this), new Among("alism", - 1, 10, "", this), new Among("ation", - 1, 8, "", this), new Among("ization", 13, 7, "", this), new Among("izer", - 1, 7, "", this), new Among("ator", - 1, 8, "", this), new Among("iveness", - 1, 13, "", this), new Among("fulness", - 1, 11, "", this), new Among("ousness", - 1, 12, "", this)};
			a4 = new Among[]{new Among("icate", - 1, 2, "", this), new Among("ative", - 1, 3, "", this), new Among("alize", - 1, 1, "", this), new Among("iciti", - 1, 2, "", this), new Among("ical", - 1, 2, "", this), new Among("ful", - 1, 3, "", this), new Among("ness", - 1, 3, "", this)};
			a5 = new Among[]{new Among("ic", - 1, 1, "", this), new Among("ance", - 1, 1, "", this), new Among("ence", - 1, 1, "", this), new Among("able", - 1, 1, "", this), new Among("ible", - 1, 1, "", this), new Among("ate", - 1, 1, "", this), new Among("ive", - 1, 1, "", this), new Among("ize", - 1, 1, "", this), new Among("iti", - 1, 1, "", this), new Among("al", - 1, 1, "", this), new Among("ism", - 1, 1, "", this), new Among("ion", - 1, 2, "", this), new Among("er", - 1, 1, "", this), new Among("ous", - 1, 1, "", this), new Among("ant", - 1, 1, "", this), new Among("ent", - 1, 1, "", this), new Among("ment", 15, 1, "", this), new Among("ement", 16, 1, "", this), new Among("ou", - 1, 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private static readonly char[] gV = new char[]{(char) (17), (char) (65), (char) (16), (char) (1)};
		private static readonly char[] g_v_WXY = new char[]{(char) (1), (char) (17), (char) (65), (char) (208), (char) (1)};
		
		private bool B_Y_found;
		private int I_p2;
		private int I_p1;
		
		protected internal virtual void  copyFrom(PorterStemmer other)
		{
			B_Y_found = other.B_Y_found;
			I_p2 = other.I_p2;
			I_p1 = other.I_p1;
			base.copyFrom(other);
		}
		
		private bool rShortv()
		{
			// (, line 19
			if (!(outGroupingB(g_v_WXY, 89, 121)))
			{
				return false;
			}
			if (!(inGroupingB(gV, 97, 121)))
			{
				return false;
			}
			if (!(outGroupingB(gV, 97, 121)))
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
		
		private bool r_Step1a()
		{
			int amongVar;
			// (, line 24
			// [, line 25
			ket = cursor;
			// substring, line 25
			amongVar = findAmongB(a0, 4);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 25
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 26
					// <-, line 26
					sliceFrom("ss");
					break;
				
				case 2: 
					// (, line 27
					// <-, line 27
					sliceFrom("i");
					break;
				
				case 3: 
					// (, line 29
					// delete, line 29
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step1b()
		{
			int amongVar;
			int v1;
			int v3;
			int v4;
			// (, line 33
			// [, line 34
			ket = cursor;
			// substring, line 34
			amongVar = findAmongB(a2, 3);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 34
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 35
					// call R1, line 35
					if (!r_R1())
					{
						return false;
					}
					// <-, line 35
					sliceFrom("ee");
					break;
				
				case 2: 
					// (, line 37
					// test, line 38
					v1 = limit - cursor;
					// gopast, line 38
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
					// delete, line 38
					sliceDel();
					// test, line 39
					v3 = limit - cursor;
					// substring, line 39
					amongVar = findAmongB(a1, 13);
					if (amongVar == 0)
					{
						return false;
					}
					cursor = limit - v3;
					switch (amongVar)
					{
						
						case 0: 
							return false;
						
						case 1: 
							// (, line 41
							// <+, line 41
							{
								int c = cursor;
								insert(cursor, cursor, "e");
								cursor = c;
							}
							break;
						
						case 2: 
							// (, line 44
							// [, line 44
							ket = cursor;
							// next, line 44
							if (cursor <= limitBackward)
							{
								return false;
							}
							cursor--;
							// ], line 44
							bra = cursor;
							// delete, line 44
							sliceDel();
							break;
						
						case 3: 
							// (, line 45
							// atmark, line 45
							if (cursor != I_p1)
							{
								return false;
							}
							// test, line 45
							v4 = limit - cursor;
							// call shortv, line 45
							if (!rShortv())
							{
								return false;
							}
							cursor = limit - v4;
							// <+, line 45
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
			// (, line 51
			// [, line 52
			ket = cursor;
			// or, line 52
			do 
			{
				v1 = limit - cursor;
				do 
				{
					// literal, line 52
					if (!(eqSB(1, "y")))
					{
						goto lab2Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v1;
				// literal, line 52
				if (!(eqSB(1, "Y")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;
			
			// ], line 52
			bra = cursor;
			// gopast, line 53
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
			
			// <-, line 54
			sliceFrom("i");
			return true;
		}
		
		private bool r_Step2()
		{
			int amongVar;
			// (, line 57
			// [, line 58
			ket = cursor;
			// substring, line 58
			amongVar = findAmongB(a3, 20);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 58
			bra = cursor;
			// call R1, line 58
			if (!r_R1())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 59
					// <-, line 59
					sliceFrom("tion");
					break;
				
				case 2: 
					// (, line 60
					// <-, line 60
					sliceFrom("ence");
					break;
				
				case 3: 
					// (, line 61
					// <-, line 61
					sliceFrom("ance");
					break;
				
				case 4: 
					// (, line 62
					// <-, line 62
					sliceFrom("able");
					break;
				
				case 5: 
					// (, line 63
					// <-, line 63
					sliceFrom("ent");
					break;
				
				case 6: 
					// (, line 64
					// <-, line 64
					sliceFrom("e");
					break;
				
				case 7: 
					// (, line 66
					// <-, line 66
					sliceFrom("ize");
					break;
				
				case 8: 
					// (, line 68
					// <-, line 68
					sliceFrom("ate");
					break;
				
				case 9: 
					// (, line 69
					// <-, line 69
					sliceFrom("al");
					break;
				
				case 10: 
					// (, line 71
					// <-, line 71
					sliceFrom("al");
					break;
				
				case 11: 
					// (, line 72
					// <-, line 72
					sliceFrom("ful");
					break;
				
				case 12: 
					// (, line 74
					// <-, line 74
					sliceFrom("ous");
					break;
				
				case 13: 
					// (, line 76
					// <-, line 76
					sliceFrom("ive");
					break;
				
				case 14: 
					// (, line 77
					// <-, line 77
					sliceFrom("ble");
					break;
				}
			return true;
		}
		
		private bool r_Step3()
		{
			int amongVar;
			// (, line 81
			// [, line 82
			ket = cursor;
			// substring, line 82
			amongVar = findAmongB(a4, 7);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 82
			bra = cursor;
			// call R1, line 82
			if (!r_R1())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 83
					// <-, line 83
					sliceFrom("al");
					break;
				
				case 2: 
					// (, line 85
					// <-, line 85
					sliceFrom("ic");
					break;
				
				case 3: 
					// (, line 87
					// delete, line 87
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step4()
		{
			int amongVar;
			int v1;
			// (, line 91
			// [, line 92
			ket = cursor;
			// substring, line 92
			amongVar = findAmongB(a5, 19);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 92
			bra = cursor;
			// call R2, line 92
			if (!r_R2())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 95
					// delete, line 95
					sliceDel();
					break;
				
				case 2: 
					// (, line 96
					// or, line 96

lab2: 
					do 
					{
						v1 = limit - cursor;
						do 
						{
							// literal, line 96
							if (!(eqSB(1, "s")))
							{
								goto lab2Brk;
							}
							goto lab2Brk;
						}
						while (false);

lab2Brk: ;
						
						cursor = limit - v1;
						// literal, line 96
						if (!(eqSB(1, "t")))
						{
							return false;
						}
					}
					while (false);
					// delete, line 96
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool r_Step5a()
		{
			int v1;
			int v2;
			// (, line 100
			// [, line 101
			ket = cursor;
			// literal, line 101
			if (!(eqSB(1, "e")))
			{
				return false;
			}
			// ], line 101
			bra = cursor;
			// or, line 102
			do 
			{
				v1 = limit - cursor;
				do 
				{
					// call R2, line 102
					if (!r_R2())
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v1;
				// (, line 102
				// call R1, line 102
				if (!r_R1())
				{
					return false;
				}
				// not, line 102
				{
					v2 = limit - cursor;
					do 
					{
						// call shortv, line 102
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

			// delete, line 103
			sliceDel();
			return true;
		}
		
		private bool r_Step5b()
		{
			// (, line 106
			// [, line 107
			ket = cursor;
			// literal, line 107
			if (!(eqSB(1, "l")))
			{
				return false;
			}
			// ], line 107
			bra = cursor;
			// call R2, line 108
			if (!r_R2())
			{
				return false;
			}
			// literal, line 108
			if (!(eqSB(1, "l")))
			{
				return false;
			}
			// delete, line 109
			sliceDel();
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			int v10;
			int v11;
			int v12;
			int v13;
			int v14;
			int v15;
			int v16;
			int v17;
			int v18;
			int v19;
			int v20;
			// (, line 113
			// unset Y_found, line 115
			B_Y_found = false;
			// do, line 116
			v1 = cursor;
			do 
			{
				// (, line 116
				// [, line 116
				bra = cursor;
				// literal, line 116
				if (!(eqS(1, "y")))
				{
					goto lab0Brk;
				}
				// ], line 116
				ket = cursor;
				// <-, line 116
				sliceFrom("Y");
				// set Y_found, line 116
				B_Y_found = true;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// do, line 117
			v2 = cursor;
			do 
			{
				// repeat, line 117
				while (true)
				{
					v3 = cursor;
					do 
					{
						// (, line 117
						// goto, line 117
						while (true)
						{
							v4 = cursor;
							do 
							{
								// (, line 117
								if (!(inGrouping(gV, 97, 121)))
								{
									goto lab5Brk;
								}
								// [, line 117
								bra = cursor;
								// literal, line 117
								if (!(eqS(1, "y")))
								{
									goto lab5Brk;
								}
								// ], line 117
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
						
						// <-, line 117
						sliceFrom("Y");
						// set Y_found, line 117
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
			I_p1 = limit;
			I_p2 = limit;
			// do, line 121
			v5 = cursor;
			do 
			{
				// (, line 121
				// gopast, line 122
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
						goto lab6Brk;
					}
					cursor++;
				}

golab7Brk: ;
				
				// gopast, line 122
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
						goto lab6Brk;
					}
					cursor++;
				}

golab9Brk: ;
				
				// setmark p1, line 122
				I_p1 = cursor;
				// gopast, line 123
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 97, 121)))
						{
							goto lab12Brk;
						}
						goto golab11Brk;
					}
					while (false);

lab12Brk: ;
					
					if (cursor >= limit)
					{
						goto lab6Brk;
					}
					cursor++;
				}

golab11Brk: ;
				
				// gopast, line 123
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 97, 121)))
						{
							goto lab14Brk;
						}
						goto golab13Brk;
					}
					while (false);

lab14Brk: ;
					
					if (cursor >= limit)
					{
						goto lab6Brk;
					}
					cursor++;
				}

golab13Brk: ;
				
				// setmark p2, line 123
				I_p2 = cursor;
			}
			while (false);

lab6Brk: ;
			
			cursor = v5;
			// backwards, line 126
			limitBackward = cursor; cursor = limit;
			// (, line 126
			// do, line 127
			v10 = limit - cursor;
			do 
			{
				// call Step1a, line 127
				if (!r_Step1a())
				{
					goto lab15Brk;
				}
			}
			while (false);

lab15Brk: ;
			
			cursor = limit - v10;
			// do, line 128
			v11 = limit - cursor;
			do 
			{
				// call Step1b, line 128
				if (!r_Step1b())
				{
					goto lab16Brk;
				}
			}
			while (false);

lab16Brk: ;
			
			cursor = limit - v11;
			// do, line 129
			v12 = limit - cursor;
			do 
			{
				// call Step1c, line 129
				if (!r_Step1c())
				{
					goto lab17Brk;
				}
			}
			while (false);

lab17Brk: ;
			
			cursor = limit - v12;
			// do, line 130
			v13 = limit - cursor;
			do 
			{
				// call Step2, line 130
				if (!r_Step2())
				{
					goto lab18Brk;
				}
			}
			while (false);

lab18Brk: ;
			
			cursor = limit - v13;
			// do, line 131
			v14 = limit - cursor;
			do 
			{
				// call Step3, line 131
				if (!r_Step3())
				{
					goto lab19Brk;
				}
			}
			while (false);

lab19Brk: ;
			
			cursor = limit - v14;
			// do, line 132
			v15 = limit - cursor;
			do 
			{
				// call Step4, line 132
				if (!r_Step4())
				{
					goto lab20Brk;
				}
			}
			while (false);

lab20Brk: ;
			
			cursor = limit - v15;
			// do, line 133
			v16 = limit - cursor;
			do 
			{
				// call Step5a, line 133
				if (!r_Step5a())
				{
					goto lab21Brk;
				}
			}
			while (false);

lab21Brk: ;
			
			cursor = limit - v16;
			// do, line 134
			v17 = limit - cursor;
			do 
			{
				// call Step5b, line 134
				if (!r_Step5b())
				{
					goto lab22Brk;
				}
			}
			while (false);

lab22Brk: ;
			
			cursor = limit - v17;
			cursor = limitBackward; // do, line 137
			v18 = cursor;
			do 
			{
				// (, line 137
				// Boolean test Y_found, line 137
				if (!(B_Y_found))
				{
					goto lab23Brk;
				}
				// repeat, line 137
				while (true)
				{
					v19 = cursor;
					do 
					{
						// (, line 137
						// goto, line 137
						while (true)
						{
							v20 = cursor;
							do 
							{
								// (, line 137
								// [, line 137
								bra = cursor;
								// literal, line 137
								if (!(eqS(1, "Y")))
								{
									goto lab27Brk;
								}
								// ], line 137
								ket = cursor;
								cursor = v20;
								goto golab26Brk;
							}
							while (false);

lab27Brk: ;
							
							cursor = v20;
							if (cursor >= limit)
							{
								goto lab25Brk;
							}
							cursor++;
						}

golab26Brk: ;
						
						// <-, line 137
						sliceFrom("y");
						goto replab24;
					}
					while (false);

lab25Brk: ;
					
					cursor = v19;
					goto replab24Brk;

replab24: ;
				}

replab24Brk: ;
				
			}
			while (false);

lab23Brk: ;
			
			cursor = v18;
			return true;
		}
	}
}
