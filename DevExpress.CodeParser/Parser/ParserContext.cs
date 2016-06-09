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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ParserContext
	{
		LanguageElement _Context;
	LanguageElement _ContextAfterParse;
		RegionDirective _RegionContext;
		TextStringCollection _TextStrings;
		CompilerDirective _CompilerDirectiveContext;
		ISourceReader _SourceReader;
		IDocument _Document;
		SourceRange _ParseRange;
		CommentCollection _Comments;
		SourceFile _SourceFile;
		bool _AllowCommentsInParseTree;
	bool _SaveUserFormat;
		ScannerExtension _ScannerExtension;
	ErrorList _Errors;
		public ParserContext()
		{
			ClearValues();
		}
		public void AutoSetValues()
		{
			if (Context == null)
				return;
			if (RegionContext != null && TextStrings != null && CompilerDirectiveContext != null && SourceFile != null)
				return;
			RegionDirective lRegionDirectives;
			TextStringCollection lStrings;
			CompilerDirective lCompilerDirectives;
			SourceFile lSourceFile;
			Context.GetSourceFileSupportLists(out lRegionDirectives, out lStrings, out lCompilerDirectives, out lSourceFile);
			if (RegionContext == null)
				RegionContext = lRegionDirectives;
			if (TextStrings == null)
				TextStrings = lStrings;
			if (CompilerDirectiveContext == null)
				CompilerDirectiveContext = lCompilerDirectives;
			if (SourceFile == null)
				SourceFile = lSourceFile;
		}
		public void IntroduceComments()
		{
			SourceTreeCommenter lCommenter = new SourceTreeCommenter();
			lCommenter.CommentNode(SourceFile, Comments);
		}
		protected void ClearValues()
		{
			_Context = null;
			_RegionContext = null;
			_TextStrings = null;
			_CompilerDirectiveContext = null;
			_SourceReader = null;
			_Document = null;
			_ParseRange = SourceRange.Empty;
			_AllowCommentsInParseTree = true;
	  _SaveUserFormat = false;
	  _Errors = null;
		}
		public LanguageElement Context
		{
			get
			{
				return _Context;
			}
			set
			{
				if (_Context == value)
					return;
				_Context = value;
			}
		}
	public LanguageElement ContextAfterParse
	{
	  get
	  {
		return _ContextAfterParse;
	  }
	  set
	  {
		if (_ContextAfterParse == value)
		  return;
		_ContextAfterParse = value;
	  }
	}
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
		public TextStringCollection TextStrings
		{
			get
			{
				return _TextStrings;
			}
			set
			{
				if (_TextStrings == value)
					return;
				_TextStrings = value;
			}
		}
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
		public ISourceReader SourceReader
		{
			get
			{
				return _SourceReader;
			}
			set
			{
				if (_SourceReader == value)
					return;
				_SourceReader = value;
			}
		}
		public IDocument Document
		{
			get
			{
				return _Document;
			}
			set
			{
				if (_Document == value)
					return;
				_Document = value;
			}
		}
		public SourceRange ParseRange
		{
			get
			{
				return _ParseRange;
			}
			set
			{
				if (_ParseRange == value)
					return;
				_ParseRange = value;
			}
		}
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
	public bool SaveUserFormat
	{
	  get
	  {
		return _SaveUserFormat;
	  }
	  set
	  {
		_SaveUserFormat = value;
	  }
	}
		public CommentCollection Comments
		{
			get
			{
				if (_Comments == null)
					_Comments = new CommentCollection();
				return _Comments;
			}
			set
			{
				if (_Comments == value)
					return;
				_Comments = value;
			}
		}
		public SourceFile SourceFile
		{
			get
			{
				return _SourceFile;
			}
			set
			{
				if (_SourceFile == value)
					return;
				_SourceFile = value;
			}
		}
		public ScannerExtension ScannerExtension
		{
			get
			{
				return _ScannerExtension;
			}
			set
			{
				_ScannerExtension = value;
			}
		}
	public ErrorList Errors
	{
	  get
	  {
		return _Errors;
	  }
	  set
	  {
		_Errors = value;
	  }
	}
	}
}
