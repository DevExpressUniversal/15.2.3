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
using System.ComponentModel;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraPrinting.Preview {
	public abstract class RibbonControllerBase : Component , ISupportInitialize {
		bool isDisposed = false;
		RibbonControl ribbonControl;
		RibbonStatusBar ribbonStatusBar;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public virtual RibbonControl RibbonControl {
			get { return ribbonControl; }
			set { ribbonControl = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public virtual RibbonStatusBar RibbonStatusBar {
			get { return ribbonStatusBar; }
			set { ribbonStatusBar = value; }
		}
		static RibbonControllerBase() {
			DevExpress.Utils.Design.DXAssemblyResolverEx.Init();
		}
		protected RibbonControllerBase() {
		}
		public void Initialize(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar) {
			Initialize(ribbonControl, ribbonStatusBar, GetImagesFromAssembly());
		}
		protected internal abstract Dictionary<string, Image> GetImagesFromAssembly();
		protected internal abstract void ConfigureRibbonController(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images);
		internal void Initialize(RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images) {
			if(ribbonControl == null)
				throw new ArgumentNullException("ribbonControl");
			((ISupportInitialize)this).BeginInit();
			RibbonControl = ribbonControl;
			RibbonControl.TransparentEditors = true;
			RibbonControl.AutoHideEmptyItems = true;
			RibbonStatusBar = ribbonStatusBar;
			if(ribbonStatusBar != null)
				ribbonStatusBar.Ribbon = ribbonControl;
			ConfigureRibbonController(RibbonControl, RibbonStatusBar, images);
			((ISupportInitialize)this).EndInit();
		}
		protected override void Dispose(bool disposing) {
			if(disposing & !isDisposed) {
					DestroyOwnObjects();
				ribbonControl = null;
				isDisposed = true;
				}
				base.Dispose(disposing);
			}
		void DestroyOwnObjects() {
			ICollection<IDisposable> objectsToDispose = GetObjectsToDispose();
			if(objectsToDispose == null) return;
			foreach(IDisposable obj in objectsToDispose) {
				if(obj != null) {
					try {
					obj.Dispose();
					} catch { }
				}
			}
		}
		protected virtual ICollection<IDisposable> GetObjectsToDispose() { return null; }
		#region ISupportInitialize Members
		public abstract void BeginInit();
		public abstract void EndInit();
		#endregion
	}
}
namespace DevExpress.XtraPrinting.Preview.Native {
	public static class RibbonControllerHelper {
		public static void Initialize(RibbonControllerBase controller, RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images) {
			controller.Initialize(ribbonControl, ribbonStatusBar, images);
		}
		public static void ConfigurePrintRibbonController(DevExpress.XtraPrinting.Preview.PrintRibbonController controller, RibbonControl ribbonControl, RibbonStatusBar ribbonStatusBar, Dictionary<string, Image> images) {
			controller.ConfigureRibbonController(ribbonControl, ribbonStatusBar, images);
		}
		public static Dictionary<string, Image> JoinImageDictionaries(Dictionary<string, Image> dictionary1, Dictionary<string, Image> dictionary2) {
			Dictionary<string, Image> result = new Dictionary<string, Image>();
			foreach(KeyValuePair<string, System.Drawing.Image> kv in dictionary1)
				result.Add(kv.Key, kv.Value);
			foreach(KeyValuePair<string, System.Drawing.Image> kv in dictionary2)
				result.Add(kv.Key, kv.Value);
			return result;
		}
	}
}
