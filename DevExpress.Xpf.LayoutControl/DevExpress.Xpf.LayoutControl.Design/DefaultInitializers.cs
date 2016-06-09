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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	using DataLayoutItem = Platform::DevExpress.Xpf.LayoutControl.DataLayoutItem;
	class DefaultInitializerBase : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			if (DesignerHelper.IsBlend) {
				ModelItem = item;
				ModelService modelService = item.Context.Services.GetService<ModelService>();
				modelService.ModelChanged += OnModelChanged;
			}
		}
		protected virtual void OnDefaultsInitialized(ModelItem item) {
		}
		protected virtual void OnItemPropertyChanged(ModelItem item, PropertyChangedEventArgs e) {
		}
		protected void SubscribeToPropertyChanges(ModelItem item) {
			if (IsSubscribedToPropertyChanges)
				return;
			item.PropertyChanged += OnItemPropertyChanged;
			IsSubscribedToPropertyChanges = true;
		}
		protected void UnsubscribeFromPropertyChanges(ModelItem item) {
			if (!IsSubscribedToPropertyChanges)
				return;
			item.PropertyChanged -= OnItemPropertyChanged;
			IsSubscribedToPropertyChanges = false;
		}
		void OnModelChanged(object sender, ModelChangedEventArgs e) {
			((ModelService)sender).ModelChanged -= OnModelChanged;
			OnDefaultsInitialized(ModelItem);
			ModelItem = null;
		}
		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (IsSubscribedToPropertyChanges)
				OnItemPropertyChanged((ModelItem)sender, e);
		}
		ModelItem ModelItem { get; set; }
		bool IsSubscribedToPropertyChanges { get; set; }
	}
	class FrameworkElementDefaultInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			if (item.Parent != null && item.Parent.IsItemOfType(typeof(LayoutGroup)))
				LayoutGroupParentAdapter.InitializeChild(item.Parent, item);
		}
	}
	class GroupBoxDefaultInitializer : DefaultInitializer {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? "GroupBox" : item.Name);
			InitializerHelper.Initialize(item);
		}
	}
	class LayoutGroupDefaultInitializer : DefaultInitializerBase {
		public static void CheckRequirementForLayoutGroupParent(ModelItem item) {
			if (!item.Parent.IsItemOfType(typeof(LayoutGroup)))
				MessageBox.Show(item.ItemType.Name + " requires LayoutControl or LayoutGroup as a parent.", "Information",
					MessageBoxButton.OK, MessageBoxImage.Information);
		}
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			if (!item.IsItemOfType(typeof(LayoutControl))) {
				item.ResetLayout();
				if (item.Parent == null)
					SubscribeToPropertyChanges(item);
				else
					CheckRequirementForLayoutGroupParent(item);
			}
			InitializerHelper.Initialize(item);
		}
		protected override void OnDefaultsInitialized(ModelItem item) {
			base.OnDefaultsInitialized(item);
			if (!item.IsItemOfType(typeof(LayoutControl)))
				item.ResetLayout();
		}
		protected override void OnItemPropertyChanged(ModelItem item, PropertyChangedEventArgs e) {
			if (item.Parent == null)
				return;
			UnsubscribeFromPropertyChanges(item);
			CheckRequirementForLayoutGroupParent(item);
		}
	}
	class LayoutGroupDesignModeValueProvider : DesignModeValueProvider {
		private static PropertyIdentifier BackgroundProperty = new PropertyIdentifier(typeof(LayoutGroup), "Background");
		private static Platform::System.Windows.Media.SolidColorBrush TransparentBrush =
			new Platform::System.Windows.Media.SolidColorBrush(Platform::System.Windows.Media.Colors.Transparent);
		public LayoutGroupDesignModeValueProvider() {
			if (!DesignerHelper.IsBlend)
				Properties.Add(BackgroundProperty);
		}
		public override object TranslatePropertyValue(ModelItem item, PropertyIdentifier identifier, object value) {
			if (identifier == BackgroundProperty)
				return value ?? TransparentBrush;
			else
				return base.TranslatePropertyValue(item, identifier, value);
		}
	}
	class LayoutItemDefaultInitializer : DefaultInitializerBase {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.ResetLayout();
			if (IsContentAndLabelInitializationNeeded(item)) {
				ModelItem defaultContent = ModelFactory.CreateItem(item.Context, typeof(Platform::DevExpress.Xpf.Editors.TextEdit),
					CreateOptions.InitializeDefaults, null);
				defaultContent.ResetLayout();
				item.Properties["Content"].SetValue(defaultContent);
				item.Properties["Label"].SetValue(string.IsNullOrEmpty(item.Name) ? "LayoutItem" : item.Name);
			}
			if (item.Parent == null)
				SubscribeToPropertyChanges(item);
			else
				LayoutGroupDefaultInitializer.CheckRequirementForLayoutGroupParent(item);
			InitializerHelper.Initialize(item);
		}
		protected bool IsContentAndLabelInitializationNeeded(ModelItem item) {
			return !item.IsItemOfType(typeof(DataLayoutItem));
		}
		protected override void OnDefaultsInitialized(ModelItem item) {
			base.OnDefaultsInitialized(item);
			item.ResetLayout();
		}
		protected override void OnItemPropertyChanged(ModelItem item, PropertyChangedEventArgs e) {
			if (item.Parent == null)
				return;
			UnsubscribeFromPropertyChanges(item);
			LayoutGroupDefaultInitializer.CheckRequirementForLayoutGroupParent(item);
		}
	}
	class TileLayoutControlDefaultInitializer : DefaultInitializerBase {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			ModelItem defaultBackground = ModelFactory.CreateItem(item.Context, typeof(Platform::System.Windows.Media.SolidColorBrush), null);
			defaultBackground.Properties["Color"].SetValue(Platform::System.Windows.Media.Color.FromArgb(255, 14, 109, 56));
			item.Properties["Background"].SetValue(defaultBackground);
			InitializerHelper.Initialize(item);
		}
	}
	class TileDefaultInitializer : DefaultInitializerBase {
		public override void InitializeDefaults(ModelItem item) {
			base.InitializeDefaults(item);
			item.ResetLayout();
			item.Properties["Header"].SetValue(string.IsNullOrEmpty(item.Name) ? "Tile" : item.Name);
		}
	}
}
