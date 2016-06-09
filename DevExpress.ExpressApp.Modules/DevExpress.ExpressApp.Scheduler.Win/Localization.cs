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
using DevExpress.ExpressApp.Localization;
using DevExpress.XtraScheduler;
using System.Resources;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.SystemModule;
using System.Reflection;
using DevExpress.ExpressApp.Win.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraScheduler.Localization;
using DevExpress.Utils.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Scheduler.Win {
	public class SchedulerControlResourceLocalizerProvider : ActiveLocalizerProvider<SchedulerStringId> {
		protected override XtraLocalizer<SchedulerStringId> GetActiveLocalizerCore() {
			return SchedulerControlLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<SchedulerStringId> localizer) {
		}
		public SchedulerControlResourceLocalizerProvider()
			: base(new SchedulerControlDefaultLocalizer()) {
		}
	}
	public class SchedulerControlDefaultLocalizer : SchedulerResLocalizer {
		public override string GetLocalizedString(SchedulerStringId id) {
			if(id == SchedulerStringId.MenuCmd_RestoreOccurrence) {
				return "Restore Occurrence";
			}
			return base.GetLocalizedString(id);
		}
	}
	[System.ComponentModel.DisplayName("XtraScheduler Control")]
	public class SchedulerControlLocalizer : SchedulerResLocalizer, IXafResourceLocalizer {
		private ControlResourcesLocalizerLogic logic;
		private static SchedulerControlLocalizer active;
		protected override ResourceManager CreateResourceManagerCore() {
			logic = new ControlResourcesLocalizerLogic(this);
			return logic.Manager;
		}
		public static new SchedulerControlLocalizer Active {
			get {
				return active;
			}
			set {
				active = value;
			}
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			Active = this;
			DevExpress.XtraScheduler.Localization.SchedulerResLocalizer.SetActiveLocalizerProvider(new SchedulerControlResourceLocalizerProvider());
		}
		public void Reset() {
			logic.Reset();
		}
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
						"DevExpress.XtraScheduler",
						"DevExpress.XtraScheduler.LocalizationRes",
						"SchedulerStringId.",
						typeof(XtraScheduler.Localization.SchedulerLocalizer).Assembly
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
}
