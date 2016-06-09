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

using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Case : ConditionalParentingStatement, ICaseStatement
	{
		private const int INT_MaintainanceComplexity = 2;
	  bool _IsDefault = false;
		Expression _Expression;
		CaseClausesList _CaseClauses;
	protected void SetExpression(Expression expression)
	{
	  Expression oldExpression = _Expression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Expression = expression;
	  if (_Expression != null)
		AddDetailNode(_Expression);
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Expression == oldElement)
				_Expression = (Expression)newElement;
			else if (_CaseClauses == oldElement)
				_CaseClauses = (CaseClausesList)newElement;
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
			if (!(source is Case))
				return;
			Case lSource = (Case)source;
			_IsDefault = lSource._IsDefault;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
			if (lSource._CaseClauses != null)
			{				
				_CaseClauses = ParserUtils.GetCloneFromNodes(this, lSource, lSource._CaseClauses) as CaseClausesList;
				if (_CaseClauses == null)
					_CaseClauses = lSource._CaseClauses.Clone(options) as CaseClausesList;
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetEndPoint()
		{
			LanguageElement node = LastNode;
			if (node != null)
				SetEnd(node.Range.End);
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.CaseBlock;	
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_Expression = null;
			_CaseClauses = null;
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_Expression = null;
			_CaseClauses = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Case lClone = new Case();
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
				return LanguageElementType.Case;
			}
		}
		#region IsDefault
		public bool IsDefault
	{
		get
		{
			return _IsDefault;
		}
			set
			{
				_IsDefault = value;
			}
	}
		#endregion
		#region Expression
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
		#region CaseClauses
		public CaseClausesList CaseClauses
		{
			get
			{
				if (_CaseClauses == null)
					_CaseClauses = new CaseClausesList();
				return _CaseClauses;
			}
		}
		#endregion
		public void AddCaseClause(CaseClause caseClause)
		{
			if (caseClause == null)
				return;
			AddDetailNode(caseClause);
			if (_CaseClauses == null)
				_CaseClauses = new CaseClausesList();
			_CaseClauses.Clauses.Add(caseClause);
			_CaseClauses.AddDetailNode(caseClause);
			caseClause.SetParent(this);
		}
	public void AddCaseClauses(CaseClausesList clauses)
	{
	  if (clauses == null)
		return;
	  foreach (LanguageElement element in clauses.Clauses)
	  {
		CaseClause clause = element as CaseClause;
		if (clause == null)
		  continue;
		AddCaseClause(clause);
	  }
	}
		#region ICaseStatement Members
		IExpression ICaseStatement.Expression
		{
			get 
			{
		return Expression;
			}
		}
		ICaseClausesList ICaseStatement.CaseClauses
		{
			get 
			{
				return _CaseClauses;
			}
		}
		#endregion
	}
}
