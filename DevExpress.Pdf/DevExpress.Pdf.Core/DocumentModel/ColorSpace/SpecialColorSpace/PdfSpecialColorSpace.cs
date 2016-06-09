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
	public abstract class PdfSpecialColorSpace : PdfCustomColorSpace {
		readonly PdfColorSpace alternateSpace;
		readonly PdfCustomFunction tintTransform;
		public PdfColorSpace AlternateSpace { get { return alternateSpace; } }
		public PdfCustomFunction TintTransform { get { return tintTransform; } }
		protected PdfSpecialColorSpace(PdfObjectCollection objects, IList<object> array) {
			if (!CheckArraySize(array.Count))
				PdfDocumentReader.ThrowIncorrectDataException();
			alternateSpace = PdfColorSpace.Parse(objects, array[2]);
			tintTransform = PdfCustomFunction.Parse(objects, array[3]);
			if (tintTransform == null || tintTransform.RangeSize != alternateSpace.ComponentsCount)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			if (bitsPerComponent != 8)
				return null;
			int sourceComponentsCount = ComponentsCount;
			int componentsCount = alternateSpace.ComponentsCount;
			double[] arg = new double[sourceComponentsCount];
			int length = data.Length;
			byte[] intermediateData = new byte[length * componentsCount];
			for (int i = 0, trg = 0; i < length;) {
				for (int j = 0; j < sourceComponentsCount; j++)
					arg[j] = data[i++] / 255.0;
				double[] result = tintTransform.Transform(arg);
				if (result.Length < componentsCount)
					return null;
				for (int j = 0; j < componentsCount; j++)
					intermediateData[trg++] = PdfMathUtils.ToByte(result[j] * 255);
			}
			return alternateSpace.Transform(intermediateData, width, height, 8, null);
		}
		protected internal override PdfColor Transform(PdfColor color) {
			return alternateSpace.Transform(new PdfColor(tintTransform.Transform(color.Components)));
		}
		protected abstract bool CheckArraySize(int actualSize);
	}
}
