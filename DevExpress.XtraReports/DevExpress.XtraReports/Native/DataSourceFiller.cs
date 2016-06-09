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
using System.Text;
using DevExpress.XtraReports.UI;
using System.Reflection;
using System.Data;
using System.ComponentModel;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Data.Helpers;
using System.Security.Permissions;
using System.Data.Common;
using DevExpress.Data.Browsing;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraPrinting.Preview;
using System.Threading;
namespace DevExpress.XtraReports.Native {
	public abstract class DataSourceFiller {
		public static DataTable ForceDataTable(object dataSource, string dataMember) {
			return DataSetHelper.GetDataTable(dataSource as DataSet, dataMember);			
		}
		static object GetEffectiveDataSource(IDataContainerBase dataContainerBase) {
			return dataContainerBase is IDataContainer ? ((IDataContainer)dataContainerBase).GetEffectiveDataSource() : dataContainerBase.DataSource;
		}
		public static DataSourceFiller CreateInstance(IDataContainerBase dataContainerBase, IServiceProvider servProvider, bool skipIfFilled) {
			IListAdapter adapter = GetEffectiveDataSource(dataContainerBase) as IListAdapter;
			if(adapter != null)
				return skipIfFilled && adapter.IsFilled ? null : new ListAdapterFiller(adapter, servProvider);
			if(dataContainerBase is IDataContainer) {
				ReflectionPermissionFlag flag = Enum.IsDefined(typeof(ReflectionPermissionFlag), ReflectionPermissionFlag.RestrictedMemberAccess) ?
					ReflectionPermissionFlag.RestrictedMemberAccess :
					ReflectionPermissionFlag.MemberAccess;
				return !SecurityHelper.IsPermissionGranted(new ReflectionPermission(flag)) ?
					 new PartialTrustDataSourceFiller((IDataContainer)dataContainerBase, servProvider) :
					 new FullTrustDataSourceFiller((IDataContainer)dataContainerBase, servProvider);
			}
			return null;
		}
		public void Execute() {
			try {
				ExecuteCore();
			} catch(Exception ex) {
				string s = String.Format("{0} \r\n{1}", Localization.ReportLocalizer.GetString(Localization.ReportStringId.Msg_FillDataError), ex.Message);
				throw new Exception(s, ex);
			}
		}
		protected abstract void ExecuteCore();
	}
	public class ListAdapterFiller : DataSourceFiller {
		IListAdapter adapter;
		IServiceProvider servProvider;
		ICancellationService cancelationService;
		ICancellationService CancelationService { 
			get {
				if(cancelationService == null)
					cancelationService = servProvider.GetService(typeof(ICancellationService)) as ICancellationService;
				return cancelationService;
			}
		}
		public ListAdapterFiller(IListAdapter adapter, IServiceProvider servProvider) {
			this.adapter = adapter;
			this.servProvider = servProvider;
		}
		protected override void ExecuteCore() {
			if(servProvider != null && adapter is IListAdapterAsync) {
				IBackgroundService backgroundService = servProvider.GetService(typeof(IBackgroundService)) as IBackgroundService;
				if(backgroundService != null && CancelationService.CanBeCanceled()) {
					CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancelationService.TokenSource.Token);
					try {
						FillAsynch((IListAdapterAsync)adapter, backgroundService, linkedTokenSource.Token);
					} finally {
						linkedTokenSource.Dispose();
					}
					return;
				}
			}
			adapter.FillList(servProvider);
		}
		void FillAsynch(IListAdapterAsync asyncAdapter, IBackgroundService backgroundService, CancellationToken token) {
			IAsyncResult result = asyncAdapter.BeginFillList(servProvider, token);
			while(!result.IsCompleted) {
				backgroundService.PerformAction();
				Thread.Sleep(1);
			}
			asyncAdapter.EndFillList(result);
		}
	}
	public class FullTrustDataSourceFiller : DataSourceFiller {
		IServiceProvider servProvider;
		IDataContainer dataContainer;
		public FullTrustDataSourceFiller(IDataContainer dataContainer, IServiceProvider servProvider) {
			this.dataContainer = dataContainer;
			this.servProvider = servProvider;
		}
		protected override void ExecuteCore() {
			FillDataSource(dataContainer.GetEffectiveDataSource(), dataContainer.DataMember, dataContainer.DataAdapter);
		}
		protected virtual void FillDataSource(object dataSource, string dataMember, object dataAdapter) {
			IDataAdapter realDataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(dataAdapter);
			FillDataSource(DevExpress.Data.Native.BindingHelper.ConvertToDataSet(dataSource), dataMember, realDataAdapter);
		}
		static void FillDataSource(object dataSource, string dataMember, IDataAdapter dataAdapter) {
			if(dataAdapter == null)
				return;
			DataTable dataTable = ForceDataTable(dataSource, dataMember);
			DataSet dataSet = dataTable != null ? dataTable.DataSet : dataSource as DataSet;
			if(dataSet == null)
				return;
			DataTable targetTable = FindDataTable(dataSet.Tables, dataAdapter.TableMappings);
			if(targetTable != null) {
				if(DataTableIsEmpty(targetTable))
					dataAdapter.Fill(dataSet);
			} if(dataTable != null) {
				if(DataTableIsEmpty(dataTable) && dataAdapter is DbDataAdapter)
					((DbDataAdapter)dataAdapter).Fill(dataTable);
			}
		}
		protected static bool DataTableIsEmpty(DataTable dataTable) {
			return dataTable.Rows.Count == 0;
		}
		static DataTable FindDataTable(DataTableCollection tables, ITableMappingCollection mappings) {
			foreach(DataTableMapping mapping in mappings) {
				DataTable dataTable = tables[mapping.DataSetTable];
				if(dataTable != null)
					return dataTable;
			}
			return null;
		}
	}
	public class PartialTrustDataSourceFiller : FullTrustDataSourceFiller {
		#region static
		static MethodInfo GetFillMethod(object dataAdapter) {
			System.Reflection.MethodInfo[] mInfos = dataAdapter.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
			foreach(MethodInfo mi in mInfos) {
				object[] objs = mi.GetCustomAttributes(typeof(DataObjectMethodAttribute), false);
				if(objs != null && objs.Length > 0 && objs[0] is DataObjectMethodAttribute && ((DataObjectMethodAttribute)objs[0]).MethodType == DataObjectMethodType.Fill)
					return mi;
			}
			return null;
		}
		static bool IsValidDataAdapter(object obj) {
			DataObjectAttribute attrib = TypeDescriptor.GetAttributes(obj)[typeof(DataObjectAttribute)] as DataObjectAttribute;
			return attrib != null && attrib.IsDataObject && GetFillMethod(obj) != null;
		}
		#endregion
		public PartialTrustDataSourceFiller(IDataContainer dataContainer, IServiceProvider servProvider) : base(dataContainer, servProvider) {
		}
		protected override void FillDataSource(object dataSource, string dataMember, object dataAdapter) {
			if(DevExpress.Data.Native.BindingHelper.IsDataObject(dataAdapter)) {
				MethodInfo fillMethodInfo = GetFillMethod(dataAdapter);
				if(fillMethodInfo == null || fillMethodInfo.GetParameters().Length > 1)
					return;
				DataTable table = GetDataTable(dataSource, dataMember);
				if(table != null && DataTableIsEmpty(table))
					fillMethodInfo.Invoke(dataAdapter, new object[] { table });
			} else
				base.FillDataSource(dataSource, dataMember, dataAdapter);
		}
		static DataTable GetDataTable(object dataSource, string dataMember) {
			return dataSource is DataTable ? (DataTable)dataSource : 
				ForceDataTable(DevExpress.Data.Native.BindingHelper.ConvertToDataSet(dataSource), dataMember);
		}
	}
}
