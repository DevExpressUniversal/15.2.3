#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class StringScanner {
		static readonly char[] specialSymbols = new char[] { '(', ')', '[', ']', '/', '.', ',' };
		static readonly int[] specialSymbolTypes = new int[] { PathParser._OpenParenthesis, PathParser._CloseParenthesis, PathParser._OpenBracket, PathParser._CloseBracket, PathParser._Slash, PathParser._Dot, PathParser._Comma };
		PathToken t;
		char ch;
		int pos;
		PathToken tokens;
		PathToken pt;
		int tlen;
		string str;
		public StringScanner(string str) {
			this.str = str;
			Init();
		}
		void Init() {
			pos = -1;
			NextCh();
			pt = tokens = new PathToken();  
		}
		void NextCh() {
			pos++;
			tlen++;
			if(pos < str.Length)
				ch = str[pos];
		}
		public PathToken Scan() {
			if (tokens.Next == null) {
				return NextToken();
			}
			pt = tokens = tokens.Next;
			return tokens;
		}
		public PathToken Peek() {
			if (pt.Next == null) {
				do {
					pt = pt.Next = NextToken();
				} while (pt.Kind > PathParser.maxT); 
			}
			else {
				do {
					pt = pt.Next;
				} while (pt.Kind > PathParser.maxT);
			}
			return pt;
		}
		public void ResetPeek() { pt = tokens; }
		PathToken NextToken() {
			t = new PathToken();
			t.Position = pos;			
			tlen = 0;
			if (pos < str.Length) {
				int specialIndex = Array.IndexOf(specialSymbols, ch);
				if (specialIndex >= 0) {
					t.Kind = specialSymbolTypes[specialIndex];
					NextCh();
				}
				else {
					while (pos < str.Length && Array.IndexOf(specialSymbols, ch) < 0) {
						if (ch == '\\' && pos < str.Length - 1)
							NextCh();
						NextCh();
					}
					t.Kind = PathParser._Text;
				}
			}
			else			
				t.Kind = PathParser._EOF;
			t.Value = str.Substring(t.Position, tlen);
			return t;
		}
	}
}
