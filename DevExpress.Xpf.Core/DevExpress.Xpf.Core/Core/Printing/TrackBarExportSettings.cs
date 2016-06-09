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

namespace DevExpress.Xpf.Printing {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Windows;
	public static class TrackBarExportSettings {
		public static readonly DependencyProperty PositionProperty;
		public static readonly DependencyProperty MinimumProperty;
		public static readonly DependencyProperty MaximumProperty;
		static TrackBarExportSettings() {
			Type ownerType = typeof(TrackBarExportSettings);
			PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(int), ownerType, new PropertyMetadata(ExportSettingDefaultValue.TrackBarPosition));
			MinimumProperty = DependencyProperty.RegisterAttached("Minimum", typeof(int), ownerType, new PropertyMetadata(ExportSettingDefaultValue.TrackBarMinimum));
			MaximumProperty = DependencyProperty.RegisterAttached("Maximum", typeof(int), ownerType, new PropertyMetadata(ExportSettingDefaultValue.TrackBarMaximum));
		}
		public static int GetPosition(DependencyObject d) {
			return (int)d.GetValue(PositionProperty);
		}
		public static void SetPosition(DependencyObject d, int value) {
			d.SetValue(PositionProperty, value);
		}
		public static int GetMinimum(DependencyObject d) {
			return (int)d.GetValue(MinimumProperty);
		}
		public static void SetMinimum(DependencyObject d, int value) {
			d.SetValue(MinimumProperty, value);
		}
		public static int GetMaximum(DependencyObject d) {
			return (int)d.GetValue(MaximumProperty);
		}
		public static void SetMaximum(DependencyObject d, int value) {
			d.SetValue(MaximumProperty, value);
		}
	}
}
