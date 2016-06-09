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

using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.Drawing;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils;
namespace DevExpress.XtraGrid.Views.WinExplorer.Drawing {
	internal static class Win8WinExplorerViewColors {
		public static Color groupSelectedHoveredBrushColor = Color.FromArgb(50, 51, 153, 255);
		public static Color groupSelectedPenColor = Color.FromArgb(51, 153, 255);
		public static Color groupHoveredBrushColor = Color.FromArgb(50, 112, 192, 231);
		public static Color groupHoveredPenColor = Color.FromArgb(112, 192, 231);
		public static Color expGCBNormalBrushColor = Color.FromArgb(89, 89, 89);
		public static Color expGCBNormalPenColor = Color.FromArgb(38, 38, 38);
		public static Color expGCBHoveredBrushColor = Color.FromArgb(100, 28, 196, 247);
		public static Color expGCBHoveredPenColor = Color.FromArgb(28, 196, 247);
		public static Color colGCBNormalPenColor = Color.FromArgb(166, 166, 166);
		public static Color colGCBHoveredBrushColor = Color.FromArgb(100, 39, 199, 247);
		public static Color colGCBHoveredPenColor = Color.FromArgb(39, 199, 247);
	}
	public class Windows8WinExplorerViewPainter : WinExplorerViewPainter {
		public Windows8WinExplorerViewPainter(BaseView view) : base(view) { }
		protected internal override void DrawGroupBackground(ViewDrawArgs e, DevExpress.XtraGrid.Views.WinExplorer.ViewInfo.WinExplorerGroupViewInfo groupInfo) {
			base.DrawGroupBackground(e, groupInfo);
			DrawGroupState(e, groupInfo);
		}
		protected override void DrawGroupCaptionLineCore(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			base.DrawGroupCaptionLineCore(e, groupInfo);
			XPGroupLinePainter painter = new XPGroupLinePainter();
			XPObjectInfoArgs args = new XPObjectInfoArgs();
			args.Bounds = groupInfo.LineBounds;
			ObjectPainter.DrawObject(e.Cache, painter, args);
		}
		protected void DrawGroupState(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			if(groupInfo.IsSelected || groupInfo.IsFocused) {
				if(groupInfo.IsHovered)
					e.Graphics.FillRectangle(e.Cache.GetSolidBrush(Win8WinExplorerViewColors.groupSelectedHoveredBrushColor), groupInfo.SelectionBounds);
				e.Graphics.DrawRectangle(e.Cache.GetPen(Win8WinExplorerViewColors.groupSelectedPenColor), new Rectangle(groupInfo.SelectionBounds.X, groupInfo.SelectionBounds.Y, groupInfo.SelectionBounds.Width - 1, groupInfo.SelectionBounds.Height - 1));
			}
			else if(groupInfo.IsHovered) {
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(Win8WinExplorerViewColors.groupHoveredBrushColor), groupInfo.SelectionBounds);
				e.Graphics.DrawRectangle(e.Cache.GetPen(Win8WinExplorerViewColors.groupHoveredPenColor), new Rectangle(groupInfo.SelectionBounds.X, groupInfo.SelectionBounds.Y, groupInfo.SelectionBounds.Width - 1, groupInfo.SelectionBounds.Height - 1));
			}
		}
		protected internal override void DrawGroupCaption(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			e.ViewInfo.PaintAppearance.GetAppearance("GroupNormal").DrawString(e.Cache, groupInfo.Text, groupInfo.TextBounds); 
		}
		protected internal override void DrawItemBackground(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			if(itemInfo.IsHovered) {
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance("ItemHovered").BackColor), itemInfo.SelectionBounds);
				e.Graphics.DrawRectangle(e.Cache.GetPen(e.ViewInfo.PaintAppearance.GetAppearance("ItemHovered").BorderColor), new Rectangle(itemInfo.SelectionBounds.X, itemInfo.SelectionBounds.Y, itemInfo.SelectionBounds.Width - 1, itemInfo.SelectionBounds.Height - 1));
			}
			else if(itemInfo.IsSelected || itemInfo.IsPressed) {
				e.Graphics.FillRectangle(e.Cache.GetSolidBrush(e.ViewInfo.PaintAppearance.GetAppearance("ItemPressed").BackColor), itemInfo.SelectionBounds);
				e.Graphics.DrawRectangle(e.Cache.GetPen(e.ViewInfo.PaintAppearance.GetAppearance("ItemPressed").BorderColor), new Rectangle(itemInfo.SelectionBounds.X, itemInfo.SelectionBounds.Y, itemInfo.SelectionBounds.Width - 1, itemInfo.SelectionBounds.Height - 1));
			}
			else if(itemInfo.IsFocused) {
				e.Graphics.DrawRectangle(e.Cache.GetPen(e.ViewInfo.PaintAppearance.GetAppearance("ItemPressed").BorderColor), new Rectangle(itemInfo.SelectionBounds.X, itemInfo.SelectionBounds.Y, itemInfo.SelectionBounds.Width - 1, itemInfo.SelectionBounds.Height - 1));
			}
		}
		protected override void DrawSelection(ViewDrawArgs e) {
			base.DrawSelection(e);
			Pen selectionPen = e.Cache.GetPen(Color.Black);
			selectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			e.Graphics.DrawRectangle(selectionPen, ((WinExplorerViewInfo)e.ViewInfo).SelectionVisualRect);
		}
		protected internal override void DrawGroupCaptionButton(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			Size captionExpandButtonSize = new Size(3, 7);
			Size captionCollapseButtonSize = new Size(5, 5);
			PointF[] pointsCollapse = ((Windows8WinExplorerViewInfo)e.ViewInfo).CalcCollapseButtonCoordinates(groupInfo.CaptionButtonBounds, captionExpandButtonSize);
			PointF[] pointsExpand = ((Windows8WinExplorerViewInfo)e.ViewInfo).CalcExpandButtonCoordinates(groupInfo.CaptionButtonBounds, captionCollapseButtonSize);
			if(groupInfo.ViewInfo.WinExplorerView.GetRowExpanded(groupInfo.Row.RowHandle)) {
				if(((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Normal) {
					e.Graphics.FillPolygon(e.Cache.GetSolidBrush(Win8WinExplorerViewColors.expGCBNormalBrushColor), pointsExpand);
					e.Graphics.DrawPolygon(e.Cache.GetPen(Win8WinExplorerViewColors.expGCBNormalPenColor), pointsExpand);
				}
				else if(((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Hot || ((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Pressed) {
					e.Graphics.FillPolygon(e.Cache.GetSolidBrush(Win8WinExplorerViewColors.expGCBHoveredBrushColor), pointsExpand);
					e.Graphics.DrawPolygon(e.Cache.GetPen(Win8WinExplorerViewColors.expGCBHoveredPenColor), pointsExpand);
				}
			}
			else {
				if(((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Normal) {
					e.Graphics.DrawPolygon(e.Cache.GetPen(Win8WinExplorerViewColors.colGCBNormalPenColor), pointsCollapse);
				}
				else if(((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Hot || ((WinExplorerViewInfo)e.ViewInfo).CalcCaptionButtonState(groupInfo) == ObjectState.Pressed) {
					e.Graphics.FillPolygon(e.Cache.GetSolidBrush(Win8WinExplorerViewColors.colGCBHoveredBrushColor), pointsCollapse);
					e.Graphics.DrawPolygon(e.Cache.GetPen(Win8WinExplorerViewColors.colGCBHoveredPenColor), pointsCollapse);
				}
			}
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
				args.Bounds = new Rectangle(itemInfo.ItemSeparatorBounds.Location, new Size(itemInfo.ItemSeparatorBounds.Width, 1));
				ObjectPainter.DrawObject(e.Cache, painter, args);
			}
		}
	}
}
