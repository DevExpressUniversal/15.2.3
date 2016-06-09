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
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region RectangleOffsetInfo
	#endregion
	#region RectangleOffsetInfoCache
	#endregion
	#region PictureInfo
	public class PictureInfo : ICloneable<PictureInfo>, ISupportsCopyFrom<PictureInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskIsPublished = 0x00000001;				  
		const uint MaskPreferRelativeResize = 0x00000008;		 
		uint packedValues;
		string macro;
		#endregion
		#region Properties
		#region Picture properties
		public string Macro { get { return macro; } set { macro = value; } }
		public bool IsPublished { get { return GetBooleanVal(MaskIsPublished); } set { SetBooleanVal(MaskIsPublished, value); } }
		#endregion
		#region cNvPicPr (Non-Visual Picture Drawing Properties) §5.6.2.7
		public bool PreferRelativeResize { get { return GetBooleanVal(MaskPreferRelativeResize); } set { SetBooleanVal(MaskPreferRelativeResize, value); } }
		#endregion
		#endregion
		#region ICloneable<PictureInfo> Members
		public PictureInfo Clone() {
			PictureInfo result = new PictureInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
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
		#region ISupportsCopyFrom<PictureInfo> Members
		public void CopyFrom(PictureInfo value) {
			Guard.ArgumentNotNull(value, "PictureInfo");
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
			PictureInfo info = obj as PictureInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues
				   && this.macro == info.macro;
		}
		public override int GetHashCode() {
			return (int)(packedValues
				^ (macro == null ? 0 : macro.GetHashCode())
				);
		}
	}
	#endregion
	#region PictureInfoCache
	public class PictureInfoCache : UniqueItemsCache<PictureInfo> {
		public PictureInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override PictureInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			PictureInfo info = new PictureInfo();
			info.PreferRelativeResize = true;
			info.Macro = "";
			return info;
		}
	}
	#endregion
	#region Picture
	public class Picture : SpreadsheetUndoableIndexBasedObject<PictureInfo>, IDrawingObject, ICloneable<Picture>, ISupportsCopyFrom<Picture>, IPictureNonVisualProperties {
		#region Fields
		DrawingObject drawingObject;
		DrawingBlipFill pictureFill;
		ShapeProperties shapeProperties;
		ShapeStyle shapeStyle;
		int id;
		PictureLocks locks;
		#endregion
		public Picture(Worksheet worksheet)
			: base(worksheet) {
			drawingObject = new DrawingObject(worksheet);
			Initialize(worksheet);
		}
		public Picture(DrawingObject drawingObject)
			: base(drawingObject.Worksheet) {
			Guard.ArgumentNotNull(drawingObject, "DrawingObject");
			this.drawingObject = drawingObject;
			Initialize(drawingObject.Worksheet);
		}
		void Initialize(Worksheet worksheet) {
			pictureFill = new DrawingBlipFill(DocumentModel);
			shapeProperties = new ShapeProperties(DocumentModel);
			shapeStyle = new ShapeStyle(worksheet.Workbook);
			locks = new PictureLocks(DrawingObject.Locks);
		}
		#region Properties
		public DrawingObject DrawingObject { get { return drawingObject; } }
		public IPictureNonVisualProperties Properties { get { return this; } }
		public DrawingBlipFill PictureFill { get { return pictureFill; } }
		#region IsPublished
		public bool IsPublished {
			get { return Info.IsPublished; }
			set {
				if (IsPublished == value)
					return;
				SetPropertyValue(SetIsPublishedCore, value);
			}
		}
		DocumentModelChangeActions SetIsPublishedCore(PictureInfo info, bool value) {
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
		DocumentModelChangeActions SetMacroCore(PictureInfo info, string value) {
			info.Macro = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region cNvPicPr (Non-Visual Picture Drawing Properties) §5.6.2.7
		#region PreferRelativeResize
		bool IPictureNonVisualProperties.PreferRelativeResize {
			get { return Info.PreferRelativeResize; }
			set {
				if (Properties.PreferRelativeResize == value)
					return;
				SetPropertyValue(SetPreferRelativeResizeCore, value);
			}
		}
		DocumentModelChangeActions SetPreferRelativeResizeCore(PictureInfo info, bool value) {
			info.PreferRelativeResize = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		public int OriginalHeight { get { return Image.SizeInTwips.Height; } }
		public int OriginalWidth { get { return Image.SizeInTwips.Width; } }
		#endregion
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		public ShapeStyle ShapeStyle { get { return shapeStyle; } }
		public OfficeImage Image {
			get { return this.pictureFill.Blip.Image; }
			set { this.pictureFill.Blip.Image = value; }
		}
		public int IndexInCollection {
			get { return id; }
		}
		#region IDrawingObject members
		public Worksheet Worksheet { get { return drawingObject.Worksheet; } }
		public PictureLocks Locks { get { return locks; } }
		public IGraphicFrameInfo GraphicFrameInfo { get { return drawingObject.GraphicFrameInfo; } }
		public bool NoChangeAspect { get { return Locks.NoChangeAspect; } set { Locks.NoChangeAspect = value; } }
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
		public DrawingObjectType DrawingType { get { return DrawingObjectType.Picture; } }
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
			set {if (ZOrder == value)
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
			RotateCore(ShapeProperties.Transform2D.Rotation + value);
		}
		public void RotateCore(int newValue) {
			ShapeProperties.RotateCore(newValue);
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
		protected override UniqueItemsCache<PictureInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.PictureInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
		}
		#endregion
		public Picture Clone() {
			Picture result = new Picture(drawingObject);
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(Picture value) {
			Guard.ArgumentNotNull(value, "Picture");
			drawingObject.CopyFrom(value.drawingObject);
			PictureFill.CopyFrom(value.PictureFill);
			ShapeStyle.CopyFrom(ShapeStyle);
		}
		public void Copy(Picture source) {
			drawingObject.DrawingInfo.CopyFrom(source.drawingObject.DrawingInfo);
			PictureFill.CopyFrom(source.PictureFill);
			ShapeProperties.CopyFrom(source.ShapeProperties);
			if (drawingObject.AnchorType!=AnchorType.TwoCell) {
				drawingObject.SetIndependentHeight(source.Height);
				drawingObject.SetIndependentWidth(source.Width);				
			}
		}
		public void Visit(IDrawingObjectVisitor visitor) { visitor.Visit(this); }
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
			if (disposing) {
				if (this.pictureFill.Blip.Image != null) {
					this.pictureFill.Blip.Image.Dispose();
					this.pictureFill.Blip.SetImageCore(null);
				}
			}
		}
		#endregion
	}
	#endregion
	#region Tile
	#endregion
}
