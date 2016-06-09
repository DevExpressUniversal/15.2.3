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
using DevExpress.Utils;
namespace DevExpress.Data.WizardFramework {
	public interface IWizardPage<TWizardModel> where TWizardModel : IWizardModel {
		event EventHandler Changed;
		event EventHandler<WizardPageErrorEventArgs> Error;
		TWizardModel Model { get; set; }
		bool MoveNextEnabled { get; }
		bool FinishEnabled { get; }
		object PageContent { get; }
		Type GetNextPageType();
		bool Validate(out string errorMessage);
		void Begin();
		void Commit();
	}
	public abstract class WizardPageBase<TView, TModel> : IWizardPage<TModel>
		where TModel : IWizardModel {
		readonly TView view;
		TModel model;
		protected TView View { get { return view; } }
		protected WizardPageBase(TView view) {
			Guard.ArgumentNotNull(view, "view");
			this.view = view;
		}
		#region IWizardPage Members
		public event EventHandler Changed;
		public event EventHandler<WizardPageErrorEventArgs> Error;
		public TModel Model {
			get {
				return model;
			}
			set {
				model = value;
			}
		}
		public virtual bool MoveNextEnabled {
			get {
				return false;
			}
		}
		public virtual bool FinishEnabled {
			get {
				return false;
			}
		}
		public object PageContent {
			get {
				return view;
			}
		}
		public virtual Type GetNextPageType() {
			return null;
		}
		public virtual bool Validate(out string errorMessage) {
			errorMessage = null;
			return true;
		}
		#endregion
		#region Methods
		public abstract void Begin();
		public abstract void Commit();
		protected void RaiseChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		protected void RaiseError(string errorMessage) {
			if(Error != null) {
				Error(this, new WizardPageErrorEventArgs(errorMessage));
			}
		}
		#endregion
	}
}
