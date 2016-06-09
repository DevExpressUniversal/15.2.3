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

using System.Windows;
using DevExpress.Xpf.Docking.VisualElements;
using SWC = System.Windows.Controls;
namespace DevExpress.Xpf.Docking.Platform {
	public class ShadowResizeBackground : psvControl {
		#region static
		static ShadowResizeBackground() {
			var dProp = new DependencyPropertyRegistrator<ShadowResizeBackground>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public ShadowResizeBackground() {
		}
	}
	public class ShadowResizePointer : psvControl {
		#region static
		static ShadowResizePointer() {
			var dProp = new DependencyPropertyRegistrator<ShadowResizePointer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public ShadowResizePointer() {
		}
	}
	public class AutoHideResizePointer : psvControl {
		#region static
		public static readonly DependencyProperty DockProperty;
		static AutoHideResizePointer() {
			var dProp = new DependencyPropertyRegistrator<AutoHideResizePointer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Dock", ref DockProperty, SWC.Dock.Left);
		}
		#endregion static
		public AutoHideResizePointer() {
		}
		public SWC.Dock Dock {
			get { return (SWC.Dock)GetValue(DockProperty); }
			set { SetValue(DockProperty, value); }
		}
	}
	public class FloatingResizePointer : psvControl {
		#region static
		static FloatingResizePointer() {
			var dProp = new DependencyPropertyRegistrator<FloatingResizePointer>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
		}
		#endregion static
		public FloatingResizePointer() {
		}
	}
}
