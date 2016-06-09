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
  public class CSharpSupportElementCodeGen : TokenSupportElementCodeGenBase
	{
		public CSharpSupportElementCodeGen(CodeGen codeGen)
			: base(codeGen) 
		{
		}
		protected override void GenerateCppAttribute(CppAttributeSection element)
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
	protected override bool HasSameLineRange(Comment com, SupportElementPosition possiblePosition)
	{
	  if (com == null || com.Position != possiblePosition)
		return false;
	  CodeElement element = com.TargetNode as CodeElement;
	  if (element == null)
		return false;
	  return HasSameLineRange(element, com);
	}
	protected override void GenerateElementComment(CodeElement codeElement, SupportElementPosition position)
	{
	  if (codeElement == null || codeElement.Comments == null || codeElement.CommentCount == 0)
		return;
	  Comment comment = null;
	  CommentCollection coll = codeElement.Comments;
	  int commentsCount = codeElement.CommentCount;
	  for (int i = 0; i < commentsCount; i++)
	  {
		comment = coll[i];
		if (CodeGen.TokenArgsContainsElement(comment))
		  continue;
		if (comment.Position != position ||
			comment.TargetNode != codeElement ||
			CodeGen.IsSkiped(comment))
		  continue;
		if (HasSameLineRange(comment, SupportElementPosition.After))
		  CodeGen.AddWSIfNeeded();
		CodeGen.GenerateElement(comment);
		if (HasSameLineRange(comment, SupportElementPosition.Before))
		  CodeGen.AddWSIfNeeded();
		CodeGen.AddSkiped(comment);
	  }
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (element == null)
		return false;
	  Comment com = element as Comment;
	  if (com != null && com.CommentType == CommentType.SingleLine && !CodeGen.GeneratingInSupportElement)
	  {
		CodeGen.AddNewLineIfNeeded();
		return true;
	  }
	  return false;
	}
	}
}
