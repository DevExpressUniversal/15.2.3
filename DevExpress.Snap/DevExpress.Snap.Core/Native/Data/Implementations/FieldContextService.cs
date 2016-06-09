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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.DataAccess;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils;
using DevExpress.XtraReports.Native;
using DevExpress.XtraRichEdit.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class DataControllerFieldContextService : IFieldContextService {
		readonly ParameterCollection parameters;
		public DataControllerFieldContextService(ParameterCollection parameters) {
			this.parameters = parameters;
		}
		public ParameterCollection Parameters { get { return parameters; } }
		DataControllerCalculationContext calculationContext;
		public ICalculationContext BeginCalculation(IDataSourceDispatcher dataSourceDispatcher) {
			if (calculationContext == null)
				calculationContext = new DataControllerCalculationContext(this, dataSourceDispatcher);
			calculationContext.IncRefCount();
			return calculationContext;
		}
		public void EndCalculation(ICalculationContext calculationContext) {
			Guard.ArgumentNotNull(calculationContext, "calculationContext");
			if(!Object.ReferenceEquals(calculationContext, this.calculationContext))
				Exceptions.ThrowInternalException();
			this.calculationContext.DecRefCount();
			if (!this.calculationContext.HasRef) {
				this.calculationContext.Dispose();
				this.calculationContext = null;
			}
		}
	}
	public class DataControllerCalculationContext : ICalculationContext, IDisposable {
		class SnapDataContext : DataContextBase {
			public DataBrowser GetDataBrowserSafe(IDataComponent dataSource, string dataMember) {
				return GetDataBrowserInternal(dataSource, dataMember, true);
			}
		}
		int refCount;
		DataControllerFieldContextService dataContextService;
		IDataSourceDispatcher dataSourceDispatcher;
		Dictionary<IDataControllerListFieldContext, SnapListController> masterControllersCache;
		string[] fieldNames;
		public DataControllerCalculationContext(DataControllerFieldContextService dataContextService, IDataSourceDispatcher dataSourceDispatcher) {
			this.dataContextService = dataContextService;
			this.dataSourceDispatcher = dataSourceDispatcher;
			masterControllersCache = new Dictionary<IDataControllerListFieldContext, SnapListController>();
		}
		public bool HasRef { get { return refCount > 0; } }
		public void IncRefCount() {
			refCount++;			
		}
		public void DecRefCount() {
			if(refCount == 0)
				Exceptions.ThrowInternalException();
			refCount--;
		}
		#region ICalculationContext members        
		public string[] FieldNames {
			get { return this.fieldNames; }
			set { this.fieldNames = value; }
		}
		public IDataEnumerator GetChildDataEnumerator(IFieldContext fieldContext, FieldPathDataMemberInfo path) {
			ISingleObjectFieldContext singleObjectFieldContext = fieldContext as ISingleObjectFieldContext;
			if (singleObjectFieldContext == null)
				Exceptions.ThrowInternalException();
			IDataEnumerator result = null;
			if (singleObjectFieldContext.ListContext != null)
				result = GetDetailListEnumerator(singleObjectFieldContext, path);
			else
				result = GetMasterListEnumerator(singleObjectFieldContext, path);
			return result;
		}
		public object GetRawValue(IFieldContext fieldContext, FieldPathDataMemberInfo path) {
			ProxyFieldContext proxyFieldContext = fieldContext as ProxyFieldContext;
			if (proxyFieldContext != null) {
				return FieldNull.Value;
			}
			ISingleObjectFieldContext rawValueContext = GetRawValueFieldContextCore(fieldContext, path);
			return GetRawValueCore(rawValueContext);
		}
		object GetRawValueCore(ISingleObjectFieldContext fieldContext) {
			if (fieldContext.ListContext != null)
				return GetColumnValue(fieldContext, FieldPathDataMemberInfo.Empty);
			else
				return GetObjectValue(fieldContext, FieldPathDataMemberInfo.Empty);
		}
		public object GetSummaryValue(IFieldContext fieldContext, FieldPathDataMemberInfo path, SummaryRunning summaryRunning, SummaryItemType summaryFunc, bool ignoreNullValues, int groupLevel) {
			ISingleObjectFieldContext singleObjectFieldContext = fieldContext as ISingleObjectFieldContext;
			if (singleObjectFieldContext == null)
				Exceptions.ThrowInternalException();
			SnapListController listController = GetListController(singleObjectFieldContext.ListContext);
			if (listController == null)
				return FieldNull.Value;
			string pathToList = GetFullPathToParentList(singleObjectFieldContext);
			if (path.Items.Count > 0)
				for (int i = 0; i < path.Items.Count; i++)
					pathToList = PathHelper.Join(pathToList, path.Items[i].FieldName);
			return listController.GetGroupSummaryValue(singleObjectFieldContext.RowHandle, pathToList, summaryRunning, summaryFunc, ignoreNullValues, groupLevel);
		}
		public SnapListController RestoreListController(IFieldContext fieldContext) {
			IDataControllerListFieldContext listFieldContext = GetListFieldContext(fieldContext);
			if (listFieldContext == null)
				return null;
			return GetListController(listFieldContext);
		}
		IDataControllerListFieldContext GetListFieldContext(IFieldContext fieldContext) {
			ISingleObjectFieldContext singleObjectFieldContext = null;
			singleObjectFieldContext = fieldContext as ISingleObjectFieldContext;
			if (singleObjectFieldContext != null) {
				if (singleObjectFieldContext.ListContext != null)
					return singleObjectFieldContext.ListContext;
				else
					return new ListFieldContext(singleObjectFieldContext, null);
			}
			return fieldContext as IDataControllerListFieldContext;
		}
		public IFieldContext GetRawValueFieldContext(IFieldContext fieldContext, FieldPathDataMemberInfo path) {
			return GetRawValueFieldContextCore(fieldContext, path);
		}
		ISingleObjectFieldContext GetRawValueFieldContextCore(IFieldContext fieldContext, FieldPathDataMemberInfo path) {
			ISingleObjectFieldContext singleObjectFieldContext = fieldContext as ISingleObjectFieldContext;
			if (singleObjectFieldContext == null)
				Exceptions.ThrowInternalException();
			int count = path.Items.Count;
			if (count == 0)
				return singleObjectFieldContext;
			for (int i = 0; i < count; i++) {
				FieldDataMemberInfoItem item = path.Items[i];
				singleObjectFieldContext = new SimplePropertyFieldContext(singleObjectFieldContext, item.FieldName);
				if (item.HasGroups || item.HasFilters) {
					ListFieldContext listContext = new ListFieldContext(singleObjectFieldContext, new ListParameters(item.Groups, item.FilterProperties));
					singleObjectFieldContext = new SingleListItemFieldContext(listContext, 0, 0, 1);					
				}
			}
			return singleObjectFieldContext;
		}
		#endregion
		string GetWrapPath(FieldPathDataMemberInfo path) {
			int count = path.Items.Count;
			if (count <= 1)
				return null;
			FieldDataMemberInfoItem item = path.Items[count - 1];
			if (!String.IsNullOrEmpty(item.FieldName) && !item.HasFilters && !item.HasGroups)
				return item.FieldName;
			else
				return null;
		}
		IDataEnumerator GetMasterListEnumerator(ISingleObjectFieldContext singleObjectFieldContext, FieldPathDataMemberInfo path) {			
			Debug.Assert(singleObjectFieldContext.ListContext == null);			
			int count = path.Items.Count;			
			object source = this.GetObjectValue(singleObjectFieldContext, path);
			if (source == null) {
				return null;
			}
			ISingleObjectFieldContext listParentContext = singleObjectFieldContext;
			if (count > 0 && !String.IsNullOrEmpty(path.Items[0].FieldName))
				listParentContext = new SimplePropertyFieldContext(singleObjectFieldContext, path.Items[0].FieldName);
			ListFieldContext listContext = new ListFieldContext(listParentContext, count > 0 ? new ListParameters(path.Items[0].Groups, path.Items[0].FilterProperties) : null);
			string pathToRoot = GetFullPathToRoot(singleObjectFieldContext);
			if (count > 0)
				pathToRoot = PathHelper.Join(pathToRoot, FieldPathService.EncodePath(path.Items[0].FieldName));
			SnapListController masterListController = GetMasterListController(listContext);
			if (masterListController == null) {
				if (!path.IsEmpty && path.LastItem.HasFilters)
					return null;
				else
					return new SingleValueDataEnumerator(singleObjectFieldContext, source);
			}
			if (count > 1) {
				if (!path.LastItem.HasFilters && masterListController.Columns[path.LastItem.FieldName] != null) {					
					SingleListDataEnumerator dataEnumerator = new SingleListDataEnumerator(listContext, masterListController);
					return new DataEnumeratorWrapper(dataEnumerator, path.LastItem.FieldName);
				}
				path.Items.RemoveAt(0);
				return new MultilistDataEnumerator(this, listContext, masterListController, path);
			}
			else {
				SingleListDataEnumerator dataEnumerator = new SingleListDataEnumerator(listContext, masterListController);
				return dataEnumerator;
			}
		}
		IDataEnumerator GetDetailListEnumerator(ISingleObjectFieldContext singleObjectFieldContext, FieldPathDataMemberInfo path) {
			Debug.Assert(singleObjectFieldContext.ListContext != null);			
			string pathToDetail = GetFullPathToParentList(singleObjectFieldContext);
			string fullPathToRoot = GetFullPathToRoot(singleObjectFieldContext);
			int count = path.Items.Count;
			if (count > 0) {
				List<string> fieldNames = new List<string>();
				path.Items.ForEach(f => fieldNames.Add(f.FieldName));
				pathToDetail = PathHelper.Join(pathToDetail, string.Join(".", fieldNames));
			}
			ISingleObjectFieldContext listParentContext = singleObjectFieldContext;
			if (!String.IsNullOrEmpty(pathToDetail))
				listParentContext = new SimplePropertyFieldContext(singleObjectFieldContext, pathToDetail);
			FieldDataMemberInfoItem lastPathItem = null;
			if(count > 0)
				lastPathItem = path.Items[path.Items.Count - 1];
			ListFieldContext listContext = new ListFieldContext(listParentContext, count > 0 ? new ListParameters(lastPathItem.Groups, lastPathItem.FilterProperties) : null);
			string realPathToRoot = !string.IsNullOrEmpty(fullPathToRoot) ? string.Join(".", fullPathToRoot, pathToDetail) : pathToDetail;
			SnapListController detailListController = GetDetailListController(listParentContext.ListContext, pathToDetail, realPathToRoot, count > 0 ? new ListParameters(lastPathItem.Groups, lastPathItem.FilterProperties) : null, singleObjectFieldContext.RowHandle);
				if(detailListController == null) {
					SnapListController masterController = GetListController(listParentContext.ListContext);
					if(masterController == null)
						return null;
					if(String.IsNullOrEmpty(pathToDetail))
						return new SingleValueDataEnumerator(listParentContext, masterController.GetItem(listParentContext.RowHandle));
					else {
						return new SingleValueDataEnumerator(listParentContext, masterController.GetColumnValue(listParentContext.RowHandle, pathToDetail));
					}
				}
				SingleListDataEnumerator result = new SingleListDataEnumerator(listContext, detailListController);
				return result;
		}
		SnapListController GetMasterListController(object source, IEnumerable<ICalculatedField> calculatedFields, ParameterCollection parameters, ListParameters listParameters) {
			SnapListController controller = new SnapListController();
			if (!controller.Update(source, listParameters, calculatedFields, parameters, GetNestedFields()))
				return null;
			return controller;
		}
		protected virtual SnapListController GetMasterListController(IDataControllerListFieldContext listContext) {
			Debug.Assert(listContext.Parent.ListContext == null);
			SnapListController controller;
			lock (masterControllersCache) {
				if (masterControllersCache.TryGetValue(listContext, out controller))
					return controller;
				string pathToRoot = GetFullPathToRoot(listContext);
				object source = this.GetObjectValue(listContext.Parent.Root, pathToRoot);
				if (source == null)
					return null;
				controller = new SnapListController();
				if (!controller.Update(source, listContext.ListParameters, GetFilteredCalculatedFields(listContext.Parent.Root.Source, pathToRoot), dataContextService.Parameters, GetNestedFields()))
					return null;
				masterControllersCache.Add(listContext, controller);
			}
			return controller;
		}
		string[] GetNestedFields() {
			if (FieldNames == null)
				return null;
			List<string> nestedFields = new List<string>();
			foreach (string item in this.FieldNames) {
				if (!string.IsNullOrEmpty(item) && item.Contains("."))
					nestedFields.Add(item);
			}
			return nestedFields.ToArray();
		}
		IEnumerable<ICalculatedField> GetFilteredCalculatedFields(object dataSource, string path) {
			ICollection<ICalculatedField> calculatedFields = dataSourceDispatcher.GetCalculatedFields(dataSource);
			List<ICalculatedField> filteredCalculatedFields = new List<ICalculatedField>();
			foreach (ICalculatedField field in calculatedFields) {
				if ((string.IsNullOrEmpty(field.DataMember) && string.IsNullOrEmpty(path)) || string.Compare(field.DataMember, path, true) == 0)
					filteredCalculatedFields.Add(field);
			}
			return filteredCalculatedFields.ToArray();
		}
		SnapListController lastCreatedListController;
		IDataControllerListFieldContext prevListContext;
		public virtual SnapListController GetListController(IDataControllerListFieldContext listContext) {
			IDataControllerListFieldContext parentListContext = listContext.Parent.ListContext;
			if (parentListContext == null) {
				return GetMasterListController(listContext);
			}
			else {
				if (lastCreatedListController != null && listContext == prevListContext) {
					return lastCreatedListController;
				}
				string pathToDetail = GetFullPathToParentList(listContext);
				string pathToRoot = GetFullPathToRoot(listContext);
				lastCreatedListController = GetDetailListController(parentListContext, pathToDetail, pathToRoot, listContext.ListParameters, listContext.Parent.RowHandle);
				prevListContext = listContext;
				return lastCreatedListController;
			}
		}
		SnapListController GetDetailListController(IDataControllerListFieldContext listContext, string pathToDetail, string pathToRoot, ListParameters listParameters, int controllerRow) {
			ListFieldContext listFieldContext = listContext as ListFieldContext;
			if (listFieldContext == null)
				Exceptions.ThrowInternalException();
			SnapListController masterController = GetListController(listContext);
			if (masterController == null)
				return null;
			return masterController.CreateDetailController(pathToDetail, listParameters, controllerRow, GetFilteredCalculatedFields(listContext.Parent.Root.Source, pathToRoot), GetNestedFields());
		}
		StringBuilder sb = new StringBuilder();
		string GetFullPathToRoot(IDataControllerFieldContext singleObjectFieldContext) {
			GetFullPathToRootVisitor visitor = new GetFullPathToRootVisitor(sb);
			singleObjectFieldContext.Accept(visitor);
			return sb.ToString();
		}
		protected virtual string GetFullPathToParentList(ISingleObjectFieldContext singleObjectFieldContext) {
			GetFullPathToParentListVisitor visitor = new GetFullPathToParentListVisitor(sb, singleObjectFieldContext.ListContext);
			singleObjectFieldContext.Accept(visitor);
			return sb.ToString();
		}
		protected virtual string GetFullPathToParentList(IDataControllerListFieldContext listContext) {
			GetFullPathToParentListVisitor visitor = new GetFullPathToParentListVisitor(sb, listContext.Parent.ListContext);
			listContext.Accept(visitor);
			return sb.ToString();
		}
		object GetObjectValue(ISingleObjectFieldContext singleObjectFieldContext, FieldPathDataMemberInfo path) {
			Debug.Assert(singleObjectFieldContext.ListContext == null);
			int count = path.Items.Count;
			string pathToRoot = GetFullPathToRoot(singleObjectFieldContext);
			if (count > 0)
				pathToRoot = PathHelper.Join(pathToRoot, FieldPathService.EncodePath(path.Items[0].FieldName));
			object source = singleObjectFieldContext.Root.Source;
			if (Object.ReferenceEquals(source, null))
				return String.IsNullOrEmpty(pathToRoot) ? null : FieldNull.Value;
			if(String.IsNullOrEmpty(pathToRoot))
				return source;
			string[] parts = FieldPathService.SplitPath(pathToRoot);
			return GetObjectValueCore(source, parts);
		}
		object GetObjectValue(RootFieldContext root, string path) {
			object source = root.Source;
			if (String.IsNullOrEmpty(path) || Object.ReferenceEquals(source, null)) {
				if (source is DataSet)
					source = DevExpress.Data.Helpers.MasterDetailHelper.GetDataSource(source, String.Empty);
				return source;
			}
			string[] parts = FieldPathService.SplitPath(path);
			return GetObjectValueCore(source, parts);
		}
		object GetObjectValueCore(object source, string[] parts) {
			for (int i = 0; i < parts.Length; i++) {
				if (source == null)
					return null;
				string part = parts[i];
				System.Data.DataSet dataSet = source as System.Data.DataSet;
				if (dataSet != null) {
					source = DevExpress.Data.Helpers.MasterDetailHelper.GetDataSource(source, part);
					continue;
				}
				IDataComponent component = source as IDataComponent;
				if (component != null) {
					SnapDataContext context = new SnapDataContext();
					DataBrowser browser = context.GetDataBrowserSafe(component, part);
					if (browser != null) {
						source = browser.GetValue();
						continue;
					}
				}
				System.ComponentModel.PropertyDescriptor pd = System.ComponentModel.TypeDescriptor.GetProperties(source)[part];
				if (pd == null) {
					IList iListSource = source as IList;
					if (iListSource != null && iListSource.Count > 0) {
						object firstItem = iListSource[0];
						if (firstItem != null) {
							pd = System.ComponentModel.TypeDescriptor.GetProperties(iListSource[0])[part];
							if (pd == null)
								return null;
							source = pd.GetValue(iListSource[0]);
						}
						else
							return null;
					}
				}
				else
					source = pd.GetValue(source);
			}
			return source;
		}
		object GetColumnValue(ISingleObjectFieldContext singleObjectFieldContext, FieldPathDataMemberInfo path) {
			Debug.Assert(singleObjectFieldContext.ListContext != null);
			string columnName = GetFullPathToParentList(singleObjectFieldContext);
			int count = path.Items.Count;
			if (count > 0)
				columnName = PathHelper.Join(columnName, path.Items[0].FieldName);
			SnapListController controller = GetListController(singleObjectFieldContext.ListContext);
			if(object.ReferenceEquals(controller, null))
				return null;
			if (!String.IsNullOrEmpty(columnName))
				return controller.GetColumnValue(singleObjectFieldContext.RowHandle, columnName);
			else
				return controller.GetItem(singleObjectFieldContext.RowHandle);
		}
		public void PrepareDataContext(DataContext dataContext, IFieldContext fieldContext) {
			ISingleObjectFieldContext singleObjectFieldContext = fieldContext as ISingleObjectFieldContext;
			if (singleObjectFieldContext == null)
				return;
			DataBrowser dataBrowser = dataContext.GetDataBrowser(singleObjectFieldContext.Root.Source, String.Empty, true);			
			if (dataBrowser != null && !(fieldContext is RootFieldContext))
				dataBrowser.Position = singleObjectFieldContext.VisibleIndex;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DataControllerCalculationContext() {
			Dispose(false);
		}
		void Dispose(bool disposing) {
			if (disposing) {
				foreach (SnapListController controller in this.masterControllersCache.Values)
					controller.Dispose();
			}
		}
		#endregion
	}
	public class DataEnumeratorWrapper : IDataEnumerator {
		readonly IDataEnumerator dataEnumerator;
		readonly string path;
		public DataEnumeratorWrapper(IDataEnumerator dataEnumerator, string path) {
			Guard.ArgumentNotNull(dataEnumerator, "dataEnumerator");
			Guard.ArgumentIsNotNullOrEmpty(path, "path");
			this.dataEnumerator = dataEnumerator;
			this.path = path;
		}
		public ISingleObjectFieldContext CreateFieldContext() {
			ISingleObjectFieldContext context = dataEnumerator.CreateFieldContext();
			return new SimplePropertyFieldContext(context, path);
		}
		public object Current { get { return dataEnumerator.GetColumnValue(path); } }
		public bool GroupStart { get { return dataEnumerator.GroupStart; } }
		public void Dispose() {			
			dataEnumerator.Dispose();
		}
		public bool MoveNext() {
			return dataEnumerator.MoveNext();
		}
		public void Reset() {
			dataEnumerator.Reset();
		}
		public object GetColumnValue(string columnName) {
			return dataEnumerator.GetColumnValue(PathHelper.Join(path, columnName));
		}
		public List<GroupProperties> GetChangedGroupsWithTemplates() {
			return dataEnumerator.GetChangedGroupsWithTemplates();
		}
		public IDataControllerListFieldContext GetListFieldContext() {
			return dataEnumerator.GetListFieldContext();
		}
	}
	public class SingleListDataEnumerator : IDataEnumerator {
		readonly IDataControllerListFieldContext listContext;
		readonly SnapListController listController;
		GroupRowInfo lastGroupRowInfo;
		List<GroupProperties> changedGroups;		
		int visibleIndex;
		int rowHandle;
		int currentGroupStartPosition = 0;
		bool groupChanged;
		public SingleListDataEnumerator(IDataControllerListFieldContext listContext, SnapListController listController) {
			Guard.ArgumentNotNull(listContext, "listContext");
			Guard.ArgumentNotNull(listController, "listController");
			this.listContext = listContext;
			this.listController = listController;
			Reset();
		}
		public IDataControllerListFieldContext ListContext { get { return listContext; } }
		public SnapListController ListController { get { return listController; } }
		public ISingleObjectFieldContext CreateFieldContext() {
			return new SingleListItemFieldContext(listContext, visibleIndex, rowHandle, rowHandle - currentGroupStartPosition + 1 );
		}
		public object Current {
			get { return listController.GetItem(rowHandle); }
		}
		public bool GroupStart { get { return groupChanged; } }
		public void Dispose() {
		}
		public bool MoveNext() {	   
			groupChanged = false;
			changedGroups = null;
			while (visibleIndex < listController.Count) {
				visibleIndex++;
				int controllerRowHandle = listController.GetControllerRowHandle(visibleIndex);
				if (controllerRowHandle == DevExpress.Data.DataController.InvalidRow)
					continue;
				if (controllerRowHandle >= 0) {
					if (groupChanged) {
						OnGroupChanged(listController.GetParentGroupRow(controllerRowHandle));
						if (this.changedGroups.Count > 0)
							this.currentGroupStartPosition = controllerRowHandle;
					}
					this.rowHandle = controllerRowHandle;
					return true;
				}
				groupChanged = true;
			}
			return false;
		}
		public void Reset() {
			visibleIndex = -1;
			groupChanged = false;
		}
		public object GetColumnValue(string columnName) {
			return listController.GetColumnValue(rowHandle, columnName);
		}
		public List<GroupProperties> GetChangedGroupsWithTemplates() {
			return changedGroups;			
		}
		void OnGroupChanged(GroupRowInfo groupRowInfo) {
			changedGroups = new List<GroupProperties>();
			ListParameters listParameters = listContext.ListParameters;
			if (listParameters == null || listParameters.Groups == null || listParameters.Groups.Count == 0)
				return;
			List<GroupProperties> groups = listParameters.Groups;
			int changedGroupCount = GetChangedGroupCount(lastGroupRowInfo, groupRowInfo);
			int addedGroupCount = 0;
			int groupIndex = groups.Count - 1;
			while (addedGroupCount < changedGroupCount && groupIndex >= 0) {
				GroupProperties groupProperties = groups[groupIndex];
				if (groupProperties.HasGroupTemplates)
					changedGroups.Add(groupProperties);
				addedGroupCount += groupProperties.GroupFieldInfos.Count;
				groupIndex--;
			}
			changedGroups.Reverse();
			lastGroupRowInfo = groupRowInfo;
		}
		int GetChangedGroupCount(GroupRowInfo lastGroupRowInfo, GroupRowInfo currentGroupRowInfo) {
			if (lastGroupRowInfo == null)
				return currentGroupRowInfo.Level + 1;
			int result = 0;
			Debug.Assert(currentGroupRowInfo.Level == lastGroupRowInfo.Level);
			while (lastGroupRowInfo != currentGroupRowInfo) {
				result++;
				currentGroupRowInfo = currentGroupRowInfo.ParentGroup;
				lastGroupRowInfo = lastGroupRowInfo.ParentGroup;
			}
			return result;
		}
		public IDataControllerListFieldContext GetListFieldContext() {
			return ListContext;
		}
	}
	public class MultilistDataEnumerator : IDataEnumerator {
		IDataEnumerator[] innerEnumerators;
		DataControllerCalculationContext calculationContext;
		ListFieldContext listContext;
		SnapListController masterListController;
		FieldPathDataMemberInfo path;
		public MultilistDataEnumerator(DataControllerCalculationContext calculationContext, ListFieldContext listContext, SnapListController masterListController, FieldPathDataMemberInfo path) {
			this.listContext = listContext;
			this.path = path;
			this.masterListController = masterListController;
			this.calculationContext = calculationContext;
		}
		public object Current { get { return innerEnumerators[innerEnumerators.Length - 1].Current; } }
		public bool GroupStart { get { return innerEnumerators[innerEnumerators.Length - 1].GroupStart; } }
		public bool MoveNext() {
			if (innerEnumerators == null) {
				innerEnumerators = new IDataEnumerator[path.Items.Count + 1];
				if (!RecreateInnerEnumerators(0))
					return false;
				return true;
			}
			int index = innerEnumerators.Length - 1;
			if (innerEnumerators[index].MoveNext())
				return true;
			index--;
			while (index >= 0) {				  
				if (innerEnumerators[index].MoveNext()) {
					if (!RecreateInnerEnumerators(index + 1))
						continue;
					return true;
				}
				else
					index--;
			}
			return false;
		}
		public ISingleObjectFieldContext CreateFieldContext() {
			return innerEnumerators[innerEnumerators.Length - 1].CreateFieldContext();
		}
		public void Reset() {
			innerEnumerators = null;
		}
		public void Dispose() {
		}
		bool RecreateInnerEnumerators(int invalidEnumeratorIndex) {
			int count = innerEnumerators.Length;
			for (int i = invalidEnumeratorIndex; i < count; i++) {
				if (innerEnumerators[i] != null) {
					innerEnumerators[i].Dispose();
					innerEnumerators[i] = null;
				}
				innerEnumerators[i] = CreateInnerEnumerator(i > 0 ? innerEnumerators[i - 1] : null, i);
				if (!innerEnumerators[i].MoveNext())
					return false;
			}
			return true;
		}
		private IDataEnumerator CreateInnerEnumerator(IDataEnumerator parentEnumerator, int index) {
			if (parentEnumerator != null) {
				Debug.Assert(index > 0);
				FieldDataMemberInfoItem pathItem = path.Items[index - 1];
				ISingleObjectFieldContext singleObjectFieldContext = parentEnumerator.CreateFieldContext();
				ListParameters listParameters = null;
				Debug.Assert(pathItem.HasFilters || pathItem.HasGroups);
				if (!String.IsNullOrEmpty(pathItem.FieldName))
					singleObjectFieldContext = new SimplePropertyFieldContext(singleObjectFieldContext, pathItem.FieldName);
				listParameters = new ListParameters(pathItem.Groups, pathItem.FilterProperties);
				ListFieldContext innerListContext = new ListFieldContext(singleObjectFieldContext, listParameters);
				SnapListController controller = calculationContext.GetListController(innerListContext);
				return new SingleListDataEnumerator(innerListContext, controller);
			}
			else {
				return new SingleListDataEnumerator(listContext, masterListController);
			}
		}		
		public object GetColumnValue(string columnName) {
			return innerEnumerators[innerEnumerators.Length - 1].GetColumnValue(columnName);
		}
		public List<GroupProperties> GetChangedGroupsWithTemplates() {
			return innerEnumerators[innerEnumerators.Length - 1].GetChangedGroupsWithTemplates();
		}
		public IDataControllerListFieldContext GetListFieldContext() {
			return innerEnumerators[innerEnumerators.Length - 1].GetListFieldContext();
		}
	}
	public class GetFullPathToParentListVisitor : IFieldContextVisitor {
		readonly IDataControllerListFieldContext parentListContext;
		readonly StringBuilder sb;
		public GetFullPathToParentListVisitor(StringBuilder sb, IDataControllerListFieldContext parentListContext) {
			this.parentListContext = parentListContext;
			this.sb = sb;
			sb.Length = 0;
		}
		public void Visit(EmptyFieldContext context) {
			throw new NotImplementedException();
		}
		public void Visit(ProxyFieldContext context) {
			throw new NotImplementedException();
		}
		public void Visit(RootFieldContext context) {
		}
		public void Visit(SnapMailMergeRootFieldContext context) {
			Visit((RootFieldContext)context);
		}
		public void Visit(SingleListItemFieldContext context) {
			context.Parent.Accept(this);
		}
		public void Visit(ListFieldContext context) {
			if (context == parentListContext)
				return;
			else
				context.Parent.Accept(this);
		}
		public void Visit(SimplePropertyFieldContext context) {
			context.Parent.Accept(this);
			PathHelper.Join(sb, context.Path);
		}
	}
	public class GetFullPathToRootVisitor  : IFieldContextVisitor {
		readonly StringBuilder sb;
		public GetFullPathToRootVisitor(StringBuilder sb) {
			this.sb = sb;
			sb.Length = 0;
		}
		public void Visit(EmptyFieldContext context) {
			throw new NotImplementedException();
		}
		public void Visit(ProxyFieldContext context) {
			throw new NotImplementedException();
		}
		public void Visit(RootFieldContext context) {
		}
		public void Visit(SnapMailMergeRootFieldContext context) {
			Visit((RootFieldContext)context);
		}
		public void Visit(SingleListItemFieldContext context) {			
			context.Parent.Accept(this);
		}
		public void Visit(ListFieldContext context) {
			context.Parent.Accept(this);			
		}
		public void Visit(SimplePropertyFieldContext context) {
			context.Parent.Accept(this);
			PathHelper.Join(sb, FieldPathService.EncodePath(context.Path));
		}
	}	
	public class RootFieldContext : ISingleObjectFieldContext, IDisposable {
		readonly IDataSourceDispatcher dataSourceDispatcher;
		readonly string dataSourceName;
		public RootFieldContext(IDataSourceDispatcher dataSourceDispatcher, string dataSourceName) {
			dataSourceDispatcher.IncRefCount();
			this.dataSourceDispatcher = dataSourceDispatcher;
			this.dataSourceName = dataSourceName;
		}
		public IDataControllerFieldContext Parent { get { return null; } }
		public IDataControllerListFieldContext ListContext { get { return null; } }
		public int VisibleIndex { get { throw new NotImplementedException(); } }
		public int RowHandle { get { throw new NotImplementedException(); } }
		public int CurrentRecordIndex { get { return 1; } }
		public int CurrentRecordIndexInGroup { get { return 1; } }
		public RootFieldContext Root { get { return this; } }
		public virtual object Source { 
			get {
				if(object.ReferenceEquals(dataSourceName, null))
					return null;
				return dataSourceDispatcher.GetDataSource(dataSourceName); 
			} 
		}
		public void Accept(IFieldContextVisitor visitor) {
			visitor.Visit(this);
		}
		public T Accept<T>(IFieldContextVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;
			RootFieldContext other = obj as RootFieldContext;
			if (Object.ReferenceEquals(other, null))
				return false;
			return string.Equals(this.dataSourceName, other.dataSourceName);
		}
		public override int GetHashCode() {
			if (object.ReferenceEquals(dataSourceName, null))
				return 0;
			else
				return dataSourceName.GetHashCode();
		}
		#region IDispose
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				this.dataSourceDispatcher.Dispose();
		}
		~RootFieldContext() {
			Dispose(false);
		}
		#endregion
	}
	public class SnapMailMergeRootFieldContext : RootFieldContext {
		public SnapMailMergeRootFieldContext(IDataSourceDispatcher dataSourceDispatcher, string dataSourceName)
			: base(dataSourceDispatcher, dataSourceName) {
		}
		public override object Source {
			get {
				object source = base.Source;
				BindingSource bs = source as BindingSource;
				if (bs != null && string.IsNullOrEmpty(bs.DataMember))
					return bs.DataSource;
				return source;
			}
		}
	}
	public class ListFieldContext : IDataControllerListFieldContext {
		readonly ISingleObjectFieldContext parent;
		readonly ListParameters listParameters;
		public ListFieldContext(ISingleObjectFieldContext parent, ListParameters listParameters) {
			Guard.ArgumentNotNull(parent, "parent");			
			this.parent = parent;
			this.listParameters = listParameters;
		}		
		public ISingleObjectFieldContext Parent { get { return parent; } }
		public ListParameters ListParameters { get { return listParameters; } }
		public RootFieldContext Root { get { return Parent.Root; } }
		public void Accept(IFieldContextVisitor visitor) {
			visitor.Visit(this);
		}
		public T Accept<T>(IFieldContextVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;			
			ListFieldContext other = obj as ListFieldContext;
			if (Object.ReferenceEquals(other, null))
				return false;
			ListParameters listParameters1 = listParameters ?? ListParameters.Empty;
			ListParameters listParameters2 = other.ListParameters ?? ListParameters.Empty;
			return Object.Equals(parent, other.Parent) && Object.Equals(listParameters1, listParameters2);
		}
		public override int GetHashCode() {			
			return parent.GetHashCode() ^ (listParameters != null ? listParameters.GetHashCode() : 0);
		}
	}	
	public class SingleListItemFieldContext : ISingleObjectFieldContext {
		readonly IDataControllerListFieldContext parent;
		readonly int visibleIndex;
		readonly int rowHandle;
		readonly int currentrecordIndexInGroup;
		public SingleListItemFieldContext(IDataControllerListFieldContext parent, int visibleIndex, int rowHandle, int currentrecordIndexInGroup) {
			Guard.ArgumentNotNull(parent, "parent");
			this.parent = parent;
			this.visibleIndex = visibleIndex;
			this.rowHandle = rowHandle;
			this.currentrecordIndexInGroup = currentrecordIndexInGroup;
		}
		public IDataControllerFieldContext Parent { get { return parent; } }
		public IDataControllerListFieldContext ListContext { get { return parent; } }
		public int VisibleIndex { get { return visibleIndex; } }
		public int RowHandle { get { return rowHandle; } }
		public int CurrentRecordIndex { get { return rowHandle + 1; } }
		public RootFieldContext Root { get { return Parent.Root; } }
		public int CurrentRecordIndexInGroup { get { return currentrecordIndexInGroup; } }
		public void Accept(IFieldContextVisitor visitor) {
			visitor.Visit(this);
		}
		public T Accept<T>(IFieldContextVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;
			SingleListItemFieldContext other = obj as SingleListItemFieldContext;
			if (Object.ReferenceEquals(other, null))
				return false;
			return visibleIndex == other.VisibleIndex && rowHandle == other.RowHandle && Object.Equals(parent, other.Parent);
		}
		public override int GetHashCode() {
			return parent.GetHashCode() ^ visibleIndex ^ rowHandle;
		}
	}
	public class SimplePropertyFieldContext : ISingleObjectFieldContext {
		readonly ISingleObjectFieldContext parent;
		readonly string path;
		public SimplePropertyFieldContext(ISingleObjectFieldContext parent, string path) {
			Guard.ArgumentNotNull(parent, "parent");
			this.parent = parent;
			this.path = path;
		}
		public string Path { get { return path; } }
		public IDataControllerListFieldContext ListContext { get { return parent.ListContext; } }
		public IDataControllerFieldContext Parent { get { return parent; } }
		public int RowHandle { get { return parent.RowHandle; } }
		public int VisibleIndex { get { return parent.VisibleIndex; } }
		public int CurrentRecordIndex { get { return parent.CurrentRecordIndex; } }
		public int CurrentRecordIndexInGroup { get { return parent.CurrentRecordIndexInGroup; } }
		public RootFieldContext Root { get { return Parent.Root; } }
		public void Accept(IFieldContextVisitor visitor) {
			visitor.Visit(this);
		}
		public T Accept<T>(IFieldContextVisitor<T> visitor) {
			return visitor.Visit(this);
		}
		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(obj, this))
				return true;
			SimplePropertyFieldContext other = obj as SimplePropertyFieldContext;
			if (Object.ReferenceEquals(other, null))
				return false;
			return Object.Equals(path, other.Path) && Object.Equals(parent, other.Parent);
		}
		public override int GetHashCode() {
			return parent.GetHashCode();
		}
	}
}
