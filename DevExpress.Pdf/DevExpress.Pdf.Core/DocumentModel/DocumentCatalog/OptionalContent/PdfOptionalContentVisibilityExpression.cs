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
	public enum PdfOptionalContentVisibilityExpressionOperator { And, Or, Not }
	public class PdfOptionalContentVisibilityExpression : PdfOptionalContent {
		readonly PdfOptionalContentVisibilityExpressionOperator operation;
		readonly List<PdfOptionalContent> operands;
		public PdfOptionalContentVisibilityExpressionOperator Operation { get { return operation; } }
		public IList<PdfOptionalContent> Operands { get { return operands; } }
		internal PdfOptionalContentVisibilityExpression(PdfObjectCollection objects, IList<object> array) : base(PdfObject.DirectObjectNumber) {
			int count = array.Count;
			if (count < 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfName name = array[0] as PdfName;
			if (name == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			operation = PdfEnumToStringConverter.Parse<PdfOptionalContentVisibilityExpressionOperator>(name.Name);
			if (operation == PdfOptionalContentVisibilityExpressionOperator.Not && count != 2)
				PdfDocumentReader.ThrowIncorrectDataException();
			operands = new List<PdfOptionalContent>(count - 1);
			for (int i = 1; i < count; i++) {
				object value = objects.TryResolve(array[i]);
				PdfReaderDictionary dictionary = value as PdfReaderDictionary;
				if (dictionary == null) {
					IList<object> list = value as IList<object>;
					if (list == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					operands.Add(new PdfOptionalContentVisibilityExpression(objects, list));
				}
				else {
					PdfOptionalContentGroup operand = PdfOptionalContent.Create(dictionary) as PdfOptionalContentGroup;
					if (operand == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					operands.Add(operand);
				}
			}
		}
		protected internal override object Write(PdfObjectCollection objects) {
			int count = operands.Count;
			object[] array = new object[count + 1];
			array[0] = PdfEnumToStringConverter.Convert(operation, false);
			for (int i = 1, index = 0; i <= count; i++)
				array[i] = operands[index++];
			return array;
		}
	}
}
