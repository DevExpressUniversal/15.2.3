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
using System.Reflection;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.Persistent.Base;
using DevExpress.Web.Localization;
using DevExpress.Web.ASPxScheduler.Localization;
using DevExpress.Utils.Localization;
using DevExpress.Web.ASPxScheduler;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraScheduler.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Scheduler.Web {
	public class SchedulerControlCoreLocalizerProvider : ActiveLocalizerProvider<SchedulerStringId> {
		protected override XtraLocalizer<SchedulerStringId> GetActiveLocalizerCore() {
			return SchedulerControlCoreLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<SchedulerStringId> localizer) {
		}
		public SchedulerControlCoreLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("Scheduler Core Control")]
	public class SchedulerControlCoreLocalizer : DevExpress.XtraScheduler.Localization.SchedulerResLocalizer, IXafResourceLocalizer {
		private ControlResourcesLocalizerLogic logic;
		protected override ResourceManager CreateResourceManagerCore() {
			logic = new ControlResourcesLocalizerLogic(this);
			return logic.Manager;
		}
		public static new SchedulerControlCoreLocalizer Active {
			get {
				return ValueManager.GetValueManager<SchedulerControlCoreLocalizer>("SchedulerControlCoreLocalizer_SchedulerControlCoreLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<SchedulerControlCoreLocalizer>("SchedulerControlCoreLocalizer_SchedulerControlCoreLocalizer").Value = value;
			}
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			Active = this;
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
	public class ASPxSchedulerControlLocalizerProvider : ActiveLocalizerProvider<ASPxSchedulerStringId> {
		protected override XtraLocalizer<ASPxSchedulerStringId> GetActiveLocalizerCore() {
			return ASPxSchedulerControlLocalizer.Active;
		}
		protected override void SetActiveLocalizerCore(XtraLocalizer<ASPxSchedulerStringId> localizer) {
		}
		public ASPxSchedulerControlLocalizerProvider()
			: base(null) {
		}
	}
	[System.ComponentModel.DisplayName("ASPxScheduler Control")]
	public class ASPxSchedulerControlLocalizer : DevExpress.Web.ASPxScheduler.Localization.ASPxSchedulerResLocalizer, IXafResourceLocalizer {
		private ControlResourcesLocalizerLogic logic;
		static ASPxSchedulerControlLocalizer() {
			string[] forceStaticConstructor = ASPxScheduler.FormNames; 
		}
		public ASPxSchedulerControlLocalizer() {
			logic = new ControlResourcesLocalizerLogic(this);
		}
		private string GetStringName(ASPxSchedulerStringId id) {
			return "ASPxSchedulerStringId." + id.ToString();
		}
		private ResourceManager Manager {
			get {
				return logic.Manager;
			}
		}
		public override string GetLocalizedString(ASPxSchedulerStringId id) {
			return Manager.GetString(GetStringName(id));
		}
		public static new ASPxSchedulerControlLocalizer Active {
			get {
				return ValueManager.GetValueManager<ASPxSchedulerControlLocalizer>("ASPxSchedulerControlLocalizer_ASPxSchedulerControlLocalizer").Value;
			}
			set {
				ValueManager.GetValueManager<ASPxSchedulerControlLocalizer>("ASPxSchedulerControlLocalizer_ASPxSchedulerControlLocalizer").Value = value;
			}
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			Active = this;
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
						"DevExpress.ASPxScheduler",
						"LocalizationRes",
						"ASPxSchedulerStringId.",
						typeof(ASPxScheduler).Assembly
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public enum SchedulerAspNetModuleId {
		DeleteConfirmationCaption,
		DeleteConfirmationOccurrence,
		DeleteConfirmationSeries,
		DeleteRecurrencePopupControlHeader,
		RecurrencePopupControlHeader,
		RecurrencePopupControlRemoveRecurrence
	}
	[System.ComponentModel.DisplayName("ASPxScheduler Module")]
	public class SchedulerAspNetModuleLocalizer : XafResourceLocalizer<SchedulerAspNetModuleId> {
		private static SchedulerAspNetModuleLocalizer activeLocalizer;
		static SchedulerAspNetModuleLocalizer() {
			activeLocalizer = new SchedulerAspNetModuleLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"SchedulerAspNetModule",
				"DevExpress.ExpressApp.Scheduler.Web.LocalizationResources",
				"",
				GetType().Assembly
				);
		}
		public static SchedulerAspNetModuleLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
}
