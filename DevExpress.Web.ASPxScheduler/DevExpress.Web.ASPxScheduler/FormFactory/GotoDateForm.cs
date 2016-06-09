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
using System.Collections;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.XtraScheduler;
namespace DevExpress.Web.ASPxScheduler {
	#region GotoDateFormTemplateContainer
	public class GotoDateFormTemplateContainer : SchedulerFormTemplateContainer {
		readonly IEnumerable viewDataSource;
		public GotoDateFormTemplateContainer(ASPxScheduler control)
			: base(control) {
			this.viewDataSource = CreateViewDataSource();
		}
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerDate")]
#endif
		public DateTime Date { get { return Control.InnerControl.TimeZoneHelper.ToClientTime(Control.Selection.Interval.Start).Date; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerViewsDataSource")]
#endif
		public IEnumerable ViewsDataSource { get { return viewDataSource; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerActiveViewType")]
#endif
		public SchedulerViewType ActiveViewType { get { return Control.ActiveViewType; } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerApplyHandler")]
#endif
		public virtual string ApplyHandler { get { return String.Format("function() {{ ASPx.GotoDateApply(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerApplyScript")]
#endif
		public virtual string ApplyScript { get { return String.Format("ASPx.GotoDateApply(\"{0}\")", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerCancelHandler")]
#endif
		public override string CancelHandler { get { return String.Format("function() {{ ASPx.GotoDateCancel(\"{0}\"); }}", ControlClientId); } }
#if !SL
	[DevExpressWebASPxSchedulerLocalizedDescription("GotoDateFormTemplateContainerCancelScript")]
#endif
		public override string CancelScript { get { return String.Format("ASPx.GotoDateCancel(\"{0}\")", ControlClientId); } }
		protected override EventArgs CreateCommandEventArgs(object source, EventArgs e) {
			return e;
		}
		protected internal virtual IEnumerable CreateViewDataSource() {
			ListEditItemCollection collection = new ListEditItemCollection();
			SchedulerViewRepository views = Control.Views;
			int count = views.Count;
			for (int i = 0; i < count; i++) {
				InnerSchedulerViewBase view = views[i].InnerView;
				if (view.Enabled)
					collection.Add(view.DisplayName, view.Type);
			}
			return collection;
		}
	}
	#endregion
}
