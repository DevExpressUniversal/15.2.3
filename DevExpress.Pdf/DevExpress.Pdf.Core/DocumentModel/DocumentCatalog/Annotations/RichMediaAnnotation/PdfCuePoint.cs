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
	public enum PdfCuePointKind { Navigation, Event }
	public class PdfCuePoint {
		const string cuePointDictionaryName = "CuePoint";
		const string timeKey = "Time";
		const string nameKey = "Name";
		readonly double time;
		readonly PdfCuePointKind kind;
		readonly string name;
		readonly PdfAction action;
		public double Time { get { return time; } }
		public PdfCuePointKind Kind { get { return kind; } }
		public string Name { get { return name; } }
		public PdfAction Action { get { return action; } }
		internal PdfCuePoint(PdfReaderDictionary dictionary) {
			string type = dictionary.GetName(PdfDictionary.DictionaryTypeKey);
			double? timeValue = dictionary.GetNumber(timeKey);
			if ((type != null && type != cuePointDictionaryName) || !timeValue.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			time = timeValue.Value;
			string subtype = dictionary.GetName(PdfDictionary.DictionarySubtypeKey);
			name = dictionary.GetString(nameKey);
			if (name == null) {
				action = dictionary.GetAction(PdfDictionary.DictionaryActionKey);
				if (action == null || (subtype != null && subtype != "Event"))
					PdfDocumentReader.ThrowIncorrectDataException();
				kind = PdfCuePointKind.Event;
			}
			else {
				if (String.IsNullOrEmpty(name) || (subtype != null && subtype != "Navigation") || dictionary.ContainsKey(PdfDictionary.DictionaryActionKey))
					PdfDocumentReader.ThrowIncorrectDataException();
				kind = PdfCuePointKind.Navigation;
			}
		}
		internal PdfWriterDictionary Write(PdfObjectCollection collection) {
			PdfWriterDictionary result = new PdfWriterDictionary(collection);
			result.AddName(PdfDictionary.DictionaryTypeKey, cuePointDictionaryName);
			result.Add(timeKey, time);
			result.AddName(PdfDictionary.DictionarySubtypeKey, CuePointKindToString());
			if (!String.IsNullOrEmpty(name)) {
				result.Add(nameKey, name);
			}
			else {
				result.Add("A", action);
			}
			return result;
		}
		string CuePointKindToString() {
			if (kind == PdfCuePointKind.Event)
				return "Event";
			else return "Navigation";
		}
	}
}
