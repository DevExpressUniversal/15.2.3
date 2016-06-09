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
	public class TypeCastExpression : Expression, ITypeCastExpression
	{
		const int INT_MaintainanceComplexity = 4;
		#region private fields...
		TypeReferenceExpression _TypeReference;
		Expression _Target;
		#endregion		
		#region TypeCastExpression
		public TypeCastExpression()
		{
		}
		#endregion
		#region TypeCastExpression
		public TypeCastExpression(TypeReferenceExpression type, Expression target)
		{
			SetTypeReference(type);
			SetTarget(target);
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (_TypeReference != null)
				lResult = "(" + _TypeReference.ToString() + ")";
			if (_Target != null)
				lResult += _Target.ToString();
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.TypeCast;
		}
		#endregion
		#region SetTypeReference(TypeReferenceExpression type)
		public void SetTypeReference(TypeReferenceExpression type)
		{
	  if (_TypeReference != null)
		RemoveDetailNode(_TypeReference);
	  _TypeReference = type;
	  if (_TypeReference != null)
			{
		AddDetailNode(_TypeReference);
		SetStart(_TypeReference.Range);
			}
		}
		#endregion
		#region SetTarget(Expression target)
		public void SetTarget(Expression target)
		{
	  if (_Target != null)
		RemoveNode(_Target);
	  _Target = target;
	  if (_Target != null)
			{
		AddNode(_Target);
		SetEnd(_Target.Range);
			}
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is TypeCastExpression)
			{
				TypeCastExpression lExpression = (TypeCastExpression)expression;
				if (TypeReference == null || !TypeReference.IsIdenticalTo(lExpression.TypeReference))
					return false;
				if (Target == null || !Target.IsIdenticalTo(lExpression.Target))
					return false;
				return true;
			}
			return false;
		}
		#endregion
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver != null)
				return resolver.Resolve(this);
			return null;
		}
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_TypeReference = null;
			_Target = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_TypeReference == oldElement)
				_TypeReference = (TypeReferenceExpression)newElement;
			else if (_Target == oldElement)
				_Target = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is TypeCastExpression))
				return;
			TypeCastExpression lSource = (TypeCastExpression)source;
			if (lSource.Target != null)
			{				
				_Target = ParserUtils.GetCloneFromNodes(this, lSource, lSource.Target) as Expression;
				if (_Target == null)
					_Target = lSource.Target.Clone(options) as Expression;
			}
			if (lSource.TypeReference != null)
			{
				_TypeReference = ParserUtils.GetCloneFromNodes(this, lSource, lSource.TypeReference) as TypeReferenceExpression;
				if (_TypeReference == null)
					_TypeReference = lSource.TypeReference.Clone(options) as TypeReferenceExpression;
			}
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			TypeCastExpression clone = new TypeCastExpression();
			clone.CloneDataFrom(this, options);
			return clone;
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
	  get { return INT_MaintainanceComplexity; }
		}
		public override LanguageElementType ElementType
		{
	  get { return LanguageElementType.TypeCastExpression; }
		}
		#region TypeReference
		public TypeReferenceExpression TypeReference
		{
	  get { return _TypeReference; }
	  set { SetTypeReference(value); }
		}
		#endregion
		#region Target
		public Expression Target
		{
	  get { return _Target; }
	  set { SetTarget(value); }
		}
		#endregion
		#region ITypeCastExpression Members
		ITypeReferenceExpression ITypeCastExpression.TypeReference
		{
	  get { return TypeReference; }
		}
		IExpression ITypeCastExpression.Target
		{
	  get { return Target; }
		}
		#endregion
	}
}
