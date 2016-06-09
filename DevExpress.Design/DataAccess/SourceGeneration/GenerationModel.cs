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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	using DevExpress.Design.CodeGenerator;
	public interface IModelGenerationServiceContainer : IModelServiceProvider, IDisposable {
		void Initialize(IServiceProvider externalServiceProvider);
	}	
	public interface IModelService {
		IModelItem CreateModelItem(object obj);
		IModelItem CreateModelItem(Type type);
		IModelItemExpression CreateModelItemExpression(IModelItem item, string expressionString);
	}
	public interface IModelEditingScope : IDisposable {
		void Complete();
	}
	public interface IEditingContext {
		IModelServiceProvider ServiceProvider { get; }
		IModelItem CreateItem(Type type);
		IModelItemExpression CreateExpression(IModelItem item, string expressionString);
	}
	public interface IModelProperty {
		string Name { get; }
		IModelItem Value { get; }
		IModelItem SetValue(object value);
		void ClearValue();
		object ReadLocalValue();
		IModelItemCollection Collection { get; }
	}
	public interface IModelPropertyCollection {
		IModelProperty this[string propertyName] { get; }
	}
	public interface IModelItem {
		string Name { get; }
		IModelEditingScope BeginEdit();
		IModelEditingScope BeginEdit(string description);
		IEditingContext EditingContext { get; }
		IModelPropertyCollection Properties { get; }
		Type ItemType { get; }
		object Value { get; }
	}
	public interface IModelItemExpression : IModelItem {
		string ExpressionString { get; }
	}
	public interface IModelItemCollection : IEnumerable<IModelItem> {
		void Add(object value);
		void Add(IModelItem valueItem);
		void Remove(object value);
		void Remove(IModelItem valueItem);
	}
	public static class NestedPropertyHelper {
		public static IModelProperty GetProperty(IModelItem modelItem, string path) {
			return GetPropertyCore(modelItem.Properties, path);
		}
		public static void Clear(IModelItem modelItem, string path) {
			IModelProperty property = GetPropertyCore(modelItem.Properties, path);
			if(property != null) property.ClearValue();
		}
		public static void Set(IModelItem modelItem, string path, object value) {
			IModelProperty property = GetPropertyCore(modelItem.Properties, path);
			if(property != null) property.SetValue(value);
		}
		static IModelProperty GetPropertyCore(IModelPropertyCollection properties, string path) {
			if(string.IsNullOrEmpty(path)) return null;
			int pathSeparatorPos = path.IndexOf('.');
			if(pathSeparatorPos > 0) {
				string rootPath = path.Substring(0, pathSeparatorPos);
				path = path.Substring(pathSeparatorPos + 1);
				IModelProperty rootDescriptor = properties[rootPath];
				if(rootDescriptor != null)
					return GetPropertyCore(rootDescriptor.Value.Properties, path);
			}
			return properties[path];
		}
	}
}
