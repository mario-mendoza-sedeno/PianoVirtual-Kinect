using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.Kinect.Nui;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using KinectNui = Microsoft.Research.Kinect.Nui;

namespace KinectManipulation
{
    public class KinectUser 
    {
        public int TrackingID { get; set; }

        public bool Tracked { get; set; }

        public Vector HandLeft { get; set; }

        public Vector HandRight { get; set; }

        public KinectUser(int trackingID) {
            TrackingID = trackingID;
        }
    }

    public static class Kinect
    {
        public static Action<BitmapSource> BitmapSourceActionForVideo { get; set; }

        public static Action<BitmapSource> BitmapSourceActionForDepth { get; set; }

        public static Action<List<KinectUser>> TrackedUsersActionForSkeleton { get; set; }

        public static Action<KinectUser> AddKinectUserAction { get; set; }

        public static Action<KinectUser> RemoveKinectUserAction { get; set; }

        public static Dictionary<int, KinectUser> KinectUsers { get; set; }

        public static RuntimeOptions RuntimeOptions { get; set; }

        static byte[] depthFrame32 = new byte[320 * 240 * 4];
        static Runtime runtime;

        const int RED_IDX = 2;
        const int GREEN_IDX = 1;
        const int BLUE_IDX = 0;

        const int NinetySix = 96;
        // constants for ConvertDepthFrame
        const int hexseven = 0x07;
        const int TwoFiftyFive = 255;
        const int RealDepth = 0x0fff;
        const int Five = 5;
        const int Three = 3;
        const int One = 1;
        const int Zero = 0;
        const int Two = 2;
        const int Four = 4;
        const int Six = 6;
        const int Seven = 7;

