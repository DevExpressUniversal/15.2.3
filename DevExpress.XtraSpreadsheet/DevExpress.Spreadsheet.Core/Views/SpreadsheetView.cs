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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet {
	#region SpreadsheetView (abstract class)
#if !SL
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	[ComVisible(true)]
	public abstract partial class SpreadsheetView : IDisposable {
		#region Fields
		Color backColor = DXSystemColors.Window;
		bool isDisposed;
		ISpreadsheetControl control;
		Rectangle bounds;
		const float minZoomFactor = 0.1f;
		const float maxZoomFactor = 10000f;
		const float defaultZoomFactor = 1.0f;
		const int minWidth = 5;
		const int minHeight = 5;
		SpreadsheetViewVerticalScrollController verticalScrollController;
		SpreadsheetViewHorizontalScrollController horizontalScrollController;
		SelectionLayout selectionLayout;
		CommentLayout commentLayout;
		AutoFilterLayout autoFilterLayout;
		DataValidationLayout dataValidationLayout;
		PivotTableLayout pivotTableLayout;
		bool allowCalculateHeaderHitTest;
		bool allowCalculateGroupHitTest;
		#endregion
		protected SpreadsheetView(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.Bounds = new Rectangle(0, 0, MinWidth, MinHeight);
			this.selectionLayout = new SelectionLayout(this);
			this.commentLayout = new CommentLayout(this);
			this.autoFilterLayout = new AutoFilterLayout(this);
			this.dataValidationLayout = new DataValidationLayout(this);
			this.pivotTableLayout = new PivotTableLayout(this);
			this.allowCalculateHeaderHitTest = true;
			this.allowCalculateGroupHitTest = true;
			RecreateScrollControllers();
		}
		#region Properties
		#region Control
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		public ISpreadsheetControl Control { get { return control; } }
		public SelectionLayout SelectionLayout { get { return selectionLayout; } }
		public CommentLayout CommentLayout { get { return commentLayout; } }
		public AutoFilterLayout AutoFilterLayout { get { return autoFilterLayout; } }
		public DataValidationLayout DataValidationLayout { get { return dataValidationLayout; } }
		public PivotTableLayout PivotTableLayout { get { return pivotTableLayout; } }
		#endregion
		#region Type
		[Browsable(false)]
		public abstract SpreadsheetViewType Type { get; }
		#endregion
		#region Bounds
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Hidden)]
		protected internal Rectangle Bounds {
			get { return bounds; }
			set {
				if (bounds == value)
					return;
				if (value.Width < MinWidth)
					value.Width = MinWidth;
				if (value.Height < MinHeight)
					value.Height = MinHeight;
				bounds = value;
			}
		}
		#endregion
		#region ZoomFactor
		public float ZoomFactor {
			get { return GetActualZoomFactor(); }
			set {
				if (value < minZoomFactor)
					value = minZoomFactor;
				if (value > maxZoomFactor)
					value = maxZoomFactor;
				float zoomFactor = GetActualZoomFactor();
				if (zoomFactor == value)
					return;
				OnZoomFactorChanging();
				float oldValue = zoomFactor;
				SetActualZoomFactor(value);
				OnZoomFactorChanged(oldValue, GetActualZoomFactor());
			}
		}
		#endregion
		public Color ActualBackColor { get { return BackColor; } }
		#region BackColor
		public Color BackColor {
			get { return backColor; }
			set {
				if (backColor == value)
					return;
				backColor = value;
				RaiseBackColorChanged();
			}
		}
		protected internal virtual bool ShouldSerializeBackColor() {
			return BackColor != DXSystemColors.Window;
		}
		protected internal virtual void ResetBackColor() {
			BackColor = DXSystemColors.Window;
		}
		#endregion
		public bool AllowCalculateHeaderHitTest { get { return allowCalculateHeaderHitTest; } set { allowCalculateHeaderHitTest = value; } }
		public bool AllowCalculateGroupHitTest { get { return allowCalculateGroupHitTest; } set { allowCalculateGroupHitTest = value; } }
		internal bool IsDisposed { get { return isDisposed; } }
		protected internal DocumentModel DocumentModel { get { return control.InnerControl.DocumentModel; } }
		[Browsable(false)]
		protected internal DocumentLayout DocumentLayout { get { return control.InnerControl.DesignDocumentLayout; } }
		protected internal SpreadsheetViewVerticalScrollController VerticalScrollController { get { return verticalScrollController; } }
		protected internal SpreadsheetViewHorizontalScrollController HorizontalScrollController { get { return horizontalScrollController; } }
		protected internal virtual int MinWidth { get { return minWidth; } }
		protected internal virtual int MinHeight { get { return minHeight; } }
