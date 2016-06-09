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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Xpf.Map.Native;
namespace DevExpress.Xpf.Map {
	[TypeConverter(typeof(GeoPointCollectionConverter))]
	public class GeoPointCollection : ObservableCollection<GeoPoint> {
		public static GeoPointCollection Parse(string source) {
			if (string.IsNullOrEmpty(source))
				throw new ArgumentNullException("source");
			GeoPointCollection result = new GeoPointCollection();
			try {
				string[] elements = source.Split(',', ' ');
				if (elements.Length % 2 != 0)
					throw new Exception();
				for (int i = 0; i < elements.Length; i += 2) {
					double lat = double.Parse(elements[i], CultureInfo.InvariantCulture);
					double lon = double.Parse(elements[i + 1], CultureInfo.InvariantCulture);
					result.Add(new GeoPoint(lat, lon));
				}
			}
			catch {
				throw new FormatException(DXMapStrings.MsgIncorrectGeoPointCollectionStringFormat);
			}
			return result;
		}
		public override string ToString() {
			return this.ToString(CultureInfo.CurrentCulture);
		}
		public string ToString(IFormatProvider provider) {
			string result = String.Empty;
			foreach (GeoPoint point in this)
				result += point.ToString(provider) + " ";
			return result.TrimEnd(' ');
		}
	}
	public class GeoPointCollectionConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string source = value as string;
			if (!string.IsNullOrEmpty(source))
				return GeoPointCollection.Parse(source);
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType != null && destinationType == typeof(string)
				&& value != null && value is GeoPointCollection)
				return ((GeoPointCollection)value).ToString(culture);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
