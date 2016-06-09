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
	public class VBSnippetCodeGen : SnippetCodeGenBase
	{
		public VBSnippetCodeGen(CodeGen codeGen) : base(codeGen)
		{
		}
		protected override void GenerateSnippetCodeElement(SnippetCodeElement snippet)
		{
			Write(snippet.Code);
		}
		protected override void GenerateSnippetCodeMember(SnippetCodeMember snippet)
		{
			Write(snippet.Code);
	  CodeGen.AddNewLineIfNeeded();
		}
		protected override void GenerateSnippetCodeStatement(SnippetCodeStatement snippet)
		{
			if (snippet == null)
				return;
			string code = snippet.Code;
			if (code == null || code.Trim() == String.Empty)
				return;
			Write(snippet.Code);
	  CodeGen.AddNewLineIfNeeded();
		}
		protected override void GenerateSnippetCodeStatementBlock(SnippetCodeStatementBlock snippet)
		{
	  Write(snippet.Code);
	  CodeGen.AddNewLineIfNeeded();
		}
		protected override void GenerateSnippetExpression(SnippetExpression snippet)
		{
			Write(snippet.Code);
		}
	}
}
