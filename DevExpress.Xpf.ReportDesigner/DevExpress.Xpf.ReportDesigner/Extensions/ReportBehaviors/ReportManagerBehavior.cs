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

using DevExpress.Images;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Ribbon;
using DevExpress.XtraReports.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
namespace DevExpress.Xpf.Reports.UserDesigner.Extensions {
	[TargetType(typeof(BarSplitButtonItem))]
	public class ReportManagerBehavior : Behavior<BarSplitButtonItem> {
		public ReportManagerService Service {
			get { return (ReportManagerService)GetValue(ServiceProperty); }
			set { SetValue(ServiceProperty, value); }
		}
		public static readonly DependencyProperty ServiceProperty =
			DependencyProperty.Register("Service", typeof(ReportManagerService), typeof(ReportManagerBehavior), new PropertyMetadata(null,
				(d, e) => ((ReportManagerBehavior)d).OnServiceChanged((ReportManagerService)e.OldValue, (ReportManagerService)e.NewValue)));
		event EventHandler ServiceChanged;
		protected override void OnAttached() {
			base.OnAttached();
			UpdateReports();
		}
		void UpdateReports() {
			if(AssociatedObject != null && Service != null) {
				PopulateDropDownList(AssociatedObject, vm => vm.ShowReportWizardCommand);
			}
		}
		void PopulateDropDownList(
			BarSplitButtonItem reportItem, 
			Func<ReportManagerServiceViewModel, ICommand> getCommand)
		{
			var listPresenter = new PopupMenu();
			var reports = Service.ViewModel.Reports;
			foreach(var report in reports) {
				var item = new BarSubItem {
					Content = report.Info.Name
				}; 
				item.Items.Add(new BarButtonItem {
					Content = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuPrint),
					Glyph = DXImageHelper.GetImageSource(DXImages.Print, ImageSize.Size16x16, ImageType.Colored),
					Command = report.PreviewCommand
				});
				item.Items.Add(new BarButtonItem {
					Content = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuEdit),
					Glyph = DXImageHelper.GetImageSource(DXImages.Edit, ImageSize.Size16x16, ImageType.Colored),
					Command = report.ShowCommand
				});
				if(!report.Info.IsReadOnly) {
					item.Items.Add(new BarButtonItem {
						Content = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuRename),
						Glyph = DXImageHelper.GetImageSource(DXImages.EditDataSource, ImageSize.Size16x16, ImageType.Colored),
						Command = report.RenameCommand
					});
					item.Items.Add(new BarButtonItem {
						Content = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuDelete),
						Glyph = DXImageHelper.GetImageSource(DXImages.Delete, ImageSize.Size16x16, ImageType.Colored),
						Command = report.DeleteCommand
					});
				}
				listPresenter.Items.Add(item);
			}
			if (reports.Any()) {
				listPresenter.Items.Add(new BarItemSeparator());
			}
			var barTemplate = (DataTemplate)ReportManagerServiceViewModel.ResourceDictionary["ReportBehaviorPrintDesignBarItemsTemplate"];
			if (Service.ViewModel.IsPrintPreviewAvailable) {
				listPresenter.Items.Add(new BarButtonItem {
					ContentTemplate = barTemplate,
					Content = new {
						Caption = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuPrintPreview),
						Description = "",
					},
					Glyph = DXImageHelper.GetImageSource(DXImages.Preview, ImageSize.Size16x16, ImageType.Colored),
					Command = Service.ViewModel.ShowReportPreviewCommand
				});
			}
			listPresenter.Items.Add(new BarButtonItem {
				ContentTemplate = barTemplate,
				Content = new {
					Caption = PrintingLocalizer.GetString(PrintingStringId.ReportBehavior_MenuDesignReport),
					Description = "",
				},
				Glyph = DXImageHelper.GetImageSource(DXImages.New, ImageSize.Size16x16, ImageType.Colored),
				Command = Service.ViewModel.ShowReportWizardCommand
			});
			reportItem.PopupControl = listPresenter;
		}
		protected override void OnDetaching() {
			if(Service != null) {
				Service.ReportsChanged -= OnReportsChanged;
			}
			base.OnDetaching();
		}
		void OnCreateReport() {
			Service.ViewModel.ShowReportWizardCommand.Execute(null);
		}
		void OnPrintPreview() {
			Service.ViewModel.ShowReportPreviewCommand.Execute(null);
		}
		void OnGalleryItemClicked(GalleryItem item) {
			var vm = (ReportInfoViewModel)item.DataContext;
			vm.ShowCommand.Execute(null);
		}
		void OnServiceChanged(ReportManagerService oldValue, ReportManagerService value) {
			if(oldValue != null) {
				value.ReportsChanged -= OnReportsChanged;
			}
			if(value != null) {
				if(ServiceChanged != null)
					ServiceChanged(this, EventArgs.Empty);
				value.ReportsChanged += OnReportsChanged;
			}
			UpdateReports();
		}
		void OnReportsChanged(object sender, EventArgs e) {
			UpdateReports();
		}
	}
}
