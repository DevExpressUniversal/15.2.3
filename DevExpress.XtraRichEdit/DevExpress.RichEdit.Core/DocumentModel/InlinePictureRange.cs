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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Export.Rtf;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraPrinting;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Utils.Internal;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing.Imaging;
using System.Diagnostics;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Model {
	public interface IPictureContainerRun {
		TextRunBase Run { get; }
		Size ActualSize { get; set; }
		SizeF ActualSizeF { get; }
		float ScaleX { get; }
		float ScaleY { get; }
		DocumentModelChangeActions GetBatchUpdateChangeActions();
		DocumentModel DocumentModel { get; }
		PictureFloatingObjectContent PictureContent { get; }
		void SetActualSizeInternal(Size actualSize);
	}
	public interface IOpenXMLDrawingObject {
		bool IsFloatingObject { get; } 
		string Name { get; }
		Size ActualSize { get; }
		int Rotation { get; }
		bool LockAspectRatio { get; }
		bool AllowOverlap { get; }
		bool IsBehindDoc { get; }
		bool LayoutInTableCell { get; }
		bool Locked { get; }
		int ZOrder { get; }
		bool UseBottomDistance { get; }
		int BottomDistance { get; }
		bool UseLeftDistance { get; }
		int LeftDistance { get; }
		bool UseRightDistance { get; }
		int RightDistance { get; }
		bool UseTopDistance { get; }
		int TopDistance { get; }
		bool UseHidden { get; }
		bool Hidden { get; }
		FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment { get; }
		FloatingObjectHorizontalPositionType HorizontalPositionType { get; }
		bool UsePercentOffset { get; }
		int PercentOffsetX { get; }
		FloatingObjectVerticalPositionAlignment VerticalPositionAlignment { get; }
		FloatingObjectVerticalPositionType VerticalPositionType { get; }
		int PercentOffsetY { get; }
		bool UseLockAspectRatio { get; }
		bool UseRotation { get; }
		FloatingObjectTextWrapType TextWrapType { get; }
		FloatingObjectTextWrapSide TextWrapSide { get; }
		Shape Shape { get; }
		bool UseRelativeWidth { get; }
		bool UseRelativeHeight { get; }
		FloatingObjectRelativeWidth RelativeWidth { get; }
		FloatingObjectRelativeHeight RelativeHeight { get; }
		Point Offset { get; }
		Point PercentOffset { get; }
	}
	public interface IOpenXMLDrawingObjectContent {
		OfficeImage Image { get; }
	}
	#region InlinePictureRun 
	public class InlinePictureRun : InlineObjectRunBase<InlinePictureInfo>, IDisposable, IInlinePicturePropertiesContainer, IPictureContainerRun, IInlineObjectRun, IHighlightableTextRun {
		#region static
		public static bool ShouldExportInlinePictureRunCharacterProperties(CharacterFormattingInfo info, CharacterFormattingOptions options) {
			return (options.UseHidden && info.Hidden) ||
				(options.UseFontUnderlineType && info.FontUnderlineType != UnderlineType.None) ||
				(options.UseFontStrikeoutType && info.FontStrikeoutType != StrikeoutType.None) || (options.UseBackColor && !DXColor.IsTransparentOrEmpty(info.BackColor));
		}
		#endregion
		PictureFloatingObjectContent imageContent;
		public InlinePictureRun(Paragraph paragraph, OfficeImage image)
			: base(paragraph, new Size(1, 1)) {
			this.imageContent = new PictureFloatingObjectContent(this, image);
			PictureProperties.ObtainAffectedRange += OnCharacterPropertiesObtainAffectedRange;
		}
		#region Properties
		protected internal new InlinePictureProperties Properties { get { return (InlinePictureProperties)base.Properties; } }
		public override bool CanPlaceCaretBefore { get { return true; } }
		public OfficeImage Image { get { return imageContent.Image; } }
		protected internal InlinePictureProperties PictureProperties { get { return (InlinePictureProperties)Properties; } }
		public override Size OriginalSize { get { return imageContent.OriginalSize; } }
		public override Size ActualSize {
			get { return base.ActualSize; }
			set {
				base.ActualSize = value;
				if (!Image.IsLoaded) {
					imageContent.Image.DesiredSizeAfterLoad = value;
					imageContent.Image.ShouldSetDesiredSizeAfterLoad = true;
				}
			}
		}
		public override float ScaleX {
			get {
				return base.ScaleX;
			}
			set {
				base.ScaleX = value;
				if(!Image.IsLoaded) {
					imageContent.Image.ShouldSetDesiredSizeAfterLoad = false;
				}
			}
		}
		public override float ScaleY {
			get {
				return base.ScaleY;
			}
			set {
				base.ScaleY = value;
				if (!Image.IsLoaded) {
					imageContent.Image.ShouldSetDesiredSizeAfterLoad = false;
				}
			}
		}
		#region Sizing
		public ImageSizeMode Sizing {
			get { return Properties.Sizing; }
			set {
				if (Sizing == value)
					return;
				Properties.Sizing = value;
			}
		}
		#endregion
		#region ResizingShadowDisplayMode
		public ResizingShadowDisplayMode ResizingShadowDisplayMode {
			get { return Properties.ResizingShadowDisplayMode; }
			set {
				if (ResizingShadowDisplayMode == value)
					return;
				Properties.ResizingShadowDisplayMode = value;
			}
		}
		#endregion
		#region LockAspectRatio
		public bool LockAspectRatio {
			get { return Properties.LockAspectRatio; }
			set {
				if(LockAspectRatio == value) return;
				Properties.LockAspectRatio = value;
			}
		}
		#endregion
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (imageContent != null) {
					imageContent.Dispose();
					imageContent = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		protected internal override void SetOriginalSize(Size size) {
			if (imageContent != null)
				imageContent.SetOriginalSize(size);
		}
		protected internal override void CopyOriginalSize(Size originalSize) {
			imageContent.SetOriginalSize(originalSize);
		}
		protected internal override void BeforeRunRemoved() {
			base.BeforeRunRemoved();
			this.imageContent.BeforeRunRemoved();
		}
		protected internal override void AfterRunInserted() {
			base.AfterRunInserted();
			this.imageContent.AfterRunInserted();
		}
		protected internal override InlineObjectProperties<InlinePictureInfo> CreateProperties(PieceTable pieceTable) {
			return new InlinePictureProperties(this);
		}
		#region IInlinePicturePropertiesContainer Members
		IndexChangedHistoryItemCore<DocumentModelChangeActions> IInlinePicturePropertiesContainer.CreateInlinePicturePropertiesChangedHistoryItem() {			
			return new RunInlinePicturePropertiesChangedHistoryItem(Paragraph.PieceTable, GetRunIndex());
		}
		PieceTable IInlinePicturePropertiesContainer.PieceTable { get { return PieceTable; } }
		void IInlinePicturePropertiesContainer.OnInlineCharacterPropertiesChanged() {
		}
		#endregion
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			boxInfo.Size = measurer.MeasureInlinePicture(this);
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			return false;
		}
		public override void Export(IDocumentModelExporter exporter) {
			InternalOfficeImageHelper.EnsureLoadComplete(Image);
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable targetPieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			Debug.Assert(targetPosition.PieceTable == targetPieceTable);
			Debug.Assert(targetPosition.RunOffset == 0);
			DocumentModel targetModel = copyManager.TargetModel;
			if (!targetModel.DocumentCapabilities.InlinePicturesAllowed) {
				targetPieceTable.InsertText(targetPosition.LogPosition, " ");
				return targetPieceTable.Runs[targetPosition.RunIndex];
			}
			InsertInlinePicture(targetPieceTable, targetPosition.LogPosition);
			InlinePictureRun run = (InlinePictureRun)targetPieceTable.Runs[targetPosition.RunIndex];
			run.PictureProperties.CopyFrom(this.PictureProperties.Info);
			DocumentCapabilitiesOptions options = targetModel.DocumentCapabilities;
			if (options.CharacterFormattingAllowed)
				run.CharacterProperties.CopyFrom(this.CharacterProperties.Info);
			if (options.CharacterStyleAllowed)
				run.CharacterStyleIndex = CharacterStyle.Copy(targetModel);
			run.CopyOriginalSize(this.OriginalSize);
			return run;
		}
		protected virtual void InsertInlinePicture(PieceTable targetPieceTable, DocumentLogPosition logPosition) {
			targetPieceTable.InsertInlinePicture(logPosition, Image.Clone(targetPieceTable.DocumentModel));
		}
		#region IPictureContainerRun Members
		TextRunBase IPictureContainerRun.Run { get { return this; } }
		Size IPictureContainerRun.ActualSize { get { return this.ActualSize; } set { this.ActualSize = value; } }
		DocumentModelChangeActions IPictureContainerRun.GetBatchUpdateChangeActions() {
			return PictureProperties.GetBatchUpdateChangeActions();
		}
		#endregion
		#region IPictureContainerRun Members
		public PictureFloatingObjectContent PictureContent {
			get { return imageContent as PictureFloatingObjectContent; }
		}
		#endregion
		#region IPictureContainerRun Members
		DocumentModel IPictureContainerRun.DocumentModel {
			get { return this.DocumentModel; }
		}
		#endregion
		#region IInlineObjectRun Members
		public Size MeasureRun(IObjectMeasurer measurer) {
			return measurer.MeasureInlinePicture(this);
		}
		public virtual Box CreateBox() {
			return new InlinePictureBox();
		}
		#endregion
	}
	#endregion
	public class InlineDrawingObject : IOpenXMLDrawingObject {
		InlinePictureRun run;
		bool isFloatingObject = false;
		public InlineDrawingObject(InlinePictureRun run) {
			this.run = run;
		}
		public bool IsFloatingObject {
			get { return isFloatingObject; }
		}
		public string Name {
			get { return String.Empty; }
		}
		public Size ActualSize {
			get { return run.ActualSize; }
		}
		public int Rotation {
			get { return 0; }
		}
		public bool LockAspectRatio {
			get { return run.LockAspectRatio; }
		}
		public bool AllowOverlap {
			get { return false; }
		}
		public bool IsBehindDoc {
			get { return false; }
		}
		public bool LayoutInTableCell {
			get { return false; }
		}
		public bool Locked {
			get { return false; }
		}
		public int ZOrder {
			get { return 0; }
		}
		public bool UseBottomDistance {
			get { return false; }
		}
		public int BottomDistance {
			get { return 0; }
		}
		public bool UseLeftDistance {
			get { return false; }
		}
		public int LeftDistance {
			get { return 0; }
		}
		public bool UseRightDistance {
			get { return false; }
		}
		public int RightDistance {
			get { return 0; }
		}
		public bool UseTopDistance {
			get { return false; }
		}
		public int TopDistance {
			get { return 0; }
		}
		public bool UseHidden {
			get { return false; }
		}
		public bool Hidden {
			get { return false; }
		}
		public FloatingObjectHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return FloatingObjectHorizontalPositionAlignment.None; }
		}
		public FloatingObjectHorizontalPositionType HorizontalPositionType {
			get { return FloatingObjectHorizontalPositionType.Page; }
		}
		public bool UsePercentOffset {
			get { return false; }
		}
		public int PercentOffsetX {
			get { return 0; }
		}
		public FloatingObjectVerticalPositionAlignment VerticalPositionAlignment {
			get { return FloatingObjectVerticalPositionAlignment.None; }
		}
		public FloatingObjectVerticalPositionType VerticalPositionType {
			get { return FloatingObjectVerticalPositionType.Page; }
		}
		public int PercentOffsetY {
			get { return 0; }
		}
		public bool UseLockAspectRatio {
			get { return false; }
		}
		public bool UseRotation {
			get { return false; }
		}
		public FloatingObjectTextWrapType TextWrapType {
			get { return FloatingObjectTextWrapType.None; }
		}
		public FloatingObjectTextWrapSide TextWrapSide {
			get { return FloatingObjectTextWrapSide.Left; }
		}
		public Shape Shape {
			get { return null; }
		}
		public bool UseRelativeWidth {
			get { return false; }
		}
		public bool UseRelativeHeight {
			get { return false; }
		}
		public FloatingObjectRelativeWidth RelativeWidth {
			get { return new FloatingObjectRelativeWidth(); }
		}
		public FloatingObjectRelativeHeight RelativeHeight {
			get { return new FloatingObjectRelativeHeight(); }
		}
		public Point Offset {
			get { return new Point(0, 0); }
		}
		public Point PercentOffset {
			get { return new Point(0, 0); }
		}
	}
	public class InlineDrawingObjectContent : IOpenXMLDrawingObjectContent {
		OfficeImage image;
		public InlineDrawingObjectContent(OfficeImage image) {
			this.image = image;
		}
		public OfficeImage Image {
			get { return image; }
		}
	}
}
