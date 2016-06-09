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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraLayout;
namespace DevExpress.XtraLayout.Customization {
	public class SizeConstraintsFreeSizing : SizeConstraintsBase {
		public SizeConstraintsFreeSizing(UserInteractionHelper owner) : base(owner) { Text = "Free Sizing"; Image = UCIconsHelper.UnlockIcon; Category = Customization.Category.SizeConstrains; }
		public override void ExecuteCore(BaseLayoutItem item) { LockCore(false, false, item); }
		public override bool CanExecute() {
			return base.CanExecute();
		}
	}
	public class SizeConstraintsLockWidth : SizeConstraintsBase {
		public SizeConstraintsLockWidth(UserInteractionHelper owner) : base(owner) { Text = "Lock Width"; Image = UCIconsHelper.WLockIcon; Category = Customization.Category.SizeConstrains; }
		public override void ExecuteCore(BaseLayoutItem item) { LockCore(true, false, item); }
		public override bool CanExecute() {
			return base.CanExecute();
		}
	}
	public class SizeConstraintsLockHeight : SizeConstraintsBase {
		public SizeConstraintsLockHeight(UserInteractionHelper owner) : base(owner) { Text = "Lock Height"; Image = UCIconsHelper.HLockIcon; Category = Customization.Category.SizeConstrains; }
		public override void ExecuteCore(BaseLayoutItem item) { LockCore(false, true, item); }
		public override bool CanExecute() {
			return base.CanExecute();
		}
	}
	public class SizeConstraintsLockSize : SizeConstraintsBase {
		public SizeConstraintsLockSize(UserInteractionHelper owner) : base(owner) { Text = "Lock Size"; Image = UCIconsHelper.LockIcon; Category = Customization.Category.SizeConstrains; }
		public override void ExecuteCore(BaseLayoutItem item) { LockCore(true, true, item); }
		public override bool CanExecute() {
			return base.CanExecute();
		}
	}
	public class SizeConstraintsResetToDefault : SizeConstraintsBase {
		public SizeConstraintsResetToDefault(UserInteractionHelper owner) : base(owner) { Text = "Reset To Default"; Image = UCIconsHelper.ResetDefaultIcon; Category = Customization.Category.SizeConstrains; }
		public override void ExecuteCore(BaseLayoutItem item) {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			LayoutControlItem citem = item as LayoutControlItem;
			if(citem != null) {
				citem.SizeConstraintsType = SizeConstraintsType.Default;
			}
		}
	}
	public class SizeConstraintsBase : InteractionMethod {
		public SizeConstraintsBase(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
		}
		protected void LockCore(bool lockW, bool lockH, BaseLayoutItem item) {
			Size sz = item.Size;
			Size minSize = item.MinSize;
			Size maxSize = item.MaxSize;
			item.BeginInit();
			if(lockW && !lockH) {
				item.MaxSize = new Size(sz.Width, maxSize.Height);
				item.MinSize = new Size(sz.Width, minSize.Height);
			}
			if(lockH && !lockW) {
				item.MaxSize = new Size(maxSize.Width, sz.Height);
				item.MinSize = new Size(minSize.Width, sz.Height);
			}
			if(lockH && lockW) {
				item.MaxSize = sz;
				item.MinSize = sz;
			}
			if(!lockH && !lockW) {
				item.MaxSize = Size.Empty;
				item.MinSize = new Size(50, 20);
			}
			item.EndInit();
			item.Invalidate();
		}
		public override bool CanExecute() {
			List<BaseLayoutItem> selItems = Owner.GetSelection();
			if(selItems.Count < 1) return false;
			foreach(BaseLayoutItem item in selItems) {
				if(item is LayoutControlGroup || item is TabbedGroup) {
					return false;
				}
			}
			return true;
		}
	}
}
