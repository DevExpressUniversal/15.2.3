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
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ReportServer.Printing.Services;
using System.ComponentModel;
namespace DevExpress.ReportServer.ServiceModel.Native.RemoteOperations {
	public class RequestPagesOperation : RemoteScalarOperation<DeserializedPrintingSystem> {
		readonly int[] pageIndexes;
		const int maximumPagesPerRequest = 50;
		int indexesRequested;
		readonly DeserializedPrintingSystem ps;
		[Category(NativeSR.CatDocumentCreation)]
		public event ExceptionEventHandler RequestPagesException;
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		internal static int MaximumPagesPerRequest { get { return maximumPagesPerRequest; } }
		public RequestPagesOperation(IReportServiceClient client, DocumentId documentId, TimeSpan updateInterval, int[] pageIndexes, IBrickPagePairFactory factory)
			: base(client, updateInterval, documentId) {
			this.pageIndexes = pageIndexes;
			ps = new DeserializedPrintingSystem();
			((IServiceContainer)ps).AddService(typeof(IBrickPagePairFactory), factory);
		}
		public override bool CanStop { get { return false; } }
		public override void Start() {
			Client.GetPagesCompleted += Client_GetPagesCompleted;
			StartRequestPages();
		}
		void StartRequestPages() {
			int count = Math.Min(maximumPagesPerRequest, pageIndexes.Length - indexesRequested);
			int[] indexesToRequest = new int[count];
			Array.Copy(pageIndexes, indexesRequested, indexesToRequest, 0, count);
			indexesRequested += count;
			Client.GetPagesAsync(documentId, indexesToRequest, PageCompatibility.Prnx, instanceId);
		}
		void Client_GetPagesCompleted(object sender, ScalarOperationCompletedEventArgs<byte[]> e) {
			if(!IsSameInstanceId(e.UserState))
				return;
			if(e.Error != null) {
				UnsubscribeClientEvents();
				if (RequestPagesException != null)
					RequestPagesException(this, new ExceptionEventArgs(e.Error));
				return;
			}
			Deserialize(ps.Document, e.Result);
			if(pageIndexes.Length > indexesRequested)
				StartRequestPages();
			else {
				UnsubscribeClientEvents();
				ScalarOperationCompletedEventArgs<DeserializedPrintingSystem> result = new ScalarOperationCompletedEventArgs<DeserializedPrintingSystem>(ps, e.Error, e.Cancelled, e.UserState);
				RaiseOperationCompleted(result);
			}
		}
		static void Deserialize(Document document, byte[] serializedData) {
			using(Stream stream = new MemoryStream(serializedData)) {
				document.Deserialize(stream, new PrintingSystemXmlSerializer());
			}
		}
		public override void Stop() {
			throw new NotImplementedException();
		}
		protected override void UnsubscribeClientEvents() {
			Client.GetPagesCompleted -= Client_GetPagesCompleted;
		}
	}
}
