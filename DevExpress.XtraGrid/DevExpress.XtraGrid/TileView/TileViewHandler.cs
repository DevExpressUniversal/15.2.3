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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Tile.Handler {
	class TileViewHandler : BaseViewHandler {
		public TileViewHandler(BaseView view) : base(view) { }
		protected TileView TileView { get { return View as TileView; } }
		public TileControlHandler HandlerCore { get { return (TileView.ViewInfo as ITileControl).Handler; } }
		protected override void OnResize(Rectangle clientRect) {
			TileView.InternalSetViewRectCore(clientRect);
		}
		protected override void OnMouseEnter(EventArgs e) {
			HandlerCore.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			HandlerCore.OnMouseLeave(e);
		}
		protected override bool OnMouseMove(System.Windows.Forms.MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseMove(e);
			if(e.Handled)
				return true;
			HandlerCore.OnMouseMove(e);
			return false;
		}
		protected override bool OnMouseDown(System.Windows.Forms.MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseDown(e);
			if(e.Handled)
				return true;
			HandlerCore.OnMouseDown(e);
			return false;
		}
		protected override bool OnMouseUp(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseUp(e);
			if(e.Handled)
				return true;
			HandlerCore.OnMouseUp(e);
			return false;
		}
		MouseWheelScrollHelper wheelScrollHelper;
		MouseWheelScrollHelper WheelScrollHelper {
			get {
				if(wheelScrollHelper == null)
					wheelScrollHelper = new MouseWheelScrollHelper(HandlerCore as TileViewHandlerCore);
				return wheelScrollHelper;
			}
		}
		protected override bool OnMouseWheel(MouseEventArgs ev) {
			DXMouseEventArgs e = DXMouseEventArgs.GetMouseArgs(ev);
			base.OnMouseWheel(e);
			if(e.Handled)
				return true;
			WheelScrollHelper.OnMouseWheel(e);
			return false;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled)
				return;
			if(TileView.CheckShowFindPanelKey(e))
				return;
			HandlerCore.OnKeyDown(e);
		}
	}
	public class TileViewHandlerCore : TileControlHandler, IMouseWheelScrollClient {
		public TileViewHandlerCore(ITileControl owner) : base(owner) { }
		TileViewInfo ViewInfo { get { return Control as TileViewInfo; } }
		TileView View { get { return ViewInfo.View as TileView; } }
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			OnMouseWheel(e);
		}
		bool IMouseWheelScrollClient.PixelModeHorz {
			get { return true; }
		}
		bool IMouseWheelScrollClient.PixelModeVert {
			get { return true; }
		}
		protected override bool CheckItemCore(TileItem item) {
			if(!View.AllowEditCheckColumn) return true;
			bool result = base.CheckItemCore(item);
			if(result)
				View.ToggleRowCheckValue(item as TileViewItem);
			return result;
		}
	}
}
