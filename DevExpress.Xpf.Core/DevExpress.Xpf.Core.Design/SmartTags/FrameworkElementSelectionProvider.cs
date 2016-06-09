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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Design.SmartTags;
using DevExpress.Xpf.Core.Design.SmartTags;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;
using DevExpress.Xpf.CreateLayoutWizard;
using System.Diagnostics;
using System.Windows.Input;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
namespace DevExpress.Xpf.Core.Design {
	[FeatureConnector(typeof(SmartTagFeatureConnector))]
	[UsesItemPolicy(typeof(SmartTagAdornerSelectionPolicy))]
	public class FrameworkElementSmartTagAdorner : SmartTagAdornerBase {
		protected SmartTagDesignService Service {
			get { return service; }
			set {
				if(Service == value) return;
				var oldValue = service;
				service = value;
				OnDesignServiceChanged(oldValue);
			}
		}
#if !SL
		static bool IsStandardControlValid {
			get {
				bool isStandardControlValid = SharedMemoryDataHelper.GetSharedData().IsStandardSmartTagsEnabled;
#if !SL
				isStandardControlValid &= VSVersion > 10;
#endif
				return isStandardControlValid;
			}
		}
#endif
		public static int VSVersion { get; private set; }
		static FrameworkElementSmartTagAdorner() {
			VSVersion = Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart;
		}
		public FrameworkElementSmartTagAdorner() {
			SmartTagButton.IsPressedChanged += OnSmartTagButtonIsPressedChanged;
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(300);
			timer.Tick += OnTimerTick;
			isSmartTagButtonEnabled = CheckIsSmartTagEnabled();
			SmartTagButton.Cursor = Cursors.Arrow;
		}
		bool isSmartTagButtonEnabled;
		void OnTimerTick(object sender, EventArgs e) {
			bool isEnabled = CheckIsSmartTagEnabled();
			if(isSmartTagButtonEnabled != isEnabled) {
				isSmartTagButtonEnabled = isEnabled;
				UpdateSmartTagButtonVisibility();
			}
		}
		protected override object GetViewModel(ModelItem primarySelection) {
			IModelItem xpfPrimarySelection = UseParentLinesLines(primarySelection) ? XpfModelItem.FromModelItem(primarySelection.Parent) : XpfModelItem.FromModelItem(primarySelection);
			var viewModel = new FrameworkElementSmartTagViewModel(xpfPrimarySelection);
			return viewModel;
		}
		protected override void Activate(ModelItem item) {
			timer.Start();
			Service = item.Context.Services.GetService<SmartTagDesignService>();
			base.Activate(item);
		}
		protected override void Deactivate() {
			timer.Stop();
			if(Service != null) {
				Service.IsSmartTagButtonPressed = SmartTagButton.IsPressed;
			}
			base.Deactivate();
			Service = null;
		}
		protected override Visibility GetSmartTagButtonVisibility() {
			if(!((CheckIsValidElement(PrimarySelection) || CheckIsSmartTagEnabled()) && CheckServiceIsSmartTagButtonVisible())) {
				return Visibility.Collapsed;
			}
			return base.GetSmartTagButtonVisibility();
		}
		bool CheckServiceIsSmartTagButtonVisible() {
			return Service == null ? true : Service.IsSmartTagButtonVisible;
		}
		bool CheckIsSmartTagEnabled() {
			return SharedMemoryDataHelper.GetSharedData().IsStandardSmartTagsEnabled;
		}
		protected override Rect GetSelectedElementBounds() {
			Rect baseValue = base.GetSelectedElementBounds();
			if(PrimarySelection == null) return baseValue;
			foreach(DesignTimeParentAttribute attribute in AttributeHelper.GetAttributes<DesignTimeParentAttribute>(PrimarySelection.ItemType)) {
				if(attribute.ViewProvider == null) continue;
				IViewProvider provider = Activator.CreateInstance(attribute.ViewProvider) as IViewProvider;
				var viewElem = provider.ProvideView(PrimarySelection);
				if(viewElem == null) continue;
				ViewItem view = ViewItemHelper.GetViewItem(AdornedElement.View, viewElem);
				if(view == null) continue;
				var transform = view.TransformToView(AdornedElement.View);
				return transform.TransformBounds(new Rect(view.RenderSizeBounds.Size));
			}
			return baseValue;
		}
		protected virtual void OnDesignServiceChanged(SmartTagDesignService oldValue) {
			if(oldValue != null) {
				oldValue.IsSmartTagButtonPressedChanged -= OnServiceIsSmartTagButtonPressedChanged;
				oldValue.IsSmartTagButtonVisibleChanged -= OnServiceIsSmartTagButtonVisibleChanged;
			}
			if(Service != null) {
				Service.IsSmartTagButtonPressedChanged += OnServiceIsSmartTagButtonPressedChanged;
				Service.IsSmartTagButtonVisibleChanged += OnServiceIsSmartTagButtonVisibleChanged;
			}
			UpdateSmartTagButtonIsPressed();
			UpdateSmartTagButtonVisibility();
		}
		bool CheckIsValidElement(ModelItem selectedElement) {
			if(selectedElement == null)
				return false;
			Type itemType = selectedElement.ItemType;
			if(itemType != null && IsDXType(itemType))
				return true;
			if(IsStandardControlValid) {
				var itemAssembly = itemType.Assembly;
				if (typeof(Window).IsAssignableFrom(itemType) || typeof(UserControl).IsAssignableFrom(itemType) || itemAssembly == null)
					return true;
				return IsInAssemblyWithNameStarts(itemAssembly.GetName(), "PresentationFramework");
			}
			return false;
		}
		bool UseParentLinesLines(ModelItem modelItem) {
			if(modelItem == null || modelItem.Parent == null)
				return false;
			Type parentType = modelItem.Parent.ItemType;
			foreach(var attribute in AttributeHelper.GetAttributes<UseParentPropertyLinesAttribute>(modelItem.ItemType)) {
				if(attribute.ParentType.IsAssignableFrom(parentType))
					return true;
			}
			return false;
		}
		bool IsDXType(Type itemType) {
			Type type = itemType;
			while(type != null) {
				AssemblyName assemblyName = type.Assembly.GetName();
				if (IsInAssemblyWithNameStarts(assemblyName, "DevExpress"))
					return true;
				type = type.BaseType;
			}
			return false;
		}
		bool IsInAssemblyWithNameStarts(AssemblyName assemblyName, string name) {
			string stringName = assemblyName.Name;
			return stringName.StartsWith(name);
		}
		void OnServiceIsSmartTagButtonPressedChanged(object sender, EventArgs e) {
			UpdateSmartTagButtonIsPressed();
		}
		void OnServiceIsSmartTagButtonVisibleChanged(object sender, EventArgs e) {
			UpdateSmartTagButtonVisibility();
		}
		void OnSmartTagButtonIsPressedChanged(object sender, EventArgs e) {
			if(Service != null)
				Service.IsSmartTagButtonPressed = SmartTagButton.IsPressed;
		}
		void UpdateSmartTagButtonIsPressed() {
			if(Service != null && SmartTagButton != null)
				SmartTagButton.IsPressed = Service.IsSmartTagButtonPressed;
		}
		SmartTagDesignService service;
		DispatcherTimer timer;
	}
	public class SmartTagAdornerSelectionPolicy : SelectionPolicy {
		protected override System.Collections.Generic.IEnumerable<ModelItem> GetPolicyItems(Selection selection) {
			ModelItem primarySelection = selection.PrimarySelection;
			if(primarySelection == null)
				return base.GetPolicyItems(selection);
			var attributes = AttributeHelper.GetAttributes<DesignTimeParentAttribute>(primarySelection.ItemType).Distinct();
			var parents = attributes.Select(dtAttr => BarManagerDesignTimeHelper.FindParentByType(dtAttr.ParentType, primarySelection)).Where(item => item != null);
			if(parents.Count() > 0)
				return new ModelItem[] { parents.First() };
			return new ModelItem[] { primarySelection };
		}
	}
	public class SmartTagFeatureConnector : FeatureConnector<FrameworkElementSmartTagAdorner> {
		public SmartTagFeatureConnector(FeatureManager manager) : base(manager) {
			Context.Services.Publish<SmartTagDesignService>(new SmartTagDesignService());
		}
	}
	public class SmartTagDesignService {
		public bool IsSmartTagButtonPressed {
			get { return isSmartTagButtonPressed; }
			set {
				if(IsSmartTagButtonPressed == value) return;
				isSmartTagButtonPressed = value;
				OnIsSmartTagButtonPressedChanged();
			}
		}
		public bool IsSmartTagButtonVisible {
			get { return isSmartTagButtonVisible; }
			set {
				if(isSmartTagButtonVisible == value) return;
				isSmartTagButtonVisible = value;
				OnIsSmartTagButtonVisibleChanged();
			}
		}
		public SmartTagDesignService() : base() {
			isSmartTagButtonVisible = true;
		}
		public event EventHandler IsSmartTagButtonPressedChanged;
		public event EventHandler IsSmartTagButtonVisibleChanged;
		void OnIsSmartTagButtonPressedChanged() {
			if(IsSmartTagButtonPressedChanged != null) {
				IsSmartTagButtonPressedChanged(this, EventArgs.Empty);
			}
		}
		void OnIsSmartTagButtonVisibleChanged() {
			if(IsSmartTagButtonVisibleChanged != null) {
				IsSmartTagButtonVisibleChanged(this, EventArgs.Empty);
			}
		}
		bool isSmartTagButtonPressed;
		bool isSmartTagButtonVisible;
	}
	public class SmartTagPropertyLineComparer : IComparer<SmartTagLineViewModelBase> {
		public int Compare(SmartTagLineViewModelBase x, SmartTagLineViewModelBase y) {
			if(x is PropertyLineViewModelBase && y is PropertyLineViewModelBase)
				return string.Compare(((PropertyLineViewModelBase)x).PropertyName, ((PropertyLineViewModelBase)y).PropertyName);
			else if(x is PropertyLineViewModelBase) {
				return -1;
			} else return 1;
		}
	}
}
