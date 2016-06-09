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
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
namespace DevExpress.Utils.Animation {
	public abstract class BaseAsyncAdornerElementInfoArgs : BaseAdornerElementInfoArgs, IAsyncAdornerElementInfoArgs {
		protected virtual IntPtr hWndInsertAfter {
			get { return new IntPtr(-1); }
		}
		protected override IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return new Rectangle[] { Bounds };
		}
		protected override int CalcCore() { return 0; }
		protected abstract void Destroy();
		protected abstract ObjectPainter GetPainter();
		protected abstract void BeginAsync(IntPtr adornerHandle);
		protected abstract void EndAsync();
		public virtual AsyncSkinInfo SkinInfo { get { return null; } }
		#region IAsyncAdornerElementInfoArgs Members
		void IAsyncAdornerElementInfoArgs.Destroy() { Destroy(); }
		ObjectPainter IAsyncAdornerElementInfoArgs.GetPainter() { return GetPainter(); }
		void IAsyncAdornerElementInfoArgs.BeginAsync(IntPtr adornerHandle) { BeginAsync(adornerHandle); }
		void IAsyncAdornerElementInfoArgs.EndAsync() { EndAsync(); }
		IntPtr IAsyncAdornerElementInfoArgs.hWndInsertAfter { get { return hWndInsertAfter; } }
		#endregion
	}
	public class BaseAsyncAdornerElementInfo : BaseAdornerElementInfo {
		public BaseAsyncAdornerElementInfo(AdornerOpaquePainter opaquePainter, BaseAsyncAdornerElementInfoArgs info) : base(opaquePainter, info) { }
		public new IAsyncAdornerElementInfoArgs InfoArgs {
			get { return base.InfoArgs as IAsyncAdornerElementInfoArgs; }
		}
	}
	public class AsyncSkinInfo : IDisposable {
		bool isDisposing;
		AsyncUserLookAndFeelDefault lookAndFeelCore;
		SkinManager managerCore;
		public AsyncSkinInfo(DevExpress.LookAndFeel.UserLookAndFeel lookAndFeel) {
			managerCore = new SkinManager();
			lookAndFeelCore = new AsyncUserLookAndFeelDefault();
			lookAndFeelCore.Assign(lookAndFeel);
			string skinName = lookAndFeel.ActiveSkinName;
			lookAndFeelCore.SetSkinStyle(skinName);
			var skinContainer = DevExpress.Skins.SkinManager.Default.Skins[skinName];
			if(skinContainer == null)
				skinContainer = DevExpress.Skins.SkinManager.Default.Skins["DevExpress Style"];
			SkinContainer container = GetSkinContainer(skinContainer);
			managerCore.RegisterSkin(container.Creator);
		}
		SkinContainer GetSkinContainer(SkinContainer containerCore) {
			SkinContainer container = null;
			if(containerCore.Loaded) {
				string name = containerCore.SkinName;
				SkinCreator creator = containerCore.Creator.Clone(name);
				container = new SkinContainer(creator);
				Skin[] skins = containerCore.GetSkins();
				foreach(Skin skin in skins) {
					Skin cloneSkin = skin.Clone(name) as Skin;
					SkinProductId productId = cloneSkin.GetProductId();
					container.Products[productId] = cloneSkin;
					cloneSkin.SetContainer(container);
				}
			}
			if(container == null) {
				container = new SkinContainer(containerCore.SkinName);
				container.Load();
			}
			return container;
		}
		public SkinManager SkinManager { get { return managerCore; } }
		public DevExpress.LookAndFeel.Design.UserLookAndFeelDefault LookAndFeel { get { return lookAndFeelCore; } }
		public void Dispose() {
			if(isDisposing) return;
			isDisposing = true;
			managerCore.Skins.Clear();
			managerCore = null;
			Ref.Dispose(ref lookAndFeelCore);
		}
		class AsyncUserLookAndFeelDefault : DevExpress.LookAndFeel.Design.UserLookAndFeelDefault {
			protected override void OnFirstSubscribe() { }
			protected override void OnLastUnsubscribe() { }
		}
	}
}
