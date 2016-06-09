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
using System.Text;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Utils;
using PivotFieldsObservableCollection = DevExpress.Xpf.Core.ObservableCollectionCore<DevExpress.Xpf.PivotGrid.PivotGridField>;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class FieldListFields : CustomizationFormFields {
		PivotFieldsObservableCollection[] fieldLists;
		PivotFieldsObservableCollection hiddenFields;
		PivotFieldsObservableCollection hiddenDataFields;
		PivotFieldsObservableCollection allFields;
		PivotFieldsReadOnlyObservableCollection[] publicLists;
		PivotFieldsReadOnlyObservableCollection hiddenFieldsCache;
		PivotFieldsReadOnlyObservableCollection hiddenDataFieldsCache;
		PivotFieldsReadOnlyObservableCollection allFieldsCache;
		public FieldListFields(PivotGridWpfData data)
			: base(data) {			
		}
		protected PivotFieldsObservableCollection HiddenFieldsInternal {
			get {
				if(hiddenFields == null)
					hiddenFields = new PivotFieldsObservableCollection();
				return hiddenFields;
			}
		}
		protected PivotFieldsObservableCollection HiddenDataFieldsInternal {
			get {
				if(hiddenDataFields == null)
					hiddenDataFields = new PivotFieldsObservableCollection();
				return hiddenDataFields;
			}
		}
		protected PivotFieldsObservableCollection AllFieldsInternal {
			get {
				if(allFields == null)
					allFields = new PivotFieldsObservableCollection();
				return allFields;
			}
		}
		public new PivotFieldsReadOnlyObservableCollection HiddenFields {
			get {
				if(hiddenFieldsCache == null)
					hiddenFieldsCache = new PivotFieldsReadOnlyObservableCollection(HiddenFieldsInternal);
				return hiddenFieldsCache;
			}
		}
		public PivotFieldsReadOnlyObservableCollection HiddenDataFields {
			get {
				if(hiddenDataFieldsCache == null)
					hiddenDataFieldsCache = new PivotFieldsReadOnlyObservableCollection(HiddenDataFieldsInternal);
				return hiddenDataFieldsCache;
			}
		}
		public PivotFieldsReadOnlyObservableCollection AllFields {
			get {
				if(allFieldsCache == null)
					allFieldsCache = new PivotFieldsReadOnlyObservableCollection(AllFieldsInternal);
				return allFieldsCache;
			}
		}
		public PivotFieldsReadOnlyObservableCollection this[FieldArea area] {
			get {
				if(publicLists == null)
					publicLists = new PivotFieldsReadOnlyObservableCollection[Helpers.GetEnumValues(typeof(FieldArea)).Length];
				if(publicLists[(int)area] == null)
					publicLists[(int)area] = new PivotFieldsReadOnlyObservableCollection(GetFieldList(area));
				return publicLists[(int)area];
			}
		}
		public bool IsFieldInArea(PivotGridField movedField, FieldArea toArea) {
			foreach(FieldArea area in Helpers.GetEnumValues(typeof(FieldArea)))
				if(this[area].IndexOf(movedField) > -1 && area == toArea)
					return true;
			return false;
		}
		protected PivotFieldsObservableCollection GetFieldList(FieldArea area) {
			if(fieldLists == null)
				fieldLists = new PivotFieldsObservableCollection[Helpers.GetEnumValues(typeof(FieldArea)).Length];
			if(fieldLists[(int)area] == null)
				fieldLists[(int)area] = new PivotFieldsObservableCollection();
			return fieldLists[(int)area];
		}
		protected override void GetFieldsFromData() {
			base.GetFieldsFromData();
			GetFieldsFromCustomizatoinFormFields();
		}
		protected override void OnListsChanged() {
			base.OnListsChanged();
			if(DeferUpdates)
				GetFieldsFromCustomizatoinFormFields();
		}
		internal void GetFieldsFromCustomizatoinFormFields() {
			foreach(PivotArea area in Helpers.GetEnumValues(typeof(PivotArea))) {
				PivotFieldsObservableCollection collection = GetFieldList(area.ToFieldArea());
				CopyCollection(collection, base.GetFieldList(area));
			}
			CopyCollection(HiddenFieldsInternal, base.HiddenFields, true);
			CopyHiddenDataFields();
			CopyAllFields();
		}
		void CopyHiddenDataFields() {
			List<PivotGridField> fields = new List<PivotGridField>();
			fields.AddRange(this[FieldArea.DataArea]);
			for(int i = 0; i < HiddenFields.Count; i++)
				if(HiddenFields[i].Area == FieldArea.DataArea)
					fields.Add(HiddenFields[i]);
		   fields.Sort(new StringComparer());
		   CopyCollection(HiddenDataFieldsInternal, fields);
		}
		void CopyAllFields() {
			List<PivotGridField> fields = new List<PivotGridField>();
			fields.AddRange(HiddenFields);
			fields.AddRange(this[FieldArea.ColumnArea]);
			fields.AddRange(this[FieldArea.DataArea]);
			fields.AddRange(this[FieldArea.FilterArea]);
			fields.AddRange(this[FieldArea.RowArea]);
			fields.Sort(new StringComparer());
			CopyCollection(AllFieldsInternal, fields);
		}
		void CopyCollection(PivotFieldsObservableCollection collection, IList<PivotGridField> baseList) {
			collection.BeginUpdate();
			collection.Clear();
			for(int i = 0; i < baseList.Count; i++) {
				collection.Add(baseList[i]);
			}
			collection.EndUpdate();
		}
		void CopyCollection(PivotFieldsObservableCollection collection, IList<PivotFieldItemBase> baseList, bool sort = false) {
			if(sort)
				baseList = GetSortedList(baseList);
			collection.BeginUpdate();
			collection.Clear();
			for(int i = 0; i < baseList.Count; i++) {
				collection.Add(((PivotFieldItem)baseList[i]).Wrapper);
			}
			collection.EndUpdate();
		}
		IList<PivotFieldItemBase> GetSortedList(IList<PivotFieldItemBase> baseList) {
			List<PivotFieldItemBase> list = new List<PivotFieldItemBase>();
			foreach(DevExpress.XtraPivotGrid.Data.PivotFieldItemBase item in baseList) {
				list.Add((PivotFieldItemBase)item);
			}
			list.Sort(delegate(PivotFieldItemBase x, PivotFieldItemBase y) {
				return Comparer<string>.Default.Compare(((PivotFieldItem)x).Wrapper.DisplayText,
					((PivotFieldItem)y).Wrapper.DisplayText);
			});
			return list;
		}
		class StringComparer : IComparer<PivotGridField> {
			int IComparer<PivotGridField>.Compare(PivotGridField x, PivotGridField y) {
				return Comparer.Default.Compare(x.ToString(), y.ToString());
			}
		}
	}
}
