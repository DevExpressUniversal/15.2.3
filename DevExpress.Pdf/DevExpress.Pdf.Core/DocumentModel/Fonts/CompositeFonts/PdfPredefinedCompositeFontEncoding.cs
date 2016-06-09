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
	public class PdfPredefinedCompositeFontEncoding : PdfCompositeFontEncoding {
		static readonly List<string> horizontalEncodingNames = new List<string>() { "GB-EUC-H", "GBpc-EUC-H", "GBK-EUC-H", "GBKp-EUC-H", "GBK2K-H", "UniGB-UCS2-H", "UniGB-UTF16-H", "B5pc-H", 
																					"HKscs-B5-H", "ETen-B5-H", "ETenms-B5-H", "CNS-EUC-H", "UniCNS-UCS2-H", "UniCNS-UTF16-H", "83pv-RKSJ-H", "90ms-RKSJ-H",
																					"90msp-RKSJ-H", "90pv-RKSJ-H", "Add-RKSJ-H", "EUC-H", "Ext-RKSJ-H", "H", "UniJIS-UCS2-H", "UniJIS-UCS2-HW-H", 
																					"UniJIS-UTF16-H", "KSC-EUC-H", "KSCms-UHC-H", "KSCms-UHC-HW-H", "KSCpc-EUC-H", "UniKS-UCS2-H", "UniKS-UTF16-H" };
		static readonly List<string> verticalEncodingNames = new List<string>() { "GB-EUC-V", "GBpc-EUC-V", "GBK-EUC-V", "GBKp-EUC-V", "GBK2K-V", "UniGB-UCS2-V", "UniGB-UTF16-V", "B5pc-V", 
																				  "HKscs-B5-V", "ETen-B5-V", "ETenms-B5-V", "CNS-EUC-V", "UniCNS-UCS2-V", "UniCNS-UTF16-V", "90ms-RKSJ-V", 
																				  "90msp-RKSJ-V", "Add-RKSJ-V", "EUC-V", "Ext-RKSJ-V", "V", "UniJIS-UCS2-V", "UniJIS-UCS2-HW-V", 
																				  "UniJIS-UTF16-V", "KSC-EUC-V", "KSCms-UHC-V", "KSCms-UHC-HW-V", "UniKS-UCS2-V", "UniKS-UTF16-V" };
		readonly string name;
		readonly bool isVertical;
		public string Name { get { return name; } }
		public override bool IsVertical { get { return isVertical; } }
		internal PdfPredefinedCompositeFontEncoding(string name) {
			this.name = name;
			if (!horizontalEncodingNames.Contains(name))
				if (verticalEncodingNames.Contains(name))
					isVertical = true;
				else
					PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override PdfStringData GetStringData(byte[] bytes, double[] glyphOffsets) {
			int length = bytes.Length / 2;
			double[] offsets = new double[length + 1];
			if (glyphOffsets != null)
				for (int i = 0, byteIndex = 0; i < length; i++)
					offsets[i] = glyphOffsets[byteIndex];
			return new PdfStringData(new byte[length][], new short[length], offsets);
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return new PdfName(name);
		}
	}
}
