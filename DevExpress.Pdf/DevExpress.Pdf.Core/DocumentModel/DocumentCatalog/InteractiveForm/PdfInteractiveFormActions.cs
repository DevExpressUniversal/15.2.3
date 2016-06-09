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
	public class PdfInteractiveFormFieldActions {
		internal const string CharacterChangedDictionaryKey = "K";
		internal const string FieldFormattingDictionaryKey = "F";
		internal const string FieldValueChangedDictionaryKey = "V";
		internal const string FieldValueRecalculatingDictionaryKey = "C";
		readonly PdfJavaScriptAction characterChanged;
		readonly PdfJavaScriptAction fieldFormatting;
		readonly PdfJavaScriptAction fieldValueChanged;
		readonly PdfJavaScriptAction fieldValueRecalculating;
		public PdfJavaScriptAction CharacterChanged { get { return characterChanged; } }
		public PdfJavaScriptAction FieldFormatting { get { return fieldFormatting; } }
		public PdfJavaScriptAction FiledValueChanged { get { return fieldValueChanged; } }
		public PdfJavaScriptAction FieldValueRecalculating { get { return fieldValueRecalculating; } }
		internal PdfInteractiveFormFieldActions(PdfReaderDictionary dictionary) {
			characterChanged = dictionary.GetJavaScriptAction(CharacterChangedDictionaryKey);
			fieldFormatting = dictionary.GetJavaScriptAction(FieldFormattingDictionaryKey);
			fieldValueChanged = dictionary.GetJavaScriptAction(FieldValueChangedDictionaryKey);
			fieldValueRecalculating = dictionary.GetJavaScriptAction(FieldValueRecalculatingDictionaryKey);
		}
		internal PdfWriterDictionary FillDictionary(PdfWriterDictionary dictionary) {
			dictionary.Add(CharacterChangedDictionaryKey, characterChanged);
			dictionary.Add(FieldFormattingDictionaryKey, fieldFormatting);
			dictionary.Add(FieldValueChangedDictionaryKey, fieldValueChanged);
			dictionary.Add(FieldValueRecalculatingDictionaryKey, fieldValueRecalculating);
			return dictionary;
		}
	}
}
