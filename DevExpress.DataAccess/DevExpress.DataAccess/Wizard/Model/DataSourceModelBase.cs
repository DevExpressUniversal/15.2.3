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
using System.ComponentModel;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Wizard.Model {
	public abstract class DataSourceModelBase {
		#region static
		static int CompareColumnInfo(ColumnInfo columnInfo1, ColumnInfo columnInfo2) {
			int res = string.CompareOrdinal(columnInfo1.Name, columnInfo2.Name);
			if(res != 0)
				return res;
			res = string.CompareOrdinal(columnInfo1.DisplayName, columnInfo2.DisplayName);
			if(res != 0)
				return res;
			if(columnInfo1.TypeSpecifics != columnInfo2.TypeSpecifics)
				return -1;
			return 0;
		}
		static List<ColumnInfo> GetColumnInfos(PropertyDescriptorCollection properties) {
			List<ColumnInfo> columnInfos = new List<ColumnInfo>();
			TypeSpecificsService typeSpecificsService = new TypeSpecificsService();
			foreach(PropertyDescriptor propertyDescriptor in properties) {
				columnInfos.Add(new ColumnInfo() { Name = propertyDescriptor.Name, DisplayName = propertyDescriptor.DisplayName, TypeSpecifics = typeSpecificsService.GetPropertyTypeSpecifics(propertyDescriptor) });
			}
			return columnInfos;
		}
		#endregion
		public object DataSchema { get; set; }
		protected DataSourceModelBase() { }
		protected DataSourceModelBase(DataSourceModelBase other) {
			DataSchema = other.CloneDataSchema();
		}
		public abstract object Clone(); 
		protected object CloneDataSchema() {
			object dataSchema = DataSchema;
			if(dataSchema == null)
				return null;
			DataContextBase context = new DataContextBase();
			PropertyDescriptorCollection properties = context[dataSchema].GetItemProperties();
			DataView dataView = new DataView(new SelectStatementResultRow[0]);
			foreach(PropertyDescriptor property in properties) {
				dataView.ColumnDescriptors.Add(property);
			}
			return dataView;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj))
				return true;
			DataSourceModelBase model = obj as DataSourceModelBase;
			if(model == null)
				return false;
			if((DataSchema == null && model.DataSchema != null) || (DataSchema != null && model.DataSchema == null))
				return false;
			if(DataSchema != null && model.DataSchema != null) {
				DataContextBase dataContext = new DataContextBase();
				PropertyDescriptorCollection properties = dataContext[DataSchema].GetItemProperties();
				List<ColumnInfo> columnInfos = GetColumnInfos(properties);
				PropertyDescriptorCollection modelProperties = dataContext[model.DataSchema].GetItemProperties();
				List<ColumnInfo> modelColumnInfos = GetColumnInfos(modelProperties);
				if(columnInfos.Count != modelColumnInfos.Count)
					return false;
				columnInfos.Sort(CompareColumnInfo);
				modelColumnInfos.Sort(CompareColumnInfo);
				for(int i = 0; i < columnInfos.Count; i++) {
					if(!columnInfos[i].Equals(modelColumnInfos[i]))
						return false;
				}
			}
			return true;
		}
		public override int GetHashCode() {
			return 0;
		}
	}
}
