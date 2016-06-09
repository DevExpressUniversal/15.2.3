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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors.Design {
	public class ContextButtonsPreviewControlPainter : BaseControlPainter {
		public override void Draw(ControlGraphicsInfoArgs info) {
			base.Draw(info);
			ContextButtonsPreviewControlViewInfo vi = (ContextButtonsPreviewControlViewInfo)info.ViewInfo;
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(vi.ContextItemsViewInfo, info.Cache, info.Bounds));
		}
		protected override void DrawBorder(ControlGraphicsInfoArgs info) {
			DrawOutsideArea(info);
			base.DrawBorder(info);
		}
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawImage(info);
		}
		private void DrawImage(ControlGraphicsInfoArgs info) {
			ContextButtonsPreviewControlViewInfo vi = (ContextButtonsPreviewControlViewInfo)info.ViewInfo;
			if(vi.PreviewOwner.Image != null)
				info.Graphics.DrawImage(vi.PreviewOwner.Image, vi.ImageBounds);
		}
		protected void DrawOutsideArea(ControlGraphicsInfoArgs info) {
			ContextButtonsPreviewControlViewInfo vi = (ContextButtonsPreviewControlViewInfo)info.ViewInfo;
			Rectangle left = new Rectangle(vi.Bounds.X, vi.Bounds.Y, vi.ContentRect.X - vi.Bounds.X, vi.Bounds.Height);
			Rectangle right = new Rectangle(vi.ContentRect.Right, vi.Bounds.Y, vi.Bounds.Right - vi.ContentRect.Right, vi.Bounds.Height);
			Rectangle top = new Rectangle(vi.ContentRect.X, vi.Bounds.Y, vi.ContentRect.Right, vi.ContentRect.Y - vi.Bounds.Y);
			Rectangle bottom = new Rectangle(vi.ContentRect.X, vi.ContentRect.Bottom, vi.ContentRect.Right, vi.Bounds.Bottom - vi.ContentRect.Bottom);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.Control)), left);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.Control)), top);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.Control)), right);
			info.Graphics.FillRectangle(info.Cache.GetSolidBrush(CommonSkins.GetSkin(vi.LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.Control)), bottom);
		}
	}
	public class ContextButtonsPreviewControlViewInfo : BaseStyleControlViewInfo {
		public ContextButtonsPreviewControlViewInfo(BaseStyleControl owner) : base(owner) { }
		public override void CalcViewInfo(Graphics g) {
			base.CalcViewInfo(g);
			ImageBounds = CalcImageBounds();
			ContextItemsViewInfo = new ContextItemCollectionViewInfo(PreviewOwner.ContextItems, PreviewOwner.Options, PreviewOwner);
			ContextItemsViewInfo.CalcItems();
		}
		private Rectangle CalcImageBounds() {
			if(PreviewOwner.Image == null)
				return Rectangle.Empty;
			return ImageLayoutHelper.GetImageBounds(PreviewOwner.ClientRectangle, PreviewOwner.Image.Size, ImageLayoutMode.ZoomInside); 
		}
		public Rectangle ImageBounds {
			get;
			set;
		}
		public override Rectangle BorderRect {
			get {
				return ContentRect;
			}
		}
		protected override void CalcContentRect(Rectangle bounds) {
			this.fContentRect = bounds;
		}
		internal ContextButtonsPreviewControl PreviewOwner { get { return (ContextButtonsPreviewControl)Owner; } }
		internal ContextItemCollectionViewInfo ContextItemsViewInfo { get; set; }
	}
	public class ContextButtonsPreviewControl : BaseStyleControl, IContextItemCollectionOptionsOwner, ISupportContextItems, IContextItemCollectionOwner {
		public ContextButtonsPreviewControl() {
			ContextItems = new ContextItemCollection(this);
			Options = new ContextItemCollectionOptions(this);
			UseRightToLeft = false;
		}
		protected override Drawing.BaseControlPainter CreatePainter() {
			return new ContextButtonsPreviewControlPainter();
		}
		protected override ViewInfo.BaseStyleControlViewInfo CreateViewInfo() {
			return new ContextButtonsPreviewControlViewInfo(this);
		}
		Image image;
		[DefaultValue(null)]
		public Image Image {
			get { return image; }
			set {
				if(Image == value)
					return;
				image = value;
				LayoutChanged();
			}
		}
		ContextItemCollection contextItems;
		public ContextItemCollection ContextItems {
			get { return contextItems; }
			set {
				if(ContextItems == value)
					return;
				contextItems = value;
				contextItems.Owner = this;
				LayoutChanged();
			}
		}
		ContextItemCollectionOptions options;
		public ContextItemCollectionOptions Options {
			get { return options; }
			set {
				if(Options == value)
					return;
				options = value;
				LayoutChanged();
			}
		}
		ContextItemCollectionHandler handler;
		protected ContextItemCollectionHandler Handler {
			get {
				if(handler == null)
					handler = new ContextItemCollectionHandler();
				return handler;
			}
		}
		protected override void OnMouseEnter(EventArgs e) {
			base.OnMouseEnter(e);
			Handler.ViewInfo = ((ContextButtonsPreviewControlViewInfo)ViewInfo).ContextItemsViewInfo;
			Handler.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			Handler.ViewInfo = ((ContextButtonsPreviewControlViewInfo)ViewInfo).ContextItemsViewInfo;
			Handler.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(IsDragging) {
				Point newPoint = PointToScreen(e.Location);
				Size deltaSize = new Size(newPoint.X - DownPoint.X, newPoint.Y - DownPoint.Y);
				Size = new Size(Math.Max(40, DownSize.Width + deltaSize.Width), Math.Max(40, DownSize.Height + deltaSize.Height));
			}
			Handler.ViewInfo = ((ContextButtonsPreviewControlViewInfo)ViewInfo).ContextItemsViewInfo;
			Handler.OnMouseMove(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			IsDragging = false;
			Capture = false;
		}
		Point DownPoint { get; set; }
		bool IsDragging { get; set; }
		Size DownSize { get; set; }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			Handler.ViewInfo = ((ContextButtonsPreviewControlViewInfo)ViewInfo).ContextItemsViewInfo;
			Handler.OnMouseDown(e);
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
		Rectangle ISupportContextItems.DisplayBounds {
			get {
				Rectangle res = (ViewInfo as ContextButtonsPreviewControlViewInfo).ImageBounds;
				return res;
			}
		}
		Rectangle ISupportContextItems.DrawBounds {
			get {
				Rectangle res = (ViewInfo as ContextButtonsPreviewControlViewInfo).ImageBounds;
				return res;
			}
		}
		Rectangle ISupportContextItems.ActivationBounds {
			get { return ViewInfo.ContentRect; }
		}
		ContextItemCollection ISupportContextItems.ContextItems {
			get { return ContextItems; }
		}
		bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
		Control ISupportContextItems.Control {
			get { return this; }
		}
		bool ISupportContextItems.DesignMode { get { return false; } }
		bool ISupportContextItems.CloneItems {
			get { return false; }
		}
		void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) { }
		void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) { }
		void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) { }
		Utils.Drawing.ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
			return Utils.Drawing.ItemHorizontalAlignment.Left;
		}
		Utils.Drawing.ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
			return Utils.Drawing.ItemVerticalAlignment.Center;
		}
		Utils.Drawing.ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
			return Utils.Drawing.ItemHorizontalAlignment.Left;
		}
		Utils.Drawing.ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
			return Utils.Drawing.ItemLocation.Left;
		}
		int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
			return Options.Indent;
		}
		Utils.Drawing.ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
			return Utils.Drawing.ItemVerticalAlignment.Center;
		}
		LookAndFeel.UserLookAndFeel ISupportContextItems.LookAndFeel {
			get { return LookAndFeel.ActiveLookAndFeel; }
		}
		ContextItemCollectionOptions ISupportContextItems.Options {
			get { return Options; }
		}
		void ISupportContextItems.Redraw() {
			Invalidate();
		}
		void ISupportContextItems.Redraw(Rectangle rect) {
			Invalidate(rect);
		}
		void ISupportContextItems.Update() {
			Update();
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			LayoutChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		bool IContextItemCollectionOwner.IsDesignMode { get { return false; } }
		bool IContextItemCollectionOwner.IsRightToLeft {
			get {
				return UseRightToLeft;
			}
		}
		protected internal bool UseRightToLeft { get; set; }
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if(Parent != null)
				Parent.Resize += Parent_Resize;
		}
		void Parent_Resize(object sender, EventArgs e) {
			UpdatePreviewBounds();
		}
		private void UpdatePreviewBounds() {
			Location = new Point(
					Parent.ClientRectangle.X + (Parent.ClientRectangle.Width - Location.X) / 2,
					Parent.ClientRectangle.Y + (Parent.ClientRectangle.Height - Location.Y) / 2
				);
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(Parent != null) {
				x = Parent.ClientRectangle.X + (Parent.ClientRectangle.Width - width) / 2;
				y = Parent.ClientRectangle.Y + (Parent.ClientRectangle.Height - height) / 2;
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
}
