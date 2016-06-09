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
using DevExpress.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel.Design;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Native.LayoutAdjustment;
using System.Collections.Generic;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraReports.Native.DrillDown;
using System.Collections.ObjectModel;
namespace DevExpress.XtraReports.UI {
	internal class EmptySpaceBand : Band {
		XRControl parent;
		List<CrossBandControlsInfo> crossBandControlsInfo = new List<CrossBandControlsInfo>();
		internal EmptySpaceBand(XRControl parent) {
			this.parent = parent;
			this.Dpi = parent.Dpi;
		}
		public override XRControl Parent {
			get { return parent; }
		}
		public List<CrossBandControlsInfo> CrossBandControlsInfo { get { return crossBandControlsInfo; } }
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposed) {
				parent = null;
			}
			base.Dispose(disposing);
		}
	}
	class CrossBandControlsInfo {
		readonly float offsetX;
		readonly List<XRControl> crossBandControls = new List<XRControl>();
		public CrossBandControlsInfo(float offsetX) {
			this.offsetX = offsetX;
		}
		public float OffsetX { get { return offsetX; } }
		public List<XRControl> CrossBandControls { get { return crossBandControls; } }
	}
	internal interface IMoveableBand {
		bool CanBeMoved(BandReorderDirection direction);
		void Move(BandReorderDirection direction);
	}
	internal interface IDrillDownNode {
		bool DrillDownExpanded { get; set; }
		XRControl DrillDownControl { get; }
	}
	[Flags]
	public enum BandKind {
		None = 0x0,
		[BandTypeAttribute(typeof(TopMarginBand))]
		TopMargin = 0x1,
		[BandTypeAttribute(typeof(PageHeaderBand))]
		PageHeader = 0x2,
		[BandTypeAttribute(typeof(ReportHeaderBand))]
		ReportHeader = 0x4,
		[BandTypeAttribute(typeof(GroupHeaderBand))]
		GroupHeader = 0x8,
		[BandTypeAttribute(typeof(DetailBand))]
		Detail = 0x10,
		[BandTypeAttribute(typeof(DetailReportBand))]
		DetailReport = 0x20,
		[BandTypeAttribute(typeof(GroupFooterBand))]
		GroupFooter = 0x40,
		[BandTypeAttribute(typeof(ReportFooterBand))]
		ReportFooter = 0x80,
		[BandTypeAttribute(typeof(PageFooterBand))]
		PageFooter = 0x100,
		[BandTypeAttribute(typeof(BottomMarginBand))]
		BottomMargin = 0x200,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[BandTypeAttribute(typeof(SubBand))]
		SubBand = 0x400,
	}
	internal enum BandReorderDirection { Up, Down }
	static class BandExtentsion {
		public static DocumentBandKind ToDocumentBandKind(this BandKind bandKind) {
			switch(bandKind) {
				case BandKind.None:
					return DocumentBandKind.PageBreak;
				case BandKind.PageHeader:
					return DocumentBandKind.PageHeader;
				case BandKind.GroupHeader:
					return DocumentBandKind.Header;
				case BandKind.TopMargin:
					return DocumentBandKind.TopMargin;
				case BandKind.PageFooter:
					return DocumentBandKind.PageFooter;
				case BandKind.GroupFooter:
					return DocumentBandKind.Footer;
				case BandKind.BottomMargin:
					return DocumentBandKind.BottomMargin;
				case BandKind.ReportHeader:
					return DocumentBandKind.ReportHeader;
				case BandKind.ReportFooter:
					return DocumentBandKind.ReportFooter;
				default:
					return DocumentBandKind.Detail;
			}
		}
		public static DocumentBand ToEmptyDocumentBand(this BandKind bandKind) {
			if(bandKind == BandKind.BottomMargin)
				return DocumentBand.CreateEmptyMarginBand(bandKind.ToDocumentBandKind(), 0);
			else
				return DocumentBand.CreateEmptyInstance(bandKind.ToDocumentBandKind());
		}
		public static Band GetPreviousBand(this Band band) {
			Band result = null;
			foreach(Band item in band.Report.OrderedBands) {
				if(ReferenceEquals(item, band))
					break;
				result = item;
			}
			return result;
		}
	}
	[
	DesignTimeVisible(false),
	XRDesigner("DevExpress.XtraReports.Design.BandDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._BandDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	BandKind(BandKind.None),
	]
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Name = {Name}}")]
#endif
	abstract public class Band : XRControl {
		#region static
		 internal static bool TryGetMultiColumn(Band band, out XtraReports.UI.MultiColumn mc) {
			 Band band2 = band is SubBand ? (Band)band.Parent : band;
			 mc = GetMultiColumn(band2.Report);
			 if(mc != null && (band2.BandKind == BandKind.Detail || band2.BandKind == BandKind.DetailReport || (band2 is GroupBand && mc.Layout == ColumnLayout.DownThenAcross)))
				 return true;
			 return band.Report != band.RootReport ? TryGetMultiColumn(band.Report, out mc) : false;
		 }
		 static XtraReports.UI.MultiColumn GetMultiColumn(XtraReportBase report) {
			 DetailBand detail = report.Bands[BandKind.Detail] as DetailBand;
			 return detail != null && detail.MultiColumn.IsMultiColumn ? detail.MultiColumn : null;
		 }
		internal static BandKind GetBandKindByType(Type bandType) {
			BandKindAttribute attr = (BandKindAttribute)TypeDescriptor.GetAttributes(bandType)[typeof(BandKindAttribute)];
			return attr != null ? attr.BandKind : BandKind.None;
		}
		internal const int TopMarginWeight = 1;
		internal const int ReportHeaderWeight = 2;
		internal const int PageHeaderWeight = 3;
		internal const int GroupHeaderWeight = 4;
		internal const int DetailWeight = 5;
		internal const int DetailReportWeight = 6;
		internal const int GroupFooterWeight = 7;
		internal const int ReportFooterWeight = 8;
		internal const int PageFooterWeight = 9;
		internal const int BottomMarginWeight = 10;
		#endregion
		#region Fields & Properties
		float height = DefaultSizes.BandHeight;
		PageBreak pageBreak = PageBreak.None;
		BandKind bandKind = BandKind.None;
		protected int weightingFactor;
		protected XRWriteInfo writeInfo;
		bool expanded = true;
		SubBandCollection subBands;
		bool canIgnoreAnchoring = false;
		internal XRWriteInfo WriteInfo { get { return writeInfo; } }
		protected override BrickOwnerType BrickOwnerType { get { return BrickOwnerType.Band; } }
		internal bool HasSubBands { get { return subBands != null && subBands.Count > 0; } }
		internal virtual bool DrillDownExpandedInternal {
			get { return true; }
			set {}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandSubBands"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.SubBands"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		SRCategory(ReportStringId.CatStructure),
		Editor("DevExpress.XtraReports.Design.SubBandCollectionEditor, " + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached | XtraSerializationFlags.DeserializeCollectionItemBeforeCallSetIndex),
		]
		public virtual SubBandCollection SubBands {
			get {
				if(subBands == null) {
					subBands = new SubBandCollection(this);
					subBands.CollectionChanged += OnSubBandsChanged;
				}
				return subBands;
			}
		}
		bool ShouldSerializeSubBands() {
			return HasSubBands;
		}
		[
		DefaultValue(true),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public virtual bool Expanded {
			get { return expanded; }
			set { expanded = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override PaddingInfo SnapLineMargin {
			get { return base.SnapLineMargin; }
			set { base.SnapLineMargin = value; }
		}
		[
		Browsable(true),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override PaddingInfo SnapLinePadding {
			get { return base.SnapLinePadding; }
			set { base.SnapLinePadding = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandScripts"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new BandScripts Scripts { get { return (BandScripts)fEventScripts; } }
		bool ShouldSerializeScripts() {
			return !fEventScripts.IsDefault();
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("BandCanHaveChildren")]
#endif
		public override bool CanHaveChildren { get { return true; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandPageBreak"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.PageBreak"),
		DefaultValue(PageBreak.None),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public virtual PageBreak PageBreak { get { return pageBreak; } set { pageBreak = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanPublish { get { return true; } set { } }
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
	DevExpressXtraReportsLocalizedDescription("BandHeightF"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.Height"),
		DefaultValue(DefaultSizes.BandHeight),
		Browsable(true),
		SRCategory(ReportStringId.CatLayout),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Localizable(true),
		XtraSerializableProperty,
		]
		public override float HeightF {
			get {
				return height;
			}
			set {
				if(ReportIsLoading)
					height = value;
				else if(!Suspended)
					SetHeight(value);
			}
		}
		protected internal override RectangleF ClientRectangleF { get { return GetClientRectangle(); } }
		protected internal virtual bool IsDetail { get { return false; } }
		protected internal virtual BandKind BandKind { 
			get {
				if(bandKind == UI.BandKind.None)
					bandKind = GetBandKindByType(GetType());
				return bandKind;
			}
		}
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
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override float WidthF {
			get { return 0; }
			set { }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override XRBindingCollection DataBindings { get { return base.DataBindings; } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override RectangleF BoundsF {
			get { return new RectangleF(PointF.Empty, new SizeF(RootReport.PageWidth - RootReport.Margins.Left - RootReport.Margins.Right, HeightF)); }
			set { HeightF = value.Height; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Text { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override SizeF SizeF { get { return SizeF.Empty; } set { } }
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override PointF LocationF { get { return PointF.Empty; } set { } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PointFloat LocationFloat { get { return base.LocationFloat; } set { base.LocationFloat = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override float LeftF { get { return 0; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override float TopF { get { return 0; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override float RightF { get { return base.RightF; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override float BottomF { get { return HeightF; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandCanGrow"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanGrow { get { return false; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandCanShrink"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CanShrink { get { return false; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap { get { return base.WordWrap; } set { base.WordWrap = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("BandKeepTogether"),
#endif
 DefaultValue(false)]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		protected internal override bool IsNavigateTarget { get { return false; } }
		protected internal virtual string SortFieldsPropertyName {
			get { return string.Empty; }
		}
		protected internal virtual GroupFieldCollection SortFieldsInternal {
			get {
				return null;
			}
		}
		protected internal virtual int LevelInternal {
			get { return -1; }
			set { }
		}
#if DEBUGTEST
		[Browsable(false)]
		public
#else
		internal
#endif
 override PointF RightBottomF {
			get {
				RectangleF rect = this.GetBounds();
				return new PointF(rect.Right, rect.Bottom);
			}
		}
		protected override bool CanHaveExportWarning { get { return false; } }
		#endregion
		protected Band() {
			KeepTogether = false;
			DrillDownExpandedInternal = true;
		}
		protected virtual void OnSubBandsChanged(object sender, CollectionChangeEventArgs e) {
			if(RootReport != null)
				RootReport.AllBands.Clear();
		}
		protected internal override void OnReportInitialize() {
			base.OnReportInitialize();
			if(HasSubBands) {
				foreach(Band band in SubBands) {
					band.OnReportInitialize();
				}
			}
		}
		protected internal override void InitializeScripts() {
			base.InitializeScripts();
			if(HasSubBands) {
				foreach(Band band in SubBands) {
					band.InitializeScripts();
				}
			}
		}
		protected internal override void OnReportEndDeserializing() {
			base.OnReportEndDeserializing();
			if(HasSubBands) {
				NestedComponentEnumerator nestedComponentEnumerator = new NestedComponentEnumerator(SubBands);
				while(nestedComponentEnumerator.MoveNext()) {
					nestedComponentEnumerator.Current.OnReportEndDeserializing();
				}
			}
		}
		protected override object CreateCollectionItem(string propertyName, DevExpress.Utils.Serializing.XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.SubBands)
				return CreateControl(e);
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.AddRange(SubBands);
			for(int i = 0; i < SubBands.Count; i++)
				SubBands[i].CollectAssociatedComponents(components);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.SubBands)
				SubBands.Add((SubBand)e.Item.Value);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
		protected override XRControlScripts CreateScripts() {
			return new BandScripts(this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXHelpExclude(true) ]
		public virtual IEnumerable<Band> OrderedBands {
			get {
				return SubBands;
			}
		}
		internal IEnumerable<Band> EnumBandsRecursive() {
			yield return this;
			foreach(Band band in OrderedBands) {
				foreach(Band band2 in band.EnumBandsRecursive())
					yield return band2;
			}
		}
		internal override IEnumerable<XRControl> AllControls(Predicate<XRControl> enumChildren) {
			foreach(XRControl item in base.AllControls(enumChildren)) {
				yield return item;
			}
			foreach(XRControl subBand in SubBands) {
				if(enumChildren(subBand)) {
					foreach(XRControl item in subBand.AllControls(enumChildren))
						yield return item;
				}
				yield return subBand;
			}
		}
		#region Events
		private static readonly object HeightChangedEvent = new object();
		[Browsable(false)]
		public event EventHandler HeightChanged {
			add { Events.AddHandler(HeightChangedEvent, value); }
			remove { Events.RemoveHandler(HeightChangedEvent, value); }
		}
		protected void OnHeightChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[HeightChangedEvent];
			if(handler != null)
				handler(this, e);
			Controls.UpdateLayout();
		}
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
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler LocationChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event ChangeEventHandler ParentChanged { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event BindingEventHandler EvaluateBinding { add { } remove { } }
		#endregion
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeSingle("Height", height);
			serializer.SerializeEnum("PageBreak", pageBreak);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			height = serializer.DeserializeSingle("Height", DefaultSizes.BandHeight);
			pageBreak = (PageBreak)serializer.DeserializeEnum("PageBreak", typeof(PageBreak), PageBreak.None);
		}
		#endregion
		protected internal override bool SupportSnapLines {
			get { return false; }
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			return
				typeof(SubreportBase).IsAssignableFrom(componentType) ||
				typeof(XRPageBreak).IsAssignableFrom(componentType) ||
				typeof(XRPivotGrid).IsAssignableFrom(componentType) ||
				base.CanAddControl(componentType, control);
		}
		protected internal RectangleF GetClientRectangle() {
			return new RectangleF(0, 0, RootReport.GetClientSize().Width, HeightF);
		}
		internal RectangleF GetBounds() {
			return this.GetBandBounds(this.HeightF);
		}
		RectangleF GetBandBounds(float height) {
			return new RectangleF(0, 0, RootReport.PageBounds.Width, height);
		}
		protected override Band GetBand() {
			return this;
		}
		internal override ArrayList GetPrintableControls() {
			return RootReport != null ? RootReport.GetBandPrintableControls(this) : new ArrayList();
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override Image ToImage() {
			throw new NotImplementedException();
		}
		[
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override Image ToImage(System.Drawing.Text.TextRenderingHint textRenderingHint) {
			throw new NotImplementedException();
		}
		void SetHeight(float val) {
			val = Math.Max(Math.Max(0, val), GetMaxControlBottom(false));
			if(height != val) {
				float oldHeight = height;
				height = val;
				OnHeightChanged(EventArgs.Empty);
				OnBoundsChanged(GetBandBounds(oldHeight), GetBandBounds(height));
				if(RootReport != null)
					RootReport.RaiseBandHeightChanged(new BandEventArgs(this));
			}
			if(canIgnoreAnchoring && height < GetMaxControlBottom(true))
				height = GetMaxControlBottom(true);
		}
		protected override void OnBoundsChanged(RectangleF oldBounds, RectangleF newBounds) {
			base.OnBoundsChanged(oldBounds, newBounds);
			if(RootReport != null)
				foreach(XRCrossBandControl cbItem in RootReport.CrossBandControls)
					cbItem.OnParentBoundsChanged(this, oldBounds, newBounds);
		}
		internal void UpdateHeightOnChangeContent() {
			try {
				canIgnoreAnchoring = true;
				UpdateHeight();
			} finally {
				canIgnoreAnchoring = false;
			}
		}
		internal void UpdateHeight() {
			if(!ReportIsLoading)
				try {
					XRAccessor.SetProperty(this, XRComponentPropertyNames.Height, HeightF);
				} catch { ; }
		}
		float GetMaxControlBottom(bool ignoreAnchoring) {
			return GetLogic().GetMaxControlBottom(this, ignoreAnchoring);
		}
		IBandLogic GetLogic() {
			IBandLogic logic = null;
			if(RootReport != null)
				logic = ((IServiceProvider)RootReport).GetService(typeof(IBandLogic)) as IBandLogic;
			return logic ?? new BandLogic();
		}
		protected internal override void SyncDpi(float dpi) {
			if(dpi != Dpi)
				height = Convert.ToInt32(XRConvert.Convert(height, Dpi, dpi));
			base.SyncDpi(dpi);
		}
		protected internal override int GetWeightingFactor() {
			return weightingFactor * 100;
		}
		protected override XRControlStyles CreateStyles() {
			return new XRBandStyles(this);
		}
		internal void FillDocumentBand(DocumentBand docBand, PrintingSystemBase ps, int rowIndex, IEnumerable<Brick> bricks) {
			writeInfo = CreateWriteInfo(ps, docBand, PageBuildInfo.Empty);
			writeInfo.IsSecondary = true;
			writeInfo.RegisterSecondaryBricks(bricks);
			GenerateContentAndDecompose(docBand, rowIndex, true);
		}
		void GenerateContentAndDecompose(DocumentBand docBand, int rowIndex, bool fireBeforePrint) {
			InitializeDocumentBand(docBand);
			GenerateContent(docBand, rowIndex, fireBeforePrint);
			new LayoutDecomposer(docBand).Decompose();
		}
		protected internal virtual void SetDataPosition(int[] indexPath) {
			XtraReportBase report = Report;
			for(int i = 0; i < indexPath.Length && report != null; i++) {
				report.DataBrowser.Position = indexPath[i];
				report = (XtraReportBase)report.Parent;
			}
		}
		XRWriteInfo CreateWriteInfo(PrintingSystemBase ps, DocumentBand docBand, PageBuildInfo pageBuildInfo) {
			return new XRWriteInfo(ps, Dpi, docBand, pageBuildInfo);
		}
		protected virtual void GenerateContent(DocumentBand docBand, int rowIndex, bool fireBeforePrint) {
			object state = Report.DataContext.SaveState();
			Report.DataBrowser.Position = rowIndex;
			try {
				if(fireBeforePrint) {
					PrintEventArgs args = new PrintEventArgs();
					OnBeforePrint(args);
					if(args.Cancel)
						return;
				}
				ArrayList printableControls = GetPrintableControls();
				foreach(XRControl control in printableControls) {
					control.UpdateBinding(Report.DataContext, writeInfo.PrintingSystem.Images);
				}
				foreach(XRControl control in printableControls) {
					control.WriteContentTo(writeInfo);
				}
				OnAfterPrint(EventArgs.Empty);
				docBand.ApplySpans(XRConvert.Convert(HeightF, Dpi, GraphicsDpi.Document));
				InsertPageBreaks(docBand);
			} finally {
				if(Report != null)
					Report.DataContext.LoadState(state);
			}
		}
		protected internal virtual float GetDocumentBandSelfHeight(float docBandSelfHeight) {
			return docBandSelfHeight;
		}
		protected internal virtual float GetDocumentBandTotalHeight(float docBandTotalHeight) {
			return docBandTotalHeight;
		}
		protected internal virtual object SetDataSource(object dataSource) {
			if(dataSource == null)
				return null;
			object oldDataSource = Report.DataSource;
			Report.DataSource = dataSource;
			return oldDataSource;
		}
		protected virtual void InsertPageBreaks(DocumentBand docBand) {
			switch(PageBreak) {
				case PageBreak.BeforeBand:
					docBand.PageBreaks.Add(new PageBreakInfo(0.0f));
					break;
				case PageBreak.BeforeBandExceptFirstEntry:
					if(this.Report.CurrentRowIndex > 0)
						docBand.PageBreaks.Add(new PageBreakInfo(0.0f));
					break;
				case PageBreak.AfterBand:
					docBand.PageBreaks.Add(PageBreakInfo.CreateMaxPageBreak());
					break;
				case PageBreak.AfterBandExceptLastEntry:
					if(docBand.RowIndex < this.Report.RowCount - 1) 
						docBand.PageBreaks.Add(PageBreakInfo.CreateMaxPageBreak());
					break;
			}
		}
		protected virtual void InitializeDocumentBand(DocumentBand docBand) {
			docBand.KeepTogether = KeepTogether;
		}
		protected virtual DocumentBand CreateEmptyDocumentBand() {
			return DocumentBand.CreateEmptyInstance(BandKind.ToDocumentBandKind());
		}
		protected internal virtual DocumentBand CreateDocumentBand(PrintingSystemBase ps, int rowIndex, PageBuildInfo pageBuildInfo) {
			if(GetEffectiveVisible()) {
				DocumentBandKind docBandKind = DocumentBandKind;
				SelfGeneratedDocumentBand docBand = new SelfGeneratedDocumentBand(docBandKind, this, rowIndex);
				writeInfo = CreateWriteInfo(ps, docBand, pageBuildInfo);
				GenerateContentAndDecompose(docBand, rowIndex, (docBandKind & DocumentBandKind.PageBand) == 0);
				docBand.Scale(writeInfo.PrintingSystem.Document.ScaleFactor, null);
				int pageBreakIndex = docBand.GetPageBreakIndex(0f);
				int pageBreakIndex2 = docBand.GetPageBreakIndex(DocumentBand.MaxBandHeightPix);
				IList<DocumentBand> subDocBands = CreateSubDocumentBands(ps, rowIndex, pageBuildInfo);
				if(pageBreakIndex >= 0 || pageBreakIndex2 >= 0 || subDocBands.Count > 0) {
					DocumentBand bandContainer = CreateBandContainer(docBandKind, rowIndex);
					bandContainer.RepeatEveryPage = docBand.RepeatEveryPage;
					bandContainer.PrintAtBottom = docBand.PrintAtBottom;
					bandContainer.PrintOn = docBand.PrintOn;
					docBand.RepeatEveryPage = false;
					if(pageBreakIndex >= 0) {
						DocumentBand pageBreakBand = DocumentBand.CreatePageBreakBand(0f);
						bandContainer.Bands.Add(pageBreakBand);
						docBand.PageBreaks.RemoveAt(pageBreakIndex);
					}
					bandContainer.Bands.Add(docBand);
					foreach(DocumentBand subDocBand in subDocBands)
						bandContainer.Bands.Add(subDocBand);
					pageBreakIndex2 = docBand.GetPageBreakIndex(DocumentBand.MaxBandHeightPix);
					if(pageBreakIndex2 >= 0) {
						DocumentBand pageBreakBand = DocumentBand.CreatePageBreakBand(0f);
						bandContainer.Bands.Add(pageBreakBand);
						docBand.PageBreaks.RemoveAt(pageBreakIndex2);
					}
					return bandContainer;
				}
				return docBand;
			}
			DocumentBand emptyInstanceDocBand = CreateEmptyDocumentBand();
			InitializeDocumentBand(emptyInstanceDocBand);
			return emptyInstanceDocBand;
		}
		protected virtual DocumentBandKind DocumentBandKind {
			get {
				return BandKind.ToDocumentBandKind();
			}
		}
		IList<DocumentBand> CreateSubDocumentBands(PrintingSystemBase ps, int rowIndex, PageBuildInfo pageBuildInfo) {
			List<DocumentBand> docBands = new List<DocumentBand>();
			if(HasSubBands) {
				foreach(Band band in SubBands) {
					DocumentBand docBand = band.CreateDocumentBand(ps, rowIndex, pageBuildInfo);
					if(!docBand.IsEmpty) docBands.Add(docBand);
				}
			}
			return docBands;
		}
		protected virtual DocumentBand CreateBandContainer(DocumentBandKind kind, int rowIndex) {
			return new DocumentBand(kind);
		}
		internal override int CompareTabOrder(XRControl xrControl) {
			return WeightingFactorComparer.MakeComparison(this, xrControl);
		}
		protected override ControlLayoutRules LayoutRules {
			get {
				return ControlLayoutRules.BottomSizeable;
			}
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			if(HasSubBands) {
				foreach(Band subBand in SubBands) {
					subBand.BeforeReportPrint();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing && HasSubBands) {
				subBands.CollectionChanged -= OnSubBandsChanged;
				subBands.Dispose();
				subBands = null;
			}
			base.Dispose(disposing);
		}
	}
	abstract public class MarginBand : Band {
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public override SubBandCollection SubBands {
			get { return SubBandCollection.Empty; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool KeepTogether {
			get { return false; }
			set { ; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override PageBreak PageBreak {
			get { return PageBreak.None; }
			set { ;}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool Expanded {
			get { return base.Expanded; }
			set { }
		}
		protected MarginBand()
			: base() {
		}
		protected override void InitializeDocumentBand(DocumentBand docBand) {
			docBand.RepeatEveryPage = true;
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.TopMarginBand", "TopMargin"),
	BandKind(BandKind.TopMargin)
	]
	public class TopMarginBand : MarginBand {
		public TopMarginBand()
			: base() {
			weightingFactor = TopMarginWeight;
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.BottomMarginBand", "BottomMargin"),
	BandKind(BandKind.BottomMargin)
	]
	public class BottomMarginBand : MarginBand {
		public BottomMarginBand()
			: base() {
			weightingFactor = BottomMarginWeight;
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.ReportHeaderBand", "ReportHeader"),
	BandKind(BandKind.ReportHeader)
	]
	public class ReportHeaderBand : Band {
		public ReportHeaderBand()
			: base() {
			weightingFactor = ReportHeaderWeight;
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			if(typeof(XRTableOfContents).IsAssignableFrom(componentType) && ReferenceEquals(Parent, RootReport) && (Controls.Contains(control) || AllControls<XRTableOfContents>().IsEmpty()))
				return true;
			return base.CanAddControl(componentType, control);
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.ReportFooterBand", "ReportFooter"),
	BandKind(BandKind.ReportFooter)
	]
	public class ReportFooterBand : Band {
		bool printAtBottom;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("ReportFooterBandPrintAtBottom"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.ReportFooterBand.PrintAtBottom"),
		DefaultValue(false),
		TypeConverter(typeof(BooleanTypeConverter)),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public bool PrintAtBottom { get { return printAtBottom; } set { printAtBottom = value; } }
		public ReportFooterBand()
			: base() {
			weightingFactor = ReportFooterWeight;
		}
		protected override void InitializeDocumentBand(DocumentBand docBand) {
			base.InitializeDocumentBand(docBand);
			docBand.PrintAtBottom = printAtBottom;
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			if(typeof(XRTableOfContents).IsAssignableFrom(componentType) && ReferenceEquals(Parent, RootReport) && (Controls.Contains(control) || AllControls<XRTableOfContents>().IsEmpty()))
				return true;
			return base.CanAddControl(componentType, control);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeBoolean("PrintAtBottom", printAtBottom);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			printAtBottom = serializer.DeserializeBoolean("PrintAtBottom", false);
		}
		#endregion
	}
	public abstract class PageBand : Band {
		PrintOnPages printOn = PrintOnPages.AllPages;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool KeepTogether {
			get { return false; }
			set { ; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override PageBreak PageBreak {
			get { return PageBreak.None; }
			set { ; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("PageBandPrintOn"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.PageBand.PrintOn"),
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(PrintOnPages.AllPages),
		XtraSerializableProperty,
		]
		public PrintOnPages PrintOn {
			get { return printOn; }
			set { printOn = value; }
		}
		protected override void InitializeDocumentBand(DocumentBand docBand) {
			docBand.RepeatEveryPage = true;
			docBand.PrintOn = printOn;
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PageHeaderBand", "PageHeader"),
	BandKind(BandKind.PageHeader)
	]
	public class PageHeaderBand : PageBand {
		public PageHeaderBand()
			: base() {
			weightingFactor = PageHeaderWeight;
		}
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.PageFooterBand", "PageFooter"),
	BandKind(BandKind.PageFooter)
	]
	public class PageFooterBand : PageBand {
		public PageFooterBand()
			: base() {
			weightingFactor = PageFooterWeight;
		}
	}
	abstract public class GroupBand : Band, IMoveableBand {
		private int level;
		private bool repeatEveryPage;
		private static readonly object LevelChangedEvent = new object();
		bool HasDownThenAcrossMultiColumn {
			get {
				DetailBand detail = Report.Bands[BandKind.Detail] as DetailBand;
				return detail != null && detail.HasDownThenAcrossMultiColumn;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupBandLevel"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupBand.Level"),
		DefaultValue(0),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public int Level {
			get { return level; }
			set {
				if(value < 0)
					throw new ArgumentOutOfRangeException("Level");
				level = value;
				if(!ReportIsLoading)
					OnLevelChanged(EventArgs.Empty);
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupBandRepeatEveryPage"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupBand.RepeatEveryPage"),
		DefaultValue(false),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool RepeatEveryPage { get { return repeatEveryPage; } set { repeatEveryPage = value; } }
		protected GroupBand()
			: base() {
		}
		protected internal override void OnBeforePrint(PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(e.Cancel || DesignMode) return;
			IDrillDownService serv = RootReport.GetService<IDrillDownService>();
			if(serv != null && !serv.BandExpanded(this))
				e.Cancel = true;
		}
		protected internal override int LevelInternal {
			get { return this.Level; }
			set { this.Level = value; }
		}
		[
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupBand.Scripts"),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public new GroupBandScripts Scripts { get { return (GroupBandScripts)fEventScripts; } }
		protected override XRControlScripts CreateScripts() {
			return new GroupBandScripts(this);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeInteger("Level", level);
			serializer.SerializeBoolean("RepeatEveryPage", repeatEveryPage);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			level = serializer.DeserializeInteger("Level", -1);
			repeatEveryPage = serializer.DeserializeBoolean("RepeatEveryPage", false);
		}
		#endregion
#if !SL
	[DevExpressXtraReportsLocalizedDescription("GroupBandBandLevelChanged")]
#endif
		public event EventHandler BandLevelChanged {
			add { Events.AddHandler(LevelChangedEvent, value); }
			remove { Events.RemoveHandler(LevelChangedEvent, value); }
		}
		private void OnLevelChanged(EventArgs e) {
			RunEventScript(LevelChangedEvent, EventNames.BandLevelChanged, e);
			EventHandler eventDelegate = (EventHandler)Events[LevelChangedEvent];
			if(eventDelegate != null)
				eventDelegate(this, e);
		}
		internal void SetLevelCore(int level) {
			this.level = level;
		}
		protected override void InitializeDocumentBand(DocumentBand docBand) {
			base.InitializeDocumentBand(docBand);
			docBand.RepeatEveryPage = RepeatEveryPage;
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			if(HasDownThenAcrossMultiColumn)
				RepeatEveryPage = false;
		}
		#region IMovableBand
		bool IMoveableBand.CanBeMoved(BandReorderDirection direction) {
			return Report.Groups.CanBandBeMoved(this, direction);
		}
		void IMoveableBand.Move(BandReorderDirection direction) {
			Report.Groups.MoveBand(this, direction);
		}
		#endregion
	}
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.GroupFooterBand", "GroupFooter"),
	BandKind(BandKind.GroupFooter),
	]
	public class GroupFooterBand : GroupBand {
		GroupFooterUnion groupUnion = GroupFooterUnion.None;
		bool printAtBottom;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupFooterBandGroupUnion"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupFooterBand.GroupUnion"),
		DefaultValue(GroupFooterUnion.None),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty,
		]
		public GroupFooterUnion GroupUnion { get { return groupUnion; } set { groupUnion = value; } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupFooterBandPrintAtBottom"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupFooterBand.PrintAtBottom"),
		DefaultValue(false),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool PrintAtBottom { get { return printAtBottom; } set { printAtBottom = value; } }
		public GroupFooterBand()
			: base() {
			weightingFactor = GroupFooterWeight;
		}
		protected internal override int GetWeightingFactor() {
			return base.GetWeightingFactor() + Level;
		}
		protected override void InitializeDocumentBand(DocumentBand docBand) {
			base.InitializeDocumentBand(docBand);
			docBand.PrintAtBottom = printAtBottom;
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.SerializeEnum("GroupUnion", groupUnion);
			serializer.SerializeBoolean("PrintAtBottom", printAtBottom);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			groupUnion = (GroupFooterUnion)serializer.DeserializeEnum("GroupUnion", typeof(GroupFooterUnion), GroupFooterUnion.None);
			printAtBottom = serializer.DeserializeBoolean("PrintAtBottom", false);
		}
		#endregion
		protected override void InsertPageBreaks(DocumentBand docBand) {
			if(PageBreak == PageBreak.BeforeBand && RepeatEveryPage) {
				docBand.PageBreaks.Add(PageBreakInfo.CreateMaxPageBreak());
			} else if(PageBreak == PageBreak.BeforeBandExceptFirstEntry && RepeatEveryPage) {
				if(docBand.RowIndex > 0)
					docBand.PageBreaks.Add(PageBreakInfo.CreateMaxPageBreak());
			} else
				base.InsertPageBreaks(docBand);
		}
	}
	[
	TypeConverter("DevExpress.XtraReports.Design.GroupFieldConverter," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupField")
	]
	public class GroupField : IXRSerializable {
		const XRColumnSortOrder defaultSortOrder = XRColumnSortOrder.Ascending;
		private GroupFieldCollection owner;
		private string fieldName = "";
		private XRColumnSortOrder sortOrder = XRColumnSortOrder.Ascending;
		internal virtual Band Band {
			get { return owner != null ? owner.Band : null; }
		}
		internal GroupFieldCollection Owner {
			get { return owner; }
			set { owner = value; }
		}
		internal int Index {
			get {
				return Band != null ? Band.SortFieldsInternal.IndexOf(this) : -1;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupFieldFieldName"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupField.FieldName"),
		Editor("DevExpress.XtraReports.Design.FieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		DefaultValue(""),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(DevExpress.XtraReports.Design.FieldNameConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public string FieldName {
			get { return fieldName != null ? fieldName : string.Empty; }
			set { fieldName = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("GroupFieldSortOrder"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.GroupField.SortOrder"),
		DefaultValue(defaultSortOrder),
		SRCategory(ReportStringId.CatBehavior),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		]
		public XRColumnSortOrder SortOrder {
			get { return sortOrder; }
			set { sortOrder = value; }
		}
		public GroupField() {
		}
		public GroupField(string fieldName) {
			this.fieldName = fieldName;
		}
		public GroupField(string fieldName, XRColumnSortOrder sortOrder) {
			this.fieldName = fieldName;
			this.sortOrder = sortOrder;
		}
		public GroupField(GroupField item) {
			fieldName = item.fieldName;
			sortOrder = item.sortOrder;
		}
		#region Serialization
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			serializer.SerializeString("FieldName", fieldName);
			serializer.SerializeEnum("SortOrder", sortOrder);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			fieldName = serializer.DeserializeString("FieldName", "");
			sortOrder = (XRColumnSortOrder)serializer.DeserializeEnum("SortOrder", typeof(XRColumnSortOrder), XRColumnSortOrder.Ascending);
		}
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		#endregion
		internal bool ShouldSerialize() {
			return
				!string.IsNullOrEmpty(fieldName) ||
				SortOrder != defaultSortOrder;
		}
	}
	[
	ListBindable(BindableSupport.No),
	TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
	]
	public class GroupFieldCollection : Collection<GroupField> {
		private Band band;
		internal Band Band {
			get { return band; }
		}
		public GroupFieldCollection(Band band)
			: base() {
			this.band = band;
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("GroupFieldCollectionItem")]
#endif
		public GroupField this[string fieldName] {
			get {
				for(int i = 0; i < Count; i++)
					if(this[i].FieldName == fieldName)
						return this[i];
				return null;
			}
		}
		public void AddRange(GroupField[] items) {
			foreach(GroupField item in items) {
				Add(item);
			}
		}
		protected override void InsertItem(int index, GroupField item) {
			base.InsertItem(index, item);
			SetItemOwner(item);
		}
		void SetItemOwner(GroupField item) {
			if(item.Owner != null && item.Owner != this)
				item.Owner.Remove(item);
			item.Owner = this;
		}
		protected override void RemoveItem(int index) {
			RemoveItemOwner(this[index]);
			base.RemoveItem(index);
		}
		void RemoveItemOwner(GroupField item) {
			if(item.Owner == this)
				item.Owner = null;
		}
	}
}
