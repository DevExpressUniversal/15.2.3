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
namespace DevExpress.Pdf.Native {
	public static class PdfNameTreeNode<T> where T : class {
		const string namesKey = "Names";
		static readonly PdfNameTreeEncoding encoding = new PdfNameTreeEncoding();
		static string ConvertToName(object key) {
			byte[] data = key as byte[];
			if (data == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return encoding.GetString(data, 0, data.Length);
		}
		static object ConvertFromName(string value) {
			return encoding.GetBytes(value);
		}
		internal static PdfDeferredSortedDictionary<string, T> Parse(PdfReaderDictionary dictionary, PdfCreateTreeElementAction<T> createElement) {
			return PdfElementTreeNode<string, T>.Parse(dictionary, k => ConvertToName(k), createElement, namesKey, true);
		}
		internal static PdfWriterDictionary Write(PdfObjectCollection objects, PdfDeferredSortedDictionary<string, T> dictionary) {
			return PdfElementTreeNode<string, T>.Write(objects, namesKey, dictionary, k => ConvertFromName(k), null);
		}
	}
}
