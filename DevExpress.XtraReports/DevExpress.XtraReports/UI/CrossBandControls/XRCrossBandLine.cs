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
using DevExpress.XtraReports.Native.CrossBandControls;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	XRDesigner("DevExpress.XtraReports.Design.XRCrossBandLineDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRCrossBandLineDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultProperty("LineStyle"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRCrossBandLine.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRCrossBandLine", "CrossBandLine"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRToolboxSubcategoryAttribute(3, 3),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRCrossBandLine.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRCrossBandLine.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRCrossBandLine : XRCrossBandControl {
		#region fields & properties
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override Color BackColor { get { return Color.Transparent; } set {} }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override Color BorderColor { get { return Color.Transparent; } set {} }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override BorderSide Borders { get { return BorderSide.None; } set {} }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override BorderDashStyle BorderDashStyle { get { return BorderDashStyle.Solid; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override float BorderWidth { get { return 0; } set {} }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PaddingInfo Padding { get { return PaddingInfo.Empty; } set {} }		
		private DashStyle lineStyle = DashStyle.Solid;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandLineLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCrossBandLine.LineStyle"),
		TypeConverter(typeof(DevExpress.Utils.Design.DashStyleTypeConverter)),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(DashStyle.Solid),
		XtraSerializableProperty,
		]
		public DashStyle LineStyle {
			get { return lineStyle; }
			set { lineStyle = value; }
		}
		#endregion
		public XRCrossBandLine() : base() {
		}
		internal override XRControl[] GetPrintableControlsForce(Band band) {
			WrappedXRLine control = (WrappedXRLine)GetXRControl(band, band);
			control.Dpi = this.Dpi;
			control.UpdateView(this.lineStyle, LineDirection.Vertical, this.WidthF, this.GetEffectiveForeColor());
			SetVerticalLineAnchor(band, control);
			if(band == this.StartBand && band == this.EndBand)
				control.SetBoundsCore(this.StartPointF, new SizeF(this.WidthF, Math.Abs(this.EndPointF.Y - this.StartPointF.Y)));
			else if(band == this.StartBand)
				control.SetBoundsCore(this.StartPointF, new SizeF(this.WidthF, Math.Max(0, band.HeightF - this.StartPointF.Y)));
			else if(band == this.EndBand)
				control.SetBoundsCore(this.EndPointF.X, 0, this.WidthF, this.EndPointF.Y);
			else
				control.SetBoundsCore(this.EndPointF.X, 0, this.WidthF, band.HeightF);
			return new XRControl[] { control };
		}
		protected override int GetMinimumWidth() {
			return XRConvert.Convert(1, GraphicsDpi.Pixel, this.Dpi);
		}
		protected override XRControl CreateXRControl(Band band) { 
			return new WrappedXRLine(this, band);
		}
	}
}
