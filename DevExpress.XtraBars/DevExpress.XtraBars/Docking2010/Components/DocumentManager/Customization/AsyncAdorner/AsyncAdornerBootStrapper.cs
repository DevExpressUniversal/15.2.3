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
using DevExpress.Utils.Animation;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public sealed class AsyncAdornerBootStrapper : BaseAsyncAdornerBootStrapper {	   
		string skinName;
		AsyncAdornerBootStrapper(IntPtr hWnd, AsyncAdornerElementInfo info) : base(hWnd, info){ }
		public static IAsyncAdorner Show(IntPtr hWnd, AsyncAdornerElementInfo info) {
			return new AsyncAdornerBootStrapper(hWnd, info);
		}
		protected override void Initialize(BaseAsyncAdornerElementInfo info) {
			base.Initialize(info);
			AsyncAdornerElementInfoArgs infoArgs = info.InfoArgs as AsyncAdornerElementInfoArgs;
			if(infoArgs != null)
				skinName = infoArgs.Owner.ElementsLookAndFeel.ActiveSkinName;		   
		}	  
		static IList<LFInfo> lfQueue;
		class LFInfo {
			public string SkinName;
			public string RelativePath;
			public System.Reflection.Assembly Assembly;
		}
		public static void RegisterAssembly(System.Reflection.Assembly assembly) {
			if(lfQueue == null)
				lfQueue = new List<LFInfo>();
			lfQueue.Add(new LFInfo() { Assembly = assembly });
		}
		public static void RegisterLookAndFeel(string skinName, string relativePath, System.Reflection.Assembly assembly) {
			if(lfQueue == null)
				lfQueue = new List<LFInfo>();
			lfQueue.Add(new LFInfo() { SkinName = skinName, RelativePath = relativePath, Assembly = assembly });
		}
		static bool RegisterLookAndFeel() {
			if(lfQueue != null) {
				foreach(LFInfo info in lfQueue) {
					if(!string.IsNullOrEmpty(info.SkinName) && !string.IsNullOrEmpty(info.RelativePath)) {
						var skinCreator = new DevExpress.Skins.Info.SkinBlobXmlCreator(info.SkinName,
							info.RelativePath, info.Assembly, null);
						DevExpress.Skins.SkinManager.Default.RegisterSkin(skinCreator);
						return true;
					}
					else { 
						if(info.Assembly!=null)
							return DevExpress.Skins.SkinManager.Default.RegisterAssembly(info.Assembly);
					}
				}
			}
			return false;
		}
		static void RegisterLookAndFeel(string skinName) {
			DevExpress.Utils.Helpers.ReflectionHelper.InvokeStaticMethod(AssemblyInfo.SRAssemblyBonusSkins,
				"DevExpress.UserSkins.BonusSkins", "Register");
			DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(skinName);
		}
		protected override void InitLookAndFeel(IAsyncAdornerElementInfoArgs infoArgs) {
			if(!RegisterLookAndFeel())
				base.InitLookAndFeel(infoArgs);
			RegisterLookAndFeel(skinName);			
		}
		protected override BaseAsyncAdornerWindow CreateAsyncAdornerWindow(BaseAsyncAdornerElementInfo info) {		   
			return new AsyncAdornerWindow(info as AsyncAdornerElementInfo);
		}
		protected override void OnDispose() {
			AsyncAdornerWindow.lockApplicationExit++;
			try {
				if(AdornerWindow != null) {
					DevExpress.Utils.Internal.LayeredWindowMessanger.PostClose(AdornerWindow.Handle);
					if(syncObj != null)
						syncObj.WaitOne();
				}
			}
			finally { AsyncAdornerWindow.lockApplicationExit--; }
			base.OnDispose();
		}
	}
}
