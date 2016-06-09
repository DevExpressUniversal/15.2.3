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

using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class CaseClause : LanguageElement, ICaseClauseStatement
	{
		private const int INT_MaintainanceComplexity = 1;
		Expression _StartExpression;
		Expression _EndExpression;
		string _ComparisonOperator;
		bool _IsComparison;
		bool _IsFromToClause;
		public CaseClause()
		{
		}
	public CaseClause(Expression startExpression, Expression endExpression)
	  : this()
	{
	  SetStartExpression(startExpression);
	  SetEndExpression(endExpression);
	}
	public IEnumerable<IElement> GetBlockChildren()
	{
	  foreach (object nodeObj in Nodes)
	  {
		IElement node = nodeObj as IElement;
		if (node != null)
		  yield return node;
	  }
	}
	protected void SetStartExpression(Expression expression)
	{
	  Expression oldExpression = _StartExpression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _StartExpression = expression;
	  if (_StartExpression != null)
		AddDetailNode(_StartExpression);
	}
	protected void SetEndExpression(Expression expression)
	{
	  Expression oldExpression = _EndExpression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _EndExpression = expression;
	  if (_EndExpression != null)
		AddDetailNode(_EndExpression);
	}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_StartExpression == oldElement)
				_StartExpression = (Expression)newElement;
			else if (_EndExpression == oldElement)
				_EndExpression = (Expression)newElement;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is CaseClause))
				return;
			CaseClause lSource = (CaseClause)source;
			_ComparisonOperator = lSource._ComparisonOperator;
			_IsComparison = lSource._IsComparison;
			_IsFromToClause = lSource._IsFromToClause;
			if (lSource._StartExpression != null)
			{				
				_StartExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._StartExpression) as Expression;
				if (_StartExpression == null)
					_StartExpression = lSource._StartExpression.Clone(options) as Expression;
			}
			if (lSource._EndExpression != null)
			{				
				_EndExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._EndExpression) as Expression;
				if (_EndExpression == null)
					_EndExpression = lSource._EndExpression.Clone(options) as Expression;
			}			
		}
		#endregion
		#region GetImageIndex()
		public override int GetImageIndex()
		{
			return ImageIndex.SwitchStatement;
		}
		#endregion
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_StartExpression = null;
			_EndExpression = null;
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void CleanUpOwnedReferences()
		{
			_StartExpression = null;
			_EndExpression = null;
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			if (IsRangeCheckClause)
			{
				if (_StartExpression != null)
					lResult.Append(_StartExpression.ToString());
				lResult.Append(" To ");
				if (_EndExpression != null)
					lResult.Append(_EndExpression.ToString());
			}
			else
			{
				lResult.Append(_ComparisonOperator);
				lResult.Append(" ");
				if (_StartExpression != null)
					lResult.Append(_StartExpression.ToString());
				if (IsComparisonClause)
					lResult.Insert(0, " Is ");
			}
			return lResult.ToString();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			CaseClause lClone = new CaseClause();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#endregion
		#region StartExpression
		public Expression StartExpression
		{
			get
			{
				return _StartExpression;
			}
			set
			{
				SetStartExpression(value);
			}
		}
		#endregion
		#region EndExpression
		public Expression EndExpression
		{
			get
			{
				return _EndExpression;
			}
			set
			{
		SetEndExpression(value);
			}
		}
		#endregion
		#region IsComparisonClause
		public bool IsComparisonClause
		{
			get
			{
				return _IsComparison;
			}
			set
			{
				_IsComparison = value;
			}
		}
		#endregion
		#region IsRangeCheckClause
		public bool IsRangeCheckClause
		{
			get
			{
				return _IsFromToClause;
			}
			set
			{
				_IsFromToClause = value;
			}
		}
		#endregion
		#region ComparisonOperator
		public string ComparisonOperator
		{
			get
			{
				return _ComparisonOperator;
			}
			set
			{
				_ComparisonOperator = value;
			}
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.CaseClause;
			}
		}
		#endregion
		#region ICaseClauseStatement Members
		IExpression ICaseClauseStatement.StartExpression
		{
			get
			{
				return _StartExpression;
			}
		}
		IExpression ICaseClauseStatement.EndExpression
		{
			get 
			{
				return _EndExpression;
			}
		}
		#endregion
	#region IStatement Members
	public bool HasDelimitedBlock
	{
	  get { return false; }
	}
	#endregion
  }
}
