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
using DevExpress.Utils;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentUpdateCompleteEventHandler
	public delegate void DocumentUpdateCompleteEventHandler(object sender, DocumentUpdateCompleteEventArgs e);
	#endregion
	#region DocumentUpdateCompleteEventArgs
	public class DocumentUpdateCompleteEventArgs : EventArgs {
		readonly DocumentModelDeferredChanges deferredChanges;
		public DocumentUpdateCompleteEventArgs(DocumentModelDeferredChanges deferredChanges) {
			Guard.ArgumentNotNull(deferredChanges, "deferredChanges");
			this.deferredChanges = deferredChanges;
		}
		public DocumentModelDeferredChanges DeferredChanges { get { return deferredChanges; } }
	}
	#endregion
	#region ParagraphEventHandler
	public delegate void ParagraphEventHandler(object sender, ParagraphEventArgs e);
	#endregion
	#region ParagraphEventArgs
	public class ParagraphEventArgs : EventArgs {
		#region Fields
		readonly PieceTable pieceTable;
		readonly SectionIndex sectionIndex;
		readonly ParagraphIndex paragraphIndex;
		#endregion
		public ParagraphEventArgs(PieceTable pieceTable, SectionIndex sectionIndex, ParagraphIndex paragraphIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.sectionIndex = sectionIndex;
			this.paragraphIndex = paragraphIndex;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public SectionIndex SectionIndex { get { return sectionIndex; } }
		#endregion
	}
	#endregion
	#region FieldEventHandler
	public delegate void FieldEventHandler(object sender, FieldEventArgs e);
	#endregion
	#region FieldEventArgs
	public class FieldEventArgs : EventArgs {
		readonly PieceTable pieceTable;
		readonly int fieldIndex;
		public FieldEventArgs(PieceTable pieceTable, int fieldIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.fieldIndex = fieldIndex;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public int FieldIndex { get { return fieldIndex; } }
	}
	#endregion
	#region ObtainAffectedRangeEventHandler
	public delegate void ObtainAffectedRangeEventHandler(object sender, ObtainAffectedRangeEventArgs e);
	#endregion
	#region ObtainAffectedRangeEventArgs
	public class ObtainAffectedRangeEventArgs : EventArgs {
		RunIndex start = new RunIndex(-1);
		RunIndex end = new RunIndex(-1);
		public RunIndex Start { get { return start; } set { start = value; } }
		public RunIndex End { get { return end; } set { end = value; } }
	}
	#endregion
	#region SearchCompleteEventHandler
	public delegate void SearchCompleteEventHandler(object sender, SearchCompleteEventArgs e);
	#endregion
	#region SearchCompleteEventArgs
	public class SearchCompleteEventArgs : CancelEventArgs {
		#region Fields
		readonly SearchAction action;
		readonly Direction direction;
		readonly SearchScope searchScope;
		int matchCount;
		int replaceCount;
		string searchString = String.Empty;
		string replaceString = String.Empty;
		bool entireDocument;
		bool @continue;
		#endregion
		public SearchCompleteEventArgs(SearchAction action, Direction direction, SearchScope searchScope) {
			this.action = action;
			this.direction = direction;
			this.searchScope = searchScope;
		}
		#region Properties
		public SearchAction Action { get { return action; } }
		public Direction Direction { get { return direction; } }
		public SearchScope SearchScope { get { return searchScope; } }
		public int MatchCount { get { return matchCount; } }
		public int ReplaceCount { get { return replaceCount; } }
		public string SearchString { get { return searchString; } }
		public string ReplaceString { get { return replaceString; } }
		public bool EntireDocument { get { return entireDocument; } }
		internal bool Continue { get { return @continue; } set { @continue = value; } }
		#endregion
		internal void SetMatchCount(int value) {
			this.matchCount = value;
		}
		internal void SetReplaceCount(int value) {
			this.replaceCount = value;
		}
		internal void SetSearchString(string value) {
			this.searchString = value;
		}
		internal void SetReplaceString(string value) {
			this.replaceString = value;
		}
		internal void SetEntireDocument(bool value) {
			this.entireDocument = value;
		}
	}
	#endregion
	#region SectionEventHandler
	public delegate void SectionEventHandler(object sender, SectionEventArgs e);
	#endregion
	#region SectionEventArgs
	public class SectionEventArgs : EventArgs {
		readonly SectionIndex sectionIndex;
		public SectionEventArgs(SectionIndex sectionIndex) {
			this.sectionIndex = sectionIndex;
		}
		public SectionIndex SectionIndex { get { return sectionIndex; } }
	}
	#endregion
	#region HyperlinkInfoEventHandler
	public delegate void HyperlinkInfoEventHandler(object sender, HyperlinkInfoEventArgs e);
	#endregion
	#region HyperlinkInfoEventArgs
	public class HyperlinkInfoEventArgs : EventArgs {
		readonly PieceTable pieceTable;
		readonly int fieldIndex;
		public HyperlinkInfoEventArgs(PieceTable pieceTable, int fieldIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.fieldIndex = fieldIndex;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public int FieldIndex { get { return fieldIndex; } }
	}
	#endregion
	#region CustomMarkEventHandler
	public delegate void CustomMarkEventHandler(object sender, CustomMarkEventArgs e);
	#endregion
	#region CustomMarkEventArgs
	public class CustomMarkEventArgs : EventArgs {
		readonly PieceTable pieceTable;
		readonly int customMarkIndex;
		public CustomMarkEventArgs(PieceTable pieceTable, int customMarkIndex) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.customMarkIndex = customMarkIndex;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public int CustomMarkIndex { get { return customMarkIndex; } }
	}
	#endregion
	#region CommentEventHandler
	public delegate void CommentEventHandler(object sender, CommentEventArgs e);
	#endregion
	#region CommentEventArgs
	public class CommentEventArgs : EventArgs {
		readonly PieceTable pieceTable;
		readonly int commentIndex;
		readonly Comment comment;
		readonly bool onExecute;
		public CommentEventArgs(PieceTable pieceTable, Comment comment, int commentIndex, bool onExecute) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			this.comment = comment;
			this.commentIndex = commentIndex;
			this.onExecute = onExecute;
		}
		public PieceTable PieceTable { get { return pieceTable; } }
		public Comment Comment { get { return comment; } }
		public int CommentIndex { get { return commentIndex; } }
		public bool OnExecute { get { return onExecute; } }
	}
	#endregion
	#region DocumentContentChangedEventHandler
	public delegate void DocumentContentChangedEventHandler(object sender, DocumentContentChangedEventArgs e);
	#endregion
	#region DocumentContentChangedEventArgs
	public class DocumentContentChangedEventArgs : EventArgs {
		readonly bool suppressBindingNotifications;
		public DocumentContentChangedEventArgs(bool suppressBindingNotifications) {
			this.suppressBindingNotifications = suppressBindingNotifications;
		}
		public bool SuppressBindingNotifications { get { return suppressBindingNotifications; } }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit { 
	#region CommentEditingEventHandler
	public delegate void CommentEditingEventHandler(object sender, CommentEditingEventArgs e);
	#endregion
	#region CommentEditingEventArgs
	public class CommentEditingEventArgs : EventArgs {
		readonly DevExpress.XtraRichEdit.API.Native.Comment comment;
		readonly int commentIndex;
		internal CommentEditingEventArgs(DevExpress.XtraRichEdit.API.Native.Comment comment, int commentIndex) {
			this.comment = comment;
			this.commentIndex = commentIndex;
		}
		public DevExpress.XtraRichEdit.API.Native.Comment Comment { get { return comment; } }
		public int CommentIndex { get { return commentIndex; } }
	}
	#endregion
}
