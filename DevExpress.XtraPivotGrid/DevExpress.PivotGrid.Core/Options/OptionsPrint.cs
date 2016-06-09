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
using System.ComponentModel;
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Controls;
using System.Drawing.Printing;
using System.Text;
using DevExpress.Compatibility.System.Drawing.Printing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsPrint : PivotGridOptionsBase {
		DefaultBoolean printHorzLines;
		DefaultBoolean printVertLines;
		bool printHeadersOnEveryPage;
		DefaultBoolean[] showHeaders;
		int columnFieldValueSeparator;
		int rowFieldValueSeparator;
		bool usePrintAppearance;
		bool mergeColumnFieldValues;
		bool mergeRowFieldValues;
		bool printUnusedFilterFields;
		PivotGridPageSettings pageSettings;
		VerticalContentSplitting verticalContentSplitting;
		int filterSeparatorBarPadding;
		public PivotGridOptionsPrint(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.printHorzLines = DefaultBoolean.Default;
			this.printVertLines = DefaultBoolean.Default;
			this.usePrintAppearance = false;
			this.showHeaders = new DefaultBoolean[Enum.GetValues(typeof(PivotArea)).Length];
			for(int i = 0; i < showHeaders.Length; i++)
				this.showHeaders[i] = DefaultBoolean.Default;
			this.columnFieldValueSeparator = 0;
			this.rowFieldValueSeparator = 0;
			this.printHeadersOnEveryPage = false;
			this.mergeColumnFieldValues = true;
			this.mergeRowFieldValues = true;
			this.printUnusedFilterFields = true;
			this.pageSettings = new PivotGridPageSettings();
			this.verticalContentSplitting = VerticalContentSplitting.Smart;
			this.filterSeparatorBarPadding = -1;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintVerticalContentSplitting")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.VerticalContentSplitting")]
		[DefaultValue(VerticalContentSplitting.Smart), XtraSerializableProperty(), NotifyParentProperty(true)]
		public virtual VerticalContentSplitting VerticalContentSplitting { get { return verticalContentSplitting; } set { verticalContentSplitting = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintHorzLines")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHorzLines")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintHorzLines { get { return printHorzLines; } set { printHorzLines = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintVertLines")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintVertLines")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintVertLines { get { return printVertLines; } set { printVertLines = value; } }
		public DefaultBoolean GetPrintHeaders(PivotArea area) { return this.showHeaders[(int)area]; }
		protected void SetPrintHeaders(PivotArea area, DefaultBoolean value) {
			if(GetPrintHeaders(area) == value) return;
			this.showHeaders[(int)area] = value;
			OnOptionsChanged();
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintDataHeaders")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintDataHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintDataHeaders {
			get { return GetPrintHeaders(PivotArea.DataArea); }
			set { SetPrintHeaders(PivotArea.DataArea, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintFilterHeaders")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintFilterHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintFilterHeaders {
			get { return GetPrintHeaders(PivotArea.FilterArea); }
			set { SetPrintHeaders(PivotArea.FilterArea, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintColumnHeaders")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintColumnHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintColumnHeaders {
			get { return GetPrintHeaders(PivotArea.ColumnArea); }
			set { SetPrintHeaders(PivotArea.ColumnArea, value); }
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintRowHeaders")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintRowHeaders")]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), NotifyParentProperty(true)]
		public DefaultBoolean PrintRowHeaders {
			get { return GetPrintHeaders(PivotArea.RowArea); }
			set { SetPrintHeaders(PivotArea.RowArea, value); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintHeadersOnEveryPage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintHeadersOnEveryPage"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool PrintHeadersOnEveryPage {
			get { return printHeadersOnEveryPage; }
			set { printHeadersOnEveryPage = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintUsePrintAppearance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.UsePrintAppearance"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool UsePrintAppearance { get { return usePrintAppearance; } set { usePrintAppearance = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintColumnFieldValueSeparator")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.ColumnFieldValueSeparator")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int ColumnFieldValueSeparator {
			get { return columnFieldValueSeparator; }
			set {
				if(value < 0) value = 0;
				columnFieldValueSeparator = value;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintRowFieldValueSeparator")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.RowFieldValueSeparator")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int RowFieldValueSeparator {
			get { return rowFieldValueSeparator; }
			set {
				if(value < 0) value = 0;
				rowFieldValueSeparator = value;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintMergeColumnFieldValues"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeColumnFieldValues"),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool MergeColumnFieldValues {
			get { return mergeColumnFieldValues; }
			set { mergeColumnFieldValues = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintMergeRowFieldValues"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.MergeRowFieldValues"),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool MergeRowFieldValues {
			get { return mergeRowFieldValues; }
			set { mergeRowFieldValues = value; }
		}
		public bool IsMergeFieldValues(bool isColumn) {
			return isColumn ? MergeColumnFieldValues : MergeRowFieldValues;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPrintUnusedFilterFields"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PrintUnusedFilterFields"),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool PrintUnusedFilterFields { get { return printUnusedFilterFields; } set { printUnusedFilterFields = value; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintFilterSeparatorBarPadding")]
#endif
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.FilterSeparatorBarPadding")]
		[DefaultValue(-1), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int FilterSeparatorBarPadding {
			get { return filterSeparatorBarPadding; }
			set { filterSeparatorBarPadding = value; }
		}
		bool ShouldSerializePageSettings() { return !PageSettings.IsEmpty; }
		void ResetPageSettings() { PageSettings.Reset(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsPrintPageSettings"),
#endif
 XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridOptionsPrint.PageSettings")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PivotGridPageSettings PageSettings { get { return pageSettings; } }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			PivotGridOptionsPrint optionsPrint = options as PivotGridOptionsPrint;
			if(optionsPrint == null) return;
			this.ColumnFieldValueSeparator = optionsPrint.ColumnFieldValueSeparator;
			this.MergeColumnFieldValues = optionsPrint.MergeColumnFieldValues;
			this.MergeRowFieldValues = optionsPrint.MergeRowFieldValues;
			this.PrintColumnHeaders = optionsPrint.PrintColumnHeaders;
			this.PrintDataHeaders = optionsPrint.PrintDataHeaders;
			this.PrintFilterHeaders = optionsPrint.PrintFilterHeaders;
			this.PrintHeadersOnEveryPage = optionsPrint.PrintHeadersOnEveryPage;
			this.PrintHorzLines = optionsPrint.PrintHorzLines;
			this.PrintRowHeaders = optionsPrint.PrintRowHeaders;
			this.PrintUnusedFilterFields = optionsPrint.PrintUnusedFilterFields;
			this.PrintVertLines = optionsPrint.PrintVertLines;
			this.RowFieldValueSeparator = optionsPrint.RowFieldValueSeparator;
			this.UsePrintAppearance = optionsPrint.UsePrintAppearance;
			this.PageSettings.Assign(optionsPrint.PageSettings);
			this.VerticalContentSplitting = optionsPrint.VerticalContentSplitting;
			this.filterSeparatorBarPadding = optionsPrint.filterSeparatorBarPadding;
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PivotGridPageSettings {
		PaperKind paperKind;
		int paperWidth, paperHeight;
		bool landscape;
		Margins margins;
		static Margins DefaultMargins = new Margins(100, 100, 100, 100);
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridPageSettingsPaperKind")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperKind")]
		[DefaultValue(PaperKind.Letter), XtraSerializableProperty(), NotifyParentProperty(true)]
		[TypeConverter(typeof(PaperKindConverter))]
		public PaperKind PaperKind {
			get { return paperKind; }
			set {
				if(paperKind == value) return;
				paperKind = value;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridPageSettingsPaperWidth")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperWidth")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int PaperWidth {
			get { return paperWidth; }
			set {
				if(paperWidth == value) return;
				paperWidth = value;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridPageSettingsPaperHeight")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.PaperHeight")]
		[DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true)]
		public int PaperHeight {
			get { return paperHeight; }
			set {
				if(paperHeight == value) return;
				paperHeight = value;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridPageSettingsLandscape"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.Landscape"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool Landscape {
			get { return landscape; }
			set {
				if(landscape == value) return;
				landscape = value;
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridPageSettingsMargins")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.Data.PivotGridPageSettings.Margins")]
		[XtraSerializableProperty(), NotifyParentProperty(true)]
		public Margins Margins {
			get { return margins; }
			set {
				if(margins == value) return;
				margins = value;
			}
		}
		bool ShouldSerializeMargins() { return !margins.Equals(DefaultMargins); }
		void ResetMargins() { margins = (Margins)DefaultMargins.Clone(); }
		public PivotGridPageSettings()
			: base() {
			Reset();
		}
#if !DXPORTABLE
		public PageSettings ToPageSettings() {
			PageSettings pageSettings = new PageSettings();
			PaperSize paperSize = new PaperSize();
			paperSize.RawKind = (int)PaperKind;
			if(PaperKind == PaperKind.Custom) {
				paperSize.Width = PaperWidth;
				paperSize.Height = PaperHeight;
			}
			pageSettings.PaperSize = paperSize;
			pageSettings.Landscape = Landscape;
			pageSettings.Margins = (Margins)Margins.Clone();
			return pageSettings;
		}
#endif
		public void Assign(PivotGridPageSettings obj) {
			this.PaperWidth = obj.PaperWidth;
			this.PaperHeight = obj.PaperHeight;
			this.PaperKind = obj.PaperKind;
			this.Landscape = obj.Landscape;
			this.Margins = obj.Margins;
		}
		public override string ToString() {
			StringBuilder result = new StringBuilder(PaperKind.ToString());
			if(PaperKind == PaperKind.Custom)
				result.Append(", ").Append(PaperWidth).Append("x").Append(PaperHeight);
			if(Landscape)
				result.Append(", Landscape");
			result.Append(", ").Append(Margins);
			return result.ToString();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsEmpty {
			get {
				return Landscape == false && PaperKind == PaperKind.Letter && Margins.Equals(DefaultMargins) && PaperWidth == 0 && PaperHeight == 0;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			this.paperKind = PaperKind.Letter;
			this.margins = (Margins)DefaultMargins.Clone();
			this.paperWidth = 0;
			this.paperHeight = 0;
			this.landscape = false;
		}
	}
}
