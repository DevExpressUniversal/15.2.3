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
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Xpf.Core;
using DevExpress.Data.Browsing;
using System.Windows.Data;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using DevExpress.Utils;
namespace DevExpress.Xpf.Data {
	public abstract class RowTypeDescriptorBase : CustomTypeDescriptorBase {
		WeakReference ownerReference;
		protected DataProviderBase Owner { get { return (DataProviderBase)ownerReference.Target; } }
		protected RowTypeDescriptorBase(WeakReference ownerReference) {
			this.ownerReference = ownerReference;
		}
		#region ICustomTypeDescriptor Members
		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
			return Owner.GetProperties();
		}
		#endregion
		protected internal abstract object GetValue(DataColumnInfo info);
		protected internal abstract void SetValue(DataColumnInfo info, object value);
		internal abstract object GetValue(string fieldName);
	}
	public class RowTypeDescriptor : RowTypeDescriptorBase {
		RowHandle rowHandle;
		int listSourceRowIndex;
		public RowTypeDescriptor(WeakReference ownerReference, RowHandle rowHandle, int listSourceRowIndex)
			: base(ownerReference) {
			this.rowHandle = rowHandle;
			this.listSourceRowIndex = listSourceRowIndex;
		}
		public RowHandle RowHandle { get { return rowHandle; } }
		protected internal override object GetValue(DataColumnInfo info) {
			return GetValueCore(info.Name);
		}
		internal override object GetValue(string fieldName) {
			return GetValueCore(fieldName);
		}
		protected internal override void SetValue(DataColumnInfo info, object value) {
			Owner.SetRowValue(RowHandle, info, value);
		}
		object GetValueCore(string fieldName) {
			if(RowHandle.Value != DataControlBase.InvalidRowHandle)
				return Owner.GetRowValue(RowHandle.Value, fieldName);
			return listSourceRowIndex != DataControlBase.InvalidRowIndex ? Owner.GetCellValueByListIndex(listSourceRowIndex, fieldName) : null;
		}
	}
}
