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
using System.Collections;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class FormattingParserBase : TokenCategorizedParserBase
  {
	bool _SaveFormat;
	FormattingParsingElement _LastParsingElement = null;
	FormattingParsingElementCollection _FormattingParsingElements;
	FormattingParsingElement AddFormattingParsingElement(FormattingToken token)
	{
	  if (token == null || token.WasChecked)
		return null;
	  FormattingTokenType tokenType = GetTokenType(token);
	  token.WasChecked = true;
	  return AddFormattingParsingElement(token.Range, new FormattingParsingElement(tokenType, token.FormattingElements));
	}
	FormattingParsingElement AddFormattingParsingElement(SourceRange range, FormattingParsingElement element)
	{
	  if (_FormattingParsingElements == null)
		_FormattingParsingElements = new FormattingParsingElementCollection();
	  _FormattingParsingElements.Add(range, element);
	  return element;
	}
	internal void AddToParsingElement(FormattingParsingElement parsingElement, FormattingElements formattingElements)
	{
	  if (parsingElement == null || formattingElements == null)
		return;
	  parsingElement.AddFormattingElements(formattingElements);
	}
	protected FormattingTokenType GetTokenType(FormattingToken token)
	{
	  if (token.Column == -1)
		return FormattingTokenType.SourceFileStart;
	  string tokenText = token.Value;
	  if (String.IsNullOrEmpty(tokenText))
		return FormattingTokenType.None;
	  if (IsIdent(token))
		return FormattingTokenType.Ident;
	  return GetFormattingTokenType(tokenText);
	}
	protected FormattingTokenType GetFormattingTokenType(string tokenText)
	{
	  return FormattingTable.Get(tokenText.Trim());
	}
	protected FormattingParsingElement ReplaceOrAddFormattingParsingElement(FormattingToken token, FormattingTokenType type)
	{
	  if(token == null)
		return null;
	  if(type == FormattingTokenType.None)
		type = GetFormattingTokenType(token.Value);
	  if (_FormattingParsingElements == null)
		_FormattingParsingElements = new FormattingParsingElementCollection();
	  FormattingParsingElement element = new FormattingParsingElement(type, token.FormattingElements);
	  _FormattingParsingElements.ReplaceOrAdd(token.Range, element);
	  return element;
	}
	protected virtual void AfterParsingWithClearing(LanguageElement element)
	{
	  AfterParsing(element);
	  ClearFormattingParsingElements();
	}
	protected virtual void AfterParsing()
	{
	  AfterParsing(RootNode);
	  ClearFormattingParsingElements();
	}
	protected virtual void AfterParsing(LanguageElementCollectionBase coll)
	{
	  if (coll == null || !SaveFormat)
		return;
	  foreach (LanguageElement element in coll)
		AfterParsing(element);
	  ClearFormattingParsingElements();
	}
	protected virtual void AfterParsing(LanguageElement element)
	{
	  if (element == null || !SaveFormat)
		return;
	  SourceFile file = element as SourceFile;
	  if (file == null)
		return;
	  file.CompilerDirectiveRootNode.FillFormattingCollectionWithChildren(_FormattingParsingElements);
	  file.SimpleDirectiveHolder.FillFormattingCollectionWithChildren(_FormattingParsingElements);
	  element.FillFormattingCollectionWithChildren(_FormattingParsingElements);
	}
	protected void ClearFormattingParsingElements()
	{
	  if (_FormattingParsingElements == null)
		return;
	  _FormattingParsingElements.Clear();
	}
	protected override void Get()
	{
	  if (SaveFormat)
		LastParsingElement = AddFormattingParsingElement(la as FormattingToken);
	  base.Get();
	}
	protected override void PrepareForParse(ParserContext parserContext)
	{
	  base.PrepareForParse(parserContext);
	  SaveFormat = parserContext.SaveUserFormat;
	}
	protected override void CleanUpInternalData()
	{
	  base.CleanUpInternalData();
	  SaveFormat = false;
	}
	protected override void CleanUpParser()
	{
	  base.CleanUpParser();
	  SaveFormat = false;
	}
	protected virtual bool IsIdent(Token token)
	{
	  return false;
	}
	internal virtual void AddCommentNode(Token lCommentToken, FormattingParsingElement lastElement)
	{
	}
	public FormattingParsingElement AddFormattingElements(FormattingToken token)
	{
	  if (token == null || !SaveFormat)
		return null;
	  return AddFormattingParsingElement(token);
	}
	public void AddToParsingElement(FormattingParsingElement parsingElement, IFormattingElement formattingElement)
	{
	  if (formattingElement == null || parsingElement == null || !SaveFormat)
		return;
	  parsingElement.AddFormattingElement(formattingElement);
	}
	internal FormattingParsingElement LastFormattingParsingElement
	{
	  get
	  {
		if (_FormattingParsingElements == null)
		  return null;
		return _FormattingParsingElements.LastElement;
	  }
	}
	protected virtual FormattingTable FormattingTable
	{
	  get
	  {
		return CSharpFormattingTable.Instance;
	  }
	}
	protected FormattingParsingElementCollection FormattingParsingElements
	{
	  get
	  {
		return _FormattingParsingElements;
	  }
	}
	protected FormattingParsingElement LastParsingElement
	{
	  get
	  {
		return _LastParsingElement;
	  }
	  set
	  {
		if (value == null)
		  return;
		_LastParsingElement = value;
	  }
	}
	public bool SaveFormat
	{
	  get
	  {
		return _SaveFormat;
	  }
	  set
	  {
		_SaveFormat = value;
		if (scanner != null)
		  scanner.SaveFormat = value;
	  }
	}
  }
}
