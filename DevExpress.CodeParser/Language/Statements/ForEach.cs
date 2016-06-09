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
	public class ForEach : ConditionalParentToSingleStatement, IForEachStatement
	{
		private const int INT_MaintainanceComplexity = 3;
	  Expression _Expression;
	BaseVariable _LoopVariable;
		public string FieldType = String.Empty;
	  public string Field = String.Empty;
	  public string Collection;
		public ForEach()
		{
			IsBreakable = true;
	}
	public ForEach(BaseVariable loop, Expression expression, Expression nextExpression):
	  this()
	{
	  SetLoopVariable(loop);
	  SetExpression(expression);
	  SetNextExpression(nextExpression);
	}
		internal void SetExpression(Expression value)
		{
	  Expression oldExpression = _Expression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Expression = value;
	  if (_Expression != null)
		AddDetailNode(_Expression); 
		} 
	internal void SetLoopVariable(BaseVariable value)
	{
	  BaseVariable oldVariable = _LoopVariable;
	  if (oldVariable != null)
		oldVariable.RemoveFromParent();
	  _LoopVariable = value;
	  if (_LoopVariable != null)
		AddDetailNode(_LoopVariable);
	}
	protected virtual void SetNextExpression(Expression value)
	{
	}
	protected virtual void SetInitializer(LanguageElement value)
	{
	}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
	  if (_Expression == oldElement && newElement is Expression)
		Expression = (Expression)newElement;
	  else if (_LoopVariable == oldElement && newElement is BaseVariable)
		_LoopVariable = (BaseVariable)newElement;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
		}		
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ForEach))
				return;
			ForEach lSource = (ForEach)source;
			FieldType = lSource.FieldType;
			Field = lSource.FieldType;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
	  if (lSource._LoopVariable != null)
	  {
		_LoopVariable = ParserUtils.GetCloneFromNodes(this, lSource, lSource._LoopVariable) as BaseVariable;
		if (_LoopVariable == null)
		  _LoopVariable = lSource._LoopVariable.Clone(options) as BaseVariable;
	  }
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ForEach;	
		}
		#endregion
		public override void CleanUpOwnedReferences()
		{
			_Expression = null;
	  _LoopVariable = null;
			base.CleanUpOwnedReferences();
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ForEach lClone = new ForEach();
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
				return LanguageElementType.ForEach;
			}
		}
		#region Expression
		public Expression Expression
		{
			get
			{
				return _Expression;
			}
			set
			{
				SetExpression(value);
			}
		}
		#endregion
	public BaseVariable LoopVariable
	{
	  get
	  {
		return _LoopVariable;
	  }
	}
		#region NextExpression
		public virtual Expression NextExpression
		{
			set
			{
				;
			}
			get
			{
				return null;
			}
		}
			#endregion
		#region IForEachStatement Members
		IVariableDeclarationStatement IForEachStatement.LoopVariable
		{
			get 
			{
		return _LoopVariable;
			}
		}
		IExpression IForEachStatement.Expression
		{
			get 
			{
				return Expression;
			}
		}
		IExpression IForEachStatement.NextExpression
		{
			get
			{
				return NextExpression;
			}
		}
		#endregion
	}
}
