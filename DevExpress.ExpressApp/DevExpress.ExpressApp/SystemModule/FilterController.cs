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
using System.ComponentModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.SystemModule {
	public enum FullTextSearchTargetPropertiesMode { AllSearchableMembers, VisibleColumns }
	public class CustomGetFullTextSearchPropertiesEventArgs : HandledEventArgs {
		private List<String> properties = new List<String>();
		public CustomGetFullTextSearchPropertiesEventArgs() { }
		public List<String> Properties {
			get { return properties; }
		}
	}
	public class CustomBuildCriteriaEventArgs : HandledEventArgs {
		private String searchText;
		private CriteriaOperator criteria;
		public CustomBuildCriteriaEventArgs(String searchText) {
			this.searchText = searchText;
		}
		public String SearchText {
			get { return searchText; }
		}
		public CriteriaOperator Criteria {
			get { return criteria; }
			set { criteria = value; }
		}
	}
	public class CustomGetFiltersEventArgs : EventArgs {
		IModelListViewFilters filters;
		public CustomGetFiltersEventArgs(IModelListViewFilters filters) {
			this.filters = filters;
		}
		public IModelListViewFilters Filters {
			get {
				return filters;
			}
			set {
				filters = value;
			}
		}
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
	public class ListViewFilterAttribute : Attribute {
		private int index;
		private String id;
		private String criteria;
		private String caption;
		private String description;
		private String imageName;
		private bool isCurrentFilter = false;
		public ListViewFilterAttribute(String id, String criteria) : this(id, criteria, "", "", false) { }
		public ListViewFilterAttribute(String id, String criteria, bool isCurrentFilter) : this(id, criteria, "", "", isCurrentFilter) { }
		public ListViewFilterAttribute(String id, String criteria, String caption) : this(id, criteria, caption, "", false) { }
		public ListViewFilterAttribute(String id, String criteria, String caption, bool isCurrentFilter) : this(id, criteria, caption, "", isCurrentFilter) { }
		public ListViewFilterAttribute(String id, String criteria, String caption, String description)
			: this(id, criteria, caption, description, false) {
		}
		public ListViewFilterAttribute(String id, String criteria, String caption, String description, bool isCurrentFilter)
			: this(id, criteria, caption, description, int.MinValue, isCurrentFilter) {
		}
		public ListViewFilterAttribute(String id, String criteria, String caption, String description, int index, bool isCurrentFilter) {
			this.id = id;
			this.criteria = criteria;
			this.caption = caption;
			this.description = description;
			this.isCurrentFilter = isCurrentFilter;
			this.index = index;
		}
		public int Index {
			get { return index; }
			set { index = value; }
		}
		public String ImageName {
			get { return imageName; }
			set { imageName = value; }
		}
		public String Id {
			get { return id; }
		}
		public String Criteria {
			get { return criteria; }
		}
		public String Caption {
			get { return caption; }
		}
		public String Description {
			get { return description; }
		}
		public bool IsCurrentFilter {
			get { return isCurrentFilter; }
		}
	}
	public interface IModelListViewFilter {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelListViewFilterFilters")]
#endif
		IModelListViewFilters Filters { get; }
	}
	[ModelNodesGenerator(typeof(ModelListViewFiltersGenerator))]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelListViewFilters")]
#endif
	public interface IModelListViewFilters : IModelNode, IModelList<IModelListViewFilterItem> {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewFiltersCurrentFilter"),
#endif
 DataSourceProperty("this")]
		[ModelPersistentName("CurrentFilterId")]
		[Category("Behavior")]
		IModelListViewFilterItem CurrentFilter { get; set; }
	}
	[DomainLogic(typeof(IModelListViewFilters))]
	public static class ModelListViewFiltersLogic {
		public static List<String> Get_Filters(IModelListViewFilters model) {
			List<String> filters = new List<String>();
			foreach (IModelListViewFilterItem filterItem in model) {
				filters.Add(filterItem.Id);
			}
			return filters;
		}
	}
	[ModelPersistentName("Filter")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelListViewFilterItem")]
#endif
	public interface IModelListViewFilterItem : IModelBaseChoiceActionItem {
		[Browsable(false)]
		Type TargetType { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewFilterItemCriteria"),
#endif
 Category("Behavior")]
		[CriteriaOptions("TargetType")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.CriteriaModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		String Criteria { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelListViewFilterItemDescription"),
#endif
 Localizable(true)]
		String Description { get; set; }
	}
	[DomainLogic(typeof(IModelListViewFilterItem))]
	public static class ModelListViewFilterItemLogic {
		public static Type Get_TargetType(IModelListViewFilterItem modelFilterItem) {
			IModelListView modelListView = modelFilterItem.Parent.Parent as IModelListView;
			return modelListView != null ? modelListView.ModelClass.TypeInfo.Type : null;
		}
	}
	public class FilterController : ViewController, IDisposable, IModelExtender {
		public const String FilterCriteriaName = "ListViewFilter";
		public const String FullTextSearchCriteriaName = "FullTextSearchCriteria";
		public const String FullTextSearchActionId = "FullTextSearch";
		private ParametrizedAction fullTextFilterAction;
		private FullTextSearchTargetPropertiesMode fullTextSearchTargetPropertiesMode = FullTextSearchTargetPropertiesMode.AllSearchableMembers;
		private SearchMode fullTextSearchMode = SearchMode.SearchInObject;
		private Object currentObject_BeforeCriteriaApplied;
		private SingleChoiceAction setFilterAction;
		private void setFilterAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			SetFilter(args);
		}
		private void fullTextSearchAction_OnExecute(Object sender, ParametrizedActionExecuteEventArgs e) {
			FullTextSearch(e);
		}
		private void View_ModelChanged(Object sender, EventArgs e) {
			SetupFilters();
		}
		private void CollectionSource_CriteriaApplying(Object sender, EventArgs e) {
			currentObject_BeforeCriteriaApplied = View.CurrentObject;
		}
		private void CollectionSource_CriteriaApplied(Object sender, EventArgs e) {
			if(!View.CollectionSource.IsCollectionResetting) {
				if(currentObject_BeforeCriteriaApplied != null) {
					Object obj = View.GetObject(currentObject_BeforeCriteriaApplied);
					if(View.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView) {
						View.CurrentObject = obj;
					}
					else {
						Boolean? isObjectFitForCollection = View.CollectionSource.IsObjectFitForCollection(obj);
						if((isObjectFitForCollection != null) && isObjectFitForCollection.Value) {
							View.CurrentObject = obj;
						}
					}
					currentObject_BeforeCriteriaApplied = null;
				}
			}
			setFilterAction.ToolTip = GetToolTip(setFilterAction.Caption);
		}
		private Boolean CanFilterByNonPersistentMembers(IObjectSpace objectSpace) {
			if(objectSpace is BaseObjectSpace) {
				return ((BaseObjectSpace)objectSpace).CanFilterByNonPersistentMembers;
			}
			else {
				return true;
			}
		}
		private IModelListViewFilters GetFiltersNode() {
			IModelListViewFilters filters = null;
			if(View != null && View.Model != null) {
				filters = ((IModelListViewFilter)View.Model).Filters;
			}
			CustomGetFiltersEventArgs args = new CustomGetFiltersEventArgs(filters);
			if(CustomGetFilters != null) {
				CustomGetFilters(this, args);
			}
			return args.Filters;
		}
		protected virtual String[] GetShownProperties() {
			List<String> visibleProperties = new List<String>();
			ColumnsListEditor _editor = View.Editor as ColumnsListEditor;
			List<IModelColumn> modelColumns = new List<IModelColumn>();
			if(_editor != null && View.IsControlCreated) {
				foreach(ColumnWrapper item in _editor.Columns) {
					if(!String.IsNullOrEmpty(item.PropertyName) && item.Visible) {
						IModelColumn modelColumn = View.Model.Columns[item.Id];
						modelColumns.Add(modelColumn);
					}
				}
			}
			else {
				foreach(IModelColumn modelColumn in View.Model.Columns.GetVisibleColumns()) {
					modelColumns.Add(modelColumn);
				}
			}
			foreach(IModelColumn modelColumn in modelColumns) {
				String propertyName = GetBindingPropertyName(modelColumn);
				if(!String.IsNullOrEmpty(propertyName)) {
					visibleProperties.Add(propertyName);
				}
			}
			return visibleProperties.ToArray();
		}
		private String GetBindingPropertyName(IModelColumn modelColumn) {
			if(modelColumn != null) {
				IMemberInfo memberInfo = null;
				if(modelColumn.ModelMember != null) {
					memberInfo = new ObjectEditorHelperBase(modelColumn.ModelMember.MemberInfo.MemberTypeInfo, modelColumn).DisplayMember;
				}
				if(memberInfo != null) {
					return modelColumn.PropertyName + "." + memberInfo.Name;
				}
				else {
					return modelColumn.PropertyName;
				}
			}
			return String.Empty;
		}
		protected virtual ISearchCriteriaBuilder CreateSearchCriteriaBuilder() {
			return new SearchCriteriaBuilder();
		}
		protected virtual void OnCustomBuildCriteria(CustomBuildCriteriaEventArgs args) {
			if(CustomBuildCriteria != null) {
				CustomBuildCriteria(this, args);
			}
		}
		protected virtual void OnCustomGetFullTextSearchProperties(CustomGetFullTextSearchPropertiesEventArgs args) {
			if(CustomGetFullTextSearchProperties != null) {
				CustomGetFullTextSearchProperties(this, args);
			}
		}
		protected virtual void FullTextSearch(ParametrizedActionExecuteEventArgs args) {
			CustomBuildCriteriaEventArgs customBuildCriteriaEventArgs = new CustomBuildCriteriaEventArgs((String)args.ParameterCurrentValue);
			OnCustomBuildCriteria(customBuildCriteriaEventArgs);
			if(!customBuildCriteriaEventArgs.Handled) {
				ISearchCriteriaBuilder criteriaBuilder = CreateSearchCriteriaBuilder();
				criteriaBuilder.TypeInfo = View.ObjectTypeInfo;
				Boolean isClientMode = (View.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.Client);
				criteriaBuilder.IncludeNonPersistentMembers = isClientMode && CanFilterByNonPersistentMembers(ObjectSpace);
				criteriaBuilder.SearchInStringPropertiesOnly = false;
				criteriaBuilder.SearchText = customBuildCriteriaEventArgs.SearchText;
				criteriaBuilder.SearchMode = fullTextSearchMode;
				CustomGetFullTextSearchPropertiesEventArgs customGetSearchPropertiesEventArgs = new CustomGetFullTextSearchPropertiesEventArgs();
				OnCustomGetFullTextSearchProperties(customGetSearchPropertiesEventArgs);
				if(!customGetSearchPropertiesEventArgs.Handled) {
					customGetSearchPropertiesEventArgs.Properties.AddRange(GetFullTextSearchProperties());
				}
				criteriaBuilder.SetSearchProperties(customGetSearchPropertiesEventArgs.Properties);
				customBuildCriteriaEventArgs.Criteria = criteriaBuilder.BuildCriteria();
			}
			View.CollectionSource.Criteria[FilterController.FullTextSearchCriteriaName] = customBuildCriteriaEventArgs.Criteria;
		}
		protected void SetFilter(String filterID, String criteria) {
			Tracing.Tracer.LogSubSeparator("FilterController.SetFilter, View: " + View.Id);
			Tracing.Tracer.LogValue("filterID", filterID);
			ApplyFilter(FilterCriteriaName, criteria);
			GetFiltersNode().CurrentFilter = GetFiltersNode()[filterID];
		}
		protected virtual void SetFilter(SingleChoiceActionExecuteEventArgs args) {
			ChoiceActionItem filterInfo = args.SelectedChoiceActionItem;
			SetFilter(filterInfo.Id, (String)filterInfo.Data);
		}
		protected virtual String GetToolTip(String defaultToolTip) {
			String result = GetToolTip();
			return String.IsNullOrEmpty(result) ? defaultToolTip : result;
		}
		protected virtual String GetToolTip() { 
			String description = "";
			String toolTip = "";
			String andString = "";
			foreach(CriteriaOperator criteria in View.CollectionSource.Criteria) {
				if(!ReferenceEquals(criteria, null)) {
					andString = " And ";
					toolTip += criteria.ToString() + andString;
				}
			}
			toolTip = toolTip.Substring(0, toolTip.Length - andString.Length);
			if(SetFilterAction.SelectedItem != null) {
				IModelListViewFilterItem modelListViewFilterItem = SetFilterAction.SelectedItem.Model as IModelListViewFilterItem;
				if(modelListViewFilterItem != null) {
					description = modelListViewFilterItem.Description;
				}
			}
			if(!String.IsNullOrEmpty(description)) {
				toolTip = description + "\n" + toolTip;
			}
			return toolTip;
		}
		protected virtual void ApplyFilter(String criteriaName, String criteriaText) {
			if(View.CollectionSource.CanApplyCriteria) {
				View.CollectionSource.SetCriteria(criteriaName, criteriaText);
			}
		}
		protected virtual void RemoveFilter(String criteriaName) {
			if(View.CollectionSource.Criteria.ContainsKey(criteriaName)) {
				View.CollectionSource.Criteria.Remove(criteriaName);
			}
		}
		protected virtual void SetupFilters() {
			setFilterAction.BeginUpdate();
			setFilterAction.Items.Clear();
			IModelListViewFilters filters = GetFiltersNode();
			if(filters != null) {
				String currentFilterId = "";
				if (filters.Count > 0 && filters.CurrentFilter != null) {
					currentFilterId = filters.CurrentFilter.Id;
				}
				foreach(IModelListViewFilterItem filterItem in filters) {
					ChoiceActionItem item = new ChoiceActionItem(filterItem, filterItem.Criteria);
					setFilterAction.Items.Add(item);
					if(item.Id == currentFilterId) {
						setFilterAction.SelectedItem = item;
					}
				}
			}
			setFilterAction.EndUpdate();
			if(SetFilterAction.SelectedItem != null) {
				SetFilter(SetFilterAction.SelectedItem.Id, (String)SetFilterAction.SelectedItem.Data);
			}
		}
		protected virtual void UpdateActionState() {
			if((View != null) && (View.CollectionSource != null) && (View.ObjectTypeInfo != null)) {
				setFilterAction.Active.SetItemValue("Criteria locked", !View.CollectionSource.IsCriteriaLocked);
				setFilterAction.Active.SetItemValue("Can apply criteria", View.CollectionSource.CanApplyCriteria);
				fullTextFilterAction.Active.SetItemValue("Can apply criteria", View.CollectionSource.CanApplyCriteria);
				fullTextFilterAction.Active["IsPersistentType"] = View.ObjectTypeInfo.IsPersistent;
			}
		}
		protected override void OnActivated() {
			base.OnActivated();
			UpdateActionState();
			View.ModelChanged += new EventHandler(View_ModelChanged);
			View.CollectionSource.CriteriaApplying += new EventHandler(CollectionSource_CriteriaApplying);
			View.CollectionSource.CriteriaApplied += new EventHandler(CollectionSource_CriteriaApplied);
			SetupFilters();
		}
		protected override void OnDeactivated() {
			try {
				currentObject_BeforeCriteriaApplied = null;
				View.CollectionSource.CriteriaApplying -= new EventHandler(CollectionSource_CriteriaApplying);
				View.CollectionSource.CriteriaApplied -= new EventHandler(CollectionSource_CriteriaApplied);
				View.ModelChanged -= new EventHandler(View_ModelChanged);
			}
			finally {
				base.OnDeactivated();
			}
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			if((Frame != null) && ((Frame.Context == TemplateContext.LookupWindow) || (Frame.Context == TemplateContext.LookupControl))) {
				this.FullTextSearchTargetPropertiesMode = FullTextSearchTargetPropertiesMode.VisibleColumns;
			}
		}
		protected object PreviousCurrentObject {
			get { return currentObject_BeforeCriteriaApplied; }
			set { currentObject_BeforeCriteriaApplied = value; }
		}
		public FilterController() {
			this.fullTextFilterAction = new ParametrizedAction(this, FullTextSearchActionId, PredefinedCategory.FullTextSearch, typeof(String));
			this.fullTextFilterAction.Caption = "Filter by Text";
			this.fullTextFilterAction.ToolTip = "Filter records by text";
			this.fullTextFilterAction.NullValuePrompt = "Text to search...";
			this.fullTextFilterAction.PaintStyle = DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Image;
			this.fullTextFilterAction.ShortCaption = "Search";
			this.fullTextFilterAction.Execute += new DevExpress.ExpressApp.Actions.ParametrizedActionExecuteEventHandler(this.fullTextSearchAction_OnExecute);
			this.setFilterAction = new SingleChoiceAction(this, "SetFilter", "Filters");
			this.setFilterAction.Caption = "Filter";
			this.setFilterAction.ImageName = "MenuBar_Filter";
			this.setFilterAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.setFilterAction_OnExecute);
			this.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
		}
		public ICollection<String> GetFullTextSearchProperties() { 
			SearchCriteriaBuilder criteriaBuilder = new SearchCriteriaBuilder(View.ObjectTypeInfo);
			criteriaBuilder.IncludeNonPersistentMembers = false;
			switch(fullTextSearchTargetPropertiesMode) {
				case FullTextSearchTargetPropertiesMode.AllSearchableMembers:
					criteriaBuilder.FillSearchProperties();
					criteriaBuilder.AddSearchProperties(GetShownProperties());
					break;
				case FullTextSearchTargetPropertiesMode.VisibleColumns:
					List<String> shownProperties = new List<String>(GetShownProperties());
					String friendlyKeyMemberName = FriendlyKeyPropertyAttribute.FindFriendlyKeyMemberName(View.ObjectTypeInfo, true);
					if(!String.IsNullOrEmpty(friendlyKeyMemberName) && !shownProperties.Contains(friendlyKeyMemberName)) {
						shownProperties.Add(friendlyKeyMemberName);
					}
					criteriaBuilder.SetSearchProperties(shownProperties);
					break;
				default:
					throw new ArgumentException(fullTextSearchTargetPropertiesMode.ToString(), "fullTextSearchTargetPropertiesMode");
			}
			ICollection<String> result = null;
			if((View.CollectionSource != null) && (View.CollectionSource.DataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				result = ListView.CorrectMemberNamesForDataViewMode(View.ObjectTypeInfo, criteriaBuilder.SearchProperties, false, false);
			}
			else {
				result = criteriaBuilder.SearchProperties;
			}
			return result;
		}
		public SearchMode FullTextSearchMode {
			get { return fullTextSearchMode; }
			set { fullTextSearchMode = value; }
		}
		public FullTextSearchTargetPropertiesMode FullTextSearchTargetPropertiesMode {
			get { return fullTextSearchTargetPropertiesMode; }
			set { fullTextSearchTargetPropertiesMode = value; }
		}
		public event EventHandler<CustomGetFullTextSearchPropertiesEventArgs> CustomGetFullTextSearchProperties;
		public event EventHandler<CustomBuildCriteriaEventArgs> CustomBuildCriteria;
		public event EventHandler<CustomGetFiltersEventArgs> CustomGetFilters;
#if !SL
	[DevExpressExpressAppLocalizedDescription("FilterControllerSetFilterAction")]
#endif
		public SingleChoiceAction SetFilterAction {
			get { return setFilterAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FilterControllerFullTextFilterAction")]
#endif
		public ParametrizedAction FullTextFilterAction {
			get { return fullTextFilterAction; }
		}
#if !SL
	[DevExpressExpressAppLocalizedDescription("FilterControllerView")]
#endif
		public new ListView View {
			get { return base.View as ListView; }
		}
#if DebugTest
		public static bool IsFinalized__ = false;
		~FilterController() {
			IsFinalized__ = true;
		}
		public static bool IsDisposed__ = false;
		protected override void Dispose(bool disposing) {
			IsDisposed__ = true;
			base.Dispose(disposing);
		}
		void IDisposable.Dispose() {
			base.Dispose();
			GC.ReRegisterForFinalize(this);
		}
#endif
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelListView, IModelListViewFilter>();
		}
		#endregion
	}
	public class ModelListViewFiltersGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			IModelListView modelView = (IModelListView)node.Parent;
			IEnumerable<ListViewFilterAttribute> filterAttributes = modelView.ModelClass.TypeInfo.FindAttributes<ListViewFilterAttribute>(true);
			foreach(ListViewFilterAttribute filterAttribute in filterAttributes) {
				IModelListViewFilterItem filterNode = node.AddNode<IModelListViewFilterItem>(filterAttribute.Id);
				filterNode.Criteria = filterAttribute.Criteria;
				if(filterAttribute.Index != int.MinValue) {
					filterNode.Index = filterAttribute.Index;
				}
				if(!String.IsNullOrEmpty(filterAttribute.Caption)) {
					filterNode.Caption = filterAttribute.Caption;
				}
				if(!String.IsNullOrEmpty(filterAttribute.Description)) {
					filterNode.Description = filterAttribute.Description;
				}
				if (!String.IsNullOrEmpty(filterAttribute.ImageName)) {
					filterNode.ImageName = filterAttribute.ImageName;
				}
				if(filterAttribute.IsCurrentFilter) {
					((IModelListViewFilters)node).CurrentFilter = filterNode;
				}
			}
		}
	}
}
