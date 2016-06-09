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
using DevExpress.Utils.Serializing.Helpers;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.ComponentModel;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.XtraPrinting.Stubs;
using System.Windows.Media;
using DevExpress.Data.Browsing;
#else
using System.Drawing.Imaging;
using System.ComponentModel;
#endif
namespace DevExpress.Utils.Serializing {
	public abstract class StructConverter<T> : IOneTypeObjectConverter2 {
		public abstract Type Type { get; }
		protected virtual char Delimiter { get { return ','; } }
		protected StructConverter() {
		}
		public string ToString(object obj) {
			T[] values = GetValues(obj);
			string[] strings = new string[values.Length];
			for(int i = 0; i < values.Length; i++) {
				strings[i] = ElementToString(values[i]);
			}
			return string.Join(Delimiter.ToString(), strings);
		}
		public object FromString(string str) {
			string[] values = str.Split(Delimiter);
			T[] fValues = new T[values.Length];
			for(int i = 0; i < values.Length; i++) {
				fValues[i] = ToType(values[i]);
			}
			return CreateObject(fValues);
		}
		protected abstract T[] GetValues(object obj);
		protected abstract object CreateObject(T[] values);
		protected abstract string ElementToString(T obj);
		protected abstract T ToType(string str);
		public virtual bool CanConvertFromString(string str) {
			return str != null && !str.StartsWith("@");
		}
	}
	public abstract class StructFloatConverter : StructConverter<float> {
		protected StructFloatConverter() {
		}
		protected override string ElementToString(float obj) {
			return XmlConvert.ToString(obj);
		}
		protected override float ToType(string str) {
			return XmlConvert.ToSingle(str);
		}
	}
	public abstract class StructStringConverter : StructConverter<string> {
		protected StructStringConverter() {
		}
		protected override string ElementToString(string obj) {
			return obj;
		}
		protected override string ToType(string str) {
			return str;
		}
	}
	public abstract class StructIntConverter : StructConverter<int> {
		protected StructIntConverter() {
		}
		protected override string ElementToString(int obj) {
			return XmlConvert.ToString(obj);
		}
		protected override int ToType(string str) {
			return XmlConvert.ToInt32(str);
		}
	}
	public class PrintingSystemXmlSerializer : XmlXtraSerializer {
	#region inner classes
		class LocalObjectConverterImplementation {
			ObjectConverters Converters;
			public LocalObjectConverterImplementation() {
				Converters = new ObjectConverters();
			}
			public void RegisterConverter(IOneTypeObjectConverter converter) {
				Converters.RegisterConverter(converter);
			}
			public void UnregisterConverter(Type type) {
				Converters.UnregisterConverter(type);
			}
			public bool CanConvertToString(Type type) {
				return Converters.IsConverterExists(type);
			}
			public string ConvertToString(object obj) {
				return Converters.ConvertToString(obj);
			}
		}
		class PrintingSystemXmlPropertyInfo : XmlXtraPropertyInfo {
			protected override ObjectConverterImplementation ObjectConverterImplementation { get { return ObjectConverterInstance; } }
			public PrintingSystemXmlPropertyInfo(string name, Type propertyType, object val, bool isKey)
				: base(name, propertyType, val, isKey) {
			}
		}
		class PrintingSystemObjectConverterImplementation : ObjectConverterImplementation {
			protected override string SerialzeWithBinaryFormatter(object obj) {
#if DEBUGTEST && !SL
				System.Diagnostics.Debug.Fail("Binary formatter fails in medium trust");
#endif
				return base.SerialzeWithBinaryFormatter(obj);
			}
		}
		class GuidConverter : IOneTypeObjectConverter {
			public static readonly IOneTypeObjectConverter Instance = new GuidConverter();
			public Type Type { get { return typeof(Guid); } }
			public string ToString(object obj) {
				return ((Guid)obj).ToString("D");
			}
			public object FromString(string str) {
				return Guid.Parse(str);
			}
		}
		class DateTimeConverter : IOneTypeObjectConverter {
			public static readonly IOneTypeObjectConverter Instance = new DateTimeConverter();
			public Type Type { get { return typeof(DateTime); } }
			public string ToString(object obj) {
				DateTime time = (DateTime)obj;
				if(time == DateTime.MinValue)
					return string.Empty;
				CultureInfo culture = CultureInfo.InvariantCulture;
				return time.TimeOfDay.TotalSeconds == 0.0 ? time.ToString("yyyy-MM-dd", culture) :
					time.ToString(culture);
			}
			public object FromString(string str) {
				string s = str.Trim();
				if(s.Length == 0)
					return DateTime.MinValue;
				CultureInfo culture = CultureInfo.InvariantCulture;
				DateTimeFormatInfo provider = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
				return provider != null ? DateTime.Parse(s, provider) : DateTime.Parse(s, culture);
			}
		}
		class DBNullConverter : IOneTypeObjectConverter {
			public static readonly DBNullConverter Instance = new DBNullConverter();
			public Type Type { get { return typeof(DBNull); } }
			DBNullConverter() {
			}
			public string ToString(object obj) {
				return "Null";
			}
			public object FromString(string str) {
				return DBNull.Value;
			}
		}
#if !DXPORTABLE
		class ImageConverter : IOneTypeObjectConverter {
			public static readonly ImageConverter Instance = new ImageConverter();
			public virtual Type Type { get { return typeof(Image); } }
			protected ImageConverter() {
			}
			public string ToString(object obj) {
				return Convert.ToBase64String(PSConvert.ImageToArray((Image)obj));
			}
			public object FromString(string str) {
				return PSConvert.ImageFromArray(Convert.FromBase64String(str));
			}
		}
		class BitmapConverter : ImageConverter {
			public new static readonly BitmapConverter Instance = new BitmapConverter();
			public override Type Type { get { return typeof(Bitmap); } }
			BitmapConverter() {
			}
		}
		class MetafileConverter : ImageConverter {
			public new static readonly MetafileConverter Instance = new MetafileConverter();
			public override Type Type { get { return typeof(Metafile); } }
			MetafileConverter() {
			}
		}
#endif
		class PointConverter : StructIntConverter {
			public static readonly PointConverter Instance = new PointConverter();
			public override Type Type { get { return typeof(Point); } }
			PointConverter() {
			}
			protected override int[] GetValues(object obj) {
				Point point = (Point)obj;
				return new int[] { point.X, point.Y };
			}
			protected override object CreateObject(int[] values) {
				return new Point(values[0], values[1]);
			}
		}
		class PointFConverter : StructFloatConverter {
			public static readonly PointFConverter Instance = new PointFConverter();
			public override Type Type { get { return typeof(PointF); } }
			PointFConverter() {
			}
			protected override float[] GetValues(object obj) {
				PointF point = (PointF)obj;
				return new float[] { point.X, point.Y };
			}
			protected override object CreateObject(float[] values) {
				return new PointF(values[0], values[1]);
			}
		}
		class SizeConverter : StructIntConverter {
			public static readonly SizeConverter Instance = new SizeConverter();
			public override Type Type { get { return typeof(Size); } }
			SizeConverter() {
			}
			protected override int[] GetValues(object obj) {
				Size size = (Size)obj;
				return new int[] { size.Width, size.Height };
			}
			protected override object CreateObject(int[] values) {
				return new Size(values[0], values[1]);
			}
		}
		class SizeFConverter : StructFloatConverter {
			public static readonly SizeFConverter Instance = new SizeFConverter();
			public override Type Type { get { return typeof(SizeF); } }
			SizeFConverter() {
			}
			protected override float[] GetValues(object obj) {
				SizeF size = (SizeF)obj;
				return new float[] { size.Width, size.Height };
			}
			protected override object CreateObject(float[] values) {
				return new SizeF(values[0], values[1]);
			}
		}
		class RectangleConverter : StructIntConverter {
			public static readonly RectangleConverter Instance = new RectangleConverter();
			public override Type Type { get { return typeof(Rectangle); } }
			RectangleConverter() {
			}
			protected override int[] GetValues(object obj) {
				Rectangle rectangle = (Rectangle)obj;
				return new int[] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height };
			}
			protected override object CreateObject(int[] values) {
				return new Rectangle(values[0], values[1], values[2], values[3]);
			}
		}
		class RectangleFConverter : StructFloatConverter {
			public static readonly RectangleFConverter Instance = new RectangleFConverter();
			public override Type Type { get { return typeof(RectangleF); } }
			RectangleFConverter() {
			}
			protected override float[] GetValues(object obj) {
				RectangleF rectangle = (RectangleF)obj;
				return new float[] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height };
			}
			protected override object CreateObject(float[] values) {
				return new RectangleF(values[0], values[1], values[2], values[3]);
			}
		}
		public class FontConverter : StructStringConverter {
			public static readonly FontConverter Instance = new FontConverter();
			public override Type Type { get { return typeof(Font); } }
			protected override string[] GetValues(object obj) {
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
				string result = (string)converter.ConvertTo(null, CultureInfo.InvariantCulture, obj, typeof(String));
				int charSet = ((Font)obj).GdiCharSet;
				return new string[] { charSet == 1 ? result : result + ", charSet=" + charSet };
			}
			protected override object CreateObject(string[] values) {
				bool hasCharSet = values[values.Length - 1].Contains("charSet");
				string fontValue = string.Join(",", values, 0, hasCharSet ? values.Length - 1 : values.Length);
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
				Font font = (Font)converter.ConvertFrom(null, CultureInfo.InvariantCulture, fontValue);				
				return hasCharSet ? new Font(font.FontFamily, font.Size, font.Style, font.Unit, GetCharSet(values[values.Length - 1])) : font;
			}
			byte GetCharSet(string value) {
				int index = value.IndexOf("=") + 1;
				return Convert.ToByte(value.Substring(index, value.Length - index));
			}
		}
		public class ColorConverter : StructStringConverter {
			static Color GetColor(byte a, byte r, byte g, byte b) {
				Color color = DXColor.FromArgb(a, r, g, b);
				TryFindPredefinedColor(ref color);
				return color;
			}
			static Color GetColor(byte r, byte g, byte b) {
				return GetColor(0xFF, r, g, b);
			}
			public static readonly ColorConverter Instance = new ColorConverter();
			const string HexPrefix = "0x";
			static string ByteToHex(byte value) {
				return value.ToString();
			}
			static byte NumToByte(string value) {
				int fromBase = 10;
				if(value.StartsWith(HexPrefix)) {
					fromBase = 16;
					value = value.Remove(0, HexPrefix.Length);
				}
				return Convert.ToByte(value.Trim(), fromBase);
			}
			static bool TryFindPredefinedColorAndName(Color color, out KeyValuePair<string, Color> result) {
				result = new KeyValuePair<string, Color>();
				foreach(KeyValuePair<string, Color> pair in DXColor.PredefinedColors)
					if(pair.Value.ToArgb() == color.ToArgb()) {
						result = pair;
						return true;
					}
				return false;
			}
			static bool TryFindPredefinedColor(ref Color color) {
				KeyValuePair<string, Color> result = new KeyValuePair<string,Color>();
				bool isFound = TryFindPredefinedColorAndName(color, out result);
				if(isFound)
					color = result.Value;
				return isFound;
			}
			static bool TryFindPredefinedColorName(Color color, out string name) {
				name = null;
				KeyValuePair<string, Color> result = new KeyValuePair<string,Color>();
				bool isFound = TryFindPredefinedColorAndName(color, out result);
				if(isFound)
					name = result.Key;
				return isFound;
			}
			public override Type Type {
				get { return typeof(Color); }
			}
			protected override string[] GetValues(object obj) {
				Color color = (Color)obj;
				string colorName;
				if(TryFindPredefinedColorName(color, out colorName))
					return new string[] { colorName };
				return new string[] { ByteToHex(color.A), ByteToHex(color.R), ByteToHex(color.G), ByteToHex(color.B) };
			}
			protected override object CreateObject(string[] values) {
				if(values.Length == 4)
					return GetColor(NumToByte(values[0]), NumToByte(values[1]), NumToByte(values[2]), NumToByte(values[3]));
				if(values.Length == 3)
					return GetColor(NumToByte(values[0]), NumToByte(values[1]), NumToByte(values[2]));
				if(values.Length != 1)
					throw new NotSupportedException();
				string value = values[0];
				Color color;
				if(DXColor.PredefinedColors.TryGetValue(value, out color))
					return color;
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
				if(converter == null)
					throw new NotSupportedException();
				return converter.ConvertFromString(value);
			}
		}
		public class ImageSizeModeConverter : StructStringConverter {
			public static readonly ImageSizeModeConverter Instance = new ImageSizeModeConverter();
			public override Type Type {
				get { return typeof(ImageSizeMode); }
			}
			protected override object CreateObject(string[] values) {
				string value = values[0];
				if(value == "Zoom")
					return ImageSizeMode.ZoomImage;
				return Enum.Parse(typeof(ImageSizeMode), value, false);
			}
			protected override string[] GetValues(object obj) {
				return new string[] { obj.ToString() };
			}
		}
