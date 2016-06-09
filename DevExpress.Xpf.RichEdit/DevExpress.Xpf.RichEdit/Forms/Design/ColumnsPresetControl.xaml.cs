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
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Windows.Media;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.Native;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	[ToolboxItem(false)]
	public partial class ColumnsPresetControl : UserControl {
		public ColumnsPresetControl() {
			DataContext = this;
			InitializeComponent();
			Loaded += OnLoaded;
		}
		#region Properties
		#region Text
		public static readonly DependencyProperty TextProperty = DependencyPropertyManager.Register("Text", typeof(string), typeof(ColumnsPresetControl), new FrameworkPropertyMetadata());
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		#endregion
		#region ImageSource
		public static readonly DependencyProperty ImageSourceProperty = DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), typeof(ColumnsPresetControl), new FrameworkPropertyMetadata());
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		#endregion
		[DefaultValue(false)]
		public bool Checked { get { return checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false; } set { checkBox.IsChecked = value; } }
		#endregion
		#region Events
		EventHandler onCheckedChanged;
		public event EventHandler CheckedChanged { add { onCheckedChanged += value; } remove { onCheckedChanged -= value; } }
		protected internal virtual void RaiseCheckedChanged() {
			if (onCheckedChanged != null)
				onCheckedChanged(this, EventArgs.Empty);
		}
		#endregion
		protected internal virtual void OnLoaded(object sender, RoutedEventArgs e) {
			checkBox.Checked += OnCheckBoxCheckedChanged;
			checkBox.Unchecked += OnCheckBoxCheckedChanged;
		}
		void OnCheckBoxCheckedChanged(object sender, RoutedEventArgs e) {
			RaiseCheckedChanged();
		}
	}
}
