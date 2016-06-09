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

using DevExpress.Office.Internal;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Scheduler.Commands;
using DevExpress.Xpf.Scheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Scheduler.UI {
	public abstract class ColorablePopupMenuBasedBarItem : SubMenuBarSubItem, IEditValueBarItem	{
		string dataTemplateString = @"
                        <DataTemplate  
                            xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                            xmlns:dxc='http://schemas.devexpress.com/winfx/2008/xaml/core'
                            xmlns:dxb='http://schemas.devexpress.com/winfx/2008/xaml/bars'
                            xmlns:dxe='http://schemas.devexpress.com/winfx/2008/xaml/editors'
                            xmlns:sys='clr-namespace:System;assembly=mscorlib'>
                            <Grid>
                                <Border x:Name='colorIndicator' HorizontalAlignment='Left' VerticalAlignment='Top'/>
                                <Image Source='{Binding ActualGlyph, RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=dxb:BarSubItemLinkControl}}'/>
                            </Grid>
                    </DataTemplate>";
		static ColorablePopupMenuBasedBarItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(ColorablePopupMenuBasedBarItem), typeof(ColorablePopupMenuBasedBarItemLink), delegate(object arg) { return new ColorablePopupMenuBasedBarItemLink(); });
		}
		protected ColorablePopupMenuBasedBarItem() {
#if SL
			GlyphTemplate = XamlReader.Load(dataTemplateString) as DataTemplate;
#else
			GlyphTemplate = XamlReader.Parse(dataTemplateString) as DataTemplate;
#endif
		}
		internal abstract Size ColorIndicatorSize { get; }
		internal abstract Thickness ColorIndicatorMarging { get; }
		internal abstract Size ColorIndicatorLargeSize { get; }
		internal abstract Thickness ColorIndicatorLargeMarging { get; }
		protected override void UpdateItemsCore() {
			int count = GetItemCount();
			for (int i = 0; i < count; i++) 
				AddCommandBarItem(i);
		}
		void AddCommandBarItem(int i) {
			BarCheckItem item = new BarCheckItem();
			SchedulerUICommand command = CreateItemCommand(i);
			item.Command = command;
			item.CommandParameter = SchedulerControl;
			item.IsPrivate = true;
			SchedulerControl.CommandManager.UpdateBarItemDefaultValues(item);
			SchedulerControl.CommandManager.UpdateBarItemCommandUIState(item, command.CreateCommand(SchedulerControl));
			ItemLinks.Add(item);
		}
		protected abstract SchedulerUICommand CreateItemCommand(int i); 
		protected abstract int GetItemCount();
		#region IEditValueBarItem
		#region EditValue
		public object EditValue {
			get { return (object)GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		public static readonly DependencyProperty EditValueProperty = CreateEditValueProperty();
		static DependencyProperty CreateEditValueProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<ColorablePopupMenuBasedBarItem, object>("EditValue", null, (d, e) => d.OnEditValueChanged(e.OldValue, e.NewValue), null);
		}
		void OnEditValueChanged(object oldValue, object newValue) {
			RaiseEditValueChanged(oldValue, newValue);
			UpdateLinkControls();
		}
		#endregion
		#region EditValueChanged
		EditValueChangedEventHandler onEditValueChanged;
		public event EditValueChangedEventHandler EditValueChanged { add { onEditValueChanged += value; } remove { onEditValueChanged -= value; } }
		protected internal virtual void RaiseEditValueChanged(object oldValue, object newValue) {
			if (onEditValueChanged != null)
				onEditValueChanged(this, new EditValueChangedEventArgs(oldValue, newValue));
		}
		#endregion
		#endregion
		protected override void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
			base.OnSchedulerControlChanged(oldValue, newValue);
			UpdateLinkControls();
		}
		protected internal virtual void UpdateLinkControls() {
			ReadOnlyLinkCollection links = this.Links;
			int count = links.Count;
			for (int i = 0; i < count; i++)
				UpdateLinkControl(links[i]);
		}
		protected internal virtual void UpdateLinkControl(BarItemLinkBase link) {
			ColorablePopupMenuBasedBarItemLink itemLink = link as ColorablePopupMenuBasedBarItemLink;
			if (itemLink != null)
				itemLink.UpdateLinkControl();
		}
	}
	public class ColorablePopupMenuBasedBarItemLink : BarSubItemLink {
		static ColorablePopupMenuBasedBarItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(ColorablePopupMenuBasedBarItemLink), typeof(ColorablePopupMenuBasedBarItemLinkControl), delegate(object arg) { return new ColorablePopupMenuBasedBarItemLinkControl((ColorablePopupMenuBasedBarItemLink)arg); });
		}
		protected override BarItemLinkControlBase CreateBarItemLinkControl() {
			return new ColorablePopupMenuBasedBarItemLinkControl(this);
		}
		protected internal virtual void UpdateLinkControl() {
			BarItemLinkInfoReferenceCollection linkInfos = LinkInfos;
			int count = linkInfos.Count;
			for (int i = 0; i < count; i++) {
				ColorablePopupMenuBasedBarItemLinkControl control = linkInfos[i].LinkControl as ColorablePopupMenuBasedBarItemLinkControl;
				if (control != null)
					control.UpdateColorIndicator();
			}
		}
	}
	public class ColorablePopupMenuBasedBarItemLinkControl : BarSubItemLinkControl {
		Border colorIndicator;
		public ColorablePopupMenuBasedBarItemLinkControl(ColorablePopupMenuBasedBarItemLink itemLink) : base(itemLink) {
			SizeChanged += OnSizeChanged;
		}
		Border ColorIndicator {
			get { return colorIndicator ?? (colorIndicator = DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(this, "colorIndicator") as Border); }
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			UpdateColorIndicatorColor();
		}		
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			if (ColorIndicator == null)
				return;
			ColorablePopupMenuBasedBarItem item = Item as ColorablePopupMenuBasedBarItem;
			if (e.NewSize.Height >= 32) {
				ColorIndicator.Margin = item.ColorIndicatorLargeMarging;
				ColorIndicator.Width = item.ColorIndicatorLargeSize.Width;
				ColorIndicator.Height = item.ColorIndicatorLargeSize.Height;
			} else {
				ColorIndicator.Margin = item.ColorIndicatorMarging;
				ColorIndicator.Width = item.ColorIndicatorSize.Width;
				ColorIndicator.Height = item.ColorIndicatorSize.Height;
			}
		}
		protected internal void UpdateColorIndicator() {
			if (UpdateColorIndicatorColor())
				ClosePopup();
		}
		bool UpdateColorIndicatorColor() {
			if (ColorIndicator == null)
				return false;
			if (Link == null)
				return false;
			ColorablePopupMenuBasedBarItem item = Link.Item as ColorablePopupMenuBasedBarItem;
			if (item == null)
				return false;
			UserInterfaceObjectWpf uiObject = (UserInterfaceObjectWpf)item.EditValue; 
			if (uiObject == null)
				return false;
			ColorIndicator.Background = uiObject.Brush;
			return true;
		}
	}
}
