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
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	partial class Worksheet {
		protected internal virtual Picture InsertPictureCore(OfficeImage image, CellKey topLeft, CellKey bottomRight, AnchorType anchorType, bool lockAspectRatio) {
			using(HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
				PictureInsertedHistoryItem item = new PictureInsertedHistoryItem(this);
				item.Image = image;
				item.TopLeft = topLeft;
				item.BottomRight = bottomRight;
				item.AnchorType = anchorType;
				item.LockAspectRatio = lockAspectRatio;
				Workbook.History.Add(item);
				item.Execute();
				if(Workbook.IsUpdateLocked)
					Workbook.ApplyChanges(DocumentModelChangeType.DrawingAdded);
				else
					Workbook.InnerApplyChanges(DocumentModelChangeActionsCalculator.CalculateChangeActions(DocumentModelChangeType.DrawingAdded));
				return item.Picture;
			}
		}
		public Picture InsertPictureAnchorTypeAbsoluteCore(OfficeImage image, float x, float y, float desiredWidth, float desiredHeight, bool lockAspectRatio) {
				using(HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					PictureAtAbsoluteCoordinatesInsertedHistoryItem item = new PictureAtAbsoluteCoordinatesInsertedHistoryItem(this);
					item.Image = image;
					item.CoordinateX = x;
					item.CoordinateY = y;
					item.DesiredWidth = desiredWidth;
					item.DesiredHeight = desiredHeight;
					item.LockAspectRatio = lockAspectRatio;
					Workbook.History.Add(item);
					item.Execute();
					if(Workbook.IsUpdateLocked)
						Workbook.ApplyChanges(DocumentModelChangeType.DrawingAdded);
					else
						Workbook.InnerApplyChanges(DocumentModelChangeActionsCalculator.CalculateChangeActions(DocumentModelChangeType.DrawingAdded));				
					return item.Picture;
				}
		}
		public Picture InsertPicture(OfficeImage image, CellKey topLeft, CellKey bottomRight, bool lockAspectRatio) {
			return InsertPictureCore(image, topLeft, bottomRight, AnchorType.TwoCell, lockAspectRatio);
		}
		public Picture InsertPicture(OfficeImage image, CellKey topLeft) {
			return InsertPictureCore(image, topLeft, topLeft, AnchorType.OneCell, true);
		}
		public Picture InsertPicture(OfficeImage image, float x, float y) {
			int pictureWidth = image.SizeInTwips.Width;
			int pictureHeight = image.SizeInTwips.Height;
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
			return InsertPictureAnchorTypeAbsoluteCore(image, x, y, pictureWidth, pictureHeight, true);
		}
		public Picture InsertPicture(OfficeImage image, float x, float y, float width, float height, bool lockAspectRatio) {
			return InsertPictureAnchorTypeAbsoluteCore(image, x, y, width, height, lockAspectRatio);
		}
		public void InsertChart(Chart chart) {
			ChartInsertedHistoryItem historyItem = new ChartInsertedHistoryItem(this, chart);
			Workbook.History.Add(historyItem);
			historyItem.Execute();
		}
		public void ClearDrawingObjectsCollection() {
			foreach (IDrawingObject drawing in DrawingObjects)
				drawing.Dispose();
			DrawingObjects.Clear();
			DrawingObjectsByZOrderCollections.Clear();
		}
		public void RemoveDrawingAt(int index) {
			if(index < 0 || index >= DrawingObjects.Count)
				Exceptions.ThrowArgumentException("index", index);
			DocumentHistory history = Workbook.History;
			DrawingRemoveAtHistoryItem historyItem = new DrawingRemoveAtHistoryItem(this, index);
			history.Add(historyItem);
			historyItem.Execute();
		}
		public void RemoveDrawing(IDrawingObject drawing) {
			int index = DrawingObjects.IndexOf(drawing);
			RemoveDrawingAt(index);
		}
		public void RemoveDrawings(List<int> indexes) {
			Workbook.BeginUpdate();
			try {
				using (HistoryTransaction transaction = new HistoryTransaction(Workbook.History)) {
					for (int i = indexes.Count - 1; i >= 0; i--) {
						RemoveDrawingAt(indexes[i]);
					}
				}
			}
			finally {
				Workbook.EndUpdate();
			}
		}
		public void MoveDrawingInPixels(int pictureIndex, int offsetX, int offsetY) {
			MoveDrawingInPixelCommand command = new MoveDrawingInPixelCommand(this, pictureIndex, offsetX, offsetY);
			command.Execute();
		}
		public void MoveDrawingInLayoutUnits(int pictureIndex, int offsetX, int offsetY) {
			MoveDrawingInLayoutUnitsCommand command = new MoveDrawingInLayoutUnitsCommand(this, pictureIndex, offsetX, offsetY);
			command.Execute();
		}
		public void SetDrawingSizeIndependent(int pictureIndex, int width, int height) {
			SetDrawingSizeIndependentCommand command = new SetDrawingSizeIndependentCommand(this, pictureIndex, width, height);
			command.Execute();
		}
		public void SetDrawingSizeIndependent(int pictureIndex, Size size) {
			SetDrawingSizeIndependentCommand command = new SetDrawingSizeIndependentCommand(this, pictureIndex, size);
			command.Execute();
		}
		public void SelectAllShapes() {
			SelectAllShapesCommand command = new SelectAllShapesCommand(this);
			command.Execute();
		}
		public void SelectNextShape() {
			SelectNextShapeCommand command = new SelectNextShapeCommand(this);
			command.Execute();
		}
		public void SelectPreviousShape() {
			SelectPreviousShapeCommand command = new SelectPreviousShapeCommand(this);
			command.Execute();
		}
		public void DeselectAllShapes() {
			DeselectAllShapesCommand command = new DeselectAllShapesCommand(this);
			command.Execute();
		}
		public void NormalizeZOrder() {
			DocumentHistory history = Workbook.History;
			ZOrderNormalizeHistoryItem historyItem = new ZOrderNormalizeHistoryItem(this);
			history.Add(historyItem);
			historyItem.Execute();
		}
		void OnDrawingRemoved(object sender, DrawingObjectsCollectionChangedEventArgs args) {
			this.selection.OnDrawingRemoved(args.Drawing.IndexInCollection);
			this.referenceEditSelection.OnDrawingRemoved(args.Drawing.IndexInCollection);
		}
	}
}
