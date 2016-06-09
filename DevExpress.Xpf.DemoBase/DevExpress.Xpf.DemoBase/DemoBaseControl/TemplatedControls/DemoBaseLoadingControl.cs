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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DemoData.Helpers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.DemoBase.Helpers;
namespace DevExpress.Xpf.DemoBase {
	class DemoBaseLoadingControl : Control {
		#region Dependency Properties
		public static readonly DependencyProperty LoadingInProgressProperty;
		public static readonly DependencyProperty LoadingBackgroundProperty;
		static DemoBaseLoadingControl() {
			Type ownerType = typeof(DemoBaseLoadingControl);
			LoadingInProgressProperty = DependencyProperty.Register("LoadingInProgress", typeof(bool), ownerType, new PropertyMetadata(false));
			LoadingBackgroundProperty = DependencyProperty.Register("LoadingBackground", typeof(LoadingSplashBackground), ownerType, new PropertyMetadata(LoadingSplashBackground.White));
		}
		#endregion
		public DemoBaseLoadingControl() {
			this.SetDefaultStyleKey(typeof(DemoBaseLoadingControl));
			FocusHelper2.SetFocusable(this, false);
			IsHitTestVisible = false;
		}
		public bool LoadingInProgress { get { return (bool)GetValue(LoadingInProgressProperty); } set { SetValue(LoadingInProgressProperty, value); } }
		public LoadingSplashBackground LoadingBackground { get { return (LoadingSplashBackground)GetValue(LoadingBackgroundProperty); } set { SetValue(LoadingBackgroundProperty, value); } }
	}
}
