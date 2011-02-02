package com.nithinphilips.wifimusicsync;

import java.io.IOException;
import java.util.Enumeration;
import java.util.Vector;

import javax.microedition.content.ActionNameMap;
import javax.microedition.content.ContentHandler;
import javax.microedition.content.Registry;

import net.rim.device.api.system.ApplicationDescriptor;
import net.rim.device.api.system.ApplicationManager;
import net.rim.device.api.system.DeviceInfo;
import net.rim.device.api.system.RuntimeStore;
import net.rim.device.api.system.WLANInfo;
import net.rim.device.api.ui.UiApplication;

import org.json.me.JSONException;

import com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties;
import com.nithinphilips.wifimusicsync.controller.PlaylistDownloader;
import com.nithinphilips.wifimusicsync.controller.Subscriber;

public final class WifiMusicSync extends UiApplication
{

    public static void checkScheduleSync()
    {
        WifiMusicSyncProperties props = WifiMusicSyncProperties.fetch();
        // check available memory FileConnection.availableSize();

        // Check battery level
        if (((DeviceInfo.getBatteryStatus() & DeviceInfo.BSTAT_CHARGING) == DeviceInfo.BSTAT_CHARGING) || (DeviceInfo.getBatteryLevel() >= 10)) 
        // Check wifi status
        if (WLANInfo.getWLANState() == WLANInfo.WLAN_STATE_CONNECTED)
        {
            WLANInfo.WLANAPInfo apInfo = WLANInfo.getAPInfo();
            //if (apInfo != null) if (apInfo.getSSID() == props.getHomeWifiName()) sync();
        }

        // Schedule run
        long runInterval = 3600000; // 1 hr

        ApplicationDescriptor current = ApplicationDescriptor.currentApplicationDescriptor();
        current.setPowerOnBehavior(ApplicationDescriptor.DO_NOT_POWER_ON);
        ApplicationManager manager = ApplicationManager.getApplicationManager();
        manager.scheduleApplication(current, System.currentTimeMillis() + runInterval, true);

    }

    public static String[] getDirectories(String root, String path)
    {
        String subStr = path.substring(root.length());
        String[] parts = split(subStr, "/");
        String[] result = new String[parts.length];

        String last = root;
        for (int i = 0; i < parts.length; i++)
        {
            result[i] = last + parts[i] + "/";
            last = result[i];
        }

        return result;
    }

    protected static void log(String string)
    {
        // TODO Auto-generated method stub

    }

    /**
     * @param args
     */
    public static void main(String[] args)
    {

        // try {
        // System.out.println("---------------------------------------------");
        // PlaylistListResponse response = PlaylistListResponse.fromJson(new
        // JSONObject("{\"Tracks\":[{\"Name\":\"Library\",\"TrackCount\":23437},{\"Name\":\"Neethu\",\"TrackCount\":95}]}"));
        // System.out.println(response.toString());
        // System.out.println("---------------------------------------------");
        // } catch (JSONException e) {
        // // TODO Auto-generated catch block
        // e.printStackTrace();
        // }

        if (args != null && args.length > 0 && "sync".equals(args[0])) checkScheduleSync();
        else
        {
            WifiMusicSync theApp = new WifiMusicSync();
            theApp.enterEventDispatcher();
        }
    }

    public static String[] split(String inString, String delimeter)
    {
        String[] retAr = new String[0];
        try
        {
            Vector vec = new Vector();
            int indexA = 0;
            int indexB = inString.indexOf(delimeter);

            while (indexB != -1)
            {
                if (indexB > indexA) vec.addElement(new String(inString.substring(indexA, indexB)));
                indexA = indexB + delimeter.length();
                indexB = inString.indexOf(delimeter, indexA);
            }
            vec.addElement(new String(inString.substring(indexA, inString.length())));
            retAr = new String[vec.size()];
            for (int i = 0; i < vec.size(); i++)
                retAr[i] = vec.elementAt(i).toString();
        }
        catch (Exception e)
        {
            // sysout

        }
        return retAr;
    }

    public static Vector splitToVector(String inString, String delimeter)
    {
        Vector vec = new Vector();
        try
        {
            int indexA = 0;
            int indexB = inString.indexOf(delimeter);

            while (indexB != -1)
            {
                if (indexB > indexA) vec.addElement(new String(inString.substring(indexA, indexB)));
                indexA = indexB + delimeter.length();
                indexB = inString.indexOf(delimeter, indexA);
            }
            vec.addElement(new String(inString.substring(indexA, inString.length())));
        }
        catch (Exception e)
        {
            // sysout

        }
        return vec;
    }

    public WifiMusicSync()
    {
        // display a new screen
        pushScreen(new WifiMusicSyncScreen());
    }
}
