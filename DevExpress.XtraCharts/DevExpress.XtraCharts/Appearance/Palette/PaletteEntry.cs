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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using DevExpress.Utils.Serializing;
using System.Xml;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(PaletteEntry.PaletteEntryTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PaletteEntry : ICloneable {
		class XmlKeys {
			public const string Entry = "Entry";
			public const string Color = "Color";
			public const string Color2 = "Color2";
		}
		internal class PaletteEntryTypeConverter : TypeConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType == typeof(InstanceDescriptor)) {
					PaletteEntry entry = (PaletteEntry)value;
					return new InstanceDescriptor(typeof(PaletteEntry).GetConstructor(new Type[] { typeof(Color), typeof(Color) }),
						new object[] { entry.Color, entry.Color2 }, true);
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		Palette palette;
		Color color;
		Color color2;
		internal Palette Palette { get { return palette; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public bool IsEmpty { get { return color == Palette.EmptyPaletteColor && color2 == Palette.EmptyPaletteColor; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PaletteEntryColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaletteEntry.Color"),
		XtraSerializableProperty
		]
		public Color Color {
			get { return color; }
			set {
				TestPredefinedFlag();
				color = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PaletteEntryColor2"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PaletteEntry.Color2"),
		XtraSerializableProperty
		]
		public Color Color2 {
			get { return color2; }
			set {
				TestPredefinedFlag();
				color2 = value;
			}
		}
		public PaletteEntry(Color color, Color color2) {
			this.color = color;
			this.color2 = color2;
		}
		public PaletteEntry(Color color)
			: this(color, color) {
		}
		public PaletteEntry()
			: this(Palette.EmptyPaletteColor) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeColor() {
			return color != Palette.EmptyPaletteColor;
		}
		void ResetColor() {
			Color = Palette.EmptyPaletteColor;
		}
		bool ShouldSerializeColor2() {
			return color != Palette.EmptyPaletteColor;
		}
		void ResetColor2() {
			Color2 = Palette.EmptyPaletteColor;
		}
		#endregion
		public object Clone() {
			PaletteEntry clone = new PaletteEntry();
			clone.Assign(this);
			return clone;
		}
		public virtual void Assign(PaletteEntry entry) {
			if (entry == null)
				throw new ArgumentNullException("entry");
			TestPredefinedFlag();
			color = entry.color;
			color2 = entry.color2;
		}
		internal void SetPalette(Palette palette) {
			this.palette = palette;
		}
		internal void WriteToXml(XmlWriter xmlWriter) {
			xmlWriter.WriteStartElement(XmlKeys.Entry);
			XmlUtils.WriteColor(xmlWriter, color, XmlKeys.Color);
			XmlUtils.WriteColor(xmlWriter, color2, XmlKeys.Color2);
			xmlWriter.WriteEndElement();
		}
		internal void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement(XmlKeys.Entry);
			color = XmlUtils.ReadColor(xmlReader, XmlKeys.Color);
			color2 = XmlUtils.ReadColor(xmlReader, XmlKeys.Color2);
			xmlReader.ReadEndElement();
		}
		void TestPredefinedFlag() {
			if (palette != null)
				palette.TestPredefinedFlag();
		}
	}
}
