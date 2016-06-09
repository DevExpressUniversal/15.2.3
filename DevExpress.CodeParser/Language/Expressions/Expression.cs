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
	public abstract class Expression : CodeElement, IExpression
	{
		static object _NullValue = new byte();
		object _TestValue = _NullValue;
		bool _IsStatement;
		string _MacroCall;
		#region Expression
		public Expression()
		{
		}
		#endregion
		#region TransferNodesTo
		private void TransferNodesTo(NodeList nodes, LanguageElement element, bool toDetail)
		{
			if (element == null)
				return;
			if (nodes == null)
				return;
			for (int i = 0; i < nodes.Count; i++)
			{
				LanguageElement lElement = nodes[i] as LanguageElement;
				if (lElement == null)
					continue;
				if (toDetail)
					element.AddDetailNode(lElement);
				else
					element.AddNode(lElement);
			}
		}
		#endregion
		#region ClearTestValues
		private void ClearTestValues(NodeList nodes)
		{
			if (nodes == null)
				return;
			for (int i = 0; i < nodes.Count; i++)
			{
				Expression lExpression = nodes[i] as Expression;
				if (lExpression != null)
					lExpression.ClearTestValue();
			}
		}
		#endregion
		bool ConvertToBool(object value)
		{
			if (value is string || value is DateTime)
				throw new InvalidOperationException("Can't convert expression to boolean type.");
			return Convert.ToBoolean(value);
		}
		#region EvaluateExpression
		protected virtual object EvaluateExpression()
		{
			return null;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.Expression;
		}
		#endregion
		#region GetByLevel(int level)
		public Expression GetByLevel(int level)
		{			
			return QualifiedExpressionHelper.GetByLevel(this, level) as Expression;
		}
		#endregion
		#region GetStartElement
		public LanguageElement GetStartElement()
		{			
			return QualifiedExpressionHelper.GetStartElement(this) as LanguageElement;
		}
		#endregion
		#region GetEndElement
		public LanguageElement GetEndElement()
		{			
			return QualifiedExpressionHelper.GetEndElement(this);
		}
		#endregion
		#region IsIdenticalTo
		public virtual bool IsIdenticalTo(Expression expression)
		{
			return false;
		}
		#endregion
		#region Invert
		public Expression Invert()
		{
	  return StructuralParserServicesHolder.Invert(this);
		}
		#endregion
		#region Simplify
		public virtual Expression Simplify()
		{
	  return StructuralParserServicesHolder.Simplify(this);
		}
		#endregion
	public virtual Expression Simplify(bool considerMethodCalls)
	{
	  return StructuralParserServicesHolder.Simplify(this, considerMethodCalls);
	}
		public abstract IElement Resolve(ISourceTreeResolver resolver);
		public Expression RemoveRedundantParens()
		{
			if (!(this is ParenthesizedExpression))
				return this;
			ParenthesizedExpression lParens = (ParenthesizedExpression)this;
			Expression lParenExpression = lParens.Expression;
			if (!lParenExpression.NeedsInvertParens)
				return lParenExpression;
			return lParens;
		}
		#region Evaluate
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object Evaluate()
		{
			if (HasTestValue)
				return TestValue;
			return EvaluateExpression();
		}
		#endregion
		#region EvaluateAsBool
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool EvaluateAsBool()
		{
			object value = Evaluate();
			if (value == null || !(value is bool))
				return ConvertToBool(value);
			return (bool)value;
		}
		#endregion
		#region TransferAllNodesTo
		public virtual void TransferAllNodesTo(LanguageElement element)
		{
			if (element == null)
				return;
			TransferNodesTo(DetailNodes, element, true);
			TransferNodesTo(Nodes, element, false);
		}
		#endregion
		#region SetTestValue
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetTestValue(object value)
		{
			_TestValue = value;
		}
		#endregion
		#region ClearTestValue
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearTestValue()
		{
			_TestValue = _NullValue;
		}
		#endregion
		#region ClearTestValues
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearTestValues()
		{
			ClearTestValue();
			ClearTestValues(DetailNodes);
			ClearTestValues(Nodes);
		}
		#endregion
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is Expression))
				return;						
			Expression lSource = (Expression)source;
			_TestValue = lSource._TestValue;
			_IsStatement = lSource._IsStatement;			
		}
		public virtual bool Is(string fullTypeName)
		{
	  ISourceTreeResolver resolver = StructuralParserServicesHolder.SourceTreeResolver;
			return Is(resolver, fullTypeName);
		}
		public virtual bool Is(ITypeElement type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public virtual bool Is(Type type)
		{
			if (type == null)
				return false;
			return Is(type.FullName);
		}
		public virtual bool Is(ISourceTreeResolver resolver, string fullTypeName)
		{
			if (resolver == null)
				return false;
			ITypeElement typeElement = resolver.ResolveExpression(this) as ITypeElement;
			if (typeElement == null)
				return false;
			return typeElement.Is(resolver, fullTypeName);
		}
		#region NeedsInvertParens
		public virtual bool NeedsInvertParens
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region TestValue
		public object TestValue
		{
			get
			{
				return HasTestValue ? _TestValue : null;
			}
		}
		#endregion
		#region TestValueAsBool
		public bool TestValueAsBool
		{
			get
			{
				if (TestValue == null)
					return false;
				if (!(TestValue is bool))
					return false;
				return (bool)TestValue;
			}
		}
		#endregion
		#region HasTestValue
		public bool HasTestValue
		{
			get
			{
				return !Object.ReferenceEquals(_TestValue, _NullValue);
			}
		}
		#endregion
		#region Level
		public virtual int Level
		{
			get
			{
				if (this is IHasQualifier)
					return QualifiedExpressionHelper.GetLevel(this as IHasQualifier);				
				return 1;
			}
		}
		#endregion
		#region CanBeStatement
		public virtual bool CanBeStatement
		{
			get
			{
				return false;
			}
		}
		#endregion
		public bool IsStatement
		{
			get
			{
				return _IsStatement;
			}
			set
			{
				_IsStatement = value;
			}
		}
		public bool IsIndirectlyModified
		{
			get
			{
				Expression parent = Parent as Expression;
				if (parent == null)
					return false;
				IndexerExpression indexer = parent as IndexerExpression;
				if (indexer != null)
				{
					if (indexer.Arguments != null && indexer.Arguments.Contains(this))
						return false;
				}
				MethodCallExpression methodCall = parent as MethodCallExpression;
				if (methodCall != null)
				{
					if (methodCall.Arguments != null && methodCall.Arguments.Contains(this))
						return false;
				}
				if (parent.IsInsideModificationExpression)
					return true;
				ElementReferenceExpression elementRef = parent as ElementReferenceExpression;
				if (elementRef != null && elementRef.IsModified)
					return true;
				if (parent.IsIndirectlyModified)
					return true;
				return false;
			}
		}
		public bool IsInsideModificationExpression
		{
			get
			{
				return InsideAssignment ||
					InsideOutArgumentDirection ||
					InsideRefArgumentDirection ||
					InsideIncrement ||
					InsideDecrement;
			}
		}
		#region ExpressionTypeName
		public virtual string ExpressionTypeName
		{
			get
			{
				return String.Empty;
			}
		}
		#endregion
		public override LanguageElementType ElementType
		{
			get
			{
				return LanguageElementType.Expression;
			}
		}
		public override string MacroCall
		{
			get
			{
				return _MacroCall;
			}
			set
			{
				_MacroCall = value;
			}
		}
	}
}
