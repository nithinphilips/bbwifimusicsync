package com.nithinphilips.wifimusicsync.model;

import org.json.me.*;

import com.nithinphilips.wifimusicsync.components.SyncActionChangedCallBack;

public class SyncAction
{

    public static final int   NONE   = 0;
    public static final int   ADD    = 1;
    public static final int   REMOVE = 2;

    int                       index  = 0;
    int                       type   = NONE;
    String                    deviceLocation;
    String                    trackPath;
    String                    status = "Pending";
    SyncActionChangedCallBack notifier;

    public String getDeviceLocation()
    {
        return deviceLocation;
    }

    public String getFileName()
    {
        int index = deviceLocation == null ? -1 : deviceLocation.lastIndexOf('/');

        if (index >= 0)
        {
            return deviceLocation.substring(index + 1);
        }
        else
        {
            return deviceLocation;
        }
    }

    public int getIndex()
    {
        return index;
    }

    public SyncActionChangedCallBack getNotifier()
    {
        return notifier;
    }

    public String getStatus()
    {
        return status;
    }

    public String getTrackUrl()
    {
        return trackPath;
    }

    public int getType()
    {
        return type;
    }

    public void setDeviceLocation(String deviceLocation)
    {
        this.deviceLocation = deviceLocation;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public void setNotifier(SyncActionChangedCallBack notifier)
    {
        this.notifier = notifier;
    }

    public void setStatus(String status)
    {
        this.status = status;
        if (notifier != null) notifier.changed(this);
    }

    public void setTrackUrl(String trackUrl)
    {
        this.trackPath = trackUrl;
    }

    public void setType(int type)
    {
        this.type = type;
    }

    public static SyncAction fromJson(JSONObject json, UrlBuilder server) throws JSONException
    {
        SyncAction result = new SyncAction();

        if (json.getString("Type").compareTo("Add") == 0)
        {
            result.type = SyncAction.ADD;
            result.trackPath = server.getFullUrl(json.getString("TrackPath"));
        }
        else if (json.getString("Type").compareTo("Remove") == 0)
        {
            result.type = SyncAction.REMOVE;
        }

        result.deviceLocation = json.getString("DeviceLocation");
        return result;
    }

    public String toString()
    {
        StringBuffer sb = new StringBuffer();
        sb.append(type == ADD ? "Add" : "Remove");
        sb.append(": \"");
        sb.append(deviceLocation);
        sb.append("\" \"");
        sb.append(trackPath);
        sb.append("\"");
        return sb.toString();

    }
}
