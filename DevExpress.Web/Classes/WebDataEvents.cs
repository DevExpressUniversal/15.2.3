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
using System.Collections.Specialized;
using System.ComponentModel;
namespace DevExpress.Web.Data {
	public class ASPxDataBaseUpdatedEventArgs : EventArgs {
		int affectedRecords;
		Exception exception;
		bool exceptionHandled;
		public ASPxDataBaseUpdatedEventArgs(int affectedRecords, Exception exception) {
			this.exception = exception;
			this.affectedRecords = affectedRecords;
			this.exceptionHandled = exception == null;
		}
		public int AffectedRecords { get { return affectedRecords; } }
		public Exception Exception { get { return exception; } }
		public bool ExceptionHandled { get { return exceptionHandled; } set { exceptionHandled = value; } }
	}
	public class ASPxDataUpdatedEventArgs : ASPxDataBaseUpdatedEventArgs {
		OrderedDictionary keys, oldValues, newValues;
		public ASPxDataUpdatedEventArgs(int affectedRecords, Exception exception, ASPxDataUpdatingEventArgs updatingArgs)
			: base(affectedRecords, exception) {
			this.keys = updatingArgs.Keys;
			this.oldValues = updatingArgs.OldValues;
			this.newValues = updatingArgs.NewValues;
		}
		public ASPxDataUpdatedEventArgs(int affectedRecords, Exception exception)
			: base(affectedRecords, exception) {
			this.keys = new OrderedDictionary();
			this.oldValues = new OrderedDictionary();
			this.newValues = new OrderedDictionary();
		}
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary NewValues { get { return newValues; } }
		public OrderedDictionary OldValues { get { return oldValues; } }
	}
	public delegate void ASPxDataUpdatedEventHandler(object sender, ASPxDataUpdatedEventArgs e);
	public class ASPxDataDeletedEventArgs : ASPxDataBaseUpdatedEventArgs {
		OrderedDictionary keys, values;
		public ASPxDataDeletedEventArgs(int affectedRecords, Exception exception, ASPxDataDeletingEventArgs deletingArgs)
			: base(affectedRecords, exception) {
			this.keys = deletingArgs.Keys;
			this.values = deletingArgs.Values;
		}
		public ASPxDataDeletedEventArgs(int affectedRecords, Exception exception)
			: base(affectedRecords, exception) {
			this.keys = new OrderedDictionary();
			this.values = new OrderedDictionary();
		}
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary Values { get { return values; } }
	}
	public delegate void ASPxDataDeletedEventHandler(object sender, ASPxDataDeletedEventArgs e);
	public class ASPxDataInsertedEventArgs : ASPxDataBaseUpdatedEventArgs {
		OrderedDictionary newValues;
		public ASPxDataInsertedEventArgs(int affectedRecords, Exception exception, OrderedDictionary values)
			: base(affectedRecords, exception) {
			this.newValues = values;
		}
		public ASPxDataInsertedEventArgs(int affectedRecords, Exception exception)
			: base(affectedRecords, exception) {
			this.newValues = new OrderedDictionary();
		}
		public OrderedDictionary NewValues { get { return newValues; } }
	}
	public delegate void ASPxDataInsertedEventHandler(object sender, ASPxDataInsertedEventArgs e);
	public class ASPxDataInitNewRowEventArgs : EventArgs {
		OrderedDictionary values;
		public ASPxDataInitNewRowEventArgs() {
			this.values = new OrderedDictionary();
		}
		public OrderedDictionary NewValues { get { return values; } }
	}
	public delegate void ASPxDataInitNewRowEventHandler(object sender, ASPxDataInitNewRowEventArgs e);
	public class ASPxDataInsertingEventArgs : CancelEventArgs {
		OrderedDictionary values;
		public ASPxDataInsertingEventArgs()
			: this(null) {
		}
		public ASPxDataInsertingEventArgs(ASPxDataInsertValues insertValues) {
			this.values = insertValues != null ? insertValues.NewValues : new OrderedDictionary();
		}
		public OrderedDictionary NewValues { get { return values; } }
	}
	public delegate void ASPxDataInsertingEventHandler(object sender, ASPxDataInsertingEventArgs e);
	public class ASPxDataDeletingEventArgs : CancelEventArgs {
		OrderedDictionary keys, values;
		public ASPxDataDeletingEventArgs()
			: this(null) {
		}
		public ASPxDataDeletingEventArgs(ASPxDataDeleteValues delValues) {
			this.keys = delValues != null ? delValues.Keys : new OrderedDictionary();
			this.values = delValues != null ? delValues.Values : new OrderedDictionary();
		}
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary Values { get { return values; } }
	}
	public delegate void ASPxDataDeletingEventHandler(object sender, ASPxDataDeletingEventArgs e);
	public class ASPxDataUpdatingEventArgs : CancelEventArgs {
		OrderedDictionary keys, oldValues, values;
		public ASPxDataUpdatingEventArgs()
			: this(null) {
		}
		public ASPxDataUpdatingEventArgs(ASPxDataUpdateValues updValues) {
			this.keys = updValues != null ? updValues.Keys : new OrderedDictionary();
			this.oldValues = updValues != null ? updValues.OldValues : new OrderedDictionary();
			this.values = updValues != null ? updValues.NewValues : new OrderedDictionary();
		}
		public OrderedDictionary Keys { get { return keys; } }
		public OrderedDictionary NewValues { get { return values; } }
		public OrderedDictionary OldValues { get { return oldValues; } }
	}
	public delegate void ASPxDataUpdatingEventHandler(object sender, ASPxDataUpdatingEventArgs e);
	public class ASPxParseValueEventArgs : EventArgs {
		string fieldName;
		object value;
		int itemIndex = -1;
		public ASPxParseValueEventArgs(string fieldName, object value) 
			: this(fieldName, value, -1) {
		}
		public ASPxParseValueEventArgs(string fieldName, object value, int itemIndex) {
			this.fieldName = fieldName;
			this.value = value;
			this.itemIndex = itemIndex;
		}
		public string FieldName {
			get { return fieldName; }
		}
		public object Value {
			get { return value; }
			set { this.value = value; }
		}
		public int ItemIndex { get { return itemIndex; } }
	}
	public delegate void ASPxParseValueEventHandler(object sender, ASPxParseValueEventArgs e);
	public class ASPxDataBatchUpdateEventArgs : EventArgs {
		public ASPxDataBatchUpdateEventArgs() {
			InsertValues = new List<ASPxDataInsertValues>();
			UpdateValues = new List<ASPxDataUpdateValues>();
			DeleteValues = new List<ASPxDataDeleteValues>();
		}
		public List<ASPxDataInsertValues> InsertValues { get; private set; }
		public List<ASPxDataUpdateValues> UpdateValues { get; private set; }
		public List<ASPxDataDeleteValues> DeleteValues { get; private set; }
		public bool Handled { get; set; }
	}
	public class ASPxDataInsertValues {
		public ASPxDataInsertValues() {
			NewValues = new OrderedDictionary();
		}
		internal ASPxDataInsertValues(int rowIndex)
			: this() {
			RowIndex = rowIndex;
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataInsertValuesNewValues")]
#endif
		public OrderedDictionary NewValues { get; private set; }
		internal int RowIndex { get; private set; }
	}
	public class ASPxDataUpdateValues {
		public ASPxDataUpdateValues() {
			Keys = new OrderedDictionary();
			OldValues = new OrderedDictionary();
			NewValues = new OrderedDictionary();
		}
		internal ASPxDataUpdateValues(object rowKey)
			: this() {
			RowKey = rowKey;
		}
		internal object RowKey { get; private set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataUpdateValuesKeys")]
#endif
		public OrderedDictionary Keys { get; private set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataUpdateValuesOldValues")]
#endif
		public OrderedDictionary OldValues { get; private set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataUpdateValuesNewValues")]
#endif
		public OrderedDictionary NewValues { get; private set; }
	}
	public class ASPxDataDeleteValues {
		public ASPxDataDeleteValues() {
			Keys = new OrderedDictionary();
			Values = new OrderedDictionary();
		}
		internal ASPxDataDeleteValues(object rowKey)
			: this() {
			RowKey = rowKey;
		}
		internal object RowKey { get; private set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataDeleteValuesKeys")]
#endif
		public OrderedDictionary Keys { get; private set; }
#if !SL
	[DevExpressWebLocalizedDescription("ASPxDataDeleteValuesValues")]
#endif
		public OrderedDictionary Values { get; private set; }
	}
	public delegate void ASPxDataBatchUpdateEventHandler(object sender, ASPxDataBatchUpdateEventArgs e);
}
