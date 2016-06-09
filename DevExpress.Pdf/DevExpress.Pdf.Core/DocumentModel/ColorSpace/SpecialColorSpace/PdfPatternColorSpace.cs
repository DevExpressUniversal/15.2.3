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
	public class PdfPatternColorSpace : PdfCustomColorSpace {
		internal const string Name = "Pattern";
		readonly PdfColorSpace alternateColorSpace;
		public PdfColorSpace AlternateColorSpace { get { return alternateColorSpace; } }
		public override int ComponentsCount { get { return alternateColorSpace.ComponentsCount; } }
		internal PdfPatternColorSpace(PdfColorSpace alternateColorSpace) {
			this.alternateColorSpace = alternateColorSpace;
		}
		internal PdfPatternColorSpace() : this(new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.RGB)) {
		}
		protected internal override PdfColor Transform(PdfColor color) {
			return alternateColorSpace.Transform(color);
		}
		protected internal override PdfColorSpaceTransformResult Transform(byte[] data, int width, int height, int bitsPerComponent, IList<PdfRange> colorKeyMask) {
			return alternateColorSpace.Transform(data, width, height, bitsPerComponent, colorKeyMask);
		}
		protected internal override object Write(PdfObjectCollection collection) {
			PdfDeviceColorSpace deviceColorSpace = alternateColorSpace as PdfDeviceColorSpace;
			return (deviceColorSpace != null && deviceColorSpace.Kind == PdfDeviceColorSpaceKind.RGB) ? (object)new PdfName(Name) : base.Write(collection);
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			PdfName name = new PdfName(Name);
			return (object)new object[] { name, alternateColorSpace.Write(collection) };
		}
	}
}
