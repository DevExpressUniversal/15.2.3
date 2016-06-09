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

using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting {
#if !SL
	[
	BrickExporter(typeof(TextBrickExporter))
	]
#endif
	public class TextBrick : TextBrickBase, ITextBrick, ITableCell {
		const DefaultBoolean DefaultXlsExportNativeFormat = DefaultBoolean.Default;
		object textValue;
		string textValueFormatString;
		object ITableCell.TextValue { get { return TextValue; } }
		string ITableCell.FormatString { get { return TextValueFormatString; } }
		string ITableCell.XlsxFormatString { get { return XlsxFormatString; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickFont")]
#endif
		public Font Font { get { return Style.Font; } set { Style = BrickStyleHelper.ChangeFont(Style, value); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickStringFormat")]
#endif
		public BrickStringFormat StringFormat { get { return Style.StringFormat; } set { Style = BrickStyleHelper.ChangeStringFormat(Style, value); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickHorzAlignment")]
#endif
		public DevExpress.Utils.HorzAlignment HorzAlignment {
			get {
				return PSConvert.ToHorzAlignment(StringFormat.Alignment);
			}
			set {
				StringFormat = StringFormat.ChangeAlignment(PSConvert.ToStringAlignment(value));
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickVertAlignment")]
#endif
		public DevExpress.Utils.VertAlignment VertAlignment {
			get {
				return PSConvert.ToVertAlignment(StringFormat.LineAlignment);
			}
			set {
				StringFormat = StringFormat.ChangeLineAlignment(PSConvert.ToStringAlignment(value));
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextBrickXlsExportNativeFormat"),
#endif
		XtraSerializableProperty,
		DefaultValue(DefaultXlsExportNativeFormat),
		]
		public DevExpress.Utils.DefaultBoolean XlsExportNativeFormat {
			get { return (DevExpress.Utils.DefaultBoolean)flags[XlsExportNativeFormatSection]; }
			set { flags[XlsExportNativeFormatSection] = (short)value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextBrickTextValue"),
#endif
		XtraSerializableProperty,
		DefaultValue(null),
		EditorBrowsable(EditorBrowsableState.Always),
		]
		public override object TextValue {
			get {  return textValue is DateTimeOffset ? ((DateTimeOffset)textValue).DateTime : textValue; }
			set { textValue = value; }
		}
		internal virtual ConvertHelper ConvertHelper { get { return BrickOwner.ConvertHelper; } }
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextBrickTextValueFormatString"),
#endif
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public override string TextValueFormatString {
			get { return textValueFormatString; }
			set {
				if(!string.IsNullOrEmpty(value))
					textValueFormatString = value;
			}
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("TextBrickXlsxFormatString"),
#endif
		XtraSerializableProperty,
		DefaultValue(null),
		]
		public override string XlsxFormatString {
			get {
				string s = GetValue(BrickAttachedProperties.XlsxFormatString, null);
#if !SL
				if(string.IsNullOrEmpty(s) && textValue is DateTimeOffset)
					return DevExpress.XtraExport.ExportUtils.GetDateTimeFormatString(((DateTimeOffset)textValue).Offset);
#endif
				return s;
			}
			set { SetAttachedValue(BrickAttachedProperties.XlsxFormatString, value, null); }
		}
		protected internal override bool ShouldApplyPaddingInternal { get { return true; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("TextBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Text; } }
		public TextBrick() {
			XlsExportNativeFormat = DefaultXlsExportNativeFormat;
		}
		public TextBrick(BrickStyle style)
			: base(style) {
			XlsExportNativeFormat = DefaultXlsExportNativeFormat;
		}
		public TextBrick(BorderSide sides, float borderWidth, Color borderColor, Color backColor, Color foreColor)
			: base(sides, borderWidth, borderColor, backColor, foreColor) {
			XlsExportNativeFormat = DefaultXlsExportNativeFormat;
		}
		public TextBrick(IBrickOwner brickOwner) 
			: base(brickOwner) {
			XlsExportNativeFormat = DefaultXlsExportNativeFormat;
		}
		internal TextBrick(TextBrick brick) : base(brick) {
			textValue = brick.textValue;
			textValueFormatString = brick.textValueFormatString;
			XlsxFormatString = brick.XlsxFormatString;
		}
		protected override float ValidatePageBottomInternal(float pageBottom, RectangleF brickRect, IPrintingSystemContext context) {
#if SL
			return pageBottom;
#else
			RectangleF clientRect = GetClientRectangle(brickRect, GraphicsDpi.Document);
			clientRect = Padding.Deflate(clientRect, GraphicsDpi.Document);
			StringFormat sf = StringFormat.Value;
			StringFormatFlags flags = sf.FormatFlags;
			sf.SetTabStops(0, Style.CalculateTabStops(context.Measurer));
			try {
				sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
				return new LineSplitter(Text, Style.Font, sf).SplitRectangle(clientRect, pageBottom, brickRect.Top, GraphicsUnit.Document);
			} finally {
				sf.FormatFlags = flags;
			}
#endif
		}
		#region ICloneable Members
		public override object Clone() {
			return new TextBrick(this);
		}
		#endregion
	}
}
