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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using System.Drawing.Printing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.CrossBandControls;
using DevExpress.XtraReports.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItemFilter, ToolboxItemFilterType.Prevent),
	ToolboxItem(true),
	]
	public abstract class XRCrossBandControl : XRControl {
		#region fields & properties
		Band startBand;
		Band endBand;
		PointF startPoint;
		PointF endPoint;
		Dictionary<object, XRControl> controlDictionary;
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override FormattingRuleCollection FormattingRules { get { return base.FormattingRules; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRControlStyles Styles { get { return base.Styles; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool KeepTogether { get { return false; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandControlWidthF"),
#endif
		Browsable(true),
		SRCategory(ReportStringId.CatLayout),
		TypeConverter(typeof(SingleTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override float WidthF { get { return base.WidthF; } set { base.WidthF = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new Font Font { get { return base.Font; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new XRControlScripts Scripts { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Text { get { return string.Empty; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override TextAlignment TextAlignment { get { return base.TextAlignment; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool WordWrap { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string NavigateUrl { get { return string.Empty; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Target { get { return string.Empty; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Bookmark {
			get { return string.Empty; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRControl BookmarkParent { get { return null; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings { get { return base.DataBindings; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override SizeF SizeF { get { return base.SizeF; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override StylePriority StylePriority { get { return base.StylePriority; } }
		[
		Browsable(false),
		]
		public override PointFloat LocationFloat { get { return base.LocationFloat; } set { base.LocationFloat = value; } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override PointF LocationF { get { return StartPointF; } set { StartPointF = value; } }
#if DEBUGTEST
		[Browsable(false)]
		public
#else
		internal
#endif
		override PointF RightBottomF {
			get { return new PointF(EndPointF.X + WidthF, EndPointF.Y); }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandControlRootReport"),
#endif
		SRCategory(ReportStringId.CatBehavior),
		]
		public override XtraReport RootReport { get { return (XtraReport)this.Parent; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandControlReport"),
#endif
		SRCategory(ReportStringId.CatBehavior),
		]
		public override XtraReportBase Report { get { return (XtraReportBase)this.Parent; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandControlStartBand"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCrossBandControl.StartBand"),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.StartBandConverter)),
		SRCategory(ReportStringId.CatLayout),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public Band StartBand {
			get { return startBand; }
			set {
				if(!this.Suspended && !AreValidBandIndexes(value, endBand))
					throw new ArgumentException("value");
				if(value is DetailReportBand)
					throw new ArgumentException("The StartBand property can be set to the DetailReportBand value");
				SetBandCore(ref startBand, value);
				ValidatePoints();
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRCrossBandControlEndBand"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCrossBandControl.EndBand"),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.EndBandConverter)),
		SRCategory(ReportStringId.CatLayout),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public Band EndBand {
			get { return endBand; }
			set {
				Band band = value;
				if(!this.Suspended && !AreValidBandIndexes(startBand, band))
					throw new ArgumentException("value");	
				DetailReportBand detailReportBand = band as DetailReportBand;
				if(detailReportBand != null) {
					if(!ReportIsLoading) {
						throw new ArgumentException("The EndBand property can not be set to the DetailReportBand value");
					}
					band = detailReportBand.Bands.Count != 0 ? detailReportBand.Bands[0] : null;
				}
				SetBandCore(ref endBand, band);
				ValidatePoints();
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public PointF StartPointF {
			get { return startPoint; }
			set {
				startPoint = value;
				endPoint.X = value.X;
				ValidatePoints();
			}
		}
		[
		Localizable(true),
		SRCategory(ReportStringId.CatLayout),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		TypeConverter(typeof(PointFloatConverterForDisplay)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCrossBandControl.StartPoint"),
		XtraSerializableProperty,
		]
		public PointFloat StartPointFloat { get { return new PointFloat(StartPointF); } set { StartPointF = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public Point StartPoint {
			get { return Point.Round(StartPointF); }
			set { StartPointF = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public PointF EndPointF {
			get { return endPoint; }
			set {
				SetEndPointCore(value);
				ValidatePoints();
			}
		}
		[
		Localizable(true),
		SRCategory(ReportStringId.CatLayout),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		TypeConverter(typeof(PointFloatConverterForDisplay)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRCrossBandControl.EndPoint"),
		XtraSerializableProperty,
		]
		public PointFloat EndPointFloat { get { return new PointFloat(EndPointF); } set { EndPointF = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public Point EndPoint {
			get { return Point.Round(EndPointF); }
			set { EndPointF = value; }
		}
		internal override XRControlCollectionBase ControlContainer { 
			get { return RootReport != null ? RootReport.CrossBandControls : null; }
		}
		internal override XRControl LocationParent {
			get { return StartBand; }
		}
		internal override XRControl RightBottomParent {
			get { return EndBand; }
		}
		internal protected override VerticalAnchorStyles DefaultAnchorVertical {
			get {
				return VerticalAnchorStyles.Both;
			}
		}
		#endregion
		#region events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PrintEventHandler BeforePrint { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler AfterPrint { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event DrawEventHandler Draw { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler SizeChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler LocationChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler ParentChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseDown { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseUp { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewDoubleClick { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PrintOnPageEventHandler PrintOnPage { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event PreviewMouseEventHandler PreviewMouseMove { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event BindingEventHandler EvaluateBinding { add { } remove { } }
		#endregion
		public XRCrossBandControl()
			: base() {
			controlDictionary = new Dictionary<object, XRControl>();
			this.WidthF = this.GetMinimumWidth();
		}
		protected internal override bool SupportSnapLines {
			get { return false; }
		}
		internal void BeforeReportPrintInternal() {
			BeforeReportPrint();
		}
		internal void AfterReportPrintInternal() {
			AfterReportPrint();
		}
		internal bool AreValidBandIndexes(Band startBand, Band endBand) {
			if(startBand != null && endBand != null) {
				int startIndex = this.GetBandIndex(startBand);
				int endIndex = this.GetBandIndex(endBand);
				if(startIndex >= 0 && endIndex >= 0 && startIndex > endIndex)
					return false;
			}
			return true;
		}
		internal void ValidatePoints() {
			this.startPoint.X = Math.Max(0, startPoint.X);
			this.endPoint.X = Math.Max(0, endPoint.X);
			if(this.Suspended)
				return;
			this.startPoint.Y = Math.Max(0, startPoint.Y);
			this.endPoint.Y = Math.Max(0, endPoint.Y);			
			if(this.StartBand != null && this.EndBand != null && this.StartBand == this.EndBand)
				this.endPoint.Y = Math.Max(this.endPoint.Y, this.startPoint.Y);
		}
		void SetBandCore(ref Band band, Band value) {
			if(band != value) {
				if(band != null)
					band.Disposing -= new EventHandler(this.OnBandDisposing);
				band = value;
				if(band != null)
					band.Disposing += new EventHandler(this.OnBandDisposing);
			}
		}
		void SetEndPointCore(PointF pt) {
			if(this.BoundsChanging)
				return;
			this.BoundsChanging = true;
			this.endPoint = pt;
			this.startPoint.X = pt.X;
			if(this.EndBand != null)
				this.EndBand.UpdateHeight();
			this.BoundsChanging = false;
		}
		protected override void SetParent(XRControl value) {
			if(fParent != value && value != null)
				((XtraReport)value).CrossBandControls.Add(this);
		}
		protected internal override void SyncDpi(float dpi) {
			if(dpi != Dpi) {
				this.startPoint = XRConvert.Convert(this.startPoint, Dpi, dpi);
				this.endPoint = XRConvert.Convert(this.endPoint, Dpi, dpi);
				foreach(XRControl control in this.controlDictionary.Values)
					control.SyncDpi(dpi);
			}
			base.SyncDpi(dpi);
		}
		IEnumerable<XRControl> BandControls() {
			foreach(Band band in RootReport.AllBands) {
				XRControl[] printableControls = GetPrintableControls(band);
				if(printableControls == null)
					continue;
				foreach(XRControl control in printableControls)
					yield return control;
			}
		}
#if DEBUGTEST
		internal IEnumerable<XRControl> Test_BandControls() {
			return BandControls();
		}
#endif // DEBUGTEST
		protected internal override void OnParentBoundsChanged(XRControl parent, RectangleF oldBounds, RectangleF newBounds) {
			if(this.Suspended)
				return;
			Band band = parent as Band;
			CrossBandControlAnchorer.CreateInstance(this, band).SetAnchorBounds(oldBounds, newBounds);
		}
		internal int GetMinimumBottom(Band band) {
			return CrossBandControlAnchorer.CreateInstance(this, band).GetMinimumBottom();
		}
		internal override RectangleF GetBounds(Band band) {
			return GetBounds(GetPrintableControlsForce(band));
		}
		static RectangleF GetBounds(XRControl[] controls) {
			if(controls == null || controls.Length == 0)
				return RectangleF.Empty;
			RectangleF rect = controls[0].BoundsF;
			for(int i = 1; i < controls.Length; i++)
				rect = RectangleF.Union(rect, controls[i].BoundsF);
			return rect;
		}
		internal override bool IsInsideBand(Band band) {
			int bandIndex = this.GetBandIndex(band);
			return bandIndex >= 0
				&& bandIndex >= this.GetBandIndex(this.StartBand)
				&& bandIndex <= this.GetBandIndex(this.EndBand);
		}
		int GetBandIndex(Band band) {
			return this.RootReport != null ? this.RootReport.AllBands.IndexOf(band) : -1;
		}
		internal Band GetBand(int index) {
			return index >= 0 && index < this.RootReport.AllBands.Count ?
				this.RootReport.AllBands[index] : null;
		}
		internal Band GetPreviousBand(Band band) {
			int index = GetBandIndex(band);
			return GetBand(index - 1);
		}
		internal Band GetNextBand(Band band) {
			int index = GetBandIndex(band);
			return GetBand(index + 1);
		}
		internal Band GetLastBand() {
			return this.RootReport.AllBands.Count > 0 ?
				this.RootReport.AllBands[this.RootReport.AllBands.Count - 1] : null;
		}
		internal XRControl[] GetPrintableControls(Band band) {
			if(!IsInsideBand(band))
				return null;
			if(!DesignMode && !ReferenceEquals(band, StartBand) && !ReferenceEquals(band, EndBand) && RepeatEveryPage(StartBand) && RepeatEveryPage(EndBand))
				return new XRControl[] { };
			return GetPrintableControlsForce(band);
		}
		static bool RepeatEveryPage(Band band) {
			return band is MarginBand || (band is PageBand && ((PageBand)band).PrintOn == PrintOnPages.AllPages) || (band is GroupBand && ((GroupBand)band).RepeatEveryPage); 
		}
		internal abstract XRControl[] GetPrintableControlsForce(Band band);
		protected override void Dispose(bool disposing) {
			DevExpress.XtraPrinting.Tracer.TraceInformationTest(NativeSR.TraceSourceTests, "Dispose");
			base.Dispose(disposing);
			if(disposing) {
				if(controlDictionary != null) {
					foreach(XRControl item in controlDictionary.Values)
						item.Dispose();
					controlDictionary.Clear();
					controlDictionary = null;
				}
				if(startBand != null) {
					startBand.Disposing -= new EventHandler(this.OnBandDisposing);
					startBand = null;
				}
				if(endBand != null) {
					endBand.Disposing -= new EventHandler(this.OnBandDisposing);
					endBand = null;
				}
			}
		}
		void OnBandDisposing(object sender, EventArgs e) {
			Band band = (Band)sender;
			if(band == this.StartBand && band == this.EndBand)
				this.Dispose();
			else if(band == this.StartBand) {
				this.StartBand = this.GetNextBand(band);
				this.startPoint.Y = 0;
			} else if(band == this.EndBand) {
				this.EndBand = this.GetPreviousBand(band);
				if(this.EndBand != null)
					this.endPoint.Y = this.EndBand.HeightF;
			}
		}
		protected XRControl GetXRControl(Band band, object key) {
			XRControl control;
			if(!controlDictionary.TryGetValue(key, out control)) {
				control = CreateXRControl(band);
				control.Dpi = this.Dpi;
				control.Site = this.Site;
				controlDictionary.Add(key, control);
			}
			return control;
		}
		protected void SetVerticalLineAnchor(Band band,XRControl control) {
			CrossBandControlAnchorer.CreateInstance(this, band).SetVerticalLineAnchor(control);
		}
		protected internal override bool HasPrintingWarning() {
			foreach(WrappedXRLine control in BandControls())
				if(control.HasPrintingWarning())
					return true;
			return false;
		}
		protected internal override bool HasExportWarning() {
			foreach(WrappedXRLine control in BandControls())
				if(control.HasExportWarning())
					return true;
			return false;
		}
		protected abstract XRControl CreateXRControl(Band band);
		protected override bool IsCrossbandControl {
			get {
				return true;
			}
		}
	}
}
