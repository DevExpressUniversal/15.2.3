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
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public partial class BindingEditorControlElementBindingEditorView : UserControl {
		public BindingEditorControlElementBindingEditorView() {
			InitializeComponent();
			expander1.IsHitTestVisible = true;
			expander2.IsHitTestVisible = false;
			expander1.IsExpanded = true;
			expander2.IsExpanded = false;
		}
		private void view1_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				expander1.IsHitTestVisible = true;
				expander2.IsHitTestVisible = false;
				expander1.IsExpanded = true;
				expander2.IsExpanded = false;
			}
		}
		private void view2_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue) {
				if(double.IsNaN(view1.Height)) {
					view1.Height = view1.ActualHeight;
					view2.Height = view1.Height;
					row1.Height = new GridLength(1, GridUnitType.Auto);
					row2.Height = new GridLength(1, GridUnitType.Auto);
					viewContainer.UpdateLayout();
				}
				expander2.IsHitTestVisible = true;
				expander1.IsHitTestVisible = false;
				expander2.IsExpanded = true;
				expander1.IsExpanded = false;
			}
		}
	}
}
