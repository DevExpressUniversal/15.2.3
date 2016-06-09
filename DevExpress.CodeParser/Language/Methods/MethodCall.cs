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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class MethodCall: Statement, IHasQualifier, IMethodCallStatement, IHasParens, IHasArguments, IWithArgumentsModifier
	{
		Expression _Source;
		ExpressionCollection _Arguments;
		bool _IsBaseConstructorCall;	
		TextRangeWrapper _ParensRange;
		TextRange _NameRange;
	  public MethodCall()
	  {
	  } 
	public MethodCall(Expression source)
	{
	  SetQualifier(source);
	}
	protected void SetQualifier(Expression expression)
	{
	  Expression oldQualifier = _Source;
	  if (oldQualifier != null)
		oldQualifier.RemoveFromParent();
	  _Source = expression;
	  if (_Source != null)
		AddNode(_Source);
	}
		public override string ToString()
		{
			StringBuilder lResult = new StringBuilder();
			if (_Source != null)
				lResult.Append(_Source.ToString());
			else
				lResult.Append(Name);
			lResult.Append("(");
			string lComma = String.Empty;
			for (int i = 0; i < Arguments.Count; i++)
			{
				Expression lExpression = Arguments[i] as Expression;
				if (lExpression != null)
	  		lResult.AppendFormat("{0}{1}", lComma, lExpression.ToString());
				lComma = ", ";
			}
			lResult.Append(")");
			return lResult.ToString();
		}
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.MethodCall;
		}
		#endregion
		#region GetDetailNodeDescription
		public override string GetDetailNodeDescription(int index)
		{
			return "Argument";
		}
		#endregion
		#region FromMethodCallExpression
		public static MethodCall FromMethodCallExpression(MethodCallExpression expression)
		{
			if (expression == null)
				return null;
			MethodCall methodCall = new MethodCall();
	  methodCall.SourceExpression = expression;
			MethodReferenceExpression methodRefExpression = expression.Qualifier as MethodReferenceExpression;
			if (methodRefExpression != null)
			{
				methodCall.InternalName = expression.Name;
				methodCall._NameRange = expression.NameRange.Clone();
			}
			else
			{
				methodCall.InternalName = String.Empty;
				methodCall._NameRange = TextRange.Empty;
			}
			methodCall._IsBaseConstructorCall = expression.IsBaseConstructorCall;
			expression.TransferCommentsTo(methodCall);
			if (expression.Qualifier != null)
				methodCall._Source = expression.Qualifier;
			methodCall.SetParensRange(expression.ParensRange);
			methodCall.SetRange(expression.Range);
			expression.TransferAllNodesTo(methodCall);
			for (int i = 0; i < expression.Arguments.Count; i++)
				methodCall.Arguments.Add(expression.Arguments[i]);
			methodCall.MacroCall = expression.MacroCall;
			return methodCall;
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			if (_Arguments != null)
			{
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
		#region SetArguments
		protected void SetArguments(ExpressionCollection arguments)
		{
			_Arguments = arguments;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is MethodCall))
				return;
			MethodCall lSource = (MethodCall)source;
			_IsBaseConstructorCall = lSource._IsBaseConstructorCall;
			_NameRange = lSource.NameRange;
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
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			MethodCall lClone = new MethodCall();
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
		public override LanguageElementType ElementType
		{
			get 
			{
				return LanguageElementType.MethodCall;
			}
		}
		#region Qualifier
		public Expression Qualifier
		{
			get
			{
				return _Source;
			}
			set
			{
				if (_Source == value)
					return;
		SetQualifier(value);
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
		public override SourceRange NameRange
		{
			get
			{
				return GetTransformedRange(_NameRange);
			}
		}
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
				if (_Arguments == value)
					return;
				_Arguments = value;
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
		public bool IsGeneric
		{
			get
			{
				if (Qualifier is ReferenceExpressionBase)
					return((ReferenceExpressionBase)Qualifier).IsGeneric;
				return false;					
			}
		}
		public TypeReferenceExpressionCollection TypeArguments
		{
			get
			{
				if (Qualifier is ReferenceExpressionBase)
					return((ReferenceExpressionBase)Qualifier).TypeArguments;
				return null;				
			}
	}
	#region IMethodCallStatement Members
	IExpressionCollection IMethodCallStatement.Arguments
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
				return GetTransformedRange(_ParensRange);
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
	public void AddArgument(Expression arg)
	{
	  Arguments.Add(arg);
	  AddDetailNode(arg);
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
