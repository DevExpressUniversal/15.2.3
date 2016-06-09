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
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting.Native.Lines;
using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using System.Windows;
using System.ComponentModel;
using DevExpress.Utils.Design;
using System.Globalization;
#if SILVERLIGHT
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.Xpf.Printing.Native.Lines {
	class BooleanLine : PropertyLine {
		ComboBoxEdit editor;
		List<bool> items;
		Dictionary<string, bool> values;
		Label label;
		public override Label Header {
			get { return label; }
		}
		public override Control Content {
			get { return editor; }
		}
		public BooleanLine(PropertyDescriptor property, object obj)
			: base(property, obj) {
			editor = new ComboBoxEdit();
			label = new Label() { Padding = new Thickness(0), VerticalAlignment = VerticalAlignment.Center };
			items = new List<bool>() { false, true };
			values = new Dictionary<string, bool>();
			values[PrintingLocalizer.GetString(PrintingStringId.True)] = true;
			values[PrintingLocalizer.GetString(PrintingStringId.False)] = false;
			editor.ItemsSource = values;
			editor.DisplayMember = "Key";
			editor.ValueMember = "Value";
			editor.SelectedIndexChanged += editor_SelectedIndexChanged;
		}
		private void editor_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			Value = editor.EditValue;
		}
		public override void SetText(string text) {
#if !SILVERLIGHT
			label.Content = new TextBlock() { Text = text, TextTrimming = TextTrimming.CharacterEllipsis };
#else 
			label.Content = text;
#endif
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				editor.SelectedIndexChanged -= editor_SelectedIndexChanged;
			}
		}
		public override void RefreshContent() {
			base.RefreshContent();
			editor.EditValue = Convert.ToBoolean(Value);
		}
	}
}
