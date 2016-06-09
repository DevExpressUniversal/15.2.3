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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Reports.UserDesigner.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.FontProperties {
	public abstract class BaseFontProperties : INotifyPropertyChanged {
		[Browsable(false)]
		public abstract FontFamily FontFamily { get; set; }
		[Browsable(false)]
		public abstract float FontSize { get; set; }
		[Browsable(false)]
		public abstract FontStyle FontStyle { get; set; }
		[Browsable(false)]
		public abstract FontWeight FontWeight { get; set; }
		[Browsable(false)]
		public abstract Brush Foreground { get; set; }
		[Browsable(false)]
		public abstract TextAlignment TextHorizontalAlignment { get; set; }
		[Browsable(false)]
		public abstract VerticalAlignment VerticalContentAlignment { get; set; }
		[Browsable(false)]
		public abstract TextDecorationCollection TextDecorations { get; set; }
		[Browsable(false)]
		public bool IsFontBold {
			get { return FontHelper.FontWeightToIsBold(FontWeight); }
			set { FontWeight = FontHelper.IsBoldToFontWeight(value); }
		}
		[Browsable(false)]
		public bool IsFontItalic {
			get { return FontHelper.FontStyleToIsItalic(FontStyle); }
			set { FontStyle = FontHelper.IsItalicToFontStyle(value); }
		}
		[Browsable(false)]
		public bool IsFontUnderline {
			get { return FontHelper.TextDecorationsToIsUnderline(TextDecorations); }
			set { TextDecorations = FontHelper.IsUnderlineToTextDecorations(value, TextDecorations); }
		}
		[Browsable(false)]
		public bool IsFontStrikethrough {
			get { return FontHelper.TextDecorationsToIsStrikethrough(TextDecorations); }
			set { TextDecorations = FontHelper.IsStrikethroughToTextDecorations(value, TextDecorations); }
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaiseFontWeightChanged() {
			RaisePropertyChanged(() => FontWeight);
			RaisePropertyChanged(() => IsFontBold);
		}
		protected virtual void RaiseFontStyleChanged() {
			RaisePropertyChanged(() => FontStyle);
			RaisePropertyChanged(() => IsFontItalic);
		}
		protected virtual void RaiseTextDecorationsChanged() {
			RaisePropertyChanged(() => TextDecorations);
			RaisePropertyChanged(() => IsFontUnderline);
			RaisePropertyChanged(() => IsFontStrikethrough);
		}
		protected void RaisePropertyChanged<T>(Expression<Func<T>> expression) {
			RaisePropertyChanged(BindableBase.GetPropertyName(expression));
		}
		protected void RaisePropertyChanged(string name) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
	}
}
