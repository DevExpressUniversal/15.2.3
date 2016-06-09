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

using System;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfIndexedColorSpace : PdfCustomColorSpace {
		internal const string TypeName = "Indexed";
		readonly PdfColorSpace baseColorSpace;
		readonly int maxIndex;
		readonly byte[] lookupTable;
		public PdfColorSpace BaseColorSpace { get { return baseColorSpace; } }
		public int MaxIndex { get { return maxIndex; } }
		public byte[] LookupTable { get { return lookupTable; } }
		public override int ComponentsCount { get { return 1; } }
		internal PdfIndexedColorSpace(PdfObjectCollection objects, IList<object> array, PdfResources resources) {
			if (array.Count != 4)
				PdfDocumentReader.ThrowIncorrectDataException();
			baseColorSpace = PdfColorSpace.Parse(objects, array[1], resources);
			object value = array[2];
			if (objects != null)
				value = objects.TryResolve(value);
			if (!(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			maxIndex = (int)value;
			if (maxIndex < 0 || maxIndex > 255)
				PdfDocumentReader.ThrowIncorrectDataException();
			value = array[3];
			if (objects != null)
				value = objects.TryResolve(value);
			lookupTable = value as byte[];
			if (lookupTable == null) {
				PdfReaderStream stream = value as PdfReaderStream;
				if (stream == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				lookupTable = stream.GetData(true);
				if (lookupTable == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			int expectedLength = baseColorSpace.ComponentsCount * (maxIndex + 1);
			int length = lookupTable.Length;
			if (length != expectedLength) {
				if (length < expectedLength)
					PdfDocumentReader.ThrowIncorrectDataException();
				Array.Resize<byte>(ref lookupTable, expectedLength);
			}
		}
		protected internal override PdfColor Transform(PdfColor color) {
			double[] components = color.Components;
			if (components.Length != 1)
				return color;
			int componentsCount = baseColorSpace.ComponentsCount;
			double[] result = new double[componentsCount];
			for (int i = 0, index = PdfMathUtils.ToInt32(components[0]) * componentsCount; i < componentsCount; i++, index++)
				result[i] = lookupTable[index] / 255.0;
			return baseColorSpace.Transform(new PdfColor(result));
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			return PdfIndexedColorSpaceTransformation.Transform(this, data, width, height, bitsPerComponent, colorKeyMask);
		}
		protected internal override PdfRange[] CreateDefaultDecodeArray(int bitsPerComponent) {
			return new PdfRange[] { new PdfRange(0, (1 << bitsPerComponent) - 1) };
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return new object[] { new PdfName(TypeName), baseColorSpace.Write(collection), maxIndex, lookupTable };
		}
	}
}
