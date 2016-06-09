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
#if !SL
using DependencyPropertyManager = System.Windows.DependencyProperty;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public enum StoreLayoutMode {
		Appearance = 1, DataSettings = 2, VisualOptions = 4, Layout = 8, PrintSettings = 16,
		AllOptions = Appearance | DataSettings | VisualOptions | Layout | PrintSettings
	};
	public class PivotSerializationOptions {
		public const int StoreAlwaysID = 0, AppearanceID = 1, DataSettingsID = 2, VisualOptionsID = 3, LayoutID = 4,
			PrintSettingsID = 5;
		public static readonly DependencyProperty RemoveOldFieldsProperty;
		public static readonly DependencyProperty AddNewFieldsProperty;
		public static readonly DependencyProperty AddNewGroupsProperty;
		public static readonly DependencyProperty StoreLayoutModeProperty;
		static PivotSerializationOptions() {
			Type ownerType = typeof(PivotSerializationOptions);
			RemoveOldFieldsProperty = DependencyPropertyManager.RegisterAttached("RemoveOldFields", typeof(bool), 
				ownerType, new UIPropertyMetadata(true));
			AddNewFieldsProperty = DependencyPropertyManager.RegisterAttached("AddNewFields", typeof(bool), 
				ownerType, new UIPropertyMetadata(true));
			AddNewGroupsProperty = DependencyPropertyManager.RegisterAttached("AddNewGroups", typeof(bool),
				ownerType, new UIPropertyMetadata(true));
			StoreLayoutModeProperty = DependencyPropertyManager.RegisterAttached("StoreLayoutMode", typeof(StoreLayoutMode),
				ownerType, new UIPropertyMetadata(StoreLayoutMode.AllOptions));
		}
		public static bool GetAddNewFields(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewFieldsProperty);
		}
		public static void SetAddNewFields(DependencyObject obj, bool value) {
			obj.SetValue(AddNewFieldsProperty, value);
		}
		public static bool GetRemoveOldFields(DependencyObject obj) {
			return (bool)obj.GetValue(RemoveOldFieldsProperty);
		}
		public static void SetRemoveOldFields(DependencyObject obj, bool value) {
			obj.SetValue(RemoveOldFieldsProperty, value);
		}
		public static bool GetAddNewGroups(DependencyObject obj) {
			return (bool)obj.GetValue(AddNewGroupsProperty);
		}
		public static void SetAddNewGroups(DependencyObject obj, bool value) {
			obj.SetValue(AddNewGroupsProperty, value);
		}
		public static StoreLayoutMode GetStoreLayoutMode(DependencyObject obj) {
			return (StoreLayoutMode)obj.GetValue(StoreLayoutModeProperty);
		}
		public static void SetStoreLayoutMode(DependencyObject obj, StoreLayoutMode value) {
			obj.SetValue(StoreLayoutModeProperty, value);
		}
	}
}
