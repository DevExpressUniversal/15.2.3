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
	abstract class PredefinedDataProcessingModesSet : IDataProcessingModesSet {
		IEnumerable<IDataProcessingMode> modes;
		public PredefinedDataProcessingModesSet() {
			var modesHash = new HashSet<IDataProcessingMode>();
			AddDefaultBinding(modesHash);
			foreach(IDataProcessingMode mode in GetModesCore())
				modesHash.Add(mode);
			modes = modesHash;
		}
		protected virtual void AddDefaultBinding(HashSet<IDataProcessingMode> modesHash) {
			modesHash.Add(DataProcessingModes.DirectBinding);
			modesHash.Add(DataProcessingModes.SimpleBinding);
		}
		public IEnumerator<IDataProcessingMode> GetEnumerator() {
			return modes.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		protected abstract IEnumerable<IDataProcessingMode> GetModesCore();
	}
	class XmlDataSetDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override void AddDefaultBinding(HashSet<IDataProcessingMode> modesHash) {
			modesHash.Add(DataProcessingModes.XMLtoDataSet);
		}
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { };
		}
	}
	class TypedDataSetDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView, 
					DataProcessingModes.InMemoryBindingSource, 
				};
		}
	}
	class EntityFrameworkDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView,
					DataProcessingModes.InMemoryBindingSource,
					DataProcessingModes.InstantFeedback,
					DataProcessingModes.ServerMode,
					DataProcessingModes.PLinqInstantFeedback,
					DataProcessingModes.PLinqServerMode,
				};
		}
	}
	class XPODataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override void AddDefaultBinding(HashSet<IDataProcessingMode> modesHash) {
			modesHash.Add(DataProcessingModes.XPCollectionForXPO);
			modesHash.Add(DataProcessingModes.XPViewForXPO);
		}
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InstantFeedback,
					DataProcessingModes.ServerMode,
				};
		}
	}
	class LinqToSqlDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView,
					DataProcessingModes.InMemoryBindingSource,  
					DataProcessingModes.InstantFeedback,
					DataProcessingModes.ServerMode,
					DataProcessingModes.PLinqInstantFeedback,
					DataProcessingModes.PLinqServerMode,
				};
		}
	}
	class WcfDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView, 
					DataProcessingModes.InMemoryBindingSource,  
					DataProcessingModes.InstantFeedback, 
					DataProcessingModes.ServerMode, 
				};
		}
	}
	class RiaDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView, 
					DataProcessingModes.InstantFeedback, 
				};
		}
	}
	class IEnumerableDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryCollectionView, 
					DataProcessingModes.InMemoryBindingSource, 
					DataProcessingModes.PLinqInstantFeedback, 
					DataProcessingModes.PLinqServerMode, 
				};
		}
	}
	class EnumDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
				};
		}
	}
	class SQLDataSourceDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryBindingSource, 
				};
		}
	}
	class ExcelDataSourceDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.InMemoryBindingSource, 
				};
		}
	}
	class OLAPDataProcessingModesSet : PredefinedDataProcessingModesSet {
		protected override void AddDefaultBinding(HashSet<IDataProcessingMode> modesHash) {
			modesHash.Add(DataProcessingModes.OLEDBforOLAP);
		}
		protected override IEnumerable<IDataProcessingMode> GetModesCore() {
			return new IDataProcessingMode[] { 
					DataProcessingModes.ADOMDforOLAP, 
					DataProcessingModes.XMLAforOLAP, 
				};
		}
	}
}
