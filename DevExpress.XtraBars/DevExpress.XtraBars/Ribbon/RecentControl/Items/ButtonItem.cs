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

using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Ribbon.Drawing;
using DevExpress.XtraBars.Ribbon.Handler;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Ribbon {
	public enum RecentButtonType { Simple, DropDown }
	public class RecentButtonItem : RecentTextGlyphItemBase {
		static readonly Size DefaultSize = new Size(82, 86);
		Orientation orientation;
		Size size;
		bool autoSize;
		IDXDropDownControl dropDownControl;
		IDXMenuManager menuManager;
		public RecentButtonItem()
			: base() {
				this.dropDownControl = null;
			this.orientation = Orientation.Horizontal;
			this.autoSize = true;
			this.size = DefaultSize;
		}
		[ DXCategory(CategoryName.Behavior), DefaultValue(null), TypeConverter(typeof(ReferenceConverter))]
		private IDXDropDownControl DropDownControl {
			get { return dropDownControl; }
			set {
				if(DropDownControl == value) return;
				if(DropDownControl != null && DropDownControl is IDXDropDownControlEx) {
					(DropDownControl as IDXDropDownControlEx).CloseUp -= new EventHandler(OnDropDownCloseUp);
				}
				dropDownControl = value;
				if(DropDownControl != null && DropDownControl is IDXDropDownControlEx) {
					(DropDownControl as IDXDropDownControlEx).CloseUp += new EventHandler(OnDropDownCloseUp);
				}
			}
		}
		[ DXCategory(CategoryName.Behavior), DefaultValue((string)null)]
		private IDXMenuManager MenuManager {
			get { return menuManager; }
			set { menuManager = value; }
		}
		void OnDropDownCloseUp(object sender, EventArgs e) {
			throw new NotImplementedException();
		}
		[DefaultValue(Orientation.Horizontal)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(Orientation == value) return;
				orientation = value;
				OnItemChanged();
			}
		}
		[DefaultValue(true)]
		public bool AutoSize {
			get { return autoSize; }
			set {
				if(AutoSize == value) return;
				autoSize = value;
				OnItemChanged();
			}
		}
		public Size Size {
			get { return size; }
			set { SetSizeCore(value); }
		}
		bool needUpdateSize;
		protected void SetSizeCore(Size newSize) {
			if(AutoSize)
				if(RecentControl == null)
					needUpdateSize = true;
				else newSize = ViewInfo.CalcBestSize(0);
			if(Size == newSize) return;
			size = newSize;
			OnItemChanged();
		}
		protected internal override void OnItemChanged() {
			needUpdateSize = true;
			UpdateSize();
			base.OnItemChanged();
		}
		protected internal override void OnRecentControlChanged(RecentItemControl rc) {
			base.OnRecentControlChanged(rc);
			if(rc != null)
				UpdateSize();
		}
		protected override void OnOwnerPanelChanged() {
			base.OnOwnerPanelChanged();
			if(RecentControl != null)
				UpdateSize();
		}
		protected internal void UpdateSize() {
			if(AutoSize) SetSizeCore(Size);
		}
		protected override RecentItemViewInfoBase CreateItemViewInfo() {
			return new RecentButtonItemViewInfo(this);
		}
		protected override RecentItemPainterBase CreateItemPainter() {
			return new RecentButtonItemPainter();
		}
		protected override RecentItemHandlerBase CreateItemHandler() {
			return new RecentControlButtonItemHandler(this);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RecentButtonItemViewInfo : RecentTextGlyphItemViewInfoBase {
		public RecentButtonItemViewInfo(RecentButtonItem item) : base(item) { }
		protected override BaseRecentItemAppearanceCollection ControlAppearances {
			get { return (Item.RecentControl.Appearances as RecentAppearanceCollection).ButtonItem; }
		}
		protected override BaseRecentItemAppearanceCollection ItemAppearanceCollection {
			get { return ButtonItem.Appearances as BaseRecentItemAppearanceCollection; }
		}
		protected override AppearanceDefaultInfo[] GetAppearanceDefaultInfo() {
			AppearanceDefault app = new AppearanceDefault();
			app.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
			app.ForeColor = CommonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel).Colors[CommonColors.ControlText];
			app.FontStyleDelta = System.Drawing.FontStyle.Bold;
			app.HAlignment = HorzAlignment.Center;
			return new AppearanceDefaultInfo[]{
				new AppearanceDefaultInfo("ItemNormal", app),
				new AppearanceDefaultInfo("ItemHovered", app),
				new AppearanceDefaultInfo("ItemPressed", app)
			};
		}
		protected override void UpdateDefaults(AppearanceObject app) {
			base.UpdateDefaults(app);
			app.TextOptions.WordWrap = WordWrap.Wrap;
			app.TextOptions.HAlignment = HorzAlignment.Center;
		}
		public RecentButtonItem ButtonItem { get { return Item as RecentButtonItem; } }
		public Orientation Orientation { get { return ButtonItem.Orientation; } }
		protected override Size CalcBestSizeCore(int width) {
			Size size = base.CalcBestSizeCore(width);
			if(Orientation == System.Windows.Forms.Orientation.Horizontal)
				size = new Size(GlyphSize.Width + CaptionTextSize.Width + ButtonItem.GlyphToCaptionIndent, Math.Max(GlyphSize.Height, CaptionTextSize.Height));
			else {
				size.Width = Math.Max(GlyphSize.Width, CaptionTextSize.Width);
				size.Height = CaptionTextSize.Height + (GlyphSize.Height == 0 ? 0 : ButtonItem.GlyphToCaptionIndent + GlyphSize.Height);
			}
			return size;
		}
		protected override Padding GetSkinItemPadding() {
			return new Padding(22, 15, 20, 19);
		}
		public override void CalcViewInfo(Rectangle bounds) {
			bounds = new Rectangle(bounds.Location, ButtonItem.Size);
			base.CalcViewInfo(bounds);
		}
		protected override Rectangle CalcCaptionTextBounds() {
			if(Orientation == System.Windows.Forms.Orientation.Horizontal) return base.CalcCaptionTextBounds();
			Rectangle rect = Rectangle.Empty;
			rect.X = ContentBounds.X;
			rect.Y = GlyphBounds.Height != 0 ? GlyphBounds.Bottom + ButtonItem.GlyphToCaptionIndent : ContentBounds.Y;
			rect.Width = ContentBounds.Width;
			rect.Height = ContentBounds.Bottom - rect.Y;
			return rect;
		}
		protected override Rectangle CalcGlyphBounds() {
			if(Orientation == System.Windows.Forms.Orientation.Horizontal) return base.CalcGlyphBounds();
			Rectangle rect = Rectangle.Empty;
			rect.X = ContentBounds.X + (ContentBounds.Width - GlyphSize.Width) / 2;
			rect.Y = ContentBounds.Y;
			rect.Size = GlyphSize;
			return rect;
		}
		public override SkinElementInfo GetItemInfo() {
			SkinElement element = CommonSkins.GetSkin(Item.RecentControl.LookAndFeel.ActiveLookAndFeel)["Button"];
			if(element == null)
				element = CommonSkins.GetSkin(DevExpress.XtraEditors.Controls.DefaultSkinProvider.Default)["Button"];
			SkinElementInfo info = new SkinElementInfo(element, new Rectangle(Bounds.Location, ButtonItem.Size));
			info.ImageIndex = -1;
			info.State = CalcItemState();
			return info;
		}
		protected internal override int CalcMinWidth() {
			return Bounds.Width;
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RecentButtonItemPainter : RecentTextGlyphItemPainterBase {
		protected override void DrawBackground(GraphicsCache cache, RecentItemViewInfoBase buttonInfo) {
			base.DrawBackground(cache, buttonInfo);
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Handler {
	public class RecentControlButtonItemHandler : RecentItemHandlerBase {
		public RecentControlButtonItemHandler(RecentButtonItem item) : base(item) { }
		public RecentButtonItem ButtonItem { get { return Item as RecentButtonItem; } }
		public override bool OnMouseUp(MouseEventArgs e) {
			return base.OnMouseUp(e);
		}
	}
}
