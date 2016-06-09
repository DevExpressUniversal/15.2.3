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

using DevExpress.Xpf.WindowsUI.Internal;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
namespace DevExpress.Xpf.WindowsUI.UIAutomation {
	public class CommandButtonAutomationPeer : ButtonAutomationPeer {
		public CommandButtonAutomationPeer(CommandButton owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "CommandButton";
		}
	}
	public class AppBarButtonAutomationPeer : CommandButtonAutomationPeer {
		public AppBarButtonAutomationPeer(AppBarButton owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "AppBarButton";
		}
	}
	public class BackButtonAutomationPeer : CommandButtonAutomationPeer {
		public BackButtonAutomationPeer(BackButton owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "BackButton";
		}
		protected override string GetNameCore() {
			return "Back";
		}
	}
	public class ForwardButtonAutomationPeer : CommandButtonAutomationPeer {
		public ForwardButtonAutomationPeer(ForwardButton owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "ForwardButton";
		}
		protected override string GetNameCore() {
			return "Forward";
		}
	}
	public class AppBarToggleButtonAutomationPeer : AppBarButtonAutomationPeer, IToggleProvider {
		static ToggleState ConvertToToggleState(bool value) {
			switch(value) {
				case true:
					return ToggleState.On;
				default:
					return ToggleState.Off;
			}
		}
		AppBarToggleButton AppBarToggleButton { get { return base.Owner as AppBarToggleButton; } }
		public AppBarToggleButtonAutomationPeer(AppBarToggleButton owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			return "AppBarToggleButton";
		}
		public override object GetPattern(PatternInterface patternInterface) {
			return patternInterface == PatternInterface.Toggle ? this : base.GetPattern(patternInterface);
		}
		internal void RaiseToggleStateChangedEvent(bool oldValue, bool newValue) {
			if(oldValue != newValue) {
				RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(oldValue), ConvertToToggleState(newValue));
			}
		}
		#region IToggleProvider Members
		public void Toggle() {
			if(!IsEnabled()) {
				throw new ElementNotEnabledException();
			}
			AppBarToggleButton button = AppBarToggleButton;
			if(button != null)
				button.OnToggle();
		}
		public ToggleState ToggleState {
			get { return ConvertToToggleState(AppBarToggleButton.IsChecked); }
		}
		#endregion
	}
}
