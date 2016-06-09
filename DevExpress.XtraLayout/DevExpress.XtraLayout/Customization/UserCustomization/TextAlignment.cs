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
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.XtraLayout.Customization {
	public static class TextAlignmentInterection {
		public static void Execute(UserInteractionHelper owner, HorzAlignment horz, VertAlignment vert) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			foreach(BaseLayoutItem item in selItems) {
				item.StartChange();
				item.AppearanceItemCaption.TextOptions.HAlignment = horz;
				item.AppearanceItemCaption.TextOptions.VAlignment = vert;
				item.EndChange();
			}
		}
		public static void CanExecute(UserInteractionHelper owner, HorzAlignment horz, VertAlignment vert, out bool check, out bool ret) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			ret = true;
			check = false;
			if(selItems.Count <= 0) { ret = false; return; }
			if(!selItems[0].TextVisible) { ret = false; return; }
			HorzAlignment ch = selItems[0].AppearanceItemCaption.TextOptions.HAlignment;
			VertAlignment cv = selItems[0].AppearanceItemCaption.TextOptions.VAlignment;
			foreach(BaseLayoutItem item in selItems) {
				if(item.TextSize.Height == item.ViewInfo.TextArea.Height && (vert == VertAlignment.Bottom || vert == VertAlignment.Top)) {
					ret = false;
					break;
				}
			}
			if(ch == horz && cv == vert) {
				check = true;
				foreach(BaseLayoutItem item in selItems) {
					if(item.AppearanceItemCaption.TextOptions.HAlignment != horz && item.AppearanceItemCaption.TextOptions.VAlignment != vert) {
						check = false;
						break;
					}
				}
			}
		}
	}
	class TextTopLeft : InteractionMethod {
		public TextTopLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top left";
			Image = UCIconsHelper.TextTopLIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Near, VertAlignment.Top);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Near, VertAlignment.Top, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextTopCenter : InteractionMethod {
		public TextTopCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top center";
			Image = UCIconsHelper.TextTopCIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Center, VertAlignment.Top);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Center, VertAlignment.Top, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextTopRight : InteractionMethod {
		public TextTopRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top right";
			Image = UCIconsHelper.TextTopRIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Far, VertAlignment.Top);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Far, VertAlignment.Top, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextMiddleLeft : InteractionMethod {
		public TextMiddleLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle left";
			Image = UCIconsHelper.TextMidLIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Near, VertAlignment.Center);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Near, VertAlignment.Center, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextMiddleCenter : InteractionMethod {
		public TextMiddleCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle center";
			Image = UCIconsHelper.TextMidCIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Center, VertAlignment.Center);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Center, VertAlignment.Center, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextMiddleRight : InteractionMethod {
		public TextMiddleRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Middle right";
			Image = UCIconsHelper.TextMidRIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Far, VertAlignment.Center);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Far, VertAlignment.Center, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextBottomLeft : InteractionMethod {
		public TextBottomLeft(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom left";
			Image = UCIconsHelper.TextBotLIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Near, VertAlignment.Bottom);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Near, VertAlignment.Bottom, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextBottomCenter : InteractionMethod {
		public TextBottomCenter(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom center";
			Image = UCIconsHelper.TextBotCIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Center, VertAlignment.Bottom);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Center, VertAlignment.Bottom, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class TextBottomRight : InteractionMethod {
		public TextBottomRight(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom right";
			Image = UCIconsHelper.TextBotRIcon;
		}
		public override void Execute() {
			TextAlignmentInterection.Execute(Owner, HorzAlignment.Far, VertAlignment.Bottom);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextAlignmentInterection.CanExecute(Owner, HorzAlignment.Far, VertAlignment.Bottom, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
}
