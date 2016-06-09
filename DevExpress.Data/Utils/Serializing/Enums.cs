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
using System.Collections.Generic;
using System.Text;
namespace DevExpress.Utils.Serializing {
	#region DXTypeCode
	public enum DXTypeCode {
		Null = 0x00,		
		Object = 0xEE,	 
		DBNull = 0xDD,	 
		Boolean = 0x0A,   
		Char = 0xCC,		 
		SByte = 0x01,	   
		Byte = 0x02,		 
		Int16 = 0x03,	   
		UInt16 = 0x04,	 
		Int32 = 0x0F,	   
		UInt32 = 0x05,	 
		Int64 = 0x06,	   
		UInt64 = 0x07,	 
		Single = 0xBB,	 
		Double = 0xAA,	 
		Decimal = 0x08,   
		DateTime = 0x09, 
		TimeSpan = 0x0B,		  
		String = 0x77,	 
		Guid = 0x88,				  
		ByteArray = 0x99,			 
		Enum = 0x66,				  
	}
	#endregion
	[Flags]
	public enum XtraSerializationFlags {
		None = 0,
		UseAssign = 1,
		DefaultValue = 2,
		Cached = 4,
		DeserializeCollectionItemBeforeCallSetIndex = 8,
		SuppressDefaultValue = 16,
	}
	public enum XtraSerializationVisibility { Hidden, Visible, Collection, SimpleCollection, NameCollection, Content, Reference };
}
