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
using DevExpress.XtraScheduler.Services;
using DevExpress.Xpf.Scheduler.Drawing;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
namespace DevExpress.Xpf.Scheduler.Services {
	public class SchedulerServices : SchedulerServicesBase {
		protected internal SchedulerServices(InnerSchedulerControl control)
			: base(control) {
		}
	}
	#region IHeaderCaptionService
	public interface IHeaderCaptionService {
		string GetDayColumnHeaderCaption(VisualDateHeader header);
		string GetDayOfWeekHeaderCaption(VisualDayOfWeekHeader header);
		string GetHorizontalWeekCellHeaderCaption(VisualDateCellHeader header); 
		string GetVerticalWeekCellHeaderCaption(VisualDateCellHeader header); 
		string GetTimeScaleHeaderCaption(VisualTimeScaleHeader header);
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
		public virtual string GetDayColumnHeaderCaption(VisualDateHeader header) {
			return Service.GetDayColumnHeaderCaption(header);
		}
		public virtual string GetDayOfWeekHeaderCaption(VisualDayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderCaption(header);
		}
		public virtual string GetHorizontalWeekCellHeaderCaption(VisualDateCellHeader header) {
			return Service.GetHorizontalWeekCellHeaderCaption(header);
		}
		public virtual string GetVerticalWeekCellHeaderCaption(VisualDateCellHeader header) {
			return Service.GetVerticalWeekCellHeaderCaption(header);
		}
		public virtual string GetTimeScaleHeaderCaption(VisualTimeScaleHeader header) {
			return Service.GetTimeScaleHeaderCaption(header);
		}
		#endregion
	}
	#endregion
	#region IHeaderToolTipService
	public interface IHeaderToolTipService {
		string GetDayColumnHeaderToolTip(VisualDateHeader header);
		string GetDayOfWeekHeaderToolTip(VisualDayOfWeekHeader header);
		string GetHorizontalWeekCellHeaderToolTip(VisualDateCellHeader header);
		string GetVerticalWeekCellHeaderToolTip(VisualDateCellHeader header);
		string GetTimeScaleHeaderToolTip(VisualTimeScaleHeader header);
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
		public virtual string GetDayColumnHeaderToolTip(VisualDateHeader header) {
			return Service.GetDayColumnHeaderToolTip(header);
		}
		public virtual string GetDayOfWeekHeaderToolTip(VisualDayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderToolTip(header);
		}
		public virtual string GetHorizontalWeekCellHeaderToolTip(VisualDateCellHeader header) {
			return Service.GetHorizontalWeekCellHeaderToolTip(header);
		}
		public virtual string GetVerticalWeekCellHeaderToolTip(VisualDateCellHeader header) {
			return Service.GetVerticalWeekCellHeaderToolTip(header);
		}
		public virtual string GetTimeScaleHeaderToolTip(VisualTimeScaleHeader header) {
			return Service.GetTimeScaleHeaderToolTip(header);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Xpf.Scheduler.Services.Internal {
	public class HeaderCaptionFormatProvider : HeaderCaptionFormatProviderBase {
		IHeaderCaptionService service;
		public HeaderCaptionFormatProvider(IHeaderCaptionService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		internal IHeaderCaptionService Service { get { return service; } }
		public override string GetDayColumnHeaderCaption(IHeaderCaptionServiceObject header) {
			VisualDateHeader VisualHeader = new VisualDateHeader(); 
			return Service.GetDayColumnHeaderCaption(VisualHeader);
		}
		public override string GetDayOfWeekHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			VisualDayOfWeekHeader VisualHeader = new VisualDayOfWeekHeader();  
			return Service.GetDayOfWeekHeaderCaption(VisualHeader);
		}
		public override string GetHorizontalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			VisualDateCellHeader VisualHeader = new VisualDateCellHeader(); 
			return Service.GetHorizontalWeekCellHeaderCaption(VisualHeader);
		}
		public override string GetVerticalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			VisualDateCellHeader VisualHeader = new VisualDateCellHeader(); 
			return Service.GetVerticalWeekCellHeaderCaption(VisualHeader);
		}
		public override string GetTimeScaleHeaderCaption(ITimeScaleHeaderCaptionServiceObject header) {
			VisualTimeScaleHeader VisualHeader = new VisualTimeScaleHeader(); 
			return Service.GetTimeScaleHeaderCaption(VisualHeader);
		}
		public override string GetDayOfWeekAbbreviatedHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			return String.Empty;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Services.Implementation {
	#region HeaderCaptionService
	public class HeaderCaptionService : IHeaderCaptionService {
		#region IHeaderCaptionService Members
		public string GetDayColumnHeaderCaption(DateHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekHeaderCaption(VisualDayOfWeekHeader ruler) {
			return String.Empty;
		}
		public string GetHorizontalWeekCellHeaderCaption(VisualDateCellHeader ruler) {
			return String.Empty;
		}
		public string GetVerticalWeekCellHeaderCaption(VisualDateCellHeader ruler) {
			return String.Empty;
		}
		public string GetTimeScaleHeaderCaption(VisualTimeScaleHeader ruler) {
			return String.Empty;
		}
		#endregion
		public string GetDayColumnHeaderCaption(VisualDateHeader header) {
			return string.Empty; 
		}
	}
	#endregion
}
