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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class CodeGen : CodeGenObject
  {
	LanguageElement _FirstElement = null;
	CodeGenOptions _Options;
	DirectiveCodeGenBase _DirectiveGen;
	ExpressionCodeGenBase _ExpressionGen;
	MemberCodeGenBase _MemberGen;
	StatementCodeGenBase _StatementGen;
	SupportElementCodeGenBase _SupportElementGen;
	TypeDeclarationCodeGenBase _TypeDeclarationGen;
	XmlCodeGenBase _XmlGen;
	NamespaceReferenceGenBase _NamespaceReferenceGen;
	NamespaceGenBase _NamespaceGen;
	SnippetCodeGenBase _SnippetGen;
	TemplateCodeGenBase _TemplateGen;
	TemplateParameterCodeGenBase _TemplateParameterGen;
	SourceFileCodeGenBase _SourceFileGen;
	HtmlXmlCodeGenBase _HtmlXmlCodeGen;
	GeneratingElements _Contexts = new GeneratingElements();
	FormattingTokenGen _TokenGen;
	bool _GenCommentsFromToken = false;
	List<LanguageElement> _GeneratedElements;
	public CodeGen()
	  : this(CodeGenOptions.Default)
	{
	}
	public CodeGen(CodeGenOptions options)
	{
	  if (options == null)
		throw new ArgumentNullException("options");
	  _Options = options;
	  InitializeCodeGenObjects();
	}
	bool PrevContextHasAttributeSection(AttributeSection section)
	{
	  if (section == null)
		return false;
	  CodeElement prevContext = GetPrevContext() as CodeElement;
	  if (prevContext == null)
		return false;
	  return prevContext.AttributeSections.Contains(section);
	}
	bool IsSecondaryAncestorTypeReference(LanguageElement context, TypeDeclaration type)
	{
	  if (type.SecondaryAncestorTypes != null)
		if (type.SecondaryAncestorTypes.Contains(context))
		  return true;
	  return false;
	}
	bool IsAncestorTypeReference(LanguageElement context, TypeDeclaration type)
	{
	  if (context == null || type == null)
		return false;
	  if (type.PrimaryAncestorType == context)
		return true;
	  return IsSecondaryAncestorTypeReference(context, type);
	}
	bool IsHandlesExpression(LanguageElement context, Member parent)
	{
	  if (context == null || parent == null || parent.HandlesExpressions == null)
		return false;
	  Expression expression = context as Expression;
	  if (expression == null)
		return false;
	  return parent.HandlesExpressions.Contains(expression);
	}
	bool IsImplementsExpression(LanguageElement context, Member member)
	{
	  if (context == null || member == null || member.ImplementsExpressions == null)
		return false;
	  Expression expression = context as Expression;
	  if (expression == null)
		return false;
	  return member.ImplementsExpressions.Contains(expression);
	}
	bool IsFirst(Variable var)
	{
	  if (var == null)
		return false;
	  bool hasNext = var.NextVariable != null;
	  bool hasPrevious = var.PreviousVariable != null;
	  return !hasPrevious && hasNext && !var.IsDetailNode;
	}
	bool IsLast(Variable var)
	{
	  if (var == null)
		return false;
	  bool hasNext = var.NextVariable != null;
	  bool hasPrevious = var.PreviousVariable != null;
	  return hasPrevious && !hasNext && !var.IsDetailNode;
	}
	bool IsNotFirst(Variable var)
	{
	  if (var == null)
		return false;
	  bool hasPrevious = var.PreviousVariable != null;
	  return hasPrevious && !var.IsDetailNode;
	}
	string GetTokenText(FormattingTokenType type, LanguageElement fromElement)
	{
	  if (type == FormattingTokenType.SourceFileStart)
		return String.Empty;
	  if (type == FormattingTokenType.Ident && fromElement != null)
		return fromElement.Name;
	  return FormattingTable.Get(type);
	}
	protected Expression[] GetBinaryList(BinaryOperatorExpression oper)
	{
	  List<Expression> result = new List<Expression>();
	  if (oper == null)
		return result.ToArray();
	  if (oper.LeftSide != null)
		if (oper.LeftSide is BinaryOperatorExpression)
		  result.AddRange(GetBinaryList(oper.LeftSide as BinaryOperatorExpression));
		else
		  result.Add(oper.LeftSide);
	  if (oper.RightSide != null)
		if (oper.RightSide is BinaryOperatorExpression)
		  result.AddRange(GetBinaryList(oper.RightSide as BinaryOperatorExpression));
		else
		  result.Add(oper.RightSide);
	  return result.ToArray();
	}
	protected virtual bool NeedIndenting(LanguageElement element, bool pushed)
	{
	  if (element == null)
		return false;
	  LanguageElement parent = element.Parent;
	  if (parent == null)
		return false;
	  switch (parent.ElementType)
	  {
		case LanguageElementType.LogicalOperation:
		  return Options.WrappingAlignment.WrapLogicalOperatorExpression || Options.WrappingAlignment.WrapBeforeOperatorInLogicalExpression;
		case LanguageElementType.BinaryOperatorExpression:
		  return Options.WrappingAlignment.WrapBinaryOperatorExpression || Options.WrappingAlignment.WrapBeforeOperatorInBinaryExpression;
		case LanguageElementType.GenericModifier:
		  if (!(element is TypeParameter))
			return false;
		  if (pushed)
		  {
			if (ElementWasGenerated(element))
			{
			  if (!Options.WrappingAlignment.WrapTypeParameterConstraints && !Options.WrappingAlignment.WrapFirstTypeParameterConstraint
			  || Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint)
				return false;
			  return !Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint;
			}
			if (!Options.WrappingAlignment.WrapTypeParameters && !Options.WrappingAlignment.WrapFirstTypeParameter
			  || Options.WrappingAlignment.AlignWithFirstTypeParameter)
			  return false;
			return !Options.WrappingAlignment.AlignWithFirstTypeParameter;
		  }
		  if (!ElementWasGenerated(element))
		  {
			if (!Options.WrappingAlignment.WrapTypeParameterConstraints && !Options.WrappingAlignment.WrapFirstTypeParameterConstraint
			|| Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint)
			  return false;
			return !Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint;
		  }
		  if (!Options.WrappingAlignment.WrapTypeParameters && !Options.WrappingAlignment.WrapFirstTypeParameter
			|| Options.WrappingAlignment.AlignWithFirstTypeParameter)
			return false;
		  return !Options.WrappingAlignment.AlignWithFirstTypeParameter;
		case LanguageElementType.ArrayInitializerExpression:
		case LanguageElementType.ObjectInitializerExpression:
		  return Options.Indention.ArrayObjectAndCollectionInitializerContents;
		case LanguageElementType.ObjectCreationExpression:
		  if (Options.WrappingAlignment.AlignWithFirstInvocationArgument ||
			!Options.WrappingAlignment.WrapFirstInvocationArgument &&
			!Options.WrappingAlignment.WrapInvocationArguments)
			return false;
		  ObjectCreationExpression objectCreation = (ObjectCreationExpression)parent;
		  return objectCreation.Arguments != null && objectCreation.Arguments.Contains(element);
		case LanguageElementType.MethodCall:
		  if (!Options.WrappingAlignment.WrapFirstInvocationArgument && !Options.WrappingAlignment.WrapInvocationArguments
		  || Options.WrappingAlignment.AlignWithFirstInvocationArgument)
			return false;
		  MethodCall methodCall = (MethodCall)parent;
		  return methodCall.Arguments != null && methodCall.Arguments.Contains(element);
		case LanguageElementType.MethodCallExpression:
		  if (!Options.WrappingAlignment.WrapFirstInvocationArgument && !Options.WrappingAlignment.WrapInvocationArguments
		  || Options.WrappingAlignment.AlignWithFirstInvocationArgument)
			return false;
		  MethodCallExpression methodCallExpression = (MethodCallExpression)parent;
		  return methodCallExpression.Arguments != null && methodCallExpression.Arguments.Contains(element);
		case LanguageElementType.IndexerExpression:
		  if (!Options.WrappingAlignment.WrapFirstInvocationArgument && !Options.WrappingAlignment.WrapInvocationArguments
		  || Options.WrappingAlignment.AlignWithFirstInvocationArgument)
			return false;
		  IndexerExpression indexer = (IndexerExpression)parent;
		  return indexer.Arguments != null && indexer.Arguments.Contains(element);
		case LanguageElementType.Method:
		  if ((Options.WrappingAlignment.WrapFirstFormalParameter || Options.WrappingAlignment.WrapFormalParameters) && !Options.WrappingAlignment.AlignWithFirstFormalParameter)
		  {
			Method method = (Method)parent;
			if (method.Parameters != null && method.Parameters.Contains(element))
			  return true;
		  }
		  break;
		case LanguageElementType.QueryExpression:
		  return parent.Parent != null && !Options.WrappingAlignment.AlignWithFirstQueryExpressionItem && Options.WrappingAlignment.WrapQueryExpression;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (Options.WrappingAlignment.WrapFirstAncestorsListItem || Options.WrappingAlignment.WrapAncestorsListInTypeDeclaration && !Options.WrappingAlignment.AlignWithFirstAncestorsListItem)
	  {
		if (type != null)
		{
		  if (type.SecondaryAncestorTypes != null && type.SecondaryAncestorTypes.Count > 0 &&
			type.SecondaryAncestorTypes.IndexOf(element) == type.SecondaryAncestorTypes.Count - 1)
			return true;
		  if (type.PrimaryAncestorType == element)
			return true;
		}
	  }
	  if ((Options.WrappingAlignment.WrapFirstImplementsHandlesSectionItem || Options.WrappingAlignment.WrapImplementsHandlesSectionItems) && !Options.WrappingAlignment.AlignWithFirstImplementsHandlesSectionItem)
	  {
		Member member = parent as Member;
		if (member != null && (IsHandlesExpression(element, member) || IsImplementsExpression(element, member)))
		  return true;
		if (type != null && IsAncestorTypeReference(element, type))
		  return true;
	  }
	  return false;
	}
	Expression[] GetBinaryList(LanguageElement element)
	{
	  BinaryOperatorExpression parentBinary = element.Parent as BinaryOperatorExpression;
	  if (element == null || parentBinary == null)
		return null;
	  while (parentBinary.Parent is BinaryOperatorExpression)
		parentBinary = (BinaryOperatorExpression)parentBinary.Parent;
	  return GetBinaryList(parentBinary);
	}
	protected void Alignment(LanguageElement context, bool pushed)
	{
	  if (context == null)
		return;
	  if (pushed)
	  {
		if (IsFirst(context))
		{
		  if (NeedWrapFirst(context))
			AddWrappingNewLine();
		  if (NeedAlignment(context, pushed))
			AddIncreaseAlignment();
		  else if (NeedIndenting(context, pushed))
			AddIncreaseIndent();
		}
		else if (IsNotFirst(context))
		{
		  if (NeedWrapWithoutFirst(context))
			AddWrappingNewLine();
		}
		return;
	  }
	  if (!IsLast(context))
		return;
	  if (NeedAlignment(context, pushed))
		AddDecreaseAlignment();
	  else if (NeedIndenting(context, pushed))
		AddDecreaseIndent();
	}
	protected void AddDecreaseAlignment()
	{
	  TokenGen.TokenGenArgs.FormattingElements.AddDecreaseAlignment();
	}
	protected bool IsFirst(NodeList coll, LanguageElement element)
	{
	  if (coll == null || element == null || coll.Count == 0)
		return false;
	  return coll[0] == element;
	}
	protected bool IsLast(NodeList coll, LanguageElement element)
	{
	  if (coll == null || element == null)
		return false;
	  if (coll.Count == 0)
		return false;
	  return coll[coll.Count - 1] == element;
	}
	protected bool IsNotFirst(NodeList coll, LanguageElement element)
	{
	  if (coll == null || element == null || coll.Count == 0)
		return false;
	  return coll.Contains(element) && coll[0] != element;
	}
	protected virtual bool IsFirst(LanguageElement context)
	{
	  if (context == null)
		return false;
	  Variable var = context as Variable;
	  if (var != null)
		return IsFirst(var);
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  if (context.ElementType != LanguageElementType.AttributeVariableInitializer)
	  {
		Expression[] binaryList = GetBinaryList(context);
		if (binaryList != null && binaryList.Length > 0)
		  return binaryList[0] == context;
	  }
	  switch (parent.ElementType)
	  {
		case LanguageElementType.GenericModifier:
		  TypeParameterCollection typeParameters = ((GenericModifier)parent).TypeParameters;
		  return typeParameters != null && typeParameters.IndexOf(context) == 0;
		case LanguageElementType.ArrayInitializerExpression:
		  ArrayInitializerExpression arrayInit = (ArrayInitializerExpression)parent;
		  return arrayInit.Initializers != null && arrayInit.Initializers.IndexOf(context) == 0;
		case LanguageElementType.ObjectInitializerExpression:
		  ObjectInitializerExpression objectInit = (ObjectInitializerExpression)parent;
		  return objectInit.Initializers != null && objectInit.Initializers.IndexOf(context) == 0;
		case LanguageElementType.ObjectCreationExpression:
		  ObjectCreationExpression objectCreation = (ObjectCreationExpression)parent;
		  if (objectCreation.Parent is Throw)
			return false;
		  return objectCreation.Arguments != null && objectCreation.Arguments.IndexOf(context) == 0;
		case LanguageElementType.MethodCall:
		  MethodCall methodCall = (MethodCall)parent;
		  return methodCall.Arguments != null && methodCall.Arguments.IndexOf(context) == 0;
		case LanguageElementType.MethodCallExpression:
		  MethodCallExpression methodCallExpression = (MethodCallExpression)parent;
		  return methodCallExpression.Arguments != null && methodCallExpression.Arguments.IndexOf(context) == 0;
		case LanguageElementType.Method:
		  Method method = (Method)parent;
		  if (method.Parameters != null && method.Parameters.IndexOf(context) == 0)
			return true;
		  break;
		case LanguageElementType.IndexerExpression:
		case LanguageElementType.QueryExpression:
		  return parent.DetailNodes != null && parent.DetailNodes.IndexOf(context) == 0;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (type != null && IsFirstTypeAncestor(context, type))
		return true;
	  Member member = parent as Member;
	  if (member != null && (IsFirst(member.HandlesExpressions, context) || IsFirst(member.ImplementsExpressions, context)))
		return true;
	  return false;
	}
	protected virtual bool IsLast(LanguageElement context)
	{
	  if (context == null)
		return false;
	  if (context.ElementType != LanguageElementType.AttributeVariableInitializer)
	  {
		Expression[] binaryList = GetBinaryList(context);
		if (binaryList != null && binaryList.Length > 0)
		  return binaryList[binaryList.Length - 1] == context;
	  }
	  Variable var = context as Variable;
	  if (var != null)
		return IsLast(var);
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (type != null)
	  {
		if (type.SecondaryAncestorTypes == null || type.SecondaryAncestorTypes.Count == 0)
		{
		  if (type.PrimaryAncestorType == context)
			return true;
		}
		else if (IsLast(type.SecondaryAncestorTypes, context))
		  return true;
	  }
	  switch (parent.ElementType)
	  {
		case LanguageElementType.LogicalOperation:
		case LanguageElementType.BinaryOperatorExpression:
		  if (parent.Parent is BinaryOperatorExpression)
			return false;
		  return ((BinaryOperatorExpression)parent).RightSide == context;
		case LanguageElementType.GenericModifier:
		  TypeParameterCollection typeParameters = ((GenericModifier)parent).TypeParameters;
		  return IsLast(typeParameters, context);
		case LanguageElementType.ArrayInitializerExpression:
		  return IsLast(((ArrayInitializerExpression)parent).Initializers, context);
		case LanguageElementType.ObjectInitializerExpression:
		  return IsLast(((ObjectInitializerExpression)parent).Initializers, context);
		case LanguageElementType.ObjectCreationExpression:
		  if (((ObjectCreationExpression)parent).Parent is Throw)
			return false;
		  return IsLast(((ObjectCreationExpression)parent).Arguments, context);
		case LanguageElementType.MethodCall:
		  return IsLast(((MethodCall)parent).Arguments, context);
		case LanguageElementType.MethodCallExpression:
		  return IsLast(((MethodCallExpression)parent).Arguments, context);
		case LanguageElementType.IndexerExpression:
		case LanguageElementType.QueryExpression:
		  return IsLast(parent.DetailNodes, context);
	  }
	  Method method = parent as Method;
	  if (method != null && IsLast(method.Parameters, context))
		return true;
	  Member member = parent as Member;
	  if (member != null && (IsLast(member.ImplementsExpressions, context) || IsLast(member.HandlesExpressions, context)))
		return true;
	  return false;
	}
	protected virtual bool IsNotFirst(LanguageElement element)
	{
	  if (element == null)
		return false;
	  Variable var = element as Variable;
	  if (var != null)
		return IsNotFirst(var);
	  LanguageElement parent = element.Parent;
	  if (parent == null)
		return false;
	  if (element.ElementType != LanguageElementType.AttributeVariableInitializer)
	  {
		if (element is BinaryOperatorExpression)
		  return false;
		Expression[] binaryList = GetBinaryList(element);
		if (binaryList != null && binaryList.Length > 0)
		  return binaryList[0] != element;
	  }
	  switch (parent.ElementType)
	  {
		case LanguageElementType.GenericModifier:
		  TypeParameterCollection typeParameters = ((GenericModifier)parent).TypeParameters;
		  return typeParameters != null && typeParameters.IndexOf(element) > 0;
		case LanguageElementType.ArrayInitializerExpression:
		  ArrayInitializerExpression arrayInit = (ArrayInitializerExpression)parent;
		  return arrayInit.Initializers != null && arrayInit.Initializers.IndexOf(element) > 0;
		case LanguageElementType.ObjectInitializerExpression:
		  ObjectInitializerExpression objectInit = (ObjectInitializerExpression)parent;
		  return objectInit.Initializers != null && objectInit.Initializers.IndexOf(element) > 0;
		case LanguageElementType.ObjectCreationExpression:
		  ObjectCreationExpression objectCreation = (ObjectCreationExpression)parent;
		  if (objectCreation.Parent is Throw)
			return false;
		  return objectCreation.Arguments != null && objectCreation.Arguments.IndexOf(element) > 0;
		case LanguageElementType.MethodCall:
		  MethodCall methodCall = (MethodCall)parent;
		  return methodCall.Arguments != null && methodCall.Arguments.IndexOf(element) > 0;
		case LanguageElementType.MethodCallExpression:
		  MethodCallExpression methodCallExpression = (MethodCallExpression)parent;
		  return methodCallExpression.Arguments != null && methodCallExpression.Arguments.IndexOf(element) > 0;
		case LanguageElementType.Method:
		  Method method = (Method)parent;
		  if (method.Parameters != null && method.Parameters.IndexOf(element) > 0)
			return true;
		  break;
		case LanguageElementType.IndexerExpression:
		case LanguageElementType.QueryExpression:
		  return parent.DetailNodes != null && parent.DetailNodes.IndexOf(element) > 0;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (type != null && IsAncestorTypeReference(element, type) && !IsFirstTypeAncestor(element, type))
		return true;
	  Member member = parent as Member;
	  if (member != null && (IsNotFirst(member.ImplementsExpressions, element) || IsNotFirst(member.HandlesExpressions, element)))
		return true;
	  return false;
	}
	protected virtual bool NeedWrapWithoutFirst(LanguageElement context)
	{
	  if (context == null)
		return false;
	  Variable var = Context as Variable;
	  if (var != null)
		return Options.WrappingAlignment.WrapMultiVariableDeclaration;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  switch (parent.ElementType)
	  {
		case LanguageElementType.LogicalOperation:
		  return Options.WrappingAlignment.WrapLogicalOperatorExpression;
		case LanguageElementType.BinaryOperatorExpression:
		  return Options.WrappingAlignment.WrapBinaryOperatorExpression;
		case LanguageElementType.GenericModifier:
		  if (ElementWasGenerated(context))
			return Options.WrappingAlignment.WrapTypeParameterConstraints;
		  return Options.WrappingAlignment.WrapTypeParameters;
		case LanguageElementType.ArrayInitializerExpression:
		  return Options.WrappingAlignment.WrapArrayObjectAndCollectionInitializers;
		case LanguageElementType.ObjectCreationExpression:
		  ObjectCreationExpression expression = (ObjectCreationExpression)parent;
		  if (expression.Parent is Throw)
			return false;
		  if (expression.Arguments == null || expression.Arguments.Count == 0)
			return false;
		  return Options.WrappingAlignment.WrapInvocationArguments;
		case LanguageElementType.MethodCallExpression:
		  return Options.WrappingAlignment.WrapInvocationArguments;
		case LanguageElementType.ObjectInitializerExpression:
		  LanguageElement parentParent = parent.Parent;
		  if (parentParent != null && string.IsNullOrEmpty(parentParent.Name))
			return Options.WrappingAlignment.WrapMembersInAnonymousType;
		  return Options.WrappingAlignment.WrapArrayObjectAndCollectionInitializers;
		case LanguageElementType.IndexerExpression:
		case LanguageElementType.MethodCall:
		  return Options.WrappingAlignment.WrapInvocationArguments;
		case LanguageElementType.QueryExpression:
		  return Options.WrappingAlignment.WrapQueryExpression;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (Options.WrappingAlignment.WrapAncestorsListInTypeDeclaration && type != null && IsSecondaryAncestorTypeReference(context, type))
		return true;
	  if (Options.WrappingAlignment.WrapImplementsHandlesSectionItems)
	  {
		Member member = parent as Member;
		if (member != null && (IsNotFirst(member.HandlesExpressions, context) || IsNotFirst(member.ImplementsExpressions, context)))
		  return true;
		if (type != null && IsNotFirst(type.SecondaryAncestorTypes, context))
		  return true;
	  }
	  if (Options.WrappingAlignment.WrapFormalParameters)
	  {
		Method method = parent as Method;
		if (method != null && IsNotFirst(method.Parameters, context))
		  return true;
	  }
	  return false;
	}
	protected virtual bool NeedWrapFirst(LanguageElement context)
	{
	  if (context == null)
		return false;
	  if (context is Variable)
		return Options.WrappingAlignment.WrapFirstMultiVariableDeclaration;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  switch (parent.ElementType)
	  {
		case LanguageElementType.GenericModifier:
		  if (!ElementWasGenerated(context))
			return Options.WrappingAlignment.WrapFirstTypeParameter;
		  return Options.WrappingAlignment.WrapFirstTypeParameterConstraint;
		case LanguageElementType.ArrayInitializerExpression:
		  return Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer;
		case LanguageElementType.ObjectCreationExpression:
		  ObjectCreationExpression expression = (ObjectCreationExpression)parent;
		  if (expression.Parent is Throw)
			return false;
		  if (expression.Arguments == null || expression.Arguments.Count == 0)
			return false;
		  return Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.MethodCallExpression:
		  return Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.ObjectInitializerExpression:
		  LanguageElement parentParent = parent.Parent;
		  if (parentParent != null && string.IsNullOrEmpty(parentParent.Name))
			return Options.WrappingAlignment.WrapFirstMemberInAnonymousType;
		  return Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer;
		case LanguageElementType.IndexerExpression:
		case LanguageElementType.MethodCall:
		  return Options.WrappingAlignment.WrapFirstInvocationArgument;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (Options.WrappingAlignment.WrapFirstAncestorsListItem && type != null)
		return type.PrimaryAncestorType == context;
	  if (Options.WrappingAlignment.WrapFirstFormalParameter)
	  {
		Method method = parent as Method;
		if (method != null && IsFirst(method.Parameters, context))
		  return true;
	  }
	  if (Options.WrappingAlignment.WrapFirstImplementsHandlesSectionItem)
	  {
		Member member = parent as Member;
		if (member != null && (IsFirst(member.HandlesExpressions, context) || IsFirst(member.ImplementsExpressions, context)))
		  return true;
		if (type != null)
		  return IsFirst(type.SecondaryAncestorTypes, context);
	  }
	  return false;
	}
	protected virtual bool NeedAlignment(LanguageElement context, bool pushed)
	{
	  if (context == null)
		return false;
	  if (context is Variable)
		return !pushed && Options.WrappingAlignment.AlignWithFirstMultiVariableDeclarationItem && !Options.WrappingAlignment.WrapFirstMultiVariableDeclaration;
	  LanguageElement parent = context.Parent;
	  if (parent == null)
		return false;
	  switch (parent.ElementType)
	  {
		case LanguageElementType.LogicalOperation:
		  return Options.WrappingAlignment.AlignWithFirstLogicalExpressionItem;
		case LanguageElementType.BinaryOperatorExpression:
		  return Options.WrappingAlignment.AlignWithFirstBinaryExpressionItem;
		case LanguageElementType.GenericModifier:
		  if (!(context is TypeParameter))
			return false;
		  if (pushed)
		  {
			if (ElementWasGenerated(context))
			  return Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint && !Options.WrappingAlignment.WrapFirstTypeParameterConstraint;
			return Options.WrappingAlignment.AlignWithFirstTypeParameter && !Options.WrappingAlignment.WrapFirstTypeParameter;
		  }
		  if (!ElementWasGenerated(context))
			return Options.WrappingAlignment.AlignWithFirstTypeParameterConstraint && !Options.WrappingAlignment.WrapFirstTypeParameterConstraint;
		  return Options.WrappingAlignment.AlignWithFirstTypeParameter && !Options.WrappingAlignment.WrapFirstTypeParameter;
		case LanguageElementType.ArrayInitializerExpression:
		  ExpressionCollection initializers = ((ArrayInitializerExpression)parent).Initializers;
		  if (initializers == null || initializers.Count < 2)
			return false;
		  return Options.WrappingAlignment.AlignWithFirstArrayObjectOrCollectionInitializerItem && !Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer;
		case LanguageElementType.ObjectCreationExpression:
		  ObjectCreationExpression expression = (ObjectCreationExpression)parent;
		  if (expression.Parent is Throw)
			return false;
		  if (expression.Arguments == null || expression.Arguments.Count < 2)
			return false;
		  return Options.WrappingAlignment.AlignWithFirstInvocationArgument && !Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.MethodCallExpression:
		  if (((MethodCallExpression)parent).ArgumentsCount < 2)
			return false;
		  return Options.WrappingAlignment.AlignWithFirstInvocationArgument && !Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.ObjectInitializerExpression:
		  LanguageElement parentParent = parent.Parent;
		  initializers = ((ObjectInitializerExpression)parent).Initializers;
		  if (initializers == null || initializers.Count < 2)
			return false;
		  if (parentParent != null && string.IsNullOrEmpty(parentParent.Name))
			return Options.WrappingAlignment.AlignWithFirstMemberInAnonymousType && !Options.WrappingAlignment.WrapFirstMemberInAnonymousType;
		  return Options.WrappingAlignment.AlignWithFirstArrayObjectOrCollectionInitializerItem && !Options.WrappingAlignment.WrapFirstArrayObjectAndCollectionInitializer;
		case LanguageElementType.IndexerExpression:
		  ExpressionCollection arguments = ((IndexerExpression)parent).Arguments;
		  if (arguments == null || arguments.Count < 2)
			return false;
		  return Options.WrappingAlignment.AlignWithFirstInvocationArgument && !Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.MethodCall:
		  if (((MethodCall)parent).ArgumentsCount < 2)
			return false;
		  return Options.WrappingAlignment.AlignWithFirstInvocationArgument && !Options.WrappingAlignment.WrapFirstInvocationArgument;
		case LanguageElementType.QueryExpression:
		  return Options.WrappingAlignment.AlignWithFirstQueryExpressionItem;
	  }
	  TypeDeclaration type = parent as TypeDeclaration;
	  if (Options.WrappingAlignment.AlignWithFirstAncestorsListItem && !Options.WrappingAlignment.WrapFirstAncestorsListItem && type != null)
		if (IsAncestorTypeReference(context, type))
		  return true;
	  if (Options.WrappingAlignment.AlignWithFirstFormalParameter && !Options.WrappingAlignment.WrapFirstFormalParameter)
	  {
		Method method = parent as Method;
		if (method != null && method.ParameterCount >= 2 && method.Parameters.Contains(context))
		  return true;
	  }
	  if (Options.WrappingAlignment.AlignWithFirstImplementsHandlesSectionItem && !Options.WrappingAlignment.WrapFirstImplementsHandlesSectionItem)
	  {
		Member member = parent as Member;
		if (member != null && (IsHandlesExpression(context, member) || IsImplementsExpression(context, parent as Member)))
		  return true;
		if (type != null && IsAncestorTypeReference(context, type))
		  return true;
	  }
	  return false;
	}
	protected virtual bool IsFirstTypeAncestor(LanguageElement context, TypeDeclaration type)
	{
	  return type.PrimaryAncestorType == context;
	}
	protected virtual void AddWrappingNewLine()
	{
	  AddNewLineIfNeeded();
	}
	protected virtual void InitializeCodeGenObjects()
	{
	  _DirectiveGen = CreateDirectiveGen();
	  _ExpressionGen = CreateExpressionGen();
	  _MemberGen = CreateMemberGen();
	  _StatementGen = CreateStatementGen();
	  _SupportElementGen = CreateSupportElementGen();
	  _TypeDeclarationGen = CreateTypeDeclarationGen();
	  _XmlGen = CreateXmlGen();
	  _NamespaceReferenceGen = CreateNamespaceReferenceGen();
	  _NamespaceGen = CreateNamespaceGen();
	  _SnippetGen = CreateSnippetGen();
	  _TemplateGen = CreateTemplateGen();
	  _TemplateParameterGen = CreateTemplateParameterGen();
	  _SourceFileGen = CreateSourceFileGen();
	  _HtmlXmlCodeGen = CreateHtmlXmlGen();
	}
	protected virtual void ContextPushed()
	{
	}
	protected virtual void ContextPoped(LanguageElement oldContext)
	{
	}
	protected virtual SourceFileCodeGenBase CreateSourceFileGen()
	{
	  return new SourceFileCodeGenBase(this);
	}
	protected virtual bool NeedGenerateSideComment(LanguageElement element)
	{
	  if (element == null || GenCommentsFromToken)
		return false;
	  return !(element is Expression) && !SaveFormat;
	}
	protected virtual void GenerateElementList(LanguageElement element)
	{
	  ExpressionGen.GenerateElementCollection(element.Nodes, FormattingTokenType.None);
	}
	protected virtual bool IsSpecificElement(LanguageElement element)
	{
	  return false;
	}
	protected virtual void GenerateSpecificElement(LanguageElement element)
	{
	}
	protected abstract DirectiveCodeGenBase CreateDirectiveGen();
	protected abstract ExpressionCodeGenBase CreateExpressionGen();
	protected abstract MemberCodeGenBase CreateMemberGen();
	protected abstract StatementCodeGenBase CreateStatementGen();
	protected abstract SupportElementCodeGenBase CreateSupportElementGen();
	protected abstract TypeDeclarationCodeGenBase CreateTypeDeclarationGen();
	protected abstract XmlCodeGenBase CreateXmlGen();
	protected abstract NamespaceReferenceGenBase CreateNamespaceReferenceGen();
	protected abstract NamespaceGenBase CreateNamespaceGen();
	protected abstract SnippetCodeGenBase CreateSnippetGen();
	protected abstract TemplateCodeGenBase CreateTemplateGen();
	protected abstract TemplateParameterCodeGenBase CreateTemplateParameterGen();
	protected abstract HtmlXmlCodeGenBase CreateHtmlXmlGen();
	internal void AddIncreaseAlignment()
	{
	  TokenGen.TokenGenArgs.FormattingElements.AddIncreaseAlignment();
	}
	internal bool ElementWasGenerated(LanguageElement element)
	{
	  if (element == null || _GeneratedElements == null)
		return false;
	  return _GeneratedElements.Contains(element);
	}
	internal void AddGeneratedElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  if (_GeneratedElements == null)
		_GeneratedElements = new List<LanguageElement>();
	  _GeneratedElements.Add(element);
	}
	internal void RemoveGeneratedElement(LanguageElement element)
	{
	  if (element == null || _GeneratedElements == null)
		return;
	  _GeneratedElements.Remove(element);
	}
	internal LanguageElement GetPrevContext()
	{
	  int count = Contexts.Count;
	  if (count < 2)
		return null;
	  LanguageElement element = Contexts.Pop();
	  LanguageElement result = null;
	  if (Contexts.Count > 0)
		result = Contexts.Peek();
	  Contexts.Push(element);
	  return result;
	}
	internal void GenerateElementInternal(LanguageElementCodeGenBase codeGen, LanguageElement element)
	{
	  if (codeGen == null)
		return;
	  LanguageElementCodeGenBase oldElementCodeGen = TokenGen.ElementCodeGen;
	  try
	  {
		TokenGen.ElementCodeGen = codeGen;
		if (element == null)
		  return;
		codeGen.GenerateElement(element);
	  }
	  finally
	  {
		TokenGen.ElementCodeGen = oldElementCodeGen;
	  }
	}
	internal bool TokenArgsContainsElement(object element)
	{
	  return TokenGen.TokenArgsContainsElement(element);
	}
	internal TokenGenArgs GetArgs()
	{
	  return TokenGen.TokenGenArgs;
	}
	internal void RestoreSavedArgs(TokenGenArgs args)
	{
	  if ((args == null) || (args.FormattingElements == null))
		return;
	  if (TokenGen.FormattingElements != null)
		args.FormattingElements.InsertRange(0, TokenGen.FormattingElements);
	  TokenGen.TokenGenArgs = args;
	}
	internal FormattingTokenGen TokenGen
	{
	  get
	  {
		if (_TokenGen == null)
		  _TokenGen = new FormattingTokenGen(this);
		return _TokenGen;
	  }
	}
	internal virtual FormattingTable FormattingTable
	{
	  get { return null; }
	}
	internal GeneratingElements Contexts
	{
	  get
	  {
		return _Contexts;
	  }
	}
	public override void GenerateElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  if (_FirstElement == null)
		_FirstElement = element;
	  PushContext(element);
	  if (NeedGenerateSideComment(element))
		GenerateElementForeComment(element);
	  SupportElementGen.GenerateAttributes(element);
	  if (element is ISnippetCodeElement)
		GenerateElementInternal(SnippetGen, element);
	  else if (element is Expression)
		GenerateElementInternal(ExpressionGen, element);
	  else if (element is Statement)
		GenerateElementInternal(StatementGen, element);
	  else if (MemberGen.IsMemberGenElement(element))
		GenerateElementInternal(MemberGen, element);
	  else if (element is TypeDeclaration)
		GenerateElementInternal(TypeDeclarationGen, element);
	  else if (element is ConstructorInitializer)
		GenerateElementInternal(StatementGen, element);
	  else if (element is NamespaceReference)
		GenerateElementInternal(NamespaceReferenceGen, element);
	  else if (element is ExternAlias)
		GenerateElementInternal(NamespaceReferenceGen, element);
	  else if (element is Namespace)
		GenerateElementInternal(NamespaceGen, element);
	  else if (XmlGen.IsXmlGenElement(element))
		GenerateElementInternal(XmlGen, element);
	  else if (element is XmlElement)
		GenerateElementInternal(XmlGen, element);
	  else if (element is PreprocessorDirective)
		GenerateElementInternal(DirectiveGen, element);
	  else if (element is TemplateModifier)
		GenerateElementInternal(TemplateGen, element);
	  else if (element is TemplateParameter)
		GenerateElementInternal(TemplateParameterGen, element);
	  else if (element is SourceFile)
		GenerateElementInternal(SourceFileGen, element);
	  else if (IsSpecificElement(element))
		GenerateSpecificElement(element);
	  else if (element is ElementList)
		GenerateElementList(element); 
	  else if (element is MemberVisibilitySpecifier)
		GenerateMemberVisibilitySpecifier((MemberVisibilitySpecifier)element);
	  else if (element is GenericModifier)
		MemberGen.GenerateGenericModifier((GenericModifier)element);
	  else if (element is SupportElement)
	  {
		if (SaveFormat)
		{
		  if (GeneratingInSupportElement)
			GenerateElementInternal(SupportElementGen, element);
		}
		else
		{
		  if (element.ElementType != LanguageElementType.AttributeSection || ((SupportElement)element).TargetNode == null)
		  {
			if (!IsSkiped(element) && _FirstElement != element)
			{
			  GenerateElementInternal(SupportElementGen, element);
			  AddSkiped(element);
			}
			else if (_FirstElement == element || ((SupportElement)element).TargetNode == null)
			  GenerateElementInternal(SupportElementGen, element);
		  }
		  else if (element.ElementType == LanguageElementType.AttributeSection && !IsSkiped(element))
		  {
			if (element.Parent is ElementList)
			{
			  GenerateElementInternal(SupportElementGen, element);
			  AddSkiped(element);
			}
			else if ((this is CSharp.CSharpCodeGen || this is VB.VBCodeGen)&& PrevContextHasAttributeSection(element as AttributeSection))
			  GenerateElementInternal(SupportElementGen, element);
		  }
		}
	  }
	  else
		ExpressionGen.GenerateElementCollection(element.Nodes, FormattingTokenType.None);
	  GenerateContextBackComment();
	  PopContext();
	}
	public override bool IsSkiped(LanguageElement element)
	{
	  if (base.IsSkiped(element))
		return true;
	  return TokenArgsContainsElement(element);
	}
	public string GetTokenText(FormattingTokenType type)
	{
	  return GetTokenText(type, Context);
	}
	public string GetTokenText(FormattingTokenType type, int contextIndex)
	{
	  LanguageElement context = null;
	  if (Contexts != null && contextIndex >= 0 && contextIndex < Contexts.Count)
		context = Contexts[contextIndex];
	  return GetTokenText(type, context);
	}
	public void GenerateContextBackComment()
	{
	  LanguageElement element = Context;
	  if (element == null)
		return;
	  if (NeedGenerateSideComment(element))
		GenerateElementBackComment(element);
	  if (element != null && _FirstElement != null && element == _FirstElement)
	  {
		_FirstElement = null;
		ClearSkipedElements();
	  }
	}
	public void GenerateElementForeComment(LanguageElement element)
	{
	  if (SupportElementGen == null || element == null)
		return;
	  SupportElementGen.GenerateElementForeComment(element);
	}
	public void GenerateElementBackComment(LanguageElement element)
	{
	  if (SupportElementGen == null || element == null)
		return;
	  SupportElementGen.GenerateElementBackComment(element);
	}
	public void Write(FormattingTokenType type)
	{
	  Write(type, 0);
	}
	public void Write(FormattingTokenType type, int index)
	{
	  TokenGen.WriteCode(new GenTextArgs(this, type, index));
	}
	public void IncreaseIndent()
	{
	  Code.IncreaseIndent();
	}
	public void DecreaseIndent()
	{
	  Code.DecreaseIndent();
	}
	public void AddIncreaseIndent()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddIncreaseIndent();
	}
	public void AddDecreaseIndent()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddDecreaseIndent();
	}
	public void AddClearIndent()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddClearIndent();
	}
	public void AddRestoreIndent()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddRestoreIndent();
	}
	public void AddNewLineIfNeeded()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddNewLineIfNeeded();
	}
	public void AddNewLine(int countConsecutive)
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddNewLine(countConsecutive);
	}
	public void AddWSIfNeeded()
	{
	  if (SaveFormat)
		return;
	  TokenGen.AddWSIfNeeded();
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void PushContext(LanguageElement element)
	{
	  if (element == null)
		return;
	  Contexts.Push(element);
	  if (!SaveFormat)
		ContextPushed();
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public LanguageElement PopContext()
	{
	  LanguageElement element = Contexts.Pop();
	  if (!SaveFormat)
		ContextPoped(element);
	  if ((element is PreprocessorDirective || element is Comment) && TokenGen.GeneratingInSupportElement)
		return element;
	  if (Contexts.Count == 0)
	  {
		bool isExpression = element is Expression || element is Param;
		if (!isExpression && element is ElementList)
		{
		  ElementList list = (ElementList)element;
		  if (list.NodeCount == 1)
		  {
			LanguageElement first = (LanguageElement)list.Nodes[0];
			isExpression = first is Expression || first is Param || first is XmlDocComment;
		  }
		}
		if (!SaveFormat && !(element is SourceFile) && !isExpression)
		  if (this is CSharp.CSharpCodeGen) 
		  {
			ElementList list = element as ElementList;
			if (list == null || list.NodeCount > 0)
			  AddNewLineIfNeeded();
		  }
		  else if (this is JavaScript.JavaScriptCodegen)
		  {
			ElementList list = element as ElementList;
			if (list == null || list.NodeCount > 0)
			  AddNewLineIfNeeded();
		  }
		  else
			if (this is VB.VBCodeGen && !string.IsNullOrEmpty(Code.LastLine))
			  AddNewLineIfNeeded();
		TokenGen.WriteResiduaryFormattingElements();
	  }
	  return element;
	}
	public virtual FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  return new FormattingElements();
	}
	public virtual FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  return new FormattingElements();
	}
	public virtual void WriteWS()
	{
	  Code.Write(" ");
	}
	public virtual void WriteTab()
	{
	  Code.Write("\t");
	}
	public virtual void WriteEOL()
	{
	  Code.WriteLine();
	}
	public virtual void WriteText(GenTextArgs args)
	{
	  Code.Write(args.Text, args.SplitLines);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public string GenerateStatements(ICollection elements) {
		if(elements == null || StatementGen == null)
			return null;
		StatementGen.GenerateElementCollection(elements);
		return Code.Code;
	}
	public abstract void GenerateMemberVisibilitySpecifier(MemberVisibilitySpecifier specifier);
	public LanguageElement Context
	{
	  get
	  {
		if (Contexts == null || Contexts.Count == 0)
		  return null;
		return Contexts.Peek();
	  }
	}
	public TemplateParameterCodeGenBase TemplateParameterGen
	{
	  get { return _TemplateParameterGen; }
	}
	public SourceFileCodeGenBase SourceFileGen
	{
	  get { return _SourceFileGen; }
	}
	public TemplateCodeGenBase TemplateGen
	{
	  get { return _TemplateGen; }
	}
	public DirectiveCodeGenBase DirectiveGen
	{
	  get
	  {
		return _DirectiveGen;
	  }
	}
	public ExpressionCodeGenBase ExpressionGen
	{
	  get
	  {
		return _ExpressionGen;
	  }
	}
	public MemberCodeGenBase MemberGen
	{
	  get
	  {
		return _MemberGen;
	  }
	}
	public SupportElementCodeGenBase SupportElementGen
	{
	  get
	  {
		return _SupportElementGen;
	  }
	}
	public StatementCodeGenBase StatementGen
	{
	  get
	  {
		return _StatementGen;
	  }
	}
	public TypeDeclarationCodeGenBase TypeDeclarationGen
	{
	  get
	  {
		return _TypeDeclarationGen;
	  }
	}
	public XmlCodeGenBase XmlGen
	{
	  get
	  {
		return _XmlGen;
	  }
	}
	public NamespaceReferenceGenBase NamespaceReferenceGen
	{
	  get
	  {
		return _NamespaceReferenceGen;
	  }
	}
	public NamespaceGenBase NamespaceGen
	{
	  get
	  {
		return _NamespaceGen;
	  }
	}
	public SnippetCodeGenBase SnippetGen
	{
	  get
	  {
		return _SnippetGen;
	  }
	}
	public override CodeGenOptions Options
	{
	  get
	  {
		if (SaveFormat)
		  _Options.SaveFormat = true;
		else
		  _Options.SaveFormat = false;
		return _Options;
	  }
	}
	  [EditorBrowsable(EditorBrowsableState.Never)]
	public bool GenCommentsFromToken
	{
	  set
	  {
		_GenCommentsFromToken = value;
	  }
	  get
	  {
		return _GenCommentsFromToken;
	  }
	}
	public HtmlXmlCodeGenBase HtmlXmlCodeGen
	{
	  get
	  {
		return _HtmlXmlCodeGen;
	  }
	}
	public bool GeneratingInSupportElement
	{
	  get { return TokenGen.GeneratingInSupportElement; }
	  set { TokenGen.GeneratingInSupportElement = value; }
	}
  }
}
