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
  public class FormattingParsingElement : IComparable<FormattingParsingElement>
  {
	FormattingElements _FormattingElements;
	FormattingTokenType _FormattingType;
	int _StartLine;
	int _StartOffset;
	int _EndLine;
	int _EndOffset;
	public FormattingParsingElement(FormattingTokenType type, FormattingElements formattingElements)
	{
	  FormattingType = type;
	  FormattingElements = formattingElements;
	}
	public void AddFormattingElements(FormattingParsingElement formattingParsingElement)
	{
	  if (formattingParsingElement == null || formattingParsingElement == this)
		return;
	  AddFormattingElements(formattingParsingElement.FormattingElements);
	}
	public void AddFormattingElements(FormattingElements formattingElements)
	{
	  if (formattingElements == null)
		return;
	  if (FormattingElements == null)
		FormattingElements = formattingElements;
	  else
		FormattingElements.AddRange(formattingElements);
	}
	public void AddFormattingElement(IFormattingElement formattingElement)
	{
	  if (formattingElement == null)
		return;
	  if (FormattingElements == null)
		FormattingElements = new FormattingElements();
	  FormattingElements.Add(formattingElement);
	}
	public override string ToString()
	{
	  return string.Format(@"Range = {0}; Type: ""{1}""", Range, _FormattingType);
	}
	public int CompareTo(FormattingParsingElement other)
	{
	  if (other.Contains(_StartLine, _StartOffset) &&
		other.Contains(_EndLine, _EndOffset))
		return 0;
	  if (_StartLine > other._EndLine)
		return 1;
	  if (_StartLine == other._EndLine && _StartOffset >= other._EndOffset)
		return 1;
	  return -1;
	}
	internal bool Contains(FormattingParsingElement element)
	{
	  return Contains(element._StartLine, element._StartOffset)
		&& Contains(element._EndLine, element._EndOffset);
	}
	bool Contains(int line, int offset)
	{
	  if (line < _StartLine || line > _EndLine)
		return false;
	  bool result = true;
	  if (line == _StartLine)
		result = offset >= _StartOffset;
	  if (line == _EndLine)
		result &= offset <= _EndOffset;
	  return result;
	}
	public FormattingElements FormattingElements
	{
	  get
	  {
		return _FormattingElements;
	  }
	  set
	  {
		_FormattingElements = value;
	  }
	}
	public FormattingTokenType FormattingType
	{
	  get { return _FormattingType; }
	  set { _FormattingType = value; }
	}
	public SourceRange Range
	{
	  get
	  {
		return new SourceRange(_StartLine, _StartOffset, _EndLine, _EndOffset);
	  }
	  set
	  {
		_StartLine = value.Start.Line;
		_StartOffset = value.Start.Offset;
		_EndLine = value.End.Line;
		_EndOffset = value.End.Offset;
	  }
	}
  }
}
