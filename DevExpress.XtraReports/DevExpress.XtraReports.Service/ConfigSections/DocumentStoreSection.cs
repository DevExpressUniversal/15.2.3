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

using System.Configuration;
using DevExpress.XtraReports.Service.ConfigSections.Native;
namespace DevExpress.XtraReports.Service.ConfigSections {
	public class DocumentStoreSection : ConfigurationElement {
		const string KeepIntervalPropertyName = "keepInterval";
		const string ConnectionStringNamePropertyName = "connectionStringName";
		const string BinaryStorageChunkSizePropertyName = "binaryChunkSize";
		[ConfigurationProperty(KeepIntervalPropertyName, IsRequired = false, DefaultValue = ConfigurationDefaultConstants.DocumentStoreKeepIntervalValue)]
		[IntegerValidator(MinValue = 2000)]
		public int KeepInterval {
			get { return (int)base[KeepIntervalPropertyName]; }
			set { base[KeepIntervalPropertyName] = value; }
		}
		[ConfigurationProperty(ConnectionStringNamePropertyName, IsRequired = false, DefaultValue = ConfigurationDefaultConstants.ConnectionStringNameValue)]
		public string ConnectionStringName {
			get { return (string)base[ConnectionStringNamePropertyName]; }
			set { base[ConnectionStringNamePropertyName] = value; }
		}
		[ConfigurationProperty(BinaryStorageChunkSizePropertyName, IsRequired = false, DefaultValue = ConfigurationDefaultConstants.BinaryStorageChunkSizeValue)]
		[IntegerValidator(MinValue = 128)]
		public int BinaryStorageChunkSize {
			get { return (int)base[BinaryStorageChunkSizePropertyName]; }
			set { base[BinaryStorageChunkSizePropertyName] = value; }
		}
	}
}
