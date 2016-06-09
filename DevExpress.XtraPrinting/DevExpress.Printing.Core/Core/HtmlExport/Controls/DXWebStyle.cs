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
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public class DXWebStyle {
		static readonly string[] borderStyles = new string[] { "NotSet", "None", "Dotted", "Dashed", "Solid", "Double", "Groove", "Ridge", "Inset", "Outset" };
		static string FormatStringArray(string[] array, char delimiter) {
			switch(array.Length) {
				case 1:
					return array[0];
				case 0:
					return string.Empty;
			}
			return string.Join(new String(delimiter, 1), array);
		}
		const string CssClassStr = "class";
		DXWebFontInfo fontInfo;
		bool marked;
		int markedBits;
		bool ownStateBag;
		string registeredCssClass;
		int setBits;
		DXStateBag statebag;
		public Color BackColor {
			get {
				if(IsSet(8))
					return (Color)ViewState["BackColor"];
				return DXColor.Empty;
			}
			set {
				ViewState["BackColor"] = value;
				SetBit(8);
			}
		}
		public Color BorderColor {
			get {
				if(IsSet(0x10))
					return (Color)ViewState["BorderColor"];
				return DXColor.Empty;
			}
			set {
				ViewState["BorderColor"] = value;
				SetBit(0x10);
			}
		}
		public DXWebBorderStyle BorderStyle {
			get {
				if(IsSet(0x40))
					return (DXWebBorderStyle)ViewState["BorderStyle"];
				return DXWebBorderStyle.NotSet;
			}
			set {
				if(value < DXWebBorderStyle.NotSet || value > DXWebBorderStyle.Outset)
					throw new ArgumentOutOfRangeException("value");
				ViewState["BorderStyle"] = value;
				SetBit(0x40);
			}
		}
		public DXWebUnit BorderWidth {
			get {
				if(IsSet(0x20))
					return (DXWebUnit)ViewState["BorderWidth"];
				return DXWebUnit.Empty;
			}
			set {
				if(value.Type == DXWebUnitType.Percentage || value.Value < 0.0)
					throw new ArgumentOutOfRangeException("value", "Style_InvalidBorderWidth");
				ViewState["BorderWidth"] = value;
				SetBit(0x20);
			}
		}
		public string CssClass {
			get {
				if(IsSet(2)) {
					string str = (string)ViewState[CssClassStr];
					if(str != null)
						return str;
				}
				return string.Empty;
			}
			set {
				ViewState[CssClassStr] = value;
				SetBit(2);
			}
		}
		public DXWebFontInfo Font {
			get {
				if(fontInfo == null)
					fontInfo = new DXWebFontInfo(this);
				return fontInfo;
			}
		}
		public Color ForeColor {
			get {
				if(IsSet(4))
					return (Color)ViewState["ForeColor"];
				return DXColor.Empty;
			}
			set {
				ViewState["ForeColor"] = value;
				SetBit(4);
			}
		}
		public DXWebUnit Height {
			get {
				if(IsSet(0x80))
					return (DXWebUnit)ViewState["Height"];
				return DXWebUnit.Empty;
			}
			set {
				if(value.Value < 0.0)
					throw new ArgumentOutOfRangeException("value");
				ViewState["Height"] = value;
				SetBit(0x80);
			}
		}
		public virtual bool IsEmpty {
			get { return setBits == 0 && RegisteredCssClass.Length == 0; }
		}
		protected bool IsTrackingViewState {
			get { return marked; }
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public string RegisteredCssClass {
			get {
				if(registeredCssClass == null)
					return string.Empty;
				return registeredCssClass;
			}
		}
		public DXWebUnit Width {
			get {
				if(IsSet(0x100))
					return (DXWebUnit)ViewState["Width"];
				return DXWebUnit.Empty;
			}
			set {
				if(value.Value < 0.0)
					throw new ArgumentOutOfRangeException("value");
				ViewState["Width"] = value;
				SetBit(0x100);
			}
		}
		protected internal DXStateBag ViewState {
			get {
				if(statebag == null) {
					statebag = new DXStateBag();
					if(IsTrackingViewState)
						statebag.TrackViewState();
				}
				return statebag;
			}
		}
		public DXWebStyle()
			: this(null) {
			ownStateBag = true;
		}
		public DXWebStyle(DXStateBag bag) {
			statebag = bag;
			marked = false;
			setBits = 0;
		}
		public void AddAttributesToRender(DXHtmlTextWriter writer) {
			AddAttributesToRender(writer, null);
		}
		public virtual void AddAttributesToRender(DXHtmlTextWriter writer, DXHtmlControl owner) {
			string registeredCssClass = string.Empty;
			bool flag = true;
			if(IsSet(2)) {
				registeredCssClass = (string)ViewState[CssClassStr];
				if(registeredCssClass == null)
					registeredCssClass = string.Empty;
			}
			if(!string.IsNullOrEmpty(this.registeredCssClass)) {
				flag = false;
				if(registeredCssClass.Length != 0)
					registeredCssClass = string.Format("{0} {1}", registeredCssClass, this.registeredCssClass);
				else
					registeredCssClass = this.registeredCssClass;
			}
			if(registeredCssClass.Length > 0)
				writer.AddAttribute(DXHtmlTextWriterAttribute.Class, registeredCssClass);
			if(flag)
				GetStyleAttributes().Render(writer);
		}
		public virtual void CopyFrom(DXWebStyle s) {
			if(RegisteredCssClass.Length != 0)
				throw new InvalidOperationException("Style_RegisteredStylesAreReadOnly");
			if(s != null && !s.IsEmpty) {
				this.Font.CopyFrom(s.Font);
				if(s.IsSet(2))
					CssClass = s.CssClass;
				if(s.RegisteredCssClass.Length != 0) {
					if(IsSet(2))
						CssClass = string.Format("{0} {1}", CssClass, s.RegisteredCssClass);
					else
						CssClass = s.RegisteredCssClass;
					if(s.IsSet(8) && s.BackColor != DXColor.Empty) {
						ViewState.Remove("BackColor");
						ClearBit(8);
					}
					if(s.IsSet(4) && s.ForeColor != DXColor.Empty) {
						ViewState.Remove("ForeColor");
						ClearBit(4);
					}
					if(s.IsSet(0x10) && s.BorderColor != DXColor.Empty) {
						ViewState.Remove("BorderColor");
						ClearBit(0x10);
					}
					if(s.IsSet(0x20) && s.BorderWidth != DXWebUnit.Empty) {
						ViewState.Remove("BorderWidth");
						ClearBit(0x20);
					}
					if(s.IsSet(0x40)) {
						ViewState.Remove("BorderStyle");
						ClearBit(0x40);
					}
					if(s.IsSet(0x80) && s.Height != DXWebUnit.Empty) {
						ViewState.Remove("Height");
						ClearBit(0x80);
					}
					if(s.IsSet(0x100) && s.Width != DXWebUnit.Empty) {
						ViewState.Remove("Width");
						ClearBit(0x100);
					}
				} else {
					if(s.IsSet(8) && s.BackColor != DXColor.Empty)
						BackColor = s.BackColor;
					if(s.IsSet(4) && s.ForeColor != DXColor.Empty)
						ForeColor = s.ForeColor;
					if(s.IsSet(0x10) && s.BorderColor != DXColor.Empty)
						BorderColor = s.BorderColor;
					if(s.IsSet(0x20) && s.BorderWidth != DXWebUnit.Empty)
						BorderWidth = s.BorderWidth;
					if(s.IsSet(0x40))
						BorderStyle = s.BorderStyle;
					if(s.IsSet(0x80) && s.Height != DXWebUnit.Empty)
						Height = s.Height;
					if(s.IsSet(0x100) && s.Width != DXWebUnit.Empty)
						Width = s.Width;
				}
			}
		}
		public DXCssStyleCollection GetStyleAttributes() {
			DXCssStyleCollection attributes = new DXCssStyleCollection();
			FillStyleAttributes(attributes);
			return attributes;
		}
		public virtual void MergeWith(DXWebStyle s) {
			if(RegisteredCssClass.Length != 0)
				throw new InvalidOperationException("Style_RegisteredStylesAreReadOnly");
			if(s != null && !s.IsEmpty) {
				if(IsEmpty)
					CopyFrom(s);
				else {
					Font.MergeWith(s.Font);
					if(s.IsSet(2) && !IsSet(2))
						CssClass = s.CssClass;
					if(s.RegisteredCssClass.Length == 0) {
						if(s.IsSet(8) && (!IsSet(8) || BackColor == DXColor.Empty))
							BackColor = s.BackColor;
						if(s.IsSet(4) && (!IsSet(4) || ForeColor == DXColor.Empty))
							ForeColor = s.ForeColor;
						if(s.IsSet(0x10) && (!IsSet(0x10) || BorderColor == DXColor.Empty))
							BorderColor = s.BorderColor;
						if(s.IsSet(0x20) && (!IsSet(0x20) || BorderWidth == DXWebUnit.Empty))
							BorderWidth = s.BorderWidth;
						if(s.IsSet(0x40) && !IsSet(0x40))
							BorderStyle = s.BorderStyle;
						if(s.IsSet(0x80) && (!IsSet(0x80) || Height == DXWebUnit.Empty))
							Height = s.Height;
						if(s.IsSet(0x100) && (!IsSet(0x100) || Width == DXWebUnit.Empty))
							Width = s.Width;
					} else if(IsSet(2))
						CssClass = string.Format("{0} {1}", CssClass, s.RegisteredCssClass);
					else
						CssClass = s.RegisteredCssClass;
				}
			}
		}
		public virtual void Reset() {
			if(statebag != null) {
				if(IsSet(2))
					ViewState.Remove(CssClassStr);
				if(IsSet(8))
					ViewState.Remove("BackColor");
				if(IsSet(4))
					ViewState.Remove("ForeColor");
				if(IsSet(0x10))
					ViewState.Remove("BorderColor");
				if(IsSet(0x20))
					ViewState.Remove("BorderWidth");
				if(IsSet(0x40))
					ViewState.Remove("BorderStyle");
				if(IsSet(0x80))
					ViewState.Remove("Height");
				if(IsSet(0x100))
					ViewState.Remove("Width");
				Font.Reset();
				ViewState.Remove("_!SB");
				markedBits = 0;
			}
			setBits = 0;
		}
		public void SetDirty() {
			ViewState.SetDirty(true);
			markedBits = setBits;
		}
		internal void SetRegisteredCssClass(string cssClass) {
			registeredCssClass = cssClass;
		}
		internal bool IsSet(int propKey) {
			return (setBits & propKey) != 0;
		}
		internal void ClearBit(int bit) {
			setBits &= ~bit;
		}
		protected internal virtual void TrackViewState() {
			if(ownStateBag)
				ViewState.TrackViewState();
			marked = true;
		}
		protected virtual void FillStyleAttributes(DXCssStyleCollection attributes) {
			Color color;
			DXWebUnit unit3;
			if(IsSet(4)) {
				color = (Color)ViewState["ForeColor"];
				if(!DXColor.IsEmpty(color))
					attributes.Add(DXHtmlTextWriterStyle.Color, DXColor.ToHtml(color));
			}
			if(IsSet(8)) {
				color = (Color)ViewState["BackColor"];
				if(!DXColor.IsEmpty(color))
					attributes.Add(DXHtmlTextWriterStyle.BackgroundColor, DXColor.ToHtml(color));
			}
			if(IsSet(0x10)) {
				color = (Color)ViewState["BorderColor"];
				if(!DXColor.IsEmpty(color))
					attributes.Add(DXHtmlTextWriterStyle.BorderColor, DXColor.ToHtml(color));
			}
			DXWebBorderStyle borderStyle = BorderStyle;
			DXWebUnit borderWidth = BorderWidth;
			if(!borderWidth.IsEmpty) {
				attributes.Add(DXHtmlTextWriterStyle.BorderWidth, borderWidth.ToString(CultureInfo.InvariantCulture));
				if(borderStyle == DXWebBorderStyle.NotSet) {
					if(borderWidth.Value != 0.0)
						attributes.Add(DXHtmlTextWriterStyle.BorderStyle, "solid");
				} else
					attributes.Add(DXHtmlTextWriterStyle.BorderStyle, borderStyles[(int)borderStyle]);
			} else if(borderStyle != DXWebBorderStyle.NotSet)
				attributes.Add(DXHtmlTextWriterStyle.BorderStyle, borderStyles[(int)borderStyle]);
			DXWebFontInfo font = Font;
			string[] names = font.Names;
			if(names.Length > 0)
				attributes.Add(DXHtmlTextWriterStyle.FontFamily, FormatStringArray(names, ','));
			DXWebFontUnit size = font.Size;
			if(!size.IsEmpty)
				attributes.Add(DXHtmlTextWriterStyle.FontSize, size.ToString(CultureInfo.InvariantCulture));
			if(IsSet(0x800)) {
				if(font.Bold)
					attributes.Add(DXHtmlTextWriterStyle.FontWeight, "bold");
				else
					attributes.Add(DXHtmlTextWriterStyle.FontWeight, "normal");
			}
			if(IsSet(0x1000)) {
				if(font.Italic)
					attributes.Add(DXHtmlTextWriterStyle.FontStyle, "italic");
				else
					attributes.Add(DXHtmlTextWriterStyle.FontStyle, "normal");
			}
			string str = string.Empty;
			if(font.Underline)
				str = "underline";
			if(font.Overline)
				str += " overline";
			if(font.Strikeout)
				str += " line-through";
			if(str.Length > 0)
				attributes.Add(DXHtmlTextWriterStyle.TextDecoration, str);
			else if(IsSet(0x2000) || IsSet(0x4000) || IsSet(0x8000))
				attributes.Add(DXHtmlTextWriterStyle.TextDecoration, "none");
			if(IsSet(0x80)) {
				unit3 = (DXWebUnit)ViewState["Height"];
				if(!unit3.IsEmpty)
					attributes.Add(DXHtmlTextWriterStyle.Height, unit3.ToString(CultureInfo.InvariantCulture));
			}
			if(IsSet(0x100)) {
				unit3 = (DXWebUnit)ViewState["Width"];
				if(!unit3.IsEmpty)
					attributes.Add(DXHtmlTextWriterStyle.Width, unit3.ToString(CultureInfo.InvariantCulture));
			}
		}
		protected internal virtual void SetBit(int bit) {
			setBits |= bit;
			if(IsTrackingViewState)
				markedBits |= bit;
		}
	}
}
