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
using System.Collections.Generic;
using System.Collections.Specialized;
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
  using Preprocessor;
  using System.IO;
  using Html;
  public enum RazorParsingMode
  {
	None,
	Statement,
	Expression,
	Helper,
	Functions
  }
  public partial class CSharp30Parser : FormattingParserBase, IRazorCodeParser
  {
	private const string STR_Cs = ".cs";
	const int INT_MaxNestingLevel = 400;
	ArrayList ccs = new ArrayList();
	bool IsAsyncContext;
	bool _WorkAsCharp30Parser = false;
	CommentCollection _Comments = new CommentCollection();
	Stack _Regions;
	bool _PreviousTokenWasComment = false;
	bool _ShouldWorkAsExpressionParser = false;
	LanguageElement _UnclosedMemberContext = null;
	int _StatementNestingLevel = 0;
	private int _FirstTokenStartPosition;
	ISourceReader _Reader = null;
	bool _ParsingPostponedTokens = false;
	Hashtable _PrimitiveToFullTypes;
	QueryExpression _ActiveSqlExpression;
	CSharpPreprocessor _Preprocessor;
	bool _ParsingRazor;
	RazorParsingMode _RazorParsingMode;
			void CallAppropriateParseRule(LanguageElement context)
		{
			if (context == null)
				return;
	  if (context.ElementType == LanguageElementType.Method)
	  {
		Method method = (Method)context;
		IsAsyncContext = method.IsAsynchronous;
	  }
			try
			{
				_ParsingPostponedTokens = context.ParsingPostponedTokens;
				switch(context.ElementType)
				{
					case LanguageElementType.SourceFile:
							Parser();
							Expect(0);
							SetContextEndIfNeeded();
							break;
					case LanguageElementType.Namespace:
					case LanguageElementType.TemplateModifier:
					case LanguageElementType.ExternDeclaration:
			  while (la.Type == Tokens.USINGKW)
				UsingDirective();
							NamespaceMemberDeclarations();
							ParseTypeEnd();
							break;
					case LanguageElementType.Case:
					case LanguageElementType.If:
					case LanguageElementType.Else:
					case LanguageElementType.Switch:
					case LanguageElementType.While:
					case LanguageElementType.Do:
					case LanguageElementType.For:
					case LanguageElementType.ForEach:
					case LanguageElementType.Method:
					case LanguageElementType.Try:
					case LanguageElementType.Lock:
					case LanguageElementType.Catch:
					case LanguageElementType.Finally:
					case LanguageElementType.EventAdd:
					case LanguageElementType.EventRaise:
					case LanguageElementType.EventRemove:
					case LanguageElementType.PropertyAccessorGet:
					case LanguageElementType.PropertyAccessorSet:
							StatementSeq();
							ParseMemberEnd(context.ElementType);
							break;
					case LanguageElementType.Event:
							EventAccessorDeclarationsSeq();
							ParseMemberEnd(context.ElementType);
							break;
					case LanguageElementType.Property:
							AccessorDeclarationsSeq();
							ParseMemberEnd(context.ElementType);
							break;
					case LanguageElementType.Class:
					case LanguageElementType.Interface:
					case LanguageElementType.Union:
					case LanguageElementType.Struct:
							NamespaceMemberDeclaration();
							ParseTypeEnd();
							break;
		  case LanguageElementType.Enum:
						while (la != null && la.Type == Tokens.COMMA)
						  Get();
						if (la.Type != Tokens.EOF)
							  EnumMembers();
			  ParseTypeEnd();
							break;
				}
			}
			finally
			{
				_ParsingPostponedTokens = false;
			}
		}
		protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
		{
			try
			{
		SaveFormat = parserContext.SaveUserFormat;
				LanguageElement context = parserContext.Context;
				if (context == null)
					return null;
				SetRootNode(context);
				if (context is SourceFile)
				{
					((SourceFile)context).SetDocument(parserContext.Document);
					((SourceFile)context).TextStrings = TextStrings;
					((SourceFile)context).ClearFriendAssemblyNames();
					TextStrings.Clear();
					Comments.Clear();
					Regions.Clear();
				}
				_Reader = reader;
		 scanner = CreateScanner(reader, parserContext.ScannerExtension);
				PreparePreprocessor();
		FirstGet();
				CallAppropriateParseRule(context);
				if (parserContext != null && Comments != null)
					parserContext.Comments.AddRange(Comments);
				if (parserContext != null)
					parserContext.ContextAfterParse = Context;
				BindComments();
				CheckForErrors(RootNode);
				if(context is SourceFile)
		  LastParsingHasEOF = (tToken != null && tToken.Type == 0);
				else
				  LastParsingHasEOF = (la != null && la.Type == 0);
				AfterParsing();
				return RootNode;
			}
			finally
			{
				_Reader = null;
		parserContext.Errors = parserErrors.GetClonedErrorList();
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		public LanguageElement Parse(ISourceReader reader)
		{
			try
			{
				_Reader = reader;
				scanner = new CSharpScanner(reader);
				if (RootNode == null)
		  OpenContext(GetSourceFile("SourceFile"));
				Parse();
				CheckForErrors(RootNode);
				AfterParsing();
				return RootNode;
			}
			finally
			{
				_Reader = null;
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		public Expression ParseExpression(ISourceReader reader)
		{
			try
			{
		_Reader = reader;
		scanner = CreateScanner(reader);
		if (RootNode == null)
		  OpenContext(GetSourceFile("SourceFile"));
				Comments.Clear();
				TextStrings.Clear();
				Regions.Clear();
				PreparePreprocessor();
				la = new Token();
				la.Value = "";
				Get();
				Expression result = null;
				VariableInitializer(out result);
				AfterParsing(result);
		ClearFormattingParsingElements();
				return result;
			}
			finally
			{
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		public LanguageElementCollection ParseFormalParameters(ISourceReader reader)
		{
			try
			{
		scanner = CreateScanner(reader);
				Comments.Clear();
				TextStrings.Clear();
				Regions.Clear();
				PreparePreprocessor();
				la = new Token();
				la.Value = "";
				Get();
				Hashtable parameterAttributes = null;
				LanguageElementCollection parameters = null;
				FormalParameterList(out parameters, out parameterAttributes);
				AfterParsing(parameters);
				return parameters;
			}
			finally
			{
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		public ExpressionCollection ParseMethodCallParameters(ISourceReader reader)
		{
			try
			{
		scanner = CreateScanner(reader);
				Comments.Clear();
				TextStrings.Clear();
				Regions.Clear();
				PreparePreprocessor();
				la = new Token();
				la.Value = "";
				Get();
				ExpressionCollection parameters = null;
				ArgumentCollection(out parameters);
				AfterParsing(parameters);
				return parameters;
			}
			finally
			{
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
	GeneratedScannerBase CreateScanner(ISourceReader s, ScannerExtension extension)
	{
	  CSharpScanner cSharpScanner = new CSharpScanner(s, extension);
	  cSharpScanner.SaveFormat = SaveFormat;
	  return cSharpScanner;
	}
	GeneratedScannerBase CreateScanner(ISourceReader s)
	{
	  return CreateScanner(s, null);
	}
	public CSharp30Parser()
	{
	  parserErrors = new CSharpErrors();
	  set = CreateSetArray();
	  maxTokens = Tokens.MaxTokens;
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
	}
		public LanguageElementCollection ParseStatements(ISourceReader reader)
		{
			try
			{
		scanner = CreateScanner(reader);
				Comments.Clear();
				LanguageElementCollection result = new LanguageElementCollection();
				Method rootNode = new Method("root");
				TextStrings.Clear();
				Regions.Clear();
				PreparePreprocessor();
				la = new Token();
				la.Value = "";
				Get();
				OpenContext(rootNode);
				StatementSeq();
				for (int i = 0; i < rootNode.NodeCount; i++)
					result.Add(rootNode.Nodes[i]);
				AfterParsing(result);
				return result;
			}
			finally
			{
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
		public TypeReferenceExpression ParseTypeReferenceExpression(ISourceReader reader)
		{
			try
			{
		scanner = CreateScanner(reader);
				Comments.Clear();
				TextStrings.Clear();
				Regions.Clear();
				PreparePreprocessor();
				la = new Token();
				la.Value = "";
				Get();
				TypeReferenceExpression result = null;
				Type(out result, false);
				AfterParsing(result);
		ClearFormattingParsingElements();
				return result;
			}
			finally
			{
				CleanUpParser();
				if (reader != null)
				{
					reader.Dispose();
					reader = null;
				}
			}
		}
	protected LanguageElementCollection ParseRazorExpression()
	{
	  LanguageElementCollection result = new LanguageElementCollection();
	  Expression expression = null;
	  _RazorParsingMode = RazorParsingMode.Expression;
	  Primary(out expression, UnaryOperatorType.None);
	  if (expression != null)
		result.Add(expression);
	  return result;
	}
	private ISourceReader GetRazorReader()
	{
	  ISourceReader scannerSourceReader = scanner.SourceReader;
	  TextReader textReader = scannerSourceReader.GetStream();
	  string text = textReader.ReadToEnd();
	  int streamStartPosition = la.StartPosition - _FirstTokenStartPosition;
	  return scannerSourceReader.GetSubStream(streamStartPosition, text.Length - streamStartPosition, la.Line, la.Column);
	}
	private void SetHasEndingSemicolonIfNeeded(LanguageElement namespaceOrType)
	{
	  switch(namespaceOrType.ElementType)
	  {
		case LanguageElementType.Namespace:
		  ((Namespace)namespaceOrType).HasEndingSemicolon = true;
		  break;
		case LanguageElementType.Class:
		case LanguageElementType.Struct:
		case LanguageElementType.Interface:
		case LanguageElementType.Enum:
		  ((TypeDeclaration)namespaceOrType).HasEndingSemicolon = true;
		  break;
	  }
	}
	protected void ParseRazorHtmlEmbedding(out LanguageElementCollection htmlParseResult)
	{
	  HtmlParser htmlParser = new HtmlParser(true, false, DotNetLanguageType.CSharp);
	  bool setTokensCategory = SetTokensCategory;
	  htmlParser.SetTokensCategory = setTokensCategory;
	  ISourceReader htmlReader = GetRazorReader();
	  int scannerPositionDelta = -1;
	  htmlParseResult = htmlParser.ParseSingleElement(htmlReader, out scannerPositionDelta);
	  SourceRange resultRange = SourceRange.Empty;
	  if (htmlParseResult != null && htmlParseResult.Count > 0 && Context != null)
	  {
		Context.AddNodes(htmlParseResult);
		resultRange = new SourceRange(htmlParseResult[0].Range.Start, htmlParseResult[htmlParseResult.Count - 1].Range.End);
	  }
	  if (SetTokensCategory)
		foreach (CategorizedToken token in htmlParser.SavedTokens)
		  if (resultRange.Contains(token.Range))
			SavedTokens.Add(token);
	  la.Next = null;
	  scannerPositionDelta = la.StartPosition + scannerPositionDelta - (scanner.Position - 1);
	  if (scannerPositionDelta < 0  && la != scanner.tToken)
		la.Next = scanner.tToken;
	  scanner.SyncPosition(scannerPositionDelta);
	}
	bool IsNamespaceReference(Token token)
	{
	  if (token.Type != Tokens.USINGKW)
		return false;
	  ResetPeek();
	  Token nextToken = Peek();
	  return nextToken != null && nextToken.Type != Tokens.LPAR;
	}
	bool IsAwaitExpression()
	{
	  bool laTokenIsAwait = la.Type == Tokens.IDENT && la.Value == "await";
	  if (IsAsyncContext && laTokenIsAwait)
		return true;
	  if (laTokenIsAwait)
	  {
		ResetPeek();
		Token nextToken = Peek();
		if (nextToken.Type == Tokens.IDENT)
		  return true;
	  }
	  return false;
	}
	bool IsAsyncModifier()
	{
	  if (la.Type != Tokens.IDENT || la.Value != "async")
		return false;
	  ResetPeek();
	  Token nextToken = Peek();
	  switch (nextToken.Type)
	  {
		case Tokens.NEW:
		case Tokens.PUBLIC:
		case Tokens.PROTECTED:
		case Tokens.INTERNAL:
		case Tokens.PRIVATE:
		case Tokens.UNSAFE:
		case Tokens.STATIC:
		case Tokens.READONLY:
		case Tokens.VOLATILE:
		case Tokens.VIRTUAL:
		case Tokens.SEALED:
		case Tokens.OVERRIDE:
		case Tokens.ABSTRACT:
		case Tokens.EXTERN:
		case Tokens.VOID:
		  return true;
		case Tokens.IDENT:
		  if(IsPartialModifier(nextToken))
			return true;
		  break;
	  }
	  if (!IsType(ref nextToken))
		return false;
	  if (nextToken.Type == Tokens.LPAR)
		return false;
	  return true;
	}
	bool IsAsyncDelegate()
	{
	  if (la.Type != Tokens.IDENT || la.Value != "async")
		return false;
	  ResetPeek();
	  Token nextToken = Peek();
	  return nextToken != null && nextToken.Type == Tokens.DELEGATE;
	}
	public LanguageElementCollection ParseRazorCode(ISourceReader reader, ref int scannerPositionDelta)
	{
	  try
	  {
		_ParsingRazor = true;
		scanner = CreateScanner(reader);
		int scannerStartPosition = scanner.Position;
		int scannerEndPosition = scannerStartPosition;
		Comments.Clear();
		TextStrings.Clear();
		Regions.Clear();
		PreparePreprocessor();
		la = new Token();
		la.Value = "";
		Get();
		_FirstTokenStartPosition = la.StartPosition;
		LanguageElementCollection result = null;
		if (IsNamespaceReference(la))
		  result = ParseNamespaceReference();
		else
		{
		  if (IsValidRazorStatementStart(la))
			result = ParseRazorStatement();
		  else
			result = ParseRazorExpression();
		}
		scannerEndPosition = tToken.EndPosition + 1;
		scannerPositionDelta += scannerEndPosition - scannerStartPosition;
		return result;
	  }
	  finally
	  {
		_ParsingRazor = false;
		_RazorParsingMode = RazorParsingMode.None;
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
		_RazorParsingMode = RazorParsingMode.Helper;
		scanner = CreateScanner(reader);
		int scannerStartPosition = scanner.Position;
		int scannerEndPosition = scannerStartPosition;
		Comments.Clear();
		TextStrings.Clear();
		Regions.Clear();
		PreparePreprocessor();
		la = new Token();
		la.Value = "";
		Get();
		SourceRange laRange = la.Range;
		_FirstTokenStartPosition = la.StartPosition;
		LanguageElement result = ParseRazorHelper();
		if (result != null)
		  result.SetRange(GetRange(laRange, result));
		scannerEndPosition = tToken.EndPosition + 1;
		scannerPositionDelta = scannerEndPosition - scannerStartPosition;
		return result;
	  }
	  finally
	  {
		_ParsingRazor = false;
		_RazorParsingMode = RazorParsingMode.None;
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
	  ElementReferenceExpression helperName = null;
	  TypeReferenceExpression fictiveType = new TypeReferenceExpression("string");
	  Class rootNode = new Class();
	  OpenContext(rootNode);
	  MemberName(out helperName);
	  MethodDeclaration(fictiveType, helperName, null, MemberVisibility.Public, null);
	  if (rootNode != null && rootNode.NodeCount > 0)
		return rootNode.Nodes[0] as LanguageElement;
	  else
		return null;
	}
	public LanguageElementCollection ParseRazorFunctions(ISourceReader reader, out int scannerPositionDelta)
	{
	  scannerPositionDelta = 0;
	  LanguageElementCollection result = null;
	  try
	  {
		_ParsingRazor = true;
		_RazorParsingMode = RazorParsingMode.Functions;
		scanner = CreateScanner(reader);
		int scannerStartPosition = scanner.Position;
		int scannerEndPosition = scannerStartPosition;
		Comments.Clear();
		TextStrings.Clear();
		Regions.Clear();
		PreparePreprocessor();
		la = new Token();
		la.Value = "";
		Get();
		SourceRange laRange = la.Range;
		_FirstTokenStartPosition = la.StartPosition;
		result = ParseRazorFunctions();
		scannerEndPosition = tToken.EndPosition + 1;
		scannerPositionDelta = scannerEndPosition - scannerStartPosition;
		return result;
	  }
	  finally
	  {
		_ParsingRazor = false;
		_RazorParsingMode = RazorParsingMode.None;
		CleanUpParser();
		if (reader != null)
		{
		  reader.Dispose();
		  reader = null;
		}
	  }
	}
	LanguageElementCollection ParseRazorFunctions()
	{
	  LanguageElementCollection result = new LanguageElementCollection();
	  Class classContext = new Class();
	  OpenContext(classContext);
	  ClassBody(classContext);
	  for (int i = 0; i < classContext.NodeCount; i++)
		result.Add(classContext.Nodes[i] as LanguageElement);
	  if (SetTokensCategory)
	  {
		SourceRange range = classContext.BlockRange;
		for (int i = SavedTokens.Count - 1; i > -1; i--)
		  if (!range.Contains(SavedTokens[i].Range))
			SavedTokens.RemoveAt(i);
		  else
			break;
		if (SavedTokens.Count > 0)
		{
		  CategorizedToken catToken = SavedTokens[0] as CategorizedToken;
		  if (catToken.Type == Tokens.LBRACE)
			catToken.Category = TokenCategory.HtmlServerSideScript;
		  catToken = SavedTokens[SavedTokens.Count - 1] as CategorizedToken;
		  if (catToken.Type == Tokens.RBRACE)
			catToken.Category = TokenCategory.HtmlServerSideScript;
		}
	  }
	  return result;
	}
	protected bool IsRazorInlineExpression()
	{
	  if (!_ParsingRazor)
		return false;
	  if (la.Type != Tokens.AT)
		return false;
	  ResetPeek();
	  Token nextToken = Peek();
	  if (nextToken == null || nextToken.Type != Tokens.LT)
		return false;
	  return true;
	}
	protected bool IsRazorHtmlCode()
	{
	  if (!_ParsingRazor)
		return false;
	  if (la.Type == Tokens.LT)
		return true;
	  if (la.Type == Tokens.AT)
	  {
		ResetPeek();
		Token nextToken = Peek();
		if (nextToken != null && nextToken.Type != Tokens.LT)
		return true;
	  }
	  if (la.Type == Tokens.ATCOLON)
		return true;
	  if (la.Type == Tokens.IDENT && la.Value.StartsWith("@"))
		return true;
	  return false;
	}
	protected LanguageElementCollection ParseRazorStatement()
	{
	  LanguageElementCollection result = null;
	  Block rootNode = new Block();
	  OpenContext(rootNode);
	  _RazorParsingMode = RazorParsingMode.Statement;
	  Statement();
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
	protected LanguageElementCollection ParseNamespaceReference()
	{
	  LanguageElementCollection result = null;
	  SourceFile rootNode = new SourceFile();
	  OpenContext(rootNode);
	  UsingDirective();
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
	protected bool IsValidRazorStatementStart(Token token)
	{
	  switch (token.Type)
	{
		case Tokens.LBRACE:
		case Tokens.FOR:
		case Tokens.FOREACH:
		case Tokens.DO:
		case Tokens.WHILE:
		case Tokens.ELSE:
		case Tokens.USINGKW:
		case Tokens.IFCLAUSE:
		case Tokens.SWITCH:
		  return true;
	  }
	  return false;
	}
	protected Token CreatePostponedToken(Token left, Token right)
	{
	  Token newToken = left;
	  newToken.StartPosition = left.StartPosition + 1;
	  newToken.Column = left.Column + 1;
	  newToken.Line = left.Line;
	  newToken.EndPosition = right.EndPosition - 1;
	  newToken.EndColumn = right.EndColumn - 1;
	  newToken.EndLine = right.EndLine;
	  return newToken;
	}
	protected void CorrectFormattingTokenType(Token token)
	{
	  SetKeywordTokenCategory(token);
	  if (LastParsingElement == null)
		return;
	  FormattingTokenType setType = FormattingTokenType.None;
	  if (token.Type == Tokens.IDENT)
		switch (token.Value)
		{
		  case "var":
			setType = FormattingTokenType.Var;
			break;
		  case "alias":
			setType  = FormattingTokenType.Alias;
			break;
		  case "partial":
			setType  = FormattingTokenType.Partial;
			break;
		  case "async":
			setType  = FormattingTokenType.Async;
			break;
		  case "await":
			setType  = FormattingTokenType.Await;
			break;
		}
	  LastParsingElement = ReplaceOrAddFormattingParsingElement(token as FormattingToken, setType);
	}
	void ParseMemberEnd(LanguageElementType elementType)
	{
	  LanguageElement oldContext = Context;
	  if (elementType == LanguageElementType.Method
		|| elementType == LanguageElementType.Property
		|| elementType == LanguageElementType.Event)
		ParseMethodEnd();
	  if (oldContext != Context)
		CallAppropriateParseRule(Context);
	}
	void ParseMethodEnd()
	{
	  if (la == null || la.Type != Tokens.RBRACE || Context == null)
		return;
	  Get();
	  ReadBlockEnd(tToken.Range);
	  Context.SetRange(GetRange(Context, tToken));
	  CloseContext();
	}
	#region SkipMethodBody
	protected void SkipMethodBody()
	{
	  int lCount = 0;
	  while (true)
	  {
		if (la.Type == Tokens.EOF)
		  return;
		if (la.Type == Tokens.LBRACE)
		  lCount++;
		if (la.Type == Tokens.RBRACE)
		{
		  if (lCount == 0)
			return;
		  lCount--;
		}
		Get();
	  }
	}
	#endregion
	#region SkipTo
	protected void SkipTo(int tokenType)
	{
	  while (la != null && la.Type != Tokens.EOF && tokenType != la.Type)
	  {
		Get();
	  }
	}
	#endregion
	#region IsAccessorDeclaration
	protected bool IsAccessorDeclaration()
	{
	  if (la == null || la.Type != Tokens.IDENT || !(la.Value == "get" || la.Value == "set"))
		return false;
	  ResetPeek();
	  Token next = Peek();
	  if (next.Type == Tokens.SCOLON || next.Type == Tokens.LBRACE)
		return true;
	  else
		return false;
	}
	#endregion
	#region GetTokenCategory
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  return CSharpTokensHelper.GetUncategorizedTokenCategory(token);
	}
	#endregion
	#region ConvertToJoinIntoExpression
	protected JoinExpressionBase ConvertToJoinIntoExpression(JoinExpressionBase joinBase, QueryIdent intoName)
	{
	  if (joinBase == null || intoName == null)
		return joinBase;
	  JoinIntoExpression result = new JoinIntoExpression();
	  result.SetRange(joinBase.Range);
	  result.SetInExpression(joinBase.InExpression as InExpression);
	  for (int i = 0; i < joinBase.EqualsExpressions.Count; i++)
		result.AddEqualsExpression(joinBase.EqualsExpressions[i] as EqualsExpression);
	  result.AddIntoElement(intoName);
	  return result;
	}
	#endregion
	#region CreateQueryIdent
	protected QueryIdent CreateQueryIdent(Token nameToken, SourceRange operatorRange, Expression initializer)
	{
	  if (nameToken == null || initializer == null)
		return null;
	  QueryIdent result = new QueryIdent(nameToken.Value);
	  result.NameRange = nameToken.Range;
	  result.OperatorRange = operatorRange;
	  result.Expression = initializer;
	  result.AddDetailNode(initializer);
	  result.SetRange(GetRange(nameToken, initializer));
	  return result;
	}
	#endregion
	#region CleanUpParser
	protected override void CleanUpParser()
	{
	  _UnclosedMemberContext = null;
	  _StatementNestingLevel = 0;
	  base.CleanUpParser();
	  if (Comments != null)
		Comments.Clear();
	  if (_Preprocessor != null)
		_Preprocessor.CleanUp();
	  _ActiveSqlExpression = null;
	}
	#endregion
	#region TokenIsComment
	bool TokenIsComment(int tokenType)
	{
	  return tokenType == Tokens.SINGLELINECOMMENT || tokenType == Tokens.SINGLELINEXML
		  || tokenType == Tokens.MULTILINECOMMENT || tokenType == Tokens.MULTILINEXML
		  || tokenType == Tokens.RAZORCOMMENT ;
	}
	#endregion
	#region GetFileNode
	private SourceFile GetFileNode()
	{
	  if (Context == null)
		return null;
	  SourceFile rootNode = null;
	  if (Context is SourceFile)
		rootNode = Context as SourceFile;
	  else
		rootNode = Context.FileNode;
	  return rootNode;
	}
	#endregion
	#region CalculateRegionNameRange
	private SourceRange CalculateRegionNameRange(int startLine, int tokenOffset, string regionName, string tokenValue)
	{
	  int line = startLine;
	  int startOffset = tokenValue.IndexOf(regionName) + tokenOffset;
	  string trimmedName = SaveFormat ? regionName : regionName.Trim();
	  return new SourceRange(line, startOffset, line, trimmedName.Length + startOffset);
	}
	#endregion
	#region TokenIsSupportElement
	bool TokenIsSupportElement(int tokenType)
	{
	  return tokenType == Tokens.DEFINE || tokenType == Tokens.UNDEF
		  || tokenType == Tokens.IFDIR || tokenType == Tokens.ELIF
		  || tokenType == Tokens.ELSEDIR || tokenType == Tokens.ENDIF
		  || tokenType == Tokens.LINE || tokenType == Tokens.ERROR
		  || tokenType == Tokens.WARNING || tokenType == Tokens.REGION
		  || tokenType == Tokens.ENDREG || tokenType == Tokens.PRAGMADIR;
	}
	#endregion
	#region IsConditionalExpressionCore
	bool IsConditionalExpressionCore()
	{
	  Token nextToken = Peek();
	  int prevTokenType = -1;
	  int parenCount = 0;
	  int brackCount = 0;
	  int braceCount = 0;
	  int nextTokenType = nextToken.Type;
			if (nextTokenType == Tokens.GT || nextTokenType == Tokens.COMMA)
				return false;
	  while (nextToken.Type != Tokens.EOF && nextToken.Type != Tokens.SCOLON)
	  {
		switch (nextToken.Type)
		{
		  case Tokens.NEW:
			nextToken = Peek();
			IsType(ref nextToken);
			continue;
		  case Tokens.LBRACK:
			brackCount++;
			break;
		  case Tokens.RBRACK:
			brackCount--;
			if (brackCount < 0)
			  return false;
			break;
		  case Tokens.IDENT:
			if (string.Compare("where", nextToken.Value, StringComparison.CurrentCulture) == 0)
			  return false;
			break;
		  case Tokens.QUESTION:
			if (parenCount == 0)
			  return false;
			break;
		  case Tokens.ASSGN:
			return false;
		  case Tokens.LBRACE:
			if (prevTokenType == Tokens.IDENT)
			  return false;
			braceCount++;
			break;
		  case Tokens.RBRACE:
			braceCount--;
			if (braceCount < 0)
			  return false;
			break;
		  case Tokens.COLON:
			return true;
		  case Tokens.LPAR:
			parenCount++;
			break;
		  case Tokens.RPAR:
			parenCount--;
			if (parenCount < 0)
			  return false;
			break;
		}
		prevTokenType = nextToken.Type;
		nextToken = Peek();
	  }
	  return false;
	}
	#endregion
	#region IsConditionalExpressionStart
	protected bool IsConditionalExpressionStart()
	{
	  if (la == null || la.Type != Tokens.QUESTION)
		return false;
	  ResetPeek();
	  return IsConditionalExpressionCore();
	}
	#endregion
	#region AddToSqlExpression
	protected void AddToSqlExpression(Expression expression)
	{
	  if (expression == null || _ActiveSqlExpression == null)
		return;
	  _ActiveSqlExpression.AddDetailNode(expression);
	}
	#endregion
	#region SetAttributesForParameters
	protected void SetAttributesForParameters(LanguageElementCollection parameters, Hashtable parameterAttributes)
	{
	  if (parameters == null || parameters.Count == 0 || parameterAttributes == null || parameterAttributes.Count == 0)
		return;
	  for (int i = 0; i < parameters.Count; i++)
	  {
		Param currentParameter = parameters[i] as Param;
		if (currentParameter == null || !parameterAttributes.Contains(currentParameter))
		  continue;
		LanguageElementCollection attributes = parameterAttributes[currentParameter] as LanguageElementCollection;
		if (attributes != null)
		  currentParameter.SetAttributes(attributes);
	  }
	}
	#endregion
	#region AddImplicitParamToAccessor
	protected void AddImplicitParamToAccessor(Member member, Accessor accessor)
	{
	  if (accessor == null || member == null)
		return;
	  string type = member.MemberType;
	  TypeReferenceExpression typeReference = member.MemberTypeReference;
	  Param param = new Param(type, "value");
	  param.IsImplicit = true;
	  param.MemberTypeReference = typeReference;
	  param.SetParent(accessor);
	  accessor.ImplicitVariable = param;
	}
	#endregion
	#region GetSourceFile
	public override SourceFile GetSourceFile(string fileName)
	{
	  return new CSharpSourceFile(fileName);
	}
	#endregion
	#region ProcessSupportElement
	FormattingElements CutElementsFromEOL(FormattingElements elements)
	{
	  if(elements == null)
		return null;
	  FormattingElements elementsAfter = new FormattingElements();
	  int indexEOL = 0;
	  for (; indexEOL < elements.Count; indexEOL++)
	  {
		FormattingElement fe = elements[indexEOL] as FormattingElement;
		if(fe != null && fe.Type == FormattingElementType.EOL)
		  break;
	  }
	  while(indexEOL < elements.Count)
	  {
		elementsAfter.Add(elements[indexEOL]);
		elements.RemoveAt(indexEOL);
	  }
	  return elementsAfter;
	}
	string ProcessDirective(PreprocessorDirective directive, FormattingToken formattingToken, SourceFile sourceFile, bool canHaveComment, out FormattingElements elementsAfter)
	{
	  elementsAfter = null;
	  AddFormattingElements(formattingToken as FormattingToken);
	  formattingToken.WasChecked = true;
	  int startLine = formattingToken.Line;
	  int startOffset = formattingToken.Column;
	  string text = "";
	  bool saveFormat = scanner.SaveFormat;
	  if (!saveFormat)
		scanner.GetFormatingTokenElement();
	  scanner.SaveFormat = true;
	  FormattingElements fElements = formattingToken.FormattingElements;
	  if (scanner.EndLineValue > formattingToken.EndLine)
		elementsAfter = CutElementsFromEOL(fElements);
	  while(formattingToken != null && scanner.EndLineValue <= formattingToken.EndLine)
	  {
		Token token = scanner.Scan();
		if(token.Type == Tokens.EOF)
		  break;
		SaveCategorizedTokenIfNeeded(token);
		text += token.Value;
		formattingToken = token as FormattingToken;
		if(formattingToken == null)
		  continue;
		formattingToken.WasChecked = true;
		fElements = formattingToken.FormattingElements;
		elementsAfter = fElements;
		if(scanner.EndLineValue > formattingToken.EndLine)
		{
		  if(canHaveComment)
			break;
		  elementsAfter = CutElementsFromEOL(fElements);
		}
		if (canHaveComment && scanner.CurrentChar == '/')
		  break;
		text += _Preprocessor.GetFEText(fElements);
	  }
	  scanner.SaveFormat = saveFormat;
	  directive.SetRange(startLine, startOffset, startLine, formattingToken.EndColumn +
		(fElements != null && elementsAfter != fElements ? fElements.Count : 0));
	  if (sourceFile != null)
		sourceFile.AddDirective(directive);
	  return text;
	}
	PreprocessorDirective ProcessSupportElement(FormattingToken token, out FormattingElements elementsAfter)
	{
	  elementsAfter = null;
	  if (token == null)
		return null;
	  SourceFile sourceFile = GetFileNode();
	  switch (token.Type)
	  {
		case Tokens.LINE:
		  LineDirective line = new LineDirective();
		  string lineText = ProcessDirective(line, token, sourceFile, true, out elementsAfter);
		  SetLineProperties(line, lineText);
		  return line;
		case Tokens.PRAGMADIR:
		  PragmaDirective pragmaDirective = new PragmaDirective();
		  pragmaDirective.Text = ProcessDirective(pragmaDirective, token, sourceFile, true, out elementsAfter);
		  return pragmaDirective;
		case Tokens.ERROR:
		  ErrorDirective errorDirective = new ErrorDirective();
		  errorDirective.Expression = ProcessDirective(errorDirective, token, sourceFile, false, out elementsAfter);
		  return errorDirective;
		case Tokens.WARNING:
		  WarningDirective warningDirective = new WarningDirective();
		  warningDirective.Text = ProcessDirective(warningDirective, token, sourceFile, false, out elementsAfter);
		  return warningDirective;
		case Tokens.REGION:
		  RegionDirective regionDirective = new RegionDirective();
		  regionDirective.Name = ProcessDirective(regionDirective, token, sourceFile, false, out elementsAfter);
		  regionDirective.NameRange = new SourceRange(token.Line, regionDirective.EndOffset - regionDirective.Name.Length,
			token.Line, regionDirective.EndOffset);
		  return regionDirective;
		case Tokens.ENDREG:
		  EndRegionDirective endRegion = new EndRegionDirective();
		  endRegion.Message = ProcessDirective(endRegion, token, sourceFile, false, out elementsAfter);
		  return endRegion;
	  }
	  return null;
	}
	protected void PreparePreprocessor()
	{
	  if (RootNode == null || scanner == null)
		return;
	  SourceFile sourceFile = RootNode as SourceFile;
	  if (sourceFile == null)
		sourceFile = RootNode.GetSourceFile();
	  if (sourceFile == null)
		sourceFile = GetSourceFile(string.Empty);
	  _Preprocessor = new CSharpPreprocessor(scanner as CSharpScanner, this, sourceFile);
	}
	bool IsDirectiveForPreprocessing(Token token)
	{
	  if (token == null)
		return false;
	  int tokenType = token.Type;
	  return Tokens.DEFINE == tokenType || Tokens.UNDEF == tokenType ||
		Tokens.IFDIR == tokenType || Tokens.ELIF == tokenType ||
		Tokens.ELSEDIR == tokenType || Tokens.ENDIF == tokenType;
	}
	void SetLineProperties(LineDirective line, string text)
	{
	  string lineIndicatorStr = text.Trim();  
	  switch (lineIndicatorStr)
	  {
		case "default":
		  line.Default = true;
		  break;
		case "hidden":
		  line.Hidden = true;
		  break;
		default:
		  int lineNumber = 0;
		  string fileName = null;
		  GetLineInfo(lineIndicatorStr, out lineNumber, out fileName);
		  line.LineNumber = lineNumber;
		  line.FileName = fileName;
		  break;
	  }
	}
	void GetLineInfo(string lineIndicatorStr, out int lineNumber, out string fileName)
	{
	  lineNumber = 0;
	  fileName = null;
	  if (string.IsNullOrEmpty(lineIndicatorStr))
		return;
	  int numberEndIndex = 0;
	  int length = lineIndicatorStr.Length;
	  for (int i = 0; i < length; i++)
		if (char.IsDigit(lineIndicatorStr[i]))
		  numberEndIndex = i;
		else
		  break;
	  if (numberEndIndex == 0)
		return;
	  string lineNumberStr = lineIndicatorStr.Substring(0, ++numberEndIndex);
	  int.TryParse(lineNumberStr, out lineNumber);
	  if (numberEndIndex == length)
		return;
	  string fileNameLocal = lineIndicatorStr.Substring(numberEndIndex).Trim().Trim('"');
	  if (string.IsNullOrEmpty(fileNameLocal))
		return;
	  fileName = fileNameLocal;
	}
	#endregion
	#region GetSourceFileFromContext
	SourceFile GetSourceFileFromContext()
	{
	  if (Context == null)
		return null;
	  if (Context is SourceFile)
		return Context as SourceFile;
	  else
		return Context.FileNode;
	}
	#endregion
	#region AddTextString
	protected void AddTextString(Token stringToken)
	{
	  if (ShouldWorkAsExpressionParser)
		return;
	  int startOffset = stringToken.Range.Start.Offset + 1;
	  string stringValue = String.Empty;
	  if (stringToken.Value[0] == '@')
	  {
		startOffset++;
		stringValue = stringToken.Value.Substring(2, stringToken.Value.Length - 3);
	  }
	  else
		stringValue = stringToken.Value.Substring(1, stringToken.Value.Length - 2);
	  TextString lTextString = new TextString(stringValue, false);
	  SourceFile fileParent = null;
	  if (Context != null)
		fileParent = Context.FileNode;
	  lTextString.SetParent(fileParent);
	  lTextString.SetRange(new SourceRange(stringToken.Range.Start.Line, startOffset, stringToken.Range.End.Line, stringToken.Range.End.Offset - 1));
	  TextStrings.Add(lTextString);
	}
	#endregion
	#region TokenIsAspToken
	bool TokenIsAspToken(int type)
	{
	  return type == Tokens.ASPBLOCKEND || type == Tokens.ASPBLOCKSTART || type == Tokens.ASPCOMMENT;
	}
	#endregion
	#region GetDocumentText
	string GetDocumentText(SourceRange range)
	{
	  if (range.IsEmpty)
		return String.Empty;
	  IDocument document = Document;
	  if (document == null)
		return String.Empty;
	  return document.GetText(range.Start.Line, range.Start.Offset, range.End.Line, range.End.Offset);
	}
	#endregion
		private void CloseNestedMethodContext(Token token)
		{
			if (Context == null)
				return;
			LanguageElement nestedMethod = Context as NestedMethod;
			if (nestedMethod == null)
				nestedMethod = Context.GetParent(LanguageElementType.NestedMethod) as NestedMethod;
			if (nestedMethod == null || nestedMethod.Parent == null)
				return;
			while (Context != nestedMethod.Parent && Context != null)
			{
				if (Context != nestedMethod)
					Context.SetRange(GetRange(Context, tToken));
				else
					Context.SetRange(GetRange(Context, token));
				CloseContext();
			}
		}
	#region ProcessAspToken
	void ProcessAspToken(Token token)
	{
	  if (token == null)
		return;
	  switch (token.Type)
	  {
		case Tokens.ASPBLOCKSTART:
		  NestedMethod block = new NestedMethod();
		  block.SetRange(token.Range);
		  OpenContext(block);
		  break;
		case Tokens.ASPBLOCKEND:
					CloseNestedMethodContext(token);
		  break;
		case Tokens.ASPCOMMENT:
		  break;
		default:
		  break;
	  }
	}
	#endregion
	#region Get
	Token TokenStep(bool isPeek)
	{
	  if (isPeek)
		return Peek();
	  base.Get();
	  return la;
	}
	Token PreprocessToken(bool isPeek, Token startToken, FormattingParsingElement lastElement)
	{
	  if (startToken == null)
		return null;
			if (_Preprocessor == null)
		PreparePreprocessor();
	  if (_Preprocessor == null)
		return TokenStep(isPeek);
	  _Preprocessor.ActiveParsingElement = lastElement;
	  return _Preprocessor.Preprocess(startToken);
	}
	protected override void Get()
	{
	  _PreviousTokenWasComment = false;
	  base.Get();
	  Token oldT = tToken;
	  int tokenType = la.Type;
	  while (TokenIsComment(tokenType) || TokenIsSupportElement(tokenType) || tokenType == Tokens.SHARPCOCODIRECTIVE || TokenIsAspToken(tokenType))
	  {
		if (IsDirectiveForPreprocessing(la))
		{
		  la = PreprocessToken(false, la, LastParsingElement);
		}
		else
		{
		  FormattingToken laF = la as FormattingToken;
		  if (TokenIsComment(tokenType))
		  {
			AddCommentNode(la, LastParsingElement);
			_PreviousTokenWasComment = true;
		  }
		  else if (TokenIsSupportElement(tokenType))
		  {
			FormattingElements elementsAfter;
			PreprocessorDirective directive = ProcessSupportElement(laF, out elementsAfter);
			AddToParsingElement(LastParsingElement, directive);
			AddToParsingElement(LastParsingElement, elementsAfter);
		  }
		  else if (TokenIsAspToken(tokenType))
		  {
			ProcessAspToken(la);
		  }
		  if (laF != null)
			laF.WasChecked = true;
		  base.Get();
		}
		tokenType = la.Type;
	  }
	  if (tToken.Type != Tokens.RAZORCOMMENT)
		tToken = oldT;
	  if (la.Type == Tokens.STRINGCON)
		AddTextString(la);
	}
	#endregion
	#region Peek
	protected new Token Peek()
	{
	  _PreviousTokenWasComment = false;
	  Token pt = scanner.GetPt();
	  LastParsingElement = AddFormattingElements(pt as FormattingToken);
	  Token tokens = scanner.GetTokens();
	  Token result = scanner.Peek();
	  int tokenType = result.Type;
	  SaveCategorizedTokenIfNeeded(result);
	  FormattingParsingElement lastElement = LastFormattingParsingElement;
	  while (TokenIsComment(tokenType) || TokenIsSupportElement(tokenType) || tokenType == Tokens.SHARPCOCODIRECTIVE)
	  {
		if (tokenType == Tokens.EOF)
		  return null;
		if (IsDirectiveForPreprocessing(result))
		{
		  scanner.SetTokens(result);
		  result = PreprocessToken(true, result, lastElement);
		  SaveCategorizedTokenIfNeeded(result);
		}
		else
		{
		  if (tokenType == Tokens.SHARPCOCODIRECTIVE)
		  {
			tokens = scanner.GetTokens();
			pt = scanner.GetPt();
			result = scanner.Peek();
			if (result != null)
			  tokenType = result.Type;
			continue;
		  }
		  FormattingToken laF = result as FormattingToken;
		  if (TokenIsComment(tokenType))
		  {
			AddCommentNode(result, lastElement);
			_PreviousTokenWasComment = true;
		  }
		  else if (TokenIsSupportElement(tokenType))
		  {
			scanner.SetTokens(laF);
			FormattingElements elementsAfter;
			PreprocessorDirective directive = ProcessSupportElement(laF, out elementsAfter);
			AddToParsingElement(lastElement, directive);
			AddToParsingElement(lastElement, elementsAfter);
		  }
		  if (laF != null)
			laF.WasChecked = true;
		  result = scanner.Peek();
		  SaveCategorizedTokenIfNeeded(result);
		}
		scanner.SetTokens(tokens);
		scanner.SetOnPeek(pt, result);
		tokenType = result.Type;
	  }
	  return result;
	}
	#endregion
	#region IsSingleLineXmlComment
	bool IsSingleLineXmlComment(Comment comment)
	{
	  return (comment is XmlDocComment && comment.CommentType == CommentType.SingleLine);
	}
	#endregion
	#region AddCommentNode
	internal override void AddCommentNode(Token lCommentToken, FormattingParsingElement lastElement)
	{
	  Comment lComment;
	  CommentType lType = (lCommentToken.Type == Tokens.SINGLELINECOMMENT || lCommentToken.Type == Tokens.SINGLELINEXML) ? CommentType.SingleLine : CommentType.MultiLine;
	  string lValue = lCommentToken.Value;
	  bool hasOpenMultiLineTag = lValue.StartsWith("/*") || lValue.StartsWith("/**");
	  int StartIdx = 2;
	  if (lCommentToken.Type == Tokens.SINGLELINEXML || lCommentToken.Type == Tokens.MULTILINEXML)
	  {
		lComment = new XmlDocComment();
		StartIdx = 3;
	  }
	  else
		lComment = new Comment();
	  lComment.SetCommentType(lType);
	  lComment.SetTextStartOffset(StartIdx);
	  if (!(lComment is XmlDocComment))
	  {
		if (lValue.Length > StartIdx)
		  lValue = lValue.Substring(StartIdx);
		else
		  lValue = "";
	  }
	  if (_PreviousTokenWasComment && IsSingleLineXmlComment(lComment))
	  {
		XmlDocComment previousComment = Comments[Comments.Count - 1] as XmlDocComment;
		if (previousComment != null && IsSingleLineXmlComment(previousComment) && ShouldConcatComments(previousComment.Range, lCommentToken.Range))
		{
		  previousComment.InternalName += "\r\n";
		  int spacesCount = lCommentToken.Range.Start.Offset - 1;
		  int newLinesCount = lCommentToken.Range.Start.Line - previousComment.Range.End.Line - 1;
		  string appendHeightStr = String.Empty;
		  if (newLinesCount > 0)
			appendHeightStr = new String('\n', newLinesCount);
		  string appendStr = String.Empty;
		  if (spacesCount > 0)
			appendStr = new String(' ', spacesCount);
		  previousComment.InternalName = string.Concat(previousComment.InternalName, appendHeightStr, appendStr, lValue);
		  previousComment.SetRange(GetRange(previousComment, lCommentToken));
		  return;
		}
	  }
	  int lMultiLineCommentClosePos = lValue.LastIndexOf("*/");
	  if (lCommentToken.Type == Tokens.RAZORCOMMENT) {
		lMultiLineCommentClosePos = lValue.LastIndexOf("*@");
		tToken = lCommentToken.Clone();
	  }
	  bool hasCloseMultiLineTag = lMultiLineCommentClosePos >= 0;
	  if (lType == CommentType.MultiLine && hasCloseMultiLineTag && !(lComment is XmlDocComment))
		lValue = lValue.Substring(0, lMultiLineCommentClosePos);
	  if (hasOpenMultiLineTag && !hasCloseMultiLineTag)
		lComment.IsUnfinished = true;
	  lComment.SetRange(lCommentToken.Range);
	  lComment.StartPos = lCommentToken.StartPosition;
	  lComment.EndPos = lCommentToken.EndPosition;
	  lComment.InternalName = GlobalStringStorage.Intern(lValue);
	  Comments.Add(lComment);
	  FormattingToken commentToken = lCommentToken as FormattingToken;
	  if (commentToken != null && !commentToken.WasChecked)
	  {
		AddToParsingElement(lastElement, lComment);
		AddToParsingElement(lastElement, ((FormattingToken)lCommentToken).FormattingElements);
	  }
	  if (_ParsingRazor)
		AddNode(lComment);
	}
	private bool ShouldConcatComments(SourceRange previousCommentRange, SourceRange newCommentRange)
	{
	  int previousEndLine = previousCommentRange.End.Line;
	  int newCommentStartLine = newCommentRange.Start.Line;
	  return (newCommentStartLine - previousEndLine) < 2;
	}
	#endregion
	#region Error
	public void Error(string text)
	{
	}
	#endregion
	#region AddConditionalCompilationSymbols
	public void AddConditionalCompilationSymbols(String[] symbols)
	{
	  if (symbols != null)
	  {
		for (int i = 0; i < symbols.Length; ++i)
		{
		  symbols[i] = symbols[i].Trim();
		  if (symbols[i].Length > 0 && !ccs.Contains(symbols[i]))
		  {
			ccs.Add(symbols[i]);
		  }
		}
	  }
	}
	#endregion
	#region SetAttributes
	protected void SetAttributes(CodeElement target, LanguageElementCollection attributes)
	{
	  if (attributes != null && target != null)
	  {
		target.SetAttributes(attributes);
	  }
	}
	#endregion
	#region SetAccessSpecifiers
	protected void SetAccessSpecifiers(AccessSpecifiedElement element, AccessSpecifiers accessSpecifiers, MemberVisibility visibility)
	{
	  SetAccessSpecifiers(element, accessSpecifiers, visibility, true);
	}
	#endregion
	#region SetAccessSpecifiers
	protected void SetAccessSpecifiers(AccessSpecifiedElement element, AccessSpecifiers accessSpecifiers, MemberVisibility visibility, bool setRange)
	{
	  if (element == null || accessSpecifiers == null)
		return;
	  element.SetAccessSpecifiers(accessSpecifiers);
	  if (visibility != MemberVisibility.Illegal)
	  {
		element.SetVisibility(visibility);
		element.IsDefaultVisibility = false;
	  }
	  if (setRange)
		AddAccessSpecifiersRange(element, accessSpecifiers);
	}
	#endregion
	#region AddAccessSpecifiersRange
	protected void AddAccessSpecifiersRange(LanguageElement element, AccessSpecifiers accessSpecifiers)
	{
	  if (element == null || accessSpecifiers == null)
		return;
	  if (!accessSpecifiers.MutableRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.MutableRange, element.Range));
	  if (!accessSpecifiers.NewRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.NewRange, element.Range));
	  if (!accessSpecifiers.OverloadsRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.OverloadsRange, element.Range));
	  if (!accessSpecifiers.ReadOnlyRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.ReadOnlyRange, element.Range));
	  if (!accessSpecifiers.RegisterRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.RegisterRange, element.Range));
	  if (!accessSpecifiers.SealedRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.SealedRange, element.Range));
	  if (!accessSpecifiers.StaticRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.StaticRange, element.Range));
	  if (!accessSpecifiers.TypeDefRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.TypeDefRange, element.Range));
	  if (!accessSpecifiers.UnsafeRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.UnsafeRange, element.Range));
	  if (!accessSpecifiers.VirtualOverrideAbstractRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.VirtualOverrideAbstractRange, element.Range));
	  if (!accessSpecifiers.VisibilityRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.VisibilityRange, element.Range));
	  if (!accessSpecifiers.VolatileRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.VolatileRange, element.Range));
	  if (!accessSpecifiers.WithEventsRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.WithEventsRange, element.Range));
	  if (!accessSpecifiers.WriteOnlyRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.WriteOnlyRange, element.Range));
	  if (!accessSpecifiers.ExternRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.ExternRange, element.Range));
	  if (!accessSpecifiers.PartialRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.PartialRange, element.Range));
	  if (!accessSpecifiers.AsynchronousRange.IsEmpty)
		element.SetRange(SourceRange.Union(accessSpecifiers.AsynchronousRange, element.Range));
	}
	#endregion
	#region SetMethodParensRanges
	protected void SetMethodParensRanges(Method target, SourceRange parenOpenRange, SourceRange parenCloseRange)
	{
	  if (target == null)
		return;
	  target.ParamOpenRange = parenOpenRange;
	  target.ParamCloseRange = parenCloseRange;
	  SetParensRanges(target, parenOpenRange, parenCloseRange);
	}
	#endregion
	#region SetParensRanges
	protected void SetParensRanges(MemberWithParameters target, SourceRange parenOpenRange, SourceRange parenCloseRange)
	{
	  if (target == null)
		return;
	  target.SetParensRange(GetRange(parenOpenRange, parenCloseRange));
	}
	#endregion
	#region SetContextEndIfNeeded
	protected void SetContextEndIfNeeded()
	{
	  if (Context == null)
		return;
	  SourcePoint newEndPoint = tToken.Range.End;
	  SourcePoint contextEndPoint = Context.Range.End;
	  if (newEndPoint > contextEndPoint)
	  {
		SetContextEnd(newEndPoint);
		return;
	  }
	  for (int i = Context.NodeCount - 1; i >= 0; i--)
	  {
		LanguageElement currentNode = Context.Nodes[i] as LanguageElement;
		if (currentNode == null)
		  continue;
		SourcePoint nodeRangeEnd = currentNode.Range.End;
		if (nodeRangeEnd > newEndPoint)
		  return;
	  }
	  if (newEndPoint > contextEndPoint)
		SetContextEnd(newEndPoint);
	}
	#endregion
	#region SetIndexRanges
	protected void SetIndexRanges(Property target, SourceRange indexOpenRange, SourceRange indexCloseRange)
	{
	  if (target == null)
		return;
	  target.IndexOpenRange = indexOpenRange;
	  target.IndexCloseRange = indexCloseRange;
	}
	#endregion
	#region SetParameters
	protected void SetParameters(MemberWithParameters target, LanguageElementCollection parameters)
	{
	  if (parameters != null && target != null)
	  {
		target.SetParameters(parameters);
		for (int i = 0; i < parameters.Count; i++)
		  target.AddDetailNode(parameters[i]);
	  }
	}
	#endregion
	#region CreateEvent
	protected Event CreateEvent(TypeReferenceExpression type, ElementReferenceExpression name)
	{
	  Event result = CreateEvent(type, GlobalStringStorage.Intern(name.ToString()), name.NameRange);
	  if (result.Name.IndexOf(".") != -1)
		result.AddImplementsExpression(name);
	  return result;
	}
	protected Event CreateEvent(TypeReferenceExpression type, String name, SourceRange nameRange)
	{
	  if (type == null || name == null)
		return null;
	  Event result = new Event(name);
	  result.NameRange = nameRange;
	  result.MemberType = GlobalStringStorage.Intern(type.ToString());
	  result.MemberTypeReference = type;
	  result.TypeRange = type.Range;
	  result.AddDetailNode(type);
	  if (result.Name.IndexOf(".") != -1)
		result.IsExplicitInterfaceMember = true;
	  return result;
	}
	#endregion
	#region CreateIndexer
	protected Property CreateIndexer(TypeReferenceExpression type, String name, SourceRange nameRange)
	{
	  if (type == null || name.Length == 0)
		return null;
	  Property property = new Property(GlobalStringStorage.Intern(type.ToString()), name);
	  property.NameRange = nameRange;
	  property.MemberTypeReference = type;
	  property.TypeRange = type.Range;
	  property.AddDetailNode(type);
	  if (property.Name.IndexOf(".") != -1)
	  {
		property.IsExplicitInterfaceMember = true;
	  }
	  return property;
	}
	#endregion
	#region CreateProperty
	protected Property CreateProperty(TypeReferenceExpression type, ElementReferenceExpression name)
	{
	  if (type == null || name == null)
		return null;
	  Property property = new Property(GlobalStringStorage.Intern(type.ToString()), GlobalStringStorage.Intern(name.ToString()));
	  property.NameRange = name.NameRange;
	  property.MemberTypeReference = type;
	  property.TypeRange = type.Range;
	  property.AddDetailNode(type);
	  if (property.Name.IndexOf(".") != -1)
	  {
		property.IsExplicitInterfaceMember = true;
		property.AddImplementsExpression(name);
	  }
	  return property;
	}
	#endregion
	#region CreateMethod
	protected Method CreateMethod(TypeReferenceExpression type, ElementReferenceExpression name)
	{
	  if (type == null || name == null)
		return null;
	  Method method = new Method(GlobalStringStorage.Intern(type.ToString()), GlobalStringStorage.Intern(name.ToString()));
	  method.NameRange = name.NameRange;
	  method.MemberTypeReference = type;
	  method.TypeRange = type.Range;
	  method.AddDetailNode(type);
	  if (method.Name.IndexOf(".") != -1)
	  {
		method.IsExplicitInterfaceMember = true;
		method.AddImplementsExpression(name);
	  }
	  if (type.ToString() == "void")
		method.MethodType = MethodTypeEnum.Void;
	  else
		method.MethodType = MethodTypeEnum.Function;
	  return method;
	}
	#endregion
	#region CreateCastOperator
	protected Method CreateCastOperator(TypeReferenceExpression type, bool isExplicit)
	{
	  if (type == null)
		return null;
	  String name = String.Empty;
	  if (isExplicit)
		name = "op_Explicit";
	  else
		name = "op_Implicit";
	  Method method = new Method(GlobalStringStorage.Intern(type.ToString()), name);
	  method.IsClassOperator = true;
	  method.NameRange = type.NameRange;
	  method.MemberTypeReference = type;
	  method.TypeRange = type.Range;
	  method.AddDetailNode(type);
	  if (type.ToString() == "void")
		method.MethodType = MethodTypeEnum.Void;
	  else
		method.MethodType = MethodTypeEnum.Function;
	  method.IsImplicitCast = isExplicit == false;
	  method.IsExplicitCast = isExplicit;
	  return method;
	}
	#endregion
	#region CreateConstructor
	protected Method CreateConstructor(String name, SourceRange nameRange)
	{
	  Method method = new Method(name);
	  method.NameRange = nameRange;
	  method.MethodType = MethodTypeEnum.Constructor;
	  return method;
	}
	#endregion
	#region CreateDestructor
	protected Method CreateDestructor(String name, SourceRange nameRange)
	{
	  Method method = new Method("~" + name);
	  method.NameRange = nameRange;
	  method.MethodType = MethodTypeEnum.Destructor;
	  return method;
	}
	#endregion
	#region CreateEnumElement
	protected EnumElement CreateEnumElement(String name, Expression expression, SourceRange nameRange, SourceRange startRange, SourceRange endRange)
	{
	  EnumElement result = new EnumElement(name, expression);
	  result.NameRange = nameRange;
	  result.SetRange(GetRange(startRange, endRange));
	  return result;
	}
	#endregion
	#region CreateClassOperator
	protected Method CreateClassOperator(String name, SourceRange nameRange, TypeReferenceExpression typeRef, OperatorType operatorType)
	{
	  if (typeRef == null || name == null)
		return null;
	  Method overloadableOperator = new Method(GlobalStringStorage.Intern(typeRef.ToString()), name);
	  overloadableOperator.NameRange = nameRange;
	  overloadableOperator.MemberTypeReference = typeRef;
	  overloadableOperator.TypeRange = typeRef.Range;
	  overloadableOperator.AddDetailNode(typeRef);
	  overloadableOperator.IsClassOperator = true;
	  overloadableOperator.OverloadsOperator = operatorType;
	  if (typeRef.ToString() != "void")
		overloadableOperator.MethodType = MethodTypeEnum.Function;
	  else
		overloadableOperator.MethodType = MethodTypeEnum.Void;
	  return overloadableOperator;
	}
	#endregion
	#region IsConstructor
	protected bool IsConstructor()
	{
	  if (la.Type != Tokens.IDENT)
		return false;
	  ResetPeek();
	  if (Peek().Type != Tokens.LPAR)
		return false;
	  return true;
	}
	#endregion
	#region SetBinaryOperationFields
	void SetBinaryOperationFields(BinaryOperatorExpression expression, Token operatorToken, Expression leftPart, Expression rightPart)
	{
	  expression.OperatorText = operatorToken.Value;
	  expression.SetOperatorRange(operatorToken.Range);
	  SourceRange endRange = operatorToken.Range;
	  if (rightPart != null)
		endRange = rightPart.Range;
	  expression.SetRange(GetRange(leftPart, endRange));
	}
	#endregion
	#region GetTypeCheck
	protected TypeCheck GetTypeCheck(Expression leftPart, Expression rightPart, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  TypeCheck result = new TypeCheck(leftPart, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetConditionalTypeCast
	protected ConditionalTypeCast GetConditionalTypeCast(Expression leftPart, Expression rightPart, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  ConditionalTypeCast result = new ConditionalTypeCast(leftPart, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetBinaryOperatorExpression
	protected BinaryOperatorExpression GetBinaryOperatorExpression(Expression leftPart, Expression rightPart, BinaryOperatorType operatorType, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  BinaryOperatorExpression result = new BinaryOperatorExpression(leftPart, operatorType, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetRelationalOperation
	protected RelationalOperation GetRelationalOperation(Expression leftPart, Expression rightPart, RelationalOperator operatorType, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  RelationalOperation result = new RelationalOperation(leftPart, operatorType, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetAssignmentExpression
	protected AssignmentExpression GetAssignmentExpression(Expression leftPart, Expression rightPart, AssignmentOperatorType assignmentOperator, Token operatorToken)
	{
	  if ( operatorToken == null)
		return null;
	  AssignmentExpression result = new AssignmentExpression();
	  if (leftPart != null)
		result.LeftSide = leftPart;
	  if (rightPart != null)
		result.RightSide = rightPart;
	  result.AssignmentOperator = assignmentOperator;
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetLogicalOperation
	protected LogicalOperation GetLogicalOperation(Expression leftPart, Expression rightPart, LogicalOperator operatorType, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  LogicalOperation result = new LogicalOperation(leftPart, operatorType, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region GetNullCoalescingExpression
	protected NullCoalescingExpression GetNullCoalescingExpression(Expression leftPart, Expression rightPart, Token operatorToken)
	{
	  if (leftPart == null || rightPart == null || operatorToken == null)
		return null;
	  NullCoalescingExpression result = new NullCoalescingExpression(leftPart, rightPart);
	  SetBinaryOperationFields(result, operatorToken, leftPart, rightPart);
	  return result;
	}
	#endregion
	#region ToMethodReference
	protected MethodReferenceExpression ToMethodReference(ElementReferenceExpression source)
	{
	  if (source == null)
		return null;
	  MethodReferenceExpression result = null;
	  if (source is PointerElementReference)
	  {
		result = new PointerMethodReference(source, source.Name, source.NameRange);
		result.AddNode(source.Qualifier);
	  }
	  else
	  {
		if (source.Qualifier == null)
		  result = new MethodReferenceExpression(source.Name, source.NameRange);
		else
		{
		  result = new MethodReferenceExpression(source.Qualifier, source.Name, source.NameRange);
		  result.AddNode(source.Qualifier);
		}
	  }
	  CopyFieldValuesFromElementReference(result, source);
	  return result;
	}
	#endregion
	#region CopyFieldValuesFromElementReference
	protected void CopyFieldValuesFromElementReference(ReferenceExpressionBase target, ReferenceExpressionBase source)
	{
	  if (target == null || source == null)
		return;
	  if (source.TypeArguments != null && source.TypeArguments.Count > 0)
		target.TypeArguments = source.TypeArguments;
	  CopyFieldValuesFromExpression(target, source);
	  if (source.Qualifier != null)
		target.Qualifier = source.Qualifier;
	}
	#endregion
	#region CopyFieldValuesFromExpression
	protected void CopyFieldValuesFromExpression(Expression target, Expression source)
	{
	  if (target == null || source == null)
		return;
	  target.SetRange(source.Range);
	  target.NameRange = source.NameRange;
	  if (source.DetailNodeCount > 0)
		for (int i = 0; i < source.DetailNodeCount; i++)
		  if (!target.DetailNodes.Contains(source.DetailNodes[i]))
			target.AddDetailNode(source.DetailNodes[i] as LanguageElement);
	}
	#endregion
	#region CreateParameter
	protected Param CreateParameter(Token nameToken, TypeReferenceExpression typeRef, ArgumentDirection direction, SourceRange paramRange)
	{
	  Param parameter = null;
	  if (nameToken == null)
		return parameter;
	  if (direction == ArgumentDirection.ArgList)
	  {
		parameter = new Param(nameToken.Value);
		SetParameterValues(parameter, nameToken, null, direction, paramRange);
	  }
	  if (typeRef == null)
		return parameter;
	  parameter = new Param(GlobalStringStorage.Intern(typeRef.ToString()), nameToken.Value);
	  SetParameterValues(parameter, nameToken, typeRef, direction, paramRange);
	  return parameter;
	}
	#endregion
	#region CreateExtensionMethodParameter
	protected Param CreateExtensionMethodParameter(Token nameToken, TypeReferenceExpression typeRef, ArgumentDirection direction, SourceRange paramRange)
	{
	  Param parameter = null;
	  if (nameToken == null || typeRef == null)
		return parameter;
	  parameter = new ExtensionMethodParam(GlobalStringStorage.Intern(typeRef.ToString()), nameToken.Value);
	  SetParameterValues(parameter, nameToken, typeRef, direction, paramRange);
	  return parameter;
	}
	#endregion
	#region IsMemberInit
	protected bool IsMemberInit()
	{
	  if (la.Type != Tokens.IDENT)
		return false;
	  ResetPeek();
	  return Peek().Type == Tokens.ASSGN;
	}
	#endregion
	#region InitializeTypeDeclaration
	protected void InitializeTypeDeclaration(TypeDeclaration typeDecl, SourceRange startRange, AccessSpecifiers accessSpecifiers, MemberVisibility visibility)
	{
	  if (typeDecl == null)
		return;
	  typeDecl.SetRange(startRange);
	  SetAccessSpecifiers(typeDecl, accessSpecifiers, visibility);
	}
	#endregion
	#region IsSqlExpression
	protected bool IsSqlExpression()
	{
	  if (la.Type != Tokens.IDENT || la.Value != "from")
		return false;
	  ResetPeek();
	  Token nextToken = Peek();
	  if (nextToken.Type != Tokens.IDENT)
		return false;
	  nextToken = Peek();
	  if (nextToken.Type == Tokens.SCOLON || nextToken.Type == Tokens.ASSGN || nextToken.Type == Tokens.COMMA)
		return false;
	  return true;
	}
	#endregion
	#region SkipLambdaParamList
	bool SkipLambdaParamList(ref Token currentToken)
	{
	  int currentTokenType = currentToken.Type;
	  int lParCount = 1;
	  while (currentTokenType != Tokens.EOF && currentTokenType != Tokens.SCOLON && currentTokenType != Tokens.ASSGN)
	  {
		if (lParCount == 0)
		  return true;
		if (currentTokenType == Tokens.LPAR)
		  lParCount++;
		if (currentTokenType == Tokens.RPAR)
		  lParCount--;
		currentToken = Peek();
		if (currentToken == null)
		  return false;
		currentTokenType = currentToken.Type;
	  }
	  return true;
	}
	#endregion
	bool IsYieldStatement()
	{
	  if (la.Type != Tokens.IDENT || la.Value != "yield")
		return false;
	  ResetPeek();
	  int nextType = Peek().Type;
	  return nextType == Tokens.RETURN || nextType == Tokens.BREAK;
	}
	bool IsPartialModifier(Token token)
	{
	  return token.Value == "partial";
	}
	#region IsLambda
	bool IsLambda()
	{
	  ResetPeek();
	  Token currentToken = la;
	  if (currentToken == null)
		return false;
	  if (currentToken.Type == Tokens.IDENT && currentToken.Value == "async")
		currentToken = Peek();
	  if (currentToken.Type != Tokens.LPAR)
		return IsLambdaWithImplicitParams();
	  currentToken = Peek();
	  if (!SkipLambdaParamList(ref currentToken))
		return false;
	  return currentToken.Type == Tokens.LAMBDA;
	}
	#endregion
	#region IsLambdaWithImplicitParams
	bool IsAssignOperatorToken(int token)
	{
	  if (token == Tokens.ASSGN)
		return true;
	  return assgnOps[token];
	}
	bool IsLambdaWithImplicitParams()
	{
	  int currentTokenType = la.Type;
	  while (currentTokenType != Tokens.EOF && currentTokenType != Tokens.SCOLON && currentTokenType != Tokens.COLON
					&& currentTokenType != Tokens.LPAR && currentTokenType != Tokens.RPAR
		  && currentTokenType != Tokens.LBRACK && currentTokenType != Tokens.RBRACK
					&& currentTokenType != Tokens.COMMA && currentTokenType != Tokens.NEW && currentTokenType != Tokens.STRINGCON && !IsAssignOperatorToken(currentTokenType))
	  {
		if (currentTokenType == Tokens.LAMBDA)
		  return true;
		currentTokenType = Peek().Type;
	  }
	  return false;
	}
	#endregion
	#region IsImplicitLambdaParameter
	protected bool IsImplicitLambdaParameter()
	{
	  if (la.Type != Tokens.IDENT)
		return false;
	  ResetPeek();
	  int nextTokenType = Peek().Type;
	  if (nextTokenType == Tokens.COMMA || nextTokenType == Tokens.LAMBDA || nextTokenType == Tokens.RPAR)
		return true;
	  return false;
	}
	#endregion
	#region SetParameterValues
	void SetParameterValues(Param parameter, Token nameToken, TypeReferenceExpression typeRef, ArgumentDirection direction, SourceRange paramRange)
	{
	  if (parameter == null)
		return;
	  parameter.Direction = direction;
	  parameter.NameRange = nameToken.Range;
	  if (typeRef != null)
	  {
		parameter.MemberTypeReference = typeRef;
		parameter.SetFullTypeName(GetFullType(GlobalStringStorage.Intern(typeRef.ToString())));
		parameter.TypeRange = typeRef.Range;
		parameter.AddDetailNode(typeRef);
	  }
	  parameter.SetRange(paramRange);
	}
	#endregion
	#region GetFullType
	string GetFullType(string shortName)
	{
	  if (shortName == null || shortName.Length == 0)
		return shortName;
	  if (_PrimitiveToFullTypes.ContainsKey(shortName))
		return (string)_PrimitiveToFullTypes[shortName];
	  return shortName;
	}
	#endregion
	#region AttachToQualifiedElementReference
	protected void AttachToQualifiedElementReference(Expression source, QualifiedElementReference elRef)
	{
	  if (source == null || elRef == null)
		return;
	  elRef.SetRange(GetRange(source, elRef));
	  Expression currentQualifier = elRef.Qualifier;
	  ReferenceExpressionBase currentQualified = elRef;
	  ReferenceExpressionBase qualifier = currentQualifier as ReferenceExpressionBase;
	  while (qualifier != null && qualifier.Qualifier != null)
	  {
		qualifier.SetRange(GetRange(source, qualifier));
		currentQualified = qualifier;
		currentQualifier = qualifier.Qualifier;
		qualifier = qualifier.Qualifier as ReferenceExpressionBase;
	  }
	  QualifiedElementReference newQualifier = new QualifiedElementReference(source, currentQualifier.Name, currentQualifier.NameRange);
	  newQualifier.AddNode(source);
	  CopyFieldValuesFromElementReference(newQualifier, currentQualifier as ElementReferenceExpression);
	  newQualifier.SetRange(GetRange(source, newQualifier));
	  if (currentQualified != null)
		currentQualified.Qualifier = newQualifier;
	}
	#endregion
	#region AttachToPointerElementReference
	protected void AttachToPointerElementReference(Expression source, QualifiedElementReference elRef)
	{
	  if (source == null || elRef == null)
		return;
	  elRef.SetRange(GetRange(source, elRef));
	  Expression currentQualifier = elRef.Qualifier;
	  ReferenceExpressionBase currentQualified = elRef;
	  while (currentQualifier is ReferenceExpressionBase && (currentQualifier as ReferenceExpressionBase).Qualifier != null)
	  {
		currentQualifier.SetRange(GetRange(source, currentQualifier));
		currentQualified = currentQualifier as ReferenceExpressionBase;
		currentQualifier = currentQualified.Qualifier;
	  }
	  QualifiedElementReference newQualifier = new PointerElementReference(source, currentQualifier.Name, currentQualifier.NameRange);
	  newQualifier.AddNode(source);
	  CopyFieldValuesFromElementReference(newQualifier, currentQualifier as ElementReferenceExpression);
	  newQualifier.SetRange(GetRange(source, newQualifier));
	  currentQualified.Qualifier = newQualifier;
	}
	#endregion
	#region CreateConstant
	protected Const CreateConstant(string constName, TypeReferenceExpression typeRef, Expression constValue, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef, SourceRange operatorRange)
	{
	  Const constant = null;
	  if (typeRef != null && constName != null)
	  {
		constant = new Const(GlobalStringStorage.Intern(typeRef.ToString()), constName);
		SetVariableProperties(constant, typeRef, nameRange, shouldAddTypeRef, operatorRange);
		if (constValue != null)
		{
		  constant.Expression = constValue;
		  constant.SetRange(GetRange(startRange, constValue));
		}
		else
		  constant.SetRange(GetRange(startRange, tToken));
	  }
	  return constant;
	}
	#endregion
	#region SetVariableProperties
	void SetVariableProperties(Variable variable, TypeReferenceExpression typeRef, SourceRange nameRange, bool shouldAddTypeRef, SourceRange operatorRange)
	{
	  if (variable == null)
		return;
	  variable.NameRange = nameRange;
	  variable.MemberTypeReference = typeRef;
	  variable.TypeRange = typeRef.Range;
	  variable.OperatorRange = operatorRange;
	  if (shouldAddTypeRef)
		variable.AddDetailNode(typeRef);
	  else
		typeRef.SetParent(variable);
	}
	#endregion
	#region CreateVariable
	protected Variable CreateVariable(string name, TypeReferenceExpression typeRef, Expression initializer, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef, SourceRange operatorRange)
	{
	  if (typeRef == null || name == null)
		return null;
	  if (!shouldAddTypeRef)
	  {
		typeRef = typeRef.Clone() as TypeReferenceExpression;
		typeRef.Comments.Clear();
	  }
	  Variable variable = null;
	  if (initializer != null)
	  {
		InitializedVariable initVariable = new InitializedVariable(GlobalStringStorage.Intern(typeRef.ToString()), name);
		SetVariableProperties(initVariable, typeRef, nameRange, shouldAddTypeRef, operatorRange);
		initVariable.Expression = initializer;
		initVariable.SetRange(GetRange(startRange, initializer));
		return initVariable;
	  }
	  variable = new Variable(GlobalStringStorage.Intern(typeRef.ToString()), name);
	  SetVariableProperties(variable, typeRef, nameRange, shouldAddTypeRef, operatorRange);
	  variable.SetRange(GetRange(startRange, nameRange));
	  return variable;
	}
	#endregion
	#region CreateImplicitVariable(string name, Expression initializer, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef, SourceRange operatorRange, SourceRange varKeywordRange)
	protected ImplicitVariable CreateImplicitVariable(string name, Expression initializer, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef, SourceRange operatorRange, SourceRange varKeywordRange)
	{
	  if (name == null)
		return null;
	  ImplicitVariable variable = new ImplicitVariable(name, initializer);
	  variable.NameRange = nameRange;
	  variable.OperatorRange = operatorRange;
	  if (initializer != null)
	  {
		variable.AddDetailNode(initializer);
		variable.SetRange(GetRange(startRange, initializer));
	  }
	  else
		variable.SetRange(GetRange(startRange, nameRange.End));
	  variable.TypeRange = varKeywordRange;
	  return variable;
	}
	#endregion
	#region CreateImplicitVariable(string name, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef)
	protected ImplicitVariable CreateImplicitVariable(string name, SourceRange nameRange, SourceRange startRange, bool shouldAddTypeRef)
	{
	  if (name == null)
		return null;
	  ImplicitVariable variable = new ImplicitVariable(name);
	  variable.NameRange = nameRange;
	  return variable;
	}
	#endregion
	#region GetTypeParameter
	protected TypeParameter GetTypeParameter(GenericModifier modifier, string paramName)
	{
	  if (modifier == null || modifier.TypeParameters == null
			  || modifier.TypeParameters.Count == 0 || paramName == null || paramName.Length == 0)
		return null;
	  for (int i = 0; i < modifier.TypeParameters.Count; i++)
		if (modifier.TypeParameters[i].Name == paramName)
		  return modifier.TypeParameters[i];
	  return null;
	}
	#endregion
	#region SetTypeArguments
	protected void SetTypeArguments(ReferenceExpressionBase expression, TypeReferenceExpressionCollection typeRefColl)
	{
	  if (expression == null || typeRefColl == null || typeRefColl.Count == 0)
		return;
	  expression.TypeArguments = typeRefColl;
	}
	#endregion
	#region EndOf
	protected int EndOf(String symbol, int start, bool whitespaces)
	{
	  while ((start < symbol.Length) && (Char.IsWhiteSpace(symbol[start]) ^ !whitespaces))
	  {
		++start;
	  }
	  return start;
	}
	#endregion
	#region RemPPDirective
	protected String RemPPDirective(String symbol)
	{
	  int start = 1;
	  int end;
	  start = EndOf(symbol, start, true);
	  start = EndOf(symbol, start, false);
	  start = EndOf(symbol, start, true);
	  end = EndOf(symbol, start, false);
	  return symbol.Substring(start, end - start);
	}
	#endregion
	#region AddCCS
	protected void AddCCS(String symbol)
	{
	  symbol = RemPPDirective(symbol);
	  if (!ccs.Contains(symbol))
	  {
		ccs.Add(symbol);
	  }
	}
	#endregion
	#region RemCCS
	protected void RemCCS(String symbol)
	{
	  ccs.Remove(RemPPDirective(symbol));
	}
	#endregion
	#region IsCCS
	protected bool IsCCS(String symbol)
	{
	  return ccs.Contains(RemPPDirective(symbol));
	}
	#endregion
	void ParseTypeEnd()
	{
	  if (la == null || la.Type != Tokens.RBRACE)
		return;
	  LanguageElement oldContext = Context;
	  NamespaceMemberDeclarationEnd(Context, false);
	  if (oldContext != Context)
		CallAppropriateParseRule(Context);
	}
	const int maxTerminals = 160;  
	#region Sets...
	protected static BitArray
			unaryOp = NewSet(Tokens.PLUS, Tokens.MINUS, Tokens.NOT, Tokens.TILDE, Tokens.INC, Tokens.DEC, Tokens.TRUE, Tokens.FALSE),
			typeKW = NewSet(Tokens.CHAR, Tokens.BOOL, Tokens.SBYTE, Tokens.BYTE, Tokens.SHORT,
			Tokens.USHORT, Tokens.INT, Tokens.UINT, Tokens.LONG, Tokens.ULONG, Tokens.FLOAT, Tokens.DOUBLE, Tokens.DECIMAL),
			unaryHead = NewSet(Tokens.PLUS, Tokens.MINUS, Tokens.NOT, Tokens.TILDE, Tokens.TIMES, Tokens.INC, Tokens.DEC, Tokens.AND),
			assnStartOp = NewSet(Tokens.PLUS, Tokens.MINUS, Tokens.NOT, Tokens.TILDE, Tokens.TIMES),
			castFollower = NewSet(Tokens.TILDE, Tokens.NOT, Tokens.LPAR, Tokens.IDENT, 
			Tokens.INTCON, Tokens.REALCON, Tokens.CHARCON, Tokens.STRINGCON,
			Tokens.ABSTRACT, Tokens.BASE, Tokens.BOOL, Tokens.BREAK, Tokens.BYTE, Tokens.CASE, Tokens.CATCH,
			Tokens.CHAR, Tokens.CHECKED, Tokens.CLASS, Tokens.CONST, Tokens.CONTINUE, Tokens.DECIMAL, Tokens.DEFAULT,
			Tokens.DELEGATE, Tokens.DO, Tokens.DOUBLE, Tokens.ELSE, Tokens.ENUM, Tokens.EVENT, Tokens.EXPLICIT,
			Tokens.EXTERN, Tokens.FALSE, Tokens.FINALLY, Tokens.FIXED, Tokens.FLOAT, Tokens.FOR, Tokens.FOREACH,
			Tokens.GOTO, Tokens.IFCLAUSE, Tokens.IMPLICIT, Tokens.IN, Tokens.INT, Tokens.INTERFACE, Tokens.INTERNAL,
			Tokens.LOCK, Tokens.LONG, Tokens.NAMESPACE, Tokens.NEW, Tokens.NULL, Tokens.OPERATOR,
			Tokens.OUT, Tokens.OVERRIDE, Tokens.PARAMS, Tokens.PRIVATE, Tokens.PROTECTED, Tokens.PUBLIC,
			Tokens.READONLY, Tokens.REF, Tokens.RETURN, Tokens.SBYTE, Tokens.SEALED, Tokens.SHORT, Tokens.SIZEOF,
			Tokens.STACKALLOC, Tokens.STATIC, Tokens.STRUCT, Tokens.SWITCH, Tokens.THIS, Tokens.THROW,
			Tokens.TRUE, Tokens.TRY, Tokens.TYPEOF, Tokens.UINT, Tokens.ULONG, Tokens.UNCHECKED, Tokens.UNSAFE,
			Tokens.USHORT, Tokens.USINGKW, Tokens.VIRTUAL, Tokens.VOID, Tokens.VOLATILE, Tokens.WHILE
			),
			typArgLstFol = NewSet(Tokens.LPAR, Tokens.RPAR, Tokens.RBRACK, Tokens.COLON, Tokens.SCOLON, Tokens.COMMA, Tokens.DOT,
			Tokens.QUESTION, Tokens.EQ, Tokens.NEQ),
			keyword = NewSet(Tokens.ABSTRACT, Tokens.AS, Tokens.BASE, Tokens.BOOL, Tokens.BREAK, Tokens.BYTE, Tokens.CASE, Tokens.CATCH,
			Tokens.CHAR, Tokens.CHECKED, Tokens.CLASS, Tokens.CONST, Tokens.CONTINUE, Tokens.DECIMAL, Tokens.DEFAULT,
			Tokens.DELEGATE, Tokens.DO, Tokens.DOUBLE, Tokens.ELSE, Tokens.ENUM, Tokens.EVENT, Tokens.EXPLICIT,
			Tokens.EXTERN, Tokens.FALSE, Tokens.FINALLY, Tokens.FIXED, Tokens.FLOAT, Tokens.FOR, Tokens.FOREACH,
			Tokens.GOTO, Tokens.IFCLAUSE, Tokens.IMPLICIT, Tokens.IN, Tokens.INT, Tokens.INTERFACE, Tokens.INTERNAL,
			Tokens.IS, Tokens.LOCK, Tokens.LONG, Tokens.NAMESPACE, Tokens.NEW, Tokens.NULL, Tokens.OPERATOR,
			Tokens.OUT, Tokens.OVERRIDE, Tokens.PARAMS, Tokens.PRIVATE, Tokens.PROTECTED, Tokens.PUBLIC,
			Tokens.READONLY, Tokens.REF, Tokens.RETURN, Tokens.SBYTE, Tokens.SEALED, Tokens.SHORT, Tokens.SIZEOF,
			Tokens.STACKALLOC, Tokens.STATIC, Tokens.STRUCT, Tokens.SWITCH, Tokens.THIS, Tokens.THROW,
			Tokens.TRUE, Tokens.TRY, Tokens.TYPEOF, Tokens.UINT, Tokens.ULONG, Tokens.UNCHECKED, Tokens.UNSAFE,
			Tokens.USHORT, Tokens.USINGKW, Tokens.VIRTUAL, Tokens.VOID, Tokens.VOLATILE, Tokens.WHILE),
			assgnOps = NewSet(Tokens.ASSGN, Tokens.PLUSASSGN, Tokens.MINUSASSGN, Tokens.TIMESASSGN, Tokens.DIVASSGN,
			Tokens.MODASSGN, Tokens.ANDASSGN, Tokens.ORASSGN, Tokens.XORASSGN, Tokens.LSHASSGN) 
			;
	#endregion
	void SetUnclosedMemberContext()
	{
	  _UnclosedMemberContext = Context;
	  Member uclosedMember = _UnclosedMemberContext as Member;
	  if (uclosedMember != null)
		uclosedMember.Unclosed = true;
	}
	void CloseMemberContextIfNeeded(LanguageElement newContext)
	{
	  if (_UnclosedMemberContext == null || Context != _UnclosedMemberContext ||
		  !(newContext is Method || newContext is Property))
		return;
	  _UnclosedMemberContext = null;
	  CloseContext();
	}
	protected override void OpenContext(LanguageElement newContext)
	{
	  CloseMemberContextIfNeeded(newContext);
	  base.OpenContext(newContext);
	}
	void CloseMemberContext(Token token)
	{
	  if (token == null || token.Type == Tokens.RBRACE)
	  {
		CloseContext();
		return;
	  }
	  SetUnclosedMemberContext();
	}
	string GetAccessorName(string prefix)
	{
	  Member member = Context as Member;
	  if (member != null && member.ImplementsCount > 0)
	  {
		string name = string.Empty;
		QualifiedElementReference reference = member.ImplementsExpressions[0] as QualifiedElementReference;
		if (reference != null && reference.Qualifier != null)
		  return reference.Qualifier.ToString() + '.' + prefix + member.GetNameWithoutImplementsQualifier();
	  }
	  return prefix + Context.Name;
	}
	#region NewSet
	static BitArray NewSet(params int[] values)
	{
	  BitArray a = new BitArray(maxTerminals);
	  foreach (int x in values) a[x] = true;
	  return a;
	}
	#endregion
	#region IsAssignment
	protected bool IsAssignment()
	{
	  return la.Type == Tokens.IDENT && Peek().Type == Tokens.ASSGN;
	}
	#endregion
	#region NotFinalComma
	protected bool NotFinalComma()
	{
	  int peek = Peek().Type;
	  return la.Type == Tokens.COMMA && peek != Tokens.RBRACE && peek != Tokens.RBRACK;
	}
	#endregion
	#region IsQualident
	protected bool IsQualident(ref Token pt, out string qualident)
	{
	  qualident = "";
	  if (pt.Type == Tokens.IDENT)
	  {
		qualident = pt.Value;
		pt = Peek();
		while (pt.Type == Tokens.DOT)
		{
		  pt = Peek();
		  if (pt.Type != Tokens.IDENT) return false;
		  qualident += "." + pt.Value;
		  pt = Peek();
		}
		return true;
	  }
	  else return false;
	}
	#endregion
	#region IsGeneric
	protected bool IsGeneric()
	{
	  if (_ParsingRazor && _RazorParsingMode == RazorParsingMode.Expression)
		return false;
	  scanner.ResetPeek();
	  Token pt = la;
	  if (!IsTypeArgumentList(ref pt))
	  {
		return false;
	  }
	  return true;
	}
	#endregion
	#region IsTypeArgumentList
	protected bool IsTypeArgumentList(ref Token pt)
	{
	  if (pt.Type == Tokens.LT)
	  {
		pt = Peek();
		while (true)
		{
		  if (!IsType(ref pt))
		  {
			return false;
		  }
		  if (pt.Type == Tokens.GT)
		  {
			pt = Peek();
			break;
		  }
		  else if (pt.Type == Tokens.COMMA)
		  {
			pt = Peek();
		  }
		  else
		  {
			return false;
		  }
		}
	  }
	  else
	  {
		return false;
	  }
	  return true;
	}
	#endregion
		#region MoveToQuestionToken
	Token MoveToQuestionToken(Token questionToken)
	{
	  ResetPeek();
	  Token peek = Peek();
	  while (peek != null && questionToken != null && peek != questionToken)
		peek = Peek();
	  return Peek();
	}
	#endregion
		protected void CloseContextAndSetRange(LanguageElement element)
		{
			if (element == null || Context != element)
				return;
			CloseContext();
			element.SetRange(GetRange(element, tToken));
		}
	#region SkipQualifier
	bool SkipQualifier(ref Token pt)
	{
	  String dummyId;
	  if (pt.Type == Tokens.DBLCOLON || pt.Type == Tokens.DOT)
	  {
		pt = Peek();
		if (!IsQualident(ref pt, out dummyId))
		{
		  return false;
		}
	  }
	  return true;
	}
	#endregion
	#region IsType
	protected bool IsType(ref Token pt)
	{
	  if (typeKW[pt.Type])
	  {
		pt = Peek();
	  }
	  else if (pt.Type == Tokens.VOID)
	  {
		pt = Peek();
		if (pt.Type != Tokens.TIMES)
		{
		  return false;
		}
		pt = Peek();
	  }
	  else if (pt.Type == Tokens.IDENT)
	  {
		pt = Peek();
	  }
	  else
	  {
		return false;
	  }
	  if (pt.Type == Tokens.LT && !IsTypeArgumentList(ref pt))
	  {
		return false;
	  }
	  while (pt.Type == Tokens.DBLCOLON || pt.Type == Tokens.DOT)
	  {
		if (!SkipQualifier(ref pt))
		  return false;
		if (pt.Type == Tokens.LT && !IsTypeArgumentList(ref pt))
		{
		  return false;
		}
	  }
	  if (pt.Type == Tokens.QUESTION)
	  {
				Token questionToken = pt;
		if (IsConditionalExpressionCore())
		  return false;
		else
		  pt = MoveToQuestionToken(pt);
	  }
	  return SkipPointerOrDims(ref pt);
	}
	#endregion
	#region IsLocalVarDecl
	protected bool IsLocalVarDecl()
	{
	  Token pt = la;
	  bool laIsAwait = la.Match(Tokens.IDENT) && la.Value == "await";
	  if (IsAsyncContext && laIsAwait)
		return false;
	  if (laIsAwait)
	  {
		ResetPeek();
		if (Peek().Match(Tokens.IDENT))
		  return Peek().Match(Tokens.SCOLON);
	  }
	  if (IsSqlExpression())
		return false;
	  scanner.ResetPeek();
	  return IsType(ref pt) && (pt.Type == Tokens.IDENT || pt.Type == Tokens.DOT || pt.Type == Tokens.LBRACK);
	}
	#endregion
	#region IsDims
	protected bool IsDims()
	{
	  int peek = Peek().Type;
	  return la.Type == Tokens.LBRACK && (peek == Tokens.COMMA || peek == Tokens.RBRACK);
	}
	protected bool IsLBRACK()
	{
	  int peek = Peek().Type;
	  return la.Type == Tokens.LBRACK;
	}
	#endregion
	#region IsPointerOrDims
	protected bool IsPointerOrDims()
	{
	  return la.Type == Tokens.TIMES || IsDims();
	}
	#endregion
	#region SkipPointerOrDims
	protected bool SkipPointerOrDims(ref Token pt)
	{
	  for (; ; )
	  {
		if (pt.Type == Tokens.LBRACK)
		{
		  do pt = Peek();
		  while (pt.Type == Tokens.COMMA);
		  if (pt.Type != Tokens.RBRACK) return false;
		}
		else if (pt.Type != Tokens.TIMES) break;
		pt = Peek();
	  }
	  return true;
	}
	#endregion
	#region IsAttrTargSpec
	protected bool IsAttrTargSpec()
	{
	  return (la.Type == Tokens.IDENT || keyword[la.Type]) && Peek().Type == Tokens.COLON;
	}
	#endregion
	#region IsFieldDecl
	protected bool IsFieldDecl()
	{
	  int peek = Peek().Type;
	  return la.Type == Tokens.IDENT &&
		  (peek == Tokens.COMMA || peek == Tokens.ASSGN || peek == Tokens.SCOLON);
	}
	#endregion
	#region IsTypeCast
	protected bool IsTypeCast()
	{
	  if (la.Type != Tokens.LPAR) { return false; }
	  if (IsSimpleTypeCast()) { return true; }
	  return GuessTypeCast();
	}
	#endregion
	#region IsSimpleTypeCast
	protected bool IsSimpleTypeCast()
	{
	  scanner.ResetPeek();
	  Token pt1 = Peek();
	  Token pt2 = Peek();
	  return typeKW[pt1.Type] &&
		  (pt2.Type == Tokens.RPAR || pt2.Type == Tokens.TIMES ||
		  (pt2.Type == Tokens.QUESTION && Peek().Type == Tokens.RPAR));
	}
	#endregion
	#region GuessTypeCast
	protected bool GuessTypeCast()
	{
	  scanner.ResetPeek();
	  Token pt = Peek();
	  if (!IsType(ref pt))
	  {
		return false;
	  }
	  if (pt.Type != Tokens.RPAR)
	  {
		return false;
	  }
	  pt = Peek();
	  if (_ActiveSqlExpression != null && pt.Type == Tokens.IDENT && IsSqlKeyword(pt.Value))
		return false;
	  if (IsAdressOfOperation(pt.Type))
		return true;
	  return castFollower[pt.Type];
	}
	#endregion
	bool IsAdressOfOperation(int tokenType)
	{
	  if (tokenType != Tokens.AND)
		return false;
	  if (!IsUnsafeContext())
		return false;
	  return true;
	}
	bool IsImplicitVariable()
	{
	  return WorkAsCharp30Parser && String.Compare(la.Value, "var") == 0;
	}
	bool IsUnsafeContext()
	{
	  return IsUnsafeMethod() || IsUnsafeStatement();
	}
	bool IsUnsafeMethod()
	{
	  AccessSpecifiedElement methodContext = Context as AccessSpecifiedElement;
	  if (methodContext == null)
		methodContext = Context.GetParentMethodOrAccessor() as AccessSpecifiedElement;
	  if (methodContext == null)
		return false;
	  return methodContext.IsUnsafe;
	}
	bool IsUnsafeStatement()
	{
	  UnsafeStatement statementContext = Context as UnsafeStatement;
	  if (statementContext == null)
		statementContext = Context.GetParent(LanguageElementType.UnsafeStatement) as UnsafeStatement;
	  if (statementContext == null)
		return false;
	  return true;
	}
	#region IsGlobalAttrTarget
	protected bool IsGlobalAttrTarget()
	{
	  Token pt = Peek();
	  return la.Type == Tokens.LBRACK && pt.Type == Tokens.IDENT && ("assembly".Equals(pt.Value) || "module".Equals(pt.Value));
	}
	#endregion
	#region IsExternAliasDirective
	protected bool IsExternAliasDirective()
	{
	  return la.Type == Tokens.EXTERN && "alias".Equals(Peek().Value);
	}
	#endregion
	#region IsLtNoWs
	protected bool IsLtNoWs()
	{
	  return (la.Type == Tokens.LT);
	}
	#endregion
	#region IsNoSwitchLabelOrRBrace
	protected bool IsNoSwitchLabelOrRBrace()
	{
	  return (la.Type != Tokens.CASE && la.Type != Tokens.DEFAULT && la.Type != Tokens.RBRACE) ||
		  (la.Type == Tokens.DEFAULT && Peek().Type != Tokens.COLON);
	}
	#endregion
	#region IsSqlKeyword
	bool IsSqlKeyword(string name)
	{
	  return name == "select" || name == "from" || name == "join" || name == "on" || name == "where" ||
		  name == "let" || name == "orderby" || name == "ascending" || name == "descending" ||
		  name == "group" || name == "by" || name == "into";
	}
	#endregion
	#region QueryIdentHasType
	protected bool QueryIdentHasType()
	{
	  ResetPeek();
	  Token nextToken = Peek();
	  return (nextToken.Type != Tokens.IN && !IsSqlKeyword(nextToken.Value));
	}
	#endregion
	#region IsPartOfMemberName
	protected bool IsPartOfMemberName()
	{
	  scanner.ResetPeek();
	  Token pt = la;
	  if (!IsTypeArgumentList(ref pt))
	  {
		return false;
	  }
	  return pt.Type != Tokens.LPAR;
	}
	#endregion
	#region CreateExpressionParser
	public override ExpressionParserBase CreateExpressionParser()
	{
	  return new CSharp30ExpressionParser(this);
	}
	#endregion
	#region SupportsFileExtension
	public override bool SupportsFileExtension(string ext)
	{
	  return ext.ToLower() == STR_Cs;
	}
	#endregion
	#region CreateExpressionInverter
	public override IExpressionInverter CreateExpressionInverter()
	{
	  return new CSharpExpressionInverter();
	}
	#endregion
	#region !!!NewGetRange
	public static SourceRange GetRange(LanguageElement a, LanguageElement b)
	{
	  if (a != null && b != null)
		return new SourceRange(a.InternalRange.Start, b.InternalRange.End);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(LanguageElement a, SourceRange b)
	{
	  if (a != null)
		return new SourceRange(a.InternalRange.Start, b.End);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(LanguageElement a, Token b)
	{
	  if (a != null && b != null)
		return new SourceRange(a.InternalRange.Start, b.Range.End);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(Token a, Token b)
	{
	  if (a != null && b != null)
		return new SourceRange(a.Line, a.Column, b.EndLine, b.EndColumn);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(Token a, SourceRange b)
	{
	  if (a != null)
		return new SourceRange(a.Line, a.Column, b.End.Line, b.End.Offset);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(Token a, LanguageElement b)
	{
	  if (a != null && b != null)
		return new SourceRange(a.Line, a.Column, b.InternalRange.End.Line, b.InternalRange.End.Offset);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(SourceRange a, SourceRange b)
	{
	  return new SourceRange(a.Start, b.End);
	}
	public static SourceRange GetRange(SourceRange a, Token b)
	{
	  if (b != null)
		return new SourceRange(a.Start.Line, a.Start.Offset, b.EndLine, b.EndColumn);
	  return SourceRange.Empty;
	}
	public static SourceRange GetRange(SourceRange a, LanguageElement b)
	{
	  if (b != null)
		return new SourceRange(a.Start, b.InternalRange.End);
	  return SourceRange.Empty;
	}
	#endregion
	protected bool ShouldSkipMethodBody
	{
	  get
	  {
		return false;
	  }
	}
	#region Regions
	protected Stack Regions
	{
	  get
	  {
		if (_Regions == null)
		  _Regions = new Stack();
		return _Regions;
	  }
	}
	#endregion
	#region Language
	public override string Language
	{
	  get
	  {
		return "CSharp";
	  }
	}
	#endregion
	#region Comments
	public override CommentCollection Comments
	{
	  get
	  {
		return _Comments;
	  }
	}
	#endregion
	#region ShouldWorkAsExpressionParser
	public bool ShouldWorkAsExpressionParser
	{
	  get
	  {
		return _ShouldWorkAsExpressionParser;
	  }
	  set
	  {
		_ShouldWorkAsExpressionParser = value;
	  }
	}
	#endregion
	#region WorkAsCharp30Parser
	public bool WorkAsCharp30Parser
	{
	  get
	  {
		return _WorkAsCharp30Parser;
	  }
	  set
	  {
		_WorkAsCharp30Parser = value;
	  }
	}
	#endregion
	public override bool IsQueryExpressionStart(string lineText)
	{
	  if (String.IsNullOrEmpty(lineText))
		return false;
	  string[] tokens = lineText.Split(' ', '\t');
	  int count = tokens.Length;
	  if (count > 2)
	  {
		string tokenPrevTemplate = tokens[count - 2];
		if (String.IsNullOrEmpty(tokenPrevTemplate))
		  return false;
		return String.Compare(tokenPrevTemplate, "from", StringComparison.CurrentCultureIgnoreCase) == 0;
	  }
	  return false;
	}
	protected override bool IsIdent(Token token)
	{
	  return token.Type == Tokens.IDENT;
	}
	void FirstGet()
	{
	  la = new FormattingToken(TokenLanguage.CSharp);
	  la.Value = "";
	  string str = scanner.FirstSkip();
	  if (!String.IsNullOrEmpty(str))
		((FormattingToken)la).FormattingElements.AddSourceFileStartText(str);
	  Get();
	}
	protected ISourceReader Reader
	{
	  get { return _Reader; }
	  set { _Reader = value; }
	}
	protected bool ParsingPostponedTokens
	{
	  get { return _ParsingPostponedTokens; }
	  set { _ParsingPostponedTokens = value; }
	}
	protected Hashtable PrimitiveToFullTypes
	{
	  get { return _PrimitiveToFullTypes; }
	  set { _PrimitiveToFullTypes = value; }
	}
	protected QueryExpression ActiveSqlExpression
	{
	  get { return _ActiveSqlExpression; }
	  set { _ActiveSqlExpression = value; }
	}
	protected CSharpPreprocessor Preprocessor
	{
	  get { return _Preprocessor; }
	  set { _Preprocessor = value; }
	}
  }
  public class CSharp30ExpressionParser : ExpressionParserBase
  {
	public CSharp30ExpressionParser(ParserBase parser) : base(parser) { }
	CSharp30Parser GetParser()
	{
	  CSharp30Parser parser = Parser as CSharp30Parser;
	  if (parser == null)
		parser = new CSharp30Parser();
	  return parser;
	}
	void UseLikeExpressionParser(Action<CSharp30Parser> action)
	{
	  bool shouldWorkAsExpressionParser = false;
	  CSharp30Parser parser = GetParser();
	  if (parser != null)
	  {
		shouldWorkAsExpressionParser = parser.ShouldWorkAsExpressionParser;
		parser.ShouldWorkAsExpressionParser = true;
	  }
	  try
	  {
		if (action != null)
		  action(parser);
	  }
	  finally
	  {
		if (parser != null)
		  parser.ShouldWorkAsExpressionParser = shouldWorkAsExpressionParser;
	  }
	}
	#region Parse
	public override Expression Parse(ISourceReader reader)
	{
	  Expression result = null;
	  UseLikeExpressionParser(delegate(CSharp30Parser parser)
	  {
		result = parser.ParseExpression(reader);
	  });
	  return result;
	}
	#endregion
	#region ParseTypeReferenceExpression
	public override TypeReferenceExpression ParseTypeReferenceExpression(ISourceReader reader)
	{
	  TypeReferenceExpression result = null;
	  UseLikeExpressionParser(delegate(CSharp30Parser parser)
	  {
		result = parser.ParseTypeReferenceExpression(reader);
	  });
	  return result;
	}
	#endregion
  }
}
