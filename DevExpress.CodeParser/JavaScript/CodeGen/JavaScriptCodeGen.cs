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
namespace DevExpress.CodeRush.StructuralParser.JavaScript
#else
namespace DevExpress.CodeParser.JavaScript
#endif
{
	public class JavaScriptCodegen : CodeGen
	{
		public JavaScriptCodegen()
		{
		}
		public JavaScriptCodegen(CodeGenOptions options) : base(options)
		{
		}
	void AddCharOperatorsToFormattingElements(FormattingTokenType tokenType, FormattingElements result)
	{
	  if (result == null)
		return;
	  LanguageElementType contextType = Context.ElementType;
	  if (contextType == LanguageElementType.UnaryOperatorExpression
		|| contextType == LanguageElementType.UnaryIncrement
		|| contextType == LanguageElementType.UnaryDecrement)
		return;
	  switch (tokenType)
	  {
		case FormattingTokenType.Equal:
		case FormattingTokenType.BackSlashEqual:
		case FormattingTokenType.AmpersandEqual:
		case FormattingTokenType.VerticalBarEqual:
		case FormattingTokenType.MinusEqual:
		case FormattingTokenType.PercentEqual:
		case FormattingTokenType.PlusEqual:
		case FormattingTokenType.LessThanLessThanEqual:
		case FormattingTokenType.GreatThanGreatThanEqual:
		case FormattingTokenType.GreaterThenEqual:
		case FormattingTokenType.SlashEqual:
		case FormattingTokenType.AsteriskEqual:
		case FormattingTokenType.CaretEqual:
		case FormattingTokenType.GreatThanGreatThanGreatThanEqual:
		case FormattingTokenType.QuestionQuestion:
		case FormattingTokenType.EqualEqual:
		case FormattingTokenType.EqualEqualEqual:
		case FormattingTokenType.AmpersandAmpersand:
		case FormattingTokenType.VerticalBarVerticalBar:
		case FormattingTokenType.ExclamationEqual:
		case FormattingTokenType.LessThanEqual:
		case FormattingTokenType.GreatThanGreatThan:
		case FormattingTokenType.LessThanLessThan:
		case FormattingTokenType.Plus:
		case FormattingTokenType.Ampersand:
		case FormattingTokenType.VerticalBar:
		case FormattingTokenType.Caret:
		case FormattingTokenType.Slash:
		case FormattingTokenType.Percent:
		case FormattingTokenType.Asterisk:
		case FormattingTokenType.GreaterThen:
		case FormattingTokenType.LessThan:
		case FormattingTokenType.Minus:
		case FormattingTokenType.In:
		  result.AddWhiteSpace();
		  break;
	  }
	}
	bool ContextIsInitializerExpression()
	{
	  LanguageElementType elementType = Context.ElementType;
	  return elementType == LanguageElementType.MemberInitializerExpression ||
			 elementType == LanguageElementType.ObjectInitializerExpression;
	}
	bool ContextIsAnonymousMethodExpression()
	{
	  LanguageElementType elementType = Context.ElementType;
	  return elementType == LanguageElementType.AnonymousMethodExpression;
	}
		protected override DirectiveCodeGenBase CreateDirectiveGen()
		{
			return null;
		}
		protected override ExpressionCodeGenBase CreateExpressionGen()
		{
			return new JavaScriptExpressionCodeGen(this);
		}
		protected override MemberCodeGenBase CreateMemberGen()
		{
			return new JavaScriptMemberCodeGen(this);
		}
		protected override StatementCodeGenBase CreateStatementGen()
		{
			return new JavaScriptStatementCodeGen(this);
		}
		protected override SupportElementCodeGenBase CreateSupportElementGen()
		{
			return new JavaScriptSupportElementCodeGen(this);
		}
		protected override TypeDeclarationCodeGenBase CreateTypeDeclarationGen()
		{
			return null;
		}
		protected override XmlCodeGenBase CreateXmlGen()
		{
			return new JavaScriptXmlCodeGen (this);
		}
		protected override NamespaceReferenceGenBase CreateNamespaceReferenceGen()
		{
			return null;
		}
		protected override NamespaceGenBase CreateNamespaceGen()
		{
			return null;
		}
		protected override SnippetCodeGenBase CreateSnippetGen()
		{
			return new TokenCSnippetCodeGen(this);
		}
		protected override TemplateCodeGenBase CreateTemplateGen()
		{
			return null;
		}
		protected override TemplateParameterCodeGenBase CreateTemplateParameterGen()
		{
			return null;
		}
		protected override HtmlXmlCodeGenBase CreateHtmlXmlGen()
		{
			return null;
		}
	internal override FormattingTable FormattingTable
	{
	  get { return CSharpFormattingTable.Instance; }
	}
		public override void GenerateMemberVisibilitySpecifier(MemberVisibilitySpecifier specifier)
		{
		}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  AddCharOperatorsToFormattingElements(tokenType, result);
	  switch (tokenType)
	  {
		case FormattingTokenType.CurlyBraceOpen:
		  if (!ContextIsInitializerExpression())
			result.AddNewLine();
		  break;
				case FormattingTokenType.CurlyBraceClose:
					if (!ContextIsInitializerExpression() && !ContextIsAnonymousMethodExpression())
						result.AddNewLine();
					break;
	  }
	  return result;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Function:
		  result.AddWhiteSpace();
		  break;
		case FormattingTokenType.CurlyBraceOpen:
		  if (Context.NodeCount > 0 && !ContextIsInitializerExpression())
			result.AddNewLine();
		  break;
		}
	  AddCharOperatorsToFormattingElements(tokenType, result);
	  return result;
	}
	}
}
