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
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public class IndentionFormattingOptions : FormattingOptions
  {
	bool _OpenAndCloseBraces;
	bool _CodeBlockContents;
	bool _AnonymousMethodContents;
	bool _ArrayObjectAndCollectionInitializerContents;
	bool _CaseStatementFromSwitchStatement;
	bool _CaseStatementContents;
	bool _IndentLabels;
	bool _IndentComment;
	bool _IndentNestedUsingStatements;
	public override void Load(FormattingRuleCollection rules)
	{
	  _OpenAndCloseBraces = GetOptionByName(Rules.Indentation.OpenAndCloseBraces, rules);
	  _CodeBlockContents = GetOptionByName(Rules.Indentation.CodeBlockContents, rules);
	  _AnonymousMethodContents = GetOptionByName(Rules.Indentation.AnonymousMethodContents, rules);
	  _ArrayObjectAndCollectionInitializerContents = GetOptionByName(Rules.Indentation.ArrayObjectAndCollectionInitializerContents, rules);
	  _CaseStatementFromSwitchStatement = GetOptionByName(Rules.Indentation.CaseStatementFromSwitchStatement, rules);
	  _CaseStatementContents = GetOptionByName(Rules.Indentation.CaseStatementContents, rules);
	  _IndentLabels = GetOptionByName(Rules.Indentation.IndentLabels, rules);
	  _IndentComment = GetOptionByName(Rules.Indentation.IndentComments, rules);
	  _IndentNestedUsingStatements = GetOptionByName(Rules.Indentation.NestedUsingStatements, rules);
	}
	public override void LoadDefault(ParserLanguageID language)
	{
	  _AnonymousMethodContents = true;
	  _CodeBlockContents = true;
	  _CaseStatementContents = true;
	  _CaseStatementFromSwitchStatement = true;
	  _IndentNestedUsingStatements = true;
	  _IndentComment = true;
	}
	public bool OpenAndCloseBraces
	{
	  get
	  {
		return _OpenAndCloseBraces;
	  }
	}
	public bool CodeBlockContents
	{
	  get
	  {
		return _CodeBlockContents;
	  }
	}
	public bool AnonymousMethodContents
	{
	  get
	  {
		return _AnonymousMethodContents;
	  }
	}
	public bool ArrayObjectAndCollectionInitializerContents
	{
	  get
	  {
		return _ArrayObjectAndCollectionInitializerContents;
	  }
	}
	public bool CaseStatementFromSwitchStatement
	{
	  get
	  {
		return _CaseStatementFromSwitchStatement;
	  }
	}
	public bool CaseStatementContents
	{
	  get
	  {
		return _CaseStatementContents;
	  }
	}
	public bool IndentLabels
	{
	  get
	  {
		return _IndentLabels;
	  }
	}
	public bool IndentComment
	{
	  get
	  {
		return _IndentComment;
	  }
	}
	public bool IndentNestedUsingStatements
	{
	  get
	  {
		return _IndentNestedUsingStatements;
	  }
	}
  }
}
