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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using DevExpress.Accessibility;
using DevExpress.Skins;
using DevExpress.Skins.Info;
using DevExpress.Skins.XtraForm;
using DevExpress.Utils.About;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.XtraFrames;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel.DesignService;
using System.ComponentModel.Design;
namespace DevExpress.Utils.Design {
	[ToolboxItem(false)]
	public partial class XtraDesignForm : XtraForm {
		ProductInfo productInfoCore;
		Assembly assemblyCore;
		Utils.Paint.DXDashStyle savedFocusRectStyle;
		public XtraDesignForm() {
			InitStyle();
			WindowsFormsDesignTimeSettings.ApplyDesignSettings(this);
			InitializeComponent();
			ShowAssemblyVersion = true;
			AllowCaptionBorder = true;
			savedFocusRectStyle = DevExpress.Utils.Paint.XPaint.FocusRectStyle;
			DevExpress.Utils.Paint.XPaint.FocusRectStyle = Utils.Paint.DXDashStyle.None;
			FormBorderEffect = XtraEditors.FormBorderEffect.Default;
			ShowIcon = false;
			AttachMenuManager();
		}
		public XtraDesignForm(ProductInfo info)
			: this() {
			productInfoCore = info;
		}
		void AttachMenuManager() {
			try {
				var barsAssembly = AssemblyHelper.GetLoadedAssembly(AssemblyInfo.SRAssemblyBars) ??
					AssemblyHelper.LoadDXAssembly(AssemblyInfo.SRAssemblyBars);
				if(barsAssembly == null) return;
				Type menuManagerCreatorType = barsAssembly.GetType("DevExpress.XtraBars.MenuManagerCreator");
				if(menuManagerCreatorType != null) {
					MethodInfo createMenuManagerMethod = menuManagerCreatorType.GetMethod("CreateMenuManager");
					if(createMenuManagerMethod != null)
						createMenuManagerMethod.Invoke(null, new object[] { this });
				}
			}
			catch { }
		}
		[DefaultValue(true)]
		public virtual bool ShowAssemblyVersion { get; set; }
		[DefaultValue(true)]
		public bool AllowCaptionBorder { get; set; }
		[DefaultValue(null)]
		public ProductInfo ProductInfo {
			get { return productInfoCore; }
			set { productInfoCore = value; }
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Invalidate();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			DevExpress.Utils.Paint.XPaint.FocusRectStyle = savedFocusRectStyle;
		}
		protected override FormPainter CreateFormBorderPainter() {
			return new XtraDesignFormPainter(this, LookAndFeel);
		}
		protected internal virtual string Serial { get { return AboutHelper.GetSerial(ProductInfo); } }
		protected internal bool IsTrial { get { return Serial == AboutHelper.TrialVersion; } }
		protected internal bool IsCustom { get { return Serial == AboutHelper.CustomVersion; } }
		protected internal bool IsExpired {
			get {
				bool res = false;
				return res;
			}
		}
		protected internal string GetVersionLabel() {
			if(IsExpired) return AccLocalizer.Active.GetLocalizedString(AccStringId.AboutExpiredVersion);
			return AccLocalizer.Active.GetLocalizedString(AccStringId.AboutTrialVersion);
		}
		protected internal virtual string AssemblyName {
			get {
				if(assemblyCore == null)
					assemblyCore = this.GetType().Assembly;
				string result = assemblyCore.ToString();
				return result.Substring(0, result.IndexOf(", Culture"));
			}
		}
		protected void InitStyle() {
			SkinManager.EnableFormSkins();
			SkinBlobXmlCreator skinCreator = new SkinBlobXmlCreator("DevExpress Design",
				"DevExpress.Utils.Design.", typeof(XtraDesignForm).Assembly, null);
			SkinManager.Default.RegisterSkin(skinCreator);
		}
		void InitializeComponent() {
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(945, 469);
			this.Padding = new Padding(0, 0, 0, 14);
			this.Name = "XtraDesignForm";
			this.ResumeLayout(false);
		}
	}
	public class XtraDesignFormPainter : FormPainter {
		const int WidthZoomedDelta = 8;
		const int CaptionToProductInfoInterval = 25;
		const int AssemblyToLicenseInterval = 10;
		XtraDesignForm OwnerXtraDesignForm { get { return Owner as XtraDesignForm; } }
		protected Rectangle AssemblyVersionBounds { get; set; }
		protected Rectangle LicenseImageBounds { get; set; }
		protected string AssemblyName { get { return OwnerXtraDesignForm.AssemblyName; } }
		protected internal virtual Image LicenseImage {
			get {
				if(OwnerXtraDesignForm.ProductInfo == null || OwnerXtraDesignForm.IsCustom) return null;
				if(!OwnerXtraDesignForm.IsTrial) return ImageResourceLoader.GetImage("Licensed");
				if(OwnerXtraDesignForm.IsExpired) return ImageResourceLoader.GetImage("Expired");
				return ImageResourceLoader.GetImage("Trial");
			}
		}
		AppearanceObject appearanceProductInfoCore;
		public virtual AppearanceObject AppearanceProductInfo {
			get {
				if(appearanceProductInfoCore == null)
					appearanceProductInfoCore = CreateAppearanceProductInfo();
				return appearanceProductInfoCore;
			}
		}
		protected virtual AppearanceObject CreateAppearanceProductInfo() {
			FrozenAppearance appearance = new FrozenAppearance();
			appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			appearance.Font = new Font("Segoe UI", 11, GraphicsUnit.Pixel);
			Skin skin = CommonSkins.GetSkin(this);
			if(skin != null)
				appearance.ForeColor = DividerColor = CreateAppearanceForeColor(skin);
			return appearance;
		}
		protected virtual Color CreateAppearanceForeColor(Skin skin) {
			return skin.Colors.GetColor(CommonColors.DisabledText);
		}
		public XtraDesignFormPainter(Control owner, ISkinProvider provider) : base(owner, provider) { }
		protected override void CheckReady() {
			if(IsReady) return;
			IsReady = true;
			GInfo.AddGraphics(null);
			Size textSize = Size.Empty;
			try {
				TextBounds = Rectangle.Empty;
				if(IsDrawCaption && Owner is XtraDesignForm) {
					Buttons.Clear();
					AppearanceObject appearance = new AppearanceObject(GetDefaultAppearance());
					Buttons.CreateButtons(GetVisibleButtons());
					ShouldUseSmallButtons = false;
					textSize = CalcCaptionSize(GInfo.Graphics, appearance);
					Size buttons = Buttons.CalcSize(GInfo.Graphics, ButtonPainter);
					captionHeight = Math.Max(buttons.Height, Buttons.CalcMinSize(GInfo.Graphics, ButtonPainter).Height);
					captionHeight = Math.Max(captionHeight, textSize.Height);
					captionHeight = Math.Max(captionHeight, 0);
					captionHeight = ObjectPainter.CalcBoundsByClientRectangle(null, SkinElementPainter.Default, new SkinElementInfo(GetSkinCaption()), new Rectangle(0, 0, 10, captionHeight)).Height;
				}
				else {
					captionHeight = 0;
				}
				margins = GetMargins();
				zoomedMargins = null;
				if(!IsDrawCaption) return;
				Rectangle captionClient = GetCaptionClient(CaptionBounds);
				if(NativeMethods.IsZoomed(Handle)) {
					SkinPaddingEdges zoomed = GetZoomedMargins();
					captionClient.X += zoomed.Left;
					captionClient.Width -= zoomed.Width;
					captionClient.Width -= WidthZoomedDelta;
				}
				ShouldUseSmallButtons = IsXtraFormMaximized;
				Rectangle restBounds = Buttons.CalcBounds(null, ButtonPainter, captionClient);
				if(restBounds.Width > 0)
					this.TextBounds = new Rectangle(restBounds.Location, textSize);
				Size assemblyVersionSize = Size.Round(AppearanceProductInfo.CalcTextSize(GInfo.Graphics, AssemblyName, 0));
				int assemblyVersionWidth = assemblyVersionSize.Width < restBounds.Right - TextBounds.Right - CaptionToProductInfoInterval ?
					assemblyVersionSize.Width : restBounds.Right - TextBounds.Right - CaptionToProductInfoInterval;
				if(!OwnerXtraDesignForm.ShowAssemblyVersion)
					assemblyVersionWidth = 0;
				AssemblyVersionBounds = new Rectangle(TextBounds.Right + CaptionToProductInfoInterval, TextBounds.Top + (TextBounds.Height - assemblyVersionSize.Height) / 2 + 1,
					assemblyVersionWidth, assemblyVersionSize.Height);
				Size imageSize = LicenseImage != null ? LicenseImage.Size : Size.Empty;
				if(restBounds.Right - AssemblyVersionBounds.Right - AssemblyToLicenseInterval >= imageSize.Width)
					LicenseImageBounds = new Rectangle(AssemblyVersionBounds.Right + AssemblyToLicenseInterval, TextBounds.Top + (TextBounds.Height - imageSize.Height) / 2 + 2, imageSize.Width, imageSize.Height);
				else
					LicenseImageBounds = Rectangle.Empty;
				Buttons.CalcMdiBounds(null, ButtonPainter, GetMdiBarClient(MdiBarBounds));
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual Size CalcCaptionSize(Graphics graphics, AppearanceObject appearance) {
			if(AllowHtmlDraw) {
				if(string.IsNullOrEmpty(Text)) return appearance.CalcDefaultTextSize(graphics);
				return DevExpress.Utils.Text.StringPainter.Default.Calculate(graphics, appearance, null, Text, 0, null, HtmlContext).Bounds.Size;
			}
			return Size.Round(appearance.CalcTextSize(graphics, Text, 0));
		}
		protected override void DrawText(GraphicsCache cache) {
			base.DrawText(cache);
			if(!string.IsNullOrEmpty(AssemblyName)) {
				DrawDivider(cache);
				DrawAssemblyVersion(cache);
				OwnerXtraDesignForm.ResumeLayout(false);
				DrawLicenseImage(cache);
			}
		}
		protected virtual void DrawAssemblyVersion(GraphicsCache cache) {
			AppearanceProductInfo.DrawString(cache, AssemblyName, AssemblyVersionBounds);
		}
		protected virtual void DrawLicenseImage(GraphicsCache cache) {
			if(LicenseImage != null)
				cache.Graphics.DrawImage(LicenseImage, LicenseImageBounds);
		}
		Color DividerColor;
		protected virtual void DrawDivider(GraphicsCache cache) {
			if(AssemblyVersionBounds.Width > 0)
				cache.Graphics.DrawLine(new Pen(DividerColor),
					new Point(AssemblyVersionBounds.Left - CaptionToProductInfoInterval / 2, TextBounds.Top + 1),
					new Point(AssemblyVersionBounds.Left - CaptionToProductInfoInterval / 2, TextBounds.Bottom));
		}
		int BorderIndent {
			get {
				if(Owner is XtraDesignForm)
					return (Owner as XtraDesignForm).AllowCaptionBorder ? 1 : 0;
				return 0;
			}
		}
	}
	public class RegistryDesignerSkinHelper {
		const string Path = "Software\\Developer Express\\Designer\\";
		public static bool CanUseDefaultControlDesignersSkin {
			get {
				bool result = true;
				Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Path);
				if(key == null) return result;
				var value = key.GetValue("UseDefaultControlDesignersSkin");
				if(value != null)
					result = Boolean.Parse(value.ToString());
				if(key != null) key.Close();
				return result;
			}
		}
	}
	[ToolboxItem(false)]
	public class XtraDesignFormWithAdaptiveSkining : XtraDesignForm {
		public XtraDesignFormWithAdaptiveSkining() : base() { }
		public XtraDesignFormWithAdaptiveSkining(ProductInfo info)
			: this() {
			this.ProductInfo = info;
		}
		protected virtual void SetSkin() {
			ILookAndFeelService serv = null;
			serv = this.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			if(serv != null && !CanUseDefaultControlDesignersSkin())
				serv.InitializeRootLookAndFeel(this.LookAndFeel);
			else
				LookAndFeel.SetSkinStyle("DevExpress Design");
		}
		protected virtual bool CanUseDefaultControlDesignersSkin() {
			return RegistryDesignerSkinHelper.CanUseDefaultControlDesignersSkin;
		}
		protected override void OnLoad(EventArgs e) {
			SetSkin();
			base.OnLoad(e);
		}
		protected override FormPainter CreateFormBorderPainter() {
			return new XtraDesignFormWithAdaptiveSkiningPainter(this, LookAndFeel);
		}
		protected class XtraDesignFormWithAdaptiveSkiningPainter : XtraDesignFormPainter {
			public XtraDesignFormWithAdaptiveSkiningPainter(Control owner, ISkinProvider provider) : base(owner, provider) { }
			protected override Color CreateAppearanceForeColor(Skin skin) {
				return ControlPaint.LightLight(skin.Colors.GetColor(CommonColors.WindowText));
			}
		}
	}
}
