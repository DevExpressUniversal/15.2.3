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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Layout {
	#region PageFormattingCompleteEventHandler
	public delegate void PageFormattingCompleteEventHandler(object sender, PageFormattingCompleteEventArgs e);
	#endregion
	#region PageFormattingCompleteEventArgs
	public class PageFormattingCompleteEventArgs : EventArgs {
		readonly Page page;
		readonly bool documentFormattingComplete;
		public PageFormattingCompleteEventArgs(Page page, bool documentFormattingComplete) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
			this.documentFormattingComplete = documentFormattingComplete;
		}
		public Page Page { get { return page; } }
		public bool DocumentFormattingComplete { get { return documentFormattingComplete; } }
	}
	#endregion
	#region ResetFormattingEventHandler
	public delegate void ResetFormattingEventHandler(object sender, ResetFormattingEventArgs e);
	#endregion
	#region ResetFormattingEventArgs
	public class ResetFormattingEventArgs : EventArgs {
		readonly DocumentModelPosition pos;
		public ResetFormattingEventArgs(DocumentModelPosition pos) {
			this.pos = pos;
		}
		public DocumentModelPosition Position { get { return pos; } }
	}
	#endregion
}
