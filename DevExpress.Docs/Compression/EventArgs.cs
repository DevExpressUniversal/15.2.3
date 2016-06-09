#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using System.ComponentModel;
using DevExpress.Utils;
namespace DevExpress.Compression {
	#region AllowFileOverwriteEvent
	public delegate void AllowFileOverwriteEventHandler(object sender, AllowFileOverwriteEventArgs e);
	public class AllowFileOverwriteEventArgs : CancelEventArgs {
		public AllowFileOverwriteEventArgs(ZipItem zipItem, string targetFilePath) {
			Guard.ArgumentNotNull(zipItem, "zipItem");
			ZipItem = zipItem;
			TargetFilePath = targetFilePath;
		}
		public ZipItem ZipItem { get; private set; }
		public string TargetFilePath { get; private set; }
	}
	#endregion
	public abstract class CanContinueEventArgs : EventArgs {
		protected CanContinueEventArgs() {
			CanContinue = true;
		}
		public bool CanContinue { get; set; }
	}
	#region ErrorEvent
	public class ErrorEventArgs : CanContinueEventArgs {
		Exception exception;
		string itemName;
		public ErrorEventArgs(Exception exception, String itemName) {
			this.exception = exception;
			this.itemName = itemName;
		}
		public string ItemName { get { return itemName; } }
		public Exception GetException() {
			return exception;
		}
	}
	public delegate void ErrorEventHandler(object sender, ErrorEventArgs args);
	#endregion
	#region ProgressEvent
	public class ProgressEventArgs : CanContinueEventArgs {
		public ProgressEventArgs(double progress) {
			Progress = progress;
		}
		public double Progress { get; private set; }
	}
	public delegate void ProgressEventHandler(object sender, ProgressEventArgs args);
	#endregion
	#region ZipItemAddingEvent
	public class ZipItemAddingEventArgs : EventArgs {
		public ZipItemAddingEventArgs(ZipItem item) {
			Action = ZipItemAddingAction.Continue;
			Item = item;
		}
		public ZipItemAddingAction Action { get; set; }
		public ZipItem Item { get; private set; }
	}
	public delegate void ZipItemAddingEventHandler(object sender, ZipItemAddingEventArgs args);
	#endregion
}
