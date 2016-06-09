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
using System.ComponentModel;
using System.Windows;
using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core;
using System.Reflection;
using DevExpress.Data.Utils;
using System.Windows.Controls;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public static class ManagerHelper {
		public static bool ShowDialog(object viewModel, string description, IDialogService service, Action<CancelEventArgs> executeMethod = null) {
			return ManagerHelperBase.ShowDialog(viewModel, description, service, executeMethod);
		}
		public static void SetProperty(DependencyObject dependencyObject, DependencyProperty property, object value) {
			ManagerHelperBase.SetProperty(dependencyObject, property, value);
		}
		public static string ConvertExpression(string expression, IDataColumnInfo info, Func<IDataColumnInfo, string, string> convertFunc) {
			return string.IsNullOrEmpty(expression) ? null : convertFunc(info, expression);
		}
		public static object SafeGetValue(int index, object[] values) {
			return index < values.Length ? values[index] : null;
		}
	}
	public static class ManagerExtensions {
		public static ManagerItemViewModel Init(this ManagerItemViewModel item, ManagerViewModel vm) {
			item.SetOwner(vm);
			return item;
		}
	}
	public class GridAssemblyHelper {
		static GridAssemblyHelper instance;
		IGridManagerFactory gridFactory;
		const string managerAssembly = "DevExpress.Xpf.Core.ConditionalFormattingManager";
		const string factoryTypeName = "GridManagerFactory";
		GridAssemblyHelper() {
			gridFactory = CreateFactory();
		}
		IGridManagerFactory CreateFactory() {
			Assembly gridAssembly = Helpers.LoadWithPartialName(AssemblyInfo.SRAssemblyXpfGrid + ", Version=" + AssemblyInfo.Version);
			if(gridAssembly == null)
				return null;
			Type type = gridAssembly.GetType(String.Format("{0}.{1}", managerAssembly, factoryTypeName));
			if(type == null)
				return null;
			return Activator.CreateInstance(type) as IGridManagerFactory;
		}
		public static GridAssemblyHelper Instance {
			get {
				if(instance == null)
					instance = new GridAssemblyHelper();
				return instance;
			}
		}
		public bool IsGridAvailable { get { return gridFactory != null; } }
		public UIElement CreateGrid() {
			return CreateIfAvailable(() => gridFactory.CreateGrid());
		}
		public ContentControl CreatePreviewControl() {
			return CreateIfAvailable(() => gridFactory.CreatePreviewControl());
		}
		T CreateIfAvailable<T>(Func<T> creator) where T : class {
			return IsGridAvailable ? creator() : null;
		}
	}
	public interface IGridManagerFactory {
		UIElement CreateGrid();
		ContentControl CreatePreviewControl();
	}
}
