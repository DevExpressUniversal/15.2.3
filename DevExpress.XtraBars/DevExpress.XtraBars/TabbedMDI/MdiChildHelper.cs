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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.Utils.Mdi {
	public static class MdiChildHelper {
		public static void PatchBeforeRegister(Form chidForm) {
			if(chidForm == null) return;
			chidForm.MinimizeBox = false;
			chidForm.MaximizeBox = false;
			chidForm.FormBorderStyle = FormBorderStyle.None;
			chidForm.WindowState = FormWindowState.Normal;
			chidForm.MinimumSize = Size.Empty;
			chidForm.MaximumSize = Size.Empty;
			if(chidForm.IsHandleCreated)
				chidForm.Dock = DockStyle.Fill;
			else {
				chidForm.StartPosition = FormStartPosition.Manual;
				chidForm.Location = new Point(-chidForm.Width, -chidForm.Height);
			}
		}
		public static void PatchAfterUnregister(Form chidForm) {
			PatchAfterUnregister(chidForm, new FormBorderInfo() { FormBorderStyle = FormBorderStyle.Sizable });
		}
		public static void PatchAfterUnregister(Form chidForm, FormBorderStyle borderStyle) {
			FormBorderInfo info;
			if(!FormBorderInfo.TryGetValue(chidForm, out info))
				info = new FormBorderInfo() { FormBorderStyle = borderStyle };
			PatchAfterUnregister(chidForm, info);
		}
		public static void PatchAfterUnregister(Form chidForm, FormBorderInfo borderInfo) {
			if(chidForm == null) return;
			chidForm.Dock = DockStyle.None;
			chidForm.MinimizeBox = borderInfo.MinimizeBox.HasValue ? borderInfo.MinimizeBox.Value : true;
			chidForm.MaximizeBox = borderInfo.MaximizeBox.HasValue ? borderInfo.MaximizeBox.Value : true;
			chidForm.MinimumSize = borderInfo.MinimumSize.HasValue ? borderInfo.MinimumSize.Value : Size.Empty;
			chidForm.MaximumSize = borderInfo.MaximumSize.HasValue ? borderInfo.MaximumSize.Value : Size.Empty;
			chidForm.FormBorderStyle = borderInfo.FormBorderStyle;
			if(chidForm.IsHandleCreated)
				chidForm.BeginInvoke(new Action<Form>(Force_B138501_CB46592_Fix), chidForm);
		}
		public static void PatchMaximized(Form chidForm) {
			if(chidForm != null && chidForm.WindowState != FormWindowState.Normal)
				chidForm.WindowState = FormWindowState.Normal;
		}
		public static void PatchActiveChild(Form activeChild, Form[] children, Rectangle bounds, Rectangle view) {
			foreach(Form child in children) {
				if(child == activeChild)
					PatchMaximized(child);
				child.SuspendLayout();
				try {
					if(child != activeChild)
						child.Bounds = new Rectangle(view.X - bounds.Width, view.Y - bounds.Height, bounds.Width, bounds.Height);
					else child.Bounds = bounds;
				}
				finally { child.ResumeLayout(); }
				if(child != activeChild)
					PatchMaximized(child);
			}
		}
		public static void PatchBeforeActivateChild(Control activatedChild, Control activeChild, Rectangle bounds) {
			if(activatedChild == null || activeChild == null) return;
			if(!activatedChild.IsHandleCreated) return;
			if(!ReferenceEquals(activatedChild, activeChild)) 
				TryResizeControlOutOfTheView(activatedChild, bounds);
		}
		static void TryResizeControlOutOfTheView(Control ctrl, Rectangle client) {
			Rectangle ctrlBounds = ctrl.Bounds;
			if(ctrlBounds.Left == -ctrlBounds.Width && ctrlBounds.Top== -ctrlBounds.Height) 
				ResizeControlOutOfTheView(ctrl, client, ctrlBounds);
			else ctrl.Bounds = client;
		}
		internal static void ResizeControlOutOfTheView(Control ctrl, Rectangle bounds) {
			if(ctrl.Size != bounds.Size) 
				ResizeControlCore(ctrl, bounds);
		}
		internal static void ResizeControlOutOfTheView(Control ctrl, Rectangle client, Rectangle ctrlBounds) {
			if(ctrlBounds.Size != client.Size) 
				ResizeControlCore(ctrl, client);
			ctrl.Location = client.Location;
		}
		internal static bool MaximizeControl(IntPtr handle, Size size, Size ownerSize) {
			if(size.Width == ownerSize.Width && size.Height == ownerSize.Height) return false;
			int flags = DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOREDRAW |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOMOVE |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOOWNERZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOACTIVATE;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowPos(handle, IntPtr.Zero,
			   0, 0, ownerSize.Width, ownerSize.Height, flags);
			return true;
		}
		internal static bool GetIsMaximized(IntPtr handle) {
			var wp = new DevExpress.XtraBars.BarNativeMethods.WINDOWPLACEMENT();
			wp.length = System.Runtime.InteropServices.Marshal.SizeOf(
				typeof(DevExpress.XtraBars.BarNativeMethods.WINDOWPLACEMENT));
			DevExpress.XtraBars.BarNativeMethods.GetWindowPlacement(handle, ref wp);
			return wp.showCmd == 0x3 ;
		}
		static void ResizeControlCore(Control ctrl, Rectangle bounds) {
			if(!ctrl.IsHandleCreated) return;
			int flags = DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOREDRAW |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOOWNERZORDER |
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SWP.SWP_NOACTIVATE;
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SetWindowPos(ctrl.Handle, IntPtr.Zero,
			   -bounds.Width, -bounds.Height, bounds.Width, bounds.Height, flags);
		}
		delegate void B138501_CB46592_FixInvoker(Form control);
		static void Force_B138501_CB46592_Fix(Form control) {
			recreateHandle.Invoke(control, null);
			DevExpress.Utils.WXPaint.WXPPainter.Default.ResetWindowTheme(control);
		}
		static MethodInfo recreateHandle;
		static MdiChildHelper() {
			recreateHandle = typeof(Control).GetMethod("RecreateHandle", BindingFlags.NonPublic | BindingFlags.Instance);
		}
	}
	public class FormBorderInfo {
		#region static
		static IDictionary<FormWeakReference, FormBorderInfo> infos = new Dictionary<FormWeakReference, FormBorderInfo>();
		static void AttachCore(Form form, FormBorderInfo info) {
			if(form == null) return;
			FormBorderInfo existingInfo = null;
			if(infos.TryGetValue(new FormWeakReference(form), out existingInfo)) {
				existingInfo.FormBorderStyle = info.FormBorderStyle;
				existingInfo.MaximizeBox = info.MaximizeBox;
				existingInfo.MinimizeBox = info.MinimizeBox;
				existingInfo.MinimumSize = info.MinimumSize;
				existingInfo.MaximumSize = info.MaximumSize;
			}
			else infos.Add(new FormWeakReference(form), info);
		}
		public static void Attach(Form form) {
			FormBorderInfo info = new FormBorderInfo()
			{
				FormBorderStyle = form.FormBorderStyle,
				MinimizeBox = form.MinimizeBox,
				MaximizeBox = form.MaximizeBox,
				MinimumSize = form.MinimumSize,
				MaximumSize = form.MaximumSize,
			};
			AttachCore(form, info);
		}
		public static bool Contains(Form form) {
			return (form != null) && infos.ContainsKey(new FormWeakReference(form));
		}
		public static bool TryGetValue(Form form, out FormBorderInfo info) {
			return infos.TryGetValue(new FormWeakReference(form), out info);
		}
		public static void Detach(Form form) {
			if(form != null)
				infos.Remove(new FormWeakReference(form));
		}
		#endregion static
		public FormBorderStyle FormBorderStyle { get; set; }
		public bool? MinimizeBox { get; private set; }
		public bool? MaximizeBox { get; private set; }
		public Size? MinimumSize { get; private set; }
		public Size? MaximumSize { get; private set; }
	}
	class FormWeakReference : WeakReference {
		int formHashCodeCore;
		public FormWeakReference(object form)
			: base(form) {
			formHashCodeCore = form.GetHashCode();
		}
		public override int GetHashCode() {
			return formHashCodeCore;
		}
		public override bool Equals(object value) {
			if(value == null) {
				return false;
			}
			if(value.GetHashCode() != formHashCodeCore) {
				return false;
			}
			return true;
		}
	}
	static class MDIChildPreview {
		public static Image Create(Control control, Color transparentColor) {
			if(control == null) return null;
			Size size = control.ClientSize;
			if(size.Width <= 0 || size.Height <= 0) return null;
			Bitmap image = new Bitmap(size.Width, size.Height);
			control.DrawToBitmap(image, new Rectangle(0, 0, size.Width, size.Height));
			if(!transparentColor.IsEmpty)
				image.MakeTransparent(transparentColor);
			return image;
		}
	}
}
