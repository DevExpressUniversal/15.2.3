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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Reflection;
using DevExpress.Design.SmartTags;
using System.Windows.Data;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Utils;
using Guard = Platform::DevExpress.Utils.Guard;
#if !SL
using DevExpress.Xpf.Core.Design.SmartTags;
using System.Runtime.CompilerServices;
using DevExpress.Xpf.Design;
#endif
namespace DevExpress.Xpf.Core.Design {
	public class FrameworkElementSmartTagPropertiesViewModel : DesignTimeViewModelBase, IPropertyLineContext {
		#region static
		static List<IPropertyLineProvider> registeredProvidersList = new List<IPropertyLineProvider>();
		static TypeTree<IPropertyLineProvider> registeredProviders = new TypeTree<IPropertyLineProvider>();
		public static IEnumerable<IPropertyLineProvider> RegisteredProviders { get { return registeredProvidersList.AsReadOnly(); } }
		public static void RegisterPropertyLineProvider(IPropertyLineProvider provider, bool isSealed = true) {
			Guard.ArgumentNotNull(provider, "provider");
			if(isSealed && !provider.GetType().IsSealed)
				throw new ArgumentException("You cannot register a non-sealed class as a PropertyLineProvider. If you want to do this anyway, use the optional parameter to disable this warning.", "provider");
			registeredProvidersList.Add(provider);
			registeredProviders.Add(provider.ItemType, provider);
		}
		#endregion
		public IEnumerable<SmartTagLineViewModelBase> Lines { get; protected set; }
		XpfPropertyLinePlatformInfoFactory factory;
		public bool IsDXType {
			get { return isDXType; }
			set {
				if(IsDXType == value) return;
				isDXType = value;
				RaisePropertyChanged("IsDXType");
			}
		}
		public FrameworkElementSmartTagPropertiesViewModel(IModelItem selectedItem)
			: base(selectedItem) {
			factory = new XpfPropertyLinePlatformInfoFactory(this);
			Lines = GetProviders().SelectMany(p => p.GetProperties(this)).ToArray();
#if !SL
			if(IsDXType)
				InitializerHelper.AddVBLicensedItem(selectedItem);
#endif
		}
		protected override void Dispose(bool disposing) {
			Lines = new SmartTagLineViewModelBase[] { };
			base.Dispose(disposing);
		}
		IEnumerable<IPropertyLineProvider> GetProviders() {
			return registeredProviders.Find(RuntimeSelectedItem.ItemType).Where(p => p.IsNearest || p.Value.GetType().IsSealed).Select(p => p.Value);
		}
		protected override void OnSelectedItemPropertyChanged(string propertyName) {
			base.OnSelectedItemPropertyChanged(propertyName);
			foreach(var line in Lines) {
				line.OnSelectedItemPropertyChanged(propertyName);
			}
		}
		protected override void OnSelectedItemChanged(IModelItem oldSelectedItem) {
			IsDXType = GetIsDXType(this.RuntimeSelectedItem);
			base.OnSelectedItemChanged(oldSelectedItem);
		}
		bool GetIsDXType(IModelItem modelItem) {
			if(modelItem == null) return false;
			return modelItem.ItemType.Assembly.FullName.StartsWith("DevExpress");
		}
		IModelItem IPropertyLineContext.ModelItem {
			get { return RuntimeSelectedItem; }
		}
		IPropertyLinePlatformInfoFactory IPropertyLineContext.PlatformInfoFactory {
			get { return factory; }
		}
		IPropertyLineContext IPropertyLineContext.CreateContext(IModelItem modelItem) {
			return new FrameworkElementSmartTagPropertiesViewModel(modelItem);
		}
		bool isDXType;
	}
	public class PreparePropertyLinesEventArgs : EventArgs {
		public IEnumerable<SmartTagLineViewModelBase> Lines { get; set; }
		public Type SelectedItemType { get; private set; }
		public PreparePropertyLinesEventArgs(IEnumerable<SmartTagLineViewModelBase> lines, Type selectedItemType) {
			SelectedItemType = selectedItemType;
			Lines = lines;
		}
	}
	public delegate void PreparePropertyLineEventHandler(object sender, PreparePropertyLinesEventArgs args);
}
