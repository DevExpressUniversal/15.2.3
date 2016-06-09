#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum DashboardItemMasterFilterMode { None, Single, Multiple };
	public class DashboardItemInteractivityOptions : FilterableDashboardItemInteractivityOptions {
		const string xmlIsMasterFilter = "IsMasterFilter";
		const string xmlIsDrillDownEnabled = "IsDrillDownEnabled";
		const string xmlMasterFilterMode = "MasterFilterMode";
		const DashboardItemMasterFilterMode DefaultMasterFilterMode = DashboardItemMasterFilterMode.None;
		bool isDrillDownEnabled;
		DashboardItemMasterFilterMode masterFilterMode = DefaultMasterFilterMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemInteractivityOptionsMasterFilterMode"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DashboardItemMasterFilterMode.None)
		]
		public DashboardItemMasterFilterMode MasterFilterMode {
			get { return masterFilterMode; }
			set {
				if(value != masterFilterMode) {
					masterFilterMode = value;
					OnChanged(ChangeReason.Interactivity);
				}
			}
		}
		[
		Obsolete("This property is now obsolete. Use the MasterFilterMode property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool IsMasterFilter {
			get {
				return masterFilterMode == DashboardItemMasterFilterMode.Single || masterFilterMode == DashboardItemMasterFilterMode.Multiple;
			}
			set {
				SetMasterFilterMode(value);
				OnChanged(ChangeReason.Interactivity);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemInteractivityOptionsIsDrillDownEnabled"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(false)
		]
		public bool IsDrillDownEnabled {
			get { return isDrillDownEnabled; }
			set {
				if(isDrillDownEnabled != value) {
					isDrillDownEnabled = value;
					OnChanged(ChangeReason.Interactivity);
				}
			}
		}
		protected override bool ShouldSerializeToXml { get { return base.ShouldSerializeToXml || isDrillDownEnabled || masterFilterMode != DefaultMasterFilterMode; } }
		protected internal DashboardItemInteractivityOptions(bool defaultIgnoreMasterFilters)
			: base(defaultIgnoreMasterFilters) { 
		}
		internal override void CopyTo(FilterableDashboardItemInteractivityOptions destination) {
			base.CopyTo(destination);
			DashboardItemInteractivityOptions dashboardItemInteractivityOptions = destination as DashboardItemInteractivityOptions;
			if (dashboardItemInteractivityOptions != null) {
				dashboardItemInteractivityOptions.IsDrillDownEnabled = IsDrillDownEnabled;
				dashboardItemInteractivityOptions.MasterFilterMode = MasterFilterMode;
			}
		}
		protected override void SaveContentToXml(XElement element) {
			base.SaveContentToXml(element);
			if(masterFilterMode != DefaultMasterFilterMode)
				element.Add(new XAttribute(xmlMasterFilterMode, masterFilterMode));
			if(isDrillDownEnabled)
				element.Add(new XAttribute(xmlIsDrillDownEnabled, isDrillDownEnabled));
		}
		protected override void LoadContentFromXml(XElement element) {
			base.LoadContentFromXml(element);
			string isDrillDownEnabledAttr = XmlHelper.GetAttributeValue(element, xmlIsDrillDownEnabled);
			if (!String.IsNullOrEmpty(isDrillDownEnabledAttr))
				isDrillDownEnabled = XmlHelper.FromString<bool>(isDrillDownEnabledAttr);
			string isMasterFilterAttr = XmlHelper.GetAttributeValue(element, xmlIsMasterFilter);
			if(!String.IsNullOrEmpty(isMasterFilterAttr))
				SetMasterFilterMode(XmlHelper.FromString<bool>(isMasterFilterAttr));
			string masterFilterModeAttr = XmlHelper.GetAttributeValue(element, xmlMasterFilterMode);
			if(!String.IsNullOrEmpty(masterFilterModeAttr))
				MasterFilterMode = XmlHelper.FromString<DashboardItemMasterFilterMode>(masterFilterModeAttr);
		}
		void SetMasterFilterMode(bool isMasterFilter) {
			masterFilterMode = isMasterFilter ? DashboardItemMasterFilterMode.Multiple : DashboardItemMasterFilterMode.None;
		}
	}
}
