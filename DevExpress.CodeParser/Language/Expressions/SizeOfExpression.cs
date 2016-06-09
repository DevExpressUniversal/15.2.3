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
	public class SizeOfExpression : Expression, ISizeOfExpression
	{
		const int INT_MaintainanceComplexity = 3;
		#region private fields...
		Expression _TypeReference;
		bool _IsParenthizedExpression = false;
		#endregion
		#region SizeOfExpression
		protected SizeOfExpression()
		{			
		}
		#endregion
		#region SizeOfExpression
		public SizeOfExpression(Expression type)
		{
	  SetTypeReference(type);
		}
		#endregion
		#region SizeOfExpression
		public SizeOfExpression(TypeReferenceExpression type)
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
				_TypeReference = (Expression)newElement;
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
			if (!(source is SizeOfExpression))
				return;
			SizeOfExpression lSource = (SizeOfExpression)source;
			_IsParenthizedExpression = lSource.IsParenthizedExpression;
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
				return "sizeof(" + _TypeReference.ToString() + ")";
			return base.ToString();
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is SizeOfExpression)
			{
				SizeOfExpression lExpression = (SizeOfExpression)expression;
				if (TypeReference == null)
					return false;
				return TypeReference.IsIdenticalTo(lExpression.TypeReference);
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
			SizeOfExpression lClone = new SizeOfExpression();
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
				return LanguageElementType.SizeOfExpression;
			}
		}
		public bool IsParenthizedExpression
		{
			get
			{
				return _IsParenthizedExpression;
			}
			set
			{
				_IsParenthizedExpression = value;
			}
		}
		#region TypeReference
		public Expression TypeReference
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
		#region ISizeOfExpression Members
		IExpression ISizeOfExpression.TypeReference
		{
			get
			{
				return TypeReference;
			}
		}
		#endregion
	}
}
