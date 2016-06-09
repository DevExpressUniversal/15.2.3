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
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using System.Collections;
using DevExpress.Utils;
using DevExpress.XtraReports.Native.CrossBandControls;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	XRDesigner("DevExpress.XtraReports.Design.XRCrossBandBoxDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRCrossBandBoxDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultProperty("Borders"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRCrossBandBox.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRCrossBandBox", "CrossBandBox"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRToolboxSubcategoryAttribute(3, 4),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRCrossBandBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRCrossBandBox.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRCrossBandBox : XRCrossBandControl {
		#region hiden properties
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override Color ForeColor { get { return Color.Transparent; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PaddingInfo Padding { get { return PaddingInfo.Empty; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override Color BackColor { get { return Color.Transparent; } set { } }
		#endregion
		#region fields & properties
		const int widthMultiplier = 3;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandBoxBorders"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(BorderSide.All)
		]
		public override BorderSide Borders { 
			get { return base.Borders; }
			set { base.Borders = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandBoxBorderWidth"),
#endif
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(2)
		]
		public override float BorderWidth {  
			get { return base.BorderWidth; }
			set { base.BorderWidth = Math.Max(1f, value); }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRCrossBandBoxBorderDashStyle")]
#endif
		[TypeConverter(typeof(DevExpress.XtraReports.Design.XRCrossBandBoxBorderDashStyleConverter))]
		public override BorderDashStyle BorderDashStyle {
			get { return base.BorderDashStyle; }
			set { base.BorderDashStyle = value; }
		}
		#endregion
		public XRCrossBandBox()
			: base() {
			BorderWidth = 2;
			Borders = BorderSide.All;
		}
		internal override XRControl[] GetPrintableControlsForce(Band band) {
			Dictionary<BorderSide, WrappedXRLine> controls = new Dictionary<BorderSide, WrappedXRLine>();
			controls[BorderSide.Right] = GetXRControl(band, BorderSide.Right);
			controls[BorderSide.Top] =  GetXRControl(band,  BorderSide.Top);
			controls[BorderSide.Left] = GetXRControl(band, BorderSide.Left);
			controls[BorderSide.Bottom] = GetXRControl(band, BorderSide.Bottom);
			SetControlsAnchors(band, controls);
			SetControlsBounds(band, controls);
			return BorderCollector.CreateInstance(this, DesignMode).GetBorderControls(controls, band);
		}
		WrappedXRLine GetXRControl(Band band, BorderSide border) {
			MultiKey key = new MultiKey(new object[] { band, border });
			WrappedXRLine control = (WrappedXRLine)base.GetXRControl(band, key);
			control.Dpi = this.Dpi;
			LineDirection lineDirection = LineDirection.Vertical;
			if(border == BorderSide.Top || border == BorderSide.Bottom)
				lineDirection = LineDirection.Horizontal;
			float dpiBorderWidth = XRConvert.Convert(this.BorderWidth, GraphicsDpi.Pixel, this.Dpi);
			Color lineColor = DesignMode ? Color.Transparent : this.GetEffectiveBorderColor();
			BorderDashStyle borderDashStyle = this.GetEffectiveBorderDashStyle();
			if(StartBand != EndBand && band == EndBand && border == BorderSide.Bottom) {
				control.UpdateView(VisualBrick.ConvertDashStyle(borderDashStyle), lineDirection, Math.Min(dpiBorderWidth, EndPointFloat.Y), lineColor);
			} else {
				control.UpdateView(VisualBrick.ConvertDashStyle(borderDashStyle), lineDirection, dpiBorderWidth, lineColor);
			}
			return control;
		}
		protected override XRControl CreateXRControl(Band band) {
			return new WrappedXRLine(this, band);
		}
		public override BorderDashStyle GetEffectiveBorderDashStyle() {
			BorderDashStyle dashStyle = base.GetEffectiveBorderDashStyle();
			return dashStyle == BorderDashStyle.Double ? BorderDashStyle.Solid : dashStyle;
		}
		internal protected override int GetMinimumHeight() {
			int width = (this.StartBand == this.EndBand) ? (int)Math.Ceiling(this.BorderWidth * widthMultiplier) : (int)Math.Ceiling(this.BorderWidth);
			return Math.Max(base.GetMinimumHeight(), XRConvert.Convert(width, GraphicsDpi.Pixel, this.Dpi));
		}
		protected override int GetMinimumWidth() {
			return (int)Math.Ceiling(Math.Max(base.GetMinimumWidth(), XRConvert.Convert(this.BorderWidth * widthMultiplier, GraphicsDpi.Pixel, this.Dpi)));
		}
		void SetControlsBounds(Band band, Dictionary<BorderSide, WrappedXRLine> controls) {
			RectangleF rect = GetBounds(band);
			float borderWidth = XRConvert.Convert(this.BorderWidth, GraphicsDpi.Pixel, this.Dpi);
			RectangleF leftRect = new RectangleF(rect.Left, rect.Top, borderWidth, rect.Height);
			RectangleF topRect = new RectangleF(leftRect.Right, leftRect.Top, rect.Width - 2 * borderWidth, borderWidth);
			RectangleF bottomRect = new RectangleF(leftRect.Right, leftRect.Bottom - borderWidth, topRect.Width, topRect.Height);
			RectangleF rightRect = new RectangleF(topRect.Right, topRect.Top, leftRect.Width, rect.Height);
			if(bottomRect.Location.Y < 0) {
				bottomRect.Height += bottomRect.Location.Y;
				bottomRect.Y = 0;
			}
			controls[BorderSide.Left].BoundsF = leftRect;
			controls[BorderSide.Top].BoundsF = topRect;
			controls[BorderSide.Bottom].BoundsF = bottomRect;
			controls[BorderSide.Right].BoundsF = rightRect;
		}
		void SetControlsAnchors(Band band, Dictionary<BorderSide, WrappedXRLine> controls) {
			SetVerticalLineAnchor(band, controls[BorderSide.Right]);
			SetHorizontalTopAnchor(band, controls[BorderSide.Top]);
			controls[BorderSide.Left].AnchorVertical = controls[BorderSide.Right].AnchorVertical;
			SetHorizontalBottomAnchor(band, controls[BorderSide.Bottom]);
		}
		internal override RectangleF GetBounds(Band band) {
			float top = band == this.StartBand ? this.StartPointF.Y : 0;
			float bottom = band == this.EndBand ? this.EndPointF.Y : band.BottomF;
			return RectangleF.FromLTRB(this.StartPointF.X, top, this.StartPointF.X + this.WidthF, bottom);
		}
		void SetHorizontalTopAnchor(Band band, XRControl control) {
			control.AnchorVertical = this.AnchorVertical == VerticalAnchorStyles.Bottom ? VerticalAnchorStyles.Bottom : VerticalAnchorStyles.Top;
			if(band == this.StartBand && band != this.EndBand && this.AnchorVertical == VerticalAnchorStyles.None) {
				control.AnchorVertical = VerticalAnchorStyles.Bottom;
			}
		}
		void SetHorizontalBottomAnchor(Band band, XRControl control) {
			control.AnchorVertical = VerticalAnchorStyles.Top;
			if(this.AnchorVertical == VerticalAnchorStyles.Bottom || this.AnchorVertical == VerticalAnchorStyles.Both) {
				control.AnchorVertical = VerticalAnchorStyles.Bottom;
			}
		}
	}
}
