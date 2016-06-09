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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.Spreadsheet {
	public interface ParametersCollection : ISimpleCollection<Parameter> {
		Parameter AddParameter(string name, object value);
		Parameter AddParameter(string name);
		Parameter AddParameter(string name, Type type, object value);
		void RemoveAt(int index);
		void Clear();
		bool Contains(Parameter parameter);
		int IndexOf(Parameter parameter);
		Parameter this[string name] { get; }
	}
	#region Parameter
	public interface Parameter {
		string Name { get; set; }
		Type Type { get; set; }
		object Value { get; set; }
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using System.Collections;
	using DevExpress.Spreadsheet;
	partial class NativeParameter : NativeObjectBase, Parameter {
		readonly SpreadsheetParameter modelParameter;
		public NativeParameter(SpreadsheetParameter modelParameter) {
			this.modelParameter = modelParameter;
		}
		#region Implementation of Parameter
		public string Name {
			get {
				CheckValid();
				return modelParameter.Name;
			}
			set {
				CheckValid();
				modelParameter.Name = value;
			}
		}
		public Type Type {
			get {
				CheckValid();
				return modelParameter.Type;
			}
			set {
				CheckValid();
				modelParameter.Type = value;
			}
		}
		public object Value {
			get {
				CheckValid();
				return modelParameter.Value;
			}
			set {
				CheckValid();
				modelParameter.Value = value;
			}
		}
		public SpreadsheetParameter ModelParameter { get { return modelParameter; } }
		#endregion
	}
	partial class NativeParametersCollection : ParametersCollection {
		readonly NativeWorkbook workbook;
		readonly List<NativeParameter> innerList;
		public NativeParametersCollection(NativeWorkbook workbook) {
			this.workbook = workbook;
			innerList = new List<NativeParameter>();
		}
		public NativeWorkbook Workbook { get { return workbook; } }
		public void PopulateParameters() {
			innerList.Clear();
			foreach(SpreadsheetParameter parameter in workbook.ModelWorkbook.MailMergeParameters) {
				NativeParameter nativeParameter = new NativeParameter(parameter);
				this.AddCore(nativeParameter);
			}
		}
		#region Implementation of IEnumerable
		public IEnumerator<Parameter> GetEnumerator() {
			return new EnumeratorConverter<NativeParameter, Parameter>(innerList.GetEnumerator(), ConvertNativeParameterToParameter);
		}
		Parameter ConvertNativeParameterToParameter(NativeParameter item) {
			return item;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return innerList.GetEnumerator();
		}
		#endregion
		#region Implementation of ICollection
		public void CopyTo(Array array, int index) {
			Array.Copy(innerList.ToArray(), 0, array, index, innerList.Count);
		}
		public int Count { get { return innerList.Count; } }
		public object SyncRoot {
			get {
				ICollection collection = innerList;
				return collection.SyncRoot;
			}
		}
		public bool IsSynchronized {
			get {
				ICollection collection = innerList;
				return collection.IsSynchronized;
			}
		}
		#endregion
		#region Implementation of ISimpleCollection<Parameter>
		public Parameter this[int index] { get { return innerList[index]; } }
		#endregion
		#region Implementation of ParametersCollection
		public Parameter AddParameter(string name, object value) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			workbook.ModelWorkbook.MailMergeParameters.Add(name, value);
			return this[this.Count - 1];
		}
		public Parameter AddParameter(string name) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			workbook.ModelWorkbook.MailMergeParameters.Add(name);
			return this[this.Count - 1];
		}
		public Parameter AddParameter(string name, Type type, object value) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			Guard.ArgumentNotNull(type, "type");
			workbook.ModelWorkbook.MailMergeParameters.Add(name, type, value);
			return this[this.Count - 1];
		}
		public void RemoveAt(int index) {
			NativeParameter item = innerList[index];
			if(item.IsValid) {
				item.IsValid = false;
				workbook.ModelWorkbook.MailMergeParameters.RemoveAt(index);
			}
			innerList.Remove(item);
		}
		public void Clear() {
			int count = this.Count;
			for(int i = count - 1; i >= 0; i--) {
				NativeParameter item = innerList[i];
				if(item.IsValid) {
					item.IsValid = false;
					workbook.ModelWorkbook.MailMergeParameters.RemoveAt(i);
				}
				innerList.Remove(item);
			}
		}
		public bool Contains(Parameter parameter) {
			return innerList.Contains(parameter as NativeParameter);
		}
		public int IndexOf(Parameter parameter) {
			return innerList.IndexOf(parameter as NativeParameter);
		}
		Parameter ParametersCollection.this[string name] {
			get {
				foreach(NativeParameter parameter in innerList) {
					if(parameter.Name == name) {
						return parameter;
					}
				}
				return null;
			}
		}
		#endregion
		public void AddCore(NativeParameter item) {
			innerList.Add(item);
		}
		public void InsetCore(int index, NativeParameter parameter) {
			innerList.Insert(index, parameter);
		}
		internal void RemoveCore(NativeParameter item) {
			item.IsValid = false;
			innerList.Remove(item);
		}
	}
}
