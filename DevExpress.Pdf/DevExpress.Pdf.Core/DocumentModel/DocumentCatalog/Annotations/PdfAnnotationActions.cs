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
	public class PdfAnnotationActions {
		internal const string CursorEnteredDictionaryKey = "E";
		internal const string CursorExitedDictionaryKey = "X";
		internal const string MouseButtonPressedDictionaryKey = "D";
		internal const string MouseButtonReleasedDictionaryKey = "U";
		internal const string InputFocusReceivedDictionaryKey = "Fo";
		internal const string InputFocusLostDictionaryKey = "Bl";
		internal const string PageOpenedDictionaryKey = "PO";
		internal const string PageClosedDictionaryKey = "PC";
		internal const string PageBecameVisibleDictionaryKey = "PV";
		internal const string PageBecameInvisibleDictionaryKey = "PI";
		readonly PdfAction cursorEntered;
		readonly PdfAction cursorExited;
		readonly PdfAction mouseButtonPressed;
		readonly PdfAction mouseButtonReleased;
		readonly PdfAction inputFocusReceived;
		readonly PdfAction inputFocusLost;
		readonly PdfAction pageOpened;
		readonly PdfAction pageClosed;
		readonly PdfAction pageBecameVisible;
		readonly PdfAction pageBecameInvisible;
		public PdfAction CursorEntered { get { return cursorEntered; } }
		public PdfAction CursorExited { get { return cursorExited; } }
		public PdfAction MouseButtonPressed { get { return mouseButtonPressed; } }
		public PdfAction MouseButtonReleased { get { return mouseButtonReleased; } }
		public PdfAction InputFocusReceived { get { return inputFocusReceived; } }
		public PdfAction InputFocusLost { get { return inputFocusLost; } }
		public PdfAction PageOpened { get { return pageOpened; } }
		public PdfAction PageClosed { get { return pageClosed; } }
		public PdfAction PageBecameVisible { get { return pageBecameVisible; } }
		public PdfAction PageBecameInvisible { get { return pageBecameInvisible; } }
		internal PdfAnnotationActions(PdfAnnotation parent, PdfReaderDictionary dictionary) {
			cursorEntered = dictionary.GetAction(CursorEnteredDictionaryKey);
			cursorExited = dictionary.GetAction(CursorExitedDictionaryKey);
			mouseButtonPressed = dictionary.GetAction(MouseButtonPressedDictionaryKey);
			mouseButtonReleased = dictionary.GetAction(MouseButtonReleasedDictionaryKey);
			inputFocusReceived = dictionary.GetAction(InputFocusReceivedDictionaryKey);
			inputFocusLost = dictionary.GetAction(InputFocusLostDictionaryKey);
			pageOpened = dictionary.GetAction(PageOpenedDictionaryKey);
			pageClosed = dictionary.GetAction(PageClosedDictionaryKey);
			pageBecameVisible = dictionary.GetAction(PageBecameVisibleDictionaryKey);
			pageBecameInvisible = dictionary.GetAction(PageBecameInvisibleDictionaryKey);
			if (!(parent is PdfWidgetAnnotation) && (inputFocusReceived != null || inputFocusLost != null))
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		internal PdfWriterDictionary FillDictionary(PdfWriterDictionary dictionary) {
			dictionary.Add(CursorEnteredDictionaryKey, cursorEntered);
			dictionary.Add(CursorExitedDictionaryKey, cursorExited);
			dictionary.Add(MouseButtonPressedDictionaryKey, mouseButtonPressed);
			dictionary.Add(MouseButtonReleasedDictionaryKey, mouseButtonReleased);
			dictionary.Add(InputFocusReceivedDictionaryKey, inputFocusReceived);
			dictionary.Add(InputFocusLostDictionaryKey, inputFocusLost);
			dictionary.Add(PageOpenedDictionaryKey, pageOpened);
			dictionary.Add(PageClosedDictionaryKey, pageClosed);
			dictionary.Add(PageBecameVisibleDictionaryKey, pageBecameVisible);
			dictionary.Add(PageBecameInvisibleDictionaryKey, pageBecameInvisible);
			return dictionary;
		}
	}
}
