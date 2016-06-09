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
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Xpf.Core;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Mouse;
using DevExpress.Office;
using DevExpress.Xpf.Office.Internal;
using DevExpress.Pdf;
namespace DevExpress.Xpf.RichEdit {
	public partial class RichEditControl : IGestureClient {
		GestureHelper gestureHelper;
		bool IRichEditControl.IsPrintPreviewAvailable { get { return GetService<IRichEditPrintingService>() != null; } }
		public virtual void LoadDocument(string fileName) {
			if (InnerControl != null)
				InnerControl.LoadDocument(fileName, DocumentFormat.Undefined);
		}
		public virtual void LoadDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.LoadDocument(fileName, documentFormat);
		}
		public virtual void SaveDocument(string fileName, DocumentFormat documentFormat) {
			if (InnerControl != null)
				InnerControl.SaveDocument(fileName, documentFormat);
		}
		public virtual void LoadDocument(IWin32Window parent) {
			if (InnerControl != null)
				InnerControl.LoadDocument(parent);
		}
		public virtual bool SaveDocument() {
			if (InnerControl != null)
				return InnerControl.SaveDocument();
			else
				return false;
		}
		public virtual bool SaveDocument(IWin32Window parent) {
			if (InnerControl != null)
				return InnerControl.SaveDocument(parent);
			else
				return false;
		}
		public void ExportToPdf(string fileName) {
			ExportToPdf(fileName, new PdfCreationOptions() { Compatibility = PdfCompatibility.Pdf }, new PdfSaveOptions());
		}
		public void ExportToPdf(Stream stream) {
			ExportToPdf(stream, new PdfCreationOptions() { Compatibility = PdfCompatibility.Pdf }, new PdfSaveOptions());
		}
		public void ExportToPdf(string fileName, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
				ExportToPdf(stream, creationOptions, saveOptions);
			}
		}
		public void ExportToPdf(Stream stream, PdfCreationOptions creationOptions, PdfSaveOptions saveOptions) {
			if (InnerControl != null)
				InnerControl.ExportToPdf(stream, creationOptions, saveOptions);
		}
		protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
			if (e.Handled) {
				if (InnerControl != null && InnerControl.MouseHandler != null)
					InnerControl.MouseHandler.SwitchToDefaultState();
			}
		}
		#region IGestureClient
		void IGestureClient.OnManipulationStarting(ManipulationStartingEventArgs e) {
			if (!Options.Behavior.TouchAllowed)
				return;
			this.overPanX = 0;
			this.overPanY = 0;
			e.Mode = ManipulationModes.TranslateY;
			if (Options != null && Options.Behavior.ZoomingAllowed)
				e.Mode |= ManipulationModes.Scale;
			if (ActiveView != null && ActiveView.HorizontalScrollController.IsScrollPossible())
				e.Mode |= ManipulationModes.TranslateX;
		}
		int overPanX;
		int overPanY;
		int IGestureClient.OnVerticalPan(ManipulationDeltaEventArgs e) {
			if (!Options.Behavior.TouchAllowed)
				return 0;
			int deltaY = (int)Math.Round(e.DeltaManipulation.Translation.Y);
			ScrollVerticallyByPhysicalOffsetCommand command = new ScrollVerticallyByPhysicalOffsetCommand(this);
			command.PhysicalOffset = -this.ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(deltaY, this.DpiY);
			command.Execute();
			if (!command.ExecutedSuccessfully)
				this.overPanY += deltaY;
			return overPanY;
		}
		int IGestureClient.OnHorizontalPan(ManipulationDeltaEventArgs e) {
			if (!Options.Behavior.TouchAllowed)
				return 0;
			int deltaX = (int)Math.Round(e.DeltaManipulation.Translation.X);
			ScrollHorizontallyByPhysicalOffsetCommand command = new ScrollHorizontallyByPhysicalOffsetCommand(this);
			command.PhysicalOffset = -this.ActiveView.DocumentLayout.UnitConverter.PixelsToLayoutUnits(deltaX, this.DpiX);
			command.Execute();
			if (!command.ExecutedSuccessfully)
				overPanX += deltaX;
			return overPanX;
		}
		void IGestureClient.OnZoom(ManipulationDeltaEventArgs e, double deltaZoom) {
			if (!Options.Behavior.TouchAllowed)
				return;
			ActiveView.ZoomFactor *= (float)deltaZoom;
		}
		#endregion
		#region IRichEditControl
		bool IRichEditControl.UseStandardDragDropMode { get { return false; } }
		RichEditMouseHandlerStrategyFactory IRichEditControl.CreateRichEditMouseHandlerStrategyFactory() {
			return new XpfRichEditMouseHandlerStrategyFactory();
		}
		#endregion
		void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {
			if (e.TargetObject == this.KeyCodeConverter)
				e.Handled = true;
		}
		void AddBehaviors() {
			var behaviors = DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(this);
			behaviors.Add(new HorizontalScrollingBehavior());
		}
		void RemoveBehaviors() {
			var behaviors = DevExpress.Mvvm.UI.Interactivity.Interaction.GetBehaviors(this);
			behaviors.Clear();
		}
	}
	internal class HorizontalScrollingBehavior : DevExpress.Mvvm.UI.Interactivity.Behavior<RichEditControl> {
		System.Windows.Interop.HwndSource source;
		protected override void OnAttached() {
			base.OnAttached();
			this.source = PresentationSource.FromVisual(AssociatedObject) as System.Windows.Interop.HwndSource;
			if (this.source != null)
				source.AddHook(Filter);
		}
		protected override void OnDetaching() {
			if (this.source != null)
				this.source.RemoveHook(Filter);
			this.source = null;
			base.OnDetaching();
		}
		IntPtr Filter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			const int WM_MOUSEHWHEEL = 0x20E;
			if (msg == WM_MOUSEHWHEEL) {
				int wPar = wParam.ToInt32();
				int delta = Math.Sign(wPar) * (Math.Abs(wPar) >> 16);
				if (AssociatedObject != null)
					handled = AssociatedObject.OnMouseWheelCore(new OfficeMouseEventArgs(AssociatedObject.ObtainPressedMouseButtons(), 0, 0, 0, delta, true));
			}
			return IntPtr.Zero;
		}
	}
}
