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
using System;
namespace SF.Snowball
{
	
	public class SnowballProgram
	{
		/// <summary> Get the current string.</summary>
		virtual public System.String GetCurrent()
		{
			return current.ToString();
		}
		protected internal SnowballProgram()
		{
			current = new System.Text.StringBuilder();
			SetCurrent("");
		}
		
		/// <summary> Set the current string.</summary>
		public virtual void  SetCurrent(System.String value_Renamed)
		{
			//// current.Replace(current.ToString(0, current.Length - 0), value_Renamed, 0, current.Length - 0);
            current.Remove(0, current.Length);
            current.Append(value_Renamed);
			cursor = 0;
			limit = current.Length;
			limitBackward = 0;
			bra = cursor;
			ket = limit;
		}
		
		// current string
		protected internal System.Text.StringBuilder current;
		
		protected internal int cursor;
		protected internal int limit;
		protected internal int limitBackward;
		protected internal int bra;
		protected internal int ket;
		
		protected internal virtual void  copyFrom(SnowballProgram other)
		{
			current = other.current;
			cursor = other.cursor;
			limit = other.limit;
			limitBackward = other.limitBackward;
			bra = other.bra;
			ket = other.ket;
		}
		
		protected internal virtual bool inGrouping(char[] s, int min, int max)
		{
			if (cursor >= limit)
				return false;
			char ch = current[cursor];
			if (ch > max || ch < min)
				return false;
			ch -= (char) (min);
			if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
				return false;
			cursor++;
			return true;
		}
		
		protected internal virtual bool inGroupingB(char[] s, int min, int max)
		{
			if (cursor <= limitBackward)
				return false;
			char ch = current[cursor - 1];
			if (ch > max || ch < min)
				return false;
			ch -= (char) (min);
			if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
				return false;
			cursor--;
			return true;
		}
		
		protected internal virtual bool outGrouping(char[] s, int min, int max)
		{
			if (cursor >= limit)
				return false;
			char ch = current[cursor];
			if (ch > max || ch < min)
			{
				cursor++;
				return true;
			}
			ch -= (char) (min);
			if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
			{
				cursor++;
				return true;
			}
			return false;
		}
		
		protected internal virtual bool outGroupingB(char[] s, int min, int max)
		{
			if (cursor <= limitBackward)
				return false;
			char ch = current[cursor - 1];
			if (ch > max || ch < min)
			{
				cursor--;
				return true;
			}
			ch -= (char) (min);
			if ((s[ch >> 3] & (0x1 << (ch & 0x7))) == 0)
			{
				cursor--;
				return true;
			}
			return false;
		}
		
		protected internal virtual bool inRange(int min, int max)
		{
			if (cursor >= limit)
				return false;
			char ch = current[cursor];
			if (ch > max || ch < min)
				return false;
			cursor++;
			return true;
		}
		
		protected internal virtual bool inRangeB(int min, int max)
		{
			if (cursor <= limitBackward)
				return false;
			char ch = current[cursor - 1];
			if (ch > max || ch < min)
				return false;
			cursor--;
			return true;
		}
		
		protected internal virtual bool outRange(int min, int max)
		{
			if (cursor >= limit)
				return false;
			char ch = current[cursor];
			if (!(ch > max || ch < min))
				return false;
			cursor++;
			return true;
		}
		
		protected internal virtual bool outRangeB(int min, int max)
		{
			if (cursor <= limitBackward)
				return false;
			char ch = current[cursor - 1];
			if (!(ch > max || ch < min))
				return false;
			cursor--;
			return true;
		}
		
		protected internal virtual bool eqS(int sSize, System.String s)
		{
			if (limit - cursor < sSize)
				return false;
			int i;
			for (i = 0; i != sSize; i++)
			{
				if (current[cursor + i] != s[i])
					return false;
			}
			cursor += sSize;
			return true;
		}
		
