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
using DevExpress.Office;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
#if !SL
using System.Drawing;
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Spreadsheet.Charts;
namespace DevExpress.Spreadsheet {
	public interface ProtectedRange {
		string Name { get; set; }
		Range Range { get; set; }
		[Obsolete("Use the Range property instead.", false)]
		IList<Range> Ranges { get; set; }
		string RefersTo { get; set; }
		string RefersToInvariant { get; set; }
		string SecurityDescriptor { get; set; }
		void SetPassword(string password);
		string CreateSecurityDescriptor(IEnumerable<EditRangePermission> permissions);
	}
	public class EditRangePermission {
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("EditRangePermissionUserName")]
#endif
		public string UserName { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("EditRangePermissionDomainName")]
#endif
		public string DomainName { get; set; }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("EditRangePermissionDeny")]
#endif
		public bool Deny { get; set; }
	}
	public interface ProtectedRangeCollection : ISimpleCollection<ProtectedRange> {
		void Remove(ProtectedRange item);
		void Remove(string name);
		void RemoveAt(int index);
		void Clear();
		bool Contains(string name);
		bool Contains(ProtectedRange item);
		ProtectedRange Add(string name, string refersTo);
		ProtectedRange Add(string name, Range range);
		ProtectedRange Add(string name, IList<Range> ranges);
	}
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Diagnostics;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Charts;
	using DevExpress.Utils;
	using DevExpress.XtraSpreadsheet.API.Internal;
	using DevExpress.XtraSpreadsheet.Localization;
	using DevExpress.XtraSpreadsheet.Utils;
	using ModelWorkbook = DevExpress.XtraSpreadsheet.Model.DocumentModel;
	using ModelWorksheet = DevExpress.XtraSpreadsheet.Model.Worksheet;
	using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
	using ModelCellRangeBase = DevExpress.XtraSpreadsheet.Model.CellRangeBase;
	using ModelProtectedRange = DevExpress.XtraSpreadsheet.Model.ModelProtectedRange;
	using ModelProtectedRangeCollection = DevExpress.XtraSpreadsheet.Model.ModelProtectedRangeCollection;
	using DevExpress.XtraSpreadsheet.Services;
	#region NativeProtectedRange
	partial class NativeProtectedRange : ProtectedRange {
		readonly NativeWorksheet nativeWorksheet;
		readonly int index;
		readonly ModelProtectedRangeCollection ranges;
		internal NativeProtectedRange(NativeWorksheet nativeWorksheet, int index) {
			this.nativeWorksheet = nativeWorksheet;
			this.ranges = nativeWorksheet.ModelWorksheet.ProtectedRanges;
			this.index = index;
		}
		#region Properties
		ModelProtectedRange ModelProtectedRange { get { CheckValid(); return ranges[index]; } }
		public string Name { 
			get { return ModelProtectedRange.Name; } 
			set {
				CheckName(value);
				ModelProtectedRange.Name = value; 
			} 
		}
		public Range Range {
			get {
				return new NativeRange(ModelProtectedRange.CellRange, nativeWorksheet);
			}
			set {
				NativeRange range = value as NativeRange;
				if (range == null)
					return;
				ModelProtectedRange.CellRange = range.ModelRange;
			}
		}
		public IList<Range> Ranges {
			get {
				List<Range> result = new List<Range>();
				foreach (ModelCellRange innerRange in ModelProtectedRange.CellRange.GetAreasEnumerable())
					result.Add(new NativeRange(innerRange, nativeWorksheet));
				return result;
			}
			set {
				Guard.ArgumentNotNull(value, "Ranges");
				Guard.ArgumentPositive(value.Count, "Ranges.Count");
				List<ModelCellRangeBase> innerRanges = new List<ModelCellRangeBase>();
				for (int i = 0; i < value.Count; i++) {
					if (!object.ReferenceEquals(value[i].Worksheet, nativeWorksheet))
						throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
					innerRanges.Add(nativeWorksheet.GetModelRange(value[i]));
				}
				ModelProtectedRange.CellRange = new Model.CellUnion(innerRanges);
			}
		}
		public string RefersTo {
			get { return ModelProtectedRange.CellRange.ToString(nativeWorksheet.ModelWorkbook.DataContext); }
			set {
				ModelCellRangeBase range = ModelCellRangeBase.CreateRangeBase(nativeWorksheet.ModelWorksheet, value, nativeWorksheet.ModelWorksheet.DataContext.GetListSeparator());
				if (range == null)
					return;
				ModelProtectedRange.CellRange = range;
			}
		}
		public string RefersToInvariant {
			get {
				Model.WorkbookDataContext dataContext = nativeWorksheet.ModelWorkbook.DataContext;
				dataContext.PushCulture(CultureInfo.InvariantCulture);
				try {
					return RefersTo;
				}
				finally {
					dataContext.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext dataContext = nativeWorksheet.ModelWorkbook.DataContext;
				dataContext.PushCulture(CultureInfo.InvariantCulture);
				try {
					RefersTo = value;
				}
				finally {
					dataContext.PopCulture();
				}
			}
		}
		public string SecurityDescriptor { get { return ModelProtectedRange.SecurityDescriptor; } set { ModelProtectedRange.SecurityDescriptor = value; } }
		internal int Index { get { return index; } }
		internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		#endregion
		void CheckName(string name) {
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeEmptyName));
			if (!Model.ModelProtectedRange.IsValidName(name))
				throw new ArgumentException(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeInvalidName), name));
			int count = this.ranges.Count;
			for (int i = 0; i < count; i++) {
				if (i != this.index && this.ranges[i].Name == name)
					throw new ArgumentException(string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeDuplicateName), name));
			}
		}
		public void CheckValid() {
			if (index >= ranges.Count)
				throw new InvalidOperationException();
		}
		public void SetPassword(string password) {
			if (String.IsNullOrEmpty(password))
				ModelProtectedRange.Credentials = Model.ProtectionCredentials.NoProtection;
			else {
				ModelWorkbook modelWorkbook = NativeWorksheet.DocumentModel as ModelWorkbook;
				if (modelWorkbook != null) {
					ModelProtectedRange.Credentials = new Model.ProtectionCredentials(password, false,
						modelWorkbook.ProtectionOptions.UseStrongPasswordVerifier, modelWorkbook.ProtectionOptions.SpinCount);
				}
				else
					ModelProtectedRange.Credentials = new Model.ProtectionCredentials(password, false);
			}
		}
		public string CreateSecurityDescriptor(IEnumerable<EditRangePermission> permissions) {
			if (permissions == null)
				return String.Empty;
			IRangeSecurityService service = NativeWorksheet.ModelWorkbook.GetService<IRangeSecurityService>();
			if (service == null)
				return String.Empty;
			return service.CreateSecurityDescriptor(permissions);
		}
	}
	#endregion
	#region NativeProtectedRangeCollection
	partial class NativeProtectedRangeCollection : ProtectedRangeCollection {
		readonly object syncRoot = new object();
		readonly NativeWorksheet nativeWorksheet;
		readonly Dictionary<int, NativeProtectedRange> cachedItems = new Dictionary<int, NativeProtectedRange>();
		internal NativeProtectedRangeCollection(NativeWorksheet nativeWorksheet) {
			Guard.ArgumentNotNull(nativeWorksheet, "nativeWorksheet");
			this.nativeWorksheet = nativeWorksheet;
		}
		internal NativeWorksheet NativeWorksheet { get { return nativeWorksheet; } }
		ModelProtectedRangeCollection ProtectedRanges { get { return NativeWorksheet.ModelWorksheet.ProtectedRanges; } }
		protected Dictionary<int, NativeProtectedRange> CachedItems { get { return cachedItems; } }
		public int Count { get { return ProtectedRanges.Count; } }
		public ProtectedRange this[int index] {
			get {
				CheckIndex(index);
				NativeProtectedRange result;
				if (!cachedItems.TryGetValue(index, out result)) {
					result = CreateNativeObject(index);
					cachedItems.Add(index, result);
					return result;
				}
				return result;
			}
		}
		public void RemoveAt(int index) {
			if (cachedItems.ContainsKey(index))
				cachedItems.Remove(index);
			ProtectedRanges.RemoveAt(index);
		}
		public void Remove(ProtectedRange item) {
			NativeProtectedRange range = (NativeProtectedRange)item;
			RemoveAt(range.Index);
		}
		public void Remove(string name) {
			int index = ProtectedRanges.LookupProtectedRangeIndex(name);
			if (index < 0)
				return;
			RemoveAt(index);
		}
		public bool Contains(string name) {
			return ProtectedRanges.LookupProtectedRangeIndex(name) >= 0;
		}
		public bool Contains(ProtectedRange item) {
			NativeProtectedRange range = (NativeProtectedRange)item;
			if (!Object.ReferenceEquals(range.NativeWorksheet, this.nativeWorksheet))
				return false;
			return range.Index >= 0 && range.Index < ProtectedRanges.Count;
		}
		public void Clear() {
			cachedItems.Clear();
			ProtectedRanges.Clear();
		}
		public ProtectedRange Add(string name, string refersTo) {
			ModelCellRange range = ModelCellRange.TryCreate(nativeWorksheet.ModelWorksheet, refersTo);
			if (range == null)
				return null;
			return Add(name, range);
		}
		public ProtectedRange Add(string name, Range range) {
			return Add(name, ((NativeRange)range).ModelRange);
		}
		public ProtectedRange Add(string name, IList<Range> ranges) {
			Guard.ArgumentNotNull(ranges, "ranges");
			Guard.ArgumentPositive(ranges.Count, "ranges.Count");
			List<ModelCellRangeBase> innerRanges = new List<ModelCellRangeBase>();
			for (int i = 0; i < ranges.Count; i++) {
				if (!object.ReferenceEquals(ranges[i].Worksheet, nativeWorksheet))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
				innerRanges.Add(nativeWorksheet.GetModelRange(ranges[i]));
			}
			return Add(name, new Model.CellUnion(innerRanges));
		}
		ProtectedRange Add(string name, ModelCellRangeBase range) {
			CheckName(name);
			ModelProtectedRange modelProtectedRange = new ModelProtectedRange(name, range);
			ProtectedRanges.Add(modelProtectedRange);
			return this[Count - 1];
		}
		void CheckName(string name) {
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeEmptyName));
			if (!Model.ModelProtectedRange.IsValidName(name))
				throw new ArgumentException(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeInvalidName), name));
			if (Contains(name))
				throw new ArgumentException(String.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ProtectedRangeDuplicateName), name));
		}
		protected internal NativeProtectedRange CreateNativeObject(int index) {
			return new NativeProtectedRange(NativeWorksheet, index);
		}
		public void CheckIndex(int index) {
			if (index < 0 && index >= Count)
				throw new IndexOutOfRangeException();
		}
		IEnumerator<ProtectedRange> IEnumerable<ProtectedRange>.GetEnumerator() {
			return GetEnumeratorCore();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumeratorCore();
		}
		IEnumerator<ProtectedRange> GetEnumeratorCore() {
			int count = Count;
			for (int i = 0; i < count; i++)
				yield return this[i];
		}
		#region ICollection implementation
		void ICollection.CopyTo(Array array, int index) {
			int count = Count;
			for (int i = 0; i < count; i++)
				array.SetValue(this[i], i + index);
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return syncRoot; } }
		#endregion
	}
	#endregion
}
