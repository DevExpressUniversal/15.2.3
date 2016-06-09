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

using System.Windows;
using System.Windows.Media;
using DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing.BrickCollection {
	class BrickStyleKey {
		public Color BackColor { get; set; }
		public Color ForeColor { get; set; }
		public Color BorderColor { get; set; }
		public Thickness BorderThickness { get; set; }
		public BorderDashStyle BorderDashStyle { get; set; }
		public FontFamily FontFamily { get; set; }
		public FontStyle FontStyle { get; set; }
		public FontWeight FontWeight { get; set; }
		public double FontSize { get; set; }
		public Thickness Padding { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public TextWrapping TextWrapping { get; set; }
		public BrickTextDecorations TextDecorations { get; set; }
		public TextTrimming TextTrimming { get; set; }
		public override bool Equals(object obj) {
			BrickStyleKey key = obj as BrickStyleKey;
			if(key != null)
				return EqualsCore(key);
			return base.Equals(obj);
		}
		bool EqualsCore(BrickStyleKey key) {
			return object.Equals(BackColor, key.BackColor) &&
				object.Equals(ForeColor, key.ForeColor) &&
				object.Equals(BorderColor, key.BorderColor) &&
				object.Equals(BorderThickness, key.BorderThickness) &&
				object.Equals(FontFamily, key.FontFamily) &&
				object.Equals(FontStyle, key.FontStyle) &&
				object.Equals(FontWeight, key.FontWeight) &&
				object.Equals(FontSize, key.FontSize) &&
				object.Equals(Padding, key.Padding) &&
				object.Equals(HorizontalAlignment, key.HorizontalAlignment) &&
				object.Equals(VerticalAlignment, key.VerticalAlignment) &&
				object.Equals(TextWrapping, key.TextWrapping) &&
				object.Equals(TextDecorations, key.TextDecorations) &&
				object.Equals(BorderDashStyle, key.BorderDashStyle) &&
				object.Equals(TextTrimming, key.TextTrimming);
		}
		public override int GetHashCode() {
			return 0;
		}
	}
}
