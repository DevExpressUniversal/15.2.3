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
	public class TypeOfIsExpression : Expression, ITypeOfIsExpression
	{
		private const int INT_MaintainanceComplexity = 6;
		#region private fields...
		Expression _Exp;
		TypeReferenceExpression _TypeReference;
		#endregion
		#region TypeOfIsExpression
		protected TypeOfIsExpression()
		{			
		}
		#endregion
		#region TypeOfIsExpression
		public TypeOfIsExpression(Expression expression, TypeReferenceExpression type)
		{
	  SetExpression(expression);
	  SetTypeReference(type);
		}
		#endregion
	void SetExpression(Expression expression)
	{
	  if (_Exp != null)
		RemoveDetailNode(_Exp);
	  _Exp = expression;
	  if (_Exp != null)
		AddDetailNode(_Exp);
	}
	void SetTypeReference(TypeReferenceExpression type)
	{
	  if (_TypeReference != null)
		RemoveDetailNode(_TypeReference);
	  _TypeReference = type;
	  if (_TypeReference != null)
		AddDetailNode(_TypeReference);
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_TypeReference == oldElement)
				_TypeReference = (TypeReferenceExpression)newElement;
			else if (_Exp == oldElement)
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
			if (!(source is TypeOfIsExpression))
				return;
			TypeOfIsExpression lSource = (TypeOfIsExpression)source;
			if (lSource._Exp != null)
			{				
				_Exp = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exp) as Expression;
				if (_Exp == null)
					_Exp = lSource._Exp.Clone(options) as Expression;
			}
			if (lSource._TypeReference != null)
			{				
				_TypeReference = ParserUtils.GetCloneFromNodes(this, lSource, lSource._TypeReference) as TypeReferenceExpression;
				if (_TypeReference == null)
					_TypeReference = lSource._TypeReference.Clone(options) as TypeReferenceExpression;
			}			
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			if (_TypeReference != null)
				return "typeof(" + _TypeReference.ToString() + ")";
			return base.ToString();
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.IsExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is TypeOfIsExpression)
			{
				TypeOfIsExpression lExpression = (TypeOfIsExpression)expression;
				if (TypeReference == null)
					return false;
				if(!TypeReference.IsIdenticalTo(lExpression.TypeReference))
					return false;
				if (Expression == null)
					return false;
				if(!Expression.IsIdenticalTo(lExpression.Expression))
					return false;
				return true;
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
			_TypeReference = null;
			_Exp = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeOfIsExpression lClone = new TypeOfIsExpression();
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
				return LanguageElementType.TypeOfIsExpression;
			}
		}
		#region Expression
		public Expression Expression
		{
			get
			{
				return _Exp;
			}
			set
			{
		SetExpression(value);
			}
		}
		#endregion
		#region TypeReference
		public TypeReferenceExpression TypeReference
		{
			get
			{
				return _TypeReference;
			}
			set
			{
		SetTypeReference(value);
			}
		}
		#endregion
		#region ITypeOfIsExpression Members
		IExpression ITypeOfIsExpression.Expression
		{
			get
			{
				return Expression;
			}
		}
		ITypeReferenceExpression ITypeOfIsExpression.TypeReference
		{
			get
			{
				return TypeReference;
			}
		}
		#endregion
		#region IBinaryOperatorExpression Members
		IExpression IBinaryOperatorExpression.LeftSide
		{
			get
			{
				return Expression;
			}
		}
	BinaryOperatorType IBinaryOperatorExpression.BinaryOperator
	{
	  get
	  {
		return BinaryOperatorType.None;
	  }
	}
		IExpression IBinaryOperatorExpression.RightSide
		{
			get
			{
				return TypeReference;
			}
		}
		#endregion
	}
}
