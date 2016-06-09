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
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Office.UI {
	#region BarSplitButtonColorEditItem
	[DXToolboxBrowsable(false)]
	public partial class BarSplitButtonColorEditItem : BarSplitButtonEditItem {
		static BarSplitButtonColorEditItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(BarSplitButtonColorEditItem), typeof(BarSplitButtonColorEditItemLink), delegate(object arg) { return new BarSplitButtonColorEditItemLink(); });
#if !SL
			EditValueProperty.OverrideMetadata(typeof(BarSplitButtonColorEditItem), new FrameworkPropertyMetadata(System.Windows.Media.Colors.Black));
#endif
		}
		public BarSplitButtonColorEditItem() {
			InitializeComponent();
			ClearValue(NameScope.NameScopeProperty);
			InitializePopupControl();
		}
		void InitializePopupControl() {
			var colorEdit = new ColorEdit() { ShowBorder = false };
			colorEdit.SetBinding(ColorEdit.EditValueProperty, new Binding("EditValue") { Source = this, Mode = BindingMode.TwoWay });
			PopupControl = new PopupControlContainer() { Content = colorEdit };
		}
		public override BarItemLink CreateLink(bool isPrivate) {
			return new BarSplitButtonColorEditItemLink();
		}
		protected internal override void UpdateLinkControl(BarItemLinkBase link) {
			base.UpdateLinkControl(link);
			BarSplitButtonColorEditItemLink colorEditLink = link as BarSplitButtonColorEditItemLink;
			if (colorEditLink != null)
				colorEditLink.UpdateLinkControl();
		}
		protected override void OnEditValueChanged(object oldValue, object newValue) {
			base.OnEditValueChanged(oldValue, newValue);
		}
	}
	#endregion
	#region BarSplitButtonColorEditItemLink
	[DXToolboxBrowsable(false)]
	public class BarSplitButtonColorEditItemLink : BarSplitButtonItemLink {
		public BarSplitButtonColorEditItemLink() {
		}
		static BarSplitButtonColorEditItemLink() {
			BarItemLinkControlCreator.Default.RegisterObject(typeof(BarSplitButtonColorEditItemLink), typeof(BarSplitButtonColorEditItemLinkControl), delegate(object arg) { return new BarSplitButtonColorEditItemLinkControl((BarSplitButtonColorEditItemLink)arg); });
		}
		protected internal override BarItemLinkControlBase CreateBarItemLinkControl() {
			return new BarSplitButtonColorEditItemLinkControl(this);
		}
		protected internal virtual void UpdateLinkControl() {
			BarItemLinkInfoReferenceCollection linkInfos = LinkInfos;
			int count = linkInfos.Count;
			for (int i = 0; i < count; i++) {
				BarSplitButtonColorEditItemLinkControl control = linkInfos[i].LinkControl as BarSplitButtonColorEditItemLinkControl;
				if (control != null)
					control.UpdateColorIndicator();
			}
		}
	}
	#endregion
	#region BarSplitButtonColorEditItemLinkControl
	[DXToolboxBrowsable(false)]
	public class BarSplitButtonColorEditItemLinkControl : BarSplitButtonItemLinkControl {
		Border colorIndicator;
		public BarSplitButtonColorEditItemLinkControl(BarSplitButtonItemLink link)
			: base(link) {
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
			if (e.NewSize.Height >= 32) {
				ColorIndicator.Margin = new Thickness(0, 0, 0, 2);
				ColorIndicator.Width = 28;
				ColorIndicator.Height = 4;
			}
			else {
				ColorIndicator.Margin = new Thickness(0, 0, 0, 0);
				ColorIndicator.Width = 16;
				ColorIndicator.Height = 3;
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
			BarSplitButtonEditItem item = Link.Item as BarSplitButtonEditItem;
			if (item == null)
				return false;
			if (item.EditValue == null || !(item.EditValue is Color))
				return false;
			ColorIndicator.Background = new SolidColorBrush((Color)item.EditValue);
			return true;
		}
	}
	#endregion
}
