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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class JoinIntoExpression: JoinExpression, IIntoContainingElement, IJoinIntoExpression
	{		
		LanguageElementCollection _IntoElements = null;
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is JoinIntoExpression))
				return;
			JoinIntoExpression lSource = source as JoinIntoExpression;
	  if (lSource._IntoElements != null)
	  {
		_IntoElements = new LanguageElementCollection();
		ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _IntoElements, lSource._IntoElements);
		if (_IntoElements.Count == 0 && lSource._IntoElements.Count > 0)
		  _IntoElements = lSource._IntoElements.DeepClone(options);
	  }
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_IntoElements != null && _IntoElements.Contains(oldElement))
				_IntoElements.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			JoinIntoExpression lClone = new JoinIntoExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
	public void AddIntoElements(IEnumerable<LanguageElement> elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddIntoElement(element);
	}
		#region AddIntoElement
		public void AddIntoElement(LanguageElement element)
		{
			if (element == null)
				return;
			IntoElements.Add(element);
			AddDetailNode(element);
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return "JoinIntoExpression";
		}
		#endregion
		#region IntoElements
		public LanguageElementCollection IntoElements
		{
			get
			{
				if (_IntoElements == null)
					_IntoElements = new LanguageElementCollection();
				return _IntoElements;
			}
		}
		#endregion
	IElementCollection IJoinIntoExpression.IntoElements
	{
	  get
	  {
		IElementCollection result = new IElementCollection();
		result.AddRange(IntoElements);
		return result;
	  }
	}
		#region ElementType
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.JoinIntoExpression;
			}
		}
		#endregion
	}
}
