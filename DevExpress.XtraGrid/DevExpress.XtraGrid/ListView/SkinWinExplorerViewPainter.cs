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

using System.Drawing.Imaging;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using System.Drawing;
namespace DevExpress.XtraGrid.Views.WinExplorer.Drawing {
	public class SkinWinExplorerViewPainter : WinExplorerViewPainter {
		public SkinWinExplorerViewPainter(BaseView view) : base(view) { }
		ImageAttributes attributes;
		protected ImageAttributes Attributes {
			get {
				if(attributes == null)
					attributes = new ImageAttributes();
				return attributes;
			}
		}
		protected override void DrawBackground(ViewDrawArgs e, Rectangle bounds) {
			e.Graphics.FillRectangle(e.Cache.GetSolidBrush(GridSkins.GetSkin(e.ViewInfo.View).GetSystemColor(SystemColors.Window)), bounds);
		}
		protected void UpdateAttributes(double value) {
			float[][] matrix = new float[][] { 
				new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
				new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
				new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
				new float[] { 0.0f, 0.0f, 0.0f, (float)value, 0.0f },
				new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f },
			};
			Attributes.SetColorMatrix(new ColorMatrix(matrix));
		}
		protected override void DrawGroupCaptionLineCore(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			if(groupInfo.LineBounds == Rectangle.Empty)
				return;
			SkinWinExplorerViewInfo viewInfo = (SkinWinExplorerViewInfo)groupInfo.ViewInfo;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, viewInfo.GetGroupCaptionLine(groupInfo));
		}
		protected internal override void DrawGroupBackground(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			SkinWinExplorerViewInfo viewInfo = (SkinWinExplorerViewInfo)groupInfo.ViewInfo;
			SkinElementInfo info = viewInfo.GetGroupHeaderInfo(groupInfo);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected override void DrawFocusedRect(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			AppearanceObject obj = e.ViewInfo.PaintAppearance.GetAppearance("ItemNormal");
			e.Paint.DrawFocusRectangle(e.Graphics, itemInfo.SelectionBounds, obj.ForeColor, obj.BackColor);
		}
		protected internal override void DrawItemBackground(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			WinExplorerViewInfo vi = (WinExplorerViewInfo)e.ViewInfo;
			vi.GetTextAppearance(itemInfo).DrawBackground(e.Cache, itemInfo.Bounds); 
			if(itemInfo.NeedDrawFocusedRect)
				return;
			WinExplorerViewOpcaityAnimationInfo fadeInfo = XtraAnimator.Current.Get(e.ViewInfo, itemInfo.Row) as WinExplorerViewOpcaityAnimationInfo;
			SkinElementInfo info = ((SkinWinExplorerViewInfo)vi).GetItemBackgroundInfo(itemInfo);
			if(fadeInfo != null) {
				if(fadeInfo.PrevImageIndex != -1) {
					info.ImageIndex = fadeInfo.PrevImageIndex;
					ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
				}
				info.ImageIndex = itemInfo.StartImageIndex;
				UpdateAttributes(fadeInfo.Value);
				info.Attributes = Attributes;
			}
			if(info.ImageIndex == 0)
				return;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected virtual SkinElementInfo GetCheckBoxInfo(WinExplorerItemViewInfo itemInfo) {
			SkinElement elem = EditorsSkins.GetSkin(itemInfo.ViewInfo.WinExplorerView)[EditorsSkins.SkinCheckBox];
			SkinElementInfo info = new SkinElementInfo(elem, itemInfo.CheckBoxBounds);
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(elem.Image, GetCheckBoxState(itemInfo));
			info.ImageIndex += itemInfo.IsChecked ? 4 : 0;
			return info;
		}
		protected virtual SkinElementInfo GetGroupCheckBoxInfo(WinExplorerGroupViewInfo groupInfo) {
			SkinElement elem = EditorsSkins.GetSkin(groupInfo.ViewInfo.WinExplorerView)[EditorsSkins.SkinCheckBox];
			SkinElementInfo info = new SkinElementInfo(elem, groupInfo.CheckBoxBounds);
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(elem.Image, GetCheckBoxState(groupInfo));
			switch(groupInfo.CheckState) {
				case System.Windows.Forms.CheckState.Checked:
					info.ImageIndex += 4;
					break;
				case System.Windows.Forms.CheckState.Indeterminate:
					info.ImageIndex += 8;
					break;
			}
			return info;
		}
		protected virtual ObjectState GetCheckBoxState(WinExplorerItemViewInfo itemInfo) {
			if(itemInfo.ViewInfo.ColumnSet.CheckBoxColumn != null && !itemInfo.ViewInfo.ColumnSet.CheckBoxColumn.OptionsColumn.AllowEdit) {
				return ObjectState.Disabled;
			} else if(itemInfo.ViewInfo.PressedInfo.ItemInfo == itemInfo && (itemInfo.ViewInfo.PressedInfo.HitTest == WinExplorerViewHitTest.GroupCaptionCheckBox || itemInfo.ViewInfo.PressedInfo.HitTest == WinExplorerViewHitTest.ItemCheck)) {
				return ObjectState.Pressed;
			} else if(itemInfo.ViewInfo.HoverInfo.ItemInfo == itemInfo && (itemInfo.ViewInfo.HoverInfo.HitTest == WinExplorerViewHitTest.GroupCaptionCheckBox || itemInfo.ViewInfo.HoverInfo.HitTest == WinExplorerViewHitTest.ItemCheck)) {
				return ObjectState.Hot;
			} else {
				return ObjectState.Normal;
			}
		}
		protected internal override void DrawItemCheck(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetCheckBoxInfo(itemInfo));
		}
		protected override void DrawSelection(ViewDrawArgs e) {
			WinExplorerViewInfo vi = (WinExplorerViewInfo)e.ViewInfo;
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(e.ViewInfo.View)[CommonSkins.SkinSelection], vi.SelectionVisualRect);
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected internal override void DrawGroupCaptionButton(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			SkinWinExplorerViewInfo vi = (SkinWinExplorerViewInfo)e.ViewInfo;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.GetGroupCaptionButtonInfo(groupInfo));
		}
		protected internal override void DrawGroupCaptionCheckBox(ViewDrawArgs e, WinExplorerGroupViewInfo groupInfo) {
			SkinWinExplorerViewInfo vi = (SkinWinExplorerViewInfo)e.ViewInfo;
			ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, GetGroupCheckBoxInfo(groupInfo));
		}
		protected internal override void DrawItemSeparator(ViewDrawArgs e, WinExplorerItemViewInfo itemInfo) {
			SkinWinExplorerViewInfo vi = (SkinWinExplorerViewInfo)e.ViewInfo;
			if(vi.AllowDrawItemSeparator)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, vi.GetWinExplorerViewItemSeparatorInfo(itemInfo));
		}
	}
}
