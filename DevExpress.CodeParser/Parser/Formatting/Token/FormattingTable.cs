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

using System.Collections.Generic;
using System;
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class FormattingTable
  {
	Dictionary<string, FormattingTokenType> _FormattingTokenTypeCorrespondanceTable;
	Dictionary<FormattingTokenType, string> _FormattingTokenTypeCorrespondanceReversedTable;
	Dictionary<FormattingTokenType, string> _OnlyGetValueTable;
	internal FormattingTable()
	{
	  _FormattingTokenTypeCorrespondanceTable = new Dictionary<string, FormattingTokenType>();
	  _FormattingTokenTypeCorrespondanceReversedTable = new Dictionary<FormattingTokenType, string>();
	  _OnlyGetValueTable = new Dictionary<FormattingTokenType, string>();
	  FillFormattingTokenTypeCorrespondanceTable();
	}
	protected abstract void FillFormattingTokenTypeCorrespondanceTable();
	protected void AddToOnlyGetValue(FormattingTokenType type, string text)
	{
	  OnlyGetValueTable.Add(type, text);
	}
	protected void AddToken(FormattingTokenType type, string text)
	{
	  FormattingTokenTypeCorrespondanceTable.Add(text, type);
	  FormattingTokenTypeCorrespondanceReversedTable.Add(type, text);
	}
	public virtual string Get(FormattingTokenType type)
	{
	  string result = String.Empty;
	  if (OnlyGetValueTable.TryGetValue(type, out result))
		return result;
	  if (FormattingTokenTypeCorrespondanceReversedTable.TryGetValue(type, out result))
		return result;
	  return String.Empty;
	}
	public virtual FormattingTokenType Get(string text)
	{
	  FormattingTokenType result = FormattingTokenType.None;
	  if (FormattingTokenTypeCorrespondanceTable.TryGetValue(text, out result))
		return result;
	  return FormattingTokenType.Ident;
	}
	protected Dictionary<string, FormattingTokenType> FormattingTokenTypeCorrespondanceTable
	{
	  get { return _FormattingTokenTypeCorrespondanceTable; }
	}
	protected Dictionary<FormattingTokenType, string> FormattingTokenTypeCorrespondanceReversedTable
	{
	  get { return _FormattingTokenTypeCorrespondanceReversedTable; }
	}
	protected Dictionary<FormattingTokenType, string> OnlyGetValueTable
	{
	  get { return _OnlyGetValueTable; }
	}
  }
}
