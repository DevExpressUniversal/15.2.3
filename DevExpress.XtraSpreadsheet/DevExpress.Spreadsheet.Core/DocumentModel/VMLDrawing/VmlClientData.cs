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
using System.Text;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Model {
	public class VmlAnchorData : ISupportsCopyFrom<VmlAnchorData>, ISupportsNoChangeAspect {
		const string AnchorDelimiter = ", ";
		public static VmlAnchorData FromString(string value, Worksheet worksheet) {
			VmlAnchorData result = new VmlAnchorData(worksheet);
			try {
				string[] parts = value.Split(new[] { ',', ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				int leftColumn = Convert.ToInt32(parts[0]);
				int leftOffset = Convert.ToInt32(parts[1]);
				int topRow = Convert.ToInt32(parts[2]);
				int topOffset = Convert.ToInt32(parts[3]);
				int rightColumn = Convert.ToInt32(parts[4]);
				int rightOffset = Convert.ToInt32(parts[5]);
				int bottomRow = Convert.ToInt32(parts[6]);
				int bottomOffset = Convert.ToInt32(parts[7]);
				result.SetFrom(worksheet, leftColumn, leftOffset, topRow, topOffset);
				result.SetTo(worksheet, rightColumn, rightOffset, bottomRow, bottomOffset);
			}
			catch {
				throw new Exception("Wrong Vml client data anchor string");
			}
			return result;
		}
		#region Fields
		readonly AnchorData anchorData;
		#endregion
		#region Properties
		public int LeftColumn { get { return AnchorData.From.Col; } }
		public int LeftOffset { get { return ModelUnitsToPixelsX(AnchorData.From.ColOffset); } }
		public int TopRow { get { return AnchorData.From.Row; } }
		public int TopOffset { get { return ModelUnitsToPixelsY(AnchorData.From.RowOffset); } }
		public int RightColumn { get { return AnchorData.To.Col; } }
		public int RightOffset { get { return ModelUnitsToPixelsX(AnchorData.To.ColOffset); } }
		public int BottomRow { get { return AnchorData.To.Row; } }
		public int BottomOffset { get { return ModelUnitsToPixelsY(AnchorData.To.RowOffset); } }
		public AnchorData AnchorData { get { return anchorData; } }
		DocumentModel Workbook { get { return AnchorData.DocumentModel; } }
		public bool MoveWithCells { get { return AnchorData.ResizingBehavior != AnchorType.Absolute; } set { SetAnchorType(value, SizeWithCells); } }
		public bool SizeWithCells { get { return AnchorData.ResizingBehavior == AnchorType.TwoCell; } set { SetAnchorType(MoveWithCells, value); } }
		public float Width { get { return AnchorData.Width; } set { anchorData.Width = value; } }
		public float Height { get { return AnchorData.Height; } set { anchorData.Height = value; } }
		public AnchorPoint From { get { return AnchorData.From; } }
		public AnchorPoint To { get { return AnchorData.To; } }
		public bool LockAspectRatio { get; set; }
		public bool NoChangeAspect { get { return LockAspectRatio; } set { LockAspectRatio = value; } }
		#endregion
		public VmlAnchorData(Worksheet worksheet) {
			anchorData = new AnchorData(worksheet, this);
			anchorData.AnchorType = AnchorType.TwoCell;
			anchorData.ResizingBehavior = AnchorType.Absolute;
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(LeftColumn);
			sb.Append(AnchorDelimiter);
			sb.Append(LeftOffset);
			sb.Append(AnchorDelimiter);
			sb.Append(TopRow);
			sb.Append(AnchorDelimiter);
			sb.Append(TopOffset);
			sb.Append(AnchorDelimiter);
			sb.Append(RightColumn);
			sb.Append(AnchorDelimiter);
			sb.Append(RightOffset);
			sb.Append(AnchorDelimiter);
			sb.Append(BottomRow);
			sb.Append(AnchorDelimiter);
			sb.Append(BottomOffset);
			return sb.ToString();
		}
		public void CopyFrom(VmlAnchorData value) {
			anchorData.CopyFrom(value.AnchorData);
		}
		float PixelsToModelUnitsX(int pixels) {
			return PixelsToModelUnitsCore(pixels, DocumentModel.DpiX);
		}
		float PixelsToModelUnitsY(int pixels) {
			return PixelsToModelUnitsCore(pixels, DocumentModel.DpiY);
		}
		float PixelsToModelUnitsCore(int pixels, float dpi) {
			return Workbook.UnitConverter.PixelsToModelUnits(pixels, dpi);
		}
		int ModelUnitsToPixelsX(float model) {
			return ModelUnitsToPixelsCore(model, DocumentModel.DpiX);
		}
		int ModelUnitsToPixelsY(float model) {
			return ModelUnitsToPixelsCore(model, DocumentModel.DpiY);
		}
		int ModelUnitsToPixelsCore(float model, float dpi) {
			return (int)Workbook.UnitConverter.ModelUnitsToPixelsF(model, dpi);
		}
		void SetAnchorType(bool moveWithCells, bool sizeWithCells) {
			AnchorData.AnchorType = AnchorType.TwoCell;
			AnchorData.ResizingBehavior = moveWithCells ? sizeWithCells ? AnchorType.TwoCell : AnchorType.OneCell : AnchorType.Absolute;
		}
		public void SetFrom(InternalSheetBase sheet, int leftColumn, int leftOffset, int topRow, int topOffset) {
			AnchorPoint point = new AnchorPoint(sheet.SheetId, leftColumn, topRow, PixelsToModelUnitsX(leftOffset), PixelsToModelUnitsY(topOffset));
			AnchorData.SetStartingPosition(point);
		}
		public void SetTo(InternalSheetBase sheet, int rightColumn, int rightOffset, int bottomRow, int bottomOffset) {
			AnchorPoint point = new AnchorPoint(sheet.SheetId, rightColumn, bottomRow, PixelsToModelUnitsX(rightOffset), PixelsToModelUnitsY(bottomOffset));
			AnchorData.SetEndingPosition(point);
		}
		public void Move(float offsetX, float offsetY) {
			AnchorData.Move(offsetY, offsetX);
		}
		public void SetIndependentSize(float width, float height) {
			AnchorData.SetIndependentHeight(height);
			AnchorData.SetIndependentWidth(width);
		}
	}
	public class VmlClientData {
		#region Fields
		VmlAnchorData anchor; 
		bool autoFill; 
		int row; 
		int column; 
		bool locked; 
		bool lockText; 
		bool visible; 
		VmlObjectType objectType; 
		bool moveWithCells;
		bool sizeWithCells;
		bool lockAspectRatio;
		bool cameraTool;
		string cameraSourceReference;
		ClipboardFormat clipboardFormat;
		XlHorizontalAlignment textHAlign; 
		XlVerticalAlignment textVAlign; 
		#endregion
		public VmlClientData(Worksheet sheet) {
			autoFill = false;
			locked = true;
			lockText = true;
			visible = false;
			objectType = VmlObjectType.Note;
			column = -1;
			row = -1;
			textHAlign = XlHorizontalAlignment.Left;
			textVAlign = XlVerticalAlignment.Top;
			moveWithCells = true;
			sizeWithCells = true;
			Worksheet = sheet;
			this.anchor = new VmlAnchorData(sheet);
		}
		#region Properties
		public VmlAnchorData Anchor {
			get { return anchor; }
			set {
				anchor = value;
				if (anchor == null)
					return;
				anchor.MoveWithCells = moveWithCells;
				anchor.SizeWithCells = sizeWithCells;
				anchor.LockAspectRatio = lockAspectRatio;
			}
		}
		public bool AutoFill { get { return autoFill; } set { autoFill = value; } }
		public int Row { get { return row; } set { row = value; } }
		public int Column { get { return column; } set { column = value; } }
		public VmlObjectType ObjectType { get { return objectType; } set { objectType = value; } }
		public bool Locked { get { return locked; } set { locked = value; } }
		public bool LockText { get { return lockText; } set { lockText = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public bool MoveWithCells {
			get { return anchor == null ? moveWithCells : anchor.MoveWithCells; }
			set {
				moveWithCells = value;
				if (Anchor != null)
					Anchor.MoveWithCells = value;
			}
		}
		public bool SizeWithCells {
			get { return anchor == null ? sizeWithCells : anchor.SizeWithCells; }
			set {
				sizeWithCells = value;
				if (Anchor != null)
					Anchor.SizeWithCells = value;
			}
		}
		public XlHorizontalAlignment TextHAlign { get { return textHAlign; } set { textHAlign = value; } }
		public XlVerticalAlignment TextVAlign { get { return textVAlign; } set { textVAlign = value; } }
		public Worksheet Worksheet { get; set; }
		public bool LockAspectRatio {
			get { return anchor == null ? lockAspectRatio : anchor.LockAspectRatio; }
			set {
				lockAspectRatio = value;
				if (anchor != null)
					anchor.LockAspectRatio = value;
			}
		}
		public bool CameraTool { get { return cameraTool; } set { cameraTool = value; } }
		public string CameraSourceReference { get { return cameraSourceReference; } set { cameraSourceReference = value; } }
		public ClipboardFormat ClipboardFormat { get { return clipboardFormat; } set { clipboardFormat = value; } }
		#endregion
		public void CopyFrom(VmlClientData source) {
			CopyAnchor(source.Anchor);
			AutoFill = source.AutoFill;
			Row = source.Row;
			Column = source.Column;
			Locked = source.Locked;
			LockText = source.LockText;
			Visible = source.Visible;
			ObjectType = source.ObjectType;
			MoveWithCells = source.MoveWithCells;
			SizeWithCells = source.SizeWithCells;
			TextHAlign = source.TextHAlign;
			TextVAlign = source.TextVAlign;
			cameraTool = source.CameraTool;
			cameraSourceReference = source.cameraSourceReference;
			clipboardFormat = source.ClipboardFormat;
		}
		void CopyAnchor(VmlAnchorData sourceData) {
			Anchor = new VmlAnchorData(Worksheet);
			AnchorPoint from = sourceData.From;
			AnchorPoint to = sourceData.To;
			anchor.SetFrom(Worksheet, from.Col, (int)from.ColOffset, from.Row, (int)from.RowOffset);
			anchor.SetTo(Worksheet, to.Col, (int)to.ColOffset, to.Row, (int)to.RowOffset);
		}
		internal void SetReference(CellPosition position) {
			Column = position.Column;
			Row = position.Row;
			int leftColumn, rightColumn, leftOffset, rightOffset, topRow, bottomRow, topOffset, bottomOffset;
			if (Column + 3 < IndicesChecker.MaxColumnCount) {
				leftColumn = Column + 1;
				rightColumn = Column + 3;
				leftOffset = 15;
				rightOffset = 31;
			}
			else {
				leftColumn = Column - 3;
				rightColumn = Column - 1;
				leftOffset = 33;
				rightOffset = 49;
			}
			if (Row - 1 < 0) {
				topRow = 0;
				bottomRow = Row + 4;
				topOffset = 2;
				bottomOffset = 1;
			}
			else if (Row + 3 < IndicesChecker.MaxRowCount) {
				topRow = Row - 1;
				bottomRow = Row + 3;
				topOffset = 10;
				bottomOffset = 9;
			}
			else {
				topRow = IndicesChecker.MaxRowCount - 5;
				bottomRow = IndicesChecker.MaxRowCount - 1;
				topOffset = 9;
				bottomOffset = 8;
			}
			anchor.SetFrom(Worksheet, leftColumn, leftOffset, topRow, topOffset);
			anchor.SetTo(Worksheet, rightColumn, rightOffset, bottomRow, bottomOffset);
		}
	}
	#region VmlObjectType
	public enum VmlObjectType {
		Button,
		Checkbox,
		Dialog,
		Drop,
		Edit,
		GBox,
		Label,
		LineA,
		List,
		Movie,
		Note,
		Pict,
		Radio,
		RectA,
		Scroll,
		Spin,
		Shape,
		Group,
		Rect
	}
	#endregion
	#region ClipboardFormat
	public enum ClipboardFormat { 
		Empty, 
		Bitmap,
		Pict,
		PictOld,
		PictPrint,
		PictScreen
	}
	#endregion
}
