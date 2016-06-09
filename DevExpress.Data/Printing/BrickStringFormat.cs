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
using System.Drawing;
using System.Drawing.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Text;
#if SL || DXPORTABLE
using DevExpress.Xpf.Drawing;
#endif
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Helpers;
namespace DevExpress.XtraPrinting.Helpers {
	public static class ReportSpecificEnumHelper {
		public static short GetEnumMaxValue(Type enumType) {
			uint value = 0;
			Array enumElements = EnumExtensions.GetValues(enumType);
			foreach(object element in enumElements) {
				value |= Convert.ToUInt32(element);
			}
			return (short)value;
		}
	}
}
namespace DevExpress.XtraPrinting
{
	public enum BrickStringFormatPrototypeKind { 
		Default = 0,
		GenericDefault = 1,
		GenericTypographic = 2
	}
	public class BrickStringFormat : ICloneable, IDisposable {
#region static
		static readonly StringFormat DefaultStringFormat = new StringFormat();
		static readonly StringAlignment DefaultAlignment;
		static readonly StringAlignment DefaultLineAlignment;
		static readonly StringFormatFlags DefaultFormatFlags;
		static readonly HotkeyPrefix DefaultHotkeyPrefix;
		static readonly StringTrimming DefaultTrimming;
		static readonly StringFormat[] prototypes = new StringFormat[] { DefaultStringFormat, StringFormat.GenericDefault, StringFormat.GenericTypographic };
		static readonly int AlignmentOffset;
		static readonly int LineAlignmentOffset;
		static readonly int FormatFlagsOffset;
		static readonly int HotkeyPrefixOffset;
		static readonly int TrimmingOffset;
		static readonly int PrototypeKindOffset; 
		static BrickStringFormat() {
			DefaultAlignment = DefaultStringFormat.Alignment;
			DefaultLineAlignment = DefaultStringFormat.LineAlignment;
			DefaultFormatFlags = DefaultStringFormat.FormatFlags;
			DefaultHotkeyPrefix = DefaultStringFormat.HotkeyPrefix;
			DefaultTrimming = DefaultStringFormat.Trimming;
			BitVector32.Section section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(StringAlignment)));
			AlignmentOffset = section.Offset;
			section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(StringAlignment)), section);
			LineAlignmentOffset = section.Offset;
			section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(StringFormatFlags)), section);
			FormatFlagsOffset = section.Offset;
			section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(HotkeyPrefix)), section);
			HotkeyPrefixOffset = section.Offset;
			section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(StringTrimming)), section);
			TrimmingOffset = section.Offset;
			section = BitVector32.CreateSection(ReportSpecificEnumHelper.GetEnumMaxValue(typeof(BrickStringFormatPrototypeKind)), section);
			PrototypeKindOffset = section.Offset;
		}
		public static BrickStringFormat Create(TextAlignment textAlignment, bool wordWrap) {
			return Create(textAlignment, wordWrap, StringTrimming.Character);
		}
		public static BrickStringFormat Create(TextAlignment textAlignment, bool wordWrap, StringTrimming trimming) {
			return Create(textAlignment, wordWrap, trimming, false);
		}
		public static BrickStringFormat Create(TextAlignment textAlignment, bool wordWrap, StringTrimming trimming, bool rightToLeft) {
			TextAlignment actualTextAlignment = textAlignment;
			StringFormatFlags formatFlags = StringFormatFlags.NoClip | StringFormatFlags.LineLimit | StringFormatFlags.FitBlackBox;
			if(!wordWrap)
				formatFlags |= StringFormatFlags.NoWrap;
			if(rightToLeft) {
				formatFlags |= StringFormatFlags.DirectionRightToLeft;
				actualTextAlignment = GraphicsConvertHelper.RTLTextAlignment(actualTextAlignment);
			}
			return Create(actualTextAlignment, formatFlags, trimming);
		}
		public static BrickStringFormat Create(TextAlignment textAlignment, StringFormatFlags formatFlags, StringTrimming trimming) {
			BrickStringFormat brickStringFormat = new BrickStringFormat(formatFlags, GraphicsConvertHelper.ToHorzStringAlignment(textAlignment), GraphicsConvertHelper.ToVertStringAlignment(textAlignment), trimming);
			brickStringFormat.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			return brickStringFormat;
		}
