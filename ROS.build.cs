//#define USE_EXPLICIT_PATHS_FOR_LINKING //use complete file name and full lib<something>.so links instead of relative/auto ones
#define USE_COMPLETE_STATIC_PATHS_FOR_LINKING

// Fill out your copyright notice in the Description page of Project Se ttings.
//NOTE: You need androidlibs cloned and ANDROIDLIBS_ROOT and ANDROID_ROS_BOOST_LOCATION to whever that is
using UnrealBuildTool;
using System.IO;
using System;


public class ROS : ModuleRules
{
    bool bUseGstreamer = true;
    
    private string ModulePath
    {
        get { return ModuleDirectory; }
    }

    public void addPreproc(string ros_preproc)
    {
        foreach (string s in ros_preproc.Split(';'))
        {
            Console.WriteLine("DEFINE:" + s);
            Definitions.Add(s);

        }
    }

    private string ThirdPartyPath
    {
        get { return Path.GetFullPath(Path.Combine(ModulePath, "../../ThirdParty/")); }
    }

    public void includeAdd(string env)
    {
        var items = Environment.GetEnvironmentVariable(env);
        foreach (string s in items.Split(';'))
        {
            Console.WriteLine("INCLUDE:" + s);
            PublicIncludePaths.Add(s);

        }

        

    }

    public enum LinkTypes { Static, Dynamic};

    public void linkLib(string [] libNames, LinkTypes linkType, bool explicitPathsForLinking = false)
    {

        string tmp;
        
        foreach (string l in libNames)
        {

            if (explicitPathsForLinking)
            {
                if (linkType == LinkTypes.Static)
                {
                    tmp = "lib" + l + ".a";
                    
                }
                else
                {
                    tmp = "lib" + l + ".so";
                }

                tmp = ModuleDirectory + "/Lib/Android/ARMv7/" + tmp;
            }
            else
                tmp = l;

            PublicAdditionalLibraries.Add(tmp);

            if(linkType==LinkTypes.Static)
                Console.WriteLine("Library .a linked:{0}", l);
            else
                Console.WriteLine("Library .so linked:{0}", l);

        }
    }

    //takes raw path and complete lib filename and links with that, ie, c:\somepath\somelib.a
    public void linkCompletePathLib(string[] libNames, LinkTypes linkType)
    {

        string tmp;

        foreach (string l in libNames)
        {

            PublicAdditionalLibraries.Add(l);

            if (linkType == LinkTypes.Static)
                Console.WriteLine("Library(Complete Path) .a linked:{0}", l);
            else
                Console.WriteLine("Library(Complete Path) .so linked:{0}", l);

        }
    }



    public void includeLib(string env, string prefix = null)
    {
        var items = Environment.GetEnvironmentVariable(env);
        string slib;

        if (prefix != null)
        {//TODO:make for non windows case
            if (prefix.PadRight(1) != "\\")
                prefix += '\\';
        }

        foreach (string s in items.Split(';'))
        {


            slib = prefix + s;
            Console.WriteLine("LIB INCLUDE:" + slib);
            PublicAdditionalLibraries.Add(slib);

        }
    }


