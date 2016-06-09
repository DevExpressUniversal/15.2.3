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
  internal class FormattingHelper
  {
	public static bool IsFormattingChar(char ch)
	{
	  return ch == '\n' || ch == '\r' || ch == ' ' || ch == '\t';
	}
	public static void AddTokenElement(FormattingElements list, char ch)
	{
	  if (ch == '\n')
		list.AddNewLine();
	  if (ch == ' ')
		list.AddWhiteSpace();
	}
	public static bool CanProcessTextToken(GenTextArgs args)
	{
	  FormattingTokenType tokenType = args.Type;
	  return args.Context is ISnippetCodeElement
			 && tokenType != FormattingTokenType.NewLine
			 && tokenType != FormattingTokenType.WhiteSpace
			 && tokenType != FormattingTokenType.SourceFileStart;
	}
	public static void ProcessTokenText(GenTextArgs args, out FormattingElements prevElementsInText, out FormattingElements nextElementsInText)
	{
	  prevElementsInText = null;
	  nextElementsInText = null;
	  string tokenText = args.Text;
	  if (String.IsNullOrEmpty(tokenText))
		return;
	  prevElementsInText = new FormattingElements();
	  nextElementsInText = new FormattingElements();
	  char[] charArray = tokenText.ToCharArray();
	  int rSymbols = 0;
	  for (int i = 0; i < charArray.Length; i++)
	  {
		char ch = charArray[i];
		if (!FormattingHelper.IsFormattingChar(ch))
		  break;
		FormattingHelper.AddTokenElement(prevElementsInText, ch);
		rSymbols++;
	  }
	  if (rSymbols != 0)
		tokenText = tokenText.Remove(0, rSymbols);
	  if (rSymbols >= charArray.Length)
	  {
		args.Text = tokenText;
		return;
	  }
	  charArray = tokenText.ToCharArray();
	  rSymbols = 0;
	  for (int i = charArray.Length - 1; i >= 0; i--)
	  {
		char ch = charArray[i];
		if (!FormattingHelper.IsFormattingChar(ch))
		  break;
		FormattingHelper.AddTokenElement(nextElementsInText, ch);
		rSymbols++;
	  }
	  if (rSymbols != 0)
	  {
		tokenText = tokenText.Remove(tokenText.Length - rSymbols);
		if (nextElementsInText.Count != 0)
		  nextElementsInText.Reverse();
	  }
	  args.Text = tokenText;
	}
	static void Sort(FormattingElements elements) 
	{
	  if (elements == null)
		return;
	  bool passed = false;
	  while (!passed)
	  {
		passed = true;
		int count = elements.Count;
		for (int i = 0; i < count - 1; i++)
		{
		  FormattingElement current = elements[i] as FormattingElement;
		  if (current == null)
			continue;
		  FormattingElement next = elements[i + 1] as FormattingElement;
		  if (next == null)
			continue;
		  switch (next.Type)
		  {
			case FormattingElementType.DecreaseAlignment:
			case FormattingElementType.DecreaseIndent:
			case FormattingElementType.IncreaseAlignment:
			case FormattingElementType.IncreaseIndent:
			case FormattingElementType.ClearIndent:
			case FormattingElementType.RestoreIndent:
			  if (current.Type == FormattingElementType.EOL || current.Type == FormattingElementType.WS || current.Type == FormattingElementType.Tab)
			  {
				passed = false;
				elements[i] = next;
				elements[i + 1] = current;
			  }
			  break;
		  }
		}
	  }
	}
	static FormattingElements MergeIndents(CodeGen codeGen, List<FormattingElement> indents)
	{
	  if (codeGen == null || indents == null)
		return null;
	  FormattingElements result = new FormattingElements();
	  int count = 0;
	  int tabSize = codeGen.Options.TabSize;
	  foreach (FormattingElement indent in indents)
		if (indent.Type == FormattingElementType.WS)
		  count += indent.CountConsecutive;
		else
		  count += indent.CountConsecutive * tabSize;
	  if (codeGen.Options.InsertSpaces)
		result.Add(new CheckedIndent(0, count));
	  else
	  {
		int tabCount = count / tabSize;
		int spaceCount = count % tabSize;
		result.Add(new CheckedIndent(tabCount, spaceCount));
	  }
	  return result;
	}
	public static void MergeIndents(CodeGen codeGen, FormattingElements elements)
	{
	  if (elements == null)
		return;
	  int index = 0;
	  while (index < elements.Count)
	  {
		FormattingElement current = elements[index] as FormattingElement;
		if (current != null && current.IsEol() && index < elements.Count - 1)
		{
		  int startSpaceIndex = index + 1;
		  int spaceIndex = startSpaceIndex;
		  List<FormattingElement> indents = new List<FormattingElement>();
		  while (spaceIndex < elements.Count)
		  {
			FormattingElement space = elements[spaceIndex] as FormattingElement;
			if (space is CheckedIndent)
			  break;
			if (space == null || !space.IsWsOrTab())
			  break;
			indents.Add(space);
			spaceIndex++;
		  }
		  if (indents.Count > 0)
		  {
			FormattingElements mergedIndents = MergeIndents(codeGen, indents);
			elements.RemoveRange(startSpaceIndex, indents.Count);
			for (int i = mergedIndents.Count - 1; i >= 0; i--)
			  elements.Insert(startSpaceIndex, mergedIndents[i]);
		  }
		}
		index++;
	  }
	}
	public static bool KeepExpressionIndent(CodeGen codeGen)
	{
	  if (codeGen == null)
		return false;
	  if (!codeGen.Options.General.KeepExpressionsIndent)
		return false;
	  Expression context = codeGen.Context as Expression;
	  if (context == null)
		return false;
	  LanguageElement parent = context.GetParentStatementOrVariable();
	  if (parent == null)
	  {
		if (context.GetParent(LanguageElementType.Parameter) != null)
		  return !codeGen.Options.WrappingAlignment.AlignWithFirstFormalParameter;
		return false;
	  }
	  IHasArguments hasArgs = parent as IHasArguments;
	  if (hasArgs != null && hasArgs.Arguments.Contains(context))
		return !codeGen.Options.WrappingAlignment.AlignWithFirstInvocationArgument;
	  if ((parent is Variable || parent is Assignment) && parent.IsDetailNode)
		parent = parent.Parent;
	  if (parent == null)
		return false;
	  return parent.Range.Start != context.NameRange.Start;
	}
	public static void CleanupElements(CodeGen codeGen, FormattingElements result)
	{
	  if (result == null || codeGen.SaveFormat)
		return;
	  Sort(result);
	  int from = result.Count - 1;
	  for (int i = from; i > 0; i--)
	  {
		FormattingElement next = result[i] as FormattingElement;
		if (next == null || next.IsIndent())
		  continue;
		FormattingElement current = result[i - 1] as FormattingElement;
		if (current == null)
		  continue;
		if (current.Type == FormattingElementType.IncreaseAlignment && next.IsWsOrTab())
		{
		  result[i - 1] = next;
		  result[i] = current;
		  i++;
		  continue;
		}
		int j = i - 1;
		if (next.Type == FormattingElementType.EOL)
		{
		  if (current.Type == FormattingElementType.WS || current.Type == FormattingElementType.Tab)
		  {
			result.RemoveAt(j);
			continue;
		  }
		}
		else if (current.IsEol() && !KeepExpressionIndent(codeGen))
		{
		  if (next.Type == FormattingElementType.WS || next.Type == FormattingElementType.Tab)
		  {
			result.RemoveAt(i);
			if (i + 1 < result.Count)
			  i++;
			continue;
		  }
		}
		if (next.Type == current.Type)
		  if (current.CountConsecutive < next.CountConsecutive && !(current is CheckedIndent))
			result.RemoveAt(j);
		  else
			result.RemoveAt(i);
	  }
	}
  }
}