#endregion
#region fields & properties
		StringFormat value;
		StringAlignment alignment = DefaultAlignment;
		StringAlignment lineAlignment = DefaultLineAlignment;
		StringFormatFlags formatFlags = DefaultFormatFlags;
		HotkeyPrefix hotkeyPrefix = DefaultHotkeyPrefix;
		StringTrimming trimming = DefaultTrimming;
		BrickStringFormatPrototypeKind prototypeKind = BrickStringFormatPrototypeKind.Default;
		float[] tabStops = null;
		[Browsable(false)]
		public bool WordWrap {
			get { return (FormatFlags & StringFormatFlags.NoWrap) == 0; }
		}
		[Browsable(false)]
		public bool RightToLeft {
			get { return (FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0; }
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatAlignment")]
#endif
		public StringAlignment Alignment {
			get {
				if(value == null) return alignment;
				return value.Alignment;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatLineAlignment")]
#endif
		public StringAlignment LineAlignment {
			get {
				if(value == null) return lineAlignment;
				return value.LineAlignment;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatFormatFlags")]
#endif
		public StringFormatFlags FormatFlags {
			get {
				if(value == null) return formatFlags;
				return value.FormatFlags;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatHotkeyPrefix")]
#endif
		public HotkeyPrefix HotkeyPrefix {
			get {
				if(value == null) return hotkeyPrefix;
				return value.HotkeyPrefix;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatTrimming")]
#endif
		public StringTrimming Trimming {
			get {
				if(value == null) return trimming;
				return value.Trimming;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatValue")]
#endif
		public StringFormat Value {
			get {
				if(value == null) {
					value = new StringFormat(prototypes[(int)prototypeKind]);
					value.FormatFlags = formatFlags;
					value.Alignment = alignment;
					value.LineAlignment = lineAlignment;
					value.HotkeyPrefix = hotkeyPrefix;
					value.Trimming = trimming;
				}
				return value;
			}
		}
#if !SL
	[DevExpressDataLocalizedDescription("BrickStringFormatPrototypeKind")]
#endif
		public BrickStringFormatPrototypeKind PrototypeKind {
			get { return prototypeKind; }
			set {
				CheckNullValue();
				prototypeKind = value;
			}
		}
#endregion
#region ctors
		public BrickStringFormat() {
		}
		public BrickStringFormat(StringAlignment alignment, StringAlignment lineAlignment, StringFormatFlags formatFlags, HotkeyPrefix hotkeyPrefix, StringTrimming trimming) {
			this.alignment = alignment;
			this.lineAlignment = lineAlignment;
			this.formatFlags = formatFlags;
			this.hotkeyPrefix = hotkeyPrefix;
			this.trimming = trimming;
		}
		public BrickStringFormat(BrickStringFormat source)
			: this(source.Alignment, source.LineAlignment, source.FormatFlags, source.HotkeyPrefix, source.Trimming) {
			PrototypeKind = source.PrototypeKind;
		}
		public BrickStringFormat(StringFormat source)
			: this(source.Alignment, source.LineAlignment, source.FormatFlags, source.HotkeyPrefix, source.Trimming) {
		}
		public BrickStringFormat(BrickStringFormat source, StringTrimming trimming)
			: this(source.Alignment, source.LineAlignment, source.FormatFlags, source.HotkeyPrefix, trimming) {
			PrototypeKind = source.PrototypeKind;
		}
		public BrickStringFormat(BrickStringFormat source, StringAlignment alignment, StringAlignment lineAlignment)
			: this(alignment, lineAlignment, source.FormatFlags, source.HotkeyPrefix, source.Trimming) {
			PrototypeKind = source.PrototypeKind;
		}
		public BrickStringFormat(BrickStringFormat source, StringFormatFlags options)
			: this(source.Alignment, source.LineAlignment, options, source.HotkeyPrefix, source.Trimming) {
			PrototypeKind = source.PrototypeKind;
		}
		public BrickStringFormat(StringAlignment alignment, StringAlignment lineAlignment)
			: this(alignment, lineAlignment, DefaultFormatFlags, DefaultHotkeyPrefix, DefaultTrimming) {
		}
		public BrickStringFormat(StringFormatFlags options)
			: this(DefaultAlignment, DefaultLineAlignment, options, DefaultHotkeyPrefix, DefaultTrimming) {
		}
		public BrickStringFormat(StringAlignment alignment)
			: this(alignment, DefaultLineAlignment, DefaultFormatFlags, DefaultHotkeyPrefix, DefaultTrimming) {
		}
		public BrickStringFormat(StringFormatFlags options, StringAlignment alignment, StringAlignment lineAlignment)
			: this(alignment, lineAlignment, options, DefaultHotkeyPrefix, DefaultTrimming) {
		}
		public BrickStringFormat(StringFormatFlags options, StringAlignment alignment, StringAlignment lineAlignment, StringTrimming trimming)
			: this(alignment, lineAlignment, options, DefaultHotkeyPrefix, trimming) {
		}
#endregion
		void CheckNullValue() {
			if(this.value != null)
				throw new InvalidOperationException();
		}
		public virtual object Clone() {
			return new BrickStringFormat( this );
		}
		public virtual void Dispose() {
			if (value != null) {
				value.Dispose();
				value = null;
			}
			GC.SuppressFinalize(this);
		}
		public override int GetHashCode() {
			return ((int)alignment << AlignmentOffset) |
				((int)lineAlignment << LineAlignmentOffset) |
				((int)formatFlags << FormatFlagsOffset) |
				((int)hotkeyPrefix << HotkeyPrefixOffset) |
				((int)trimming << TrimmingOffset) |
				((int)prototypeKind << PrototypeKindOffset);
		}
		public override bool Equals(object obj) {
			if (obj is DevExpress.XtraPrinting.BrickStringFormat) {
				DevExpress.XtraPrinting.BrickStringFormat sf = (DevExpress.XtraPrinting.BrickStringFormat)obj;
				return Alignment == sf.Alignment && LineAlignment == sf.LineAlignment &&
					FormatFlags == sf.FormatFlags && HotkeyPrefix == sf.HotkeyPrefix &&
					Trimming == sf.Trimming && prototypeKind == sf.PrototypeKind;
			}
			return false;
		}
		public BrickStringFormat ChangeAlignment(StringAlignment newAlignment) {
			return new BrickStringFormat(this, newAlignment, LineAlignment);
		}
		public BrickStringFormat ChangeAlignment(StringAlignment newAlignment, StringAlignment newLineAlignment) {
			return new BrickStringFormat(this, newAlignment, newLineAlignment);
		}
		public BrickStringFormat ChangeLineAlignment(StringAlignment newLineAlignment) {
			return new BrickStringFormat(this, Alignment, newLineAlignment);
		}
		public BrickStringFormat ChangeFormatFlags(StringFormatFlags options) {
			return new BrickStringFormat(this, options);
		}
		internal void ClearTabStops() {
			SetTabStops(new float[] { });
		}
		internal void SetTabStops(float[] tabStops) {
			if(this.tabStops == null || !DevExpress.Utils.ArrayHelper.ArraysEqual<float>(this.tabStops, tabStops)) {
				this.tabStops = tabStops;
				Value.SetTabStops(0, tabStops);
			}
		}
	}
}