#endregion
#region static
		[ThreadStatic]
		static LocalObjectConverterImplementation localObjectConverter;
		const string Name = "PreviewSerializer";
		public static readonly ObjectConverterImplementation ObjectConverterInstance;
		static LocalObjectConverterImplementation LocalObjectConverter {
			get {
				if(localObjectConverter == null)
					localObjectConverter = new LocalObjectConverterImplementation();
				return localObjectConverter; 
			}
		}
		static PrintingSystemXmlSerializer() {
			ObjectConverterInstance = new PrintingSystemObjectConverterImplementation();
			RegisterConverter(PointConverter.Instance);
			RegisterConverter(SizeConverter.Instance);
			RegisterConverter(RectangleConverter.Instance);
			RegisterConverter(PointFConverter.Instance);
			RegisterConverter(SizeFConverter.Instance);
			RegisterConverter(RectangleFConverter.Instance);
#if !DXPORTABLE
			RegisterConverter(ImageConverter.Instance);
			RegisterConverter(BitmapConverter.Instance);
			RegisterConverter(MetafileConverter.Instance);
#endif
			RegisterConverter(DBNullConverter.Instance);
			RegisterConverter(ColorConverter.Instance);
			RegisterConverter(ImageSizeModeConverter.Instance);
			RegisterConverter(DateTimeConverter.Instance);
			RegisterConverter(GuidConverter.Instance);
#if !SL
			RegisterConverter(FontConverter.Instance);
#else
			RegisterConverter(DevExpress.Xpf.ComponentModel.FontConverter.Instance);
#endif
		}
		public static void RegisterConverter(IOneTypeObjectConverter converter) {
			ObjectConverterInstance.RegisterConverter(converter);
		}
		public static void UnregisterConverter(Type type) {
			ObjectConverterInstance.UnregisterConverter(type);
		}
		public static void RegisterLocalConverter(IOneTypeObjectConverter converter) {
			LocalObjectConverter.RegisterConverter(converter);
		}
		public static void UnregisterLocalConverter(Type type) {
			LocalObjectConverter.UnregisterConverter(type);
		}
		internal static XtraPropertyInfo ReadInfoCore(XmlReader tr) {
			tr.Read();
			tr.MoveToContent();
			return ReadInfoCore(tr, true);
		}
		public static XtraPropertyInfo ReadInfo(XmlReader tr, bool skipZeroDepth) {
			tr.MoveToContent();
			return ReadInfoCore(tr, skipZeroDepth);
		}
		static XtraPropertyInfo ReadInfoCore(XmlReader tr, bool skipZeroDepth) {
			if(tr.NodeType == XmlNodeType.Element) {
				XtraPropertyInfo info = CreateXmlPropertyInfo(tr.Name, null, null, true);
				bool isEmptyElement = tr.IsEmptyElement;
				ReadAttributes(tr, info, skipZeroDepth);
				if(isEmptyElement)
					return info;
				while(tr.ReadState != ReadState.EndOfFile) {
					tr.Read();
					if(tr.NodeType == XmlNodeType.EndElement)
						break;
					XtraPropertyInfo child = ReadInfoCore(tr, false);
					if(child != null)
						info.ChildProperties.Add(child);
				}
				return info;
			}
			return null;
		}
		static void ReadAttributes(XmlReader tr, XtraPropertyInfo info, bool skipZeroDepth) {
			if(!(skipZeroDepth && tr.Depth == 0)) {
				while(tr.MoveToNextAttribute()) {
					Type type = null;
					if(tr.Name.EndsWith("_type")) {
						type = Type.GetType(tr.Value, false);
						if(type == null)
							type = ObjectConverterInstance.ResolveType(tr.Value);
						tr.MoveToNextAttribute();
					}
					info.ChildProperties.Add(CreateXmlPropertyInfo(tr.Name, type, tr.Value, false));
				}
			}
		}
		static PrintingSystemXmlPropertyInfo CreateXmlPropertyInfo(string name, Type propertyType, object val, bool isKey) {
			return new PrintingSystemXmlPropertyInfo(name, propertyType, val, isKey);
		}
