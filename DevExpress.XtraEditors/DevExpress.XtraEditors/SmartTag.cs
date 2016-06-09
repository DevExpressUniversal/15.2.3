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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraTab;
using System.Collections.Generic;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors {	
	public class BaseControlBoundsProvider : ControlBoundsProvider {	 
		public override Control GetOwnerControl(IComponent component) {
			return component as BaseControl;
		}
	}
	public class PanelControlBoundsProvider : ControlBoundsProvider {
		public override Control GetOwnerControl(IComponent component) {
			return component as PanelControl;
		}
	}
	public class BaseEditFilter : ControlFilter {
		protected virtual bool AllowShowText { get { return true; } }
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			return base.FilterMethod(MethodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(descriptor.Name == "Text" && !AllowShowText) return false;
			return base.FilterProperty(descriptor);
		}
		public override void SetComponent(IComponent component) { base.SetComponent(component); }
	}
	public class ImageSliderActions : ControlActions {
		public void Images(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null) EditorContextHelper.EditValue(designer, component, "Images");
		}
	}
	public class ImageSliderFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }
	}
	public class SparklineActions : ControlActions {
		public void Data(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null) EditorContextHelper.EditValue(designer, component, "Data");
		}
	}
	public class ZoomTrackBarActions : ControlActions {		
		public void EditLabel(IComponent component) {
			ZoomTrackBarControl control = component as ZoomTrackBarControl;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null) EditorContextHelper.EditValue(designer, control.Properties, "Labels");
		}
	}
	public class ToggleSwitchActions : ControlActions {
		void ChangeState(IComponent component, bool state) {
			ToggleSwitch control = component as ToggleSwitch;
			if(control != null) control.IsOn = state;
		}
		public void On(IComponent component) {
			ChangeState(component, true);
		}
		public void Off(IComponent component) {
			ChangeState(component, false);
		}
	}
	public class ToggleSwitchFilter : ControlFilter {
		public new ToggleSwitch Control { get { return base.Control as ToggleSwitch; } }
		public override  bool FilterMethod(string MethodName, object actionMethodItem) {
			if(Control.IsOn && MethodName == "On") return false;
			if(!Control.IsOn && MethodName == "Off") return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor descriptor) {
			return base.FilterProperty(descriptor);
		}
	}
	public class ButtonEditActions : TextEditActions {
		public void Buttons(IComponent component) {
			ButtonEdit control = component as ButtonEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "Buttons");
		}
	}
	public class ButtonEditFilter : TextEditFilter {
		protected virtual bool AllowShowButtons { get { return true; } }
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "Buttons" && !AllowShowButtons) return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class TextEditActions : ControlActions {
		public void EditMask(IComponent component) {
			TextEdit control = component as TextEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "Mask");
		}
	}
	public class TextEditFilter : BaseEditFilter {
		public new TextEdit Control { get { return base.Control as TextEdit; } }		
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "EditMask" && !Control.ViewInfo.AllowMaskBox) return false; 
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class MemoEditFilter : TextEditFilter {
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "EditMask") return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class PopupBaseEditFilter : ButtonEditFilter {
		protected override bool AllowShowButtons { get { return false; } }		
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(descriptor.Name == "ButtonsStyle") return false;
			return base.FilterProperty(descriptor);
		}
	}
	public class ImageEditActions : ButtonEditActions {
		public virtual void Image(IComponent component) {
			IDesigner designer = GetDesigner(component);
			EditorContextHelper.EditValue(designer, component, "Image");
		}
	}
	public class LookUpEditActions : ButtonEditActions {
		public virtual void Columns(IComponent component) {
			LookUpEdit control = component as LookUpEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null) EditorContextHelper.EditValue(designer, control.Properties, "Columns");
		}
		public virtual void PopulateColumns(IComponent component) {
			LookUpEdit control = component as LookUpEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null) {
				control.Properties.PopulateColumns();
				EditorContextHelper.FireChanged(designer, control);
			}
		}
	}
	public class SimpleButtonActions : ControlActions {
		public virtual void Image(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null) EditorContextHelper.EditValue(designer, component, "Image");
		}
	}
	public class CheckButtonActions : SimpleButtonActions {
		void ChangeState(CheckButton button, bool state) {
			if(button != null) button.Checked = state;
		}
		public virtual void Checked(IComponent component) {
			ChangeState(component as CheckButton, true);
		}
		public virtual void Unchecked(IComponent component) {
			ChangeState(component as CheckButton, false);
		}
	}
	public class CheckButtonFilter : ControlFilter {
		public new CheckButton Control { get { return base.Control as CheckButton; } }
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(Control.Checked && MethodName == "Checked") return false;
			if(!Control.Checked && MethodName == "Unchecked") return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor descriptor) {
			return base.FilterProperty(descriptor);
		}
	}
	public class BaseListBoxControlActions : ControlActions {
		public void Items(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null) EditorContextHelper.EditValue(designer, component, "Items");
		}
	}
	public class BaseListBoxControlFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }
	}
	public class BaseImageListBoxControlFilter : BaseListBoxControlFilter {
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "Items") 
				return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class BaseCheckedListBoxControlFilter : BaseListBoxControlFilter {
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "Items")
				return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class BaseImageListBoxControlActions : BaseListBoxControlActions {
		public void EditItems(IComponent component) {
			Items(component);
		}
	}
	public class BaseCheckedListBoxControlActions : BaseListBoxControlActions {
		public void EditItems(IComponent component) {
			Items(component);
		}
	}
	public class LabelControlFilter : ControlFilter {
		public new LabelControl Control { get { return base.Control as LabelControl; } }
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(Control != null && !Control.LineVisible) {
				if(descriptor.Name == "LineStyle") return false;
				if(descriptor.Name == "LineOrientation") return false;
				if(descriptor.Name == "LineLocation") return false;
			}
			return base.FilterProperty(descriptor);
		}
	}
	public class HyperLinkEditFilter : ButtonEditFilter {
		protected override bool AllowShowButtons { get { return false; ; } }	
	}
	public class ComboBoxEditActions : ButtonEditActions {
		public void Items(IComponent component) {
			ComboBoxEdit control = component as ComboBoxEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)				 
				EditorContextHelper.EditValue(designer, control.Properties, "Items");			
		}
	}
	public class BreadCrumbEditFilter : ButtonEditFilter {
		List<string> hiddenProperties;
		public BreadCrumbEditFilter() {
			this.hiddenProperties = CreateHiddenPropetiesList();
		}
		protected virtual List<string> CreateHiddenPropetiesList() {
			return new List<string> { "Items", "Text", "DropDownRows" };
		}
		public override bool FilterMethod(string methodName, object actionMethodItem) {
			if(this.hiddenProperties.Contains(methodName)) return false;
			return base.FilterMethod(methodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor pd) {
			if(this.hiddenProperties.Contains(pd.Name)) return false;
			return base.FilterProperty(pd);
		}
	}
	public class BreadCrumbActions : ButtonEditActions {
		public void Nodes(IComponent component) {
			BreadCrumbEdit control = component as BreadCrumbEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "Nodes");
		}		
	}
	public class TokenEditActions : ControlActions {
		public void EditTokenEditTokens(IComponent component) {
			EditTokenEditProperty(component, "Tokens");
		}
		public void EditTokenEditSeparators(IComponent component) {
			EditTokenEditProperty(component, "Separators");
		}
		protected void EditTokenEditProperty(IComponent component, string propertyName) {
			TokenEdit control = component as TokenEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, propertyName);
		}
	}
	public class ComboBoxEditFilter : PopupBaseEditFilter {
		protected virtual bool AllowShowEditMultiLineText { get { return true; } }
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(!AllowShowEditMultiLineText && MethodName == "Items") return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
	}
	public class FontEditFilter : ComboBoxEditFilter {
		protected override bool AllowShowEditMultiLineText { get { return false; } }
	}
	public class ImageComboBoxEditFilter : ComboBoxEditFilter {
		protected override bool AllowShowEditMultiLineText { get { return false; } }
	}
	public class ImageComboBoxEditActions : ComboBoxEditActions {
		public void EditItems(IComponent component) {
			Items(component);
		}
	}
	public class RadioGroupActions : ControlActions {
		public void Items(IComponent component) {
			RadioGroup control = component as RadioGroup;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "Items");
		}
	}
	public class MemoEditActions : ControlActions {
		public void Lines(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "Lines");
		}
	}
	public class MemoExEditActions : ButtonEditActions {
		public void Lines(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "Lines");
		}	  
	}
	public class ColorEditActions : ButtonEditActions {
		public void CustomColors(IComponent component) {
			ColorEdit control = component as ColorEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "CustomColors");
		}
	}
	public class CheckedComboBoxEditActions : ButtonEditActions { 
		public void Items(IComponent component){
			CheckedComboBoxEdit control = component as CheckedComboBoxEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "Items");
		}
	}
	public class CheckedComboBoxEditFilter : PopupBaseEditFilter {
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(descriptor.Name == "PopupControl") return false;
			return base.FilterProperty(descriptor);
		}
	}
	public class RangeControlActions : ControlActions {
		public void AddNumericClient(IComponent component) {
			RangeControl rangeControl = component as RangeControl;
			if(rangeControl != null) {
				NumericRangeControlClient client = new NumericRangeControlClient();
				client.RangeControl = rangeControl;
				rangeControl.Client = client;
				if(rangeControl.Container != null)
					rangeControl.Container.Add(client);
			}
		}
		public void AddChartNumericClient(IComponent component) {
			RangeControl rangeControl = component as RangeControl;
			if (rangeControl != null) {
				NumericChartRangeControlClient client = new NumericChartRangeControlClient();
				rangeControl.Client = client;
				if (rangeControl.Container != null)
					rangeControl.Container.Add(client);
			}
		}
		public void AddChartDateTimeClient(IComponent component) {
			RangeControl rangeControl = component as RangeControl;
			if (rangeControl != null) {
				DateTimeChartRangeControlClient client = new DateTimeChartRangeControlClient();
				rangeControl.Client = client;
				if (rangeControl.Container != null)
					rangeControl.Container.Add(client);
			}
		}
	}
	public class RangeControlFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }
	}
	public class PictureEditActions : ControlActions {
		public void ChooseImage(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "Image");
		}
		public void ErrorImage(IComponent component) {
			PictureEdit control = component as PictureEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "ErrorImage");
		}
		public void InitialImage(IComponent component) {
			PictureEdit control = component as PictureEdit;
			IDesigner designer = GetDesigner(component);
			if(control != null && designer != null)
				EditorContextHelper.EditValue(designer, control.Properties, "InitialImage");
		}
	}
	public class SearchControlFilter : PopupBaseEditFilter {
		List<string> hiddenProperties;
		public SearchControlFilter() {
			this.hiddenProperties = CreateHiddenPropetiesList();
		}
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(hiddenProperties.Contains(descriptor.Name)) return false;
			if(descriptor.Name == "FindDelay" && !Control.Properties.AllowAutoApply) return false;
			return base.FilterProperty(descriptor);
		}
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(MethodName == "Items") return false;
			return base.FilterMethod(MethodName, actionMethodItem);
		}
		protected virtual List<string> CreateHiddenPropetiesList() {
			return new List<string> { "Text", "MaxItemCount", "DropDownRows", "Sorted", "ImmediatePopup" };
		}
		public new SearchControl Control { get { return base.Control as SearchControl; } }
	}
	public class XtraTabControlFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }
	}
	public class XtraTabControlActions : ControlActions {		
		public void TabPages(IComponent component) {
			EditorContextHelper.EditValue(GetDesigner(component), component, "TabPages");
		}
		protected XtraTabPage AddPage(XtraTabControl control) {
			XtraTabPage page = control.TabPages.Add();
			control.Container.Add(page);
			page.Text = page.Name;
			return page;
		}
		public void AddTabPage(IComponent component) {
			XtraTabControl control = component as XtraTabControl;
			if(control != null) {
				control.BeginInit();
				XtraTabPage page = AddPage(control);
				try {
					control.SelectedTabPage = page;
				}
				finally { }
				control.EndInit();
			}
		}
		public void RemoveTabPage(IComponent component) {
			  XtraTabControl control = component as XtraTabControl;
			  if(control != null) {
				  XtraTabPage page = control.SelectedTabPage;
				  if(page == null) return;
				  page.Dispose();
			  }
		}
		public void CustomHeaderButtons(IComponent component) {
			EditorContextHelper.EditValue(GetDesigner(component), component, "CustomHeaderButtons");
		}
	}
	public class XtraTabControlBoundsProvider : ControlBoundsProvider {	  
		public override Control GetOwnerControl(IComponent component) {
			return component as XtraTabControl;
		}
	}
	public class XtraTabPageBounds : ControlBoundsProvider {
		protected override Point OffsetLocation { get { return new Point(0, 0); } }
		public override Control GetOwnerControl(IComponent component) {
			return component as XtraTabPage;
		}
	}
	public class XtraTabPageFilter : ControlFilter {
		protected override bool AllowDock { get { return false; } }
	}
	public class ControlNavigatorButtonsActions : ComponentActions {
		public void CustomButtons(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, (component as ControlNavigator).Buttons, "CustomButtons");
		}
	}
	public class DataNavigatorButtonsActions : ComponentActions {
		public void CustomButtons(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, (component as DataNavigator).Buttons, "CustomButtons");
		}
	}
	public class ColorPickEditBaseFilter : ControlFilter {
		List<string> hiddenNames;
		public ColorPickEditBaseFilter() {
			this.hiddenNames = CreateHiddenNameList();
		}
		protected virtual List<string> CreateHiddenNameList() {
			return new List<string> { "ShowColorDialog", "ButtonsStyle", "Buttons", "EditMask" };
		}
		public override bool FilterMethod(string methodName, object actionMethodItem) {
			if(this.hiddenNames.Contains(methodName))
				return false;
			return base.FilterMethod(methodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor pd) {
			if(this.hiddenNames.Contains(pd.Name))
				return false;
			return base.FilterProperty(pd);
		}
	}
	public class TimeSpanEditFilter : ControlFilter {
		List<string> hiddenNames;
		public TimeSpanEditFilter() {
			this.hiddenNames = CreateHiddenNameList();
		}
		protected virtual List<string> CreateHiddenNameList() {
			return new List<string> { "TextEditStyle", "ChangeMask", "Buttons", "EditMask", "ButtonsStyle" };
		}
		public override bool FilterMethod(string methodName, object actionMethodItem) {
			if(this.hiddenNames.Contains(methodName))
				return false;
			return base.FilterMethod(methodName, actionMethodItem);
		}
		public override bool FilterProperty(MemberDescriptor pd) {
			if(this.hiddenNames.Contains(pd.Name))
				return false;
			return base.FilterProperty(pd);
		}
	}
}
