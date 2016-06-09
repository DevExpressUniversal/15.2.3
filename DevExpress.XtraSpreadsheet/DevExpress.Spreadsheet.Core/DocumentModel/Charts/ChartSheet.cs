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

using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.History;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ChartsheetProperties : SheetPropertiesBase, ICloneable<ChartsheetProperties>, ISupportsCopyFrom<ChartsheetProperties> {
		#region Fields
		readonly Chartsheet sheet;
		Margins margins;
		PrintSetup printSetup;
		HeaderFooterOptions headerFooter;
		TransitionOptions transitionOptions;
		ModelWorksheetView viewOptions;
		WorksheetProtectionOptions protectionOptions;
		int tabColorIndex;
		string codeName;
		#endregion
		public ChartsheetProperties(IDocumentModelPartWithApplyChanges documentModelPart)
			: base(documentModelPart) {
			Guard.ArgumentNotNull(documentModelPart, "sheet");
			this.sheet = documentModelPart as Chartsheet;
			Guard.ArgumentNotNull(this.sheet, "chartSheet");
			this.margins = new Margins(documentModelPart);
			this.printSetup = new PrintSetup(documentModelPart);
			this.headerFooter = new HeaderFooterOptions(documentModelPart);
			this.protectionOptions = new WorksheetProtectionOptions(documentModelPart);
			this.transitionOptions = new TransitionOptions();
			this.codeName = string.Empty;
			this.viewOptions = new ModelWorksheetView(sheet.Workbook);
		}
		public Margins Margins { get { return this.margins; } }
		public PrintSetup PrintSetup { get { return this.printSetup; } }
		public HeaderFooterOptions HeaderFooter { get { return headerFooter; } }
		public override WorksheetProtectionOptions Protection { get { return this.protectionOptions; } }
		public TransitionOptions TransitionOptions { get { return transitionOptions; } }
		public ModelWorksheetView ModelWorksheetView { get { return viewOptions; } }
		public int TabColorIndex { get { return tabColorIndex; } set { tabColorIndex = value; } }
		public string CodeName {
			get { return codeName; }
			set {
				if (string.IsNullOrEmpty(value))
					codeName = string.Empty;
				else
					codeName = value;
			}
		}
		#region ICloneable<ChartsheetProperties> Members
		public ChartsheetProperties Clone() {
			ChartsheetProperties result = new ChartsheetProperties(sheet);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ChartsheetProperties> Members
		public void CopyFrom(ChartsheetProperties value) {
			using (HistoryTransaction transaction = new HistoryTransaction(this.Margins.DocumentModel.History)) {
				try {
					this.Margins.DocumentModel.BeginUpdate();
					base.FormatProperties.CopyFrom(value.FormatProperties);
					this.headerFooter.CopyFrom(value.headerFooter);
					this.margins.CopyFrom(value.margins);
					this.printSetup.CopyFrom(value.printSetup);
					this.protectionOptions.CopyFrom(value.protectionOptions);
					this.transitionOptions.CopyFrom(value.transitionOptions);
					this.codeName = value.codeName;
				}
				finally {
					this.Margins.DocumentModel.EndUpdate();
				}
			}
		}
		#endregion
	}
	#region Chartsheet
	public class Chartsheet : InternalSheetBase, IWorksheet, IChart, IDrawingObject, IDocumentModelPartWithApplyChanges, ISupportsCopyFrom<Chartsheet> {
		Chart innerChart;
		public Chartsheet(DocumentModel workbook)
			: base(workbook) {
			SetProperties(new ChartsheetProperties(this));
			InnerChart = new Chart(this);
		}
		protected Chart InnerChart { get { return innerChart; } set { innerChart = value; } }
		public override SheetType SheetType { get { return SheetType.ChartSheet; } }
		public new ChartsheetProperties Properties { get { return (ChartsheetProperties)base.Properties; } }
		public new DocumentModel Workbook { get { return (DocumentModel)base.Workbook; } }
		public override bool IsSelected { get { return ActiveView.TabSelected; } set { ActiveView.TabSelected = value; } }
		DefinedNameCollectionBase IWorksheet.DefinedNames { get { return null; } }
		#region IWorksheet Members
		public bool IsColumnVisible(int columnIndex) {
			return false;
		}
		public bool IsRowVisible(int rowIndex) {
			return false;
		}
		public Table GetTableByCellPosition(int columnIndex, int rowIndex) {
			return null;
		}
		public ITableCollection Tables { get { return null; } }
		public bool IsDataSheet { get { return false; } }
		#endregion
		#region ICellTable Members
		public IRowCollectionBase Rows { get { return null; } }
		public ICellBase TryGetCell(int column, int row) {
			return null;
		}
		public ICellBase GetCell(int column, int row) {
			return TryGetCell(column, row);
		}
		public VariantValue GetCalculatedCellValue(int column, int row) {
			return VariantValue.ErrorValueNotAvailable; 
		}
		public int MaxRowCount { get { return 0; } }
		public int MaxColumnCount { get { return 0; } }
		#endregion
		#region ISupportsCopyFrom<Chartsheet> Members
		public void CopyFrom(Chartsheet value) {
			SetProperties(value.Properties.Clone());
			InnerChart = value.InnerChart.Clone();
		}
		#endregion
		#region IChart Members
		public DocumentModel DocumentModel { get { return Workbook as DocumentModel; } }
		public ChartViewCollection Views { get { return InnerChart.Views; } }
		public ChartViewSeriesDirection SeriesDirection { get { return InnerChart.SeriesDirection; } set { InnerChart.SeriesDirection = value; } }
		public AxisGroup PrimaryAxes { get { return InnerChart.PrimaryAxes; } }
		public AxisGroup SecondaryAxes { get { return InnerChart.SecondaryAxes; } }
		#endregion
		#region IDrawingObject Members
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			innerChart.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			innerChart.OnRangeRemoving(context);
		}
		#endregion
		#region ISupportsInvalidate Members
		public void Invalidate() {
		}
		#endregion
		#region IDrawingObject Members
		public Worksheet Worksheet {
			get { return null ; }
		}
		public DrawingObject DrawingObject { get { return InnerChart.DrawingObject; } }
		public ChartLocks Locks { get { return InnerChart.Locks; } }
		public bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public IGraphicFrameInfo GraphicFrameInfo { get { return InnerChart.GraphicFrameInfo; } }
		public bool LocksWithSheet { get { return InnerChart.LocksWithSheet; } set { InnerChart.LocksWithSheet = value; } }
		public bool PrintsWithSheet { get { return InnerChart.PrintsWithSheet; } set { InnerChart.PrintsWithSheet = value; } }
		public AnchorType AnchorType { get { return InnerChart.AnchorType; } set {  } }
		public AnchorType ResizingBehavior { get { return InnerChart.ResizingBehavior; } set {  } }
		public AnchorPoint From { get { return InnerChart.From; } set {  } }
		public AnchorPoint To { get { return InnerChart.To; } set {  } }
		public DrawingObjectType DrawingType { get { return DrawingObjectType.Chart; } }
		public bool CanRotate { get { return false; } }
		public float Height { get { return InnerChart.Height; } set { InnerChart.Height = value; } }
		public float Width { get { return InnerChart.Width; } set { InnerChart.Width = value; } }
		public float CoordinateX { get { return InnerChart.CoordinateX; } set {  } }
		public float CoordinateY { get { return InnerChart.CoordinateY; } set {  } }
		public int ZOrder { get { return InnerChart.ZOrder; } set {  } }
		public ShapeProperties ShapeProperties { get { return InnerChart.ShapeProperties; } }
		public int IndexInCollection { get { return InnerChart.IndexInCollection; } }
		public AnchorData AnchorData {get { return InnerChart.AnchorData; }}
		public void SetIndexInCollection(int value) {
		}
		public void Move(float offsetY, float offsetX) {
		}
		public void SetIndependentWidth(float width) {
		}
		public void SetIndependentHeight(float height) {
		}
		public void CoordinatesFromCellKey(CellKey cellKey) {
		}
		public void SizeFromCellKey(CellKey cellKey) {
		}
		public void Rotate(int angle) {
		}
		public float GetRotationAngleInDegrees() {
			return 0;
		}
		public void EnlargeWidth(float scale) {
		}
		public void EnlargeHeight(float scale) {
		}
		public void ReduceWidth(float scale) {
		}
		public void ReduceHeight(float scale) {
		}
		public void Visit(IDrawingObjectVisitor visitor) {
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			InnerChart = null;
		}
		#endregion
		#region IDocumentModelPart Members
		IDocumentModel IDocumentModelPart.DocumentModel { get { return this.Workbook ; } }
		DocumentModel IDocumentModelPartWithApplyChanges.Workbook { get { return this.Workbook; } }
		void IDocumentModelPartWithApplyChanges.ApplyChanges(DocumentModelChangeActions actions) {
			DocumentModel.ApplyChanges(actions);
		}
		#endregion
		#region IBatchUpdateable Members (Not sure)
		public BatchUpdateHelper BatchUpdateHelper {
			get { return (innerChart as IBatchUpdateable).BatchUpdateHelper; }
		}
		public void BeginUpdate() {
			(innerChart as IBatchUpdateable).BeginUpdate();
		}
		public void CancelUpdate() {
			(innerChart as IBatchUpdateable).CancelUpdate();
		}
		public void EndUpdate() {
			(innerChart as IBatchUpdateable).EndUpdate();
		}
		public bool IsUpdateLocked {
			get { return (innerChart as IBatchUpdateable).IsUpdateLocked; }
		}
		#endregion
	}
	#endregion
}
