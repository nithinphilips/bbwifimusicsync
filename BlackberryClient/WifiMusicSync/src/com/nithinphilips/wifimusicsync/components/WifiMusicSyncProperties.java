package com.nithinphilips.wifimusicsync.components;

import net.rim.device.api.synchronization.UIDGenerator;
import net.rim.device.api.system.*;
import net.rim.device.api.util.*;

public class WifiMusicSyncProperties implements Persistable
{

    public final static int         LOCAL_STORE_TYPE_MEMORY = 0;
    public final static int         LOCAL_STORE_TYPE_SDCARD = 1;

    public final static String      LOCAL_STORE_SD_PATH     = "file:///SDCard/Blackberry/music/WiFiSync/";
    public final static String      LOCAL_STORE_MEMORY_PATH = "file:///store/home/user/music/WiFiSync/";

    private String                  serverIp;
    private String                  serverPort;

    private String                  clientId;
    private String                  localStoreRoot;

    private String                  homeWifiName;

    // Hash of com.nithinphilips.wifimusicsync.components.WifiMusicSyncProperties
    private static final long       PERSISTENCE_ID          = 0x4fb6f443a08bf1f1L;

    // Persistent object wrapping the effective properties instance
    private static PersistentObject store;

    // Ensure that an effective properties set exists on startup.
    static
    {
        store = PersistentStore.getPersistentObject(PERSISTENCE_ID);
        synchronized (store)
        {
            if (store.getContents() == null)
            {
                store.setContents(new WifiMusicSyncProperties());
                store.commit();
            }
        }
    }

    // Constructs a properties set with default values.
    private WifiMusicSyncProperties()
    {
        serverIp = "";
        serverPort = "";
        homeWifiName = "";

        localStoreRoot = LOCAL_STORE_SD_PATH;
        clientId = Integer.toHexString(DeviceInfo.getDeviceId() + UIDGenerator.getUID());
    }

    // Canonical copy constructor.
    private WifiMusicSyncProperties(WifiMusicSyncProperties other)
    {
        serverIp = other.serverIp;
        serverPort = other.serverPort;
        homeWifiName = other.homeWifiName;

        localStoreRoot = other.localStoreRoot;
        clientId = other.clientId;
    }

    // Retrieves a copy of the effective properties set from storage.
    public static WifiMusicSyncProperties fetch()
    {
        WifiMusicSyncProperties savedProps = (WifiMusicSyncProperties) store.getContents();
        return new WifiMusicSyncProperties(savedProps);
    }

    // Causes the values within this instance to become the effective
    // properties for the application by saving this instance to the store.
    public void save()
    {
        store.setContents(this);
        store.commit();
    }

    public String getServerIp()
    {
        return serverIp;
    }

    public void setServerIp(String serverIp)
    {
        this.serverIp = serverIp;
    }

    public String getServerPort()
    {
        return serverPort;
    }

    public void setServerPort(String serverPort)
    {
        this.serverPort = serverPort;
    }

    public String getServerUrl()
    {
        return "http://" + serverIp + ":" + serverPort;
    }

    public String getClientId()
    {
        return clientId;
    }

    public void setClientId(String clientId)
    {
        this.clientId = clientId;
    }

    public String getLocalStoreRoot()
    {
        return localStoreRoot;
    }

    public void setLocalStoreRoot(String localStoreRoot)
    {
        if (localStoreRoot.endsWith("/")) this.localStoreRoot = localStoreRoot;
        else this.localStoreRoot = localStoreRoot + "/";
    }

    public int getLocalStoreType()
    {
        return localStoreRoot.equals(LOCAL_STORE_SD_PATH) ? LOCAL_STORE_TYPE_SDCARD : LOCAL_STORE_TYPE_MEMORY;
    }

    public String getHomeWifiName()
    {
        return homeWifiName;
    }

    public void setHomeWifiName(String homeWifiName)
    {
        this.homeWifiName = homeWifiName;
    }

}
