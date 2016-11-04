// Fill out your copyright notice in the Description page of Project Se ttings.
//NOTE: You need androidlibs cloned and ANDROIDLIBS_ROOT and ANDROID_ROS_BOOST_LOCATION to whever that is
using UnrealBuildTool;
using System.IO;
using System;

public class ROS : ModuleRules
{
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
				PrivateIncludePaths.AddRange(
					new string[] {

								"../../../../Source/Runtime/Renderer/Private",
								"../../../../Source/Runtime/Launch/Private"
						}
					);

					


				PublicIncludePaths.AddRange(
							new string[] {
								@"C:\Program Files (x86)\Epic Games\4.12\Engine\Source\Runtime\Launch\Public\Android", //laptop on couch
								@"C:\Program Files\Epic Games\4.12\Engine\Source\Runtime\Launch\Public\Android", //home office machine
							 // "C:/Users/gbrill/Source/Repos/UnrealEngine/Engine/Source/Runtime/Core/Public/Android",
							  "../../../../Source/Runtime/Launch/Public/Android"

							}
							);
		}

        bUseRTTI = true; //oh so very important...lets boost dynamic cast return horror to the client. /GR
        UEBuildConfiguration.bForceEnableExceptions = true;

        //BOOST_REGEX_NO_EXTERNAL_TEMPLATES
        var ros_preproc = "OPENCV;_NO_FTDI;GREG1;BOOST_LIB_DIAGNOSTIC";// ;BOOST_LIB_DIAGNOSTIC";BOOST_TYPE_INDEX_FORCE_NO_RTTI_COMPATIBILITY;BOOST_NO_RTTI;BOOST_NO_TYPEID
        addPreproc(ros_preproc);

		
        if (Target.Platform == UnrealTargetPlatform.Android || Target.Platform == UnrealTargetPlatform.Win64)
        {

           	//includeAdd("BOOST_162_INCLUDE");
			PublicIncludePaths.Add(Environment.GetEnvironmentVariable("BOOST_162_INCLUDE")); 
            includeAdd("ROS_JADE_INCLUDE_PATHS");
			

		    

        }

		
		   if (Target.Platform == UnrealTargetPlatform.Win64)
        {
			
			Console.WriteLine("^In Win64 Build/Single DLL ROSWindowsJade.dll");
			PublicLibraryPaths.Add("BOOST_162_64_LIB");
			
			PublicLibraryPaths.Add(Environment.GetEnvironmentVariable("BOOST_162_64_LIB")); //boost will automatically bring in static libs. 
        

			//ModuleDirectory + \x64\Rel-64-15\rosjadecpp-r-2015.lib"
			string lib=ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSWindowsJade.lib";
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSWindowsJade.lib");
			Console.WriteLine(lib);
            
            string fname = Path.Combine(ModuleDirectory + @"\bin\x64\ROSWindowsJade.dll");
            PublicDelayLoadDLLs.Add(fname);
            RuntimeDependencies.Add(new RuntimeDependency(fname));
			RuntimeDependencies.Add(new RuntimeDependency(Path.Combine(ModuleDirectory + @"\bin\Windows\x64\ROSWindowsJade.dll")));
			


        }
		/*
		
 //this is the old and working windows section, but we are going to start trying to work with that single dll like android. 
        if (Target.Platform == UnrealTargetPlatform.Win64)
        {
			
			Console.WriteLine("^In Win64 Build");
			
	//		includeAdd("BOOST_160_INCLUDE");			
			PublicLibraryPaths.Add(Environment.GetEnvironmentVariable("BOOST_162_64_LIB")); //boost will automatically bring in static libs. 
			//PublicLibraryPaths.Add(@"G:\include\boost_1_62_0\boost_1_62_0\stage\lib_BOOST_TYPE_INDEX_FORCE_NO_RTTI_COMPATIBILITY\lib");

			//ModuleDirectory + \x64\Rel-64-15\rosjadecpp-r-2015.lib"
			
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\rosjadecpp-r-2015.lib");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSJadeInterop-r-2015.lib"); 
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\ROSActionlib-r-2015.lib");
            //Notes: rebuild with tf2 is already defined collisions, all hell breaks loose
            //rebuild with tf is ok, and if you reference a tf item, you get tf2 missing items because tf calls tf2.
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\TFShared-r-2015.lib");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\Lib\Windows\x64\Rel-64-15\rosbag-r-2015.lib");
            //Console.WriteLine("**"+Path.Combine(ModuleDirectory + @"\bin\x64\rosjadecpp-r-2015.dll"));

            string fname = Path.Combine(ModuleDirectory + @"\bin\x64\rosjadecpp-r-2015.dll");
            PublicDelayLoadDLLs.Add(fname);
            RuntimeDependencies.Add(new RuntimeDependency(fname));
			RuntimeDependencies.Add(new RuntimeDependency(Path.Combine(ModuleDirectory + @"\bin\x64\ROSJadeInterop-r-2015.dll")));
			RuntimeDependencies.Add(new RuntimeDependency(Path.Combine(ModuleDirectory + @"\bin\x64\ROSActionlib-r-2015.dll")));
			RuntimeDependencies.Add(new RuntimeDependency(Path.Combine(ModuleDirectory + @"\bin\x64\TFShared-r-2015.dll")));
            RuntimeDependencies.Add(new RuntimeDependency(Path.Combine(ModuleDirectory + @"\bin\x64\rosbag-r-2015.dll")));


        }

*/
		if (Target.Platform == UnrealTargetPlatform.Android)
        {
			Console.WriteLine("^^^^Android ROS section started.........");

			
			string ModDir = UEBuildConfiguration.UEThirdPartySourceDirectory + "ROS/";
			
			Console.WriteLine("^Make sure ANDROIDLIBS_ROOT and EPIC_INSTALL are present as EnVars");
			
			//includeAdd("BOOST_162_INCLUDE");
           // includeAdd("ROS_JADE_INCLUDE_PATHS");
			
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

			
	
		}
		
        if (false) //the old android stuff for reference
        {
			Console.WriteLine("!!!!!!!!!!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@##########################");

			Console.WriteLine("^Make sure ANDROIDLIBS_ROOT and EPIC_INSTALL are present as EnnVars");

            includeAdd("ANDROIDLIBS_ROOT");
            string root = Environment.GetEnvironmentVariable("ANDROIDLIBS_ROOT");
			string epic_install = Environment.GetEnvironmentVariable("EPIC_INSTALL");
			string epic_android_path=Path.Combine(epic_install,@"Engine\Source\Runtime\Launch\Public\Android\");
			
			PublicIncludePaths.Add(epic_android_path);
			

            PublicIncludePaths.Add(Path.Combine(root, @"\boost-android\boost_1_55_0"));
            PublicIncludePaths.Add(Path.Combine(root, @"ros\install_isolated\include"));
            PublicIncludePaths.Add(Path.Combine(root, @"boost-android\boost_1_55_0"));
                                            //            ros\install_isolated\include\include_generated\rosclientinterop
            PublicIncludePaths.Add(Path.Combine(root, @"ros\install_isolated\include\include_generated\rosclientinterop"));
            PublicIncludePaths.Add(Path.Combine(root, @"ros-android-src\ros-android\ros\src\geometry"));
            PublicIncludePaths.Add(Path.Combine(root, @"ros\install_isolated\include"));



            string PluginPath = Utils.MakePathRelativeTo(ModuleDirectory, BuildConfiguration.RelativeEnginePath);
            AdditionalPropertiesForReceipt.Add(new ReceiptProperty("AndroidPlugin", Path.Combine(PluginPath, "GameActivityInsert.xml"))); //../../../../repos/UDPUnreal\Source\UDPSendReceive\GregAndroidTest1.xml
            Console.WriteLine("^Path {0}:", Path.Combine(PluginPath, "GameActivityInsert.xml"));



            string p;


            p = ModuleDirectory + @"\Lib\armeabi-v7a\libgregtest2.so";
            PublicAdditionalLibraries.Add(p);

            //libconsole_bridge.so libcpp_common.so librosconsole.so librosconsole_backend_interface.so librosconsole_print.so libroscpp.so libroscpp_serialization.so librostime.so libxmlrpcpp.so

            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libconsole_bridge.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libcpp_common.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\librosconsole.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\librosconsole_backend_interface.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\librosconsole_print.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libroscpp.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libroscpp_serialization.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\librostime.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libxmlrpcpp.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libROSTF_Android.so");
            PublicAdditionalLibraries.Add(ModuleDirectory + @"\lib\armeabi-v7a\libRosInteropAndroid.so");



            //PublicAdditionalLibraries.Add(@"C:\Users\gbril\sources\repos\android\ROS\Source\ROS\libgregtest2.so");
            //PublicAdditionalLibraries.Add(@"G:\androidlibs\ros\install_isolated\lib\libroscpp.so");

        }

    }
}
