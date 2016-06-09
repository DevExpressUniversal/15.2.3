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

using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using System;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(false),
	XRDesigner("DevExpress.XtraReports.Design.XRTableRowDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRTableRowDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DesignerSerializer("DevExpress.XtraReports.Design.XRControlCodeDomSerializer," + AssemblyInfo.SRAssemblyReportsExtensionsFull, AttributeConstants.CodeDomSerializer),
	DefaultCollectionName("Cells"),
	ToolboxBitmap(typeof(ResFinder), "Images.XRTableRow.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableRow", "TableRow"),
	]
	public class XRTableRow : XRControl, IWeighty, IWeightyContainer {
		#region Static
		public static XRTableRow CreateRow(SizeF size, int cellCount) {
			return CreateRow(size.Height, cellCount);
		}
		public static XRTableRow CreateRow(float height, int cellCount) {
			XRTableRow row = new XRTableRow(height);
			for(int i = 0; i < cellCount; i++) {
				XRTableCell cell = new XRTableCell();
				cell.Weight = WeightHelper.DefaultWeight;
				row.Cells.Add(cell);
			}
			return row;
		}
		#endregion
		#region Fields & Properties
		double? weight;
		bool tableSuspendedByRow;
		TableRowModifier tableRowModifier;
		internal override BorderSide VisibleContourBorders { get { return GetVisibleContourBorders(); } }
		internal TableRowModifier TableRowModifier {
			get {
				if(tableRowModifier == null)
					tableRowModifier = new TableRowModifier(this);
				return tableRowModifier;
			}
			set { tableRowModifier = value; }
		}
		internal RectangleF RealBoundsF {
			get {
				float top = TopF;
				float height;
				if(weight == null || Table == null || IsLoading)
					height = fBounds.Height;
				else if(Next == null)
					height = Table.HeightF - top;
				else
					height = (float)WeightHelper.GetAmount(Table, weight.Value);
				return new RectangleF(LeftF, top, WidthF, height);
			}
		}
		protected internal override XRControl OverlappingParent { get { return Table != null ? Table.Parent : null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty,
		Localizable(true),
		]
		public double Weight {
			get { return ((IWeighty)this).Weight; }
			set { ((IWeighty)this).Weight = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override PointFloat LocationFloat { get { return base.LocationFloat; } set { base.LocationFloat = value; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override SizeF SizeF {
			get { return base.SizeF; }
			set { base.SizeF = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableRowHeightF"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		SRCategory(ReportStringId.CatLayout),
		Browsable(true),
		CategoryAttribute("Layout")
		]
		public override float HeightF {
			get {
				return fBounds.Height;
			}
			set {
				if(base.HeightF != value || IsLoading)
					base.HeightF = value;
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowBoundsF")]
#endif
		public override RectangleF BoundsF {
			get { return new RectangleF(LeftF, TopF, WidthF, HeightF); }
			set { base.BoundsF = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowLeftF")]
#endif
		public override float LeftF {
			get { return 0; }
			set { }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowTopF")]
#endif
		public override float TopF {
			get {
				if(Table == null || IsLoading)
					return fBounds.Top;
				float top = 0;
				foreach(XRControl item in Parent.Controls) {
					if(ReferenceEquals(item, this)) break;
					top += item.HeightF;
				}
				return top;
			}
			set { }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowWidthF")]
#endif
		public override float WidthF {
			get {
				if(Table != null)
					return Table.WidthF;
				return fBounds.Width;
			}
			set { }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override VerticalAnchorStyles AnchorVertical {
			get { return VerticalAnchorStyles.None; }
			set { }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override HorizontalAnchorStyles AnchorHorizontal { get { return HorizontalAnchorStyles.None; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool LockedInUserDesigner {
			get { return Table != null ? Table.LockedInUserDesigner : base.LockedInUserDesigner; }
			set { base.LockedInUserDesigner = value; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableRowWordWrap"),
#endif
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
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override string NavigateUrl {
			get { return string.Empty; }
			set { }
		}
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
		public override XRControl BookmarkParent {
			get { return null; }
			set { }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XRTable Table {
			get { return Parent as XRTable; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowCanGrow")]
#endif
		public override bool CanGrow {
			get {
				return Table == null ? fCanGrow :
					fCanGrow && Table.AnchorAllowsShrinkGrow;
			}
			set { fCanGrow = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowCanShrink")]
#endif
		public override bool CanShrink {
			get {
				return Table == null ? fCanShrink :
					fCanShrink && Table.AnchorAllowsShrinkGrow;
			}
			set { fCanShrink = value; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public new XRControlCollection Controls {
			get { return base.Controls; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public XRTableCellCollection Cells {
			get { return (XRTableCellCollection)fXRControls; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override XRBindingCollection DataBindings {
			get { return base.DataBindings; }
		}
		internal XRTableRow Next {
			get {
				int nextIndex = Index + 1;
				if(nextIndex >= Table.Rows.Count)
					return null;
				return Table.Rows[nextIndex];
			}
		}
		internal XRTableRow Previous {
			get {
				if(Index <= 0)
					return null;
				return Table.Rows[Index - 1];
			}
		}
		internal double AmountOfOneWeight { get { return WeightHelper.GetAmountOfOneWeight(Cells, WidthF); } }
		protected override bool CanChangeZOrder { get { return false; } }
		protected internal override bool CanDrawBackground { get { return false; } }
		protected internal override bool IsNavigateTarget { get { return false; } }
		protected override bool CanHaveExportWarning { get { return false; } }
		internal bool IsLoading { get { return Table != null && Table.IsLoading; } }
		#endregion
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event HtmlEventHandler HtmlItemCreated { add { } remove { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRTableRow() {
		}
		internal XRTableRow(float height)
			: this() {
			HeightF = height;
		}
		protected override XRControlScripts CreateScripts() {
			return new XRTableScripts(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing)
				tableRowModifier = null;
			base.Dispose(disposing);
		}
		#region Serialization
		protected override void SerializeProperties(XRSerializer serializer) {
			base.SerializeProperties(serializer);
		}
		protected override void DeserializeProperties(XRSerializer serializer) {
			base.DeserializeProperties(serializer);
		}
		#endregion
		public void SetCellRange(params XRTableCell[] cells) {
			TableRowModifier.SetCellRange(cells);
		}
		public void SwapCells(XRTableCell cell1, XRTableCell cell2) {
			cell1.SwapWith(cell2);
		}
		public void InsertCell(XRTableCell cell, int index) {
			InsertCell(cell, index, false);
		}
		internal void InsertCell(XRTableCell cell, int index, bool inheritBaseCellAppearance) {
			bool isLoading = IsLoading;
			if(!isLoading)
				Table.BeginInit();
			try {
				if(Cells.Count > 0) {
					index = Math.Min(index, Cells.Count - 1);
					index = Math.Max(index, 0);
					XRTableCell baseCell = Cells[index];
					InsertCell(baseCell, cell, CellInsertPosition.Left, false, inheritBaseCellAppearance);
				} else
					SetCellRange(cell);
			} finally {
				if(!isLoading)
					Table.EndInit();
			}
		}
		public override void PerformLayout() {
			base.PerformLayout();
			ArrangeCells();
		}
		internal void ArrangeCells() {
			TableHelper.ArrangeCells(this);
		}
		internal void InsertCell(XRTableCell baseCell, XRTableCell newCell, CellInsertPosition position) {
			InsertCell(baseCell, newCell, position, false, false);
		}
		internal void InsertCell(XRTableCell baseCell, XRTableCell newCell, CellInsertPosition position, bool autoExpandTable, bool inheritBaseCellAppearance) {
			if(position == CellInsertPosition.Left)
				InsertCellToLeft(baseCell, newCell, autoExpandTable, inheritBaseCellAppearance);
			else
				InsertCellToRight(baseCell, newCell, autoExpandTable, inheritBaseCellAppearance);
		}
		void InsertCellToRight(XRTableCell baseCell, XRTableCell newCell, bool isColumnInsert, bool inheritBaseCellAppearance) {
			int index = Cells.Count;
			if(baseCell != null)
				index = baseCell.Index + 1;
			else
				if(Cells.Count > 0)
					baseCell = Cells[Cells.Count - 1];
			InsertCellByIndex(baseCell, newCell, index, isColumnInsert, inheritBaseCellAppearance);
		}
		internal void InsertCellByIndex(XRTableCell baseCell, XRTableCell newCell, int index, bool isColumnInsert, bool inheritBaseCellAppearance) {
			TableRowModifier.InsertCellByIndex(baseCell, newCell, index, isColumnInsert, inheritBaseCellAppearance);
		}
		void InsertCellToLeft(XRTableCell baseCell, XRTableCell newCell, bool autoExpandTable, bool inheritBaseCellAppearance) {
			int index = 0;
			if(baseCell != null)
				index = baseCell.Index;
			else
				if(Cells.Count > 0)
					baseCell = Cells[0];
			TableRowModifier.InsertCellByIndex(baseCell, newCell, index, autoExpandTable, inheritBaseCellAppearance);
		}
		internal void UpdateCellsWeights() {
			WeightHelper.UpdateWeightBySize(Cells, WidthF);
		}
		internal XRTableCell[] CloneCells() {
			XRTableCell[] cells = new XRTableCell[Cells.Count];
			int i = 0;
			foreach(XRTableCell cell in Cells) {
				XRTableCell newCell = new XRTableCell();
				newCell.Weight = cell.Weight;
				newCell.AssignStyle(cell.InnerStyle);
				cells[i++] = newCell;
			}
			return cells;
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			PanelBrick brick = new RowBrick(this);
			foreach(VisualBrick childBrick in childrenBricks)
				brick.Bricks.Add(childBrick);
			return brick;
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			brick.Style.Sides = BorderSide.None;
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			float maxHeight = 0;
			object mergeValue;
			for(int i = 0; i < brick.Bricks.Count; i++) {
				VisualBrick childVisualBrick = (VisualBrick)brick.Bricks[i];
				if(Table.HasSpanningCells && (childVisualBrick.BrickOwner as XRControl).ProcessDuplicatesMode != ProcessDuplicatesMode.Merge &&
					childVisualBrick.TryGetAttachedValue(BrickAttachedProperties.MergeValue, out mergeValue)) {
						if(!childVisualBrick.CanShrink)
							maxHeight = Math.Max(GetBrickBounds(brick).Height, maxHeight);
				} else
					maxHeight = Math.Max(GetBrickBounds(childVisualBrick).Height, maxHeight);
			}
			if(maxHeight >= 0) {
				for(int i = 0; i < brick.Bricks.Count; i++) {
					VisualBrick childVisualBrick = (VisualBrick)brick.Bricks[i];
					if((childVisualBrick.BrickOwner as XRControl).RowSpan > 1)
						continue;
					childVisualBrick.SetBoundsHeight(maxHeight, Dpi);
				}
				return brick.Bricks.Count > 0 ? maxHeight : GetBrickBounds(brick).Height;
			}
			return GetBrickBounds(brick).Height;
		}
		protected override bool ShouldSerializeLocation() {
			return false;
		}
		internal void SetSpecifiedBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBounds(x, y, width, height, specified, ResizeBehaviour.SpecifiedMode);
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBounds(x, y, width, height, specified, ResizeBehaviour.DefaultMode);
		}
		void SetBounds(float x, float y, float width, float height, BoundsSpecified specified, ResizeBehaviour resizeMode) {
			if(XRControl.SpecifiedEquals(specified, BoundsSpecified.X, BoundsSpecified.Y, BoundsSpecified.Location))
				return;
			bool isLoading = IsLoading;
			if(weight != null && !isLoading) {
				BeginInit();
				if(resizeMode == ResizeBehaviour.SpecifiedMode) {
					UpdateSpecifiedWeight(y, height);
				} else {
					UpdateWeight(y, height);
				}				
				SetBoundsCore(x, y, WidthF, height, specified);
				EndInit();
			} else {
				if(isLoading && weight != null)
					weight = null;
				SetBoundsCore(x, y, width, height, specified);
			}
		}
		protected internal override bool HasPrintingWarning() {
			return false;
		}
		protected override XRControlCollection CreateChildControls() {
			return new XRTableCellCollection(this);
		}
		protected internal override bool CanAddControl(Type componentType, XRControl control) {
			return typeof(XRTableCell).IsAssignableFrom(componentType);
		}
		void UpdateSpecifiedWeight(float y, float height) {
			WeightHelper.ResizeSpecifiedItem(this, TopF, y, HeightF, height, GetMinimumHeight());
		}
		void UpdateWeight(float y, float height) {
			WeightHelper.Resize(this, TopF, y, HeightF, height, GetMinimumHeight());
		}
		void BeginInit() {
			if(Table != null && !IsLoading) {
				Table.BeginInit();
				tableSuspendedByRow = true;
			}
		}
		void EndInit() {
			if(Table != null && IsLoading && tableSuspendedByRow)
				Table.EndInit();
			tableSuspendedByRow = true;
		}
		XRTableCell[] GetCells(Side side) {
			List<XRTableCell> result = new List<XRTableCell>(Table.Rows.Count);
			if(Table == null)
				return result.ToArray();
			foreach(XRTableRow row in Table.Rows) {
				if(row.Cells.Count == 0)
					break;
				XRTableCell cell = null;
				switch(side) {
					case Side.After:
						cell = row.Cells[row.Cells.Count - 1];
						break;
					case Side.Before:
						cell = row.Cells[0];
						break;
				}
				if(cell != null)
					result.Add(cell);
			}
			XRTableCell[] resultArray = result.ToArray();
			result.Clear();
			return resultArray;
		}
		BorderSide GetVisibleContourBorders() {
			BorderSide borders = BorderSide.All;
			if(Table.Rows.LastRow != this)
				borders = borders & ~BorderSide.Bottom;
			return borders;
		}
		#region IWeightyContainer
		float IWeightyContainer.ExtendingAmount {
			get { return Table.WidthF; }
			set { Table.WidthF = value; }
		}
		float IWeightyContainer.Position {
			get {
				if(Table != null)
					return Table.LeftF;
				else
					return LeftF;
			}
			set {
				if(Table != null)
					Table.LeftF = value;
				else
					LeftF = value;
			}
		}
		PointF IWeightyContainer.PositionAndExtendingAmount {
			set {
				((IWeightyContainer)this).Position = value.X;
				((IWeightyContainer)this).ExtendingAmount = value.Y;
			}
		}
		double IWeightyContainer.AmountOfOneWeight {
			get { return AmountOfOneWeight; }
		}
		void IWeightyContainer.AddAmountToChildren(Side side, ref float amount) {
			if(Table == null)
				return;
			XRTableCell[] cells = GetCells(side);
			float minWidth = GetMinimumWidth();
			foreach(XRTableCell cell in cells) {
				float width = cell.WidthF;
				if(amount + width < minWidth)
					amount = minWidth - width;
			}
			foreach(XRTableCell cell in cells)
				cell.Weight += WeightHelper.GetWeight(cell.Row, amount);
		}
		#endregion
		#region IWeighty
		IWeightyContainer IWeighty.Parent {
			get { return Table; }
		}
		IWeighty IWeighty.Next {
			get { return Next; }
		}
		IWeighty IWeighty.Previous {
			get { return Previous; }
		}
		float IWeighty.Amount {
			get { return HeightF; }
		}
		double IWeighty.Weight {
			get {
				if(weight == null)
					return 0.0;
				return weight.Value;
			}
			set {
				weight = value;
				if(Table == null)
					return;
				if(!IsLoading) {
					BeginInit();
					SetBoundsCore(0f, 0f, 0f, HeightF, BoundsSpecified.Height);
					EndInit();
				}
			}
		}
		#endregion
		public void DeleteCell(XRTableCell cell) {
			TableRowModifier.DeleteCell(cell);
		}
		protected internal override bool Snapable {
			get { return false; }
		}
		protected override object CreateCollectionItem(string propertyName, DevExpress.Utils.Serializing.XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Cells)
				return CreateControl(e);
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Cells)
				Cells.Add(e.Item.Value as XRTableCell);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
		protected override BrickOwnerType BrickOwnerType {
			get {
				return BrickOwnerType.TableRow;
			}
		}
		protected override ControlLayoutRules LayoutRules {
			get {
				return ControlLayoutRules.BottomSizeable | ControlLayoutRules.TopSizeable;
			}
		}
		protected override string ControlsUnityName {
			get {
				return Table != null ? Table.Name : null;
			}
		}
		protected override TextEditMode TextEditMode {
			get {
				return TextEditMode.None;
			}
		}
	}
	public class XRTableRowCollection : XRControlCollection, IEnumerable<IWeighty>, IEnumerable<XRTableRow> {
		#region Fields & Properties
		bool tableSuspended;
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowCollectionItem")]
#endif
		public new XRTableRow this[int index] {
			get { return (index >= 0 && index < Count) ? (XRTableRow)List[index] : null; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowCollectionFirstRow")]
#endif
		public XRTableRow FirstRow {
			get { return (Count > 0) ? this[0] : null; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableRowCollectionLastRow")]
#endif
		public XRTableRow LastRow {
			get { return (Count > 0) ? this[Count - 1] : null; }
		}
		XRTable Table { get { return owner as XRTable; } }
		bool IsLoading { get { return Table != null && Table.IsLoading; } }
		#endregion
		public XRTableRowCollection(XRTable owner)
			: base(owner) {
		}
		public bool Contains(XRTableRow row) {
			return InnerList.Contains(row);
		}
		public int IndexOf(XRTableRow row) {
			return InnerList.IndexOf(row);
		}
		public void Remove(XRTableRow row) {
			if(List.Contains(row)) {
				List.Remove(row);
				if(Table != null)
					TableHelper.ArrangeRows(Table);
			}
		}
		protected override void OnRemoveCompleteCore(int index, object value) {
			base.OnRemoveCompleteCore(index, value);
			if(Table != null)
				Table.ArrangeRows();
		}
		public int Add(XRTableRow row) {
			BeginInit();
			try {
				return base.Add(row);
			} finally {
				EndInit();
			}
		}
		public void AddRange(XRTableRow[] rows) {
			BeginInit();
			try {
				base.AddRange(rows);
			} finally {
				EndInit();
			}
		}
		public void Insert(int index, XRTableRow row) {
			if(List.IndexOf(row) < 0) {
				BeginInit();
				try {
					List.Insert(index, row);
				} finally {
					EndInit();
				}
			}
		}
		internal void BeginInit() {
			if(Table != null && !IsLoading) {
				Table.BeginInit();
				tableSuspended = true;
			}
		}
		internal void EndInit() {
			if(IsLoading && tableSuspended)
				Table.EndInit();
			tableSuspended = false;
		}
		#region IEnumerable<IXRTableItemWeighty> Members
		IEnumerator<IWeighty> IEnumerable<IWeighty>.GetEnumerator() {
			return new XRTableRowEnumerator(this);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return (this as IEnumerable<IWeighty>).GetEnumerator();
		}
		#endregion
		#region IEnumerable<XRTableRow> Members
		IEnumerator<XRTableRow> IEnumerable<XRTableRow>.GetEnumerator() {
			return (this as IEnumerable<IWeighty>).GetEnumerator() as IEnumerator<XRTableRow>;
		}
		#endregion
		public class XRTableRowEnumerator : IEnumerator<IWeighty>, IEnumerator<XRTableRow> {
			#region Fields
			IEnumerator innerEnumerator;
			#endregion
			public XRTableRowEnumerator(XRTableRowCollection collection) {
				this.innerEnumerator = collection.GetEnumerator();
			}
			#region IEnumerator<IXRTableItemWeighty> Members
			IWeighty IEnumerator<IWeighty>.Current {
				get { return (this as IEnumerator).Current as IWeighty; }
			}
			#endregion
			#region IDisposable Members
			void IDisposable.Dispose() { innerEnumerator = null; }
			#endregion
			#region IEnumerator Members
			object IEnumerator.Current {
				get { return innerEnumerator.Current; }
			}
			bool IEnumerator.MoveNext() { return innerEnumerator.MoveNext(); }
			void IEnumerator.Reset() { innerEnumerator.Reset(); }
			#endregion
			#region IEnumerator<XRTableRow> Members
			XRTableRow IEnumerator<XRTableRow>.Current {
				get { return innerEnumerator.Current as XRTableRow; }
			}
			#endregion
		}
	}
}
