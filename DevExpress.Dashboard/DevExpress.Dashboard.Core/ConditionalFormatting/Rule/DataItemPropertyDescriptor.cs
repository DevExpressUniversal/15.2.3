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
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	class DataItemPropertyDescriptor : PropertyDescriptor {
		public static PropertyDescriptorCollection GenerateCollection(IDataItemRepositoryProvider dataItemsRepository) {
			IEnumerable<DataItem> dataItems = dataItemsRepository.DataItemRepository.DataItems;
			PropertyDescriptor[] res = new PropertyDescriptor[dataItemsRepository.DataItemRepository.Count];
			int i = 0;
			foreach(DataItem dataItem in dataItems) {
				res[i++] = new DataItemPropertyDescriptor(dataItem);
			}
			return new PropertyDescriptorCollection(res);
		}
		DataItem dataItem;
		public string UniqueId { get { return dataItem == null ? null : dataItem.ActualId; } }
		public bool IsMeasure { get { return dataItem is Measure; } }
		public override string Name { get { return dataItem.ActualId; } }
		public override string DisplayName { get { return dataItem.DisplayName; } }
		public override Type ComponentType { get { return null; } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType { get { return dataItem.DataFieldType.ToType(); } }
		public DataItemPropertyDescriptor(DataItem dataItem)
			: base(dataItem.ActualId, null) {
			this.dataItem = dataItem;
		}
		public override bool CanResetValue(object component) { return false; }
		public override object GetValue(object component) { return null; }
		public override void ResetValue(object component) { }
		public override void SetValue(object component, object value) { }
		public override bool ShouldSerializeValue(object component) { return false; }
	}
}
