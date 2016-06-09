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
using System.ComponentModel;
#if SL
using DXEncoding = DevExpress.Utils.DXEncoding;
#else
using DXEncoding = System.Text.Encoding;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Diagnostics;
	using System.Collections;
	public abstract class ParserBase
	{
		#region private fields...
		double _ParseTime;
		bool _IsParsing;
		ExpressionParserBase _ExpressionParser;
		LanguageElement _RootContext;
		LanguageElement _Context;
		TextStringCollection _TextStrings;
		RegionDirective _RegionContext;
		CompilerDirective _CompilerDirectiveContext;
		IDocument _Document;
		SourceRange _ParseRange;
		bool _AllowCommentsInParseTree = true;
		object _SyncObject = new object();
	bool _LastParsingHasEOF = true;
		#endregion
		#region protected fields...
	protected ParserErrorsBase parserErrors;
		#endregion
		#region ParserBase
		public ParserBase()
		{
			_ExpressionParser = CreateExpressionParser();
		}
		#endregion
		void CorrectContextRange(LanguageElement context, CommentCollection remainingComments)
		{
			if (context == null)
				return;
			SourcePoint lEnd = context.InternalRange.End;
			LanguageElement lLastChild = context.LastChild;
			if (lLastChild != null && lLastChild.InternalRange.End.IsEmpty)
				lLastChild = lLastChild.LastChild;
			if (lLastChild != null && lLastChild.InternalRange.End > lEnd)
				lEnd = lLastChild.InternalRange.End;
			if (remainingComments != null)
			{
				Comment lLastComment = remainingComments.LastComment;
				if (lLastComment != null && lLastComment.InternalRange.End > lEnd)
					lEnd = lLastComment.InternalRange.End;
			}
			if (context.InternalRange.End != lEnd)
				context.SetEnd(lEnd);
		}
		void ProcessTextStrings()
		{
			SourceFile fileNode = null;
			if (_RootContext == null)
				return;
			if (_RootContext is SourceFile)
				fileNode = (SourceFile)_RootContext;
			else
				fileNode = _RootContext.FileNode;
			if (fileNode == null)
				return;
			TextStringCollection strings = fileNode.TextStrings;
			if (strings == null || strings.Count == 0)
				return;
			int count = strings.Count;
			for (int i = 0; i < count; i++)
			{
				TextString str = strings[i];
				if (str == null)
					continue;
				str.SetParent(fileNode);
			}
		}
		#region CreateExpressionParser
		public abstract ExpressionParserBase CreateExpressionParser();
		#endregion
		#region CreateExpressionInverter
		public abstract IExpressionInverter CreateExpressionInverter();
		#endregion
		#region PrepareForParse
		protected virtual void PrepareForParse(ParserContext parserContext)
		{
			_ParseTime = -1;
			_Context = parserContext.Context;
			_RootContext = _Context;
			_CompilerDirectiveContext = parserContext.CompilerDirectiveContext;
			_RegionContext = parserContext.RegionContext;
			_ParseRange = parserContext.ParseRange;
			_Document = parserContext.Document;
			_TextStrings = parserContext.TextStrings;
			_AllowCommentsInParseTree = parserContext.AllowCommentsInParseTree;
		}
		#endregion
		#region FinishParsing
		protected virtual void FinishParsing()
		{
			if (AllowCommentsInParseTree)
				IntroduceComments(RootContext);
			if (Context != null && (Context is SourceFile || Context is Namespace))
			{
				CorrectContextRange(Context, null);
			}
			GlobalStringStorage.Clear();
			_IsParsing = false;
		}
		#endregion
		#region CleanUpInternalData
		protected virtual void CleanUpInternalData()
		{
			_Context = null;
			_CompilerDirectiveContext = null;
			_RegionContext = null;
			_Document = null;
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual InitializedVariable CreateInitializedVariable()
		{
			return new InitializedVariable();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual Variable CreateVariable()
		{
			return new Variable();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual Const CreateConst()
		{
			return new Const();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void LinkVariablesList(LanguageElementCollection variables)
		{
			if (variables == null || variables.Count == 0)
				return;
			int count = variables.Count;
			Variable ancestor = variables[0] as Variable;
			for (int i = 0; i < count; i++)
			{
				Variable variable = variables[i] as Variable;
				if (variable == null)
					continue;
				if (i > 0)
				{
					variable.SetAncestorVariable(ancestor);
					variable.SetPreviousVariable(variables[i - 1] as Variable);
				}
				if (i < count - 1)
					variable.SetNextVariable(variables[i + 1] as Variable);
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void SetVariableTypes(LanguageElementCollection variables)
		{
		}
		#region DoParse(ParserContext parserContext, ISourceReader reader)
		protected virtual LanguageElement DoParse(ParserContext parserContext, ISourceReader reader)
		{
			double lParseTime = _ParseTime;
			LanguageElement lContext = _Context;
			LanguageElement lRootContext = _RootContext;
			TextStringCollection lTextStrings = _TextStrings;
			RegionDirective lRegionContext = _RegionContext;
			CompilerDirective lCompilerDirectiveContext = _CompilerDirectiveContext;
			IDocument lDocument = _Document;
			LanguageElement lResult = Context;
			try
			{
				PrepareForParse(parserContext);
				SetupSourceFile();
			}
			finally
			{
				FinishParsing();
				ProcessTextStrings();
				reader.Dispose();
				lResult = _Context;
				if (_RootContext is SourceFile)
					lResult = _RootContext;
				_AllowCommentsInParseTree = true;
				_ParseTime = lParseTime;
				_Context = lContext;
				_RootContext = lRootContext;
				_TextStrings = lTextStrings;
				_RegionContext = lRegionContext;
				_CompilerDirectiveContext = lCompilerDirectiveContext;
				_Document = lDocument;
				CleanUpInternalData();
			}
			if (lResult == null)
				lResult = parserContext.Context;
			return lResult;
		}
		#endregion
		public abstract bool SupportsFileExtension(string ext);
		public abstract string GetFullTypeName(string simpleName);
		#region GetSourceFile
		public virtual SourceFile GetSourceFile(string fileName)
		{
			return new SourceFile(fileName);
		}
		#endregion
		#region GotoParent(Token endToken, int adjustment)
		public virtual void GotoParent(Token endToken, int adjustment)
		{
			GotoParent(endToken, adjustment, true);
		}
		#endregion
		#region GotoParent(Token endToken, int adjustment, bool overwrite)
		public virtual void GotoParent(Token endToken, int adjustment, bool overwrite)
		{
			if (overwrite || (Context.EndOffset == 1 && Context.EndLine == 1))
				_Context.SetEnd(endToken.Line, endToken.Column + adjustment);
			GotoParentContext();
		}
		#endregion
		#region GotoParent(Token endToken)
		public virtual void GotoParent(Token endToken)
		{
			GotoParent(endToken, 0);
		}
		#endregion
		#region GotoParent(LanguageElement element)
		public virtual void GotoParent(LanguageElement element)
		{
			_Context.SetEnd(element.InternalRange.Start);
			GotoParentContext();
		}
		#endregion
		#region GotoParent(int endLine, int endOffset)
		public virtual void GotoParent(int endLine, int endOffset)
		{
			_Context.SetEnd(endLine, endOffset);
			GotoParentContext();
		}
		#endregion
		#region GotoParentContext()
		public void GotoParentContext()
		{
			if (_Context == null)
			{
				Log.SendError("GotoParentContext failed. Current context is null.");
				return;
			}
			_Context = _Context.Parent;
		}
		#endregion
		#region GotoParentRegion()
		public void GotoParentRegion()
		{
			if (_RegionContext == null)
				return;
			LanguageElement lParent = _RegionContext.Parent;
			if (lParent == null)
				_RegionContext = null;
			else
			{
				if (lParent is SourceFile)
					return;
				_RegionContext = (RegionDirective)lParent;
			}
		}
		#endregion
		#region IntroduceComments
		public virtual void IntroduceComments(LanguageElement context)
		{
		}
		#endregion
		#region IntroduceComments
		public virtual void IntroduceComments(LanguageElement context, CommentCollection comments)
		{
			SourceTreeCommenter lCommenter = new SourceTreeCommenter();
			lCommenter.CommentNode(context, comments);
		}
		#endregion
		#region Parse(LanguageElement context, RegionDirective regionContext, TextStringCollection textStrings, CompilerDirective compilerDirective, ISourceReader reader)
		public virtual LanguageElement Parse(LanguageElement context, RegionDirective regionContext,
			TextStringCollection textStrings, CompilerDirective compilerDirective, ISourceReader reader)
		{
			return Parse(context, regionContext, textStrings, compilerDirective, reader, null );
		}
		#endregion
		#region Parse(LanguageElement context, RegionDirective regionContext, TextStringCollection textStrings, CompilerDirective compilerDirectiveContext, ISourceReader reader, IDocument document)
		public virtual LanguageElement Parse(LanguageElement context, RegionDirective regionContext,
			TextStringCollection textStrings, CompilerDirective compilerDirectiveContext, ISourceReader reader,
			IDocument document)
		{
			ParserContext lParserContext = new ParserContext();
			lParserContext.Context = context;
			lParserContext.RegionContext = regionContext;
			lParserContext.TextStrings = textStrings;
			lParserContext.CompilerDirectiveContext = compilerDirectiveContext;
			lParserContext.Document = document;
			return Parse(lParserContext, reader);
		}
		#endregion
		#region Parse(ParserContext parserContext, ISourceReader reader)
		public virtual LanguageElement Parse(ParserContext parserContext, ISourceReader reader)
		{
			if (parserContext == null)
				throw new ArgumentNullException("parserContext");
			if (reader == null)
				throw new ArgumentNullException("reader");
#if DEBUG
			string threadName = System.Threading.Thread.CurrentThread.Name;
			if (threadName != null && threadName.Contains("CodeIssue"))
				Log.SendErrorWithStackTrace("Parsing is done from CodeIssue checking thread!");
#endif
			lock (_SyncObject)
			{
				try
				{
					return CallParsing(parserContext, reader);
				}
				finally
				{
				}
			}
		}
		#endregion
		public virtual LanguageElement ParseMiskFile(ParserContext parserContext, ISourceReader reader)
		{
			return Parse(parserContext, reader);
		}
		protected virtual LanguageElement CallParsing(ParserContext parserContext, ISourceReader reader)
		{
			return DoParse(parserContext, reader);
		}
		protected void SetupSourceFile()
		{
			if (Context != null && Context is SourceFile)
			{
				SourceFile lSourceFile = (SourceFile)Context;
				lSourceFile.SetDocument(Document);
				CompilerDirective lCompilerDirectiveRoot = lSourceFile.CompilerDirectiveRootNode;
				RegionDirective lRegionDirectiveRoot = lSourceFile.RegionRootNode;
				if (lCompilerDirectiveRoot != null)
					lCompilerDirectiveRoot.SetParent(lSourceFile);
				if (lRegionDirectiveRoot != null)
					lRegionDirectiveRoot.SetParent(lSourceFile);
			}
		}
		[Obsolete("Use ParseString instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement ParseSring(string code)
		{
			return ParseString(code);
		}
		public LanguageElement ParseString(string code)
		{
			return ParseString(code, 1, 1);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ICollection ParseStatements(string code) {
			if(String.IsNullOrEmpty(code))
				return null;
			ISourceReader reader = new SourceStringReader(code);
			ParserContext context = new ParserContext();
			context.Context = new Method();
			context.SourceReader = reader;
			context.AllowCommentsInParseTree = true;
			context.AutoSetValues();
			LanguageElement parsedElement = Parse(context, reader);
			if(parsedElement == null)
				return null;
			ICollection nodes = parsedElement.Nodes;
			if(nodes.Count == 0)
				nodes = context.Comments;
			return nodes;
		}
		public LanguageElement ParseString(string code, int line, int column)
		{
			if (code == null || code.Length == 0)
				return null;
			ISourceReader reader = new SourceStringReader(code, line, column);
			SourceFile node = GetSourceFile(string.Empty);
			ParserContext context = new ParserContext();
			context.Context = node;
			context.SourceFile = node;
			context.SourceReader = reader;
			context.AllowCommentsInParseTree = true;
			context.AutoSetValues();
			return Parse(context, reader);
		}
		public LanguageElement ParseFile(string path)
		{
			return ParseFile(path, DXEncoding.Default);
		}
		public LanguageElement ParseFile(string path, Encoding encoding)
	{
	  ErrorList errors;
	  return ParseFile(path, encoding, out errors);
	}
	public LanguageElement ParseFile(string path, Encoding encoding, out ErrorList errors)
		{
			errors = null;
	  if (!File.Exists(path))
				return null;
			ISourceReader lReader = new SourceStringReader(path, encoding);
			SourceFile lFileNode = GetSourceFile(path);
			ParserContext lContext = new ParserContext();
			lContext.Context = lFileNode;
			lContext.SourceFile = lFileNode;
			lContext.SourceReader = lReader;
			lContext.AllowCommentsInParseTree = true;
			lContext.AutoSetValues();
			LanguageElement result = Parse(lContext, lReader);
	  errors = lContext.Errors;
	  return result;
		}
		public LanguageElement ParseFile(SourceFile file)
		{
			if (file == null)
				return null;
			string path = file.Name;
			if (!File.Exists(path))
				return null;
			ISourceReader lReader = new SourceStringReader(path, DXEncoding.Default);
			SourceFile lFileNode = file;
			ParserContext lContext = new ParserContext();
			lContext.Context = lFileNode;
			lContext.SourceFile = lFileNode;
			lContext.SourceReader = lReader;
			lContext.AllowCommentsInParseTree = true;
			lContext.AutoSetValues();
			return Parse(lContext, lReader);
		}
		#region SetContext(LanguageElement value)
		public void SetContext(LanguageElement value)
		{
			if (_Context == value)
				return;
			_Context = value;
		}
		#endregion
		public virtual bool IsQueryExpressionStart(string str)
		{
			return false;
		}
		#region Language
		public abstract string Language
		{
			get;
		}
		#endregion
		#region IsParsing
		public bool IsParsing
		{
			get
			{
				return _IsParsing;
			}
		}
		#endregion
		#region LastParsingHasEOF
		public bool LastParsingHasEOF
		{
			get { return _LastParsingHasEOF; }
	  set { _LastParsingHasEOF = value; }
		}
		#endregion
		#region ParseTime
		public double ParseTime
		{
			get
			{
				return _ParseTime;
			}
		}
		#endregion
		#region ExpressionParser
		public ExpressionParserBase ExpressionParser
		{
			get
			{
				if (_ExpressionParser == null)
					_ExpressionParser = CreateExpressionParser();
				return _ExpressionParser;
			}
		}
		#endregion
		#region Context
		public LanguageElement Context
		{
			get
			{
				return _Context;
			}
		}
		#endregion
		#region RootContext
		public LanguageElement RootContext
		{
			get
			{
				return _RootContext;
			}
		}
		#endregion
		#region TextStrings
		public TextStringCollection TextStrings
		{
			get
			{
				return _TextStrings;
			}
		}
		#endregion
		#region RegionContext
		public RegionDirective RegionContext
		{
			get
			{
				return _RegionContext;
			}
			set
			{
				if (_RegionContext == value)
					return;
				_RegionContext = value;
			}
		}
		#endregion
		#region CompilerDirectiveContext
		public CompilerDirective CompilerDirectiveContext
		{
			get
			{
				return _CompilerDirectiveContext;
			}
			set
			{
				if (_CompilerDirectiveContext == value)
					return;
				_CompilerDirectiveContext = value;
			}
		}
		#endregion
	public ParserErrorsBase ParseErrors
	{
	  get
	  {
		return parserErrors;
	  }
	}
		#region Document
		public IDocument Document
		{
			get
			{
				return _Document;
			}
		}
		#endregion
		#region ParseRange
		public SourceRange ParseRange
		{
			get
			{
				return _ParseRange;
			}
		}
		#endregion
		#region AllowCommentsInParseTree
		public bool AllowCommentsInParseTree
		{
			get
			{
				return _AllowCommentsInParseTree;
			}
			set
			{
				if (_AllowCommentsInParseTree == value)
					return;
				_AllowCommentsInParseTree = value;
			}
		}
		#endregion
	}
}
