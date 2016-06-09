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

using DevExpress.Office;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Office.Utils;
using System.IO;
using DevExpress.Office.Drawing;
namespace DevExpress.Snap.Core {
	public abstract class CustomRunObjectBase : ICustomRunObject {
		#region static 
		protected static float DocumentLayoutUnitToDpi(DevExpress.Office.DocumentLayoutUnit unit) {
			switch (unit) {
				case DevExpress.Office.DocumentLayoutUnit.Document:
					return GraphicsDpi.Document;
				case DevExpress.Office.DocumentLayoutUnit.Pixel:
					return GraphicsDpi.Pixel;
				case DevExpress.Office.DocumentLayoutUnit.Twip:
					return GraphicsDpi.Twips;
				default:
					return GraphicsDpi.Document;
			}
		}
		#endregion
		#region ICustomRunObject Members
		CustomRunPropertiesChangedEventHandler propertiesChanged;
		public event CustomRunPropertiesChangedEventHandler PropertiesChanged {
			add { propertiesChanged += value; }
			remove { propertiesChanged -= value; }
		}
		protected virtual void RaisePropertiesChanged(ICustomRunPropertiesChangedInfo info) {
			if (propertiesChanged != null) {
				CustomRunPropertiesChangedEventArgs e = new CustomRunPropertiesChangedEventArgs();
				e.ChangedInfo = info;
				propertiesChanged(this, e);
			}
		}
		public virtual void Export(PieceTable pieceTable, Painter painter, Rectangle bounds) {
			VisualBrick brick = GetVisualBrick(pieceTable, bounds);
			PrintingSystemBase ps = new PrintingSystemBase();
			brick.Initialize(ps, pieceTable.DocumentModel.LayoutUnitConverter.LayoutUnitsToDocuments(bounds));
			painter.DrawBrick(ps, brick, bounds);
		}
		public virtual OfficeImage ExportToImage(PieceTable pieceTable, Rectangle bounds) {
			VisualBrick brick = GetVisualBrick(pieceTable, bounds);
			return ExportToImage(pieceTable.DocumentModel, brick, bounds);
		}
		protected OfficeImage ExportToImage(DocumentModel documentModel, VisualBrick brick, Rectangle bounds) {
			PrintingSystemBase ps = new PrintingSystemBase();
			RectangleF rect = bounds;
			rect.Width = documentModel.UnitConverter.ModelUnitsToDocumentsF(bounds.Width);
			rect.Height = documentModel.UnitConverter.ModelUnitsToDocumentsF(bounds.Height);
			brick.Initialize(ps, rect);
			using (OfficeImage picture = documentModel.CreateImage(GetNativeImage(brick, ps, documentModel, bounds))) {
				byte[] bytes = picture.GetImageBytes(OfficeImageFormat.Png);
				MemoryStream stream = new MemoryStream(bytes);
				return documentModel.CreateImage(stream);
			}
		}
		Image GetNativeImage(VisualBrick brick, PrintingSystemBase ps, DocumentModel documentModel, Rectangle bounds) {
			VisualBrickExporter exporter = (VisualBrickExporter)ExportersFactory.CreateExporter(brick);
			bounds.Width = documentModel.UnitConverter.ModelUnitsToPixels(bounds.Width, DocumentModel.DpiX);
			bounds.Height = documentModel.UnitConverter.ModelUnitsToPixels(bounds.Height, DocumentModel.DpiY);
			return exporter.CreateImageProvider().CreateContentImage(ps, bounds, false, DocumentModel.Dpi);
		}
		public virtual string GetText() {
			return GetType().FullName;
		}
		public abstract VisualBrick GetVisualBrick(PieceTable pieceTable, Rectangle bounds);
		public virtual void Measure(BoxInfo boxInfo, IObjectMeasurer measurer, CustomRun run, DocumentModelUnitToLayoutUnitConverter unitConverter) {
			measurer.MeasureText(boxInfo, "q", run.Paragraph.DocumentModel.FontCache[run.FontCacheIndex]);
		}
		#endregion
		#region ICloneable<ICustomRunObject> Members
		public virtual ICustomRunObject Clone() {
			CustomRunObjectBase customRunObject = (CustomRunObjectBase)Activator.CreateInstance(GetType());
			customRunObject.CopyFrom(this);			
			return customRunObject;
		}
		protected virtual void CopyFrom(CustomRunObjectBase customRunObject) {
		}
		#endregion
	}
	public class CheckBoxRunObject : CustomRunObjectBase, IRectangularObject {
		#region IRectangularObject Members
		public Size ActualSize { get { return Size.Empty; } set { } }
		#endregion
		#region ICustomRunObject Members
		public override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer, CustomRun run, DocumentModelUnitToLayoutUnitConverter unitConverter) {
			base.Measure(boxInfo, measurer, run, unitConverter);
			boxInfo.Size = new Size(boxInfo.Size.Height, boxInfo.Size.Height);
		}
		public CheckState CheckState {
			get;
			set;
		}
		public override VisualBrick GetVisualBrick(PieceTable pieceTable, Rectangle bounds) {
			return CreateCheckBoxBrick(pieceTable);
		}
		CheckBoxBrick CreateCheckBoxBrick(PieceTable pieceTable) {
			float toDpi = DocumentLayoutUnitToDpi(pieceTable.DocumentModel.LayoutUnit);
			return new CheckBoxBrick() { CheckState = CheckState, ShouldAlignToBottom = true, BackColor = Color.Transparent, Sides = BorderSide.None, ToDpi = toDpi };
		}
		protected override void CopyFrom(CustomRunObjectBase customRunObject) {
			base.CopyFrom(customRunObject);
			CheckState = ((CheckBoxRunObject)customRunObject).CheckState;			
		}
		public override OfficeImage ExportToImage(PieceTable pieceTable, Rectangle bounds) {
			CheckBoxBrick brick = CreateCheckBoxBrick(pieceTable);
			DocumentModel documentModel = pieceTable.DocumentModel;
			int width = documentModel.UnitConverter.PixelsToModelUnits((int)brick.CheckSize.Width);
			int height = documentModel.UnitConverter.PixelsToModelUnits((int)brick.CheckSize.Height);
			return ExportToImage(documentModel, brick, new Rectangle(bounds.Location, new Size(width, height)));
		}
		#endregion
	}
	public class BarCodeRunObject : CustomRunObjectBase, IRectangularObject, IResizeableObject {
		#region inner classes
		class BarCodeRunObjectSizeChangedInfo : ICustomRunPropertiesChangedInfo {
			readonly Size oldSize;
			readonly Size newSize;
			public BarCodeRunObjectSizeChangedInfo(Size oldSize, Size newSize) {
				this.oldSize = oldSize;
				this.newSize = newSize;
			}
			public void Redo(ICustomRunObject customRunObject) {
				((BarCodeRunObject)customRunObject).SetActualSizeInternal(newSize);
			}
			public void Undo(ICustomRunObject customRunObject) {
				((BarCodeRunObject)customRunObject).SetActualSizeInternal(oldSize);
			}
		}
		#endregion
		public const int DefaultModule = 29;
		public const bool DefaultAutoModule = false;
		public const bool DefaultShowText = true;
		public const TextAlignment DefaultAlignment = TextAlignment.TopLeft;
		public const BarCodeOrientation DefaultOrientation = BarCodeOrientation.Normal;
		Size actualSize = new Size(1440, 1440);
		public BarCodeRunObject() {
			AutoModule = DefaultAutoModule;
			Module = DefaultModule;
			ShowText = DefaultShowText;
			Alignment = DefaultAlignment;
			TextAlignment = DefaultAlignment;
			Orientation = DefaultOrientation;
		}
		public TextAlignment Alignment { get; set; }
		public TextAlignment TextAlignment { get; set; }
		public BarCodeOrientation Orientation { get; set; }
		public string Text { get; set; }
		public BarCodeGeneratorBase BarCodeGenerator { get; set; }
		public double Module { get; set; }
		public bool ShowText { get; set; }
		public bool AutoModule { get; set; }
		public bool Resizeable { get { return true; } }
		protected override void CopyFrom(CustomRunObjectBase customRunObject) {
			BarCodeRunObject barCodeObject = (BarCodeRunObject)customRunObject;
			ActualSize = barCodeObject.ActualSize;
			Alignment = barCodeObject.Alignment;
			TextAlignment = barCodeObject.TextAlignment;
			Orientation = barCodeObject.Orientation;
			Text = barCodeObject.Text;
			BarCodeGenerator = barCodeObject.BarCodeGenerator;
			AutoModule = barCodeObject.AutoModule;
			Module = barCodeObject.Module;
			ShowText = barCodeObject.ShowText;
		}
		public Size ActualSize {
			get {
				return actualSize;
			}
			set {
				if (actualSize == value)
					return;
				BarCodeRunObjectSizeChangedInfo info = new BarCodeRunObjectSizeChangedInfo(actualSize, value);
				actualSize = value;
				RaisePropertiesChanged(info);
			}
		}
		public override VisualBrick GetVisualBrick(PieceTable pieceTable, Rectangle bounds) {
			float toDpi = DocumentLayoutUnitToDpi(pieceTable.DocumentModel.LayoutUnit);
			BarCodeBrick result = new BarCodeBrick() { FromDpi = GraphicsDpi.Twips, ToDpi = toDpi, Alignment = Alignment, Orientation = Orientation, Text = Text, Sides = BorderSide.None, AutoModule = AutoModule, Module = Module, Generator = BarCodeGenerator, ShowText = ShowText, BackColor = Color.Transparent };
			result.Style.TextAlignment = TextAlignment;
			result.Style.StringFormat = BrickStringFormat.Create(result.Style.TextAlignment, false);
			return result;
		}
		 public override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer, CustomRun run, DocumentModelUnitToLayoutUnitConverter unitConverter) {
			int width = unitConverter.ToLayoutUnits(actualSize.Width);
			int height = unitConverter.ToLayoutUnits(actualSize.Height);
			boxInfo.Size = new Size(width, height);
		}
		protected internal void SetActualSizeInternal(Size value) {
			this.actualSize = value;
			RaisePropertiesChanged(null);
		}
	}
}
