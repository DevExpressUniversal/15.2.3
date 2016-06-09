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
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public abstract class CSharpTokensBase : LanguageTokensBase
	{
		#region private fields...		
		Hashtable _Keywords;
		Hashtable _Punctuators;
		Hashtable _Directives;
		string[] tokens;
		Hashtable _PrimitiveToFullTypes;
		Hashtable _FullToPrimitiveTypes;
		#endregion
		#region CSharpTokensBase()
		public CSharpTokensBase()
		{
			tokens = new string[200];
			PopulateTokenTable();
		}
		#endregion
	#region AddToken
		void AddToken(int token, string name) 
		{
			tokens[token] = name;
	}
	#endregion
	#region PopulateTokenTable()
	private void PopulateTokenTable()
		{
			LoadKeyWords();
			LoadPunctuators();
			LoadDirectives();
			LoadTypeConversionTables();
		}
		#endregion
		#region LoadKeyWords
		private void LoadKeyWords()
		{
			_Keywords = new Hashtable();
			StringCollection lKeywords = CreateKeywords();
			foreach(string lKeyword in lKeywords) 
			{
				int token = GetTokenType(lKeyword);
				AddToken(token, lKeyword);
				_Keywords[lKeyword] = token;
			}
		}
		#endregion
	#region GetTokenType
	int GetTokenType(string code)
		{
			TokenCollection tokens = CSharpTokensHelper.GetTokens(code);
			if (tokens == null || tokens.Count == 0)
				return -1;
			Token token = tokens[0];
			return token.Type;
	}
	#endregion
	#region LoadPunctuator
	private void LoadPunctuator(int type)
		{
			string lPunctuator = CSharpPunctuators.GetPunctuator(type);
			AddToken(type, lPunctuator);
			if (lPunctuator!=null)
				_Punctuators[lPunctuator] = type;
		}
		#endregion
		#region LoadPunctuators
		private void LoadPunctuators()
		{
	  _Punctuators = new Hashtable();
			IEnumerator lTokens = CSharpPunctuators.TokenTypes.GetEnumerator();
			while (lTokens.MoveNext())
				LoadPunctuator((int)lTokens.Current);
		}
		#endregion
	#region AddDirective
	void AddDirective(string name, int token) 
		{
			_Directives[name] = token;
			AddToken(token, name);
	}
	#endregion
	#region LoadDirectives
	private void LoadDirectives()
		{
			_Directives = new Hashtable();
			AddDirective("#region", Tokens.REGION);
			AddDirective("#endregion", Tokens.ENDREG);
			AddDirective("#if", Tokens.IFDIR);
			AddDirective("#else", Tokens.ELSEDIR);
			AddDirective("#elif", Tokens.ELIF);
			AddDirective("#endif", Tokens.ENDIF);
			AddDirective("#define", Tokens.DEFINE);
			AddDirective("#undef", Tokens.UNDEF);
			AddDirective("#warning", Tokens.WARNING);
			AddDirective("#error", Tokens.ERROR);
			AddDirective("#line", Tokens.LINE);
			AddDirective("#pragma", Tokens.PRAGMADIR);
		}
		#endregion
		#region LoadTypeConversionTables
		private void LoadTypeConversionTables()
		{
			_PrimitiveToFullTypes = new Hashtable();
			_PrimitiveToFullTypes["bool"] = "System.Boolean";
			_PrimitiveToFullTypes["byte"] = "System.Byte";
			_PrimitiveToFullTypes["sbyte"] = "System.SByte";
			_PrimitiveToFullTypes["char"] = "System.Char";
			_PrimitiveToFullTypes["decimal"] = "System.Decimal";
			_PrimitiveToFullTypes["double"] = "System.Double";
			_PrimitiveToFullTypes["float"] = "System.Single";
			_PrimitiveToFullTypes["int"] = "System.Int32";
			_PrimitiveToFullTypes["uint"] = "System.UInt32";
			_PrimitiveToFullTypes["long"] = "System.Int64";
			_PrimitiveToFullTypes["ulong"] = "System.UInt64";
			_PrimitiveToFullTypes["object"] = "System.Object";
			_PrimitiveToFullTypes["short"] = "System.Int16";
			_PrimitiveToFullTypes["ushort"] = "System.UInt16";
			_PrimitiveToFullTypes["string"] = "System.String";
			_PrimitiveToFullTypes["void"] = "System.Void";
			_FullToPrimitiveTypes = new Hashtable();
			_FullToPrimitiveTypes["System.Boolean"] = "bool";
			_FullToPrimitiveTypes["System.Byte"] = "byte";
			_FullToPrimitiveTypes["System.SByte"] = "sbyte";
			_FullToPrimitiveTypes["System.Char"] = "char";
			_FullToPrimitiveTypes["System.Decimal"] = "decimal";
			_FullToPrimitiveTypes["System.Double"] = "double";
			_FullToPrimitiveTypes["System.Single"] = "float";
			_FullToPrimitiveTypes["System.Int32"] = "int";
			_FullToPrimitiveTypes["System.UInt32"] = "uint";
			_FullToPrimitiveTypes["System.Int64"] = "long";
			_FullToPrimitiveTypes["System.UInt64"] = "ulong";
			_FullToPrimitiveTypes["System.Object"] = "object";
			_FullToPrimitiveTypes["System.Int16"] = "short";
			_FullToPrimitiveTypes["System.UInt16"] = "ushort";
			_FullToPrimitiveTypes["System.String"] = "string";
			_FullToPrimitiveTypes["System.Void"] = "void";
		}
		#endregion
		#region IsUnaryOperator
		public bool IsUnaryOperator(Token token)
		{
			switch (token.Type)
			{
				case Tokens.PLUS:
				case Tokens.MINUS:
				case Tokens.NOT:
				case Tokens.DEC:
				case Tokens.INC:
				case Tokens.TILDE:
				case Tokens.TIMES:
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
		#region IsRealTypeSuffix(char ch)
		public bool IsRealTypeSuffix(char ch)
		{
			switch (Char.ToLower(ch))
			{
				case 'f':
				case 'd':
				case 'm':
					return true;
				default:
					return false;
			}
		}
		#endregion
	#region IsIdentifier(int tokenType)
	public bool IsIdentifier(int tokenType) {
				return tokenType == Tokens.IDENT;
	  }
	#endregion
	#region IsIdentifier(string word)
	  public override bool IsIdentifier(string word)
		{
			return IsIdentifier(shortToToken(word));
		}
	#endregion
	#region IsIdentifier(Token token)
		public bool IsIdentifier(Token token)
		{
			if (token == null)
				return false;
			return IsIdentifier(token.Type);
		}
	#endregion
		#region IsKeyword(string word)
		public override bool IsKeyword(string word)
		{
			return _Keywords.Contains(word);
		}
		#endregion
		#region IsPunctuator(string s)
		public bool IsPunctuator(string s)
		{
			return _Punctuators.Contains(s);
		}
		#endregion		
		#region IsPunctuator(char ch)
		public bool IsPunctuator(char ch)
		{
			return IsPunctuator(ch.ToString());
		}
		#endregion
		#region IsPunctuator(Token token)
		public bool IsPunctuator(Token token)
		{
			if (token == null)
				return false;
			return IsPunctuator(token.EscapedValue);
		}
		#endregion
		#region IsDirective(int type)
		public override bool IsDirective(int type)
		{
			return _Directives.ContainsValue(type);
		}
		#endregion
		#region IsComment(int type)
		public override bool IsComment(int type)
		{
			return type == Tokens.SINGLELINECOMMENT || type == Tokens.MULTILINECOMMENT;
		}
		#endregion
		#region IsXmlDocComment(int type)
		public override bool IsXmlDocComment(int type)
		{
			return type == Tokens.SINGLELINEXML || type == Tokens.MULTILINEXML;
		}
		#endregion
		#region IsNumber(int type)
		public override bool IsNumber(int type)
		{
			return type == Tokens.INTCON || type == Tokens.REALCON;
		}
		#endregion
		#region IsStringLiteral(int type)
		public override bool IsStringLiteral(int type)
		{
			return Tokens.STRINGCON == type;
		}
		#endregion
	#region ToToken( string word )
		int shortToToken(string word)
		{
			if (_Punctuators.Contains(word))
				return (int) _Punctuators[word];
			else if (_Keywords.Contains(word))
				return (int) _Keywords[word];
			else if (_Directives.Contains(word))
				return (int) _Directives[word];
			else
				return Tokens.IDENT;
		}
		public override int ToTokenType(string token)
		{
			return shortToToken(token);
		}
		public int  GetKeywordToken(string word)
		{
			object token = _Keywords[word];
			return token == null ? Tokens.IDENT : (int)token;
		}
		public int  GetDirectiveToken(string word)
		{
			object token = _Directives[word];
			return token == null ? Tokens.EOF : (int)token;
		}
		#endregion
	#region ToTokenType( Token token )
	public int  ToTokenType( Token token )
	{
	  return ToTokenType( token.EscapedValue );
	}
	#endregion
		#region IsStandardType(string word)
		public override bool IsStandardType(string word)
		{
			if (word != "string" && word != "object")
				return IsStandardType(shortToToken(word));
			else
				return true;
		}
		#endregion
	#region IsStandardType ( int /*TokenTypeEnum*/ word )
	public bool IsStandardType( int  word )
		{
			return word == Tokens.BOOL || word == Tokens.BYTE || word == Tokens.CHAR || 
				   word == Tokens.DECIMAL || word == Tokens.DOUBLE || word == Tokens.FLOAT || 
				   word == Tokens.INT || word == Tokens.LONG || 
				   word == Tokens.SBYTE || word == Tokens.SHORT ||
				   word == Tokens.UINT || word == Tokens.ULONG || word == Tokens.USHORT || word == Tokens.VOID;
		}
	#endregion
	#region IsBooleanLiteral(int tokenType)
	public bool IsBooleanLiteral(int tokenType)
		{
			return tokenType == Tokens.TRUE || tokenType == Tokens.FALSE;
		}
	#endregion
	#region IsBooleanLiteral(string word)
	public bool IsBooleanLiteral(string word)
		{
			int lTokenType = shortToToken(word);
			return IsBooleanLiteral(lTokenType);
	}
	#endregion
	#region IsNullLiteral(int tokenType)
	public bool IsNullLiteral(int tokenType)
		{
			return tokenType == Tokens.NULL;
	}
	#endregion
	#region IsNullLiteral(string word)
	public bool IsNullLiteral(string word)
		{
			int lTokenType = shortToToken(word);
			return IsNullLiteral(lTokenType);
	}
	#endregion
	#region GetFullTypeName
	public string GetFullTypeName(string simpleName)
		{
			if (simpleName == null || simpleName.Length == 0)
				return String.Empty;
			if (_PrimitiveToFullTypes.ContainsKey(simpleName))
				return String.Intern((string)_PrimitiveToFullTypes[simpleName]);
			return simpleName;
	}
	#endregion
	#region GetSimpleTypeName
	public string GetSimpleTypeName(string fullName)
		{
			if (fullName == null || fullName.Length == 0)
				return String.Empty;
			if (_FullToPrimitiveTypes.ContainsKey(fullName))
				return String.Intern((string)_FullToPrimitiveTypes[fullName]);
			return fullName;
	}
	#endregion
	#region GetTokenString
	public string GetTokenString(int tokenType) 
		{
			if (tokenType < 0 || tokenType > tokens.Length)
				return String.Empty;
			return tokens[tokenType];
	}
	#endregion
  }
}
