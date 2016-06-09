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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.UI {
	#region ChartWizardItemBuilder
	public class ChartWizardItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new RunDesignerChartItem());
		}
	}
	#endregion
	#region ChartTemplatesItemBuilder
	public class ChartTemplatesItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new SaveAsTemplateChartItem());
			items.Add(new LoadTemplateChartItem());
		}
	}
	#endregion
	#region ChartPrintExportItemBuilder
	public class ChartPrintExportItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new PrintPreviewChartItem());
			items.Add(new PrintChartItem());
			CreateExportBaseItem createExportBaseItem = new CreateExportBaseItem();
			createExportBaseItem.AddBarItem(new ExportToPDFChartItem());
			createExportBaseItem.AddBarItem(new ExportToHTMLChartItem());
			createExportBaseItem.AddBarItem(new ExportToMHTChartItem());
			createExportBaseItem.AddBarItem(new ExportToXLSChartItem());
			createExportBaseItem.AddBarItem(new ExportToXLSXChartItem());
			createExportBaseItem.AddBarItem(new ExportToRTFChartItem());
			CreateExportToImageBaseItem createExportToImageBaseItem = new CreateExportToImageBaseItem();
			createExportToImageBaseItem.AddBarItem(new ExportToBMPChartItem());
			createExportToImageBaseItem.AddBarItem(new ExportToGIFChartItem());
			createExportToImageBaseItem.AddBarItem(new ExportToJPEGChartItem());
			createExportToImageBaseItem.AddBarItem(new ExportToPNGChartItem());
			createExportToImageBaseItem.AddBarItem(new ExportToTIFFChartItem());
			createExportBaseItem.AddBarItem(createExportToImageBaseItem);
			items.Add(createExportBaseItem);
		}
	}
	#endregion
	#region RunWizardChartItem
	public class RunWizardChartItem : ChartCommandBarButtonItem {
		public RunWizardChartItem()
			: base() {
		}
		public RunWizardChartItem(BarManager manager)
			: base(manager) {
		}
		public RunWizardChartItem(string caption)
			: base(caption) {
		}
		public RunWizardChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.RunWizard; } }
	}
	#endregion
	#region RunDesignerChartItem
	public class RunDesignerChartItem : ChartCommandBarButtonItem {
		public RunDesignerChartItem()
			: base() {
		}
		public RunDesignerChartItem(BarManager manager)
			: base(manager) {
		}
		public RunDesignerChartItem(string caption)
			: base(caption) {
		}
		public RunDesignerChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.RunDesigner; } }
	}
	#endregion
	#region SaveAsTemplateChartItem
	public class SaveAsTemplateChartItem : ChartCommandBarButtonItem {
		public SaveAsTemplateChartItem()
			: base() {
		}
		public SaveAsTemplateChartItem(BarManager manager)
			: base(manager) {
		}
		public SaveAsTemplateChartItem(string caption)
			: base(caption) {
		}
		public SaveAsTemplateChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.SaveAsTemplate; } }
	}
	#endregion
	#region LoadTemplateChartItem
	public class LoadTemplateChartItem : ChartCommandBarButtonItem {
		public LoadTemplateChartItem()
			: base() {
		}
		public LoadTemplateChartItem(BarManager manager)
			: base(manager) {
		}
		public LoadTemplateChartItem(string caption)
			: base(caption) {
		}
		public LoadTemplateChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.LoadTemplate; } }
	}
	#endregion
	#region PrintPreviewChartItem
	public class PrintPreviewChartItem : ChartCommandBarButtonItem {
		public PrintPreviewChartItem()
			: base() {
		}
		public PrintPreviewChartItem(BarManager manager)
			: base(manager) {
		}
		public PrintPreviewChartItem(string caption)
			: base(caption) {
		}
		public PrintPreviewChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.PrintPreview; } }
	}
	#endregion
	#region PrintChartItem
	public class PrintChartItem : ChartCommandBarButtonItem {
		public PrintChartItem()
			: base() {
		}
		public PrintChartItem(BarManager manager)
			: base(manager) {
		}
		public PrintChartItem(string caption)
			: base(caption) {
		}
		public PrintChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.Print; } }
	}
	#endregion
	#region CreateExportBaseItem
	public class CreateExportBaseItem : ChartCommandBarSubItem {
		public CreateExportBaseItem()
			: base() {
			this.MenuDrawMode = MenuDrawMode.SmallImagesText;
		}
		public CreateExportBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateExportBaseItem(string caption)
			: base(caption) {
		}
		public CreateExportBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportPlaceHolder; } }
	}
	#endregion
	#region ExportToPDFChartItem
	public class ExportToPDFChartItem : ChartCommandBarButtonItem {
		public ExportToPDFChartItem()
			: base() {
		}
		public ExportToPDFChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToPDFChartItem(string caption)
			: base(caption) {
		}
		public ExportToPDFChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToPDF; } }
	}
	#endregion
	#region ExportToHTMLChartItem
	public class ExportToHTMLChartItem : ChartCommandBarButtonItem {
		public ExportToHTMLChartItem()
			: base() {
		}
		public ExportToHTMLChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToHTMLChartItem(string caption)
			: base(caption) {
		}
		public ExportToHTMLChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToHTML; } }
	}
	#endregion
	#region ExportToMHTChartItem
	public class ExportToMHTChartItem : ChartCommandBarButtonItem {
		public ExportToMHTChartItem()
			: base() {
		}
		public ExportToMHTChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToMHTChartItem(string caption)
			: base(caption) {
		}
		public ExportToMHTChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToMHT; } }
	}
	#endregion
	#region ExportToXLSChartItem
	public class ExportToXLSChartItem : ChartCommandBarButtonItem {
		public ExportToXLSChartItem()
			: base() {
		}
		public ExportToXLSChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToXLSChartItem(string caption)
			: base(caption) {
		}
		public ExportToXLSChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToXLS; } }
	}
	#endregion
	#region ExportToXLSXChartItem
	public class ExportToXLSXChartItem : ChartCommandBarButtonItem {
		public ExportToXLSXChartItem()
			: base() {
		}
		public ExportToXLSXChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToXLSXChartItem(string caption)
			: base(caption) {
		}
		public ExportToXLSXChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToXLSX; } }
	}
	#endregion
	#region ExportToRTFChartItem
	public class ExportToRTFChartItem : ChartCommandBarButtonItem {
		public ExportToRTFChartItem()
			: base() {
		}
		public ExportToRTFChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToRTFChartItem(string caption)
			: base(caption) {
		}
		public ExportToRTFChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToRTF; } }
	}
	#endregion
	#region CreateExportToImageBaseItem
	public class CreateExportToImageBaseItem : ChartCommandBarSubItem {
		public CreateExportToImageBaseItem()
			: base() {
			this.MenuDrawMode = MenuDrawMode.SmallImagesText;
		}
		public CreateExportToImageBaseItem(BarManager manager)
			: base(manager) {
		}
		public CreateExportToImageBaseItem(string caption)
			: base(caption) {
		}
		public CreateExportToImageBaseItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToImagePlaceHolder; } }
	}
	#endregion
	#region ExportToBMPChartItem
	public class ExportToBMPChartItem : ChartCommandBarButtonItem {
		public ExportToBMPChartItem()
			: base() {
		}
		public ExportToBMPChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToBMPChartItem(string caption)
			: base(caption) {
		}
		public ExportToBMPChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToBMP; } }
	}
	#endregion
	#region ExportToGIFChartItem
	public class ExportToGIFChartItem : ChartCommandBarButtonItem {
		public ExportToGIFChartItem()
			: base() {
		}
		public ExportToGIFChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToGIFChartItem(string caption)
			: base(caption) {
		}
		public ExportToGIFChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToGIF; } }
	}
	#endregion
	#region ExportToJPEGChartItem
	public class ExportToJPEGChartItem : ChartCommandBarButtonItem {
		public ExportToJPEGChartItem()
			: base() {
		}
		public ExportToJPEGChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToJPEGChartItem(string caption)
			: base(caption) {
		}
		public ExportToJPEGChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToJPEG; } }
	}
	#endregion
	#region ExportToPNGChartItem
	public class ExportToPNGChartItem : ChartCommandBarButtonItem {
		public ExportToPNGChartItem()
			: base() {
		}
		public ExportToPNGChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToPNGChartItem(string caption)
			: base(caption) {
		}
		public ExportToPNGChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToPNG; } }
	}
	#endregion
	#region ExportToTIFFChartItem
	public class ExportToTIFFChartItem : ChartCommandBarButtonItem {
		public ExportToTIFFChartItem()
			: base() {
		}
		public ExportToTIFFChartItem(BarManager manager)
			: base(manager) {
		}
		public ExportToTIFFChartItem(string caption)
			: base(caption) {
		}
		public ExportToTIFFChartItem(BarManager manager, string caption)
			: base(manager, caption) {
		}
		protected override ChartCommandId CommandId { get { return ChartCommandId.ExportToTIFF; } }
	}
	#endregion
	#region CreateChartOtherRibbonPage
	public class CreateChartOtherRibbonPage : ControlCommandBasedRibbonPage {
		public CreateChartOtherRibbonPage() {
		}
		public CreateChartOtherRibbonPage(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonOtherPageCaption); } }
	}
	#endregion
	#region ChartWizardRibbonPageGroup
	public class ChartWizardRibbonPageGroup : ChartRibbonPageGroup {
		public ChartWizardRibbonPageGroup() {
		}
		public ChartWizardRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonDesignerGroupCaption); } }
	}
	#endregion
	#region ChartTemplatesRibbonPageGroup
	public class ChartTemplatesRibbonPageGroup : ChartRibbonPageGroup {
		public ChartTemplatesRibbonPageGroup() {
		}
		public ChartTemplatesRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonTemplatesGroupCaption); } }
	}
	#endregion
	#region ChartPrintExportRibbonPageGroup
	public class ChartPrintExportRibbonPageGroup : ChartRibbonPageGroup {
		public ChartPrintExportRibbonPageGroup() {
		}
		public ChartPrintExportRibbonPageGroup(string text)
			: base(text) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonPrintExportGroupCaption); } }
	}
	#endregion
	#region ChartWizardBar
	public class ChartWizardBar : ControlCommandBasedBar<IChartContainer, ChartCommandId> {
		public ChartWizardBar() {
		}
		public ChartWizardBar(BarManager manager)
			: base(manager) {
		}
		public ChartWizardBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonDesignerGroupCaption); } }
	}
	#endregion
	#region ChartTemplatesBar
	public class ChartTemplatesBar : ControlCommandBasedBar<IChartContainer, ChartCommandId> {
		public ChartTemplatesBar() {
		}
		public ChartTemplatesBar(BarManager manager)
			: base(manager) {
		}
		public ChartTemplatesBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonTemplatesGroupCaption); } }
	}
	#endregion
	#region ChartPrintExportBar
	public class ChartPrintExportBar : ControlCommandBasedBar<IChartContainer, ChartCommandId> {
		public ChartPrintExportBar() {
		}
		public ChartPrintExportBar(BarManager manager)
			: base(manager) {
		}
		public ChartPrintExportBar(BarManager manager, string name)
			: base(manager, name) {
		}
		public override string DefaultText { get { return ChartLocalizer.GetString(ChartStringId.RibbonPrintExportGroupCaption); } }
	}
	#endregion
	#region ChartWizardBarCreator
	public class ChartWizardBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(CreateChartOtherRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartWizardRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartWizardBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 0; } }
		public override Bar CreateBar() {
			return new ChartWizardBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartWizardItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new CreateChartOtherRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartWizardRibbonPageGroup();
		}
	}
	#endregion
	#region ChartTemplatesBarCreator
	public class ChartTemplatesBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(CreateChartOtherRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartTemplatesRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartTemplatesBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 1; } }
		public override Bar CreateBar() {
			return new ChartTemplatesBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartTemplatesItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new CreateChartOtherRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartTemplatesRibbonPageGroup();
		}
	}
	#endregion
	#region ChartPrintExportBarCreator
	public class ChartPrintExportBarCreator : ControlCommandBarCreator {
		public override Type SupportedRibbonPageType { get { return typeof(CreateChartOtherRibbonPage); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartPrintExportRibbonPageGroup); } }
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(ChartPrintExportBar); } }
		public override int DockRow { get { return 1; } }
		public override int DockColumn { get { return 2; } }
		public override Bar CreateBar() {
			return new ChartPrintExportBar();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartPrintExportItemBuilder();
		}
		public override CommandBasedRibbonPage CreateRibbonPageInstance() {
			return new CreateChartOtherRibbonPage();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartPrintExportRibbonPageGroup();
		}
	}
	#endregion
}
