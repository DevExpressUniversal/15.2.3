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

using DevExpress.Data.Camera.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
namespace DevExpress.Data.Camera {
	[SecuritySafeCritical]
	public class CameraDeviceBase : IDisposable {
		IGraphBuilder graph;
		ISampleGrabber grabber;
		ICaptureGraphBuilder2 captureGraphBuilder;
		IBaseFilter sourceFilter, grabberFilter;
		IMediaControl mediaControl;
		IMediaEventEx mediaEventEx;
		CaptureGrabber capGrabber;
		AMMediaType mediaType;
		IntPtr framePtr;
		IntPtr capturedFramePtr;
		FrameRateCounter rateCounter = new FrameRateCounter();
		string deviceMoniker;
		string name;
		ICameraDeviceClient client;
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetClient(ICameraDeviceClient client) {
			this.client = client;
		}
		float FPS {
			get { return rateCounter.FrameRate; }
		}
		public CameraDeviceBase(CameraDeviceInfo deviceInfo) {
			this.deviceMoniker = deviceInfo.MonikerString;
			this.name = deviceInfo.Name;
		}
		public string DeviceMoniker {
			get { return deviceMoniker; }
		}
		public string Name {
			get { return name; }
		}
		public bool IsBusy { get; private set; }
		bool isRunning;
		public bool IsRunning {
			get { return isRunning; }
			private set { isRunning = value; }
		}
		public void Start() {
			if(IsRunning) return;
			capGrabber = new CaptureGrabber(this);
			SubscribeOnCaptureGrabber(capGrabber);
			this.IsBusy = false;
			RunDevice();
		}
		void SubscribeOnCaptureGrabber(CaptureGrabber cg) {
			cg.NewFrame += cg_NewFrame;
			cg.SizeChanged += cg_SizeChanged;
		}
		void UnSubscribeOnCaptureGrabber(CaptureGrabber cg) {
			cg.NewFrame -= cg_NewFrame;
			cg.SizeChanged -= cg_SizeChanged;
		}
		void cg_SizeChanged(object sender, EventArgs e) {
			OnGrabberSizeChanged();
		}
		void cg_NewFrame(object sender, EventArgs e) {
			OnNewFrame();
		}
		protected virtual void OnNewFrame() {
			if(client != null) {
				client.OnNewFrame();
			}
			rateCounter.Inc();
		}
		public void Dispose() {
			this.Stop();
			this.SetClient(null);
			rateCounter.Free();
			rateCounter = null;
		}
		public Bitmap TakeSnapshot() {
			if(capGrabber != null && capGrabber.FramePtr != IntPtr.Zero) {
				int bytesCount = (int)(capGrabber.Width * capGrabber.Height * BitsPerPixel / 8);
				CameraImport.CopyMemory(capturedFramePtr, capGrabber.FramePtr, bytesCount);
				Bitmap bmp = new Bitmap(capGrabber.Width, capGrabber.Height, capGrabber.Width * BitsPerPixel / 8, CurrentPixelFormat, capturedFramePtr);
				return new Bitmap(bmp);
			}
			return null;
		}
		void OnGrabberSizeChanged() {
			Marshal.FreeHGlobal(capturedFramePtr);
			capturedFramePtr = IntPtr.Zero;
			if(capGrabber.Width != default(int) && capGrabber.Height != default(int)) {
				uint pcount = (uint)(this.capGrabber.Width * this.capGrabber.Height * System.Windows.Media.PixelFormats.Bgr32.BitsPerPixel / 8);
				IntPtr section = CameraImport.CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, pcount, null);
				framePtr = CameraImport.MapViewOfFile(section, 0xF001F, 0, 0, pcount);
				this.capGrabber.FramePtr = this.framePtr;
				this.capturedFramePtr = Marshal.AllocHGlobal((int)pcount);
				CreateFrameCore(section, this.capGrabber.Width, this.capGrabber.Height, framePtr);
			}
		}
		protected virtual void CreateFrameCore(IntPtr section, int width, int height, IntPtr stride) { }
		protected internal int BitsPerPixel {
			get { return Image.GetPixelFormatSize(CurrentPixelFormat); }
		}
		protected PixelFormat CurrentPixelFormat {
			get { return PixelFormat.Format32bppRgb; }
		}
		void SubscribeOnMediaEventNotifying(IGraphBuilder iGraphBuilder) {
			if(iGraphBuilder == null) 
				return;
			this.mediaEventEx = iGraphBuilder as IMediaEventEx;
			if(this.mediaEventEx != null && this.client != null) {
				this.mediaEventEx.SetNotifyWindow(this.client.Handle, CameraImport.WM_GRAPHNOTIFY, IntPtr.Zero);
			}
		}
		void UnsubscribeFromMediaEventNotifying() {
			if(this.mediaEventEx != null)
				this.mediaEventEx.SetNotifyWindow(IntPtr.Zero, CameraImport.WM_GRAPHNOTIFY, IntPtr.Zero);
		}
		IBaseFilter CreateSourceFilter() {
			return CameraDeviceInfo.CreateFilter(DeviceMoniker);
		}
		private void RunDevice() {
			try {
				graph = Activator.CreateInstance(Type.GetTypeFromCLSID(CameraImport.FilterGraph)) as IGraphBuilder;
				SubscribeOnMediaEventNotifying(graph);
				sourceFilter = CreateSourceFilter();
				if(sourceFilter == null) return;
				grabber = Activator.CreateInstance(Type.GetTypeFromCLSID(CameraImport.SampleGrabber)) as ISampleGrabber;
				grabberFilter = grabber as IBaseFilter;
				graph.AddFilter(sourceFilter, "Camera");
				graph.AddFilter(grabberFilter, "Output");
				captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
				captureGraphBuilder.SetFiltergraph(graph);
				mediaType = new AMMediaType();
				mediaType.MajorType = MediaTypes.Video;
				mediaType.SubType = MediaSubTypes.RGB32;
				grabber.SetMediaType(mediaType);
				IAMStreamConfig streamCfg = (IAMStreamConfig)this.sourceFilter.GetPin(PinDirection.Output, 0);
				ConfigureStream(streamCfg);
				if(graph.Connect(this.sourceFilter.GetPin(PinDirection.Output, 0), grabberFilter.GetPin(PinDirection.Input, 0)) >= 0) {
					if(grabber.GetConnectedMediaType(mediaType) == 0) {
						VideoInfoHeader header = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.FormatPtr, typeof(VideoInfoHeader));
						capGrabber.Width = header.BmiHeader.Width;
						capGrabber.Height = header.BmiHeader.Height;
					}
				}
				graph.Render(this.grabberFilter.GetPin(PinDirection.Output, 0));
				grabber.SetBufferSamples(false);
				grabber.SetOneShot(false);
				grabber.SetCallback(this.capGrabber, 1);
				IVideoWindow wnd = (IVideoWindow)graph;
				wnd.put_AutoShow(false);
				wnd = null;
				mediaControl = (IMediaControl)graph;
				int runResult = mediaControl.Run();
				Marshal.ThrowExceptionForHR(runResult);
				rateCounter.Start();
				IsRunning = true;
			}
			catch(Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				Stop(true);
				IsBusy = true;
				OnDeviceLost();
			}
		}
		private void ConfigureStream(IAMStreamConfig streamCfg) {
			if(streamCfg != null) {
				AMMediaType streamMediaType = new AMMediaType();
				streamCfg.GetFormat(out streamMediaType);
				VideoInfoHeader header = (VideoInfoHeader)Marshal.PtrToStructure(streamMediaType.FormatPtr, typeof(VideoInfoHeader));
				header.AverageTimePerFrame = (long)(10000000 / 30);
				header.BmiHeader.Height = PrefferedResolution.Height;
				header.BmiHeader.Width = PrefferedResolution.Width;
				Marshal.StructureToPtr(header, streamMediaType.FormatPtr, false);
				streamCfg.SetFormat(streamMediaType);
			}
		}
		Size PrefferedResolution { get; set; }
		public Size Resolution {
			get { return GetCurrentResolution(); }
			set { SetResolution(value); }
		}
		Size GetCurrentResolution() {
			if(IsRunning) {
				return new Size(this.capGrabber.Width, this.capGrabber.Height);
			}
			var resolutions = GetAvailiableResolutions();
			if(resolutions.Count > 0)
				return resolutions[0];
			return PrefferedResolution;
		}
		void SetResolution(Size resolution) {
			if(IsRunning) {
				Stop();
				PrefferedResolution = resolution;
				Start();
				if(this.client != null) this.client.OnResolutionChanged();
			}
			else {
				PrefferedResolution = resolution;
			}
		}
		public List<Size> GetAvailiableResolutions() { 
			var result = new List<Size>();
			if(IsRunning) {
				GetAvailiableResolutions(this.sourceFilter, result);
			}
			else {
				var source = CreateSourceFilter();
				if(source == null) return result;
				GetAvailiableResolutions(source, result);
				Marshal.ReleaseComObject(source);
			}
			return result;
		}
		void GetAvailiableResolutions(IBaseFilter source, List<Size> result) {
			if(source == null) 
				return;
			var streamCfg = (IAMStreamConfig)source.GetPin(PinDirection.Output, 0);
			if(streamCfg == null) 
				return;
			var streamMediaType = new AMMediaType();
			streamCfg.GetFormat(out streamMediaType);
			int piCount, piSize;
			streamCfg.GetNumberOfCapabilities(out piCount, out piSize);
			IntPtr taskMemPtr = Marshal.AllocCoTaskMem(piSize);
			try {
				for(int i = 0; i < piCount; i++) {
					streamCfg.GetStreamCaps(i, out streamMediaType, taskMemPtr);
					var header = (VideoInfoHeader)Marshal.PtrToStructure(streamMediaType.FormatPtr, typeof(VideoInfoHeader));
					if(header.BmiHeader.BitCount > 0) {
						var resolution = new Size(header.BmiHeader.Width, header.BmiHeader.Height);
						if(!result.Contains(resolution))
							result.Add(resolution);
					}
				}
			}
			finally {
				Marshal.FreeCoTaskMem(taskMemPtr);
			}
		}
		public void Stop() {
			Stop(false);
		}
		void Stop(bool forced) {
			if(!forced && !IsRunning)
				return;
			UnsubscribeFromMediaEventNotifying();
			IsRunning = false;
			mediaControl.StopWhenReady();
			if(capGrabber != null) {
				UnSubscribeOnCaptureGrabber(capGrabber);
				CameraImport.UnmapViewOfFile(capGrabber.FramePtr);
				capGrabber.FramePtr = IntPtr.Zero;
			}
			sourceFilter.Stop();
			Marshal.ReleaseComObject(sourceFilter);
			sourceFilter = null;
			grabberFilter.Stop();
			Marshal.ReleaseComObject(grabberFilter);
			grabberFilter = null;
			Marshal.ReleaseComObject(graph);
			graph = null;
			Marshal.ReleaseComObject(grabber);
			grabber = null;
			Marshal.ReleaseComObject(captureGraphBuilder);
			captureGraphBuilder = null;
			Marshal.ReleaseComObject(mediaControl);
			mediaControl = null;
			Marshal.ReleaseComObject(mediaEventEx);
			mediaEventEx = null;
			Marshal.FreeHGlobal(capturedFramePtr);
			capturedFramePtr = IntPtr.Zero;
			capGrabber = null;
			mediaType = null;
			rateCounter.Stop();
			FreeFrame();
		}
		protected virtual void FreeFrame() { }
		int GetVideoProcessingProperty(VideoProcAmpProperty videoProperty, bool getValue, string subPropName) {
			IAMVideoProcAmp videoProc = sourceFilter as IAMVideoProcAmp;
			if(videoProc != null) {
				VideoProcAmpFlags flgs;
				int value;
				if(getValue) {
					if(0 == videoProc.Get(videoProperty, out value, out flgs)) 
						return value;
				}
				else {
					int min, max, steppingdelta, def;
					if(0 == videoProc.GetRange(videoProperty, out min, out max, out steppingdelta, out def, out flgs)) {
						switch(subPropName){
							case "Min": return min;
							case "Max": return max;
							case "SteppingDelta": return steppingdelta;
							case "Default": return def;
							default: return 0;
						}
					}
				}
			}
			return 0;
		}
		internal int GetVideoProcessingPropertyByName(string propName, bool getValue, string subPropName) {
			var videoProperty = (VideoProcAmpProperty)Enum.Parse(typeof(VideoProcAmpProperty), propName, true);
			return GetVideoProcessingProperty(videoProperty, getValue, subPropName);
		}
		internal void SetVideoProcessingProperty(VideoProcAmpProperty videoProperty, int value) {
			var videoProc = sourceFilter as IAMVideoProcAmp;
			if(videoProc == null) return;
			videoProc.Set(videoProperty, value, VideoProcAmpFlags.Manual);
		}
		internal void SetVideoProcessingPropertyByName(string propName, int val) {
			var videoProperty = (VideoProcAmpProperty)Enum.Parse(typeof(VideoProcAmpProperty), propName, true);
			SetVideoProcessingProperty(videoProperty, val);
		}
		internal void HandleGraphEvent() {
			EventCode evCode = 0;
			IntPtr evParam1;
			IntPtr evParam2;
			while(this.mediaEventEx != null && this.mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0) {
				this.mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
				if(evCode == EventCode.DeviceLost) {
					OnDeviceLost();
				}
			}
		}
		void OnDeviceLost() {
			if(this.client != null) {
				this.client.OnDeviceLost(this);
			}
			Stop();
		}
		public virtual void WndProc(ref System.Windows.Forms.Message m) {
			if(m.Msg == CameraImport.WM_GRAPHNOTIFY) {
				HandleGraphEvent();
			}
		}
		public override bool Equals(object obj) {
			CameraDeviceBase input = obj as CameraDeviceBase;
			if(input == null) return false;
			return this.DeviceMoniker.Equals(input.DeviceMoniker);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	[SecuritySafeCritical]
	internal class CaptureGrabber : ISampleGrabberCB {
		CameraDeviceBase device;
		int imageHeight = default(int);
		int imageWidth = default(int);
		public CaptureGrabber(CameraDeviceBase device) {
			this.device = device;
		}
		public CameraDeviceBase Device { get { return this.device; } }
		public IntPtr FramePtr { get; set; }
		int stride;
		public int Width {
			get { return this.imageWidth; }
			set {
				this.imageWidth = value;
				this.stride = CalcStride();
				this.OnPropertyChanged();
			}
		}
		int CalcStride() {
			return this.Width * device.BitsPerPixel / 8;
		}
		public int Height {
			get { return this.imageHeight; }
			set {
				this.imageHeight = value;
				this.OnPropertyChanged();
			}
		}
		public int SampleCB(double sampleTime, IntPtr sample) { return 0; }
		[SecuritySafeCritical]
		public int BufferCB(double sampleTime, IntPtr buffer, int bufferLen) {
			if(this.FramePtr != IntPtr.Zero && Device.IsRunning) {
				byte[] tempArray = new byte[bufferLen];
				byte[] resultArray = new byte[bufferLen];
				Marshal.Copy(buffer, tempArray, 0, bufferLen);
				for(int i = 0; i < this.Height; i++) {
					Array.Copy(tempArray, bufferLen - this.stride * i - this.stride, resultArray, i * this.stride, this.stride);
				}
				Marshal.Copy(resultArray, 0, this.FramePtr, bufferLen);
				this.OnNewFrameArrived();
			}
			return 0;
		}
		void OnNewFrameArrived() {
			if(NewFrame != null) {
				NewFrame(this, new EventArgs());
			}
		}
		void OnPropertyChanged() {
			if(SizeChanged != null) {
				SizeChanged(this, new EventArgs());
			}
		}
		public event EventHandler NewFrame;
		public event EventHandler SizeChanged;
	}
}
