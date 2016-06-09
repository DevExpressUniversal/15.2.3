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
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public interface IRazorCodeParser
  {
	LanguageElementCollection ParseRazorFunctions(ISourceReader reader, out int scannerPositionDelta);
	LanguageElementCollection ParseRazorCode(ISourceReader reader, ref int scannerPositionDelta);
	LanguageElement ParseRazorHelper(ISourceReader reader, out int scannerPositionDelta);
  }
	public abstract class GeneratedParserBase : ParserBase
	{
		const int minErrDist = 2;
		protected const bool T = true;
		protected const bool x = false;
		protected int errDist = minErrDist;
		protected Token tToken;	
		protected Token la;   
		protected GeneratedScannerBase scanner;
		protected bool[,] set;
		protected int maxTokens;
		TextStringCollection _TextStrings = null;
		LanguageElement _RootNode  = null;
		protected abstract bool[,] CreateSetArray();
	protected virtual void Get()
	{
	  while (true)
	  {
		tToken = la;
		la = scanner.Scan();
		if (la.Type <= maxTokens)
		{
		  ++errDist;
		  break;
		}
		HandlePragmas();
		la = tToken;
	  }
	}
		protected void AddNamespaceToDeclaredList(Namespace declaredNamespace)
		{
			if (declaredNamespace == null || declaredNamespace.FileNode == null)
				return;
			StringCollection declaredNamespaces = declaredNamespace.FileNode.DeclaredNamespaces;
			string name = declaredNamespace.Name;
			if (String.IsNullOrEmpty(name) || declaredNamespaces.Contains(name))
				return;
			declaredNamespaces.Add(name);
		}
		protected virtual void CheckForErrors(LanguageElement rootNode)
		{
			if (rootNode == null) 
				return;
			SourceFile sourceFile = null;
			if (rootNode is SourceFile)
				sourceFile = rootNode as SourceFile;
			else
				sourceFile = rootNode.FileNode;
			if (sourceFile == null)
				return;
			if (parserErrors == null || parserErrors.Count == 0)
				sourceFile.HasErrors = false;
			else
				sourceFile.HasErrors = true;
		}
		protected virtual void HandlePragmas()
		{
		}
		protected Token Peek()
		{
			return scanner.Peek();
		}
		protected Token Peek(int pos)
		{
			return scanner.Peek(pos);
		}
		protected void ResetPeek()
		{
			scanner.ResetPeek();
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
		#region ExpectWeak
		protected void ExpectWeak(int n, int follow)
		{
			if (la.Type == n)
				Get();
			else 
			{
				SynErr(n);
				while (!StartOf(follow))
					Get();
			}
		}
		#endregion
		#region WeakSeparator
		protected bool WeakSeparator(int n, int syFol, int repFol)
		{
			bool[] s = new bool[maxTokens + 1];
			if (la.Type == n)
			{ 
				Get();
				return true; 
			}
			else if (StartOf(repFol))
				return false;
			else 
			{
				for (int i = 0; i <= maxTokens; i++) 
					s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
				SynErr(n);
				while (!s[la.Type])
					Get();
				return StartOf(syFol);
			}
		}
		#endregion
		#region SynErr
		protected void SynErr(int n)
		{
			if (errDist >= minErrDist)
				parserErrors.SynErr(la.Line, la.Column, n);
			errDist = 0;
		}
		#endregion
		#region SemErr
		protected void SemErr(string msg)
		{
			if (errDist >= minErrDist)
				parserErrors.Error(tToken.Line, tToken.Column, msg);
			errDist = 0;
		}
		#endregion		
		#region StartOf
		protected bool StartOf(int s)
		{
			return set[s, la.Type];
		}
		#endregion
		protected static SourceRange GetRange(object a, object b)
		{
			return SourceRangeUtils.GetRange(a, b);
		}
		protected static SourceRange GetRange(object a)
		{
			return SourceRangeUtils.GetRange(a);
		}
		protected static SourceRange GetRange(params object[] objs) 
		{
			return SourceRangeUtils.GetRange(objs);
		}
		protected virtual void BindComments()
		{
			if (RootNode == null || !AllowCommentsInParseTree)
				return;
			SourceTreeCommenter lCommenter = new SourceTreeCommenter();
			CommentCollection lComments = Comments;
			if (lComments != null)
				lCommenter.CommentNode(RootNode, Comments);
		}
		protected virtual void SetRootNode(LanguageElement newContext)
		{
			SetContext(newContext);
			_RootNode = newContext;
		}
		#region OpenContextWithoutAddNode
		protected void OpenContextWithoutAddNode(LanguageElement newContext)
		{
			if (Context == null)
				return;
			if (Context != null && Context == newContext)
				return;
			newContext.SetParent(Context);
			SetContext(newContext);
		}
		#endregion
		protected virtual void OpenContext(LanguageElement newContext)
		{
			if (Context != null)
			{
				newContext.SetParent(Context);
				AddNode(newContext);
				SetContext(newContext);
			}
			else
			{
				_RootNode = newContext;
				SetContext(newContext);
			}
		}
		protected virtual void CloseContext()
		{
			if(Context != null)
				SetContext(Context.Parent);
		}
		protected void ReadBlockStart(SourceRange range)
		{
			if (Context is IHasBlock)
				(Context as IHasBlock).SetBlockStart(range);
			if (Context is DelimiterCapableBlock)
			{
				DelimiterCapableBlock lBlock = (DelimiterCapableBlock)Context;
				lBlock.SetBlockType(DelimiterBlockType.Brace);
			}
		}
		protected void ReadBlockEnd(SourceRange range)
		{
			if (Context is IHasBlock)
				(Context as IHasBlock).SetBlockEnd(range);
		}
		protected virtual void AddNode(LanguageElement node)
		{
			if(Context != null)
				Context.AddNode(node);
		}
		protected virtual void AddDetailNode(LanguageElement detailNode)
		{
			if(Context != null)
				Context.AddDetailNode(detailNode);
		}
		protected virtual void SetContextEnd(SourcePoint end)
		{
			if (Context != null)
				Context.SetEnd(end);
		}
		protected virtual void CleanUpParser()
		{
			base.FinishParsing();
			_RootNode = null;
			if (scanner != null)
			{
		if (!scanner.IsDisposed)
				  scanner.Dispose();
				scanner = null;
			}
			if (parserErrors != null)
				parserErrors.Clear();
			SetContext(null);
			_TextStrings = null;
		}
	protected override LanguageElement CallParsing(ParserContext parserContext, ISourceReader reader)
	{
	  LanguageElement result = null;
	  LanguageElementCollection trailingRegions = new LanguageElementCollection();
	  SourceFile contextSourceFile = parserContext != null ? parserContext.SourceFile : null;
	  try
	  {
		if (parserContext != null)
		  AllowCommentsInParseTree = parserContext.AllowCommentsInParseTree;
		if (contextSourceFile != null)
		  contextSourceFile.InvalidateSimples(parserContext.ParseRange, trailingRegions);
		result = DoParse(parserContext, reader);
	  }
	  finally
	  {
		AllowCommentsInParseTree = true;
		if (contextSourceFile != null)
		  contextSourceFile.RestoreTrailingRegions(trailingRegions);
	  }
	  if (result != null && result is SourceFile)
	  {
		SourceFile sourceFile = (SourceFile)result;
		CompilerDirective lCompilerDirectiveRoot = sourceFile.CompilerDirectiveRootNode;
		if (lCompilerDirectiveRoot != null)
		  lCompilerDirectiveRoot.SetParent(sourceFile);
		sourceFile.SetRegionDirectiveHolderParent(sourceFile);
	  }
	  return result;
	}
	#region ConcatTokens(Token left,Token right)
	protected Token ConcatTokens(Token left, Token right)
	{
	  Token newToken = left;
	  newToken.StartPosition = left.StartPosition;
	  newToken.Column = left.Column;
	  newToken.Line = left.Line;
	  newToken.EndPosition = right.EndPosition;
	  newToken.EndColumn = right.EndColumn;
	  newToken.EndLine = right.EndLine;
	  return newToken;
	}
	#endregion
	#region ConcatTokens(Token left,Token right,int kind, string val)
	protected Token ConcatTokens(Token left, Token right, int kind, string val)
	{
	  Token newToken = ConcatTokens(left, right);
	  newToken.Type = kind;
	  newToken.Value = val;
	  return newToken;
	}
	#endregion
	string GetAssemblyName(Attribute attributeDef)
	{
	  if (attributeDef == null || attributeDef.ArgumentCount < 1)
		return String.Empty;
	  PrimitiveExpression arg = attributeDef.Arguments[0] as PrimitiveExpression;
	  if (arg == null || arg.PrimitiveType != PrimitiveType.String)
		return String.Empty;
	  return arg.PrimitiveValue as string;
	}
	SourceFile GetFileNode()
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
	protected void ProcessAttribute(Attribute attribute)
	{
	  if (attribute == null || attribute.TargetType != AttributeTargetType.Assembly)
		return;
	  if (string.Compare(attribute.Name, "InternalsVisibleTo", StringComparison.CurrentCultureIgnoreCase) != 0)
		return;
	  SourceFile fileNode = GetFileNode();
	  if (fileNode == null)
		return;
	  string assemblyName = GetAssemblyName(attribute);
	  fileNode.AddFriendAssemblyName(assemblyName);
	}
		public override bool SupportsFileExtension(string ext)
		{
			return false;
		}
		public override ExpressionParserBase CreateExpressionParser()
		{
			return null;
		}
		public override IExpressionInverter CreateExpressionInverter()
		{
			return null;
		}
		public override string GetFullTypeName(string simpleName)
		{
			return simpleName;
		}		
		#region TextStrings
		public new TextStringCollection TextStrings
		{
			get
			{
				if(_TextStrings == null)
					_TextStrings = new TextStringCollection();
				return _TextStrings;
			}
		}
		#endregion		
		public LanguageElement RootNode
		{
			get
			{
				return _RootNode;
			}
		}
		public virtual CommentCollection Comments
		{
			get
			{
				return null;
			}
		}
  }
}
