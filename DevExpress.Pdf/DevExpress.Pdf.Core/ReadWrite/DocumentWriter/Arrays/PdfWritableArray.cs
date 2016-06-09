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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public abstract class PdfWritableArray<T> : IPdfWritableObject, IEnumerable {
		readonly IEnumerable<T> value;
		protected PdfWritableArray(IEnumerable<T> value) {
			this.value = value;
		}
		public void Write(PdfDocumentWritableStream stream, int number) {
			stream.WriteOpenBracket();
			using (IEnumerator<T> enumerator = value.GetEnumerator()) {
				if (enumerator.MoveNext())
					WriteItem(stream, enumerator.Current, number);
				while (enumerator.MoveNext()) {
					stream.WriteSpace();
					WriteItem(stream, enumerator.Current, number);
				}
			}
			stream.WriteCloseBracket();
		}
		public IEnumerator GetEnumerator() {
			return value.GetEnumerator();
		}
		protected abstract void WriteItem(PdfDocumentWritableStream documentStream, T value, int number);
	}
	public class PdfWritableArray : PdfWritableArray<object>, IEnumerable<object> {
		static IEnumerable<object> GetEnumerable(IEnumerable value) {
			foreach (object v in value)
				yield return v;
		}
		public PdfWritableArray(IEnumerable<object> enumerable) : base(enumerable) {
		}
		public PdfWritableArray(IEnumerable value) : base(GetEnumerable(value)) {
		}
		protected override void WriteItem(PdfDocumentWritableStream documentStream, object value, int number) {
			documentStream.WriteObject(value, number);
		}
		IEnumerator<object> IEnumerable<object>.GetEnumerator() {
			IEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}
	}
}
