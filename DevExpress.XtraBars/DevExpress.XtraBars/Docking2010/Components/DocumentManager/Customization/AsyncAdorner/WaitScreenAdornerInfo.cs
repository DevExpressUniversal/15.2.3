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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Animation;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
using DevExpress.XtraBars.Docking2010.Views;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class WaitScreenAdornerInfoArgs : AsyncAdornerElementInfoArgs, IWaitingIndicatorInfoArgs {
		BaseDocument documentCore;
		IWaitingIndicatorInfo waitingInfoCore;
		AsyncSkinInfo skinInfoCore;
		WaitScreenAdornerInfoArgs(BaseView owner)
			: base(owner) {
		}
		protected sealed override IntPtr hWndInsertAfter {
			get { return new IntPtr(-2); }
		}
		protected sealed override ObjectPainter GetPainter() {
			return waitingInfoCore.Painter;
		}
		public BaseDocument Document {
			get { return documentCore; }
			internal set { documentCore = value; }
		}
		IntPtr adornerHandle;
		protected sealed override void BeginAsync(IntPtr adornerHandle) {
			this.adornerHandle = adornerHandle;
			waitingInfoCore = RegisterWaitingIndicatorInfo();
			LoadingAnimator.Invalidate += LoadingAnimator_Invalidate;
		}
		protected virtual IWaitingIndicatorInfo RegisterWaitingIndicatorInfo() {
			return new WaitScreenAdornerInfo(this);
		}
		protected sealed override void EndAsync() {
			DestroyCore();
		}
		void LoadingAnimator_Invalidate(object sender, EventArgs e) {
			LayeredWindowMessanger.PostInvalidate(adornerHandle);
		}
		protected sealed override void Destroy() {
			LayeredWindowMessanger.PostClose(adornerHandle);
			DestroyCore();
		}
		protected virtual void DestroyCore() {
			if(waitingInfoCore == null) return;
			LoadingAnimator.Invalidate -= LoadingAnimator_Invalidate;
			Ref.Dispose(ref skinInfoCore);
			Ref.Dispose(ref waitingInfoCore);
			Ref.Dispose(ref localizerInfo);
			waitingInfoCore = null;
		}
		public static WaitScreenAdornerInfoArgs EnsureInfoArgs(ref AsyncAdornerElementInfo target, BaseView owner, BaseDocument document) {
			WaitScreenAdornerInfoArgs args;
			if(target == null) {
				args = new WaitScreenAdornerInfoArgs(owner);
				target = new AsyncAdornerElementInfo(new AsyncAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as WaitScreenAdornerInfoArgs;
			args.Document = document;
			args.Bounds = owner.GetBounds(document);
			args.InitSkinInfo();
			args.InitLocalizerInfo();
			args.SetDirty();
			return args;
		}
		public ILoadingAnimator LoadingAnimator {
			get { return waitingInfoCore.WaitingAnimator; }
		}
		#region IWaitingIndicatorInfoArgs Members
		IWaitingIndicatorInfo IWaitingIndicatorInfoArgs.WaitingInfo {
			get { return waitingInfoCore; }
		}
		public void InitSkinInfo() {
			if(Owner.ElementsLookAndFeel != null)
				skinInfoCore = new AsyncSkinInfo(Owner.ElementsLookAndFeel);
		}
		public override AsyncSkinInfo SkinInfo { get { return skinInfoCore; } }
		#endregion
		void InitLocalizerInfo() {
			localizerInfo = new LocalizerInfo();
		}
		#region Localization
		internal LocalizerInfo localizerInfo;
		internal class LocalizerInfo : IDisposable {
			DevExpress.Utils.Localization.XtraLocalizer<DocumentManagerStringId> localizer;
			public LocalizerInfo() {
				localizer = DocumentManagerLocalizer.Active;
			}
			void IDisposable.Dispose() {
				localizer = null;
			}
			internal string GetString(DocumentManagerStringId stringId) {
				return (localizer ?? DocumentManagerLocalizer.Active).GetLocalizedString(stringId);
			}
		}
		#endregion Localization
	}
	public class WaitScreenPainter : WaitingIndicatorPainter { }
	public class WaitScreenSkinPainter : WaitingIndicatorSkinPainter {
		public WaitScreenSkinPainter(ISkinProvider provider) : base(provider) { }
	}
	public class WaitScreenAdornerInfo : BaseWaitingIndicatorInfo {
		WaitScreenAdornerInfoArgs infoArgs;
		WaitingIndicatorPainter painter;
		ILoadingAnimator loadingAnimatorCore;
		public WaitScreenAdornerInfo(WaitScreenAdornerInfoArgs args)
			: base(args.Owner.LoadingIndicatorProperties) {
			infoArgs = args;
			painter = args.Owner.Painter.GetWaitScreenPainter() as WaitingIndicatorPainter;
			loadingAnimatorCore = new LoadingAnimator(LoadingImages.GetImage(args.Owner.ElementsLookAndFeel, true), true);
		}
		protected override void OnDispose() {
			Ref.Dispose(ref loadingAnimatorCore);
			base.OnDispose();
			painter = null;
			infoArgs = null;
		}
		protected override Size CalcContentSize() {
			int wInterval = (ImageBounds.Width == 0) || (CaptionBounds.Width == 0 && DescriptionBounds.Width == 0) ? 0 :
				Properties.ImageOffset + Properties.ImageToTextDistance + ImageBounds.Width;
			int w = ImageBounds.Width + Math.Max(CaptionBounds.Width, DescriptionBounds.Width) + wInterval;
			int hInterval = (CaptionBounds.Height == 0) || (DescriptionBounds.Height == 0) ? 0 : Properties.CaptionToDescriptionDistance;
			int h = Math.Max(CaptionBounds.Height + DescriptionBounds.Height + hInterval, ImageBounds.Height);
			return new Size(w, h);
		}
		protected override Point CalcCaptionLocation(Rectangle clientRect) {
			int x = ImageBounds.Right + Properties.ImageToTextDistance;
			int y = (DescriptionBounds.Height != 0) ? clientRect.Y : ImageBounds.Top + (ImageBounds.Height - CaptionBounds.Height) / 2;
			return new Point(x, y);
		}
		protected override Point CalcDescriptionLocation(Rectangle clientRect) {
			int x = CaptionBounds.X;
			int y = CaptionBounds.Width != 0 ? CaptionBounds.Bottom + Properties.CaptionToDescriptionDistance : ImageBounds.Top + (ImageBounds.Height - DescriptionBounds.Height) / 2;
			return new Point(x, y);
		}
		protected override Point CalcImageLocation(Rectangle clientRect) {
			int x = clientRect.X + Properties.ImageOffset;
			int y = clientRect.Y + (clientRect.Height - ImageBounds.Height) / 2;
			return new Point(x, y);
		}
		public override string Caption {
			get { return Properties.Caption ?? GetLocalizableString(DocumentManagerStringId.LoadingIndicatorCaption); }
		}
		public override string Description {
			get {
				if(!string.IsNullOrEmpty(Properties.DescriptionFormat))
					return string.Format(Properties.DescriptionFormat, infoArgs.Document.Caption, infoArgs.Document.Header);
				return Properties.Description ?? GetLocalizableString(DocumentManagerStringId.LoadingIndicatorDescription);
			}
		}
		string GetLocalizableString(DocumentManagerStringId id) {
			if(infoArgs == null || infoArgs.localizerInfo == null)
				return DocumentManagerLocalizer.GetString(id);
			return infoArgs.localizerInfo.GetString(id);
		}
		public override WaitingIndicatorPainter Painter { get { return painter; } }
		public override ILoadingAnimator WaitingAnimator { get { return loadingAnimatorCore; } }
		public new ILoadingIndicatorProperties Properties { get { return base.Properties as ILoadingIndicatorProperties; } }
	}
}
