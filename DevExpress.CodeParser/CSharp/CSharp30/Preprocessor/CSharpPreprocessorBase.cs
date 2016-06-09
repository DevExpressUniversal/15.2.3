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
using System.Collections.Generic;
using System.Text;
#if !DXCORE
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp.Preprocessor
#else
namespace DevExpress.CodeParser.CSharp.Preprocessor
#endif
{
	public enum PreprocessMode
	{
		None,
		SimplePreprocessing,
		SmartPreprocessing
	}
	public abstract class PreprocessorBase
	{
		Stack _IfConditions;
		FormattingParserBase _Parser;
		const int minErrDist = 2;
		protected const bool T = true;
		protected const bool x = false;
		protected int errDist = minErrDist;
		protected Token tToken;	
		protected Token la;   
		protected ParserErrorsBase errors;
		protected bool[,] set;
		protected int maxTokens;
		SourceFile _SourceFile;
	bool _AddFormattingTokens = true;
	CSharpScanner _Scanner;
		protected abstract void StartRule();
		protected abstract bool[,] CreateSetArray();
		protected abstract void Get();
		Dictionary<string, string> _Defines;
		Dictionary<string, object> _OnlyThisFileDefines;
		PreprocessMode _PreprocessMode = PreprocessMode.SimplePreprocessing;
	public FormattingParsingElement ActiveParsingElement;
	public bool AddFormattingTokens
	{
	  get { return _AddFormattingTokens; }
	  set { _AddFormattingTokens = value; }
	}
		protected FormattingParserBase Parser
	{
	  get { return _Parser; }
	  set { _Parser = value; }
	}
	protected CSharpScanner Scanner
	{
	  get { return _Scanner; }
	  set { _Scanner = value; }
	}
	protected PreprocessMode PreprocessMode
	{
	  get { return _PreprocessMode; }
	  set { _PreprocessMode = value; }
	}
		public PreprocessorBase(CSharpScanner scanner, SourceFile rootNode)
		{
			_IfConditions = new Stack();
			_Defines = new Dictionary<string, string>();
			_OnlyThisFileDefines = new Dictionary<string, object>();
			_Scanner = scanner;
			_SourceFile = rootNode;
			AddProjectDefines(rootNode);
		}
		void AddDefineCall(string name)
		{
			if (String.IsNullOrEmpty(name) || _OnlyThisFileDefines.ContainsKey(name) || _SourceFile == null)
				return;
			string fileName = _SourceFile.Name;
			if (String.IsNullOrEmpty(name))
				return;
			_SourceFile.AddInvalidateMacro(name);
		}
		void AddProjectDefines(SourceFile sourceFile)
		{
			if (sourceFile == null)
				return;
	  IProjectElement project = sourceFile.Project;
	  if (project == null)
				return;
			string[] projectDefines = project.Defines;
			if (projectDefines == null || projectDefines.Length == 0)
				return;
			foreach (string def in projectDefines)
				_Defines.Add(def, def);
		}
	protected string GetDirectiveExpression(Token startToken, Token endToken)
	{
	  if (startToken == null)
		return string.Empty;
	  Token currentToken = startToken;
	  StringBuilder result = new StringBuilder(startToken.Value);
	  while (currentToken != endToken && currentToken != null)
	  {
		result.Append(GetFEText(((FormattingToken)currentToken).FormattingElements));
		currentToken = currentToken.Next;
		if(currentToken != null)
		  result.Append(currentToken.Value);
	  }
	  return result.ToString();
	}
	List<object> _PreprocessorTokens;
	protected List<object> PreprocessorTokens
	{
	  get
	  {
		if (_PreprocessorTokens == null)
		  _PreprocessorTokens = new List<object>();
		return _PreprocessorTokens;
	  }
	}
	protected void ProcessFormattingToken()
	{
	  int type = la.Type;
	  if (!AddFormattingTokens || type == Tokens.LINETERMINATOR || type == Tokens.ELSEDIR || type == Tokens.ENDIF
		|| type == Tokens.ENDREG || type == Tokens.IDENT)
		return;
	 Parser.AddFormattingElements(la as FormattingToken);
	}
	protected bool ProcessLineTerminator(FormattingToken token)
	{
	  if (token == null || token.Type != Tokens.LINETERMINATOR)
		return false;
	  AddToActiveParsingElement(token.FormattingElements);
	  return true;
	}
	protected void AddLineTerminatorToFormattingTokens()
	{
	  AddToActiveParsingElement(new FormattingElement(FormattingElementType.EOL));
	}
	protected void ProcessSingleLineComment(FormattingToken token)
	{
	  if (token == null || token.Type != Tokens.SINGLELINECOMMENT)
		return;
	  Parser.AddCommentNode(token, ActiveParsingElement);
	  Get();
	}
	void AddToActiveParsingElement(IFormattingElement element)
	{
	  Parser.AddToParsingElement(ActiveParsingElement, element);
	}
	void AddToActiveParsingElement(FormattingElements elements)
	{
	  Parser.AddToParsingElement(ActiveParsingElement, elements);
	}
	string GetTokenValue(FormattingToken token)
	{
	  if (token == null)
		return String.Empty;
	  StringBuilder result = new StringBuilder(token.Value);
	  result.Append(GetFEText(token.FormattingElements));
	  return result.ToString();
	}
	internal string GetFEText(FormattingElements fes)
	{
	  StringBuilder result = new StringBuilder();
	  if (fes == null)
		return String.Empty;
	  foreach (object element in fes)
	  {
		FormattingElement fte = element as FormattingElement;
		if (fte == null)
		  continue;
		if (fte.Type == FormattingElementType.WS)
		  result.Append(' ');
		else if (fte.Type == FormattingElementType.Tab)
		  result.Append('\t');
		else if (fte.Type == FormattingElementType.EOL)
		  result.Append("\r\n");
	  }
	  return result.ToString();
	}
		protected void DefineMacro(string name)
		{
			if (String.IsNullOrEmpty(name))
				return;
			if (!_OnlyThisFileDefines.ContainsKey(name))
				_OnlyThisFileDefines.Add(name, null);
			if (_Defines.ContainsKey(name))
				return;
			_Defines.Add(name, name);
		}
		protected void AddTokenToCategoryCollectionIfNeeded()
		{
			if (_Parser != null)
				_Parser.SaveCategorizedTokenIfNeeded(la);
		}
		protected void UndefMacro(string name)
		{
			if (String.IsNullOrEmpty(name))
				return;
			AddDefineCall(name);
			if (!_Defines.ContainsKey(name))
				return;
			_Defines.Remove(name);
		}
		#region Expect
		protected void Expect(int n)
		{
			if (la.Type == n)
				Get();
			else
				SynErr(n);
		}
		#endregion
		#region SynErr
		protected void SynErr(int n)
		{
			if (errDist >= minErrDist)
				errors.SynErr(la.Line, la.Column, n);
			errDist = 0;
		}
		#endregion
		#region StartOf
		protected bool StartOf(int s)
		{
			return set[s, la.Type];
		}
		#endregion
		public Token Preprocess(Token laToken)
		{
	  PreprocessorTokens.Clear();
			la = laToken;
			StartRule();
			return la;
		}
		protected SourceFile SourceFile
		{
			get
			{
				return _SourceFile;
			}
			set
			{
				_SourceFile = value;
			}
		}
		bool IsEOF(StringBuilder bufferString)
		{
	  return _Scanner.IsEof(bufferString);
		}
	bool IsNextChar(char ch, StringBuilder bufferString)
		{
	  return _Scanner.IsNextChar(ch, bufferString);
		}
	FormattingElements CutPPFormElements(FormattingToken token)
	{
	  FormattingElements elements = token.FormattingElements;
	  FormattingElements result = new FormattingElements();
	  if (elements == null || elements.Count == 0)
		return result;
	  int i = 0;
	  for (; i < elements.Count; i++)
	  {
		FormattingElement fte = elements[i] as FormattingElement;
		if (fte.Type == FormattingElementType.EOL)
		{
		  result = elements.GetElements(i + 1, elements.Count - (i + 1));
		  elements.RemoveRange(i , elements.Count - i);
		  return result;
		}
	  }
	  return result;
	}
	protected void ProcessNonNewLineToken(FormattingToken t)
	{
	  if (t == null)
		return;
	  string val = t.Value;
	  if (!val.StartsWith("#"))
		return;
	  val = val.Remove(0, 1);
	  val = val.TrimStart();
	  while (true)
	  {
		if (String.IsNullOrEmpty(val))
		  break;
		char ch = val[0];
		if (!Char.IsLetter(ch))
		  break;
		val = val.Remove(0, 1);
	  }
	  if (!String.IsNullOrEmpty(val))
		AddToActiveParsingElement(new InsignificantDirectiveText(val));
	}
		void IfWaitEndIf()
		{
			int count = 0;
			bool parsingWasNotInPrep = la.Type != Tokens.LINETERMINATOR;
	  StringBuilder text = new StringBuilder(); 
	  try
	  {
		if (la.Type != Tokens.LINETERMINATOR)
		{
		  FormattingElements fe = null;
		  fe = CutPPFormElements(tToken as FormattingToken);
		  text.Append(GetFEText(fe));
		  if (la.Type == Tokens.ELSEDIR || la.Type == Tokens.ELIF || la.Type == Tokens.ENDIF)
			return;
		  if (la.Type == Tokens.IFDIR)
		  {
			text.Append("#if ");
			AddTokenToCategoryCollectionIfNeeded();
			count++;
		  }
		  else
			text.Append(GetTokenValue(la as FormattingToken));
		}
		else
		{
		  text.Append(GetFEText(((FormattingToken)la).FormattingElements));
		}
		while (true)
		{
		  StringBuilder skipedString = new StringBuilder();
		  if (IsEOF(skipedString))
		  {
			text.Append(skipedString.ToString());
			return;
		  }
		  if (IsNextChar('#', skipedString))
		  {
			tToken = la;
			bool oldValue = _Scanner.SaveFormat;
			_Scanner.SaveFormat = true;
			la = _Scanner.Scan();
			_Scanner.SaveFormat = oldValue;
			AddTokenToCategoryCollectionIfNeeded();
			if (la.Type == Tokens.EOF)
			  return;
			if (la.Type == Tokens.IFDIR)
			  count++;
			if (la.Type == Tokens.ELSEDIR || la.Type == Tokens.ELIF)
			{
			  if (count == 0)
				return;
			}
			if (la.Type == Tokens.ENDIF)
			{
			  if (count == 0)
				return;
			  count--;
			}
			text.Append(skipedString.ToString());
			text.Append(GetTokenValue(la as FormattingToken));
		  }
		else
		  {
			text.Append(skipedString.ToString());
			text.Append(SkipToEOL());
		  }
		}
	  }
	  finally
	  {
		AddToActiveParsingElement(new DeadCode(text.ToString()));
	  }
		}
		protected void SkipToEolIfNeeded()
		{
			if (la.Type == Tokens.LINETERMINATOR || la.Type == Tokens.EOF)
				return;
			string text = SkipToEOL();
		}
		protected void ProcessIFDirectiveCondition(bool condition)
		{
			if (_PreprocessMode == PreprocessMode.None)
				return;
			IfConditions.Push(condition);
			if (!condition)
			{
				IfWaitEndIf();
			}
		}
		protected bool WillSkipInIf(bool result)
		{
			if (_PreprocessMode == PreprocessMode.None)
				return false;
			return !result;
		}
		protected bool WillSkip(bool result)
		{
			if (_PreprocessMode == PreprocessMode.None)
				return false;
			if (ConditionWasTrue)
				return true;
			if (result)
				return false;
			return true;
		}
		void ProcessDirectiveConditionCore(bool condition)
		{
			if (condition)
			{
				if (IfConditions.Count > 0)
					IfConditions.Pop();
				IfConditions.Push(true);
				return;
			}
			IfWaitEndIf();
		}
		protected void ProcessDirectiveCondition(bool condition)
		{
			if (_PreprocessMode == PreprocessMode.None)
				return;
			if (ConditionWasTrue)
				IfWaitEndIf();
			else
				ProcessDirectiveConditionCore(condition);
		}
		protected string GetNextToken()
		{
			if (la == null)
				return null;
			string result = la.Value;
			Get();
			return result;
		}
		protected bool IsDefineMacro(string name)
		{
			if (String.IsNullOrEmpty(name))
				return false;
			AddDefineCall(name);
			return _Defines.ContainsKey(name);
		}
		protected Stack IfConditions
		{
			get
			{
				return _IfConditions;
			}
		}
		public bool ConditionWasTrue
		{
			get
			{
				if (_IfConditions.Count > 0)
					return (bool)_IfConditions.Peek();
				else
					return false;
			}
		}
	protected void AddPreprocessorDirective(PreprocessorDirective directive)
	{
	  if (directive == null)
		return;
	  SourceFile sourceFile = SourceFile;
	  if (sourceFile == null)
		return;
	  if (Parser != null && Parser.SaveFormat)
	  {
		AddToActiveParsingElement(directive);
		FormattingToken fToken = tToken as FormattingToken;
		if (fToken != null)
		  AddToActiveParsingElement(fToken.FormattingElements);
	  }
	  sourceFile.AddSimpleDirective(directive, false);
	  CompilerDirective compilerDirective = sourceFile.CompilerDirectiveRootNode;
	  if (compilerDirective == null)
		return;
	  compilerDirective.AddNode(directive);
	}
		protected void SetDirectiveRange(LanguageElement directive, SourceRange range)
		{
			range = SourceRangeUtils.GetRange(range, tToken.Range);
			directive.SetRange(range);
		}
		protected void TurnOnScannerPreprocessMode()
		{
			_Scanner.InPreprocess = true;
		}
		protected void TurnOffScannerPreprocessMode()
		{
			_Scanner.InPreprocess = false;
		}
		protected string SkipToEOL()
		{
			return _Scanner.SkipToEOL();
		}
	public void CleanUp()
	{
	  _Defines.Clear();
	  _IfConditions.Clear();
	  _SourceFile = null;
	  _Scanner = null;
	  tToken = null;
		  la = null;
		  errors = null;
		  set = null;
	}
	void ProcessNonEndLineToken(FormattingToken t)
	{
	  if (t == null)
		return;
	  string val = t.Value;
	}
		protected void ProcessEndIf()
		{
			if (IfConditions.Count <= 0)
				return;
			IfConditions.Pop();
		}
	}
}
