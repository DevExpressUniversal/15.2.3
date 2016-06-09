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
using DevExpress.Xpf.Docking.VisualElements;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Platform {
	public enum TabHintType {
		Tab, TabHeader
	};
	public abstract class BaseTabHint : psvControl {
		#region static
		public static readonly DependencyProperty TabHeaderLocationProperty;
		static BaseTabHint() {
			var dProp = new DependencyPropertyRegistrator<BaseTabHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("TabHeaderLocation", ref TabHeaderLocationProperty, SWC.Dock.Top,
				(dObj, e) => ((BaseTabHint)dObj).OnTabHeaderLocationChanged((SWC.Dock)e.NewValue));
		}
		#endregion static
		public SWC.Dock TabHeaderLocation {
			get { return (SWC.Dock)GetValue(TabHeaderLocationProperty); }
			set { SetValue(TabHeaderLocationProperty, value); }
		}
		protected virtual void OnTabHeaderLocationChanged(SWC.Dock headerLocation) {
			VisualStateManager.GoToState(this, headerLocation.ToString(), false);
		}
		public BaseTabHint(TabHintType type) {
			Type = type;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			VisualStateManager.GoToState(this, TabHeaderLocation.ToString(), false);
		}
		public TabHintType Type { get; private set; }
	}
	public class TabHint : BaseTabHint {
		static TabHint() {
			var dProp = new DependencyPropertyRegistrator<TabHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public TabHint()
			: base(TabHintType.Tab) {
		}
	}
	public class TabHeaderHint : BaseTabHint {
		static TabHeaderHint() {
			var dProp = new DependencyPropertyRegistrator<TabHeaderHint>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		public TabHeaderHint()
			: base(TabHintType.TabHeader) {
		}
	}
}
