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

using System.Reflection;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Collections;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using DevExpress.Data.Utils;
using DevExpress.Utils.Design;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using System.Windows.Forms;
using DevExpress.Utils.About;
using System.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class Pair<TFirst, TSecond> {
		TFirst first;
		TSecond second;
		public TFirst First { get { return this.first; } set { this.first = value; } }
		public TSecond Second { get { return this.second; } set { this.second = value; } }
		public Pair() {
		}
		public Pair(TFirst first, TSecond second) {
			this.first = first;
			this.second = second;
		}
		public Pair<TFirst, TSecond> Clone() {
			return new Pair<TFirst, TSecond>(this.first, this.second);
		}
		public override string ToString() {
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			builder.Append('[');
			if(this.First != null) {
				builder.Append(this.First.ToString());
			}
			builder.Append(", ");
			if(this.Second != null) {
				builder.Append(this.Second.ToString());
			}
			builder.Append(']');
			return builder.ToString();
		}
		public override bool Equals(object obj) {
			Pair<TFirst, TSecond> pair = obj as Pair<TFirst, TSecond>;
			if(pair == null)
				return false;
			return object.Equals(first, pair.First) && object.Equals(second, pair.Second);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
namespace DevExpress.XtraPrinting {
#if !SILVERLIGHT
	[
	TypeConverter(typeof(VerticalContentSplittingConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum VerticalContentSplitting {
		Exact,
		Smart
	}
#if !SILVERLIGHT
	[
	TypeConverter(typeof(HorizontalContentSplittingConverter)),
	ResourceFinder(typeof(ResFinder)),
	]
#endif
	public enum HorizontalContentSplitting {
		Exact,
		Smart
	}
	public sealed class StringUtils {
		public static string Join(string separator, string str1, string str2) {
			if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
				return string.Empty;
			else if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
				return str1;
			else if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
				return str2;
			return str1 + separator + str2;
		}
		public static string Join(string separator, params string[] array) {
			string result = string.Empty;
			foreach (string str in array)
				result = Join(separator, result, str);
			return result;
		}
		public static string Join(string separator, List<string> list) {
			return Join(separator, list.ToArray());
		}
	}
}
#if DXPORTABLE
namespace DevExpress.XtraEditors.DXErrorProvider {
	class EnsureNamespaceExistsInAssembly {
	}
}
namespace DevExpress.Data.Utils {
	class EnsureNamespaceExistsInAssembly {
	}
}
namespace DevExpress.Utils.About {
	class EnsureNamespaceExistsInAssembly {
	}
}
#else
namespace DevExpress.XtraPrinting.Native {
	public class XtraSerializableArray<T> {
		T[] values;
		[XtraSerializableProperty]
		public T[] Values {
			get { return values; }
			set { values = value; }
		}
	}
}
namespace DevExpress.Utils {
	using System.Threading;
#if !SILVERLIGHT
#endif
}
#if !SILVERLIGHT
namespace DevExpress.XtraEditors.Controls {
	public class ByteImageConverter {
		protected static Image FromByteArray(byte[] b, int offset) {
			if(b == null || b.Length - offset <= 0) return null;
			Image tempI = null;
			System.IO.MemoryStream s = new System.IO.MemoryStream(b, offset, (int)b.Length - offset);
			try {
				tempI = ImageTool.ImageFromStream(s);
			}
			catch { }
			return tempI;
		}
		public static Image FromByteArray(byte[] b) {
			if(b == null || b.Length == 0) return null;
			Image i = null;
			if(b.Length > 78) {
				if (b[0] == 0x15 && b[1] == 0x1c)  
					i = FromByteArray(b, 78);
			}
			if(i == null)
				i = FromByteArray(b, 0);
			return i;
		}
		static byte[] MetafileToByteArray(Image img) {
			Metafile mf = img as Metafile;
			if(mf == null) return null;
			MemoryStream stream = new MemoryStream();
			using(Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb)) {
				using(Graphics bitmapGraphics = Graphics.FromImage(bitmap)) {
					IntPtr hdc = bitmapGraphics.GetHdc();
					using(Metafile temp = new Metafile(stream, hdc)) {
						using(Graphics tempGraphics = Graphics.FromImage(temp))
							tempGraphics.DrawImage(mf, 0, 0, mf.Width, mf.Height);
						bitmapGraphics.ReleaseHdc(hdc);
					}
				}
			}
			stream.Close();
			return stream.GetBuffer();
		}
		public static ImageFormat GetImageFormatByPixelFormat(Image image) {
			PixelFormat pFormat = PixelFormat.DontCare;
			try {
				if(image != null) pFormat = image.PixelFormat;
			} catch { }
			List<PixelFormat> formats = new List<PixelFormat>() { PixelFormat.Alpha, PixelFormat.Format16bppArgb1555, PixelFormat.Format32bppArgb, PixelFormat.Format32bppPArgb, PixelFormat.Format64bppArgb, PixelFormat.Format64bppPArgb, PixelFormat.PAlpha };
			if(formats.Contains(pFormat)) return ImageFormat.Png;
			return ImageFormat.Jpeg;
		}
		public static byte[] ToByteArray(Image image, ImageFormat imageFormat) {
			if (image == null) return null;
			try {
				if (image is Metafile)
					return MetafileToByteArray(image);
			}
			catch { }
			MemoryStream ms = new MemoryStream();
			try {
				image.Save(ms, image.RawFormat);
			}
			catch {
				try {
					image.Save(ms, imageFormat);
				}
				catch {
					return null;
				}
			}
			byte[] ret = ms.ToArray();
			ms.Close();
			return ret;
		}
		public static byte[] ToByteArray(object obj) {
			byte[] arr;
			if(obj is System.Data.Linq.Binary) {
				return (obj as System.Data.Linq.Binary).ToArray();
			}
			if (!(obj is byte[])) return null;
			try {
				arr = (byte[])obj;
			}
			catch {
				arr = null;
			}
			return arr;
		}
	}
	public enum CryptoServiceProvider { MD5, SHA1 };
	public class ImagesComparer {
		public static CryptoServiceProvider CryptoServiceProvider = CryptoServiceProvider.MD5;
		public static bool AreEqual(Image imageA, Image imageB) {
			return GetImageHash(imageA) == GetImageHash(imageB);
		}
		public static string GetImageHash(Image image) {
			if(image == null) return string.Empty;
			byte[] imageByteArray = ConvertImageToByteArray(image);
			return GetHashString(imageByteArray);
		}
		static byte[] ConvertImageToByteArray(Image image) {
			using(MemoryStream ms = new MemoryStream()) {
				image.Save(ms, FindAppropriateImageFormat(image.RawFormat));
				return ms.ToArray();
			}
		}
		static string GetHashString(byte[] arrayToHash) {
			string computeHash = string.Empty;
			if(ImagesComparer.CryptoServiceProvider == CryptoServiceProvider.SHA1) {
				System.Security.Cryptography.SHA1CryptoServiceProvider encoder = new System.Security.Cryptography.SHA1CryptoServiceProvider();
				foreach(byte b in encoder.ComputeHash(arrayToHash))
					computeHash += b.ToString("X");
				return computeHash;
			} else {
				System.Security.Cryptography.MD5 encoder = System.Security.Cryptography.MD5.Create();
				foreach(byte b in encoder.ComputeHash(arrayToHash))
					computeHash += b.ToString("X");
				return computeHash;
			}
		}
		static ImageFormat FindAppropriateImageFormat(ImageFormat imageFormat) {
			ImageFormat ret = null;
			foreach(ImageCodecInfo info in ImageCodecInfo.GetImageEncoders()) {
				if(imageFormat.Guid == info.FormatID) ret = imageFormat;
			}
			if(ret == null || ret == System.Drawing.Imaging.ImageFormat.MemoryBmp) ret = ImageFormat.Bmp;
			return ret;
		}
	}
	public interface IDatesCollectionOwner {
		void OnCollectionChanged();
	}
	public class DatesCollection : CollectionBase {
		protected IDatesCollectionOwner Owner { get; private set; }
		public DatesCollection() : this(null) { }
		public DatesCollection(IDatesCollectionOwner owner) {
			Owner = owner;
		}
		protected int LockUpdateCount { get; set; }
		protected bool IsLockUpdate { get { return LockUpdateCount > 0; } }
		public void BeginUpdate() {
			LockUpdateCount++;
		}
		public void CancelUpdate() {
			if(LockUpdateCount > 0) LockUpdateCount--;
		}
		public void EndUpdate() {
			CancelUpdate();
			if(LockUpdateCount == 0)
				OnCollectionChanged();
		}
		protected virtual void OnCollectionChanged() {
			if(Owner != null && !IsLockUpdate)
				Owner.OnCollectionChanged();
		}
		public DateTime this[int index] { get { return (DateTime)InnerList[index]; } }
		protected virtual DateTime ExtractDate(DateTime dt) {
			return dt.Date;
		}
		public int Add(DateTime obj) {
			DateTime date = ExtractDate(obj);
			if (InnerList.Contains(date))
				return InnerList.IndexOf(date);
			return List.Add(date);
		}
		public void AddRange(DatesCollection dates) {
			BeginUpdate();
			try {
				int count = dates.Count;
				for(int i = 0; i < count; i++)
					Add(dates[i]);
			}
			finally {
				EndUpdate();
			}
		}
		public void RemoveRange(DatesCollection dates) {
			BeginUpdate();
			try {
				int count = dates.Count;
				for(int i = 0; i < count; i++)
					Remove(dates[i]);
			}
			finally {
				EndUpdate();
			}
		}
		public bool Contains(DateTime obj) {
			DateTime date = ExtractDate(obj);
			return InnerList.Contains(date);
		}
		public void Remove(DateTime obj) {
			DateTime date = ExtractDate(obj);
			if (InnerList.Contains(date))
				List.Remove(date);
		}
		public bool IsContinuous() {
			if (Count == 0)
				return true;
			DatesCollection copy = new DatesCollection();
			copy.InnerList.AddRange(this.InnerList);
			copy.InnerList.Sort(Comparer.Default);
			TimeSpan duration = copy[copy.Count - 1] - copy[0];
			return duration == TimeSpan.FromDays(copy.Count - 1);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			OnCollectionChanged();
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			OnCollectionChanged();
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnCollectionChanged();
		}
	}
	public class EditorButtonTypeConverter : DevExpress.Utils.Design.UniversalTypeConverter {
		protected override ConstructorInfo[] FilterConstructors(ConstructorInfo[] ctors) {
			return Array.FindAll(ctors, ConstructorAllowed);
		}
		static bool ConstructorAllowed(ConstructorInfo cInfo) {
			object[] attributes = cInfo.GetCustomAttributes(
					typeof(EditorButtonPreferredConstructorAttribute), false
				);
			return attributes.Length == 1;
		}
	}
	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
	public sealed class EditorButtonPreferredConstructorAttribute : Attribute { }
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
	public class EditorPainterActivatorAttribute : Attribute {
		Type objectType;
		Type returnType;
		public EditorPainterActivatorAttribute(Type objectType, Type returnType) {
			this.objectType = objectType;
			this.returnType = returnType;
		}
		public Type ObjectType { get { return objectType; } }
		public Type ReturnType { get { return returnType; } }
	}
}
namespace DevExpress.Utils {
	public static class DragAndDropCursors {
		const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
		[System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
		static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
		[System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
		static extern IntPtr FreeLibrary(IntPtr hModule);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern IntPtr LoadCursor(IntPtr hModule, UInt32 cursorIndex);
		const UInt32 noneEffectCursorIndex = 1;
		const UInt32 moveEffectCursorIndex = 2;
		const UInt32 copyEffectCursorIndex = 3;
		const UInt32 linkEffectCursorIndex = 4;
		public static readonly Cursor NoneEffectCursor;
		public static readonly Cursor MoveEffectCursor;
		public static readonly Cursor CopyEffectCursor;
		public static readonly Cursor LinkEffectCursor;
		[System.Security.SecuritySafeCritical]
		static DragAndDropCursors() {
			IntPtr hModule = IntPtr.Zero;
			try {
				hModule = LoadLibraryEx("ole32.dll", IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
				if (hModule == IntPtr.Zero) {
					NoneEffectCursor = Cursors.No;
					MoveEffectCursor = Cursors.Default;
					CopyEffectCursor = Cursors.Default;
					LinkEffectCursor = Cursors.Default;
					return;
				}
				NoneEffectCursor = LoadCursor(hModule, noneEffectCursorIndex, Cursors.No);
				MoveEffectCursor = LoadCursor(hModule, moveEffectCursorIndex, Cursors.Default);
				CopyEffectCursor = LoadCursor(hModule, copyEffectCursorIndex, Cursors.Default);
				LinkEffectCursor = LoadCursor(hModule, linkEffectCursorIndex, Cursors.Default);
			}
			catch (Exception) {
				NoneEffectCursor = Cursors.No;
				MoveEffectCursor = Cursors.Default;
				CopyEffectCursor = Cursors.Default;
				LinkEffectCursor = Cursors.Default;
			}
			finally {
				if (hModule != IntPtr.Zero)
					FreeLibrary(hModule);
			}
		}
		[System.Security.SecuritySafeCritical]
		static Cursor LoadCursor(IntPtr hModule, UInt32 cursorIndex, Cursor defaultCursor) {
			IntPtr hCursor = LoadCursor(hModule, cursorIndex);
			return hCursor != IntPtr.Zero ? new Cursor(hCursor) : defaultCursor;
		}
		public static Cursor GetCursor(DragDropEffects effect) {
			switch (effect) {
				case DragDropEffects.None:
					return NoneEffectCursor;
				case DragDropEffects.Move:
					return MoveEffectCursor;
				case DragDropEffects.Copy:
					return CopyEffectCursor;
				case DragDropEffects.Link:
					return LinkEffectCursor;
				default:
					return Cursors.Default;
			}
		}
	}
}
#endif
namespace DevExpress.CodeParser {
	public interface IToken {
		int StartPosition { get; }
		int EndPosition { get; }
		int Length { get; }
	}
	public interface ITokenCollection {
		IToken this[int index] { get; }
		int Count { get; }
	}
}
#endif
