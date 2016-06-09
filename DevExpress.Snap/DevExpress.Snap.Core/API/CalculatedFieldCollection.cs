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
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.API;
using System.Collections;
namespace DevExpress.Snap.Core.API {
	[TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))]
	public class CalculatedFieldCollection : BindingList<CalculatedField> {
		protected internal event CalculatedFieldAddedEventHandler CalculatedFieldAdded;
		protected virtual void RaiseCalculatedFieldAdded(CalculatedField field) {
			if (CalculatedFieldAdded != null)
				CalculatedFieldAdded(this, new CalculatedFieldAddedEventArgs(field));			
		}
		protected override void InsertItem(int index, CalculatedField item) {
			item.Disposed += item_Disposed;
			RaiseCalculatedFieldAdded(item);
			base.InsertItem(index, item);
		}
		public void AddRangeByValue(IList calculatedFields) {
			AddRangeCore(calculatedFields, true);
		}
		protected internal void AddRangeCore(IList calculatedFields, bool clone) {
			foreach (CalculatedField cf in calculatedFields) {
				CalculatedField newCf = clone ? cf.Clone() : cf;
				this.Add(newCf);
			}
		}
		void item_Disposed(object sender, EventArgs e) {
			Remove(((CalculatedField)sender));
		}
	}
}
namespace DevExpress.Snap.Core.Native {
	public delegate void CalculatedFieldAddedEventHandler(object sender, CalculatedFieldAddedEventArgs e);
	public class CalculatedFieldAddedEventArgs : EventArgs {
		public CalculatedFieldAddedEventArgs(CalculatedField field) {
			Field = field;
		}
		public CalculatedField Field { get; private set; }
	}
}
