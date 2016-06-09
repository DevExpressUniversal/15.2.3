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
using System.ComponentModel;
using System.Resources;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Data;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Office.Internal {
	#region LocalizationHelper
	public static class OfficeLocalizationHelper {
		public static ResourceManager CreateResourceManager(Type resourceFinder) {
			string resourceFileName = DXDisplayNameAttribute.DefaultResourceFile;
			return new System.Resources.ResourceManager(string.Concat(resourceFinder.Namespace, ".", resourceFileName), resourceFinder.Assembly);
		}
		public static string GetString(ResourceManager resourceManager, string fullName, object value) {
			string key = String.Concat(fullName, ".", value);
			return resourceManager.GetString(key) ?? value.ToString();
		}
		public static string GetPaperKindString(ResourceManager resourceManager, PaperKind value) {
			string fullName = "System.Drawing.Printing." + value.GetType().Name;
			return GetString(resourceManager, fullName, value);
		}
	}
	#endregion
}
namespace DevExpress.Office.Internal {
	#region DependencyPropertyChangeHandler
	public class DependencyPropertyChangeHandler : DependencyObject {
		class HandlersTable : Dictionary<string, DependencyPropertyChangeHandler> {
		}
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		private static readonly DependencyProperty TargetPropertyProperty = CreateTargetPropertyProperty();
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		private static readonly DependencyProperty HandlersProperty = CreateHandlersProperty();
		static DependencyProperty CreateHandlersProperty() {
			return DependencyProperty.RegisterAttached("Handlers", typeof(HandlersTable), typeof(DependencyPropertyChangeHandler), new PropertyMetadata(null));
		}
		static DependencyProperty CreateTargetPropertyProperty() {
			return DependencyProperty.Register("TargetProperty", typeof(object), typeof(DependencyPropertyChangeHandler), new PropertyMetadata(null, OnTargetPropertyChanged));
		}
		static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DependencyPropertyChangeHandler handler = d as DependencyPropertyChangeHandler;
			if (handler != null)
				handler.OnTargetPropertyChanged();
		}
		public static void AddHandler(DependencyObject element, string property, Action action) {
			Binding binding = new Binding(property);
			binding.Source = element;
			DependencyPropertyChangeHandler handler = new DependencyPropertyChangeHandler(action);
			HandlersTable table = element.GetValue(DependencyPropertyChangeHandler.HandlersProperty) as HandlersTable;
			if (table == null) {
				table = new HandlersTable();
				element.SetValue(DependencyPropertyChangeHandler.HandlersProperty, table);
			}
			table[property] = handler;
			handler.LockHandling();
			BindingOperations.SetBinding(handler, TargetPropertyProperty, binding);
			handler.UnlockHandling();
		}
		public static void AddHandler(DependencyObject element, DependencyProperty attachedProperty, Action action) {
			Binding binding = new Binding();
			binding.Path = new PropertyPath("(0)", attachedProperty);
			binding.Source = element;
			DependencyPropertyChangeHandler handler = new DependencyPropertyChangeHandler(action);
			HandlersTable table = element.GetValue(DependencyPropertyChangeHandler.HandlersProperty) as HandlersTable;
			if (table == null) {
				table = new HandlersTable();
				element.SetValue(DependencyPropertyChangeHandler.HandlersProperty, table);
			}
			table[GetPropertyName(attachedProperty)] = handler;
			handler.LockHandling();
			BindingOperations.SetBinding(handler, TargetPropertyProperty, binding);
			handler.UnlockHandling();
		}
		public static void RemoveHandler(DependencyObject element, string property) {
			HandlersTable table = element.GetValue(HandlersProperty) as HandlersTable;
			if (table == null || !table.ContainsKey(property))
				return;
			DependencyPropertyChangeHandler handler = table[property];
			handler.LockHandling();
			handler.ClearValue(DependencyPropertyChangeHandler.TargetPropertyProperty);
			handler.UnlockHandling();
			table.Remove(property);
		}
		public static void RemoveHandler(DependencyObject element, DependencyProperty attachedProperty) {
			string propertyName = GetPropertyName(attachedProperty);
			RemoveHandler(element, propertyName);
		}
		static string GetPropertyName(DependencyProperty property) {
			return String.Format("{0}.{1}", property.OwnerType.Name, property.Name);
		}
		bool isHandlingLocked;
		readonly Action action;
		protected DependencyPropertyChangeHandler(Action action) {
			this.action = action;
		}
		void LockHandling() {
			this.isHandlingLocked = true;
		}
		void UnlockHandling() {
			this.isHandlingLocked = false;
		}
		void OnTargetPropertyChanged() {
			if (!this.isHandlingLocked)
				action();
		}
	}
	#endregion
}
