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
using System.Linq;
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Group)
	]
	public class DashboardItemGroup : DashboardItem, IMasterFilterItem, IFilterGroupOwner, IFiltersProvider {
		const string xmlGroup = "Group";
		DashboardItemGroupInteractivityOptions interactivityOptions;
		GroupFilterAgent filterAgent;
		IFilterGroup filterGroup;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemGroupItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public IEnumerable<DashboardItem> Items {
			get {
				CheckDashboard();
				return Dashboard.Items.Where(item => item.Group == this);
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemGroupInteractivityOptions"),
#endif
 Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardItemGroupInteractivityOptions InteractivityOptions { get { return interactivityOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new DashboardItemGroup Group { get { return null; } }
		IFilterGroup IFilterGroupOwner.FilterGroup { get { return filterGroup; } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameItemGroup); } }
		protected internal override bool IsGroup { get { return true; } }
		public DashboardItemGroup() {
			interactivityOptions = new DashboardItemGroupInteractivityOptions();
			interactivityOptions.Changed += OnInteractivityOptionsChanged;
			filterGroup = new FilterGroup();
			filterAgent = new GroupFilterAgent(filterGroup, 0);
		}
		void OnInteractivityOptionsChanged(object sender, ChangedEventArgs e) {
			SetupFilterAgent();
			OnChanged(ChangeReason.Interactivity);
		}
		void SetupFilterAgent() {
			if (ContextFilterGroup == null)
				filterAgent.Level = null;
			else
				filterAgent.Level = ContextFilterGroup.GetFilterLevel(InteractivityOptions.IgnoreMasterFilters, InteractivityOptions.IsMasterFilter);
		}
		protected override void DashboardChanged() {
			base.DashboardChanged();
			SetupFilterAgent();
		}
		void CheckDashboard() {
			if (Dashboard == null)
				throw new InvalidOperationException(DashboardLocalizer.GetString(DashboardStringId.MessageUnassignedDashboardItemGroup));
		}
		public void Add(DashboardItem item) {
			CheckDashboard();
			item.Group = this;
			if (!Dashboard.Items.Contains(item))
				Dashboard.Items.Add(item);
		}
		public void AddRange(params DashboardItem[] items) {
			foreach (DashboardItem item in items)
				Add(item);
		}
		public bool Contains(DashboardItem item) {
			CheckDashboard();
			return item.Group == this;
		}
		public void Remove(DashboardItem item) {
			if (Contains(item))
				item.Group = null;
		}
		public void Clear() {
			CheckDashboard();
			foreach (DashboardItem item in Dashboard.Items)
				Remove(item);
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		new public DashboardItem CreateCopy() {
			return null;
		}
		internal override DashboardLayoutNode CreateDashboardLayoutNode(double weight) {
			return new DashboardLayoutGroup(this, weight);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new DashboardItemGroupViewModel(this);
		}
		protected internal override XElement SaveToXml() {
			XElement element = new XElement(xmlGroup);
			SaveToXml(element);
			return element;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			interactivityOptions.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			interactivityOptions.LoadFromXml(element);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (filterAgent != null)
					filterAgent.Dispose();
			}
			base.Dispose(disposing);
		}
		string IMasterFilterItem.Name { get { return ComponentName; } }
		#region IUniqueMasterFilterValuesProvider
		IEnumerable<string> IFiltersProvider.FilterItemNames {
			get {
				IEnumerable<string> inputFilterNames = filterAgent.InputFilter.GetFilterValuesProviders(null).Cast<DataDashboardItem>().Select(item => item.ComponentName);
				IEnumerable<string> groupFilterNames = filterGroup.GetFilterValuesProviders(null).Cast<DataDashboardItem>().Select(item => item.ComponentName);
				return inputFilterNames.Concat(groupFilterNames);
			}
		}
		#endregion
	}
}
