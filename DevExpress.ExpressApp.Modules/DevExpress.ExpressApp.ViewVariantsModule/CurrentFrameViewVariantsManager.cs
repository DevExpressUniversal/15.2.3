#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ViewVariantsModule {
	public interface ICurrentFrameViewVariantsManager : IDisposable {
		void ChangeToVariant(VariantInfo variantInfo);
		void RefreshVariants();
		VariantsInfo Variants { get; }
		event EventHandler<EventArgs> VariantsChanged;
	}
	public class CurrentFrameViewVariantsManager : ICurrentFrameViewVariantsManager, IDisposable {
		private VariantsInfo viewVariants;
		private bool isChangingViewVariant = false;
		private IFrameVariantsEngine variantsManager;
		private Frame frame;
		private void Frame_ViewChanged(object sender, ViewChangedEventArgs e) {
			RefreshVariants();
		}
		protected virtual void OnVariantsChanged() {
			if(VariantsChanged != null) {
				VariantsChanged(this, EventArgs.Empty);
			}
		}
		public CurrentFrameViewVariantsManager(Frame frame, IFrameVariantsEngine variantsManager) {
			Guard.ArgumentNotNull(frame, "frame");
			Guard.ArgumentNotNull(variantsManager, "variantsManager");
			this.frame = frame;
			frame.ViewChanged += new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
			this.variantsManager = variantsManager;
			RefreshVariants();
		}
		public void Dispose() {
			viewVariants = null;
			variantsManager = null;
			if(frame != null) {
				frame.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Frame_ViewChanged);
				frame = null;
			}
		}
		public void RefreshVariants() {
			if(!isChangingViewVariant) {
				if((frame != null) && (frame.View != null) && (VariantsManager != null)) {
					Variants = VariantsManager.GetVariants(frame.View);
				}
				else {
					Variants = null;
				}
			}
		}
		public VariantsInfo Variants {
			get { return viewVariants; }
			private set {
				if(viewVariants != value) {
					viewVariants = value;
					OnVariantsChanged();
				}
			}
		}
		public void ChangeToVariant(VariantInfo variantInfo) {
			isChangingViewVariant = true;
			try {
				if((frame == null) || (VariantsManager == null)) {
					throw new ObjectDisposedException(GetType().FullName);
				}
				if(Variants == null) {
					throw new InvalidOperationException("Variants is null");
				}
				VariantsManager.ChangeFrameViewToVariant(frame, Variants, variantInfo);
			}
			finally {
				isChangingViewVariant = false;
			}
		}
		public IFrameVariantsEngine VariantsManager {
			get { return variantsManager; }
		}
		public event EventHandler<EventArgs> VariantsChanged;
	}
}
