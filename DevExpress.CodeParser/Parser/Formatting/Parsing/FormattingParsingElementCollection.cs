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
  public class FormattingParsingElementCollection
  {
	List<FormattingParsingElement> _Coll;
	public void Add(SourceRange range, FormattingParsingElement element)
	{
	  if (element == null || range.IsEmpty)
		return;
	  element.Range = range;
	  Coll.Add(element);
	}
	internal void ReplaceOrAdd(SourceRange range, FormattingParsingElement element)
	{
	  element.Range = range;
	  int index = Coll.BinarySearch(element);
	  if(index < 0)
	  {
		Add(range, element);
		return;
	  }
	  if(_Coll[index].Range != range)
		return;
	  _Coll[index] = element;
	}
	public FormattingParsingElement LastElement
	{
	  get
	  {
		int lastIndex = Coll.Count - 1;
		if (lastIndex < 0)
		  return null;
		return Coll[lastIndex];
	  }
	}
	public void Clear()
	{
	  if (_Coll == null)
		return;
	  _Coll.Clear();
	}
	List<FormattingParsingElement> Coll
	{
	  get
	  {
		if (_Coll == null)
		  _Coll = new List<FormattingParsingElement>();
		return _Coll;
	  }
	}
	public LanguageElementTokens GetSourceFileTokens()
	{
	  if (_Coll == null)
		return null;
	  FormattingParsingElement fpe = null;
	  foreach (FormattingParsingElement element in _Coll)
	  {
		if (element.FormattingType == FormattingTokenType.SourceFileStart)
		{
		  if (fpe == null)
			fpe = element;
		  else
			fpe.AddFormattingElements(element);
		}
	  }
	  _Coll.Clear();
	  if (fpe == null)
		return null;
	  List<FormattingParsingElement> elements = new List<FormattingParsingElement>();
	  elements.Add(fpe);
	  return new LanguageElementTokens(elements);
	}
	public LanguageElementTokens GetTokens(SourceRange scopeRange)
	{
	  if (_Coll == null)
		return null;
	  List<FormattingParsingElement> result = new List<FormattingParsingElement>();
	  FormattingParsingElement fictiveToken = new FormattingParsingElement(FormattingTokenType.Fictive, null);
	  fictiveToken.Range = scopeRange;
	  int index = _Coll.BinarySearch(fictiveToken);
	  if (index < 0)
		return null;
	  int count = _Coll.Count;
	  int startIndex = index - 1;
	  FormattingParsingElement element;
	  for (; startIndex >= 0; startIndex--)
	  {
		element = _Coll[startIndex];
		if (!fictiveToken.Contains(element))
		  break;
		result.Add(element);
	  }
	  startIndex++;
	  result.Reverse();
	  result.Add(_Coll[index]);
	  index ++;
	  for (; index < count; index++)
	  {
		element = _Coll[index];
		if (!fictiveToken.Contains(element))
		  break;
		result.Add(element);
	  }
	  _Coll.RemoveRange(startIndex, result.Count);
	  if (result == null || result.Count == 0)
		return null;
	  return new LanguageElementTokens(result);
	}
  }
}
