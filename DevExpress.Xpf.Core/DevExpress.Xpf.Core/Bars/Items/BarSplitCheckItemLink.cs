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
using System.Windows.Data;
using DevExpress.Utils.Serializing;
using System.ComponentModel;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	public class BarSplitCheckItemLink : BarSplitButtonItemLink, IBarCheckItemLink {
		#region static
		public static readonly DependencyProperty IsCheckedProperty;
		static readonly DependencyPropertyKey IsCheckedPropertyKey;
		public static readonly DependencyProperty IsThreeStateProperty;
		static readonly DependencyPropertyKey IsThreeStatePropertyKey;
		static BarSplitCheckItemLink() {
			IsThreeStatePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsThreeState", typeof(bool), typeof(BarSplitCheckItemLink), new FrameworkPropertyMetadata(false));
			IsThreeStateProperty = IsThreeStatePropertyKey.DependencyProperty;
			IsCheckedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsChecked", typeof(bool?), typeof(BarSplitCheckItemLink), new FrameworkPropertyMetadata(false, (d, e) => ((BarSplitCheckItemLink)d).OnIsCheckedChanged(e)));
			IsCheckedProperty = IsCheckedPropertyKey.DependencyProperty;
		}
		#endregion
		#region dep props
		public bool IsThreeState {
			get { return (bool)GetValue(IsThreeStateProperty); }
			private set { this.SetValue(IsThreeStatePropertyKey, value); }
		}
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
		#endregion
		protected BarSplitCheckItem SplitCheckItem { get { return base.Item as BarSplitCheckItem; } }
		protected internal override void UpdateProperties() {
			base.UpdateProperties();
			UpdateSplitCheckItemProperties();
		}
		protected internal virtual void UpdateSplitCheckItemProperties() {
			if(SplitCheckItem == null) return;
			IsThreeState = SplitCheckItem.IsThreeState;
			IsChecked = SplitCheckItem.IsChecked;
		}
		protected virtual void OnIsCheckedChanged(DependencyPropertyChangedEventArgs e) {
			ExecuteActionOnLinkControls<BarSplitCheckItemLinkControl>(lc => lc.OnSourceIsCheckedChanged());
		}
		public override void Assign(BarItemLinkBase link) {
			base.Assign(link);
			BarSplitCheckItemLink cLink = link as BarSplitCheckItemLink;
			if(cLink == null) return;
			IsThreeState = cLink.IsThreeState;
			IsChecked = cLink.IsChecked;
		}
		void IBarCheckItemLink.UpdateCheckItemProperties() { UpdateSplitCheckItemProperties(); }
	}
}
