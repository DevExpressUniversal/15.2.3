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
using System.ComponentModel.Design;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design 
{
	using Commands;
	using DevExpress.Utils.Internal;
	public abstract class FontServiceBase : IDisposable 
	{
		MenuCommandHandler menuCommandHandler;
		IMenuCommandServiceEx menuCommandService;
		protected IComponentChangeService changeServ;
		protected ISelectionService selectionServ;
		protected IServiceProvider serviceProvider;
		bool subscribed;
		protected FontServiceBase() {
		}
		protected FontServiceBase(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
			SubscribeEvents();
			menuCommandService = serviceProvider.GetService(typeof(IMenuCommandService)) as IMenuCommandServiceEx;
			System.Diagnostics.Debug.Assert(menuCommandService != null, "");
		}
		public virtual void Dispose() {
			UnsubscribeEvents();
		}
		private void SubscribeEvents() {
			if(subscribed) return;
			subscribed = true;
			if(serviceProvider == null) return;
			menuCommandHandler = serviceProvider.GetService(typeof(MenuCommandHandler)) as MenuCommandHandler;
			menuCommandHandler.CommandStatusChanged += new MenuCommandEventHandler(OnCommandStatusChanged);
			selectionServ = serviceProvider.GetService(typeof(ISelectionService)) as ISelectionService;
			System.Diagnostics.Debug.Assert(selectionServ != null, "");
			if(selectionServ != null)
				selectionServ.SelectionChanged += new EventHandler(OnSelectionChanged);
			changeServ = serviceProvider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			System.Diagnostics.Debug.Assert(changeServ != null, "");
			if(changeServ != null) 
				changeServ.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
		}
		private void UnsubscribeEvents() {
			if(!subscribed) return;
			subscribed = false;
			System.Diagnostics.Debug.Assert(changeServ != null, "");
			if(changeServ != null)
				changeServ.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
			System.Diagnostics.Debug.Assert(selectionServ != null, "");
			if(selectionServ != null)
				selectionServ.SelectionChanged -= new EventHandler(OnSelectionChanged);
			System.Diagnostics.Debug.Assert(menuCommandHandler != null, "");
			if(menuCommandHandler != null)
				menuCommandHandler.CommandStatusChanged -= new MenuCommandEventHandler(OnCommandStatusChanged);
		}
		private void OnCommandStatusChanged(object sender, MenuCommandEventArgs e) {
			MenuCommand menuCommand = e.MenuCommand;
			if(menuCommand.CommandID.Equals(FormattingCommands.FontName) ||
				menuCommand.CommandID.Equals(FormattingCommands.FontSize) )
				SetFontControlsVisibility(GetCommandEnabled(menuCommand));
		}
		protected static bool GetCommandEnabled(MenuCommand menuCommand) {
			return menuCommand.Enabled && menuCommand.Supported;
		}
		private void OnSelectionChanged(object sender, EventArgs e) {
			OnCurrentFontChanged();
		}
		private void OnCurrentFontChanged() {
			if(selectionServ != null) {
				XRControl control = selectionServ.PrimarySelection as XRControl; 
				if(control != null && !control.IsDisposed && control is XRRichTextBase) {
					IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
					XRRichTextBaseDesigner designer = host.GetDesigner(control) as XRRichTextBaseDesigner;
					if(designer != null && designer.IsInplaceEditingMode) {
						UpdateFontControls(designer.Editor.SelectionFont);
						return;
					}
				}
				Font font = (control != null && !control.IsDisposed) ? control.GetEffectiveFont() : null;
				UpdateFontControls(FontSurrogate.FromFont(font));
			}
		}
		private void OnComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(e.Component is XRControl && e.Member != null && e.Member.Name.Equals(XRComponentDesigner.GetStylePropertyName(XRComponentPropertyNames.Font)))
				UpdateFontControls(((XRControl)e.Component).Font);
		}
		protected void ExecuteFontNameCommand(string item) {
			ExecuteFontCommand(FormattingCommands.FontName, item);
		}
		protected void ExecuteFontSizeCommand(string item) {
			ExecuteFontCommand(FormattingCommands.FontSize, item);
		}
		void ExecuteFontCommand(CommandID commandID, string item) {
			if(menuCommandService != null && RootDesignerIsActive())
				menuCommandService.GlobalInvoke(commandID, new object[] { item });
			OnCurrentFontChanged();
		}
		private bool RootDesignerIsActive() {
			IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if(host != null) {
				ReportDesigner designer = (ReportDesigner)host.GetDesigner(host.RootComponent);
				if(designer != null) return designer.IsActive;
			}
			return false;
		}
		protected virtual void SetFontControlsVisibility(bool enabled) {
		}
		protected virtual void ResetFontControls() {
		}
		public void UpdateFontControls(Font font) {
			UpdateFontControls(FontSurrogate.FromFont(font));
		}
		public virtual void UpdateFontControls(FontSurrogate font) {
		}
		public virtual void Activate() {
			SubscribeEvents();
		}
		public virtual void Deactivate() {
			UnsubscribeEvents();
		}
		public virtual void Close() {
		}
		public static FontStyle GetFirstAvailableFontStyle(FontFamily fontFamily) {
			return XtraEditors.FontServiceBase.GetFirstAvailableFontStyle(fontFamily);
		}
		public static FontStyle GetFirstAvailableFontStyle(string fontName) {
			return GetFirstAvailableFontStyle(new FontFamily(fontName));
		}
		public static bool IsFontStyleSupported(string fontName, FontStyle fontStyle) {
			return new FontFamily(fontName).IsStyleAvailable(fontStyle);
		}
		public static FontStyle GetAvailableFontStyle(FontFamily fontFamily, FontStyle fontStyle) {
			if( !fontFamily.IsStyleAvailable(fontStyle) ) {
				fontStyle |= FontServiceBase.GetFirstAvailableFontStyle(fontFamily);
			}
			return fontStyle;
		}
		public static FontStyle GetAvailableFontStyle(string fontName, FontStyle fontStyle) {
			return GetAvailableFontStyle(new FontFamily(fontName), fontStyle);
		}
	}
}
