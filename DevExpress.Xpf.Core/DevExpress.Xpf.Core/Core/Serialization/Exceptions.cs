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
using DevExpress.Xpf.Core.Serialization.Native;
namespace DevExpress.Xpf.Core.Serialization {
	public class InvalidSerializationIDException : Exception {
		const string messageFormat = "Invalid SerializationID (\"{0}\"). SerializationID must not be null or empty, or contain the '{1}' character.";
		InvalidSerializationIDException(string id)
			: base(string.Format(messageFormat, id, SerializationProviderWrapper.PathSeparator)) {
		}
		public static void Assert(string id) {
			if(!string.IsNullOrEmpty(id) && id.Contains(SerializationProviderWrapper.PathSeparator))
				throw new InvalidSerializationIDException(id);
		}
	}
	public class DuplicateSerializationIDException : Exception {
		const string messageFormat = "Another object with the same SerializationID (\"{0}\") exists.";
		DuplicateSerializationIDException(IDXSerializable dxObj, string id)
			: base(string.Format(messageFormat, id)) {
			FullPath = dxObj.FullPath;
		}
		public string FullPath { get; private set; }
		public static void Assert(IDXSerializable dxObj1, IDXSerializable dxObj2) {
			if(dxObj1.Source != dxObj2.Source)
				throw new DuplicateSerializationIDException(dxObj1, DXSerializer.GetSerializationID(dxObj1.Source));
		}
	}
}
