del ue4utils.jar
del .\stage\com\infusion\tailforgreg\*.class
javac -source %1 -target %1 .\stage\com\infusion\tailforgreg\FindServicesNSD.java -classpath g:\NVPACK\android-sdk-windows\platforms\android-19\android.jar
jar cvf ue4utils.jar -C .\stage .
xcopy /y .\ue4utils.jar G:\repos\AllUEThirdParty\ROS\Lib\Android\java\
G:\NVPACK\android-sdk-windows\build-tools\android-7.0\dx.bat --dex --output="test.apk" ue4utils.jar



