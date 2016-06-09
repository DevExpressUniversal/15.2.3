#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Model.Core {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public delegate object ModelValueDefaultCalculator(ModelNode node);
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelValueCalculator {
		object Calculate(ModelNode node, string propertyName);
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class ModelValuePersistentPathCalculator : IModelValueCalculator {
		const string HelperValueNamePostFix = "_ID";
		ModelValueDefaultCalculator defaultCalculator;
		public ModelValuePersistentPathCalculator() : this(null) { }
		public ModelValuePersistentPathCalculator(ModelValueDefaultCalculator defaultCalculator) {
			this.defaultCalculator = defaultCalculator;
		}
		private object RunDefaultCalculator(ModelNode node) {
			return defaultCalculator != null ? defaultCalculator(node) : null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object Calculate(ModelNode originalNode, string propertyName) {
			return Calculate(originalNode, propertyName, true, false);
		}
		internal object Calculate(ModelNode originalNode, string propertyName, bool runDefaultCalculator, bool inThisLayer) {
			Guard.ArgumentNotNull(originalNode, "originalNode");
			Guard.ArgumentNotNullOrEmpty(propertyName, "propertyName");
			ModelValueInfo info = originalNode.GetValueInfo(propertyName);
			if(info == null || string.IsNullOrEmpty(info.PersistentPath)) {
				return runDefaultCalculator ? RunDefaultCalculator(originalNode) : null;
			}
			string helperValueName = GetHelperValueName(propertyName);
			bool hasValue = inThisLayer ? originalNode.HasValueInThisLayer(helperValueName) : originalNode.HasValue(helperValueName);
			if(!hasValue) {
				return runDefaultCalculator ? RunDefaultCalculator(originalNode) : null;
			}
			string targetNodeId = originalNode.GetValue<string>(helperValueName);
			object value = ModelNodePersistentPathHelper.FindValueByPath(originalNode, info.PersistentPath);
			ModelNode node = value as ModelNode;
			if(node != null) {
				return node.GetNode(targetNodeId);
			}
			IModelList list = value as IModelList;
			if(list != null) {
				return list[targetNodeId];
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetHelperValueName(string realValueName) {
			return realValueName + HelperValueNamePostFix;
		}
		internal static string GetRealValueName(string helperValueName) {
			return helperValueName.Substring(0, helperValueName.Length - HelperValueNamePostFix.Length);
		}
		internal static bool IsHelperValueName(string valueName) {
			return valueName.EndsWith(HelperValueNamePostFix);
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public static class ModelNodePersistentPathHelper {
		private const string ThisNodeName = "this";
		private const string RootNodeName = "Application";
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static object FindValueByPath(ModelNode modelNode, string persistentPath) {
			Guard.ArgumentNotNull(modelNode, "modelNode");
			Guard.ArgumentNotNullOrEmpty(persistentPath, "persistentPath");
			if(persistentPath == ThisNodeName) {
				return modelNode;
			}
			string[] pathItems = persistentPath.Split('.');
			int firstItemIndex = 0;
			object value = modelNode;
			if(pathItems[0] == RootNodeName) {
				firstItemIndex = 1;
				value = modelNode.Root;
			}
			for(int i = firstItemIndex; i < pathItems.Length; ++i) {
				string pathItem = pathItems[i];
				ModelNode node = value as ModelNode;
				if(node == null) {
					return null;
				}
				value = node.GetNode(pathItem);
				if(value == null) {
					value = node.GetValue(pathItem);
				}
			}
			return value;
		}
	}
	static class ModelNodePathHelper {
		const string PathSeparator = "/";
		const string RootNodeName = "Application";
		internal static string GetNodePath(ModelNode node) {
			Guard.ArgumentNotNull(node, "node");
			if(!node.IsRoot) {
				System.Text.StringBuilder path = new System.Text.StringBuilder(node.Id);
				for(ModelNode item = node.Parent; !item.IsRoot; item = item.Parent) {
					path.Insert(0, PathSeparator).Insert(0, item.Id);
				}
				path.Insert(0, PathSeparator).Insert(0, RootNodeName);
				return path.ToString();
			}
			return RootNodeName;
		}
		internal static ModelNode GetNodeByPath(ModelNode node, string path) {
			Guard.ArgumentNotNull(node, "node");
			Guard.ArgumentNotNullOrEmpty(path, "path");
			string[] items = path.Split(new string[] { PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
			ModelNode sourceNode = items[0] == RootNodeName ? node.Root : node.GetNode(items[0]);
			for(int i = 1; i < items.Length; ++i) {
				if(sourceNode == null) {
					return null;
				}
				sourceNode = sourceNode.GetNode(items[i]);
			}
			return sourceNode;
		}
	}
	public class StringToTypeConverterBase : ReferenceConverter {
		public StringToTypeConverterBase() : base(typeof(Type)) { }
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				if(value != null) {
					return ((Type)value).FullName;
				} else {
					return CaptionHelper.NoneValue;
				}
			}
			return null;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
			return value != null ? ReflectionHelper.FindType(value.ToString()) : null;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class StringToTypeConverter : StringToTypeConverterBase {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<Type> list = new List<Type>();
			foreach(ITypeInfo classInfo in XafTypesInfo.Instance.PersistentTypes) {
				if(classInfo.IsVisible && classInfo.IsPersistent) {
					if(classInfo.Type != null) {
						list.Add(classInfo.Type);
					}
				}
			}
			list.Sort(CompareTypes);
			return new StandardValuesCollection(list);
		}
		protected static int CompareTypes(Type x, Type y) {
			return Comparer<string>.Default.Compare(x.FullName, y.FullName);
		}
	}
	public class StringToTypeConverterExtended : StringToTypeConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection standardValuesCollection = base.GetStandardValues(context);
			List<Type> list = new List<Type>();
			foreach(Type type in standardValuesCollection) {
				list.Add(type);
			}
			foreach(Type simpleType in DevExpress.Persistent.Base.SimpleTypes.GetSimpleTypes) { 
				list.Add(simpleType);
			}
			list.Sort(CompareTypes);
			return new StandardValuesCollection(list);
		}
	}
	public class StringToTypeConverterEx : StringToTypeConverter { 
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection standardValuesCollection = base.GetStandardValues(context);
			List<Type> result = new List<Type>();
			foreach(Type simpleType in DevExpress.Persistent.Base.SimpleTypes.GetSimpleTypes) { 
				result.Add(simpleType);
			}
			result.Sort(CompareTypes);
			List<Type> persistentTypes = new List<Type>();
			foreach(Type type in standardValuesCollection) {
				persistentTypes.Add(type);
			}
			persistentTypes.Sort(CompareTypes);
			result.AddRange(persistentTypes);
			return new StandardValuesCollection(result);
		}
	}
}
