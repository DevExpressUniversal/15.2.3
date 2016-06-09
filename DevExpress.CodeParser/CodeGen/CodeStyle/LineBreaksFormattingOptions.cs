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
  public class LineBreaksFormattingOptions : FormattingOptions
  {
	bool _PlaceTypeAttributeOnSeparateLine;
	bool _PlaceMethodAttributeOnSeparateLine;
	bool _PlacePropertyAttributeOnSeparateLine;
	bool _PlaceEventAttributeOnSeparateLine;
	bool _PlaceFieldConstantAttributeOnSeparateLine;
	bool _PlaceEnumElementAttributeOnSeparateLine;
	bool _PlaceMultipleAttributesOnTheirOwnLine;
	bool _PlaceOpenBraceOnNewLineForNamespaces;
	bool _PlaceOpenBraceOnNewLineForTypeDeclarations;
	bool _PlaceOpenBraceOnNewLineForMethods;
	bool _PlaceOpenBraceOnNewLineForAnonymousMethods;
	bool _PlaceOpenBraceOnNewLineForCodeBlocks;
	bool _PlaceOpenBraceOnNewLineForAnonymousTypes;
	bool _PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers;
	bool _PlaceOpenBraceOnNewLineForLambdaExpressions;
	bool _PlaceOpenBraceOnNewLineForProperties;
	bool _PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement;
	bool _PlaceCloseBraceOnNewLineForNamespaces;
	bool _PlaceCloseBraceOnNewLineForTypeDeclarations;
	bool _PlaceCloseBraceOnNewLineForMethods;
	bool _PlaceCloseBraceOnNewLineForAnonymousMethods;
	bool _PlaceCloseBraceOnNewLineForCodeBlocks;
	bool _PlaceCloseBraceOnNewLineForAnonymousTypes;
	bool _PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers;
	bool _PlaceCloseBraceOnNewLineForLambdaExpressions;
	bool _PlaceCloseBraceOnNewLineForProperties;
	bool _PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement;
	bool _PlaceSimpleEmbeddedStatementOnSingleLine;
	bool _PlaceElseStatementOnNewLine;
	bool _PlaceWhileStatementOnNewLine;
	bool _PlaceCatchStatementOnNewLine;
	bool _PlaceFinallyStatementOnNewLine;
	bool _PlaceIfStatementFollowedByElseOnNewLine;
	bool _PlaceAbstractInterfaceMemberOnSingleLine;
	bool _PlaceAutoImplementedPropertyOnSingleLine;
	bool _PlaceSimpleMemberOnSingleLine;
	bool _PlaceSimpleAccessorOnSingleLine;
	bool _PlaceSimpleAnonymousMethodOnSingleLine;
	bool _PlaceConstructorInitializerOnSameLine;
	public override void Load(FormattingRuleCollection rules)
	{
	  _PlaceTypeAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceTypeAttributeOnSeparateLine, rules);
	  _PlaceMethodAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceMethodAttributeOnSeparateLine, rules);
	  _PlacePropertyAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlacePropertyAttributeOnSeparateLine, rules);
	  _PlaceEventAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceEventAttributeOnSeparateLine, rules);
	  _PlaceFieldConstantAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceFieldConstantAttributeOnSeparateLine, rules);
	  _PlaceEnumElementAttributeOnSeparateLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceEnumElementAttributeOnSeparateLine, rules);
	  _PlaceMultipleAttributesOnTheirOwnLine = GetOptionByName(Rules.LineBreaks.Attributes.PlaceMultipleAttributesOnTheirOwnLine, rules);
	  _PlaceOpenBraceOnNewLineForNamespaces = GetOptionByName(Rules.LineBreaks.OpenBrace.Namespaces, rules);
	  _PlaceOpenBraceOnNewLineForTypeDeclarations = GetOptionByName(Rules.LineBreaks.OpenBrace.TypeDeclarations, rules);
	  _PlaceOpenBraceOnNewLineForMethods = GetOptionByName(Rules.LineBreaks.OpenBrace.Methods, rules);
	  _PlaceOpenBraceOnNewLineForAnonymousMethods = GetOptionByName(Rules.LineBreaks.OpenBrace.AnonymousMethods, rules);
	  _PlaceOpenBraceOnNewLineForCodeBlocks = GetOptionByName(Rules.LineBreaks.OpenBrace.CodeBlocks, rules);
	  _PlaceOpenBraceOnNewLineForAnonymousTypes = GetOptionByName(Rules.LineBreaks.OpenBrace.AnonymousTypes, rules);
	  _PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers = GetOptionByName(Rules.LineBreaks.OpenBrace.ArrayObjectAndCollInitializers, rules);
	  _PlaceOpenBraceOnNewLineForLambdaExpressions = GetOptionByName(Rules.LineBreaks.OpenBrace.LambdaExpressions, rules);
	  _PlaceOpenBraceOnNewLineForProperties = GetOptionByName(Rules.LineBreaks.OpenBrace.Properties, rules);
	  _PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement = GetOptionByName(Rules.LineBreaks.OpenBrace.BlocksUnderCaseStatement, rules);
	  _PlaceCloseBraceOnNewLineForNamespaces = GetOptionByName(Rules.LineBreaks.CloseBrace.Namespaces, rules);
	  _PlaceCloseBraceOnNewLineForTypeDeclarations = GetOptionByName(Rules.LineBreaks.CloseBrace.TypeDeclarations, rules);
	  _PlaceCloseBraceOnNewLineForMethods = GetOptionByName(Rules.LineBreaks.CloseBrace.Methods, rules);
	  _PlaceCloseBraceOnNewLineForAnonymousMethods = GetOptionByName(Rules.LineBreaks.CloseBrace.AnonymousMethods, rules);
	  _PlaceCloseBraceOnNewLineForCodeBlocks = GetOptionByName(Rules.LineBreaks.CloseBrace.CodeBlocks, rules);
	  _PlaceCloseBraceOnNewLineForAnonymousTypes = GetOptionByName(Rules.LineBreaks.CloseBrace.AnonymousTypes, rules);
	  _PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers = GetOptionByName(Rules.LineBreaks.CloseBrace.ArrayObjectAndCollInitializers, rules);
	  _PlaceCloseBraceOnNewLineForLambdaExpressions = GetOptionByName(Rules.LineBreaks.CloseBrace.LambdaExpressions, rules);
	  _PlaceCloseBraceOnNewLineForProperties = GetOptionByName(Rules.LineBreaks.CloseBrace.Properties, rules);
	  _PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement = GetOptionByName(Rules.LineBreaks.CloseBrace.BlocksUnderCaseStatement, rules);
	  _PlaceSimpleEmbeddedStatementOnSingleLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceSimpleEmbeddedStatementOnSingleLine, rules);
	  _PlaceElseStatementOnNewLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceElseStatementOnNewLine, rules);
	  _PlaceWhileStatementOnNewLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceWhileStatementOnNewLine, rules);
	  _PlaceCatchStatementOnNewLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceCatchStatementOnNewLine, rules);
	  _PlaceFinallyStatementOnNewLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceFinallyStatementOnNewLine, rules);
	  _PlaceIfStatementFollowedByElseOnNewLine = GetOptionByName(Rules.LineBreaks.Statements.PlaceIfStatementFollowedByElseOnNewLine, rules);
	  _PlaceAbstractInterfaceMemberOnSingleLine = GetOptionByName(Rules.LineBreaks.Members.PlaceAbstractInterfaceMemberOnSingleLine, rules);
	  _PlaceAutoImplementedPropertyOnSingleLine = GetOptionByName(Rules.LineBreaks.Members.PlaceAutoImplementedPropertyOnSingleLine, rules);
	  _PlaceSimpleMemberOnSingleLine = GetOptionByName(Rules.LineBreaks.Members.PlaceSimpleMemberOnSingleLine, rules);
	  _PlaceSimpleAccessorOnSingleLine = GetOptionByName(Rules.LineBreaks.Members.PlaceSimpleAccessorOnSingleLine, rules);
	  _PlaceSimpleAnonymousMethodOnSingleLine = GetOptionByName(Rules.LineBreaks.Members.PlaceSimpleAnonymousMethodOnSingleLine, rules);
	  _PlaceConstructorInitializerOnSameLine = GetOptionByName(Rules.LineBreaks.Members.PlaceConstructorInitializerOnSameLine, rules);
	}
	public override void LoadDefault(ParserLanguageID language)
	{
	  _PlaceTypeAttributeOnSeparateLine = true;
	  _PlaceMethodAttributeOnSeparateLine = true;
	  _PlacePropertyAttributeOnSeparateLine = true;
	  _PlaceFieldConstantAttributeOnSeparateLine = true;
	  _PlaceEventAttributeOnSeparateLine = true;
	  _PlaceEnumElementAttributeOnSeparateLine = true;
	  _PlaceAbstractInterfaceMemberOnSingleLine = true;
	  _PlaceAutoImplementedPropertyOnSingleLine = true;
	  _PlaceCatchStatementOnNewLine = true;
	  _PlaceFinallyStatementOnNewLine = true;
	  _PlaceOpenBraceOnNewLineForNamespaces = true;
	  _PlaceOpenBraceOnNewLineForAnonymousMethods = true;
	  _PlaceOpenBraceOnNewLineForLambdaExpressions = true;
	  _PlaceOpenBraceOnNewLineForTypeDeclarations = true;
	  _PlaceOpenBraceOnNewLineForProperties = true;
	  _PlaceOpenBraceOnNewLineForMethods = true;
	  _PlaceOpenBraceOnNewLineForCodeBlocks = true;
	  _PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement = true;
	  _PlaceElseStatementOnNewLine = true;
	  _PlaceWhileStatementOnNewLine = true;
	  _PlaceCloseBraceOnNewLineForTypeDeclarations = true;
	  _PlaceCloseBraceOnNewLineForLambdaExpressions = true;
	  _PlaceCloseBraceOnNewLineForAnonymousMethods = true;
	  _PlaceCloseBraceOnNewLineForMethods = true;
	  _PlaceCloseBraceOnNewLineForNamespaces = true;
	  _PlaceCloseBraceOnNewLineForProperties = true;
	  _PlaceCloseBraceOnNewLineForCodeBlocks = true;
	  _PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement = true;
	  _PlaceMultipleAttributesOnTheirOwnLine = false;
	}
	public bool PlaceTypeAttributeOnSeparateLine { get { return _PlaceTypeAttributeOnSeparateLine; } }
	public bool PlaceMethodAttributeOnSeparateLine { get { return _PlaceMethodAttributeOnSeparateLine; } }
	public bool PlacePropertyAttributeOnSeparateLine { get { return _PlacePropertyAttributeOnSeparateLine; } }
	public bool PlaceEventAttributeOnSeparateLine { get { return _PlaceEventAttributeOnSeparateLine; } }
	public bool PlaceFieldConstantAttributeOnSeparateLine { get { return _PlaceFieldConstantAttributeOnSeparateLine; } }
	public bool PlaceEnumElementAttributeOnSeparateLine { get { return _PlaceEnumElementAttributeOnSeparateLine; } }
	public bool PlaceMultipleAttributesOnTheirOwnLine { get { return _PlaceMultipleAttributesOnTheirOwnLine; } }
	public bool PlaceOpenBraceOnNewLineForNamespaces { get { return _PlaceOpenBraceOnNewLineForNamespaces; } }
	public bool PlaceOpenBraceOnNewLineForTypeDeclarations { get { return _PlaceOpenBraceOnNewLineForTypeDeclarations; } }
	public bool PlaceOpenBraceOnNewLineForMethods { get { return _PlaceOpenBraceOnNewLineForMethods; } }
	public bool PlaceOpenBraceOnNewLineForAnonymousMethods { get { return _PlaceOpenBraceOnNewLineForAnonymousMethods; } }
	public bool PlaceOpenBraceOnNewLineForCodeBlocks { get { return _PlaceOpenBraceOnNewLineForCodeBlocks; } }
	public bool PlaceOpenBraceOnNewLineForAnonymousTypes { get { return _PlaceOpenBraceOnNewLineForAnonymousTypes; } }
	public bool PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers { get { return _PlaceOpenBraceOnNewLineForArrayObjectAndCollInitializers; } }
	public bool PlaceOpenBraceOnNewLineForLambdaExpressions { get { return _PlaceOpenBraceOnNewLineForLambdaExpressions; } }
	public bool PlaceOpenBraceOnNewLineForProperties { get { return _PlaceOpenBraceOnNewLineForProperties; } }
	public bool PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement { get { return _PlaceOpenBraceOnNewLineForBlocksUnderCaseStatement; } }
	public bool PlaceSimpleEmbeddedStatementOnSingleLine { get { return _PlaceSimpleEmbeddedStatementOnSingleLine; } }
	public bool PlaceElseStatementOnNewLine { get { return _PlaceElseStatementOnNewLine; } }
	public bool PlaceWhileStatementOnNewLine { get { return _PlaceWhileStatementOnNewLine; } }
	public bool PlaceCatchStatementOnNewLine { get { return _PlaceCatchStatementOnNewLine; } }
	public bool PlaceFinallyStatementOnNewLine { get { return _PlaceFinallyStatementOnNewLine; } }
	public bool PlaceIfStatementFollowedByElseOnNewLine { get { return _PlaceIfStatementFollowedByElseOnNewLine; } }
	public bool PlaceAbstractInterfaceMemberOnSingleLine { get { return _PlaceAbstractInterfaceMemberOnSingleLine; } }
	public bool PlaceAutoImplementedPropertyOnSingleLine { get { return _PlaceAutoImplementedPropertyOnSingleLine; } }
	public bool PlaceSimpleMemberOnSingleLine { get { return _PlaceSimpleMemberOnSingleLine; } }
	public bool PlaceSimpleAccessorOnSingleLine { get { return _PlaceSimpleAccessorOnSingleLine; } }
	public bool PlaceSimpleAnonymousMethodOnSingleLine { get { return _PlaceSimpleAnonymousMethodOnSingleLine; } }
	public bool PlaceConstructorInitializerOnSameLine { get { return _PlaceConstructorInitializerOnSameLine; } }
	public bool PlaceCloseBraceOnNewLineForNamespaces { get { return _PlaceCloseBraceOnNewLineForNamespaces; } }
	public bool PlaceCloseBraceOnNewLineForTypeDeclarations { get { return _PlaceCloseBraceOnNewLineForTypeDeclarations; } }
	public bool PlaceCloseBraceOnNewLineForMethods { get { return _PlaceCloseBraceOnNewLineForMethods; } }
	public bool PlaceCloseBraceOnNewLineForAnonymousMethods { get { return _PlaceCloseBraceOnNewLineForAnonymousMethods; } }
	public bool PlaceCloseBraceOnNewLineForCodeBlocks { get { return _PlaceCloseBraceOnNewLineForCodeBlocks; } }
	public bool PlaceCloseBraceOnNewLineForAnonymousTypes { get { return _PlaceCloseBraceOnNewLineForAnonymousTypes; } }
	public bool PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers { get { return _PlaceCloseBraceOnNewLineForArrayObjectAndCollInitializers; } }
	public bool PlaceCloseBraceOnNewLineForLambdaExpressions { get { return _PlaceCloseBraceOnNewLineForLambdaExpressions; } }
	public bool PlaceCloseBraceOnNewLineForProperties { get { return _PlaceCloseBraceOnNewLineForProperties; } }
	public bool PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement { get { return _PlaceCloseBraceOnNewLineForBlocksUnderCaseStatement; } }
  }
}
