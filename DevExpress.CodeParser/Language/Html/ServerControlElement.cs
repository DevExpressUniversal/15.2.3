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
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum ActiveHtmlTagPart
  {
	Property,
	Qualifier,
	None
  }
  public class ServerControlElement : HtmlElement, IMarkupElement
  {
	ActiveHtmlTagPart _ActiveHtmlTagPart = ActiveHtmlTagPart.None;
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void EvaluateActivePart(SourcePoint activePoint)
	{
	  SourceRange qualifierRange = SourceRange.Empty;
	  SourceRange propertyRange = GetPropertyRange(out qualifierRange);
	  bool wasSet = SetActivePart(activePoint, propertyRange, qualifierRange);
	  if (wasSet)
		return;
	  qualifierRange = SourceRange.Empty;
	  propertyRange = GetClosePropertyRange(out qualifierRange);
	  SetActivePart(activePoint, propertyRange, qualifierRange);
	}
	bool SetActivePart(SourcePoint activePoint, SourceRange propertyRange, SourceRange qualifierRange)
	{
	  if (propertyRange == SourceRange.Empty || qualifierRange == SourceRange.Empty)
	  {
		_ActiveHtmlTagPart = ActiveHtmlTagPart.None;
		return true;
	  }
	  if (propertyRange.Contains(activePoint))
	  {
		_ActiveHtmlTagPart = ActiveHtmlTagPart.Property;
		return true;
	  }
	  if (qualifierRange.Contains(activePoint))
	  {
		_ActiveHtmlTagPart = ActiveHtmlTagPart.Qualifier;
		return true;
	  }
	  return false;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ActiveHtmlTagPart ActiveHtmlTagPart
	{
	  get
	  {
		return _ActiveHtmlTagPart;
	  }
	}
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.ServerControlElement;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  ServerControlElement lClone = new ServerControlElement();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.ServerControlElement;
	  }
	}
	#endregion
	#region TagPrefix
	public string TagPrefix
	{
	  get
	  {
		if (InternalName == null || InternalName.Length == 0)
		  return String.Empty;
		int colonPos = InternalName.IndexOf(":");
		if (colonPos == -1)
		  return String.Empty;
		return InternalName.Substring(0, colonPos);
	  }
	}
	#endregion
	#region TagName
	public string TagName
	{
	  get
	  {
		if (InternalName == null || InternalName.Length == 0)
		  return String.Empty;
		int colonPos = InternalName.IndexOf(":");
		if (colonPos == -1)
		  return String.Empty;
		return InternalName.Substring(colonPos + 1);
	  }
	}
	#endregion
	#region TagNameRange
	public SourceRange TagNameRange
	{
	  get
	  {
		string tagName = TagName;
		if (tagName == null)
		  return SourceRange.Empty;
		SourceRange range = NameRange;
		SourcePoint end = range.End;
		SourcePoint start = new SourcePoint(end.Line, end.Offset - tagName.Length);
		return new SourceRange(start, end);
	  }
	}
	#endregion
	#region TagPrefixRange
	public SourceRange TagPrefixRange
	{
	  get
	  {
		string tagPrefix = TagPrefix;
		if (tagPrefix == null)
		  return SourceRange.Empty;
		SourceRange range = NameRange;
		SourcePoint start = range.Start;
		SourcePoint end = new SourcePoint(start.Line, start.Offset + tagPrefix.Length);
		return new SourceRange(start, end);
	  }
	}
	#endregion
	public SourceRange GetPropertyRange(out SourceRange qualifierRange)
	{
	  qualifierRange = SourceRange.Empty;
	  SourceRange range = NameRange;
	  SourcePoint end = range.End;
	  return GetPropertyRangeCore(end, out qualifierRange);
	}
	public SourceRange GetClosePropertyRange(out SourceRange qualifierRange)
	{
	  SourcePoint end = Range.End;
	  int endLine = end.Line;
	  int endOffset = end.Offset;
	  return GetPropertyRangeCore(new SourcePoint(endLine, endOffset - 1), out qualifierRange);
	}
	SourceRange GetPropertyRangeCore(SourcePoint end, out SourceRange qualifierRange)
	{
	  qualifierRange = SourceRange.Empty;
	  string qualifierName = null;
	  string propertyName = GetPropertyName(out qualifierName);
	  if (propertyName == null)
		return SourceRange.Empty;
	  SourcePoint startProperty = new SourcePoint(end.Line, end.Offset - propertyName.Length);
	  if (qualifierName != null)
	  {
		SourcePoint endQualifierName = new SourcePoint(startProperty.Line, startProperty.Offset - 1);
		SourcePoint startQualifierName = new SourcePoint(endQualifierName.Line, endQualifierName.Offset - qualifierName.Length);
		qualifierRange = new SourceRange(startQualifierName, endQualifierName);
	  }
	  return new SourceRange(startProperty, end);
	}
	public string GetPropertyName(out string qualifierName)
	{
	  qualifierName = null;
	  if (String.IsNullOrEmpty(InternalName))
		return null;
	  List<char> propertyName = new List<char>();
	  List<char> qualifierResult = new List<char>();
	  int index = InternalName.Length - 1;
	  bool wasDot = false;
	  char ch = InternalName[index];
	  while (ch != ':' || Char.IsLetter(ch) || ch == '.')
	  {
		if (ch == '.')
		{
		  if (wasDot)
			return null;
		  wasDot = true;
		}
		else
		{
		  if (wasDot)
			qualifierResult.Add(ch);
		  else
			propertyName.Add(ch);
		}
		index--;
		if (index < 0)
		  break;
		ch = InternalName[index];
	  }
	  if (qualifierResult.Count > 0)
	  {
		qualifierResult.Reverse();
		qualifierName = new string(qualifierResult.ToArray());
	  }
	  else
		qualifierName = null;
	  if (propertyName.Count > 0)
	  {
		propertyName.Reverse();
		return new string(propertyName.ToArray());
	  }
	  return null;
	}
	#region CloseNameRange
	public SourceRange CloseNameRange
	{
	  get
	  {
		if (!HasCloseTag)
		  return SourceRange.Empty;
		SourceRange range = NameRange;
		int tagLength = range.End.Offset - range.Start.Offset;
		SourcePoint end = Range.End;
		int endLine = end.Line;
		int endOffset = end.Offset;
		return new SourceRange(endLine, endOffset - tagLength - 1, endLine, endOffset - 1);
	  }
	}
	#endregion
	#region CloseTagNameRange
	public override SourceRange CloseTagNameRange
	{
	  get
	  {
		string tagName = TagName;
		if (!HasCloseTag || tagName == null)
		  return SourceRange.Empty;
		SourceRange range = CloseNameRange;
		SourcePoint end = range.End;
		SourcePoint start = new SourcePoint(end.Line, end.Offset - tagName.Length);
		return new SourceRange(start, end);
	  }
	}
	#endregion
	#region CloseTagPrefixRange
	public SourceRange CloseTagPrefixRange
	{
	  get
	  {
		string tagPrefix = TagPrefix;
		if (!HasCloseTag || tagPrefix == null)
		  return SourceRange.Empty;
		SourceRange range = CloseNameRange;
		SourcePoint start = range.Start;
		SourcePoint end = new SourcePoint(start.Line, start.Offset + tagPrefix.Length);
		return new SourceRange(start, end);
	  }
	}
	#endregion
  }
}
