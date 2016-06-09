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

using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Data.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
namespace DevExpress.XtraReports.UI {
	public abstract class DataContainerComponent : Component, IDataContainer {
		#region Fields & Properties
		object dataSource;
		string dataMember = string.Empty;
		[SRCategory(ReportStringId.CatData)]
		[Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))]
		[TypeConverter(typeof(DataSourceConverter))]
		[XtraSerializableProperty(XtraSerializationVisibility.Reference)]
		public virtual object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		[SRCategory(ReportStringId.CatData)]
		[TypeConverter(typeof(DevExpress.XtraReports.Design.DataMemberTypeConverter))]
		[Editor("DevExpress.XtraReports.Design.DataContainerDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor))]
		[DefaultValue("")]
		[XtraSerializableProperty]
		public virtual string DataMember {
			get { return dataMember; }
			set { dataMember = value ?? string.Empty; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public virtual bool CanSetPropertyValue {
			get { return Parent != null; }
		}
		protected abstract IDataContainer Parent {
			get;
		}
		object ParentDataSource {
			get { return (Parent != null) ? Parent.GetEffectiveDataSource() : null; }
		}
		object NativeDataSource {
			get { return (dataSource != ParentDataSource) ? dataSource : null; }
		}
		object IDataContainer.DataAdapter {
			get { return null; }
			set { }
		}
		#endregion
		#region Methods
		public void Assign(object dataSource, string dataMember) {
			Guard.ArgumentNotNull(dataMember, "dataMember");
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		internal void CopyDataProperties(DataContainerComponent source) {
			Assign(source.NativeDataSource, source.dataMember);
		}
		internal void AdjustDataSourse() {
			if(dataSource == ParentDataSource)
				dataSource = null;
		}
		object IDataContainer.GetEffectiveDataSource() {
			return GetEffectiveDataSource();
		}
		internal object GetEffectiveDataSource() {
			return dataSource ?? ParentDataSource;
		}
		internal string GetEffectiveDataMember() {
			return !string.IsNullOrEmpty(dataMember) ? dataMember : Parent != null ? Parent.DataMember : string.Empty;
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSourceHelper.ConvertToSerializableDataSource(dataSource);
		}
		protected bool ShouldSerializeDataSourceCore() {
			return dataSource is IComponent && !object.ReferenceEquals(dataSource, ParentDataSource);
		}
		protected bool ShouldSerializeXmlDataSourceCore() {
			return dataSource != null && !object.ReferenceEquals(dataSource, ParentDataSource);
		}
		protected bool EqualsCore(DataContainerComponent other) {
			return !object.ReferenceEquals(other, null)
				&& this.dataSource == other.dataSource
				&& this.dataMember == other.dataMember;
		}
		#endregion
	}
}
