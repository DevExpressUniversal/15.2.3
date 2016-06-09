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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm;
using DevExpress.Xpf.Reports.UserDesigner.FieldList;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Reports.UserDesigner.ReportExplorer;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraReports.UI;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native.Metadata;
using DevExpress.Mvvm.DataAnnotations;
namespace DevExpress.Xpf.Reports.UserDesigner.PropertyGrid {
	public class ReportDesignerPropertyGridControl : Control {
		internal static void RegisterMetadata() {
			MetadataHelper.AddMetadata<XtraReportMetadata>();
			MetadataHelper.AddMetadata<XRControlMetadata>();
			MetadataHelper.AddMetadata<XRWatermarkMetadata>();
			MetadataHelper.AddMetadata<WatermarkMetadata>();
			MetadataHelper.AddMetadata<PageWatermarkMetadata>();
			MetadataHelper.AddMetadata<XtraReportBaseMetadata>();
			MetadataHelper.AddMetadata<XRChartMetadata>();
			MetadataHelper.AddMetadata<PdfExportOptionsMetadata>();
			MetadataHelper.AddMetadata<PageByPageExportOptionsBaseMetadata>();
			MetadataHelper.AddMetadata<SubreportBaseMetadata>();
			MetadataHelper.AddMetadata<HtmlExportOptionsBaseMetadata>();
			MetadataHelper.AddMetadata<ImageExportOptionsMetadata>();
			MetadataHelper.AddMetadata<BindingSettingsMetadata>();
			MetadataHelper.AddMetadata<XRLabelScriptsMetadata>();
			MetadataHelper.AddMetadata<XRControlEventsMetadata>();
			MetadataHelper.AddMetadata<GroupHeaderBandScriptsMetadata>();
			MetadataHelper.AddMetadata<GroupBandScriptsMetadata>();
			MetadataHelper.AddMetadata<XtraReportBaseScriptsMetadata>();
			MetadataHelper.AddMetadata<XtraReportScriptsMetadata>();
			MetadataHelper.AddMetadata<XRSummaryEventsMetadata>();
			MetadataHelper.AddMetadata<CalculatedFieldScriptsMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridScriptsMetadata>();
			MetadataHelper.AddMetadata<XRChartScriptsMetadata>();
			MetadataHelper.AddMetadata<BorderSideMetadata>();
			MetadataHelper.AddMetadata<EncodingMetadata>();
			MetadataHelper.AddMetadata<FontMetadata>();
			MetadataHelper.AddMetadata<PdfPasswordSecurityOptionsMetadata>();
			MetadataHelper.AddMetadata<FormattingRuleSheetMetadata>();
			MetadataHelper.AddMetadata<FormattingRuleMetadata>();
			MetadataHelper.AddMetadata<FormattingRuleCollectionMetadata>();
			MetadataHelper.AddMetadata<BindingDataMetadata>();
			MetadataHelper.AddMetadata<XRControlStyleSheetMetadata>();
			MetadataHelper.AddMetadata<XRControlStyleMetadata>();
			MetadataHelper.AddMetadata<SeriesCollectionMetadata>();
			MetadataHelper.AddMetadata<CalculatedFieldCollectionMetadata>();
			MetadataHelper.AddMetadata<CalculatedFieldMetadata>();
			MetadataHelper.AddMetadata<ParameterCollectionMetadata>();
			MetadataHelper.AddMetadata<LookUpSettingsMetadata>();
			MetadataHelper.AddMetadata<DynamicListLookUpSettingsMetadata>();
			MetadataHelper.AddMetadata<StaticListLookUpSettingsMetadata>();
			MetadataHelper.AddMetadata<ParameterBindingCollectionMetadata>();
			MetadataHelper.AddMetadata<ParameterBindingMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridCustomTotalCollectionMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridCustomTotalMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridFieldCollectionMetadata>();
			MetadataHelper.AddMetadata<ViewTypeMetadata>();
			MetadataHelper.AddMetadata<DataFilterCollectionMetadata>();
			MetadataHelper.AddMetadata<DataFilterMetadata>();
			MetadataHelper.AddMetadata<ChartTitleCollectionMetadata>();
			MetadataHelper.AddMetadata<ChartTitleMetadata>();
			MetadataHelper.AddMetadata<XRAnnotationCollectionMetadata>();
			MetadataHelper.AddMetadata<RecipientCollectionMetadata>();
			MetadataHelper.AddMetadata<RecipientMetadata>();
			MetadataHelper.AddMetadata<XRTableOfContentsLevelCollectionMetadata>();
			MetadataHelper.AddMetadata<GroupFieldCollectionMetadata>();
			MetadataHelper.AddMetadata<GroupFieldMetadata>();
			MetadataHelper.AddMetadata<SubBandCollectionMetadata>();
			MetadataHelper.AddMetadata<SubBandMetadata>();
			MetadataHelper.AddMetadata<ExportOptionsMetadata>();
			MetadataHelper.AddMetadata<XRTableOfContentsLevelMetadata>();
			MetadataHelper.AddMetadata<PrinterSettingsUsingMetadata>();
			MetadataHelper.AddMetadata<XlsxExportOptionsMetadata>();
			MetadataHelper.AddMetadata<TextExportOptionsMetadata>();
			MetadataHelper.AddMetadata<CsvExportOptionsMetadata>();
			MetadataHelper.AddMetadata<HtmlExportOptionsMetadata>();
			MetadataHelper.AddMetadata<MhtExportOptionsMetadata>();
			MetadataHelper.AddMetadata<RtfExportOptionsMetadata>();
			MetadataHelper.AddMetadata<NativeFormatOptionsMetadata>();
			MetadataHelper.AddMetadata<EmailOptionsMetadata>();
			MetadataHelper.AddMetadata<PrintPreviewOptionsMetadata>();
			MetadataHelper.AddMetadata<PdfDocumentOptionsMetadata>();
			MetadataHelper.AddMetadata<BarCodeGeneratorBaseMetadata>();
			MetadataHelper.AddMetadata<DesignerOptionsMetadata>();
			MetadataHelper.AddMetadata<ReportPrintOptionsMetadata>();
			MetadataHelper.AddMetadata<DesignerOptionsMetadata>();
			MetadataHelper.AddMetadata<XRSummaryMetadata>();
			MetadataHelper.AddMetadata<XRBindingCollectionMetadata>();
			MetadataHelper.AddMetadata<StylePriorityMetadata>();
			MetadataHelper.AddMetadata<XRControlStyleMetadata>();
			MetadataHelper.AddMetadata<XRControlScriptsMetadata>();
			MetadataHelper.AddMetadata<MultiColumnMetadata>();
			MetadataHelper.AddMetadata<BandScriptsMetadata>();
			MetadataHelper.AddMetadata<SubreportBaseScriptsMetadata>();
			MetadataHelper.AddMetadata<XRTableOfContentsTitleMetadata>();
			MetadataHelper.AddMetadata<XRControlXRControlStylesMetadata>();
			MetadataHelper.AddMetadata<PaddingInfoMetadata>();
			MetadataHelper.AddMetadata<MarginsMetadata>();
			MetadataHelper.AddMetadata<PointFloatMetadata>();
			MetadataHelper.AddMetadata<SizeFMetadata>();
			MetadataHelper.AddMetadata<SizeMetadata>();
			MetadataHelper.AddMetadata<ImageSourceMetadata>();
			MetadataHelper.AddMetadata<ColorMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridMetadata>();
			MetadataHelper.AddMetadata<XRPivotGridFieldMetadata>();
			MetadataHelper.AddMetadata<XRSparklineMetadata>();
			MetadataHelper.AddMetadata<RangeControlRangeMetadata>();
			MetadataHelper.AddMetadata<DataContainerComponentMetadata>();
			MetadataHelper.AddMetadata<XRRichTextMetadata>();
			MetadataHelper.AddMetadata<ShapeBaseViewModelMetadata>();
			MetadataHelper.AddMetadata<XRPictureBoxMetadata>();
			MetadataHelper.AddMetadata<CalculatedFieldMetadata>();
			MetadataHelper.AddMetadata<XRSubreportMetadata>();
			MetadataHelper.AddMetadata<XRShapeMetadata>();
		}
		public const string PageRangeMask = @"((\d+(-\d+?)?)(\,(\d+(-\d+?)?)?)*)?";
		public const string ListMask = @"([^;]+;)*";
		public static readonly DependencyProperty MessageBoxServiceTemplateProperty;
		static readonly Action<ReportDesignerPropertyGridControl, Action<IMessageBoxService>> messageBoxServiceAccessor;
		public static readonly DependencyProperty FieldListNodesProperty;
		public static readonly DependencyProperty DataSourcesProperty;
		public static readonly DependencyProperty ItemsProperty;
		public static readonly DependencyProperty SelectedItemProperty;
		public static readonly DependencyProperty SelectedObjectModelProperty;
		public static readonly RoutedEvent InstaledPrintersErrorEvent;
		public static readonly DependencyProperty ReportProperty;
		public static readonly DependencyProperty PrimarySelectionProperty;
		static ReportDesignerPropertyGridControl() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<ReportDesignerPropertyGridControl>.New()
				.RegisterServiceTemplateProperty(d => d.MessageBoxServiceTemplate, out MessageBoxServiceTemplateProperty, out messageBoxServiceAccessor)
				.Register(d => d.FieldListNodes, out FieldListNodesProperty, null)
				.Register(d => d.DataSources, out DataSourcesProperty, null)
				.Register(d => d.Items, out ItemsProperty, null)
				.Register(d => d.SelectedItem, out SelectedItemProperty, null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
				.Register(d => d.SelectedObjectModel, out SelectedObjectModelProperty, null)
				.RegisterAttached((FrameworkElement d) => GetReport(d), out ReportProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.RegisterAttached((FrameworkElement d) => GetPrimarySelection(d), out PrimarySelectionProperty, null, FrameworkPropertyMetadataOptions.Inherits)
				.OverrideDefaultStyleKey();
			InstaledPrintersErrorEvent = EventManager.RegisterRoutedEvent("InstaledPrintersError", RoutingStrategy.Bubble, typeof(EventHandler<ReportDesignerPropertyGridErrorEventArgs>), typeof(ReportDesignerPropertyGridControl));
		}
		public static XtraReport GetReport(FrameworkElement d) { return (XtraReport)d.GetValue(ReportProperty); }
		public static void SetReport(FrameworkElement d, XtraReport v) { d.SetValue(ReportProperty, v); }
		public static object GetPrimarySelection(FrameworkElement d) { return d.GetValue(ReportProperty); }
		public static void SetPrimarySelection(FrameworkElement d, object v) { d.SetValue(ReportProperty, v); }
		protected void DoWithMessageBoxService(Action<IMessageBoxService> action) { messageBoxServiceAccessor(this, action); }
		public DataTemplate MessageBoxServiceTemplate {
			get { return (DataTemplate)GetValue(MessageBoxServiceTemplateProperty); }
			set { SetValue(MessageBoxServiceTemplateProperty, value); }
		}
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> FieldListNodes {
			get { return (IEnumerable<FieldListNodeBase<XRDiagramControl>>)GetValue(FieldListNodesProperty); }
			set { SetValue(FieldListNodesProperty, value); }
		}
		public IEnumerable<FieldListNodeBase<XRDiagramControl>> DataSources {
			get { return (IEnumerable<FieldListNodeBase<XRDiagramControl>>)GetValue(DataSourcesProperty); }
			set { SetValue(DataSourcesProperty, value); }
		}
		public IEnumerable<IReportExplorerItem> Items {
			get { return (IEnumerable<IReportExplorerItem>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		public IReportExplorerItem SelectedItem {
			get { return (IReportExplorerItem)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public object SelectedObjectModel {
			get { return GetValue(SelectedObjectModelProperty); }
			set { SetValue(SelectedObjectModelProperty, value); }
		}
		IEnumerable<string> installedPrinters;
		public IEnumerable<string> InstalledPrinters {
			get {
				if(installedPrinters == null) {
					try {
						installedPrinters = GetInstalledPrinters();
					} catch (Win32Exception e) {
						installedPrinters = Enumerable.Empty<string>();
						var args = new ReportDesignerPropertyGridErrorEventArgs(e, InstaledPrintersErrorEvent, this);
						args.ErrorMessage = string.Format("The available printers could not be enumerated ({0}).", ExceptionHelper.GetInnerErrorMessage(e));
						RaiseEvent(args);
						if(args.Rethrow)
							throw;
						if(args.ShowErrorMessage)
							DoWithMessageBoxService(x => x.ShowMessage(args.ErrorMessage, "Error", MessageButton.OK, MessageIcon.Error));
					}
				}
				return installedPrinters;
			}
		}
		public event EventHandler<ReportDesignerPropertyGridErrorEventArgs> InstalledPrintersError {
			add { AddHandler(InstaledPrintersErrorEvent, value); }
			remove { RemoveHandler(InstaledPrintersErrorEvent, value); }
		}
		protected virtual IEnumerable<string> GetInstalledPrinters() {
			return System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
		}
	}
}
