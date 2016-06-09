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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public interface IEncoder {
		int Encode(object value);
		object Decode(int code);
	}
	public interface ISliceOwner {
		int Encode(StorageColumn column, object value);
		object Decode(StorageColumn column, int code);
		StorageColumn GetColumn(string name);
	}
	public interface IRowOwner {
		StorageColumn[] KeyColumns { get; }
		int GetColumnIndex(StorageColumn column);
		Dictionary<int, object> ExtractValues(CompositeKey key);
	}
	public class DataStorage : IEnumerable<StorageSlice>, ISliceOwner {
		#region internal types
		public struct StorageSliceEnumerator : IEnumerator<StorageSlice> {
			Dictionary<SliceSignature, StorageSlice>.Enumerator enumerator;
			public StorageSlice Current { get { return enumerator.Current.Value; } }
			object IEnumerator.Current { get { return this.Current; } }
			public StorageSliceEnumerator(DataStorage storage) {
				this.enumerator = storage.slices.GetEnumerator();
			}
			public void Dispose() {
				enumerator.Dispose();
			}
			public bool MoveNext() {
				return enumerator.MoveNext();
			}
			public void Reset() {
				((IEnumerator)enumerator).Reset();
			}
		}
		class SliceSignature {
			HashSet<StorageColumn> signature;
			public SliceSignature(IEnumerable<StorageColumn> signature) {
				this.signature = new HashSet<StorageColumn>(signature);
			}
			public bool IsSignatureEquals(StorageColumn[] testSignature) {
				return signature.Count == testSignature.Length && signature.SetEquals(testSignature);
			}
		}
		#endregion
		DataStorageDTO dto;
		Dictionary<StorageColumn, EncodeHelper> encoders = new Dictionary<StorageColumn, EncodeHelper>();
		Dictionary<string, StorageColumn> columns = new Dictionary<string, StorageColumn>();
		Dictionary<SliceSignature, StorageSlice> slices = new Dictionary<SliceSignature, StorageSlice>();
		public bool IsEmpty { get { return dto.Slices.All(vs => vs.KeyIds.Length == 0 ? vs.ValueIds.Count == 0 : vs.Data.Count == 0); } } 
		DataStorage(DataStorageDTO dto) {
			this.dto = dto;
			var keyColumnNames = dto.Slices.SelectMany(vs => vs.KeyIds).Distinct();
			var measureColumnNames = dto.Slices.SelectMany(vs => vs.ValueIds.Keys).Distinct();
			keyColumnNames.ForEach(name => CreateColumn(name, true));
			measureColumnNames.ForEach(name => CreateColumn(name, false));
			foreach(SliceDTO vsDto in dto.Slices) {
				IList<StorageColumn> signature = vsDto.KeyIds.Select(name => columns[name]).ToList();
				slices.Add(new SliceSignature(signature), new StorageSlice(signature, vsDto, this));
			}
		}
		#region API
		public StorageColumn CreateColumn(string name, bool isKey) {
			StorageColumn column;
			if(!columns.TryGetValue(name, out column)) {
				column = new StorageColumn(name, isKey, GetEncoder(name));
				columns.Add(name, column);
			}
			return column;
		}
		public StorageColumn GetColumn(string name) {
			StorageColumn column;
			return columns.TryGetValue(name, out column) ? column : null;
		}
		public StorageSlice GetSlice(IEnumerable<StorageColumn> keyColumns) {
			return GetOrCreateSlice(keyColumns, true);
		}
		public StorageSlice GetSliceIfExists(IEnumerable<StorageColumn> keyColumns) {
			return GetOrCreateSlice(keyColumns, false);
		}
		public DataStorageDTO GetDTO() {
			return dto;
		}
		public StorageSliceEnumerator GetEnumerator() {
			return new StorageSliceEnumerator(this);
		}
		IEnumerator<StorageSlice> IEnumerable<StorageSlice>.GetEnumerator() {
			return new StorageSliceEnumerator(this);
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return new StorageSliceEnumerator(this);
		}
		StorageSlice GetOrCreateSlice(IEnumerable<StorageColumn> keyColumns, bool create) {
			StorageColumn[] keyColumnsArr = keyColumns.ToArray();
			StorageSlice slice = null;
			foreach(var pair in slices)
				if(pair.Key.IsSignatureEquals(keyColumnsArr)) {
					slice = pair.Value;
					break;
				}
			if(slice == null && create) {
				SliceDTO vsDto = new SliceDTO() { KeyIds = keyColumns.Select(c => c.Name).ToArray() };
				dto.Slices.Add(vsDto);
				slice = new StorageSlice(keyColumns, vsDto, this);
				slices.Add(new SliceSignature(keyColumns), slice);
			}
			return slice;
		}
		#endregion
		#region ISliceOwner implementation
		EncodeHelper GetEncodeHelper(StorageColumn column) {
			EncodeHelper helper;
			if(!encoders.TryGetValue(column, out helper)) {
				helper = GetEncoder(column.Name);
				encoders[column] = helper;
			}
			return helper;
		}
		EncodeHelper GetEncoder(string columnName) {
			ArrayList encodeStorage;
			if(!dto.EncodeMaps.TryGetValue(columnName, out encodeStorage)) {
				encodeStorage = new ArrayList();
				dto.EncodeMaps.Add(columnName, encodeStorage);
			}
			return new EncodeHelper(encodeStorage);
		}
		int ISliceOwner.Encode(StorageColumn column, object value) {
			return GetEncodeHelper(column).Encode(value);
		}
		object ISliceOwner.Decode(StorageColumn column, int code) {
			return GetEncodeHelper(column).Decode(code);
		}
		#endregion
		public static DataStorage CreateEmpty() {
			return new DataStorage(new DataStorageDTO());
		}
		public static DataStorage CreateWithDTO(DataStorageDTO dto) {
			if(dto == null)
				return CreateEmpty();
			else
				return new DataStorage(dto);
		}
		public object GetOLAPValue(StorageValue value) {
			StorageColumn column = value.KeyBindData.Column;
			StorageSlice slice = (this).Single((s) => s.KeyColumns.Count() == 1 && s.KeyColumns.FirstOrDefault() == column);
			StorageRow rowToFind = new StorageRow();
			rowToFind[column] = value;
			return slice.FindRow(rowToFind).Value[slice.MeasureColumns.First((c) => c.Name == DataStorageGenerator.ValueStorageColumnName)].MaterializedValue;
		}
	}
	public class StorageColumn : IEncoder {
		string name;
		bool isKey;
		EncodeHelper encoder;
		public string Name { get { return name; } }
		public bool IsKey { get { return isKey; } }
		public StorageColumn(string name, bool isKey, EncodeHelper encoder) {
			this.name = name;
			this.isKey = isKey;
			this.encoder = encoder;
		}
		public override string ToString() {
			return name;
		}
		public int Encode(object value) {
			DXContract.Requires(isKey);
			return encoder.Encode(value);
		}
		public object Decode(int code) {
			DXContract.Requires(isKey);
			return encoder.Decode(code);
		}
	}
}
