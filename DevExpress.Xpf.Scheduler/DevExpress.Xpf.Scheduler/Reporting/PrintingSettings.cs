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
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Reporting;
using DevExpress.Xpf.Scheduler.Reporting.Native;
namespace DevExpress.Xpf.Scheduler.Reporting {
	#region ISchedulerReportPrintingSettings
	public interface ISchedulerPrintingSettings {
#if !SL
		ISchedulerReport ReportInstance { get; }
		DXSchedulerPrintAdapter SchedulerPrintAdapter { get; }
#endif
		string GetReportTemplatePath();
	}
	#endregion
	#region BaseSchedulerPrintingSettings
	public abstract class BaseSchedulerPrintingSettings : DependencyObject, ISchedulerPrintingSettings {
#if !SL
		#region ReportInstance
		public ISchedulerReport ReportInstance {
			get { return (ISchedulerReport)GetValue(ReportInstanceProperty); }
			set { SetValue(ReportInstanceProperty, value); }
		}
		public static readonly DependencyProperty ReportInstanceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<BaseSchedulerPrintingSettings, ISchedulerReport>("ReportInstance", null);
		#endregion
		#region SchedulerPrintAdapter
		public DXSchedulerPrintAdapter SchedulerPrintAdapter {
			get { return (DXSchedulerPrintAdapter)GetValue(SchedulerPrintAdapterProperty); }
			set { SetValue(SchedulerPrintAdapterProperty, value); }
		}
		public static readonly DependencyProperty SchedulerPrintAdapterProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<BaseSchedulerPrintingSettings, DXSchedulerPrintAdapter>("SchedulerPrintAdapter", null);
		#endregion
#endif
		public abstract string GetReportTemplatePath();
	}
	#endregion
	#region SchedulerPrintingSettings
	public class SchedulerPrintingSettings : BaseSchedulerPrintingSettings, ISchedulerPrintingSettings {
		#region ReportTemplatePath
		public string ReportTemplatePath {
			get { return (string)GetValue(ReportTemplatePathProperty); }
			set { SetValue(ReportTemplatePathProperty, value); }
		}
		public static readonly DependencyProperty ReportTemplatePathProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettings, string>("ReportTemplatePath", string.Empty);
		#endregion
		public override string GetReportTemplatePath() { return ReportTemplatePath; }
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Reporting.UI {
	#region ISchedulerPrintingSettingsFormViewModel
	public interface ISchedulerPrintingSettingsFormViewModel : ISchedulerPrintingSettings {
		TimeInterval TimeInterval { get; set; }
		IList<ISchedulerReportTemplateInfo> ReportTemplateInfoSource { get; }
		int ActiveReportTemplateIndex { get; set;}
		bool UseSpecificResources { get; }
		IList<XtraScheduler.Resource> AvailableResources { get; }
		IList<XtraScheduler.Resource> OnScreenResources { get; }
		IList<XtraScheduler.Resource> PrintResources { get; }
	}
	#endregion
	#region ISchedulerReportTemplateInfo
	public interface ISchedulerReportTemplateInfo {
		string DisplayName { get; }
		string ReportTemplatePath { get; }
	}
	#endregion
	#region SchedulerPrintingSettings
	public class SchedulerPrintingSettingsFormViewModel : BaseSchedulerPrintingSettings, ISchedulerPrintingSettingsFormViewModel {
		public SchedulerPrintingSettingsFormViewModel() {
			PrintResources = new ResourceBaseCollection();
			AvailableResources = new ResourceBaseCollection();
			OnScreenResources = new ResourceBaseCollection();
		}
		#region Properties
		#region ReportTemplateInfoSource
		public IList<ISchedulerReportTemplateInfo> ReportTemplateInfoSource {
			get { return (IList<ISchedulerReportTemplateInfo>)GetValue(ReportTemplateInfoSourceProperty); }
			set { SetValue(ReportTemplateInfoSourceProperty, value); }
		}
		public static readonly DependencyProperty ReportTemplateInfoSourceProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, IList<ISchedulerReportTemplateInfo>>("ReportTemplateInfoSource", null);
		#endregion
		#region ActiveReportTemplateIndex
		public int ActiveReportTemplateIndex {
			get { return (int)GetValue(ActiveReportTemplateIndexProperty); }
			set { SetValue(ActiveReportTemplateIndexProperty, value); }
		}
		public static readonly DependencyProperty ActiveReportTemplateIndexProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, int>("ActiveReportTemplateIndex", -1);
		#endregion
		#region TimeInterval
		public TimeInterval TimeInterval {
			get { return (TimeInterval)GetValue(TimeIntervalProperty); }
			set { SetValue(TimeIntervalProperty, value); }
		}
		public static readonly DependencyProperty TimeIntervalProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, TimeInterval>("TimeInterval", TimeInterval.Empty);
		#endregion
		#region AvailableResources
		public ResourceBaseCollection AvailableResources {
			get { return (ResourceBaseCollection)GetValue(AvailableResourcesProperty); }
			set { SetValue(AvailableResourcesProperty, value); }
		}
		public static readonly DependencyProperty AvailableResourcesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, ResourceBaseCollection>("AvailableResources", null);
		#endregion
		#region OnScreenResources
		public ResourceBaseCollection OnScreenResources {
			get { return (ResourceBaseCollection)GetValue(OnScreenResourcesProperty); }
			set { SetValue(OnScreenResourcesProperty, value); }
		}
		public static readonly DependencyProperty OnScreenResourcesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, ResourceBaseCollection>("OnScreenResources", null);
		#endregion
		#region PrintResources
		public ResourceBaseCollection PrintResources {
			get { return (ResourceBaseCollection)GetValue(PrintResourcesProperty); }
			set { SetValue(PrintResourcesProperty, value); }
		}
		public static readonly DependencyProperty PrintResourcesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, ResourceBaseCollection>("PrintResources", null);
		#endregion
		#region UseSpecificResources
		public bool UseSpecificResources {
			get { return (bool)GetValue(UseSpecificResourcesProperty); }
			set { SetValue(UseSpecificResourcesProperty, value); }
		}
		public static readonly DependencyProperty UseSpecificResourcesProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, bool>("UseSpecificResources", false);
		#endregion
		#region ResourcesKindIndex
		public int ResourcesKindIndex {
			get { return (int)GetValue(ResourcesKindIndexProperty); }
			set { SetValue(ResourcesKindIndexProperty, value); }
		}
		public static readonly DependencyProperty ResourcesKindIndexProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPrintingSettingsFormViewModel, int>("ResourcesKindIndex", 0);
		#endregion
		#endregion
		protected virtual ISchedulerReportTemplateInfo GetActiveReportTemplateInfo() {
			if (ActiveReportTemplateIndex < 0 || ActiveReportTemplateIndex >= ReportTemplateInfoSource.Count)
				return null;
			return ReportTemplateInfoSource[ActiveReportTemplateIndex];
		}
		public override string GetReportTemplatePath() {
			ISchedulerReportTemplateInfo info = GetActiveReportTemplateInfo();
			return info != null ? info.ReportTemplatePath : string.Empty;
		}
		#region ISchedulerPrintingSettingsFormViewModel implementation
		IList<XtraScheduler.Resource> ISchedulerPrintingSettingsFormViewModel.AvailableResources {
			get { return AvailableResources; }
		}
		IList<XtraScheduler.Resource> ISchedulerPrintingSettingsFormViewModel.OnScreenResources {
			get { return OnScreenResources; }
		}
		IList<XtraScheduler.Resource> ISchedulerPrintingSettingsFormViewModel.PrintResources {
			get { return PrintResources; }
		}
		#endregion
	}
	#endregion
}
