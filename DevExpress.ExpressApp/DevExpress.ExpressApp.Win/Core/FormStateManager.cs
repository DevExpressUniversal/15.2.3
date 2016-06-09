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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Win.Core {
	[ToolboxItem(false)] 
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	[DesignerCategory("Component")]
	public class FormStateModelSynchronizer : Component, IModelSynchronizable {
		private Form form;
		private IModelFormState model;
		public FormStateModelSynchronizer() { }
		public FormStateModelSynchronizer(IContainer container) {
			container.Add(this);
		}
		public Form Form {
			get { return form; }
			set { form = value; }
		}
		[Browsable(false)]
		[DefaultValue(null)]
		public IModelFormState Model {
			get { return model; }
			set { model = value; }
		}
		#region IModelSynchronizable Members
		public void ApplyModel() {
			if(form != null && model != null) {
				using(FormStateModelSynchronizerInternal synchronizer = new FormStateModelSynchronizerInternal(form, model)) {
					synchronizer.ApplyModel();
				}
			}
		}
		public void SynchronizeModel() {
			if(form != null && model != null) {
				using(FormStateModelSynchronizerInternal synchronizer = new FormStateModelSynchronizerInternal(form, model)) {
					synchronizer.SynchronizeModel();
				}
			}
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing) {
				form = null;
				model = null;
			}
			base.Dispose(disposing);
		}
	}
	[Flags]
	public enum FormStateStoreOptions {
		None = 0,
		Size = 1,
		Location = 2,
		SizeAndLocation = 3,
		State = 4,
		All = 7
	}
	public class FormStateModelSynchronizerInternal : ModelSynchronizer<Form, IModelFormState> {
		public FormStateModelSynchronizerInternal(Form control, IModelFormState model)
			: base(control, model) {
		}
		protected override void ApplyModelCore() {
			new FormStateAndBoundsManager().Load(Control, new SettingsStorageOnModel(Model));
		}
		public override void SynchronizeModel() {
			new FormStateAndBoundsManager().Save(Control, new SettingsStorageOnModel(Model));
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum SizeSavingMode {ClientSize, DesktopBounds}
	public class FormStateAndBoundsManager {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static SizeSavingMode SizeSavingMode = SizeSavingMode.DesktopBounds; 
		private string TrimControlChars(string text) {
			int trimIndex = 0;
			while(trimIndex < text.Length && text[trimIndex] != 0) {
				trimIndex++;
			}
			return text.Substring(0, trimIndex);
		}
		private void SetFormSize(Form form, Size size) {
			if(SizeSavingMode == SizeSavingMode.ClientSize) {
				form.ClientSize = size;
			}
			else {
				form.Size = size;
			}
		}
		public void Save(Form form, SettingsStorage storage) {
			SaveFormState(storage, form.WindowState);
			if(form.WindowState == FormWindowState.Maximized) {
				SaveDisplayDevice(storage, form);
				SaveFormLocation(storage, form.RestoreBounds.Location);
				SaveFormSize(storage, form.RestoreBounds.Size);
			}
			else {
				SaveFormLocation(storage, form.DesktopBounds.Location);
				SaveFormSize(storage, SizeSavingMode == SizeSavingMode.ClientSize ? form.ClientSize : form.DesktopBounds.Size);
			}
		}
		public void SaveFormState(SettingsStorage storage, FormWindowState state) {
			storage.SaveOption("", "State", state.ToString());
		}
		public void SaveDisplayDevice(SettingsStorage storage, Form form) {
			storage.SaveOption("", "MaximizedOnScreen", TrimControlChars(Screen.FromControl(form).DeviceName));
			}
		public void SaveFormLocation(SettingsStorage storage, Point location) {
			storage.SaveOption("", "X", location.X.ToString());
			storage.SaveOption("", "Y", location.Y.ToString());
		}
		public void SaveFormSize(SettingsStorage storage, Size size) {
			storage.SaveOption("", "Width", size.Width.ToString());
			storage.SaveOption("", "Height", size.Height.ToString());
		}
		public void Load(Form form, SettingsStorage storage) {
			Load(form, storage, FormStateStoreOptions.All);
		}
		public void Load(Form form, SettingsStorage storage, FormStateStoreOptions options) {
			string windowStateString = null;
			windowStateString = storage.LoadOption("", "State");
			if(!string.IsNullOrEmpty(windowStateString)) {
				if(!form.AutoScaleBaseSize.IsEmpty && (form.AutoScaleMode == AutoScaleMode.None || form.AutoScaleMode == AutoScaleMode.Inherit)) {
					form.AutoScaleMode = AutoScaleMode.Font;
				}
				FormWindowState state = LoadFormState(storage);
				Point storedFormLocation = LoadFormLocation(storage);
				Size storedFormSize = LoadFormSize(storage, true);
				if(state == FormWindowState.Normal) {
					Rectangle header = new Rectangle(storedFormLocation, new Size(storedFormSize.Width, SystemInformation.CaptionHeight));
					Rectangle visiblePartOfHeader = SystemInformation.VirtualScreen;
					visiblePartOfHeader.Intersect(header);
					Size minWindowHeader = SystemInformation.MinWindowTrackSize;
					FormWindowState stateCore = LoadFormStateCore(storage);
					if(visiblePartOfHeader.Width < minWindowHeader.Width / 2 || visiblePartOfHeader.Height < minWindowHeader.Height / 2 || stateCore == FormWindowState.Minimized) {
						form.StartPosition = FormStartPosition.WindowsDefaultBounds;
					}
					else {
						if(string.IsNullOrEmpty(storage.LoadOption("", "X")) || string.IsNullOrEmpty(storage.LoadOption("", "Y"))) {
							form.StartPosition = FormStartPosition.CenterScreen;
							if((options & FormStateStoreOptions.Size) == FormStateStoreOptions.Size) {
								SetFormSize(form, storedFormSize);
							}
						}
						else {
							form.StartPosition = FormStartPosition.Manual;
							if((options & FormStateStoreOptions.Location) == FormStateStoreOptions.Location) {
								form.DesktopLocation = storedFormLocation;
							}
							if((options & FormStateStoreOptions.Size) == FormStateStoreOptions.Size) {
								SetFormSize(form, storedFormSize);
							}
						}
					}
				}
				else {
					form.StartPosition = FormStartPosition.Manual;
					if((options & FormStateStoreOptions.Location) == FormStateStoreOptions.Location) {
						form.DesktopLocation = LoadDisplayDevice(storage).Bounds.Location;
						form.Bounds = new Rectangle(storedFormLocation, form.Size);
					}
					if((options & FormStateStoreOptions.Size) == FormStateStoreOptions.Size) {
						form.Bounds = new Rectangle(form.Location, storedFormSize);
					}
				}
				if((options & FormStateStoreOptions.State) == FormStateStoreOptions.State) {
					form.WindowState = state;
				}
			}
		}
		public FormWindowState LoadFormState(SettingsStorage storage) {
			FormWindowState state = LoadFormStateCore(storage);
			if(state == FormWindowState.Minimized) {
				state = FormWindowState.Normal;
			}
			return state;
		}
		private FormWindowState LoadFormStateCore(SettingsStorage storage) {
			FormWindowState state = FormWindowState.Normal;
			try {
				state = (FormWindowState)Enum.Parse(typeof(FormWindowState), storage.LoadOption("", "State"));
			}
			catch { }
			return state;
		}
		public Screen LoadDisplayDevice(SettingsStorage storage) {
			string destinationDevice = storage.LoadOption("", "MaximizedOnScreen");
			Screen destinationScreen = Screen.PrimaryScreen;
			foreach (Screen screen in Screen.AllScreens) {
				if (TrimControlChars(screen.DeviceName) == destinationDevice) {
					destinationScreen = screen;
					break;
				}
			}
			return destinationScreen;
		}
		public Point LoadFormLocation(SettingsStorage storage) {
			return new Point(storage.LoadIntOption("", "X", 0), storage.LoadIntOption("", "Y", 0));
		}
		public Size LoadFormSize(SettingsStorage storage, bool allowUseDefaultSize) {
			const int DefaultWidth = 500;
			const int DefaultHeight = 400;
			return new Size(storage.LoadIntOption("", "Width", allowUseDefaultSize ? DefaultWidth : 0),
				storage.LoadIntOption("", "Height", allowUseDefaultSize ? DefaultHeight : 0));
		}
	}
}
