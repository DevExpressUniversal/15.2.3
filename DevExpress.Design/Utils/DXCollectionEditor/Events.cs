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
using System.ComponentModel;
namespace DevExpress.Utils.Design.Internal {
	public delegate void QueryNewItemEventHandler(object sender, QueryNewItemEventArgs e);
	public class QueryNewItemEventArgs : EventArgs {
		public QueryNewItemEventArgs(Type itemType) {
			this.ItemType = itemType;
		}
		public Type ItemType { get; private set; }
		public object Item { get; set; }
	}
	public enum CollectionAction { Add, Remove, Reorder }
	public delegate void CollectionChangedEventHandler(object sender, CollectionChangedEventArgs e);
	public class CollectionChangedEventArgs : EventArgs {
		public CollectionChangedEventArgs(CollectionAction action, object item, object targetItem = null) {
			this.Action = action;
			this.Item = item;
			this.TargetItem = targetItem;
		}
		public CollectionAction Action { get; private set; }
		public object Item { get; private set; }
		public object TargetItem { get; private set; }
		internal bool IsCreatedItem = true;
	}
	public delegate void CollectionChangingEventHandler(object sender, CollectionChangingEventArgs e);
	public class CollectionChangingEventArgs : CancelEventArgs {
		public CollectionChangingEventArgs(CollectionAction action, object item, object targetItem = null)
			: base(false) {
			this.Item = item;
			this.TargetItem = targetItem;
			this.Action = action;
		}
		public CollectionAction Action { get; private set; }
		public object Item { get; private set; }
		public object TargetItem { get; private set; }
	}
	public delegate void PropertyItemChangedEventHandler(object sender, PropertyItemChangedEventArgs e);
	public class PropertyItemChangedEventArgs : EventArgs {
		public PropertyItemChangedEventArgs(object item, string propertyName) {
			this.Item = item;
			this.PropertyName = propertyName;
		}
		public bool UpdateVisibleInfo { get; set; }
		public object Item { get; private set; }
		public string PropertyName { get; private set; }
	}
	public delegate void QueryCustomDisplayTextEventHandler(object sender, CustomDisplayTextEventArgs e);
	public class CustomDisplayTextEventArgs : EventArgs {
		public CustomDisplayTextEventArgs(object item, string fieldName) {
			this.Item = item;
			this.FieldName = fieldName;
		}
		public string DisplayText { get; set; }
		public object Item { get; private set; }
		public string FieldName { get; private set; }
	}
	public delegate void SelectedItemChanged(object sender, SelectedItemChangedEventArgs e);
	public class SelectedItemChangedEventArgs : EventArgs {
		public SelectedItemChangedEventArgs(object selectedItem) {
			this.SelectedItem = selectedItem;
		}
		public object SelectedItem { get; private set; }
	}
}
