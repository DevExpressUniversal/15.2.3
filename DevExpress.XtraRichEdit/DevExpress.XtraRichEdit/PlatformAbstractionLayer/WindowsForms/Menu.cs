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
using System.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.Menu {
	#region RichEditMenuItem
	public class RichEditMenuItem : CommandMenuItem<RichEditCommandId> {
		public RichEditMenuItem() : this(String.Empty) { }
		public RichEditMenuItem(string caption) : this(caption, null) { }
		public RichEditMenuItem(string caption, EventHandler click) : this(caption, click, null) { }
		public RichEditMenuItem(string caption, EventHandler click, Image image) : this(caption, click, image, null) { }
		public RichEditMenuItem(string caption, EventHandler click, Image image, EventHandler update)
			: base(caption, click, image, update) {
			this.Id = RichEditCommandId.None;
		}
	}
	#endregion
	#region RichEditMenuCheckItem
	public class RichEditMenuCheckItem : CommandMenuCheckItem<RichEditCommandId> {
		public RichEditMenuCheckItem() : this(String.Empty) { }
		public RichEditMenuCheckItem(string caption) : this(caption, false) { }
		public RichEditMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged) : this(caption, check, image, checkedChanged, null) { }
		public RichEditMenuCheckItem(string caption, bool check) : this(caption, check, null) { }
		public RichEditMenuCheckItem(string caption, bool check, Image image, EventHandler checkedChanged, EventHandler update)
			: base(caption, check, image, checkedChanged, update) {
			this.Id = RichEditCommandId.None;
		}
		public RichEditMenuCheckItem(string caption, bool check, EventHandler update)
			: base(caption, check, update) {
			this.Id = RichEditCommandId.None;
		}
	}
	#endregion
	#region RichEditPopupMenu
	public class RichEditPopupMenu : CommandPopupMenu<RichEditCommandId> {
		public RichEditPopupMenu(EventHandler beforePopup)
			: base(beforePopup) {
			this.Id = RichEditCommandId.None;
		}
		public RichEditPopupMenu()
			: base() {
			this.Id = RichEditCommandId.None;
		}
	}
	#endregion
	#region RichEditMenuItemCommandWinAdapter
	public class RichEditMenuItemCommandWinAdapter : DXMenuItemCommandAdapter<RichEditCommandId> {
		public RichEditMenuItemCommandWinAdapter(RichEditCommand command)
			: base(command) {
		}
		public override IDXMenuItem<RichEditCommandId> CreateMenuItem(DXMenuItemPriority priority) {
			RichEditMenuItem item = new RichEditMenuItem(Command.MenuCaption, new EventHandler(this.OnClick), Command.Image, new EventHandler(this.OnUpdate));
			RichEditCommand command = (RichEditCommand)Command;
			item.Id = command.Id;
			item.Priority = priority;
			return item;
		}
		public override void OnClick(object sender, EventArgs e) {
			try {
				base.OnClick(sender, e);
			}
			catch (Exception exception) {
				RichEditCommand richEditCommand = (RichEditCommand)Command;
				RichEditControl control = richEditCommand.Control as RichEditControl;
				if (control == null || !control.HandleException(exception))
					throw;
			}
		}
	}
	#endregion
	#region RichEditMenuCheckItemCommandWinAdapter
	public class RichEditMenuCheckItemCommandWinAdapter : DXMenuCheckItemCommandAdapter<RichEditCommandId> {
		public RichEditMenuCheckItemCommandWinAdapter(RichEditCommand command)
			: base(command) {
		}
		public override void OnCheckedChanged(object sender, EventArgs e) {
			try {
				base.OnCheckedChanged(sender, e);
			}
			catch (Exception exception) {
				RichEditCommand richEditCommand = (RichEditCommand)Command;
				RichEditControl control = richEditCommand.Control as RichEditControl;
				if (control == null || !control.HandleException(exception))
					throw;
			}
		}
		public override IDXMenuCheckItem<RichEditCommandId> CreateMenuItem(string groupId) {
			RichEditMenuCheckItem item = new RichEditMenuCheckItem(Command.MenuCaption, false, Command.Image, new EventHandler(this.OnCheckedChanged), new EventHandler(this.OnUpdate));
			RichEditCommand command = (RichEditCommand)Command;
			item.Id = command.Id;
			return item;
		}
	}
	#endregion
}
