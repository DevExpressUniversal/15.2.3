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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils.Themes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Ribbon.Themes;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Ribbon {
	public class ApplicationMenuBarControl : PopupMenuBarControl {
		#region static
		public static readonly DependencyProperty ActualBorderTemplateProperty;
		protected static readonly DependencyPropertyKey ActualBorderTemplatePropertyKey;
		public static readonly DependencyProperty ActualRightPaneWidthProperty;
		protected static readonly DependencyPropertyKey ActualRightPaneWidthPropertyKey;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty BorderTemplateForOffice2007StyleProperty;
		public static readonly DependencyProperty BorderTemplateForOffice2010StyleProperty;
		public static readonly DependencyProperty RightPaneWidthProperty;
		static ApplicationMenuBarControl() {
			Type ownerType = typeof(ApplicationMenuBarControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			ActualBorderTemplatePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualBorderTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null));
			ActualBorderTemplateProperty = ActualBorderTemplatePropertyKey.DependencyProperty;
			ActualRightPaneWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualRightPaneWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(double.NaN));
			ActualRightPaneWidthProperty = ActualRightPaneWidthPropertyKey.DependencyProperty;
			RibbonStyleProperty = RibbonControl.RibbonStyleProperty.AddOwner(ownerType, new PropertyMetadata(OnRibbonStylePropertyChanged));
			BorderTemplateForOffice2007StyleProperty = DependencyPropertyManager.Register("BorderTemplateForOffice2007Style", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnBorderTemplateForOffice2007StylePropertyChanged));
			BorderTemplateForOffice2010StyleProperty = DependencyPropertyManager.Register("BorderTemplateForOffice2010Style", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnBorderTemplateForOffice2010StylePropertyChanged));
			RightPaneWidthProperty = DependencyPropertyManager.Register("RightPaneWidth", typeof(double), ownerType,
							new PropertyMetadata(double.NaN, OnRightPaneWidthPropertyChanged));
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuBarControl)d).UpdateActualBorderTemplate();
		}
		protected static void OnBorderTemplateForOffice2010StylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuBarControl)d).UpdateActualBorderTemplate();
		}
		protected static void OnBorderTemplateForOffice2007StylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuBarControl)d).UpdateActualBorderTemplate();
		}
		protected static void OnRightPaneWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuBarControl)d).UpdateActualRightPaneWidth();
		}
		#endregion
		#region propdefs
		public ControlTemplate ActualBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualBorderTemplateProperty); }
			protected set { this.SetValue(ActualBorderTemplatePropertyKey, value); }
		}	   
		public double ActualRightPaneWidth {
			get { return (double)GetValue(ActualRightPaneWidthProperty); }
			protected set { this.SetValue(ActualRightPaneWidthPropertyKey, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public ControlTemplate BorderTemplateForOffice2007Style {
			get { return (ControlTemplate)GetValue(BorderTemplateForOffice2007StyleProperty); }
			set { SetValue(BorderTemplateForOffice2007StyleProperty, value); }
		}
		public ControlTemplate BorderTemplateForOffice2010Style {
			get { return (ControlTemplate)GetValue(BorderTemplateForOffice2010StyleProperty); }
			set { SetValue(BorderTemplateForOffice2010StyleProperty, value); }
		}
		public double RightPaneWidth {
			get { return (double)GetValue(RightPaneWidthProperty); }
			set { SetValue(RightPaneWidthProperty, value); }
		}
		#endregion
		public ApplicationMenuBarControl() {
			ContainerType = LinkContainerType.ApplicationMenu;
		}
		public ApplicationMenuBarControl(ApplicationMenu menu) : this() {
			Popup = menu;
		}
		protected ApplicationMenu ApplicationMenu { get { return Popup as ApplicationMenu; } }
		public DXContentPresenter RightPane { get; private set; }
		protected Grid LeftPaneGrid { get; private set; }
		protected override System.Windows.Size DefaultMaxGlyphSize {
			get {
				if(ApplicationMenu != null && ApplicationMenu.GlyphSize != GlyphSize.Default)
					return base.DefaultMaxGlyphSize;
				return new Size(32, 32);
			}
		}
		public override void OnApplyTemplate() {
			RightPane = GetTemplateChild("PART_RightPanePresenter") as DXContentPresenter;		   
			UpdateActualRightPaneWidth();
			LeftPaneGrid = GetTemplateChild("PART_LeftPaneGrid") as Grid;
			UpdateLeftPaneGridColumns(ApplicationMenu==null ? false : ApplicationMenu.ShowRightPane);
		}
		protected virtual void UpdateActualRightPaneWidth() {
			if(ApplicationMenu == null || double.IsNaN(ApplicationMenu.RightPaneWidth))
				ActualRightPaneWidth = RightPaneWidth;
			else
				ActualRightPaneWidth = ApplicationMenu.RightPaneWidth;
		}
		internal void UpdateLeftPaneGridColumns(bool newValue) {
			if (LeftPaneGrid == null)
				return;
			LeftPaneGrid.ColumnDefinitions.Clear();
			if (newValue) {
				LeftPaneGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
				LeftPaneGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
			}
		}
		protected virtual void UpdateActualBorderTemplate() {
			ActualBorderTemplate = RibbonStyle == Ribbon.RibbonStyle.Office2007 ? BorderTemplateForOffice2007Style : BorderTemplateForOffice2010Style;
		}
	}
}
