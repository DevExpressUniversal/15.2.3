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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Navigation.Internal;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using System.Collections.ObjectModel;
using System.Windows;
namespace DevExpress.Xpf.Navigation {
	public class TileNavSubItem : NavElementBase {
		static TileNavSubItem() {
			var dProp = new DependencyPropertyRegistrator<TileNavSubItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.OverrideMetadata(ColorModeProperty, TileColorMode.Auto);
		}
		protected override System.Windows.Size GetSizeForTileSize(Internal.TileSize tileSize) {
			return new TilNavSubItemSizeProvider().GetSize(tileSize);
		}
		bool shouldInvertBrushes;
		protected override System.Windows.Media.Brush GetCalculatedBackground() {
			return shouldInvertBrushes ? Foreground : base.GetCalculatedBackground();
		}
		protected override System.Windows.Media.Brush GetCalculatedForeground() {
			return shouldInvertBrushes ? Background : base.GetCalculatedForeground();
		}
		protected override void UpdateCalculatedBrushes() {
			TileNavPaneBar owner = Owner as TileNavPaneBar;
			if(owner == null) {
				base.UpdateCalculatedBrushes();
				return;
			}
			shouldInvertBrushes = !this.IsInDesignTool() && (TileNavPane.GetFlyoutSourceType(owner) != FlyoutSourceType.FromTileNavPane && ActualColorMode == TileColorMode.Auto);
			base.UpdateCalculatedBrushes();
		}
		protected override void OnFlyoutClosing(ref bool cancel) {
			cancel = false;
		}
		internal class TilNavSubItemSizeProvider : SizeProvider {
			protected override System.Windows.Size GetSizeCore(Internal.TileSize tileSize) {
				switch(tileSize) {
					case TileSize.Auto:
						return new Size(double.NaN, double.NaN);
					case TileSize.Small:
						return new Size(60, 58);
					case TileSize.Wide:
						return new Size(260, 58);
					case TileSize.Default:
					case TileSize.Medium:
					default:
						return new Size(125, 58);
				}
			}
		}
	}
	public class TileNavSubItemCollection : ObservableCollection<TileNavSubItem> {
		INavElement Owner;
		internal TileNavSubItemCollection(INavElement owner) {
			Owner = owner;
		}
		protected override void InsertItem(int index, TileNavSubItem item) {
			base.InsertItem(index, item);
			item.AllowSelection = Owner.AllowSelection;
			if(Owner is NavButton) return;
			INavElement navElement = item;
			navElement.NavParent = Owner;
			Owner.AddChild(item);
		}
		protected override void RemoveItem(int index) {
			INavElement item = this[index];
			if(item != null && item.NavParent == Owner) {
				item.NavParent = null;
				Owner.RemoveChild(item);
			}
			base.RemoveItem(index);
		}
	}
}
