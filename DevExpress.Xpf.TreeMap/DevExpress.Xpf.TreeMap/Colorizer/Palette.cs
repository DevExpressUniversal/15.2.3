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
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Collections.Specialized;
using System.Windows.Markup;
namespace DevExpress.Xpf.TreeMap {
	public abstract class PaletteBase : TreeMapDependencyObject {
		protected internal abstract ColorCollection ActualColors { get; }
		public Color this[int index] { get { return Count > 0 ? ActualColors[index % ActualColors.Count] : Colors.Transparent; } }
		public int Count { get { return ActualColors != null ? ActualColors.Count : 0; } }
		public virtual string PaletteName { get { return null; } }
	}
	[ContentProperty("Colors")]
	public class CustomPalette : PaletteBase {
		readonly ColorCollection colors;
		protected internal override ColorCollection ActualColors { get { return Colors; } }
		public ColorCollection Colors { get { return colors; } }
		public override string PaletteName { get { return "Custom"; } }
		public CustomPalette() {
			colors = new ColorCollection();
			colors.CollectionChanged += new NotifyCollectionChangedEventHandler(ColorsChanged);
		}
		void ColorsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			NotifyPropertyChanged("Colors");
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new CustomPalette();
		}
	}
	[TypeConverter(typeof(ColorCollectionTypeConverter))]
	public class ColorCollection : ObservableCollection<Color>, IFormattable {
		public string ToString(string format, IFormatProvider formatProvider) {
			List<string> values = new List<string>();
			foreach (Color color in this)
				values.Add(color.ToString(formatProvider));
			return string.Join(" ", values);
		}
	}
	public class ColorCollectionTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
			return (destType == typeof(string)) || (destType == typeof(InstanceDescriptor));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) {
			if (value is ColorCollection) {
				if (destType == typeof(string)) {
					return ((IFormattable)value).ToString(null, culture);
				}
				if (destType == typeof(InstanceDescriptor)) {
					ConstructorInfo constructor = value.GetType().GetConstructor(new Type[] { typeof(string) });
					if (constructor != null)
						return new InstanceDescriptor(constructor, new object[] { ((ColorCollection)value).ToString() });
				}
			}
			return base.ConvertTo(context, culture, value, destType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			ColorCollection colors = new ColorCollection();
			string str = value as string;
			if (str != null) {
				string[] values = str.Split(' ');
				if (values != null && values.Length > 0) {
					foreach (string val in values) {
						if (!string.IsNullOrWhiteSpace(val)) {
							try {
								Color color = (Color)ColorConverter.ConvertFromString(val);
								colors.Add(color);
							}
							catch {
							}
						}
					}
				}
			}
			return colors;
		}
	}
}
