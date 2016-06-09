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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class While : ConditionalParentToSingleStatement, IWhileStatement
	{
		const int INT_MaintainanceComplexity = 3;
	  Expression _Condition;
		public While()
		{
	  IsBreakable = true;
	}
		public While(Expression expression, LanguageElementCollection block)
			: this (expression, block, SourceRange.Empty)
		{
		}
		public While(Expression expression, LanguageElementCollection block, SourceRange range)
		{
			IsBreakable = true;
	  SetExpression(expression);
			SetRange(range);
			AddNodes(block);
		}
	protected void SetExpression(Expression expression)
	{
	  Expression oldExpression = _Condition;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Condition = expression;
	  if (_Condition != null)
		AddDetailNode(_Condition);
	}
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is While))
				return;
			While lSource = (While)source;			
			if (lSource._Condition != null)
			{				
				_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as Expression;
				if (_Condition == null)
					_Condition = lSource._Condition.Clone(options) as Expression;
			}
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Condition == oldElement)
				_Condition = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.WhileDo;	
		}
		#endregion
		#region GetDetailNodeDescription
		public override string GetDetailNodeDescription(int index)
		{
			return "While-loop expression";
		}
		#endregion
		#region SetCondition
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCondition(Expression condition)
		{
			_Condition = condition;
			AddDetailNode(condition);
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			While lClone = new While();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.While;
			}
		}
		public Expression Condition
		{
			get
			{
				return _Condition;
			}
	  set
	  {
		SetExpression(value);
	  }
		}
		IExpression IWhileStatement.Condition
		{
			get 
			{
				return Condition;
			}
		}		
	}
}
