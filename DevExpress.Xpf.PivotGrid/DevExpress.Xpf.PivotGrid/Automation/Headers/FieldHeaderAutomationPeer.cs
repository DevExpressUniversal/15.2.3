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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using DevExpress.Xpf.PivotGrid.Internal;
namespace DevExpress.Xpf.PivotGrid.Automation {
	public class FieldHeaderAutomationPeer : PivotGridAutomationPeerBase, IDockProvider {
		public FieldHeaderAutomationPeer(PivotGridControl pivotGrid, FieldHeader header)
			: base(pivotGrid, header) {
			Header = header;
		}
		protected FieldHeader Header { get; set; }
		protected override string GetNameCore() {
			return Header.ToString();
		}
		protected override AutomationControlType GetAutomationControlTypeCore() {
			return AutomationControlType.HeaderItem;
		}
		public override object GetPattern(PatternInterface patternInterface) {
			if(patternInterface == PatternInterface.Dock)
				return this;
			return null;
		}
		#region IDockProvider Members
		System.Windows.Automation.DockPosition IDockProvider.DockPosition {
			get {
				switch(Header.HeaderPosition) {
					case HeaderPosition.Left:
						return System.Windows.Automation.DockPosition.Left;
					case HeaderPosition.Single:
						return System.Windows.Automation.DockPosition.None;
					case HeaderPosition.Right:
						return System.Windows.Automation.DockPosition.Right;
					case HeaderPosition.Middle:
						return System.Windows.Automation.DockPosition.None;
					default:
						throw new NotImplementedException();
				}
			}
		}
		void IDockProvider.SetDockPosition(System.Windows.Automation.DockPosition dockPosition) { }
		#endregion
	}
}
