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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IParsedThingFactory
	public interface IParsedThingFactory {
		short GetTypeCodeByType(Type thingType);
		IParsedThing CreateParsedThing(short typeCode);
		IParsedThing CreateParsedThing(BinaryReader reader);
	}
	#endregion
	#region ParsedThingFactoryBase (abstract factory)
	public abstract class ParsedThingFactoryBase : IParsedThingFactory {
		#region Internal classes
		protected interface IParsedThingCreator {
			IParsedThing Create();
		}
		class ParsedThingSharedInstance : IParsedThingCreator {
			IParsedThing sharedInstance;
			public ParsedThingSharedInstance(IParsedThing sharedThing) {
				this.sharedInstance = sharedThing;
			}
			#region IParsedThingCreator Members
			public IParsedThing Create() {
				return this.sharedInstance;
			}
			#endregion
		}
		#region ParsedThingCreators
		protected class ParsedThingExpCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingExp(); } }
		protected class ParsedThingDataTableCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingDataTable(); } }
		#region Elf
		protected class ParsedThingElfLelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfLel(); } }
		protected class ParsedThingElfRwCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfRw(); } }
		protected class ParsedThingElfColCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfCol(); } }
		protected class ParsedThingElfRwVCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfRwV(); } }
		protected class ParsedThingElfColVCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfColV(); } }
		protected class ParsedThingElfRadicalCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfRadical(); } }
		protected class ParsedThingElfRadicalSCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfRadicalS(); } }
		protected class ParsedThingElfColSCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfColS(); } }
		protected class ParsedThingElfColSVCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfColSV(); } }
		protected class ParsedThingElfRadicalLelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingElfRadicalLel(); } }
		protected class ParsedThingSxNameCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingSxName(); } }
		#endregion
		#region Attributes
		protected class ParsedThingAttrSemiCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrSemi(); } }
		protected class ParsedThingAttrIfCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrIf(); } }
		protected class ParsedThingAttrChooseCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrChoose(); } }
		protected class ParsedThingAttrGotoCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrGoto(); } }
		protected class ParsedThingAttrSumCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrSum(); } }
		protected class ParsedThingAttrBaxcelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrBaxcel(); } }
		protected class ParsedThingAttrBaxcelVolatileCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrBaxcelVolatile(); } }
		protected class ParsedThingAttrSpaceCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrSpace(); } }
		protected class ParsedThingAttrSpaceSemiCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAttrSpaceSemi(); } }
		#endregion
		#region Operand
		protected class ParsedThingStringValueCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingStringValue(); } }
		protected class ParsedThingErrorCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingError(); } }
		protected class ParsedThingBooleanCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingBoolean(); } }
		protected class ParsedThingIntegerCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingInteger(); } }
		protected class ParsedThingNumericCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingNumeric(); } }
		protected class ParsedThingArrayCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingArray(); } }
		protected class ParsedThingFuncCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingFunc(); } }
		protected class ParsedThingFuncVarCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingFuncVar(); } }
		protected class ParsedThingUnknownFuncCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingUnknownFunc(); } }
		protected class ParsedThingCustomFuncCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingCustomFunc(); } }
		protected class ParsedThingUnknownFuncExtCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingUnknownFuncExt(); } }
		protected class ParsedThingAddinFuncCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAddinFunc(); } }
		protected class ParsedThingNameCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingName(); } }
		protected class ParsedThingNameXCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingNameX(); } }
		protected class ParsedThingMemAreaCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingMemArea(); } }
		protected class ParsedThingMemErrCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingMemErr(); } }
		protected class ParsedThingMemNoMemCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingMemNoMem(); } }
		protected class ParsedThingMemFuncCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingMemFunc(); } }
		protected class ParsedThingAreaCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingArea(); } }
		protected class ParsedThingAreaErrCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAreaErr(); } }
		protected class ParsedThingArea3dCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingArea3d(); } }
		protected class ParsedThingAreaErr3dCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAreaErr3d(); } }
		protected class ParsedThingAreaNCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingAreaN(); } }
		protected class ParsedThingArea3dRelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingArea3dRel(); } }
		protected class ParsedThingRefCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingRef(); } }
		protected class ParsedThingRefErrCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingRefErr(); } }
		protected class ParsedThingRef3dCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingRef3d(); } }
		protected class ParsedThingErr3dCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingErr3d(); } }
		protected class ParsedThingRefRelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingRefRel(); } }
		protected class ParsedThingRef3dRelCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingRef3dRel(); } }
		protected class ParsedThingTableCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingTable(); } }
		protected class ParsedThingTableExtCreator : IParsedThingCreator { public IParsedThing Create() { return new ParsedThingTableExt(); } }
		#endregion
		#endregion
		#endregion
		Dictionary<short, IParsedThingCreator> ptgTypes;
		Dictionary<Type, short> typeCodes;
		protected ParsedThingFactoryBase() {
			ptgTypes = new Dictionary<short, IParsedThingCreator>();
			typeCodes = new Dictionary<Type, short>();
			InitializeFactory();
		}
		#region IParsedThingFactory Members
		public short GetTypeCodeByType(Type thingType) {
			short typeCode;
			if (!typeCodes.TryGetValue(thingType, out typeCode))
				typeCode = 0x0000;
			return typeCode;
		}
		public IParsedThing CreateParsedThing(short typeCode) {
			IParsedThing ptgInstance = ptgTypes[typeCode].Create();
			OperandDataType dataType = GetDataType((typeCode & 0x0060) >> 5);
			if (dataType != OperandDataType.None)
				ptgInstance.DataType = dataType;
			return ptgInstance;
		}
		public IParsedThing CreateParsedThing(BinaryReader reader) {
			ushort typeCode = reader.ReadByte();
			if (typeCode == 0x18 || typeCode == 0x19) {
				ushort subTypeCode = reader.ReadByte();
				if (subTypeCode == 0)
					subTypeCode = 8;
				typeCode |= (ushort)(subTypeCode << 8);
			}
			return CreateParsedThing((short)typeCode);
		}
		OperandDataType GetDataType(int code) {
			switch (code) {
				case 1:
					return OperandDataType.Reference;
				case 2:
					return OperandDataType.Value;
				case 3:
					return OperandDataType.Array;
			}
			return OperandDataType.None;
		}
		#endregion
		protected void AddProduct(short typeCode, Type thingType, IParsedThingCreator creator) {
			ptgTypes.Add(typeCode, creator);
			if (!typeCodes.ContainsKey(thingType))
				typeCodes.Add(thingType, typeCode);
		}
		protected void AddProduct(short typeCode, IParsedThing sharedThing) {
			ptgTypes.Add(typeCode, new ParsedThingSharedInstance(sharedThing));
			Type thingType = sharedThing.GetType();
			if (!typeCodes.ContainsKey(thingType))
				typeCodes.Add(thingType, typeCode);
		}
		protected virtual void InitializeFactory() {
			AddProduct(0x0001, typeof(ParsedThingExp), new ParsedThingExpCreator());
			AddProduct(0x0002, typeof(ParsedThingDataTable), new ParsedThingDataTableCreator());
			AddProduct(0x0003, ParsedThingAdd.Instance);
			AddProduct(0x0004, ParsedThingSubtract.Instance);
			AddProduct(0x0005, ParsedThingMultiply.Instance);
			AddProduct(0x0006, ParsedThingDivide.Instance);
			AddProduct(0x0007, ParsedThingPower.Instance);
			AddProduct(0x0008, ParsedThingConcat.Instance);
			AddProduct(0x0009, ParsedThingLess.Instance);
			AddProduct(0x000a, ParsedThingLessEqual.Instance);
			AddProduct(0x000b, ParsedThingEqual.Instance);
			AddProduct(0x000c, ParsedThingGreaterEqual.Instance);
			AddProduct(0x000d, ParsedThingGreater.Instance);
			AddProduct(0x000e, ParsedThingNotEqual.Instance);
			AddProduct(0x000f, ParsedThingIntersect.Instance);
			AddProduct(0x0010, ParsedThingUnion.Instance);
			AddProduct(0x0011, ParsedThingRange.Instance);
			AddProduct(0x0012, ParsedThingUnaryPlus.Instance);
			AddProduct(0x0013, ParsedThingUnaryMinus.Instance);
			AddProduct(0x0014, ParsedThingPercent.Instance);
			AddProduct(0x0015, ParsedThingParentheses.Instance);
			AddProduct(0x0016, ParsedThingMissingArg.Instance);
			AddProduct(0x0017, typeof(ParsedThingStringValue), new ParsedThingStringValueCreator());
			AddProduct(0x0118, typeof(ParsedThingElfLel), new ParsedThingElfLelCreator());
			AddProduct(0x0218, typeof(ParsedThingElfRw), new ParsedThingElfRwCreator());
			AddProduct(0x0318, typeof(ParsedThingElfCol), new ParsedThingElfColCreator());
			AddProduct(0x0618, typeof(ParsedThingElfRwV), new ParsedThingElfRwVCreator());
			AddProduct(0x0718, typeof(ParsedThingElfColV), new ParsedThingElfColVCreator());
			AddProduct(0x0a18, typeof(ParsedThingElfRadical), new ParsedThingElfRadicalCreator());
			AddProduct(0x0b18, typeof(ParsedThingElfRadicalS), new ParsedThingElfRadicalSCreator());
			AddProduct(0x0d18, typeof(ParsedThingElfColS), new ParsedThingElfColSCreator());
			AddProduct(0x0f18, typeof(ParsedThingElfColSV), new ParsedThingElfColSVCreator());
			AddProduct(0x1018, typeof(ParsedThingElfRadicalLel), new ParsedThingElfRadicalLelCreator());
			AddProduct(0x1d18, typeof(ParsedThingSxName), new ParsedThingSxNameCreator());
			AddProduct(0x0119, typeof(ParsedThingAttrSemi), new ParsedThingAttrSemiCreator());
			AddProduct(0x0219, typeof(ParsedThingAttrIf), new ParsedThingAttrIfCreator());
			AddProduct(0x0419, typeof(ParsedThingAttrChoose), new ParsedThingAttrChooseCreator());
			AddProduct(0x0819, typeof(ParsedThingAttrGoto), new ParsedThingAttrGotoCreator());
			AddProduct(0x1019, typeof(ParsedThingAttrSum), new ParsedThingAttrSumCreator());
			AddProduct(0x2019, typeof(ParsedThingAttrBaxcel), new ParsedThingAttrBaxcelCreator());
			AddProduct(0x2119, typeof(ParsedThingAttrBaxcelVolatile), new ParsedThingAttrBaxcelVolatileCreator());
			AddProduct(0x4019, typeof(ParsedThingAttrSpace), new ParsedThingAttrSpaceCreator());
			AddProduct(0x4119, typeof(ParsedThingAttrSpaceSemi), new ParsedThingAttrSpaceSemiCreator());
			AddProduct(0x001c, typeof(ParsedThingError), new ParsedThingErrorCreator());
			AddProduct(0x001d, typeof(ParsedThingBoolean), new ParsedThingBooleanCreator());
			AddProduct(0x001e, typeof(ParsedThingInteger), new ParsedThingIntegerCreator());
			AddProduct(0x001f, typeof(ParsedThingNumeric), new ParsedThingNumericCreator());
			AddProduct(0x0020, typeof(ParsedThingArray), new ParsedThingArrayCreator());
			AddProduct(0x0040, typeof(ParsedThingArray), new ParsedThingArrayCreator());
			AddProduct(0x0060, typeof(ParsedThingArray), new ParsedThingArrayCreator());
			AddProduct(0x0021, typeof(ParsedThingFunc), new ParsedThingFuncCreator());
			AddProduct(0x0041, typeof(ParsedThingFunc), new ParsedThingFuncCreator());
			AddProduct(0x0061, typeof(ParsedThingFunc), new ParsedThingFuncCreator());
			AddProduct(0x0022, typeof(ParsedThingFuncVar), new ParsedThingFuncVarCreator());
			AddProduct(0x0042, typeof(ParsedThingFuncVar), new ParsedThingFuncVarCreator());
			AddProduct(0x0062, typeof(ParsedThingFuncVar), new ParsedThingFuncVarCreator());
			AddProduct(0x0023, typeof(ParsedThingName), new ParsedThingNameCreator());
			AddProduct(0x0043, typeof(ParsedThingName), new ParsedThingNameCreator());
			AddProduct(0x0063, typeof(ParsedThingName), new ParsedThingNameCreator());
			AddProduct(0x0024, typeof(ParsedThingRef), new ParsedThingRefCreator());
			AddProduct(0x0044, typeof(ParsedThingRef), new ParsedThingRefCreator());
			AddProduct(0x0064, typeof(ParsedThingRef), new ParsedThingRefCreator());
			AddProduct(0x0025, typeof(ParsedThingArea), new ParsedThingAreaCreator());
			AddProduct(0x0045, typeof(ParsedThingArea), new ParsedThingAreaCreator());
			AddProduct(0x0065, typeof(ParsedThingArea), new ParsedThingAreaCreator());
			AddProduct(0x0026, typeof(ParsedThingMemArea), new ParsedThingMemAreaCreator());
			AddProduct(0x0046, typeof(ParsedThingMemArea), new ParsedThingMemAreaCreator());
			AddProduct(0x0066, typeof(ParsedThingMemArea), new ParsedThingMemAreaCreator());
			AddProduct(0x0027, typeof(ParsedThingMemErr), new ParsedThingMemErrCreator());
			AddProduct(0x0047, typeof(ParsedThingMemErr), new ParsedThingMemErrCreator());
			AddProduct(0x0067, typeof(ParsedThingMemErr), new ParsedThingMemErrCreator());
			AddProduct(0x0028, typeof(ParsedThingMemNoMem), new ParsedThingMemNoMemCreator());
			AddProduct(0x0048, typeof(ParsedThingMemNoMem), new ParsedThingMemNoMemCreator());
			AddProduct(0x0068, typeof(ParsedThingMemNoMem), new ParsedThingMemNoMemCreator());
			AddProduct(0x0029, typeof(ParsedThingMemFunc), new ParsedThingMemFuncCreator());
			AddProduct(0x0049, typeof(ParsedThingMemFunc), new ParsedThingMemFuncCreator());
			AddProduct(0x0069, typeof(ParsedThingMemFunc), new ParsedThingMemFuncCreator());
			AddProduct(0x002a, typeof(ParsedThingRefErr), new ParsedThingRefErrCreator());
			AddProduct(0x004a, typeof(ParsedThingRefErr), new ParsedThingRefErrCreator());
			AddProduct(0x006a, typeof(ParsedThingRefErr), new ParsedThingRefErrCreator());
			AddProduct(0x002b, typeof(ParsedThingAreaErr), new ParsedThingAreaErrCreator());
			AddProduct(0x004b, typeof(ParsedThingAreaErr), new ParsedThingAreaErrCreator());
			AddProduct(0x006b, typeof(ParsedThingAreaErr), new ParsedThingAreaErrCreator());
			AddProduct(0x002c, typeof(ParsedThingRefRel), new ParsedThingRefRelCreator());
			AddProduct(0x004c, typeof(ParsedThingRefRel), new ParsedThingRefRelCreator());
			AddProduct(0x006c, typeof(ParsedThingRefRel), new ParsedThingRefRelCreator());
			AddProduct(0x002d, typeof(ParsedThingAreaN), new ParsedThingAreaNCreator());
			AddProduct(0x004d, typeof(ParsedThingAreaN), new ParsedThingAreaNCreator());
			AddProduct(0x006d, typeof(ParsedThingAreaN), new ParsedThingAreaNCreator());
			AddProduct(0x0039, typeof(ParsedThingNameX), new ParsedThingNameXCreator());
			AddProduct(0x0059, typeof(ParsedThingNameX), new ParsedThingNameXCreator());
			AddProduct(0x0079, typeof(ParsedThingNameX), new ParsedThingNameXCreator());
			AddProduct(0x003c, typeof(ParsedThingErr3d), new ParsedThingErr3dCreator());
			AddProduct(0x005c, typeof(ParsedThingErr3d), new ParsedThingErr3dCreator());
			AddProduct(0x007c, typeof(ParsedThingErr3d), new ParsedThingErr3dCreator());
			AddProduct(0x003d, typeof(ParsedThingAreaErr3d), new ParsedThingAreaErr3dCreator());
			AddProduct(0x005d, typeof(ParsedThingAreaErr3d), new ParsedThingAreaErr3dCreator());
			AddProduct(0x007d, typeof(ParsedThingAreaErr3d), new ParsedThingAreaErr3dCreator());
		}
	}
	#endregion
	#region CommonParsedThingFactory (concrete factory)
	public class CommonParsedThingFactory : ParsedThingFactoryBase {
		protected override void InitializeFactory() {
			base.InitializeFactory();
			AddProduct(0x003a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x005a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x007a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x003b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
			AddProduct(0x005b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
			AddProduct(0x007b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
		}
	}
	#endregion
	#region NameParsedThingFactory (concrete factory)
	public class NameParsedThingFactory : ParsedThingFactoryBase {
		protected override void InitializeFactory() {
			base.InitializeFactory();
			AddProduct(0x003a, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x005a, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x007a, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x003b, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
			AddProduct(0x005b, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
			AddProduct(0x007b, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
		}
	}
	#endregion
	#region ModelCommonParsedThingFactory (concrete factory)
	public class ModelCommonParsedThingFactory : ParsedThingFactoryBase {
		protected override void InitializeFactory() {
			base.InitializeFactory();
			AddProduct(0x003a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x005a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x007a, typeof(ParsedThingRef3d), new ParsedThingRef3dCreator());
			AddProduct(0x003b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
			AddProduct(0x005b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
			AddProduct(0x007b, typeof(ParsedThingArea3d), new ParsedThingArea3dCreator());
			AddProduct(0x003e, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x005e, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x007e, typeof(ParsedThingRef3dRel), new ParsedThingRef3dRelCreator());
			AddProduct(0x003f, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
			AddProduct(0x005f, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
			AddProduct(0x007f, typeof(ParsedThingArea3dRel), new ParsedThingArea3dRelCreator());
			AddProduct(0x0031, typeof(ParsedThingTable), new ParsedThingTableCreator());
			AddProduct(0x0051, typeof(ParsedThingTable), new ParsedThingTableCreator());
			AddProduct(0x0071, typeof(ParsedThingTable), new ParsedThingTableCreator());
			AddProduct(0x0032, typeof(ParsedThingTableExt), new ParsedThingTableExtCreator());
			AddProduct(0x0052, typeof(ParsedThingTableExt), new ParsedThingTableExtCreator());
			AddProduct(0x0072, typeof(ParsedThingTableExt), new ParsedThingTableExtCreator());
			AddProduct(0x0033, typeof(ParsedThingUnknownFunc), new ParsedThingUnknownFuncCreator());
			AddProduct(0x0053, typeof(ParsedThingUnknownFunc), new ParsedThingUnknownFuncCreator());
			AddProduct(0x0073, typeof(ParsedThingUnknownFunc), new ParsedThingUnknownFuncCreator());
			AddProduct(0x0034, typeof(ParsedThingUnknownFuncExt), new ParsedThingUnknownFuncExtCreator());
			AddProduct(0x0054, typeof(ParsedThingUnknownFuncExt), new ParsedThingUnknownFuncExtCreator());
			AddProduct(0x0074, typeof(ParsedThingUnknownFuncExt), new ParsedThingUnknownFuncExtCreator());
			AddProduct(0x0035, typeof(ParsedThingAddinFunc), new ParsedThingAddinFuncCreator());
			AddProduct(0x0055, typeof(ParsedThingAddinFunc), new ParsedThingAddinFuncCreator());
			AddProduct(0x0075, typeof(ParsedThingAddinFunc), new ParsedThingAddinFuncCreator());
			AddProduct(0x0036, typeof(ParsedThingCustomFunc), new ParsedThingCustomFuncCreator());
			AddProduct(0x0056, typeof(ParsedThingCustomFunc), new ParsedThingCustomFuncCreator());
			AddProduct(0x0076, typeof(ParsedThingCustomFunc), new ParsedThingCustomFuncCreator());
		}
	}
	#endregion
	#region ParsedThingFactories (static container)
	public static class ParsedThingFactories {
		#region Fields
		static readonly ModelCommonParsedThingFactory modelCommonFactory = new ModelCommonParsedThingFactory();
		static readonly CommonParsedThingFactory commonFactory = new CommonParsedThingFactory();
		static readonly NameParsedThingFactory nameFactory = new NameParsedThingFactory();
		#endregion
		#region Properties
		public static IParsedThingFactory ModelCommonFactory { get { return modelCommonFactory; } }
		public static IParsedThingFactory CommonFactory { get { return commonFactory; } }
		public static IParsedThingFactory NameFactory { get { return nameFactory; } }
		#endregion
	}
	#endregion
}