#endregion
		protected override string SerializerName { get { return Name; } }
		protected override string Version { get { return AssemblyInfo.Version; } }
		protected override void WriteApplicationAttribute(string appName, XmlWriter tw) {
		}
		protected override void WriteStartDocument(XmlWriter tw) {
			tw.WriteStartDocument();
		}
		protected override void SerializeLevelCore(XmlWriter tw, IXtraPropertyCollection props) {
			if(!props.IsSinglePass) {
				foreach(XtraPropertyInfo p in props) {
					if(!p.IsKey)
						SerializeAttributeProperty(tw, p);
				}
			}
			foreach(XtraPropertyInfo p in props) {
				if(p.IsKey)
					SerializeContentProperty(tw, p);
			}
		}
		void SerializeAttributeProperty(XmlWriter tw, XtraPropertyInfo p) {
			object val = p.Value;
			if(val != null) {
				Type type = val.GetType();
				if(!type.IsPrimitive())
					val = ObjToString(val);
			}
			if(p.Value != null) {
				if(object.Equals(p.PropertyType, typeof(object)))
					tw.WriteAttributeString(p.Name + "_type", GetObjectTypeName(p.Value));
				tw.WriteAttributeString(p.Name, XmlObjectToString(val));
			}
		}
		string ObjToString(object val) {
			if(localObjectConverter != null && localObjectConverter.CanConvertToString(val.GetType()))
				return localObjectConverter.ConvertToString(val);
			return ObjectConverterInstance.ObjectToString(val);
		}
		protected virtual string GetObjectTypeName(object obj) {
			return obj.GetType().FullName;
		}
		internal void SerializeObject(Stream stream, XtraPropertyInfo p) {
			XmlWriter tw = CreateXmlTextWriter(stream);
			SerializeContentPropertyCore(tw, p);
			tw.Flush();
		}
		void SerializeContentProperty(XmlWriter tw, XtraPropertyInfo p) {
			if(p.ChildProperties.Count == 0)
				return;
			SerializeContentPropertyCore(tw, p);
		}
		private void SerializeContentPropertyCore(XmlWriter tw, XtraPropertyInfo p) {
			tw.WriteStartElement(p.Name.Replace("$", string.Empty));
			SerializeLevel(tw, p.ChildProperties);
			tw.WriteEndElement();
		}
		protected override IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			XmlReader tr = CreateReader(stream);
			return new DeserializationVirtualXtraPropertyCollection(tr);
		}
		internal XtraPropertyInfo DeserializeObject(Stream stream) {
			XmlReader tr = CreateReader(stream);
			return ReadInfo(tr, false);
		}
	}
