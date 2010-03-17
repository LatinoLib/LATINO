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
    public class RussianStemmer : SnowballProgram, ISnowballStemmer
	{
		public RussianStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("\u00D7\u00DB\u00C9", - 1, 1, "", this), new Among("\u00C9\u00D7\u00DB\u00C9", 0, 2, "", this), new Among("\u00D9\u00D7\u00DB\u00C9", 0, 2, "", this), new Among("\u00D7", - 1, 1, "", this), new Among("\u00C9\u00D7", 3, 2, "", this), new Among("\u00D9\u00D7", 3, 2, "", this), new Among("\u00D7\u00DB\u00C9\u00D3\u00D8", - 1, 1, "", this), new Among("\u00C9\u00D7\u00DB\u00C9\u00D3\u00D8", 6, 2, "", this), new Among("\u00D9\u00D7\u00DB\u00C9\u00D3\u00D8", 6, 2, "", this)};
			a1 = new Among[]{new Among("\u00C0\u00C0", - 1, 1, "", this), new Among("\u00C5\u00C0", - 1, 1, "", this), new Among("\u00CF\u00C0", - 1, 1, "", this), new Among("\u00D5\u00C0", - 1, 1, "", this), new Among("\u00C5\u00C5", - 1, 1, "", this), new Among("\u00C9\u00C5", - 1, 1, "", this), new Among("\u00CF\u00C5", - 1, 1, "", this), new Among("\u00D9\u00C5", - 1, 1, "", this), new Among("\u00C9\u00C8", - 1, 1, "", this), new Among("\u00D9\u00C8", - 1, 1, "", this), new Among("\u00C9\u00CD\u00C9", - 1, 1, "", this), new Among("\u00D9\u00CD\u00C9", - 1, 1, "", this), new Among("\u00C5\u00CA", - 1, 1, "", this), new Among("\u00C9\u00CA", - 1, 1, "", this), new Among("\u00CF\u00CA", - 1, 1, "", this), new Among("\u00D9\u00CA", - 1, 1, "", this), new Among("\u00C5\u00CD", - 1, 1, "", this), new Among("\u00C9\u00CD", - 1, 1, "", this), new Among("\u00CF\u00CD", - 1, 1, "", this), new Among("\u00D9\u00CD", - 1, 1, "", this), new Among("\u00C5\u00C7\u00CF", - 1, 1, "", this), new Among("\u00CF\u00C7\u00CF", - 1, 1, "", this), new Among("\u00C1\u00D1", - 1, 1, "", this), new Among("\u00D1\u00D1", - 1, 1, "", this), new Among("\u00C5\u00CD\u00D5", - 1, 1, "", this), new Among("\u00CF\u00CD\u00D5", - 1, 1, "", this)};
			a2 = new Among[]{new Among("\u00C5\u00CD", - 1, 1, "", this), new Among("\u00CE\u00CE", - 1, 1, "", this), new Among("\u00D7\u00DB", - 1, 1, "", this), new Among("\u00C9\u00D7\u00DB", 2, 2, "", this), new Among("\u00D9\u00D7\u00DB", 2, 2, "", this), new Among("\u00DD", - 1, 1, "", this), new Among("\u00C0\u00DD", 5, 1, "", this), new Among("\u00D5\u00C0\u00DD", 6, 2, "", this)};
			a3 = new Among[]{new Among("\u00D3\u00D1", - 1, 1, "", this), new Among("\u00D3\u00D8", - 1, 1, "", this)};
			a4 = new Among[]{new Among("\u00C0", - 1, 2, "", this), new Among("\u00D5\u00C0", 0, 2, "", this), new Among("\u00CC\u00C1", - 1, 1, "", this), new Among("\u00C9\u00CC\u00C1", 2, 2, "", this), new Among("\u00D9\u00CC\u00C1", 2, 2, "", this), new Among("\u00CE\u00C1", - 1, 1, "", this), new Among("\u00C5\u00CE\u00C1", 5, 2, "", this), new Among("\u00C5\u00D4\u00C5", - 1, 1, "", this), new Among("\u00C9\u00D4\u00C5", - 1, 2, "", this), new Among("\u00CA\u00D4\u00C5", - 1, 1, "", this), new Among("\u00C5\u00CA\u00D4\u00C5", 9, 2, "", this), new Among("\u00D5\u00CA\u00D4\u00C5", 9, 2, "", this), new Among("\u00CC\u00C9", - 1, 1, "", this), new Among("\u00C9\u00CC\u00C9", 12, 2, "", this), new Among("\u00D9\u00CC\u00C9", 12, 2, "", this), new Among("\u00CA", - 1, 1, "", this), new Among("\u00C5\u00CA", 15, 2, "", this), new Among("\u00D5\u00CA", 15, 2, "", this), new Among("\u00CC", - 1, 1, "", this), new Among("\u00C9\u00CC", 18, 2, "", this), new Among("\u00D9\u00CC", 18, 2, "", this), new Among("\u00C5\u00CD", - 1, 1, "", this), new Among("\u00C9\u00CD", - 1, 2, "", this), new Among("\u00D9\u00CD", - 1, 2, "", this), new Among("\u00CE", - 1, 1, "", this), new Among("\u00C5\u00CE", 24, 2, "", this), new Among("\u00CC\u00CF", - 1, 1, "", this), new Among("\u00C9\u00CC\u00CF", 26, 2, "", this), new Among("\u00D9\u00CC\u00CF", 26, 2, "", this), new Among("\u00CE\u00CF", - 1, 1, "", this), new Among("\u00C5\u00CE\u00CF", 29, 2, "", this), new Among("\u00CE\u00CE\u00CF", 29, 1, "", this), new Among("\u00C0\u00D4", - 1, 1, "", this), new Among("\u00D5\u00C0\u00D4", 32, 2, "", this), new Among("\u00C5\u00D4", - 1, 1, "", this), new Among("\u00D5\u00C5\u00D4", 34, 2, "", this), new Among("\u00C9\u00D4", - 1, 2, "", this), new Among("\u00D1\u00D4", - 1, 2, "", this), new Among("\u00D9\u00D4", - 1, 2, "", this), new Among("\u00D4\u00D8", - 1, 1, "", this), new Among("\u00C9\u00D4\u00D8", 39, 2, "", this), new Among("\u00D9\u00D4\u00D8", 39, 2, "", this), new Among("\u00C5\u00DB\u00D8", - 1, 1, "", this), 
				new Among("\u00C9\u00DB\u00D8", - 1, 2, "", this), new Among("\u00CE\u00D9", - 1, 1, "", this), new Among("\u00C5\u00CE\u00D9", 44, 2, "", this)};
			a5 = new Among[]{new Among("\u00C0", - 1, 1, "", this), new Among("\u00C9\u00C0", 0, 1, "", this), new Among("\u00D8\u00C0", 0, 1, "", this), new Among("\u00C1", - 1, 1, "", this), new Among("\u00C5", - 1, 1, "", this), new Among("\u00C9\u00C5", 4, 1, "", this), new Among("\u00D8\u00C5", 4, 1, "", this), new Among("\u00C1\u00C8", - 1, 1, "", this), new Among("\u00D1\u00C8", - 1, 1, "", this), new Among("\u00C9\u00D1\u00C8", 8, 1, "", this), new Among("\u00C9", - 1, 1, "", this), new Among("\u00C5\u00C9", 10, 1, "", this), new Among("\u00C9\u00C9", 10, 1, "", this), new Among("\u00C1\u00CD\u00C9", 10, 1, "", this), new Among("\u00D1\u00CD\u00C9", 10, 1, "", this), new Among("\u00C9\u00D1\u00CD\u00C9", 14, 1, "", this), new Among("\u00CA", - 1, 1, "", this), new Among("\u00C5\u00CA", 16, 1, "", this), new Among("\u00C9\u00C5\u00CA", 17, 1, "", this), new Among("\u00C9\u00CA", 16, 1, "", this), new Among("\u00CF\u00CA", 16, 1, "", this), new Among("\u00C1\u00CD", - 1, 1, "", this), new Among("\u00C5\u00CD", - 1, 1, "", this), new Among("\u00C9\u00C5\u00CD", 22, 1, "", this), new Among("\u00CF\u00CD", - 1, 1, "", this), new Among("\u00D1\u00CD", - 1, 1, "", this), new Among("\u00C9\u00D1\u00CD", 25, 1, "", this), new Among("\u00CF", - 1, 1, "", this), new Among("\u00D1", - 1, 1, "", this), new Among("\u00C9\u00D1", 28, 1, "", this), new Among("\u00D8\u00D1", 28, 1, "", this), new Among("\u00D5", - 1, 1, "", this), new Among("\u00C5\u00D7", - 1, 1, "", this), new Among("\u00CF\u00D7", - 1, 1, "", this), new Among("\u00D8", - 1, 1, "", this), new Among("\u00D9", - 1, 1, "", this)};
			a6 = new Among[]{new Among("\u00CF\u00D3\u00D4", - 1, 1, "", this), new Among("\u00CF\u00D3\u00D4\u00D8", - 1, 1, "", this)};
			a7 = new Among[]{new Among("\u00C5\u00CA\u00DB\u00C5", - 1, 1, "", this), new Among("\u00CE", - 1, 2, "", this), new Among("\u00D8", - 1, 3, "", this), new Among("\u00C5\u00CA\u00DB", - 1, 1, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		private Among[] a4;
		private Among[] a5;
		private Among[] a6;
		private Among[] a7;
		private static readonly char[] gV = new char[]{(char) (35), (char) (130), (char) (34), (char) (18)};
		
		private int I_p2;
		private int I_pV;
		
		protected internal virtual void  copyFrom(RussianStemmer other)
		{
			I_p2 = other.I_p2;
			I_pV = other.I_pV;
			base.copyFrom(other);
		}
		
		private bool rMarkRegions()
		{
			int v1;
			// (, line 96
			I_pV = limit;
			I_p2 = limit;
			// do, line 100
			v1 = cursor;
			do 
			{
				// (, line 100
				// gopast, line 101
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 192, 220)))
						{
							goto lab2Brk;
						}
						goto golab1Brk;
					}
					while (false);

