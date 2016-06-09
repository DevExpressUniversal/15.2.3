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
using System.ComponentModel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
namespace DevExpress.XtraScheduler.UI {
	#region SchedulerCommandBarComponent (abstract class)
	[Designer("DevExpress.XtraScheduler.Design.SchedulerCommandBarComponentDesigner," + AssemblyInfo.SRAssemblySchedulerDesign)]
	public abstract class SchedulerCommandBarComponent : CommandBarComponentBase {
		#region Fields
		SchedulerControl schedulerControl;
		#endregion
		protected SchedulerCommandBarComponent()
			: base() {
		}
		protected SchedulerCommandBarComponent(IContainer container)
			: base(container) {
		}
		#region Properties
		[DefaultValue(null)]
		public SchedulerControl SchedulerControl {
			get { return schedulerControl; }
			set {
				if (Object.ReferenceEquals(schedulerControl, value))
					return;
				SetSchedulerControl(value);
			}
		}
		#endregion
		protected override void DetachItemProviderControl() {
			this.schedulerControl = null;
		}
		protected internal virtual void SetSchedulerControl(SchedulerControl value) {
			if (Initialization) {
				SetSchedulerControlCore(value);
				return;
			}
			if (SchedulerControl != null) {
				UnsubscribeSchedulerControlEvents();
				DeleteVisualItems();
			}
			SetSchedulerControlCore(value);
			if (SchedulerControl != null) {
				CreateNewVisualItems();
				SubscribeSchedulerControlEvents();
			}
		}
		protected internal virtual void SetSchedulerControlCore(SchedulerControl schedulerControl) {
			this.schedulerControl = schedulerControl;
		}
		protected internal virtual void UnsubscribeSchedulerControlEvents() {
			if (schedulerControl != null)
				schedulerControl.BeforeDispose -= new EventHandler(OnSchedulerControlBeforeDispose);
		}
		protected override void SubscribeItemProviderControlEvents() {
			SubscribeSchedulerControlEvents();
		}
		protected override void UnsubscribeItemProviderControlEvents() {
			UnsubscribeSchedulerControlEvents();
		}
		protected internal virtual void SubscribeSchedulerControlEvents() {
			if (schedulerControl != null)
				schedulerControl.BeforeDispose += new EventHandler(OnSchedulerControlBeforeDispose);
		}
		protected internal virtual void OnSchedulerControlBeforeDispose(object sender, EventArgs e) {
			SchedulerControl = null;
		}
		protected override void UpdateBarItem(BarItem item) {
			ISchedulerBarItem btn = item as ISchedulerBarItem;
			if (btn == null)
				return;
			btn.SchedulerControl = schedulerControl;
			base.UpdateBarItem(item);
		}
		protected override void PopulateNewItems(List<BarItem> items, BarCreationContextBase creationContext) {
			if (SchedulerControl == null)
				return;
			base.PopulateNewItems(items, creationContext);
		}
		protected override bool CanCreateVisualItems() {
			return base.CanCreateVisualItems() && SchedulerControl != null;
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler {
	#region FakeClassInRootNamespace (helper class)
	internal class FakeClassInRootNamespace {
	}
	#endregion
}
