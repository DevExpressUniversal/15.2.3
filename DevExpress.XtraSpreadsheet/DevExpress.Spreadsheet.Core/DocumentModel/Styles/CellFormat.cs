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
using DevExpress.Office.Drawing;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region IFormatStringAccessor
	public interface IFormatStringAccessor {
		string FormatString { get; set; }
	}
	#endregion
	#region IFormatBase
	public interface IFormatBase : IRunFontInfo, ICellAlignmentInfo, IBorderInfo, IFillInfo, ICellProtectionInfo, IFormatStringAccessor, IGradientFillInfo, IConvergenceInfo {
		IRunFontInfo Font { get; }
		ICellAlignmentInfo Alignment { get; }
		IBorderInfo Border { get; }
		IFillInfo Fill { get; }
		ICellProtectionInfo Protection { get; }
	}
	#endregion
	#region IActualFormat
	public interface IActualFormat : IActualRunFontInfo, IActualCellAlignmentInfo, IActualBorderInfo, IActualFillInfo, IActualCellProtectionInfo, IActualGradientFillInfo, IActualConvergenceInfo {
		IActualRunFontInfo ActualFont { get; }
		IActualCellAlignmentInfo ActualAlignment { get; }
		IActualBorderInfo ActualBorder { get; }
		IActualFillInfo ActualFill { get; }
		IActualCellProtectionInfo ActualProtection { get; }
		string ActualFormatString { get; }
		int ActualFormatIndex { get; }
		IActualApplyInfo ActualApplyInfo { get; }
	}
	#endregion
	#region IActualApplyInfo
	public interface IActualApplyInfo {
		bool ApplyFont { get; }
		bool ApplyFill { get; }
		bool ApplyBorder { get; }
		bool ApplyAlignment { get; }
		bool ApplyProtection { get; }
		bool ApplyNumberFormat { get; }
	}
	#endregion
	#region IFormatBaseBatchUpdateable
	public interface IFormatBaseBatchUpdateable : IFormatStringAccessor, IFormatBase, IActualFormat, IBatchUpdateable {
		bool ApplyFont { get; set; }
		bool ApplyAlignment { get; set; }
		bool ApplyBorder { get; set; }
		bool ApplyFill { get; set; }
		bool ApplyProtection { get; set; }
		bool ApplyNumberFormat { get; set; }
		DocumentModel DocumentModel { get; }
	}
	#endregion
	#region ICellFormat
	public interface ICellFormat : IFormatBase, IActualFormat {
		CellStyleBase Style { get; set; }
	}
	#endregion
	#region CellFormatCache
	public class CellFormatCache : UniqueItemsCache<FormatBase> {
		#region Fields
		public const int DefaultItemIndex = 0;
		public const int DefaultCellStyleFormatIndex = 1;
		public const int DefaultDifferentialFormatIndex = 2;
		#endregion
		public CellFormatCache(IDocumentModelUnitConverter unitConverter, DocumentModel workbook)
			: base(unitConverter, workbook) {
			UniquenessProviderType = DXCollectionUniquenessProviderType.MaximizePerformance;
		}
		public FormatBase DefaultCellStyleFormatItem { get { return Items[DefaultCellStyleFormatIndex]; } }
		public FormatBase DefaultDifferentialFormatItem { get { return Items[DefaultDifferentialFormatIndex]; } }
		protected override FormatBase CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new CellFormat((DocumentModel)workbook);
		}
		FormatBase CreateDefaultCellStyleFormatItem() {
			return new CellStyleFormat((DocumentModel)workbook);
		}
		FormatBase CreateDefaultDifferentialFormatItem() {
			return new DifferentialFormat((DocumentModel)workbook);
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			FormatBase defaultItem = CreateDefaultItem(unitConverter);
			if (defaultItem != null)
				AppendItem(defaultItem);
			FormatBase defaultCellStyleFormatItem = CreateDefaultCellStyleFormatItem();
			if (defaultCellStyleFormatItem != null)
				AppendItem(defaultCellStyleFormatItem);
			FormatBase defaultDifferentialFormatItem = CreateDefaultDifferentialFormatItem();
			if (defaultDifferentialFormatItem != null)
				AppendItem(defaultDifferentialFormatItem);
		}
		public void CopyFrom(CellFormatCache source) {
			this.Items.Clear();
			if (ItemDictionary != null)
				this.ItemDictionary.Clear();
			int currentIndex = 0;
			foreach (FormatBase item in source.Items) {
				FormatBase clone = item.CreateEmptyClone(workbook);
				clone.CopySimple(item);
				Items.Add(clone);
				if (UniquenessProviderType == DXCollectionUniquenessProviderType.MaximizePerformance)
					ItemDictionary.Add(clone, currentIndex);
				currentIndex++;
			}
		}
#if DEBUGTEST
		public static bool CheckDefaults2(CellFormatCache collection) {
			bool result = true;
			result &= collection != null;
			result &= collection.Count > 0;
			CellFormat cellFormat = (CellFormat)collection.DefaultItem;
			CellStyleFormat cellStyleFormat = (CellStyleFormat)collection.DefaultCellStyleFormatItem;
			DifferentialFormat differentialFormat = (DifferentialFormat)collection.DefaultDifferentialFormatItem;
			result &= CellAlignmentInfoCache.DefaultItemIndex == cellFormat.AlignmentIndex;
			result &= BorderInfoCache.DefaultItemIndex == cellFormat.BorderIndex;
			result &= CellFormatFlagsInfo.DefaultCellFormatValue == cellFormat.CellFormatFlagsIndex;
			result &= FillInfoCache.DefaultItemIndex == cellFormat.FillIndex;
			result &= RunFontInfoCache.DefaultItemIndex == cellFormat.FontIndex;
			result &= 0 == cellFormat.NumberFormatIndex;
			result &= 0 == cellFormat.StyleIndex;
			result &= CellAlignmentInfoCache.DefaultItemIndex == cellStyleFormat.AlignmentIndex;
			result &= BorderInfoCache.DefaultItemIndex == cellStyleFormat.BorderIndex;
			result &= CellFormatFlagsInfo.DefaultStyleValue == cellStyleFormat.CellFormatFlagsIndex;
			result &= FillInfoCache.DefaultItemIndex == cellStyleFormat.FillIndex;
			result &= RunFontInfoCache.DefaultItemIndex == cellStyleFormat.FontIndex;
			result &= 0 == cellStyleFormat.NumberFormatIndex;
			result &= RunFontInfoCache.DefaultItemIndex == differentialFormat.FontIndex;
			result &= 0 == differentialFormat.NumberFormatIndex;
			result &= FillInfoCache.DefaultItemIndex == differentialFormat.FillIndex;
			result &= CellAlignmentInfoCache.DefaultItemIndex == differentialFormat.AlignmentIndex;
			result &= BorderInfoCache.DefaultItemIndex == differentialFormat.BorderIndex;
			result &= CellFormatFlagsInfo.DefaultCellFormatValue == differentialFormat.CellFormatFlagsIndex;
			result &= MultiOptionsInfo.DefaultIndex == differentialFormat.MultiOptionsIndex;
			result &= BorderOptionsInfo.DefaultIndex == differentialFormat.BorderOptionsIndex;
			return result;
		}
