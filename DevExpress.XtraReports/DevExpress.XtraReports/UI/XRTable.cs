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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Runtime.Serialization;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraPrinting.Export.Rtf;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Utils.Design;
using System.Linq;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	XRDesigner("DevExpress.XtraReports.Design.XRTableDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRTableDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DefaultCollectionName("Rows"),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRTable.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTable", "Table"),
	XRToolboxSubcategoryAttribute(0, 5),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRTable.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRTable.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRTable : XRControl, IWeightyContainer, ISupportInitialize {
		#region static
		public static XRTable CreateTable(RectangleF bounds, int rowCount, int columnCount) {
			XRTable table = new XRTable(bounds);
			table.BeginInit();
			if(rowCount > 0) {
				SizeF size = bounds.Size;
				size.Height /= rowCount;
				for(int i = 0; i < rowCount; i++) {
					XRTableRow row = XRTableRow.CreateRow(size.Height, columnCount);
					row.Weight = WeightHelper.DefaultWeight;
					table.Rows.Add(row);
				}
			}
			table.EndInit();
			return table;
		}
		#endregion
		#region Inner Classes
		class MergedBrickInfo {
			readonly float mergedBrickHeight;
			readonly List<RowBrick> rowBricks;
			float delta;
			public MergedBrickInfo(float mergedBrickHeight, RowBrick rowBrick) {
				this.mergedBrickHeight = mergedBrickHeight;
				this.rowBricks = new List<RowBrick>() { rowBrick };
			}
			public void CalculateDelta() {
				delta = Math.Max(0, mergedBrickHeight - rowBricks.Sum(x => x.Height));
			}
			public float MergedBrickHeight { get { return mergedBrickHeight; } }
			public List<RowBrick> RowBricks { get { return rowBricks; } }
			public float Delta { get { return delta; } }
		}
		#endregion
		#region Fields & Properties
		private bool isLoading;
		bool hasSpanningCells = false;
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableKeepTogether"),
#endif
		DefaultValue(false)
		]
		public override bool KeepTogether { get { return base.KeepTogether; } set { base.KeepTogether = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
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
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string XlsxFormatString { get { return ""; } set { } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new XRControlCollection Controls { get { return base.Controls; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public XRTableRowCollection Rows { get { return (XRTableRowCollection)fXRControls; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string Target {
			get { return string.Empty; }
			set { }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Bindable(false)
		]
		public override string NavigateUrl {
			get { return string.Empty; }
			set { }
		}
		internal double AmountOfOneWeight { get { return WeightHelper.GetAmountOfOneWeight(Rows, HeightF); } }
		internal bool IsLoading { get { return isLoading; } }
		internal bool HasSpanningCells { get { return hasSpanningCells; } }
		protected internal override bool CanDrawBackground { get { return false; } }
		TableModifier tableModifier;
		internal TableModifier TableModifier {
			get {
				if(tableModifier == null)
					tableModifier = new TableModifier(this);
				return tableModifier;
			}
			set { tableModifier = value; }
		}
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRTable()
			: this(new Rectangle(Point.Empty, DefaultSizes.Table)) {
		}
		internal XRTable(RectangleF bounds)
			: base(bounds) {
			KeepTogether = false;
		}
		protected override bool IntersectsWithChildren(int digits) {
			return false;
		}
		protected override XRControlScripts CreateScripts() {
			return new XRTableScripts(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				tableModifier = null;
			base.Dispose(disposing);
		}
		public void AdjustSize() {
			float width = 0;
			float height = 0;
			for(int i = 0; i < Rows.Count; i++) {
				height += Rows[i].HeightF;
				float rowWidth = 0;
				for(int j = 0; j < Rows[i].Cells.Count; j++) {
					rowWidth += Rows[i].Cells[j].WidthF;
				}
				width = Math.Max(width, rowWidth);
			}
			if(width > 0 && height > 0)
				SizeF = new SizeF(width, height);
		}
		public XRTableCell[] InsertColumnToLeft(XRTableCell baseCell) {
			return InsertColumnToLeft(baseCell, false);
		}
		public XRTableCell[] InsertColumnToRight(XRTableCell baseCell) {
			return InsertColumnToRight(baseCell, false);
		}
		public XRTableCell[] InsertColumnToLeft(XRTableCell baseCell, bool autoExpandTable) {
			return TableModifier.InsertColumn(baseCell, CellInsertPosition.Left, autoExpandTable, false);
		}
		public XRTableCell[] InsertColumnToRight(XRTableCell baseCell, bool autoExpandTable) {
			return TableModifier.InsertColumn(baseCell, CellInsertPosition.Right, autoExpandTable, false);
		}
		internal XRTableCell[] InsertColumnToLeft(XRTableCell baseCell, bool autoExpandTable, bool inheritBaseCellAppearance) {
			return TableModifier.InsertColumn(baseCell, CellInsertPosition.Left, autoExpandTable, inheritBaseCellAppearance);
		}
		internal XRTableCell[] InsertColumnToRight(XRTableCell baseCell, bool autoExpandTable, bool inheritBaseCellAppearance) {
			return TableModifier.InsertColumn(baseCell, CellInsertPosition.Right, autoExpandTable, inheritBaseCellAppearance);
		}
		public void DeleteColumn(XRTableCell baseCell) {
			DeleteColumn(baseCell, false);
		}
		public void DeleteColumn(XRTableCell baseCell, bool autoShrinkTable) {
			if(!ContainsCell(baseCell))
				throw new ArgumentException("baseCell doesn't belong to this XRTable");
			List<XRTableCell> cells = GetAlignedCells(baseCell, CellInsertPosition.Left);
			List<XRTableRow> rows = new List<XRTableRow>(cells.Count);
			float lowestWidth = baseCell.WidthF;
			bool isLeftColumn = baseCell.PreviousCell == null;
			foreach(XRTableCell cell in cells) {
				XRTableRow row = cell.Row;
				if(cell.PreviousCell == null && cell.NextCell == null && cell.Row != null)
					TableModifier.DeleteRow(row);
				else {
					if(!rows.Contains(row))
						rows.Add(row);
					row.DeleteCell(cell);
				}
				if(cell.WidthF < lowestWidth) lowestWidth = cell.WidthF;
			}
			if(autoShrinkTable)
				TableModifier.DeleteColumn(lowestWidth, isLeftColumn);
			if(!IsLoading)
				foreach(XRTableRow row in rows)
					row.ArrangeCells();
			rows.Clear();
			cells.Clear();
		}
		public void DeleteRow(XRTableRow row) {
			TableModifier.DeleteRow(row);
		}
		public XRTableRow InsertRowAbove(XRTableRow baseRow) {
			if(baseRow != null && !Rows.Contains(baseRow))
				throw new ArgumentException("baseRow doesn't belong to this XRTable");
			int index = 0;
			if(baseRow != null)
				index = baseRow.Index;
			else if(Rows.Count > 0)
				baseRow = Rows[0];
			return TableModifier.InsertRowByIndex(baseRow, index);
		}
		public XRTableRow InsertRowBelow(XRTableRow baseRow) {
			if(baseRow != null && !Rows.Contains(baseRow))
				throw new ArgumentException("baseRow doesn't belong to this XRTable");
			int index = Rows.Count;
			if(baseRow != null)
				index = baseRow.Index + 1;
			else if(Rows.Count > 0)
				baseRow = Rows[Rows.Count - 1];
			return TableModifier.InsertRowByIndex(baseRow, index);
		}
		public XRControl[] ConvertToControls() {
			return TableModifier.ConvertToControls();
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
		}
		#endregion
		internal void UpdatedBounds() {
			Band band = this.Parent as Band;
			if(band == null) return;
			RectangleF rect = this.BoundsF;
			if(this.AnchorVertical == VerticalAnchorStyles.Top || this.AnchorVertical == VerticalAnchorStyles.None) rect.Y = 0;
			else if(this.AnchorVertical == VerticalAnchorStyles.Bottom) {
				rect.Height = Math.Min(rect.Height, band.HeightF);
				rect.Y = band.HeightF - rect.Height;
			} else if(this.AnchorVertical == VerticalAnchorStyles.Both) {
				rect.Y = 0;
				rect.Height = band.HeightF;
			}
			this.BoundsF = rect;
		}
		internal void UpdateRowLayout() {
			if(isLoading || Suspended || Rows.Count == 0) return;
			SuspendLayout();
			try {
				UpdateWeights();
				ArrangeRows();
			} finally {
				ResumeLayout();
			}
		}
		internal void UpdateWeights() {
			WeightHelper.UpdateWeightBySize(Rows, HeightF);
			foreach(XRTableRow row in Rows)
				row.UpdateCellsWeights();
		}
		internal void ArrangeRows() {
			TableHelper.ArrangeRows(this);
		}
		internal void UpdateHasSpanningCells() {
			for(int i = 0; i < Rows.Count; i++) {
				XRTableRow currentRow = Rows[i];
				for(int j = 0; j < currentRow.Cells.Count; j++) {
					if(currentRow.Cells[j].RowSpan > 1) {
						hasSpanningCells = true;
						return;
					}
				}
			}
			hasSpanningCells = false;
		}
		protected internal override void SyncDpi(float dpi) {
			if(Dpi == dpi) return;
			base.SyncDpi(dpi);
			UpdateLayout();
		}
		protected override XRControlCollection CreateChildControls() {
			return new XRTableRowCollection(this);
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			return typeof(XRTableRow).IsAssignableFrom(componentType);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			PanelBrick brick = new TableBrick(this);
			foreach(VisualBrick childBrick in childrenBricks)
				brick.Bricks.Add(childBrick);
			return brick;
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			if(ps.Document.State != DocumentState.PostProcessing)
				brick.Style.Sides = BorderSide.None;
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			float totalHeight = 0;
			if(hasSpanningCells) {
				UpdateHeightForMergedBricks(brick as TableBrick);
			}
			for(int i = brick.Bricks.Count - 1; i >= 0; i--) {
				VisualBrick childVisualBrick = (VisualBrick)brick.Bricks[i];
				VisualBrickHelper.SetBrickBoundsY(childVisualBrick, totalHeight, Dpi);
				totalHeight += GetBrickBounds(childVisualBrick).Height;
			}
			return totalHeight;
		}
		#region Update Height For Merged Bricks
		List<MergedBrickInfo> CreateMergedBricksInfo(TableBrick tableBrick) {
			Dictionary<object, MergedBrickInfo> dict = new Dictionary<object, MergedBrickInfo>();
			object mergeValue;
			for(int i = tableBrick.InnerBrickList.Count - 1; i >= 0; i--) {
				RowBrick currentRowBrick = tableBrick.InnerBrickList[i] as RowBrick;
				for(int j = currentRowBrick.InnerBrickList.Count - 1; j >= 0; j--) {
					VisualBrick innerCellBrick = currentRowBrick.InnerBrickList[j] as VisualBrick;
					if(innerCellBrick.TryGetAttachedValue(BrickAttachedProperties.MergeValue, out mergeValue)) {  
						if(dict.ContainsKey(mergeValue))
							dict[mergeValue].RowBricks.Add(currentRowBrick);
						else
							dict.Add(mergeValue, new MergedBrickInfo(innerCellBrick.Height, currentRowBrick));
					}
				}
			}
			return dict.Values.ToList();
		}
		List<MergedBrickInfo> FilterAndSortMergedBricksInfo(List<MergedBrickInfo> mergedBricksInfo) {
			foreach(MergedBrickInfo info in mergedBricksInfo)
				info.CalculateDelta();
			return mergedBricksInfo.Where(x => x.Delta > 0).OrderBy(x => x.RowBricks.Last().Rect.Bottom).ToList();
		}
		void UpdateTableRowsWithMergedBricks(List<MergedBrickInfo> mergedBricksInfo) {
			Dictionary<RowBrick, float> rowDeltas = new Dictionary<RowBrick, float>();
			for(int i = 0; i < mergedBricksInfo.Count; i++) {
				MergedBrickInfo mergedBrickInfo = mergedBricksInfo[i];
				float actualDelta = mergedBrickInfo.Delta;
				float outDelta;
				foreach(RowBrick rowBrick in mergedBrickInfo.RowBricks) {
					if(rowDeltas.TryGetValue(rowBrick, out outDelta)) {
						actualDelta -= outDelta;
					}
				}
				if(actualDelta > 0) {
					RowBrick lastRowBrick = mergedBrickInfo.RowBricks.Last();
					if(rowDeltas.ContainsKey(lastRowBrick)) {
						rowDeltas[lastRowBrick] += actualDelta;
					} else {
						rowDeltas.Add(lastRowBrick, actualDelta);
					}
				}
			}
			foreach(var rows in rowDeltas) {
				RowBrick currentRowBrick = rows.Key;
				float newHeight = rows.Key.Height + rows.Value;
				for(int i = 0; i < currentRowBrick.Bricks.Count; i++)
					(currentRowBrick.Bricks[i] as VisualBrick).SetBoundsHeight(newHeight, GraphicsDpi.Document);
				currentRowBrick.SetBoundsHeight(newHeight, GraphicsDpi.Document);
			}
		}
		void UpdateHeightForMergedBricks(TableBrick tableBrick) {
			List<MergedBrickInfo> mergedBricksInfo = CreateMergedBricksInfo(tableBrick);
			if(mergedBricksInfo.IsEmpty())
				return;
			List<MergedBrickInfo> filteredMergedBricksInfo = FilterAndSortMergedBricksInfo(mergedBricksInfo);
			if(filteredMergedBricksInfo.IsEmpty())
				return;
			UpdateTableRowsWithMergedBricks(filteredMergedBricksInfo);
		}
		#endregion
		protected internal override void UpdateLayout() {
			if(IsDisposed || isLoading || Suspended || Rows.Count == 0) return;
			SuspendLayout();
			try {
				UpdateWeights();
				ArrangeRows();
			} finally {
				ResumeLayout();
			}
		}
		internal bool ContainsCell(XRTableCell cell) {
			foreach(XRTableRow row in Rows)
				if(row.Cells.Contains(cell))
					return true;
			return false;
		}
		#region ISupportInitialize
		public void BeginInit() {
			isLoading = true;
			DevExpress.XtraPrinting.Tracer.TraceInformationTest(NativeSR.TraceSourceTests, "XRTable.BeginInit");
		}
		public void EndInit() {
			isLoading = false;
			UpdateLayout();
			UpdateHasSpanningCells();
			DevExpress.XtraPrinting.Tracer.TraceInformationTest(NativeSR.TraceSourceTests, "XRTable.EndInit");
		}
		#endregion
		internal List<XRTableCell> GetAlignedCells(XRTableCell baseCell, CellInsertPosition alignKind) {
			List<XRTableCell> cells = new List<XRTableCell>();
			foreach(XRTableRow row in Rows)
				foreach(XRTableCell cell in row.Cells) {
					if((alignKind == CellInsertPosition.Left && FloatsComparer.Default.FirstEqualsSecond(cell.LeftF, baseCell.LeftF))
						|| (alignKind == CellInsertPosition.Right && FloatsComparer.Default.FirstEqualsSecond(cell.RightF, baseCell.RightF))) {
						cells.Add(cell);
						break;
					}
				}
			return cells;
		}
		#region IWeightyContainer
		double IWeightyContainer.AmountOfOneWeight {
			get { return AmountOfOneWeight; }
		}
		float IWeightyContainer.ExtendingAmount {
			get { return HeightF; }
			set { HeightF = value; }
		}
		float IWeightyContainer.Position {
			get { return TopF; }
			set { TopF = value; }
		}
		PointF IWeightyContainer.PositionAndExtendingAmount {
			set { SetBounds(LeftF, value.X, WidthF, value.Y, BoundsSpecified.Height | BoundsSpecified.Y); }
		}
		void IWeightyContainer.AddAmountToChildren(Side side, ref float amount) {
			if(Rows.Count == 0) return;
			XRTableRow row = null;
			switch(side) {
				case Side.After:
					row = Rows[Rows.Count - 1];
					break;
				case Side.Before:
					row = Rows[0];
					break;
			}
			if(row != null) {
				float minHeight = GetMinimumHeight();
				if(amount + row.HeightF < minHeight)
					amount = minHeight - row.HeightF;
				row.Weight += WeightHelper.GetWeight(this, amount);
			}
		}
		#endregion
		protected internal override bool Snapable {
			get { return false; }
		}
		protected override object CreateCollectionItem(string propertyName, DevExpress.Utils.Serializing.XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Rows)
				return CreateControl(e);
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Rows)
				Rows.Add(e.Item.Value as XRTableRow);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
		protected override BrickOwnerType BrickOwnerType {
			get {
				return BrickOwnerType.Table;
			}
		}
		protected override string ControlsUnityName {
			get {
				return Name;
			}
		}
		protected override TextEditMode TextEditMode {
			get {
				return TextEditMode.None;
			}
		}
	}
	public enum CellInsertPosition { Left, Right };
	internal enum ResizeBehaviour : byte {
		DefaultMode,
		ProportionalMode,
		SpecifiedMode,
	}
}
