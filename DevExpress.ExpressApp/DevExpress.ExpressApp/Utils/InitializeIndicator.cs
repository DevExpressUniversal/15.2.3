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
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Utils {
	public interface ISupportSplashScreen {
		void RemoveSplash();
		void StartSplash();
		void StopSplash();
		void UpdateSplash(string context, string caption, string description, params object[] args);
	}
	public class StatusUpdatingEventArgs : EventArgs {
		private String context;
		private Object[] additionalParams;
		public StatusUpdatingEventArgs(String context, String title, String message, params Object[] additionalParams) {
			this.context = context;
			this.Title = title;
			this.Message = message;
			this.additionalParams = additionalParams;
		}
		public String Context {
			get { return context; }
		}
		public String Title { get; set; }
		public String Message { get; set; }
		public Object[] AdditionalParams {
			get { return additionalParams; }
		}
	}
	public interface ISupportInitializeExt : ISupportInitialize {
		bool IsInitializing { get; }
	}
	public class InitializeIndicator : ISupportInitializeExt {
		private bool isInitializing = false;
		#region ISupportInitializeExt Members
		public bool IsInitializing {
			get { return isInitializing; }
		}
		#endregion
		#region ISupportInitialize Members
		public void BeginInit() {
			if(isInitializing) {
				throw new InvalidOperationException("Cannot begin initialization because it is already started");
			}
			isInitializing = true;
		}
		public void EndInit() {
			if(!isInitializing) {
				throw new InvalidOperationException("Cannot end initialization because it has not been started");
			}
			isInitializing = false;
		}
		#endregion
	}
}
