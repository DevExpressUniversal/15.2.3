#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Text;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
namespace DevExpress.ExpressApp.Scheduler {
	public class SchedulerResourceController : ObjectViewController {
		private void View_ControlsCreating(object sender, EventArgs e) {
			if(View.CurrentObject != null) {
				DeactivateResourceEditor();
			}
		}
		protected virtual void DeactivateResourceEditor() {
			IList<ListPropertyEditor> listPropertyEditors = ((DetailView)View).GetItems<ListPropertyEditor>();
			foreach(ListPropertyEditor listPropertyEditor in listPropertyEditors) {
				if(typeof(DevExpress.XtraScheduler.Resource).IsAssignableFrom(listPropertyEditor.MemberInfo.ListElementType)) {
					listPropertyEditor.AllowEdit.SetItemValue("IsNotOccurrence", SchedulerUtils.IsBaseType((AppointmentType)((IEvent)View.CurrentObject).Type));
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreating += new EventHandler(View_ControlsCreating);
		}
		protected override void OnDeactivated() {
			View.ControlsCreating -= new EventHandler(View_ControlsCreating);
			base.OnDeactivated();
		}
		public SchedulerResourceController() {
			TargetObjectType = typeof(IEvent);
			TargetViewType = ViewType.DetailView;
		}
	}
}
