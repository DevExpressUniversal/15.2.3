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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
  using Preprocessor;
  using Html;
  using Diagnostics;
  using VBXmlDocComment = VB.XMLDocComment;
  using System.Text;
	public partial class VB90Parser : IRazorCodeParser
	{
		bool _IsNewContext = false;
	static object _SyncObject = new object();
		bool _PreviousTokenWasComment = false;
		CommentCollection _Comments = new CommentCollection();
	VBPreprocessor _Preprocessor;
		LocalVarArrayCollection _LocalVarArrayCollection;
		ParserVersion _VsVersion = ParserVersion.VS2008;
		DeclarationsCache _DeclarationsCache = new DeclarationsCache();
	int _ExpressionNestingLevel;
		bool _NotXmlNode = false;
		string[] _AggregateMethodNames = {
		"All", "Any", "AsEnumerable", "AsQueryable", "Average", "Cast",
		"Clone", "Concat", "Count", "DefaultIfEmpty", "Distinct", "ElementAt",
		"ElementAtOrDefault", "First", "FirstOrDefault", "GetEnumerator", 
		"GetHashCode", "GetType", "GroupBy", "Last", "LastOrDefault", "LongCount", 
		"Max", "Min", "OfType", "OrderBy", "OrderByDescending", "Select", "SelectMany", 
		"Single", "SingleOrDefault", "Skip", "SkipWhile", "Sum", "Take", "TakeWhile", 
		"ToArray", "ToDictionary", "ToList", "ToLookup", "ToString", "Where"};
		ParserBase _HtmlParser = null;
		public VB90Parser()
		{
			parserErrors = new VB90ParserErrors();
			set = CreateSetArray();
			maxTokens = Tokens.MaxTokens;
	  SetUpLineContinuationCheckState();
	  _ExpressionNestingLevel = 0;
		}
	bool _SavedLineContinuationCheckState;
	bool _LineContinuationCheckState;
	protected virtual void SetUpLineContinuationCheckState()
	{
	   DisableImplicitLineContinuationCheck();
	}
		protected void DisableImplicitLineContinuationCheck()
	{
	  _SavedLineContinuationCheckState = _LineContinuationCheckState;
	  _LineContinuationCheckState = false;
	}
	protected void EnableImplicitLineContinuationCheck()
	{
	  _SavedLineContinuationCheckState = _LineContinuationCheckState;
	  _LineContinuationCheckState = true;
	}
	protected void RestoreImplicitLineContinuationCheck()
	{
	   bool tmpState = _LineContinuationCheckState;
	  _LineContinuationCheckState = _SavedLineContinuationCheckState;
	  _SavedLineContinuationCheckState = tmpState;
	}
	protected bool CheckImplicitLineContinuations
	{
	  get { return _LineContinuationCheckState; }
	}
	protected bool IsAsynchronousContext;
		ParserBase GetHtmlParser()
		{
			if (_HtmlParser == null)
			{
		_HtmlParser = new HtmlParser();
				if (_HtmlParser == null || !(_HtmlParser is EmbeddedCodeParserBase))
					return null;
				(_HtmlParser as EmbeddedCodeParserBase).InitialDefaultDotNetLanguage = DotNetLanguageType.VisualBasic;
			}
			return _HtmlParser;
		}
		protected override TokenCategory GetTokenCategory(CategorizedToken token)
		{
			int type = token.Type;
			if (IsOperator(type))
				return TokenCategory.Operator;
			if (type != Tokens.Compare && IsKeyword(type))
				return TokenCategory.Keyword;
			if (IsPreprocessorDirective(type))
				return TokenCategory.PreprocessorKeyword;
			if (type == Tokens.StringLiteral || type == Tokens.CharacterLiteral)
				return TokenCategory.String;
			if (type == Tokens.IntegerLiteral || type == Tokens.FloatingPointLiteral)
				return TokenCategory.Number;
			if (type == Tokens.SingleLineComment)
				return TokenCategory.Comment;
			if (type == Tokens.SingleLineXmlComment)
				return TokenCategory.XmlComment;
			if (type == Tokens.Identifier)
			{
				string value = token.Value.ToLower();
				if (value != null &&
						(value == "string" || value == "object" || value == "getxmlnamespace"))
					return TokenCategory.Keyword;
				return TokenCategory.Identifier;
			}
			if (type == Tokens.EOF || type == Tokens.LineTerminator)
				return TokenCategory.Unknown;
			return TokenCategory.Text;
		}
		protected bool XmlCommentStringIsNextNode()
		{
			if (la != null && la.Type == Tokens.XmlCommentString)
				return true;
			ResetPeek();
			Token peekToken = Peek();
			while (peekToken != null && peekToken.Type != Tokens.EOF
				&& peekToken.Type == Tokens.LineTerminator)
				peekToken = Peek();
			if (peekToken != null && peekToken.Type == Tokens.XmlCommentString)
				return true;
			return false;
		}
		Expression ParseXmlString(string str, SourceRange startRange)
		{
			ParserBase htmlParser = GetHtmlParser();
			if (htmlParser == null)
				return null;
			int startLine = startRange.Start.Line;
			int startOffset = startRange.Start.Offset;
			LanguageElement sFile = htmlParser.ParseString(str, startLine, startOffset);
			if (sFile != null && sFile.Nodes != null && sFile.NodeCount != 0)
			{
				XmlExpression xmlExl = new XmlExpression();
				LanguageElement element = null;
				LanguageElement lastElement = null;
				for (int i = 0; i < sFile.NodeCount; i++)
				{
					element = sFile.Nodes[i] as LanguageElement;
					if (element != null && element.ElementType != LanguageElementType.Method)
					{
						xmlExl.AddNode(element);
						lastElement = element;
					}
				}
				if (lastElement != null)
				{
					xmlExl.SetRange(GetRange(startRange, lastElement.Range));
				}
				else
				{
					xmlExl.SetRange(GetRange(startRange));
				}
				return xmlExl;
			}
			return null;
		}
		protected string GetImportsXmlValue()
		{
			if (la == null || la.Type != Tokens.LessThan)
				return null;
			return GetOneTagXmlValue();
		}
		protected NamespaceReference CreateXmlImports(string str, Token afterImportsToken)
		{
			if (str == null || str == String.Empty)
				return null;
			string name = GetXmlImportsName(str);
			string namespaceName = GetXmlImportsNamespace(str);
			NamespaceReference result = new XmlNamespaceReference(name, namespaceName);
			ElementReferenceExpression elem = GetImportsElementRef(str, afterImportsToken, name);
			if (elem != null)
			{
				result.AddDetailNode(elem);
			}
			return result;
		}
		protected ElementReferenceExpression GetImportsElementRef(string str, Token afterImportsToken, string name)
		{
			if (str == null || str == String.Empty)
				return null;
			ElementReferenceExpression elemRef = new ElementReferenceExpression(name);
			if (afterImportsToken == null)
				return elemRef;
			int start = afterImportsToken.Range.Start.Offset;
			int startPos = GetStartIndex(str) + start;
			if (startPos <= 0)
				return elemRef;
			int length = 1;
			if (name != null)
			{
				length = name.Length;
			}
			int line = afterImportsToken.Line;
			int endPos = startPos + length;
			SourceRange range = new SourceRange(line, startPos, line, endPos);
			elemRef.SetRange(range);
			return elemRef;
		}
		int GetStartIndex(string str)
		{
			if (str == null || str == String.Empty)
				return 0;
			if (str == null)
				return 0;
			int length = str.Length;
			if (length <= 0)
				return 0;
			str = str.Trim();
			int index = str.IndexOf(":");
			if (index <= 0)
				return 0;
			index += 1;
			if (index >= length)
				return 0;
			char ch = str[index];
			while (ch == ' ')
			{
				index++;
				if (index >= length)
					return index;
				ch = str[index];
			}
			return index;
		}
		protected static string GetXmlImportsName(string str)
		{
			if (str == null)
				return String.Empty;
			int length = str.Length;
			if (length <= 0)
				return String.Empty;
			str = str.Trim();
			int index = str.IndexOf("xmlns");
			if (index <= 0)
				return String.Empty;
			index += 5;
			if (index >= length)
				return String.Empty;
			char ch = str[index];
			while (ch == ' ' || ch == ':')
			{
				index++;
				if (index >= length)
					return String.Empty;
				ch = str[index];
			}
			if (ch == '=')
				return String.Empty;
			string name = String.Empty;
			while (ch != '=')
			{
				name += ch;
				index++;
				if (index >= length)
					return name.Trim();
				ch = str[index];
			}
			return name.Trim();
		}
		protected static string GetXmlImportsNamespace(string str)
		{
			if (str == null)
				return String.Empty;
			int length = str.Length;
			if (length <= 0)
				return String.Empty;
			str = str.Trim();
			int index = str.IndexOf("=");
			if (index <= 0)
				return String.Empty;
			index += 1;
			if (index >= length)
				return String.Empty;
			char ch = str[index];
			string name = str.Substring(index);
			length = name.Length - 1;
			if (length <= 0)
				return String.Empty;
			name = name.Remove(length);
			return name.Trim();
		}
		protected string GetOneTagXmlValue()
		{
			return GetOneTagXmlValue(false);
		}
		protected string GetOneTagXmlValue(bool deleteAngelBrackets)
		{
			if (LessThanOrcas() || scanner == null)
				return null;
			VB90Scanner vbScanner = scanner as VB90Scanner;
			if (scanner is VB90Scanner)
			{
				string xmlStr = vbScanner.GetXmlOneTag(la.StartPosition);
				if (xmlStr != null && xmlStr != String.Empty)
				{
					Get();
					if (deleteAngelBrackets)
					{
						xmlStr = xmlStr.Substring(1);
					}
					else if (la != null && la.Type == Tokens.GreaterThan)
					{
						Get();
						xmlStr = xmlStr + ">";
					}
					return xmlStr;
				}
			}
			return String.Empty;
		}
		protected Expression CreateXmlExpression()
		{
			if (la == null || LessThanOrcas() ||
				(la.Type != Tokens.LessThan && la.Type != Tokens.ShiftLeft) || _NotXmlNode || scanner == null)
				return null;
			VB90Scanner vbScanner = scanner as VB90Scanner;
			if (scanner is VB90Scanner)
			{
				string xmlStr = vbScanner.GetXmlString(la.StartPosition, la.Value);
				if (xmlStr != null && xmlStr != String.Empty)
				{
					SourceRange startRange = la.Range;
					Expression exp = ParseXmlString(xmlStr, startRange);
					Get();
					if (exp != null && exp.Range != SourceRange.Empty)
					{
						tToken.Range = exp.Range;
						tToken.Value = ">";
					}
					return exp;
				}
			}
			return null;
		}
		protected void SkipToEol()
		{
			while (la != null && la.Type != Tokens.EOF && la.Type != Tokens.LineTerminator && la.Type != Tokens.Colon)
			{
				Get();
			}
		}
		bool HasAutoImplementedPropertyEnd()
		{
			StatementTerminatorRule();
	  Token nextToken = GetPeek();
			return la.Match(Tokens.EndToken) && nextToken.Match(Tokens.Property);
		}
		protected void StatementTerminatorRule()
		{
			while (la.Type == Tokens.LineTerminator || la.Type == Tokens.Colon)
			{
				Get();
			}
		}
		bool AtLeastVS2010()
		{
			return ((int)VsVersion >= (int)ParserVersion.VS2010);
		}
		bool LessThanVS2010()
		{
			return ((int)VsVersion < (int)ParserVersion.VS2010);
		}
		bool LessThanOrcas()
		{
			return ((int)VsVersion < (int)ParserVersion.VS2008);
		}
		bool AtLeastOrcas()
		{
			return ((int)VsVersion >= (int)ParserVersion.VS2008);
		}
		bool AtLeastVS2005()
		{
			return ((int)VsVersion >= (int)ParserVersion.VS2005);
		}
		protected bool IsUsing()
		{
			return (VsVersion != ParserVersion.VS2003 && VsVersion != ParserVersion.VS2002 && la.Type == Tokens.Using);
		}
		protected bool IsOperatorMemberRule()
		{
			return AtLeastVS2005() && la.Type == Tokens.Operator;
		}
		protected bool IsCustomMemberRule()
		{
			return AtLeastVS2005() && la.Type == Tokens.Custom;
		}
		protected void SetElementMemberDeclarationProp(LanguageElement element, LanguageElementCollection accessors, TokenQueueBase modifiers, LanguageElementCollection attributes, OnDemandParsingParameters onDemandParsingParameters, SourceRange blockStart)
		{
			if (element == null)
				return;
			SetBlockStart(element, blockStart);
			SetAttributesAndModifiers(element, attributes, modifiers);
			SetPropertyAccessors(element as VBProperty, accessors);
			AddBlockRangeOnDemand(element, onDemandParsingParameters);
		}
		protected bool IsContinue()
		{
			return (VsVersion != ParserVersion.VS2003 && VsVersion != ParserVersion.VS2002 && la.Type == Tokens.Continue);
		}
		protected bool IsTryCast()
		{
			return (VsVersion != ParserVersion.VS2003
				&& VsVersion != ParserVersion.VS2002
				&& la.Type == Tokens.TryCast);
		}
		protected Token GetPeek()
		{
			ResetPeek();
			Token peekToken = Peek();
			while (peekToken.Type == Tokens.LineTerminator || peekToken.Type == Tokens.LineContinuation)
				peekToken = Peek();
			return peekToken;
		}
		protected bool IsSqlExpression()
		{
			if (!(AtLeastOrcas()
				&& (la.Type == Tokens.From || la.Type == Tokens.Aggregate)))
				return false;
			Token peekToken = GetPeek();
			if (IsIdentifierOrKeyword(peekToken.Type))
				return true;
			return false;
		}
	protected bool IsSelectOrFromExpression()
	{
	  if (la.Type == Tokens.Select && GetPeek().Type != Tokens.Case)
		return true;
	  Token peekToken = GetPeek();
	  Token nextToken = Peek();
	  while (nextToken.Type == Tokens.LineTerminator || nextToken.Type == Tokens.LineContinuation)
		nextToken = Peek();
	  return ((peekToken.Type == Tokens.Select && nextToken.Type != Tokens.Case) || peekToken.Type == Tokens.From);
	}
		public void SetElementRange(LanguageElement element, SourceRange startRange, SourceRange endRange)
		{
			element.SetRange(GetRange(startRange, endRange));
		}
		protected override void Get()
		{
			_PreviousTokenWasComment = false;
			base.Get();
			Token oldT = tToken;
			while (la.Type == Tokens.LineContinuation)
				base.Get();
			while (TokenIsComment(la) || IsPreprocessorDirective(la))
			{
				if (TokenIsComment(la))
				{
					AddCommentNode(la);
					_PreviousTokenWasComment = true;
					base.Get();
					if (TokenIsComment(la))
						PassStatementTerminator();
				}
				else if (IsPreprocessorDirective(la))
				{
					PreprocessToken();
					if (IsPreprocessorDirective(la))
						PassStatementTerminator();
				}
			}
			tToken = oldT;
		}
		void PassStatementTerminator()
		{
			while (la.Type == Tokens.LineTerminator  || la.Type == Tokens.Colon )
			{
				Get();
			}
		}
		void PreprocessToken()
		{
			if (_Preprocessor == null)
			{
				Get();
				return;
			}
			CompilerDirective compilerDirectiveRootNode = null;
			if (RootNode != null && RootNode is SourceFile)
			{
				compilerDirectiveRootNode = ((SourceFile)RootNode).CompilerDirectiveRootNode;
				if (compilerDirectiveRootNode != null && compilerDirectiveRootNode.Parent == null)
					compilerDirectiveRootNode.SetParent(RootNode);
			}
			la = _Preprocessor.Preprocess(la);
		}
		protected bool IsAliasImports()
		{
			if (la.Type == Tokens.Identifier)
			{
				Token peek = Peek();
				if (peek.Type == Tokens.EqualsSymbol)
					return true;
			}
			return false;
		}
		bool IsPreprocessorDirective(Token token)
		{
			if (token == null)
				return false;
			return IsPreprocessorDirective(token.Type);
		}
		public static bool IsPreprocessorDirective(int type)
		{
			return
				(type == Tokens.IfDirective ||
				type == Tokens.EndifDirective ||
				type == Tokens.ElseIfDirective ||
				type == Tokens.ElseDirective ||
				type == Tokens.ExternalSourceDirective ||
				type == Tokens.EndExternalSourceDirective ||
				type == Tokens.ConstDirective)
				||
				(type == Tokens.Region || type == Tokens.EndRegion);
		}
		protected void PassAttributes(LanguageElement parent, CodeElement param)
		{
			if (parent == null || param == null)
				return;
			NodeList attrs = param.AttributeSections;
			if (attrs != null && attrs.Count != 0)
			{
				for (int i = 0; i < attrs.Count; i++)
				{
					AttributeSection atrSect = attrs[i] as AttributeSection;
					if (atrSect == null)
						continue;
					parent.AddDetailNode(atrSect);
				}
			}
			parent.AddDetailNode(param);
		}
		protected bool IsIdentifierOrKeyword()
		{
			return IsIdentifierOrKeyword(la.Type);
		}
		public static bool IsIdentifierOrKeyword(int type)
		{
			return IsIdentifier(type) || IsKeyword(type);
		}
		protected bool IsIdentifier()
		{
			return IsIdentifier(la.Type);
		}
		public static bool IsIdentifier(int type)
		{
			return type == Tokens.Identifier;
		}
		protected bool IsKeyword()
		{
			return IsKeyword(la.Type);
		}
		public static bool IsIdentifierOrKeywordOrOperator(Token token)
		{
			return IsIdentifierOrKeyword(token.Type) || IsOperator(token);
		}
		protected bool IsIdentifierOrKeywordOrOperator()
		{
			return IsIdentifierOrKeyword() || IsOperator(la);
		}
		public static bool IsOperator(Token token)
		{
			if (token == null)
				return false;
			return IsOperator(token.Type);
		}
		public static bool IsOperator(int type)
		{
			switch (type)
			{
				case Tokens.NotEquals:
				case Tokens.Minus:
				case Tokens.Plus:
				case Tokens.MinusEqual:
				case Tokens.PlusEqual:
				case Tokens.BackSlash:
				case Tokens.BackSlashEquals:
				case Tokens.Asterisk:
				case Tokens.MulEqual:
				case Tokens.BitAnd:
				case Tokens.Mod:
				case Tokens.Slash:
				case Tokens.Xor:
				case Tokens.XorSymbol:
				case Tokens.XorEqual:
				case Tokens.Or:
				case Tokens.OrElse:
				case Tokens.And:
				case Tokens.AndEqual:
				case Tokens.TypeOf:
				case Tokens.IsNot:
				case Tokens.EqualsSymbol:
				case Tokens.Like:
				case Tokens.GreaterThan:
				case Tokens.GreaterOrEqual:
				case Tokens.LessThan:
				case Tokens.LessOrEqual:
				case Tokens.ShiftLeft:
				case Tokens.ShiftLeftEqual:
				case Tokens.ShiftRight:
				case Tokens.ShiftRightEqual:
					return true;
			}
			return false;
		}
		public static bool IsKeyword(int type)
		{
			int[] keywords = Tokens.Keywords;
			for (int i = 0; i < keywords.Length; i++)
				if (keywords[i] == type)
					return true;
			return false;
		}
		protected bool IsFileAttributeSection()
		{
			if (la.Type == Tokens.LessThan)
			{
				Token peek = Peek();
				return peek.Type == Tokens.Assembly ||
					peek.Type == Tokens.Module;
			}
			return false;
		}
		protected bool IsNotParenClose()
		{
			return la.Type != Tokens.ParenClose;
		}
		protected Expression GetAttributeVariableInitializer(Expression leftSide, Expression rigthSide, SourceRange range)
		{
			AttributeVariableInitializer returnValue = new AttributeVariableInitializer();
			if (leftSide != null)
			{
				returnValue.LeftSide = leftSide;
			}
			if (rigthSide != null)
			{
				returnValue.RightSide = rigthSide;
			}
			returnValue.SetRange(range);
			return returnValue;
		}
		protected bool IsNamedAssign()
		{
			if (!IsIdentifierOrKeyword(la.Type))
				return false;
			Token peek = Peek();
			return peek.Type == Tokens.ColonEquals;
		}
		protected Method SetDeclareAttributes(Method result, bool isExtern, string charsetModifier)
		{
			if (result == null)
				return null;
			result.IsExtern = isExtern;
			if (charsetModifier != null)
				result.CharsetModifier = charsetModifier;
			return result;
		}
		protected bool HasConst(TokenQueueBase modifiers)
		{
			if (modifiers == null)
				return false;
			return modifiers.ContainsToken(Tokens.Const);
		}
		protected void SetAttributesAndModifiers(LanguageElement lanElement, LanguageElementCollection attributes, TokenQueueBase modifiers)
		{
			SetAttributesAndModifiers(lanElement, attributes, modifiers, true);
		}
		protected void SetAttributesAndModifiers(LanguageElement lanElement, LanguageElementCollection attributes, TokenQueueBase modifiers, bool setStartRange)
		{
			if (lanElement == null)
				return;
			CodeElement element = lanElement as CodeElement;
			if (element == null || element == null)
				return;
			AddAttributes(element, attributes);
			if (modifiers != null && modifiers.Count != 0 && setStartRange)
			{
				Token firstToken = modifiers.CurrentToken;
				if (firstToken != null)
				{
					SourceRange firstRange = firstToken.Range;
					lanElement.SetStart(firstToken);
				}
			}
			AccessSpecifiedElement accessSpecifiedElement = element as AccessSpecifiedElement;
			if (accessSpecifiedElement != null)
				VB90SpecifiersHelper.ParseSpecifiers(modifiers, accessSpecifiedElement);
			else
				VB90SpecifiersHelper.ParseSpecifiers(modifiers, element as Accessor);
		}
	private bool IsAsyncModifier()
	{
	  return la.Type == Tokens.Identifier && la.Value.ToLower() == "async";
	}
	private bool HasAsyncModifier(TokenQueueBase tokens)
	{
	  if (tokens == null)
		return false;
	  foreach (Token token in tokens)
		if (token.Value.ToLower() == "async")
		  return true;
	  return false;
	}
	private bool IsAsyncLambda()
	{
	  if (la.Type != Tokens.Identifier || la.Value.ToLower() != "async")
		return false;
	  ResetPeek();
	  int next = GetPeek().Type;
	  return next == Tokens.Sub || next == Tokens.Function;
	}
	private bool IsAwaitExpression()
	{
	  bool laTokenIsAwait = la.Type == Tokens.Identifier && la.Value.ToLower() == "await";
	  if (IsAsynchronousContext && laTokenIsAwait)
		return true;
	  if (laTokenIsAwait)
	  {
		ResetPeek();
		Token nextToken = Peek();
		if (nextToken.Type == Tokens.Identifier)
		  return true;
	  }
	  return false;
	}
		protected void SetWithEvents(Variable var, Token token)
		{
			if (var == null || token == null)
				return;
			var.IsWithEvents = true;
			var.SetWithEventsRange(token);
		}
		protected void SetBlockRange(LanguageElement element, SourceRange startRange, SourceRange endRange)
		{
			DelimiterCapableBlock blockElment = element as DelimiterCapableBlock;
			if (blockElment == null)
				return;
			SetHasBlockAndBlockType(blockElment);
			blockElment.SetBlockStart(startRange);
			blockElment.SetBlockEnd(endRange);
		}
		protected void SetBlockRangeForEmptyElement(LanguageElement element, SourceRange startRange, SourceRange endRange)
		{
			DelimiterCapableBlock blockElment = element as DelimiterCapableBlock;
			if (blockElment == null)
				return;
			SetHasBlockAndBlockType(blockElment);
			if (blockElment.NodeCount != 0)
				return;
			blockElment.SetBlockStart(startRange);
			blockElment.SetBlockEnd(endRange);
		}
		bool MakePropertyAutoImplementedIfNeeded(VBProperty property, SubMemberData subData)
		{
			if (property == null || LessThanVS2010() || property.NodeCount != 0)
				return false;
			property.IsAutoImplemented = true;
			property.SetBlockType(DelimiterBlockType.None);
			property.SetBlockStart(SourceRange.Empty);
			property.SetBlockEnd(SourceRange.Empty);
			if (subData != null)
			{
				Expression init = subData.Initializer;
				if (init != null)
				{
					property.Initializer = init;
					property.AddDetailNode(init);
				}
				ObjectCreationExpression obc = init as ObjectCreationExpression;
				if (property.MemberTypeReference == null && obc != null)
					property.MemberTypeReference = obc.ObjectType;
			}
			return true;
		}
		protected void ReadBlockStartVB(SourceRange range)
		{
			ReadBlockStart(range);
			SetHasBlockAndBlockType();
		}
		protected bool WaitNamespaceEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Namespace);
		}
		protected bool WaitEventEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Event);
		}
		protected bool WaitAddHandlerEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.AddHandler);
		}
		protected bool WaitRemoveHandlerEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.RemoveHandler);
		}
		protected bool WaitRaiseEventEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.RaiseEvent);
		}
		protected bool WaitGetEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Get);
		}
		protected bool WaitSetEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Set);
		}
		protected bool WaitEnumEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Enum);
		}
		protected bool WaitPropertyEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Property);
		}
		protected bool WaitMethodEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Sub, Tokens.Function, Tokens.Operator);
		}
		protected bool WaitTypeEndToken()
		{
			return WaitToken(Tokens.EndToken, Tokens.Class, Tokens.Structure, Tokens.Module, Tokens.Interface);
		}
		bool IsPeekToken(params int[] nextTokens)
		{
			if (nextTokens == null)
				return false;
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken == null)
				return false;
			int peekType = peekToken.Type;
			int count = nextTokens.Length;
			for (int i = 0; i < count; i++)
			{
				int currentTokenType = nextTokens[i];
				if (currentTokenType == peekType)
					return true;
			}
			return false;
		}
		bool WaitToken(int type, params int[] nextTokens)
		{
			while (la != null)
			{
				int laType = la.Type;
				if (laType == Tokens.EOF)
					return true;
				if (laType == type && IsPeekToken(nextTokens))
					return false;
				if (IsStartNewDecl(laType) || (laType == type && IsEndDeclOnPeek()))
					return true;
				ResetPeek();
				Get();
			}
			return false;
		}
		bool IsEndDeclOnPeek()
		{
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken == null || peekToken.Type == Tokens.EOF)
				return true;
			int peekType = peekToken.Type;
			return IsEndDecl(peekType);
		}
		protected bool IsDeclEnd()
		{
			return la != null && la.Type == Tokens.EndToken && IsEndDeclOnPeek();
		}
		protected void WaitIfConditionEnd()
		{
			WaitTokens(Tokens.LineTerminator, Tokens.Then);
		}
		void WaitTokens(params int[] types)
		{
			while (la != null)
			{
				if (la.Type == Tokens.EOF || HasTokenType(la.Type, types))
					return;
				Get();
			}
		}
		bool HasTokenType(int tokenType, int[] tokens)
		{
			int length = tokens.Length;
			for (int i = 0; i < length; i++)
			{
				if (tokenType == tokens[i])
					return true;
			}
			return false;
		}
		protected LanguageElement SetOperatorAttributes(Method method)
		{
			if (method == null)
				return null;
			method.IsClassOperator = true;
			string name = method.Name;
			if (name == null || name == String.Empty)
				return null;
			int paramNumber = 1;
			if (method.Parameters != null && method.Parameters.Count > 1)
				paramNumber = 2;
			OperatorType operatorType = VBOperatorHelper.GetOperatorType(name, paramNumber);
			method.OverloadsOperator = operatorType;
			string newName = VBOperatorHelper.GetOperatorName(operatorType, paramNumber);
			if (newName != null && newName != String.Empty)
				method.Name = newName;
			return method;
		}
		protected bool InInterface()
		{
			return (Context != null && Context is Interface);
		}
		protected string GetMemberType(TypeReferenceExpression typeReference, ICollection arrayModifiers)
		{
			string type = String.Empty;
			if (arrayModifiers != null && arrayModifiers.Count != 0)
			{
				for (int i = 0; i < arrayModifiers.Count; i++)
				{
					if (type != null)
						type += "()";
				}
			}
			if (typeReference == null)
				return null;
			string result = String.Empty;
			TypeReferenceExpression baseType = typeReference.BaseType;
			if (baseType != null)
			{
				result = GetMemberType(baseType, null);
				if (!typeReference.IsArrayType && !typeReference.IsNullable)
					result += ".";
			}
			if (result == null)
				result = String.Empty;
			if (typeReference != null && typeReference.IsArrayType)
			{
				result += "()";
			}
			else if (typeReference != null && typeReference.IsNullable)
			{
				result += "?";
			}
			else if (typeReference != null)
			{
				string name = typeReference.Name;
				if (name != null)
					result += name;
			}
			if (type != null)
			{
				result += type;
			}
			TypeReferenceExpressionCollection coll = typeReference.TypeArguments;
			if (coll == null || coll.Count == 0)
				return result;
			result += "(Of ";
			string typeArgName = null;
			TypeReferenceExpression typeArg = null;
			for (int i = 0; i < coll.Count; i++)
			{
				typeArg = coll[i];
				if (typeArg == null)
					continue;
				typeArgName = typeArg.Name;
				if (typeArgName == null || typeArgName == String.Empty)
					continue;
				if (i != 0)
					result += ",";
				result += typeArgName;
			}
			result += ")";
			return result;
		}
		protected Variable CreateVariableList(out Variable endVar, VariableNameCollection coll, VBDeclarator decl, DeclaratorType declaratorType, VariableListHelper varListHelper)
		{
	  TypeReferenceExpression type = varListHelper.Type;
	  Expression initializer =  varListHelper.Initializer;
	  SourceRange operatorRange = varListHelper.OperatorRange;
	  bool isObjectCreation = varListHelper.IsObjectCreation;
	  bool addToContext = varListHelper.AddToContext;
			endVar = null;
			if (coll == null)
				return null;
			Variable prevVar = null;
			Variable var = null;
			string typeName = null;
			string name = null;
			LanguageElementCollection arrayModifiers = null;
			NullableTypeModifier nullableModifier = null;
			Variable firstVar = null;
			if (isObjectCreation && type == null && initializer is ObjectCreationExpression)
				type = ((ObjectCreationExpression)initializer).ObjectType;
			if (decl != null && decl.FullTypeName != String.Empty)
				typeName = decl.FullTypeName;
			int count = coll.Count;
			for (int i = 0; i < count; i++)
			{
				name = coll[i];
				arrayModifiers = null;
				nullableModifier = null;
				TypeReferenceExpression localType = type;
				VBDeclarator nestedDecl = coll.GetDeclarator(i);
				if (nestedDecl != null)
				{
					arrayModifiers = nestedDecl.ArrayModifiers;
					nullableModifier = nestedDecl.NullableModifier;
					if (nestedDecl.CharacterType != null)
					{
						localType = nestedDecl.CharacterType;
					}
				}
				SourceRange range = coll.GetRange(i);
				SourceRange nameRange = coll.GetNameRange(i);
				Expression cloneInitializer = null;
				if (i == count - 1)
				{
					if (initializer != null)
						cloneInitializer = initializer;
					var = CreateVariable(name, typeName, arrayModifiers, nullableModifier, localType, cloneInitializer, declaratorType, !isObjectCreation) as Variable;
					var.OperatorRange = operatorRange;
		  var.AsRange = varListHelper.AsRange;
				}
				else
				{
					var = CreateVariable(name, typeName, arrayModifiers, nullableModifier, localType, null, declaratorType, false) as Variable;
				}
				if (var == null)
					continue;
				if (prevVar != null)
				{
					prevVar.SetNextVariable(var);
					var.SetPreviousVariable(prevVar);
				}
				prevVar = var;
				if (var != null)
					LocalVarArrayCollection.AddVarArrayName(var);
				var.SetRange(range);
				var.NameRange = nameRange;
				if (var is InitializedVariable && isObjectCreation)
					((InitializedVariable)var).IsObjectCreationInit = true;
				if (firstVar == null)
					firstVar = var;
				if (addToContext)
				{
					AddNode(var);
					SetDefaultVisibility(var as AccessSpecifiedElement);
				}
			}
			if (var != null)
			{
				var.SetRange(GetRange(var.Range, tToken.Range));
				endVar = var;
			}
			return firstVar;
		}
		protected BaseVariable CreateVariable(string name, string type, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType)
		{
			return CreateVariable(name, type, null, null, typeReference, initializer, declaratorType);
		}
		protected BaseVariable CreateVariable(string name, string type, LanguageElementCollection arrayModifiers, NullableTypeModifier nullableModifier, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType)
		{
			return CreateVariable(name, type, arrayModifiers, nullableModifier, typeReference, initializer, declaratorType, true);
		}
		protected BaseVariable CreateSingleVariable(string name, string type, LanguageElementCollection arrayModifiers, NullableTypeModifier nullableModifier, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType, bool isImplicitOnly)
		{
			return CreateVariable(name, type, arrayModifiers, nullableModifier, typeReference, initializer, declaratorType, true, isImplicitOnly);
		}
		protected BaseVariable CreateVariable(string name, string type, LanguageElementCollection arrayModifiers, NullableTypeModifier nullableModifier, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType, bool addTypeToDetailNode)
		{
			return CreateVariable(name, type, arrayModifiers, nullableModifier, typeReference, initializer, declaratorType, addTypeToDetailNode, false);
		}
		protected BaseVariable CreateVariable(string name, string type, LanguageElementCollection arrayModifiers, NullableTypeModifier nullableModifier, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType, bool addTypeToDetailNode, bool isImplicitOnly)
		{
			BaseVariable var = null;
			bool hasType = true;
			if (typeReference == null)
			{
				hasType = false;
				if (LessThanOrcas())
				{
					typeReference = new TypeReferenceExpression("Object", SourceRange.Empty);
				}
			}
			var = GetVar(name, type, typeReference, initializer, declaratorType, isImplicitOnly);
			if (nullableModifier != null)
			{
				var.AddDetailNode(nullableModifier);
				var.NullableModifier = nullableModifier;
			}
			if (arrayModifiers != null && arrayModifiers.Count != 0)
			{
				foreach (ArrayNameModifier arrayNameModifer in arrayModifiers)
				{
					var.ArrayNameModifiers.Add(arrayNameModifer);
					var.AddDetailNode(arrayNameModifer);
				}
			}
			if (type != null && type != String.Empty && var != null)
				var.MemberType = type;
			if (typeReference != null)
			{
		var.MemberTypeReference = ToTypeReferenceExpression(typeReference, arrayModifiers, nullableModifier, var);
				var.TypeRange = typeReference.Range;
				if (addTypeToDetailNode && hasType)
		  var.AddDetailNode(var.MemberTypeReference);
				Variable vht = var as Variable;
				if (vht != null)
					vht.HasType = hasType;
				string memberType = GetMemberType(typeReference, arrayModifiers);
				if (memberType != null && memberType != String.Empty)
					var.MemberType = memberType;
			}
			if (initializer != null && var != null)
			{
		if(var is InitializedVariable)
		  ((InitializedVariable)var).Expression = initializer;
			}
			AddVarToCache(var);
			return var;
		}
		BaseVariable GetVar(string name, string type, TypeReferenceExpression typeReference, Expression initializer, DeclaratorType declaratorType, bool isImplicitOnly)
		{
			BaseVariable var = null;
			if (typeReference == null && AtLeastOrcas() && (initializer != null || isImplicitOnly))
			{
				var = new ImplicitVariable(name, initializer);
				switch (declaratorType)
				{
					case DeclaratorType.Const:
						var.IsConst = true;
						break;
					case DeclaratorType.Static:
						var.IsStatic = true;
						break;
				}
			}
			else
			{
				if (declaratorType == DeclaratorType.Const)
				{
					var = new Const(type, name, initializer);
				}
				else if (declaratorType == DeclaratorType.Dim ||
					declaratorType == DeclaratorType.Static ||
					declaratorType == DeclaratorType.None)
				{
		  if (initializer == null)
			var = new Variable(type, name);
		  else
		  {
		   InitializedVariable initVar = new InitializedVariable(type, name);
		   var = initVar;
		  }
					if (declaratorType == DeclaratorType.Static)
						var.IsStatic = true;
				}
			}
			return var;
		}
		protected bool IsQueryIdent(DeclaratorType declaratorType)
		{
			return AtLeastOrcas() && (declaratorType == DeclaratorType.QueryIdent || declaratorType == DeclaratorType.CanAggregateFunction || declaratorType == DeclaratorType.CanAggregateElement);
		}
		protected BaseVariable CreateQueryIdent(string name, string typeName, TypeReferenceExpression type, Expression initializer, DeclaratorType declaratorType)
		{
			QueryIdent ident = new QueryIdent(typeName, name);
			if (type != null)
			{
				ident.MemberTypeReference = type;
				ident.AddDetailNode(type);
			}
			if (initializer != null)
			{
				if (declaratorType == DeclaratorType.CanAggregateFunction || declaratorType == DeclaratorType.CanAggregateElement)
				{
					initializer = CreateAggregateExpressionIfNeeded(initializer, declaratorType);
				}
				ident.Expression = initializer;
				ident.AddDetailNode(initializer);
			}
			return ident;
		}
		void AddVarToCache(BaseVariable var)
		{
			if (LessThanOrcas()
				|| var == null
				|| _DeclarationsCache == null
				|| (Context != null && Context is TypeDeclaration))
				return;
			_DeclarationsCache.AddDeclaration(var);
		}
		bool IsImplicitVarCore()
		{
			if (LessThanOrcas() || la == null)
				return false;
			return !_DeclarationsCache.HasDelclaration(la.Value);
		}
		protected bool IsImplicitVar()
		{
			if (!IsImplicitVarCore())
				return false;
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken.Type == Tokens.Dot)
				return false;
			return true;
		}
		protected bool IsDeclareVariableCoreWithDecloratorType()
		{
			ResetPeek();
			Token peekToken = Peek();
			return (peekToken.Type == Tokens.As
				|| peekToken.Type == Tokens.EqualsSymbol
				|| peekToken.Type == Tokens.ParenOpen
				);
		}
		protected override void OpenContext(LanguageElement context)
		{
			if (context == null)
				return;
			if (Context != null && Context == context)
				return;
			base.OpenContext(context);
		}
		protected override void CloseContext()
		{
			base.CloseContext();
		}
		protected string ToMemberType(string typeName, LanguageElementCollection arrayModifiers)
		{
			if (typeName == null)
				return null;
			if (arrayModifiers == null || arrayModifiers.Count == 0)
				return typeName;
			for (int i = 0; i < arrayModifiers.Count; i++)
				typeName += "()";
			return typeName;
		}
		bool IsEOL(int type)
		{
			if (type == Tokens.LineTerminator || type == Tokens.Colon)
				return true;
			return false;
		}
		protected bool IsVarInUsing()
		{
			if (!IsIdentifierOrKeyword(la.Type))
				return false;
			ResetPeek();
			Token peekToken = Peek();
			Token postPeekToken = Peek();
			while (peekToken.Type == Tokens.Dot && IsIdentifierOrKeyword(postPeekToken.Type))
			{
				peekToken = Peek();
				postPeekToken = Peek();
			}
			return !IsEOL(peekToken.Type);
		}
		protected bool IsVariableInLoopControlVariable()
		{
			ResetPeek();
			Token peekToken = Peek();
			Token postPeekToken = Peek();
			while (peekToken.Type == Tokens.Dot
				&& IsIdentifierOrKeyword(postPeekToken.Type))
			{
				peekToken = Peek();
				postPeekToken = Peek();
			}
			if (peekToken.Type == Tokens.EqualsSymbol)
				return false;
			return true;
		}
		protected TypeReferenceExpression ToTypeReferenceExpression(TypeReferenceExpression type, LanguageElementCollection arrayModifiers, LanguageElement nullableModifier, LanguageElement parent)
		{
			if (type == null)
				return null;
	  type = type.Clone() as TypeReferenceExpression;
	  if (type == null)
		return null;
	  if (nullableModifier != null)
		type = new TypeReferenceExpression(type, true);
	  if (arrayModifiers != null && arrayModifiers.Count != 0)
	  {
		ArrayNameModifier modif = null;
		ExpressionCollection coll = null;
		int rank;
		for (int i = 0; i < arrayModifiers.Count; i++)
		{
		  modif = arrayModifiers[i] as ArrayNameModifier;
		  if (modif == null)
			continue;
		  rank = modif.Rank;
		  coll = modif.SizeInitializers.DeepClone() as ExpressionCollection;
		  SourceRange range = type.Range;
		  type = new TypeReferenceExpression(type, rank, coll);
		  type.SetRange(range);
		}
	  }
	  type.SetParent(parent);
			return type;
		}
		protected bool IsLabel()
		{
			if (la == null || (la.Type != Tokens.Identifier && la.Type != Tokens.IntegerLiteral))
				return false;
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken.Type == Tokens.Colon)
				return true;
			return false;
		}
		protected bool NeedExitFromStatementLoop()
		{
	  if (la == null || la.Type == Tokens.EndToken)
	  {
		if (!ParsingRazor)
		  return true;
		ResetPeek();
		Token nextToken = Peek();
		if (IsRazorCodeKeyword(nextToken))
		  return false;
		return true;
	  }
			return false;
		}
		protected bool IsEndStatement()
		{
			if (la == null || la.Type != Tokens.EndToken)
				return false;
			ResetPeek();
			Token token = Peek();
	  return (token == null || token.Type == Tokens.LineTerminator
		|| token.Type == Tokens.SingleLineComment || token.Type == Tokens.Colon
		|| IsRazorCodeKeyword(token));
		}
		protected void SetGenericArgumentsToQualifiedIdentifier(ReferenceExpressionBase expression)
		{
			if (expression == null)
				return;
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken != null && peekToken.Type == Tokens.Of)
			{
				ExpressionCollection arguments = null;
				int typeArity = ConstructedTypeNameTailBase(out arguments);
				SetGenericTypeArguments(arguments, tToken, expression);
				if (expression != null && expression is TypeReferenceExpression)
				{
					if (arguments.Count == 0)
					{
						(expression as TypeReferenceExpression).IsUnbound = true;
					}
					(expression as TypeReferenceExpression).TypeArity = typeArity;
				}
			}
		}
		protected Expression CreateAttributeArgumentInitializer(Token token, Expression expression)
		{
			if (expression == null)
				return null;
			if (token == null)
				return expression;
			ElementReferenceExpression reference = GetElementReference(token.Value, token.Range);
			AttributeVariableInitializer initializer = new AttributeVariableInitializer(reference, token, expression);
			SetRange(initializer, reference, expression);
			return initializer;
		}
		protected void AddDirective(CompilerDirective directive)
		{
			if (CompilerDirectiveContext != null)
				CompilerDirectiveContext.AddNode(directive);
			else if (RootNode is SourceFile)
			{
				SourceFile fileNode = (SourceFile)RootNode;
				if (fileNode.CompilerDirectiveRootNode != null)
					fileNode.CompilerDirectiveRootNode.AddNode(directive);
			}
		}
		protected void AddAttribute(AttributeSection section, Attribute attribute)
		{
			if (section == null || attribute == null)
				return;
			ProcessAttribute(attribute);
			section.AttributeCollection.Add(attribute);
			section.AddDetailNode(attribute);
		}
		protected void AddAttributeArgument(Attribute attribute, Expression argument)
		{
			if (attribute == null || argument == null)
				return;
			attribute.Arguments.Add(argument);
			attribute.AddDetailNode(argument);
		}
		protected void AddAttributes(CodeElement element, LanguageElementCollection attributes)
		{
			if (attributes == null || attributes.Count == 0 || element == null)
				return;
			if (element != null)
				element.SetAttributes(attributes);
			else
			{
				int count = attributes.Count;
				for (int i = 0; i < count; i++)
					AddNode(attributes[i] as LanguageElement);
			}
		}
	protected string GetDateLiteral(string month, string day, string year, string hour,
									  string minute, string second, string ampm, string dateDelemiter)
	{
	  string date = null;
	  StringBuilder sb = new StringBuilder();
	  if (dateDelemiter == null)
		dateDelemiter = "-";
	  if (month != null)
	  {
		sb.Append(month);
		if (day == null)
		  return date;
		sb.Append(dateDelemiter);
		sb.Append(day);
		if (year == null)
		  return date;
		sb.Append(dateDelemiter);
		sb.Append(year);
	  }
	  if (hour != null)
	  {
		if (month != null)
		  sb.Append(" ");
		sb.Append(hour);
		if (minute != null)
		{
		  sb.Append(":");
		  sb.Append(minute);
		}
		if (second != null)
		{
		  sb.Append(":");
		  sb.Append(second);
		}
		if (ampm != null)
		  sb.Append(ampm);
	  }
	  return sb.ToString();
	}
		protected PrimitiveExpression GetPrimitiveExpression(string value, SourceRange range, PrimitiveType type)
		{
	  string calcValue = value;
	  if (type == PrimitiveType.DateTime)
		calcValue = string.Concat("#", value, "#");
			PrimitiveExpression result = new PrimitiveExpression(calcValue);
			result.Name = calcValue;
			result.PrimitiveType = type;
			try
			{
				result.PrimitiveValue = VBPrimitiveTypeUtils.ToPrimitiveValueFromPrimitiveType(type, value);
			}
			catch
			{
			}
			result.SetRange(range);
			return result;
		}
		#region GetBinaryExpression
		protected BinaryOperatorExpression GetBinaryExpression(Expression left, string operatorName, BinaryOperatorType type, Expression right, SourceRange nameRange)
		{
			BinaryOperatorExpression result = new BinaryOperatorExpression(left, operatorName, right);
			result.BinaryOperator = type;
			result.Name = operatorName;
			result.SetOperatorRange(nameRange);
			result.SetRange(GetRange(left, right));
			return result;
		}
		#endregion
		#region GetLogicalOperation
		protected LogicalOperation GetLogicalOperation(Expression left, string operatorName, LogicalOperator type, Expression right, SourceRange nameRange)
		{
			LogicalOperation result = new LogicalOperation(left, type, right);
			result.Name = operatorName;
			result.SetOperatorRange(nameRange);
			result.SetRange(GetRange(left, right));
			return result;
		}
		#endregion
		#region GetRelationalOperation
		protected RelationalOperation GetRelationalOperation(Expression left, string operatorName, RelationalOperator type, Expression right, SourceRange defaultRange)
		{
			RelationalOperation result = new RelationalOperation(left, type, right);
			result.Name = operatorName;
			if (left == null && defaultRange != SourceRange.Empty)
				result.SetRange(GetRange(defaultRange, right.Range));
			else
				result.SetRange(GetRange(left, right));
			return result;
		}
		#endregion
		#region GetExpressionForUnaryOperation
		protected Expression GetExpressionForUnaryOperation(string name, SourceRange start, Expression expression)
		{
			if (expression == null)
				return null;
			Expression result = new UnaryOperatorExpression(name, expression, false);
			result.Name = name;
			result.SetRange(GetRange(start, expression));
			return result;
		}
		#endregion
		#region GetAssignment
		protected Assignment GetAssignment(Expression left, SourceTextRange textRange, AssignmentOperatorType type, Expression right)
		{
			Assignment result = new Assignment();
			result.LeftSide = left;
			result.AddDetailNode(left);
			result.AssignmentOperator = type;
			result.Operator = textRange;
			result.Name = textRange.Text;
			result.Expression = right;
			result.AddDetailNode(right);
			result.SetRange(GetRange(left, right));
			return result;
		}
		protected AssignmentExpression GetAssignmentExpression(Expression left, string operaorText, AssignmentOperatorType type, SourceRange operatorRange, Expression right)
		{
			AssignmentExpression result = new AssignmentExpression();
			result.LeftSide = left;
			result.AssignmentOperator = type;
			result.OperatorText = operaorText;
			result.Name = operaorText;
			result.RightSide = right;
			result.SetRange(GetRange(left, right));
			return result;
		}
		#endregion
		#region GetNullableType
		protected TypeReferenceExpression GetNullableType(TypeReferenceExpression expression)
		{
			return GetNullableType(expression, tToken);
		}
		protected TypeReferenceExpression GetNullableType(TypeReferenceExpression expression, Token token)
		{
			if (expression == null)
				return null;
			TypeReferenceExpression oldType = expression;
			expression = new TypeReferenceExpression(oldType);
			expression.NameRange = token.Range;
			expression.IsNullable = true;
			expression.SetRange(GetRange(oldType, token));
			expression.TypeReferenceType = TypeReferenceType.None;
			return expression;
		}
		#endregion
		#region GetExpression()
		protected ReferenceExpressionBase GetExpression(string name, SourceRange nameRange, CreateElementType elementType)
		{
			switch (elementType)
			{
				case CreateElementType.ElementReferenceExpression:
					return GetElementReference(name, nameRange);
				case CreateElementType.TypeReferenceExpression:
					return GetTypeReferenceExpression(name, nameRange);
			}
			return null;
		}
		#endregion
		#region GetExpression()
		protected ReferenceExpressionBase GetExpression(Expression source, string name, SourceRange nameRange, CreateElementType elementType)
		{
			switch (elementType)
			{
				case CreateElementType.ElementReferenceExpression:
					return GetElementReference(source, name, nameRange);
				case CreateElementType.TypeReferenceExpression:
					return GetTypeReferenceExpression(source, name, nameRange);
			}
			return null;
		}
		#endregion
		#region QualifiedIdentifier(out QualifiedIdentifier identifier)
		#endregion
		protected bool isElementRef()
		{
			if (la == null)
				return false;
			int type = la.Type;
			return (type == Tokens.Class || type == Tokens.Structure ||
				type == Tokens.Interface || type == Tokens.Module ||
				type == Tokens.Enum || type == Tokens.Delegate || type == Tokens.Declare ||
				type == Tokens.Function || type == Tokens.Sub || type == Tokens.Operator ||
				type == Tokens.Event || type == Tokens.Ansi || type == Tokens.Auto ||
				type == Tokens.Unicode || type == Tokens.Property || type == Tokens.Custom ||
				type == Tokens.Dim || type == Tokens.Const);
		}
		protected bool IsLocalVar()
		{
			if (la == null)
				return false;
			int type = la.Type;
			return (type == Tokens.Dim || type == Tokens.Static || type == Tokens.Const);
		}
		protected bool IsField(TokenQueueBase modifiers)
		{
			if (Context == null)
				return false;
			bool isPrevDeclField = false;
			if (modifiers != null && modifiers.Count != 0)
			{
				int countModif = modifiers.Count;
				for (int i = 0; i < countModif; i++)
				{
					Token token = modifiers.LookUpToken(i);
					switch (token.Type)
					{
						case Tokens.Friend:
						case Tokens.Shadows:
						case Tokens.Private:
						case Tokens.Protected:
						case Tokens.Public:
						case Tokens.Shared:
						case Tokens.ReadOnly:
						case Tokens.WithEvents:
						case Tokens.Const:
							isPrevDeclField = true;
							break;
					}
				}
			}
			int type = la.Type;
			bool isNextFieldDecl = (type == Tokens.Dim || type == Tokens.Const);
			bool isElementDecl = (type == Tokens.Class || type == Tokens.Structure ||
				type == Tokens.Interface || type == Tokens.Module ||
				type == Tokens.Enum || type == Tokens.Delegate || type == Tokens.Declare ||
				type == Tokens.Function || type == Tokens.Sub || type == Tokens.Operator ||
				type == Tokens.Event || type == Tokens.Ansi || type == Tokens.Auto ||
				type == Tokens.Unicode || type == Tokens.Property || type == Tokens.Custom);
			if (isNextFieldDecl)
				return true;
			if (isPrevDeclField && !isElementDecl)
				return true;
			return false;
		}
		#region GetTypeReferenceExpression
		TypeReferenceExpression GetTypeReferenceExpression(string name, SourceRange range)
		{
			TypeReferenceExpression result = new TypeReferenceExpression(name, range);
			result.NameRange = range;
			return result;
		}
		#endregion
		#region GetElementReference
		protected ElementReferenceExpression GetElementReference(string name, SourceRange range)
		{
			ElementReferenceExpression result = new ElementReferenceExpression(name, range);
			result.NameRange = range;
			return result;
		}
		#endregion
		#region GetElementReference
		protected ElementReferenceExpression GetElementReference(Expression source, string name, SourceRange nameRange)
		{
			ElementReferenceExpression result = new QualifiedElementReference(source, name, nameRange);
			SetPropertiesForElementReferenceExpression(result, source, nameRange);
			return result;
		}
		#endregion
		#region GeXmlAttributeReferenceExpression
		protected ElementReferenceExpression GetXmlAttrElementReference(Expression source, string name, SourceRange nameRange)
		{
			ElementReferenceExpression result = new XmlAttributeReferenceExpression(source, name, nameRange);
			SetPropertiesForElementReferenceExpression(result, source, nameRange);
			return result;
		}
		#endregion
		protected ElementReferenceExpression GetXmlElementReference(Expression source, string name, SourceRange nameRange)
		{
			ElementReferenceExpression result = new XmlElementReferenceExpression(source, name, nameRange);
			SetPropertiesForElementReferenceExpression(result, source, nameRange);
			return result;
		}
		void SetPropertiesForElementReferenceExpression(ElementReferenceExpression result, Expression source, SourceRange nameRange)
		{
			if (result == null)
				return;
			result.AddNode(source);
			if (source != null)
			{
				result.Qualifier = source;
				result.SetRange(source.Range.Start, nameRange.End);
			}
		}
		protected bool ConditionContext()
		{
			return Context is ConditionalParentToSingleStatement;
		}
		protected void SetRange(LanguageElement element, params object[] args)
		{
			SourceRange range = GetRange(args);
			element.SetRange(range);
		}
		Token SkipParamQualifiers()
		{
			ResetPeek();
			Token peekToken = la;
			while (peekToken != null &&
				IsParamQualifier(peekToken.Type))
			{
				peekToken = Peek();
			}
			if (peekToken == null)
				return peekToken;
			peekToken = Peek();
			return peekToken;
		}
		bool IsParamQualifier(int type)
		{
			return type == Tokens.ByRef || type == Tokens.ByVal || type == Tokens.ParamArray;
		}
		protected bool IsLambdaParameter()
		{
			Token peekToken = SkipParamQualifiers();
			if (peekToken == null || peekToken.Type == Tokens.As)
				return false;
			return true;
		}
		protected DelegateDefinition CreateDelegate(SubMemberData subMember, VBDeclarator decl)
		{
			if (subMember == null)
				return null;
			Token nameToken = subMember.NameToken;
			string name = nameToken.Value;
			TypeReferenceExpression typeReference = subMember.MemberTypeReference;
			string typeName = null;
			if (typeReference != null)
				typeName = typeReference.Name;
			DelegateDefinition result = new DelegateDefinition(name);
			result.MemberType = typeName;
			FinishCreateMember(result, subMember, decl, null);
			return result;
		}
		protected VBEvent CreateEvent(SubMemberData subMember, VBDeclarator decl)
		{
			if (subMember == null)
				return null;
			Token nameToken = subMember.NameToken;
			string name = null;
			if (nameToken != null)
			{
				name = nameToken.Value;
			}
			VBEvent result = new VBEvent(name);
			TypeReferenceExpression typeReference = subMember.MemberTypeReference;
			string typeName = null;
			if (typeReference != null)
				typeName = typeReference.Name;
			if (Context != null && Context is Interface)
				result.IsInterfaceEvent = true;
			FinishCreateMember(result, subMember, decl, null);
			return result;
		}
		protected Method CreateMethod(SubMemberData subMember, MethodTypeEnum methodTypeEnum, TokenQueueBase modifiers, VBDeclarator decl, SourceRange methodKeyWordRange)
		{
			if (subMember == null)
				return null;
			Token nameToken = subMember.NameToken;
			string name = null;
			if (nameToken != null)
			{
				if (nameToken.Type == Tokens.New)
					methodTypeEnum = MethodTypeEnum.Constructor;
				name = nameToken.Value;
			}
			TypeReferenceExpression typeReference = subMember.MemberTypeReference;
			string typeName = null;
			if (typeReference != null)
				typeName = typeReference.Name;
			Method result = new VBMethod(typeName, name);
			result.MethodType = methodTypeEnum;
			result.Lib = subMember.LibString;
			result.Alias = subMember.AliasString;
			FinishCreateMember(result, subMember, decl, modifiers);
			result.SetMethodKeyWordRange(methodKeyWordRange);
			result.ParamOpenRange = subMember.ParamOpenRange;
			result.ParamCloseRange = subMember.ParamCloseRange;
			if (methodTypeEnum != MethodTypeEnum.Void)
			{
				BaseVariable var = GetImplicitVariableToMethod(subMember);
				var.IsImplicit = true;
				var.IsReturnedValue = true;
				var.SetParent(result);
				result.ImplicitVariable = var;
			}
			SourceRange asRange = subMember.AsRange;
			result.SetAsRange(asRange);
			return result;
		}
	bool NeedToTrimEndDoubleQuote(string value)
	{
	  bool trimEndDoubleQuote = true;
	  if (!string.IsNullOrEmpty(value))
	  {
		int count = 0;
		int lenght = value.Length - 1;
		for (int i = lenght; i >= 0; i--)
		  if (value[i] == '"')
			count++;
		  else
			break;
		if (count % 2 == 0)
		  trimEndDoubleQuote = false;
	  }
	  return trimEndDoubleQuote;
	}
	protected void AddTextString(Token stringToken)
		{
			if (stringToken == null)
				return;
	  string value = stringToken.Value;
	  bool trimEndDoubleQuote = NeedToTrimEndDoubleQuote(value);
	  int stringLiteralLength = value.Length - 1;
	  if (trimEndDoubleQuote && stringLiteralLength > 0)
		stringLiteralLength--;
			TextString textString = new TextString(value.Substring(1, stringLiteralLength), false);
			SourceRange stringRange = stringToken.Range;
			SourcePoint startPoint = stringRange.Start;
			SourcePoint endPoint = stringRange.End;
			int startOffset = startPoint.Offset + 1;
			int endOffset = endPoint.Offset - (trimEndDoubleQuote ? 1 : 0);
			startPoint.SetOffset(startOffset);
			endPoint.SetOffset(endOffset);
			stringRange = new SourceRange(startPoint, endPoint);
			textString.SetRange(stringRange);
			TextStrings.Add(textString);
		}
		BaseVariable GetImplicitVariableToMethod(SubMemberData subMember)
		{
			if (subMember == null)
				return null;
			SourceRange nameRange = subMember.NameRange;
			TypeReferenceExpression typeReference = subMember.MemberTypeReference;
			if (typeReference != null)
				typeReference = typeReference.Clone() as TypeReferenceExpression;
			string typeName = null;
			if (typeReference != null)
				typeName = typeReference.Name;
			Token nameToken = subMember.NameToken;
			string name = null;
			if (nameToken != null)
				name = nameToken.Value;
			BaseVariable result = CreateVariable(name, typeName, typeReference, null, DeclaratorType.Dim);
			return result;
		}
		protected VBProperty CreateProperty(SubMemberData subMember, TokenQueueBase modifiers, VBDeclarator decl)
		{
			if (subMember == null)
				return null;
			VBProperty result = new VBProperty();
			FinishCreateMember(result, subMember, decl, modifiers);
	  result.GenerateParens = !result.ParensRange.IsEmpty;
			return result;
		}
		void SetParametersToLocalVarArrayCollection(LanguageElementCollection parameters)
		{
			if (parameters == null || parameters.Count == 0)
				return;
			foreach (BaseVariable var in parameters)
			{
				LocalVarArrayCollection.AddVarArrayName(var);
			}
		}
		void FinishCreateMember(MemberWithParameters result, SubMemberData subMember, VBDeclarator decl, TokenQueueBase modifiers)
		{
			LanguageElementCollection parameters = subMember.ParamCollection;
			GenericModifier genericModifier = subMember.GenericModifier;
			SourceRange nameRange = subMember.NameRange;
			ExpressionCollection handles = subMember.HandlesExpressions;
			TypeReferenceExpression typeReference = subMember.MemberTypeReference;
			LocalVarArrayCollection.Clear();
			FinishCreateMember(result, parameters, typeReference, modifiers, decl, genericModifier, nameRange);
			SourceRange parensRange = GetRange(subMember.ParamOpenRange, subMember.ParamCloseRange);
			result.SetParensRange(parensRange);
			if (handles != null && handles.Count != 0 && result != null)
			{
				foreach (Expression exp in handles)
				{
					result.AddHandlesExpression(exp);
					if (result is Method)
						(result as Method).Handles.Add(exp.ToString());
				}
			}
			ExpressionCollection implements = subMember.ImplementsCollection;
			if (implements != null)
			{
				result.IsExplicitInterfaceMember = true;
				foreach (Expression exp in implements)
					result.AddImplementsExpression(exp);
			}
			SetDefaultVisibility(result as AccessSpecifiedElement);
		}
		void SetDefaultVisibility(AccessSpecifiedElement element)
		{
			if (element == null || !element.IsDefaultVisibility || Context == null)
				return;
	  MemberVisibility memberVisibility = MemberVisibility.Illegal;
	  if (Context is SourceFile || Context is Namespace)
		memberVisibility = MemberVisibility.Internal;
	  else if (Context is Struct)
		memberVisibility = MemberVisibility.Public;
			else if (Context is TypeDeclaration)
			{
				if (element is Variable)
		  memberVisibility = MemberVisibility.Private;
				else
		  memberVisibility = MemberVisibility.Public;
			}
	  else
		memberVisibility = MemberVisibility.Local;
	  element.SetVisibility(memberVisibility);
			element.IsDefaultVisibility = true;
		}
		void AddType(MemberWithParameters result, TypeReferenceExpression type)
		{
			if (result == null || type == null)
				return;
			result.MemberTypeReference = type;
			result.TypeRange = type.Range;
			PassAttributes(result, type);
		}
		protected void FinishCreateMember(MemberWithParameters result, LanguageElementCollection parameters, TypeReferenceExpression type, TokenQueueBase modifiers, VBDeclarator decl, GenericModifier genericModifier, SourceRange nameRange)
		{
			if (result == null)
				return;
			if (type != null && type.IsTypeCharacter)
			{
				AddType(result, type);
			}
			if (parameters != null && parameters.Count != 0)
			{
				foreach (LanguageElement element in parameters)
				{
					LocalVarArrayCollection.AddVarArrayName(element as BaseVariable);
					PassAttributes(result, element as CodeElement);
					((MemberWithParameters)result).Parameters.Add(element);
				}
			}
			if (type != null && !type.IsTypeCharacter)
			{
				AddType(result, type);
			}
			if (modifiers != null)
				VB90SpecifiersHelper.ParseSpecifiers(modifiers, result as AccessSpecifiedElement);
			if (genericModifier != null)
				result.SetGenericModifier(genericModifier);
			if (decl != null)
				result.MemberType = decl.FullTypeName;
			if (type != null)
				result.MemberType = GetMemberType(type, modifiers);
			if (nameRange != SourceRange.Empty)
				result.NameRange = nameRange;
		}
		protected void SetGenericTypeArguments(ExpressionCollection arguments, Token endToken, ReferenceExpressionBase result)
		{
			if (arguments == null || result == null)
				return;
			if (arguments.Count != 0)
			{
				int argumentCount = arguments.Count - 1;
				TypeReferenceExpression type = null;
				for (int i = 0; i <= argumentCount; i++)
				{
					type = arguments[i] as TypeReferenceExpression;
					if (type == null)
						continue;
					result.TypeArguments.Add(type);
					result.AddDetailNode(type);
				}
			}
			if (endToken != null)
				result.SetRange(GetRange(result.Range, endToken.Range));
		}
		protected void SetBlockRange(DelimiterCapableBlock block, SourceRange startRange, SourceRange endRange)
		{
			if (block == null)
				return;
			block.SetBlockStart(startRange);
			block.SetBlockEnd(endRange);
			block.SetBlockType(DelimiterBlockType.Token);
		}
		#region GetObjectCreationExpression
		protected ObjectCreationExpression GetObjectCreationExpression(TypeReferenceExpression type, ExpressionCollection args, SourceRange startRange, SourceRange endRange, SourceRange parensRange)
		{
			ObjectCreationExpression objectCreationExpression = new ObjectCreationExpression(type, args);
			if (args != null && args.Count != 0)
			{
				foreach (LanguageElement element in args)
					objectCreationExpression.AddDetailNode(element);
			}
			if (type != null)
			{
				objectCreationExpression.AddNode(type);
			}
			objectCreationExpression.SetRange(GetRange(startRange, tToken.Range));
			objectCreationExpression.SetParensRange(parensRange);
			return objectCreationExpression;
		}
		#endregion
		#region GetArrayTypeReference()
		protected TypeReferenceExpression GetArrayTypeReference(TypeReferenceExpression sourceType, ExpressionCollection coll, int rank)
		{
			TypeReferenceExpression returnType = null;
			if (coll == null)
				returnType = new TypeReferenceExpression(sourceType, rank);
			else
				returnType = new TypeReferenceExpression(sourceType, rank, coll);
			if (sourceType != null)
				returnType.SetRange(GetRange(sourceType.Range, tToken.Range));
			return returnType;
		}
		#endregion
		#region GetArrayCreateExpression
		protected ArrayCreateExpression GetArrayCreateExpression(TypeReferenceExpression type, ArrayInitializerExpression initializer, SourceRange startRange, SourceRange endRange)
		{
			ArrayCreateExpression arrayCreateExpression;
			if (type != null)
			{
				arrayCreateExpression = new ArrayCreateExpression(type);
				arrayCreateExpression.AddNode(type);
			}
			else
			{
				arrayCreateExpression = new ArrayCreateExpression();
			}
			if (initializer != null)
			{
				arrayCreateExpression.AddDetailNode(initializer);
				arrayCreateExpression.Initializer = initializer;
			}
			arrayCreateExpression.SetRange(GetRange(startRange, tToken.Range));
			return arrayCreateExpression;
		}
		#endregion
		#region GetTypeReferenceExpression
		protected TypeReferenceExpression GetTypeReferenceExpression(Expression source, string name, SourceRange range)
		{
			TypeReferenceExpression result = new TypeReferenceExpression(name, range);
			result.NameRange = range;
			result.AddDetailNode(source);
			result.Qualifier = source;
			result.SetRange(source.Range.Start, range.End);
			return result;
		}
		#endregion
		#region Parse methods...
		public Expression ParseConditionExpression(ref Token laToken, ref Token token)
		{
			if (laToken != null)
			{
				la = laToken;
			}
	  if (token != null)
			{
		tToken = token;
			}
			Expression result = null;
			ExpressionRuleBase(out result);
			laToken = la;
	  token = tToken;
			return result;
		}
		void PreparePreprocessor()
		{
			if (RootNode != null && scanner != null)
			{
				SourceFile sourceFile = RootNode as SourceFile;
				if (sourceFile == null)
				{
					sourceFile = RootNode.GetSourceFile();
				}
				if (sourceFile == null)
				{
					sourceFile = GetSourceFile(string.Empty);
				}
				_Preprocessor = new VBPreprocessor(scanner as VB90Scanner, sourceFile);
				_Preprocessor.VbParser = this;
			}
		}
		protected void PrepareParse()
		{
			PrepareParse(RootNode);
		}
		protected void PrepareParse(LanguageElement context)
		{
			if (context != null && context is SourceFile)
			{
				((SourceFile)context).TextStrings = TextStrings;
				TextStrings.Clear();
				Comments.Clear();
			}
			_NotXmlNode = false;
			_DeclarationsCache.Reset();
			PreparePreprocessor();
			la = new Token();
			la.Value = "";
			Get();
		}
		protected void FinishParse(LanguageElement context)
		{
			SourcePoint point = tToken.Range.End;
			point.Offset = point.Offset + 1;
			Expect(0);
			if (Context != null)
			{
				if (Context is SourceFile)
				{
					SetContextEnd(point);
					CloseContext();
				}
				_NotXmlNode = false;
				BindComments();
			}
	  AfterParsing(context);
		}
		protected bool OnDemand
		{
			get
			{
		return false;
			}
		}
		public override string GetFullTypeName(string simpleName)
		{
			return VB90Tokens.Instance.GetFullTypeName(simpleName);
		}
		protected TypeReferenceExpression ParseTypeReferenceExpression()
		{
			PrepareParse();
			TypeReferenceExpression result = null;
			WhitespaceLinesBase();
			ParseTypeExpressionRule(out result);
	  FinishParse(result);
			return result;
		}
		protected Expression ParseExpression()
		{
			PrepareParse();
			Expression result = null;
			WhitespaceLinesBase();
			ExpressionRuleBase(out result);
	  FinishParse(result);
			return result;
		}
		protected Statement ParseStatement()
		{
			PrepareParse();
			Statement result = null;
			WhitespaceLinesBase();
			StatementRuleBase(out result);
	  FinishParse(result);
			return result;
		}
		protected BaseVariable ParseVariable()
		{
			PrepareParse();
			BaseVariable var = null;
			WhitespaceLinesBase();
			LocalDeclarationStatementCoreBase(out var);
			FinishParse(var);
			return var;
		}
		protected Member ParseMember()
		{
			PrepareParse();
			Member result = null;
			WhitespaceLinesBase();
			ClassMemberDeclarationBase(out result);
			FinishParse(result);
			return result;
		}
		protected LanguageElement Parse(LanguageElement context, ISourceReader reader)
		{
			if (context == null)
				return null;
			SetRootNode(context);
			try
			{
				scanner = new VB90Scanner(reader);
				Parse();
				return RootNode;
			}
			finally
			{
				reader.Dispose();
				CleanUpParser();
			}
		}
		public LanguageElement ParseExpression(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				try
				{
					scanner = new VB90Scanner(reader);
					return ParseExpression();
				}
				finally
				{
					CleanUpParser();
				}
			}
		}
		public TypeReferenceExpression ParseTypeReferenceExpression(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				try
				{
					scanner = new VB90Scanner(reader);
					return ParseTypeReferenceExpression();
				}
				finally
				{
					CleanUpParser();
				}
			}
		}
		public LanguageElement ParseStatement(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				try
				{
					scanner = new VB90Scanner(reader);
					return ParseStatement();
				}
				finally
				{
					CleanUpParser();
				}
			}
		}
		public LanguageElement ParseVariable(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				try
				{
					scanner = new VB90Scanner(reader);
					return ParseVariable();
				}
				finally
				{
					CleanUpParser();
				}
			}
		}
		public LanguageElement ParseMember(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				try
				{
					scanner = new VB90Scanner(reader);
					return ParseMember();
				}
				finally
				{
					CleanUpParser();
				}
			}
		}
		public override SourceFile GetSourceFile(string fileName)
		{
			return new VBSourceFile(fileName);
		}
		public LanguageElement Parse(ISourceReader reader)
		{
			lock (_SyncObject)
			{
				SourceFile context = GetSourceFile("RootNode");
				return Parse(context, reader);
			}
		}
		#endregion
		protected void SetHasBlockAndBlockType()
		{
			SetHasBlockAndBlockType(Context);
		}
		protected void SetHasBlockAndBlockType(LanguageElement element)
		{
			if (element == null)
				return;
			if (element is ParentingStatement)
				(element as ParentingStatement).HasBlock = true;
			if (element is DelimiterCapableBlock)
				((DelimiterCapableBlock)element).SetBlockType(DelimiterBlockType.Token);
		}
		protected Token SkipTokensToMethod()
		{
			return SkipTokens(new int[] { Tokens.Function, Tokens.Sub, Tokens.Operator });
		}
		protected Token SkipTokensToProperty()
		{
			return SkipTokens(new int[] { Tokens.Property });
		}
		void AddTextStringOnDemand(Token token)
		{
			if (token == null || token.Type != Tokens.StringLiteral)
				return;
			AddTextString(token);
		}
		protected Token SkipTokens(params int[] tokenTypes)
		{
			if (tokenTypes == null)
				return tToken;
			int afterEndToken;
			for (; ; )
			{
				if (la.Type == Tokens.EOF || la == null)
					return tToken;
				AddTextStringOnDemand(la);
				if (la.Type == Tokens.EndToken)
				{
					Token returnToken = tToken;
					Get();
					AddTextStringOnDemand(la);
					if (la.Type == Tokens.EOF || la == null)
						return tToken;
					for (int i = 0; i < tokenTypes.Length; i++)
					{
						afterEndToken = tokenTypes[i];
						if (la.Type == afterEndToken)
						{
							return returnToken;
						}
					}
				}
				Get();
			}
		}
		protected void SetPropertyAccessors(VBProperty property, LanguageElementCollection accessors)
		{
			if (property == null || accessors == null || accessors.Count == 0)
				return;
			foreach (Accessor accessor in accessors)
			{
				AddImplicitParamToAccessor(property, accessor);
				if (accessor != null && accessor.Visibility == MemberVisibility.Illegal)
				{
					accessor.Visibility = property.Visibility;
				}
			}
		}
		protected void SetBlockEnd(LanguageElement element)
		{
			if (element == null || !(element is DelimiterCapableBlock))
				return;
			((DelimiterCapableBlock)element).SetBlockEnd(tToken.Range);
		}
		protected void SetBlockStart(LanguageElement element, SourceRange range)
		{
			if (element == null || !(element is DelimiterCapableBlock))
				return;
			((DelimiterCapableBlock)element).SetBlockStart(range);
			SetHasBlockAndBlockType(element);
		}
		protected void AddBlockRangeOnDemand(LanguageElement element, OnDemandParsingParameters onDemandParsingParameters)
		{
			if (onDemandParsingParameters == null || element == null || !(element is Method || element is VBProperty))
				return;
			Token startBlockTokenOnDemand = onDemandParsingParameters.StartBlockToken;
			Token endBlockTokenOnDemand = onDemandParsingParameters.EndBlockToken;
			AddBlockRangeOnDemand(element, startBlockTokenOnDemand, endBlockTokenOnDemand);
		}
		protected void AddBlockRangeOnDemand(LanguageElement element, Token startToken, Token endToken)
		{
			if (OnDemand && scanner != null && scanner is VBScannerBase && (scanner as VBScannerBase).SourceReader != null
				&& startToken != null && endToken != null)
			{
			}
		}
		protected IndexerExpression CreateIndexerExpression(Expression sourceExpression, Expression argument)
		{
			IndexerExpression indexerExpression = new IndexerExpression(sourceExpression);
			AddArgumentsToIndexerExpression(indexerExpression, argument);
			return indexerExpression;
		}
		protected IndexerExpression CreateIndexerExpression(Expression sourceExpression, ExpressionCollection arguments)
		{
			IndexerExpression indexerExpression = new IndexerExpression(sourceExpression);
			AddArgumentsToIndexerExpression(indexerExpression, arguments);
			return indexerExpression;
		}
		void AddArgumentsToIndexerExpression(IndexerExpression indexerExpression, Expression argument)
		{
			if (argument == null)
				return;
			if (indexerExpression.Arguments == null)
				indexerExpression.Arguments = new ExpressionCollection();
			indexerExpression.Arguments.Add(argument);
			indexerExpression.AddDetailNode(argument);
		}
		void AddArgumentsToIndexerExpression(IndexerExpression indexerExpression, ExpressionCollection arguments)
		{
			if (indexerExpression.Arguments == null)
				indexerExpression.Arguments = new ExpressionCollection();
			foreach (LanguageElement argument in arguments)
			{
				if (argument == null)
					continue;
				indexerExpression.Arguments.Add(argument);
				indexerExpression.AddDetailNode(argument);
			}
		}
		protected bool IsStatementTerminator()
		{
			return (la.Type == Tokens.LineTerminator
				|| la.Type == Tokens.EOF
				|| la.Type == Tokens.Colon);
		}
		protected Expression MethodReferenceToMethodCall(Expression mre, ExpressionCollection parameters)
		{
			return MethodReferenceToMethodCall(mre, parameters, false);
		}
		protected Expression MethodReferenceToMethodCall(Expression mre, ExpressionCollection parameters, bool isBaseConstructorCall)
		{
			if (mre == null)
				return null;
			MethodCallExpression result = null;
			string name = mre.Name;
			bool IsGetXml = name != null && AtLeastOrcas();
			if (IsGetXml)
			{
				name = name.ToLower();
				IsGetXml = name == "getxmlnamespace";
			}
			if (IsGetXml)
			{
				result = new GetXmlNamespaceOperator(mre);
				result.Name = "GetXmlNamespace";
			}
			else
			{
				result = new MethodCallExpression(mre);
				result.AddNode(mre);
			}
			result.IsBaseConstructorCall = isBaseConstructorCall;
			if (parameters != null)
			{
				result.Arguments = parameters;
				foreach (LanguageElement element in parameters)
					result.AddDetailNode(element);
			}
			return result;
		}
		bool IsSingleLineXmlComment(Comment comment)
		{
			return (comment is VBXmlDocComment && comment.CommentType == CommentType.SingleLine);
		}
		bool IsRemComment(Token token)
		{
			return IsSingleLineCommentName(token, "rem");
		}
		bool IsXmlComment(Token token)
		{
			return IsSingleLineCommentName(token, "'''");
		}
		bool IsSimpleSingleLineComment(Token token)
		{
			return IsSingleLineCommentName(token, "'");
		}
		bool IsSingleLineCommentName(Token token, string name)
		{
			if (name == null || name == String.Empty)
				return false;
			int length = name.Length;
			string commentName = GetSubStringInTokenValue(0, length, token);
			if (commentName == null)
				return false;
			if (commentName == name)
				return true;
			return false;
		}
		string GetSubStringInTokenValue(int startIndex, int lenght, Token token)
		{
			if (token == null || token.Value == null || token.Value.Length < lenght)
				return null;
			string @value = token.Value;
			string result = token.Value;
			result = result.Substring(0, 3);
			result = result.ToLower();
			return result;
		}
		protected void AddCommentNode(Token lCommentToken)
		{
			Comment lComment;
			CommentType lType = CommentType.SingleLine;
			string lValue = lCommentToken.Value;
			int StartIdx = 1;
			if (IsXmlComment(lCommentToken))
			{
				lComment = new VBXmlDocComment();
				StartIdx = 3;
			}
			else
			{
				if (IsRemComment(lCommentToken))
					StartIdx = 3;
				lComment = new Comment();
			}
			lComment.SetCommentType(lType);
			lComment.SetTextStartOffset(StartIdx);
			if (!(lComment is VBXmlDocComment))
			{
				if (lValue.Length > StartIdx)
					lValue = lValue.Substring(StartIdx);
				else
					lValue = "";
			}
			if (_PreviousTokenWasComment && IsSingleLineXmlComment(lComment))
			{
				VBXmlDocComment previousComment = Comments[Comments.Count - 1] as VBXmlDocComment;
				if (previousComment != null && IsSingleLineXmlComment(previousComment))
				{
					previousComment.InternalName += "\r\n";
					int spacesCount = 0;
					spacesCount = lCommentToken.Range.Start.Offset - 1;
					int newLinesCount = 0;
					newLinesCount = lCommentToken.Range.Start.Line - previousComment.Range.End.Line - 1;
					string appendHeightStr = new String('\n', newLinesCount);
					string appendStr = new String('\t', spacesCount);
					previousComment.InternalName += appendHeightStr;
					previousComment.InternalName += appendStr;
					previousComment.InternalName += lValue;
					previousComment.SetRange(GetRange(previousComment, lCommentToken));
					return;
				}
			}
			lComment.SetRange(lCommentToken.Range);
			lComment.StartPos = lCommentToken.StartPosition;
			lComment.EndPos = lCommentToken.EndPosition;
			lComment.InternalName = GlobalStringStorage.Intern(lValue);
			Comments.Add(lComment);
		}
		bool TokenIsComment(Token token)
		{
			if (token == null)
				return false;
			return token.Type == Tokens.SingleLineComment || token.Type == Tokens.SingleLineXmlComment;
		}
		public override ExpressionParserBase CreateExpressionParser()
		{
			return new VB90ExpressionParser(this);
		}
		protected override void CleanUpParser()
		{
			base.CleanUpParser();
			if (_Preprocessor != null)
			{
				_Preprocessor.CleanUp();
				_Preprocessor = null;
			}
			if (Comments != null)
				Comments.Clear();
		}
		#region AddToUsingList
		protected void AddToUsingList(string name, string aliasName, Expression exp)
		{
			if (exp == null)
				return;
			SourceFile sourceFile = RootNode as SourceFile;
			if (sourceFile == null)
				return;
			if (aliasName != null)
			{
				sourceFile.AliasList.Add(aliasName, name);
				if (exp != null)
				{
					Expression cloneAlias = exp.Clone() as Expression;
					if (!sourceFile.AliasHash.ContainsKey(aliasName))
						sourceFile.AliasHash.Add(aliasName, cloneAlias);
				}
			}
			else if (name != null && sourceFile.UsingList.IndexOfKey(name) < 0)
			{
				sourceFile.UsingList.Add(name, name);
			}
		}
		#endregion
		public override string Language
		{
			get
			{
				return "Basic";
			}
		}
		public override CommentCollection Comments
		{
			get
			{
				return _Comments;
			}
		}
		public VB90ParserErrors Errors
		{
			get
			{
				return (VB90ParserErrors)parserErrors;
			}
		}
		public bool ParseOnlyExpression
		{
			get
			{
				return _ParseOnlyExpression;
			}
			set
			{
				_ParseOnlyExpression = value;
			}
		}
		protected LocalVarArrayCollection LocalVarArrayCollection
		{
			get
			{
				if (_LocalVarArrayCollection == null)
					_LocalVarArrayCollection = new LocalVarArrayCollection();
				return _LocalVarArrayCollection;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual ParserVersion VsVersion
		{
			get
			{
				return _VsVersion;
			}
			set
			{
				_VsVersion = value;
			}
		}
		protected bool IsAssignmentOperator(Token token)
		{
			if (token == null)
				return false;
			int type = token.Type;
			switch (type)
			{
				case Tokens.EqualsSymbol:
				case Tokens.PlusEqual:
				case Tokens.MinusEqual:
				case Tokens.MulEqual:
				case Tokens.DivEqual:
				case Tokens.BackSlashEquals:
				case Tokens.XorEqual:
				case Tokens.ShiftLeftEqual:
				case Tokens.ShiftRightEqual:
				case Tokens.AndEqual:
					return true;
			}
			return false;
		}
		protected bool IsRelationalOp(Token token)
		{
			if (token == null)
				return false;
			switch (token.Type)
			{
				case Tokens.NotEquals:
				case Tokens.EqualsSymbol:
				case Tokens.LessThan:
				case Tokens.GreaterThan:
				case Tokens.LessOrEqual:
				case Tokens.GreaterOrEqual:
				case Tokens.Like:
				case Tokens.Is:
				case Tokens.IsNot:
					return true;
			}
			return false;
		}
		public override bool SupportsFileExtension(string ext)
		{
			return ext.ToLower() == ".vb";
		}
		public override IExpressionInverter CreateExpressionInverter()
		{
			switch (VsVersion)
			{
				case ParserVersion.VS2002:
				case ParserVersion.VS2003:
					return new VBOldExpressionInverter();
				case ParserVersion.VS2005:
				case ParserVersion.VS2008:
					return new VB90ExpressionInverter();
			}
			return new VB90ExpressionInverter();
		}
		protected bool IsQueryIdentForDeclaration(DeclaratorType type)
		{
			if (type == DeclaratorType.CanAggregateElement)
			{
				if (la.Type == Tokens.Group)
					return true;
			}
			ResetPeek();
			Token peekToken = Peek();
			return peekToken.Type == Tokens.EqualsSymbol;
		}
		protected Expression CreateAggregateExpressionIfNeeded(Expression exp, DeclaratorType declaratorType)
		{
			if (exp == null)
				return null;
			bool canHaveGroup = declaratorType == DeclaratorType.CanAggregateElement;
			if (declaratorType != DeclaratorType.CanAggregateFunction && !canHaveGroup)
				return exp;
			MethodCallExpression methodCall = exp as MethodCallExpression;
			if (methodCall != null)
			{
				string name = methodCall.Name;
				if (IsAggregateMethodName(name))
					return new AggregateMethodCallExpression(methodCall);
			}
			if (!canHaveGroup)
				return exp;
			ElementReferenceExpression elementRefence = exp as ElementReferenceExpression;
			if (elementRefence != null)
			{
				string name = elementRefence.Name;
				if (IsAggregateElementName(name))
					return new AggregateElementReferenceExpression(elementRefence);
			}
			return exp;
		}
		bool IsAggregateElementName(string name)
		{
			if (String.IsNullOrEmpty(name))
				return false;
			name = name.Trim();
			int length = name.Length;
			if (length > 2 && name[0] == '[' && name[length - 1] == ']')
				name = name.Substring(1, length - 2);
			if (String.Compare(name, "Group", StringComparison.CurrentCulture) == 0)
			{
				return true;
			}
			return false;
		}
		bool IsAggregateMethodName(string name)
		{
			if (String.IsNullOrEmpty(name))
				return false;
			name = name.Trim();
			int length = name.Length;
			if (length > 2 && name[0] == '[' && name[length - 1] == ']')
				name = name.Substring(1, length - 2);
			foreach (string aggregateMethodName in _AggregateMethodNames)
				if (String.Compare(name, aggregateMethodName, StringComparison.CurrentCultureIgnoreCase) == 0)
					return true;
			return false;
		}
		protected void AddEqualsExpression(JoinExpressionBase result, EqualsExpression exp)
		{
			if (result == null || exp == null)
				return;
			result.AddEqualsExpression(exp);
		}
		protected void SetGroupCollection(GroupByExpression result, LanguageElementCollection coll)
		{
			if (result == null || coll == null || coll.Count == 0)
				return;
			for (int i = 0; i < coll.Count; i++)
			{
				LanguageElement element = coll[i];
				if (element == null)
					continue;
				result.AddGroupElement(element);
			}
		}
		protected void SetIntoCollection(Expression result, LanguageElementCollection coll)
		{
			if (result == null || coll == null || coll.Count == 0)
				return;
			for (int i = 0; i < coll.Count; i++)
			{
				LanguageElement element = coll[i];
				if (element == null)
					continue;
				IIntoContainingElement into = result as IIntoContainingElement;
				if (into != null)
					into.AddIntoElement(element);
			}
		}
		protected void SetGroupByCollection(GroupByExpression result, LanguageElementCollection coll)
		{
			if (result == null || coll == null || coll.Count == 0)
				return;
			for (int i = 0; i < coll.Count; i++)
			{
				LanguageElement element = coll[i];
				if (element == null)
					continue;
				result.AddByElement(element);
			}
		}
		protected bool IsGroupJoinOperator()
		{
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken == null)
				return false;
			return peekToken.Type == Tokens.Join;
		}
		protected void SetSelectCollection(SelectExpression result, LanguageElementCollection coll)
		{
			if (result == null || coll == null || coll.Count == 0)
				return;
			for (int i = 0; i < coll.Count; i++)
			{
				LanguageElement element = coll[i];
				if (element == null)
					continue;
				result.AddReturnedElement(element);
			}
		}
		protected Expression GetInitializer(QueryIdent ident)
		{
			if (ident == null)
				return null;
			Expression exp = ident.Expression;
			if (exp == null)
				return null;
			SourceRange endRange = ident.NameRange;
			if (ident.MemberTypeReference != null)
			{
				endRange = ident.MemberTypeReference.Range;
			}
			ident.Expression = null;
			ident.DetailNodes.Remove(exp);
			ident.SetRange(new SourceRange(ident.Range.Start, endRange.End));
			return exp;
		}
		protected string GetTypeCharacter()
		{
			VB90Scanner vbScan = scanner as VB90Scanner;
			if (vbScan == null || !IsTypeCharacter())
				return String.Empty;
			Get();
			return tToken.Value;
		}
		bool IsTypeCharacter()
		{
			return IsRelationalTokens(tToken, la) &&
				(la.Type == Tokens.Sharp ||
				la.Type == Tokens.CommAtSymbol ||
				la.Type == Tokens.PercentSymbol ||
				la.Type == Tokens.BitAnd ||
				la.Type == Tokens.ExclamationSymbol ||
				la.Type == Tokens.DollarSybol) &&
				!IsIndexer();
		}
		bool IsIndexer()
		{
			ResetPeek();
			Token peekToken = Peek();
			if (IsIdentifierOrKeyword(peekToken.Type) && IsRelationalTokens(la, peekToken))
				return true;
			return false;
		}
		bool IsRelationalTokens(Token first, Token second)
		{
			if (first == null || second == null)
				return false;
			int tEndOffset = first.Range.End.Offset;
			int tEndLine = first.Range.End.Line;
			int laStartLine = second.Range.Start.Line;
			int laStartOffset = second.Range.Start.Offset;
			return tEndLine == laStartLine && laStartOffset == tEndOffset;
		}
		protected bool IsValidEndParsing()
		{
			if (la == null || la.Type == Tokens.EOF)
				return true;
			return false;
		}
		protected void RecoverParsing()
		{
			if (IsValidEndParsing())
				return;
			SkipTokenToBlockEnd();
			if (la == null || la.Type == Tokens.EOF)
				return;
			SetContext(RootNode);
			ParseRootRule();
		}
		protected void SetDefaultOptionStrict()
		{
			SourceFile sf = RootNode as SourceFile;
			if (sf == null)
				return;
			sf.SetOptionStrict(null);
		}
		protected void SetOptionStrict(OptionState state)
		{
			SourceFile sourceFile = RootNode as SourceFile;
			if (sourceFile == null)
				return;
			if (state == OptionState.On)
				sourceFile.OptionStrict = OptionStrict.On;
			else if (state == OptionState.Off)
				sourceFile.OptionStrict = OptionStrict.Off;
		}
	protected void SetDefaultOptionExplicit()
	{
	  SourceFile sf = RootNode as SourceFile;
	  if (sf == null)
		return;
	  sf.SetOptionExplicit(null);
	}
	protected void SetOptionExplicit(OptionState state)
	{
	  SourceFile sourceFile = RootNode as SourceFile;
	  if (sourceFile == null)
		return;
	  if (state == OptionState.On)
		sourceFile.OptionExplicit = OptionExplicit.On;
	  else if (state == OptionState.Off)
		sourceFile.OptionExplicit = OptionExplicit.Off;
	}
		protected void SetDefaultOptionInfer()
		{
			SourceFile sf = RootNode as SourceFile;
			if (sf == null)
				return;
			sf.SetOptionInfer(null);
		}
		protected void SetOptionInfer(OptionState state)
		{
			SourceFile sourceFile = RootNode as SourceFile;
			if (sourceFile == null)
				return;
			if (state == OptionState.On)
				sourceFile.OptionInfer = OptionInfer.On;
			else if (state == OptionState.Off)
				sourceFile.OptionInfer = OptionInfer.Off;
		}
		void LogSkipToken()
		{
			string tokenName = la.Value;
			if (tokenName == null)
			{
				tokenName = "NullNameToken";
			}
			Log.Send(String.Format("Token with name - {0} is skiped.", tokenName));
			Get();
		}
		void SkipTokenToBlockEnd()
		{
			while (IsSkipTokensEnd())
			{
				LogSkipToken();
			}
		}
		int[] _EndTokens = { Tokens.Class, Tokens.Structure, Tokens.Enum, Tokens.Module, Tokens.Sub, Tokens.Function };
		bool IsSkipTokensEnd()
		{
			if (la == null || la.Type == Tokens.EOF)
				return false;
			if (la.Type == Tokens.EndToken)
			{
				LogSkipToken();
				if (la == null)
				{
					return false;
				}
				for (int i = 0; i <= _EndTokens.Length; i++)
				{
					int endTokenType = _EndTokens[i];
					if (la.Type == endTokenType)
					{
						LogSkipToken();
						return false;
					}
				}
			}
			return true;
		}
		protected void SetParamProperties(Param param, VBDeclarator decl)
		{
			LanguageElementCollection arrayModifiers = null;
	  NullableTypeModifier nullableModifier = null;
			TypeReferenceExpression type = null;
			SetParamProperties(param, decl, out arrayModifiers, out nullableModifier, out type);
		}
		protected void SetParamProperties(Param param, VBDeclarator decl, out LanguageElementCollection arrayModifiers, out NullableTypeModifier nullableModifier,  out TypeReferenceExpression type)
		{
			arrayModifiers = null;
	  nullableModifier = null;
			type = null;
			if (param == null || decl == null)
				return;
			if (decl != null)
			{
				type = decl.CharacterType;
				arrayModifiers = decl.ArrayModifiers;
				nullableModifier = decl.NullableModifier;
			}
			if (nullableModifier != null)
			{
				param.AddDetailNode(nullableModifier);
				param.NullableModifier = nullableModifier;
			}
			if (arrayModifiers != null && arrayModifiers.Count != 0)
			{
				foreach (LanguageElement element in arrayModifiers)
				{
					param.AddDetailNode(element);
				}
			}
		}
		protected bool IsIfStatement()
		{
			if (la == null)
				return false;
			return la != null && la.Type == Tokens.IfToken;
		}
		protected ConditionalExpression CreateIfExpression(Expression cond, Expression first, Expression second)
		{
			ConditionalExpression result = null;
			if (second == null)
			{
				result = new IfOperatorExpression(cond, first);
			}
			else
			{
				result = new ConditionalExpression(cond, first, second);
			}
			return result;
		}
		protected Expression CreateConditionalTypeCast(Expression left, Expression right)
		{
			ConditionalTypeCast result = new ConditionalTypeCast(left, right);
			result.IsIfOperator = false;
			return result;
		}
		protected LanguageElementCollection CreateVarNextColl(Variable firstVar)
		{
			if (firstVar == null)
				return null;
			LanguageElementCollection coll = new LanguageElementCollection();
			Variable var = firstVar;
			while (var != null)
			{
				if (firstVar != var)
				{
					var.SetAncestorVariable(firstVar);
				}
				coll.Add(var);
				var = var.NextVariable;
			}
			return coll;
		}
		protected bool IsMethodReferenceExpression()
		{
			if (la.Type != Tokens.ParenOpen)
				return false;
			ResetPeek();
			Token peekToken = Peek();
			if (peekToken.Type != Tokens.Of)
				return true;
			int type = peekToken.Type;
			if (type == Tokens.EOF)
				return false;
			int count = 1;
			while (type != Tokens.LineTerminator && count != 0)
			{
				if (type == Tokens.ParenOpen)
				{
					count++;
				}
				else if (type == Tokens.ParenClose)
				{
					count--;
				}
				peekToken = Peek();
				type = peekToken.Type;
				if (type == Tokens.EOF)
					return false;
			}
			if (count != 0)
				return false;
			return type == Tokens.ParenOpen;
		}
		#region SetParensRanges
		protected void SetParensRanges(ParentingStatement target, SourceRange parenOpenRange, SourceRange parenCloseRange)
		{
			if (target == null)
				return;
			target.SetParensRange(GetRange(parenOpenRange, parenCloseRange));
		}
		#endregion
		void AddImplicitParamToAccessor(VBProperty member, Accessor accessor)
		{
			Set setter = accessor as Set;
			if (setter == null || member == null)
				return;
			LanguageElementCollection paramColl = setter.Parameters;
			if (paramColl != null && paramColl.Count > 0)
				return;
			string type = member.MemberType;
			TypeReferenceExpression typeReference = member.MemberTypeReference;
			Param param = new Param(type, "Value");
			param.IsImplicit = true;
			param.MemberTypeReference = typeReference;
			param.SetParent(accessor);
			accessor.ImplicitVariable = param;
		}
		protected Statement GetSimpleStatement(Expression leftPart, SourceRange startRange)
		{
			if (leftPart == null)
				return null;
			leftPart.IsStatement = true;
			Statement result = Statement.FromExpression(leftPart);
			result.SetRange(GetRange(startRange, tToken.Range));
			return result;
		}
		int[] _StartDeclTokens = {
			Tokens.Declare, Tokens.Function, Tokens.Sub, Tokens.Operator, Tokens.Ansi, Tokens.Auto,
Tokens.Unicode, Tokens.Assembly, Tokens.Module, Tokens.Const, Tokens.MustOverride,
Tokens.MustInherit, Tokens.Default, Tokens.Friend, Tokens.Shadows, Tokens.Overrides, Tokens.Private,
Tokens.Protected, Tokens.Public, Tokens.NotInheritable, Tokens.NotOverridable, Tokens.Shared,
Tokens.Overridable, Tokens.Overloads, Tokens.ReadOnly, Tokens.WriteOnly, Tokens.Widening,
Tokens.Narrowing, Tokens.Partial, Tokens.WithEvents, Tokens.Class, Tokens.Structure,Tokens.Interface,
Tokens.Module, Tokens.Enum, Tokens.Delegate, Tokens.Property, Tokens.Custom, Tokens.Event };
		int[] _EndDeclTokens = {Tokens.Namespace, Tokens.Class, Tokens.Structure, Tokens.Interface,
Tokens.Module, Tokens.Enum, Tokens.Function, Tokens.Sub, Tokens.Operator, Tokens.Property,
Tokens.Get, Tokens.Event, Tokens.AddHandler, Tokens.RemoveHandler, Tokens.RaiseEvent, Tokens.Set };
		bool IsEndDecl(int tokenType)
		{
			foreach (int type in _EndDeclTokens)
				if (type == tokenType)
					return true;
			return false;
		}
		bool IsStartNewDecl(int tokenType)
		{
			foreach (int type in _StartDeclTokens)
				if (type == tokenType)
					return true;
			return false;
		}
		protected void GetEndToken(bool skipToken)
		{
			if (skipToken)
				return;
			if (la.Type == Tokens.EndToken)
				Get();
		}
	private int _FirstTokenStartPosition;
	bool _ParseOnlyExpression;
	private bool _ParsingRazor;
	protected bool ParsingRazor
	{
	  get
	  {
	  	return _ParsingRazor;
	  }
	}
	private bool IsValidRazorStatementStart(Token la, int scannerStartPosition)
	{
	  if (la == null)
		return false;
	  switch (la.Type)
	  {
	  	case Tokens.IfToken:
		case Tokens.For:
		case Tokens.Do:
		case Tokens.While:
		case Tokens.Else:
		case Tokens.Using:
	  		return true;
		case Tokens.Identifier:
		  if (la.Value == "Code" && scannerStartPosition == _FirstTokenStartPosition)
			return true;
		  else
			return false;
	  }
	  return false;
	}
	public LanguageElementCollection ParseRazorFunctions(ISourceReader reader, out int scannerPositionDelta)
	{
	  scannerPositionDelta = 0;
	  return null;
	}
	private LanguageElementCollection ParseRazorStatement()
	{
	  LanguageElementCollection result = null;
	  Block rootNode = new Block();
	  OpenContext(rootNode);
	  IsRazorEndCode = false;
	  SourceRange endRange;
	  if (IsRazorCodeKeyword(la))
	  {
		Get();
		while (la.Type == Tokens.LineTerminator)
		  Get();
		BlockRule(SourceRange.Empty, BlockType.IfSingleLineStatement, true, out endRange);
	  }
	  else
	  {
		Statement statement = null;
		StatementRule(out statement);
	  }
	  if (rootNode.NodeCount <= 0)
		return result;
	  int nodeCount = rootNode.NodeCount;
	  result = new LanguageElementCollection();
	  for (int i = 0; i < nodeCount; i++)
	  {
		LanguageElement currentElement = rootNode.Nodes[i] as LanguageElement;
		if (currentElement != null)
		  result.Add(currentElement);
	  }
	  return result;
	}
	bool IsRazorMarkup()
	{
	  if (!ParsingRazor)
		return false;
	  if (la.Type != Tokens.CommAtSymbol)
		return false;
	  return true;
	}
	private void ParseRazorMarkup()
	{
	  HtmlParser htmlParser = new HtmlParser(true, false, DotNetLanguageType.VisualBasic);
	  ISourceReader htmlReader = GetRazorReader();
	  int scannerPositionDelta = -1;
	  LanguageElementCollection htmlParseResult = htmlParser.ParseSingleElement(htmlReader, out scannerPositionDelta);
	  if (htmlParseResult != null && Context != null)
		Context.AddNodes(htmlParseResult);
	  if (scanner.CurrentChar != ':' && scanner.CurrentChar != '*')
		scannerPositionDelta++;
	  scannerPositionDelta = la.StartPosition + scannerPositionDelta - scanner.Position + 1;
	  scanner.SyncPosition(scannerPositionDelta);
	}
	private ISourceReader GetRazorReader()
	{
	  ISourceReader scannerSourceReader = scanner.SourceReader;
	  System.IO.TextReader textReader = scannerSourceReader.GetStream();
	  string text = textReader.ReadToEnd();
	  int streamStartPosition = la.StartPosition - _FirstTokenStartPosition + 1;
	  int endColumn = la.EndColumn;
	  if (scanner.CurrentChar == ':' || scanner.CurrentChar == '*')
	  {
		streamStartPosition--;
		endColumn--;
	  }
	  return scannerSourceReader.GetSubStream(streamStartPosition, text.Length - streamStartPosition, la.EndLine, endColumn);
	}
	protected LanguageElementCollection ParseRazorExpression()
	{
	  LanguageElementCollection result = new LanguageElementCollection();
	  Expression expression = null;
	  PrimaryExpression(out expression);
	  result.Add(expression);
	  return result;
	}
	private bool IsRazorCodeKeyword(Token token)
	{
	  if (!ParsingRazor)
		return false;
	  if (token.Type != Tokens.Identifier)
		return false;
	  if (String.Compare(token.Value, "Code", StringComparison.CurrentCultureIgnoreCase) != 0)
		return false;
	  return true;
	}
	protected bool IsRazorEndCode;
	public LanguageElementCollection ParseRazorCode(ISourceReader reader, ref int scannerPositionDelta)
	{
	  try
	  {
		_ParsingRazor = true;
		scanner = new VB90Scanner(reader);
		int scannerStartPosition = scanner.Position - 1;
		int scannerEndPosition = scannerStartPosition;
		Comments.Clear();
		TextStrings.Clear();
		PreparePreprocessor();
		la = new Token();
		la.Value = "";
		Get();
		_FirstTokenStartPosition = la.StartPosition;
		LanguageElementCollection result = null;
		if (IsValidRazorStatementStart(la, scannerStartPosition))
		  result = ParseRazorStatement();
		else
		  result = ParseRazorExpression();
		scannerEndPosition = tToken.EndPosition;
		scannerPositionDelta = scannerEndPosition - scannerStartPosition;
		return result;
	  }
	  finally
	  {
		_ParsingRazor = false;
		IsRazorEndCode = false;
		CleanUpParser();
		if (reader != null)
		{
		  reader.Dispose();
		  reader = null;
		}
	  }
	}
	public LanguageElement ParseRazorHelper(ISourceReader reader, out int scannerPositionDelta)
	{
	  try
	  {
		_ParsingRazor = true;
		scanner = new VB90Scanner(reader);
		int scannerStartPosition = scanner.Position - 1;
		int scannerEndPosition = scannerStartPosition;
		Comments.Clear();
		TextStrings.Clear();
		PreparePreprocessor();
		la = new Token();
		la.Value = "";
		Get();
		_FirstTokenStartPosition = la.StartPosition;
		LanguageElement result = ParseRazorHelper();
		scannerEndPosition = tToken.EndPosition;
		scannerPositionDelta = scannerEndPosition - scannerStartPosition;
		return result;
	  }
	  finally
	  {
		_ParsingRazor = false;
		IsRazorEndCode = false;
		CleanUpParser();
		if (reader != null)
		{
		  reader.Dispose();
		  reader = null;
		}
	  }
	}
	LanguageElement ParseRazorHelper()
	{
	  Token startBlockToken = la;
	  SubMemberData subMember = null;
	  MethodTypeEnum methodTypeEnum = MethodTypeEnum.Function;
	  VBDeclarator decl = null;
	  MethodSignature(out subMember, out decl);
	  if (subMember == null)
		return null;
	  Method result = CreateMethod(subMember, methodTypeEnum, null, decl, SourceRange.Empty);
	  StatementTerminator();
	  OpenContext(result);
	  Block(startBlockToken);
	  CloseContext();
	  if (la.Type == Tokens.EndToken)
		Get();
	  if (la.Type == Tokens.Identifier && String.Compare("helper", la.Value, StringComparison.CurrentCultureIgnoreCase) == 0)
		Get();
	  result.SetRange(GetRange(startBlockToken, tToken));
	  return result;
	}
	protected override FormattingTable FormattingTable
	{
	  get { return VBFormattingTable.Instance; }
	}
	}
}
