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
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Metadata;
namespace DevExpress.Xpf.Core.Design {
	public class ControlResourceContainerBaseAdornerProvider : PrimarySelectionAdornerProvider {
		bool settingProperties;
		private ModelItem adornedControlModel;
		Button showPreviewButton;
		AdornerPanel autoSizeAdornerPanel;
		public ControlResourceContainerBaseAdornerProvider() {
			showPreviewButton = new Button();
			showPreviewButton.Content = "Show Preview";
			showPreviewButton.FontFamily = AdornerFonts.FontFamily;
			showPreviewButton.FontSize = AdornerFonts.FontSize;
			showPreviewButton.Background = new SolidColorBrush(Colors.Green);
		}
		protected override void Activate(ModelItem item) {
			adornedControlModel = item;
			adornedControlModel.PropertyChanged +=
				new System.ComponentModel.PropertyChangedEventHandler(
					AdornedControlModel_PropertyChanged);
			AdornerPanel panel = this.Panel;
			AdornerPlacementCollection placement = new AdornerPlacementCollection();
			AdornerPanel.SetHorizontalStretch(showPreviewButton, AdornerStretch.None);
			AdornerPanel.SetVerticalStretch(showPreviewButton, AdornerStretch.None);
			placement.SizeRelativeToAdornerDesiredWidth(1.0, 0);
			placement.SizeRelativeToAdornerDesiredHeight(1.0, 0);
			placement.PositionRelativeToAdornerHeight(-1.0, -23);
			placement.PositionRelativeToAdornerWidth(0, -23);
			AdornerPanel.SetPlacements(showPreviewButton, placement);
			showPreviewButton.PreviewMouseUp += new MouseButtonEventHandler(OnShowPreview);
		}
		protected void OnShowPreview(object sender, MouseButtonEventArgs e) {
			if(adornedControlModel == null) return;
			adornedControlModel.Properties["ShowPreview"].SetValue(1);
			adornedControlModel.Properties["ShowPreview"].SetValue(-1);
		}
		private AdornerPanel Panel {
			get {
				if(this.autoSizeAdornerPanel == null) {
					autoSizeAdornerPanel = new AdornerPanel();
					autoSizeAdornerPanel.Children.Add(showPreviewButton);
					Adorners.Add(autoSizeAdornerPanel);
				}
				return this.autoSizeAdornerPanel;
			}
		}
		void autoSizeCheckBox_Unchecked(object sender, RoutedEventArgs e) {
			this.SetHeightAndWidth(false);
		}
		private void SetHeightAndWidth(bool autoSize) {
			settingProperties = true;
			try {
				using(ModelEditingScope batchedChange = adornedControlModel.BeginEdit()) {
					batchedChange.Complete();
				}
			}
			finally { settingProperties = false; }
		}
		protected override void Deactivate() {
			adornedControlModel.PropertyChanged -=
				new System.ComponentModel.PropertyChangedEventHandler(
					AdornedControlModel_PropertyChanged);
			base.Deactivate();
		}
		void AdornedControlModel_PropertyChanged(
			object sender,
			System.ComponentModel.PropertyChangedEventArgs e) {
			if(settingProperties) {
				return;
			}
			if(e.PropertyName == "Height" || e.PropertyName == "Width") {
			}
		}
	}
}
