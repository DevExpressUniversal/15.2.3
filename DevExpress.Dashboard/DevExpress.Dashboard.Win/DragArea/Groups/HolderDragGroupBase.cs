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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class HolderDragGroupBase<THolder> : DragGroup where THolder : class, IDataItemHolder {
		readonly List<THolder> holders;
		readonly List<string> names = new List<string>();
		readonly List<DragItem> items = new List<DragItem>();
		public List<THolder> Holders { get { return holders; } }
		public THolder Holder { get { return holders[0]; } }
		public override IEnumerable<DragItem> Items { get { return items; } }
		public override List<DragItem> ItemList { get { return items; } }
		public override int DataItemsCount { get { return Holder.Count; } }
		protected IList<string> Names { get { return names; } }
		protected HolderDragGroupBase(string optionsButtonImageName, THolder holder)
			: base(optionsButtonImageName) {
			holders = new List<THolder> { holder };
		}
		public override void Initialize(DragSection section) {
			base.Initialize(section);
			UpdateNames();
			IEnumerator<string> captionsPluralEnumerator = Holder.CaptionsPlural.GetEnumerator();
			foreach(string caption in Holder.Captions) {
				string captionPlural = captionsPluralEnumerator.MoveNext() ? captionsPluralEnumerator.Current : null;
				items.Add(new DragItem(DataSource, this, GetActualCaption(caption), captionPlural));
			}
			UpdateDataItems();
		}
		protected virtual string GetActualCaption(string caption) {
			return caption;
		}
		public int IndexOf(DragItem dragItem) {
			return items.IndexOf(dragItem);
		}
		public override int ActualIndexOf(DragItem dragItem) {
			return IndexOf(dragItem);
		}
		public override DataItem GetDataItemByIndex(int index) {
			return GetDataItem(index);
		}
		protected void UpdateNames() {
			names.Clear();
			names.AddRange(Holder.Keys);
		}
		protected void UpdateDataItems() {
			for(int i = 0; i < holders.Count; i++) {
				THolder holder = holders[i];
				for(int j = 0; j < names.Count; j++) {
					DataItem dataItem = holder[names[j]];
					items[j].SetDataItem(dataItem, i);
				}
			}
		}
		protected DataItem GetDataItem(int index) {
			string key = names[index];
			return Holder[key];
		}
		protected void SetDataItem(int index, int holderIndex, DataItem dataItem) {
			THolder holder = holders[holderIndex < 0 ? holders.Count - 1 : holderIndex];
			string key = names[index];
			holder[key] = dataItem;
		}
	}
}
