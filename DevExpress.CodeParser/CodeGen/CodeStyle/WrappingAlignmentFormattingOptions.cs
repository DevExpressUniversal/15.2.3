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
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  public class WrappingAlignmentFormattingOptions : FormattingOptions
  {
	bool _WrapLongLines;
	int _MaxLineLength;
	bool _RightMarginColumnSize;
	bool _WrapFormalParameters;
	bool _WrapFirstFormalParameter;
	bool _AlignWithFirstFormalParameter;
	bool _WrapBeforeOpenBraceInDeclaration;
	bool _WrapBeforeCloseBraceInDeclaration;
	bool _WrapInvocationArguments;
	bool _WrapFirstInvocationArgument;
	bool _AlignWithFirstInvocationArgument;
	bool _WrapBeforeOpenBraceInInvocation;
	bool _WrapBeforeCloseBraceInInvocation;
	bool _WrapTernaryExpression;
	bool _WrapLogicalOperatorExpression;
	bool _WrapBinaryOperatorExpression;
	bool _WrapBeforeOperatorInLogicalExpression;
	bool _WrapBeforeOperatorInBinaryExpression;
	bool _AlignWithFirstBinaryExpressionItem;
	bool _AlignWithFirstLogicalExpressionItem;
	bool _WrapQueryExpression;
	bool _AlignWithFirstQueryExpressionItem;
	bool _WrapTypeParameters;
	bool _WrapFirstTypeParameter;
	bool _AlignWithFirstTypeParameterConstraint;
	bool _WrapTypeParameterConstraints;
	bool _WrapFirstTypeParameterConstraint;
	bool _AlignWithFirstTypeParameter;
	bool _WrapImplementsHandlesSectionItems;
	bool _WrapFirstImplementsHandlesSectionItem;
	bool _AlignWithFirstImplementsHandlesSectionItem;
	bool _WrapArrayObjectAndCollectionInitializers;
	bool _WrapFirstArrayObjectAndCollectionInitializer;
	bool _AlignWithFirstArrayObjectOrCollectionInitializerItem;
	bool _WrapMembersInAnonymousType;
	bool _WrapFirstMemberInAnonymousType;
	bool _AlignWithFirstMemberInAnonymousType;
	bool _WrapFirstMultiVariableDeclaration;
	bool _WrapMultiVariableDeclaration;
	bool _AlignWithFirstMultiVariableDeclarationItem;
	bool _WrapForStatementHeader;
	bool _WrapAncestorsListInTypeDeclaration;
	bool _WrapFirstAncestorsListItem;
	bool _AlignWithFirstAncestorsListItem;
	bool _WrapBeforeColonForAncestorsList;
	public override void Load(FormattingRuleCollection rules)
	{
	  BaseFormattingRule wrapLongLinesRule = rules[Rules.WrappingAlignment.WrapLongLines];
	  if (wrapLongLinesRule != null)
		_WrapLongLines = wrapLongLinesRule.Enabled;
	  BaseFormattingRule rightMarginColumnSize = rules[Rules.WrappingAlignment.RightMarginColumnSize];
	  if (_WrapLongLines && rightMarginColumnSize != null)
		_MaxLineLength = rightMarginColumnSize.IntValue;
	  _RightMarginColumnSize = GetOptionByName(Rules.WrappingAlignment.RightMarginColumnSize, rules);
	  _WrapFormalParameters = GetOptionByName(Rules.WrappingAlignment.WrapFormalParameters, rules);
	  _WrapFirstFormalParameter = GetOptionByName(Rules.WrappingAlignment.WrapFirstFormalParameter, rules);
	  _AlignWithFirstFormalParameter = GetOptionByName(Rules.WrappingAlignment.AlignWithFirstFormalParameter, rules);
	  _WrapBeforeOpenBraceInDeclaration = GetOptionByName(Rules.WrappingAlignment.WrapBeforeOpenBraceInDeclaration, rules);
	  _WrapBeforeCloseBraceInDeclaration = GetOptionByName(Rules.WrappingAlignment.WrapBeforeCloseBraceInDeclaration, rules);
	  _WrapInvocationArguments = GetOptionByName(Rules.WrappingAlignment.WrapInvocationArguments, rules);
	  _WrapFirstInvocationArgument = GetOptionByName(Rules.WrappingAlignment.WrapFirstInvocationArgument, rules);
	  _AlignWithFirstInvocationArgument = GetOptionByName(Rules.WrappingAlignment.AlignWithFirstInvocationArgument, rules);
	  _WrapBeforeOpenBraceInInvocation = GetOptionByName(Rules.WrappingAlignment.WrapBeforeOpenBraceInInvocation, rules);
	  _WrapBeforeCloseBraceInInvocation = GetOptionByName(Rules.WrappingAlignment.WrapBeforeCloseBraceInInvocation, rules);
	  _WrapTernaryExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapTernaryExpression, rules);
	  _WrapLogicalOperatorExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapLogicalOperatorExpression, rules);
	  _WrapBinaryOperatorExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapBinaryOperatorExpression, rules);
	  _WrapBeforeOperatorInLogicalExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapBeforeOperatorInLogicalExpression, rules);
	  _WrapBeforeOperatorInBinaryExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapBeforeOperatorInBinaryExpression, rules);
	  _AlignWithFirstBinaryExpressionItem = GetOptionByName(Rules.WrappingAlignment.Expressions.AlignWithFirstBinaryExpressionItem, rules);
	  _AlignWithFirstLogicalExpressionItem = GetOptionByName(Rules.WrappingAlignment.Expressions.AlignWithFirstLogicalExpressionItem, rules);
	  _WrapQueryExpression = GetOptionByName(Rules.WrappingAlignment.Expressions.WrapQueryExpression, rules);
	  _AlignWithFirstQueryExpressionItem = GetOptionByName(Rules.WrappingAlignment.Expressions.AlignWithFirstQueryExpressionItem, rules);
	  _WrapTypeParameters = GetOptionByName(Rules.WrappingAlignment.TypeParameters.WrapTypeParameters, rules);
	  _WrapFirstTypeParameter = GetOptionByName(Rules.WrappingAlignment.TypeParameters.WrapFirstTypeParameter, rules);
	  _WrapTypeParameterConstraints = GetOptionByName(Rules.WrappingAlignment.TypeParameters.Constraints.WrapTypeParameterConstraints, rules);
	  _WrapFirstTypeParameterConstraint = GetOptionByName(Rules.WrappingAlignment.TypeParameters.Constraints.WrapFirstTypeParameterConstraint, rules);
	  _AlignWithFirstTypeParameter = GetOptionByName(Rules.WrappingAlignment.TypeParameters.AlignWithFirstTypeParameter, rules);
	  _AlignWithFirstTypeParameterConstraint = GetOptionByName(Rules.WrappingAlignment.TypeParameters.Constraints.AlignWithFirstTypeParameterConstraint, rules);
	  _WrapImplementsHandlesSectionItems = GetOptionByName(Rules.WrappingAlignment.ImplementsHandles.WrapImplementsHandlesSectionItems, rules);
	  _WrapFirstImplementsHandlesSectionItem = GetOptionByName(Rules.WrappingAlignment.ImplementsHandles.WrapFirstImplementsHandlesSectionItem, rules);
	  _AlignWithFirstImplementsHandlesSectionItem = GetOptionByName(Rules.WrappingAlignment.ImplementsHandles.AlignWithFirstImplementsHandlesSectionItem, rules);
	  _WrapArrayObjectAndCollectionInitializers = GetOptionByName(Rules.WrappingAlignment.ArrayAndObjectInitializers.WrapArrayObjectAndCollectionInitializers, rules);
	  _WrapFirstArrayObjectAndCollectionInitializer = GetOptionByName(Rules.WrappingAlignment.ArrayAndObjectInitializers.WrapFirstArrayObjectAndCollectionInitializer, rules);
	  _AlignWithFirstArrayObjectOrCollectionInitializerItem = GetOptionByName(Rules.WrappingAlignment.ArrayAndObjectInitializers.AlignWithFirstArrayObjectOrCollectionInitializerItem, rules);
	  _WrapMembersInAnonymousType = GetOptionByName(Rules.WrappingAlignment.AnonymousTypes.WrapMembersInAnonymousType, rules);
	  _WrapFirstMemberInAnonymousType = GetOptionByName(Rules.WrappingAlignment.AnonymousTypes.WrapFirstMemberInAnonymousType, rules);
	  _AlignWithFirstMemberInAnonymousType = GetOptionByName(Rules.WrappingAlignment.AnonymousTypes.AlignWithFirstMemberInAnonymousType, rules);
	  _WrapFirstMultiVariableDeclaration = GetOptionByName(Rules.WrappingAlignment.MultiVariable.WrapFirstMultiVariableDeclaration, rules);
	  _WrapMultiVariableDeclaration = GetOptionByName(Rules.WrappingAlignment.MultiVariable.WrapMultiVariableDeclaration, rules);
	  _AlignWithFirstMultiVariableDeclarationItem = GetOptionByName(Rules.WrappingAlignment.MultiVariable.AlignWithFirstMultiVariableDeclarationItem, rules);
	  _WrapForStatementHeader = GetOptionByName(Rules.WrappingAlignment.WrapForStatementHeader, rules);
	  _WrapAncestorsListInTypeDeclaration = GetOptionByName(Rules.WrappingAlignment.TypeAncestors.WrapAncestorsListInTypeDeclaration, rules);
	  _WrapFirstAncestorsListItem = GetOptionByName(Rules.WrappingAlignment.TypeAncestors.WrapFirstAncestorsListItem, rules);
	  _AlignWithFirstAncestorsListItem = GetOptionByName(Rules.WrappingAlignment.TypeAncestors.AlignWithFirstAncestorsListItem, rules);
	  _WrapBeforeColonForAncestorsList = GetOptionByName(Rules.WrappingAlignment.TypeAncestors.WrapBeforeColonForAncestorsList, rules);
	}
	public override void LoadDefault(ParserLanguageID language)
	{
	  if(language != ParserLanguageID.Basic)
	  {
		_WrapFirstTypeParameterConstraint = true;
		_WrapTypeParameterConstraints = true;
	  }
	  else
	  {
		_WrapQueryExpression = true;
	  }
	}
	public bool WrapLongLines { get { return _WrapLongLines; } }
	public int MaxLineLength { get { return _MaxLineLength; } }
	public bool RightMarginColumnSize { get { return _RightMarginColumnSize; } }
	public bool WrapFormalParameters { get { return _WrapFormalParameters; } }
	public bool WrapFirstFormalParameter { get { return _WrapFirstFormalParameter; } }
	public bool AlignWithFirstFormalParameter { get { return _AlignWithFirstFormalParameter; } }
	public bool WrapBeforeOpenBraceInDeclaration { get { return _WrapBeforeOpenBraceInDeclaration; } }
	public bool WrapBeforeCloseBraceInDeclaration { get { return _WrapBeforeCloseBraceInDeclaration; } }
	public bool WrapInvocationArguments { get { return _WrapInvocationArguments; } }
	public bool WrapFirstInvocationArgument { get { return _WrapFirstInvocationArgument; } }
	public bool AlignWithFirstInvocationArgument { get { return _AlignWithFirstInvocationArgument; } }
	public bool WrapBeforeCloseBraceInInvocation { get { return _WrapBeforeCloseBraceInInvocation; } }
	public bool WrapBeforeOpenBraceInInvocation { get { return _WrapBeforeOpenBraceInInvocation; } }
	public bool WrapTernaryExpression { get { return _WrapTernaryExpression; } }
	public bool WrapLogicalOperatorExpression { get { return _WrapLogicalOperatorExpression; } }
	public bool WrapBinaryOperatorExpression { get { return _WrapBinaryOperatorExpression; } }
	public bool WrapBeforeOperatorInLogicalExpression { get { return _WrapBeforeOperatorInLogicalExpression; } }
	public bool WrapBeforeOperatorInBinaryExpression { get { return _WrapBeforeOperatorInBinaryExpression; } }
	public bool AlignWithFirstBinaryExpressionItem { get { return _AlignWithFirstBinaryExpressionItem; } }
	public bool AlignWithFirstLogicalExpressionItem { get { return _AlignWithFirstLogicalExpressionItem; } }
	public bool WrapQueryExpression { get { return _WrapQueryExpression; } }
	public bool AlignWithFirstQueryExpressionItem { get { return _AlignWithFirstQueryExpressionItem; } }
	public bool WrapTypeParameters { get { return _WrapTypeParameters; } }
	public bool WrapFirstTypeParameter { get { return _WrapFirstTypeParameter; } }
	public bool WrapTypeParameterConstraints { get { return _WrapTypeParameterConstraints; } }
	public bool WrapFirstTypeParameterConstraint { get { return _WrapFirstTypeParameterConstraint; } }
	public bool AlignWithFirstTypeParameter { get { return _AlignWithFirstTypeParameter; } }
	public bool AlignWithFirstTypeParameterConstraint { get { return _AlignWithFirstTypeParameterConstraint; } }
	public bool WrapImplementsHandlesSectionItems { get { return _WrapImplementsHandlesSectionItems; } }
	public bool WrapFirstImplementsHandlesSectionItem { get { return _WrapFirstImplementsHandlesSectionItem; } }
	public bool AlignWithFirstImplementsHandlesSectionItem { get { return _AlignWithFirstImplementsHandlesSectionItem; } }
	public bool WrapArrayObjectAndCollectionInitializers { get { return _WrapArrayObjectAndCollectionInitializers; } }
	public bool WrapFirstArrayObjectAndCollectionInitializer { get { return _WrapFirstArrayObjectAndCollectionInitializer; } }
	public bool AlignWithFirstArrayObjectOrCollectionInitializerItem { get { return _AlignWithFirstArrayObjectOrCollectionInitializerItem; } }
	public bool WrapMembersInAnonymousType { get { return _WrapMembersInAnonymousType; } }
	public bool WrapFirstMemberInAnonymousType { get { return _WrapFirstMemberInAnonymousType; } }
	public bool AlignWithFirstMemberInAnonymousType { get { return _AlignWithFirstMemberInAnonymousType; } }
	public bool WrapFirstMultiVariableDeclaration { get { return _WrapFirstMultiVariableDeclaration; } }
	public bool WrapMultiVariableDeclaration { get { return _WrapMultiVariableDeclaration; } }
	public bool AlignWithFirstMultiVariableDeclarationItem { get { return _AlignWithFirstMultiVariableDeclarationItem; } }
	public bool WrapForStatementHeader { get { return _WrapForStatementHeader; } }
	public bool WrapAncestorsListInTypeDeclaration { get { return _WrapAncestorsListInTypeDeclaration; } }
	public bool WrapFirstAncestorsListItem { get { return _WrapFirstAncestorsListItem; } }
	public bool AlignWithFirstAncestorsListItem { get { return _AlignWithFirstAncestorsListItem; } }
	public bool WrapBeforeColonForAncestorsList { get { return _WrapBeforeColonForAncestorsList; } }
  }
}
