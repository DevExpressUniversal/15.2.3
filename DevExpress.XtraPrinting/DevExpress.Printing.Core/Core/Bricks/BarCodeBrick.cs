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

using DevExpress.XtraPrinting.BarCode;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
using System.ComponentModel;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(BarCodeBrickExporter))]
	public class BarCodeBrick : TextBrickBase, IBarCodeData {
		public const float DefaultModule = 2.0f;
		public const bool DefaultAutoModule = false;
		public const bool DefaultShowText = true;
		public const TextAlignment DefaultAlignment = TextAlignment.TopLeft;
		public const BarCodeOrientation DefaultOrientation = BarCodeOrientation.Normal;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public const string DefaultBinaryDataBase64 = "";
		double module = DefaultModule;
		bool autoModule = DefaultAutoModule;
		bool showText = DefaultShowText;
		BarCodeOrientation orientation = DefaultOrientation;
		BarCodeGeneratorBase generator = new Code128Generator();
		TextAlignment alignment = DefaultAlignment;
		byte[] binaryData = new byte[0];
		float fromDpi = GraphicsDpi.Document;
		float toDpi = GraphicsDpi.Document;
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal float ToDpi {
			get { return toDpi; }
			set { toDpi = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal float FromDpi {
			get { return fromDpi; }
			set { fromDpi = value; }
		}
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickModule"),
#endif
		XtraSerializableProperty,
		DefaultValue((double)DefaultModule),
		]
		public double Module { 
			get {
				if (FromDpi != ToDpi)
					return DevExpress.XtraReports.UI.XRConvert.Convert((float)module, FromDpi, ToDpi);
				return module; 
			} 
			set { module = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickAutoModule"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultAutoModule),
		]
		public bool AutoModule { get { return autoModule; } set { autoModule = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickShowText"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultShowText),
		]
		public bool ShowText { get { return showText; } set { showText = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickOrientation"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultOrientation),
		]
		public BarCodeOrientation Orientation { get { return orientation; } set { orientation = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickAlignment"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultAlignment),
		]
		public TextAlignment Alignment { get { return alignment; } set { alignment = value; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickGenerator"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public BarCodeGeneratorBase Generator {
			get { return generator; }
			set {
				if(value == null)
					throw new ArgumentNullException();
				generator = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BarCodeBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.BarCode; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BarCodeBrickBinaryData")]
#endif
		public byte[] BinaryData {
			get { return binaryData; }
			set {
				if(value != null)
					binaryData = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("BarCodeBrickBinaryDataBase64"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultBinaryDataBase64),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public string BinaryDataBase64 {
			get { return Convert.ToBase64String(BinaryData); }
			set { BinaryData = Convert.FromBase64String(value); }
		}
		public BarCodeBrick()
			: this(NullBrickOwner.Instance) {
		}
		public BarCodeBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			Text = string.Empty;
		}
		internal BarCodeBrick(BarCodeBrick barCodeBrick)
			: base(barCodeBrick) {
			this.alignment = barCodeBrick.Alignment;
			this.autoModule = barCodeBrick.AutoModule;
			this.binaryData = barCodeBrick.BinaryData;
			this.generator = barCodeBrick.Generator;
			this.module = barCodeBrick.Module;
			this.orientation = barCodeBrick.Orientation;
			this.showText = barCodeBrick.ShowText;
			this.toDpi = barCodeBrick.ToDpi;
			this.fromDpi = barCodeBrick.FromDpi;
		}
		#region serialization
		protected override object CreateContentPropertyValue(XtraItemEventArgs e) {
			if(e.Item.Name == PrintingSystemSerializationNames.Generator) {
				BarCodeSymbology symbologyCode;
				try {
					symbologyCode = (BarCodeSymbology)Enum.Parse(typeof(BarCodeSymbology), BrickFactory.GetStringProperty(e, "Name"));
				} catch {
					symbologyCode = BarCodeSymbology.Codabar;
				}
				return BarCodeGeneratorFactory.Create(symbologyCode);
			}
			return base.CreateContentPropertyValue(e);
		}
		#endregion
		#region ICloneable Members
		public override object Clone() {
			return new BarCodeBrick(this);
		}
		#endregion
		protected internal override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			module = MathMethods.Scale(module, scaleFactor);
		}
	}
}
