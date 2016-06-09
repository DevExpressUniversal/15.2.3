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
	#region ImportManagerService (abstract class)
	public abstract class ImportManagerService<TFormat, TResult> : IImportManagerService<TFormat, TResult> {
		#region Fields
		readonly Dictionary<TFormat, IImporter<TFormat, TResult>> importers;
		#endregion
		protected ImportManagerService() {
			this.importers = new Dictionary<TFormat, IImporter<TFormat, TResult>>();
			RegisterNativeFormats();
		}
		#region Properties
		public Dictionary<TFormat, IImporter<TFormat, TResult>> Importers { get { return importers; } }
		#endregion
		public virtual void RegisterImporter(IImporter<TFormat, TResult> importer) {
			Guard.ArgumentNotNull(importer, "importer");
			importers.Add(importer.Format, importer);
		}
		public virtual void UnregisterImporter(IImporter<TFormat, TResult> importer) {
			if (importer == null)
				return;
			importers.Remove(importer.Format);
		}
		public void UnregisterAllImporters() {
			importers.Clear();
		}
		public virtual IImporter<TFormat, TResult> GetImporter(TFormat format) {
			IImporter<TFormat, TResult> result;
			if (importers.TryGetValue(format, out result))
				return result;
			else
				return null;
		}
		public virtual List<IImporter<TFormat, TResult>> GetImporters() {
			List<IImporter<TFormat, TResult>> result = new List<IImporter<TFormat, TResult>>();
			result.AddRange(importers.Values);
			return result;
		}
		protected internal abstract void RegisterNativeFormats();
	}
	#endregion
}
