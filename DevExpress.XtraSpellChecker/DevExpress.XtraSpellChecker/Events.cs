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
using System.Windows.Forms;
using DevExpress.XtraSpellChecker.Parser;
using DevExpress.Utils.Menu;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraSpellChecker {
	public delegate void SelectionStartChangedEventHandler(object sender, SelectionStartChangedEventArgs e);
	public enum SelectionChangeReason { Keyboard, Mouse}
	public class SelectionStartChangedEventArgs : EventArgs {
		Position selectionStart;
		SelectionChangeReason reason;
		Keys key;
		public SelectionStartChangedEventArgs(Position selectionStart, SelectionChangeReason reason, Keys key) {
			this.selectionStart = selectionStart;
			this.reason = reason;
			this.key = key;
		}
		public Position SelectionStart { get { return this.selectionStart; } }
		public SelectionChangeReason Reason { get { return this.reason; } }
		public Keys Key { get { return this.key; } }
	}
	public enum PaintReason { CharPressed, LayoutChanged, Unknown};
	public delegate void PaintEventHandler(object sender, PaintEventArgs e);
	public class PaintEventArgs : EventArgs { 
		PaintReason paintReason = PaintReason.Unknown;
		public PaintEventArgs(PaintReason paintReason) {
			this.paintReason = paintReason;
		}
		public PaintReason PaintReason {get {return this.paintReason;}}
	}
	public delegate void NeedPaintEventHandler(object sender, NeedPaintEventArgs e);
	public class NeedPaintEventArgs : EventArgs {
		bool layoutChanged = false;
		bool textChanged = false;
		public NeedPaintEventArgs(bool textChanged, bool layoutChanged) {
			this.layoutChanged = layoutChanged;
			this.textChanged = textChanged;
		}
		public bool LayoutChanged { get { return layoutChanged; } }
		public bool TextChanged { get { return textChanged; } }
	}
	public delegate void ScrollingEventHandler(object sender, ScrollingEventArgs e);
	public enum ScrollReason { Keyboard, Mouse, Wheel}
	public enum ScrollDirection { Up, Down, Undefined}
	public class ScrollingEventArgs : EventArgs {
		ScrollReason scrollReason = ScrollReason.Mouse;
		ScrollDirection scrollDirection = ScrollDirection.Undefined;
		public ScrollingEventArgs(ScrollReason scrollReason, ScrollDirection direction) {
			this.scrollReason = scrollReason;
			this.scrollDirection = direction;
		}
		public ScrollReason ScrollReason { get { return scrollReason; } }
		public ScrollDirection ScrollDirection { get { return scrollDirection; } }
	}
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	public class PopupMenuShowingEventArgs : EventArgs {
		readonly Point location;
		readonly DXPopupMenu menu;
		public PopupMenuShowingEventArgs(DXPopupMenu menu, Point location) {
			Guard.ArgumentNotNull(menu, "menu");
			this.menu = menu;
			this.location = location;
		}
		public Point Location { get { return location; } }
		public DXPopupMenu Menu { get { return menu; } }
	}
	[Obsolete("You should use the 'PopupMenuShowingEventHandler' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void PrepareContextMenuEventHandler(object sender, PrepareContextMenuEventArgs e);
	[Obsolete("You should use the 'PopupMenuShowingEventArgs' instead", true), EditorBrowsable(EditorBrowsableState.Never)]
	public class PrepareContextMenuEventArgs : PopupMenuShowingEventArgs {
		public PrepareContextMenuEventArgs(DXPopupMenu menu, Point location)
			: base(menu, location) {
		}
	}
}
