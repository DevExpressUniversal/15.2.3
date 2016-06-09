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

extern alias Platform;
using DevExpress.Design.SmartTags;
using DevExpress.Design.UI;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Windows.Input;
#if !SL
using System.Diagnostics;
using System.Reflection;
using System.Windows.Markup;
using System.Text;
using DevExpress.Design;
using DevExpress.CodeParser.Xaml;
using DevExpress.CodeParser;
using Microsoft.Windows.Design.Metadata;
using DevExpress.Xpf.Core.Design.SmartTags;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class XpfPropertyLinePlatformInfoFactory : IPropertyLinePlatformInfoFactory {
		readonly IPropertyLineContext context;
		public XpfPropertyLinePlatformInfoFactory(IPropertyLineContext context) {
			this.context = context;
		}
		IPropertyLinePlatformInfo IPropertyLinePlatformInfoFactory.Default() {
			return new XpfPropertyLinePlatformInfo(context, null);
		}
		IPropertyLinePlatformInfo IPropertyLinePlatformInfoFactory.ForStandardProperty(string typeName) {
			return new XpfPropertyLinePlatformInfo(context, PropertyGenerationInfo.ForStandardProperty(typeName));
		}
		IPropertyLinePlatformInfo IPropertyLinePlatformInfoFactory.ForCommandProperty() {
			return new XpfPropertyLinePlatformInfo(context, PropertyGenerationInfo.ForCommandProperty());
		}
	}
	public class DesignTimeObject {
		public DesignTimeObject(object value, IModelItem runtimeTypeProvider) {
			Type runtimeType = null;
			runtimeType = runtimeTypeProvider == null ? null : runtimeTypeProvider.ItemType;
			if(typeof(MarkupExtension).IsAssignableFrom(runtimeType))
				runtimeType = null;
			DebugHelper.Assert(value != null);
			DebugHelper.Assert(!(value is DesignTimeObject));
			Value = value;
			RuntimeType = runtimeType ??  value.GetType();
		}
		public object Value { get; private set; }
		public Type RuntimeType { get; private set; }
	}
	class XpfPropertyLinePlatformInfo : WpfBindableBase, IPropertyLinePlatformInfo {
		IPropertyLineContext context;
		public XpfPropertyLinePlatformInfo(IPropertyLineContext context, PropertyGenerationInfo propertyGenerationInfo) {
			this.context = context;
			PropertyGenerationInfo = propertyGenerationInfo;
		}
		public PropertyGenerationInfo PropertyGenerationInfo { get; private set; }
		public IModelItem SelectedItem { get { return context.ModelItem; } }
		string IPropertyLinePlatformInfo.GetPropertyValueText(IModelProperty property) {
#if SL
			return GetPropertyComputedValueString(property);
#else
			try {
				if(property.Value == null) return null;
				if(!typeof(MarkupExtension).IsAssignableFrom(property.Value.ItemType)) return GetPropertyComputedValueString(property);
				string currentValueSourceXaml = MarkupHelper.GetCurrentValueSourceText(property);
				if(string.IsNullOrEmpty(currentValueSourceXaml)) return GetPropertyComputedValueString(property);
				if(!currentValueSourceXaml.TrimStart(' ').StartsWith("{", StringComparison.Ordinal)) return property.Value.ItemType.Name;
				return currentValueSourceXaml;
			} catch {
#if DEBUG
				throw;
#else
				try {
					object computedValue = property.ComputedValue;
					return computedValue == null ? null : computedValue.ToString();
				} catch {
					return string.Empty;
				}
#endif
			}
#endif
		}
		static string GetPropertyComputedValueString(IModelProperty property) {
			object propertyComputedValue = ModelPropertyHelper.GetComputedValue(property);
			if(propertyComputedValue == null) return string.Empty;
			string toString = propertyComputedValue.ToString();
			if(string.Equals(toString, propertyComputedValue.GetType().FullName, StringComparison.Ordinal))
				return property.Value.ItemType.Name;
			return toString;
		}
	}
	public class CommandPropertyLineViewModel : PropertyLineViewModelBase {
		public CommandPropertyLineViewModel(IPropertyLineContext context, string propertyName, Type propertyOwnerType = null)
			: base(context, propertyName, typeof(ICommand), propertyOwnerType, context.PlatformInfoFactory.ForCommandProperty()) {
		}
	}
	public class SeparatorLineViewModel : SmartTagLineViewModelBase {
		public string Text {
			get { return text; }
			set { SetProperty(ref text, value, () => Text); }
		}
		public bool IsLineVisible {
			get { return isLineVisible; }
			set { SetProperty(ref isLineVisible, value, () => IsLineVisible); }
		}
		public SeparatorLineViewModel(IPropertyLineContext context)
			: base(context) {
				isLineVisible = true;
		}
		bool isLineVisible;
		string text;
	}
}
