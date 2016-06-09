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
using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using System.Drawing;
using System.Drawing.Text;
using DevExpress.Skins;
using DevExpress.XtraEditors.Drawing;																						   
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraDashboardLayout;
namespace DevExpress.XtraLayout.Painting {
	public class LayoutControlItemSkinPainter : LayoutControlItemPainter {
		public LayoutControlItemSkinPainter() : base() { }
		public override DevExpress.XtraLayout.Utils.Padding GetPadding(BaseViewInfo e) {
			SkinElementInfo si = GetSkinLayoutItemBackground(e);
			if (si != null && si.Element != null) {
				SkinPaddingEdges skinPadding = si.Element.ContentMargins;
				return new Utils.Padding(skinPadding.Left, skinPadding.Right, skinPadding.Top, skinPadding.Bottom);
			}
			return Utils.Padding.Empty;
		}
		public override DevExpress.XtraLayout.Utils.Padding GetSpacing(BaseViewInfo e) {
			SkinElementInfo si = GetSkinLayoutItemBackground(e);
			if (si != null && si.Element != null) {
				int skinSpacing = si.Element.Properties.GetInteger("Spacing");
				return new Utils.Padding(skinSpacing);
			}
			return Utils.Padding.Empty;
		}
		SkinElementInfo _info = null;
		protected virtual SkinElementInfo GetSkinLayoutItemBackground(BaseViewInfo e) {
			if (_info == null) {
				if (e.Owner == null || e.OwnerILayoutControl == null) return null;
				_info = new SkinElementInfo(CommonSkins.GetSkin(e.OwnerILayoutControl.LookAndFeel)[CommonSkins.SkinLayoutItemBackground], Rectangle.Empty);
			}
			return _info;
		}
		protected virtual Rectangle GetPaintBounds(BaseViewInfo e) {
			Rectangle bounds = e.Owner.ViewInfo.BoundsRelativeToControl;
			if (e.Owner.Parent != null) {
				DevExpress.XtraLayout.Utils.Padding padding = GetSpacing(e);
				bounds.X += padding.Left;
				bounds.Y += padding.Top;
				bounds.Width -= padding.Width;
				bounds.Height -= padding.Height;
			}
			return bounds;
		}
		protected virtual void PatchImageIndex(SkinElementInfo info, BaseLayoutItemViewInfo e) { 
		}
		protected override void DrawBackground(BaseLayoutItemViewInfo e) {
			if (e.Owner == null || e.OwnerILayoutControl == null) return;
			if (GetSkinLayoutItemBackground(e) == null || !e.OwnerILayoutControl.OptionsView.AllowItemSkinning) {
				base.DrawBackground(e);
				return;
			}
			SkinElementInfo info = GetSkinLayoutItemBackground(e);
			LayoutControlItem controlItem = e.Owner as LayoutControlItem;
			if(info == null) return;
			if(e.Owner is DashboardLayoutControlGroupBase) {
				PatchImageIndexCore(e, info, null);
				base.DrawBackground(e);
				return;
			}
			if (controlItem == null) return;
			PatchImageIndexCore(e, info, controlItem);
			base.DrawBackground(e);
		}
		protected virtual void PatchImageIndexCore(BaseLayoutItemViewInfo e, SkinElementInfo info, LayoutControlItem controlItem) {
			info.Bounds = GetPaintBounds(e);
			info.ImageIndex = -1;
			if(e.OwnerILayoutControl.OptionsView.DrawItemBorders) {
				info.ImageIndex = 0;
			}
			if(e.State == ObjectState.Hot && (e.OwnerILayoutControl.OptionsView.AllowHotTrack))
				info.ImageIndex = 1;
			if(controlItem!= null && controlItem.Control != null) {
				if((controlItem.Control.ContainsFocus || IsPopupOpen(controlItem.Control)) && (e.OwnerILayoutControl.OptionsView.HighlightFocusedItem)) info.ImageIndex = 2;
				if(!controlItem.Control.Enabled && (e.OwnerILayoutControl.OptionsView.HighlightDisabledItem)) info.ImageIndex = 3;
			} else
				if(e.OwnerILayoutControl.EnableCustomizationMode) info.ImageIndex = 3;
			if(info.ImageIndex > 0 && !(e.OwnerILayoutControl.OptionsView.DrawItemBorders)) {
				Rectangle rect = info.Bounds;
				rect.Inflate(-1, -1);
				info.Bounds = rect; ;
			}
			PatchImageIndex(info, e);
			if(info.ImageIndex >= 0)
				ObjectPainter.DrawObject(e.Cache, SkinElementPainter.Default, info);
		}
		protected virtual bool IsPopupOpen(Control control) {
			PopupBaseEdit ppbe = control as PopupBaseEdit;
			return ppbe != null && ppbe.IsPopupOpen;
		}
	}
	public class LayoutControlItemPainter : BaseLayoutItemPainter {
		private LabelControlObjectPainter _labelPainter = null;
		protected LabelControlObjectPainter GetLabelPainter(BaseViewInfo e) {
			if (_labelPainter == null) {
				_labelPainter = CreateLabelPainter(e);
			}
			return _labelPainter;
		}
		protected virtual LabelControlObjectPainter CreateLabelPainter(BaseViewInfo e) {
			LayoutControlItem controlItem = e.Owner as LayoutControlItem;
			if (controlItem != null) {
				switch (controlItem.GetPaintingType()) {
					case PaintingType.Normal: return new LabelControlObjectPainter();
					case PaintingType.Skinned: return new LabelControlSkinObjectPainter(e.OwnerILayoutControl.LookAndFeel);
					case PaintingType.XP: return new LabelControlWindowsXPObjectPainter();
				}
			}
			return new LabelControlObjectPainter();
		}
		public LayoutControlItemPainter() : base() { }
		protected override void DrawTextArea(BaseLayoutItemViewInfo e) {
			LayoutControlItem lci = e.Owner as LayoutControlItem;
			if (lci == null) return;
			if (!lci.TextVisible) return;
			if (!lci.ContentVisible) return;
			LayoutControlItemViewInfo vi = e as LayoutControlItemViewInfo;
			vi.UpdateAppearance();
			vi.LabelInfo.Cache = e.Cache;
			vi.CalculateLabelRects(vi.TextAreaRelativeToControl);
			vi.LabelInfo.PaintAppearance.DrawBackground(e.Cache, vi.LabelInfo.TextRect);
			if (vi.LabelInfo.PaintAppearance.TextOptions.HotkeyPrefix == HKeyPrefix.None && vi.LabelInfo.PaintAppearance.Options.UseTextOptions) vi.LabelInfo.HotkeyPrefixState = HotkeyPrefix.None;
			GetLabelPainter(e).DrawObject(vi.LabelInfo);
		}
	}
}
