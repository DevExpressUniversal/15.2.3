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
	public class PdfNamedAction : PdfAction {
		internal const string Name = "Named";
		const string actionNameDictionaryKey = "N";
		readonly string actionName;
		protected override string ActionType { get { return Name; } }
		public string ActionName { get { return actionName; } }
		internal PdfNamedAction(PdfReaderDictionary dictionary) : base(dictionary) {
			actionName = dictionary.GetName(actionNameDictionaryKey);
			if (String.IsNullOrEmpty(actionName))
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		protected internal override void Execute(IPdfInteractiveOperationController interactiveOperationController, IList<PdfPage> pages) {
			switch (actionName) {
				case "NextPage":
					interactiveOperationController.GoToNextPage();
					break;
				case "PrevPage":
					interactiveOperationController.GoToPreviousPage();
					break;
				case "FirstPage":
					interactiveOperationController.GoToFirstPage();
					break;
				case "LastPage":
					interactiveOperationController.GoToLastPage();
					break;
				case "Print":
					interactiveOperationController.Print();
					break;
			}
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.AddName(actionNameDictionaryKey, actionName);
			return dictionary;
		}
	}
}
