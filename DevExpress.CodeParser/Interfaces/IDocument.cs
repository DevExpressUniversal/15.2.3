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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public interface IDocument : IDisposableEditPointFactory
	{
	void Activate();
		int LengthOfLine(int lineNumber);
		void SelectText(int startLine, int startOffset, int endLine, int endOffset);
		void DeleteText(SourceRange range);
	void DoParse();
		void FullParse();
	LanguageElement Parse(ParserContext context);
		SourceRange InsertText(SourcePoint point, string text);
	string GetText(SourceRange range);
		bool GetSelectionBounds(out int startLine, out int startOffset, out int endLine, out int endOffset);
		void ReplaceSelection(string Text, bool KeepSelection);
		void AddHeaderFooter(LanguageElement aLanguageElement, string[] aHeader, string[] aFooter, bool aSelect);
	ICollapsibleRegion GetCollapsibleRegion(RegionDirective region);
	ICollapsibleRegion GetCollapsibleRegion(SourceRange range);
	ICollapsibleRegion GetCollapsibleRegion(RegionDirective region, IDXCoreTextView textView);
	ICollapsibleRegion GetCollapsibleRegion(SourceRange range, IDXCoreTextView textView);
		void AddNamespace(Namespace aNamespace);
		void ClearNamespaces();
		SourceRange IncludeWhitespace(SourceRange sourceRange);
		SourceRange IncludeLeadingWhiteSpace(SourceRange sourceRange);
		SourceRange IncludeTrailingWhiteSpace(SourceRange sourceRange);
		QueuedDelete QueueDelete(SourceRange range);
		QueuedDelete QueueDelete(LanguageElement element);
		QueuedDelete QueueDelete(LanguageElement firstSibling, LanguageElement lastSibling);
		QueuedInsert QueueInsert(SourcePoint insertionPoint, string newCode);
		QueuedReplace QueueReplace(SourceRange range, string newCode);
		QueuedReplace QueueReplace(LanguageElement element, string newCode);
		QueuedReplace QueueReplace(LanguageElement firstSibling, LanguageElement lastSibling, string newCode);
		void QueueMove(SourceRange range, SourcePoint insertionPoint);	
		void ApplyQueuedEdits();
		void ApplyQueuedEdits(string operation);
		void Move(SourceRange range, SourcePoint insertionPoint, string operation);
	void MoveWithBinding(SourceRange range, SourcePoint insertionPoint, string operation);
		void Replace(SourceRange range, string newCode, string operation);
	void Replace(SourceRange range, string newCode, string operation, bool format);
	SourceRange Format(SourceRange range);
		string GetText(int startLine, int startOffset, int endLine, int endOffset);
	string GetText(int lineNumber);
		SourceRange SetText(SourceRange range, string text);
		SourceRange IncludePrecedingEmptyLines(SourceRange sourceRange);
		SourceRange IncludeTrailingEmptyLines(SourceRange sourceRange);
		SourcePoint GetStartEmptyLinePoint(SourcePoint sourcePoint);
		SourcePoint GetEndEmptyLinePoint(SourcePoint sourcePoint);		
		IProjectElement GetProjectElement();
		ISourceReader GetSourceReader(SourceRange range);
	bool HasChangesInMemory();
	bool BufferAndFileSizesAreSame();
		string[] GetChildFilePaths();
		SourceRange ExpandText(int line, int offset, string text);
		SourceRange ExpandText(SourcePoint point, string text);
		IDXCoreTextView ActiveView {get;}
		IDXCoreTextView InactiveView {get;}
		string RegionStartKeyword {get;}
		string RegionEndKeyword {get;}
	string FullName {get;}
		ParserBase Parser {get;}
		string Language {get;}		
	int LineCount { get; }
		string Text { get; }
	void GetSurroundingText(SourcePoint point, out string leftText, out string rightText);
	string Name { get; }
	SourceRange Range { get; }
		bool IsDisposed { get; }
		bool IsDisposing { get; }
		IProjectElement ProjectElement { get; }
	[EditorBrowsable(EditorBrowsableState.Never)]
	DocumentHistory History { get; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		LanguageElement FileNode { get; }
	void OnBeforeParse(SourceRange range, LanguageElement parent, LanguageElement nodeBefore, LanguageElement nodeAfter, LanguageElementCollection  invalidatedNodes);
	void OnAfterParse(SourceRange range, LanguageElement parent, LanguageElement nodeBefore, LanguageElement nodeAfter, LanguageElementCollection invalidatedNodes, LanguageElement result, bool allChangesParsed);
	bool InCollapsedRange(LanguageElement languageElement, IDXCoreTextView textView);
	[EditorBrowsable(EditorBrowsableState.Never)]
	void MacrosRedefined();
  }
}
