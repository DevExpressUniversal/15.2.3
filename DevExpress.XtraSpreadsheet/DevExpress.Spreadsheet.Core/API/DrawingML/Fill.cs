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

using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.Spreadsheet.Drawings {
	#region ShapeFillType
	public enum ShapeFillType {
		Auto = DevExpress.Office.Drawing.DrawingFillType.Automatic,
		None = DevExpress.Office.Drawing.DrawingFillType.None,
		Solid = DevExpress.Office.Drawing.DrawingFillType.Solid,
		Pattern = DevExpress.Office.Drawing.DrawingFillType.Pattern,
		Gradient = DevExpress.Office.Drawing.DrawingFillType.Gradient,
		Picture = DevExpress.Office.Drawing.DrawingFillType.Picture
	}
	#endregion
	#region ShapeFillPatternType
	public enum ShapeFillPatternType {
		Cross = DevExpress.Office.Drawing.DrawingPatternType.Cross,
		DashedDownwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.DashedDownwardDiagonal,
		DashedHorizontal = DevExpress.Office.Drawing.DrawingPatternType.DashedHorizontal,
		DashedUpwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.DashedUpwardDiagonal,
		DashedVertical = DevExpress.Office.Drawing.DrawingPatternType.DashedVertical,
		DiagonalBrick = DevExpress.Office.Drawing.DrawingPatternType.DiagonalBrick,
		DiagonalCross = DevExpress.Office.Drawing.DrawingPatternType.DiagonalCross,
		Divot = DevExpress.Office.Drawing.DrawingPatternType.Divot,
		DarkDownwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.DarkDownwardDiagonal,
		DarkHorizontal = DevExpress.Office.Drawing.DrawingPatternType.DarkHorizontal,
		DarkUpwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.DarkUpwardDiagonal,
		DarkVertical = DevExpress.Office.Drawing.DrawingPatternType.DarkVertical,
		DownwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.DownwardDiagonal,
		DottedDiamond = DevExpress.Office.Drawing.DrawingPatternType.DottedDiamond,
		DottedGrid = DevExpress.Office.Drawing.DrawingPatternType.DottedGrid,
		Horizontal = DevExpress.Office.Drawing.DrawingPatternType.Horizontal,
		HorizontalBrick = DevExpress.Office.Drawing.DrawingPatternType.HorizontalBrick,
		LargeCheckerBoard = DevExpress.Office.Drawing.DrawingPatternType.LargeCheckerBoard,
		LargeConfetti = DevExpress.Office.Drawing.DrawingPatternType.LargeConfetti,
		LargeGrid = DevExpress.Office.Drawing.DrawingPatternType.LargeGrid,
		LightDownwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.LightDownwardDiagonal,
		LightHorizontal = DevExpress.Office.Drawing.DrawingPatternType.LightHorizontal,
		LightUpwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.LightUpwardDiagonal,
		LightVertical = DevExpress.Office.Drawing.DrawingPatternType.LightVertical,
		NarrowHorizontal = DevExpress.Office.Drawing.DrawingPatternType.NarrowHorizontal,
		NarrowVertical = DevExpress.Office.Drawing.DrawingPatternType.NarrowVertical,
		OpenDiamond = DevExpress.Office.Drawing.DrawingPatternType.OpenDiamond,
		Percent10 = DevExpress.Office.Drawing.DrawingPatternType.Percent10,
		Percent20 = DevExpress.Office.Drawing.DrawingPatternType.Percent20,
		Percent25 = DevExpress.Office.Drawing.DrawingPatternType.Percent25,
		Percent30 = DevExpress.Office.Drawing.DrawingPatternType.Percent30,
		Percent40 = DevExpress.Office.Drawing.DrawingPatternType.Percent40,
		Percent5 = DevExpress.Office.Drawing.DrawingPatternType.Percent5,
		Percent50 = DevExpress.Office.Drawing.DrawingPatternType.Percent50,
		Percent60 = DevExpress.Office.Drawing.DrawingPatternType.Percent60,
		Percent70 = DevExpress.Office.Drawing.DrawingPatternType.Percent70,
		Percent75 = DevExpress.Office.Drawing.DrawingPatternType.Percent75,
		Percent80 = DevExpress.Office.Drawing.DrawingPatternType.Percent80,
		Percent90 = DevExpress.Office.Drawing.DrawingPatternType.Percent90,
		Plaid = DevExpress.Office.Drawing.DrawingPatternType.Plaid,
		Shingle = DevExpress.Office.Drawing.DrawingPatternType.Shingle,
		SmallCheckerBoard = DevExpress.Office.Drawing.DrawingPatternType.SmallCheckerBoard,
		SmallConfetti = DevExpress.Office.Drawing.DrawingPatternType.SmallConfetti,
		SmallGrid = DevExpress.Office.Drawing.DrawingPatternType.SmallGrid,
		SolidDiamond = DevExpress.Office.Drawing.DrawingPatternType.SolidDiamond,
		Sphere = DevExpress.Office.Drawing.DrawingPatternType.Sphere,
		Trellis = DevExpress.Office.Drawing.DrawingPatternType.Trellis,
		UpwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.UpwardDiagonal,
		Vertical = DevExpress.Office.Drawing.DrawingPatternType.Vertical,
		Wave = DevExpress.Office.Drawing.DrawingPatternType.Wave,
		WideDownwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.WideDownwardDiagonal,
		WideUpwardDiagonal = DevExpress.Office.Drawing.DrawingPatternType.WideUpwardDiagonal,
		Weave = DevExpress.Office.Drawing.DrawingPatternType.Weave,
		ZigZag = DevExpress.Office.Drawing.DrawingPatternType.ZigZag
	}
	#endregion
	#region ShapeGradientType
	public enum ShapeGradientType {
		Linear = DevExpress.Office.Drawing.GradientType.Linear,
		Rectangle = DevExpress.Office.Drawing.GradientType.Rectangle,
		Circle = DevExpress.Office.Drawing.GradientType.Circle,
		Shape = DevExpress.Office.Drawing.GradientType.Shape
	}
	#endregion
	#region TileAlignType
	public enum TileAlignType {
		TopLeft = DevExpress.Office.Drawing.RectangleAlignType.TopLeft,
		Top = DevExpress.Office.Drawing.RectangleAlignType.Top,
		TopRight = DevExpress.Office.Drawing.RectangleAlignType.TopRight,
		Left = DevExpress.Office.Drawing.RectangleAlignType.TopLeft,
		Center = DevExpress.Office.Drawing.RectangleAlignType.Center,
		Right = DevExpress.Office.Drawing.RectangleAlignType.Right,
		BottomLeft = DevExpress.Office.Drawing.RectangleAlignType.BottomLeft,
		Bottom = DevExpress.Office.Drawing.RectangleAlignType.Bottom,
		BottomRight = DevExpress.Office.Drawing.RectangleAlignType.BottomRight
	}
	#endregion
	#region ShapeSolidFill
	public interface ShapeSolidFill {
		Color Color { get; set; }
	}
	#endregion
	#region ShapePatternFill
	public interface ShapePatternFill {
		Color ForegroundColor { get; set; }
		Color BackgroundColor { get; set; }
		ShapeFillPatternType PatternType { get; set; }
	}
	#endregion
	#region ShapeRectangleOffset
	public interface ShapeRectangleOffset {
		double Left { get; set; }
		double Top { get; set; }
		double Right { get; set; }
		double Bottom { get; set; }
	}
	#endregion
	#region ShapeComplexFill
	public interface ShapeComplexFill {
		bool RotateWithShape { get; set; }
		TileFlipType FlipType { get; set; }
		ShapeRectangleOffset FillRect { get; }
	}
	#endregion
	#region ShapeGradientFill
	public interface ShapeGradientFill : ShapeComplexFill {
		ShapeGradientType GradientType { get; }
		double Angle { get; set; }
		bool Scaled { get; set; }
		ShapeRectangleOffset TileRect { get; }
		GradientStopCollection Stops { get; }
	}
	#endregion
	#region ShapePictureFill
	public interface ShapePictureFill : ShapeComplexFill {
		OfficeImage Image { get; }
		bool Stretch { get; set; }
		TileAlignType AlignType { get; set; }
		ShapeRectangleOffset SourceRect { get; }
		double ScaleX { get; set; }
		double ScaleY { get; set; }
		double OffsetX { get; set; }
		double OffsetY { get; set; }
	}
	#endregion 
	#region ShapeOutlineFill
	public interface ShapeOutlineFill {
		ShapeFillType FillType { get; }
		ShapeSolidFill SolidFill { get; }
		ShapeGradientFill GradientFill { get; }
		void SetAutoFill();
		void SetNoFill();
		void SetSolidFill(Color color);
		void SetGradientFill(ShapeGradientType gradientType, IList<GradientStopInfo> stopInfoes);
		void SetGradientFill(ShapeGradientType gradientType, Color firstColor, Color lastColor);
	}
	#endregion
	#region GradientStopInfo (struct)
	public struct GradientStopInfo {
		#region Fields
		Color color;
		double position;
		#endregion
		#region Properties
		public Color Color { get { return color; } set { color = value; } }
		public double Position { get { return position; } set { position = value; } }
		#endregion 
	}
	#endregion
	#region ShapeFill
	public interface ShapeFill : ShapeOutlineFill {
		ShapePatternFill PatternFill { get; }
		ShapePictureFill PictureFill { get; }
		void SetPatternFill(Color foregroundColor, Color backgroundColor, ShapeFillPatternType patternType);
#if !SL
		void SetPictureFill(Image image);
		void SetPictureFill(string filename);
#endif
		void SetPictureFill(SpreadsheetImageSource imageSource);
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Office;
	using DevExpress.Office.API.Internal;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Drawings;
	using DevExpress.Office.Drawing;
	using DevExpress.Office.Model;
	#region NativeShapeSolidFill
	partial class NativeShapeSolidFill : NativeObjectBase, ShapeSolidFill {
		readonly DrawingSolidFill modelFill;
		public NativeShapeSolidFill(DrawingSolidFill modelFill) {
			this.modelFill = modelFill;
		}
		#region ShapeSolidFill Members
		public Color Color {
			get {
				CheckValid();
				return modelFill.Color.FinalColor;
			}
			set {
				CheckValid();
				modelFill.Color.OriginalColor.SetColorFromRGB(value);
			}
		}
		#endregion
	}
	#endregion
	#region NativeShapePatternFill
	partial class NativeShapePatternFill : NativeObjectBase, ShapePatternFill {
		readonly DrawingPatternFill modelFill;
		public NativeShapePatternFill(DrawingPatternFill modelFill) {
			this.modelFill = modelFill;
		}
		#region ShapePatternFill Members
		public Color ForegroundColor {
			get {
				CheckValid();
				return modelFill.ForegroundColor.FinalColor;
			}
			set {
				CheckValid();
				modelFill.ForegroundColor.OriginalColor.SetColorFromRGB(value);
			}
		}
		public Color BackgroundColor {
			get {
				CheckValid();
				return modelFill.BackgroundColor.FinalColor;
			}
			set {
				CheckValid();
				modelFill.BackgroundColor.OriginalColor.SetColorFromRGB(value);
			}
		}
		public ShapeFillPatternType PatternType {
			get {
				CheckValid();
				return (ShapeFillPatternType)modelFill.PatternType;
			}
			set {
				CheckValid();
				modelFill.PatternType = (DrawingPatternType)value;
			}
		}
		#endregion
	}
	#endregion
	#region NativeShapeRectangleOffsetBase (abstract class)
	abstract partial class NativeShapeRectangleOffsetBase : NativeObjectBase, ShapeRectangleOffset {
		protected abstract RectangleOffset ModelRectangleOffset { get; set; }
		#region ShapeRectangleOffset Members
		public double Left {
			get {
				CheckValid();
				return ConvertFromPercentage(ModelRectangleOffset.LeftOffset);
			}
			set {
				CheckValid();
				int intValue = ConvertToPercentage(value);
				if (ModelRectangleOffset.LeftOffset != intValue)
					ModelRectangleOffset = new RectangleOffset(ModelRectangleOffset.BottomOffset, intValue, ModelRectangleOffset.RightOffset, ModelRectangleOffset.TopOffset);
			}
		}
		public double Top {
			get {
				CheckValid();
				return ConvertFromPercentage(ModelRectangleOffset.TopOffset);
			}
			set {
				CheckValid();
				int intValue = ConvertToPercentage(value);
				if (ModelRectangleOffset.TopOffset != intValue)
					ModelRectangleOffset = new RectangleOffset(ModelRectangleOffset.BottomOffset, ModelRectangleOffset.LeftOffset, ModelRectangleOffset.RightOffset, intValue);
			}
		}
		public double Right {
			get {
				CheckValid();
				return ConvertFromPercentage(ModelRectangleOffset.RightOffset);
			}
			set {
				CheckValid();
				int intValue = ConvertToPercentage(value);
				if (ModelRectangleOffset.RightOffset != intValue)
					ModelRectangleOffset = new RectangleOffset(ModelRectangleOffset.BottomOffset, ModelRectangleOffset.LeftOffset, intValue, ModelRectangleOffset.TopOffset);
			}
		}
		public double Bottom {
			get {
				CheckValid();
				return ConvertFromPercentage(ModelRectangleOffset.BottomOffset);
			}
			set {
				CheckValid();
				int intValue = ConvertToPercentage(value);
				if (ModelRectangleOffset.BottomOffset != intValue)
					ModelRectangleOffset = new RectangleOffset(intValue, ModelRectangleOffset.LeftOffset, ModelRectangleOffset.RightOffset, ModelRectangleOffset.TopOffset);
			}
		}
		#endregion
		int ConvertToPercentage(double value) {
			return DrawingValueConverter.ToPercentage(value);
		}
		double ConvertFromPercentage(int value) {
			return DrawingValueConverter.FromPercentage(value);
		}
	}
	#endregion
	#region NativeShapeTileRectangleOffset
	partial class NativeShapeTileRectangleOffset : NativeShapeRectangleOffsetBase {
		DrawingGradientFill modelFill;
		public NativeShapeTileRectangleOffset(DrawingGradientFill modelFill) {
			this.modelFill = modelFill;
		}
		protected override RectangleOffset ModelRectangleOffset {
			get { return modelFill.TileRect; }
			set { modelFill.TileRect = value; }
		}
	}
	#endregion
	#region NativeShapeGradientFillRectangleOffset
	partial class NativeShapeGradientFillRectangleOffset : NativeShapeRectangleOffsetBase {
		DrawingGradientFill modelFill;
		public NativeShapeGradientFillRectangleOffset(DrawingGradientFill modelFill) {
			this.modelFill = modelFill;
		}
		protected override RectangleOffset ModelRectangleOffset {
			get { return modelFill.FillRect; }
			set { modelFill.FillRect = value; }
		}
	}
	#endregion
	#region NativeShapeBlipFillRectangleOffset
	partial class NativeShapeBlipFillRectangleOffset : NativeShapeRectangleOffsetBase {
		DrawingBlipFill modelFill;
		public NativeShapeBlipFillRectangleOffset(DrawingBlipFill modelFill) {
			this.modelFill = modelFill;
		}
		protected override RectangleOffset ModelRectangleOffset {
			get { return modelFill.FillRectangle; }
			set { modelFill.FillRectangle = value; }
		}
	}
	#endregion
	#region NativeShapeBlipFillSourceRectangleOffset
	partial class NativeShapeBlipFillSourceRectangleOffset : NativeShapeRectangleOffsetBase {
		DrawingBlipFill modelFill;
		public NativeShapeBlipFillSourceRectangleOffset(DrawingBlipFill modelFill) {
			this.modelFill = modelFill;
		}
		protected override RectangleOffset ModelRectangleOffset {
			get { return modelFill.SourceRectangle; }
			set { modelFill.SourceRectangle = value; }
		}
	}
	#endregion
	#region NativeShapeGradientFill
	partial class NativeShapeGradientFill : NativeObjectBase, ShapeGradientFill {
		#region Fields
		readonly DrawingGradientFill modelFill;
		readonly NativeDrawingGradientStopCollection gradientStops;
		NativeShapeTileRectangleOffset tileRectangleOffset;
		NativeShapeGradientFillRectangleOffset fillRectangleOffset;
		#endregion
		public NativeShapeGradientFill(DrawingGradientFill modelFill) {
			this.modelFill = modelFill;
			this.gradientStops = new NativeDrawingGradientStopCollection(modelFill.GradientStops);
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			gradientStops.IsValid = value;
			if (tileRectangleOffset != null)
				tileRectangleOffset.IsValid = value;
			if (fillRectangleOffset != null)
				fillRectangleOffset.IsValid = value;
		}
		#region ShapeGradientFill Members
		public ShapeGradientType GradientType {
			get {
				CheckValid();
				return (ShapeGradientType)modelFill.GradientType;
			}
		}
		public double Angle {
			get {
				CheckValid();
				return DrawingValueConverter.FromPositiveFixedAngle(modelFill.Angle);
			}
			set {
				CheckValid();
				modelFill.Angle = DrawingValueConverter.ToPositiveFixedAngle(value);
			}
		}
		public bool Scaled {
			get {
				CheckValid();
				return modelFill.Scaled;
			}
			set {
				CheckValid();
				modelFill.Scaled = value;
			}
		}
		public ShapeRectangleOffset TileRect {
			get {
				CheckValid();
				if (tileRectangleOffset == null)
					tileRectangleOffset = new NativeShapeTileRectangleOffset(modelFill);
				return tileRectangleOffset;
			}
		}
		public Spreadsheet.GradientStopCollection Stops {
			get {
				CheckValid();
				return gradientStops;
			}
		}
		public bool RotateWithShape {
			get {
				CheckValid();
				return modelFill.RotateWithShape;
			}
			set {
				CheckValid();
				modelFill.RotateWithShape = value;
			}
		}
		public DevExpress.Office.Drawing.TileFlipType FlipType {
			get {
				CheckValid();
				return (TileFlipType)modelFill.Flip;
			}
			set {
				CheckValid();
				modelFill.Flip = (DevExpress.Office.Drawing.TileFlipType)value;
			}
		}
		public ShapeRectangleOffset FillRect {
			get {
				CheckValid();
				if (fillRectangleOffset == null)
					fillRectangleOffset = new NativeShapeGradientFillRectangleOffset(modelFill);
				return fillRectangleOffset;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDrawingGradientStop
	partial class NativeDrawingGradientStop : NativeObjectBase, GradientStop {
		DevExpress.Office.Drawing.DrawingGradientStop modelStop;
		public NativeDrawingGradientStop(DevExpress.Office.Drawing.DrawingGradientStop modelStop) {
			this.modelStop = modelStop;
		}
		#region GradientStop Members
		public double Position {
			get {
				CheckValid();
				return DrawingValueConverter.FromPercentage(modelStop.Position);
			}
		}
		public Color Color {
			get {
				CheckValid();
				return modelStop.Color.FinalColor;
			}
		}
		#endregion
	}
	#endregion
	#region NativeDrawingGradientStopCollection
	partial class NativeDrawingGradientStopCollection : NativeDrawingCollectionBase<GradientStop, NativeDrawingGradientStop, DevExpress.Office.Drawing.DrawingGradientStop>, Spreadsheet.GradientStopCollection {
		public NativeDrawingGradientStopCollection(DevExpress.Office.Drawing.GradientStopCollection modelCollection)
			: base(modelCollection) {
		}
		protected override NativeDrawingGradientStop CreateNativeObject(DevExpress.Office.Drawing.DrawingGradientStop modelStop) {
			return new NativeDrawingGradientStop(modelStop);
		}
		IDocumentModel DocumentModel { get { return ModelCollection.DocumentModel; } }
		DevExpress.Office.Drawing.DrawingGradientStop CreateModelStop(double position, Color color) {
			DrawingGradientStop result = new DrawingGradientStop(DocumentModel);
			result.Color.OriginalColor.SetColorFromRGB(color);
			result.Position = DrawingValueConverter.ToPercentage(position);
			return result;
		}
		#region Spreadsheet.GradientStopCollection Members
		public void Add(double position, Color color) {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				AddModelItem(CreateModelStop(position, color));
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
	}
	#endregion
	#region NativeShapeOutlineFill
	abstract partial class NativeShapeOutlineFill<TModelFillOwner> : NativeObjectBase, ShapeOutlineFill where TModelFillOwner : IFillOwner {
		#region Fields
		readonly TModelFillOwner modelFillOwner;
		readonly NativeWorkbook nativeWorkbook;
		NativeShapeSolidFill solidFill;
		NativeShapeGradientFill gradientFill;
		#endregion
		protected NativeShapeOutlineFill(TModelFillOwner modelFillOwner, NativeWorkbook nativeWorkbook) {
			this.modelFillOwner = modelFillOwner;
			this.nativeWorkbook = nativeWorkbook;
			SubscribeEvents();
		}
		#region Properties
		protected TModelFillOwner ModelFillOwner { get { return modelFillOwner; } }
		protected IDrawingFill ModelFill { get { return modelFillOwner.Fill; } set { modelFillOwner.Fill = value; } }
		protected UnitConverter UnitConverter { get { return nativeWorkbook.UnitConverter; } }
		protected IDocumentModel DocumentModel { get { return modelFillOwner.DocumentModel; } }
		#endregion
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			SetValidateNativeFields(value);
			if (!value)
				UnsubscribeEvents();
		}
		#region ShapeOutlineFill Members
		public ShapeFillType FillType {
			get {
				CheckValid();
				return (ShapeFillType)ModelFill.FillType;
			}
		}
		public ShapeSolidFill SolidFill {
			get {
				CheckValid();
				return solidFill;
			}
		}
		public ShapeGradientFill GradientFill {
			get {
				CheckValid();
				return gradientFill;
			}
		}
		public void SetAutoFill() {
			CheckValid();
			ModelFill = DrawingFill.Automatic;
		}
		public void SetNoFill() {
			CheckValid();
			ModelFill = DrawingFill.None;
		}
		public void SetSolidFill(Color color) {
			CheckValid();
			ModelFill = DrawingSolidFill.Create(DocumentModel, color);
		}
		public void SetGradientFill(ShapeGradientType gradientType, IList<DevExpress.Spreadsheet.Drawings.GradientStopInfo> stopInfoes) {
			CheckValid();
			ModelFill = DrawingGradientFill.Create(DocumentModel, (GradientType)gradientType, GetDrawingGradientStopInfoes(stopInfoes));
		}
		public void SetGradientFill(ShapeGradientType gradientType, Color firstColor, Color lastColor) {
			CheckValid();
			ModelFill = DrawingGradientFill.Create(DocumentModel, (GradientType)gradientType, GetDrawingGradientStopInfoes(firstColor, lastColor));
		}
		#endregion
		#region Internal
		#region SubscribeEvents
		protected abstract void SubscribeEvents(); 
		protected abstract void UnsubscribeEvents();
		#endregion
		protected void OnSetFill(object sender, SetFillEventArgs e) {
			SetValidateNativeFields(false);
			SetNullFields();
			SetFillCore(e.Fill);
		}
		protected virtual void SetFillCore(IDrawingFill fill) {
			DrawingFillType fillType = fill.FillType;
			if (fillType == DrawingFillType.Solid)
				SetNativeSolidFill(fill as DrawingSolidFill);
			else if (fillType == DrawingFillType.Gradient)
				SetNativeGradientFill(fill as DrawingGradientFill);
		}
		protected virtual void SetNullFields() {
			solidFill = null;
			gradientFill = null;
		}
		protected void SetNativeSolidFill(DrawingSolidFill fill) {
			solidFill = new NativeShapeSolidFill(fill);
		}
		protected void SetNativeGradientFill(DrawingGradientFill fill) {
			gradientFill = new NativeShapeGradientFill(fill);
		}
		protected virtual void SetValidateNativeFields(bool value) {
			if (this.solidFill != null)
				this.solidFill.IsValid = value;
			if (this.gradientFill != null)
				this.gradientFill.IsValid = value;
		}
		IList<DevExpress.Office.Drawing.GradientStopInfo> GetDrawingGradientStopInfoes(IList<DevExpress.Spreadsheet.Drawings.GradientStopInfo> stopInfoes) {
			IList<DevExpress.Office.Drawing.GradientStopInfo> result = new List<DevExpress.Office.Drawing.GradientStopInfo>();
			int count = stopInfoes.Count;
			for (int i = 0; i < count; i++) {
				DevExpress.Spreadsheet.Drawings.GradientStopInfo info = stopInfoes[i];
				result.Add(CreateDrawingGradientStopInfo(info.Color, info.Position));
			}
			return result;
		}
		IList<DevExpress.Office.Drawing.GradientStopInfo> GetDrawingGradientStopInfoes(Color firstColor, Color lastColor) {
			IList<DevExpress.Office.Drawing.GradientStopInfo> result = new List<DevExpress.Office.Drawing.GradientStopInfo>();
			result.Add(CreateDrawingGradientStopInfo(firstColor, 0));
			result.Add(CreateDrawingGradientStopInfo(lastColor, 1));
			return result;
		}
		DevExpress.Office.Drawing.GradientStopInfo CreateDrawingGradientStopInfo(Color color, double position) {
			DevExpress.Office.Drawing.GradientStopInfo result = new DevExpress.Office.Drawing.GradientStopInfo();
			result.Color = color;
			result.Position = DevExpress.Office.Drawing.DrawingValueConverter.ToPercentage(position);
			return result;
		}
		#endregion
	}
	#endregion
	#region NativeShapePictureFill
	partial class NativeShapePictureFill : NativeObjectBase, ShapePictureFill {
		#region Fields
		readonly DrawingBlipFill modelFill;
		readonly UnitConverter unitConverter;
		NativeShapeBlipFillRectangleOffset blipFillRectangleOffset;
		NativeShapeBlipFillSourceRectangleOffset blipFillSourceRectangleOffset;
		#endregion
		public NativeShapePictureFill(DrawingBlipFill modelFill, UnitConverter unitConverter) {
			this.modelFill = modelFill;
			this.unitConverter = unitConverter;
		}
		protected override void SetIsValid(bool value) {
			base.SetIsValid(value);
			if (blipFillRectangleOffset != null)
				blipFillRectangleOffset.IsValid = value;
			if (blipFillSourceRectangleOffset != null)
				blipFillSourceRectangleOffset.IsValid = value;
		}
		#region ShapePictureFill Members
		public OfficeImage Image {
			get {
				CheckValid();
				return modelFill.Blip.Image;
			}
		}
		public bool Stretch {
			get {
				CheckValid();
				return modelFill.Stretch;
			}
			set {
				CheckValid();
				modelFill.Stretch = value;
			}
		}
		public TileAlignType AlignType {
			get {
				CheckValid();
				return (TileAlignType)modelFill.TileAlign;
			}
			set {
				CheckValid();
				modelFill.TileAlign = (RectangleAlignType)value;
			}
		}
		public ShapeRectangleOffset SourceRect {
			get {
				CheckValid();
				if (blipFillSourceRectangleOffset == null)
					blipFillSourceRectangleOffset = new NativeShapeBlipFillSourceRectangleOffset(modelFill);
				return blipFillSourceRectangleOffset;
			}
		}
		public double ScaleX {
			get {
				CheckValid();
				return DrawingValueConverter.FromPercentage(modelFill.ScaleX);
			}
			set {
				CheckValid();
				modelFill.ScaleX = DrawingValueConverter.ToPercentage(value);
			}
		}
		public double ScaleY {
			get {
				CheckValid();
				return DrawingValueConverter.FromPercentage(modelFill.ScaleY);
			}
			set {
				CheckValid();
				modelFill.ScaleY = DrawingValueConverter.ToPercentage(value);
			}
		}
		public double OffsetX {
			get {
				CheckValid();
				return unitConverter.ToUnits(modelFill.OffsetX);
			}
			set {
				CheckValid();
				modelFill.OffsetX = (long)unitConverter.FromUnits((float)value);
			}
		}
		public double OffsetY {
			get {
				CheckValid();
				return unitConverter.ToUnits(modelFill.OffsetY);
			}
			set {
				CheckValid();
				modelFill.OffsetY = (long)unitConverter.FromUnits((float)value);
			}
		}
		public bool RotateWithShape {
			get {
				CheckValid();
				return modelFill.RotateWithShape;
			}
			set {
				CheckValid();
				modelFill.RotateWithShape = value;
			}
		}
		public TileFlipType FlipType {
			get {
				CheckValid();
				return (TileFlipType)modelFill.TileFlip;
			}
			set {
				CheckValid();
				modelFill.TileFlip = (TileFlipType)value;
			}
		}
		public ShapeRectangleOffset FillRect {
			get {
				CheckValid();
				if (blipFillRectangleOffset == null)
					blipFillRectangleOffset = new NativeShapeBlipFillRectangleOffset(modelFill);
				return blipFillRectangleOffset;
			}
		}
		#endregion
	}
	#endregion
	#region NativeShapeFill
	partial class NativeShapeFill : NativeShapeOutlineFill<Model.ShapeProperties>, ShapeFill {
		#region Fields
		NativeShapePatternFill nativePatternFill;
		NativeShapePictureFill nativePictureFill;
		#endregion
		public NativeShapeFill(Model.ShapeProperties shapeProperties, NativeWorkbook nativeWorkbook)
			: base(shapeProperties, nativeWorkbook) {
		}
		#region SubscribeEvents
		protected override void SubscribeEvents() {
			ModelFillOwner.SetFill += OnSetFill;
		}
		protected override void UnsubscribeEvents() {
			ModelFillOwner.SetFill -= OnSetFill;
		}
		#endregion
		#region ShapeFill Members
		public ShapePatternFill PatternFill {
			get {
				CheckValid();
				return nativePatternFill; 
			}
		}
		public ShapePictureFill PictureFill {
			get {
				CheckValid();
				return nativePictureFill;
			}
		}
		public void SetPatternFill(Color foregroundColor, Color backgroundColor, ShapeFillPatternType patternType) {
			CheckValid();
			ModelFill = DrawingPatternFill.Create(DocumentModel, foregroundColor, backgroundColor, (DrawingPatternType)patternType);
		}
#if !SL
		public void SetPictureFill(Image image) {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				ModelFill = DrawingBlipFill.Create(DocumentModel, DocumentModel.CreateImage(image));
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public void SetPictureFill(string filename) {
			CheckValid();
			DocumentModel.BeginUpdate();
			try {
				ModelFill = DrawingBlipFill.Create(DocumentModel, filename);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
#endif
		public void SetPictureFill(SpreadsheetImageSource imageSource) {
			CheckValid();
			Model.DocumentModel documentModel = DocumentModel as Model.DocumentModel;
			if (documentModel != null) {
				OfficeImage image = imageSource.CreateImage(documentModel);
				ModelFill = DrawingBlipFill.Create(DocumentModel, image);
			}
		}
		#endregion
		protected override void SetFillCore(IDrawingFill fill) {
			DrawingFillType fillType = fill.FillType;
			if (fillType == DrawingFillType.Solid)
				SetNativeSolidFill(fill as DrawingSolidFill);
			else if (fillType == DrawingFillType.Gradient)
				SetNativeGradientFill(fill as DrawingGradientFill);
			else if (fillType == DrawingFillType.Pattern)
				SetNativePatternFill(fill as DrawingPatternFill);
			else if (fillType == DrawingFillType.Picture)
				SetNativePictureFill(fill as DrawingBlipFill);
		}
		void SetNativePictureFill(DrawingBlipFill drawingBlipFill) {
			nativePictureFill = new NativeShapePictureFill(drawingBlipFill, UnitConverter);
		}
		void SetNativePatternFill(DrawingPatternFill drawingPatternFill) {
			nativePatternFill = new NativeShapePatternFill(drawingPatternFill);
		}
		protected override void SetNullFields() {
			base.SetNullFields();
			nativePatternFill = null;
			nativePictureFill = null;
		}
		protected override void SetValidateNativeFields(bool value) {
			base.SetValidateNativeFields(value);
			if (nativePatternFill != null)
				nativePatternFill.IsValid = value;
			if (nativePictureFill != null)
				nativePictureFill.IsValid = value;
		}
	}
	#endregion
}
