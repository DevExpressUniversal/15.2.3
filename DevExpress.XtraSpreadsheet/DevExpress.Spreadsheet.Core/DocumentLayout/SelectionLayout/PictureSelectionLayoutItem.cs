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

using System.Drawing;
using System.IO;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Layout;
using DevExpress.Office.Internal;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region PictureSelectionLayoutItem
	public class PictureSelectionLayoutItem : ISelectionLayoutItem {
		#region Fields
		readonly int hotZoneSize;
		const int defaultHotZoneSizeDocuments = 24;
		readonly SelectionLayout layout;
		readonly HotZoneCollection hotZones;
		int pictureIndex;
		Rectangle bounds;
		DrawingBox drawingBox;
		#endregion
		public PictureSelectionLayoutItem(SelectionLayout layout, int pictureIndex) {
			Guard.ArgumentNotNull(layout, "layout");
			Guard.ArgumentNonNegative(pictureIndex, "pictureIndex");
			this.layout = layout;
			this.pictureIndex = pictureIndex;
			this.hotZones = new HotZoneCollection();
			this.hotZoneSize = AlignSizeToPixel(layout.LayoutUnitConverter.DocumentsToLayoutUnits(defaultHotZoneSizeDocuments)); 
		}
		#region Properties
		public Rectangle Bounds { get { return bounds; } }
		public int PictureIndex { get { return pictureIndex; } }
		public SelectionLayout Layout { get { return layout; } }
		public DocumentModel DocumentModel { get { return layout.DocumentModel; } }
		public HotZoneCollection HotZones { get { return hotZones; } }
		#endregion
		public void Draw(ISelectionPainter selectionPainter) {
			selectionPainter.Draw(this);
		}
		public void Update(Page page) {
			this.drawingBox = GetDrawingBox(page.DrawingBoxes);
			if (drawingBox == null)
				return;
			this.bounds = drawingBox.Bounds;
			HotZones.Clear();
			AddHotZones(drawingBox);
		}
		DrawingBox GetDrawingBox(List<DrawingBox> boxes) {
			int count = boxes.Count;
			for (int i = 0; i < count; i++) {
				DrawingBox currentBox = boxes[i];
				if (currentBox.DrawingIndex == pictureIndex)
					return currentBox;
			}
			return null;
		}
		int AlignSizeToPixel(int layoutSize) {
			DocumentLayoutUnitConverter unitConverter = DocumentModel.LayoutUnitConverter;
			int result = unitConverter.LayoutUnitsToPixels(layoutSize, DocumentModel.Dpi);
			if ((result % 2) == 0)
				result++;
			return unitConverter.PixelsToLayoutUnits(result, DocumentModel.Dpi);
		}
		public void AddHotZones(DrawingBox drawingBox) {
			Rectangle drawingBounds = ValidateBounds(bounds, hotZoneSize);
			IGestureStateIndicator gestureIndicator = Layout.View.Control.InnerControl as IGestureStateIndicator;
			int verticalMiddleY = drawingBounds.Left + drawingBounds.Width / 2;
			int horizontalMiddleY = drawingBounds.Top + drawingBounds.Height / 2;
			int bottomY = drawingBounds.Bottom - 1;
			int rightX = drawingBounds.Right - 1;
			Matrix transformMatrix = drawingBox.BackwardTransformMatrix;
			SpreadsheetBehaviorOptions options = DocumentModel.BehaviorOptions;
			if (drawingBox.Drawing.CanRotate && options.Drawing.RotateAllowed && options.DragAllowed) {
				DrawingObjectRotationHotZone rotationHotZone = new DrawingObjectRotationHotZone(DocumentModel, drawingBox, gestureIndicator);
				rotationHotZone.LineEnd = new Point(verticalMiddleY, drawingBounds.Top);
				AddHotZone(rotationHotZone, verticalMiddleY, drawingBounds.Top - 2 * hotZoneSize, transformMatrix);
			}
			if (options.Drawing.ResizeAllowed && options.DragAllowed) {
				AddHotZone(new DrawingObjectTopLeftHotZone(DocumentModel, drawingBox, gestureIndicator), drawingBounds.Left, drawingBounds.Top, transformMatrix);
				AddHotZone(new DrawingObjectTopRightHotZone(DocumentModel, drawingBox, gestureIndicator), rightX, drawingBounds.Top, transformMatrix);
				AddHotZone(new DrawingObjectBottomLeftHotZone(DocumentModel, drawingBox, gestureIndicator), drawingBounds.Left, bottomY, transformMatrix);
				AddHotZone(new DrawingObjectBottomRightHotZone(DocumentModel, drawingBox, gestureIndicator), rightX, bottomY, transformMatrix);
				AddHotZone(new DrawingObjectTopMiddleHotZone(DocumentModel, drawingBox, gestureIndicator), verticalMiddleY, drawingBounds.Top, transformMatrix);
				AddHotZone(new DrawingObjectBottomMiddleHotZone(DocumentModel, drawingBox, gestureIndicator), verticalMiddleY, bottomY, transformMatrix);
				AddHotZone(new DrawingObjectMiddleLeftHotZone(DocumentModel, drawingBox, gestureIndicator), drawingBounds.Left, horizontalMiddleY, transformMatrix);
				AddHotZone(new DrawingObjectMiddleRightHotZone(DocumentModel, drawingBox, gestureIndicator), rightX, horizontalMiddleY, transformMatrix);
			}
		}
		protected internal static Rectangle ValidateBounds(Rectangle bounds, int hotZoneSize) {
			Rectangle result = bounds;
			int delta = hotZoneSize * 3 - result.Width;
			if (delta > 0) {
				result.X -= delta / 2;
				result.Width += delta;
			}
			delta = hotZoneSize * 3 - result.Height;
			if (delta > 0) {
				result.Y -= delta / 2;
				result.Height += delta;
			}
			return result;
		}
		protected internal virtual void AddHotZone(HotZone hotZone, int x, int y, Matrix transformMatrix) {
			hotZone.Bounds = RectangleFromCenter(x, y, hotZoneSize);
			hotZone.HitTestTransform = transformMatrix;
			HotZones.Add(hotZone);
		}
		protected internal static Rectangle RectangleFromCenter(int x, int y, int hotZoneSize) {
			int halfHotZoneSize = hotZoneSize / 2;
			return new Rectangle(x - halfHotZoneSize, y - halfHotZoneSize, hotZoneSize, hotZoneSize);
		}
	}
	#endregion
}
