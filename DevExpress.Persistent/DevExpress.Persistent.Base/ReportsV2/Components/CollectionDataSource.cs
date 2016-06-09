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
using System.ComponentModel;
using System.Drawing;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Utils.Design;
namespace DevExpress.Persistent.Base.ReportsV2 {
	[ToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(XafAssemblyInfo.DXTabXafReports)]
	[Description("The data source component that loads a collection of business objects via the Object Space.")]
	[ToolboxBitmap(typeof(DevExpress.Persistent.Base.ResFinder), "Resources.CollectionDataSource.ico")]
	[ToolboxBitmap24("DevExpress.Persistent.Base.Resources.CollectionDataSource_24x24.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	[ToolboxBitmap32("DevExpress.Persistent.Base.Resources.CollectionDataSource_32x32.png, DevExpress.Persistent.Base" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix)]
	public class CollectionDataSource : DataSourceBase, ISortingPropertyDescriptorProvider {
		public CollectionDataSource()
			: base() {
		}
		protected override object CreateDesignTimeDataSource() {
			return CreatePropertyDescriptorProvider(ObjectTypeName, true);
		}
		protected virtual ITypedList CreatePropertyDescriptorProvider(string objectTypeName, bool showCollectionProperty) {
			return new CollectionPropertyDescriptorProvider(objectTypeName, this.Name, this, showCollectionProperty);
		}
		protected override object CreateViewDataSource() {
			object result = null;
			if(ObjectSpace != null) {
				Type objectType = DataType;
				if(objectType != null) {
					result = ObjectSpace.CreateCollection(objectType);
					if(Sorting.Count > 0) {
						ObjectSpace.SetCollectionSorting(result, Sorting);
					}
					ObjectSpace.SetTopReturnedObjectsCount(result, TopReturnedRecords);
					ObjectSpace.ApplyCriteria(result, DataSourceCriteria);
					if(result is ProxyCollection) {
						((ProxyCollection)result).Refresh();
					}
					else {
						ITypesInfo typesInfoService = GetService(typeof(ITypesInfo)) as ITypesInfo;
						if(typesInfoService != null) {
							result = new ProxyCollection(ObjectSpace, typesInfoService.FindTypeInfo(objectType), result);
						}
					}
				}
			}
			return result;
		}
		#region IPropertyDescriptorProvider Members
		ITypedList ISortingPropertyDescriptorProvider.SortingProperties {
			get {
				return CreatePropertyDescriptorProvider(ObjectTypeName, false);
			}
		}
		#endregion
	}
}
