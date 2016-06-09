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
namespace DevExpress.CodeRush.StructuralParser.Xml
#else
namespace DevExpress.CodeParser.Xml
#endif
{
	public class XmlScanner : XmlScannerBase
	{
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 54;
	const int noSym = 54;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  6,103,  0, 41,100, 99, 32, 33, 44, 45, 43,  1,  1, 86,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 42, 98, 15, 30,101,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 31,  0,102,  0,  1,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0, 40,  0,  0,  0,
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
		public XmlScanner(ISourceReader s)
		{
			Initialize(s);
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
					ignore[9] = true;ignore[10] = true;ignore[13] = true;ignore[32] = true;
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
			case "version": t.Type = 8; break;
			case "encoding": t.Type = 9; break;
			case "standalone": t.Type = 12; break;
			case "SYSTEM": t.Type = 13; break;
			case "PUBLIC": t.Type = 14; break;
			case "EMPTY": t.Type = 20; break;
			case "ANY": t.Type = 21; break;
			case "ID": t.Type = 34; break;
			case "CDATA": t.Type = 35; break;
			case "IDREF": t.Type = 36; break;
			case "IDREFS": t.Type = 37; break;
			case "ENTITY": t.Type = 38; break;
			case "ENTITIES": t.Type = 39; break;
			case "NMTOKEN": t.Type = 40; break;
			case "NMTOKENS": t.Type = 41; break;
			case "NOTATION": t.Type = 42; break;
			case "NDATA": t.Type = 47; break;
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
			if ((ch >= '-' && ch <= '.' || ch >= '0' && ch <= ':' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 258) || IsUnicodeLetter(ch))
			{
				AddCh();
				goto case 1;
			}
			else
			{
				t.Type = 1;
				t.Value = new String(tval, 0, tlen);
				CheckLiteral();
				break;
			}
		case 2:
			if (ch == '-')
			{
				AddCh();
				goto case 3;
			}
			else
			{
				goto case 0;
			}
		case 3:
			if (!(ch == '-') && ch != EOF)
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 104;
			}
			else
			{
				goto case 0;
			}
		case 4:
			if ((ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 4;
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
			if (!(ch == '"') && ch != EOF)
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '"')
			{
				AddCh();
				goto case 8;
			}
			else
			{
				goto case 0;
			}
		case 7:
			if (!(ch == 39) && ch != EOF)
			{
				AddCh();
				goto case 7;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 8;
			}
			else
			{
				goto case 0;
			}
		case 8:
			{
				t.Type = 3;
				break;
			}
		case 9:
			recEnd = pos; recKind = 4;
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 9;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 10:
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 11;
			}
			else
			{
				goto case 0;
			}
		case 11:
			recEnd = pos; recKind = 4;
			if ((ch >= '0' && ch <= '9' || ch >= 'A' && ch <= 'Z' || ch >= 'a' && ch <= 'z'))
			{
				AddCh();
				goto case 11;
			}
			else
			{
				t.Type = 4;
				break;
			}
		case 12:
			if (ch == 'm')
			{
				AddCh();
				goto case 13;
			}
			else
			{
				goto case 0;
			}
		case 13:
			if (ch == 'l')
			{
				AddCh();
				goto case 14;
			}
			else
			{
				goto case 0;
			}
		case 14:
			{
				t.Type = 5;
				break;
			}
		case 15:
			{
				t.Type = 6;
				break;
			}
		case 16:
			{
				t.Type = 11;
				break;
			}
		case 17:
			if (ch == 'O')
			{
				AddCh();
				goto case 18;
			}
			else
			{
				goto case 0;
			}
		case 18:
			if (ch == 'C')
			{
				AddCh();
				goto case 19;
			}
			else
			{
				goto case 0;
			}
		case 19:
			if (ch == 'T')
			{
				AddCh();
				goto case 20;
			}
			else
			{
				goto case 0;
			}
		case 20:
			if (ch == 'Y')
			{
				AddCh();
				goto case 21;
			}
			else
			{
				goto case 0;
			}
		case 21:
			if (ch == 'P')
			{
				AddCh();
				goto case 22;
			}
			else
			{
				goto case 0;
			}
		case 22:
			if (ch == 'E')
			{
				AddCh();
				goto case 23;
			}
			else
			{
				goto case 0;
			}
		case 23:
			{
				t.Type = 15;
				break;
			}
		case 24:
			if (ch == 'E')
			{
				AddCh();
				goto case 25;
			}
			else
			{
				goto case 0;
			}
		case 25:
			if (ch == 'M')
			{
				AddCh();
				goto case 26;
			}
			else
			{
				goto case 0;
			}
		case 26:
			if (ch == 'E')
			{
				AddCh();
				goto case 27;
			}
			else
			{
				goto case 0;
			}
		case 27:
			if (ch == 'N')
			{
				AddCh();
				goto case 28;
			}
			else
			{
				goto case 0;
			}
		case 28:
			if (ch == 'T')
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
				t.Type = 16;
				break;
			}
		case 30:
			{
				t.Type = 17;
				break;
			}
		case 31:
			{
				t.Type = 18;
				break;
			}
		case 32:
			{
				t.Type = 22;
				break;
			}
		case 33:
			{
				t.Type = 23;
				break;
			}
		case 34:
			if (ch == 'C')
			{
				AddCh();
				goto case 35;
			}
			else
			{
				goto case 0;
			}
		case 35:
			if (ch == 'D')
			{
				AddCh();
				goto case 36;
			}
			else
			{
				goto case 0;
			}
		case 36:
			if (ch == 'A')
			{
				AddCh();
				goto case 37;
			}
			else
			{
				goto case 0;
			}
		case 37:
			if (ch == 'T')
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 38:
			if (ch == 'A')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				goto case 0;
			}
		case 39:
			{
				t.Type = 24;
				break;
			}
		case 40:
			{
				t.Type = 25;
				break;
			}
		case 41:
			{
				t.Type = 26;
				break;
			}
		case 42:
			{
				t.Type = 27;
				break;
			}
		case 43:
			{
				t.Type = 28;
				break;
			}
		case 44:
			{
				t.Type = 30;
				break;
			}
		case 45:
			{
				t.Type = 31;
				break;
			}
		case 46:
			if (ch == 'O')
			{
				AddCh();
				goto case 47;
			}
			else
			{
				goto case 0;
			}
		case 47:
			if (ch == 'T')
			{
				AddCh();
				goto case 48;
			}
			else
			{
				goto case 0;
			}
		case 48:
			if (ch == 'A')
			{
				AddCh();
				goto case 49;
			}
			else
			{
				goto case 0;
			}
		case 49:
			if (ch == 'T')
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 50:
			if (ch == 'I')
			{
				AddCh();
				goto case 51;
			}
			else
			{
				goto case 0;
			}
		case 51:
			if (ch == 'O')
			{
				AddCh();
				goto case 52;
			}
			else
			{
				goto case 0;
			}
		case 52:
			if (ch == 'N')
			{
				AddCh();
				goto case 53;
			}
			else
			{
				goto case 0;
			}
		case 53:
			{
				t.Type = 32;
				break;
			}
		case 54:
			if (ch == 'T')
			{
				AddCh();
				goto case 55;
			}
			else
			{
				goto case 0;
			}
		case 55:
			if (ch == 'T')
			{
				AddCh();
				goto case 56;
			}
			else
			{
				goto case 0;
			}
		case 56:
			if (ch == 'L')
			{
				AddCh();
				goto case 57;
			}
			else
			{
				goto case 0;
			}
		case 57:
			if (ch == 'I')
			{
				AddCh();
				goto case 58;
			}
			else
			{
				goto case 0;
			}
		case 58:
			if (ch == 'S')
			{
				AddCh();
				goto case 59;
			}
			else
			{
				goto case 0;
			}
		case 59:
			if (ch == 'T')
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
				t.Type = 33;
				break;
			}
		case 61:
			if (ch == 'E')
			{
				AddCh();
				goto case 62;
			}
			else
			{
				goto case 0;
			}
		case 62:
			if (ch == 'Q')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				goto case 0;
			}
		case 63:
			if (ch == 'U')
			{
				AddCh();
				goto case 64;
			}
			else
			{
				goto case 0;
			}
		case 64:
			if (ch == 'I')
			{
				AddCh();
				goto case 65;
			}
			else
			{
				goto case 0;
			}
		case 65:
			if (ch == 'R')
			{
				AddCh();
				goto case 66;
			}
			else
			{
				goto case 0;
			}
		case 66:
			if (ch == 'E')
			{
				AddCh();
				goto case 67;
			}
			else
			{
				goto case 0;
			}
		case 67:
			if (ch == 'D')
			{
				AddCh();
				goto case 68;
			}
			else
			{
				goto case 0;
			}
		case 68:
			{
				t.Type = 43;
				break;
			}
		case 69:
			if (ch == 'M')
			{
				AddCh();
				goto case 70;
			}
			else
			{
				goto case 0;
			}
		case 70:
			if (ch == 'P')
			{
				AddCh();
				goto case 71;
			}
			else
			{
				goto case 0;
			}
		case 71:
			if (ch == 'L')
			{
				AddCh();
				goto case 72;
			}
			else
			{
				goto case 0;
			}
		case 72:
			if (ch == 'I')
			{
				AddCh();
				goto case 73;
			}
			else
			{
				goto case 0;
			}
		case 73:
			if (ch == 'E')
			{
				AddCh();
				goto case 74;
			}
			else
			{
				goto case 0;
			}
		case 74:
			if (ch == 'D')
			{
				AddCh();
				goto case 75;
			}
			else
			{
				goto case 0;
			}
		case 75:
			{
				t.Type = 44;
				break;
			}
		case 76:
			if (ch == 'I')
			{
				AddCh();
				goto case 77;
			}
			else
			{
				goto case 0;
			}
		case 77:
			if (ch == 'X')
			{
				AddCh();
				goto case 78;
			}
			else
			{
				goto case 0;
			}
		case 78:
			if (ch == 'E')
			{
				AddCh();
				goto case 79;
			}
			else
			{
				goto case 0;
			}
		case 79:
			if (ch == 'D')
			{
				AddCh();
				goto case 80;
			}
			else
			{
				goto case 0;
			}
		case 80:
			{
				t.Type = 45;
				break;
			}
		case 81:
			if (ch == 'T')
			{
				AddCh();
				goto case 82;
			}
			else
			{
				goto case 0;
			}
		case 82:
			if (ch == 'I')
			{
				AddCh();
				goto case 83;
			}
			else
			{
				goto case 0;
			}
		case 83:
			if (ch == 'T')
			{
				AddCh();
				goto case 84;
			}
			else
			{
				goto case 0;
			}
		case 84:
			if (ch == 'Y')
			{
				AddCh();
				goto case 85;
			}
			else
			{
				goto case 0;
			}
		case 85:
			{
				t.Type = 46;
				break;
			}
		case 86:
			if (ch == '>')
			{
				AddCh();
				goto case 87;
			}
			else
			{
				goto case 0;
			}
		case 87:
			{
				t.Type = 49;
				break;
			}
		case 88:
			{
				t.Type = 50;
				break;
			}
		case 89:
			if (ch == 'C')
			{
				AddCh();
				goto case 90;
			}
			else
			{
				goto case 0;
			}
		case 90:
			if (ch == 'D')
			{
				AddCh();
				goto case 91;
			}
			else
			{
				goto case 0;
			}
		case 91:
			if (ch == 'A')
			{
				AddCh();
				goto case 92;
			}
			else
			{
				goto case 0;
			}
		case 92:
			if (ch == 'T')
			{
				AddCh();
				goto case 93;
			}
			else
			{
				goto case 0;
			}
		case 93:
			if (ch == 'A')
			{
				AddCh();
				goto case 94;
			}
			else
			{
				goto case 0;
			}
		case 94:
			if (ch == '[')
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
				t.Type = 52;
				break;
			}
		case 96:
			if (ch == '>')
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
				t.Type = 53;
				break;
			}
		case 98:
			recEnd = pos; recKind = 48;
			if (ch == '!')
			{
				AddCh();
				goto case 105;
			}
			else if (ch == '?')
			{
				AddCh();
				goto case 106;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 88;
			}
			else
			{
				t.Type = 48;
				break;
			}
		case 99:
			recEnd = pos; recKind = 7;
			if (!(ch == 39) && ch != EOF)
			{
				AddCh();
				goto case 7;
			}
			else if (ch == 39)
			{
				AddCh();
				goto case 8;
			}
			else
			{
				t.Type = 7;
				break;
			}
		case 100:
			recEnd = pos; recKind = 51;
			if (ch == '#')
			{
				AddCh();
				goto case 107;
			}
			else
			{
				t.Type = 51;
				break;
			}
		case 101:
			recEnd = pos; recKind = 29;
			if (ch == '>')
			{
				AddCh();
				goto case 16;
			}
			else
			{
				t.Type = 29;
				break;
			}
		case 102:
			recEnd = pos; recKind = 19;
			if (ch == ']')
			{
				AddCh();
				goto case 96;
			}
			else
			{
				t.Type = 19;
				break;
			}
		case 103:
			if (ch == 'P')
			{
				AddCh();
				goto case 34;
			}
			else if (ch == 'R')
			{
				AddCh();
				goto case 61;
			}
			else if (ch == 'I')
			{
				AddCh();
				goto case 69;
			}
			else if (ch == 'F')
			{
				AddCh();
				goto case 76;
			}
			else
			{
				goto case 0;
			}
		case 104:
			if ((ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 108;
			}
			else
			{
				goto case 0;
			}
		case 105:
			if (ch == '-')
			{
				AddCh();
				goto case 2;
			}
			else if (ch == 'D')
			{
				AddCh();
				goto case 17;
			}
			else if (ch == 'E')
			{
				AddCh();
				goto case 109;
			}
			else if (ch == 'N')
			{
				AddCh();
				goto case 46;
			}
			else if (ch == 'A')
			{
				AddCh();
				goto case 54;
			}
			else if (ch == '[')
			{
				AddCh();
				goto case 89;
			}
			else
			{
				goto case 0;
			}
		case 106:
			recEnd = pos; recKind = 10;
			if (ch == 'x')
			{
				AddCh();
				goto case 12;
			}
			else
			{
				t.Type = 10;
				break;
			}
		case 107:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 9;
			}
			else if (ch == 'x')
			{
				AddCh();
				goto case 10;
			}
			else
			{
				goto case 0;
			}
		case 108:
			if ((ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 5;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 4;
			}
			else
			{
				goto case 0;
			}
		case 109:
			if (ch == 'L')
			{
				AddCh();
				goto case 24;
			}
			else if (ch == 'N')
			{
				AddCh();
				goto case 81;
			}
			else
			{
				goto case 0;
			}
			}
		}
	} 
}
