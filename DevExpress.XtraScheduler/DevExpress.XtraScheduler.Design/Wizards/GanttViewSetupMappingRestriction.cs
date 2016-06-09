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
using DevExpress.XtraScheduler.Native;
namespace DevExpress.XtraScheduler.Design.Wizards {
	public interface IMappingWizardExtensionView {
		bool Visible { get; set; }
		string Caption { get; set; }
		string Description { get; set; }
		string Link { get; set; }
		string LinkCaption { get; set; }
		bool IsRestrictionActive { get; set; }
		event EventHandler RestrictionActiveChanged;
	}
	public interface IMappingWizardExtensionLogic {
		void AttachView(IMappingWizardExtensionView wiazardExtensionView);
		bool CheckRestriction(string mappingDisplayName);
		Action RefreshHandler { get; set; }
		List<String> GetMappingsToIgnore();
	}
	public class GanttViewMappingWarningRestrictionLogic : IMappingWizardExtensionLogic {
		const string SRCaption = "Gantt view mappings";
		const string SRDescription = "Certain fields are required for Gantt view only. If you do not use the Gantt view, leave the check box cleared. Unnecessary fields are grayed out.";
		const string SRLink = @"http://documentation.devexpress.com/#WindowsForms/CustomDocument10699";
		const string SRLinkCaption = "How to  Enable the Gantt View Functionality";
		List<string> mappingsToIgnoreNames;
		List<string> mappingsToIgnoreDisplayNames;
		IMappingWizardExtensionView wiazardExtensionView;
		bool isActive = true;
		Action refreshHandler;
		public GanttViewMappingWarningRestrictionLogic() {
			this.mappingsToIgnoreNames = GetMappingsNames();
			this.mappingsToIgnoreDisplayNames = GetMappingsDisplayNames();
		}
		public Action RefreshHandler { get { return this.refreshHandler; } set { this.refreshHandler = value; } }
		public void InitializeState(IPersistentObjectStorage<Appointment> persistentObjectStorage) {
			this.isActive = true;
			foreach (string mappingName in this.mappingsToIgnoreNames) {
				string member = persistentObjectStorage.Mappings.GetMappingMember(mappingName);
				if (!String.IsNullOrEmpty(member)) {
					this.isActive = false;
					return;
				}
			}
		}
		public void AttachView(IMappingWizardExtensionView wiazardExtensionView) {
			this.wiazardExtensionView = wiazardExtensionView;
			if (this.wiazardExtensionView == null)
				return;
			this.wiazardExtensionView.Visible = true;
			UpdateViewCaptions(this.wiazardExtensionView);
			this.wiazardExtensionView.IsRestrictionActive = !this.isActive;
			this.wiazardExtensionView.RestrictionActiveChanged += new EventHandler(wiazardExtensionView_RestrictionActiveChanged);
		}
		void UpdateViewCaptions(IMappingWizardExtensionView view) {
			view.Visible = true;
			view.Caption = SRCaption;
			view.Description = SRDescription;
			view.Link = SRLink;
			view.LinkCaption = SRLinkCaption;
		}
		void wiazardExtensionView_RestrictionActiveChanged(object sender, EventArgs e) {
			this.isActive = !wiazardExtensionView.IsRestrictionActive;
			Refresh();
		}
		public bool CheckRestriction(string mappingDisplayName) {
			if (!this.isActive)
				return false;
			return this.mappingsToIgnoreDisplayNames.Contains(mappingDisplayName);
		}
		public List<String> GetMappingsToIgnore() {
			if (this.isActive)
				return GetMappingsNames();
			return new List<String>();
		}
		protected List<string> GetMappingsNames() {
			return new List<string>() { AppointmentSR.PercentComplete, AppointmentSR.Id };
		}
		protected List<string> GetMappingsDisplayNames() {
			return new List<string>() { AppointmentSR.PercentComplete, AppointmentSR.IdMappingName };
		}
		protected void Refresh() {
			if (RefreshHandler == null)
				return;
			RefreshHandler();
		}		
	}
}
