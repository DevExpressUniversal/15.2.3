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

using System.Drawing;
using DevExpress.XtraPrinting;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System;
using DevExpress.XtraReports.Native;
using System.Collections.Generic;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(false),
	DesignTimeVisible(false),
	XRDesigner("DevExpress.XtraReports.Design.XRTableCellDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRTableCellDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	DesignerSerializer("DevExpress.XtraReports.Design.XRControlCodeDomSerializer," + AssemblyInfo.SRAssemblyReportsExtensionsFull, AttributeConstants.CodeDomSerializer),
	DefaultBindableProperty("Text"),
	DefaultProperty("Text"),
	ToolboxBitmap(typeof(ResFinder), "Images.XRTableCell.bmp"),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableCell", "TableCell"),
	]
	public class XRTableCell : XRLabel, IBrickOwner, IWeighty {
		#region Fields & Properties
		double? weight;
		SuspendType suspendType = SuspendType.None;
		protected internal override XRControl OverlappingParent { get { return Row != null ? Row.OverlappingParent : null; } }
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
	DevExpressXtraReportsLocalizedDescription("XRTableCellWidthF"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(true),
		SRCategory(ReportStringId.CatLayout),
		CategoryAttribute("Layout"),
		]
		public override float WidthF {
			get {
				return fBounds.Width;
			}
			set {
				if(base.WidthF != value || IsLoading)
					base.WidthF = value;
			}
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellBoundsF")]
#endif
		public override RectangleF BoundsF {
			get { return new RectangleF(LeftF, TopF, WidthF, HeightF); }
			set { base.BoundsF = value; }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellLeftF")]
#endif
		public override float LeftF {
			get {
				if(Row == null || IsLoading)
					return fBounds.Left;
				float left = 0;
				foreach(XRControl item in Parent.Controls) {
					if(ReferenceEquals(item, this)) break;
					left += item.WidthF;
				}
				return left;
			}
			set { }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellTopF")]
#endif
		public override float TopF {
			get { return 0; }
			set { }
		}
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellHeightF")]
#endif
		public override float HeightF {
			get {
				if(Row != null)
					return Row.HeightF;
				return fBounds.Height;
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
			get {
				if(Table != null && Table.AnchorVertical == VerticalAnchorStyles.Both)
					return VerticalAnchorStyles.Both;
				return VerticalAnchorStyles.None;
			}
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
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellCanHaveChildren")]
#endif
		public override bool CanHaveChildren {
			get { return true; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XRTableRow Row {
			get { return Parent as XRTableRow; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableCellCanGrow"),
#endif
		TypeConverter(typeof(DevExpress.XtraReports.Design.CellCanGrowCanShrinkConverter)),
		]
		public override bool CanGrow {
			get {
				return Table == null ? fCanGrow :
					fCanGrow && Table.AnchorAllowsShrinkGrow;
			}
			set { fCanGrow = value; }
		}
		[
		TypeConverter(typeof(DevExpress.XtraReports.Design.CellCanGrowCanShrinkConverter)),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableCellCanShrink"),
#endif
		]
		public override bool CanShrink {
			get {
				return Table == null ? fCanShrink :
					fCanShrink && Table.AnchorAllowsShrinkGrow;
			}
			set { fCanShrink = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public new bool AutoWidth { get { return false; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XRTableCell PreviousCell {
			get { return Index < 1 ? null : Row.Cells[Index - 1]; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public XRTableCell NextCell {
			get { return Index >= Row.Cells.Count - 1 ? null : Row.Cells[Index + 1]; }
		}
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableCellRowSpan"),
#endif
		DXDisplayNameAttribute(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableCell.RowSpan"),
		Browsable(true),
		DefaultValue(1),
		EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override int RowSpan {
			get { return base.RowSpan; }
			set { 
				base.RowSpan = value;
				if(Table != null) {
					Table.UpdateHasSpanningCells();
				}
			}
		}
		protected XRTable Table {
			get { return (Row != null) ? Row.Table : null; }
		}
		protected override bool CanChangeZOrder {
			get { return false; }
		}
		protected override bool NeedCalcContainerHeight { get { return HasVisibleChildren; } }
		internal RectangleF RealBoundsF {
			get {
				float left = LeftF;
				float width;
				if(weight == null || Row == null || IsLoading)
					width = fBounds.Width;
				else if(NextCell == null)
					width = Row.WidthF - left;
				else
					width = (float)WeightHelper.GetAmount(Row, weight.Value);
				return new RectangleF(left, TopF, width, HeightF);
			}
		}
		internal override bool BoundsChanging {
			get { return false; }
			set { }
		}
		internal override BorderSide VisibleContourBorders { get { return GetVisibleContourBorders(); } }
		bool IsLoading {
			get { return Row != null && Row.Table != null && Row.Table.IsLoading; }
		}
		#endregion
		public XRTableCell() {
		}
		internal XRTableCell(float width) {
			WidthF = width;
		}
		protected internal override bool HasPrintingWarning() {
			return false;
		}
		protected override bool IntersectsWithSiblings(int digits) {
			return false;
		}
		protected override bool IntersectsWithParent(int digits) {
			return false;
		}
		protected internal override XRControlStyle GetEffectiveXRStyle() {
			XRControlStyle effectiveStyle = base.GetEffectiveXRStyle();
			if(effectiveStyle != fStyle)
				effectiveStyle.SetBaseBorders(CorrectBorders(effectiveStyle.Borders));
			return effectiveStyle;
		}
#if DEBUGTEST
		public
#endif
		BorderSide CorrectBorders(BorderSide borderSide) {
			int index;
			if(Row != null) {
				index = Index;
				if(index > 0 && (Row.Cells[index - 1].GetEffectiveBorders() & DevExpress.XtraPrinting.BorderSide.Right) != 0 && Row.Cells[index - 1].Visible == true)
					borderSide &= (~DevExpress.XtraPrinting.BorderSide.Left);
			}
			if(Table != null) {
				index = Row.Index;
				while(index > 0 && !Table.Rows[index - 1].Visible)
					index--;
				if(index > 0 && (Table.Rows[index - 1].GetEffectiveBorders() & DevExpress.XtraPrinting.BorderSide.Bottom) != 0)
					borderSide &= (~DevExpress.XtraPrinting.BorderSide.Top);
			}
			return borderSide;
		}
		protected internal override void SwapWith(XRControl item) {
			Table.BeginInit();
			try {
				base.SwapWith(item);
			} finally {
				Table.EndInit();
			}
		}
		protected override BorderSide GetCorrectBorders() {
			return CorrectBorders(GetEffectiveBorders());
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBounds(x, y, width, height, specified, ResizeBehaviour.DefaultMode);
		}
		internal void SetProportionalBounds(float x, float y, float width, float height, BoundsSpecified specified){
			SetBounds(x, y, width, height, specified, ResizeBehaviour.ProportionalMode);
		}
		internal void SetSpecifiedBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			SetBounds(x, y, width, height, specified, ResizeBehaviour.SpecifiedMode);
		}
		void SetBounds(float x, float y, float width, float height, BoundsSpecified specified, ResizeBehaviour resizeMode) {
			if(XRControl.SpecifiedEquals(specified, BoundsSpecified.X, BoundsSpecified.Y, BoundsSpecified.Location))
				return;
			if(XRControl.SpecifiedEquals(specified, BoundsSpecified.Height, BoundsSpecified.Size) && Row != null)
				height = Row.HeightF;
			if(!IsLoading && weight != null) {
				BeginInit();				
				switch(resizeMode) {
					case ResizeBehaviour.ProportionalMode :
						UpdateProportionalWeight(x, width);
						break;
					case ResizeBehaviour.SpecifiedMode :
						UpdateSpecifiedWeight(x, width);
						break;
					case ResizeBehaviour.DefaultMode :
						UpdateWeight(x, width);
						break;
				}
				SetBoundsCore(x, y, WidthF, height, specified);
				EndInit();
			} else {
				if(IsLoading && weight != null)
					weight = null;
				SetBoundsCore(x, y, width, height, specified);
			}
		}
		protected override void ValidateBrick(VisualBrick brick, RectangleF bounds, PointF offset) {
			if(brick.Bricks.Count == 0)
				base.ValidateBrick(brick, bounds, offset);
		}
		protected internal override void UpdateBrickBounds(VisualBrick brick) {
			if(brick is PanelBrick)
				return;
			base.UpdateBrickBounds(brick);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(childrenBricks.Length > 0) {
				PanelBrick brick = new PanelBrick(this);
				foreach(VisualBrick childBrick in childrenBricks)
					brick.Bricks.Add(childBrick);
				return brick;
			}
			return base.CreateBrick(childrenBricks);
		}
		protected override VisualBrick GetPrintableBrick(XRWriteInfo writeInfo) {
			VisualBrick brick = base.GetPrintableBrick(writeInfo);
			RowSpanHelper.MergeCells(this, writeInfo.MergedCells);
			object key;
			if(writeInfo.MergedCells.TryGetValue(this, out key)) {
				brick.SetAttachedValue(BrickAttachedProperties.MergeValue, key);
				if(RowSpan > 1) {
					brick.SetBoundsHeight(RowSpanHelper.CalculateMergedCellHeight(this, writeInfo.MergedCells), Dpi);
				}
			}
			return brick;
		}
		protected internal override VisualBrick GetDesignerBrick(PrintingSystemBase ps, XRWriteInfo writeInfo) {
			VisualBrick brick = base.GetDesignerBrick(ps, writeInfo); 
			RowSpanHelper.MergeCells(this, writeInfo.MergedCells);
			return brick;
		}
		protected override void SetParent(XRControl value) {
			if(Parent != value) {
				if(value != null)
					((XRTableRow)value).Cells.Add(this);
			}
		}
		bool HasVisibleChildren {
			get {
				if(HasChildren)
					foreach(XRControl control in fXRControls)
						if(control.Visible)
							return true;
				return false;
			}
		}
		void UpdateProportionalWeight(float x, float width) {
			bool leftItems = Math.Abs(LeftF - x) > 0.0001f;
			List<XRTableCell> neighbourItems = new List<XRTableCell>();
			foreach(XRTableCell cell in this.Row.Cells) {
				if((leftItems && cell.Index < this.Index) || (!leftItems && cell.Index > this.Index)) neighbourItems.Add(cell);
			}
			WeightHelper.ResizeProportionalItem(this, neighbourItems, neighbourItems.Count, LeftF, x, WidthF, width, GetMinimumWidth());
		}
		void UpdateSpecifiedWeight(float x, float width) {
			WeightHelper.ResizeSpecifiedItem(this, LeftF, x, WidthF, width, GetMinimumWidth());
		}
		void UpdateWeight(float x, float width) {
			WeightHelper.Resize(this, LeftF, x, WidthF, width, GetMinimumWidth());
		}
		internal float GetMaxAvailableWidth(float x, float width) {
			return WeightHelper.GetMaxAvailableWidth(this, LeftF, x, WidthF, width, GetMinimumWidth());
		}
		void BeginInit() {
			if(Row == null)
				return;
			if(Row.Table != null && !IsLoading) {
				suspendType = SuspendType.TableInitialize;
				Row.Table.BeginInit();
			} else if(!Row.Suspended) {
				Row.SuspendLayout();
				suspendType = SuspendType.RowLayout;
			} else
				suspendType = SuspendType.None;
		}
		void EndInit() {
			if(Row == null)
				return;
			if(Row.Table != null && suspendType == SuspendType.TableInitialize)
				Row.Table.EndInit();
			else if(Row.Suspended && suspendType == SuspendType.RowLayout)
				Row.ResumeLayout();
			suspendType = SuspendType.None;
		}
		BorderSide GetVisibleContourBorders() {
			BorderSide borders = BorderSide.Left | BorderSide.Top;
			if(Row.Cells[Row.Cells.Count - 1] == this)
				borders = borders | BorderSide.Right;
			if(Row.Table.Rows.LastRow == Row)
				borders = borders | BorderSide.Bottom;
			return borders;
		}
		protected override BrickOwnerType BrickOwnerType {
			get {
				return BrickOwnerType.TableCell;
			}
		}
		protected override ControlLayoutRules LayoutRules {
			get {
				return ControlLayoutRules.LeftSizeable | ControlLayoutRules.RightSizeable;
			}
		}
		protected override string ControlsUnityName {
			get {
				return Table != null ? Table.Name : null;
			}
		}
		#region IWeighty
		IWeightyContainer IWeighty.Parent {
			get { return Row; }
		}
		float IWeighty.Amount {
			get { return WidthF; }
		}
		double IWeighty.Weight {
			get {
				if(weight == null)
					return 0.0;
				return weight.Value;
			}
			set {
				weight = value;
				if(Row == null)
					return;
				if(!IsLoading) {
					BeginInit();
					SetBoundsCore(0f, 0f, WidthF, 0f, BoundsSpecified.Width);
					EndInit();
				}
			}
		}
		IWeighty IWeighty.Next {
			get { return NextCell; }
		}
		IWeighty IWeighty.Previous {
			get { return PreviousCell; }
		}
		#endregion
		internal enum SuspendType : byte {
			None,
			TableInitialize,
			RowLayout,
		}
	}
	public class XRTableCellCollection : XRControlCollection, IEnumerable<IWeighty>, IEnumerable<XRTableCell> {
		#region Properties and  Fields
		XRTableCell.SuspendType suspendType = XRTableCell.SuspendType.None;
		public new XRTableCell this[int index] { get { return List[index] as XRTableCell; } }
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRTableCellCollectionItem")]
#endif
		protected XRTableRow Row { get { return owner as XRTableRow; } }
		bool IsLoading { get { return Row != null && Row.Table != null && Row.Table.IsLoading; } }
		#endregion
		public XRTableCellCollection(XRTableRow owner)
			: base(owner) {
		}
		internal void Insert(int index, XRTableCell cell) {
			if(List.IndexOf(cell) < 0)
				List.Insert(index, cell);
		}
		public void AddRange(XRTableCell[] cells) {
			BeginInit();
			try {
				base.AddRange(cells);
			} finally {
				EndInit();
			}
		}
		public int Add(XRTableCell cell) {
			BeginInit();
			try {
				return base.Add(cell);
			} finally {
				EndInit();
			}
		}
		protected override void OnInsert(int index, object value) {
			if(!(value is XRTableCell))
				throw new ArgumentException();
			base.OnInsert(index, value);
		}
		protected override void OnInsertCompleteCore(int index, object value) {
			base.OnInsertCompleteCore(index, value);
			XRTableCell cell = (XRTableCell)value;
			AddToContainer(cell);
		}
		protected override void OnRemoveCompleteCore(int index, object value) {
			base.OnRemoveCompleteCore(index, value);
			if(Row != null)
				Row.ArrangeCells();
		}
		void BeginInit() {
			if(Row == null)
				return;
			if(Row.Table != null && !IsLoading) {
				suspendType = XRTableCell.SuspendType.TableInitialize;
				Row.Table.BeginInit();
			} else if(!Row.Suspended) {
				Row.SuspendLayout();
				suspendType = XRTableCell.SuspendType.RowLayout;
			} else
				suspendType = XRTableCell.SuspendType.None;
		}
		void EndInit() {
			if(Row == null)
				return;
			if(Row.Table != null && suspendType == XRTableCell.SuspendType.TableInitialize)
				Row.Table.EndInit();
			else if(Row.Suspended && suspendType == XRTableCell.SuspendType.RowLayout)
				Row.ResumeLayout();
			suspendType = XRTableCell.SuspendType.None;
		}
		#region IEnumerable<IXRTableItemWeighty> Members
		IEnumerator<IWeighty> IEnumerable<IWeighty>.GetEnumerator() {
			return new XRTableCellEnumerator(this);
		}
		#endregion
		#region IEnumerable Members
		IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return (this as IEnumerable<IWeighty>).GetEnumerator();
		}
		#endregion
		#region IEnumerable<XRTableCell> Members
		IEnumerator<XRTableCell> IEnumerable<XRTableCell>.GetEnumerator() {
			return (this as IEnumerable<IWeighty>).GetEnumerator() as IEnumerator<XRTableCell>;
		}
		#endregion
		public class XRTableCellEnumerator : IEnumerator<IWeighty>, IEnumerator<XRTableCell> {
			#region Fields
			IEnumerator innerEnumerator;
			#endregion
			public XRTableCellEnumerator(XRTableCellCollection collection) {
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
			#region IEnumerator<XRTableCell> Members
			XRTableCell IEnumerator<XRTableCell>.Current {
				get { return (this as IEnumerator).Current as XRTableCell; }
			}
			#endregion
		}
	}
}
