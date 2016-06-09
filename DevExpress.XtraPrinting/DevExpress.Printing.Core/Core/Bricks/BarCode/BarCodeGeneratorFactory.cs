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
using System.Collections;
using DevExpress.XtraPrinting.BarCode;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.BarCode.Native {
	public enum BarCodeSymbology {
		Codabar = 1,		 
		Industrial2of5 = 2,  
		Interleaved2of5 = 3,
		Code39 = 4,		  
		Code39Extended = 5,
		Code93 = 6,		  
		Code93Extended = 7,
		Code128 = 8,		 
		Code11 = 9,		  
		CodeMSI = 10,
		PostNet = 11,
		EAN13 = 12,
		UPCA = 13,
		EAN8 = 14,
		EAN128 = 15,
		UPCSupplemental2 = 16,
		UPCSupplemental5 = 17,
		UPCE0 = 18,
		UPCE1 = 19,
		Matrix2of5 = 20,
		PDF417 = 21,
		DataMatrix = 22,
		QRCode = 23,
		IntelligentMail = 24,
		DataMatrixGS1 = 25,
		ITF14 = 26,
		DataBar = 27,
		IntelligentMailPackage = 28
	}
	public static class BarCodeGeneratorFactory {
		static Type defaultCodeType = typeof(CodabarGenerator);
		static Dictionary<BarCodeSymbology, Type> codesHT = new Dictionary<BarCodeSymbology, Type>();
		static BarCodeGeneratorFactory() {
			codesHT[BarCodeSymbology.Codabar] = typeof(CodabarGenerator);
			codesHT[BarCodeSymbology.Industrial2of5] = typeof(Industrial2of5Generator);
			codesHT[BarCodeSymbology.Interleaved2of5] = typeof(Interleaved2of5Generator);
			codesHT[BarCodeSymbology.Code39] = typeof(Code39Generator);
			codesHT[BarCodeSymbology.Code39Extended] = typeof(Code39ExtendedGenerator);
			codesHT[BarCodeSymbology.Code93] = typeof(Code93Generator);
			codesHT[BarCodeSymbology.Code93Extended] = typeof(Code93ExtendedGenerator);
			codesHT[BarCodeSymbology.Code128] = typeof(Code128Generator);
			codesHT[BarCodeSymbology.Code11] = typeof(Code11Generator);
			codesHT[BarCodeSymbology.CodeMSI] = typeof(CodeMSIGenerator);
			codesHT[BarCodeSymbology.PostNet] = typeof(PostNetGenerator);
			codesHT[BarCodeSymbology.EAN13] = typeof(EAN13Generator);
			codesHT[BarCodeSymbology.UPCA] = typeof(UPCAGenerator);
			codesHT[BarCodeSymbology.EAN8] = typeof(EAN8Generator);
			codesHT[BarCodeSymbology.EAN128] = typeof(EAN128Generator);
			codesHT[BarCodeSymbology.UPCSupplemental2] = typeof(UPCSupplemental2Generator);
			codesHT[BarCodeSymbology.UPCSupplemental5] = typeof(UPCSupplemental5Generator);
			codesHT[BarCodeSymbology.UPCE0] = typeof(UPCE0Generator);
			codesHT[BarCodeSymbology.UPCE1] = typeof(UPCE1Generator);
			codesHT[BarCodeSymbology.Matrix2of5] = typeof(Matrix2of5Generator);
			codesHT[BarCodeSymbology.PDF417] = typeof(PDF417Generator);
			codesHT[BarCodeSymbology.DataMatrix] = typeof(DataMatrixGenerator);
			codesHT[BarCodeSymbology.DataMatrixGS1] = typeof(DataMatrixGS1Generator);
			codesHT[BarCodeSymbology.QRCode] = typeof(QRCodeGenerator);
			codesHT[BarCodeSymbology.IntelligentMail] = typeof(IntelligentMailGenerator);
			codesHT[BarCodeSymbology.ITF14] = typeof(ITF14Generator);
			codesHT[BarCodeSymbology.DataBar] = typeof(DataBarGenerator);
#if !WINRT && !WP
			codesHT[BarCodeSymbology.IntelligentMailPackage] = typeof(IntelligentMailPackageGenerator);
#endif
		}
		static public BarCodeGeneratorBase Create(BarCodeSymbology symbologyCode) {
			Type type = codesHT[symbologyCode];
			if(type == null)
				type = defaultCodeType;
			return (BarCodeGeneratorBase)Activator.CreateInstance(type);
		}
	}
}
