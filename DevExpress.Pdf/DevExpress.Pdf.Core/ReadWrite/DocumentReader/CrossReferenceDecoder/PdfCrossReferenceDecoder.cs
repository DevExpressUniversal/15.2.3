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
	public class PdfCrossReferenceDecoder {
		public static void Decode(byte[] data, IEnumerable<PdfIndexDescription> indices, PdfObjectCollection objects, int typeWeight, int offsetWeight, int generationWeight) {
			new PdfCrossReferenceDecoder(data).Decode(indices, objects, typeWeight, offsetWeight, generationWeight);
		}
		readonly byte[] data;
		readonly int length;
		int position;
		PdfCrossReferenceDecoder(byte[] data) {
			this.data = data;
			length = data.Length;
		}
		int ReadValue(int weight) {
			if (weight == 0)
				return 0;
			if (position + weight > length)
				PdfDocumentReader.ThrowIncorrectDataException();
			int value = data[position++];
			for (int i = 1; i < weight; i++)
				value = (value << 8) + data[position++];
			return value;
		}
		void Decode(IEnumerable<PdfIndexDescription> indices, PdfObjectCollection objects, int typeWeight, int offsetWeight, int generationWeight) {
			foreach (PdfIndexDescription description in indices) {
				int number = description.StartValue;
				int count = description.Count;
				for (int i = 0; i < count; i++) {
					int type = typeWeight == 0 ? 1 : ReadValue(typeWeight);
					int value1 = ReadValue(offsetWeight);
					int value2 = ReadValue(generationWeight);
					switch (type) {
						case 0:
							objects.AddFreeObject(number, value2);
							break;
						case 1:
							objects.AddItem(new PdfObjectSlot(number, value2, value1), false);
							break;
						case 2:
							objects.AddItem(new PdfObjectStreamElement(number, value1, value2), false);
							break;
						default:
							PdfDocumentReader.ThrowIncorrectDataException();
							break;
					}
					number++;
				}
			}
		}
	}
}
