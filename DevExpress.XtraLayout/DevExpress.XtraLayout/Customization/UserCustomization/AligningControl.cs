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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraLayout;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraLayout.Customization {
	public static class ControlAlignmentInterection {
		public static void Execute(UserInteractionHelper owner, ContentAlignment alignment) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			foreach(BaseLayoutItem item in selItems) {
				LayoutControlItem temp = item as LayoutControlItem;
				temp.StartChange();
				temp.FillControlToClientArea = false;
				temp.ControlAlignment = alignment;
				temp.EndChange();
			}
		}
		public static void CanExecute(UserInteractionHelper owner, ContentAlignment alignment, out bool check, out bool ret) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			ret = true;
			check = false;
			if(selItems.Count <= 0) { ret = false; return; }
			LayoutControlItem temp = selItems[0] as LayoutControlItem;
			if(temp == null) { ret = false; return; }
			if(temp.Control == null) { ret = false; return; }
			ContentAlignment commonValue = temp.ControlAlignment;
			if(commonValue == alignment) {
				check = true;
			}
			foreach(BaseLayoutItem item in selItems) {
				temp = item as LayoutControlItem;
				if(temp == null) { ret = false; return; }
				if(temp.Control == null) { ret = false; return; }
				if((item as LayoutControlItem).Control.Bounds.Height == item.ViewInfo.ClientAreaRelativeToControl.Height && (alignment == ContentAlignment.BottomCenter || alignment == ContentAlignment.BottomLeft || alignment == ContentAlignment.BottomRight || alignment == ContentAlignment.TopCenter || alignment == ContentAlignment.TopLeft || alignment == ContentAlignment.TopRight)) {
					ret = false;
					break;
				}
				if(temp.Control.Bounds.Height + temp.Control.Margin.Vertical >= temp.ViewInfo.ClientAreaRelativeToControl.Height && temp.Control.Bounds.Width + temp.Control.Margin.Horizontal >= temp.ViewInfo.ClientAreaRelativeToControl.Width) { ret = false; return; }
			}
		}
	}
	class ControlTopLeft : InteractionMethod {
		public ControlTopLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top left";
			Image = UCIconsHelper.ControlTopLIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.TopLeft);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.TopLeft, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlTopCenter : InteractionMethod {
		public ControlTopCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top center";
			Image = UCIconsHelper.ControlTopCIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.TopCenter);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.TopCenter, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlTopRight : InteractionMethod {
		public ControlTopRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top right";
			Image = UCIconsHelper.ControlTopRIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.TopRight);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.TopRight, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlMiddleLeft : InteractionMethod {
		public ControlMiddleLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle left";
			Image = UCIconsHelper.ControlMidLIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.MiddleLeft);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.MiddleLeft, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlMiddleCenter : InteractionMethod {
		public ControlMiddleCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle center";
			Image = UCIconsHelper.ControlMidCIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.MiddleCenter);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.MiddleCenter, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlMiddleRight : InteractionMethod {
		public ControlMiddleRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle right";
			Image = UCIconsHelper.ControlMidRIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.MiddleRight);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.MiddleRight, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlBottomLeft : InteractionMethod {
		public ControlBottomLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Botton left";
			Image = UCIconsHelper.ControlBotLIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.BottomLeft);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.BottomLeft, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlBottomCenter : InteractionMethod {
		public ControlBottomCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom center";
			Image = UCIconsHelper.ControlBotCIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.BottomCenter);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.BottomCenter, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class ControlBottomRight : InteractionMethod {
		public ControlBottomRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom right";
			Image = UCIconsHelper.ControlBotRIcon;
		}
		public override void Execute() {
			ControlAlignmentInterection.Execute(Owner, ContentAlignment.BottomRight);
		}
		public override bool CanExecute() {
			bool chk, ret;
			ControlAlignmentInterection.CanExecute(Owner, ContentAlignment.BottomRight, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
}
