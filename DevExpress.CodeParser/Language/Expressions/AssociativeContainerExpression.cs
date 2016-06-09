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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class AssociativeContainerExpression : Expression 
	{
		ExpressionCollection _Expressions;
		private AssociativeContainerExpression()
		{			
		}
		public AssociativeContainerExpression(ExpressionCollection expressions)
		{
			if (expressions == null)
				throw new ArgumentNullException("expressions");
			_Expressions = expressions;
			SetAssociativeRange();
		}
		void SetAssociativeRange()
		{
			if (Expressions == null || Expressions.Count == 0)
				return;
			SourcePoint lTop = SourcePoint.Empty;
			SourcePoint lBottom = SourcePoint.Empty;
			for (int i = 0; i < Expressions.Count; i++)
			{
				Expression lExpression = Expressions[i];
				SourceRange lRange = lExpression.Range;
				if (lTop.IsEmpty)
					lTop = lRange.Top;
				if (lBottom.IsEmpty)
					lBottom = lRange.Bottom;
				if (lRange.Top < lTop)
					lTop = lRange.Top;
				if (lRange.Bottom > lBottom)
					lBottom = lRange.Bottom;
			}
			SetRange(new SourceRange(lTop, lBottom));
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AssociativeContainerExpression))
				return;
			AssociativeContainerExpression lSource = (AssociativeContainerExpression)source;
			if (lSource._Expressions != null)
			{
				_Expressions = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Expressions, lSource._Expressions);
				if (_Expressions.Count == 0 && lSource._Expressions.Count > 0)
					_Expressions = lSource._Expressions.DeepClone(options) as ExpressionCollection;
			}
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expressions != null && _Expressions.Contains(oldElement))
		_Expressions.ReplaceExpression(oldElement as Expression, newElement as Expression);
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver == null)
				return null;
			if (Expressions.Count == 0)
				return null;
			Expression lExpression = Expressions[0];
			if (lExpression == null)
				return null;
			return lExpression.Resolve(resolver);
		}
		public void AttachToSourceTree()
		{
			if (Expressions.Count == 0)
				return;
			Expression lExpression = Expressions[0];
			SetParent(lExpression.Parent);
		}
		public void DetachFromSourceTree()
		{
			SetParent(null);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AssociativeContainerExpression lClone = new AssociativeContainerExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public ExpressionCollection Expressions
		{
	  get { return _Expressions; }
		}
	}
}
