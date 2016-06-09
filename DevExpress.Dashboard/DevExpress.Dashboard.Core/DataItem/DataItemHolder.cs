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
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public abstract class DataItemHolder<TDataItem> : IDataItemHolder where TDataItem : DataItem {
		EventHandler changed;
		protected internal abstract string Caption { get; }
		protected internal virtual string CaptionPlural { get { return null; } }
		protected internal abstract TDataItem DataItem { get; set; }
		protected internal virtual DataFieldType[] CompatibleTypesRestriction { get { return null; } }
		DataItem IDataItemHolder.this[string key] {
			get { return DataItem; }
			set {
				if(DataItem != value)
					DataItem = (TDataItem)value;
			}
		}
		int IDataItemHolder.GroupIndex { get { return 0; } }
		int IDataItemHolder.Count { get { return 1; } }
		IEnumerable<string> IDataItemHolder.Captions { get { return new[] { Caption }; } }
		IEnumerable<string> IDataItemHolder.CaptionsPlural { get { return new[] { CaptionPlural }; } }
		IEnumerable<string> IDataItemHolder.Keys { get { return new[] { string.Empty }; } }
		event EventHandler IDataItemHolder.Changed {
			add { changed = (EventHandler)Delegate.Combine(changed, value); }
			remove { changed = (EventHandler)Delegate.Remove(changed, value); }
		}
		protected virtual DataItem AdjustInternal(TDataItem dataItem) {
			return dataItem;
		}
		bool IDataItemHolder.IsCompatible(DataItem dataItem, IDataSourceSchema pickManager) {
			if(dataItem == null || pickManager == null || CompatibleTypesRestriction == null)
				return true;
			DataFieldType type = pickManager.GetFieldType(dataItem.DataMember);
			return CompatibleTypesRestriction.Any(compatibleType => compatibleType == type);
		}
		DataItem IDataItemHolder.Adjust(DataItem dataItem) {
			return AdjustInternal((TDataItem)dataItem);
		}
		public void OnDataItemChanged(object sender, DataItemBoxChangedEventArgs<TDataItem> e) {
			if(changed != null)
				changed(this, EventArgs.Empty);
		}
	}
}
