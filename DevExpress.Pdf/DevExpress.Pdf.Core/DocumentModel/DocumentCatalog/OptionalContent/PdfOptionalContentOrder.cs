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
	public class PdfOptionalContentOrder : PdfOptionalContent {
		readonly string name;
		readonly IList<PdfOptionalContent> items = new List<PdfOptionalContent>();
		public string Name { get { return name; } }
		public IList<PdfOptionalContent> Items { get { return items; } }
		internal PdfOptionalContentOrder(PdfObjectCollection objects, IList<object> list) : base (PdfObject.DirectObjectNumber){
			int count = list.Count;
			for (int i = 0; i < count; i++) {
				object listItem = list[i];
				if (listItem == null)
					items.Add(null);
				else {
					object item = objects.TryResolve(listItem);
					byte[] b = item as byte[];
					if (b == null) {
						IList<object> itemList = item as IList<object>;
						if (itemList == null) {
							PdfReaderDictionary dictionary = item as PdfReaderDictionary;
							if (dictionary == null)
								PdfDocumentReader.ThrowIncorrectDataException();
							items.Add(objects.GetOptionalContentGroup(item));
						}
						else
							items.Add(new PdfOptionalContentOrder(objects, itemList));
					}
					else {
						if (i != 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						name = PdfDocumentReader.ConvertToString(b);
					}
				}
			}
		}
		protected internal override object Write(PdfObjectCollection objects) {
			IList<object> result = new List<object>();
			if (!String.IsNullOrEmpty(name))
				result.Add(name);
			foreach (PdfOptionalContent item in items)
				result.Add(objects.AddObject(item));
			return result;
		}
	}
}
