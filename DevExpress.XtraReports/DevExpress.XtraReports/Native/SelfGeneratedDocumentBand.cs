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
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
using DevExpress.XtraReports.Native.Printing;
namespace DevExpress.XtraReports.UI {
	using System.ComponentModel;
	using DevExpress.XtraReports.Native.Data;
	using DevExpress.Data.Browsing;
	static class XtraReportExtensions {
		public static IEnumerable<XtraReport> AllReports(this XtraReport report) {
			XtraReport item = report;
			while(item != null) {
				yield return item;
				item = item.MasterReport;
			}
		}
		public static int[] GetRowIndexes(this XtraReportBase report) {
			if(report == null)
				return null;
			List<int> rowIndexes = new List<int>();
			while(report != null) {
				rowIndexes.Add(report.CurrentRowIndex);
				report = (XtraReportBase)report.Parent;
			}
			return rowIndexes.ToArray();
		}
		public static IList<PropertyDescriptor> GetAppropriatedCalculatedFields(this XtraReport report, object dataSource, string dataMember) {
			if(report != null) {
				using(XRDataContext dataContext = new XRDataContext(report.CalculatedFields, true)) {
					ListBrowser dataBrowser = dataContext.GetDataBrowser(dataSource, dataMember, true) as ListBrowser;
					if(dataBrowser == null) return null;
					List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
					PropertyDescriptorCollection itemProperties = dataBrowser.GetItemProperties();
					foreach(CalculatedField item in report.CalculatedFields) {
						PropertyDescriptor property = itemProperties.Find(item.Name, true);
						if(property is CalculatedPropertyDescriptorBase)
							properties.Add(property);
					}
					return properties;
				}
			}
			return null;
		}
	}
}
namespace DevExpress.XtraReports.Native {
	interface ISupportGrouping {
		bool IsGroupHeader { get; }
		object GroupingObject { get; }
	}
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, Kind = {Kind}}")]
	[System.Diagnostics.DebuggerTypeProxy(typeof(DebuggerHelpers.SelfGeneratedDocumentBandDebuggerTypeProxy))]
