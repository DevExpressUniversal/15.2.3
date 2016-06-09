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
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.Utils.StoredObjects {
	static class RepositoryProviderExtensions {
		public static RepositoryProvider CreateInstance() {
			RepositoryProvider instance = new RepositoryProvider();
			IObjectRepository<Page> pageRepository = new PersistentObjectRepository<Page>(new MMFFloatMemberDictionary(Path.GetTempFileName()));
			instance.RegisterRepository<Page>(pageRepository);
			IObjectRepository<BrickBase> brickRepository = new PersistentObjectRepository<BrickBase>(new MMFFloatMemberDictionary(Path.GetTempFileName()));
			instance.RegisterRepository<BrickBase>(brickRepository);
			IObjectRepository<BrickStyle> styleRepository = new LiveObjectRepository<BrickStyle>();
			instance.RegisterRepository<BrickStyle>(styleRepository);
			IObjectRepository<IBrickOwner> ownerRepository = new LiveObjectRepository<IBrickOwner>();
			instance.RegisterRepository<IBrickOwner>(ownerRepository);
			IPersistentDictionary dict = new MMFFloatMemberDictionary(Path.GetTempFileName());
			instance.RegisterRepository<string>(new StringRepository(dict));
			return instance;
		}
		public static long StoreObject<T>(this IRepositoryProvider provider, T obj) {
			IObjectRepository<T> repository;
			return provider.TryGetRepository<T>(out repository) ?
				repository.StoreObject(provider, obj) :
				StoredObjectExtentions.UndefinedId;
		}
		public static T RestoreObject<T>(this IRepositoryProvider provider, long id, T defaultValue) {
			if(id == StoredObjectExtentions.UndefinedId)
				return defaultValue;
			IObjectRepository<T> repository;
			if(!provider.TryGetRepository<T>(out repository))
				throw new InvalidRestoreException();
			T obj;
			if(!repository.TryRestoreObject(provider, id, out obj))
				throw new InvalidRestoreException();
			return obj;
		}
	}
	static class BinaryWriterExtentions {
		public static void WritePageBreakInfo(this BinaryWriter writer, PageBreakInfo info) {
			writer.WriteNullable(info.NextPageData, () => writer.WriteCustomPageData(info.NextPageData));
			writer.WriteValueInfo(info);
		}
		static void WriteCustomPageData(this BinaryWriter writer, CustomPageData data) {
			writer.WriteMargins(data.Margins);
			writer.WriteSize(data.PageSize);
			writer.WriteNullable(data.Landscape, () => writer.Write(data.Landscape.Value));
			writer.WriteNullable(data.PaperKind, () => writer.Write((Int32)data.PaperKind.Value));
		}
		static void WriteNullable(this BinaryWriter writer, object value, Action callback) {
			if(value != null) {
				writer.Write(1);
				callback();
			} else
				writer.Write(0);
		}
		public static void WriteValueInfo(this BinaryWriter writer, ValueInfo info) {
			writer.Write(info.Value);
			writer.Write(info.Active);
		}
		public static void WriteMultiColumn(this BinaryWriter writer, MultiColumn mc) {
			writer.Write((Int32)mc.Order);
			writer.Write(mc.ColumnCount);
			writer.Write(mc.ColumnWidth);
		}
		public static void WriteRectangleF(this BinaryWriter writer, RectangleF rect) {
			writer.WritePointF(rect.Location);
			writer.WriteSizeF(rect.Size);
		}
		public static void WritePointF(this BinaryWriter writer, PointF p) {
			writer.Write(p.X);
			writer.Write(p.Y);
		}
		public static void WriteSizeF(this BinaryWriter writer, SizeF s) {
			writer.Write(s.Width);
			writer.Write(s.Height);
		}
		public static void WriteSize(this BinaryWriter writer, Size s) {
			writer.Write(s.Width);
			writer.Write(s.Height);
		}
		public static void WriteMargins(this BinaryWriter writer, Margins m) {
			writer.Write(m.Left);
			writer.Write(m.Right);
			writer.Write(m.Top);
			writer.Write(m.Bottom);
		}
		public static void WriteMarginsF(this BinaryWriter writer, MarginsF m) {
			writer.Write(m.Left);
			writer.Write(m.Right);
			writer.Write(m.Top);
			writer.Write(m.Bottom);
		}
		public static void WritePageData(this BinaryWriter writer, ReadonlyPageData data) {
			writer.WriteMarginsF(data.MarginsF);
			writer.WriteMarginsF(data.MinMarginsF);
			writer.Write((Int32)data.PaperKind);
			writer.WriteSize(data.Size);
			writer.Write(data.Landscape);
		}
		public static void WriteStoredObjects<TObj, T>(this BinaryWriter writer, IList<TObj> objects, IRepositoryProvider provider) where TObj : T, IStoredObject {
			if(objects == null) {
				writer.Write(0);
				return;
			} 
			writer.Write(objects.Count);
			IObjectRepository<T> repository;
			provider.TryGetRepository<T>(out repository);
			foreach(TObj obj in objects) {
				if(!obj.HasId())
					repository.StoreObject(provider, obj);
				writer.Write(obj.Id);
			}
		}
	}
	static class BinaryReaderExtentions {
		public static PageBreakInfo ReadPageBreakInfo(this BinaryReader reader) {
			CustomPageData data = null;
			reader.ReadNullable(() => data = reader.ReadCustomPageData());
			PageBreakInfo info = null;
			reader.ReadValueInfo((value, active) => {
				info = new PageBreakInfo(value, data);
				info.Active = active;
			});
			return info;
		}
		static CustomPageData ReadCustomPageData(this BinaryReader reader) {
			CustomPageData data = new CustomPageData() {
				Margins = reader.ReadMargins(),
				PageSize = reader.ReadSize()
			};
			reader.ReadNullable(() => data.Landscape = reader.ReadBoolean());
			reader.ReadNullable(() => data.PaperKind = (PaperKind)reader.ReadInt32());
			return data;
		}
		static void ReadNullable(this BinaryReader reader, Action callback) {
			if(reader.ReadInt32() == 1)
				callback();
		}
		public static void ReadValueInfo(this BinaryReader reader, Action<float, bool> callback) {
			float value = reader.ReadSingle();
			bool active = reader.ReadBoolean();
			callback(value, active);
		}
		public static MultiColumn ReadMultiColumn(this BinaryReader reader) {
			ColumnLayout order = (ColumnLayout)reader.ReadInt32();
			int columnCount = reader.ReadInt32();
			int columnWidth = reader.ReadInt32();
			return new MultiColumn(columnCount, columnWidth, order);
		}
		public static RectangleF ReadRectangleF(this BinaryReader reader) {
			PointF p = reader.ReadPointF();
			SizeF s = reader.ReadSizeF();
			return new RectangleF(p, s);
		}
		public static PointF ReadPointF(this BinaryReader reader) {
			float x = reader.ReadSingle();
			float y = reader.ReadSingle();
			return new PointF(x, y);
		}
		public static SizeF ReadSizeF(this BinaryReader reader) {
			float width = reader.ReadSingle();
			float height = reader.ReadSingle();
			return new SizeF(width, height);
		}
		public static Size ReadSize(this BinaryReader reader) {
			int width = reader.ReadInt32();
			int height = reader.ReadInt32();
			return new Size(width, height);
		}
		public static Margins ReadMargins(this BinaryReader reader) {
			int l = reader.ReadInt32();
			int r = reader.ReadInt32();
			int t = reader.ReadInt32();
			int b = reader.ReadInt32();
			return new Margins(l, r, t, b);
		}
		public static MarginsF ReadMarginsF(this BinaryReader reader) {
			float l = reader.ReadSingle();
			float r = reader.ReadSingle();
			float t = reader.ReadSingle();
			float b = reader.ReadSingle();
			return new MarginsF(l, r, t, b);
		}
		public static ReadonlyPageData ReadPageData(this BinaryReader reader) {
			MarginsF margins = reader.ReadMarginsF();
			MarginsF minMargins = reader.ReadMarginsF();
			PaperKind paperKind = (PaperKind)reader.ReadInt32();
			Size size = reader.ReadSize();
			bool landscape = reader.ReadBoolean();
			return new ReadonlyPageData(margins, minMargins, paperKind, size, landscape);
		}
		public static void ReadStoredObjects<TObj, T>(this BinaryReader reader, IList<TObj> objects, IRepositoryProvider provider) where TObj : T {
			reader.ReadStoredObjects<TObj, T>(provider, obj => objects.Add(obj));
		}
		public static void ReadStoredObjects<TObj, T>(this BinaryReader reader, IRepositoryProvider provider, Action<TObj> callback) where TObj : T {
			long[] ids = reader.ReadObjectIds();
			IObjectRepository<T> repository;
			provider.TryGetRepository<T>(out repository);
			for(int i = 0; i < ids.Length; i++) {
				TObj obj;
				if(!repository.TryRestoreObject<TObj>(provider, ids[i], out obj))
					throw new InvalidRestoreException();
				callback(obj);
			}
		}
		public static long[] ReadObjectIds(this BinaryReader reader) {
			long[] result = new long[reader.ReadInt32()];
			for(int i = 0; i < result.Length; i++)
				result[i] = reader.ReadObjectId();
			return result;
		}
		public static long ReadObjectId(this BinaryReader reader) {
			return reader.ReadInt64();
		}
	}
}
