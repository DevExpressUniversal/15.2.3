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
namespace DevExpress.Pdf.Native {
	public class PdfDictionary : Dictionary<string, object>, IPdfWritableObject {
		public const string DictionaryTypeKey = "Type";
		public const string DictionarySubtypeKey = "Subtype";
		public const string DictionaryLanguageKey = "Lang";
		public const string DictionaryJustificationKey = "Q";
		public const string DictionaryAppearanceKey = "DA";
		public const string DictionaryActionKey = "A";
		public const string DictionaryAnnotationHighlightingModeKey = "H";
		public const string DictionaryPaddingKey = "RD";
		void IPdfWritableObject.Write(PdfDocumentWritableStream stream, int number) {
		   stream.WriteOpenDictionary();
			foreach (KeyValuePair<string, object> pair in this) {
				if (pair.Value is PdfStream)
					throw new InvalidOperationException();
				((IPdfWritableObject)new PdfName(pair.Key)).Write(stream, number);
				stream.WriteSpace();
				stream.WriteObject(pair.Value, number);
				stream.WriteSpace();
			}
			stream.WriteCloseDictionary();
		}
	}
}
