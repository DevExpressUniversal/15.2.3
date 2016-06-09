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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	class FormatConditionUniqueDuplicate : FormatConditionStyleBase {
		DashboardFormatConditionUniqueDuplicateType formatType = DashboardFormatConditionUniqueDuplicateType.Duplicate;
		Hashtable items;
		bool hasNull;
		[
		DefaultValue(DashboardFormatConditionUniqueDuplicateType.Duplicate)
		]
		public DashboardFormatConditionUniqueDuplicateType FormatType {
			get { return formatType; }
			set {
				if(FormatType != value) {
					formatType = value;
					OnChanged();
				}
			}
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionUniqueDuplicate();
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionUniqueDuplicate;
			if(source != null) {
				this.FormatType = source.FormatType;
			}
		}
		protected override IEnumerable<SummaryItemTypeEx> GetAggregationTypes() {
			return new SummaryItemTypeEx[] { FormatType.ToSummaryItemType() };
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			if(this.items == null) {
				object value = GetAggregationValue(FormatType.ToSummaryItemType());
				this.items = PrepareItems(value as IList, out this.hasNull);
			}
			object actualValue = valueProvider.GetValue(this);
			if(CheckValue(actualValue))
				return this.items.ContainsKey(actualValue);
			else
				return this.hasNull;
		}
		Hashtable PrepareItems(IList items, out bool hasNull) {
			hasNull = false;
			Hashtable res = new Hashtable();
			if(items == null || items.Count == 0) return res;
			try {
				foreach(var item in items) {
					if(ValueManager.IsNull(item)) {
						hasNull = true;
						continue;
					}
					res[item] = true;
				}
			} catch { 
			}
			return res;
		}
	}	
}
