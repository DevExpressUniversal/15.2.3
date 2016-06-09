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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	#region ShapeRelativeHorizontalPosition
	public enum ShapeRelativeHorizontalPosition {
		Page = 0,
		Character,
		Column,
		Margin,
		LeftMargin,
		RightMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region ShapeRelativeVerticalPosition
	public enum ShapeRelativeVerticalPosition {
		Page = 0,
		Line,
		Paragraph,
		Margin,
		TopMargin,
		BottomMargin,
		InsideMargin,
		OutsideMargin
	}
	#endregion
	#region ShapeHorizontalAlignment
	public enum ShapeHorizontalAlignment {
		None = 0,
		Left,
		Center,
		Right,
		Inside,
		Outside
	}
	#endregion
	#region ShapeVerticalAlignment
	public enum ShapeVerticalAlignment {
		None = 0,
		Top,
		Center,
		Bottom,
		Inside,
		Outside
	}
	#endregion
	#region TextWrappingType
	public enum TextWrappingType {
		Square,
		Tight,
		Through,
		TopAndBottom,
		BehindText,
		InFrontOfText
	} 
	#endregion
	#region Shape
	[ComVisible(true)]
	public interface Shape {
		string Name { get; set; }
		DocumentRange Range { get; }
		float ScaleX { get; set; }
		float ScaleY { get; set; }
		float RotationAngle { get; set; }
		PointF Offset { get; set; }
		SizeF OriginalSize { get; }
		SizeF Size { get; set; }
		string PictureUri { get; set; }
		OfficeImage Picture { get; }
		int ZOrder { get; set; }
		bool LockAspectRatio { get; set; }
		ShapeVerticalAlignment VerticalAlignment { get; set; }
		ShapeHorizontalAlignment HorizontalAlignment { get; set; }
		ShapeRelativeHorizontalPosition RelativeHorizontalPosition { get; set; }
		ShapeRelativeVerticalPosition RelativeVerticalPosition { get; set; }
		TextWrappingType TextWrapping { get; set; }
		float MarginTop { get; set; }
		float MarginBottom { get; set; }
		float MarginLeft { get; set; }
		float MarginRight { get; set; }
		ShapeFill Fill { get; }
		ShapeLine Line { get; }
		TextBox TextBox { get; }
	}
	#endregion
	#region ReadOnlyShapeCollection
	[ComVisible(true)]
	public interface ReadOnlyShapeCollection : ISimpleCollection<Shape> {
		ReadOnlyShapeCollection Get(DocumentRange range);
	}
	#endregion
	#region ShapeCollection
	[ComVisible(true)]
	public interface ShapeCollection : ReadOnlyShapeCollection {
		Shape this[string name] { get; }
		Shape InsertPicture(DocumentPosition pos, DocumentImageSource imageSource);
		Shape InsertTextBox(DocumentPosition pos);
#if !SL
		Shape InsertPicture(DocumentPosition pos, Image image);
#endif
	}
	#endregion
	#region ShapeFill
	public interface ShapeFill {
		Color Color { get; set; }
	}
	#endregion
	#region ShapeLine
	public interface ShapeLine {
		Color Color { get; set; }
		float Thickness { get; set; }
	}
	#endregion
	#region TextBoxSizeRule
	public enum TextBoxSizeRule {
		Auto,
		Exact
	}
	#endregion
	#region TextBox
	public interface TextBox {
		SubDocument Document { get; }
		float MarginTop { get; set; }
		float MarginBottom { get; set; }
		float MarginLeft { get; set; }
		float MarginRight { get; set; }
		TextBoxSizeRule HeightRule { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using DevExpress.Utils;
	using ModelPosition = DevExpress.XtraRichEdit.Model.DocumentModelPosition;
	using ModelRunIndex = DevExpress.XtraRichEdit.Model.RunIndex;
	using ModelRectangularScalableObject = DevExpress.XtraRichEdit.Model.IRectangularScalableObject;
	using ModelFloatingObjectAnchorRun = DevExpress.XtraRichEdit.Model.FloatingObjectAnchorRun;
	using ModelShape = DevExpress.XtraRichEdit.Model.Shape;
	using ModelPieceTable = DevExpress.XtraRichEdit.Model.PieceTable;
	using ModelFloatingObjectContent = DevExpress.XtraRichEdit.Model.FloatingObjectContent;
	using ModelPictureFloatingObjectContent = DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent;
	using ModelTextBoxFloatingObjectContent = DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent;
	using ModelFloatingObjectProperties = DevExpress.XtraRichEdit.Model.FloatingObjectProperties;
	using System.Collections;
	using System;
	using System.Collections.Generic;
	using ModelDocumentModel = DevExpress.XtraRichEdit.Model.DocumentModel;
	using ModelLogPosition = DevExpress.XtraRichEdit.Model.DocumentLogPosition;
	using ModelRunInfo = DevExpress.XtraRichEdit.Model.RunInfo;
	using Office.Utils.Internal;
	using Compatibility.System.Drawing;
	#region NativeShape
	public class NativeShape : Shape {
		readonly NativeSubDocument document;
		readonly NativeDocumentRange range;
		NativeTextBox textBox;
		NativeShapeFill fill;
		NativeShapeLine line;
		public static NativeShape TryCreate(NativeSubDocument document, DevExpress.XtraRichEdit.Model.TextRunBase run, ModelRunIndex runIndex) {
			ModelFloatingObjectAnchorRun anchorRun = run as ModelFloatingObjectAnchorRun;
			if (anchorRun != null)
				return new NativeShape(document, runIndex);
			return null;
		}
		public static NativeShape CreateUnsafe(NativeSubDocument document, ModelRunIndex runIndex) {
			return new NativeShape(document, runIndex);
		}
		NativeShape(NativeSubDocument document, ModelRunIndex runIndex) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
			ModelPosition start = ModelPosition.FromRunStart(document.PieceTable, runIndex);
			ModelPosition end = start.Clone();
			end.LogPosition = start.LogPosition + 1;
			this.range = new NativeDocumentRange(document, start, end);
		}
		ModelFloatingObjectAnchorRun AnchorRun { get { return (ModelFloatingObjectAnchorRun)document.PieceTable.Runs[range.Start.Position.RunIndex]; } }
		public ModelShape Shape { get { return AnchorRun.Shape; } }
		internal ModelFloatingObjectProperties Properties { get { return AnchorRun.FloatingObjectProperties; } }
		ModelRectangularScalableObject RectangleScalableObject { get { return (ModelRectangularScalableObject)document.PieceTable.Runs[range.Start.Position.RunIndex]; } }
		public DocumentModelUnitConverter UnitConverter { get { return document.PieceTable.DocumentModel.UnitConverter; } }
		#region GetTextWrapping
		TextWrappingType GetTextWrapping(FloatingObjectTextWrapType wrapType) {
			switch (wrapType) {
				case FloatingObjectTextWrapType.None:
					if (Properties.IsBehindDoc)
						return TextWrappingType.BehindText;
					return TextWrappingType.InFrontOfText;
				case FloatingObjectTextWrapType.TopAndBottom:
					return TextWrappingType.TopAndBottom;
				case FloatingObjectTextWrapType.Tight:
					return TextWrappingType.Tight;
				case FloatingObjectTextWrapType.Through:
					return TextWrappingType.Through;
				case FloatingObjectTextWrapType.Square:
					return TextWrappingType.Square;
				default:
					return TextWrappingType.InFrontOfText;
			}
		} 
		#endregion
		#region SetTextWrapping
		void SetTextWrapping(TextWrappingType wrapType) {
			Properties.IsBehindDoc = false;
			switch (wrapType) {
				case TextWrappingType.Square:
					Properties.TextWrapType = FloatingObjectTextWrapType.Square;
					break;
				case TextWrappingType.Tight:
					Properties.TextWrapType = FloatingObjectTextWrapType.Tight;
					break;
				case TextWrappingType.Through:
					Properties.TextWrapType = FloatingObjectTextWrapType.Through;
					break;
				case TextWrappingType.TopAndBottom:
					Properties.TextWrapType = FloatingObjectTextWrapType.TopAndBottom;
					break;
				case TextWrappingType.BehindText:
					Properties.TextWrapType = FloatingObjectTextWrapType.None;
					Properties.IsBehindDoc = true;
					break;
				case TextWrappingType.InFrontOfText:
					Properties.TextWrapType = FloatingObjectTextWrapType.None;
					break;
				default:
					Properties.TextWrapType = FloatingObjectTextWrapType.None;
					break;
			}
		} 
		#endregion
		#region Shape Members
		public TextWrappingType TextWrapping { get { return GetTextWrapping(Properties.TextWrapType); } set { SetTextWrapping(value); } }
		public string Name { get { return AnchorRun.Name; } set { AnchorRun.Name = value; } }
		public DocumentRange Range { get { return range; } }
		public float ScaleX {
			get { return RectangleScalableObject.ScaleX / 100.0f; }
			set { RectangleScalableObject.ScaleX = value * 100.0f; }
		}
		public float ScaleY {
			get { return RectangleScalableObject.ScaleY / 100.0f; }
			set { RectangleScalableObject.ScaleY = value * 100.0f; }
		}
		public float RotationAngle {
			get { return UnitConverter.ModelUnitsToDegreeF(Shape.Rotation); }
			set { Shape.Rotation = UnitConverter.DegreeToModelUnits(value); }
		}
		public PointF Offset {
			get {
				Point point = Properties.Offset;
				return new PointF(document.ModelUnitsToUnits(point.X), document.ModelUnitsToUnits(point.Y));
			}
			set {
				int x = document.UnitsToModelUnits(value.X);
				if (Math.Abs(x) < 1)
					x = 1;
				int y = document.UnitsToModelUnits(value.Y);
				if (Math.Abs(y) < 1)
					y = 1;
				Properties.Offset = new Point(x, y);
			}
		}
		public SizeF OriginalSize {
			get {
				Size originalSize = RectangleScalableObject.OriginalSize;
				return new SizeF(document.ModelUnitsToUnits(originalSize.Width), document.ModelUnitsToUnits(originalSize.Height));
			}
		}
		public SizeF Size {
			get {
				SizeF actualSize = Properties.ActualSize;
				return new SizeF(document.ModelUnitsToUnitsF(actualSize.Width), document.ModelUnitsToUnitsF(actualSize.Height));
			}
			set {
				int width = Math.Max(1, document.UnitsToModelUnits(value.Width));
				int height = Math.Max(1, document.UnitsToModelUnits(value.Height));
				Properties.ActualSize = new Size(width, height);
			}
		}
		public ShapeHorizontalAlignment HorizontalAlignment { get { return (ShapeHorizontalAlignment)Properties.HorizontalPositionAlignment; } set { Properties.HorizontalPositionAlignment = (DevExpress.XtraRichEdit.Model.FloatingObjectHorizontalPositionAlignment)value; } }
		public ShapeVerticalAlignment VerticalAlignment { get { return (ShapeVerticalAlignment)Properties.VerticalPositionAlignment; } set { Properties.VerticalPositionAlignment = (DevExpress.XtraRichEdit.Model.FloatingObjectVerticalPositionAlignment)value; } }
		public ShapeRelativeHorizontalPosition RelativeHorizontalPosition { get { return (ShapeRelativeHorizontalPosition)Properties.HorizontalPositionType; } set { Properties.HorizontalPositionType = (DevExpress.XtraRichEdit.Model.FloatingObjectHorizontalPositionType)value; } }
		public ShapeRelativeVerticalPosition RelativeVerticalPosition { get { return (ShapeRelativeVerticalPosition)Properties.VerticalPositionType; } set { Properties.VerticalPositionType = (DevExpress.XtraRichEdit.Model.FloatingObjectVerticalPositionType)value; } }
		public int ZOrder { get { return Properties.ZOrder; } set { Properties.ZOrder = value; } }
		public bool LockAspectRatio { get { return Properties.LockAspectRatio; } set { Properties.LockAspectRatio = value; } }
		public float MarginTop { get { return document.ModelUnitsToUnitsF(Properties.TopDistance); } set { Properties.TopDistance = document.UnitsToModelUnits(value); } }
		public float MarginBottom { get { return document.ModelUnitsToUnitsF(Properties.BottomDistance); } set { Properties.BottomDistance = document.UnitsToModelUnits(value); } }
		public float MarginLeft { get { return document.ModelUnitsToUnitsF(Properties.LeftDistance); } set { Properties.LeftDistance = document.UnitsToModelUnits(value); } }
		public float MarginRight { get { return document.ModelUnitsToUnitsF(Properties.RightDistance); } set { Properties.RightDistance = document.UnitsToModelUnits(value); } }
		public OfficeImage Picture {
			get {
				ModelPictureFloatingObjectContent content = AnchorRun.Content as ModelPictureFloatingObjectContent;
				if (content == null)
					return null;
				OfficeImage result = content.Image;
#if !SL // B201030 - RichEditControl freezes when trying to immediately access image created using DocumentImageSource.FromUri
				InternalOfficeImageHelper.EnsureLoadComplete(result);
#endif
				return result;
			}
		}
		public TextBox TextBox {
			get {
				if (textBox != null)
					return textBox;
				ModelTextBoxFloatingObjectContent content = AnchorRun.Content as ModelTextBoxFloatingObjectContent;
				if (content == null)
					return null;
				textBox = new NativeTextBox(document, content);
				return textBox;
			}
		}
		public string PictureUri {
			get {
				OfficeImage image = Picture;
				if (image != null)
					return image.Uri;
				else
					return String.Empty;
			}
			set {
				OfficeImage image = Picture;
				if (image != null)
					image.Uri = value;
			}
		}
		public ShapeFill Fill {
			get {
				if (fill != null)
					return fill;
				fill = new NativeShapeFill(this);
				return fill;
			}
		}
		public ShapeLine Line {
			get {
				if (line != null)
					return line;
				line = new NativeShapeLine(this);
				return line;
			}
		}
		#endregion
	}
	#endregion
	#region NativeTextBoxSubDocument
	public class NativeTextBoxSubDocument : NativeSubDocument {
		internal NativeTextBoxSubDocument(ModelPieceTable pieceTable, InnerRichEditDocumentServer server)
			: base(pieceTable, server) {
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertShapePicture(DocumentPosition pos, DocumentImageSource imageSource)' method instead.")]
		protected internal override Shape InsertShapePicture(DocumentPosition pos, DocumentImageSource imageSource) {
			ThrowInvalidOperationException();
			return null;
		}
		[Obsolete("This method has become obsolete. Use the 'DevExpress.XtraRichEdit.API.Native.Implementation.NativeShapeCollection.InsertTextBox(DocumentPosition pos)' method instead.")]
		public override Shape InsertTextBox(DocumentPosition pos) {
			ThrowInvalidOperationException();
			return null;
		}
		void ThrowInvalidOperationException() {
			throw new InvalidOperationException("Shape can't be inserted into TextBox.Document.");
		}
	}
	#endregion
	#region NativeTextBox
	public class NativeTextBox : TextBox {
		readonly NativeSubDocument document;
		readonly ModelTextBoxFloatingObjectContent content;
		readonly NativeTextBoxSubDocument textBoxContentDocument;
		internal NativeTextBox(NativeSubDocument document, ModelTextBoxFloatingObjectContent content) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(content, "content");
			this.document = document;
			this.content = content;
			this.textBoxContentDocument = new NativeTextBoxSubDocument(content.TextBox.PieceTable, document.DocumentServer);
		}
		public SubDocument Document { get { return textBoxContentDocument; } }
		DevExpress.XtraRichEdit.Model.TextBoxProperties Properties { get { return content.TextBoxProperties; } }
		public float MarginTop { get { return document.ModelUnitsToUnitsF(Properties.TopMargin); } set { Properties.TopMargin = document.UnitsToModelUnits(value); } }
		public float MarginBottom { get { return document.ModelUnitsToUnitsF(Properties.BottomMargin); } set { Properties.BottomMargin = document.UnitsToModelUnits(value); } }
		public float MarginLeft { get { return document.ModelUnitsToUnitsF(Properties.LeftMargin); } set { Properties.LeftMargin = document.UnitsToModelUnits(value); } }
		public float MarginRight { get { return document.ModelUnitsToUnitsF(Properties.RightMargin); } set { Properties.RightMargin = document.UnitsToModelUnits(value); } }
		public TextBoxSizeRule HeightRule {
			get { return Properties.ResizeShapeToFitText ? TextBoxSizeRule.Auto : TextBoxSizeRule.Exact; }
			set { Properties.ResizeShapeToFitText = value == TextBoxSizeRule.Auto; }
		}
	}
	#endregion
	#region NativeShapeFill
	public class NativeShapeFill : ShapeFill {
		readonly NativeShape shape;
		public NativeShapeFill(NativeShape shape) {
			Guard.ArgumentNotNull(shape, "shape");
			this.shape = shape;
		}
		ModelShape ModelShape { get { return shape.Shape; } }
		public Color Color { get { return ModelShape.FillColor; } set { ModelShape.FillColor = value; } }
	}
	#endregion
	#region NativeShapeLine
	public class NativeShapeLine : ShapeLine {
		readonly NativeShape shape;
		public NativeShapeLine(NativeShape shape) {
			Guard.ArgumentNotNull(shape, "shape");
			this.shape = shape;
		}
		ModelShape ModelShape { get { return shape.Shape; } }
		public Color Color { get { return ModelShape.OutlineColor; } set { ModelShape.OutlineColor = value; } }
		public float Thickness { get { return shape.UnitConverter.ModelUnitsToPointsF(ModelShape.OutlineWidth); } set { ModelShape.OutlineWidth = (int)Math.Round(shape.UnitConverter.PointsToModelUnitsF(value)); } }
	}
	#endregion
	#region NativeShapeCollection
	public class NativeShapeCollection : ShapeCollection {
		readonly List<Shape> innerList = new List<Shape>();
		readonly NativeSubDocument document;
		public NativeShapeCollection(NativeSubDocument document) {
			Guard.ArgumentNotNull(document, "document");
			this.document = document;
		}
		ModelPieceTable PieceTable { get { return document.PieceTable; } }
		ModelDocumentModel DocumentModel { get { return PieceTable.DocumentModel; } }
		public List<Shape> InnerList { get { return innerList; } }
		public Shape this[string name] {
			get {
				Shape result = null;
				ProcessShapes((run, runIndex) => {
					if (run.Name == name) {
						result = NativeShape.CreateUnsafe(document, new RunIndex(runIndex));
						return false;
					}
					return true;
				});
				return result;
			}
		}
		#region ISimpleCollection<Shape> Members
		Shape ISimpleCollection<Shape>.this[int index] { 
			get {
				int anchorRunCount = 0;
				Shape result = null;
				ProcessShapes((run, runIndex) => {
					anchorRunCount++;
					if (anchorRunCount - 1 == index) {
						result = NativeShape.CreateUnsafe(document, new RunIndex(runIndex));
						return false;
					}
					return true;
				});
				return result;
			}
		}
		#endregion
		void ProcessShapes(Func<ModelFloatingObjectAnchorRun, int, bool> action) {
			TextRunCollection runs = PieceTable.Runs;
			int count = runs.Count;			
			for (int i = 0; i < count; i++) {
				ModelFloatingObjectAnchorRun anchorRun = runs[new RunIndex(i)] as ModelFloatingObjectAnchorRun;
				if (anchorRun != null) {
					if (!action(anchorRun, i))
						break;
				}
			}			
		}
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<Shape>)this).GetEnumerator();
		}
		#endregion
		#region IEnumerable<Shape> Members
		IEnumerator<Shape> IEnumerable<Shape>.GetEnumerator() {
			TextRunCollection runs = PieceTable.Runs;
			int count = runs.Count;
			for (int i = 0; i < count; i++) {
				ModelFloatingObjectAnchorRun anchorRun = runs[new RunIndex(i)] as ModelFloatingObjectAnchorRun;
				if (anchorRun != null)
					yield return NativeShape.CreateUnsafe(document, new RunIndex(i));
			}			
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			List<Shape> shapes = new List<Shape>();
			ProcessShapes((run, runIndex) => {
				shapes.Add(NativeShape.CreateUnsafe(document, new RunIndex(runIndex)));
				return true;
			});
			Array.Copy(shapes.ToArray(), 0, array, index, shapes.Count);
		}
		int ICollection.Count {
			get { 
				int result = 0;
				ProcessShapes((run, runIndex) => {
					result++;
					return true;
				});
				return result;
			} 
		}
		bool ICollection.IsSynchronized {
			get {
				return false;
			}
		}
		object ICollection.SyncRoot {
			get {
				return this;
			}
		}
		#endregion
		public Shape InsertPicture(DocumentPosition pos, DocumentImageSource imageSource) {
			return InsertShapePicture(pos, imageSource);
		}
		protected internal virtual Shape InsertShapePicture(DocumentPosition pos, DocumentImageSource imageSource) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			OfficeImage image = imageSource.CreateImage(DocumentModel);
			if (image == null)
				return null;
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = document.NormalizeLogPosition(pos.LogPosition);
			DocumentModel.BeginUpdate();
			try {
				PieceTable.InsertFloatingObjectAnchor(logPosition);
				PieceTable.CalculateRunInfoStart(logPosition, runInfo);
				ModelFloatingObjectAnchorRun run = (ModelFloatingObjectAnchorRun)PieceTable.Runs[runInfo.Start.RunIndex];
				DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent content = new DevExpress.XtraRichEdit.Model.PictureFloatingObjectContent(run, image);
				run.SetContent(content);
				Size size = DocumentModel.UnitConverter.TwipsToModelUnits(image.SizeInTwips);
				run.FloatingObjectProperties.ActualSize = size;
				content.SetOriginalSize(size);
				return NativeShape.CreateUnsafe(document, runInfo.Start.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public virtual Shape InsertTextBox(DocumentPosition pos) {
			document.CheckValid();
			document.CheckDocumentPosition(pos);
			ModelRunInfo runInfo = new ModelRunInfo(PieceTable);
			ModelLogPosition logPosition = document.NormalizeLogPosition(pos.LogPosition);
			DocumentModel.BeginUpdate();
			try {
				PieceTable.InsertFloatingObjectAnchor(logPosition);
				PieceTable.CalculateRunInfoStart(logPosition, runInfo);
				ModelFloatingObjectAnchorRun run = (ModelFloatingObjectAnchorRun)PieceTable.Runs[runInfo.Start.RunIndex];
				DevExpress.XtraRichEdit.Model.TextBoxContentType textBoxContentType = new DevExpress.XtraRichEdit.Model.TextBoxContentType(DocumentModel);
				DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent content = new DevExpress.XtraRichEdit.Model.TextBoxFloatingObjectContent(run, textBoxContentType);
				run.SetContent(content);
				Size size = DocumentModel.UnitConverter.TwipsToModelUnits(new Size(3 * 1440, 2 * 1440));
				run.FloatingObjectProperties.ActualSize = size;
				return NativeShape.CreateUnsafe(document, runInfo.Start.RunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
#if !SL
		public Shape InsertPicture(DocumentPosition pos, Image image) {
			return InsertShapePicture(pos, DocumentImageSource.FromImage(image));
		}
#endif
		public ReadOnlyShapeCollection Get(DocumentRange range) {
			document.CheckValid();
			document.CheckDocumentRange(range);
			NativeDocumentRange nativeRange = (NativeDocumentRange)range;
			ModelRunIndex firstRunIndex = nativeRange.NormalizedStart.Position.RunIndex;
			ModelRunIndex lastRunIndex = nativeRange.NormalizedEnd.Position.RunIndex;
			if (nativeRange.NormalizedEnd.Position.RunOffset == 0)
				lastRunIndex--;
			NativeReadOnlyShapeCollection result = new NativeReadOnlyShapeCollection();
			for (ModelRunIndex i = firstRunIndex; i <= lastRunIndex; i++) {
				NativeShape shape = NativeShape.TryCreate(document, PieceTable.Runs[i], i);
				if (shape != null )
					result.Add(shape);
			}
			return result;
		}
	}
	#endregion
	public abstract class NativeReadOnlyCollection<TItem, TNativeCollection, TAPICollection> : List<TItem> where TNativeCollection : List<TItem>, TAPICollection {
		public TAPICollection Get(DocumentRange range) {
			TNativeCollection result = CreateCollection();
			for (int i = 0; i < this.Count; i++) {
				if(Contains(range, this[i]))
					result.Add(this[i]);
			}
			return result;
		}
		protected abstract bool Contains(DocumentRange range, TItem item);
		protected abstract TNativeCollection CreateCollection();
	}
	public class NativeReadOnlyShapeCollection : NativeReadOnlyCollection<Shape, NativeReadOnlyShapeCollection, ReadOnlyShapeCollection>, ReadOnlyShapeCollection {
		protected override NativeReadOnlyShapeCollection CreateCollection() {
			return new NativeReadOnlyShapeCollection();
		}
		protected override bool Contains(DocumentRange range, Shape item) {
			DocumentPosition start = item.Range.Start;
			return range.Contains(start);
		}
	}
}
