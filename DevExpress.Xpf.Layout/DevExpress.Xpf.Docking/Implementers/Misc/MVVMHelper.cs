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
namespace DevExpress.Xpf.Docking {
	public interface IMVVMDockingProperties {
		string TargetName { get; set; }
	}
	public interface ILayoutAdapter {
		string Resolve(DockLayoutManager owner, object item);
	}
	public class MVVMHelper {
		public static readonly DependencyProperty TargetNameProperty;
		public static readonly DependencyProperty LayoutAdapterProperty;
		static MVVMHelper() {
			var dProp = new DependencyPropertyRegistrator<MVVMHelper>();
			dProp.RegisterAttached("TargetName", ref TargetNameProperty, (string)null, OnTargetNameChanged);
			dProp.RegisterAttached("LayoutAdapter", ref LayoutAdapterProperty, (ILayoutAdapter)null);
		}
		public static string GetTargetName(DependencyObject target) {
			return (string)target.GetValue(TargetNameProperty);
		}
		public static void SetTargetName(DependencyObject target, string value) {
			target.SetValue(TargetNameProperty, value);
		}
		static void OnTargetNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			OnTargetNameChanged(o, (string)e.OldValue, (string)e.NewValue);
		}
		static void OnTargetNameChanged(DependencyObject o, string oldValue, string newValue) {
		}
		internal static string GetTargetNameForItem(object target) {
			if(target is IMVVMDockingProperties) return ((IMVVMDockingProperties)target).TargetName;
			return target is DependencyObject ? GetTargetName((DependencyObject)target) : null;
		}
		public static ILayoutAdapter GetLayoutAdapter(DependencyObject target) {
			return (ILayoutAdapter)target.GetValue(LayoutAdapterProperty);
		}
		public static void SetLayoutAdapter(DependencyObject target, ILayoutAdapter value) {
			target.SetValue(LayoutAdapterProperty, value);
		}
	}
	class LayoutAdapter : ILayoutAdapter {
		static ILayoutAdapter _Instance;
		public static ILayoutAdapter Instance {
			get {
				if(_Instance == null) _Instance = new LayoutAdapter();
				return _Instance;
			}
		}
		#region ILayoutAdapter Members
		string ILayoutAdapter.Resolve(DockLayoutManager owner, object item) {
			return MVVMHelper.GetTargetNameForItem(item);
		}
		#endregion
	}
}
