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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Utils;
using System.Runtime.InteropServices;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region DrawingInfo
	#region AnchorType
	public enum AnchorType {
		TwoCell,
		OneCell,
		Absolute,
	}
	#endregion
	#region AnchorPoint
	public class AnchorPoint : ICloneable<AnchorPoint>, ISupportsCopyFrom<AnchorPoint>, ISupportsSizeOf {
		#region Fields
		float colOffset;
		float rowOffset;
		CellKey cellKey;
		#endregion
		public AnchorPoint(int sheetId, int columnIndex, int rowIndex, float columnOffset, float rowOffset) {
			this.colOffset = columnOffset;
			this.rowOffset = rowOffset;
			cellKey = new CellKey(sheetId, columnIndex, rowIndex);
		}
		public AnchorPoint(CellKey cellKey, float columnOffset, float rowOffset) {
			this.colOffset = columnOffset;
			this.rowOffset = rowOffset;
			this.cellKey = cellKey;
		}
		public AnchorPoint(int sheetId, int columnIndex, int rowIndex) {
			colOffset = 0;
			rowOffset = 0;
			cellKey = new CellKey(sheetId, columnIndex, rowIndex);
		}
		public AnchorPoint(CellKey cellKey) {
			colOffset = 0;
			rowOffset = 0;
			this.cellKey = cellKey;
		}
		public AnchorPoint() {
			colOffset = 0;
			rowOffset = 0;
			cellKey = new CellKey();
		}
		#region Properties
		public int Col { get { return cellKey.ColumnIndex; } }
		public int Row { get { return cellKey.RowIndex; } }
		public float ColOffset { get { return colOffset; } }
		public float RowOffset { get { return rowOffset; } }
		public CellKey CellKey { get { return cellKey; } }
		#endregion
		public AnchorPoint Clone() {
			AnchorPoint result = new AnchorPoint(this.CellKey);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(AnchorPoint value) {
			Guard.ArgumentNotNull(value, "AnchorPoint");
			cellKey = new CellKey(value.cellKey.SheetId, value.CellKey.ColumnIndex, value.CellKey.RowIndex);
			colOffset = value.ColOffset;
			rowOffset = value.RowOffset;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			AnchorPoint info = obj as AnchorPoint;
			if(info == null)
				return false;
			return this.cellKey == info.cellKey && this.rowOffset == info.rowOffset && this.colOffset == info.colOffset;
		}
		public override int GetHashCode() {
			return (int)(cellKey.GetHashCode() ^ rowOffset.GetHashCode() ^ colOffset.GetHashCode());
		}
	}
	#endregion
	public interface IDrawingObjectVisitor {
		void Visit(Picture picture);
		void Visit(Chart chart);
		void Visit(ModelShape shape);
		void Visit(GroupShape groupShape);
		void Visit(ConnectionShape connectionShape);
	}
	public enum DrawingObjectType {
		Picture,
		Chart,
		Shape,
		ConnectionShape,
		GroupShape,
	}
	public interface IDrawingObject : IDisposable, ISupportsNoChangeAspect, IBatchUpdateable {
		Worksheet Worksheet { get; }
		DrawingObject DrawingObject { get; }
		IGraphicFrameInfo GraphicFrameInfo { get; }
		bool LocksWithSheet { get; set; }
		bool PrintsWithSheet { get; set; }
		AnchorType AnchorType { get; set; }
		AnchorType ResizingBehavior { get; set; }
		AnchorPoint From { get; set; }
		AnchorPoint To { get; set; }
		DrawingObjectType DrawingType { get; }
		bool CanRotate { get; }
		float Height { get; set; }
		float Width { get; set; }
		float CoordinateX { get; set; }
		float CoordinateY { get; set; }
		int ZOrder { get; set; }
		AnchorData AnchorData { get; }
		int IndexInCollection { get; }
		void SetIndexInCollection(int value);
		void Move(float offsetY, float offsetX);
		void SetIndependentWidth(float width);
		void SetIndependentHeight(float height);
		void CoordinatesFromCellKey(CellKey cellKey);
		void SizeFromCellKey(CellKey cellKey);
		void Rotate(int angle);
		float GetRotationAngleInDegrees();
		void EnlargeWidth(float scale);
		void EnlargeHeight(float scale);
		void ReduceWidth(float scale);
		void ReduceHeight(float scale);
		void Visit(IDrawingObjectVisitor visitor);
		void OnRangeInserting(InsertRangeNotificationContext context);
		void OnRangeRemoving(RemoveRangeNotificationContext context);
	}
	#region AnchorData
	public class AnchorData : ICloneable<AnchorData>, ISupportsCopyFrom<AnchorData> {
		#region Fields
		AnchorPoint from;
		AnchorPoint to;
		readonly InternalSheetBase worksheet;
		float width;
		float height;
		float left;
		float top;
		AnchorType anchorType;
		AnchorType resizingBehavior;
		#endregion
		public AnchorData(InternalSheetBase worksheet, ISupportsNoChangeAspect locks) {
			this.worksheet = worksheet;
			from = new AnchorPoint();
			to = new AnchorPoint();
			Locks = locks;
			IgnoreNoChangeAspect = false;
		}
		#region Properties
		internal ISupportsNoChangeAspect Locks { get; set; }
		bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public Worksheet Worksheet { get { return worksheet as Worksheet; } }
		public DocumentModel DocumentModel { get { return Worksheet.Workbook; } }
		#region AnchorType
		public AnchorType AnchorType {
			get { return anchorType; }
			set {
				if(AnchorType == value)
					return;
				SetAnchorDataAnchorTypeHistoryItem historyItem = new SetAnchorDataAnchorTypeHistoryItem(this, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
				ValidateAll(value);
			}
		}
		internal void SetAnchorTypeCore(AnchorType value) {
			anchorType = value;
		}
		#endregion
		#region ResizingBehavior
		public AnchorType ResizingBehavior {
			get { return resizingBehavior; }
			set {
				if(ResizingBehavior == value)
					return;
				SetAnchorDataResizingBehaviourHistoryItem historyItem = new SetAnchorDataResizingBehaviourHistoryItem(this, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
				ValidateAll(value);
			}
		}
		public void SetResizingBehaviorCore(AnchorType value) {
			resizingBehavior = value;
		}
		#endregion
		#region AnchorPoint
		public AnchorPoint From {
			get {
				if(InvalidFromColumn || InvalidFromRow) {
					ValidateFrom();
				}
				return GetNormalFrom(from);
			}
			set {
				if(from.Equals(value))
					return;
				if(IgnoreHistory) {
					from = value;
				}
				else {
					DocumentHistory history = DocumentModel.History;
					ChangeDrawingObjectStartingPositionHistoryItem historyItem = new ChangeDrawingObjectStartingPositionHistoryItem(this, from, value);
					history.Add(historyItem);
					historyItem.Execute();					
				}
			}
		}
		internal void ValidateFrom() {
			from = AnchorPointCalculator(new AnchorPoint(), CoordinateX, CoordinateY);
			InvalidFromColumn = false;
			InvalidFromRow = false;
		}
		public AnchorPoint To {
			get {
				if(InvalidToColumn || InvalidToRow) {
					ValidateTo();
				}
				return GetNormalTo(to);
			}
			set {
				if(to.Equals(value))
					return;
				if(IgnoreHistory) {
					to = value;
				}
				else {
					DocumentHistory history = DocumentModel.History;
					ChangeDrawingObjectEndingPositionHistoryItem historyItem = new ChangeDrawingObjectEndingPositionHistoryItem(this, to, value);
					history.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		internal void ValidateTo() {
			bool needRecalculateHeight;
			PositionPair rowPosition = InvalidToRow ? CalculateRowPosition(out needRecalculateHeight, From, Height) : new PositionPair(to.Row, to.RowOffset);
			bool needRecalculateWidth;
			PositionPair columnPosition = InvalidToColumn ? CalculateColumnPosition(out needRecalculateWidth, From, Width) : new PositionPair(to.Col, to.ColOffset);
			to = new AnchorPoint(Worksheet.SheetId, columnPosition.CellIndex, rowPosition.CellIndex, columnPosition.Offset, rowPosition.Offset);
			InvalidToColumn = false;
			InvalidToRow = false;
		}
		#endregion
		#region Height
		public float Height {
			get {
				if(InvalidHeight) {
					ValidateHeight();
				}
				return height;
			}
			set {
				if(!InvalidHeight && Height == value)
					return;
				if(!IgnoreHistory)
					DocumentModel.BeginUpdate();
				if(NoChangeAspect && !IgnoreNoChangeAspect) {
					float newWidth = CalculateWidthByHeight(Width, Height, value);
					SetIndependentWidth(newWidth);
				}
				SetIndependentHeight(value);
				if(!IgnoreHistory)
					DocumentModel.EndUpdate();
			}
		}
		internal void ValidateHeight() {
			height = HeightFromAnchorPoint();
			InvalidHeight = false;
		}
		#endregion
		#region Width
		public float Width {
			get {
				if(InvalidWidth) {
					ValidateWidth();
				}
				return width;
			}
			set {
				if(!InvalidWidth && Width == value)
					return;
				if(!IgnoreHistory)
					DocumentModel.BeginUpdate();
				if(NoChangeAspect && !IgnoreNoChangeAspect) {
					float newHeight = CalculateHeightByWidth(Height, Width, value);
					SetIndependentHeight(newHeight);
				}
				SetIndependentWidth(value);
				if(!IgnoreHistory)
					DocumentModel.EndUpdate();
			}
		}
		internal void ValidateWidth() {
			width = WidthFromAnchorPoint();
			InvalidWidth = false;
		}
		#endregion
		#region CoordinateX
		public float CoordinateX {
			get {
				if(InvalidCoordinateX) {
					ValidateCoordinateX();
				}
				return left;
			}
			set {
				if(value < 0)
					value = 0;
				if(!InvalidCoordinateX && CoordinateX == value)
					return;
				if(IgnoreHistory)
					SetCoordinateXCore(value);
				else {
					SetAnchorDataCoordinateXHistoryItem historyItem = new SetAnchorDataCoordinateXHistoryItem(this, value);
					DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		public void ValidateCoordinateX() {
			left = XCoordinateFromAnchorPoint();
			InvalidCoordinateX = false;
		}
		public void SetCoordinateXCore(float value) {
			left = value;
			InvalidCoordinateX = false;
			InvalidFromColumn = true;
			InvalidToColumn = true;
		}
		#endregion
		#region CoordinateY
		public float CoordinateY {
			get {
				if(InvalidCoordinateY) {
					ValidateCoordinateY();
				}
				return top;
			}
			set {
				if(value < 0)
					value = 0;
				if(!InvalidCoordinateY && CoordinateY == value)
					return;
				if(IgnoreHistory)
					SetCoordinateYCore(value);
				else {
					SetAnchorDataCoordinateYHistoryItem historyItem = new SetAnchorDataCoordinateYHistoryItem(this, value);
					DocumentModel.History.Add(historyItem);
					historyItem.Execute();
				}
			}
		}
		public void ValidateCoordinateY() {
			top = YCoordinateFromAnchorPoint();
			InvalidCoordinateY = false;
		}
		public void SetCoordinateYCore(float value) {
			top = value;
			InvalidCoordinateY = false;
			InvalidFromRow = true;
			InvalidToRow = true;
		}
		#endregion
		#region SetIndependentWidth
		public void SetIndependentWidth(float value) {
			if(!InvalidWidth && Width == value)
				return;
			if(IgnoreHistory)
				SetIndependentWidthCore(value);
			else {
				SetAnchorDataWidthHistoryItem historyItem = new SetAnchorDataWidthHistoryItem(this, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void SetIndependentWidthCore(float value) {
			InvalidWidth = false;
			InvalidToColumn = true;
			width = value;
		}
		#endregion
		#region SetIndependentHeight
		public void SetIndependentHeight(float value) {
			if(!InvalidHeight && Height == value)
				return;
			if(IgnoreHistory)
				SetIndependentHeightCore(value);
			else {
				SetAnchorDataHeightHistoryItem historyItem = new SetAnchorDataHeightHistoryItem(this, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void SetIndependentHeightCore(float value) {
			InvalidHeight = false;
			InvalidToRow = true;
			height = value;
		}
		#endregion
		#region InvalidationProperties
		public bool InvalidFromColumn { get; set; }
		public bool InvalidToColumn { get; set; }
		public bool InvalidFromRow { get; set; }
		public bool InvalidToRow { get; set; }
		public bool InvalidCoordinateX { get; set; }
		public bool InvalidCoordinateY { get; set; }
		public bool InvalidWidth { get; set; }
		public bool InvalidHeight { get; set; }
		#endregion
		#region IgnoreNoChangeAspect
		bool IgnoreNoChangeAspect { get; set; }
		bool IgnoreHistory { get; set; }
		#endregion
		#endregion
		#region  Clone, CopyFrom
		public AnchorData Clone() {
			AnchorData result = new AnchorData(Worksheet, Locks);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(AnchorData value) {
			Guard.ArgumentNotNull(value, "AnchorData");
			left = value.left;
			top = value.top;
			width = value.width;
			height = value.height;
			anchorType = value.anchorType;
			resizingBehavior = value.resizingBehavior;
			from.CopyFrom(value.from);
			to.CopyFrom(value.to);
			InvalidCoordinateX = value.InvalidCoordinateX;
			InvalidCoordinateY = value.InvalidCoordinateY;
			InvalidWidth = value.InvalidWidth;
			InvalidHeight = value.InvalidHeight;
			InvalidFromColumn = value.InvalidFromColumn;
			InvalidFromRow = value.InvalidFromRow;
			InvalidToColumn = value.InvalidToColumn;
			InvalidToRow = value.InvalidToRow;
			Locks = value.Locks;
		}
		#endregion
		#region Helpers
		AnchorType GetResizingAnchorTypeCore(AnchorType resizingBehavior, AnchorType anchorType) {
			return resizingBehavior == AnchorType.TwoCell ? anchorType : resizingBehavior;
		}
		public AnchorType GetResizingAnchorType() {
			return GetResizingAnchorTypeCore(ResizingBehavior, AnchorType);
		}
		AnchorPoint GetNormalFrom(AnchorPoint currentFrom) {
			if(GetResizingAnchorType() == AnchorType.Absolute)
				return currentFrom;
			return NormalizeAnchorPoint(currentFrom);
		}
		AnchorPoint GetNormalTo(AnchorPoint currentTo) {
			return GetResizingAnchorType() == AnchorType.TwoCell ? NormalizeAnchorPoint(currentTo) : currentTo;
		}
		AnchorPoint NormalizeAnchorPoint(AnchorPoint point) {
			int columnIndex = point.Col;
			int rowIndex = point.Row;
			float columnWidth = AccumulatedOffset.GetOneColumnWidth(Worksheet, columnIndex);
			float rowHeight = AccumulatedOffset.GetOneRowHeight(Worksheet, rowIndex);
			return new AnchorPoint(point.CellKey.SheetId, columnIndex, rowIndex, Math.Min(point.ColOffset, columnWidth), Math.Min(point.RowOffset, rowHeight));
		}
		AnchorPoint AnchorPointCalculatorCore(AnchorPoint startingPoint, float coordinateX, float coordinateY, out bool needRecalculateSize) {
			needRecalculateSize = false;
			if(DoubleComparer.AreEqual(coordinateX, 0) && (DoubleComparer.AreEqual(coordinateY, 0)))
				return startingPoint.Clone();
			bool needRecalculateHeight;
			PositionPair rowPosition = CalculateRowPosition(out needRecalculateHeight, startingPoint, coordinateY);
			bool needRecalculateWidth;
			PositionPair columnPosition = CalculateColumnPosition(out needRecalculateWidth, startingPoint, coordinateX);
			needRecalculateSize = needRecalculateHeight || needRecalculateWidth;
			return new AnchorPoint(Worksheet.SheetId, columnPosition.CellIndex, rowPosition.CellIndex, columnPosition.Offset, rowPosition.Offset);
		}
		PositionPair CalculateColumnPosition(out bool needRecalculateWidth, AnchorPoint startingPoint, float coordinateX) {
			int columnIndex = startingPoint.CellKey.ColumnIndex;
			PositionPair columnPosition = CalculateNewPositionPair(false, new PositionPair(columnIndex, startingPoint.ColOffset), coordinateX, out needRecalculateWidth);
			return columnPosition;
		}
		PositionPair CalculateRowPosition(out bool needRecalculateHeight, AnchorPoint startingPoint, float coordinateY) {
			int rowIndex = startingPoint.CellKey.RowIndex;
			PositionPair rowPosition = CalculateNewPositionPair(true, new PositionPair(rowIndex, startingPoint.RowOffset), coordinateY, out needRecalculateHeight);
			return rowPosition;
		}
		PositionPair CalculateNewPositionPair(bool calcRow, PositionPair positionPair, float coordinate, out bool needRecalculateSize) {
			needRecalculateSize = false;
			AccumulatedOffset accumulatedOffset = new AccumulatedOffset(Worksheet);
			float resultOffset = positionPair.Offset;
			int resultIndex = positionPair.CellIndex;
			int maxValue = calcRow ? Worksheet.MaxRowCount : Worksheet.MaxColumnCount;
			if(!DoubleComparer.AreEqual(coordinate, 0)) {
				resultOffset += coordinate;
				do {
					if(resultIndex >= maxValue) {
						accumulatedOffset = new AccumulatedOffset(Worksheet);
						accumulatedOffset.AddCell(maxValue - 1, calcRow);
						needRecalculateSize = true;
						return new PositionPair(maxValue - 1, accumulatedOffset.ToModel());
					}
					accumulatedOffset.AddCell(resultIndex, calcRow);
					if(resultOffset < accumulatedOffset.ToModel()) {
						accumulatedOffset.RemoveCell(resultIndex, calcRow);
						break;
					}
					resultIndex++;
				} while(true);
			}
			return new PositionPair(resultIndex, resultOffset - accumulatedOffset.ToModel());
		}
		AnchorPoint AnchorPointCalculator(AnchorPoint startingPoint, float coordinateX, float coordinateY) {
			bool needRecalculateSize;
			return AnchorPointCalculatorCore(startingPoint, coordinateX, coordinateY, out needRecalculateSize);
		}
		float WidthFromAnchorPoint() {
			AnchorPoint currentFrom = GetNormalFrom(From);
			AnchorPoint currentTo = GetNormalTo(To);
			int topLeftColumnIndex = currentFrom.CellKey.ColumnIndex;
			int bottomRightColumnIndex = currentTo.CellKey.ColumnIndex;
			float columnsWidth = CellKeyToWidth(Worksheet, topLeftColumnIndex, bottomRightColumnIndex);
			float modelToOffset = currentTo.ColOffset;
			float modelFromOffset = currentFrom.ColOffset;
			float result = columnsWidth - modelFromOffset + modelToOffset;
			return result;
		}
		float HeightFromAnchorPoint() {
			AnchorPoint currentFrom = GetNormalFrom(From);
			AnchorPoint currentTo = GetNormalTo(To);
			int topLeftRowIndex = currentFrom.CellKey.RowIndex;
			int bottomRightRowIndex = currentTo.CellKey.RowIndex;
			float rowsHeight = CellKeyToHeight(Worksheet, topLeftRowIndex, bottomRightRowIndex);
			float modelToOffset = currentTo.RowOffset;
			float modelFromOffset = currentFrom.RowOffset;
			float result = rowsHeight - modelFromOffset + modelToOffset;
			return result;
		}
		static float CalculateHeightByWidth(float height, float oldWidth, float newWidth) {
			float coef = oldWidth == 0 ? newWidth : (newWidth / oldWidth);
			return height == 0 ? coef : height * coef;
		}
		static float CalculateWidthByHeight(float width, float oldHeight, float newHeight) {
			float coef = oldHeight == 0 ? newHeight : newHeight / oldHeight;
			return width == 0 ? coef : width * coef;
		}
		float XCoordinateFromAnchorPoint() {
			AnchorPoint realFrom = GetNormalFrom(from);
			int topLeftColumnIndex = realFrom.CellKey.ColumnIndex;
			float x = CellKeyToWidth(Worksheet, 0, topLeftColumnIndex);
			float modelFromOffset = realFrom.ColOffset;
			float result = x + modelFromOffset;
			return result;
		}
		float YCoordinateFromAnchorPoint() {
			AnchorPoint realFrom = GetNormalFrom(from);
			int topLeftRowIndex = realFrom.CellKey.RowIndex;
			float y = CellKeyToHeight(Worksheet, 0, topLeftRowIndex);
			float modelFromOffset = realFrom.RowOffset;
			float result = y + modelFromOffset;
			return result;
		}
		public void CoordinatesFromCellKey(CellKey cellKey) {
			from = new AnchorPoint(cellKey);
			left = XCoordinateFromAnchorPoint();
			top = YCoordinateFromAnchorPoint();
		}
		public void SizeFromCellKey(CellKey cellKey) {
			to = new AnchorPoint(cellKey);
			width = WidthFromAnchorPoint();
			height = HeightFromAnchorPoint();
		}
		public static float CellKeyToWidth(Worksheet worksheet, int indexFrom, int indexTo) {
			return CellKeyToImageSize(false, worksheet, indexFrom, indexTo);
		}
		public static float CellKeyToHeight(Worksheet worksheet, int indexFrom, int indexTo) {
			return CellKeyToImageSize(true, worksheet, indexFrom, indexTo);
		}
		static float CellKeyToImageSize(bool calcRow, Worksheet worksheet, int indexFrom, int indexTo) {
			AccumulatedOffset accumulatedOffset = new AccumulatedOffset(worksheet);
			accumulatedOffset.AddCells(indexFrom, indexTo - 1, calcRow);
			return accumulatedOffset.ToModel();
		}
		#endregion
		#region Move
		MoveAnchorPointResult MoveAnchorPoint(AnchorPoint point, float rowOffset, float columnOffset) {
			MoveAnchorPointResult result = new MoveAnchorPointResult();
			PositionPair rowPosition = new PositionPair(point.Row, point.RowOffset);
			PositionPair colunmPosition = new PositionPair(point.Col, point.ColOffset);
			CalculateNewPositionPairResult calculateNewPositionPairResult = MovePositionPair(true, rowPosition, rowOffset);
			rowPosition = calculateNewPositionPairResult.PositionPair;
			result.OffsetY = calculateNewPositionPairResult.Offset;
			calculateNewPositionPairResult = MovePositionPair(false, colunmPosition, columnOffset);
			colunmPosition = calculateNewPositionPairResult.PositionPair;
			result.OffsetX = calculateNewPositionPairResult.Offset;
			result.Point = new AnchorPoint(Worksheet.SheetId, colunmPosition.CellIndex, rowPosition.CellIndex, colunmPosition.Offset, rowPosition.Offset);
			return result;
		}
		CalculateNewPositionPairResult MovePositionPair(bool calcRow, PositionPair positionPair, float offset) {
			CalculateNewPositionPairResult result;
			PositionPair resultPair;
			if(DoubleComparer.AreEqual(offset, 0)) {
				resultPair = new PositionPair(positionPair.CellIndex, positionPair.Offset);
				result = new CalculateNewPositionPairResult(resultPair, 0);
				return result;
			}
			float pointOffset = positionPair.Offset;
			int currentCellIndex = positionPair.CellIndex;
			bool direction = offset > 0;
			float fulloffset = offset + pointOffset;
			float realOffset = offset;
			int maxValue = calcRow ? Worksheet.MaxRowCount : Worksheet.MaxColumnCount;
			AccumulatedOffset accumulatedCellSize = new AccumulatedOffset(Worksheet);
			if(direction)
				accumulatedCellSize.AddCell(currentCellIndex, calcRow);
			while(direction ? accumulatedCellSize.ToModel() <= fulloffset : fulloffset < 0 && accumulatedCellSize.ToModel() < -fulloffset) {
				currentCellIndex = direction ? currentCellIndex + 1 : currentCellIndex - 1;
				if(currentCellIndex == -1 || currentCellIndex >= maxValue) {
					currentCellIndex = currentCellIndex == -1 ? 0 : maxValue - 1;
					realOffset = -(accumulatedCellSize.ToModel() + pointOffset);
					fulloffset = -accumulatedCellSize.ToModel();
					break;
				}
				accumulatedCellSize.AddCell(currentCellIndex, calcRow);
			}
			if(direction)
				accumulatedCellSize.RemoveCell(currentCellIndex, calcRow);
			pointOffset = direction ? fulloffset - accumulatedCellSize.ToModel() : accumulatedCellSize.ToModel() + fulloffset;
			offset = realOffset;
			resultPair = new PositionPair(currentCellIndex, pointOffset);
			result = new CalculateNewPositionPairResult(resultPair, offset);
			return result;
		}
		public void Move(float offsetY, float offsetX) {
			if(offsetX == 0 && offsetY == 0)
				return;
			DocumentModel.BeginUpdate();
			try {
				MoveCore(offsetY, offsetX);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void MoveCore(float offsetY, float offsetX) {
			ValidateHeight();
			ValidateWidth();
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					MoveAnchorPointResult moveAnchorPointResultFrom = MoveAnchorPoint(From, offsetY, offsetX);
					MoveAnchorPointResult moveAnchorPointResultTo = MoveAnchorPoint(To, moveAnchorPointResultFrom.OffsetY, moveAnchorPointResultFrom.OffsetX);
					From = moveAnchorPointResultFrom.Point;
					To = moveAnchorPointResultTo.Point;
					InvalidWidth = false;
					InvalidHeight = false;
					return;
				case AnchorType.Absolute:
					if(offsetX != 0)
						CoordinateX += offsetX;
					if(offsetY != 0)
						CoordinateY += offsetY;
					return;
				case AnchorType.OneCell:
					AnchorPoint newFrom = MoveAnchorPoint(From, offsetY, offsetX).Point;
					From = newFrom;
					InvalidWidth = false;
					InvalidHeight = false;
					return;
			}
		}
		void ChangeSizeWithKeepCenterAlignment(float scale, bool changeWidth) {
			DocumentModel.BeginUpdate();
			float centerX = this.CoordinateX + this.Width / 2;
			float centerY = this.CoordinateY + this.Height / 2;
			if(changeWidth)
				this.Width *= scale;
			else
				this.Height *= scale;
			float offsetX = (centerX - this.Width / 2) - this.CoordinateX;
			float offsetY = (centerY - this.Height / 2) - this.CoordinateY;
			this.Move(offsetY, offsetX);
			DocumentModel.EndUpdate();
		}
		public void EnlargeWidth(float scale, bool noChangeAspect) {
			NoChangeAspect = noChangeAspect;
			ChangeSizeWithKeepCenterAlignment(scale, true);
		}
		public void EnlargeHeight(float scale, bool noChangeAspect) {
			NoChangeAspect = noChangeAspect;
			ChangeSizeWithKeepCenterAlignment(scale, false);
		}
		public void ReduceWidth(float scale, bool noChangeAspect) {
			NoChangeAspect = noChangeAspect;
			ChangeSizeWithKeepCenterAlignment(1f / scale, true);
		}
		public void ReduceHeight(float scale, bool noChangeAspect) {
			NoChangeAspect = noChangeAspect;
			ChangeSizeWithKeepCenterAlignment(1f / scale, false);
		}
		#endregion
		public void SetStartingPosition(AnchorPoint value) {
			from = value;
			InvalidFromRow = false;
			InvalidFromColumn = false;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					if(InvalidToColumn || InvalidToRow)
						ValidateTo();
					InvalidHeight = true;
					InvalidWidth = true;
					InvalidCoordinateX = true;
					InvalidCoordinateY = true;
					break;
				case AnchorType.OneCell:
				case AnchorType.Absolute:
					InvalidCoordinateX = true;
					InvalidCoordinateY = true;
					if(InvalidHeight)
						ValidateHeight();
					if(InvalidWidth)
						ValidateWidth();
					InvalidToColumn = true;
					InvalidToRow = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void SetEndingPosition(AnchorPoint value) {
			to = value;
			InvalidToRow = false;
			InvalidToColumn = false;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
				case AnchorType.OneCell:
				case AnchorType.Absolute:
					InvalidHeight = true;
					InvalidWidth = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		#region SwapHeightAndWidth
		public void SwapHeightAndWidth() {
			float originalHeight = Height;
			float originalWidth = Width;
			IgnoreNoChangeAspect = true;
			IgnoreHistory = true;
			Width = originalHeight;
			Height = originalWidth;
			DocumentModelUnitConverter unitConverter = Worksheet.Workbook.UnitConverter;
			int originalHeightEMU = unitConverter.ModelUnitsToEmuF(originalHeight);
			int originalWidthEMU = unitConverter.ModelUnitsToEmuF(originalWidth);
			int deltaEMU = (originalHeightEMU - originalWidthEMU) / 2;
			float delta = unitConverter.EmuToModelUnitsF(deltaEMU);
			CoordinateY += delta;
			CoordinateX -= delta;
			IgnoreNoChangeAspect = false;
			IgnoreHistory = false;
		}
		#endregion
		#region SetExtentCore
		protected internal void SetExtentCore(float width, float height) {
			this.width = width;
			this.height = height;
			bool needRecalculateSize = false;
			to = AnchorPointCalculatorCore(From, this.width, this.height, out needRecalculateSize);
			if(needRecalculateSize)
				this.width = WidthFromAnchorPoint();
		}
		#endregion
		public void ValidateAll() {
			ValidateAll(GetResizingAnchorType());
		}
		public void ValidateAll(AnchorType anchorType) {
			switch(anchorType) {
				case AnchorType.TwoCell:
					if(InvalidFromColumn || InvalidFromRow)
						ValidateFrom();
					if(InvalidToColumn || InvalidToRow)
						ValidateTo();
					break;
				case AnchorType.OneCell:
					if(InvalidFromColumn || InvalidFromRow)
						ValidateFrom();
					if(InvalidWidth)
						ValidateWidth();
					if(InvalidHeight)
						ValidateHeight();
					break;
				case AnchorType.Absolute:
					if(InvalidCoordinateX)
						ValidateCoordinateX();
					if(InvalidCoordinateY)
						ValidateCoordinateY();
					if(InvalidWidth)
						ValidateWidth();
					if(InvalidHeight)
						ValidateHeight();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			switch(context.Mode) {
				case RemoveCellMode.ShiftCellsLeft:
					RemoveRangeShiftCellsLeft(context);
					break;
				case RemoveCellMode.ShiftCellsUp:
					RemoveRangeShiftCellsUp(context);
					break;
			}
		}
		void RemoveRangeShiftCellsUp(RemoveRangeNotificationContext context) {
			int topRow = context.Range.TopRowIndex;
			int bottomRow = context.Range.BottomRowIndex;
			int leftColumn = context.Range.LeftColumnIndex;
			int rightColumn = context.Range.RightColumnIndex;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					if(leftColumn <= From.Col && To.Col - 1 <= rightColumn) {
						bool aboveAnchorData = topRow < From.Row;
						bool includeTopAnchor = topRow <= From.Row && From.Row <= bottomRow;
						bool includeShape = (topRow <= From.Row && From.Row < bottomRow) || (From.Row < topRow && topRow <= To.Row);
						bool includeBottomAnchor = topRow <= To.Row && To.Row <= bottomRow;
						int newFromRow = From.Row;
						float newFromRowOffset = From.RowOffset;
						int newToRow = To.Row;
						float newToRowOffset = To.RowOffset;
						if(aboveAnchorData) {
							int aboveShift = Math.Min(context.Range.Height, From.Row - topRow);
							newFromRow -= aboveShift;
							newToRow -= aboveShift;
						}
						if(includeTopAnchor) {
							newFromRowOffset = 0;
							newToRow--;
						}
						if(includeShape) {
							int includeShapeShift;
							if(includeTopAnchor) {
								includeShapeShift = Math.Min(bottomRow, To.Row) - From.Row;
							}
							else if(includeBottomAnchor) {
								includeShapeShift = To.Row - topRow;
							}
							else {
								includeShapeShift = context.Range.Height;
							}
							newToRow -= includeShapeShift;
						}
						if(includeBottomAnchor) {
							newToRowOffset = 0;
						}
						From = new AnchorPoint(Worksheet.SheetId, From.Col, newFromRow, From.ColOffset, newFromRowOffset);
						To = new AnchorPoint(Worksheet.SheetId, To.Col, newToRow, To.ColOffset, newToRowOffset);
					}
					break;
				case AnchorType.OneCell:
					if(leftColumn <= From.Col && From.Col <= rightColumn) {
						bool aboveAnchorData = topRow < From.Row;
						bool includeTopAnchor = topRow <= From.Row && From.Row <= bottomRow;
						int newFromRow = From.Row;
						float newFromRowOffset = From.RowOffset;
						if(aboveAnchorData) {
							int aboveShift = Math.Min(context.Range.Height, From.Row - topRow);
							newFromRow -= aboveShift;
						}
						if(includeTopAnchor) {
							newFromRowOffset = 0;
						}
						From = new AnchorPoint(Worksheet.SheetId, From.Col, newFromRow, From.ColOffset, newFromRowOffset);
					}
					break;
				case AnchorType.Absolute:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		void RemoveRangeShiftCellsLeft(RemoveRangeNotificationContext context) {
			int topRow = context.Range.TopRowIndex;
			int bottomRow = context.Range.BottomRowIndex;
			int leftColumn = context.Range.LeftColumnIndex;
			int rightColumn = context.Range.RightColumnIndex;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					if(topRow <= From.Row && To.Row - 1 <= bottomRow) {
						bool leftAnchorData = leftColumn < From.Col;
						bool includeLeftAnchor = leftColumn <= From.Col && From.Col <= rightColumn;
						bool includeShape = (leftColumn <= From.Col && From.Col < rightColumn) || (From.Col < leftColumn && leftColumn <= To.Col);
						bool includeRightAnchor = leftColumn <= To.Col && To.Col <= rightColumn;
						int newFromCol = From.Col;
						float newFromColOffset = From.ColOffset;
						int newToCol = To.Col;
						float newToColOffset = To.ColOffset;
						if(leftAnchorData) {
							int leftShift = Math.Min(context.Range.Width, From.Col - leftColumn);
							newFromCol -= leftShift;
							newToCol -= leftShift;
						}
						if(includeLeftAnchor) {
							newFromColOffset = 0;
							newToCol--;
						}
						if(includeShape) {
							int includeShapeShift;
							if(includeLeftAnchor) {
								includeShapeShift = Math.Min(rightColumn, To.Col) - From.Col;
							}
							else if(includeRightAnchor) {
								includeShapeShift = To.Col - leftColumn;
							}
							else {
								includeShapeShift = context.Range.Width;
							}
							newToCol -= includeShapeShift;
						}
						if(includeRightAnchor) {
							newToColOffset = 0;
						}
						From = new AnchorPoint(Worksheet.SheetId, newFromCol, From.Row, newFromColOffset, From.RowOffset);
						To = new AnchorPoint(Worksheet.SheetId, newToCol, To.Row, newToColOffset, To.RowOffset);
					}
					break;
				case AnchorType.OneCell:
					if(topRow <= From.Row && From.Row <= bottomRow) {
						bool leftAnchorData = leftColumn < From.Col;
						bool includeLeftAnchor = leftColumn <= From.Col && From.Col <= rightColumn;
						int newFromCol = From.Col;
						float newFromColOffset = From.ColOffset;
						if(leftAnchorData) {
							int leftShift = Math.Min(context.Range.Width, From.Col - leftColumn);
							newFromCol -= leftShift;
						}
						if(includeLeftAnchor) {
							newFromColOffset = 0;
						}
						From = new AnchorPoint(Worksheet.SheetId, newFromCol, From.Row, newFromColOffset, From.RowOffset);
					}
					break;
				case AnchorType.Absolute:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			switch(context.Mode) {
				case InsertCellMode.ShiftCellsRight:
					InsertRangeShiftCellsUp(context);
					break;
				case InsertCellMode.ShiftCellsDown:
					InsertRangeShiftCellsDown(context);
					break;
			}
		}
		void InsertRangeShiftCellsDown(InsertRangeNotificationContext context) {
			int leftColumnIndex = context.Range.LeftColumnIndex;
			int rightColumnIndex = context.Range.RightColumnIndex;
			int topRowIndex = context.Range.TopRowIndex;
			int rangeHeight = context.Range.Height;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					if(leftColumnIndex <= From.Col && To.Col - 1 <= rightColumnIndex) {
						if(topRowIndex <= From.Row) {
							From = new AnchorPoint(worksheet.SheetId, From.Col, From.Row + rangeHeight, From.ColOffset, From.RowOffset);
						}
						if(topRowIndex <= To.Row) {
							To = new AnchorPoint(worksheet.SheetId, To.Col, To.Row + rangeHeight, To.ColOffset, To.RowOffset);
						}
					}
					break;
				case AnchorType.OneCell:
					if(leftColumnIndex <= From.Col && From.Col <= rightColumnIndex) {
						if(topRowIndex <= From.Row) {
							From = new AnchorPoint(worksheet.SheetId, From.Col, From.Row + rangeHeight, From.ColOffset, From.RowOffset);
						}
					}
					break;
				case AnchorType.Absolute:
					break;
			}
		}
		void InsertRangeShiftCellsUp(InsertRangeNotificationContext context) {
			int topRowIndex = context.Range.TopRowIndex;
			int bottomRowIndex = context.Range.BottomRowIndex;
			int leftColumnIndex = context.Range.LeftColumnIndex;
			int rangeWidth = context.Range.Width;
			switch(GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					if(topRowIndex <= From.Row && To.Row - 1 <= bottomRowIndex) {
						if(leftColumnIndex <= From.Col) {
							From = new AnchorPoint(worksheet.SheetId, From.Col + rangeWidth, From.Row, From.ColOffset, From.RowOffset);
						}
						if(leftColumnIndex <= To.Col) {
							To = new AnchorPoint(worksheet.SheetId, To.Col + rangeWidth, To.Row, To.ColOffset, To.RowOffset);
						}
					}
					break;
				case AnchorType.OneCell:
					if(topRowIndex <= From.Row && From.Row <= bottomRowIndex) {
						if(leftColumnIndex <= From.Col) {
							From = new AnchorPoint(worksheet.SheetId, From.Col + rangeWidth, From.Row, From.ColOffset, From.RowOffset);
						}
					}
					break;
				case AnchorType.Absolute:
					break;
			}
		}
	}
	#endregion
	#region DrawingObjectInfo
	public class DrawingObjectInfo : ICloneable<DrawingObjectInfo>, ISupportsCopyFrom<DrawingObjectInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskLocksWithSheet = 0x00000004;			 
		const uint MaskPrintsWithSheet = 0x00000008;			
		const uint MaskIsPublished = 0x00020000;				
		const uint MaskHidden = 0x00040000;					 
		const uint MaskHyperlinkClickIsExternal = 0x00080000;   
		uint packedValues;
		string macro;
		string description;
		string hyperlinkClickTooltip;
		string hyperlinkClickTargetFrame;
		string hyperlinkClickUrl;
		#endregion
		#region Properties
		public bool LocksWithSheet { get { return GetBooleanVal(MaskLocksWithSheet); } set { SetBooleanVal(MaskLocksWithSheet, value); } }
		public bool PrintsWithSheet { get { return GetBooleanVal(MaskPrintsWithSheet); } set { SetBooleanVal(MaskPrintsWithSheet, value); } }
		#region cNvPr (Non-Visual Drawing Properties) §5.6.2.8
		public string Description { get { return description; } set { description = value; } }
		public bool Hidden { get { return GetBooleanVal(MaskHidden); } set { SetBooleanVal(MaskHidden, value); } }
		public string HyperlinkClickTooltip { get { return hyperlinkClickTooltip; } set { hyperlinkClickTooltip = value; } }
		public string HyperlinkClickTargetFrame { get { return hyperlinkClickTargetFrame; } set { hyperlinkClickTargetFrame = value; } }
		public string HyperlinkClickUrl { get { return hyperlinkClickUrl; } set { hyperlinkClickUrl = value; } }
		public bool HyperlinkClickIsExternal { get { return GetBooleanVal(MaskHyperlinkClickIsExternal); } set { SetBooleanVal(MaskHyperlinkClickIsExternal, value); } }
		#endregion
		#region GraphicFrame
		public bool IsPublished { get { return GetBooleanVal(MaskIsPublished); } set { SetBooleanVal(MaskIsPublished, value); } }
		public string Macro { get { return macro; } set { macro = value; } }
		#endregion
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if(bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ICloneable<DrawingObjectInfo> Members
		public DrawingObjectInfo Clone() {
			DrawingObjectInfo result = new DrawingObjectInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingObjectInfo> Members
		public void CopyFrom(DrawingObjectInfo value) {
			Guard.ArgumentNotNull(value, "DrawingObjectInfo");
			this.packedValues = value.packedValues;
			this.macro = value.macro;
			this.description = value.description;
			this.hyperlinkClickTooltip = value.hyperlinkClickTooltip;
			this.hyperlinkClickTargetFrame = value.hyperlinkClickTargetFrame;
			this.hyperlinkClickUrl = value.hyperlinkClickUrl;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			DrawingObjectInfo info = obj as DrawingObjectInfo;
			if(info == null)
				return false;
			return this.packedValues == info.packedValues
				   && this.macro == info.macro
				   && this.description == info.description
				   && this.hyperlinkClickTooltip == info.hyperlinkClickTooltip
				   && this.hyperlinkClickTargetFrame == info.hyperlinkClickTargetFrame
				   && this.hyperlinkClickUrl == info.hyperlinkClickUrl;
		}
		public override int GetHashCode() {
			return
				(int)
				(packedValues ^ (macro == null ? 0 : macro.GetHashCode()))
				^ (description == null ? 0 : description.GetHashCode())
				^ (hyperlinkClickTooltip == null ? 0 : hyperlinkClickTooltip.GetHashCode())
				^ (hyperlinkClickTargetFrame == null ? 0 : hyperlinkClickTargetFrame.GetHashCode())
				^ (hyperlinkClickUrl == null ? 0 : hyperlinkClickUrl.GetHashCode());
		}
	}
	#endregion
	#region DrawingObjectInfoCache
	public class DrawingObjectInfoCache : UniqueItemsCache<DrawingObjectInfo> {
		public const int DrawingInfoIndex = 0;
		public DrawingObjectInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingObjectInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			DrawingObjectInfo info = new DrawingObjectInfo();
			info.LocksWithSheet = true;
			info.PrintsWithSheet = true;
			info.Description = "";
			info.HyperlinkClickTargetFrame = "";
			info.HyperlinkClickTooltip = "";
			info.HyperlinkClickUrl = "";
			return info;
		}
	}
	#endregion
	#region DrawingObjectIndexAccessor
	public class DrawingObjectIndexAccessor : IIndexAccessor<DrawingObject, DrawingObjectInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<DrawingObject, DrawingObjectInfo> Members
		public int GetIndex(DrawingObject owner) {
			return owner.DrawingInfoIndex;
		}
		public int GetDeferredInfoIndex(DrawingObject owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(DrawingObject owner, int value) {
			owner.AssignDrawingInfoIndex(value);
		}
		public int GetInfoIndex(DrawingObject owner, DrawingObjectInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public DrawingObjectInfo GetInfo(DrawingObject owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(DrawingObject owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<DrawingObjectInfo> GetInfoCache(DrawingObject owner) {
			return owner.DocumentModel.Cache.DrawingObjectInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(DrawingObject owner) {
			return new DrawingObjectIndexChangeHistoryItem(owner);
		}
		public DrawingObjectInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((DrawingObjectBatchUpdateHelper)helper).DrawingInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, DrawingObjectInfo info) {
			((DrawingObjectBatchUpdateHelper)helper).DrawingInfo = info.Clone();
		}
		public void InitializeDeferredInfo(DrawingObject owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(DrawingObject owner, DrawingObject from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(DrawingObject owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region DrawingObjectBatchUpdateHelper
	public class DrawingObjectBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		DrawingObjectInfo drawingInfo;
		AnchorPoint anchorPoint;
		public DrawingObjectBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public DrawingObjectInfo DrawingInfo { get { return drawingInfo; } set { drawingInfo = value; } }
		public AnchorPoint AnchorPoint { get { return anchorPoint; } set { anchorPoint = value; } }
	}
	#endregion
	#region DrawingObjectBatchInitHelper
	public class DrawingObjectBatchInitHelper : MultiIndexBatchUpdateHelper {
		public DrawingObjectBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region DrawingObject
	public class DrawingObject : MultiIndexObject<DrawingObject, DocumentModelChangeActions>, ICloneable<DrawingObject>, ISupportsCopyFrom<DrawingObject>,
								 IGraphicFrameInfo, IDrawingObjectNonVisualProperties, IHyperlinkViewInfo {
		#region Static Members
		readonly static DrawingObjectIndexAccessor drawingObjectIndexAccessor = new DrawingObjectIndexAccessor();
		readonly static IIndexAccessorBase<DrawingObject, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<DrawingObject, DocumentModelChangeActions>[] {
			drawingObjectIndexAccessor
		};
		public static DrawingObjectIndexAccessor DrawingObjectIndexAccessor { get { return drawingObjectIndexAccessor; } }
		#endregion
		public const int MaxAngle = 21600000;
		#region Fields
		readonly InternalSheetBase worksheet;
		int drawingInfoIndex = DrawingObjectInfoCache.DrawingInfoIndex;
		readonly Transform2D transform2D;
		ParsedExpression expression;
		readonly AnchorData anchorData;
		CommonDrawingLocks commonDrawingLocks;
		int id;
		string name;
		#endregion
		internal DrawingObject(InternalSheetBase sheet) {
			Guard.ArgumentNotNull(sheet, "sheet");
			this.worksheet = sheet;
			transform2D = new Transform2D(sheet.Workbook as DocumentModel);
			this.commonDrawingLocks = new CommonDrawingLocks(sheet.Workbook as DocumentModel);
			anchorData = new AnchorData(sheet, commonDrawingLocks);
			id = 0;
			name = String.Empty;
		}
		internal DrawingObject(Worksheet sheet)
			: this(sheet as InternalSheetBase) {
		}
		#region Properties
		internal new DrawingObjectBatchUpdateHelper BatchUpdateHelper { get { return (DrawingObjectBatchUpdateHelper)base.BatchUpdateHelper; } }
		public int DrawingInfoIndex { get { return drawingInfoIndex; } }
		protected internal DrawingObjectInfo DrawingInfo { get { return IsUpdateLocked ? BatchUpdateHelper.DrawingInfo : DrawingInfoCore; } }
		DrawingObjectInfo DrawingInfoCore { get { return (DrawingObjectInfo)drawingObjectIndexAccessor.GetInfo(this); } }
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		public Worksheet Worksheet { get { return worksheet as Worksheet; } }
		public CommonDrawingLocks Locks {
			get { return commonDrawingLocks; }
			set {
				commonDrawingLocks = value;
				AnchorData.Locks = value;
			}
		}
		public AnchorData AnchorData { get { return anchorData; } }
		public IGraphicFrameInfo GraphicFrameInfo { get { return this; } }
		public IDrawingObjectNonVisualProperties Properties { get { return this; } }
		#region LocksWithSheet
		public bool LocksWithSheet {
			get { return DrawingInfo.LocksWithSheet; }
			set {
				if(LocksWithSheet == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetLocksWithSheet, value);
			}
		}
		DocumentModelChangeActions SetLocksWithSheet(DrawingObjectInfo info, bool value) {
			info.LocksWithSheet = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrintsWithSheet
		public bool PrintsWithSheet {
			get { return DrawingInfo.PrintsWithSheet; }
			set {
				if(PrintsWithSheet == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetPrintsWithSheet, value);
			}
		}
		DocumentModelChangeActions SetPrintsWithSheet(DrawingObjectInfo info, bool value) {
			info.PrintsWithSheet = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AnchorType
		public AnchorType AnchorType { get { return AnchorData.AnchorType; } set { AnchorData.AnchorType = value; } }
		#endregion
		#region ResizingBehavior
		public AnchorType ResizingBehavior { get { return AnchorData.ResizingBehavior; } set { AnchorData.ResizingBehavior = value; } }
		#endregion
		#region AnchorPoint
		public AnchorPoint From { get { return AnchorData.From; } set { AnchorData.From = value; } }
		public AnchorPoint To { get { return AnchorData.To; } set { AnchorData.To = value; } }
		#endregion
		#region GraphicFrameInfo
		bool IGraphicFrameInfo.IsPublished {
			get { return DrawingInfo.IsPublished; }
			set {
				if(GraphicFrameInfo.IsPublished == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetIsPublished, value);
			}
		}
		DocumentModelChangeActions SetIsPublished(DrawingObjectInfo info, bool value) {
			info.IsPublished = value;
			return DocumentModelChangeActions.None;
		}
		string IGraphicFrameInfo.Macro {
			get { return DrawingInfo.Macro; }
			set {
				if(GraphicFrameInfo.Macro == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetMacro, value);
			}
		}
		DocumentModelChangeActions SetMacro(DrawingObjectInfo info, string value) {
			info.Macro = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Height
		public float Height { get { return AnchorData.Height; } set { AnchorData.Height = value; } }
		#endregion
		#region Width
		public float Width { get { return AnchorData.Width; } set { AnchorData.Width = value; } }
		#endregion
		public Transform2D Transform2D {
			get { return transform2D; }
		}
		#region CoordinateX
		public float CoordinateX { get { return AnchorData.CoordinateX; } set { AnchorData.CoordinateX = value; } }
		#endregion
		#region CoordinateY
		public float CoordinateY { get { return AnchorData.CoordinateY; } set { AnchorData.CoordinateY = value; } }
		#endregion
		public int ZOrder { get; set; }
		#region cNvPr (Non-Visual Drawing Properties) §5.6.2.8
		#region Id
		int IDrawingObjectNonVisualProperties.Id {
			get { return id; }
			set {
				if(Properties.Id == value)
					return;
				ChangeDrawingObjectIdHistoryItem historyItem = new ChangeDrawingObjectIdHistoryItem(this, Worksheet, Properties.Id, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void SetIdCore(int newId) {
			this.id = newId;
		}
		#endregion
		#region Name
		string IDrawingObjectNonVisualProperties.Name {
			get { return name; }
			set {
				if(Properties.Name == value)
					return;
				ChangeDrawingObjectNameHistoryItem historyItem = new ChangeDrawingObjectNameHistoryItem(this, Worksheet, Properties.Name, value);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public void SetNameCore(string newName) {
			this.name = newName;
		}
		#endregion
		#region Description
		string IDrawingObjectNonVisualProperties.Description {
			get { return DrawingInfo.Description; }
			set {
				if(Properties.Description == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetDescriptionCore, value);
			}
		}
		DocumentModelChangeActions SetDescriptionCore(DrawingObjectInfo info, string value) {
			info.Description = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Hidden
		bool IDrawingObjectNonVisualProperties.Hidden {
			get { return DrawingInfo.Hidden; }
			set {
				if(Properties.Hidden == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetHiddenCore, value);
			}
		}
		DocumentModelChangeActions SetHiddenCore(DrawingObjectInfo info, bool value) {
			info.Hidden = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HyperlinkClickTooltip
		string IDrawingObjectNonVisualProperties.HyperlinkClickTooltip {
			get { return DrawingInfo.HyperlinkClickTooltip; }
			set {
				if(Properties.HyperlinkClickTooltip == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetHyperlinkClickTooltipCore, value);
			}
		}
		DocumentModelChangeActions SetHyperlinkClickTooltipCore(DrawingObjectInfo info, string value) {
			info.HyperlinkClickTooltip = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HyperlinkClickTargetFrame
		string IDrawingObjectNonVisualProperties.HyperlinkClickTargetFrame {
			get { return DrawingInfo.HyperlinkClickTargetFrame; }
			set {
				if(Properties.HyperlinkClickTargetFrame == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetHyperlinkClickTargetFrameCore, value);
			}
		}
		DocumentModelChangeActions SetHyperlinkClickTargetFrameCore(DrawingObjectInfo info, string value) {
			info.HyperlinkClickTargetFrame = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HyperlinkClickUrl
		string IDrawingObjectNonVisualProperties.HyperlinkClickUrl {
			get { return DrawingInfo.HyperlinkClickUrl; }
			set {
				if(Properties.HyperlinkClickUrl == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetHyperlinkClickUrlCore, value);
			}
		}
		DocumentModelChangeActions SetHyperlinkClickUrlCore(DrawingObjectInfo info, string value) {
			info.HyperlinkClickUrl = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HyperlinkClickIsExternal
		bool IDrawingObjectNonVisualProperties.HyperlinkClickIsExternal {
			get { return DrawingInfo.HyperlinkClickIsExternal; }
			set {
				if(Properties.HyperlinkClickIsExternal == value)
					return;
				SetPropertyValue(drawingObjectIndexAccessor, SetHyperlinkClickIsExternalCore, value);
			}
		}
		DocumentModelChangeActions SetHyperlinkClickIsExternalCore(DrawingObjectInfo info, bool value) {
			info.HyperlinkClickIsExternal = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#endregion
		#region SetIndependentWidth
		public void SetIndependentWidth(float value) {
			AnchorData.SetIndependentWidth(value);
		}
		#endregion
		#region SetIndependentHeight
		public void SetIndependentHeight(float value) {
			AnchorData.SetIndependentHeight(value);
		}
		#endregion
		public void CoordinatesFromCellKey(CellKey cellKey) {
			AnchorData.CoordinatesFromCellKey(cellKey);
		}
		public void SizeFromCellKey(CellKey cellKey) {
			AnchorData.SizeFromCellKey(cellKey);
		}
		public AnchorType GetResizingAnchorType() {
			return AnchorData.GetResizingAnchorType();
		}
		public void Move(float offsetY, float offsetX) {
			AnchorData.Move(offsetY, offsetX);
		}
		public void SwapHeightAndWidth() {
			AnchorData.SwapHeightAndWidth();
		}
		public void EnlargeWidth(float scale) {
			AnchorData.EnlargeWidth(scale, Locks.NoChangeAspect);
		}
		public void EnlargeHeight(float scale) {
			AnchorData.EnlargeHeight(scale, Locks.NoChangeAspect);
		}
		public void ReduceWidth(float scale) {
			AnchorData.ReduceWidth(scale, Locks.NoChangeAspect);
		}
		public void ReduceHeight(float scale) {
			AnchorData.ReduceHeight(scale, Locks.NoChangeAspect);
		}
		#region MultiIndexObject members
		protected override IIndexAccessorBase<DrawingObject, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		protected override IDocumentModel GetDocumentModel() {
			return Worksheet.Workbook;
		}
		public override DrawingObject GetOwner() {
			return this;
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new DrawingObjectBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new DrawingObjectBatchInitHelper(this);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override void ApplyChanges(DocumentModelChangeActions actions) {
		}
		#endregion
		#region Index Management
		internal void AssignDrawingInfoIndex(int value) {
			this.drawingInfoIndex = value;
		}
		#endregion
		#region ICloneable<DrawingObjectInfo> Members
		public DrawingObject Clone() {
			DrawingObject result = new DrawingObject(this.Worksheet);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingObjectInfo> Members
		public void CopyFrom(DrawingObject value) {
			BeginUpdate();
			try {
				Guard.ArgumentNotNull(value, "DrawingObjectInfo");
				DrawingInfo.CopyFrom(value.DrawingInfo);
				transform2D.CopyFrom(value.Transform2D);
				AnchorData.CopyFrom(value.AnchorData);
				Properties.Id = value.Properties.Id;
				Properties.Name = value.Properties.Name;
			}
			finally {
				EndUpdate();
			}
		}
		#endregion
		public bool IsDrawingObject { get { return true; } }
		public string DisplayText { get { return String.Empty; } set { } }
		public string TooltipText { get { return ((IDrawingObjectNonVisualProperties)this).HyperlinkClickTooltip; } set { ((IDrawingObjectNonVisualProperties)this).HyperlinkClickTooltip = value; } }
		public string TargetUri { get { return ((IDrawingObjectNonVisualProperties)this).HyperlinkClickUrl.Replace("#",""); } }
		public bool IsExternal {
			get {
				return String.IsNullOrEmpty(TargetUri) || ((IDrawingObjectNonVisualProperties)this).HyperlinkClickIsExternal;
			}
			set { ((IDrawingObjectNonVisualProperties)this).HyperlinkClickIsExternal = value; }
		}
		public CellRange Range { get { return null; } }
		public DocumentModel Workbook { get { return DocumentModel; } }
		public ParsedExpression Expression {
			get {
				if(expression == null)
					expression = HyperlinkExpressionParser.Parse(DocumentModel.DataContext, TargetUri, IsExternal);
				return expression;
			}
		}
		public void SetTargetUriWithoutHistory(string uri) {
			((IDrawingObjectNonVisualProperties)this).HyperlinkClickUrl = uri;
			expression = null;
		}
		public CellRangeBase GetTargetRange() {
			return HyperlinkExpressionParser.GetTargetRange(DocumentModel.DataContext, Expression, IsExternal);
		}
		void IDrawingObjectNonVisualProperties.RemoveHyperlink() {
			SetTargetUriWithoutHistory(String.Empty);
			IsExternal = false;
			TooltipText = String.Empty;
		}
	}
	#endregion
	#region GraphicFrameInfo
	public interface IGraphicFrameInfo {
		bool IsPublished { get; set; } 
		string Macro { get; set; }
		Transform2D Transform2D { get; }
	}
	#endregion
	#region PositionPair
	struct PositionPair {
		readonly int cellIndex;
		readonly float offset;
		public PositionPair(int cellIndex, float offset) {
			this.cellIndex = cellIndex;
			this.offset = offset;
		}
		public int CellIndex { get { return cellIndex; } }
		public float Offset { get { return offset; } }
	}
	#endregion
	#region MoveAnchorPointResult
	struct MoveAnchorPointResult {
		public AnchorPoint Point { get; set; }
		public float OffsetX { get; set; }
		public float OffsetY { get; set; }
	}
	#endregion
	#region CalculateNewPositionPairResult
	struct CalculateNewPositionPairResult {
		readonly PositionPair pair;
		readonly float offset;
		public CalculateNewPositionPairResult(PositionPair positionPair, float offset) {
			pair = positionPair;
			this.offset = offset;
		}
		public PositionPair PositionPair { get { return pair; } }
		public float Offset { get { return offset; } }
	}
	#endregion
	#region AccumulatedOffset
	class AccumulatedOffset {
		#region Fields
		float sizeInLayout;
		float sizeInModel;
		float gaps;
		readonly Worksheet worksheet;
		readonly IColumnWidthCalculationService calculator;
		readonly int twoPixelsInModelUnits;
		readonly float defaultRowHeightInLayoutUnits;
		#endregion
		#region Properties
		public float SizeInLayout { get { return sizeInLayout; } set { sizeInLayout = value; } }
		public float SizeInModel { get { return sizeInModel; } set { sizeInModel = value; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public IColumnWidthCalculationService Calculator { get { return calculator; } }
		public float SizeInCharactersCustom { get; set; }
		public int DefCount { get; set; }
		#endregion
		public AccumulatedOffset(Worksheet worksheet) {
			this.worksheet = worksheet;
			sizeInLayout = 0;
			sizeInModel = 0;
			SizeInCharactersCustom = 0;
			DefCount = 0;
			gaps = 0;
			calculator = worksheet.Workbook.GetService<IColumnWidthCalculationService>();
			System.Diagnostics.Debug.Assert(calculator != null);
			twoPixelsInModelUnits = worksheet.Workbook.UnitConverter.PixelsToModelUnits(2, DocumentModel.DpiX);
			defaultRowHeightInLayoutUnits = worksheet.Workbook.CalculateDefaultRowHeightInLayoutUnits();
		}
		#region Converters
		public float ToModel() {
			float maxDigitWitdh = Worksheet.Workbook.MaxDigitWidth;
			float result = SizeInModel;
			float tempResult =
				ColumnWidthCalculationUtils.MeasureCellWidth(gaps, maxDigitWitdh) +
				calculator.CalculateDefaultColumnWidth(Worksheet, Worksheet.Workbook.MaxDigitWidth, Worksheet.Workbook.MaxDigitWidthInPixels) *
				DefCount + SizeInLayout;
			result += Worksheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(tempResult);
			return result;
		}
		public float ToLayout() {
			float maxDigitWitdh = Worksheet.Workbook.MaxDigitWidth;
			float result = SizeInLayout;
			result +=
				Worksheet.Workbook.ToDocumentLayoutUnitConverter.ToLayoutUnits(sizeInModel) +
				ColumnWidthCalculationUtils.MeasureCellWidth(gaps, maxDigitWitdh) +
				calculator.CalculateDefaultColumnWidth(Worksheet, Worksheet.Workbook.MaxDigitWidth, Worksheet.Workbook.MaxDigitWidthInPixels) *
				DefCount;
			return result;
		}
		#endregion
		#region Columns
		public void AddColumns(int startIndex, int endIndex) {
			startIndex = Math.Max(startIndex, 0);
			endIndex = Math.Min(endIndex, Worksheet.MaxColumnCount - 1);
			int defaultsCount = endIndex - startIndex + 1;
			foreach(Column column in Worksheet.Columns.GetExistingColumns()) {
				if(column.EndIndex < startIndex)
					continue;
				if(column.StartIndex > endIndex)
					break;
				int realStartIndex = Math.Max(startIndex, column.StartIndex);
				int realEndIndex = Math.Min(endIndex, column.EndIndex);
				int count = realEndIndex - realStartIndex + 1;
				AddColumn(column.StartIndex, count);
				DefCount -= count;
			}
			DefCount += defaultsCount;
		}
		void AddColumn(int index, int count = 1) {
			if(index >= worksheet.MaxColumnCount)
				return;
			Column column = worksheet.Columns.GetColumnRangeForReading(index);
			if(!column.IsVisible)
				return;
			if(!column.IsCustomWidth && column.Width == 0) {
				DefCount += count;
			} else {
				float width = column.Width;
				gaps += ColumnWidthCalculationUtils.AddGaps(Worksheet, width) * count;
				SizeInCharactersCustom += width * count;
			}
		}
		void RemoveColumn(int index) {
			Column column = worksheet.Columns.GetColumnRangeForReading(index);
			if(!column.IsCustomWidth && column.Width == 0) {
				DefCount--;
			} else if(column.IsVisible) {
				float width = column.Width;
				gaps -= ColumnWidthCalculationUtils.AddGaps(Worksheet, width);
				SizeInCharactersCustom -= width;
			}
		}
		#endregion
		#region Rows
		public static PositionPair CoordinateYToPositionPair(Worksheet worksheet, PositionPair start, float coordinate, out bool needRecalculateSize) {
			needRecalculateSize = false;
			AccumulatedOffset leftOffset = new AccumulatedOffset(worksheet);
			leftOffset.AddRows(0, start.CellIndex - 1);
			coordinate += leftOffset.ToModel() + start.Offset;
			int defaultsCount = 0;
			int prevRowIndex = -1;
			int customsCount = 0;
			bool needCalculateDefaults = true;
			AccumulatedOffset oneDefaultRow = new AccumulatedOffset(worksheet);
			oneDefaultRow.AddDefaultRowValue(1);
			float defaultRowHeight = oneDefaultRow.ToModel();
			AccumulatedOffset customs = new AccumulatedOffset(worksheet);
			foreach(Row row in worksheet.Rows.GetExistingRows()) {
				defaultsCount += (row.Index - prevRowIndex - 1);
				prevRowIndex = row.Index;
				if(coordinate < defaultRowHeight * defaultsCount + customs.ToModel()) {
					defaultsCount += (int) Math.Floor((coordinate - customs.ToModel()) / defaultRowHeight);
					break;
				}
				customs.AddRow(row.Index);
				if(coordinate < defaultRowHeight * defaultsCount + customs.ToModel()) {
					customs.RemoveRow(row.Index);
					needCalculateDefaults = false;
					break;
				}
				customsCount++;
			}
			if(needCalculateDefaults) {
				float defaultsCountF = coordinate / defaultRowHeight;
				double coordinateF = defaultsCountF * defaultRowHeight;
				defaultsCount = (int) Math.Floor(coordinateF / defaultRowHeight);
			}
			coordinate -= customs.ToModel() + defaultsCount * defaultRowHeight;
			int cellIndex = customsCount + defaultsCount;
			if(cellIndex >= worksheet.MaxRowCount) {
				AccumulatedOffset accumulatedOffset = new AccumulatedOffset(worksheet);
				accumulatedOffset.AddCell(worksheet.MaxRowCount - 1, true);
				needRecalculateSize = true;
				return new PositionPair(worksheet.MaxRowCount - 1, accumulatedOffset.ToModel());
			}
			return new PositionPair(cellIndex, coordinate);
		}
		public void AddRows(int startIndex, int endIndex) {
			int defaultsCount = endIndex - startIndex + 1;
			startIndex = Math.Max(startIndex, 0);
			endIndex = Math.Min(endIndex, Worksheet.MaxRowCount - 1);
			foreach(Row row in worksheet.Rows.GetExistingRows(startIndex, endIndex, false)) {
				defaultsCount--;
				AddRow(row.Index);
			}
			AddDefaultRowValue(defaultsCount);
		}
		void AddRow(int index) {
			if(index >= worksheet.MaxRowCount)
				return;
			Row row = worksheet.Rows.GetRowForReading(index);
			if(row == null) {
				SheetFormatProperties properties = worksheet.Properties.FormatProperties;
				if(properties.IsCustomHeight && properties.DefaultRowHeight > 0) {
					sizeInModel += properties.DefaultRowHeight;
				}
				else {
					sizeInModel += twoPixelsInModelUnits;
					sizeInLayout += defaultRowHeightInLayoutUnits;
				}
			}
			else if(!row.IsHidden) {
				if(row.IsCustomHeight)
					sizeInModel += row.Height;
				else {
					if(row.Cells.Count != 0) {
						float value = calculator.CalculateRowHeight(worksheet, index);
						sizeInLayout += value;
					}
					else
						AddDefaultRowValue(1);
				}
			}
		}
		void AddDefaultRowValue(int defaultsCount) {
			SheetFormatProperties formatProperties = worksheet.Properties.FormatProperties;
			if(formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0)
				sizeInModel += formatProperties.DefaultRowHeight * defaultsCount;
			else {
				float maxRowHeight = worksheet.Workbook.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips);
				float defaultRowHeightLayoutUnits = calculator.CalculateDefaultRowHeight(worksheet);
				float defaultRowHeightModelUnits = worksheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(defaultRowHeightLayoutUnits);
				if(defaultRowHeightModelUnits < maxRowHeight) {
					sizeInLayout += defaultRowHeightLayoutUnits * defaultsCount;
				}
				else {
					sizeInModel += maxRowHeight * defaultsCount;
				}
			}
		}
		void RemoveRow(int index) {
			Row row = worksheet.Rows.GetRowForReading(index);
			if(row == null) {
				SheetFormatProperties properties = worksheet.Properties.FormatProperties;
				if(properties.IsCustomHeight && properties.DefaultRowHeight > 0) {
					sizeInModel -= properties.DefaultRowHeight;
				} else {
					sizeInModel -= worksheet.Workbook.UnitConverter.PixelsToModelUnits(2, DocumentModel.DpiX);
					sizeInLayout -= worksheet.Workbook.CalculateDefaultRowHeightInLayoutUnits();
				}
			} else if(!row.IsHidden) {
				if(row.IsCustomHeight)
					sizeInModel -= row.Height;
				else {
					SheetFormatProperties formatProperties = worksheet.Properties.FormatProperties;
					if(formatProperties.IsCustomHeight && formatProperties.DefaultRowHeight > 0 && row.Height == 0)
						sizeInModel -= formatProperties.DefaultRowHeight;
					else {
						sizeInLayout -= calculator.CalculateRowMaxCellHeight(row, worksheet.Workbook.MaxDigitWidth,
																			 worksheet.Workbook.MaxDigitWidthInPixels);
					}
				}
			}
		}
		#endregion
		#region Cells
		public void AddCells(int startIndex, int endIndex, bool isRow) {
			if(isRow)
				AddRows(startIndex, endIndex);
			else {
				AddColumns(startIndex, endIndex);
			}
		}
		public void AddCell(int index, bool isRow) {
			if(isRow)
				AddRow(index);
			else
				AddColumn(index);
		}
		public void RemoveCell(int index, bool isRow) {
			if(isRow)
				RemoveRow(index);
			else
				RemoveColumn(index);
		}
		#endregion
		public static float GetOneColumnWidth(Worksheet worksheet, int columnIndex) {
			AccumulatedOffset offset = new AccumulatedOffset(worksheet);
			offset.AddColumn(columnIndex);
			return offset.ToModel();
		}
		public static float GetOneRowHeight(Worksheet worksheet, int rowIndex) {
			AccumulatedOffset offset = new AccumulatedOffset(worksheet);
			offset.AddRow(rowIndex);
			return offset.ToModel();
		}
		public static float GetColumnsWidth(Worksheet worksheet, int startIndex, int endIndex) {
			AccumulatedOffset offset = new AccumulatedOffset(worksheet);
			offset.AddColumns(startIndex, endIndex);
			return offset.ToModel();
		}
	}
	#endregion
	#endregion
	#region DrawingEffectWalker
	public class DrawingEffectWalker : IDrawingEffectVisitor {
		#region fields
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823")]
		Image image;
		DrawingEffectCollection effects;
		float[][] colorMatrixElements = new float[5][];
		#endregion
		public DrawingEffectWalker(IDrawingObject drawingObject) {
			Picture picture = drawingObject as Picture;
			if (picture != null) {
				this.image = picture.Image.NativeImage;
				this.effects = picture.PictureFill.Blip.Effects;
			}
			else {
				this.image = null;
				this.effects = null;
			}
			colorMatrixElements[0] = new float[] { 1, 0, 0, 0, 0 };
			colorMatrixElements[1] = new float[] { 0, 1, 0, 0, 0 };
			colorMatrixElements[2] = new float[] { 0, 0, 1, 0, 0 };
			colorMatrixElements[3] = new float[] { 0, 0, 0, 1, 0 };
			colorMatrixElements[4] = new float[] { 0, 0, 0, 0, 1 };
		}
		#region properties
		public float[][] ColorMatrixElements { get { return colorMatrixElements; } }
		#endregion
		public void Walk() {
			if (effects != null)
				effects.ForEach(ApplyEffect);
		}
		void ApplyEffect(IDrawingEffect effect) {
			effect.Visit(this);
		}
		#region IDrawingEffectVisitor Members
		public virtual void AlphaCeilingEffectVisit() {
		}
		public virtual void AlphaFloorEffectVisit() {
		}
		public virtual void GrayScaleEffectVisit() {
			float coef = .2f;
			colorMatrixElements[0][0] = coef;
			colorMatrixElements[0][1] = coef;
			colorMatrixElements[0][2] = coef;
			colorMatrixElements[1][0] = coef;
			colorMatrixElements[1][1] = coef;
			colorMatrixElements[1][2] = coef;
			colorMatrixElements[2][0] = coef;
			colorMatrixElements[2][1] = coef;
			colorMatrixElements[2][2] = coef;
		}
		public virtual void Visit(AlphaBiLevelEffect drawingEffect) {
		}
		public virtual void Visit(AlphaInverseEffect drawingEffect) {
		}
		public virtual void Visit(AlphaModulateEffect drawingEffect) {
		}
		public virtual void Visit(AlphaModulateFixedEffect drawingEffect) {
		}
		public virtual void Visit(AlphaOutsetEffect drawingEffect) {
		}
		public virtual void Visit(AlphaReplaceEffect drawingEffect) {
		}
		public virtual void Visit(BiLevelEffect drawingEffect) {
		}
		public virtual void Visit(BlendEffect drawingEffect) {
		}
		public virtual void Visit(BlurEffect drawingEffect) {
		}
		public virtual void Visit(ColorChangeEffect drawingEffect) {
		}
		public virtual void Visit(ContainerEffect drawingEffect) {
		}
		public virtual void Visit(DuotoneEffect drawingEffect) {
		}
		public virtual void Visit(Effect drawingEffect) {
		}
		public virtual void Visit(FillEffect drawingEffect) {
		}
		public virtual void Visit(FillOverlayEffect drawingEffect) {
		}
		public virtual void Visit(GlowEffect drawingEffect) {
		}
		public virtual void Visit(HSLEffect drawingEffect) {
		}
		public virtual void Visit(InnerShadowEffect drawingEffect) {
		}
		public virtual void Visit(LuminanceEffect drawingEffect) {
			if (drawingEffect.Bright != 0) {
				float bright = drawingEffect.Bright / DrawingValueConstants.ThousandthOfPercentage;
				colorMatrixElements[3][0] = bright;
				colorMatrixElements[3][1] = bright;
				colorMatrixElements[3][2] = bright;
			}
			if (drawingEffect.Contrast != 0) {
				int coef = 50000; 
				float contrast = drawingEffect.Contrast / coef;
				colorMatrixElements[0][0] = contrast;
				colorMatrixElements[1][1] = contrast;
				colorMatrixElements[2][2] = contrast;
				float bright = -drawingEffect.Contrast / DrawingValueConstants.ThousandthOfPercentage;
				colorMatrixElements[3][0] = bright;
				colorMatrixElements[3][1] = bright;
				colorMatrixElements[3][2] = bright;
			}
		}
		public virtual void Visit(OuterShadowEffect drawingEffect) {
		}
		public virtual void Visit(PresetShadowEffect drawingEffect) {
		}
		public virtual void Visit(ReflectionEffect drawingEffect) {
		}
		public virtual void Visit(RelativeOffsetEffect drawingEffect) {
		}
		public virtual void Visit(SoftEdgeEffect drawingEffect) {
		}
		public virtual void Visit(SolidColorReplacementEffect drawingEffect) {
		}
		public virtual void Visit(TintEffect drawingEffect) {
		}
		public virtual void Visit(TransformEffect drawingEffect) {
		}
		#endregion
	}
	#endregion
}
