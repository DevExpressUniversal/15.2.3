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

using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.Base {
	public abstract class veViewSelectorItem : veContentSelectorItem, IControl {
		public veViewSelectorItem() {
			Controller = CreateController();
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			Controller.UpdateState(false);
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			Controller.UpdateState(true);
		}
		protected virtual ControlControllerBase CreateController() {
			return new veViewSelectorItemController(this);
		}
		public ControlControllerBase Controller { get; private set; }
		#region IControl Members
		FrameworkElement IControl.Control { get { return this; } }
		Controller IControl.Controller { get { return Controller; } }
		#endregion
		public class veViewSelectorItemController : ControlControllerBase {
			public veViewSelectorItemController(veViewSelectorItem control)
				: base(control) {
			}
			public new veViewSelectorItem Control { get { return (veViewSelectorItem)base.Control; } }
			public override void UpdateState(bool useTransitions) {
				base.UpdateState(useTransitions);
				string stateName = "EmptySelectedState";
				VisualStateManager.GoToState(Control, stateName, useTransitions);
				if(Control.IsSelected)
					stateName = "Selected";
				else
					stateName = "Unselected";
				VisualStateManager.GoToState(Control, stateName, useTransitions);
			}
		}
	}
}
