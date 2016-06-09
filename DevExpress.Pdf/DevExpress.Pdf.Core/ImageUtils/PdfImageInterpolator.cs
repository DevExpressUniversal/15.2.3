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

namespace DevExpress.Pdf.Native {
	public class PdfImageInterpolator {
		public static byte[] HorizontalInterpolation(byte[] data, int sourceWidth, int sourceStride, int targetWidth, int height, int componentsCount) {
			byte[] result = new byte[targetWidth * height * componentsCount];
			PdfImageInterpolator interpolator = new PdfImageInterpolator(data, componentsCount, result, sourceWidth, targetWidth);
			if (sourceWidth == 1) {
				byte[] temp = new byte[componentsCount];
				for (int y = 0, targetPosition = 0, sourceX = 0; y < height; y++, sourceX += sourceStride) {
					interpolator.FillComponents(data, sourceX, temp);
					for (int targetX = 0; targetX < targetWidth; targetX++)
						for (int i = 0; i < componentsCount; i++)
							result[targetPosition++] = temp[i];
				}
			}
			else {
				for (int y = 0, sourceX = 0; y < height; y++, sourceX += sourceStride) {
					interpolator.Start(sourceX);
					for (int x = 0; x < targetWidth; x++)
						interpolator.Interpolate(x);
				}
			}
			return result;
		}
		public static byte[] VerticalInterpolation(byte[] data, int width, int sourceHeight, int targetHeight) {
			byte[] result = new byte[width * targetHeight];
			if (sourceHeight == 1) {
				for (int y = 0, trg = 0; y < targetHeight; y++)
					for (int x = 0; x < width; x++)
						result[trg++] = data[x];
			}
			else {
				PdfImageInterpolator interpolator = new PdfImageInterpolator(data, width, result, sourceHeight, targetHeight);
				interpolator.Start(0);
				for (int y = 0; y < targetHeight; y++)
					interpolator.Interpolate(y);
			}
			return result;
		}
		readonly byte[] data;
		readonly int size;
		readonly byte[] result;
		readonly double factor;
		readonly int lastSourcePosition;
		byte[] prev;
		byte[] next;
		int sourcePosition;
		int arrayPosition;
		int resultPosition;
		PdfImageInterpolator(byte[] data, int size, byte[] result, int sourceSize, int destinationSize) {
			this.data = data;
			this.size = size;
			this.result = result;
			lastSourcePosition = sourceSize - 1;
			factor = (double)sourceSize / destinationSize;
			prev = new byte[size];
			next = new byte[size];
		}
		void FillComponents(byte[] src, int position, byte[] components) {
			for (int i = 0; i < size; i++)
				components[i] = src[position++];
		}
		void Start(int position) {
			arrayPosition = position;
			FillComponents(data, arrayPosition, prev);
			arrayPosition += size;
			FillComponents(data, arrayPosition, next);
			arrayPosition += size;
			sourcePosition = 0;
		}
		void Interpolate(int value) {
			double ratio = value * factor - sourcePosition;
			if (ratio >= 1) {
				byte[] temp = prev;
				prev = next;
				next = temp;
				if (++sourcePosition < lastSourcePosition) {
					FillComponents(data, arrayPosition, next);
					arrayPosition += size;
				}
				else
					FillComponents(next, 0, prev);
				ratio -= 1;
			}
			for (int x = 0; x < size; x++) {
				double tmp = prev[x];
				result[resultPosition++] = PdfMathUtils.ToByte(tmp + (next[x] - tmp) * ratio);
			}
		}
	}
}