#endif
	}
	#endregion
	public class InvalidNumberFormatException : Exception {
		public InvalidNumberFormatException(string message, string formatString)
			: base(message) {
			FormatString = formatString;
		}
		public string FormatString { get; private set; }
	}
	#region FormatBase
	public abstract class FormatBase : MultiIndexObject<FormatBase, DocumentModelChangeActions>, IFormatBase, ICloneable<FormatBase>, ISupportsCopyFrom<FormatBase> {
		#region Static Members
		readonly static FontIndexAccessor fontIndexAccessor = new FontIndexAccessor();
		readonly static AlignmentIndexAccessor alignmentIndexAccessor = new AlignmentIndexAccessor();
		readonly static BorderIndexAccessor borderIndexAccessor = new BorderIndexAccessor();
		readonly static FillIndexAccessor fillIndexAccessor = new FillIndexAccessor();
		readonly static GradientFillIndexAccessor gradientFillIndexAccessor = new GradientFillIndexAccessor();
		readonly static CellFormatFlagsIndexAccessor cellFormatFlagsIndexAccessor = new CellFormatFlagsIndexAccessor();
		readonly static NumberFormatIndexAccessor numberFormatIndexAccessor = new NumberFormatIndexAccessor();
		readonly static IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] {
			fontIndexAccessor,
			alignmentIndexAccessor,
			borderIndexAccessor,
			fillIndexAccessor,
			gradientFillIndexAccessor, 
			cellFormatFlagsIndexAccessor,
			numberFormatIndexAccessor
		};
		public static FontIndexAccessor FontIndexAccessor { get { return fontIndexAccessor; } }
		public static AlignmentIndexAccessor AlignmentIndexAccessor { get { return alignmentIndexAccessor; } }
		public static BorderIndexAccessor BorderIndexAccessor { get { return borderIndexAccessor; } }
		public static FillIndexAccessor FillIndexAccessor { get { return fillIndexAccessor; } }
		public static GradientFillIndexAccessor GradientFillIndexAccessor { get { return gradientFillIndexAccessor; } }
		public static CellFormatFlagsIndexAccessor CellFormatFlagsIndexAccessor { get { return cellFormatFlagsIndexAccessor; } }
		public static NumberFormatIndexAccessor NumberFormatIndexAccessor { get { return numberFormatIndexAccessor; } }
		#endregion
		#region Fields
		readonly IDocumentModel documentModel;
		int fontIndex;
		int alignmentIndex;
		int borderIndex;
		int fillIndex;
		int gradientFillInfoIndex;
		int cellFormatFlagsIndex = CellFormatFlagsInfo.DefaultCellFormatValue;
		int numberFormatIndex;
		GradientStopInfoCollection gradientStopInfoCollection;
		#endregion
		protected FormatBase(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			gradientStopInfoCollection = new GradientStopInfoCollection(documentModel);
		}
		#region Properties
		internal int FontIndex { get { return fontIndex; } }
		internal int AlignmentIndex { get { return alignmentIndex; } }
		internal int BorderIndex { get { return borderIndex; } }
		internal int FillIndex { get { return fillIndex; } }
		internal int GradientFillInfoIndex { get { return gradientFillInfoIndex; } }
		internal int CellFormatFlagsIndex { get { return cellFormatFlagsIndex; } }
		internal int NumberFormatIndex { get { return numberFormatIndex; } }
		protected override IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		internal new FormatBaseBatchUpdateHelper BatchUpdateHelper { get { return (FormatBaseBatchUpdateHelper)base.BatchUpdateHelper; } }
		protected internal RunFontInfo FontInfo { get { return IsUpdateLocked ? BatchUpdateHelper.FontInfo : FontInfoCore; } }
		protected internal CellAlignmentInfo AlignmentInfo { get { return IsUpdateLocked ? BatchUpdateHelper.AlignmentInfo : AlignmentInfoCore; } }
		protected internal BorderInfo BorderInfo { get { return IsUpdateLocked ? BatchUpdateHelper.BorderInfo : BorderInfoCore; } }
		protected internal FillInfo FillInfo { get { return IsUpdateLocked ? BatchUpdateHelper.FillInfo : FillInfoCore; } }
		protected internal GradientFillInfo GradientFillInfo { get { return IsUpdateLocked ? BatchUpdateHelper.GradientFillInfo : GradientFillInfoCore; } }
		protected internal CellFormatFlagsInfo CellFormatFlagsInfo { get { return IsUpdateLocked ? BatchUpdateHelper.CellFormatFlagsInfo : CellFormatFlagsInfoCore; } }
		protected internal NumberFormat NumberFormatInfo { get { return IsUpdateLocked ? BatchUpdateHelper.NumberFormatInfo : NumberFormatInfoCore; } }
		protected internal GradientStopInfoCollection GradientStopInfoCollection { get { return gradientStopInfoCollection; } }
		RunFontInfo FontInfoCore { get { return fontIndexAccessor.GetInfo(this); } }
		CellAlignmentInfo AlignmentInfoCore { get { return alignmentIndexAccessor.GetInfo(this); } }
		BorderInfo BorderInfoCore { get { return borderIndexAccessor.GetInfo(this); } }
		FillInfo FillInfoCore { get { return fillIndexAccessor.GetInfo(this); } }
		GradientFillInfo GradientFillInfoCore { get { return gradientFillIndexAccessor.GetInfo(this); } }
		CellFormatFlagsInfo CellFormatFlagsInfoCore { get { return cellFormatFlagsIndexAccessor.GetInfo(this); } }
		NumberFormat NumberFormatInfoCore { get { return numberFormatIndexAccessor.GetInfo(this); } }
		protected abstract FillInfo DefaultFillInfo { get; }
		protected abstract GradientFillInfo DefaultGradientFillInfo { get; }
		public new DocumentModel DocumentModel { get { return (DocumentModel)base.DocumentModel; } }
		#region IFormatBase Members
		public IRunFontInfo Font { get { return this; } }
		public ICellAlignmentInfo Alignment { get { return this; } }
		public IBorderInfo Border { get { return this; } }
		public IFillInfo Fill { get { return this; } }
		public ICellProtectionInfo Protection { get { return this; } }
		#endregion
		#region Alignment
		#region Alignment.Horizontal
		XlHorizontalAlignment ICellAlignmentInfo.Horizontal {
			get { return AlignmentInfo.HorizontalAlignment; }
			set {
				if (AlignmentInfo.HorizontalAlignment == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentHorizontal, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentHorizontal(CellAlignmentInfo info, XlHorizontalAlignment value) {
			SetAlignmentCore();
			info.HorizontalAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.Vertical
		XlVerticalAlignment ICellAlignmentInfo.Vertical {
			get { return AlignmentInfo.VerticalAlignment; }
			set {
				if (AlignmentInfo.VerticalAlignment == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentVertical, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentVertical(CellAlignmentInfo info, XlVerticalAlignment value) {
			SetAlignmentCore();
			info.VerticalAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.WrapText
		bool ICellAlignmentInfo.WrapText {
			get { return AlignmentInfo.WrapText; }
			set {
				if (AlignmentInfo.WrapText == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentWrapText, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentWrapText(CellAlignmentInfo info, bool value) {
			SetAlignmentCore();
			info.WrapText = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.JustifyLastLine
		bool ICellAlignmentInfo.JustifyLastLine {
			get { return AlignmentInfo.JustifyLastLine; }
			set {
				if (AlignmentInfo.JustifyLastLine == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentJustifyLastLine, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentJustifyLastLine(CellAlignmentInfo info, bool value) {
			SetAlignmentCore();
			info.JustifyLastLine = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.ShrinkToFit
		bool ICellAlignmentInfo.ShrinkToFit {
			get { return AlignmentInfo.ShrinkToFit; }
			set {
				if (AlignmentInfo.ShrinkToFit == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentShrinkToFit, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentShrinkToFit(CellAlignmentInfo info, bool value) {
			SetAlignmentCore();
			info.ShrinkToFit = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.TextRotation
		int ICellAlignmentInfo.TextRotation {
			get { return AlignmentInfo.TextRotation; }
			set {
				if (AlignmentInfo.TextRotation == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentTextRotation, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentTextRotation(CellAlignmentInfo info, int value) {
			SetAlignmentCore();
			info.TextRotation = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.Indent
		byte ICellAlignmentInfo.Indent {
			get { return AlignmentInfo.Indent; }
			set {
				if (AlignmentInfo.Indent == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentIndent, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentIndent(CellAlignmentInfo info, byte value) {
			SetAlignmentCore();
			info.Indent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.RelativeIndent
		int ICellAlignmentInfo.RelativeIndent {
			get { return AlignmentInfo.RelativeIndent; }
			set {
				if (AlignmentInfo.RelativeIndent == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentRelativeIndent, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentRelativeIndent(CellAlignmentInfo info, int value) {
			SetAlignmentCore();
			info.RelativeIndent = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Alignment.XlReadingOrder
		XlReadingOrder ICellAlignmentInfo.ReadingOrder {
			get { return AlignmentInfo.ReadingOrder; }
			set {
				if (AlignmentInfo.ReadingOrder == value)
					return;
				SetPropertyValue(AlignmentIndexAccessor, SetAlignmentReadingOrder, value);
			}
		}
		protected DocumentModelChangeActions SetAlignmentReadingOrder(CellAlignmentInfo info, XlReadingOrder value) {
			SetAlignmentCore();
			info.ReadingOrder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region SetActualAlignment
		public void SetActualAlignment(IActualCellAlignmentInfo value) {
			SetPropertyValue(alignmentIndexAccessor, SetActualAlignmentCore, value);
		}
		DocumentModelChangeActions SetActualAlignmentCore(CellAlignmentInfo info, IActualCellAlignmentInfo value) {
			SetAlignmentCore();
			info.CopyFrom(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region Font
		#region Font.Name
		string IRunFontInfo.Name {
			get { return FontInfo.Name; }
			set {
				if (FontInfo.Name == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontName, value);
			}
		}
		protected DocumentModelChangeActions SetFontName(RunFontInfo info, string value) {
			SetFontCore();
			SetFontNameCore(info);
			info.Name = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Color
		Color IRunFontInfo.Color {
			get { return GetColor(FontInfo.ColorIndex); }
			set {
				if (GetColor(FontInfo.ColorIndex) == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontColor, value);
			}
		}
		protected DocumentModelChangeActions SetFontColor(RunFontInfo info, Color value) {
			SetFontCore();
			info.ColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Bold
		bool IRunFontInfo.Bold {
			get { return FontInfo.Bold; }
			set {
				if (FontInfo.Bold == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontBold, value);
			}
		}
		protected DocumentModelChangeActions SetFontBold(RunFontInfo info, bool value) {
			SetFontCore();
			info.Bold = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Condense
		bool IRunFontInfo.Condense {
			get { return FontInfo.Condense; }
			set {
				if (FontInfo.Condense == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontCondense, value);
			}
		}
		protected DocumentModelChangeActions SetFontCondense(RunFontInfo info, bool value) {
			SetFontCore();
			info.Condense = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Extend
		bool IRunFontInfo.Extend {
			get { return FontInfo.Extend; }
			set {
				if (FontInfo.Extend == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontExtend, value);
			}
		}
		protected DocumentModelChangeActions SetFontExtend(RunFontInfo info, bool value) {
			SetFontCore();
			info.Extend = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Italic
		bool IRunFontInfo.Italic {
			get { return FontInfo.Italic; }
			set {
				if (FontInfo.Italic == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontItalic, value);
			}
		}
		protected DocumentModelChangeActions SetFontItalic(RunFontInfo info, bool value) {
			SetFontCore();
			info.Italic = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Outline
		bool IRunFontInfo.Outline {
			get { return FontInfo.Outline; }
			set {
				if (FontInfo.Outline == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontOutline, value);
			}
		}
		protected DocumentModelChangeActions SetFontOutline(RunFontInfo info, bool value) {
			SetFontCore();
			info.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Shadow
		bool IRunFontInfo.Shadow {
			get { return FontInfo.Shadow; }
			set {
				if (FontInfo.Shadow == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontShadow, value);
			}
		}
		protected DocumentModelChangeActions SetFontShadow(RunFontInfo info, bool value) {
			SetFontCore();
			info.Shadow = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.StrikeThrough
		bool IRunFontInfo.StrikeThrough {
			get { return FontInfo.StrikeThrough; }
			set {
				if (FontInfo.StrikeThrough == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontStrikeThrough, value);
			}
		}
		protected DocumentModelChangeActions SetFontStrikeThrough(RunFontInfo info, bool value) {
			SetFontCore();
			info.StrikeThrough = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Charset
		int IRunFontInfo.Charset {
			get { return FontInfo.Charset; }
			set {
				if (FontInfo.Charset == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontCharset, value);
			}
		}
		protected DocumentModelChangeActions SetFontCharset(RunFontInfo info, int value) {
			SetFontCore();
			info.Charset = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.FontFamily
		int IRunFontInfo.FontFamily {
			get { return FontInfo.FontFamily; }
			set {
				if (FontInfo.FontFamily == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontFontFamily, value);
			}
		}
		protected DocumentModelChangeActions SetFontFontFamily(RunFontInfo info, int value) {
			SetFontCore();
			info.FontFamily = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.Size
		double IRunFontInfo.Size {
			get { return FontInfo.Size; }
			set {
				if (FontInfo.Size == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontSize, value);
			}
		}
		protected DocumentModelChangeActions SetFontSize(RunFontInfo info, double value) {
			SetFontCore();
			info.Size = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.FontSchemeStyles
		XlFontSchemeStyles IRunFontInfo.SchemeStyle {
			get { return FontInfo.SchemeStyle; }
			set {
				if (FontInfo.SchemeStyle == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontSchemeStyle, value);
			}
		}
		protected DocumentModelChangeActions SetFontSchemeStyle(RunFontInfo info, XlFontSchemeStyles value) {
			SetFontCore();
			info.SchemeStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.FontScripts
		XlScriptType IRunFontInfo.Script {
			get { return FontInfo.Script; }
			set {
				if (FontInfo.Script == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontScript, value);
			}
		}
		protected DocumentModelChangeActions SetFontScript(RunFontInfo info, XlScriptType value) {
			SetFontCore();
			info.Script = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Font.FontUnderline
		XlUnderlineType IRunFontInfo.Underline {
			get { return FontInfo.Underline; }
			set {
				if (FontInfo.Underline == value)
					return;
				SetPropertyValue(fontIndexAccessor, SetFontUnderline, value);
			}
		}
		protected DocumentModelChangeActions SetFontUnderline(RunFontInfo info, XlUnderlineType value) {
			SetFontCore();
			info.Underline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region SetActualFont
		public void SetActualFont(IActualRunFontInfo value) {
			SetPropertyValue(fontIndexAccessor, SetActualFontCore, value);
		}
		DocumentModelChangeActions SetActualFontCore(RunFontInfo info, IActualRunFontInfo value) {
			SetFontCore();
			info.CopyFrom(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region Borders
		#region Borders.LeftLineStyle
		XlBorderLineStyle IBorderInfo.LeftLineStyle {
			get { return BorderInfo.LeftLineStyle; }
			set {
				if (BorderInfo.LeftLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderLeftLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderLeftLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.LeftLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.RightLineStyle
		XlBorderLineStyle IBorderInfo.RightLineStyle {
			get { return BorderInfo.RightLineStyle; }
			set {
				if (BorderInfo.RightLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderRightLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderRightLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.RightLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.TopLineStyle
		XlBorderLineStyle IBorderInfo.TopLineStyle {
			get { return BorderInfo.TopLineStyle; }
			set {
				if (BorderInfo.TopLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderTopLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderTopLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.TopLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.BottomLineStyle
		XlBorderLineStyle IBorderInfo.BottomLineStyle {
			get { return BorderInfo.BottomLineStyle; }
			set {
				if (BorderInfo.BottomLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderBottomLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderBottomLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.BottomLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.DiagonalUpLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalUpLineStyle {
			get { return GetBorderDiagonalLineStyle(BorderInfo.DiagonalUp); }
			set {
				if (GetBorderDiagonalLineStyle(BorderInfo.DiagonalUp) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderDiagonalUpLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderDiagonalUpLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.DiagonalUp = value != XlBorderLineStyle.None;
			SetBorderDiagonalLineStyle(info, value);
			return DocumentModelChangeActions.None; 
		}
		protected XlBorderLineStyle GetBorderDiagonalLineStyle(bool IsExistDiagonal) {
			return IsExistDiagonal ? BorderInfo.DiagonalLineStyle : XlBorderLineStyle.None;
		}
		void SetBorderDiagonalLineStyle(BorderInfo info, XlBorderLineStyle value) {
			if (value != XlBorderLineStyle.None)
				info.DiagonalLineStyle = value;
			if (!info.DiagonalUp && !info.DiagonalDown)
				info.DiagonalLineStyle = XlBorderLineStyle.None;
			SetBorderCore();
		}
		#endregion
		#region Borders.DiagonalDownLineStyle
		XlBorderLineStyle IBorderInfo.DiagonalDownLineStyle {
			get { return GetBorderDiagonalLineStyle(BorderInfo.DiagonalDown); }
			set {
				if (GetBorderDiagonalLineStyle(BorderInfo.DiagonalDown) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderDiagonalDownLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderDiagonalDownLineStyle(BorderInfo info, XlBorderLineStyle value) {
			info.DiagonalDown = value != XlBorderLineStyle.None;
			SetBorderDiagonalLineStyle(info, value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.HorizontalLineStyle
		XlBorderLineStyle IBorderInfo.HorizontalLineStyle {
			get { return BorderInfo.HorizontalLineStyle; }
			set {
				if (BorderInfo.HorizontalLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderHorizontalLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderHorizontalLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.HorizontalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.VerticalLineStyle
		XlBorderLineStyle IBorderInfo.VerticalLineStyle {
			get { return BorderInfo.VerticalLineStyle; }
			set {
				if (BorderInfo.VerticalLineStyle == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderVerticalLineStyle, value);
			}
		}
		protected DocumentModelChangeActions SetBorderVerticalLineStyle(BorderInfo info, XlBorderLineStyle value) {
			SetBorderCore();
			info.VerticalLineStyle = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.Outline
		bool IBorderInfo.Outline {
			get { return BorderInfo.Outline; }
			set {
				if (BorderInfo.Outline == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderOutline, value);
			}
		}
		protected DocumentModelChangeActions SetBorderOutline(BorderInfo info, bool value) {
			SetBorderCore();
			info.Outline = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.LeftColor
		Color IBorderInfo.LeftColor {
			get { return GetColor(BorderInfo.LeftColorIndex); }
			set {
				if (GetColor(BorderInfo.LeftColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderLeftColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderLeftColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.LeftColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.RightColor
		Color IBorderInfo.RightColor {
			get { return GetColor(BorderInfo.RightColorIndex); }
			set {
				if (GetColor(BorderInfo.RightColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderRightColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderRightColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.RightColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.TopColor
		Color IBorderInfo.TopColor {
			get { return GetColor(BorderInfo.TopColorIndex); }
			set {
				if (GetColor(BorderInfo.TopColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderTopColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderTopColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.TopColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.BottomColor
		Color IBorderInfo.BottomColor {
			get { return GetColor(BorderInfo.BottomColorIndex); }
			set {
				if (GetColor(BorderInfo.BottomColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderBottomColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderBottomColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.BottomColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.DiagonalColor
		Color IBorderInfo.DiagonalColor {
			get { return GetColor(BorderInfo.DiagonalColorIndex); }
			set {
				if (GetColor(BorderInfo.DiagonalColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderDiagonalColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderDiagonalColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.DiagonalColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.HorizontalColor
		Color IBorderInfo.HorizontalColor {
			get { return GetColor(BorderInfo.HorizontalColorIndex); }
			set {
				if (GetColor(BorderInfo.HorizontalColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderHorizontalColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderHorizontalColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.HorizontalColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.VerticalColor
		Color IBorderInfo.VerticalColor {
			get { return GetColor(BorderInfo.VerticalColorIndex); }
			set {
				if (GetColor(BorderInfo.VerticalColorIndex) == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderVerticalColorCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderVerticalColorCore(BorderInfo info, Color value) {
			SetBorderCore();
			info.VerticalColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.LeftColorIndex
		int IBorderInfo.LeftColorIndex {
			get { return BorderInfo.LeftColorIndex; }
			set {
				if (BorderInfo.LeftColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderLeftColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderLeftColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.LeftColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.RightColorIndex
		int IBorderInfo.RightColorIndex {
			get { return BorderInfo.RightColorIndex; }
			set {
				if (BorderInfo.RightColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderRightColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderRightColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.RightColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.TopColorIndex
		int IBorderInfo.TopColorIndex {
			get { return BorderInfo.TopColorIndex; }
			set {
				if (BorderInfo.TopColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderTopColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderTopColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.TopColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.BottomColorIndex
		int IBorderInfo.BottomColorIndex {
			get { return BorderInfo.BottomColorIndex; }
			set {
				if (BorderInfo.BottomColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderBottomColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderBottomColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.BottomColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.DiagonalColorIndex
		int IBorderInfo.DiagonalColorIndex {
			get { return BorderInfo.DiagonalColorIndex; }
			set {
				if (BorderInfo.DiagonalColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderDiagonalColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderDiagonalColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.DiagonalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.HorizontalColorIndex
		int IBorderInfo.HorizontalColorIndex {
			get { return BorderInfo.HorizontalColorIndex; }
			set {
				if (BorderInfo.HorizontalColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderHorizontalColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderHorizontalColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.HorizontalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Borders.VerticalColorIndex
		int IBorderInfo.VerticalColorIndex {
			get { return BorderInfo.VerticalColorIndex; }
			set {
				if (BorderInfo.VerticalColorIndex == value)
					return;
				SetPropertyValue(borderIndexAccessor, SetBorderVerticalColorIndexCore, value);
			}
		}
		protected DocumentModelChangeActions SetBorderVerticalColorIndexCore(BorderInfo info, int value) {
			SetBorderCore();
			info.VerticalColorIndex = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region SetActualBorder
		public void SetActualBorder(IActualBorderInfo value) {
			SetPropertyValue(BorderIndexAccessor, SetActualBorderCore, value);
		}
		DocumentModelChangeActions SetActualBorderCore(BorderInfo info, IActualBorderInfo value) {
			SetBorderCore();
			info.CopyFrom(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#region Fill
		#region IFillInfo.Clear
		void IFillInfo.Clear() {
			if (CheckClearFill())
				ClearFillCore();
		}
		void ClearFillCore() {
			DocumentModel.BeginUpdate();
			try {
				ReplaceInfo(fillIndexAccessor, DefaultFillInfo, DocumentModelChangeActions.None);
				ReplaceInfo(gradientFillIndexAccessor, DefaultGradientFillInfo, DocumentModelChangeActions.None);
				ClearFillOptions();
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		#endregion
		#region Fill.PatternType
		XlPatternType IFillInfo.PatternType {
			get { return FillInfo.PatternType; }
			set {
				if (FillInfo.PatternType == value)
					return;
				SetPropertyValue(fillIndexAccessor, SetFillPatternType, value);
			}
		}
		protected DocumentModelChangeActions SetFillPatternType(FillInfo info, XlPatternType value) {
			SetFillCore();
			info.PatternType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Fill.ForeColor
		Color IFillInfo.ForeColor {
			get { return GetColor(FillInfo.ForeColorIndex); }
			set {
				if (GetColor(FillInfo.ForeColorIndex) == value)
					return;
				SetPropertyValue(fillIndexAccessor, SetFillForeColor, value);
			}
		}
		protected DocumentModelChangeActions SetFillForeColor(FillInfo info, Color value) {
			SetFillCore();
			info.ForeColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Fill.BackColor
		Color IFillInfo.BackColor {
			get { return GetColor(FillInfo.BackColorIndex); }
			set {
				if (GetColor(FillInfo.BackColorIndex) == value)
					return;
				SetPropertyValue(fillIndexAccessor, SetFillBackColor, value);
			}
		}
		protected DocumentModelChangeActions SetFillBackColor(FillInfo info, Color value) {
			SetFillCore();
			info.BackColorIndex = DocumentModel.Cache.ColorModelInfoCache.GetItemIndex(SetColorCore(value));
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Fill.GradientFill
		IGradientFillInfo IFillInfo.GradientFill { get { return this; } }
		#endregion
		#region Fill.FillType
		ModelFillType IFillInfo.FillType {
			get { return CellFormatFlagsInfo.FillType; }
			set {
				if (CellFormatFlagsInfo.FillType == value)
					return;
				SetPropertyValueForStruct(cellFormatFlagsIndexAccessor, SetModelFillType, value);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected DocumentModelChangeActions SetModelFillType(ref CellFormatFlagsInfo info, ModelFillType value) {
			SetFillCore(ref info);
			info.FillType = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region SetActualFill
		public void SetActualFill(IActualFillInfo value) {
			SetPropertyValue(fillIndexAccessor, SetActualFillCore, value);
		}
		DocumentModelChangeActions SetActualFillCore(FillInfo info, IActualFillInfo value) {
			SetFillCore();
			info.CopyFrom(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region GradientFill
		#region GradientFill.Convergence
		IConvergenceInfo IGradientFillInfo.Convergence { get { return this; } }
		#endregion
		#region GradientFill.GradientStops
		IGradientStopCollection IGradientFillInfo.GradientStops { get { return gradientStopInfoCollection; } }
		#endregion
		#region GradientFill.Type
		ModelGradientFillType IGradientFillInfo.Type {
			get { return GradientFillInfo.Type; }
			set {
				if (GradientFillInfo.Type == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoType, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoType(GradientFillInfo info, ModelGradientFillType value) {
			SetFillCore();
			info.Type = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region GradientFill.Degree
		double IGradientFillInfo.Degree {
			get { return GradientFillInfo.Degree; }
			set {
				if (GradientFillInfo.Degree == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoDegree, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoDegree(GradientFillInfo info, double value) {
			SetFillCore();
			info.Degree = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Convergence
		#region Convergence.Left
		float IConvergenceInfo.Left {
			get { return GradientFillInfo.Left; }
			set {
				if (GradientFillInfo.Left == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoLeft, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoLeft(GradientFillInfo info, float value) {
			SetFillCore();
			info.Left = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Convergence.Right
		float IConvergenceInfo.Right {
			get { return GradientFillInfo.Right; }
			set {
				if (GradientFillInfo.Right == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoRight, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoRight(GradientFillInfo info, float value) {
			SetFillCore();
			info.Right = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Convergence.Top
		float IConvergenceInfo.Top {
			get { return GradientFillInfo.Top; }
			set {
				if (GradientFillInfo.Top == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoTop, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoTop(GradientFillInfo info, float value) {
			SetFillCore();
			info.Top = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Convergence.Bottom
		float IConvergenceInfo.Bottom {
			get { return GradientFillInfo.Bottom; }
			set {
				if (GradientFillInfo.Bottom == value)
					return;
				SetPropertyValue(GradientFillIndexAccessor, SetGradientFillInfoBottom, value);
			}
		}
		protected DocumentModelChangeActions SetGradientFillInfoBottom(GradientFillInfo info, float value) {
			SetFillCore();
			info.Bottom = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		#endregion
		#endregion
		#region Protection
		#region Protection.Locked
		bool ICellProtectionInfo.Locked {
			get { return CellFormatFlagsInfo.Locked; }
			set {
				if (CellFormatFlagsInfo.Locked == value)
					return;
				SetPropertyValueForStruct(cellFormatFlagsIndexAccessor, SetProtectionLocked, value);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected DocumentModelChangeActions SetProtectionLocked(ref CellFormatFlagsInfo info, bool value) {
			SetProtectionCore(ref info);
			info.Locked = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Protection.Hidden
		bool ICellProtectionInfo.Hidden {
			get { return CellFormatFlagsInfo.Hidden; }
			set {
				if (CellFormatFlagsInfo.Hidden == value)
					return;
				SetPropertyValueForStruct(cellFormatFlagsIndexAccessor, SetProtectionHidden, value);
			}
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected DocumentModelChangeActions SetProtectionHidden(ref CellFormatFlagsInfo info, bool value) {
			SetProtectionCore(ref info);
			info.Hidden = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		public void SetActualProtection(IActualCellProtectionInfo value) {
			SetPropertyValueForStruct(cellFormatFlagsIndexAccessor, SetActualProtectionCore, value);
		}
		DocumentModelChangeActions SetActualProtectionCore(ref CellFormatFlagsInfo info, IActualCellProtectionInfo value) {
			SetProtectionCore(ref info);
			info.CopyFrom(value);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region FormatString
		public string FormatString {
			get { return NumberFormatInfo.FormatCode; }
			set { SetFormatString(value); }
		}
		protected virtual void SetFormatString(string value) {
			if (NumberFormatInfo.FormatCode == value)
				return;
			SetPropertyValue(numberFormatIndexAccessor, SetFormatString, value);
		}
		public void ForceSetFormatString(string value) {
			SetPropertyValue(numberFormatIndexAccessor, SetFormatString, value);
		}
		protected DocumentModelChangeActions SetFormatString(NumberFormat info, string value) {
			NumberFormat numberFormat = NumberFormatParser.Parse(value);
			if (numberFormat == null)
				throw new InvalidNumberFormatException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidNumberFormat), value);
			SetNumberFormatCore();
			info.CopyFrom(numberFormat);
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion
		protected virtual void SetAlignmentCore() {
		}
		protected virtual void SetFontCore(){
		}
		protected virtual void SetFontNameCore(RunFontInfo info) {
		}
		protected virtual void SetBorderCore() {
		}
		protected virtual void SetFillCore() {
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected virtual void SetFillCore(ref CellFormatFlagsInfo info) {
		}
		protected virtual void SetNumberFormatCore() {
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		protected virtual void SetProtectionCore(ref CellFormatFlagsInfo info) {
		}
		protected Color GetColor(int colorIndex) {
			return DocumentModel.Cache.ColorModelInfoCache[colorIndex].ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
		}
		ColorModelInfo SetColorCore(Color value) {
			ColorModelInfo result = new ColorModelInfo();
			result.Rgb = value;
			return result;
		}
		protected internal bool GetBorderDiagonal(XlBorderLineStyle diagonalLineStyle) {
			return diagonalLineStyle != XlBorderLineStyle.None;
		}
		protected internal XlBorderLineStyle GetBorderDiagonalLineStyle() {
			XlBorderLineStyle result = Border.DiagonalUpLineStyle;
			if (GetBorderDiagonal(result))
				return result;
			result = Border.DiagonalDownLineStyle;
			if (GetBorderDiagonal(result))
				return result;
			return XlBorderLineStyle.None;
		}
		protected override IDocumentModel GetDocumentModel() {
			return documentModel;
		}
		public override FormatBase GetOwner() {
			return this;
		}
		internal void AssignFontIndex(int value) {
			this.fontIndex = value;
		}
		internal void AssignAlignmentIndex(int value) {
			this.alignmentIndex = value;
		}
		internal void AssignBorderIndex(int value) {
			this.borderIndex = value;
		}
		internal void AssignFillIndex(int value) {
			this.fillIndex = value;
		}
		internal void AssignGradientFillInfoIndex(int value) {
			this.gradientFillInfoIndex = value;
		}
		internal void AssignCellFormatFlagsIndex(int value) {
			this.cellFormatFlagsIndex = value;
		}
		internal void AssignNumberFormatIndex(int value) {
			this.numberFormatIndex = value;
		}
		internal void AssignGradientStopInfoCollection(GradientStopInfoCollection stops) {
			if (stops != null)
				gradientStopInfoCollection = stops;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new FormatBaseBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new FormatBaseBatchInitHelper(this);
		}
		#region ICloneable<FormatBase> Members
		public FormatBase Clone() {
			FormatBase clone = CreateEmptyClone(DocumentModel);
			CloneCore(clone);
			clone.gradientStopInfoCollection.CopyFrom(gradientStopInfoCollection);
			return clone;
		}
		public override void CopyFrom(MultiIndexObject<FormatBase, DocumentModelChangeActions> obj) {
			base.CopyFrom(obj);
			gradientStopInfoCollection.CopyFrom(((FormatBase)obj).gradientStopInfoCollection);
		}
		void ISupportsCopyFrom<FormatBase>.CopyFrom(FormatBase value) {
			CopyFrom(value as MultiIndexObject<FormatBase, DocumentModelChangeActions>);
		}
		internal virtual void CopySimple(FormatBase item) {
			this.fontIndex = item.fontIndex;
			this.alignmentIndex = item.alignmentIndex;
			this.borderIndex = item.borderIndex;
			this.fillIndex = item.fillIndex;
			this.gradientFillInfoIndex = item.gradientFillInfoIndex;
			this.cellFormatFlagsIndex = item.cellFormatFlagsIndex;
			this.numberFormatIndex = item.numberFormatIndex;
			gradientStopInfoCollection.CopyFrom(((FormatBase)item).gradientStopInfoCollection);
		}
		public abstract FormatBase CreateEmptyClone(IDocumentModel documentModel);
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			bool result = base.Equals(obj);
			if (!result)
				return false;
			FormatBase other = obj as FormatBase;
			return other != null && gradientStopInfoCollection.Equals(other.gradientStopInfoCollection);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), gradientStopInfoCollection.GetHashCode());
		}
		#endregion
		protected abstract void ClearFillOptions();
		protected abstract bool CheckClearFill();
	}
	#endregion
	#region FormatBaseBatchUpdateHelper
	public class FormatBaseBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		RunFontInfo fontInfo;
		CellAlignmentInfo alignmentInfo;
		BorderInfo borderInfo;
		FillInfo fillInfo;
		CellFormatFlagsInfo cellFormatFlagsInfo;
		NumberFormat numberFormatInfo;
		GradientFillInfo gradientFillInfo;
		int suppressDirectNotificationsCount;
		public FormatBaseBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public RunFontInfo FontInfo { get { return fontInfo; } set { fontInfo = value; } }
		public CellAlignmentInfo AlignmentInfo { get { return alignmentInfo; } set { alignmentInfo = value; } }
		public BorderInfo BorderInfo { get { return borderInfo; } set { borderInfo = value; } }
		public FillInfo FillInfo { get { return fillInfo; } set { fillInfo = value; } }
		public CellFormatFlagsInfo CellFormatFlagsInfo { get { return cellFormatFlagsInfo; } set { cellFormatFlagsInfo = value; } }
		public NumberFormat NumberFormatInfo { get { return numberFormatInfo; } set { numberFormatInfo = value; } }
		public GradientFillInfo GradientFillInfo { get { return gradientFillInfo; } set { gradientFillInfo = value; } }
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
	}
	#endregion
	#region FormatBaseBatchInitHelper
	public class FormatBaseBatchInitHelper : FormatBaseBatchUpdateHelper {
		public FormatBaseBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region ICellProtectionInfo
	public interface ICellProtectionInfo {
		bool Locked { get; set; }
		bool Hidden { get; set; }
	}
	#endregion
	#region IActualCellProtectionInfo
	public interface IActualCellProtectionInfo {
		bool Locked { get; }
		bool Hidden { get; }
	}
	#endregion
	#region CellFormatApplyOptions
	public enum CellFormatApplyOptions {
		None = 0x0,
		ApplyNumberFormat = 0x1,
		ApplyFont = 0x2,
		ApplyFill = 0x4,
		ApplyBorder = 0x8,
		ApplyAlignment = 0x10,
		ApplyProtection = 0x20, 
		ApplyStyle = 0x40
	}
	#endregion
	#region CellFormatFlagsInfo
	public struct CellFormatFlagsInfo : ICloneable<CellFormatFlagsInfo>, ISupportsSizeOf, ISupportsCopyFrom<CellFormatFlagsInfo> {
		#region Static Members
		readonly static CellFormatFlagsInfo defaultFormat = Create(CellFormatFlagsInfo.DefaultCellFormatValue);
		readonly static CellFormatFlagsInfo defaultStyle = Create(CellFormatFlagsInfo.DefaultStyleValue);
		public static CellFormatFlagsInfo DefaultFormat { get { return defaultFormat; } }
		public static CellFormatFlagsInfo DefaultStyle { get { return defaultStyle; } }
		public static CellFormatFlagsInfo Create(int value) {
			CellFormatFlagsInfo result = new CellFormatFlagsInfo();
			result.packedValues = value;
			return result;
		}
		#endregion
		#region Fields
		internal const int MaskLocked = 0x00000001;			 
		internal const int MaskHidden = 0x00000002;			 
		internal const int MaskHasExtension = 0x00000004;	   
		internal const int MaskQuotePrefix = 0x00000008;		
		internal const int MaskPivotButton = 0x00000010;		
		internal const int MaskApplyNumberFormat = 0x00000020;  
		internal const int MaskApplyFont = 0x00000040;		  
		internal const int MaskApplyFill = 0x00000080;		  
		internal const int MaskApplyBorder = 0x00000100;		
		internal const int MaskApplyAlignment = 0x00000200;	 
		internal const int MaskApplyProtection = 0x00000400;	
		internal const int MaskFillType = 0x00000800;		   
		internal const int MaskHasNumberFormat = 0x00001000;	
		internal const int MaskHasFont = 0x00002000;			
		internal const int MaskHasFill = 0x00004000;			
		internal const int MaskHasBorder = 0x00008000;		  
		internal const int MaskHasAlignment = 0x00010000;	   
		internal const int MaskHasProtection = 0x00020000;	  
		public const int DefaultStyleValue = 0x000007E1;		
		public const int DefaultCellFormatValue = 0x00000001;   
		int packedValues;
		#endregion
		#region Properties
		public int PackedValues { get { return packedValues; } set { this.packedValues = value; } }
		public bool Locked { get { return GetBooleanValue(MaskLocked); } set { SetBooleanValue(MaskLocked, value); } }
		public bool Hidden { get { return GetBooleanValue(MaskHidden); } set { SetBooleanValue(MaskHidden, value); } }
		public bool HasExtension { get { return GetBooleanValue(MaskHasExtension); } set { SetBooleanValue(MaskHasExtension, value); } }
		public bool QuotePrefix { get { return GetBooleanValue(MaskQuotePrefix); } set { SetBooleanValue(MaskQuotePrefix, value); } }
		public bool PivotButton { get { return GetBooleanValue(MaskPivotButton); } set { SetBooleanValue(MaskPivotButton, value); } }
		public bool ApplyNumberFormat { get { return GetBooleanValue(MaskApplyNumberFormat); } set { SetBooleanValue(MaskApplyNumberFormat, value); } }
		public bool ApplyFont { get { return GetBooleanValue(MaskApplyFont); } set { SetBooleanValue(MaskApplyFont, value); } }
		public bool ApplyFill { get { return GetBooleanValue(MaskApplyFill); } set { SetBooleanValue(MaskApplyFill, value); } }
		public bool ApplyBorder { get { return GetBooleanValue(MaskApplyBorder); } set { SetBooleanValue(MaskApplyBorder, value); } }
		public bool ApplyAlignment { get { return GetBooleanValue(MaskApplyAlignment); } set { SetBooleanValue(MaskApplyAlignment, value); } }
		public bool ApplyProtection { get { return GetBooleanValue(MaskApplyProtection); } set { SetBooleanValue(MaskApplyProtection, value); } }
		public ModelFillType FillType { get { return GetFillType(); } set { SetFillType(value); } }
		public bool HasNumberFormat { get { return GetBooleanValue(MaskHasNumberFormat); } set { SetBooleanValue(MaskHasNumberFormat, value); } }
		public bool HasFont { get { return GetBooleanValue(MaskHasFont); } set { SetBooleanValue(MaskHasFont, value); } }
		public bool HasFill { get { return GetBooleanValue(MaskHasFill); } set { SetBooleanValue(MaskHasFill, value); } }
		public bool HasBorder { get { return GetBooleanValue(MaskHasBorder); } set { SetBooleanValue(MaskHasBorder, value); } }
		public bool HasAlignment { get { return GetBooleanValue(MaskHasAlignment); } set { SetBooleanValue(MaskHasAlignment, value); } }
		public bool HasProtection { get { return GetBooleanValue(MaskHasProtection); } set { SetBooleanValue(MaskHasProtection, value); } }
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(int mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(int mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		ModelFillType GetFillType() {
			return GetBooleanValue(MaskFillType) ? ModelFillType.Gradient : ModelFillType.Pattern;
		}
		void SetFillType(ModelFillType type) {
			SetBooleanValue(MaskFillType, type == ModelFillType.Gradient);
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(CellFormatFlagsInfo));
		}
		#endregion
		#region ICloneable<CellFormatFlagsInfo> Members
		public CellFormatFlagsInfo Clone() {
			CellFormatFlagsInfo clone = CellFormatFlagsInfo.Create(CellFormatFlagsInfo.DefaultCellFormatValue);
			clone.PackedValues = this.PackedValues;
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<CellFormatFlagsInfo> Members
		public void CopyFrom(CellFormatFlagsInfo value) {
			this.packedValues = value.packedValues;
		}
		#endregion
		public void CopyFrom(IActualCellProtectionInfo value) {
			this.Locked = value.Locked;
			this.Hidden = value.Hidden;
		}
	}
	#endregion
	#region FontIndexAccessor
	public class FontIndexAccessor : IIndexAccessor<FormatBase, RunFontInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, RunFontInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.FontIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignFontIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, RunFontInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public RunFontInfo GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<RunFontInfo> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.FontInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseFontIndexChangeHistoryItem(owner);
		}
		public RunFontInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).FontInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, RunFontInfo info) {
			((FormatBaseBatchUpdateHelper)helper).FontInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			RunFontInfo clone = GetInfo(from).Clone();
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel))
				clone.ColorIndex = ColorModelInfo.ConvertColorIndex(from.DocumentModel.Cache.ColorModelInfoCache, clone.ColorIndex, owner.DocumentModel.Cache.ColorModelInfoCache);
			SetDeferredInfo(owner.BatchUpdateHelper, clone);
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region AlignmentIndexAccessor
	public class AlignmentIndexAccessor : IIndexAccessor<FormatBase, CellAlignmentInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, CellAlignmentInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.AlignmentIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignAlignmentIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, CellAlignmentInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public CellAlignmentInfo GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<CellAlignmentInfo> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.CellAlignmentInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseAlignmentIndexChangeHistoryItem(owner);
		}
		public CellAlignmentInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).AlignmentInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, CellAlignmentInfo info) {
			((FormatBaseBatchUpdateHelper)helper).AlignmentInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region BorderIndexAccessor
	public class BorderIndexAccessor : IIndexAccessor<FormatBase, BorderInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, CellBorderInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.BorderIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignBorderIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, BorderInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public BorderInfo GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<BorderInfo> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.BorderInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseBorderIndexChangeHistoryItem(owner);
		}
		public BorderInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).BorderInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, BorderInfo info) {
			((FormatBaseBatchUpdateHelper)helper).BorderInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			BorderInfo clone = GetInfo(from).Clone();
			if(!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				ColorModelInfoCache fromCache = from.DocumentModel.Cache.ColorModelInfoCache;
				ColorModelInfoCache ownerCache = owner.DocumentModel.Cache.ColorModelInfoCache;
				clone.LeftColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.LeftColorIndex, ownerCache);
				clone.RightColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.RightColorIndex, ownerCache);
				clone.TopColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.TopColorIndex, ownerCache);
				clone.BottomColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.BottomColorIndex, ownerCache);
				clone.DiagonalColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.DiagonalColorIndex, ownerCache);
				clone.HorizontalColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.HorizontalColorIndex, ownerCache);
				clone.VerticalColorIndex = ColorModelInfo.ConvertColorIndex(fromCache, clone.VerticalColorIndex, ownerCache);
			}
			SetDeferredInfo(owner.BatchUpdateHelper, clone);
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region NumberFormatIndexAccessor
	public class NumberFormatIndexAccessor : IIndexAccessor<FormatBase, NumberFormat, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, NumberFormat> Members
		public int GetIndex(FormatBase owner) {
			return owner.NumberFormatIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignNumberFormatIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, NumberFormat value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public NumberFormat GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<NumberFormat> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.NumberFormatCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseNumberFormatIndexChangeHistoryItem(owner);
		}
		public NumberFormat GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).NumberFormatInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, NumberFormat info) {
			((FormatBaseBatchUpdateHelper)helper).NumberFormatInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region FillIndexAccessor
	public class FillIndexAccessor : IIndexAccessor<FormatBase, FillInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, FillInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.FillIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignFillIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, FillInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public FillInfo GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<FillInfo> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.FillInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseFillIndexChangeHistoryItem(owner);
		}
		public FillInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).FillInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FillInfo info) {
			((FormatBaseBatchUpdateHelper)helper).FillInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			FillInfo clone = GetInfo(from).Clone();
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				clone.BackColorIndex = ColorModelInfo.ConvertColorIndex(from.DocumentModel.Cache.ColorModelInfoCache, clone.BackColorIndex, owner.DocumentModel.Cache.ColorModelInfoCache);
				clone.ForeColorIndex = ColorModelInfo.ConvertColorIndex(from.DocumentModel.Cache.ColorModelInfoCache, clone.ForeColorIndex, owner.DocumentModel.Cache.ColorModelInfoCache);
			}
			SetDeferredInfo(owner.BatchUpdateHelper, clone);
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region CellFormatFlagsIndexAccessor
	public class CellFormatFlagsIndexAccessor : IIndexAccessor<FormatBase, CellFormatFlagsInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, CellFormatFlagsInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.CellFormatFlagsIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignCellFormatFlagsIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, CellFormatFlagsInfo value) {
			return value.PackedValues;
		}
		public CellFormatFlagsInfo GetInfo(FormatBase owner) {
			CellFormatFlagsInfo info = CellFormatFlagsInfo.DefaultFormat;
			info.PackedValues = owner.CellFormatFlagsIndex;
			return info;
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseCellFormatFlagsIndexChangeHistoryItem(owner);
		}
		public CellFormatFlagsInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).CellFormatFlagsInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, CellFormatFlagsInfo info) {
			((FormatBaseBatchUpdateHelper)helper).CellFormatFlagsInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region GradientFillIndexAccessor
	public class GradientFillIndexAccessor : IIndexAccessor<FormatBase, GradientFillInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, GradientFillInfo> Members
		public int GetIndex(FormatBase owner) {
			return owner.GradientFillInfoIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			owner.AssignGradientFillInfoIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, GradientFillInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public GradientFillInfo GetInfo(FormatBase owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<GradientFillInfo> GetInfoCache(FormatBase owner) {
			return owner.DocumentModel.Cache.GradientFillInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new FormatBaseGradientFillIndexChangeHistoryItem(owner);
		}
		public GradientFillInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((FormatBaseBatchUpdateHelper)helper).GradientFillInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, GradientFillInfo info) {
			((FormatBaseBatchUpdateHelper)helper).GradientFillInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
			GradientStopInfoPropertyAccessor.CopyDeferredInfo(owner.GradientStopInfoCollection, from.GradientStopInfoCollection);
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region CellFormatBase
	public abstract class CellFormatBase : FormatBase, IActualFormat, IActualApplyInfo {
		protected CellFormatBase(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		protected internal virtual bool HasActualBorder { get { return ApplyBorder; } }
		protected internal virtual bool HasActualNumberFormat { get { return ApplyNumberFormat; } }
		protected internal virtual bool HasActualFill { get { return ApplyFill; } }
		protected internal virtual bool HasActualFont { get { return ApplyFont; } }
		protected internal virtual bool HasActualAlignment { get { return ApplyAlignment; } }
		protected internal virtual bool HasActualProtection { get { return ApplyProtection; } } 
		#region Applies
		#region Applies.HasExtension
		public bool HasExtension {
			get { return CellFormatFlagsInfo.HasExtension; }
			set {
				if (CellFormatFlagsInfo.HasExtension == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetHasExtensionCore, value);
			}
		}
		DocumentModelChangeActions SetHasExtensionCore(ref CellFormatFlagsInfo info, bool value) {
			info.HasExtension = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.QuotePrefix
		public bool QuotePrefix {
			get { return CellFormatFlagsInfo.QuotePrefix; }
			set {
				if (CellFormatFlagsInfo.QuotePrefix == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetQuotePrefixCore, value);
			}
		}
		DocumentModelChangeActions SetQuotePrefixCore(ref CellFormatFlagsInfo info, bool value) {
			info.QuotePrefix = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.PivotButton
		public bool PivotButton {
			get { return CellFormatFlagsInfo.PivotButton; }
			set {
				if (CellFormatFlagsInfo.PivotButton == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetPivotButtonCore, value);
			}
		}
		DocumentModelChangeActions SetPivotButtonCore(ref CellFormatFlagsInfo info, bool value) {
			info.PivotButton = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyNumberFormat
		public bool ApplyNumberFormat {
			get { return CellFormatFlagsInfo.ApplyNumberFormat; }
			set {
				if (CellFormatFlagsInfo.ApplyNumberFormat == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyNumberFormat, value);
			}
		}
		DocumentModelChangeActions SetApplyNumberFormat(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyNumberFormat = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFont
		public bool ApplyFont {
			get { return CellFormatFlagsInfo.ApplyFont; }
			set {
				if (CellFormatFlagsInfo.ApplyFont == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyFont, value);
			}
		}
		DocumentModelChangeActions SetApplyFont(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyFont = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyFill
		public bool ApplyFill {
			get { return CellFormatFlagsInfo.ApplyFill; }
			set {
				if (CellFormatFlagsInfo.ApplyFill == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyFill, value);
			}
		}
		DocumentModelChangeActions SetApplyFill(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyFill = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyBorder
		public bool ApplyBorder {
			get { return CellFormatFlagsInfo.ApplyBorder; }
			set {
				if (CellFormatFlagsInfo.ApplyBorder == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyBorder, value);
			}
		}
		DocumentModelChangeActions SetApplyBorder(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyBorder = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyAlignment
		public bool ApplyAlignment {
			get { return CellFormatFlagsInfo.ApplyAlignment; }
			set {
				if (CellFormatFlagsInfo.ApplyAlignment == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyAlignment, value);
			}
		}
		DocumentModelChangeActions SetApplyAlignment(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyAlignment = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#region Applies.ApplyProtection
		public bool ApplyProtection {
			get { return CellFormatFlagsInfo.ApplyProtection; }
			set {
				if (CellFormatFlagsInfo.ApplyProtection == value)
					return;
				SetPropertyValueForStruct(CellFormatFlagsIndexAccessor, SetApplyProtection, value);
			}
		}
		DocumentModelChangeActions SetApplyProtection(ref CellFormatFlagsInfo info, bool value) {
			info.ApplyProtection = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		#endregion 
		#region IActualFormat Members
		public IActualRunFontInfo ActualFont { get { return this; } }
		public IActualCellAlignmentInfo ActualAlignment { get { return this; } }
		public IActualBorderInfo ActualBorder { get { return this; } }
		public IActualFillInfo ActualFill { get { return this; } }
		public IActualCellProtectionInfo ActualProtection { get { return this; } }
		#endregion
		#region IActualCellAlignmentInfo Members
		bool IActualCellAlignmentInfo.WrapText { get { return GetActualAlignmentWrapText(); } }
		protected virtual bool GetActualAlignmentWrapText() {
			return AlignmentInfo.WrapText;
		}
		bool IActualCellAlignmentInfo.JustifyLastLine { get { return GetActualAlignmentJustifyLastLine(); } }
		protected virtual bool GetActualAlignmentJustifyLastLine() {
			return AlignmentInfo.JustifyLastLine;
		}
		bool IActualCellAlignmentInfo.ShrinkToFit { get { return GetActualAlignmentShrinkToFit(); } }
		protected virtual bool GetActualAlignmentShrinkToFit() {
			return AlignmentInfo.ShrinkToFit;
		}
		int IActualCellAlignmentInfo.TextRotation { get { return GetActualAlignmentTextRotation(); } }
		protected virtual int GetActualAlignmentTextRotation() {
			return AlignmentInfo.TextRotation;
		}
		byte IActualCellAlignmentInfo.Indent { get { return GetActualAlignmentIndent(); } }
		protected virtual byte GetActualAlignmentIndent() {
			return AlignmentInfo.Indent;
		}
		int IActualCellAlignmentInfo.RelativeIndent { get { return GetActualAlignmentRelativeIndent(); } }
		protected virtual int GetActualAlignmentRelativeIndent() {
			return AlignmentInfo.RelativeIndent;
		}
		XlHorizontalAlignment IActualCellAlignmentInfo.Horizontal { get { return GetActualAlignmentHorizontal(); } }
		protected virtual XlHorizontalAlignment GetActualAlignmentHorizontal() {
			return AlignmentInfo.HorizontalAlignment;
		}
		XlVerticalAlignment IActualCellAlignmentInfo.Vertical { get { return GetActualAlignmentVertical(); } }
		protected virtual XlVerticalAlignment GetActualAlignmentVertical() {
			return AlignmentInfo.VerticalAlignment;
		}
		XlReadingOrder IActualCellAlignmentInfo.ReadingOrder { get { return GetActualAlignmentReadingOrder(); } }
		protected virtual XlReadingOrder GetActualAlignmentReadingOrder() {
			return AlignmentInfo.ReadingOrder;
		}
		#endregion
		#region IActualRunFontInfo Members
		string IActualRunFontInfo.Name { get { return GetActualFontName(); } }
		protected virtual string GetActualFontName() {
			string result = DocumentModel.OfficeTheme.FontScheme.GetTypeface(GetActualFontSchemeStyle(), DocumentModel.Culture);
			return String.IsNullOrEmpty(result) ? FontInfo.Name : result;
		}
		Color IActualRunFontInfo.Color { get { return GetActualFontColor(); } }
		protected virtual Color GetActualFontColor() {
			return GetColor(FontInfo.ColorIndex);
		}
		bool IActualRunFontInfo.Bold { get { return GetActualFontBold(); } }
		protected virtual bool GetActualFontBold() {
			return FontInfo.Bold;
		}
		bool IActualRunFontInfo.Condense { get { return GetActualFontCondense(); } }
		protected virtual bool GetActualFontCondense() {
			return FontInfo.Condense;
		}
		bool IActualRunFontInfo.Extend { get { return GetActualFontExtend(); } }
		protected virtual bool GetActualFontExtend() {
			return FontInfo.Extend;
		}
		bool IActualRunFontInfo.Italic { get { return GetActualFontItalic(); } }
		protected virtual bool GetActualFontItalic() {
			return FontInfo.Italic;
		}
		bool IActualRunFontInfo.Outline { get { return GetActualFontOutline(); } }
		protected virtual bool GetActualFontOutline() {
			return FontInfo.Outline;
		}
		bool IActualRunFontInfo.Shadow { get { return GetActualFontShadow(); } }
		protected virtual bool GetActualFontShadow() {
			return FontInfo.Shadow;
		}
		bool IActualRunFontInfo.StrikeThrough { get { return GetActualFontStrikeThrough(); } }
		protected virtual bool GetActualFontStrikeThrough() {
			return FontInfo.StrikeThrough;
		}
		int IActualRunFontInfo.Charset { get { return GetActualFontCharset(); } }
		protected virtual int GetActualFontCharset() {
			return FontInfo.Charset;
		}
		int IActualRunFontInfo.FontFamily { get { return GetActualFontFamily(); } }
		protected virtual int GetActualFontFamily() {
			return FontInfo.FontFamily;
		}
		double IActualRunFontInfo.Size { get { return GetActualFontSize(); } }
		protected virtual double GetActualFontSize() {
			return FontInfo.Size;
		}
		XlFontSchemeStyles IActualRunFontInfo.SchemeStyle { get { return GetActualFontSchemeStyle(); } }
		protected virtual XlFontSchemeStyles GetActualFontSchemeStyle() {
			return FontInfo.SchemeStyle;
		}
		XlScriptType IActualRunFontInfo.Script { get { return GetActualFontScript(); } }
		protected virtual XlScriptType GetActualFontScript() {
			return FontInfo.Script;
		}
		XlUnderlineType IActualRunFontInfo.Underline { get { return GetActualFontUnderline(); } }
		protected virtual XlUnderlineType GetActualFontUnderline() {
			return FontInfo.Underline;
		}
		int IActualRunFontInfo.ColorIndex { get { return GetActualFontColorIndex(); } }
		protected virtual int GetActualFontColorIndex() {
			return FontInfo.ColorIndex;
		}
		FontInfo IActualRunFontInfo.GetFontInfo() {
			return GetActualFontInfo();
		}
		protected virtual FontInfo GetActualFontInfo() {
			return FontIndexAccessor.GetInfo(this).GetFontInfo(DocumentModel.FontCache);
		}
		#endregion
		#region IActualBorderInfo Members
		XlBorderLineStyle IActualBorderInfo.LeftLineStyle { get { return GetActualBorderLeftLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderLeftLineStyle() {
			return BorderInfo.LeftLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.RightLineStyle { get { return GetActualBorderRightLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderRightLineStyle() {
			return BorderInfo.RightLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.TopLineStyle { get { return GetActualBorderTopLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderTopLineStyle() {
			return BorderInfo.TopLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.BottomLineStyle { get { return GetActualBorderBottomLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderBottomLineStyle() {
			return BorderInfo.BottomLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.HorizontalLineStyle { get { return GetActualBorderHorizontalLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			return BorderInfo.HorizontalLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.VerticalLineStyle { get { return GetActualBorderVerticalLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			return BorderInfo.VerticalLineStyle;
		}
		XlBorderLineStyle IActualBorderInfo.DiagonalUpLineStyle { get { return GetActualBorderDiagonalUpLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			return BorderInfo.DiagonalUp ? BorderInfo.DiagonalLineStyle : XlBorderLineStyle.None;
		}
		XlBorderLineStyle IActualBorderInfo.DiagonalDownLineStyle { get { return GetActualBorderDiagonalDownLineStyle(); } }
		protected virtual XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			return BorderInfo.DiagonalDown ? BorderInfo.DiagonalLineStyle : XlBorderLineStyle.None;
		}
		Color IActualBorderInfo.LeftColor { get { return GetActualBorderLeftColor(); } }
		protected virtual Color GetActualBorderLeftColor() {
			return GetColor(BorderInfo.LeftColorIndex);
		}
		Color IActualBorderInfo.RightColor { get { return GetActualBorderRightColor(); } }
		protected virtual Color GetActualBorderRightColor() {
			return GetColor(BorderInfo.RightColorIndex);
		}
		Color IActualBorderInfo.TopColor { get { return GetActualBorderTopColor(); } }
		protected virtual Color GetActualBorderTopColor() {
			return GetColor(BorderInfo.TopColorIndex);
		}
		Color IActualBorderInfo.BottomColor { get { return GetActualBorderBottomColor(); } }
		protected virtual Color GetActualBorderBottomColor() {
			return GetColor(BorderInfo.BottomColorIndex);
		}
		Color IActualBorderInfo.HorizontalColor { get { return GetActualBorderHorizontalColor(); } }
		protected virtual Color GetActualBorderHorizontalColor() {
			return GetColor(BorderInfo.HorizontalColorIndex);
		}
		Color IActualBorderInfo.VerticalColor { get { return GetActualBorderVerticalColor(); } }
		protected virtual Color GetActualBorderVerticalColor() {
			return GetColor(BorderInfo.VerticalColorIndex);
		}
		Color IActualBorderInfo.DiagonalColor { get { return GetActualBorderDiagonalColor(); } }
		protected virtual Color GetActualBorderDiagonalColor() {
			return GetColor(BorderInfo.DiagonalColorIndex);
		}
		int IActualBorderInfo.LeftColorIndex { get { return GetActualBorderLeftColorIndex(); } }
		protected virtual int GetActualBorderLeftColorIndex() {
			return BorderInfo.LeftColorIndex;
		}
		int IActualBorderInfo.RightColorIndex { get { return GetActualBorderRightColorIndex(); } }
		protected virtual int GetActualBorderRightColorIndex() {
			return BorderInfo.RightColorIndex;
		}
		int IActualBorderInfo.TopColorIndex { get { return GetActualBorderTopColorIndex(); } }
		protected virtual int GetActualBorderTopColorIndex() {
			return BorderInfo.TopColorIndex;
		}
		int IActualBorderInfo.BottomColorIndex { get { return GetActualBorderBottomColorIndex(); } }
		protected virtual int GetActualBorderBottomColorIndex() {
			return BorderInfo.BottomColorIndex;
		}
		int IActualBorderInfo.HorizontalColorIndex { get { return GetActualBorderHorizontalColorIndex(); } }
		protected virtual int GetActualBorderHorizontalColorIndex() {
			return BorderInfo.HorizontalColorIndex;
		}
		int IActualBorderInfo.VerticalColorIndex { get { return GetActualBorderVerticalColorIndex(); } }
		protected virtual int GetActualBorderVerticalColorIndex() {
			return BorderInfo.VerticalColorIndex;
		}
		int IActualBorderInfo.DiagonalColorIndex { get { return GetActualBorderDiagonalColorIndex(); } }
		protected virtual int GetActualBorderDiagonalColorIndex() {
			return BorderInfo.DiagonalColorIndex;
		}
		bool IActualBorderInfo.Outline { get { return GetActualBorderOutline(); } }
		protected virtual bool GetActualBorderOutline() {
			return BorderInfo.Outline;
		}
		#endregion
		#region IActualFillInfo Members
		XlPatternType IActualFillInfo.PatternType { get { return GetFillActualPatternType(); } }
		protected virtual XlPatternType GetFillActualPatternType() {
			return FillInfo.PatternType;
		}
		Color IActualFillInfo.ForeColor { get { return GetFillActualForeColor(); } }
		protected virtual Color GetFillActualForeColor() {
			return GetColor(GetFillActualForeColorIndex());
		}
		Color IActualFillInfo.BackColor { get { return GetFillActualBackColor(); } }
		protected virtual Color GetFillActualBackColor() {
			return GetColor(GetFillActualBackColorIndex());
		}
		int IActualFillInfo.ForeColorIndex { get { return GetFillActualForeColorIndex(); } }
		protected virtual int GetFillActualForeColorIndex() {
			return FillInfo.ForeColorIndex;
		}
		int IActualFillInfo.BackColorIndex { get { return GetFillActualBackColorIndex(); } }
		protected virtual int GetFillActualBackColorIndex() {
			return FillInfo.BackColorIndex;
		}
		bool IActualFillInfo.ApplyPatternType { get { return true; } }
		bool IActualFillInfo.ApplyForeColor { get { return true; } }
		bool IActualFillInfo.ApplyBackColor { get { return true; } }
		bool IActualFillInfo.IsDifferential { get { return false; } }
		IActualGradientFillInfo IActualFillInfo.GradientFill { get { return this; } }
		ModelFillType IActualFillInfo.FillType { get { return GetFillActualFillType(); } }
		protected virtual ModelFillType GetFillActualFillType() {
			return CellFormatFlagsInfo.FillType;
		}
		ModelGradientFillType IActualGradientFillInfo.Type { get { return GetFillActualGradientFillType(); } }
		protected virtual ModelGradientFillType GetFillActualGradientFillType() {
			return GradientFillInfo.Type;
		}
		double IActualGradientFillInfo.Degree { get { return GetFillActualGradientFillDegree(); } }
		protected virtual double GetFillActualGradientFillDegree() {
			return GradientFillInfo.Degree;
		}
		IActualConvergenceInfo IActualGradientFillInfo.Convergence { get { return this; } }
		IActualGradientStopCollection IActualGradientFillInfo.GradientStops { get { return GetFillActualGradientStops(); } }
		protected virtual IActualGradientStopCollection GetFillActualGradientStops() {
			return GradientStopInfoCollection;
		}
		float IActualConvergenceInfo.Left { get { return GetFillActualGradientFillConvergenceLeft(); } }
		protected virtual float GetFillActualGradientFillConvergenceLeft() {
			return GradientFillInfo.Left;
		}
		float IActualConvergenceInfo.Right { get { return GetFillActualGradientFillConvergenceRight(); } }
		protected virtual float GetFillActualGradientFillConvergenceRight() {
			return GradientFillInfo.Right;
		}
		float IActualConvergenceInfo.Top { get { return GetFillActualGradientFillConvergenceTop(); } }
		protected virtual float GetFillActualGradientFillConvergenceTop() {
			return GradientFillInfo.Top;
		}
		float IActualConvergenceInfo.Bottom { get { return GetFillActualGradientFillConvergenceBottom(); } }
		protected virtual float GetFillActualGradientFillConvergenceBottom() {
			return GradientFillInfo.Bottom;
		}
		#endregion
		#region IActualCellProtectionInfo Members
		bool IActualCellProtectionInfo.Locked { get { return GetActualProtectionLocked(); } }
		protected virtual bool GetActualProtectionLocked() {
			return CellFormatFlagsInfo.Locked;
		}
		bool IActualCellProtectionInfo.Hidden { get { return GetActualProtectionHidden(); } }
		protected virtual bool GetActualProtectionHidden() {
			return CellFormatFlagsInfo.Hidden;
		}
		#endregion
		#region ActualFormatString Members
		public string ActualFormatString { get { return GetActualFormatString(); } }
		protected virtual string GetActualFormatString() {
			return NumberFormatInfo.FormatCode;
		}
		#endregion
		#region ActualFormatIndex Members
		public int ActualFormatIndex { get { return GetActualFormatIndex(); } }
		protected virtual int GetActualFormatIndex() {
			return NumberFormatIndex;
		}
		#endregion
		#endregion
		protected override void SetAlignmentCore() {
			ApplyAlignment = true;
		}
		protected override void SetFontCore() {
			ApplyFont = true;
		}
		protected override void SetFontNameCore(RunFontInfo info) {
			info.SchemeStyle = XlFontSchemeStyles.None;
		}
		protected override void SetBorderCore() {
			ApplyBorder = true;
		}
		protected override void SetFillCore() {
			ApplyFill = true;
		}
		protected override void SetFillCore(ref CellFormatFlagsInfo info) {
			info.ApplyFill = true;
		}
		protected override void SetNumberFormatCore() {
			ApplyNumberFormat = true;
		}
		protected override void SetProtectionCore(ref CellFormatFlagsInfo info) {
			info.ApplyProtection = true;
		}
		#region IActualApplyInfo Members
		public IActualApplyInfo ActualApplyInfo { get { return this; } }
		bool IActualApplyInfo.ApplyFont { get { return GetActualApplyFont(); } }
		bool IActualApplyInfo.ApplyFill { get { return GetActualApplyFill(); } }
		bool IActualApplyInfo.ApplyBorder { get { return GetActualApplyBorder(); } }
		bool IActualApplyInfo.ApplyAlignment { get { return GetActualApplyAlignment(); } }
		bool IActualApplyInfo.ApplyProtection { get { return GetActualApplyProtection(); } }
		bool IActualApplyInfo.ApplyNumberFormat { get { return GetActualApplyNumberFormat(); } }
		#endregion
		protected virtual bool GetActualApplyFont() {
			return ApplyFont;
		}
		protected virtual bool GetActualApplyFill() {
			return ApplyFill;
		}
		protected virtual bool GetActualApplyBorder() {
			return ApplyBorder;
		}
		protected virtual bool GetActualApplyAlignment() {
			return ApplyAlignment;
		}
		protected virtual bool GetActualApplyProtection() {
			return ApplyProtection;
		}
		protected virtual bool GetActualApplyNumberFormat() {
			return ApplyNumberFormat;
		}
		protected void CopyFromInfo<TInfo>(IIndexAccessor<FormatBase, TInfo, DocumentModelChangeActions> cellFormatIndexHolder, TInfo sourceInfo) where TInfo : class, ICloneable<TInfo>, ISupportsCopyFrom<TInfo>, ISupportsSizeOf {
			if (IsUpdateLocked)
				cellFormatIndexHolder.GetDeferredInfo(BatchUpdateHelper).CopyFrom(sourceInfo);
			else
				ReplaceInfo(cellFormatIndexHolder, sourceInfo, DocumentModelChangeActions.None);
		}
		protected void CopyFromFormatProtection(CellFormatFlagsInfo targetInfo, CellFormatFlagsInfo sourceInfo) {
			targetInfo.Hidden = sourceInfo.Hidden;
			targetInfo.Locked = sourceInfo.Locked;
			ReplaceInfoForFlags(targetInfo);
		}
		protected void CopyFromFormatFillType(CellFormatFlagsInfo targetInfo, ModelFillType fillType) {
			targetInfo.FillType = fillType;
			ReplaceInfoForFlags(targetInfo);
		}
		void ReplaceInfoForFlags(CellFormatFlagsInfo info) {
			ReplaceInfoForFlags(CellFormat.CellFormatFlagsIndexAccessor, info, DocumentModelChangeActions.None);
		}
		protected override void ClearFillOptions() {
			CellFormatFlagsInfo info = GetInfoForModification(CellFormatFlagsIndexAccessor);
			info.ApplyFill = false;
			info.FillType = ModelFillType.Pattern;
			ReplaceInfoForFlags(CellFormatFlagsIndexAccessor, info, DocumentModelChangeActions.None);
		}
		protected override bool CheckClearFill() {
			return CellFormatFlagsInfo.ApplyFill;
		}
		#region ApplyFormat
		protected internal void ApplyFormat(CellFormatBase format) {
			if (!Object.ReferenceEquals(DocumentModel, format.DocumentModel))
				return;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					ApplyNumberFormatCore(format, format.ApplyNumberFormat, true);
					ApplyFontCore(format, format.ApplyFont, true);
					ApplyFillCore(format, format.ApplyFill, true);
					ApplyBorderCore(format, format.ApplyBorder, true);
					ApplyAlignmentCore(format, format.ApplyAlignment, true);
					ApplyProtectionCore(format, format.ApplyProtection, true);
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ApplyNumberFormatCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyNumberFormat = apply;
				CopyNumberFormat(format.NumberFormatInfo);
			}
		}
		protected internal void ApplyFontCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyFont = apply;
				CopyFont(format.FontInfo);
			}
		}
		protected internal void ApplyFillCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyFill = apply;
				CopyFill(format);
			}
		}
		protected internal void ApplyBorderCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyBorder = apply;
				CopyBorder(format.BorderInfo);
			}
		}
		protected internal void ApplyAlignmentCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyAlignment = apply;
				CopyAlignment(format.AlignmentInfo);
			}
		}
		protected internal void ApplyProtectionCore(CellFormatBase format, bool hasValue, bool apply) {
			if (hasValue) {
				ApplyProtection = apply;
				CopyProtection(format);
			}
		}
		protected internal void CopyNumberFormat(NumberFormat info) {
			CopyFromInfo(NumberFormatIndexAccessor, info);
		}
		protected internal void CopyFont(RunFontInfo info) {
			CopyFromInfo(FontIndexAccessor, info);
		}
		protected internal void CopyBorder(BorderInfo info) {
			CopyFromInfo(BorderIndexAccessor, info);
		}
		protected internal void CopyAlignment(CellAlignmentInfo alignmentInfo) {
			CopyFromInfo(AlignmentIndexAccessor, alignmentInfo);
		}
		protected internal void CopyFill(CellFormatBase format) {
			CopyFromInfo(FillIndexAccessor, format.FillInfo);
			CopyFromInfo(GradientFillIndexAccessor, format.GradientFillInfo);
			GradientStopInfoCollection gradientStops = format.GradientStopInfoCollection;
			if (gradientStops.Count > 0)
				Fill.GradientFill.GradientStops.CopyFrom(gradientStops);
			else
				Fill.GradientFill.GradientStops.Clear();
			CopyFromFormatFillType(CellFormatFlagsInfo.Clone(), format.CellFormatFlagsInfo.FillType);
		}
		protected internal void CopyProtection(CellFormatBase format) {
			CopyFromFormatProtection(CellFormatFlagsInfo.Clone(), format.CellFormatFlagsInfo);
		}
		#endregion
		#region HasVisibleFill
		protected bool HasGradientFill { get { return CellFormatFlagsInfo.FillType == ModelFillType.Gradient; } }
		protected bool HasVisiblePatternFill { get { return FillInfo.HasVisible(DocumentModel, false); } }
		protected internal virtual bool HasVisibleActualFill { get { return HasGradientFill || HasVisiblePatternFill; } }
		#endregion
	}
	#endregion
	#region StyleInfo
	public struct StyleInfo : ICloneable<StyleInfo>, ISupportsSizeOf { 
		int styleIndex;
		public int StyleIndex { get { return styleIndex; } set { this.styleIndex = value; } }
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(typeof(StyleInfo));
		}
		#endregion
		#region ICloneable<CellFormatFlagsInfo> Members
		public StyleInfo Clone() {
			StyleInfo clone = new StyleInfo();
			clone.StyleIndex = this.StyleIndex;
			return clone;
		}
		#endregion
	}
	#endregion
	#region StyleInfoIndexAccessor
	public class StyleInfoIndexAccessor : IIndexAccessor<FormatBase, StyleInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<FormatBase, StyleIndexInfo> Members
		public int GetIndex(FormatBase owner) {
			return ((CellFormat)owner).StyleIndex;
		}
		public int GetDeferredInfoIndex(FormatBase owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(FormatBase owner, int value) {
			((CellFormat)owner).AssignStyleIndex(value);
		}
		public int GetInfoIndex(FormatBase owner, StyleInfo value) {
			return value.StyleIndex;
		}
		public StyleInfo GetInfo(FormatBase owner) {
			StyleInfo info = new StyleInfo();
			info.StyleIndex = ((CellFormat)owner).StyleIndex;
			return info;
		}
		public bool IsIndexValid(FormatBase owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(FormatBase owner) {
			return new CellFormatStyleInfoIndexChangeHistoryItem((CellFormat)owner);
		}
		public StyleInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((CellFormatBatchUpdateHelper)helper).StyleInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, StyleInfo info) {
			CellFormatBatchUpdateHelper helperCasted = helper as CellFormatBatchUpdateHelper;
			helperCasted.StyleInfo = info.Clone();
		}
		public void InitializeDeferredInfo(FormatBase owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(FormatBase owner, FormatBase from) {
			StyleInfo source = GetInfo(from);
			CellStyleBase sourceStyle = from.DocumentModel.StyleSheet.CellStyles[source.StyleIndex];
			if (!Object.ReferenceEquals(owner.DocumentModel, from.DocumentModel)) {
				StyleInfo result = new StyleInfo();
				string styleName = sourceStyle.Name;
				int targetExistingStyle = owner.DocumentModel.StyleSheet.CellStyles.GetCellStyleIndexByName(styleName);
				if (targetExistingStyle == -1) {
					CellStyleBase newStyle = null;
					newStyle = sourceStyle.Clone(owner.DocumentModel, sourceStyle.Name);
					owner.DocumentModel.StyleSheet.CellStyles.Add(newStyle);
					result.StyleIndex = newStyle.StyleIndex;
				}
				else
					result.StyleIndex = targetExistingStyle;
				SetDeferredInfo(owner.BatchUpdateHelper, result);
				return;
			}
			SetDeferredInfo(owner.BatchUpdateHelper, source);
		}
		public bool ApplyDeferredChanges(FormatBase owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region CellFormatBatchUpdateHelper
	public class CellFormatBatchUpdateHelper : FormatBaseBatchUpdateHelper {
		StyleInfo styleInfo;
		public CellFormatBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
		}
		public StyleInfo StyleInfo { get { return styleInfo; } set { styleInfo = value; } }
	}
	#endregion
	#region CellFormatBatchInitHelper
	public class CellFormatBatchInitHelper : CellFormatBatchUpdateHelper {
		public CellFormatBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region CellFormat
	public class CellFormat : CellFormatBase, ICellFormat, ISupportsSizeOf, ICloneable<CellFormat>, IFormatBaseBatchUpdateable {
		#region Static Members
		readonly static StyleInfoIndexAccessor styleInfoIndexAccessor = new StyleInfoIndexAccessor();
		readonly static IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] indexAccessors = new IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] {
			FontIndexAccessor,
			AlignmentIndexAccessor,
			BorderIndexAccessor,
			FillIndexAccessor,
			GradientFillIndexAccessor, 
			CellFormatFlagsIndexAccessor,
			NumberFormatIndexAccessor, 
			styleInfoIndexAccessor
		};
		public static StyleInfoIndexAccessor StyleInfoIndexAccessor { get { return styleInfoIndexAccessor; } }
		#endregion
		int styleIndex;
		public CellFormat(DocumentModel documentModel)
			: base(documentModel) {
		}
		#region Properties
		internal int StyleIndex { get { return styleIndex; } }
		internal new CellFormatBatchUpdateHelper BatchUpdateHelper { get { return (CellFormatBatchUpdateHelper)base.BatchUpdateHelper; } }
		protected override IIndexAccessorBase<FormatBase, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		StyleInfo StyleInfo { get { return IsUpdateLocked ? BatchUpdateHelper.StyleInfo : StyleInfoCore; } }
		StyleInfo StyleInfoCore { get { return StyleInfoIndexAccessor.GetInfo(this); } }
		CellStyleFormat StyleFormat { get { return (CellStyleFormat)GetCellStyle(StyleIndex).FormatInfo; } }
		IActualFillInfo StyleActualFill { get { return StyleFormat.ActualFill; } }
		IActualRunFontInfo StyleActualFont { get { return StyleFormat.ActualFont; } }
		IActualCellAlignmentInfo StyleActualAlignment { get { return StyleFormat.ActualAlignment; } }
		IActualBorderInfo StyleActualBorder { get { return StyleFormat.ActualBorder; } }
		IActualCellProtectionInfo StyleActualProtection { get { return StyleFormat.ActualProtection; } }
		IActualApplyInfo StyleActualApplyInfo { get { return StyleFormat.ActualApplyInfo; } }
		protected internal override bool HasActualBorder { get { return base.HasActualBorder || !StyleFormat.ApplyBorder; } }
		protected internal override bool HasActualNumberFormat { get { return base.HasActualNumberFormat || !StyleFormat.ApplyNumberFormat; } }
		protected internal override bool HasActualFill { get { return base.HasActualFill || !StyleFormat.ApplyFill; } }
		protected internal override bool HasActualFont { get { return base.HasActualFont || !StyleFormat.ApplyFont; } }
		protected internal override bool HasActualAlignment { get { return base.HasActualAlignment || !StyleFormat.ApplyAlignment; } }
		protected internal override bool HasActualProtection { get { return base.HasActualProtection || !StyleFormat.ApplyProtection; } }
		protected override FillInfo DefaultFillInfo { get { return DocumentModel.StyleSheet.DefaultCellFormat.FillInfo; } }
		protected override GradientFillInfo DefaultGradientFillInfo { get { return DocumentModel.StyleSheet.DefaultCellFormat.GradientFillInfo; } }
		#endregion
		#region StyleIndex
		public CellStyleBase Style {
			get { return GetCellStyle(StyleInfo.StyleIndex); }
			set {
				if (GetCellStyle(StyleInfo.StyleIndex) == value)
					return;
				SetPropertyValueForStruct(StyleInfoIndexAccessor, SetStyleIndexCore, GetValidateCellStyle(value));
			}
		}
		DocumentModelChangeActions SetStyleIndexCore(ref StyleInfo info, CellStyleBase value) {
			info.StyleIndex = value.StyleIndex;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		CellStyleBase GetValidateCellStyle(CellStyleBase cellStyle) {
			if (cellStyle == null)
				return DocumentModel.StyleSheet.CellStyles.GetCellStyleByName("Normal");
			if (!cellStyle.IsRegistered)
				Exceptions.ThrowInternalException();
			return cellStyle;
		}
		CellStyleBase GetCellStyle(int index) {
			return DocumentModel.StyleSheet.CellStyles[index];
		}
		#region IActualRunFontInfo implementation
		protected override string GetActualFontName() {
			if (HasActualFont)
				return base.GetActualFontName();
			else
				return StyleActualFont.Name;
		}
		protected override Color GetActualFontColor() {
			if (HasActualFont)
				return base.GetActualFontColor();
			else
				return StyleActualFont.Color;
		}
		protected override bool GetActualFontBold() {
			if (HasActualFont)
				return base.GetActualFontBold();
			else
				return StyleActualFont.Bold;
		}
		protected override bool GetActualFontCondense() {
			if (HasActualFont)
				return base.GetActualFontCondense();
			else
				return StyleActualFont.Condense;
		}
		protected override bool GetActualFontExtend() {
			if (HasActualFont)
				return base.GetActualFontExtend();
			else
				return StyleActualFont.Extend;
		}
		protected override bool GetActualFontItalic() {
			if (HasActualFont)
				return base.GetActualFontItalic();
			else
				return StyleActualFont.Italic;
		}
		protected override bool GetActualFontOutline() {
			if (HasActualFont)
				return base.GetActualFontOutline();
			else
				return StyleActualFont.Outline;
		}
		protected override bool GetActualFontShadow() {
			if (HasActualFont)
				return base.GetActualFontShadow();
			else
				return StyleActualFont.Shadow;
		}
		protected override bool GetActualFontStrikeThrough() {
			if (HasActualFont)
				return base.GetActualFontStrikeThrough();
			else
				return StyleActualFont.StrikeThrough;
		}
		protected override int GetActualFontCharset() {
			if (HasActualFont)
				return base.GetActualFontCharset();
			else
				return StyleActualFont.Charset;
		}
		protected override int GetActualFontFamily() {
			if (HasActualFont)
				return base.GetActualFontFamily();
			else
				return StyleActualFont.FontFamily;
		}
		protected override double GetActualFontSize() {
			if (HasActualFont)
				return base.GetActualFontSize();
			else
				return StyleActualFont.Size;
		}
		protected override XlFontSchemeStyles GetActualFontSchemeStyle() {
			if (HasActualFont)
				return base.GetActualFontSchemeStyle();
			else
				return StyleActualFont.SchemeStyle;
		}
		protected override XlScriptType GetActualFontScript() {
			if (HasActualFont)
				return base.GetActualFontScript();
			else
				return StyleActualFont.Script;
		}
		protected override XlUnderlineType GetActualFontUnderline() {
			if (HasActualFont)
				return base.GetActualFontUnderline();
			else
				return StyleActualFont.Underline;
		}
		protected override int GetActualFontColorIndex() {
			if (HasActualFont)
				return base.GetActualFontColorIndex();
			else
				return StyleActualFont.ColorIndex;
		}
		protected override FontInfo GetActualFontInfo() {
			if (HasActualFont)
				return base.GetActualFontInfo();
			else
				return StyleActualFont.GetFontInfo();
		}
		#endregion
		#region IActualFillInfo implementation
		protected override XlPatternType GetFillActualPatternType() {
			if (HasActualFill)
				return base.GetFillActualPatternType();
			else
				return StyleActualFill.PatternType;
		}
		protected override Color GetFillActualForeColor() {
			if (HasActualFill)
				return base.GetFillActualForeColor();
			else
				return StyleActualFill.ForeColor;
		}
		protected override Color GetFillActualBackColor() {
			if (HasActualFill)
				return base.GetFillActualBackColor();
			else
				return StyleActualFill.BackColor;
		}
		protected override int GetFillActualForeColorIndex() {
			if (HasActualFill)
				return base.GetFillActualForeColorIndex();
			else
				return StyleActualFill.ForeColorIndex;
		}
		protected override int GetFillActualBackColorIndex() {
			if (HasActualFill)
				return base.GetFillActualBackColorIndex();
			else
				return StyleActualFill.BackColorIndex;
		}
		protected override ModelFillType GetFillActualFillType() {
			if (HasActualFill)
				return base.GetFillActualFillType();
			else
				return StyleActualFill.FillType;
		}
		protected override double GetFillActualGradientFillDegree() {
			if (HasActualFill)
				return base.GetFillActualGradientFillDegree();
			else
				return StyleActualFill.GradientFill.Degree;
		}
		protected override ModelGradientFillType GetFillActualGradientFillType() {
			if (HasActualFill)
				return base.GetFillActualGradientFillType();
			else
				return StyleActualFill.GradientFill.Type;
		}
		protected override IActualGradientStopCollection GetFillActualGradientStops() {
			if (HasActualFill)
				return base.GetFillActualGradientStops();
			else
				return StyleActualFill.GradientFill.GradientStops;
		}
		protected override float GetFillActualGradientFillConvergenceLeft() {
			if (HasActualFill)
				return base.GetFillActualGradientFillConvergenceLeft();
			else
				return StyleActualFill.GradientFill.Convergence.Left;
		}
		protected override float GetFillActualGradientFillConvergenceRight() {
			if (HasActualFill)
				return base.GetFillActualGradientFillConvergenceRight();
			else
				return StyleActualFill.GradientFill.Convergence.Right;
		}
		protected override float GetFillActualGradientFillConvergenceTop() {
			if (HasActualFill)
				return base.GetFillActualGradientFillConvergenceTop();
			else
				return StyleActualFill.GradientFill.Convergence.Top;
		}
		protected override float GetFillActualGradientFillConvergenceBottom() {
			if (HasActualFill)
				return base.GetFillActualGradientFillConvergenceBottom();
			else
				return StyleActualFill.GradientFill.Convergence.Bottom;
		}
		#endregion
		#region IActualCellAlignmentInfo implementation
		protected override bool GetActualAlignmentWrapText() {
			if (HasActualAlignment)
				return base.GetActualAlignmentWrapText();
			else
				return StyleActualAlignment.WrapText;
		}
		protected override bool GetActualAlignmentJustifyLastLine() {
			if (HasActualAlignment)
				return base.GetActualAlignmentJustifyLastLine();
			else
				return StyleActualAlignment.JustifyLastLine;
		}
		protected override bool GetActualAlignmentShrinkToFit() {
			if (HasActualAlignment)
				return base.GetActualAlignmentShrinkToFit();
			else
				return StyleActualAlignment.ShrinkToFit;
		}
		protected override int GetActualAlignmentTextRotation() {
			if (HasActualAlignment)
				return base.GetActualAlignmentTextRotation();
			else
				return StyleActualAlignment.TextRotation;
		}
		protected override byte GetActualAlignmentIndent() {
			if (HasActualAlignment)
				return base.GetActualAlignmentIndent();
			else
				return StyleActualAlignment.Indent;
		}
		protected override int GetActualAlignmentRelativeIndent() {
			if (HasActualAlignment)
				return base.GetActualAlignmentRelativeIndent();
			else
				return StyleActualAlignment.RelativeIndent;
		}
		protected override XlHorizontalAlignment GetActualAlignmentHorizontal() {
			if (HasActualAlignment)
				return base.GetActualAlignmentHorizontal();
			else
				return StyleActualAlignment.Horizontal;
		}
		protected override XlVerticalAlignment GetActualAlignmentVertical() {
			if (HasActualAlignment)
				return base.GetActualAlignmentVertical();
			else
				return StyleActualAlignment.Vertical;
		}
		protected override XlReadingOrder GetActualAlignmentReadingOrder() {
			if (HasActualAlignment)
				return base.GetActualAlignmentReadingOrder();
			else
				return StyleActualAlignment.ReadingOrder;
		}
		#endregion
		#region IActualBorderInfo implementation
		protected override XlBorderLineStyle GetActualBorderLeftLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderLeftLineStyle();
			else
				return StyleActualBorder.LeftLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderRightLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderRightLineStyle();
			else
				return StyleActualBorder.RightLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderTopLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderTopLineStyle();
			else
				return StyleActualBorder.TopLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderBottomLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderBottomLineStyle();
			else
				return StyleActualBorder.BottomLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderHorizontalLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderHorizontalLineStyle();
			else
				return StyleActualBorder.HorizontalLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderVerticalLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderVerticalLineStyle();
			else
				return StyleActualBorder.VerticalLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalDownLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderDiagonalDownLineStyle();
			else
				return StyleActualBorder.DiagonalDownLineStyle;
		}
		protected override XlBorderLineStyle GetActualBorderDiagonalUpLineStyle() {
			if (HasActualBorder)
				return base.GetActualBorderDiagonalUpLineStyle();
			else
				return StyleActualBorder.DiagonalUpLineStyle;
		}
		protected override Color GetActualBorderLeftColor() {
			if (HasActualBorder)
				return base.GetActualBorderLeftColor();
			else
				return StyleActualBorder.LeftColor;
		}
		protected override Color GetActualBorderRightColor() {
			if (HasActualBorder)
				return base.GetActualBorderRightColor();
			else
				return StyleActualBorder.RightColor;
		}
		protected override Color GetActualBorderTopColor() {
			if (HasActualBorder)
				return base.GetActualBorderTopColor();
			else
				return StyleActualBorder.TopColor;
		}
		protected override Color GetActualBorderBottomColor() {
			if (HasActualBorder)
				return base.GetActualBorderBottomColor();
			else
				return StyleActualBorder.BottomColor;
		}
		protected override Color GetActualBorderHorizontalColor() {
			if (HasActualBorder)
				return base.GetActualBorderHorizontalColor();
			else
				return StyleActualBorder.HorizontalColor;
		}
		protected override Color GetActualBorderVerticalColor() {
			if (HasActualBorder)
				return base.GetActualBorderVerticalColor();
			else
				return StyleActualBorder.VerticalColor;
		}
		protected override Color GetActualBorderDiagonalColor() {
			if (HasActualBorder)
				return base.GetActualBorderDiagonalColor();
			else
				return StyleActualBorder.DiagonalColor;
		}
		protected override int GetActualBorderLeftColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderLeftColorIndex();
			else
				return StyleActualBorder.LeftColorIndex;
		}
		protected override int GetActualBorderRightColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderRightColorIndex();
			else
				return StyleActualBorder.RightColorIndex;
		}
		protected override int GetActualBorderTopColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderTopColorIndex();
			else
				return StyleActualBorder.TopColorIndex;
		}
		protected override int GetActualBorderBottomColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderBottomColorIndex();
			else
				return StyleActualBorder.BottomColorIndex;
		}
		protected override int GetActualBorderHorizontalColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderHorizontalColorIndex();
			else
				return StyleActualBorder.HorizontalColorIndex;
		}
		protected override int GetActualBorderVerticalColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderVerticalColorIndex();
			else
				return StyleActualBorder.VerticalColorIndex;
		}
		protected override int GetActualBorderDiagonalColorIndex() {
			if (HasActualBorder)
				return base.GetActualBorderDiagonalColorIndex();
			else
				return StyleActualBorder.DiagonalColorIndex;
		}
		protected override bool GetActualBorderOutline() {
			if (HasActualBorder)
				return base.GetActualBorderOutline();
			else
				return StyleActualBorder.Outline;
		}
		#endregion
		#region IActualCellProtectionInfo implementation
		protected override bool GetActualProtectionLocked() {
			if (HasActualProtection)
				return base.GetActualProtectionLocked();
			else
				return StyleActualProtection.Locked;
		}
		protected override bool GetActualProtectionHidden() {
			if (HasActualProtection)
				return base.GetActualProtectionHidden();
			else
				return StyleActualProtection.Hidden;
		}
		#endregion
		#region IActualApplyInfo
		protected override bool GetActualApplyFont() {
			if (ApplyFont)
				return base.GetActualApplyFont();
			else
				return StyleActualApplyInfo.ApplyFont;
		}
		protected override bool GetActualApplyFill() {
			if (ApplyFill)
				return base.GetActualApplyFill();
			else
				return StyleActualApplyInfo.ApplyFill;
		}
		protected override bool GetActualApplyBorder() {
			if (ApplyBorder)
				return base.GetActualApplyBorder();
			else
				return StyleActualApplyInfo.ApplyBorder;
		}
		protected override bool GetActualApplyAlignment() {
			if (ApplyAlignment)
				return base.GetActualApplyAlignment();
			else
				return StyleActualApplyInfo.ApplyAlignment;
		}
		protected override bool GetActualApplyProtection() {
			if (ApplyProtection)
				return base.GetActualApplyProtection();
			else
				return StyleActualApplyInfo.ApplyProtection;
		}
		protected override bool GetActualApplyNumberFormat() {
			if (ApplyNumberFormat)
				return base.GetActualApplyNumberFormat();
			else
				return StyleActualApplyInfo.ApplyNumberFormat;
		}
		#endregion
		protected override string GetActualFormatString() {
			if (HasActualNumberFormat)
				return base.GetActualFormatString();
			else
				return StyleFormat.ActualFormatString;
		}
		protected override int GetActualFormatIndex() {
			if (HasActualNumberFormat)
				return base.GetActualFormatIndex();
			else
				return StyleFormat.ActualFormatIndex;
		}
		#region ApplyFormat
		protected internal void ApplyFormat(CellFormat format, CellFormatApplyOptions options) {
			if (!Object.ReferenceEquals(format.DocumentModel, DocumentModel))
				return;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					ApplyNumberFormatCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyNumberFormat), true);
					ApplyFontCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyFont), true);
					ApplyFillCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyFill), true);
					ApplyBorderCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyBorder), true);
					ApplyAlignmentCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyAlignment), true);
					ApplyProtectionCore(format, GetApplyValue(options, CellFormatApplyOptions.ApplyProtection), true);
					CopyStyleIndex(format, GetApplyValue(options, CellFormatApplyOptions.ApplyStyle));
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		protected internal void ClearFormat(CellFormatApplyOptions options) {
			CellFormat defaultFormat = DocumentModel.StyleSheet.DefaultCellFormat as CellFormat;
			if (defaultFormat == null)
				return;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					ApplyNumberFormatCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyNumberFormat), false);
					ApplyFontCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyFont), false);
					ApplyFillCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyFill), false);
					ApplyBorderCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyBorder), false);
					ApplyAlignmentCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyAlignment), false);
					ApplyProtectionCore(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyProtection), false);
					CopyStyleIndex(defaultFormat, GetApplyValue(options, CellFormatApplyOptions.ApplyStyle));
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		bool GetApplyValue(CellFormatApplyOptions left, CellFormatApplyOptions right) {
			return (left & right) != 0;
		}
		void CopyStyleIndex(CellFormat format, bool hasValue) {
			if (hasValue) 
				Style = format.Style;
		}
		#endregion
		#region ApplyStyle
		protected internal void ApplyStyle(CellStyleBase cellStyle) {
			ApplyStyle(cellStyle, false);
		}
		protected internal void ApplyStyle(CellStyleBase cellStyle, bool keepProtection) {
			cellStyle = GetValidateCellStyle(cellStyle);
			System.Diagnostics.Debug.Assert(cellStyle.StyleIndex >= 0);
			CellStyleFormat cellStyleFormat = cellStyle.FormatInfo;
			if (!Object.ReferenceEquals(cellStyleFormat.DocumentModel, this.DocumentModel))
				return;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					ApplyStyleNumberFormatCore(cellStyleFormat.ApplyNumberFormat);
					ApplyStyleFontCore(cellStyleFormat.ApplyFont);
					ApplyStyleFillCore(cellStyleFormat.ApplyFill);
					ApplyStyleBorderCore(cellStyleFormat.ApplyBorder);
					ApplyStyleAlignmentCore(cellStyleFormat.ApplyAlignment);
					if (!keepProtection)
						ApplyStyleProtectionCore(cellStyleFormat.ApplyProtection);
					Style = cellStyle;
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
		}
		void ApplyStyleNumberFormatCore(bool applyNumberFormat) {
			if (applyNumberFormat && ApplyNumberFormat)
				ApplyNumberFormat = false;
			else if (!HasActualNumberFormat)
				CopyNumberFormat(StyleFormat.NumberFormatInfo);
		}
		void ApplyStyleFontCore(bool applyFont) {
			if (applyFont && ApplyFont)
				ApplyFont = false;
			else if (!HasActualFont)
				CopyFont(StyleFormat.FontInfo);
		}
		void ApplyStyleFillCore(bool applyFill) {
			if (applyFill && ApplyFill)
				ApplyFill = false;
			else if (!HasActualFill)
				CopyFill(StyleFormat);
		}
		void ApplyStyleBorderCore(bool applyBorder) {
			if (applyBorder && ApplyBorder)
				ApplyBorder = false;
			else if (!HasActualBorder)
				CopyBorder(StyleFormat.BorderInfo);
		}
		void ApplyStyleAlignmentCore(bool applyAlignment) {
			if (applyAlignment && ApplyAlignment)
				ApplyAlignment = false;
			else if (!HasActualAlignment)
				CopyAlignment(StyleFormat.AlignmentInfo);
		}
		void ApplyStyleProtectionCore(bool applyProtection) {
			if (applyProtection && ApplyProtection)
				ApplyProtection = false;
			else if (!HasActualProtection)
				CopyProtection(StyleFormat);
		}
		#endregion
		#region ApplyBorderFormat
		protected internal void ApplyBorderFormat(BorderInfo info) {
			BeginUpdate();
			try {
				ApplyBorder = true;
				CopyFromInfo(BorderIndexAccessor, info);
			} finally {
				EndUpdate();
			}
		}
		#endregion
		internal void AssignStyleIndex(int value) {
			this.styleIndex = value;
		}
		public override bool Equals(object obj) {
			CellFormat other = obj as CellFormat;
			if (other == null)
				return false;
			return this.StyleIndex == other.StyleIndex && base.Equals(other);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), StyleIndex);
		}
		public override FormatBase CreateEmptyClone(IDocumentModel documentModel) {
			return new CellFormat((DocumentModel)documentModel);
		}
		#region ISupportsSizeOf Members
		int ISupportsSizeOf.SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
		#region ICloneable<CellFormat> Members
		public new CellFormat Clone() {
			return (CellFormat)base.Clone();
		}
		#endregion
		internal override void CopySimple(FormatBase item) {
			base.CopySimple(item);
			CellFormat cellFormatItem = (CellFormat)item;
			this.styleIndex = cellFormatItem.styleIndex;
		}
		public override string ToString() {
			return String.Format("{0} style:{1}", base.ToString(), this.StyleIndex);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchUpdateHelper() {
			return new CellFormatBatchUpdateHelper(this);
		}
		protected override MultiIndexBatchUpdateHelper CreateBatchInitHelper() {
			return new CellFormatBatchInitHelper(this);
		}
		public override void CopyFrom(MultiIndexObject<FormatBase, DocumentModelChangeActions> obj) {
			base.CopyFrom(obj);
		}
		#region IsVisible
		public bool IsVisible { get { return HasVisibleFill || HasVisibleBorders; } }
		bool HasVisibleFill {
			get {
				if (GetFillActualFillType() != ModelFillType.Pattern)
					return GetFillActualGradientStops().Count > 0;
				XlPatternType patternType = GetFillActualPatternType();
				if (patternType == XlPatternType.None)
					return false;
				Color backColor = patternType == XlPatternType.Solid ? GetFillActualForeColor() : GetFillActualBackColor();
				return !DXColor.IsTransparentOrEmpty(backColor);
			}
		}
		bool HasVisibleBorders { 
			get {
				return
					HasVisibleBorder(GetActualBorderLeftLineStyle(), GetActualBorderLeftColor()) ||
					HasVisibleBorder(GetActualBorderRightLineStyle(), GetActualBorderRightColor()) ||
					HasVisibleBorder(GetActualBorderTopLineStyle(), GetActualBorderTopColor()) ||
					HasVisibleBorder(GetActualBorderBottomLineStyle(), GetActualBorderBottomColor()) ||
					HasVisibleBorder(GetActualBorderDiagonalUpLineStyle(), GetActualBorderDiagonalColor()) ||
					HasVisibleBorder(GetActualBorderDiagonalDownLineStyle(), GetActualBorderDiagonalColor());
			}
		}
		bool HasVisibleBorder(XlBorderLineStyle style, Color color) {
			return style != XlBorderLineStyle.None && !DXColor.IsTransparentOrEmpty(color);
		}
		#endregion
		protected override int ValidateNewIndexBeforeReplaceInfo(int newIndex) {
			if (newIndex != 0)
				return newIndex;
			int defaultCellFormatIndex = this.DocumentModel.StyleSheet.DefaultCellFormatIndex;
			if (newIndex != defaultCellFormatIndex && newIndex == 0)
				return defaultCellFormatIndex;
			return newIndex;
		}
		protected internal override bool HasVisibleActualFill {
			get {
				if (HasActualFill)
					return base.HasVisibleActualFill;
				return StyleFormat.HasVisibleActualFill;
			}
		}
		#region GetActualPositionCellFormatInfo
		#endregion
		#region GetActualBorderInfo
		protected internal BorderInfo ActualBorderInfo {
			get {
				if (HasActualBorder)
					return BorderInfo;
				return StyleFormat.BorderInfo;
			}
		}
		#endregion
	}
	#endregion
	#region CellStyleFormat
	public class CellStyleFormat : CellFormatBase, ICloneable<CellStyleFormat>, ISupportsSizeOf {
		public CellStyleFormat(DocumentModel documentModel)
			: base(documentModel) {
				AssignCellFormatFlagsIndex(CellFormatFlagsInfo.DefaultStyleValue);
		}
		#region Properties
		CellStyleFormat DefaultFormatInfo { get { return DocumentModel.StyleSheet.CellStyles.Normal.FormatInfo; } }
		FillInfo DefaultFill { get { return DefaultFormatInfo.FillInfo; } }
		RunFontInfo DefaultFont { get { return DefaultFormatInfo.FontInfo; } }
		CellAlignmentInfo DefaultAlignment { get { return DefaultFormatInfo.AlignmentInfo; } }
		BorderInfo DefaultBorder { get { return DefaultFormatInfo.BorderInfo; } }
		CellFormatFlagsInfo DefaultProtection { get { return DefaultFormatInfo.CellFormatFlagsInfo; } }
		NumberFormat DefaultNumberFormat { get { return DefaultFormatInfo.NumberFormatInfo; } }
		protected override FillInfo DefaultFillInfo { get { return DefaultFormatInfo.FillInfo; } }
		protected override GradientFillInfo DefaultGradientFillInfo { get { return DefaultFormatInfo.GradientFillInfo; } }
		#endregion
		#region IActualApplyInfo implementation
		protected override bool GetActualApplyFont() {
			return FontInfo != DefaultFont;
		}
		protected override bool GetActualApplyFill() {
			return FillInfo != DefaultFill;
		}
		protected override bool GetActualApplyBorder() {
			return BorderInfo != DefaultBorder;
		}
		protected override bool GetActualApplyAlignment() {
			return AlignmentInfo != DefaultAlignment;
		}
		protected override bool GetActualApplyProtection() {
			return
				CellFormatFlagsInfo.Hidden != DefaultProtection.Hidden ||
				CellFormatFlagsInfo.Locked != DefaultProtection.Locked;
		}
		protected override bool GetActualApplyNumberFormat() {
			return NumberFormatInfo != DefaultNumberFormat;
		}
		#endregion
		public override bool Equals(object obj) {
			CellStyleFormat other = obj as CellStyleFormat;
			if(other == null)
				return false;
			return base.Equals(other);
		}
		internal bool EqualsForDifferentWorkbooks(CellStyleFormat other) {
			bool result = false;
			try {
				BeginUpdate();
				other.BeginUpdate();
				DocumentModel otherDocumentModel = other.DocumentModel;
				result =
					AlignmentInfo.Equals(other.AlignmentInfo) &&
					CellFormatFlagsInfo.Equals(other.CellFormatFlagsInfo) &&
					NumberFormatInfo.Equals(other.NumberFormatInfo) &&
					FontInfo.EqualsForDifferentWorkbooks(other.FontInfo, DocumentModel, otherDocumentModel) &&
					BorderInfo.EqualsForDifferentWorkbooks(other.BorderInfo, DocumentModel, otherDocumentModel);
				if (CellFormatFlagsInfo.FillType == ModelFillType.Pattern)
					result &= FillInfo.EqualsForDifferentWorkbooks(other.FillInfo, DocumentModel, otherDocumentModel);
				else {
					result &=
						GradientFillInfo.Equals(other.GradientFillInfo) &&
						GradientStopInfoCollection.EqualsForDifferentWorkbooks(other.GradientStopInfoCollection);
				}
			} finally {
				EndUpdate();
				other.EndUpdate();
			}
			return result;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), GetType().GetHashCode());
		}
		public override FormatBase CreateEmptyClone(IDocumentModel documentModel) {
			return new CellStyleFormat((DocumentModel)documentModel);
		}
		#region ICloneable<CellFormat> Members
		public new CellStyleFormat Clone() {
			return (CellStyleFormat)base.Clone();
		}
		#endregion
		#region ISupportsSizeOf Members
		int ISupportsSizeOf.SizeOf() {
			return base.SizeOf();
		}
		#endregion
		#region ResetToDefaultCellFormat
		protected internal bool ResetToDefaultCellFormat() {
			CellFormatBase defaultFormat = DocumentModel.StyleSheet.DefaultCellFormat as CellFormatBase;
			bool result = false;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					result |= ResetToDefaultNumberFormat(defaultFormat, true);
					result |= ResetToDefaultFont(defaultFormat, true);
					result |= ResetToDefaultFill(defaultFormat, true);
					result |= ResetToDefaultBorder(defaultFormat, true);
					result |= ResetToDefaultAlignment(defaultFormat, true);
					result |= ResetToDefaultProtection(defaultFormat, true);
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
		bool ResetToDefaultNumberFormat(CellFormatBase defaultFormat, bool apply) {
			if (defaultFormat.NumberFormatIndex != NumberFormatIndex) {
				ApplyNumberFormatCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyNumberFormat != apply) {
				ApplyNumberFormat = apply;
				return true;
			}
			return false;
		}
		bool ResetToDefaultFont(CellFormatBase defaultFormat, bool apply) {
			if (defaultFormat.FontIndex != FontIndex) {
				ApplyFontCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyFont != apply) {
				ApplyFont = apply;
				return true;
			}
			return false;
		}
		bool ResetToDefaultFill(CellFormatBase defaultFormat, bool apply) {
			if (defaultFormat.FillIndex != FillIndex) {
				ApplyFillCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyFill != apply) {
				ApplyFill = apply;
				return true;
			}
			return false;
		}
		bool ResetToDefaultBorder(CellFormatBase defaultFormat, bool apply) {
			if (defaultFormat.BorderIndex != BorderIndex) {
				ApplyBorderCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyBorder != apply) {
				ApplyBorder = apply;
				return true;
			}
			return false;
		}
		bool ResetToDefaultAlignment(CellFormatBase defaultFormat, bool apply) {
			if (defaultFormat.AlignmentIndex != AlignmentIndex) {
				ApplyAlignmentCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyAlignment != apply) {
				ApplyAlignment = apply;
				return true;
			}
			return false;
		}
		bool ResetToDefaultProtection(CellFormatBase defaultFormat, bool apply) {
			ICellProtectionInfo defaultProtection = defaultFormat.Protection;
			if (defaultProtection.Hidden != Protection.Hidden || defaultProtection.Locked != Protection.Locked) {
				ApplyProtectionCore(defaultFormat, true, apply);
				return true;
			}
			if (ApplyProtection != apply) {
				ApplyProtection = apply;
				return true;
			}
			return false;
		}
		#endregion
		#region ResetToDefaultBuiltInFormat
		protected internal bool ResetToDefaultBuiltInFormat(BuiltInCellStyleInfo styleInfo) {
			CellFormatBase defaultFormat = DocumentModel.StyleSheet.DefaultCellFormat as CellFormatBase;
			bool result = false;
			DocumentModel.BeginUpdate();
			try {
				BeginUpdate();
				try {
					CellFormatFlagsInfo builtInFlagsInfo = styleInfo.FlagsInfo;
					result |= ResetToDefaultBuiltInNumberFormat(builtInFlagsInfo.ApplyNumberFormat, styleInfo, defaultFormat);
					result |= ResetToDefaultBuiltInFont(builtInFlagsInfo.ApplyFont, styleInfo, defaultFormat);
					result |= ResetToDefaultBuiltInFill(builtInFlagsInfo.ApplyFill, styleInfo, defaultFormat);
					result |= ResetToDefaultBuiltInBorder(builtInFlagsInfo.ApplyBorder, styleInfo, defaultFormat);
					result |= ResetToDefaultAlignment(defaultFormat, false);
					result |= ResetToDefaultProtection(defaultFormat, false);
				} finally {
					EndUpdate();
				}
			} finally {
				DocumentModel.EndUpdate();
			}
			return result;
		}
		bool ResetToDefaultBuiltInNumberFormat(bool hasBuiltInValue, BuiltInCellStyleInfo styleInfo, CellFormatBase defaultFormat) {
			if (hasBuiltInValue) {
				if (styleInfo.NumberFormatIndex != NumberFormatIndex) {
					NumberFormat numberFormat = DocumentModel.Cache.NumberFormatCache[styleInfo.NumberFormatIndex];
					ApplyNumberFormat = true;
					FormatString = numberFormat.FormatCode;
					return true;
				}
				if (!ApplyNumberFormat) {
					ApplyNumberFormat = true;
					return true;
				}
				return false;
			}
			return ResetToDefaultNumberFormat(defaultFormat, false);
		}
		bool ResetToDefaultBuiltInFont(bool hasBuiltInValue, BuiltInCellStyleInfo styleInfo, CellFormatBase defaultFormat) {
			if (hasBuiltInValue) {
				if (!styleInfo.CheckEqualsFontInfo(this)) {
					RunFontInfo fontInfo = styleInfo.CreateFontInfo(DocumentModel.Cache);
					ApplyFont = true;
					CopyFont(fontInfo);
					return true;
				}
				if (!ApplyFont) {
					ApplyFont = true;
					return true;
				}
				return false;
			}
			return ResetToDefaultFont(defaultFormat, false);
		}
		bool ResetToDefaultBuiltInFill(bool hasBuiltInValue, BuiltInCellStyleInfo styleInfo, CellFormatBase defaultFormat) {
			if (hasBuiltInValue) {
				if (!styleInfo.CheckEqualsFillInfo(this)) {
					FillInfo fillInfo = styleInfo.CreateFillInfo(DocumentModel.Cache);
					ApplyFill = true;
					CopyFromInfo(FillIndexAccessor, fillInfo);
					ClearGradientFill();
					return true;
				}
				if (!ApplyFill) {
					ApplyFill = true;
					ClearGradientFill();
					return true;
				}
				return false;
			}
			return ResetToDefaultFill(defaultFormat, false);
		}
		bool ResetToDefaultBuiltInBorder(bool hasBuiltInValue, BuiltInCellStyleInfo styleInfo, CellFormatBase defaultFormat) {
			if (hasBuiltInValue) {
				if (!styleInfo.CheckEqualsBorderInfo(this)) {
					BorderInfo borderInfo = styleInfo.CreateBorderInfo(DocumentModel.Cache);
					ApplyBorder = true;
					CopyBorder(borderInfo);
					return true;
				}
				if (!ApplyBorder) {
					ApplyBorder = true;
					return true;
				}
				return false;
			}
			return ResetToDefaultBorder(defaultFormat, false);
		}
		void ClearGradientFill() {
			if (CellFormatFlagsInfo.FillType != ModelFillType.Pattern) {
				CopyFromInfo(GradientFillIndexAccessor, DocumentModel.Cache.GradientFillInfoCache.DefaultItem.Clone());
				GradientStopInfoCollection.Clear();
				CopyFromFormatFillType(CellFormatFlagsInfo.Clone(), ModelFillType.Pattern);
			}
		}
		#endregion
	}
	#endregion
}
