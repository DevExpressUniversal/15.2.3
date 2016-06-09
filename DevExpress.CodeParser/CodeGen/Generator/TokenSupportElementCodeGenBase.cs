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
  public abstract class TokenSupportElementCodeGenBase : SupportElementCodeGenBase
  {
	public TokenSupportElementCodeGenBase(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected virtual void GenerateAttributeTargetType(AttributeTargetType target)
	{
	  switch (target)
	  {
		case AttributeTargetType.Assembly:
		  Write(FormattingTokenType.Assembly);
		  break;
		case AttributeTargetType.Class:
		  Write(FormattingTokenType.Class);
		  break;
		case AttributeTargetType.Constructor:
		  Write(FormattingTokenType.Constructor);
		  break;
		case AttributeTargetType.Delegate:
		  Write(FormattingTokenType.Delegate);
		  break;
		case AttributeTargetType.Enum:
		  Write(FormattingTokenType.Enum);
		  break;
		case AttributeTargetType.Event:
		  Write(FormattingTokenType.Event);
		  break;
		case AttributeTargetType.Field:
		  Write(FormattingTokenType.Field);
		  break;
		case AttributeTargetType.Interface:
		  Write(FormattingTokenType.Interface);
		  break;
		case AttributeTargetType.Method:
		  Write(FormattingTokenType.Method);
		  break;
		case AttributeTargetType.Param:
		  Write(FormattingTokenType.Param);
		  break;
		case AttributeTargetType.Property:
		  Write(FormattingTokenType.Property);
		  break;
		case AttributeTargetType.Return:
		  Write(FormattingTokenType.Return);
		  break;
		case AttributeTargetType.Struct:
		  Write(FormattingTokenType.Struct);
		  break;
		case AttributeTargetType.Module:
		  Write(FormattingTokenType.Module);
		  break;
		case AttributeTargetType.None:
		  return;
	  }
	  Write(FormattingTokenType.Colon);
	}
	protected virtual void GenerateAttributeQualifier(Attribute attr, FormattingTokenType delimiter)
	{
	  if (attr.Qualifier == null)
		return;
	  CodeGen.GenerateElement(attr.Qualifier);
	  Write(delimiter);
	}
	protected override void GenerateAttribute(Attribute attr)
	{
	  GenerateAttributeTargetType(attr.TargetType);
	  GenerateAttributeQualifier(attr, FormattingTokenType.Dot);
	  Write(FormattingTokenType.Ident);
	  if (attr.ArgumentCount <= 0)
		return;
	  GenerateParameters(attr.Arguments);
	}
	protected override void GenerateAttribute(AttributeSection section)
	{
	  Write(FormattingTokenType.BracketOpen);
	  GenerateElementCollection(section.AttributeCollection, FormattingTokenType.Comma);
	  Write(FormattingTokenType.BracketClose);
	}
	protected override void GenerateComment(Comment comment)
	{
	  if (CodeGen.TokenArgsContainsElement(comment))
		return;
	  using (GetClearIndent(!Code.Options.Indention.IndentComment))
	  {
		if (comment.CommentType == CommentType.SingleLine)
		  Write(string.Concat("//", comment.Name));
		else
		{
		  if (!Code.IsAtWhitespace)
			CodeGen.AddWSIfNeeded();
		  int spacesCount = comment.TextStartOffset - 2;
		  string spaces = String.Empty;
		  if (spacesCount > 0)
			spaces = " ".PadRight(spacesCount);
		  Write(string.Concat("/*", spaces, comment.Name, "*/"), false);
		}
	  }
	}
	protected override void GenerateElementComment(CodeElement codeElement, SupportElementPosition position)
	{
	  if (codeElement == null || codeElement.Comments == null || codeElement.CommentCount == 0)
		return;
	  Comment comment = null;
	  CommentCollection coll = codeElement.Comments;
	  int commentsCount = codeElement.CommentCount;
	  for ( int i = 0; i < commentsCount; i++ )
	  {
		comment = coll[i];
		if ( comment.Position != position || comment.TargetNode != codeElement || CodeGen.IsSkiped(comment) )
		  continue;
		if (HasSameLineRange(comment) )
		  CodeGen.AddWSIfNeeded();
		CodeGen.GenerateElement(comment);
		if (comment.CommentType == CommentType.SingleLine && position == SupportElementPosition.Before)
		  CodeGen.AddNewLineIfNeeded();
		CodeGen.AddSkiped(comment);
	  }
	}
	public override void GenerateAttributes(LanguageElement element)
	{
	  CodeElement codeElement = element as CodeElement;
	  if (codeElement == null)
		return;
	  CodeGen codeGen = CodeGen;
	  AttributeSection section = null;
	  for (int i = 0; i < codeElement.AttributeSections.Count; i++)
	  {
		section = codeElement.AttributeSections[i] as AttributeSection;
		if (section == null ||
			  section.TargetNode != codeElement ||
			  codeGen.IsSkiped(section))
		  continue;
		bool lastState = CodeGen.GeneratingInSupportElement;
		CodeGen.GeneratingInSupportElement = true;
		CodeGen.GenerateElement(section);
		CodeGen.GeneratingInSupportElement = lastState;
	  }
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Colon:
		  if (Context.ElementType == LanguageElementType.Attribute && Options.Spacing.BeforeAttributeTargetColon)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketClose:
		  if (Context.ElementType == LanguageElementType.AttributeSection && Options.Spacing.WithinAttributeBrackets)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	private bool GenerateCodeBlock(PropertyAccessor target)
	{
	  return target != null ? target.GenerateCodeBlock : false;
	}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Colon:
		  if (Context.ElementType == LanguageElementType.Attribute && Options.Spacing.AfterAttributeTargetColon)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketOpen:
		  if (Context.ElementType == LanguageElementType.AttributeSection && Options.Spacing.WithinAttributeBrackets)
			result.AddWhiteSpace();
		  break;
		case FormattingTokenType.BracketClose:
		  AttributeSection section = Context as AttributeSection;
		  if (section == null)
			break;
		  LanguageElement target = section.TargetNode;
		  bool placeAttributeOnSeparateLine = target == null ||
			  Options.LineBreaks.PlaceTypeAttributeOnSeparateLine && (target is TypeDeclaration || target is DelegateDefinition) ||
			  Options.LineBreaks.PlaceMethodAttributeOnSeparateLine && target is Method ||
			  Options.LineBreaks.PlaceMethodAttributeOnSeparateLine && GenerateCodeBlock(target as PropertyAccessor) ||
			  Options.LineBreaks.PlacePropertyAttributeOnSeparateLine && target is Property ||
			  Options.LineBreaks.PlaceFieldConstantAttributeOnSeparateLine && target is Variable ||
			  Options.LineBreaks.PlaceEventAttributeOnSeparateLine && target is Event ||
			  Options.LineBreaks.PlaceEnumElementAttributeOnSeparateLine && target is EnumElement;
		  if (target is Param || !placeAttributeOnSeparateLine)
			result.AddWhiteSpace();
		  else
			if ((section.NextSibling != null || section.Parent == null) && target != null)
			  result.AddNewLine();
		  break;
		case FormattingTokenType.Comma:
		  if (ContextMatch(LanguageElementType.AttributeSection) && Options.LineBreaks.PlaceMultipleAttributesOnTheirOwnLine)
			result.AddNewLine();
		  break;
	  }
	  return result;
	}
  }
}
