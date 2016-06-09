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
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class QueryableCollectionReferenceExpression : Expression, IQueryableCollectionReferenceExpression
  {
	Expression _Expression;
	TypeReferenceExpression _Type;
	ExpressionCollection _AdditionalExpressions;
	public QueryableCollectionReferenceExpression()
	{
	}
	public QueryableCollectionReferenceExpression(Expression expression, TypeReferenceExpression type)
	{
	  SetExpression(expression);
	  _Type = type;
	}
	MethodCallExpression GetMethodCall(Expression expression, string name, TypeReferenceExpressionCollection typeArguments)
	{
	  if (expression == null)
		return null;
	  Expression clone = expression.Clone() as Expression;
	  if (clone == null)
		return null;
	  MethodReferenceExpression methodReference = new MethodReferenceExpression(clone, name);
	  if (typeArguments != null)
		methodReference.AddTypeArguments(typeArguments);
	  MethodCallExpression methodCall = new MethodCallExpression(methodReference);
	  methodCall.SetParent(this);
	  return methodCall;
	}
	MethodCallExpression GetAsQueryableMethodCall(Expression expression)
	{
	  return GetMethodCall(expression, "AsQueryable", null);
	}
	MethodCallExpression GetAsEnumerableMethodCall(Expression expression)
	{
	  return GetMethodCall(expression, "AsEnumerable", null);
	}
	MethodCallExpression GetCastMethodCall(Expression expression, TypeReferenceExpression type)
	{
	  TypeReferenceExpressionCollection castTypeArguments = new TypeReferenceExpressionCollection();
	  castTypeArguments.Add(CreateSystemObjectTypeReference());
	  MethodCallExpression castMethodCall = GetMethodCall(expression, "Cast", castTypeArguments);
	  if (castMethodCall == null)
		return null;
	  TypeReferenceExpressionCollection selectTypeArguments = new TypeReferenceExpressionCollection();
	  selectTypeArguments.Add(CreateSystemObjectTypeReference());
	  selectTypeArguments.Add(type.Clone() as TypeReferenceExpression);
	  MethodCallExpression selectMethodCall = GetMethodCall(castMethodCall, "Select", selectTypeArguments);
	  if (selectMethodCall == null)
		return null;
	  LambdaFunctionExpression argument = new LambdaFunctionExpression();
	  argument.AddParameter(new LambdaImplicitlyTypedParam("t"));
	  argument.AddNode(new TypeCastExpression(type.Clone() as TypeReferenceExpression, new ElementReferenceExpression("t")));
	  selectMethodCall.AddArgument(argument);
	  return selectMethodCall;
	}
	TypeReferenceExpression CreateSystemObjectTypeReference()
	{
	  TypeReferenceExpression systemTypeReference = new TypeReferenceExpression("System");
	  return new TypeReferenceExpression("Object", SourceRange.Empty, systemTypeReference);
	}
	ExpressionCollection GetAdditionalExpressions()
	{
	  ExpressionCollection result = new ExpressionCollection();
	  Expression methodCall = GetAsQueryableMethodCall(_Expression);
	  if (methodCall != null)
		result.Add(methodCall);
	  methodCall = GetAsEnumerableMethodCall(_Expression);
	  if (methodCall != null)
		result.Add(methodCall);
	  TypeReferenceExpression type = _Type;
	  if (type == null)
		type = CreateSystemObjectTypeReference();
	  methodCall = GetCastMethodCall(_Expression, type);
	  if (methodCall != null)
		result.Add(methodCall);
	  return result;
	}
	protected void SetExpression(Expression expression)
	{
	  if (expression == null)
		return;
	  _Expression = expression;
	  AddNode(expression);
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  QueryableCollectionReferenceExpression clone = new QueryableCollectionReferenceExpression();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  QueryableCollectionReferenceExpression lSource = source as QueryableCollectionReferenceExpression;
	  if (lSource == null)
		return;
	  if (lSource._Expression != null)
		SetExpression(lSource._Expression.Clone() as Expression);
	  if (lSource._Type != null)
		_Type = lSource._Type.Clone() as TypeReferenceExpression;
	}
	#endregion
	public override string ToString()
	{
	  return _Expression == null ? string.Empty : _Expression.ToString();
	}
	public override IElement Resolve(ISourceTreeResolver resolver)
	{
	  if (_Expression == null)
		return null;
	  return _Expression.Resolve(resolver);
	}
	public Expression Expression
	{
	  get
	  {
		return _Expression;
	  }
	  set
	  {
		_Expression = value;
	  }
	}
	public ExpressionCollection AdditionalExpressions
	{
	  get
	  {
		if (_AdditionalExpressions == null)
		  _AdditionalExpressions = GetAdditionalExpressions();
		return _AdditionalExpressions;
	  }
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.QueryableCollectionReferenceExpression;
	  }
	}
	IExpression IQueryableCollectionReferenceExpression.Expression
	{
	  get
	  {
		return Expression;
	  }
	}
	IExpressionCollection IQueryableCollectionReferenceExpression.AdditionalExpressions
	{
	  get
	  {
		return AdditionalExpressions;
	  }
	}
  }
}
