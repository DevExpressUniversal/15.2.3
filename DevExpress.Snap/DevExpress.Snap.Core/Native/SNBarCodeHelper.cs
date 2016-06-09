#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.Snap.Core.Fields;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.BarCode.Native;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native {
	public static class SNBarCodeHelper {
		#region inner classes
		public class HelperEntity {
			[XtraSerializableProperty(-1)]
			public string Name { get; set; }
		}
		#endregion
		public static string GetGeneratorBase64String(BarCodeGeneratorBase barCodeGeneratorBase) {
			return StreamSerializeHelper.ToBase64String(barCodeGeneratorBase);
		}
		public static BarCodeGeneratorBase GetBarCodeGenerator(string generatorString) {
			BarCodeGeneratorBase result = GetBarCodeGeneratorCore(generatorString);
			StreamSerializeHelper.FromBase64String(generatorString, result);
			return result;
		}
		static BarCodeGeneratorBase GetBarCodeGeneratorCore(string generatorString) {
			HelperEntity helperEntity = StreamSerializeHelper.FromBase64String<HelperEntity>(generatorString);
			BarCodeSymbology symbology = (BarCodeSymbology)Enum.Parse(typeof(BarCodeSymbology), helperEntity.Name, true);
			return CreateBarCodeGenerator(symbology);
		}
		public static BarCodeGeneratorBase CreateBarCodeGenerator(BarCodeSymbology barCodeSymbology) {
			switch (barCodeSymbology) {
				case BarCodeSymbology.Codabar: return new CodabarGenerator();
				case BarCodeSymbology.Code11: return new Code11Generator();
				case BarCodeSymbology.Code39: return new Code39Generator();
				case BarCodeSymbology.Code39Extended: return new Code39ExtendedGenerator();
				case BarCodeSymbology.Code93: return new Code93Generator();
				case BarCodeSymbology.Code93Extended: return new Code93ExtendedGenerator();
				case BarCodeSymbology.Code128: return new Code128Generator();
				case BarCodeSymbology.CodeMSI: return new CodeMSIGenerator();
				case BarCodeSymbology.DataBar: return new DataBarGenerator();
				case BarCodeSymbology.DataMatrix: return new DataMatrixGenerator();
				case BarCodeSymbology.DataMatrixGS1: return new DataMatrixGS1Generator();
				case BarCodeSymbology.EAN8: return new EAN8Generator();
				case BarCodeSymbology.EAN13: return new EAN13Generator();
				case BarCodeSymbology.EAN128: return new EAN128Generator();
				case BarCodeSymbology.Industrial2of5: return new Industrial2of5Generator();
				case BarCodeSymbology.IntelligentMail: return new IntelligentMailGenerator();
				case BarCodeSymbology.Interleaved2of5: return new Interleaved2of5Generator();
				case BarCodeSymbology.ITF14: return new ITF14Generator();
				case BarCodeSymbology.Matrix2of5: return new Matrix2of5Generator();
				case BarCodeSymbology.PDF417: return new PDF417Generator();
				case BarCodeSymbology.PostNet: return new PostNetGenerator();
				case BarCodeSymbology.QRCode: return new QRCodeGenerator();
				case BarCodeSymbology.UPCA: return new UPCAGenerator();
				case BarCodeSymbology.UPCE0: return new UPCE0Generator();
				case BarCodeSymbology.UPCE1: return new UPCE1Generator();
				case BarCodeSymbology.UPCSupplemental2: return new UPCSupplemental2Generator();
				case BarCodeSymbology.UPCSupplemental5: return new UPCSupplemental5Generator();
				default: return new Code128Generator();
			}
		}
		public static DocumentModel CreateBarCodeGeneratorDocumentModel(BarCodeGeneratorBase barCodeGeneratorBase, SnapDocumentModel source) {
			DocumentModel result = source.CreateNew();
			result.IntermediateModel = true;
			result.BeginSetContent();
			try {
				result.InheritDataServices(source);
				Base64StringDataContainer container = new Base64StringDataContainer();
				container.SetData(StreamSerializeHelper.ToBase64String(barCodeGeneratorBase));
				result.MainPieceTable.InsertDataContainerRun(DocumentLogPosition.Zero, container, false);
			}
			finally {
				result.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, null);
			}
			return result;
		}
	}
}
