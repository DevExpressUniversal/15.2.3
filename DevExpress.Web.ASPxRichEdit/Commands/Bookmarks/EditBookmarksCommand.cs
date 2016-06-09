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
using System.Linq;
using System.Text;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class CreateBookmarkCommand : WebRichEditStateBasedCommand<CreateBookmarkCommandState> {
		public override CommandType Type { get { return CommandType.CreateBookmark; } }
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.BookmarksAllowed;
		}
		protected override bool IsEnabled() { return true; }
		public CreateBookmarkCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModelCore(CreateBookmarkCommandState stateObject) {
			DocumentLogPosition startLogPosition = new DocumentLogPosition(stateObject.start);
			PieceTable.CreateBookmarkCore(startLogPosition, stateObject.end - stateObject.start, stateObject.name);
		}
	}
	public class DeleteBookmarkCommand : WebRichEditStateBasedCommand<DeleteBookmarkCommandState> {
		public override CommandType Type { get { return CommandType.DeleteBookmark; } }
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.BookmarksAllowed;
		}
		protected override bool IsEnabled() { return true; }
		public DeleteBookmarkCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModelCore(DeleteBookmarkCommandState stateObject) {
			var index = GetIndex(stateObject.name);
			if(index > -1)
				this.PieceTable.DeleteBookmarkCore(index);
		}
		private int GetIndex(string name) {
			var bookmarks = PieceTable.GetBookmarks(true);
			for(int i = 0; i < bookmarks.Count; i++)
				if(bookmarks[i].Name == name)
					return i;
			return -1;
		}
	}
}
