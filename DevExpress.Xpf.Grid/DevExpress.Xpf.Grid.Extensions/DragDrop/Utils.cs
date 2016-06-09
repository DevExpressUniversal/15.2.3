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

using DevExpress.Data.Access;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#else
using System.Data;
#endif
namespace DevExpress.Xpf.Grid.DragDrop {
	public static class Utils {
		public static object GetPropertyValue(object obj, string propertyName) {
			PropertyDescriptor property = GetPropertyDescriptor(obj, propertyName);
			if(property != null)
				return property.GetValue(obj);
			else return null;
		}
		public static void SetPropertyValue(object obj, string propertyName, object value) {
#if !SL
			DataRow rowView = obj as DataRow;
			if(rowView != null) {
				rowView[propertyName] = value;
				return;
			}
#endif
			PropertyDescriptor property = GetPropertyDescriptor(obj, propertyName);
			if(property != null)
				property.SetValue(obj, value);
		}
		static PropertyDescriptor GetPropertyDescriptor(object obj, string propertyName) {
			if(propertyName.Contains(".")) {
				return new DevExpress.Data.Access.ComplexPropertyDescriptorReflection(obj, propertyName);
			}
			else
				return TypeDescriptor.GetProperties(obj)[propertyName];
		}
#if !SL
		public static DataRow CloneDataRow(DataView dataView, object sourceMngObj) {
			DataTable table = dataView.Table;
			DataRow row = table.NewRow();
			foreach(DataColumn column in table.Columns)
				row[column] = Utils.GetPropertyValue(sourceMngObj, column.ColumnName) ?? DBNull.Value;
			return row;
		}
#endif
	}
}
