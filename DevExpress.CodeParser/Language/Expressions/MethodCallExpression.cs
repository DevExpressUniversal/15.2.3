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
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class MethodCallExpression: Expression, IHasQualifier, IMethodCallExpression, IHasParens, IHasArguments, IWithArgumentsModifier 
	{
		private const int INT_MaintainanceComplexity = 3;
		#region private fields...
		Expression _Source;
		ExpressionCollection _Arguments;
		TextRange _NameRange;
		TextRangeWrapper _ParensRange;
		bool _IsBaseConstructorCall = false;
		#endregion
		#region MethodCallExpression
		protected MethodCallExpression()
		{			
		}
		#endregion
		#region MethodCallExpression(Expression source)
		public MethodCallExpression(Expression source)
		{
			SetSource(source);
	  string name = null;
			if (source != null && !(source is MethodCallExpression))
			{
				name = source.Name;
				_NameRange = source.NameRange;
			}
			else
				_NameRange = SourceRange.Empty;
			if (name != null)
				InternalName = GlobalStringStorage.Intern(name);
			else
				InternalName = String.Empty;
		}
		#endregion
		#region ComareArguments
		private bool ComareArguments(MethodCallExpression expression)
		{
			if (expression == null)
				return false;
			if (Arguments == null && expression.Arguments == null)
				return true;
			if (Arguments == null)
				return false;
			return Arguments.IsIdenticalTo(expression.Arguments);
		}
		#endregion
	void SetSource(Expression source)
	{
	  if (_Source != null)
		RemoveNode(_Source);
	  _Source = source;
	  if (_Source != null)
		AddNode(_Source);
	}
	void SetArguments(ExpressionCollection arguments)
	{
	  if (_Arguments != null)
		RemoveDetailNodes(_Arguments);
	  _Arguments = arguments;
	  if (_Arguments != null)
		AddDetailNodes(_Arguments);
	}
		#region ReplaceOwnedReference
		protected override void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
			if (_Source == oldElement)
				_Source = (Expression)newElement;
			else if (_Arguments != null)
				_Arguments.ReplaceExpression(oldElement as Expression, newElement as Expression);
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
			if (!(source is MethodCallExpression))
				return;			
			MethodCallExpression lSource = (MethodCallExpression)source;
			_NameRange = lSource.NameRange;
			_IsBaseConstructorCall = lSource.IsBaseConstructorCall;
			if (lSource._Source != null)
			{				
				_Source = ParserUtils.GetCloneFromNodes(this, lSource, lSource._Source) as Expression;
				if (_Source == null)
					_Source = lSource._Source.Clone(options) as Expression;
			}
			if (lSource._Arguments != null)
			{
				_Arguments = new ExpressionCollection();
				ParserUtils.GetClonesFromNodes(DetailNodes, lSource.DetailNodes, _Arguments, lSource._Arguments);
				if (_Arguments.Count == 0 && lSource._Arguments.Count > 0)
					_Arguments = lSource._Arguments.DeepClone(options) as ExpressionCollection;
			}
			if (lSource._ParensRange != null)
				_ParensRange = lSource.ParensRange;
		}
		#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _NameRange = NameRange;
	  _ParensRange = ParensRange;
	}
		#region ToString
		public override string ToString()
		{
			string lResult = String.Empty;
			if (_Source != null)
				lResult += _Source.ToString();
			lResult += "(";
			if (_Arguments != null)
				lResult += _Arguments.ToString();
			lResult += ")";
			return lResult;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.MethodCallExpression;
		}
		#endregion
		#region IsIdenticalTo
		public override bool IsIdenticalTo(Expression expression)
		{
			if (expression == null)
				return false;
			if (expression is MethodCallExpression)
			{
				MethodCallExpression lExpression = (MethodCallExpression)expression;
				if (Qualifier == null)
					return ComareArguments(lExpression);
				else
					return Qualifier.IsIdenticalTo(lExpression.Qualifier) && ComareArguments(lExpression);
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
		#region OwnedReferencesTransfered
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void OwnedReferencesTransfered()
		{
			_Source = null;
			if (_Arguments != null)
			{
				_Arguments.Clear();
				_Arguments = null;
			}
			base.OwnedReferencesTransfered();
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			if (_Arguments != null)
			{
				for (int i = 0; i < _Arguments.Count; i++)
				{
					_Arguments[i].CleanUpOwnedReferences();
				}
				_Arguments.Clear();
				_Arguments = null;
			}
			if (_Source != null)
			{
				_Source.CleanUpOwnedReferences();
				_Source = null;
			}
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			MethodCallExpression lClone = new MethodCallExpression();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(Token parenOpen, Token parenClose)
		{
			SetParensRange(new SourceRange(parenOpen.Range.Start, parenClose.Range.End));			
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetParensRange(SourceRange range)
		{
	  ClearHistory();
			if (range.IsEmpty)
				_ParensRange = null;
			else
				_ParensRange = range;
		}
	public void AddArgument(Expression arg)
	{
	  Arguments.Add(arg);
	  AddDetailNode(arg);
	}
	public void AddArguments(IEnumerable<Expression> arguments)
	{
	  foreach (Expression arg in arguments)
		AddArgument(arg);
	}
	public void RemoveArgument(Expression arg)
	{
	  Arguments.Remove(arg);
	  RemoveDetailNode(arg);
	}
	public void InsertArgument(int index, Expression arg)
	{
	  Arguments.Insert(index, arg);
	  InsertDetailNode(index, arg);
	}
		#region ThisMaintenanceComplexity
		protected override int ThisMaintenanceComplexity
		{
			get
			{
				return INT_MaintainanceComplexity;
			}
		}
		#endregion
		#region IsBaseConstructorCall
		[Description("Returns true if this method is call to base construtor.")]
		[Category("Details")]
		public bool IsBaseConstructorCall
		{
			get
			{
				return _IsBaseConstructorCall;
			}
			set
			{
				_IsBaseConstructorCall = value;
			}
		}
		#endregion
		#region NeedsInvertParens
		public override bool NeedsInvertParens
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region ElementType
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.MethodCallExpression;
			}
		}
		#endregion
		#region Qualifier
		public Expression Qualifier
		{
			get
			{
				return _Source;
			}
			set
			{
		SetSource(value);
			}
		}
		#endregion
		#region Source
		[Obsolete("Use Qualifier instead")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Expression Source
		{
			get
			{
				return Qualifier;
			}
			set
			{
				if (Qualifier == value)
					return;
				Qualifier = value;
			}
		}
		#endregion
		#region Arguments
		public ExpressionCollection Arguments
		{
			get
			{
				if (_Arguments == null)
					_Arguments = new ExpressionCollection();
				return _Arguments;
			}
			set
			{
		SetArguments(value);
			}
		}
		#endregion
		#region CanBeStatement
		public override bool CanBeStatement
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region NameRange
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
		}
		#endregion
		#region IsGeneric
		public bool IsGeneric
		{
			get
			{
				if (Qualifier is ReferenceExpressionBase)
					return((ReferenceExpressionBase)Qualifier).IsGeneric;
				return false;					
			}
		}
		#endregion
		#region TypeArguments
		public TypeReferenceExpressionCollection TypeArguments
		{
			get
			{
				if (Qualifier is ReferenceExpressionBase)
					return((ReferenceExpressionBase)Qualifier).TypeArguments;
				return null;				
			}			
		}
		#endregion
	#region IMethodCallExpression Members
	IExpressionCollection IMethodCallExpression.Arguments
	{
	  get
	  {
		if (Arguments == null)
		  return EmptyLiteElements.EmptyIExpressionCollection;
		return Arguments;
	  }
	}
	#endregion
	#region IWithArguments Members
	IExpressionCollection IWithArguments.Args
	{
	  get
	  {
		if (Arguments == null)
		  return EmptyLiteElements.EmptyIExpressionCollection;
		return Arguments;
	  }
	}
	#endregion
	#region IWithArgumentsModifier Members
	void IWithArgumentsModifier.AddArgument(IExpression argument)
	{
	  AddArgument(argument as Expression);
	}
	void IWithArgumentsModifier.InsertArgument(int index, IExpression argument)
	{
	  InsertArgument(index, argument as Expression);
	}
	void IWithArgumentsModifier.RemoveArgument(IExpression argument)
	{
	  RemoveArgument(argument as Expression);
	}
	#endregion
		#region IGenericExpression Members
		ITypeReferenceExpressionCollection IGenericExpression.TypeArguments
		{
			get
			{
				if (TypeArguments == null)
					return EmptyLiteElements.EmptyITypeReferenceExpressionCollection;
				return TypeArguments;
			}
		}
		int IGenericExpression.TypeArity
		{
			get
			{
				return 0;
			}
		}
		#endregion
		#region IWithSource Members
		IExpression IWithSource.Source
		{
			get
			{
				return Qualifier;
			}
		}
		#endregion
		#region IHasParens Members
		public SourceRange ParensRange
		{
			get
			{
				if (_ParensRange == null)
					return SourceRange.Empty;
				return _ParensRange;
			}
		}
		#endregion
	#region IHasArguments Members
		ExpressionCollection IHasArguments.Arguments
		{
			get
			{
				return Arguments;
			}
		}
	public int ArgumentsCount
	{
	  get
	  {
		if (_Arguments == null)
		  return 0;
		return _Arguments.Count;
	  }
	}
	#endregion
  }
}
