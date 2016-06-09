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
  public class FormattingTokenGen
  {
	CodeGen _CodeGen;
	TokenGenArgs _TokenGenArgs;
	LanguageElementCodeGenBase _ElementCodeGen;
	FormattingElements _FormattingElementsForSupportElement;
	IFormattingElement _LastGeneratedFormattingElement = null;
	bool _NeedIncreaseAlignment = false;
	bool _GeneratingInSupportElement = false;
	public FormattingTokenGen(CodeGen codeGen)
	{
	  _CodeGen = codeGen;
	}
	public bool TokenArgsContainsElement(object element)
	{
	  if (element == null)
		return false;
	  if (TokenGenArgs.IsEmpty)
		return false;
	  return TokenGenArgs.Contains(element);
	}
	FormattingElements GetPrevFormattingElements(FormattingTokenType tokenType)
	{
	  if (CodeGen.SaveFormat)
		return null;
	  FormattingElements result = null;
	  if (ElementCodeGen != null)
		result = ElementCodeGen.PrevFormattingElements(tokenType);
	  else
		result = CodeGen.PrevFormattingElements(tokenType);
	  if (!CodeGen.SaveFormat)
		CleanupElements(result);
	  return result;
	}
	void ClearUserFormattingElements(FormattingElements formattingElements)
	{
	  if (CodeGen.SaveFormat || formattingElements == null)
		return;
	  FormattingElements elementsToRemove = new FormattingElements();
	  int count = formattingElements.Count;
	  FormattingElement eol = null;
	  for (int i = 0; i < count; i++)
	  {
		object currentElement = formattingElements[i];
		if (currentElement == null)
		  continue;
		FormattingElement fte = currentElement as FormattingElement;
		if (fte != null)
		{
		  if (fte.IsEol())
		  {
			eol = formattingElements.GetLastEOLElement(i);
			break;
		  }
		  else
		  {
			if (fte.IsEol() && CodeGen.Options.General.KeepExistingLineBreaks ||
			  fte.IsWsOrTab() && CodeGen.Options.General.KeepExistingWhiteSpace)
			  continue;
			elementsToRemove.Add(fte);
		  }
		}
		if (currentElement is Comment)
		  elementsToRemove.Clear();
	  }
	  formattingElements.RemoveRange(elementsToRemove);
	  if (formattingElements.Count == 0)
		return;
	  if (formattingElements.HasOnlyNewLine())
	  {
		if (!CodeGen.Options.General.KeepExistingLineBreaks)
		  formattingElements.RemoveFirstEOL();
		return;
	  }
	  count = formattingElements.Count;
	  int lastIndex = 0;
	  if (eol != null)
		lastIndex = formattingElements.IndexOf(eol);
	  if (count - 1 - lastIndex <= 0)
		return;
	  bool nextIsNotFE = false;
	  for (int i = count - 1; i >= lastIndex; i--)
	  {
		FormattingElement fte = formattingElements[i] as FormattingElement;
		if (fte == null)
		  continue;
		int nextIndex = i + 1;
		nextIsNotFE = nextIndex >= formattingElements.Count || !(formattingElements[nextIndex] is FormattingElement);
		if ((fte.IsEol() && CodeGen.Options.General.KeepExistingLineBreaks) || (fte.IsWsOrTab() && CodeGen.Options.General.KeepExistingWhiteSpace))
		{
		  if (i < formattingElements.Count - 1)
		  {
			FormattingElement previous = formattingElements[i + 1] as FormattingElement;
			if (previous != null && previous.Type == fte.Type)
			{
			  previous.CountConsecutive++;
			  formattingElements.RemoveAt(i);
			}
		  }
		  continue;
		}
		if (fte.IsEol() && nextIsNotFE)
			continue;
		int prevIndex = i - 1;
		bool prevIsNotFE = prevIndex < 0 || !(formattingElements[prevIndex] is FormattingElement);
		if (fte.IsEol() && prevIsNotFE)
		  continue;
		if (fte.IsWsOrTab())
		{
		  if (prevIndex >= 0 && formattingElements[prevIndex] is Comment)
			continue;
		  if (prevIsNotFE && nextIsNotFE)
			continue;
		}
		formattingElements.RemoveAt(i);
	  }
	  if (formattingElements.HasOnlyNewLine() && !CodeGen.Options.General.KeepExistingLineBreaks)
		formattingElements.RemoveFirstEOL();
	}
	FormattingElements ProcessStartToken(FormattingTokenType tokenType, FormattingElements list)
	{
	  if (tokenType != FormattingTokenType.SourceFileStart || list == null)
		return list;
	  if (list.Count > 0)
	  {
		SourceFileStartText fileStartText = list[0] as SourceFileStartText;
		if (fileStartText != null)
		  list.RemoveAt(0);
	  }
	  return list;
	}
	public void WritePrevFormattingElements(FormattingTokenType type, int index)
	{
	  FormattingElements elements = GetPrevFormattingElements(type);	  
	  if (!TokenGenArgs.IsEmpty)
		elements = MergeFormattingElements(FormattingElements, elements);
	  TokenGenArgs.Reset();
	  WriteFormattingElements(elements);
	}
	public void WriteNextFormattingElements(FormattingTokenType type, int index)
	{
	  LanguageElement element = CodeGen.Context;
	  if (element == null)
		return;
	  TokenGenArgs.Reset();
	  FormattingElements nextElements = !CodeGen.SaveFormat ? ElementCodeGen.NextFormattingElements(type) : new FormattingElements();
	  if (nextElements != null)
		TokenGenArgs.SetFormattingElements(nextElements);
	  FormattingElements userTokens = GetUserFormattingElements(element, type, index);
	  if (userTokens != null)
		TokenGenArgs.SetUserFormattingElements(userTokens);
	}
	void WrapLongTokenTextIfNeeded(GenTextArgs args, bool lastEOL)
	{
	  string tokenText = args.Text;
	  if (!CodeGen.Options.WrappingAlignment.WrapLongLines || String.IsNullOrEmpty(tokenText) || lastEOL)
		return;
	  int lastLineLength = CodeGen.Code.LastLine.Length;
	  if (lastLineLength == 0)
		return;
	  int length = tokenText.Length + lastLineLength;
	  if (length <= 0)
		return;
	  int maxVal = CodeGen.Options.WrappingAlignment.MaxLineLength;
	  if (length <= maxVal)
		return;
	  FormattingElementsWriter.WriteEOL(CodeGen);
	}
	public void WriteTokenText(GenTextArgs args)
	{
	  CodeGen.WriteText(args);
	}
	FormattingElements MergePrevTokenTextElements(FormattingElements prevArgsElements, FormattingElements prevTextElements)
	{
	  if (prevTextElements == null || prevTextElements.Count == 0)
		return prevArgsElements;
	  if (prevArgsElements == null || prevArgsElements.Count == 0)
		return prevTextElements;
	  int count = prevArgsElements.Count;
	  FormattingElement tokenElement = null;
	  for (int i = 1; i <= count; i++)
	  {
		tokenElement = prevArgsElements[count - i] as FormattingElement;
		if (tokenElement != null && tokenElement.IsIndent())
		  continue;
	  }
	  FormattingElement prevTokenElement = prevTextElements[0] as FormattingElement;
	  if (tokenElement != null && prevTokenElement != null && tokenElement.Type == prevTokenElement.Type)
		tokenElement.CountConsecutive++;
	  return MergeFormattingElements(prevArgsElements, prevTextElements);
	}
	FormattingElements MergeNextTokenTextElements(FormattingElements nextArgsElements, FormattingElements nextTextElements)
	{
	  if (nextTextElements == null || nextTextElements.Count == 0)
		return nextArgsElements;
	  if (nextArgsElements == null || nextArgsElements.Count == 0)
		return nextTextElements;
	  FormattingElement nextTokenElement = nextTextElements[nextTextElements.Count - 1] as FormattingElement;
	  FormattingElement tokenElement = null;
	  int count = nextArgsElements.Count - 1;
	  for (int i = 0; i < count; i++)
	  {
		tokenElement = nextArgsElements[i] as FormattingElement;
		if (tokenElement != null && tokenElement.IsIndent())
		  continue;
	  }
	  if (tokenElement != null && nextTokenElement != null && tokenElement.Type == nextTokenElement.Type)
		tokenElement.CountConsecutive++;
	  return MergeFormattingElements(nextTextElements, nextArgsElements);
	}
	bool SkipFormattingElement(object element)
	{
	  if (element == null)
		return false;
	  return !(element is PreprocessorDirective);
	}
	protected FormattingElements GetFormattingElementsForRemovedElement(FormattingElements elements)
	{
	  if (elements == null)
		return null;
	  int count = elements.Count;
	  FormattingElements filtredElements = null;
	  for (int i = 0; i < count; i++)
	  {
		IFormattingElement element = elements[i];
		if (SkipFormattingElement(element))
		  continue;
		if (filtredElements == null)
		  filtredElements = new FormattingElements();
		filtredElements.Add(element);
		filtredElements.Add(new FormattingElement(FormattingElementType.EOL));
	  }
	  return filtredElements;
	}
	void WriteRemovedElement(GenTextArgs args)
	{
	  if (!TokenGenArgs.IsEmpty)
	  {
		FormattingElements list = TokenGenArgs.UserFormattingElements;
		list = GetFormattingElementsForRemovedElement(list);
		TokenGenArgs.UserFormattingElements.Clear();
		WriteFormattingElements(list);
	  }
	  FormattingElements elements = GetUserFormattingElements(CodeGen.Context, args.Type, args.Index);
	  TokenGenArgs.SetUserFormattingElements(elements);
	}
	public void WriteCode(GenTextArgs args)
	{
	  if (args == null || args.IsEmpty)
		return;
	  LanguageElement element = null;
	  if (CodeGen != null && CodeGen.Contexts != null && args.ContextsIndex >= 0 && args.ContextsIndex < CodeGen.Contexts.Count)
		element = CodeGen.Contexts[args.ContextsIndex];
	  if (element != null && element.IsRemoved)
	  {
		WriteRemovedElement(args);
		return;
	  }
	  FormattingTokenType tokenType = args.Type;
	  FormattingElements prevElementsInText = null;
	  FormattingElements nextElementsInText = null;
	  if (FormattingHelper.CanProcessTextToken(args))
		FormattingHelper.ProcessTokenText(args, out prevElementsInText, out nextElementsInText);
	  FormattingElements elements = GetPrevFormattingElements(tokenType);
	  CorrectUserElements(args.Type, true);
	  elements = MergeFormattingElements(FormattingElements, elements);
	  elements = MergePrevTokenTextElements(elements, prevElementsInText);
	  TokenGenArgs.Reset();
	  WriteFormattingElements(elements);
	  bool isLastEol = elements != null && elements.IsLastElementNewLine();
	  WrapLongTokenTextIfNeeded(args, isLastEol);
	  if (NeedIncreaseAlignment)
	  {
		CodeGen.Code.IncreaseAlignment();
		NeedIncreaseAlignment = false; 
	  }
	  WriteTokenText(args);
	  FormattingElements nextElements = null;
	  if (!CodeGen.SaveFormat)
		if (ElementCodeGen != null)
		  nextElements = ElementCodeGen.NextFormattingElements(tokenType);
		else
		  nextElements = CodeGen.NextFormattingElements(tokenType);
	  if (nextElements == null)
		nextElements = new FormattingElements();
	  nextElements = MergeNextTokenTextElements(nextElements, nextElementsInText);
	  if (nextElements != null)
		TokenGenArgs.SetFormattingElements(nextElements);
	  FormattingElements userTokens = GetUserFormattingElements(element, tokenType, args.Index);
	  if (userTokens != null)
		TokenGenArgs.SetUserFormattingElements(userTokens);
	  CorrectUserElements(args.Type, false);
	}
	bool NeedCorrectUserElements(FormattingTokenType token, bool previous, LanguageElement context, CodeGenOptions options)
	{
	  if (token == FormattingTokenType.CurlyBraceOpen || token == FormattingTokenType.CurlyBraceClose && previous)
		return true;
	  if (!previous)
		return false;
	  if (token == FormattingTokenType.If && context != null && context.Parent is Else && !CodeGen.Options.LineBreaks.PlaceIfStatementFollowedByElseOnNewLine)
		return true;
	  if (token == FormattingTokenType.Catch && !CodeGen.Options.LineBreaks.PlaceCatchStatementOnNewLine)
		return true;
	  if (token == FormattingTokenType.Else && !CodeGen.Options.LineBreaks.PlaceElseStatementOnNewLine)
		return true;
	  if (token == FormattingTokenType.Finally && !CodeGen.Options.LineBreaks.PlaceFinallyStatementOnNewLine)
		return true;
	  if (token == FormattingTokenType.While && context != null && context is Do && !CodeGen.Options.LineBreaks.PlaceWhileStatementOnNewLine)
		return true;
	  return false;
	}
	void CorrectUserElements(FormattingTokenType token, bool previous)
	{
	  if (CodeGen.SaveFormat)
		return;
	  if (FormattingHelper.KeepExpressionIndent(CodeGen))
		FormattingHelper.MergeIndents(CodeGen, TokenGenArgs.UserFormattingElements);
	  if (!NeedCorrectUserElements(token, previous, CodeGen.Context, CodeGen.Options))
		return;
	  for (int i = TokenGenArgs.UserFormattingElements.Count - 1; i > 0; i--)
	  {
		FormattingElement current = TokenGenArgs.UserFormattingElements[i] as FormattingElement;
		IFormattingElement prevIFE = TokenGenArgs.UserFormattingElements[i - 1];
		if (prevIFE is Comment || prevIFE is PreprocessorDirective)
		{
		  if (current != null && !current.IsEol())
			TokenGenArgs.UserFormattingElements.AddNewLine();
		  return;
		}
		if (TokenGenArgs.UserFormattingElements[i] is Comment)
		{
		  TokenGenArgs.UserFormattingElements.AddNewLine();
		  break;
		}
		if (current != null && !(current is CheckedIndent))
			TokenGenArgs.UserFormattingElements.RemoveAt(i);
	  }
	  if (TokenGenArgs.UserFormattingElements.Count == 1 && !(TokenGenArgs.UserFormattingElements[0] is Comment))
		TokenGenArgs.UserFormattingElements.Clear();
	}
	void CleanupElements(FormattingElements result)
	{
	  FormattingHelper.CleanupElements(CodeGen, result);
	}
	FormattingElements MergeFormattingElements(FormattingElements elements, FormattingElements formattingElements)
	{
	  FormattingElements result = new FormattingElements();
	  if (elements != null && elements.Count != 0)
		result.AddRange(elements);
	  if (formattingElements != null && formattingElements.Count != 0)
		result.AddRange(formattingElements);
	  if (CodeGen.SaveFormat)
		return result;
	  CleanupElements(result);
	  return result;
	}
	FormattingElements GetUserFormattingElements(LanguageElement element, FormattingTokenType type, int index)
	{
	  if (element == null)
		return null;
	  FormattingElements result = new FormattingElements();
	  FormattingElements elements = element.GetFormattingElements(type, index);
	  if (elements != null)
		result.AddRange(elements);
	  if (!CodeGen.SaveFormat)
	  {
		ClearUserFormattingElements(result);
		result = ProcessStartToken(type, result);
	  }
	  return result;
	}
	public void WriteResiduaryFormattingElements()
	{
	  if (TokenGenArgs.IsEmpty)
		return;
	  FormattingElements formattingElements = FormattingElements;
	  TokenGenArgs = null;
	  CleanupElements(formattingElements);
	  WriteFormattingElements(formattingElements);
	}
	protected void WriteFormattingElements(FormattingElements elements)
	{
	  if (elements == null)
		return;
	  int count = elements.Count;
	  for (int i = 0; i < count; i++)
	  {
		IFormattingElement element = elements[i];
		if (element == null)
		  continue;
		FormattingElementsWriter.WriteSupportElement(CodeGen, element);
		_LastGeneratedFormattingElement = element;
		if (!TokenGenArgs.IsEmpty)
		{
		  FormattingElements remainingElements = null;
		  if (i + 1 < count)
			remainingElements = elements.GetElements(i + 1, count - i - 1);
		  elements = MergeFormattingElements(FormattingElements, remainingElements);
		  TokenGenArgs.Reset();
		  if (elements == null || elements.Count == 0)
			return;
		  i = -1;
		  count = elements.Count;
		}
	  }
	}
	public void AddNewLine(int countConsecutive)
	{
	  TokenGenArgs.AddNewLine(countConsecutive);
	}
	public void AddNewLineIfNeeded()
	{
	  foreach (object obj in FormattingElements)
	  {
		FormattingElement fe = obj as FormattingElement;
		if (fe != null && fe.IsEol())
		  break;
		if (obj is Comment)
		{
		  if (!FormattingElements.IsLastElementNewLine())
			TokenGenArgs.EndsEOLNecessary = true;
		  return;
		}
	  }
	  if (TokenGenArgs.IsLastElementNewLine())
		return;
	  TokenGenArgs.AddNewLine();
	}
	internal bool IsLastGeneratedEOL()
	{
	  FormattingElement element = LastGeneratedFormattingElement as FormattingElement;
	  if (element == null)
		return false;
	  return element.Type == FormattingElementType.EOL;
	}
	public void AddWSIfNeeded()
	{
	  if (TokenGenArgs.IsLastElementWhiteSpace())
		return;
	  TokenGenArgs.AddWhiteSpace();
	}
	public void AddDecreaseIndent()
	{
	  TokenGenArgs.AddDecreaseIndent();
	}
	public void AddIncreaseIndent()
	{
	  TokenGenArgs.AddIncreaseIndent();
	}
	public void AddClearIndent()
	{
	  TokenGenArgs.AddClearIndent();
	}
	public void AddRestoreIndent()
	{
	  TokenGenArgs.AddRestoreIndent();
	}
	CodeGen CodeGen
	{
	  get
	  {
		return _CodeGen;
	  }
	}
	internal bool GeneratingInSupportElement
	{
	  get
	  {
		return _GeneratingInSupportElement;
	  }
	  set
	  {
		_GeneratingInSupportElement = value;
	  }
	}
	internal TokenGenArgs TokenGenArgs
	{
	  get
	  {
		if (_TokenGenArgs == null)
		  _TokenGenArgs = new TokenGenArgs();
		return _TokenGenArgs;
	  }
	  set
	  {
		_TokenGenArgs = value;
	  }
	}
	internal FormattingElements FormattingElements
	{
	  get
	  {
		if (TokenGenArgs == null)
		  return new FormattingElements();
		PrepareElements(TokenGenArgs.FormattingElements, TokenGenArgs.UserFormattingElements);
		FormattingElements result = MergeFormattingElements(TokenGenArgs.FormattingElements, TokenGenArgs.UserFormattingElements);
		if (!result.IsLastElementNewLine() && TokenGenArgs.EndsEOLNecessary)
		  result.Add(new FormattingElement(FormattingElementType.EOL));
		return result;
	  }
	}
	void PrepareElements(FormattingElements formattingElements, FormattingElements userElements)
	{
	  if (formattingElements == null || userElements == null)
		return;
	 bool wasComment = false;
	  for (int i = 0; i < userElements.Count; i++)
	  {
		FormattingElement fe = userElements[i] as FormattingElement;
		if (fe != null && fe.IsEol())
		  break;
		Comment com = userElements[i] as Comment;
		if (com == null)
		  continue;
		wasComment = true;
		break;
	  }
	  if (wasComment)
		formattingElements.RemoveLastEolIfNeeded();
	}
	public FormattingElements FormattingElementsForSupportElement
	{
	  get
	  {
		if (_FormattingElementsForSupportElement == null)
		  _FormattingElementsForSupportElement = new FormattingElements();
		return _FormattingElementsForSupportElement;
	  }
	  set { _FormattingElementsForSupportElement = value; }
	}
	public LanguageElementCodeGenBase ElementCodeGen
	{
	  get { return _ElementCodeGen; }
	  set { _ElementCodeGen = value; }
	}
	public bool NeedIncreaseAlignment
	{
	  get { return _NeedIncreaseAlignment; }
	  set { _NeedIncreaseAlignment = value; }
	}
	public IFormattingElement LastGeneratedFormattingElement
	{
	  get { return _LastGeneratedFormattingElement;}
	}
  }
}
