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

#if SL
extern alias Platform;
using Platform::System.Windows.Input;
using Platform::System;
#endif
using Microsoft.Windows.Design;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Design.CoreUtils;
using DevExpress.Xpf.Core.Design;
using System.Linq;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
namespace DevExpress.Xpf.Grid.Design {
	[ToolboxBrowsableAttribute(false)]
	partial class GridControlAdornerPanel : UserControl {
		public GridControlAdornerPanel() {
			InitializeComponent();
			border.Effect = new DropShadowEffect() { Color = Colors.Black, ShadowDepth = 2, Direction = 270, BlurRadius = 6, Opacity = 0.15 };
			SetMenuBindings(changeViewMenu, selectViewButton);
			SetMenuBindings(changeEditSettingsMenu, selectEditSettingsButton);
		}
		void SetMenuBindings(ContextMenu menu, ContextMenuToggleButton button) {
			menu.SetBinding(FrameworkElement.DataContextProperty, new Binding(FrameworkElement.DataContextProperty.Name) { Source = this });
			menu.SetBinding(FrameworkElement.MinWidthProperty, new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = button });
			button.SetBinding(FrameworkElement.TagProperty, new Binding() { Source = menu, Mode = BindingMode.OneWay });
			menu.PlacementTarget = button;
		}
	}
}
