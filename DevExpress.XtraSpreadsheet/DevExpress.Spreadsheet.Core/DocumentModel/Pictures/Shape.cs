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
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ModelShapeInfo
	public class ModelShapeInfo : ICloneable<ModelShapeInfo>, ISupportsCopyFrom<ModelShapeInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskIsPublished = 0x00000001; 
		const uint MaskTextBoxMode = 0x00000004; 
		uint packedValues;
		string macro;
		#endregion
		#region Properties
		#region Shape properties
		public string Macro { get { return macro; } set { macro = value; } }
		public bool IsPublished { get { return GetBooleanVal(MaskIsPublished); } set { SetBooleanVal(MaskIsPublished, value); } }
		public bool TextBoxMode { get { return GetBooleanVal(MaskTextBoxMode); } set { SetBooleanVal(MaskTextBoxMode, value); } }
		#endregion
		#endregion
		#region ICloneable<ModelShapeInfo> Members
		public ModelShapeInfo Clone() {
			ModelShapeInfo result = new ModelShapeInfo();
			result.CopyFrom(this);
			return result;
		}
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
		#region ISupportsCopyFrom<ModelShapeInfo> Members
		public void CopyFrom(ModelShapeInfo value) {
			Guard.ArgumentNotNull(value, "ModelShapeInfo");
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
			ModelShapeInfo info = obj as ModelShapeInfo;
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
	#region ModelShapeInfoCache
	public class ModelShapeInfoCache : UniqueItemsCache<ModelShapeInfo> {
		public ModelShapeInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) { }
		protected override ModelShapeInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ModelShapeInfo info = new ModelShapeInfo();
			info.Macro = string.Empty;
			info.TextBoxMode = false;
			return info;
		}
	}
	#endregion
	#region ModelShapeBase (abstract)
	public abstract class ModelShapeBase : SpreadsheetUndoableIndexBasedObject<ModelShapeInfo>, IDrawingObject, ISupportsCopyFrom<ModelShapeBase> {
		#region Fields
		readonly DrawingObject drawingObject;
		ShapeProperties shapeProperties;
		ShapeStyle shapeStyle;
		int id;
		#endregion
		protected ModelShapeBase(Worksheet worksheet)
			: base(worksheet) {
			this.drawingObject = new DrawingObject(worksheet);
			Initialize(worksheet);
		}
		protected ModelShapeBase(DrawingObject drawingObject)
			: base(drawingObject.Worksheet) {
			Guard.ArgumentNotNull(drawingObject, "DrawingObject");
			this.drawingObject = drawingObject;
			Initialize(drawingObject.Worksheet);
		}
		protected virtual void Initialize(Worksheet worksheet) {
			shapeProperties = new ShapeProperties(DocumentModel);
			shapeStyle = new ShapeStyle(worksheet.Workbook);
		}
		#region Properties
		public DrawingObject DrawingObject { get { return drawingObject; } }
		#region IsPublished
		public bool IsPublished {
			get { return Info.IsPublished; }
			set {
				if (IsPublished == value)
					return;
				SetPropertyValue(SetIsPublishedCore, value);
			}
		}
		DocumentModelChangeActions SetIsPublishedCore(ModelShapeInfo info, bool value) {
			info.IsPublished = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Macro
		public string Macro {
			get { return Info.Macro; }
			set {
				if (Macro == value)
					return;
				SetPropertyValue(SetMacroCore, value);
			}
		}
		DocumentModelChangeActions SetMacroCore(ModelShapeInfo info, string value) {
			info.Macro = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion        
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public ShapeStyle ShapeStyle { get { return shapeStyle; } }
		public int IndexInCollection { get { return id; } }
		#region IDrawingObject members
		public Worksheet Worksheet { get { return drawingObject.Worksheet; } }
		public abstract bool NoChangeAspect { get; set; }
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
		public abstract DrawingObjectType DrawingType { get; }
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
				if (ZOrder == value)
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
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ModelShapeInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ModelShapeInfoCache;
		}
		#endregion
		#region Rotate
		public void Rotate(int angle) {
			int value = angle % DrawingObject.MaxAngle;
			RotateCore(ShapeProperties.Transform2D.Rotation + value);
		}
		public void RotateCore(int newValue) {
			ShapeProperties.Transform2D.Rotation = newValue;
		}
		public int GetRotationAngleInModelUnits() {
			return ShapeProperties.Transform2D.Rotation;
		}
		public float GetRotationAngleInDegrees() {
			return ShapeProperties.Transform2D.GetRotationAngleInDegrees();
		}
		#endregion
		#region SwapHeightAndWidth
		public void CheckRotationAndSwapBox() {
			float angle0To180 = this.GetRotationAngleInDegrees() % 180;
			if (angle0To180 < 0)
				angle0To180 += 180;
			if (angle0To180 >= 45 && angle0To180 < 135)
				SwapHeightAndWidth();
		}
		void SwapHeightAndWidth() {
			drawingObject.SwapHeightAndWidth();
		}
		#endregion
		#region Enlarge / Reduce
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
		#endregion        
		public abstract void Visit(IDrawingObjectVisitor visitor);
		#region IDrawingObject Members
		public void OnRangeInserting(InsertRangeNotificationContext context) {
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) { } 
		}
		#endregion
		#region ISupportsCopyFrom<ModelShapeBase> Members
		public void CopyFrom(ModelShapeBase value) {
			base.CopyFrom(value);
			drawingObject.CopyFrom(value.DrawingObject);
			shapeStyle.CopyFrom(value.ShapeStyle);
			shapeProperties.CopyFrom(value.ShapeProperties);
		}
		#endregion
	}
	#endregion
	#region ModelShape
	public class ModelShape : ModelShapeBase, ICloneable<ModelShape>, ISupportsCopyFrom<ModelShape> {
		#region Fields
		ShapeLocks locks;
		TextProperties textProperties;
		#endregion
		public ModelShape(Worksheet worksheet)
			: base(worksheet) {
		}
		public ModelShape(DrawingObject drawingObject)
			: base(drawingObject) {
		}
		protected override void Initialize(Worksheet worksheet) {
			base.Initialize(worksheet);
			locks = new ShapeLocks(DrawingObject.Locks);
			textProperties = new TextProperties(DocumentModel);
		}
		#region Properties
		public TextProperties TextProperties { get { return textProperties; } }
		#region TextBoxMode
		public bool TextBoxMode {
			get { return Info.TextBoxMode; }
			set {
				if (value == TextBoxMode)
					return;
				SetPropertyValue(SetTextBoxModeCore, value);
			}
		}
		DocumentModelChangeActions SetTextBoxModeCore(ModelShapeInfo info, bool value) {
			info.TextBoxMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region IDrawingObject members
		public ShapeLocks Locks { get { return locks; } }
		public override bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public override DrawingObjectType DrawingType { get { return DrawingObjectType.Shape; } }
		#endregion
		public ModelShape Clone() {
			ModelShape result = new ModelShape(DrawingObject);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ModelShape value) {
			Guard.ArgumentNotNull(value, "ModelShape");
			base.CopyFrom(value);
			textProperties.CopyFrom(value.TextProperties);
		}
		public override void Visit(IDrawingObjectVisitor visitor) {
			visitor.Visit(this);
		}
		internal static Color GetPathFillColorCore(Color fillColor, PathFillMode fillMode) {
			switch (fillMode) {
				case PathFillMode.None:
				case PathFillMode.Norm:
					return fillColor;
				case PathFillMode.Lighten:
					return ColorHSL.CalculateColorRGB(fillColor, 0.4);
				case PathFillMode.LightenLess:
					return ColorHSL.CalculateColorRGB(fillColor, 0.2);
				case PathFillMode.Darken:
					return GetRealColorCore(0.6, fillColor);
				case PathFillMode.DarkenLess:
					return GetRealColorCore(0.8, fillColor);
				default:
					throw new ArgumentOutOfRangeException("fillMode", fillMode, null);
			}
		}
		internal static Color GetRealColorCore(double vmod, Color color) {
			double r = color.R / 255d;
			double g = color.G / 255d;
			double b = color.B / 255d;
			double max = Math.Max(r, Math.Max(g, b));
			double min = Math.Min(r, Math.Min(g, b));
			double h;
			if (max == min) {
				h = 0;
			}
			else if (max == r && g >= b) {
				h = 60 * (g - b) / (max - min);
			}
			else if (max == r && g < b) {
				h = 60 * (g - b) / (max - min) + 360;
			}
			else if (max == g) {
				h = 60 * (b - r) / (max - min) + 120;
			}
			else {
				h = 60 * (r - g) / (max - min) + 240;
			}
			double s = max == 0 ? 0 : 1 - min / max;
			double v = max;
			v = Math.Min(1, v * vmod);
			int hi = (int)(h / 60) % 6;
			double vmin = (1 - s) * v;
			double a = (v - vmin) * (h % 60) / 60;
			double vinc = vmin + a;
			double vdec = v - a;
			double newR;
			double newG;
			double newB;
			if (hi == 0) {
				newR = v;
				newG = vinc;
				newB = vmin;
			}
			else if (hi == 1) {
				newR = vdec;
				newG = v;
				newB = vmin;
			}
			else if (hi == 2) {
				newR = vmin;
				newG = v;
				newB = vinc;
			}
			else if (hi == 3) {
				newR = vmin;
				newG = vdec;
				newB = v;
			}
			else if (hi == 4) {
				newR = vinc;
				newG = vmin;
				newB = v;
			}
			else {
				newR = v;
				newG = vmin;
				newB = vdec;
			}
			return Color.FromArgb((int)(newR * 255), (int)(newG * 255), (int)(newB * 255));
		}
	}
	#endregion
	#region ConnectionShape
	public class ConnectionShape : ModelShapeBase, ICloneable<ConnectionShape>, ISupportsCopyFrom<ConnectionShape> {
		#region Fields
		const int idxStartId = 0;
		const int idxStartSite = 1;
		const int idxEndId = 2;
		const int idxEndSite = 3;
		ConnectionShapeLocks locks;
		int[] siteProperties = new int[4];
		#endregion
		public ConnectionShape(Worksheet worksheet) 
			: base(worksheet) {
		}
		public ConnectionShape(DrawingObject drawingObject)
			: base(drawingObject) {
		}
		protected override void Initialize(Worksheet worksheet) {
			base.Initialize(worksheet);
			locks = new ConnectionShapeLocks(DrawingObject.Locks);
		}
		public int StartConnectionId { 
			get { return siteProperties[idxStartId]; } 
			set {
				int oldValue = StartConnectionId;
				if (oldValue == value)
					return;
				ConnectionShapeSiteHistoryItem historyItem = new ConnectionShapeSiteHistoryItem(this, idxStartId, value, oldValue);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			} 
		}
		public int StartConnectionIdx {
			get { return siteProperties[idxStartSite]; }
			set {
				int oldValue = StartConnectionIdx;
				if (oldValue == value)
					return;
				ConnectionShapeSiteHistoryItem historyItem = new ConnectionShapeSiteHistoryItem(this, idxStartSite, value, oldValue);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public int EndConnectionId {
			get { return siteProperties[idxEndId]; }
			set {
				int oldValue = EndConnectionId;
				if (oldValue == value)
					return;
				ConnectionShapeSiteHistoryItem historyItem = new ConnectionShapeSiteHistoryItem(this, idxEndId, value, oldValue);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		public int EndConnectionIdx {
			get { return siteProperties[idxEndSite]; }
			set {
				int oldValue = EndConnectionIdx;
				if (oldValue == value)
					return;
				ConnectionShapeSiteHistoryItem historyItem = new ConnectionShapeSiteHistoryItem(this, idxEndSite, value, oldValue);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetSiteProperty(int valueIndex, int value) {
			this.siteProperties[valueIndex] = value;
		}
		#region IDrawingObject members
		public ConnectionShapeLocks Locks { get { return locks; } }
		public override bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
		public override DrawingObjectType DrawingType { get { return DrawingObjectType.ConnectionShape; } }
		public override void Visit(IDrawingObjectVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region ICloneable<ConnectionShape> Members
		public ConnectionShape Clone() {
			ConnectionShape result = new ConnectionShape(DrawingObject);
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ConnectionShape> Members
		public void CopyFrom(ConnectionShape value) {
			Guard.ArgumentNotNull(value, "ConnectionShape");
			base.CopyFrom(value);
			Array.Copy(value.siteProperties, this.siteProperties, value.siteProperties.Length);
		}
		#endregion
	}
	#endregion
}
