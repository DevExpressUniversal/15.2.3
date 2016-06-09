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

using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
namespace DevExpress.XtraPrinting {
	public abstract class HtmlExportOptionsBase : PageByPageExportOptionsBase {
		const int DefaultPageBorderWidth = 1;
		static readonly Color DefaultPageBorderColor = Color.Black;
		const bool DefaultEmbedImagesInHTML = false;
		const bool DefaultExportWatermarks = true;
		bool embedImagesInHTML;
		Color pageBorderColor = DefaultPageBorderColor;
		int pageBorderWidth = DefaultPageBorderWidth;
		const HtmlExportMode DefaultExportMode = HtmlExportMode.SingleFile;
		protected const string DefaultCharacterSet = "utf-8";
		protected const string DefaultTitle = "Document";
		HtmlExportMode exportMode = DefaultExportMode;
		string characterSet = DefaultCharacterSet;
		string title = DefaultTitle;
		bool removeSecondarySymbols;
		bool tableLayout = true;
		bool exportWatermarks = DefaultExportWatermarks;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBasePageBorderColor"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.PageBorderColor"),
		 TypeConverter(typeof(HtmlPageBorderColorConverter)),
		 XtraSerializableProperty,
		]
		public Color PageBorderColor { get { return pageBorderColor; } set { pageBorderColor = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBasePageBorderWidth"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.PageBorderWidth"),
		 DefaultValue(DefaultPageBorderWidth),
		 TypeConverter(typeof(HtmlPageBorderWidthConverter)),
		 XtraSerializableProperty,
		]
		public int PageBorderWidth { get { return pageBorderWidth; } set { pageBorderWidth = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseExportMode"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.ExportMode"),
		 DefaultValue(DefaultExportMode),
		 RefreshProperties(RefreshProperties.All),
		 XtraSerializableProperty,
		]
		public HtmlExportMode ExportMode { get { return exportMode; } set { exportMode = value; } }
		protected internal override bool IsMultiplePaged {
			get { return ExportMode == HtmlExportMode.DifferentFiles || ExportMode == HtmlExportMode.SingleFilePageByPage; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBasePageRange"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.PageRange"),
		 TypeConverter(typeof(HtmlPageRangeConverter)),
		 XtraSerializableProperty,
		]
		public override string PageRange { get { return base.PageRange; } set { base.PageRange = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseCharacterSet"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.CharacterSet"),
		 TypeConverter(typeof(CharSetConverter)),
		 DefaultValue(DefaultCharacterSet),
		 Localizable(true),
		 XtraSerializableProperty,
		]
		public string CharacterSet { get { return characterSet; } set { characterSet = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseTitle"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.Title"),
		 DefaultValue(DefaultTitle),
		 Localizable(true),
		 XtraSerializableProperty,
		]
		public string Title { get { return title; } set { title = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseRemoveSecondarySymbols"),
#endif
		 DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.RemoveSecondarySymbols"),
		 TypeConverter(typeof(BooleanTypeConverter)),
		 DefaultValue(false),
		 XtraSerializableProperty,
		]
		public bool RemoveSecondarySymbols { get { return removeSecondarySymbols; } set { removeSecondarySymbols = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseEmbedImagesInHTML"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptions.EmbedImagesInHTML"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(DefaultEmbedImagesInHTML),
		XtraSerializableProperty,
		]
		public virtual bool EmbedImagesInHTML { get { return embedImagesInHTML; } set { embedImagesInHTML = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseTableLayout"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.TableLayout"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(true),
		XtraSerializableProperty,
		]
		public bool TableLayout { get { return tableLayout; } set { tableLayout = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("HtmlExportOptionsBaseExportWatermarks"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraPrinting.HtmlExportOptionsBase.ExportWatermarks"),
		DefaultValue(DefaultExportWatermarks),
		TypeConverter(typeof(HtmlExportWatermarksConverter)),
		XtraSerializableProperty,
		]
		public bool ExportWatermarks { get { return exportWatermarks; } set { exportWatermarks = value; } }
		public HtmlExportOptionsBase() {
		}
		protected HtmlExportOptionsBase(HtmlExportOptionsBase source)
			: base(source) {
		}
		public override void Assign(ExportOptionsBase source) {
			base.Assign(source);
			HtmlExportOptionsBase htmlSource = (HtmlExportOptionsBase)source;
			exportMode = htmlSource.ExportMode;
			characterSet = htmlSource.CharacterSet;
			removeSecondarySymbols = htmlSource.RemoveSecondarySymbols;
			title = htmlSource.Title;
			pageBorderColor = htmlSource.PageBorderColor;
			pageBorderWidth = htmlSource.PageBorderWidth;
			embedImagesInHTML = htmlSource.EmbedImagesInHTML;
			tableLayout = htmlSource.TableLayout;
			exportWatermarks = htmlSource.exportWatermarks;
		}
		bool ShouldSerializePageBorderColor() {
			return PageBorderColor != DefaultPageBorderColor;
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializePageBorderColor() || pageBorderWidth != DefaultPageBorderWidth ||
				exportMode != DefaultExportMode || characterSet != DefaultCharacterSet || title != DefaultTitle ||
				removeSecondarySymbols != false || embedImagesInHTML != DefaultEmbedImagesInHTML || tableLayout != true || exportWatermarks != true ||
				base.ShouldSerialize();
		}
	}
}
