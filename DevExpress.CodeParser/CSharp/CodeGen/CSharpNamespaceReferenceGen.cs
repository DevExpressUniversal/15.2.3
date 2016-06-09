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
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public class CSharpNamespaceReferenceGen : NamespaceReferenceGenBase 
	{
		public CSharpNamespaceReferenceGen(CodeGen codeGen) : base(codeGen) 
		{
		}
	# region  GenerateCode
	public override void GenerateCode(CodeWriter writer, LanguageElement languageElement, bool calculateIndent)
	{
	  if (writer == null)
		throw new ArgumentNullException("writer");
	  if (CodeGen == null)
		throw new ArgumentNullException("CodeGen is Null");
	  PushCodeWriter();
	  SetCodeWriter(writer);
	  if (calculateIndent)
		CalculateIndent(languageElement);
	  CodeGen.GenerateElement(languageElement);
	  if (calculateIndent)
		ResetIndent();
	  PopCodeWriter();
	}
	#endregion
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Extern:
		case FormattingTokenType.Alias:
		case FormattingTokenType.Using:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.Semicolon:
		  NamespaceReference nr = Context as NamespaceReference;
		  if (nr == null || nr.NextSibling == null)
			break;
		  if (!Options.BlankLines.AfterImportedNamespacesSection || nr.NextSibling is NamespaceReference)
			result.AddNewLine();
		  else
			result.AddNewLine(2);
		  break;
	  }
	  return result;
	}
		protected override void GenerateExternAlias(ExternAlias element)
		{
			Write(FormattingTokenType.Extern);
			Write(FormattingTokenType.Alias);
			Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Semicolon);
		}
		protected override void GenerateNamespaceReference(NamespaceReference element) 
		{
			Write(FormattingTokenType.Using);
	  LanguageElement expression = element.FirstDetail;
			if (element.IsAlias) 
			{
		if (element.AliasExpression != null)
		  CodeGen.GenerateElement(element.AliasExpression);
		else
		  Write(element.AliasName);
		Write(FormattingTokenType.Equal);
		if (element.DetailNodeCount > 1)
		  expression = element.DetailNodes[1] as LanguageElement;
	  }
	  if (expression != null)
		CodeGen.GenerateElement(expression);
	  else
		Write(FormattingTokenType.Ident);
	  Write(FormattingTokenType.Semicolon);
		}
	}
}
