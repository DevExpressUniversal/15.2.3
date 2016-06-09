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
	public class PdfIndexedColorSpaceEightBitTransformation : PdfIndexedColorSpaceTransformation {
		public PdfIndexedColorSpaceEightBitTransformation(PdfIndexedColorSpace colorSpace, int width, int height, IList<PdfRange> colorKeyMask) : base(colorSpace, width, height, colorKeyMask) {
		}
		protected override void Transform(byte[] data) {
			int size = Width * Height;
			int maxIndex = MaxIndex;
			int componentsCount = ComponentsCount;
			byte[] lookupTable = LookupTable;
			byte[] result = Result;
			PdfRange transparentRange = TransparentRange;
			byte[] maskData = MaskData;
			for (int i = 0, index = 0; i < size; i++) {
				byte value = data[i];
				if (value <= maxIndex) {
					int position = value * componentsCount;
					for (int j = 0; j < componentsCount; j++)
						result[index++] = lookupTable[position++];
				}
				else 
					index += componentsCount;
				if (transparentRange != null)
					maskData[i] = (value >= transparentRange.Min && value <= transparentRange.Max) ? (byte)0 : (byte)255;
			}   
		}
	}
}
