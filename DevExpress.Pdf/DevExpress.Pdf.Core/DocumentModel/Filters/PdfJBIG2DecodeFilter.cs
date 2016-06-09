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

using DevExpress.Pdf.Native;
using System.Collections.Generic;
namespace DevExpress.Pdf {
	 public class PdfJBIG2DecodeFilter : PdfFilter {
		const string globalSegmentsDictionaryKey = "JBIG2Globals";
		internal const string Name = "JBIG2Decode";
		readonly PdfJBIG2GlobalSegments globalSegments;
		public PdfJBIG2GlobalSegments GlobalSegments { get { return globalSegments; } }
		protected internal override string FilterName { get { return Name; } }
		internal PdfJBIG2DecodeFilter(PdfReaderDictionary parameters) {
			if (parameters != null) {
				object value= parameters.GetObjectReference(globalSegmentsDictionaryKey);
				if (value != null)
					globalSegments = PdfJBIG2GlobalSegments.Parse(parameters.Objects, value);
			}
		}
		protected internal override PdfDictionary Write(PdfObjectCollection objects) {
			if (globalSegments == null)
				return null;
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(globalSegmentsDictionaryKey, globalSegments);
			return dictionary;
		}
		protected internal override byte[] Decode(byte[] data) {
			Dictionary<int, JBIG2SegmentHeader> globalData = globalSegments == null ? null : globalSegments.Segments;
			return JBIG2Image.Decode(data, globalData);
		}
	}
}
