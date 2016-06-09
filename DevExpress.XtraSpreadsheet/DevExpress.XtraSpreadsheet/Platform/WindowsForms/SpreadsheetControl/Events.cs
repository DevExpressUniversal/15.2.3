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
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Menu;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet {
	public partial class SpreadsheetControl {
		#region PopupMenuShowing
		internal static readonly object onPopupMenuShowing = new object();
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlPopupMenuShowing")]
#endif
		public event PopupMenuShowingEventHandler PopupMenuShowing {
			add { Events.AddHandler(onPopupMenuShowing, value); }
			remove { Events.RemoveHandler(onPopupMenuShowing, value); }
		}
		protected internal virtual SpreadsheetPopupMenu RaisePopupMenuShowing(SpreadsheetPopupMenu menu, SpreadsheetMenuType menuType) {
			PopupMenuShowingEventHandler handler = (PopupMenuShowingEventHandler)this.Events[onPopupMenuShowing];
			if (handler != null) {
				PopupMenuShowingEventArgs args = new PopupMenuShowingEventArgs(menu, menuType);
				handler(this, args);
				return args.Menu;
			}
			else
				return menu;
		}
		#endregion
		#region CustomDrawCell
		static readonly object onCustomDrawCell = new object();
		internal bool HasCustomDrawCell { get { return (CustomDrawCellEventHandler)this.Events[onCustomDrawCell] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawCell")]
#endif
		public event CustomDrawCellEventHandler CustomDrawCell {
			add { Events.AddHandler(onCustomDrawCell, value); }
			remove { Events.RemoveHandler(onCustomDrawCell, value); }
		}
		protected internal virtual void RaiseCustomDrawCell(CustomDrawCellEventArgs args) {
			CustomDrawCellEventHandler handler = (CustomDrawCellEventHandler)this.Events[onCustomDrawCell];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawCellBackground
		static readonly object onCustomDrawCellBackground = new object();
		internal bool HasCustomDrawCellBackground { get { return (CustomDrawCellBackgroundEventHandler)this.Events[onCustomDrawCellBackground] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawCellBackground")]
#endif
		public event CustomDrawCellBackgroundEventHandler CustomDrawCellBackground {
			add { Events.AddHandler(onCustomDrawCellBackground, value); }
			remove { Events.RemoveHandler(onCustomDrawCellBackground, value); }
		}
		protected internal virtual void RaiseCustomDrawCellBackground(CustomDrawCellBackgroundEventArgs args) {
			CustomDrawCellBackgroundEventHandler handler = (CustomDrawCellBackgroundEventHandler)this.Events[onCustomDrawCellBackground];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawColumnHeader
		static readonly object onCustomDrawColumnHeader = new object();
		internal bool HasCustomDrawColumnHeader { get { return (CustomDrawColumnHeaderEventHandler)this.Events[onCustomDrawColumnHeader] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawColumnHeader")]
#endif
		public event CustomDrawColumnHeaderEventHandler CustomDrawColumnHeader {
			add { Events.AddHandler(onCustomDrawColumnHeader, value); }
			remove { Events.RemoveHandler(onCustomDrawColumnHeader, value); }
		}
		protected internal virtual void RaiseCustomDrawColumnHeader(CustomDrawColumnHeaderEventArgs args) {
			CustomDrawColumnHeaderEventHandler handler = (CustomDrawColumnHeaderEventHandler)this.Events[onCustomDrawColumnHeader];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawColumnHeaderBackground
		static readonly object onCustomDrawColumnHeaderBackground = new object();
		internal bool HasCustomDrawColumnHeaderBackground { get { return (CustomDrawColumnHeaderBackgroundEventHandler)this.Events[onCustomDrawColumnHeaderBackground] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawColumnHeaderBackground")]
#endif
		public event CustomDrawColumnHeaderBackgroundEventHandler CustomDrawColumnHeaderBackground {
			add { Events.AddHandler(onCustomDrawColumnHeaderBackground, value); }
			remove { Events.RemoveHandler(onCustomDrawColumnHeaderBackground, value); }
		}
		protected internal virtual void RaiseCustomDrawColumnHeaderBackground(CustomDrawColumnHeaderBackgroundEventArgs args) {
			CustomDrawColumnHeaderBackgroundEventHandler handler = (CustomDrawColumnHeaderBackgroundEventHandler)this.Events[onCustomDrawColumnHeaderBackground];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawRowHeader
		static readonly object onCustomDrawRowHeader = new object();
		internal bool HasCustomDrawRowHeader { get { return (CustomDrawRowHeaderEventHandler)this.Events[onCustomDrawRowHeader] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawRowHeader")]
#endif
		public event CustomDrawRowHeaderEventHandler CustomDrawRowHeader {
			add { Events.AddHandler(onCustomDrawRowHeader, value); }
			remove { Events.RemoveHandler(onCustomDrawRowHeader, value); }
		}
		protected internal virtual void RaiseCustomDrawRowHeader(CustomDrawRowHeaderEventArgs args) {
			CustomDrawRowHeaderEventHandler handler = (CustomDrawRowHeaderEventHandler)this.Events[onCustomDrawRowHeader];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawRowHeaderBackground
		static readonly object onCustomDrawRowHeaderBackground = new object();
		internal bool HasCustomDrawRowHeaderBackground { get { return (CustomDrawRowHeaderBackgroundEventHandler)this.Events[onCustomDrawRowHeaderBackground] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawRowHeaderBackground")]
#endif
		public event CustomDrawRowHeaderBackgroundEventHandler CustomDrawRowHeaderBackground {
			add { Events.AddHandler(onCustomDrawRowHeaderBackground, value); }
			remove { Events.RemoveHandler(onCustomDrawRowHeaderBackground, value); }
		}
		protected internal virtual void RaiseCustomDrawRowHeaderBackground(CustomDrawRowHeaderBackgroundEventArgs args) {
			CustomDrawRowHeaderBackgroundEventHandler handler = (CustomDrawRowHeaderBackgroundEventHandler)this.Events[onCustomDrawRowHeaderBackground];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region CustomDrawFrozenPaneBorder
		static readonly object onCustomDrawFrozenPaneBorder = new object();
		internal bool HasCustomDrawFrozenPaneBorder { get { return (CustomDrawFrozenPaneBorderEventHandler)this.Events[onCustomDrawFrozenPaneBorder] != null; } }
#if !SL
	[DevExpressXtraSpreadsheetLocalizedDescription("SpreadsheetControlCustomDrawFrozenPaneBorder")]
#endif
		public event CustomDrawFrozenPaneBorderEventHandler CustomDrawFrozenPaneBorder {
			add { Events.AddHandler(onCustomDrawFrozenPaneBorder, value); }
			remove { Events.RemoveHandler(onCustomDrawFrozenPaneBorder, value); }
		}
		protected internal virtual void RaiseCustomDrawFrozenPaneBorder(CustomDrawFrozenPaneBorderEventArgs args) {
			CustomDrawFrozenPaneBorderEventHandler handler = (CustomDrawFrozenPaneBorderEventHandler)this.Events[onCustomDrawFrozenPaneBorder];
			if (handler != null)
				handler(this, args);
		}
		#endregion
		#region HyperlinkFormShowing
		static readonly object onHyperlinkFormShowing = new object();
		protected internal event HyperlinkFormShowingEventHandler HyperlinkFormShowing {
			add { Events.AddHandler(onHyperlinkFormShowing, value); }
			remove { Events.RemoveHandler(onHyperlinkFormShowing, value); }
		}
		protected internal virtual void RaiseHyperlinkFormShowing(HyperlinkFormShowingEventArgs e) {
			HyperlinkFormShowingEventHandler handler = (HyperlinkFormShowingEventHandler)Events[onHyperlinkFormShowing];
			if (handler != null)
				handler(this, e);
		}
		#endregion
	}
	#region PopupMenuShowingEventHandler
	public delegate void PopupMenuShowingEventHandler(object sender, PopupMenuShowingEventArgs e);
	#endregion
	#region PopupMenuShowingEventArgs
	public class PopupMenuShowingEventArgs : EventArgs {
		#region Fields
		SpreadsheetPopupMenu menu;
		SpreadsheetMenuType menuType;
		#endregion
		public PopupMenuShowingEventArgs(SpreadsheetPopupMenu menu, SpreadsheetMenuType menuType) {
			Guard.ArgumentNotNull(menu, "menu");
			this.menu = menu;
			this.menuType = menuType;
		}
		#region Properties
		public SpreadsheetPopupMenu Menu { get { return menu; } set { menu = value; } }
		public SpreadsheetMenuType MenuType { get { return menuType; } }
		#endregion
	}
	#endregion
}
