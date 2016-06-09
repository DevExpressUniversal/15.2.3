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
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base.General;
using DevExpress.XtraScheduler;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerRecurrenceInfoController : SchedulerRecurrenceInfoControllerBase {
		protected override PropertyEditor GetRecurrenceInfoPropertyEditor() {
			IList<SchedulerRecurrenceInfoPropertyEditor> propertyEditorList = ((DetailView)View).GetItems<SchedulerRecurrenceInfoPropertyEditor>();
			if(propertyEditorList.Count == 1) {
				return propertyEditorList[0];
			}
			return null;
		}
		private void UpdateEnableRecurrenceChangeConfirmation() {
			SchedulerRecurrenceInfoPropertyEditor editor = (SchedulerRecurrenceInfoPropertyEditor)GetRecurrenceInfoPropertyEditor();
			if(editor != null && View.CurrentObject != null) {
				editor.RecurrenceChangeConfirmationEnabled = !ObjectSpace.IsNewObject(View.CurrentObject);
			}
		}
		private void View_ControlsCreated(object sender, EventArgs e) {
			UpdateEnableRecurrenceChangeConfirmation();
		}
		private void View_CurrentObjectChanged(object sender, EventArgs e) {
			UpdateEnableRecurrenceChangeConfirmation();
		}
		private void ObjectSpace_Committed(object sender, EventArgs e) {
			UpdateEnableRecurrenceChangeConfirmation();
		}
		private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
			if(e.Object == View.CurrentObject && (e.PropertyName == "StartOn" || e.PropertyName == "EndOn")) {
				IEvent schedulerEvent = View.CurrentObject as IEvent;
				if(schedulerEvent != null) {
					RefreshRecurrenceInfoPropertyEditor();
				}
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			View.ControlsCreated += new EventHandler(View_ControlsCreated);
			View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
			ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed += new EventHandler(ObjectSpace_Committed);
		}
		protected override void OnDeactivated() {
			View.ControlsCreated -= new EventHandler(View_ControlsCreated);
			View.CurrentObjectChanged -= new EventHandler(View_CurrentObjectChanged);
			ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
			ObjectSpace.Committed -= new EventHandler(ObjectSpace_Committed);
			base.OnDeactivated();
		}
	}
}
