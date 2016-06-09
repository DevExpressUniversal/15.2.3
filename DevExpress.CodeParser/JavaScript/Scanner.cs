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
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
  partial class JavaScriptScanner
  {
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 86;
	const int noSym = 86;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0, 86, 37,  0,  1, 90, 91, 40, 45, 46, 89, 87, 50, 88, 83, 94,
	 82, 79, 79, 79, 79, 79, 79, 79, 79, 79, 61, 49, 81, 85, 84, 60,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 47, 80, 48, 93,  1,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 43, 92, 44, 57,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};
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
			case "break": t.Type = 6; break;
			case "case": t.Type = 7; break;
			case "catch": t.Type = 8; break;
			case "continue": t.Type = 9; break;
			case "default": t.Type = 10; break;
			case "delete": t.Type = 11; break;
			case "do": t.Type = 12; break;
			case "else": t.Type = 13; break;
			case "finally": t.Type = 14; break;
			case "for": t.Type = 15; break;
			case "function": t.Type = 16; break;
			case "if": t.Type = 17; break;
			case "in": t.Type = 18; break;
			case "instanceof": t.Type = 19; break;
			case "new": t.Type = 20; break;
			case "return": t.Type = 21; break;
			case "switch": t.Type = 22; break;
			case "this": t.Type = 23; break;
			case "throw": t.Type = 24; break;
			case "try": t.Type = 25; break;
			case "typeof": t.Type = 26; break;
			case "var": t.Type = 27; break;
			case "void": t.Type = 28; break;
			case "while": t.Type = 29; break;
			case "with": t.Type = 30; break;
			case "null": t.Type = 31; break;
			case "true": t.Type = 32; break;
			case "false": t.Type = 33; break;
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
	  if (ShouldReadRegExpToken)
	  {
		ReadRegExpToken();
		return;
	  }
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
			if ((ch == '$' || ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 1;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 95;
			}
			else
			{
				t.Type = 1;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 2:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 3;
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
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 6:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 7;
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
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 14:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 15;
			}
			else
			{
				goto case 0;
			}
		case 15:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 16;
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
				goto case 1;
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
				goto case 20;
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
				goto case 1;
			}
			else
			{
				goto case 0;
			}
		case 26:
			if ((ch <= '$' || ch >= '&' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 26;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 96;
			}
			else
			{
				goto case 0;
			}
		case 27:
			{
				t.Type = 2;
				break;
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
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'F' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 29;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 30:
			recEnd = pos; recKind = 4;
			if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 31;
			}
			else if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 31:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 33;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 32;
			}
			else
			{
				goto case 0;
			}
		case 32:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 33;
			}
			else
			{
				goto case 0;
			}
		case 33:
			recEnd = pos; recKind = 4;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 33;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 34:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 36;
			}
			else if ((ch == '+' || ch == '-'))
			{
				AddCh();
				goto case 35;
			}
			else
			{
				goto case 0;
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
			recEnd = pos; recKind = 4;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 36;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 37:
			if ((ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 37;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 42;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 38:
			if ((ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 39;
			}
			else
			{
				goto case 0;
			}
		case 39:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 37;
			}
			else if ((ch == 10 || ch == 13))
			{
				AddCh();
				goto case 98;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 42;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 40:
			if ((ch <= '&' || ch >= '(' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 40;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 42;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 41;
			}
			else
			{
				goto case 0;
			}
		case 41:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 40;
			}
			else
			{
				goto case 0;
			}
		case 42:
			{
				t.Type = 5;
				break;
			}
		case 43:
			{
				t.Type = 34;
				break;
			}
		case 44:
			{
				t.Type = 35;
				break;
			}
		case 45:
			{
				t.Type = 36;
				break;
			}
		case 46:
			{
				t.Type = 37;
				break;
			}
		case 47:
			{
				t.Type = 38;
				break;
			}
		case 48:
			{
				t.Type = 39;
				break;
			}
		case 49:
			{
				t.Type = 41;
				break;
			}
		case 50:
			{
				t.Type = 42;
				break;
			}
		case 51:
			{
				t.Type = 45;
				break;
			}
		case 52:
			{
				t.Type = 46;
				break;
			}
		case 53:
			{
				t.Type = 49;
				break;
			}
		case 54:
			{
				t.Type = 50;
				break;
			}
		case 55:
			{
				t.Type = 55;
				break;
			}
		case 56:
			{
				t.Type = 56;
				break;
			}
		case 57:
			{
				t.Type = 64;
				break;
			}
		case 58:
			{
				t.Type = 65;
				break;
			}
		case 59:
			{
				t.Type = 66;
				break;
			}
		case 60:
			{
				t.Type = 67;
				break;
			}
		case 61:
			{
				t.Type = 68;
				break;
			}
		case 62:
			{
				t.Type = 70;
				break;
			}
		case 63:
			{
				t.Type = 71;
				break;
			}
		case 64:
			{
				t.Type = 72;
				break;
			}
		case 65:
			{
				t.Type = 73;
				break;
			}
		case 66:
			{
				t.Type = 74;
				break;
			}
		case 67:
			{
				t.Type = 75;
				break;
			}
		case 68:
			{
				t.Type = 76;
				break;
			}
		case 69:
			{
				t.Type = 77;
				break;
			}
		case 70:
			{
				t.Type = 78;
				break;
			}
		case 71:
			{
				t.Type = 79;
				break;
			}
		case 72:
			{
				t.Type = 81;
				break;
			}
		case 73:
			recEnd = pos; recKind = 82;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else
			{
				t.Type = 82;
				break;
			}
		case 74:
			if (!(ch == '*') && ch != EOF)
			{
				AddCh();
				goto case 74;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 99;
			}
			else
			{
				goto case 0;
			}
		case 75:
			{
				t.Type = 83;
				break;
			}
		case 76:
			if (ch == '-')
			{
				AddCh();
				goto case 77;
			}
			else
			{
				goto case 0;
			}
		case 77:
			if (ch == '-')
			{
				AddCh();
				goto case 78;
			}
			else
			{
				goto case 0;
			}
		case 78:
			recEnd = pos; recKind = 84;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 78;
			}
			else
			{
				t.Type = 84;
				break;
			}
		case 79:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 79;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 34;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 100;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 80:
			if (ch == 'u')
			{
				AddCh();
				goto case 14;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 18;
			}
			else
			{
				goto case 0;
			}
		case 81:
			recEnd = pos; recKind = 43;
			if (ch == '%')
			{
				AddCh();
				goto case 26;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 51;
			}
			else if (ch == '<')
			{
				AddCh();
				goto case 101;
			}
			else if (ch == '!')
			{
				AddCh();
				goto case 76;
			}
			else
			{
				t.Type = 43;
				break;
			}
		case 82:
			recEnd = pos; recKind = 3;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 79;
			}
			else if ((ch == 'X' || ch == 'x'))
			{
				AddCh();
				goto case 28;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 34;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 100;
			}
			else
			{
				t.Type = 3;
				break;
			}
		case 83:
			recEnd = pos; recKind = 40;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 40;
				break;
			}
		case 84:
			recEnd = pos; recKind = 44;
			if (ch == '=')
			{
				AddCh();
				goto case 52;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 102;
			}
			else
			{
				t.Type = 44;
				break;
			}
		case 85:
			recEnd = pos; recKind = 69;
			if (ch == '=')
			{
				AddCh();
				goto case 103;
			}
			else
			{
				t.Type = 69;
				break;
			}
		case 86:
			recEnd = pos; recKind = 63;
			if (ch == '=')
			{
				AddCh();
				goto case 104;
			}
			else
			{
				t.Type = 63;
				break;
			}
		case 87:
			recEnd = pos; recKind = 51;
			if (ch == '+')
			{
				AddCh();
				goto case 55;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 62;
			}
			else
			{
				t.Type = 51;
				break;
			}
		case 88:
			recEnd = pos; recKind = 52;
			if (ch == '-')
			{
				AddCh();
				goto case 56;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				t.Type = 52;
				break;
			}
		case 89:
			recEnd = pos; recKind = 53;
			if (ch == '=')
			{
				AddCh();
				goto case 64;
			}
			else
			{
				t.Type = 53;
				break;
			}
		case 90:
			recEnd = pos; recKind = 54;
			if (ch == '=')
			{
				AddCh();
				goto case 65;
			}
			else
			{
				t.Type = 54;
				break;
			}
		case 91:
			recEnd = pos; recKind = 60;
			if (ch == '&')
			{
				AddCh();
				goto case 58;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 69;
			}
			else
			{
				t.Type = 60;
				break;
			}
		case 92:
			recEnd = pos; recKind = 61;
			if (ch == '|')
			{
				AddCh();
				goto case 59;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 70;
			}
			else
			{
				t.Type = 61;
				break;
			}
		case 93:
			recEnd = pos; recKind = 62;
			if (ch == '=')
			{
				AddCh();
				goto case 71;
			}
			else
			{
				t.Type = 62;
				break;
			}
		case 94:
			recEnd = pos; recKind = 80;
			if (ch == '=')
			{
				AddCh();
				goto case 72;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 74;
			}
			else
			{
				t.Type = 80;
				break;
			}
		case 95:
			if (ch == 'u')
			{
				AddCh();
				goto case 2;
			}
			else if (ch == 'U')
			{
				AddCh();
				goto case 6;
			}
			else
			{
				goto case 0;
			}
		case 96:
			if ((ch <= '$' || ch >= '&' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 97;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 27;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 96;
			}
			else
			{
				goto case 0;
			}
		case 97:
			if ((ch <= '$' || ch >= '&' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 97;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 96;
			}
			else
			{
				goto case 0;
			}
		case 98:
			if ((ch == 10 || ch == 13))
			{
				AddCh();
				goto case 98;
			}
			else if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '[' || ch >= ']' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 37;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 42;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 99:
			if ((ch <= ')' || ch >= '+' && ch <= '.' || ch >= '0' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 74;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 75;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 99;
			}
			else
			{
				goto case 0;
			}
		case 100:
			recEnd = pos; recKind = 4;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 30;
			}
			else if ((ch == 'E' || ch == 'e'))
			{
				AddCh();
				goto case 31;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 101:
			recEnd = pos; recKind = 57;
			if (ch == '=')
			{
				AddCh();
				goto case 66;
			}
			else
			{
				t.Type = 57;
				break;
			}
		case 102:
			recEnd = pos; recKind = 58;
			if (ch == '>')
			{
				AddCh();
				goto case 105;
			}
			else if (ch == '=')
			{
				AddCh();
				goto case 67;
			}
			else
			{
				t.Type = 58;
				break;
			}
		case 103:
			recEnd = pos; recKind = 47;
			if (ch == '=')
			{
				AddCh();
				goto case 53;
			}
			else
			{
				t.Type = 47;
				break;
			}
		case 104:
			recEnd = pos; recKind = 48;
			if (ch == '=')
			{
				AddCh();
				goto case 54;
			}
			else
			{
				t.Type = 48;
				break;
			}
		case 105:
			recEnd = pos; recKind = 59;
			if (ch == '=')
			{
				AddCh();
				goto case 68;
			}
			else
			{
				t.Type = 59;
				break;
			}
			}
		}
  } 
}
