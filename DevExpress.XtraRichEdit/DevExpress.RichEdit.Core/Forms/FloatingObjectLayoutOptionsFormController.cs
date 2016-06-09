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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Reflection;
using System.IO;
using DevExpress.XtraRichEdit.Localization;
using System.Collections.Generic;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Design.Internal;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Forms {
	public class FloatingInlineObjectParameters {
		private FloatingObjectAnchorRun floatingObjectAnchorRun;
		readonly InlinePictureRun inlinePictureRun;
		internal FloatingObjectAnchorRun FloatingObjectAnchorRun { get { return floatingObjectAnchorRun; } set { floatingObjectAnchorRun = value; } }
		internal InlinePictureRun InlinePictureRun { get { return inlinePictureRun; } }
		public FloatingInlineObjectParameters(FloatingObjectAnchorRun floatingObjectAnchorRun) {
			this.floatingObjectAnchorRun = floatingObjectAnchorRun;
			this.inlinePictureRun = null;
		}
		public FloatingInlineObjectParameters(InlinePictureRun inlinePictureRun) {
			this.inlinePictureRun = inlinePictureRun;
			this.floatingObjectAnchorRun = null;
		}
	}
	public class FloatingInlineObjectLayoutOptionsFormControllerParameters : FormControllerParameters {
		#region Fields
		readonly FloatingInlineObjectParameters floatingInlineObjectParameters;
		PageSetupFormInitialTabPage initialTabPage;
		readonly FloatingObjectTargetPlacementInfo placementInfo;
		readonly Size? originalSize;
		private FloatingObjectProperties floatingObjectProperties;
		#endregion
		#region Properties
		internal FloatingInlineObjectParameters FloatingInlineObjectParameters { get { return floatingInlineObjectParameters; } }
		internal PageSetupFormInitialTabPage InitialTabPage { get { return initialTabPage; } set { initialTabPage = value; } }
		internal FloatingObjectTargetPlacementInfo PlacementInfo { get { return placementInfo; } }
		internal Size? OriginalSize { get { return originalSize; } }
		internal FloatingObjectProperties FloatingObjectProperties { get { return floatingObjectProperties; } set { floatingObjectProperties = value; } }
		#endregion
		internal FloatingInlineObjectLayoutOptionsFormControllerParameters(IRichEditControl control, FloatingInlineObjectParameters floatingInlineObjectParameters): base(control) {
			Guard.ArgumentNotNull(floatingInlineObjectParameters, "floatingInlineObjectParameters");
			this.floatingInlineObjectParameters = floatingInlineObjectParameters;
			this.placementInfo = new FloatingObjectTargetPlacementInfo();
			if (floatingInlineObjectParameters.FloatingObjectAnchorRun != null) {
				floatingObjectProperties = floatingInlineObjectParameters.FloatingObjectAnchorRun.FloatingObjectProperties;
				InitialFillPlacementInfo(floatingInlineObjectParameters.FloatingObjectAnchorRun.PieceTable, floatingInlineObjectParameters.FloatingObjectAnchorRun.Paragraph);
				FloatingObject floatingObject = floatingInlineObjectParameters.FloatingObjectAnchorRun.Shape.ShapeOwner as FloatingObject;
				this.originalSize = ((floatingObject == null) ? null : (Size?)floatingObject.Content.OriginalSize);
			}
			else {
				floatingObjectProperties = null;
				InitialFillPlacementInfo(floatingInlineObjectParameters.InlinePictureRun.PieceTable, floatingInlineObjectParameters.InlinePictureRun.Paragraph);
				this.originalSize = floatingInlineObjectParameters.InlinePictureRun.OriginalSize;
			}
		}
		public void InitialFillPlacementInfo(PieceTable pieceTable, Paragraph paragraph) {
			RichEditView activeView = Control.InnerControl.ActiveView;
			DocumentLogPosition logPosition = pieceTable.Paragraphs[paragraph.Index].LogPosition;
			DocumentLayoutPosition positionWithDetailsLevelColumn = activeView.DocumentLayout.CreateLayoutPosition(pieceTable, logPosition, 0);
			positionWithDetailsLevelColumn.Update(activeView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Column);
			placementInfo.ColumnBounds = positionWithDetailsLevelColumn.Column.Bounds;
			DocumentLayoutPosition positionWithDetailsLevelPage = activeView.DocumentLayout.CreateLayoutPosition(pieceTable, logPosition, 0);
			positionWithDetailsLevelPage.Update(activeView.DocumentLayout.Pages, DocumentLayoutDetailsLevel.Page);
			placementInfo.PageBounds = positionWithDetailsLevelColumn.Page.Bounds;
			placementInfo.PageClientBounds = positionWithDetailsLevelColumn.Page.ClientBounds;
		}
	}
	public class FloatingObjectLayoutOptionsFormController : FormController {
		#region Field
		readonly FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters;
		readonly FloatingInlineObjectParameters floatingInlineObjectParameters;
		readonly UIUnitConverter uiUnitConverter;
		FloatingObjectHorizontalPositionAlignment? horizontalPositionAlignment;
		FloatingObjectVerticalPositionAlignment? verticalPositionAlignment;
		FloatingObjectVerticalPositionType? verticalPositionType;
		FloatingObjectHorizontalPositionType? horizontalPositionType;
		bool? locked;
		int? offsetX;
		int? offsetY;
		bool isHorizontalAbsolutePosition;
		bool isVerticalAbsolutePosition;
		Dictionary<FloatingObjectHorizontalPositionType, XtraRichEditStringId> horizontalPositionTypeTable; 
		Dictionary<FloatingObjectHorizontalPositionAlignment, XtraRichEditStringId> horizontalPositionAlignmentTable;
		Dictionary<FloatingObjectVerticalPositionType, XtraRichEditStringId> verticalPositionTypeTable;
		Dictionary<FloatingObjectVerticalPositionAlignment, XtraRichEditStringId> verticalPositionAlignmentTable;
		bool? isBehindDocument;
		FloatingObjectRichTextIndentEditProperties floatingObjectRichTextIndentEditProperties; 
		FloatingObjectTextWrapType textWrapType;
		FloatingObjectTextWrapSide? textWrapSide;
		int? topDistance;
		int? bottomDistance;
		int? leftDistance;
		int? rightDistance;
		int actualHeight;
		int actualWidth;
		int? rotation;
		bool lockAspectRatio;
		#endregion
		#region Properties
		internal FloatingInlineObjectLayoutOptionsFormControllerParameters ControllerParameters { get { return controllerParameters; } }
		internal FloatingInlineObjectParameters FloatingInlineObjectParameters { get { return floatingInlineObjectParameters; } }
		internal FloatingObjectAnchorRun FloatingObjectAnchorRun { get { return floatingInlineObjectParameters.FloatingObjectAnchorRun; } }
		internal InlinePictureRun InlinePictureRun { get { return FloatingInlineObjectParameters.InlinePictureRun; } }
		internal FloatingObjectProperties FloatingObjectProperties { get { return controllerParameters.FloatingObjectProperties; } set { controllerParameters.FloatingObjectProperties = value; } }
		internal IRichEditControl Control { get { return ControllerParameters.Control; } }
		internal DocumentUnit UIType { get { return Control.InnerControl.UIUnit; } }
		internal DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		internal RichEditView ActiveView { get { return Control.InnerControl.ActiveView; } }
		internal PageSetupFormInitialTabPage InitialTabPage { get { return ControllerParameters.InitialTabPage; } }
		internal FloatingObjectTargetPlacementInfo PlacementInfo { get { return ControllerParameters.PlacementInfo; } }
		internal DocumentModelUnitToLayoutUnitConverter ToDocumentLayoutUnitConverter { get { return Control.InnerControl.DocumentModel.ToDocumentLayoutUnitConverter; } }
		internal FloatingObjectRichTextIndentEditProperties FloatingObjectRichTextIndentEditProperties { get { return floatingObjectRichTextIndentEditProperties; } }
		public FloatingObjectHorizontalPositionAlignment? HorizontalPositionAlignment { get { return horizontalPositionAlignment; } set { horizontalPositionAlignment = value; } }
		public FloatingObjectVerticalPositionAlignment? VerticalPositionAlignment { get { return verticalPositionAlignment; } set { verticalPositionAlignment = value; } }
		public FloatingObjectVerticalPositionType? VerticalPositionType { get { return verticalPositionType; } set { verticalPositionType = value; } }
		public FloatingObjectHorizontalPositionType? HorizontalPositionType { get { return horizontalPositionType; } set { horizontalPositionType = value; } }
		public bool? Locked { get { return locked; } set { locked = value; } }
		public FloatingObjectTextWrapType TextWrapType { get { return textWrapType; } set { textWrapType = value; } }
		public FloatingObjectTextWrapSide? TextWrapSide { get { return textWrapSide; } set { textWrapSide = value; } }
		public bool? IsBehindDocument { get { return isBehindDocument; } set { isBehindDocument = value; } }
		public int? TopDistance { get { return topDistance; } set { topDistance = value; } }
		public int? BottomDistance { get { return bottomDistance; } set { bottomDistance = value; } }
		public int? LeftDistance { get { return leftDistance; } set { leftDistance = value; } }
		public int? RightDistance { get { return rightDistance; } set { rightDistance = value; } }
		public Dictionary<FloatingObjectHorizontalPositionType, XtraRichEditStringId> HorizontalPositionTypeTable { get { return horizontalPositionTypeTable; } }
		public Dictionary<FloatingObjectHorizontalPositionAlignment, XtraRichEditStringId> HorizontalPositionAlignmentTable { get { return horizontalPositionAlignmentTable; } }
		public Dictionary<FloatingObjectVerticalPositionType, XtraRichEditStringId> VerticalPositionTypeTable { get { return verticalPositionTypeTable; } }
		public Dictionary<FloatingObjectVerticalPositionAlignment, XtraRichEditStringId> VerticalPositionAlignmentTable { get { return verticalPositionAlignmentTable; } }
		public int? OffsetX { get { return offsetX; } set { offsetX = value; } }
		public int? OffsetY { get { return offsetY; } set { offsetY = value; } }
		public bool IsHorizontalAbsolutePosition { get { return isHorizontalAbsolutePosition; } set { isHorizontalAbsolutePosition = value; } }
		public bool IsVerticalAbsolutePosition { get { return isVerticalAbsolutePosition; } set { isVerticalAbsolutePosition = value; } }
		public Size? OriginalSize { get { return ControllerParameters.OriginalSize; } }
		public int ActualHeight { get { return actualHeight; } set { actualHeight = value; } }
		public int ActualWidth { get { return actualWidth; } set { actualWidth = value; } }
		public bool LockAspectRatio { get { return lockAspectRatio; } set { lockAspectRatio = value; } }
		public int? Rotation { get { return rotation; } set { rotation = value; } }
		#endregion
		public FloatingObjectLayoutOptionsFormController(FloatingInlineObjectLayoutOptionsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
			this.floatingInlineObjectParameters = controllerParameters.FloatingInlineObjectParameters;
			this.uiUnitConverter = new UIUnitConverter(UnitPrecisionDictionary.DefaultPrecisions);
			InitializeController();
		}
		protected internal virtual void InitializeController() {
			horizontalPositionTypeTable = CreateHorizontalPositionTypeTable();
			horizontalPositionAlignmentTable = CreateHorizontalPositionAlignmentTable();
			verticalPositionTypeTable = CreateVerticalPositionTypeTable();
			verticalPositionAlignmentTable = CreateVerticalPositionAlignmentTable();
			InnerRichEditControl innerControl = Control.InnerControl;
			DocumentModelUnitConverter unitConverter = innerControl.DocumentModel.UnitConverter;
			floatingObjectRichTextIndentEditProperties = new FloatingObjectRichTextIndentEditProperties(innerControl.UIUnit, unitConverter);
			if (FloatingObjectAnchorRun == null) {
				HorizontalPositionAlignment = FloatingObjectHorizontalPositionAlignment.Center;
				HorizontalPositionType = FloatingObjectHorizontalPositionType.Column;
				VerticalPositionAlignment = FloatingObjectVerticalPositionAlignment.Top;
				VerticalPositionType = FloatingObjectVerticalPositionType.Paragraph;
				Locked = null;
				OffsetX = null;
				OffsetY = null;
				IsHorizontalAbsolutePosition = false;
				IsVerticalAbsolutePosition = false;
				IsBehindDocument = null;
				TextWrapType = FloatingObjectTextWrapType.Inline;
				TextWrapSide = null;
				TopDistance = null;
				BottomDistance = null;
				LeftDistance = null;
				RightDistance = null;
				LockAspectRatio = InlinePictureRun.LockAspectRatio;
				ActualHeight = InlinePictureRun.ActualSize.Height;
				ActualWidth = InlinePictureRun.ActualSize.Width;
				Rotation = null;
			}
			else {
				HorizontalPositionAlignment = FloatingObjectProperties.HorizontalPositionAlignment;
				VerticalPositionAlignment = FloatingObjectProperties.VerticalPositionAlignment;
				VerticalPositionType = FloatingObjectProperties.VerticalPositionType;
				HorizontalPositionType = FloatingObjectProperties.HorizontalPositionType;
				Locked = FloatingObjectProperties.Locked;
				OffsetX = FloatingObjectProperties.OffsetX;
				OffsetY = FloatingObjectProperties.OffsetY;
				IsHorizontalAbsolutePosition = (HorizontalPositionAlignment == FloatingObjectHorizontalPositionAlignment.None);
				IsVerticalAbsolutePosition = (VerticalPositionAlignment == FloatingObjectVerticalPositionAlignment.None);
				IsBehindDocument = FloatingObjectProperties.IsBehindDoc;
				TextWrapType = FloatingObjectProperties.TextWrapType;
				TextWrapSide = FloatingObjectProperties.TextWrapSide;
				TopDistance = FloatingObjectProperties.TopDistance;
				BottomDistance = FloatingObjectProperties.BottomDistance;
				LeftDistance = FloatingObjectProperties.LeftDistance;
				RightDistance = FloatingObjectProperties.RightDistance;
				LockAspectRatio = FloatingObjectProperties.LockAspectRatio;
				ActualHeight = FloatingObjectProperties.ActualHeight;
				ActualWidth = FloatingObjectProperties.ActualWidth;
				Rotation = FloatingInlineObjectParameters.FloatingObjectAnchorRun.Shape.Rotation;
			}
		}
		private void ApplyChangesInlinePicture() {
			InlinePictureRun run = floatingInlineObjectParameters.InlinePictureRun;
			Size newSize = run.ActualSize;
			newSize.Height = ActualHeight;
			newSize.Width = ActualWidth;
			if (!run.ActualSize.Equals(newSize))
				run.ActualSize = newSize;
			if (LockAspectRatio != run.LockAspectRatio)
				run.LockAspectRatio = LockAspectRatio;
		}
		private void SetPositionProperties(FloatingObjectProperties floatObjProp) {
			if (HorizontalPositionType.HasValue && (floatObjProp.HorizontalPositionType != HorizontalPositionType.Value))
				floatObjProp.HorizontalPositionType = HorizontalPositionType.Value;
			if (isHorizontalAbsolutePosition) {
				if (OffsetX.HasValue && (OffsetX != floatObjProp.OffsetX))
					floatObjProp.OffsetX = OffsetX.Value;
			}
			else {
				if (VerticalPositionAlignment.HasValue && (HorizontalPositionAlignment != floatObjProp.HorizontalPositionAlignment))
					floatObjProp.HorizontalPositionAlignment = HorizontalPositionAlignment.Value;
				floatObjProp.OffsetX = 0;
			}
			if (VerticalPositionType.HasValue && (floatObjProp.VerticalPositionType != VerticalPositionType.Value))
				floatObjProp.VerticalPositionType = VerticalPositionType.Value;
			if (isVerticalAbsolutePosition) {
				if (OffsetY.HasValue && (OffsetY != floatObjProp.OffsetY))
					floatObjProp.OffsetY = OffsetY.Value;
			}
			else {
				if (VerticalPositionAlignment.HasValue && (VerticalPositionAlignment != floatObjProp.VerticalPositionAlignment))
					floatObjProp.VerticalPositionAlignment = VerticalPositionAlignment.Value;
				floatObjProp.OffsetY = 0;
			}
			if (Locked.HasValue && (Locked != floatObjProp.Locked))
				floatObjProp.Locked = Locked.Value;
		}
		private void SetWrappingOptios(FloatingObjectProperties floatObjProp) {
			if (TextWrapType != floatObjProp.TextWrapType)
				floatObjProp.TextWrapType = TextWrapType;
			if (TextWrapSide.HasValue && (TextWrapSide != floatObjProp.TextWrapSide))
				floatObjProp.TextWrapSide = TextWrapSide.Value;
			if (TopDistance.HasValue && (TopDistance != floatObjProp.TopDistance))
				floatObjProp.TopDistance = TopDistance.Value;
			if (BottomDistance.HasValue && (BottomDistance != floatObjProp.BottomDistance))
				floatObjProp.BottomDistance = BottomDistance.Value;
			if (RightDistance.HasValue && (RightDistance != floatObjProp.RightDistance))
				floatObjProp.RightDistance = RightDistance.Value;
			if (LeftDistance.HasValue && (LeftDistance != floatObjProp.LeftDistance))
				floatObjProp.LeftDistance = LeftDistance.Value;
			if (IsBehindDocument.HasValue && (IsBehindDocument != floatObjProp.IsBehindDoc))
				floatObjProp.IsBehindDoc = IsBehindDocument.Value;
		}
		private void SetSizeOptions(FloatingObjectAnchorRun run, FloatingObjectProperties floatObjProp) {
			if (ActualHeight != floatObjProp.ActualHeight)
				floatObjProp.ActualHeight = ActualHeight;
			if (ActualWidth != floatObjProp.ActualWidth)
				floatObjProp.ActualWidth = ActualWidth;
			if (Rotation.HasValue && (Rotation != run.Shape.Rotation))
				run.Shape.Rotation = Rotation.Value;
			if (LockAspectRatio != floatObjProp.LockAspectRatio)
				floatObjProp.LockAspectRatio = LockAspectRatio;
		}
		private void ApplyChangesFloatingObject(FloatingObjectAnchorRun run) {
			FloatingObjectProperties floatObjProp = run.FloatingObjectProperties;
			floatObjProp.BeginUpdate();
			try {
				SetPositionProperties(floatObjProp);  
				SetWrappingOptios(floatObjProp);  
				SetSizeOptions(run, floatObjProp);  
			}
			finally {
				floatObjProp.EndUpdate();
			}
		}
		private FloatingObjectAnchorRun SubsituteInlineToFloatingObjectRun() {
			PieceTable pieceTable = FloatingInlineObjectParameters.InlinePictureRun.PieceTable;
			DocumentLogPosition logPosition = DocumentModel.Selection.Interval.NormalizedStart.LogPosition;
			ParagraphIndex paragraphIndex = FloatingInlineObjectParameters.InlinePictureRun.Paragraph.Index;
			FloatingObjectAnchorRun newRun = pieceTable.InsertFloatingObjectAnchorCore(paragraphIndex, logPosition);
			PictureFloatingObjectContent newContent = new PictureFloatingObjectContent(newRun, FloatingInlineObjectParameters.InlinePictureRun.Image);
			newRun.SetContent(newContent);
			pieceTable.DeleteContent(logPosition + 1, 1, false);
			return newRun;
		}
		public override void ApplyChanges() {
			DocumentModel.BeginUpdate();
			if ((TextWrapType == FloatingObjectTextWrapType.Inline) && (!Rotation.HasValue || (Rotation == 0)))
			   ApplyChangesInlinePicture();
			else {
				if (floatingInlineObjectParameters.FloatingObjectAnchorRun == null) {
					FloatingObjectAnchorRun run = SubsituteInlineToFloatingObjectRun();
					floatingInlineObjectParameters.FloatingObjectAnchorRun = run;
					ApplyChangesFloatingObject(run);
				}
				else {
					ApplyChangesFloatingObject(FloatingInlineObjectParameters.FloatingObjectAnchorRun);
				}
			}
			DocumentModel.EndUpdate();
		}
		public void ApplyPreset(TextWrapTypeInfoPreset preset) {
			TextWrapType = preset.TextWrapType;
			IsBehindDocument = preset.IsBehindDocument;
		}
		Dictionary<FloatingObjectHorizontalPositionAlignment, XtraRichEditStringId> CreateHorizontalPositionAlignmentTable() {
			Dictionary<FloatingObjectHorizontalPositionAlignment, XtraRichEditStringId> table = new Dictionary<FloatingObjectHorizontalPositionAlignment, XtraRichEditStringId>();
			table.Add(FloatingObjectHorizontalPositionAlignment.Left, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentLeft);
			table.Add(FloatingObjectHorizontalPositionAlignment.Center, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentCenter);
			table.Add(FloatingObjectHorizontalPositionAlignment.Right, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionAlignmentRight);
			return table;
		}
		Dictionary<FloatingObjectVerticalPositionType, XtraRichEditStringId> CreateVerticalPositionTypeTable() {
			Dictionary<FloatingObjectVerticalPositionType, XtraRichEditStringId> table = new Dictionary<FloatingObjectVerticalPositionType, XtraRichEditStringId>();
			table.Add(FloatingObjectVerticalPositionType.Margin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeMargin);
			table.Add(FloatingObjectVerticalPositionType.Page, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypePage);
			table.Add(FloatingObjectVerticalPositionType.Line,XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeLine);
			table.Add(FloatingObjectVerticalPositionType.TopMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeTopMargin);
			table.Add(FloatingObjectVerticalPositionType.BottomMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeBottomMargin);
			table.Add(FloatingObjectVerticalPositionType.InsideMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeInsideMargin);
			table.Add(FloatingObjectVerticalPositionType.OutsideMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeOutsideMargin);
			table.Add(FloatingObjectVerticalPositionType.Paragraph, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionTypeParagraph);
			return table;
		}
		Dictionary<FloatingObjectVerticalPositionAlignment, XtraRichEditStringId> CreateVerticalPositionAlignmentTable() {
			Dictionary<FloatingObjectVerticalPositionAlignment, XtraRichEditStringId> table = new Dictionary<FloatingObjectVerticalPositionAlignment, XtraRichEditStringId>();
			table.Add(FloatingObjectVerticalPositionAlignment.Top, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentTop);
			table.Add(FloatingObjectVerticalPositionAlignment.Center, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentCenter);
			table.Add(FloatingObjectVerticalPositionAlignment.Bottom, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentBottom);
			table.Add(FloatingObjectVerticalPositionAlignment.Inside, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentInside);
			table.Add(FloatingObjectVerticalPositionAlignment.Outside, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_VerticalPositionAlignmentOutside);
			return table;
		}
		Dictionary<FloatingObjectHorizontalPositionType, XtraRichEditStringId> CreateHorizontalPositionTypeTable() {
			Dictionary<FloatingObjectHorizontalPositionType, XtraRichEditStringId> table = new Dictionary<FloatingObjectHorizontalPositionType, XtraRichEditStringId>();
			table.Add(FloatingObjectHorizontalPositionType.Margin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeMargin);
			table.Add(FloatingObjectHorizontalPositionType.Page, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypePage);
			table.Add(FloatingObjectHorizontalPositionType.Column, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeColumn);
			table.Add(FloatingObjectHorizontalPositionType.Character, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeCharacter);
			table.Add(FloatingObjectHorizontalPositionType.LeftMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeLeftMargin);
			table.Add(FloatingObjectHorizontalPositionType.RightMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeRightMargin);
			table.Add(FloatingObjectHorizontalPositionType.InsideMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeInsideMargin);
			table.Add(FloatingObjectHorizontalPositionType.OutsideMargin, XtraRichEditStringId.FloatingObjectLayoutOptionsForm_HorizontalPositionTypeOutsideMargin);
			return table;
		}
		public void ResetActualHeight() {
			ActualHeight = (OriginalSize.HasValue ? OriginalSize.Value.Height : 0);
		}
		public void ResetActualWidth() {
			ActualWidth = (OriginalSize.HasValue ? OriginalSize.Value.Width : 0);
		}
		public void ResetRotation() {
			Rotation = (Rotation.HasValue ? (int?)0 : null);
		}
		public void RecalculateSizeDependingOnHeight(bool lockAspectRatio, int newHeight) {
			ActualHeight = newHeight;
			if (lockAspectRatio) {
				if (OriginalSize.HasValue && OriginalSize.Value.Height != 0)
					ActualWidth = (int)((double)OriginalSize.Value.Width / (double)OriginalSize.Value.Height * (double)ActualHeight);
				else
					ActualWidth = 0;
			}
		}
		public void RecalculateSizeDependingOnWidth(bool lockAspectRatio, int newWidth) {
			ActualWidth = newWidth;
			if (lockAspectRatio) {
				if (OriginalSize.HasValue && OriginalSize.Value.Width != 0)
					ActualHeight = (int)((double)OriginalSize.Value.Height / (double)OriginalSize.Value.Width * (double)ActualWidth);
				else
					ActualHeight = 0;
			}
		}
		public int StringToTwips(String s) {
			if (String.IsNullOrEmpty(s))
				return 0;
			UIUnit unit = UIUnit.Create(s, UIType, UnitPrecisionDictionary.DefaultPrecisions, false);
			return uiUnitConverter.ToTwipsUnit(unit, false);
		}
		public String OriginalHeightAsString() {
			int height = OriginalSize.HasValue ? OriginalSize.Value.Height : 0;
			UIUnit unit = uiUnitConverter.ToUIUnitF(height, UIType, false);
			return unit.ToString();
		}
		public String OriginalWidthAsString() {
			int width = OriginalSize.HasValue ? OriginalSize.Value.Width : 0;
			UIUnit unit = uiUnitConverter.ToUIUnitF(width, UIType, false);
			return unit.ToString();
		}
	}
	public abstract class TextWrapTypeInfoPreset {
		protected internal virtual string ImageName { get { return String.Empty; } }
		public virtual Image Image { get { return !string.IsNullOrEmpty(ImageName) ? LoadImage() : null; } }
		public virtual Stream ImageStream { get { return !string.IsNullOrEmpty(ImageName) ? GetType().GetAssembly().GetManifestResourceStream(ImageResourceUri) : null; } }
		protected internal string ImageResourceUri { get { return "DevExpress.XtraRichEdit.Images." + ImageName + "_32x32.png"; } }
		public abstract FloatingObjectTextWrapType TextWrapType { get; }
		public virtual bool IsBehindDocument { get { return false; } }
		protected internal virtual Image LoadImage() {
			return CommandResourceImageLoader.CreateBitmapFromResources(ImageResourceUri, GetType().GetAssembly());
		}
	}
	public class FloatingObjectSquareTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Square; } }
		protected internal override string ImageName { get { return "TextWrapSquare"; } }
	}
	public class FloatingObjectTightTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Tight; } }
		protected internal override string ImageName { get { return "TextWrapTight"; } }
	}
	public class FloatingObjectThroughTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.Through; } }
		protected internal override string ImageName { get { return "TextWrapThrough"; } }
	}
	public class FloatingObjectTopAndBottomTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.TopAndBottom; } }
		protected internal override string ImageName { get { return "TextWrapTopAndBottom"; } }
	}
	public class FloatingObjectBehindTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.None; } }
		public override bool IsBehindDocument { get { return true; } }
		protected internal override string ImageName { get { return "TextWrapBehind"; } }
	}
	public class FloatingObjectInFrontOfTextWrapTypePreset : TextWrapTypeInfoPreset {
		public override FloatingObjectTextWrapType TextWrapType { get { return FloatingObjectTextWrapType.None; } }
		protected internal override string ImageName { get { return "TextWrapInFrontOfText"; } }
	}
	public class FloatingObjectRichTextIndentEditProperties {
#region Fields
		int maxValue;
		DocumentUnit defaultUnitType;
		DocumentModelUnitConverter valueUnitConverter;
#endregion
		public FloatingObjectRichTextIndentEditProperties(DocumentUnit defaultUnitType, DocumentModelUnitConverter valueUnitConverter) {
			this.maxValue = valueUnitConverter.TwipsToModelUnits(1440 * 22);
			this.defaultUnitType = defaultUnitType;
			this.valueUnitConverter = valueUnitConverter;
		}
#region Properties
		public int MaxValue { get { return maxValue; } }
		public DocumentUnit DefaultUnitType { get { return defaultUnitType;} }
		public DocumentModelUnitConverter ValueUnitConverter { get { return valueUnitConverter; } }
#endregion
		public int GetMinValue(bool allowNegativeValues) {
			return allowNegativeValues ? - valueUnitConverter.TwipsToModelUnits(1440 * 22) : 0;
		}
	}
}
