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

using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Serialization;
using System.ComponentModel;
using System;
using System.Linq;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Design;
using System.Collections;
using DevExpress.Utils.Serializing;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native.DrillDown;
using DevExpress.XtraReports.Native.DrillDown;
namespace DevExpress.XtraReports.UI {
	[
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.DetailBand", "Detail"),
	BandKind(BandKind.Detail)
	]
	public class DetailBand : Band, IDrillDownNode {
		bool? printOnEmptyDataSource;
		private MultiColumn multiColumn;
		int repeatCount = -1;
		GroupFieldCollection sortFields;
		bool keepTogetherWithDetailReports;
		int printedCount;
		DrillDownHelper drillDownHelper;
		internal override bool DrillDownExpandedInternal {
			get {
				return DrillDownExpanded;
			}
			set {
				DrillDownExpanded = value;
			}
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownExpanded"),
		XtraSerializableProperty,
		]
		public bool DrillDownExpanded {
			get;
			set;
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DefaultValue(null),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.Band.DrillDownControl"),
		TypeConverter(typeof(DevExpress.XtraReports.Design.XRControlReferenceConverter)),
		Editor("DevExpress.XtraReports.Design.DrillDownBandEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		]
		public XRControl DrillDownControl {
			get;
			set;
		}
		internal int PrintedCount {
			get { return printedCount; }
			private set { printedCount = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("DetailBandKeepTogetherWithDetailReports"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.DetailBand.KeepTogetherWithDetailReports"),
		DefaultValue(false),
		SRCategory(ReportStringId.CatBehavior),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		]
		public bool KeepTogetherWithDetailReports {
			get { return keepTogetherWithDetailReports; }
			set { keepTogetherWithDetailReports = value; }
		}
		internal bool HasDownThenAcrossMultiColumn {
			get { return multiColumn.IsMultiColumn && multiColumn.Layout == ColumnLayout.DownThenAcross; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The PrintOnEmptyDataSource property is now obsolete. If it was False, set the XtraReportBase.DetailPrintCountOnEmptyDataSource property to 0.")
		]
		public bool PrintOnEmptyDataSource {
			get { return printOnEmptyDataSource != null ? printOnEmptyDataSource.Value : false; }
			set { printOnEmptyDataSource = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("DetailBandMultiColumn"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.DetailBand.MultiColumn"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		SRCategory(ReportStringId.CatBehavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public MultiColumn MultiColumn { get { return multiColumn; } }
		bool ShouldSerializeMultiColumn() {
			return multiColumn.ShouldSerialize();
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("The RepeatCountOnEmptyDataSource property is now obsolete. Use the XtraReportBase.DetailPrintCount property instead.")
		]
		public int RepeatCountOnEmptyDataSource {
			get { return repeatCount; }
			set {
				if(value <= 0)
					return;
				repeatCount = value;
			}
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("DetailBandSortFields"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.DetailBand.SortFields"),
		SRCategory(ReportStringId.CatData),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		]
		public GroupFieldCollection SortFields {
			get { return sortFields; }
		}
		bool ShouldSerializeSortFields() {
			return sortFields.Count != 0;
		}
		protected internal override string SortFieldsPropertyName {
			get { return "SortFields"; }
		}
		protected internal override GroupFieldCollection SortFieldsInternal {
			get {
				return sortFields;
			}
		}
		protected internal override bool IsDetail {
			get { return true; }
		}
		protected override XRControlStyles CreateStyles() {
			return new XRControlStyles(this);
		}
		public DetailBand()
			: base() {
			sortFields = new GroupFieldCollection(this);
			weightingFactor = DetailWeight;
			multiColumn = new MultiColumn(this);
			DrillDownControl = null;
		}
		protected internal override void OnBeforePrint(PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(e.Cancel || DesignMode) return;
			IDrillDownService serv = RootReport.GetService<IDrillDownService>();
			if(serv != null && !serv.BandExpanded(this))
				e.Cancel = true;
		}
		protected override DocumentBand CreateBandContainer(DocumentBandKind kind, int rowIndex) {
			return new DetailDocumentBand(rowIndex);
		}
		protected override DocumentBand CreateEmptyDocumentBand() {
			return new EmptyDetailDocumentBand(BandKind.ToDocumentBandKind());
		}
		internal DocumentBand CreateDocumentBand(PrintingSystemBase ps, int rowIndex, int rowCount, PageBuildInfo pageBuildInfo) {
			return Report.ReportPrintOptions.DetailCountOnEmptyDataSource > 0 || rowCount > 0 ?
				CreateDocumentBand(ps, rowIndex, pageBuildInfo) :
				CreateEmptyDocumentBand();
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				NullDrillDownHelper();
			base.Dispose(disposing);
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			PrintedCount = 0;
			NullDrillDownHelper();
			drillDownHelper = DrillDownHelper.CreateInstance(this, RootReport);
		}
		void NullDrillDownHelper() {
			if(drillDownHelper != null) {
				drillDownHelper.Dispose();
				drillDownHelper = null;
			}
		}
		protected internal override void OnAfterPrint(EventArgs e) {
			base.OnAfterPrint(e);
			PrintedCount++;
		}
		protected internal override void OnReportInitialize() {
			base.OnReportInitialize();
			sortFields.AddRange(Report.SortFieldsInternal.ToArray<GroupField>());
		}
		protected internal override void SyncDpi(float dpi) {
			if(dpi != Dpi) {
				multiColumn.SetColumnWidth(XRConvert.Convert(multiColumn.ColumnWidth, Dpi, dpi));
				multiColumn.SetColumnSpacing(XRConvert.Convert(multiColumn.ColumnSpacing, Dpi, dpi));
			}
			base.SyncDpi(dpi);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
			serializer.Serialize("MultiColumn", multiColumn);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
			serializer.Deserialize("MultiColumn", multiColumn);
		}
		#endregion
		internal void ApplyMultiColumn(DocumentBand docBand, float usefulPageWidth) {
			if(docBand != null && multiColumn.Mode != MultiColumnMode.None) {
				int columnCount = multiColumn.GetColumnCount(usefulPageWidth);
				float columnWidth = multiColumn.GetColumnWidth(usefulPageWidth, GraphicsDpi.Document);
				if(columnCount > 1 && columnWidth > 0)
					docBand.MultiColumn = new DevExpress.XtraPrinting.Native.MultiColumn(columnCount, columnWidth, multiColumn.Layout);
			}
		}
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.SortFields) {
				return new GroupField();
			}
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.SortFields)
				SortFields.Add((GroupField)e.Item.Value);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
	}
	class EmptyDetailDocumentBand : DocumentBand {
		public EmptyDetailDocumentBand(DocumentBandKind kind)
			: base(kind) {
		}
		public override bool IsDetailBand {
			get { return true; }
		}
	}
}
