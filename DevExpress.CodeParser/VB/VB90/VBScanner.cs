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
using System.IO;
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public partial class VBScanner : VBScannerBase
	{
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 248;
	const int noSym = 248;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  2,  0,  0,  1,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,115,138,142,130,154,139,144,112,113,147,145,109,146,140,148,
	137,137,137,137,137,137,137,137,137,137,153,  0,151,116,152,117,
	123,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 24,149,  0,150,141,
	  0, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
	 22, 22,143, 22, 22, 22, 22, 22, 22, 22, 22,110,  0,111,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};
	char valCh;
	public VBScanner(string input)
			: base(input)
		{
		}
	public VBScanner(ISourceReader s)
	  : base(s)
		{
		}
		public VBScanner(ISourceReader reader, int line, int offset)
			: base(reader, line, offset)
		{
		}
		protected override int GetUnicodeLetterIndex()
		{
			return UnicodeLetterIndex;
		}
		protected override int GetNextState(int input)
		{
			return start[input];
		}
		protected override void InitializeIgnoreTable()
		{
			ignore = new BitArray(charSetSize + 1);
			ignore[' '] = true;  
					ignore[9] = true;
		}
		protected override void NextChCasing()
		{
					valCh = ch;
		if (ch != EOF) ch = char.ToLower(ch);
	  if (ch != EOF)
		ch = CharUtils.Translate(ch);
		}
		protected override void AddCh()
		{
			base.AddCh();
					tval[tlen ++] = valCh;
			NextCh();
		}
		void CheckLiteral()
		{
					switch (t.Value.ToLower())
		{
			case "addhandler": t.Type = 52; break;
			case "addressof": t.Type = 53; break;
			case "alias": t.Type = 54; break;
			case "and": t.Type = 55; break;
			case "andalso": t.Type = 56; break;
			case "as": t.Type = 57; break;
			case "boolean": t.Type = 58; break;
			case "byref": t.Type = 59; break;
			case "byte": t.Type = 60; break;
			case "byval": t.Type = 61; break;
			case "call": t.Type = 62; break;
			case "case": t.Type = 63; break;
			case "catch": t.Type = 64; break;
			case "cbool": t.Type = 65; break;
			case "cbyte": t.Type = 66; break;
			case "cchar": t.Type = 67; break;
			case "cdate": t.Type = 68; break;
			case "cdbl": t.Type = 69; break;
			case "cdec": t.Type = 70; break;
			case "char": t.Type = 71; break;
			case "cint": t.Type = 72; break;
			case "class": t.Type = 73; break;
			case "clng": t.Type = 74; break;
			case "cobj": t.Type = 75; break;
			case "const": t.Type = 76; break;
			case "continue": t.Type = 77; break;
			case "csbyte": t.Type = 78; break;
			case "cshort": t.Type = 79; break;
			case "csng": t.Type = 80; break;
			case "cstr": t.Type = 81; break;
			case "ctype": t.Type = 82; break;
			case "cuint": t.Type = 83; break;
			case "culng": t.Type = 84; break;
			case "cushort": t.Type = 85; break;
			case "date": t.Type = 86; break;
			case "decimal": t.Type = 87; break;
			case "declare": t.Type = 88; break;
			case "default": t.Type = 89; break;
			case "delegate": t.Type = 90; break;
			case "dim": t.Type = 91; break;
			case "directcast": t.Type = 92; break;
			case "do": t.Type = 93; break;
			case "double": t.Type = 94; break;
			case "each": t.Type = 95; break;
			case "else": t.Type = 96; break;
			case "elseif": t.Type = 97; break;
			case "end": t.Type = 98; break;
			case "endif": t.Type = 99; break;
			case "enum": t.Type = 100; break;
			case "erase": t.Type = 101; break;
			case "error": t.Type = 102; break;
			case "event": t.Type = 103; break;
			case "exit": t.Type = 104; break;
			case "false": t.Type = 105; break;
			case "finally": t.Type = 106; break;
			case "for": t.Type = 107; break;
			case "friend": t.Type = 108; break;
			case "function": t.Type = 109; break;
			case "get": t.Type = 110; break;
			case "gettype": t.Type = 111; break;
			case "global": t.Type = 112; break;
			case "gosub": t.Type = 113; break;
			case "goto": t.Type = 114; break;
			case "handles": t.Type = 115; break;
			case "if": t.Type = 116; break;
			case "implements": t.Type = 117; break;
			case "imports": t.Type = 118; break;
			case "in": t.Type = 119; break;
			case "out": t.Type = 120; break;
			case "inherits": t.Type = 121; break;
			case "integer": t.Type = 122; break;
			case "interface": t.Type = 123; break;
			case "is": t.Type = 124; break;
			case "isnot": t.Type = 125; break;
			case "isfalse": t.Type = 126; break;
			case "istrue": t.Type = 127; break;
			case "let": t.Type = 128; break;
			case "lib": t.Type = 129; break;
			case "like": t.Type = 130; break;
			case "long": t.Type = 131; break;
			case "loop": t.Type = 132; break;
			case "me": t.Type = 133; break;
			case "mod": t.Type = 134; break;
			case "module": t.Type = 135; break;
			case "mustinherit": t.Type = 136; break;
			case "mustoverride": t.Type = 137; break;
			case "mybase": t.Type = 138; break;
			case "myclass": t.Type = 139; break;
			case "namespace": t.Type = 140; break;
			case "narrowing": t.Type = 141; break;
			case "new": t.Type = 142; break;
			case "next": t.Type = 143; break;
			case "not": t.Type = 144; break;
			case "nothing": t.Type = 145; break;
			case "notinheritable": t.Type = 146; break;
			case "notoverridable": t.Type = 147; break;
			case "object": t.Type = 148; break;
			case "of": t.Type = 149; break;
			case "on": t.Type = 150; break;
			case "operator": t.Type = 151; break;
			case "option": t.Type = 152; break;
			case "optional": t.Type = 153; break;
			case "or": t.Type = 154; break;
			case "orelse": t.Type = 155; break;
			case "overloads": t.Type = 156; break;
			case "overridable": t.Type = 157; break;
			case "overrides": t.Type = 158; break;
			case "paramarray": t.Type = 159; break;
			case "partial": t.Type = 160; break;
			case "private": t.Type = 161; break;
			case "property": t.Type = 162; break;
			case "protected": t.Type = 163; break;
			case "public": t.Type = 164; break;
			case "preserve": t.Type = 165; break;
			case "raiseevent": t.Type = 166; break;
			case "readonly": t.Type = 167; break;
			case "redim": t.Type = 168; break;
			case "removehandler": t.Type = 169; break;
			case "resume": t.Type = 170; break;
			case "return": t.Type = 171; break;
			case "rem": t.Type = 172; break;
			case "sbyte": t.Type = 173; break;
			case "select": t.Type = 174; break;
			case "set": t.Type = 175; break;
			case "shadows": t.Type = 176; break;
			case "shared": t.Type = 177; break;
			case "short": t.Type = 178; break;
			case "single": t.Type = 179; break;
			case "static": t.Type = 180; break;
			case "step": t.Type = 181; break;
			case "stop": t.Type = 182; break;
			case "string": t.Type = 183; break;
			case "structure": t.Type = 184; break;
			case "sub": t.Type = 185; break;
			case "synclock": t.Type = 186; break;
			case "then": t.Type = 187; break;
			case "throw": t.Type = 188; break;
			case "to": t.Type = 189; break;
			case "true": t.Type = 190; break;
			case "try": t.Type = 191; break;
			case "trycast": t.Type = 192; break;
			case "typeof": t.Type = 193; break;
			case "uinteger": t.Type = 194; break;
			case "ulong": t.Type = 195; break;
			case "ushort": t.Type = 196; break;
			case "using": t.Type = 197; break;
			case "until": t.Type = 198; break;
			case "variant": t.Type = 199; break;
			case "wend": t.Type = 200; break;
			case "when": t.Type = 201; break;
			case "while": t.Type = 202; break;
			case "widening": t.Type = 203; break;
			case "with": t.Type = 204; break;
			case "withevents": t.Type = 205; break;
			case "writeonly": t.Type = 206; break;
			case "xor": t.Type = 207; break;
			case "add": t.Type = 208; break;
			case "remove": t.Type = 209; break;
			case "ansi": t.Type = 210; break;
			case "assembly": t.Type = 211; break;
			case "auto": t.Type = 212; break;
			case "unicode": t.Type = 213; break;
			case "explicit": t.Type = 214; break;
			case "strict": t.Type = 215; break;
			case "compare": t.Type = 216; break;
			case "binary": t.Type = 217; break;
			case "text": t.Type = 218; break;
			case "off": t.Type = 219; break;
			case "custom": t.Type = 220; break;
			case "from": t.Type = 221; break;
			case "where": t.Type = 222; break;
			case "join": t.Type = 223; break;
			case "equals": t.Type = 224; break;
			case "into": t.Type = 225; break;
			case "order": t.Type = 226; break;
			case "by": t.Type = 227; break;
			case "group": t.Type = 228; break;
			case "ascending": t.Type = 229; break;
			case "descending": t.Type = 230; break;
			case "distinct": t.Type = 232; break;
			case "infer": t.Type = 233; break;
			case "key": t.Type = 234; break;
			case "aggregate": t.Type = 235; break;
			case "skip": t.Type = 236; break;
			case "take": t.Type = 237; break;
			default: break;
		}
		}
		protected override void NextTokenScan(int state)
		{
	  int recKind = noSym;
		  int recEnd = pos;
	  int recPrevLineStart = prevLineStart;
			AddCh();		
			switch (state)
			{
				case -1: { t.Type = eofSym; break; } 
				case 0:
		{
				  if (recKind != noSym)
		  {
					  tlen = recEnd - t.StartPosition;
			prevLineStart = recPrevLineStart;
					  BackTrackScannerToToken();
				  }
				  t.Type = recKind;
		  break;
			  }
			  		case 1:
			recEnd = pos; recKind = 1;
			if ((ch == 10))
			{
				AddCh();
				goto case 2;
			}
			else
			{
				t.Type = 1;
				break;
			}
		case 2:
			{
				t.Type = 1;
				break;
			}
		case 3:
			recEnd = pos; recKind = 2;
			if ((ch == 'i' || ch == 'l' || ch == 's'))
			{
				AddCh();
				goto case 8;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 4:
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 5;
			}
			else
			{
				goto case 0;
			}
		case 5:
			recEnd = pos; recKind = 2;
			if ((ch >= '%' && ch <= '&' || ch == 'i' || ch == 'l' || ch == 's'))
			{
				AddCh();
				goto case 8;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 5;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 3;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 6:
			if ((ch >= '0' && ch <= '7'))
			{
				AddCh();
				goto case 7;
			}
			else
			{
				goto case 0;
			}
		case 7:
			recEnd = pos; recKind = 2;
			if ((ch >= '%' && ch <= '&' || ch == 'i' || ch == 'l' || ch == 's'))
			{
				AddCh();
				goto case 8;
			}
			else if ((ch >= '0' && ch <= '7'))
			{
				AddCh();
				goto case 7;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 3;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 8:
			{
				t.Type = 2;
				break;
			}
		case 9:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 9;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 10;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 10:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 12;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 11;
			}
			else
			{
				goto case 0;
			}
		case 11:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 12;
			}
			else
			{
				goto case 0;
			}
		case 12:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 12;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 13:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 14;
			}
			else
			{
				goto case 0;
			}
		case 14:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 14;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 15;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 15:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 17;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 16;
			}
			else
			{
				goto case 0;
			}
		case 16:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 17;
			}
			else
			{
				goto case 0;
			}
		case 17:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 17;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 18:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 20;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 19;
			}
			else
			{
				goto case 0;
			}
		case 19:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 20;
			}
			else
			{
				goto case 0;
			}
		case 20:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 20;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 21:
			{
				t.Type = 3;
				break;
			}
		case 22:
			recEnd = pos; recKind = 4;
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 29;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 22;
			}
			else
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 23:
			recEnd = pos; recKind = 4;
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 29;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 23;
			}
			else
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 24:
			if ((ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 25;
			}
			else if (ch == '_')
			{
				AddCh();
				goto case 27;
			}
			else
			{
				goto case 0;
			}
		case 25:
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 26;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 25;
			}
			else if (ch == ']')
			{
				AddCh();
				goto case 29;
			}
			else
			{
				goto case 0;
			}
		case 26:
			if (ch == ']')
			{
				AddCh();
				goto case 29;
			}
			else
			{
				goto case 0;
			}
		case 27:
			if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 28;
			}
			else
			{
				goto case 0;
			}
		case 28:
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 26;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 28;
			}
			else if (ch == ']')
			{
				AddCh();
				goto case 29;
			}
			else
			{
				goto case 0;
			}
		case 29:
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 30:
			if (ch == 'o')
			{
				AddCh();
				goto case 31;
			}
			else
			{
				goto case 0;
			}
		case 31:
			if (ch == 'n')
			{
				AddCh();
				goto case 32;
			}
			else
			{
				goto case 0;
			}
		case 32:
			if (ch == 's')
			{
				AddCh();
				goto case 33;
			}
			else
			{
				goto case 0;
			}
		case 33:
			if (ch == 't')
			{
				AddCh();
				goto case 34;
			}
			else
			{
				goto case 0;
			}
		case 34:
			{
				t.Type = 5;
				break;
			}
		case 35:
			if (ch == 'f')
			{
				AddCh();
				goto case 36;
			}
			else
			{
				goto case 0;
			}
		case 36:
			{
				t.Type = 6;
				break;
			}
		case 37:
			if (ch == 'f')
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 38:
			{
				t.Type = 7;
				break;
			}
		case 39:
			if (ch == 'f')
			{
				AddCh();
				goto case 40;
			}
			else
			{
				goto case 0;
			}
		case 40:
			{
				t.Type = 8;
				break;
			}
		case 41:
			if (ch == 'e')
			{
				AddCh();
				goto case 42;
			}
			else
			{
				goto case 0;
			}
		case 42:
			if (ch == 'g')
			{
				AddCh();
				goto case 43;
			}
			else
			{
				goto case 0;
			}
		case 43:
			if (ch == 'i')
			{
				AddCh();
				goto case 44;
			}
			else
			{
				goto case 0;
			}
		case 44:
			if (ch == 'o')
			{
				AddCh();
				goto case 45;
			}
			else
			{
				goto case 0;
			}
		case 45:
			if (ch == 'n')
			{
				AddCh();
				goto case 46;
			}
			else
			{
				goto case 0;
			}
		case 46:
			{
				t.Type = 10;
				break;
			}
		case 47:
			if (ch == 'e')
			{
				AddCh();
				goto case 48;
			}
			else
			{
				goto case 0;
			}
		case 48:
			if (ch == 'g')
			{
				AddCh();
				goto case 49;
			}
			else
			{
				goto case 0;
			}
		case 49:
			if (ch == 'i')
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 50:
			if (ch == 'o')
			{
				AddCh();
				goto case 51;
			}
			else
			{
				goto case 0;
			}
		case 51:
			if (ch == 'n')
			{
				AddCh();
				goto case 52;
			}
			else
			{
				goto case 0;
			}
		case 52:
			{
				t.Type = 11;
				break;
			}
		case 53:
			if (ch == 't')
			{
				AddCh();
				goto case 54;
			}
			else
			{
				goto case 0;
			}
		case 54:
			if (ch == 'e')
			{
				AddCh();
				goto case 55;
			}
			else
			{
				goto case 0;
			}
		case 55:
			if (ch == 'r')
			{
				AddCh();
				goto case 56;
			}
			else
			{
				goto case 0;
			}
		case 56:
			if (ch == 'n')
			{
				AddCh();
				goto case 57;
			}
			else
			{
				goto case 0;
			}
		case 57:
			if (ch == 'a')
			{
				AddCh();
				goto case 58;
			}
			else
			{
				goto case 0;
			}
		case 58:
			if (ch == 'l')
			{
				AddCh();
				goto case 59;
			}
			else
			{
				goto case 0;
			}
		case 59:
			if (ch == 's')
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 60:
			if (ch == 'o')
			{
				AddCh();
				goto case 61;
			}
			else
			{
				goto case 0;
			}
		case 61:
			if (ch == 'u')
			{
				AddCh();
				goto case 62;
			}
			else
			{
				goto case 0;
			}
		case 62:
			if (ch == 'r')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				goto case 0;
			}
		case 63:
			if (ch == 'c')
			{
				AddCh();
				goto case 64;
			}
			else
			{
				goto case 0;
			}
		case 64:
			if (ch == 'e')
			{
				AddCh();
				goto case 65;
			}
			else
			{
				goto case 0;
			}
		case 65:
			{
				t.Type = 12;
				break;
			}
		case 66:
			if (ch == 'x')
			{
				AddCh();
				goto case 67;
			}
			else
			{
				goto case 0;
			}
		case 67:
			if (ch == 't')
			{
				AddCh();
				goto case 68;
			}
			else
			{
				goto case 0;
			}
		case 68:
			if (ch == 'e')
			{
				AddCh();
				goto case 69;
			}
			else
			{
				goto case 0;
			}
		case 69:
			if (ch == 'r')
			{
				AddCh();
				goto case 70;
			}
			else
			{
				goto case 0;
			}
		case 70:
			if (ch == 'n')
			{
				AddCh();
				goto case 71;
			}
			else
			{
				goto case 0;
			}
		case 71:
			if (ch == 'a')
			{
				AddCh();
				goto case 72;
			}
			else
			{
				goto case 0;
			}
		case 72:
			if (ch == 'l')
			{
				AddCh();
				goto case 73;
			}
			else
			{
				goto case 0;
			}
		case 73:
			if (ch == 's')
			{
				AddCh();
				goto case 74;
			}
			else
			{
				goto case 0;
			}
		case 74:
			if (ch == 'o')
			{
				AddCh();
				goto case 75;
			}
			else
			{
				goto case 0;
			}
		case 75:
			if (ch == 'u')
			{
				AddCh();
				goto case 76;
			}
			else
			{
				goto case 0;
			}
		case 76:
			if (ch == 'r')
			{
				AddCh();
				goto case 77;
			}
			else
			{
				goto case 0;
			}
		case 77:
			if (ch == 'c')
			{
				AddCh();
				goto case 78;
			}
			else
			{
				goto case 0;
			}
		case 78:
			if (ch == 'e')
			{
				AddCh();
				goto case 79;
			}
			else
			{
				goto case 0;
			}
		case 79:
			{
				t.Type = 13;
				break;
			}
		case 80:
			recEnd = pos; recKind = 14;
			if ((ch == 13))
			{
				AddCh();
				goto case 81;
			}
			else if ((ch == 10))
			{
				AddCh();
				goto case 82;
			}
			else if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 80;
			}
			else
			{
				t.Type = 14;
				break;
			}
		case 81:
			recEnd = pos; recKind = 14;
			if ((ch == 10))
			{
				AddCh();
				goto case 82;
			}
			else
			{
				t.Type = 14;
				break;
			}
		case 82:
			{
				t.Type = 14;
				break;
			}
		case 83:
			recEnd = pos; recKind = 15;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 83;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 84:
			recEnd = pos; recKind = 15;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 84;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 85:
			recEnd = pos; recKind = 15;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 85;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 86:
			{
				t.Type = 16;
				break;
			}
		case 87:
			recEnd = pos; recKind = 17;
			if ((ch == '"'))
			{
				AddCh();
				goto case 155;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 87;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 88:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 88;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 89;
			}
			else
			{
				goto case 0;
			}
		case 89:
			{
				t.Type = 18;
				break;
			}
		case 90:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 90;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 91;
			}
			else
			{
				goto case 0;
			}
		case 91:
			{
				t.Type = 19;
				break;
			}
		case 92:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 92;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 93;
			}
			else
			{
				goto case 0;
			}
		case 93:
			{
				t.Type = 20;
				break;
			}
		case 94:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 94;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 95;
			}
			else
			{
				goto case 0;
			}
		case 95:
			{
				t.Type = 21;
				break;
			}
		case 96:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 96;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 97;
			}
			else
			{
				goto case 0;
			}
		case 97:
			{
				t.Type = 22;
				break;
			}
		case 98:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 98;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 99;
			}
			else
			{
				goto case 0;
			}
		case 99:
			{
				t.Type = 23;
				break;
			}
		case 100:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 100;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 101;
			}
			else
			{
				goto case 0;
			}
		case 101:
			{
				t.Type = 24;
				break;
			}
		case 102:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 102;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 103;
			}
			else
			{
				goto case 0;
			}
		case 103:
			{
				t.Type = 27;
				break;
			}
		case 104:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 104;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 105;
			}
			else
			{
				goto case 0;
			}
		case 105:
			{
				t.Type = 28;
				break;
			}
		case 106:
			{
				t.Type = 29;
				break;
			}
		case 107:
			{
				t.Type = 30;
				break;
			}
		case 108:
			{
				t.Type = 31;
				break;
			}
		case 109:
			{
				t.Type = 32;
				break;
			}
		case 110:
			{
				t.Type = 33;
				break;
			}
		case 111:
			{
				t.Type = 34;
				break;
			}
		case 112:
			{
				t.Type = 35;
				break;
			}
		case 113:
			{
				t.Type = 36;
				break;
			}
		case 114:
			{
				t.Type = 39;
				break;
			}
		case 115:
			{
				t.Type = 45;
				break;
			}
		case 116:
			{
				t.Type = 47;
				break;
			}
		case 117:
			{
				t.Type = 231;
				break;
			}
		case 118:
			if (ch == '=')
			{
				AddCh();
				goto case 119;
			}
			else
			{
				goto case 0;
			}
		case 119:
			{
				t.Type = 238;
				break;
			}
		case 120:
			{
				t.Type = 239;
				break;
			}
		case 121:
			{
				t.Type = 240;
				break;
			}
		case 122:
			{
				t.Type = 241;
				break;
			}
		case 123:
			{
				t.Type = 242;
				break;
			}
		case 124:
			if (ch == '.')
			{
				AddCh();
				goto case 125;
			}
			else
			{
				goto case 0;
			}
		case 125:
			{
				t.Type = 243;
				break;
			}
		case 126:
			if (ch == '-')
			{
				AddCh();
				goto case 127;
			}
			else
			{
				goto case 0;
			}
		case 127:
			if (ch == '-')
			{
				AddCh();
				goto case 128;
			}
			else
			{
				goto case 0;
			}
		case 128:
			if (!(ch == '-') && ch != EOF)
			{
				AddCh();
				goto case 128;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 156;
			}
			else
			{
				goto case 0;
			}
		case 129:
			{
				t.Type = 244;
				break;
			}
		case 130:
			{
				t.Type = 245;
				break;
			}
		case 131:
			recEnd = pos; recKind = 247;
			if ((ch == 13))
			{
				AddCh();
				goto case 132;
			}
			else if ((ch == 10))
			{
				AddCh();
				goto case 133;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 131;
			}
			else
			{
				t.Type = 247;
				break;
			}
		case 132:
			if ((ch >= 9 && ch <= 10 || ch == ' '))
			{
				AddCh();
				goto case 133;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 134;
			}
			else
			{
				goto case 0;
			}
		case 133:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 133;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 134;
			}
			else
			{
				goto case 0;
			}
		case 134:
			if (ch == 39)
			{
				AddCh();
				goto case 135;
			}
			else
			{
				goto case 0;
			}
		case 135:
			if (ch == 39)
			{
				AddCh();
				goto case 136;
			}
			else
			{
				goto case 0;
			}
		case 136:
			recEnd = pos; recKind = 247;
			if ((ch == 13))
			{
				AddCh();
				goto case 132;
			}
			else if ((ch == 10))
			{
				AddCh();
				goto case 133;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 136;
			}
			else
			{
				t.Type = 247;
				break;
			}
		case 137:
			recEnd = pos; recKind = 2;
			if ((ch >= '%' && ch <= '&' || ch == 'i' || ch == 'l' || ch == 's'))
			{
				AddCh();
				goto case 8;
			}
			else if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 137;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 13;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 18;
			}
			else if ((ch == '!' || ch == '#' || ch == '@' || ch == 'd' || ch == 'f' || ch == 'r'))
			{
				AddCh();
				goto case 21;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 138:
			recEnd = pos; recKind = 17;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 157;
			}
			else if ((ch == '"'))
			{
				AddCh();
				goto case 158;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 139:
			recEnd = pos; recKind = 50;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 100;
			}
			else if (ch == 'h')
			{
				AddCh();
				goto case 4;
			}
			else if (ch == 'o')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 101;
			}
			else
			{
				t.Type = 50;
				break;
			}
		case 140:
			recEnd = pos; recKind = 37;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 9;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 124;
			}
			else
			{
				t.Type = 37;
				break;
			}
		case 141:
			recEnd = pos; recKind = 14;
			if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 23;
			}
			else if ((ch == 13))
			{
				AddCh();
				goto case 81;
			}
			else if ((ch == 10))
			{
				AddCh();
				goto case 82;
			}
			else if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 80;
			}
			else
			{
				t.Type = 14;
				break;
			}
		case 142:
			recEnd = pos; recKind = 51;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 159;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 30;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 35;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 160;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 41;
			}
			else
			{
				t.Type = 51;
				break;
			}
		case 143:
			recEnd = pos; recKind = 4;
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 29;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'd' || ch >= 'f' && ch <= 'z'))
			{
				AddCh();
				goto case 22;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 161;
			}
			else
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 144:
			recEnd = pos; recKind = 15;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '&' || ch >= '(' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 84;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 162;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 145:
			recEnd = pos; recKind = 40;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 88;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 89;
			}
			else
			{
				t.Type = 40;
				break;
			}
		case 146:
			recEnd = pos; recKind = 41;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 90;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 91;
			}
			else
			{
				t.Type = 41;
				break;
			}
		case 147:
			recEnd = pos; recKind = 42;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 92;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 93;
			}
			else
			{
				t.Type = 42;
				break;
			}
		case 148:
			recEnd = pos; recKind = 43;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 94;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 95;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 122;
			}
			else
			{
				t.Type = 43;
				break;
			}
		case 149:
			recEnd = pos; recKind = 44;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 96;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 97;
			}
			else
			{
				t.Type = 44;
				break;
			}
		case 150:
			recEnd = pos; recKind = 46;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 98;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 99;
			}
			else
			{
				t.Type = 46;
				break;
			}
		case 151:
			recEnd = pos; recKind = 49;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 163;
			}
			else if (ch == '<')
			{
				AddCh();
				goto case 164;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 106;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 107;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 118;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 121;
			}
			else if (ch == '!')
			{
				AddCh();
				goto case 126;
			}
			else
			{
				t.Type = 49;
				break;
			}
		case 152:
			recEnd = pos; recKind = 48;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 165;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 166;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 108;
			}
			else
			{
				t.Type = 48;
				break;
			}
		case 153:
			recEnd = pos; recKind = 38;
			if (ch == '=')
			{
				AddCh();
				goto case 114;
			}
			else
			{
				t.Type = 38;
				break;
			}
		case 154:
			recEnd = pos; recKind = 246;
			if (ch == '>')
			{
				AddCh();
				goto case 120;
			}
			else
			{
				t.Type = 246;
				break;
			}
		case 155:
			recEnd = pos; recKind = 17;
			if ((ch == '"'))
			{
				AddCh();
				goto case 87;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 156:
			if ((ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 128;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 129;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 156;
			}
			else
			{
				goto case 0;
			}
		case 157:
			recEnd = pos; recKind = 17;
			if ((ch == '"'))
			{
				AddCh();
				goto case 167;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 87;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 158:
			recEnd = pos; recKind = 17;
			if ((ch == '"'))
			{
				AddCh();
				goto case 157;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 159:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 159;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 30;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 35;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 160;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 41;
			}
			else
			{
				goto case 0;
			}
		case 160:
			if (ch == 'n')
			{
				AddCh();
				goto case 168;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 169;
			}
			else if (ch == 'x')
			{
				AddCh();
				goto case 53;
			}
			else
			{
				goto case 0;
			}
		case 161:
			recEnd = pos; recKind = 4;
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 29;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'l' || ch >= 'n' && ch <= 'z'))
			{
				AddCh();
				goto case 22;
			}
			else if (ch == 'm')
			{
				AddCh();
				goto case 170;
			}
			else
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 162:
			recEnd = pos; recKind = 15;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '&' || ch >= '(' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 85;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 131;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 163:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 163;
			}
			else if (ch == '<')
			{
				AddCh();
				goto case 164;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 106;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 107;
			}
			else
			{
				goto case 0;
			}
		case 164:
			recEnd = pos; recKind = 25;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 104;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 105;
			}
			else
			{
				t.Type = 25;
				break;
			}
		case 165:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 165;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 166;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 108;
			}
			else
			{
				goto case 0;
			}
		case 166:
			recEnd = pos; recKind = 26;
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 102;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 103;
			}
			else
			{
				t.Type = 26;
				break;
			}
		case 167:
			recEnd = pos; recKind = 17;
			if ((ch == '"'))
			{
				AddCh();
				goto case 87;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 86;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 168:
			if (ch == 'd')
			{
				AddCh();
				goto case 171;
			}
			else
			{
				goto case 0;
			}
		case 169:
			if (ch == 's')
			{
				AddCh();
				goto case 172;
			}
			else
			{
				goto case 0;
			}
		case 170:
			recEnd = pos; recKind = 4;
			if ((ch == '!' || ch >= '#' && ch <= '&' || ch == '@'))
			{
				AddCh();
				goto case 29;
			}
			else if ((ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 22;
			}
			else if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 83;
			}
			else
			{
				t.Type = 4;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 171:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 173;
			}
			else
			{
				goto case 0;
			}
		case 172:
			if (ch == 'e')
			{
				AddCh();
				goto case 174;
			}
			else
			{
				goto case 0;
			}
		case 173:
			if ((ch == 9 || ch == ' '))
			{
				AddCh();
				goto case 173;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 37;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 47;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 66;
			}
			else
			{
				goto case 0;
			}
		case 174:
			recEnd = pos; recKind = 9;
			if (ch == 'i')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 9;
				break;
			}
			}
		}
		protected override Token NextToken()
		{
	  Token token = base.NextToken();
	  if (token != null && token.Type == maxT)
		return NextToken();
	  return token;
		}
	public override char CharValue
	{
	  get { return valCh; }
	}
	} 
}
