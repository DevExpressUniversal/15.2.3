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
using System.ComponentModel;
using System.Linq;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.DataAccess.Sql.DataApi;
namespace DevExpress.DataAccess.Native.Sql {
	public class ResultColumn : PropertyDescriptor, IColumn, ICloneable {
		readonly Type propertyType;
		string displayName;
		public ResultColumn(string name, Type propertyType, IList values)
			: base(name, new Attribute[] { }) {
			this.propertyType = propertyType;
			Values = values;
		}
		public int Count {
			get { return Values != null ? Values.Count : 0; }
		}
		public IList Values { get; set; }
		#region Overrides of PropertyDescriptor
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) {
			if(Values == null)
				return null;
			return Values[((ResultRow)component).Index] ?? DBNull.Value;
		}
		public override void ResetValue(object component) { throw new NotSupportedException(); }
		public override void SetValue(object component, object value) { throw new NotSupportedException(); }
		public override bool ShouldSerializeValue(object component) { return false; }
		public override Type ComponentType { get { return typeof(ResultRow); } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return this.propertyType; } }
		#endregion
		#region Overrides of MemberDescriptor
		public override string DisplayName { get { return this.displayName ?? base.DisplayName; } }
		#endregion
		public void SetDisplayName(string displayName) { this.displayName = displayName; }
		public object Clone() {
			return new ResultColumn(Name, PropertyType, null);
		}
		#region Implementation of IColumn
		Type IColumn.Type { get { return PropertyType; } }
		T IColumn.GetValue<T>(IRow row) { return ((IColumn)this).GetValue<T>(((ResultRow)row).Index); }
		T IColumn.GetValue<T>(int rowIndex) { return Values.Cast<T>().ElementAt(rowIndex); }
		object IColumn.this[int rowIndex] { get { return ((IColumn)this).GetValue<object>(rowIndex); } }
		#endregion
	}
}
