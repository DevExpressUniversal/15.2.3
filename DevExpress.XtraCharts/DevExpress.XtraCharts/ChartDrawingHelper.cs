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
using System.Drawing;
using System.Threading;
namespace DevExpress.XtraCharts.Native {
	public delegate void DrawCompleteDelegate();
	public class ChartDrawingHelper : IDisposable {
		Chart chart;
		Thread buildGraphicsTreeThread, drawImageThread;
		ManualResetEvent exitEvent;
		AutoResetEvent startDrawImageEvent, startBuildGraphicsTreeEvent;
		Bitmap image;
		Node graphicsTree;
		Size imageSize;
		GraphicsCommand command;
		bool imageReady, graphicsTreeReady;
		bool disposed;
		PrimitivesContainer primitivesContainer;
		public bool IsImageReady { get { return imageReady; } }
		public ChartDrawingHelper(Chart chart) {
			this.chart = chart;
			buildGraphicsTreeThread = new Thread(new ThreadStart(BuildGraphicsTreeLoop));
			drawImageThread = new Thread(new ThreadStart(DrawImageLoop));
			exitEvent = new ManualResetEvent(false);
			startDrawImageEvent = new AutoResetEvent(false);
			startBuildGraphicsTreeEvent = new AutoResetEvent(false);
			image = null;
			buildGraphicsTreeThread.Name = "BuildGraphicsTree";
			buildGraphicsTreeThread.Priority = ThreadPriority.BelowNormal;
			buildGraphicsTreeThread.IsBackground = true;
			buildGraphicsTreeThread.Start();
			drawImageThread.Name = "DrawImage";
			drawImageThread.Priority = ThreadPriority.BelowNormal;
			drawImageThread.IsBackground = true;
			drawImageThread.Start();
		}
		~ChartDrawingHelper() {
			Dispose(false);
		}
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		void Dispose(bool disposing) {
			if (disposing && !disposed) {
				disposed = true;
				exitEvent.Set();
				buildGraphicsTreeThread.Join();
				drawImageThread.Join();
				DisposeGraphicsCommand();
				DisposeImage();
				startDrawImageEvent.Close();
				startBuildGraphicsTreeEvent.Close();
				exitEvent.Close();
			}
		}
		void DisposeImage() {
			if (image != null) {
				image.Dispose();
				image = null;
			}
		}
		void DisposeGraphicsCommand() {
			if (command != null) {
				command.Dispose();
				command = null;
			}
		}
		void DrawImageLoop() {
			for (; ; ) {
				try {
					WaitHandle.WaitAny(new WaitHandle[] { exitEvent, startDrawImageEvent });
					if (exitEvent.WaitOne(0, false))
						return;
					if (startBuildGraphicsTreeEvent.WaitOne(0, false)) {
						startBuildGraphicsTreeEvent.Set();
						continue;
					}
					GraphicsCommand command;
					lock (this) {
						command = this.command;
						if (command == null) {
							chart.ContainerAdapter.InvokeInvalidate();
							continue;
						}
						this.command = null;
						imageReady = false;
						DisposeImage();
					}
					try {
						image = ChartBitmapContainer3D.Draw(chart, imageSize, command);
					}
					finally {
						command.Dispose();
					}
					if (image == null)
						startDrawImageEvent.Reset();
					lock (chart) {
						if (startDrawImageEvent.WaitOne(0, false)) {
							startDrawImageEvent.Set();
							continue;
						}
						if (startBuildGraphicsTreeEvent.WaitOne(0, false)) {
							startBuildGraphicsTreeEvent.Set();
							continue;
						}
						if (image == null)
							continue;
						imageReady = true;
						if (!disposed)
							chart.ContainerAdapter.InvokeInvalidate();
					}
				}
				catch { }
			}
		}
		void BuildGraphicsTreeLoop() {
			for (; ; ) {
				try {
					WaitHandle.WaitAny(new WaitHandle[] { exitEvent, startBuildGraphicsTreeEvent });
					if (exitEvent.WaitOne(0, false))
						return;
					PrimitivesContainer container;
					lock (this) {
						graphicsTreeReady = false;
						container = primitivesContainer;
					}
					if (startBuildGraphicsTreeEvent.WaitOne(0, false)) {
						startBuildGraphicsTreeEvent.Set();
						continue;
					}
					graphicsTree = GraphicsHelper.BuildGraphicsTree(container);
					if (startBuildGraphicsTreeEvent.WaitOne(0, false)) {
						startBuildGraphicsTreeEvent.Set();
						continue;
					}
					lock (chart) {
						imageReady = false;
						startDrawImageEvent.Set();
						graphicsTreeReady = true;
						if (!disposed)
							chart.ContainerAdapter.InvokeInvalidate();
					}
				}
				catch { }
			}
		}
		void Process(PrimitivesContainer container) {
			imageReady = false;
			graphicsTreeReady = false;
			primitivesContainer = container;
			startBuildGraphicsTreeEvent.Set();
		}
		public void Invalidate() {
			lock (this)
				imageReady = false;
		}
		public void Process(Size imageSize, GraphicsCommand command) {
			lock (this) {
				imageReady = false;
				this.imageSize = imageSize;
				DisposeGraphicsCommand();
				this.command = command;
				startDrawImageEvent.Set();
			}
		}
		public Node GetGraphicsTree(PrimitivesContainer container) {
			lock (this) {
				if (container == primitivesContainer) {
					if (graphicsTreeReady)
						return graphicsTree;
				}
				else
					Process(container);
			}
			return null;
		}
		public Image GetImage() {
			lock (this)
				return image == null ? null : (Bitmap)image.Clone();
		}
	}
}