		protected internal virtual bool eqSB(int sSize, System.String s)
		{
			if (cursor - limitBackward < sSize)
				return false;
			int i;
			for (i = 0; i != sSize; i++)
			{
				if (current[cursor - sSize + i] != s[i])
					return false;
			}
			cursor -= sSize;
			return true;
		}
		
		protected internal virtual bool eqV(System.Text.StringBuilder s)
		{
			return eqS(s.Length, s.ToString());
		}
		
		protected internal virtual bool eqVB(System.Text.StringBuilder s)
		{
			return eqSB(s.Length, s.ToString());
		}
		
		protected internal virtual int findAmong(Among[] v, int vSize)
		{
			int i = 0;
			int j = vSize;
			
			int c = cursor;
			int l = limit;
			
			int commonI = 0;
			int commonJ = 0;
			
			bool firstKeyInspected = false;
			
			while (true)
			{
				int k = i + ((j - i) >> 1);
				int diff = 0;
				int common = commonI < commonJ?commonI:commonJ; // smaller
				Among w = v[k];
				int i2;
				for (i2 = common; i2 < w.sSize; i2++)
				{
					if (c + common == l)
					{
						diff = - 1;
						break;
					}
					diff = current[c + common] - w.s[i2];
					if (diff != 0)
						break;
					common++;
				}
				if (diff < 0)
				{
					j = k;
					commonJ = common;
				}
				else
				{
					i = k;
					commonI = common;
				}
				if (j - i <= 1)
				{
					if (i > 0)
						break; // v->s has been inspected
					if (j == i)
						break; // only one item in v
					
					// - but now we need to go round once more to get
					// v->s inspected. This looks messy, but is actually
					// the optimal approach.
					
					if (firstKeyInspected)
						break;
					firstKeyInspected = true;
				}
			}
			while (true)
			{
				Among w = v[i];
				if (commonI >= w.sSize)
				{
					cursor = c + w.sSize;
					if (w.method == null)
						return w.result;
					bool res;
					try
					{
						System.Object resobj = w.method.Invoke(w.methodobject, (System.Object[]) new System.Object[0]);
						// {{Aroush}} UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca10433"'
						res = resobj.ToString().Equals("true");
					}
					catch (System.Reflection.TargetInvocationException e)
					{
						res = false;
						// FIXME - debug message
					}
					catch (System.UnauthorizedAccessException e)
					{
						res = false;
						// FIXME - debug message
					}
					cursor = c + w.sSize;
					if (res)
						return w.result;
				}
				i = w.substringI;
				if (i < 0)
					return 0;
			}
		}
		
		// findAmongB is for backwards processing. Same comments apply
		protected internal virtual int findAmongB(Among[] v, int vSize)
		{
			int i = 0;
			int j = vSize;
			
			int c = cursor;
			int lb = limitBackward;
			
			int commonI = 0;
			int commonJ = 0;
			
			bool firstKeyInspected = false;
			
			while (true)
			{
				int k = i + ((j - i) >> 1);
				int diff = 0;
				int common = commonI < commonJ?commonI:commonJ;
				Among w = v[k];
				int i2;
				for (i2 = w.sSize - 1 - common; i2 >= 0; i2--)
				{
					if (c - common == lb)
					{
						diff = - 1;
						break;
					}
					diff = current[c - 1 - common] - w.s[i2];
					if (diff != 0)
						break;
					common++;
				}
				if (diff < 0)
				{
					j = k;
					commonJ = common;
				}
				else
				{
					i = k;
					commonI = common;
				}
				if (j - i <= 1)
				{
					if (i > 0)
						break;
					if (j == i)
						break;
					if (firstKeyInspected)
						break;
					firstKeyInspected = true;
				}
			}
			while (true)
			{
				Among w = v[i];
				if (commonI >= w.sSize)
				{
					cursor = c - w.sSize;
					if (w.method == null)
						return w.result;
					
					bool res;
					try
					{
						System.Object resobj = w.method.Invoke(w.methodobject, (System.Object[]) new System.Object[0]);
						// {{Aroush}} UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca10433"'
						res = resobj.ToString().Equals("true");
					}
					catch (System.Reflection.TargetInvocationException e)
					{
						res = false;
						// FIXME - debug message
					}
					catch (System.UnauthorizedAccessException e)
					{
						res = false;
						// FIXME - debug message
					}
					cursor = c - w.sSize;
					if (res)
						return w.result;
				}
				i = w.substringI;
				if (i < 0)
					return 0;
			}
		}
		
