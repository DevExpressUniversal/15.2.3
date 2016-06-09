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
namespace DevExpress.Xpf.WindowsUI.Base {
	[TemplateVisualState(Name = UnselectedState, GroupName = SelectedStates)]
	[TemplateVisualState(Name = SelectedState, GroupName = SelectedStates)]
	public abstract class veSelectorItem : veContentControl, ISelectorItem, IItemContainer {
		#region States
		public const string SelectedStates = "SelectedStates";
		public const string UnselectedState = "Unselected";
		public const string SelectedState = "Selected";
		#endregion States
		#region static
		public static readonly DependencyProperty IsSelectedProperty;
		static veSelectorItem() {
			var dProp = new DependencyPropertyRegistrator<veSelectorItem>();
			dProp.Register("IsSelected", ref IsSelectedProperty, (bool)false,
				(dObj, e) => ((veSelectorItem)dObj).OnIsSelectedChanged((bool)e.NewValue));
		}
		#endregion static
		protected override void OnDispose() {
			base.OnDispose();
			ownerCore = null;
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		ISelector ownerCore;
		public ISelector Owner {
			get { return ownerCore ?? Parent as ISelector; }
		}
		ISelector ISelectorItem.Owner {
			get { return ownerCore; }
			set { ownerCore = value; }
		}
		bool ISelectorItem.IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		protected virtual void OnIsSelectedChanged(bool isSelected) {
			veSelectorBase.SetIsSelected(this, isSelected);
		}
	}
}
