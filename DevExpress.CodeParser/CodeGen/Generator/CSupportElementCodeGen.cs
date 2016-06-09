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
  public abstract class CSupportElementCodeGen : SupportElementCodeGenBase
  {
	public CSupportElementCodeGen(CodeGen codeGen)
	  : base(codeGen)
	{
	}
	protected virtual void GenerateAttributeTargetType(CodeWriter code, AttributeTargetType target)
	{
	  switch (target)
	  {
		case AttributeTargetType.Assembly:
		  code.Write("assembly");
		  break;
		case AttributeTargetType.Class:
		  code.Write("class");
		  break;
		case AttributeTargetType.Constructor:
		  code.Write("constructor");
		  break;
		case AttributeTargetType.Delegate:
		  code.Write("delegate");
		  break;
		case AttributeTargetType.Enum:
		  code.Write("enum");
		  break;
		case AttributeTargetType.Event:
		  code.Write("event");
		  break;
		case AttributeTargetType.Field:
		  code.Write("field");
		  break;
		case AttributeTargetType.Interface:
		  code.Write("interface");
		  break;
		case AttributeTargetType.Method:
		  code.Write("method");
		  break;
		case AttributeTargetType.Param:
		  code.Write("parameter");
		  break;
		case AttributeTargetType.Property:
		  code.Write("property");
		  break;
		case AttributeTargetType.Return:
		  code.Write("returnvalue");
		  break;
		case AttributeTargetType.Struct:
		  code.Write("struct");
		  break;
		case AttributeTargetType.Module:
		  code.Write("module");
		  break;
		case AttributeTargetType.None:
		  return;
	  }
	  if (code.Options.Spacing.BeforeAttributeTargetColon)
		code.Write(" ");
	  code.Write(":");
	  if (code.Options.Spacing.AfterAttributeTargetColon)
		code.Write(" ");
	}
	protected virtual void GenerateAttributeQualifier(Attribute attr, string delimiter)
	{
	  if (attr.Qualifier == null)
		return;
	  CodeGen.GenerateElement(attr.Qualifier);
	  if (Options.Spacing.BeforeDot)
		Code.Write(" ");
	  Code.Write(".");
	  if (Options.Spacing.AfterDot)
		Code.Write(" ");
	}
	protected override void GenerateAttribute(Attribute attr)
	{
	  GenerateAttributeTargetType(Code, attr.TargetType);
	  GenerateAttributeQualifier(attr, ".");
	  Code.Write(attr.Name);
	  if (attr.ArgumentCount <= 0)
		return;
	  Code.Write("(");
	  GenerateParameters(attr.Arguments, GetCommaDelimeter());
	  Code.Write(")");
	}
	protected override void GenerateAttribute(AttributeSection section)
	{
	  Code.Write("[");
	  bool withinAttributeBracketsSpacing = Options.Spacing.WithinAttributeBrackets;
	  if (withinAttributeBracketsSpacing)
		Code.Write(" ");
	  GenerateElementCollection(section.AttributeCollection, GetCommaDelimeter());
	  if (withinAttributeBracketsSpacing)
		Code.Write(" ");
	  Code.Write("]");
	  LanguageElement target = section.TargetNode;
	  bool placeAttributeOnSeparateLine = target == null ||
			Options.LineBreaks.PlaceTypeAttributeOnSeparateLine && target is TypeDeclaration
	   || Options.LineBreaks.PlaceMethodAttributeOnSeparateLine && target is Method
	   || Options.LineBreaks.PlacePropertyAttributeOnSeparateLine && target is Property
	   || Options.LineBreaks.PlaceFieldConstantAttributeOnSeparateLine && target is Variable
	   || Options.LineBreaks.PlaceEventAttributeOnSeparateLine && target is Event
	   || Options.LineBreaks.PlaceEnumElementAttributeOnSeparateLine && target is EnumElement;
	  if (target is Param || !placeAttributeOnSeparateLine)
		Code.Write(" ");
	  else
		if (section.NextSibling != null && target != null)
		  Code.WriteLine();
	}
	protected override void GenerateComment(Comment comment)
	{
	  bool indentComment = Code.Options.Indention.IndentComment;
	  int indentLevel = 0;
	  if (!indentComment)
	  {
		indentLevel = Code.IndentLevel;
		Code.IndentLevel = 0;
	  }
	  if (comment.CommentType == CommentType.SingleLine)
	  {
		Code.Write("//");
		Code.Write(comment.Name);
	  }
	  else
	  {
		if (!Code.IsAtWhitespace)
		  Code.Write(" ");
		Code.Write("/*");
		int spacesCount = comment.TextStartOffset - 2;
		string spaces = String.Empty;
		if (spacesCount > 0)
		  spaces = " ".PadRight(spacesCount);
		Code.Write(spaces);
		Code.Write(comment.Name);
		Code.Write("*/");
	  }
	  if (!indentComment)
		Code.IndentLevel = indentLevel;
	}
  }
}
