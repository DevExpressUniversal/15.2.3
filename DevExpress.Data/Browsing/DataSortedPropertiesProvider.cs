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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.Browsing;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using System.ComponentModel;
namespace DevExpress.XtraReports.Design {
	public enum ShowComplexProperties { Default, First, Last }
}
namespace DevExpress.XtraReports.Native.Data {
	public class DataContextServiceBase : IDisposable, IDataContextService {
		internal const SortOrder DefaultSortOrder = SortOrder.Ascending;
		protected Dictionary<DataContextOptions, DataContext> dictionary = new Dictionary<DataContextOptions, DataContext>();
		SortOrder propertiesSortOrder = DefaultSortOrder;
		ShowComplexProperties showComplexProperties = ShowComplexProperties.First;
		public ShowComplexProperties ShowComplexProperties {
			get { return showComplexProperties; }
			set { showComplexProperties = value; }
		}
		public SortOrder PropertiesSortOrder {
			get { return propertiesSortOrder; }
			set { propertiesSortOrder = value; }
		}
		protected virtual IEnumerable<ICalculatedField> CalculatedFields {
			get { return null; }
		}
		protected virtual bool SuppressListFilling { get { return false; } }
		public DataContextServiceBase() {
		}
		public event EventHandler<DataContextFilterPropertiesEventArgs> PrefilterProperties;
		protected void DisposeDataContext() {
			foreach(DataContext dataContext in dictionary.Values)
				dataContext.Dispose();
			dictionary.Clear();
		}
		public virtual void Dispose() {
			DisposeDataContext();
		}
		PropertyDescriptor[] IDataContextService.FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
			if(PrefilterProperties != null) {
				List<PropertyDescriptor> props = new List<PropertyDescriptor>(properties);
				PrefilterProperties(this, new DataContextFilterPropertiesEventArgs(props, dataSource, dataMember, dataContext));
				return FilterProperties(props.ToArray(), dataSource, dataMember, dataContext);
			}
			return FilterProperties(properties, dataSource, dataMember, dataContext);
		}
		protected virtual PropertyDescriptor[] FilterProperties(PropertyDescriptor[] properties, object dataSource, string dataMember, DataContext dataContext) {
			return properties;
		}
		public virtual void SortProperties(IPropertyDescriptor[] properties) {
			if(propertiesSortOrder == SortOrder.None && showComplexProperties == ShowComplexProperties.Default)
				return;
			IPropertyDescriptor[] array = new IPropertyDescriptor[properties.Length]; 
			properties.CopyTo(array, 0);
			Array.Sort<IPropertyDescriptor>(properties, delegate(IPropertyDescriptor x, IPropertyDescriptor y) {
				int result = showComplexProperties == ShowComplexProperties.First ? Comparer<int>.Default.Compare(ToFactor(x), ToFactor(y)) :
					showComplexProperties == ShowComplexProperties.Last ? Comparer<int>.Default.Compare(ToFactor(y), ToFactor(x)) :
					0;
				return result != 0 ? result :
					propertiesSortOrder == SortOrder.Ascending ? string.Compare(x.DisplayName, y.DisplayName) :
					propertiesSortOrder == SortOrder.Descending ? string.Compare(y.DisplayName, x.DisplayName) :
					Comparer<int>.Default.Compare(Array.IndexOf<IPropertyDescriptor>(array, x), Array.IndexOf<IPropertyDescriptor>(array, y));
			});
		}
		static int ToFactor(IPropertyDescriptor property) {
			Type propertyType = ((FakedPropertyDescriptor)property).RealProperty.PropertyType;
			return ListTypeHelper.IsListType(propertyType) && !DataContext.IsStandardType(propertyType) ? 0 :
				DataContext.IsComplexType(propertyType) ? 1 :
				2;
		}
		public DataContext CreateDataContext(DataContextOptions options, bool useCache) {
			if(useCache) {
				DataContext value;
				if(!dictionary.TryGetValue(options, out value)) {
					value = CreateDataContextInternal(options);
					dictionary.Add(options, value);
				} else {
					XRDataContextBase contextBase = value as XRDataContextBase;
					if(options.UseCalculatedFields && contextBase != null)
						contextBase.ApplyCalculatedFields(CalculatedFields);
				}
				return value;
			}
			return CreateDataContextInternal(options);
		}
		public DataContext CreateDataContext(DataContextOptions options) {
			return CreateDataContext(options, true);
		}
		protected virtual DataContext CreateDataContextInternal(DataContextOptions options) {
			return new XRDataContextBase(options.UseCalculatedFields ? CalculatedFields : null, SuppressListFilling);
		}
	}
}
