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
namespace DevExpress.DataAccess.Native.Sql.ConnectionStrategies {
	[Flags]
	public enum ConnectionParameterEdits {
		None				= 0x00000000,
		ServerType		  = 0x00000001,
		FileName			= 0x00000002,
		ServerName		  = 0x00000004,
		Port				= 0x00000008,
		AuthTypeMsSql	   = 0x00000010,
		UserName			= 0x00000020,
		Password			= 0x00000040,
		Database			= 0x00000080,
		CustomString		= 0x00000100,
		ProjectID		   = 0x00000200,
		DataSetID		   = 0x00000400,
		KeyFileName		 = 0x00000800,
		ServiceEmail		= 0x00001000,
		OAuthClientID	   = 0x00002000,
		OAuthClientSecret   = 0x00004000,
		OAuthRefreshToken   = 0x00008000,
		AuthTypeBigQuery	= 0x00010000,
		Hostname			= 0x00020000,
		AdvantageServerType = 0x00040000,
	}
}
