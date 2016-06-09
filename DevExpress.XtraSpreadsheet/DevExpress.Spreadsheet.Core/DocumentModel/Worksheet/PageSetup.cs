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
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing.Printing;
using DevExpress.Office.Utils;
#if !SL
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region MarginsInfo
	public class MarginsInfo : ICloneable<MarginsInfo>, ISupportsCopyFrom<MarginsInfo>, ISupportsSizeOf {
		int left;
		int top;
		int right;
		int bottom;
		int header;
		int footer;
		public int Left { get { return left; } set { left = value; } }
		public int Top { get { return top; } set { top = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		public int Header { get { return header; } set { header = value; } }
		public int Footer { get { return footer; } set { footer = value; } }
		#region ICloneable<MarginsInfo> Members
		public MarginsInfo Clone() {
			MarginsInfo clone = new MarginsInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<MarginsInfo> Members
		public void CopyFrom(MarginsInfo value) {
			this.Left = value.Left;
			this.Top = value.Top;
			this.Right = value.Right;
			this.Bottom = value.Bottom;
			this.Header = value.Header;
			this.Footer = value.Footer;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			MarginsInfo info = obj as MarginsInfo;
			if (info == null)
				return false;
			return info.Left == this.Left &&
				info.Top == this.Top &&
				info.Right == this.Right &&
				info.Bottom == this.Bottom &&
				info.Header == this.Header &&
				info.Footer == this.Footer;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(Top, Left, Right, Bottom, Header, Footer);
		}
	}
	#endregion
	#region MarginsInfoCache
	public class MarginsInfoCache : UniqueItemsCache<MarginsInfo> {
		public MarginsInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override MarginsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			MarginsInfo info = new MarginsInfo();
			info.Left = unitConverter.TwipsToModelUnits(1008);
			info.Right = info.Left;
			info.Top = unitConverter.TwipsToModelUnits(1080);
			info.Bottom = info.Top;
			info.Header = unitConverter.TwipsToModelUnits(432);
			info.Footer = info.Header;
			return info;
		}
	}
	#endregion
	#region Margins
	public class Margins : SpreadsheetUndoableIndexBasedObject<MarginsInfo> {
		public Margins(IDocumentModelPartWithApplyChanges documentModelPart)
			: base(documentModelPart) {
		}
		#region Properties
		#region Left
		public int Left {
			get { return Info.Left; }
			set {
				if (Left == value)
					return;
				SetPropertyValue(SetLeftCore, value);
			}
		}
		DocumentModelChangeActions SetLeftCore(MarginsInfo info, int value) {
			info.Left = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Top
		public int Top {
			get { return Info.Top; }
			set {
				if (Top == value)
					return;
				SetPropertyValue(SetTopCore, value);
			}
		}
		DocumentModelChangeActions SetTopCore(MarginsInfo info, int value) {
			info.Top = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Right
		public int Right {
			get { return Info.Right; }
			set {
				if (Right == value)
					return;
				SetPropertyValue(SetRightCore, value);
			}
		}
		DocumentModelChangeActions SetRightCore(MarginsInfo info, int value) {
			info.Right = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Bottom
		public int Bottom {
			get { return Info.Bottom; }
			set {
				if (Bottom == value)
					return;
				SetPropertyValue(SetBottomCore, value);
			}
		}
		DocumentModelChangeActions SetBottomCore(MarginsInfo info, int value) {
			info.Bottom = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Header
		public int Header {
			get { return Info.Header; }
			set {
				if (Header == value)
					return;
				SetPropertyValue(SetHeaderCore, value);
			}
		}
		DocumentModelChangeActions SetHeaderCore(MarginsInfo info, int value) {
			info.Header = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Footer
		public int Footer {
			get { return Info.Footer; }
			set {
				if (Footer == value)
					return;
				SetPropertyValue(SetFooterCore, value);
			}
		}
		DocumentModelChangeActions SetFooterCore(MarginsInfo info, int value) {
			info.Footer = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<MarginsInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.MarginInfoCache;
		}
		public bool IsDefault() {
			return Index == 0;
		}
	}
	#endregion
	#region ModelCommentsPrintMode
	public enum ModelCommentsPrintMode { 
		AsDisplayed = 0,
		AtEnd = 1,
		None = 2,
	}
	#endregion
	#region ModelErrorsPrintMode
	public enum ModelErrorsPrintMode { 
		Displayed = 0,
		Blank = 1,
		Dash = 2,
		NA = 3,
	}
	#endregion
	#region PagePrintOrder
	public enum PagePrintOrder { 
		DownThenOver = 0,
		OverThenDown = 1,
	}
	#endregion
	#region ModelPageOrientation
	public enum ModelPageOrientation { 
		Default,
		Portrait,
		Landscape,
	}
	#endregion
	#region PrintSetupInfo
	public class PrintSetupInfo : ICloneable<PrintSetupInfo>, ISupportsCopyFrom<PrintSetupInfo>, ISupportsSizeOf {
		#region Fields
		const uint MaskPaperKind = 0x000001FF; 
		const uint MaskCommentsPrintMode = 0x00000600; 
		const uint MaskErrorsPrintMode = 0x00001800; 
		const uint MaskPagePrintOrder = 0x00006000; 
		const uint MaskPageOrientation = 0x00018000; 
		const uint MaskScale = 0x03FE0000; 
		const uint MaskBlackAndWhite = 0x04000000; 
		const uint MaskDraft = 0x08000000; 
		const uint MaskUseFirstPageNumber = 0x10000000; 
		const uint MaskUsePrinterDefaults = 0x20000000; 
		const uint MaskAutoPageBreaks = 0x40000000; 
		const uint MaskFitToPage = 0x80000000; 
		const uint MaskHorizontalDpi	  = 0x00001FFF;
		const uint MaskVerticalDpi		= 0x07FFE000;
		const uint MaskHorizontalCentered = 0x08000000;
		const uint MaskVerticalCentered   = 0x10000000;
		const uint MaskHeadings		   = 0x20000000;
		const uint MaskGridLines		  = 0x40000000;
		const uint MaskGridLinesSet	   = 0x80000000;
		uint packedValues;
		uint packedValues2;
		int copies;
		int firstPageNumber;
		int fitToWidth; 
		int fitToHeight; 
		#endregion
		#region Properties
		#region PaperKind // maxValue=118 => may reserve 9bit: 0-511
		public PaperKind PaperKind {
			get { return (PaperKind)(packedValues & MaskPaperKind); }
			set {
				packedValues &= ~MaskPaperKind;
				packedValues |= (uint)value & MaskPaperKind;
			}
		}
		#endregion
		#region ModelCommentsPrintMode
		public ModelCommentsPrintMode CommentsPrintMode {
			get { return (ModelCommentsPrintMode)((packedValues & MaskCommentsPrintMode) >> 9); }
			set {
				packedValues &= ~MaskCommentsPrintMode;
				packedValues |= ((uint)value << 9) & MaskCommentsPrintMode;
			}
		}
		#endregion
		#region ModelErrorsPrintMode
		public ModelErrorsPrintMode ErrorsPrintMode {
			get { return (ModelErrorsPrintMode)((packedValues & MaskErrorsPrintMode) >> 11); }
			set {
				packedValues &= ~MaskErrorsPrintMode;
				packedValues |= ((uint)value << 11) & MaskErrorsPrintMode;
			}
		}
		#endregion
		#region PagePrintOrder
		public PagePrintOrder PagePrintOrder {
			get { return (PagePrintOrder)((packedValues & MaskPagePrintOrder) >> 13); }
			set {
				packedValues &= ~MaskPagePrintOrder;
				packedValues |= ((uint)value << 13) & MaskPagePrintOrder;
			}
		}
		#endregion
		#region ModelPageOrientation
		public ModelPageOrientation Orientation {
			get { return (ModelPageOrientation)((packedValues & MaskPageOrientation) >> 15); }
			set {
				packedValues &= ~MaskPageOrientation;
				packedValues |= ((uint)value << 15) & MaskPageOrientation;
			}
		}
		#endregion
		#region Scale // 10-400: 9bit
		public int Scale {
			get { return (int)((packedValues & MaskScale) >> 17); }
			set {
				packedValues &= ~MaskScale;
				packedValues |= ((uint)value << 17) & MaskScale;
			}
		}
		#endregion
		public bool BlackAndWhite { get { return GetBooleanValue(MaskBlackAndWhite); } set { SetBooleanValue(MaskBlackAndWhite, value); } }
		public bool Draft { get { return GetBooleanValue(MaskDraft); } set { SetBooleanValue(MaskDraft, value); } }
		public bool UseFirstPageNumber { get { return GetBooleanValue(MaskUseFirstPageNumber); } set { SetBooleanValue(MaskUseFirstPageNumber, value); } }
		public bool UsePrinterDefaults { get { return GetBooleanValue(MaskUsePrinterDefaults); } set { SetBooleanValue(MaskUsePrinterDefaults, value); } }
		public bool AutoPageBreaks { get { return GetBooleanValue(MaskAutoPageBreaks); } set { SetBooleanValue(MaskAutoPageBreaks, value); } }
		public bool FitToPage { get { return GetBooleanValue(MaskFitToPage); } set { SetBooleanValue(MaskFitToPage, value); } }
		public int Copies { get { return copies; } set { copies = value; } }
		public int FirstPageNumber { get { return firstPageNumber; } set { firstPageNumber = value; } }
		public int FitToWidth { get { return fitToWidth; } set { fitToWidth = value; } }
		public int FitToHeight { get { return fitToHeight; } set { fitToHeight = value; } }
		#region HorizontalDpi // 0 <= HorizontalDpi <= (2<<13)-1
		public int HorizontalDpi {
			get { return (int)(packedValues2 & MaskHorizontalDpi); }
			set {
				packedValues2 &= ~MaskHorizontalDpi;
				packedValues2 |= (uint)value & MaskHorizontalDpi;
			}
		}
		#endregion
		#region VerticalDpi // 0 <= VerticalDpi <= (2<<13)-1
		public int VerticalDpi {
			get { return (int)((packedValues2 & MaskVerticalDpi) >> 13); }
			set {
				packedValues2 &= ~MaskVerticalDpi;
				packedValues2 |= ((uint)(value << 13)) & MaskVerticalDpi;
			}
		}
		#endregion
		public bool CenterHorizontally { get { return GetBooleanValue2(MaskHorizontalCentered); } set { SetBooleanValue2(MaskHorizontalCentered, value); } }
		public bool CenterVertically { get { return GetBooleanValue2(MaskVerticalCentered); } set { SetBooleanValue2(MaskVerticalCentered, value); } }
		public bool PrintHeadings { get { return GetBooleanValue2(MaskHeadings); } set { SetBooleanValue2(MaskHeadings, value); } }
		public bool PrintGridLines { get { return GetBooleanValue2(MaskGridLines); } set { SetBooleanValue2(MaskGridLines, value); } }
		public bool PrintGridLinesSet { get { return GetBooleanValue2(MaskGridLinesSet); } set { SetBooleanValue2(MaskGridLinesSet, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region GetBooleanValue2/SetBooleanValue2 helpers
		void SetBooleanValue2(uint mask, bool bitVal) {
			if (bitVal)
				packedValues2 |= mask;
			else
				packedValues2 &= ~mask;
		}
		bool GetBooleanValue2(uint mask) {
			return (packedValues2 & mask) != 0;
		}
		#endregion
		#region ICloneable<MarginsInfo> Members
		public PrintSetupInfo Clone() {
			PrintSetupInfo clone = new PrintSetupInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<PrintSetupInfo> Members
		public void CopyFrom(PrintSetupInfo value) {
			this.packedValues = value.packedValues;
			this.packedValues2 = value.packedValues2;
			this.Copies = value.Copies;
			this.FirstPageNumber = value.FirstPageNumber;
			this.FitToWidth = value.FitToWidth;
			this.FitToHeight = value.FitToHeight;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			PrintSetupInfo info = obj as PrintSetupInfo;
			if (info == null)
				return false;
			return info.packedValues == this.packedValues &&
				info.packedValues2 == this.packedValues2 &&
				info.Copies == this.Copies &&
				info.FirstPageNumber == this.FirstPageNumber &&
				info.FitToWidth == this.FitToWidth &&
				info.FitToHeight == this.FitToHeight;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)packedValues, (int)packedValues2, Copies, FirstPageNumber, FitToWidth, FitToHeight);
		}
	}
	#endregion
	#region PrintSetupInfoCache
	public class PrintSetupInfoCache : UniqueItemsCache<PrintSetupInfo> {
		public PrintSetupInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PrintSetupInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			PrintSetupInfo info = new PrintSetupInfo();
			info.AutoPageBreaks = true;
			info.HorizontalDpi = 600;
			info.VerticalDpi = 600;
			info.PaperKind = PaperKind.Letter;
			info.Scale = 100;
			info.FirstPageNumber = 1;
			info.FitToWidth = 1;
			info.FitToHeight = 1;
			info.Copies = 1;
			info.Orientation = ModelPageOrientation.Default;
			info.CommentsPrintMode = ModelCommentsPrintMode.None;
			info.PrintGridLinesSet = true;
			info.UsePrinterDefaults = true;
			return info;
		}
	}
	#endregion
	#region PrintSetup
	public class PrintSetup : SpreadsheetUndoableIndexBasedObject<PrintSetupInfo> {
		public PrintSetup(IDocumentModelPartWithApplyChanges part)
			: base(part) {
		}
		#region Properties
		public Worksheet Sheet { get { return DocumentModelPart as Worksheet; } }
		#region PaperKind
		public PaperKind PaperKind {
			get { return Info.PaperKind; }
			set {
				if (PaperKind == value)
					return;
				SetPropertyValue(SetPaperKindCore, value);
			}
		}
		DocumentModelChangeActions SetPaperKindCore(PrintSetupInfo info, PaperKind value) {
			info.PaperKind = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ModelCommentsPrintMode
		public ModelCommentsPrintMode CommentsPrintMode {
			get { return Info.CommentsPrintMode; }
			set {
				if (CommentsPrintMode == value)
					return;
				SetPropertyValue(SetCommentsPrintModeCore, value);
			}
		}
		DocumentModelChangeActions SetCommentsPrintModeCore(PrintSetupInfo info, ModelCommentsPrintMode value) {
			info.CommentsPrintMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ModelErrorsPrintMode
		public ModelErrorsPrintMode ErrorsPrintMode {
			get { return Info.ErrorsPrintMode; }
			set {
				if (ErrorsPrintMode == value)
					return;
				SetPropertyValue(SetErrorsPrintModeCore, value);
			}
		}
		DocumentModelChangeActions SetErrorsPrintModeCore(PrintSetupInfo info, ModelErrorsPrintMode value) {
			info.ErrorsPrintMode = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PagePrintOrder
		public PagePrintOrder PagePrintOrder {
			get { return Info.PagePrintOrder; }
			set {
				if (PagePrintOrder == value)
					return;
				SetPropertyValue(SetPagePrintOrderCore, value);
			}
		}
		DocumentModelChangeActions SetPagePrintOrderCore(PrintSetupInfo info, PagePrintOrder value) {
			info.PagePrintOrder = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Orientation
		public ModelPageOrientation Orientation {
			get { return Info.Orientation; }
			set {
				if (Orientation == value)
					return;
				SetPropertyValue(SetOrientationCore, value);
			}
		}
		DocumentModelChangeActions SetOrientationCore(PrintSetupInfo info, ModelPageOrientation value) {
			info.Orientation = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Scale
		public int Scale {
			get { return Info.Scale; }
			set {
				if (Scale == value)
					return;
				SetPropertyValue(SetScaleCore, value);
			}
		}
		DocumentModelChangeActions SetScaleCore(PrintSetupInfo info, int value) {
			info.Scale = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region BlackAndWhite
		public bool BlackAndWhite {
			get { return Info.BlackAndWhite; }
			set {
				if (BlackAndWhite == value)
					return;
				SetPropertyValue(SetBlackAndWhiteCore, value);
			}
		}
		DocumentModelChangeActions SetBlackAndWhiteCore(PrintSetupInfo info, bool value) {
			info.BlackAndWhite = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Draft
		public bool Draft {
			get { return Info.Draft; }
			set {
				if (Draft == value)
					return;
				SetPropertyValue(SetDraftCore, value);
			}
		}
		DocumentModelChangeActions SetDraftCore(PrintSetupInfo info, bool value) {
			info.Draft = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UseFirstPageNumber
		public bool UseFirstPageNumber {
			get { return Info.UseFirstPageNumber; }
			set {
				if (UseFirstPageNumber == value)
					return;
				SetPropertyValue(SetUseFirstPageNumberCore, value);
			}
		}
		DocumentModelChangeActions SetUseFirstPageNumberCore(PrintSetupInfo info, bool value) {
			info.UseFirstPageNumber = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region UsePrinterDefaults
		public bool UsePrinterDefaults {
			get { return Info.UsePrinterDefaults; }
			set {
				if (UsePrinterDefaults == value)
					return;
				SetPropertyValue(SetUsePrinterDefaultsCore, value);
			}
		}
		DocumentModelChangeActions SetUsePrinterDefaultsCore(PrintSetupInfo info, bool value) {
			info.UsePrinterDefaults = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region AutoPageBreaks
		public bool AutoPageBreaks {
			get { return Info.AutoPageBreaks; }
			set {
				if (AutoPageBreaks == value)
					return;
				SetPropertyValue(SetAutoPageBreaksCore, value);
			}
		}
		DocumentModelChangeActions SetAutoPageBreaksCore(PrintSetupInfo info, bool value) {
			info.AutoPageBreaks = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FitToPage
		public bool FitToPage {
			get { return Info.FitToPage; }
			set {
				if (FitToPage == value)
					return;
				SetPropertyValue(SetFitToPageCore, value);
			}
		}
		DocumentModelChangeActions SetFitToPageCore(PrintSetupInfo info, bool value) {
			info.FitToPage = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region Copies
		public int Copies {
			get { return Info.Copies; }
			set {
				if (Copies == value)
					return;
				SetPropertyValue(SetCopiesCore, value);
			}
		}
		DocumentModelChangeActions SetCopiesCore(PrintSetupInfo info, int value) {
			info.Copies = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FirstPageNumber
		public int FirstPageNumber {
			get { return Info.FirstPageNumber; }
			set {
				if (FirstPageNumber == value)
					return;
				SetPropertyValue(SetFirstPageNumberCore, value);
			}
		}
		DocumentModelChangeActions SetFirstPageNumberCore(PrintSetupInfo info, int value) {
			info.FirstPageNumber = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FitToWidth
		public int FitToWidth {
			get { return Info.FitToWidth; }
			set {
				if (FitToWidth == value)
					return;
				SetPropertyValue(SetFitToWidthCore, value);
			}
		}
		DocumentModelChangeActions SetFitToWidthCore(PrintSetupInfo info, int value) {
			info.FitToWidth = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FitToHeight
		public int FitToHeight {
			get { return Info.FitToHeight; }
			set {
				if (FitToHeight == value)
					return;
				SetPropertyValue(SetFitToHeightCore, value);
			}
		}
		DocumentModelChangeActions SetFitToHeightCore(PrintSetupInfo info, int value) {
			info.FitToHeight = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region HorizontalDpi
		public int HorizontalDpi {
			get { return Info.HorizontalDpi; }
			set {
				if (HorizontalDpi == value)
					return;
				SetPropertyValue(SetHorizontalDpiCore, value);
			}
		}
		DocumentModelChangeActions SetHorizontalDpiCore(PrintSetupInfo info, int value) {
			info.HorizontalDpi = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region VerticalDpi
		public int VerticalDpi {
			get { return Info.VerticalDpi; }
			set {
				if (VerticalDpi == value)
					return;
				SetPropertyValue(SetVerticalDpiCore, value);
			}
		}
		DocumentModelChangeActions SetVerticalDpiCore(PrintSetupInfo info, int value) {
			info.VerticalDpi = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CenterHorizontally
		public bool CenterHorizontally {
			get { return Info.CenterHorizontally; }
			set {
				if (CenterHorizontally == value)
					return;
				SetPropertyValue(SetCenterHorizontallyCore, value);
			}
		}
		DocumentModelChangeActions SetCenterHorizontallyCore(PrintSetupInfo info, bool value) {
			info.CenterHorizontally = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region CenterVertically
		public bool CenterVertically {
			get { return Info.CenterVertically; }
			set {
				if (CenterVertically == value)
					return;
				SetPropertyValue(SetCenterVerticallyCore, value);
			}
		}
		DocumentModelChangeActions SetCenterVerticallyCore(PrintSetupInfo info, bool value) {
			info.CenterVertically = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrintGridLines
		public bool PrintGridLines {
			get { return Info.PrintGridLines; }
			set {
				if (PrintGridLines == value)
					return;
				SetPropertyValue(SetPrintGridLinesCore, value);
			}
		}
		DocumentModelChangeActions SetPrintGridLinesCore(PrintSetupInfo info, bool value) {
			info.PrintGridLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrintGridLinesSet
		public bool PrintGridLinesSet {
			get { return Info.PrintGridLinesSet; }
			set {
				if (PrintGridLinesSet == value)
					return;
				SetPropertyValue(SetPrintGridLinesSetCore, value);
			}
		}
		DocumentModelChangeActions SetPrintGridLinesSetCore(PrintSetupInfo info, bool value) {
			info.PrintGridLinesSet = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region PrintHeadings
		public bool PrintHeadings {
			get { return Info.PrintHeadings; }
			set {
				if (PrintHeadings == value)
					return;
				SetPropertyValue(SetPrintHeadingsCore, value);
			}
		}
		DocumentModelChangeActions SetPrintHeadingsCore(PrintSetupInfo info, bool value) {
			info.PrintHeadings = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<PrintSetupInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.PrintSetupInfoCache;
		}
		public bool IsDefault() {
			return Index == 0;
		}
	}
	#endregion
	#region HeaderFooterInfo
	public class HeaderFooterInfo : ICloneable<HeaderFooterInfo>, ISupportsCopyFrom<HeaderFooterInfo>, ISupportsSizeOf {
		#region Fields
		const uint maskAlignWithMargins = 0x0001;
		const uint maskDifferentFirst   = 0x0002;
		const uint maskDifferentOddEven = 0x0004;
		const uint maskScaleWithDoc	 = 0x0008;
		uint packedValues			   = 0x0009;
		#endregion
		public HeaderFooterInfo() {
			EvenFooter = string.Empty;
			EvenHeader = string.Empty;
			FirstFooter = string.Empty;
			FirstHeader = string.Empty;
			OddFooter = string.Empty;
			OddHeader = string.Empty;
		}
		#region Properties
		public bool AlignWithMargins {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskAlignWithMargins); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskAlignWithMargins, value); }
		}
		public bool DifferentFirst {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDifferentFirst); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDifferentFirst, value); }
		}
		public bool DifferentOddEven {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDifferentOddEven); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDifferentOddEven, value); }
		}
		public bool ScaleWithDoc {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskScaleWithDoc); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskScaleWithDoc, value); }
		}
		public string EvenFooter { get; set; }
		public string EvenHeader { get; set; }
		public string FirstFooter { get; set; }
		public string FirstHeader { get; set; }
		public string OddFooter { get; set; }
		public string OddHeader { get; set; }
		#endregion
		#region ICloneable<HeaderFooterInfo> Members
		public HeaderFooterInfo Clone() {
			HeaderFooterInfo result = new HeaderFooterInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<HeaderFooterInfo> Members
		public void CopyFrom(HeaderFooterInfo value) {
			this.packedValues = value.packedValues;
			this.EvenFooter = value.EvenFooter;
			this.EvenHeader = value.EvenHeader;
			this.FirstFooter = value.FirstFooter;
			this.FirstHeader = value.FirstHeader;
			this.OddFooter = value.OddFooter;
			this.OddHeader = value.OddHeader;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			HeaderFooterInfo other = obj as HeaderFooterInfo;
			if (other == null)
				return false;
			return this.packedValues == other.packedValues &&
				this.EvenFooter == other.EvenFooter &&
				this.EvenHeader == other.EvenHeader &&
				this.FirstFooter == other.FirstFooter &&
				this.FirstHeader == other.FirstHeader &&
				this.OddFooter == other.OddFooter &&
				this.OddHeader == other.OddHeader;
		}
		public override int GetHashCode() 
			{
			CombinedHashCode combined = new CombinedHashCode();
			combined.AddInt((int)this.packedValues);
			combined.AddObject(EvenFooter);
			combined.AddObject(EvenHeader);
			combined.AddObject(FirstFooter);
			combined.AddObject(FirstHeader);
			combined.AddObject(OddFooter);
			combined.AddObject(OddHeader);
			return combined.CombinedHash32;
		}
	}
	#endregion
	#region HeaderFooterInfoCache
	public class HeaderFooterInfoCache : UniqueItemsCache<HeaderFooterInfo> {
		public HeaderFooterInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override HeaderFooterInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			HeaderFooterInfo info = new HeaderFooterInfo();
			return info;
		}
	}
	#endregion
	#region IHeaderFooterFormatTagProvider
	public interface IHeaderFooterFormatTagProvider {
		string CurrentPage { get; }
		string TotalPages { get; }
		string CurrentDate { get; }
		string CurrentTime { get; }
		string FilePath { get; }
		string FileName { get; }
		string SheetName { get; }
	}
	#endregion
	#region HeaderFooterBuilder
	public class HeaderFooterBuilder {
		#region Static
		static List<char> formattingCharTags = CreateFormattingCharTags();
		static List<char> CreateFormattingCharTags() {
			List<char> result = new List<char>();
			result.Add('P'); 
			result.Add('N'); 
			result.Add('S'); 
			result.Add('X'); 
			result.Add('Y'); 
			result.Add('D'); 
			result.Add('T'); 
			result.Add('G'); 
			result.Add('U'); 
			result.Add('E'); 
			result.Add('Z'); 
			result.Add('F'); 
			result.Add('A'); 
			result.Add('+'); 
			result.Add('-'); 
			result.Add('B'); 
			result.Add('I'); 
			result.Add('O'); 
			result.Add('H'); 
			return result;
		}
		static string numberPattern = "[0-9]";
		#endregion
		#region Fields
		string left = string.Empty;
		string center = string.Empty;
		string right = string.Empty;
		IHeaderFooterFormatTagProvider formatTagProvider;
		#endregion
		public HeaderFooterBuilder() { }
		public HeaderFooterBuilder(string value)
			: this(value, false) {
		}
		public HeaderFooterBuilder(string value, bool cutFormattingTags)
			: this(value, cutFormattingTags, null) {
		}
		public HeaderFooterBuilder(string value, bool cutFormattingTags, IHeaderFooterFormatTagProvider formatTagProvider) {
			this.CutFormattingTags = cutFormattingTags;
			this.formatTagProvider = formatTagProvider;
			Parse(value);
		}
		#region Properties
		public bool IsEmpty {
			get { return string.IsNullOrEmpty(this.left) && string.IsNullOrEmpty(this.center) && string.IsNullOrEmpty(this.right); }
		}
		public string Left {
			get { return left; }
			set {
				if (string.IsNullOrEmpty(value))
					left = string.Empty;
				else
					left = value;
			}
		}
		public string Center {
			get { return center; }
			set {
				if (string.IsNullOrEmpty(value))
					center = string.Empty;
				else
					center = value;
			}
		}
		public string Right {
			get { return right; }
			set {
				if (string.IsNullOrEmpty(value))
					right = string.Empty;
				else
					right = value;
			}
		}
		public bool CutFormattingTags { get; set; }
		#endregion
		public void FromString(string value) {
			this.left = string.Empty;
			this.center = string.Empty;
			this.right = string.Empty;
			Parse(value);
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			if (!string.IsNullOrEmpty(this.left)) {
				sb.Append("&L");
				sb.Append(this.left);
			}
			if (!string.IsNullOrEmpty(this.center)) {
				sb.Append("&C");
				sb.Append(this.center);
			}
			if (!string.IsNullOrEmpty(this.right)) {
				sb.Append("&R");
				sb.Append(this.right);
			}
			return sb.ToString();
		}
		protected virtual void Parse(string value) {
			if (string.IsNullOrEmpty(value))
				return;
			StringBuilder sbLeft = new StringBuilder();
			StringBuilder sbCenter = new StringBuilder();
			StringBuilder sbRight = new StringBuilder();
			StringBuilder sbCurrent = sbCenter;
			bool isCommand = false;
			bool isDoubleQuote = false;
			bool isNumber = false;
			for (int i = 0; i < value.Length; i++) {
				char ch = value[i];
				if (ch == '&') {
					if (isCommand) {
						string text = CutFormattingTags ? "&" : "&&";
						sbCurrent.Append(text);
						isCommand = false;
					}
					else
						isCommand = true;
				}
				else if (isDoubleQuote) {
					if (ch == '"')
						isDoubleQuote = false;
				}
				else if (isCommand) {
					if (ch == 'L')
						sbCurrent = sbLeft;
					else if (ch == 'C')
						sbCurrent = sbCenter;
					else if (ch == 'R')
						sbCurrent = sbRight;
					else if (CutFormattingTags && ch == '"')
						isDoubleQuote = true;
					else if (CutFormattingTags && ch == 'K')
						i += 6;
					else if (CutFormattingTags && IsNumber(ch)) {
						isNumber = true;
					}
					else if (TryReplaceFormattingTag(ch, sbCurrent)) { }
					else if (!TryCutFormattingTag(ch)) {
						sbCurrent.Append('&');
						sbCurrent.Append(ch);
					}
					isCommand = false;
				}
				else if (isNumber) {
					isNumber = IsNumber(ch);
					if (!isNumber)
						sbCurrent.Append(ch);
				}
				else
					sbCurrent.Append(ch);
			}
			this.left = sbLeft.ToString();
			this.center = sbCenter.ToString();
			this.right = sbRight.ToString();
		}
		bool IsNumber(char ch) {
			return Regex.IsMatch(ch.ToString(), numberPattern);
		}
		bool TryCutFormattingTag(char currentChar) {
			if (!CutFormattingTags)
				return false;
			if (formattingCharTags.Contains(currentChar))
				return true;
			return false;
		}
		bool TryReplaceFormattingTag(char currentChar, StringBuilder buffer) {
			if (!CutFormattingTags || formatTagProvider == null)
				return false;
			if (currentChar == 'P') {
				buffer.Append(formatTagProvider.CurrentPage);
				return true;
			}
			if (currentChar == 'N') {
				buffer.Append(formatTagProvider.TotalPages);
				return true;
			}
			if (currentChar == 'D') {
				buffer.Append(formatTagProvider.CurrentDate);
				return true;
			}
			if (currentChar == 'T') {
				buffer.Append(formatTagProvider.CurrentTime);
				return true;
			}
			if (currentChar == 'Z') {
				buffer.Append(formatTagProvider.FilePath);
				return true;
			}
			if (currentChar == 'F') {
				buffer.Append(formatTagProvider.FileName);
				return true;
			}
			if (currentChar == 'A') {
				buffer.Append(formatTagProvider.SheetName);
				return true;
			}
			return false;
		}
	}
	#endregion
	#region HeaderFooterOptions
	public class HeaderFooterOptions : SpreadsheetUndoableIndexBasedObject<HeaderFooterInfo>, ICloneable<HeaderFooterOptions> {
		public HeaderFooterOptions(IDocumentModelPartWithApplyChanges part)
			: base(part) {
		}
		#region Properties
		#region AlignWithMargins
		public bool AlignWithMargins {
			get { return Info.AlignWithMargins; }
			set {
				if (AlignWithMargins == value)
					return;
				SetPropertyValue(SetAlignWithMarginsCore, value);
			}
		}
		DocumentModelChangeActions SetAlignWithMarginsCore(HeaderFooterInfo info, bool value) {
			info.AlignWithMargins = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DifferentFirst
		public bool DifferentFirst {
			get { return Info.DifferentFirst; }
			set {
				if (DifferentFirst == value)
					return;
				SetPropertyValue(SetDifferentFirstCore, value);
			}
		}
		DocumentModelChangeActions SetDifferentFirstCore(HeaderFooterInfo info, bool value) {
			info.DifferentFirst = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region DifferentOddEven
		public bool DifferentOddEven {
			get { return Info.DifferentOddEven; }
			set {
				if (DifferentOddEven == value)
					return;
				SetPropertyValue(SetDifferentOddEvenCore, value);
			}
		}
		DocumentModelChangeActions SetDifferentOddEvenCore(HeaderFooterInfo info, bool value) {
			info.DifferentOddEven = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ScaleWithDoc
		public bool ScaleWithDoc {
			get { return Info.ScaleWithDoc; }
			set {
				if (ScaleWithDoc == value)
					return;
				SetPropertyValue(SetScaleWithDocCore, value);
			}
		}
		DocumentModelChangeActions SetScaleWithDocCore(HeaderFooterInfo info, bool value) {
			info.ScaleWithDoc = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EvenFooter
		public string EvenFooter {
			get { return Info.EvenFooter; }
			set {
				if (value == null)
					value = string.Empty;
				if (EvenFooter == value)
					return;
				SetPropertyValue(SetEvenFooterCore, value);
			}
		}
		DocumentModelChangeActions SetEvenFooterCore(HeaderFooterInfo info, string value) {
			info.EvenFooter = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region EvenHeader
		public string EvenHeader {
			get { return Info.EvenHeader; }
			set {
				if (value == null)
					value = string.Empty;
				if (EvenHeader == value)
					return;
				SetPropertyValue(SetEvenHeaderCore, value);
			}
		}
		DocumentModelChangeActions SetEvenHeaderCore(HeaderFooterInfo info, string value) {
			info.EvenHeader = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FirstFooter
		public string FirstFooter {
			get { return Info.FirstFooter; }
			set {
				if (value == null)
					value = string.Empty;
				if (FirstFooter == value)
					return;
				SetPropertyValue(SetFirstFooterCore, value);
			}
		}
		DocumentModelChangeActions SetFirstFooterCore(HeaderFooterInfo info, string value) {
			info.FirstFooter = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region FirstHeader
		public string FirstHeader {
			get { return Info.FirstHeader; }
			set {
				if (value == null)
					value = string.Empty;
				if (FirstHeader == value)
					return;
				SetPropertyValue(SetFirstHeaderCore, value);
			}
		}
		DocumentModelChangeActions SetFirstHeaderCore(HeaderFooterInfo info, string value) {
			info.FirstHeader = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OddFooter
		public string OddFooter {
			get { return Info.OddFooter; }
			set {
				if (value == null)
					value = string.Empty;
				if (OddFooter == value)
					return;
				SetPropertyValue(SetOddFooterCore, value);
			}
		}
		DocumentModelChangeActions SetOddFooterCore(HeaderFooterInfo info, string value) {
			info.OddFooter = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region OddHeader
		public string OddHeader {
			get { return Info.OddHeader; }
			set {
				if (value == null)
					value = string.Empty;
				if (OddHeader == value)
					return;
				SetPropertyValue(SetOddHeaderCore, value);
			}
		}
		DocumentModelChangeActions SetOddHeaderCore(HeaderFooterInfo info, string value) {
			info.OddHeader = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<HeaderFooterInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.HeaderFooterInfoCache;
		}
		#endregion
		#region ICloneable<HeaderFooterOptions> Members
		public HeaderFooterOptions Clone() {
			HeaderFooterOptions result = new HeaderFooterOptions(DocumentModelPart);
			result.CopyFrom(this);
			return result;
		}
		#endregion
	}
	#endregion
}
