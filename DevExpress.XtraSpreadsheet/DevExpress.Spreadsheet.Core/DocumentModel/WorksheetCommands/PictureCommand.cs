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
using DevExpress.Utils;
using DevExpress.Office;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region MoveDrawingInPixelCommand
	public class MoveDrawingInPixelCommand : SpreadsheetModelCommand {
		#region Fields
		int drawingIndex;
		int offsetX;
		int offsetY;
		#endregion
		public MoveDrawingInPixelCommand(Worksheet worksheet, int drawingIndex, int offsetX, int offsetY)
			: base(worksheet) {
			Guard.ArgumentNonNegative(drawingIndex, "pictureIndex");
			this.drawingIndex = drawingIndex;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}
		protected internal override void ExecuteCore() {
			DocumentModelUnitConverter unitConverter = DocumentModel.UnitConverter;
			int modelOffsetX = unitConverter.PixelsToModelUnits(offsetX, DevExpress.XtraSpreadsheet.Model.DocumentModel.DpiX);
			int modelOffsetY = unitConverter.PixelsToModelUnits(offsetY, DevExpress.XtraSpreadsheet.Model.DocumentModel.DpiY);
			Worksheet.DrawingObjects[drawingIndex].Move(modelOffsetY, modelOffsetX);
		}
	}
	#endregion
	#region MoveDrawingInLayoutUnitsCommand
	public class MoveDrawingInLayoutUnitsCommand : SpreadsheetModelCommand {
		#region Fields
		int drawingIndex;
		int offsetX;
		int offsetY;
		#endregion
		public MoveDrawingInLayoutUnitsCommand(Worksheet worksheet, int drawingIndex, int offsetX, int offsetY)
			: base(worksheet) {
			Guard.ArgumentNonNegative(drawingIndex, "pictureIndex");
			this.drawingIndex = drawingIndex;
			this.offsetX = offsetX;
			this.offsetY = offsetY;
		}
		protected internal override void ExecuteCore() {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			int modelOffsetX = unitConverter.ToModelUnits(offsetX);
			int modelOffsetY = unitConverter.ToModelUnits(offsetY);
			Worksheet.DrawingObjects[drawingIndex].Move(modelOffsetY, modelOffsetX);
		}
	}
	#endregion
	#region SetDrawingSizeIndependentCommand
	public class SetDrawingSizeIndependentCommand : SpreadsheetModelCommand {
		#region Fields
		int drawingIndex;
		int width;
		int height;
		#endregion
		public SetDrawingSizeIndependentCommand(Worksheet worksheet, int drawingIndex, Size size)
			: this(worksheet, drawingIndex, size.Width, size.Height) {
		}
		public SetDrawingSizeIndependentCommand(Worksheet worksheet, int drawingIndex, int width, int height)
			: base(worksheet) {
			Guard.ArgumentNonNegative(drawingIndex, "pictureIndex");
			Guard.ArgumentNonNegative(width, "width");
			Guard.ArgumentNonNegative(height, "height");
			this.drawingIndex = drawingIndex;
			this.width = width;
			this.height = height;
		}
		protected internal override void ExecuteCore() {
			DocumentModelUnitToLayoutUnitConverter unitConverter = DocumentModel.ToDocumentLayoutUnitConverter;
			IDrawingObject drawing = Worksheet.DrawingObjects[drawingIndex];
			drawing.SetIndependentWidth(unitConverter.ToModelUnits(width));
			drawing.SetIndependentHeight(unitConverter.ToModelUnits(height));
		}
	}
	#endregion
	#region SelectAllShapesCommand
	public class SelectAllShapesCommand : SpreadsheetModelCommand {
		public SelectAllShapesCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void ExecuteCore() {
			SheetViewSelection selection = Worksheet.Selection;
			selection.ClearDrawingSelection();
			SelectAllDrawings(selection);
		}
		void SelectAllDrawings(SheetViewSelection selection) {
			DrawingObjectsCollection drawings = Worksheet.DrawingObjects;
			int count = drawings.Count;
			for (int i = 0; i < count; i++) {
				selection.AddSelectedDrawingIndex(drawings[i].IndexInCollection);
			}
		}
	}
	#endregion
	#region DeselectAllShapesCommand
	public class DeselectAllShapesCommand : SpreadsheetModelCommand {
		public DeselectAllShapesCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void ExecuteCore() {
			Worksheet.Selection.ClearDrawingSelection();
		}
	}
	#endregion
	#region SelectShapeCommandBase (abstract class)
	public abstract class SelectShapeCommandBase : SpreadsheetModelCommand {
		protected SelectShapeCommandBase(Worksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void ExecuteCore() {
			SheetViewSelection selection = Worksheet.Selection;
			if (!selection.IsDrawingSelected)
				return;
			List<int> pictureIndexes = selection.SelectedDrawingIndexes;
			int activeIndex = pictureIndexes[pictureIndexes.Count - 1];
			int lastIndex = Worksheet.DrawingObjects.Last.IndexInCollection;
			int newIndex = CalculateNewIndex(activeIndex, lastIndex);
			selection.SetSelectedDrawingIndex(newIndex);
		}
		protected abstract int CalculateNewIndex(int activeIndex, int lastIndex);
	}
	#endregion
	#region SelectNextShapeCommand
	public class SelectNextShapeCommand : SelectShapeCommandBase {
		public SelectNextShapeCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected override int CalculateNewIndex(int activeIndex, int lastIndex) {
			if (activeIndex == lastIndex)
				return 0;
			return activeIndex + 1;
		}
	}
	#endregion
	#region SelectPreviousShapeCommand
	public class SelectPreviousShapeCommand : SelectShapeCommandBase {
		public SelectPreviousShapeCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected override int CalculateNewIndex(int activeIndex, int lastIndex) {
			if (activeIndex == 0)
				return lastIndex;
			return activeIndex - 1;
		}
	}
	#endregion
}
