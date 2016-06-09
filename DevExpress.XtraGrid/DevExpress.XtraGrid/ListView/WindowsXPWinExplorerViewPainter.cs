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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.Drawing;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
namespace DevExpress.XtraGrid.Views.WinExplorer.Drawing {
	public class WindowsXPWinExplorerViewPainter : WinExplorerViewPainter {
		static Color groupLineColor = Color.FromArgb(61, 149, 255);
		public WindowsXPWinExplorerViewPainter(BaseView view) : base(view) { }
		protected override void DrawGroupCaptionLineCore(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			((WindowsXPWinExplorerViewInfo)e.ViewInfo).CalcGroupLineBounds(groupInfo);
			Rectangle rect = groupInfo.LineBounds;
			LinearGradientBrush gb = new LinearGradientBrush(rect, groupLineColor, Color.White, LinearGradientMode.Horizontal);
			e.Graphics.FillRectangle(gb, rect);
		}
		protected internal override void DrawGroupCaptionButton(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			XPWinExplorerViewExpandButtonPainter painter = new XPWinExplorerViewExpandButtonPainter(((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo), groupInfo.ViewInfo.WinExplorerView.GetRowExpanded(groupInfo.Row.RowHandle));
			XPObjectInfoArgs args = new XPObjectInfoArgs();
			args.Bounds = groupInfo.CaptionButtonBounds;
			ObjectPainter.DrawObject(e.Cache, painter, args);
		}
		protected override void DrawSelection(ViewDrawArgs e) {
			base.DrawSelection(e);
			Pen myPen = e.Cache.GetPen(Color.Black);
			myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			e.Graphics.DrawRectangle(myPen, ((WinExplorerViewInfo)e.ViewInfo).SelectionVisualRect);
			myPen.Dispose();
		}
		protected internal override void DrawItemText(ViewDrawArgs e, ViewInfo.WinExplorerItemViewInfo itemInfo, WinExplorerViewCustomDrawItemEventArgs args) {
			DrawTextBackground(e, itemInfo);
			base.DrawItemText(e, itemInfo, args);
		}
		protected virtual void DrawTextBackground(ViewDrawArgs e, ViewInfo.WinExplorerItemViewInfo itemInfo){
			Pen myPen = new Pen(Color.Black);
			Rectangle textRect = itemInfo.TextBounds;
			WinExplorerView view = ((WinExplorerViewInfo)e.ViewInfo).WinExplorerView;
			myPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			if(view.OptionsView.Style == WinExplorerViewStyle.List || view.OptionsView.Style == WinExplorerViewStyle.Tiles || view.OptionsView.Style == WinExplorerViewStyle.Content)
				textRect = new Rectangle(itemInfo.TextBounds.X, itemInfo.SelectionBounds.Y, itemInfo.SelectionBounds.Right - itemInfo.TextBounds.X - 1, itemInfo.SelectionBounds.Height - 1);
			if(itemInfo.IsPressed || itemInfo.IsSelected) {
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance("ItemPressed").BackColor), new Rectangle(textRect.X + 1, textRect.Y + 1, textRect.Width - 1, textRect.Height - 1));
				e.Graphics.DrawRectangle(myPen, textRect);
			}
			if(itemInfo.IsFocused)
				e.Graphics.DrawRectangle(myPen, textRect);
			myPen.Dispose();
		}
		protected internal override void DrawItemImage(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			base.DrawItemImage(e, itemInfo);
			DrawImageBackground(e, itemInfo);
		}
		protected virtual void DrawImageBackground(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			if(((WinExplorerViewInfo)e.ViewInfo).WinExplorerView.OptionsView.Style == WinExplorerViewStyle.ExtraLarge || ((WinExplorerViewInfo)e.ViewInfo).WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Large) {
				int width = 1;
				string text = "ItemNormal";
				if(itemInfo.IsPressed || itemInfo.IsSelected) {
					width = 3;
					text = "ItemPressed";
				}
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance(text).BackColor), new Rectangle(itemInfo.ImageBounds.X - width, itemInfo.ImageBounds.Y - width, itemInfo.ImageBounds.Width + width * 2, width));
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance(text).BackColor), new Rectangle(itemInfo.ImageBounds.Right, itemInfo.ImageBounds.Y - width, width, itemInfo.ImageBounds.Height + width * 2));
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance(text).BackColor), new Rectangle(itemInfo.ImageBounds.X - width, itemInfo.ImageBounds.Y - width, width, itemInfo.ImageBounds.Height + width * 2));
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance(text).BackColor), new Rectangle(itemInfo.ImageBounds.X - width, itemInfo.ImageBounds.Bottom, itemInfo.ImageBounds.Width + width * 2, width));
			}
			else 
				if(itemInfo.IsPressed || itemInfo.IsSelected)
					e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance("ItemPressed").BackColor2), itemInfo.ImageBounds);
		}
		protected internal override void DrawItemCheck(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			base.DrawItemCheck(e, itemInfo);
			XPCheckBoxInfoArgs args;
			if(itemInfo.IsChecked)
				args = new XPCheckBoxInfoArgs(false, System.Windows.Forms.CheckState.Checked);
			else args = new XPCheckBoxInfoArgs(false, System.Windows.Forms.CheckState.Unchecked);
			XPCheckBoxPainter painter = new XPCheckBoxPainter();
			args.Bounds = itemInfo.CheckBoxBounds;
			ObjectPainter.DrawObject(e.Cache, painter, args);
		}
		protected internal override void DrawGroupCaptionCheckBox(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			base.DrawGroupCaptionCheckBox(e, groupInfo);
			XPCheckBoxInfoArgs args;
			args = new XPCheckBoxInfoArgs(false, groupInfo.CheckState);
			XPCheckBoxPainter painter = new XPCheckBoxPainter();
			args.Bounds = groupInfo.CheckBoxBounds;
			ObjectPainter.DrawObject(e.Cache, painter, args);
		}
		protected internal override void DrawItemSeparator(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			if(((WinExplorerViewInfo)e.ViewInfo).WinExplorerView.OptionsView.Style == WinExplorerViewStyle.Content) {
				XPGroupLinePainter painter = new XPGroupLinePainter();
				XPObjectInfoArgs args = new XPObjectInfoArgs();
				args.Bounds = new Rectangle(itemInfo.ItemSeparatorBounds.Location, new Size(itemInfo.ItemSeparatorBounds.Width,1));
				ObjectPainter.DrawObject(e.Cache, painter, args);
			}
		}
	}
	public class XPGroupLinePainter : XPObjectPainter {
		public XPGroupLinePainter() {
			DrawArgs = new WXPPainterArgs("LISTVIEW", 7, 1);
		}
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			int LVP_GROUPHEADERLINE = 7;
			DrawArgs.PartId = LVP_GROUPHEADERLINE; 
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			int LVGHL_OPEN = 3;
			return LVGHL_OPEN ;
		}
	}
	public class XPWinExplorerViewExpandButtonPainter : XPObjectPainter {
		public XPWinExplorerViewExpandButtonPainter(ObjectState state, bool isOpened) { 
			DrawArgs = new WXPPainterArgs("LISTVIEW", 8, 1);
			State = state;
			IsOpened = isOpened;
		}
		bool IsOpened { get; set; }
		ObjectState State { get; set; }
		protected override void UpdateDrawArgs(ObjectInfoArgs e) {
			base.UpdateDrawArgs(e);
			int LVP_EXPANDBUTTON = 8;
			int LVP_COLLAPSEBUTTON = 9;
			DrawArgs.PartId = IsOpened ? LVP_COLLAPSEBUTTON : LVP_EXPANDBUTTON;
		}
		protected override int CalcThemeStateId(ObjectInfoArgs e) {
			int LVEB_NORMAL = 1;
			int LVEB_HOVER = 2;
			int LVEB_PUSHED = 3;
			switch (State) {
				case ObjectState.Hot:
					return LVEB_HOVER;
				case ObjectState.Pressed:
					return LVEB_PUSHED;
				default: 
					return LVEB_NORMAL;
			}
		}
	}
}
