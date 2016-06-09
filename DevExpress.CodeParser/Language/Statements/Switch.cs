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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class Switch: ParentingStatement, ISwitchStatement
	{
		const int INT_MaintainanceComplexity = 3;
		Expression _Expression;
	LiteCaseStatementCollection _CaseStatements;
		#region Switch
		public Switch()
		{
			IsBreakable = true;
	}
		#endregion
		void SetExpression(Expression expression) 
		{
			RemoveDetailNode(_Expression);
			_Expression = expression;
			if (_Expression != null)
				AddDetailNode(_Expression);
		}
	public void AddCaseStatement(LanguageElement element)
	{
	  Case caseStatement = element as Case;
	  if (caseStatement == null)
		return;
	  if (_CaseStatements == null)
		_CaseStatements = new LiteCaseStatementCollection();
	  _CaseStatements.Add(caseStatement);
	  AddNode(caseStatement);
	}
	public void AddCaseStatements(LanguageElementCollection elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddCaseStatement(element);
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Switch))
				return;
			Switch lSource = (Switch)source;
			if (lSource._Expression != null)
			{				
				_Expression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Expression) as Expression;
				if (_Expression == null)
					_Expression = lSource._Expression.Clone(options) as Expression;
			}
	  if (lSource._CaseStatements != null)
	  {
		_CaseStatements = new LiteCaseStatementCollection();
		ParserUtils.GetClonesFromNodes(Nodes, lSource.Nodes, _CaseStatements, lSource._CaseStatements);
		if (_CaseStatements.Count == 0 && lSource._CaseStatements.Count > 0)
		  _CaseStatements = lSource._CaseStatements.DeepClone(options) as LiteCaseStatementCollection;
	  }
		}
		#endregion
	protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (_Expression == oldElement)
		_Expression = newElement as Expression;
	  else if (_CaseStatements != null && _CaseStatements.Contains(oldElement) && newElement is Case)
		_CaseStatements.Replace(oldElement, newElement);
	  else base.ReplaceOwnedReference(oldElement, newElement);
	}
	public override void CleanUpOwnedReferences()
	{
	  if (_Expression != null)
	  {
		_Expression.CleanUpOwnedReferences();
		_Expression = null;
	  }
	  if (_CaseStatements != null)
	  {
		_CaseStatements.Clear();
		_CaseStatements = null;
	  }
	  base.CleanUpOwnedReferences();
	}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.SwitchStatement;	
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			Switch lClone = new Switch();
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
				return LanguageElementType.Switch;
			}
		}
		public override bool IsNewContext
		{
			get 
			{
				return true;
			}
		}
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
		#region ISwitchStatement Members
		IExpression ISwitchStatement.Expression
		{
			get 
			{
				return _Expression;
			}
		}
		ICaseStatementCollection ISwitchStatement.CaseStatements
		{
	  get
	  {
		if (NodeCount == 0)
		  return EmptyLiteElements.EmptyICaseStatementCollection;
		return _CaseStatements;
	  }
		}
		#endregion		
	}
}
