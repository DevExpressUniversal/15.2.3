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
namespace DevExpress.PivotGrid.OLAP.AdoWrappers {
	public sealed class AdomdSchemaGuid {
		public static readonly Guid Actions = new Guid("{A07CCD08-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid Catalogs = new Guid("{C8B52211-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid Columns = new Guid("{C8B52214-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid Connections = new Guid("{a07ccd25-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Cubes = new Guid("{C8B522D8-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid DataSources = new Guid("{06c03d41-f66d-49f3-b1b8-987f7af4cf18}");
		public static readonly Guid DBConnections = new Guid("{a07ccd2a-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Dimensions = new Guid("{C8B522D9-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid DimensionStat = new Guid("{a07ccd90-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Enumerators = new Guid("{55a9e78b-accb-45b4-95a6-94c5065617a7}");
		public static readonly Guid Functions = new Guid("{A07CCD07-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid Hierarchies = new Guid("{C8B522DA-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid InputDataSources = new Guid("{A07CCD32-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid Instances = new Guid("{20518699-2474-4c15-9885-0E947EC7A7E3}");
		public static readonly Guid Jobs = new Guid("{a07ccd27-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Keywords = new Guid("{1426c443-4cdd-4a40-8f45-572fab9bbaa1}");
		public static readonly Guid Kpis = new Guid("{2ae44109-ed3d-4842-b16f-b694d1cb0e3f}");
		public static readonly Guid Levels = new Guid("{C8B522DB-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid Literals = new Guid("{c3ef5ecb-0a07-4665-a140-b075722dbdc2}");
		public static readonly Guid Locations = new Guid("{a07ccd92-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Locks = new Guid("{A07CCD24-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid MasterKey = new Guid("{a07ccd29-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid MeasureGroupDimensions = new Guid("{a07CCD33-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid MeasureGroups = new Guid("{E1625EBF-FA96-42fd-BEA6-DB90ADAFD96B}");
		public static readonly Guid Measures = new Guid("{C8B522DC-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid MemberProperties = new Guid("{C8B522DD-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid Members = new Guid("{C8B522DE-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid MemoryGrant = new Guid("{A07CCD23-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid MemoryUsage = new Guid("{A07CCD21-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid MiningColumns = new Guid("{3ADD8A78-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningFunctions = new Guid("{3ADD8A79-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningModelContent = new Guid("{3ADD8A76-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningModelContentPmml = new Guid("{4290B2D5-0E9C-4AA7-9369-98C95CFD9D13}");
		public static readonly Guid MiningModels = new Guid("{3ADD8A77-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningModelXml = new Guid("{4290B2D5-0E9C-4AA7-9369-98C95CFD9D13}");
		public static readonly Guid MiningServiceParameters = new Guid("{3ADD8A75-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningServices = new Guid("{3ADD8A95-D8B9-11D2-8D2A-00E029154FDE}");
		public static readonly Guid MiningStructureColumns = new Guid("{9952E836-BFBF-4D1F-8535-9B67DBD9DDFE}");
		public static readonly Guid MiningStructures = new Guid("{883269F3-0CAD-462f-B6F5-E88A72418C4B}");
		public static readonly Guid PartitionDimensionStat = new Guid("{a07ccd8e-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid PartitionStat = new Guid("{a07ccd8f-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid PerformanceCounters = new Guid("{a07ccd2e-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid ProviderTypes = new Guid("{C8B5222C-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid SchemaRowsets = new Guid("{eea0302b-7922-4992-8991-0e605d0e5593}");
		public static readonly Guid Sessions = new Guid("{a07ccd26-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Sets = new Guid("{A07CCD0B-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid Tables = new Guid("{C8B52229-5CF3-11CE-ADE5-00AA0044773D}");
		public static readonly Guid TablesInfo = new Guid("{c8b522e0-5cf3-11ce-ade5-00aa0044773d}");
		public static readonly Guid TraceColumns = new Guid("{a07ccd18-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid TraceDefinitionProviderInfo = new Guid("{A07CCD1B-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid TraceEventCategories = new Guid("{a07ccd19-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid Traces = new Guid("{A07CCD1A-8148-11D0-87BB-00C04FC33942}");
		public static readonly Guid Transactions = new Guid("{a07ccd28-8148-11d0-87bb-00c04fc33942}");
		public static readonly Guid XmlaProperties = new Guid("{4b40adfb-8b09-4758-97bb-636e8ae97bcf}");
		public static readonly Guid XmlMetadata = new Guid("{3444B255-171E-4cb9-AD98-19E57888A75F}");
		private AdomdSchemaGuid() {
		}
	}
}
