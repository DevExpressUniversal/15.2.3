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
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarCheckItemLink : BarButtonItemLink, IBarCheckItemLink {
		#region static
		public static readonly DependencyProperty IsCheckedProperty;
		static readonly DependencyPropertyKey IsCheckedPropertyKey;
		public static readonly DependencyProperty IsThreeStateProperty;
		static readonly DependencyPropertyKey IsThreeStatePropertyKey;
		static BarCheckItemLink() {
			IsThreeStatePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsThreeState", typeof(bool), typeof(BarCheckItemLink), new FrameworkPropertyMetadata(false));
			IsThreeStateProperty = IsThreeStatePropertyKey.DependencyProperty;
			IsCheckedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsChecked", typeof(bool?), typeof(BarCheckItemLink), new FrameworkPropertyMetadata(false, (d, e) => ((BarCheckItemLink)d).OnIsCheckedChanged(e)));
			IsCheckedProperty = IsCheckedPropertyKey.DependencyProperty;
		}
		#endregion
		public BarCheckItemLink() { }
		protected BarCheckItem CheckItem { get { return base.Item as BarCheckItem; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCheckItemLinkIsThreeState")]
#endif
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			private set { this.SetValue(IsThreeStatePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCheckItemLinkIsChecked")]
#endif
		public bool? IsChecked {
			get {
				object value = GetValue(IsCheckedProperty);
				if(value == null) return null;
				return new bool?((bool)value);
			}
			private set {
				this.SetValue(IsCheckedPropertyKey, value.HasValue ? (bool?)(value.Value) : null);
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCheckItemLinkGroupIndex")]
#endif
		public int GroupIndex {
			get {
				if(CheckItem != null)
					return CheckItem.GroupIndex;
				return -1;
			}
		}
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			UpdateCheckItemProperties();
		}
		protected internal virtual void UpdateCheckItemProperties() {
			if(CheckItem == null) return;
			IsThreeState = CheckItem.IsThreeState;
			IsChecked = CheckItem.IsChecked;
		}
		protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnBaseLinkControls((lc) => { if(lc is BarCheckItemLinkControl) ((BarCheckItemLinkControl)lc).OnSourceIsCheckedChanged(); });
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarCheckItemLink cLink = link as BarCheckItemLink;
			if(cLink == null) return;
			IsThreeState = cLink.IsThreeState;
			IsChecked = cLink.IsChecked;
		}
		void IBarCheckItemLink.UpdateCheckItemProperties() { UpdateCheckItemProperties(); }
	}
}
