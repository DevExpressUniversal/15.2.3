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
using DevExpress.XtraScheduler.Native;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Internal;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler.Services.Internal;
namespace DevExpress.Web.ASPxScheduler.Services {
	#region ASPxSchedulerServices
	public class ASPxSchedulerServices : SchedulerServicesBase {
		protected internal ASPxSchedulerServices(InnerSchedulerControl control)
			: base(control) {
		}
	}
	#endregion
	#region IHeaderCaptionService
	public interface IHeaderCaptionService {
		string GetDayColumnHeaderCaption(WebDateHeader header);
		string GetDayOfWeekHeaderCaption(WebDayOfWeekHeader header);
		string GetHorizontalWeekCellHeaderCaption(WebDateCellHeader header); 
		string GetVerticalWeekCellHeaderCaption(WebDateCellHeader header); 
		string GetTimeScaleHeaderCaption(WebTimeScaleHeader header);
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
		public virtual string GetDayColumnHeaderCaption(WebDateHeader header) {
			return Service.GetDayColumnHeaderCaption(header);
		}
		public virtual string GetDayOfWeekHeaderCaption(WebDayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderCaption(header);
		}
		public virtual string GetHorizontalWeekCellHeaderCaption(WebDateCellHeader header) {
			return Service.GetHorizontalWeekCellHeaderCaption(header);
		}
		public virtual string GetVerticalWeekCellHeaderCaption(WebDateCellHeader header) {
			return Service.GetVerticalWeekCellHeaderCaption(header);
		}
		public virtual string GetTimeScaleHeaderCaption(WebTimeScaleHeader header) {
			return Service.GetTimeScaleHeaderCaption(header);
		}
		#endregion
	}
	#endregion
	#region IHeaderToolTipService
	public interface IHeaderToolTipService {
		string GetDayColumnHeaderToolTip(WebDateHeader header);
		string GetDayOfWeekHeaderToolTip(WebDayOfWeekHeader header);
		string GetHorizontalWeekCellHeaderToolTip(WebDateCellHeader header);
		string GetVerticalWeekCellHeaderToolTip(WebDateCellHeader header); 
		string GetTimeScaleHeaderToolTip(WebTimeScaleHeader header);
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
		public virtual string GetDayColumnHeaderToolTip(WebDateHeader header) {
			return Service.GetDayColumnHeaderToolTip(header);
		}
		public virtual string GetDayOfWeekHeaderToolTip(WebDayOfWeekHeader header) {
			return Service.GetDayOfWeekHeaderToolTip(header);
		}
		public virtual string GetHorizontalWeekCellHeaderToolTip(WebDateCellHeader header) {
			return Service.GetHorizontalWeekCellHeaderToolTip(header);
		}
		public virtual string GetVerticalWeekCellHeaderToolTip(WebDateCellHeader header) {
			return Service.GetVerticalWeekCellHeaderToolTip(header);
		}
		public virtual string GetTimeScaleHeaderToolTip(WebTimeScaleHeader header) {
			return Service.GetTimeScaleHeaderToolTip(header);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Services.Implementation {
	#region HeaderCaptionService
	public class HeaderCaptionService : IHeaderCaptionService {
		#region IHeaderCaptionService Members
		public string GetDayColumnHeaderCaption(WebDateHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekHeaderCaption(WebDayOfWeekHeader ruler) {
			return String.Empty;
		}
		public string GetHorizontalWeekCellHeaderCaption(WebDateCellHeader ruler) {
			return String.Empty;
		}
		public string GetVerticalWeekCellHeaderCaption(WebDateCellHeader ruler) {
			return String.Empty;
		}
		public string GetTimeScaleHeaderCaption(WebTimeScaleHeader ruler) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	#region HeaderToolTipService
	public class HeaderToolTipService : IHeaderToolTipService {
		#region IHeaderToolTipService Members
		public string GetDayColumnHeaderToolTip(WebDateHeader header) {
			return String.Empty;
		}
		public string GetDayOfWeekHeaderToolTip(WebDayOfWeekHeader ruler) {
			return String.Empty;
		}
		public string GetTimeScaleHeaderToolTip(WebTimeScaleHeader ruler) {
			return String.Empty;
		}
		public string GetHorizontalWeekCellHeaderToolTip(WebDateCellHeader header) {
			return String.Empty;
		}
		public string GetVerticalWeekCellHeaderToolTip(WebDateCellHeader header) {
			return String.Empty;
		}
		#endregion
	}
	#endregion
	public class DefaultSupportXpCollectionService : ISupportCollectionDataSourceService {
		public bool IsSupported(System.Collections.IEnumerable data) {
			Type xpBaseCollectionType = GetXpBaseCollectionType();
			if (xpBaseCollectionType == null)
				return false;
			return xpBaseCollectionType.IsAssignableFrom(data.GetType());
		}
		#region GetXpBaseCollectionType
		internal static Type xpBaseCollectionType = typeof(object);
		internal virtual Type GetXpBaseCollectionType() {
			lock (xpBaseCollectionType) {
				if (xpBaseCollectionType == typeof(object)) {
					string name = this.GetType().AssemblyQualifiedName;
					string tokenSearchString = "PublicKeyToken=";
					int tokenPos = name.IndexOf(tokenSearchString);
					if (tokenPos >= 0) {
						string token = name.Substring(tokenPos + tokenSearchString.Length);
						string fullQualifiedName = "DevExpress.Xpo.XPBaseCollection, " + AssemblyInfo.SRAssemblyXpo + ", Version=" + AssemblyInfo.Version + ", Culture=neutral, PublicKeyToken=" + token;
						xpBaseCollectionType = Type.GetType(fullQualifiedName, false);
						if (xpBaseCollectionType == null) {
							xpBaseCollectionType = typeof(object);
							return null;
						}
						else
							return xpBaseCollectionType;
					}
					else
						return null;
				}
				else
					return xpBaseCollectionType;
			}
		}
		#endregion
	}
}
namespace DevExpress.Web.ASPxScheduler.Services.Internal {
	public class HeaderCaptionFormatProvider : HeaderCaptionFormatProviderBase {
		IHeaderCaptionService service;
		public HeaderCaptionFormatProvider(IHeaderCaptionService service) {
			Guard.ArgumentNotNull(service, "service");
			this.service = service;
		}
		internal IHeaderCaptionService Service { get { return service; } }
		public override string GetDayColumnHeaderCaption(IHeaderCaptionServiceObject header) {
			WebDateHeader webHeader = new WebDateHeader(header.Interval, header.Resource);
			return Service.GetDayColumnHeaderCaption(webHeader);
		}
		public override string GetDayOfWeekHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			WebDayOfWeekHeader webHeader = new WebDayOfWeekHeader(header.DayOfWeek, header.Resource);
			return Service.GetDayOfWeekHeaderCaption(webHeader);
		}
		public override string GetHorizontalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			WebDateCellHeader webHeader = new WebDateCellHeader(header.Interval, header.Resource, false);
			return Service.GetHorizontalWeekCellHeaderCaption(webHeader);
		}
		public override string GetVerticalWeekCellHeaderCaption(IHeaderCaptionServiceObject header) {
			WebDateCellHeader webHeader = new WebDateCellHeader(header.Interval, header.Resource, true);
			return Service.GetVerticalWeekCellHeaderCaption(webHeader);
		}
		public override string GetTimeScaleHeaderCaption(ITimeScaleHeaderCaptionServiceObject header) {
			WebTimeScaleHeader webHeader = new WebTimeScaleHeader(header.Interval, header.Scale);
			return Service.GetTimeScaleHeaderCaption(webHeader);
		}
		public override string GetDayOfWeekAbbreviatedHeaderCaption(IDayOfWeekHeaderCaptionServiceObject header) {
			return String.Empty;
		}
	}
	public interface ISchedulerScriptService {
		string GetShowGotoDateCalendarAction();
	}
	public interface ISupportCollectionDataSourceService {
		bool IsSupported(System.Collections.IEnumerable data);
	}
}
