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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[DXToolboxItem(false)]
	public abstract class DashboardItem : IDashboardComponent, IChangeService, ISupportPrefix, ICurrencyCultureNameProvider, IDashboardItem, IFiltersProvider {
		const string xmlUniqueName = "UniqueName";
		const string xmlName = "Name";
		const string xmlComponentName = "ComponentName";
		const string xmlShowCaption = "ShowCaption";
		const string xmlGroup = "Group";
		internal static DashboardItem LoadDashboardItemFromXml(XElement itemElement) {
			XmlSerializer<DashboardItem> itemSerializer = XmlRepository.DashboardItemRepository.GetSerializer(itemElement.Name.LocalName);
			return itemSerializer == null ? null : itemSerializer.LoadFromXml(itemElement);
		}
		readonly Locker loadingLocker = new Locker();
		readonly Locker changeLocker = new Locker();
		bool showCaption;
		Dashboard dashboard;
		DashboardItemGroup group;
		string uniqueName_13_1;
		string componentName;
		string name;
		string groupName;
		ISite site;
		[
		Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dashboard Dashboard {
			get { return dashboard; }
			internal set {
				if (dashboard != value) {
					dashboard = value;
					DashboardChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemGroup"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.DashboardItemGroupListConverter),
		DefaultValue(null)
		]
		public DashboardItemGroup Group { 
			get { return group; } 
			set {
				if(group != value) {
					group = value;
					if(group != null && !Loading && !IsVSDesignMode) {
						Dashboard dashboard = Dashboard;
						if(dashboard != null) {
							ICollection<DashboardItemGroup> groups = dashboard.Groups;
							if(!groups.Contains(group))
								groups.Add(group);
						}
					}
					OnChanged(ChangeReason.DashboardItemGroup);
					GroupChanged();
				}
			} 
		}
		internal string GroupName { get { return group != null ? group.ComponentName : groupName; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false)
		]
		public string ComponentName { 
			get { return componentName; } 
			set {
				if(value != ComponentName) {
					string oldComponentName = ComponentName;
					if(!Loading && NameChanging != null)
						NameChanging(this, new NameChangingEventArgs(value));
					componentName = value;
					if(!Loading && site != null && site.Name != componentName)
						site.Name = componentName;
					if(Dashboard != null)
						Dashboard.RaiseDashboardItemComponentNameChanged(oldComponentName, componentName);
				}
			} 
		}
		[
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(true)
		]
		public string Name {
			get { return name; }
			set {
				if(value != name) {
					name = value;
					OnChanged(ChangeReason.Caption);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardItemShowCaption"),
#endif
		Category(CategoryNames.General)
		]
		public bool ShowCaption {
			get { return showCaption; }
			set {
				if(showCaption != value) {
					showCaption = value;
					OnChanged(ChangeReason.Caption, null, showCaption);
				}
			}
		}
		[Browsable(false)]
		public bool IsDisposed { get; private set; }
		ISite IComponent.Site { get { return site; } set { site = value; } }
		protected internal bool Loading { 
			get {
				bool loading = loadingLocker.IsLocked;
				if(!loading && dashboard != null)
					loading = dashboard.Loading;
				return loading;
			}
		}
		protected IFilterGroup ContextFilterGroup {
			get {
				if (Dashboard == null)
					return null;
				else {
					IFilterGroupOwner groupOwner = (IFilterGroupOwner)((object)Group ?? (object)Dashboard);
					return groupOwner.FilterGroup;
				}
			}
		}
		string ISupportPrefix.Prefix { get { return CaptionPrefix; } }
		protected internal abstract string CaptionPrefix { get; }
		protected string ActualCaption { get { return !string.IsNullOrEmpty(Name) ? Name : ComponentName; } }
		protected internal virtual string ElementCaption { get { return ActualCaption; } }
		protected internal virtual IElementContainer ElementContainer { get { return null; } }
		protected internal virtual bool IsGroup { get { return false; } }
		protected internal virtual bool IsFixed { get { return false; } }
		protected virtual bool DefaultShowCaption { get { return true; } }
		protected virtual IEnumerable<string> InputFilterNames { get { return new string[0]; } }
		internal bool IsVSDesignMode { get { return Helper.IsComponentVSDesignMode(this); } }
		internal string UniqueName_13_1 { get { return uniqueName_13_1; } }
		string IDashboardComponent.ComponentName { get { return ComponentName; } set { ComponentName = value; } }
		string IDashboardComponent.Name { get { return Name; } set { Name = value; } }
		string ICurrencyCultureNameProvider.CurrencyCultureName { get { return Dashboard != null ? Dashboard.CurrencyCultureName : Helper.DefaultCurrencyCultureName; } }
		[Browsable(false)]
		public event EventHandler Disposed;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging {
			add { NameChanging += value; }
		   remove { NameChanging -= value; }
		}
		event EventHandler<ChangedEventArgs> Changed;
		event EventHandler<ChangedEventArgs> IChangeService.Changed {
			add { Changed += value; }
			remove { Changed -= value; }
		}
		internal event EventHandler LoadCompleted;
		protected DashboardItem() {
			this.showCaption = DefaultShowCaption;
			Changed += (s, e) => {
				if (dashboard != null)
					dashboard.OnDashboardItemChanged(this, e);
			};
			LoadCompleted += (s, e) => {
				if (dashboard != null)
					dashboard.OnDashboardItemChanged(this, new ChangedEventArgs(ChangeReason.DashboardItemLoadCompleted, null, null));
			};
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			EndInitInternal();
			loadingLocker.Unlock();			
			if (LoadCompleted != null)
				LoadCompleted(this, EventArgs.Empty);
		}
		public DashboardItem CreateCopy() {
			XElement element = SaveToXml();
			DashboardItem copy = DashboardItem.LoadDashboardItemFromXml(element);
			copy.ComponentName = null;
			copy.EnsureConsistecy(this);
			return copy;
		}
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			IsDisposed = true;
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		internal void LockChanging() {
			changeLocker.Lock();
		}
		internal void UnlockChanging() {
			changeLocker.Unlock();
		}
		protected internal virtual XElement SaveToXml() {
			XElement itemElement = null;
			XmlSerializer<DashboardItem> itemSerializer = XmlRepository.DashboardItemRepository.GetSerializer(this);
			if(itemSerializer != null)
				itemElement = itemSerializer.SaveToXml(this);
			return itemElement;
		}
		internal void LoadFromXml(XElement element) {
			BeginInit();
			try {
				LoadFromXmlInternal(element);
			}
			finally {
				EndInit();
			}
		}
		protected virtual void DashboardChanged() { 
		}
		protected virtual void GroupChanged() { 
		}
		protected internal virtual void SaveToXml(XElement element) {
			if(!string.IsNullOrEmpty(ComponentName))
				element.Add(new XAttribute(xmlComponentName, ComponentName));
			if(!string.IsNullOrEmpty(name))
				element.Add(new XAttribute(xmlName, name));
			if(showCaption != DefaultShowCaption)
				element.Add(new XAttribute(xmlShowCaption, showCaption));
			if(group != null && !string.IsNullOrEmpty(group.ComponentName))
				element.Add(new XAttribute(xmlGroup, group.ComponentName));
		}
		protected internal virtual void LoadFromXmlInternal(XElement element) {
			string componentName = XmlHelper.GetAttributeValue(element, xmlComponentName);
			string name = XmlHelper.GetAttributeValue(element, xmlName);
			if(!string.IsNullOrEmpty(componentName)) {
				ComponentName = componentName;
				this.name = name;
			}
			else {
				string uniqueName_13_1 = XmlHelper.GetAttributeValue(element, xmlUniqueName);
				if(!string.IsNullOrEmpty(uniqueName_13_1)) {
					if(!ObjectTypeNameGenerator.ContainsWrongCharacters(uniqueName_13_1))
						ComponentName = uniqueName_13_1;
					else
						this.uniqueName_13_1 = uniqueName_13_1;
					this.name = !string.IsNullOrEmpty(name) ? name : uniqueName_13_1;
				}
			}
			string showCaptionAttr = XmlHelper.GetAttributeValue(element, xmlShowCaption);
			if(!String.IsNullOrEmpty(showCaptionAttr))
				showCaption = XmlHelper.FromString<bool>(showCaptionAttr);
			groupName = XmlHelper.GetAttributeValue(element, xmlGroup);
		}
		protected virtual void EndInitInternal() {
			OnEndLoading();
		}
		protected virtual void OnEndLoading() {
		}
		protected internal virtual void EnsureConsistecy(DashboardItem dashboardItem) {
			Group = dashboardItem.Group;
		}
		protected internal virtual void EnsureConsistecy(Dashboard dashboard) {
			if (!string.IsNullOrEmpty(groupName))
				Group = dashboard.Groups.FindFirst(group => group.ComponentName == groupName);
			else
				Group = null;
		}
		internal void OnChanged(ChangeReason reason) {
			OnChanged(reason, this);
		}
		internal void OnChanged(ChangeReason reason, object context) {
			OnChanged(reason, context, null);
		}
		protected internal void OnChanged(ChangeReason reason, object context, object param) {
			OnChanged(new ChangedEventArgs(reason, context, param));
		}
		void IChangeService.OnChanged(ChangedEventArgs e) {
			OnChanged(e);
		}
		internal void OnChanged(ChangedEventArgs e) {
			if(!Loading && !changeLocker.IsLocked) {
				OnChangedInternal(e);
				if (Changed != null)
					Changed(this, e);
			}
		}
		protected virtual void OnChangedInternal(ChangedEventArgs e) {
		}
		protected virtual IList<DimensionFilterValues> GetExternalFilterValues() {
			return null;
		}
		protected internal virtual void SetState(DashboardItemState state) {
		}
		protected internal abstract DashboardItemViewModel CreateViewModel();
		protected internal virtual ConditionalFormattingModel CreateConditionalFormattingModel() {
			return null;
		}
		internal DashboardItemCaptionViewModel CreateCaptionViewModel() {
			return new DashboardItemCaptionViewModel {
				ShowCaption = ShowCaption,
				Caption = ElementCaption
			};
		}
		protected internal virtual void PrepareState(DashboardItemState state) {
		}
		protected virtual void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionDashboardItemName), new[] { new DashboardItemNameProvider(this) }));
			descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionDashboardItemComponentName), new[] { new DashboardItemComponentNameProvider(this) }));
		}
		internal IEnumerable<EditNameDescription> GetEditNameDescriptions() {
			List<EditNameDescription> descriptions = new List<EditNameDescription>();
			FillEditNameDescriptions(descriptions);
			return descriptions;
		}
		internal virtual DashboardLayoutNode CreateDashboardLayoutNode(double weight) {
			return new DashboardLayoutItem(this, weight);
		}
		DashboardItem IDashboardItem.GetItem() {
			return this;
		}
		#region IFiltersProvider
		IEnumerable<string> IFiltersProvider.FilterItemNames { get { return InputFilterNames; ; } }
		#endregion
	}
}
