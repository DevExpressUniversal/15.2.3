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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Data.Access;
using System.IO;
using System.Text;
using DevExpress.Compatibility.System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Collections;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Utils.Zip;
#else
using System.IO.Compression;
#endif
namespace DevExpress.Utils.Serializing.Helpers {
	public class PrintingSystemSerializationContext : SerializationContext {
		Dictionary<Type, PropertyDescriptorCollection> propertyDescriptorCollections = new Dictionary<Type, PropertyDescriptorCollection>();
#if DEBUG
		Dictionary<Type, DXCollection<SerializablePropertyDescriptorPair>> sortedPairLists = new Dictionary<Type, DXCollection<SerializablePropertyDescriptorPair>>();
		public class DebugPropertyComparer : DevExpress.Utils.Serializing.Helpers.PropertyDescriptorComparer {
			public DebugPropertyComparer(SerializationContext serializationContext, object obj) : base(serializationContext, obj) {
			}
			protected override int CompareProperties(SerializablePropertyDescriptorPair x, SerializablePropertyDescriptorPair y) {
				int result = base.CompareProperties(x, y);
				if(result == 0 && x.Property != null && y.Property != null)
					return string.CompareOrdinal(x.Property.Name, y.Property.Name);
				return result;
			}
		}
		protected internal override IList<SerializablePropertyDescriptorPair> SortProps(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			DXCollection<SerializablePropertyDescriptorPair> list;
			Type type = obj.GetType();
			if(sortedPairLists.TryGetValue(type, out list))
				return list;
			list = new DXCollection<SerializablePropertyDescriptorPair>();
			list.UniquenessProviderType = DXCollectionUniquenessProviderType.None;
			list.AddRange(pairsList);
			list.Sort(new DebugPropertyComparer(this, obj));
			sortedPairLists.Add(type, list);
			return list;
		}
#else
		protected internal override IList<SerializablePropertyDescriptorPair> SortProps(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			IXtraSortableProperties sortable = obj as IXtraSortableProperties;
			if(sortable != null && sortable.ShouldSortProperties())
				return SortPropsCore(obj, pairsList);
			return pairsList;
		}
#endif
		protected IList<SerializablePropertyDescriptorPair> SortPropsCore(object obj, List<SerializablePropertyDescriptorPair> pairsList) {
			return base.SortProps(obj, pairsList);
		}
		protected internal override bool ShouldSerializeProperty(SerializeHelper helper, object obj, PropertyDescriptor prop, XtraSerializableProperty xtraSerializableProperty) {
			if(xtraSerializableProperty.SupressDefaultValue)
				return true;
			DefaultValueAttribute attr = prop.Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
			return attr == null || !object.Equals(attr.Value, prop.GetValue(obj));
		}
		protected internal override int GetCollectionItemsCount(XtraPropertyInfo root) {
			return root.ChildProperties.Count;
		}
		protected internal override void DeserializeObjectsCore(DeserializeHelper helper, IList objects, IXtraPropertyCollection store, OptionsLayoutBase options) {
			DeserializeHelper.CallStartDeserializing(helper.RootObject, string.Empty);
			try {
				foreach(XtraPropertyInfo propertyInfo in store) {
					if(propertyInfo.ChildProperties == null)
						continue;
					XtraObjectInfo objectInfo = FindObject(objects, propertyInfo.Name);
					helper.DeserializeObject(objectInfo.Instance, propertyInfo.ChildProperties, options);
				}
			} finally {
				DeserializeHelper.CallEndDeserializing(helper.RootObject, string.Empty);
			}
		}
		static XtraObjectInfo FindObject(IList objects, string name) {
			foreach(XtraObjectInfo info in objects) {
				if(info.Name == name)
					return info;
			}
			throw new InvalidOperationException();
		}
		protected internal override IXtraPropertyCollection SerializeObjectsCore(SerializeHelper helper, IList objects, OptionsLayoutBase options) {
			SerializeHelper.CallStartSerializing(helper.RootObject);
			try {
				return new SerializationVirtualXtraPropertyCollection(helper, options, objects);
			} finally {
				SerializeHelper.CallEndSerializing(helper.RootObject);
			}
		}
		protected internal override PropertyDescriptorCollection GetProperties(object obj, IXtraPropertyCollection store) {
			PropertyDescriptorCollection properties;
			Type type = obj.GetType();
			if(propertyDescriptorCollections.TryGetValue(type, out properties))
				return properties;
			properties = DataListDescriptor.GetFastProperties(type);
			propertyDescriptorCollections.Add(type, properties);
			return properties;
		}
		protected internal override bool InvokeShouldSerialize(SerializeHelper helper, object obj, PropertyDescriptor property) {
			IXtraSupportShouldSerialize supportShouldSerialize = obj as IXtraSupportShouldSerialize;
			return supportShouldSerialize != null ? supportShouldSerialize.ShouldSerialize(property.Name) : true;
		}
		protected internal override MethodInfo GetShouldSerializeCollectionMethodInfo(SerializeHelper helper, string name, object owner) {
			return null;
		}
		protected internal override bool ShouldSerializeCollectionItem(SerializeHelper helper, object owner, MethodInfo mi, XtraPropertyInfo itemProperty, object item, XtraItemEventArgs e) {
			IXtraSupportShouldSerializeCollectionItem supportShouldSerializeCollectionItem = owner as IXtraSupportShouldSerializeCollectionItem;
			if(supportShouldSerializeCollectionItem != null) {
				itemProperty.Value = item;
				try {
					return supportShouldSerializeCollectionItem.ShouldSerializeCollectionItem(e);
				} finally {
					itemProperty.Value = null;
				}
			}
			return true;
		}
		protected internal override object InvokeCreateContentPropertyValueMethod(DeserializeHelper helper, XtraItemEventArgs e, PropertyDescriptor prop) {
			IXtraSupportCreateContentPropertyValue supportCreateContentPropertyValue = e.Owner as IXtraSupportCreateContentPropertyValue;
			if(supportCreateContentPropertyValue != null) {
				object value = supportCreateContentPropertyValue.Create(e);
				prop.SetValue(e.Owner, value);
				return value;
			}
			return null;
		}
		protected internal override void InvokeSetIndexCollectionItem(DeserializeHelper helper, string propertyName, XtraSetItemIndexEventArgs e) {
			IXtraSupportDeserializeCollectionItem supportDeserializeCollectionItem = e.Owner as IXtraSupportDeserializeCollectionItem;
			if(supportDeserializeCollectionItem != null)
				supportDeserializeCollectionItem.SetIndexCollectionItem(propertyName, e);
		}
		protected internal override object InvokeCreateCollectionItem(DeserializeHelper helper, string propertyName, XtraItemEventArgs e) {
			IXtraSupportDeserializeCollectionItem supportDeserializeCollectionItem = e.Owner as IXtraSupportDeserializeCollectionItem;
			if(supportDeserializeCollectionItem != null)
				return supportDeserializeCollectionItem.CreateCollectionItem(propertyName, e);
			return null;
		}
		protected internal override void InvokeAfterDeserialize(DeserializeHelper helper, object obj, XtraPropertyInfo bp, object value) {
			IXtraSupportAfterDeserialize supportAfterSerialize = obj as IXtraSupportAfterDeserialize;
			if(supportAfterSerialize != null) {
				bp.Value = value;
				supportAfterSerialize.AfterDeserialize(new XtraItemEventArgs(helper.RootObject, obj, null, bp));
			}
		}
	}
	public abstract class DeflateStreamsArchiveManagerBase {
		#region inner classes
		class ArhiveManagerDeflateStream : DeflateStream {
			DeflateStreamsArchiveManagerBase manager;
			public ArhiveManagerDeflateStream(DeflateStreamsArchiveManagerBase manager, CompressionMode mode)
				: base(manager.baseStream, mode, true) {
				this.manager = manager;
			}
			protected override void Dispose(bool disposing) {
				if(disposing)
					manager.StreamClosed();
				base.Dispose(disposing);
			}
		}
		#endregion
		#region static
		protected const int Int32Size = 4;
		protected const string Prefix = "dxsa";
		public static readonly byte[] PrefixBytes = Encoding.UTF8.GetBytes(Prefix);
		public static readonly byte[] VersionBytes = Encoding.UTF8.GetBytes(AssemblyInfo.Version);
		protected static void ThrowInvalidOperationException() {
			throw new InvalidOperationException();
		}
		#endregion
		protected Stream baseStream;
		bool streamAllocated;
		protected int fStreamCount;
		protected int[] offsets;
		public int StreamCount { get { return fStreamCount; } }
		protected DeflateStreamsArchiveManagerBase(Stream baseStream) {
			this.baseStream = baseStream;
			baseStream.Seek(0, SeekOrigin.Begin);
		}
		protected void StreamClosed() {
			streamAllocated = false;
		}
		protected Stream CreateDeflateStream(CompressionMode mode) {
			streamAllocated = true;
			return new ArhiveManagerDeflateStream(this, mode);
		}
		protected void CheckStreamIndex(int streamIndex) {
			if(streamIndex >= StreamCount || streamAllocated)
				ThrowInvalidOperationException();
		}
	}
	public class DeflateStreamsArchiveWriter : DeflateStreamsArchiveManagerBase {
		int offsetTablePosition;
		int streamIndex;
		int CurrentOffset { get { return (int)baseStream.Position; } }
		public DeflateStreamsArchiveWriter(int streamCount, Stream baseStream)
			: base(baseStream) {
			this.fStreamCount = streamCount;
			WriteBytes(PrefixBytes);
			WriteBytes(VersionBytes);
			WriteInt32(streamCount);
			offsetTablePosition = CurrentOffset;
			offsets = new int[streamCount];
			SkipInt32(streamCount);
		}
		public Stream GetNextStream() {
			CheckStreamIndex(streamIndex);
			offsets[streamIndex] = CurrentOffset;
			streamIndex++;
			return CreateDeflateStream(CompressionMode.Compress);
		}
		public void Close() {
			if(streamIndex < StreamCount)
				ThrowInvalidOperationException();
			baseStream.Seek(offsetTablePosition, SeekOrigin.Begin);
			foreach(int offset in offsets) {
				WriteInt32(offset);
			}
		}
		void WriteInt32(int number) {
			byte[] bytes = BitConverter.GetBytes(number);
			System.Diagnostics.Debug.Assert(Int32Size == bytes.Length);
			WriteBytes(bytes);
		}
		void WriteBytes(byte[] bytes) {
			baseStream.Write(bytes, 0, bytes.Length);
		}
		void SkipInt32(int count) {
			baseStream.Seek(count * Int32Size, SeekOrigin.Current);
		}
	}
	public class DeflateStreamsArchiveReader : DeflateStreamsArchiveManagerBase {
		public static bool IsValidStream(Stream stream) {
			byte[] prefix = new byte[PrefixBytes.Length];
			ReadBytes(stream, prefix);
			return ByteArraysEqual(PrefixBytes, prefix);
		}
		static void ReadBytes(Stream stream, byte[] buffer) {
			int result = stream.Read(buffer, 0, buffer.Length);
			if(result != buffer.Length)
				ThrowInvalidOperationException();
		}
		static bool ByteArraysEqual(byte[] array1, byte[] array2) {
			if(array1.Length != array2.Length)
				return false;
			for(int i = 0; i < array1.Length; i++) {
				if(array1[i] != array2[i])
					return false;
			}
			return true;
		}
		public DeflateStreamsArchiveReader(Stream baseStream)
			: base(baseStream) {
			if(!IsValidStream(baseStream))
				ThrowInvalidOperationException();
			long position = baseStream.Position;
			byte[] bytes = new byte[12];
			baseStream.Read(bytes, 0, bytes.Length);
			string s = new string(Encoding.UTF8.GetChars(bytes));
			System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(s.ToLower(), @"\d+\.\d+\.\d+\.0");
			System.Diagnostics.Debug.Assert(m.Success);
			baseStream.Position = position + m.Value.Length;
			fStreamCount = ReadInt32();
			offsets = ReadInt32Array(StreamCount);
		}
		public Stream GetStream(int streamIndex) {
			CheckStreamIndex(streamIndex);
			baseStream.Seek(offsets[streamIndex], SeekOrigin.Begin);
			return CreateDeflateStream(CompressionMode.Decompress);
		}
		void ReadBytes(byte[] buffer) {
			ReadBytes(baseStream, buffer);
		}
		int ReadInt32() {
			byte[] bytes = new byte[Int32Size];
			ReadBytes(bytes);
			return BitConverter.ToInt32(bytes, 0);
		}
		int[] ReadInt32Array(int count) {
			byte[] bytes = new byte[count * Int32Size];
			ReadBytes(bytes);
			int[] values = new int[count];
			for(int i = 0; i < count; i++) {
				values[i] = BitConverter.ToInt32(bytes, i * Int32Size);
			}
			return values;
		}
	}
}
