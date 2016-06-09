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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.DataAccess.ConnectionParameters;
using System.Threading;
using System.Threading.Tasks;
namespace DevExpress.DashboardCommon {
	[
	DXToolboxItem(false),
	SuppressMessage("Microsoft.Design", "CA1039")
	]
	public class DashboardExcelDataSource : ExcelDataSource, IDashboardDataSource, IDashboardDataSourceInternal {
		const string xmlFilter = "Filter";
		string filter;
		readonly Locker loadingLocker = new Locker();
		readonly Locker pickManagerLocker = new Locker();
		readonly CalculatedFieldsController calculatedFieldsController;
		readonly DataSourceProperties properties;
		readonly DataSourceComponentNameController componentNameController;
		readonly PickManagerWithCalcFields pickManager;
		IEnumerable<IParameter> parameters;
		IStorage storage;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardExcelDataSourceCalculatedFields"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CalculatedFieldCollection CalculatedFields { get { return calculatedFieldsController.CalculatedFields; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardExcelDataSourceFilter"),
#endif
		Category(CategoryNames.Data),
		Editor("DevExpress.DashboardWin.Design.DataSourceFilterCriteriaEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		public string Filter { get { return filter; } set { filter = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardExcelDataSourceName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		Browsable(true)
		]
		public override string Name { get { return componentNameController.Name; } set { componentNameController.Name = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DashboardExcelDataSourceComponentName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(false),
		]
		public string ComponentName { get { return componentNameController.ComponentName; } set { componentNameController.ComponentName = value; } }
		string PickManagerDisplayName { get { return Name; } }
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<NameChangingEventArgs> NameChanging;
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		public DashboardExcelDataSource()
			: this(null) {
		}
		public DashboardExcelDataSource(string name)
			: base() {
			properties = new DataSourceProperties(this);
			calculatedFieldsController = new CalculatedFieldsController(this);
			pickManager = new PickManagerWithCalcFields(string.Empty, this, null, calculatedFieldsController);
			componentNameController = new DataSourceComponentNameController(name, loadingLocker, () => Site);
			componentNameController.NameChanged += componentNameController_NameChanged;
			componentNameController.NameChanging += componentNameController_NameChanging;
			componentNameController.CaptionChanged += componentNameController_CaptionChanged;
			calculatedFieldsController.ConstructTree += calculatedFieldsController_ConstructTree;
			ConstructTree();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override XElement SaveToXml() {
			XElement element = base.SaveToXml();
			componentNameController.SaveComponentNameToXml(element);
			calculatedFieldsController.SaveToXml(element);
			if(!string.IsNullOrEmpty(Filter))
				element.Add(new XElement(xmlFilter, Filter));
			return element;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void LoadFromXml(XElement element) {
			try {
				base.LoadFromXml(element);
			} catch(BadImageFormatException) {
			} catch(FileLoadException) {
			} catch(FileNotFoundException) {
			} catch(TypeLoadException) {
			}
			componentNameController.LoadComponentNameFromXml(element);
			calculatedFieldsController.LoadFromXml(element);
			XElement filterElement = element.Element(xmlFilter);
			if(filterElement != null)
				Filter = filterElement.Value;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void BeginInit() {
			loadingLocker.Lock();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void EndInit() {
			loadingLocker.Unlock();
		}
		protected override void Fill(IEnumerable<IParameter> sourceParameters) {
			base.Fill(sourceParameters);
			OnAfterFill(sourceParameters);
		}
		void OnAfterFill(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
			ConstructTree();
			storage = null;
			if(DataSourceDataChanged != null)
				DataSourceDataChanged(this, new DataChangedEventArgs(true));
		}
		internal void FillSync(IEnumerable<IParameter> parameters, IPlatformDependenciesService platformDependenciesService, CancellationToken cancellationToken) {
#if !DXPORTABLE
			Task<SelectedDataEx> task = FillAsync(cancellationToken);
			try {
				while(!task.IsCompleted) {
					if(platformDependenciesService != null)
						platformDependenciesService.DoEvents();
					Thread.Sleep(10);
				}
			}
			catch(AggregateException e) {
				if(task.IsFaulted)
					throw ExceptionHelper.Unwrap(e);
			}
			this.result.SetColumns(task.Result);
			OnAfterFill(parameters);
#else
			Fill(parameters);
#endif
		}
		void calculatedFieldsController_ConstructTree(object sender, EventArgs e) {
			ConstructTree();
		}
		void componentNameController_CaptionChanged(object sender, EventArgs e) {
			ConstructTree();
			if(CaptionChanged != null)
				CaptionChanged(this, e);
		}
		void componentNameController_NameChanging(object sender, NameChangingEventArgs e) {
			if(NameChanging != null)
				NameChanging(this, e);
		}
		void componentNameController_NameChanged(object sender, NameChangedEventArgs e) {
			if(NameChanged != null)
				NameChanged(this, e);
		}
		void ConstructTree() {
			if(pickManagerLocker.IsLocked)
				return;
			try {
				pickManagerLocker.Lock();
				pickManager.ConstructTree(PickManagerDisplayName);
			} finally {
				pickManagerLocker.Unlock();
			}
		}
		#region IDashboardDataSource
#if !DXPORTABLE
		IDataProvider IDashboardDataSource.DataProvider { get { return null; } set { throw new NotSupportedException(); } }
#pragma warning disable 612, 618
		OlapDataProvider IDashboardDataSource.OlapDataProvider { get { return null; } }
		SqlDataProvider IDashboardDataSource.SqlDataProvider { get { return null; } }
#pragma warning restore 612, 618
#endif
		bool IDashboardDataSource.HasDataProvider { get { return false; } }
		bool IDashboardDataSource.IsConnected { get { return true; } }
		bool IDashboardDataSource.IsServerModeSupported { get { return false; } }
		DataProcessingMode IDashboardDataSource.DataProcessingMode { get { return DataProcessingMode.Client; } set { throw new NotSupportedException(); } }
		IEnumerable<IParameter> IDashboardDataSource.Parameters { get { return parameters; } }
		object IDashboardDataSource.Data { get { return this; } set { throw new NotSupportedException(); } }
		IDataSourceSchema IDashboardDataSource.GetDataSourceSchema(string dataMember) {
			return pickManager;
		}
		ICalculatedFieldsController IDashboardDataSource.GetCalculatedFieldsController() {
			return calculatedFieldsController;
		}
		IDashboardDataSourceInternal IDashboardDataSource.GetDataSourceInternal() {
			return this;
		}
		#endregion
		#region IDashboardDataSourceInternal
		Dashboard IDashboardDataSourceInternal.Dashboard { get; set; }
		DataSourceProperties IDashboardDataSourceInternal.Properties { get { return properties; } }
		event EventHandler<DataChangedEventArgs> IDashboardDataSourceInternal.DataSourceDataChanged {
			add { DataSourceDataChanged += value; }
			remove { DataSourceDataChanged -= value; }
		}
		event EventHandler<NameChangedEventArgs> IDashboardDataSourceInternal.NameChanged {
			add { NameChanged += value; }
			remove { NameChanged -= value; }
		}
		event EventHandler IDashboardDataSourceInternal.CaptionChanged {
			add { CaptionChanged += value; }
			remove { CaptionChanged -= value; }
		}
		event EventHandler<DataProcessingModeChangedEventArgs> IDashboardDataSourceInternal.DataProcessingModeChanged { add { } remove { } }
		IPivotGridDataSource IDashboardDataSourceInternal.GetPivotDataSource(string dataMember) {
			return null;
		}
		IList IDashboardDataSourceInternal.GetListSource(string dataMember) {
			return ((IListSource)this).GetList();
		}
		IStorage IDashboardDataSourceInternal.GetStorage(string dataMember) {
			if(storage == null) {
				string[] columnNames = pickManager.GetDataMembers().ToArray();
				IList list = ((IListSource)this).GetList();
				ITypedList typedList = list as ITypedList;
				if(typedList != null) {
					using(
						DataReaderExEnumerableWrapper dataReaderEx =
						new DataReaderExEnumerableWrapper(typedList.GetItemProperties(null).Cast<PropertyDescriptor>().ToList(), columnNames, list)
					) {
						DataStorageProcessor processor = new DataStorageProcessor(dataReaderEx);
						storage = processor.ProcessTables();
					}
				}
			}
			return storage;
		}
		bool IDashboardDataSourceInternal.GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName) {
			return false;
		}
		Type IDashboardDataSourceInternal.ServerGetUnboundExpressionType(string expression, string queryName) {
			return typeof(object);
		}
		CalculatedFieldDataColumnInfo IDashboardDataSourceInternal.CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters) {
			return new CalculatedFieldDataColumnInfo(field, pickManager.RootNode, calculatedFieldsController.CalculatedFields, parameters, true);
		}
		bool IDashboardDataSourceInternal.ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName) {
			IDataSourceSchema dataSourceSchema = ((IDashboardDataSource)this).GetDataSourceSchema(queryName);
			return DataSourceHelper.ContainsDisplayMember(dataSourceSchema, displayMember, valueMember);
		}
		List<ViewModel.ParameterValueViewModel> IDashboardDataSourceInternal.GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider) {
			return DataSourceHelper.GetDynamicLookupValues(this, queryName, displayMember, valueMember);
		}
		string IDashboardDataSourceInternal.GetName_13_1() {
			return componentNameController.Name_13_1;
		}
		bool IDashboardDataSourceInternal.IsSqlServerMode(string queryName) {
			return false;
		}
		IEnumerable<string> IDashboardDataSourceInternal.GetDataSets() {
			return new string[] { string.Empty };
		}
		object IDashboardDataSourceInternal.GetDataSchema(string dataMember) {
			return this;
		}
		void IDashboardDataSourceInternal.SetParameters(IEnumerable<IParameter> parameters) {
			this.parameters = parameters;
		}
		#endregion
		#region IDashboardComponent
		event EventHandler<NameChangingEventArgs> IDashboardComponent.NameChanging {
			add { NameChanging += value; }
			remove { NameChanging -= value; }
		}
		#endregion
		#region ISupportPrefix
		string ISupportPrefix.Prefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultExcelDataSourceName); } }
		#endregion        
	}
	public class ExcelDataSourceConnectionParameters : FileConnectionParametersBase {
		public ExcelDataSourceConnectionParameters(string fileName)
			: this(fileName, null) {
		}
		public ExcelDataSourceConnectionParameters(string fileName, string password):base(fileName,password) {			
		}
	}
 }
