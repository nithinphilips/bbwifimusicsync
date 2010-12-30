package com.nithinphilips.wifimusicsync.model;

import net.rim.blackberry.api.pdap.BlackBerryContactList.AddressTypes;

import org.json.me.JSONException;
import org.json.me.JSONObject;

public class SyncAction {
	
	public static final int NONE = 0;
	public static final int ADD = 1;
	public static final int REMOVE = 2;
	
	int type = NONE;
	String deviceLocation;
	String trackPath;

	public int getType() {
		return type;
	}
	public String getDeviceLocation() {
		return deviceLocation;
	}
	public String getTrackUrl() {
		return trackPath;
	}
	
	public static SyncAction fromJson(JSONObject json, String serverUrl) throws JSONException{
		SyncAction result = new SyncAction();

		if(json.getString("Type").compareTo("Add") == 0){
			result.type = SyncAction.ADD;
			result.trackPath = serverUrl + json.getString("TrackPath");
		}else if(json.getString("Type").compareTo("Remove") == 0){
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
