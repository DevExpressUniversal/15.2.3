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
	public class PdfOpacityInterpolator {
		public static byte[] HorizontalInterpolation(byte[] data, int sourceWidth, int sourceStride, int targetWidth, int height, int componentsCount) {
			byte[] result = new byte[targetWidth * height * componentsCount];
			PdfOpacityInterpolator interpolator = new PdfOpacityInterpolator(data, componentsCount, result, sourceWidth, targetWidth);
			for (int y = 0, sourceX = 0; y < height; y++, sourceX += sourceStride) {
				interpolator.Start(sourceX);
				for (int x = 1; x < sourceWidth; x++)
					interpolator.Interpolate(x);
				interpolator.End();
			}
			return result;
		}
		public static byte[] VerticalInterpolation(byte[] data, int width, int sourceHeight, int targetHeight) {
			byte[] result = new byte[width * targetHeight];
			PdfOpacityInterpolator interpolator = new PdfOpacityInterpolator(data, width, result, sourceHeight, targetHeight);
			interpolator.Start(0);
			for (int y = 1; y < sourceHeight; y++)
				interpolator.Interpolate(y);
			interpolator.End();
			return result;
		}
		readonly byte[] data;
		readonly int size;
		readonly byte[] result;
		readonly double factor;
		readonly int destinationSize;
		byte[] prev;
		int prevResultPosition;
		byte[] next;
		int sourcePosition;
		int resultPosition;
		PdfOpacityInterpolator(byte[] data, int size, byte[] result, int sourceSize, int destinationSize) {
			this.data = data;
			this.size = size;
			this.result = result;
			this.destinationSize = destinationSize;
			factor = (double)destinationSize / sourceSize;
			prev = new byte[size];
			next = new byte[size];
		}
		byte[] NextInterpolationArray() {
			byte[] row = new byte[size];
			for (int i = 0; i < size; i++)
				row[i] = data[sourcePosition++];
			return row;
		}
		void FillResultData(byte[] array) {
			for (int i = 0; i < size; i++)
				result[resultPosition++] = array[i];
		}
		void Start(int position) {
			sourcePosition = position;
			prev = NextInterpolationArray();
			prevResultPosition = 0;
		}
		void Interpolate(int value) {
			next = NextInterpolationArray();
			double nextResultPosition = value * factor;
			int position = (int)nextResultPosition;
			for (; prevResultPosition < position; prevResultPosition++)
				FillResultData(prev);
			double fraction = nextResultPosition - position;
			if (fraction == 0) 
				prevResultPosition = position;
			else {
				byte[] current = new byte[size];
				for (int i = 0; i < size; i++)
					current[i] = PdfMathUtils.ToByte(prev[i] + (next[i] - prev[i]) * fraction);
				FillResultData(current);
				prevResultPosition = ++position;
			}			  
			prev = next;  
		}
		void End() {
			for (; prevResultPosition < destinationSize; prevResultPosition++)
				FillResultData(prev); 
		}
	}
}
