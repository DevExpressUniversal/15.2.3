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
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region Group Classes
	#region GroupShapeInfo
	public class GroupShapeInfo : ICloneable<GroupShapeInfo>, ISupportsCopyFrom<GroupShapeInfo>, ISupportsSizeOf {
		#region Fields
		uint packedValues;
		string macro;
		#endregion
		#region Properties
		#endregion
		#region ICloneable<ModelShapeInfo> Members
		public GroupShapeInfo Clone() {
			GroupShapeInfo result = new GroupShapeInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeInfo> Members
		public void CopyFrom(GroupShapeInfo value) {
			Guard.ArgumentNotNull(value, "GroupShapeInfo");
			this.packedValues = value.packedValues;
			this.macro = value.macro;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			GroupShapeInfo info = obj as GroupShapeInfo;
			if(info == null)
				return false;
			return this.packedValues == info.packedValues
				   && this.macro == info.macro;
		}
		public override int GetHashCode() {
			return (int)(packedValues ^ (macro == null ? 0 : macro.GetHashCode()));
		}
	}
	#endregion
	#region GroupShapeInfoCache
	public class GroupShapeInfoCache : UniqueItemsCache<GroupShapeInfo> {
		public GroupShapeInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) { }
		protected override GroupShapeInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			GroupShapeInfo info = new GroupShapeInfo();
			return info;
		}
	}
	#endregion
	#region GroupShape
	public class GroupShape : SpreadsheetUndoableIndexBasedObject<GroupShapeInfo>, IDrawingObject, ISupportsInvalidate, IDrawingObjectsContainer, IDrawingObjectByZOrderSorter {
		#region Fields
		readonly DrawingObject drawingObject;
		readonly GroupShapeProperties groupShapeProperties;
		readonly GroupShapeLocks locks;
		readonly DrawingObjectsCollection drawingObjects;
		readonly DrawingObjectsByZOrderCollections drawingObjectsByZOrderCollection;
		int id;
		bool invalidated;
		#endregion
		public GroupShape(Worksheet worksheet)
			: base(worksheet) {
			drawingObject = new DrawingObject(worksheet);
			groupShapeProperties = new GroupShapeProperties(DocumentModel) { Parent = this };
			locks = new GroupShapeLocks(DrawingObject.Locks);
			drawingObjects = new DrawingObjectsCollection(this);
			drawingObjectsByZOrderCollection = new DrawingObjectsByZOrderCollections(this);
		}
		public GroupShape(DrawingObject drawingObject)
			: base(drawingObject.Worksheet) {
			this.drawingObject = drawingObject;
			groupShapeProperties = new GroupShapeProperties(DocumentModel) { Parent = this };
			locks = new GroupShapeLocks(DrawingObject.Locks);
			drawingObjects = new DrawingObjectsCollection(this);
			drawingObjectsByZOrderCollection = new DrawingObjectsByZOrderCollections(this);
		}
		#region Properties
		public DrawingObjectsByZOrderCollections DrawingObjectsByZOrderCollections { get { return drawingObjectsByZOrderCollection; } }
		public DrawingObjectsCollection DrawingObjects { get { return drawingObjects; } }
		public DrawingObject DrawingObject { get { return drawingObject; } }
		#endregion
		public GroupShapeProperties GroupShapeProperties { get { return groupShapeProperties; } }
		public int IndexInCollection { get { return id; } }
		#region IDrawingObject members
		public Worksheet Worksheet { get { return drawingObject.Worksheet; } }
		public GroupShapeLocks Locks { get { return locks; } }
		public bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public IGraphicFrameInfo GraphicFrameInfo { get { return drawingObject.GraphicFrameInfo; } }
		public bool LocksWithSheet {
			get { return drawingObject.LocksWithSheet; }
			set { drawingObject.LocksWithSheet = value; }
		}
		public bool PrintsWithSheet {
			get { return drawingObject.PrintsWithSheet; }
			set { drawingObject.PrintsWithSheet = value; }
		}
		public AnchorType AnchorType {
			get { return drawingObject.AnchorType; }
			set { drawingObject.AnchorType = value; }
		}
		public AnchorType ResizingBehavior {
			get { return drawingObject.ResizingBehavior; }
			set { drawingObject.ResizingBehavior = value; }
		}
		public AnchorPoint From {
			get { return drawingObject.From; }
			set { drawingObject.From = value; }
		}
		public AnchorPoint To {
			get { return drawingObject.To; }
			set { drawingObject.To = value; }
		}
		public DrawingObjectType DrawingType { get { return DrawingObjectType.GroupShape; } }
		public bool CanRotate { get { return true; } }
		public float Height {
			get { return drawingObject.Height; }
			set { drawingObject.Height = value; }
		}
		public float Width {
			get { return drawingObject.Width; }
			set { drawingObject.Width = value; }
		}
		public float CoordinateX {
			get { return drawingObject.CoordinateX; }
			set { drawingObject.CoordinateX = value; }
		}
		public float CoordinateY {
			get { return drawingObject.CoordinateY; }
			set { drawingObject.CoordinateY = value; }
		}
		public int ZOrder {
			get { return Worksheet.DrawingObjectsByZOrderCollections.GetZOrder(this); }
			set {
				if(ZOrder == value)
					return;
				Worksheet.Workbook.BeginUpdate();
				DocumentHistory history = DocumentModel.History;
				SetZOrderHistoryItem historyItem = new SetZOrderHistoryItem(this, value, ZOrder);
				history.Add(historyItem);
				historyItem.Execute();
				Worksheet.Workbook.EndUpdate();
			}
		}
		public void Move(float offsetY, float offsetX) {
			drawingObject.Move(offsetY, offsetX);
		}
		public void SetIndependentWidth(float width) {
			drawingObject.SetIndependentWidth(width);
		}
		public void SetIndependentHeight(float height) {
			drawingObject.SetIndependentHeight(height);
		}
		public void CoordinatesFromCellKey(CellKey cellKey) {
			drawingObject.CoordinatesFromCellKey(cellKey);
		}
		public void SizeFromCellKey(CellKey cellKey) {
			drawingObject.SizeFromCellKey(cellKey);
		}
		public AnchorData AnchorData { get { return drawingObject.AnchorData; } }
		#endregion
		public void SetIndexInCollection(int value) {
			id = value;
		}
		#region Rotate
		public void Rotate(int angle) {
			int value = angle % DrawingObject.MaxAngle;
			RotateCore(GroupShapeProperties.Transform2D.MainTransform.Rotation + value);
		}
		public void RotateCore(int newValue) {
			GroupShapeProperties.Transform2D.MainTransform.Rotation = newValue;
		}
		public int GetRotationAngleInModelUnits() {
			return GroupShapeProperties.Transform2D.MainTransform.Rotation;
		}
		public float GetRotationAngleInDegrees() {
			return GroupShapeProperties.Transform2D.MainTransform.GetRotationAngleInDegrees();
		}
		#endregion
		#region SwapHeightAndWidth
		public void CheckRotationAndSwapBox() {
			float angle0To180 = this.GetRotationAngleInDegrees() % 180;
			if(angle0To180 < 0)
				angle0To180 += 180;
			if(angle0To180 >= 45 && angle0To180 < 135)
				SwapHeightAndWidth();
		}
		void SwapHeightAndWidth() {
			drawingObject.SwapHeightAndWidth();
		}
		#endregion
		public void EnlargeWidth(float scale) {
			drawingObject.EnlargeWidth(scale);
		}
		public void EnlargeHeight(float scale) {
			drawingObject.EnlargeHeight(scale);
		}
		public void ReduceWidth(float scale) {
			drawingObject.ReduceWidth(scale);
		}
		public void ReduceHeight(float scale) {
			drawingObject.ReduceHeight(scale);
		}
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<GroupShapeInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.GroupShapeInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			this.Invalidate();
		}
		protected override void OnLastEndUpdateCore() {
			base.OnLastEndUpdateCore();
			if(invalidated) {
				invalidated = false;
				Redraw();
			}
		}
		#endregion
		public void Visit(IDrawingObjectVisitor visitor) {
			visitor.Visit(this);
		}
		#region IDrawingObject Members
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			drawingObjects.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			drawingObjects.OnRangeRemoving(context);
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				drawingObjects.Dispose();
			} 
		}
		#endregion
		#region Implementation of ISupportsInvalidate
		public void Invalidate() {
			if(IsUpdateLocked)
				invalidated = true;
			else {
				Redraw();
			}
		}
		#endregion
		void Redraw() {
		}
	}
	#endregion
	#endregion
}
