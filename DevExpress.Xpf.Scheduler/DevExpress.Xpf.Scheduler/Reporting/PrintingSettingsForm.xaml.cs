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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.XtraScheduler.Reporting;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Printing;
using DevExpress.XtraScheduler.Localization;
namespace DevExpress.Xpf.Scheduler.Reporting.UI {
	public partial class SchedulerPrintingSettingsForm : DXWindow {
			public SchedulerPrintingSettingsForm(ISchedulerPrintingSettingsFormViewModel printingSettings) {
			Guard.ArgumentNotNull(printingSettings, "viewModel");
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_PrintingSettings);
			InitializeResourcesKindSource();
			InitializeComponent();
			PrintingSettings = printingSettings;
			SetReportTemplateInfoSource();
			DataContext = this;
			PopulateResources();
		}
		protected virtual void SetReportTemplateInfoSource() {
			cbe_reportTemplate.ItemsSource = PrintingSettings.ReportTemplateInfoSource;
		}
		#region PrintingSettings
		public ISchedulerPrintingSettingsFormViewModel PrintingSettings {
			get { return (ISchedulerPrintingSettingsFormViewModel)GetValue(PrintingSettingsProperty); }
			set { SetValue(PrintingSettingsProperty, value); }
		}
		public static readonly DependencyProperty PrintingSettingsProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsForm, ISchedulerPrintingSettingsFormViewModel>("PrintingSettings", null, (d, e) => d.OnPrintingSettingsChanged(e.OldValue, e.NewValue));
		void OnPrintingSettingsChanged(ISchedulerPrintingSettingsFormViewModel oldValue, ISchedulerPrintingSettingsFormViewModel newValue) {
		}
		#endregion
		#region ResourcesKindSource
		public ObservableCollection<String> ResourcesKindSource {
			get { return (ObservableCollection<String>)GetValue(ResourcesKindSourceProperty); }
			set { SetValue(ResourcesKindSourceProperty, value); }
		}
		public static readonly DependencyProperty ResourcesKindSourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsForm, ObservableCollection<String>>("ResourcesKindSource", null);
		#endregion
#if SL
		#region ShowPrintPreview (event)
		public event EventHandler<ShowPrintPreviewEventArgs> ShowPrintPreview;
		protected void RaiseShowPrintPreview() {
			if (ShowPrintPreview == null)
				return;
			ShowPrintPreview(this, new ShowPrintPreviewEventArgs(PrintingSettings));
		}
		#endregion
#endif
#if !SL
		public event EventHandler PreviewButtonClick;
		protected void RaisePreviewButtonClick() {
			if (PreviewButtonClick != null)  
				PreviewButtonClick(this, EventArgs.Empty);
		}
#endif
		void InitializeResourcesKindSource() {
			ResourcesKindSource = new ObservableCollection<string>();
			ResourcesKindSource.Add(SchedulerLocalizer.GetString(SchedulerStringId.Caption_ResourceAll));
			ResourcesKindSource.Add(SchedulerLocalizer.GetString(SchedulerStringId.Caption_OnScreenResources));
		}
		void PopulateResources() {
			if (PrintingSettings.UseSpecificResources) {
				AddResources(lbe_AvailableResources, PrintingSettings.AvailableResources, PrintingSettings.PrintResources);
				AddResources(lbe_CustomResources, PrintingSettings.PrintResources);
			}
			else
				AddResources(lbe_AvailableResources, PrintingSettings.AvailableResources);
		}
		void UpdatePrintResources() {
			IList<XtraScheduler.Resource> resources = this.GetPrintResources();
			AddPrintResources(resources);
		}
		protected virtual void AddPrintResources(IList<XtraScheduler.Resource> resources) {
			PrintingSettings.PrintResources.Clear();
			foreach (XtraScheduler.Resource r in resources)
				this.PrintingSettings.PrintResources.Add(r);
		}
		protected virtual IList<XtraScheduler.Resource> GetPrintResources() {
			if ((bool)chk_PrintCustomCollection.IsChecked)
				return GetCustomResources();
			switch (cbe_ResourcesKind.SelectedIndex) {
				case 0:
					return PrintingSettings.AvailableResources;
				case 1:
					return PrintingSettings.OnScreenResources;
				default:
					return PrintingSettings.AvailableResources;
			}
		}
		protected virtual ResourceBaseCollection GetCustomResources() {
			return GetResources(lbe_CustomResources);
		}
		void AddResources(ListBoxEdit listBox, IList<XtraScheduler.Resource> resources) {
			listBox.Items.Clear();
			foreach (XtraScheduler.Resource resource in resources) {
				listBox.Items.Add(new NamedElement(resource, resource.Caption));
			}
		}
		void AddResources(ListBoxEdit listBox, IList<XtraScheduler.Resource> resources, IList<XtraScheduler.Resource> exclusions) {
			listBox.Items.Clear();
			foreach (XtraScheduler.Resource resource in resources) {
				if (exclusions.IndexOf(resource) < 0)
					listBox.Items.Add(new NamedElement(resource, resource.Caption));
			}
		}
		ResourceBaseCollection GetResources(ListBoxEdit listBox) {
			ResourceBaseCollection result = new ResourceBaseCollection();
			foreach (object item in listBox.Items) {
				NamedElement namedElement = (NamedElement)item;
				XtraScheduler.Resource resource = (XtraScheduler.Resource)(namedElement.Id);
				result.Add(resource);
			}
			return result;
		}
		void MoveAllItems(ListBoxEdit source, ListBoxEdit target) {
			int count = source.Items.Count;
			for (int i = 0; i < count; i++)
				target.Items.Add(source.Items[i]);
			source.Items.Clear();
		}
		void MoveSelectedItem(ListBoxEdit source, ListBoxEdit target) {
			if (source.SelectedIndex < 0)
				return;
			target.Items.Add(source.Items[source.SelectedIndex]);
			source.Items.RemoveAt(source.SelectedIndex);
		}
		void MoveSelectedItemUp(ListBoxEdit listBox) {
			int index = listBox.SelectedIndex;
			if (index <= 0)
				return;
			object item = listBox.Items[index];
			listBox.Items.RemoveAt(index);
			listBox.Items.Insert(index - 1, item);
		}
		void MoveSelectedItemDown(ListBoxEdit listBox) {
			int index = listBox.SelectedIndex;
			if ((index < 0) || (index >= listBox.Items.Count - 1))
				return;
			object item = listBox.Items[index];
			listBox.Items.RemoveAt(index);
			listBox.Items.Insert(index + 1, item);
		}
		void OnBtnToCustomCollectionClick(object sender, RoutedEventArgs e) {
			MoveSelectedItem(lbe_AvailableResources, lbe_CustomResources);
		}
		void OnBtnFromCustomCollectionClick(object sender, RoutedEventArgs e) {
			MoveSelectedItem(lbe_CustomResources, lbe_AvailableResources);
		}
		void OnBtnMoveUpClick(object sender, RoutedEventArgs e) {
			MoveSelectedItemUp(lbe_CustomResources);
		}
		void OnBtnMoveDownClick(object sender, RoutedEventArgs e) {
			MoveSelectedItemDown(lbe_CustomResources);
		}
		void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
			UpdatePrintResources();
		}
		void OnBtnCloseClick(object sender, RoutedEventArgs e) {
			Close();
		}
		void OnBtnPreviewClick(object sender, RoutedEventArgs e) {
			UpdatePrintResources();
#if SL
			RaiseShowPrintPreview();
#else
			RaisePreviewButtonClick();
			SchedulerReportConfigurator configurator = new SchedulerReportConfigurator();
			configurator.Configure(PrintingSettings);
			PrintHelper.ShowPrintPreviewDialog(this, PrintingSettings.ReportInstance);
#endif
		}
	}
}
