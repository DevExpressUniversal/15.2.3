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
	public class If : IfElse, IIfStatement
	{
		const int INT_MaintainanceComplexity = 3;
		LanguageElement _Condition = null;
		bool _HasElseStatement = false;
		public If()
		{
		}
		public If(Expression expression, LanguageElementCollection block)
			: this(expression, block, SourceRange.Empty)
		{
		}
		public If(Expression expression, LanguageElementCollection block, SourceRange range)
		{
			SetExpression(expression);
			AddNodes(block);
			SetRange(range);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is If))
				return;
			If lSource = (If)source;
			_HasElseStatement = lSource._HasElseStatement;
			if (lSource._Condition != null)
			{
				_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as LanguageElement;
				if (_Condition == null)
					_Condition = lSource._Condition.Clone(options) as LanguageElement;
			}
		}
		#endregion
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Condition == oldElement)
				_Condition = newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetExpression(Expression expression)
		{
			SetCondition(expression);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCondition(LanguageElement condition)
		{
			if (_Condition == null)
			{
				_Condition = condition;
				AddDetailNode(condition);
			}
			else
			{
				ReplaceDetailNode(_Condition, condition);
			}
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.IfBlock;
		}
		#endregion
		#region GetDetailNodeDescription
		public override string GetDetailNodeDescription(int index)
		{
			return "If-block expression";
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			If lClone = new If();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.If;
			}
		}
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#region HasElseStatement
		[Description("True if this if-block has a matching else statement.")]
		[Category("Details")]
		[DefaultValue(false)]
		public bool HasElseStatement
		{
			get
			{
				return ElseStatement != null;
			}
		}
		#endregion
		#region ElseStatement
		public Statement ElseStatement
		{
			get
			{
				LanguageElement lNextCodeSibling = NextCodeSibling;
				if (lNextCodeSibling is Else)
					return (Else)lNextCodeSibling;
				if (lNextCodeSibling is ElseIf)
					return (ElseIf)lNextCodeSibling;
				return null;
			}
		}
		#endregion
		#region AcceptsElse
		public override bool AcceptsElse
		{
			get
			{
				return !HasBlock || EndLine > 1;
			}
		}
		#endregion
		#region TrueStatementsBlockRange
		public SourceRange TrueStatementsBlockRange
		{
			get
			{
				return BlockRange;
			}
		}
		#endregion
		#region FalseStatementsBlockRange
		public SourceRange FalseStatementsBlockRange
		{
			get
			{
				DelimiterCapableBlock lElse = ElseStatement;
				if (lElse == null)
					return SourceRange.Empty;
				return lElse.BlockRange;
			}
		}
		#endregion
		#region TrueStatementsRange
		public SourceRange TrueStatementsRange
		{
			get
			{
				return BlockCodeRange;
			}
		}
		#endregion
		#region FalseStatementsRange
		public SourceRange FalseStatementsRange
		{
			get
			{
				DelimiterCapableBlock lElse = ElseStatement;
				if (lElse == null)
					return SourceRange.Empty;
				return lElse.BlockCodeRange;
			}
		}
		#endregion
		#region Condition
		public LanguageElement Condition
		{
			get
			{
				return _Condition;
			}
			set
			{
				SetCondition(value);
			}
		}
		#endregion
		#region Expression
		public Expression Expression
		{
			get
			{
				return _Condition as Expression;
			}
			set
			{
				SetExpression(value);
			}
		}
		#endregion
		#region IIfStatement Members
		IExpression IIfStatement.Condition
		{
			get 
			{
				return _Condition as IExpression;
			}
		}
		IStatement IIfStatement.ElseStatement
		{
			get
			{
				return ElseStatement;
			}
		}
		bool IIfStatement.HasElseStatement
		{
			get
			{
				return HasElseStatement;
			}
		}
		#endregion
	public override string ToString()
	{
	  string result = base.ToString();
	  #if DEBUG
	  if(this.NodeCount == 1)
		result += " <" + this.Nodes[0].ToString() + ">";
	  #endif
	  return result;
	}
	}
}
