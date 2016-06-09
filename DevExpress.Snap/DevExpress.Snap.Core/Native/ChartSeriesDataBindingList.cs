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

using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Snap.Core.Native {
	public class SeriesDataBindingList : IEnumerable<SeriesDataBinding>, IXtraSupportDeserializeCollectionItem {
		const string BindingsSerializationName = "Bindings";
		const string DataSourceNameSerializationName = "DataSourceName";
		const string SeriesIndexSerializationName = "SeriesIndex";
		readonly List<SeriesDataBinding> bindings = new List<SeriesDataBinding>();
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false)]
		public List<SeriesDataBinding> Bindings { get { return bindings; } }
		public void Add(string dataSourceName, int seriesIndex) {
			Bindings.Add(new SeriesDataBinding { DataSourceName = dataSourceName, SeriesIndex = seriesIndex });
		}
		public SeriesDataBinding this[int index] { get { return Bindings[index]; } }
		public int Count { get { return Bindings.Count; } }
		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable<SeriesDataBinding>)this).GetEnumerator();
		}
		IEnumerator<SeriesDataBinding> IEnumerable<SeriesDataBinding>.GetEnumerator() {
			return Bindings.GetEnumerator();
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName != BindingsSerializationName)
				throw new InvalidOperationException();
			return CreateSeriesDataBinding(e);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			Bindings.Add((SeriesDataBinding)e.Item.Value);
		}
		SeriesDataBinding CreateSeriesDataBinding(XtraItemEventArgs e) {
			string dataSourceName = (string)e.Item.ChildProperties[DataSourceNameSerializationName].Value;
			int seriesIndex = Convert.ToInt32(e.Item.ChildProperties[SeriesIndexSerializationName].Value);
			return new SeriesDataBinding { DataSourceName = dataSourceName, SeriesIndex = seriesIndex };
		}
	}
	public class SeriesDataBinding {
		[XtraSerializableProperty]
		public string DataSourceName { get; set; }
		[XtraSerializableProperty]
		public int SeriesIndex { get; set; }
		public bool IsBound { get { return DataSourceName != null; } }
	}
}
