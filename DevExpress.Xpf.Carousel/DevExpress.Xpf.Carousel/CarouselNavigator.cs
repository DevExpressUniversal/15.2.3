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
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using System.ComponentModel;
using DevExpress.Xpf.Utils.Themes;
using System.Collections.Generic;
using System;
namespace DevExpress.Xpf.Carousel {
	[DXToolboxBrowsable, ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	public class CarouselNavigator : Control {
		delegate void CarouselAction();
		public static readonly DependencyProperty CarouselProperty;
		static CarouselNavigator() {
			CarouselProperty = DependencyProperty.Register("Carousel", typeof(CarouselPanel), typeof(CarouselNavigator), new FrameworkPropertyMetadata(null));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CarouselNavigator), new FrameworkPropertyMetadata(typeof(CarouselNavigator)));
		}
#if !SL
	[DevExpressXpfCarouselLocalizedDescription("CarouselNavigatorCarousel")]
#endif
		public CarouselPanel Carousel {
			get { return (CarouselPanel)GetValue(CarouselProperty); }
			set { SetValue(CarouselProperty, value); }
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new CarouselNavigatorAutomationPeer(this);
		}
	}
}
