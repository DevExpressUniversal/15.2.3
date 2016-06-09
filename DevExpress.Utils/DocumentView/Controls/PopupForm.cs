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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DocumentView.Controls {
	public interface IPopupForm {
		AppearanceObject AppearanceObject { get; }
		bool ShowToolTips { get; set; }
		bool IsPopupOwner(object obj);
		void ShowText(string text);
		void ShowText(string text, Point pt, MarginSide side, object popupOwner);
		void HidePopup();
	}
	public class PopupForm : ToolTipController, IPopupForm {
		object popupOwner;
		bool showToolTips = true;
		Point suspendedLocation;
		Point location = Point.Empty;
		object suspendedPopupOwner;
		System.Windows.Forms.Control control;
		public Point Location {
			get { return location; }
			set { location = value; }
		}
		public bool Visible {
			get { return CurrentShowArgs != null; }
		}
		public PopupForm(System.Windows.Forms.Control control) {
			this.control = control;
			this.ToolTipType = Utils.ToolTipType.SuperTip;
			Appearance.Changed += Appearance_Changed;
		}
		void Appearance_Changed(object sender, System.EventArgs e) {
			AppearanceObject appearence = ((AppearanceObject)sender);
			if(!IsDefaultAppearanceSettings(appearence) && ToolTipStyle != DevExpress.Utils.ToolTipStyle.WindowsXP) {
				this.ToolTipStyle = DevExpress.Utils.ToolTipStyle.WindowsXP;
				this.ToolTipType = Utils.ToolTipType.Standard;
				DestroyToolWindow();
			} else if(IsDefaultAppearanceSettings(appearence) && ToolTipStyle != DevExpress.Utils.ToolTipStyle.Default) {
				this.ToolTipStyle = DevExpress.Utils.ToolTipStyle.Default;
				this.ToolTipType = Utils.ToolTipType.SuperTip;
				DestroyToolWindow();
			}
		}
		bool IsDefaultAppearanceSettings(AppearanceObject appearence) {
			return appearence.BackColor == Color.Empty && appearence.ForeColor == Color.Empty && appearence.Font == AppearanceObject.DefaultFont;
		}
		#region IPopupForm Members
		public AppearanceObject AppearanceObject {
			get { return Appearance; }
		}
		public bool ShowToolTips {
			get {
				return showToolTips;
			}
			set {
				if(showToolTips == value)
					return;
				showToolTips = value;
				if(showToolTips) {
					SetLocation(suspendedLocation, suspendedPopupOwner);
				} else {
					SuspendLocation(Location, popupOwner);
					popupOwner = null;
				}
			}
		}
		public bool IsPopupOwner(object obj) {
			return (obj != null) ? Equals(obj, popupOwner) : false;
		}
		public void ShowText(string text) {
			ShowTextCore(text);
		}
		public void ShowText(string text, Point pt, MarginSide side, object popupOwner) {
			Point position = pt;
			if(side == MarginSide.Left) {
				position.X -= Cursor.Current.Size.Width / 2;
				ToolTipLocation = Utils.ToolTipLocation.LeftCenter;
			} else if(side == MarginSide.Right) {
				ToolTipLocation = Utils.ToolTipLocation.RightCenter;
			} else if(side == MarginSide.Top) {
				position.Y -= Cursor.Current.Size.Height / 2;
				ToolTipLocation = Utils.ToolTipLocation.TopCenter;
			} else if(side == MarginSide.Bottom) {
				ToolTipLocation = Utils.ToolTipLocation.BottomCenter;
			}
			if(showToolTips) {
				SetLocation(position, popupOwner);
				ShowTextCore(text);
			} else {
				SuspendLocation(position, popupOwner);
			}
		}
		public void HidePopup() {
			if(Visible) {
				HideHint();
				popupOwner = null;
			}
		}
		#endregion
		void ShowTextCore(string text) {
			ToolTipControllerShowEventArgs clone = CreateShowArgs();
			clone.ToolTip = text;
			clone.SelectedObject = clone.SelectedControl = control;
			ShowHint(clone, Location);
		}
		void SuspendLocation(Point location, object popupOwner) {
			suspendedLocation = location;
			suspendedPopupOwner = popupOwner;
			HideHint();
		}
		void SetLocation(Point location, object popupOwner) {
			Location = location;
			this.popupOwner = popupOwner;
		}
	}
}
