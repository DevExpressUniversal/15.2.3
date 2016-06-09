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
	public class PdfTextAnnotation : PdfMarkupAnnotation {
		internal const string Type = "Text";
		const string isOpenedDictionaryKey = "Open";
		const string iconNameDictionaryKey = "Name";
		const string stateDictionaryKey = "State";
		const string stateModelDictionaryKey = "StateModel";
		const bool defaultOpenedState = false;
		const string defaultIconName = "Note";
		readonly bool isOpened;
		readonly string iconName;
		readonly string state;
		readonly string stateModel;
		public bool IsOpened { get { return isOpened; } }
		public string IconName { get { return iconName; } }
		public string State { get { return state; } }
		public string StateModel { get { return stateModel; } }
		protected override string AnnotationType { get { return Type; } }
		internal PdfTextAnnotation(PdfPage page, PdfReaderDictionary dictionary) : base(page, dictionary) {
			isOpened = dictionary.GetBoolean(isOpenedDictionaryKey) ?? defaultOpenedState;
			iconName = dictionary.GetName(iconNameDictionaryKey) ?? defaultIconName;
			state = dictionary.GetString(stateDictionaryKey);
			stateModel = dictionary.GetString(stateModelDictionaryKey);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection collection) {
			PdfWriterDictionary dictionary = base.CreateDictionary(collection);
			dictionary.Add(isOpenedDictionaryKey, isOpened, defaultOpenedState);
			dictionary.AddName(iconNameDictionaryKey, iconName, defaultIconName);
			dictionary.Add(stateDictionaryKey, state);
			dictionary.Add(stateModelDictionaryKey, stateModel);
			return dictionary;
		}
	}
}