        public static void KinectDetection() {

            Runtime.Kinects.StatusChanged += new EventHandler<StatusChangedEventArgs>(delegate(object sender, StatusChangedEventArgs e) {
                switch (e.Status) 
                { 
                    case KinectStatus.Connected :
                        runtime = e.KinectRuntime;
                        Initialize();
                        break;
                    case KinectStatus.Disconnected:
                        runtime = e.KinectRuntime;
                        break;
                    default:
                        break;
                }
            });
            if (Runtime.Kinects.Count > 0) {
                foreach (Runtime _runtime in Runtime.Kinects)
                {
                    if (_runtime.Status == KinectStatus.Connected)
                    {
                        if (runtime == null)
                        {
                            runtime = _runtime;
                            Initialize();
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Skeletal tracking only works on one Kinect right now.  So return false if it is already in use.
        /// </summary>
        private static bool IsSkeletalViewerAvailable
        {
            get { return KinectNui.Runtime.Kinects.All(k => k.SkeletonEngine == null); }
        }

        private static void Initialize()
        {
            RuntimeOptions = IsSkeletalViewerAvailable ?
                                     RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking | RuntimeOptions.UseColor
                                     : RuntimeOptions.UseDepth | RuntimeOptions.UseColor;
            runtime.Initialize(RuntimeOptions);

            runtime.SkeletonEngine.TransformSmooth = true;
            runtime.SkeletonEngine.SmoothParameters = new TransformSmoothParameters()
            {
                Smoothing = .75f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f,
                Correction = 0.0f,
                Prediction = 0.0f,
            };

            runtime.VideoStream.Open(ImageStreamType.Video, Two, ImageResolution.Resolution640x480, ImageType.Color);
            runtime.DepthStream.Open(ImageStreamType.Depth, Two, ImageResolution.Resolution320x240, RuntimeOptions.HasFlag(RuntimeOptions.UseDepthAndPlayerIndex) || RuntimeOptions.HasFlag(RuntimeOptions.UseSkeletalTracking) ? ImageType.DepthAndPlayerIndex : ImageType.Depth);

            

            runtime.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(delegate(object sender, ImageFrameReadyEventArgs e)
            {
                if(BitmapSourceActionForVideo != null) {
                    PlanarImage videoImage = e.ImageFrame.Image;
                    BitmapSourceActionForVideo.Invoke(BitmapSource.Create(videoImage.Width, videoImage.Height, NinetySix, NinetySix, PixelFormats.Bgr32, null,
                        videoImage.Bits, videoImage.Width * videoImage.BytesPerPixel));
                }
            });

            runtime.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(delegate(object sender, ImageFrameReadyEventArgs e)
            {
                if (BitmapSourceActionForDepth != null) 
                {
                    PlanarImage depthImage = e.ImageFrame.Image;
                    byte[] convertedBits = GenerateColoredBytes(e.ImageFrame);
                    BitmapSourceActionForDepth.Invoke(BitmapSource.Create(depthImage.Width, depthImage.Height, NinetySix, NinetySix, PixelFormats.Bgr32, null,
                        convertedBits, depthImage.Width * Four));
                }
            });

            KinectUsers = new Dictionary<int, KinectUser>();
            runtime.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(delegate(object sender, SkeletonFrameReadyEventArgs e)
            {
                if (TrackedUsersActionForSkeleton != null) 
                {
                    IEnumerable<SkeletonData> skeletons = e.SkeletonFrame.Skeletons
                        .Where(s => s.TrackingState == SkeletonTrackingState.Tracked);

                    //resetTrackedFlag
                    foreach (KinectUser kinectUser in KinectUsers.Values)
                    {
                        kinectUser.Tracked = false;
                    }

                    //track users
                    foreach (SkeletonData skeleton in skeletons)
                    {
                        KinectUser kinectUser;
                        if (!KinectUsers.ContainsKey(skeleton.TrackingID))
                        {
                            kinectUser = new KinectUser(skeleton.TrackingID);
                            kinectUser.HandLeft = skeleton.Joints[JointID.HandLeft].Position;
                            kinectUser.HandRight = skeleton.Joints[JointID.HandRight].Position;
                            kinectUser.Tracked = true;
                            KinectUsers.Add(kinectUser.TrackingID, kinectUser);

                            //invoke new user action
                            if (AddKinectUserAction != null) {
                                AddKinectUserAction.Invoke(kinectUser);
                            }
                        }
                        else {
                            kinectUser = KinectUsers[skeleton.TrackingID];
                            kinectUser.HandLeft = skeleton.Joints[JointID.HandLeft].Position;
                            kinectUser.HandRight = skeleton.Joints[JointID.HandRight].Position;
                            kinectUser.Tracked = true;
                        }
                    }
                    //remove untracked users
                    foreach (KinectUser kinectUser in KinectUsers.Values.Where(user => (!user.Tracked)).ToList())
                    {
                        KinectUsers.Remove(kinectUser.TrackingID);

                        //invoke remove user action
                        if (RemoveKinectUserAction != null)
                        {
                            RemoveKinectUserAction.Invoke(kinectUser);
                        }
                        
                    }
                    TrackedUsersActionForSkeleton.Invoke(KinectUsers.Values.ToList());
                }
            });
        }

        public static bool IsKinectStarted
        {
            get { return runtime != null; }
        }

        public static int ElevationAngle
        {
            get
            {
                int elevationAngle = 0;
                if (IsKinectStarted)
                {
                    elevationAngle = runtime.NuiCamera.ElevationAngle;
                }
                return elevationAngle;
            }
            set
            {
                if (IsKinectStarted)
                {
                    runtime.NuiCamera.ElevationAngle = value;
                }
            }
        }

        private static byte[] GenerateColoredBytes(ImageFrame imageFrame)
        {
            int height = imageFrame.Image.Height;
            int width = imageFrame.Image.Width;

            //profunidad de datos por cada pixel
            Byte[] depthData = imageFrame.Image.Bits;

            //ColorFrame contiene información de todos los pixeles en la imagen
            //Height x Width x 4  (Red, Green, Blue, empty byte)
            Byte[] colorFrame = new byte[imageFrame.Image.Height * imageFrame.Image.Width * 4];

            //Bgr32  -blue, green red, emptybyte
            //Bgra32 - blue, green, red, transparency
            // you must set transparency for Bgra32 as .NET Default a byte to 0 = fully transparency

            //hardcore location to Blue, Green, Red (BGR) Index position
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;


            var depthIndex = 0;
            for (var y = 0; y < height; y++)
            {
                var heightOffset = y * width;
                for (var x = 0; x < width; x++)
                {

                    var index = ((width - x - 1) + heightOffset) * 4;
                    var distance = GetDistanceWithPlayerIndex(depthData[depthIndex], depthData[depthIndex + 1]);
                    depthIndex += 2;
                    if (distance <= 400)
                    {
                        // we are very close
                        colorFrame[index + BlueIndex] = 255;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 0;
                    }
                    if (distance > 400 && distance <= 800)
                    {
                        colorFrame[index + BlueIndex] = 255;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 0;
                    }
                    if (distance > 800 && distance <= 1200)
                    {
                        colorFrame[index + BlueIndex] = 255;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 255;
                    }
                    if (distance > 1200 && distance <= 1600)
                    {
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 255;
                    }
                    if (distance > 1600 && distance <= 2000)
                    {
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 255;
                    }

                    if (distance > 2000 && distance <= 2400)
                    {
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 0;
                    }

                    else if (distance > 2400 && distance <= 2800)
                    {
                        colorFrame[index + BlueIndex] = 231;
                        colorFrame[index + GreenIndex] = 231;
                        colorFrame[index + RedIndex] = 24;
                    }
                    else if (distance > 2800 && distance <= 3200)
                    {
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 128;
                    }

                    else if (distance > 3200 && distance <= 3600)
                    {
                        colorFrame[index + BlueIndex] = 200;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 100;
                    }



                    else if (distance > 3600)
                    {
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 0;
                    }


                }
            }


            return colorFrame;
        }

        private static int GetDistanceWithPlayerIndex(byte firstFrame, byte secondFrame)
        {
            //offset by 3 in first byte to get value after player index
            int distance = (int)(firstFrame >> 3 | secondFrame << 5);


            return distance;

        }

        private static int GetPlayerIndex(byte firstFrame)
        {
            // returns  0 = no players;    1 = primer player;     2 = second player
            // bitwise & on firstFrame
            return (int)firstFrame & 7;

        }

        const float MaxDepthDistance = 4000;  //maximun value returned
        const float MinDepthDistance = 850;   // minimun distance returned
        const float MaxDepthDistanceOffeset = MaxDepthDistance - MinDepthDistance;


        public static byte CalculateIntensityFromDepth(int distance)
        {
            //fórmula para calcular la intensidad monocromática para el histograma
            return (byte)(255 - (255 * Math.Max(distance - MinDepthDistance, 0) / (MaxDepthDistanceOffeset)));

        }

        static byte[] ConvertDepthFrame(byte[] depthFrame16)
        {
            bool hasPlayerData = RuntimeOptions.HasFlag(RuntimeOptions.UseDepthAndPlayerIndex);
            for (int i16 = 0, i32 = 0; i16 < depthFrame16.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
            {
                int player = hasPlayerData ? depthFrame16[i16] & 0x07 : -1;
                int realDepth = 0;

                if (hasPlayerData)
                {
                    realDepth = (depthFrame16[i16 + 1] << 5) | (depthFrame16[i16] >> 3);
                }
                else
                {
                    realDepth = (depthFrame16[i16 + 1] << 8) | (depthFrame16[i16]);
                }

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(255 - (255 * realDepth / 0x0fff));

                depthFrame32[i32 + RED_IDX] = 0;
                depthFrame32[i32 + GREEN_IDX] = 0;
                depthFrame32[i32 + BLUE_IDX] = 0;

                // choose different display colors based on player
                switch (player)
                {
                    case -1:
                    case 0:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 2);
                        break;
                    case 1:
                        depthFrame32[i32 + RED_IDX] = intensity;
                        break;
                    case 2:
                        depthFrame32[i32 + GREEN_IDX] = intensity;
                        break;
                    case 3:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 4:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity / 4);
                        break;
                    case 5:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 4);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 6:
                        depthFrame32[i32 + RED_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(intensity / 2);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(intensity);
                        break;
                    case 7:
                        depthFrame32[i32 + RED_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + GREEN_IDX] = (byte)(255 - intensity);
                        depthFrame32[i32 + BLUE_IDX] = (byte)(255 - intensity);
                        break;
                }
            }
            return depthFrame32;
        }

        public static void Uninitialize()
        {
            if (IsKinectStarted)
            {
                runtime.Uninitialize();
            }
        }

    }

}