lab2Brk: ;
					
					if (cursor >= limit)
					{
						goto lab0Brk;
					}
					cursor++;
				}

golab1Brk: ;
				
				// setmark pV, line 101
				I_pV = cursor;
				// gopast, line 101
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 192, 220)))
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
				
				// gopast, line 102
				while (true)
				{
					do 
					{
						if (!(inGrouping(gV, 192, 220)))
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
				
				// gopast, line 102
				while (true)
				{
					do 
					{
						if (!(outGrouping(gV, 192, 220)))
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
				
				// setmark p2, line 102
				I_p2 = cursor;
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
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
		
		private bool rPerfectiveGerund()
		{
			int amongVar;
			int v1;
			// (, line 110
			// [, line 111
			ket = cursor;
			// substring, line 111
			amongVar = findAmongB(a0, 9);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 111
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 115
					// or, line 115

lab1: 
					do 
					{
						v1 = limit - cursor;
						do 
						{
							// literal, line 115
							if (!(eqSB(1, "\u00C1")))
							{
								goto lab1Brk;
							}
							goto lab1Brk;
						}
						while (false);

lab1Brk: ;
						
						cursor = limit - v1;
						// literal, line 115
						if (!(eqSB(1, "\u00D1")))
						{
							return false;
						}
					}
					while (false);
					// delete, line 115
					sliceDel();
					break;
				
				case 2: 
					// (, line 122
					// delete, line 122
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rAdjective()
		{
			int amongVar;
			// (, line 126
			// [, line 127
			ket = cursor;
			// substring, line 127
			amongVar = findAmongB(a1, 26);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 127
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 136
					// delete, line 136
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rAdjectival()
		{
			int amongVar;
			int v1;
			int v2;
			// (, line 140
			// call adjective, line 141
			if (!rAdjective())
			{
				return false;
			}
			// try, line 148
			v1 = limit - cursor;
			do 
			{
				// (, line 148
				// [, line 149
				ket = cursor;
				// substring, line 149
				amongVar = findAmongB(a2, 8);
				if (amongVar == 0)
				{
					cursor = limit - v1;
					goto lab0Brk;
				}
				// ], line 149
				bra = cursor;
				switch (amongVar)
				{
					
					case 0: 
						cursor = limit - v1;
						goto lab0Brk;
					
					case 1: 
						// (, line 154
						// or, line 154
						do 
						{
							v2 = limit - cursor;
							do 
							{
								// literal, line 154
								if (!(eqSB(1, "\u00C1")))
								{
									goto lab2Brk;
								}
								goto lab1Brk;
							}
							while (false);

lab2Brk: ;
							
							cursor = limit - v2;
							// literal, line 154
							if (!(eqSB(1, "\u00D1")))
							{
								cursor = limit - v1;
								goto lab0Brk;
							}
						}
						while (false);

lab1Brk: ;
						
						// delete, line 154
						sliceDel();
						break;
					
					case 2: 
						// (, line 161
						// delete, line 161
						sliceDel();
						break;
					}
			}
			while (false);

lab0Brk: ;
			
			return true;
		}
		
		private bool rReflexive()
		{
			int amongVar;
			// (, line 167
			// [, line 168
			ket = cursor;
			// substring, line 168
			amongVar = findAmongB(a3, 2);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 168
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 171
					// delete, line 171
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rVerb()
		{
			int amongVar;
			int v1;
			// (, line 175
			// [, line 176
			ket = cursor;
			// substring, line 176
			amongVar = findAmongB(a4, 46);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 176
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 182
					// or, line 182

lab3: 
					do 
					{
						v1 = limit - cursor;
						do 
						{
							// literal, line 182
							if (!(eqSB(1, "\u00C1")))
							{
								goto lab3Brk;
							}
							goto lab3Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = limit - v1;
						// literal, line 182
						if (!(eqSB(1, "\u00D1")))
						{
							return false;
						}
					}
					while (false);
					// delete, line 182
					sliceDel();
					break;
				
				case 2: 
					// (, line 190
					// delete, line 190
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rNoun()
		{
			int amongVar;
			// (, line 198
			// [, line 199
			ket = cursor;
			// substring, line 199
			amongVar = findAmongB(a5, 36);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 199
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 206
					// delete, line 206
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rDerivational()
		{
			int amongVar;
			// (, line 214
			// [, line 215
			ket = cursor;
			// substring, line 215
			amongVar = findAmongB(a6, 2);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 215
			bra = cursor;
			// call R2, line 215
			if (!r_R2())
			{
				return false;
			}
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 218
					// delete, line 218
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rTidyUp()
		{
			int amongVar;
			// (, line 222
			// [, line 223
			ket = cursor;
			// substring, line 223
			amongVar = findAmongB(a7, 4);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 223
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 227
					// delete, line 227
					sliceDel();
					// [, line 228
					ket = cursor;
					// literal, line 228
					if (!(eqSB(1, "\u00CE")))
					{
						return false;
					}
					// ], line 228
					bra = cursor;
					// literal, line 228
					if (!(eqSB(1, "\u00CE")))
					{
						return false;
					}
					// delete, line 228
					sliceDel();
					break;
				
				case 2: 
					// (, line 231
					// literal, line 231
					if (!(eqSB(1, "\u00CE")))
					{
						return false;
					}
					// delete, line 231
					sliceDel();
					break;
				
				case 3: 
					// (, line 233
					// delete, line 233
					sliceDel();
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
			// (, line 238
			// do, line 240
			v1 = cursor;
			do 
			{
				// call markRegions, line 240
				if (!rMarkRegions())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = v1;
			// backwards, line 241
			limitBackward = cursor; cursor = limit;
			// setlimit, line 241
			v2 = limit - cursor;
			// tomark, line 241
			if (cursor < I_pV)
			{
				return false;
			}
			cursor = I_pV;
			v3 = limitBackward;
			limitBackward = cursor;
			cursor = limit - v2;
			// (, line 241
			// do, line 242
			v4 = limit - cursor;
			do 
			{
				// (, line 242
				// or, line 243
				do 
				{
					v5 = limit - cursor;
					do 
					{
						// call perfectiveGerund, line 243
						if (!rPerfectiveGerund())
						{
							goto lab3Brk;
						}
						goto lab3Brk;
					}
					while (false);

lab3Brk: ;
					
					cursor = limit - v5;
					// (, line 244
					// try, line 244
					v6 = limit - cursor;
					do 
					{
						// call reflexive, line 244
						if (!rReflexive())
						{
							cursor = limit - v6;
							goto lab4Brk;
						}
					}
					while (false);

lab4Brk: ;
					
					// or, line 245
					do 
					{
						v7 = limit - cursor;
						do 
						{
							// call adjectival, line 245
							if (!rAdjectival())
							{
								goto lab6Brk;
							}
							goto lab5Brk;
						}
						while (false);

lab6Brk: ;
						
						cursor = limit - v7;
						do 
						{
							// call verb, line 245
							if (!rVerb())
							{
								goto lab7Brk;
							}
							goto lab5Brk;
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v7;
						// call noun, line 245
						if (!rNoun())
						{
							goto lab1Brk;
						}
					}
					while (false);

lab5Brk: ;
					
				}
				while (false);

lab2Brk: ;
				
			}
			while (false);

lab1Brk: ;

			cursor = limit - v4;
			// try, line 248
			v8 = limit - cursor;
			do 
			{
				// (, line 248
				// [, line 248
				ket = cursor;
				// literal, line 248
				if (!(eqSB(1, "\u00C9")))
				{
					cursor = limit - v8;
					goto lab8Brk;
				}
				// ], line 248
				bra = cursor;
				// delete, line 248
				sliceDel();
			}
			while (false);

lab8Brk: ;
			
			// do, line 251
			v9 = limit - cursor;
			do 
			{
				// call derivational, line 251
				if (!rDerivational())
				{
					goto lab9Brk;
				}
			}
			while (false);

lab9Brk: ;
			
			cursor = limit - v9;
			// do, line 252
			v10 = limit - cursor;
			do 
			{
				// call tidyUp, line 252
				if (!rTidyUp())
				{
					goto lab10Brk;
				}
			}
			while (false);

lab10Brk: ;
			
			cursor = limit - v10;
			limitBackward = v3;
			cursor = limitBackward; return true;
		}
	}
}
