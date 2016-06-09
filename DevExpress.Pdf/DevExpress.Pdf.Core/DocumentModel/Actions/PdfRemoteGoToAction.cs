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
	public class PdfRemoteGoToAction : PdfJumpAction {
		internal const string Name = "GoToR";
		const string fileDictionaryKey = "F";
		const string newWindowKey = "NewWindow";
		readonly PdfFileSpecification fileSpecification;
		readonly bool openInNewWindow;
		protected override string ActionType { get { return Name; } }
		protected override bool IsInternal { get { return false; } }
		public PdfFileSpecification FileSpecification { get { return fileSpecification; } }
		public bool OpenInNewWindow { get { return openInNewWindow; } }
		internal PdfRemoteGoToAction(PdfReaderDictionary dictionary)
			: base(dictionary) {
			fileSpecification = PdfFileSpecification.Parse(dictionary, fileDictionaryKey, true);
			openInNewWindow = dictionary.GetBoolean(newWindowKey) ?? false;
		}
		protected internal override void Execute(IPdfInteractiveOperationController interactiveOperationController, IList<PdfPage> pages) {
			PdfDestination destination = Destination;
			PdfTarget target =  destination != null ? destination.CreateTarget(pages) : null;
			interactiveOperationController.OpenReferencedDocument(fileSpecification.FileName, target, openInNewWindow);
		}
		protected override PdfWriterDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = base.CreateDictionary(objects);
			dictionary.Add(fileDictionaryKey, objects.AddObject(fileSpecification));
			dictionary.Add(newWindowKey, openInNewWindow, false);
			return dictionary;
		}
	}
}
