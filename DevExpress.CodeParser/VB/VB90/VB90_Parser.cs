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
using System.Collections;
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  using XmlImports = XmlNamespaceReference;
  using Exit = VB.ExitStatement;
#if DXCORE
using StructuralParser = DevExpress.CodeRush.StructuralParser;
#else
using StructuralParser = DevExpress.CodeParser;
#endif
  public class Tokens
	{
		public const int Preserve = 165;
		public const int SByte = 173;
		public const int Descending = 230;
		public const int Each = 95;
		public const int AndAlso = 56;
		public const int EndRegion = 11;
		public const int CUShort = 85;
		public const int Xor = 207;
		public const int By = 227;
		public const int Like = 130;
		public const int MyBase = 138;
		public const int CLng = 74;
		public const int SingleLineCloseTag = 241;
		public const int Call = 62;
		public const int Case = 63;
		public const int String = 183;
		public const int Interface = 123;
		public const int From = 221;
		public const int Until = 198;
		public const int Auto = 212;
		public const int ByRef = 59;
		public const int Property = 162;
		public const int ParenClose = 36;
		public const int Strict = 215;
		public const int GoSub = 113;
		public const int NotOverridable = 147;
		public const int New = 142;
		public const int StringLiteral = 17;
		public const int Region = 10;
		public const int LessThan = 49;
		public const int ExternalSourceDirective = 12;
		public const int CBool = 65;
		public const int ShiftLeft = 25;
		public const int XmlCommentString = 244;
		public const int Select = 174;
		public const int EqualsSymbol = 47;
		public const int Let = 128;
		public const int CloseTag = 240;
		public const int Static = 180;
		public const int Overloads = 156;
		public const int Imports = 118;
		public const int Module = 135;
		public const int EndIf = 99;
		public const int CStr = 81;
		public const int CChar = 67;
		public const int Explicit = 214;
		public const int In = 119;
		public const int Long = 131;
		public const int CurlyBraceClose = 34;
		public const int PlusEqual = 18;
		public const int CloseEmbeddedCodeTag = 239;
		public const int Comma = 32;
		public const int Handles = 115;
		public const int Not = 144;
		public const int GreaterOrEqual = 31;
		public const int CByte = 66;
		public const int Single = 179;
		public const int Aggregate = 235;
		public const int Structure = 184;
		public const int CShort = 79;
		public const int Delegate = 90;
		public const int Erase = 101;
		public const int Ascending = 229;
		public const int Then = 187;
		public const int Binary = 217;
		public const int When = 201;
		public const int Overridable = 157;
		public const int Out = 120;
		public const int Do = 93;
		public const int Shadows = 176;
		public const int Friend = 108;
		public const int ULong = 195;
		public const int LineTerminator = 1;
		public const int MulEqual = 20;
		public const int Wend = 200;
		public const int While = 202;
		public const int Off = 219;
		public const int AddToken = 208;
		public const int Using = 197;
		public const int CDbl = 69;
		public const int DivEqual = 21;
		public const int SyncLock = 186;
		public const int RemoveHandler = 169;
		public const int BackSlashEquals = 22;
		public const int Partial = 160;
		public const int CInt = 72;
		public const int MinusEqual = 19;
		public const int ColonEquals = 39;
		public const int Const = 76;
		public const int Minus = 41;
		public const int Ansi = 210;
		public const int True = 190;
		public const int Char = 71;
		public const int IsTrue = 127;
		public const int ElseIfDirective = 8;
		public const int EqualsToken = 224;
		public const int GetTypeToken = 111;
		public const int Loop = 132;
		public const int Group = 228;
		public const int IfToken = 116;
		public const int CommAtSymbol = 242;
		public const int Custom = 220;
		public const int IntegerLiteral = 2;
		public const int Event = 103;
		public const int FloatingPointLiteral = 3;
		public const int CType = 82;
		public const int LineContinuation = 14;
		public const int Order = 226;
		public const int ShiftRightEqual = 27;
		public const int XorEqual = 23;
		public const int GreaterThan = 48;
		public const int Private = 161;
		public const int Byte = 60;
		public const int Identifier = 4;
		public const int ReadOnly = 167;
		public const int NotEquals = 29;
		public const int Return = 171;
		public const int CULng = 84;
		public const int TripleDot = 243;
		public const int Inherits = 121;
		public const int MyClass = 139;
		public const int Declare = 88;
		public const int Unicode = 213;
		public const int Get = 110;
		public const int ShiftRight = 26;
		public const int Short = 178;
		public const int Or = 154;
		public const int ParenOpen = 35;
		public const int Widening = 203;
		public const int CSng = 80;
		public const int Shared = 177;
		public const int Assembly = 211;
		public const int MustOverride = 137;
		public const int Implements = 117;
		public const int BackSlash = 44;
		public const int SingleLineComment = 15;
		public const int Integer = 122;
		public const int AddHandler = 52;
		public const int Function = 109;
		public const int Slash = 43;
		public const int Is = 124;
		public const int Rem = 172;
		public const int CUInt = 83;
		public const int Namespace = 140;
		public const int Continue = 77;
		public const int Me = 133;
		public const int Global = 112;
		public const int SingleLineXmlComment = 247;
		public const int As = 57;
		public const int Resume = 170;
		public const int Sub = 185;
		public const int WithEvents = 205;
		public const int EndToken = 98;
		public const int ReDim = 168;
		public const int Finally = 106;
		public const int Where = 222;
		public const int Nothing = 145;
		public const int Alias = 54;
		public const int Catch = 64;
		public const int Enum = 100;
		public const int For = 107;
		public const int Of = 149;
		public const int Protected = 163;
		public const int CSByte = 78;
		public const int And = 55;
		public const int Colon = 38;
		public const int OpenEmbeddedCodeTag = 238;
		public const int Operator = 151;
		public const int Try = 191;
		public const int OrElse = 155;
		public const int Error = 102;
		public const int Optional = 153;
		public const int IsNot = 125;
		public const int Mod = 134;
		public const int Dim = 91;
		public const int LessOrEqual = 30;
		public const int ConstDirective = 5;
		public const int Take = 237;
		public const int False = 105;
		public const int Boolean = 58;
		public const int On = 150;
		public const int NotInheritable = 146;
		public const int CDec = 70;
		public const int Sharp = 51;
		public const int Overrides = 158;
		public const int Into = 225;
		public const int UShort = 196;
		public const int Narrowing = 141;
		public const int BitAnd = 50;
		public const int ByVal = 61;
		public const int IsFalse = 126;
		public const int EndifDirective = 7;
		public const int PercentSymbol = 246;
		public const int AndEqual = 24;
		public const int Set = 175;
		public const int Else = 96;
		public const int Option = 152;
		public const int GoTo = 114;
		public const int Object = 148;
		public const int With = 204;
		public const int Variant = 199;
		public const int MustInherit = 136;
		public const int Class = 73;
		public const int Default = 89;
		public const int Join = 223;
		public const int Date = 86;
		public const int ElseIf = 97;
		public const int RaiseEvent = 166;
		public const int Plus = 40;
		public const int Dot = 37;
		public const int CurlyBraceOpen = 33;
		public const int Lib = 129;
		public const int Throw = 188;
		public const int Text = 218;
		public const int ShiftLeftEqual = 28;
		public const int TryCast = 192;
		public const int CharacterLiteral = 16;
		public const int DirectCast = 92;
		public const int Next = 143;
		public const int ExclamationSymbol = 45;
		public const int Distinct = 232;
		public const int Question = 231;
		public const int Double = 94;
		public const int Compare = 216;
		public const int CDate = 68;
		public const int XorSymbol = 46;
		public const int ToToken = 189;
		public const int EOF = 0;
		public const int Exit = 104;
		public const int WriteOnly = 206;
		public const int ParamArray = 159;
		public const int Stop = 182;
		public const int AddressOf = 53;
		public const int Infer = 233;
		public const int RemoveToken = 209;
		public const int IfDirective = 6;
		public const int Asterisk = 42;
		public const int UInteger = 194;
		public const int Step = 181;
		public const int DollarSybol = 245;
		public const int ElseDirective = 9;
		public const int Decimal = 87;
		public const int Public = 164;
		public const int KeyToken = 234;
		public const int Skip = 236;
		public const int TypeOf = 193;
		public const int EndExternalSourceDirective = 13;
		public const int CObj = 75;
		public const int MaxTokens = 248;
		public static int[] Keywords = {
			165 
			,173 
			,230 
			,95 
			,56 
			,85 
			,227 
			,130 
			,138 
			,74 
			,62 
			,63 
			,183 
			,198 
			,212 
			,59 
			,162 
			,68 
			,215 
			,106 
			,147 
			,142 
			,141 
			,90 
			,65 
			,145 
			,174 
			,128 
			,156 
			,118 
			,135 
			,99 
			,81 
			,67 
			,214 
			,119 
			,131 
			,189 
			,150 
			,200 
			,66 
			,179 
			,184 
			,79 
			,101 
			,229 
			,187 
			,201 
			,157 
			,120 
			,93 
			,108 
			,195 
			,98 
			,186 
			,129 
			,202 
			,219 
			,109 
			,197 
			,160 
			,136 
			,169 
			,172 
			,72 
			,75 
			,76 
			,132 
			,210 
			,190 
			,71 
			,127 
			,232 
			,104 
			,224 
			,111 
			,228 
			,116 
			,103 
			,82 
			,70 
			,89 
			,237 
			,161 
			,60 
			,167 
			,171 
			,170 
			,176 
			,139 
			,88 
			,110 
			,226 
			,178 
			,154 
			,121 
			,203 
			,80 
			,177 
			,137 
			,117 
			,122 
			,52 
			,115 
			,207 
			,152 
			,83 
			,140 
			,77 
			,133 
			,112 
			,57 
			,185 
			,205 
			,204 
			,168 
			,188 
			,54 
			,64 
			,100 
			,107 
			,149 
			,163 
			,78 
			,55 
			,151 
			,191 
			,155 
			,102 
			,153 
			,125 
			,134 
			,91 
			,225 
			,105 
			,58 
			,181 
			,146 
			,123 
			,158 
			,61 
			,126 
			,194 
			,175 
			,96 
			,196 
			,114 
			,213 
			,222 
			,199 
			,211 
			,73 
			,144 
			,223 
			,86 
			,166 
			,84 
			,220 
			,218 
			,113 
			,192 
			,92 
			,143 
			,53 
			,231 
			,94 
			,216 
			,235 
			,69 
			,180 
			,206 
			,159 
			,182 
			,217 
			,233 
			,209 
			,97 
			,148 
			,124 
			,87 
			,164 
			,234 
			,236 
			,193 
			,221 
			,208 
		};
	}
	public partial class VB90Parser
  {
	private bool IsYieldStatement()
	{
	  if (la.Type != Tokens.Identifier || la.Value.ToLower() != "yield")
		return false;
	  ResetPeek();
	  int nextType = Peek().Type;
	  switch(nextType)
	  {
		case Tokens.Identifier:
		case Tokens.ParenOpen:
		case Tokens.True:
		case Tokens.False:
		case Tokens.TypeOf:
		case Tokens.IntegerLiteral:
		case Tokens.FloatingPointLiteral:
		case Tokens.StringLiteral:
		case Tokens.String:
		  return true;
	  }
	  return false;
	}
	protected override void HandlePragmas()
	{
	}
		void ParserRoot()
	{
		SetDefaultOptionStrict();
		SetDefaultOptionInfer();
		ParserRootCore();
		if (IsDeclEnd())
		{
			LanguageElement oldContext = Context;
		EndNamespaceOrType();
		if (oldContext != Context)
		CallAppropriateParseRule(Context);
		}
		else if (!IsValidEndParsing())
			ParserRootCore();
	}
	void ParserRootCore()
	{
		WhitespaceLines();
		while (la.Type == Tokens.Option )
		{
			OptionDirective();
		}
		while (la.Type == Tokens.Imports )
		{
			ImportsStmt();
		}
		while (IsFileAttributeSection())
		{
			FileAttributesSection();
		}
		while (StartOf(1))
		{
			if (StartOf(2))
			{
				NamespaceDeclaration();
			}
			else
			{
				bool wasCorruptedElement = false;
				CorruptedEndDeclaration(out wasCorruptedElement);
				if (!wasCorruptedElement)
				break;
			}
		}
		StatementTerminatorCall();
	}
	void EndNamespaceOrType()
	{
		LanguageElement element = Context;
		SetBlockRangeForEmptyContext();
		Expect(Tokens.EndToken );
		switch (la.Type)
		{
		case Tokens.Namespace : 
		{
			Get();
			break;
		}
		case Tokens.Class : 
		{
			Get();
			break;
		}
		case Tokens.Structure : 
		{
			Get();
			break;
		}
		case Tokens.Interface : 
		{
			Get();
			break;
		}
		case Tokens.Module : 
		{
			Get();
			break;
		}
		case Tokens.Enum : 
		{
			Get();
			break;
		}
		case Tokens.Function : 
		{
			Get();
			break;
		}
		case Tokens.Sub : 
		{
			Get();
			break;
		}
		case Tokens.Operator : 
		{
			Get();
			break;
		}
		case Tokens.Property : 
		{
			Get();
			break;
		}
		case Tokens.Get : 
		{
			Get();
			break;
		}
		case Tokens.Event : 
		{
			Get();
			break;
		}
		case Tokens.AddHandler : 
		{
			Get();
			break;
		}
		case Tokens.RemoveHandler : 
		{
			Get();
			break;
		}
		case Tokens.RaiseEvent : 
		{
			Get();
			Expect(Tokens.Set );
			break;
		}
		default: SynErr(249); break;
		}
		if (element != null)
		element.SetRange(GetRange(element, tToken));
		CloseContext();
		Namespace namespaceElement = element as Namespace;
		if (namespaceElement != null)
			AddNamespaceToDeclaredList(namespaceElement);
	}
	void WhitespaceLines()
	{
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
	}
	void OptionDirective()
	{
		Token start = la;
		OptionState optionState = OptionState.On;
		OptionType optionType = OptionType.Explicit;
		Expect(Tokens.Option );
		if (la.Type == Tokens.Explicit )
		{
			Get();
			optionType = OptionType.Explicit; 
			OptionValue(out optionState);
			SetOptionExplicit(optionState);
		}
		else if (la.Type == Tokens.Strict )
		{
			Get();
			optionType = OptionType.Strict; 
			OptionValue(out optionState);
			SetOptionStrict(optionState);
		}
		else if (la.Type == Tokens.Infer )
		{
			Get();
			optionType = OptionType.Infer; 
			OptionValue(out optionState);
			SetOptionInfer(optionState);
		}
		else if (la.Type == Tokens.Compare )
		{
			Get();
			optionType = OptionType.Compare; 
			if (la.Type == Tokens.Binary )
			{
				Get();
				optionState = OptionState.Binary; 
			}
			else if (la.Type == Tokens.Text )
			{
				Get();
				optionState = OptionState.Text; 
			}
			else
				SynErr(250);
		}
		else
			SynErr(251);
		OptionStatement option = new OptionStatement(optionState, optionType);
		option.SetRange(GetRange(start, tToken.Range));
		AddNode(option);
		StatementTerminator();
	}
	void ImportsStmt()
	{
		Expect(Tokens.Imports );
		NamespaceReference imports = null; 
		ImportClause(out imports);
		AddNode(imports);
		while (la.Type == Tokens.Comma )
		{
			Get();
			if (imports != null)
			imports.SetEnd(tToken.Range.End);
			ImportClause(out imports);
			AddNode(imports);
		}
		StatementTerminator();
	}
	void FileAttributesSection()
	{
		AttributeSection section = null;
		Token start = la; 
		Expect(Tokens.LessThan );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		section = new AttributeSection(); 
		AttributeList(section);
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		Expect(Tokens.GreaterThan );
		SetRange(section, start, tToken);
		StatementTerminator();
		if (section != null)
		AddNode(section);
	}
	void NamespaceDeclaration()
	{
		LanguageElement declaration = null;
		if (la.Type == Tokens.Namespace )
		{
			Token start = la; 
			QualifiedIdentifier identifier = null;
			Get();
			QualifiedIdentifier(out identifier);
			StatementTerminator();
			declaration = new Namespace(identifier.Identifier);
			OpenContext(declaration);
			declaration.NameRange = identifier.Range;
			bool wasCorruptedElement = false;
			while (StartOf(1))
			{
				if (StartOf(2))
				{
					NamespaceDeclaration();
				}
				else
				{
					CorruptedEndDeclaration(out wasCorruptedElement);
					if (!wasCorruptedElement)
					break;
				}
			}
			bool skipEnd = WaitNamespaceEndToken();
			GetEndToken(skipEnd);
			SetBlockStart(declaration, start.Range);
			SetBlockEnd(declaration);
			if (!skipEnd)
			{
			Expect(Tokens.Namespace );
			CloseContext();
			}
			SetRange(declaration, start, tToken);
			AddNamespaceToDeclaredList(declaration as Namespace);
		}
		else if (StartOf(3))
		{
			ElementMemberDeclaration(out declaration);
		}
		else
			SynErr(252);
		StatementTerminatorCall();
	}
	void CorruptedEndDeclaration(out bool wasCorruptedElement)
	{
		LanguageElement declaration = null;
		wasCorruptedElement = false;
		CorruptedEndDeclarationCore(out declaration, out wasCorruptedElement);
	}
	void StatementTerminatorCall()
	{
		while (la.Type == Tokens.LineTerminator  || la.Type == Tokens.Colon )
		{
			if (la.Type == Tokens.LineTerminator )
			{
				Get();
			}
			else
			{
				Get();
			}
		}
	}
	void StatementTerminator()
	{
		SkipToEol();
		StatementTerminatorCall();
	}
	void OptionValue(out OptionState optionState)
	{
		optionState = OptionState.On;
		if (la.Type == Tokens.On )
		{
			Get();
		}
		else if (la.Type == Tokens.Off )
		{
			Get();
			optionState = OptionState.Off; 
		}
		else
			SynErr(253);
	}
	void ImportClause(out NamespaceReference result)
	{
		Token start = la;
		Token afterImportsToken = la;
		if (tToken.Type == Tokens.Imports)
			start = tToken;
		string xmlStr = GetImportsXmlValue();
		if (xmlStr != null && xmlStr != String.Empty)
		{
			result = CreateXmlImports(xmlStr, afterImportsToken);
			result.SetRange(GetRange(start, tToken));
			return;
		}
		bool isAlias = false;
		string name = String.Empty;
		SourceRange nameRange = SourceRange.Empty;
		QualifiedIdentifier identifier = null;
		NamespaceReference imports = new NamespaceReference();
		Expression expression = null;
		if (IsAliasImports())
		{
			isAlias = true; 
			VarKeyword();
			imports.AliasName = tToken.Value;
			imports.AliasNameRange = tToken.Range;
			Expression aliasExpression = GetElementReference(tToken.Value, tToken.Range);
			imports.AddDetailNode(aliasExpression);
			EqualOperator();
		}
		QualifiedIdentifier(out identifier);
		Token end = tToken;
		imports.SetRange(GetRange(start, end));
		if (identifier != null)
		{
			name = identifier.Identifier;
			nameRange = identifier.Range;
			expression = identifier.Expression;
			if (expression != null)
				imports.AddDetailNode(expression);
		}
		imports.NameRange = nameRange;
		imports.Name = name;
		imports.IsAlias = isAlias;
		string aliasName = imports.AliasName;		
		AddToUsingList(name, aliasName, expression);
		result = imports;
	}
	void VarKeyword()
	{
		switch (la.Type)
		{
		case Tokens.Identifier : 
		{
			Get();
			break;
		}
		case Tokens.Text : 
		{
			Get();
			break;
		}
		case Tokens.Continue : 
		{
			Get();
			break;
		}
		case Tokens.Off : 
		{
			Get();
			break;
		}
		case Tokens.Global : 
		{
			Get();
			break;
		}
		case Tokens.IsNot : 
		{
			Get();
			break;
		}
		case Tokens.IsFalse : 
		{
			Get();
			break;
		}
		case Tokens.IsTrue : 
		{
			Get();
			break;
		}
		case Tokens.Narrowing : 
		{
			Get();
			break;
		}
		case Tokens.Of : 
		{
			Get();
			break;
		}
		case Tokens.Operator : 
		{
			Get();
			break;
		}
		case Tokens.Partial : 
		{
			Get();
			break;
		}
		case Tokens.TryCast : 
		{
			Get();
			break;
		}
		case Tokens.SByte : 
		{
			Get();
			break;
		}
		case Tokens.Using : 
		{
			Get();
			break;
		}
		case Tokens.UInteger : 
		{
			Get();
			break;
		}
		case Tokens.ULong : 
		{
			Get();
			break;
		}
		case Tokens.UShort : 
		{
			Get();
			break;
		}
		case Tokens.AddToken : 
		{
			Get();
			break;
		}
		case Tokens.RemoveToken : 
		{
			Get();
			break;
		}
		case Tokens.Explicit : 
		{
			Get();
			break;
		}
		case Tokens.Strict : 
		{
			Get();
			break;
		}
		case Tokens.Compare : 
		{
			Get();
			break;
		}
		case Tokens.Binary : 
		{
			Get();
			break;
		}
		case Tokens.Ansi : 
		{
			Get();
			break;
		}
		case Tokens.Assembly : 
		{
			Get();
			break;
		}
		case Tokens.Auto : 
		{
			Get();
			break;
		}
		case Tokens.Unicode : 
		{
			Get();
			break;
		}
		case Tokens.Where : 
		{
			Get();
			break;
		}
		case Tokens.Join : 
		{
			Get();
			break;
		}
		case Tokens.EqualsToken : 
		{
			Get();
			break;
		}
		case Tokens.Into : 
		{
			Get();
			break;
		}
		case Tokens.Order : 
		{
			Get();
			break;
		}
		case Tokens.By : 
		{
			Get();
			break;
		}
		case Tokens.Group : 
		{
			Get();
			break;
		}
		case Tokens.Ascending : 
		{
			Get();
			break;
		}
		case Tokens.Descending : 
		{
			Get();
			break;
		}
		case Tokens.Question : 
		{
			Get();
			break;
		}
		case Tokens.Distinct : 
		{
			Get();
			break;
		}
		case Tokens.Custom : 
		{
			Get();
			break;
		}
		case Tokens.Infer : 
		{
			Get();
			break;
		}
		case Tokens.KeyToken : 
		{
			Get();
			break;
		}
		case Tokens.Skip : 
		{
			Get();
			break;
		}
		case Tokens.Take : 
		{
			Get();
			break;
		}
		case Tokens.Out : 
		{
			Get();
			break;
		}
		default: SynErr(254); break;
		}
	}
	void EqualOperator()
	{
		Expect(Tokens.EqualsSymbol );
	}
	void QualifiedIdentifier(out QualifiedIdentifier identifier, CreateElementType createElementType)
	{
		identifier = new QualifiedIdentifier();
		string name = String.Empty;
		string value = String.Empty;
		ReferenceExpressionBase expression = null;
		SourceRange range = SourceRange.Empty;
		Token start = la;
		SourceRange nameRange = la.Range;
		Token token = null;
		IdentifierOrKeyword(out token);
		if (token == null)
		return;
		name = tToken.Value;
		value = name;
		expression = GetExpression(name, tToken.Range, createElementType);
		if (expression != null && expression is TypeReferenceExpression)
		{
		if (la.Type == Tokens.Question )
		{
			TypeReferenceExpression nullableType = null; 
			NullableTypeRule(out nullableType, expression as TypeReferenceExpression);
			if (nullableType != null)
			expression = nullableType;
		}
		}
		SetGenericArgumentsToQualifiedIdentifier(expression);
		while (la.Type == Tokens.Dot )
		{
			Get();
			value += tToken.Value;
			token = null;
			nameRange = la.Range;
			IdentifierOrKeyword(out token);
			if (token != null)
			{
				name = token.Value;
				value += name;
				expression = GetExpression(expression, name, token.Range, createElementType);
			}
			else
			{
				break;
			}
			if (expression != null && expression is TypeReferenceExpression)
			{
			if (la.Type == Tokens.Question )
			{
				TypeReferenceExpression nullableType = null; 
				NullableTypeRule(out nullableType, expression as TypeReferenceExpression);
				if (nullableType != null)
				expression = nullableType;
			}
			}
			SetGenericArgumentsToQualifiedIdentifier(expression);
		}
		Token end = tToken;
		range = GetRange(start, end);
		identifier.Identifier = value;
		expression.NameRange = nameRange;
		identifier.Expression = expression;
		identifier.Range = range;
	}
	void AttributeList(AttributeSection section)
	{
		Attribute attribute = null;
		Attribute(out attribute);
		AddAttribute(section, attribute); 
		while (la.Type == Tokens.Comma )
		{
			Get();
			Attribute(out attribute);
			AddAttribute(section, attribute); 
		}
	}
	void AttributeSection(out AttributeSection section)
	{
		section = null;
		Token start = la; 
		Expect(Tokens.LessThan );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		section = new AttributeSection(); 
		AttributeList(section);
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		Expect(Tokens.GreaterThan );
		SetRange(section, start, tToken); 
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
	}
	void AttributeSections(out LanguageElementCollection attributes)
	{
		attributes = null;
		AttributeSection attrSection = null;	
		while (la.Type == Tokens.LessThan )
		{
			AttributeSection(out attrSection);
			if (attrSection != null)
			{
				if (attributes == null)
					attributes = new LanguageElementCollection();
				attributes.Add(attrSection);
			}
		}
	}
	void Attribute(out Attribute attribute)
	{
		attribute = null;
		AttributeTargetType targetType = AttributeTargetType.None;
		Token start = la; 
		if (la.Type == Tokens.Module  || la.Type == Tokens.Assembly )
		{
			if (la.Type == Tokens.Assembly )
			{
				Get();
				targetType = AttributeTargetType.Assembly; 
			}
			else
			{
				Get();
				targetType = AttributeTargetType.Module; 
			}
			Expect(Tokens.Colon );
		}
		QualifiedIdentifier identifier = null; 
		QualifiedIdentifier(out identifier);
		attribute = new Attribute();
		Expression exp = identifier.Expression;
		if (exp != null)
		{
			attribute.Name = exp.Name;
			attribute.NameRange = exp.NameRange;
			ElementReferenceExpression attrQual = exp as ElementReferenceExpression;
			if (attrQual != null)
			{
				attribute.Qualifier = attrQual.Qualifier;		
			}
		}
		attribute.TargetType = targetType;
		if (la.Type == Tokens.ParenOpen )
		{
			AttributeArguments(attribute);
		}
		Token end = tToken;
		SetRange(attribute, start, end);
	}
	void AttributeArguments(Attribute attribute)
	{
		Expression result = null;
		Token token = null;
		Expect(Tokens.ParenOpen );
		if (IsNotParenClose())
		{
			if (IsNamedAssign())
			{
				IdentifierOrKeyword(out token);
				Expect(Tokens.ColonEquals );
			}
			ExpressionRule(out result);
			if (token != null)
			result = CreateAttributeArgumentInitializer(token, result);
			AddAttributeArgument(attribute, result);
			while (la.Type == Tokens.Comma )
			{
				Get();
				if (IsNamedAssign())
				{
					IdentifierOrKeyword(out token);
					Expect(Tokens.ColonEquals );
				}
				ExpressionRule(out result);
				if (token != null)
				result = CreateAttributeArgumentInitializer(token, result);
				AddAttributeArgument(attribute, result);
			}
		}
		Expect(Tokens.ParenClose );
	}
	void IdentifierOrKeyword(out Token token)
	{
		token = null; 
		if (StartOf(4))
		{
			if (IsIdentifierOrKeyword())
			{
				Get();
				token = tToken; 
			}
			else
			{
			}
		}
	}
	void ExpressionRule(out Expression result)
	{
		result = null;
		try
		{
		  _ExpressionNestingLevel++;
		  if (_ExpressionNestingLevel > 100)
			return;
		OperatorExpression(out result);
		}
		finally
		{
		  _ExpressionNestingLevel--;
		}
	}
	void ElementMemberDeclaration(out LanguageElement element)
	{
		element = null;
		LanguageElementCollection attributes = null;
		TokenQueueBase modifiers = null;
		LanguageElementCollection accessors = null;
		Token startToken = la;
		OnDemandParsingParameters onDemandParsingParameters = null;
		SourceRange blockStart = la.Range;
		AttributesAndModifiers(out attributes, out modifiers);
		if (StartOf(5))
		{
			if (IsOperatorMemberRule())
			{
				MethodDeclaration(out element, out onDemandParsingParameters, new MethodHelper(modifiers));
				SetElementMemberDeclarationProp(element, accessors, modifiers, attributes, onDemandParsingParameters, blockStart);
			}
			else if (IsCustomMemberRule())
			{
				CustomEventMemberDeclaration(out element);
				SetElementMemberDeclarationProp(element, accessors, modifiers, attributes, onDemandParsingParameters, blockStart);
			}
			else
			{
				LanguageElementCollection coll = null;
				bool isConst = HasConst(modifiers);
			DeclareVariableList(out coll, isConst, true);
			if (coll != null && coll.Count != 0)
			{
				bool setStartRange = false;
				for (int i = 0; i < coll.Count; i++)
				{
					LanguageElement var = coll[i];
					if (var == null)
						continue;
					if (i == 0)
						setStartRange = true;
					else
						setStartRange = false;
					SetAttributesAndModifiers(var, attributes, modifiers, setStartRange);
				}
			}
			}
		}
		else if (StartOf(6))
		{
			if (StartOf(7))
			{
				TypeDeclarationRule(out element);
			}
			else if (StartOf(8))
			{
				bool oldIsAsynchronousContext = IsAsynchronousContext;
				IsAsynchronousContext = HasAsyncModifier(modifiers);
				MethodDeclaration(out element, out onDemandParsingParameters, new MethodHelper(modifiers));
				IsAsynchronousContext = oldIsAsynchronousContext;
			}
			else if (la.Type == Tokens.Property )
			{
				PropertyDeclaration(out element, out accessors, out onDemandParsingParameters, new MethodHelper(modifiers));
			}
			else
			{
				CustomEventMemberDeclaration(out element);
			}
			SetElementMemberDeclarationProp(element, accessors, modifiers, attributes, onDemandParsingParameters, blockStart);				
		}
		else
			SynErr(255);
	}
	void CorruptedEndDeclarationCore(out LanguageElement declaration, out bool wasCorruptedElement)
	{
		declaration = null;
		wasCorruptedElement = false;
		if (!IsCorruptedElement())
			return;
		wasCorruptedElement = true;
		declaration = new CorruptedLanguageElement();
		AddNode(declaration);
		SourceRange startRange = la.Range;
		Expect(Tokens.EndToken );
		switch (la.Type)
		{
		case Tokens.Class : 
		{
			Get();
			break;
		}
		case Tokens.Structure : 
		{
			Get();
			break;
		}
		case Tokens.Module : 
		{
			Get();
			break;
		}
		case Tokens.Interface : 
		{
			Get();
			break;
		}
		case Tokens.Event : 
		{
			Get();
			break;
		}
		case Tokens.Namespace : 
		{
			Get();
			break;
		}
		case Tokens.AddHandler : 
		{
			Get();
			break;
		}
		case Tokens.RemoveHandler : 
		{
			Get();
			break;
		}
		case Tokens.RaiseEvent : 
		{
			Get();
			break;
		}
		case Tokens.Get : 
		{
			Get();
			break;
		}
		case Tokens.Set : 
		{
			Get();
			break;
		}
		case Tokens.Enum : 
		{
			Get();
			break;
		}
		case Tokens.Property : 
		{
			Get();
			break;
		}
		case Tokens.Sub : 
		{
			Get();
			break;
		}
		case Tokens.Function : 
		{
			Get();
			break;
		}
		case Tokens.Operator : 
		{
			Get();
			break;
		}
		default: SynErr(256); break;
		}
		SetCorruptedType(tToken, declaration as CorruptedLanguageElement);
		declaration.SetRange(GetRange(startRange, tToken));
	}
	void TypeDeclarationRule(out LanguageElement declaration)
	{
		declaration = null;
		if (StartOf(9))
		{
			ElementDeclaration(out declaration);
		}
		else if (la.Type == Tokens.Enum )
		{
			EnumerationDeclaration(out declaration);
		}
		else if (la.Type == Tokens.Delegate )
		{
			DelegateDeclaration(out declaration);
		}
		else
			SynErr(257);
	}
	void ElementDeclaration(out LanguageElement declaration)
	{
		declaration = null;
		LanguageElement element = null;
		TypeDeclarationEnum typeDeclaration = TypeDeclarationEnum.Class;
		Token dummyToken = null;
		Token start = la; 
		if (la.Type == Tokens.Class )
		{
			Get();
		}
		else if (la.Type == Tokens.Structure )
		{
			Get();
			typeDeclaration = TypeDeclarationEnum.Struct; 
		}
		else if (la.Type == Tokens.Interface )
		{
			Get();
			typeDeclaration = TypeDeclarationEnum.Interface; 
		}
		else if (la.Type == Tokens.Module )
		{
			Get();
			typeDeclaration = TypeDeclarationEnum.Module; 
		}
		else
			SynErr(258);
		IdentifierOrKeyword(out dummyToken);
		if (typeDeclaration == TypeDeclarationEnum.Class)
		declaration = new Class(tToken.Value);
		else if (typeDeclaration == TypeDeclarationEnum.Struct)
			declaration = new Struct(tToken.Value);
		else if (typeDeclaration == TypeDeclarationEnum.Interface)
			declaration = new Interface(tToken.Value);
			else if (typeDeclaration == TypeDeclarationEnum.Module)
			declaration = new Module(tToken.Value);
		   SetDefaultVisibility(declaration as AccessSpecifiedElement);
		OpenContext(declaration);
		declaration.NameRange = tToken.Range;
		if (la.Type == Tokens.ParenOpen )
		{
			GenericModifier genericModifier = null;
			GenericModifierRule(out genericModifier);
			if (genericModifier != null && declaration != null && declaration is AccessSpecifiedElement)
			((AccessSpecifiedElement)declaration).SetGenericModifier(genericModifier);
		}
		StatementTerminatorCall();
		if (la.Type == Tokens.Inherits )
		{
			ClassBaseType(declaration as TypeDeclaration);
		}
		if (la.Type == Tokens.Implements )
		{
			TypeImplementsClause(declaration as TypeDeclaration);
		}
		SourceRange startBlockRange = la.Range;
		SourceRange endBlockRange = la.Range;
		bool wasCorruptedElement = false;
		while (StartOf(10))
		{
			if (StartOf(3))
			{
				ElementMemberDeclaration(out element);
			}
			else
			{
				CorruptedEndDeclarationCore(out element, out wasCorruptedElement);
				if (!wasCorruptedElement)
				break;
			}
			StatementTerminatorCall();
		}
		SetBlockRangeForEmptyElement(declaration, startBlockRange, la.Range);
		bool skipEnd =  WaitTypeEndToken();
		GetEndToken(skipEnd);
		SetBlockEnd(declaration);
		if (!skipEnd)
		{
		if (la.Type == Tokens.Class )
		{
			Get();
		}
		else if (la.Type == Tokens.Structure )
		{
			Get();
		}
		else if (la.Type == Tokens.Interface )
		{
			Get();
		}
		else if (la.Type == Tokens.Module )
		{
			Get();
		}
		else
			SynErr(259);
		CloseContext();
		}
		SetRange(declaration, start, tToken);
	}
	void EnumerationDeclaration(out LanguageElement declaration)
	{
		declaration = null;
		Token dummyToken = null;
		SourceRange typeRange = SourceRange.Empty;
		string name = String.Empty;
		Token start = la; 
		Expect(Tokens.Enum );
		IdentifierOrKeyword(out dummyToken);
		declaration = new Enumeration(tToken.Value);
		declaration.NameRange = tToken.Range;
		if (la.Type == Tokens.As )
		{
			Get();
			TypeReferenceExpression type = null;
			if (StartOf(11))
			{
				PrimitiveTypeName(out type);
				if (type != null)
				{
					name = type.Name;
					typeRange = type.Range;
				}
			}
			else if (StartOf(12))
			{
				VarKeyword();
				typeRange = tToken.Range;
				name = tToken.Value;
			}
			else
				SynErr(260);
			((Enumeration)declaration).UnderlyingType = name;
			((Enumeration)declaration).UnderlyingTypeRange = typeRange;
		}
		SetDefaultVisibility(declaration as AccessSpecifiedElement);
		OpenContext(declaration);
		StatementTerminatorCall();
		EnumBody();
		bool skipEnd = WaitEnumEndToken();
		GetEndToken(skipEnd);				
		SetBlockEnd(declaration); 
		if (!skipEnd)
		{
		Expect(Tokens.Enum );
		CloseContext();
		}
		SetRange(declaration, start, tToken);
	}
	void DelegateDeclaration(out LanguageElement result)
	{
		result = null;	
		SubMemberData subMember = null;
		VBDeclarator decl = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.Delegate );
		if (la.Type == Tokens.Function )
		{
			Get();
		}
		else if (la.Type == Tokens.Sub )
		{
			Get();
		}
		else
			SynErr(261);
		MethodSignature(out subMember, out decl);
		result = CreateDelegate(subMember, decl);
		result.SetRange(GetRange(startRange, tToken.Range));
		AddNode(result);
	}
	void GenericModifierRule(out GenericModifier result)
	{
		TypeParameterCollection coll = new TypeParameterCollection();
		SourceRange startRange = la.Range;
		Expect(Tokens.ParenOpen );
		Expect(Tokens.Of );
		TypeParameters(out coll);
		result = new GenericModifier(coll);
		Expect(Tokens.ParenClose );
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void ClassBaseType(TypeDeclaration result)
	{
		if (result == null)
		return;
		TypeReferenceExpression type = null;
		VBDeclarator decl = null;
		Expect(Tokens.Inherits );
		TypeName(out type, out decl);
		if (type != null)
		{
			result.PrimaryAncestorType = type;			
		}
		if (result != null && result is Class)
		{
			if (decl != null && decl.FullTypeName != null && decl.FullTypeName != String.Empty)
			{
				(result as Class).AncestorName = decl.FullTypeName;
			}
			else if (type != null && type.Name != null)
			{
				(result as Class).AncestorName = type.Name;
			}
		}
		while (la.Type == Tokens.Comma )
		{
			Get();
			TypeName(out type, out decl);
			result.AddSecondaryAncestorType(type);
		}
		StatementTerminator();
		while (la.Type == Tokens.Inherits )
		{
			Get();
			TypeName(out type, out decl);
			result.AddSecondaryAncestorType(type); 
			while (la.Type == Tokens.Comma )
			{
				Get();
				TypeName(out type, out decl);
				result.AddSecondaryAncestorType(type);
			}
			StatementTerminator();
		}
	}
	void TypeImplementsClause(TypeDeclaration result)
	{
		TypeReferenceExpression type = null;
		VBDeclarator decl = null;
		Expect(Tokens.Implements );
		TypeName(out type, out decl);
		result.AddSecondaryAncestorType(type);
		while (la.Type == Tokens.Comma )
		{
			Get();
			TypeName(out type, out decl);
			result.AddSecondaryAncestorType(type);
		}
		StatementTerminator();
		if (la.Type == Tokens.Implements )
		{
			TypeImplementsClause(result);
		}
	}
	void PrimitiveTypeName(out TypeReferenceExpression result)
	{
		result = null;
		switch (la.Type)
		{
		case Tokens.Boolean : 
		{
			Get();
			break;
		}
		case Tokens.Date : 
		{
			Get();
			break;
		}
		case Tokens.Char : 
		{
			Get();
			break;
		}
		case Tokens.String : 
		{
			Get();
			break;
		}
		case Tokens.Decimal : 
		{
			Get();
			break;
		}
		case Tokens.Byte : 
		{
			Get();
			break;
		}
		case Tokens.Short : 
		{
			Get();
			break;
		}
		case Tokens.Integer : 
		{
			Get();
			break;
		}
		case Tokens.Long : 
		{
			Get();
			break;
		}
		case Tokens.Single : 
		{
			Get();
			break;
		}
		case Tokens.Double : 
		{
			Get();
			break;
		}
		case Tokens.Object : 
		{
			Get();
			break;
		}
		default: SynErr(262); break;
		}
		string typeName = tToken.Value.Trim();
		result = new TypeReferenceExpression(typeName);
		result.SetRange(GetRange(tToken.Range));
		if (la.Type == Tokens.Question )
		{
			Get();
			TypeReferenceExpression nullableType = GetNullableType(result);
			if (nullableType != null)
				result = nullableType;
		}
	}
	void EnumBody()
	{
		while (StartOf(13))
		{
			EnumMemberDeclaration();
			if (la.Type == Tokens.Comma )
			{
				Get();
			}
			if (la.Type == Tokens.LineTerminator)
			{
			StatementTerminator();
			}
		}
	}
	void EnumMemberDeclaration()
	{
		LanguageElementCollection attributes;	
		string enumElementName = String.Empty;
		SourceRange startRange = la.Range;
		Expression exp = null;
		SourceRange nameRange = SourceRange.Empty;
		AttributeSections(out attributes);
		startRange = la.Range;
		VarKeyword();
		enumElementName = tToken.Value;
		nameRange = tToken.Range;
		if (la.Type == Tokens.EqualsSymbol )
		{
			EqualOperator();
			ExpressionRule(out exp);
		}
		EnumElement enumElement = new EnumElement(enumElementName, exp);
		enumElement.SetRange(GetRange(startRange, tToken.Range));
		enumElement.NameRange = nameRange;
		AddNode(enumElement);
		AddAttributes(enumElement, attributes);
	}
	void TypeName(out TypeReferenceExpression result, out VBDeclarator decl)
	{
		result = null;
		decl = null;
		SourceRange startTypeRange = la.Range;	
		TypeReferenceExpression parensType = null;
		TypeNameCore(out result, out decl);
		while (la.Type == Tokens.ParenOpen )
		{
			TypeParensRule(out parensType, result, decl, startTypeRange);
			if (parensType != null)
			{
				result = parensType;
			}
		}
	}
	void MethodSignature(out SubMemberData result, out VBDeclarator decl)
	{
		result = null;
		TypeReferenceExpression type = null;
		decl = null;
		LanguageElementCollection attributes = null;
		Expression initializer = null;
		SubSignature(out result);
		if (la.Type == Tokens.As )
		{
			Get();
			result.AsRange = tToken.Range;
			SourceRange startTypeRange = la.Range;
			if (la.Type == Tokens.LessThan  || la.Type == Tokens.New )
			{
				AttributeSections(out attributes);
			}
			if (la.Type == Tokens.New)
			{
			NewExpression(out initializer);
			AddAttributes(initializer, attributes);
			}
			else
			{
			TypeName(out type, out decl);
			AddAttributes(type, attributes);
			result.MemberTypeReference = type;
			result.TypeRange = GetRange(startTypeRange, tToken.Range);
			}
		}
		if (la.Type == Tokens.EqualsSymbol )
		{
			EqualOperator();
			ExpressionRule(out initializer);
		}
		if (la.Type == Tokens.Handles  || la.Type == Tokens.Implements )
		{
			HandlesOrImplement(ref result);
		}
		result.Initializer = initializer; 
	}
	void SubSignature(out SubMemberData result)
	{
		LanguageElementCollection paramColl = new LanguageElementCollection();
		result = new SubMemberData();
		result.NameRange = SourceRange.Empty;
		Token nameToken = null;
		TypeReferenceExpression typeCharacter = null;
		IdentifierOrKeywordOrOperator(out nameToken);
		if (StartOf(14))
		{
			TypeCharaterSymbol(out typeCharacter);
			result.MemberTypeReference = typeCharacter;
		}
		if (nameToken != null)
		{
			result.NameToken = nameToken;
			result.NameRange = nameToken.Range;
		}
		ResetPeek();
		Token peekToken = Peek();	
		if (peekToken != null && peekToken.Type == Tokens.Of)
		{
			GenericModifier genericModifier = null;
			GenericModifierRule(out genericModifier);
			if (genericModifier != null)
				result.GenericModifier = genericModifier;
		}
		if (la.Type == Tokens.Lib )
		{
			Get();
			Expect(Tokens.StringLiteral );
			result.LibString = tToken.Value;
		}
		if (la.Type == Tokens.Alias )
		{
			Get();
			Expect(Tokens.StringLiteral );
			result.AliasString = tToken.Value;
		}
		if (la.Type == Tokens.ParenOpen )
		{
			Get();
			result.ParamOpenRange = tToken.Range;
			if (StartOf(15))
			{
				ParameterList(out paramColl);
				result.ParamCollection = paramColl;
			}
			Expect(Tokens.ParenClose );
			result.ParamCloseRange = tToken.Range;
		}
	}
	void IdentifierOrKeywordOrOperator(out Token token)
	{
		token = null; 
		if (IsIdentifierOrKeywordOrOperator())
		{
			Get();
			token = tToken; 
		}
	}
	void TypeCharaterSymbol(out TypeReferenceExpression result)
	{
		DisableImplicitLineContinuationCheck();
		result = new TypeReferenceExpression();
		string name = String.Empty;
		result.SetRange(la.Range);
		switch (la.Type)
		{
		case Tokens.BitAnd : 
		{
			Get();
			name = "Long";
			break;
		}
		case Tokens.ExclamationSymbol : 
		{
			Get();
			name = "Single";
			break;
		}
		case Tokens.Sharp : 
		{
			Get();
			name = "Double";
			break;
		}
		case Tokens.DollarSybol : 
		{
			Get();
			name = "String";
			break;
		}
		case Tokens.CommAtSymbol : 
		{
			Get();
			name = "Decimal";
			break;
		}
		case Tokens.PercentSymbol : 
		{
			Get();
			name = "Integer";
			break;
		}
		default: SynErr(263); break;
		}
		result.Name = name;
		result.IsTypeCharacter = true;
		  RestoreImplicitLineContinuationCheck();
	}
	void ParameterList(out LanguageElementCollection coll)
	{
		LanguageElementCollection attributes = null;
		coll = new LanguageElementCollection();
		Param param = null;
		if (StartOf(15))
		{
			AttributeSections(out attributes);
		}
		ParameterRule(out param);
		if (param != null)
		{
			AddAttributes(param, attributes);
			coll.Add(param);
		}
		while (la.Type == Tokens.Comma )
		{
			Get();
			if (StartOf(15))
			{
				AttributeSections(out attributes);
			}
			ParameterRule(out param);
			if (param != null)
			{
				AddAttributes(param, attributes);
				coll.Add(param);
			}
		}
	}
	void CharsetModifier(out string result)
	{
		result = la.Value;
		if (la.Type == Tokens.Ansi )
		{
			Get();
		}
		else if (la.Type == Tokens.Auto )
		{
			Get();
		}
		else if (la.Type == Tokens.Unicode )
		{
			Get();
		}
		else
			SynErr(264);
	}
	void MethodDeclaration(out LanguageElement result, out OnDemandParsingParameters onDemandParsingParameters, MethodHelper helper)
	{
		result = null;
		SubMemberData subMember = null;
		SourceRange startRange =  la.Range;
		MethodTypeEnum methodTypeEnum = MethodTypeEnum.Void;
		Token startBlockToken = la;
		VBDeclarator decl = null;
		bool isOperator = false;
		bool isExtern = false;
		string charsetModifier = null;
		SourceRange methodKeyWordRange = SourceRange.Empty;
		onDemandParsingParameters = null;
		bool skipEnd = false;
		if (la.Type == Tokens.Declare )
		{
			Get();
			isExtern = true; 
		}
		if (la.Type == Tokens.Ansi  || la.Type == Tokens.Auto  || la.Type == Tokens.Unicode )
		{
			CharsetModifier(out charsetModifier);
		}
		if (la.Type == Tokens.Function )
		{
			Get();
			methodTypeEnum = MethodTypeEnum.Function; 
		}
		else if (la.Type == Tokens.Sub )
		{
			Get();
			methodTypeEnum = MethodTypeEnum.Void; 
		}
		else if (la.Type == Tokens.Operator )
		{
			Get();
			isOperator = true;
			methodTypeEnum = MethodTypeEnum.Function;
		}
		else
			SynErr(265);
		methodKeyWordRange = tToken.Range;
		MethodSignature(out subMember, out decl);
		if (subMember == null)
		return;
		result = CreateMethod(subMember, methodTypeEnum, null, decl, methodKeyWordRange);
		if (isOperator)
			result = SetOperatorAttributes(result as Method);
		if (((isExtern || (helper != null && helper.WithoutBody())) || (Context != null && Context is Interface)) && result != null)
		{
			if (result is Method)
				result = SetDeclareAttributes(result as Method, isExtern, charsetModifier);
			AddNode(result);
			result.SetRange(GetRange(startRange, tToken.Range));
			return;
		}
		Token startBlockTokenOnDemand = la;
		SourceRange startBlockRange = la.Range;
		Token endBlockTokenOnDemand = null;
		if (OnDemand)
		{
			if (result != null)
			{
				AddNode(result);
			}
			endBlockTokenOnDemand = SkipTokensToMethod();
			onDemandParsingParameters = new OnDemandParsingParameters(startBlockTokenOnDemand, endBlockTokenOnDemand);
		}
		else
		{
		StatementTerminator();
		if (StartOf(16))
		{
			OpenContext(result);
			Block(startBlockToken);
			CloseContext();
			SetBlockRangeForEmptyElement(result, startBlockRange, la.Range);
		}
		skipEnd = WaitMethodEndToken();
		GetEndToken(skipEnd);
		}
		if (result != null && result is DelimiterCapableBlock)
		{
			SetBlockRange(result, startRange, tToken.Range);
		}
		if (!skipEnd)
		{
		if (la.Type == Tokens.Function )
		{
			Get();
		}
		else if (la.Type == Tokens.Sub )
		{
			Get();
		}
		else if (la.Type == Tokens.Operator )
		{
			Get();
		}
		else
			SynErr(266);
		}
		if (result != null)
		{
			result.SetRange(GetRange(startRange, tToken.Range));
		}
	}
	void Block(Token startBlockToken)
	{
		SourceRange endBlockRange = SourceRange.Empty;
		BlockCore(startBlockToken, BlockType.None, out endBlockRange);
	}
	void NewExpression(out Expression result)
	{
		result = null;
		VBDeclarator decl = new VBDeclarator(CreateElementType.None);
		CreationExpressionCore(out result, ref decl);
	}
	void HandlesOrImplement(ref SubMemberData result)
	{
		if (result == null)
		return;
		if (la.Type == Tokens.Implements )
		{
			MemberImplementsClause(result);
		}
		else if (la.Type == Tokens.Handles )
		{
			HandlesClause(result);
		}
		else
			SynErr(267);
	}
	void MemberImplementsClause(SubMemberData result)
	{
		ElementReferenceExpression elementRefExp = null;
		QualifiedIdentifier identifier = null;
		Expect(Tokens.Implements );
		QualifiedIdentifier(out identifier, CreateElementType.ElementReferenceExpression);
		elementRefExp = identifier.Expression as ElementReferenceExpression;
		result.AddImplement(elementRefExp);
		while (la.Type == Tokens.Comma )
		{
			Get();
			QualifiedIdentifier(out identifier, CreateElementType.ElementReferenceExpression);
			elementRefExp = identifier.Expression as ElementReferenceExpression;
			result.AddImplement(elementRefExp);
		}
		if (la != null && la.Type == Tokens.Implements)
		{
			StatementTerminatorCall();
		if (la.Type == Tokens.Implements )
		{
			MemberImplementsClause(result);
		}
		}
	}
	void HandlesClause(SubMemberData result)
	{
		Expression exp = null;
		Expect(Tokens.Handles );
		if (StartOf(17))
		{
			PrimaryExpression(out exp);
			result.HandlesExpressions.Add(exp);
		}
		while (la.Type == Tokens.Comma )
		{
			Get();
			if (StartOf(17))
			{
				PrimaryExpression(out exp);
				result.HandlesExpressions.Add(exp);
			}
		}
	}
	void PrimaryExpression(out Expression result)
	{
		result = null;
		PrimaryExpressionCore(out result, true);
	}
	void AttributesAndModifiers(out LanguageElementCollection attributes, out TokenQueueBase modifiers)
	{
		attributes = null;
		modifiers = null;
		if (GetMissedAttributesAndModifiers(out attributes, out modifiers))
			return;
		AttributeSections(out attributes);
		MemberModifiers(out modifiers);
	}
	void MemberModifiers(out TokenQueueBase modifiers)
	{
		modifiers = new TokenQueueBase();
		Token modifier = null;
		while (StartOf(18))
		{
			if (StartOf(19))
			{
				ProcedureModifier(out modifier);
			}
			else
			{
				Get();
				modifier = tToken;
			}
			if (modifier != null)
			modifiers.Enqueue(modifier);
		}
		if (IsAsyncModifier())
		{
			Expect(Tokens.Identifier );
			modifiers.Enqueue(tToken); SetKeywordTokenCategory(tToken); 
		}
		if (la.Type == Tokens.Identifier && la.Value.ToLower() == "iterator")
		{
			Expect(Tokens.Identifier );
			modifiers.Enqueue(tToken); SetKeywordTokenCategory(tToken); 
		}
		while (StartOf(18))
		{
			if (StartOf(19))
			{
				ProcedureModifier(out modifier);
			}
			else
			{
				Get();
				modifier = tToken;
			}
			if (modifier != null)
			modifiers.Enqueue(modifier);
		}
	}
	void DeclareVariableList(out LanguageElementCollection coll, bool isConst, bool addToContext)
	{
		DeclaratorType declaratorType = DeclaratorType.Dim;
		coll = null;
		SourceRange startRange = SourceRange.Empty;
		LanguageElement result = null;
		if (la != null)
			startRange = la.Range;
		if (StartOf(20))
		{
			DeclaratorQualifier(out declaratorType);
		}
		if (isConst)
		{
			declaratorType = DeclaratorType.Const;
		}
		DeclareVariableWithOneQualifier(out result, startRange, declaratorType, addToContext);
		coll = CreateVarNextColl(result as Variable);
	}
	void PropertyDeclaration(out LanguageElement result, out LanguageElementCollection accessors, out OnDemandParsingParameters onDemandParsingParameters, MethodHelper helper)
	{
		result = null;
		VBProperty property = null;
		SubMemberData subData = null;
		VBDeclarator decl = null;
		SourceRange startRange = la.Range;	
		accessors = new LanguageElementCollection();
		onDemandParsingParameters = null;
		bool skipEnd = false;
		bool isAuto = false;
		Expect(Tokens.Property );
		MethodSignature(out subData, out decl);
		property = CreateProperty(subData, null, decl);
		property.Name = subData.Name;
		if (InInterface() || (helper != null && helper.WithoutBody()))
		{
			AddNode(property);
			property.SetRange(GetRange(startRange, tToken.Range));
			result = property;
			return;
		}
		Token startBlockTokenOnDemand = la;	
		SourceRange blockStartRange = la.Range;					
		SourceRange blockEndRange = la.Range;
		Token endBlockTokenOnDemand = null;
		SourceRange signatureRangeEnd = tToken.Range;
		if (OnDemand)
		{
			if (property != null)
			{
				AddNode(property);
			}
			endBlockTokenOnDemand = SkipTokensToProperty();
			onDemandParsingParameters = new OnDemandParsingParameters(startBlockTokenOnDemand, endBlockTokenOnDemand);
		}
		else
		{
			OpenContext(property);
			StatementTerminator();
		PropertyAccessorDeclarationList(accessors, subData.Name, ref blockEndRange);
		CloseContext();
		isAuto = MakePropertyAutoImplementedIfNeeded(property, subData);
		if (isAuto && !HasAutoImplementedPropertyEnd())
		{
			result = property;
			result.SetRange(GetRange(startRange, signatureRangeEnd));
			return;
		}
		if (!isAuto)
		{
			SetBlockRangeForEmptyElement(property, blockStartRange, la.Range);
			skipEnd = WaitPropertyEndToken();
		}
		GetEndToken(skipEnd);
		}
		if (!isAuto)
			SetBlockEnd(property);
		if (!skipEnd)
		{
		Expect(Tokens.Property );
		}
		if (property != null)
		{
			property.SetRange(GetRange(startRange, tToken.Range));
			result = property;
		}
	}
	void CustomEventMemberDeclaration(out LanguageElement result)
	{
		result = null;
		SourceRange startRange = la.Range;	
		SubMemberData subMember = null;
		VBDeclarator decl = null;
		bool isCustom = false;
		if (la.Type == Tokens.Custom )
		{
			Get();
			isCustom = true;
		}
		Expect(Tokens.Event );
		MethodSignature(out subMember, out decl);
		result = CreateEvent(subMember, decl);
		if (result == null || !(result is Event))
			return;
		if (isCustom)
		{
			OpenContext(result);
		StatementTerminator();
		EventAccessorDeclarations();
		bool skipEnd = WaitEventEndToken();
		GetEndToken(skipEnd);
		SetBlockEnd(result);
		if (!skipEnd)
		{
			CloseContext();
		Expect(Tokens.Event );
		}
		}
		else
		{
			AddNode(result);
		}
			result.SetRange(GetRange(startRange, tToken.Range));
	}
	void PropertyAccessorDeclarationList(LanguageElementCollection accessors, string name, ref SourceRange blockEndRange)
	{
		_DeclarationsCache.OpenScope(); 
		while (StartOf(21))
		{
			PropertyAccessorDeclaration(accessors, name);
			if (accessors == null || accessors.Count == 0)
			return;
			blockEndRange = tToken.Range;
			StatementTerminator();
		}
		_DeclarationsCache.CloseScope(); 
	}
	void PropertyAccessorDeclaration(LanguageElementCollection accessors, string name)
	{
		LanguageElementCollection attributes = null;
		PropertyAccessor accessor = null;
		TokenQueueBase modifiers = null;
		AttributesAndModifiers(out attributes, out modifiers);
		SetMissedAttributesAndModifiersIfNeeded(attributes, modifiers);
		if (!LaIsStartPropertyAccessor())
			return;
		if (la.Type == Tokens.Get )
		{
			PropertyGetDeclaration(out accessor);
			accessor.Name = GetAccessorName("get_");
			accessors.Add(accessor);
		}
		else if (la.Type == Tokens.Set )
		{
			PropertySetDeclaration(out accessor);
			accessor.Name = GetAccessorName("set_");
			accessors.Add(accessor);
		}
		else
			SynErr(268);
		if (accessor != null)
		SetAttributesAndModifiers(accessor, attributes, modifiers);
	}
	void PropertyGetDeclaration(out PropertyAccessor result)
	{
		Token startBlockToken = la;
		SourceRange startRange = la.Range;
		Get getter = new Get();
		Expect(Tokens.Get );
		StatementTerminator();
		OpenContext(getter);
		Block(startBlockToken);
		CloseContext();
		bool skipEnd = WaitGetEndToken();
		if (!skipEnd)
		{
		Expect(Tokens.EndToken );
		Expect(Tokens.Get );
		}
		getter.NameRange = startRange;
		result = getter;
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void PropertySetDeclaration(out PropertyAccessor result)
	{
		Token startBlockToken = la;
		SourceRange startRange = la.Range;
		Set setter = new Set();
		Expect(Tokens.Set );
		if (la.Type == Tokens.ParenOpen )
		{
			Get();
			if (StartOf(15))
			{
				LanguageElementCollection parameterList = null;
				SourceRange parenOpenRange = tToken.Range;
				ParameterList(out parameterList);
				if ( parameterList != null)
				{
					foreach (LanguageElement element in parameterList)
					{
						PassAttributes(setter, element as CodeElement);
					}
					setter.Parameters = parameterList;
					SetParensRanges(setter, parenOpenRange, la.Range);
				}
			}
			Expect(Tokens.ParenClose );
		}
		StatementTerminator();
		OpenContext(setter);
		Block(startBlockToken);
		CloseContext();
		bool skipEnd = WaitSetEndToken();
		if (!skipEnd)
		{
		Expect(Tokens.EndToken );
		Expect(Tokens.Set );
		}
		result = setter;
		setter.NameRange = startRange;
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void EventAccessorDeclarations()
	{
		LanguageElement accessor = null;
		_DeclarationsCache.OpenScope(); 
		while (StartOf(22))
		{
			EventAccessorDeclaration(out accessor);
			StatementTerminator();
		}
		_DeclarationsCache.CloseScope(); 
	}
	void EventAccessorDeclaration(out LanguageElement result)
	{
		result = null;
		LanguageElementCollection attributes = null;
		TokenQueueBase modifiers = null;
		AttributesAndModifiers(out attributes, out modifiers);
		if (la.Type == Tokens.AddHandler )
		{
			AddHandlerDeclaration(out result);
		}
		else if (la.Type == Tokens.RemoveHandler )
		{
			RemoveHandlerDeclaration(out result);
		}
		else if (la.Type == Tokens.RaiseEvent )
		{
			RaiseEventDeclaration(out result);
		}
		else
			SynErr(269);
		if (result != null)
		{
			SetAttributesAndModifiers(result, attributes, modifiers);
		}
	}
	void AddHandlerDeclaration(out LanguageElement result)
	{
		result = new EventAdd();
		Token startBlockToken = la;
		SourceRange startRange = la.Range;
		Expect(Tokens.AddHandler );
		EventDeclarationParameters(result as EventAccessor);
		StatementTerminator();
		OpenContext(result);
		Block(startBlockToken);
		CloseContext();
		bool skipEnd = WaitAddHandlerEndToken();
		if (!skipEnd)
		{
		Expect(Tokens.EndToken );
		Expect(Tokens.AddHandler );
		}
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void RemoveHandlerDeclaration(out LanguageElement result)
	{
		result = new EventRemove();
		Token startBlockToken = la;
		SourceRange startRange = la.Range;
		Expect(Tokens.RemoveHandler );
		EventDeclarationParameters(result as EventAccessor);
		StatementTerminator();
		OpenContext(result);
		Block(startBlockToken);
		CloseContext();
		bool skipEnd = WaitRemoveHandlerEndToken();
		if (!skipEnd)
		{
		Expect(Tokens.EndToken );
		Expect(Tokens.RemoveHandler );
		}
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void RaiseEventDeclaration(out LanguageElement result)
	{
		result = new EventRaise();
		Token startBlockToken = la;
		SourceRange startRange = la.Range;
		Expect(Tokens.RaiseEvent );
		EventDeclarationParameters(result as EventAccessor);
		StatementTerminator();
		OpenContext(result);
		Block(startBlockToken);
		CloseContext();
		bool skipEnd = WaitRaiseEventEndToken();
		if (!skipEnd)
		{
		Expect(Tokens.EndToken );
		Expect(Tokens.RaiseEvent );
		}
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void EventDeclarationParameters(EventAccessor accessor)
	{
		if (accessor == null)
		return;
		SourceRange parenOpen = la.Range;
		Expect(Tokens.ParenOpen );
		if (StartOf(15))
		{
			LanguageElementCollection parameterList = null;
			ParameterList(out parameterList);
			if ( parameterList != null)
			{
				foreach (LanguageElement element in parameterList)
				{
					PassAttributes(accessor, element as CodeElement);
				}
				accessor.Parameters = parameterList;
			}
		}
		Expect(Tokens.ParenClose );
		SetParensRanges(accessor, parenOpen, tToken.Range);
	}
	void VarIdentifierCollection(out VariableNameCollection varColl)
	{
		varColl = new VariableNameCollection();
		SourceRange startRange = la.Range;
		string name = la.Value;
		VBDeclarator decl = null;
		VarIdentifier(out decl);
		varColl.Add(name, decl, GetRange(startRange, tToken.Range), startRange);
		while (la.Type == Tokens.Comma )
		{
			Get();
			name = la.Value;
			startRange = la.Range;
			VarIdentifier(out decl);
			varColl.Add(name, decl, GetRange(startRange, tToken.Range), startRange);
		}
	}
	void VarIdentifier(out VBDeclarator result)
	{
		ArrayNameModifier arrayModifier = null;
		result = null;
		VarIdentifierOrKeyword();
		if (StartOf(14))
		{
			TypeCharacterRule(out result);
		}
		else if (StartOf(4))
		{
			if (la.Type == Tokens.Question )
			{
				Get();
				result = new VBDeclarator();
				NullableTypeModifier nullableModifier = new NullableTypeModifier();
				nullableModifier.SetRange(tToken.Range);
				nullableModifier.NameRange = tToken.Range;
				result.NullableModifier = nullableModifier;
			}
			while (la.Type == Tokens.ParenOpen )
			{
				ArrayNameModifier(out arrayModifier);
				if (result == null)
				{
					result = new VBDeclarator();
				}
				result.AddArrayModifier(arrayModifier);
			}
		}
		else
			SynErr(270);
	}
	void VarIdentifierOrKeyword()
	{
		if (StartOf(12))
		{
			VarKeyword();
			return; 
		}
		else if (la.Type == Tokens.Until  || la.Type == Tokens.From  || la.Type == Tokens.Aggregate )
		{
			if (la.Type == Tokens.From )
			{
				Get();
			}
			else if (la.Type == Tokens.Aggregate )
			{
				Get();
			}
			else
			{
				Get();
			}
			return; 
		}
		else
			SynErr(271);
		if (IsIdentifierOrKeyword())
		{
			Get();
		}
	}
	void TypeCharacterRule(out VBDeclarator result)
	{
		result = new VBDeclarator();
		TypeReferenceExpression type = null;	
		SourceRange startRange = la.Range;
		TypeReferenceExpression parensType = null;
		TypeCharaterSymbol(out type);
		if (la.Type == Tokens.Question )
		{
			TypeReferenceExpression nullableType = null; 
			NullableTypeRule(out nullableType, type);
			if (nullableType != null)
			type = nullableType;
		}
		while (la.Type == Tokens.ParenOpen )
		{
			TypeParensRule(out parensType, type, result, startRange);
			if (parensType != null)
			{
				type = parensType;
			}
		}
		result.CharacterType = type;
	}
	void ArrayNameModifier(out ArrayNameModifier result)
	{
		Expression num = null;
		ExpressionCollection expColl = new ExpressionCollection();
		int rank = 1;	
		SourceRange startRange = la.Range;
		Expect(Tokens.ParenOpen );
		while (StartOf(23))
		{
			if (la.Type == Tokens.Comma )
			{
				Get();
				rank++;
			}
			BoundsElement(out num);
			if (num != null)
			expColl.Add(num);
		}
		Expect(Tokens.ParenClose );
		result = new ArrayNameModifier(rank, expColl);
		result.SetRange(GetRange(startRange, tToken.Range));
		result.NameRange = result.Range;
	}
	void NullableTypeRule(out TypeReferenceExpression result, TypeReferenceExpression type)
	{
		result = GetNullableType(type, la);
		Expect(Tokens.Question );
	}
	void TypeParensRule(out TypeReferenceExpression result, TypeReferenceExpression type, VBDeclarator decl, SourceRange startRange)
	{
		int rank;
		result = null;
		ExpressionCollection arguments = new ExpressionCollection();
		ArrayOrCreationParenthesis(out arguments, out rank);
		SourceRange endTypeRange = tToken.Range;
		result = GetArrayTypeReference(type, arguments, rank);
		if (decl != null)
		{
			for (int i = 0; i < rank; i++)
				decl.FullTypeName += "()";
		}
		result.SetRange(GetRange(startRange, endTypeRange));
	}
	void BoundsElement(out Expression result)
	{
		result = null;
		ExpressionRule(out result);
		if (la.Type == Tokens.ToToken )
		{
			Get();
			ExpressionRule(out result);
		}
	}
	void BlockCore(Token startBlockToken, BlockType blockType, out SourceRange endBlockRange)
	{
		endBlockRange = SourceRange.Empty;
		BlockRule(startBlockToken.Range, blockType, false, out endBlockRange);
	}
	void BlockRule(SourceRange startBlockRange, BlockType blockType, bool skipSetBounds, out SourceRange endBlockRange)
	{
		Statement statementTemp = null;
		endBlockRange = SourceRange.Empty;
		SetHasBlockAndBlockType();
		if (blockType != BlockType.IfSingleLineStatement && !skipSetBounds)
		  ReadBlockStartVB(startBlockRange);
		_DeclarationsCache.OpenScope(); 
		if (Context != null && Context.ElementType == LanguageElementType.Method)
		{
		  Method method = Context as Method;
		  foreach(BaseVariable variable in method.Parameters)
			AddVarToCache(variable);
		}
		while (StartOf(16))
		{
			StatementRule(out statementTemp);
			if (statementTemp != null)
			{
				endBlockRange = statementTemp.Range;
			}
			if (la.Type == Tokens.LineTerminator &&
				blockType == BlockType.IfSingleLineStatement && !skipSetBounds)
			{
			_DeclarationsCache.CloseScope();
				return;
			}
			if (NeedExitFromStatementLoop())
			{
				break;
			}
			   if (IsRazorEndCode)
			   {
				 IsRazorEndCode = false;
				 break;
			   }
			StatementTerminatorCall();
		}
		_DeclarationsCache.CloseScope(); 
		if (blockType != BlockType.IfSingleLineStatement && !skipSetBounds)	
			ReadBlockEnd(la.Range); 
	}
	void StatementRule(out Statement statement)
	{
		statement = null;	
		Token startToken = la;
		if (IsLabel())
		{
			LabelRule(out statement);
		}
		else if (IsRazorMarkup())
		{
			RazorMarkup();
		}
		else if (IsEndStatement())
		{
			EndStatement(out statement);
		}
		else if (StartOf(20))
		{
			LocalDeclarationStatement();
		}
		else if (StartOf(24))
		{
			EmbeddedStatement(out statement);
		}
		else
			SynErr(272);
		if (startToken == la && la.Value == "@" && !ParsingRazor)
		 Get();
	}
	void LabelRule(out Statement result)
	{
		result = null;
		LabelDeclarationStatement(out result);
		Expect(Tokens.Colon );
		if (result != null)
		{
			result.SetRange(GetRange(result.Range, tToken.Range));
			AddNode(result);
		}
	}
	void RazorMarkup()
	{
		if (!ParsingRazor)
		{
		  Get();
		  return;
		}
		ParseRazorMarkup();
		Expect(Tokens.CommAtSymbol );
	}
	void EndStatement(out Statement result)
	{
		result = new End();
		result.SetRange(la.Range);
		AddNode(result);
		Expect(Tokens.EndToken );
		if (IsRazorCodeKeyword(la))
		{
		  Get();
		  IsRazorEndCode = true;
		  result.SetRange(GetRange(result, tToken));
		}
	}
	void LocalDeclarationStatement()
	{
		LanguageElement variable = null;
		LocalDeclarationStatementCore(out variable, true);
	}
	void EmbeddedStatement(out Statement result)
	{
		result = null;
		switch (la.Type)
		{
		case Tokens.IntegerLiteral : 
case Tokens.FloatingPointLiteral : 
case Tokens.Identifier : 
case Tokens.CharacterLiteral : 
case Tokens.StringLiteral : 
case Tokens.ShiftLeft : 
case Tokens.CurlyBraceOpen : 
case Tokens.ParenOpen : 
case Tokens.Dot : 
case Tokens.Plus : 
case Tokens.Minus : 
case Tokens.LessThan : 
case Tokens.Sharp : 
case Tokens.AddressOf : 
case Tokens.Boolean : 
case Tokens.Byte : 
case Tokens.Call : 
case Tokens.CBool : 
case Tokens.CByte : 
case Tokens.CChar : 
case Tokens.CDate : 
case Tokens.CDbl : 
case Tokens.CDec : 
case Tokens.Char : 
case Tokens.CInt : 
case Tokens.CLng : 
case Tokens.CObj : 
case Tokens.Continue : 
case Tokens.CSByte : 
case Tokens.CShort : 
case Tokens.CSng : 
case Tokens.CStr : 
case Tokens.CType : 
case Tokens.CUInt : 
case Tokens.CULng : 
case Tokens.CUShort : 
case Tokens.Date : 
case Tokens.Decimal : 
case Tokens.DirectCast : 
case Tokens.Double : 
case Tokens.False : 
case Tokens.Function : 
case Tokens.GetTypeToken : 
case Tokens.Global : 
case Tokens.IfToken : 
case Tokens.Out : 
case Tokens.Integer : 
case Tokens.IsNot : 
case Tokens.IsFalse : 
case Tokens.IsTrue : 
case Tokens.Long : 
case Tokens.Me : 
case Tokens.MyBase : 
case Tokens.MyClass : 
case Tokens.Narrowing : 
case Tokens.New : 
case Tokens.Not : 
case Tokens.Nothing : 
case Tokens.Object : 
case Tokens.Of : 
case Tokens.Operator : 
case Tokens.Partial : 
case Tokens.SByte : 
case Tokens.Short : 
case Tokens.Single : 
case Tokens.String : 
case Tokens.Sub : 
case Tokens.True : 
case Tokens.TryCast : 
case Tokens.UInteger : 
case Tokens.ULong : 
case Tokens.UShort : 
case Tokens.Using : 
case Tokens.Until : 
case Tokens.AddToken : 
case Tokens.RemoveToken : 
case Tokens.Ansi : 
case Tokens.Assembly : 
case Tokens.Auto : 
case Tokens.Unicode : 
case Tokens.Explicit : 
case Tokens.Strict : 
case Tokens.Compare : 
case Tokens.Binary : 
case Tokens.Text : 
case Tokens.Off : 
case Tokens.Custom : 
case Tokens.From : 
case Tokens.Where : 
case Tokens.Join : 
case Tokens.EqualsToken : 
case Tokens.Into : 
case Tokens.Order : 
case Tokens.By : 
case Tokens.Group : 
case Tokens.Ascending : 
case Tokens.Descending : 
case Tokens.Question : 
case Tokens.Distinct : 
case Tokens.Infer : 
case Tokens.KeyToken : 
case Tokens.Aggregate : 
case Tokens.Skip : 
case Tokens.Take : 
		{
			if (!IsUsing() && !IsContinue() && !IsIfStatement() && !IsYieldStatement())
			{
			AssignmentStatement(out result);
			AddNode(result);
			}
			else if (IsIfStatement())
			{
			IfStatement(out result);
			}
			else if (IsUsing())
			{
			UsingStatementRule(out result);
			}
			else if (IsContinue())
			{
			ContinueStatement(out result);
			}
			   else if (IsYieldStatement())
			   {
			YieldStatement(out result);
			}
			else
			{
				AssignmentStatement(out result);
			}
			break;
		}
		case Tokens.Exit : 
		{
			ExitStatement(out result);
			break;
		}
		case Tokens.Try : 
		{
			TryStatement();
			break;
		}
		case Tokens.Throw : 
		{
			ThrowStatement(out result);
			break;
		}
		case Tokens.Return : 
		{
			ReturnStatement(out result);
			break;
		}
		case Tokens.SyncLock : 
		{
			SyncLockStatement(out result);
			break;
		}
		case Tokens.With : 
		{
			WithStatement(out result);
			break;
		}
		case Tokens.While : 
		{
			WhileStatement(out result);
			break;
		}
		case Tokens.Do : 
		{
			DoLoopStatement(out result);
			break;
		}
		case Tokens.For : 
		{
			ForStatement(out result);
			break;
		}
		case Tokens.Stop : 
		{
			StopStatement(out result);
			break;
		}
		case Tokens.Select : 
		{
			SelectStatement(out result);
			break;
		}
		case Tokens.GoTo : 
		{
			GoToStatement(out result);
			break;
		}
		case Tokens.Error : 
case Tokens.On : 
case Tokens.Resume : 
		{
			UnstructuredErrorStatement(out result);
			break;
		}
		case Tokens.Erase : 
case Tokens.ReDim : 
		{
			ArrayHandlingStatement(out result);
			break;
		}
		case Tokens.RaiseEvent : 
		{
			RaiseEventStatement(out result);
			break;
		}
		case Tokens.AddHandler : 
case Tokens.RemoveHandler : 
		{
			AddHandlerStatement(out result);
			break;
		}
		default: SynErr(273); break;
		}
	}
	void LabelDeclarationStatement(out Statement result)
	{
		result = null;
		LabelName();
		result = new Label();
		result.Name = tToken.Value;
		result.NameRange = tToken.Range;
		result.SetRange(GetRange(tToken.Range));
	}
	void LabelName()
	{
		if (StartOf(12))
		{
			VarKeyword();
		}
		else if (la.Type == Tokens.IntegerLiteral )
		{
			Get();
		}
		else
			SynErr(274);
	}
	void SyncLockStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		Expression statementExpression = null;
		Token startBlockToken = la;
		Expect(Tokens.SyncLock );
		ExpressionRule(out statementExpression);
		StatementTerminator();
		result = new Lock();
		result.AddDetailNode(statementExpression);
		((Lock)result).Expression = statementExpression;
		OpenContext(result);
		Block(startBlockToken);
		Expect(Tokens.EndToken );
		Expect(Tokens.SyncLock );
		result.SetRange(GetRange(startRange, tToken.Range));
		CloseContext();
	}
	void LocalDeclarationStatementCore(out LanguageElement result, bool addToContext)
	{
		SourceRange startRange = SourceRange.Empty;
		result = null;
		DeclaratorType declaratorType = DeclaratorType.Dim;
		if (la != null)
				startRange = la.Range;
		DeclaratorQualifier(out declaratorType);
		DeclareVariableWithOneQualifier(out result, startRange, declaratorType, addToContext);
		if (result != null)
		{
			result.SetRange(GetRange(startRange, result.Range));
		}
		if (la.Type == Tokens.LineTerminator  || la.Type == Tokens.Colon )
		{
			StatementTerminator();
		}
	}
	void DeclaratorQualifier(out DeclaratorType type)
	{
		type = DeclaratorType.None;
		if (la.Type == Tokens.Const )
		{
			Get();
			type = DeclaratorType.Const;
		}
		else if (la.Type == Tokens.Dim )
		{
			Get();
			type = DeclaratorType.Dim;
		}
		else if (la.Type == Tokens.Shared  || la.Type == Tokens.Static )
		{
			if (la.Type == Tokens.Static )
			{
				Get();
			}
			else
			{
				Get();
			}
			type = DeclaratorType.Static;
		}
		else
			SynErr(275);
		while (la.Type == Tokens.Dim )
		{
			Get();
			if (type == DeclaratorType.None)
			{
				type = DeclaratorType.Dim;
			}
		}
	}
	void DeclareVariableWithOneQualifier(out LanguageElement variable, SourceRange startRange, DeclaratorType declaratorType, bool addToContext)
	{
		variable = null;
		if (startRange == SourceRange.Empty)
		{
			startRange = la.Range;
		}
		Variable prevVar = null;
		Variable endVar = null;
		VariableDeclaratorCore(out prevVar, out endVar, declaratorType, addToContext);
		variable = prevVar;
		if (variable != null)
			variable.SetRange(GetRange(startRange, variable.Range));
		prevVar = endVar;
		while (la.Type == Tokens.Comma )
		{
			Variable nextPrevVariable = null;
			Get();
			VariableDeclaratorCore(out nextPrevVariable, out endVar, declaratorType, addToContext);
			if (nextPrevVariable != null && prevVar != null)
			{
				prevVar.SetNextVariable(nextPrevVariable);
				nextPrevVariable.SetPreviousVariable(prevVar);
			}
			prevVar = endVar;
		}
	}
	void VariableDeclaratorCore(out Variable var, out Variable endVar, DeclaratorType declaratorType, bool addToContext)
	{
		var = null;
		VBDeclarator decl = null;
		VariableNameCollection varColl = null;
		Token weToken = null;
		SourceRange asRange = SourceRange.Empty;
		Expression initializer = null;
		VariableListHelper varListHelper = new VariableListHelper(addToContext);
		if (la.Type == Tokens.WithEvents )
		{
			Get();
			weToken = tToken;
		}
		VarIdentifierCollection(out varColl);
		if (la.Type == Tokens.As )
		{
			Get();
			varListHelper.AsRange = tToken.Range; 
		}
		if (la.Type == Tokens.New)
		{
			ObjectCreationExpression(out initializer, out decl);
			varListHelper.Initializer = initializer;
			varListHelper.IsObjectCreation = true;
		}
		else if (StartOf(4))
		{
			if (StartOf(4))
			{
				if(IsIdentifierOrKeyword())
				{
					TypeReferenceExpression type = null;
				TypeName(out type, out decl);
				varListHelper.Type = type;
				}
			}
			if (la.Type == Tokens.EqualsSymbol )
			{
				EqualOperator();
				varListHelper.OperatorRange = tToken.Range;
				ExpressionRule(out initializer);
				varListHelper.Initializer = initializer; 
			}
		}
		else
			SynErr(276);
		var = CreateVariableList(out endVar, varColl, decl, declaratorType, varListHelper);
		SetWithEvents(var, weToken);
	}
	void ObjectCreationExpression(out Expression result, out VBDeclarator decl)
	{
		result = null;
		decl = null;
		ObjectCreationExpressionCore(out result, out decl);
	}
	void WhileStatement(out Statement @while)
	{
		While result = new While();
		SourceRange startRange = la.Range;
		Expression condition = null;
		Token startBlockToken = la;
		Expect(Tokens.While );
		ExpressionRule(out condition);
		result.SetCondition(condition);
		OpenContext(result);
		StatementTerminator();
		Block(startBlockToken);
		Expect(Tokens.EndToken );
		Expect(Tokens.While );
		CloseContext();
		result.SetRange(GetRange(startRange, tToken.Range));
		@while = result;
	}
	void ContinueStatement(out Statement result)
	{
		ContinueKind continueKind = ContinueKind.Unknown;
		SourceRange startRange = la.Range;
		Expect(Tokens.Continue );
		if (la.Type == Tokens.Do )
		{
			Get();
			continueKind = ContinueKind.Do;
		}
		else if (la.Type == Tokens.For )
		{
			Get();
			continueKind = ContinueKind.For;
		}
		else if (la.Type == Tokens.While )
		{
			Get();
			continueKind = ContinueKind.While;
		}
		else
			SynErr(277);
		result = new VBContinue(continueKind);
		result.SetRange(GetRange(startRange, tToken.Range));
		AddNode(result);
	}
	void ExitStatement(out Statement result)
	{
		Exit @exit = new Exit();
		SourceRange startRange = la.Range;
		ExitKind exitKind = ExitKind.Sub;
		Expect(Tokens.Exit );
		switch (la.Type)
		{
		case Tokens.Sub : 
		{
			Get();
			break;
		}
		case Tokens.Function : 
		{
			Get();
			exitKind = ExitKind.Function; 
			break;
		}
		case Tokens.Property : 
		{
			Get();
			exitKind = ExitKind.Property; 
			break;
		}
		case Tokens.Do : 
		{
			Get();
			exitKind = ExitKind.Do; 
			break;
		}
		case Tokens.For : 
		{
			Get();
			exitKind = ExitKind.For; 
			break;
		}
		case Tokens.Try : 
		{
			Get();
			exitKind = ExitKind.Try; 
			break;
		}
		case Tokens.While : 
		{
			Get();
			exitKind = ExitKind.While; 
			break;
		}
		case Tokens.Select : 
		{
			Get();
			exitKind = ExitKind.Select; 
			break;
		}
		default: SynErr(278); break;
		}
		@exit.SetRange(GetRange(startRange, tToken.Range));
		@exit.ExitKind = exitKind;
		result = @exit;
		AddNode(result);
	}
	void WithStatement(out Statement result)
	{
		result = null;
		Expression objectElement = null;
		SourceRange startRange = la.Range;
		Token startBlockToken = la;
		Expect(Tokens.With );
		DisableImplicitLineContinuationCheck();
		ExpressionRule(out objectElement);
		RestoreImplicitLineContinuationCheck();
		StatementTerminator();
		result = new With();
		((With)result).Expression = objectElement;
		result.AddDetailNode(objectElement);
		OpenContext(result);
		Block(startBlockToken);
		Expect(Tokens.EndToken );
		Expect(Tokens.With );
		CloseContext();
		result.SetRange(GetRange(startRange, tToken.Range));		
	}
	void TryBlock(out Statement result)
	{
		result = new Try();
		SourceRange startRange = la.Range;	
		Token startBlockToken = la;
		Expect(Tokens.Try );
		StatementTerminator();
		OpenContext(result);
		Block(startBlockToken);
		CloseContext();
		result.SetRange(new SourceRange(startRange.Start, la.Range.Start));
	}
	void CatchExceptionDeclaration(Catch result)
	{
		if ( result == null || la == null || 
		  la.Type == Tokens.When || la.Type == Tokens.LineTerminator)
		return;
		LanguageElement var = null;
		DeclaratorType dummyDeclType;
		if (StartOf(20))
		{
			DeclaratorQualifier(out dummyDeclType);
		}
		DeclareVariableAssignDim(out var);
		StatementTerminatorCall();
		if (var != null)
		{
			result.Exception = var;
			result.AddDetailNode(var);
		}
	}
	void DeclareVariableAssignDim(out LanguageElement result)
	{
		result = null;
		Expression exp = null;
		if (IsDeclareVariableCoreWithDecloratorType() || IsImplicitVar())
		{
			BaseVariable var = null;
			DeclVariableCoreWithoutSpecifier(out var, DeclaratorType.Dim);
			result = var;
		}
		else if (StartOf(4))
		{
			ElementReferenceQualifiedIdentifier(out exp);
			result = exp;
		}
		else
			SynErr(279);
	}
	void CatchCondition(Catch result)
	{
		if (result == null || la == null)
		return;
		Expression condition = null;
		Expect(Tokens.When );
		ExpressionRule(out condition);
		result.Condition = condition;
		result.AddDetailNode(condition);
	}
	void CatchBlock(out Statement result)
	{
		result = new Catch();
		SourceRange startRange = la.Range;
		Token startBlockToken = la;
		Expect(Tokens.Catch );
		if (StartOf(4))
		{
			CatchExceptionDeclaration(result as Catch);
		}
		if (la.Type == Tokens.When )
		{
			CatchCondition(result as Catch);
		}
		StatementTerminatorCall();
		OpenContext(result);
		Block(startBlockToken);
		CloseContext();
		result.SetRange(new SourceRange(startRange.Start, la.Range.Start));
	}
	void FinallyBlock(out Statement result)
	{
		result = new Finally();
		SourceRange startRange = la.Range;
		Token startBlockToken = la;
		Expect(Tokens.Finally );
		OpenContext(result);
		StatementTerminator();
		Block(startBlockToken);
		CloseContext();
		result.SetRange(new SourceRange(startRange.Start, la.Range.Start));
	}
	void TryStatement()
	{
		Statement result = null;
		TryBlock(out result);
		while (la.Type == Tokens.Catch )
		{
			CatchBlock(out result);
		}
		if (la.Type == Tokens.Finally )
		{
			FinallyBlock(out result);
		}
		Expect(Tokens.EndToken );
		Expect(Tokens.Try );
		SourceRange endRange = tToken.Range;
		result.SetEnd(endRange.End);
	}
	void DoLoopStatement(out Statement result)
	{
		Do @do = new Do();
		@do.PreCondition = true;
		DoConditionType conditionType = DoConditionType.While;
		Expression condition;
		SourceRange startRange = la.Range;
		Token startBlockToken = la;
		Expect(Tokens.Do );
		if (la.Type == Tokens.Until  || la.Type == Tokens.While )
		{
			WhileOrUntil(out conditionType);
			ExpressionRule(out condition);
			@do.SetCondition(condition);
			OpenContext(@do);
			StatementTerminator();
			Block(startBlockToken);
			CloseContext();
			Expect(Tokens.Loop );
		}
		else if (StartOf(25))
		{
			StatementTerminatorCall();
			OpenContext(@do);
			Block(startBlockToken);
			CloseContext();
			Expect(Tokens.Loop );
			WhileOrUntil(out conditionType);
			ExpressionRule(out condition);
			@do.PreCondition = false;
			@do.SetCondition(condition);
		}
		else
			SynErr(280);
		@do.ConditionType = conditionType;
		result = @do;
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void WhileOrUntil(out DoConditionType result)
	{
		result = DoConditionType.While;
		if (la.Type == Tokens.While )
		{
			Get();
		}
		else if (la.Type == Tokens.Until )
		{
			Get();
			result = DoConditionType.Until;
		}
		else
			SynErr(281);
	}
	void LoopControlVariable(out LanguageElement result)
	{
		result = null;
		if (IsVariableInLoopControlVariable() || IsImplicitVar())
		{
			DeclareVariableAssignDim(out result);
		}
		else if (StartOf(17))
		{
			Expression assignment = null;
			AssignmentExpressionRule(out assignment);
			result = assignment;
		}
		else
			SynErr(282);
	}
	void AssignmentExpressionRule(out Expression result)
	{
		result = null;
		if (!IsIdentifierOrKeyword())
		{	
			ExpressionRule(out result);
			return;
		}
		Expression exp = null;
		string operatorText = String.Empty;
		AssignmentOperatorType assignmentType = AssignmentOperatorType.None;
		SourceRange startRange = la.Range;
		PrimaryExpressionCore(out result, false);
		if (!IsAssignmentOperator(la))
		return;		
		SourceRange operatorRange = la.Range;
		AssignmentOperator(out assignmentType, out operatorText);
		ExpressionRule(out exp);
		if (result != null && result is ElementReferenceExpression)
		{
			((ElementReferenceExpression)result).IsModified = true;
		}
		result = GetAssignmentExpression(result, operatorText, assignmentType, operatorRange, exp);
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void DeclVariableCoreWithoutSpecifier(out BaseVariable result, DeclaratorType declaratorType)
	{
		VBDeclarator decl = null;
		LanguageElementCollection arrayModifiers = null;
		TypeReferenceExpression type = null;
		SourceRange startRange = la.Range;
		Expression initializer = null;
		SourceRange operatorRange = SourceRange.Empty;
		SourceRange nameRange = la.Range;
		NullableTypeModifier nullableModifier = null;
		SourceRange asRange = SourceRange.Empty;
		string name = la.Value.Trim(); 
		VarIdentifier(out decl);
		if (decl != null)
		{
			type = decl.CharacterType;
			arrayModifiers = decl.ArrayModifiers;
			nullableModifier = decl.NullableModifier;
		}
		nameRange = GetRange(nameRange, tToken.Range);
		if (la.Type == Tokens.As )
		{
			Get();
			asRange = tToken.Range; 
			TypeName(out type, out decl);
		}
		if (la.Type == Tokens.EqualsSymbol )
		{
			EqualOperator();
			operatorRange = tToken.Range; 
			ExpressionRule(out initializer);
		}
		string typeName = String.Empty;
		if (decl != null && decl.FullTypeName != String.Empty)
			typeName = decl.FullTypeName;
		if (IsQueryIdent(declaratorType))
		{
			result = CreateQueryIdent(name, typeName, type, initializer, declaratorType);
		}
		else
		{			
			result = CreateSingleVariable(name, typeName, arrayModifiers, nullableModifier, type, initializer, declaratorType, true);
			if (result != null)
			{
				LocalVarArrayCollection.AddVarArrayName(result);
			}
		}
		if (result != null)
		{
			result.AsRange = asRange;
			if (operatorRange != SourceRange.Empty)
			{
				result.OperatorRange = operatorRange;
			}
			result.NameRange = nameRange;
			result.SetRange(GetRange(startRange, tToken.Range));
		}		
	}
	void ElementReferenceQualifiedIdentifier(out Expression result)
	{
		result = null;
		QualifiedIdentifier identifier = null;
		QualifiedIdentifier(out identifier, CreateElementType.ElementReferenceExpression);
		result = identifier.Expression as ElementReferenceExpression;
	}
	void ExpressionList(out LanguageElementList list)
	{
		list = new LanguageElementList();
		Expression exp = null;
		ExpressionRule(out exp);
		list.Add(exp);
		while (la.Type == Tokens.Comma )
		{
			Get();
			ExpressionRule(out exp);
			list.Add(exp);
		}
	}
	void ForStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		LanguageElement init = null;
		Token startBlockToken = la;
		DeclaratorType dummyDeclType;
		_DeclarationsCache.OpenScope();
		Expect(Tokens.For );
		if (la.Type == Tokens.Each )
		{
			Get();
			VBForEach forEach = new VBForEach();
			if (StartOf(20))
			{
				DeclaratorQualifier(out dummyDeclType);
			}
			DeclareVariableAssignDim(out init);
			forEach.Initializer = init;
			forEach.AddDetailNode(init);
			Expect(Tokens.In );
			Expression exp = null;
			ExpressionRule(out exp);
			forEach.Expression = exp;
			OpenContext(forEach);
			StatementTerminator();
			Block(startBlockToken);
			CloseContext();
			Expect(Tokens.Next );
			if (StartOf(26))
			{
				Expression nextExpression = null;
				ExpressionRule(out nextExpression);
				forEach.NextExpression = nextExpression;
				forEach.AddDetailNode(nextExpression);
			}
			result = forEach;
		}
		else if (StartOf(4))
		{
			if (StartOf(20))
			{
				DeclaratorQualifier(out dummyDeclType);
			}
			LoopControlVariable(out init);
			For @for = new VBFor();
			@for.AddInitializer(init);
			Expect(Tokens.ToToken );
			Expression toExpression = null;
			ExpressionRule(out toExpression);
			@for.ToExpression = toExpression;
			@for.AddDetailNode(toExpression);
			if (la.Type == Tokens.Step )
			{
				Get();
				Expression stepExpression = null;
				ExpressionRule(out stepExpression);
				if (stepExpression != null)
				{
					@for.StepExpression = stepExpression;
					@for.AddDetailNode(stepExpression);
				}
			}
			OpenContext(@for);
			StatementTerminator();
			Block(startBlockToken);
			CloseContext();
			Expect(Tokens.Next );
			if (StartOf(26))
			{
				LanguageElementList nextList = null;
				ExpressionList(out nextList);
				@for.NextExpressionList = nextList;
				foreach(LanguageElement element in nextList)
				{
					@for.AddIncrementor(element);
				}
			}
			result = @for;
		}
		else
			SynErr(283);
		_DeclarationsCache.CloseScope();
		if (result != null)
			result.SetRange(GetRange(startRange, tToken.Range));
	}
	void IfStatement(out Statement result)
	{
		result = null;
		BlockType blockType = BlockType.IfSingleLineStatement;
		Token startBlockToken = la;
		Expression exp = null;
		SourceRange endStatementRange = la.Range;
		Expect(Tokens.IfToken );
		SourceRange ifStartRange = tToken.Range;
		ExpressionRule(out exp);
		If ifNode = new If();
		if (exp != null)
			ifNode.SetExpression(exp);
		WaitIfConditionEnd();
		OpenContext(ifNode);
		if (la.Type == Tokens.Then )
		{
			Get();
		}
		if (StartOf(27))
		{
			StatementTerminatorCall();
			if (tToken.Type == Tokens.LineTerminator || tToken.Type == Tokens.Colon)
			blockType = BlockType.None;
		}
		BlockCore(startBlockToken, blockType, out endStatementRange);
		CloseContext();
		if (blockType == BlockType.IfSingleLineStatement)
		{
			ifNode.IsOneLine = true;
		}
		result = ifNode;
		while (la.Type == Tokens.Else  || la.Type == Tokens.ElseIf )
		{
			IfElse ifElseNode = null; 
			bool isElseIf = false;
			SourceRange endRange = la.Range;
			if (la.Type == Tokens.Else )
			{
				startBlockToken = la;
				Get();
				if (la.Type == Tokens.IfToken )
				{
					Get();
					isElseIf = true; 
				}
			}
			else
			{
				Get();
				isElseIf = true; 
			}
			if (!isElseIf)
			{
				ifElseNode = new Else();
			}
			result.SetRange(new SourceRange(ifStartRange.Start, endRange.Start));
			ifStartRange = endRange;
				if(isElseIf)
				{
			ExpressionRule(out exp);
			WaitIfConditionEnd(); 
			if (la.Type == Tokens.Then )
			{
				Get();
			}
			ifElseNode = new ElseIf();
			if (exp != null)
				((ElseIf)ifElseNode).SetExpression(exp);
			}	
			OpenContext(ifElseNode);
			StatementTerminatorCall();
			BlockCore(startBlockToken, blockType, out endStatementRange);
			CloseContext();
			if (blockType == BlockType.IfSingleLineStatement && ifElseNode != null)
			{
				ifElseNode.IsOneLine = true;
			}
			result = ifElseNode;
		}
		if (blockType != BlockType.IfSingleLineStatement)
		{
		if (la.Type == Tokens.EndToken )
		{
			Get();
			Expect(Tokens.IfToken );
		}
		endStatementRange = tToken.Range;
		}
		result.SetRange(GetRange(ifStartRange, endStatementRange));
	}
	void ThrowStatement(out Statement result)
	{
		result = null;
		Throw @throw = new Throw();
		SourceRange startRange = la.Range;
		Expect(Tokens.Throw );
		if (StartOf(26))
		{
			Expression exp = null;
			ExpressionRule(out exp);
			if (exp != null)
			{
				@throw.Expression = exp;
				@throw.AddDetailNode(exp);
			}
		}
		@throw.SetRange(GetRange(startRange, tToken.Range));
		result = @throw;
		AddNode(result);
	}
	void ReturnStatement(out Statement result)
	{
		result = null;
		Return  @return = new Return();
		SourceRange startRange = la.Range;
		Expect(Tokens.Return );
		if (StartOf(26))
		{
			Expression exp = null;
			ExpressionRule(out exp);
			@return.Expression = exp;
			@return.AddDetailNode(exp);
		}
		@return.SetRange(GetRange(startRange, tToken.Range));
		result = @return;
		AddNode(result);
	}
	void AssignmentStatement(out Statement result)
	{
		result = null;
		Expression leftPart = null;
		string operatorText = String.Empty;
		AssignmentOperatorType assignmentType = AssignmentOperatorType.None;
		SourceRange startRange = la.Range;
		 bool isCall = tToken.Type == Tokens.Call;
		PrimaryExpressionCore(out leftPart, false);
		if (StartOf(28))
		{
			AssignmentOperator(out assignmentType, out operatorText);
			Expression rightPart = null;
			SourceTextRange textRange = new SourceTextRange(operatorText, tToken.Range);
			ExpressionRule(out rightPart);
			if (leftPart != null && rightPart != null)
			{
				if (leftPart is ElementReferenceExpression)
				{
					((ElementReferenceExpression)leftPart).IsModified = true;
				}
				result = GetAssignment(leftPart, textRange, assignmentType, rightPart);			
			}
			return;
		}
		if(leftPart != null && leftPart is MethodCallExpression)
		{
		   if (isCall)
		   {
			 CallStatement callStatement = new CallStatement();
			 callStatement.CalledExpression = leftPart as MethodCallExpression;
			 callStatement.SetRange(GetRange(startRange, leftPart));
			 result = callStatement;
		   }
		   else
		   {
			result = MethodCall.FromMethodCallExpression(leftPart as MethodCallExpression);
			if (result != null)
				((MethodCall)result).IsBaseConstructorCall = ((MethodCallExpression)leftPart).IsBaseConstructorCall;
		}
		}
		else if (leftPart != null)
		{
			result = GetSimpleStatement(leftPart, startRange);
		}
	}
	void UsingStatementRule(out Statement result)
	{
		UsingStatement @using = new UsingStatement();
		LanguageElementCollection coll = new LanguageElementCollection();
		SourceRange startRange = la.Range;
		Token startBlockToken = la;
		result = null;
		Token startToken = la;
		Expect(Tokens.Using );
		if (!IsVarInUsing())
		{
			Expression exp = null;
			ExpressionRule(out exp);
			if (exp != null)
			{
				@using.Initializers.Add(exp);
				@using.AddDetailNode(exp);
			}
		}
		if (StartOf(5))
		{
			DeclareVariableList(out coll, false, false);
			StatementTerminatorCall();
			foreach(LanguageElement element in coll)
			{
				if (element == null)
					break;
				@using.Initializers.Add(element);
				@using.AddDetailNode(element);
			}
		}
		StatementTerminatorCall();
		OpenContext(@using);
		Block(startBlockToken);
		CloseContext();
		Expect(Tokens.EndToken );
		Expect(Tokens.Using );
		@using.SetRange(GetRange(startToken, tToken.Range));
		result = @using;
	}
	void YieldStatement(out Statement result)
	{
		Expression exp;
		SourceRange startRange = la.Range;
		Expect(Tokens.Identifier );
		ExpressionRule(out exp);
		YieldReturn yieldReturn = new YieldReturn();
		yieldReturn.Expression = exp;
		yieldReturn.SetRange(GetRange(startRange, tToken.Range));
		result = yieldReturn;
		AddNode(result);
	}
	void StopStatement(out Statement result)
	{
		result = new Stop();
		result.SetRange(la.Range);
		AddNode(result);
		Expect(Tokens.Stop );
	}
	void SelectStatement(out Statement result)
	{
		LanguageElementCollection coll = null;
		Switch select = new Switch();
		SourceRange startRange = la.Range;
		result = null;
		Expect(Tokens.Select );
		select.SetBlockStart(tToken.Range);		
		select.SetBlockType(DelimiterBlockType.Token);
		if (la.Type == Tokens.Case )
		{
			Get();
		}
		Expression exp = null; 
		ExpressionRule(out exp);
		select.Expression = exp;
		Case @case = null;	
		StatementTerminator();
		while (la.Type == Tokens.Case )
		{
			if (@case != null)
			{
				@case.SetEnd(la.Range.Start);
			}
			Get();
			select.HasBlock = true;		
			Token startBlockToken = tToken;
			@case = new Case();
			StatementTerminatorCall();
			if (la.Type == Tokens.Else )
			{
				Get();
				@case.IsDefault = true;
			}
			else if (StartOf(29))
			{
				CaseClauses(out coll);
				foreach(CaseClause caseClause in coll)
				{				
					@case.AddCaseClause(caseClause);
				}
			}
			else
				SynErr(284);
			OpenContextWithoutAddNode(@case);
			StatementTerminatorCall();
			Block(startBlockToken);
			StatementTerminatorCall();
			CloseContext();
			@case.SetRange(GetRange(startBlockToken, tToken.Range));
			select.AddCaseStatement(@case);			
		}
		Expect(Tokens.EndToken );
		select.SetBlockEnd(tToken.Range);
		Expect(Tokens.Select );
		result = select;
		if (result != null)
		{
			AddNode(result);
			result.SetRange(GetRange(startRange, tToken.Range));
		}
	}
	void GoToStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.GoTo );
		SourceRange startLableRange = la.Range;
		LabelName();
		result = new Goto();
		((Goto)result).Label = tToken.Value;
		((Goto)result).LabelRange = GetRange(startLableRange, tToken.Range);
		result.SetRange(GetRange(startRange, tToken.Range));
		AddNode(result);
	}
	void UnstructuredErrorStatement(out Statement result)
	{
		result = null;
		if (la.Type == Tokens.Error )
		{
			ErrorStatement(out result);
		}
		else if (la.Type == Tokens.On )
		{
			OnErrorStatement(out result);
		}
		else if (la.Type == Tokens.Resume )
		{
			ResumeStatement(out result);
		}
		else
			SynErr(285);
		if (result != null)
		AddNode(result);
	}
	void ArrayHandlingStatement(out Statement result)
	{
		result = null;
		if (la.Type == Tokens.ReDim )
		{
			RedimStatement(out result);
		}
		else if (la.Type == Tokens.Erase )
		{
			EraseStatement(out result);
		}
		else
			SynErr(286);
		if (result != null)
		AddNode(result);
	}
	void RaiseEventStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		Expression exp = null;
		Expect(Tokens.RaiseEvent );
		ExpressionRule(out exp);
		result = new RaiseEvent(exp);
		result.SetRange(GetRange(startRange, tToken.Range));
		AddNode(result);
	}
	void AddHandlerStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		Expression eventExp = null;
		Expression addressExp = null;
		bool isAddHandler = false;
		if (la.Type == Tokens.AddHandler )
		{
			Get();
			isAddHandler = true;
		}
		else if (la.Type == Tokens.RemoveHandler )
		{
			Get();
		}
		else
			SynErr(287);
		ExpressionRule(out eventExp);
		Expect(Tokens.Comma );
		ExpressionRule(out addressExp);
		if (isAddHandler)
		{
			result = new AddHandler(eventExp, addressExp);
		}
		else
		{
			result = new RemoveHandler(eventExp, addressExp);
		}
		result.SetRange(GetRange(startRange, tToken.Range));
		AddNode(result);
	}
	void RedimStatement(out Statement result)
	{
		result = null;
		ExpressionCollection coll = null;
		bool hasPreserv = false;
		SourceRange startRange = la.Range;
		Expect(Tokens.ReDim );
		if (la.Type == Tokens.Preserve )
		{
			Get();
			hasPreserv = true;
		}
		ReDimExpressionListRule(out coll);
		result = new ReDim(hasPreserv, coll);
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void EraseStatement(out Statement result)
	{
		result = null;
		Expression exp = null;
		Erase erase = new Erase();
		SourceRange startRange = la.Range;
		Expect(Tokens.Erase );
		ExpressionRule(out exp);
		if (exp != null)
		{
			erase.Expressions.Add(exp);
			erase.AddDetailNode(exp);
		}
		while (la.Type == Tokens.Comma )
		{
			Get();
			ExpressionRule(out exp);
			if (exp != null)
			{
				erase.Expressions.Add(exp);
				erase.AddDetailNode(exp);
			}
		}
		erase.SetRange(GetRange(startRange, tToken.Range));
		result = erase;
	}
	void ReDimExpressionListRule(out ExpressionCollection result)
	{
		result = new ExpressionCollection();
		Expression exp = null;
		ReDimExpressionRule(out exp);
		result.Add(exp);
		while (la.Type == Tokens.Comma )
		{
			Get();
			ReDimExpressionRule(out exp);
			result.Add(exp);
		}
	}
	void ReDimExpressionRule(out Expression reDimExp)
	{
		reDimExp = null;
		ReDimExpression result = new ReDimExpression();
		ArrayNameModifier arrayNameModifier = null;
		SourceRange startRange = la.Range;
		Expression expression = null;
		ExpressionRule(out expression);
		MethodCallExpression methodCall = expression as MethodCallExpression;
		if (methodCall != null)
		{
			MethodReferenceExpression methodReference = methodCall.Qualifier as MethodReferenceExpression;
			if (methodReference == null)
				expression = methodCall.Qualifier;
			else
			{
				if (methodReference.Qualifier == null)
					expression = new ElementReferenceExpression(methodReference.Name, methodReference.Range);
				else
				{
					expression = new QualifiedElementReference(methodReference.Qualifier, methodReference.Name, methodReference.NameRange);
					expression.SetRange(methodReference.Range);
				}
			}
			arrayNameModifier = new ArrayNameModifier(methodCall.ArgumentsCount, methodCall.Arguments);
			arrayNameModifier.SetRange(new SourceRange(expression.Range.End, methodCall.Range.End));
		}
		if(expression != null)
		{
			result.Name = expression.Name;
			result.NameRange = expression.NameRange;
			ElementReferenceExpression elementRefExpression = expression as ElementReferenceExpression;
			if (elementRefExpression != null)
				elementRefExpression.IsModified = true;
			result.Expression = expression;
			result.AddDetailNode(expression);
		}
				if(arrayNameModifier != null)
				{
					result.AddDetailNode(arrayNameModifier);
					result.AddModifier(arrayNameModifier);
				}
		reDimExp = result;
		reDimExp.SetRange(GetRange(startRange, tToken.Range));
	}
	void ErrorStatement(out Statement result)
	{
		result = null;
		Expression exp = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.Error );
		ExpressionRule(out exp);
		if (exp != null)
		{
			result = new VB.Error(exp);
			result.AddDetailNode(exp);
		}
		else
		{
			result = new VB.Error();
		}
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void OnErrorStatement(out Statement result)
	{
		result = null;
		SourceRange startRange = la.Range;
		string onErrorString = String.Empty;	
		Expect(Tokens.On );
		Expect(Tokens.Error );
		while (la.Type == Tokens.GoTo  || la.Type == Tokens.Resume )
		{
			if (la.Type == Tokens.GoTo )
			{
				Get();
				if (onErrorString != String.Empty)
				{
					onErrorString += " ";
				}
				onErrorString += tToken.Value;
				if (la.Type == Tokens.Minus )
				{
					Get();
					onErrorString += " " + tToken.Value;
				}
				if (StartOf(12))
				{
					VarKeyword();
				}
				else if (la.Type == Tokens.IntegerLiteral )
				{
					Get();
				}
				else
					SynErr(288);
				onErrorString += " " + tToken.Value;
			}
			else
			{
				Get();
				onErrorString += " " + tToken.Value; 
				Expect(Tokens.Next );
				onErrorString += " " + tToken.Value; 
			}
		}
		SourceRange endRange = GetRange(startRange, tToken.Range);
		result = new OnError(onErrorString, endRange);
	}
	void ResumeStatement(out Statement result)
	{
		result = null;
		Resume resume = new Resume();
		SourceRange startRange = la.Range;	
		Expect(Tokens.Resume );
		if (StartOf(30))
		{
			if (StartOf(31))
			{
				LabelName();
				{
				resume.Label = tToken.Value;
				}
			}
			else
			{
				Get();
				resume.HasNextClause = true;
			}
		}
		resume.SetRange(GetRange(startRange, tToken.Range));
		result = resume;	
	}
	void CaseClauses(out LanguageElementCollection coll)
	{
		coll = new LanguageElementCollection();
		CaseClause caseClause = null;
		CaseClause(out caseClause);
		coll.Add(caseClause); 
		while (la.Type == Tokens.Comma )
		{
			Get();
			CaseClause(out caseClause);
			coll.Add(caseClause); 
		}
		if (la.Type == Tokens.Colon )
		{
			Get();
		}
	}
	void CaseClause(out CaseClause caseClause)
	{
		Expression exp = null;
		caseClause = new CaseClause();
		SourceRange startRange = la.Range;
		if (la.Type == Tokens.Is )
		{
			Get();
			caseClause.IsComparisonClause = true; 
		}
		if (la.Type == Tokens.LessThan)
		{
			_NotXmlNode = true;
		}
		ExpressionRule(out exp);
		_NotXmlNode = false;
		caseClause.StartExpression = exp;
		caseClause.AddDetailNode(exp);
		if (la.Type == Tokens.ToToken )
		{
			Get();
			caseClause.IsRangeCheckClause = true;
			ExpressionRule(out exp);
			caseClause.EndExpression = exp;
			caseClause.AddDetailNode(exp);
		}
		caseClause.SetRange(GetRange(startRange, tToken.Range));
	}
	void PrimaryExpressionCore(out Expression result, bool canBeXmlExpression)
	{
		result = null;
		bool isCall = false;
		bool isBaseConstructorCall = false;
		TypeReferenceExpression type = null;
		SourceRange startRange = la.Range;
		SourceRange	callRange = SourceRange.Empty;
		if (la.Type == Tokens.Call )
		{
			Get();
			isCall = true;
			callRange = tToken.Range;
			startRange = la.Range;
		}
		if (la.Type == Tokens.ShiftLeft  || la.Type == Tokens.LessThan )
		{
			if (canBeXmlExpression)
			{
			XmlExpressionRule(out result);
			}
			else
			{
				Get();
				return;
			}
		}
		else if (IsAsyncLambda())
		{
			Expect(Tokens.Identifier );
			LambdaExpression(out result);
		}
		else if (IsAwaitExpression())
		{
			AwaitExpression(out result);
		}
		else if (StartOf(32))
		{
			PrimitiveExpressionRule(out result);
		}
		else if (la.Type == Tokens.CurlyBraceOpen )
		{
			ArrayInitializerExpressionRule(out result);
		}
		else if (la.Type == Tokens.False  || la.Type == Tokens.True )
		{
			TrueOrFalseExpressionRule(out result);
		}
		else if (la.Type == Tokens.ParenOpen )
		{
			ParenthesizedExpressionRule(out result);
		}
		else if (StartOf(11))
		{
			PrimitiveTypeName(out type);
			result = type;
		}
		else if (la.Type == Tokens.Me  || la.Type == Tokens.MyBase  || la.Type == Tokens.MyClass )
		{
			MeOrMyBaseOrMyClassRule(out result);
		}
		else if (la.Type == Tokens.New )
		{
			NewExpression(out result);
		}
		else if (StartOf(33))
		{
			CastExpression(out result);
		}
		else if (la.Type == Tokens.AddressOf )
		{
			AdressOfExpressionRule(out result);
		}
		else if (la.Type == Tokens.GetTypeToken )
		{
			TypeOfExpressionRule(out result);
		}
		else if (la.Type == Tokens.Dot )
		{
			MemberAccessExpressionRule(out result);
		}
		else if (la.Type == Tokens.Not )
		{
			LogicalNotOpInPrimaryExpression(out result);
		}
		else if (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			UnaryExpressionInPrimaryExpression(out result);
		}
		else if (la.Type == Tokens.Function  || la.Type == Tokens.Sub )
		{
			LambdaExpression(out result);
		}
		else if (la.Type == Tokens.IfToken )
		{
			ConditionalExpression(out result);
		}
		else if (StartOf(34))
		{
			if (!IsTryCast() && !IsSqlExpression())
			{
			HeadIdentifierInPrimaryExpression(out result);
			}
			else if(IsTryCast())
			{
			TryCast(out result);
			}
			else if (IsSqlExpression())
			{
			SqlExpression(out result);
			return;
			}
			else
			{
				HeadIdentifierInPrimaryExpression(out result);
			}
		}
		else
			SynErr(289);
		if (result == null || (result is ArrayInitializerExpression && _IsNewContext))
			return;
		while (StartOf(35))
		{
			if (la.Type == Tokens.Dot  || la.Type == Tokens.TripleDot )
			{
				bool isTripleDot = false; 
				if (la.Type == Tokens.Dot )
				{
					Get();
				}
				else if (la.Type == Tokens.TripleDot )
				{
					Get();
					isTripleDot = true;
				}
				else
					SynErr(290);
				if (la.Type == Tokens.LessThan )
				{
					XmlElementReferenceExpressionRule(ref result);
					if (result != null && result is XmlElementReferenceExpression && isTripleDot)
					{
						(result as XmlElementReferenceExpression).XmlReferenceType = XmlReferenceType.TripleDot;
					}
				}
				else if (la.Type == Tokens.CommAtSymbol )
				{
					XmlAttributeReferenceExpressionRule(ref result);
				}
				else if (StartOf(4))
				{
					ReferenceExpressionRule(ref result, startRange, isCall, callRange, ref isBaseConstructorCall);
				}
				else
					SynErr(291);
			}
			else if (la.Type == Tokens.ParenOpen )
			{
				MethodCallExpression(ref result);
				if (result is MethodCallExpression)
					((MethodCallExpression)result).IsBaseConstructorCall = isBaseConstructorCall;
			}
			else
			{
				IndexerInPrimaryExpressionRule(ref result);
			}
		}
	}
	void AssignmentOperator(out AssignmentOperatorType type, out string operatorText)
	{
		type = AssignmentOperatorType.None;
		switch (la.Type)
		{
		case Tokens.EqualsSymbol : 
		{
			EqualOperator();
			type = AssignmentOperatorType.Assignment;
			break;
		}
		case Tokens.PlusEqual : 
		{
			Get();
			type = AssignmentOperatorType.PlusEquals;
			break;
		}
		case Tokens.MinusEqual : 
		{
			Get();
			type = AssignmentOperatorType.MinusEquals;
			break;
		}
		case Tokens.MulEqual : 
		{
			Get();
			type = AssignmentOperatorType.StarEquals;
			break;
		}
		case Tokens.DivEqual : 
		{
			Get();
			type = AssignmentOperatorType.SlashEquals;
			break;
		}
		case Tokens.BackSlashEquals : 
		{
			Get();
			type = AssignmentOperatorType.BackSlashEquals;
			break;
		}
		case Tokens.XorEqual : 
		{
			Get();
			type = AssignmentOperatorType.XorEquals;
			break;
		}
		case Tokens.ShiftLeftEqual : 
		{
			Get();
			type = AssignmentOperatorType.ShiftLeftEquals;
			break;
		}
		case Tokens.ShiftRightEqual : 
		{
			Get();
			type = AssignmentOperatorType.ShiftRightEquals;
			break;
		}
		case Tokens.AndEqual : 
		{
			Get();
			type = AssignmentOperatorType.BitAndEquals;
			break;
		}
		default: SynErr(292); break;
		}
		if (type == AssignmentOperatorType.None)
		operatorText = String.Empty;
		else
			operatorText = tToken.Value;
	}
	void OperatorExpression(out Expression result)
	{
		SourcePoint startPoint = la.Range.Start;
		LogicalXorOp(out result);
		if (result != null)
		{
			SourceRange operatorRange = result.Range;
			SourcePoint operatorPoint = operatorRange.Start;
			int line = operatorPoint.Line;
			int offset = operatorPoint.Offset;
			if (line == 0 && offset == 0)
				result.SetRange(new SourceRange(startPoint, result.Range.End));
		}
	}
	void CastExpression(out Expression result)
	{
		result = null;
		Token token = null;
		VBDeclarator decl = null;
		TypeReferenceExpression type = null;
		string castTypeName = null;
		string castKeyword = null;
		if (la.Type == Tokens.CType  || la.Type == Tokens.DirectCast )
		{
			bool isDirectCast = false;
			if (la.Type == Tokens.DirectCast )
			{
				Get();
				isDirectCast = true;
			}
			else
			{
				Get();
			}
			token = tToken; 
			Expect(Tokens.ParenOpen );
			ExpressionRule(out result);
			Expect(Tokens.Comma );
			TypeName(out type, out decl);
			Expect(Tokens.ParenClose );
			if (isDirectCast)
			result = new DirectCastExpression(type, result);		
			else
				result = new CTypeExpression(type, result);
			result.SetRange(GetRange(token.Range, tToken.Range));
		}
		else if (StartOf(36))
		{
			CastTarget(out castTypeName, out castKeyword);
			SourceRange castRange = tToken.Range;
			Expect(Tokens.ParenOpen );
			ExpressionRule(out result);
			Expect(Tokens.ParenClose );
			result = new CastTargetExpression(castTypeName, castKeyword, castRange, result);
			result.SetRange(GetRange(castRange, tToken.Range));
		}
		else
			SynErr(293);
	}
	void CastTarget(out string castTypeName, out string castKeyword)
	{
		castTypeName = String.Empty;
		castKeyword = String.Empty;
		switch (la.Type)
		{
		case Tokens.CBool : 
		{
			Get();
			castTypeName = "Boolean";
			castKeyword = "CBool";
			break;
		}
		case Tokens.CByte : 
		{
			Get();
			castTypeName = "Byte";
			castKeyword = "CByte";
			break;
		}
		case Tokens.CChar : 
		{
			Get();
			castTypeName = "Char";
			castKeyword = "CChar";
			break;
		}
		case Tokens.CDate : 
		{
			Get();
			castTypeName = "Date";
			castKeyword = "CDate";
			break;
		}
		case Tokens.CDec : 
		{
			Get();
			castTypeName = "Decimal";
			castKeyword = "CDec";
			break;
		}
		case Tokens.CDbl : 
		{
			Get();
			castTypeName = "Double";
			castKeyword = "CDbl";
			break;
		}
		case Tokens.CInt : 
		{
			Get();
			castTypeName = "Integer";
			castKeyword = "CInt";
			break;
		}
		case Tokens.CLng : 
		{
			Get();
			castTypeName = "Long";
			castKeyword = "CLng";
			break;
		}
		case Tokens.CObj : 
		{
			Get();
			castTypeName = "Object";
			castKeyword = "CObj";
			break;
		}
		case Tokens.CShort : 
		{
			Get();
			castTypeName = "Short";
			castKeyword = "CShort";
			break;
		}
		case Tokens.CSng : 
		{
			Get();
			castTypeName = "Single";
			castKeyword = "CSng";
			break;
		}
		case Tokens.CStr : 
		{
			Get();
			castTypeName = "String";
			castKeyword = "CStr";
			break;
		}
		case Tokens.CUInt : 
		{
			Get();
			castTypeName = "UInteger";
			castKeyword = "CUInt";
			break;
		}
		case Tokens.CULng : 
		{
			Get();
			castTypeName = "ULong";
			castKeyword = "CULng";
			break;
		}
		case Tokens.CUShort : 
		{
			Get();
			castTypeName = "UShort";
			castKeyword = "CUShort";
			break;
		}
		case Tokens.CSByte : 
		{
			Get();
			castTypeName = "SByte";
			castKeyword = "CSByte";
			break;
		}
		default: SynErr(294); break;
		}
	}
	void PrimitiveExpressionRule(out Expression result)
	{
		result = null;
		PrimitiveType intType = PrimitiveType.Undefined;
		 string value = la.Value;
		 SourceRange range = la.Range;
		switch (la.Type)
		{
		case Tokens.StringLiteral : 
		{
			Get();
			intType = PrimitiveType.String;
			AddTextString(tToken);
			break;
		}
		case Tokens.CharacterLiteral : 
		{
			Get();
			intType = VBPrimitiveTypeUtils.NarrowCharAndStrignLiteralType(tToken.Value);
			break;
		}
		case Tokens.FloatingPointLiteral : 
		{
			Get();
			intType = VBPrimitiveTypeUtils.NarrowFloatingPointLiteralType(tToken.Value);
			break;
		}
		case Tokens.IntegerLiteral : 
		{
			Get();
			intType = VBPrimitiveTypeUtils.NarrowIntegerLiteralType(tToken.Value);
			break;
		}
		case Tokens.Sharp : 
		{
			DateRule(out value);
			intType = PrimitiveType.DateTime;
			  range = GetRange(range, tToken.Range);
			break;
		}
		case Tokens.Nothing : 
		{
			Get();
			intType = PrimitiveType.Void;
			break;
		}
		default: SynErr(295); break;
		}
		if (intType != PrimitiveType.Undefined)
		{
			result = GetPrimitiveExpression(value, range, intType);
		}
	}
	void DateRule(out string name)
	{
		string month = null;
		string day = null;
		string year = null;
		string ampm = null;
		string hour = null;
		string minute = null;
		string second = null;
		string dateDelemiter = "-";
		Expect(Tokens.Sharp );
		while (la.Type == Tokens.IntegerLiteral )
		{
			Get();
			if (la.Type == Tokens.Minus  || la.Type == Tokens.Slash )
			{
				month = tToken.Value;
				dateDelemiter = la.Value;
				if (la.Type == Tokens.Slash )
				{
					Get();
				}
				else
				{
					Get();
				}
				Expect(Tokens.IntegerLiteral );
				day = tToken.Value; 
				if (la.Type == Tokens.Slash )
				{
					Get();
				}
				else if (la.Type == Tokens.Minus )
				{
					Get();
				}
				else
					SynErr(296);
				if (la.Type == Tokens.IntegerLiteral )
				{
					Get();
					year = tToken.Value; 
				}
				else if (la.Type == Tokens.FloatingPointLiteral )
				{
					Get();
					year = tToken.Value;
					if (!string.IsNullOrEmpty(year))
					  year = year.Remove(year.Length - 1);
				}
				else
					SynErr(297);
			}
			else if (la.Type == Tokens.Colon )
			{
				hour = tToken.Value; 
				Get();
				if (la.Type == Tokens.IntegerLiteral )
				{
					Get();
					minute = tToken.Value; 
				}
				else if (la.Type == Tokens.FloatingPointLiteral )
				{
					Get();
					minute = tToken.Value;
					if (!string.IsNullOrEmpty(minute))
					  minute = minute.Remove(minute.Length - 1);
				}
				else
					SynErr(298);
				if (la.Type == Tokens.Colon )
				{
					Get();
					if (la.Type == Tokens.IntegerLiteral )
					{
						Get();
						second = tToken.Value; 
					}
					else if (la.Type == Tokens.FloatingPointLiteral )
					{
						Get();
						second = tToken.Value;
						if (!string.IsNullOrEmpty(second))
						  second = second.Remove(minute.Length - 1);
					}
					else
						SynErr(299);
				}
				if (la.Type == Tokens.Identifier )
				{
					Get();
					ampm = tToken.Value; 
				}
			}
			else if (la.Type == Tokens.Identifier )
			{
				hour = tToken.Value; 
				Get();
				ampm = tToken.Value; 
			}
			else
				SynErr(300);
		}
		if (la.Type == Tokens.Sharp )
		{
			Get();
		}
		name = GetDateLiteral(month, day, year, hour, minute, second, ampm, dateDelemiter);
	}
	void TrueOrFalseExpressionRule(out Expression result)
	{
		result = null;
		if (la.Type == Tokens.True )
		{
			Get();
			result = new PrimitiveExpression(tToken.Value, tToken.Range);
			(result as PrimitiveExpression).PrimitiveValue = true;
			(result as PrimitiveExpression).PrimitiveType = PrimitiveType.Boolean;
		}
		else if (la.Type == Tokens.False )
		{
			Get();
			result = new PrimitiveExpression(tToken.Value, tToken.Range);
			(result as PrimitiveExpression).PrimitiveValue = false;
			(result as PrimitiveExpression).PrimitiveType = PrimitiveType.Boolean;
		}
		else
			SynErr(301);
	}
	void ParenthesizedExpressionRule(out Expression result)
	{
		result = null;
		Expect(Tokens.ParenOpen );
		Token parenOpenToken = tToken; 
		ExpressionRule(out result);
		Expect(Tokens.ParenClose );
		result = new ParenthesizedExpression(result);
		result.SetRange(GetRange(parenOpenToken, tToken));
	}
	void MeOrMyBaseOrMyClassRule(out Expression result)
	{
		result = null;
		if (la.Type == Tokens.Me )
		{
			Get();
			result = new ThisReferenceExpression();
		}
		else if (la.Type == Tokens.MyBase )
		{
			Get();
			result = new BaseReferenceExpression();
		}
		else if (la.Type == Tokens.MyClass )
		{
			Get();
			result = new MyClassExpression();
		}
		else
			SynErr(302);
		result.Name = tToken.Value;
		result.SetRange(tToken.Range);
	}
	void AdressOfExpressionRule(out Expression result)
	{
		result = null;
		Token addressofToken = la;
		Expect(Tokens.AddressOf );
		ExpressionRule(out result);
		result = new AddressOfExpression(result);
		result.SetRange(GetRange(addressofToken.Range, result.Range));
	}
	void TypeOfExpressionRule(out Expression result)
	{
		result = null;
		TypeReferenceExpression type = null;
		Token getTypeToken = la;
		VBDeclarator decl = null;
		Expect(Tokens.GetTypeToken );
		Expect(Tokens.ParenOpen );
		TypeName(out type, out decl);
		Expect(Tokens.ParenClose );
		result = new TypeOfExpression(type);
		if (type != null)
			result.AddNode(type);
		result.SetRange(GetRange(getTypeToken, tToken));
	}
	void IndexerInPrimaryExpressionRule(ref Expression result)
	{
		Expect(Tokens.ExclamationSymbol );
		Expression argument = null;
		ElementReferenceQualifiedIdentifier(out argument);
		IndexerExpression indexerExpression = CreateIndexerExpression(result, argument);
		indexerExpression.SetRange(GetRange(result.Range, tToken.Range));
		result = indexerExpression;
	}
	void MethodCallExpression(ref Expression result)
	{
		ResetPeek();
		Token peekToken = Peek();
		ExpressionCollection arguments = null;
		ReferenceExpressionBase reb = result as ReferenceExpressionBase;
		int typeArity = 0;
		SourceRange parensOpenRange = la.Range;
		SourceRange parensCloseRange = la.Range;
		if (peekToken != null && peekToken.Type == Tokens.Of)
		{
			ConstructedTypeNameTail(out arguments, out typeArity);
			SetGenericTypeArguments(arguments, tToken, reb);
		}
		if (la.Type != Tokens.ParenOpen)
		return;
		Expect(Tokens.ParenOpen );
		ExpressionCollection parameters = null;
		int methodCallRank = 1;
		parensOpenRange = tToken.Range;
		if (StartOf(4))
		{
			ArgumentList(out parameters, out methodCallRank);
		}
		Expect(Tokens.ParenClose );
		if (result == null)
		return;
		parensCloseRange = tToken.Range;
		SourceRange startRange = result.Range;
		if (result is ElementReferenceExpression)
		{
			result = CreateIndexerExpression(result, parameters);
		}
		else
		{
			result = MethodReferenceToMethodCall(result, parameters);
			if (result != null)
			{
				(result as MethodCallExpression).SetParensRange(GetRange(parensOpenRange, parensCloseRange));
			}
		}
		if (result != null)
		{
			result.SetRange(GetRange(startRange, tToken.Range));
		}
	}
	void ConstructedTypeNameTail(out ExpressionCollection result, out int typeArity)
	{
		result = null;
		Expect(Tokens.ParenOpen );
		Expect(Tokens.Of );
		TypeArgumentList(out result, out typeArity);
		Expect(Tokens.ParenClose );
	}
	void ArgumentList(out ExpressionCollection arguments, out int rank)
	{
		arguments = new ExpressionCollection();
		Expression element = null;
		rank = 1;
		while (StartOf(4))
		{
			if (la.Type == Tokens.Comma )
			{
				Get();
				rank++; 
				element = new EmptyArgumentExpression();
				element.SetRange(tToken.Range.Start, tToken.Range.Start);
				element.NameRange = element.Range;
			}
			else
			{
				Argument(out element);
				if (la.Type != Tokens.Comma)
				{
				  if (element != null)
					arguments.Add(element);
				  break;
				}
				if (la.Type == Tokens.Comma )
				{
					Get();
					rank++; 
				}
			}
			if (element != null)
			{
			  arguments.Add(element);
			  element = null;
			}
			else
			  break;
		}
		if (tToken.Type == Tokens.Comma)
		{
		  element = new EmptyArgumentExpression();
		  element.SetRange(la.Range.Start, la.Range.Start);
		  element.NameRange = element.Range;
		  arguments.Add(element);
		}
	}
	void MemberAccessExpressionRule(out Expression result)
	{
		result = null;
		Token dummyToken = null;
		SourceRange startRange = la.Range;	
		bool isCall = (tToken.Type == Tokens.Call);
		SourceRange	callRange = SourceRange.Empty;
		if (isCall)
			callRange = tToken.Range;
		Expect(Tokens.Dot );
		result = new MemberAccessExpression();
		result.Name = tToken.Value;
		result.SetRange(GetRange(tToken.Range));
		if (la.Type == Tokens.CommAtSymbol)
		{
		XmlAttributeReferenceExpressionRule(ref result);
		return;
		}
		if (StartOf(4))
		{
			IdentifierOrKeyword(out dummyToken);
			if (la.Type == Tokens.ParenOpen || (isCall && IsStatementTerminator()))
			{
				MethodReferenceExpression mre = new MethodReferenceExpression(result, tToken.Value, tToken.Range);
				mre.SetRange(GetRange(startRange, tToken.Range));
				if (result != null)
				{
					mre.AddNode(result);
				}				
				if (isCall && IsStatementTerminator())
				{
					result = MethodReferenceToMethodCall(mre, null);
					result.SetRange(GetRange(callRange, tToken.Range));
				}
				else
				{
					result = mre;			
				}
			}				
			else
			{
				result = GetElementReference(result, tToken.Value, tToken.Range);
			}			
		}
	}
	void XmlAttributeReferenceExpressionRule(ref Expression result)
	{
		SourceRange startRange = la.Range;
		Token token = null;
		 string value = string.Empty;
		Expect(Tokens.CommAtSymbol );
		IdentifierOrKeyword(out token);
		value = tToken.Value; 
		if (la.Type == Tokens.Colon )
		{
			Get();
			value += tToken.Value + la.Value; 
			IdentifierOrKeyword(out token);
		}
		result = GetXmlAttrElementReference(result, value, tToken.Range);
		if (result != null)
			result.SetRange(GetRange(result.Range, tToken.Range));
	}
	void HeadIdentifierInPrimaryExpression(out Expression result)
	{
		result = null;
		SourceRange startRange = la.Range;
		SourceRange typeCharRange = SourceRange.Empty;
		bool isCall = (tToken.Type == Tokens.Call);
		SourceRange	callRange = SourceRange.Empty;
		if (isCall)
			callRange = tToken.Range;
		string prevName = la.Value;
		bool isIndexer = false;
		ResetPeek();
		Token peekToken = Peek();
		if (peekToken.Type != Tokens.Of)
		{
			isIndexer = LocalVarArrayCollection.IsVarArrayName(prevName);
		}
		VarIdentifierOrKeyword();
		string typeCharacterStr = GetTypeCharacter();
		if (typeCharacterStr != null)
			typeCharRange = tToken.Range;
		if ((isCall && IsStatementTerminator()) || (!isIndexer && IsMethodReferenceExpression()))
		{
			result = new MethodReferenceExpression(prevName, GetRange(startRange, tToken.Range));
			result.SetRange(GetRange(startRange, tToken.Range));
			if (isCall && IsStatementTerminator())
			{
				result = MethodReferenceToMethodCall(result as MethodReferenceExpression, null);				
				result.SetRange(GetRange(callRange, tToken.Range));
				(result as MethodCallExpression).NameRange = startRange;
			}						
		}
		else
		{
			if (typeCharacterStr != null && typeCharacterStr.Length > 0)
				result = new TypedElementReferenceExpression(prevName, typeCharacterStr[0], typeCharRange);
			else
				result = new ElementReferenceExpression(prevName);					
			(result as ElementReferenceExpression).NameRange = startRange;
			result.SetRange(GetRange(startRange, tToken.Range));
		}		
	}
	void LogicalNotOpInPrimaryExpression(out Expression result)
	{
		result = null;
		LogicalInversion inversion = null; 
		LogicalInversion topInversion = null; 
		LanguageElementCollection coll = new LanguageElementCollection();	
		Expect(Tokens.Not );
		inversion = new LogicalInversion();
		inversion.Name = tToken.Value;
		inversion.UnaryOperator = UnaryOperatorType.LogicalNot;
		inversion.SetOperatorRange(tToken.Range);
		inversion.SetRange(tToken.Range);
		coll.Add(inversion);
		topInversion = inversion;
		while (la.Type == Tokens.Not )
		{
			Get();
			LogicalInversion nestedInversion = new LogicalInversion();
			inversion.Expression = nestedInversion;
			inversion = nestedInversion;			
			inversion.Name = tToken.Value;
			inversion.UnaryOperator = UnaryOperatorType.LogicalNot;
			inversion.SetOperatorRange(tToken.Range);
			inversion.SetRange(tToken.Range);
			coll.Add(inversion);
		}
		RelationalOp(out result);
		if (inversion != null)
		{
			inversion.Expression = result;
			result = topInversion;
			foreach(LanguageElement element in coll)
				element.SetRange(element.Range.Start, tToken.Range.End);
		}
	}
	void RelationalOp(out Expression result)
	{
		TypeReferenceExpression tempPrimitiveTypeName = null;
		RelationalOperator relationType = RelationalOperator.None;
		string opName = String.Empty;
		result = null;
		bool  isTypeOf = false;
		bool inIs = false;
		bool inIsNot = false;
		SourceRange typeOfStartRange = la.Range;
		VBDeclarator decl = null;
		SourceRange operatorStartRange = SourceRange.Empty;
		if (la.Type == Tokens.TypeOf )
		{
			Get();
			isTypeOf = true;
		}
		ShiftOp(out result);
		while (StartOf(37))
		{
			if (!IsRelationalOp(la))
			{
				break;
			}
			if (operatorStartRange == SourceRange.Empty)
				operatorStartRange = la.Range;
			if (StartOf(38))
			{
				switch (la.Type)
				{
				case Tokens.NotEquals : 
				{
					Get();
					relationType = RelationalOperator.Inequality;
					break;
				}
				case Tokens.EqualsSymbol : 
				{
					EqualOperator();
					relationType = RelationalOperator.Equality;
					break;
				}
				case Tokens.LessThan : 
				{
					Get();
					relationType = RelationalOperator.LessThan;
					while (la.Type == Tokens.LineTerminator )
					{
						Get();
					}
					break;
				}
				case Tokens.LessOrEqual : 
				{
					Get();
					relationType = RelationalOperator.LessOrEqual;
					break;
				}
				case Tokens.GreaterThan : 
				{
					Get();
					relationType = RelationalOperator.GreaterThan;
					while (la.Type == Tokens.LineTerminator )
					{
						Get();
					}
					break;
				}
				case Tokens.GreaterOrEqual : 
				{
					Get();
					relationType = RelationalOperator.GreaterOrEqual;
					break;
				}
				case Tokens.Like : 
				{
					Get();
					relationType = RelationalOperator.Like;
					break;
				}
				case Tokens.Is : 
				{
					Get();
					inIs = true;
					if (isTypeOf)
					{
						TypeName(out tempPrimitiveTypeName, out decl);
						if (isTypeOf && tempPrimitiveTypeName != null && result != null)
						{
							TypeOfIsExpression typeOfIsExpression = new TypeOfIsExpression(result, tempPrimitiveTypeName);
							if (result != null)
							{
								typeOfIsExpression.AddDetailNode(result);
							}
							if (tempPrimitiveTypeName != null)
							{
								typeOfIsExpression.AddDetailNode(tempPrimitiveTypeName);
							}
							typeOfIsExpression.SetRange(GetRange(typeOfStartRange, tToken.Range));
							result = typeOfIsExpression;
							isTypeOf = false;
							inIs = false;
							continue;
						} 
					}
					break;
				}
				case Tokens.IsNot : 
				{
					Get();
					inIsNot = true;
					break;
				}
				}
			}
			Expression right = null;
			opName = tToken.Value;
			if (la.Type == Tokens.TypeOf )
			{
				Get();
				isTypeOf = true;
				typeOfStartRange = tToken.Range;
			}
			ShiftOp(out right);
			if (inIs)
			{
				   if (result != null)
				   {
				  SourceRange tempRange = result.Range;
				  result = new Is(result, right);
				  result.SetRange(GetRange(tempRange, tToken.Range));
				  inIs = false;
				   }
			}
			else if (inIsNot)
			{
				   if (result != null)
				   {
				  SourceRange tempRange = result.Range;
				  result = new IsNot(result, right);
				  result.SetRange(GetRange(tempRange, tToken.Range));
				  inIsNot = false;
				   }
			}
			else
			{
				result = GetRelationalOperation(result, opName, relationType, right, operatorStartRange);
			}
			if (result != null)
			{
				OperatorExpression opExp = result as OperatorExpression;
				if (opExp != null)
				{
					opExp.SetOperatorRange(operatorStartRange);
				}
			}
		}
	}
	void UnaryExpressionInPrimaryExpression(out Expression result)
	{
		result = null;
		UnaryOperatorExpression unaryOp = null; 
		UnaryOperatorExpression topUnaryOp = null; 
		LanguageElementCollection coll = new LanguageElementCollection();	
		UnaryOperatorType typeOperator = UnaryOperatorType.None;
		if (la.Type == Tokens.Plus )
		{
			Get();
			typeOperator = UnaryOperatorType.UnaryPlus; 
		}
		else if (la.Type == Tokens.Minus )
		{
			Get();
			typeOperator = UnaryOperatorType.UnaryNegation; 
		}
		else
			SynErr(303);
		unaryOp = new UnaryOperatorExpression();
		topUnaryOp = unaryOp;
		unaryOp.Name = tToken.Value;
		unaryOp.UnaryOperator = typeOperator;
		unaryOp.SetOperatorRange(tToken.Range);
		unaryOp.SetRange(tToken.Range);
		coll.Add(unaryOp);
		while (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			if (la.Type == Tokens.Plus )
			{
				Get();
				typeOperator = UnaryOperatorType.UnaryPlus; 
			}
			else
			{
				Get();
				typeOperator = UnaryOperatorType.UnaryNegation; 
			}
			UnaryOperatorExpression nestedUnaryOp = new UnaryOperatorExpression();
			unaryOp.Expression = nestedUnaryOp;
			unaryOp = nestedUnaryOp;
			unaryOp.Name = tToken.Value;
			unaryOp.UnaryOperator = typeOperator;
			unaryOp.SetOperatorRange(tToken.Range);
			unaryOp.SetRange(tToken.Range);
			coll.Add(unaryOp);
		}
		ExponentiationOp(out result);
		if (unaryOp != null)
		{
			foreach(LanguageElement element in coll)
				element.SetRange(GetRange(element.Range, tToken.Range));
			unaryOp.Expression = result;
			result = topUnaryOp;
		}
	}
	void ExponentiationOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		PrimaryExpression(out result);
		while (la.Type == Tokens.XorSymbol )
		{
			Get();
			binaryType = BinaryOperatorType.Exponentiation;
			opName = tToken.Value;
			Expression right = null;
			nameRange = tToken.Range;
			PrimaryExpression(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void XmlElementReferenceExpressionRule(ref Expression result)
	{
		SourceRange startRange = la.Range;
		string name = String.Empty;
		Expect(Tokens.LessThan );
		name = GetOneTagXmlValue(true);
		Expect(Tokens.GreaterThan );
		SourceRange nameRange = GetRange(startRange, tToken.Range);
		result = GetXmlElementReference(result, name, nameRange);
		if (result != null)
		{
			SourceRange nodeRange = GetRange(result.Range, tToken.Range);
			result.SetRange(nodeRange);
		}
	}
	void ReferenceExpressionRule(ref Expression result, SourceRange startRange, bool isCall, SourceRange callRange, ref bool isBaseConstructorCall)
	{
		Token tempToken = null;
		Token identifierOrKeyword = la;
		IdentifierOrKeyword(out tempToken);
		if (IsMethodReferenceExpression() || (isCall && IsStatementTerminator()))
		{
			if ((result is BaseReferenceExpression) && (tToken.Type == Tokens.New))
			{
				isBaseConstructorCall = true;
			}
			MethodReferenceExpression mre = new MethodReferenceExpression(result, tToken.Value, tToken.Range);
			mre.SetRange(GetRange(startRange, tToken.Range));
			if (result != null)
			{
				mre.AddNode(result);
			}
			if (isCall && IsStatementTerminator())
			{
				result = MethodReferenceToMethodCall(mre, null, isBaseConstructorCall);								
				result.SetRange(GetRange(callRange, tToken.Range));
			}
			else
			{
				result = mre;
			}
		}
		else
		{
			result = GetElementReference(result, identifierOrKeyword.Value, identifierOrKeyword.Range);
		}
	}
	void TryCast(out Expression result)
	{
		result = null;
		VBDeclarator decl = null;
		TypeReferenceExpression type = null;	
		SourceRange startRange = la.Range;
		Expect(Tokens.TryCast );
		Expect(Tokens.ParenOpen );
		ExpressionRule(out result);
		Expect(Tokens.Comma );
		TypeName(out type, out decl);
		Expect(Tokens.ParenClose );
		result = CreateConditionalTypeCast(result, type);
		result.SetRange(GetRange(startRange, tToken.Range)); 
	}
	void XmlCommentStringRule(Expression result)
	{
		if (result == null || !(result is XmlExpression) || !XmlCommentStringIsNextNode())
		return;
		string value = String.Empty;
		Comment comment = null;
		StatementTerminatorCall();
		Expect(Tokens.XmlCommentString );
		comment = new Comment();
		comment.Name = tToken.Value;
		comment.SetRange(tToken.Range);
		result.AddNode(comment);
		StatementTerminatorCall();
		while (la.Type == Tokens.XmlCommentString )
		{
			Get();
			comment = new Comment();
			comment.Name = tToken.Value;
			comment.SetRange(tToken.Range);
			result.AddNode(comment);
			StatementTerminator();
		}
		if (comment != null)
		{
			result.SetRange(GetRange(result.Range, comment.Range));
		}
	}
	void XmlExpressionRule(out Expression result)
	{
		result = null;
		if(_NotXmlNode)
			return;
		if (LessThanOrcas())
		{
		if (la.Type == Tokens.LessThan )
		{
			Get();
		}
		else if (la.Type == Tokens.ShiftLeft )
		{
			Get();
		}
		else
			SynErr(304);
		return;
		}
		result = CreateXmlExpression();
		if (result != null)
		{
		XmlCommentStringRule(result);
		}
	}
	void LambdaExpression(out Expression result)
	{
		SourceRange startRange = la.Range;
		LambdaExpression lambdaExpression = null;
		Token startBlockToken = la;
		bool isFunction = false;
		 bool isAsync = false;
		 if (tToken.Type == Tokens.Identifier && tToken.Value.ToLower() == "async")
		 {
		   SetKeywordTokenCategory(tToken);
		   isAsync = true;
		   startBlockToken = tToken;
		   startRange = tToken.Range;
		 }
		 bool oldIsAsynchronousContext = IsAsynchronousContext;
		 IsAsynchronousContext = isAsync;
		if (la.Type == Tokens.Function )
		{
			Get();
			isFunction = true; 
		}
		else if (la.Type == Tokens.Sub )
		{
			Get();
		}
		else
			SynErr(305);
		if (isFunction)
		 lambdaExpression = new LambdaFunctionExpression();
		else
		  lambdaExpression = new LambdaExpression();
		 lambdaExpression.IsAsynchronous = isAsync;
		LambdaSignature(lambdaExpression);
		if (la.Type == Tokens.As )
		{
			TypeReferenceExpression type = null;
			VBDeclarator decl = null;
			Get();
			TypeName(out type, out decl);
			if (isFunction)
			 ((LambdaFunctionExpression)lambdaExpression).Type = type;
		}
		if (la.Type != Tokens.LineTerminator)
		{
			if (isFunction)
			{
				Expression expression;
				ExpressionRule(out expression);
				if (expression != null)
				lambdaExpression.AddNode(expression);
			}
			else if (StartOf(16))
			{
				Statement statement;
				LanguageElement oldContext = Context;
				SetContext(lambdaExpression);
				StatementRule(out statement);
				SetContext(oldContext);
			}
			else
				SynErr(306);
		}
		else if (StartOf(39))
		{
			while (la.Type == Tokens.LineTerminator )
			{
				Get();
			}
			LanguageElement oldContext = Context;
			SetContext(lambdaExpression);
			Block(startBlockToken);
			SetContext(oldContext);
			Expect(Tokens.EndToken );
			if (la.Type == Tokens.Function )
			{
				Get();
			}
			else if (la.Type == Tokens.Sub )
			{
				Get();
			}
			else
				SynErr(307);
		}
		else
			SynErr(308);
		lambdaExpression.SetRange(GetRange(startRange, tToken));
		result  = lambdaExpression;
		 IsAsynchronousContext = oldIsAsynchronousContext;
	}
	void AwaitExpression(out Expression result)
	{
		Expression expression;
		Token startToken = la;
		Expect(Tokens.Identifier );
		SetKeywordTokenCategory(tToken); 
		PrimaryExpression(out expression);
		result = new AwaitExpression(expression);
		result.SetRange(GetRange(startToken, tToken));
	}
	void ArrayInitializerExpressionRule(out Expression result)
	{
		result = null;
		ExpressionCollection coll = new ExpressionCollection();
		Expression expr = null;
		Token token = la;
		Expect(Tokens.CurlyBraceOpen );
		StatementTerminatorCall();
		if (StartOf(40))
		{
			while (StartOf(23))
			{
				if (la.Type == Tokens.Comma )
				{
					Get();
					StatementTerminatorCall();
				}
				ExpressionRule(out expr);
				if (expr != null)
				coll.Add(expr);
			}
			StatementTerminatorCall();
		}
		Expect(Tokens.CurlyBraceClose );
		result = new ArrayInitializerExpression(coll);
		result.AddDetailNodes(coll);
		result.SetRange(GetRange(token, tToken));
		result.NameRange = result.InternalRange;
	}
	void ConditionalExpression(out Expression result)
	{
		result = null;
		Expression condition = null;
		Expression firstPart = null;
		Expression secondPart = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.IfToken );
		Expect(Tokens.ParenOpen );
		ExpressionRule(out condition);
		Expect(Tokens.Comma );
		ExpressionRule(out firstPart);
		if (la.Type == Tokens.Comma )
		{
			Get();
			ExpressionRule(out secondPart);
		}
		Expect(Tokens.ParenClose );
		result = CreateIfExpression(condition, firstPart, secondPart);
		if (result == null)
			return;
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void SqlExpression(out Expression result)
	{
		result = new QueryExpression();
		SourceRange startRange = la.Range;
		Expression exp = null;
		FromClause(out exp);
		result.AddDetailNode(exp);
		if (IsSelectOrFromExpression())
		  if(la.Type == Tokens.LineTerminator)
			Get();
		while (StartOf(41))
		{
			QueryOperator(out exp);
			result.AddDetailNode(exp);
			if (IsSelectOrFromExpression())
				if(la.Type == Tokens.LineTerminator)
				  Get();
		}
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void LogicalXorOp(out Expression result)
	{
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		LogicalOrOp(out result);
		while (la.Type == Tokens.Xor )
		{
			Get();
			nameRange = tToken.Range;
			Expression right = null;
			opName = tToken.Value;
			LogicalOrOp(out right);
			result = GetLogicalOperation(result, opName, LogicalOperator.ExclusiveOr, right, nameRange);
		}
	}
	void LogicalOrOp(out Expression result)
	{
		string opName = String.Empty;
		LogicalOperator logicType = LogicalOperator.None;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		LogicalAndOp(out result);
		while (la.Type == Tokens.Or  || la.Type == Tokens.OrElse )
		{
			if (la.Type == Tokens.Or )
			{
				Get();
				logicType = LogicalOperator.Or; 
			}
			else
			{
				Get();
				logicType = LogicalOperator.ShortCircuitOr; 
			}
			nameRange = tToken.Range;
			Expression right = null;
			opName = tToken.Value;
			LogicalAndOp(out right);
			result = GetLogicalOperation(result, opName,  logicType, right, nameRange);
		}
	}
	void LogicalAndOp(out Expression result)
	{
		string opName = String.Empty;
		LogicalOperator logicType = LogicalOperator.None;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		LogicalNotOp(out result);
		while (la.Type == Tokens.And  || la.Type == Tokens.AndAlso )
		{
			if (la.Type == Tokens.And )
			{
				Get();
				logicType = LogicalOperator.And; 
			}
			else
			{
				Get();
				logicType = LogicalOperator.ShortCircuitAnd; 
			}
			nameRange = tToken.Range;
			Expression right = null;
			opName = tToken.Value;
			LogicalNotOp(out right);
			result = GetLogicalOperation(result, opName, logicType, right, nameRange);
		}
	}
	void LogicalNotOp(out Expression result)
	{
		result = null;
		LogicalInversion inversion = null; 
		LogicalInversion topInversion = null; 
		LanguageElementCollection coll = new LanguageElementCollection();	
		while (la.Type == Tokens.Not )
		{
			Get();
			if (inversion == null)
			{
				inversion = new LogicalInversion();
				topInversion = inversion;
			}
			else
			{
				LogicalInversion nestedInversion = new LogicalInversion();
				inversion.Expression = nestedInversion;
				inversion = nestedInversion;
			}
			inversion.Name = tToken.Value;
			inversion.UnaryOperator = UnaryOperatorType.LogicalNot;
			inversion.SetOperatorRange(tToken.Range);
			inversion.SetRange(tToken.Range);
			coll.Add(inversion);
		}
		RelationalOp(out result);
		if (inversion != null)
		{
			inversion.Expression = result;
			result = topInversion;
			foreach(LanguageElement element in coll)
				element.SetRange(element.Range.Start, tToken.Range.End);
		}
	}
	void ShiftOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		ConcatenationOp(out result);
		while (la.Type == Tokens.ShiftLeft  || la.Type == Tokens.ShiftRight )
		{
			if (la.Type == Tokens.ShiftLeft )
			{
				Get();
				binaryType = BinaryOperatorType.ShiftLeft;
			}
			else
			{
				Get();
				binaryType = BinaryOperatorType.ShiftRight;
			}
			nameRange = tToken.Range;
			Expression right = null;
			opName = tToken.Value;
			ConcatenationOp(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void ConcatenationOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		AdditiveOp(out result);
		while (la.Type == Tokens.BitAnd )
		{
			Get();
			binaryType = BinaryOperatorType.Concatenation; 
			Expression right = null;
			opName = tToken.Value;
			nameRange = tToken.Range;
			AdditiveOp(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void AdditiveOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		ModulusOp(out result);
		while (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			if (la.Type == Tokens.Plus )
			{
				Get();
				binaryType = BinaryOperatorType.Add;
			}
			else
			{
				Get();
				binaryType = BinaryOperatorType.Subtract;
			}
			Expression right = null;
			opName = tToken.Value;
			nameRange = tToken.Range;
			ModulusOp(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void ModulusOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		IntegerDivisionOp(out result);
		while (la.Type == Tokens.Mod )
		{
			Get();
			binaryType = BinaryOperatorType.Modulus;
			Expression right = null;
			opName = tToken.Value;
			nameRange = tToken.Range;
			IntegerDivisionOp(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void IntegerDivisionOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		MultiplicativeOp(out result);
		while (la.Type == Tokens.BackSlash )
		{
			Get();
			binaryType = BinaryOperatorType.IntegerDivision;
			Expression right = null;
			opName = tToken.Value;
			nameRange = tToken.Range;
			MultiplicativeOp(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void MultiplicativeOp(out Expression result)
	{
		BinaryOperatorType binaryType = BinaryOperatorType.None;
		string opName = String.Empty;
		result = null;
		SourceRange nameRange = SourceRange.Empty;
		UnaryExpression(out result);
		while (la.Type == Tokens.Asterisk  || la.Type == Tokens.Slash )
		{
			if (la.Type == Tokens.Asterisk )
			{
				Get();
				binaryType = BinaryOperatorType.Multiply; 
			}
			else
			{
				Get();
				binaryType = BinaryOperatorType.Divide; 
			}
			opName = tToken.Value;
			Expression right = null;
			nameRange = tToken.Range;
			UnaryExpression(out right);
			result = GetBinaryExpression(result, opName, binaryType, right, nameRange);
		}
	}
	void UnaryExpression(out Expression result)
	{
		result = null;
		UnaryOperatorExpression unaryOp = null; 
		UnaryOperatorExpression topUnaryOp = null; 
		UnaryOperatorType typeOperator = UnaryOperatorType.None;
		LanguageElementCollection coll = new LanguageElementCollection();	
		while (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			if (la.Type == Tokens.Plus )
			{
				Get();
				typeOperator = UnaryOperatorType.UnaryPlus; 
			}
			else
			{
				Get();
				typeOperator = UnaryOperatorType.UnaryNegation; 
			}
			if (unaryOp == null)
			{
				unaryOp = new UnaryOperatorExpression();
				topUnaryOp = unaryOp;
			}
			else
			{
				UnaryOperatorExpression nestedUnaryOp = new UnaryOperatorExpression();
				nestedUnaryOp.UnaryOperator = typeOperator;
				unaryOp.Expression = nestedUnaryOp;
				unaryOp = nestedUnaryOp;
			}
			unaryOp.Name = tToken.Value;
			unaryOp.UnaryOperator = typeOperator;
			unaryOp.SetOperatorRange(tToken.Range);
			unaryOp.SetRange(tToken.Range);
			coll.Add(unaryOp);
		}
		ExponentiationOp(out result);
		if (unaryOp != null)
		{
			foreach(LanguageElement element in coll)
				element.SetRange(GetRange(element.Range, tToken.Range));
			unaryOp.Expression = result;
			result = topUnaryOp;
		}
	}
	void CreationExpressionCore(out Expression result,  ref VBDeclarator decl)
	{
		TypeReferenceExpression type = null;
		result = null;
		SourceRange startRange = la.Range;
		CreateElementType createElementType = CreateElementType.None;
		if (decl != null)
		{
			createElementType = decl.CreateElementType;
		}
		Expect(Tokens.New );
		if (la != null && la.Type != Tokens.With)
		{
		TypeNameCore(out type, out decl);
		}
		  _IsNewContext = true;
		InitializeParenthesis(out result, ref type, createElementType, startRange);
		_IsNewContext = false; 
	}
	void ArrayOrCreationParenthesis(out ExpressionCollection arguments, out int rank)
	{
		arguments = null;
		rank = 0;
		Expect(Tokens.ParenOpen );
		if (StartOf(4))
		{
			ArgumentList(out arguments, out rank);
		}
		if (arguments != null && arguments.Count > 0)
		{
		  ExpressionCollection oldArguments = arguments;
		arguments = new ExpressionCollection();
		foreach(Expression expression in oldArguments)
		  if(expression.ElementType != LanguageElementType.EmptyArgumentExpression)
			arguments.Add(expression);
		 }
		Expect(Tokens.ParenClose );
	}
	void TypeNameCore(out TypeReferenceExpression result, out VBDeclarator declarator)
	{
		result = null;
		QualifiedIdentifier identifier = null;
		declarator = new VBDeclarator();	
		QualifiedIdentifier(out identifier, CreateElementType.TypeReferenceExpression);
		result = identifier.Expression as TypeReferenceExpression;				
		if (result != null)
			declarator.FullTypeName = result.Name;
	}
	void InitializeParenthesis(out Expression result, ref TypeReferenceExpression type, CreateElementType createElementType, SourceRange startRange)
	{
		result = null;
		ExpressionCollection arguments = null;
		Expression initializer = null;
		bool isFirstType = true;
		int rank = 0;
		TypeReferenceExpression firstTypeReferenceExpression = null;
		ExpressionCollection firstArguments = null;
		int firstRank = 0;
		SourceRange startElementCreationRange = la.Range;
		SourceRange parenOpenRange = la.Range;
		SourceRange parensRange = new SourceRange(la.Range.Start);
		while (la.Type == Tokens.ParenOpen )
		{
			ArrayOrCreationParenthesis(out arguments, out rank);
			parensRange = GetRange(parenOpenRange, tToken.Range);
			if (createElementType == CreateElementType.ObjectCreationExpression)
			{				
				result = GetObjectCreationExpression(type, arguments, startRange, tToken.Range, parensRange);
			if (la.Type == Tokens.With  || la.Type == Tokens.From )
			{
				AdditionObjectCreationInitializer(result as ObjectCreationExpression);
			}
			result.SetRange(GetRange(startRange, tToken.Range));
			return;
			}
			else if (createElementType == CreateElementType.ArrayCreationExpression || !isFirstType)
			{
				if (firstTypeReferenceExpression != null)
				{
					type = GetArrayTypeReference(firstTypeReferenceExpression, firstArguments, firstRank);
					firstTypeReferenceExpression = null;
				}				
				SourceRange typeStartRange = SourceRange.Empty;
				type =  GetArrayTypeReference(type, arguments, rank);				
			}
			if (isFirstType)
			{
				firstTypeReferenceExpression =  type;
				firstArguments = arguments;
				firstRank = rank;
			}
			isFirstType = false;
		}
		if (StartOf(26))
		{
			if (la.Type == Tokens.CurlyBraceOpen)
			{
				if (firstTypeReferenceExpression != null)
						type = GetArrayTypeReference(firstTypeReferenceExpression, firstArguments, firstRank);
			InitializerClause(out initializer);
			if (initializer != null && initializer is ArrayInitializerExpression)
			{
				result = GetArrayCreateExpression(type, initializer as ArrayInitializerExpression, startRange, tToken.Range);
				createElementType = CreateElementType.ArrayCreationExpression;
			}
			}
		}
		if (result == null && createElementType != CreateElementType.ArrayCreationExpression)
		{
			result = GetObjectCreationExpression(type, arguments, startRange, tToken.Range, parensRange);
		}
		if (la.Type == Tokens.With  || la.Type == Tokens.From )
		{
			AdditionObjectCreationInitializer(result as ObjectCreationExpression);
		}
		if (result != null)
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void WithInitializersCore(out Expression result)
	{
		result = new ObjectInitializerExpression();
		ExpressionCollection coll = ((ObjectInitializerExpression)result).Initializers;
		SourceRange startRange = la.Range;
		Expect(Tokens.With );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		Expect(Tokens.CurlyBraceOpen );
		StatementTerminatorCall();
		if (StartOf(4))
		{
			WithInitializerList(ref coll);
			if (coll != null)
			{
				foreach(LanguageElement element in coll)
				{
					result.AddDetailNode(element);
				}
			}
		}
		StatementTerminatorCall();
		Expect(Tokens.CurlyBraceClose );
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void WithInitializerList(ref ExpressionCollection coll)
	{
		Expression element = null;
		MemberInitializerExpressionRule(out element);
		if (element != null)
		coll.Add(element);
		while (la.Type == Tokens.Comma )
		{
			Get();
			MemberInitializerExpressionRule(out element);
			if (element != null)
			coll.Add(element);
		}
	}
	void MemberInitializerExpressionRule(out Expression result)
	{
		result = null;
		SourceRange startRange = la.Range;
		string name = String.Empty;
		Expression exp = null;
		bool isKey = false;
		if (la.Type == Tokens.KeyToken )
		{
			Get();
			isKey = true;
		}
		if (la.Type == Tokens.Dot )
		{
			Get();
			result = new MemberInitializerExpression();
			if (isKey)
			{
				(result as MemberInitializerExpression).IsKey = true;
			}
			result.NameRange = la.Range;
			Get(); 
			result.Name = tToken.Value;
			EqualOperator();
			ExpressionRule(out exp);
			(result as MemberInitializerExpression).Value = exp;
			result.AddDetailNode(exp);
		}
		else if (StartOf(4))
		{
			ElementReferenceQualifiedIdentifier(out result);
			if (result != null && result is ElementReferenceExpression && isKey)
			(result as ElementReferenceExpression).IsKey = true;
		}
		else
			SynErr(309);
		if (result != null)
		{
			result.SetRange(GetRange(startRange, tToken.Range));
		}
	}
	void AdditionObjectCreationInitializer(ObjectCreationExpression result)
	{
		if (result == null)
		return;
		Expression exp = null;
		if (la.Type == Tokens.With )
		{
			WithInitializersCore(out exp);
		}
		else if (la.Type == Tokens.From )
		{
			FromInitializerCore(out exp);
		}
		else
			SynErr(310);
		if (result != null && exp != null && exp is ObjectInitializerExpression)
		{
			result.ObjectInitializer = exp as ObjectInitializerExpression;
			result.AddDetailNode(exp);
		}
	}
	void InitializerClause(out Expression result)
	{
		SourceRange startRange = la.Range;
		result = null;
		if (la.Type == Tokens.From )
		{
			Get();
		}
		ExpressionRule(out result);
		if (result != null)
		{
			result.SetRange(GetRange(startRange, result.Range));
		}
	}
	void FromInitializerCore(out Expression result)
	{
		ObjectCollectionInitializer objectCollection = new ObjectCollectionInitializer();
		result = objectCollection;
		SourceRange startRange = la.Range;
		LanguageElementList coll;
		Expect(Tokens.From );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		Expect(Tokens.CurlyBraceOpen );
		StatementTerminatorCall();
		if (StartOf(26))
		{
			ExpressionList(out coll);
			if (coll != null)
			{
			  ExpressionCollection objectInitializers = objectCollection.Initializers;
				foreach(LanguageElement element in coll)
				{
					objectCollection.AddDetailNode(element);
					objectInitializers.Add(element);
				}
			}
		}
		StatementTerminatorCall();
		Expect(Tokens.CurlyBraceClose );
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void Argument(out Expression result)
	{
		result = null;
		SourceRange startRange = la.Range;
		Expression leftPart = null;
		if (IsNamedAssign())
		{
			ElementReferenceQualifiedIdentifier(out leftPart);
			Expect(Tokens.ColonEquals );
			ExpressionRule(out result);
			result = GetAttributeVariableInitializer(leftPart, result, GetRange(startRange, tToken.Range));
		}
		else if (StartOf(26))
		{
			BoundsElement(out result);
		}
		else
			SynErr(311);
	}
	void TypeArgumentList(out ExpressionCollection result, out int typeArity)
	{
		result = new ExpressionCollection();
		typeArity = 1;
		TypeReferenceExpression argument = null;
		VBDeclarator decl = null;
		TypeName(out argument, out decl);
		if (argument != null)
		result.Add(argument);
		while (la.Type == Tokens.Comma )
		{
			Get();
			typeArity++; 
			TypeName(out argument, out decl);
			if (argument != null)
			result.Add(argument);
		}
	}
	void TypeParameters(out TypeParameterCollection result)
	{
		result = new TypeParameterCollection();
		TypeParameter par = null;
		TypeParameterElement(out par);
		result.Add(par);
		while (la.Type == Tokens.Comma )
		{
			Get();
			if (StartOf(42))
			{
				TypeParameterElement(out par);
				result.Add(par);
			}
		}
	}
	void TypeParameterElement(out TypeParameter result)
	{
		result = null;
		TypeParameterConstraintCollection coll = null;
		SourceRange startRange = la.Range;
		TypeParameterDirection typeArgumentDirection = TypeParameterDirection.None;
		if (la.Type == Tokens.In  || la.Type == Tokens.Out )
		{
			if (la.Type == Tokens.In )
			{
				Get();
				typeArgumentDirection = TypeParameterDirection.In; 
			}
			else
			{
				Get();
				typeArgumentDirection = TypeParameterDirection.Out; 
			}
		}
		Token nameToken = la;
		VarKeyword();
		if (la.Type == Tokens.As )
		{
			TypeParameterConstraintRule(out coll);
		}
		result = new TypeParameter(nameToken.Value, coll, GetRange(startRange, tToken.Range));
		result.NameRange = nameToken.Range;
		result.Direction = typeArgumentDirection;
	}
	void TypeParameterConstraintRule(out TypeParameterConstraintCollection result)
	{
		result = null;
		TypeParameterConstraint paramConstraint = null;
		Expect(Tokens.As );
		if (la.Type == Tokens.CurlyBraceOpen )
		{
			TypeParameterConstraintList(out result);
		}
		else if (StartOf(4))
		{
			TypeParameterConstraintElement(out paramConstraint);
			result = new TypeParameterConstraintCollection();
			if (paramConstraint != null)
				result.Add(paramConstraint);
		}
		else
			SynErr(312);
	}
	void TypeParameterConstraintList(out TypeParameterConstraintCollection result)
	{
		TypeParameterConstraint paramConstraint = null;
		result = new TypeParameterConstraintCollection();
		Expect(Tokens.CurlyBraceOpen );
		TypeParameterConstraintElement(out paramConstraint);
		if (paramConstraint != null)
		result.Add(paramConstraint);
		while (la.Type == Tokens.Comma )
		{
			Get();
			TypeParameterConstraintElement(out paramConstraint);
			if (paramConstraint != null)
			result.Add(paramConstraint);
		}
		Expect(Tokens.CurlyBraceClose );
	}
	void TypeParameterConstraintElement(out TypeParameterConstraint result)
	{
		result = null;
		TypeReferenceExpression type = null;
		SourceRange startRange = la.Range;
		VBDeclarator decl = null;
		TypeName(out type, out decl);
		SourceRange range = GetRange(startRange, tToken.Range);
		if (tToken.Type == Tokens.New)
		  result = new NewTypeParameterConstraint(tToken.Value, range);
		else if (tToken.Type == Tokens.Class)
		  result = new ClassTypeParameterConstraint(tToken.Value, range);
		else if (tToken.Type == Tokens.Structure)
		  result = new StructTypeParameterConstraint(tToken.Value, range);
		else
		{
		  result = new NamedTypeParameterConstraint(type);
		  result.SetRange(range);
		}
		if (type != null)
		 result.Name = type.Name;
	}
	void ParameterRule(out Param param)
	{
		param = null;
		TypeReferenceExpression type = null;
		ArgumentDirection argDir = ArgumentDirection.In;
		 SourceRange directionRange = SourceRange.Empty;
		SourceRange startRange = la.Range;
		bool isOptional = false;
		VBDeclarator decl = null;
		string typeNameTail = String.Empty;
		SourceRange operatorRange = SourceRange.Empty;
		LanguageElementCollection arrayModifiers = null;
		 NullableTypeModifier nullableModifier = null;
		if (StartOf(20))
		{
			DeclaratorType dummyType;
			DeclaratorQualifier(out dummyType);
		}
		if (la.Type == Tokens.Optional )
		{
			Get();
			isOptional = true; 
		}
		ParameterQualifierRule(out argDir, out directionRange);
		if (la.Type == Tokens.Optional )
		{
			Get();
			isOptional = true; 
		}
		param = new Param(la.Value);
		param.IsOptional = isOptional;
		param.Direction = argDir;
		param.NameRange = la.Range;
		   param.DirectionRange = directionRange;
		VarIdentifier(out decl);
		SetParamProperties(param, decl, out arrayModifiers, out nullableModifier, out type);
		if (la.Type == Tokens.As )
		{
			Get();
			param.AsRange = tToken.Range; 
			TypeName(out type, out decl);
		}
		if (type != null)
		{
			  string memberType = GetMemberType(type, arrayModifiers);
			if (memberType != null && memberType != String.Empty)
				param.MemberType = memberType;
			param.MemberTypeReference = ToTypeReferenceExpression(type, arrayModifiers, nullableModifier, param);		  
			param.TypeRange = type.Range;
			param.AddDetailNode(param.MemberTypeReference);
			param.SetFullTypeName(VB90Tokens.Instance.GetFullTypeName(type.Name));					
		}
		if (la.Type == Tokens.EqualsSymbol )
		{
			EqualOperator();
			Expression init = null;
			operatorRange = tToken.Range;
			ExpressionRule(out init);
			if (init != null)
			{
				param.DefaultValueExpression = init;
				param.DefaultValue = init.ToString();
				param.AddDetailNode(init);
			}
		}
		if (param != null)
		{
			param.SetRange(GetRange(startRange, tToken.Range));
			if (operatorRange != SourceRange.Empty)
			{
				param.OperatorRange = operatorRange;
			}
		}
	}
	void ParameterQualifierRule(out ArgumentDirection result, out SourceRange directionRange)
	{
		result = ArgumentDirection.In;
		directionRange = SourceRange.Empty;
		while (la.Type == Tokens.ByRef  || la.Type == Tokens.ByVal  || la.Type == Tokens.ParamArray )
		{
			if (la.Type == Tokens.ByRef )
			{
				Get();
				result = ArgumentDirection.Ref; 
			}
			else if (la.Type == Tokens.ByVal )
			{
				Get();
				result = ArgumentDirection.In; directionRange = tToken.Range; 
			}
			else
			{
				Get();
				result = ArgumentDirection.ParamArray; 
			}
		}
	}
	void ProcedureModifier(out Token modifier)
	{
		modifier = null;
		switch (la.Type)
		{
		case Tokens.MustOverride : 
		{
			Get();
			break;
		}
		case Tokens.MustInherit : 
		{
			Get();
			break;
		}
		case Tokens.Default : 
		{
			Get();
			break;
		}
		case Tokens.Friend : 
		{
			Get();
			break;
		}
		case Tokens.Shadows : 
		{
			Get();
			break;
		}
		case Tokens.Overrides : 
		{
			Get();
			break;
		}
		case Tokens.Private : 
		{
			Get();
			break;
		}
		case Tokens.Protected : 
		{
			Get();
			break;
		}
		case Tokens.Public : 
		{
			Get();
			break;
		}
		case Tokens.NotInheritable : 
		{
			Get();
			break;
		}
		case Tokens.NotOverridable : 
		{
			Get();
			break;
		}
		case Tokens.Shared : 
		{
			Get();
			break;
		}
		case Tokens.Overridable : 
		{
			Get();
			break;
		}
		case Tokens.Overloads : 
		{
			Get();
			break;
		}
		case Tokens.ReadOnly : 
		{
			Get();
			break;
		}
		case Tokens.WriteOnly : 
		{
			Get();
			break;
		}
		case Tokens.Widening : 
		{
			Get();
			break;
		}
		case Tokens.Narrowing : 
		{
			Get();
			break;
		}
		case Tokens.Partial : 
		{
			Get();
			break;
		}
		case Tokens.WithEvents : 
		{
			Get();
			break;
		}
		default: SynErr(313); break;
		}
		modifier = tToken; 
	}
	void LambdaSignature(LambdaExpression result)
	{
		if (la.Type == Tokens.ParenOpen )
		{
			Get();
			result.ParamOpenRange = tToken.Range;
			if (StartOf(43))
			{
				LambdaParameterList(result);
			}
			Expect(Tokens.ParenClose );
			result.ParamCloseRange = tToken.Range;
		}
	}
	void LambdaParameterList(LambdaExpression result)
	{
		StructuralParser.Param param = null;
		LambdaOrSimpleParam(out param);
		if (param != null && result != null)
		result.AddParameter(param);
		while (la.Type == Tokens.Comma )
		{
			Get();
			LambdaOrSimpleParam(out param);
			if (param != null && result != null)
			result.AddParameter(param);
		}
	}
	void LambdaOrSimpleParam(out StructuralParser.Param param)
	{
		param = null;
		if (IsLambdaParameter())
		{
			LambdaParameter(out param);
		}
		else if (StartOf(43))
		{
			Param vbParam = null; 
			ParameterRule(out vbParam);
			param = vbParam ; 
		}
		else
			SynErr(314);
	}
	void LambdaParameter(out StructuralParser.Param param)
	{
		param = null;
		ArgumentDirection argDir = ArgumentDirection.In;
		 SourceRange directionRange = SourceRange.Empty;
		SourceRange startRange = la.Range;
		VBDeclarator decl = null;
		ParameterQualifierRule(out argDir, out directionRange);
		VarIdentifier(out decl);
		param = new LambdaImplicitlyTypedParam(tToken.Value);
		param.Direction = argDir;
		  param.DirectionRange = directionRange;
		param.SetRange(GetRange(startRange, tToken.Range));
		param.NameRange = tToken.Range;
		SetParamProperties(param as Param, decl);
	}
	void FromClause(out Expression result)
	{
		result = null;
		if (la.Type == Tokens.From )
		{
			FromOperator(out result);
		}
		else if (la.Type == Tokens.Aggregate )
		{
			AggregateOperator(out result);
		}
		else
			SynErr(315);
	}
	void QueryOperator(out Expression result)
	{
		result = null;
		switch (la.Type)
		{
		case Tokens.From : 
case Tokens.Aggregate : 
		{
			FromClause(out result);
			break;
		}
		case Tokens.Select : 
		{
			SelectOperator(out result);
			break;
		}
		case Tokens.Distinct : 
		{
			DistinctOperator(out result);
			break;
		}
		case Tokens.Where : 
		{
			WhereOperator(out result);
			break;
		}
		case Tokens.Order : 
		{
			OrderByOperator(out result);
			break;
		}
		case Tokens.Skip : 
		{
			SkipOperator(out result);
			break;
		}
		case Tokens.Take : 
		{
			TakeOperator(out result);
			break;
		}
		case Tokens.Join : 
		{
			JoinOperator(out result);
			break;
		}
		case Tokens.Let : 
		{
			LetOperator(out result);
			break;
		}
		case Tokens.Group : 
		{
			if (IsGroupJoinOperator())
			{
			GroupJoinOperator(out result);
			}
			else
			{
			GroupByOperator(out result);
			}
			break;
		}
		default: SynErr(316); break;
		}
	}
	void InExpressionRule(out InExpression result)
	{
		result = null;
		QueryIdent ident = null;
		Expression expression = null;
		SourceRange startRange = la.Range;
		bool hasIn = false;
		InExpressionType type = InExpressionType.InExpression;
		QueryIdent(out ident);
		if (la.Type == Tokens.In )
		{
			Get();
			hasIn = true; 
			ExpressionRule(out expression);
		}
		if (!hasIn && expression == null)
		{
			expression = GetInitializer(ident);
			type = InExpressionType.LetExpression;
		}
		result = new InExpression(ident, expression);
		result.InExpressionType = type;
		if (ident != null)
			result.AddDetailNode(ident);
		if (expression != null)
			result.AddDetailNode(expression);
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void QueryIdent(out QueryIdent ident)
	{
		ident = null;
		QueryIdentBase(out ident, DeclaratorType.QueryIdent);
	}
	void QueryIdentBase(out QueryIdent ident, DeclaratorType declaratorType)
	{
		ident = new QueryIdent();
		SourceRange startRange = la.Range;
		BaseVariable result = null;
		DeclVariableCoreWithoutSpecifier(out result, declaratorType);
		ident = result as QueryIdent; 
	}
	void DistinctOperator(out Expression result)
	{
		result = new DistinctExpression();
		result.SetRange(la.Range);
		Expect(Tokens.Distinct );
	}
	void SelectOperator(out Expression vbSelect)
	{
		vbSelect = null;
		SourceRange startRange = la.Range;
		SelectExpression result = new SelectExpression();
		LanguageElementCollection coll = null;
		Expect(Tokens.Select );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		QueryIdentOrExpressionList(out coll);
		SetSelectCollection(result, coll);
		result.SetRange(GetRange(startRange, tToken.Range));
		vbSelect = result;
	}
	void WhereOperator(out Expression whereExpression)
	{
		whereExpression = null;
		Expression expression = null;
		whereExpression = null;
		SourceRange startRange = la.Range;
		WhereExpression result = new WhereExpression();
		Expect(Tokens.Where );
		ExpressionRule(out expression);
		result.SetWhereClause(expression);
		result.SetRange(GetRange(startRange, tToken));
		whereExpression = result;
	}
	void OrderByOperator(out Expression orderBy)
	{
		orderBy = null;	
		SourceRange startRange = la.Range;
		OrderByExpression result = new OrderByExpression();
		Expect(Tokens.Order );
		Expect(Tokens.By );
		OrderingList(result);
		result.SetRange(GetRange(startRange, tToken));
		orderBy = result;
	}
	void SkipOperator(out Expression result)
	{
		result = null;
		Expression exp = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.Skip );
		if (la.Type == Tokens.While )
		{
			Get();
			result = new SkipWhileExpression();
		}
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		if (result == null)
		{
			result = new SkipExpression();
		}
		ExpressionRule(out exp);
		SkipExpressionBase skipExpr = result as SkipExpressionBase;
		if (skipExpr == null)
			return;
		if (exp != null)
		{
			skipExpr.Expression = exp;
			skipExpr.AddDetailNode(exp);
		}
		skipExpr.SetRange(GetRange(startRange, tToken.Range));
	}
	void TakeOperator(out Expression result)
	{
		result = null;
		Expression exp = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.Take );
		if (la.Type == Tokens.While )
		{
			Get();
			result = new TakeWhileExpression();
		}
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		if (result == null)
		{
			result = new TakeExpression();
		}
		ExpressionRule(out exp);
		TakeExpressionBase takeExpr = result as TakeExpressionBase;
		if (takeExpr == null)
			return;
		if (exp != null)
		{
			takeExpr.Expression = exp;
			takeExpr.AddDetailNode(exp);
		}
		takeExpr.SetRange(GetRange(startRange, tToken.Range));
	}
	void JoinOperator(out Expression joinExpression)
	{
		JoinExpressionBase result = new JoinExpression();
		SourceRange startRange = la.Range;
		JoinBaseOperator(result);
		result.SetRange(GetRange(startRange, tToken.Range));
		joinExpression = result;
	}
	void LetOperator(out Expression result)
	{
		result = new LetExpression();
		result.SetRange(la.Range);
		QueryIdent ident = null;
		Expect(Tokens.Let );
		QueryIdent(out ident);
		if (ident != null)
		(result as LetExpression).AddDeclaration(ident);
		while (la.Type == Tokens.Comma )
		{
			Get();
			QueryIdent(out ident);
			if (ident != null)
			(result as LetExpression).AddDeclaration(ident);
		}
		result.SetRange(GetRange(result, tToken));
	}
	void GroupJoinOperator(out Expression joinExpression)
	{
		JoinExpressionBase result = new JoinIntoExpression();
		SourceRange startRange = la.Range;
		Expect(Tokens.Group );
		JoinBaseOperator(result);
		IntoTailRuleForGroup(result);
		result.SetRange(GetRange(startRange, tToken.Range));
		joinExpression = result;
	}
	void GroupByOperator(out Expression groupExp)
	{
		groupExp = null;
		GroupByExpression result = new GroupByExpression();
		LanguageElementCollection coll = null;
		SourceRange startRange = la.Range;
		Expect(Tokens.Group );
		if (la != null && la.Type != Tokens.By)
		{
		if (StartOf(26))
		{
			QueryIdentOrExpressionList(out coll);
			SetGroupCollection(result, coll);
			coll = null;
		}
		}  
		Expect(Tokens.By );
		QueryIdentOrExpressionList(out coll);
		SetGroupByCollection(result, coll);
		IntoTailRuleForGroup(result);
		result.SetRange(GetRange(startRange, tToken.Range));
		groupExp = result;
	}
	void EqualsExpressionRule(out EqualsExpression result)
	{
		result = null;
		Expression left = null;
		Expression right = null;
		SourceRange startRange = la.Range;
		LogicalNotOp(out left);
		Expect(Tokens.EqualsToken );
		LogicalNotOp(out right);
		result = new EqualsExpression(left, right);
		result.SetRange(GetRange(startRange, tToken.Range));
	}
	void JoinBaseOperator(JoinExpressionBase result)
	{
		if (result == null)
		return;
		Expect(Tokens.Join );
		JoinSourceRule(result);
		JoinOrGroupJoinList(result);
		OnQueryOperator(result);
	}
	void IntoTailRuleForGroup(Expression result)
	{
		IntoTailRuleBase(result, DeclaratorType.CanAggregateElement);
	}
	void JoinSourceRule(JoinExpressionBase result)
	{
		if (result == null)
		return;
		InExpression inExpression = null;
		InExpressionRule(out inExpression);
		result.SetInExpression(inExpression);
	}
	void JoinOrGroupJoinList(JoinExpressionBase result)
	{
		if (result == null)
		return;
		Expression exp = null;
		while (la.Type == Tokens.Join  || la.Type == Tokens.Group )
		{
			if (la.Type == Tokens.Join )
			{
				JoinOperator(out exp);
			}
			else
			{
				GroupJoinOperator(out exp);
			}
			JoinExpressionBase joinExp = exp as JoinExpressionBase;
			if (joinExp != null)
				result.AddJoinExpression(joinExp);
		}
	}
	void OnQueryOperator(JoinExpressionBase result)
	{
		if (result == null)
		return;
		EqualsExpression exp = null;
		Expect(Tokens.On );
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
		EqualsExpressionRule(out exp);
		AddEqualsExpression(result, exp);
		while (la.Type == Tokens.And )
		{
			Get();
			EqualsExpressionRule(out exp);
			AddEqualsExpression(result, exp);
		}
	}
	void FromOperator(out Expression vbFromExp)
	{
		vbFromExp = null;
		FromExpression result = new FromExpression();
		SourceRange startRange = la.Range;
		Expect(Tokens.From );
		FromDeclarationList(result);
		result.SetRange(GetRange(startRange, tToken.Range));
		vbFromExp = result;
	}
	void AggregateOperator(out Expression aggregateExpression)
	{
		AggregateExpression result = new AggregateExpression();	
		SourceRange startRange = la.Range;
		Expression exp = null;
		Expect(Tokens.Aggregate );
		FromDeclarationList(result);
		if (IsSelectOrFromExpression())
		 while(la.Type == Tokens.LineTerminator)
		   Get();
		while (StartOf(41))
		{
			QueryOperator(out exp);
			if (exp != null)
			{
				result.AddQueryOperator(exp);
				result.AddDetailNode(exp);
			}
		}
		IntoTailRule(result);
		result.SetRange(GetRange(startRange, tToken.Range));
		aggregateExpression = result;
	}
	void FromDeclarationList(FromExpression result)
	{
		if (result == null)
		return;
		InExpression inExpression = null;
		InExpressionRule(out inExpression);
		result.AddInExpression(inExpression);
		while (la.Type == Tokens.Comma )
		{
			Get();
			InExpressionRule(out inExpression);
			result.AddInExpression(inExpression);
		}
	}
	void IntoTailRule(Expression result)
	{
		IntoTailRuleBase(result, DeclaratorType.CanAggregateFunction);
	}
	void QueryIdentOrExpressionList(out LanguageElementCollection result)
	{
		result = null;
		QueryIdentOrExpressionListBase(out result, DeclaratorType.QueryIdent);
	}
	void IntoTailRuleBase(Expression result, DeclaratorType declaratorType)
	{
		if (result == null)
		return;
		LanguageElementCollection coll = null;
		Expect(Tokens.Into );
		QueryIdentOrExpressionListBase(out coll, declaratorType);
		SetIntoCollection(result, coll);
	}
	void QueryIdentOrExpressionListBase(out LanguageElementCollection result, DeclaratorType declaratorType)
	{
		result = new LanguageElementCollection();
		LanguageElement element = null;
		QueryIdentOrExpression(out element, declaratorType);
		if (element != null)
		{
			result.Add(element);
		}
		while (la.Type == Tokens.Comma )
		{
			Get();
			QueryIdentOrExpression(out element, declaratorType);
			if (element != null)
			{
				result.Add(element);
			}
		}
	}
	void QueryIdentOrExpression(out LanguageElement result,  DeclaratorType declaratorType)
	{
		result = null;
		if (!IsQueryIdentForDeclaration(declaratorType))
		{
			Expression exp = null;
		ExpressionRule(out exp);
		result = CreateAggregateExpressionIfNeeded(exp, declaratorType);
		}
		else
		{
			QueryIdent ident = null;
		QueryIdentBase(out ident, declaratorType);
		result = ident;
		}
	}
	void OrderingList(OrderByExpression orderBy)
	{
		OrderingExpression ordering = null;
		OrderingRule(out ordering);
		if (orderBy != null)
		orderBy.AddOrdering(ordering);
		while (la.Type == Tokens.Comma )
		{
			Get();
			OrderingRule(out ordering);
			if (orderBy != null)
			orderBy.AddOrdering(ordering);
		}
	}
	void OrderingRule(out OrderingExpression ordering)
	{
		Expression expression = null;
		ordering = new OrderingExpression();
		SourceRange startRange = la.Range;
		ExpressionRule(out expression);
		ordering.SetOrdering(expression);
		if (la.Type == Tokens.Ascending  || la.Type == Tokens.Descending )
		{
			if (la.Type == Tokens.Ascending )
			{
				Get();
				ordering.Order = OrderingType.Ascending; 
			}
			else
			{
				Get();
				ordering.Order = OrderingType.Descending; 
			}
		}
		ordering.SetRange(GetRange(startRange, tToken));
	}
	protected void Parse()
	{
	  PrepareParse();
  		ParserRoot();
	  if (SetTokensCategory)
		while (la != null && la.Type != 0)
		  Get();
	  FinishParse(RootNode);
	}
	protected override bool[,] CreateSetArray()
	{
	  bool[,] set =
	  {
				{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, T,x,x,T, x,T,T,T, x,x,x,x, x,x,x,T, T,T,x,x, T,T,x,x, x,x,T,T, x,T,x,T, x,x,x,x, T,T,T,x, T,T,T,T, T,x,x,T, x,x,x,x, x,T,x,x, T,T,x,x, T,x,x,x, T,T,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, T,x,x,T, x,T,T,T, x,x,x,x, x,x,x,T, T,T,x,x, T,T,x,x, x,x,T,T, x,T,x,T, x,x,x,x, T,T,T,x, T,T,T,T, T,x,x,T, x,x,x,x, x,T,x,x, T,T,x,x, T,x,x,x, T,T,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, T,x,x,T, x,T,T,T, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,T,T, x,T,x,T, x,x,x,x, T,T,T,x, T,T,T,T, T,x,x,T, x,x,x,x, x,T,x,x, T,T,x,x, T,x,x,x, T,T,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,T,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, x,x,x,x, x,x,T,x, T,x,x,T, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, T,x,x,T, x,T,T,T, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, x,x,T,T, x,T,x,T, x,x,x,x, T,T,T,x, T,T,T,T, T,x,x,T, x,x,x,x, x,T,x,x, T,T,x,x, T,x,x,x, T,T,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,T,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, T,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,x, x,x,T,x, x,T,T,x, T,T,x,T, x,T,x,T, T,x,T,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,T,T,T, x,T,T,x, x,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,T,T, T,T,T,x, x,x,T,x, T,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,T,x, T,T,x,T, T,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,T,x, T,T,x,T, T,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,T,x, T,T,x,T, T,x,x,T, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,T,T,x, T,T,x,T, T,x,T,T, x,T,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, T,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,T,T,x, x,x,x,x, x,T,T,x, T,T,x,T, x,T,x,T, T,x,T,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,T,T,T, x,T,T,x, x,x,T,T, x,x,T,T, x,T,T,x, T,x,T,T, T,x,T,T, T,T,T,x, x,x,T,x, T,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,T,x, T,T,x,x, x,x,x,x, x,T,x,T, T,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,x, x,x,T,x, x,T,T,x, T,T,x,T, x,T,x,T, T,x,T,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, T,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,T,T,T, x,T,T,x, x,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,T,T, T,T,T,x, x,x,T,x, T,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,T,x, T,T,x,x, x,x,x,x, x,T,x,T, T,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,x, T,T,T,x, x,T,T,x, T,T,x,T, x,T,x,T, T,x,T,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,T,T,T, x,T,T,x, x,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,T,T, T,T,T,x, x,x,T,x, T,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, T,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, x,x,T,T, T,T,T,T, T,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, x,x,T,T, T,T,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,T,T,T, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,T, T,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, T,T,T,T, x,x,T,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, T,T,x,x, x,x,x,x, x,T,x,T, T,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,x,x,T, T,T,T,x, x,x,T,x, x,T,T,x, T,T,x,T, x,T,x,T, T,x,T,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, T,T,T,T, x,T,T,x, x,T,T,T, T,x,T,T, x,T,T,x, T,x,T,T, T,x,T,T, T,T,T,x, x,x,T,x, T,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x},
		{x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,T,T,T, x,T,T,x, T,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,T, T,T,T,T, T,x,T,T, x,T,T,T, T,T,T,T, T,T,T,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,x,x,x, T,x,x,x, T,x,T,x, x,T,T,T, x,x,x,T, x,T,x,x, x,x,T,T, x,T,T,x, T,T,x,x, T,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,x,T,x, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,T,x, T,x,x,x, T,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x}
	  };
	  return set;
	}
  } 
	public class VB90ParserErrors : ParserErrorsBase
	{
		protected override string GetSyntaxErrorText(int n)
		{
			string s;
			switch (n)
			{
				case 0: s = "EOF expected"; break;
			case 1: s = "EOL expected"; break;
			case 2: s = "INTEGERLITERAL expected"; break;
			case 3: s = "FLOATINGPOINTLITERAL expected"; break;
			case 4: s = "IDENTIFIER expected"; break;
			case 5: s = "CONSTDIRECTIVE expected"; break;
			case 6: s = "IFDIRECTIVE expected"; break;
			case 7: s = "ENDIFDIRECTIVE expected"; break;
			case 8: s = "ELSEIFDIRECTIVE expected"; break;
			case 9: s = "ELSEDIRECTIVE expected"; break;
			case 10: s = "REGION expected"; break;
			case 11: s = "ENDREGION expected"; break;
			case 12: s = "EXTERNALSOURCEDIRECTIVE expected"; break;
			case 13: s = "ENDEXTERNALSOURCEDIRECTIVE expected"; break;
			case 14: s = "LINECONTINUATION expected"; break;
			case 15: s = "SINGLELINECOMMENT expected"; break;
			case 16: s = "CHARACTERLITERAL expected"; break;
			case 17: s = "STRINGLITERAL expected"; break;
			case 18: s = "PLUSEQUAL expected"; break;
			case 19: s = "MINUSEQUAL expected"; break;
			case 20: s = "MULEQUAL expected"; break;
			case 21: s = "DIVEQUAL expected"; break;
			case 22: s = "BACKSLASHEQUALS expected"; break;
			case 23: s = "XOREQUAL expected"; break;
			case 24: s = "ANDEQUAL expected"; break;
			case 25: s = "SHIFTLEFT expected"; break;
			case 26: s = "SHIFTRIGHT expected"; break;
			case 27: s = "SHIFTRIGHTEQUAL expected"; break;
			case 28: s = "SHIFTLEFTEQUAL expected"; break;
			case 29: s = "NOTEQUALS expected"; break;
			case 30: s = "LESSOREQUAL expected"; break;
			case 31: s = "GREATEROREQUAL expected"; break;
			case 32: s = "COMMA expected"; break;
			case 33: s = "CURLYBRACEOPEN expected"; break;
			case 34: s = "CURLYBRACECLOSE expected"; break;
			case 35: s = "PARENOPEN expected"; break;
			case 36: s = "PARENCLOSE expected"; break;
			case 37: s = "DOT expected"; break;
			case 38: s = "COLON expected"; break;
			case 39: s = "COLONEQUALS expected"; break;
			case 40: s = "PLUS expected"; break;
			case 41: s = "MINUS expected"; break;
			case 42: s = "ASTERISK expected"; break;
			case 43: s = "SLASH expected"; break;
			case 44: s = "BACKSLASH expected"; break;
			case 45: s = "EXCLAMATIONSYMBOL expected"; break;
			case 46: s = "XORSYMBOL expected"; break;
			case 47: s = "EQUALSSYMBOL expected"; break;
			case 48: s = "GREATERTHAN expected"; break;
			case 49: s = "LESSTHAN expected"; break;
			case 50: s = "BITAND expected"; break;
			case 51: s = "SHARP expected"; break;
			case 52: s = "ADDHANDLER expected"; break;
			case 53: s = "ADDRESSOF expected"; break;
			case 54: s = "ALIAS expected"; break;
			case 55: s = "AND expected"; break;
			case 56: s = "ANDALSO expected"; break;
			case 57: s = "AS expected"; break;
			case 58: s = "BOOLEAN expected"; break;
			case 59: s = "BYREF expected"; break;
			case 60: s = "BYTE expected"; break;
			case 61: s = "BYVAL expected"; break;
			case 62: s = "CALL expected"; break;
			case 63: s = "CASE expected"; break;
			case 64: s = "CATCH expected"; break;
			case 65: s = "CBOOL expected"; break;
			case 66: s = "CBYTE expected"; break;
			case 67: s = "CCHAR expected"; break;
			case 68: s = "CDATE expected"; break;
			case 69: s = "CDBL expected"; break;
			case 70: s = "CDEC expected"; break;
			case 71: s = "CHAR expected"; break;
			case 72: s = "CINT expected"; break;
			case 73: s = "CLASS expected"; break;
			case 74: s = "CLNG expected"; break;
			case 75: s = "COBJ expected"; break;
			case 76: s = "CONST expected"; break;
			case 77: s = "CONTINUE expected"; break;
			case 78: s = "CSBYTE expected"; break;
			case 79: s = "CSHORT expected"; break;
			case 80: s = "CSNG expected"; break;
			case 81: s = "CSTR expected"; break;
			case 82: s = "CTYPE expected"; break;
			case 83: s = "CUINT expected"; break;
			case 84: s = "CULNG expected"; break;
			case 85: s = "CUSHORT expected"; break;
			case 86: s = "DATE expected"; break;
			case 87: s = "DECIMAL expected"; break;
			case 88: s = "DECLARE expected"; break;
			case 89: s = "DEFAULT expected"; break;
			case 90: s = "DELEGATE expected"; break;
			case 91: s = "DIM expected"; break;
			case 92: s = "DIRECTCAST expected"; break;
			case 93: s = "DO expected"; break;
			case 94: s = "DOUBLE expected"; break;
			case 95: s = "EACH expected"; break;
			case 96: s = "ELSE expected"; break;
			case 97: s = "ELSEIF expected"; break;
			case 98: s = "ENDTOKEN expected"; break;
			case 99: s = "ENDIF expected"; break;
			case 100: s = "ENUM expected"; break;
			case 101: s = "ERASE expected"; break;
			case 102: s = "ERROR expected"; break;
			case 103: s = "EVENT expected"; break;
			case 104: s = "EXIT expected"; break;
			case 105: s = "FALSE expected"; break;
			case 106: s = "FINALLY expected"; break;
			case 107: s = "FOR expected"; break;
			case 108: s = "FRIEND expected"; break;
			case 109: s = "FUNCTION expected"; break;
			case 110: s = "GET expected"; break;
			case 111: s = "GETTYPE expected"; break;
			case 112: s = "GLOBAL expected"; break;
			case 113: s = "GOSUB expected"; break;
			case 114: s = "GOTO expected"; break;
			case 115: s = "HANDLES expected"; break;
			case 116: s = "IFTOKEN expected"; break;
			case 117: s = "IMPLEMENTS expected"; break;
			case 118: s = "IMPORTS expected"; break;
			case 119: s = "IN expected"; break;
			case 120: s = "OUT expected"; break;
			case 121: s = "INHERITS expected"; break;
			case 122: s = "INTEGER expected"; break;
			case 123: s = "INTERFACE expected"; break;
			case 124: s = "IS expected"; break;
			case 125: s = "ISNOT expected"; break;
			case 126: s = "ISFALSE expected"; break;
			case 127: s = "ISTRUE expected"; break;
			case 128: s = "LET expected"; break;
			case 129: s = "LIB expected"; break;
			case 130: s = "LIKE expected"; break;
			case 131: s = "LONG expected"; break;
			case 132: s = "LOOP expected"; break;
			case 133: s = "ME expected"; break;
			case 134: s = "MOD expected"; break;
			case 135: s = "MODULE expected"; break;
			case 136: s = "MUSTINHERIT expected"; break;
			case 137: s = "MUSTOVERRIDE expected"; break;
			case 138: s = "MYBASE expected"; break;
			case 139: s = "MYCLASS expected"; break;
			case 140: s = "NAMESPACE expected"; break;
			case 141: s = "NARROWING expected"; break;
			case 142: s = "NEW expected"; break;
			case 143: s = "NEXT expected"; break;
			case 144: s = "NOT expected"; break;
			case 145: s = "NOTHING expected"; break;
			case 146: s = "NOTINHERITABLE expected"; break;
			case 147: s = "NOTOVERRIDABLE expected"; break;
			case 148: s = "OBJECT expected"; break;
			case 149: s = "OF expected"; break;
			case 150: s = "ON expected"; break;
			case 151: s = "OPERATOR expected"; break;
			case 152: s = "OPTION expected"; break;
			case 153: s = "OPTIONAL expected"; break;
			case 154: s = "OR expected"; break;
			case 155: s = "ORELSE expected"; break;
			case 156: s = "OVERLOADS expected"; break;
			case 157: s = "OVERRIDABLE expected"; break;
			case 158: s = "OVERRIDES expected"; break;
			case 159: s = "PARAMARRAY expected"; break;
			case 160: s = "PARTIAL expected"; break;
			case 161: s = "PRIVATE expected"; break;
			case 162: s = "PROPERTY expected"; break;
			case 163: s = "PROTECTED expected"; break;
			case 164: s = "PUBLIC expected"; break;
			case 165: s = "PRESERVE expected"; break;
			case 166: s = "RAISEEVENT expected"; break;
			case 167: s = "READONLY expected"; break;
			case 168: s = "REDIM expected"; break;
			case 169: s = "REMOVEHANDLER expected"; break;
			case 170: s = "RESUME expected"; break;
			case 171: s = "RETURN expected"; break;
			case 172: s = "REM expected"; break;
			case 173: s = "SBYTE expected"; break;
			case 174: s = "SELECT expected"; break;
			case 175: s = "SET expected"; break;
			case 176: s = "SHADOWS expected"; break;
			case 177: s = "SHARED expected"; break;
			case 178: s = "SHORT expected"; break;
			case 179: s = "SINGLE expected"; break;
			case 180: s = "STATIC expected"; break;
			case 181: s = "STEP expected"; break;
			case 182: s = "STOP expected"; break;
			case 183: s = "STRING expected"; break;
			case 184: s = "STRUCTURE expected"; break;
			case 185: s = "SUB expected"; break;
			case 186: s = "SYNCLOCK expected"; break;
			case 187: s = "THEN expected"; break;
			case 188: s = "THROW expected"; break;
			case 189: s = "TOTOKEN expected"; break;
			case 190: s = "TRUE expected"; break;
			case 191: s = "TRY expected"; break;
			case 192: s = "TRYCAST expected"; break;
			case 193: s = "TYPEOF expected"; break;
			case 194: s = "UINTEGER expected"; break;
			case 195: s = "ULONG expected"; break;
			case 196: s = "USHORT expected"; break;
			case 197: s = "USING expected"; break;
			case 198: s = "UNTIL expected"; break;
			case 199: s = "VARIANT expected"; break;
			case 200: s = "WEND expected"; break;
			case 201: s = "WHEN expected"; break;
			case 202: s = "WHILE expected"; break;
			case 203: s = "WIDENING expected"; break;
			case 204: s = "WITH expected"; break;
			case 205: s = "WITHEVENTS expected"; break;
			case 206: s = "WRITEONLY expected"; break;
			case 207: s = "XOR expected"; break;
			case 208: s = "ADD expected"; break;
			case 209: s = "REMOVE expected"; break;
			case 210: s = "ANSI expected"; break;
			case 211: s = "ASSEMBLY expected"; break;
			case 212: s = "AUTO expected"; break;
			case 213: s = "UNICODE expected"; break;
			case 214: s = "EXPLICIT expected"; break;
			case 215: s = "STRICT expected"; break;
			case 216: s = "COMPARE expected"; break;
			case 217: s = "BINARY expected"; break;
			case 218: s = "TEXT expected"; break;
			case 219: s = "OFF expected"; break;
			case 220: s = "CUSTOM expected"; break;
			case 221: s = "FROMTOKEN expected"; break;
			case 222: s = "WHERE expected"; break;
			case 223: s = "JOIN expected"; break;
			case 224: s = "EQUALS expected"; break;
			case 225: s = "INTO expected"; break;
			case 226: s = "ORDER expected"; break;
			case 227: s = "BY expected"; break;
			case 228: s = "GROUP expected"; break;
			case 229: s = "ASCENDING expected"; break;
			case 230: s = "DESCENDING expected"; break;
			case 231: s = "QUESTION expected"; break;
			case 232: s = "DISTINCT expected"; break;
			case 233: s = "INFER expected"; break;
			case 234: s = "KEYTOKEN expected"; break;
			case 235: s = "AGGREGATE expected"; break;
			case 236: s = "SKIP expected"; break;
			case 237: s = "TAKE expected"; break;
			case 238: s = "OpenEmbeddedCodeTAG expected"; break;
			case 239: s = "CloseEmbeddedCodeTAG expected"; break;
			case 240: s = "CLOSETAG expected"; break;
			case 241: s = "SingleLineCloseTAG expected"; break;
			case 242: s = "CommAtSymbol expected"; break;
			case 243: s = "TripleDot expected"; break;
			case 244: s = "XmlComStr expected"; break;
			case 245: s = "DOLLARSYMBOL expected"; break;
			case 246: s = "PERCENTSYMBOL expected"; break;
			case 247: s = "SINGLELINEXMLCOMMENT expected"; break;
			case 248: s = "??? expected"; break;
			case 249: s = "invalid EndNamespaceOrType"; break;
			case 250: s = "invalid OptionDirective"; break;
			case 251: s = "invalid OptionDirective"; break;
			case 252: s = "invalid NamespaceDeclaration"; break;
			case 253: s = "invalid OptionValue"; break;
			case 254: s = "invalid VarKeyword"; break;
			case 255: s = "invalid ElementMemberDeclaration"; break;
			case 256: s = "invalid CorruptedEndDeclarationCore"; break;
			case 257: s = "invalid TypeDeclarationRule"; break;
			case 258: s = "invalid ElementDeclaration"; break;
			case 259: s = "invalid ElementDeclaration"; break;
			case 260: s = "invalid EnumerationDeclaration"; break;
			case 261: s = "invalid DelegateDeclaration"; break;
			case 262: s = "invalid PrimitiveTypeName"; break;
			case 263: s = "invalid TypeCharaterSymbol"; break;
			case 264: s = "invalid CharsetModifier"; break;
			case 265: s = "invalid MethodDeclaration"; break;
			case 266: s = "invalid MethodDeclaration"; break;
			case 267: s = "invalid HandlesOrImplement"; break;
			case 268: s = "invalid PropertyAccessorDeclaration"; break;
			case 269: s = "invalid EventAccessorDeclaration"; break;
			case 270: s = "invalid VarIdentifier"; break;
			case 271: s = "invalid VarIdentifierOrKeyword"; break;
			case 272: s = "invalid StatementRule"; break;
			case 273: s = "invalid EmbeddedStatement"; break;
			case 274: s = "invalid LabelName"; break;
			case 275: s = "invalid DeclaratorQualifier"; break;
			case 276: s = "invalid VariableDeclaratorCore"; break;
			case 277: s = "invalid ContinueStatement"; break;
			case 278: s = "invalid ExitStatement"; break;
			case 279: s = "invalid DeclareVariableAssignDim"; break;
			case 280: s = "invalid DoLoopStatement"; break;
			case 281: s = "invalid WhileOrUntil"; break;
			case 282: s = "invalid LoopControlVariable"; break;
			case 283: s = "invalid ForStatement"; break;
			case 284: s = "invalid SelectStatement"; break;
			case 285: s = "invalid UnstructuredErrorStatement"; break;
			case 286: s = "invalid ArrayHandlingStatement"; break;
			case 287: s = "invalid AddHandlerStatement"; break;
			case 288: s = "invalid OnErrorStatement"; break;
			case 289: s = "invalid PrimaryExpressionCore"; break;
			case 290: s = "invalid PrimaryExpressionCore"; break;
			case 291: s = "invalid PrimaryExpressionCore"; break;
			case 292: s = "invalid AssignmentOperator"; break;
			case 293: s = "invalid CastExpression"; break;
			case 294: s = "invalid CastTarget"; break;
			case 295: s = "invalid PrimitiveExpressionRule"; break;
			case 296: s = "invalid DateRule"; break;
			case 297: s = "invalid DateRule"; break;
			case 298: s = "invalid DateRule"; break;
			case 299: s = "invalid DateRule"; break;
			case 300: s = "invalid DateRule"; break;
			case 301: s = "invalid TrueOrFalseExpressionRule"; break;
			case 302: s = "invalid MeOrMyBaseOrMyClassRule"; break;
			case 303: s = "invalid UnaryExpressionInPrimaryExpression"; break;
			case 304: s = "invalid XmlExpressionRule"; break;
			case 305: s = "invalid LambdaExpression"; break;
			case 306: s = "invalid LambdaExpression"; break;
			case 307: s = "invalid LambdaExpression"; break;
			case 308: s = "invalid LambdaExpression"; break;
			case 309: s = "invalid MemberInitializerExpressionRule"; break;
			case 310: s = "invalid AdditionObjectCreationInitializer"; break;
			case 311: s = "invalid Argument"; break;
			case 312: s = "invalid TypeParameterConstraintRule"; break;
			case 313: s = "invalid ProcedureModifier"; break;
			case 314: s = "invalid LambdaOrSimpleParam"; break;
			case 315: s = "invalid FromClause"; break;
			case 316: s = "invalid QueryOperator"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}	
}
