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
using System.Text;
using System.Collections;
using System.Collections.Specialized;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public abstract class VBTokensBase : LanguageTokensBase
	{
		#region Private fields...
		Hashtable _Keywords;
		Hashtable _Punctuators;
		Hashtable _Directives;
		Hashtable _Tokens;
		string[] tokens;
		Hashtable _PrimitiveToFullTypes;
		Hashtable _FullToPrimitiveTypes;
		static StringBuilder _Worker = new StringBuilder("");
		#endregion
		#region Const
		public const char IntegerTypeCharacter = '%';
		public const char LongTypeCharacter = '&';
		public const char DecimalTypeCharacter = '@';
		public const char SingleTypeCharacter = '!';
		public const char DoubleTypeCharacter = '#';
		public const char StringTypeCharacter = '$';
		public const char ShortCharacter = 'S';
		public const char IntegerCharacter = 'I';
		public const char LongCharacter = 'L';
		public const char SingleCharacter = 'F';
		public const char DoubleCharacter = 'R';
		public const char DecimalCharacter = 'D';
		#endregion
		#region VBTokensBase()
		protected VBTokensBase()
		{
			tokens = new string[1560];
			PopulateTokenTable();
		}
		#endregion
		void AddToken(int token, string name) 
		{
			tokens[token] = name;
		}
		void AddDirective(string name, int token) 
		{
			_Directives[name] = token;
			AddToken(token, name);
		}
		#region PopulateTokenTable
		private void PopulateTokenTable()
		{
			LoadKeyWords();
			LoadPunctuators();
			LoadDirectives();
			LoadTokens();
			LoadTypeConversionTables();
		}
		#endregion
		private void LoadTokens(Hashtable source)
		{
			foreach(DictionaryEntry e in source)
				_Tokens.Add(e.Key, e.Value);
		}
		private void LoadTokens()
		{
			_Tokens = CollectionsUtil.CreateCaseInsensitiveHashtable();
			LoadTokens(_Keywords);
			LoadTokens(_Directives);
			LoadTokens(_Punctuators);
		}
		#region LoadKeyWords
		private void LoadKeyWords()
		{
			_Keywords = CollectionsUtil.CreateCaseInsensitiveHashtable();
			StringCollection lKeywords = CreateKeywords();
			foreach(string lKeyword in lKeywords)
			{
				int token = TokenType.GetTokenType(lKeyword);
				AddToken(token, lKeyword);
				_Keywords[lKeyword] = token;
			}
		}
		#endregion
		#region LoadPunctuator
		private void LoadPunctuator(int type)
		{
			string lPunctuator = VBPunctuators.GetPunctuator(type);
			AddToken(type, lPunctuator);
			if (lPunctuator!=null)
				_Punctuators[lPunctuator] = type;
		}
		#endregion
		#region LoadPunctuators
		private void LoadPunctuators()
		{
			_Punctuators = new Hashtable();
			IEnumerator lTokens = VBPunctuators.TokenTypes.GetEnumerator();
			while (lTokens.MoveNext())
				LoadPunctuator((int)lTokens.Current);
		}
		#endregion
		#region LoadDirectives
		private void LoadDirectives()
		{
			_Directives = CollectionsUtil.CreateCaseInsensitiveHashtable();
			AddDirective("#const", TokenType.ConstDirective);
			AddDirective("#if", TokenType.IfDirective);
			AddDirective("#else", TokenType.ElseDirective);
			AddDirective("#elseif", TokenType.ElseIfDirective);
			AddDirective("#region", TokenType.Region);
			AddDirective("#externalsource", TokenType.ExternalSourceDirective);
			AddDirective("#endif", TokenType.EndifDirective);
			AddDirective("#end if", TokenType.EndifDirective);
			AddDirective("#end region", TokenType.EndRegion);
			AddDirective("#end externalsource", TokenType.EndExternalSourceDirective);
		}
		#endregion
		#region LoadTypeConversionTables
	private void LoadTypeConversionTables()
	{
	  _PrimitiveToFullTypes = new Hashtable();
	  _PrimitiveToFullTypes["sbyte"] = "System.SByte";
	  _PrimitiveToFullTypes["boolean"] = "System.Boolean";
	  _PrimitiveToFullTypes["date"] = "System.DateTime";
	  _PrimitiveToFullTypes["byte"] = "System.Byte";
	  _PrimitiveToFullTypes["char"] = "System.Char";
	  _PrimitiveToFullTypes["decimal"] = "System.Decimal";
	  _PrimitiveToFullTypes["double"] = "System.Double";
	  _PrimitiveToFullTypes["single"] = "System.Single";
	  _PrimitiveToFullTypes["integer"] = "System.Int32";
	  _PrimitiveToFullTypes["long"] = "System.Int64";
	  _PrimitiveToFullTypes["object"] = "System.Object";
	  _PrimitiveToFullTypes["short"] = "System.Int16";
	  _PrimitiveToFullTypes["string"] = "System.String";
	  _PrimitiveToFullTypes["void"] = "System.Void";
	  _PrimitiveToFullTypes["ushort"] = "System.UInt16";
	  _PrimitiveToFullTypes["uinteger"] = "System.UInt32";
	  _PrimitiveToFullTypes["ulong"] = "System.UInt64";
	  _FullToPrimitiveTypes = new Hashtable();
	  _FullToPrimitiveTypes["system.uint16"] = "UShort";
	  _FullToPrimitiveTypes["system.uint32"] = "UInteger";
	  _FullToPrimitiveTypes["system.uint64"] = "ULong";
	  _FullToPrimitiveTypes["system.sbyte"] = "SByte";
	  _FullToPrimitiveTypes["system.boolean"] = "Boolean";
	  _FullToPrimitiveTypes["system.byte"] = "Byte";
	  _FullToPrimitiveTypes["system.datetime"] = "Date";
	  _FullToPrimitiveTypes["system.char"] = "Char";
	  _FullToPrimitiveTypes["system.decimal"] = "Decimal";
	  _FullToPrimitiveTypes["system.double"] = "Double";
	  _FullToPrimitiveTypes["system.single"] = "Single";
	  _FullToPrimitiveTypes["system.int32"] = "Integer";
	  _FullToPrimitiveTypes["system.int64"] = "Long";
	  _FullToPrimitiveTypes["system.object"] = "Object";
	  _FullToPrimitiveTypes["system.int16"] = "Short";
	  _FullToPrimitiveTypes["system.string"] = "String";
	  _FullToPrimitiveTypes["system.void"] = "Void";
	}
		#endregion
		#region Keywords
		public Hashtable Keywords
		{
			get
			{
				return _Keywords;
			}
		}
		#endregion
		#region Punctuators
		public Hashtable Punctuators
		{
			get
			{
				return _Punctuators;
			}
		}
		#endregion
		#region Directives
		public Hashtable Directives
		{
			get
			{
				return _Directives;
			}
		}
		#endregion
		public static bool IsEscapedIdentifier(string name)
		{
			if (name == null || name.Length == 0)
				return false;
			string lName = name.Trim();
			return lName.IndexOf("[") >= 0 && lName.IndexOf("]") >= 0;
		}
		public static string RemoveIdentifierEscapes(string name)
		{
			if (name == null || name.Length == 0)
				return name;
			return name.Substring(1, name.Length - 2);
		}
		public int  GetKeywordToken(string word)
		{
			object token = _Keywords[word];
			return token == null ? TokenType.Identifier : (int)token;
		}
		public string GetTokenString(int tokenType) 
		{
			return tokens[tokenType];
		}
		public int  GetDirectiveToken(string word)
		{
			object token = _Directives[word];
			return token == null ? TokenType.UnknownToken : (int)token;
		}
		public string GetFullTypeName(string simpleName)
		{
			if (simpleName == null || simpleName.Length == 0)
				return String.Empty;
			string lName = simpleName.ToLower();
			if (IsEscapedIdentifier(lName))
				lName = RemoveIdentifierEscapes(lName);
			if (_PrimitiveToFullTypes.ContainsKey(lName))
				return (string)_PrimitiveToFullTypes[lName];
			return simpleName;
		}
		public string GetSimpleTypeName(string fullName)
		{
			if (fullName == null || fullName.Length == 0)
				return String.Empty;
			string lName = fullName.ToLower();
			if (_FullToPrimitiveTypes.ContainsKey(lName))
				return (string)_FullToPrimitiveTypes[lName];
			return fullName;
		}
		public bool IsBooleanLiteral(int tokenType)
		{
			return tokenType == TokenType.True || tokenType == TokenType.False;
		}
		public bool IsBooleanLiteral(string word)
		{
			int lTokenType = ToTokenType(word);
			return IsBooleanLiteral(lTokenType);
		}		
		public bool IsNullLiteral(int tokenType)
		{
			return tokenType == TokenType.Nothing;
		}
		public bool IsNullLiteral(string word)
		{
			int lTokenType = ToTokenType(word);
			return IsNullLiteral(lTokenType);
		}
		#region IsCastTargetToken
		public bool IsCastTargetToken(Token token)
		{
			switch (token.Type)
			{
				case TokenType.CBool:
				case TokenType.CByte:
				case TokenType.CChar:
				case TokenType.CDate:
				case TokenType.CDec:
				case TokenType.CDbl:
				case TokenType.Cint:
				case TokenType.Clng:
				case TokenType.Cobj:
				case TokenType.Cshort:
				case TokenType.Csng:
				case TokenType.Cstr:
					return true;
			}
			return false;
		}
		#endregion
		#region IsTypeCharacter(char aCh)
		public bool IsTypeCharacter(char aCh)
		{
			switch (aCh)
			{
				case IntegerTypeCharacter:
				case LongTypeCharacter:
				case DecimalTypeCharacter:
				case SingleTypeCharacter:
				case DoubleTypeCharacter:
				case StringTypeCharacter:
					return true;
				default:
					return false;
			}
		}
		#endregion
		#region IsIntegralTypeCharacter(char aCh)
		public bool IsIntegralTypeCharacter(char aCh)
		{
			switch (Char.ToUpper(aCh))
			{
				case ShortCharacter:
				case IntegerCharacter:
				case LongCharacter:
				case IntegerTypeCharacter:
				case LongTypeCharacter:
					return true;
				default:
					return false;
			}
		}
		#endregion
		#region IsFloatingPointTypeCharacter(char aCh)
		public bool IsFloatingPointTypeCharacter(char aCh)
		{
			switch (Char.ToUpper(aCh))
			{
				case SingleCharacter:
				case DoubleCharacter:
				case DecimalCharacter:
				case SingleTypeCharacter:
				case DoubleTypeCharacter:
				case DecimalTypeCharacter:
					return true;
				default:
					return false;
			}
		}
		#endregion
		#region IsLineContinuation(char ch)
		public bool IsLineContinuation(char ch)
		{
			switch (ch)
			{
				case '_':
					return true;
				default:
					return false;
			}
		}
		#endregion
		public override bool IsKeyword(string word)
		{
			return _Keywords.Contains(word);
		}
		#region IsIdentifier( string word )
		public override bool IsIdentifier( string word )
		{
			return ToTokenType(word) == TokenType.Identifier;
		}
		#endregion
		#region IsIdentifier(Token token)
		public bool IsIdentifier(Token token)
		{
			if (token.Match(TokenType.LineTerminator))
				return false;
			if (token.Match(TokenType.UnknownToken))
				return false;
			return token.Type == TokenType.Identifier;
		}
		#endregion
		public override bool IsStandardType(string word)
		{
			if (IsKeyword(word))
				return IsStandardType(ToTokenType(word));
			return false;
		}
		#region IsStandardType(int tokenType)
		public bool IsStandardType(int tokenType)
		{
			switch (tokenType)
			{
				case TokenType.Boolean:
				case TokenType.Date:
				case TokenType.Char:
				case TokenType.String:
				case TokenType.Decimal:
				case TokenType.Byte:
				case TokenType.Short:
				case TokenType.Integer:
				case TokenType.Long:
				case TokenType.Single:
				case TokenType.Double:
				case TokenType.Object:
				case TokenType.Void:
				case TokenType.Sbyte:
				case TokenType.UShortToken:
				case TokenType.UInteger:
				case TokenType.ULongToken:
					return true;
				default:
					return false;
			}
		}
		#endregion
		#region IsOctalDigit(char ch)
		public bool IsOctalDigit(char ch)
		{
			if (Char.IsDigit(ch))
			{
				int lDigit = int.Parse(ch.ToString());
				if (lDigit >= 0 && lDigit <= 7)
					return true;
			}
			return false;
		}
		#endregion
		#region IsHexDigit(char ch)
		public bool IsHexDigit(char ch)
		{
			if (Char.IsDigit(ch))
				return true;
			else if (Char.IsLetter(ch))
			{
				char lCh = Char.ToUpper(ch);
				switch (lCh)
				{
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
						return true;
					default:
						return false;
				}
			}
			return false;
		}
		#endregion
		#region IsDirective(int type)
		public override bool IsDirective(int type)
		{
			return _Directives.ContainsValue(type);
		}
		#endregion
		#region IsPunctuator(string s)
		public bool IsPunctuator(string s)
		{
			return _Punctuators.Contains(s);
		}
		#endregion
		public override bool IsOperator(int type)
		{
			return false;
		}
		public string GetPunctuator(char[] chars, int length)
		{
			string lPunctuator = new String(chars, 0, length);
			return _Punctuators.Contains(lPunctuator) ? lPunctuator : null;
		}
		#region IsPunctuator(char ch)
		public bool IsPunctuator(char ch)
		{
			return IsPunctuator(ch.ToString());
		}
		#endregion
		#region IsPunctuator(params char[] chars)
		public bool IsPunctuator(params char[] chars)
		{
			_Worker.Length = 0;
			for (int i = 0; i < chars.Length; i++)
				_Worker.Append(chars[i]);
			return IsPunctuator(_Worker.ToString());
		}
		#endregion
		#region IsComment(int type)
		public override bool IsComment(int type)
		{
			return TokenType.Comment == type;
		}
		#endregion
		#region IsXmlDocComment(int type)
		public override bool IsXmlDocComment(int type)
		{
			return TokenType.XmlComment == type;
		}
		#endregion
		#region IsNumber(int type)
		public override bool IsNumber(int type)
		{
			return TokenType.IntegerLiteral == type ||
				TokenType.FloatingPointLiteral == type;
		}
		#endregion
		#region IsStringLiteral(int type)
		public override bool IsStringLiteral(int type)
		{
			return TokenType.StringLiteral == type;
		}
		#endregion
		#region ToTokenType(string token)
		public override int ToTokenType(string token)
		{
			object res = _Tokens[token];
			if(res == null) 
			{
				return TokenType.Identifier;
			}
			else
				return (int)res;
		}
		#endregion
	}
}
