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
using DevExpress.XtraScheduler.Services.Internal;
namespace DevExpress.XtraScheduler.Services {
	#region ISchedulerStateService
	public interface ISchedulerStateService {
		bool IsModalFormOpened { get; }
		bool IsInplaceEditorOpened { get; }
		bool IsAppointmentResized { get; }
		bool IsPopupMenuOpened { get; }
		bool AreAppointmentsDragged { get; }
		bool IsDataRefreshAllowed { get; }
		bool IsAnimationActive { get; }
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Services.Internal {
	#region ISetSchedulerStateService
	public interface ISetSchedulerStateService {
		bool IsModalFormOpened { get; set; }
		bool IsInplaceEditorOpened { get; set; }
		bool IsAppointmentResized { get; set; }
		bool IsPopupMenuOpened { get; set; }
		bool AreAppointmentsDragged { get; set; }
		bool IsAnimationActive { get; set; }
		bool IsApplyChanges { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Services.Implementation {
	#region SchedulerStateService
	public class SchedulerStateService : ISchedulerStateService, ISetSchedulerStateService {
		#region Fields
		bool isModalFormOpened;
		bool isInplaceEditorOpened;
		bool isAppointmentResized;
		bool isPopupMenuOpened;
		bool areAppointmentsDragged;
		bool isAnimationActive;
		bool isApplyChanges;
		#endregion
		#region ISchedulerStateService Members
		bool ISchedulerStateService.IsModalFormOpened { get { return isModalFormOpened; } }
		bool ISchedulerStateService.IsInplaceEditorOpened { get { return isInplaceEditorOpened; } }
		bool ISchedulerStateService.IsAppointmentResized { get { return isAppointmentResized; } }
		bool ISchedulerStateService.IsPopupMenuOpened { get { return isPopupMenuOpened; } }
		bool ISchedulerStateService.AreAppointmentsDragged { get { return areAppointmentsDragged; } }
		bool ISchedulerStateService.IsDataRefreshAllowed {
			get {
				if(isModalFormOpened || isInplaceEditorOpened || isAppointmentResized || isPopupMenuOpened || areAppointmentsDragged || isApplyChanges)
					return false;
				else
					return true;
			}
		}
		bool ISchedulerStateService.IsAnimationActive { get { return isAnimationActive; } }
		#endregion
		#region ISetSchedulerStateService Members
		bool ISetSchedulerStateService.IsModalFormOpened { get { return isModalFormOpened; } set { isModalFormOpened = value; } }
		bool ISetSchedulerStateService.IsInplaceEditorOpened { get { return isInplaceEditorOpened; } set { isInplaceEditorOpened = value; } }
		bool ISetSchedulerStateService.IsAppointmentResized { get { return isAppointmentResized; } set { isAppointmentResized = value; } }
		bool ISetSchedulerStateService.IsPopupMenuOpened { get { return isPopupMenuOpened; } set { isPopupMenuOpened = value; } }
		bool ISetSchedulerStateService.AreAppointmentsDragged { get { return areAppointmentsDragged; } set { areAppointmentsDragged = value; } }
		bool ISetSchedulerStateService.IsAnimationActive { get { return isAnimationActive; } set { isAnimationActive = value; } }
		bool ISetSchedulerStateService.IsApplyChanges { get { return isApplyChanges; } set { isApplyChanges = value; } }
		#endregion
	}
	#endregion
	public static class SchedulerStateHelper {
		public static void BeginResize(InnerSchedulerControl control) {
			ISetSchedulerStateService setStateService = control.GetService<ISetSchedulerStateService>();
			if(setStateService != null)
				setStateService.IsAppointmentResized = true;
		}
		public static void EndResize(InnerSchedulerControl control) {
			ISetSchedulerStateService setStateService = control.GetService<ISetSchedulerStateService>();
			if(setStateService != null)
				setStateService.IsAppointmentResized = false;
		}
	}
}