#if DEBUGTEST
		[Browsable(false)]
#endif
		#endregion
		public virtual void RecreateScrollControllers() {
			DeactivateScrollControllers();
			this.verticalScrollController = control.CreateSpreadsheetViewVerticalScrollController(this);
			this.horizontalScrollController = control.CreateSpreadsheetViewHorizontalScrollController(this);
			ActivateScrollControllers();
		}
		protected internal abstract float GetActualZoomFactor();
		protected internal abstract void SetActualZoomFactor(float zoomFactor);
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DeactivateScrollControllers();
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Events
		#region ZoomChanging
		EventHandler onZoomChanging;
		internal event EventHandler ZoomChanging { add { onZoomChanging += value; } remove { onZoomChanging -= value; } }
		protected internal virtual void RaiseZoomChanging() {
			if (onZoomChanging != null)
				onZoomChanging(this, EventArgs.Empty);
		}
		#endregion
		#region ZoomChanged
		EventHandler onZoomChanged;
		internal event EventHandler ZoomChanged { add { onZoomChanged += value; } remove { onZoomChanged -= value; } }
		protected internal virtual void RaiseZoomChanged() {
			if (onZoomChanged != null)
				onZoomChanged(this, EventArgs.Empty);
		}
		#endregion
		#region BackColorChanged
		EventHandler onBackColorChanged;
		protected internal event EventHandler BackColorChanged { add { onBackColorChanged += value; } remove { onBackColorChanged -= value; } }
		protected internal virtual void RaiseBackColorChanged() {
			if (onBackColorChanged != null)
				onBackColorChanged(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal virtual DocumentLayout CreateDocumentLayout(DocumentModel documentModel) {
			return new DocumentLayout(documentModel);
		}
		protected internal virtual void Deactivate() {
			DeactivateScrollControllers();
		}
		protected void DeactivateScrollControllers() {
			if (verticalScrollController != null)
				verticalScrollController.Deactivate();
			if (horizontalScrollController != null)
				horizontalScrollController.Deactivate();
		}
		protected virtual void ActivateScrollControllers() {
			VerticalScrollController.Activate();
			HorizontalScrollController.Activate();
		}
		protected internal virtual void Activate(Rectangle viewBounds) {
			Debug.Assert(viewBounds.Location == Point.Empty);
			DeactivateScrollControllers();
			ActivateScrollControllers();
		}
		protected internal virtual void OnZoomFactorChanging() {
			RaiseZoomChanging();
		}
		protected internal virtual void OnZoomFactorChanged(float oldValue, float newValue) {
			PerformZoomFactorChanged();
			RaiseZoomChanged();
			Control.InnerControl.RaiseUpdateUI();
		}
		protected internal virtual void PerformZoomFactorChanged() {
			Control.InnerControl.ResetDocumentLayout();
			Control.InnerControl.Owner.Redraw(RefreshAction.Zoom);
		}
		protected internal virtual void OnZoomFactorChangedCore() {
		}
		protected internal virtual void CorrectZoomFactor() {
			SpreadsheetBehaviorOptions options = Control.InnerControl.Options.InnerBehavior;
			float zoom = Math.Max(defaultZoomFactor, options.MinZoomFactor);
			if (options.MaxZoomFactor == options.DefaultMaxZoomFactor)
				this.ZoomFactor = zoom;
			else
				this.ZoomFactor = Math.Min(zoom, options.MaxZoomFactor);
		}
		protected internal virtual void OnSelectionChanged() {
			SelectionLayout.Invalidate();
		}
		protected internal HotZone CalculateHotZone(Point point) {
			Page page = GetPageByPoint(point);
			if (page == null)
				return null;
			HotZone result = PivotTableLayout.CalculateHotZone(point, page);
			if (result != null)
				return result;
			result = DataValidationLayout.CalculateHotZone(point, page);
			if (result != null)
				return result;
			result = CommentLayout.CalculateHotZone(point, page);
			if (result != null)
				return result;
			result = AutoFilterLayout.CalculateHotZone(point, page);
			if (result != null)
				return result;
			return SelectionLayout.CalculateHotZone(point, page);
		}
		protected internal Page GetPageByPoint(Point point) {
			return DocumentLayout.GetPageByPoint(point);
		}
		protected internal virtual void OnVerticalScroll(int offset) {
			Worksheet activeSheet = Control.InnerControl.DocumentModel.ActiveSheet;
			ModelWorksheetView activeSheetView = activeSheet.ActiveView;
			CellPosition topLeft = activeSheetView.ScrolledTopLeftCell;
			int scrolledRowIndex = topLeft.Row + offset;
			if (!activeSheet.IsRowVisible(scrolledRowIndex)) {
				int visibleIndex = topLeft.Row + offset;
				if (offset < 0) {
					for (int i = scrolledRowIndex; i > -1; i--)
						if (activeSheet.IsRowVisible(i)) {
							visibleIndex = i;
							break;
						}
				}
				else
					for (int i = scrolledRowIndex; i < activeSheet.MaxRowCount; i++)
						if (activeSheet.IsRowVisible(i)) {
							visibleIndex = i;
							break;
						}
				offset = visibleIndex - topLeft.Row;
			}
			if (topLeft.Row + offset >= Control.InnerControl.DocumentModel.ActiveSheet.MaxRowCount)
				return;
			SetScrollPositionToActiveSheet(topLeft.Column, Math.Max(0, GetOffsetedRow(topLeft.Row, offset)));
			Control.InnerControl.ResetDocumentLayout();
			Control.InnerControl.Owner.Redraw(RefreshAction.Transforms);
		}
		int GetOffsetedRow(int row, int offset) {
			Worksheet activeSheet = Control.InnerControl.DocumentModel.ActiveSheet;
			int start = row;
			int end = row + offset;
			if (offset < 0 && !activeSheet.IsRowVisible(Math.Max(end, 0))) {
				for (int i = start; i >= end; i--)
					if (end > 0) {
						Row currentRow = activeSheet.Rows.TryGetRow(i);
						if (currentRow != null && !currentRow.IsVisible)
							end--;
					}
			}
			return Math.Max(end, 0);
		}
		protected internal virtual void OnHorizontalScroll(int offset) {
			Worksheet activeSheet = Control.InnerControl.DocumentModel.ActiveSheet;
			ModelWorksheetView activeSheetView = activeSheet.ActiveView;
			CellPosition topLeft = activeSheetView.ScrolledTopLeftCell;
			int scrolledColumnIndex = topLeft.Column + offset;
			if (!activeSheet.IsColumnVisible(scrolledColumnIndex)) {
				int visibleIndex = topLeft.Column + offset;
				if (offset < 0) {
					for (int i = scrolledColumnIndex; i > -1; i--)
						if (activeSheet.IsColumnVisible(i)) {
							visibleIndex = i;
							break;
						}
				}
				else
					for (int i = scrolledColumnIndex; i < activeSheet.MaxColumnCount; i++)
						if (activeSheet.IsColumnVisible(i)) {
							visibleIndex = i;
							break;
						}
				offset = visibleIndex - topLeft.Column;
			}
			if (topLeft.Column + offset >= Control.InnerControl.DocumentModel.ActiveSheet.MaxColumnCount)
				return;
			SetScrollPositionToActiveSheet(Math.Max(0, topLeft.Column + offset), topLeft.Row);
			Control.InnerControl.ResetDocumentLayout();
			Control.InnerControl.Owner.Redraw(RefreshAction.Transforms);
		}
		void SetScrollPositionToActiveSheet(int columnIndex, int rowIndex) {
			DocumentModel.BeginUpdateFromUI();
			try {
				DocumentModel.ActiveSheet.SetScrollPosition(columnIndex, rowIndex);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected internal virtual void UpdateHorizontalScrollbar() {
			HorizontalScrollController.UpdateScrollBar();
		}
		public virtual SpreadsheetHitTestResult CalculatePageHitTest(Point point) {
			SpreadsheetHitTestResult result = CalculateNearestCellHitTest(point);
			if (result == null && AllowCalculateHeaderHitTest)
				result = CalculateHeaderHitTest(point);
			if (result == null && AllowCalculateGroupHitTest)
				result = CalculateGroupHitTest(point);
			return result;
		}
		public virtual SpreadsheetHitTestResult CalculateNearestCellHitTest(Point point) {
			SpreadsheetHitTestRequest request = new SpreadsheetHitTestRequest();
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Cell;
			request.Accuracy =  HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestCell;
			SpreadsheetHitTestResult result = HitTest(request);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Cell))
				return null;
			else
				return result;
		}
		public virtual SpreadsheetHitTestResult CalculateHeaderHitTest(Point point) {
			SpreadsheetHitTestRequest request = new SpreadsheetHitTestRequest();
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Header;
			request.Accuracy =  HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea;
			SpreadsheetHitTestResult result = HitTest(request);
			if (!result.IsValid(DocumentLayoutDetailsLevel.Header))
				return null;
			else
				return result;
		}
		public virtual SpreadsheetHitTestResult CalculateGroupHitTest(Point point) {
			SpreadsheetHitTestRequest request = new SpreadsheetHitTestRequest();
			request.PhysicalPoint = point;
			request.DetailsLevel = DocumentLayoutDetailsLevel.GroupArea;
			request.Accuracy =  HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea;
			SpreadsheetHitTestResult result = HitTest(request);
			if (!result.IsValid(DocumentLayoutDetailsLevel.GroupArea))
				return null;
			else
				return result;
		}
		protected internal virtual SpreadsheetHitTestResult HitTest(SpreadsheetHitTestRequest request) {
			SpreadsheetHitTestResult result = new SpreadsheetHitTestResult(DocumentLayout);
			Point logicalPoint = CreateLogicalPoint(request.PhysicalPoint);
			request.LogicalPoint = logicalPoint;
			Page page = DocumentLayout.GetPageByPoint(logicalPoint);
			HeaderTextBox headerBox = null;
			OutlineLevelBox groupBox = null;
			if (page == null) {
				HeaderPage headerPage = DocumentLayout.GetHeaderPageByPoint(logicalPoint);
				GroupItemsPage groupPage = DocumentLayout.GetGroupPageByPoint(logicalPoint);
				if (headerPage != null) {
					headerBox = headerPage.GetHeaderBox(logicalPoint);
					if (headerBox == null)
						return result;
				}
				if (groupPage != null) {
					groupBox = groupPage.GetGroupBox(logicalPoint);
					if (groupBox == null)
						return result;
				}
			}
			result.Page = page;
			HitTestCore(page, headerBox, groupBox, request, result);
			return result;
		}
		protected internal Point CreateLogicalPoint(Point point) {
			Matrix matrix = CreateTransformMatrix(ZoomFactor);
			matrix.Invert();
			Point[] result = new Point[1] { point };
			matrix.TransformPoints(result);
			return result[0];
		}
		protected internal Matrix CreateTransformMatrix(float zoomFactor) {
			Matrix matrix = new Matrix();
			matrix.Scale(zoomFactor, zoomFactor);
			return matrix;
		}
		protected internal virtual void HitTestCore(Page page, HeaderTextBox headerBox, OutlineLevelBox groupBox, SpreadsheetHitTestRequest request, SpreadsheetHitTestResult result) {
			HitTestCalculator calculator = new HitTestCalculator(request, result);
			if (page != null)
				calculator.CalculateHitTest(page);
			else if (headerBox != null)
				calculator.CalculateHitTest(headerBox);
			else if (groupBox != null)
				calculator.CalculateHitTest(groupBox);
		}
		public virtual SpreadsheetHitTestResult CalculateHitTest(Point point) {
			return new SpreadsheetHitTestResult(DocumentLayout);
		}
		public virtual void OnResize(Rectangle bounds) {
			Debug.Assert(bounds.Location == Point.Empty);
			Control.InnerControl.ResetDocumentLayout();
		}
		protected internal virtual void OnLayoutUnitChanging() {
		}
		protected internal virtual void OnLayoutUnitChanged() {
			Control.InnerControl.ResetDocumentLayout();
		}
		protected internal Rectangle GetPhysicalPixelRectangle(Rectangle logicalBounds) {
			int left = (int)Math.Round(logicalBounds.Left * ZoomFactor);
			int top = (int)Math.Round(logicalBounds.Top * ZoomFactor);
			int width = (int)Math.Round(logicalBounds.Width * ZoomFactor);
			int height = (int)Math.Round(logicalBounds.Height * ZoomFactor);
			return new Rectangle(left, top, width, height);
		}
		public void ScrollLineUpDown(int lineCount) {
			VerticalScrollController.ScrollLineUpDown(lineCount);
			OnVerticalScroll(lineCount);
		}
		public void ScrollLineLeftRight(int lineCount) {
			HorizontalScrollController.ScrollLineLeftRight(lineCount);
			OnHorizontalScroll(lineCount);
		}
		public bool IsHorizontalScrollPossible() {
			return horizontalScrollController.IsScrollPossible();
		}
		public bool IsVerticalScrollPossible() {
			return verticalScrollController.IsScrollPossible();
		}
		public bool IsHorizontalScrollMinimum() {
			IScrollBarAdapter scrollBarAdapter = horizontalScrollController.ScrollBarAdapter;
			return scrollBarAdapter.Value == scrollBarAdapter.Minimum;
		}
		public bool IsHorizontalScrollMaximum() {
			IScrollBarAdapter scrollBarAdapter = horizontalScrollController.ScrollBarAdapter;
			return scrollBarAdapter.Value == scrollBarAdapter.Maximum - 1;
		}
		public bool IsVerticalScrollMinimum() {
			IScrollBarAdapter scrollBarAdapter = verticalScrollController.ScrollBarAdapter;
			return scrollBarAdapter.Value == scrollBarAdapter.Minimum;
		}
		public bool IsVerticalScrollMaximum() {
			IScrollBarAdapter scrollBarAdapter = verticalScrollController.ScrollBarAdapter;
			return scrollBarAdapter.Value == scrollBarAdapter.Maximum - 1;
		}
		public abstract void Visit(ISpreadsheetViewVisitor visitor);
	}
	#endregion
	#region ISpreadsheetViewVisitor
	[ComVisible(true)]
	public interface ISpreadsheetViewVisitor {
		void Visit(NormalView view);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Internal {
	public static class SpreadsheetViewAccessor {
		public static bool ShouldSerializeBackColor(SpreadsheetView view) {
			return view.ShouldSerializeBackColor();
		}
	}
}
