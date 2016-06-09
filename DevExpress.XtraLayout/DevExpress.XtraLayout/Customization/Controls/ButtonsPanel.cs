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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Localization;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraLayout.Customization.Controls {
	public class WrongParentTypeMessagePainter {
		[ThreadStatic]
		static WrongParentTypeMessagePainter defInstance;
		public static WrongParentTypeMessagePainter Default {
			get {
				if (defInstance == null) defInstance = new WrongParentTypeMessagePainter();
				return defInstance;
			}
		}
		public WrongParentTypeMessagePainter() {
			app = new AppearanceObject();
			app.TextOptions.WordWrap = WordWrap.Wrap;
			app.BackColor = Color.Red;
		}
		AppearanceObject app = new AppearanceObject();
		public void Draw(Rectangle bounds, PaintEventArgs e) {
			using (GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				cache.FillRectangle(Brushes.Gray, bounds);
				Rectangle textRect = bounds;
				textRect.Inflate(-5, -5);
				app.DrawString(cache, "Invalid parent type. Parent must be derived from UserCustomizationForm class.", textRect);
			}
		}
	}
	public class OwnerControlHelper {
		public static ILayoutControl GetOwnerControl(Control parent) {
			if(parent == null) return null;
			ICustomizationContainer container = parent as ICustomizationContainer;
			if(container != null) return container.OwnerControl;
			else return GetOwnerControl(parent.Parent);
		}
	}
	[DesignTimeVisible(true), ToolboxItem(false)]
	[Designer(LayoutControlConstants.ButtonsPanelDesignerName, typeof(System.ComponentModel.Design.IDesigner)), ToolboxBitmap(typeof(LayoutControl), "Images.buttons.bmp")]
	public class ButtonsPanel : XtraUserControl, ICustomizationFormControl {
		private static readonly object sizeableChanged;
		protected internal SimpleButton loadLayoutButtonCore, saveLayoutButtonCore, undoButtonCore, redoButtonCore;
		protected BaseCustomizationFormToolTipManager hintManager;
		internal LayoutControl layoutManagerCore;
		protected LayoutControlGroup commandBarGroup;
		static ButtonsPanel() {
			sizeableChanged = new object();
		}
		#region ICustomizationFormControl
		public void Register() { Init(); }
		public void UnRegister() {
			UnSubscribeCommandButtonEvents();
			UnsubscribeUndoStatusChangedEvent();
		}
		ILayoutControl ownerControlCore;
		public ILayoutControl OwnerControl { 
			get {
				if(ownerControlCore == null)
					ownerControlCore = GetOwnerControl();
				return ownerControlCore; 
			} 
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(ownerControlCore == null)
				WrongParentTypeMessagePainter.Default.Draw(new Rectangle(0, 0, Width, Height), e);
			else base.OnPaint(e);
		}
		protected virtual ILayoutControl GetOwnerControl() {
			return OwnerControlHelper.GetOwnerControl(Parent);
		}
		public UserLookAndFeel ControlOwnerLookAndFeel { 
			get { return OwnerControl != null ? OwnerControl.LookAndFeel : null; } 
		}
		#endregion
		public void BeginInit() { }
		public void EndInit() { }
		protected virtual void Init() {
			if(OwnerControl == null) return;
			CreateLayoutManager();
			layoutManagerCore.BeginInit();
			InitLayoutManager();
			if(OwnerControl.DesignMode && OwnerControl.OptionsCustomizationForm.DesignerExpertMode || (!OwnerControl.DesignMode)) {
				CreateCommandBar(layoutManagerCore.Root);
				if(OwnerControl.DesignMode) UpdateUndoRedoButtonsState();
			}
			SubscribeUndoStatusChangedEvent();
			layoutManagerCore.EndInit();
			UpdateUndoRedoButtonsState();
		}
		bool isDisposing = false;
		protected bool IsDisposingInProgress { get { return isDisposing; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				isDisposing = true;
				UnRegister();
				DisposeLayoutManager();
			}
			base.Dispose(disposing);
		}
		protected void DisposeLayoutManager() {
			if(layoutManagerCore != null) {
				layoutManagerCore.Dispose();
				layoutManagerCore = null;
			}
		}
		protected void InitLayoutManager() {
			layoutManagerCore.Parent = this;
			((ILayoutControl)layoutManagerCore).EnableCustomizationForm = false;
			layoutManagerCore.OptionsView.AutoSizeInLayoutControl = AutoSizeModes.UseMinAndMaxSize;
			layoutManagerCore.SetControlDefaultsLast();
			layoutManagerCore.Root.GroupBordersVisible = false;
			layoutManagerCore.Root.EnableIndentsWithoutBorders =  DefaultBoolean.False;
			if(ControlOwnerLookAndFeel != null)
				layoutManagerCore.LookAndFeel.Assign(ControlOwnerLookAndFeel);
			layoutManagerCore.Dock = DockStyle.Fill;
		}
		protected void CreateLayoutManager() {
			layoutManagerCore = new LayoutControl(LayoutControlRoles.CustomizationFormControl);
		}
		protected virtual SimpleButton CreateSimpleButton(Size buttonMaximumSize) {
			SimpleButton button = new SimpleButton();
			button.MaximumSize = buttonMaximumSize;
			button.ImageLocation = ImageLocation.MiddleCenter;
			return button;
		}
		protected virtual void CreateCommandBarButtons(Size buttonMaximumSize) {
			loadLayoutButtonCore = CreateSimpleButton(buttonMaximumSize);
			loadLayoutButtonCore.Image = LayoutControlImageStorage.Default.CustomizationFormButton.Images[0];
			saveLayoutButtonCore = CreateSimpleButton(buttonMaximumSize);
			saveLayoutButtonCore.Image = LayoutControlImageStorage.Default.CustomizationFormButton.Images[1];
			undoButtonCore = CreateSimpleButton(buttonMaximumSize);
			undoButtonCore.Image = LayoutControlImageStorage.Default.CustomizationFormButton.Images[2];
			redoButtonCore = CreateSimpleButton(buttonMaximumSize);
			redoButtonCore.Image = LayoutControlImageStorage.Default.CustomizationFormButton.Images[3];
			hintManager = new BaseCustomizationFormToolTipManager();
			hintManager.AssignHintToUndoControl(undoButtonCore);
			hintManager.AssignHintToRedoControl(redoButtonCore);
			hintManager.AssignHintToSaveControl(saveLayoutButtonCore);
			hintManager.AssignHintToLoadControl(loadLayoutButtonCore);
			SubscribeCommandButtonEvents();
		}
		public SimpleButton LoadLayoutButton { get { return loadLayoutButtonCore; } }
		public SimpleButton SaveLayoutButton { get { return saveLayoutButtonCore; } }
		public SimpleButton UndoButton { get { return undoButtonCore; } }
		public SimpleButton RedoButton { get { return redoButtonCore; } }
		protected virtual void SubscribeCommandButtonEvents() {
			if (Site != null) return;
			loadLayoutButtonCore.Click += new EventHandler(OnLoadLayoutButtonClick);
			saveLayoutButtonCore.Click += new EventHandler(OnSaveLayoutButtonClick);
			undoButtonCore.Click += new EventHandler(OnUndoButtonClick);
			redoButtonCore.Click += new EventHandler(OnRedoButtonClick);
		}
		protected virtual void UnSubscribeCommandButtonEvents() {
			if(loadLayoutButtonCore != null) loadLayoutButtonCore.Click -= new EventHandler(OnLoadLayoutButtonClick);
			if(saveLayoutButtonCore != null) saveLayoutButtonCore.Click -= new EventHandler(OnSaveLayoutButtonClick);
			if(undoButtonCore != null) undoButtonCore.Click -= new EventHandler(OnUndoButtonClick);
			if(redoButtonCore != null) redoButtonCore.Click -= new EventHandler(OnRedoButtonClick);
		}
		protected virtual void OnLoadLayoutButtonClick(object sender, EventArgs e) {
			if (OwnerControl != null) {
				if (CheckOwnerControlDesignMode()) {
					DialogResult warningRes = XtraMessageBox.Show(this, "Do you want to load a layout from an XML file and override the current layout?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
					if (warningRes != DialogResult.Yes) return;
				}
				OpenFileDialog dialog = CreateOpenFileDialog();
				IUndoEngine u = UndoEngineHelper.GetUndoEngine(OwnerControl.Control);
				DialogResult result = DialogResult.Cancel;
				try {
					OwnerControl.FireChanging(OwnerControl.Control);
					result = dialog.ShowDialog(this);
					if (result == DialogResult.OK) {
						if (u != null) u.Enabled = false;
#if DEBUG
						if (Control.ModifierKeys == (Keys.Control | Keys.Alt))
							(OwnerControl as LayoutControl).DiagnosticRestoreLayoutFromXml(dialog.FileName);
						else
#endif
							OwnerControl.RestoreLayoutFromXml(dialog.FileName);
						if (OwnerControl.UndoManager != null) OwnerControl.UndoManager.Reset();
					}
				} catch {
					XtraMessageBox.Show("file not found");
				} finally {
					if (result == DialogResult.OK) {
						DesignTimeCheckNames();
						OwnerControl.FireChanged(OwnerControl.Control);
						if (u != null) u.Enabled = true;
					}
				}
			}
		}
		protected bool CheckFast() {
			bool result = false;
			Hashtable ht = new Hashtable();
			foreach (IComponent component in OwnerControl.Site.Container.Components) {
				if (!ht.ContainsKey(component.Site.Name))
					ht.Add(component.Site.Name, component);
				else { result = true; break; }
			}
			ht.Clear();
			return result;
		}
		protected void DesignTimeCheckNames() {
			if (!((ILayoutControl)OwnerControl).DesignMode) return;
			Container container = OwnerControl.Site.Container as Container;
			if (container == null) return;
			if (CheckFast()) {
				foreach (IComponent component in OwnerControl.Site.Container.Components) {
					String originalName = component.Site.Name;
					component.Site.Name = component.Site.Name + "_test";
					component.Site.Name = originalName;
				}
			}
		}
		protected virtual void OnSaveLayoutButtonClick(object sender, EventArgs e) {
			SaveFileDialog dialog = CreateSaveFileDialog();
			if(OwnerControl != null) {
				DialogResult result = dialog.ShowDialog(this);
				if(result == DialogResult.OK)
					try {
						OwnerControl.SaveLayoutToXml(dialog.FileName);
					} catch (Exception ex) {
						DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message, ex.Source);
					}
			}
		}
		protected virtual OpenFileDialog CreateOpenFileDialog() {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			if(OwnerControl.OptionsCustomizationForm.DefaultRestoreDirectory != string.Empty) dialog.InitialDirectory = OwnerControl.OptionsCustomizationForm.DefaultRestoreDirectory;
			return dialog;
		}
		protected virtual SaveFileDialog CreateSaveFileDialog() {
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.DefaultExt = "xml";
			dialog.Filter = "XML files (*.xml)|*.xml";
			if(OwnerControl.OptionsCustomizationForm.DefaultSaveDirectory != string.Empty) dialog.InitialDirectory = OwnerControl.OptionsCustomizationForm.DefaultSaveDirectory;
			return dialog;
		}
		protected virtual void OnUndoButtonClick(object sender, EventArgs e) {
			if(OwnerControl != null) {
				if(!CheckOwnerControlDesignMode()) {
					OwnerControl.UndoManager.Undo();
					UpdateUndoRedoButtonsState();
				}
			}
		}
		protected virtual void OnRedoButtonClick(object sender, EventArgs e) {
			if(OwnerControl != null) {
				if(!CheckOwnerControlDesignMode()) {
					OwnerControl.UndoManager.Redo();
					UpdateUndoRedoButtonsState();
				}
			}
		}
		protected virtual internal void UpdateUndoRedoButtonsState() {
			if(undoButtonCore != null) {
				if(OwnerControl.UndoManager != null && OwnerControl.UndoManager.IsUndoAllowed)
					undoButtonCore.Enabled = true;
				else undoButtonCore.Enabled = false;
			}
			if(redoButtonCore != null) {
				if(OwnerControl.UndoManager != null && OwnerControl.UndoManager.IsRedoAllowed)
					redoButtonCore.Enabled = true;
				else redoButtonCore.Enabled = false;
			}
		}
		protected virtual LayoutControlGroup CreateCommandBarGroup(LayoutControlGroup group) {
			return group.AddGroup();
		}
		protected virtual LayoutControlItem CreateCommandBarCore(Size barButtonSize, SimpleButton control, bool isVisible) {
			LayoutControlItem itemForButton = AddItemSetDefaults(barButtonSize, commandBarGroup);
			itemForButton.Control = control;
			itemForButton.TextVisible = false;
			itemForButton.Visibility = LayoutVisibilityConvertor.FromBoolean(isVisible);
			control.AllowFocus = false;
			return itemForButton;
		}
		protected virtual void CreateCommandBar(LayoutControlGroup group) {
			Size sizeToButton = WindowsFormsSettings.TouchUIMode == TouchUIMode.True ? new Size(44, 44) : new Size(22, 22);
			CreateCommandBarButtons(sizeToButton);
			Size barButtonSize = new Size(sizeToButton.Width + 3, sizeToButton.Height + 3);
			commandBarGroup = CreateCommandBarGroup(group);
			commandBarGroup.GroupBordersVisible = false;
			commandBarGroup.DefaultLayoutType = LayoutType.Horizontal;
			CreateCommandBarCore(barButtonSize, loadLayoutButtonCore, OwnerControl.OptionsCustomizationForm.ShowLoadButton);
			CreateCommandBarCore(barButtonSize, saveLayoutButtonCore, OwnerControl.OptionsCustomizationForm.ShowSaveButton);
			CreateCommandBarCore(barButtonSize, undoButtonCore, OwnerControl.OptionsCustomizationForm.ShowUndoButton);
			CreateCommandBarCore(barButtonSize, redoButtonCore, OwnerControl.OptionsCustomizationForm.ShowRedoButton);
		}
		protected virtual LayoutControlItem AddItemSetDefaults(Size barButtonSize, LayoutControlGroup commandBarGroup) {
			LayoutControlItem layoutItem = commandBarGroup.AddItem();
			layoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
			layoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(1);
			layoutItem.SizeConstraintsType = SizeConstraintsType.Custom;
			layoutItem.MinSize = layoutItem.MaxSize = barButtonSize;
			return layoutItem;
		}
		protected virtual void ProcessCustomizationHotKey(Keys key) {
			if(ModifierKeys == Keys.Control) {
				switch(key) {
					case Keys.Z:
						OnUndoButtonClick(this, null);
						break;
					case Keys.Y:
						OnRedoButtonClick(this, null);
						break;
					case Keys.S:
						OnSaveLayoutButtonClick(this, null);
						break;
					case Keys.O:
						OnLoadLayoutButtonClick(this, null);
						break;
				}
			}
		}
		protected override bool ProcessKeyPreview(ref Message m) {
			if(m.Msg == 0x100) {
				ProcessCustomizationHotKey((Keys)m.WParam);
			}
			return base.ProcessKeyPreview(ref m);
		}
		protected virtual void OnUndoStackChanged(object sender, EventArgs e) {
			UpdateUndoRedoButtonsState();
		}
		bool subscribeUndoStatusChangedEvent = false;
		protected void SubscribeUndoStatusChangedEvent() {
			if(!CheckOwnerControlDesignMode()) {
				subscribeUndoStatusChangedEvent = true;
				OwnerControl.UndoManager.UndoStackChanged += OnUndoStackChanged;
			}
		}
		protected void UnsubscribeUndoStatusChangedEvent() {
			if(OwnerControl == null) return;
			if(!CheckOwnerControlDesignMode() && subscribeUndoStatusChangedEvent) 
				OwnerControl.UndoManager.UndoStackChanged -= OnUndoStackChanged;
		}
		protected override void OnParentChanged(EventArgs e) {
			if(Parent == null)
				UnRegister();
			base.OnParentChanged(e);
			ownerControlCore = null;
		}
		protected virtual bool CheckOwnerControlDesignMode() { return OwnerControl.DesignMode; }
	}
	public class BaseCustomizationFormToolTipManager : IDisposable {
		ToolTip hint;
		string undoCaption = string.Empty;
		string redoCaption = string.Empty;
		string saveCaption = string.Empty;
		string loadCaption = string.Empty;
		string undoHint = string.Empty;
		string redoHint = string.Empty;
		string saveHint = string.Empty;
		string loadHint = string.Empty;
		Control undoButton = null;
		Control redoButton = null;
		Control saveButton = null;
		Control loadButton = null;
		public BaseCustomizationFormToolTipManager() {
			hint = new ToolTip();
			hint.ToolTipIcon = ToolTipIcon.Info;
			UndoCaptionStringLoad();
			RedoCaptionStringLoad();
			SaveCaptionStringLoad();
			LoadCaptionStringLoad();
			UndoHintStringLoad();
			RedoHintStringLoad();
			SaveHintStringLoad();
			LoadHintStringLoad();
			hint.Popup += new PopupEventHandler(OnPopupToolTip);
		}
		public ToolTip Owner {
			get { return hint; }
		}
		public void AssignHintToUndoControl(Control control) {
			if(control != null) {
				undoButton = control;
				hint.SetToolTip(undoButton, undoHint);
			}
		}
		public void AssignHintToRedoControl(Control control) {
			if(control != null) {
				redoButton = control;
				hint.SetToolTip(redoButton, redoHint);
			}
		}
		public void AssignHintToSaveControl(Control control) {
			if(control != null) {
				saveButton = control;
				hint.SetToolTip(saveButton, saveHint);
			}
		}
		public void AssignHintToLoadControl(Control control) {
			if(control != null) {
				loadButton = control;
				hint.SetToolTip(loadButton, loadHint);
			}
		}
		protected virtual void UndoCaptionStringLoad() {
			undoCaption = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.UndoHintCaption);
		}
		protected virtual void RedoCaptionStringLoad() {
			redoCaption = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.RedoHintCaption);
		}
		protected virtual void SaveCaptionStringLoad() {
			saveCaption = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SaveHintCaption);
		}
		protected virtual void LoadCaptionStringLoad() {
			loadCaption = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LoadHintCaption);
		}
		protected virtual void UndoHintStringLoad() {
			undoHint = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.UndoButtonHintText);
		}
		protected virtual void RedoHintStringLoad() {
			redoHint = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.RedoButtonHintText);
		}
		protected virtual void SaveHintStringLoad() {
			saveHint = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.SaveButtonHintText);
		}
		protected virtual void LoadHintStringLoad() {
			loadHint = LayoutLocalizer.Active.GetLocalizedString(LayoutStringId.LoadButtonHintText);
		}
		void OnPopupToolTip(object sender, PopupEventArgs ea) {
			if(ea.AssociatedControl == undoButton) {
				hint.ToolTipTitle = undoCaption;
			} else if (ea.AssociatedControl == redoButton) {
				hint.ToolTipTitle = redoCaption;
			} else if (ea.AssociatedControl == saveButton) {
				hint.ToolTipTitle = saveCaption;
			} else if (ea.AssociatedControl == loadButton) {
				hint.ToolTipTitle = loadCaption;
			}
		}
		public void Dispose() {
			hint.Popup -= new PopupEventHandler(OnPopupToolTip);
		}
	}
}
