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

using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.BarCode;
namespace DevExpress.XtraReports.UI.BarCode {
	[Obsolete("The DevExpress.XtraReports.UI.BarCodeXRCodabarGenerator. class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.CodabarGenerator class instead.")]
	public class XRCodabarGenerator : CodabarGenerator, IXRSerializable {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRCodabarGeneratorStartStopSymbols")]
#endif
		public CodabarStartStopPair StartStopSymbols {
			get { return (CodabarStartStopPair)StartStopPair; }
			set { StartStopPair = (DevExpress.XtraPrinting.BarCode.CodabarStartStopPair)value; }
		}
		public XRCodabarGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
#pragma warning disable 0618
			StartStopPair = (DevExpress.XtraPrinting.BarCode.CodabarStartStopPair)serializer.DeserializeEnum("StartStopSymbols", typeof(CodabarStartStopPair), CodabarStartStopPair.AT);
#pragma warning restore 0618
			WideNarrowRatio = serializer.DeserializeSingle("WideNarrowRatio", defaultWideNarrowRatio);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRIndustrial2of5Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Industrial2of5Generator class instead.")]
	public class XRIndustrial2of5Generator : Industrial2of5Generator, IXRSerializable {
		public XRIndustrial2of5Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
			WideNarrowRatio = serializer.DeserializeSingle("WideNarrowRatio", defaultWideNarrowRatio);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRInterleaved2of5Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator class instead.")]
	public class XRInterleaved2of5Generator : Interleaved2of5Generator, IXRSerializable {
		public XRInterleaved2of5Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
			WideNarrowRatio = serializer.DeserializeSingle("WideNarrowRatio", defaultWideNarrowRatio);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode39Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code39Generator class instead.")]
	public class XRCode39Generator : Code39Generator, IXRSerializable {
		public XRCode39Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
			WideNarrowRatio = serializer.DeserializeSingle("WideNarrowRatio", defaultWideNarrowRatio);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode128Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code128Generator class instead.")]
	public class XRCode128Generator : Code128Generator, IXRSerializable {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRCode128GeneratorCharSet")]
#endif
		public Code128Charset CharSet {
			get { return (Code128Charset)CharacterSet; }
			set { CharacterSet = (DevExpress.XtraPrinting.BarCode.Code128Charset)value; }
		}
		public XRCode128Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
#pragma warning disable 0618
			CharacterSet = (DevExpress.XtraPrinting.BarCode.Code128Charset)serializer.DeserializeEnum("CharSet", typeof(Code128Charset), Code128Charset.CharsetA);
#pragma warning restore 0618
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XREAN128Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.EAN128Generator class instead.")]
	public class XREAN128Generator : EAN128Generator, IXRSerializable {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XREAN128GeneratorCharSet")]
#endif
		public Code128Charset CharSet {
			get { return (Code128Charset)CharacterSet; }
			set { CharacterSet = (DevExpress.XtraPrinting.BarCode.Code128Charset)value; }
		}
		public XREAN128Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
			FNC1Substitute = serializer.DeserializeString("FNC1Substitute", defaultFNC1Subst);
			HumanReadableText = serializer.DeserializeBoolean("HumanReadableText", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCodeMSIGenerator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.CodeMSIGenerator class instead.")]
	public class XRCodeMSIGenerator : CodeMSIGenerator, IXRSerializable {
#if !SL
	[DevExpressXtraReportsLocalizedDescription("XRCodeMSIGeneratorCheckSum")]
#endif
		public MSICheckSum CheckSum {
			get { return (MSICheckSum)MSICheckSum; }
			set { MSICheckSum = (DevExpress.XtraPrinting.BarCode.MSICheckSum)value; }
		}
		public XRCodeMSIGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
#pragma warning disable 0618
			MSICheckSum = (DevExpress.XtraPrinting.BarCode.MSICheckSum)serializer.DeserializeEnum("CheckSum", typeof(MSICheckSum), (MSICheckSum)defaultCheckSum);
#pragma warning restore 0618
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode11Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code11Generator class instead.")]
	public class XRCode11Generator : Code11Generator, IXRSerializable {
		public XRCode11Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode39ExtendedGenerator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code39ExtendedGenerator class instead.")]
	public class XRCode39ExtendedGenerator : Code39ExtendedGenerator, IXRSerializable {
		public XRCode39ExtendedGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode93ExtendedGenerator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code93ExtendedGenerator class instead.")]
	public class XRCode93ExtendedGenerator: Code93ExtendedGenerator, IXRSerializable {
		public XRCode93ExtendedGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRCode93Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Code93Generator class instead.")]
	public class XRCode93Generator: Code93Generator, IXRSerializable {
		public XRCode93Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XREAN13Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.EAN13Generator class instead.")]
	public class XREAN13Generator: EAN13Generator, IXRSerializable {
		public XREAN13Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XREAN8Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.EAN8Generator class instead.")]
	public class XREAN8Generator : EAN8Generator, IXRSerializable {
		public XREAN8Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRMatrix2of5Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.Matrix2of5Generator class instead.")]
	public class XRMatrix2of5Generator : Matrix2of5Generator, IXRSerializable {
		public XRMatrix2of5Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRPostNetGenerator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.PostNetGenerator class instead.")]
	public class XRPostNetGenerator: PostNetGenerator, IXRSerializable {
		public XRPostNetGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRUPCAGenerator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.UPCAGenerator class instead.")]
	public class XRUPCAGenerator: UPCAGenerator, IXRSerializable {
		public XRUPCAGenerator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRUPCE0Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.UPCE0Generator class instead.")]
	public class XRUPCE0Generator: UPCE0Generator, IXRSerializable {
		public XRUPCE0Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRUPCE1Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.UPCE1Generator class instead.")]
	public class XRUPCE1Generator: UPCE1Generator, IXRSerializable {
		public XRUPCE1Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRUPCSupplemental2Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.UPCSupplemental2Generator class instead.")]
	public class XRUPCSupplemental2Generator: UPCSupplemental2Generator, IXRSerializable {
		public XRUPCSupplemental2Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
	[Obsolete("The DevExpress.XtraReports.UI.BarCode.XRUPCSupplemental5Generator class is now obsolete. Use the DevExpress.XtraPrinting.BarCode.UPCSupplemental5Generator class instead.")]
	public class XRUPCSupplemental5Generator: UPCSupplemental5Generator, IXRSerializable {
		public XRUPCSupplemental5Generator() { }
		IList IXRSerializable.SerializableObjects { get { return new object[] { }; } }
		void IXRSerializable.SerializeProperties(XRSerializer serializer) { }
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			CalcCheckSum = serializer.DeserializeBoolean("CalcCheckSum", true);
		}
	}
}
