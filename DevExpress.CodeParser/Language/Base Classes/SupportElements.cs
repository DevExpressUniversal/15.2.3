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
	class SupportElements
	{
		NodeList _Attributes;
		NodeList _AttributeSections;
		CommentCollection _Comments;
		XmlDocComment _DocComment;
		public SupportElements()
		{
		}
		public void CleanUp()
		{
			if (_DocComment != null)
				_DocComment = null;
			if (_Comments != null)
			{
				_Comments.Clear();
				_Comments = null;
			}
			if (_AttributeSections != null)
			{
				_AttributeSections.Clear();
				_AttributeSections = null;
			}
		}
		public void TransferCommentsTo(CodeElement element)
		{
			if (element == null || _Comments == null)
				return;
			foreach (Comment lComment in _Comments)
			{
				_Comments.Remove(lComment);
				element.Comments.Add(lComment);
			}
		}
		public void TransferXmlDocCommentTo(CodeElement element)
		{
			if (element == null || _DocComment == null)
				return;
			element.SetDocComment(_DocComment);
			_DocComment = null;
		}
	public void AddComment(Comment comment)
	{
	  if (comment == null)
		return;
	  InnerComments.Add(comment);
	}
	public void InsertComment(int index, Comment comment)
		{
	  if (comment == null)
		return;
	  InnerComments.Insert(index, comment);			  
		}
	public void RemoveComment(Comment comment)
	{
	  if (comment == null)
		return;
	  InnerComments.Remove(comment);
	}
		public void AddAttribute(Attribute attribute)
		{
			if (attribute == null)
				return;
			InnerAttributes.Add(attribute);
		}
		public void AddAttributeSection(AttributeSection section)
		{
			if (section == null)
				return;
			InnerAttributeSections.Add(section);
		}
		public bool ContainsElement(LanguageElement element)
		{
			if (_Attributes != null && _Attributes.Contains(element))
				return true;
			if (_AttributeSections != null && _AttributeSections.Contains(element))
				return true;
			if (_Comments != null && _Comments.Contains(element))
				return true;
			if (_DocComment == element)
				return true;
			return false;
		}
		public void ReplaceElement(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Attributes != null && _Attributes.Contains(oldElement))
				_Attributes.Replace(oldElement, newElement);
			if (_AttributeSections != null && _AttributeSections.Contains(oldElement))
				_AttributeSections.Replace(oldElement, newElement);
			if (_Comments != null && _Comments.Contains(oldElement))
				_Comments.Replace(oldElement, newElement);
			if (_DocComment == oldElement)
				_DocComment = newElement as XmlDocComment;
		}
		public NodeList Attributes
		{
			get
			{
				if (_Attributes == null)
					_Attributes = new NodeList();
				return _Attributes;
			}
			set
			{
				_Attributes = value;
			}
		}
		public int AttributeCount
		{
			get
			{
				return _Attributes == null ? 0 : _Attributes.Count;
			}
		}
		public NodeList AttributeSections
		{
			get
			{
				if (_AttributeSections == null)
					_AttributeSections = new NodeList();
				return _AttributeSections;
			}
			set
			{
				_AttributeSections = value;
			}
		}
		[Description("The number of AttributeSections associated with this element.")]
		[Category("SupportElements")]
		public int AttributeSectionsCount
		{
			get
			{
				return _AttributeSections == null ? 0 : _AttributeSections.Count;
			}
		}
		[Description("A create-on-demand collection of comments bound to this element.")]
		[Category("SupportElements")]
		public CommentCollection InnerComments
		{
			get
			{
				if (_Comments == null)
					_Comments = new CommentCollection();
				return _Comments;
			}
			set
			{
				_Comments = value;
			}
		}
		public NodeList InnerAttributes
		{
			get
			{
				if (_Attributes == null)
					_Attributes = new NodeList();
				return _Attributes;
			}
			set
			{
				_Attributes = value;
			}
		}
		public NodeList InnerAttributeSections
		{
			get
			{
				if (_AttributeSections == null)
					_AttributeSections = new NodeList();
				return _AttributeSections;
			}
			set
			{
				_AttributeSections = value;
			}
		}
		[Description("The number of comments bound to this element.")]
		[Category("SupportElements")]
		public int CommentCount
		{
			get
			{
				return InnerComments == null ? 0 : InnerComments.Count;
			}
		}
		[Description("Gets xml doc comment associated with this element.")]
		[Category("SupportElements")]
		public XmlDocComment DocComment
		{
			get
			{
				return _DocComment;
			}
			set
			{
				_DocComment = value;
			}
		}
	}
}
