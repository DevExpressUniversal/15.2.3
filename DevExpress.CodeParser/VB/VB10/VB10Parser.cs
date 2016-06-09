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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public class VB10Parser: VB90Parser
	{
	bool[] _ImplicitLineContinuationAfterTokenTable;
	bool[] _ImplicitLineContinuationBeforeTokenTable;
	public VB10Parser() : base()
	{
	  _ImplicitLineContinuationAfterTokenTable = new bool[Tokens.MaxTokens + 1];
	  _ImplicitLineContinuationBeforeTokenTable = new bool[Tokens.MaxTokens + 1];
	  FillImplicitLineContinuationBeforeTokenTable();
	  FillImplicitLineContinuationAfterTokenTable();
	}
	protected override void SetUpLineContinuationCheckState()
	{
	  EnableImplicitLineContinuationCheck();
	}
	void FillImplicitLineContinuationAfterTokenTable()
	{
	  int length = _ImplicitLineContinuationAfterTokenTable.Length;
	  for (int i = 0; i < length; i++)
		_ImplicitLineContinuationAfterTokenTable[i] = false;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Comma] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ParenOpen] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.CurlyBraceOpen] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Aggregate] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Distinct] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.From] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Let] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.By] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Join] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Where] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.In] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Into] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Ascending] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Descending] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.XorSymbol] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Asterisk] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Slash] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.BackSlash] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Mod] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Plus] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Minus] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.EqualsSymbol] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.XorEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.MulEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.DivEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.BackSlashEquals] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.PlusEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.MinusEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ShiftLeftEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ShiftRightEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.EqualsSymbol] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ColonEquals] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.AndEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.LessOrEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.GreaterOrEqual] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.NotEquals] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Is] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.IsNot] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Like] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.BitAnd] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.And] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Or] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Xor] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.AndAlso] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.OrElse] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ShiftLeft] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.ShiftRight] = true;
	  _ImplicitLineContinuationAfterTokenTable[Tokens.Dot] = true;
	}
	void FillImplicitLineContinuationBeforeTokenTable()
	{
	  int length = _ImplicitLineContinuationBeforeTokenTable.Length;
	  for (int i = 0; i < length; i++)
		_ImplicitLineContinuationBeforeTokenTable[i] = false;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.ParenClose] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.CurlyBraceClose] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Aggregate] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Distinct] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Group] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Let] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Order] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Skip] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Take] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Where] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.In] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Into] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.On] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Ascending] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Descending] = true;
	  _ImplicitLineContinuationBeforeTokenTable[Tokens.Join] = true;
	}
	bool IsImplicitLineContinuationAfterToken(Token token)
	{
	  if (token == null)
		return false;
	  int type = token.Type;
	  if (type > Tokens.MaxTokens || type < 0)
		return false;
	  return _ImplicitLineContinuationAfterTokenTable[type];
	}
	bool IsImplicitLineContinuationBeforeToken(Token token)
	{
	  if (token == null)
		return false;
	  int type = token.Type;
	  if (type > Tokens.MaxTokens || type < 0)
		return false;
	  bool result = _ImplicitLineContinuationBeforeTokenTable[type];
	  if (!result)
		return false;
	  if (type == Tokens.Order && Peek().Type != Tokens.By)
		return false;
	  return true;
	}
	protected override void Get()
	{
	  base.Get();
	  if (tToken.Match(Tokens.Then))
		return;
	  if (!CheckImplicitLineContinuations)
		return;
	  Token oldT = tToken; 
	  if (IsImplicitLineContinuationAfterToken(tToken))
		while (la.Match(Tokens.LineTerminator))
		  base.Get();
	  if (la.Type == Tokens.LineTerminator)
		if (IsImplicitLineContinuationBeforeToken(GetPeek()))
		  while (la.Type == Tokens.LineTerminator)
			base.Get();
	  tToken = oldT;
	}
	}
}
