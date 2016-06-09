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
using System.Collections.ObjectModel;
using System.ComponentModel;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	[ListBindable(BindableSupport.No)]
	[TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter))]
	public class CalculatedFieldCollection : Collection<CalculatedField>, IEnumerable<ICalculatedField>, IDisposable {
		readonly XtraReport report;
		internal XtraReport Report {
			get { return report; }
		}
		public CalculatedFieldCollection(XtraReport report) {
			this.report = report;
		}
		public void Dispose() {
			while(this.Count > 0)
				this[0].Dispose();
			this.Clear();
		}
		public void AddRange(CalculatedField[] items) {
			foreach(CalculatedField item in items)
				this.Add(item);
		}
		internal CalculatedField GetByName(string calculatedFieldName) {
			foreach(CalculatedField calculatedField in this)
				if(calculatedField.Name == calculatedFieldName)
					return calculatedField;
			return default(CalculatedField);
		}
		internal void CopyFrom(CalculatedFieldCollection source) {
			this.Clear();
			this.AddRange(source.ToArray());
		}
		protected override void InsertItem(int index, CalculatedField item) {
			if(Contains(item))
				return;
			base.InsertItem(index, item);
			item.Owner = this;
		}
		protected CalculatedField[] ToArray() {
			CalculatedField[] items = new CalculatedField[this.Count];
			this.CopyTo(items, 0);
			return items;
		}
		#region IEnumerable<ICalculatedField> Members
		IEnumerator<ICalculatedField> IEnumerable<ICalculatedField>.GetEnumerator() {
			foreach(var item in Items) {
				yield return item;
			}
		}
		#endregion
	}
}
