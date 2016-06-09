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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Data;
using DevExpress.XtraReports.Data;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.XtraReports {
	[Designer("DevExpress.XtraReports.Design.LightSwitchDataSourceDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),]
	public class LightSwitchDataSource : EntityBindingSource, IParameterSupplier {
		#region inner classes
		public class QueryParameterCollection : ParameterCollection, IList {
			bool IList.IsReadOnly {
				get { return true; }
			}
			public void AddRange(QueryParameter[] parameters) {
				foreach (QueryParameter item in parameters) {
					Add(item);
				}
			}
		}
		class LightSwitchDataSourceTypeConverter : TypeConverter {
			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
				if(context != null && context.Instance != null && destinationType == typeof(string)) {
					LightSwitchDataSource dataSource = (LightSwitchDataSource)context.Instance;
					if(dataSource.DataSource == null)
						return "(none)";
					return dataSource.CollectionName + (dataSource.IsQuery ? " (Query)" : " (Table)");
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if (sourceType == typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
		}
		#endregion
		string dataSourceName;
		string collectionName;
		private bool isQuery;
		readonly QueryParameterCollection parameters = new QueryParameterCollection();
		readonly List<object> lastUsedParameterValues = new List<object>();
		protected override bool CacheList {
			get {
				if(!IsQuery)
					return base.CacheList;
				return QueryParametersMatchLastUsedValues();
			}
		}
		public static event EventHandler<FillDataSourceEventArgs> FillDataSource;
		[
		Editor("DevExpress.XtraReports.Design.LightSwitch.EntityDataSourceEditor," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(UITypeEditor)), CategoryAttribute("Data"),
		TypeConverter(typeof(LightSwitchDataSourceTypeConverter))]
		public override object DataSource {
			get {
				return base.DataSource;
			}
			set {
				base.DataSource = value;
			}
		}
		bool ShouldSerializeDataSource() {
			return DataSource != null;
		}
		void ResetDataSource() { 
		}
		[DefaultValue(""), Browsable(false)]
		public string DataSourceName {
			get {
				return dataSourceName;
			}
			set {
				dataSourceName = value;
			}
		}
		[DefaultValue(""), Browsable(false)]
		public string CollectionName {
			get {
				return collectionName;
			}
			set {
				collectionName = value;
			}
		}
		[DefaultValue(false), Browsable(false)]
		public bool IsQuery {
			get {
				return isQuery;
			}
			set {
				isQuery = value;
			}
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.LightSwitch.QueryParameterCollectionEditor," + AssemblyInfo.SRAssemblyReportsDesignFull, typeof(System.Drawing.Design.UITypeEditor)),
		]
		public QueryParameterCollection QueryParameters {
			get { return parameters; }
		}
		protected override IList GetRuntimeList() {
			if (FillDataSource != null) {
				FillDataSourceEventArgs e = new FillDataSourceEventArgs(null, DataSourceName, CollectionName, IsQuery, new ReadOnlyCollection<Parameter>(QueryParameters));
				FillDataSource(this, e);
				SaveLastUsedParameterValues();
				return e.List;
			}
			return null;
		}
		bool QueryParametersMatchLastUsedValues() {
			if(QueryParameters.Count != lastUsedParameterValues.Count)
				return false;
			for(int i = 0; i < QueryParameters.Count; i++) {
				if(!object.Equals(QueryParameters[i].Value, lastUsedParameterValues[i]))
					return false;
			}
			return true;
		}
		void SaveLastUsedParameterValues() {
			lastUsedParameterValues.Clear();
			foreach(Parameter parameter in QueryParameters) {
				lastUsedParameterValues.Add(parameter.Value);
			}
		}
		#region IParameterSupplier Members
		public ICollection<Parameter> GetParameters() {
			return QueryParameters;
		}
		IEnumerable<IParameter> IParameterSupplierBase.GetIParameters() {
			return QueryParameters;
		}
		#endregion
	}
	public class QueryParameter : Parameter {
		class QueryParameterTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if (sourceType != typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
		}
		[
		TypeConverter(typeof(QueryParameterTypeConverter))
		]
		public override Type Type {
			get {
				return base.Type;
			}
			set {
				base.Type = value;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override LookUpSettings LookUpSettings {
			get {
				return base.LookUpSettings;
			}
			set {
				base.LookUpSettings = value;
			}
		}
	}
}
