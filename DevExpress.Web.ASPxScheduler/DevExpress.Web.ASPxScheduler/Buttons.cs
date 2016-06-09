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

using System.ComponentModel;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxScheduler.Controls { 
	using DevExpress.Web.ASPxScheduler.Internal;
	public class NoBorderButton : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new NoBorderButtonStyles(this);
		}
	}
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	[ToolboxItem(false)]
	public abstract class CustomButtonBase : ASPxButton {
		protected CustomButtonBase()
			: base() {
			this.EnableClientSideAPI = true;
		}
		protected override bool AutoPostBackInternal { get { return false; } set { } }
		public override bool EnableViewState { get { return false; } set { } }
		public override bool UseSubmitBehavior { get { return false; } set { } }
		public override bool CausesValidation { get { return false; } }
	}
	public class ViewNavigatorButton : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new ViewNavigatorButtonStyles(this);
		}
	}
	public class ViewNavigatorGotoDateButton : ViewNavigatorButton {
		protected override StylesBase CreateStyles() {
			return new ViewNavigatorGotoDateButtonStyles(this);
		}
	}
	public class ResourceNavigatorButton : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new ResourceNavigatorButtonStyles(this);
		}
	}
	public class NavigationButtonInternal : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new NavigationButtonStyles(this);
		}
	}
	public class DayViewTopMoreButtonInternal : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new DayViewTopMoreButtonStyles(this);
		}
	}
	public class DayViewBottomMoreButtonInternal : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new DayViewBottomMoreButtonStyles(this);
		}
	}
	public class ViewSelectorButton : CustomButtonBase {
		protected override StylesBase CreateStyles() {
			return new ViewSelectorButtonStyles(this);
		}
	}
	#region Styles
	public abstract class CustomButtonStylesBase : ButtonControlStyles {
		protected CustomButtonStylesBase(ISkinOwner owner)
			: base(owner) {
		}
		protected override AppearanceStyleBase GetDefaultButtonContentDivStyle() {
			return new AppearanceStyleBase();
		}
	}
	public class ViewNavigatorButtonStyles : CustomButtonStylesBase {
		public ViewNavigatorButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscViewNavigator";
		}
	}
	public class ViewNavigatorGotoDateButtonStyles : ViewNavigatorButtonStyles {
		public ViewNavigatorGotoDateButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscViewNavigatorGotoDate";
		}
	}
	public class ResourceNavigatorButtonStyles : CustomButtonStylesBase {
		public ResourceNavigatorButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscResourceNavigator";
		}
	}
	public class NavigationButtonStyles : CustomButtonStylesBase {
		public NavigationButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscNavigation";
		}
	}
	public class DayViewTopMoreButtonStyles : CustomButtonStylesBase {
		public DayViewTopMoreButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscDVTopMore";
		}
	}
	public class DayViewBottomMoreButtonStyles : CustomButtonStylesBase {
		public DayViewBottomMoreButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscDVBottomMore";
		}
	}
	public class ViewSelectorButtonStyles : CustomButtonStylesBase {
		public ViewSelectorButtonStyles(ISkinOwner owner)
			: base(owner) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscViewSelector";
		}
	}
	public class NoBorderButtonStyles : CustomButtonStylesBase {
		public NoBorderButtonStyles(ISkinOwner button)
			: base(button) {
		}
		protected override string GetCssClassNamePrefix() {
			return "dxscNoBorder";
		}
	}
	#endregion
}
