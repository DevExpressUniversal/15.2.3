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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Reports.UserDesigner.Extensions.Native;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Drawing.Printing;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	public class ReportManagerServiceViewModel : FrameworkElement, IReportStorage {
		internal ReportManagerService service;
		string previewId = Guid.NewGuid().ToString();
		public ReportManagerServiceViewModel(ReportManagerService service) {
			this.service = service;
			service.ReportsChanged += (s, e) => OnReportsUpdated();
			Reports = new ObservableCollection<ReportInfoViewModel>();
			ShowReportWizardCommand = new DelegateCommand(OnCreateAndShowReport);
			ShowReportPreviewCommand = new DelegateCommand<object>(OnPreview);
			EditReportCommand = new DelegateCommand<object>(OnEdit);
		}
		static ResourceDictionary resourceDictionary;
		internal static ResourceDictionary ResourceDictionary {
			get {
				if(resourceDictionary == null) {
					resourceDictionary = new ResourceDictionary();
					var relativePath = "/Extensions/ReportBehaviors/ReportServiceResources.xaml";
					var path = string.Format("pack://application:,,,/{0};component{1}", AssemblyInfo.SRAssemblyXpfReportDesigner, relativePath);
					resourceDictionary.Source = new Uri(path, UriKind.Absolute);
				}
				return resourceDictionary;
			}
		}
		void OnReportsUpdated() {
			IsPrintPreviewAvailable = service != null && service.HasPreview;
			AreThereAnyReports = service != null && service.GetReports().Any();
			Reports.Clear();
			foreach(var info in service.GetReports()) {
				Reports.Add(new ReportInfoViewModel(this, info));
			}
		}
		public ObservableCollection<ReportInfoViewModel> Reports { get; private set; }
		public ICommand ShowReportWizardCommand { get; private set; }
		public ICommand ShowReportPreviewCommand { get; private set; }
		public ICommand EditReportCommand { get; private set; }
		public bool IsPrintPreviewAvailable {
			get { return (bool)GetValue(IsPrintPreviewAvailableProperty); }
			set { SetValue(IsPrintPreviewAvailableProperty, value); }
		}
		public static readonly DependencyProperty IsPrintPreviewAvailableProperty =
			DependencyProperty.Register("IsPrintPreviewAvailable", typeof(bool), typeof(ReportManagerServiceViewModel), new PropertyMetadata(false));
		public bool AreThereAnyReports {
			get { return (bool)GetValue(AreThereAnyReportsProperty); }
			set { SetValue(AreThereAnyReportsProperty, value); }
		}
		public static readonly DependencyProperty AreThereAnyReportsProperty =
			DependencyProperty.Register("AreThereAnyReports", typeof(bool), typeof(ReportManagerServiceViewModel), new PropertyMetadata(false));
		void OnCreateAndShowReport() {
			var report = new XtraReport();
			var window = new DXDialogWindow();
			window.CommandsSource = UICommand.GenerateFromMessageButton(MessageButton.OKCancel, new DefaultMessageButtonLocalizer());
			var selector = new ReportThemeSelector();
			var themes = OfficeThemeRepository.Themes.Where(t => t.Name != "Retrospect").OrderBy(x => x.Name.Length).Select(t => new ThemeInfo { 
				Theme = t, Shades = OfficeThemesConverter.MakeShades(t.Colors)
			}).ToList();
			var themesViewModel = new ThemesViewModel { Themes = themes };
			selector.DataContext = themesViewModel;
			selector.SelectedTheme = themes.First();
			window.Content = selector;
			window.Width = 800;
			window.Height = 490;
			window.Title = PrintingLocalizer.GetString(PrintingStringId.ReportManagerServiceWizard_WindowTitle);
			window.ShowIcon = false;
			window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			window.ResizeMode = ResizeMode.NoResize;
			window.Owner = Window.GetWindow(service.AssociatedObject);
			var result = window.ShowDialogWindow();
			if(result == null || !Equals(result.Tag, MessageResult.OK))
				return;
			report.PaperKind = themesViewModel.PageSetupViewModel.SelectedPageFormat.Kind;
			report.ReportUnit = themesViewModel.PageSetupViewModel.SelectedUnit.Unit;
			report.Landscape = themesViewModel.PageSetupViewModel.IsHorizontal;
			service.GenerateReport(report);
			var styles = new ReportStyles {
				Header = report.StyleSheet.First(s => s.Name == "ReportHeaderBandStyle"),
				GroupHeader = report.StyleSheet.First(s => s.Name == "ReportGroupHeaderBandStyle"),
				Footer = report.StyleSheet.First(s => s.Name == "ReportFooterBandStyle"),
				GroupFooter = report.StyleSheet.First(s => s.Name == "ReportGroupFooterBandStyle"),
				DetailEven = report.StyleSheet.First(s => s.Name == "ReportEvenStyle"),
				DetailOdd = report.StyleSheet.First(s => s.Name == "ReportOddStyle")
			};
			OfficeThemesConverter.ConvertTheme(null, selector.SelectedTheme.Theme.Colors, selector.SelectedTheme.Theme.Fonts, styles);
			service.AssignDataSource(report);
			ShowReport(report, null, false);
		}
		void OnPreview(object parameter) {
			var info = parameter as ReportInfo;
			var report = parameter as XtraReport;
			ShowReport(report, info, true);
		}
		void OnEdit(object parameter) {
			var info = parameter as ReportInfo;
			var report = parameter as XtraReport;
			ShowReport(report, info, false);
		}
		internal void ShowReport(XtraReport report, ReportInfo info, bool forcePreview) {
			var reportDesigner = new ReportDesigner();
			reportDesigner.ReportStorage = this;
			bool isNew = info == null;
			if (info == null && report == null) {
				report = service.GenerateReport(null);
			}
			var document = isNew || info.IsReadOnly ?
				  reportDesigner.NewDocument(report)
				: reportDesigner.OpenDocument(info.Key);
			if(forcePreview || (info != null && info.IsReadOnly)) {
				document.ViewKind = ReportDesignerDocumentViewKind.Preview;
			}
			reportDesigner.ShowWindow(null);
		}
		internal string ShowReportNameDialog(Window owner, string template, string name, string title) {
			var window = new DXWindow();
			var vm = ViewModelSource.Create(() => new ReportNameDialogViewModel(() => window.Close()) { Name = name });
			window.Title = title;
			window.Content = new ContentPresenter {
				Content = vm,
				ContentTemplate = (DataTemplate)ResourceDictionary[template]
			};
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.ResizeMode = ResizeMode.NoResize;
			window.Owner = owner;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowDialog();
			if(!vm.IsOK) {
				return null;
			}
			return vm.Name.With(n => n.Trim());
		}
		string IReportStorage.GetErrorMessage(Exception exception) {
			return "error";
		}
		bool IReportStorage.CanCreateNew() {
			return false;
		}
		XtraReport IReportStorage.CreateNew() {
			throw new NotSupportedException();
		}
		bool IReportStorage.CanOpen() {
			return false;
		}
		string IReportStorage.Open(IReportDesignerUI designer) {
			throw new NotSupportedException();
		}
		XtraReport IReportStorage.CreateNewSubreport() {
			return new XtraReport();
		}
		XtraReport IReportStorage.Load(string reportID, IReportSerializer designerReportSerializer) {
			var info = service.GetReports().FirstOrDefault(i => i.Key == reportID);
			var report = info.With(service.GetReport);
			if (info != null && report != null) {
				report.DisplayName = info.Name;
			}
			report.Do(service.AssignDataSource);
			return report;
		}
		string IReportStorage.Save(string reportID, IReportProvider reportProvider, bool saveAs, string reportTitle, IReportDesignerUI designer) {
			var info = service.GetReports().FirstOrDefault(i => i.Key == reportID);
			if (info != null && !saveAs) {
				service.UpdateReport(info, reportProvider.GetReport());
			} else {
				var reportDesigner = (DependencyObject)designer;
				var name = ShowReportNameDialog(Window.GetWindow(reportDesigner), "RenameDialogTemplate", "",
					PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_SaveDialogCaption));
				if(name == null)
					return null;
				var report = reportProvider.GetReport(name);
				if(info == null) {
					info = service.SaveReport(report);
				} else {
					service.UpdateReport(info, report);
				}
				report.DisplayName = name;
				info.Name = name;
				info.IsReadOnly = false;
				service.UpdateReportInfo(info);
			}
			return info.Key;
		}
	}
	public class ThemesViewModel : ISupportWizard {
		public object Themes { get; set; }
		public PageSetupViewModel PageSetupViewModel { get; set; }
		public ThemesViewModel() {
			PageSetupViewModel = ViewModelSource.Create(() => new PageSetupViewModel());
		}
		void ISupportWizard.NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(PageSetupViewModel);
		}
	}
	public class PageFormat {
		public PageFormat(double ratio, string name, PaperKind kind) {
			Name = name;
			Kind = kind;
			Width = 300.0;
			Height = Width * ratio;
		}
		public double Width { get; set; }
		public double Height { get; set; }
		public string Name { get; set; }
		public PaperKind Kind { get; set; }
	}
	public class ReportUnitInfo {
		public string Name { get; set; }
		public ReportUnit Unit { get; set; }
	}
	public class PageSetupViewModel {
		public IEnumerable<PageFormat> PageFormats { get; private set; }
		public ReportUnitInfo[] Units { get; private set; }
		public ReportUnitInfo SelectedUnit { get; set; }
		public virtual PageFormat SelectedPageFormat { get; set; }
		public virtual bool IsHorizontal { get; set; }
		public virtual bool IsVertical { get; set; }
		public virtual double Width { get; set; }
		public virtual double Height { get; set; }
		protected void OnSelectedPageFormatChanged() {
			UpdatePreview();
		}
		protected void OnIsHorizontalChanged() {
			UpdatePreview();
		}
		protected void OnVerticalChanged() {
			UpdatePreview();
		}
		void UpdatePreview() {
			var w = SelectedPageFormat.Width;
			var h = SelectedPageFormat.Height;
			if(IsHorizontal) {
				Width = w;
				Height = h;
			} else {
				Width = h;
				Height = w;
			}
		}
		public PageSetupViewModel() {
			IsVertical = true;
			PageFormats = new[] {
				new PageFormat(105.0 / 148.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_A6), PaperKind.A6),
				new PageFormat(148.0 / 210.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_A5), PaperKind.A5),
				new PageFormat(210.0 / 297.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_A4), PaperKind.A4),
				new PageFormat(297.0 / 420.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_A3), PaperKind.A3),
				new PageFormat(8.5 / 11.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Letter), PaperKind.Letter),
				new PageFormat(8.5 / 14.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Legal), PaperKind.Legal),
				new PageFormat(250.0 / 353.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_B4), PaperKind.B4),
				new PageFormat(176.0 / 250.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_B5), PaperKind.B5),
				new PageFormat(324.0 / 458.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_C3Envelope), PaperKind.C3Envelope),
				new PageFormat(223.0 / 324.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_C4Envelope), PaperKind.C4Envelope),
				new PageFormat(10.0 / 11.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard10x11), PaperKind.Standard10x11),
				new PageFormat(10.0 / 14.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard10x14), PaperKind.Standard10x14),
				new PageFormat(11.0 / 17.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard11x17), PaperKind.Standard11x17),
				new PageFormat(12.0 / 11.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard12x11), PaperKind.Standard12x11),
				new PageFormat(15.0 / 11.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard15x11), PaperKind.Standard15x11),
				new PageFormat(9.0 / 11.0, PrintingLocalizer.GetString(PrintingStringId.PaperKind_Standard9x11), PaperKind.Standard9x11),
			};
			SelectedPageFormat = PageFormats.First(x => x.Kind == PaperKind.A4);
			Units = new[] {
				new ReportUnitInfo { Name = PrintingLocalizer.GetString(PrintingStringId.PageSetupInches), Unit = ReportUnit.HundredthsOfAnInch },
				new ReportUnitInfo { Name = PrintingLocalizer.GetString(PrintingStringId.PageSetupMillimeters), Unit = ReportUnit.TenthsOfAMillimeter },
			};
			SelectedUnit = Units.First();
		}
	}
}
