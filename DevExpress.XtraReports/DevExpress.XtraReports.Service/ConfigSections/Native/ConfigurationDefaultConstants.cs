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

namespace DevExpress.XtraReports.Service.ConfigSections.Native {
	public static class ConfigurationDefaultConstants {
		internal const int DocumentStoreKeepIntervalValue = 86400000;
		internal const string ConnectionStringNameValue = ConfigSectionNameValue;
		internal const int BinaryStorageChunkSizeValue = 20971520;
		const string ConfigSectionGroupNameValue = "devExpress";
		const string ConfigSectionNameValue = "xpf.printing";
		const string ReportServiceDbFileNameValue = "ReportService.mdf";
		public static string ConfigSectionGroupName {
			get { return ConfigSectionGroupNameValue; }
		}
		public static string ConfigSectionName {
			get { return ConfigSectionNameValue; }
		}
		public static string ConnectionStringName {
			get { return ConnectionStringNameValue; }
		}
		public static int BinaryStorageChunkSize {
			get { return BinaryStorageChunkSizeValue; }
		}
		public static int DocumentStoreKeepInterval {
			get { return DocumentStoreKeepIntervalValue; }
		}
		public static string ReportServiceDbFileName {
			get { return ReportServiceDbFileNameValue; }
		}
	}
}
