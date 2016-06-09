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
namespace DevExpress.CodeRush.StructuralParser.Css
#else
namespace DevExpress.CodeParser.Css
#endif
{
	using Xml;
	partial class CssScanner
	{
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 45;
	const int noSym = 45;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0, 42,  7, 22, 35,  0,  0,  8, 40, 41, 77, 20, 16, 71, 76, 74,
	 70, 70, 70, 70, 70, 70, 70, 70, 70, 70, 19, 10,  2, 29, 21,  0,
	  1,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 28, 73, 38, 33, 69,
	  0, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69, 69,
	 69, 69, 69, 69, 69, 72, 69, 69, 69, 69, 69, 17, 31, 18, 75,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0, 69,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};
	char valCh;
		protected override void InitializeIgnoreTable()
		{
			ignore = new BitArray(charSetSize + 1);
			ignore[' '] = true;  
					ignore[9] = true;ignore[10] = true;ignore[13] = true;ignore[32] = true;
		}
		protected override void NextChCasing()
		{
					valCh = ch;
		if (ch != EOF) ch = char.ToLower(ch);
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
			case "-": t.Type = 15; break;
			case "not": t.Type = 32; break;
			case "only": t.Type = 33; break;
			case "and": t.Type = 34; break;
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
			{
				t.Type = 1;
				break;
			}
		case 2:
			if (ch == '!')
			{
				AddCh();
				goto case 3;
			}
			else
			{
				goto case 0;
			}
		case 3:
			if (ch == '-')
			{
				AddCh();
				goto case 4;
			}
			else
			{
				goto case 0;
			}
		case 4:
			if (ch == '-')
			{
				AddCh();
				goto case 5;
			}
			else
			{
				goto case 0;
			}
		case 5:
			{
				t.Type = 2;
				break;
			}
		case 6:
			{
				t.Type = 3;
				break;
			}
		case 7:
			if (!(ch == '"') && ch != EOF)
			{
				AddCh();
				goto case 7;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 9;
			}
			else
			{
				goto case 0;
			}
		case 8:
			if (!(ch == 39) && ch != EOF)
			{
				AddCh();
				goto case 8;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 9;
			}
			else
			{
				goto case 0;
			}
		case 9:
			{
				t.Type = 4;
				break;
			}
		case 10:
			{
				t.Type = 5;
				break;
			}
		case 11:
			if (!(ch == '"') && ch != EOF)
			{
				AddCh();
				goto case 11;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 12;
			}
			else
			{
				goto case 0;
			}
		case 12:
			if (ch == ')')
			{
				AddCh();
				goto case 15;
			}
			else
			{
				goto case 0;
			}
		case 13:
			if (!(ch == 39) && ch != EOF)
			{
				AddCh();
				goto case 13;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 12;
			}
			else
			{
				goto case 0;
			}
		case 14:
			if (!(ch == ')') && ch != EOF)
			{
				AddCh();
				goto case 14;
			}
			else if (ch == ')')
			{
				AddCh();
				goto case 15;
			}
			else
			{
				goto case 0;
			}
		case 15:
			{
				t.Type = 6;
				break;
			}
		case 16:
			{
				t.Type = 7;
				break;
			}
		case 17:
			{
				t.Type = 8;
				break;
			}
		case 18:
			{
				t.Type = 9;
				break;
			}
		case 19:
			{
				t.Type = 11;
				break;
			}
		case 20:
			{
				t.Type = 13;
				break;
			}
		case 21:
			{
				t.Type = 14;
				break;
			}
		case 22:
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 26;
			}
			else
			{
				goto case 0;
			}
		case 23:
			recEnd = pos; recKind = 17;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 24;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 24:
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 25;
			}
			else
			{
				goto case 0;
			}
		case 25:
			recEnd = pos; recKind = 17;
			if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 80;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 24;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 26:
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 27;
			}
			else
			{
				goto case 0;
			}
		case 27:
			recEnd = pos; recKind = 17;
			if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 81;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 24;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 28:
			{
				t.Type = 20;
				break;
			}
		case 29:
			{
				t.Type = 21;
				break;
			}
		case 30:
			{
				t.Type = 22;
				break;
			}
		case 31:
			if (ch == '=')
			{
				AddCh();
				goto case 32;
			}
			else
			{
				goto case 0;
			}
		case 32:
			{
				t.Type = 23;
				break;
			}
		case 33:
			if (ch == '=')
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
				t.Type = 24;
				break;
			}
		case 35:
			if (ch == '=')
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
				t.Type = 25;
				break;
			}
		case 37:
			{
				t.Type = 26;
				break;
			}
		case 38:
			{
				t.Type = 27;
				break;
			}
		case 39:
			{
				t.Type = 28;
				break;
			}
		case 40:
			{
				t.Type = 29;
				break;
			}
		case 41:
			{
				t.Type = 30;
				break;
			}
		case 42:
			{
				t.Type = 31;
				break;
			}
		case 43:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 44;
			}
			else
			{
				goto case 0;
			}
		case 44:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 44;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else
			{
				goto case 0;
			}
		case 45:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 46;
			}
			else
			{
				goto case 0;
			}
		case 46:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 46;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else
			{
				goto case 0;
			}
		case 47:
			{
				t.Type = 36;
				break;
			}
		case 48:
			if (ch == 'm')
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 49:
			if (ch == 'n')
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 50:
			{
				t.Type = 37;
				break;
			}
		case 51:
			{
				t.Type = 38;
				break;
			}
		case 52:
			{
				t.Type = 39;
				break;
			}
		case 53:
			if (ch == 'e')
			{
				AddCh();
				goto case 54;
			}
			else
			{
				goto case 0;
			}
		case 54:
			if (ch == 'g')
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 55:
			if (ch == 'a')
			{
				AddCh();
				goto case 56;
			}
			else
			{
				goto case 0;
			}
		case 56:
			if (ch == 'd')
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 57:
			if (ch == 'r')
			{
				AddCh();
				goto case 58;
			}
			else
			{
				goto case 0;
			}
		case 58:
			if (ch == 'a')
			{
				AddCh();
				goto case 59;
			}
			else
			{
				goto case 0;
			}
		case 59:
			if (ch == 'd')
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 60:
			{
				t.Type = 40;
				break;
			}
		case 61:
			{
				t.Type = 41;
				break;
			}
		case 62:
			if (ch == 'z')
			{
				AddCh();
				goto case 65;
			}
			else
			{
				goto case 0;
			}
		case 63:
			if (ch == 'h')
			{
				AddCh();
				goto case 64;
			}
			else
			{
				goto case 0;
			}
		case 64:
			if (ch == 'z')
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
				t.Type = 42;
				break;
			}
		case 66:
			recEnd = pos; recKind = 43;
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 66;
			}
			else
			{
				t.Type = 43;
				break;
			}
		case 67:
			if (!(ch == '*') && ch != EOF)
			{
				AddCh();
				goto case 67;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 83;
			}
			else
			{
				goto case 0;
			}
		case 68:
			{
				t.Type = 44;
				break;
			}
		case 69:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 70:
			recEnd = pos; recKind = 35;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 70;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 85;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else if (ch == 'p')
			{
				AddCh();
				goto case 82;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 48;
			}
			else if (ch == 'm')
			{
				AddCh();
				goto case 86;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 49;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 87;
			}
			else if (ch == 'd')
			{
				AddCh();
				goto case 53;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 55;
			}
			else if (ch == 'g')
			{
				AddCh();
				goto case 57;
			}
			else if (ch == 's')
			{
				AddCh();
				goto case 61;
			}
			else if (ch == 'h')
			{
				AddCh();
				goto case 62;
			}
			else if (ch == 'k')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				t.Type = 35;
				break;
			}
		case 71:
			recEnd = pos; recKind = 10;
			if ((ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 88;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 89;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 45;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 72:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'q' || ch >= 's' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 90;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 73:
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 91;
			}
			else
			{
				goto case 0;
			}
		case 74:
			recEnd = pos; recKind = 12;
			if (ch == '/')
			{
				AddCh();
				goto case 66;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 67;
			}
			else
			{
				t.Type = 12;
				break;
			}
		case 75:
			recEnd = pos; recKind = 16;
			if (ch == '=')
			{
				AddCh();
				goto case 30;
			}
			else
			{
				t.Type = 16;
				break;
			}
		case 76:
			recEnd = pos; recKind = 18;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 92;
			}
			else
			{
				t.Type = 18;
				break;
			}
		case 77:
			recEnd = pos; recKind = 19;
			if (ch == '=')
			{
				AddCh();
				goto case 37;
			}
			else
			{
				t.Type = 19;
				break;
			}
		case 78:
			if ((ch <= '!' || ch >= '#' && ch <= '(' || ch >= '*' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 78;
			}
			else if (ch == ')')
			{
				AddCh();
				goto case 93;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 94;
			}
			else
			{
				goto case 0;
			}
		case 79:
			if ((ch <= '&' || ch == '(' || ch >= '*' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 79;
			}
			else if (ch == ')')
			{
				AddCh();
				goto case 95;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 94;
			}
			else
			{
				goto case 0;
			}
		case 80:
			recEnd = pos; recKind = 17;
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 80;
			}
			else if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 24;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 81:
			recEnd = pos; recKind = 17;
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 81;
			}
			else if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 23;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 24;
			}
			else
			{
				t.Type = 17;
				break;
			}
		case 82:
			if ((ch == 'c' || ch == 't' || ch == 'x'))
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 83:
			if ((ch <= ')' || ch >= '+' && ch <= '.' || ch >= '0' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 67;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 68;
			}
			else if (ch == '*')
			{
				AddCh();
				goto case 83;
			}
			else
			{
				goto case 0;
			}
		case 84:
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 96;
			}
			else
			{
				goto case 0;
			}
		case 85:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 97;
			}
			else
			{
				goto case 0;
			}
		case 86:
			if (ch == 'm')
			{
				AddCh();
				goto case 50;
			}
			else if (ch == 's')
			{
				AddCh();
				goto case 61;
			}
			else
			{
				goto case 0;
			}
		case 87:
			if (ch == 'm')
			{
				AddCh();
				goto case 51;
			}
			else if (ch == 'x')
			{
				AddCh();
				goto case 52;
			}
			else
			{
				goto case 0;
			}
		case 88:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 88;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else if (ch == '.')
			{
				AddCh();
				goto case 43;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 89:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 90:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'k' || ch >= 'm' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 'l')
			{
				AddCh();
				goto case 98;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 91:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 99;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 92:
			recEnd = pos; recKind = 35;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 92;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else if (ch == 'p')
			{
				AddCh();
				goto case 82;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 48;
			}
			else if (ch == 'm')
			{
				AddCh();
				goto case 86;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 49;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 87;
			}
			else if (ch == 'd')
			{
				AddCh();
				goto case 53;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 55;
			}
			else if (ch == 'g')
			{
				AddCh();
				goto case 57;
			}
			else if (ch == 's')
			{
				AddCh();
				goto case 61;
			}
			else if (ch == 'h')
			{
				AddCh();
				goto case 62;
			}
			else if (ch == 'k')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				t.Type = 35;
				break;
			}
		case 93:
			recEnd = pos; recKind = 6;
			if (!(ch == '"') && ch != EOF)
			{
				AddCh();
				goto case 11;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 12;
			}
			else
			{
				t.Type = 6;
				break;
			}
		case 94:
			if (!(ch == ')') && ch != EOF)
			{
				AddCh();
				goto case 14;
			}
			else if (ch == ')')
			{
				AddCh();
				goto case 15;
			}
			else
			{
				goto case 0;
			}
		case 95:
			recEnd = pos; recKind = 6;
			if (!(ch == 39) && ch != EOF)
			{
				AddCh();
				goto case 13;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 12;
			}
			else
			{
				t.Type = 6;
				break;
			}
		case 96:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 100;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 97:
			recEnd = pos; recKind = 35;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 97;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 47;
			}
			else if (ch == 'p')
			{
				AddCh();
				goto case 82;
			}
			else if (ch == 'c')
			{
				AddCh();
				goto case 48;
			}
			else if (ch == 'm')
			{
				AddCh();
				goto case 86;
			}
			else if (ch == 'i')
			{
				AddCh();
				goto case 49;
			}
			else if (ch == 'e')
			{
				AddCh();
				goto case 87;
			}
			else if (ch == 'd')
			{
				AddCh();
				goto case 53;
			}
			else if (ch == 'r')
			{
				AddCh();
				goto case 55;
			}
			else if (ch == 'g')
			{
				AddCh();
				goto case 57;
			}
			else if (ch == 's')
			{
				AddCh();
				goto case 61;
			}
			else if (ch == 'h')
			{
				AddCh();
				goto case 62;
			}
			else if (ch == 'k')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				t.Type = 35;
				break;
			}
		case 98:
			recEnd = pos; recKind = 10;
			if ((ch == '-' || ch >= '0' && ch <= '9' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 101;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 99:
			recEnd = pos; recKind = 10;
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 99;
			}
			else if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 100:
			recEnd = pos; recKind = 10;
			if ((ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'f'))
			{
				AddCh();
				goto case 100;
			}
			else if ((ch == '-' || ch == '_' || ch >= 'g' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 92)
			{
				AddCh();
				goto case 84;
			}
			else if (ch == '(')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				t.Type = 10;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 101:
			recEnd = pos; recKind = 28;
			if ((ch <= '!' || ch >= '#' && ch <= '&' || ch == '(' || ch >= '*' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 14;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 78;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 79;
			}
			else
			{
				t.Type = 28;
				break;
			}
			}
		}
	} 
}
