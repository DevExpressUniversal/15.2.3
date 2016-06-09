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

using System.Collections.Generic;
using DevExpress.Data;
using System;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql;
namespace DevExpress.DataAccess.Native {
	[TypeConverter("DevExpress.XtraReports.Design.ParameterValueEditorChangingConverter," + AssemblyInfo.SRAssemblyUtilsUI)]
	public class DataSourceParameterBase : DevExpress.Data.IParameter {
		public class BaseEqualityComparer : IEqualityComparer<DataSourceParameterBase> {
			public static bool Equals(IParameter x, IParameter y) {
				return string.Equals(x.Name, y.Name, StringComparison.Ordinal) && x.Type == y.Type && object.Equals(x.Value, y.Value);
			}
			#region IEqualityComparer<DataSourceParameterBase> Members
			bool IEqualityComparer<DataSourceParameterBase>.Equals(DataSourceParameterBase x, DataSourceParameterBase y) {
				return Equals(x, y);
			}
			int IEqualityComparer<DataSourceParameterBase>.GetHashCode(DataSourceParameterBase obj) {
				return 0;
			}
			#endregion
		}
		Type type;
		object value;
		[LocalizableCategory(DataAccessStringId.PropertyGridDesignCategoryName)]
		public string Name { get; set; }
		[DefaultValue(null)]
		[TypeConverter("DevExpress.DataAccess.UI.Native.Sql.QueryParameterTypeConverter," + AssemblyInfo.SRAssemblyDataAccessUI)]
		[RefreshProperties(RefreshProperties.All)]
		[LocalizableCategory(DataAccessStringId.PropertyGridDesignCategoryName)]
		public Type Type { 
			get { return type; }
			set {
				if(this.type == value)
					return;
				this.type = value;
				if(type == null || type == typeof(Expression))
					return;
				if(XtraReports.Parameters.ParameterHelper.ShouldConvertValue(this.value, this.type))
					this.value = XtraReports.Parameters.ParameterHelper.ConvertFrom(this.value, this.type, XtraReports.Parameters.ParameterHelper.GetDefaultValue(this.type));
			} 
		}
		[LocalizableCategory(DataAccessStringId.PropertyGridDesignCategoryName)]
		public object Value { 
			get {
				if(!string.IsNullOrEmpty(ValueInfo))
					this.value = XtraReports.Parameters.ParameterHelper.ConvertFrom(ValueInfo, Type, null);
				return this.value; 
			} 
			set { 
				this.value = value;
				ValueInfo = null;
			} 
		}
		bool ShouldSerializeValue() { return Type == typeof(Expression); }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string ValueInfo { get; set; }
		bool ShouldSerializeValueInfo() {
			if(Type == typeof(Expression))
				return false;
			if(string.IsNullOrEmpty(ValueInfo)) {
				ValueInfo = XtraReports.Parameters.ParameterHelper.ConvertValueToString(Value);
			}
			return !string.IsNullOrEmpty(this.ValueInfo);
		}
		public DataSourceParameterBase() : this(null, typeof(string), null) { }
		public DataSourceParameterBase(string name, Type type, object value) {
			this.Name = name;
			this.type = type;
			this.value = value;
		}
		internal XElement SaveToXml() {
			return QuerySerializer.SerializeParameter(null, this);
		}
		internal void LoadFromXMl(XElement element){
			QuerySerializer.DeserializeParameter(this, element, null);
		}
		internal static DataSourceParameterBase FromIParameter(IParameter value) {
			return value == null
				? null
				: (value as DataSourceParameterBase ?? new DataSourceParameterBase(value.Name, value.Type, value.Value));
		}
	}
}
