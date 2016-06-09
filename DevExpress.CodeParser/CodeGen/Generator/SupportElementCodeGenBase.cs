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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class SupportElementCodeGenBase : LanguageElementCodeGenBase
  {
	public SupportElementCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected virtual bool GenerateSupportElement(SupportElement element)
	{
	  if (element == null)
		return false;
	  switch (element.ElementType)
	  {
		case LanguageElementType.Attribute:
		  GenerateAttribute(element as Attribute);
		  return true;
		case LanguageElementType.AttributeSection:
		  GenerateAttribute(element as AttributeSection);
		  return true;
		case LanguageElementType.CppAttributeSection:
		  GenerateCppAttribute(element as CppAttributeSection);
		  return true;
		case LanguageElementType.Comment:
		  GenerateComment(element as Comment);
		  return true;
	  }
	  return false;
	}
	protected abstract void GenerateAttribute(Attribute element);
	protected abstract void GenerateAttribute(AttributeSection element);
	protected abstract void GenerateComment(Comment element);
	protected abstract void GenerateCppAttribute(CppAttributeSection element);
	protected virtual void GenerateElementComment(CodeElement codeElement, SupportElementPosition position)
	{
	  if (codeElement == null || codeElement.Comments == null || codeElement.CommentCount == 0)
		return;
	  Comment comment = null;
	  CommentCollection coll = codeElement.Comments;
	  int commentsCount = codeElement.CommentCount;
	  for (int i = 0; i < commentsCount; i++)
	  {
		comment = coll[i];
		if (comment.Position != position ||
			comment.TargetNode != codeElement ||
			CodeGen.IsSkiped(comment))
		  continue;
		if (HasSameLineRange(comment))
		  Code.Write(" ");
		CodeGen.GenerateElement(comment);
		if (comment.CommentType == CommentType.SingleLine && position == SupportElementPosition.Before)
		  Code.WriteLine();
		CodeGen.AddSkiped(comment);
	  }
	}
	protected bool HasSameLineRange(Comment com)
	{
	  return HasSameLineRange(com, SupportElementPosition.After);
	}
	protected virtual bool HasSameLineRange(Comment com, SupportElementPosition possiblePosition)
	{
	  if (com == null || com.Position != possiblePosition)
		return false;
	  CodeElement element = com.TargetNode as CodeElement;
	  if (element == null)
		return false;
	  return HasSameLineRange(element, com);
	}
	public override void GenerateElement(LanguageElement languageElement)
	{
	  if (languageElement == null)
		return;
	  GenerateSupportElement(languageElement as SupportElement);
	}
	public virtual void GenerateAttributes(LanguageElement element)
	{
	  CodeElement codeElement = element as CodeElement;
	  if (codeElement == null)
		return;
	  CodeGen codeGen = CodeGen;
	  AttributeSection section = null;
	  for (int i = 0; i < codeElement.AttributeSections.Count; i++)
	  {
		section = codeElement.AttributeSections[i] as AttributeSection;
		if (section.TargetNode != codeElement ||
			codeGen.IsSkiped(section))
		  continue;
		CodeGen.GenerateElement(section);
	  }
	}
	public virtual void GenerateElementForeComment(LanguageElement element)
	{
	  if (element == null || !(element is CodeElement))
		return;
	  CodeElement codeElement = element as CodeElement;
	  GenerateElementComment(codeElement, SupportElementPosition.Before);
	}
	public virtual void GenerateElementBackComment(LanguageElement element)
	{
	  if (element == null || !(element is CodeElement))
		return;
	  CodeElement codeElement = element as CodeElement;
	  GenerateElementComment(codeElement, SupportElementPosition.After);
	}
	public static bool HasAfterComment(CodeElement target)
	{
	  if (target == null)
		return false;
	  CommentCollection coll = target.Comments;
	  if (coll == null || coll.Count == 0)
		return false;
	  int count = coll.Count;
	  for (int i = 0; i < count; i++)
	  {
		Comment comment = coll[i];
		if (comment == null)
		  continue;
		if (comment.Position == SupportElementPosition.After && HasSameLineRange(target, comment))
		  return true;
	  }
	  return false;
	}
	public static bool HasSameLineRange(CodeElement target, Comment comment)
	{
	  SourceRange targetRange = target.Range;
	  SourceRange commentRange = comment.Range;
	  if (targetRange.IsEmpty || commentRange.IsEmpty)
		return false;
	  int targetLine = targetRange.End.Line;
	  int commentLine = commentRange.Start.Line;
	  return targetLine == commentLine;
	}
  }
}
