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
namespace DevExpress.Pdf {
	public class PdfCIDSystemInfo : PdfObject {
		internal const string RegistryKey = "Registry";
		internal const string OrderingKey = "Ordering";
		internal const string SupplementKey = "Supplement";
		readonly string registry;
		readonly string ordering;
		readonly int supplement;
		public string Registry { get { return registry; } }
		public string Ordering { get { return ordering; } }
		public int Supplement { get { return supplement; } }
		internal PdfCIDSystemInfo(PdfReaderDictionary dictionary) : base(dictionary.Number) {
			registry = dictionary.GetString(RegistryKey);
			ordering = dictionary.GetString(OrderingKey);
			int? supplementValue = dictionary.GetInteger(SupplementKey);
			if (registry == null || ordering == null || !supplementValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			supplement = supplementValue.Value;
		}
		internal PdfCIDSystemInfo(string registry, string ordering, int supplement)
			: base(PdfObject.DirectObjectNumber) {
			this.registry = registry;
			this.ordering = ordering;
			this.supplement = supplement;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddASCIIString(RegistryKey, registry);
			dictionary.AddASCIIString(OrderingKey, ordering);
			dictionary.Add(SupplementKey, supplement);
			return dictionary;
		}
	}
}
