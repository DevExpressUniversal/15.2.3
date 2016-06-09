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

using DevExpress.Utils.Menu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
namespace DevExpress.XtraToolbox {
	public delegate void ToolboxStateChangedEventHandler(object sender, ToolboxStateChangedEventArgs e);
	public class ToolboxStateChangedEventArgs : EventArgs {
		public ToolboxStateChangedEventArgs() { }
	}
	public delegate void ToolboxItemChangedEventHandler(object sender, ToolboxItemChangedEventArgs e);
	public class ToolboxItemChangedEventArgs : EventArgs {
		public ToolboxItemChangedEventArgs() { }
	}
	public delegate void ToolboxGroupChangedEventHandler(object sender, ToolboxGroupChangedEventArgs e);
	public class ToolboxGroupChangedEventArgs : EventArgs {
		public ToolboxGroupChangedEventArgs() { }
	}
	public delegate void ToolboxSelectedGroupChangedEventHandler(object sender, ToolboxSelectedGroupChangedEventArgs e);
	public class ToolboxSelectedGroupChangedEventArgs : ToolboxGroupEventArgsBase {
		public ToolboxSelectedGroupChangedEventArgs(IToolboxGroup group) : base(group) { }
	}
	public delegate void ToolboxSearchTextChangedEventHandler(object sender, ToolboxSearchTextChangedEventArgs e);
	public class ToolboxSearchTextChangedEventArgs : EventArgs {
		string text;
		public ToolboxSearchTextChangedEventArgs(string text) {
			this.text = text;
		}
		public string Text {
			get { return text; }
		}
		public bool Handled {
			get { return Result != null; }
		}
		IEnumerable<ToolboxItem> result;
		public IEnumerable<ToolboxItem> Result {
			get { return result; }
			set { result = value; }
		}
	}
	public delegate void ToolboxInitializeMenuEventHandler(object sender, ToolboxInitializeMenuEventArgs e);
	public class ToolboxInitializeMenuEventArgs : EventArgs {
		DXPopupMenu menu;
		bool isMinimized;
		public ToolboxInitializeMenuEventArgs(DXPopupMenu menu, bool isMinimized) : base() {
			this.menu = menu;
			this.isMinimized = isMinimized;
		}
		public DXPopupMenu Menu {
			get { return menu; }
		}
		public bool IsMinimized {
			get { return isMinimized; }
		}
	}
	public delegate void ToolboxItemClickEventHandler(object sender, ToolboxItemClickEventArgs e);
	public class ToolboxItemClickEventArgs : ToolboxItemEventArgsBase {
		public ToolboxItemClickEventArgs(IToolboxItem item) : base(item) { }
	}
	public delegate void ToolboxItemDoubleClickEventHandler(object sender, ToolboxItemDoubleClickEventArgs e);
	public class ToolboxItemDoubleClickEventArgs : ToolboxItemEventArgsBase {
		public ToolboxItemDoubleClickEventArgs(IToolboxItem item) : base(item) { }
	}
	public delegate void ToolboxDragItemDropEventHandler(object sender, ToolboxDragItemDropEventArgs e);
	public class ToolboxDragItemDropEventArgs : ToolboxItemCollectionEventArgsBase {
		public ToolboxDragItemDropEventArgs(IEnumerable<IToolboxItem> items) : base(items) { }
	}
	public delegate void ToolboxDragItemMoveEventHandler(object sender, ToolboxDragItemMoveEventArgs e);
	public class ToolboxDragItemMoveEventArgs : ToolboxItemCollectionEventArgsBase {
		public ToolboxDragItemMoveEventArgs(IEnumerable<IToolboxItem> items, Point point) : base(items) {
			location = point;
		}
		Point location;
		public Point Location {
			get { return location; }
		}
		public DragDropEffects DragDropEffects { get; set; }
	}
	public delegate void ToolboxDragItemStartEventHandler(object sender, ToolboxDragItemStartEventArgs e);
	public class ToolboxDragItemStartEventArgs : ToolboxItemCollectionEventArgsBase {
		Image image;
		bool handled;
		bool cancel;
		public ToolboxDragItemStartEventArgs(IEnumerable<IToolboxItem> items) : base(items) {
			this.image = null;
			this.handled = false;
			this.cancel = false;
		}
		public Image Image {
			get { return image; }
			set { image = value; }
		}
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public delegate void ToolboxGetItemImageEventHandler(object sender, ToolboxGetItemImageEventArgs e);
	public class ToolboxGetItemImageEventArgs : ToolboxItemEventArgsBase {
		Image image;
		Size bestImageSize;
		public ToolboxGetItemImageEventArgs(IToolboxItem item, Size bestImageSize) : base(item) {
			this.image = null;
			this.bestImageSize = bestImageSize;
		}
		public Size BestImageSize { get { return bestImageSize; } }
		public Image Image { get { return image; } set { image = value; } }
	}
	public abstract class ToolboxItemCollectionEventArgsBase : EventArgs {
		IEnumerable<IToolboxItem> items;
		public ToolboxItemCollectionEventArgsBase(IEnumerable<IToolboxItem> items) {
			this.items = items;
		}
		public IToolboxItem Item { get { return Items.FirstOrDefault(); } }
		public IEnumerable<IToolboxItem> Items { get { return items; } }
	}
	public abstract class ToolboxItemEventArgsBase : EventArgs {
		IToolboxItem item;
		public ToolboxItemEventArgsBase(IToolboxItem item) {
			this.item = item;
		}
		public IToolboxItem Item { get { return item; } }
	}
	public abstract class ToolboxGroupEventArgsBase : EventArgs {
		IToolboxGroup group;
		public ToolboxGroupEventArgsBase(IToolboxGroup group) {
			this.group = group;
		}
		public IToolboxGroup Group { get { return group; } }
	}
}
