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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using Microsoft.VisualStudio.CommandBars;
using DevExpress.Utils.Design;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraReports.Design.Commands {
	public interface IControllerCallbacks {
		void FontNameChanged(string text);
		void FontSizeChanged(string text);
		void ZoomIn();
		void ZoomOut();
		void ZoomChanged(string text);
	}
	public interface ICommandBarController {
		void Activate();
		void ToggleCommandBarVisibility();
		void Deactivate();
		void Close();
		void Dispose();
		void SetZoomFactorsText(string text);
		void OnDesignerActivated();
		void OnDesignerDeactivated();
		void UpdateFontControls(string fontName, string size);
		void SetFontControlsVisibility(bool enabled);
		void ResetFontControls();
	}
	public class NullCommandBarController : ICommandBarController {
		public NullCommandBarController() {
		}
		public void Activate() {
		}
		public void ToggleCommandBarVisibility() {
		}
		public void Deactivate() {
		}
		public void Close() {
		}
		public void Dispose() {
		}
		public void SetZoomFactorsText(string text) {
		}
		public void OnDesignerActivated() {
		}
		public void OnDesignerDeactivated() {
		}
		public void UpdateFontControls(string fontName, string size) {
		}
		public void SetFontControlsVisibility(bool enabled) {
		}
		public void ResetFontControls() {
		}
	}
	public class CommandBarFontService : FontServiceBase, IControllerCallbacks {
		ICommandBarController commandBarController;
		ZoomEditController editController;
		public CommandBarFontService(IServiceProvider serviceProvider)
			: base(serviceProvider) {
			commandBarController = CreateCommandBarController(serviceProvider);
			editController = new ZoomEditController(ZoomService.MinZoomInPercents, ZoomService.MaxZoomInPercents, ZoomService.ZoomStringFormat);
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if(host != null) {
				host.Activated += new EventHandler(OnDesignerActivated);
				host.Deactivated += new EventHandler(OnDesignerDeactivated);
			}
		}
		ICommandBarController CreateCommandBarController(IServiceProvider serviceProvider) {
			if(VSPackageCommandBarController.IsPackagePresent(serviceProvider))
				return new VSPackageCommandBarController(serviceProvider, this);
			return new NullCommandBarController();
		}
		public override void Dispose() {
			if(commandBarController != null) {
				commandBarController.Dispose();
				commandBarController = null;
			}
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if(host != null) {
				host.Activated -= new EventHandler(OnDesignerActivated);
				host.Deactivated -= new EventHandler(OnDesignerDeactivated);
			}
			ZoomService.GetInstance(serviceProvider).ZoomChanged -= new EventHandler(OnZoomChanged);
			base.Dispose();
		}
		void OnDesignerActivated(object sender, EventArgs e) {
			XRControl control = selectionServ.PrimarySelection as XRControl;
			if(control != null) {
				IMenuCommandService menuService = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandService;
				SetFontControlsVisibility(GetCommandEnabled(menuService.FindCommand(FormattingCommands.FontName)));
				UpdateFontControls(control.Font);
			} else
				SetFontControlsVisibility(false);
			commandBarController.OnDesignerActivated();
			ZoomService.GetInstance(serviceProvider).ZoomChanged += new EventHandler(OnZoomChanged);
			SetZoomFactorsText();
		}
		void OnDesignerDeactivated(object sender, EventArgs e) {
			commandBarController.OnDesignerDeactivated();
			ZoomService.GetInstance(serviceProvider).ZoomChanged -= new EventHandler(OnZoomChanged);
		}
		void OnZoomChanged(object sender, EventArgs e) {
			SetZoomFactorsText();
		}
		void SetZoomFactorsText() {
			commandBarController.SetZoomFactorsText(ZoomService.GetInstance(serviceProvider).ZoomFactorInPercents + "%");
		}
		protected override void SetFontControlsVisibility(bool enabled) {
			if(!enabled) commandBarController.ResetFontControls();
			commandBarController.SetFontControlsVisibility(enabled);
		}
		protected override void ResetFontControls() {
			commandBarController.ResetFontControls();
		}
		public override void UpdateFontControls(FontSurrogate font) {
			if(font != null)
				commandBarController.UpdateFontControls(font.Name, font.Size.ToString());
			else
				commandBarController.ResetFontControls();
		}
		public override void Activate() {
			base.Activate();
			commandBarController.Activate();
		}
		public void ToggleCommandBarVisibility() {
			commandBarController.ToggleCommandBarVisibility();
		}
		public override void Deactivate() {
			base.Deactivate();
			commandBarController.Deactivate();
		}
		public override void Close() {
			commandBarController.Close();
			Dispose();
		}
		void IControllerCallbacks.FontNameChanged(string text) {
			ExecuteFontNameCommand(text);
		}
		void IControllerCallbacks.FontSizeChanged(string text) {
			ExecuteFontSizeCommand(text);
		}
		void IControllerCallbacks.ZoomIn() {
			InvokeCommand(ZoomCommands.ZoomIn, null);
		}
		void IControllerCallbacks.ZoomOut() {
			InvokeCommand(ZoomCommands.ZoomOut, null);
		}
		void IControllerCallbacks.ZoomChanged(string text) {
			string message = string.Empty;
			if(editController.IsValidZoomValue(text, ref message)) {
				InvokeCommand(ZoomCommands.Zoom, editController.GetZoomValue(text));
				SetZoomFactorsText();
				return;
			}
			XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(serviceProvider), message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			SetZoomFactorsText();
		}
		void InvokeCommand(CommandID commandID, object argument) {
			IMenuCommandService menuCommadService = (IMenuCommandService)serviceProvider.GetService(typeof(IMenuCommandService));
			if(menuCommadService == null) return;
			MenuCommand menuCommand = menuCommadService.FindCommand(commandID);
			MenuCommandHandler.InvokeCommandEx(menuCommand, new object[] { argument });
		}
	}
}
