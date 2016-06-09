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

using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Windows.Forms;
using System.Windows.Interop;
namespace DevExpress.Xpf.Editors.Internal {
	public class SparklinePropertyDescriptor : SparklinePropertyDescriptorBase {
		static bool IsComplexColumn(string member) {
			return !string.IsNullOrEmpty(member) && member.Contains(".");
		}
		static readonly object Wrapped = new object();
		static object GetWrapped(object component) {
			return component ?? Wrapped;
		}
		readonly Dictionary<Type, SparklinePropertyDescriptor> descriptorsCache = new Dictionary<Type, SparklinePropertyDescriptor>();
		PropertyDescriptor BaseDescriptor;
		bool ShouldCreateFastPropertyDescriptor {
			get {
#if SL
				return !DesignerProperties.IsInDesignTool;
#else
				return !BrowserInteropHelper.IsBrowserHosted;
#endif
			}
		}
		public SparklinePropertyDescriptor(string path, string internalPath)
			: base(path, internalPath) { }
		PropertyDescriptor CreateBaseDescriptor(object component) {
			System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(InternalPath), "InternalPath");
			if (IsComplexColumn(InternalPath))
				return new ComplexPropertyDescriptorReflection(component, InternalPath);
			PropertyDescriptor descriptor = CreatePropertyAccessDescriptor(component);
			return descriptor;
		}
		PropertyDescriptor CreatePropertyAccessDescriptor(object component) {
			if (component is DynamicObject)
				return new DynamicObjectPropertyDescriptor(InternalPath);
			if (component is ExpandoObject)
				return new ExpandoPropertyDescriptor(null, InternalPath, null);
			PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(component);
			PropertyDescriptor descriptor = properties[InternalPath];
			if (descriptor == null) {
				properties = TypeDescriptor.GetProperties(component);
				descriptor = properties[InternalPath];
			}
			if (descriptor != null)
				return ShouldCreateFastPropertyDescriptor ? CreateFastPropertyDescriptor(descriptor) : descriptor;
			return null;
		}
		SparklinePropertyDescriptor GetDescriptor(object component) {
			Type componentType = GetWrapped(component).GetType();
			SparklinePropertyDescriptor descriptor;
			descriptorsCache.TryGetValue(componentType, out descriptor);
			if (descriptor == null || !descriptor.IsRelevant(InternalPath)) {
				descriptor = new SparklinePropertyDescriptor(Path, InternalPath);
				descriptorsCache[componentType] = descriptor;
			}
			return descriptor;
		}
		protected PropertyDescriptor CreateFastPropertyDescriptor(PropertyDescriptor descriptor) {
			return DataListDescriptor.GetFastProperty(descriptor);
		}
		protected override object GetValueImpl(object component) {
			if (BaseDescriptor == null) {
				BaseDescriptor = CreateBaseDescriptor(component);
				if (BaseDescriptor == null)
					return null;
			}
			return BaseDescriptor.GetValue(component);
		}
		public override object GetValue(object component) {
			if (string.IsNullOrEmpty(InternalPath))
				return component;
			return GetDescriptor(component).GetValueImpl(component);
		}
		public override void Reset() {
			if (descriptorsCache != null)
				descriptorsCache.Clear();
		}
	}
}
