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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public abstract class SnippetCodeGenBase : LanguageElementCodeGenBase
	{
		public SnippetCodeGenBase(CodeGen codeGen) : base (codeGen)
		{
		}
		public override void GenerateElement(LanguageElement languageElement)
		{
			GenerateSnippet(languageElement);
		}
		protected virtual bool GenerateSnippet(LanguageElement snippet)
		{
			if (snippet == null)
				return false;
			switch (snippet.ElementType)
			{
				case LanguageElementType.SnippetCodeElement:
					GenerateSnippetCodeElement(snippet as SnippetCodeElement);
					return true;
				case LanguageElementType.SnippetCodeMember:
					GenerateSnippetCodeMember(snippet as SnippetCodeMember);
					return true;
				case LanguageElementType.SnippetCodeStatement:
					GenerateSnippetCodeStatement(snippet as SnippetCodeStatement);
					return true;
				case LanguageElementType.SnippetCodeStatementBlock:
					GenerateSnippetCodeStatementBlock(snippet as SnippetCodeStatementBlock);
					return true;
				case LanguageElementType.SnippetExpression:
					GenerateSnippetExpression(snippet as SnippetExpression);
					return true;
			}
			return false;
		}
		protected abstract void GenerateSnippetCodeElement(SnippetCodeElement snippet);
		protected abstract void GenerateSnippetCodeMember(SnippetCodeMember snippet);
		protected abstract void GenerateSnippetCodeStatement(SnippetCodeStatement snippet);
		protected abstract void GenerateSnippetCodeStatementBlock(SnippetCodeStatementBlock snippet);
		protected abstract void GenerateSnippetExpression(SnippetExpression snippet);
	}
}
