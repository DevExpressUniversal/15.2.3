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
namespace DevExpress.XtraLayout.Customization
{
	public static class TextPositionInterection {
		public static void Execute(UserInteractionHelper owner, Locations location) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			foreach(BaseLayoutItem item in selItems) {
				item.TextLocation = location;
			}
		}
		public static void CanExecute(UserInteractionHelper owner, Locations location, out bool check, out bool ret) {
			List<BaseLayoutItem> selItems = owner.GetSelection();
			check = false;
			if(selItems.Count <= 0) { ret = false; return; }
			Locations temp = selItems[0].TextLocation;
			if(temp == location) {
				check = true;
				foreach(BaseLayoutItem item in selItems) {
					if(item.TextLocation != temp) {
						check = false;
						break;
					}
				}
			}
			ret = true;
		}
	}
	class Top : InteractionMethod {
		public Top(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Top";
			Image = UCIconsHelper.TextTopIcon;
		}
		public override void Execute() {
			TextPositionInterection.Execute(Owner, Locations.Top);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextPositionInterection.CanExecute(Owner, Locations.Top, out chk, out ret);
			Checked = chk;
			return ret;
		}	   
	}
	class Bottom : InteractionMethod {
		public Bottom(UserInteractionHelper owner)
			: base(owner) {
			EditorType = EditorTypes.RadioButton;
			Text = "Bottom";
			Image = UCIconsHelper.TextBottomIcon;
		}
		public override void Execute() {
			TextPositionInterection.Execute(Owner, Locations.Bottom);		  
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextPositionInterection.CanExecute(Owner, Locations.Bottom, out chk, out ret);
			Checked = chk;
			return ret;		  
		}
	}
	class Left : InteractionMethod {
		public Left(UserInteractionHelper owner)
			: base(owner) {
				EditorType = EditorTypes.RadioButton;
			Text = "Left";
			Image = UCIconsHelper.TextLeftIcon;
		}
		public override void Execute() {
			TextPositionInterection.Execute(Owner, Locations.Left);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextPositionInterection.CanExecute(Owner, Locations.Left, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class Right : InteractionMethod {
		public Right(UserInteractionHelper owner)
			: base(owner) {
				EditorType = EditorTypes.RadioButton;
			Text = "Right";
			Image = UCIconsHelper.TextRightIcon;
		}
		public override void Execute() {
			TextPositionInterection.Execute(Owner, Locations.Right);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextPositionInterection.CanExecute(Owner, Locations.Right, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
	class Default : InteractionMethod {
		public Default(UserInteractionHelper owner)
			: base(owner) {
				EditorType = EditorTypes.RadioButton;
			Text = "Default";
		}
		public override void Execute() {
			TextPositionInterection.Execute(Owner, Locations.Default);
		}
		public override bool CanExecute() {
			bool chk, ret;
			TextPositionInterection.CanExecute(Owner, Locations.Default, out chk, out ret);
			Checked = chk;
			return ret;
		}
	}
}
