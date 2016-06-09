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
using DevExpress.Compatibility.System;
namespace DevExpress.Pdf {
	public class PdfStringFormat : ICloneable {
		static readonly PdfStringFormat genericDefault = new PdfStringFormat();
		static readonly PdfStringFormat genericTypographic;
		public static PdfStringFormat GenericDefault { get { return new PdfStringFormat(genericDefault); } }
		public static PdfStringFormat GenericTypographic { get { return new PdfStringFormat(genericTypographic); } }
		static PdfStringFormat() {
			genericTypographic = new PdfStringFormat(PdfStringFormatFlags.LineLimit | PdfStringFormatFlags.NoClip);
			genericTypographic.trimming = PdfStringTrimming.None;
			genericTypographic.leadingMarginFactor = 0;
			genericTypographic.trailingMarginFactor = 0;
		}
		double leadingMarginFactor = 1.0 / 6.0;
		double trailingMarginFactor = 1.0 / 4.0;
		PdfStringFormatFlags formatFlags;
		PdfStringAlignment alignment = PdfStringAlignment.Near;
		PdfStringAlignment lineAlignment = PdfStringAlignment.Near;
		PdfStringTrimming trimming = PdfStringTrimming.Character;
		PdfHotkeyPrefix hotkeyPrefix = PdfHotkeyPrefix.None;
		public PdfStringFormatFlags FormatFlags {
			get { return formatFlags; }
			set { formatFlags = value; }
		}
		public PdfStringAlignment Alignment {
			get { return alignment; }
			set { alignment = value; }
		}
		public PdfStringAlignment LineAlignment {
			get { return lineAlignment; }
			set { lineAlignment = value; }
		}
		public PdfStringTrimming Trimming {
			get { return trimming; }
			set { trimming = value; }
		}
		public PdfHotkeyPrefix HotkeyPrefix {
			get { return hotkeyPrefix; }
			set { hotkeyPrefix = value; }
		}
		public double LeadingMarginFactor {
			get { return leadingMarginFactor; }
			set { leadingMarginFactor = value; }
		}
		public double TrailingMarginFactor {
			get { return trailingMarginFactor; }
			set { trailingMarginFactor = value; }
		}
		public PdfStringFormat() {
		}
		public PdfStringFormat(PdfStringFormatFlags formatFlags) {
			this.formatFlags = formatFlags;
		}
		public PdfStringFormat(PdfStringFormat format) {
			formatFlags = format.formatFlags;
			alignment = format.alignment;
			lineAlignment = format.lineAlignment;
			trimming = format.trimming;
			hotkeyPrefix = format.hotkeyPrefix;
			leadingMarginFactor = format.leadingMarginFactor;
			trailingMarginFactor = format.trailingMarginFactor;
		}
		public object Clone() {
			return new PdfStringFormat(this);
		}
	}
}
