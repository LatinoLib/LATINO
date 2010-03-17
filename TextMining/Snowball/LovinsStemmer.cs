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
    public class LovinsStemmer : SnowballProgram, ISnowballStemmer
	{
		public LovinsStemmer()
		{
			InitBlock();
		}
		private void  InitBlock()
		{
			a0 = new Among[]{new Among("d", - 1, - 1, "", this), new Among("f", - 1, - 1, "", this), new Among("ph", - 1, - 1, "", this), new Among("th", - 1, - 1, "", this), new Among("l", - 1, - 1, "", this), new Among("er", - 1, - 1, "", this), new Among("or", - 1, - 1, "", this), new Among("es", - 1, - 1, "", this), new Among("t", - 1, - 1, "", this)};
			a1 = new Among[]{new Among("s'", - 1, 1, "r_A", this), new Among("a", - 1, 1, "r_A", this), new Among("ia", 1, 1, "r_A", this), new Among("ata", 1, 1, "r_A", this), new Among("ic", - 1, 1, "r_A", this), new Among("aic", 4, 1, "r_A", this), new Among("allic", 4, 1, "r_BB", this), new Among("aric", 4, 1, "r_A", this), new Among("atic", 4, 1, "r_B", this), new Among("itic", 4, 1, "r_H", this), new Among("antic", 4, 1, "r_C", this), new Among("istic", 4, 1, "r_A", this), new Among("alistic", 11, 1, "r_B", this), new Among("aristic", 11, 1, "r_A", this), new Among("ivistic", 11, 1, "r_A", this), new Among("ed", - 1, 1, "r_E", this), new Among("anced", 15, 1, "r_B", this), new Among("enced", 15, 1, "r_A", this), new Among("ished", 15, 1, "r_A", this), new Among("ied", 15, 1, "r_A", this), new Among("ened", 15, 1, "r_E", this), new Among("ioned", 15, 1, "r_A", this), new Among("ated", 15, 1, "r_I", this), new Among("ented", 15, 1, "r_C", this), new Among("ized", 15, 1, "r_F", this), new Among("arized", 24, 1, "r_A", this), new Among("oid", - 1, 1, "r_A", this), new Among("aroid", 26, 1, "r_A", this), new Among("hood", - 1, 1, "r_A", this), new Among("ehood", 28, 1, "r_A", this), new Among("ihood", 28, 1, "r_A", this), new Among("elihood", 30, 1, "r_E", this), new Among("ward", - 1, 1, "r_A", this), new Among("e", - 1, 1, "r_A", this), new Among("ae", 33, 1, "r_A", this), new Among("ance", 33, 1, "r_B", this), new Among("icance", 35, 1, "r_A", this), new Among("ence", 33, 1, "r_A", this), new Among("ide", 33, 1, "r_L", this), new Among("icide", 38, 1, "r_A", this), new Among("otide", 38, 1, "r_A", this), new Among("age", 33, 1, "r_B", this), new Among("able", 33, 1, "r_A", this), new Among("atable", 42, 1, "r_A", this), new Among("izable", 42, 1, "r_E", this), new Among("arizable", 44, 1, "r_A", this), new Among("ible", 33, 1, "r_A", this), new Among("encible", 46, 1, "r_A", this), new Among("ene", 33, 1, "r_E", this), new Among("ine", 33, 1, "r_M", this), new Among("idine", 49, 1, "r_I", this), new 
				Among("one", 33, 1, "r_R", this), new Among("ature", 33, 1, "r_E", this), new Among("eature", 52, 1, "r_Z", this), new Among("ese", 33, 1, "r_A", this), new Among("wise", 33, 1, "r_A", this), new Among("ate", 33, 1, "r_A", this), new Among("entiate", 56, 1, "r_A", this), new Among("inate", 56, 1, "r_A", this), new Among("ionate", 56, 1, "r_D", this), new Among("ite", 33, 1, "r_AA", this), new Among("ive", 33, 1, "r_A", this), new Among("ative", 61, 1, "r_A", this), new Among("ize", 33, 1, "r_F", this), new Among("alize", 63, 1, "r_A", this), new Among("icalize", 64, 1, "r_A", this), new Among("ialize", 64, 1, "r_A", this), new Among("entialize", 66, 1, "r_A", this), new Among("ionalize", 64, 1, "r_A", this), new Among("arize", 63, 1, "r_A", this), new Among("ing", - 1, 1, "r_N", this), new Among("ancing", 70, 1, "r_B", this), new Among("encing", 70, 1, "r_A", this), new Among("aging", 70, 1, "r_B", this), new Among("ening", 70, 1, "r_E", this), new Among("ioning", 70, 1, "r_A", this), new Among("ating", 70, 1, "r_I", this), new Among("enting", 70, 1, "r_C", this), new Among("ying", 70, 1, "r_B", this), new Among("izing", 70, 1, "r_F", this), new Among("arizing", 79, 1, "r_A", this), new Among("ish", - 1, 1, "r_C", this), new Among("yish", 81, 1, "r_A", this), new Among("i", - 1, 1, "r_A", this), new Among("al", - 1, 1, "r_BB", this), new Among("ical", 84, 1, "r_A", this), new Among("aical", 85, 1, "r_A", this), new Among("istical", 85, 1, "r_A", this), new Among("oidal", 84, 1, "r_A", this), new Among("eal", 84, 1, "r_Y", this), new Among("ial", 84, 1, "r_A", this), new Among("ancial", 90, 1, "r_A", this), new Among("arial", 90, 1, "r_A", this), new Among("ential", 90, 1, "r_A", this), new Among("ional", 84, 1, "r_A", this), new Among("ational", 94, 1, "r_B", this), new Among("izational", 95, 1, "r_A", this), new Among("ental", 84, 1, "r_A", this), new Among("ful", - 1, 1, "r_A", this), new Among("eful", 98, 1, "r_A", this), new Among("iful", 98, 1, "r_A", this), new Among("yl", - 1, 1, 
				"r_R", this), new Among("ism", - 1, 1, "r_B", this), new Among("icism", 102, 1, "r_A", this), new Among("oidism", 102, 1, "r_A", this), new Among("alism", 102, 1, "r_B", this), new Among("icalism", 105, 1, "r_A", this), new Among("ionalism", 105, 1, "r_A", this), new Among("inism", 102, 1, "r_J", this), new Among("ativism", 102, 1, "r_A", this), new Among("um", - 1, 1, "r_U", this), new Among("ium", 110, 1, "r_A", this), new Among("ian", - 1, 1, "r_A", this), new Among("ician", 112, 1, "r_A", this), new Among("en", - 1, 1, "r_F", this), new Among("ogen", 114, 1, "r_A", this), new Among("on", - 1, 1, "r_S", this), new Among("ion", 116, 1, "r_Q", this), new Among("ation", 117, 1, "r_B", this), new Among("ication", 118, 1, "r_G", this), new Among("entiation", 118, 1, "r_A", this), new Among("ination", 118, 1, "r_A", this), new Among("isation", 118, 1, "r_A", this), new Among("arisation", 122, 1, "r_A", this), new Among("entation", 118, 1, "r_A", this), new Among("ization", 118, 1, "r_F", this), new Among("arization", 125, 1, "r_A", this), new Among("action", 117, 1, "r_G", this), new Among("o", - 1, 1, "r_A", this), new Among("ar", - 1, 1, "r_X", this), new Among("ear", 129, 1, "r_Y", this), new Among("ier", - 1, 1, "r_A", this), new Among("ariser", - 1, 1, "r_A", this), new Among("izer", - 1, 1, "r_F", this), new Among("arizer", 133, 1, "r_A", this), new Among("or", - 1, 1, "r_T", this), new Among("ator", 135, 1, "r_A", this), new Among("s", - 1, 1, "r_W", this), new Among("'s", 137, 1, "r_A", this), new Among("as", 137, 1, "r_B", this), new Among("ics", 137, 1, "r_A", this), new Among("istics", 140, 1, "r_A", this), new Among("es", 137, 1, "r_E", this), new Among("ances", 142, 1, "r_B", this), new Among("ences", 142, 1, "r_A", this), new Among("ides", 142, 1, "r_L", this), new Among("oides", 145, 1, "r_A", this), new Among("ages", 142, 1, "r_B", this), new Among("ies", 142, 1, "r_P", this), new Among("acies", 148, 1, "r_A", this), new Among("ancies", 148, 1, "r_A", this), new Among("encies", 
				148, 1, "r_A", this), new Among("aries", 148, 1, "r_A", this), new Among("ities", 148, 1, "r_A", this), new Among("alities", 153, 1, "r_A", this), new Among("ivities", 153, 1, "r_A", this), new Among("ines", 142, 1, "r_M", this), new Among("nesses", 142, 1, "r_A", this), new Among("ates", 142, 1, "r_A", this), new Among("atives", 142, 1, "r_A", this), new Among("ings", 137, 1, "r_N", this), new Among("is", 137, 1, "r_A", this), new Among("als", 137, 1, "r_BB", this), new Among("ials", 162, 1, "r_A", this), new Among("entials", 163, 1, "r_A", this), new Among("ionals", 162, 1, "r_A", this), new Among("isms", 137, 1, "r_B", this), new Among("ians", 137, 1, "r_A", this), new Among("icians", 167, 1, "r_A", this), new Among("ions", 137, 1, "r_B", this), new Among("ations", 169, 1, "r_B", this), new Among("arisations", 170, 1, "r_A", this), new Among("entations", 170, 1, "r_A", this), new Among("izations", 170, 1, "r_A", this), new Among("arizations", 173, 1, "r_A", this), new Among("ars", 137, 1, "r_O", this), new Among("iers", 137, 1, "r_A", this), new Among("izers", 137, 1, "r_F", this), new Among("ators", 137, 1, "r_A", this), new Among("less", 137, 1, "r_A", this), new Among("eless", 179, 1, "r_A", this), new Among("ness", 137, 1, "r_A", this), new Among("eness", 181, 1, "r_E", this), new Among("ableness", 182, 1, "r_A", this), new Among("eableness", 183, 1, "r_E", this), new Among("ibleness", 182, 1, "r_A", this), new Among("ateness", 182, 1, "r_A", this), new Among("iteness", 182, 1, "r_A", this), new Among("iveness", 182, 1, "r_A", this), new Among("ativeness", 188, 1, "r_A", this), new Among("ingness", 181, 1, "r_A", this), new Among("ishness", 181, 1, "r_A", this), new Among("iness", 181, 1, "r_A", this), new Among("ariness", 192, 1, "r_E", this), new Among("alness", 181, 1, "r_A", this), new Among("icalness", 194, 1, "r_A", this), new Among("antialness", 194, 1, "r_A", this), new Among("entialness", 194, 1, "r_A", this), new Among("ionalness", 194, 1, "r_A", this), new Among("fulness", 
				181, 1, "r_A", this), new Among("lessness", 181, 1, "r_A", this), new Among("ousness", 181, 1, "r_A", this), new Among("eousness", 201, 1, "r_A", this), new Among("iousness", 201, 1, "r_A", this), new Among("itousness", 201, 1, "r_A", this), new Among("entness", 181, 1, "r_A", this), new Among("ants", 137, 1, "r_B", this), new Among("ists", 137, 1, "r_A", this), new Among("icists", 207, 1, "r_A", this), new Among("us", 137, 1, "r_V", this), new Among("ous", 209, 1, "r_A", this), new Among("eous", 210, 1, "r_A", this), new Among("aceous", 211, 1, "r_A", this), new Among("antaneous", 211, 1, "r_A", this), new Among("ious", 210, 1, "r_A", this), new Among("acious", 214, 1, "r_B", this), new Among("itous", 210, 1, "r_A", this), new Among("ant", - 1, 1, "r_B", this), new Among("icant", 217, 1, "r_A", this), new Among("ent", - 1, 1, "r_C", this), new Among("ement", 219, 1, "r_A", this), new Among("izement", 220, 1, "r_A", this), new Among("ist", - 1, 1, "r_A", this), new Among("icist", 222, 1, "r_A", this), new Among("alist", 222, 1, "r_A", this), new Among("icalist", 224, 1, "r_A", this), new Among("ialist", 224, 1, "r_A", this), new Among("ionist", 222, 1, "r_A", this), new Among("entist", 222, 1, "r_A", this), new Among("y", - 1, 1, "r_B", this), new Among("acy", 229, 1, "r_A", this), new Among("ancy", 229, 1, "r_B", this), new Among("ency", 229, 1, "r_A", this), new Among("ly", 229, 1, "r_B", this), new Among("ealy", 233, 1, "r_Y", this), new Among("ably", 233, 1, "r_A", this), new Among("ibly", 233, 1, "r_A", this), new Among("edly", 233, 1, "r_E", this), new Among("iedly", 237, 1, "r_A", this), new Among("ely", 233, 1, "r_E", this), new Among("ately", 239, 1, "r_A", this), new Among("ively", 239, 1, "r_A", this), new Among("atively", 241, 1, "r_A", this), new Among("ingly", 233, 1, "r_B", this), new Among("atingly", 243, 1, "r_A", this), new Among("ily", 233, 1, "r_A", this), new Among("lily", 245, 1, "r_A", this), new Among("arily", 245, 1, "r_A", this), new Among("ally", 233, 1, "r_B", 
				this), new Among("ically", 248, 1, "r_A", this), new Among("aically", 249, 1, "r_A", this), new Among("allically", 249, 1, "r_C", this), new Among("istically", 249, 1, "r_A", this), new Among("alistically", 252, 1, "r_B", this), new Among("oidally", 248, 1, "r_A", this), new Among("ially", 248, 1, "r_A", this), new Among("entially", 255, 1, "r_A", this), new Among("ionally", 248, 1, "r_A", this), new Among("ationally", 257, 1, "r_B", this), new Among("izationally", 258, 1, "r_B", this), new Among("entally", 248, 1, "r_A", this), new Among("fully", 233, 1, "r_A", this), new Among("efully", 261, 1, "r_A", this), new Among("ifully", 261, 1, "r_A", this), new Among("enly", 233, 1, "r_E", this), new Among("arly", 233, 1, "r_K", this), new Among("early", 265, 1, "r_Y", this), new Among("lessly", 233, 1, "r_A", this), new Among("ously", 233, 1, "r_A", this), new Among("eously", 268, 1, "r_A", this), new Among("iously", 268, 1, "r_A", this), new Among("ently", 233, 1, "r_A", this), new Among("ary", 229, 1, "r_F", this), new Among("ery", 229, 1, "r_E", this), new Among("icianry", 229, 1, "r_A", this), new Among("atory", 229, 1, "r_A", this), new Among("ity", 229, 1, "r_A", this), new Among("acity", 276, 1, "r_A", this), new Among("icity", 276, 1, "r_A", this), new Among("eity", 276, 1, "r_A", this), new Among("ality", 276, 1, "r_A", this), new Among("icality", 280, 1, "r_A", this), new Among("iality", 280, 1, "r_A", this), new Among("antiality", 282, 1, "r_A", this), new Among("entiality", 282, 1, "r_A", this), new Among("ionality", 280, 1, "r_A", this), new Among("elity", 276, 1, "r_A", this), new Among("ability", 276, 1, "r_A", this), new Among("izability", 287, 1, "r_A", this), new Among("arizability", 288, 1, "r_A", this), new Among("ibility", 276, 1, "r_A", this), new Among("inity", 276, 1, "r_CC", this), new Among("arity", 276, 1, "r_B", this), new Among("ivity", 276, 1, "r_A", this)};
			a2 = new Among[]{new Among("bb", - 1, - 1, "", this), new Among("dd", - 1, - 1, "", this), new Among("gg", - 1, - 1, "", this), new Among("ll", - 1, - 1, "", this), new Among("mm", - 1, - 1, "", this), new Among("nn", - 1, - 1, "", this), new Among("pp", - 1, - 1, "", this), new Among("rr", - 1, - 1, "", this), new Among("ss", - 1, - 1, "", this), new Among("tt", - 1, - 1, "", this)};
			a3 = new Among[]{new Among("uad", - 1, 18, "", this), new Among("vad", - 1, 19, "", this), new Among("cid", - 1, 20, "", this), new Among("lid", - 1, 21, "", this), new Among("erid", - 1, 22, "", this), new Among("pand", - 1, 23, "", this), new Among("end", - 1, 24, "", this), new Among("ond", - 1, 25, "", this), new Among("lud", - 1, 26, "", this), new Among("rud", - 1, 27, "", this), new Among("ul", - 1, 9, "", this), new Among("her", - 1, 28, "", this), new Among("metr", - 1, 7, "", this), new Among("istr", - 1, 6, "", this), new Among("urs", - 1, 5, "", this), new Among("uct", - 1, 2, "", this), new Among("et", - 1, 32, "", this), new Among("mit", - 1, 29, "", this), new Among("ent", - 1, 30, "", this), new Among("umpt", - 1, 3, "", this), new Among("rpt", - 1, 4, "", this), new Among("ert", - 1, 31, "", this), new Among("yt", - 1, 33, "", this), new Among("iev", - 1, 1, "", this), new Among("olv", - 1, 8, "", this), new Among("ax", - 1, 14, "", this), new Among("ex", - 1, 15, "", this), new Among("bex", 26, 10, "", this), new Among("dex", 26, 11, "", this), new Among("pex", 26, 12, "", this), new Among("tex", 26, 13, "", this), new Among("ix", - 1, 16, "", this), new Among("lux", - 1, 17, "", this), new Among("yz", - 1, 34, "", this)};
		}
		
		private Among[] a0;
		private Among[] a1;
		private Among[] a2;
		private Among[] a3;
		
		protected internal virtual void  copyFrom(LovinsStemmer other)
		{
			base.copyFrom(other);
		}
		
		private bool r_A()
		{
			// (, line 21
			// hop, line 21
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			return true;
		}
		
		private bool r_B()
		{
			// (, line 22
			// hop, line 22
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			return true;
		}
		
		private bool r_C()
		{
			// (, line 23
			// hop, line 23
			{
				int c = cursor - 4;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			return true;
		}
		
		private bool r_D()
		{
			// (, line 24
			// hop, line 24
			{
				int c = cursor - 5;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			return true;
		}
		
		private bool r_E()
		{
			int v1;
			int v2;
			// (, line 25
			// test, line 25
			v1 = limit - cursor;
			// hop, line 25
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 25
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 25
					if (!(eqSB(1, "e")))
					{
						goto lab0Brk;
					}
					return false;
				}
				while (false);

lab0Brk: ;
				
				cursor = limit - v2;
			}
			return true;
		}
		
		private bool r_F()
		{
			int v1;
			int v2;
			// (, line 26
			// test, line 26
			v1 = limit - cursor;
			// hop, line 26
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 26
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 26
					if (!(eqSB(1, "e")))
					{
						goto lab1Brk;
					}
					return false;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
			}
			return true;
		}
		
		private bool r_G()
		{
			int v1;
			// (, line 27
			// test, line 27
			v1 = limit - cursor;
			// hop, line 27
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// literal, line 27
			if (!(eqSB(1, "f")))
			{
				return false;
			}
			return true;
		}
		
		private bool r_H()
		{
			int v1;
			int v2;
			// (, line 28
			// test, line 28
			v1 = limit - cursor;
			// hop, line 28
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 28

lab1: 
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 28
					if (!(eqSB(1, "t")))
					{
						goto lab1Brk;
					}
					goto lab1Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				// literal, line 28
				if (!(eqSB(2, "ll")))
				{
					return false;
				}
			}
			while (false);
			return true;
		}
		
		private bool r_I()
		{
			int v1;
			int v2;
			int v3;
			// (, line 29
			// test, line 29
			v1 = limit - cursor;
			// hop, line 29
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 29
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 29
					if (!(eqSB(1, "o")))
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 29
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 29
					if (!(eqSB(1, "e")))
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v3;
			}
			return true;
		}
		
		private bool r_J()
		{
			int v1;
			int v2;
			int v3;
			// (, line 30
			// test, line 30
			v1 = limit - cursor;
			// hop, line 30
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 30
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 30
					if (!(eqSB(1, "a")))
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 30
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 30
					if (!(eqSB(1, "e")))
					{
						goto lab2Brk;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v3;
			}
			return true;
		}
		
		private bool r_K()
		{
			int v1;
			int v2;
			// (, line 31
			// test, line 31
			v1 = limit - cursor;
			// hop, line 31
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 31
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 31
					if (!(eqSB(1, "l")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				do 
				{
					// literal, line 31
					if (!(eqSB(1, "i")))
					{
						goto lab2Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
				// (, line 31
				// literal, line 31
				if (!(eqSB(1, "e")))
				{
					return false;
				}
				// next, line 31
				if (cursor <= limitBackward)
				{
					return false;
				}
				cursor--;
				// literal, line 31
				if (!(eqSB(1, "u")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_L()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 32
			// test, line 32
			v1 = limit - cursor;
			// hop, line 32
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 32
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 32
					if (!(eqSB(1, "u")))
					{
						goto lab0Brk;
					}
					return false;
				}
				while (false);

lab0Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 32
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 32
					if (!(eqSB(1, "x")))
					{
						goto lab1Brk;
					}
					return false;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v3;
			}
			// not, line 32
			{
				v4 = limit - cursor;
				do 
				{
					// (, line 32
					// literal, line 32
					if (!(eqSB(1, "s")))
					{
						goto lab2Brk;
					}
					// not, line 32
					{
						v5 = limit - cursor;
						do 
						{
							// literal, line 32
							if (!(eqSB(1, "o")))
							{
								goto lab3Brk;
							}
							goto lab2Brk;
						}
						while (false);

lab3Brk: ;
						
						cursor = limit - v5;
					}
					return false;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v4;
			}
			return true;
		}
		
		private bool r_M()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			int v5;
			// (, line 33
			// test, line 33
			v1 = limit - cursor;
			// hop, line 33
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 33
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 33
					if (!(eqSB(1, "a")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 33
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 33
					if (!(eqSB(1, "c")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v3;
			}
			// not, line 33
			{
				v4 = limit - cursor;
				do 
				{
					// literal, line 33
					if (!(eqSB(1, "e")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v4;
			}
			// not, line 33
			{
				v5 = limit - cursor;
				do 
				{
					// literal, line 33
					if (!(eqSB(1, "m")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v5;
			}
			return true;
		}
		
		private bool r_N()
		{
			int v1;
			int v2;
			int v3;
			// (, line 34
			// test, line 34
			v1 = limit - cursor;
			// hop, line 34
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// (, line 34
			// hop, line 34
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			// or, line 34
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// not, line 34
					{
						v3 = limit - cursor;
						do 
						{
							// literal, line 34
							if (!(eqSB(1, "s")))
							{
								goto lab2Brk;
							}
							goto lab1Brk;
						}
						while (false);

lab2Brk: ;
						
						cursor = limit - v3;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				// hop, line 34
				{
					int c = cursor - 2;
					if (limitBackward > c || c > limit)
					{
						return false;
					}
					cursor = c;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_O()
		{
			int v1;
			int v2;
			// (, line 35
			// test, line 35
			v1 = limit - cursor;
			// hop, line 35
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 35

lab4: 
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 35
					if (!(eqSB(1, "l")))
					{
						goto lab4Brk;
					}
					goto lab4Brk;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
				// literal, line 35
				if (!(eqSB(1, "i")))
				{
					return false;
				}
			}
			while (false);
			return true;
		}
		
		private bool r_P()
		{
			int v1;
			int v2;
			// (, line 36
			// test, line 36
			v1 = limit - cursor;
			// hop, line 36
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 36
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 36
					if (!(eqSB(1, "c")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
			}
			return true;
		}
		
		private bool r_Q()
		{
			int v1;
			int v2;
			int v3;
			int v4;
			// (, line 37
			// test, line 37
			v1 = limit - cursor;
			// hop, line 37
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// test, line 37
			v2 = limit - cursor;
			// hop, line 37
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v2;
			// not, line 37
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 37
					if (!(eqSB(1, "l")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v3;
			}
			// not, line 37
			{
				v4 = limit - cursor;
				do 
				{
					// literal, line 37
					if (!(eqSB(1, "n")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v4;
			}
			return true;
		}
		
		private bool r_R()
		{
			int v1;
			int v2;
			// (, line 38
			// test, line 38
			v1 = limit - cursor;
			// hop, line 38
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 38

lab4: 
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 38
					if (!(eqSB(1, "n")))
					{
						goto lab4Brk;
					}
					goto lab4Brk;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
				// literal, line 38
				if (!(eqSB(1, "r")))
				{
					return false;
				}
			}
			while (false);
			return true;
		}
		
		private bool r_S()
		{
			int v1;
			int v2;
			int v3;
			// (, line 39
			// test, line 39
			v1 = limit - cursor;
			// hop, line 39
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 39
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 39
					if (!(eqSB(2, "dr")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				// (, line 39
				// literal, line 39
				if (!(eqSB(1, "t")))
				{
					return false;
				}
				// not, line 39
				{
					v3 = limit - cursor;
					do 
					{
						// literal, line 39
						if (!(eqSB(1, "t")))
						{
							goto lab2Brk;
						}
						return false;
					}
					while (false);

lab2Brk: ;
					
					cursor = limit - v3;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_T()
		{
			int v1;
			int v2;
			int v3;
			// (, line 40
			// test, line 40
			v1 = limit - cursor;
			// hop, line 40
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 40
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 40
					if (!(eqSB(1, "s")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				// (, line 40
				// literal, line 40
				if (!(eqSB(1, "t")))
				{
					return false;
				}
				// not, line 40
				{
					v3 = limit - cursor;
					do 
					{
						// literal, line 40
						if (!(eqSB(1, "o")))
						{
							goto lab2Brk;
						}
						return false;
					}
					while (false);

lab2Brk: ;
					
					cursor = limit - v3;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_U()
		{
			int v1;
			int v2;
			// (, line 41
			// test, line 41
			v1 = limit - cursor;
			// hop, line 41
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 41
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 41
					if (!(eqSB(1, "l")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				do 
				{
					// literal, line 41
					if (!(eqSB(1, "m")))
					{
						goto lab2Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
				do 
				{
					// literal, line 41
					if (!(eqSB(1, "n")))
					{
						goto lab3Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab3Brk: ;
				
				cursor = limit - v2;
				// literal, line 41
				if (!(eqSB(1, "r")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_V()
		{
			int v1;
			// (, line 42
			// test, line 42
			v1 = limit - cursor;
			// hop, line 42
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// literal, line 42
			if (!(eqSB(1, "c")))
			{
				return false;
			}
			return true;
		}
		
		private bool r_W()
		{
			int v1;
			int v2;
			int v3;
			// (, line 43
			// test, line 43
			v1 = limit - cursor;
			// hop, line 43
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 43
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 43
					if (!(eqSB(1, "s")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 43
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 43
					if (!(eqSB(1, "u")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v3;
			}
			return true;
		}
		
		private bool r_X()
		{
			int v1;
			int v2;
			// (, line 44
			// test, line 44
			v1 = limit - cursor;
			// hop, line 44
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// or, line 44
			do 
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 44
					if (!(eqSB(1, "l")))
					{
						goto lab1Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab1Brk: ;
				
				cursor = limit - v2;
				do 
				{
					// literal, line 44
					if (!(eqSB(1, "i")))
					{
						goto lab2Brk;
					}
					goto lab0Brk;
				}
				while (false);

lab2Brk: ;
				
				cursor = limit - v2;
				// (, line 44
				// literal, line 44
				if (!(eqSB(1, "e")))
				{
					return false;
				}
				// next, line 44
				if (cursor <= limitBackward)
				{
					return false;
				}
				cursor--;
				// literal, line 44
				if (!(eqSB(1, "u")))
				{
					return false;
				}
			}
			while (false);

lab0Brk: ;

			return true;
		}
		
		private bool r_Y()
		{
			int v1;
			// (, line 45
			// test, line 45
			v1 = limit - cursor;
			// hop, line 45
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// literal, line 45
			if (!(eqSB(2, "in")))
			{
				return false;
			}
			return true;
		}
		
		private bool r_Z()
		{
			int v1;
			int v2;
			// (, line 46
			// test, line 46
			v1 = limit - cursor;
			// hop, line 46
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 46
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 46
					if (!(eqSB(1, "f")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
			}
			return true;
		}
		
		private bool r_AA()
		{
			int v1;
			// (, line 47
			// test, line 47
			v1 = limit - cursor;
			// hop, line 47
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// among, line 47
			if (findAmongB(a0, 9) == 0)
			{
				return false;
			}
			return true;
		}
		
		private bool r_BB()
		{
			int v1;
			int v2;
			int v3;
			// (, line 49
			// test, line 49
			v1 = limit - cursor;
			// hop, line 49
			{
				int c = cursor - 3;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// not, line 49
			{
				v2 = limit - cursor;
				do 
				{
					// literal, line 49
					if (!(eqSB(3, "met")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v2;
			}
			// not, line 49
			{
				v3 = limit - cursor;
				do 
				{
					// literal, line 49
					if (!(eqSB(4, "ryst")))
					{
						goto lab4Brk;
					}
					return false;
				}
				while (false);

lab4Brk: ;
				
				cursor = limit - v3;
			}
			return true;
		}
		
		private bool r_CC()
		{
			int v1;
			// (, line 50
			// test, line 50
			v1 = limit - cursor;
			// hop, line 50
			{
				int c = cursor - 2;
				if (limitBackward > c || c > limit)
				{
					return false;
				}
				cursor = c;
			}
			cursor = limit - v1;
			// literal, line 50
			if (!(eqSB(1, "l")))
			{
				return false;
			}
			return true;
		}
		
		private bool rEndings()
		{
			int amongVar;
			// (, line 55
			// [, line 56
			ket = cursor;
			// substring, line 56
			amongVar = findAmongB(a1, 294);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 56
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 145
					// delete, line 145
					sliceDel();
					break;
				}
			return true;
		}
		
		private bool rUndouble()
		{
			int v1;
			// (, line 151
			// test, line 152
			v1 = limit - cursor;
			// substring, line 152
			if (findAmongB(a2, 10) == 0)
			{
				return false;
			}
			cursor = limit - v1;
			// [, line 154
			ket = cursor;
			// next, line 154
			if (cursor <= limitBackward)
			{
				return false;
			}
			cursor--;
			// ], line 154
			bra = cursor;
			// delete, line 154
			sliceDel();
			return true;
		}
		
		private bool rRespell()
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
			// (, line 159
			// [, line 160
			ket = cursor;
			// substring, line 160
			amongVar = findAmongB(a3, 34);
			if (amongVar == 0)
			{
				return false;
			}
			// ], line 160
			bra = cursor;
			switch (amongVar)
			{
				
				case 0: 
					return false;
				
				case 1: 
					// (, line 161
					// <-, line 161
					sliceFrom("ief");
					break;
				
				case 2: 
					// (, line 162
					// <-, line 162
					sliceFrom("uc");
					break;
				
				case 3: 
					// (, line 163
					// <-, line 163
					sliceFrom("um");
					break;
				
				case 4: 
					// (, line 164
					// <-, line 164
					sliceFrom("rb");
					break;
				
				case 5: 
					// (, line 165
					// <-, line 165
					sliceFrom("ur");
					break;
				
				case 6: 
					// (, line 166
					// <-, line 166
					sliceFrom("ister");
					break;
				
				case 7: 
					// (, line 167
					// <-, line 167
					sliceFrom("meter");
					break;
				
				case 8: 
					// (, line 168
					// <-, line 168
					sliceFrom("olut");
					break;
				
				case 9: 
					// (, line 169
					// not, line 169
					{
						v1 = limit - cursor;
						do 
						{
							// literal, line 169
							if (!(eqSB(1, "a")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v1;
					}
					// not, line 169
					{
						v2 = limit - cursor;
						do 
						{
							// literal, line 169
							if (!(eqSB(1, "i")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v2;
					}
					// not, line 169
					{
						v3 = limit - cursor;
						do 
						{
							// literal, line 169
							if (!(eqSB(1, "o")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v3;
					}
					// <-, line 169
					sliceFrom("l");
					break;
				
				case 10: 
					// (, line 170
					// <-, line 170
					sliceFrom("bic");
					break;
				
				case 11: 
					// (, line 171
					// <-, line 171
					sliceFrom("dic");
					break;
				
				case 12: 
					// (, line 172
					// <-, line 172
					sliceFrom("pic");
					break;
				
				case 13: 
					// (, line 173
					// <-, line 173
					sliceFrom("tic");
					break;
				
				case 14: 
					// (, line 174
					// <-, line 174
					sliceFrom("ac");
					break;
				
				case 15: 
					// (, line 175
					// <-, line 175
					sliceFrom("ec");
					break;
				
				case 16: 
					// (, line 176
					// <-, line 176
					sliceFrom("ic");
					break;
				
				case 17: 
					// (, line 177
					// <-, line 177
					sliceFrom("luc");
					break;
				
				case 18: 
					// (, line 178
					// <-, line 178
					sliceFrom("uas");
					break;
				
				case 19: 
					// (, line 179
					// <-, line 179
					sliceFrom("vas");
					break;
				
				case 20: 
					// (, line 180
					// <-, line 180
					sliceFrom("cis");
					break;
				
				case 21: 
					// (, line 181
					// <-, line 181
					sliceFrom("lis");
					break;
				
				case 22: 
					// (, line 182
					// <-, line 182
					sliceFrom("eris");
					break;
				
				case 23: 
					// (, line 183
					// <-, line 183
					sliceFrom("pans");
					break;
				
				case 24: 
					// (, line 184
					// not, line 184
					{
						v4 = limit - cursor;
						do 
						{
							// literal, line 184
							if (!(eqSB(1, "s")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v4;
					}
					// <-, line 184
					sliceFrom("ens");
					break;
				
				case 25: 
					// (, line 185
					// <-, line 185
					sliceFrom("ons");
					break;
				
				case 26: 
					// (, line 186
					// <-, line 186
					sliceFrom("lus");
					break;
				
				case 27: 
					// (, line 187
					// <-, line 187
					sliceFrom("rus");
					break;
				
				case 28: 
					// (, line 188
					// not, line 188
					{
						v5 = limit - cursor;
						do 
						{
							// literal, line 188
							if (!(eqSB(1, "p")))
							{
								goto lab4Brk;
							}
							return false;
						}
						while (false);

lab4Brk: ;
						
						cursor = limit - v5;
					}
					// not, line 188
					{
						v6 = limit - cursor;
						do 
						{
							// literal, line 188
							if (!(eqSB(1, "t")))
							{
								goto lab5Brk;
							}
							return false;
						}
						while (false);

lab5Brk: ;
						
						cursor = limit - v6;
					}
					// <-, line 188
					sliceFrom("hes");
					break;
				
				case 29: 
					// (, line 189
					// <-, line 189
					sliceFrom("mis");
					break;
				
				case 30: 
					// (, line 190
					// not, line 190
					{
						v7 = limit - cursor;
						do 
						{
							// literal, line 190
							if (!(eqSB(1, "m")))
							{
								goto lab6Brk;
							}
							return false;
						}
						while (false);

lab6Brk: ;
						
						cursor = limit - v7;
					}
					// <-, line 190
					sliceFrom("ens");
					break;
				
				case 31: 
					// (, line 192
					// <-, line 192
					sliceFrom("ers");
					break;
				
				case 32: 
					// (, line 193
					// not, line 193
					{
						v8 = limit - cursor;
						do 
						{
							// literal, line 193
							if (!(eqSB(1, "n")))
							{
								goto lab7Brk;
							}
							return false;
						}
						while (false);

lab7Brk: ;
						
						cursor = limit - v8;
					}
					// <-, line 193
					sliceFrom("es");
					break;
				
				case 33: 
					// (, line 194
					// <-, line 194
					sliceFrom("ys");
					break;
				
				case 34: 
					// (, line 195
					// <-, line 195
					sliceFrom("ys");
					break;
				}
			return true;
		}
		
		public virtual bool Stem()
		{
			int v1;
			int v2;
			int v3;
			// (, line 200
			// backwards, line 202
			limitBackward = cursor; cursor = limit;
			// (, line 202
			// do, line 203
			v1 = limit - cursor;
			do 
			{
				// call endings, line 203
				if (!rEndings())
				{
					goto lab0Brk;
				}
			}
			while (false);

lab0Brk: ;
			
			cursor = limit - v1;
			// do, line 204
			v2 = limit - cursor;
			do 
			{
				// call undouble, line 204
				if (!rUndouble())
				{
					goto lab1Brk;
				}
			}
			while (false);

lab1Brk: ;
			
			cursor = limit - v2;
			// do, line 205
			v3 = limit - cursor;
			do 
			{
				// call respell, line 205
				if (!rRespell())
				{
					goto lab2Brk;
				}
			}
			while (false);

lab2Brk: ;
			
			cursor = limit - v3;
			cursor = limitBackward; return true;
		}
	}
}