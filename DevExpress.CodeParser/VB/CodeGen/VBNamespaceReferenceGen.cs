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
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  public class VBNamespaceReferenceGen : NamespaceReferenceGenBase
  {
	public VBNamespaceReferenceGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected override void GenerateXmlNamespaceReference(XmlNamespaceReference element)
	{
	  Write("Imports <xmlns");
	  string name = element.Name;
	  if (name != null && name != String.Empty)
		Write(FormattingTokenType.Colon, FormattingTokenType.Ident);
	  Write(FormattingTokenType.Equal);
	  Write(element.NamespaceName);
	  Write(FormattingTokenType.GreaterThen);
	}
	protected override void GenerateExternAlias(ExternAlias element)
	{
	}
	protected override void GenerateNamespaceReference(NamespaceReference element)
	{
	  Write(FormattingTokenType.Imports);
	  CodeGen.AddWSIfNeeded();
	  if (element.IsAlias)
	  {
		Write(element.AliasName);
		Write(FormattingTokenType.Equal);
	  }
	  Expression exp = null;
	  for (int i = element.DetailNodeCount - 1; i >= 0; i--)
	  {
		exp = element.DetailNodes[i] as Expression;
		if (exp != null)
		  break;
	  }
	  if (exp != null)
		CodeGen.GenerateElement(exp);
	  else
		Write(FormattingTokenType.Ident);
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Equal:
		  if (!ContextMatch(LanguageElementType.XmlNamespaceReference) && Options.Spacing.AroundEqualsInNamespaceAliasDeclaration)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Equal:
		  if (!ContextMatch(LanguageElementType.XmlNamespaceReference) && Options.Spacing.AroundEqualsInNamespaceAliasDeclaration)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (Options.BlankLines.AfterImportedNamespacesSection)
	  {
		NamespaceReference reference = element as NamespaceReference;
		if (reference != null)
		{
		  if (!(reference.NextCodeSibling is NamespaceReference))
			CodeGen.AddNewLine(2);
		  return true;
		}
	  }
	  return false;
	}
  }
}
