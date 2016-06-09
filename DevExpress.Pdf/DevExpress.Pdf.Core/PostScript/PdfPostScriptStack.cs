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
	public class PdfPostScriptStack {
		readonly List<object> list = new List<object>();
		int count = 0;
		public int Count { get { return count; } }
		public void Clear() {
			count = 0;
		}
		public object PeekAtIndex(int index) {
			if (index < 0 || index >= count)
				PdfDocumentReader.ThrowIncorrectDataException();
			return list[count - index - 1];
		}
		public object Peek() {
			return PeekAtIndex(0);
		}
		public object Pop() {
			if (count <= 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return list[--count];
		}
		public void Push(object obj) {
			if (count < list.Count)
				list[count++] = obj;
			else {
				list.Add(obj);
				count++;
			}
		}
		public void Exchange() {
			if (count < 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			int firstIndex = count - 1;
			int nextIndex = firstIndex - 1;
			object value = list[firstIndex];
			list[firstIndex] = list[nextIndex];
				list[nextIndex] = value;
		}
	}
}
