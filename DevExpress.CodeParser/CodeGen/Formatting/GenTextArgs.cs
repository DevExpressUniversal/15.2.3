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
using System.Globalization;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class GenTextArgs
  {
	FormattingTokenType _Type = FormattingTokenType.None;
	int _Index = 0;
	string _Text = null;
	bool _SplitLines = true;
	CodeGen _CodeGen = null;
	int _ContextsIndex = -1;
	public GenTextArgs(CodeGen codeGen, string tokenText, bool split)
	  : this(codeGen, FormattingTokenType.None, 0, tokenText)
	{
	  _SplitLines = split;
	}
	public GenTextArgs(CodeGen codeGen, FormattingTokenType tokenType, int index)
	  : this(codeGen, tokenType, index, null)
	{
	}
	public GenTextArgs(CodeGen codeGen, FormattingTokenType tokenType, int index, string tokenText)
	  : this(codeGen, codeGen.Contexts.Count - 1, tokenType, index, tokenText)
	{
	}
	public GenTextArgs(CodeGen codeGen, int contextsIndex, FormattingTokenType tokenType, int index, string tokenText)
	{
	  _CodeGen = codeGen;
	  _ContextsIndex = contextsIndex;
	  _Text = tokenText;
	  Type = tokenType;
	  _Index = index;
	}
	public bool SplitLines
	{
	  get { return _SplitLines; }
	  set { _SplitLines = value; }
	}
	public int Index
	{
	  get { return _Index; }
	  set { _Index = value; }
	}
	public string Text
	{
	  get { return _Text; }
	  set { _Text = value; }
	}
	public FormattingTokenType Type
	{
	  get { return _Type; }
	  set 
	  {
		_Type = value;
		SetTokenText(value);
	  }
	}
	public int ContextsIndex
	{
	  get { return _ContextsIndex; }
	  set { _ContextsIndex = value; }
	}
	public bool IsEmpty
	{
	  get { return Type == FormattingTokenType.None && Text == null ; }
	}
	void SetTokenText(FormattingTokenType type)
	{
	  if (_CodeGen == null || _Text != null)
		return;
	  if (type == FormattingTokenType.None)
	  {
		_Text = String.Empty;
		return;
	  }
	  _Text = _CodeGen.GetTokenText(type, _ContextsIndex);
	}
	public LanguageElement Context
	{
	  get
	  {
		if (_CodeGen == null)
		  return null;
		return _CodeGen.Context;
	  }
	}
  }
}
