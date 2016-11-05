/*
 * Created by VisualGDB. Based on hello-jni example.
 * Copyright (C) 2009 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.

 //some info but not all correct: http://apple.stackexchange.com/questions/175241/how-can-i-list-the-ip-addresses-of-all-the-airprint-printers-on-a-network

 registration example, do this on windows, but may not resolve:
 dns-sd -R MyWebsite _http._tcp local 80

 //a complete working publishing tested on windows and picked up on using this
 dns-sd -P ROSCore _http._tcp local 80 roscore.local 10.1.55.163

 browse:
 dns-sd -B _http._tcp local

 lookup: but gives no up
 dns-sd  -L  "ROSCore"  _http._tcp  local.

 */
package com.Infusion.TailForGreg;

	import android.net.nsd.NsdManager;
    import android.net.nsd.NsdManager.DiscoveryListener;
    import android.net.nsd.NsdManager.ResolveListener;
    import android.net.nsd.NsdServiceInfo;
    import android.util.Log;
    import java.util.HashMap;
	 import android.text.TextUtils;

    public final class FindServicesNSD
                implements
                    DiscoveryListener,
                    ResolveListener
    {
        // DiscoveryListener
        // DiscoveryListener
	
		public static HashMap<String, String> servicesMap = new HashMap<String, String>();
		//public native void setENV();

        @Override
        public void onDiscoveryStarted(String theServiceType)
        {
            Log.d(TAG, "onDiscoveryStarted");
        }
    
        @Override
        public void onStartDiscoveryFailed(String theServiceType, int theErrorCode)
        {
            Log.d(TAG, "onStartDiscoveryFailed(" + theServiceType + ", " + theErrorCode);
        }
	
        @Override
        public void onDiscoveryStopped(String serviceType)
        {
            Log.d(TAG, "onDiscoveryStopped");
        }
	
        @Override
        public void onStopDiscoveryFailed(String theServiceType, int theErrorCode)
        {
            Log.d(TAG, "onStartDiscoveryFailed(" + theServiceType + ", " + theErrorCode);
        }
	
        @Override
        public void onServiceFound(NsdServiceInfo theServiceInfo)
        {
            Log.d(TAG, "onServiceFound(" + theServiceInfo + ")");
            Log.d(TAG, "name == " + theServiceInfo.getServiceName());
            Log.d(TAG, "type == " + theServiceInfo.getServiceType());
            serviceFound(theServiceInfo);
        }
    
        @Override
        public void onServiceLost(NsdServiceInfo theServiceInfo)
        {
            Log.d(TAG, "onServiceLost(" + theServiceInfo + ")");
        }
    
        // Resolve Listener
    
        @Override
        public void onServiceResolved(NsdServiceInfo theServiceInfo)
        {
            Log.d(TAG, "onServiceResolved(" + theServiceInfo + ")");
            Log.d(TAG, "name == " + theServiceInfo.getServiceName());
            Log.d(TAG, "type == " + theServiceInfo.getServiceType());
            Log.d(TAG, "host == " + theServiceInfo.getHost());
            Log.d(TAG, "port == " + theServiceInfo.getPort());
	    }
	
        @Override
        public void onResolveFailed(NsdServiceInfo theServiceInfo, int theErrorCode)
        {
            Log.d(TAG, "onResolveFailed(" + theServiceInfo + ", " + theErrorCode);
        }
    
        //
	
        public FindServicesNSD(NsdManager theManager, String theServiceType)
        {
            manager     = theManager;
            serviceType = theServiceType;
        }
	
		public static String getResolution(String name)
		{

			try
			{			
				String str= (String)servicesMap.get(name).toString();
				if( TextUtils.isEmpty(str) )
				{
					return "NOTHING";
				}
				else
				{
					return str;
				}

			} 
			catch (Exception e)
			{
				return "NOTHING";
			}

			

		}

        public void run()
        {

			//servicesMap.put("ROSCore","111.222.333.444"); //debug value
			//setENV(); //native call to set environment and object variables
			Log.d("FindServicesNSD", "^Run");
            manager.discoverServices(serviceType, NsdManager.PROTOCOL_DNS_SD, this);
        }
	
        private void serviceFound(NsdServiceInfo theServiceInfo)
        {
            //manager.resolveService(theServiceInfo, this);
			manager.resolveService(theServiceInfo, 
					 new NsdManager.ResolveListener() {
					@Override
						public void onResolveFailed(NsdServiceInfo serviceInfo, int errorCode)
						{
							Log.e(TAG, "Resolve Failed: " + serviceInfo);
						}
					
						@Override
						public void onServiceResolved(NsdServiceInfo theServiceInfo) 
						{
							  Log.d(TAG, "onServiceResolved(" + theServiceInfo + ")");
								Log.d(TAG, "name == " + theServiceInfo.getServiceName());
								Log.d(TAG, "type == " + theServiceInfo.getServiceType());
								Log.d(TAG, "host == " + theServiceInfo.getHost());
								Log.d(TAG, "port == " + theServiceInfo.getPort());

								Log.d(TAG, "Place in servicesMap the name and host you see here...host:"+theServiceInfo.getHost());
								servicesMap.put(theServiceInfo.getServiceName().toString(),theServiceInfo.getHost().toString());
								
								String s=(String)servicesMap.get(theServiceInfo.getServiceName().toString());
								Log.d(TAG, "Test pull from map:"+s);

						}
					
					}
			
			
			
					);
        }
	
        //
	
        private NsdManager   manager;
        private String       serviceType;
	
        //
	
        private static final String TAG = "FindServicesNSD";


    }