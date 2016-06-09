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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Css
#else
namespace DevExpress.CodeParser.Css
#endif
{
  public partial class CssScanner : GeneratedScannerBase
  {
	public CssScanner(ISourceReader s)
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
	protected override Token CreateToken()
	{
	  return new CategorizedToken(TokenLanguage.Css);
	}
	protected override void NextTokenAfterScan()
	{
	  if (t.Type == Tokens.FUNCTION && t.Value == "url(")
	  {
		ReadURIText();
		t.Value = new String(tval, 0, tlen);
		t.Type = Tokens.URI;
	  }
	}
	void ReadURIText()
	{
	  SkipIgnoredChars();
	  if (ch == '"')
	  {
		AddCh();
		while (ch != EOF)
		{
		  if (ch == '"')
		  {
			AddCh();
			break;
		  }
		  AddCh();
		}
	  }
	  else
		if (ch == '\'')
		{
		  AddCh();
		  while (ch != EOF)
		  {
			if (ch == '\'')
			{
			  AddCh();
			  break;
			}
			AddCh();
		  }
		}
		else
		{
		  int parenCount = 1;
		  if (ch == ')')
		  {
			AddCh();
			return;
		  }
		  while (ch != EOF)
		  {
			AddCh();
			if (ch == '(')
			  parenCount++;
			if (ch == ')')
			  parenCount--;
			if (parenCount == 0)
			{
			  AddCh();
			  return;
			}
		  }
		}
	  SkipIgnoredChars();
	  if (ch == ')')
		AddCh();
	}
  }
}
