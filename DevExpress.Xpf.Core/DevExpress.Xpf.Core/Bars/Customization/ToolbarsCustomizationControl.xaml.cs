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

using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Xpf.Bars.Customization {
   public partial class ToolbarsCustomizationControl : UserControl {
		readonly ToolbarCustomizationControlModel model;
		readonly BarManagerCustomizationHelper helper;
		protected ToolbarCustomizationControlModel Model { get { return model; } }
		protected BarManagerCustomizationHelper Helper { get { return helper; } }
		public ToolbarsCustomizationControl(BarManagerCustomizationHelper helper) {
			InitializeComponent();			
			this.helper = helper;
			this.model = CreateModel();
			Model.Create += OnCreateNewBar;
			Model.Rename += OnRenameSelectedBar;
			Model.Remove += OnRemoveSelectedBar;
			DataContext = Model;
		}
		protected virtual ToolbarCustomizationControlModel CreateModel() {
			return ViewModelSource<ToolbarCustomizationControlModel>.Create<Func<IEnumerable<Bar>>, bool>(Helper.GetBars, Manager!=null);
		}
		BarManager Manager { get { return helper.Scope.With(x => x.Target).With(BarManager.GetBarManager); } }
		protected virtual void CreateNewBar(string caption) {
			helper.Strategy.OnCreateNewBar(caption);			
		}
		protected virtual void OnCreateNewBar() {
			string caption;
			if (GetCaption(out caption)) {
				CreateNewBar(caption);
			}
		}
		protected virtual void OnRenameSelectedBar(Bar param) {
			string caption;
			if (GetCaption(out caption)) {
				helper.Strategy.OnRenameBar(param, caption);
			}
		}
		protected virtual void OnRemoveSelectedBar(Bar bar) {
			helper.Strategy.OnRemoveBar(bar);
		}
		protected virtual ToolbarCaptionEditor CreateToolbarCaptionEditor() {
			return new ToolbarCaptionEditor();
		}
		protected bool GetCaption(out string result) {
			var captionEditor = CreateToolbarCaptionEditor();
			try {
				return new DXDialog() {
					Content = captionEditor,
					SizeToContent = SizeToContent.Height,
					Width = Manager != null ? Manager.ValuesProvider.ToolbarCaptionEditorWindowFloatSize.Width : 400,
					WindowStartupLocation = WindowStartupLocation.CenterOwner,
					Owner = Window.GetWindow(this),
					Title = BarsLocalizer.GetString(BarsStringId.ToolbarsCustomizationControl_EditorWindowCaption),
					FlowDirection = FlowDirection
				}.ShowDialog(MessageBoxButton.OKCancel) == MessageBoxResult.OK;
			} finally {
				result = captionEditor.ToolbarCaption;				
			}			
		}
		void OnListBoxItemGotFocus(object sender, RoutedEventArgs e) {
			toolbarList.SelectedItem = ((ListBoxItem)sender).DataContext;
			((ListBoxItem)sender).Focus();
		}
		void VisibilityCheckEditValueChanged(object sender, EditValueChangedEventArgs e) {
			CheckEdit ce = sender as CheckEdit;
			if(ce==null)
				return;
			Bar bar = ce.DataContext as Bar;
			if (bar == null)
				return;
			Helper.Strategy.OnChangeBarVisibility(bar, (bool)ce.IsChecked);
		}		
	}
	public class ToolbarCustomizationControlModel {
		Func<IEnumerable<Bar>> requestBars;
		bool allowCreate;
		IEnumerable<Bar> RequestBars() {
			return requestBars().Where(x => !x.IsPrivate && !x.IsRemoved);
		}
		public ToolbarCustomizationControlModel(Func<IEnumerable<Bar>> requestBars, bool allowCreate) {
			this.requestBars = requestBars;
			this.allowCreate = allowCreate;
			UpdateBars();			
		}
		public virtual List<object> Bars { get; set; }
		public void CreateBar() {
			Create();
			UpdateBars();
		}		
		public bool CanCreateBar() {
			return allowCreate;
		}
		public void DeleteBar(object parameter) {
			Remove((Bar)parameter);
			UpdateBars();
		}
		public bool CanDeleteBar(object parameter) {
			Bar bar = (Bar)parameter;
			if (bar == null)
				return false;			
			return true;
		}
		public void RenameBar(object parameter) {
			Rename((Bar)parameter);
		}
		public bool CanRenameBar(object parameter) {
			Bar bar = (Bar)parameter;
			if (bar == null)
				return false;
			return !bar.IsMainMenu && !bar.IsStatusBar && bar.AllowRename;
		}		
		void UpdateBars() {
			Bars = RequestBars().ToList<object>();
		}
		public event Action<Bar> Rename = new Action<Bar>((bar) => { });
		public event Action Create = new Action(() => { });
		public event Action<Bar> Remove = new Action<Bar>((bar) => { });
	}	
	public class DefaultBooleanToBooleanConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			Bar bar = parameter as Bar;
			if(bar != null) return bar.IsAllowHide;
			else return ((DefaultBoolean)value) == DefaultBoolean.False ? false : true;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return ((bool)value) ? DefaultBoolean.True : DefaultBoolean.False;
		}
	}
}