    public ROS(TargetInfo Target)
    {
      
		Type = ModuleType.External;

        Console.WriteLine("^...iterating through Definitions...");

        foreach(string s in Definitions)
        {
            Console.WriteLine("-->"+s);
        }

		
		if (Target.Platform == UnrealTargetPlatform.Android)
		{
#region SLATE
            // Uncomment if you are using Slate UI  ANDROIDLIBS_ROOT
            // PrivateDependencyModuleNames.AddRange(new string[] { "Slate", "SlateCore" });

            // Uncomment if you are using online features
            // PrivateDependencyModuleNames.Add("OnlineSubsystem");
            // if ((Target.Platform == UnrealTargetPlatform.Win32) || (Target.Platform == UnrealTargetPlatform.Win64))
            // {
            //		if (UEBuildConfiguration.bCompileSteamOSS == true)
            //		{
            //			DynamicallyLoadedModuleNames.Add("OnlineSubsystemSteam");
            //		}
            // }
#endregion
            PrivateIncludePaths.AddRange(
					new string[] {

								"../../../../Source/Runtime/Renderer/Private",
								"../../../../Source/Runtime/Launch/Private"
						}
					);


		}

        bUseRTTI = true; //oh so very important...lets boost dynamic cast return horror to the client. /GR
        UEBuildConfiguration.bForceEnableExceptions = true;

        //BOOST_REGEX_NO_EXTERNAL_TEMPLATES

        var ros_preproc = "OPENCV;_NO_FTDI;GREG1;BOOST_LIB_DIAGNOSTIC;_SCL_SECURE_NO_WARNINGS";//;BOOST_TYPE_INDEX_FORCE_NO_RTTI_COMPATIBILITY;BOOST_NO_RTTI;BOOST_NO_TYPEID

        if (bUseGstreamer)
        {
            Console.WriteLine("USING GSTREAMER...");
            ros_preproc += ";USE_GSTREAMER";
        }

        addPreproc(ros_preproc);

		
        if (Target.Platform == UnrealTargetPlatform.Android || Target.Platform == UnrealTargetPlatform.Win64)
        {

			PublicIncludePaths.Add(Environment.GetEnvironmentVariable("BOOST_162_INCLUDE")); 
            includeAdd("ROS_JADE_INCLUDE_PATHS");

		    

        }

		
		   if (Target.Platform == UnrealTargetPlatform.Win64)
        {
			UEBuildConfiguration.bForceEnableExceptions = true;
			Console.WriteLine("^In Win64 Build/Single DLL ROSWindowsJade.dll");
			
			PublicLibraryPaths.Add(Environment.GetEnvironmentVariable("BOOST_162_64_LIB")); //boost will automatically bring in static libs. 
        
			string lib=ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSWindowsJade.lib";
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSWindowsJade.lib");
            Console.WriteLine(lib);
            string fname = Path.Combine(ModuleDirectory + @"\bin\Windows\x64\ROSWindowsJade.dll");
            PublicDelayLoadDLLs.Add(fname);
            RuntimeDependencies.Add(new RuntimeDependency(fname));

            if (bUseGstreamer)
            {

                //include paths for GSTREAMER
                var env = Environment.GetEnvironmentVariable("GSTREAMER_WINDOWS");

                Console.WriteLine("GSTREAMER WINDOWS ENVAR IS:{0}", env);

                var gstreamerIncludes = new string[]
                {
                    @"\include\gstreamer-1.0",
                    @"\include\glib-2.0",
                    @"\lib\glib-2.0\include"
                };

                foreach (string l in gstreamerIncludes)
                {
                    PublicIncludePaths.Add(env + l);
                    Console.WriteLine("Adding gstreamerpaths:{0}",env + l);

                    if (!Directory.Exists(env + l))
                    {
                        Console.WriteLine("Whoa...this path doesn't exist, lets stop this now...");
                   //     throw new System.Exception("bad include");
                    }
                }

                var gstreamerDllLinks = new string[]
                {
                    @"\Lib\Windows\x64\Rel-64-15\gstreamer-1.0.lib",
                    @"\Lib\Windows\x64\Rel-64-15\gobject-2.0.lib",
                    @"\Lib\Windows\x64\Rel-64-15\glib-2.0.lib"
                };

                string tmp;
                foreach(string l in gstreamerDllLinks)
                {
                    tmp = ModuleDirectory + l;
                    PublicAdditionalLibraries.Add(tmp);
                    Console.WriteLine("Dll Link added:{0}", tmp);

                    if (!File.Exists(tmp))
                    {
                        Console.WriteLine("Whoa...this library doesn't exist, lets stop this now...");
                        throw new System.Exception("bad lib");
                    }


                }
                //link libraries for GSTREAMER
                
                //dependent DLLS so we can package this when the time comes (have run windepends on these and should be complete group)
                var gstreamerLibs = new string[] {
                                    "/libffi-6.dll",
                                    "/libglib-2.0-0.dll",
                                    "/libgmodule-2.0-0.dll",
                                    "/libgstreamer-1.0-0.dll",
                                    "/libintl-8.dll",
                                    "/libwinpthread-1.dll"
                                        };

                foreach (string l in gstreamerLibs)
                {
                    RuntimeDependencies.Add(new RuntimeDependency(ModuleDirectory+ l));
                }
            }


        }

        if (Target.Platform == UnrealTargetPlatform.Android)
        {
			Console.WriteLine("^^^^Android ROS section started.........");

			
			string ModDir = UEBuildConfiguration.UEThirdPartySourceDirectory + "ROS/";
			
			Console.WriteLine("^Make sure ANDROIDLIBS_ROOT and EPIC_INSTALL are present as EnVars");

            if (bUseGstreamer)
            {

                //include paths for GSTREAMER
                var env = Environment.GetEnvironmentVariable("GSTREAMER_ANDROID");

                Console.WriteLine("GSTREAMER ANDROID ENVAR IS:{0}", env); //usually G:\gstreamer-1.0-android-arm-1.11.0

                var gstreamerIncludes = new string[]
                {
                    @"/include",
                    @"/include/glib-2.0",
                    @"/lib/glib-2.0/include",
                    @"/include/gstreamer-1.0"


                };

                foreach (string l in gstreamerIncludes)
                {
                    PublicIncludePaths.Add(env + l);
                    Console.WriteLine("Adding gstreamerpaths:{0}", env + l);

                    if (!Directory.Exists(env + l))
                    {
                        Console.WriteLine("Whoa...this path doesn't exist, lets stop this now...");
                        throw new System.Exception("bad include");
                    }
                }


             




                }



                //apparently you put in every type and UBT will pick the correct one automatically...new to me!
                //I have BOOST static libs in here as well so that they, also, might be slurped up as needed
                //so there is libROSJadeAndroid as well as the boost.a files in these directories
            PublicLibraryPaths.Add(ModuleDirectory+ "/Lib/Android/ARMv7");
            PublicLibraryPaths.Add(ModuleDirectory+ "/Lib/Android/x86");
			PublicLibraryPaths.Add(ModuleDirectory+ "/Lib/Android/ARM64");
			PublicLibraryPaths.Add(ModuleDirectory+"/Lib/Android/x64");

			Console.WriteLine("^DEBUG ROS: "+ModuleDirectory+"---"+ "/Lib/Android/ARMv7");
			
			 
			string epic_install = Environment.GetEnvironmentVariable("EPIC_INSTALL");
			string epic_android_path=Path.Combine(epic_install,@"Engine\Source\Runtime\Launch\Public\Android\");
			
			PublicIncludePaths.Add(epic_android_path); //in order to get android header files for unreal
			
            string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, BuildConfiguration.RelativeEnginePath);
            AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", Path.Combine(PluginPath, "GameActivityInsert.xml"))); //../../../../repos/UDPUnreal\Source\UDPSendReceive\GregAndroidTest1.xml
            Console.WriteLine("^Path {0}:", Path.Combine(PluginPath, "GameActivityInsert.xml"));

			//need to figure out how to know WHICH libraries to bring in.
			PublicAdditionalLibraries.Add("ROSJadeAndroid");

            //G:\NVPACK\android-ndk-r12b\toolchains\arm-linux-androideabi-4.9\prebuilt\windows-x86_64\lib\gcc\arm-linux-androideabi\4.9.x\armv7-a\libgcc.a
            //some wierd UQI error (http://stackoverflow.com/questions/12174247/android-ndk-8b-cannot-load-library)
            // dlopen failed: cannot locate symbol "__gnu_thumb1_case_uqi" referenced by "libpng16.so"...
           // PublicAdditionalLibraries.Add(@"G:\NVPACK\android-ndk-r12b\toolchains\arm-linux-androideabi-4.9\prebuilt\windows-x86_64\lib\gcc\arm-linux-androideabi\4.9.x\armv7-a\libgcc.a");

            if(bUseGstreamer)
            {

                PublicLibraryPaths.Add(ModuleDirectory + "/Lib/Android/ARMv7/gstreamer_runtime/gstreamer-1.0");
                PublicLibraryPaths.Add(ModuleDirectory + "/Lib/Android/ARMv7/gstreamer_runtime/gio/modules");
                PublicLibraryPaths.Add(ModuleDirectory + "/Lib/Android/ARMv7/gstreamer_runtime");



                var gstreamerSOLinks = new string[]
                 {

                    // @"gstreamer-1.0",
                    //@"gobject-2.0",
                    //@"glib-2.0",
                    //@"ffi",
                    //@"gmodule-2.0",
                    //@"intl",
                    //@"iconv"


                };

                var gstreamerStaticLinks = new string[]
                {

                };

                var gstreamerStaticCompletePathLinks = new string[]
                 {
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libz.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/liba52.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libass.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libavcodec.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libavfilter.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libavformat.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libavutil.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libbz2.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libcairo.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libcairo-gobject.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libcairo-script-interpreter.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libcharset.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libcroco-0.6.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libdca.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libdv.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libexpat.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libfaad.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libffi.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libFLAC.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libfontconfig.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libfreetype.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libfribidi.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgdk_pixbuf-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libges-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgio-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libglib-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgmodule-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgmp.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgnustl.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgnutls.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgobject-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgraphene-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstadaptivedemux-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstallocators-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstapp-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstaudio-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstbadaudio-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstbadbase-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstbadvideo-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstbase-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstbasecamerabinsrc-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstcheck-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstcodecparsers-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstcontroller-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstfft-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstgl-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstinsertbin-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstmpegts-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstnet-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstpbutils-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstphotography-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstplayer-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstreamer-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstriff-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstrtp-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstrtsp-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstrtspserver-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstsdp-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgsttag-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgsturidownloader-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstvalidate-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgstvideo-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libgthread-2.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libharfbuzz.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libhogweed.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libiconv.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libintl.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libjpeg.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libjson-glib-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libkate.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libmms.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libmp3lame.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libmpeg2.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libmpeg2convert.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libmpg123.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libnettle.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libnice.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libogg.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/liboggkate.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libopencore-amrnb.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libopencore-amrwb.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libopenh264.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libopus.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/liborc-0.4.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/liborc-test-0.4.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libpango-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libpangocairo-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libpangoft2-1.0.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libpixman-1.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libpng16.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/librsvg-2.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/librtmp.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libsbc.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libSoundTouch.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libsoup-2.4.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libspandsp.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libspeex.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libsrtp.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libsupc++.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libswresample.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libswscale.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtag.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtasn1.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtheora.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtheoradec.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtheoraenc.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libtiff.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libturbojpeg.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvisual-0.4.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvo-aacenc.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvorbis.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvorbisenc.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvorbisfile.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvorbisidec.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libvpx.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libwavpack.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libwebrtc_audio_processing.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libx264.a",
@"G:/gstreamer-1.0-android-arm-1.11.0/lib/libxml2.a"




                 };

#if USE_EXPLICIT_PATHS_FOR_LINKING
                linkLib(gstreamerStaticLinks,LinkTypes.Static,true);
                linkLib(gstreamerSOLinks,LinkTypes.Dynamic,true);
#else
                linkLib(gstreamerStaticLinks, LinkTypes.Static , false);
                linkLib(gstreamerSOLinks, LinkTypes.Dynamic,  false);
#endif
                linkCompletePathLib(gstreamerStaticCompletePathLinks, LinkTypes.Static);


            }



			
	
		}
		
      

    }
}
