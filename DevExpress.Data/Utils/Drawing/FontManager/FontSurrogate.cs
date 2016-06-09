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
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.Utils.Internal {
	public class FontSurrogate {
		public static FontSurrogate FromFont(Font font) {
			return font != null ? new FontSurrogate() { Name = font.Name, Size = font.Size, Style = font.Style } :
				new FontSurrogate() { Name = string.Empty, Size = 0f, Style = FontStyle.Regular };
		}
		public static Font ToFont(FontSurrogate surrogate) {
			return new Font(surrogate.Name, surrogate.Size, surrogate.Style);
		}
		#region fields & properties
		public float Size { get; set; }
		public string Name { get; set; }
		public FontStyle Style { get; set; }
		public bool Regular {
			get { return Style == System.Drawing.FontStyle.Regular; }
			set { Style = System.Drawing.FontStyle.Regular; }
		}
		public bool Bold {
			get { return ExistsStyle(System.Drawing.FontStyle.Bold); }
			set {
				if(value)
					SetStyle(System.Drawing.FontStyle.Bold);
				else
					ClearStyle(System.Drawing.FontStyle.Bold);
			}
		}
		public bool Italic {
			get { return ExistsStyle(System.Drawing.FontStyle.Italic); }
			set {
				if(value)
					SetStyle(System.Drawing.FontStyle.Italic);
				else
					ClearStyle(System.Drawing.FontStyle.Italic);
			}
		}
		public bool Underline {
			get { return ExistsStyle(System.Drawing.FontStyle.Underline); }
			set {
				if(value)
					SetStyle(System.Drawing.FontStyle.Underline);
				else
					ClearStyle(System.Drawing.FontStyle.Underline);
			}
		}
		public bool Strikeout {
			get { return ExistsStyle(System.Drawing.FontStyle.Strikeout); }
			set {
				if(value)
					SetStyle(System.Drawing.FontStyle.Strikeout);
				else
					ClearStyle(System.Drawing.FontStyle.Strikeout);
			}
		}
		public bool IsEmpty {
			get { return Name == string.Empty && Size == 0f && Style == FontStyle.Regular; }
		}
		#endregion
		bool ExistsStyle(System.Drawing.FontStyle flag) {
			return (Style & flag) > 0;
		}
		void SetStyle(System.Drawing.FontStyle flag) {
			Style |= flag;
		}
		void ClearStyle(System.Drawing.FontStyle flag) {
			Style &= ~flag;
		}
		public override bool Equals(object obj) {
			FontSurrogate surrogate = obj as FontSurrogate;
			if(surrogate != null)
				return Name == surrogate.Name && Size == surrogate.Size && Style == surrogate.Style;
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
