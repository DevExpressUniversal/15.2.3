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
	public class PdfOperands {
		readonly Queue<object> queue = new Queue<object>();
		public int Count { get { return queue.Count; } }
		public void Add(object obj) {
			queue.Enqueue(obj);
		}
		public string TryPopLastName() {
			int lastIndex = queue.Count - 1;
			if (lastIndex < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			List<object> objects = new List<object>(queue);
			PdfName lastParameter = objects[lastIndex] as PdfName;
			if (lastParameter != null) {
				objects.RemoveAt(lastIndex);
				queue.Clear();
				foreach (object value in objects)
					queue.Enqueue(value);
				if (String.IsNullOrEmpty(lastParameter.Name))
					PdfDocumentReader.ThrowIncorrectDataException();
				return lastParameter.Name;
			}
			return null;
		}
		public object PopObject() {
			if (queue.Count == 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return queue.Dequeue();
		}
		public string PopName() {
			PdfName name = PopObject() as PdfName;
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return name.Name;
		}
		public double PopDouble() {
			return PdfDocumentReader.ConvertToDouble(PopObject());
		}
		public int PopInt() {
			object value = PopObject();
			if (!(value is int))
				PdfDocumentReader.ThrowIncorrectDataException();
			return (int)value;
		}
		public void VerifyCount() {
			if (queue.Count != 0)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
	}
}
