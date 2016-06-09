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
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Printing.Native {
	public class LobControl : ContentControl {
		public static readonly DependencyProperty HeaderProperty =
			DependencyPropertyManager.Register("Header", typeof(Label), typeof(LobControl), new UIPropertyMetadata(HeaderChanged));
		public static readonly DependencyProperty HeaderSizeGroupProperty =
			DependencyPropertyManager.Register("HeaderSizeGroup", typeof(SharedSizeGroup), typeof(LobControl), new UIPropertyMetadata(HeaderSizeGroupChanged));
		public Label Header {
			get { return (Label)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public SharedSizeGroup HeaderSizeGroup {
			get { return (SharedSizeGroup)GetValue(HeaderSizeGroupProperty); }
			set { SetValue(HeaderSizeGroupProperty, value); }
		}
#if !SILVERLIGHT
		static LobControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LobControl), new FrameworkPropertyMetadata(typeof(LobControl)));
		}
#else
		public LobControl() {
			DefaultStyleKey = typeof(LobControl);
		}
#endif
		static void HeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LobControl lobControl = d as LobControl;
			if(lobControl.HeaderSizeGroup == null)
				return;
			Label oldHeader = e.OldValue as Label;
			Label newHeader = e.NewValue as Label;
			if(oldHeader != null)
				lobControl.HeaderSizeGroup.UnregisterElement(oldHeader);
			if(newHeader != null)
				lobControl.HeaderSizeGroup.RegisterElement(newHeader); 
		}
		static void HeaderSizeGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			LobControl lobControl = d as LobControl;
			if(lobControl.Header == null)
				return;
			SharedSizeGroup oldSizeGroup = e.OldValue as SharedSizeGroup;
			SharedSizeGroup newSizeGroup = e.NewValue as SharedSizeGroup;
			if(oldSizeGroup != null)
				oldSizeGroup.UnregisterElement(lobControl.Header);
			if(newSizeGroup != null)
				newSizeGroup.RegisterElement(lobControl.Header);
		}
	}
}
