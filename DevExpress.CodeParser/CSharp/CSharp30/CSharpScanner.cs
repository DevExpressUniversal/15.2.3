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
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public class CSharpScanner : CSharpScannerBase
	{
	  	const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 149;
	const int noSym = 149;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 80,  0,  0, 79,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,181, 61,186,  0,180,173, 44, 91,100,184,178, 83,172,171,176,
	170,168,168,168,168,168,168,168,168,168,175,101,179,174,177,183,
	169,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 90, 15, 99,185,  1,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 89,182, 98,102,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  1,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  1,  0,  0,  0,  0,  1,  0,  0,  0,  0,  0,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  0,  1,  1,  1,  1,  1,  1,  1,  1,
	  0,  0,  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};
		public CSharpScanner(ISourceReader s)
		{
			Initialize(s);
		}
		public CSharpScanner(ISourceReader s, ScannerExtension extension)
		{
			Initialize(s);
			ScannerExtension = extension;
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
					ignore[9] = true;ignore[10] = true;ignore[13] = true;
		}			
		protected override void NextChCasing()
		{
		}
		protected override void AddCh()
		{
			base.AddCh();
					tval[tlen ++] = ch;
			NextCh();
		}
		void CheckLiteral()
		{
					switch (t.Value)
		{
			case "abstract": t.Type = 8; break;
			case "as": t.Type = 9; break;
			case "base": t.Type = 10; break;
			case "bool": t.Type = 11; break;
			case "break": t.Type = 12; break;
			case "byte": t.Type = 13; break;
			case "case": t.Type = 14; break;
			case "catch": t.Type = 15; break;
			case "char": t.Type = 16; break;
			case "checked": t.Type = 17; break;
			case "class": t.Type = 18; break;
			case "const": t.Type = 19; break;
			case "continue": t.Type = 20; break;
			case "decimal": t.Type = 21; break;
			case "default": t.Type = 22; break;
			case "delegate": t.Type = 23; break;
			case "do": t.Type = 24; break;
			case "double": t.Type = 25; break;
			case "else": t.Type = 26; break;
			case "enum": t.Type = 27; break;
			case "event": t.Type = 28; break;
			case "explicit": t.Type = 29; break;
			case "extern": t.Type = 30; break;
			case "false": t.Type = 31; break;
			case "finally": t.Type = 32; break;
			case "fixed": t.Type = 33; break;
			case "float": t.Type = 34; break;
			case "for": t.Type = 35; break;
			case "foreach": t.Type = 36; break;
			case "goto": t.Type = 37; break;
			case "if": t.Type = 38; break;
			case "implicit": t.Type = 39; break;
			case "in": t.Type = 40; break;
			case "int": t.Type = 41; break;
			case "interface": t.Type = 42; break;
			case "internal": t.Type = 43; break;
			case "is": t.Type = 44; break;
			case "lock": t.Type = 45; break;
			case "long": t.Type = 46; break;
			case "namespace": t.Type = 47; break;
			case "new": t.Type = 48; break;
			case "null": t.Type = 49; break;
			case "operator": t.Type = 50; break;
			case "out": t.Type = 51; break;
			case "override": t.Type = 52; break;
			case "params": t.Type = 53; break;
			case "private": t.Type = 54; break;
			case "protected": t.Type = 55; break;
			case "public": t.Type = 56; break;
			case "readonly": t.Type = 57; break;
			case "ref": t.Type = 58; break;
			case "return": t.Type = 59; break;
			case "sbyte": t.Type = 60; break;
			case "sealed": t.Type = 61; break;
			case "short": t.Type = 62; break;
			case "sizeof": t.Type = 63; break;
			case "stackalloc": t.Type = 64; break;
			case "static": t.Type = 65; break;
			case "struct": t.Type = 66; break;
			case "switch": t.Type = 67; break;
			case "this": t.Type = 68; break;
			case "throw": t.Type = 69; break;
			case "true": t.Type = 70; break;
			case "try": t.Type = 71; break;
			case "typeof": t.Type = 72; break;
			case "uint": t.Type = 73; break;
			case "ulong": t.Type = 74; break;
			case "unchecked": t.Type = 75; break;
			case "unsafe": t.Type = 76; break;
			case "ushort": t.Type = 77; break;
			case "using": t.Type = 78; break;
			case "virtual": t.Type = 79; break;
			case "void": t.Type = 80; break;
			case "volatile": t.Type = 81; break;
			case "while": t.Type = 82; break;
			case "__arglist": t.Type = 83; break;
			case "__refvalue": t.Type = 84; break;
			default: break;
		}
		}
		protected override void NextTokenComments()
		{
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
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 1;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 2;
			}
			else
			{
				t.Type = 1;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 2:
			if (ch == 'u')
			{
				AddCh();
				goto case 3;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 7;
			}
			else
			{
				goto case 0;
			}
		case 3:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 4;
			}
			else
			{
				goto case 0;
			}
		case 4:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 5;
			}
			else
			{
				goto case 0;
			}
		case 5:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 6;
			}
			else
			{
				goto case 0;
			}
		case 6:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 7:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 8;
			}
			else
			{
				goto case 0;
			}
		case 8:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 9;
			}
			else
			{
				goto case 0;
			}
		case 9:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 10;
			}
			else
			{
				goto case 0;
			}
		case 10:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 11;
			}
			else
			{
				goto case 0;
			}
		case 11:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 12;
			}
			else
			{
				goto case 0;
			}
		case 12:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 13;
			}
			else
			{
				goto case 0;
			}
		case 13:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 14;
			}
			else
			{
				goto case 0;
			}
		case 14:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 15:
			if (ch == 'u')
			{
				AddCh();
				goto case 16;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 20;
			}
			else
			{
				goto case 0;
			}
		case 16:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 17;
			}
			else
			{
				goto case 0;
			}
		case 17:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 18;
			}
			else
			{
				goto case 0;
			}
		case 18:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 19;
			}
			else
			{
				goto case 0;
			}
		case 19:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 20:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 21;
			}
			else
			{
				goto case 0;
			}
		case 21:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 22;
			}
			else
			{
				goto case 0;
			}
		case 22:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 23;
			}
			else
			{
				goto case 0;
			}
		case 23:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 24;
			}
			else
			{
				goto case 0;
			}
		case 24:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 25;
			}
			else
			{
				goto case 0;
			}
		case 25:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 26;
			}
			else
			{
				goto case 0;
			}
		case 26:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 27;
			}
			else
			{
				goto case 0;
			}
		case 27:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 28:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 29;
			}
			else
			{
				goto case 0;
			}
		case 29:
			recEnd = pos; recKind = 2;
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 29;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 191;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 192;
			}
			else if (ch == 'L')
			{
				AddCh();
				goto case 193;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 194;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 30:
			{
				t.Type = 2;
				break;
			}
		case 31:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 31;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 32;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 32:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 34;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 33;
			}
			else
			{
				goto case 0;
			}
		case 33:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 34;
			}
			else
			{
				goto case 0;
			}
		case 34:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 34;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 35:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 36;
			}
			else
			{
				goto case 0;
			}
		case 36:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 36;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 37;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 37:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 39;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 38:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 39;
			}
			else
			{
				goto case 0;
			}
		case 39:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 39;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 40:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 42;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 41;
			}
			else
			{
				goto case 0;
			}
		case 41:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 42;
			}
			else
			{
				goto case 0;
			}
		case 42:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 42;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 43:
			{
				t.Type = 3;
				break;
			}
		case 44:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 45;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 195;
			}
			else
			{
				goto case 0;
			}
		case 45:
			if (ch == 39)
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 46:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 47;
			}
			else
			{
				goto case 0;
			}
		case 47:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 196;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 48:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 49;
			}
			else
			{
				goto case 0;
			}
		case 49:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 50:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 51;
			}
			else
			{
				goto case 0;
			}
		case 51:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 45;
			}
			else
			{
				goto case 0;
			}
		case 52:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 53;
			}
			else
			{
				goto case 0;
			}
		case 53:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 54;
			}
			else
			{
				goto case 0;
			}
		case 54:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 55;
			}
			else
			{
				goto case 0;
			}
		case 55:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 56;
			}
			else
			{
				goto case 0;
			}
		case 56:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 57;
			}
			else
			{
				goto case 0;
			}
		case 57:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 58;
			}
			else
			{
				goto case 0;
			}
		case 58:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 59;
			}
			else
			{
				goto case 0;
			}
		case 59:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 45;
			}
			else
			{
				goto case 0;
			}
		case 60:
			{
				t.Type = 4;
				break;
			}
		case 61:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 61;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 77;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 198;
			}
			else
			{
				goto case 0;
			}
		case 62:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 63;
			}
			else
			{
				goto case 0;
			}
		case 63:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '/' || ch >= ':' && ch <= '@' || ch >= 'G' && ch <= '[' || ch >= ']' && ch <= '`' || ch >= 'g' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 61;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 199;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 77;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 198;
			}
			else
			{
				goto case 0;
			}
		case 64:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 65;
			}
			else
			{
				goto case 0;
			}
		case 65:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 66;
			}
			else
			{
				goto case 0;
			}
		case 66:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 67;
			}
			else
			{
				goto case 0;
			}
		case 67:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 61;
			}
			else
			{
				goto case 0;
			}
		case 68:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 69;
			}
			else
			{
				goto case 0;
			}
		case 69:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 70;
			}
			else
			{
				goto case 0;
			}
		case 70:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 71;
			}
			else
			{
				goto case 0;
			}
		case 71:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 72;
			}
			else
			{
				goto case 0;
			}
		case 72:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 73;
			}
			else
			{
				goto case 0;
			}
		case 73:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 74;
			}
			else
			{
				goto case 0;
			}
		case 74:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 75;
			}
			else
			{
				goto case 0;
			}
		case 75:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 61;
			}
			else
			{
				goto case 0;
			}
		case 76:
			if (!(ch == '"') && ch != EOF)
			{
				AddCh();
				goto case 76;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 201;
			}
			else
			{
				goto case 0;
			}
		case 77:
			{
				t.Type = 5;
				break;
			}
		case 78:
			recEnd = pos; recKind = 6;
			if ((ch == 9 || ch >= 11 && ch <= 12 || ch == ' ' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 78;
			}
			else
			{
				t.Type = 6;
				break;
			}
		case 79:
			recEnd = pos; recKind = 7;
			if ((ch == 10))
			{
				AddCh();
				goto case 80;
			}
			else
			{
				t.Type = 7;
				break;
			}
		case 80:
			{
				t.Type = 7;
				break;
			}
		case 81:
			{
				t.Type = 86;
				break;
			}
		case 82:
			{
				t.Type = 88;
				break;
			}
		case 83:
			{
				t.Type = 91;
				break;
			}
		case 84:
			{
				t.Type = 93;
				break;
			}
		case 85:
			{
				t.Type = 95;
				break;
			}
		case 86:
			{
				t.Type = 96;
				break;
			}
		case 87:
			{
				t.Type = 98;
				break;
			}
		case 88:
			{
				t.Type = 99;
				break;
			}
		case 89:
			{
				t.Type = 100;
				break;
			}
		case 90:
			{
				t.Type = 101;
				break;
			}
		case 91:
			{
				t.Type = 102;
				break;
			}
		case 92:
			{
				t.Type = 103;
				break;
			}
		case 93:
			{
				t.Type = 107;
				break;
			}
		case 94:
			{
				t.Type = 108;
				break;
			}
		case 95:
			{
				t.Type = 109;
				break;
			}
		case 96:
			{
				t.Type = 111;
				break;
			}
		case 97:
			{
				t.Type = 113;
				break;
			}
		case 98:
			{
				t.Type = 115;
				break;
			}
		case 99:
			{
				t.Type = 116;
				break;
			}
		case 100:
			{
				t.Type = 117;
				break;
			}
		case 101:
			{
				t.Type = 118;
				break;
			}
		case 102:
			{
				t.Type = 119;
				break;
			}
		case 103:
			{
				t.Type = 121;
				break;
			}
		case 104:
			{
				t.Type = 122;
				break;
			}
		case 105:
			{
				t.Type = 123;
				break;
			}
		case 106:
			recEnd = pos; recKind = 124;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 106;
			}
			else
			{
				t.Type = 124;
				break;
			}
		case 107:
			if (!(ch == '*') && ch != EOF)
			{
				AddCh();
				goto case 107;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 202;
			}
			else
			{
				goto case 0;
			}
		case 108:
			{
				t.Type = 125;
				break;
			}
		case 109:
			if (!(ch == '*') && ch != EOF)
			{
				AddCh();
				goto case 109;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 203;
			}
			else
			{
				goto case 0;
			}
		case 110:
			{
				t.Type = 126;
				break;
			}
		case 111:
			{
				t.Type = 127;
				break;
			}
		case 112:
			{
				t.Type = 128;
				break;
			}
		case 113:
			{
				t.Type = 129;
				break;
			}
		case 114:
			{
				t.Type = 132;
				break;
			}
		case 115:
			{
				t.Type = 136;
				break;
			}
		case 116:
			if (ch == 'e')
			{
				AddCh();
				goto case 117;
			}
			else
			{
				goto case 0;
			}
		case 117:
			if (ch == 'f')
			{
				AddCh();
				goto case 118;
			}
			else
			{
				goto case 0;
			}
		case 118:
			if (ch == 'i')
			{
				AddCh();
				goto case 119;
			}
			else
			{
				goto case 0;
			}
		case 119:
			if (ch == 'n')
			{
				AddCh();
				goto case 120;
			}
			else
			{
				goto case 0;
			}
		case 120:
			if (ch == 'e')
			{
				AddCh();
				goto case 121;
			}
			else
			{
				goto case 0;
			}
		case 121:
			{
				t.Type = 137;
				break;
			}
		case 122:
			if (ch == 'n')
			{
				AddCh();
				goto case 123;
			}
			else
			{
				goto case 0;
			}
		case 123:
			if (ch == 'd')
			{
				AddCh();
				goto case 124;
			}
			else
			{
				goto case 0;
			}
		case 124:
			if (ch == 'e')
			{
				AddCh();
				goto case 125;
			}
			else
			{
				goto case 0;
			}
		case 125:
			if (ch == 'f')
			{
				AddCh();
				goto case 126;
			}
			else
			{
				goto case 0;
			}
		case 126:
			{
				t.Type = 138;
				break;
			}
		case 127:
			if (ch == 'f')
			{
				AddCh();
				goto case 128;
			}
			else
			{
				goto case 0;
			}
		case 128:
			{
				t.Type = 139;
				break;
			}
		case 129:
			if (ch == 'f')
			{
				AddCh();
				goto case 130;
			}
			else
			{
				goto case 0;
			}
		case 130:
			{
				t.Type = 140;
				break;
			}
		case 131:
			if (ch == 'e')
			{
				AddCh();
				goto case 132;
			}
			else
			{
				goto case 0;
			}
		case 132:
			{
				t.Type = 141;
				break;
			}
		case 133:
			if (ch == 'f')
			{
				AddCh();
				goto case 134;
			}
			else
			{
				goto case 0;
			}
		case 134:
			{
				t.Type = 142;
				break;
			}
		case 135:
			if (ch == 'i')
			{
				AddCh();
				goto case 136;
			}
			else
			{
				goto case 0;
			}
		case 136:
			if (ch == 'n')
			{
				AddCh();
				goto case 137;
			}
			else
			{
				goto case 0;
			}
		case 137:
			if (ch == 'e')
			{
				AddCh();
				goto case 138;
			}
			else
			{
				goto case 0;
			}
		case 138:
			{
				t.Type = 143;
				break;
			}
		case 139:
			if (ch == 'r')
			{
				AddCh();
				goto case 140;
			}
			else
			{
				goto case 0;
			}
		case 140:
			if (ch == 'o')
			{
				AddCh();
				goto case 141;
			}
			else
			{
				goto case 0;
			}
		case 141:
			if (ch == 'r')
			{
				AddCh();
				goto case 142;
			}
			else
			{
				goto case 0;
			}
		case 142:
			{
				t.Type = 144;
				break;
			}
		case 143:
			if (ch == 'a')
			{
				AddCh();
				goto case 144;
			}
			else
			{
				goto case 0;
			}
		case 144:
			if (ch == 'r')
			{
				AddCh();
				goto case 145;
			}
			else
			{
				goto case 0;
			}
		case 145:
			if (ch == 'n')
			{
				AddCh();
				goto case 146;
			}
			else
			{
				goto case 0;
			}
		case 146:
			if (ch == 'i')
			{
				AddCh();
				goto case 147;
			}
			else
			{
				goto case 0;
			}
		case 147:
			if (ch == 'n')
			{
				AddCh();
				goto case 148;
			}
			else
			{
				goto case 0;
			}
		case 148:
			if (ch == 'g')
			{
				AddCh();
				goto case 149;
			}
			else
			{
				goto case 0;
			}
		case 149:
			{
				t.Type = 145;
				break;
			}
		case 150:
			if (ch == 'e')
			{
				AddCh();
				goto case 151;
			}
			else
			{
				goto case 0;
			}
		case 151:
			if (ch == 'g')
			{
				AddCh();
				goto case 152;
			}
			else
			{
				goto case 0;
			}
		case 152:
			if (ch == 'i')
			{
				AddCh();
				goto case 153;
			}
			else
			{
				goto case 0;
			}
		case 153:
			if (ch == 'o')
			{
				AddCh();
				goto case 154;
			}
			else
			{
				goto case 0;
			}
		case 154:
			if (ch == 'n')
			{
				AddCh();
				goto case 155;
			}
			else
			{
				goto case 0;
			}
		case 155:
			{
				t.Type = 146;
				break;
			}
		case 156:
			if (ch == 'e')
			{
				AddCh();
				goto case 157;
			}
			else
			{
				goto case 0;
			}
		case 157:
			if (ch == 'g')
			{
				AddCh();
				goto case 158;
			}
			else
			{
				goto case 0;
			}
		case 158:
			if (ch == 'i')
			{
				AddCh();
				goto case 159;
			}
			else
			{
				goto case 0;
			}
		case 159:
			if (ch == 'o')
			{
				AddCh();
				goto case 160;
			}
			else
			{
				goto case 0;
			}
		case 160:
			if (ch == 'n')
			{
				AddCh();
				goto case 161;
			}
			else
			{
				goto case 0;
			}
		case 161:
			{
				t.Type = 147;
				break;
			}
		case 162:
			if (ch == 'r')
			{
				AddCh();
				goto case 163;
			}
			else
			{
				goto case 0;
			}
		case 163:
			if (ch == 'a')
			{
				AddCh();
				goto case 164;
			}
			else
			{
				goto case 0;
			}
		case 164:
			if (ch == 'g')
			{
				AddCh();
				goto case 165;
			}
			else
			{
				goto case 0;
			}
		case 165:
			if (ch == 'm')
			{
				AddCh();
				goto case 166;
			}
			else
			{
				goto case 0;
			}
		case 166:
			if (ch == 'a')
			{
				AddCh();
				goto case 167;
			}
			else
			{
				goto case 0;
			}
		case 167:
			{
				t.Type = 148;
				break;
			}
		case 168:
			recEnd = pos; recKind = 2;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 168;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 187;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 188;
			}
			else if (ch == 'L')
			{
				AddCh();
				goto case 189;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 190;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 35;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 40;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 169:
			recEnd = pos; recKind = 89;
			if ((ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 1;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 15;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 76;
			}
			else if (ch == ':')
			{
				AddCh();
				goto case 82;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 109;
			}
			else
			{
				t.Type = 89;
				break;
			}
		case 170:
			recEnd = pos; recKind = 2;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 168;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 187;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 188;
			}
			else if (ch == 'L')
			{
				AddCh();
				goto case 189;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 190;
			}
			else if ((ch == 'X' || ch == 'x'))
			{
				AddCh();
				goto case 28;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 35;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 40;
			}
			else if ((ch == 'D' || ch == 'F' || ch == 'M' || ch == 'd' || ch == 'f' || ch == 'm'))
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 171:
			recEnd = pos; recKind = 94;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 31;
			}
			else
			{
				t.Type = 94;
				break;
			}
		case 172:
			recEnd = pos; recKind = 106;
			if (ch == '-')
			{
				AddCh();
				goto case 204;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 93;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 205;
			}
			else
			{
				t.Type = 106;
				break;
			}
		case 173:
			recEnd = pos; recKind = 85;
			if (ch == '=')
			{
				AddCh();
				goto case 81;
			}
			else if (ch == '&')
			{
				AddCh();
				goto case 113;
			}
			else
			{
				t.Type = 85;
				break;
			}
		case 174:
			recEnd = pos; recKind = 87;
			if (ch == '=')
			{
				AddCh();
				goto case 86;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 115;
			}
			else
			{
				t.Type = 87;
				break;
			}
		case 175:
			recEnd = pos; recKind = 90;
			if (ch == ':')
			{
				AddCh();
				goto case 85;
			}
			else
			{
				t.Type = 90;
				break;
			}
		case 176:
			recEnd = pos; recKind = 133;
			if (ch == '=')
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 106;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 107;
			}
			else
			{
				t.Type = 133;
				break;
			}
		case 177:
			recEnd = pos; recKind = 97;
			if (ch == '=')
			{
				AddCh();
				goto case 87;
			}
			else
			{
				t.Type = 97;
				break;
			}
		case 178:
			recEnd = pos; recKind = 112;
			if (ch == '+')
			{
				AddCh();
				goto case 88;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 97;
			}
			else
			{
				t.Type = 112;
				break;
			}
		case 179:
			recEnd = pos; recKind = 104;
			if (ch == '<')
			{
				AddCh();
				goto case 206;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 114;
			}
			else
			{
				t.Type = 104;
				break;
			}
		case 180:
			recEnd = pos; recKind = 134;
			if (ch == '=')
			{
				AddCh();
				goto case 94;
			}
			else
			{
				t.Type = 134;
				break;
			}
		case 181:
			recEnd = pos; recKind = 110;
			if (ch == '=')
			{
				AddCh();
				goto case 95;
			}
			else
			{
				t.Type = 110;
				break;
			}
		case 182:
			recEnd = pos; recKind = 130;
			if (ch == '=')
			{
				AddCh();
				goto case 96;
			}
			else if (ch == '|')
			{
				AddCh();
				goto case 112;
			}
			else
			{
				t.Type = 130;
				break;
			}
		case 183:
			recEnd = pos; recKind = 114;
			if (ch == '?')
			{
				AddCh();
				goto case 111;
			}
			else
			{
				t.Type = 114;
				break;
			}
		case 184:
			recEnd = pos; recKind = 120;
			if (ch == '=')
			{
				AddCh();
				goto case 103;
			}
			else
			{
				t.Type = 120;
				break;
			}
		case 185:
			recEnd = pos; recKind = 131;
			if (ch == '=')
			{
				AddCh();
				goto case 104;
			}
			else
			{
				t.Type = 131;
				break;
			}
		case 186:
			if ((ch == 9 || ch >= 11 && ch <= 12 || ch == ' '))
			{
				AddCh();
				goto case 186;
			}
			else if (ch == 'd')
			{
				AddCh();
				goto case 116;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 122;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 127;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 207;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 135;
			}
			else if (ch == 'w')
			{
				AddCh();
				goto case 143;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 150;
			}
			else if (ch == 'p')
			{
				AddCh();
				goto case 162;
			}
			else
			{
				goto case 0;
			}
		case 187:
			recEnd = pos; recKind = 2;
			if ((ch == 'L' || ch == 'l'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 188:
			recEnd = pos; recKind = 2;
			if ((ch == 'L' || ch == 'l'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 189:
			recEnd = pos; recKind = 2;
			if ((ch == 'U' || ch == 'u'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 190:
			recEnd = pos; recKind = 2;
			if ((ch == 'U' || ch == 'u'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 191:
			recEnd = pos; recKind = 2;
			if ((ch == 'L' || ch == 'l'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 192:
			recEnd = pos; recKind = 2;
			if ((ch == 'L' || ch == 'l'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 193:
			recEnd = pos; recKind = 2;
			if ((ch == 'U' || ch == 'u'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 194:
			recEnd = pos; recKind = 2;
			if ((ch == 'U' || ch == 'u'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 2;
				break;
			}
		case 195:
			if ((ch == '"' || ch == 39 || ch == '0' || ch == 92 || ch >= 'a' && ch <= 'b' || ch == 'f' || ch == 'n' || ch == 'r' || ch == 't' || ch == 'v'))
			{
				AddCh();
				goto case 45;
			}
			else if (ch == 'x')
			{
				AddCh();
				goto case 46;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 48;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 52;
			}
			else
			{
				goto case 0;
			}
		case 196:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 197;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 197:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 45;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 198:
			if ((ch == '"' || ch == 39 || ch == '0' || ch == 92 || ch >= 'a' && ch <= 'b' || ch == 'f' || ch == 'n' || ch == 'r' || ch == 't' || ch == 'v'))
			{
				AddCh();
				goto case 61;
			}
			else if (ch == 'x')
			{
				AddCh();
				goto case 62;
			}
			else if (ch == 'u')
			{
				AddCh();
				goto case 64;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 68;
			}
			else
			{
				goto case 0;
			}
		case 199:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 200;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '/' || ch >= ':' && ch <= '@' || ch >= 'G' && ch <= '[' || ch >= ']' && ch <= '`' || ch >= 'g' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 61;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 77;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 198;
			}
			else
			{
				goto case 0;
			}
		case 200:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 61;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 77;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 198;
			}
			else
			{
				goto case 0;
			}
		case 201:
			recEnd = pos; recKind = 5;
			if (ch == '"')
			{
				AddCh();
				goto case 76;
			}
			else
			{
				t.Type = 5;
				break;
			}
		case 202:
			if ((ch <= ')' || ch >= '+' && ch <= '.' || ch >= '0' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 107;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 108;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 202;
			}
			else
			{
				goto case 0;
			}
		case 203:
			if ((ch <= ')' || ch >= '+' && ch <= '?' || ch >= 'A' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 109;
			}
			else if (ch == '@')
			{
				AddCh();
				goto case 110;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 203;
			}
			else
			{
				goto case 0;
			}
		case 204:
			recEnd = pos; recKind = 92;
			if (ch == '>')
			{
				AddCh();
				goto case 78;
			}
			else
			{
				t.Type = 92;
				break;
			}
		case 205:
			recEnd = pos; recKind = 135;
			if (ch == '*')
			{
				AddCh();
				goto case 105;
			}
			else
			{
				t.Type = 135;
				break;
			}
		case 206:
			recEnd = pos; recKind = 105;
			if (ch == '=')
			{
				AddCh();
				goto case 92;
			}
			else
			{
				t.Type = 105;
				break;
			}
		case 207:
			if (ch == 'l')
			{
				AddCh();
				goto case 208;
			}
			else if (ch == 'n')
			{
				AddCh();
				goto case 209;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 139;
			}
			else
			{
				goto case 0;
			}
		case 208:
			if (ch == 'i')
			{
				AddCh();
				goto case 129;
			}
			else if (ch == 's')
			{
				AddCh();
				goto case 131;
			}
			else
			{
				goto case 0;
			}
		case 209:
			if (ch == 'd')
			{
				AddCh();
				goto case 210;
			}
			else
			{
				goto case 0;
			}
		case 210:
			if (ch == 'i')
			{
				AddCh();
				goto case 133;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 156;
			}
			else
			{
				goto case 0;
			}
			}
		}
	}
}
