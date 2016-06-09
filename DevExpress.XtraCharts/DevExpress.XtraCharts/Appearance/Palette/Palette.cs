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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public enum PaletteScaleMode { Repeat, Extrapolate }
	[
	TypeConverter(typeof(Palette.PaletteTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Palette : CollectionBase, IPalette, ICloneable {
		class XmlKeys {
			public const string Palette = "Palette";
			public const string Name = "Name";
			public const string ScaleMode = "ScaleMode";
			public const string Items = "Items";
			public const string Count = "Count";
		}
		internal class PaletteTypeConverter : TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					Palette palette = (Palette)value;
					int count = palette.Count;
					PaletteEntry[] entries = new PaletteEntry[count];
					for (int i = 0; i < count; i++)
						entries[i] = (PaletteEntry)palette[i].Clone();
					ConstructorInfo ci = typeof(Palette).GetConstructor(
						new Type[] { typeof(string), typeof(PaletteScaleMode), typeof(PaletteEntry[]) });
					return new InstanceDescriptor(ci, new object[] { palette.Name, palette.ScaleMode, entries }, true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		public static readonly Color EmptyPaletteColor = Color.Silver;
		static Color ConvertColor(Color color, int cycleIndex, int cycleCount) {
			const float minPercent = 0.5f;
			const float maxPercent = 0.5f;
			ColorHSL colorHSL = (ColorHSL)color;
			float diapason = (float)(cycleCount - 1) / cycleCount;
			float minLuminance = colorHSL.Luminance - diapason * minPercent;
			if (minLuminance < colorHSL.MinLuminance)
				minLuminance = colorHSL.MinLuminance;
			float maxLuminance = colorHSL.Luminance + diapason * maxPercent;
			if (maxLuminance > colorHSL.MaxLuminance)
				maxLuminance = colorHSL.MaxLuminance;
			float cycleMiddle = (float)(cycleCount - 1) / 2.0f;
			float cycleDiff = cycleIndex - cycleMiddle;
			if (cycleDiff < 0.0f)
				colorHSL.Luminance = colorHSL.Luminance - (minLuminance - colorHSL.Luminance) * (cycleDiff / cycleMiddle);
			else
				colorHSL.Luminance = colorHSL.Luminance + (maxLuminance - colorHSL.Luminance) * (cycleDiff / cycleMiddle);
			return (Color)colorHSL;
		}
		internal static Palette CreatePredefinedPalette(ChartStringId displayNameId, string resourceName) {
			Palette palette = new Palette(displayNameId);
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			XmlReader reader = new XmlTextReader(stream);
			try {
				palette.ReadFromXml(reader);
			} 
			finally {
				reader.Close();
			}
			return palette;
		}
		string name;
		string displayName;
		PaletteScaleMode scaleMode;
		bool predefined = false;
		ChartStringId? displayNameId;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool IsEmpty { get { return Count == 1 && this[0].IsEmpty; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public string NameSerializable {
			get { return Name; }
			set { SetName(value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public PaletteScaleMode ScaleModeSerializable {
			get { return scaleMode; }
			set { scaleMode = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteDisplayName")]
#endif
		public string DisplayName { 
			get { return displayNameId.HasValue ?  ChartLocalizer.GetString(displayNameId.Value) : displayName; }			
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteName")]
#endif
		public string Name {
			get { return name; }			 
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteScaleMode")]
#endif
		public PaletteScaleMode ScaleMode { get { return scaleMode; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PalettePredefined")]
#endif
		public bool Predefined {
			get { return predefined; }
			internal set { this.predefined = value; }
		}
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PaletteItem")]
#endif
		public PaletteEntry this[int index] { get { return (PaletteEntry)InnerList[index]; } }
		public Palette(string name, PaletteScaleMode scaleMode) {
			this.name = name;
			this.scaleMode = scaleMode;
			displayName = name;
		}
		protected internal Palette(ChartStringId displayNameId, string name) : this(name) {
			this.displayNameId = displayNameId;
			this.predefined = true;
		}
		protected internal Palette(ChartStringId displayNameId) : this(displayNameId, string.Empty) { }
		public Palette(string name) : this(name, PaletteScaleMode.Repeat) { }
		public Palette(string name, PaletteScaleMode scaleMode, PaletteEntry[] entries) : this(name, scaleMode) {
			foreach (PaletteEntry entry in entries)
				Add(entry);
		}
		public Palette(string name, PaletteEntry[] entries) : this(name, PaletteScaleMode.Repeat, entries) { }
		[Obsolete("This constructor is now obsolete. Use the Palette(string name) constructor instead.")]
		public Palette() : this(String.Empty) {
		}
		[Obsolete("This constructor is now obsolete. Use the Palette(string name, PaletteEntry[] entries) constructor instead.")]
		public Palette(PaletteEntry[] entries) : this(String.Empty, entries) {
		}
		PaletteEntry GetEntryByCycle(int index) {
			return this[index % Count];
		}
		void WriteToXml(XmlWriter xmlWriter) {
			xmlWriter.WriteStartElement(XmlKeys.Palette);
			xmlWriter.WriteElementString(XmlKeys.Name, name);
			XmlUtils.WriteEnum(xmlWriter, scaleMode, XmlKeys.ScaleMode);
			xmlWriter.WriteStartElement(XmlKeys.Items);
			xmlWriter.WriteElementString(XmlKeys.Count, Count.ToString());
			foreach (PaletteEntry entry in this)
				entry.WriteToXml(xmlWriter);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}
		int Add(PaletteEntry entry, bool testPredefinedFlag) {
			if (entry == null)
				throw new ArgumentNullException("entry");
			if (testPredefinedFlag)
				TestPredefinedFlag();
			entry.SetPalette(this);
			return InnerList.Add(entry);
		}
		protected void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement(XmlKeys.Palette);
			if (!(this is DefaultPalette))
				this.name = xmlReader.ReadElementString(XmlKeys.Name);
			else
				xmlReader.ReadElementString(XmlKeys.Name);
			scaleMode = (PaletteScaleMode)XmlUtils.ReadEnum(xmlReader, XmlKeys.ScaleMode, typeof(PaletteScaleMode));
			xmlReader.ReadStartElement(XmlKeys.Items);
			InnerList.Clear();
			int count = XmlUtils.ReadInteger(xmlReader, XmlKeys.Count);
			for (int i = 0; i < count; i++) {
				PaletteEntry entry = new PaletteEntry();
				entry.ReadFromXml(xmlReader);
				bool testPredefinedFlag = false;
				Add(entry, testPredefinedFlag);
			}
			xmlReader.ReadEndElement();
			xmlReader.ReadEndElement();
		}	   
		protected internal Image CreateEditorImage() {
			return PaletteUtils.CreateEditorImage(this);
		}
		protected override void OnRemove(int index, object value) {
			TestPredefinedFlag();
		}
		protected override void OnClear() {
			TestPredefinedFlag();
		}
		protected internal void AddDefaultColorIfEmpty() {
			if (Count == 0)
				Add(new PaletteEntry());
		}
		protected internal void SetName(string name) {
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			this.name = name;
			displayName = name;
		}
		protected internal void SetDisplayName(string displayName) {
			this.displayName = displayName;
		}
		protected internal void TestPredefinedFlag() {
			if (predefined)
				throw new PaletteException(ChartLocalizer.GetString(ChartStringId.MsgModifyDefaultPaletteError));
		}
		protected internal PaletteEntry GetEntry(int index, int count, int baseColorNumber) {
			if (Count == 0)
				return new PaletteEntry(Color.Empty, Color.Empty);
			if (baseColorNumber != 0) {
				PaletteEntry baseColorEntry = GetEntryByCycle(baseColorNumber - 1);
				return count == 1 ? baseColorEntry :
					new PaletteEntry(ConvertColor(baseColorEntry.Color, index, count), ConvertColor(baseColorEntry.Color2, index, count));
			}
			PaletteEntry entry = GetEntryByCycle(index);
			if (scaleMode == PaletteScaleMode.Extrapolate) {
				int cycles = (count - 1) / Count + 1;
				if (cycles > 1) {
					int cycleIndex = index / Count;
					return new PaletteEntry(ConvertColor(entry.Color, cycleIndex, cycles), ConvertColor(entry.Color2, cycleIndex, cycles));
				}
			}
			return entry;
		}
		public int Add(PaletteEntry entry) {
			bool testPredefinedFlag = true;
			return Add(entry, testPredefinedFlag);
		} 
		public int Add(Color color, Color color2) {
			return Add(new PaletteEntry(color, color2));
		}
		public int Add(Color color) {
			return Add(color, color);
		}
		public void Assign(Palette palette) {
			if (palette == null)
				throw new ArgumentNullException("palette");
			TestPredefinedFlag();
			name = palette.name;
			displayName = palette.displayName;
			InnerList.Clear();
			foreach (PaletteEntry entry in palette)
				Add((PaletteEntry)entry.Clone());
		}
		public object Clone() {
			Palette palette = new Palette(String.Empty);
			palette.Assign(this);
			return palette;
		}
		public override string ToString() {
			return DisplayName;
		}
		public void SaveToXml(Stream stream) {
			PaletteSerializer.SaveToStream(this, stream);
		}
		public void SaveToXml(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
				SaveToXml(fs);
		}
		public void LoadFromXml(Stream stream) {
			Assign(PaletteSerializer.LoadFromStream(null, stream));
		}
		public void LoadFromXml(string path) {
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
				LoadFromXml(fs);
		}
		#region IPalette implementation
		void IPalette.SetName(string name) {
			SetName(name);
		}
		void IPalette.SetDisplayName(string name) {
			SetDisplayName(name);
		}
		void IPalette.AddDefaultColorIfEmpty() {
			AddDefaultColorIfEmpty();
		}
		#endregion
	}
	public class DefaultPalette : Palette {
		const string actualDefaultPaletteName = "Office";
		const string defaultPaletteName = "Default";
		public static string DefaultPaletteName { get { return defaultPaletteName; } }
		protected internal DefaultPalette(): base(ChartStringId.PltDefault, defaultPaletteName) { 
			LoadColorsFromActualPalette(actualDefaultPaletteName);
		}
		public void LoadColorsFromActualPalette(string actualPaletteName) {
			string resourceName = Palettes.resourcePrefix + actualPaletteName + ".xml";
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			XmlReader reader = new XmlTextReader(stream);
			try {
				ReadFromXml(reader);
			}
			finally {
				reader.Close();
			}
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public static class PaletteSerializer {
		const string PaletteNodeName = "Palette";
		const string PaletteNameAttributeName = "Name";
		const string PaletteEntryNodeName = "PaletteEntry";
		const string ColorAttributeName = "Color";
		const string Color2AttributeName = "Color2";
		static XmlNode AddNode(XmlDocument document, XmlNode parent, string name) {
			XmlNode node = document.CreateElement(name);
			parent.AppendChild(node);
			return node;
		}
		static void AddAttribute(XmlDocument document, XmlNode parent, string name, string value) {
			XmlAttribute attribute = document.CreateAttribute(name);
			attribute.Value = value;
			parent.Attributes.Append(attribute);
		}
		public static void SaveToStream(Palette palette, Stream stream) {
			if (stream == null || palette == null)
				return;
			XmlDocument document = new XmlDocument();
			XmlNode paletteNode = AddNode(document, document, PaletteNodeName);
			AddAttribute(document, paletteNode, PaletteNameAttributeName, palette.Name);
			for (int i = 0; i < palette.Count; i++) {
				XmlNode paletteEntryNode = AddNode(document, paletteNode, PaletteEntryNodeName);
				AddAttribute(document, paletteEntryNode, ColorAttributeName, ColorTranslator.ToHtml(palette[i].Color));
				AddAttribute(document, paletteEntryNode, Color2AttributeName, ColorTranslator.ToHtml(palette[i].Color2));
			}
			document.Save(stream);
		}
		public static void SetPaletteName(Palette palette, string name) {
			palette.SetName(name);
		}
		public static Palette LoadFromStream(IChartContainer chart, Stream stream) {
			if (stream == null)
				return null;
			stream.Seek(0, SeekOrigin.Begin);
			XmlDocument document = new XmlDocument();
			bool isValidXML = true;
			try {
				document.Load(stream);
			}
			catch {
				isValidXML = false;
			}
			if (isValidXML && document.ChildNodes.Count == 1) {
				XmlNode paletteNode = document.ChildNodes[0];
				if (paletteNode.Name == PaletteNodeName && paletteNode.Attributes.Count == 1 && paletteNode.Attributes[0].Name == PaletteNameAttributeName) {
					string name = paletteNode.Attributes[0].Value;
					Palette palette = new Palette(name);
					for (int i = 0; i < paletteNode.ChildNodes.Count; i++) {
						XmlNode paletteEntryNode = paletteNode.ChildNodes[i];
						if (paletteEntryNode.Name == PaletteEntryNodeName && paletteEntryNode.Attributes.Count == 2 &&
							paletteEntryNode.Attributes[0].Name == ColorAttributeName && paletteEntryNode.Attributes[1].Name == Color2AttributeName) {
							Color color1 = ColorTranslator.FromHtml(paletteEntryNode.Attributes[0].Value);
							Color color2 = ColorTranslator.FromHtml(paletteEntryNode.Attributes[1].Value);
							palette.Add(new PaletteEntry(color1, color2));
						}
					}
					return palette;
				}
			}
			if (chart != null)
				chart.ShowErrorMessage(ChartLocalizer.GetString(ChartStringId.MsgPaletteEditorInvalidFile), ChartLocalizer.GetString(ChartStringId.MsgPaletteEditorTitle));
			return null;
		}
	}
}
