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
using System.Windows.Data;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System;
namespace DevExpress.Xpf.Ribbon.Customization {
	public partial class CustomizationControl : UserControl {
		public static readonly DependencyProperty RibbonProperty =
			DependencyPropertyManager.Register("Ribbon", typeof(RibbonControl), typeof(CustomizationControl), new PropertyMetadata(null));
		public CustomizationControl(RibbonControl ribbon) {
			this.Ribbon = ribbon;
			this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(CustomizationControl_IsVisibleChanged);						  
			InitializeComponent();
			foreach(var item in tabsCustomizationControl.LeftPartItems) {
				leftPartItems.Items.Add(item);
			}
			InitialCustomizationsCount = ribbon.RuntimeCustomizations.Count;
			tabsCustomizationControl.Loaded += new RoutedEventHandler(OnTabsCustomizationControlLoaded);
			tabsCustomizationControl.Reseted += new RoutedEventHandler(OnTabsCustomizationControlReseted);				   
			this.Unloaded += new RoutedEventHandler(OnCustomizationControlUnloaded);
			BindingOperations.SetBinding(tabsCustomizationControl, TabsCustomizationControl.RibbonProperty, new Binding("Ribbon") { Source = this });
		}
		void CustomizationControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(IsVisible == false) Ribbon.CustomizationHelper.IsCustomizationMode = false;
		}
		void OnCustomizationControlUnloaded(object sender, RoutedEventArgs e) {
			this.Unloaded -= OnCustomizationControlUnloaded;		 
		}
		private void OnCloseButtonClick(object sender, Bars.ItemClickEventArgs e) {
			Ribbon.CustomizationHelper.CustomizationForm.Close();
			Ribbon.RuntimeCustomizations.Undo(InitialCustomizationsCount, true, true);
		}
		private void OnOkButtonClick(object sender, Bars.ItemClickEventArgs e) {
			if (Ribbon.IsMerged)
				((IHierarchicalMergingSupport<RibbonControl>)Ribbon).Helper.ReMergeForce();
			Ribbon.CustomizationHelper.CustomizationForm.Close();			
			Ribbon.RuntimeCustomizations.Apply(InitialCustomizationsCount);
		}
		void OnTabsCustomizationControlLoaded(object sender, RoutedEventArgs e) {
			tabsCustomizationControl.Loaded -= OnTabsCustomizationControlLoaded;
		}
		void OnTabsCustomizationControlReseted(object sender, RoutedEventArgs e) {
			 InitialCustomizationsCount = Ribbon.RuntimeCustomizations.Count;
		}
		public int InitialCustomizationsCount { get; private set; }
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
	}	
}
