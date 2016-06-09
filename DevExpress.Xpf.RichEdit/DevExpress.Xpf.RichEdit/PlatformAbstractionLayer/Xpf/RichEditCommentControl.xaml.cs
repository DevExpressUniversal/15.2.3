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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.RichEdit {
	public partial class RichEditCommentControl : UserControl {
		public RichEditCommentControl() {
			InitializeComponent();
		}
		public static readonly DependencyProperty LayoutPanelProperty = DependencyPropertyManager.Register("LayoutPanel", typeof(LayoutPanel), typeof(RichEditCommentControl), new FrameworkPropertyMetadata((new PropertyChangedCallback(OnLayoutPanelChanged))));
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutPanel LayoutPanel {
			get { return (LayoutPanel)GetValue(LayoutPanelProperty); }
			set { SetValue(LayoutPanelProperty, value); }
		}
		public static readonly DependencyProperty DockLayoutManagerProperty = DependencyPropertyManager.Register("DockLayoutManager", typeof(DockLayoutManager), typeof(RichEditCommentControl), new FrameworkPropertyMetadata((new PropertyChangedCallback(OnDockLayoutManagerChanged))));
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DockLayoutManager DockLayoutManager {
			get { return (DockLayoutManager)GetValue(DockLayoutManagerProperty); }
			set { SetValue(DockLayoutManagerProperty, value); }
		}
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditCommentControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		[ Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditCommentControl instance = (RichEditCommentControl)d;
			if ((e.NewValue != null)) {
				instance.innerCommentControl.RichEditControl = (RichEditControl)e.NewValue;
				instance.innerCommentControl.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
			}
		}
		static void OnLayoutPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditCommentControl instance = (RichEditCommentControl)d;
			instance.innerCommentControl.LayoutPanel = (LayoutPanel)e.NewValue;
		}
		static void OnDockLayoutManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditCommentControl instance = (RichEditCommentControl)d;
			instance.innerCommentControl.DockLayoutManager = (DockLayoutManager)e.NewValue;
		}
	}
}
