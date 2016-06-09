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

using DevExpress.Design.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Design.SmartTags {
	public partial class PropertyLineDataTemplates : ResourceDictionary {
		public PropertyLineDataTemplates() {
			InitializeComponent();
		}
		private void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e) {
			TextBlock textBlock = sender as TextBlock;
			if(textBlock == null)
				return;
			TextBlockService.SetIsTextTrimmed(textBlock, TextBlockService.GetIsActualTextBlockTrimmed(textBlock));
		}
		private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			var fe = sender as FrameworkElement;
			if(fe != null && fe.ContextMenu != null) {
				fe.ContextMenu.Placement = PlacementMode.Bottom;
				fe.ContextMenu.PlacementTarget = fe;
				fe.ContextMenu.IsOpen = true;
				e.Handled = fe.ContextMenu.IsOpen;
			}
		}
		void Grid_SizeChanged(object sender, SizeChangedEventArgs e) {
			WpfLayoutHelper.FindElementByName((FrameworkElement)sender, "txtBlock").MaxWidth = e.NewSize.Width * 0.35;
		}
	}
}
