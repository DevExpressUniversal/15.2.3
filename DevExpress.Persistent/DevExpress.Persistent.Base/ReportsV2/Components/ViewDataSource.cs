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
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.Persistent.Base.ReportsV2 {
	[ToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafReports)]
	[Description("The data source component that retrieves a list of data records (a data view) via the Object Space without loading complete business classes.")]
	[ToolboxBitmap(typeof(DevExpress.Persistent.Base.ResFinder), "Resources.ViewDataSource.ico")]
	[ToolboxBitmap24("DevExpress.Persistent.Base.Resources.ViewDataSource_24x24.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	[ToolboxBitmap32("DevExpress.Persistent.Base.Resources.ViewDataSource_32x32.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	public class ViewDataSource : DataSourceBase, ISortingPropertyDescriptorProvider, IXtraSupportDeserializeCollectionItem {
		ViewPropertiesCollection props;
		public ViewDataSource()
			: base() {
			props = new ViewPropertiesCollection(this);
		}
		[Category("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !SL
	[DevExpressPersistentBaseLocalizedDescription("ViewDataSourceProperties")]
#endif
		[XtraSerializableProperty(XtraSerializationVisibility.Collection,true)]
		public ViewPropertiesCollection Properties {
			get {
				return props;
			}
		}
		public void RefreshProperties() {
			RefreshViewDataSource();
			RefreshDesignTimeDataSource();
		}
		protected override object CreateDesignTimeDataSource() {
			return CreatePropertyDescriptorProvider(ObjectTypeName, Properties);
		}
		protected override object CreateViewDataSource() {
			object result = null;
			if(ObjectSpace != null && Properties.Count > 0) {
				Type objectType = DataType;
				if(objectType != null) {
					result = ObjectSpace.CreateDataView(objectType, CollectDataViewExpressions(), DataSourceCriteria, Sorting);
					ObjectSpace.SetTopReturnedObjectsCount(result, TopReturnedRecords);
				}
			}
			return result;
		}
		protected virtual ITypedList CreatePropertyDescriptorProvider(string objectTypeName, ViewPropertiesCollection properties) {
			return new ViewPropertyDescriptorProvider(this.Name, properties);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(props != null) {
					props.Clear();
				}
				props = null;
			}
		}
		private IList<DataViewExpression> CollectDataViewExpressions() {
			IList<DataViewExpression> propertyCriteriaOperators = new List<DataViewExpression>();
			for(int i = 0; i < Properties.Count; i++) {
				DataViewExpression dataViewExpression = new DataViewExpression(Properties[i].DisplayName, Properties[i].Expression);
				propertyCriteriaOperators.Add(dataViewExpression);
			}
			return propertyCriteriaOperators;
		}
		#region IPropertyDescriptorProvider Members
		ITypedList ISortingPropertyDescriptorProvider.SortingProperties {
			get {
				return CreatePropertyDescriptorProvider(ObjectTypeName, Properties);
			}
		}
		#endregion
		#region IXtraSupportDeserializeCollectionItem Members
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "Properties") {
				return new ViewProperty();
			}
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == "Properties" && e.Item !=null && e.Item.Value is ViewProperty) {
				props.Add((ViewProperty)e.Item.Value);
			}
		}
		#endregion
		#region ITypedList
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(listAccessors != null) {
				return new PropertyDescriptorCollection(null);
			}
			return ((ITypedList)DataSource).GetItemProperties(listAccessors);
		}
		#endregion
	}
}
