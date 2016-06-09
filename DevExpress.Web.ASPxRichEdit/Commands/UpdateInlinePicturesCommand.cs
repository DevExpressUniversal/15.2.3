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
using System.Collections;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class UpdateInlinePicturesCommand : WebRichEditUpdateModelCommandBase {
		public UpdateInlinePicturesCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.InsertInlinePicture; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			ArrayList imagesInfo = (ArrayList)Parameters["updatedImagesInfo"];
			for(var i = 0; i < imagesInfo.Count; i++) {
				Hashtable imageInfo = (Hashtable)imagesInfo[i];
				DocumentLogPosition logPosition = new DocumentLogPosition((int)imageInfo["position"]);
				int scaleX = Convert.ToInt32(imageInfo["scaleX"]);
				int scaleY = Convert.ToInt32(imageInfo["scaleY"]);
				int id = (int)imageInfo["id"];
				PieceTable.DeleteContent(logPosition, 1, false);
				PieceTable.InsertInlinePicture(logPosition, DocumentModel.ImageCache.GetImageByKey(id), scaleX, scaleY);
			}
		}
		protected override bool IsValidOperation() {
			return Manager.Client.DocumentCapabilitiesOptions.InlinePicturesAllowed;
		}
	}
}
