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
using DevExpress.Office;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
using System.IO;
using System.ComponentModel.Design;
#endif
namespace DevExpress.Spreadsheet {
	public enum PicturePosition {
		Fill,
		Fit,
		Stretch,
		Tile,
		Center
	}
	#region Shape
	public interface Shape {
		int Id { get; }
		string Name { get; set; }
		int ZOrderPosition { get; set; }
		Cell TopLeftCell { get; set; }
		Cell BottomRightCell { get; set; }
		Placement Placement { get; set; }
		bool LockAspectRatio { get; set; }
		float OffsetX { get; }
		float OffsetY { get; }
		float Left { get; set; }
		float Top { get; set; }
		float Width { get; set; }
		float Height { get; set; }
		void Move(float rowOffset, float columnOffset);
		void Delete();
		void IncrementRotation(int degrees);
		int Rotation { get; set; }
		ShapeHyperlink Hyperlink { get; }
		ShapeHyperlink InsertHyperlink(string uri, bool isExternal);
		void RemoveHyperlink();
	}
	#endregion
	#region Picture
	public interface Picture : Shape {
		OfficeImageFormat ImageFormat { get; }
		OfficeImage Image { get; }
		string AlternativeText { get; set; }
		float OriginalHeight { get; }
		float OriginalWidth { get; }
		float BorderWidth { get; set; }
		Color BorderColor { get; set; }
		void RemoveOutline();
	}
	#endregion
	public enum Placement {
		MoveAndSize = DevExpress.XtraSpreadsheet.Model.AnchorType.TwoCell,
		Move = DevExpress.XtraSpreadsheet.Model.AnchorType.OneCell,
		FreeFloating = DevExpress.XtraSpreadsheet.Model.AnchorType.Absolute
	}
	#region DrawingHyperlink 
	public interface ShapeHyperlink : HyperlinkBase {
		void Remove();
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Utils;
	using DevExpress.Office.Utils;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelCell = DevExpress.XtraSpreadsheet.Model.ICell;
	using ModelCellKey = DevExpress.XtraSpreadsheet.Model.CellKey;
	using ModelCellPosition = DevExpress.XtraSpreadsheet.Model.CellPosition;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using ModelAnchorType = DevExpress.XtraSpreadsheet.Model.AnchorType;
	#region NativeDrawingObject
	abstract partial class NativeDrawingObject : NativeObjectBase, Shape {
		readonly Model.DrawingObject modelDrawingObject;
		readonly NativeWorksheet worksheet;
		ShapeHyperlink hyperlink;
		protected NativeDrawingObject(Model.DrawingObject modelDrawingObject, NativeWorksheet worksheet) {
			this.modelDrawingObject = modelDrawingObject;
			this.worksheet = worksheet;
			if (!String.IsNullOrEmpty(modelDrawingObject.Properties.HyperlinkClickUrl)) {
				hyperlink = new NativeShapeHyperlink(modelDrawingObject.Properties);
			}
		}
		protected NativeWorksheet Worksheet { get { return worksheet; } }
		protected internal NativeWorkbook NativeWorkbook { get { return worksheet.NativeWorkbook; } }
		public Model.DrawingObject ModelDrawingObject { get { return modelDrawingObject; } }
		public string Name { 
			get {
				CheckValid();
				return ModelDrawingObject.Properties.Name; 
			} 
			set {
				CheckValid();
				ModelDrawingObject.Properties.Name = value; 
			} 
		}
		public string AlternativeText {
			get {
				CheckValid();
				string description = ModelDrawingObject.Properties.Description;
				return String.IsNullOrEmpty(description) ? "" : description;
			}
			set {
				CheckValid();
				ModelDrawingObject.Properties.Description = value;
			}
		}
		public ShapeHyperlink Hyperlink {
			get {
				CheckValid();
				return hyperlink; 
			}
		}
		public Cell TopLeftCell {
			get {
				CheckValid();
				return worksheet.Cells[modelDrawingObject.From.Row, modelDrawingObject.From.Col];
			}
			set {
				CheckValid();
				modelDrawingObject.DocumentModel.BeginUpdate();
				try {
					ModelCellKey cellKey = new ModelCellKey(worksheet.ModelWorksheet.SheetId, value.ColumnIndex, value.RowIndex);
					modelDrawingObject.From = new Model.AnchorPoint(cellKey);
				}
				finally {
					modelDrawingObject.DocumentModel.EndUpdate();
				}
			}
		}
		public Cell BottomRightCell {
			get {
				CheckValid();
				int rowIndex = modelDrawingObject.To.Row;
				int colIndex = modelDrawingObject.To.Col;
				if(modelDrawingObject.To.RowOffset == 0 && rowIndex > 0)
					rowIndex--;
				if(modelDrawingObject.To.ColOffset == 0 && colIndex > 0)
					colIndex--;
				return worksheet.Cells[rowIndex, colIndex];
			}
			set {
				CheckValid();
				modelDrawingObject.DocumentModel.BeginUpdate();
				try {
					int rowIndex = value.RowIndex + 1;
					int colIndex = value.ColumnIndex + 1;
					ModelCellKey cellKey = new ModelCellKey(worksheet.ModelWorksheet.SheetId, colIndex, rowIndex);
					modelDrawingObject.To = new Model.AnchorPoint(cellKey);
				}
				finally {
					modelDrawingObject.DocumentModel.EndUpdate();
				}
			}
		}
		public float Width {
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.Width);
			}
			set {
				CheckValid();
				modelDrawingObject.Width = UnitsToModelUnitsF(value);
			}
		}
		public float Height {
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.Height);
			}
			set {
				CheckValid();
				modelDrawingObject.Height = UnitsToModelUnitsF(value);
			}
		}
		public Placement Placement {
			get {
				CheckValid();
				return (Placement)modelDrawingObject.GetResizingAnchorType();
			}
			set {
				CheckValid();
				switch(modelDrawingObject.AnchorType) {
					case Model.AnchorType.TwoCell:
						modelDrawingObject.ResizingBehavior = (Model.AnchorType) value;
						break;
					case Model.AnchorType.Absolute:
					case Model.AnchorType.OneCell:
						modelDrawingObject.AnchorType = (Model.AnchorType) value;
						break;
				}
			}
		}
		public float OffsetX { 
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.From.ColOffset); 
			} 
		}
		public float OffsetY { 
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.From.RowOffset); 
			} 
		}
		public float Left {
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.CoordinateX); 
			}
			set {
				CheckValid();
				modelDrawingObject.CoordinateX = UnitsToModelUnitsF(value); 
			}
		}
		public float Top {
			get {
				CheckValid();
				return ModelUnitsToUnitsF(modelDrawingObject.CoordinateY); 
			}
			set {
				CheckValid();
				modelDrawingObject.CoordinateY = UnitsToModelUnitsF(value); 
			}
		}
		public abstract int ZOrderPosition { get; set; }
		public int Id { 
			get {
				CheckValid();
				return modelDrawingObject.Properties.Id; 
			} 
		}
		public bool LockAspectRatio {
			get {
				CheckValid();
				return modelDrawingObject.Locks.NoChangeAspect; 
			}
			set {
				CheckValid();
				modelDrawingObject.Locks.NoChangeAspect = value;
			}
		}
		public abstract void Delete();
		public abstract void IncrementRotation(int degrees);
		public abstract int Rotation { get; set; }
		public void Move(float rowOffset, float columnOffset) {
			CheckValid();
			float modelRowOffset = UnitsToModelUnitsF(rowOffset);
			float modelColumnOffset = UnitsToModelUnitsF(columnOffset);
			modelDrawingObject.Move(modelRowOffset, modelColumnOffset);
		}
		public virtual void RemoveHyperlink() {
			CheckValid();
			hyperlink.Remove();
			hyperlink = null;
		}
		public virtual ShapeHyperlink InsertHyperlink(string uri, bool isExternal) {
			CheckValid();
			hyperlink = new NativeShapeHyperlink(modelDrawingObject.Properties);
			hyperlink.SetUri(uri, isExternal);
			return hyperlink;
		}
		protected float UnitsToModelUnitsF(float value) {
			return worksheet.NativeWorkbook.UnitsToModelUnitsF(value);
		}
		protected float ModelUnitsToUnitsF(float value) {
			return worksheet.NativeWorkbook.ModelUnitsToUnitsF(value);
		}
		protected int UnitsToModelUnits(float value) {
			return worksheet.NativeWorkbook.UnitsToModelUnits(value);
		}
		protected float ModelUnitsToUnits(int value) {
			return worksheet.NativeWorkbook.ModelUnitsToUnits(value);
		}
	}
	#endregion
	#region NativePicture
	partial class NativePicture : NativeDrawingObject, Picture {
		Model.Picture modelPicture;
		public NativePicture(Model.Picture modelPicture, NativeWorksheet worksheet)
			: base(modelPicture.DrawingObject, worksheet) {
			this.modelPicture = modelPicture;
		}
		public Model.Picture ModelPicture { get { return modelPicture; } }
		public OfficeImage Image { get { return ModelPicture.Image; } }
		DocumentModelUnitConverter UnitConverter { get { return Worksheet.NativeWorkbook.UnitConverter.Converter; } }
		public override int Rotation {
			get {
				CheckValid();
				return UnitConverter.ModelUnitsToDegree(ModelPicture.ShapeProperties.Transform2D.Rotation);
			}
			set {
				CheckValid();
				ModelPicture.RotateCore(UnitConverter.DegreeToModelUnits(value));
			}
		}
		public OfficeImageFormat ImageFormat {
			get {
				CheckValid();
				return ModelPicture.Image.RawFormat; 
			}
		}
		public float OriginalHeight { 
			get {
				CheckValid();
				return ModelUnitsToUnitsF(ModelPicture.OriginalHeight); 
			} 
		}
		public float OriginalWidth { 
			get {
				CheckValid();
				return ModelUnitsToUnitsF(ModelPicture.OriginalWidth); 
			} 
		}
		public override void IncrementRotation(int degrees) {
			CheckValid();
			ModelPicture.Rotate(UnitConverter.DegreeToModelUnits(degrees));
		}
		public float BorderWidth {
			get {
				CheckValid();
				return ModelUnitsToUnits(ModelPicture.ShapeProperties.Outline.Width); 
			}
			set {
				CheckValid();
				this.ModelDrawingObject.BeginUpdate();
				try {
					if (ModelPicture.ShapeProperties.OutlineType == Model.OutlineType.None) {
						ModelPicture.ShapeProperties.OutlineColor.OriginalColor.Rgb = Color.FromArgb(0, 0, 0, 0);
						ModelPicture.ShapeProperties.OutlineType = Model.OutlineType.Solid;
					}
					ModelPicture.ShapeProperties.Outline.Width = UnitsToModelUnits(value);
				}
				finally {
					this.ModelDrawingObject.EndUpdate();
				}
			}
		}
		public Color BorderColor {
			get {
				CheckValid();
				return ModelPicture.ShapeProperties.OutlineColor.FinalColor;
			} 
			set {
				CheckValid();
				this.ModelDrawingObject.BeginUpdate();
				try {
					ModelPicture.ShapeProperties.OutlineType = Model.OutlineType.Solid;
					ModelPicture.ShapeProperties.OutlineColor.OriginalColor.Rgb = value;
				}
				finally {
					this.ModelDrawingObject.EndUpdate();
				}
			} 
		}
		public void RemoveOutline() {
			CheckValid();
			ModelPicture.ShapeProperties.OutlineType = Model.OutlineType.None;
		}
		public override int ZOrderPosition {
			get {
				CheckValid();
				return ModelPicture.ZOrder;
			} 
			set {
				CheckValid();
				ModelPicture.ZOrder = value;
			}
		}
		public override void Delete() {
			CheckValid();
			int index = Worksheet.ModelWorksheet.DrawingObjects.IndexOf(this.ModelPicture);
			if(index >= 0)
				Worksheet.ModelWorksheet.RemoveDrawingAt(index);
		}
	}
	#endregion
	#region NativeShapeHyperlink
	partial class NativeShapeHyperlink : ShapeHyperlink {
		Model.IDrawingObjectNonVisualProperties properties;
		public NativeShapeHyperlink(Model.IDrawingObjectNonVisualProperties properties) {
			Guard.ArgumentNotNull(properties, "properties");
			this.properties = properties;
		}
		public string TooltipText {
			get { return properties.HyperlinkClickTooltip; }
			set { properties.HyperlinkClickTooltip = value; }
		}
		public string Uri { get { return properties.HyperlinkClickUrl; } }
		public bool IsExternal { get { return properties.HyperlinkClickIsExternal; } }
		public void SetUri(string uri, bool isExternal) {
			properties.HyperlinkClickUrl = uri;
			properties.HyperlinkClickIsExternal = isExternal;
		}
		public void Remove() {
			properties.RemoveHyperlink();
		}
	}
	#endregion
}
