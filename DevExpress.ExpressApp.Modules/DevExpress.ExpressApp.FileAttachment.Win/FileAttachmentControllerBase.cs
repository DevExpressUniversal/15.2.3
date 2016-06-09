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
using System.Text;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public class FileAttachmentControllerBase : ViewController {
		private IFileDataManager fileDataManager;
		public const string ActiveKeyAttachmentAssigned = "AttachmentAssigned";
		public const string ActiveKeyHasFileAttachmentAttribute = "HasFileAttachmentAttribute";
		public const string ActiveKeyObjectSpaceNotModified = "ObjectSpaceNotModified";
		protected override void OnViewChanging(View view) {
			base.OnViewChanging(view);
			FileAttachmentAttribute attribute = GetFileAttachmentAttribute(view);
			Active.SetItemValue(ActiveKeyHasFileAttachmentAttribute, attribute != null);
		}
		protected virtual FileAttachmentAttribute GetFileAttachmentAttribute(View view) {
			if(view is ObjectView) {
				return ((ObjectView)view).ObjectTypeInfo.FindAttribute<FileAttachmentAttribute>();
			}
			else {
				return null;
			}
		}
		protected IFileDataManager FileDataManager {
			get {
				if(fileDataManager == null) {
					fileDataManager = FileAttachmentsWindowsFormsModule.GetFileDataManager(Application);
				}
				return fileDataManager;
			}
		}
	}
}
