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
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class LanguageElementCodeGenBase : CodeGenObject
  {
	CodeGen _CodeGen;
	internal abstract class DisposableOption : IDisposable
	{
	  readonly bool _NeedApplyOption;
	  readonly CodeGen _CodeGen;
	  public DisposableOption(bool needApply, CodeGen codeGen)
	  {
		_NeedApplyOption = needApply;
		_CodeGen = codeGen;
		if (_NeedApplyOption && _CodeGen != null)
		  SetOption(_CodeGen);
	  }
	  protected abstract void SetOption(CodeGen codeGen);
	  protected abstract void ClearOption(CodeGen codeGen);
	  public void Dispose()
	  {
		if (_CodeGen == null || !_NeedApplyOption)
		  return;
		ClearOption(_CodeGen);
	  }
	}
	internal class Indention : DisposableOption
	{
	  public Indention(bool needApply, CodeGen codeGen)
		: base(needApply, codeGen)
	  {
	  }
	  protected override void SetOption(CodeGen codeGen)
	  {
		codeGen.AddIncreaseIndent();
	  }
	  protected override void ClearOption(CodeGen codeGen)
	  {
		codeGen.AddDecreaseIndent();
	  }
	}
	internal class ClearIndention : DisposableOption
	{
	  public ClearIndention(bool needApply, CodeGen codeGen)
		: base(needApply, codeGen)
	  {
	  }
	  protected override void SetOption(CodeGen codeGen)
	  {
		codeGen.AddClearIndent();
	  }
	  protected override void ClearOption(CodeGen codeGen)
	  {
		codeGen.AddRestoreIndent();
	  }
	}
	public LanguageElementCodeGenBase(CodeGen codeGen)
	{
	  if (codeGen == null)
		throw new ArgumentNullException("codeGen");
	  _CodeGen = codeGen;
	}
	bool FindElement(LanguageElementType[] list, LanguageElement element)
	{
	  if (list == null || element == null)
		return false;
	  int lCount = list.Length;
	  for (int i = 0; i < lCount; i++)
		if (list[i] == element.ElementType)
		  return true;
	  return false;
	}
	bool WillSkipOtherElements(ICollection parameters, LanguageElementType[] excludeElements, int paramIndex)
	{
	  if (parameters == null)
		return false;
	  int currentIndex = 0;
	  bool hasSkippedElements = false;
	  foreach (LanguageElement element in parameters)
	  {
		if (currentIndex++ <= paramIndex)
		  continue;
		if (!(SkipElement(element) || FindElement(excludeElements, element)))
		  return false;
		hasSkippedElements = true;
		continue;
	  }
	  return hasSkippedElements;
	}
	void WriteDelimiter(FormattingTokenType[] delimiters, ref int delimiterIndex)
	{
	  int contextsIndex = CodeGen.Contexts.Count - 1;
	  int contextsCount = CodeGen.Contexts.Count;
	  if(contextsCount > 1 && CodeGen.Contexts[contextsCount - 2] is GenericModifier )
		contextsIndex -= 2;
	  for (int i = 0; i < delimiters.Length; i++)
	  {
		FormattingTokenType delimiter = delimiters[i];
		if (delimiter == FormattingTokenType.None)
		  continue;
		Write(delimiter, contextsIndex, delimiterIndex, null);
		delimiterIndex++;
	  }
	}
	void AddLineBreakAfterDelimiterIfNeeded(bool needLineBreak, LanguageElement generated)
	{
	  if (!needLineBreak || generated is SnippetCodeElement)
		return;
	  Variable var = generated as Variable;
	  if (var != null && var.NextVariable != null)
		return;
	  CodeGen.AddNewLineIfNeeded();
	}
	protected string GetLineContinuation()
	{
	  string lineContinuation = LineContinuation;
	  string codeLastLine = Code.LastLine;
	  if (string.IsNullOrEmpty(codeLastLine) || string.IsNullOrEmpty(lineContinuation))
		return lineContinuation;
	  char lastChar = codeLastLine[codeLastLine.Length - 1];
	  if (lineContinuation.StartsWith(lastChar.ToString()))
		return lineContinuation.Remove(0, 1);
	  return lineContinuation;
	}
	protected bool SkipElement(LanguageElement element)
	{
	  if (element == null)
		return true;
	  if (CodeGen.IsSkiped(element))
		return true;
	  Comment comment = element as Comment;
	  if (comment != null && comment.ElementType != LanguageElementType.XmlDocComment)
		return comment.TargetNode != null && comment.TargetNode.Parent != null && comment.Position != SupportElementPosition.Inside;
	  AttributeSection attributeSection = element as AttributeSection;
	  if (attributeSection != null && attributeSection.TargetNode != null &&
		!(attributeSection.TargetNode is SourceFile) &&
		!(attributeSection.Parent is ElementList))
		return true;
	  return false;
	}
	protected bool ContextMatch(LanguageElementType type)
	{
	  return Context.ElementType == type;
	}
	protected virtual void Write(string text)
	{
	  Write(FormattingTokenType.Ident, text);
	}
	protected virtual void Write(string text, bool split)
	{
	  TokenGen.WriteCode(new GenTextArgs(CodeGen, text, split));
	}
	protected virtual void IncreaseIndent()
	{
	  CodeGen.AddIncreaseIndent();
	}
	protected virtual void DecreaseIndent()
	{
	  CodeGen.AddDecreaseIndent();
	}
	protected IDisposable GetIndent(bool needIndent)
	{
	  return new Indention(needIndent, CodeGen);
	}
	protected IDisposable GetIndent()
	{
	  return GetIndent(Options.Indention.CodeBlockContents);
	}
	protected IDisposable GetClearIndent(bool needClear)
	{
	  return new ClearIndention(needClear, CodeGen);
	}
	internal bool ElementWasGenerated(LanguageElement element)
	{
	  return CodeGen.ElementWasGenerated(element);
	}
	internal void AddGeneratedElement(LanguageElement element)
	{
	  CodeGen.AddGeneratedElement(element);
	}
	internal void RemoveGeneratedElement(LanguageElement element)
	{
	  CodeGen.RemoveGeneratedElement(element);
	}
	public virtual bool GenerateElementTail(LanguageElement element)
	{
	  return false;
	}
	public virtual FormattingElements NextFormattingElements(FormattingTokenType tokenType)
	{
	  return CodeGen.NextFormattingElements(tokenType);
	}
	public virtual FormattingElements PrevFormattingElements(FormattingTokenType tokenType)
	{
	  return CodeGen.PrevFormattingElements(tokenType);
	}
	public virtual void GenerateElementCollection(ICollection parameters, FormattingTokenType delimiter, bool lineBreakAfterDelimiter, params LanguageElementType[] excludeElements)
	{
	  GenerateElementCollection(parameters, new FormattingTokenType[] { delimiter }, lineBreakAfterDelimiter, excludeElements);
	}
	public virtual void GenerateElementCollection(ICollection parameters, FormattingTokenType[] delimiters, bool lineBreakAfterDelimiter, params LanguageElementType[] excludeElements)
	{
	  int count = parameters.Count;
	  if (count < 1)
		return;
	  int delimiterIndex = 0;
	  int paramIndex = -1;
	  foreach (LanguageElement element in parameters)
	  {
		paramIndex++;
		if (SkipElement(element))
		  continue;
		if (FindElement(excludeElements, element))
		  continue;
		CodeGen.GenerateElement(element);
		if (paramIndex < count - 1 && !WillSkipOtherElements(parameters, excludeElements, paramIndex))
		  WriteDelimiter(delimiters, ref delimiterIndex);
		AddLineBreakAfterDelimiterIfNeeded(lineBreakAfterDelimiter, element);
	  }
	}
	public virtual void GenerateParameters(ICollection parameters)
	{
	  Write(FormattingTokenType.ParenOpen);
	  GenerateElementCollection(parameters, FormattingTokenType.Comma);
	  Write(FormattingTokenType.ParenClose);
	}
	public override bool IsSkiped(LanguageElement element)
	{
	  return _CodeGen.IsSkiped(element);
	}
	public override void AddSkiped(LanguageElement element)
	{
	  _CodeGen.AddSkiped(element);
	}
	public override void ClearSkipedElements()
	{
	  _CodeGen.ClearSkipedElements();
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public CodeGen GetCodeGen()
	{
	  return CodeGen;
	}
	public void GenerateElementCollection(ICollection parameters)
	{
	  GenerateElementCollection(parameters, FormattingTokenType.None);
	}
	public void GenerateElementCollection(ICollection parameters, FormattingTokenType delimiter)
	{
	  GenerateElementCollection(parameters, new FormattingTokenType[] { delimiter }, false, null);
	}
	public void GenerateElementCollection(ICollection parameters, FormattingTokenType[] delimiters)
	{
	  GenerateElementCollection(parameters, delimiters, false, null);
	}
	public void GenerateElementCollection(ICollection parameters, FormattingTokenType delimiter, params LanguageElementType[] excludeElements)
	{
	  GenerateElementCollection(parameters, delimiter, false, excludeElements);
	}
	public void GenerateElementCollection(ICollection parameters, FormattingTokenType delimiter, bool lineBreakAfterDelimiter)
	{
	  GenerateElementCollection(parameters, delimiter, lineBreakAfterDelimiter, null);
	}
	public void Write(TokenList types)
	{
	  if (types == null)
		return;
	  foreach (FormattingTokenType type in types)
		Write(type, 0);
	}
	public void Write(params FormattingTokenType[] types)
	{
	  if (types == null)
		return;
	  foreach (FormattingTokenType type in types)
	  {
		Write(type, 0);
	  }
	}
	public void Write(FormattingTokenType type)
	{
	  Write(type, 0);
	}
	public void Write(FormattingTokenType type, int index)
	{
	  TokenGen.WriteCode(new GenTextArgs(CodeGen, type, index));
	}
	public void Write(FormattingTokenType type, string tokenText)
	{
	  Write(new GenTextArgs(CodeGen, type, 0, tokenText));
	}
	public void Write(FormattingTokenType type, int index, string tokenText)
	{
	  TokenGen.WriteCode(new GenTextArgs(CodeGen, type, index, tokenText));
	}
	public void Write(FormattingTokenType type, int contextsIndex, int index, string tokenText)
	{
	  TokenGen.WriteCode(new GenTextArgs(CodeGen, contextsIndex, type, index, tokenText));
	}
	public void Write(GenTextArgs args)
	{
	  TokenGen.WriteCode(args);
	}
	public void CodeWritePrevFormattingElements(FormattingTokenType type)
	{
	  TokenGen.WritePrevFormattingElements(type, 0);
	}
	public void CodeWriteNextFormattingElements(FormattingTokenType type)
	{
	  TokenGen.WriteNextFormattingElements(type, 0);
	}
	protected override CodeGen CodeGen
	{
	  get
	  {
		return _CodeGen;
	  }
	}
	protected FormattingTokenGen TokenGen
	{
	  get
	  {
		return CodeGen.TokenGen;
	  }
	}
	public override CodeGenOptions Options
	{
	  get
	  {
		return _CodeGen.Options;
	  }
	}
	public LanguageElement Context
	{
	  get
	  {
		return _CodeGen.Context;
	  }
	}
	#region Old string style
	bool SkipElementOld(LanguageElement element)
	{
	  if (element == null)
		return true;
	  if (CodeGen.IsSkiped(element))
		return true;
	  AttributeSection attributeSection = element as AttributeSection;
	  if (attributeSection != null && attributeSection.TargetNode != null && !(attributeSection.TargetNode is SourceFile))
		return true;
	  return false;
	}
	public void GenerateParameters(ICollection parameters, string delimiter)
	{
	  GenerateElementCollection(parameters, delimiter, false, null);
	}
	public void GenerateElementCollection(ICollection parameters, string delimiter)
	{
	  GenerateElementCollection(parameters, delimiter, false, null);
	}
	public void GenerateElementCollection(ICollection parameters, string delimiter, params LanguageElementType[] excludeElements)
	{
	  GenerateElementCollection(parameters, delimiter, false, excludeElements);
	}
	public void GenerateElementCollection(ICollection parameters, string delimiter, bool lineBreakAfterDelimiter)
	{
	  GenerateElementCollection(parameters, delimiter, lineBreakAfterDelimiter, null);
	}
	public virtual void GenerateElementCollection(ICollection parameters, string delimiter, bool lineBreakAfterDelimiter, params LanguageElementType[] excludeElements)
	{
	  int count = parameters.Count;
	  if (count < 1)
		return;
	  bool first = true;
	  bool prevElementIsSnippet = false;
	  bool prevElementIsAttributeSection = false;
	  foreach (LanguageElement element in parameters)
	  {
		if (SkipElementOld(element))
		  continue;
		if (FindElement(excludeElements, element))
		  continue;
		if (!first)
		{
		  Code.Write(delimiter);
		  if (lineBreakAfterDelimiter && !prevElementIsSnippet)
		  {
			if (prevElementIsAttributeSection)
			  CodeGen.TokenGen.AddNewLineIfNeeded();
			else
			  Code.WriteLine();
		  }
		}
		CodeGen.GenerateCode(Code, element);
		first = false;
		prevElementIsSnippet = element is SnippetCodeElement ||
		  element is SnippetCodeStatementBlock;
		if (!prevElementIsSnippet)
		{
		  SnippetCodeStatementBlock snippet = element.LastChild as SnippetCodeStatementBlock;
		  prevElementIsSnippet = snippet != null && snippet.AddNewLineAfter;
		}
		prevElementIsAttributeSection = element is AttributeSection;
	  }
	}
	protected void WriteOpenBrace(bool onNewLine, bool spaceBefore, bool spaceAfter, bool increaseIndent)
	{
	  bool indentBefore = Options.Indention.OpenAndCloseBraces;
	  if (indentBefore)
		Code.IncreaseIndent();
	  if (onNewLine)
		Code.WriteLine();
	  else
		if (spaceBefore)
		  Code.Write(" ");
	  Code.Write("{");
	  if (spaceAfter)
		Code.Write(" ");
	  if (indentBefore)
		Code.DecreaseIndent();
	  if (increaseIndent)
		Code.IncreaseIndent();
	}
	protected void WriteCloseBrace(bool onNewLine, bool spaceBefore, bool decreaseIndent)
	{
	  bool indentBefore = Options.Indention.OpenAndCloseBraces;
	  if (indentBefore)
		Code.IncreaseIndent();
	  if (decreaseIndent)
		Code.DecreaseIndent();
	  if (onNewLine)
		Code.WriteLine();
	  else
		if (spaceBefore)
		  Code.Write(" ");
	  Code.Write("}");
	  if (indentBefore)
		Code.DecreaseIndent();
	}
	protected string GetCommaDelimeter()
	{
	  return GetCommaDelimeter(string.Empty);
	}
	protected string GetCommaDelimeter(string endOfLineChar)
	{
	  string delimeter = ",";
	  if (Options.Spacing.BeforeComma)
		delimeter = " " + delimeter;
	  if (Options.Spacing.AfterComma)
	  {
		if (endOfLineChar.StartsWith(" "))
		  delimeter += endOfLineChar;
		else
		  delimeter += " ";
	  }
	  else
		if (!string.IsNullOrEmpty(endOfLineChar))
		  delimeter += endOfLineChar;
	  return delimeter;
	}
	protected void WriteOpenParen(bool spaceBefore, bool spaceAfter)
	{
	  if (spaceBefore)
		Code.Write(" ");
	  Code.Write("(");
	  if (spaceAfter)
		Code.Write(" ");
	}
	protected void WriteCloseParen(bool spaceBefore)
	{
	  if (spaceBefore)
		Code.Write(" ");
	  Code.Write(")");
	}
	protected void WriteLineTerminator<T>(bool needWrite, LanguageElement element)
	{
	  if (!needWrite)
		return;
	  if (element == null)
		return;
	  LanguageElement nextCodeSibling = element.NextCodeSibling;
	  if (nextCodeSibling == null)
		return;
	  if (nextCodeSibling is T || nextCodeSibling is SourceFile)
		return;
	  Code.WriteLine();
	}
	protected void WriteDot()
	{
	  if (Options.Spacing.BeforeDot)
		Code.Write(" ");
	  Code.Write(".");
	  if (Options.Spacing.AfterDot)
		Code.Write(" ");
	}
	protected void WriteOperator(string op)
	{
	  if (string.IsNullOrEmpty(op))
		return;
	  bool within = op.Length == 1 ? Options.Spacing.AroundOneCharOperators : Options.Spacing.AroundTwoCharOperators;
	  if (within)
		Code.Write(" ");
	  Code.Write(op);
	  if (within)
		Code.Write(" ");
	}
	protected void WriteSemicolon()
	{
	  WriteSemicolon(true );
	}
	protected void WriteSemicolon(bool spacingAfter)
	{
	  WriteWhithing(Options.Spacing.BeforeSemicolon, ";", spacingAfter);
	}
	protected void WriteWhithing(bool before, string text, bool after)
	{
	  if (before)
		Code.Write(" ");
	  Code.Write(text);
	  if (after)
		Code.Write(" ");
	}
	public void GenerateElementsByRules(NodeList parameters, ElementsGenerationRules rules)
	{
	  if (parameters == null)
		return;
	  int count = parameters.Count;
	  if (count <= 0)
		return;
	  bool isFirst = true;
	  if (rules.WrapFirst)
		Code.WriteLine(LineContinuation);
	  bool indentFirst = rules.Indenting && rules.WrapFirst;
	  bool indentSecond = rules.Indenting && rules.WrapParams;
	  string firstSpaces = Code.LastLineInSpacesWithouIndent;
	  if (indentFirst)
		Code.IncreaseIndent();
	  foreach (LanguageElement element in parameters)
	  {
		if (!isFirst)
		  PrepareNextElementSpaces(firstSpaces, rules);
		CodeGen.GenerateElement(element);
		if (isFirst && !indentFirst && indentSecond)
		  Code.IncreaseIndent();
		isFirst = false;
	  }
	  if (indentFirst || indentSecond)
		Code.DecreaseIndent();
	}
	void PrepareNextElementSpaces(string firstSpaces, ElementsGenerationRules rules)
	{
	  if (rules.HasStringDelimiter)
		Code.Write(rules.StringDelimiter);
	  if (rules.WrapParams)
	  {
		Code.WriteLine(LineContinuation);
		if (rules.AlignIfWrap)
		  Code.Write(firstSpaces);
	  }
	}
	#endregion
  }
}
