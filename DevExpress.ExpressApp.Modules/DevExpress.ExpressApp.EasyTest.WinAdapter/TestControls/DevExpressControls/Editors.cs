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
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Standard;
using DevExpress.ExpressApp.PivotChart.Win;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.ExpressApp.Win.Templates.Bars.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraScheduler;
using DevExpress.XtraTab;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls {
	public abstract class TestControlDXTextValidated<T> : TestControlTextValidated<T> where T : BaseEdit {
		public TestControlDXTextValidated(T control) : base(control) { }
		protected override void BeforeEndCurrentEdit() {
			control.DoValidate();
		}
	}
	public class DXBaseEditControlText<T> : TestControlDXTextValidated<T> where T : BaseEdit {
		protected const string ControlIsReadOnly = "The control is read only";
		public DXBaseEditControlText(T control)
			: base(control) {
		}
		protected override string Validate(string text) {
			if(control.Properties.ReadOnly) {
				return ControlIsReadOnly;
			}
			return base.Validate(text);
		}
		protected override void InternalSetText(string text) {
			control.Text = text;
			control.IsModified = true;
		}
	}
	public class DXBaseEditControlText : DXBaseEditControlText<BaseEdit> {
		public DXBaseEditControlText(BaseEdit control)
			: base(control) {
		}
	}
	public class TextEditControl<T> : DXBaseEditControlText<T> where T : TextEdit {
		public TextEditControl(T control)
			: base(control) {
		}
		protected override string Validate(string text) {
			string result = ValidateMaxLength(control.Properties.MaxLength, text);
			if(control.MaskBox.ReadOnly) {
				return ControlIsReadOnly;
			}
			if(!string.IsNullOrEmpty(result)) {
				return result;
			}
			return base.Validate(text);
		}
	}
	public class TextEditControl : TextEditControl<TextEdit> {
		public TextEditControl(TextEdit control)
			: base(control) {
		}
	}
	public class ButtonEditTextControl : TextEditControl<ButtonEdit> {
		public ButtonEditTextControl(ButtonEdit control)
			: base(control) {
		}
		protected override string Validate(string text) {
			if(control.Properties.TextEditStyle != TextEditStyles.Standard) {
				return ControlIsReadOnly;
			}
			return base.Validate(text);
		}
	}
	public class DXBaseEditControlEnabled : WinControlEnabled<BaseEdit> {
		public DXBaseEditControlEnabled(BaseEdit control)
			: base(control) {
		}
		public override bool Enabled {
			get { return base.Enabled && !((BaseEdit)control).Properties.ReadOnly; }
		}
	}
	public class CheckEditControl : TestControlDXTextValidated<CheckEdit> {
		public CheckEditControl(CheckEdit control)
			: base(control) {
		}
		protected override void InternalSetText(string text) {
			try {
				control.Checked = bool.Parse(text);
			}
			catch(FormatException) {
				throw new AdapterException(string.Format("Cannot set the check box value to '{0}'. Use 'True' or 'False'", text));
			}
		}
		protected override string GetText() {
			return control.Checked.ToString();
		}
	}
	internal class ComboBoxEditControl : DXBaseEditControlText<ComboBoxEdit>, IControlActionItems {
		public ComboBoxEditControl(ComboBoxEdit control)
			: base(control) {
		}
		private IEnumerable GetComboBoxItems() {
			return control.Properties.Items;
		}
		private bool CheckDisplayText(object item, string text) {
			if(MultiLineComparisionHelper.CompareString(item.ToString(), text)) {
				return true;
			}
			return false;
		}
		protected override void InternalSetText(string text) {
			foreach(object item in GetComboBoxItems()) {
				if(item != null) {
					if(CheckDisplayText(item, text)) {
						control.EditValue = item;
						return;
					}
				}
			}
			base.InternalSetText(text);
		}
		protected override string Validate(string text) {
			if(control.Properties.TextEditStyle != DevExpress.XtraEditors.Controls.TextEditStyles.Standard) {
				bool isFound = false;
				string values = "";
				foreach(object item in GetComboBoxItems()) {
					if(item != null) {
						if(CheckDisplayText(item, text)) {
							isFound = true;
							break;
						}
						values += '\n' + item.ToString();
					}
				}
				if(!isFound) {
					return String.Format("Requested value {0} was not found among the available items: {1}", text, values);
				}
			}
			return base.Validate(text);
		}
		#region IControlActionItems Members
		private bool ExistsComboBoxItem(string value) {
			if(control.Enabled && !string.IsNullOrEmpty(value)) {
				foreach(object item in GetComboBoxItems()) {
					if(CheckDisplayText(item, value)) {
						return true;
					}
				}
			}
			return false;
		}
		public bool IsVisible(string value) {
			return ExistsComboBoxItem(value);
		}
		public bool IsEnabled(string value) {
			return ExistsComboBoxItem(value);
		}
		#endregion
	}
	public class TimeEditControl : TestControlDXTextValidated<TimeEdit>, IControlAct {
		public TimeEditControl(TimeEdit control)
			: base(control) {
		}
		protected override void InternalSetText(string text) {
			DateTime dateTime = DateTime.MinValue;
			if(text == null || DateTime.TryParseExact(text, control.Properties.Mask.EditMask, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out dateTime)) {
				if(text == null) {
					control.EditValue = null;
				}
				else {
					control.EditValue = dateTime;
				}
				control.IsModified = true;
			}
			else {
				throw new WarningException(String.Format("The '{0}' string doesn't correspond to the '{1}'target input format", text, control.Properties.Mask.EditMask));
			}
		}
		public virtual void Act(string value) {
			throw new Exception("The method or operation is not implemented.");
		}
	}
	public class DateTimeEditControl : TestControlDXTextValidated<DateTimeEdit>, IControlAct {
		public DateTimeEditControl(DateTimeEdit control)
			: base(control) {
		}
		protected override void InternalSetText(string text) {
			DateTime dateTime = DateTime.MinValue;
			if(text == null || DateTime.TryParseExact(text, control.Properties.Mask.EditMask, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.None, out dateTime)) {
				if(text == null) {
					control.EditValue = null;
				}
				else {
					control.EditValue = dateTime;
				}
				control.IsModified = true;
			}
			else {
				throw new WarningException(String.Format("The '{0}' string doesn't correspond to the '{1}'target input format", text, control.Properties.Mask.EditMask));
			}
		}
		#region IControlAct Members
		public virtual void Act(string value) {
			if(value == "Clear") {
				Text = null;
			}
			else {
				throw new WarningException("The DateTimeEdit control supports the 'Clear' action only");
			}
		}
		#endregion
	}
	public class ButtonEditWithClearButtonControl : ButtonEditControlBase<ButtonEditWithClearButton>, IControlAct, IControlActionItems {
		public ButtonEditWithClearButtonControl(ButtonEditWithClearButton control)
			: base(control) {
		}
		private EditorButton FindEditorButtonClear() {
			foreach(EditorButton editorButton in control.Properties.Buttons) {
				if(editorButton.Kind == ButtonPredefines.Delete) {
					return editorButton;
				}
			}
			throw new AdapterOperationException("Clear button is not found.");
		}
		protected override EditorButton GetEditorButton() {
			EditorButton result = ParametrizedActionItemControlFactory.GetButtonById(ParametrizedActionItemControlFactory.GoButtonID, control.Properties);
			if(result == null) {
				result = RepositoryItemButtonEditAdapter.FindExecuteButton(control.Properties);
			}
			if(result == null) {
				foreach(EditorButton editorButtonEdit in control.Properties.Buttons) {
					if(!editorButtonEdit.IsDefaultButton) {
						result = editorButtonEdit;
						break;
					}
				}
			}
			return result;
		}
		public override void Act(string parameterValue) {
			if(parameterValue == ":Clear") {
				if(control.Enabled && control.Visible) {
					EditorButton clearEditorButton = FindEditorButtonClear();
					if(clearEditorButton.Visible) {
						control.PerformClick(clearEditorButton);
					}
					else {
						throw new AdapterOperationException("The 'Clear' button is invisible");
					}
				}
				else {
					throw new AdapterOperationException(String.Format("The '{0}' control's is disabled", TestControl.Name));
				}
			}
			else {
				base.Act(parameterValue);
			}
		}
		#region IControlActionItems Members
		public bool IsEnabled(string item) {
			return IsVisible(item);
		}
		public bool IsVisible(string item) {
			if(item != null) {
				if(item == ":Clear") {
					return FindEditorButtonClear().Visible;
				}
				return false;
			}
			return true;
		}
		#endregion
	}
	public abstract class ButtonEditControlBase<T> : TestControlInterfaceImplementerBase<T>, IControlAct where T : ButtonEdit {
		private object GetNonPublicProperty(object obj, string name) {
			PropertyInfo propertyInfo = obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
			if(propertyInfo != null) {
				return propertyInfo.GetValue(obj, new object[] { });
			}
			else {
				throw new WarningException(@"Cannot find the " + name + " property");
			}
		}
		private object ExecuteNonPublicMethod(object obj, string methodName, params object[] args) {
			MethodInfo raiseButtonClickMethod = obj.GetType().GetMethod(methodName,
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if(raiseButtonClickMethod != null) {
				return raiseButtonClickMethod.Invoke(obj, args);
			}
			else {
				throw new WarningException(@"Cannot find the " + methodName + " method");
			}
		}
		protected void InternalExecuteAction() {
			control.Focus();
			if(control.Properties.Buttons.Count > 0) {
				EditorButton editorButton = null;
				editorButton = GetEditorButton();
				if(editorButton.Visible && editorButton.Enabled) {
					ExecuteNonPublicMethod(control, "OnClickButton", new EditorButtonObjectInfoArgs(editorButton, new AppearanceObject()));
					return;
				}
			}
			throw new AdapterOperationException("Cannot click the editor's button");
		}
		protected abstract EditorButton GetEditorButton();
		public virtual void Act(string parameterValue) {
			if(control.Enabled && control.Visible) {
				if(control.Tag != null && control.Tag.ToString().StartsWith(DevExpress.ExpressApp.Utils.EasyTestTagHelper.TestAction)) {
					if(parameterValue != null) {
						control.EditValue = parameterValue;
					}
					control.PerformClick(GetEditorButton());
				}
				else {
					if(string.IsNullOrEmpty(parameterValue)) {
						InternalExecuteAction();
					}
					else {
						foreach(EditorButton editorButtonEdit in control.Properties.Buttons) {
							if(editorButtonEdit.Caption == parameterValue) {
								if(editorButtonEdit.Visible && editorButtonEdit.Enabled) {
									ExecuteNonPublicMethod(control, "OnClickButton", new EditorButtonObjectInfoArgs(editorButtonEdit, new AppearanceObject()));
									return;
								}
							}
						}
						DXPopupMenu menu = (DXPopupMenu)GetNonPublicProperty(control, "Menu");
						ExecuteNonPublicMethod(control, "UpdateMenu");
						foreach(DXMenuItem item in menu.Items) {
							if(GetDisplayCaption(item.Caption) == parameterValue) {
								if(item.Enabled && item.Visible) {
									item.GenerateClickEvent();
									return;
								}
								else {
									throw new AdapterOperationException(String.Format("The '{0}' control's '{1}' action is disabled or hidden", TestControl.Name, parameterValue));
								}
							}
						}
						throw new AdapterOperationException(String.Format("Cannot find the '{0}' action in the '{1}' control", TestControl.Name, parameterValue));
					}
				}
			}
			else {
				throw new AdapterOperationException(String.Format("The '{0}' control's is disabled", TestControl.Name));
			}
		}
		public static string GetDisplayCaption(string caption) {
			string result = "";
			if(caption != null) {
				result = caption.Replace("&&", "\x1");
				result = result.Replace("&", "");
				result = result.Replace("\x1", "&&");
			}
			return result;
		}
		public ButtonEditControlBase(T control)
			: base(control) {
		}
	}
	public class ButtonEditControl : ButtonEditControlBase<ButtonEdit>, IControlAct {
		public ButtonEditControl(ButtonEdit control)
			: base(control) {
		}
		protected override EditorButton GetEditorButton() {
			EditorButton result = null;
			if(control.Properties.Buttons.Count == 1) {
				result = control.Properties.Buttons[0];
			}
			foreach(EditorButton editorButtonEdit in control.Properties.Buttons) {
				if(!editorButtonEdit.IsDefaultButton) {
					result = editorButtonEdit;
					break;
				}
			}
			return result;
		}
	}
	public class AnalysisEditControl : TestControlTextValidated<XtraTabControl> {
		public AnalysisEditControl(AnalysisControlWin control)
			: base(control.TabControl) {
		}
		protected override string GetText() {
			return control.SelectedTabPage.Text;
		}
		protected override void InternalSetText(string text) {
			if(!String.IsNullOrEmpty(text)) {
				foreach(XtraTabPage tabPage in control.TabPages) {
					if(tabPage.Text == text) {
						control.SelectedTabPage = tabPage;
						break;
					}
				}
			}
		}
		protected override string Validate(string text) {
			if(!String.IsNullOrEmpty(text)) {
				bool isFoundTabPage = false;
				string values = String.Empty;
				foreach(XtraTabPage tabPage in control.TabPages) {
					if(tabPage.Text == text) {
						isFoundTabPage = true;
						break;
					}
					values += Environment.NewLine + tabPage.Text;
				}
				if(!isFoundTabPage) {
					return String.Format("Requested tab page {0} was not found among the available tab pages: {1}", text, values);
				}
			}
			return base.Validate(text);
		}
	}
	public class DateNavigatorControl : TestControlInterfaceImplementerBase<DateNavigator>, IControlAct {
		public DateNavigatorControl(DateNavigator control)
			: base(control) {
		}
		public void Act(string parameterValue) {
			DateTime date = DateTime.Parse(parameterValue);
			if(!DateTime.TryParse(parameterValue, out date)) {
				throw new WarningException(String.Format("Cannot convert the '{0}' string to a DateTime structure", parameterValue));
			}
			control.SchedulerControl.GoToDate(date);
		}
	}
	public class MemoExEditTextControl : TextEditControl<MemoExEdit> {
		public MemoExEditTextControl(MemoExEdit control)
			: base(control) {
		}
	}
	public class RtfEditExTextControl : TextEditControl<RtfEditEx> {
		public RtfEditExTextControl(RtfEditEx control)
			: base(control) {
		}
		protected override void InternalSetText(string text) {
			RichTextBox rtfBox = new RichTextBox();
			rtfBox.Text = text;
			base.InternalSetText(rtfBox.Rtf);
		}
	}
	public class ExpressionEditControl : TextEditControl<PopupExpressionEdit> {
		public ExpressionEditControl(PopupExpressionEdit control)
			: base(control) {
		}
		protected override string Validate(string text) {
			string result = ValidateMaxLength(control.Properties.MaxLength, text);
			if(!control.Enabled) {
				return ControlIsReadOnly;
			}
			if(!string.IsNullOrEmpty(result)) {
				return result;
			}
			return base.Validate(text);
		}
	}
	public class PopupBaseEditControl : TestControlInterfaceImplementerBase<PopupBaseEdit>, IControlAct {
		public PopupBaseEditControl(ImageEdit control) :
			base(control) {
		}
		#region IControlAct Members
		private static object GetNonPublicPropertyValue(object instance, string propertyName) {
			PropertyInfo propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
			return propertyInfo.GetValue(instance, new object[0]);
		}
		public void Act(string value) {
			if(!control.IsPopupOpen) {
				control.ShowPopup();
			}
			if(!string.IsNullOrEmpty(value)) {
				PopupBaseForm popupForm = (PopupBaseForm)GetNonPublicPropertyValue(control, "PopupForm");
				Guard.ArgumentNotNull(popupForm, "popupForm");
				object embeddedControl = GetNonPublicPropertyValue(popupForm, "EmbeddedControl");
				ITestControl testControl = TestControlFactoryWin.Instance.CreateControl(embeddedControl);
				testControl.GetInterface<IControlAct>().Act(value);
			}
		}
		#endregion
	}
	public class PictureEditControl : TestControlInterfaceImplementerBase<PictureEdit>, IControlAct {
		public PictureEditControl(PictureEdit control) :
			base(control) {
		}
		#region IControlAct Members
		public void Act(string value) {
			control.Focus();
			PropertyInfo menuPropertyInfo = control.GetType().GetProperty("Menu", BindingFlags.NonPublic | BindingFlags.Instance);
			PictureMenu pictureMenu = (PictureMenu)menuPropertyInfo.GetValue(control, new object[0]);
			Guard.ArgumentNotNull(pictureMenu, "popupForm");
			foreach(DXMenuItem menuItem in pictureMenu.Items) {
				if(menuItem.Caption == value) {
					menuItem.GenerateClickEvent();
					return;
				}
			}
			throw new AdapterOperationException(String.Format("A context menu doesn't contain the '{0}' action item", value));
		}
		#endregion
	}
}
