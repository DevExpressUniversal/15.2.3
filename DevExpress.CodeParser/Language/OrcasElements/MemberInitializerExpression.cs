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
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class MemberInitializerExpression : Expression, IMemberInitializerExpression
	{
		Expression _Value;
		SourceRange _NameRange = SourceRange.Empty;
		bool _IsKey = false;
	public MemberInitializerExpression()
	{
	}
	public MemberInitializerExpression(string name, Expression value)
	{
	  Name = name;
	  SetValue(value);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is MemberInitializerExpression))
				return;
			MemberInitializerExpression lSource = (MemberInitializerExpression)source;
			if (lSource._Value != null)
			{
				_Value = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Value) as Expression;
				if (_Value == null)
					_Value = lSource._Value.Clone(options) as Expression;
			}	
			_IsKey = lSource.IsKey;
			_NameRange = lSource.NameRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	}
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Value == oldElement)
		_Value = (Expression)newElement;								
	  else
		base.ReplaceOwnedReference(oldElement, newElement);
	}
		public override IElement Resolve(ISourceTreeResolver resolver)
		{
			if (resolver == null)
				return null;
			return resolver.Resolve(this);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			MemberInitializerExpression lClone = new MemberInitializerExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			MemberInitializerExpression memberInit = expression as MemberInitializerExpression;
			if (memberInit != null)
			{
				if (Name != memberInit.Name)
					return false;
				if (Value == null && memberInit.Value == null)
					return true;
				return Value.IsIdenticalTo(memberInit.Value);
			}
			return false;
		}
		#endregion
	public void SetValue(Expression expression)
	{
	  if (_Value != null)
		RemoveDetailNode(_Value);
	  _Value = expression;
	  if (_Value != null)
		AddDetailNode(_Value);
	}
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
			set
			{
		ClearHistory();
				_NameRange = value;
			}
		}
		public Expression Value
		{
			get
			{
				return _Value;
			}
			set
			{
		SetValue(value);
			}
		}
		public bool IsKey
		{
			get
			{
				return _IsKey;
			}
			set
			{
				_IsKey = value;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.MemberInitializerExpression;
			}
		}		
		IExpression IMemberInitializerExpression.Value
		{
			get
			{
				return _Value;
			}
		}
	}
}
