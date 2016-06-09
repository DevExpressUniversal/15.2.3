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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.XtraFrames {
	[ToolboxItem(false)]
	public class CaptionPanel : Panel {
		const int CaptionToProductInfoInterval = 25;
		const int AssemblyToLicenseInterval = 10;
		static Color DefaultDividerColor = Color.FromArgb(202, 202, 202);
		public CaptionPanel()
			: base() {
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
			DividerColor = DefaultDividerColor;
		}
		XtraDesignForm OwnerXtraDesignForm { get { return FindForm() as XtraDesignForm; } }
		protected Rectangle AssemblyVersionBounds { get; set; }
		protected Rectangle LicenseImageBounds { get; set; }
		protected string AssemblyName { get { return OwnerXtraDesignForm.AssemblyName; } }
		protected Rectangle TextBounds { get; set; }
		protected internal virtual Image LicenseImage {
			get {
				if(OwnerXtraDesignForm.ProductInfo == null || OwnerXtraDesignForm.IsCustom) return null;
				if(!OwnerXtraDesignForm.IsTrial) return ImageResourceLoader.GetImage("Licensed");
				if(OwnerXtraDesignForm.IsExpired) return ImageResourceLoader.GetImage("Expired");
				return ImageResourceLoader.GetImage("Trial");
			}
		}
		AppearanceObject appearanceProductInfoCore;
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceProductInfo {
			get {
				if(appearanceProductInfoCore == null)
					appearanceProductInfoCore = CreateAppearanceProductInfo();
				return appearanceProductInfoCore;
			}
		}
		bool ShouldSerializeAppearanceProductInfo() { return false; }
		void ResetAppearanceProductInfo() { AppearanceProductInfo.Reset(); }
		AppearanceObject appearanceCaptionCore;
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject AppearanceCaption {
			get {
				if(appearanceCaptionCore == null)
					appearanceCaptionCore = CreateAppearanceCaption();
				return appearanceCaptionCore;
			}
		}
		bool ShouldSerializeAppearanceCaption() { return false; }
		void ResetAppearanceCaption() { AppearanceCaption.Reset(); }
		protected virtual AppearanceObject CreateAppearanceProductInfo() {
			FrozenAppearance appearance = new FrozenAppearance();
			appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			appearance.ForeColor = Color.FromArgb(136, 136, 136);
			appearance.Font = new Font("Segoe UI", 11, GraphicsUnit.Pixel);
			return appearance;
		}
		protected virtual AppearanceObject CreateAppearanceCaption() {
			FrozenAppearance appearance = new FrozenAppearance();
			appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			appearance.Font = new Font("Segoe UI Light", 30, GraphicsUnit.Pixel);
			return appearance;
		}
		[Obsolete("Use DividerColor"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Color DeviderColor { get { return DividerColor; } set { DividerColor = value; } }
		public Color DividerColor { get; set; }
		bool ShouldSerializeDividerColor() {
			return DividerColor != DefaultDividerColor;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			if(OwnerXtraDesignForm != null) {
				CalcCaptionElements(e.Graphics, CalcClientRectangle(ClientRectangle));
				using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
					DrawCaption(cache);
				}
			}
		}
		protected void DrawCaption(GraphicsCache cache) {
			DrawText(cache);
			if(!string.IsNullOrEmpty(AssemblyName)) {
				DrawDivider(cache);
				DrawAssemblyVersion(cache);
				DrawLicenseImage(cache);
			}
		}
		protected virtual void DrawText(GraphicsCache cache) {
			AppearanceCaption.DrawString(cache, Text, TextBounds);
		}
		protected virtual void DrawAssemblyVersion(GraphicsCache cache) {
			AppearanceProductInfo.DrawString(cache, AssemblyName, AssemblyVersionBounds);
		}
		protected virtual void DrawLicenseImage(GraphicsCache cache) {
			if(LicenseImage != null)
				cache.Graphics.DrawImage(LicenseImage, LicenseImageBounds);
		}
		protected virtual void DrawDivider(GraphicsCache cache) {
			if(AssemblyVersionBounds.Width > 0)
				cache.Graphics.DrawLine(new Pen(DividerColor),
					new Point(AssemblyVersionBounds.Left - CaptionToProductInfoInterval / 2, TextBounds.Top + 1),
					new Point(AssemblyVersionBounds.Left - CaptionToProductInfoInterval / 2, TextBounds.Bottom - 1));
		}
		bool isReadyCore;
		protected bool IsReady { get { return isReadyCore; } }
		protected internal void SetDirty() {
			isReadyCore = false;
		}
		protected virtual void CalcCaptionElements(Graphics graphics, Rectangle clientBounds) {
			if(IsReady) return;
			isReadyCore = true;
			Text = Parent.Text;
			Size textSize = Size.Round(AppearanceCaption.CalcTextSize(graphics, Text, 0));
			TextBounds = new Rectangle(ClientRectangle.Location, textSize);
			Size productInfoSize = Size.Round(AppearanceProductInfo.CalcTextSize(graphics, AssemblyName, 0));
			int assemblyVersionWidth = productInfoSize.Width < clientBounds.Right - TextBounds.Right - CaptionToProductInfoInterval ?
				productInfoSize.Width : clientBounds.Right - TextBounds.Right - CaptionToProductInfoInterval;
			if(!OwnerXtraDesignForm.ShowAssemblyVersion)
				assemblyVersionWidth = 0;
			AssemblyVersionBounds = new Rectangle(TextBounds.Right + CaptionToProductInfoInterval, TextBounds.Top + (TextBounds.Height - productInfoSize.Height) / 2  + 3,
				assemblyVersionWidth, productInfoSize.Height);
			Size imageSize = LicenseImage != null ? LicenseImage.Size : Size.Empty;
			if(clientBounds.Right - AssemblyVersionBounds.Right - AssemblyToLicenseInterval >= imageSize.Width)
				LicenseImageBounds = new Rectangle(AssemblyVersionBounds.Right + AssemblyToLicenseInterval, TextBounds.Top + (TextBounds.Height - imageSize.Height) / 2  + 3, imageSize.Width, imageSize.Height);
			else
				LicenseImageBounds = Rectangle.Empty;
		}
		protected Rectangle CalcClientRectangle(Rectangle bounds) {
			return new Rectangle(
				bounds.Left + Padding.Left,
				bounds.Top + Padding.Top,
				bounds.Width - Padding.Horizontal,
				bounds.Height - Padding.Vertical);
		}
		protected override void CreateHandle() {
			base.CreateHandle();
			SendToBack();
		}
		bool IsDesignMode { get { return Site != null && Site.DesignMode; } }
		protected override void WndProc(ref Message m) {
			if(m.Msg == MSG.WM_NCHITTEST && !IsDesignMode)
				m.Result = new IntPtr(NativeMethods.HT.HTTRANSPARENT);
			else
				base.WndProc(ref m);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Invalidate();
			SetDirty();
		}
	}
	static class ImageResourceLoader {
		static IDictionary<object, Image> images;
		static ImageResourceLoader() {
			images = new Dictionary<object, Image>();
			images.Add("Trial", LoadImageFromResources("Trial"));
			images.Add("Expired", LoadImageFromResources("Expired"));
			images.Add("Licensed", LoadImageFromResources("Licensed"));
		}
		static Image LoadImageFromResources(string name) {
			return ResourceImageHelper.CreateImageFromResources(
				string.Format("DevExpress.Utils.Design.{0}.png", name), typeof(ImageResourceLoader).Assembly);
		}
		public static Image GetImage(object key) {
			Image img;
			return images.TryGetValue(key, out img) ? img : null;
		}
	}
}
