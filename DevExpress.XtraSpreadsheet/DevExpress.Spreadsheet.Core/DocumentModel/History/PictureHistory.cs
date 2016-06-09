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
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region DrawingObjectHistoryItems
	#region DrawingObjectHistoryItem
	public abstract class DrawingObjectHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly DrawingObject obj;
		static IDocumentModelPart GetModelPart(DrawingObject obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected DrawingObjectHistoryItem(DrawingObject obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected DrawingObject Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region DrawingObjectIndexChangeHistoryItem
	public class DrawingObjectIndexChangeHistoryItem : DrawingObjectHistoryItem {
		public DrawingObjectIndexChangeHistoryItem(DrawingObject obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(DrawingObject.DrawingObjectIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(DrawingObject.DrawingObjectIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	public abstract class ChangeAnchorPointHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly AnchorData anchorData;
		readonly AnchorPoint oldValue;
		readonly AnchorPoint newValue;
		#endregion
		protected ChangeAnchorPointHistoryItem(AnchorData anchorData, AnchorPoint oldValue, AnchorPoint newValue)
			: base(anchorData.Worksheet) {
			this.anchorData = anchorData;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public AnchorData AnchorData { get { return anchorData; } }
		public AnchorPoint OldValue { get { return oldValue; } }
		public AnchorPoint NewValue { get { return newValue; } }
		#endregion
	}
	public class ChangeDrawingObjectStartingPositionHistoryItem : ChangeAnchorPointHistoryItem {
		#region Fields
		#endregion
		public ChangeDrawingObjectStartingPositionHistoryItem(AnchorData drawingObj, AnchorPoint oldValue, AnchorPoint newValue)
			: base(drawingObj, oldValue, newValue) {
		}
		protected override void UndoCore() {
			AnchorData.SetStartingPosition(OldValue);
		}
		protected override void RedoCore() {
			AnchorData.SetStartingPosition(NewValue);
		}
	}
	public class ChangeDrawingObjectEndingPositionHistoryItem : ChangeAnchorPointHistoryItem {
		#region Fields
		#endregion
		public ChangeDrawingObjectEndingPositionHistoryItem(AnchorData anchorData, AnchorPoint oldValue, AnchorPoint newValue)
			: base(anchorData, oldValue, newValue) {
		}
		protected override void UndoCore() {
			AnchorData.SetEndingPosition(OldValue);
		}
		protected override void RedoCore() {
			AnchorData.SetEndingPosition(NewValue);
		}
	}
	#region DrawingObjectMoveAbsoluteHistoryItem
	public class DrawingObjectMoveAbsoluteHistoryItem : HistoryItem {
		#region Fields
		readonly AnchorData anchorData;
		readonly float offsetX, offsetY;
		#endregion
		public DrawingObjectMoveAbsoluteHistoryItem(AnchorData anchorData, float offsetX, float offsetY)
			: base(anchorData.Worksheet) {
			this.anchorData = anchorData;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}
		protected override void UndoCore() {
			anchorData.CoordinateX -= offsetX;
			anchorData.CoordinateY -= offsetY;
		}
		protected override void RedoCore() {
			anchorData.CoordinateX += offsetX;
			anchorData.CoordinateY += offsetY;
		}
	}
	#endregion
	#region ChangeDrawingObjectIdHistoryItem
	public class ChangeDrawingObjectIdHistoryItem : SpreadsheetIntHistoryItem {
		#region Fields
		readonly DrawingObject drawingObject;
		#endregion
		public ChangeDrawingObjectIdHistoryItem(DrawingObject drawingObject, Worksheet worksheet, int oldValue, int newValue) : base(worksheet, oldValue, newValue) {
			this.drawingObject = drawingObject;
		}
		#region Overrides of HistoryItem
		protected override void UndoCore() {
			drawingObject.SetIdCore(OldValue);
		}
		protected override void RedoCore() {
			drawingObject.SetIdCore(NewValue);
		}
		#endregion
	}
	#endregion
	#region ChangeDrawingObjectNameHistoryItem
	public class ChangeDrawingObjectNameHistoryItem : SpreadsheetStringHistoryItem {
		#region Fields
		readonly DrawingObject drawingObject;
		#endregion
		public ChangeDrawingObjectNameHistoryItem(DrawingObject drawingObject, Worksheet worksheet, string oldValue, string newValue)
			: base(worksheet, oldValue, newValue) {
			this.drawingObject = drawingObject;
		}
		#region Overrides of HistoryItem
		protected override void UndoCore() {
			drawingObject.SetNameCore(OldValue);
		}
		protected override void RedoCore() {
			drawingObject.SetNameCore(NewValue);
		}
		#endregion
	}
	#endregion
	#endregion
	#region PictureHystoryItems
	public class PictureInsertedHistoryItem : SpreadsheetHistoryItem {
		Picture picture;
		OfficeImage image;
		CellKey topLeftKey;
		CellKey bottomRightKey;
		AnchorType anchorType;
		bool lockAspectRatio;
		int position;
		public PictureInsertedHistoryItem(Worksheet worksheet)
			: base(worksheet) {
		}
		public OfficeImage Image { get { return image; } set { image = value; } }
		public CellKey TopLeft { get { return topLeftKey; } set { this.topLeftKey = value; } }
		public CellKey BottomRight { get { return bottomRightKey; } set { this.bottomRightKey = value; } }
		public AnchorType AnchorType { get { return anchorType; } set { this.anchorType = value; } }
		public bool LockAspectRatio { get { return this.lockAspectRatio; } set { this.lockAspectRatio = value; } }
		public Picture Picture { get { return picture; } }
		protected override void Dispose(bool disposing) {
			try {
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void RedoCore() {
			position = Worksheet.DrawingObjects.Add(picture);
		}
		protected override void UndoCore() {
			Worksheet.DrawingObjects.RemoveAt(position);
		}
		public override void Execute() {
			this.picture = InsertPicture(image, LockAspectRatio);
			OfficeNativeImage nativeImage = picture.Image.RootImage as OfficeNativeImage;
			if(nativeImage != null)
				this.Workbook.ImageCache.RegisterImage(nativeImage);
		}
		Picture InsertPicture(OfficeImage image, bool lockAspectRatio) {
			Picture result = new Picture(Worksheet);
			int pictureWidth = image.SizeInTwips.Width;
			int pictureHeight = image.SizeInTwips.Height;
			if(lockAspectRatio) {
				if(pictureHeight > 15840) {
					float k = pictureHeight / 15840f;
					pictureHeight = 15840;
					pictureWidth = (int)(pictureWidth / k);
				}
				if(pictureWidth > 15840) {
					float k = pictureWidth / 15840f;
					pictureWidth = 15840;
					pictureHeight = (int)(pictureHeight / k);
				}
			}
			SetPicturePosition(result, topLeftKey, bottomRightKey, pictureWidth, pictureHeight);
			result.Locks.NoChangeAspect = lockAspectRatio;
			result.Image = image;
			if (result.AnchorType == AnchorType.Absolute) {
				result.PictureFill.Stretch = true;
			} else {
				result.PictureFill.FillRectangle = new RectangleOffset();
			}
			DefinePictureId(result);
			result.DrawingObject.Properties.Name = string.Format("Picture {0}", result.DrawingObject.Properties.Id - 1);
			result.PictureFill.Stretch = true;
			position = Worksheet.DrawingObjects.Add(result);
			return result;
		}
		protected virtual void SetPicturePosition(Picture result,  CellKey topLeft1, CellKey bottomRight1, int pictureWidth, int pictureHeight) {
			float desiredWidth = AnchorData.CellKeyToWidth(Worksheet, topLeft1.ColumnIndex, bottomRight1.ColumnIndex);
			float desiredHeight = AnchorData.CellKeyToHeight(Worksheet, topLeft1.RowIndex, bottomRight1.RowIndex);
			DrawingObjectsDimensionCalculator calculator = new DrawingObjectsDimensionCalculator(pictureWidth, pictureHeight, desiredWidth, desiredHeight, lockAspectRatio);
			calculator.Calculate();
			result.AnchorType = AnchorType;
			result.From = new AnchorPoint(topLeft1);
			result.To = new AnchorPoint(bottomRight1);
			result.Height = calculator.ResultHeight;
			result.Width = calculator.ResultWidth;
			result.AnchorData.ValidateTo();
			result.AnchorData.ValidateWidth();
			result.AnchorData.ValidateHeight();
		}
		protected internal void DefinePictureId(Picture result) {
			int id = 1;
			for (int i = 0; i < Worksheet.DrawingObjects.Count; i++)
				id = Math.Max(Worksheet.DrawingObjects[i].DrawingObject.Properties.Id, id);
			result.DrawingObject.Properties.Id = id + 1;
		}
	}
	public class PictureAtAbsoluteCoordinatesInsertedHistoryItem : PictureInsertedHistoryItem {
		float coordinateX;
		float coordinateY;
		float desiredWidth;
		float desiredHeight;
		public PictureAtAbsoluteCoordinatesInsertedHistoryItem(Worksheet worksheet)
			: base(worksheet) {
		}
		#region Properties
		public float CoordinateX { get { return coordinateX; } set { coordinateX = value; } }
		public float CoordinateY { get { return coordinateY; } set { coordinateY = value; } }
		public float DesiredWidth { get { return this.desiredWidth; } set { this.desiredWidth = value; } }
		public float DesiredHeight { get { return this.desiredHeight; } set { this.desiredHeight = value; } }
		#endregion
		protected override void SetPicturePosition(Picture result, CellKey topLeft, CellKey bottomRight, int pictureWidth, int pictureHeight) {
			DrawingObjectsDimensionCalculator calculator = new DrawingObjectsDimensionCalculator(pictureWidth, pictureHeight, DesiredWidth, DesiredHeight, LockAspectRatio);
			calculator.Calculate();
			result.AnchorType = AnchorType.Absolute;
			result.CoordinateX = CoordinateX;
			result.CoordinateY = coordinateY;
			result.Width = calculator.ResultWidth;
			result.Height = calculator.ResultHeight;
		}
	}
	#region DrawingRemoveAtHistoryItem
	public class DrawingRemoveAtHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		int index;
		IDrawingObject drawing;
		int zOrder;
		#endregion
		public DrawingRemoveAtHistoryItem(Worksheet worksheet, int index)
			: base(worksheet) {
			Guard.ArgumentNonNegative(index, "index");
			this.index = index;
		}
		protected override void RedoCore() {
			DrawingObjectsCollection drawings = Worksheet.DrawingObjects;
			drawing = drawings[index];
			zOrder = drawing.ZOrder;
			drawings.RemoveAt(index);
			if(drawing.DrawingType == DrawingObjectType.Chart)
				((Chart) drawing).Deactivate();
			if(Worksheet.Workbook.IsUpdateLocked)
				Worksheet.Workbook.ApplyChanges(DocumentModelChangeType.DrawingRemoved);
			else
				Worksheet.Workbook.InnerApplyChanges(DocumentModelChangeActionsCalculator.CalculateChangeActions(DocumentModelChangeType.DrawingRemoved));
		}
		protected override void UndoCore() {
			Worksheet.DrawingObjects.Insert(index, drawing, zOrder);
			if (drawing.DrawingType == DrawingObjectType.Chart)
				((Chart)drawing).Activate();
			drawing = null;
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (drawing != null) {
						drawing.Dispose();
						drawing = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
	#endregion
	#endregion
	#region SetZOrderHistoryItem
	public class SetZOrderHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly IDrawingObject drawingObject;
		readonly int newZOrder;
		readonly int oldZOrder;
		readonly ZOrderRange zOrderRange;
		#endregion
		public SetZOrderHistoryItem(IDrawingObject drawingObject, int newZOrder, int oldZOrder)
			: base(drawingObject.Worksheet.Workbook) { 
			this.drawingObject = drawingObject;
			this.newZOrder = newZOrder;
			this.oldZOrder = oldZOrder;
			zOrderRange = drawingObject.Worksheet.DrawingObjectsByZOrderCollections.PrepareZOrderRange(drawingObject, newZOrder);
		}
		protected override void UndoCore() {
			drawingObject.Worksheet.DrawingObjectsByZOrderCollections.UndoZOrder(drawingObject, oldZOrder, newZOrder, zOrderRange);
		}
		protected override void RedoCore() {
			drawingObject.Worksheet.DrawingObjectsByZOrderCollections.SetZOrder(drawingObject, newZOrder);
		}
	}
	#endregion
	#region ZOrderNormalizeHistoryItem
	public class ZOrderNormalizeHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		Worksheet sheet;
		readonly List<ZOrderRange> zOrderRanges;
		#endregion
		public ZOrderNormalizeHistoryItem(Worksheet worksheet)
			: base(worksheet.Workbook) {
			this.sheet = worksheet;
			zOrderRanges = worksheet.DrawingObjectsByZOrderCollections.PrepareZOrderRangesForNormalize();
		}
		protected override void UndoCore() {
			sheet.DrawingObjectsByZOrderCollections.UndoNormalize(zOrderRanges);
		}
		protected override void RedoCore() {
			sheet.DrawingObjectsByZOrderCollections.Normalize();
		}
	}
	#endregion
	#region InvalidateAnchorDataBaseHistoryItem
	public abstract class InvalidateAnchorDataBaseHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		protected int StartIndex { get; set; }
		protected int EndIndex { get; set; }
		protected List<int> InvalidatedDrawings { get; set; }
		protected List<int> InvalidatedVmDrawings { get; set; }
		#endregion
		protected InvalidateAnchorDataBaseHistoryItem(Worksheet worksheet, int startIndex, int endIndex)
			: base(worksheet) {
			StartIndex = startIndex;
			EndIndex = endIndex;
			InvalidatedDrawings = new List<int>();
			InvalidatedVmDrawings = new List<int>();
		}
		protected override void UndoCore() {
			InvalidateByList();
		}
		protected override void RedoCore() {
			InvalidateByList();
		}
		public override void Execute() {
			InvalidateAnchorDatas();
		}
		void ForceInvalidateAnchorData(AnchorData anchorData) {
			switch(anchorData.GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					anchorData.InvalidCoordinateX = true;
					anchorData.InvalidCoordinateY = true;
					anchorData.InvalidWidth = true;
					anchorData.InvalidHeight = true;
					break;
				case AnchorType.OneCell:
					anchorData.InvalidCoordinateX = true;
					anchorData.InvalidCoordinateY = true;
					anchorData.InvalidToColumn = true;
					anchorData.InvalidToRow = true;
					break;
				case AnchorType.Absolute:
					anchorData.InvalidFromColumn = true;
					anchorData.InvalidFromRow = true;
					anchorData.InvalidToColumn = true;
					anchorData.InvalidToRow = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		void InvalidateByList() {
			foreach(int index in InvalidatedDrawings) {
				ForceInvalidateAnchorData(Worksheet.DrawingObjects[index].AnchorData);
			}
			foreach(int index in InvalidatedVmDrawings) {
				ForceInvalidateAnchorData(Worksheet.VmlDrawing.Shapes[index].ClientData.Anchor.AnchorData);
			}
		}
		void InvalidateAnchorDatas() {
			for(int i = 0; i < Worksheet.DrawingObjects.Count; i++) {
				AnchorData anchorData = Worksheet.DrawingObjects[i].AnchorData;
				if(InvalidateAnchorData(anchorData)) {
					InvalidatedDrawings.Add(i);
				}
			}
			for(int i = 0; i < Worksheet.VmlDrawing.Shapes.Count; i++) {
				AnchorData anchorData = Worksheet.VmlDrawing.Shapes[i].ClientData.Anchor.AnchorData;
				if(InvalidateAnchorData(anchorData)) {
					InvalidatedVmDrawings.Add(i);
				}
			}
		}
		bool InvalidateAnchorData(AnchorData anchorData) {
			if(IsInvalidated(anchorData)) {
				anchorData.ValidateAll();
			}
			switch(anchorData.GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					return InvalidateTwoCellAnchor(anchorData);
				case AnchorType.OneCell:
					return InvalidateOneCellAnchor(anchorData);
				case AnchorType.Absolute:
					return InvalidateAbsoluteAnchor(anchorData);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		static bool IsInvalidated(AnchorData anchorData) {
			switch(anchorData.GetResizingAnchorType()) {
				case AnchorType.TwoCell:
					return anchorData.InvalidFromColumn || anchorData.InvalidFromRow || anchorData.InvalidToColumn || anchorData.InvalidToRow;
				case AnchorType.OneCell:
					return anchorData.InvalidFromColumn || anchorData.InvalidFromRow || anchorData.InvalidWidth || anchorData.InvalidHeight;
				case AnchorType.Absolute:
					return anchorData.InvalidCoordinateX || anchorData.InvalidCoordinateY || anchorData.InvalidWidth || anchorData.InvalidHeight;
			}
			return false;
		}
		protected abstract bool InvalidateTwoCellAnchor(AnchorData anchorData);
		protected abstract bool InvalidateOneCellAnchor(AnchorData anchorData);
		protected abstract bool InvalidateAbsoluteAnchor(AnchorData anchorData);
	}
	#endregion
	#region InvalidateAnchorDatasByColumnHistoryItem
	public class InvalidateAnchorDatasByColumnHistoryItem : InvalidateAnchorDataBaseHistoryItem {
		#region Overrides of HistoryItem
		public InvalidateAnchorDatasByColumnHistoryItem(Worksheet worksheet, int startIndex, int endIndex) : base(worksheet, startIndex, endIndex) {}
		protected override bool InvalidateTwoCellAnchor(AnchorData anchorData) {
			bool result = false;
			AnchorPoint from = anchorData.From;
			AnchorPoint to = anchorData.To;
			if(StartIndex <= from.Col) {
				anchorData.InvalidCoordinateX = true;
				result = true;
			}
			if((from.Col <= StartIndex && StartIndex <= to.Col) || (from.Col <= EndIndex && EndIndex <= to.Col)) {
				anchorData.InvalidWidth = true;
				result = true;
			}
			return result;
		}
		protected override bool InvalidateOneCellAnchor(AnchorData anchorData) {
			bool result = false;
			AnchorPoint from = anchorData.From;
			if(StartIndex <= from.Col) {
				anchorData.InvalidCoordinateX = true;
				result = true;
			}
			if(!anchorData.InvalidToColumn) {
				AnchorPoint to = anchorData.To;
				if(StartIndex <= to.Col) {
					anchorData.InvalidToColumn = true;
					result = true;
				}
			}
			return result;
		}
		protected override bool InvalidateAbsoluteAnchor(AnchorData anchorData) {
			bool result = false;
			if(!anchorData.InvalidFromColumn) {
				InvalidateFrom(anchorData);
				result = true;
			}
			if(!anchorData.InvalidToColumn) {
				InvalidateTo(anchorData);
				result = true;
			}
			return result;
		}
		void InvalidateTo(AnchorData anchorData) {
			AnchorPoint to = anchorData.To;
			if(StartIndex <= to.Col) {
				anchorData.InvalidToColumn = true;
			}
		}
		void InvalidateFrom(AnchorData anchorData) {
			AnchorPoint from = anchorData.From;
			if(StartIndex <= @from.Col) {
				anchorData.InvalidFromColumn = true;
			}
		}
		#endregion
	}
	#endregion
	#region InvalidateAnchorDatasByRowHistoryItem
	public class InvalidateAnchorDatasByRowHistoryItem : InvalidateAnchorDataBaseHistoryItem {
		#region Overrides of HistoryItem
		public InvalidateAnchorDatasByRowHistoryItem(Worksheet worksheet, int index) : base(worksheet, index, index) {}
		protected override bool InvalidateTwoCellAnchor(AnchorData anchorData) {
			bool result = false;
			AnchorPoint from = anchorData.From;
			AnchorPoint to = anchorData.To;
			if(StartIndex <= from.Row) {
				anchorData.InvalidCoordinateY = true;
				result = true;
			}
			if((from.Row <= StartIndex && StartIndex <= to.Row) || (from.Row <= EndIndex && EndIndex <= to.Row)) {
				anchorData.InvalidHeight = true;
				result = true;
			}
			return result;
		}
		protected override bool InvalidateOneCellAnchor(AnchorData anchorData) {
			bool result = false;
			AnchorPoint from = anchorData.From;
			if(StartIndex <= from.Row) {
				anchorData.InvalidCoordinateY = true;
				result = true;
			}
			if(!anchorData.InvalidToRow) {
				AnchorPoint to = anchorData.To;
				if(StartIndex <= to.Row) {
					anchorData.InvalidToRow = true;
					result = true;
				}
			}
			return result;
		}
		protected override bool InvalidateAbsoluteAnchor(AnchorData anchorData) {
			bool result = false;
			if(!anchorData.InvalidFromRow) {
				InvalidateFrom(anchorData);
				result = true;
			}
			if(!anchorData.InvalidToRow) {
				InvalidateTo(anchorData);
				result = true;
			}
			return result;
		}
		void InvalidateTo(AnchorData anchorData) {
			AnchorPoint to = anchorData.To;
			if(StartIndex <= to.Row) {
				anchorData.InvalidToRow = true;
			}
		}
		void InvalidateFrom(AnchorData anchorData) {
			AnchorPoint from = anchorData.From;
			if(StartIndex <= from.Row) {
				anchorData.InvalidFromRow = true;
			}
		}
		#endregion
	}
	#endregion
	#region SetAnchorDataPropertyHistoryItem
	public class SetAnchorDataPropertyHistoryItem : SpreadsheetHistoryItem {
		protected delegate void SetAnchorDataPropertyDelegate(float newValue);
		#region Properties
		protected float OldValue { get; set; }
		protected float NewValue { get; set; }
		protected AnchorData AnchorData { get; set; }
		protected SetAnchorDataPropertyDelegate SetPropertyDelegate { get; set; }
		#endregion
		protected SetAnchorDataPropertyHistoryItem(AnchorData anchorData, float newValue, float oldValue, SetAnchorDataPropertyDelegate setPropertyDelegate) : base(anchorData.Worksheet) {
			AnchorData = anchorData;
			NewValue = newValue;
			OldValue = oldValue;
			SetPropertyDelegate = setPropertyDelegate;
		}
		#region Overrides of HistoryItem
		protected override void UndoCore() {
			SetPropertyDelegate(OldValue);
		}
		protected override void RedoCore() {
			SetPropertyDelegate(NewValue);
		}
		#endregion
	}
	#endregion
	#region SetAnchorDataWidthHistoryItem
	public class SetAnchorDataWidthHistoryItem : SetAnchorDataPropertyHistoryItem {
		public SetAnchorDataWidthHistoryItem(AnchorData anchorData, float newWidth) : base(anchorData, newWidth, anchorData.Width, anchorData.SetIndependentWidthCore) {}
	}
	#endregion
	#region SetAnchorDataHeightHistoryItem
	public class SetAnchorDataHeightHistoryItem : SetAnchorDataPropertyHistoryItem {
		public SetAnchorDataHeightHistoryItem(AnchorData anchorData, float newHeight) : base(anchorData, newHeight, anchorData.Height, anchorData.SetIndependentHeightCore) {}
	}
	#endregion
	#region SetAnchorDataCoordinateXHistoryItem
	public class SetAnchorDataCoordinateXHistoryItem : SetAnchorDataPropertyHistoryItem {
		public SetAnchorDataCoordinateXHistoryItem(AnchorData anchorData, float newCoordinateX) : base(anchorData, newCoordinateX, anchorData.CoordinateX, anchorData.SetCoordinateXCore) {}
	}
	#endregion
	#region SetAnchorDataCoordinateYHistoryItem
	public class SetAnchorDataCoordinateYHistoryItem : SetAnchorDataPropertyHistoryItem {
		public SetAnchorDataCoordinateYHistoryItem(AnchorData anchorData, float newCoordinateY) : base(anchorData, newCoordinateY, anchorData.CoordinateY, anchorData.SetCoordinateYCore) {}
	}
	#endregion
	#region SetAnchorDataAnchorTypeHistoryItem
	public class SetAnchorDataAnchorTypeHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		AnchorType NewValue { get; set; }
		AnchorType OldValue { get; set; }
		AnchorData AnchorData { get; set; }
		#endregion
		public SetAnchorDataAnchorTypeHistoryItem(AnchorData anchorData, AnchorType newValue) : base(anchorData.Worksheet) {
			NewValue = newValue;
			OldValue = anchorData.AnchorType;
			AnchorData = anchorData;
		}
		#region Overrides of HistoryItem
		protected override void UndoCore() {
			AnchorData.SetAnchorTypeCore(OldValue);
		}
		protected override void RedoCore() {
			AnchorData.SetAnchorTypeCore(NewValue);
		}
		#endregion
	}
	#endregion
	#region SetAnchorDataAnchorTypeHistoryItem
	public class SetAnchorDataResizingBehaviourHistoryItem : SpreadsheetHistoryItem {
		#region Properties
		AnchorType NewValue { get; set; }
		AnchorType OldValue { get; set; }
		AnchorData AnchorData { get; set; }
		#endregion
		public SetAnchorDataResizingBehaviourHistoryItem(AnchorData anchorData, AnchorType newValue) : base(anchorData.Worksheet) {
			NewValue = newValue;
			OldValue = anchorData.AnchorType;
			AnchorData = anchorData;
		}
		#region Overrides of HistoryItem
		protected override void UndoCore() {
			AnchorData.SetResizingBehaviorCore(OldValue);
		}
		protected override void RedoCore() {
			AnchorData.SetResizingBehaviorCore(NewValue);
		}
		#endregion
	}
	#endregion
	#region ConnectionShapeHistoryItems
	public class ConnectionShapeSiteHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly ConnectionShape shape;
		readonly int newValue;
		readonly int oldValue;
		readonly int valueIndex;
		#endregion
		public ConnectionShapeSiteHistoryItem(ConnectionShape shape, int valueIndex, int newValue, int oldValue)
			: base(shape.DocumentModel) {
			this.shape = shape;
			this.valueIndex = valueIndex;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			shape.SetSiteProperty(valueIndex, oldValue);
		}
		protected override void RedoCore() {
			shape.SetSiteProperty(valueIndex, newValue);
		}
	}
	#endregion
	#region ModelAdjustableRectHistoryItem
	public class ModelAdjustableRectHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly ModelAdjustableRect rect;
		readonly AdjustableCoordinate newValue;
		readonly AdjustableCoordinate oldValue;
		readonly int valueIndex;
		#endregion
		public ModelAdjustableRectHistoryItem(ModelAdjustableRect rect, int valueIndex, AdjustableCoordinate oldValue, AdjustableCoordinate newValue)
			: base(rect.DocumentModelPart) {
			this.rect = rect;
			this.valueIndex = valueIndex;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			rect.SetCoordinate(valueIndex, oldValue);
		}
		protected override void RedoCore() {
			rect.SetCoordinate(valueIndex, newValue);
		}
	}
	#endregion
	#region DelegateHistoryItem<T>
	public class DelegateHistoryItem<T> : HistoryItem {
		#region Fields
		T oldValue;
		T newValue;
		Action<T> setValueAction;
		#endregion
		public DelegateHistoryItem(IDocumentModelPart documentModelPart, T oldValue, T newValue, Action<T> setValueAction)
			: base(documentModelPart) {
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.setValueAction = setValueAction;
		}
		protected override void RedoCore() {
			setValueAction(newValue);
		}
		protected override void UndoCore() {
			setValueAction(oldValue);
		}
	}
	#endregion
}
