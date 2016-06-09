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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class For: ConditionalParentToSingleStatement, IForStatement
	{
		private const int INT_MaintainanceComplexity = 3;
	  Expression _Condition = null;
	LanguageElementList _Initializers = new LanguageElementList();
	LanguageElementList _Incrementors = new LanguageElementList();
		Expression _ToExpression;
		Expression _StepExpression;
		public For()
		{
			IsBreakable = true;
	}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is For))
				return;
			For lSource = (For)source;
			if (lSource._Condition != null)
			{				
				_Condition = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Condition) as Expression;
				if (_Condition == null)
					_Condition = lSource._Condition.Clone(options) as Expression;
			}
			if (lSource._Initializers != null)
			{
				_Initializers = new LanguageElementList();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Initializers, lSource._Initializers);
				if (_Initializers.Count == 0 && lSource._Initializers.Count > 0)
					_Initializers = lSource._Initializers.DeepClone(options) as LanguageElementList;
			}
			if (lSource._Incrementors != null)
			{
				_Incrementors = new LanguageElementList();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Incrementors, lSource._Incrementors);
				if (_Incrementors.Count == 0 && lSource._Incrementors.Count > 0)
					_Incrementors = lSource._Incrementors.DeepClone(options) as LanguageElementList;
			}
	  if (lSource._ToExpression != null)
	  {
		_ToExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._ToExpression) as Expression;
		if (_ToExpression == null)
		  _ToExpression = lSource._ToExpression.Clone(options) as Expression;
	  }
	  if (lSource._StepExpression != null)
	  {
		_StepExpression = ParserUtils.GetCloneFromNodes(this, lSource, lSource._StepExpression) as Expression;
		if (_StepExpression == null)
		  _StepExpression = lSource._StepExpression.Clone(options) as Expression;
	  }
		}
		#endregion
		public void AddInitializer(LanguageElement element)
		{
	  if(element == null)
		return;
			Initializers.Add(element);
			AddDetailNode(element);
		}
	public void AddInitializers(LanguageElementCollection elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddInitializer(element);
	}
		public void AddIncrementor(LanguageElement element)
		{
	  if (element == null)
		return;
			Incrementors.Add(element);
			AddDetailNode(element);
		}
	public void AddIncrementors(LanguageElementCollection elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddIncrementor(element);
	}
	public virtual void AddNextExpression(LanguageElement element)
	{
	}
	public void AddNextExpressions(LanguageElementCollection elements)
	{
	  if (elements == null)
		return;
	  foreach (LanguageElement element in elements)
		AddNextExpression(element);
	}
	protected void SetCondition(Expression expression)
	{
	  Expression oldExpression = _Condition;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _Condition = expression;
	  if (_Condition != null)
		AddDetailNode(_Condition);
	}
	protected void SetToExpression(Expression expression)
	{
	  Expression oldExpression = _ToExpression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _ToExpression = expression;
	  if (_ToExpression != null)
		AddDetailNode(_ToExpression);
	}
	protected void SetStepExpression(Expression expression)
	{
	  Expression oldExpression = _StepExpression;
	  if (oldExpression != null)
		oldExpression.RemoveFromParent();
	  _StepExpression = expression;
	  if (_StepExpression != null)
		AddDetailNode(_StepExpression);
	} 
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.ForLoop;
		}
		#endregion
		public override void CleanUpOwnedReferences()
		{
			if (_Condition != null)
			{
				_Condition.CleanUpOwnedReferences();
				_Condition = null;
			}
	  if (_ToExpression != null)
	  {
		_ToExpression.CleanUpOwnedReferences();
		_ToExpression = null;
	  }
	  if (_StepExpression != null)
	  {
		_StepExpression.CleanUpOwnedReferences();
		_StepExpression = null;
	  }
			if (_Initializers != null)
			{
				_Initializers.Clear();
				_Initializers = null;
			}
			if (_Incrementors != null)
			{
				_Incrementors.Clear();
				_Incrementors = null;
			}
			base.CleanUpOwnedReferences();
		}
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Condition == oldElement)
				_Condition = newElement as Expression;
			else if (_Initializers != null && _Initializers.Contains(oldElement))
				_Initializers.Replace(oldElement, newElement);
			else if (_Incrementors != null && _Incrementors.Contains(oldElement))
				_Incrementors.Replace(oldElement, newElement);
	  else if (_ToExpression == oldElement)
		_ToExpression = newElement as Expression;
	  else if (_StepExpression == oldElement)
		_StepExpression = newElement as Expression;
			else
				base.ReplaceOwnedReference(oldElement, newElement);
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			For lClone = new For();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.For;
			}
		}
		#region Initializers
		public LanguageElementList Initializers
		{
			get
			{
				return _Initializers;
			}
		}
		#endregion
		#region Incrementors
		public LanguageElementList Incrementors
		{
			get
			{
				return _Incrementors;
			}
		}
		#endregion
		#region Condition
		public Expression Condition
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
		#region ToExpression
		public Expression ToExpression
		{
			get
			{
				return _ToExpression;
			}
			set
			{
				SetToExpression(value);
			}
		}
		#endregion
		#region StepExpression
		public Expression StepExpression
		{
			get
			{
				return _StepExpression;
			}
			set
			{
		SetStepExpression(value);
			}
		}
		#endregion
		#region NextExpression
		public virtual NodeList NextExpressionList
		{
			get
			{
				return null;
			}
			set
			{
				;
			}
		}
		#endregion
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		IExpression IForStatement.Condition
		{
			get 
			{ 
				return Condition; 
			}
		}
		IElementCollection IForStatement.Initializers
		{
			get 
			{ 
				IElementCollection initializers = new IElementCollection();
		if (Initializers != null)
		{
		  int count = Initializers.Count;
		  for (int i = 0; i < count; i++)
		  {
			LanguageElement current = Initializers[i] as LanguageElement;
			if (current == null)
			  continue;
			initializers.Add(current);
		  }
		}
				return initializers;
			}
		}
		IExpressionCollection IForStatement.Incrementors
		{
			get 
			{ 
				LiteExpressionCollection incrementors = new LiteExpressionCollection();
				int count = Incrementors.Count;
				for (int i = 0; i < count; i++)
				{
					Expression current = Incrementors[i] as Expression;
					if (current == null)
						continue;
					incrementors.Add(current);
				}
				return incrementors;
			}
		}
		IExpression IForStatement.ToExpression
		{
			get 
			{
				return ToExpression;
			}
		}
		IExpression IForStatement.StepExpression
		{
			get 
			{
				return StepExpression;
			}
		}
		IExpressionCollection IForStatement.NextExpressions
		{
			get 
			{
				LiteExpressionCollection nextExpressions = new LiteExpressionCollection();
				int count = NextExpressionList.Count;
				for (int i = 0; i < count; i++)
				{
					Expression current = NextExpressionList[i] as Expression;
					if (current == null)
						continue;
					nextExpressions.Add(current);
				}
				return nextExpressions;
			}
		}
	}
}
