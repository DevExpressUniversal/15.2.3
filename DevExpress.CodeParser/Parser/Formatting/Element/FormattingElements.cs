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
  public class FormattingElements : List<IFormattingElement>
  {
	public FormattingElements()
	{
	}
	public FormattingElements(List<IFormattingElement> elements)
	{
	  AddRange(elements);
	}
	public void AddDeadCode(string text)
	{
	  if (String.IsNullOrEmpty(text))
		return;
	  DeadCode deadCode = new DeadCode(text);
	  Add(deadCode);
	}
	public void AddSourceFileStartText(string text)
	{
	  if (String.IsNullOrEmpty(text))
		return;
	  SourceFileStartText sourceFileStartText = new SourceFileStartText(text);
	  Add(sourceFileStartText);
	}
	public FormattingElements CloneArray(ElementCloneOptions options)
	{
	  FormattingElements result = new FormattingElements();
	  foreach (IFormattingElement formattingElement in this)
	  {
		IFormattingElement clonedFormattingElement = CloneElement(formattingElement, options);
		if (clonedFormattingElement == null)
		  continue;
		result.Add(clonedFormattingElement);
	  }
	  return result;
	}
	public FormattingElements GetElements(int index, int count)
	{
	  List<IFormattingElement> elements = GetRange(index, count);
	  return new FormattingElements(elements);
	}
	IFormattingElement CloneElement(IFormattingElement element, ElementCloneOptions options)
	{
	  if (element == null)
		return null;
	  if (element is XmlDocComment)
		return ((XmlDocComment)element).Clone(options) as IFormattingElement;
	  if (element is Comment)
		return ((Comment)element).Clone(options) as IFormattingElement;
	  if (element is PreprocessorDirective)
		return ((PreprocessorDirective)element).Clone(options) as IFormattingElement;
	  return element.Clone() as IFormattingElement;
	}
	bool IsLastElement(FormattingElementType type)
	{
	  int count = Count;
	  if (count == 0)
		return false;
	  FormattingElement tokenElement = this[count - 1] as FormattingElement;
	  if (tokenElement == null)
		return false;
	  return tokenElement.Type == type || tokenElement.Type == type;
	}
	internal void Add(FormattingElementType type, int countConsecutive)
	{
	  FormattingElement element = new FormattingElement(type, countConsecutive);
	  Add(element);
	}
	public void RemoveRange(FormattingElements list)
	{
	  if (list == null)
		return;
	  foreach (IFormattingElement element in list)
		Remove(element);
	}
	public int GetEndEOLIndex(int index)
	{
	  if (Count <= index)
		return index;
	  FormattingElement fte = this[index] as FormattingElement;
	  if (fte == null)
		return index;
	  if (fte.Type != FormattingElementType.CR)
		return index;
	  if (index + 1 >= Count)
		return index;
	  fte = this[index + 1] as FormattingElement;
	  if (fte == null)
		return index;
	  if (fte.Type == FormattingElementType.EOL)
		return index + 1;
	  return index;
	}
	public FormattingElement GetLastEOLElement(int index)
	{
	  if (Count <= index)
		return null;
	  FormattingElement fte = this[index] as FormattingElement;
	  if (fte == null)
		return null;
	  if (fte.Type != FormattingElementType.CR)
		return fte;
	  if (index + 1 >= Count)
		return fte;
	  FormattingElement eolFte = this[index + 1] as FormattingElement;
	  if (eolFte == null)
		return fte;
	  if (eolFte.Type == FormattingElementType.EOL)
		return eolFte;
	  return fte;
	}
	public bool HasOnlyNewLine()
	{
	  if (Count != 1)
		return false;
	  FormattingElement fte = this[0] as FormattingElement;
	  if (fte == null)
		return false;
	  return fte.IsEol();
	}
	public void RemoveFirstEOL()
	{
	  if (Count <= 0)
		return;
	  FormattingElement fte = this[0] as FormattingElement;
	  if (fte == null)
		return;
	  if (fte.Type == FormattingElementType.CR)
		RemoveAt(0);
	  else if (fte.Type == FormattingElementType.EOL)
	  {
		RemoveAt(0);
		return;
	  }
	  if (Count <= 0)
		return;
	  fte = this[0] as FormattingElement;
	  if (fte == null)
		return;
	  if (fte.Type == FormattingElementType.EOL)
		RemoveAt(0);
	}
	public void RemoveEOL(int index)
	{
	  if (index >= Count || index < 0)
		return;
	  FormattingElement element = this[index] as FormattingElement;
	  if (element == null || !element.IsEol())
		return;
	  RemoveAt(index--);
	  if (index >= Count || index < 0)
		return;
	  FormattingElement prevElement = this[index] as FormattingElement;
	  if (prevElement == null || !prevElement.IsEol() || prevElement.Type == element.Type)
		return;
	  RemoveAt(index);
	}
	public void AddWhiteSpace()
	{
	  AddWhiteSpace(1);
	}
	public void AddWhiteSpace(int countConsecutive)
	{
	  Add(FormattingElementType.WS, countConsecutive);
	}
	public void AddIncreaseIndent()
	{
	  Add(FormattingElementType.IncreaseIndent, 1);
	}
	public void AddDecreaseIndent()
	{
	  Add(FormattingElementType.DecreaseIndent, 1);
	}
	public void AddIncreaseAlignment()
	{
	  Add(FormattingElementType.IncreaseAlignment, 1);
	}
	public void AddDecreaseAlignment()
	{
	  Add(FormattingElementType.DecreaseAlignment, 1);
	}
	public void AddClearIndent()
	{
	  Add(FormattingElementType.ClearIndent, 1);
	}
	public void AddRestoreIndent()
	{
	  Add(FormattingElementType.RestoreIndent, 1);
	}
	public void AddNewLine()
	{
	  AddNewLine(1);
	}
	public void AddNewLine(int countConsecutive)
	{
	  Add(FormattingElementType.EOL, countConsecutive);
	}
	public bool IsLastElementNewLine()
	{
	  return IsLastElement(FormattingElementType.EOL) || IsLastElement(FormattingElementType.CR);
	}
	public bool IsLastElementWhiteSpace()
	{
	  return IsLastElement(FormattingElementType.WS);
	}
	public bool IsLastElementIndent()
	{
	  return IsLastElement(FormattingElementType.IncreaseIndent) || IsLastElement(FormattingElementType.DecreaseIndent);
	}
	public bool DeleteDirective(PreprocessorDirective searchElement)
	{
	  int start = 0;
	  int count = Count;
	  bool isFind = false;
	  for (int i = 0; i < count; i++)
	  {
		IFormattingElement ife = this[i];
		if (!isFind && ife == searchElement)
		  isFind = true;
		FormattingElement fe = ife as FormattingElement;
		if (fe != null && fe.Type == FormattingElementType.EOL)
		  if (!isFind)
			start = i + 1;
		  else
		  {
			RemoveRange(start, i - start + 1);
			return true;
		  }
	  }
	  if (isFind)
		RemoveRange(start, count - start);
	  return isFind;
	}
	public void DeleteEmptyLines(bool attemptDeleteLastLine)
	{
	  bool haveNotFEInLastLine = false;
	  bool haveNotFEAfterFirstLine = false;
	  bool haveSingleLineCommentAtFirstLine = false;
	  int elementAfterEOL = -1;
	  for (int i = 0; i < Count; i++)
	  {
		FormattingElement fe = this[i] as FormattingElement;
		if (fe != null)
		{
		  if (fe.IsEol())
		  {
			if (!haveNotFEInLastLine && elementAfterEOL != -1)
			{
			  int countInLine = i - elementAfterEOL + 1;
			  RemoveRange(elementAfterEOL, countInLine);
			  i -= countInLine;
			}
			haveNotFEInLastLine = false;
			elementAfterEOL = i + 1;
		  }
		}
		else if (elementAfterEOL != -1)
		  haveNotFEAfterFirstLine = haveNotFEInLastLine = true;
		else
		{
		  Comment comment = this[i] as Comment;
		  haveSingleLineCommentAtFirstLine = comment != null && comment.CommentType == CommentType.SingleLine;
		}
	  }
	  if (attemptDeleteLastLine && !haveNotFEAfterFirstLine && !haveSingleLineCommentAtFirstLine && elementAfterEOL != -1)
		RemoveRange(elementAfterEOL - 1, Count - elementAfterEOL + 1);
	}
	internal void RemoveLastEolIfNeeded()
	{
	  int eolIndex = -1;
	  for (int i = Count - 1; i >= 0; i--)
	  {
		FormattingElement fe = this[i] as FormattingElement;
		if (fe == null)
		  break;
		if (fe.IsEol())
		  eolIndex = i;
	  }
	  if (eolIndex < 0)
		return;
	  for (int i = Count - 1; i >= eolIndex; i--)
	  {
		FormattingElement fe = this[i] as FormattingElement;
		if (fe != null && (fe.IsEol() || fe.IsWsOrTab()))
		  RemoveAt(i);
	  }
	}
	internal void AddNewLineAfterIndex(int i)
	{
	  if (i >= Count)
	  {
		AddNewLine();
		return;
	  }
	  base.Insert(i + 1, new FormattingElement(FormattingElementType.EOL));
	  return;
	}
	internal void AddFirstIfNeeded(FormattingElementType type)
	{
	  if (Count == 0)
	  {
		Add(new FormattingElement(type));
		return;
	  }
	  FormattingElement tElement = this[0] as FormattingElement;
	  if (tElement != null && tElement.Type == type)
		return;
	  Insert(0, new FormattingElement(type));
	}
  }
}
