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
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region DisplayObjectsOptions
	public enum DisplayObjectsOptions {
		ShowAll = 0x00,
		ShowPlaceholders = 0x01,
		HideAll = 0x02
	}
	#endregion
	#region DisplayCommentsOptions
	public enum DisplayCommentsOptions {
		None = 0x00,
		CommentIndicatorOnly = 0x01,
		CommentAndIndicator = 0x02
	}
	#endregion
	#region ViewPaneType
	public enum ViewPaneType {
		TopLeft,
		BottomLeft,
		BottomRight,
		TopRight
	}
	#endregion
	#region ViewSplitState
	public enum ViewSplitState {
		Split,
		Frozen,
		FrozenSplit,
	}
	#endregion
	#region SheetViewType
	public enum SheetViewType {
		Normal = 0x00,
		PageBreakPreview = 0x01,
		PageLayout = 0x02
	}
	#endregion
	public interface IGridlinesColorAccessor {
		Color GridlinesColor { get; }
	}
	#region ModelWorksheetView
	public class ModelWorksheetView : ModelWorksheetViewInfo, IGridlinesColorAccessor {
		static readonly ModelWorksheetViewInfo defaultItem = new ModelWorksheetView();
		public static ModelWorksheetViewInfo DefaultItem { get { return defaultItem; } }
		#region Fields
		readonly DocumentModel documentModel;
		int workbookViewId;
		DefaultGridBorder defaultGridBorder;
		EmptyBorder emptyBorder;
		PrintGridBorder printGridBorder;
		#endregion
		ModelWorksheetView() {
			base.ShowStartupTaskPane = true;
			base.ShowFormulaBar = true;
			base.ShowStatusBar = true;
			base.ShowWindowsInTaskBar = true;
			base.ShowGridlines = true;
			base.ShowRowColumnHeaders = true;
			base.ShowOutlineSymbols = true;
			base.ShowZeroValues = true;
			base.ShowHorizontalScrollBars = true;
			base.ShowVerticalScrollBars = true;
			base.ShowSheetTabs = true;
			base.ShowRuler = true;
			base.ShowWhiteSpace = true;
			base.ZoomScale = 100;
		}
		public ModelWorksheetView(DocumentModel workbook)
			: this() {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.documentModel = workbook;
			this.defaultGridBorder = new DefaultGridBorder(workbook, this);
			this.emptyBorder = new EmptyBorder(workbook);
			this.printGridBorder = new PrintGridBorder(workbook, this);
		}
		#region Properties
		protected internal int WorkbookViewId { get { return workbookViewId; } set { workbookViewId = value; } }
		public DefaultGridBorder DefaultGridBorder { get { return defaultGridBorder; } }
		public EmptyBorder EmptyBorder { get { return emptyBorder; } }
		public PrintGridBorder PrintGridBorder { get { return printGridBorder; } }
		Color IGridlinesColorAccessor.GridlinesColor { get { return base.GridlinesColor; } }
		#region ShowGridlines
		public override bool ShowGridlines {
			get { return base.ShowGridlines; }
			set {
				if (ShowGridlines == value)
					return;
				base.ShowGridlines = value;
				ApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI);
			}
		}
		#endregion
		#region ShowFormulas
		public override bool ShowFormulas {
			get { return base.ShowFormulas; }
			set {
				if (ShowFormulas == value)
					return;
				base.ShowFormulas = value;
				ApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.RaiseUpdateUI);
			}
		}
		#endregion
		#region ShowRowColumnHeaders
		public override bool ShowRowColumnHeaders {
			get { return base.ShowRowColumnHeaders; }
			set {
				if (ShowRowColumnHeaders == value)
					return;
				base.ShowRowColumnHeaders = value;
				ApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetHeaderLayout | DocumentModelChangeActions.RaiseUpdateUI);
			}
		}
		#endregion
		#region ZoomScale
		public override int ZoomScale {
			get { return base.ZoomScale; }
			set {
				if (ZoomScale == value)
					return;
				base.ZoomScale = value;
				ApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetHeaderLayout);
			}
		}
		#endregion
		#region ZoomScaleNormal
		public override int ZoomScaleNormal {
			get { return base.ZoomScaleNormal; }
			set {
				if (ZoomScaleNormal == value)
					return;
				base.ZoomScaleNormal = value;
				ApplyChanges(DocumentModelChangeActions.ResetAllLayout | DocumentModelChangeActions.ResetHeaderLayout);
			}
		}
		#endregion
		#endregion
		void ApplyChanges(DocumentModelChangeActions changeActions) {
			documentModel.BeginUpdate();
			try {
				documentModel.ApplyChanges(changeActions);
				documentModel.IncrementTransactionVersion();
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		public void BeginUpdate() {
			documentModel.BeginUpdate();
		}
		public void EndUpdate() {
			documentModel.EndUpdate();
		}
		public void CopyFrom(ModelWorksheetView obj) {
			base.CopyFrom(obj);
			this.WorkbookViewId = obj.WorkbookViewId;
		}
		public CellPosition GetSplitPosition() {
			if (SplitState == ViewSplitState.Split)
				return new CellPosition(0, 0);
			else
				return new CellPosition(this.HorizontalSplitPosition, this.VerticalSplitPosition);
		}
		public void SetSplitTopLeftCell(CellPosition position) {
			this.VerticalSplitPosition = position.Row - TopLeftCell.Row;
			this.HorizontalSplitPosition = position.Column - TopLeftCell.Column;
			this.SplitState = ViewSplitState.Frozen;
			this.SplitTopLeftCell = position;
		}
		protected internal CellPosition GetSplitPosition(Worksheet sheet) {
			if(SplitState == ViewSplitState.Split) {
				int column = CalcRowColIndex(sheet, this.HorizontalSplitPosition, false);
				int row = CalcRowColIndex(sheet, this.VerticalSplitPosition, true);
				return new CellPosition(column, row);
			}
			else
				return new CellPosition(this.HorizontalSplitPosition, this.VerticalSplitPosition);
		}
		protected internal CellPosition GetTopLeftCell() {
			return TopLeftCell;
		}
		int CalcRowColIndex(Worksheet sheet, int splitPosition, bool isRow) {
			int result = 0;
			int offset = isRow ? sheet.ActiveView.TopLeftCell.Row : sheet.ActiveView.TopLeftCell.Column;
			int maxValue = isRow ? sheet.MaxRowCount : sheet.MaxColumnCount;
			if(splitPosition > 0) {
				AccumulatedOffset accumulatedOffset = new AccumulatedOffset(sheet);
				do {
					if(result + offset >= maxValue) {
						result--;
						break;
					}
					accumulatedOffset.AddCell(result + offset, isRow);
					if(splitPosition < accumulatedOffset.ToModel())
						break;
					result++;
				}
				while(true);
			}
			return result;
		}
		internal void UpdateGridlineColor() {
			this.defaultGridBorder = new DefaultGridBorder(documentModel, this);
		}
	}
	#endregion
	#region ModelWorksheetViewInfo
	public class ModelWorksheetViewInfo : 
		ICloneable<ModelWorksheetViewInfo>, 
		ISupportsCopyFrom<ModelWorksheetViewInfo>, 
		ISupportsSizeOf 
	{
		#region Fields
		const uint MaskViewType = 0x00000003;					   
		const uint MaskActivePaneType = 0x0000000C;				 
		const uint MaskSplitState = 0x00000030;					 
		const uint MaskDisplayComments = 0x000000C0;				
		const uint MaskRightToLeft = 0x00000100;					
		const uint MaskShowStartupTaskPane = 0x00000200;			
		const uint MaskShowFormulaBar = 0x00000400;				 
		const uint MaskShowStatusBar = 0x00000800;				  
		const uint MaskShowWindowsInTaskBar = 0x00001000;		   
		const uint MaskShowPageBreaks = 0x00002000;				 
		const uint MaskShowFormulas = 0x00004000;				   
		const uint MaskShowGridlines = 0x00008000;				  
		const uint MaskShowRowColumnHeaders = 0x00010000;		   
		const uint MaskShowOutlineSymbols = 0x00020000;			 
		const uint MaskShowZeroValues = 0x00040000;				 
		const uint MaskShowHorizontalScrollBars = 0x00080000;	   
		const uint MaskShowVerticalScrollBars = 0x00100000;		 
		const uint MaskSynchronizeHorizontalScrolling = 0x00200000; 
		const uint MaskSynchronizeVerticalScrolling = 0x00400000;   
		const uint MaskShowSheetTabs = 0x00800000;				  
		const uint MaskWindowProtection = 0x01000000;			   
		const uint MaskTabSelected = 0x02000000;					
		const uint MaskShowRuler = 0x04000000;					  
		const uint MaskShowWhiteSpace = 0x08000000;				 
		uint packedValues;
		int zoomScale;
		int zoomScaleNormal;
		int zoomScaleSheetLayoutView;
		int zoomScalePageLayoutView;
		int horizontalSplitPosition;
		int verticalSplitPosition;
		CellPosition splitTopLeftCell;
		Color gridlinesColor;
		CellPosition topLeftCell;
		#endregion
		#region Properties
		public bool RightToLeft { get { return GetBooleanVal(MaskRightToLeft); } set { SetBooleanVal(MaskRightToLeft, value); } }
		public bool ShowStartupTaskPane { get { return GetBooleanVal(MaskShowStartupTaskPane); } set { SetBooleanVal(MaskShowStartupTaskPane, value); } }
		public bool ShowFormulaBar { get { return GetBooleanVal(MaskShowFormulaBar); } set { SetBooleanVal(MaskShowFormulaBar, value); } }
		public bool ShowStatusBar { get { return GetBooleanVal(MaskShowStatusBar); } set { SetBooleanVal(MaskShowStatusBar, value); } }
		public bool ShowWindowsInTaskBar { get { return GetBooleanVal(MaskShowWindowsInTaskBar); } set { SetBooleanVal(MaskShowWindowsInTaskBar, value); } }
		public bool ShowPageBreaks { get { return GetBooleanVal(MaskShowPageBreaks); } set { SetBooleanVal(MaskShowPageBreaks, value); } }
		public virtual bool ShowFormulas { get { return GetBooleanVal(MaskShowFormulas); } set { SetBooleanVal(MaskShowFormulas, value); } }
		public virtual bool ShowGridlines { get { return GetBooleanVal(MaskShowGridlines); } set { SetBooleanVal(MaskShowGridlines, value); } }
		public virtual bool ShowRowColumnHeaders { get { return GetBooleanVal(MaskShowRowColumnHeaders); } set { SetBooleanVal(MaskShowRowColumnHeaders, value); } }
		public bool ShowOutlineSymbols { get { return GetBooleanVal(MaskShowOutlineSymbols); } set { SetBooleanVal(MaskShowOutlineSymbols, value); } }
		public bool ShowZeroValues { get { return GetBooleanVal(MaskShowZeroValues); } set { SetBooleanVal(MaskShowZeroValues, value); } }
		public bool ShowHorizontalScrollBars { get { return GetBooleanVal(MaskShowHorizontalScrollBars); } set { SetBooleanVal(MaskShowHorizontalScrollBars, value); } }
		public bool ShowVerticalScrollBars { get { return GetBooleanVal(MaskShowVerticalScrollBars); } set { SetBooleanVal(MaskShowVerticalScrollBars, value); } }
		public bool SynchronizeHorizontalScrolling { get { return GetBooleanVal(MaskSynchronizeHorizontalScrolling); } set { SetBooleanVal(MaskSynchronizeHorizontalScrolling, value); } }
		public bool SynchronizeVerticalScrolling { get { return GetBooleanVal(MaskSynchronizeVerticalScrolling); } set { SetBooleanVal(MaskSynchronizeVerticalScrolling, value); } }
		public bool ShowSheetTabs { get { return GetBooleanVal(MaskShowSheetTabs); } set { SetBooleanVal(MaskShowSheetTabs, value); } }
		public bool WindowProtection { get { return GetBooleanVal(MaskWindowProtection); } set { SetBooleanVal(MaskWindowProtection, value); } }
		public bool ShowRuler { get { return GetBooleanVal(MaskShowRuler); } set { SetBooleanVal(MaskShowRuler, value); } }
		public bool TabSelected { get { return GetBooleanVal(MaskTabSelected); } set { SetBooleanVal(MaskTabSelected, value); } }
		public bool ShowWhiteSpace { get { return GetBooleanVal(MaskShowWhiteSpace); } set { SetBooleanVal(MaskShowWhiteSpace, value); } }
		public SheetViewType ViewType {
			get { return (SheetViewType)(packedValues & MaskViewType); }
			set {
				packedValues &= ~MaskViewType;
				packedValues |= (uint)value;
			}
		}
		public ViewPaneType ActivePaneType {
			get { return (ViewPaneType)((packedValues & MaskActivePaneType) >> 2); }
			set {
				packedValues &= ~MaskActivePaneType;
				packedValues |= ((uint)value << 2);
			}
		}
		public ViewSplitState SplitState {
			get { return (ViewSplitState)((packedValues & MaskSplitState) >> 4); }
			set {
				packedValues &= ~MaskSplitState;
				packedValues |= ((uint)value << 4);
			}
		}
		public DisplayCommentsOptions DisplayComments {
			get { return (DisplayCommentsOptions)((packedValues & MaskDisplayComments) >> 6); }
			set {
				packedValues &= ~MaskDisplayComments;
				packedValues |= ((uint)value << 6);
			}
		}
		public Color GridlinesColor { get { return gridlinesColor; } set { gridlinesColor = value; } }
		public virtual int ZoomScale { get { return zoomScale; } set { zoomScale = value; } }
		public virtual int ZoomScaleNormal { get { return zoomScaleNormal; } set { zoomScaleNormal = value; } }
		public int ZoomScaleSheetLayoutView { get { return zoomScaleSheetLayoutView; } set { zoomScaleSheetLayoutView = value; } }
		public int ZoomScalePageLayoutView { get { return zoomScalePageLayoutView; } set { zoomScalePageLayoutView = value; } }
		public int HorizontalSplitPosition { get { return horizontalSplitPosition; } set { horizontalSplitPosition = value; } }
		public int VerticalSplitPosition { get { return verticalSplitPosition; } set { verticalSplitPosition = value; } }
		public CellPosition TopLeftCell {
			get { return topLeftCell; }
			set {
				if (value.ColumnType == PositionType.Absolute || value.RowType == PositionType.Absolute)
					value = new CellPosition(value.Column, value.Row);
				topLeftCell = value;
			}
		}
		protected internal CellPosition SplitTopLeftCell {
			get { return splitTopLeftCell; }
			set {
				if(value.ColumnType == PositionType.Absolute || value.RowType == PositionType.Absolute)
					value = new CellPosition(value.Column, value.Row);
				splitTopLeftCell = value;
			}
		}
		public CellPosition ScrolledTopLeftCell {
			get { return IsFrozen() ? splitTopLeftCell : topLeftCell; }
			set { SetScrolledPosition(value); }
		}
		void SetScrolledPosition(CellPosition value) {
			if (IsFrozen()) {
				if (horizontalSplitPosition == 0 || verticalSplitPosition == 0) {
					if (verticalSplitPosition == 0)
						topLeftCell = new CellPosition(topLeftCell.Column, value.Row);
					if(horizontalSplitPosition == 0)
						topLeftCell = new CellPosition(value.Column, topLeftCell.Row);
				}
				int validColumnIndex = Math.Max(value.Column, topLeftCell.Column + horizontalSplitPosition);
				int validRowIndex = Math.Max(value.Row, topLeftCell.Row + verticalSplitPosition);
				SplitTopLeftCell = new CellPosition(validColumnIndex, validRowIndex);
			}
			else TopLeftCell = value;
		}
		protected internal CellPosition FrozenCell {
			get { return new CellPosition(horizontalSplitPosition != 0 ? topLeftCell.Column + horizontalSplitPosition : 0, verticalSplitPosition != 0 ? topLeftCell.Row + verticalSplitPosition: 0); }
		}
		#endregion
		public bool IsFrozen() {
			ModelWorksheetViewInfo item = ModelWorksheetView.DefaultItem;
			return (SplitState == ViewSplitState.Frozen || SplitState == ViewSplitState.FrozenSplit) &&
				(HorizontalSplitPosition != item.HorizontalSplitPosition || VerticalSplitPosition != item.VerticalSplitPosition);
		}
		public bool IsOnlyColumnsFrozen() {
			ModelWorksheetViewInfo item = ModelWorksheetView.DefaultItem;
			return (SplitState == ViewSplitState.Frozen || SplitState == ViewSplitState.FrozenSplit) &&
				HorizontalSplitPosition != item.HorizontalSplitPosition && VerticalSplitPosition == item.VerticalSplitPosition;
		}
		public bool IsOnlyRowsFrozen() {
			ModelWorksheetViewInfo item = ModelWorksheetView.DefaultItem;
			return (SplitState == ViewSplitState.Frozen || SplitState == ViewSplitState.FrozenSplit) && 
				VerticalSplitPosition != item.VerticalSplitPosition && HorizontalSplitPosition == item.HorizontalSplitPosition;
		}
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		public ModelWorksheetViewInfo Clone() {
			ModelWorksheetViewInfo result = new ModelWorksheetViewInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ModelWorksheetViewInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
			this.GridlinesColor = value.GridlinesColor;
			this.ZoomScale = value.ZoomScale;
			this.ZoomScaleNormal = value.ZoomScaleNormal;
			this.ZoomScaleSheetLayoutView = value.ZoomScaleSheetLayoutView;
			this.ZoomScalePageLayoutView = value.ZoomScalePageLayoutView;
			this.TopLeftCell = value.TopLeftCell;
			this.HorizontalSplitPosition = value.HorizontalSplitPosition;
			this.VerticalSplitPosition = value.VerticalSplitPosition;
			this.SplitTopLeftCell = value.SplitTopLeftCell;
		}
		public override bool Equals(object obj) {
			ModelWorksheetViewInfo other = obj as ModelWorksheetViewInfo;
			if (other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.GridlinesColor == other.GridlinesColor &&
				this.ZoomScale == other.ZoomScale &&
				this.ZoomScaleNormal == other.ZoomScaleNormal &&
				this.ZoomScaleSheetLayoutView == other.ZoomScaleSheetLayoutView &&
				this.ZoomScalePageLayoutView == other.ZoomScalePageLayoutView &&
				this.HorizontalSplitPosition == other.HorizontalSplitPosition &&
				this.VerticalSplitPosition == other.VerticalSplitPosition &&
				this.TopLeftCell.EqualsPosition(other.TopLeftCell) &&
				this.SplitTopLeftCell.EqualsPosition(other.SplitTopLeftCell);
		}
		public override int GetHashCode() {
			var result = new Office.Utils.CombinedHashCode();
			result.AddInt((int)this.packedValues);
			result.AddInt(GridlinesColor.GetHashCode());
			result.AddInt(ZoomScale);
			result.AddInt(ZoomScaleNormal);
			result.AddInt(ZoomScaleSheetLayoutView);
			result.AddInt(ZoomScalePageLayoutView);
			result.AddInt(HorizontalSplitPosition);
			result.AddInt(VerticalSplitPosition);
			result.AddInt(TopLeftCell.GetHashCode());
			result.AddInt(SplitTopLeftCell.GetHashCode());
			return result.CombinedHash32;
		}
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
	}
	#endregion
}
