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
	public class PdfDeviceNColorSpace : PdfSpecialColorSpace {
		internal const string TypeName = "DeviceN";
		const string noneName = "None";
		readonly string[] names;
		public string[] Names { get { return names; } }
		public override int ComponentsCount { get { return names.Length; } }
		internal PdfDeviceNColorSpace(PdfObjectCollection collection, IList<object> array) : base(collection, array) {
			PdfColorSpace alternateSpace = AlternateSpace;
			if (!(alternateSpace is PdfDeviceColorSpace) && !(alternateSpace is PdfCIEBasedColorSpace) && !(alternateSpace is PdfICCBasedColorSpace))
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> namesArray = array[1] as IList<object>;
			if (namesArray == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			int namesCount = namesArray.Count;
			names = new string[namesCount];
			for (int i = 0; i < namesCount; i++) {
				PdfName name = namesArray[i] as PdfName;
				if (name == null)
					PdfDocumentReader.ThrowIncorrectDataException();
				string componentName = name.Name;
				if (String.IsNullOrEmpty(componentName))
					PdfDocumentReader.ThrowIncorrectDataException();
				if (componentName != noneName && i > 0)
					for (int j = 0; j < i; j++)
						if (componentName == names[j])
							PdfDocumentReader.ThrowIncorrectDataException();
				names[i] = componentName;
			}
		}
		protected internal override object ToWritableObject(PdfObjectCollection collection) {
			return CreateListToWrite(collection);
		}
		protected override bool CheckArraySize(int actualSize) {
			return actualSize == 4 || actualSize == 5;
		}
		protected virtual IList<object> CreateListToWrite(PdfObjectCollection collection) {
			List<object> namesToWrite = new List<object>(names.Length);
			foreach (string name in names)
				namesToWrite.Add(new PdfName(name));
			return new object[] { new PdfName(TypeName), namesToWrite, AlternateSpace.Write(collection), TintTransform.Write(collection) };
		}
	}
}
