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
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraEditors.Controls;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.Design;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.Data.Browsing;
using DevExpress.Data.Native;
using DevExpress.Utils;
using DevExpress.Data.Browsing.Design;
using System.Linq;
using DevExpress.Utils.UI;
namespace DevExpress.XtraReports.Native.Data {
	public class DataSourceProxy : FilterFormParametersOwner, IFilteredComponent {
		class CustomDataContextUtils : DataContextUtils {			
			public CustomDataContextUtils(DataContext dataContext) : base(dataContext) {
			}
			protected override PropertyDescriptor[] FilterProperties(PropertyDescriptorCollection properties, object dataSource, string dataMember, Func<PropertyDescriptor, bool> predicate) {
				Attribute filterAttribute = new BrowsableAttribute(false);
				return base.FilterProperties(properties, dataSource, dataMember, delegate(PropertyDescriptor obj) {
					return !obj.Attributes.Contains(filterAttribute) && (predicate == null || predicate(obj));
				});
			}
		}
		object dataSource;
		string dataMember;
		IServiceProvider provider;
		IDataContextService dataContextService;
		IExtensionsProvider extensionsProvider;
		readonly bool canAddParameters;
		public object DataSource { get { return dataSource; } }
		public string DataMember { get { return dataMember; } }
		public DataSourceProxy(IServiceProvider provider, object dataSource, string dataMember, IList<IParameter> parameters, IExtensionsProvider extensionsProvider, bool canAddParameters)
			: base((IParameterCreator)provider.GetService(typeof(IParameterCreator)), parameters, canAddParameters) {
			this.canAddParameters = canAddParameters;
			this.provider = provider;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.extensionsProvider = extensionsProvider;
			this.dataContextService = (IDataContextService)provider.GetService(typeof(IDataContextService));
			IServiceContainer servContainer = provider.GetService(typeof(IServiceContainer)) as IServiceContainer;
			if(servContainer != null) {
				servContainer.RemoveService(typeof(DataSourceProxy));
				servContainer.AddService(typeof(DataSourceProxy), this);
			}
		}
		public DataSourceProxy(IServiceProvider provider, object dataSource, string dataMember, IEnumerable<IParameter> parameters, IExtensionsProvider extensionsProvider) : this(provider, dataSource, dataMember, new List<IParameter>(parameters), extensionsProvider, false) {
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add {}
			remove {}
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add {}
			remove {}
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get { return null; }
			set { }
		}
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			DataColumnInfo[] dataColumns = GetDataColumns(string.Empty);
			return new XRFilterColumnCollection(dataColumns, provider, extensionsProvider);
		}
		public DataColumnInfo[] GetDataColumns(string dataMember) {
			PropertyDescriptor[] properties = GetDisplayedProperties(BindingHelper.JoinStrings(".", this.dataMember, dataMember));
			if(properties != null) {
				List<DataColumnInfo> dataColumns = new List<DataColumnInfo>();
				foreach(PropertyDescriptor property in properties) {
					dataColumns.Add(new DataColumnInfo(property));
				}
				return dataColumns.ToArray();
			}
			return new DataColumnInfo[] { };
		}
		PropertyDescriptor[] GetDisplayedProperties(string dataMember) {
			using (DataContext dc = dataContextService.CreateDataContext(new DataContextOptions(true, true), false)) {
				return new CustomDataContextUtils(dc).GetDisplayedProperties(dataSource, dataMember, null);
			}
		}
	}
	class XRFilterColumnCollection : DataColumnInfoFilterColumnCollection, IServiceProvider {
		IServiceProvider serviceProvider;
		IExtensionsProvider extensionsProvider;
		public XRFilterColumnCollection(DataColumnInfo[] columns, IServiceProvider serviceProvider, IExtensionsProvider extensionsProvider) : base(columns) {
			this.serviceProvider = serviceProvider;
			this.extensionsProvider = extensionsProvider;
		}
		protected override FilterColumn CreateFilterColumn(DataColumnInfo column) {
			return new XRFilterColumn(column, null, this);
		}
		protected override void Fill(DataColumnInfo[] columns) {
		 	if(columns == null) return;
			foreach(DataColumnInfo column in columns) {
				FilterColumn filterColumn = CreateFilterColumn(column);
				if(!string.IsNullOrEmpty(((ITreeSelectableItem)filterColumn).Text))
					this.Add(filterColumn);
			}
		}
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if (serviceType == typeof(IExtensionsProvider))
				return extensionsProvider;
			return serviceProvider.GetService(serviceType);
		}
		#endregion
	}
	class XRFilterColumn : DataColumnInfoFilterColumn {
		IServiceProvider serviceProvider;
		FilterColumn parent;
		List<IBoundProperty> children;
		bool? hasChildren;
		public override bool IsAggregate {
			get {
				return false;
			}
		}
		public override bool IsList {
			get {
				return BindingHelper.IsList(this.Column.PropertyDescriptor);
			}
		}
		public override IBoundProperty Parent {
			get {
				return parent;
			}
		}
		public override List<IBoundProperty> Children {
			get {
				if (XRDataContextBase.IsStandardType(this.ColumnType))
					return null;
				if (children == null)
					children = CreateChildren();
				return children;
			}
		}
		public override bool HasChildren {
			get {
				if (hasChildren == null) {
					List<IBoundProperty> children = Children;
					hasChildren = children != null && children.Count > 0;
				}
				return hasChildren.Value;
			}
		}
		List<IBoundProperty> CreateChildren() {
			DataColumnInfo[] dataColumns = GetDataColumns();
			if(dataColumns != null) {
				List<IBoundProperty> result = new List<IBoundProperty>();
				foreach(DataColumnInfo item in dataColumns) {
					FilterColumn filterColumn = new XRFilterColumn(item, this, this.serviceProvider);
					if(!string.IsNullOrEmpty(((ITreeSelectableItem)filterColumn).Text))
						result.Add(filterColumn);
				}
				return result;
			}
			return null;
		}
		DataColumnInfo[] GetDataColumns() {
			DataSourceProxy dataSource = this.serviceProvider.GetService(typeof(DataSourceProxy)) as DataSourceProxy;
			if(dataSource != null) {
				string dataMember = GetDataMemberRecurcive(string.Empty);
				return dataSource.GetDataColumns(dataMember);
			}
			return null;
		}
		string GetDataMemberRecurcive(string dataMember) {
			string value = BindingHelper.JoinStrings(".", this.Column.PropertyDescriptor.Name, dataMember);
			return Parent != null ? ((XRFilterColumn)Parent).GetDataMemberRecurcive(value) : value;
		}
		public XRFilterColumn(DataColumnInfo column, FilterColumn parent, IServiceProvider serviceProvider) : base(column, false) {
			this.serviceProvider = serviceProvider;
			this.parent = parent;
		}
		protected override RepositoryItem CreateRepository() {
			IExtensionsProvider extensionsProvider = (IExtensionsProvider)serviceProvider.GetService(typeof(IExtensionsProvider));
			if (extensionsProvider != null) {
				EditingContext editingContext = new EditingContext(extensionsProvider.Extensions[DataEditorService.Guid], extensionsProvider);
				RepositoryItem repositoryItem = DataEditorService.GetRepositoryItem(ColumnType, Column, editingContext);
				if (repositoryItem != null)
					return repositoryItem;
			}
			return DataEditorService.GetRepositoryItem(ColumnType);
		}
	}
}
