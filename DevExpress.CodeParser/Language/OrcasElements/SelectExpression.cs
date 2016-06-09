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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	#region SelectExpression
	public class SelectExpression: QueryExpressionBase
	{
		LanguageElementCollection _ReturnedElements;
		public  SelectExpression()
		{
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is SelectExpression))
				return;
			SelectExpression lSource = source as SelectExpression;
			if (lSource._ReturnedElements != null)
			{
				_ReturnedElements = new LanguageElementCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _ReturnedElements, lSource._ReturnedElements);
				if (_ReturnedElements.Count == 0 && lSource._ReturnedElements.Count > 0)
				{
					for (int i = 0; i < lSource._ReturnedElements.Count; i++)
					{
						LanguageElement element = lSource._ReturnedElements[i];
						_ReturnedElements.Add(element.Clone());
					}
				}
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_ReturnedElements != null && _ReturnedElements.Contains(oldElement))
				_ReturnedElements.Replace(oldElement, newElement);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		public override BaseElement Clone(ElementCloneOptions options)
		{
			SelectExpression lClone = new SelectExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddReturnedExpression(LanguageElement element)
		{
			AddReturnedElement(element);
		}
	public void AddReturnedElements(IEnumerable<LanguageElement> elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddReturnedElement(element);
	}
		public void AddReturnedElement(LanguageElement element)
		{
			if (element == null)
				return;
			ReturnedElements.Add(element);
			AddDetailNode(element);
		}	
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElementCollection ReturnedElements
		{
			get
			{
				if (_ReturnedElements == null)
				{
					_ReturnedElements = new LanguageElementCollection();
				}
				return _ReturnedElements;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.SelectExpression;
			}
		}
		public override string ToString()
		{
			return "SelectExpression";
		}
	}
	#endregion
}
