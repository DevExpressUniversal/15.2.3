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
	public abstract class InitializedValue : Variable, IFieldElement, IVariableDeclarationStatement
	{
		Expression _Expression;
		bool _IsShortInitialize;
		public InitializedValue()
		{
		}		
		public InitializedValue(string type, string name)
			: this(type, name, null)
		{
		}
		public InitializedValue(string type, string name, Expression expr)
			: base(type, name)
		{
			if (expr != null)
				Expression = expr;
		}
	protected void SetExpression(Expression expression)
	{
	  Expression oldExpression = _Expression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression); 
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Expression == oldElement)
		_Expression = newElement as Expression;
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is InitializedValue))
				return;			
			InitializedValue lSource = (InitializedValue)source;
			if (lSource._Expression != null)
			{
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
			_IsShortInitialize = lSource.IsShortInitialize;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Expression = null;
			base.CleanUpOwnedReferences();
		}
		#region GetDetailNodeDescription
		public override string GetDetailNodeDescription(int index)
		{
			if (index == 0)
				return "Variable initialization";
			else
				return base.GetDetailNodeDescription(index);
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return String.Format("{0} = {1}", base.ToString(), _Expression);
		}
		#endregion
		#region Expression
		[Description("The expression used to initialize this variable.")]
		[Category("")]
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
		#region IFieldElement Members
		public bool IsShortInitialize
		{
			get
			{
				return _IsShortInitialize;
			}
			set
			{
				_IsShortInitialize = value;
			}
		}
		bool IFieldElement.IsConst
		{
			get
			{
				return IsConst;
			}
		}
		IExpression IFieldElement.Expression
		{
			get
			{
				return Expression;
			}
		}
		bool IFieldElement.IsBitField
		{
			get
			{
				return IsBitField;
			}
		}
		IExpression IFieldElement.BitFieldSize
		{
			get
			{
				return BitFieldSize;
			}
		}
		#endregion
		#region IVariableDeclarationStatement Members
		bool IVariableDeclarationStatement.IsConst
		{
			get
			{
				return IsConst;
			}
		}
		IExpression IVariableDeclarationStatement.Expression
		{
			get
			{
				return Expression;
			}
		}
		bool IVariableDeclarationStatement.IsBitField
		{
			get
			{
				return IsBitField;
			}
		}
		IExpression IVariableDeclarationStatement.BitFieldSize
		{
			get
			{
				return BitFieldSize;
			}
		}		
		#endregion
	}
}
