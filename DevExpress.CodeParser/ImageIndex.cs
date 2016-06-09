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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ImageIndex
	{
	public const int None = 0;
		public const int Assignment = 1;
	public const int BaseType = 2;
	public const int CaseBlock = 3;
	public const int CatchBlock = 4;
	public const int Class = 5;
	public const int ClassAliasImport = 6;
	public const int CodeBlock = 7;
	public const int CSharpComment = 8;
	public const int ConditionalDirective = 9;
	public const int Constructor = 10;
	public const int Delegate = 11;
	public const int CSharpDocComment = 12;
	public const int DoWhile = 13;
	public const int ElseBlock = 14;
	public const int EnumElement = 15;
	public const int Enumeration = 16;
	public const int Event = 17;
	public const int Expression = 18;
	public const int Field = 19;
	public const int File = 20;
	public const int FinallyBlock = 21;
	public const int FlowBreak = 22;
	public const int ForEach = 23;
	public const int ForLoop = 24;
	public const int IfBlock = 25;
	public const int IndexedProperty = 26;
	public const int Interface = 27;
	public const int Internal = 28;
	public const int Lock = 29;
	public const int Method = 30;
	public const int MethodCall = 31;
	public const int Namespace = 32;
	public const int Private = 33;
	public const int Property = 34;
	public const int Protected = 35;
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use ImageIndex.ProtectedOrInternal instead.")]
	public const int ProtectedInternal = 36;
		public const int ProtectedOrInternal = 36;
	public const int Reference = 37;
	public const int Region = 38;
	public const int ResourceString = 39;
	public const int RootNode = 40;
	public const int Set = 41;
	public const int StaticClass = 42;
	public const int StaticConstructor = 43;
	public const int StaticField = 44;
	public const int StaticIndexedProperty = 45;
	public const int StaticMethod = 46;
	public const int StaticProperty = 47;
	public const int StaticStruct = 48;
	public const int Struct = 49;
	public const int SwitchStatement = 50;
	public const int ThreadVar = 51;
	public const int TryBlock = 52;
	public const int UsingOrImports = 53;
	public const int UsingStatement = 54;
	public const int WhileDo = 55;
	public const int WithBlock = 56;
		public const int DocumentElement = 57;
		public const int LanguageElement = 58;
	public const int BitwiseEnum = 59;
	public const int StaticEnumeration = 60;
	public const int StaticBitwiseEnum = 61;
		public const int Constant = 62;
		public const int StaticConstant = 63;
		public const int FixedField = 64;
		public const int FixedBlock = 65;
		public const int Param = 66;
		public const int ParamIn = 67;
		public const int ParamOut = 68;
		public const int ParamRef = 69;
	public const int ParamArray = 70;
	public const int Attribute = 71;
		public const int AssemblyCode = 72;
		public const int IllegalVisibility = 73;
		public const int VisualBasicSourceFile = 74;
		public const int CPlusPlusSourceFile = 75;
		public const int CSharpSourceFile = 76;
		public const int VisualJSharpSourceFile = 77;
		public const int StaticEvent = 78;
	public const int Abort = 79;
	public const int Continue = 80;
		public const int Return = 81;
	public const int Throw = 82;
	public const int Break = 83;
		public const int Goto = 84;
		public const int Label = 85;
		public const int Statement = 86;
		public const int End = 87;
		public const int Stop = 88;
		public const int TypeCast = 89;
		public const int TypeOf = 90;
		public const int ObjectCreation = 91;
		public const int BinaryOperator = 92;
		public const int MethodCallExpression = 93;
		public const int TypeReference = 94;
		public const int MethodReference = 95;
	public const int Module = 145;
		public const int ElementReference = 96;
		public const int PrimitiveExpression = 97;
		public const int ArgumentDirection = 98;
		public const int ThisReference = 99;
		public const int UnaryOperator = 100;
		public const int Indexer = 101;
		public const int AddressOfExpression = 102;
		public const int ArrayCreateExpression = 103;
		public const int BaseReference = 104;
		public const int CheckedExpression = 105;
		public const int UncheckedExpression = 106;
		public const int ConditionalExpression = 107;
		public const int IsExpression = 108;
		public const int Public = 109;
		public const int Comment = 110;
		public const int DocComment = 111;
		public const int DocCommentSummary = 112;
		public const int DocCommentParam = 113;
		public const int DocCommentReturns = 114;
		public const int DocCommentSeeAlso = 115;
		public const int DocCommentElement = 116;
		public const int ProtectedAndInternal = 117;
		public const int DocCommentText = 118;
		public const int DocCommentAttribute = 119;
		public const int DocCommentList = 120;
		public const int DocCommentItem = 121;
		public const int DocCommentCodeMultiLine = 122;
		public const int DocCommentCodeSingleLine = 123;
		public const int DocCommentParagraph = 124;
		public const int LogicalOperation = 125;
		public const int LogicalInversion = 126;
		public const int UnaryIncrement = UnaryOperator;
		public const int UnaryDecrement = 127;
		public const int RelationalOperation = 128;
		public const int TypeParameter = 129;
		public const int NestedMethod = 130;
		public const int HtmlScript = 131;
		public const int ServerControlElement = 132;
		public const int AspCodeNugget = 133;
		public const int XmlElement = 134;
		public const int Project = 135;
		public const int WebProject = 136;
		public const int AspxFile = 137;
		public const int CodeBehindFile = 138;
		public const int StyleSheet = 139;
		public const int WebConfig = 140;
		public const int XamlFile = 141;
		public const int SqlFile = 142;
		public const int YieldBreak = 143;
		public const int YieldReturn = 144;
	public const int MarkupExtensionExpression = 145;
	public const int DependencyProperty = 146;
	}
}
