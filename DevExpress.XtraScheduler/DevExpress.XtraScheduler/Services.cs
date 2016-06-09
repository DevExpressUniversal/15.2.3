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
using System.Windows.Forms;
using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
namespace DevExpress.XtraScheduler.Services {
	#region SchedulerServices
	public class SchedulerServices : SchedulerServicesBase {
		protected internal SchedulerServices(InnerSchedulerControl control)
			: base(control) {
		}
		#region Properties
		public IMouseHandlerService MouseHandler { get { return (IMouseHandlerService)Control.GetService(typeof(IMouseHandlerService)); } }
		public IKeyboardHandlerService KeyboardHandler { get { return (IKeyboardHandlerService)Control.GetService(typeof(IKeyboardHandlerService)); } }
		public ISchedulerStateService SchedulerState { get { return (ISchedulerStateService)Control.GetService(typeof(ISchedulerStateService)); } }
		public ISchedulerPrintService PrintService { get { return Control.GetService<ISchedulerPrintService>(); } }
		#endregion
	}
	#endregion
	#region IHeaderCaptionService
	public interface IHeaderCaptionService {
		string GetDayColumnHeaderCaption(DayHeader header);
		string GetDayOfWeekHeaderCaption(DayOfWeekHeader header);
		string GetDayOfWeekAbbreviatedHeaderCaption(DayOfWeekHeader header);
		string GetHorizontalWeekCellHeaderCaption(SchedulerHeader header); 
		string GetVerticalWeekCellHeaderCaption(SchedulerHeader header); 
		string GetTimeScaleHeaderCaption(TimeScaleHeader header);
	}
	#endregion
	#region HeaderCaptionServiceWrapper
	public class HeaderCaptionServiceWrapper : IHeaderCaptionService {
		IHeaderCaptionService service;
		public HeaderCaptionServiceWrapper(IHeaderCaptionService service) {
			if (service == null)
				throw new ArgumentNullException("service", "service");
			this.service = service;
		}
		public IHeaderCaptionService Service { get { return service; } }
		#region IHeaderCaptionService Members
		public virtual string GetDayColumnHeaderCaption(DayHeader header) {
			return Service.GetDayColumnHeaderCaption(header);
		}
		public virtual string GetDayOfWeekHeaderCaption(DayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderCaption(header);
		}
		public virtual string GetDayOfWeekAbbreviatedHeaderCaption(DayOfWeekHeader header) {
			return Service.GetDayOfWeekAbbreviatedHeaderCaption(header);
		}
		public virtual string GetHorizontalWeekCellHeaderCaption(SchedulerHeader header) {
			return Service.GetHorizontalWeekCellHeaderCaption(header);
		}
		public virtual string GetVerticalWeekCellHeaderCaption(SchedulerHeader header) {
			return Service.GetVerticalWeekCellHeaderCaption(header);
		}
		public virtual string GetTimeScaleHeaderCaption(TimeScaleHeader header) {
			return Service.GetTimeScaleHeaderCaption(header);
		}
		#endregion
	}
	#endregion
	#region IHeaderToolTipService
	public interface IHeaderToolTipService {
		string GetDayColumnHeaderToolTip(DayHeader header);
		string GetDayOfWeekHeaderToolTip(DayOfWeekHeader header);
		string GetTimeScaleHeaderToolTip(TimeScaleHeader header);
	}
	#endregion
	#region HeaderToolTipServiceWrapper
	public class HeaderToolTipServiceWrapper : IHeaderToolTipService {
		IHeaderToolTipService service;
		public HeaderToolTipServiceWrapper(IHeaderToolTipService service) {
			if (service == null)
				throw new ArgumentNullException("service", "service");
			this.service = service;
		}
		public IHeaderToolTipService Service { get { return service; } }
		#region IHeaderToolTipService Members
		public virtual string GetDayColumnHeaderToolTip(DayHeader header) {
			return Service.GetDayColumnHeaderToolTip(header);
		}
		public virtual string GetDayOfWeekHeaderToolTip(DayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderToolTip(header);
		}
		public virtual string GetTimeScaleHeaderToolTip(TimeScaleHeader header) {
			return Service.GetTimeScaleHeaderToolTip(header);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Services.Implementation {
	#region WinSelectionService
	public class WinSelectionService : SelectionService {
		SchedulerControl schedulerControl;
		public WinSelectionService(SchedulerControl control)
			: base(control.InnerControl) {
			this.schedulerControl = control;
		}
		public SchedulerControl SchedulerControl { get { return schedulerControl; } }
		protected internal override SetSelectionCommand CreateSetSelectionCommand(TimeInterval interval, Resource resource) {
			return schedulerControl.ActiveView.CreateSetSelectionCommand(schedulerControl.InnerControl, interval, resource);
		}
	}
	#endregion
	#region HeaderCaptionService
	public class HeaderCaptionService : IHeaderCaptionService {
		#region IHeaderCaptionService Members
		public string GetDayColumnHeaderCaption(DayHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekHeaderCaption(DayOfWeekHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekAbbreviatedHeaderCaption(DayOfWeekHeader header) {
			return String.Empty;
		}
		public string GetHorizontalWeekCellHeaderCaption(SchedulerHeader header) {
			return String.Empty;
		}
		public string GetVerticalWeekCellHeaderCaption(SchedulerHeader header) {
			return String.Empty;
		}
		public string GetTimeScaleHeaderCaption(TimeScaleHeader header) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	#region HeaderToolTipService
	public class HeaderToolTipService : IHeaderToolTipService {
		#region IHeaderToolTipService Members
		public string GetDayColumnHeaderToolTip(DayHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekHeaderToolTip(DayOfWeekHeader header) {
			return String.Empty;
		}
		public string GetTimeScaleHeaderToolTip(TimeScaleHeader header) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	public class SchedulerPrintService : ISchedulerPrintService {
		SchedulerControl scheduler;
		public SchedulerPrintService(SchedulerControl scheduler) {
			this.scheduler = scheduler;
		}
		public void PageSetup() {
			this.scheduler.ShowPrintOptionsForm();
		}
		public void Print() {
			this.scheduler.Print();
		}
		public void PrintPreview() {
			this.scheduler.ShowPrintPreview();
		}
	}
}
namespace DevExpress.XtraScheduler.Services.Internal {
	public class HeaderCaptionFormatProvider : HeaderCaptionFormatProviderBase {
		IHeaderCaptionService service;
		public HeaderCaptionFormatProvider(IHeaderCaptionService service)
			: base() {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		internal IHeaderCaptionService Service { get { return service; } }
		public override string GetDayColumnHeaderCaption(IHeaderCaptionServiceObject header) {
			return Service.GetDayColumnHeaderCaption((DayHeader)header);
		}
		public override string GetDayOfWeekHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			return Service.GetDayOfWeekHeaderCaption((DayOfWeekHeader)header);
		}
		public override string GetHorizontalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			return Service.GetHorizontalWeekCellHeaderCaption((SchedulerHeader)header);
		}
		public override string GetVerticalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			return Service.GetVerticalWeekCellHeaderCaption((SchedulerHeader)header);
		}
		public override string GetTimeScaleHeaderCaption(ITimeScaleHeaderCaptionServiceObject header) {
			return Service.GetTimeScaleHeaderCaption((TimeScaleHeader)header);
		}
		public override string GetDayOfWeekAbbreviatedHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			return Service.GetDayOfWeekAbbreviatedHeaderCaption((DayOfWeekHeader)header);
		}
	}
}
