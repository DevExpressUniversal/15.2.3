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
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.EasyTest.Framework;
using System.Collections.Generic;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public abstract class FindStrategy {
		public virtual List<object> GetCompatibleControls(Control control) {
			return new List<object>();
		}
		public abstract EasyTestException CreateDuplicateException();
	}
	public class TagFindStrategy : FindStrategy {
		public const string DuplicateMessage = "There are multiple control with the '{0}' name. Use one of full name variant's: {1}";
		private List<string> controlTags;
		private string testTagType;
		private string name;
		string tag;
		private string GetTag(string testTagType, string name) {
			return "test" + testTagType.ToLower() + "=" + name;
		}
		public override List<object> GetCompatibleControls(Control control) {
			List<object> result = base.GetCompatibleControls(control);
			if(control != null && control.Tag != null) {
				string testTag = tag.ToString().Replace("\"", "").ToUpper();
				string[] tags = control.Tag.ToString().ToUpper().Split(';');
				foreach(string controlTag in tags) {
					string controlTag2 = controlTag.Replace("\"", "");
					if(controlTag2 == testTag || ButtonEditControl.GetDisplayCaption(controlTag2) == testTag ||
						(name == null && controlTag2.StartsWith(tag.ToUpper()))) {
						result.Add(control);
						int index = control.Tag.ToString().IndexOf('=') + 1;
						controlTags.Add("'" + control.Tag.ToString().Substring(index) + "'");
					}
				}
			}
			return result;
		}
		public TagFindStrategy(string testTagType, string name) {
			this.testTagType = testTagType;
			this.name = name;
			this.tag = GetTag(testTagType, name);
			controlTags = new List<string>();
		}
		public override EasyTestException CreateDuplicateException() {
			throw new EasyTestException(string.Format(DuplicateMessage, tag.Substring(tag.IndexOf('=') + 1), string.Join(", ", controlTags.ToArray())));
		}
	}
	public abstract class ValueFindStrategy : FindStrategy {
		string value;
		Type type;
		public override List<object> GetCompatibleControls(Control control) {
			List<object> result = base.GetCompatibleControls(control);
			if(type.IsAssignableFrom(control.GetType())) {
				if(value == null || GetValueToCompare(control) == value) {
					result.Add(control);
				}
			}
			return result;
		}
		protected abstract string GetValueToCompare(Control control);
		public ValueFindStrategy(Type type, string value) {
			this.type = type;
			this.value = value;
		}
		public override EasyTestException CreateDuplicateException() {
			if(String.IsNullOrEmpty(value)) {
				throw new EasyTestException(string.Format(
					"There are multiplecontrols of the '{0}' type. Please, specify the control name", type));
			}
			else {
				throw new EasyTestException(string.Format(
					"There are multiplecontrols of the '{0}' type and with the '{1}' identifier. Please, make the controls' names unique",
					type, value));
			}
		}
	}
	public class NameFindStrategy : ValueFindStrategy {
		protected override string GetValueToCompare(Control control) {
			return control.Name;
		}
		public NameFindStrategy(Type type, string name)
			: base(type, name) {
		}
	}
	public class TextFindStrategy : ValueFindStrategy {
		protected override string GetValueToCompare(Control control) {
			string displayCaption = ButtonEditControl.GetDisplayCaption(control.Text);
			return displayCaption != null ? displayCaption.Trim() : null;
		}
		public TextFindStrategy(Type type, string text)
			: base(type, text) {
		}
	}
	public class LayoutTabFindStrategy {
		string name;
		ArrayList controls = new ArrayList();
		private void GetCompatibleControls(DevExpress.XtraLayout.Utils.BaseItemCollection items) {
			int i = 0;
			while(i < items.Count) {
				BaseLayoutItem item = items[i++];
				if(item is LayoutControlGroup) {
					LayoutControlGroup layoutControlGroup = (LayoutControlGroup)item;
					GetCompatibleControls(layoutControlGroup.Items);
				}
				if(item is TabbedGroup) {
					TabbedGroup tabbedGroup = (TabbedGroup)item;
					int j = 0;
					while(j < tabbedGroup.TabPages.Count) {
						LayoutGroup tab = tabbedGroup.TabPages[j++];
						if(name == null || ButtonEditControl.GetDisplayCaption(tab.Text) == name) {
							if(controls.IndexOf(tab) == -1) {
								controls.Add(tab);
							}
						}
					}
				}
			}
		}
		public object[] GetCompatibleControls(LayoutControl control) {
			controls.Clear();
			GetCompatibleControls(control.Items);
			return (object[])controls.ToArray(typeof(object));
		}
		public LayoutTabFindStrategy(string name) {
			this.name = name;
		}
	}
	public class LayoutControlFindStrategy : FindStrategy {
		string name;
		LayoutTabFindStrategy layoutTabFindStrategy;
		public override List<object> GetCompatibleControls(Control control) {
			List<Object> result = base.GetCompatibleControls(control);
			if(typeof(LayoutControl).IsAssignableFrom(control.GetType())) {
				result.AddRange(layoutTabFindStrategy.GetCompatibleControls((LayoutControl)control));
			}
			return result;
		}
		public LayoutControlFindStrategy(string name) {
			this.name = name;
			this.layoutTabFindStrategy = new LayoutTabFindStrategy(name);
		}
		public override EasyTestException CreateDuplicateException() {
			throw new EasyTestException(string.Format(
				"There are multiple controls of the '{0}' type. Please, specify the control name", name));
		}
	}
	public class XtraFormFindStrategy : FindStrategy {
		string name;
		public XtraFormFindStrategy(string name) {
			this.name = name;
		}
		public override List<object> GetCompatibleControls(Control control) {
			List<object> result = base.GetCompatibleControls(control);
			if(control is XtraForm && control.Text == name && control.Parent != null) {
				result.Add(control);
			}
			return result;
		}
		public override EasyTestException CreateDuplicateException() {
			throw new EasyTestException(string.Format(
				"There are multiple MDI tab of the '{0}'.", name));
		}
	}
	public class StandardControlFinder : IControlFinder {
		private ArrayList FindControls(Control parent, FindStrategy findStrategy) {
			ArrayList controls = new ArrayList();
			if(parent != null) {
				List<object> compatibleControls = findStrategy.GetCompatibleControls(parent);
				if(compatibleControls.Count != 0) {
					foreach(object control in compatibleControls) {
						if(control is Control) {
							if(IsControlVisible((Control)control)) {
								controls.Add(control);
							}
						}
						else {
							controls.Add(control);
						}
					}
				}
				else {
					foreach(Control childControl in parent.Controls) {
						controls.AddRange(FindControls(childControl, findStrategy));
					}
				}
			}
			return controls;
		}
		private object InternalFindControl(Control parent, FindStrategy findStrategy) {
			ArrayList controls = FindControls(parent, findStrategy);
			return CheckDuplicate(controls, findStrategy);
		}
		private static bool IsControlVisible(Control control) {
			if(!control.Visible) {
				return false;
			}
			while(control.Parent != null) {
				control = control.Parent;
				if(!control.Visible) {
					return false;
				}
			}
			return true;
		}
		private static object CheckDuplicate(ArrayList objects, FindStrategy findStrategy) {
			object foundedControl = null;
			if(objects.Count > 1) {
				foreach(object control in objects) {
					if(control is Control) {
						if(IsControlVisible((Control)control)) {
							if(foundedControl == null) {
								foundedControl = control;
							}
							else {
								throw findStrategy.CreateDuplicateException();
							}
						}
					}
					else {
						throw findStrategy.CreateDuplicateException();
					}
				}
			}
			else {
				if(objects.Count == 1) {
					foundedControl = objects[0];
				}
			}
			return foundedControl;
		}
		private BaseEdit FindEditorInVerticalGrid(Form form, string name) {
			string[] nameParts = name.Split('.');
			string gridName = nameParts[0];
			VGridControlBase verticalGrid = InternalFindControl(form, new TagFindStrategy(TestControlType.Field, gridName)) as VGridControlBase;
			if(verticalGrid == null) {
				verticalGrid = InternalFindControl(form, new NameFindStrategy(typeof(VGridControlBase), gridName)) as VGridControlBase;
			}
			if(verticalGrid != null) {
				string rowName = name.Substring(gridName.Length + 1);
				if(verticalGrid.FocusedRow != null && verticalGrid.FocusedRow.Properties.Caption != rowName) {
					verticalGrid.CloseEditor();
				}
				return FindEditorInVerticalGridRows(verticalGrid, verticalGrid.Rows, rowName);
			}
			return null;
		}
		private BaseEdit FindEditorInVerticalGridRows(VGridControlBase verticalGrid, VGridRows rows, string rowName) {
			foreach(BaseRow row in rows) {
				if(row.Properties.Caption == rowName) {
					verticalGrid.FocusedRow = row;
					verticalGrid.ShowEditor();
					return verticalGrid.ActiveEditor;
				}
				else {
					BaseEdit result = FindEditorInVerticalGridRows(verticalGrid, row.ChildRows, rowName);
					if(result != null) {
						return result;
					}
				}
			}
			return null;
		}
		public object Find(Form form, string contolType, string name) {
			List<ControlDescription> findControls = FindCore(form, contolType, name, null);
			if(findControls.Count == 0) {
				int separatorIndex = name.IndexOf('.');
				if(separatorIndex != -1) {
					string prefix = name.Substring(0, separatorIndex);
					name = name.Substring(separatorIndex + 1);
					findControls = FindCore(form, contolType, name, prefix);
				}
			}
			if(findControls.Count == 1) {
				return findControls[0].Control;
			} else {
				return CheckDuplicate(findControls, name);
			}
		}
		private Control GetNextControl(Control findControl) {
			bool isLabel = false;
			foreach(Control control in findControl.Parent.Controls) {
				if(!isLabel && object.ReferenceEquals(control, findControl)) {
					isLabel = true;
				}
				else if(isLabel) {
					return control;
				}
			}
			return null;
		}
		private void AddToResultIsFind(object control, string controlName, string prefix, List<ControlDescription> findControls) {
			if(control != null) {
				findControls.Add(new ControlDescription(control, controlName, prefix));
			}
		}
		protected class ControlDescription {
			public object Control;
			public string FullName;
			public ControlDescription(object control, string name, string prefix) {
				Control = control;
				FullName = prefix + "." + name;
			}
		}
		protected List<ControlDescription> FindCore(Form form, string contolType, string Name, string prefix) {
			List<ControlDescription> findControls = new List<ControlDescription>();
			switch(contolType) {
				case TestControlType.Action: {
						if(prefix == null || prefix == "Menu") {
							AddToResultIsFind(BarItemFindStrategy.Instance.FindControl(form, Name), Name, "Menu", findControls);
						}
						if(prefix == null || prefix == "Form") {
							AddToResultIsFind(InternalFindControl(form, new XtraFormFindStrategy(Name)), Name, "Form", findControls);
						}
						if(prefix == null || prefix == "XtraTab") {
							AddToResultIsFind(InternalFindControl(form, new TextFindStrategy(typeof(XtraTabPage), Name)), Name, "XtraTab", findControls);
						}
						if(prefix == null || prefix == "LayoutTab") {
							AddToResultIsFind(InternalFindControl(form, new LayoutControlFindStrategy(Name)), Name, "LayoutTab", findControls);
						}
						if(prefix == null || prefix == "TabPage") {
							AddToResultIsFind(InternalFindControl(form, new TextFindStrategy(typeof(TabPage), Name)), Name, "TabPage", findControls);
						}
						if(prefix == null || prefix == "Tag") {
							AddToResultIsFind(InternalFindControl(form, new TagFindStrategy(contolType, Name)), Name, "Tag", findControls);
						}
						if(prefix == null || prefix == "Button") {
							AddToResultIsFind(InternalFindControl(form, new TextFindStrategy(typeof(Button), Name)), Name, "Button", findControls);
						}
						if(findControls.Count == 0 && (prefix == null || prefix == "SimpleButton")) {
							AddToResultIsFind(InternalFindControl(form, new TextFindStrategy(typeof(SimpleButton), Name)), Name, "SimpleButton", findControls);
						}
						break;
					}
				case TestControlType.Table: {
						if(Name == "") {
							AddToResultIsFind(InternalFindControl(form, new TagFindStrategy(contolType, null)), Name, "Tag", findControls);
						}
						break;
					}
				case TestControlType.Field: {
						AddToResultIsFind(StatusBarFindStrategy.Instance.FindControl(form, Name), Name, "Field", findControls);
						if(findControls.Count == 0 && Name.Contains(".")) {
							AddToResultIsFind(FindEditorInVerticalGrid(form, Name), Name, "Field", findControls);
						}
						break;
					}
				case TestControlType.Dialog: {
						AddToResultIsFind(form, Name, "Dialog", findControls);
						break;
					}
			}
			if(prefix == null && !string.IsNullOrEmpty(Name)) {
				if(findControls.Count == 0) {
					AddToResultIsFind(InternalFindControl(form, new TagFindStrategy(contolType, Name)), Name, "Tag", findControls);
				}
				if(findControls.Count == 0 && contolType == TestControlType.Field) {
					AddToResultIsFind(BarItemFindStrategy.Instance.FindControl(form, Name), Name, "BarItem", findControls);
				}
				if(findControls.Count == 0) {
					object findControl = InternalFindControl(form, new TextFindStrategy(typeof(Control), Name));
					if(findControl != null && findControl is LabelControl) {
						findControl = GetNextControl((Control)findControl);
					}
					AddToResultIsFind(findControl, Name, "Text", findControls);
				}
				if(findControls.Count == 0) {
					AddToResultIsFind(InternalFindControl(form, new NameFindStrategy(typeof(Control), Name)), Name, "NameStr", findControls);
				}
			}
			return findControls;
		}
		private object CheckDuplicate(List<ControlDescription> controlDescriptions, string name) {
			if(controlDescriptions.Count > 0) {
				Dictionary<object, string> duplicates2 = new Dictionary<object, string>();
				foreach(ControlDescription description in controlDescriptions) {
					if(!duplicates2.ContainsKey(description.Control)) {
						duplicates2[description.Control] = "'" + description.FullName + "'";
					}
				}
				if(duplicates2.Count > 1) {
					List<String> duplicates = new List<string>();
					foreach(string fullName in duplicates2.Values) {
						duplicates.Add(fullName);
					}
					throw new EasyTestException(
						string.Format(TagFindStrategy.DuplicateMessage, name, string.Join(", ", duplicates.ToArray()))
						);
				} else {
					foreach(object result in duplicates2.Keys) {
						return result;
					}
				}
			}
			return null;
		}
	}
}
