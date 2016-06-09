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
using DevExpress.XtraReports.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls), 
	XRDesigner("DevExpress.XtraReports.Design.XRPageBreakDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRPageBreakDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultProperty(XRComponentPropertyNames.Location),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRPageBreak.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRPageBreak", "PageBreak"),
	XRToolboxSubcategoryAttribute(3, 2),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRPageBreak.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRPageBreak.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRPageBreak : XRControl {
		#region fields & properties
		protected override bool CanHaveExportWarning { get { return false; } }
		protected internal override bool HasUndefinedBounds { get { return true; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanPublish { get { return true; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override VerticalAnchorStyles AnchorVertical { get { return VerticalAnchorStyles.None; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override HorizontalAnchorStyles AnchorHorizontal { get { return HorizontalAnchorStyles.None; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRPageBreak.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new XRPageBreakScripts Scripts { get { return (XRPageBreakScripts)fEventScripts; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakFont"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font { get { return base.Font; } set { base.Font = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakForeColor"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakBackColor"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakBorderColor"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Color BorderColor { get { return base.BorderColor; } set { base.BorderColor = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakBorderWidth"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new float BorderWidth { get { return base.BorderWidth; } set { base.BorderWidth = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool KeepTogether { get { return false; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override SizeF SizeF { get { return base.SizeF; } set { base.SizeF = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakText"),
#endif
		Bindable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakTextAlignment"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment { get { return base.TextAlignment; } set { base.TextAlignment = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakWordWrap"),
#endif
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string NavigateUrl { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Target { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Bookmark { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override XRControl BookmarkParent { get { return null; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakStyles"),
#endif
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRControlStyles Styles { get { return base.Styles; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string StyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakEvenStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string EvenStyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakOddStyleName"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string OddStyleName { get { return ""; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRPageBreakDataBindings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings { get { return base.DataBindings; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PaddingInfo Padding {
			get { return base.Padding; }
			set { base.Padding = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRPageBreakBoundsF")]
#endif
		public override RectangleF BoundsF {
			get {
				RectangleF rect = base.BoundsF;
				if(RootReport != null) {
					MultiColumn mc;
					rect.Width = Band.TryGetMultiColumn(Band, out mc) ?
						mc.GetUsefulColumnWidth(Band.ClientRectangleF.Width, Band.Dpi) :
						RootReport.GetClientSize().Width;
				}
				return rect;
			}
			set {
				base.BoundsF = value;
			}
		}
		protected internal override bool CanDrawBackground { get { return false; } }
		protected internal override bool IsNavigateTarget { get { return false; } }
		protected internal override int DefaultWidth {
			get { return DefaultSizes.PageBreak.Width; }
		}
		protected internal override int DefaultHeight {
			get { return DefaultSizes.PageBreak.Height; }
		}
		#endregion
		public XRPageBreak()
			: base() {
		}
		protected override XRControlScripts CreateScripts() {
			return new XRPageBreakScripts(this); 
		}
		protected internal override bool HasPrintingWarning() {
			return false;
		}
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event BindingEventHandler EvaluateBinding { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PrintOnPageEventHandler PrintOnPage { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseMove { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseDown { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseUp { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewDoubleClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event DrawEventHandler Draw { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler SizeChanged { add { } remove { } }
		#endregion
		protected override void WriteContentToCore(XRWriteInfo writeInfo, VisualBrick brick) {
			writeInfo.InsertPageBreak(VisualBrickHelper.GetBrickInitialRect(brick).Top);
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			specified &= ~BoundsSpecified.Size;
			base.SetBounds(0, y, width, height, specified);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new LineBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			LineBrick lineBrick = (LineBrick)brick;
			lineBrick.LineWidth = 1;
			lineBrick.LineStyle = DashStyle.Dash;
			lineBrick.LineDirection = LineDirection.Horizontal;
			lineBrick.ForeColor = Color.Black;
		}
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
	}
}
