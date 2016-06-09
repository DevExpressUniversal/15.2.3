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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.Compatibility.System.ComponentModel;
#if !SL
using System.ComponentModel;
#else
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotGridFieldPropertyDescriptor : PropertyDescriptor {
		public static string GetNameByField(PivotGridFieldBase field) {
			return field.IsOLAPField ? field.IsOLAPMeasure && field.UnboundType == UnboundColumnType.Bound && string.IsNullOrEmpty(field.Name) ? field.FieldName : field.UniqueName : ((IDataColumnInfo)field).FieldName;
		}
		public PivotGridFieldPropertyDescriptor(PivotGridFieldBase field)
			: base(field.ExpressionFieldName, null) {
			this.field = field;
		}
		PivotGridFieldBase field;
		public PivotGridFieldBase Field { get { return field; } }
		protected IDataColumnInfo Column { get { return field; } }
		public override string Name { get { return GetNameByField(field); } }
		public override string DisplayName { get { return Column.Caption; } }
		public override Type ComponentType { get { return null; } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return Column.FieldType; } }
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) { return null; }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
}
