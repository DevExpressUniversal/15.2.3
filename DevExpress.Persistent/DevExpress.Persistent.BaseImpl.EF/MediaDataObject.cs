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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
namespace DevExpress.Persistent.BaseImpl.EF {
	[MediaDataObjectAttribute("MediaDataKey", "MediaData", "MediaResource.MediaData")]
	[Browsable(false)]
	[DevExpress.Persistent.Base.EditorAlias(EditorAliases.ImagePropertyEditor)]
	public class MediaDataObject : IXafEntityObject {
		private bool loaded = false;
		[Browsable(false)]
		public int Id { get; protected set; }
		[NotMapped]
		[Browsable(false)]
		public byte[] MediaData {
			get {
				return MediaResource != null ? MediaResource.MediaData : null;
			}
			set {
				if(loaded && MediaDataAreNotEqual(MediaResource.MediaData, value)) {
					MediaDataKey = (value == null || value.Length == 0) ? null : CreateKey();
					MediaResource.MediaData = value;
				}
			}
		}
		protected virtual bool MediaDataAreNotEqual(byte[] oldValue, byte[] newValue) {
			return (oldValue == null && newValue != null) ||
				(oldValue != null && newValue == null) ||
				!oldValue.SequenceEqual(newValue);
		}
		[Browsable(false)]
		[DevExpress.ExpressApp.DC.Aggregated]
		public virtual MediaResourceObject MediaResource { get; set; }
		[Browsable(false)]
		public string MediaDataKey { get; private set; }
		protected virtual string CreateKey() {
			return Guid.NewGuid().ToString("N");
		}
		#region IXafEntityObject Members
		public void OnCreated() {
			MediaResource = new MediaResourceObject();
			loaded = true;
		}
		public void OnSaving() {
		}
		public void OnLoaded() {
			loaded = true;
		}
		#endregion
	}
	[Browsable(false)]
	public class MediaResourceObject {
		[Browsable(false)]
		public int Id { get; protected set; }
		[Browsable(false)]
		public byte[] MediaData { get; set; }
	}
}
