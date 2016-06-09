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
	public abstract class PdfShading : PdfObject {
		const string shadingTypeDictionaryKey = "ShadingType";
		const string colorSpaceDictionaryKey = "ColorSpace";
		const string backgroundDictionaryKey = "Background";
		const string boundingBoxDictionaryKey = "BBox";
		const string antiAliasDictionaryKey = "AntiAlias";
		const string functionDictionaryKey = "Function";
		static void CheckStreamPresence(PdfReaderStream stream) {
			if (stream == null)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		static void CheckStreamAbsence(PdfReaderStream stream) {
			if (stream != null)
				PdfDocumentReader.ThrowIncorrectDataException();
		}
		internal static PdfShading Parse(PdfReaderStream stream, PdfReaderDictionary dictionary) {
			int? shadingType = dictionary.GetInteger(shadingTypeDictionaryKey);
			if (!shadingType.HasValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			switch (shadingType.Value) {
				case PdfFunctionBasedShading.Type:
					CheckStreamAbsence(stream);
					return new PdfFunctionBasedShading(dictionary);
				case PdfAxialShading.Type:
					CheckStreamAbsence(stream);
					return new PdfAxialShading(dictionary);
				case PdfRadialShading.Type:
					CheckStreamAbsence(stream);
					return new PdfRadialShading(dictionary);
				case PdfFreeFormGourandShadedTriangleMesh.Type:
					CheckStreamPresence(stream);
					return new PdfFreeFormGourandShadedTriangleMesh(stream);
				case PdfLatticeFormGourandShadedTriangleMesh.Type:
					CheckStreamPresence(stream);
					return new PdfLatticeFormGourandShadedTriangleMesh(stream);
				case PdfCoonsPatchMesh.Type:
					CheckStreamPresence(stream);
					return new PdfCoonsPatchMesh(stream);
				case PdfTensorProductPatchMesh.Type:
					CheckStreamPresence(stream);
					return new PdfTensorProductPatchMesh(stream);
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					return null;
			}
		}
		readonly PdfColorSpace colorSpace;
		readonly PdfColor background;
		readonly PdfRectangle boundingBox;
		readonly bool antiAlias;
		readonly PdfObjectList<PdfCustomFunction> function;
		public PdfColorSpace ColorSpace { get { return colorSpace; } }
		public PdfColor Background { get { return background; } }
		public PdfRectangle BoundingBox { get { return boundingBox; } }
		public bool AntiAlias { get { return antiAlias; } }
		public IList<PdfCustomFunction> Function { get { return function; } }
		protected virtual bool IsFunctionRequired { get { return true; } }
		protected virtual int FunctionDomainDimension { get { return 1; } }
		protected abstract int ShadingType { get; }
		internal PdfColor TransformFunction(params double[] arguments) { 
			if (function == null)
				return colorSpace.Transform(new PdfColor(arguments));
			int functionCount = function.Count;
			double[] colorComponents;
			if (functionCount == 1)
				colorComponents = function[0].Transform(arguments);
			else {
				colorComponents = new double[colorSpace.ComponentsCount]; 
				for (int index = 0; index < functionCount; index++) 
					colorComponents[index] = function[index].Transform(new double[] { arguments[0] })[0];
			}
			return colorSpace.Transform(new PdfColor(colorComponents));
		}
		protected PdfShading(PdfReaderDictionary dictionary) : base (dictionary.Number){
			object value;
			if (!dictionary.TryGetValue(colorSpaceDictionaryKey, out value))
				PdfDocumentReader.ThrowIncorrectDataException();
			colorSpace = PdfColorSpace.Parse(dictionary.Objects, value);
			if (colorSpace is PdfPatternColorSpace)
				PdfDocumentReader.ThrowIncorrectDataException();
			IList<object> backgroundArray = dictionary.GetArray(backgroundDictionaryKey);
			if (backgroundArray != null) {
				int count = backgroundArray.Count;
				if (count != colorSpace.ComponentsCount)
					PdfDocumentReader.ThrowIncorrectDataException();
				double[] components = new double[count];
				for (int i = 0; i < count; i++)
					components[i] = PdfDocumentReader.ConvertToDouble(backgroundArray[i]);
				background = new PdfColor(components);
			}
			boundingBox = dictionary.GetRectangle(boundingBoxDictionaryKey);
			antiAlias = dictionary.GetBoolean(antiAliasDictionaryKey) ?? false;
			function = dictionary.GetFunctions(functionDictionaryKey, false);
			if (function == null) {
				if (IsFunctionRequired)
					PdfDocumentReader.ThrowIncorrectDataException();
			}
			else {
				if (colorSpace is PdfIndexedColorSpace)
					PdfDocumentReader.ThrowIncorrectDataException();
				int finctionDomainDimension = FunctionDomainDimension;
				if (function.Count == 1) {
					PdfCustomFunction customFunction = function[0];
					if (customFunction.Domain.Count != finctionDomainDimension || customFunction.RangeSize != colorSpace.ComponentsCount)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else {
					if (function.Count != colorSpace.ComponentsCount)
						PdfDocumentReader.ThrowIncorrectDataException();
					foreach (PdfCustomFunction customFunction in function)
						if (customFunction.Domain.Count != finctionDomainDimension || customFunction.RangeSize != 1)
							PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
		}
		protected PdfShading(PdfObjectList<PdfCustomFunction> blendFunctions) {
			function = blendFunctions;
			colorSpace = new PdfDeviceColorSpace(PdfDeviceColorSpaceKind.RGB);
		}
		protected virtual PdfDictionary CreateDictionary(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(shadingTypeDictionaryKey, ShadingType);
			dictionary.Add(colorSpaceDictionaryKey, colorSpace.Write(objects));
			if (background != null)
				dictionary.Add(backgroundDictionaryKey, background.ToWritableObject());
			dictionary.Add(boundingBoxDictionaryKey, boundingBox);
			dictionary.Add(antiAliasDictionaryKey, antiAlias, false);
			if (function != null)
				dictionary.Add(functionDictionaryKey, function.Count == 1 ? (PdfObject)function[0] : (PdfObject)function);
			return dictionary;
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return CreateDictionary(objects);
		}
	}
}