#endif
	public class SelfGeneratedDocumentBand : DocumentBand, ISupportProgress {
		#region static
		static object GetDataSource(DocumentBand docBand) {
			if(docBand == null)
				return null;
			if(docBand is SelfGeneratedDocumentBand && docBand.HasDataSource)
				return ((SelfGeneratedDocumentBand)docBand).DataSource;
			return GetDataSource(docBand.Parent);
		}
		static bool ShouldScale(DocumentBand band) {
			return !(band is SelfGeneratedSubreportDocumentBand);
		}
		#endregion
		#region fields & properties
		Band band;
		int[] rowIndexes;
		internal Band Band { get { return band; } }
		protected virtual object DataSource {
			get { return null; }
		}
		object ParentDataSource {
			get {
				return GetDataSource(Parent);
			}
		}
		public override float SelfHeight {
			get { return band.GetDocumentBandSelfHeight(base.SelfHeight); }
		}
		public override float TotalHeight {
			get { return band.GetDocumentBandTotalHeight(base.TotalHeight); }
		}
		public override bool HasDataSource {
			get { return band is XtraReportBase; }
		}
		public override bool IsDetailBand {
			get { return band is DetailBand; }
		}
		#endregion
		public SelfGeneratedDocumentBand(DocumentBandKind kind, Band band, int rowIndex)
			: base(kind, rowIndex) {
			this.band = band;
			rowIndexes = ((XtraReportBase)band.Report.Parent).GetRowIndexes();
		}
		public override bool ProcessIsFinished(ProcessState processState, int bandIndex) {
			return band is DetailReportBand ? bandIndex >= this.Bands.Count :
				base.ProcessIsFinished(processState, bandIndex);
		}
		protected SelfGeneratedDocumentBand(SelfGeneratedDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			this.band = source.band;
			DeactivatePageBreaks(source, rowIndex);
			if(source.ParentDataSource != null)
				Parent = source.Parent;
		}
		void DeactivatePageBreaks(SelfGeneratedDocumentBand source, int rowIndex) {
			if(source.Kind == DocumentBandKind.Footer && source.Parent.Kind == DocumentBandKind.Footer &&
			   source.Band.PageBreak == PageBreak.AfterBandExceptLastEntry && rowIndex == source.Band.Report.RowCount - 1) {
				DocumentBand lastBand = source.Parent.Bands.GetLast<DocumentBand>();
				if(lastBand.Kind == DocumentBandKind.PageBreak && lastBand.HasActivePageBreaks) {
					foreach(ValueInfo pageBreak in lastBand.PageBreaks)
						pageBreak.Active = false;
				}
			}
		}
		public override DocumentBand GetInstance(int rowIndex) {
			object dataContextState = this.band.Report.DataContext.SaveState();
			try {
				if(rowIndexes != null)
					this.band.Report.SetDataPosition(rowIndexes);
				DocumentBand band = CreateInstance(rowIndex);
				return band;
			} finally {
				this.band.Report.DataContext.LoadState(dataContextState);
			}
		}
		protected override void GenerateContent(DocumentBand source, int rowIndex) {
			SelfGeneratedDocumentBand selfGeneratedSource = (SelfGeneratedDocumentBand)source;
			Band sourceBand = selfGeneratedSource.band;
			object oldDataSource = sourceBand.SetDataSource(selfGeneratedSource.ParentDataSource);
			PrintingSystemBase ps = source.GetPrintingSystem();
			sourceBand.FillDocumentBand(this, ps, rowIndex, EnumerateBricks(selfGeneratedSource));
			sourceBand.SetDataSource(oldDataSource);
			ClearPageBreaks();
			Scale(ps.Document.ScaleFactor, ShouldScale);
		}
		static IEnumerable<Brick> EnumerateBricks(DocumentBand source) {
			foreach(Brick brick in source.Bricks)
				yield return brick;
			foreach(DocumentBand band in source.Bands)
				foreach(Brick brick in EnumerateBricks(band))
					yield return brick;
		}
		void ClearPageBreaks() {
			int pageBreakIndex = GetPageBreakIndex(0f);
			if(pageBreakIndex >= 0)
				PageBreaks.RemoveAt(pageBreakIndex);
			pageBreakIndex = GetPageBreakIndex(DocumentBand.MaxBandHeightPix);
			if(pageBreakIndex >= 0)
				PageBreaks.RemoveAt(pageBreakIndex);
		}
		protected virtual SelfGeneratedDocumentBand CreateInstance(int rowIndex) {
			return new SelfGeneratedDocumentBand(this, rowIndex);
		}
		public void SetDataPosition() {
			if(rowIndexes != null) {				
				int[] tempIndexes = new int[rowIndexes.Length + 1];
				Array.Copy(rowIndexes, 0, tempIndexes, 1, rowIndexes.Length);
				tempIndexes[0] = RowIndex;
				this.band.SetDataPosition(tempIndexes);
			} else
				this.band.SetDataPosition(new int[] { RowIndex });
		}
		#region ISupportProgress Members
		object ISupportProgress.ProgressAccessor {
			get { return object.ReferenceEquals(this, primarySource) ? band : null; }
		}
		#endregion
		public override object GroupKey {
			get { return Band is DetailReportBand ? Band : null; }
		}
		public override bool IsGroupItem {
			get { return Band is DetailBand; }
		}
	}
	public class SelfGeneratedMultiColumnDocumentBand : SelfGeneratedDocumentBand {
		DevExpress.XtraPrinting.Native.MultiColumn multiColumn;
		public override DevExpress.XtraPrinting.Native.MultiColumn MultiColumn {
			get { return multiColumn; }
			set { multiColumn = value; }
		}
		public SelfGeneratedMultiColumnDocumentBand(DocumentBandKind kind, Band band, int rowIndex)
			: base(kind, band, rowIndex) {
		}
		protected SelfGeneratedMultiColumnDocumentBand(SelfGeneratedMultiColumnDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			if(source.multiColumn != null)
				multiColumn = new DevExpress.XtraPrinting.Native.MultiColumn(source.multiColumn);
		}
		protected override SelfGeneratedDocumentBand CreateInstance(int rowIndex) {
			return new SelfGeneratedMultiColumnDocumentBand(this, rowIndex);
		}
		public override void Scale(double scaleFactor, Predicate<DocumentBand> shouldScale) {
			base.Scale(scaleFactor, shouldScale);
			if(multiColumn != null)
				multiColumn.Scale(scaleFactor);
		}
	}
	public class SelfGeneratedSubreportDocumentBand : SelfGeneratedMultiColumnDocumentBand, ISubreportDocumentBand, ISupportProgress {
		RectangleF reportRect;
		float minSelfHeight;
		object owner;
		object dataSource;
		protected override object DataSource {
			get { return dataSource; }
		}
		internal void SetDataSource(object dataSource) {
			this.dataSource = dataSource;
		}
		public override float MinSelfHeight {
			get {
				return minSelfHeight;
			}
		}
		RectangleF ISubreportDocumentBand.ReportRect {
			get { return reportRect; }
			set { reportRect = value; }
		}
		ILayoutData ISubreportDocumentBand.CreateLayoutData(LayoutDataContext context, RectangleF bounds) {
			return new SubreportDocumentBandLayoutData(this, context, bounds);
		}
		public SelfGeneratedSubreportDocumentBand(Band band, float minSelfHeight)
			: base(DocumentBandKind.Storage, band, -1) {
			this.minSelfHeight = minSelfHeight;
		}
		public SelfGeneratedSubreportDocumentBand(Band band, float minSelfHeight, SubreportBase owner)
			: this(band, minSelfHeight) {
			this.owner = owner;
		}
		SelfGeneratedSubreportDocumentBand(SelfGeneratedSubreportDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			this.reportRect = source.reportRect;
			this.minSelfHeight = source.minSelfHeight;
		}
		protected override SelfGeneratedDocumentBand CreateInstance(int rowIndex) {
			return new SelfGeneratedSubreportDocumentBand(this, rowIndex);
		}
		public override void Scale(double ratio, Predicate<DocumentBand> shouldScale) {
			base.Scale(ratio, shouldScale);
			minSelfHeight = MathMethods.Scale(minSelfHeight, ratio);
			reportRect = MathMethods.Scale(reportRect, ratio);
		}
		#region ISupportProgress Members
		object ISupportProgress.ProgressAccessor {
			get { return owner; }
		}
		#endregion
	}
}
