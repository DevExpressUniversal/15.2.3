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
namespace DevExpress.CodeRush.StructuralParser.Xaml
#else
namespace DevExpress.CodeParser.Xaml
#endif
{
  partial class MarkupExtensionScanner
  {
		const int charSetSize = 272;
	const int UnicodeCharIndex = 257;
	const int UnicodeLetterIndex = 258;
	const int maxT = 13;
	const int noSym = 13;
	short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0, 10,  0,  0,  0,  0, 10,  0,  0,  0,  0,  2,  0,  1, 13,
	 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,  3,  0,  0,  4,  0,  0,
	  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
	  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  8,  0,  9,  0,  7,
	  0,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
	  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  5,  0,  6,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  7,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  7,  0,  0,  0,  0,  7,  0,  0,  0,  0,  0,
	  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
	  7,  7,  7,  7,  7,  7,  7,  0,  7,  7,  7,  7,  7,  7,  7,  7,
	  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,  7,
	  7,  7,  7,  7,  7,  7,  7,  0,  7,  7,  7,  7,  7,  7,  7,  7,
	  0,  0,  7,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};
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
		if (recKind != noSym)
		{
		  tlen = recEnd - t.StartPosition;
		  prevLineStart = recPrevLineStart;
		  BackTrackScannerToToken();
		}
		t.Type = recKind;
		break;
				case 1:
			{
				t.Type = 1;
				break;
			}
		case 2:
			{
				t.Type = 2;
				break;
			}
		case 3:
			{
				t.Type = 3;
				break;
			}
		case 4:
			{
				t.Type = 4;
				break;
			}
		case 5:
			{
				t.Type = 5;
				break;
			}
		case 6:
			{
				t.Type = 6;
				break;
			}
		case 7:
			if ((ch >= '0' && ch <= '9' || ch >= '@' && ch <= 'Z' || ch == '_' || ch >= 'a' && ch <= 'z' || ch == 170 || ch == 181 || ch == 186 || ch >= 192 && ch <= 214 || ch >= 216 && ch <= 246 || ch >= 248 && ch <= 255 || ch == 258) || char.IsLetter(ch))
			{
				AddCh();
				goto case 7;
			}
			else
			{
				t.Type = 7;
				break;
			}
		case 8:
			{
				t.Type = 8;
				break;
			}
		case 9:
			{
				t.Type = 9;
				break;
			}
		case 10:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '!' || ch >= '#' && ch <= '&' || ch >= '(' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 10;
			}
			else if ((ch == '"' || ch == 39))
			{
				AddCh();
				goto case 11;
			}
			else
			{
				t.Type = noSym;
				break;
			}
		case 11:
			{
				t.Type = 10;
				break;
			}
		case 12:
			if ((ch >= '0' && ch <= '9'))
			{
				AddCh();
				goto case 12;
			}
			else
			{
				t.Type = 11;
				break;
			}
		case 13:
			if ((ch <= 9 || ch >= 11 && ch <= 12 || ch >= 14 && ch <= '|' || ch >= '~' && ch <= 271) ||( ch > 255 && ch != EOF))
			{
				AddCh();
				goto case 13;
			}
			else
			{
				t.Type = 12;
				break;
			}
	  }
	}
  } 
}
