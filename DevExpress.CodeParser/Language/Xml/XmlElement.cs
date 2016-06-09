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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum XmlElementType : byte
  {
	[EditorBrowsable(EditorBrowsableState.Never)]
	NotSet,
	Unknown,
	Summary,
	Param,
	SeeAlso,
	Returns,
	FormattedSingleLineCode,
	FormattedMultiLineCode,
	List,
	Item,
	Paragraph,
	ParamRef,
	Description,
	Include,
	Example,
	Exception,
	Permission,
	Value,
	Remarks
  }
  public class XmlElement : XmlNode, ICapableBlock, IMarkupElement
  {
	#region private fields...
	private XmlElementType _XmlElementType;
	TextRangeWrapper _BlockStart;
	TextRangeWrapper _BlockEnd;
	bool _HasCloseTag = true;
	#endregion
	#region XmlElement
	public XmlElement()
	{
	  _XmlElementType = XmlElementType.NotSet;		
	}
	#endregion
	#region XmlElement
	public XmlElement(string name)
	  : this()
	{
	  InternalName = name;
	}
	#endregion
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is XmlElement))
		return;
	  XmlElement lSource = (XmlElement)source;
	  _XmlElementType = lSource._XmlElementType;
	  _HasCloseTag = lSource._HasCloseTag;
	}
	#endregion
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  switch (XmlElementType)
	  {
		case XmlElementType.Summary:
		  return ImageIndex.DocCommentSummary;
		case XmlElementType.Param:
		  return ImageIndex.DocCommentParam;
		case XmlElementType.SeeAlso:
		  return ImageIndex.DocCommentSeeAlso;
		case XmlElementType.Returns:
		  return ImageIndex.DocCommentReturns;
		case XmlElementType.FormattedSingleLineCode:
		  return ImageIndex.DocCommentCodeSingleLine;
		case XmlElementType.FormattedMultiLineCode:
		  return ImageIndex.DocCommentCodeMultiLine;
		case XmlElementType.List:
		  return ImageIndex.DocCommentList;
		case XmlElementType.Item:
		  return ImageIndex.DocCommentItem;
		default:
		  return ImageIndex.DocCommentElement;
	  }
	}
	#endregion
	#region ToString
	public override string ToString()
	{
	  return "<" + InternalName + ">";
	}
	#endregion
	#region GetAttribute(string attributeName)
	public XmlAttribute GetAttribute(string attributeName)
	{
	  return GetAttribute(attributeName, false);
	}
	#endregion
	#region GetAttribute(string attributeName, bool ignoreCase)
	public XmlAttribute GetAttribute(string attributeName, bool ignoreCase)
	{
	  StringComparison stringComparison = ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
	  for (int i = 0; i < DetailNodeCount; i++)
	  {
		if (DetailNodes[i] is XmlAttribute)
		{
		  XmlAttribute lXmlAttribute = (XmlAttribute)DetailNodes[i];
		  if (String.Compare(lXmlAttribute.Name, attributeName, stringComparison) == 0)
			return lXmlAttribute;
		}
	  }
	  return null;
	}
	#endregion
	#region GetValue
	public string GetValue(string attributeName)
	{
	  XmlAttribute lXmlAttribute = GetAttribute(attributeName);
	  if (lXmlAttribute != null)
		return lXmlAttribute.Value;
	  else
		return String.Empty;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  XmlElement lClone = new XmlElement();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region SetBlockStart()
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetBlockStart(SourceRange range)
	{
	  ClearHistory();
	  _BlockStart = range;
	}
	#endregion
	#region SetBlockEnd()
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void SetBlockEnd(SourceRange range)
	{
	  ClearHistory();
	  _BlockEnd = range;
	}
	#endregion
	#region HasCloseTag
	public bool HasCloseTag
	{
	  get
	  {
		return _HasCloseTag;
	  }
	  set
	  {
		_HasCloseTag = value;
	  }
	}
	#endregion
	#region XmlAttributes
	public NodeList XmlAttributes
	{
	  get
	  {
		return InnerDetailNodes;
	  }
	}
	#endregion
	#region HasAttributes
	public bool HasAttributes
	{
	  get
	  {
		return XmlAttributes.Count > 0;
	  }
	}
	#endregion
	#region XmlElementType
	public XmlElementType XmlElementType
	{
	  get
	  {
		if (_XmlElementType == XmlElementType.NotSet)
		{
		  switch (InternalName)
		  {
			case "summary":
			  _XmlElementType = XmlElementType.Summary;
			  break;
			case "param":
			  _XmlElementType = XmlElementType.Param;
			  break;
			case "seealso":
			  _XmlElementType = XmlElementType.SeeAlso;
			  break;
			case "returns":
			  _XmlElementType = XmlElementType.Returns;
			  break;
			case "c":
			  _XmlElementType = XmlElementType.FormattedSingleLineCode;
			  break;
			case "code":
			  _XmlElementType = XmlElementType.FormattedMultiLineCode;
			  break;
			case "list":
			  _XmlElementType = XmlElementType.List;
			  break;
			case "item":
			  _XmlElementType = XmlElementType.Item;
			  break;
			case "description":
			  _XmlElementType = XmlElementType.Description;
			  break;
			case "example":
			  _XmlElementType = XmlElementType.Example;
			  break;
			case "exception":
			  _XmlElementType = XmlElementType.Exception;
			  break;
			case "include":
			  _XmlElementType = XmlElementType.Include;
			  break;
			case "para":
			  _XmlElementType = XmlElementType.Paragraph;
			  break;
			case "paramref":
			  _XmlElementType = XmlElementType.ParamRef;
			  break;
			case "permission":
			  _XmlElementType = XmlElementType.Permission;
			  break;
			case "value":
			  _XmlElementType = XmlElementType.Value;
			  break;
			case "remarks":
			  _XmlElementType = XmlElementType.Remarks;
			  break;
			default:
			  _XmlElementType = XmlElementType.Unknown;
			  break;
		  }
		}
		return _XmlElementType;
	  }
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.XmlDocElement;
	  }
	}
	public DelimiterBlockType BlockType
	{
	  get
	  {
		return DelimiterBlockType.Middle;
	  }
	}
	#region HasDelimitedBlock
	[Category("Block Delimiters")]
	public virtual bool HasDelimitedBlock
	{
	  get
	  {
		return BlockStart != SourceRange.Empty && BlockEnd != SourceRange.Empty;
	  }
	}
	#endregion
	[Category("Block Delimiters")]
	public SourceRange BlockStart
	{
	  get
	  {
		return GetTransformedRange(_BlockStart);
	  }
	}
	[Category("Block Delimiters")]
	public SourceRange BlockEnd
	{
	  get
	  {
		return GetTransformedRange(_BlockEnd);
	  }
	}
  }
}
