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
using System.Collections;
using DevExpress.Data.PivotGrid;
using System.IO;
using DevExpress.Utils;
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridFileDataSource : PivotGridNativeDataSource {
		public PivotGridFileDataSource(PivotGridData pivotGridData)
			: base(pivotGridData) {
		}
		public override void SetListSource(IList value) {
			if(ListSource == value) return;
			PivotFileDataSource fileDataSource = value as PivotFileDataSource;
			if(fileDataSource == null)
				throw new ArgumentException(GetType().Name + " supports only PivotFileDataSource lists");
			base.SetListSource(value);
			if(fileDataSource != null) {
				if(fileDataSource.CreatedFromStream)
					LoadFileDataSource(fileDataSource.Stream);
				else
					using(FileStream stream = new FileStream(fileDataSource.FileName, FileMode.Open, FileAccess.Read)) {
						LoadFileDataSource(stream);
					}
			}
		}
		protected virtual void LoadFileDataSource(Stream stream) {
			PivotFileDataSourceHelper.SeekToFields(stream);
			BinaryReader reader = new BinaryReader(stream);
			long endLayoutPosition = reader.ReadInt64();
			int layoutLength = (int)(endLayoutPosition - stream.Position);
			using(MemoryStream layoutStream = new MemoryStream(layoutLength)) {
				layoutStream.SetLength(layoutLength);
				stream.Read(layoutStream.GetBuffer(), 0, layoutLength);
				GridData.LoadFieldsFromStreamCore(layoutStream);
			}
			stream.Position = endLayoutPosition;
			GridData.LoadCollapsedStateFromStream(stream);
			RaiseLayoutChanged();
		}  
	}
}
