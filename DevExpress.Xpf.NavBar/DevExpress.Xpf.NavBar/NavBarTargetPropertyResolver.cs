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
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
using DevExpress.Xpf.NavBar.Platform;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils.Native;
namespace DevExpress.Xpf.NavBar {
	internal static class AttachedProperties {
		static AttachedProperties() {
			Properties = new Dictionary<string, DependencyProperty>();
			Properties.Add("NavBarAnimationOptions.HorizontalExpand", NavBarAnimationOptions.HorizontalExpandProperty);
			Properties.Add("NavBarAnimationOptions.VerticalExpand", NavBarAnimationOptions.VerticalExpandProperty);
			Properties.Add("XPFDockPanel.Dock", XPFDockPanel.DockProperty);
			Properties.Add("NavigationPaneView.Element", NavigationPaneView.ElementProperty);
			Properties.Add("CursorHelper.Cursor", CursorHelper.CursorProperty);
		}
		public static Dictionary<string, DependencyProperty> Properties { get; set; }
	}
	public class NavBarTargetPropertyResolver : TargetPropertyUpdater {
		public NavBarTargetPropertyResolver(FrameworkElement owner)
			: base(owner) { }
		protected override DependencyProperty GetDependencyPropertyByPath(string path) {
			if(!AttachedProperties.Properties.ContainsKey(path))
				return null;
			return AttachedProperties.Properties[path];
		}
	}
	public class NavBarTargetPropertyResolverFactory : TargetPropertyUpdaterFactory {
		public override TargetPropertyUpdater CreateTargetPropertyResolver(FrameworkElement owner) {
			return new NavBarTargetPropertyResolver(owner);
		}
	}
	public class NavBarVisualStateHelper : VisualStateHelper {
		public static void UpdateStates(FrameworkElement owner, string groupName) {
			UpdateStates(owner, groupName, AttachedProperties.Properties);
		}
	}
}
