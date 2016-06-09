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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
using DevExpress.Xpf;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
using System.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region IParagraphProperties
	public interface IParagraphProperties {
		ParagraphAlignment Alignment { get; set; }
		int LeftIndent { get; set; }
		int RightIndent { get; set; }
		int SpacingBefore { get; set; }
		int SpacingAfter { get; set; }
		ParagraphLineSpacing LineSpacingType { get; set; }
		float LineSpacing { get; set; }
		ParagraphFirstLineIndent FirstLineIndentType { get; set; }
		int FirstLineIndent { get; set; }
		bool SuppressHyphenation { get; set; }
		bool SuppressLineNumbers { get; set; }
		bool ContextualSpacing { get; set; }
		bool PageBreakBefore { get; set; }
		bool BeforeAutoSpacing { get; set; }
		bool AfterAutoSpacing { get; set; }
		bool KeepWithNext { get; set; }
		bool KeepLinesTogether { get; set; }
		bool WidowOrphanControl { get; set; }
		int OutlineLevel { get; set; }
		Color BackColor { get; set; }
#if THEMES_EDIT
		Shading Shading { get; set; }
#endif
		BorderInfo LeftBorder { get; set; }
		BorderInfo RightBorder { get; set; }
		BorderInfo TopBorder { get; set; }
		BorderInfo BottomBorder { get; set; }
	}
	#endregion
	#region ParagraphAlignment
	public enum ParagraphAlignment {
		Left = OfficeParagraphAlignment.Left,
		Right = OfficeParagraphAlignment.Right,
		Center = OfficeParagraphAlignment.Center,
		Justify = OfficeParagraphAlignment.Justify
	}
	#endregion
	#region ParagraphLineSpacing
	public enum ParagraphLineSpacing {
		Single = 0,
		Sesquialteral = 1,
		Double = 2,
		Multiple = 3,
		Exactly = 4,
		AtLeast = 5
	}
	#endregion
	#region ParagraphFirstLineIndent
	public enum ParagraphFirstLineIndent {
		None = 0,
		Indented = 1,
		Hanging = 2
	}
	#endregion
	#region IParagraphPropertiesContainer
	public interface IParagraphPropertiesContainer {
		PieceTable PieceTable { get; }
		IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateParagraphPropertiesChangedHistoryItem();
		void OnParagraphPropertiesChanged();
	}
	#endregion
	#region ParagraphFormattingInfo
	public class ParagraphFormattingInfo : ICloneable<ParagraphFormattingInfo>, ISupportsCopyFrom<ParagraphFormattingInfo>, ISupportsSizeOf {
		#region Fields
		const int MaskLineSpacingType = 0x00000007; 
		const int MaskFirstLineIndentType = 0x00000018; 
		const int MaskAlignment = 0x00000060; 
		const int MaskOutlineLevel = 0x00000780; 
		const int MaskSuppressHyphenation = 0x00000800;
		const int MaskSuppressLineNumbers = 0x00001000;
		const int MaskContextualSpacing = 0x00002000;
		const int MaskPageBreakBefore = 0x00004000;
		const int MaskBeforeAutoSpacing = 0x00008000;
		const int MaskAfterAutoSpacing = 0x00010000;
		const int MaskKeepWithNext = 0x00020000;
		const int MaskKeepLinesTogether = 0x00040000;
		const int MaskWidowOrphanControl = 0x00080000;
		int packedValues;
		int leftIndent;
		int rightIndent;
		int spacingBefore;
		int spacingAfter;
		int firstLineIndent;
		float lineSpacing;
		Color backColor;
#if THEMES_EDIT
		Shading shading;
#endif
		BorderInfo leftBorder;
		BorderInfo rightBorder;
		BorderInfo topBorder;
		BorderInfo bottomBorder;
		#endregion
		public ParagraphFormattingInfo() {
			this.leftBorder = new BorderInfo();
			this.rightBorder = new BorderInfo();
			this.topBorder = new BorderInfo();
			this.bottomBorder = new BorderInfo();
#if THEMES_EDIT
			this.shading = Shading.Create();
#endif
		}
		#region Properties
		[JSONEnum((int)JSONParagraphFormattingProperty.Alignment)]
		public ParagraphAlignment Alignment {
			get { return (ParagraphAlignment)((packedValues & MaskAlignment) >> 5); }
			set {
				packedValues &= ~MaskAlignment;
				packedValues |= ((int)value << 5) & MaskAlignment;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.LeftIndent)]
		public int LeftIndent { get { return leftIndent; } set { leftIndent = value; } }
		[JSONEnum((int)JSONParagraphFormattingProperty.RightIndent)]
		public int RightIndent { get { return rightIndent; } set { rightIndent = value; } }
		[JSONEnum((int)JSONParagraphFormattingProperty.SpacingBefore)]
		public int SpacingBefore {
			get { return spacingBefore; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("SpacingBefore", value);
				spacingBefore = value;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.SpacingAfter)]
		public int SpacingAfter {
			get { return spacingAfter; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("SpacingAfter", value);
				spacingAfter = value;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.LineSpacingType)]
		public ParagraphLineSpacing LineSpacingType {
			get { return (ParagraphLineSpacing)(packedValues & MaskLineSpacingType); }
			set {
				packedValues &= ~MaskLineSpacingType;
				packedValues |= ((int)value & MaskLineSpacingType);
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.LineSpacing)]
		public float LineSpacing {
			get { return lineSpacing; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("LineSpacing", value);
				lineSpacing = value;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.FirstLineIndentType)]
		public ParagraphFirstLineIndent FirstLineIndentType {
			get { return (ParagraphFirstLineIndent)((packedValues & MaskFirstLineIndentType) >> 3); }
			set {
				packedValues &= ~MaskFirstLineIndentType;
				packedValues |= ((int)value << 3) & MaskFirstLineIndentType;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.FirstLineIndent)]
		public int FirstLineIndent {
			get { return firstLineIndent; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("FirstLineIndent", value);
				firstLineIndent = value;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.SuppressHyphenation)]
		public bool SuppressHyphenation { get { return GetBooleanValue(MaskSuppressHyphenation); } set { SetBooleanValue(MaskSuppressHyphenation, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.SuppressLineNumbers)]
		public bool SuppressLineNumbers { get { return GetBooleanValue(MaskSuppressLineNumbers); } set { SetBooleanValue(MaskSuppressLineNumbers, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.ContextualSpacing)]
		public bool ContextualSpacing { get { return GetBooleanValue(MaskContextualSpacing); } set { SetBooleanValue(MaskContextualSpacing, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.PageBreakBefore)]
		public bool PageBreakBefore { get { return GetBooleanValue(MaskPageBreakBefore); } set { SetBooleanValue(MaskPageBreakBefore, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.BeforeAutoSpacing)]
		public bool BeforeAutoSpacing { get { return GetBooleanValue(MaskBeforeAutoSpacing); } set { SetBooleanValue(MaskBeforeAutoSpacing, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.AfterAutoSpacing)]
		public bool AfterAutoSpacing { get { return GetBooleanValue(MaskAfterAutoSpacing); } set { SetBooleanValue(MaskAfterAutoSpacing, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.KeepWithNext)]
		public bool KeepWithNext { get { return GetBooleanValue(MaskKeepWithNext); } set { SetBooleanValue(MaskKeepWithNext, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.KeepLinesTogether)]
		public bool KeepLinesTogether { get { return GetBooleanValue(MaskKeepLinesTogether); } set { SetBooleanValue(MaskKeepLinesTogether, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.WidowOrphanControl)]
		public bool WidowOrphanControl { get { return GetBooleanValue(MaskWidowOrphanControl); } set { SetBooleanValue(MaskWidowOrphanControl, value); } }
		[JSONEnum((int)JSONParagraphFormattingProperty.OutlineLevel)]
		public int OutlineLevel {
			get { return ((packedValues & MaskOutlineLevel) >> 7); }
			set {
				packedValues &= ~MaskOutlineLevel;
				packedValues |= (value << 7) & MaskOutlineLevel;
			}
		}
		[JSONEnum((int)JSONParagraphFormattingProperty.BackColor)]
		public Color BackColor { get { return backColor; } set { backColor = value; } }
#if THEMES_EDIT
		public Shading Shading { get { return shading; } set { shading = value; } }
#endif
		[JSONEnum((int)JSONParagraphFormattingProperty.LeftBorder)]
		public BorderInfo LeftBorder { get { return leftBorder; } set { leftBorder = value; } }
		[JSONEnum((int)JSONParagraphFormattingProperty.RightBorder)]
		public BorderInfo RightBorder { get { return rightBorder; } set { rightBorder = value; } }
		[JSONEnum((int)JSONParagraphFormattingProperty.TopBorder)]
		public BorderInfo TopBorder { get { return topBorder; } set { topBorder = value; } }
		[JSONEnum((int)JSONParagraphFormattingProperty.BottomBorder)]
		public BorderInfo BottomBorder { get { return bottomBorder; } set { bottomBorder = value; } }
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
		public ParagraphFormattingInfo Clone() {
			ParagraphFormattingInfo result = new ParagraphFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ParagraphFormattingInfo val) {
			this.packedValues = val.packedValues;
			this.LeftIndent = val.LeftIndent;
			this.RightIndent = val.RightIndent;
			this.SpacingBefore = val.SpacingBefore;
			this.SpacingAfter = val.SpacingAfter;
			this.LineSpacing = val.LineSpacing;
			this.FirstLineIndent = val.FirstLineIndent;
			this.BackColor = val.BackColor;
			this.TopBorder = val.TopBorder;
			this.BottomBorder = val.BottomBorder;
			this.LeftBorder = val.LeftBorder;
			this.RightBorder = val.RightBorder;
#if THEMES_EDIT
			this.Shading = val.Shading;
#endif
		}
		public override bool Equals(object obj) {
			ParagraphFormattingInfo info = (ParagraphFormattingInfo)obj;
			return
				this.packedValues == info.packedValues &&
				this.LeftIndent == info.LeftIndent &&
				this.RightIndent == info.RightIndent &&
				this.SpacingAfter == info.SpacingAfter &&
				this.SpacingBefore == info.SpacingBefore &&
				(SkipCompareLineSpacing() || this.LineSpacing == info.LineSpacing) &&
				(SkipCompareFirstLineIndent() || this.FirstLineIndent == info.FirstLineIndent) &&
				this.BackColor == info.BackColor &&
#if THEMES_EDIT
				this.Shading.Equals(info.Shading) &&
#endif
				this.TopBorder.Equals(info.TopBorder) &&
				this.BottomBorder.Equals(info.BottomBorder) &&
				this.LeftBorder.Equals(info.LeftBorder) &&
				this.RightBorder.Equals(info.RightBorder);			  
		}
		bool SkipCompareLineSpacing() {
			return false;
		}
		bool SkipCompareFirstLineIndent() {
			return false;
		}
		public override int GetHashCode() {
			return packedValues ^
				leftIndent ^
				rightIndent ^
				spacingAfter ^
				spacingBefore ^
				(SkipCompareLineSpacing() ? 0 : lineSpacing.GetHashCode()) ^
				(SkipCompareFirstLineIndent() ? 0 : firstLineIndent) ^
				backColor.GetHashCode() ^
#if THEMES_EDIT
				shading.GetHashCode() ^
#endif
				topBorder.GetHashCode() ^
				bottomBorder.GetHashCode() ^
				leftBorder.GetHashCode() ^
				rightBorder.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ParagraphFormattingInfoCache
	public class ParagraphFormattingInfoCache : UniqueItemsCache<ParagraphFormattingInfo> {
		internal const int DefaultItemIndex = 0;
		public ParagraphFormattingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ParagraphFormattingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ParagraphFormattingInfo defaultItem = new ParagraphFormattingInfo();
			defaultItem.WidowOrphanControl = true;
			return defaultItem;
		}
	}
	#endregion
	#region ParagraphFormattingCache
	public class ParagraphFormattingCache : UniqueItemsCache<ParagraphFormattingBase> {
		#region Fields
		public const int RootParagraphFormattingIndex = 1;
		public const int EmptyParagraphFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public ParagraphFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new ParagraphFormattingBase(DocumentModel.MainPieceTable, DocumentModel, 0, ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex));
			AppendItem(new ParagraphFormattingBase(DocumentModel.MainPieceTable, DocumentModel, 0, ParagraphFormattingOptionsCache.RootParagraphFormattingOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override ParagraphFormattingBase CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	#region ParagraphFormattingOptions
	public class ParagraphFormattingOptions : ICloneable<ParagraphFormattingOptions>, ISupportsCopyFrom<ParagraphFormattingOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseAlignment = 0x00000001,
			UseLeftIndent = 0x00000002,
			UseRightIndent = 0x00000004,
			UseSpacingBefore = 0x00000008,
			UseSpacingAfter = 0x00000010,
			UseLineSpacing = 0x00000020,
			UseFirstLineIndent = 0x00000040,
			UseSuppressHyphenation = 0x00000080,
			UseSuppressLineNumbers = 0x00000100,
			UseContextualSpacing = 0x00000200,
			UsePageBreakBefore = 0x00000400,
			UseBeforeAutoSpacing = 0x00000800,
			UseAfterAutoSpacing = 0x00001000,
			UseKeepWithNext = 0x00002000,
			UseKeepLinesTogether = 0x00004000,
			UseWidowOrphanControl = 0x00008000,
			UseOutlineLevel = 0x00010000,
			UseBackColor = 0x00020000,
			UseLeftBorder = 0x00040000,
			UseRightBorder = 0x00080000,
			UseTopBorder = 0x00100000,
			UseBottomBorder = 0x00200000,
			UseBorders = 0x003C0000,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseAlignment { get { return GetVal(Mask.UseAlignment); } set { SetVal(Mask.UseAlignment, value); } }
		public bool UseLeftIndent { get { return GetVal(Mask.UseLeftIndent); } set { SetVal(Mask.UseLeftIndent, value); } }
		public bool UseRightIndent { get { return GetVal(Mask.UseRightIndent); } set { SetVal(Mask.UseRightIndent, value); } }
		public bool UseSpacingBefore { get { return GetVal(Mask.UseSpacingBefore); } set { SetVal(Mask.UseSpacingBefore, value); } }
		public bool UseSpacingAfter { get { return GetVal(Mask.UseSpacingAfter); } set { SetVal(Mask.UseSpacingAfter, value); } }
		public bool UseLineSpacing { get { return GetVal(Mask.UseLineSpacing); } set { SetVal(Mask.UseLineSpacing, value); } }
		public bool UseFirstLineIndent { get { return GetVal(Mask.UseFirstLineIndent); } set { SetVal(Mask.UseFirstLineIndent, value); } }
		public bool UseSuppressHyphenation { get { return GetVal(Mask.UseSuppressHyphenation); } set { SetVal(Mask.UseSuppressHyphenation, value); } }
		public bool UseSuppressLineNumbers { get { return GetVal(Mask.UseSuppressLineNumbers); } set { SetVal(Mask.UseSuppressLineNumbers, value); } }
		public bool UseContextualSpacing { get { return GetVal(Mask.UseContextualSpacing); } set { SetVal(Mask.UseContextualSpacing, value); } }
		public bool UsePageBreakBefore { get { return GetVal(Mask.UsePageBreakBefore); } set { SetVal(Mask.UsePageBreakBefore, value); } }
		public bool UseBeforeAutoSpacing { get { return GetVal(Mask.UseBeforeAutoSpacing); } set { SetVal(Mask.UseBeforeAutoSpacing, value); } }
		public bool UseAfterAutoSpacing { get { return GetVal(Mask.UseAfterAutoSpacing); } set { SetVal(Mask.UseAfterAutoSpacing, value); } }
		public bool UseKeepWithNext { get { return GetVal(Mask.UseKeepWithNext); } set { SetVal(Mask.UseKeepWithNext, value); } }
		public bool UseKeepLinesTogether { get { return GetVal(Mask.UseKeepLinesTogether); } set { SetVal(Mask.UseKeepLinesTogether, value); } }
		public bool UseWidowOrphanControl { get { return GetVal(Mask.UseWidowOrphanControl); } set { SetVal(Mask.UseWidowOrphanControl, value); } }
		public bool UseOutlineLevel { get { return GetVal(Mask.UseOutlineLevel); } set { SetVal(Mask.UseOutlineLevel, value); } }
		public bool UseBackColor { get { return GetVal(Mask.UseBackColor); } set { SetVal(Mask.UseBackColor, value); } }
		public bool UseLeftBorder { get { return GetVal(Mask.UseLeftBorder); } set { SetVal(Mask.UseLeftBorder, value); } }
		public bool UseRightBorder { get { return GetVal(Mask.UseRightBorder); } set { SetVal(Mask.UseRightBorder, value); } }
		public bool UseTopBorder { get { return GetVal(Mask.UseTopBorder); } set { SetVal(Mask.UseTopBorder, value); } }
		public bool UseBottomBorder { get { return GetVal(Mask.UseBottomBorder); } set { SetVal(Mask.UseBottomBorder, value); } }
		public bool UseInsideHorizontalBorderMask { get { return false; } set { value = false; } }
		public bool UseInsideVerticalBorderMask { get { return false; } set { value = false; } }
		#endregion
		#region GetVal/SetVal helpers
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		#endregion
		public ParagraphFormattingOptions() {
		}
		internal ParagraphFormattingOptions(Mask val) {
			this.val = val;
		}
		public ParagraphFormattingOptions Clone() {
			return new ParagraphFormattingOptions(this.val);
		}
		public override bool Equals(object obj) {
			ParagraphFormattingOptions opts = (ParagraphFormattingOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(ParagraphFormattingOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ParagraphFormattingOptionsCache
	public class ParagraphFormattingOptionsCache : UniqueItemsCache<ParagraphFormattingOptions> {
		internal const int EmptyParagraphFormattingOptionIndex = 0;
		internal const int RootParagraphFormattingOptionIndex = 1;
		public ParagraphFormattingOptionsCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override ParagraphFormattingOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ParagraphFormattingOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new ParagraphFormattingOptions(ParagraphFormattingOptions.Mask.UseAll));
		}
	}
	#endregion
	public enum JSONParagraphFormattingProperty {
		Alignment = 0,
		FirstLineIndent = 1,
		FirstLineIndentType = 2,
		LeftIndent = 3,
		LineSpacing = 4,
		LineSpacingType = 5,
		RightIndent = 6,
		SpacingBefore = 7,
		SpacingAfter = 8,
		SuppressHyphenation = 9,
		SuppressLineNumbers = 10,
		ContextualSpacing = 11,
		PageBreakBefore = 12,
		BeforeAutoSpacing = 13,
		AfterAutoSpacing = 14,
		KeepWithNext = 15,
		KeepLinesTogether = 16,
		WidowOrphanControl = 17,
		OutlineLevel = 18,
		BackColor = 19,
		LeftBorder = 20,
		RightBorder = 21,
		TopBorder = 22,
		BottomBorder = 23,
		UseValue = 24
	}
	#region ParagraphFormattingBase
	public class ParagraphFormattingBase : IndexBasedObjectB<ParagraphFormattingInfo, ParagraphFormattingOptions>, ICloneable<ParagraphFormattingBase>, ISupportsCopyFrom<ParagraphFormattingBase>, IParagraphProperties {
		internal ParagraphFormattingBase(PieceTable pieceTable, DocumentModel documentModel, int formattingInfoIndex, int formattingOptionsIndex)
			: base(pieceTable, documentModel, formattingInfoIndex, formattingOptionsIndex) {
		}
		internal ParagraphFormattingInfo FormattingInfo { get { return Info; } }
		internal ParagraphFormattingOptions FormattingOptions { get { return Options; } }
		protected override UniqueItemsCache<ParagraphFormattingInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.ParagraphFormattingInfoCache; } }
		protected override UniqueItemsCache<ParagraphFormattingOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.ParagraphFormattingOptionsCache; } }
		#region Alignment
		[JSONEnum((int)JSONParagraphFormattingProperty.Alignment)]
		public ParagraphAlignment Alignment {
			get { return Info.Alignment; }
			set {
				if (Info.Alignment == value && Options.UseAlignment)
					return;
				SetPropertyValue(SetAlignmentCore, value, SetUseAlignmentCore);
			}
		}
		void SetAlignmentCore(ParagraphFormattingInfo info, ParagraphAlignment value) {
			info.Alignment = value;
		}
		void SetUseAlignmentCore(ParagraphFormattingOptions options, bool value) {
			options.UseAlignment = value;
		}
		#endregion
		#region FirstLineIndent
		[JSONEnum((int)JSONParagraphFormattingProperty.FirstLineIndent)]
		public int FirstLineIndent {
			get { return Info.FirstLineIndent; }
			set {
				if (Info.FirstLineIndent == value && Options.UseFirstLineIndent)
					return;
				SetPropertyValue(SetFirstLineIndentCore, value, SetUseFirstLineIndentCore);
			}
		}
		void SetFirstLineIndentCore(ParagraphFormattingInfo info, int value) {
			info.FirstLineIndent = value;
		}
		void SetUseFirstLineIndentCore(ParagraphFormattingOptions options, bool value) {
			options.UseFirstLineIndent = value;
		}
		#endregion
		#region FirstLineIndentType
		[JSONEnum((int)JSONParagraphFormattingProperty.FirstLineIndentType)]
		public ParagraphFirstLineIndent FirstLineIndentType {
			get { return Info.FirstLineIndentType; }
			set {
				if (Info.FirstLineIndentType == value && Options.UseFirstLineIndent)
					return;
				SetPropertyValue(SetFirstLineIndentTypeCore, value, SetUseFirstLineIndentTypeCore);
			}
		}
		void SetFirstLineIndentTypeCore(ParagraphFormattingInfo info, ParagraphFirstLineIndent value) {
			info.FirstLineIndentType = value;
			info.FirstLineIndent = 0;
		}
		void SetUseFirstLineIndentTypeCore(ParagraphFormattingOptions options, bool value) {
			options.UseFirstLineIndent = value;
		}
		#endregion
		#region LeftIndent
		[JSONEnum((int)JSONParagraphFormattingProperty.LeftIndent)]
		public int LeftIndent {
			get { return Info.LeftIndent; }
			set {
				if (Info.LeftIndent == value && Options.UseLeftIndent)
					return;
				SetPropertyValue(SetLeftIndentCore, value, SetUseLeftIndentCore);
			}
		}
		void SetLeftIndentCore(ParagraphFormattingInfo info, int value) {
			info.LeftIndent = value;
		}
		void SetUseLeftIndentCore(ParagraphFormattingOptions options, bool value) {
			options.UseLeftIndent = value;
		}
		#endregion
		#region LineSpacing
		[JSONEnum((int)JSONParagraphFormattingProperty.LineSpacing)]
		public float LineSpacing {
			get { return Info.LineSpacing; }
			set {
				if (Info.LineSpacing == value && Options.UseLineSpacing)
					return;
				SetPropertyValue(SetLineSpacingCore, value, SetUseLineSpacingCore);
			}
		}
		void SetLineSpacingCore(ParagraphFormattingInfo info, float value) {
			info.LineSpacing = value;
		}
		void SetUseLineSpacingCore(ParagraphFormattingOptions options, bool value) {
			options.UseLineSpacing = value;
		}
		#endregion
		#region LineSpacingType
		[JSONEnum((int)JSONParagraphFormattingProperty.LineSpacingType)]
		public ParagraphLineSpacing LineSpacingType {
			get { return Info.LineSpacingType; }
			set {
				if (Info.LineSpacingType == value && Options.UseLineSpacing)
					return;
				SetPropertyValue(SetLineSpacingTypeCore, value, SetUseLineSpacingTypeCore);
			}
		}
		void SetLineSpacingTypeCore(ParagraphFormattingInfo info, ParagraphLineSpacing value) {
			info.LineSpacingType = value;
			info.LineSpacing = 0.0f;
		}
		void SetUseLineSpacingTypeCore(ParagraphFormattingOptions options, bool value) {
			options.UseLineSpacing = value;
		}
		#endregion
		#region RightIndent
		[JSONEnum((int)JSONParagraphFormattingProperty.RightIndent)]
		public int RightIndent {
			get { return Info.RightIndent; }
			set {
				if (Info.RightIndent == value && Options.UseRightIndent)
					return;
				SetPropertyValue(SetRightIndentCore, value, SetUseRightIndentCore);
			}
		}
		void SetRightIndentCore(ParagraphFormattingInfo info, int value) {
			info.RightIndent = value;
		}
		void SetUseRightIndentCore(ParagraphFormattingOptions options, bool value) {
			options.UseRightIndent = value;
		}
		#endregion
		#region SpacingAfter
		[JSONEnum((int)JSONParagraphFormattingProperty.SpacingAfter)]
		public int SpacingAfter {
			get { return Info.SpacingAfter; }
			set {
				if (Info.SpacingAfter == value && Options.UseSpacingAfter)
					return;
				SetPropertyValue(SetSpacingAfterCore, value, SetUseSpacingAfterCore);
			}
		}
		void SetSpacingAfterCore(ParagraphFormattingInfo info, int value) {
			info.SpacingAfter = value;
		}
		void SetUseSpacingAfterCore(ParagraphFormattingOptions options, bool value) {
			options.UseSpacingAfter = value;
		}
		#endregion
		#region SpacingBefore
		[JSONEnum((int)JSONParagraphFormattingProperty.SpacingBefore)]
		public int SpacingBefore {
			get { return Info.SpacingBefore; }
			set {
				if (Info.SpacingBefore == value && Options.UseSpacingBefore)
					return;
				SetPropertyValue(SetSpacingBeforeCore, value, SetUseSpacingBeforeCore);
			}
		}
		void SetSpacingBeforeCore(ParagraphFormattingInfo info, int value) {
			info.SpacingBefore = value;
		}
		void SetUseSpacingBeforeCore(ParagraphFormattingOptions options, bool value) {
			options.UseSpacingBefore = value;
		}
		#endregion
		#region SuppressHyphenation
		[JSONEnum((int)JSONParagraphFormattingProperty.SuppressHyphenation)]
		public bool SuppressHyphenation {
			get { return Info.SuppressHyphenation; }
			set {
				if (Info.SuppressHyphenation == value && Options.UseSuppressHyphenation)
					return;
				SetPropertyValue(SetSuppressHyphenationCore, value, SetUseSuppressHyphenationCore);
			}
		}
		void SetSuppressHyphenationCore(ParagraphFormattingInfo info, bool value) {
			info.SuppressHyphenation = value;
		}
		void SetUseSuppressHyphenationCore(ParagraphFormattingOptions options, bool value) {
			options.UseSuppressHyphenation = value;
		}
		#endregion
		#region SuppressLineNumbers
		[JSONEnum((int)JSONParagraphFormattingProperty.SuppressLineNumbers)]
		public bool SuppressLineNumbers {
			get { return Info.SuppressLineNumbers; }
			set {
				if (Info.SuppressLineNumbers == value && Options.UseSuppressLineNumbers)
					return;
				SetPropertyValue(SetSuppressLineNumbersCore, value, SetUseSuppressLineNumbersCore);
			}
		}
		void SetSuppressLineNumbersCore(ParagraphFormattingInfo info, bool value) {
			info.SuppressLineNumbers = value;
		}
		void SetUseSuppressLineNumbersCore(ParagraphFormattingOptions options, bool value) {
			options.UseSuppressLineNumbers = value;
		}
		#endregion
		#region ContextualSpacing
		[JSONEnum((int)JSONParagraphFormattingProperty.ContextualSpacing)]
		public bool ContextualSpacing {
			get { return Info.ContextualSpacing; }
			set {
				if (Info.ContextualSpacing == value && Options.UseContextualSpacing)
					return;
				SetPropertyValue(SetContextualSpacingCore, value, SetUseContextualSpacingCore);
			}
		}
		void SetContextualSpacingCore(ParagraphFormattingInfo info, bool value) {
			info.ContextualSpacing = value;
		}
		void SetUseContextualSpacingCore(ParagraphFormattingOptions options, bool value) {
			options.UseContextualSpacing = value;
		}
		#endregion
		#region PageBreakBefore
		[JSONEnum((int)JSONParagraphFormattingProperty.PageBreakBefore)]
		public bool PageBreakBefore {
			get { return Info.PageBreakBefore; }
			set {
				if (Info.PageBreakBefore == value && Options.UsePageBreakBefore)
					return;
				SetPropertyValue(SetPageBreakBeforeCore, value, SetUsePageBreakBeforeCore);
			}
		}
		void SetPageBreakBeforeCore(ParagraphFormattingInfo info, bool value) {
			info.PageBreakBefore = value;
		}
		void SetUsePageBreakBeforeCore(ParagraphFormattingOptions options, bool value) {
			options.UsePageBreakBefore = value;
		}
		#endregion
		#region BeforeAutoSpacing
		[JSONEnum((int)JSONParagraphFormattingProperty.BeforeAutoSpacing)]
		public bool BeforeAutoSpacing {
			get { return Info.BeforeAutoSpacing; }
			set {
				if (Info.BeforeAutoSpacing == value && Options.UseBeforeAutoSpacing)
					return;
				SetPropertyValue(SetBeforeAutoSpacingCore, value, SetUseBeforeAutoSpacingCore);
			}
		}
		void SetBeforeAutoSpacingCore(ParagraphFormattingInfo info, bool value) {
			info.BeforeAutoSpacing = value;
		}
		void SetUseBeforeAutoSpacingCore(ParagraphFormattingOptions options, bool value) {
			options.UseBeforeAutoSpacing = value;
		}
		#endregion
		#region AfterAutoSpacing
		[JSONEnum((int)JSONParagraphFormattingProperty.AfterAutoSpacing)]
		public bool AfterAutoSpacing {
			get { return Info.AfterAutoSpacing; }
			set {
				if (Info.AfterAutoSpacing == value && Options.UseAfterAutoSpacing)
					return;
				SetPropertyValue(SetAfterAutoSpacingCore, value, SetUseAfterAutoSpacingCore);
			}
		}
		void SetAfterAutoSpacingCore(ParagraphFormattingInfo info, bool value) {
			info.AfterAutoSpacing = value;
		}
		void SetUseAfterAutoSpacingCore(ParagraphFormattingOptions options, bool value) {
			options.UseAfterAutoSpacing = value;
		}
		#endregion
		#region KeepWithNext
		[JSONEnum((int)JSONParagraphFormattingProperty.KeepWithNext)]
		public bool KeepWithNext {
			get { return Info.KeepWithNext; }
			set {
				if (Info.KeepWithNext == value && Options.UseKeepWithNext)
					return;
				SetPropertyValue(SetKeepWithNextCore, value, SetUseKeepWithNextCore);
			}
		}
		void SetKeepWithNextCore(ParagraphFormattingInfo info, bool value) {
			info.KeepWithNext = value;
		}
		void SetUseKeepWithNextCore(ParagraphFormattingOptions options, bool value) {
			options.UseKeepWithNext = value;
		}
		#endregion
		#region KeepLinesTogether
		[JSONEnum((int)JSONParagraphFormattingProperty.KeepLinesTogether)]
		public bool KeepLinesTogether {
			get { return Info.KeepLinesTogether; }
			set {
				if (Info.KeepLinesTogether == value && Options.UseKeepLinesTogether)
					return;
				SetPropertyValue(SetKeepLinesTogetherCore, value, SetUseKeepLinesTogetherCore);
			}
		}
		void SetKeepLinesTogetherCore(ParagraphFormattingInfo info, bool value) {
			info.KeepLinesTogether = value;
		}
		void SetUseKeepLinesTogetherCore(ParagraphFormattingOptions options, bool value) {
			options.UseKeepLinesTogether = value;
		}
		#endregion
		#region WidowOrphanControl
		[JSONEnum((int)JSONParagraphFormattingProperty.WidowOrphanControl)]
		public bool WidowOrphanControl {
			get { return Info.WidowOrphanControl; }
			set {
				if (Info.WidowOrphanControl == value && Options.UseWidowOrphanControl)
					return;
				SetPropertyValue(SetWidowOrphanControlCore, value, SetUseWidowOrphanControlCore);
			}
		}
		void SetWidowOrphanControlCore(ParagraphFormattingInfo info, bool value) {
			info.WidowOrphanControl = value;
		}
		void SetUseWidowOrphanControlCore(ParagraphFormattingOptions options, bool value) {
			options.UseWidowOrphanControl = value;
		}
		#endregion
		#region OutlineLevel
		[JSONEnum((int)JSONParagraphFormattingProperty.OutlineLevel)]
		public int OutlineLevel {
			get { return Info.OutlineLevel; }
			set {
				if (Info.OutlineLevel == value && Options.UseOutlineLevel)
					return;
				SetPropertyValue(SetOutlineLevelCore, value, SetUseOutlineLevelCore);
			}
		}
		void SetOutlineLevelCore(ParagraphFormattingInfo info, int value) {
			info.OutlineLevel = value;
		}
		void SetUseOutlineLevelCore(ParagraphFormattingOptions options, bool value) {
			options.UseOutlineLevel = value;
		}
		#endregion
		#region BackColor
		[JSONEnum((int)JSONParagraphFormattingProperty.BackColor)]
		public Color BackColor {
			get { return Info.BackColor; }
			set {
				if (Info.BackColor == value && Options.UseBackColor)
					return;
				SetPropertyValue(SetBackColorCore, value, SetUseBackColorCore);
			}
		}
		void SetBackColorCore(ParagraphFormattingInfo info, Color value) {
			info.BackColor = value;
		}
		void SetUseBackColorCore(ParagraphFormattingOptions options, bool value) {
			options.UseBackColor = value;
		}
		#endregion
#if THEMES_EDIT
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				if (Info.Shading == value && Options.UseBackColor)
					return;
				SetPropertyValue(SetShadingCore, value, SetUseBackColorCore);
			}
		}
		void SetShadingCore(ParagraphFormattingInfo info, Shading value) {
			info.Shading = value;
		}
		#endregion
#endif
		#region LeftBorder
		[JSONEnum((int)JSONParagraphFormattingProperty.LeftBorder)]
		public BorderInfo LeftBorder {
			get { return Info.LeftBorder; }
			set {
				if (Info.LeftBorder == value && Options.UseLeftBorder)
					return;
				SetPropertyValue(SetLeftBorderCore, value, SetUseLeftBorderCore);
			}
		}
		void SetLeftBorderCore(ParagraphFormattingInfo info, BorderInfo value) {
			info.LeftBorder = value;
		}
		void SetUseLeftBorderCore(ParagraphFormattingOptions options, bool value) {
			options.UseLeftBorder = value;
		}
		#endregion
		#region RightBorder
		[JSONEnum((int)JSONParagraphFormattingProperty.RightBorder)]
		public BorderInfo RightBorder {
			get { return Info.RightBorder; }
			set {
				if (Info.RightBorder == value && Options.UseRightBorder)
					return;
				SetPropertyValue(SetRightBorderCore, value, SetUseRightBorderCore);
			}
		}
		void SetRightBorderCore(ParagraphFormattingInfo info, BorderInfo value) {
			info.RightBorder = value;
		}
		void SetUseRightBorderCore(ParagraphFormattingOptions options, bool value) {
			options.UseRightBorder = value;
		}
		#endregion
		#region TopBorder
		[JSONEnum((int)JSONParagraphFormattingProperty.TopBorder)]
		public BorderInfo TopBorder {
			get { return Info.TopBorder; }
			set {
				if (Info.TopBorder == value && Options.UseTopBorder)
					return;
				SetPropertyValue(SetTopBorderCore, value, SetUseTopBorderCore);
			}
		}
		void SetTopBorderCore(ParagraphFormattingInfo info, BorderInfo value) {
			info.TopBorder = value;
		}
		void SetUseTopBorderCore(ParagraphFormattingOptions options, bool value) {
			options.UseTopBorder = value;
		}
		#endregion
		#region BottomBorder
		[JSONEnum((int)JSONParagraphFormattingProperty.BottomBorder)]
		public BorderInfo BottomBorder {
			get { return Info.BottomBorder; }
			set {
				if (Info.BottomBorder == value && Options.UseBottomBorder)
					return;
				SetPropertyValue(SetBottomBorderCore, value, SetUseBottomBorderCore);
			}
		}
		void SetBottomBorderCore(ParagraphFormattingInfo info, BorderInfo value) {
			info.BottomBorder = value;
		}
		void SetUseBottomBorderCore(ParagraphFormattingOptions options, bool value) {
			options.UseBottomBorder = value;
		}
		#endregion
		#region UseValue
		[JSONEnum((int)JSONParagraphFormattingProperty.UseValue)]
		public ParagraphFormattingOptions.Mask UseValue {
			get { return Options.Value; }
			internal set { Options.Value = value; }
		}
		#endregion
		protected internal virtual void SetMultipleLineSpacing(float newLineSpacing) {
			if (newLineSpacing <= 0) {
				LineSpacingType = ParagraphLineSpacing.Single;
				return;
			}
			LineSpacingType = ParagraphLineSpacing.Multiple;
			if (newLineSpacing == 2f)
				LineSpacingType = ParagraphLineSpacing.Double;
			if (newLineSpacing == 1.5f)
				LineSpacingType = ParagraphLineSpacing.Sesquialteral;
			if (newLineSpacing == 1f)
				LineSpacingType = ParagraphLineSpacing.Single;
			LineSpacing = newLineSpacing;
		}
		#region ICloneable<ParagraphFormattingBase> Members
		public ParagraphFormattingBase Clone() {
			return new ParagraphFormattingBase(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(ParagraphFormattingBase paragraphFormatting) {
			CopyFrom(paragraphFormatting.Info, paragraphFormatting.Options);
		}
		public void CopyFrom(ParagraphFormattingInfo info, ParagraphFormattingOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<ParagraphFormattingInfo, ParagraphFormattingOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<ParagraphFormattingInfo, ParagraphFormattingOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<ParagraphFormattingInfo, ParagraphFormattingOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.ParagraphFormattingAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
	}
	#endregion
	#region ParagraphProperties
	public class ParagraphProperties : RichEditIndexBasedObject<ParagraphFormattingBase>, IParagraphProperties {
		#region Static
		public static void ApplyPropertiesDiff(ParagraphProperties target, ParagraphFormattingInfo targetMergedInfo, ParagraphFormattingInfo sourceMergedInfo) {
			if (targetMergedInfo.Alignment != sourceMergedInfo.Alignment)
				target.Alignment = sourceMergedInfo.Alignment;
			if (targetMergedInfo.FirstLineIndent != sourceMergedInfo.FirstLineIndent)
				target.FirstLineIndent = sourceMergedInfo.FirstLineIndent;
			if (targetMergedInfo.FirstLineIndentType != sourceMergedInfo.FirstLineIndentType)
				target.FirstLineIndentType = sourceMergedInfo.FirstLineIndentType;
			if (targetMergedInfo.LeftIndent != sourceMergedInfo.LeftIndent)
				target.LeftIndent = sourceMergedInfo.LeftIndent;
			if (targetMergedInfo.LineSpacingType != sourceMergedInfo.LineSpacingType) 
				target.LineSpacingType = sourceMergedInfo.LineSpacingType;
			if (targetMergedInfo.LineSpacing != sourceMergedInfo.LineSpacing) 
				target.LineSpacing = sourceMergedInfo.LineSpacing;
			if (targetMergedInfo.RightIndent != sourceMergedInfo.RightIndent)
				target.RightIndent = sourceMergedInfo.RightIndent;
			if (targetMergedInfo.SpacingAfter != sourceMergedInfo.SpacingAfter)
				target.SpacingAfter = sourceMergedInfo.SpacingAfter;
			if (targetMergedInfo.SpacingBefore != sourceMergedInfo.SpacingBefore)
				target.SpacingBefore = sourceMergedInfo.SpacingBefore;
			if (targetMergedInfo.SuppressHyphenation != sourceMergedInfo.SuppressHyphenation)
				target.SuppressHyphenation = sourceMergedInfo.SuppressHyphenation;
			if (targetMergedInfo.SuppressLineNumbers != sourceMergedInfo.SuppressLineNumbers)
				target.SuppressLineNumbers = sourceMergedInfo.SuppressLineNumbers;
			if (targetMergedInfo.ContextualSpacing != sourceMergedInfo.ContextualSpacing)
				target.ContextualSpacing = sourceMergedInfo.ContextualSpacing;
			if (targetMergedInfo.PageBreakBefore != sourceMergedInfo.PageBreakBefore)
				target.PageBreakBefore = sourceMergedInfo.PageBreakBefore;
			if (targetMergedInfo.OutlineLevel != sourceMergedInfo.OutlineLevel)
				target.OutlineLevel = sourceMergedInfo.OutlineLevel;
			if (targetMergedInfo.BackColor != sourceMergedInfo.BackColor)
				target.BackColor = sourceMergedInfo.BackColor;
#if THEMES_EDIT            
			if (!targetMergedInfo.Shading.Equals(sourceMergedInfo.Shading))
				target.Shading = sourceMergedInfo.Shading;
#endif
		}
		#endregion
		readonly IParagraphPropertiesContainer owner;
		public ParagraphProperties(IParagraphPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(IParagraphPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		#region Properties
		#region Alignment
		public ParagraphAlignment Alignment {
			get { return Info.Alignment; }
			set {
				if (Info.Alignment == value && UseAlignment)
					return;
				SetPropertyValue(SetAlignmentCore, value);
			}
		}
		public bool UseAlignment { get { return Info.Options.UseAlignment; } }
		protected internal virtual DocumentModelChangeActions SetAlignmentCore(ParagraphFormattingBase info, ParagraphAlignment value) {
			info.Alignment = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.Alignment);
		}
		#endregion
		#region FirstLineIndent
		public int FirstLineIndent {
			get { return Info.FirstLineIndent; }
			set {
				if (Info.FirstLineIndent == value && UseFirstLineIndent)
					return;
				SetPropertyValue(SetFirstLineIndentCore, value);
			}
		}
		public bool UseFirstLineIndent { get { return Info.Options.UseFirstLineIndent; } }
		protected internal virtual DocumentModelChangeActions SetFirstLineIndentCore(ParagraphFormattingBase info, int value) {
			info.FirstLineIndent = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.FirstLineIndent);
		}
		#endregion
		#region FirstLineIndentType
		public ParagraphFirstLineIndent FirstLineIndentType {
			get { return Info.FirstLineIndentType; }
			set {
				if (Info.FirstLineIndentType == value && UseFirstLineIndentType)
					return;
				SetPropertyValue(SetFirstLineIndentTypeCore, value);
			}
		}
		public bool UseFirstLineIndentType { get { return Info.Options.UseFirstLineIndent; } }
		protected internal virtual DocumentModelChangeActions SetFirstLineIndentTypeCore(ParagraphFormattingBase info, ParagraphFirstLineIndent value) {
			info.FirstLineIndentType = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.FirstLineIndentType);
		}
		#endregion
		#region LeftIndent
		public int LeftIndent {
			get { return Info.LeftIndent; }
			set {
				if (Info.LeftIndent == value && UseLeftIndent)
					return;
				SetPropertyValue(SetLeftIndentCore, value);
			}
		}
		public bool UseLeftIndent { get { return Info.Options.UseLeftIndent; } }
		protected internal virtual DocumentModelChangeActions SetLeftIndentCore(ParagraphFormattingBase info, int value) {
			info.LeftIndent = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.LeftIndent);
		}
		#endregion
		#region LineSpacing
		public float LineSpacing {
			get { return Info.LineSpacing; }
			set {
				if (Info.LineSpacing == value && UseLineSpacing)
					return;
				SetPropertyValue(SetLineSpacingCore, value);
			}
		}
		public bool UseLineSpacing { get { return Info.Options.UseLineSpacing; } }
		protected internal virtual DocumentModelChangeActions SetLineSpacingCore(ParagraphFormattingBase info, float value) {
			info.LineSpacing = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.LineSpacing);
		}
		#endregion
		#region LineSpacingType
		public ParagraphLineSpacing LineSpacingType {
			get { return Info.LineSpacingType; }
			set {
				if (Info.LineSpacingType == value && UseLineSpacingType)
					return;
				SetPropertyValue(SetLineSpacingTypeCore, value);
			}
		}
		public bool UseLineSpacingType { get { return Info.Options.UseLineSpacing; } }
		protected internal virtual DocumentModelChangeActions SetLineSpacingTypeCore(ParagraphFormattingBase info, ParagraphLineSpacing value) {
			info.LineSpacingType = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.LineSpacingType);
		}
		#endregion
		#region RightIndent
		public int RightIndent {
			get { return Info.RightIndent; }
			set {
				if (Info.RightIndent == value && UseRightIndent)
					return;
				SetPropertyValue(SetRightIndentCore, value);
			}
		}
		public bool UseRightIndent { get { return Info.Options.UseRightIndent; } }
		protected internal virtual DocumentModelChangeActions SetRightIndentCore(ParagraphFormattingBase info, int value) {
			info.RightIndent = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.RightIndent);
		}
		#endregion
		#region SpacingAfter
		public int SpacingAfter {
			get { return Info.SpacingAfter; }
			set {
				if (Info.SpacingAfter == value && UseSpacingAfter)
					return;
				SetPropertyValue(SetSpacingAfterCore, value);
			}
		}
		public bool UseSpacingAfter { get { return Info.Options.UseSpacingAfter; } }
		protected internal virtual DocumentModelChangeActions SetSpacingAfterCore(ParagraphFormattingBase info, int value) {
			info.SpacingAfter = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.SpacingAfter);
		}
		#endregion
		#region SpacingBefore
		public int SpacingBefore {
			get { return Info.SpacingBefore; }
			set {
				if (Info.SpacingBefore == value && UseSpacingBefore)
					return;
				SetPropertyValue(SetSpacingBeforeCore, value);
			}
		}
		public bool UseSpacingBefore { get { return Info.Options.UseSpacingBefore; } }
		protected internal virtual DocumentModelChangeActions SetSpacingBeforeCore(ParagraphFormattingBase info, int value) {
			info.SpacingBefore = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.SpacingBefore);
		}
		#endregion
		#region SuppressHyphenation
		public bool SuppressHyphenation {
			get { return Info.SuppressHyphenation; }
			set {
				if (Info.SuppressHyphenation == value && UseSuppressHyphenation)
					return;
				SetPropertyValue(SetSuppressHyphenationCore, value);
			}
		}
		public bool UseSuppressHyphenation { get { return Info.Options.UseSuppressHyphenation; } }
		protected internal virtual DocumentModelChangeActions SetSuppressHyphenationCore(ParagraphFormattingBase info, bool value) {
			info.SuppressHyphenation = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.SuppressHyphenation);
		}
		#endregion
		#region SuppressLineNumbers
		public bool SuppressLineNumbers {
			get { return Info.SuppressLineNumbers; }
			set {
				if (Info.SuppressLineNumbers == value && UseSuppressLineNumbers)
					return;
				SetPropertyValue(SetSuppressLineNumbersCore, value);
			}
		}
		public bool UseSuppressLineNumbers { get { return Info.Options.UseSuppressLineNumbers; } }
		protected internal virtual DocumentModelChangeActions SetSuppressLineNumbersCore(ParagraphFormattingBase info, bool value) {
			info.SuppressLineNumbers = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.SuppressLineNumbers);
		}
		#endregion
		#region ContextualSpacing
		public bool ContextualSpacing {
			get { return Info.ContextualSpacing; }
			set {
				if (Info.ContextualSpacing == value && UseContextualSpacing)
					return;
				SetPropertyValue(SetContextualSpacingCore, value);
			}
		}
		public bool UseContextualSpacing { get { return Info.Options.UseContextualSpacing; } }
		protected internal virtual DocumentModelChangeActions SetContextualSpacingCore(ParagraphFormattingBase info, bool value) {
			info.ContextualSpacing = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.ContextualSpacing);
		}
		#endregion
		#region PageBreakBefore
		public bool PageBreakBefore {
			get { return Info.PageBreakBefore; }
			set {
				if (Info.PageBreakBefore == value && UsePageBreakBefore)
					return;
				SetPropertyValue(SetPageBreakBeforeCore, value);
			}
		}
		public bool UsePageBreakBefore { get { return Info.Options.UsePageBreakBefore; } }
		protected internal virtual DocumentModelChangeActions SetPageBreakBeforeCore(ParagraphFormattingBase info, bool value) {
			info.PageBreakBefore = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.PageBreakBefore);
		}
		#endregion
		#region BeforeAutoSpacing
		public bool BeforeAutoSpacing {
			get { return Info.BeforeAutoSpacing; }
			set {
				if (Info.BeforeAutoSpacing == value && UseBeforeAutoSpacing)
					return;
				SetPropertyValue(SetBeforeAutoSpacingCore, value);
			}
		}
		public bool UseBeforeAutoSpacing { get { return Info.Options.UseBeforeAutoSpacing; } }
		protected internal virtual DocumentModelChangeActions SetBeforeAutoSpacingCore(ParagraphFormattingBase info, bool value) {
			info.BeforeAutoSpacing = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BeforeAutoSpacing);
		}
		#endregion
		#region AfterAutoSpacing
		public bool AfterAutoSpacing {
			get { return Info.AfterAutoSpacing; }
			set {
				if (Info.AfterAutoSpacing == value && UseAfterAutoSpacing)
					return;
				SetPropertyValue(SetAfterAutoSpacingCore, value);
			}
		}
		public bool UseAfterAutoSpacing { get { return Info.Options.UseAfterAutoSpacing; } }
		protected internal virtual DocumentModelChangeActions SetAfterAutoSpacingCore(ParagraphFormattingBase info, bool value) {
			info.AfterAutoSpacing = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.AfterAutoSpacing);
		}
		#endregion
		#region KeepWithNext
		public bool KeepWithNext {
			get { return Info.KeepWithNext; }
			set {
				if (Info.KeepWithNext == value && UseKeepWithNext)
					return;
				SetPropertyValue(SetKeepWithNextCore, value);
			}
		}
		public bool UseKeepWithNext { get { return Info.Options.UseKeepWithNext; } }
		protected internal virtual DocumentModelChangeActions SetKeepWithNextCore(ParagraphFormattingBase info, bool value) {
			info.KeepWithNext = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.KeepWithNext);
		}
		#endregion
		#region KeepLinesTogether
		public bool KeepLinesTogether {
			get { return Info.KeepLinesTogether; }
			set {
				if (Info.KeepLinesTogether == value && UseKeepLinesTogether)
					return;
				SetPropertyValue(SetKeepLinesTogetherCore, value);
			}
		}
		public bool UseKeepLinesTogether { get { return Info.Options.UseKeepLinesTogether; } }
		protected internal virtual DocumentModelChangeActions SetKeepLinesTogetherCore(ParagraphFormattingBase info, bool value) {
			info.KeepLinesTogether = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.KeepLinesTogether);
		}
		#endregion
		#region WidowOrphanControl
		public bool WidowOrphanControl {
			get { return Info.WidowOrphanControl; }
			set {
				if (Info.WidowOrphanControl == value && UseWidowOrphanControl)
					return;
				SetPropertyValue(SetWidowOrphanControlCore, value);
			}
		}
		public bool UseWidowOrphanControl { get { return Info.Options.UseWidowOrphanControl; } }
		protected internal virtual DocumentModelChangeActions SetWidowOrphanControlCore(ParagraphFormattingBase info, bool value) {
			info.WidowOrphanControl = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.WidowOrphanControl);
		}
		#endregion
		#region OutlineLevel
		public int OutlineLevel {
			get { return Info.OutlineLevel; }
			set {
				if (Info.OutlineLevel == value && UseOutlineLevel)
					return;
				SetPropertyValue(SetOutlineLevelCore, value);
			}
		}
		public bool UseOutlineLevel { get { return Info.Options.UseOutlineLevel; } }
		protected internal virtual DocumentModelChangeActions SetOutlineLevelCore(ParagraphFormattingBase info, int value) {
			info.OutlineLevel = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.OutlineLevel);
		}
		#endregion
		#region BackColor
		public Color BackColor {
			get { return Info.BackColor; }
			set {
				if (Info.BackColor == value && UseBackColor)
					return;
				SetPropertyValue(SetBackColorCore, value);
			}
		}
		public bool UseBackColor { get { return Info.Options.UseBackColor; } }
		protected internal virtual DocumentModelChangeActions SetBackColorCore(ParagraphFormattingBase info, Color value) {
			info.BackColor = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BackColor);
		}
		#endregion
