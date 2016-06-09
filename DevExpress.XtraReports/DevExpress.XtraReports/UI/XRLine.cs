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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using System.IO;
using System.Reflection;
using System.Net;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export;
using System.Collections.Specialized;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls), 
	XRDesigner("DevExpress.XtraReports.Design.XRLineDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRLineDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultProperty("LineWidth"),	
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRLine.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRLine", "Line"),
	XRToolboxSubcategoryAttribute(1, 0),
	DefaultBindableProperty(null),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRLine.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRLine.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRLine : XRControl {
		#region fields & properties
		private LineDirection lineDirection = LineDirection.Horizontal;
		private DashStyle lineStyle = DashStyle.Solid;
		protected float fLineWidth = 1;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLine.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XRLineScripts Scripts { get { return (XRLineScripts)fEventScripts; } }
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineFont"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineText"),
#endif
		Bindable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineTextAlignment"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineWordWrap"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineLineWidth"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLine.LineWidth"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(1),
		XtraSerializableProperty,
		]
		public int LineWidth {
			get { return (int)Math.Round(fLineWidth); }
			set { fLineWidth = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineLineDirection"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLine.LineDirection"),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(LineDirection.Horizontal),
		XtraSerializableProperty,
		]
		public LineDirection LineDirection {
			get { return lineDirection; }
			set { lineDirection = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineLineStyle"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRLine.LineStyle"),
		TypeConverter(typeof(DevExpress.Utils.Design.DashStyleTypeConverter)),
		SRCategory(ReportStringId.CatAppearance),
		DefaultValue(DashStyle.Solid),
		XtraSerializableProperty,
		]
		public DashStyle LineStyle {
			get { return lineStyle; }
			set { lineStyle = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string NavigateUrl {
			get { return ""; }
			set { ;}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Target {
			get { return ""; }
			set { ;}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Bindable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Bookmark {
			get { return ""; }
			set { ; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override XRControl BookmarkParent {
			get { return null; }
			set { ; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRLineDataBindings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings {
			get { return base.DataBindings; }
		}
		#endregion
		#region events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRLine()
			: base() {
			fLineWidth = Convert.ToInt32(XRConvert.Convert(1, GraphicsDpi.Pixel, Dpi));
		}
		protected override XRControlScripts CreateScripts() {
			return new XRLineScripts(this);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeEnum("LineDirection", lineDirection);
			serializer.SerializeEnum("LineStyle", lineStyle);
			serializer.SerializeInteger("LineWidth", (int)fLineWidth);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			lineDirection = (LineDirection)serializer.DeserializeEnum("LineDirection", typeof(LineDirection), LineDirection.Horizontal);
			lineStyle = (DashStyle)serializer.DeserializeEnum("LineStyle", typeof(DashStyle), DashStyle.Solid);
			fLineWidth = serializer.DeserializeInteger("LineWidth", XRConvert.Convert(1, GraphicsDpi.Pixel, Dpi));
		}
		#endregion
		protected internal override void SyncDpi(float dpi) {
			if(dpi != Dpi)
				fLineWidth = XRConvert.Convert(fLineWidth, Dpi, dpi);
			base.SyncDpi(dpi);
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			if (Site == null)
				base.SetBounds(x, y, width, height, specified);
			else
				base.SetBounds(x, y, Math.Max(width, fLineWidth), Math.Max(height, fLineWidth), specified);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new LineBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			LineBrick lineBrick = (LineBrick)brick;
			lineBrick.LineWidth = GetLineWidth();
			lineBrick.LineStyle = LineStyle;
			switch(LineDirection) {
				case LineDirection.Horizontal:
					lineBrick.HtmlLineDirection = HtmlLineDirection.Horizontal;
					break;
				case LineDirection.Vertical:
					lineBrick.HtmlLineDirection = HtmlLineDirection.Vertical;
					break;
			}
			lineBrick.LineDirection = this.LineDirection;
		}
		protected virtual float GetLineWidth() {
			return Math.Max(1, XRConvert.Convert(LineWidth, Dpi, GraphicsDpi.Pixel)); 
		}
	}
}
