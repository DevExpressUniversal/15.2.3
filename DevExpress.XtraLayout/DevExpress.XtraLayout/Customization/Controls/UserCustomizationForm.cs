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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.LookAndFeel;
using DevExpress.XtraLayout.Localization;
using System.Windows.Forms.Design;
using DevExpress.XtraEditors.Customization;
using DevExpress.XtraLayout.Customization.Controls;
using DevExpress.Utils.Design;
namespace DevExpress.XtraLayout.Customization {
	public interface ICustomizationFormControl {
		void Register();
		void UnRegister();
		ILayoutControl OwnerControl { get;}
		UserLookAndFeel ControlOwnerLookAndFeel { get;}
	}
	public interface ICustomizationContainer {
		void Register(ILayoutControl layoutControl);
		void UnRegister();
		ILayoutControl OwnerControl { get;}
	}
	[Flags]
	internal enum CustomizationFormStates : int {
		AllowActivate = 1,
		AllowSaveWidth = 2,
		AllowEverything = AllowSaveWidth | AllowActivate,
		AllowNothing = 0
	}
	[Designer("DevExpress.XtraLayout.Design.UserCustomizationFormDesigner, " + AssemblyInfo.SRAssemblyLayoutControlDesign, typeof(IRootDesigner))]
	public class UserCustomizationForm : CustomizationFormBase, ICustomizationContainer {
		protected ILayoutControl ownerControlCore;
		Point preferredLocation = Point.Empty;
		public UserCustomizationForm() {
			CustomizationMode = CustomizationModes.Default;
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				(this as ICustomizationContainer).UnRegister();
			}
			base.Dispose(disposing);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
		}
		void ICustomizationContainer.Register(ILayoutControl layoutControl) {
			ownerControlCore = layoutControl;
			if(layoutControl != null && layoutControl.DesignMode) ApplyDesignModeUI();
			NotifyChildrenILayoutControl(true, this);
			if(OwnerControl is LayoutControl) {
				(OwnerControl as LayoutControl).RightToLeftChanged += UserCustomizationForm_RightToLeftChanged;
			   RightToLeft = (OwnerControl as LayoutControl).RightToLeft;
			}
		}
		void UserCustomizationForm_RightToLeftChanged(object sender, EventArgs e) {
			RightToLeft = (sender as LayoutControl).RightToLeft;
		}
		void ApplyDesignModeUI() {
		}
		void ICustomizationContainer.UnRegister() {
			if(OwnerControl is LayoutControl) {
				(OwnerControl as LayoutControl).RightToLeftChanged -= UserCustomizationForm_RightToLeftChanged;
			}
			NotifyChildrenILayoutControl(false, this);
			ownerControlCore = null;
		}
		public virtual void ResetActiveControl(){}
		protected override void ShowCustomizationCore(Point location, bool calcLocation) {
			ShowCustomizationCore(location, calcLocation, false);
		}
		private void NotifyChildrenILayoutControl(bool register, Control target) {
			ArrayList controls = new ArrayList(target.Controls);
			ICustomizationFormControl customizationControl = target as ICustomizationFormControl;
			if(target is DXScrollableTreeView) controls.Add((target as DXScrollableTreeView).MaskTreeView);
			if(customizationControl != null)
				if(register) customizationControl.Register();
				else customizationControl.UnRegister();
			foreach(Control control in controls)
				NotifyChildrenILayoutControl(register, control);
		}
		public ILayoutControl OwnerControl { get { return ownerControlCore; } }
		protected virtual Size CalcCustomizationFormSize() {
			if (OwnerControl != null && OwnerControl.OptionsCustomizationForm.ShowPropertyGrid) return new Size((int)(Skins.DpiProvider.Default.DpiScaleFactor * 450), (int)(Skins.DpiProvider.Default.DpiScaleFactor * 350));
			else return new Size((int)(Skins.DpiProvider.Default.DpiScaleFactor * 250), (int)(Skins.DpiProvider.Default.DpiScaleFactor * 350));
		}
		protected virtual void CreateCustomizationForm() {
			this.Size = CalcCustomizationFormSize();
			this.Location = preferredLocation == Point.Empty ? new Point(30, 30) : preferredLocation;
			if(OwnerControl != null) {
				if(OwnerControl.DesignMode) this.TopMost = true;
				Form form = this.ControlOwner.FindForm();
				if(form != null && form.MdiParent == null)
					form.AddOwnedForm(this);
			}
			this.StartPosition = FormStartPosition.Manual;
		}
		protected internal virtual void CreateCustomization(Point location, bool setVisible) {
			preferredLocation = location;
			this.Visible = setVisible;
			ControlOwner.Focus();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CustomizationModes CustomizationMode { get; set; }
		protected override void OnClosing(CancelEventArgs e) {
			e.Cancel = true;
			if(OwnerControl != null && OwnerControl.UndoManager != null) {
				OwnerControl.UndoManager.Enabled = false;
			}
			ProcessClosing();
			base.OnClosing(e);
		}
		protected void ProcessClosing() {
			if (OwnerControl == null) return;
			Visible = false;
			ILayoutControl storedOwnerControl = OwnerControl;
			if(!storedOwnerControl.DesignMode && storedOwnerControl.CustomizationMode != CustomizationModes.Quick) storedOwnerControl.RootGroup.ClearSelection();
			else storedOwnerControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = Bounds;
			storedOwnerControl.EnableCustomizationMode = false;
			storedOwnerControl.Invalidate();
			storedOwnerControl.Control.Focus();
			storedOwnerControl = null;
		}
		protected override string FormCaption {
			get { return LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.CustomizationFormTitle); }
		}
		protected override void OnLoad(EventArgs e) {
			CreateCustomizationForm();
			base.OnLoad(e);
		}
		protected override void InitCustomizationForm() {
		}
		public override Control ControlOwner {
			get { return OwnerControl == null ? null : OwnerControl.Control; }
		}
		protected override UserLookAndFeel ControlOwnerLookAndFeel {
			get { return OwnerControl == null ? null : OwnerControl.LookAndFeel; }
		}
		protected override Rectangle CustomizationFormBounds {
			get { return OwnerControl == null ? Rectangle.Empty : OwnerControl.Bounds; }
		}
		protected override CustomizationListBoxBase CreateCustomizationListBox() { return null; }
		protected override void OnFormClosed() { }
		bool wmSizing = false;
		public bool allowAutoSizeMode = true;
		internal bool fullCustomization = false;
		ResizingState resizingState;
		Point startPosition;
		SizeInfo curentMaxMinSize;
		int marginHeight = 5;
		System.Collections.Generic.List<SizeInfo> sizeInfo;
		CustomizationFormStates flagsCore = CustomizationFormStates.AllowEverything;
		internal CustomizationFormStates Flags {
			get { return flagsCore; }
			set { flagsCore = value; }
		}
		protected override void WndProc(ref Message msg) {
			if(OwnerControl != null  && OwnerControl.CustomizationMode == CustomizationModes.Quick && allowAutoSizeMode && !OwnerControl.DesignMode) {
				switch(msg.Msg) {
					case 0x0214:
						WmSizeChanged(msg);
						base.WndProc(ref msg);
						break;
					case 561:
						SetBeginState();
						startPosition = Cursor.Position;
						wmSizing = true;
						base.WndProc(ref msg);
						break;
					case 562:
						wmSizing = false;
						SetBeginState();
						base.WndProc(ref msg);
						break;
					case 6:
						if((Flags & CustomizationFormStates.AllowActivate) != 0) {
							base.WndProc(ref msg);
						}
						break;
					default:
						base.WndProc(ref msg);
						break;
				}
			} else base.WndProc(ref msg);
		}
		private void SetBeginState() {
			if(sizeInfo == null && Controls[1] != null && Controls[1] is DXFlowLayoutPanel) {
				sizeInfo = Controls[1].Tag as System.Collections.Generic.List<SizeInfo>;
				if(sizeInfo.Count > 0) {
					curentMaxMinSize = sizeInfo[0];
				}
			}
			resizingState = ResizingState.Begin;
		}
		private void setMinMaxSize() {
			if(curentMaxMinSize != null) {
				if(fullCustomization) {
					MaximumSize = new Size(3000, 3000);
					MinimumSize = new Size(0, 500);
				}else MinimumSize = MaximumSize = curentMaxMinSize.AutoSize;
			}
		}
		private void WmSizeChanged(Message m) {
			if(wmSizing) {
				Point curPosition = Cursor.Position;
				int diffHeight = curPosition.Y - startPosition.Y;			   
					if(curentMaxMinSize.Index >= sizeInfo.Count) {
						curentMaxMinSize.Index = sizeInfo.Count - 1;
					}
				if(m.WParam == (IntPtr)7 || m.WParam == (IntPtr)6 || m.WParam == (IntPtr)8) {
					if(diffHeight > 0 && (curentMaxMinSize.Index + 1 != sizeInfo.Count || resizingState == ResizingState.ShowStatePlus)) CheckPlusDiff(diffHeight);
					if(diffHeight < 0 && (curentMaxMinSize.Index != 0 || resizingState == ResizingState.ShowStateMinus)) CheckMinusDiff(diffHeight);
				}
			}
		}
		private void CheckMinusDiff(int diffHeight) {
			CheckBeginStateMinus(diffHeight);
			CheckShowStateMinus(diffHeight);
			CheckAgreeWithSize();
		}
		private void CheckPlusDiff(int diffHeight) {
			CheckBeginStatePlus(diffHeight);
			chechShowStatePlus(diffHeight);
			CheckAgreeWithSize();
		}
		private void CheckAgreeWithSize() {
			if(resizingState == ResizingState.AgreeWithSize) {
				startPosition.Y = Location.Y + Height + 4;
				resizingState = ResizingState.Begin;
			}
		}
		private void CheckShowStateMinus(int diffHeight) {
			if(resizingState == ResizingState.ShowStateMinus) {
				if(Math.Abs(diffHeight) > marginHeight) setMinMaxSize();
				else {
					if(sizeInfo.Count > 0) {
						curentMaxMinSize = sizeInfo[curentMaxMinSize.Index + 1];
					}
					setMinMaxSize();
					resizingState = ResizingState.Begin;
					return;
				}
				if(Math.Abs(diffHeight) >= Math.Abs(curentMaxMinSize.Height - sizeInfo[curentMaxMinSize.Index + 1].Height)) {
					resizingState = ResizingState.AgreeWithSize;
				}
			}
		}
		private void CheckBeginStateMinus(int diffHeight) {
			if(resizingState == ResizingState.Begin || resizingState == ResizingState.ShowStatePlus) {
				if(Math.Abs(diffHeight) > marginHeight) {
					resizingState = ResizingState.ShowStateMinus;
					if(sizeInfo.Count > 0) {
						curentMaxMinSize = sizeInfo[curentMaxMinSize.Index - 1];
					}
				}
			}
		}
		private void chechShowStatePlus(int diffHeight) {
			if(resizingState == ResizingState.ShowStatePlus) {
				if(diffHeight > marginHeight) setMinMaxSize();
				else {
					if(sizeInfo.Count > 0) {
						curentMaxMinSize = sizeInfo[curentMaxMinSize.Index - 1];
					}
					setMinMaxSize();
					resizingState = ResizingState.Begin;
					return;
				}
				if(diffHeight >= curentMaxMinSize.Height - sizeInfo[curentMaxMinSize.Index - 1].Height) {
					resizingState = ResizingState.AgreeWithSize;
				}
			}
		}
		private void CheckBeginStatePlus(int diffHeight) {
			if(resizingState == ResizingState.Begin || resizingState == ResizingState.ShowStateMinus) {
				if(diffHeight > marginHeight) {
					resizingState = ResizingState.ShowStatePlus;
					if(sizeInfo.Count > 0) {						
					   curentMaxMinSize = sizeInfo[curentMaxMinSize.Index + 1];
					}
				}
			}
		}
	}
}
