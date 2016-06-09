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
	public class AddressOfExpression : Expression, IAddressOfExpression
	{
		const int INT_MaintainanceComplexity = 2;
		Expression _Exp;
		public AddressOfExpression()			
		{
		}
		public AddressOfExpression(Expression expression)
	  : this(null, expression)
		{
		}
	public AddressOfExpression(Token token, Expression expression)
	{
	  SetOperator(token);
	  SetExpression(expression);
	}
		#region SetOperator(Token token)
	void SetOperator(Token token)
	{
	  if (token == null)
		return;
	  InternalName = token.EscapedValue;
	  SetStart(token.Range);
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
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is AddressOfExpression))
				return;
			AddressOfExpression lSource = (AddressOfExpression)source;
			if (lSource._Exp != null)
			{				
				_Exp = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Exp) as Expression;
				if (_Exp == null)
					_Exp = lSource._Exp.Clone(options) as Expression;
			}
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (InternalName != null)
				lResult += InternalName + " ";
			if (_Exp != null)
				lResult += _Exp.ToString();
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.AddressOfExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is AddressOfExpression)
			{
				AddressOfExpression lExpression = expression as AddressOfExpression;
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
			_Exp = null;
			base.CleanUpOwnedReferences();
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			AddressOfExpression lClone = new AddressOfExpression();
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
				return LanguageElementType.AddressOfExpression;
			}
		}
		#region Operator
		public string Operator
		{
			get
			{
				return InternalName;
			}
		}
		#endregion
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
		#region IAddressOfExpression Members
		IExpression IAddressOfExpression.Expression
		{
			get
			{
				return Expression;
			}
		}
		#endregion
	}
}
