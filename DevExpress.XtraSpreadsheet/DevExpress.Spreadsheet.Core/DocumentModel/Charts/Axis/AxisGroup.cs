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
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class AxisGroup : UndoableCollection<AxisBase>, ISupportsCopyFrom<AxisGroup> {
		readonly IChart parent;
		public AxisGroup(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
		}
		public AxisBase Find(AxisDataType type) {
			foreach (AxisBase axis in InnerList) {
				if (axis.AxisType == type)
					return axis;
			}
			return null;
		}
		public AxisBase GetItem(int index) {
			if (index < 0 || index >= Count)
				return null;
			return InnerList[index];
		}
		public void Reverse() {
			if (Count != 2)
				return;
			AxisBase axis = this[1];
			RemoveAt(1);
			Insert(0, axis);
		}
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			foreach (AxisBase axis in InnerList)
				axis.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			foreach (AxisBase axis in InnerList)
				axis.OnRangeRemoving(context);
		}
		#endregion
		#region ISupportsCopyFrom<AxisGroup> Members
		public void CopyFrom(AxisGroup value) {
			Guard.ArgumentNotNull(value, "value");
			this.Clear();
			int count = value.Count;
			for (int i = 0; i < count; i++) {
				AxisBase item = value[i].CloneTo(parent);
				this.Add(item);
			}
			for (int i = 0; i < count; i++) {
				int index = value.IndexOf(value[i].CrossesAxis);
				if (index != -1)
					this[i].CrossesAxis = this[index];
			}
		}
		#endregion
	}
}
