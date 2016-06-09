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
using System.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting.Localization;
using System.IO;
namespace DevExpress.XtraPrinting {
	public static class ExceptionHelper {
		public static void ThrowInvalidOperationException(string message) {
			ThrowInvalidOperationException<object>(message);
		}
		public static void ThrowInvalidOperationException() {
			ThrowInvalidOperationException<object>(string.Empty);
		}
		public static T ThrowInvalidOperationException<T>(string message) {
			throw new InvalidOperationException(message);
		}
		public static T ThrowInvalidOperationException<T>() {
			return ThrowInvalidOperationException<T>(string.Empty);
		}
		public static Exception CreateFriendlyException(IOException ex, string fileName) {
			return new Exception(PreviewStringId.Msg_CannotAccessFile.GetString(fileName), ex);
		}
		public static Exception CreateFriendlyException(OutOfMemoryException ex) {
			return new Exception(PreviewStringId.Msg_BigFileToCreate.GetString(), ex);
		}
	}
}
