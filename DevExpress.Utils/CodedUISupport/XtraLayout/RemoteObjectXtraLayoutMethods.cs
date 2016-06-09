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
namespace DevExpress.Utils.CodedUISupport {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class RemoteObjectXtraLayoutMethods : IXtraLayoutCodedUIHelper {
		ClientSideHelpersManager HelperManager;
		internal RemoteObjectXtraLayoutMethods(ClientSideHelpersManager manager) {
			this.HelperManager = manager;
		}
		public LayoutControlElementInfo GetLayoutControlElementFromPoint(IntPtr windowHandle, int pointX, int pointY) {
			return HelperManager.Get(windowHandle) == null ? new LayoutControlElementInfo() : HelperManager.Get(windowHandle).XtraLayoutMethods.GetLayoutControlElementFromPoint(windowHandle, pointX, pointY);
		}
		public string GetLayoutControlElementRectangle(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			return HelperManager.Get(windowHandle) == null ? null : HelperManager.Get(windowHandle).XtraLayoutMethods.GetLayoutControlElementRectangle(windowHandle, elementInfo);
		}
		public LayoutControlElementInfo GetControlParentItem(IntPtr windowHandle) {
			return HelperManager.Get(windowHandle) == null ? new LayoutControlElementInfo() : HelperManager.Get(windowHandle).XtraLayoutMethods.GetControlParentItem(windowHandle);
		}
		public IntPtr GetLayoutControlItemControlHandle(IntPtr layoutControlHandle, LayoutControlElementInfo elementInfo) {
			return HelperManager.Get(layoutControlHandle) == null ? IntPtr.Zero : new IntPtr(HelperManager.Get(layoutControlHandle).XtraLayoutMethods.GetLayoutControlItemControlHandle(layoutControlHandle, elementInfo));
		}
		public ValueStruct GetLayoutControlElementProperty(IntPtr windowHandle, LayoutControlElementInfo elementInfo, LayoutControlElementsProperties property) {
			return HelperManager.Get(windowHandle) == null ? new ValueStruct() : HelperManager.Get(windowHandle).XtraLayoutMethods.GetLayoutControlElementProperty(windowHandle, elementInfo, property);
		}
		public void MakeVisible(IntPtr windowHandle, LayoutControlElementInfo elementInfo) {
			if(HelperManager.Get(windowHandle) != null)
				HelperManager.Get(windowHandle).XtraLayoutMethods.MakeVisible(windowHandle, elementInfo);
		}
	}
}
