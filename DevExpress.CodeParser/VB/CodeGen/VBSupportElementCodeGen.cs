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
	public class VBSupportElementCodeGen : SupportElementCodeGenBase 
	{
		public VBSupportElementCodeGen(CodeGen codeGen) : base(codeGen) 
		{
		}
		void GenerateAttributeTargetType(AttributeTargetType target) 
		{
			switch (target) 
			{
				case AttributeTargetType.Assembly:
					Write(FormattingTokenType.Assembly, FormattingTokenType.Colon);
					break;
				case AttributeTargetType.Module:
		  Write(FormattingTokenType.Module, FormattingTokenType.Colon);
					break;
			}
		}
	protected override void GenerateCppAttribute(CppAttributeSection element)
	{
	}
	protected override void GenerateAttribute(Attribute attr) 
		{
			if (attr == null)
				return;
			GenerateAttributeTargetType(attr.TargetType);
			if (attr.Qualifier != null)
			{
				CodeGen.GenerateElement(attr.Qualifier);
				Write(FormattingTokenType.Dot);
			}
			Write(FormattingTokenType.Ident);
	  if (attr.ArgumentCount > 0)
			  GenerateParameters(attr.Arguments);
		}
		protected override void GenerateAttribute(AttributeSection section) 
		{
	  FormattingTokenType[] delimiter;
	  if (Options.LineBreaks.PlaceMultipleAttributesOnTheirOwnLine)
		delimiter = new FormattingTokenType[] { FormattingTokenType.Comma, FormattingTokenType.LineContinuation };
	  else
		delimiter = new FormattingTokenType[] { FormattingTokenType.Comma };
			Write(FormattingTokenType.LessThan);
			GenerateElementCollection(section.AttributeCollection, delimiter);
		  Write(FormattingTokenType.GreaterThen);
		}
		protected override void GenerateComment(Comment comment) 
		{
	  using (GetClearIndent(!Options.Indention.IndentComment))
	  {
		if (comment.CommentType == CommentType.SingleLine)
		  Write(FormattingTokenType.SingleQuote, FormattingTokenType.Ident);
		else
		{
		  string[] commentLines = StringHelper.SplitLines(comment.Name, false);
		  int count = commentLines.Length;
		  for (int i = 0; i < count; i++)
		  {
			Write(FormattingTokenType.SingleQuote);
			Write(commentLines[i]);
		  }
		}
	  }
		}
	public override FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.NextFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Colon:
		  if (ContextMatch(LanguageElementType.Attribute) && Options.Spacing.AfterAttributeTargetColon)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  FormattingElements result = base.PrevFormattingElements(tokenType);
	  switch (tokenType)
	  {
		case FormattingTokenType.Colon:
		  if (ContextMatch(LanguageElementType.Attribute) && Options.Spacing.BeforeAttributeTargetColon)
			result.AddWhiteSpace();
		  break;
	  }
	  return result;
	}
	public override bool GenerateElementTail(LanguageElement element)
	{
	  if (element.ElementType != LanguageElementType.AttributeSection)
		return false;
	  AttributeSection section = (AttributeSection)element;
	  if (section.TargetNode == null)
	  {
		AttributeSection nextAttribute = section.NextSibling as AttributeSection;
		if (Options.BlankLines.AfterGlobalAttributes && (nextAttribute == null || nextAttribute.TargetNode != null))
		  CodeGen.AddNewLine(2);
		else
		  CodeGen.AddNewLineIfNeeded();
		return true;
	  }
	  bool needNewLine = false;
	  switch (section.TargetNode.ElementType)
	  {
		case LanguageElementType.Event:
		  needNewLine = Options.LineBreaks.PlaceEventAttributeOnSeparateLine;
		  break;
		case LanguageElementType.Variable:
		case LanguageElementType.Const:
		case LanguageElementType.InitializedVariable:
		  BaseVariable var = (BaseVariable)section.TargetNode;
		  if (var.IsField || var.IsConst)
			needNewLine = Options.LineBreaks.PlaceFieldConstantAttributeOnSeparateLine;
		  break;
		case LanguageElementType.Method:
		  needNewLine = Options.LineBreaks.PlaceMethodAttributeOnSeparateLine;
		  break;
		case LanguageElementType.Property:
		  needNewLine = Options.LineBreaks.PlacePropertyAttributeOnSeparateLine;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Enum:
		case LanguageElementType.Struct:
		case LanguageElementType.Delegate:
		case LanguageElementType.Module:
		case LanguageElementType.Interface:
		  needNewLine = Options.LineBreaks.PlaceTypeAttributeOnSeparateLine;
		  break;
		case LanguageElementType.EnumElement:
		  needNewLine = Options.LineBreaks.PlaceEnumElementAttributeOnSeparateLine;
		  break;
	  }
	  if (needNewLine)
		Write(FormattingTokenType.LineContinuation);
	  else
		CodeGen.AddWSIfNeeded();
	  return true;
	}
	}
}
