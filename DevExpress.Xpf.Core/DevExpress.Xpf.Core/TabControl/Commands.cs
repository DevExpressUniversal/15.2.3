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
using System.Reflection;
using System.Windows.Controls;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows;
using DevExpress.Xpf.Core.Localization;
namespace DevExpress.Xpf.Core {
	public abstract class TabControlCommand : Command {
		public override string MenuCaption { get { return TabControlLocalizer.GetString(MenuCaptionStringId); } }
		public override string Description { get { return TabControlLocalizer.GetString(DescriptionStringId); } }
		protected internal abstract TabControlStringId MenuCaptionStringId { get; }
		protected internal abstract TabControlStringId DescriptionStringId { get; }
		protected internal DXTabControl Control { get { return control; } }
		protected TabControlCommand(DXTabControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		readonly DXTabControl control;
	}
	#region Scroll Commands
	public class TabControlScrollPrevCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			((TabControlScrollView)Control.View).ScrollPrev();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_ScrollPrev; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_ScrollPrevDescription; } }
		protected TabControlScrollPrevCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.View is TabControlScrollView;
			if(!(Control.View is TabControlScrollView))
				return;
			state.Enabled = ((TabControlScrollView)Control.View).CanScrollPrev;
		}
	}
	public class TabControlScrollNextCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			((TabControlScrollView)Control.View).ScrollNext();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_ScrollNext; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_ScrollNextDescription; } }
		protected TabControlScrollNextCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.View is TabControlScrollView;
			if(!(Control.View is TabControlScrollView))
				return;
			state.Enabled = ((TabControlScrollView)Control.View).CanScrollNext;
		}
	}
	public class TabControlScrollToSelectedTabItemCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			((TabControlScrollView)Control.View).ScrollToSelectedTabItem();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_ScrollToSelectedTabItem; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_ScrollToSelectedTabItemDescription; } }
		protected TabControlScrollToSelectedTabItemCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.View is TabControlScrollView;
			if(!(Control.View is TabControlScrollView))
				return;
			state.Enabled = ((TabControlScrollView)Control.View).CanScrollToSelectedTabItem;
		}
	}
	public class TabControlScrollFirstCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			((TabControlScrollView)Control.View).ScrollFirst();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_ScrollFirst; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_ScrollFirstDescription; } }
		protected TabControlScrollFirstCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.View is TabControlScrollView;
			if(!(Control.View is TabControlScrollView))
				return;
			state.Enabled = ((TabControlScrollView)Control.View).CanScrollPrev;
		}
	}
	public class TabControlScrollLastCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			((TabControlScrollView)Control.View).ScrollLast();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_ScrollLast; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_ScrollLastDescription; } }
		protected TabControlScrollLastCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.View is TabControlScrollView;
			if(!(Control.View is TabControlScrollView))
				return;
			state.Enabled = ((TabControlScrollView)Control.View).CanScrollNext;
		}
	}
	#endregion Scroll Commands
	#region Select Commands
	public class TabControlSelectPrevCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			Control.SelectPrev();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_SelectPrev; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_SelectPrevDescription; } }
		protected TabControlSelectPrevCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.SelectedIndex != -1;
			state.Enabled = Control.SelectedIndex > 0;
		}
	}
	public class TabControlSelectNextCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			Control.SelectNext();
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_SelectNext; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_SelectNextDescription; } }
		protected TabControlSelectNextCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = Control.SelectedIndex != -1;
			state.Enabled = Control.SelectedIndex < Control.Items.Count - 1;
		}
	}
	#endregion Select Commands
	public class TabControlHideSelectedItemCommand : TabControlCommand {
		public override void ForceExecute(ICommandUIState state) {
			Control.HideTabItem(Control.SelectedIndex);
		}
		protected internal override TabControlStringId MenuCaptionStringId { get { return TabControlStringId.MenuCmd_HideSelectedItem; } }
		protected internal override TabControlStringId DescriptionStringId { get { return TabControlStringId.MenuCmd_HideSelectedItemDescription; } }
		protected TabControlHideSelectedItemCommand(DXTabControl control) : base(control) { }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = state.Visible = Control.SelectedIndex != -1;
		}
	}
}
