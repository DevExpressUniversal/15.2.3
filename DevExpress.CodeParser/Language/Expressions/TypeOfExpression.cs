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
	public class TypeOfExpression : Expression, ITypeOfExpression
	{
		private const int INT_MaintainanceComplexity = 3;
		#region private fields...
	Expression _TypeReference;
		#endregion
		#region TypeOfExpression
		protected TypeOfExpression()
		{			
		}
		#endregion
		#region TypeOfExpression
		public TypeOfExpression(Expression type)
		{
	  SetTypeReference(type);
		}
		#endregion
		#region TypeOfExpression
		public TypeOfExpression(TypeReferenceExpression type)
		{
	  SetTypeReference(type);
		}
		#endregion
	void SetTypeReference(Expression type)
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
				_TypeReference = newElement as Expression;
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
			if (!(source is TypeOfExpression))
				return;
			TypeOfExpression lSource = (TypeOfExpression)source;
			if (lSource._TypeReference != null)
			{				
				_TypeReference = ParserUtils.GetCloneFromNodes(this, lSource, lSource._TypeReference) as Expression;
				if (_TypeReference == null)
		  _TypeReference = lSource._TypeReference.Clone(options) as Expression;
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
			return ImageIndex.TypeOf;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is TypeOfExpression)
			{
				TypeOfExpression lExpression = (TypeOfExpression)expression;
				if (Expression == null)
					return false;
		return Expression.IsIdenticalTo(lExpression.Expression);
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
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeOfExpression lClone = new TypeOfExpression();
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
				return LanguageElementType.TypeOfExpression;
			}
		}
		#region TypeReference
		public TypeReferenceExpression TypeReference
		{
			get
			{
				return _TypeReference as TypeReferenceExpression;
			}
			set
			{
		SetTypeReference(value);
			}
		}
		#endregion
		#region Expression
		public Expression Expression
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
		#region ITypeOfExpression Members
		ITypeReferenceExpression ITypeOfExpression.TypeReference
		{
			get
			{
				return TypeReference;
			}
		}
		#endregion
	}
}