		/* to replace chars between cBra and cKet in current by the
		* chars in s.
		*/
		protected internal virtual int replaceS(int cBra, int cKet, System.String s)
		{
			int adjustment = s.Length - (cKet - cBra);
            if (current.Length > bra)
    			current.Replace(current.ToString(bra, ket - bra), s, bra, ket - bra);
            else
                current.Append(s);
			limit += adjustment;
			if (cursor >= cKet)
				cursor += adjustment;
			else if (cursor > cBra)
				cursor = cBra;
			return adjustment;
		}
		
		protected internal virtual void  sliceCheck()
		{
			if (bra < 0 || bra > ket || ket > limit || limit > current.Length)
			// this line could be removed
			{
				System.Console.Error.WriteLine("faulty slice operation");
				// FIXME: report error somehow.
				/*
				fprintf(stderr, "faulty slice operation:\n");
				debug(z, -1, 0);
				exit(1);
				*/
			}
		}
		
		protected internal virtual void  sliceFrom(System.String s)
		{
			sliceCheck();
			replaceS(bra, ket, s);
		}
		
		protected internal virtual void  sliceFrom(System.Text.StringBuilder s)
		{
			sliceFrom(s.ToString());
		}
		
		protected internal virtual void  sliceDel()
		{
			sliceFrom("");
		}
		
		protected internal virtual void  insert(int cBra, int cKet, System.String s)
		{
			int adjustment = replaceS(cBra, cKet, s);
			if (cBra <= bra)
				bra += adjustment;
			if (cBra <= ket)
				ket += adjustment;
		}
		
		protected internal virtual void  insert(int cBra, int cKet, System.Text.StringBuilder s)
		{
			insert(cBra, cKet, s.ToString());
		}
		
		/* Copy the slice into the supplied StringBuffer */
		protected internal virtual System.Text.StringBuilder sliceTo(System.Text.StringBuilder s)
		{
			sliceCheck();
			int len = ket - bra;
			//// s.Replace(s.ToString(0, s.Length - 0), current.ToString(bra, ket), 0, s.Length - 0);
			s.Remove(0, s.Length);
            s.Append(current.ToString(bra, ket));
			return s;
		}
		
		protected internal virtual System.Text.StringBuilder assignTo(System.Text.StringBuilder s)
		{
			//// s.Replace(s.ToString(0, s.Length - 0), current.ToString(0, limit), 0, s.Length - 0);
			s.Remove(0, s.Length);
            s.Append(current.ToString(0, limit));
			return s;
		}
		
		/*
		extern void debug(struct SN_env * z, int number, int lineCount)
		{   int i;
		int limit = SIZE(z->p);
		//if (number >= 0) printf("%3d (line %4d): '", number, lineCount);
		if (number >= 0) printf("%3d (line %4d): [%d]'", number, lineCount,limit);
		for (i = 0; i <= limit; i++)
		{   if (z->lb == i) printf("{");
		if (z->bra == i) printf("[");
		if (z->c == i) printf("|");
		if (z->ket == i) printf("]");
		if (z->l == i) printf("}");
		if (i < limit)
		{   int ch = z->p[i];
		if (ch == 0) ch = '#';
		printf("%c", ch);
		}
		}
		printf("'\n");
		}*/
	}
	
}