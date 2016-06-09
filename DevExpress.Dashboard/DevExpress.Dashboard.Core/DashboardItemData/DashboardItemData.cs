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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Data {
#if DEBUG
	[SuppressMessage("Microsoft.Design", "CA1039")]
#endif
	public class DashboardItemData : ReadOnlyTypedList {
		readonly ArrayList records = new ArrayList();
		public DashboardItemData(ReadOnlyTypedList source) {
			ITypedList sourceTypedList = (ITypedList)source;
			PropertyDescriptorCollection sourceProperties = sourceTypedList.GetItemProperties(null);
			PropertiesRepository properties = Properties;
			int index = 0;
			foreach (PropertyDescriptor propertyDescriptor in sourceProperties)
				properties.Add(new DashboardItemDataArrayPropertyDescriptor(index++, propertyDescriptor));
			object[] record;
			foreach (object item in source) {
				record = new object[index];
				for (int i = 0; i < index; i++)
					record[i] = sourceProperties[i].GetValue(item);
				records.Add(record);
			}
		}
		public DashboardItemData(Type dataType, ICollection data) {
			PropertiesRepository properties = Properties;
			foreach (PropertyInfo propertyInfo in dataType.GetProperties())
				properties.Add(new DashboardItemDataObjectPropertyDescriptor(dataType, propertyInfo));
			records.AddRange(data);
		}
		protected override int GetItemsCount() {
			return records.Count;
		}
		protected override object GetItemValue(int index) {
			return records[index];
		}
	}
}
