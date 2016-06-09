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
using System.Windows.Controls;
using DevExpress.XtraScheduler;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using System.Windows;
using DevExpress.XtraScheduler.Services;
using DevExpress.Utils;
using DevExpress.Xpf.Scheduler.Drawing;
namespace DevExpress.Xpf.Scheduler.UI {
	public partial class GotoDateForm : UserControl {
		public static readonly DependencyProperty DateProperty;
		public static readonly DependencyProperty TargetViewTypeProperty;
		NamedElementList actualViews = new NamedElementList();
		SchedulerControl control;
		static GotoDateForm() {
			DateProperty = DependencyPropertyHelper.RegisterProperty<GotoDateForm, DateTime>("Date", DateTime.Today);
			TargetViewTypeProperty = DependencyPropertyHelper.RegisterProperty<GotoDateForm, SchedulerViewType>("TargetViewType", SchedulerViewType.Day);
		}
		public GotoDateForm(SchedulerControl control, SchedulerViewRepository views, DateTime date, SchedulerViewType viewType) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			Date = date;
			TargetViewType = viewType;
			PopulateActiveViews(views);
			Title = SchedulerControlLocalizer.GetString(SchedulerControlStringId.Caption_GotoDate);
			InitializeComponent();
		}
		#region Title
		public String Title {
			get { return (String)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		public static readonly DependencyProperty TitleProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<GotoDateForm, String>("Title", String.Empty);
		#endregion
		public DateTime Date {
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}
		public SchedulerViewType TargetViewType {
			get { return (SchedulerViewType)GetValue(TargetViewTypeProperty); }
			set { SetValue(TargetViewTypeProperty, value); }
		}
		public NamedElementList ActualViews { get { return actualViews; } }
		public SchedulerControl Control { get { return control; } }
		public void PopulateActiveViews(SchedulerViewRepository views) {
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewBase view = views[i];
				if (view.Enabled)
					ActualViews.Add(new NamedElement(view.Type, view.DisplayName));
			}
		}
		public void OnOKButtonClick(object sender, RoutedEventArgs e) {
			GoToDate(Date, TargetViewType);
		}
		public void GoToDate(DateTime date, SchedulerViewType viewType) {
			IDateTimeNavigationService service = (IDateTimeNavigationService)Control.GetService(typeof(IDateTimeNavigationService));
			if (service != null)
				service.GoToDate(date, viewType);
			CloseForm(true);
		}
		void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			CloseForm(false);
		}
		void CloseForm(bool dialogResult) {
			SchedulerFormBehavior.Close(this, dialogResult);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			LayoutUpdated += new EventHandler(OnLayoutUpdated);
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			LayoutUpdated -= new EventHandler(OnLayoutUpdated);
			Dispatcher.BeginInvoke((Action)(delegate() { this.edtGotoDate.Focus(); }));
		}
	}
}
