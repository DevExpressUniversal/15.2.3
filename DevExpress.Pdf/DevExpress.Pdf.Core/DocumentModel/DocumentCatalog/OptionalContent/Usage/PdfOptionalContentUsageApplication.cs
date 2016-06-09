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
	public enum PdfOptionalContentUsageApplicationEvent { View, Print, Export }
	public class PdfOptionalContentUsageApplication {
		const string eventDictionaryKey = "Event";
		const string groupsDictionaryKey = "OCGs";
		const string categoryDictionaryKey = "Category";
		readonly PdfOptionalContentUsageApplicationEvent usageEvent;
		readonly IList<PdfOptionalContentGroup> groups;
		readonly IList<string> category;
		public PdfOptionalContentUsageApplicationEvent Event { get { return usageEvent; } }
		public IList<PdfOptionalContentGroup> Groups { get { return groups; } }
		public IList<string> Category { get { return category; } }
		internal PdfOptionalContentUsageApplication(PdfReaderDictionary dictionary) {
			usageEvent = dictionary.GetEnumName<PdfOptionalContentUsageApplicationEvent>(eventDictionaryKey);
			groups = dictionary.GetArray<PdfOptionalContentGroup>(groupsDictionaryKey, oc => dictionary.Objects.GetOptionalContentGroup(oc));
			category = dictionary.GetArray<string>(categoryDictionaryKey, o => ReadCategory(dictionary.Objects, o));
		}
		string ReadCategory(PdfObjectCollection objects, object o) {
			PdfName name = o as PdfName;
			if (name == null) {
				PdfObjectReference reference = o as PdfObjectReference;
				if (reference == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				object resolved = objects.GetObjectData(reference.Number);
				name = resolved as PdfName;
				if (name == null)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			return name.Name;
		}
		internal object Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.AddEnumName(eventDictionaryKey, usageEvent);
			dictionary.AddList(groupsDictionaryKey, groups);
			dictionary.AddList(categoryDictionaryKey, category, o => new PdfName((string)o));
			return dictionary;
		}
	}
}
