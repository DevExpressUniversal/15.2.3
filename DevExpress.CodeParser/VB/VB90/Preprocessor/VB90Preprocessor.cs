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
namespace DevExpress.CodeRush.StructuralParser.VB.Preprocessor
#else
namespace DevExpress.CodeParser.VB.Preprocessor
#endif
{
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
	public partial class VBPreprocessor
	{
		protected void HandlePragmas()
		{
		}
			void PreprocessorRoot()
	{
		WhitespaceLines();
		if (la.Type == Tokens.Region  || la.Type == Tokens.EndRegion )
		{
			RegionGroup();
		}
		else if (StartOf(1))
		{
			ConditionGroup();
		}
		else if (la.Type == Tokens.ConstDirective )
		{
			ConstRule();
		}
		else if (la.Type == Tokens.ExternalSourceDirective  || la.Type == Tokens.EndExternalSourceDirective )
		{
			ExternalRule();
		}
		else
			SynErr(249);
		StatementTerminator();
	}
	void WhitespaceLines()
	{
		while (la.Type == Tokens.LineTerminator )
		{
			Get();
		}
	}
	void RegionGroup()
	{
		if (la.Type == Tokens.Region )
		{
			StartRegionRule();
		}
		else if (la.Type == Tokens.EndRegion )
		{
			EndRegionRule();
		}
		else
			SynErr(250);
	}
	void ConditionGroup()
	{
		bool condition;
		SourceRange startRange = la.Range;
		PreprocessorDirective result = null;
		if (la.Type == Tokens.IfDirective )
		{
			Get();
			ExpressionValue(out condition);
			if (la.Type == Tokens.Then )
			{
				Get();
			}
			ProcessIFDirectiveCondition(condition);
			IfDirective @if = new IfDirective();
			@if.ExpressionValue = condition;
			result = @if;
		}
		else if (la.Type == Tokens.ElseIfDirective )
		{
			Get();
			ExpressionValue(out condition);
			if (la.Type == Tokens.Then )
			{
				Get();
			}
			ProcessDirectiveCondition(condition);
			 ElifDirective elif = new ElifDirective();
			 elif.ExpressionValue = condition;
			result = elif;
		}
		else if (la.Type == Tokens.ElseDirective )
		{
			Get();
			ProcessDirectiveCondition(true);
			ElseDirective @else = new ElseDirective();
			@else.IsSatisfied = !ConditionWasTrue;
			result = @else;
		}
		else if (la.Type == Tokens.EndifDirective )
		{
			Get();
			result = new EndIfDirective();
			ProcessEndIf();
		}
		else
			SynErr(251);
		SourceRange rangeElement = SourceRangeUtils.GetRange(startRange, tToken.Range);
		result.SetRange(rangeElement);
		AddDirectiveNode(result);
	}
	void ConstRule()
	{
		Object condition;
		ConstDirective result = new ConstDirective();
		SourceRange startRange = la.Range;
		string name = null;
		Expect(Tokens.ConstDirective );
		Expect(Tokens.Identifier );
		name = tToken.Value; 
		Expect(Tokens.EqualsSymbol );
		Expression(out condition);
		AddConst(name, condition);
		SourceRange rangeElement = SourceRangeUtils.GetRange(startRange, tToken.Range);
		result.SetRange(rangeElement);
		AddDirectiveNode(result);
	}
	void ExternalRule()
	{
		if (la.Type == Tokens.ExternalSourceDirective )
		{
			EternalSourceBegin();
		}
		else if (la.Type == Tokens.EndExternalSourceDirective )
		{
			Get();
		}
		else
			SynErr(255);
	}
	void StatementTerminator()
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
	void EternalSourceBegin()
	{
		Expect(Tokens.ExternalSourceDirective );
		Expect(Tokens.ParenOpen );
		StringLiteralRule();
		Expect(Tokens.Comma );
		Expect(Tokens.IntegerLiteral );
		Expect(Tokens.ParenClose );
	}
	void StringLiteralRule()
	{
		if (la.Type == Tokens.StringLiteral )
		{
			Get();
		}
		else if (la.Type == Tokens.CharacterLiteral )
		{
			Get();
		}
		else
			SynErr(253);
	}
	void Expression(out Object result)
	{
		result = null;
		LogicalXorOp(out result);
	}
	void ExpressionValue(out bool result)
	{
		object @object;
		Expression(out @object);
		result = GetBool(@object);
	}
	void StartRegionRule()
	{
		RegionDirective regionDirective = new RegionDirective();
		regionDirective.SetRange(la.Range);
		regionDirective.SetStartTokenLength(7);
		Expect(Tokens.Region );
		if (la.Type == Tokens.CharacterLiteral  || la.Type == Tokens.StringLiteral )
		{
			StringLiteralRule();
			string name = tToken.Value;
			if (name != String.Empty)
			{
				int length = name.Length;
				if (length >= 2)
				{
					name = name.Substring(1, length - 2);
				}
				else
				{
					name = String.Empty;
				}
				regionDirective.Name = name;
			}
			regionDirective.NameRange = tToken.Range;
		}
		if (SourceFile != null)
		SourceFile.AddRegionDirective(regionDirective);
		StatementTerminator();
	}
	void EndRegionRule()
	{
		EndRegionDirective endRegionDirective = new EndRegionDirective();
		Expect(Tokens.EndRegion );
		if (StartOf(2))
		{
			IdentifierOrKeywordOrOperator();
		}
		endRegionDirective.SetRange(tToken.Range);
		if (SourceFile != null)
			SourceFile.AddEndRegionDirective(endRegionDirective);
		StatementTerminator();
	}
	void IdentifierOrKeywordOrOperator()
	{
		if (VB90Parser.IsIdentifierOrKeywordOrOperator(la))
		{
			Get();
		}
	}
	void SimpleName(out Object result)
	{
		result =  null;
		Expect(Tokens.Identifier );
		result = GetConstValue(tToken.Value);
	}
	void LogicalXorOp(out Object result)
	{
		result = null;
		LogicalOrOp(out result);
		while (la.Type == Tokens.Xor )
		{
			Get();
			Object right = null;
			int opType = Tokens.Xor;
			LogicalOrOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void LogicalOrOp(out Object result)
	{
		result = null;
		LogicalAndOp(out result);
		while (la.Type == Tokens.Or  || la.Type == Tokens.OrElse )
		{
			if (la.Type == Tokens.Or )
			{
				Get();
			}
			else
			{
				Get();
			}
			Object right;
			int opType = tToken.Type;
			LogicalAndOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void LogicalAndOp(out Object result)
	{
		result = null;
		LogicalNotOp(out result);
		while (la.Type == Tokens.And  || la.Type == Tokens.AndAlso )
		{
			if (la.Type == Tokens.And )
			{
				Get();
			}
			else
			{
				Get();
			}
			Object right = null;
			int opType = tToken.Type;
			LogicalNotOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void LogicalNotOp(out Object result)
	{
		result = null;
		bool isNot = false;
		while (la.Type == Tokens.Not )
		{
			Get();
			isNot = !isNot;
		}
		RelationalOp(out result);
		if (isNot)
		result = Eval(result, null, Tokens.Not);
	}
	void RelationalOp(out Object result)
	{
		result = null;
		ShiftOp(out result);
		while (StartOf(3))
		{
			switch (la.Type)
			{
			case Tokens.NotEquals : 
			{
				Get();
				break;
			}
			case Tokens.EqualsSymbol : 
			{
				Get();
				break;
			}
			case Tokens.LessThan : 
			{
				Get();
				break;
			}
			case Tokens.GreaterThan : 
			{
				Get();
				break;
			}
			case Tokens.LessOrEqual : 
			{
				Get();
				break;
			}
			case Tokens.GreaterOrEqual : 
			{
				Get();
				break;
			}
			}
			Object right = null;
			int opType = tToken.Type;
			ShiftOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void ShiftOp(out Object result)
	{
		result = null;
		ConcatenationOp(out result);
		while (la.Type == Tokens.ShiftLeft  || la.Type == Tokens.ShiftRight )
		{
			if (la.Type == Tokens.ShiftLeft )
			{
				Get();
			}
			else
			{
				Get();
			}
			Object right = null;
			int opType = tToken.Type;
			ConcatenationOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void ConcatenationOp(out Object result)
	{
		result = null;
		AdditiveOp(out result);
		while (la.Type == Tokens.BitAnd )
		{
			Get();
			Object right = null;
			int opType = tToken.Type;
			AdditiveOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void AdditiveOp(out Object result)
	{
		result = null;
		ModulusOp(out result);
		while (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			if (la.Type == Tokens.Plus )
			{
				Get();
			}
			else
			{
				Get();
			}
			Object right = null;
			int opType = tToken.Type;
			ModulusOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void ModulusOp(out Object result)
	{
		result = null;
		IntegerDivisionOp(out result);
		while (la.Type == Tokens.Mod )
		{
			Get();
			Object right = null;
			int opType = tToken.Type;
			IntegerDivisionOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void IntegerDivisionOp(out Object result)
	{
		result = null;
		MultiplicativeOp(out result);
		while (la.Type == Tokens.BackSlash )
		{
			Get();
			Object right = null;
			int opType = tToken.Type;
			MultiplicativeOp(out right);
			result = Eval(result, right, opType);
		}
	}
	void MultiplicativeOp(out Object result)
	{
		result = null;
		UnaryExpression(out result);
		while (la.Type == Tokens.Asterisk  || la.Type == Tokens.Slash )
		{
			if (la.Type == Tokens.Asterisk )
			{
				Get();
			}
			else
			{
				Get();
			}
			Object right = null;
			int opType = tToken.Type;
			UnaryExpression(out right);
			result = Eval(result, right, opType);
		}
	}
	void UnaryExpression(out Object result)
	{
		result = null;
		bool isMin = false;
		while (la.Type == Tokens.Plus  || la.Type == Tokens.Minus )
		{
			if (la.Type == Tokens.Plus )
			{
				Get();
			}
			else
			{
				Get();
				isMin = !isMin;
			}
		}
		ExponentiationOp(out result);
		if (isMin)
		result = Eval(result, null, Tokens.Minus);
	}
	void ExponentiationOp(out Object result)
	{
		result = null;
		PrimaryExpression(out result);
		while (la.Type == Tokens.BitAnd )
		{
			Get();
			Object right = null;
			int opType = tToken.Type;
			PrimaryExpression(out right);
			result = Eval(result, right, opType);
		}
	}
	void PrimaryExpression(out Object result)
	{
		result = null;
		if (StartOf(4))
		{
			Literal(out result);
		}
		else if (la.Type == Tokens.Identifier )
		{
			SimpleName(out result);
		}
		else if (la.Type == Tokens.ParenOpen )
		{
			ParenthesizedExpressionRule(out result);
		}
		else
			SynErr(254);
	}
	void ParenthesizedExpressionRule(out Object result)
	{
		result = null;
		Expect(Tokens.ParenOpen );
		Expression(out result);
		Expect(Tokens.ParenClose );
	}
	void Literal(out Object result)
	{
		result =  GetZeroInt();
		switch (la.Type)
		{
		case Tokens.IntegerLiteral : 
		{
			Get();
			result = GetIntegerValue(tToken.Value);
			break;
		}
		case Tokens.CharacterLiteral : 
		{
			Get();
			result = GetStringValueFromChar(tToken.Value);
			break;
		}
		case Tokens.FloatingPointLiteral : 
		{
			Get();
			result = GetDoubleValue(tToken.Value);
			break;
		}
		case Tokens.StringLiteral : 
		{
			Get();
			result = GetStringValue(tToken.Value);
			break;
		}
		case Tokens.False : 
case Tokens.True : 
		{
			BooleanLiteral(out result);
			break;
		}
		case Tokens.Nothing : 
		{
			Get();
			result = new Nothing();
			break;
		}
		default: SynErr(255); break;
		}
	}
	void BooleanLiteral(out Object val)
	{
		val = null;
		if (la.Type == Tokens.True )
		{
			Get();
			val = Evaluator.GetNumberFromCondition(true);
		}
		else if (la.Type == Tokens.False )
		{
			Get();
			val = Evaluator.GetNumberFromCondition(false);
		}
		else
			SynErr(256);
	}
		protected void StartRule()
		{
					PreprocessorRoot();
		}
		protected void WhitespaceLinesBase()
		{
			WhitespaceLines();
		}
		protected bool[,] CreateSetArray()
		{
			bool[,] set =
			{
				{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}
			};
			return set;
		}
	} 
	public class PreprocessorErrors : ParserErrorsBase
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
			case 249: s = "invalid PreprocessorRoot"; break;
			case 250: s = "invalid RegionGroup"; break;
			case 251: s = "invalid ConditionGroup"; break;
			case 252: s = "invalid ExternalRule"; break;
			case 253: s = "invalid StringLiteralRule"; break;
			case 254: s = "invalid PrimaryExpression"; break;
			case 255: s = "invalid Literal"; break;
			case 256: s = "invalid BooleanLiteral"; break;
				default:
					s = "error " + n;
					break;
			}
			return s;
		}
	}
}
