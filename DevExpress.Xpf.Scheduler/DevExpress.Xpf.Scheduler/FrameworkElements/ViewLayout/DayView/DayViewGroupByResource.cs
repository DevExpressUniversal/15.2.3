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

using System.Windows;
using System;
using System.Collections.ObjectModel;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDayViewGroupByResource : VisualDayViewResourcesBasedViewInfo {
		static VisualDayViewGroupByResource() {
		}
		public VisualDayViewGroupByResource() {
			DefaultStyleKey = typeof(VisualDayViewGroupByResource);
		}
		protected override void CopyAppointmentsViewInfoCore(ISchedulerViewInfoBase source) {
			base.CopyAppointmentsViewInfoCore(source);
			if (ResourceContainers == null)
				return;
			DayViewGroupByResource groupByNone = source as DayViewGroupByResource;
			int count = groupByNone.ResourcesContainers.Count;
			for (int i = 0; i < count; i++)
				ResourceContainers[i].CopyAppointmentsFrom(groupByNone.ResourcesContainers[i]);
		}	  
	}	
}