#if !SL
	public class IndependentPagesPrintingSystemSerializer : PrintingSystemXmlSerializer {
		protected override bool SerializeCore(Stream stream, IXtraPropertyCollection props, string appName) {
			DeflateStreamsArchiveWriter writer = new DeflateStreamsArchiveWriter(props.Count, stream);
			foreach(XtraPropertyInfo propertyInfo in props) {
				using(Stream elementStream = writer.GetNextStream()) {
					new PrintingSystemXmlSerializer().SerializeObject(elementStream, propertyInfo);
				}
			}
			writer.Close();
			return true;
		}
		protected override IXtraPropertyCollection DeserializeCore(Stream stream, string appName, IList objects) {
			return new IndependentPagesDeserializationVirtualXtraPropertyCollection(stream, objects);
		}
	}
	public class IndependentPagesDeserializationVirtualXtraPropertyCollection : VirtualXtraPropertyCollectionBase {
		Stream stream;
		IList objects;
		public IndependentPagesDeserializationVirtualXtraPropertyCollection(Stream stream, IList objects) {
			this.stream = stream;
			this.objects = objects;
		}
		protected override CollectionItemInfosEnumeratorBase CreateEnumerator() {
			return new DeserializationCollectionItemInfosEnumerator(stream, objects);
		}
	}
	public class DeserializationCollectionItemInfosEnumerator : CollectionItemInfosEnumeratorBase {
		DeflateStreamsArchiveReader reader;
		IList objects;
		int index;
		public DeserializationCollectionItemInfosEnumerator(Stream stream, IList objects)
			: base() {
			reader = new DeflateStreamsArchiveReader(stream);
			this.objects = objects;
			Reset();
		}
		public override void Reset() {
			base.Reset();
			index = -1;
		}
		protected override bool MoveNextCore() {
			index++;
			if(index >= reader.StreamCount) {
				currentInfo = null;
				return false;
			}
			XtraObjectInfo objectInfo = (XtraObjectInfo)objects[index];
			if(objectInfo.Skip) {
				currentInfo = new XtraPropertyInfo();
				return true;
			}
			using(Stream stream = reader.GetStream(index)) {
				currentInfo = new PrintingSystemXmlSerializer().DeserializeObject(stream);
			}
			return true;
		}
	}
#endif
}
