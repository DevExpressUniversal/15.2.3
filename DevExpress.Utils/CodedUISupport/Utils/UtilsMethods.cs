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
using System.ComponentModel;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Skins.XtraForm;
using System.Reflection;
namespace DevExpress.Utils.CodedUISupport {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoteObjectUtilsMethods {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectUtilsMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public FormElementInfo GetXtraFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new FormElementInfo() : HelperManager.Get(windowHandle).UtilsMethods.GetXtraFormElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetXtraFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).UtilsMethods.GetXtraFormElementRectangle(windowHandle, elementInfo);
		}
	}
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideUtilsMethods : ClientSideHelperBase {
		internal ClientSideUtilsMethods(RemoteObject remoteObject) : base(remoteObject) { }
		public FormElementInfo GetXtraFormElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return CrossThreadRunMethod(windowHandle, delegate() {
				XtraForm form = Control.FromHandle(windowHandle) as XtraForm;
				FormElementInfo result = new FormElementInfo();
				if(form != null && form.FormPainter != null) {
					Point point = new Point(pointX, pointY);
					if(form.FormPainter.CaptionBounds.Contains(point)) {
						result.ElementType = FormElements.Caption;
						FormCaptionButtonCollection buttons = GetXtraFormButtons(form);
						if(buttons != null) {
							foreach(FormCaptionButton button in buttons)
								if(button.Bounds.Contains(point)) {
									result.ElementType = FormElements.Button;
									result.Name = CodedUIUtils.ConvertToString(button.Kind);
									break;
								}
						}
					}
				}
				else {
					Rectangle closeButtonRectangle = CalcFormCloseButtonRect(form);
					if(closeButtonRectangle.Contains(new Point(pointX, pointY))) {
						result.ElementType = FormElements.Button;
						result.Name = CodedUIUtils.ConvertToString(FormCaptionButtonKind.Close);
					}
				}
				return result;
			});
		}
		public string GetXtraFormElementRectangle(IntPtr windowHandle, FormElementInfo elementInfo) {
			return CrossThreadRunMethod(windowHandle, delegate() {
				XtraForm form = Control.FromHandle(windowHandle) as XtraForm;
				if(form != null)
					if(form.FormPainter != null) {
						switch(elementInfo.ElementType) {
							case FormElements.Caption:
								return CodedUIUtils.ConvertToString(form.FormPainter.CaptionBounds);
							case FormElements.Button:
								FormCaptionButtonCollection buttons = GetXtraFormButtons(form);
								if(buttons != null) {
									foreach(FormCaptionButton button in buttons)
										if(elementInfo.Name == CodedUIUtils.ConvertToString(button.Kind))
											return CodedUIUtils.ConvertToString(button.Bounds);
								}
								break;
						}
					}
					else {
						if(elementInfo.ElementType == FormElements.Button && elementInfo.Name == CodedUIUtils.ConvertToString(FormCaptionButtonKind.Close)) {
							Rectangle closeButtonRectangle = CalcFormCloseButtonRect(form);
							return CodedUIUtils.ConvertToString(closeButtonRectangle);
						}
					}
				return null;
			});
		}
		protected FormCaptionButtonCollection GetXtraFormButtons(XtraForm form) {
			PropertyInfo pi = form.FormPainter.GetType().GetProperty("Buttons", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if(pi != null)
				return pi.GetValue(form.FormPainter, new object[] { }) as FormCaptionButtonCollection;
			return null;
		}
		protected Rectangle CalcFormCloseButtonRect(Control control) {
			Rectangle closeButtonRectangle = new Rectangle();
			closeButtonRectangle.Width = SystemInformation.CaptionButtonSize.Width;
			closeButtonRectangle.Height = SystemInformation.CaptionButtonSize.Height;
			closeButtonRectangle.X = control.Bounds.Width - SystemInformation.FixedFrameBorderSize.Width - closeButtonRectangle.Width;
			closeButtonRectangle.Y = SystemInformation.FixedFrameBorderSize.Height;
			return closeButtonRectangle;
		}
	}
}
