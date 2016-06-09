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
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public sealed class DXWebFontInfo {
		DXWebStyle owner;
		internal DXWebFontInfo(DXWebStyle owner) {
			this.owner = owner;
		}
		public void ClearDefaults() {
			if(Names.Length == 0) {
				owner.ViewState.Remove("Font_Names");
				owner.ClearBit(0x200);
			}
			if(Size == DXWebFontUnit.Empty) {
				owner.ViewState.Remove("Font_Size");
				owner.ClearBit(0x400);
			}
			if(!Bold)
				ResetBold();
			if(!Italic)
				ResetItalic();
			if(!Underline)
				ResetUnderline();
			if(!Overline)
				ResetOverline();
			if(!Strikeout)
				ResetStrikeout();
		}
		public void CopyFrom(DXWebFontInfo f) {
			if(f != null) {
				DXWebStyle owner = f.Owner;
				if(owner.RegisteredCssClass.Length != 0) {
					if(owner.IsSet(0x200))
						ResetNames();
					if(owner.IsSet(0x400) && (f.Size != DXWebFontUnit.Empty))
						ResetFontSize();
					if(owner.IsSet(0x800))
						ResetBold();
					if(owner.IsSet(0x1000))
						ResetItalic();
					if(owner.IsSet(0x4000))
						ResetOverline();
					if(owner.IsSet(0x8000))
						ResetStrikeout();
					if(owner.IsSet(0x2000))
						ResetUnderline();
				} else {
					if(owner.IsSet(0x200))
						Names = f.Names;
					if(owner.IsSet(0x400) && f.Size != DXWebFontUnit.Empty)
						Size = f.Size;
					if(owner.IsSet(0x800))
						Bold = f.Bold;
					if(owner.IsSet(0x1000))
						Italic = f.Italic;
					if(owner.IsSet(0x4000))
						Overline = f.Overline;
					if(owner.IsSet(0x8000))
						Strikeout = f.Strikeout;
					if(owner.IsSet(0x2000))
						Underline = f.Underline;
				}
			}
		}
		public void MergeWith(DXWebFontInfo f) {
			if(f != null) {
				DXWebStyle owner = f.Owner;
				if(owner.RegisteredCssClass.Length == 0) {
					if(owner.IsSet(0x200) && !this.owner.IsSet(0x200))
						Names = f.Names;
					if(owner.IsSet(0x400) && (!this.owner.IsSet(0x400) || Size == DXWebFontUnit.Empty))
						Size = f.Size;
					if(owner.IsSet(0x800) && !this.owner.IsSet(0x800))
						Bold = f.Bold;
					if(owner.IsSet(0x1000) && !this.owner.IsSet(0x1000))
						Italic = f.Italic;
					if(owner.IsSet(0x4000) && !this.owner.IsSet(0x4000))
						Overline = f.Overline;
					if(owner.IsSet(0x8000) && !this.owner.IsSet(0x8000))
						Strikeout = f.Strikeout;
					if(owner.IsSet(0x2000) && !this.owner.IsSet(0x2000))
						Underline = f.Underline;
				}
			}
		}
		internal void Reset() {
			if(owner.IsSet(0x200))
				ResetNames();
			if(owner.IsSet(0x400))
				ResetFontSize();
			if(owner.IsSet(0x800))
				ResetBold();
			if(owner.IsSet(0x1000))
				ResetItalic();
			if(owner.IsSet(0x2000))
				ResetUnderline();
			if(owner.IsSet(0x4000))
				ResetOverline();
			if(owner.IsSet(0x8000))
				ResetStrikeout();
		}
		private void ResetBold() {
			owner.ViewState.Remove("Font_Bold");
			owner.ClearBit(0x800);
		}
		private void ResetFontSize() {
			owner.ViewState.Remove("Font_Size");
			owner.ClearBit(0x400);
		}
		private void ResetItalic() {
			owner.ViewState.Remove("Font_Italic");
			owner.ClearBit(0x1000);
		}
		private void ResetNames() {
			owner.ViewState.Remove("Font_Names");
			owner.ClearBit(0x200);
		}
		private void ResetOverline() {
			owner.ViewState.Remove("Font_Overline");
			owner.ClearBit(0x4000);
		}
		private void ResetStrikeout() {
			owner.ViewState.Remove("Font_Strikeout");
			owner.ClearBit(0x8000);
		}
		private void ResetUnderline() {
			owner.ViewState.Remove("Font_Underline");
			owner.ClearBit(0x2000);
		}
		private bool ShouldSerializeBold() {
			return owner.IsSet(0x800);
		}
		private bool ShouldSerializeItalic() {
			return owner.IsSet(0x1000);
		}
		public bool ShouldSerializeNames() {
			return Names.Length > 0;
		}
		bool ShouldSerializeOverline() {
			return owner.IsSet(0x4000);
		}
		bool ShouldSerializeStrikeout() {
			return owner.IsSet(0x8000);
		}
		bool ShouldSerializeUnderline() {
			return owner.IsSet(0x2000);
		}
		public override string ToString() {
			string str = Size.ToString(CultureInfo.InvariantCulture);
			string name = Name;
			if(str.Length == 0)
				return name;
			if(name.Length != 0)
				return string.Format("{0}, {1}", name, str);
			return str;
		}
		public bool Bold {
			get { return owner.IsSet(0x800) && (bool)owner.ViewState["Font_Bold"]; }
			set {
				owner.ViewState["Font_Bold"] = value;
				owner.SetBit(0x800);
			}
		}
		public bool Italic {
			get { return owner.IsSet(0x1000) && (bool)owner.ViewState["Font_Italic"]; }
			set {
				owner.ViewState["Font_Italic"] = value;
				owner.SetBit(0x1000);
			}
		}
		public string Name {
			get {
				string[] names = Names;
				if(names.Length > 0)
					return names[0];
				return string.Empty;
			}
			set {
				Guard.ArgumentNotNull(value, "value");
				if(value.Length == 0)
					Names = null;
				else
					Names = new string[] { value };
			}
		}
		public string[] Names {
			get {
				if(owner.IsSet(0x200)) {
					string[] strArray = owner.ViewState["Font_Names"] as string[];
					if(strArray != null)
						return strArray;
				}
				return new string[0];
			}
			set {
				owner.ViewState["Font_Names"] = value;
				owner.SetBit(0x200);
			}
		}
		public bool Overline {
			get { return owner.IsSet(0x4000) && (bool)owner.ViewState["Font_Overline"]; }
			set {
				owner.ViewState["Font_Overline"] = value;
				owner.SetBit(0x4000);
			}
		}
		internal DXWebStyle Owner {
			get { return owner; }
		}
		public DXWebFontUnit Size {
			get {
				if(owner.IsSet(0x400))
					return (DXWebFontUnit)owner.ViewState["Font_Size"];
				return DXWebFontUnit.Empty;
			}
			set {
				if(value.Type == DXWebFontSize.AsUnit && value.Unit.Value < 0.0)
					throw new ArgumentOutOfRangeException("value");
				owner.ViewState["Font_Size"] = value;
				owner.SetBit(0x400);
			}
		}
		public bool Strikeout {
			get { return owner.IsSet(0x8000) && (bool)owner.ViewState["Font_Strikeout"]; }
			set {
				owner.ViewState["Font_Strikeout"] = value;
				owner.SetBit(0x8000);
			}
		}
		public bool Underline {
			get { return owner.IsSet(0x2000) && (bool)owner.ViewState["Font_Underline"]; }
			set {
				owner.ViewState["Font_Underline"] = value;
				owner.SetBit(0x2000);
			}
		}
	}
}
