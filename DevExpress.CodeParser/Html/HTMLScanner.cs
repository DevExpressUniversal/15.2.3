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
namespace DevExpress.CodeRush.StructuralParser.Html
#else
namespace DevExpress.CodeParser.Html
#endif
{
	using Xml;
	public class HtmlScanner : HtmlScannerBase
	{	
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 53;
	const int noSym = 53;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0, 14, 71,  0, 69, 67, 13, 21, 22, 32, 33, 31,  1,  1, 54,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 30, 66, 12, 19, 68,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 20,  0, 70,  0,  1,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0, 29,  0,  0,  0,
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
		public HtmlScanner(ISourceReader s) : this(s, false)
		{
		}
	public HtmlScanner(ISourceReader s, bool isRazor)
		{
	  IsRazor = isRazor;
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
		ignore[160] = true;
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
			case "SYSTEM": t.Type = 13; break;
			case "PUBLIC": t.Type = 14; break;
			case "DOCTYPE": t.Type = 16; break;
			case "ELEMENT": t.Type = 17; break;
			case "EMPTY": t.Type = 21; break;
			case "ANY": t.Type = 22; break;
			case "ATTLIST": t.Type = 33; break;
			case "ID": t.Type = 34; break;
			case "CDATA": t.Type = 35; break;
			case "IDREF": t.Type = 36; break;
			case "IDREFS": t.Type = 37; break;
			case "ENTITY": t.Type = 38; break;
			case "ENTITIES": t.Type = 39; break;
			case "NMTOKEN": t.Type = 40; break;
			case "NMTOKENS": t.Type = 41; break;
			case "NOTATION": t.Type = 42; break;
			case "NDATA": t.Type = 46; break;
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
				goto case 72;
			}
			else
			{
				goto case 0;
			}
		case 4:
			{
				t.Type = 2;
				break;
			}
		case 5:
			if (ch == '-')
			{
				AddCh();
				goto case 6;
			}
			else
			{
				goto case 0;
			}
		case 6:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 74;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 7;
			}
			else
			{
				goto case 0;
			}
		case 7:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 6;
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
			{
				t.Type = 5;
				break;
			}
		case 13:
			{
				t.Type = 6;
				break;
			}
		case 14:
			{
				t.Type = 7;
				break;
			}
		case 15:
			{
				t.Type = 8;
				break;
			}
		case 16:
			{
				t.Type = 9;
				break;
			}
		case 17:
			{
				t.Type = 11;
				break;
			}
		case 18:
			{
				t.Type = 12;
				break;
			}
		case 19:
			{
				t.Type = 18;
				break;
			}
		case 20:
			{
				t.Type = 19;
				break;
			}
		case 21:
			{
				t.Type = 23;
				break;
			}
		case 22:
			{
				t.Type = 24;
				break;
			}
		case 23:
			if (ch == 'C')
			{
				AddCh();
				goto case 24;
			}
			else
			{
				goto case 0;
			}
		case 24:
			if (ch == 'D')
			{
				AddCh();
				goto case 25;
			}
			else
			{
				goto case 0;
			}
		case 25:
			if (ch == 'A')
			{
				AddCh();
				goto case 26;
			}
			else
			{
				goto case 0;
			}
		case 26:
			if (ch == 'T')
			{
				AddCh();
				goto case 27;
			}
			else
			{
				goto case 0;
			}
		case 27:
			if (ch == 'A')
			{
				AddCh();
				goto case 28;
			}
			else
			{
				goto case 0;
			}
		case 28:
			{
				t.Type = 25;
				break;
			}
		case 29:
			{
				t.Type = 26;
				break;
			}
		case 30:
			{
				t.Type = 28;
				break;
			}
		case 31:
			{
				t.Type = 29;
				break;
			}
		case 32:
			{
				t.Type = 31;
				break;
			}
		case 33:
			{
				t.Type = 32;
				break;
			}
		case 34:
			if (ch == 'E')
			{
				AddCh();
				goto case 35;
			}
			else
			{
				goto case 0;
			}
		case 35:
			if (ch == 'Q')
			{
				AddCh();
				goto case 36;
			}
			else
			{
				goto case 0;
			}
		case 36:
			if (ch == 'U')
			{
				AddCh();
				goto case 37;
			}
			else
			{
				goto case 0;
			}
		case 37:
			if (ch == 'I')
			{
				AddCh();
				goto case 38;
			}
			else
			{
				goto case 0;
			}
		case 38:
			if (ch == 'R')
			{
				AddCh();
				goto case 39;
			}
			else
			{
				goto case 0;
			}
		case 39:
			if (ch == 'E')
			{
				AddCh();
				goto case 40;
			}
			else
			{
				goto case 0;
			}
		case 40:
			if (ch == 'D')
			{
				AddCh();
				goto case 41;
			}
			else
			{
				goto case 0;
			}
		case 41:
			{
				t.Type = 43;
				break;
			}
		case 42:
			if (ch == 'M')
			{
				AddCh();
				goto case 43;
			}
			else
			{
				goto case 0;
			}
		case 43:
			if (ch == 'P')
			{
				AddCh();
				goto case 44;
			}
			else
			{
				goto case 0;
			}
		case 44:
			if (ch == 'L')
			{
				AddCh();
				goto case 45;
			}
			else
			{
				goto case 0;
			}
		case 45:
			if (ch == 'I')
			{
				AddCh();
				goto case 46;
			}
			else
			{
				goto case 0;
			}
		case 46:
			if (ch == 'E')
			{
				AddCh();
				goto case 47;
			}
			else
			{
				goto case 0;
			}
		case 47:
			if (ch == 'D')
			{
				AddCh();
				goto case 48;
			}
			else
			{
				goto case 0;
			}
		case 48:
			{
				t.Type = 44;
				break;
			}
		case 49:
			if (ch == 'I')
			{
				AddCh();
				goto case 50;
			}
			else
			{
				goto case 0;
			}
		case 50:
			if (ch == 'X')
			{
				AddCh();
				goto case 51;
			}
			else
			{
				goto case 0;
			}
		case 51:
			if (ch == 'E')
			{
				AddCh();
				goto case 52;
			}
			else
			{
				goto case 0;
			}
		case 52:
			if (ch == 'D')
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
				t.Type = 45;
				break;
			}
		case 54:
			if (ch == '>')
			{
				AddCh();
				goto case 55;
			}
			else
			{
				goto case 0;
			}
		case 55:
			{
				t.Type = 48;
				break;
			}
		case 56:
			{
				t.Type = 49;
				break;
			}
		case 57:
			if (ch == 'C')
			{
				AddCh();
				goto case 58;
			}
			else
			{
				goto case 0;
			}
		case 58:
			if (ch == 'D')
			{
				AddCh();
				goto case 59;
			}
			else
			{
				goto case 0;
			}
		case 59:
			if (ch == 'A')
			{
				AddCh();
				goto case 60;
			}
			else
			{
				goto case 0;
			}
		case 60:
			if (ch == 'T')
			{
				AddCh();
				goto case 61;
			}
			else
			{
				goto case 0;
			}
		case 61:
			if (ch == 'A')
			{
				AddCh();
				goto case 62;
			}
			else
			{
				goto case 0;
			}
		case 62:
			if (ch == '[')
			{
				AddCh();
				goto case 63;
			}
			else
			{
				goto case 0;
			}
		case 63:
			{
				t.Type = 51;
				break;
			}
		case 64:
			if (ch == '>')
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
				t.Type = 52;
				break;
			}
		case 66:
			recEnd = pos; recKind = 47;
			if (ch == '!')
			{
				AddCh();
				goto case 75;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 76;
			}
			else if (ch == '?')
			{
				AddCh();
				goto case 15;
			}
			else if (ch == '/')
			{
				AddCh();
				goto case 56;
			}
			else
			{
				t.Type = 47;
				break;
			}
		case 67:
			recEnd = pos; recKind = 50;
			if (ch == '#')
			{
				AddCh();
				goto case 77;
			}
			else
			{
				t.Type = 50;
				break;
			}
		case 68:
			recEnd = pos; recKind = 30;
			if (ch == '>')
			{
				AddCh();
				goto case 16;
			}
			else
			{
				t.Type = 30;
				break;
			}
		case 69:
			recEnd = pos; recKind = 27;
			if (ch == '>')
			{
				AddCh();
				goto case 17;
			}
			else
			{
				t.Type = 27;
				break;
			}
		case 70:
			recEnd = pos; recKind = 20;
			if (ch == ']')
			{
				AddCh();
				goto case 64;
			}
			else
			{
				t.Type = 20;
				break;
			}
		case 71:
			if (ch == 'P')
			{
				AddCh();
				goto case 23;
			}
			else if (ch == 'R')
			{
				AddCh();
				goto case 34;
			}
			else if (ch == 'I')
			{
				AddCh();
				goto case 42;
			}
			else if (ch == 'F')
			{
				AddCh();
				goto case 49;
			}
			else
			{
				goto case 0;
			}
		case 72:
			if ((ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 3;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 4;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 72;
			}
			else
			{
				goto case 0;
			}
		case 73:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 74;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 78;
			}
			else
			{
				goto case 0;
			}
		case 74:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 79;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 7;
			}
			else
			{
				goto case 0;
			}
		case 75:
			recEnd = pos; recKind = 15;
			if (ch == '-')
			{
				AddCh();
				goto case 2;
			}
			else if (ch == '[')
			{
				AddCh();
				goto case 57;
			}
			else
			{
				t.Type = 15;
				break;
			}
		case 76:
			recEnd = pos; recKind = 10;
			if (ch == '-')
			{
				AddCh();
				goto case 5;
			}
			else if (ch == '@')
			{
				AddCh();
				goto case 18;
			}
			else
			{
				t.Type = 10;
				break;
			}
		case 77:
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
		case 78:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 74;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 7;
			}
			else
			{
				goto case 0;
			}
		case 79:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 73;
			}
			else if (ch == '-')
			{
				AddCh();
				goto case 79;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '%')
			{
				AddCh();
				goto case 80;
			}
			else
			{
				goto case 0;
			}
		case 80:
			if ((ch <= '$' || ch >= '&' && ch <= ',' || ch >= '.' && ch <= '=' || ch >= '?' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 6;
			}
			else if (ch == '>')
			{
				AddCh();
				goto case 8;
			}
			else
			{
				goto case 0;
			}
			}
		}
	} 
}