#if THEMES_EDIT        
		#region Shading
		public Shading Shading {
			get { return Info.Shading; }
			set {
				if (Info.Shading == value && UseBackColor)
					return;
				SetPropertyValue(SetShadingCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetShadingCore(ParagraphFormattingBase info, Shading value) {
			info.Shading = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BackColor);
		}
		#endregion
#endif
		#region LeftBorder
		public BorderInfo LeftBorder {
			get { return Info.LeftBorder; }
			set {
				if (Info.LeftBorder == value && UseLeftBorder)
					return;
				SetPropertyValue(SetLeftBorderCore, value);
			}
		}
		public bool UseLeftBorder { get { return Info.Options.UseLeftBorder; } }
		protected internal virtual DocumentModelChangeActions SetLeftBorderCore(ParagraphFormattingBase info, BorderInfo value) {
			info.LeftBorder = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.LeftBorder);
		}
		#endregion
		#region RightBorder
		public BorderInfo RightBorder {
			get { return Info.RightBorder; }
			set {
				if (Info.RightBorder == value && UseRightBorder)
					return;
				SetPropertyValue(SetRightBorderCore, value);
			}
		}
		public bool UseRightBorder { get { return Info.Options.UseRightBorder; } }
		protected internal virtual DocumentModelChangeActions SetRightBorderCore(ParagraphFormattingBase info, BorderInfo value) {
			info.RightBorder = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.RightBorder);
		}
		#endregion
		#region TopBorder
		public BorderInfo TopBorder {
			get { return Info.TopBorder; }
			set {
				if (Info.TopBorder == value && UseTopBorder)
					return;
				SetPropertyValue(SetTopBorderCore, value);
			}
		}
		public bool UseTopBorder { get { return Info.Options.UseTopBorder; } }
		protected internal virtual DocumentModelChangeActions SetTopBorderCore(ParagraphFormattingBase info, BorderInfo value) {
			info.TopBorder = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.TopBorder);
		}
		#endregion
		#region BottomBorder
		public BorderInfo BottomBorder {
			get { return Info.BottomBorder; }
			set {
				if (Info.BottomBorder == value && UseBottomBorder)
					return;
				SetPropertyValue(SetBottomBorderCore, value);
			}
		}
		public bool UseBottomBorder { get { return Info.Options.UseBottomBorder; } }
		protected internal virtual DocumentModelChangeActions SetBottomBorderCore(ParagraphFormattingBase info, BorderInfo value) {
			info.BottomBorder = value;
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BottomBorder);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<ParagraphFormattingBase> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.ParagraphFormattingCache;
		}
		protected internal bool UseVal(ParagraphFormattingOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public void Reset() {
			ParagraphFormattingBase info = GetInfoForModification();
			ParagraphFormattingBase emptyInfo = GetCache(DocumentModel)[ParagraphFormattingCache.EmptyParagraphFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override bool Equals(object obj) {
			ParagraphProperties other = obj as ParagraphProperties;
			if (ReferenceEquals(obj, null))
				return false;
			if (DocumentModel == other.DocumentModel)
				return Index == other.Index;
			else
				return Info.Equals(other.Info);
		}
		internal void ResetUse(ParagraphFormattingOptions.Mask mask) {
			ParagraphFormattingBase info = GetInfoForModification();
			ParagraphFormattingOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			ParagraphFormattingBase info = GetInfoForModification();
			ParagraphFormattingOptions options = info.GetOptionsForModification();
			options.Value = ParagraphFormattingOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override int GetHashCode() {
			return Index;
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("FormattingInfoIndex:{0}, InfoIndex:{1}, OptionsIndex:{2}", Index, Info.InfoIndex, Info.OptionsIndex);
		}
#endif
		internal virtual void Merge(ParagraphProperties properties) {
			ParagraphPropertiesMerger merger = new ParagraphPropertiesMerger(this);
			merger.Merge(properties);
			CopyFrom(merger.MergedProperties);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			owner.OnParagraphPropertiesChanged();
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return owner.CreateParagraphPropertiesChangedHistoryItem();
		}
		public void CopyFrom(MergedProperties<ParagraphFormattingInfo, ParagraphFormattingOptions> paragraphProperties) {
			ParagraphFormattingBase info = GetInfoForModification();
			info.CopyFromCore(paragraphProperties.Info, paragraphProperties.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		protected internal override IObtainAffectedRangeListener GetObtainAffectedRangeListener() {
			return owner as IObtainAffectedRangeListener;
		}
	}
	#endregion
	#region ParagraphFormattingChangeType
	public enum ParagraphFormattingChangeType {
		None = 0,
		Alignment,
		LeftIndent,
		RightIndent,
		SpacingBefore,
		SpacingAfter,
		LineSpacingType,
		LineSpacing,
		FirstLineIndentType,
		FirstLineIndent,
		SuppressHyphenation,
		SuppressLineNumbers,
		ContextualSpacing,
		PageBreakBefore,
		BeforeAutoSpacing,
		AfterAutoSpacing,
		KeepWithNext,
		KeepLinesTogether,
		WidowOrphanControl,
		OutlineLevel,
		BackColor,
		ParagraphStyle,
		BatchUpdate,
		NumberingListIndex,
		LeftBorder,
		RightBorder,
		TopBorder,
		BottomBorder
	}
	#endregion
	#region ParagraphFormattingChangeActionsCalculator
	public static class ParagraphFormattingChangeActionsCalculator {
		internal class ParagraphFormattingChangeActionsTable : Dictionary<ParagraphFormattingChangeType, DocumentModelChangeActions> {
		}
		internal static readonly ParagraphFormattingChangeActionsTable paragraphFormattingChangeActionsTable = CreateParagraphFormattingChangeActionsTable();
		internal static ParagraphFormattingChangeActionsTable CreateParagraphFormattingChangeActionsTable() {
			ParagraphFormattingChangeActionsTable table = new ParagraphFormattingChangeActionsTable();
			table.Add(ParagraphFormattingChangeType.None, DocumentModelChangeActions.None);
			table.Add(ParagraphFormattingChangeType.Alignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.LeftIndent, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(ParagraphFormattingChangeType.RightIndent, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(ParagraphFormattingChangeType.SpacingBefore, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.SpacingAfter, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.LineSpacingType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.LineSpacing, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.FirstLineIndentType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(ParagraphFormattingChangeType.FirstLineIndent, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(ParagraphFormattingChangeType.SuppressHyphenation, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.SuppressLineNumbers, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.ContextualSpacing, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.PageBreakBefore, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetAllPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.BeforeAutoSpacing, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.AfterAutoSpacing, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.KeepWithNext, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.KeepLinesTogether, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.WidowOrphanControl, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.OutlineLevel, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.BackColor, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.ParagraphStyle, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.NumberingListIndex, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(ParagraphFormattingChangeType.LeftBorder, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.RightBorder, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.TopBorder, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFormattingChangeType.BottomBorder, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(ParagraphFormattingChangeType change) {
			return paragraphFormattingChangeActionsTable[change];
		}
	}
	#endregion
	#region MergedParagraphProperties
	public class MergedParagraphProperties : MergedProperties<ParagraphFormattingInfo, ParagraphFormattingOptions> {
		public MergedParagraphProperties(ParagraphFormattingInfo info, ParagraphFormattingOptions options)
			: base(info, options) {
		}
	}
	#endregion
	#region ParagraphMergedParagraphPropertiesCachedResult
	public class ParagraphMergedParagraphPropertiesCachedResult {
		int paragraphPropertiesIndex = -1;
		int paragraphStyleIndex = -1;
		int tableStyleParagraphPropertiesIndex = -1;
		int ownListLevelParagraphPropertiesIndex = -1;
		MergedParagraphProperties mergedParagraphProperties;
		public int ParagraphPropertiesIndex { get { return paragraphPropertiesIndex; } set { paragraphPropertiesIndex = value; } }
		public int ParagraphStyleIndex { get { return paragraphStyleIndex; } set { paragraphStyleIndex = value; } }
		public int TableStyleParagraphPropertiesIndex { get { return tableStyleParagraphPropertiesIndex; } set { tableStyleParagraphPropertiesIndex = value; } }
		public int OwnListLevelParagraphPropertiesIndex { get { return ownListLevelParagraphPropertiesIndex; } set { ownListLevelParagraphPropertiesIndex = value; } }
		public MergedParagraphProperties MergedParagraphProperties { get { return mergedParagraphProperties; } set { mergedParagraphProperties = value; } }
	}
	#endregion
	#region ParagraphPropertiesMerger
	public class ParagraphPropertiesMerger : PropertiesMergerBase<ParagraphFormattingInfo, ParagraphFormattingOptions, MergedParagraphProperties> {
		public ParagraphPropertiesMerger(ParagraphProperties initialProperties)
			: base(new MergedParagraphProperties(initialProperties.Info.Info, initialProperties.Info.Options)) {
		}
		public ParagraphPropertiesMerger(MergedParagraphProperties initialProperties)
			: base(new MergedParagraphProperties(initialProperties.Info, initialProperties.Options)) {
		}
		public void Merge(ParagraphProperties properties) {
			MergeCore(properties.Info.Info, properties.Info.Options);
		}
		protected internal override void MergeCore(ParagraphFormattingInfo info, ParagraphFormattingOptions options) {
			if (!OwnOptions.UseAlignment && options.UseAlignment) {
				OwnInfo.Alignment = info.Alignment;
				OwnOptions.UseAlignment = true;
			}
			if (!OwnOptions.UseFirstLineIndent && options.UseFirstLineIndent) {
				OwnInfo.FirstLineIndentType = info.FirstLineIndentType;
				OwnInfo.FirstLineIndent = info.FirstLineIndent;
				OwnOptions.UseFirstLineIndent = true;
			}
			if (!OwnOptions.UseLeftIndent && options.UseLeftIndent) {
				OwnInfo.LeftIndent = info.LeftIndent;
				OwnOptions.UseLeftIndent = true;
			}
			if (!OwnOptions.UseLineSpacing && options.UseLineSpacing) {
				OwnInfo.LineSpacing = info.LineSpacing;
				OwnInfo.LineSpacingType = info.LineSpacingType;
				OwnOptions.UseLineSpacing = true;
			}
			if (!OwnOptions.UseRightIndent && options.UseRightIndent) {
				OwnInfo.RightIndent = info.RightIndent;
				OwnOptions.UseRightIndent = true;
			}
			if (!OwnOptions.UseSpacingAfter && options.UseSpacingAfter) {
				OwnInfo.SpacingAfter = info.SpacingAfter;
				OwnOptions.UseSpacingAfter = true;
			}
			if (!OwnOptions.UseSpacingBefore && options.UseSpacingBefore) {
				OwnInfo.SpacingBefore = info.SpacingBefore;
				OwnOptions.UseSpacingBefore = true;
			}
			if (!OwnOptions.UseSuppressHyphenation && options.UseSuppressHyphenation) {
				OwnInfo.SuppressHyphenation = info.SuppressHyphenation;
				OwnOptions.UseSuppressHyphenation = true;
			}
			if (!OwnOptions.UseSuppressLineNumbers && options.UseSuppressLineNumbers) {
				OwnInfo.SuppressLineNumbers = info.SuppressLineNumbers;
				OwnOptions.UseSuppressLineNumbers = true;
			}
			if (!OwnOptions.UseContextualSpacing && options.UseContextualSpacing) {
				OwnInfo.ContextualSpacing = info.ContextualSpacing;
				OwnOptions.UseContextualSpacing = true;
			}
			if (!OwnOptions.UsePageBreakBefore && options.UsePageBreakBefore) {
				OwnInfo.PageBreakBefore = info.PageBreakBefore;
				OwnOptions.UsePageBreakBefore = true;
			}
			if (!OwnOptions.UseBeforeAutoSpacing && options.UseBeforeAutoSpacing) {
				OwnInfo.BeforeAutoSpacing = info.BeforeAutoSpacing;
				OwnOptions.UseBeforeAutoSpacing = true;
			}
			if (!OwnOptions.UseAfterAutoSpacing && options.UseAfterAutoSpacing) {
				OwnInfo.AfterAutoSpacing = info.AfterAutoSpacing;
				OwnOptions.UseAfterAutoSpacing = true;
			}
			if (!OwnOptions.UseKeepWithNext && options.UseKeepWithNext) {
				OwnInfo.KeepWithNext = info.KeepWithNext;
				OwnOptions.UseKeepWithNext = true;
			}
			if (!OwnOptions.UseKeepLinesTogether && options.UseKeepLinesTogether) {
				OwnInfo.KeepLinesTogether = info.KeepLinesTogether;
				OwnOptions.UseKeepLinesTogether = true;
			}
			if (!OwnOptions.UseWidowOrphanControl && options.UseWidowOrphanControl) {
				OwnInfo.WidowOrphanControl = info.WidowOrphanControl;
				OwnOptions.UseWidowOrphanControl = true;
			}
			if (!OwnOptions.UseOutlineLevel && options.UseOutlineLevel) {
				OwnInfo.OutlineLevel = info.OutlineLevel;
				OwnOptions.UseOutlineLevel = true;
			}
			if (!OwnOptions.UseBackColor && options.UseBackColor) {
				OwnInfo.BackColor = info.BackColor;
#if THEMES_EDIT
				OwnInfo.Shading = info.Shading;
#endif
				OwnOptions.UseBackColor = true;
			}
			if (!OwnOptions.UseTopBorder && options.UseTopBorder) {
				OwnInfo.TopBorder = info.TopBorder;
				OwnOptions.UseTopBorder = true;
			}
			if (!OwnOptions.UseBottomBorder && options.UseBottomBorder) {
				OwnInfo.BottomBorder = info.BottomBorder;
				OwnOptions.UseBottomBorder = true;
			}
			if (!OwnOptions.UseLeftBorder && options.UseLeftBorder) {
				OwnInfo.LeftBorder = info.LeftBorder;
				OwnOptions.UseLeftBorder = true;
			}
			if (!OwnOptions.UseRightBorder && options.UseRightBorder) {
				OwnInfo.RightBorder = info.RightBorder;
				OwnOptions.UseRightBorder = true;
			}
		}
	}
	#endregion
	#region DropCapLocation
	public enum DropCapLocation {
		None = 0,
		Drop,
		Margin
	}
	#endregion
	#region ParagraphFrameHorizontalRule
	public enum ParagraphFrameHorizontalRule {
		Auto = 0,
		AtLeast,
		Exact
	}
	#endregion
	#region ParagraphFrameTextWrapType
	public enum ParagraphFrameTextWrapType {
		Auto = 0,
		Around,
		None,
		NotBeside,
		Through,
		Tight
	}
	#endregion
	#region ParagraphFrameHorizontalPositionType
	public enum ParagraphFrameHorizontalPositionType {
		Page = 0,
		Column,
		Margin
	}
	#endregion
	#region ParagraphFrameHorizontalPositionAlignment
	public enum ParagraphFrameHorizontalPositionAlignment {
		None = 0,
		Left,
		Center,
		Right,
		Inside,
		Outside
	}
	#endregion
	#region ParagraphFrameVerticalPositionType
	public enum ParagraphFrameVerticalPositionType {
		Page = 0,
		Paragraph,
		Margin
	}
	#endregion
	#region ParagraphFrameVerticalPositionAlignment
	public enum ParagraphFrameVerticalPositionAlignment {
		None = 0,
		Top,
		Center,
		Bottom,
		Inline,
		Inside,
		Outside
	}
	#endregion
	#region ParagraphFrameFormattingInfo
	public class ParagraphFrameFormattingInfo : ICloneable<ParagraphFrameFormattingInfo>, ISupportsCopyFrom<ParagraphFrameFormattingInfo>, ISupportsSizeOf {
		#region Fields
		const int MaskHorizontalRule = 0x00000003; 
		const int MaskWrapType = 0x0000001C; 
		const int MaskHorizontalPositionType = 0x00000060; 
		const int MaskVerticalPositionType = 0x00000180; 
		const int MaskHorizontalPositionAlignment = 0x00000E00; 
		const int MaskVerticalPositionAlignment = 0x0000F000; 
		const int MaskDropCap = 0x00030000; 
		const int MaskLockFrameAnchorToParagraph = 0x00040000;
		int packedValues;
		int verticalPosition;
		int horizontalPosition;
		int horizontalPadding;
		int verticalPadding;
		int height;
		int width;
		int x;
		int y;
		int dropCapVerticalHeightInLines;
		#endregion
		public ParagraphFrameFormattingInfo() {
			HorizontalPositionType = ParagraphFrameHorizontalPositionType.Column;
			VerticalPositionType = ParagraphFrameVerticalPositionType.Margin;
		}
		#region Properties
		#region HorizontalPosition
		public int HorizontalPosition {
			get { return horizontalPosition; }
			set {
				if (horizontalPosition == value)
					return;
				horizontalPosition = value;
			}
		}
		#endregion
		#region VerticalPosition
		public int VerticalPosition {
			get { return verticalPosition; }
			set {
				if (verticalPosition == value)
					return;
				verticalPosition = value;
			}
		}
		#endregion
		public int HorizontalPadding {
			get { return horizontalPadding; }
			set {
				if (horizontalPadding == value)
					return;
				horizontalPadding = value;
			}
		}
		public int VerticalPadding {
			get { return verticalPadding; }
			set {
				if (verticalPadding == value)
					return;
				verticalPadding = value;
			}
		}
		public ParagraphFrameHorizontalRule HorizontalRule {
			get { return (ParagraphFrameHorizontalRule)(packedValues & MaskHorizontalRule); }
			set {
				packedValues &= ~MaskHorizontalRule;
				packedValues |= (int)value & MaskHorizontalRule;
			}
		}
		public ParagraphFrameTextWrapType TextWrapType {
			get { return (ParagraphFrameTextWrapType)((packedValues & MaskWrapType) >> 2); }
			set {
				packedValues &= ~MaskWrapType;
				packedValues |= ((int)value << 2) & MaskWrapType;
			}
		}
		#region IParagraphFrameLocation Members
		#region X
		public int X {
			get { return x; }
			set {
				if (X == value)
					return;
				x = value;
			}
		}
		#endregion
		#region Y
		public int Y {
			get { return y; }
			set {
				if (Y == value)
					return;
				y = value;
			}
		}
		#endregion
		public ParagraphFrameHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return (ParagraphFrameHorizontalPositionAlignment)((packedValues & MaskHorizontalPositionAlignment) >> 9); }
			set {
				packedValues &= ~MaskHorizontalPositionAlignment;
				packedValues |= ((int)value << 9) & MaskHorizontalPositionAlignment;
			}
		}
		public ParagraphFrameVerticalPositionAlignment VerticalPositionAlignment {
			get { return (ParagraphFrameVerticalPositionAlignment)((packedValues & MaskVerticalPositionAlignment) >> 12); }
			set {
				packedValues &= ~MaskVerticalPositionAlignment;
				packedValues |= ((int)value << 12) & MaskVerticalPositionAlignment;
			}
		}
		#region HorizontalPositionType
		public ParagraphFrameHorizontalPositionType HorizontalPositionType {
			get { return (ParagraphFrameHorizontalPositionType)((packedValues & MaskHorizontalPositionType) >> 5); }
			set {
				packedValues &= ~MaskHorizontalPositionType;
				packedValues |= ((int)value << 5) & MaskHorizontalPositionType;
			}
		}
		#endregion
		#region VerticalPositionType
		public ParagraphFrameVerticalPositionType VerticalPositionType {
			get { return (ParagraphFrameVerticalPositionType)((packedValues & MaskVerticalPositionType) >> 7); }
			set {
				packedValues &= ~MaskVerticalPositionType;
				packedValues |= ((int)value << 7) & MaskVerticalPositionType;
			}
		}
		#endregion
		#region Width
		public int Width {
			get { return width; }
			set {
				if (Width == value)
					return;
				width = value;
			}
		}
		#endregion
		#region Height
		public int Height {
			get { return height; }
			set {
				if (Height == value)
					return;
				height = value;
			}
		}
		#endregion
		#endregion
		#region DropCap
		public DropCapLocation DropCap {
			get { return (DropCapLocation)((packedValues & MaskDropCap) >> 16); }
			set {
				packedValues &= ~MaskDropCap;
				packedValues |= ((int)value << 16) & MaskDropCap;
			}
		}
		#endregion
		#region DropCapVerticalHeightInLines
		public int DropCapVerticalHeightInLines {
			get {
				return dropCapVerticalHeightInLines;
			}
			set {
				dropCapVerticalHeightInLines = value;
			}
		}
		#endregion
		#region LockFrameAnchorToParagraph
		public bool LockFrameAnchorToParagraph { get { return GetBooleanValue(MaskLockFrameAnchorToParagraph); } set { SetBooleanValue(MaskLockFrameAnchorToParagraph, value); } }
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
		#endregion
		public ParagraphFrameFormattingInfo Clone() {
			ParagraphFrameFormattingInfo result = new ParagraphFrameFormattingInfo();
			result.CopyFrom(this);
			return result;
		}
		public void CopyFrom(ParagraphFrameFormattingInfo val) {
			this.packedValues = val.packedValues;
			this.VerticalPosition = val.VerticalPosition;
			this.HorizontalPosition = val.HorizontalPosition;
			this.HorizontalPadding = val.HorizontalPadding;
			this.VerticalPadding = val.VerticalPadding;
			this.Height = val.Height;
			this.Width = val.Width;
			this.X = val.X;
			this.Y = val.Y;
			this.DropCapVerticalHeightInLines = val.DropCapVerticalHeightInLines;
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			ParagraphFrameFormattingInfo info = (ParagraphFrameFormattingInfo)obj;
			return
				this.packedValues == info.packedValues &&
			this.VerticalPosition == info.VerticalPosition &&
			this.HorizontalPosition == info.HorizontalPosition &&
			this.HorizontalPadding == info.HorizontalPadding &&
			this.VerticalPadding == info.VerticalPadding &&
			this.Height == info.Height &&
			this.Width == info.Width &&
			this.X == info.X &&
			this.Y == info.Y &&
			this.DropCapVerticalHeightInLines == info.DropCapVerticalHeightInLines;
		}
		public override int GetHashCode() {
			return packedValues ^
			verticalPosition ^
			horizontalPosition ^
			horizontalPadding ^
			verticalPadding ^
			height ^
			width ^
			x ^
			y ^
			dropCapVerticalHeightInLines;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ParagraphFrameFormattingInfoCache
	public class ParagraphFrameFormattingInfoCache : UniqueItemsCache<ParagraphFrameFormattingInfo> {
		internal const int DefaultItemIndex = 0;
		public ParagraphFrameFormattingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ParagraphFrameFormattingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ParagraphFrameFormattingInfo defaultItem = new ParagraphFrameFormattingInfo();
			return defaultItem;
		}
	}
	#endregion
	#region ParagraphFrameFormattingCache
	public class ParagraphFrameFormattingCache : UniqueItemsCache<ParagraphFrameFormattingBase> {
		#region Fields
		public const int RootParagraphFrameFormattingIndex = 1;
		public const int EmptyParagraphFrameFormattingIndex = 0;
		readonly DocumentModel documentModel;
		#endregion
		public ParagraphFrameFormattingCache(DocumentModel documentModel)
			: base(documentModel.UnitConverter) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			AppendItem(new ParagraphFrameFormattingBase(DocumentModel.MainPieceTable, DocumentModel, 0, ParagraphFrameFormattingOptionsCache.EmptyParagraphFrameFormattingOptionIndex));
			AppendItem(new ParagraphFrameFormattingBase(DocumentModel.MainPieceTable, DocumentModel, 0, ParagraphFrameFormattingOptionsCache.RootParagraphFrameFormattingOptionIndex));
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected override ParagraphFrameFormattingBase CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return null;
		}
	}
	#endregion
	#region ParagraphFrameFormattingOptions
	public class ParagraphFrameFormattingOptions : ICloneable<ParagraphFrameFormattingOptions>, ISupportsCopyFrom<ParagraphFrameFormattingOptions>, ISupportsSizeOf {
		#region Mask enumeration
		public enum Mask {
			UseNone = 0x00000000,
			UseHorizontalPosition = 0x00000001,
			UseVerticalPosition = 0x00000002,
			UseHorizontalPadding = 0x00000004,
			UseVerticalPadding = 0x00000008,
			UseHorizontalRule = 0x00000010,
			UseTextWrapType = 0x00000020,
			UseX = 0x00000040,
			UseY = 0x00000080,
			UseWidth = 0x00000100,
			UseHeight = 0x00000200,
			UseHorizontalPositionAlignment = 0x00000400,
			UseVerticalPositionAlignment = 0x00000800,
			UseHorizontalPositionType = 0x00001000,
			UseVerticalPositionType = 0x00002000,
			UseDropCap = 0x00004000,
			UseDropCapVerticalHeightInLines = 0x00008000,
			UseLockFrameAnchorToParagraph = 0x00010000,
			UseAll = 0x7FFFFFFF
		}
		#endregion
		Mask val = Mask.UseNone;
		#region Properties
		internal Mask Value { get { return val; } set { val = value; } }
		public bool UseHorizontalPosition { get { return GetVal(Mask.UseHorizontalPosition); } set { SetVal(Mask.UseHorizontalPosition, value); } }
		public bool UseVerticalPosition { get { return GetVal(Mask.UseVerticalPosition); } set { SetVal(Mask.UseVerticalPosition, value); } }
		public bool UseHorizontalPadding { get { return GetVal(Mask.UseHorizontalPadding); } set { SetVal(Mask.UseHorizontalPadding, value); } }
		public bool UseVerticalPadding { get { return GetVal(Mask.UseVerticalPadding); } set { SetVal(Mask.UseVerticalPadding, value); } }
		public bool UseHorizontalRule { get { return GetVal(Mask.UseHorizontalRule); } set { SetVal(Mask.UseHorizontalRule, value); } }
		public bool UseTextWrapType { get { return GetVal(Mask.UseTextWrapType); } set { SetVal(Mask.UseTextWrapType, value); } }
		public bool UseX { get { return GetVal(Mask.UseX); } set { SetVal(Mask.UseX, value); } }
		public bool UseY { get { return GetVal(Mask.UseY); } set { SetVal(Mask.UseY, value); } }
		public bool UseWidth { get { return GetVal(Mask.UseWidth); } set { SetVal(Mask.UseWidth, value); } }
		public bool UseHeight { get { return GetVal(Mask.UseHeight); } set { SetVal(Mask.UseHeight, value); } }
		public bool UseHorizontalPositionAlignment { get { return GetVal(Mask.UseHorizontalPositionAlignment); } set { SetVal(Mask.UseHorizontalPositionAlignment, value); } }
		public bool UseVerticalPositionAlignment { get { return GetVal(Mask.UseVerticalPositionAlignment); } set { SetVal(Mask.UseVerticalPositionAlignment, value); } }
		public bool UseHorizontalPositionType { get { return GetVal(Mask.UseHorizontalPositionType); } set { SetVal(Mask.UseHorizontalPositionType, value); } }
		public bool UseVerticalPositionType { get { return GetVal(Mask.UseVerticalPositionType); } set { SetVal(Mask.UseVerticalPositionType, value); } }
		public bool UseDropCap { get { return GetVal(Mask.UseDropCap); } set { SetVal(Mask.UseDropCap, value); } }
		public bool UseDropCapVerticalHeightInLines { get { return GetVal(Mask.UseDropCapVerticalHeightInLines); } set { SetVal(Mask.UseDropCapVerticalHeightInLines, value); } }
		public bool UseLockFrameAnchorToParagraph { get { return GetVal(Mask.UseLockFrameAnchorToParagraph); } set { SetVal(Mask.UseLockFrameAnchorToParagraph, value); } }
		#endregion
		#region GetVal/SetVal helpers
		void SetVal(Mask mask, bool bitVal) {
			if (bitVal)
				val |= mask;
			else
				val &= ~mask;
		}
		public bool GetVal(Mask mask) {
			return (val & mask) != 0;
		}
		#endregion
		public ParagraphFrameFormattingOptions() {
		}
		internal ParagraphFrameFormattingOptions(Mask val) {
			this.val = val;
		}
		public ParagraphFrameFormattingOptions Clone() {
			return new ParagraphFrameFormattingOptions(this.val);
		}
		public override bool Equals(object obj) {
			ParagraphFrameFormattingOptions opts = (ParagraphFrameFormattingOptions)obj;
			return opts.Value == this.Value;
		}
		public override int GetHashCode() {
			return (int)this.Value;
		}
		public void CopyFrom(ParagraphFrameFormattingOptions options) {
			this.val = options.Value;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ParagraphFrameFormattingOptionsCache
	public class ParagraphFrameFormattingOptionsCache : UniqueItemsCache<ParagraphFrameFormattingOptions> {
		internal const int EmptyParagraphFrameFormattingOptionIndex = 0;
		internal const int RootParagraphFrameFormattingOptionIndex = 1;
		public ParagraphFrameFormattingOptionsCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override void InitItems(IDocumentModelUnitConverter unitConverter) {
			base.InitItems(unitConverter);
			AddRootStyleOptions();
		}
		protected override ParagraphFrameFormattingOptions CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ParagraphFrameFormattingOptions();
		}
		void AddRootStyleOptions() {
			AppendItem(new ParagraphFrameFormattingOptions(ParagraphFrameFormattingOptions.Mask.UseAll));
		}
	}
	#endregion
	public interface IParagraphFrameLocation {
		int X { get; }
		int Y { get; }
		ParagraphFrameHorizontalPositionAlignment HorizontalPositionAlignment { get; }
		ParagraphFrameVerticalPositionAlignment VerticalPositionAlignment { get; }
		ParagraphFrameHorizontalPositionType HorizontalPositionType { get; }
		ParagraphFrameVerticalPositionType VerticalPositionType { get; }
		int Width { get; }
		int Height { get; }
	}
	#region ParagraphFrameFormattingBase
	public class ParagraphFrameFormattingBase : IndexBasedObjectB<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions>, ICloneable<ParagraphFrameFormattingBase>, ISupportsCopyFrom<ParagraphFrameFormattingBase>, IParagraphFrameLocation {
		internal ParagraphFrameFormattingBase(PieceTable pieceTable, DocumentModel documentModel, int formattingInfoIndex, int formattingOptionsIndex)
			: base(pieceTable, documentModel, formattingInfoIndex, formattingOptionsIndex) {
		}
		protected override UniqueItemsCache<ParagraphFrameFormattingInfo> InfoCache { get { return ((DocumentModel)DocumentModel).Cache.ParagraphFrameFormattingInfoCache; } }
		protected override UniqueItemsCache<ParagraphFrameFormattingOptions> OptionsCache { get { return ((DocumentModel)DocumentModel).Cache.ParagraphFrameFormattingOptionsCache; } }
		#region Properties
		#region HorizontalPosition
		public int HorizontalPosition {
			get { return Info.HorizontalPosition; }
			set {
				if (Info.HorizontalPosition == value && Options.UseHorizontalPosition)
					return;
				SetPropertyValue(SetHorizontalPositionCore, value, SetUseHorizontalPositionCore);
			}
		}
		void SetHorizontalPositionCore(ParagraphFrameFormattingInfo info, int value) {
			info.HorizontalPosition = value;
		}
		void SetUseHorizontalPositionCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHorizontalPosition = value;
		}
		#endregion
		#region VerticalPosition
		public int VerticalPosition {
			get { return Info.VerticalPosition; }
			set {
				if (Info.VerticalPosition == value && Options.UseVerticalPosition)
					return;
				SetPropertyValue(SetVerticalPositionCore, value, SetUseVerticalPositionCore);
			}
		}
		void SetVerticalPositionCore(ParagraphFrameFormattingInfo info, int value) {
			info.VerticalPosition = value;
		}
		void SetUseVerticalPositionCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseVerticalPosition = value;
		}
		#endregion
		public int HorizontalPadding {
			get { return Info.HorizontalPadding; }
			set {
				if (Info.HorizontalPadding == value && Options.UseHorizontalPadding)
					return;
				SetPropertyValue(SetHorizontalPaddingCore, value, SetUseHorizontalPaddingCore);
			}
		}
		void SetHorizontalPaddingCore(ParagraphFrameFormattingInfo info, int value) {
			info.HorizontalPadding = value;
		}
		void SetUseHorizontalPaddingCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHorizontalPadding = value;
		}
		public int VerticalPadding {
			get { return Info.VerticalPadding; }
			set {
				if (Info.VerticalPadding == value && Options.UseVerticalPadding)
					return;
				SetPropertyValue(SetVerticalPaddingCore, value, SetUseVerticalPaddingCore);
			}
		}
		void SetVerticalPaddingCore(ParagraphFrameFormattingInfo info, int value) {
			info.VerticalPadding = value;
		}
		void SetUseVerticalPaddingCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseVerticalPadding = value;
		}
		public ParagraphFrameHorizontalRule HorizontalRule {
			get { return Info.HorizontalRule; }
			set {
				if (Info.HorizontalRule == value && Options.UseHorizontalRule)
					return;
				SetPropertyValue(SetHorizontalRuleCore, value, SetUseHorizontalRuleCore);
			}
		}
		void SetHorizontalRuleCore(ParagraphFrameFormattingInfo info, ParagraphFrameHorizontalRule value) {
			info.HorizontalRule = value;
		}
		void SetUseHorizontalRuleCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHorizontalRule = value;
		}
		public ParagraphFrameTextWrapType TextWrapType {
			get { return Info.TextWrapType; }
			set {
				if (Info.TextWrapType == value && Options.UseTextWrapType)
					return;
				SetPropertyValue(SetTextWrapTypeCore, value, SetUseTextWrapTypeCore);
			}
		}
		void SetTextWrapTypeCore(ParagraphFrameFormattingInfo info, ParagraphFrameTextWrapType value) {
			info.TextWrapType = value;
		}
		void SetUseTextWrapTypeCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseTextWrapType = value;
		}
		#region IParagraphFrameLocation Members
		#region X
		public int X {
			get { return Info.X; }
			set {
				if (Info.X == value && Options.UseX)
					return;
				SetPropertyValue(SetXCore, value, SetUseXCore);
			}
		}
		void SetXCore(ParagraphFrameFormattingInfo info, int value) {
			info.X = value;
		}
		void SetUseXCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseX = value;
		}
		#endregion
		#region Y
		public int Y {
			get { return Info.Y; }
			set {
				if (Info.Y == value && Options.UseY)
					return;
				SetPropertyValue(SetYCore, value, SetUseYCore);
			}
		}
		void SetYCore(ParagraphFrameFormattingInfo info, int value) {
			info.Y = value;
		}
		void SetUseYCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseY = value;
		}
		#endregion
		public ParagraphFrameHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return Info.HorizontalPositionAlignment; }
			set {
				if (Info.HorizontalPositionAlignment == value && Options.UseHorizontalPositionAlignment)
					return;
				SetPropertyValue(SetHorizontalPositionAlignmentCore, value, SetUseHorizontalPositionAlignmentCore);
			}
		}
		void SetHorizontalPositionAlignmentCore(ParagraphFrameFormattingInfo info, ParagraphFrameHorizontalPositionAlignment value) {
			info.HorizontalPositionAlignment = value;
		}
		void SetUseHorizontalPositionAlignmentCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHorizontalPositionAlignment = value;
		}
		public ParagraphFrameVerticalPositionAlignment VerticalPositionAlignment {
			get { return Info.VerticalPositionAlignment; }
			set {
				if (Info.VerticalPositionAlignment == value && Options.UseVerticalPositionAlignment)
					return;
				SetPropertyValue(SetVerticalPositionAlignmentCore, value, SetUseVerticalPositionAlignmentCore);
			}
		}
		void SetVerticalPositionAlignmentCore(ParagraphFrameFormattingInfo info, ParagraphFrameVerticalPositionAlignment value) {
			info.VerticalPositionAlignment = value;
		}
		void SetUseVerticalPositionAlignmentCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseVerticalPositionAlignment = value;
		}
		#region HorizontalPositionType
		public ParagraphFrameHorizontalPositionType HorizontalPositionType {
			get { return Info.HorizontalPositionType; }
			set {
				if (Info.HorizontalPositionType == value && Options.UseHorizontalPositionType)
					return;
				SetPropertyValue(SetHorizontalPositionTypeCore, value, SetUseHorizontalPositionTypeCore);
			}
		}
		void SetHorizontalPositionTypeCore(ParagraphFrameFormattingInfo info, ParagraphFrameHorizontalPositionType value) {
			info.HorizontalPositionType = value;
		}
		void SetUseHorizontalPositionTypeCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHorizontalPositionType = value;
		}
		#endregion
		#region VerticalPositionType
		public ParagraphFrameVerticalPositionType VerticalPositionType {
			get { return Info.VerticalPositionType; }
			set {
				if (Info.VerticalPositionType == value && Options.UseVerticalPositionType)
					return;
				SetPropertyValue(SetVerticalPositionTypeCore, value, SetUseVerticalPositionTypeCore);
			}
		}
		void SetVerticalPositionTypeCore(ParagraphFrameFormattingInfo info, ParagraphFrameVerticalPositionType value) {
			info.VerticalPositionType = value;
		}
		void SetUseVerticalPositionTypeCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseVerticalPositionType = value;
		}
		#endregion
		#region Width
		public int Width {
			get { return Info.Width; }
			set {
				if (Info.Width == value && Options.UseWidth)
					return;
				SetPropertyValue(SetWidthCore, value, SetUseWidthCore);
			}
		}
		void SetWidthCore(ParagraphFrameFormattingInfo info, int value) {
			info.Width = value;
		}
		void SetUseWidthCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseWidth = value;
		}
		#endregion
		#region Height
		public int Height {
			get { return Info.Height; }
			set {
				if (Info.Height == value && Options.UseHeight)
					return;
				SetPropertyValue(SetHeightCore, value, SetUseHeightCore);
			}
		}
		void SetHeightCore(ParagraphFrameFormattingInfo info, int value) {
			info.Height = value;
		}
		void SetUseHeightCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseHeight = value;
		}
		#endregion
		#endregion
		#region DropCap
		public DropCapLocation DropCap {
			get { return Info.DropCap; }
			set {
				if (Info.DropCap == value && Options.UseDropCap)
					return;
				SetPropertyValue(SetDropCapCore, value, SetUseDropCapCore);
			}
		}
		void SetDropCapCore(ParagraphFrameFormattingInfo info, DropCapLocation value) {
			info.DropCap = value;
		}
		void SetUseDropCapCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseDropCap = value;
		}
		#endregion
		#region DropCapVerticalHeightInLines
		public int DropCapVerticalHeightInLines {
			get { return Info.DropCapVerticalHeightInLines; }
			set {
				if (Info.DropCapVerticalHeightInLines == value && Options.UseDropCapVerticalHeightInLines)
					return;
				SetPropertyValue(SetDropCapVerticalHeightInLinesCore, value, SetUseDropCapVerticalHeightInLinesCore);
			}
		}
		void SetDropCapVerticalHeightInLinesCore(ParagraphFrameFormattingInfo info, int value) {
			info.DropCapVerticalHeightInLines = value;
		}
		void SetUseDropCapVerticalHeightInLinesCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseDropCapVerticalHeightInLines = value;
		}
		#endregion
		#region LockFrameAnchorToParagraph
		public bool LockFrameAnchorToParagraph {
			get { return Info.LockFrameAnchorToParagraph; }
			set {
				if (Info.LockFrameAnchorToParagraph == value && Options.UseLockFrameAnchorToParagraph)
					return;
				SetPropertyValue(SetLockFrameAnchorToParagraphCore, value, SetUseLockFrameAnchorToParagraphCore);
			}
		}
		void SetLockFrameAnchorToParagraphCore(ParagraphFrameFormattingInfo info, bool value) {
			info.LockFrameAnchorToParagraph = value;
		}
		void SetUseLockFrameAnchorToParagraphCore(ParagraphFrameFormattingOptions options, bool value) {
			options.UseLockFrameAnchorToParagraph = value;
		}
		#endregion
		#endregion
		#region UseValue
		public ParagraphFrameFormattingOptions.Mask UseValue {
			get { return Options.Value; }
			internal set { Options.Value = value; }
		}
		#endregion
		#region ICloneable<ParagraphFormattingBase> Members
		public ParagraphFrameFormattingBase Clone() {
			return new ParagraphFrameFormattingBase(PieceTable, (DocumentModel)DocumentModel, InfoIndex, OptionsIndex);
		}
		#endregion
		public void CopyFrom(ParagraphFrameFormattingBase paragraphFormatting) {
			CopyFrom(paragraphFormatting.Info, paragraphFormatting.Options);
		}
		public void CopyFrom(ParagraphFrameFormattingInfo info, ParagraphFrameFormattingOptions options) {
			CopyFromCore(info, options);
		}
		protected override bool PropertyEquals(IndexBasedObject<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions> other) {
			Guard.ArgumentNotNull(other, "other");
			return Options.Value == other.Options.Value &&
				Info.Equals(other.Info);
		}
		protected override void SetPropertyValue<U>(IndexBasedObjectB<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions>.SetPropertyValueDelegate<U> setter, U newValue, IndexBasedObjectB<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions>.SetOptionsValueDelegate optionsSetter) {
			if (((DocumentModel)DocumentModel).DocumentCapabilities.ParagraphFormattingAllowed)
				base.SetPropertyValue<U>(setter, newValue, optionsSetter);
		}
	}
	#endregion
	public class FrameProperties : RichEditIndexBasedObject<ParagraphFrameFormattingBase>, IParagraphFrameLocation {
		#region Static
		public static void ApplyPropertiesDiff(FrameProperties target, ParagraphFrameFormattingInfo targetMergedInfo, ParagraphFrameFormattingInfo sourceMergedInfo) {
			if (targetMergedInfo.HorizontalPosition != sourceMergedInfo.HorizontalPosition)
				target.HorizontalPosition = sourceMergedInfo.HorizontalPosition;
			if (targetMergedInfo.VerticalPosition != sourceMergedInfo.VerticalPosition)
				target.VerticalPosition = sourceMergedInfo.VerticalPosition;
			if (targetMergedInfo.HorizontalPadding != sourceMergedInfo.HorizontalPadding)
				target.HorizontalPadding = sourceMergedInfo.HorizontalPadding;
			if (targetMergedInfo.VerticalPadding != sourceMergedInfo.VerticalPadding)
				target.VerticalPadding = sourceMergedInfo.VerticalPadding;
			if (targetMergedInfo.HorizontalRule != sourceMergedInfo.HorizontalRule)
				target.HorizontalRule = sourceMergedInfo.HorizontalRule;
			if (targetMergedInfo.TextWrapType != sourceMergedInfo.TextWrapType)
				target.TextWrapType = sourceMergedInfo.TextWrapType;
			if (targetMergedInfo.X != sourceMergedInfo.X)
				target.X = sourceMergedInfo.X;
			if (targetMergedInfo.Y != sourceMergedInfo.Y)
				target.Y = sourceMergedInfo.Y;
			if (targetMergedInfo.Width != sourceMergedInfo.Width)
				target.Width = sourceMergedInfo.Width;
			if (targetMergedInfo.Height != sourceMergedInfo.Height)
				target.Height = sourceMergedInfo.Height;
			if (targetMergedInfo.HorizontalPositionAlignment != sourceMergedInfo.HorizontalPositionAlignment)
				target.HorizontalPositionAlignment = sourceMergedInfo.HorizontalPositionAlignment;
			if (targetMergedInfo.VerticalPositionAlignment != sourceMergedInfo.VerticalPositionAlignment)
				target.VerticalPositionAlignment = sourceMergedInfo.VerticalPositionAlignment;
			if (targetMergedInfo.HorizontalPositionType != sourceMergedInfo.HorizontalPositionType)
				target.HorizontalPositionType = sourceMergedInfo.HorizontalPositionType;
			if (targetMergedInfo.VerticalPositionType != sourceMergedInfo.VerticalPositionType)
				target.VerticalPositionType = sourceMergedInfo.VerticalPositionType;
			if (targetMergedInfo.DropCap != sourceMergedInfo.DropCap)
				target.DropCap = sourceMergedInfo.DropCap;
			if (targetMergedInfo.DropCapVerticalHeightInLines != sourceMergedInfo.DropCapVerticalHeightInLines)
				target.DropCapVerticalHeightInLines = sourceMergedInfo.DropCapVerticalHeightInLines;
			if (targetMergedInfo.LockFrameAnchorToParagraph != sourceMergedInfo.LockFrameAnchorToParagraph)
				target.LockFrameAnchorToParagraph = sourceMergedInfo.LockFrameAnchorToParagraph;
		}
		#endregion
		readonly IParagraphPropertiesContainer owner;
		public FrameProperties(IParagraphPropertiesContainer owner)
			: base(GetPieceTable(owner)) {
			this.owner = owner;
		}
		static PieceTable GetPieceTable(IParagraphPropertiesContainer owner) {
			Guard.ArgumentNotNull(owner, "owner");
			return owner.PieceTable;
		}
		#region Properties
		#region VerticalPosition
		public int VerticalPosition {
			get { return Info.VerticalPosition; }
			set {
				if (Info.VerticalPosition == value && UseVerticalPosition)
					return;
				SetPropertyValue(SetVerticalPositionCore, value);
			}
		}
		public bool UseVerticalPosition { get { return Info.Options.UseVerticalPosition; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPositionCore(ParagraphFrameFormattingBase info, int value) {
			info.VerticalPosition = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.VerticalPosition);
		}
		#endregion
		#region HorizontalPosition
		public int HorizontalPosition {
			get { return Info.HorizontalPosition; }
			set {
				if (Info.HorizontalPosition == value && UseHorizontalPosition)
					return;
				SetPropertyValue(SetHorizontalPositionCore, value);
			}
		}
		public bool UseHorizontalPosition { get { return Info.Options.UseHorizontalPosition; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPositionCore(ParagraphFrameFormattingBase info, int value) {
			info.HorizontalPosition = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.HorizontalPosition);
		}
		#endregion
		#region HorizontalPadding
		public int HorizontalPadding {
			get { return Info.HorizontalPadding; }
			set {
				if (Info.HorizontalPadding == value && UseHorizontalPadding)
					return;
				SetPropertyValue(SetHorizontalPaddingCore, value);
			}
		}
		public bool UseHorizontalPadding { get { return Info.Options.UseHorizontalPadding; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPaddingCore(ParagraphFrameFormattingBase info, int value) {
			info.HorizontalPadding = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.HorizontalPadding);
		}
		#endregion
		#region VerticalPadding
		public int VerticalPadding {
			get { return Info.VerticalPadding; }
			set {
				if (Info.VerticalPadding == value && UseVerticalPadding)
					return;
				SetPropertyValue(SetVerticalPaddingCore, value);
			}
		}
		public bool UseVerticalPadding { get { return Info.Options.UseVerticalPadding; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPaddingCore(ParagraphFrameFormattingBase info, int value) {
			info.VerticalPadding = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.VerticalPadding);
		}
		#endregion
		#region HorizontalRule
		public ParagraphFrameHorizontalRule HorizontalRule {
			get { return Info.HorizontalRule; }
			set {
				if (Info.HorizontalRule == value && UseHorizontalRule)
					return;
				SetPropertyValue(SetHorizontalRuleCore, value);
			}
		}
		public bool UseHorizontalRule { get { return Info.Options.UseHorizontalRule; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalRuleCore(ParagraphFrameFormattingBase info, ParagraphFrameHorizontalRule value) {
			info.HorizontalRule = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.HorizontalRule);
		}
		#endregion
		#region TextWrapType
		public ParagraphFrameTextWrapType TextWrapType {
			get { return Info.TextWrapType; }
			set {
				if (Info.TextWrapType == value && UseTextWrapType)
					return;
				SetPropertyValue(SetTextWrapTypeCore, value);
			}
		}
		public bool UseTextWrapType { get { return Info.Options.UseTextWrapType; } }
		protected internal virtual DocumentModelChangeActions SetTextWrapTypeCore(ParagraphFrameFormattingBase info, ParagraphFrameTextWrapType value) {
			info.TextWrapType = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.TextWrapType);
		}
		#endregion
		#region IParagraphFrameLocation Members
		#region X
		public int X {
			get { return Info.X; }
			set {
				if (Info.X == value && UseX)
					return;
				SetPropertyValue(SetXCore, value);
			}
		}
		public bool UseX { get { return Info.Options.UseX; } }
		protected internal virtual DocumentModelChangeActions SetXCore(ParagraphFrameFormattingBase info, int value) {
			info.X = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.X);
		}
		#endregion
		#region Y
		public int Y {
			get { return Info.Y; }
			set {
				if (Info.Y == value && UseY)
					return;
				SetPropertyValue(SetYCore, value);
			}
		}
		public bool UseY { get { return Info.Options.UseY; } }
		protected internal virtual DocumentModelChangeActions SetYCore(ParagraphFrameFormattingBase info, int value) {
			info.Y = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.Y);
		}
		#endregion
		#region HorizontalPositionAlignment
		public ParagraphFrameHorizontalPositionAlignment HorizontalPositionAlignment {
			get { return Info.HorizontalPositionAlignment; }
			set {
				if (Info.HorizontalPositionAlignment == value && UseHorizontalPositionAlignment)
					return;
				SetPropertyValue(SetHorizontalPositionAlignmentCore, value);
			}
		}
		public bool UseHorizontalPositionAlignment { get { return Info.Options.UseHorizontalPositionAlignment; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPositionAlignmentCore(ParagraphFrameFormattingBase info, ParagraphFrameHorizontalPositionAlignment value) {
			info.HorizontalPositionAlignment = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.HorizontalPositionAlignment);
		}
		#endregion
		#region VerticalPositionAlignment
		public ParagraphFrameVerticalPositionAlignment VerticalPositionAlignment {
			get { return Info.VerticalPositionAlignment; }
			set {
				if (Info.VerticalPositionAlignment == value && UseVerticalPositionAlignment)
					return;
				SetPropertyValue(SetVerticalPositionAlignmentCore, value);
			}
		}
		public bool UseVerticalPositionAlignment { get { return Info.Options.UseVerticalPositionAlignment; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPositionAlignmentCore(ParagraphFrameFormattingBase info, ParagraphFrameVerticalPositionAlignment value) {
			info.VerticalPositionAlignment = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.VerticalPositionAlignment);
		}
		#endregion
		#region HorizontalPositionType
		public ParagraphFrameHorizontalPositionType HorizontalPositionType {
			get { return Info.HorizontalPositionType; }
			set {
				if (Info.HorizontalPositionType == value && UseHorizontalPositionType)
					return;
				SetPropertyValue(SetHorizontalPositionTypeCore, value);
			}
		}
		public bool UseHorizontalPositionType { get { return Info.Options.UseHorizontalPositionType; } }
		protected internal virtual DocumentModelChangeActions SetHorizontalPositionTypeCore(ParagraphFrameFormattingBase info, ParagraphFrameHorizontalPositionType value) {
			info.HorizontalPositionType = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.HorizontalPositionType);
		}
		#endregion
		#region VerticalPositionType
		public ParagraphFrameVerticalPositionType VerticalPositionType {
			get { return Info.VerticalPositionType; }
			set {
				if (Info.VerticalPositionType == value && UseVerticalPositionType)
					return;
				SetPropertyValue(SetVerticalPositionTypeCore, value);
			}
		}
		public bool UseVerticalPositionType { get { return Info.Options.UseVerticalPositionType; } }
		protected internal virtual DocumentModelChangeActions SetVerticalPositionTypeCore(ParagraphFrameFormattingBase info, ParagraphFrameVerticalPositionType value) {
			info.VerticalPositionType = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.VerticalPositionType);
		}
		#endregion
		#region Width
		public int Width {
			get { return Info.Width; }
			set {
				if (Info.Width == value && UseWidth)
					return;
				SetPropertyValue(SetWidthCore, value);
			}
		}
		public bool UseWidth { get { return Info.Options.UseWidth; } }
		protected internal virtual DocumentModelChangeActions SetWidthCore(ParagraphFrameFormattingBase info, int value) {
			info.Width = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.Width);
		}
		#endregion
		#region Height
		public int Height {
			get { return Info.Height; }
			set {
				if (Info.Height == value && UseHeight)
					return;
				SetPropertyValue(SetHeightCore, value);
			}
		}
		public bool UseHeight { get { return Info.Options.UseHeight; } }
		protected internal virtual DocumentModelChangeActions SetHeightCore(ParagraphFrameFormattingBase info, int value) {
			info.Height = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.Height);
		}
		#endregion
		#endregion
		#region LockFrameAnchorToParagraph
		public bool LockFrameAnchorToParagraph {
			get { return Info.LockFrameAnchorToParagraph; }
			set {
				if (Info.LockFrameAnchorToParagraph == value && UseLockFrameAnchorToParagraph)
					return;
				SetPropertyValue(SetLockFrameAnchorToParagraphCore, value);
			}
		}
		public bool UseLockFrameAnchorToParagraph { get { return Info.Options.UseLockFrameAnchorToParagraph; } }
		protected internal virtual DocumentModelChangeActions SetLockFrameAnchorToParagraphCore(ParagraphFrameFormattingBase info, bool value) {
			info.LockFrameAnchorToParagraph = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.LockFrameAnchorToParagraph);
		}
		#endregion
		#region DropCap
		public DropCapLocation DropCap {
			get { return Info.DropCap; }
			set {
				if (Info.DropCap == value && UseDropCap)
					return;
				SetPropertyValue(SetDropCapCore, value);
			}
		}
		public bool UseDropCap { get { return Info.Options.UseDropCap; } }
		protected internal virtual DocumentModelChangeActions SetDropCapCore(ParagraphFrameFormattingBase info, DropCapLocation value) {
			info.DropCap = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.DropCap);
		}
		#endregion
		#region DropCapVerticalHeightInLines
		public int DropCapVerticalHeightInLines {
			get { return Info.DropCapVerticalHeightInLines; }
			set {
				if (Info.DropCapVerticalHeightInLines == value && UseDropCapVerticalHeightInLines)
					return;
				SetPropertyValue(SetDropCapVerticalHeightInLinesCore, value);
			}
		}
		public bool UseDropCapVerticalHeightInLines { get { return Info.Options.UseDropCapVerticalHeightInLines; } }
		protected internal virtual DocumentModelChangeActions SetDropCapVerticalHeightInLinesCore(ParagraphFrameFormattingBase info, int value) {
			info.DropCapVerticalHeightInLines = value;
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.DropCapVerticalHeightInLines);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<ParagraphFrameFormattingBase> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.ParagraphFrameFormattingCache;
		}
		protected internal bool UseVal(ParagraphFrameFormattingOptions.Mask mask) {
			return (Info.Options.Value & mask) != 0;
		}
		public void Reset() {
			ParagraphFrameFormattingBase info = GetInfoForModification();
			ParagraphFrameFormattingBase emptyInfo = GetCache(DocumentModel)[ParagraphFrameFormattingCache.EmptyParagraphFrameFormattingIndex];
			info.ReplaceInfo(emptyInfo.Info, emptyInfo.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override bool Equals(object obj) {
			FrameProperties other = obj as FrameProperties;
			if (ReferenceEquals(obj, null))
				return false;
			if (DocumentModel == other.DocumentModel)
				return Index == other.Index;
			else
				return Info.Equals(other.Info);
		}
		internal void ResetUse(ParagraphFrameFormattingOptions.Mask mask) {
			ParagraphFrameFormattingBase info = GetInfoForModification();
			ParagraphFrameFormattingOptions options = info.GetOptionsForModification();
			options.Value &= ~mask;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		internal void ResetAllUse() {
			ParagraphFrameFormattingBase info = GetInfoForModification();
			ParagraphFrameFormattingOptions options = info.GetOptionsForModification();
			options.Value = ParagraphFrameFormattingOptions.Mask.UseNone;
			info.ReplaceInfo(info.GetInfoForModification(), options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		public override int GetHashCode() {
			return Index;
		}
#if DEBUGTEST
		public override string ToString() {
			return String.Format("FormattingInfoIndex:{0}, InfoIndex:{1}, OptionsIndex:{2}", Index, Info.InfoIndex, Info.OptionsIndex);
		}
#endif
		public bool CanMerge(FrameProperties properties) {
			bool canMegre = this.TextWrapType == properties.TextWrapType && this.HorizontalPositionType == properties.HorizontalPositionType && this.VerticalPositionType == properties.VerticalPositionType && this.VerticalPosition == properties.VerticalPosition && this.HorizontalPosition == properties.HorizontalPosition && this.HorizontalPadding == properties.HorizontalPadding && this.VerticalPadding == properties.VerticalPadding && this.Width == properties.Width && this.X == properties.X && this.Y == properties.Y;
			if (this.HorizontalRule == properties.HorizontalRule) {
				if (this.HorizontalRule != ParagraphFrameHorizontalRule.Auto)
					canMegre = canMegre && this.Height == properties.Height;
			}
			else
				return false;
			if (!this.UseX && !properties.UseX)
				canMegre = canMegre && this.HorizontalPositionAlignment == properties.HorizontalPositionAlignment;
			if (!this.UseY && !properties.UseY)
				canMegre = canMegre && this.VerticalPositionAlignment == properties.VerticalPositionAlignment;
			return canMegre;
		}
		internal virtual void Merge(FrameProperties properties) {
			FramePropertiesMerger merger = new FramePropertiesMerger(this);
			merger.Merge(properties);
			CopyFrom(merger.MergedProperties);
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return ParagraphFrameFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFrameFormattingChangeType.BatchUpdate);
		}
		protected override void OnIndexChanged() {
			base.OnIndexChanged();
			owner.OnParagraphPropertiesChanged();
		}
		protected override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateIndexChangedHistoryItem() {
			return new IndexChangedHistoryItem<DocumentModelChangeActions>(DocumentModel.MainPieceTable, this);
		}
		public void CopyFrom(MergedProperties<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions> frameProperties) {
			ParagraphFrameFormattingBase info = GetInfoForModification();
			info.CopyFromCore(frameProperties.Info, frameProperties.Options);
			ReplaceInfo(info, GetBatchUpdateChangeActions());
		}
		protected internal override IObtainAffectedRangeListener GetObtainAffectedRangeListener() {
			return owner as IObtainAffectedRangeListener;
		}
	}
	#region ParagraphFrameFormattingChangeType
	public enum ParagraphFrameFormattingChangeType {
		None = 0,
		HorizontalPosition,
		VerticalPosition,
		HorizontalPadding,
		VerticalPadding,
		HorizontalRule,
		TextWrapType,
		X,
		Y,
		Width,
		Height,
		HorizontalPositionAlignment,
		VerticalPositionAlignment,
		HorizontalPositionType,
		VerticalPositionType,
		DropCap,
		DropCapVerticalHeightInLines,
		LockFrameAnchorToParagraph,
		BatchUpdate
	}
	#endregion
	#region ParagraphFrameFormattingChangeActionsCalculator
	public static class ParagraphFrameFormattingChangeActionsCalculator {
		internal class ParagraphFrameFormattingChangeActionsTable : Dictionary<ParagraphFrameFormattingChangeType, DocumentModelChangeActions> {
		}
		internal static readonly ParagraphFrameFormattingChangeActionsTable paragraphFrameFormattingChangeActionsTable = CreateParagraphFrameFormattingChangeActionsTable();
		internal static ParagraphFrameFormattingChangeActionsTable CreateParagraphFrameFormattingChangeActionsTable() {
			ParagraphFrameFormattingChangeActionsTable table = new ParagraphFrameFormattingChangeActionsTable();
			table.Add(ParagraphFrameFormattingChangeType.None, DocumentModelChangeActions.None);
			table.Add(ParagraphFrameFormattingChangeType.HorizontalPosition, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.VerticalPosition, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.HorizontalPadding, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.VerticalPadding, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.HorizontalRule, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.TextWrapType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.X, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.Y, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.Width, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.Height, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.HorizontalPositionAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.VerticalPositionAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.HorizontalPositionType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.VerticalPositionType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.DropCap, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.DropCapVerticalHeightInLines, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.LockFrameAnchorToParagraph, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(ParagraphFrameFormattingChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(ParagraphFrameFormattingChangeType change) {
			return paragraphFrameFormattingChangeActionsTable[change];
		}
	}
	#endregion
	#region MergedFrameProperties
	public class MergedFrameProperties : MergedProperties<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions>, IParagraphFrameLocation {
		public MergedFrameProperties(ParagraphFrameFormattingInfo info, ParagraphFrameFormattingOptions options)
			: base(info, options) {
		}
		#region IParagraphFrameLocation Members
		public int X { get { return Info.X; } }
		public int Y { get { return Info.Y; } }
		public ParagraphFrameHorizontalPositionAlignment HorizontalPositionAlignment { get { return Info.HorizontalPositionAlignment; } }
		public ParagraphFrameVerticalPositionAlignment VerticalPositionAlignment { get { return Info.VerticalPositionAlignment; } }
		public ParagraphFrameHorizontalPositionType HorizontalPositionType { get { return Info.HorizontalPositionType; } }
		public ParagraphFrameVerticalPositionType VerticalPositionType { get { return Info.VerticalPositionType; } }
		public int Width { get { return Info.Width; } }
		public int Height { get { return Info.Height; } }
		#endregion
		public bool CanMerge(MergedFrameProperties properties) {
			bool canMegre = this.Info.TextWrapType == properties.Info.TextWrapType && this.HorizontalPositionType == properties.HorizontalPositionType && this.VerticalPositionType == properties.VerticalPositionType && this.Info.VerticalPosition == properties.Info.VerticalPosition && this.Info.HorizontalPosition == properties.Info.HorizontalPosition && this.Info.HorizontalPadding == properties.Info.HorizontalPadding && this.Info.VerticalPadding == properties.Info.VerticalPadding && this.Width == properties.Width && this.X == properties.X && this.Y == properties.Y;
			if (this.Info.HorizontalRule == properties.Info.HorizontalRule) {
				if (this.Info.HorizontalRule != ParagraphFrameHorizontalRule.Auto)
					canMegre = canMegre && this.Height == properties.Height;
			}
			else
				return false;
			if (!this.Options.UseX && !properties.Options.UseX)
				canMegre = canMegre && this.HorizontalPositionAlignment == properties.HorizontalPositionAlignment;
			if (!this.Options.UseY && !properties.Options.UseY)
				canMegre = canMegre && this.VerticalPositionAlignment == properties.VerticalPositionAlignment;
			return canMegre;
		}
		public override bool Equals(object obj) {
			MergedFrameProperties other = obj as MergedFrameProperties;
			if (ReferenceEquals(obj, null))
				return false;
			return Info.Equals(other.Info);
		}
		public override int GetHashCode() {
			return Info.GetHashCode();
		}
	}
	#endregion
	#region FramePropertiesMerger
	public class FramePropertiesMerger {
		MergedFrameProperties mergedProperties;
		public FramePropertiesMerger(FrameProperties initialProperties) {
			if (initialProperties != null)
				mergedProperties = new MergedFrameProperties(initialProperties.Info.Info, initialProperties.Info.Options);
		}
		protected FramePropertiesMerger(MergedFrameProperties initial) {
			mergedProperties = initial;
		}
		public MergedFrameProperties MergedProperties { get { return mergedProperties; } }
		protected ParagraphFrameFormattingInfo OwnInfo { get { return MergedProperties.Info; } }
		protected ParagraphFrameFormattingOptions OwnOptions { get { return MergedProperties.Options; } }
		public void Merge(FrameProperties properties) {
			if (properties != null) {
				if (MergedProperties == null)
					mergedProperties = new MergedFrameProperties(properties.Info.Info, properties.Info.Options);
				MergeCore(properties.Info.Info, properties.Info.Options);
			}
		}
		public void Merge(MergedProperties<ParagraphFrameFormattingInfo, ParagraphFrameFormattingOptions> properties) {
			if (properties != null) {
				if (MergedProperties == null)
					mergedProperties = new MergedFrameProperties(properties.Info, properties.Options);
				MergeCore(properties.Info, properties.Options);
			}
		}
		protected internal void MergeCore(ParagraphFrameFormattingInfo info, ParagraphFrameFormattingOptions options) {
			if (!OwnOptions.UseHorizontalPosition && options.UseHorizontalPosition) {
				OwnInfo.HorizontalPosition = info.HorizontalPosition;
				OwnOptions.UseHorizontalPosition = true;
			}
			if (!OwnOptions.UseVerticalPosition && options.UseVerticalPosition) {
				OwnInfo.VerticalPosition = info.VerticalPosition;
				OwnOptions.UseVerticalPosition = true;
			}
			if (!OwnOptions.UseHorizontalPadding && options.UseHorizontalPadding) {
				OwnInfo.HorizontalPadding = info.HorizontalPadding;
				OwnOptions.UseHorizontalPadding = true;
			}
			if (!OwnOptions.UseVerticalPadding && options.UseVerticalPadding) {
				OwnInfo.VerticalPadding = info.VerticalPadding;
				OwnOptions.UseVerticalPadding = true;
			}
			if (!OwnOptions.UseHorizontalRule && options.UseHorizontalRule) {
				OwnInfo.HorizontalRule = info.HorizontalRule;
				OwnOptions.UseHorizontalRule = true;
			}
			if (!OwnOptions.UseTextWrapType && options.UseTextWrapType) {
				OwnInfo.TextWrapType = info.TextWrapType;
				OwnOptions.UseTextWrapType = true;
			}
			if (!OwnOptions.UseX && options.UseX) {
				OwnInfo.X = info.X;
				OwnOptions.UseX = true;
			}
			if (!OwnOptions.UseY && options.UseY) {
				OwnInfo.Y = info.Y;
				OwnOptions.UseY = true;
			}
			if (!OwnOptions.UseWidth && options.UseWidth) {
				OwnInfo.Width = info.Width;
				OwnOptions.UseWidth = true;
			}
			if (!OwnOptions.UseHeight && options.UseHeight) {
				OwnInfo.Height = info.Height;
				OwnOptions.UseHeight = true;
			}
			if (!OwnOptions.UseHorizontalPositionAlignment && options.UseHorizontalPositionAlignment) {
				OwnInfo.HorizontalPositionAlignment = info.HorizontalPositionAlignment;
				OwnOptions.UseHorizontalPositionAlignment = true;
			}
			if (!OwnOptions.UseVerticalPositionAlignment && options.UseVerticalPositionAlignment) {
				OwnInfo.VerticalPositionAlignment = info.VerticalPositionAlignment;
				OwnOptions.UseVerticalPositionAlignment = true;
			}
			if (!OwnOptions.UseHorizontalPositionType && options.UseHorizontalPositionType) {
				OwnInfo.HorizontalPositionType = info.HorizontalPositionType;
				OwnOptions.UseHorizontalPositionType = true;
			}
			if (!OwnOptions.UseVerticalPositionType && options.UseVerticalPositionType) {
				OwnInfo.VerticalPositionType = info.VerticalPositionType;
				OwnOptions.UseVerticalPositionType = true;
			}
			if (!OwnOptions.UseDropCap && options.UseDropCap) {
				OwnInfo.DropCap = info.DropCap;
				OwnOptions.UseDropCap = true;
			}
			if (!OwnOptions.UseDropCapVerticalHeightInLines && options.UseDropCapVerticalHeightInLines) {
				OwnInfo.DropCapVerticalHeightInLines = info.DropCapVerticalHeightInLines;
				OwnOptions.UseDropCapVerticalHeightInLines = true;
			}
			if (!OwnOptions.UseLockFrameAnchorToParagraph && options.UseLockFrameAnchorToParagraph) {
				OwnInfo.LockFrameAnchorToParagraph = info.LockFrameAnchorToParagraph;
				OwnOptions.UseLockFrameAnchorToParagraph = true;
			}
		}
	}
	#endregion
}
