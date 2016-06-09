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
using System.Linq;
using DevExpress.Map.Kml.Model;
using System.Globalization;
using System.IO;
namespace DevExpress.Map.Kml {
	public class KmlModelConvert {
		static Random RandomInstance = new Random(DateTime.Now.Millisecond);
		public static bool IsStyleReference(string styleUrl) {
			return styleUrl.StartsWith("#");
		}
		public static string ToStyleId(string styleUrl) {
			return IsStyleReference(styleUrl) ? styleUrl.Substring(1) : styleUrl;
		}
		public string ToString(string value, string defaultValue) {
			return (value != null) ? value.ToString() : defaultValue;
		}
		public double ToDouble(string value, double defaultValue) {
			double result;
			if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				return result;
			return defaultValue;
		}
		public int ToInt(string value, int defaultValue) {
			int result;
			if (int.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
				return result;
			return defaultValue;
		}
		public Unit ToUnit(string value) {
			switch (value) {
				case KmlValueTokens.Pixels:
					return Unit.Pixels;
				case KmlValueTokens.InsetPixels:
					return Unit.InsetPixels;
				default:
					return Unit.Fraction;
			}
		}
		public Uri ToUri(string value, string workingPath) {
			Uri uri;
			if (string.IsNullOrEmpty(value) || !Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out uri))
				return null;
			if(!uri.IsAbsoluteUri)
				if(!string.IsNullOrEmpty(workingPath))
					uri = new Uri(Path.Combine(workingPath, uri.OriginalString));
				else
					return null;
			return uri;
		}
		public Uri ToRelativeUri(string value) {
			Uri uri;
			if (string.IsNullOrEmpty(value) || !Uri.TryCreate(value, UriKind.Relative, out uri))
				return null;
			return uri;
		}
		public List<LatLonPoint> ToLatLonPointList(string value) {
			List<LatLonPoint> result = new List<LatLonPoint>();
			if (string.IsNullOrEmpty(value))
				return result;
			string[] parts = value.Split(new Char[] { ' ', '\t' });
			int count = parts.Length;
			if (count == 0)
				return result;
			for (int i = 0; i < count; i++) {
				string[] coordinates = parts[i].Split(',');
				if (coordinates.Length > 1) {
					double longitude = ToDouble(coordinates[0], 0);
					double latitude = ToDouble(coordinates[1], 0);
					result.Add(new LatLonPoint(latitude, longitude));
				}
			}
			return result;
		}
		public ColorABGR GenerateRandomColor(ColorABGR color) {
			return new ColorABGR(color.A, (byte)RandomInstance.Next(color.G), (byte)RandomInstance.Next(color.B), (byte)RandomInstance.Next(color.G));
		}
	}
}
