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

extern alias Platform;
using Platform::DevExpress.Xpf.Core;
#if SL
using DependencyObject = Platform::System.Windows.DependencyObject;
#endif
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Effects;
namespace DevExpress.Xpf.Core.Design.Wizards {
	public partial class EditMaskWizardWindow : Window {
		public EditMaskWizardWindow() {
			InitializeComponent();
#if !SL
			DevExpress.Xpf.Core.ThemeManager.SetThemeName(this, DevExpress.Xpf.Core.Theme.MetropolisLightName);
#endif
			content.Child = new EditMaskWizard();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Border b = GetTemplateChild("dropShadowBorder") as Border;
			b.Effect = new DropShadowEffect() { BlurRadius = 32, Direction = -90, Opacity = 0.35, ShadowDepth = 8 };
		}
		void OKButton_Click(object sender, RoutedEventArgs e) {
			((EditMaskWizard)content.Child).FormResult();
			Close();
		}
		void CancelButton_Click(object sender, RoutedEventArgs e) {
			Close();
		}
		void OnCloseButtonClick(object sender, RoutedEventArgs e) {
			Close();
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if (!e.Handled) DragMove();
		}
	}
}
