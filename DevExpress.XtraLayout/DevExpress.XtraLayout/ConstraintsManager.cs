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
using System.IO;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Controls;
using DevExpress.Utils;
using DXUtils = DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Printing;
using DevExpress.Accessibility;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Registrator;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Adapters;
using DevExpress.XtraLayout.Customization;
using Padding = DevExpress.XtraLayout.Utils.Padding;
using DevExpress.XtraLayout.Accessibility;
using System.Collections.Generic;
namespace DevExpress.XtraLayout {
	[DesignerCategory("Component")]
	public class ConstraintsManager : IDisposable {
		[ThreadStatic]
		static ConstraintsManager defaultCore;
		public static ConstraintsManager Default {
			get {
				if(defaultCore == null) defaultCore = new ConstraintsManager();
				return defaultCore;
			}
		}
		protected Size DefaultMaxSize = Size.Empty;
		protected Size DefaultMinSize = new Size(100, 20);
		protected Size DefaultNet20ControlMinSize = new Size(20, 20);
		Dictionary<Type, ControlConstraints> standartControlsDefaultContstraints;
		Hashtable commonControlsContstraints;
		public ConstraintsManager() {
			CreateControlsConstraintsList();
			commonControlsContstraints = new Hashtable(10, new EqualityComparer());
		}
		void IDisposable.Dispose() {
			if (commonControlsContstraints != null) {
				commonControlsContstraints.Clear();
			}
			if (defaultCore != null && defaultCore != this) ((IDisposable)defaultCore).Dispose();
		}
		public Size GetDefaultMaxSize() {
			return DefaultMaxSize;
		}
		public Size GetDefaultMinSize() {
			return DefaultMinSize;
		}
		public Size CorrectValueByDefaultNet20ControlMinSize(Size size) {
			if(size.Width == 0) size.Width = DefaultNet20ControlMinSize.Width;
			if(size.Height == 0) size.Height = DefaultNet20ControlMinSize.Height;
			return size;
		}
		public Size CorrectValueByDefMaxSize(Size size) {
			if(size.Width == 0) size.Width = DefaultMaxSize.Width;
			if(size.Height == 0) size.Height = DefaultMaxSize.Height;
			return size;
		}
		public struct ControlConstraints {
			public System.Type type;
			public Size maxSize;
			public Size minSize;
			public bool isCaptionVisible;
			public ControlConstraints(System.Type type, Size maxsize, Size minsize, bool IsCaptionVisible) {
				this.type = type;
				this.maxSize = maxsize;
				this.minSize = minsize;
				this.isCaptionVisible = IsCaptionVisible;
			}
		}
		public void RegisterControlConstraints(Type type, Size minSize, Size maxSize) {
			RegisterControlConstraints(type, minSize, maxSize, true);
		}
		public void RegisterControlConstraints(Type type, Size minSize, Size maxSize, bool IsCaptionVisible) {
			standartControlsDefaultContstraints.Add(type, new ControlConstraints(type, maxSize, minSize, IsCaptionVisible));
		}
		void CreateControlsConstraintsList() {
			standartControlsDefaultContstraints = new Dictionary<Type, ControlConstraints>();
			standartControlsDefaultContstraints.Add(typeof(Label), new ControlConstraints(typeof(Label), new Size(0, 0), new Size(20, 20), false));
			standartControlsDefaultContstraints.Add(typeof(LinkLabel), new ControlConstraints(typeof(LinkLabel), new Size(0, 20), new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(Button), new ControlConstraints(typeof(Button), Size.Empty, new Size(20, 20), false));
			standartControlsDefaultContstraints.Add(typeof(TextBox), new ControlConstraints(typeof(TextBox), new Size(0, 20), new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(CheckBox), new ControlConstraints(typeof(CheckBox), new Size(0, 20), new Size(20, 20), false));
			standartControlsDefaultContstraints.Add(typeof(RadioButton), new ControlConstraints(typeof(RadioButton), new Size(0, 25), new Size(20, 25), false));
			standartControlsDefaultContstraints.Add(typeof(ListBox), new ControlConstraints(typeof(ListBox), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(CheckedListBox), new ControlConstraints(typeof(CheckedListBox), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(System.Windows.Forms.ComboBox), new ControlConstraints(typeof(System.Windows.Forms.ComboBox), new Size(0, 21), new Size(20, 21), true));
			standartControlsDefaultContstraints.Add(typeof(ListView), new ControlConstraints(typeof(ListView), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(TreeView), new ControlConstraints(typeof(TreeView), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(TabControl), new ControlConstraints(typeof(TabControl), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(DateTimePicker), new ControlConstraints(typeof(DateTimePicker), new Size(0, 20), new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(MonthCalendar), new ControlConstraints(typeof(MonthCalendar), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(DomainUpDown), new ControlConstraints(typeof(DomainUpDown), new Size(0, 20), new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(NumericUpDown), new ControlConstraints(typeof(NumericUpDown), new Size(0, 20), new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(TrackBar), new ControlConstraints(typeof(TrackBar), Size.Empty, new Size(20, 20), false));
			standartControlsDefaultContstraints.Add(typeof(System.Windows.Forms.ProgressBar), new ControlConstraints(typeof(System.Windows.Forms.ProgressBar), Size.Empty, new Size(20, 20), false));
			standartControlsDefaultContstraints.Add(typeof(RichTextBox), new ControlConstraints(typeof(RichTextBox), Size.Empty, new Size(20, 20), true));
			standartControlsDefaultContstraints.Add(typeof(System.Windows.Forms.BindingNavigator), new ControlConstraints(typeof(BindingNavigator), new Size(0, 30), new Size(100, 30), false));
		}
		public bool TextVisible(object control) {
			GetMinMaxSize(control, true);
			return lastconstraints.isCaptionVisible;
		}
		ControlConstraints lastconstraints;
		public Size GetMinMaxSize(object control, bool returnMin) {
			lastconstraints = GetMinMaxSizeInternal(control);
			return returnMin ? lastconstraints.minSize : lastconstraints.maxSize;
		}
		public void UpdateIResizableConstraints(Control control) {
			if(commonControlsContstraints.Contains(control))
				commonControlsContstraints.Remove(control);
		}
		ControlConstraints GetIXtraResizableControlConstraints(IXtraResizableControl rcontrol) {
			if(commonControlsContstraints.Contains(rcontrol))
				return (ControlConstraints)commonControlsContstraints[rcontrol];
			else {
				WeakReference rcontrolRef = new WeakReference(rcontrol);
				ControlConstraints constraints = new ControlConstraints(null, rcontrol.MaxSize, rcontrol.MinSize, rcontrol.IsCaptionVisible);
				commonControlsContstraints.Add(rcontrolRef, constraints);
				return constraints;
			}
		}
		class EqualityComparer : IEqualityComparer {
			int CompareCore(object o1, object o2) {
				WeakReference wref1 = o1 as WeakReference;
				Control control = o2 as Control;
				if(wref1 != null && control != null && wref1.IsAlive) {
					if(wref1.Target == control) {
						return 0;
					}
				}
				return -1;
			}
			public int Compare(object o1, object o2) {
				int result = CompareCore(o1, o2);
				if(result == 0) return 0;
				result = CompareCore(o2, o1);
				if(result == 0) return 0;
				return result;
			}
			bool IEqualityComparer.Equals(object a, object b) {
				return Compare(a, b) == 0;
			}
			int IEqualityComparer.GetHashCode(object obj) {
				Control control = obj as Control;
				if(control != null) {
					return control.GetHashCode();
				}
				WeakReference wref = obj as WeakReference;
				if(wref != null && wref.IsAlive)
					return wref.Target.GetHashCode();
				return 0;
			}
		}
		protected int CalcMaxConstraint(int maximum, int minimum) {
			if(maximum == 0) return 0;
			else return Math.Max(minimum, maximum);
		}
		protected ControlMaxSizeCalcMode GetSizeMode(Control net20Control) {
			if(net20Control == null || net20Control.Parent == null) return ControlMaxSizeCalcMode.CombineControlMaximumSizeAndIXtraResizableMaxSize;
			LayoutControl lc = net20Control.Parent as LayoutControl;
			if(lc == null) return ControlMaxSizeCalcMode.CombineControlMaximumSizeAndIXtraResizableMaxSize;
			else return lc.OptionsView.ControlDefaultMaxSizeCalcMode;
		}
		protected virtual ControlConstraints GetMinMaxSizeInternal(object control) {
			Control net20Control = control as Control;
			IXtraResizableControl rcontrol = control as IXtraResizableControl;
			ControlConstraints iXtraResizableControlConstraints = new ControlConstraints();
			bool iXtraResizableControlConstraintsInitialized = false;
			if (rcontrol != null && net20Control != null && net20Control.Parent != null && net20Control.Parent is ILayoutControl) {
				iXtraResizableControlConstraints = GetIXtraResizableControlConstraints(rcontrol);
				iXtraResizableControlConstraintsInitialized = true;
			}
			if (net20Control != null && (net20Control.MinimumSize != Size.Empty || net20Control.MaximumSize != Size.Empty)) {
				return GetControlConstraintsFromControl(net20Control, ref iXtraResizableControlConstraints, iXtraResizableControlConstraintsInitialized);
			}
			if (iXtraResizableControlConstraintsInitialized)
				return iXtraResizableControlConstraints;
			if (control == null) {
				return new ControlConstraints(null, GetDefaultMaxSize(), GetDefaultMinSize(), true);
			}
			Type controlType = control.GetType();
			ControlConstraints constraints;
			bool constraintsResolved = false;
			if (standartControlsDefaultContstraints.TryGetValue(controlType, out constraints)) {
				constraints = new ControlConstraints(controlType,
				constraints.maxSize,
				constraints.minSize,
				constraints.isCaptionVisible);
				if (control is TextBox && ((TextBox)control).Multiline)
					constraints.maxSize = Size.Empty;
				constraintsResolved = true;
			}
			if (!constraintsResolved && controlType != typeof(Control)) {
				Type tempType = controlType;
				do {
					constraintsResolved = standartControlsDefaultContstraints.TryGetValue(tempType, out constraints);
					tempType = tempType.BaseType;
				}
				while (tempType != typeof(Control) && !constraintsResolved);
			}
			Control sControl = control as Control;
			if (constraintsResolved) {
				return new ControlConstraints(controlType, CorrectValueByDefMaxSize(constraints.maxSize), CorrectValueByDefMaxSize(constraints.minSize), constraints.isCaptionVisible);
			}
			else {
				return new ControlConstraints(typeof(String), GetDefaultMaxSize(), GetDefaultMinSize(), true);
			}
		}
		private ControlConstraints GetControlConstraintsFromControl(Control net20Control, ref ControlConstraints iXtraResizableControlConstraints, bool iXtraResizableControlConstraintsInitialized) {
			Size min = Size.Empty, max = Size.Empty;
			min.Width = Math.Max(net20Control.MinimumSize.Width, iXtraResizableControlConstraints.minSize.Width);
			min.Height = Math.Max(net20Control.MinimumSize.Height, iXtraResizableControlConstraints.minSize.Height);
			min = CorrectValueByDefaultNet20ControlMinSize(min);
			int cW = 0, cH = 0;
			switch(GetSizeMode(net20Control)) {
				case ControlMaxSizeCalcMode.CombineControlMaximumSizeAndIXtraResizableMaxSize:
					cW = Math.Max(net20Control.MaximumSize.Width, iXtraResizableControlConstraints.maxSize.Width);
					cH = Math.Max(net20Control.MaximumSize.Height, iXtraResizableControlConstraints.maxSize.Height);
					max = new Size(CalcMaxConstraint(cW, min.Width), CalcMaxConstraint(cH, min.Height));
					break;
				case ControlMaxSizeCalcMode.UseControlMaximumSize:
					cW = net20Control.MaximumSize.Width;
					cH = net20Control.MaximumSize.Height;
					max = new Size(CalcMaxConstraint(cW, net20Control.MinimumSize.Width), CalcMaxConstraint(cH, net20Control.MinimumSize.Height));
					break;
				case ControlMaxSizeCalcMode.UseControlMaximumSizeIfNotZero:
					cW = net20Control.MaximumSize.Width == 0 ? iXtraResizableControlConstraints.maxSize.Width : net20Control.MaximumSize.Width;
					cH = net20Control.MaximumSize.Height == 0 ? iXtraResizableControlConstraints.maxSize.Height : net20Control.MaximumSize.Height;
					max = new Size(CalcMaxConstraint(cW, net20Control.MaximumSize.Width == 0 ? min.Width : net20Control.MinimumSize.Width), CalcMaxConstraint(cH, net20Control.MaximumSize.Height == 0 ? min.Height : net20Control.MinimumSize.Height));
					break;
			}
			Type type = net20Control.GetType();
			bool isCaptionVisible = true;
			if(iXtraResizableControlConstraintsInitialized) {
				isCaptionVisible = iXtraResizableControlConstraints.isCaptionVisible;
			} else {
				if(standartControlsDefaultContstraints.ContainsKey(type))
					isCaptionVisible = ((ControlConstraints)standartControlsDefaultContstraints[type]).isCaptionVisible;
			}
			return new ControlConstraints(type, max, min, isCaptionVisible);
		}
	}
}
