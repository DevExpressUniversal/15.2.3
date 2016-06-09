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
#if NETFX_CORE
using Windows.Data.Xml.Dom;
#else 
using System.Xml;
#endif
namespace DevExpress.WinRTPresenter.BackgroundTasks {
	public sealed class MessageClass {
		public string FileName { get; set; }
		public XmlDocument Content { get; set; }
		public DateTimeOffset? ExpirationTime { get; set; }
		public string Tag { get; set; }
	}
	public sealed class RegisterWinClass {
		public string Id { get; set; }
		public string AppName { get; set; }
		public string ExePath { get; set; }
		public int ProcessId { get; set; }
		public string TileId { get; set; }
		public bool Equals(RegisterWinClass other) {
			bool tileIdEquals = true;
			if(Object.ReferenceEquals(other, null)) return false;
			if(Object.ReferenceEquals(this, other)) return true;
			if(TileId != null && other.TileId != null)
				tileIdEquals = TileId.Equals(other.TileId);
			else
				if(TileId == null && other.TileId == null)
					tileIdEquals = true;
				else
					tileIdEquals = false;
			return Id.Equals(other.Id) && ExePath.Equals(other.ExePath) && ProcessId.Equals(other.ProcessId) && AppName.Equals(other.AppName)
				&& tileIdEquals;
		}
		public override int GetHashCode() {
			int hashId = Id.GetHashCode();
			int hashAppName = AppName.GetHashCode();
			int hashExepath = ExePath.GetHashCode();
			int hashProcessId = ProcessId.GetHashCode();
			int hashTileId = TileId == null ? 0 : TileId.GetHashCode();
			return hashId + hashExepath + hashProcessId + hashAppName + hashTileId;
		}
	}
}
