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
	public enum DoConditionType
	{
		While,
		Until
	}
	public class Do : ConditionalParentToSingleStatement, IDoStatement
	{
	  Expression _Expression;
		DoConditionType _ConditionType;
		bool _PreCondition;
		public Do()
		{
			IsBreakable = true;
			_PreCondition = true;
			_ConditionType = DoConditionType.While;
		}
	public Do(Expression condition)
	  : this()
	{
	  SetCondition(condition); 
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			Do lSource = source as Do;
			if (lSource == null)
				return;
			if (lSource._Expression != null)
			{
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
			_ConditionType = lSource._ConditionType;
			_PreCondition = lSource._PreCondition;
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region SetCondition
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCondition(Expression expression)
		{
			if (expression != null)
				ReplaceDetailNode(_Expression, expression);
			_Expression = expression;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.DoWhile;	
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Do lClone = new Do();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.Do;
			}
		} 
		public bool PreCondition
		{
			get
			{
				return _PreCondition;
			}
			set
			{
				_PreCondition = value;
			}
		}
		public DoConditionType ConditionType
		{
			get
			{
				return _ConditionType;
			}
			set
			{
				_ConditionType = value;
			}
		}
		#region Condition
		public Expression Condition
		{
		  get
		  {
			return _Expression;
		  }
		}
		#endregion
		IExpression IDoStatement.Condition
		{
			get
			{
				return Condition;
			}
		}
	}
}
