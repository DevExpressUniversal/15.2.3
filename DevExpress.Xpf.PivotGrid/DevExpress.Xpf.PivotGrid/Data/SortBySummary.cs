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

using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Windows;
using System;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid;
using DevExpress.Utils.Serializing;
using System.Collections.Specialized;
#if SL
using DXFrameworkContentElement = System.Windows.FrameworkElement;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class SortByCondition : DXFrameworkContentElement {
		#region statics
		public static readonly DependencyProperty FieldProperty;
		public static readonly DependencyProperty ValueProperty;
		public static readonly DependencyProperty OlapUniqueMemberNameProperty;
		static SortByCondition() {
			Type ownerType = typeof(SortByCondition);
			FieldProperty = DependencyProperty.Register("Field", typeof(PivotGridField), 
				ownerType, new PropertyMetadata(null));
			ValueProperty = DependencyProperty.Register("Value", typeof(object),
				ownerType, new PropertyMetadata(null));
			OlapUniqueMemberNameProperty = DependencyProperty.Register("OlapUniqueMemberName", typeof(string),
				ownerType, new PropertyMetadata(null));
		}
		#endregion
		string serializedField;
		public SortByCondition() : this(null, null) { }
		public SortByCondition(PivotGridField field, object value)
			: this(field, value, null) { }
		public SortByCondition(PivotGridField field, object value, string olapUniqueName) {
			Field = field;
			Value = value;
			OlapUniqueMemberName = olapUniqueName;
		}
		public SortByCondition(PivotGridFieldSortCondition condition)
			: this(condition.Field.GetWrapper(), condition.Value, condition.OLAPUniqueMemberName) {
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("SortByConditionField"),
#endif
		DefaultValue(null)
		]
		public PivotGridField Field {
			get { return (PivotGridField)GetValue(FieldProperty); }
			set { SetValue(FieldProperty, value); }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string SerializedField {
			get {
				if(!string.IsNullOrEmpty(serializedField))
					return serializedField;
				return Field != null ? Field.Name : null;
			}
			set { serializedField = value; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("SortByConditionValue"),
#endif
		DefaultValue(null),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("SortByConditionOlapUniqueMemberName"),
#endif
		DefaultValue(null),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string OlapUniqueMemberName {
			get { return (string)GetValue(OlapUniqueMemberNameProperty); }
			set { SetValue(OlapUniqueMemberNameProperty, value); }
		}
	}
	public class SortByConditionCollection : PivotChildCollection<SortByCondition> {
		public SortByConditionCollection(PivotGridControl pivotGrid, PivotGridField field)
			: base(pivotGrid, true, field) { }
		internal void AddRange(List<PivotGridFieldSortCondition> list) {
			for(int i = 0; i < list.Count; i++) {
				Add(new SortByCondition(list[i]));
			}
		}
		internal bool AreEqual(PivotGridFieldSortConditionCollection internalCollection) {
			if(internalCollection == null || internalCollection.Count != Count) {
				return false;
			} else {
				for(int i = 0; i < Count; i++) {
					SortByCondition condition = this[i];
					PivotGridFieldSortCondition internalCondition = internalCollection[i];
					if(condition.Field.InternalField != internalCondition.Field
						|| condition.OlapUniqueMemberName != internalCondition.OLAPUniqueMemberName
						|| condition.Value != internalCondition.Value) {
						return false;
					}
				}
				return true;
			}
		}
	}
}
