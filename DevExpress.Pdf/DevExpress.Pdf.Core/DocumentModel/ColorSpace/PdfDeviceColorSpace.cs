#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public enum PdfDeviceColorSpaceKind {
		[PdfFieldName(PdfDeviceColorSpace.GrayName)]
		Gray,
		[PdfFieldName(PdfDeviceColorSpace.RGBName)]
		RGB,
		[PdfFieldName(PdfDeviceColorSpace.CMYKName)]
		CMYK
	};
	public class PdfDeviceColorSpace : PdfColorSpace {
		internal const string GrayName = "DeviceGray";
		internal const string RGBName = "DeviceRGB";
		internal const string CMYKName = "DeviceCMYK";
		readonly PdfDeviceColorSpaceKind kind;
		public PdfDeviceColorSpaceKind Kind { get { return kind; } }
		public override int ComponentsCount {
			get {
				switch (kind) {
					case PdfDeviceColorSpaceKind.Gray:
						return 1;
					case PdfDeviceColorSpaceKind.CMYK:
						return 4;
					default:
						return 3;
				}
			}
		}
		public PdfDeviceColorSpace(PdfDeviceColorSpaceKind kind) {
			this.kind = kind;
		}
		protected internal override PdfColor Transform(PdfColor color) {
			double[] components = color.Components;
			foreach (double component in components)
				if (component < 0 || component > 1) {
					int count = components.Length;
					double[] result = new double[count];
					for (int i = 0; i < count; i++) {
						double value = components[i];
						if (value < 0)
							result[i] = 0;
						else if (value > 1)
							result[i] = 1;
						else
							result[i] = value;
					}
					return new PdfColor(result);
				}
			return color;
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			PdfColorSpaceTransformation transformation;
			switch (kind) {
				case PdfDeviceColorSpaceKind.Gray:
					transformation = new PdfGrayColorSpaceTransformation(width, height, bitsPerComponent, colorKeyMask);
					break;
				case PdfDeviceColorSpaceKind.RGB:
					transformation = new PdfRGBColorSpaceTransformation(width, height, bitsPerComponent, colorKeyMask);
					break;
				case PdfDeviceColorSpaceKind.CMYK:
					transformation = new PdfCMYKColorSpaceTransformation(width, height, bitsPerComponent, colorKeyMask);
					break;
				default:
					return null;
			}
			return transformation.Transform(data);
		}
		protected internal override object Write(PdfObjectCollection collection) {
			return new PdfName(PdfEnumToStringConverter.Convert(kind));
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return Write(collection);
		}
	}
}
