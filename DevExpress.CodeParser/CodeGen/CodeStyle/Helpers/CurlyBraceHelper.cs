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
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  static class CurlyBraceHelper
  {
	static int GetNodeCount(LanguageElement element)
	{
	  if (element == null)
		return 0;
	  return GetNodeCount(element.Nodes);
	}
	static int GetNodeCount(NodeList nodes)
	{
	  if (nodes == null || nodes.Count == 0)
		return 0;
	  int result = 0;
	  foreach (LanguageElement node in nodes)
		if (!(node is SupportElement))
		  result++;
	  return result;
	}
	static bool IsSimpleElement(LanguageElement element, CodeGenOptions options)
	{
	  if (element == null)
		return true;
	  if (element.NodeCount == 0 && element.DetailNodeCount == 0)
		return true;
	  if (element.NodeCount > 1)
		return false;
	  if (element.NodeCount > 0)
		foreach (LanguageElement node in element.Nodes)
		  if (!IsSimpleElement(node, options))
			return false;
	  if (element.DetailNodeCount > 0)
		foreach (LanguageElement node in element.DetailNodes)
		  if (!IsSimpleElement(node, options))
			return false;
	  switch (element.ElementType)
	  {
		case LanguageElementType.UsingStatement:
		case LanguageElementType.Else:
		case LanguageElementType.Try:
		case LanguageElementType.Catch:
		case LanguageElementType.Finally:
		case LanguageElementType.Lock:
		  return false;
	  }
	  if (element is ArrayInitializerExpression)
	  {
		ArrayInitializerExpression expr = (ArrayInitializerExpression)element;
		if (expr.Initializers.Count > 1)
		  return false;
	  }
	  if (element is Method && ((Method)element).Parameters.Count > 0 && 
		(options.WrappingAlignment.WrapFirstFormalParameter || 
		options.WrappingAlignment.WrapFormalParameters))
		return false;
	  if (element is MethodCall && ((MethodCall)element).ArgumentsCount > 0 &&
		(options.WrappingAlignment.WrapFirstInvocationArgument ||
		options.WrappingAlignment.WrapInvocationArguments))
		return false;
	  if (element is MethodCallExpression && ((MethodCallExpression)element).ArgumentsCount > 0 &&
		(options.WrappingAlignment.WrapFirstInvocationArgument ||
		options.WrappingAlignment.WrapInvocationArguments))
		return false;
		if (element is LambdaExpression && !options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine &&
		(options.LineBreaks.PlaceCloseBraceOnNewLineForLambdaExpressions || options.LineBreaks.PlaceOpenBraceOnNewLineForLambdaExpressions))
		  return false;
	  if (element is AnonymousMethodExpression && !options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine &&
		(options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousMethods || options.LineBreaks.PlaceCloseBraceOnNewLineForAnonymousMethods))
		return false;
	  if (element is If && !options.LineBreaks.PlaceSimpleEmbeddedStatementOnSingleLine)
		return false;
	  if (element is Switch)
		return false;
	  return true;
	}
	#region AddTokensBeforeOpen
	public static void AddTokensBeforeOpen(FormattingElements tokens, CodeGen codeGen)
	{
	  CodeGenOptions options = codeGen.Options;
	  LanguageElement context = codeGen.Context;
	  bool lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForCodeBlocks && context.ElementType != LanguageElementType.MarkupExtensionExpression;
	  bool space = options.Spacing.BeforeOpenCurlyBraceOnTheSameLine && context.ElementType != LanguageElementType.MarkupExtensionExpression;
	  bool onSingleLine;
	  switch (context.ElementType)
	  {
		case LanguageElementType.Property:
		  Property property = (Property)context;
		  bool parentIsInterface = property.Parent != null && property.Parent.ElementType == LanguageElementType.Interface;
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  if (parentIsInterface || property.IsAbstract)
			onSingleLine = options.LineBreaks.PlaceAbstractInterfaceMemberOnSingleLine;
		  else
			if (property.IsAutoImplemented)
			  onSingleLine = options.LineBreaks.PlaceAutoImplementedPropertyOnSingleLine;
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceOpenBraceOnNewLineForProperties;
		  space = options.Spacing.BeforeOpeningBraceInProperty;
		  break;
		case LanguageElementType.PropertyAccessorGet:
		case LanguageElementType.PropertyAccessorSet:
		  onSingleLine = options.LineBreaks.PlaceSimpleAccessorOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = !onSingleLine && lineBreak;
		  space = options.Spacing.BeforeOpeningBraceInAccessor;
		  break;
		case LanguageElementType.Method:
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForMethods && !onSingleLine;
		  space = options.Spacing.BeforeOpenCurlyBraceOnTheSameLine;
		  break;
		case LanguageElementType.Namespace:
		  lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForNamespaces;
		  space = options.Spacing.BeforeOpenCurlyBraceOnTheSameLine;
		  break;
		case LanguageElementType.EventAdd:
		case LanguageElementType.EventRemove:
		  onSingleLine = options.LineBreaks.PlaceSimpleAccessorOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.BeforeOpeningBraceInAccessor;
		  lineBreak = !onSingleLine && lineBreak;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Interface:
		case LanguageElementType.Struct:
		case LanguageElementType.Enum:
		  lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForTypeDeclarations;
		  space = options.Spacing.BeforeOpenCurlyBraceOnTheSameLine;
		  break;
		case LanguageElementType.Block:
		  LanguageElement parent = context.Parent;
		  if (parent != null && parent.NodeCount == 1 && parent.ElementType == LanguageElementType.Case)
			lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement;
		  break;
		case LanguageElementType.Event:
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.BeforeOpeningBraceInProperty;
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceOpenBraceOnNewLineForProperties;
		  break;
		case LanguageElementType.LambdaExpression:
		  onSingleLine = options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  bool isSimleExpression = context.NodeCount > 0 && context.Nodes[0] is Expression;
		  bool placeOpenBraceOnNewLine = options.LineBreaks.PlaceOpenBraceOnNewLineForLambdaExpressions && !isSimleExpression;
		  space = !options.Spacing.AroundLambdaExpressionOperator && space;
		  lineBreak = !onSingleLine && placeOpenBraceOnNewLine;
		  break;
		case LanguageElementType.AnonymousMethodExpression:
		  onSingleLine = options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousMethods;
		  break;
		case LanguageElementType.ArrayInitializerExpression:
		case LanguageElementType.ObjectInitializerExpression:
		  ObjectCreationExpression objCreation = context.Parent as ObjectCreationExpression;
		  if (objCreation != null && objCreation.ObjectType == null)
			lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForAnonymousTypes;
		  else
			lineBreak = options.LineBreaks.PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers;
		  break;
	  }
	  if (lineBreak)
		tokens.AddNewLine();
	  else
		if (space)
		  tokens.AddWhiteSpace();
	  if (options.Indention.OpenAndCloseBraces)
		tokens.AddIncreaseIndent();
	}
	#endregion
	#region AddTokensAfterOpen
	public static void AddTokensAfterOpen(FormattingElements tokens, CodeGen codeGen)
	{
	  CodeGenOptions options = codeGen.Options;
	  LanguageElement context = codeGen.Context;
	  if (context.ElementType == LanguageElementType.MarkupExtensionExpression)
		return;
	  bool space = false;
	  if (options.Indention.OpenAndCloseBraces)
		tokens.AddDecreaseIndent();
	  int nodeCount = GetNodeCount(context);
	  bool lineBreak = !(options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options));
	  bool indent = options.Indention.CodeBlockContents;
	  switch (context.ElementType)
	  {
		case LanguageElementType.Property:
		  Property property = (Property)context;
		  bool parentIsInterface = property.Parent != null && property.Parent.ElementType == LanguageElementType.Interface;
		  bool onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  if (parentIsInterface || property.IsAbstract)
			onSingleLine = options.LineBreaks.PlaceAbstractInterfaceMemberOnSingleLine;
		  else
			if (property.IsAutoImplemented)
			  onSingleLine = options.LineBreaks.PlaceAutoImplementedPropertyOnSingleLine;
		  space = onSingleLine && options.Spacing.WithinBracesInPropertyDeclaration && nodeCount > 0;
		  lineBreak = !onSingleLine;
		  break;
		case LanguageElementType.PropertyAccessorGet:
		case LanguageElementType.PropertyAccessorSet:
		case LanguageElementType.EventAdd:
		case LanguageElementType.EventRemove:
		  onSingleLine = options.LineBreaks.PlaceSimpleAccessorOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.WithinSingleLineAccessor && onSingleLine;
		  lineBreak = !onSingleLine;
		  break;
		case LanguageElementType.Method:
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.WithinSingleLineMethod && onSingleLine;
		  lineBreak = !onSingleLine && nodeCount > 0;
		  break;
		case LanguageElementType.Namespace:
		  lineBreak = nodeCount > 0;
		  break;
		case LanguageElementType.Event:
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = !onSingleLine;
		  break;
		case LanguageElementType.LambdaExpression:
		case LanguageElementType.AnonymousMethodExpression:
		  onSingleLine = options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.WithinSingleLineAnonymousMethod && onSingleLine;
		  indent = options.Indention.AnonymousMethodContents;
		  lineBreak = !onSingleLine;
		  break;
		case LanguageElementType.ArrayInitializerExpression:
		case LanguageElementType.ObjectInitializerExpression:
		  space = options.Spacing.WithinArrayInitializerBraces;
		  lineBreak = false;
		  indent = false;
		  break;
		case LanguageElementType.Switch:
		  indent = options.Indention.CaseStatementFromSwitchStatement;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Interface:
		case LanguageElementType.Struct:
		case LanguageElementType.Enum:
		  lineBreak = nodeCount > 0;
		  break;
	  }
	  if (lineBreak)
		tokens.AddNewLine();
	  else
		if (space)
		  tokens.AddWhiteSpace();
	  if (indent)
		tokens.AddIncreaseIndent();
	}
	#endregion
	#region AddTokensBeforeClose
	public static void AddTokensBeforeClose(FormattingElements tokens, CodeGen codeGen)
	{
	  LanguageElement context = codeGen.Context;
	  if (context.ElementType == LanguageElementType.MarkupExtensionExpression)
		return;
	  CodeGenOptions options = codeGen.Options;
	  int nodeCount = GetNodeCount(context);
	  bool needLineBreakForSimple = !(options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options));
	  bool lineBreak =  needLineBreakForSimple && options.LineBreaks.PlaceCloseBraceOnNewLineForCodeBlocks && nodeCount > 0;
	  bool space = false;
	  bool indent = options.Indention.CodeBlockContents;
	  switch (context.ElementType)
	  {
		case LanguageElementType.Property:
		  Property property = (Property)context;
		  bool parentIsInterface = property.Parent != null && property.Parent.ElementType == LanguageElementType.Interface;
		  bool onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  if (parentIsInterface || property.IsAbstract)
			onSingleLine = options.LineBreaks.PlaceAbstractInterfaceMemberOnSingleLine;
		  else
			if (property.IsAutoImplemented)
			  onSingleLine = options.LineBreaks.PlaceAutoImplementedPropertyOnSingleLine;
		  lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForProperties && !onSingleLine && nodeCount > 0;
		  space = options.Spacing.WithinBracesInPropertyDeclaration && !property.IsAutoImplemented && !property.IsAbstract && nodeCount > 0;
		  break;
		case LanguageElementType.PropertyAccessorGet:
		case LanguageElementType.PropertyAccessorSet:
		case LanguageElementType.EventAdd:
		case LanguageElementType.EventRemove:
		  onSingleLine = options.LineBreaks.PlaceSimpleAccessorOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceCloseBraceOnNewLineForCodeBlocks && nodeCount > 0;
		  space = options.Spacing.WithinSingleLineAccessor && onSingleLine;
		  break;
		case LanguageElementType.Method:
		  onSingleLine = options.LineBreaks.PlaceSimpleMemberOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  space = options.Spacing.WithinSingleLineMethod && onSingleLine;
		  lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForMethods && !onSingleLine;
		  break;
		case LanguageElementType.Namespace:
		  lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForNamespaces;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Interface:
		case LanguageElementType.Struct:
		case LanguageElementType.Enum:
		  lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForTypeDeclarations;
		  break;
		case LanguageElementType.Block:
		  LanguageElement parent = context.Parent;
		  if (parent != null && parent.NodeCount == 1 && parent.ElementType == LanguageElementType.Case)
			lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement && nodeCount > 0;
		  break;
		case LanguageElementType.Event:
		  lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForProperties;
		  break;
		case LanguageElementType.LambdaExpression:
		  onSingleLine = options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  indent = options.Indention.AnonymousMethodContents;
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceCloseBraceOnNewLineForLambdaExpressions && nodeCount > 0;
		  break;
		case LanguageElementType.AnonymousMethodExpression:
		  onSingleLine = options.LineBreaks.PlaceSimpleAnonymousMethodOnSingleLine && IsSimpleElement(context, codeGen.Options);
		  lineBreak = !onSingleLine && options.LineBreaks.PlaceCloseBraceOnNewLineForAnonymousMethods && nodeCount > 0;
		  space = onSingleLine && options.Spacing.WithinSingleLineAnonymousMethod && nodeCount < 2;
		  indent = options.Indention.AnonymousMethodContents;
		  break;
		case LanguageElementType.ArrayInitializerExpression:
		case LanguageElementType.ObjectInitializerExpression:
		  ObjectCreationExpression objCreation = context.Parent as ObjectCreationExpression;
		  if(objCreation != null && objCreation.ObjectType == null)
			lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForAnonymousTypes;
		  else
		  {
			int detailNodeCount = GetNodeCount(context.DetailNodes);
			lineBreak = options.LineBreaks.PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers && detailNodeCount > 0;
		  }
		  space = options.Spacing.WithinArrayInitializerBraces;
		  indent = false;
		  break;
		case LanguageElementType.Switch:
		  indent = options.Indention.CaseStatementFromSwitchStatement;
		  break;
	  }
	  if (lineBreak)
		tokens.AddNewLine();
	  else
		if (space)
		  tokens.AddWhiteSpace();
	  if (indent)
		tokens.AddDecreaseIndent();
	  if (options.Indention.OpenAndCloseBraces)
		tokens.AddIncreaseIndent();
	}
	#endregion
	#region AddTokensAfterClose
	public static void AddTokensAfterClose(FormattingElements tokens, CodeGen codeGen)
	{
	  if (codeGen.Options.Indention.OpenAndCloseBraces)
		tokens.AddDecreaseIndent();
	  LanguageElement context = codeGen.Context;
	  CodeGenOptions options = codeGen.Options;
	  bool lineBreak = false;
	  switch (context.ElementType)
	  {
		case LanguageElementType.Namespace:
		  lineBreak = options.BlankLines.AfterNamespaces && context.NextCodeSibling != null;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Interface:
		case LanguageElementType.Struct:
		case LanguageElementType.Enum:
		  lineBreak = options.BlankLines.AfterTypeDeclarations && context.NextSibling != null;
		  break;
		case LanguageElementType.EventAdd:
		case LanguageElementType.EventRemove:
		case LanguageElementType.EventRaise:
		case LanguageElementType.PropertyAccessorGet:
		case LanguageElementType.PropertyAccessorSet:
		  if (context.NextCodeSibling != null)
			tokens.AddNewLine();
		  break;
	  }
	  if (lineBreak)
		tokens.AddNewLine();
	}
	#endregion
  }
}
