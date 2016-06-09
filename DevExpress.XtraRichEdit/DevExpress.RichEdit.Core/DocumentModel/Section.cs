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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using System.Drawing;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.NumberConverters;
using DevExpress.Compatibility.System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing.Printing;
using System.Diagnostics;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region VerticalAlignment
	public enum VerticalAlignment {
		Top = OpenDocumentVerticalAlignment.Top,
		Both = OpenDocumentVerticalAlignment.Both, 
		Center = OpenDocumentVerticalAlignment.Center,
		Bottom = OpenDocumentVerticalAlignment.Bottom,
	}
	#endregion
	#region SectionGutterAlignment
	public enum SectionGutterAlignment {
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}
	#endregion
	#region MarginsInfo
	public class MarginsInfo : ICloneable<MarginsInfo>, ISupportsCopyFrom<MarginsInfo>, ISupportsSizeOf {
		#region Fields
		int left;
		int right;
		int top;
		int bottom;
		int gutter;
		SectionGutterAlignment gutterAlignment;
		int headerOffset;
		int footerOffset;
		#endregion
		#region Properties
		public int Left { get { return left; } set { left = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Top { get { return top; } set { top = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
		public int Gutter { get { return gutter; } set { gutter = value; } }
		public SectionGutterAlignment GutterAlignment { get { return gutterAlignment; } set { gutterAlignment = value; } }
		public int HeaderOffset { get { return headerOffset; } set { headerOffset = value; } }
		public int FooterOffset { get { return footerOffset; } set { footerOffset = value; } }
		#endregion
		#region ICloneable<MarginsInfo> Members
		public MarginsInfo Clone() {
			MarginsInfo result = new MarginsInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public override bool Equals(object obj) {
			MarginsInfo info = (MarginsInfo)obj;
			return
				this.Top == info.Top &&
				this.Bottom == info.Bottom &&
				this.Left == info.Left &&
				this.Right == info.Right &&
				this.Gutter == info.Gutter &&
				this.HeaderOffset == info.HeaderOffset &&
				this.FooterOffset == info.FooterOffset &&
				this.GutterAlignment == info.GutterAlignment;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void CopyFrom(MarginsInfo info) {
			this.left = info.Left;
			this.right = info.Right;
			this.top = info.Top;
			this.bottom = info.Bottom;
			this.gutter = info.Gutter;
			this.gutterAlignment = info.GutterAlignment;
			this.headerOffset = info.HeaderOffset;
			this.footerOffset = info.FooterOffset;
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region MarginsInfoCache
	public class MarginsInfoCache : UniqueItemsCache<MarginsInfo> {
		public MarginsInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override MarginsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			MarginsInfo result = new MarginsInfo();
			result.Left = unitConverter.HundredthsOfMillimeterToModelUnits(3000);
			result.Top = unitConverter.HundredthsOfMillimeterToModelUnits(2000);
			result.Bottom = unitConverter.HundredthsOfMillimeterToModelUnits(2000);
			result.Right = unitConverter.HundredthsOfMillimeterToModelUnits(1500);
			result.HeaderOffset = unitConverter.HundredthsOfMillimeterToModelUnits(1250);
			result.FooterOffset = unitConverter.HundredthsOfMillimeterToModelUnits(1250);
			return result;
		}
	}
	#endregion
	#region SectionMargins
	public class SectionMargins : RichEditIndexBasedObject<MarginsInfo> {
		public SectionMargins(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region Left
		public int Left {
			get { return Info.Left; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Left", value);
				if (Left == value)
					return;
				SetPropertyValue(SetLeftCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetLeftCore(MarginsInfo margins, int value) {
			margins.Left = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Left);
		}
		#endregion
		#region Right
		public int Right {
			get { return Info.Right; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Right", value);
				if (Right == value)
					return;
				SetPropertyValue(SetRightCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetRightCore(MarginsInfo margins, int value) {
			margins.Right = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Right);
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
		protected internal virtual DocumentModelChangeActions SetTopCore(MarginsInfo margins, int value) {
			margins.Top = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Top);
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
		protected internal virtual DocumentModelChangeActions SetBottomCore(MarginsInfo margins, int value) {
			margins.Bottom = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Bottom);
		}
		#endregion
		#region Gutter
		public int Gutter {
			get { return Info.Gutter; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Gutter", value);
				if (Gutter == value)
					return;
				SetPropertyValue(SetGutterCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetGutterCore(MarginsInfo margins, int value) {
			margins.Gutter = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.Gutter);
		}
		#endregion
		#region GutterAlignment
		public SectionGutterAlignment GutterAlignment {
			get { return Info.GutterAlignment; }
			set {
				if (GutterAlignment == value)
					return;
				SetPropertyValue(SetGutterAlignmentCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetGutterAlignmentCore(MarginsInfo margins, SectionGutterAlignment value) {
			margins.GutterAlignment = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.GutterAlignment);
		}
		#endregion
		#region HeaderOffset
		public int HeaderOffset {
			get { return Info.HeaderOffset; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("HeaderOffset", value);
				if (HeaderOffset == value)
					return;
				SetPropertyValue(SetHeaderOffsetCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetHeaderOffsetCore(MarginsInfo margins, int value) {
			margins.HeaderOffset = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.HeaderOffset);
		}
		#endregion
		#region FooterOffset
		public int FooterOffset {
			get { return Info.FooterOffset; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("FooterOffset", value);
				if (FooterOffset == value)
					return;
				SetPropertyValue(SetFooterOffsetCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetFooterOffsetCore(MarginsInfo margins, int value) {
			margins.FooterOffset = value;
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.FooterOffset);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<MarginsInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.MarginsInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionMarginsChangeActionsCalculator.CalculateChangeActions(SectionMarginsChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionMarginsChangeType
	public enum SectionMarginsChangeType {
		None = 0,
		Left,
		Right,
		Top,
		Bottom,
		Gutter,
		GutterAlignment,
		HeaderOffset,
		FooterOffset,
		BatchUpdate
	}
	#endregion
	#region SectionMarginsChangeActionsCalculator
	public static class SectionMarginsChangeActionsCalculator {
		internal class SectionMarginsChangeActionsTable : Dictionary<SectionMarginsChangeType, DocumentModelChangeActions> {
		}
		internal static SectionMarginsChangeActionsTable sectionMarginsChangeActionsTable = CreateSectionMarginsChangeActionsTable();
		internal static SectionMarginsChangeActionsTable CreateSectionMarginsChangeActionsTable() {
			SectionMarginsChangeActionsTable table = new SectionMarginsChangeActionsTable();
			table.Add(SectionMarginsChangeType.None, DocumentModelChangeActions.None);
			table.Add(SectionMarginsChangeType.Left, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionMarginsChangeType.Right, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionMarginsChangeType.Top, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionMarginsChangeType.Bottom, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionMarginsChangeType.Gutter, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionMarginsChangeType.GutterAlignment, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionMarginsChangeType.HeaderOffset, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionMarginsChangeType.FooterOffset, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionMarginsChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionMarginsChangeType change) {
			return sectionMarginsChangeActionsTable[change];
		}
	}
	#endregion
	#region ColumnInfo
	public class ColumnInfo : ICloneable<ColumnInfo> {
		#region Fields
		int width;
		int space;
		#endregion
		#region Properties
		public int Width { get { return width; } set { width = value; } }
		public int Space { get { return space; } set { space = value; } }
		#endregion
		#region ICloneable<ColumnInfo> Members
		public ColumnInfo Clone() {
			ColumnInfo result = new ColumnInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public void CopyFrom(ColumnInfo info) {
			this.Width = info.Width;
			this.Space = info.Space;
		}
		public override bool Equals(object obj) {
			ColumnInfo info = (ColumnInfo)obj;
			return
				this.Width == info.Width &&
				this.Space == info.Space;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	#endregion
	#region ColumnInfoCollection
	public class ColumnInfoCollection : List<ColumnInfo> {
	}
	#endregion
	#region ColumnsInfo
	public class ColumnsInfo : ICloneable<ColumnsInfo>, ISupportsCopyFrom<ColumnsInfo>, ISupportsSizeOf {
		#region Fields
		ColumnInfoCollection columns = new ColumnInfoCollection();
		bool drawVerticalSeparator;
		bool equalWidthColumns;
		int space; 
		int columnCount; 
		#endregion
		#region Properties
		public bool DrawVerticalSeparator { get { return drawVerticalSeparator; } set { drawVerticalSeparator = value; } }
		public bool EqualWidthColumns { get { return equalWidthColumns; } set { equalWidthColumns = value; } }
		public int Space { get { return space; } set { space = value; } }
		public int ColumnCount { get { return columnCount; } set { columnCount = value; } }
		public List<ColumnInfo> Columns { get { return columns; } }
		#endregion
		#region ICloneable<ColumnsInfo> Members
		public ColumnsInfo Clone() {
			ColumnsInfo result = new ColumnsInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public void CopyFrom(ColumnsInfo info) {
			this.DrawVerticalSeparator = info.DrawVerticalSeparator;
			this.EqualWidthColumns = info.EqualWidthColumns;
			this.Space = info.Space;
			this.ColumnCount = info.ColumnCount;
			this.Columns.Clear();
			int count = info.Columns.Count;
			for (int i = 0; i < count; i++)
				this.Columns.Add(info.Columns[i].Clone());
		}
		public override bool Equals(object obj) {
			ColumnsInfo info = (ColumnsInfo)obj;
			if (info.EqualWidthColumns != EqualWidthColumns || info.DrawVerticalSeparator != DrawVerticalSeparator || info.Space != Space || info.ColumnCount != ColumnCount)
				return false;
			int count = columns.Count;
			if (count != info.Columns.Count)
				return false;
			for (int i = 0; i < count; i++)
				if (!Columns[i].Equals(info.Columns[i]))
					return false;
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region ColumnsInfoCache
	public class ColumnsInfoCache : UniqueItemsCache<ColumnsInfo> {
		public ColumnsInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override ColumnsInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			ColumnsInfo result = new ColumnsInfo();
			result.ColumnCount = 1;
			result.EqualWidthColumns = true;
			result.Space = unitConverter.HundredthsOfMillimeterToModelUnits(1250);
			return result;
		}
	}
	#endregion
	#region SectionColumns
	public class SectionColumns : RichEditIndexBasedObject<ColumnsInfo> {
		public SectionColumns(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region EqualWidthColumns
		public bool EqualWidthColumns {
			get { return Info.EqualWidthColumns; }
			set {
				if (EqualWidthColumns == value)
					return;
				SetPropertyValue(SetEqualWidthColumnsCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetEqualWidthColumnsCore(ColumnsInfo info, bool value) {
			info.EqualWidthColumns = value;
			return SectionColumnsChangeActionsCalculator.CalculateChangeActions(SectionColumnsChangeType.EqualWidthColumns);
		}
		#endregion
		#region DrawVerticalSeparator
		public bool DrawVerticalSeparator {
			get { return Info.DrawVerticalSeparator; }
			set {
				if (DrawVerticalSeparator == value)
					return;
				SetPropertyValue(SetDrawVerticalSeparatorCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDrawVerticalSeparatorCore(ColumnsInfo info, bool value) {
			info.DrawVerticalSeparator = value;
			return SectionColumnsChangeActionsCalculator.CalculateChangeActions(SectionColumnsChangeType.DrawVerticalSeparator);
		}
		#endregion
		#region Space
		public int Space {
			get { return Info.Space; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Space", value);
				if (Space == value)
					return;
				SetPropertyValue(SetSpaceCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetSpaceCore(ColumnsInfo info, int value) {
			info.Space = value;
			return SectionColumnsChangeActionsCalculator.CalculateChangeActions(SectionColumnsChangeType.Space);
		}
		#endregion
		#region ColumnCount
		public int ColumnCount {
			get { return Info.ColumnCount; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("ColumnCount", value);
				if (ColumnCount == value)
					return;
				SetPropertyValue(SetColumnCountCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetColumnCountCore(ColumnsInfo info, int value) {
			info.ColumnCount = value;
			return SectionColumnsChangeActionsCalculator.CalculateChangeActions(SectionColumnsChangeType.ColumnCount);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<ColumnsInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.ColumnsInfoCache;
		}
		public ColumnInfoCollection GetColumns() {
			ColumnInfoCollection result = new ColumnInfoCollection();
			result.AddRange(Info.Columns);
			return result;
		}
		public void SetColumns(ColumnInfoCollection value) {
			Guard.ArgumentNotNull(value, "value");
			ColumnsInfo columns = GetInfoForModification();
			columns.Columns.Clear();
			columns.Columns.AddRange(value);
			ReplaceInfo(columns, GetBatchUpdateChangeActions());
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionColumnsChangeActionsCalculator.CalculateChangeActions(SectionColumnsChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionColumnsChangeType
	public enum SectionColumnsChangeType {
		None = 0,
		EqualWidthColumns,
		DrawVerticalSeparator,
		Space,
		ColumnCount,
		BatchUpdate
	}
	#endregion
	#region SectionColumnsChangeActionsCalculator
	public static class SectionColumnsChangeActionsCalculator {
		internal class SectionColumnsChangeActionsTable : Dictionary<SectionColumnsChangeType, DocumentModelChangeActions> {
		}
		internal static SectionColumnsChangeActionsTable sectionColumnsChangeActionsTable = CreateSectionColumnsChangeActionsTable();
		internal static SectionColumnsChangeActionsTable CreateSectionColumnsChangeActionsTable() {
			SectionColumnsChangeActionsTable table = new SectionColumnsChangeActionsTable();
			table.Add(SectionColumnsChangeType.None, DocumentModelChangeActions.None);
			table.Add(SectionColumnsChangeType.EqualWidthColumns, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionColumnsChangeType.DrawVerticalSeparator, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler);
			table.Add(SectionColumnsChangeType.Space, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionColumnsChangeType.ColumnCount, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionColumnsChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResetHorizontalRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionColumnsChangeType change) {
			return sectionColumnsChangeActionsTable[change];
		}
	}
	#endregion
	#region PageInfo
	public class PageInfo : ICloneable<PageInfo>, ISupportsCopyFrom<PageInfo>, ISupportsSizeOf {
		#region Fields
		int width;
		int height;
		PaperKind paperKind;
		bool landscape;
		#endregion
		#region Properties
		public int Width { get { return width; } set { width = value; } }
		public int Height { get { return height; } set { height = value; } }
		public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }
		public bool Landscape { get { return landscape; } set { landscape = value; } }
		#endregion
		#region ICloneable<PageInfo> Members
		public PageInfo Clone() {
			PageInfo result = new PageInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		public void CopyFrom(PageInfo info) {
			this.Width = info.Width;
			this.Height = info.Height;
			this.Landscape = info.Landscape;
			this.PaperKind = info.PaperKind;
		}
		public override bool Equals(object obj) {
			PageInfo info = (PageInfo)obj;
			return
				this.Width == info.Width &&
				this.Height == info.Height &&
				this.Landscape == info.Landscape &&
				this.PaperKind == info.PaperKind;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
		public void ValidatePaperKind(DocumentModelUnitConverter unitConverter) {
			Size paperKindSize = PaperSizeCalculator.CalculatePaperSize(PaperKind);
			if (Width <= 0) {
				if (Landscape)
					Width = unitConverter.TwipsToModelUnits(paperKindSize.Height);
				else
					Width = unitConverter.TwipsToModelUnits(paperKindSize.Width);
			}
			if (Height <= 0) {
				if (Landscape)
					Height = unitConverter.TwipsToModelUnits(paperKindSize.Width);
				else
					Height = unitConverter.TwipsToModelUnits(paperKindSize.Height);
			}
			Size size;
			if (Landscape)
				size = new Size(Height, Width);
			else
				size = new Size(Width, Height);
			size = unitConverter.ModelUnitsToTwips(size);
			PaperKind paperKind = PaperSizeCalculator.CalculatePaperKind(size, PaperKind.Custom, 0, PaperKind.Letter);
			if (paperKind != this.PaperKind)
				this.PaperKind = paperKind;
		}
	}
	#endregion
	#region PageInfoCache
	public class PageInfoCache : UniqueItemsCache<PageInfo> {
		public PageInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PageInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			PageInfo result = new PageInfo();
			result.PaperKind = PaperKind.Letter;
			result.Landscape = false;
			result.Width = unitConverter.TwipsToModelUnits(12240);
			result.Height = unitConverter.TwipsToModelUnits(15840);
			return result;
		}
	}
	#endregion
	#region SectionPage
	public class SectionPage : RichEditIndexBasedObject<PageInfo> {
		public SectionPage(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region Width
		public int Width {
			get { return Info.Width; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("Width", value);
				if (Width == value)
					return;
				SetPropertyValue(SetWidthCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetWidthCore(PageInfo info, int value) {
			info.Width = value;
			return SectionPageChangeActionsCalculator.CalculateChangeActions(SectionPageChangeType.Width);
		}
		#endregion
		#region Height
		public int Height {
			get { return Info.Height; }
			set {
				if (value <= 0)
					Exceptions.ThrowArgumentException("Height", value);
				if (Height == value)
					return;
				SetPropertyValue(SetHeightCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetHeightCore(PageInfo info, int value) {
			info.Height = value;
			return SectionPageChangeActionsCalculator.CalculateChangeActions(SectionPageChangeType.Height);
		}
		#endregion
		#region Landscape
		public bool Landscape {
			get { return Info.Landscape; }
			set {
				if (Landscape == value)
					return;
				SetPropertyValue(SetLandscapeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetLandscapeCore(PageInfo info, bool value) {
			info.Landscape = value;
			return SectionPageChangeActionsCalculator.CalculateChangeActions(SectionPageChangeType.Landscape);
		}
		#endregion
		#region PaperKind
		public PaperKind PaperKind {
			get { return Info.PaperKind; }
			set {
				if (PaperKind == value)
					return;
				SetPropertyValue(SetPaperKindCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetPaperKindCore(PageInfo info, PaperKind value) {
			info.PaperKind = value;
			return SectionPageChangeActionsCalculator.CalculateChangeActions(SectionPageChangeType.PaperKind);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<PageInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.PageInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionPageChangeActionsCalculator.CalculateChangeActions(SectionPageChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionPageChangeType
	public enum SectionPageChangeType {
		None = 0,
		Width,
		Height,
		Landscape,
		PaperKind,
		BatchUpdate
	}
	#endregion
	#region SectionPageChangeActionsCalculator
	public static class SectionPageChangeActionsCalculator {
		internal class SectionPageChangeActionsTable : Dictionary<SectionPageChangeType, DocumentModelChangeActions> {
		}
		internal static SectionPageChangeActionsTable sectionPageChangeActionsTable = CreateSectionPageChangeActionsTable();
		internal static SectionPageChangeActionsTable CreateSectionPageChangeActionsTable() {
			SectionPageChangeActionsTable table = new SectionPageChangeActionsTable();
			table.Add(SectionPageChangeType.None, DocumentModelChangeActions.None);
			table.Add(SectionPageChangeType.Width, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler);
			table.Add(SectionPageChangeType.Height, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionPageChangeType.Landscape, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionPageChangeType.PaperKind, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			table.Add(SectionPageChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ForceResize | DocumentModelChangeActions.ForceResetHorizontalRuler | DocumentModelChangeActions.ForceResetVerticalRuler);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionPageChangeType change) {
			return sectionPageChangeActionsTable[change];
		}
	}
	#endregion
	#region GeneralSectionInfo
	public class GeneralSectionInfo : ICloneable<GeneralSectionInfo>, ISupportsCopyFrom<GeneralSectionInfo>, ISupportsSizeOf {
		#region Fields
		bool onlyAllowEditingOfFormFields;
		bool differentFirstPage;
		int firstPagePaperSource;
		int otherPagePaperSource;
		TextDirection textDirection;
		VerticalAlignment verticalTextAlignment;
		SectionStartType startType;
		#endregion
		#region Properties
		public bool OnlyAllowEditingOfFormFields { get { return onlyAllowEditingOfFormFields; } set { onlyAllowEditingOfFormFields = value; } }
		public bool DifferentFirstPage { get { return differentFirstPage; } set { differentFirstPage = value; } }
		public int FirstPagePaperSource { get { return firstPagePaperSource; } set { firstPagePaperSource = value; } }
		public int OtherPagePaperSource { get { return otherPagePaperSource; } set { otherPagePaperSource = value; } }
		public TextDirection TextDirection { get { return textDirection; } set { textDirection = value; } }
		public VerticalAlignment VerticalTextAlignment { get { return verticalTextAlignment; } set { verticalTextAlignment = value; } }
		public SectionStartType StartType { get { return startType; } set { startType = value; } }
		#endregion
		#region ICloneable<GeneralSectionInfo> Members
		public GeneralSectionInfo Clone() {
			GeneralSectionInfo info = new GeneralSectionInfo();
			info.CopyFrom(this);
			return info;
		}
		#endregion
		public void CopyFrom(GeneralSectionInfo info) {
			this.OnlyAllowEditingOfFormFields = info.OnlyAllowEditingOfFormFields;
			this.DifferentFirstPage = info.DifferentFirstPage;
			this.FirstPagePaperSource = info.FirstPagePaperSource;
			this.OtherPagePaperSource = info.OtherPagePaperSource;
			this.TextDirection = info.TextDirection;
			this.VerticalTextAlignment = info.VerticalTextAlignment;
			this.StartType = info.StartType;
		}
		public override bool Equals(object obj) {
			GeneralSectionInfo info = (GeneralSectionInfo)obj;
			return
				this.StartType == info.StartType &&
				this.TextDirection == info.TextDirection &&
				this.VerticalTextAlignment == info.VerticalTextAlignment &&
				this.OnlyAllowEditingOfFormFields == info.OnlyAllowEditingOfFormFields &&
				this.DifferentFirstPage == info.DifferentFirstPage &&
				this.FirstPagePaperSource == info.FirstPagePaperSource &&
				this.OtherPagePaperSource == info.OtherPagePaperSource;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region GeneralSectionInfoCache
	public class GeneralSectionInfoCache : UniqueItemsCache<GeneralSectionInfo> {
		public GeneralSectionInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override GeneralSectionInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new GeneralSectionInfo();
		}
	}
	#endregion
	#region SectionGeneralSettings
	public class SectionGeneralSettings : RichEditIndexBasedObject<GeneralSectionInfo> {
		public SectionGeneralSettings(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region OnlyAllowEditingOfFormFields
		public bool OnlyAllowEditingOfFormFields {
			get { return Info.OnlyAllowEditingOfFormFields; }
			set {
				if (OnlyAllowEditingOfFormFields == value)
					return;
				SetPropertyValue(SetOnlyAllowEditingOfFormFieldsCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetOnlyAllowEditingOfFormFieldsCore(GeneralSectionInfo info, bool value) {
			info.OnlyAllowEditingOfFormFields = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.OnlyAllowEditingOfFormFields);
		}
		#endregion
		#region DifferentFirstPage
		public bool DifferentFirstPage {
			get { return Info.DifferentFirstPage; }
			set {
				if (DifferentFirstPage == value)
					return;
				SetPropertyValue(SetDifferentFirstPageCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDifferentFirstPageCore(GeneralSectionInfo info, bool value) {
			info.DifferentFirstPage = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.DifferentFirstPage);
		}
		#endregion
		#region FirstPagePaperSource
		public int FirstPagePaperSource {
			get { return Info.FirstPagePaperSource; }
			set {
				if (FirstPagePaperSource == value)
					return;
				SetPropertyValue(SetFirstPagePaperSourceCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetFirstPagePaperSourceCore(GeneralSectionInfo info, int value) {
			info.FirstPagePaperSource = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.FirstPagePaperSource);
		}
		#endregion
		#region OtherPagePaperSource
		public int OtherPagePaperSource {
			get { return Info.OtherPagePaperSource; }
			set {
				if (OtherPagePaperSource == value)
					return;
				SetPropertyValue(SetOtherPagePaperSourceCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetOtherPagePaperSourceCore(GeneralSectionInfo info, int value) {
			info.OtherPagePaperSource = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.OtherPagePaperSource);
		}
		#endregion
		#region TextDirection
		public TextDirection TextDirection {
			get { return Info.TextDirection; }
			set {
				if (TextDirection == value)
					return;
				SetPropertyValue(SetTextDirectionCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetTextDirectionCore(GeneralSectionInfo info, TextDirection value) {
			info.TextDirection = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.TextDirection);
		}
		#endregion
		#region VerticalTextAlignment
		public VerticalAlignment VerticalTextAlignment {
			get { return Info.VerticalTextAlignment; }
			set {
				if (VerticalTextAlignment == value)
					return;
				SetPropertyValue(SetVerticalTextAlignmentCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetVerticalTextAlignmentCore(GeneralSectionInfo info, VerticalAlignment value) {
			info.VerticalTextAlignment = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.VerticalTextAlignement);
		}
		#endregion
		#region StartType
		public SectionStartType StartType {
			get { return Info.StartType; }
			set {
				if (StartType == value)
					return;
				SetPropertyValue(SetStartTypeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStartTypeCore(GeneralSectionInfo info, SectionStartType value) {
			info.StartType = value;
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.StartType);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<GeneralSectionInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.GeneralSectionInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionGeneralSettingsChangeActionsCalculator.CalculateChangeActions(SectionGeneralSettingsChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionGeneralSettingsChangeType
	public enum SectionGeneralSettingsChangeType {
		None = 0,
		OnlyAllowEditingOfFormFields,
		DifferentFirstPage,
		FirstPagePaperSource,
		OtherPagePaperSource,
		TextDirection,
		VerticalTextAlignement,
		StartType,
		BatchUpdate
	}
	#endregion
	#region SectionGeneralSettingsChangeActionsCalculator
	public static class SectionGeneralSettingsChangeActionsCalculator {
		internal class SectionGeneralSettingsChangeActionsTable : Dictionary<SectionGeneralSettingsChangeType, DocumentModelChangeActions> {
		}
		internal static SectionGeneralSettingsChangeActionsTable sectionGeneralSettingsChangeActionsTable = CreateSectionGeneralSettingsChangeActionsTable();
		internal static SectionGeneralSettingsChangeActionsTable CreateSectionGeneralSettingsChangeActionsTable() {
			SectionGeneralSettingsChangeActionsTable table = new SectionGeneralSettingsChangeActionsTable();
			table.Add(SectionGeneralSettingsChangeType.None, DocumentModelChangeActions.None);
			table.Add(SectionGeneralSettingsChangeType.OnlyAllowEditingOfFormFields, DocumentModelChangeActions.None);
			table.Add(SectionGeneralSettingsChangeType.DifferentFirstPage, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionGeneralSettingsChangeType.FirstPagePaperSource, DocumentModelChangeActions.None);
			table.Add(SectionGeneralSettingsChangeType.OtherPagePaperSource, DocumentModelChangeActions.None);
			table.Add(SectionGeneralSettingsChangeType.TextDirection, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionGeneralSettingsChangeType.VerticalTextAlignement, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionGeneralSettingsChangeType.StartType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionGeneralSettingsChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionGeneralSettingsChangeType change) {
			return sectionGeneralSettingsChangeActionsTable[change];
		}
	}
	#endregion
	#region SectionStartType
	public enum SectionStartType {
		NextPage = 0,
		OddPage,
		EvenPage,
		Continuous,
		Column
	}
	#endregion
	#region PageNumberingInfo
	public class PageNumberingInfo : ICloneable<PageNumberingInfo>, ISupportsCopyFrom<PageNumberingInfo>, ISupportsSizeOf {
		#region Fields
		char chapterSeparator;
		int chapterHeaderStyle;
		NumberingFormat numberingFormat;
		int startingPageNumber;
		#endregion
		#region Properties
		public char ChapterSeparator { get { return chapterSeparator; } set { chapterSeparator = value; } }
		public int ChapterHeaderStyle { get { return chapterHeaderStyle; } set { chapterHeaderStyle = value; } }
		public NumberingFormat NumberingFormat { get { return numberingFormat; } set { numberingFormat = value; } }
		public int StartingPageNumber { get { return startingPageNumber; } set { startingPageNumber = value; } }
		#endregion
		#region ICloneable<PageNumberingInfo> Members
		public PageNumberingInfo Clone() {
			PageNumberingInfo info = new PageNumberingInfo();
			info.CopyFrom(this);
			return info;
		}
		#endregion
		public void CopyFrom(PageNumberingInfo info) {
			this.ChapterSeparator = info.ChapterSeparator;
			this.ChapterHeaderStyle = info.ChapterHeaderStyle;
			this.NumberingFormat = info.NumberingFormat;
			this.StartingPageNumber = info.StartingPageNumber;
		}
		public override bool Equals(object obj) {
			PageNumberingInfo info = (PageNumberingInfo)obj;
			return
				this.ChapterSeparator == info.ChapterSeparator &&
				this.ChapterHeaderStyle == info.ChapterHeaderStyle &&
				this.NumberingFormat == info.NumberingFormat &&
				this.StartingPageNumber == info.StartingPageNumber;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region PageNumberingInfoCache
	public class PageNumberingInfoCache : UniqueItemsCache<PageNumberingInfo> {
		public PageNumberingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override PageNumberingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			PageNumberingInfo result = new PageNumberingInfo();
			result.StartingPageNumber = -1;
			return result;
		}
	}
	#endregion
	#region SectionPageNumbering
	public class SectionPageNumbering : RichEditIndexBasedObject<PageNumberingInfo> {
		public SectionPageNumbering(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region ChapterSeparator
		public char ChapterSeparator {
			get { return Info.ChapterSeparator; }
			set {
				if (ChapterSeparator == value)
					return;
				SetPropertyValue(SetChapterSeparatorCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetChapterSeparatorCore(PageNumberingInfo info, char value) {
			info.ChapterSeparator = value;
			return SectionPageNumberingChangeActionsCalculator.CalculateChangeActions(SectionPageNumberingChangeType.ChapterSeparator);
		}
		#endregion
		#region ChapterHeaderStyle
		public int ChapterHeaderStyle {
			get { return Info.ChapterHeaderStyle; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("ChapterHeaderStyle", value);
				if (ChapterHeaderStyle == value)
					return;
				SetPropertyValue(SetChapterHeaderStyleCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetChapterHeaderStyleCore(PageNumberingInfo info, int value) {
			info.ChapterHeaderStyle = value;
			return SectionPageNumberingChangeActionsCalculator.CalculateChangeActions(SectionPageNumberingChangeType.ChapterHeaderStyle);
		}
		#endregion
		#region NumberingFormat
		public NumberingFormat NumberingFormat {
			get { return Info.NumberingFormat; }
			set {
				if (NumberingFormat == value)
					return;
				SetPropertyValue(SetNumberingFormatCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetNumberingFormatCore(PageNumberingInfo info, NumberingFormat value) {
			info.NumberingFormat = value;
			return SectionPageNumberingChangeActionsCalculator.CalculateChangeActions(SectionPageNumberingChangeType.NumberingFormat);
		}
		#endregion
		#region StartingPageNumber
		public int StartingPageNumber {
			get { return Info.StartingPageNumber; }
			set {
				if (StartingPageNumber == value)
					return;
				SetPropertyValue(SetStartingPageNumberCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStartingPageNumberCore(PageNumberingInfo info, int value) {
			info.StartingPageNumber = value;
			return SectionPageNumberingChangeActionsCalculator.CalculateChangeActions(SectionPageNumberingChangeType.StartingPageNumber);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<PageNumberingInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.PageNumberingInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionPageNumberingChangeActionsCalculator.CalculateChangeActions(SectionPageNumberingChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionPageNumberingChangeType
	public enum SectionPageNumberingChangeType {
		None = 0,
		ChapterSeparator,
		ChapterHeaderStyle,
		NumberingFormat,
		StartingPageNumber,
		BatchUpdate
	}
	#endregion
	#region SectionPageNumberingChangeActionsCalculator
	public static class SectionPageNumberingChangeActionsCalculator {
		internal class SectionPageNumberingChangeActionsTable : Dictionary<SectionPageNumberingChangeType, DocumentModelChangeActions> {
		}
		internal static SectionPageNumberingChangeActionsTable sectionPageNumberingChangeActionsTable = CreateSectionPageNumberingChangeActionsTable();
		internal static SectionPageNumberingChangeActionsTable CreateSectionPageNumberingChangeActionsTable() {
			SectionPageNumberingChangeActionsTable table = new SectionPageNumberingChangeActionsTable();
			table.Add(SectionPageNumberingChangeType.None, DocumentModelChangeActions.None);
			table.Add(SectionPageNumberingChangeType.ChapterSeparator, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionPageNumberingChangeType.ChapterHeaderStyle, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionPageNumberingChangeType.NumberingFormat, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionPageNumberingChangeType.StartingPageNumber, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			table.Add(SectionPageNumberingChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionPageNumberingChangeType change) {
			return sectionPageNumberingChangeActionsTable[change];
		}
	}
	#endregion
	#region LineNumberingRestart
	public enum LineNumberingRestart {
		NewPage,
		NewSection,
		Continuous,
	}
	#endregion
	#region LineNumberingInfo
	public class LineNumberingInfo : ICloneable<LineNumberingInfo>, ISupportsCopyFrom<LineNumberingInfo>, ISupportsSizeOf {
		#region Fields
		int distance;
		int startingLineNumber;
		int step;
		LineNumberingRestart numberingRestartType;
		#endregion
		#region Properties
		public int Distance { get { return distance; } set { distance = value; } }
		public int StartingLineNumber { get { return startingLineNumber; } set { startingLineNumber = value; } }
		public int Step { get { return step; } set { step = value; } }
		public LineNumberingRestart NumberingRestartType { get { return numberingRestartType; } set { numberingRestartType = value; } }
		#endregion
		#region ICloneable<LineNumberingInfo> Members
		public LineNumberingInfo Clone() {
			LineNumberingInfo info = new LineNumberingInfo();
			info.CopyFrom(this);
			return info;
		}
		#endregion
		public virtual void CopyFrom(LineNumberingInfo info) {
			this.Distance = info.Distance;
			this.StartingLineNumber = info.StartingLineNumber;
			this.Step = info.Step;
			this.NumberingRestartType = info.NumberingRestartType;
		}
		public override bool Equals(object obj) {
			LineNumberingInfo info = (LineNumberingInfo)obj;
			return
				this.Distance == info.Distance &&
				this.StartingLineNumber == info.StartingLineNumber &&
				this.Step == info.Step &&
				this.NumberingRestartType == info.NumberingRestartType;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return ObjectSizeHelper.CalculateApproxObjectSize32(this, true);
		}
		#endregion
	}
	#endregion
	#region LineNumberingInfoCache
	public class LineNumberingInfoCache : UniqueItemsCache<LineNumberingInfo> {
		public LineNumberingInfoCache(DocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override LineNumberingInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			LineNumberingInfo info = new LineNumberingInfo();
			info.Distance = 0; 
			info.StartingLineNumber = 1;
			info.Step = 0; 
			return info;
		}
	}
	#endregion
	#region SectionLineNumbering
	public class SectionLineNumbering : RichEditIndexBasedObject<LineNumberingInfo> {
		public SectionLineNumbering(DocumentModel documentModel)
			: base(Section.GetMainPieceTable(documentModel)) {
		}
		#region Properties
		#region Distance
		public int Distance {
			get { return Info.Distance; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Distance", value);
				if (Distance == value)
					return;
				SetPropertyValue(SetDistanceCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetDistanceCore(LineNumberingInfo info, int value) {
			info.Distance = value;
			return SectionLineNumberingChangeActionsCalculator.CalculateChangeActions(SectionLineNumberingChangeType.Distance);
		}
		#endregion
		#region StartingLineNumber
		public int StartingLineNumber {
			get { return Info.StartingLineNumber; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("StartingLineNumber", value);
				if (StartingLineNumber == value)
					return;
				SetPropertyValue(SetStartingLineNumberCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStartingLineNumberCore(LineNumberingInfo info, int value) {
			info.StartingLineNumber = value;
			return SectionLineNumberingChangeActionsCalculator.CalculateChangeActions(SectionLineNumberingChangeType.StartingLineNumber);
		}
		#endregion
		#region Step
		public int Step {
			get { return Info.Step; }
			set {
				if (value < 0)
					Exceptions.ThrowArgumentException("Step", value);
				if (Step == value)
					return;
				SetPropertyValue(SetStepCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetStepCore(LineNumberingInfo info, int value) {
			info.Step = value;
			return SectionLineNumberingChangeActionsCalculator.CalculateChangeActions(SectionLineNumberingChangeType.Step);
		}
		#endregion
		#region NumberingRestartType
		public LineNumberingRestart NumberingRestartType {
			get { return Info.NumberingRestartType; }
			set {
				if (NumberingRestartType == value)
					return;
				SetPropertyValue(SetNumberingRestartTypeCore, value);
			}
		}
		protected internal virtual DocumentModelChangeActions SetNumberingRestartTypeCore(LineNumberingInfo info, LineNumberingRestart value) {
			info.NumberingRestartType = value;
			return SectionLineNumberingChangeActionsCalculator.CalculateChangeActions(SectionLineNumberingChangeType.NumberingRestartType);
		}
		#endregion
		#endregion
		protected internal override UniqueItemsCache<LineNumberingInfo> GetCache(DocumentModel documentModel) {
			return documentModel.Cache.LineNumberingInfoCache;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return SectionLineNumberingChangeActionsCalculator.CalculateChangeActions(SectionLineNumberingChangeType.BatchUpdate);
		}
	}
	#endregion
	#region SectionLineNumberingChangeType
	public enum SectionLineNumberingChangeType {
		None = 0,
		Distance,
		StartingLineNumber,
		Step,
		NumberingRestartType,
		BatchUpdate
	}
	#endregion
	#region SectionLineNumberingChangeActionsCalculator
	public static class SectionLineNumberingChangeActionsCalculator {
		internal class SectionLineNumberingChangeActionsTable : Dictionary<SectionLineNumberingChangeType, DocumentModelChangeActions> {
		}
		internal static SectionLineNumberingChangeActionsTable sectionLineNumberingChangeActionsTable = CreateSectionLineNumberingChangeActionsTable();
		internal static SectionLineNumberingChangeActionsTable CreateSectionLineNumberingChangeActionsTable() {
			SectionLineNumberingChangeActionsTable table = new SectionLineNumberingChangeActionsTable();
			table.Add(SectionLineNumberingChangeType.None, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionLineNumberingChangeType.Distance, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionLineNumberingChangeType.StartingLineNumber, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionLineNumberingChangeType.Step, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionLineNumberingChangeType.NumberingRestartType, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			table.Add(SectionLineNumberingChangeType.BatchUpdate, DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetPrimaryLayout | DocumentModelChangeActions.ResetSecondaryLayout | DocumentModelChangeActions.ResetSelectionLayout);
			return table;
		}
		public static DocumentModelChangeActions CalculateChangeActions(SectionLineNumberingChangeType change) {
			return sectionLineNumberingChangeActionsTable[change];
		}
	}
	#endregion
	public class SectionPrinterSettings {
	}
	public enum HeaderFooterType {
		First = 0,
		Odd = 1,
		Even = 2,
		Primary = Odd
	}
	#region Section
	public class Section {
		#region Fields
		readonly DocumentModel documentModel;
		ParagraphIndex firstParagraphIndex = new ParagraphIndex(-1);
		ParagraphIndex lastParagraphIndex = new ParagraphIndex(-1);
		readonly SectionMargins margins;
		readonly SectionColumns columns;
		readonly SectionPage page;
		readonly SectionGeneralSettings generalSettings;
		readonly SectionPageNumbering pageNumbering;
		readonly SectionLineNumbering lineNumbering;
		readonly SectionFootNote footNote;
		readonly SectionFootNote endNote;
		readonly SectionHeaders headers;
		readonly SectionFooters footers;
		#endregion
		public Section(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.margins = new SectionMargins(documentModel);
			this.columns = new SectionColumns(documentModel);
			this.page = new SectionPage(documentModel);
			this.generalSettings = new SectionGeneralSettings(documentModel);
			this.pageNumbering = new SectionPageNumbering(documentModel);
			this.lineNumbering = new SectionLineNumbering(documentModel);
			this.footNote = new SectionFootNote(documentModel);
			this.endNote = new SectionFootNote(documentModel);
			this.endNote.SetIndexInitial(FootNoteInfoCache.DefaultEndNoteItemIndex);
			this.headers = new SectionHeaders(this);
			this.footers = new SectionFooters(this);
			SubscribeInnerObjectsEvents();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public SectionMargins Margins { get { return margins; } }
		public SectionColumns Columns { get { return columns; } }
		public SectionPage Page { get { return page; } }
		public SectionGeneralSettings GeneralSettings { get { return generalSettings; } }
		public SectionPageNumbering PageNumbering { get { return pageNumbering; } }
		public SectionLineNumbering LineNumbering { get { return lineNumbering; } }
		public SectionFootNote FootNote { get { return footNote; } }
		public SectionFootNote EndNote { get { return endNote; } }
		public ParagraphIndex FirstParagraphIndex { get { return firstParagraphIndex; } set { firstParagraphIndex = value; } }
		public ParagraphIndex LastParagraphIndex { get { return lastParagraphIndex; } set { lastParagraphIndex = value; } }
		public bool HasNonEmptyHeadersOrFooters {
			get {
				SectionHeaderFooterBase headerFooter;
				headerFooter = InnerFirstPageHeader;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				headerFooter = InnerOddPageHeader;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				headerFooter = InnerEvenPageHeader;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				headerFooter = InnerFirstPageFooter;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				headerFooter = InnerOddPageFooter;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				headerFooter = InnerEvenPageFooter;
				if (headerFooter != null && !headerFooter.PieceTable.IsEmpty)
					return true;
				return false;
			}
		}
		public SectionHeaders Headers { get { return headers; } }
		public SectionFooters Footers { get { return footers; } }
		#region FirstPageHeader
		public SectionHeader FirstPageHeader {
			get {
				if (GeneralSettings.DifferentFirstPage)
					return InnerFirstPageHeader;
				else
					return null;
			}
		}
		public SectionHeader InnerFirstPageHeader { get { return Headers.GetObject(HeaderFooterType.First); } }
		public HeaderIndex InnerFirstPageHeaderIndex {
			get { return Headers.GetObjectIndex(HeaderFooterType.First); }
			protected internal set { Headers.SetObjectIndex(HeaderFooterType.First, value); }
		}
		#endregion
		#region OddPageHeader
		public SectionHeader OddPageHeader {
			get {
				return InnerOddPageHeader;
			}
		}
		public SectionHeader InnerOddPageHeader { get { return Headers.GetObject(HeaderFooterType.Odd); } }
		public HeaderIndex InnerOddPageHeaderIndex {
			get { return Headers.GetObjectIndex(HeaderFooterType.Odd); }
			protected internal set { Headers.SetObjectIndex(HeaderFooterType.Odd, value); }
		}
		#endregion
		#region EvenPageHeader
		public SectionHeader EvenPageHeader {
			get {
				if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages)
					return InnerEvenPageHeader;
				else
					return InnerOddPageHeader;
			}
		}
		public SectionHeader InnerEvenPageHeader { get { return Headers.GetObject(HeaderFooterType.Even); } }
		public HeaderIndex InnerEvenPageHeaderIndex {
			get { return Headers.GetObjectIndex(HeaderFooterType.Even); }
			protected internal set { Headers.SetObjectIndex(HeaderFooterType.Even, value); }
		}
		#endregion
		#region FirstPageFooter
		public SectionFooter FirstPageFooter {
			get {
				if (GeneralSettings.DifferentFirstPage)
					return InnerFirstPageFooter;
				else
					return null;
			}
		}
		public SectionFooter InnerFirstPageFooter { get { return Footers.GetObject(HeaderFooterType.First); } }
		public FooterIndex InnerFirstPageFooterIndex {
			get { return Footers.GetObjectIndex(HeaderFooterType.First); }
			protected internal set { Footers.SetObjectIndex(HeaderFooterType.First, value); }
		}
		#endregion
		#region OddPageFooter
		public SectionFooter OddPageFooter { get { return InnerOddPageFooter; } }
		public SectionFooter InnerOddPageFooter { get { return Footers.GetObject(HeaderFooterType.Odd); } }
		public FooterIndex InnerOddPageFooterIndex {
			get { return Footers.GetObjectIndex(HeaderFooterType.Odd); }
			protected internal set { Footers.SetObjectIndex(HeaderFooterType.Odd, value); }
		}
		#endregion
		#region EvenPageFooter
		public SectionFooter EvenPageFooter {
			get {
				if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages)
					return InnerEvenPageFooter;
				else
					return InnerOddPageFooter;
			}
		}
		public SectionFooter InnerEvenPageFooter { get { return Footers.GetObject(HeaderFooterType.Even); } }
		public FooterIndex InnerEvenPageFooterIndex {
			get { return Footers.GetObjectIndex(HeaderFooterType.Even); }
			protected internal set { Footers.SetObjectIndex(HeaderFooterType.Even, value); }
		}
		#endregion
		#endregion
		internal static PieceTable GetMainPieceTable(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			return documentModel.MainPieceTable;
		}
		protected internal virtual void SubscribeInnerObjectsEvents() {
			margins.ObtainAffectedRange += OnObtainAffectedRange;
			columns.ObtainAffectedRange += OnObtainAffectedRange;
			page.ObtainAffectedRange += OnObtainAffectedRange;
			generalSettings.ObtainAffectedRange += OnObtainAffectedRange;
			pageNumbering.ObtainAffectedRange += OnObtainAffectedRange;
			lineNumbering.ObtainAffectedRange += OnObtainAffectedRange;
			footNote.ObtainAffectedRange += OnObtainAffectedRange;
			endNote.ObtainAffectedRange += OnObtainAffectedRange;
		}
		protected internal virtual void UnsubscribeInnerObjectsEvents() {
			margins.ObtainAffectedRange -= OnObtainAffectedRange;
			columns.ObtainAffectedRange -= OnObtainAffectedRange;
			page.ObtainAffectedRange -= OnObtainAffectedRange;
			generalSettings.ObtainAffectedRange -= OnObtainAffectedRange;
			pageNumbering.ObtainAffectedRange -= OnObtainAffectedRange;
			lineNumbering.ObtainAffectedRange -= OnObtainAffectedRange;
			footNote.ObtainAffectedRange -= OnObtainAffectedRange;
			endNote.ObtainAffectedRange -= OnObtainAffectedRange;
		}
		protected internal virtual void SubscribeHeadersFootersEvents() {
			Headers.SubscribeEvents();
			Footers.SubscribeEvents();
		}
		protected internal virtual void UnsubscribeHeadersFootersEvents() {
			Headers.UnsubscribeEvents();
			Footers.UnsubscribeEvents();
		}
		protected internal virtual void OnObtainAffectedRange(object sender, ObtainAffectedRangeEventArgs e) {
			ParagraphCollection paragraphs = DocumentModel.MainPieceTable.Paragraphs;
			if (FirstParagraphIndex >= ParagraphIndex.Zero) {
				e.Start = paragraphs[FirstParagraphIndex].FirstRunIndex;
				e.End = paragraphs[LastParagraphIndex].LastRunIndex;
			}
		}
		protected internal virtual Section Copy(DocumentModelCopyManager copyManager) {
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			DocumentModel targetModel = copyManager.TargetModel;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			targetModel.InsertSection(targetPosition.LogPosition);
			SectionIndex sectionIndex = targetModel.MainPieceTable.LookupSectionIndexByParagraphIndex(targetPosition.ParagraphIndex);
			Section resultSection = targetModel.Sections[sectionIndex];
			resultSection.CopyFromCore(this);
			Paragraph lastParagraph = targetModel.MainPieceTable.Paragraphs[resultSection.LastParagraphIndex];
			DocumentModel.MainPieceTable.Paragraphs[this.LastParagraphIndex].CopyFrom(targetModel, lastParagraph);
			resultSection.Headers.CopyFrom(this);
			resultSection.Footers.CopyFrom(this);
			return resultSection;
		}
		protected internal virtual void CopyFrom(Section section) {
			Debug.Assert(Object.ReferenceEquals(section.DocumentModel, DocumentModel));
			CopyFromCore(section);
			Headers.CopyFrom(section.Headers);
			Footers.CopyFrom(section.Footers);
			this.FirstParagraphIndex = section.FirstParagraphIndex;
			this.LastParagraphIndex = section.LastParagraphIndex;
		}
		protected internal virtual void CopyFromCore(Section section) {
			this.Margins.CopyFrom(section.Margins.Info);
			this.Columns.CopyFrom(section.Columns.Info);
			this.Page.CopyFrom(section.Page.Info);
			this.GeneralSettings.CopyFrom(section.GeneralSettings.Info);
			this.PageNumbering.CopyFrom(section.PageNumbering.Info);
			this.LineNumbering.CopyFrom(section.LineNumbering.Info);
			this.FootNote.CopyFrom(section.FootNote.Info);
			this.EndNote.CopyFrom(section.EndNote.Info);
		}
		public void Reset() {
			DocumentCache cache = documentModel.Cache;
			Page.CopyFrom(cache.PageInfoCache[0]);
			Margins.CopyFrom(cache.MarginsInfoCache[0]);
			PageNumbering.CopyFrom(cache.PageNumberingInfoCache[0]);
			GeneralSettings.CopyFrom(cache.GeneralSectionInfoCache[0]);
			LineNumbering.CopyFrom(cache.LineNumberingInfoCache[0]);
			Columns.CopyFrom(cache.ColumnsInfoCache[0]);
			FootNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultFootNoteItemIndex]);
			EndNote.CopyFrom(cache.FootNoteInfoCache[FootNoteInfoCache.DefaultEndNoteItemIndex]);
		}
		public virtual bool IsHidden() {
			ParagraphCollection paragraphs = DocumentModel.MainPieceTable.Paragraphs;
			for (ParagraphIndex i = FirstParagraphIndex; i <= LastParagraphIndex; i++) {
				if (!paragraphs[i].IsHidden())
					return false;
			}
			return true;
		}
		protected internal virtual Section GetPreviousSection() {
			SectionIndex index = DocumentModel.Sections.IndexOf(this);
			if (index <= new SectionIndex(0))
				return null;
			return DocumentModel.Sections[index - 1];
		}
		protected internal virtual Section GetNextSection() {
			SectionIndex index = DocumentModel.Sections.IndexOf(this);
			if (index + 1 >= new SectionIndex(DocumentModel.Sections.Count))
				return null;
			return DocumentModel.Sections[index + 1];
		}
		protected internal void AddPieceTables(List<PieceTable> result, bool includeUnreferenced) {
			if (InnerFirstPageHeader != null)
				InnerFirstPageHeader.PieceTable.AddPieceTables(result, includeUnreferenced);
			if (InnerOddPageHeader != null)
				InnerOddPageHeader.PieceTable.AddPieceTables(result, includeUnreferenced);
			if (InnerEvenPageHeader != null)
				InnerEvenPageHeader.PieceTable.AddPieceTables(result, includeUnreferenced);
			if (InnerFirstPageFooter != null)
				InnerFirstPageFooter.PieceTable.AddPieceTables(result, includeUnreferenced);
			if (InnerOddPageFooter != null)
				InnerOddPageFooter.PieceTable.AddPieceTables(result, includeUnreferenced);
			if (InnerEvenPageFooter != null)
				InnerEvenPageFooter.PieceTable.AddPieceTables(result, includeUnreferenced);
		}
		protected internal virtual SectionHeader GetCorrespondingHeader(SectionFooter footer) {
			return Headers.GetObject(footer.Type);
		}
		protected internal virtual SectionFooter GetCorrespondingFooter(SectionHeader header) {
			return Footers.GetObject(header.Type);
		}
		protected internal virtual SectionHeader GetPreviousSectionHeader(SectionFooter header) {
			Section previousSection = GetPreviousSection();
			if (previousSection == null)
				return null;
			return previousSection.Headers.GetObject(header.Type);
		}
		protected internal virtual SectionFooter GetPreviousSectionFooter(SectionFooter footer) {
			Section previousSection = GetPreviousSection();
			if (previousSection == null)
				return null;
			return previousSection.Footers.GetObject(footer.Type);
		}
		internal int GetActualColumnsCount() {
			if (Columns.EqualWidthColumns)
				return Columns.ColumnCount;
			else
				return Columns.Info.Columns.Count;
		}
	}
	#endregion
	#region JSONSectionProperty
	public enum JSONSectionProperty {
		MarginLeft = 0,
		MarginTop = 1,
		MarginRight = 2,
		MarginBottom = 3,
		ColumnCount = 4,
		Space = 5,
		ColumnsInfo = 6,
		PageWidth = 7,
		PageHeight = 8,
		StartType = 9,
		Landscape = 10,
		EqualWidthColumns = 11,
		DifferentFirstPage = 12,
		HeaderOffset = 13,
		FooterOffset = 14
	}
	#endregion
	#region SectionHeadersFootersBase (abstract class)
	public abstract class SectionHeadersFootersBase {
		readonly Section section;
		protected SectionHeadersFootersBase(Section section) {
			Guard.ArgumentNotNull(section, "section");
			this.section = section;
		}
		protected internal Section Section { get { return section; } }
		protected internal DocumentModel DocumentModel { get { return section.DocumentModel; } }
		protected internal SectionHeaderFooterBase CalculateActualObjectCore(bool firstPageOfSection, bool isEvenPage) {
			if (firstPageOfSection) {
				if (Section.GeneralSettings.DifferentFirstPage)
					return GetObjectCore(HeaderFooterType.First);
			}
			if (isEvenPage) {
				if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages)
					return GetObjectCore(HeaderFooterType.Even);
				else
					return GetObjectCore(HeaderFooterType.Odd);
			}
			else
				return GetObjectCore(HeaderFooterType.Odd);
		}
		protected internal HeaderFooterType CalculateActualObjectType(bool firstPageOfSection, bool isEvenPage) {
			HeaderFooterType result = HeaderFooterType.Odd;
			if (firstPageOfSection) {
				if (Section.GeneralSettings.DifferentFirstPage)
					result = HeaderFooterType.First;
			}
			if (result != HeaderFooterType.Odd)
				return result;
			if (isEvenPage) {
				if (DocumentModel.DocumentProperties.DifferentOddAndEvenPages)
					return HeaderFooterType.Even;
				else
					return HeaderFooterType.Odd;
			}
			else
				return HeaderFooterType.Odd;
		}
		public abstract void Create(HeaderFooterType type);
		public abstract void Remove(HeaderFooterType type);
		public abstract bool CanLinkToPrevious(HeaderFooterType type);
		public abstract bool IsLinkedToPrevious(HeaderFooterType type);
		public abstract bool IsLinkedToNext(HeaderFooterType type);
		public abstract void LinkToPrevious(HeaderFooterType type);
		public abstract void LinkToNext(HeaderFooterType type);
		public abstract void UnlinkFromPrevious(HeaderFooterType type);
		public abstract void UnlinkFromNext(HeaderFooterType type);
		protected internal abstract SectionHeaderFooterBase GetObjectCore(HeaderFooterType type);
		protected internal abstract bool ShouldRelinkNextSection(HeaderFooterType type);
		protected internal abstract bool ShouldRelinkPreviousSection(HeaderFooterType type);
	}
	#endregion
	#region SectionHeadersFooters<TObject, TIndex> (abstract class)
	public abstract class SectionHeadersFooters<TObject, TIndex> : SectionHeadersFootersBase
		where TObject : SectionHeaderFooterBase
		where TIndex : struct, IConvertToInt<TIndex> {
		readonly TIndex[] indices;
		protected SectionHeadersFooters(Section section) : base(section) {
			this.indices = new TIndex[3];
			for (int i = 0; i < 3; i++)
				indices[i] = InvalidIndex;
		}
		protected internal abstract TIndex InvalidIndex { get; }
		protected internal abstract HeaderFooterCollectionBase<TObject, TIndex> ObjectCache { get; }
		protected internal abstract TObject CreateEmptyObjectCore(HeaderFooterType type);
		protected internal abstract TIndex CreateIndex(int value);
		protected internal abstract SectionHeaderFooterIndexChangedHistoryItem<TIndex> CreateHistoryItem();
		protected internal abstract SectionHeadersFooters<TObject, TIndex> GetObjectProvider(Section section);
		public virtual void CopyFrom(SectionHeadersFooters<TObject, TIndex> source) {
			Debug.Assert(Object.ReferenceEquals(DocumentModel, source.DocumentModel));
			UnsubscribeEvents();
			try {
				for (int i = 0; i < 3; i++)
					indices[i] = source.indices[i];
			}
			finally {
				SubscribeEvents();
			}
		}
		public override void Create(HeaderFooterType type) {
			ChangeObjectIndex(type, CreateEmptyObject(type));
		}
		public override void Remove(HeaderFooterType type) {
			ChangeObjectIndex(type, InvalidIndex);
		}
		public override bool CanLinkToPrevious(HeaderFooterType type) {
			return Section.GetPreviousSection() != null;
		}
		public override bool IsLinkedToPrevious(HeaderFooterType type) {
			Section previousSection = Section.GetPreviousSection();
			if (previousSection == null)
				return false;
			return Object.Equals(GetObjectProvider(previousSection).GetObjectIndex(type), GetObjectIndex(type));
		}
		public override bool IsLinkedToNext(HeaderFooterType type) {
			Section nextSection = Section.GetNextSection();
			if (nextSection == null)
				return false;
			return Object.Equals(GetObjectProvider(nextSection).GetObjectIndex(type), GetObjectIndex(type));
		}
		public override void LinkToPrevious(HeaderFooterType type) {
			PerformLinkToPrevious(type, true, LinkCore);
		}
		public override void UnlinkFromPrevious(HeaderFooterType type) {
			PerformLinkToPrevious(type, false, UnlinkCore);
		}
		public override void LinkToNext(HeaderFooterType type) {
			PerformLinkToNext(type, true, LinkCore);
		}
		public override void UnlinkFromNext(HeaderFooterType type) {
			PerformLinkToNext(type, false, UnlinkCore);
		}
		protected internal virtual void SubscribeEvents() {
			List<TIndex> objectIndices = GetValidUniqueObjectIndices();
			int count = objectIndices.Count;
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++)
				SubscribeObjectEvents(ObjectCache[objectIndices[i]]);
		}
		protected internal virtual void UnsubscribeEvents() {
			List<TIndex> objectIndices = GetValidUniqueObjectIndices();
			int count = objectIndices.Count;
			if (count <= 0)
				return;
			for (int i = 0; i < count; i++)
				UnsubscribeObjectEvents(ObjectCache[objectIndices[i]]);
		}
		protected internal virtual void SubscribeObjectEvents(TObject obj) {
			obj.RequestSectionIndex += OnRequestSectionIndex;
		}
		protected internal virtual void UnsubscribeObjectEvents(TObject obj) {
			obj.RequestSectionIndex -= OnRequestSectionIndex;
		}
		void OnRequestSectionIndex(object sender, RequestSectionIndexEventArgs args) {
			SectionIndex index = DocumentModel.Sections.IndexOf(Section);
			if (index < new SectionIndex(0))
				Exceptions.ThrowInternalException();
			args.SectionIndex = Algorithms.Min(index, args.SectionIndex);
		}
		protected internal virtual List<TIndex> GetValidUniqueObjectIndices() {
			List<TIndex> result = new List<TIndex>();
			for (int i = 0; i < 3; i++) {
				TIndex index = indices[i];
				if (!Object.Equals(index, InvalidIndex) && !result.Contains(index))
					result.Add(index);
			}
			return result;
		}
		internal delegate void LinkObjectDelegate(SectionHeadersFooters<TObject, TIndex> targetObject, HeaderFooterType type, TIndex sectionObjectIndex);
		internal virtual void PerformLinkToPrevious(HeaderFooterType type, bool link, LinkObjectDelegate linkAction) {
			Section previousSection = Section.GetPreviousSection();
			if (previousSection == null)
				return;
			TIndex previousSectionObjectIndex = GetObjectProvider(previousSection).GetObjectIndex(type);
			if (Object.Equals(previousSectionObjectIndex, GetObjectIndex(type)) == link)
				return;
			bool relinkNextSection = ShouldRelinkNextSection(type);
			linkAction(this, type, previousSectionObjectIndex);
			if (relinkNextSection)
				GetObjectProvider(Section.GetNextSection()).PerformLinkToPrevious(type, true, LinkCore);
		}
		protected internal override bool ShouldRelinkNextSection(HeaderFooterType type) {
			Section nextSection = Section.GetNextSection();
			if (nextSection != null)
				return (Object.Equals(GetObjectProvider(nextSection).GetObjectIndex(type), GetObjectIndex(type)));
			else
				return false;
		}
		internal virtual void PerformLinkToNext(HeaderFooterType type, bool link, LinkObjectDelegate linkAction) {
			Section nextSection = Section.GetNextSection();
			if (nextSection == null)
				return;
			TIndex nextSectionObjectIndex = GetObjectProvider(nextSection).GetObjectIndex(type);
			if (Object.Equals(nextSectionObjectIndex, GetObjectIndex(type)) == link)
				return;
			bool relinkPrevSection = ShouldRelinkPreviousSection(type);
			linkAction(this, type, nextSectionObjectIndex);
			if (relinkPrevSection)
				GetObjectProvider(Section.GetPreviousSection()).PerformLinkToNext(type, true, LinkCore);
		}
		protected internal override bool ShouldRelinkPreviousSection(HeaderFooterType type) {
			Section prevSection = Section.GetPreviousSection();
			if (prevSection != null)
				return (Object.Equals(GetObjectProvider(prevSection).GetObjectIndex(type), GetObjectIndex(type)));
			else
				return false;
		}
		static void LinkCore(SectionHeadersFooters<TObject, TIndex> targetObject, HeaderFooterType type, TIndex sectionObjectIndex) {
			targetObject.ChangeObjectIndex(type, sectionObjectIndex);
		}
		static void UnlinkCore(SectionHeadersFooters<TObject, TIndex> targetObject, HeaderFooterType type, TIndex sectionObjectIndex) {
			targetObject.UnlinkCore(type, sectionObjectIndex);
		}
		void UnlinkCore(HeaderFooterType type, TIndex sectionObjectIndex) {
			if (Object.ReferenceEquals(sectionObjectIndex, InvalidIndex))
				Create(type);
			else {
				TObject sectionObject = ObjectCache[sectionObjectIndex];
				ChangeObjectIndex(type, CreateObjectDeepCopy(sectionObject));
			}
		}
		protected internal TIndex GetObjectIndex(HeaderFooterType type) {
			return indices[(int)type];
		}
		protected internal void SetObjectIndex(HeaderFooterType type, TIndex index) {
			UnsubscribeEvents();
			try {
				indices[(int)type] = index;
			}
			finally {
				SubscribeEvents();
			}
		}
		protected internal TObject GetObject(HeaderFooterType type) {
			TIndex index = GetObjectIndex(type);
			if (Object.Equals(index, InvalidIndex))
				return null;
			else
				return ObjectCache[index];
		}
		protected internal override SectionHeaderFooterBase GetObjectCore(HeaderFooterType type) {
			return GetObject(type);
		}
		public TObject CalculateActualObject(bool firstPageOfSection, bool isEvenPage) {
			return (TObject)CalculateActualObjectCore(firstPageOfSection, isEvenPage);
		}
		protected internal TIndex CreateEmptyObject(HeaderFooterType type) {
			TObject newObject = CreateEmptyObjectCore(type);
			DocumentModel.UnsafeEditor.InsertFirstParagraph(newObject.PieceTable);
			TIndex result = CreateIndex(ObjectCache.Count);
			ObjectCache.Add(newObject);
			return result;
		}
		protected internal TIndex CreateObjectDeepCopy(TObject sourceObject) {
			TIndex result = CreateEmptyObject(sourceObject.Type);
			CopyObjectContent(sourceObject, ObjectCache[result]);
			return result;
		}
		protected internal void CopyObjectContent(TObject sourceHeaderFooter, TObject targetHeaderFooter) {
			PieceTable source = sourceHeaderFooter.PieceTable;
			PieceTable target = targetHeaderFooter.PieceTable;
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(source, target, ParagraphNumerationCopyOptions.CopyAlways);
			CopySectionOperation operation = source.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.FixLastParagraph = true;
			operation.Execute(source.DocumentStartLogPosition, source.DocumentEndLogPosition - source.DocumentStartLogPosition + 1, false);
		}
		protected internal virtual void CopyFrom(Section sourceSection) {
			SectionHeadersFooters<TObject, TIndex> source = GetObjectProvider(sourceSection);
			for (int i = 0; i < 3; i++) {
				HeaderFooterType type = (HeaderFooterType)i;
				if (!source.IsLinkedToPrevious(type)) {
					TObject sourceObject = source.GetObject(type);
					if (sourceObject != null) {
						Create(type);
						CopyObjectContent(sourceObject, GetObject(type));
					}
				}
				else
					LinkToPrevious(type);
			}
		}
		protected internal virtual void ChangeObjectIndex(HeaderFooterType type, TIndex newIndex) {
			DocumentModel.BeginUpdate();
			try {
				SectionHeaderFooterIndexChangedHistoryItem<TIndex> item = CreateHistoryItem();
				item.Type = type;
				item.PreviousIndex = GetObjectIndex(item.Type);
				item.NextIndex = newIndex;
				DocumentModel.History.Add(item);
				item.Execute();
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
	}
	#endregion
	#region SectionHeaders
	public class SectionHeaders : SectionHeadersFooters<SectionHeader, HeaderIndex> {
		public SectionHeaders(Section section)
			: base(section) {
		}
		protected internal override HeaderIndex InvalidIndex { get { return HeaderIndex.Invalid; } }
		protected internal override HeaderFooterCollectionBase<SectionHeader, HeaderIndex> ObjectCache { get { return DocumentModel.Headers; } }
		protected internal override SectionHeader CreateEmptyObjectCore(HeaderFooterType type) {
			return new SectionHeader(DocumentModel, type);
		}
		protected internal override HeaderIndex CreateIndex(int value) {
			return new HeaderIndex(value);
		}
		protected internal override SectionHeaderFooterIndexChangedHistoryItem<HeaderIndex> CreateHistoryItem() {
			return new SectionPageHeaderIndexChangedHistoryItem(Section);
		}
		protected internal override SectionHeadersFooters<SectionHeader, HeaderIndex> GetObjectProvider(Section section) {
			return section.Headers;
		}
	}
	#endregion
	#region SectionFooters
	public class SectionFooters : SectionHeadersFooters<SectionFooter, FooterIndex> {
		public SectionFooters(Section section)
			: base(section) {
		}
		protected internal override FooterIndex InvalidIndex { get { return FooterIndex.Invalid; } }
		protected internal override HeaderFooterCollectionBase<SectionFooter, FooterIndex> ObjectCache { get { return DocumentModel.Footers; } }
		protected internal override SectionFooter CreateEmptyObjectCore(HeaderFooterType type) {
			return new SectionFooter(DocumentModel, type);
		}
		protected internal override FooterIndex CreateIndex(int value) {
			return new FooterIndex(value);
		}
		protected internal override SectionHeaderFooterIndexChangedHistoryItem<FooterIndex> CreateHistoryItem() {
			return new SectionPageFooterIndexChangedHistoryItem(Section);
		}
		protected internal override SectionHeadersFooters<SectionFooter, FooterIndex> GetObjectProvider(Section section) {
			return section.Footers;
		}
	}
	#endregion
	#region SectionIndex
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct SectionIndex : IConvertToInt<SectionIndex>, IComparable<SectionIndex> {
		readonly int m_value;
		public static SectionIndex DontCare = new SectionIndex(-1);
		public static SectionIndex MinValue = new SectionIndex(0);
		public static SectionIndex MaxValue = new SectionIndex(int.MaxValue);
		public SectionIndex(int value) {
			m_value = value;
		}
		public override bool Equals(object obj) {
			return ((obj is SectionIndex) && (this.m_value == ((SectionIndex)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static SectionIndex operator +(SectionIndex index, int value) {
			return new SectionIndex(index.m_value + value);
		}
		public static int operator -(SectionIndex index1, SectionIndex index2) {
			return index1.m_value - index2.m_value;
		}
		public static SectionIndex operator -(SectionIndex index, int value) {
			return new SectionIndex(index.m_value - value);
		}
		public static SectionIndex operator ++(SectionIndex index) {
			return new SectionIndex(index.m_value + 1);
		}
		public static SectionIndex operator --(SectionIndex index) {
			return new SectionIndex(index.m_value - 1);
		}
		public static bool operator <(SectionIndex index1, SectionIndex index2) {
			return index1.m_value < index2.m_value;
		}
		public static bool operator <=(SectionIndex index1, SectionIndex index2) {
			return index1.m_value <= index2.m_value;
		}
		public static bool operator >(SectionIndex index1, SectionIndex index2) {
			return index1.m_value > index2.m_value;
		}
		public static bool operator >=(SectionIndex index1, SectionIndex index2) {
			return index1.m_value >= index2.m_value;
		}
		public static bool operator ==(SectionIndex index1, SectionIndex index2) {
			return index1.m_value == index2.m_value;
		}
		public static bool operator !=(SectionIndex index1, SectionIndex index2) {
			return index1.m_value != index2.m_value;
		}
		#region IConvertToInt<SectionIndex> Members
		int IConvertToInt<SectionIndex>.ToInt() {
			return m_value;
		}
		SectionIndex IConvertToInt<SectionIndex>.FromInt(int value) {
			return new SectionIndex(value);
		}
		#endregion
		#region IComparable<SectionIndex> Members
		public int CompareTo(SectionIndex other) {
			if (m_value < other.m_value)
				return -1;
			if (m_value > other.m_value)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region SectionCollection
	public class SectionCollection : List<Section, SectionIndex> {
	}
	#endregion
	#region SectionAndLogPositionComparable
	public class SectionAndLogPositionComparable : IComparable<Section> {
		readonly DocumentLogPosition logPosition;
		public SectionAndLogPositionComparable(DocumentLogPosition logPosition) {
			this.logPosition = logPosition;
		}
		public DocumentLogPosition LogPosition { get { return logPosition; } }
		#region IComparable<Section> Members
		public int CompareTo(Section section) {
			DocumentLogPosition firstParagraphLogPosition = section.DocumentModel.MainPieceTable.Paragraphs[section.FirstParagraphIndex].LogPosition;
			if (logPosition < firstParagraphLogPosition)
				return 1;
			else if (logPosition > firstParagraphLogPosition) {
				Paragraph lastParagraph = section.DocumentModel.MainPieceTable.Paragraphs[section.LastParagraphIndex];
				DocumentLogPosition lastParagraphLogPosition = lastParagraph.LogPosition;
				if (logPosition < lastParagraphLogPosition + lastParagraph.Length)
					return 0;
				else
					return -1;
			}
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region SectionParagraphIndexComparable
	public class SectionParagraphIndexComparable : IComparable<Section> {
		readonly ParagraphIndex paragraphIndex;
		public SectionParagraphIndexComparable(ParagraphIndex paragraphIndex) {
			this.paragraphIndex = paragraphIndex;
		}
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		#region IComparable<Section> Members
		public int CompareTo(Section section) {
			if (section.LastParagraphIndex < paragraphIndex)
				return -1;
			else if (section.FirstParagraphIndex > paragraphIndex)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
