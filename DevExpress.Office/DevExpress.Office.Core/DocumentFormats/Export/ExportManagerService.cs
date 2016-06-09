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
using DevExpress.Utils;
#if !SL
#else
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Office.Internal {
	#region ExportManagerService (abstract class)
	public abstract class ExportManagerService<TFormat, TResult> : IExportManagerService<TFormat, TResult> {
		#region Fields
		readonly Dictionary<TFormat, IExporter<TFormat, TResult>> exporters;
		#endregion
		protected ExportManagerService() {
			this.exporters = new Dictionary<TFormat, IExporter<TFormat, TResult>>();
			RegisterNativeFormats();
		}
		#region Properties
		public Dictionary<TFormat, IExporter<TFormat, TResult>> Exporters { get { return exporters; } }
		#endregion
		public virtual void RegisterExporter(IExporter<TFormat, TResult> Exporter) {
			Guard.ArgumentNotNull(Exporter, "Exporter");
			Exporters.Add(Exporter.Format, Exporter);
		}
		public virtual void UnregisterExporter(IExporter<TFormat, TResult> Exporter) {
			if (Exporter == null)
				return;
			Exporters.Remove(Exporter.Format);
		}
		public void UnregisterAllExporters() {
			Exporters.Clear();
		}
		public virtual IExporter<TFormat, TResult> GetExporter(TFormat format) {
			return Exporters[format];
		}
		public virtual List<IExporter<TFormat, TResult>> GetExporters() {
			List<IExporter<TFormat, TResult>> result = new List<IExporter<TFormat, TResult>>();
			result.AddRange(Exporters.Values);
			return result;
		}
		protected internal abstract void RegisterNativeFormats();
	}
	#endregion
}
