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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Native;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.Utils.Serializing;
using DevExpress.XtraGauges.Core;
using DevExpress.XtraGauges.Core.Styles;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	XRToolboxSubcategoryAttribute(2, 1),
	ToolboxTabName(AssemblyInfo.DXTabReportControls),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "XRGauge.bmp"),
	Designer("DevExpress.XtraReports.Design._XRGaugeDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	XRDesigner("DevExpress.XtraReports.Design.XRGaugeDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRGauge", "Gauge"),
	DefaultProperty("ViewType"),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRGauge.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRGauge.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRGauge : XRControl {
		Utils.IoC.IntegrityContainer gaugesIntegrityContainer = new Utils.IoC.IntegrityContainer();
		public XRGauge() {
			gaugesIntegrityContainer.RegisterType<IStyleResourceProvider, StyleResourceProvider>();
			gaugesIntegrityContainer.RegisterType<IThemeNameResolutionService, ThemeNameResolutionService>();
		}
		const DashboardGaugeType defaultViewType = DashboardGaugeType.Circular;
		const DashboardGaugeStyle defaultViewStyle = DashboardGaugeStyle.Half;
		const DashboardGaugeTheme defaultViewTheme = DashboardGaugeTheme.FlatLight;
		IDashboardGauge gauge;
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IDashboardGauge Gauge {
			get {
				if(gauge == null)
					gauge = CreateGauge();
				return gauge;
			}
		}
		void InitializeGauge(IDashboardGauge gauge) {
			if(Minimum.HasValue)
				gauge.MinValue = Minimum.Value;
			if(Maximum.HasValue)
				gauge.MaxValue = Maximum.Value;
			if(ActualValue.HasValue)
				gauge.Value = ActualValue.Value;
			if(TargetValue.HasValue)
				gauge.MarkerValue = TargetValue.Value;
		}
		protected virtual IDashboardGauge CreateGauge() {
			return DashboardGaugeCreator.CreateGauge(ViewType, ViewStyle, ViewTheme, TargetValue.HasValue, gaugesIntegrityContainer);
		}
		protected virtual void ClearGauge() {
			if(gauge is IDisposable)
				((IDisposable)gauge).Dispose();
			gauge = null;
		}
		float? minimum;
		float? maximum;
		float? actualValue;
		float? targetValue;
		DashboardGaugeType viewType = defaultViewType;
		DashboardGaugeStyle viewStyle = defaultViewStyle;
		DashboardGaugeTheme viewTheme = defaultViewTheme;
		GaugeImageType imageType = GaugeImageType.Metafile;
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.ViewTheme"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(defaultViewTheme),
		XtraSerializableProperty,
		]
		public DashboardGaugeTheme ViewTheme {
			get { return viewTheme; }
			set {
				if(viewTheme != value) {
					viewTheme = value;
					ClearGauge();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.ViewStyle"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(defaultViewStyle),
		XtraSerializableProperty,
		TypeConverter(typeof(DevExpress.XtraReports.Design.DashboardGaugeStyleConverter)),
		]
		public DashboardGaugeStyle ViewStyle {
			get { return viewStyle.ValidateByType(ViewType); }
			set {
				if(ViewStyle != value) {
					viewStyle = value;
					ClearGauge();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.ViewType"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(defaultViewType),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All),
		]
		public DashboardGaugeType ViewType {
			get { return viewType; }
			set {
				if(viewType != value) {
					viewType = value;
					ClearGauge();
				}
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.TargetValue"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		XtraSerializableProperty,
		Bindable(true),
		]
		public float? TargetValue {
			get { return targetValue; }
			set {
				if(targetValue.HasValue != value.HasValue)
					ClearGauge();
				targetValue = value;
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.ActualValue"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		XtraSerializableProperty,
		Bindable(true),
		]
		public float? ActualValue {
			get { return actualValue; }
			set { actualValue = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.Minimum"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		XtraSerializableProperty,
		Bindable(true),
		]
		public float? Minimum {
			get { return minimum; }
			set { minimum = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.Maximum"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(null),
		XtraSerializableProperty,
		Bindable(true),
		]
		public float? Maximum {
			get { return maximum; }
			set { maximum = value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRGauge.ImageType"),
		DefaultValue(GaugeImageType.Metafile),
		SRCategory(ReportStringId.CatAppearance),
		XtraSerializableProperty,
		]
		public GaugeImageType ImageType {
			get { return imageType; }
			set { imageType = value; }
		}
		#region hidden properties
		[
		Bindable(false),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string XlsxFormatString {
			get { return base.XlsxFormatString; }
			set { base.XlsxFormatString = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		protected override XRControlScripts CreateScripts() {
			return new XRGaugeScripts(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				ClearGauge();
			}
			base.Dispose(disposing);
		}
		protected internal override int DefaultWidth {
			get { return DefaultSizes.Gauge.Width; }
		}
		protected internal override int DefaultHeight {
			get { return DefaultSizes.Gauge.Height; }
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ImageBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			ImageBrick imageBrick = (ImageBrick)brick;
			imageBrick.Padding = PaddingInfo.Empty;
			imageBrick.SizeMode = ImageSizeMode.Normal;
			RectangleF rect = DeflateBorderWidth(ClientRectangleF);
			rect = XRConvert.Convert(rect, Dpi, GraphicsDpi.Pixel);
			Rectangle bounds = new Rectangle(Point.Empty, Size.Round(rect.Size));
			imageBrick.Image = GetImage(bounds);
		}
		Image GetImage(Rectangle bounds) {
			if(bounds.Width == 0 || bounds.Height == 0)
				return null;
			Image image = (imageType == GaugeImageType.Metafile) ? CreateMetafile(bounds) : CreateBitmap(bounds);
			if(image != null)
				using(Graphics graphics = Graphics.FromImage(image)) {
					InitializeGauge(Gauge);
					Gauge.Bounds = Rectangle.Round(Padding.Deflate(bounds, GraphicsDpi.Pixel));
					Gauge.Render(graphics);
				}
			return image;
		}
		static Image CreateMetafile(Rectangle bounds) {
			using(Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero)) {
				return graphics.CreateMetafile(bounds, MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			}
		}
		static Image CreateBitmap(Rectangle bounds) {
			Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
			bmp.SetResolution(GraphicsDpi.DeviceIndependentPixel, GraphicsDpi.DeviceIndependentPixel);
			return bmp;
		}
	}
}
namespace DevExpress.XtraReports.Native {
	using DevExpress.XtraGauges.Core.Styles;
	using System.Reflection;
	class ThemeNameResolutionService : IThemeNameResolutionService {
		string IThemeNameResolutionService.Resolve(string themeName) {
			if(themeName == "Style27")
				return "Style27";
			if(themeName == "Style28")
				return "Style28";
			if(themeName == "FlatLight")
				return "Style27";
			if(themeName == "FlatDark")
				return "Style28";
			return null;
		}
	}
	class StyleResourceProvider : IStyleResourceProvider {
		Assembly IStyleResourceProvider.GetAssembly() {
			return typeof(XtraReports.UI.XtraReport).Assembly;
		}
		string IStyleResourceProvider.GetPathPrefix() {
			return "DevExpress.XtraReports.Resources.GaugesPresets.";
		}
		string IStyleResourceProvider.GetPathSuffix() {
			return ".preset";
		}
	}
	public static class DashboardGaugeStyleExtension {
		const DashboardGaugeStyle defaultCircularStyle = DashboardGaugeStyle.Half;
		const DashboardGaugeStyle defaultLinearStyle = DashboardGaugeStyle.Horizontal;
		public static DashboardGaugeStyle[] LinearStyles = new DashboardGaugeStyle[] { DashboardGaugeStyle.Horizontal, DashboardGaugeStyle.Vertical };
		public static DashboardGaugeStyle[] CircularStyles = new DashboardGaugeStyle[] { DashboardGaugeStyle.Full, DashboardGaugeStyle.Half, DashboardGaugeStyle.QuarterLeft, DashboardGaugeStyle.QuarterRight, DashboardGaugeStyle.ThreeFourth };
		public static DashboardGaugeStyle ValidateByType(this DashboardGaugeStyle style, DashboardGaugeType type) {
			if(type == DashboardGaugeType.Linear && !LinearStyles.Contains<DashboardGaugeStyle>(style))
				return defaultLinearStyle;
			if(type == DashboardGaugeType.Circular && !CircularStyles.Contains<DashboardGaugeStyle>(style))
				return defaultCircularStyle;
			return style;
		}
	}
}
