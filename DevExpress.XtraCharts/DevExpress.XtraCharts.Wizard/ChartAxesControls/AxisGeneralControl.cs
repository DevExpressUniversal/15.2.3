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
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class AxisGeneralControl : ChartUserControl {
		abstract class AxisBaseManager {
			static void AddSeparator(ChartPanelControl separator, List<Control> activeControls) {
				if (separator != null)
					activeControls.Add(separator);
			}
			public static AxisBaseManager CreateInstance(IAxisData axis, AxisGeneralControl control, Chart chart, Action0 changeAxisNameMethod) {
				if (axis is Axis) {
					if (axis is AxisX || axis is AxisY)
						return new AxisManager((Axis)axis, control, chart);
					if (axis is SecondaryAxisX || axis is SecondaryAxisY)
						return new SecondaryAxisManager((Axis)axis, control, chart, changeAxisNameMethod);
				}
				if(axis is SwiftPlotDiagramAxis) {
					if(axis is SwiftPlotDiagramAxisX || axis is SwiftPlotDiagramAxisY)
						return new SwiftPlotDiagramAxisManager((SwiftPlotDiagramAxis)axis, control, chart);
					if(axis is SwiftPlotDiagramSecondaryAxisX || axis is SwiftPlotDiagramSecondaryAxisY)
						return new SwiftPlotDiagramSecondaryAxisManager((SwiftPlotDiagramAxis)axis, control, chart, changeAxisNameMethod);
				}
				if (axis is Axis3D)
					return new Axis3DManager((Axis3D)axis, control, chart);
				if (axis is RadarAxis) {
					if (axis is RadarAxisX)
						return (axis is PolarAxisX) ? new PolarAxisXManager((PolarAxisX)axis, control, chart) : 
													  new RadarAxisXManager((RadarAxisX)axis, control, chart);
					if (axis is RadarAxisY)
						return new RadarAxisYManager((RadarAxisY)axis, control, chart);
				}
				ChartDebug.Fail("Unknown axis type.");
				return null;
			}
			readonly AxisBase axis;
			readonly AxisGeneralControl control;
			readonly Chart chart;
			int lockCounter;
			public bool Locked {
				get {
					ChartDebug.Assert(lockCounter >= 0, "Invalid lock state");
					return lockCounter > 0;
				}
			}
			protected AxisBase Axis { get { return axis; } }
			protected AxisGeneralControl Control { get { return control; } }
			protected ActualScaleType ScaleType { get { return ((IAxisData)axis).AxisScaleTypeMap.ScaleType; } }
			protected abstract bool VisibleSupported { get; }
			protected abstract bool TopLevelSupported { get; }
			protected abstract bool NameSupported { get; }
			protected abstract bool VisibleInPanesSupported { get; }
			protected abstract bool PositionSupported { get; }
			protected virtual bool ReverseSupported { get { return false; } }
			protected virtual bool LogarithmicOptionsSupported { get { return ScaleType == ActualScaleType.Numerical; } }
			protected AxisBaseManager(AxisBase axis, AxisGeneralControl control, Chart chart) {
				this.axis = axis;
				this.control = control;
				this.chart = chart;
			}
			void Lock() {
				lockCounter++;
			}
			void Unlock() {
				lockCounter--;
			}
			IList<Control> GetActiveControls() {
				List<Control> activeControls = new List<Control>();
				ChartPanelControl separator = null;
				if (LogarithmicOptionsSupported) {
					activeControls.Add(control.pnlLogarithmicOptions);
					separator = control.sepLogarihmicOptions;
				}
				if (PositionSupported) {
					AddSeparator(separator, activeControls);
					activeControls.Add(control.pnlPosition);
					separator = control.sepPosition;
					if(!ReverseSupported) {
						control.pnlReverse.Visible = false;
						control.sepAlignment.Visible = false;
					}
				}
				if (VisibleInPanesSupported) {
					AddSeparator(separator, activeControls);
					activeControls.Add(control.pnlVisibleInPanes);
					separator = control.sepVisibleInpanes;
				}
				if (NameSupported) {
					AddSeparator(separator, activeControls);
					activeControls.Add(control.pnlName);
				}
				separator = control.sepName;
				if (TopLevelSupported) {
					AddSeparator(separator, activeControls);
					activeControls.Add(control.pnlTopLevel);
					separator = null;
				}
				if (VisibleSupported) {
					AddSeparator(separator, activeControls);
					activeControls.Add(control.pnlVisible);
					separator = null;
				}
				return activeControls;
			}
			void UpdateControls() {
				Lock();
				try {
					UpdateVisible();
					UpdateTopLevel();
					UpdateName();
					UpdatePosition();
					UpdateLogarithmicOptions();
				}
				finally {
					Unlock();
				}
			}
			void UpdateLogarithmicOptions() {
				control.chLogarithmic.Checked = axis.Logarithmic;
				control.spnLogarithmicBase.EditValue = axis.LogarithmicBase;
				control.spnLogarithmicBase.Enabled = axis.Logarithmic;
			}
			protected virtual void UpdateVisible() {
			}
			protected virtual void UpdateTopLevel() {
			}
			protected virtual void UpdateName() {
			}
			protected virtual void UpdatePosition() {
			}
			public virtual void ChangeVisible() {
			}
			public virtual void ChangeTopLevel() {
			}
			public virtual void ChangeName() {
			}
			public virtual void ChangeVisibilityInPanes() {
			}
			public virtual void ChangeReverse() {
			}
			public virtual void ChangeAlignment() {
			}
			public void ChangeLogarithmic() {
				axis.Logarithmic = control.chLogarithmic.Checked;
				UpdateControls();
			}
			public void ChangeLogarithmicBase() {
				axis.LogarithmicBase = Convert.ToDouble(control.spnLogarithmicBase.EditValue);
			}
			public bool Initialize() {
				control.Hide();
				try {
					control.Controls.Clear();
					IList<Control> activeControls = GetActiveControls();
					if (activeControls.Count == 0)
						return false;
					foreach (Control ctrl in activeControls) {
						control.Controls.Add(ctrl);
						ctrl.SendToBack();
					}
					UpdateControls();
				}
				finally {
					control.Show();
				}
				return true;
			}
		}
		abstract class Axis2DManager : AxisBaseManager {
			struct AxisAlignmentItem {
				readonly AxisAlignment alignment;
				readonly string text;
				public AxisAlignment Alignment { get { return alignment; } }
				public AxisAlignmentItem(AxisAlignment alignment) {
					this.alignment = alignment;
					switch (alignment) {
						case AxisAlignment.Near:
							text = ChartLocalizer.GetString(ChartStringId.WizAxesAlignmentNear);
							break;
						case AxisAlignment.Far:
							text = ChartLocalizer.GetString(ChartStringId.WizAxesAlignmentFar);
							break;
						case AxisAlignment.Zero:
							text = ChartLocalizer.GetString(ChartStringId.WizAxesAlignmentZero);
							break;
						default:
							ChartDebug.Fail("Unknown axis alignment.");
							text = ChartLocalizer.GetString(ChartStringId.WizAxesAlignmentNear);
							break;
					}
				}
				public override string ToString() {
					return text;
				}
				public override bool Equals(object obj) {
					return (obj is AxisAlignmentItem) && alignment == ((AxisAlignmentItem)obj).alignment;
				}
				public override int GetHashCode() {
					return alignment.GetHashCode();
				}
			}
			protected new Axis2D Axis { get { return (Axis2D)base.Axis; } }
			protected override bool VisibleSupported { get { return true; } }
			protected override bool TopLevelSupported { get { return false; } }
			protected override bool NameSupported { get { return true; } }
			protected override bool VisibleInPanesSupported { get { return true; } }
			protected override bool PositionSupported { get { return true; } }
			protected virtual bool NameEnabled { get { return false; } }
			public Axis2DManager(Axis2D axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
			protected override void UpdateVisible() {
				CheckEditHelper.SetCheckEditState(Control.chVisible, Axis.Visibility);
			}
			protected override void UpdateName() {
				Control.txtName.Text = Axis.Name;
				Control.pnlName.Enabled = NameEnabled;
			}
			protected override void UpdatePosition() {				
				Control.cmbAlignment.Properties.Items.Clear();
				Control.cmbAlignment.Properties.Items.Add(new AxisAlignmentItem(AxisAlignment.Near));
				Control.cmbAlignment.Properties.Items.Add(new AxisAlignmentItem(AxisAlignment.Far));
				if (!CommonUtils.IsShouldFilterZeroAlignment((Axis2D)Axis))
					Control.cmbAlignment.Properties.Items.Add(new AxisAlignmentItem(AxisAlignment.Zero));
				Control.cmbAlignment.SelectedItem = new AxisAlignmentItem(Axis.Alignment);
			}
			public override void ChangeVisible() {
				Axis.Visibility = CheckEditHelper.GetCheckEditState(Control.chVisible);
			}
			public override void ChangeVisibilityInPanes() {
				Chart chart = ChartDesignHelper.GetOwner<Chart>(Axis);
				using (AxisVisibilityInPanesForm form = new AxisVisibilityInPanesForm(Axis.VisibilityInPanes, chart)) {
					form.ShowDialog();
				}
			}
			public override void ChangeAlignment() {
				Axis.Alignment = ((AxisAlignmentItem)Control.cmbAlignment.SelectedItem).Alignment;
			}
		}
		class AxisManager : Axis2DManager {
			protected new Axis Axis { get { return (Axis)base.Axis; } }
			protected override bool ReverseSupported { get { return true; } }
			public AxisManager(Axis axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) { }
			protected override void UpdatePosition() {
				Control.chReverse.Checked = Axis.Reverse;
				base.UpdatePosition();
			}
			public override void ChangeReverse() {
				Axis.Reverse = Control.chReverse.Checked;
			}
		}
		class SecondaryAxisManager : AxisManager {
			readonly Action0 changeAxisNameMethod;
			protected override bool NameEnabled { get { return true; } }
			public SecondaryAxisManager(Axis axis, AxisGeneralControl control, Chart chart, Action0 changeAxisNameMethod) : base(axis, control, chart) {
				this.changeAxisNameMethod = changeAxisNameMethod;
			}
			public override void ChangeName() {
				if(changeAxisNameMethod != null) {
					Axis.Name = Control.txtName.EditValue.ToString();
					changeAxisNameMethod();
				}
			}
		}
		class SwiftPlotDiagramAxisManager : Axis2DManager {
			public SwiftPlotDiagramAxisManager(SwiftPlotDiagramAxis axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) { }
		}
		class SwiftPlotDiagramSecondaryAxisManager : SwiftPlotDiagramAxisManager {
			readonly Action0 changeAxisNameMethod;
			protected override bool NameEnabled { get { return true; } }
			public SwiftPlotDiagramSecondaryAxisManager(SwiftPlotDiagramAxis axis, AxisGeneralControl control, Chart chart, Action0 changeAxisNameMethod) : base(axis, control, chart) {
				this.changeAxisNameMethod = changeAxisNameMethod;
			}
			public override void ChangeName() {
				if (changeAxisNameMethod != null) {
					Axis.Name = Control.txtName.EditValue.ToString();
					changeAxisNameMethod();
				}
			}
		}
		class Axis3DManager : AxisBaseManager {
			protected override bool VisibleSupported { get { return false; } }
			protected override bool TopLevelSupported { get { return false; } }
			protected override bool NameSupported { get { return false; } }
			protected override bool VisibleInPanesSupported { get { return false; } }
			protected override bool PositionSupported { get { return false; } }
			public Axis3DManager(Axis3D axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
		}
		abstract class RadarAxisManager : AxisBaseManager {
			protected override bool NameSupported { get { return false; } }
			protected override bool VisibleInPanesSupported { get { return false; } }
			protected override bool PositionSupported { get { return false; } }
			public RadarAxisManager(RadarAxis axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
		}
		class RadarAxisXManager : RadarAxisManager {
			protected override bool VisibleSupported { get { return false; } }
			protected override bool TopLevelSupported { get { return false; } }
			public RadarAxisXManager(RadarAxisX axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
		}
		class RadarAxisYManager : RadarAxisManager {
			protected new RadarAxisY Axis { get { return (RadarAxisY)base.Axis; } }
			protected override bool VisibleSupported { get { return true; } }
			protected override bool TopLevelSupported { get { return true; } }
			public RadarAxisYManager(RadarAxisY axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
			protected override void UpdateVisible() {
				Control.chVisible.Checked = Axis.Visible;
			}
			protected override void UpdateTopLevel() {
				Control.chTopLevel.Checked = Axis.TopLevel;
			}
			public override void ChangeVisible() {
				Axis.Visible = Control.chVisible.Checked;
			}
			public override void ChangeTopLevel() {
				Axis.TopLevel = Control.chTopLevel.Checked;
			}
		}
		class PolarAxisXManager : RadarAxisXManager {
			protected override bool LogarithmicOptionsSupported { get { return false; } }
			public PolarAxisXManager(PolarAxisX axis, AxisGeneralControl control, Chart chart) : base(axis, control, chart) {
			}
		}
		AxisGeneralTabsControl tabsControl;
		AxisBaseManager manager;
		public AxisGeneralTabsControl TabsControl { get { return tabsControl; } }
		bool ManagerEnabled { get { return manager != null && !manager.Locked; } }
		public AxisGeneralControl() {
			InitializeComponent();
		}
		void chVisible_CheckStateChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeVisible();
		}
		void chTopLevel_CheckedChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeTopLevel();
		}
		void txtName_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if (txtName.EditValue.ToString() == "") {
				e.Cancel = true;
				txtName.ErrorText = ChartLocalizer.GetString(ChartStringId.MsgEmptySecondaryAxisName);
			}
			else
				if (ManagerEnabled)
					manager.ChangeName();
		}
		void btnVisibleInPanes_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			if (ManagerEnabled)
				manager.ChangeVisibilityInPanes();
		}
		void chReverse_CheckedChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeReverse();
		}
		void cmbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeAlignment();
		}
		void chLogarithmic_CheckedChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeLogarithmic();
		}
		void spnLogarithmicBase_EditValueChanged(object sender, EventArgs e) {
			if (ManagerEnabled)
				manager.ChangeLogarithmicBase();
		}
		public bool Initialize(AxisGeneralTabsControl tabsControl, IAxisData axis, Chart chart, Action0 changeAxisNameMethod) {
			this.tabsControl = tabsControl;
			manager = AxisBaseManager.CreateInstance(axis, this, chart, changeAxisNameMethod);
			return manager != null && manager.Initialize();
		}
	}
}
