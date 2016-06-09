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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.Html
#else
namespace DevExpress.CodeParser.Html
#endif
{
  using Xml;
  using Css;
  using Diagnostics;
	using AspCodeEmbeddingClass = AspCodeEmbedding;
	public enum ScriptLanguageType
	{
		JavaScript,
		VBScript,
		Perl,
		EcmaScript,
		Unknown
	}
	public partial class HtmlParser : XmlLanguageParserBase
	{
	const int AspLanguageTokenBlock = -1;
	const int AspTokenBlockStart = TokenType.AspBlockStart;
	const int AspTokenBlockEnd = TokenType.AspBlockEnd;
	const int AspCommentTokenBlock = TokenType.AspCommentStatement;
	const int MaxElementNestingLevel = 500;
	const string STR_StartTag = "<%";
	const string STR_EndTag = "%>";
	ScriptLanguageType _DefaultScriptLanguage;
	DotNetLanguageType _DefaultDotNetLanguage;
	ParserBase _VbParser = null;
	CSharp.CSharp30Parser _CsExpressionParser;
   private int _FirstTokenStartPosition;
	protected int ElementNestingLevel;
		protected ExpressionCollection InlineExpressions;
		static Hashtable _OptionalEndTagElements;
		protected static Hashtable EmptyElements;
		protected static Hashtable HtmlElementTypes;
		protected List<SourceRange> BlockRanges;
		protected List<SourceRange> FictiveCommentRanges;
		protected SourceRange FictiveCommentStart;
		protected SourceRange SourceFileStartRange = SourceRange.Empty;
		protected SourceRange SourceFileEndRange = SourceRange.Empty;
		protected bool InsideAttribute;
		protected bool AspEmbCodeIsName;
	protected int PosOffset;
		protected int TopLevelReturnCount;
	protected static Hashtable EventAttributeNames;
		protected bool WorkAsXAMLParser;
	protected bool IsRazor;
	void InitializeParser()
	{
	  parserErrors = new HtmlParserErrors();
	  set = CreateSetArray();
	  maxTokens = Tokens.MaxTokens;
	}
		public HtmlParser(bool workAsXAMLParser)
			: this(workAsXAMLParser, DotNetLanguageType.Unknown)
		{
		}
		public HtmlParser(bool workAsXAMLParser, DotNetLanguageType codeEmbeddingDefaultLanguage) : this(false, workAsXAMLParser, codeEmbeddingDefaultLanguage)
		{
		}
	public HtmlParser(bool isRazorParser, bool workAsXAMLParser, DotNetLanguageType codeEmbeddingDefaultLanguage)
	{
	  WorkAsXAMLParser = workAsXAMLParser;
	  InitialDefaultDotNetLanguage = codeEmbeddingDefaultLanguage;
	  IsRazor = isRazorParser;
	  if (EmptyElements == null)
	  {
		EmptyElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		FillEmptyElementsTable();
	  }
	  if (HtmlElementTypes == null)
	  {
		HtmlElementTypes = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		FillHtmlElementTypes();
	  }
	  if (EventAttributeNames == null)
	  {
		EventAttributeNames = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		FillEventAttributeNames();
	  }
	  if (_OptionalEndTagElements == null)
	  {
		_OptionalEndTagElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
		FillOptionalEndElementsTable();
	  }
	  InlineExpressions = new ExpressionCollection();
	  InitializeParser();
	}
		#region FillOptionalEndElementsTable
		protected void FillOptionalEndElementsTable()
		{
			FillOptionalEndElementsForCOLGROUP();
			FillOptionalEndElementsForDD();
			FillOptionalEndElementsForDT();
			FillOptionalEndElementsForOPTION();
			FillOptionalEndElementsForTBODY();
			FillOptionalEndElementsForTFOOT();
			FillOptionalEndElementsForTHEAD();
			FillOptionalEndElementsForTR();
			FillOptionalEndElementsForTH();
			FillOptionalEndElementsForTD();
			FillOptionalEndElementsForLI();
		}
		#endregion
		#region FillOptionalEndElementsForCOLGROUP
		void FillOptionalEndElementsForCOLGROUP()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			validNestedElements.Add("COL", "COL");
			_OptionalEndTagElements.Add("COLGROUP", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForDD
		void FillOptionalEndElementsForDD()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddFlowElements(validNestedElements);
			_OptionalEndTagElements.Add("DD", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForDT
		void FillOptionalEndElementsForDT()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddInlineElements(validNestedElements);
			_OptionalEndTagElements.Add("DT", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForOPTION
		void FillOptionalEndElementsForOPTION()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			_OptionalEndTagElements.Add("OPTION", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForP
		void FillOptionalEndElementsForP()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddInlineElements(validNestedElements);
			_OptionalEndTagElements.Add("P", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTBODY
		void FillOptionalEndElementsForTBODY()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			validNestedElements.Add("TR", "TR");
			_OptionalEndTagElements.Add("TBODY", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTFOOT
		void FillOptionalEndElementsForTFOOT()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			validNestedElements.Add("TR", "TR");
			_OptionalEndTagElements.Add("TFOOT", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTHEAD
		void FillOptionalEndElementsForTHEAD()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			validNestedElements.Add("TR", "TR");
			_OptionalEndTagElements.Add("THEAD", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTR
		void FillOptionalEndElementsForTR()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			validNestedElements.Add("TH", "TH");
			validNestedElements.Add("TD", "TD");
			validNestedElements.Add("FORM", "FORM");
			validNestedElements.Add("NOBR", "NOBR");
			_OptionalEndTagElements.Add("TR", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTH
		void FillOptionalEndElementsForTH()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddFlowElements(validNestedElements);
			_OptionalEndTagElements.Add("TH", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForTD
		void FillOptionalEndElementsForTD()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddFlowElements(validNestedElements);
			_OptionalEndTagElements.Add("TD", validNestedElements);
		}
		#endregion
		#region FillOptionalEndElementsForLI
		void FillOptionalEndElementsForLI()
		{
			Hashtable validNestedElements = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			AddFlowElements(validNestedElements);
			_OptionalEndTagElements.Add("LI", validNestedElements);
		}
		#endregion
	public HtmlParser() : this(false)
	{
	}
	public LanguageElement Parse(ISourceReader reader)
	{
	  try
	  {
		ElementNestingLevel = 0;
		ClearDefaultLanguages();
		scanner = CreateScanner(reader);
		if (!(RootNode is SourceFile))
		  OpenContext(GetSourceFile("dsf"));
		Parse();
		if (RootNode != null)
		{
		  SourceFileStartRange = new SourceRange(RootNode.Range.Start);
		  SourcePoint endPoint = RootNode.Range.End;
		  if (endPoint.IsEmpty)
			endPoint = tToken.Range.End;
		  SourceFileEndRange = new SourceRange(endPoint);
		}
		PerformPostProcessing();
		return RootNode;
	  }
	  finally
	  {
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		_CsExpressionParser = null;
		CleanUpParser();
	  }
	}
	void AddComment(Comment comment)
	{
	  AddNode(comment);
	  var sourceFile = RootNode as SourceFile;
	  if (sourceFile != null)
		sourceFile.AddComment(comment);
	}
	bool IsHttpPath(string path)
	{
	  return path.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase);
	}
	private HtmlScanner CreateScanner(ISourceReader reader)
	{
	  return new HtmlScanner(reader, IsRazor);
	}
	void SplitRazorDerectiveTokenIfNeeded()
	{
	  if (!SetTokensCategory)
		return;
	  int count = SavedTokens.Count;
	  if (count == 0)
		return;
	  CategorizedToken token1 = SavedTokens[SavedTokens.Count - 1].Clone() as CategorizedToken;
	  CategorizedToken token2 = token1.Clone() as CategorizedToken;
	  SavedTokens.RemoveAt(count - 1);
	  token1.EndPosition = token1.StartPosition + 1;
	  token1.EndColumn = token1.Column + 1;
	  token1.Category = TokenCategory.HtmlServerSideScript;
	  token1.Value = "@";
	  SavedTokens.Add(token1);
	  token2.StartPosition = token2.StartPosition + 1;
	  token2.Column = token2.Column + 1;
	  token2.Value = token2.Value.Remove(0, 1);
	  SavedTokens.Add(token2);
	}
	private ISourceReader GetRazorReader(int htmlScannerPosition, int startLine, int startColumn)
	{
	  TextReader textReader = HtmlScanner.SourceReader.GetStream();
	  string text = textReader.ReadToEnd();
	  int spaceCount = 0;
	  foreach (char item in text)
	  {
		if (item == '\r' || item == '\n')
		{
		  spaceCount = 0;
		  break;
		}
		if (char.IsWhiteSpace(item))
		  spaceCount++;
		else
		  break;
	  }
	  int updatedScannerPosition = htmlScannerPosition  + spaceCount;
	  return HtmlScanner.SourceReader.GetSubStream(updatedScannerPosition, text.Length - updatedScannerPosition, startLine, startColumn);
	}
	private void ParseRazorText(string text, SourceRange textRange)
	{
	  HtmlParser htmlParser = new HtmlParser(true, false, _DefaultDotNetLanguage);
	  SourceStringReader reader = new SourceStringReader(text, textRange.Start.Line, textRange.Start.Offset);
	  LanguageElementCollection parseResult = htmlParser.ParseRazorTextLine(reader);
	  if (parseResult != null && Context != null)
		Context.AddNodes(parseResult);
	}
	public LanguageElementCollection ParseRazorTextLine(SourceStringReader reader)
	{
	  try
	  {
		ElementNestingLevel = 0;
		ClearDefaultLanguages();
		scanner = CreateScanner(reader);
		HtmlScanner.ShouldCheckForXmlText = true;
		if (!(RootNode is SourceFile))
		  OpenContext(GetSourceFile("dsf"));
		la = new Token();
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		la.Value = "";
		Get();
		_FirstTokenStartPosition = la.StartPosition;
		Parser();
		if (RootNode.NodeCount <= 0)
		  return null;
		LanguageElementCollection result = new LanguageElementCollection();
		for (int i = 0; i < RootNode.NodeCount; i++)
		{
		  LanguageElement currentNode = RootNode.Nodes[i] as LanguageElement;
		  if (currentNode != null)
			result.Add(currentNode);
		}
		return result;
	  }
	  finally
	  {
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		CleanUpParser();
	  }
	}
	public LanguageElementCollection ParseSingleElement(ISourceReader reader, out int scannerPositionDelta)
	{
	  try
	  {
		ElementNestingLevel = 0;
		ClearDefaultLanguages();
		scanner = CreateScanner(reader);
		if (!(RootNode is SourceFile))
		  OpenContext(GetSourceFile("dsf"));
		la = new Token();
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		la.Value = "";
		Get();
		_FirstTokenStartPosition = la.StartPosition;
		int startPosition = la.StartPosition;
		Element();
		int endPosition = la.StartPosition;
		scannerPositionDelta = endPosition - startPosition;
		if (RootNode.NodeCount <= 0)
		  return null;
		LanguageElementCollection result = new LanguageElementCollection();
		for (int i = 0; i < RootNode.NodeCount; i++)
		{
		  LanguageElement currentNode = RootNode.Nodes[i] as LanguageElement;
		  if (currentNode != null)
			result.Add(currentNode);
		}
		return result;
	  }
	  finally
	  {
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		CleanUpParser();
	  }
	}
	AspCodeEmbedding _StringEmbedding;
	public void ParseRazorInlineExpressionInString(int pos, ISourceReader sourceReader)
	{
	  TokenCategorizedParserBase parser = GetParserByLanguageType(_DefaultDotNetLanguage, false);
	  IRazorCodeParser razorParser = parser as IRazorCodeParser;
	  if (razorParser == null)
		return;
	  ISourceReader reader = GetRazorReader(pos - _FirstTokenStartPosition, la.EndLine, la.EndColumn + pos - la.EndPosition);
	  int scannerPositionDelta = -1;
	  LanguageElementCollection parseResult = razorParser.ParseRazorCode(reader, ref scannerPositionDelta);
	  if (_StringEmbedding == null)
		_StringEmbedding = new AspCodeEmbedding();
	  if (parseResult != null && parseResult.Count > 0)
		_StringEmbedding.AddDetailNodes(parseResult);
	  HtmlScanner.SyncPosition(scannerPositionDelta, true);
	}
	protected void ParseRazorFunctions()
	{
	  TokenCategorizedParserBase parser = GetParserByLanguageType(_DefaultDotNetLanguage, false);
	  IRazorCodeParser razorParser = parser as IRazorCodeParser;
	  if (razorParser == null)
		return;
	  ISourceReader reader = GetRazorReader(HtmlScanner.Position - 1 - _FirstTokenStartPosition, la.EndLine, la.EndColumn);
	  int scannerPositionDelta = -1;
	  LanguageElementCollection elements = razorParser.ParseRazorFunctions(reader, out scannerPositionDelta);
	  if (elements == null || elements.Count == 0)
		return;
	  RazorFunctions embedding = new RazorFunctions();
	  embedding.IsRazorEmbedding = true;
	  embedding.SetRange(la.Range);
	  AddNode(embedding);
	  embedding.AddDetailNodes(elements);
	  embedding.SetRange(GetRange(embedding, elements[elements.Count - 1]));
	  if (SetTokensCategory)
		foreach (CategorizedToken token in parser.SavedTokens)
		  SavedTokens.Add(token);
	  HtmlScanner.SyncPosition(scannerPositionDelta);
	}
	protected void ParseRazorHelper()
	{
	  TokenCategorizedParserBase parser = GetParserByLanguageType(_DefaultDotNetLanguage, false);
	  IRazorCodeParser razorParser = parser as IRazorCodeParser;
	  if (razorParser == null)
		return;
	  ISourceReader reader = GetRazorReader(HtmlScanner.Position - 1 - _FirstTokenStartPosition, la.EndLine, la.EndColumn);
	  int scannerPositionDelta = -1;
	  LanguageElement helper = razorParser.ParseRazorHelper(reader, out scannerPositionDelta);
	  RazorHelper embedding = new RazorHelper();
	  embedding.IsRazorEmbedding = true;
	  embedding.SetRange(la.Range);
	  AddNode(embedding);
	  if (helper != null)
	  {
		embedding.AddDetailNode(helper);
		embedding.SetRange(GetRange(embedding, helper));
		SourceRange range = helper.Range;
		if (SetTokensCategory)
		  foreach (CategorizedToken token in parser.SavedTokens)
			if (range.Contains(token.Range))
			  SavedTokens.Add(token);
	  }
	  HtmlScanner.SyncPosition(scannerPositionDelta);
	}
	protected void ParseRazorEmbedding()
	{
	  if (RootNode is SourceFile)
	  {
		SourceFile file = (SourceFile)RootNode;
		if (file.AspPageLanguage == DotNetLanguageType.Unknown)
		  file.AspPageLanguage = _DefaultDotNetLanguage;
	  }
	  TokenCategorizedParserBase parser = GetParserByLanguageType(_DefaultDotNetLanguage, false);
	  IRazorCodeParser razorParser = parser as IRazorCodeParser;
	  if (razorParser == null)
		return;
	  ISourceReader reader = GetRazorReader(HtmlScanner.Position - 1 - _FirstTokenStartPosition, la.EndLine, la.EndColumn);
	  int scannerPositionDelta = 0;
	  LanguageElementCollection parseResult = razorParser.ParseRazorCode(reader, ref scannerPositionDelta);
	  AspCodeEmbedding embedding = new AspCodeEmbedding();
	  if (tToken.Type == Tokens.RAZORSECTION)
	  {
		RazorSection section = new RazorSection();
		section.Name = la.Value;
		int startOffset = la.Range.Start.Offset;
		int startLine = la.Range.Start.Line;
		SourceRange sectionNameRange = new SourceRange(startLine, startOffset, startLine, startOffset + la.Value.Length);
		section.NameRange = sectionNameRange;
		section.DotNetLanguageType = _DefaultDotNetLanguage;
		embedding = section;
	  }
	  embedding.IsRazorEmbedding = true;
	  SourceRange firstTokenRange = la.Range;
	  if (tToken.Type == Tokens.RAZORSECTION)
		firstTokenRange = tToken.Range;
	  embedding.SetRange(firstTokenRange);
	  AddNode(embedding);
	  SourceRange resultRange = SourceRange.Empty;
	  if (parseResult != null && parseResult.Count > 0)
	  {
		embedding.SetRange(GetRange(embedding, parseResult[parseResult.Count - 1]));
		for (int i = 0; i < parseResult.Count; i++)
		{
		  LanguageElement element = parseResult[i];
		  embedding.AddDetailNode(element);
		}
		resultRange = new SourceRange(parseResult[0].Range.Start, parseResult[parseResult.Count - 1].Range.End);
	  }
	  HtmlScanner.SyncPosition(scannerPositionDelta);
	  if (parser.SetTokensCategory)
		foreach (CategorizedToken token in parser.SavedTokens)
		  if (resultRange.Contains(token.Range))
			SavedTokens.Add(token);
	}
	LanguageElement ParseWebHandler(ParserContext parserContext, ISourceReader reader)
	{
	  try
	  {
		ClearDefaultLanguages();
		ElementNestingLevel = 0;
		scanner = CreateScanner(reader);
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		string aspDirectiveString = String.Empty;
		SourceRange aspDirectiveRange = SourceRange.Empty;
		(scanner as HtmlScanner).GetAspDirectiveString(out aspDirectiveString, out aspDirectiveRange);
		SetupForAspDirectiveParsing(aspDirectiveString, aspDirectiveRange);
		AspDirectiveDef();
		ISourceReader dotNetReader = GetReaderForWebHandler(scanner as HtmlScannerBase, reader);
		ParseWebHandlerDotNetCode(parserContext, dotNetReader);
		if (Context != null)
		  Context.SetRange(GetRange(Context, tToken));
		CloseContext();
		if (RootNode != null)
		{
		  SourceFileStartRange = new SourceRange(RootNode.Range.Start);
		  SourceFileEndRange = new SourceRange(RootNode.Range.End);
		}
		return RootNode;
	  }
	  finally
	  {
		if (InlineExpressions != null)
		  InlineExpressions.Clear();
		CleanUpParser();
	  }
	}
	protected override LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
	{
	  LanguageElement context = parserContext.Context;
	  ElementNestingLevel = 0;
	  SourceFile fileNode = null;
	  if (context == null)
		return null;
	  SetRootNode(context);
	  if (context is SourceFile)
	  {
		fileNode = context as SourceFile;
		((SourceFile)context).SetDocument(parserContext.Document);
	  }
	  if (AspHelper.IsWebHandlerFile(fileNode))
		return ParseWebHandler(parserContext, reader);
	  else
		return Parse(reader);
	}
	#region AddBlockElements
	void AddBlockElements(Hashtable validNestedElements)
		{
			#region %block
			validNestedElements.Add("P","P");
			#region %heading
			validNestedElements.Add("H1","H1");
			validNestedElements.Add("H2","H2");
			validNestedElements.Add("H3","H3");
			validNestedElements.Add("H4","H4");
			validNestedElements.Add("H5","H5");
			validNestedElements.Add("H6","H6");
			#endregion
			#region %list
			validNestedElements.Add("UL","UL");
			validNestedElements.Add("OL","OL");
			#endregion
			#region %preformatting
			validNestedElements.Add("PRE","PRE");
			#endregion
			validNestedElements.Add("DL","DL");
			validNestedElements.Add("DIV","DIV");
			validNestedElements.Add("NOSCRIPT","NOSCRIPT");
			validNestedElements.Add("BLOCKQUOTE","BLOCKQUOTE");
			validNestedElements.Add("FORM","FORM");
			validNestedElements.Add("NOBR","NOBR");
			validNestedElements.Add("HR","HR");
			validNestedElements.Add("TABLE","TABLE");
			validNestedElements.Add("FIELDSET","FIELDSET");
			validNestedElements.Add("ADRESS","ADRESS");
			#endregion
		}
		#endregion
		#region AddInlineElements
		void AddInlineElements(Hashtable validNestedElements)
		{
			#region %inline
			#region %fontstyle
			validNestedElements.Add("TT","TT");
			validNestedElements.Add("I","I");
			validNestedElements.Add("B","B");
			validNestedElements.Add("BIG","SMALL");
			validNestedElements.Add("FONT","FONT");
	  validNestedElements.Add("U", "U");
			#endregion
			#region %phrase
			validNestedElements.Add("EM","EM");
			validNestedElements.Add("STRONG","STRONG");
			validNestedElements.Add("DFN","DFN");
			validNestedElements.Add("CODE","CODE");
			validNestedElements.Add("SAMP","SAMP");
			validNestedElements.Add("KBD","KBD");
			validNestedElements.Add("VAR","VAR");
			validNestedElements.Add("CITE","CITE");
			validNestedElements.Add("ABBR","ABBR");
			validNestedElements.Add("ACRONYM","ACRONYM");
			#endregion
			#region %special
			validNestedElements.Add("A","A");
			validNestedElements.Add("IMG","IMG");
			validNestedElements.Add("OBJECT","OBJECT");
			validNestedElements.Add("BR","BR");
			validNestedElements.Add("SCRIPT","SCRIPT");
			validNestedElements.Add("MAP","MAP");
			validNestedElements.Add("Q","Q");
			validNestedElements.Add("SUB","SUB");
			validNestedElements.Add("SUP","SUP");
			validNestedElements.Add("SPAN","SPAN");
			#endregion
			#region %formctrl
			validNestedElements.Add("INPUT","INPUT");
			validNestedElements.Add("SELECT","SELECT");
			validNestedElements.Add("TEXTAREA","TEXTAREA");
			validNestedElements.Add("LABEL","LABEL");
			validNestedElements.Add("BUTTON","BUTTON");
			#endregion
			#endregion
		}
		#endregion
		#region AddFlowElements
		void AddFlowElements(Hashtable validNestedElements)
		{
			AddBlockElements(validNestedElements);
			AddInlineElements(validNestedElements);
		}
		#endregion
	#region FillEventAttributeNames
	private void FillEventAttributeNames()
	{
	  EventAttributeNames.Add("onabort", "onabort");
	  EventAttributeNames.Add("onblur", "onblur");
	  EventAttributeNames.Add("onchange", "onchange");
	  EventAttributeNames.Add("onclick", "onclick");
	  EventAttributeNames.Add("ondblclick", "ondblclick");
	  EventAttributeNames.Add("onerror", "onerror");
	  EventAttributeNames.Add("onfocus", "onfocus");
	  EventAttributeNames.Add("onkeydown", "onkeydown");
	  EventAttributeNames.Add("onkeypress", "onkeypress");
	  EventAttributeNames.Add("onkeyup", "onkeyup");
	  EventAttributeNames.Add("onload", "onload");
	  EventAttributeNames.Add("onmousedown", "onmousedown");
	  EventAttributeNames.Add("onmousemove", "onmousemove");
	  EventAttributeNames.Add("onmouseout", "onmouseout");
	  EventAttributeNames.Add("onmouseover", "onmouseover");
	  EventAttributeNames.Add("onmouseup", "onmouseup");
	  EventAttributeNames.Add("onreset", "onreset");
	  EventAttributeNames.Add("onresize", "onresize");
	  EventAttributeNames.Add("onselect", "onselect");
	  EventAttributeNames.Add("onsubmit", "onsubmit");
	  EventAttributeNames.Add("onunload", "onunload");
	}
	#endregion
		#region GetDotNetLanguageFromString
		DotNetLanguageType GetDotNetLanguageFromString(string language)
		{
			string lowCaseLang = language.ToLower();
			if (lowCaseLang.IndexOf("csharp") != -1 || lowCaseLang.IndexOf("cs") != -1 || lowCaseLang.IndexOf("c#") != -1)
				return DotNetLanguageType.CSharp;
			if (lowCaseLang.IndexOf("vb") != -1 || lowCaseLang.IndexOf("vbasic") != -1 || lowCaseLang.IndexOf("visualbasic") != -1)
				return DotNetLanguageType.VisualBasic;
			return DotNetLanguageType.ScriptLanguage;
		}
		#endregion
		#region GetDefaultScriptFromVs
		void GetDefaultScriptFromVs(HtmlElement element)
		{
			string defaultLangAttributeValue = String.Empty;
			XmlAttribute name = element.GetAttribute("name", true);
			if (name != null && String.Compare(name.Value, "vs_defaultClientScript", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				XmlAttribute contentAttr = element.GetAttribute("content", true);
				if (contentAttr != null)
				{
					SetDefaultScriptLanguage(contentAttr.Value);
					return;
				}
			}
		}
		#endregion
		#region GetDefaultScriptFromHttpEq
		void GetDefaultScriptFromHttpEq(HtmlElement element)
		{
			string defaultLangAttributeValue = String.Empty;
			XmlAttribute httpEquiv = element.GetAttribute("http-equiv", true);
			if (httpEquiv != null && String.Compare(httpEquiv.Value, "Content-Script-Type", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				XmlAttribute contentAttr = element.GetAttribute("content", true);
				if (contentAttr != null)
				{
					SetDefaultScriptLanguage(contentAttr.Value);
					return;
				}
			}
		}
		#endregion
		#region SetDefaultScriptLanguage
		void SetDefaultScriptLanguage(string language)
		{
			if (language == null)
				return;
			string lowCaseLang = language.ToLower();
			if (lowCaseLang.IndexOf("javascript") != -1)			
			{
				_DefaultScriptLanguage = ScriptLanguageType.JavaScript;
			} 
			else
				if (lowCaseLang.IndexOf("ecmascript") != -1)			
			{
				_DefaultScriptLanguage = ScriptLanguageType.EcmaScript;
			} 
			else
				if (lowCaseLang.IndexOf("vbscript") != -1)			
			{
				_DefaultScriptLanguage = ScriptLanguageType.VBScript;
			} 
			else
				_DefaultScriptLanguage = ScriptLanguageType.Unknown;
		}
		#endregion
		#region SetDefaultLanguage
		void SetDefaultLanguage(string language)
		{
			if (language == null)
				return;
			_DefaultDotNetLanguage = GetDotNetLanguageFromString(language);
			if (_DefaultDotNetLanguage == DotNetLanguageType.ScriptLanguage)
				_DefaultDotNetLanguage = DotNetLanguageType.Unknown;
		}
		#endregion
		#region GetInlineCodeStart
		int GetInlineCodeStart(string text)
		{
	  if (WorkAsXAMLParser)
		return text.IndexOf('{');
			string chars = String.Empty;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
					case ' ':
					case '\r':
					case '\n':
					case '\t':
						continue;
					case '<':
						if (chars == String.Empty)
							chars = "<";
						else
							return -1;
						break;
					case '%':
						if (chars == "<")
							chars = STR_StartTag;
						else
							return -1;
						break;
					case '=':
					case ':':
					case '#':
						if (chars == STR_StartTag)
							return i + 1;
						else
							return -1;		
				}
			}
			return -1;
		}
		#endregion
	CSharp.CSharp30Parser GetCsParser()
	{
	  CSharp.CSharp30Parser csParser = new CSharp.CSharp30Parser();
	  csParser.WorkAsCharp30Parser = true;
	  return csParser;
	}
		#region GetParserByLanguageType
		TokenCategorizedParserBase GetParserByLanguageType(DotNetLanguageType languageType, bool aspxExpressionParser)
		{
			TokenCategorizedParserBase parser = null;
	  if (languageType == DotNetLanguageType.CSharp)
	  {
		if (aspxExpressionParser)
		{
		  if (_CsExpressionParser == null)
			_CsExpressionParser = GetCsParser();
		  parser = _CsExpressionParser;
		}
		else
		  parser = GetCsParser();
	  }
	  else if (languageType == DotNetLanguageType.VisualBasic)
	  {
		if (IsRazor)
		  return new VB.VB10Parser();
		if (_VbParser == null)
		{
		  _VbParser = new VB.VB10Parser();
		}
		parser = _VbParser as TokenCategorizedParserBase;
	  }
	  if (WorkAsXAMLParser)
		parser = new Xaml.MarkupExtensionParser();
	  if (parser != null)
		parser.SetTokensCategory = SetTokensCategory;
			return parser;
		}
		#endregion
	#region ClearFictiveCommentStart
	void ClearFictiveCommentStart()
	{
	  FictiveCommentStart = SourceFileStartRange;
	}
	#endregion
	#region SetLastFictiveComment
	void SetLastFictiveComment()
	{
	  SourceRange newFictiveCommentRange = new SourceRange(FictiveCommentStart.Start, SourceFileEndRange.End);
	  FictiveCommentRanges.Add(newFictiveCommentRange);
	}
	#endregion
	#region AddFictiveCommentRange
	void AddFictiveCommentRange(SourceRange codeEmbeddingRange)
	{
	  if (codeEmbeddingRange.IsEmpty)
		return;
	  if (FictiveCommentRanges == null)
		FictiveCommentRanges = new List<SourceRange>();
	  SourceRange newFictiveCommentRange = new SourceRange(FictiveCommentStart.Start, codeEmbeddingRange.Start);
	  FictiveCommentRanges.Add(newFictiveCommentRange);
	  FictiveCommentStart = new SourceRange(codeEmbeddingRange.End);
	}
	#endregion
	#region CheckForInlineExpression
	void CheckForInlineExpression(ArrayList codeEmbeddings)
	{
	  if (codeEmbeddings == null || codeEmbeddings.Count == 0)
		return;
	  ClearFictiveCommentStart();
	  int i = 0;
	  while (i < codeEmbeddings.Count)
	  {
		AspCodeEmbedding currentCodeEmbedding = codeEmbeddings[i] as AspCodeEmbedding;
		if (currentCodeEmbedding == null)
		  i++;
		int equalityPos = GetEqualityOrHashPos(currentCodeEmbedding.Code);
		if (equalityPos != -1)
		{
		  codeEmbeddings.RemoveAt(i);
		  string inlineCode = currentCodeEmbedding.Code.Substring(equalityPos, currentCodeEmbedding.Code.Length - equalityPos);
		  int startLine = currentCodeEmbedding.Range.Start.Line;
		  int startOffset = currentCodeEmbedding.Range.Start.Offset + equalityPos + 2;
					Expression expression = ParseInlineExpression(inlineCode, startLine, startOffset, currentCodeEmbedding.CodeEmbeddingToken);
		  if (expression != null)
		  {
			Expression clonedExpression = expression.Clone() as Expression;
			InlineExpressions.Add(clonedExpression);
			currentCodeEmbedding.AddDetailNode(expression);
			AddFictiveCommentRange(expression.Range);
		  }
		}
		else
		{
		  i++;
		  AddFictiveCommentRange(currentCodeEmbedding.CodeRange);
		}
	  }
	  SetLastFictiveComment();
	}
	#endregion
	#region GetEqualityOrHashPos
	int GetEqualityOrHashPos(string text)
	{
	  if (text == null || text.Length == 0)
		return -1;
	  for (int i = 0; i < text.Length; i++)
	  {
		switch (text[i])
		{
		  case ' ':
		  case '\r':
		  case '\n':
		  case '\t':
			continue;
		  case '=':
		  case '#':
					case ':':
			return i + 1;
		  default:
			return -1;
		}
	  }
	  return -1;
	}
	#endregion
	#region GetFictiveString
	string GetFictiveString(SourceRange rangeStart, SourceRange rangeEnd)
	{
	  int lineDif = rangeEnd.Start.Line - rangeStart.End.Line;
	  int offsetDif = rangeEnd.Start.Offset + 1;
	  string lineDifCompensation = lineDif < 0 ? String.Empty : new String('\n', lineDif);
	  string offsetDifCompensation = offsetDif < 0 ? String.Empty : new String(' ', offsetDif);
	  return lineDifCompensation + offsetDifCompensation;
	}
	#endregion
	#region GetAspCodeEmbeddingsFromAttributes
	ArrayList GetAspCodeEmbeddingsFromAttributes(HtmlAttributeCollection attributes, ref SourceRange codeBlockRange)
	{
	  if (attributes == null)
		return null;
	  bool shouldAddBlock = attributes["runat"] != null;
	  ArrayList result = new ArrayList();
	  for (int i = 0; i < attributes.Count; i++)
	  {
		HtmlAttribute attr = attributes[i];
		if (attr == null)
		  continue;
		ArrayList attrCodeEmbs = GetAspCodeEmbeddingsFromAttribute(attr, ref codeBlockRange, shouldAddBlock);
		if (attrCodeEmbs != null)
		{
		  result.AddRange(attrCodeEmbs);
		}
	  }
	  if (result.Count > 0)
		return result;
	  return null;
	}
	#endregion
	#region GetAspCodeEmbeddingsFromAttribute
	ArrayList GetAspCodeEmbeddingsFromAttribute(HtmlAttribute node, ref SourceRange codeBlockRange, bool shouldAddBlock)
	{
	  if (node == null || node.DetailNodeCount == 0)
		return null;
	  ArrayList result = new ArrayList();
	  for (int i = 0; i < node.DetailNodeCount; i++)
	  {
		LanguageElement currentNode = node.DetailNodes[i] as AspCodeEmbedding;
		if (currentNode == null)
		  continue;
		if (shouldAddBlock)
		{
		  if (codeBlockRange == SourceRange.Empty)
			codeBlockRange = currentNode.Range;
		  else
			codeBlockRange = GetRange(codeBlockRange, currentNode);
		}
		result.Add(currentNode as AspCodeEmbedding);
	  }
	  if (result.Count > 0)
		return result;
	  return null;
	}
	#endregion
	#region CanGetAspEmbCode
	bool CanGetAspEmbCode(LanguageElement node)
	{
	  if (node == null)
		return false;
	  if (node is HtmlElement)
	  {
		if (node.NodeCount == 0 && node.DetailNodeCount == 0)
		  return false;
	  }
	  else
	  {
		if (node.NodeCount == 0)
		  return false;
	  }
	  return true;
	}
	#endregion
	#region GetAspCodeEmbeddingsFromNode
	ArrayList GetAspCodeEmbeddingsFromNode(ArrayList runAtServer, ArrayList nodes, LanguageElement node)
	{
	  if (!CanGetAspEmbCode(node))
		return null;
	  ElementNestingLevel++;
	  try
	  {
		if (ShouldPreventCycling())
		  return null;
		HtmlElement htmlNode = node as HtmlElement;
		bool shouldAddBlock = htmlNode != null && htmlNode.Attributes["runat"] != null;
		if (shouldAddBlock)
		  runAtServer.Add(htmlNode);
		else
		  nodes.Add(htmlNode);
		SourceRange codeBlockRange = SourceRange.Empty;
		ArrayList result = null;
		if (htmlNode != null)
		{
		  result = GetAspCodeEmbeddingsFromAttributes(htmlNode.Attributes, ref codeBlockRange);
		  for (int i = 0; i < htmlNode.DetailNodeCount; i++)
		  {
			AspCodeEmbedding currentNode = htmlNode.DetailNodes[i] as AspCodeEmbedding;
			if (currentNode == null)
			  continue;
			if (result == null)
			  result = new ArrayList();
			result.Add(currentNode);
			ArrayList currentNodeEmbeddings = GetAspCodeEmbeddingsFromNode(runAtServer, nodes, currentNode);
			if (currentNodeEmbeddings != null)
			  result.AddRange(currentNodeEmbeddings);
		  }
		}
		if (result == null)
		  result = new ArrayList();
		for (int i = 0; i < node.NodeCount; i++)
		{
		  LanguageElement currentNode = node.Nodes[i] as LanguageElement;
		  if (currentNode == null)
			continue;
		  if (currentNode is AspCodeEmbedding)
		  {
			if (shouldAddBlock)
			{
			  if (codeBlockRange == SourceRange.Empty)
				codeBlockRange = currentNode.Range;
			  else
				codeBlockRange = GetRange(codeBlockRange, currentNode);
			}
			result.Add(currentNode as AspCodeEmbedding);
		  }
		  ArrayList currentNodeEmbeddings = GetAspCodeEmbeddingsFromNode(runAtServer, nodes, currentNode);
		  if (currentNodeEmbeddings != null)
			result.AddRange(currentNodeEmbeddings);
		}
		if (codeBlockRange.IsEmpty == false)
		  BlockRanges.Insert(0, codeBlockRange);
		if (result.Count > 0)
		  return result;
		else
		  return null;
	  }
	  finally
	  {
		ElementNestingLevel--;
	  }
	}
	#endregion
	#region ParseInlineExpression
	Expression ParseInlineExpression(string inlineCode, int startLine, int startOffset, Token codeEmbeddingToken)
	{
	  try
	  {
		ParserBase parser = GetParserByLanguageType(DefaultDotNetLanguage, true);
				TokenCategorizedParserBase categorizedParser = parser as TokenCategorizedParserBase;
		if (parser == null)
		  return null;
				if (SetTokensCategory && categorizedParser != null)
					categorizedParser.SetTokensCategory = true;
		ISourceReader reader = new SourceStringReader(inlineCode, startLine, startOffset);
		ExpressionParserBase expressionParser = parser.CreateExpressionParser();
		Expression expression = expressionParser.Parse(reader);
		return expression;
	  }
	  catch (Exception ex)
	  {
		Log.SendException(ex);
		return null;
	  }
	}
	#endregion
	private void ReplaceEmbeddingToken(Token codeEmbeddingToken, TokenCollection embeddingTokens)
	{
	  if (codeEmbeddingToken == null || embeddingTokens == null || embeddingTokens.Count < 1)
		return;
	  if (SavedTokens == null)
		return;
	  int codeEmbeddingTokenPosition = SavedTokens.IndexOf(codeEmbeddingToken);
	  if (codeEmbeddingTokenPosition == -1)
		return;
	  Token firstToken = embeddingTokens[0];
	  SavedTokens.Remove(codeEmbeddingToken);
	  int startPositionAddition = codeEmbeddingToken.StartPosition + codeEmbeddingToken.Value.IndexOf(firstToken.Value) - firstToken.StartPosition;
	  for (int i = 0; i < embeddingTokens.Count; i++)
	  {
		Token currentToken = embeddingTokens[embeddingTokens.Count - i - 1];
		currentToken.StartPosition += startPositionAddition;
		currentToken.EndPosition += startPositionAddition;
		SavedTokens.Insert(codeEmbeddingTokenPosition, currentToken);
	  }
	  if (WorkAsXAMLParser)
	  {
		CategorizedToken startToken = new CategorizedToken(TokenLanguage.Html);
		startToken.Category = TokenCategory.HtmlAttributeValue;
		startToken.Value = codeEmbeddingToken.Value[0].ToString();
		startToken.StartPosition = codeEmbeddingToken.StartPosition;
		startToken.EndPosition = startToken.StartPosition + 1;
		startToken.Line = codeEmbeddingToken.Line;
		startToken.EndLine = codeEmbeddingToken.Line;
		startToken.Column = codeEmbeddingToken.Column;
		startToken.EndColumn = codeEmbeddingToken.Column + 1;
		SavedTokens.Insert(codeEmbeddingTokenPosition, startToken);
		CategorizedToken endToken = new CategorizedToken(TokenLanguage.Html);
		endToken.Category = TokenCategory.HtmlAttributeValue;
		endToken.Value = codeEmbeddingToken.Value[codeEmbeddingToken.Value.Length - 1].ToString();
		endToken.StartPosition = codeEmbeddingToken.EndPosition - 1;
		endToken.EndPosition = codeEmbeddingToken.EndPosition;
		endToken.Line = codeEmbeddingToken.EndLine;
		endToken.EndLine = codeEmbeddingToken.EndLine;
		endToken.Column = codeEmbeddingToken.EndColumn - 1;
		endToken.EndColumn = codeEmbeddingToken.EndColumn;
		SavedTokens.Insert(codeEmbeddingTokenPosition + embeddingTokens.Count + 1, endToken);
	  }
	}
		#region GetCodeEmbeddingsText
		string GetCodeEmbeddingsText(ArrayList aspTokenBlocks, ArrayList aspCodeEmbeddings, ArrayList runAtServerNodes, ArrayList nodes, ref int startLine, ref int startOffset)
	{
	  AspCodeEmbedding currentNode = null;
	  AspCodeEmbedding previousNode = null;
	  StringBuilder builder = new StringBuilder();
	  for (int i = 0; i < aspCodeEmbeddings.Count; i++)
	  {
		currentNode = aspCodeEmbeddings[i] as AspCodeEmbedding;
		if (currentNode == null)
		  continue;
		if (previousNode == null)
		{
		  startLine = currentNode.Range.Start.Line;
		  startOffset = currentNode.Range.Start.Offset + 2;
		  builder.AppendFormat("{0}", currentNode.Code);
		  aspTokenBlocks.Add(new TokenBlock(currentNode.Range, AspLanguageTokenBlock));
		}
		else
		{
		  builder.AppendFormat("{0}{1}", GetFictiveString(previousNode.Range, currentNode.Range), currentNode.Code);
		  SourceRange commentRange = new SourceRange(previousNode.Range.End, currentNode.Range.Start);
		  aspTokenBlocks.Add(new TokenBlock(commentRange, AspCommentTokenBlock));
		  aspTokenBlocks.Add(new TokenBlock(currentNode.Range, AspLanguageTokenBlock));
		}
		previousNode = currentNode;
	  }
	  return builder.ToString();
	}
	#endregion
	#region SplitAspTokenBlocks
	void SplitAspTokenBlocks(ArrayList aspTokenBlocks, ArrayList runAtServerNodes)
	{
	  if (runAtServerNodes == null)
		return;
	  int count = runAtServerNodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		HtmlElement node = runAtServerNodes[i] as HtmlElement;
		SplitStartAspTokenBlock(aspTokenBlocks, node);
		SplitEndAspTokenBlock(aspTokenBlocks, node);
	  }
	}
	#endregion
	#region FindAspTokenBlock
	int FindAspTokenBlock(ArrayList aspTokenBlocks, SourcePoint point)
	{
	  if (aspTokenBlocks == null || point.IsEmpty)
		return -1;
	  int count = aspTokenBlocks.Count;
	  for (int i = 0; i < count; i++)
	  {
		TokenBlock block = aspTokenBlocks[i] as TokenBlock;
		if (block == null || !block.Range.Contains(point))
		  continue;
		return i;
	  }
	  return -1;
	}
	#endregion
	#region SplitStartAspTokenBlock
	void SplitStartAspTokenBlock(ArrayList aspTokenBlocks, HtmlElement node)
	{
	  if (!node.HasCloseTag)
		return;
	  int index = FindAspTokenBlock(aspTokenBlocks, node.Range.Start);
	  if (index < 0)
	  {
		if (aspTokenBlocks.Count > 0)
		{
		  TokenBlock firstBlock = (TokenBlock)aspTokenBlocks[0];
		  if (node.Range.Start < firstBlock.Range.Start)
		  {
			SourceRange beforeStart = new SourceRange(node.Range.Start, node.InnerRange.Start);
			SourceRange beforeComment = new SourceRange(node.InnerRange.Start, firstBlock.Range.Start);
			if (!beforeComment.IsEmpty)
			  aspTokenBlocks.Insert(0, new TokenBlock(beforeComment, AspCommentTokenBlock));
			aspTokenBlocks.Insert(0, new TokenBlock(beforeStart, AspTokenBlockStart));
		  }
		}
		return;
	  }
	  TokenBlock block = (TokenBlock)aspTokenBlocks[index];
	  SourceRange nodeStartTagRange = new SourceRange(node.Range.Start, node.InnerRange.Start);
	  SourceRange firstRange = new SourceRange(block.Range.Start, nodeStartTagRange.Start);
	  SourceRange secondRange = new SourceRange(nodeStartTagRange.End, block.Range.End);
	  aspTokenBlocks.RemoveAt(index);
	  if (!secondRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(secondRange, AspCommentTokenBlock));
	  if (!nodeStartTagRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(nodeStartTagRange, AspTokenBlockStart));
	  if (!firstRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(firstRange, AspCommentTokenBlock));
	}
	#endregion
	#region SplitEndAspTokenBlock
	void SplitEndAspTokenBlock(ArrayList aspTokenBlocks, HtmlElement node)
	{
	  if (!node.HasCloseTag)
		return;
	  int index = FindAspTokenBlock(aspTokenBlocks, node.Range.End);
	  if (index < 0)
	  {
		if (aspTokenBlocks.Count > 0)
		{
		  TokenBlock lastBlock = (TokenBlock)aspTokenBlocks[aspTokenBlocks.Count - 1];
		  if (node.Range.End > lastBlock.Range.End)
		  {
			SourceRange afterEnd = new SourceRange(node.InnerRange.End, node.Range.End);
			SourceRange afterComment = new SourceRange(lastBlock.Range.End, node.InnerRange.End);
			if (!afterComment.IsEmpty)
			  aspTokenBlocks.Add(new TokenBlock(afterComment, AspCommentTokenBlock));
			aspTokenBlocks.Add(new TokenBlock(afterEnd, AspTokenBlockEnd));
		  }
		}
		return;
	  }
	  TokenBlock block = (TokenBlock)aspTokenBlocks[index];
	  SourceRange nodeEndTagRange = new SourceRange(node.InnerRange.End, node.Range.End);
	  SourceRange firstRange = new SourceRange(block.Range.Start, nodeEndTagRange.Start);
	  SourceRange secondRange = new SourceRange(nodeEndTagRange.End, block.Range.End);
	  aspTokenBlocks.RemoveAt(index);
	  if (!secondRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(secondRange, AspCommentTokenBlock));
	  if (!nodeEndTagRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(nodeEndTagRange, AspTokenBlockEnd));
	  if (!firstRange.IsEmpty)
		aspTokenBlocks.Insert(index, new TokenBlock(firstRange, AspCommentTokenBlock));
	}
	#endregion
	#region SplitAspCommentBlocks
	void SplitAspCommentBlocks(ArrayList aspTokenBlocks, ArrayList nodes)
	{
	  if (nodes == null)
		return;
	  int count = aspTokenBlocks.Count;
	  TokenBlock[] blockArray = new TokenBlock[count];
	  aspTokenBlocks.CopyTo(blockArray);
	  for (int i = 0; i < count; i++)
	  {
		TokenBlock block = blockArray[i] as TokenBlock;
		if (block == null || block.Type != AspCommentTokenBlock)
		  continue;
		ArrayList blockNodes = GetNodesInRange(nodes, block.Range);
		if (blockNodes == null || blockNodes.Count == 0)
		  aspTokenBlocks.Remove(block);
		if (blockNodes.Count < 2)
		  continue;
		SplitAspCommentBlock(aspTokenBlocks, block, blockNodes);
	  }
	}
	#endregion
	#region GetNodesInRange
	ArrayList GetNodesInRange(ArrayList nodes, SourceRange range)
	{
	  if (nodes == null)
		return null;
	  ArrayList result = new ArrayList();
	  int count = nodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement node = nodes[i] as LanguageElement;
		if (node == null)
		  continue;
		if (range.Contains(node.Range))
		  result.Add(node);
	  }
	  return result;
	}
	#endregion
	#region SplitAspCommentBlock
	void SplitAspCommentBlock(ArrayList aspTokenBlocks, TokenBlock block, ArrayList nodes)
	{
	  if (block == null)
		return;
	  if (aspTokenBlocks == null || nodes == null)
		return;
	  int index = aspTokenBlocks.IndexOf(block);
	  if (index < 0)
		return;
	  aspTokenBlocks.RemoveAt(index);
	  int count = nodes.Count;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement node = nodes[i] as LanguageElement;
		if (node == null)
		  continue;
		TokenBlock newBlock = new TokenBlock(node.Range, AspCommentTokenBlock);
		aspTokenBlocks.Insert(index + i, newBlock);
	  }
	}
	#endregion
	#region InsertBlockNodes
	void InsertBlockNodes(LanguageElement rootNode, NodeList blockNodes, int insertAtIndex)
	{
	  NestedMethod block = new NestedMethod();
	  for (int i = 0; i < blockNodes.Count; i++)
	  {
		rootNode.RemoveNode(blockNodes[i] as LanguageElement);
		block.AddNode(blockNodes[i] as LanguageElement);
	  }
	  block.SetRange(GetRange(blockNodes[0], blockNodes[blockNodes.Count - 1]));
	  rootNode.InsertNode(insertAtIndex, block);
	}
	#endregion
	#region InsertCodeBlocks
	void InsertCodeBlocks(LanguageElement rootNode)
	{
	  if (rootNode == null || BlockRanges == null || BlockRanges.Count == 0)
		return;
	  int currentBlockRangeIndex = 0;
	  int rangesChecked = 0;
	  int rangesCount = BlockRanges.Count;
	  while (rangesCount > rangesChecked)
	  {
		SourceRange currentBlockRange = BlockRanges[currentBlockRangeIndex];
		NodeList blockNodes = new NodeList();
		int insertAtIndex = -1;
		GetBlockNodes(blockNodes, rootNode, currentBlockRange, out insertAtIndex);
		if (blockNodes.Count > 0)
		{
		  InsertBlockNodes(rootNode, blockNodes, insertAtIndex);
		  BlockRanges.RemoveAt(currentBlockRangeIndex);
		}
		else
		  currentBlockRangeIndex++;
		rangesChecked++;
	  }
	  if (BlockRanges.Count != 0)
		for (int i = 0; i < rootNode.NodeCount; i++)
		  InsertCodeBlocks(rootNode.Nodes[i] as LanguageElement);
	}
	#endregion	
	#region CreateJavaScriptParser
	private ParserBase CreateJavaScriptParser()
	{
	  ParserBase parser = null;
	  parser = new JavaScript.JavaScriptParser();
	  return parser;
	}
	#endregion
	#region IsScriptBlockWithoutLanguageInformation
	private bool IsScriptBlockWithoutLanguageInformation(HtmlScriptDefinition script)
	{
	  if (script == null)
		return false;
	  if (script.GetAttribute("language", true) != null || script.GetAttribute("type", true) != null)
		return false;
	  if (_DefaultScriptLanguage != ScriptLanguageType.Unknown)
		return false;
	  return true;
	}
	#endregion
	#region GetClientSideParser
	private ParserBase GetClientSideParser(HtmlScriptDefinition script)
	{
	  if (script != null && (script.Language == DotNetLanguageType.JavaScript || IsScriptBlockWithoutLanguageInformation(script)))
		return CreateJavaScriptParser();
	  return null;
	}
	#endregion
	#region SetClientSideScriptTypeByAttributeValue
	private void SetClientSideScriptTypeByAttributeValue(string value, HtmlScriptDefinition script)
	{
	  if (String.IsNullOrEmpty(value) || script == null)
		return;
	  if (value.IndexOf("javascript", StringComparison.InvariantCultureIgnoreCase) != -1 || value.IndexOf("jscript", StringComparison.InvariantCultureIgnoreCase) != -1)
		script.Language = DotNetLanguageType.JavaScript;
	}
	#endregion
	#region ParseClientSideScript
	private void ParseClientSideScript(HtmlScriptDefinition script)
	{
	  ParserBase parser = GetClientSideParser(script);
	  if (parser == null)
		return;
	  PosOffset = la.EndPosition - la.EndColumn;
	  ParseEmbeddedCode(script, parser, RemoveHtmlCommentEndIfNeeded(script.ScriptText), script.CodeRange.Start);
	}
	#endregion
	#region RemoveHtmlCommentStartIfNeeded
	private string RemoveHtmlCommentEndIfNeeded(string scriptText)
	{
	  string result = scriptText;
	  if (String.IsNullOrEmpty(scriptText))
		return result;
	  string htmlCommentEnd = "-->";
	  result = result.TrimEnd();
	  if (result.EndsWith(htmlCommentEnd))
	  {
		int endNewLinePos = result.LastIndexOf("\r\n");
		if (endNewLinePos < 0)
		  endNewLinePos = result.LastIndexOf("\n");
		if (endNewLinePos < 0)
		  endNewLinePos = result.LastIndexOf("\r");
		if (endNewLinePos > -1)
		  result = result.Remove(endNewLinePos);
	  }
	  return result;
	}
	#endregion
	#region GetRootNodeForScript
	private LanguageElement GetRootNodeForScript(ParserBase parser, string scriptText, SourcePoint strtPoint)
	{
	  TokenCategorizedParserBase tokenCategorized = parser as TokenCategorizedParserBase;
	  if (tokenCategorized != null)
		tokenCategorized.SetTokensCategory = SetTokensCategory;
	  LanguageElement root = null;
	  if (parser != null)
	  {
		int startLine = strtPoint.Line;
		if (!string.IsNullOrEmpty(scriptText) && scriptText[0] == 10)
		  startLine--;
		root = parser.ParseString(scriptText, startLine, strtPoint.Offset);
	  }
	  if (SetTokensCategory && tokenCategorized != null && root != null)
	  {
		if (root is SourceFile && root.NodeCount == 0)
		  return root;
		foreach (CategorizedToken token in tokenCategorized.SavedTokens)
		{
		  token.StartPosition += PosOffset;
		  token.EndPosition += PosOffset;
		  if (token.Category != TokenCategory.Unknown)
			SavedTokens.Add(token);
		}
	  }
	  return root;
	}
	#endregion
	#region ParseEmbeddedCode(LanguageElement element, ParserBase parser, string scriptText, SourcePoint strtPoint)
	private void ParseEmbeddedCode(LanguageElement element, ParserBase parser, string scriptText, SourcePoint strtPoint)
	{
	  LanguageElement root = GetRootNodeForScript(parser, scriptText, strtPoint);
	  try
	  {
		if (root != null)
		{
		  if (root is SourceFile)
			for (int i = 0; i < root.NodeCount; i++)
			  element.AddNode(root.Nodes[i] as LanguageElement);
		  else
			element.AddNode(root);
		}
	  }
	  catch (Exception ex)
	  {
		Log.SendException(ex);
	  }
	}
	#endregion
	#region IsScriptLanguage
	private bool IsScriptLanguage(DotNetLanguageType currentLanguage)
	{
	  return currentLanguage == DotNetLanguageType.ScriptLanguage || currentLanguage == DotNetLanguageType.JavaScript;
	}
	#endregion
	#region ParseServerSideScript
	private void ParseServerSideScript(HtmlScriptDefinition script)
	{
	  DotNetLanguageType currentLanguage = script.Language;
	  if (IsScriptLanguage(currentLanguage))
		return;
	  if (currentLanguage == DotNetLanguageType.Unknown)
	  {
		currentLanguage = _DefaultDotNetLanguage;
		if (currentLanguage != DotNetLanguageType.Unknown)
		  script.Language = currentLanguage;
	  }
	  PosOffset = la.EndPosition - la.EndColumn;
	  ParseEmbeddedCode(script, GetParserByLanguageType(currentLanguage, false), RemoveHtmlCommentEndIfNeeded(script.ScriptText), script.CodeRange.Start);
	}
	#endregion
	#region ParseScript
		protected void ParseScript(HtmlScriptDefinition script)
		{
	  if (script == null)
		return;
	  if (script.Attributes["runat"] == null || IsScriptLanguage(script.Language))
		ParseClientSideScript(script);
	  else
		ParseServerSideScript(script);
		}
		#endregion
	#region ParseStyleDefinition
	protected void ParseStyleDefinition(HtmlStyleDefinition styleTag)
	{
	  if (styleTag == null)
		return;
	  PosOffset = tToken.StartPosition - tToken.Column;
	  LanguageElement element = GetRootNodeForScript(new CssParser(), styleTag.StyleText, styleTag.TextRange.Start);
	  if (element != null)
	  {
		if (element is SourceFile)
		  foreach (LanguageElement el in element.Nodes)
			if (el != null)
			  styleTag.AddNode(el);
		else
		  styleTag.AddNode(element);
	  }
	}
	#endregion
	#region GetAttributeValue
	private string GetAttributeValue(AspDirective directive, string attributeName)
	{
	  if (directive == null || String.IsNullOrEmpty(attributeName))
		return String.Empty;
	  HtmlAttribute attribute = directive.Attributes[attributeName];
	  if (attribute == null)
		return String.Empty;
	  return attribute.Value;
	}
	#endregion
	#region SetCodeBehindFileName
	private void SetCodeBehindFileName(SourceFile fileNode, AspDirective directive)
	{
	  if (fileNode == null || directive == null)
		return;
	  string codeBehindFileName = GetAttributeValue(directive, "codebehind");
	  if (String.IsNullOrEmpty(codeBehindFileName))
		codeBehindFileName = GetAttributeValue(directive, "codefile");
	  if (!String.IsNullOrEmpty(codeBehindFileName))
		fileNode.CodeBehindFileName = codeBehindFileName;
	}
	#endregion
	#region SetAspPageBaseType
	private void SetAspPageBaseType(SourceFile rootFile, AspDirective directive)
	{
	  if (rootFile == null || directive == null)
		return;
	  string inherits = GetAttributeValue(directive, "inherits");
	  if (!String.IsNullOrEmpty(inherits))
		rootFile.AspPageBaseType = inherits;
	  if(directive.ElementType == LanguageElementType.RazorModelDirective)
		rootFile.SetModelTypeName(((RazorModelDirective)directive).Model);
	  else
		rootFile.SetModelTypeName(ExtractModelFromPageOrControlDirective(directive));
	}
	#endregion
	public string ExtractModelFromPageOrControlDirective(LanguageElement modelDirective)
	{
	  if (modelDirective == null)
		return string.Empty;
	  if (modelDirective.ElementType == LanguageElementType.PageDirective || modelDirective.ElementType == LanguageElementType.ControlDirective)
	  {
		string value = GetAttributeValue(modelDirective as AspDirective, "inherits");
		if (string.IsNullOrEmpty(value))
		  return string.Empty;
		int start = value.IndexOf("<", 0);
		int end = value.LastIndexOf(">");
		if (start < 0 || end < 0)
		  return string.Empty;
		return value.Substring(start + 1, end - start - 1);
	  }
	  return string.Empty;
	}
	#region InsertFictiveCommentInProperPlace
	void InsertFictiveCommentInProperPlace(LanguageElement root, SourceRange fictiveCommentRange)
	{
	  if (root == null || fictiveCommentRange.IsEmpty)
		return;
	  for (int i = 0; i < root.NodeCount; i++)
	  {
		LanguageElement currentNode = root.Nodes[i] as LanguageElement;
		if (currentNode == null)
		  continue;
		if (currentNode.Range.Start >= fictiveCommentRange.End)
		{
		  root.InsertNode(i, CreateFictiveComment(fictiveCommentRange));
		  return;
		}
		if (fictiveCommentRange.Start >= currentNode.Range.Start && fictiveCommentRange.End <= currentNode.Range.End)
		{
		  InsertFictiveCommentInProperPlace(currentNode, fictiveCommentRange);
		  return;
		}
	  }
	  root.AddNode(CreateFictiveComment(fictiveCommentRange));
	}
	#endregion
	#region CreateFictiveComment
	LanguageElement CreateFictiveComment(SourceRange range)
	{
	  FictiveAspComment newComment = new FictiveAspComment();
	  newComment.SetRange(range);
	  return newComment;
	}
	#endregion
	#region InsertInlineExpressionsStatements
	void InsertInlineExpressionsStatements(LanguageElement root)
	{
	  if (root == null || InlineExpressions == null || InlineExpressions.Count == 0)
		return;
	  for (int i = 0; i < InlineExpressions.Count; i++)
	  {
		Expression node = InlineExpressions[i];
		if (node == null)
		  continue;
		Statement parentStatement = Statement.FromExpression(node);
		parentStatement.SetRange(node.Range);
				InsertInlineExpressionStatement(root, parentStatement);
	  }
	}
	#endregion
	bool InsertInlineExpressionStatement(LanguageElement root, LanguageElement node)
	{
	  if (root == null || node == null)
		return false;
	  SourceRange nodeRange = node.Range;
	  if (!root.Range.Contains(nodeRange))
		return false;
	  int count = root.NodeCount;
	  if (count > 0)
	  {
		LanguageElement lastNode = root.Nodes[count - 1] as LanguageElement;
		if (lastNode.Range.EndsBefore(nodeRange))
		{
		  root.AddNode(node);
		  return true;
		}
	  }
	  for (int i = 0; i < count - 1; i++)
	  {
		LanguageElement current = root.Nodes[i] as LanguageElement;
		LanguageElement next = root.Nodes[i + 1] as LanguageElement;
		if (current == null || next == null)
		  continue;
		SourceRange currentRange = current.Range;
		if (currentRange.Contains(nodeRange))
		  return InsertInlineExpressionStatement(current, node);
		SourceRange nextRange = next.Range;
		if (nextRange.Contains(nodeRange))
		  return InsertInlineExpressionStatement(next, node);
		if (currentRange.EndsBefore(nodeRange) && nextRange.StartsAfter(nodeRange))
		{
		  root.InsertNode(i + 1, node);
		  return true;
		}
		if (currentRange.StartsAfter(nodeRange))
		{
		  root.InsertNode(i, node);
		  return true;
		}
		if (i == (count - 2))
		{
		  if (nextRange.EndsBefore(nodeRange))
		  {
			root.AddNode(node);
			return true;
		  }
		}
	  }
	  if (count == 0)
	  {
		root.AddNode(node);
		return true;
	  }
	  if (count == 1)
	  {
		LanguageElement firstNode = root.Nodes[0] as LanguageElement;
		if (firstNode.Range.Contains(nodeRange))
		  return InsertInlineExpressionStatement(firstNode, node);
		if (firstNode.Range.StartsAfter(nodeRange))
		  root.InsertNode(0, node);
		else
		  root.AddNode(node);
		return true;
	  }
	  return false;
	}
	#region GetBlockNodes
	void GetBlockNodes(NodeList blockNodes, LanguageElement rootNode, SourceRange currentBlockRange, out int insertAtIndex)
	{
	  insertAtIndex = -1;
	  for (int i = 0; i < rootNode.NodeCount; i++)
	  {
		LanguageElement currentSubNode = rootNode.Nodes[i] as LanguageElement;
		if (currentBlockRange.Contains(currentSubNode.Range))
		{
		  if (insertAtIndex == -1)
			insertAtIndex = i;
		  blockNodes.Add(currentSubNode);
		}
	  }
	}
	#endregion
	#region SetServerSideScriptType
	private void SetServerSideScriptType(HtmlScriptDefinition script)
	{
	  XmlAttribute languageDef = script.GetAttribute("language", true);
	  if (languageDef == null || languageDef.Value == null)
	  {
		script.Language = DotNetLanguageType.Unknown;
		return;
	  }
	  script.Language = GetDotNetLanguageFromString(languageDef.Value);
	}
	#endregion
	#region SetClientSideScriptType
	private void SetClientSideScriptType(HtmlScriptDefinition script)
	{
	  if (script == null)
		return;
	  script.Language = DotNetLanguageType.ScriptLanguage;
	  if (_DefaultScriptLanguage == ScriptLanguageType.EcmaScript || _DefaultScriptLanguage == ScriptLanguageType.JavaScript)
		script.Language = DotNetLanguageType.JavaScript;
	  XmlAttribute languageAttribute = script.GetAttribute("language", true);
	  XmlAttribute typeAttribute = script.GetAttribute("type", true);
	  if (languageAttribute != null && !String.IsNullOrEmpty(languageAttribute.Value))
		SetClientSideScriptTypeByAttributeValue(languageAttribute.Value, script);
	  else
		if (typeAttribute != null && !String.IsNullOrEmpty(typeAttribute.Value))
		  SetClientSideScriptTypeByAttributeValue(typeAttribute.Value, script);
	  if ((script.Language == DotNetLanguageType.ScriptLanguage || script.Language == DotNetLanguageType.Unknown) && _DefaultScriptLanguage == ScriptLanguageType.Unknown)
		script.Language = DotNetLanguageType.JavaScript;
	}
	#endregion
	#region GetScriptLanguage
	private DotNetLanguageType GetScriptLanguage(ScriptLanguageType defaultScriptLanguage)
	{
	  DotNetLanguageType result = DotNetLanguageType.Unknown;
	  if (_DefaultScriptLanguage == ScriptLanguageType.EcmaScript || _DefaultScriptLanguage == ScriptLanguageType.JavaScript || _DefaultScriptLanguage == ScriptLanguageType.Unknown)
		result = DotNetLanguageType.JavaScript;
	  return result;
	}
	#endregion
	#region GetClientSideParser
	private ParserBase GetClientSideParser(DotNetLanguageType codeLanguage)
	{
	  if (codeLanguage == DotNetLanguageType.JavaScript || codeLanguage == DotNetLanguageType.Unknown || codeLanguage == DotNetLanguageType.ScriptLanguage)
		return CreateJavaScriptParser();
	  else
		return null;
	}
	#endregion
		protected void AddToUsingList(SourceFile fileNode, AspImportDirective directive)
		{
			if (fileNode == null || directive == null)
				return;
			string importedNamespace = directive.Namespace;
			if (String.IsNullOrEmpty(importedNamespace))
				return;
			if (!fileNode.UsingList.Contains(importedNamespace))
				fileNode.UsingList.Add(importedNamespace, importedNamespace);
		}
	#region SetSourceFileProperties
	protected void SetSourceFileProperties(SourceFile fileNode, AspDirective directive, DotNetLanguageType language)
	{
	  if (fileNode == null)
		return;
	  fileNode.AspPageLanguage = language;
	  SetAspPageBaseType(fileNode, directive);
	  SetCodeBehindFileName(fileNode, directive);
	}
	#endregion
	#region SetMasterPageFile
	protected void SetMasterPageFile(SourceFile fileNode, string masterPageFile)
	{
	  if (fileNode == null)
		return;
	  fileNode.SetMasterPageFile(masterPageFile);
	}
	#endregion
	#region SplitAspDirectiveTagStartIfNeeded
	protected void SplitAspDirectiveTagStartIfNeeded()
	{
	  if (!SetTokensCategory)
		return;
	  Token token = SavedTokens[SavedTokens.Count - 1];
	  CategorizedToken newToken = new CategorizedToken(token.EndPosition - 1, token.EndPosition, token.Line, token.EndColumn - 1, token.EndLine, token.EndColumn, Tokens.TAGOPEN, "@", TokenCategory.HtmlTagDelimiter);
	  token.EndPosition = newToken.StartPosition;
	  token.EndColumn = newToken.Column;
	  token.Value = STR_StartTag;
	  SavedTokens.Add(newToken);
	}
	#endregion
	#region SplitInlineExpressionIfNeeded
	protected void SplitInlineExpressionIfNeeded()
	{
	  if (WorkAsXAMLParser || !SetTokensCategory)
		return;
	  string value = tToken.Value;
	  if (string.IsNullOrEmpty(value) || value.Length < 4)
		return;
	  char quote = value[0];
	  if (quote != '\'' && quote != '"')
		return;
	  quote = value[value.Length - 1];
	  if (quote != '\'' && quote != '"')
		return;
	  value = value.Substring(1, value.Length - 2);
	  string trimValue = value.Trim();
	  if (!trimValue.StartsWith(STR_StartTag) || !trimValue.EndsWith(STR_EndTag))
		return;
	  int serverStartTagPos = value.IndexOf(trimValue);
	  CategorizedToken startTagToken = new CategorizedToken(tToken.StartPosition + serverStartTagPos + 1, tToken.StartPosition + serverStartTagPos + 3, tToken.Line, tToken.Column + serverStartTagPos + 1, tToken.EndLine, tToken.Column + serverStartTagPos + 3, Tokens.ASPSCRIPTTAGSTART, STR_StartTag, TokenCategory.HtmlServerSideScript);
	  int valueLength = value.Length;
	  string code = trimValue.Substring(2, trimValue.Length - 4);
	  int codeLenght = code.Length;
	  CategorizedToken codeToken = new CategorizedToken(startTagToken.EndPosition, startTagToken.EndPosition + codeLenght, tToken.Line, startTagToken.EndColumn, tToken.EndLine, startTagToken.EndColumn + codeLenght, Tokens.NAME, code, TokenCategory.Text);
	  CategorizedToken endTagToken = new CategorizedToken(codeToken.EndPosition, codeToken.EndPosition + 2, tToken.Line, codeToken.EndColumn, tToken.EndLine, codeToken.EndColumn + 2, Tokens.ASPSCRIPTTAGCLOSE, STR_EndTag, TokenCategory.HtmlServerSideScript);
	  CategorizedToken endQuotToken = new CategorizedToken(tToken.EndPosition - 1, tToken.EndPosition, tToken.EndLine, tToken.EndColumn - 1, tToken.EndLine, tToken.EndColumn, Tokens.NAME, tToken.Value[tToken.Value.Length - 1].ToString(), TokenCategory.HtmlAttributeValue);
	  tToken.Value = tToken.Value[0].ToString();
	  tToken.EndPosition = tToken.StartPosition + 1;
	  tToken.EndColumn = tToken.Column + 1;
	  SetCategoryIfNeeded(TokenCategory.HtmlAttributeValue);
	  SavedTokens.RemoveAt(SavedTokens.Count - 1);
	  SavedTokens.Add(startTagToken);
	  SavedTokens.Add(codeToken);
	  SavedTokens.Add(endTagToken);
	  SavedTokens.Add(endQuotToken);
	  SavedTokens.Add(la);
	}
	#endregion
	#region ShouldCloseEmptyTag
	protected bool ShouldCloseEmptyTag(string tagName)
	{
	  if (!EmptyElements.Contains(tagName))
		return false;
	  return true;
	}
	#endregion
	#region MoveNodesInsideElement
	protected void MoveNodesInsideElement(LanguageElement newParentElement, LanguageElement activeContext)
	{
	  if (newParentElement == null || activeContext == null)
		return;
	  int parentIndex = activeContext.Nodes.IndexOf(newParentElement);
	  if (parentIndex == -1)
		return;
	  int nodeCount = activeContext.NodeCount;
			int lastNodeToRemove = parentIndex + 1;
	  for (int i = lastNodeToRemove; i < nodeCount; i++)
	  {
				if (lastNodeToRemove >= activeContext.NodeCount)
					break;
		LanguageElement currentNode = activeContext.Nodes[lastNodeToRemove] as LanguageElement;
		activeContext.RemoveNode(currentNode);
		newParentElement.AddNode(currentNode);
	  }
	}
	#endregion
	#region ParseAttributeStyle
	protected void ParseAttributeStyle(HtmlAttribute attribute)
	{
		if (attribute == null || attribute.Name == null || attribute.Value == null || String.Compare(attribute.Name, "style", StringComparison.CurrentCultureIgnoreCase) != 0)
		return;
	  ISourceReader reader = new SourceStringReader(attribute.Value, attribute.ValueRange.Start.Line, attribute.ValueRange.Start.Offset);
	  CssParser parser = new CssParser();
	  if (parser == null)
		return;
	  ArrayList properties = parser.ParseAttributeStyleDefinition(reader);
	  if (properties != null && properties.Count > 0)
	  {
		attribute.StyleProperties = new CssPropertyDeclarationCollection();
		for (int i = 0; i < properties.Count; i++)
		{
		  CssPropertyDeclaration propertyDecl = properties[i] as CssPropertyDeclaration;
		  if (propertyDecl != null)
		  {
			attribute.AddDetailNode(propertyDecl);
			attribute.StyleProperties.Add(propertyDecl);
		  }
		}
	  }
	}
	#endregion
	#region SetupForAspDirectiveParsing
	protected void SetupForAspDirectiveParsing(string aspDirectiveString, SourceRange aspDirectiveRange)
	{
	  ISourceReader reader = new SourceStringReader(aspDirectiveString, aspDirectiveRange.Start.Line, aspDirectiveRange.Start.Offset);
	  scanner = CreateScanner(reader);
	  la = new Token();
	  la.Value = "";
	  Get();
	}
	#endregion
	#region ParseWebHandlerDotNetCode
	protected void ParseWebHandlerDotNetCode(ParserContext parserContext, ISourceReader reader)
	{
	  try
	  {
		ParserBase parser = GetParserByLanguageType(DefaultDotNetLanguage, false);
		if (parser == null)
		  return;
		parser.Parse(parserContext, reader);
	  }
	  catch (Exception ex)
	  {
		Log.SendException(ex);
	  }
	}
	#endregion
		#region FillHtmlElementTypes
		protected void FillHtmlElementTypes()
		{
			HtmlElementTypes.Add("A", HtmlElementType.A);
			HtmlElementTypes.Add("ABBR", HtmlElementType.Abbr);
			HtmlElementTypes.Add("ACRONYM", HtmlElementType.Acronym);
			HtmlElementTypes.Add("ADDRESS", HtmlElementType.Address);
			HtmlElementTypes.Add("APPLET", HtmlElementType.Applet);
			HtmlElementTypes.Add("AREA", HtmlElementType.Area);
			HtmlElementTypes.Add("B", HtmlElementType.B);
			HtmlElementTypes.Add("BASE", HtmlElementType.Base);
			HtmlElementTypes.Add("BASEFONT", HtmlElementType.Basefont);
			HtmlElementTypes.Add("BDO", HtmlElementType.Bdo);
			HtmlElementTypes.Add("BIG", HtmlElementType.Big);
			HtmlElementTypes.Add("BLOCKQUOTE", HtmlElementType.Blockquote);
			HtmlElementTypes.Add("BODY", HtmlElementType.Body);
			HtmlElementTypes.Add("BR", HtmlElementType.Br);
			HtmlElementTypes.Add("BUTTON", HtmlElementType.Button);
			HtmlElementTypes.Add("CAPTION", HtmlElementType.Caption);
			HtmlElementTypes.Add("CENTER", HtmlElementType.Center);
			HtmlElementTypes.Add("CITE", HtmlElementType.Cite);
			HtmlElementTypes.Add("CODE", HtmlElementType.Code);
			HtmlElementTypes.Add("COL", HtmlElementType.Col);
			HtmlElementTypes.Add("COLGROUP", HtmlElementType.Colgroup);
			HtmlElementTypes.Add("DD", HtmlElementType.Dd);
			HtmlElementTypes.Add("DEL", HtmlElementType.Del);
			HtmlElementTypes.Add("DFN", HtmlElementType.Dfn);
			HtmlElementTypes.Add("DIR", HtmlElementType.Dir);
			HtmlElementTypes.Add("DIV", HtmlElementType.Div);
			HtmlElementTypes.Add("DL", HtmlElementType.Dl);
			HtmlElementTypes.Add("DT", HtmlElementType.Dt);
			HtmlElementTypes.Add("EM", HtmlElementType.Em);
			HtmlElementTypes.Add("FIELDSET", HtmlElementType.Fieldset);
			HtmlElementTypes.Add("FONT", HtmlElementType.Font);
			HtmlElementTypes.Add("FORM", HtmlElementType.Form);
			HtmlElementTypes.Add("FRAME", HtmlElementType.Frame);
			HtmlElementTypes.Add("FRAMESET", HtmlElementType.Frameset);
			HtmlElementTypes.Add("H1", HtmlElementType.H1);
			HtmlElementTypes.Add("H2", HtmlElementType.H2);
			HtmlElementTypes.Add("H3", HtmlElementType.H3);
			HtmlElementTypes.Add("H4", HtmlElementType.H4);
			HtmlElementTypes.Add("H5", HtmlElementType.H5);
			HtmlElementTypes.Add("H6", HtmlElementType.H6);
			HtmlElementTypes.Add("HEAD", HtmlElementType.Head);
			HtmlElementTypes.Add("HR", HtmlElementType.Hr);
			HtmlElementTypes.Add("HTML", HtmlElementType.Html);
			HtmlElementTypes.Add("I", HtmlElementType.I);
			HtmlElementTypes.Add("IFRAME", HtmlElementType.Iframe);
			HtmlElementTypes.Add("IMG", HtmlElementType.Img);
			HtmlElementTypes.Add("INPUT", HtmlElementType.Input);
			HtmlElementTypes.Add("INS", HtmlElementType.Ins);
			HtmlElementTypes.Add("ISINDEX", HtmlElementType.Isindex);
			HtmlElementTypes.Add("KBD", HtmlElementType.Kbd);
			HtmlElementTypes.Add("LABEL", HtmlElementType.Label);
			HtmlElementTypes.Add("LEGEND", HtmlElementType.Legend);
			HtmlElementTypes.Add("LI", HtmlElementType.Li);
			HtmlElementTypes.Add("LINK", HtmlElementType.Link);
			HtmlElementTypes.Add("MAP", HtmlElementType.Map);
			HtmlElementTypes.Add("MENU", HtmlElementType.Menu);
			HtmlElementTypes.Add("META", HtmlElementType.Meta);
			HtmlElementTypes.Add("NOFRAMES", HtmlElementType.Noframes);
			HtmlElementTypes.Add("NOSCRIPT", HtmlElementType.Noscript);
			HtmlElementTypes.Add("OBJECT", HtmlElementType.Object);
			HtmlElementTypes.Add("OL", HtmlElementType.Ol);
			HtmlElementTypes.Add("OPTGROUP", HtmlElementType.Optgroup);
			HtmlElementTypes.Add("OPTION", HtmlElementType.Option);
			HtmlElementTypes.Add("P", HtmlElementType.P);
			HtmlElementTypes.Add("PARAM", HtmlElementType.Param);
			HtmlElementTypes.Add("PRE", HtmlElementType.Pre);
			HtmlElementTypes.Add("Q", HtmlElementType.Q);
			HtmlElementTypes.Add("S", HtmlElementType.S);
			HtmlElementTypes.Add("SAMP", HtmlElementType.Samp);
			HtmlElementTypes.Add("SCRIPT", HtmlElementType.Script);
			HtmlElementTypes.Add("SELECT", HtmlElementType.Select);
			HtmlElementTypes.Add("SMALL", HtmlElementType.Small);
			HtmlElementTypes.Add("SPAN", HtmlElementType.Span);
			HtmlElementTypes.Add("STRIKE", HtmlElementType.Strike);
			HtmlElementTypes.Add("STRONG", HtmlElementType.Strong);
			HtmlElementTypes.Add("STYLE", HtmlElementType.Style);
			HtmlElementTypes.Add("SUB", HtmlElementType.Sub);
			HtmlElementTypes.Add("SUP", HtmlElementType.Sup);
			HtmlElementTypes.Add("TABLE", HtmlElementType.Table);
			HtmlElementTypes.Add("TBODY", HtmlElementType.Tbody);
			HtmlElementTypes.Add("TD", HtmlElementType.Td);
			HtmlElementTypes.Add("TEXTAREA", HtmlElementType.Textarea);
			HtmlElementTypes.Add("TFOOT", HtmlElementType.Tfoot);
			HtmlElementTypes.Add("TH", HtmlElementType.Th);
			HtmlElementTypes.Add("THEAD", HtmlElementType.Thead);
			HtmlElementTypes.Add("TITLE", HtmlElementType.Title);
			HtmlElementTypes.Add("TR", HtmlElementType.Tr);
			HtmlElementTypes.Add("TT", HtmlElementType.Tt);
			HtmlElementTypes.Add("U", HtmlElementType.U);
			HtmlElementTypes.Add("UL", HtmlElementType.Ul);
			HtmlElementTypes.Add("VAR", HtmlElementType.Var);
		}
		#endregion
		#region FillEmptyElementsTable
		protected void FillEmptyElementsTable()
		{
			EmptyElements.Add("AREA","AREA");	
			EmptyElements.Add("BASE","BASE");	
			EmptyElements.Add("BASEFONT","BASEFONT");
			EmptyElements.Add("BR","BR");	
			EmptyElements.Add("COL","COL");
			EmptyElements.Add("FRAME","FRAME");
			EmptyElements.Add("HR","HR");
			EmptyElements.Add("IMG","IMG");
			EmptyElements.Add("INPUT","INPUT");
			EmptyElements.Add("ISINDEX","ISINDEX");
			EmptyElements.Add("LINK","LINK");
			EmptyElements.Add("META","META");
			EmptyElements.Add("PARAM","PARAM");
		}
		#endregion
	#region ShouldPreventCycling
	protected bool ShouldPreventCycling()
	{
	  return ElementNestingLevel >= MaxElementNestingLevel;
	}
	#endregion
	#region IsBadCloseTag
	protected bool IsBadCloseTag(string closeTagElementName)
		{
			if (closeTagElementName == null || Context == null)
				return false;
			if (EmptyElements != null && EmptyElements.Contains(closeTagElementName))
				return false;
			LanguageElement currentContext = Context;
			while (currentContext != null && !(currentContext is SourceFile))
			{
				if (currentContext.Name != null && String.Compare(closeTagElementName, currentContext.Name, StringComparison.CurrentCultureIgnoreCase) == 0) 
					return false;
				currentContext = currentContext.Parent;
			}
			return true;
		}
		#endregion
		#region SkipCloseTag
		void SkipCloseTag()
		{
	  if (Context != null)
		Context.HasErrors = true;
			Get();
			if (la.Type != Tokens.TAGCLOSE)
			{
				Get();
		SetCategoryIfNeeded(TokenCategory.HtmlElementName);
			}
			HtmlScanner.ShouldCheckForXmlText = true;
			Get();
		}
		#endregion
	#region SetCategoryIfNeeded
	protected void SetCategoryIfNeeded(TokenCategory category)
	{
	  if (SetTokensCategory)
	  {
		CategorizedToken token = tToken as CategorizedToken;
		if (token != null)
		  token.Category = category;
	  }
	}
	#endregion
	#region GetReaderForWebHandler
	internal ISourceReader GetReaderForWebHandler(HtmlScannerBase scanner, ISourceReader baseReader)
	{
	  if (baseReader == null || scanner == null)
		return baseReader;
	  TextReader textReader = baseReader.GetStream();
	  string text = textReader.ReadToEnd();
	  return baseReader.GetSubStream(scanner.Position, text.Length - scanner.Position, scanner.Line, scanner.Column);
	}
	#endregion
		#region ClearDefaultLanguages
		protected void ClearDefaultLanguages()
		{
			_DefaultDotNetLanguage = InitialDefaultDotNetLanguage;
			_DefaultScriptLanguage = ScriptLanguageType.Unknown;
		}
		#endregion
		#region IsWebHandlerOrWebService
		protected bool IsWebHandlerOrWebService(HtmlElement element)
		{
			if (!(element is AspDirective))
				return false;
			if (String.Compare("WebService", element.Name) == 0)
				return true;
			if (String.Compare("WebHandler", element.Name) == 0)
				return true;
	  if (String.Compare("Application", element.Name) == 0)
		return true;
			return false;
		}
		#endregion
		#region GetDefaultLanguage
		protected DotNetLanguageType GetDefaultLanguage(HtmlElement element)
		{
			DotNetLanguageType result = DotNetLanguageType.Unknown;
			if (!(element is PageDirective || element is ControlDirective || IsWebHandlerOrWebService(element)))
				return result;
			if (element == null || element.Name == null || element.HasAttributes == false)
				return result;
			XmlAttribute language = element.GetAttribute("language", true);
			if (language != null)
			{
				result = GetDotNetLanguageFromString(language.Value);
				SetDefaultLanguage(language.Value);
			}
			return result;
		}
		#endregion
	#region GetMasterPageFile
	protected string GetMasterPageFile(HtmlElement element)
	{
	  PageDirective pageDirective = element as PageDirective;
	  if (pageDirective == null || !pageDirective.HasAttributes)
		return string.Empty;
	XmlAttribute masterPageAttribute = element.GetAttribute("MasterPageFile", true);
	  if (masterPageAttribute == null)
		return null;
	  SourceFile fileNode = element.FileNode;
	  if (fileNode == null)
		return null;
	  return GetIncludedFilePath(fileNode, masterPageAttribute.Value);
	}
	#endregion
	#region GetDefaultScript
	protected void GetDefaultScript(HtmlElement element)
		{
			if (element == null || element.Name == null || String.Compare(element.Name, "meta", StringComparison.CurrentCultureIgnoreCase) != 0 || element.HasAttributes == false) 
				return;
			GetDefaultScriptFromHttpEq(element);
			GetDefaultScriptFromVs(element);
		}
		#endregion 
		#region SetScriptLanguageType
		protected void SetScriptLanguageType(HtmlScriptDefinition script)
		{
			if (script == null)
				return;
	  if (script.Attributes["runat"] == null)
		SetClientSideScriptType(script);
	  else
		SetServerSideScriptType(script);
		}
		#endregion
		private void CheckForRazorExpression(HtmlAttribute attribute)
	{
	  if (_StringEmbedding == null || attribute == null)
		return;
	  _StringEmbedding.SetRange(attribute.Range);
	  _StringEmbedding.IsRazorEmbedding = true;
	  attribute.AddDetailNode(_StringEmbedding);
	  if (_StringEmbedding.DetailNodeCount > 0)
	  {
		Expression embeddedExpression = _StringEmbedding.DetailNodes[0] as Expression;
		if (embeddedExpression != null)
		{
		  attribute.InlineExpression = embeddedExpression;
		  SourceRange range = embeddedExpression.Range;
		  _StringEmbedding.SetRange(new SourceRange(range.Start.Line, range.Start.Offset - 1, range.End.Line, range.End.Offset));
		}
	  }
		_StringEmbedding = null;
	}
	#region CheckForInlineExpression
		protected void CheckForInlineExpression(HtmlAttribute attribute)
		{
	  if (IsRazor)
		CheckForRazorExpression(attribute);
			if (attribute == null || attribute.Value == null || attribute.Value.Length == 0)
		return;
	  if (!WorkAsXAMLParser && DefaultDotNetLanguage == DotNetLanguageType.Unknown)
				return;
			string attrVal = attribute.Value;
			int inlineCodeStart = GetInlineCodeStart(attrVal);
	  if (inlineCodeStart == -1)
		return;
			int inlineCodeEnd = -1;
	  if (WorkAsXAMLParser)
		inlineCodeEnd = attrVal.LastIndexOf('}') + 1;
	  else
		inlineCodeEnd = attrVal.IndexOf(STR_EndTag);
			if (inlineCodeEnd == -1)
				return;
	  if (inlineCodeStart > inlineCodeEnd)
		return;
			string inlineCode = attrVal.Substring(inlineCodeStart, inlineCodeEnd - inlineCodeStart);
			int startLine = attribute.ValueRange.Start.Line;
			int startOffset = attribute.ValueRange.Start.Offset + inlineCodeStart;
			Expression expression = ParseInlineExpression(inlineCode, startLine, startOffset, null);
			if (expression == null)
		return;
	  attribute.InlineExpression = expression;
	  if (WorkAsXAMLParser)
	  {
		attribute.AddDetailNode(expression);
		return;
	  }
	  Expression clone = expression.Clone() as Expression;
	  if (clone != null)
		InlineExpressions.Add(clone);
	  AspCodeEmbeddingClass aspCodeEmbedding = new AspCodeEmbeddingClass();
	  aspCodeEmbedding.SetRange(expression.Range);
	  aspCodeEmbedding.AddDetailNode(expression);
	  attribute.AddDetailNode(aspCodeEmbedding);
	}
		#endregion
		#region PerformPostProcessing
		protected void PerformPostProcessing()
		{
			if (RootNode == null || RootNode.NodeCount == 0)
				return;
	  ElementNestingLevel = 0;
			try
			{
				BlockRanges = new List<SourceRange>();
		FictiveCommentRanges = new List<SourceRange>();
				ArrayList runAtServer = new ArrayList();
				ArrayList nodes = new ArrayList();
				ArrayList aspCodeEmbeddings = GetAspCodeEmbeddingsFromNode(runAtServer, nodes, RootNode);
				if (aspCodeEmbeddings == null)
					return;
				CheckForInlineExpression(aspCodeEmbeddings);
		List<AspCodeEmbedding> razorEmbeddings = GetRazorEmbeddings(aspCodeEmbeddings);
				TokenCategorizedParserBase parser = GetParserByLanguageType(_DefaultDotNetLanguage, false);
				if (parser == null)
					return;
				Method aspFictiveMethod = new Method("void", "FictiveMethod");
				aspFictiveMethod.SetRange(GetRange(SourceFileStartRange, SourceFileEndRange));
				aspFictiveMethod.MethodType = MethodTypeEnum.Function;
				parser.SetTokensCategory = SetTokensCategory;
				ParserContext context = GetAspEmbeddingsParserContext(aspFictiveMethod, aspCodeEmbeddings, runAtServer, nodes);
				LanguageElement root =  parser.Parse(context, context.SourceReader);
				if (root != null)
				{
					SourceFile fileNode = RootNode as SourceFile;
					if (fileNode != null)
						fileNode.Code = aspFictiveMethod;
					RootNode.AddNode(aspFictiveMethod);
					InsertInlineExpressionsStatements(aspFictiveMethod);
		  InsertRazorEmbeddings(aspFictiveMethod, razorEmbeddings);
					if (SetTokensCategory)
						ReplaceAllEmbeddingTokens(aspCodeEmbeddings, parser.SavedTokens);
				}
				BlockRanges = null;
			}
			catch(Exception ex)
			{
				Log.SendException(ex);
			}
		}
	private LanguageElementCollection NodeListToLanguageElementCollection(NodeList nodes)
	{
	  LanguageElementCollection collection = new LanguageElementCollection();
	  for (int i = 0; i < nodes.Count; i++)
	  {
		LanguageElement element = nodes[i] as LanguageElement;
		if (element != null)
		  collection.Add(element);
	  }
	  return collection;
	}
	LanguageElementCollection FilterRazorEmbeddingNodes(AspCodeEmbeddingClass codeEmbedding, LanguageElementCollection nodes)
	{
	  if (codeEmbedding == null || !codeEmbedding.IsRazorEmbedding)
		return nodes;
	  if (codeEmbedding is RazorSection || codeEmbedding is RazorFunctions)
		return nodes;
	  if (nodes.Count != 1)
		return nodes;
	  Block block = nodes[0] as Block;
	  if (block == null)
		return nodes;
	  LanguageElementCollection result = new LanguageElementCollection();
	  int count = block.NodeCount;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement node = block.Nodes[i] as LanguageElement;
		if (node != null)
		  result.Add(node);
	  }
	  return result;
	}
	private void InsertRazorEmbeddings(Method aspFictiveMethod, List<AspCodeEmbeddingClass> razorEmbeddings)
	{
	  LanguageElementCollection nodes = null;
	  LanguageElementCollection detailNodes = null;
	  for (int i = 0; i < razorEmbeddings.Count; i++)
	  {
		AspCodeEmbeddingClass embedding = razorEmbeddings[i];
		if (embedding.DetailNodeCount > 0)
		{
		  detailNodes = NodeListToLanguageElementCollection(embedding.DetailNodes);
		  detailNodes = FilterRazorEmbeddingNodes(embedding, detailNodes);
		  AddEmbeddingNodes(aspFictiveMethod, detailNodes);
		}
		if (embedding.NodeCount > 0)
		{
		  nodes = NodeListToLanguageElementCollection(embedding.Nodes);
		  nodes = FilterRazorEmbeddingNodes(embedding, nodes);
		  AddEmbeddingNodes(aspFictiveMethod, nodes);
		}
	  }
	}
	private void AddEmbeddingNodes(Method aspFictiveMethod, NodeList nodeList)
	{
	  for (int i = 0; i < nodeList.Count; i++)
	  {
		LanguageElement element = nodeList[i] as LanguageElement;
		Expression expression = element as Expression;
		if (expression != null)
		{
		  Statement parentStatement = Statement.FromExpression(expression.Clone() as Expression);
		  parentStatement.SetRange(expression.Range);
		  InsertInlineExpressionStatement(aspFictiveMethod, parentStatement);
		}
		else
		  InsertInlineExpressionStatement(aspFictiveMethod, element.Clone() as LanguageElement);
	  }
	}
	private List<AspCodeEmbeddingClass> GetRazorEmbeddings(ArrayList aspCodeEmbeddings)
	{
	  List<AspCodeEmbeddingClass> result = new List<AspCodeEmbeddingClass>();
	  for (int i = 0; i < aspCodeEmbeddings.Count; i++)
	  {
		AspCodeEmbeddingClass codeEmbedding = aspCodeEmbeddings[i] as AspCodeEmbeddingClass;
		if (codeEmbedding != null && codeEmbedding.IsRazorEmbedding)
		  result.Add(codeEmbedding);
	  }
	  return result;
	}
		private void ReplaceAllEmbeddingTokens(ArrayList aspCodeEmbeddings, TokenCollection tokenCollection)
		{
			if (aspCodeEmbeddings == null || aspCodeEmbeddings.Count == 0)
				return;
			if (tokenCollection == null || tokenCollection.Count == 0)
				return;
			foreach (AspCodeEmbedding codeEmbedding in aspCodeEmbeddings)
			{
				if (codeEmbedding == null)
					continue;
				Token codeEmbeddingToken = codeEmbedding.CodeEmbeddingToken;
				if (codeEmbeddingToken == null)
					continue;
				TokenCollection savedTokensForEmbedding = GetCodeEmbeddingsTokens(codeEmbeddingToken, tokenCollection);
				if (savedTokensForEmbedding != null && savedTokensForEmbedding.Count > 0)
					ReplaceEmbeddingToken(codeEmbeddingToken, savedTokensForEmbedding);
			}
		}
		private TokenCollection GetCodeEmbeddingsTokens(Token codeEmbeddingToken, TokenCollection savedTokens)
		{
			TokenCollection result = new TokenCollection();
			SourceRange embeddingTokenRange = codeEmbeddingToken.Range;
			for (int i = 0; i < savedTokens.Count; i++)
			{
				Token currentToken = savedTokens[i];
				if (currentToken == null)
					continue;
				SourceRange currentTokenRange = currentToken.Range;
				if (embeddingTokenRange.Contains(currentTokenRange) || 
					(currentTokenRange.Contains(embeddingTokenRange.End) && !currentTokenRange.Contains(embeddingTokenRange.Start)))
					result.Add(currentToken);
			}
			for (int i = 0; i < result.Count; i++)
				savedTokens.Remove(result[i]);
			return result;
		}
		ParserContext GetAspEmbeddingsParserContext(Method aspFictiveMethod, ArrayList aspCodeEmbeddings, ArrayList runAtServerNodes, ArrayList nodes)
		{
			int startLine = 1;
			int startOffset = 1;
			ArrayList aspTokenBlocks = new ArrayList();
			string text = GetCodeEmbeddingsText(aspTokenBlocks, aspCodeEmbeddings, runAtServerNodes, nodes, ref startLine, ref startOffset);
			ScannerExtension extension = new ScannerExtension();
			foreach (TokenBlock block in aspTokenBlocks)
				extension.EnqueueBlock(block);
			ISourceReader reader = new SourceStringReader(text, startLine, startOffset);
			ParserContext context = new ParserContext();
			if (RootNode != null)
				context.Document = RootNode.Document;
			context.Context = aspFictiveMethod;
			context.SourceReader = reader;
			context.ScannerExtension = extension;
			return context;
		}
		#endregion
		#region InsertFictiveComments
		void InsertFictiveComments(LanguageElement root)
		{
			if (root == null || FictiveCommentRanges == null || FictiveCommentRanges.Count <= 1)
				return;
			for (int i = 0; i < FictiveCommentRanges.Count; i++)
				InsertFictiveCommentInProperPlace(root, FictiveCommentRanges[i]);
		}
		#endregion
		#region ShouldCloseTopLevelTag
		protected bool ShouldCloseTopLevelTag(string currentTagName, string topLevelTagName)
		{
			if (currentTagName == null || topLevelTagName == null)
				return false;
			string lowerCaseTagName = currentTagName.ToLower();
			if (lowerCaseTagName.IndexOf("asp:") != -1 || lowerCaseTagName.IndexOf("atlas:") != -1)
				return false;
			if (!_OptionalEndTagElements.Contains(topLevelTagName))
				return false;
			if (!HtmlElementTypes.Contains(currentTagName))
				return false;
			Hashtable validNestedElements = _OptionalEndTagElements[topLevelTagName] as Hashtable;
			if (validNestedElements == null)
				return false;
			return validNestedElements.Contains(currentTagName) == false;
		}
		#endregion
		protected void HandleCodeEmbedding(HtmlElement htmlElement, AspCodeEmbedding codeEmbedding)
		{
			if (codeEmbedding == null || htmlElement == null)
				return;
			htmlElement.NameRange = codeEmbedding.Range;
			htmlElement.AddDetailNode(codeEmbedding);
			tToken.Value = String.Format("<%{0}%>", codeEmbedding.Code);
			tToken.Range = codeEmbedding.Range;
		}
		protected void SetContextForHtmlElement(HtmlElement htmlElement, SourceRange lastTagRange, SourceRange tagOpenRange)
		{
			if (htmlElement == null)
				return;
			SetCategoryIfNeeded(TokenCategory.HtmlElementName);
			TopLevelReturnCount = 0;
			while (ShouldCloseTopLevelTag(tToken.Value, Context.Name))
			{
				if (lastTagRange != SourceRange.Empty)
					Context.SetRange(GetRange(Context, lastTagRange));
				if (Context is HtmlElement)
				{
					(Context as HtmlElement).InnerRange = GetRange((Context as HtmlElement).InnerRange, new SourceRange(tagOpenRange.Start.Line, tagOpenRange.Start.Offset));
				}
				CloseContext();
				TopLevelReturnCount++;
			}
			if (TopLevelReturnCount > ElementNestingLevel)
				TopLevelReturnCount = ElementNestingLevel;
			OpenContext(htmlElement);
			SetHtmlElementType(htmlElement, tToken.Value);
		}
		protected void HandleTagClose(HtmlElement htmlElement, out bool shouldReturn)
		{
			shouldReturn = true;
			if (htmlElement == null)
				return;
			SourceRange range = GetRange(htmlElement, tToken);
			htmlElement.SetRange(range);
			htmlElement.SetBlockStart(range);
			htmlElement.InnerRange = new SourceRange(tToken.Range.End.Line, tToken.Range.End.Offset);
			if (ShouldCloseEmptyTag(Context.Name))
			{
				CloseContext();
				if (ShouldPreventCycling())
				{
					htmlElement.HasCloseTag = false;
					ElementNestingLevel--;
					return;
				}
			}
			ElementNestingLevel++;
			shouldReturn = false;
		}
	protected void HandleEndingTagClose(HtmlElement htmlElement, SourceRange tagCloseRange, out bool shouldReturn)
	{
	  shouldReturn = true;
	  htmlElement.SetBlockEnd(GetRange(tagCloseRange, tToken));
	  ElementNestingLevel--;
	  Token nextToken;
	  while (la.Type == Tokens.TAGCLOSESTART)
	  {
		ResetPeek();
		HtmlScanner.ShouldCheckForXmlText = false;
		nextToken = Peek();
		if (nextToken.Type != Tokens.NAME)
		  return;
		string closeTagElementName = nextToken.Value;
		if (String.Compare(closeTagElementName, Context.Name, StringComparison.CurrentCultureIgnoreCase) != 0)
		{
		  if (IsBadCloseTag(closeTagElementName))
		  {
			SkipCloseTag();
			if (la.Type != Tokens.TAGCLOSESTART)
			  return;
		  }
		  else
			break;
		}
		else
		  break;
	  }
	  shouldReturn = false;
	}
		protected void HanldeEmptyTagClose(HtmlElement htmlElement)
		{
			htmlElement.IsEmptyTag = true;
			htmlElement.HasCloseTag = false;
			htmlElement.InnerRange = new SourceRange(tToken.Range.End.Line, tToken.Range.End.Offset);
			CloseContext();
			htmlElement.SetRange(GetRange(htmlElement, tToken)); 
		}
		protected void HandleTagCloseStart(HtmlElement htmlElement, out SourceRange tagCloseRange, out bool shouldReturn)
		{
			shouldReturn = true;
			tagCloseRange = SourceRange.Empty;
			if (htmlElement == null)
				return;
			ResetPeek();
	  Token nextToken = Peek();
	  if (nextToken.Type == Tokens.NAME)
	  {
		string closeTagElementName = nextToken.Value;
		if (String.Compare(closeTagElementName, htmlElement.Name, StringComparison.CurrentCultureIgnoreCase) != 0)
			  {
				  if (htmlElement != null)
					  htmlElement.HasCloseTag = false;
				  if (IsBadCloseTag(closeTagElementName))
				  {
					  SkipCloseTag();
					  if (la.Type != Tokens.TAGCLOSESTART)
					  {
						  if (Context != null)
						  {
							  Context.SetRange(GetRange(Context, tToken));
							  CloseContext();
						  }
						  ElementNestingLevel--;
						  return;
					  }
				  }
				  else
				  {
					  if (Context != null)
						  Context.SetRange(GetRange(Context, tToken));
					  if (String.Compare(closeTagElementName, Context.Name, StringComparison.CurrentCultureIgnoreCase) != 0)
						  CloseContext();
					  ElementNestingLevel--;
					  return;
				  }
			  }
	  }
			tagCloseRange = la.Range;
			HtmlScanner.ShouldCheckForXmlText = false;
			htmlElement.SetBlockEnd(la.Range);
			shouldReturn = false;
		}
		protected void HandleClosingNameToken(HtmlElement htmlElement, SourceRange tagCloseRange, out string closeTagElementName)
		{
	  closeTagElementName = string.Empty;
	  if (tToken.Type == Tokens.NAME)
	  {
		SetCategoryIfNeeded(TokenCategory.HtmlElementName);
		closeTagElementName = tToken.Value;
	  }
	  bool isEmptyName = string.IsNullOrEmpty(closeTagElementName);
	  if (String.Compare(closeTagElementName, htmlElement.Name, StringComparison.CurrentCultureIgnoreCase) == 0 || isEmptyName)
	  {
		htmlElement.SetRange(GetRange(htmlElement, la));
		if (!isEmptyName)
		  htmlElement.CloseTagNameRange = tToken.Range;
		htmlElement.InnerRange = GetRange(htmlElement.InnerRange, new SourceRange(tagCloseRange.Start.Line, tagCloseRange.Start.Offset));
		if (EmptyElements.Contains(closeTagElementName))
		  MoveNodesInsideElement(htmlElement, Context);
	  }
	  if (String.Compare(closeTagElementName, Context.Name, StringComparison.CurrentCultureIgnoreCase) == 0 || isEmptyName)
		CloseContext();
	  HtmlScanner.ShouldCheckForXmlText = true;
		}
		protected void TryToHandleScriptOrStyleSheets(HtmlElement htmlElement)
		{
	  const string styleName = "style";
	  const string scriptName = "script";
			if (htmlElement == null || String.IsNullOrEmpty(htmlElement.Name))
				return;
	  string htmlElementName = htmlElement.Name.ToLower();
			GetDefaultScript(htmlElement);
			if (htmlElement is HtmlScriptDefinition)
			{
				HtmlScriptDefinition htmlScript = htmlElement as HtmlScriptDefinition;
				SetScriptLanguageType(htmlScript);
				AddIncludedScriptFilesItem(htmlScript);
			}
			if (htmlElement != null && string.Compare(htmlElement.Name, "link", StringComparison.CurrentCultureIgnoreCase) == 0)
				AddIncludedStyleSheetFilesItem(htmlElement);
			string elementContent = String.Empty;
			SourceRange contentRange = SourceRange.Empty;
			if ((htmlElementName == styleName && !WorkAsXAMLParser)|| htmlElementName == scriptName)
			{
		if (la.Type != Tokens.EMPTYTAGCLOSE)
		  HtmlScanner.GetElementContent(htmlElement.Name, out elementContent, out contentRange);
				if (htmlElement is HtmlScriptDefinition)
				{
					HtmlScriptDefinition htmlScriptDefinition = (HtmlScriptDefinition)htmlElement;
					htmlScriptDefinition.ScriptText = elementContent;
					htmlScriptDefinition.CodeRange = contentRange;
					int oldNodeCount = htmlScriptDefinition.NodeCount;
					ParseScript(htmlScriptDefinition);
					if (oldNodeCount == htmlScriptDefinition.NodeCount && elementContent.Length > 0 && SetTokensCategory)
					{
						CategorizedToken token = new CategorizedToken();
						token.Value = elementContent;
						token.Range = contentRange;
						token.Category = TokenCategory.Text;
						token.StartPosition = la.EndPosition;
						token.EndPosition = elementContent.Length + token.StartPosition;
						SavedTokens.Add(token);
					}
				}
				else
					if (htmlElement is HtmlStyleDefinition)
					{
						(htmlElement as HtmlStyleDefinition).StyleText = elementContent;
						(htmlElement as HtmlStyleDefinition).TextRange = contentRange;
						ParseStyleDefinition(htmlElement as HtmlStyleDefinition);
					}
			}
			else
				HtmlScanner.ShouldCheckForXmlText = true;
		}
		protected HtmlElement CreateElement(string elementName, SourceRange elementNameRange, SourceRange openTagRange)
		{
			HtmlElement htmlElement;
			if (String.Compare(elementName, "SCRIPT", StringComparison.CurrentCultureIgnoreCase) == 0)
				htmlElement = new HtmlScriptDefinition();
			else
				if (String.Compare(elementName, "STYLE", StringComparison.CurrentCultureIgnoreCase) == 0)
					htmlElement = new HtmlStyleDefinition();
				else
					if (elementName.IndexOf(":") != -1)
					{
						string laUpper = elementName.ToUpper();
						if (laUpper.IndexOf("CONTENTPLACEHOLDER") != -1)
							htmlElement = new ContentPlaceHolder();
						else
							htmlElement = new ServerControlElement();
					}
					else
						htmlElement = new HtmlElement();
			htmlElement.SetRange(openTagRange);
			htmlElement.NameRange = elementNameRange;
			return htmlElement;
		}
		#region SetHtmlElementType
		protected void SetHtmlElementType(HtmlElement htmlElement, string name)
		{
			if (htmlElement == null || name == null)
				return;
			htmlElement.Name = name;
			HtmlElementType htmlType = HtmlElementType.Unknown;
			if (HtmlElementTypes.Contains(name))
				htmlType = (HtmlElementType)HtmlElementTypes[name];
			htmlElement.HtmlElementType = htmlType;
		}
		#endregion
	protected override TokenCategory GetTokenCategory(CategorizedToken token)
	{
	  switch (token.Type)
	  {
		case Tokens.EQUALSTOKEN:
		  return TokenCategory.HtmlOperator;
		case Tokens.COMMENT:
		case Tokens.SERVERCOMMENT:
		  return TokenCategory.HtmlComment;
		case Tokens.ASPSCRIPTTAGSTART:
		case Tokens.ASPSCRIPTTAGCLOSE:
		case Tokens.ASPDIRECTIVETAGSTART:
		case Tokens.RAZORSTARTCHAR:
		  return TokenCategory.HtmlServerSideScript;
		case Tokens.QUESTTAGOPEN:
		case Tokens.QUESTTAGCLOSE:
		case Tokens.TAGCLOSE:
		case Tokens.TAGOPEN:
		case Tokens.EMPTYTAGCLOSE:
		case Tokens.TAGCLOSESTART:
		case Tokens.AMPERSAND:
		case Tokens.DECL:
		  return TokenCategory.HtmlTagDelimiter;
		case Tokens.QUOTEDLITERAL:
		  return TokenCategory.HtmlString;
		case Tokens.EOF:
		  return TokenCategory.Unknown;
		default:
		  return TokenCategory.Text;
	  }
	}
	#region CheckForEventHandlerScriptCode
	protected void CheckForEventHandlerScriptCode(HtmlAttribute attribute)
	{
	  if (attribute == null)
		return;
	  string attributeValue = attribute.Value;
	  if (String.IsNullOrEmpty(attributeValue) || !EventAttributeNames.Contains(attribute.Name))
		return;
	  DotNetLanguageType codeLanguage = GetScriptLanguage(_DefaultScriptLanguage);
	  ParserBase parser = GetClientSideParser(codeLanguage);
	  if (parser == null)
		return;
	  PosOffset = tToken.StartPosition - tToken.Column;
	  int count = SavedTokens.Count;
	  LanguageElement root = GetRootNodeForScript(parser, attributeValue, attribute.ValueRange.Start);
	  while (SavedTokens.Count > count)
		SavedTokens.RemoveAt(SavedTokens.Count - 1);
	  if (root == null)
		return;
	  attribute.ScriptLanguage = codeLanguage;
	  attribute.ScriptCode = new LanguageElementCollection();
	  LanguageElementCollection attributeScriptCode = attribute.ScriptCode;
	  if (root is SourceFile)
		for (int i = 0; i < root.NodeCount; i++)
		{
		  attributeScriptCode.Add(root.Nodes[i]);
		  attribute.AddDetailNode(root.Nodes[i] as LanguageElement);
		}
	  else
		attributeScriptCode.Add(root);
	}
	#endregion
	private string GetIncludedFilePath(SourceFile fileNode, string includedFileName)
	{
	  if (fileNode == null || string.IsNullOrEmpty(includedFileName))
		return string.Empty;
	  if (IsHttpPath(includedFileName))
		return string.Empty;
	  includedFileName = includedFileName.Replace('/', '\\');
	  string result = string.Empty;
	  try
	  {
		string currentFolder = Path.GetDirectoryName(fileNode.Name);
		if (includedFileName.StartsWith("~"))
		{
		  includedFileName = includedFileName.Substring(1);
		  IProjectElement project = fileNode.Project;
		  if (project == null)
			return string.Empty;
		  string projectPath = project.FilePath;
		  if (String.IsNullOrEmpty(projectPath))
			return String.Empty;
		  currentFolder = Path.GetDirectoryName(projectPath);
		}
		if (includedFileName.StartsWith("\\"))
		  includedFileName = includedFileName.Substring(1);
		if (includedFileName.StartsWith("<%") || includedFileName.StartsWith("@"))
		  return string.Empty;
		result = Path.Combine(currentFolder, includedFileName);
		return Path.GetFullPath(result);
	  }
	  catch (Exception e)
	  {
		Log.SendException("Exception while trying to get full included file path", e);
		Log.Send(includedFileName);
		return string.Empty;
	  }
	}
	#region AddIncludedStyleSheetFilesItem
	protected void AddIncludedStyleSheetFilesItem(HtmlElement htmlLink)
	{
	  if (htmlLink == null)
		return;
	  HtmlAttributeCollection attributes = htmlLink.Attributes;
	  if (attributes == null || attributes.Count == 0)
		return;
	  HtmlAttribute typeAttribute = htmlLink.Attributes["type"];
	  HtmlAttribute relAttribute = htmlLink.Attributes["rel"];
	  if (typeAttribute == null || relAttribute == null)
		return;
	  if (string.Compare("text/css", typeAttribute.Value, StringComparison.CurrentCultureIgnoreCase) != 0 || string.Compare("stylesheet", relAttribute.Value, StringComparison.CurrentCultureIgnoreCase) != 0)
		return;
	  HtmlAttribute hrefAttribute = htmlLink.Attributes["href"];
	  if (hrefAttribute == null)
		return;
	  string pathPart = hrefAttribute.Value;
	  if (String.IsNullOrEmpty(pathPart))
		return;
	  SourceFile fileNode = htmlLink.FileNode;
	  if (fileNode == null)
		return;
	  pathPart = GetIncludedFilePath(fileNode, pathPart);
	  StringCollection includedStyleSheetFiles = fileNode.IncludedStyleSheetFiles;
	  if (includedStyleSheetFiles != null && !includedStyleSheetFiles.Contains(pathPart))
		includedStyleSheetFiles.Add(pathPart);
	}
	#endregion
	#region AddIncludedScriptFilesItem
	protected void AddIncludedScriptFilesItem(HtmlScriptDefinition htmlScript)
	{
	  if (htmlScript == null || htmlScript.FileNode == null)
		return;
	  HtmlAttribute srcAttribute = htmlScript.Attributes["src"];
	  if (srcAttribute == null || String.IsNullOrEmpty(srcAttribute.Value))
		return;
	  string pathPart = srcAttribute.Value;
	  if (IsHttpPath(pathPart))
		return;
	  SourceFile fileNode = htmlScript.FileNode;
	  try
	  {
		string currentFolder = Path.GetDirectoryName(fileNode.Name);
		pathPart = Path.Combine(currentFolder, pathPart);
		pathPart = Path.GetFullPath(pathPart);
		StringCollection includedScriptFiles = fileNode.IncludedScriptFiles;
		if (includedScriptFiles != null && !includedScriptFiles.Contains(pathPart))
		  includedScriptFiles.Add(pathPart);
	  }
	  catch (Exception e)
	  {
		Log.SendException("Exception while trying to get full script source path", e);
		Log.Send(srcAttribute.Value);
		return;
	  }
	}
	#endregion
	#region HtmlScanner
	protected HtmlScanner HtmlScanner
	{
	  get
	  {
		return (HtmlScanner)scanner;
	  }
	}
	#endregion
	public override string Language
	{
	  get
	  {
		return "HTML";
	  }
	}
		#region DefaultScriptLanguage
		public ScriptLanguageType DefaultScriptLanguage
		{
			get
			{
				return _DefaultScriptLanguage;
			}
	  set
	  {
		_DefaultScriptLanguage = value;
	  }
		}
		#endregion
		#region DefaultDotNetLanguage
		public  DotNetLanguageType DefaultDotNetLanguage
		{			
			set
			{
				_DefaultDotNetLanguage = value;
			}
			get
			{
				return _DefaultDotNetLanguage;
			}			
		}
		#endregion
	}
}
