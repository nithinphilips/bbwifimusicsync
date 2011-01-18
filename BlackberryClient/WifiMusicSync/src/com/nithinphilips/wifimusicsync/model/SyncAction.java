package com.nithinphilips.wifimusicsync.model;

import net.rim.blackberry.api.pdap.BlackBerryContactList.AddressTypes;

import org.json.me.JSONException;
import org.json.me.JSONObject;

import com.nithinphilips.wifimusicsync.components.SyncActionChangedCallBack;

public class SyncAction {
	
	public static final int NONE = 0;
	public static final int ADD = 1;
	public static final int REMOVE = 2;
	
	SyncActionChangedCallBack notifier;
	
	public SyncActionChangedCallBack getNotifier() {
		return notifier;
	}
	public void setNotifier(SyncActionChangedCallBack notifier) {
		this.notifier = notifier;
	}

	int index = 0;

	public int getIndex() {
		return index;
	}
	public void setIndex(int index) {
		this.index = index;
	}

	int type = NONE;
	String deviceLocation;
	
	public void setType(int type) {
		this.type = type;
	}
	public void setDeviceLocation(String deviceLocation) {
		this.deviceLocation = deviceLocation;
	}
	
	public String getFileName(){
		int index = deviceLocation == null ? -1 : deviceLocation.lastIndexOf('/');
		
		if(index >= 0){
			return deviceLocation.substring(index + 1);
		}else{
			return deviceLocation;
		}
	}
	
	public void setTrackUrl(String trackUrl) {
		this.trackPath = trackUrl;
	}
	

	String trackPath;
	String status = "Queued";

	public int getType() {
		return type;
	}
	public String getDeviceLocation() {
		return deviceLocation;
	}
	public String getTrackUrl() {
		return trackPath;
	}
	
	public static SyncAction fromJson(JSONObject json, UrlBuilder server) throws JSONException{
		SyncAction result = new SyncAction();

		if(json.getString("Type").compareTo("Add") == 0){
			result.type = SyncAction.ADD;
			result.trackPath = server.getFullUrl(json.getString("TrackPath"));
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
	
	public String getStatus() {		
		return status;
	}
	
	public void setStatus(String status) {
		this.status = status;
		if(notifier != null) notifier.changed(this);
	}
}
