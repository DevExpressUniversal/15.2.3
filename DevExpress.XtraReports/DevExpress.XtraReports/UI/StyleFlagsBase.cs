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
using System.Text;
using DevExpress.XtraPrinting.Native;
using System.ComponentModel;
using DevExpress.XtraPrinting;
using System.Collections.Specialized;
namespace DevExpress.XtraReports.UI {
	[
	TypeConverter(typeof(LocalizableObjectConverter)),
	]
	public abstract class StyleFlagsBase : ICloneable {
		const int bitFont = (int)StyleProperty.Font;
		static readonly int bitForeColor = (int)StyleProperty.ForeColor;
		static readonly int bitBackColor = (int)StyleProperty.BackColor;
		static readonly int bitBorderColor = (int)StyleProperty.BorderColor;
		static readonly int bitBorders = (int)StyleProperty.Borders;
		static readonly int bitBorderWidth = (int)StyleProperty.BorderWidth;
		static readonly int bitBorderDashStyle = (int)StyleProperty.BorderDashStyle;
		static readonly int bitTextAlignment = (int)StyleProperty.TextAlignment;
		static readonly int bitPadding = (int)StyleProperty.Padding;
		BitVector32 flags = new BitVector32();
		internal StyleProperty SetProperties { get { return (StyleProperty)flags.Data; } }
		protected bool UseFontCore {
			get { return flags[bitFont]; }
			set { flags[bitFont] = value; }
		}
		protected bool UseForeColorCore {
			get { return flags[bitForeColor]; }
			set { flags[bitForeColor] = value; }
		}
		protected bool UseBackColorCore {
			get { return flags[bitBackColor]; }
			set { flags[bitBackColor] = value; }
		}
		protected bool UseBorderColorCore {
			get { return flags[bitBorderColor]; }
			set { flags[bitBorderColor] = value; }
		}
		protected bool UseBordersCore {
			get { return flags[bitBorders]; }
			set { flags[bitBorders] = value; }
		}
		protected bool UseBorderWidthCore {
			get { return flags[bitBorderWidth]; }
			set { flags[bitBorderWidth] = value; }
		}
		protected bool UseBorderDashStyleCore {
			get { return flags[bitBorderDashStyle]; }
			set { flags[bitBorderDashStyle] = value; }
		}
		protected bool UseTextAlignmentCore {
			get { return flags[bitTextAlignment]; }
			set { flags[bitTextAlignment] = value; }
		}
		protected bool UsePaddingCore {
			get { return flags[bitPadding]; }
			set { flags[bitPadding] = value; }
		}
		internal bool IsSet(StyleProperty property) {
			return (flags.Data & (int)property) > 0;
		}
		internal void Set(StyleProperty property, bool value) {
			flags[(int)property] = value;
		}
		public override bool Equals(object obj) {
			StyleFlagsBase styleUsing = obj as StyleFlagsBase;
			return styleUsing != null && flags.Equals(styleUsing.flags);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public void Assign(StyleFlagsBase styleUsing) {
			flags = styleUsing.flags;
		}
		public object Clone() {
			StyleFlagsBase result = CreateInstance();
			result.Assign(this);
			return result;
		}
		protected abstract StyleFlagsBase CreateInstance();
	}
}
