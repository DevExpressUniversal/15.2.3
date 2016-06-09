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
	public enum ArgumentDirection : byte
	{
		In,
		Out,
		Ref,
		ParamArray,
	ArgList
	}
	public class ArgumentDirectionExpression : Expression, IArgumentDirectionExpression
	{
		const int INT_MaintainanceComplexity = 3;
		#region private fields...
		Expression _Exp;
		ArgumentDirection _Direction;
		TextRange _NameRange;
		#endregion
		#region ArgumentDirectionExpression
		public ArgumentDirectionExpression()
		{
		}
		#endregion
		#region ArgumentDirectionExpression
		public ArgumentDirectionExpression(Token token, Expression expression)
		{
			SetDirectionToken(token);
			SetExpression(expression);
		}
		#endregion
		#region ArgumentDirectionExpression
		public ArgumentDirectionExpression(ArgumentDirection direction, Expression expression)
		{
			SetDirection(direction);
			SetExpression(expression);
		}
		#endregion
		#region SetDirectionToken(Token token)
		void SetDirectionToken(Token token)
		{
	  if (token == null)
		return;
	  InternalName = token.EscapedValue;
	  SetNameRange(token.Range);
	  SetStart(token.Range);
		}
		#endregion
		#region SetDirection(ArgumentDirection direction)
		void SetDirection(ArgumentDirection direction)
		{
			_Direction = direction;
		}
		#endregion
		#region SetExpression(Expression expression)
		void SetExpression(Expression expression)
		{
	  if (_Exp != null)
		RemoveDetailNode(_Exp);
	  _Exp = expression;
	  if (_Exp != null)
			{
				AddDetailNode(_Exp);
		SetEnd(_Exp.InternalRange);
			}
		}
		#endregion
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Exp == oldElement)
				_Exp = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is ArgumentDirectionExpression))
				return;
			ArgumentDirectionExpression lSource = (ArgumentDirectionExpression)source;
			_Direction = lSource._Direction;
			_NameRange = lSource.NameRange;
			if (lSource._Exp != null)
			{				
				_Exp = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exp) as Expression;
				if (_Exp == null)
					_Exp = lSource._Exp.Clone(options) as Expression;
			}						
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	protected void SetNameRange(SourceRange value)
	{
	  ClearHistory();
	  _NameRange = value;
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ArgumentDirection;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = Name + " ";
			if (_Exp != null)
				lResult += _Exp.ToString();
			return lResult;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is ArgumentDirectionExpression)
			{
				ArgumentDirectionExpression lExpression = expression as ArgumentDirectionExpression;
				if (Expression == null)
					return false;
				return Expression.IsIdenticalTo(lExpression.Expression) && Direction == lExpression.Direction;
			}
			return false;
		}
		#endregion
		#region Resolve
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Exp = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			ArgumentDirectionExpression lClone = new ArgumentDirectionExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.ArgumentDirectionExpression; }
		}
		#endregion
		#region NameRange
		public override SourceRange NameRange
		{
	  get { return GetTransformedRange(_NameRange); }
		}
		#endregion
		#region Direction
		public ArgumentDirection Direction
		{
	  get { return _Direction; }
	  set { SetDirection(value); }
		}
		#endregion
		#region Expression
		public Expression Expression
		{
	  get { return _Exp; }
	  set { SetExpression(value); }
		}
		#endregion
		#region IArgumentDirectionExpression Members
		ArgumentDirection IArgumentDirectionExpression.Direction
		{
	  get { return Direction; }
		}
		IExpression IArgumentDirectionExpression.Expression
		{
	  get { return Expression; }
		}
		#endregion
	}
}
