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
namespace DevExpress.Pdf.Native {
	public abstract class PdfIndexedColorSpaceTransformation {
		public static PdfColorSpaceTransformResult Transform(PdfIndexedColorSpace colorSpace, byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			PdfIndexedColorSpaceTransformation transformation;
			switch (bitsPerComponent) {
				case 1:
				case 2:
					transformation = new PdfIndexedColorSpaceOneTwoBitTransformation(colorSpace, width, height, colorKeyMask, bitsPerComponent);
					break;
				case 4:
					transformation = new PdfIndexedColorSpaceFourBitTransformation(colorSpace, width, height, colorKeyMask);
					break;
				case 8:
					transformation = new PdfIndexedColorSpaceEightBitTransformation(colorSpace, width, height, colorKeyMask);
					break;
				default:
					return null;
			}
			transformation.Transform(data);
			PdfColorSpaceTransformResult result = colorSpace.BaseColorSpace.Transform(transformation.result, width, height, 8, null);
			return result == null ? null : new PdfColorSpaceTransformResult(result, transformation.maskData);
		}
		readonly int width;
		readonly int height;
		readonly int maxIndex;
		readonly byte[] lookupTable;
		readonly int componentsCount;
		readonly byte[] result;
		readonly PdfRange transparentRange;
		readonly byte[] maskData;
		protected int Width { get { return width; } }
		protected int Height { get { return height; } }
		protected int MaxIndex { get { return maxIndex; } }
		protected byte[] LookupTable { get { return lookupTable; } }
		protected int ComponentsCount { get { return componentsCount; } }
		protected byte[] Result { get { return result; } }
		protected PdfRange TransparentRange { get { return transparentRange; } }
		protected byte[] MaskData { get { return maskData; } }
		protected PdfIndexedColorSpaceTransformation(PdfIndexedColorSpace colorSpace, int width, int height, IList<PdfRange> colorKeyMask) {
			this.width = width;
			this.height = height;
			int dataSize = width * height;
			maxIndex = colorSpace.MaxIndex;
			lookupTable = colorSpace.LookupTable;
			componentsCount = colorSpace.BaseColorSpace.ComponentsCount;
			result = new byte[dataSize * componentsCount];
			if (colorKeyMask != null && colorKeyMask.Count == 1) {
				transparentRange = colorKeyMask[0];
				maskData = new byte[dataSize];
			}
		}
		protected abstract void Transform(byte[] data);
	}
}
