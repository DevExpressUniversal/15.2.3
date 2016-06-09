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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	public sealed class DataProcessingModes {
		#region static
		public static IDataProcessingMode DirectBinding {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.DirectBinding); }
		}
		public static IDataProcessingMode SimpleBinding {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.SimpleBinding); }
		}
		public static IDataProcessingMode InMemoryCollectionView {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.InMemoryCollectionView); }
		}
		public static IDataProcessingMode InMemoryBindingSource {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.InMemoryBindingSource); }
		}
		public static IDataProcessingMode InstantFeedback {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.InstantFeedback); }
		}
		public static IDataProcessingMode ServerMode {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.ServerMode); }
		}
		public static IDataProcessingMode PLinqInstantFeedback {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.PLinqInstantFeedback); }
		}
		public static IDataProcessingMode PLinqServerMode {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.PLinqServerMode); }
		}
		public static IDataProcessingMode XMLtoDataSet {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.XMLtoDataSet); }
		}
		public static IDataProcessingMode OLEDBforOLAP {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.OLEDBforOLAP); }
		}
		public static IDataProcessingMode ADOMDforOLAP {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.ADOMDforOLAP); }
		}
		public static IDataProcessingMode XMLAforOLAP {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.XMLAforOLAP); }
		}
		public static IDataProcessingMode XPCollectionForXPO {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.XPCollectionForXPO); }
		}
		public static IDataProcessingMode XPViewForXPO {
			get { return DataProcessingMode.FromCodeName(DataProcessingModeCodeName.XPViewForXPO); }
		}
		internal static IDataProcessingMode FromCodeName(DataProcessingModeCodeName codeName) {
			return DataProcessingMode.FromCodeName(codeName);
		}
		#endregion static
		sealed class DataProcessingMode : LocalizableDataAccessObject<DataProcessingModeCodeName>, IDataProcessingMode {
			DataProcessingMode(DataProcessingModeCodeName codeName)
				: base(codeName) {
				string strCodeName = codeName.ToString();
				IsInMemoryProcessing = strCodeName.Contains("InMemory") || strCodeName.Contains("XPCollection") || strCodeName.Contains("XPView");
				IsAsynchronous = strCodeName.Contains("InstantFeedback");
				IsServerSide = strCodeName.Contains("ServerMode") || strCodeName.Contains("InstantFeedback") || strCodeName.Contains("OLAP");
				IsOLAP = strCodeName.Contains("OLAP");
				IsParallel = strCodeName.StartsWith("PLinq");
			}
			public bool IsServerSide { get; private set; }
			public bool IsAsynchronous { get; private set; }
			public bool IsParallel { get; private set; }
			public bool IsOLAP { get; private set; }
			public bool IsInMemoryProcessing { get; private set; }
			protected override int GetHash(DataProcessingModeCodeName codeName) {
				return (int)codeName;
			}
			#region static
			static IDictionary<DataProcessingModeCodeName, IDataProcessingMode> modes;
			static DataProcessingMode() {
				modes = new Dictionary<DataProcessingModeCodeName, IDataProcessingMode>();
			}
			internal static IDataProcessingMode FromCodeName(DataProcessingModeCodeName codeName) {
				IDataProcessingMode mode;
				if(!modes.TryGetValue(codeName, out mode)) {
					mode = new DataProcessingMode(codeName);
					modes.Add(codeName, mode);
				}
				return mode;
			}
			#endregion static
		}
	}
}
