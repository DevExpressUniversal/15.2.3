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
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System.Collections.Generic;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridSerializationHelper<T> where T : PivotGridFieldBase {
		protected readonly PivotGridData data;
		public PivotGridSerializationHelper(PivotGridData data) {
			this.data = data;
		}
		protected PivotGridFieldCollectionBase Fields { get { return data.Fields; } }
		protected PivotGridGroupCollection Groups { get { return data.Groups; } }
		public object FindField(XtraItemEventArgs e) {
			if(e.Item.ChildProperties == null) return null;
			string name = null;
			XtraPropertyInfo xp = e.Item.ChildProperties["Name"];
			if(xp != null && xp.Value != null) name = xp.Value.ToString();
			if(string.IsNullOrEmpty(name)) return null;
			T field = (T)Fields.GetFieldByName(name);
			return field;
		}
		public bool ClearFields(XtraItemEventArgs e) {
			PivotGridOptionsLayout gridOptions = e.Options as PivotGridOptionsLayout;
			bool addNewColumns = (gridOptions != null && gridOptions.Columns.AddNewColumns);
			List<T> commonFields = GetCommonFields(e.Item.ChildProperties);
			List<T> newFields = GetNewFields(commonFields);
			if(!addNewColumns) {
				RemoveNewFields(newFields);
			} else {
				SetNew(newFields);
			}
			return true;
		}
		List<T> GetNewFields(List<T> savingFields) {
			List<T> newFields = new List<T>();
			for(int i = Fields.Count - 1; i >= 0; i--) {
				T field = (T)Fields[i];
				if(!savingFields.Contains(field))
					newFields.Add(field);
			}
			return newFields;
		}
		void SetNew(List<T> newFields) {
			foreach(T field in newFields)
				field.IsNew = true;
		}
		List<T> GetCommonFields(IXtraPropertyCollection layoutItems) {
			List<T> commonFields = new List<T>();
			if(layoutItems == null)
				return commonFields;
			foreach(XtraPropertyInfo pInfo in layoutItems) {
				T commonField = FindField(new XtraItemEventArgs(this, Fields, pInfo)) as T;
				if(commonField != null)
					commonFields.Add(commonField);
			}
			return commonFields;
		}
		void RemoveNewFields(List<T> newFields) {
			foreach(T field in newFields)
				Fields.Remove(field);
		}
		public bool ClearGroups(XtraItemEventArgs e) {
			PivotGridOptionsLayout gridOptions = e.Options as PivotGridOptionsLayout;
			bool clearGroups = gridOptions == null ? true : !gridOptions.AddNewGroups;
			if(clearGroups)
				Groups.Clear();
			return true;
		}
		public object DeserializeField(XtraItemEventArgs e) {
			PivotGridOptionsLayout gridOptions = e.Options as PivotGridOptionsLayout;
			if(gridOptions != null) {
				XtraPropertyInfo propertyInfo = e.Item.ChildProperties["Name"];
				string name = propertyInfo != null ? propertyInfo.ValueToObject(typeof(string)) as string : null;
				if(gridOptions.Columns.RemoveOldColumns) return null;
			}
			return Fields.Add();
		}
		public object DeserializeGroup(XtraItemEventArgs e) {
			return Groups.Add();
		}
	}
}
