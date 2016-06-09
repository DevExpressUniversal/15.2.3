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
  public class BlankLinesFormattingOptions : FormattingOptions
  {
	bool _AfterFileOptionsSection;
	bool _AfterImportedNamespacesSection;
	bool _AfterNamespaces;
	bool _AfterTypeDeclarations;
	bool _AfterSingleLineMembers;
	bool _AfterMultiLineMembers;
	bool _AroundRegionDirectives;
	bool _InsideRegionDirectives;
	bool _AfterGlobalAttributes;
	bool _BetweenDifferentImportedNamespacesGroups;
	bool _AfterProcessingInstructions;
	bool _MaximumBlankLinesBetweenTags;
	public override void Load(FormattingRuleCollection rules)
	{
	  _AfterFileOptionsSection = GetOptionByName(Rules.BlankLines.AfterFileOptionsSection, rules);
	  _AfterImportedNamespacesSection = GetOptionByName(Rules.BlankLines.AfterImportedNamespacesSection, rules);
	  _AfterNamespaces = GetOptionByName(Rules.BlankLines.AfterNamespaces, rules);
	  _AfterTypeDeclarations = GetOptionByName(Rules.BlankLines.AfterTypeDeclarations, rules);
	  _AfterSingleLineMembers = GetOptionByName(Rules.BlankLines.AfterSingleLineMembers, rules);
	  _AfterMultiLineMembers = GetOptionByName(Rules.BlankLines.AfterMultiLineMembers, rules);
	  _AroundRegionDirectives = GetOptionByName(Rules.BlankLines.AroundRegionDirectives, rules);
	  _InsideRegionDirectives = GetOptionByName(Rules.BlankLines.InsideRegionDirectives, rules);
	  _AfterGlobalAttributes = GetOptionByName(Rules.BlankLines.AfterGlobalAttributes, rules);
	  _BetweenDifferentImportedNamespacesGroups = GetOptionByName(Rules.BlankLines.BetweenDifferentImportedNamespacesGroups, rules);
	  _AfterProcessingInstructions = GetOptionByName(Rules.BlankLines.AfterProcessingInstructions, rules);
	  _MaximumBlankLinesBetweenTags = GetOptionByName(Rules.BlankLines.MaximumBlankLinesBetweenTags, rules);
	}
	public override void LoadDefault(ParserLanguageID language)
	{
	}
	public bool AfterFileOptionsSection
	{
	  get
	  {
		return _AfterFileOptionsSection;
	  }
	}
	public bool AfterImportedNamespacesSection
	{
	  get
	  {
		return _AfterImportedNamespacesSection;
	  }
	}
	public bool AfterNamespaces
	{
	  get
	  {
		return _AfterNamespaces;
	  }
	}
	public bool AfterTypeDeclarations
	{
	  get
	  {
		return _AfterTypeDeclarations;
	  }
	}
	public bool AfterSingleLineMembers
	{
	  get
	  {
		return _AfterSingleLineMembers;
	  }
	}
	public bool AfterMultiLineMembers
	{
	  get
	  {
		return _AfterMultiLineMembers;
	  }
	}
	public bool AroundRegionDirectives
	{
	  get
	  {
		return _AroundRegionDirectives;
	  }
	}
	public bool InsideRegionDirectives
	{
	  get
	  {
		return _InsideRegionDirectives;
	  }
	}
	public bool AfterGlobalAttributes
	{
	  get
	  {
		return _AfterGlobalAttributes;
	  }
	}
	public bool BetweenDifferentImportedNamespacesGroups
	{
	  get
	  {
		return _BetweenDifferentImportedNamespacesGroups;
	  }
	}
	public bool AfterProcessingInstructions
	{
	  get
	  {
		return _AfterProcessingInstructions;
	  }
	}
	public bool MaximumBlankLinesBetweenTags
	{
	  get
	  {
		return _MaximumBlankLinesBetweenTags;
	  }
	}
  }
}
