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
  public class TokenGenArgs
  {
	FormattingElements _FormattingElements;
	FormattingElements _UserFormattingElements;
	bool _EndsEOLNecessary;
	public TokenGenArgs()
	{
	}
	public TokenGenArgs(FormattingElements formattingElements)
	{
	  _FormattingElements = formattingElements;
	}
	public bool IsEmpty
	{
	  get
	  {
		return FormattingElements.Count <= 0 && UserFormattingElements.Count <= 0 && !_EndsEOLNecessary;
	  }
	}
	public void AddWhiteSpace()
	{
	  FormattingElements.AddWhiteSpace();
	}
	public void AddDecreaseIndent()
	{
	  FormattingElements.AddDecreaseIndent();
	}
	public void AddIncreaseIndent()
	{
	  FormattingElements.AddIncreaseIndent();
	}
	public void AddClearIndent()
	{
	  FormattingElements.AddClearIndent();
	}
	public void AddRestoreIndent()
	{
	  FormattingElements.AddRestoreIndent();
	}
	public void Reset()
	{
	  _FormattingElements = null;
	  _UserFormattingElements = null;
	  _EndsEOLNecessary = false;
	}
	public bool Contains(object element)
	{
	  if (element == null)
		return false;
	  if (_FormattingElements != null)
		foreach (object el in _FormattingElements)
		  if (el == element)
			return true;
	  if (_UserFormattingElements != null)
		foreach (object el in _UserFormattingElements)
		  if (el == element)
			return true;
	  return false;
	}
	public void AddNewLine(int countConsecutive)
	{
	  if (_FormattingElements == null)
		_FormattingElements = new FormattingElements();
	  _FormattingElements.AddNewLine(countConsecutive);
	}
	public void AddNewLine()
	{
	  if (_FormattingElements == null)
		_FormattingElements = new FormattingElements();
	  _FormattingElements.AddNewLine();
	}
	public void SetFormattingElements
	  (FormattingElements nextElements)
	{
	  _FormattingElements = nextElements;
	}
	public void SetUserFormattingElements(FormattingElements userTokens)
	{
	  _UserFormattingElements = userTokens;
	}
	public FormattingElements FormattingElements
	{
	  get
	  {
		if (_FormattingElements == null)
		  _FormattingElements = new FormattingElements();
		return _FormattingElements;
	  }
	}
	public FormattingElements UserFormattingElements
	{
	  get
	  {
		if (_UserFormattingElements == null)
		  _UserFormattingElements = new FormattingElements();
		return _UserFormattingElements;
	  }
	}
	public bool EndsEOLNecessary
	{
	  get { return _EndsEOLNecessary; }
	  set {	_EndsEOLNecessary = value; }
	}
	public void ClearFormattingElements()
	{
	  _FormattingElements = null;
	  _UserFormattingElements = null;
	  _EndsEOLNecessary = false;
	}
	internal bool IsLastElementNewLine()
	{
	  if (_FormattingElements == null)
		return false;
	  return _FormattingElements.IsLastElementNewLine();
	}
	internal bool IsLastElementWhiteSpace()
	{
	  if (_FormattingElements == null)
		return false;
	  return _FormattingElements.IsLastElementWhiteSpace();
	}
	internal bool IsLastElementIndent()
	{
	  if (_FormattingElements == null)
		return false;
	  return _FormattingElements.IsLastElementIndent();
	}
	public TokenGenArgs SaveAndReset()
	{
	  TokenGenArgs result  = new TokenGenArgs();
	  result._FormattingElements = _FormattingElements;
	  result._UserFormattingElements = _UserFormattingElements;
	  result._EndsEOLNecessary = _EndsEOLNecessary;
	  Reset();
	  return result;
	}
	public TokenGenArgs LogicForProcessComment()
	{
	  if (_UserFormattingElements == null)
		return null;
	  TokenGenArgs result = new TokenGenArgs();
	  bool needRedirect = false;
	  for (int i = 0; i < _UserFormattingElements.Count; i++)
	  {
		if(_UserFormattingElements[i] is PreprocessorDirective)
		  needRedirect = true;
		if (needRedirect)
		{
		  result.UserFormattingElements.AddRange(_UserFormattingElements.GetElements(i, _UserFormattingElements.Count - i));
		  _UserFormattingElements.RemoveRange(i, _UserFormattingElements.Count - i);
		  break;
		}
		FormattingElement formattingElement = _UserFormattingElements[i] as FormattingElement;
		if (formattingElement != null && formattingElement.IsEol())
		  needRedirect = true;
		Comment com = _UserFormattingElements[i] as Comment;
		if (com != null && com.CommentType == CommentType.SingleLine)
		  needRedirect = true;
	  }
	  return result;
	}
  }
}
